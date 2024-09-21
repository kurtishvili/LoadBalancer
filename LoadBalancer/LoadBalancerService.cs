using Microsoft.Extensions.Options;

namespace LoadBalancer
{
    public class LoadBalancerService
    {
        private readonly LoadBalancerSettings _settings;
        private readonly HttpClient _httpClient;
        private int _currentIndex;
        private List<string> _servers;
        private readonly object _lock = new();

        public LoadBalancerService(IOptions<LoadBalancerSettings> options, HttpClient httpClient)
        {
            _settings = options.Value;
            _httpClient = httpClient;
            _currentIndex = _settings.CurrentIndex;
            _servers = _settings.Servers;
        }

        public async Task<string> GetNextActiveServer()
        {
            var activeServers = new List<string>();

            foreach (var server in _servers)
            {
                var response = await _httpClient.GetAsync($"{server}/health");

                if (response.IsSuccessStatusCode)
                {
                    activeServers.Add(server);
                }
            }

            if (activeServers.Count == 0)
            {
                throw new InvalidOperationException("No active servers available");
            }

            string selectedServer;
            
            lock (_lock)
            {
                selectedServer = activeServers[_currentIndex];
                _currentIndex = (_currentIndex + 1) % activeServers.Count;
            }

            return selectedServer;
        }
    }
}

namespace LoadBalancer
{
    public class LoadBalancerService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private int _currentIndex = 0;
        private List<string> server = new List<string> { "http://fb.com", "http://www.linkedin.com/", "http://github.com/" };

        public LoadBalancerService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }   
        public async Task<string> GetNextActiveServerAsync()
        {
            var activeServers = new List<string>();

            foreach (var server in server)
            {
                var response = await _httpClient.GetAsync($"{server}/health");

                if (response.IsSuccessStatusCode)
                    activeServers.Add(server);
            }

            if (activeServers.Count == 0)
                throw new InvalidOperationException("No active servers available.");

            var selectedServer = activeServers[_currentIndex];
            _currentIndex = (_currentIndex + 1) % activeServers.Count;

            return selectedServer;
        }
    }
}

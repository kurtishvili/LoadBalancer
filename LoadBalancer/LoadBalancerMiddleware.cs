namespace LoadBalancer
{
    public class LoadBalancerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoadBalancerService _loadBalancer;

        public LoadBalancerMiddleware(RequestDelegate next, LoadBalancerService loadBalancer)
        {
            _next = next;
            _loadBalancer = loadBalancer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var serverUrl = await _loadBalancer.GetNextActiveServerAsync();

            context.Response.Redirect(serverUrl);
            await _next(context);
        }
    }

}

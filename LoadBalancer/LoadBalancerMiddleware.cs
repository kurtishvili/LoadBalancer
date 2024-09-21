namespace LoadBalancer
{
    public class LoadBalancerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoadBalancerService _loadBalancerService;

        public LoadBalancerMiddleware(RequestDelegate next, LoadBalancerService loadBalancerService)
        {
            _next = next;
            _loadBalancerService = loadBalancerService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var server = await _loadBalancerService.GetNextActiveServer();

            context.Response.Redirect(server);
            await _next(context);
        }
    }
}

namespace CreditCardManager.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} at {DateTime.Now}");

            await _next(context);

            Console.WriteLine($"Response: {context.Response.StatusCode} at {DateTime.Now}");
        }
    }
}
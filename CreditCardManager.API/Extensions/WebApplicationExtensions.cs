using CreditCardManager.Middlewares;

namespace CreditCardManager.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCreditCardManagerApp(this WebApplication app)
        {
            app.UseMiddleware<LogMiddleware>();
            app.MapControllers();
            app.UseHttpsRedirection();

            return app;
        }
    }
}
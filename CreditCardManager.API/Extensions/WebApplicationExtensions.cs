using CreditCardManager.Middlewares;

namespace CreditCardManager.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCreditCardManagerApp(this WebApplication app)
        {
            app.UseCors();

            app.MapControllers();

            app.UseHttpsRedirection();

            return app;
        }

        public static WebApplication UseDevelopmentTools(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
                app.UseMiddleware<LogMiddleware>();
            }

            return app;
        }
    }
}
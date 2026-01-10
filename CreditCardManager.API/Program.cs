using Microsoft.EntityFrameworkCore;

using CreditCardManager.Extensions;
using CreditCardManager.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices(builder.Configuration).AddDbServices(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCreditCardManagerApp();
app.Run();
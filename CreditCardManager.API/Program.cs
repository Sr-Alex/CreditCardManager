using CreditCardManager.Extensions;
using CreditCardManager.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration).AddDbServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseCreditCardManagerApp();
app.UseDevelopmentTools();

app.Run();
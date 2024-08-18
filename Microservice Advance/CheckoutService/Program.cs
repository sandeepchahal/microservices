using System.Configuration;
using CheckoutService;
using CheckoutService.Coordinator;
using CheckoutService.ServiceImplementations;
using Consul;
using JWTConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service", Version = "v1" });
});

var consulHost = builder.Configuration.GetValue<string>("ConsulConfiguration:Host");

if (string.IsNullOrEmpty(consulHost))
    throw new ConfigurationErrorsException($"Consul Configuration Host is not found in the configuration");

builder.Services.AddSingleton<IConsulClient>(_ => new ConsulClient(config =>
{
    config.Address = new Uri(consulHost);
}));

builder.Services.AddHttpClient("ProductDetailServiceClient", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Services:ProductDetail:BaseUrl"];
    if (baseUrl is null)
        throw new ConfigurationErrorsException("Base Url is not found");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient("CartServiceClient", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Services:Cart:BaseUrl"];
    if (baseUrl is null)
        throw new ConfigurationErrorsException("Base Url is not found");
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtConfigurationProvider.GetTokenValidationParameters();
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IHostedService, ConsulRegistrationService>();
builder.Services.AddSingleton<IMessageService, MessageServiceAction>();
builder.Services.AddScoped<ICheckOutCoordinator, CheckOutCoordinator>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.Urls.Add("http://0.0.0.0:80"); 
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
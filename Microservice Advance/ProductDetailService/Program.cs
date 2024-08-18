using System.Configuration;
using Consul;
using JWTConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ProductDetailService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Detail Service", Version = "v1" });
});

builder.Services.AddSingleton<IHostedService, ConsulRegistrationService>();

var consulHost = builder.Configuration.GetValue<string>("ConsulConfiguration:Host");

if (string.IsNullOrEmpty(consulHost))
    throw new ConfigurationErrorsException($"Consul Configuration Host is not found in the configuration");

builder.Services.AddSingleton<IConsulClient>(_ => new ConsulClient(config =>
{
    config.Address = new Uri(consulHost);
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtConfigurationProvider.GetTokenValidationParameters();
    });


builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseMiddleware<MyLogger>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
using System.Configuration;
using Consul;
using Microsoft.OpenApi.Models;
using UserService;
using UserService.ServiceRegistrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service", Version = "v1" });
});


builder.Services.AddSingleton<IHostedService, ConsulRegistrationService>();

var consulHost = builder.Configuration.GetValue<string>("ConsulConfiguration:Host");

if (string.IsNullOrEmpty(consulHost))
    throw new ConfigurationErrorsException($"Consul Configuration Host is not found in the configuration");

builder.Services.AddSingleton<IConsulClient>(_ => new ConsulClient(config =>
{
    config.Address = new Uri(consulHost);
}));

builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1"));
}
else
{
    app.Urls.Add("http://0.0.0.0:80"); 
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHostedService, MessageBroker>();
var app = builder.Build();

app.Run();
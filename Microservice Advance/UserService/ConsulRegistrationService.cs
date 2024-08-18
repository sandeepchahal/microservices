using Consul;

namespace UserService;

public class ConsulRegistrationService(IConsulClient consulClient, IConfiguration configuration) : IHostedService
{
    private string _serviceId = string.Empty;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = configuration.GetValue<string>("ServiceConfiguration:ServiceName");
        var host =configuration.GetValue<string>("ServiceConfiguration:Host");
        var port =configuration.GetValue<string>("ServiceConfiguration:Port");
        _serviceId = $"{serviceName}-{Guid.NewGuid()}";
        var tagList = new string[] { serviceName??"" };
        
        var register = new AgentServiceRegistration()
        {
            ID = _serviceId,
            Name = serviceName,
            Port = Convert.ToInt32(port),
            Address = host,
            Tags = tagList
        };

        await consulClient.Agent.ServiceRegister(register, cancellationToken);

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
    }
}
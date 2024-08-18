using CheckoutService.Enums;
using System.Text.Json;
using RabbitMQ.Client;
namespace CheckoutService.ServiceImplementations;

public class MessageServiceAction:IMessageService
{
    private readonly IModel _model;
    private const string ExchangeName = "order-exchange";
    public MessageServiceAction()
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST")??"localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")??"guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")??"guest"
        };

         var connection = factory.CreateConnection();
        _model = connection.CreateModel();
        _model.ExchangeDeclare(ExchangeName, "topic", false, false, null);
    }
    public void PublishSuccessMessage(object message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);
            var basicProperties = _model.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            _model.BasicPublish(
                ExchangeName,
                "order.created", 
                basicProperties, 
                body);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public void PublishFailedMessage(object message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);
            var basicProperties = _model.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            _model.BasicPublish(
                ExchangeName,
                "order.failed", 
                basicProperties, 
                body);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
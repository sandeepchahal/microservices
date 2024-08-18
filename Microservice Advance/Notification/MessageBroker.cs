using System.Text;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notification;

public class MessageBroker : IHostedService
{
    private readonly IConnection _connection;
    private readonly IModel _model;
    private const string QueueName = "order-creation-queue";
    private const string ExchangeName = "order-exchange";
    private const string FailureQueueName = "order-failure-queue";

    public MessageBroker()
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST")??"localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")??"guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")??"guest"
        };

        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();
        _model.ExchangeDeclare(ExchangeName, "topic", false, false, null);
        _model.QueueDeclare(QueueName, false, false, false, null);
        _model.QueueDeclare(FailureQueueName, false, false, false, null);
        _model.QueueBind(QueueName, ExchangeName, "order.created", null);
        _model.QueueBind(FailureQueueName, ExchangeName, "order.failed", null);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ConsumeOrderSuccessNotifications();
        ConsumeFailureMessages();
    }

    private void ConsumeOrderSuccessNotifications()
    {
        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var orderNotification = JsonConvert.DeserializeObject<OrderNotification>(message);
            if (orderNotification != null)
                ProcessOrderNotification(orderNotification);
            else
                Console.WriteLine("Order items are empty");
        };

        _model.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
    }

    private void ConsumeFailureMessages()
    {
        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Error: - {message}");
        };

        _model.BasicConsume(queue: FailureQueueName, autoAck: true, consumer: consumer);
    }

    private void ProcessOrderNotification(OrderNotification orderNotification)
    {
        Console.WriteLine($"User Id: {orderNotification.UserId}");
        Console.WriteLine($"Received order notification for Order ID: {orderNotification.OrderId}");
        Console.WriteLine($"Status: {orderNotification.Status}");
        Console.WriteLine("Products Information:");
        foreach (var product in orderNotification.Products)
        {
            Console.WriteLine($"- Product ID: {product.ProductId}, Quantity: {product.Quantity}");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _connection.Close();
        _model.Close();
    }
}
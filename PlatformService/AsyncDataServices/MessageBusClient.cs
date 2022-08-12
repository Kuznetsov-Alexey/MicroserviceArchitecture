using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private const string ExchangeName = "trigger";
    
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _chanel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMqHost"],
            Port = int.Parse(_configuration["RabbitMqPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _chanel = _connection.CreateModel();
            
            _chanel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            
            Console.WriteLine("--> Connected to MessageBus");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }
    
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMq Connection Open, Sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMq Connection Closed, not sending");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        _chanel.BasicPublish(exchange: ExchangeName, 
            routingKey: "",
            basicProperties: null,
            body: body);
        
        Console.WriteLine($"--> We have sent a message: {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> MessageBus disposed");

        if (_chanel.IsOpen)
        {
            _chanel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMq Connection Shutdown");
    }
}
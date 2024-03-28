using System.Text;
using RabbitMQ.Client;

namespace PeakVentures.Task.PixelService;

public interface IMessagePublisher
{
    /// <summary>
    /// Publishes message with the visitor information to the message queue
    /// </summary>
    /// <param name="referer">Visitor referer</param>
    /// <param name="userAgent">Visitor user agent</param>
    /// <param name="ipAddress">Visitor IP address</param>
    void PublishMessage(string? referer, string? userAgent, string? ipAddress);
}

public class MessagePublisher : IMessagePublisher
{
    private const string DefaultQueueName = "visits";
    private const string EmptyStringSubstitution = "null";
    
    private readonly ILogger<MessagePublisher> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName;
    
    public MessagePublisher(ILogger<MessagePublisher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _queueName = configuration.GetValue<string>("MessageQueue:Queue") ?? DefaultQueueName;
        _connectionFactory = CreateConnectionFactory(configuration);
    }
    
    /// <inheritdoc/>
    public void PublishMessage(string? referer, string? userAgent, string? ipAddress)
    {
        string SubstituteEmptyString(string? source) => string.IsNullOrEmpty(source) ? EmptyStringSubstitution : source;

        if (string.IsNullOrEmpty(ipAddress))
        {
            throw new ArgumentNullException(nameof(ipAddress), "Visitor IP Address should be specified");
        }
        
        var message =
            $"{DateTime.UtcNow:O}|{SubstituteEmptyString(referer)}|{SubstituteEmptyString(userAgent)}|{ipAddress}";
        var messageBody = Encoding.UTF8.GetBytes(message);

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicPublish(exchange: string.Empty,
                routingKey: _queueName,
                basicProperties: null,
                body: messageBody);

            _logger.LogInformation("Message {message} was published to the message queue", message);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured while publishing message {message} to the message queue: {error}",
                message, ex);
        }
    }

    private static ConnectionFactory CreateConnectionFactory(IConfiguration configuration)
    {
        var mqHost = configuration.GetValue<string>("MessageQueue:Host");
        var mqPort = configuration.GetValue<int>("MessageQueue:Port");

        return new ConnectionFactory { HostName = mqHost, Port = mqPort };
    }
}
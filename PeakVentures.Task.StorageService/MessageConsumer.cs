using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PeakVentures.Task.StorageService;

public interface IMessageConsumer
{
    System.Threading.Tasks.Task ReadMessages();
}

public class MessageConsumer : IMessageConsumer, IDisposable
{
    private const string DefaultLogFilePath = "/tmp/visits.log";
    private const string DefaultQueueName = "visits";
    
    private readonly IModel _model;
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MessageConsumer> _logger;
    
    private readonly string _queueName;
    private readonly string _logFilePath;
    
    public MessageConsumer(IConfiguration configuration, ILogger<MessageConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connection = CreateChannel(configuration);
        _model = _connection.CreateModel();
        
        _queueName = configuration.GetValue<string>("MessageQueue:Queue") ?? DefaultQueueName;
        _logFilePath = _configuration.GetValue<string>("FilePath") ?? DefaultLogFilePath;
        
        _model.QueueDeclare(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public async System.Threading.Tasks.Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (ch, ea) =>
        {
            _logger.LogDebug("Receiving message from the message queue");
            WriteVisitorLogToFile(ea.Body.ToArray());
            
            await System.Threading.Tasks.Task.CompletedTask;
            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume(_queueName, false, consumer);
        await System.Threading.Tasks.Task.CompletedTask;
    }
    
    private void WriteVisitorLogToFile(byte[] messageBody)
    {
        var locker = new object();
        
        var message = Encoding.UTF8.GetString(messageBody);
        _logger.LogInformation("Writing message '{message}' to the log file", message);
        
        lock (locker)
        {
            File.AppendAllLines(_logFilePath, new[] { message });
        }
    }
    
    private static IConnection CreateChannel(IConfiguration configuration)
    {
        var mqHost = configuration.GetValue<string>("MessageQueue:Host");
        var mqPort = configuration.GetValue<int>("MessageQueue:Port");
        
        var connection = new ConnectionFactory
        {
            HostName = mqHost,
            Port = mqPort,
            DispatchConsumersAsync = true
        };
        var channel = connection.CreateConnection();
        return channel;
    }
    
    public void Dispose()
    {
        if (_model.IsOpen)
            _model.Close();

        if (_connection.IsOpen)
            _connection.Close();
    }
}
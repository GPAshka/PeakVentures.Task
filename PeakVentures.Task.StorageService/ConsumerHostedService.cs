namespace PeakVentures.Task.StorageService;

public class ConsumerHostedService : BackgroundService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IMessageConsumer _messageConsumer;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IMessageConsumer messageConsumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Storage service is waiting for incoming messages");
        
        await _messageConsumer.ReadMessages();
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PeakVentures.Task.PixelService;

namespace PeakVentures.Task.Tests;

public class MessagePublisherTests
{
    private readonly IMessagePublisher _messagePublisher;
    
    public MessagePublisherTests()
    {
        var configuration = BuildConfiguration();
        var logger = new Logger<MessagePublisher>(new NullLoggerFactory());
        _messagePublisher = new MessagePublisher(logger, configuration);
    }
    
    [Fact]
    public void TestConstructMessageIpAddressEmpty()
    {
        const string referer = "referer";
        const string userAgent = "userAgent";
        const string ipAddress = "";

        Assert.Throws<ArgumentNullException>(() => _messagePublisher.ConstructMessage(referer, userAgent, ipAddress));
    }
    
    [Fact]
    public void TestConstructMessageRefererEmpty()
    {
        const string referer = "";
        const string userAgent = "userAgent";
        const string ipAddress = "127.0.0.1";

        var actualMessage = _messagePublisher.ConstructMessage(referer, userAgent, ipAddress);
        var splitMessage = actualMessage.Split("|", StringSplitOptions.RemoveEmptyEntries);
        
        // Check referer from the constructed message 
        Assert.Equal("null", splitMessage[1]);
    }
    
    [Fact]
    public void TestConstructMessageUserAgentEmpty()
    {
        const string referer = "referer";
        const string userAgent = "";
        const string ipAddress = "127.0.0.1";

        var actualMessage = _messagePublisher.ConstructMessage(referer, userAgent, ipAddress);
        var splitMessage = actualMessage.Split("|", StringSplitOptions.RemoveEmptyEntries);
        
        // Check user agent from the constructed message 
        Assert.Equal("null", splitMessage[2]);
    }
    
    [Fact]
    public void TestConstructMessageAllVariablesSet()
    {
        const string referer = "referer";
        const string userAgent = "userAgent";
        const string ipAddress = "127.0.0.1";

        var actualMessage = _messagePublisher.ConstructMessage(referer, userAgent, ipAddress);
        
        // Check user agent from the constructed message 
        Assert.DoesNotContain(MessagePublisher.EmptyStringSubstitution, actualMessage);
    }

    private static IConfiguration BuildConfiguration()
    {
        var myConfiguration = new Dictionary<string, string?>
        {
            {"MessageQueue:Host", "localhost"},
            {"MessageQueue:Port", "5672"},
            {"MessageQueue:Queue", "visits"},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        return configuration;
    }
}
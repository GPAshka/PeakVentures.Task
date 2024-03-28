using PeakVentures.Task.StorageService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IMessageConsumer, MessageConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

var host = builder.Build();
host.Run();
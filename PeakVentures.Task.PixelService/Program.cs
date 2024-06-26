using Microsoft.OpenApi.Models;
using PeakVentures.Task.PixelService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/track", (IMessagePublisher messagePublisher, HttpContext http, CancellationToken token) =>
    {
        var referer = http.Request.Headers.Referer;
        var userAgent = http.Request.Headers.UserAgent;
        var visitorIpAddress = http.Connection.RemoteIpAddress?.ToString();

        var message = messagePublisher.ConstructMessage(referer, userAgent, visitorIpAddress);
        messagePublisher.PublishMessage(message);
        
        const string clearGif1X1 = "R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==";
        return Results.File(Convert.FromBase64String(clearGif1X1), "image/gif");
    })
    .WithName("TrackVisits")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Tracks user visits",
        Description = "Returns a transparent 1-pixel image in GIF format and collects the visitor information.",
        Tags = new List<OpenApiTag> { new() { Name = "Peak Ventures" } }
    });

app.Run();
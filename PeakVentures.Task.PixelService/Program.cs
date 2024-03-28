var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/track", (HttpContext http, CancellationToken token) =>
    {
        var referer = http.Request.Headers.Referer;
        var userAgent = http.Request.Headers.UserAgent;
        var visitorIpAddress = http.Connection.RemoteIpAddress?.ToString();
        
        const string clearGif1X1 = "R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==";
        return Results.File(Convert.FromBase64String(clearGif1X1), "image/gif");
    })
    .WithName("TrackVisits")
    .WithOpenApi();

app.Run();
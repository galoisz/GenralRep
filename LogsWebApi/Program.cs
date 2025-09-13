using WebApi.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add RabbitMQ logging with level-based routing
builder.Logging.AddRabbitMQ(config =>
{
    config.HostName = "localhost";
    config.Port = 5672;
    config.UserName = "guest";
    config.Password = "guest";
    config.ExchangeName = "yarel_logs";
    config.MinimumLogLevel = LogLevel.Debug;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


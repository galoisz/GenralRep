using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace WebApi.Logging
{
    public class RabbitMQLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly RabbitMQLoggerConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQLogger(string categoryName, RabbitMQLoggerConfiguration config, IConnection connection)
        {
            _categoryName = categoryName;
            _config = config;
            _connection = connection;
            _channel = _connection.CreateModel();
            
            // Declare exchanges and queues
            SetupRabbitMQInfrastructure();
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.MinimumLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Category = _categoryName,
                EventId = eventId.Id,
                Message = message,
                Exception = exception?.ToString()
            };

            var jsonMessage = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions 
            { 
                WriteIndented = false 
            });
            
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var routingKey = GetRoutingKeyForLogLevel(logLevel);

            try
            {
                // Auto-acknowledgment: fire-and-forget publishing
                _channel.BasicPublish(
                    exchange: _config.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }
            catch (Exception ex)
            {
                // Fallback to console if RabbitMQ fails
                Console.WriteLine($"Failed to publish log to RabbitMQ: {ex.Message}");
                Console.WriteLine($"Original log: [{logLevel}] {message}");
            }
        }

        private void SetupRabbitMQInfrastructure()
        {
            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _config.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            // Declare queues and bind them
            var queues = new[] { "debug", "info", "errors" };
            
            foreach (var queue in queues)
            {
                _channel.QueueDeclare(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                _channel.QueueBind(
                    queue: queue,
                    exchange: _config.ExchangeName,
                    routingKey: queue);
            }
        }

        private string GetRoutingKeyForLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "debug",
                LogLevel.Debug => "debug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warning",
                LogLevel.Error => "error",
                LogLevel.Critical => "fatal",
                _ => "info"
            };
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}

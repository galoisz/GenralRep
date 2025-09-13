using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace WebApi.Logging
{
    [ProviderAlias("RabbitMQ")]
    public class RabbitMQLoggerProvider : ILoggerProvider
    {
        private readonly RabbitMQLoggerConfiguration _config;
        private readonly IConnection _connection;
        private readonly ConcurrentDictionary<string, RabbitMQLogger> _loggers = new();

        public RabbitMQLoggerProvider(RabbitMQLoggerConfiguration config)
        {
            _config = config;
            
            var factory = new ConnectionFactory()
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.UserName,
                Password = config.Password
            };

            try
            {
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to connect to RabbitMQ: {ex.Message}", ex);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new RabbitMQLogger(name, _config, _connection));
        }

        public void Dispose()
        {
            foreach (var logger in _loggers.Values)
            {
                logger.Dispose();
            }
            _loggers.Clear();
            _connection?.Dispose();
        }
    }
}

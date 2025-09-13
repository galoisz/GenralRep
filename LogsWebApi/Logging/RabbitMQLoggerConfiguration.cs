namespace WebApi.Logging
{
    public class RabbitMQLoggerConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = "yarel_logs";
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;
    }
}

namespace WebApi.Logging
{
    public static class RabbitMQLoggerExtensions
    {
        public static ILoggingBuilder AddRabbitMQ(this ILoggingBuilder builder, RabbitMQLoggerConfiguration config)
        {
            builder.AddProvider(new RabbitMQLoggerProvider(config));
            return builder;
        }

        public static ILoggingBuilder AddRabbitMQ(this ILoggingBuilder builder, Action<RabbitMQLoggerConfiguration> configure)
        {
            var config = new RabbitMQLoggerConfiguration();
            configure(config);
            return builder.AddRabbitMQ(config);
        }
    }
}

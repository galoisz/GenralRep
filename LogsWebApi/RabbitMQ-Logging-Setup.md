# RabbitMQ Logging Setup

This project implements custom RabbitMQ logging with level-based routing.

## RabbitMQ Setup

### Using Docker (Recommended)

1. Run RabbitMQ with management UI:
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

2. Access RabbitMQ Management UI at: http://localhost:15672
   - Username: `guest`
   - Password: `guest`

### Manual Installation

1. Download and install RabbitMQ from: https://www.rabbitmq.com/download.html
2. Enable management plugin: `rabbitmq-plugins enable rabbitmq_management`
3. Start RabbitMQ service

## Log Routing Configuration

The logging system routes messages based on log levels:

- **DEBUG/TRACE** → `debug` queue
- **INFO/WARNING** → `info` queue  
- **ERROR/CRITICAL** → `errors` queue

## Configuration

The RabbitMQ logging is configured in `Program.cs`:

```csharp
builder.Logging.AddRabbitMQ(config =>
{
    config.HostName = "localhost";     // RabbitMQ server
    config.Port = 5672;               // RabbitMQ port
    config.UserName = "guest";        // Username
    config.Password = "guest";        // Password
    config.ExchangeName = "logs";     // Exchange name
    config.MinimumLogLevel = LogLevel.Debug; // Minimum log level
});
```

## Testing

1. Start the application
2. Navigate to: `GET /api/Product/test-logs`
3. Check RabbitMQ Management UI to see messages in different queues:
   - `debug` queue: Debug and Trace messages
   - `info` queue: Info and Warning messages
   - `errors` queue: Error and Critical messages

## Message Format

Each log message is sent as JSON:

```json
{
  "Timestamp": "2025-09-12T10:30:00.000Z",
  "Level": "Information",
  "Category": "WebApi.Controllers.ProductController",
  "EventId": 0,
  "Message": "Getting all products. Total count: 5",
  "Exception": null
}
```

## Monitoring

Use the RabbitMQ Management UI to:
- Monitor queue depths
- View message rates
- Inspect individual messages
- Set up alerts

## Troubleshooting

If RabbitMQ connection fails, logs will fallback to console output with error messages.

Common issues:
- RabbitMQ not running: Ensure Docker container or service is started
- Connection refused: Check hostname and port configuration
- Authentication failed: Verify username/password

using LoggerMicroservice.Models;
using LoggerMicroservice.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LoggerMicroservice.Services;

public class LogConsumerService : BackgroundService
{
    private readonly ILogRepository _logRepository;
    private readonly ILogger<LogConsumerService> _logger;
    private readonly RabbitMQ.Client.IConnection _connection;
    private readonly IModel _channel;

    public LogConsumerService(IConfiguration config, ILogRepository logRepository, ILogger<LogConsumerService> logger)
    {
        _logRepository = logRepository;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            //UserName = config["RabbitMQ:Username"],
            //Password = config["RabbitMQ:Password"]
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        //_channel.ExchangeDeclare(exchange: "yarel_logs", type: ExchangeType.Direct);
        _channel.ExchangeDeclare(exchange: "yarel_logs", type: ExchangeType.Direct,  autoDelete: false);

    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var level in new[] { "info", "warning", "error", "fatal" })
        {
            string queueName = $"{level}";
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: queueName, exchange: "yarel_logs", routingKey: level);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) => await ProcessMessage(ea);
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        return Task.CompletedTask;
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var logMessage = JsonSerializer.Deserialize<LogMessage>(message);

        if (logMessage != null && !await _logRepository.WriteToElasticsearch(logMessage))
        {
            await _logRepository.WriteToFile(message);
        }

        _channel.BasicAck(ea.DeliveryTag, false);
    }
}

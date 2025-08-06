using LoggerMicroservice.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoggerMicroservice.Services;


public class LogProducerService : BackgroundService
{
    private readonly IModel _channel;
    private readonly Random _random = new();
    private readonly string[] _levels = { "info", "warning", "error", "fatal" };

    public LogProducerService(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName =  "localhost"//config["RabbitMQ:Host"],
            //UserName = config["RabbitMQ:Username"],
            //Password = config["RabbitMQ:Password"]
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "logs-exchange", type: ExchangeType.Direct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var logMessage = new LogMessage
            {
                Level = _levels[_random.Next(_levels.Length)],
                Message = $"Random log message at {DateTime.UtcNow}",
                Timestamp = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logMessage));
            _channel.BasicPublish(exchange: "logs-exchange", routingKey: logMessage.Level, basicProperties: null, body: body);

            await Task.Delay(500, stoppingToken);
        }
    }
}

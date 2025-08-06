using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace RabbitMqPersonDirectExcahnge;

internal class BatchConsumer
{
    private const string ExchangeName = "cio_batch_exchange";
    private const string QueueName = "persons_batch";
    private const string RoutingKey = "persons";

    public void Consume()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare the exchange and queue
        channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        // Bind the queue to the exchange with the routing key
        channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);

        Console.WriteLine($"Listening for batches on queue '{QueueName}'...");

        // Create a consumer for the queue
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                // Deserialize the batch message
                var persons = JsonSerializer.Deserialize<List<Person>>(message);

                if (persons != null)
                {
                    Console.WriteLine($"Received a batch of {persons.Count} persons:");
                    foreach (var person in persons)
                    {
                        Console.WriteLine($" - ID: {person.Id}, Name: {person.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }

            // Acknowledge the message
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            

        };

        // Start consuming messages
        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        Thread.Sleep(2000);
    }
}

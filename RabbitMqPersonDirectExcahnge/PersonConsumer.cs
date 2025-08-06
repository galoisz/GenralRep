using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Concurrent;

namespace RabbitMqPersonDirectExcahnge;

internal class PersonConsumer
{

    private const string InputExchange = "cio_exchange";
    private const string InputQueue = "persons";
    private const string RoutingKey = "persons";
    private const string OutputExchange = "cio_batch_exchange";


    public void Consume()
    {


        List<Person> Batch = new List<Person>();
        const int BatchSize = 10;


        var factory = new ConnectionFactory() { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare the input queue and bind it to the input exchange
        channel.QueueDeclare(queue: InputQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: InputQueue, exchange: InputExchange, routingKey: RoutingKey);

        // Declare the output exchange
        channel.ExchangeDeclare(exchange: OutputExchange, type: ExchangeType.Direct);

        Console.WriteLine($"Listening for messages on queue '{InputQueue}'...");

        // Create a consumer for the input queue
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var person = JsonSerializer.Deserialize<Person>(message);

                if (person != null)
                {
                    Batch.Add(person);
                    Console.WriteLine($"Received person {person.Id}");

                    // If batch size is reached, publish to the new exchange
                    if (Batch.Count == BatchSize)
                    {
                        PublishBatch(channel, Batch);
                        Batch.Clear(); // Clear the batch after publishing
                    }
                }

                Console.WriteLine($"acknowledging person {person.Id}");

                // Acknowledge the message
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: true);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

        };

        // Start consuming messages
        channel.BasicConsume(queue: InputQueue, autoAck: false, consumer: consumer);


        Thread.Sleep(1000);
        //Console.WriteLine("Press [enter] to exit.");
        
    }

    private void PublishBatch(IModel channel, List<Person> batch)
    {
        var batchMessage = JsonSerializer.Serialize(batch);
        var body = Encoding.UTF8.GetBytes(batchMessage);

        channel.BasicPublish(
            exchange: OutputExchange,
            routingKey: RoutingKey,
            basicProperties: null,
            body: body);

        Console.WriteLine($"Published batch of {batch.Count} persons to '{OutputExchange}' with routing key '{RoutingKey}'.");
    }
}


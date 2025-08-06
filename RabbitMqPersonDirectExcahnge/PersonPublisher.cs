using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqPersonDirectExcahnge;

internal class PersonPublisher
{
    public void Publish(int indx) {

        const string exchangeName = "cio_exchange";
        const string routingKey = "persons";
        const string queueName = "persons_queue";

        // Create a connection to RabbitMQ
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare a direct exchange
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

        // Declare a queue
        channel.QueueDeclare(
            queue: routingKey,
            durable: true,    // Messages will survive a broker restart
            exclusive: false, // Queue can be accessed by multiple connections
            autoDelete: false, // Queue won't be deleted automatically
            arguments: null);

        // Bind the queue to the exchange with the routing key
        channel.QueueBind(
            queue: routingKey,
            exchange: exchangeName,
            routingKey: routingKey);

        Console.WriteLine($"Queue '{queueName}' is bound to the exchange '{exchangeName}' with route '{routingKey}'.");

        int frIndx = indx * 100;
        // Create and publish 100 Person entries
        for (int i = frIndx ; i <= frIndx + 100; i++)
        {
            var person = new Person
            {
                Id = i,
                Name = $"Person {i}"
            };

            // Serialize the Person object to JSON
            var message = JsonConvert.SerializeObject(person);
            var body = Encoding.UTF8.GetBytes(message);

            // Publish the message to the single route
            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);

            Console.WriteLine($"Published: {message} to Route: {routingKey}");
        }

        Console.WriteLine("All messages published and queue is ready to consume them.");

    }
}

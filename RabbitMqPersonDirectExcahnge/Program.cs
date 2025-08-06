using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMqPersonDirectExcahnge;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        

        for (int i = 0; i < 100; i++)
        {
            PersonPublisher publisher = new();
            publisher.Publish(i);

            Thread.Sleep(500);

            //PersonConsumer consumer = new PersonConsumer();
            //consumer.Consume();

            //Thread.Sleep(500);

            //BatchConsumer batchConsumer = new BatchConsumer();
            //batchConsumer.Consume();

            //Thread.Sleep(500);

        }


        Console.ReadLine();
    }
}

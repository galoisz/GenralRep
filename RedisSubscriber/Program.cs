using StackExchange.Redis;

public class Program
{
    private static readonly string _redisHost = "localhost"; // Assuming Redis is running on localhost
    private static readonly int _redisPort = 6379;
    private static readonly string _channel = "mychannel";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Redis Subscriber Started");

        // Connect to Redis
        var connection = ConnectionMultiplexer.Connect($"{_redisHost}:{_redisPort}");
        var subscriber = connection.GetSubscriber();

        await subscriber.SubscribeAsync(_channel, (channel, message) =>
        {
            Console.WriteLine($"Received: {message}");
            //return Task.CompletedTask;
        });

        Console.ReadLine(); // Keep the subscriber running
    }
}
using StackExchange.Redis;

public class Program
{
    private static readonly string _redisHost = "localhost"; // Assuming Redis is running on localhost
    private static readonly int _redisPort = 6379;
    private static readonly string _channel = "mychannel";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Redis Publisher Started");

        // Connect to Redis
        var connection = ConnectionMultiplexer.Connect($"{_redisHost}:{_redisPort}");
        var subscriber = connection.GetSubscriber();

        while (true)
        {
            var message = $"Message {DateTime.Now}";
            await subscriber.PublishAsync(_channel, message);
            Console.WriteLine($"Published: {message}");
            await Task.Delay(1000); // Publish every second
        }
    }
}
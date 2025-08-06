using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

//var severity = "info"; // Replace with your desired severity
var messageTemplate = "This is a recurring {0} log message {1} - with guid {2}"; // Replace with your desired log message
//var body = Encoding.UTF8.GetBytes(message);

// Infinite loop to send a log message every 500 mscd 
int i = 0;
while (true)
{
    i++;
    string severity = (i % 3) switch
    {
        0 => "info",
        1 => "warning",
        2 => "error",
        _ => throw new InvalidOperationException("Unexpected value") // Default case for completeness
    };

    var body = Encoding.UTF8.GetBytes(string.Format(messageTemplate, severity, i, Guid.NewGuid().ToString()));
    await channel.BasicPublishAsync(exchange: "direct_logs", routingKey: severity, body: body);
    Console.WriteLine($" [x] Sent '{severity}':'{Encoding.UTF8.GetString(body)}'");

    // Wait for 500 milliseconds before sending the next message
    await Task.Delay(500);
    
}

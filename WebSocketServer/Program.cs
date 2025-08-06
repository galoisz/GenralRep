using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebSockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebSocketServer;



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await SendRandomStockData(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

async Task SendRandomStockData(WebSocket webSocket)
{
    var stocks = new[] { "AAPL", "MSFT", "NVDA" };
    var random = new Random();

    while (webSocket.State == WebSocketState.Open)
    {
        // Create random DTO
        var stockData = new StockDto
        {
            Stock = stocks[random.Next(stocks.Length)],
            Price = Math.Round(random.NextDouble() * (100 - 50) + 50, 2),
            Date = DateTime.UtcNow.AddDays(-random.Next(1, 365)) // Random date in the last year
        };

        var json = JsonSerializer.Serialize(stockData);
        var bytes = Encoding.UTF8.GetBytes(json);

        // Send data to client
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for response
        var buffer = new byte[1024];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Client Response: {receivedMessage}");

        await Task.Delay(500); // Send every 5 seconds
    }
}


app.Run();

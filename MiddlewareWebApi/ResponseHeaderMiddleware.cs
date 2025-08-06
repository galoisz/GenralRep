using System.IO;
using System.Text;

namespace MiddlewareWebApi;

public class ResponseHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context); // Call the next middleware
        //string response = await ConvertStreamToStringAsync(context.Response.Body);
        //Console.WriteLine($"Outgoing Response: {context.Response.StatusCode} {response}");
        Console.WriteLine($"Outgoing Response: {context.Response.StatusCode}");

    }

    public static async Task<string> ConvertStreamToStringAsync(Stream stream)
    {
        // Ensure the stream's position is at the beginning
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}

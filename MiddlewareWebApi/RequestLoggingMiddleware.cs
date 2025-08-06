namespace MiddlewareWebApi;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
        await _next(context); // Call the next middleware
    }
}

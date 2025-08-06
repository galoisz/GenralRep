using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;


namespace UserProfileApi;


public class UserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabase _redisDatabase;

    public UserMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        _next = next;
        _redisDatabase = redis.GetDatabase();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //var userToken = context.Request.Headers["userToken"].ToString();

        //if (!string.IsNullOrEmpty(userToken))
        //{
        //    var userId = await _redisDatabase.SetPopAsync(userToken);  // Get userId from Redis set

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        var userProfile = new UserProfile { UserId = userId }; // Create UserProfile
        //        context.Items["UserProfile"] = userProfile;  // Inject into context
        //    }



        //}
        context.Items["UserProfile"] = new UserProfile { UserId = new Random().Next(1000).ToString() };
        
        await _next(context);  // Pass request to the next middleware
    }
}

public class UserProfile
{
    public string UserId { get; set; }
}

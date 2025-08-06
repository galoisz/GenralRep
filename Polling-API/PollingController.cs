
using Microsoft.AspNetCore.Mvc;
namespace Polling_API;


[ApiController]
[Route("api/[controller]")]
public class PollingController : ControllerBase
{
    private static readonly Random _random = new();

    [HttpGet("data")]
    public IActionResult GetPollingData()
    {
        var data = new
        {
            Timestamp = DateTime.UtcNow,
            Value = _random.Next(1, 100) // Simulate changing data
        };

        return Ok(data);
    }
}

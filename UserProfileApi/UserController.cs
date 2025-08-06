
using Microsoft.AspNetCore.Mvc;
namespace UserProfileApi;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userProfile = HttpContext.Items["UserProfile"] as UserProfile;

        if (userProfile == null)
        {
            return Unauthorized(new { message = "User not found or token invalid" });
        }

        return Ok(new { userProfile.UserId });
    }
}

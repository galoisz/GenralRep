using IOptionsSnapshotExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace OptionsSnapshotExample;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly MyService _myService;

    public ConfigController(MyService myService)
    {
        _myService = myService;
    }

    [HttpGet]
    public IActionResult GetConfig()
    {
        return Ok(_myService.GetAppInfo());
    }
}

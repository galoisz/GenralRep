

using Microsoft.Extensions.Options;

namespace IOptionsSnapshotExample.Services;

public class MyService
{
    private readonly MySettings _settings;

    public MyService(IOptionsSnapshot<MySettings> options)
    {
        _settings = options.Value; // Gets the latest configuration for each request
    }

    public string GetAppInfo()
    {
        return $"App: {_settings.AppName}, Max Users: {_settings.MaxUsers}";
    }
}

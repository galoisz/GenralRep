using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Application", "MyApp")  // Adds custom property
    .MinimumLevel.Debug() // Logs Debug and higher
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    //.Enrich.WithMachineName("")  // Adds machine name to every log
    .CreateLogger();


while (true) {
    Log.Information("User {UserId} logged in at {LoginTime}", 1234, DateTime.Now);


    Log.Debug("debug message");
    Log.Error("error message");

    var user = new { Id = 1234, Name = "Bob" };
    Log.Information("User details: {@User}", user);

    try
    {
        int x = 1, y = 0, result;

        result = x / y;

    }
    catch (Exception ex)
    {

        Log.Error(ex, "An exception occurred while processing");

    }


    await Task.Delay(500);
}



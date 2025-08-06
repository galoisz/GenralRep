using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LoggerMicroservice.Repositories;
using LoggerMicroservice.Services;





public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
    //.ConfigureLogging((context, logging) =>
    //{
    //    //logging.ClearProviders(); // Ensure ClearProviders() is available
    //    //logging.AddConsole(); // Enables logging to the console
    //    //logging.AddDebug();   // Optional: Enables debug logging
    //})
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<ILogRepository, LogRepository>();
                services.AddHostedService<LogConsumerService>();
                services.AddHostedService<LogProducerService>();
            })
            .Build();

        await host.RunAsync();
    }
}

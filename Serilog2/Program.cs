using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        IndexFormat = "logs-indx",
        AutoRegisterTemplate = true,
        NumberOfShards = 1,
        NumberOfReplicas = 1,
        BatchPostingLimit = 50,
        CustomFormatter = new Serilog.Formatting.Compact.CompactJsonFormatter()
    })
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MyApp")
    .CreateLogger();

for (int i = 0; i < 100; i++)
{
    Log.Information($"Application has started {i}");
    Log.Warning($"This is a warning message {i}");
    Log.Error($"An error occurred {i}");
}

// Flush logs to Elasticsearch
Log.CloseAndFlush();

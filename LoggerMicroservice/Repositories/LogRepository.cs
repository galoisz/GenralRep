using Elasticsearch.Net;
using LoggerMicroservice.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoggerMicroservice.Repositories;

public class LogRepository : ILogRepository
{
    private readonly IElasticLowLevelClient _elasticClient;
    private readonly string _fallbackPath;
    private readonly ILogger<LogRepository> _logger;

    public LogRepository(IConfiguration config, ILogger<LogRepository> logger)
    {
        _elasticClient = new ElasticLowLevelClient();
        _fallbackPath = config["Logging:FallbackPath"] ?? "logs-fallback.txt";
        _logger = logger;
    }

    public async Task<bool> WriteToElasticsearch(LogMessage logMessage)
    {
        

        try
        {
            _logger.LogInformation($"Elasticsearch write : {JsonSerializer.Serialize(logMessage)}");
            var response = await _elasticClient.IndexAsync<StringResponse>("logs", JsonSerializer.Serialize(logMessage));
            return response.Success;


            //_logger.LogInformation($"Elasticsearch write : {PostData.Serializable(logMessage)}");
            //var response = await _elasticClient.IndexAsync<StringResponse>("logs", PostData.Serializable(logMessage));
            //return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Elasticsearch write failed: {ex.Message}");
            return false;
        }
    }

    public async Task WriteToFile(string message)
    {
        await File.AppendAllTextAsync(_fallbackPath, message + "\n");
    }
}

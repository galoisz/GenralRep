using LoggerMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerMicroservice.Repositories;

public interface ILogRepository
{
    Task<bool> WriteToElasticsearch(LogMessage logMessage);
    Task WriteToFile(string message);
}

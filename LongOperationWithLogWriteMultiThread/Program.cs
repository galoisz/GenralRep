using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class Logger
{
    private readonly string logFilePath = "log.txt";
    private readonly BlockingCollection<string> logQueue = new BlockingCollection<string>();
    private readonly Thread logThread;
    private bool isRunning = true;

    public Logger()
    {
        logThread = new Thread(ProcessLogQueue) { IsBackground = true };
        logThread.Start();
    }

    public void Log(string message)
    {
        logQueue.Add($"{DateTime.Now}: {message}");
    }

    private void ProcessLogQueue()
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
        {
            while (isRunning || logQueue.Count > 0)
            {
                if (logQueue.TryTake(out string logMessage, Timeout.Infinite))
                {
                    writer.WriteLine(logMessage);
                    writer.Flush(); // Ensure logs are written immediately
                }
            }
        }
    }

    public void Stop()
    {
        isRunning = false;
        logQueue.CompleteAdding();
        logThread.Join();
    }
}

class A
{
    private readonly Logger logger;

    public A(Logger logger)
    {
        this.logger = logger;
    }

    public void LongCalculation()
    {
        for (int i = 0; i < 10; i++)
        {
            // Simulate long calculation
            Console.WriteLine($"Calculating step {i + 1}...");
            Task.Delay(1000).Wait(); // Simulating delay

            // Queue log message
            logger.Log($"Step {i + 1} completed.");
        }
    }
}

class Program
{
    static void Main()
    {
        Logger logger = new Logger();
        A a = new A(logger);

        a.LongCalculation();

        logger.Stop(); // Ensure all logs are written before exiting
    }
}

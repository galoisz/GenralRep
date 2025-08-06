using System;
using System.Threading;

class SemaphoreExample
{
    static Semaphore semaphore = new Semaphore(2, 3, "Global\\MyNamedSemaphore");  // Initial: 2 permits, Max: 3

    static void Main()
    {
        // Create and start threads
        Thread threadA = new Thread(AccessResource) { Name = "Thread A" };
        Thread threadB = new Thread(AccessResource) { Name = "Thread B" };
        Thread threadC = new Thread(AccessResource) { Name = "Thread C" };
        Thread threadD = new Thread(AccessResource) { Name = "Thread D" };
        Thread threadE = new Thread(AccessResource) { Name = "Thread E" };
        Thread threadF = new Thread(AccessResource) { Name = "Thread F" };

        threadA.Start();
        threadB.Start();
        threadC.Start();
        threadD.Start();
        threadE.Start();
        threadF.Start();

        threadA.Join();
        threadB.Join();
        threadC.Join();
        threadD.Join();
        threadE.Join();
        threadF.Join();

        Console.WriteLine("All threads have completed.");
    }

    static void AccessResource()
    {
        Console.WriteLine($"{Thread.CurrentThread.Name} is requesting access...");
        semaphore.WaitOne();  // Wait for a permit

        try
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} has entered the critical section.");
            Thread.Sleep(2000);  // Simulate work
        }
        finally
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} is leaving the critical section.");
            semaphore.Release();  // Release the permit
        }
    }
}

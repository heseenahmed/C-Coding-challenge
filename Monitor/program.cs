using Serilog;
using System.Collections.Generic;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("health.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        double prevCpuMs = 0;
        DateTime prev = DateTime.UtcNow;
        var proc = Process.GetCurrentProcess();
        bool first = true;
        while (true)
        {
            proc.Refresh();
            var now = DateTime.UtcNow;
            var cpuMs = proc.TotalProcessorTime.TotalMilliseconds;
            var elapsedMs = (now - prev).TotalMilliseconds;
            var cpuPercent = 0.0;

            // ⚡ Skip first iteration because we don’t have a valid baseline yet
            if (first)
            {
                first = false;
                prevCpuMs = cpuMs;
                prev = now;
                await Task.Delay(TimeSpan.FromSeconds(10));
                continue;
            }

            if (elapsedMs > 0)
            {
                cpuPercent = ((cpuMs - prevCpuMs) / elapsedMs) * 100.0 / Environment.ProcessorCount;
                prevCpuMs = cpuMs;
                prev = now;
            }

            var workingSet = proc.WorkingSet64;
            var managedHeap = GC.GetTotalMemory(false);

            Log.Information("ts={Timestamp} cpu={CpuPercent:F1}% ws={WorkingSetMB:F2}MB heap={HeapMB:F2}MB",
                DateTime.UtcNow, cpuPercent,
                workingSet / (1024.0 * 1024.0),
                managedHeap / (1024.0 * 1024.0));

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}

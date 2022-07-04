using System;
using System.Diagnostics;

namespace Toradex.TestApp;

public static class PerformanceCounter
{
    private static readonly Stopwatch _watch = new();
    private static DateTime _startTime;

    public static void Start()
    {
        _watch.Start();
        _startTime = DateTime.Now;
    }

    public static void Stop()
    {
        _watch.Stop();
    }

    public static void Step(string message)
    {
        if (!_watch.IsRunning)
            Start();
        Console.WriteLine(
            $"Total:{(DateTime.Now - _startTime).TotalMilliseconds} ms Step:{_watch.ElapsedMilliseconds} ms: {message}");
        _watch.Restart();
    }
}
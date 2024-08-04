using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/**
 * A stopwatch that performs an action when it is disposed.
 */
public class DisposableStopwatch : IDisposable
{
    private static readonly List<string> placeholders = new() { "<millis>", "<ms>" };
    private readonly Stopwatch stopwatch;
    private readonly Action<Stopwatch> action;

    public double ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;
    public TimeSpan Elapsed => stopwatch.Elapsed;
    public long ElapsedTicks => stopwatch.ElapsedTicks;

    public DisposableStopwatch(Action<Stopwatch> action)
    {
        this.action = action;
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    // Logs the given text when the stopwatch is disposed.
    // Thereby, the placeholder <millis> will be replaced with the elapsed milliseconds.
    public DisposableStopwatch(string textWithPlaceholders)
        : this((sw) => LogTextWithPlaceholders(sw, textWithPlaceholders))
    {
    }

    public void Dispose()
    {
        stopwatch.Stop();
        action.Invoke(stopwatch);
    }

    private static void LogTextWithPlaceholders(Stopwatch sw, string textWithPlaceholders)
    {
        string millis = sw.ElapsedMilliseconds.ToString();
        string logText = textWithPlaceholders;
        if (ContainsPlaceholder(logText))
        {
            foreach (string placeholder in placeholders)
            {
                logText = logText.Replace(placeholder, millis);
            }
        }
        else
        {
            logText = $"{logText}: {millis} ms";
        }

        Debug.Log(logText);
    }

    private static bool ContainsPlaceholder(string logText)
    {
        foreach (string placeholder in placeholders)
        {
            if (logText.Contains(placeholder))
            {
                return true;
            }
        }

        return false;
    }
}

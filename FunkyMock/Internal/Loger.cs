using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FunkyMock.Internal;
// Idea from https://dev.to/panoukos41/debugging-c-source-generators-1flm

/// <summary>
/// A poor mans log, debugging or logging the real thing is tricky. This create s acs file with comment logs
/// </summary>
public static class Logger
{
    private const string Filename = "logs.g.cs";

    private class Entry
    {
        private readonly string _message;
        public Entry(string message) => _message = $"{DateTime.UtcNow:hh:mm:ss.fff} {message}";
        public override string ToString() => _message;
    }

    private static readonly List<Entry> Entries = new();
    /// <summary>
    /// In the tests we need to know (Debug vs Release) how many output files there will be
    /// </summary>
#if DEBUG
    public static bool IsEnabled => true;
#else
    public static bool IsEnabled => false;
#endif

    [Conditional("DEBUG")]
    public static void Log(string msg)
    {
        Entries.Add(new Entry(msg));
        Debug.WriteLine(msg);
    }

    [Conditional("DEBUG")]
    public static void Flush(SourceProductionContext context)
    {
        if (!IsEnabled)
        {
            return;
        }

        context.AddSource(
            Filename,
            SourceText.From(Content, Encoding.UTF8)
        );
    }

    private static string Content => new StringBuilder()
        .Append("/*\n")
        .Append(string.Join("\n", Entries.Select(e => e.ToString())))
        .Append("*/\n")
        .ToString();
}

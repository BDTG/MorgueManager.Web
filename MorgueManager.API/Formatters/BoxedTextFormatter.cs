using Serilog.Events;
using Serilog.Formatting;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MorgueManager.API.Formatters;

public class BoxedTextFormatter : ITextFormatter
{
    private const int BoxWidth = 100;
    private const int ContentWidth = BoxWidth - 4; // 96 characters

    public void Format(LogEvent logEvent, TextWriter output)
    {
        var sb = new StringBuilder();
        var message = logEvent.RenderMessage();
        var timestamp = logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
        var level = logEvent.Level.ToString().ToUpper();

        // Top Border
        sb.AppendLine("┌" + new string('─', BoxWidth - 2) + "┐");
        
        // Header info
        sb.AppendLine($"│ Timestamp: {timestamp,-ContentWidth} │");
        sb.AppendLine($"│ Level:     {level,-ContentWidth} │");
        
        // Message
        sb.AppendLine("├" + new string('─', BoxWidth - 2) + "┤");
        var messageLines = WrapText(message, ContentWidth);
        foreach (var line in messageLines)
        {
            sb.AppendLine($"│ {line,-ContentWidth} │");
        }

        // Exception detail if present
        if (logEvent.Exception != null)
        {
            sb.AppendLine("├" + new string('─', BoxWidth - 2) + "┤");
            sb.AppendLine($"│ {"EXCEPTION STACK TRACE",-ContentWidth} │");
            sb.AppendLine("├" + new string('─', BoxWidth - 2) + "┤");
            
            var exceptionText = logEvent.Exception.ToString();
            var exLines = exceptionText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var exLine in exLines)
            {
                var wrappedExLines = WrapText(exLine, ContentWidth);
                foreach (var wl in wrappedExLines)
                {
                    sb.AppendLine($"│ {wl,-ContentWidth} │");
                }
            }
        }

        // Bottom Border
        sb.AppendLine("└" + new string('─', BoxWidth - 2) + "┘");
        sb.AppendLine(); // extra empty line to separate boxes

        output.Write(sb.ToString());
    }

    private static List<string> WrapText(string text, int maxWidth)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string> { "" };

        var lines = new List<string>();
        for (int i = 0; i < text.Length; i += maxWidth)
        {
            if (i + maxWidth < text.Length)
                lines.Add(text.Substring(i, maxWidth));
            else
                lines.Add(text.Substring(i));
        }
        return lines;
    }
}

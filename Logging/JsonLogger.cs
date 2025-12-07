using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoomamaaApp.Logging
{
    public class JsonLogger : ILogger
    {
        private readonly List<LogEntry> logs = new List<LogEntry>();
        private readonly string filePath;

        public JsonLogger(string filePath = "logs.json")
        {
            this.filePath = filePath;
        }

        public void Log(string message)
        {
            logs.Add(new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message
            });
        }

        public void SaveLogs()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("[");

                for (int i = 0; i < logs.Count; i++)
                {
                    var entry = logs[i];
                    sb.AppendLine("  {");
                    sb.AppendLine($"    \"timestamp\": \"{entry.Timestamp:o}\",");
                    sb.AppendLine($"    \"message\": {EscapeJsonString(entry.Message)}");
                    sb.Append("  }");
                    
                    if (i < logs.Count - 1)
                        sb.AppendLine(",");
                    else
                        sb.AppendLine();
                }

                sb.AppendLine("]");

                File.WriteAllText(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving JSON logs: {ex.Message}");
            }
        }

        private string EscapeJsonString(string str)
        {
            if (str == null) return "null";

            var sb = new StringBuilder("\"");
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 32)
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }

        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
        }
    }
}

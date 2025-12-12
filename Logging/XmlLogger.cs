using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LoomamaaApp.Logging
{
    public class XmlLogger : ILogger
    {
        private readonly List<LogEntry> logs = new List<LogEntry>();
        private readonly string filePath;

        public XmlLogger(string filePath = "logs.xml")
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
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  "
                };

                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Logs");

                    foreach (var entry in logs)
                    {
                        writer.WriteStartElement("LogEntry");
                        writer.WriteElementString("Timestamp", entry.Timestamp.ToString("o"));
                        writer.WriteElementString("Message", entry.Message);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving XML logs: {ex.Message}");
            }
        }

        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
        }
    }
}

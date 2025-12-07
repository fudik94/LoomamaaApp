using System;

namespace LoomamaaApp.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void SaveLogs();
    }
}

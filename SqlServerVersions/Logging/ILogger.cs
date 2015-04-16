using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerVersions.Logging
{
    public interface ILogger
    {
        bool IsLoggerValid();
        void LogMessage(LogEntry logEntry);
    }
}

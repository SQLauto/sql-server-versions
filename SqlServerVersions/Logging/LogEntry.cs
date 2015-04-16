using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SqlServerVersions.Logging
{
    public class LogEntry
    {
        public string Message { get; set; }
        public string MessageType { get; set; }
        public string StackTrace { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SqlServerVersions.Logging
{
    public class InvalidLoggerException : Exception
    {
        public InvalidLoggerException() : base("Logger destination is invalid.") { }
        public InvalidLoggerException(string message) : base(message) { }
    }
}
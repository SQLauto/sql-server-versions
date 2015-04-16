using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlServerVersions.Models;

namespace SqlServerVersions.ViewModels
{
    public class SupportabilityBoundaries
    {
        public VersionInfo BaseVersion { get; set; }

        public VersionInfo NewestSupported { get; set; }
        public VersionInfo OldestSupported { get; set; }
    }
}
using SqlServerVersions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SqlServerVersions.ViewModels
{
    public class VersionSearchViewModel
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }

        public bool IsSearchedFor { get; set; }
        public VersionInfo FoundVersion { get; set; }

        public VersionInfo NewVersion { get; set; }
        public string ReferenceLink { get; set; }
    }
}
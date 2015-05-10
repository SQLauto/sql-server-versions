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

        public string NewFriendlyNameLong { get; set; }
        public string NewFriendlyNameShort { get; set; }
        public DateTime? NewReleaseDate { get; set; }
        public string NewReferenceLink { get; set; }
        public bool NewIsSupported { get; set; }
    }
}
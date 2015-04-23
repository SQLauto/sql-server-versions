using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlServerVersions.Models;

namespace SqlServerVersions.ViewModels
{
    public class BackFillViewModel
    {
        public VersionBuild BackFillBuild { get; set; }
        public int BackFillCount { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public string FriendlyNameLong { get; set; }
        public string FriendlyNameShort { get; set; }
        public bool IsSupported { get; set; }
    }
}
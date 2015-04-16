using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlServerVersions.Models;
using System.Web.Mvc;

namespace SqlServerVersions.ViewModels
{
    public class RecentReleaseViewModel
    {
        public IEnumerable<VersionInfo> RecentVersions { get; set; }
        public IEnumerable<VersionInfo> MajorMinorBaseVersions { get; set; }

        public int SelectedId { get; set; }
    }
}
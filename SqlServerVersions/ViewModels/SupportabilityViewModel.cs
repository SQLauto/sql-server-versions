using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlServerVersions.Models;

namespace SqlServerVersions.ViewModels
{
    public class SupportabilityViewModel
    {
        public IEnumerable<VersionInfo> MajorMinorBaseVersions { get; set; }
        public IEnumerable<SupportabilityBoundaries> VersionBoundaries { get; set; }

        public int SelectedId { get; set; }
    }
}
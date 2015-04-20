using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlServerVersions.Models;

namespace SqlServerVersions.ViewModels
{
    public class BackFillViewModel
    {
        public Build BackFillBuild { get; set; }
        public int BackFillCount { get; set; }
    }
}
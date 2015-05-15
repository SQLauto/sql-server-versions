using SqlServerVersions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SqlServerVersions.Controllers
{
    public class VersionController : ApiController
    {
        private DataAccess _dataAccess;

        public VersionController()
        {
            _dataAccess = new DataAccess();
        }

        [HttpGet]
        public IEnumerable<VersionInfo> SearchVersion()
        {
            return _dataAccess.GetVersionInfo();
        }
        [HttpGet]
        public IEnumerable<VersionInfo> SearchVersion(int major)
        {
            return _dataAccess.GetVersionInfo(major);
        }
        [HttpGet]
        public IEnumerable<VersionInfo> SearchVersion(int major, int minor)
        {
            return _dataAccess.GetVersionInfo(major, minor);
        }
        [HttpGet]
        public VersionInfo SearchVersion(int major, int minor, int build, int revision)
        {
            return _dataAccess.GetVersionInfo(major, minor, build, revision);
        }
        [HttpPost]
        public HttpResponseMessage SearchVersion(VersionInfo newVersionInfo)
        {
            return PostNewVersion(newVersionInfo);
        }
        [HttpPut]
        public HttpResponseMessage SearchVersion(int major, int minor, int build, VersionInfo modifiedVersionInfo)
        {
            return PutModifiedVersion(major, minor, build, modifiedVersionInfo);
        }

        [HttpGet]
        public IEnumerable<VersionInfo> TopRecentRelease(int topcount)
        {
            return _dataAccess.GetTopRecentReleaseVersionInfo(topcount);
        }
        [HttpGet]
        public VersionInfo MostRecentByMajorMinor(int major, int minor)
        {
            return _dataAccess.GetMostRecentByMajorMinor(major, minor);
        }
        [HttpGet]
        public VersionInfo LowestSupportedByMajorMinor(int major, int minor)
        {
            return _dataAccess.GetLowestSupportedByMajorMinor(major, minor);
        }
        
        [HttpGet]
        public IEnumerable<VersionInfo> MajorMinorReleases()
        {
            return _dataAccess.GetMajorMinorReleases();
        }
        [HttpGet]
        public IEnumerable<VersionInfo> RecentAndOldestSupportedVersions()
        {
            return _dataAccess.GetRecentAndOldestSupportedVersions().OrderBy(m => m.Major).ThenBy(m => m.Minor).ThenBy(m => m.Build).ThenBy(m => m.Revision);
        }

        private HttpResponseMessage PostNewVersion(VersionInfo newVersionInfo)
        {
            HttpResponseMessage Response;

            // first check to make sure that this version info doesn't already exist
            //
            if (_dataAccess.GetVersionInfo(newVersionInfo.Major, newVersionInfo.Minor, newVersionInfo.Build, newVersionInfo.Revision) != null)
                Response = Request.CreateResponse<VersionInfo>(HttpStatusCode.BadRequest, newVersionInfo);
            else
            { 
                // attempt to add the new version info
                //
                if (_dataAccess.AddVersionInfo(newVersionInfo))
                    Response = Request.CreateResponse<VersionInfo>(HttpStatusCode.Created, newVersionInfo);
                else 
                    Response = Request.CreateResponse<VersionInfo>(HttpStatusCode.BadRequest, newVersionInfo);
            }

            return Response;
        }

        private HttpResponseMessage PutModifiedVersion(int major, int minor, int build, VersionInfo modifiedVersionInfo)
        {
            HttpResponseMessage Response;

            // check to make sure the version info already exists
            //
            if (_dataAccess.GetVersionInfo(major, minor, build, 0) == null)
            {
                Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                // if it already exists then attempt to modify it
                //
                if (_dataAccess.ModifyVersionInfo(major, minor, build, modifiedVersionInfo))
                    Response = Request.CreateResponse<VersionInfo>(HttpStatusCode.OK, modifiedVersionInfo);
                else
                    Response = Request.CreateResponse<VersionInfo>(HttpStatusCode.BadRequest, modifiedVersionInfo);
            }

            return Response;
        }
    }
}

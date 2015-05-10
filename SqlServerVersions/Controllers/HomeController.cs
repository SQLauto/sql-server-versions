using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlServerVersions.Models;
using SqlServerVersions.ViewModels;

namespace SqlServerVersions.Controllers
{
    public class HomeController : Controller
    {
        private const int _topRecentCount = 10;
        private const int _allItemsIndex = -1;
        private const string _allItemsName = "<ALL>";
        private IEnumerable<VersionInfo> _baseMajorMinorVersions;

        public HomeController()
        {
            DataAccess dataAccess = new DataAccess();
            _baseMajorMinorVersions = dataAccess.GetMajorMinorReleases();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "SQL Server Versions";

            return View();
        }

        [HttpPost]
        public ActionResult VersionSearch(VersionSearchViewModel versionSearchViewModel)
        {
            if (ModelState.IsValid) 
            {
                if (versionSearchViewModel.IsNewVersion)
                {
                    if (IsNewVersionDataComplete(versionSearchViewModel))
                    {
                        // at this point we need to actually add the new version
                        //
                        DataAccess dataAccess = new DataAccess();

                        if (dataAccess.AddVersionInfo(new VersionInfo()
                        {
                            Major = versionSearchViewModel.Major,
                            Minor = versionSearchViewModel.Minor,
                            Build = versionSearchViewModel.Build,
                            Revision = versionSearchViewModel.Revision,
                            ReleaseDate = versionSearchViewModel.NewReleaseDate.Value,
                            FriendlyNameLong = versionSearchViewModel.NewFriendlyNameLong,
                            FriendlyNameShort = versionSearchViewModel.NewFriendlyNameShort,
                            ReferenceLinks = new List<string>() { versionSearchViewModel.NewReferenceLink }
                        })) { 
                            // if the added version info is also a back fill build
                            // and if it was successfully added then we need to remove 
                            // this build from the back fill list
                            //
                            if (dataAccess.IsBackFillBuild(versionSearchViewModel.Major, versionSearchViewModel.Minor, versionSearchViewModel.Build))
                                dataAccess.DeleteBackFillBuild(new VersionBuild()
                                {
                                    Major = versionSearchViewModel.Major,
                                    Minor = versionSearchViewModel.Minor,
                                    Build = versionSearchViewModel.Build,
                                    Revision = versionSearchViewModel.Revision
                                });

                            return RedirectToAction(
                                "VersionSearch",
                                new
                                {
                                    major = versionSearchViewModel.Major,
                                    minor = versionSearchViewModel.Minor,
                                    build = versionSearchViewModel.Build,
                                    revision = versionSearchViewModel.Revision
                                });
                        }
                    }
                    else
                    {
                        // this is an error condition, the data isn't all there
                        //
                    }                        
                }

                return RedirectToAction(
                    "VersionSearch",
                    new
                    {
                        major = versionSearchViewModel.Major,
                        minor = versionSearchViewModel.Minor,
                        build = versionSearchViewModel.Build,
                        revision = versionSearchViewModel.Revision
                    });
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet]
        public ActionResult VersionSearch(int major, int minor, int build, int revision)
        {
            VersionSearchViewModel versionSearchViewModel = new VersionSearchViewModel();
            
            versionSearchViewModel.FoundVersion = (new DataAccess()).GetVersionInfo(major, minor, build, revision);

            versionSearchViewModel.Major = major;
            versionSearchViewModel.Minor = minor;
            versionSearchViewModel.Build = build;
            versionSearchViewModel.Revision = revision;

            if (versionSearchViewModel.FoundVersion == null)
            {
                versionSearchViewModel.IsNewVersion = true;
                // we need to initially seed supported to true
                //
                versionSearchViewModel.NewIsSupported = true;
            }
            else
                versionSearchViewModel.IsNewVersion = false;

            versionSearchViewModel.IsSearchedFor = true;

            return View("Index", versionSearchViewModel);
        }
        private bool IsNewVersionDataComplete(VersionSearchViewModel versionSearchViewModel)
        {
            return 
                !string.IsNullOrWhiteSpace(versionSearchViewModel.NewFriendlyNameLong) &&
                !string.IsNullOrWhiteSpace(versionSearchViewModel.NewFriendlyNameShort) &&
                versionSearchViewModel.NewReleaseDate != null &&
                !string.IsNullOrWhiteSpace(versionSearchViewModel.NewReferenceLink);
        }

        [HttpGet]
        public ActionResult RecentRelease(int major, int minor)
        {
            int PreSelectedId;
            DataAccess dataAccess = new DataAccess();
            List<VersionInfo> MajorMinorReleasesList = _baseMajorMinorVersions.ToList();

            MajorMinorReleasesList.Insert(
                0, 
                new VersionInfo()
                {
                    Id = _allItemsIndex,
                    Major = 0,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    FriendlyNameShort = _allItemsName
                });

            PreSelectedId = MajorMinorReleasesList.First(m => m.Major == major && m.Minor == minor).Id;

            return View(
                new RecentReleaseViewModel()
                {
                    MajorMinorBaseVersions = MajorMinorReleasesList,
                    RecentVersions = dataAccess.GetTopRecentReleaseVersionInfo(_topRecentCount, major, minor),
                    SelectedId = PreSelectedId
                });
        }
        [HttpPost]
        public ActionResult RecentRelease(RecentReleaseViewModel recentReleaseViewModel)
        {
            int major, minor;

            if (recentReleaseViewModel.SelectedId == -1)
            {
                major = 0;
                minor = 0;
            }
            else
            {
                VersionInfo SelectedBaseVersion = _baseMajorMinorVersions.First(m => m.Id == recentReleaseViewModel.SelectedId);
                major = SelectedBaseVersion.Major;
                minor = SelectedBaseVersion.Minor;
            }

            return RedirectToAction("RecentRelease", new { major = major, minor = minor });
        }

        [HttpGet]
        public ActionResult Supportability(int major, int minor)
        {
            int PreSelectedId;
            DataAccess dataAccess = new DataAccess();
            List<VersionInfo> MajorMinorReleasesList = _baseMajorMinorVersions.ToList();

            MajorMinorReleasesList.Insert(
                0,
                new VersionInfo()
                {
                    Id = _allItemsIndex,
                    Major = 0,
                    Minor = 0,
                    Build = 0,
                    Revision = 0,
                    FriendlyNameShort = _allItemsName
                });

            PreSelectedId = MajorMinorReleasesList.First(m => m.Major == major && m.Minor == minor).Id;

            List<SupportabilityBoundaries> Boundaries = new List<SupportabilityBoundaries>();

            // if the major and minor are set to zero then the user wants to display all 
            // and in this case we will need to populate the collection with all versions
            //
            // otherwise just add the particular boundary value if we are specifiy (i.e. 
            // major and minor are not zero)
            //
            if (major == 0 && minor == 0)
                foreach (VersionInfo majorMinorVersion in _baseMajorMinorVersions)
                    Boundaries.Add(GetBoundaries(majorMinorVersion));
            else
                Boundaries.Add(GetBoundaries(MajorMinorReleasesList.First(m => m.Major == major && m.Minor == minor)));

            return View(
                new SupportabilityViewModel()
                {
                    MajorMinorBaseVersions = MajorMinorReleasesList,
                    SelectedId = PreSelectedId,
                    VersionBoundaries = Boundaries
                });
        }
        [HttpPost]
        public ActionResult Supportability(SupportabilityViewModel supportabilityViewModel)
        {
            int major, minor;

            if (supportabilityViewModel.SelectedId == -1)
            {
                major = 0;
                minor = 0;
            }
            else
            {
                VersionInfo SelectedBaseVersion = _baseMajorMinorVersions.First(m => m.Id == supportabilityViewModel.SelectedId);
                major = SelectedBaseVersion.Major;
                minor = SelectedBaseVersion.Minor;
            }

            return RedirectToAction("Supportability", new { major = major, minor = minor });
        }

        private SupportabilityBoundaries GetBoundaries(VersionInfo baseVersion)
        {
            DataAccess dataAccess = new DataAccess();
            VersionInfo LowestSupported, HighestSupported;
            IEnumerable<VersionInfo> VersionBoundaries = dataAccess.GetRecentAndOldestSupportedVersions().Where(m => m.Major == baseVersion.Major && m.Minor == baseVersion.Minor);

            if (VersionBoundaries.Count() > 0)
            {
                LowestSupported = VersionBoundaries.OrderBy(m => m.ReleaseDate).First();
                HighestSupported = VersionBoundaries.OrderByDescending(m => m.ReleaseDate).First();
            }
            else
            {
                LowestSupported = null;
                HighestSupported = null;
            }

            return new SupportabilityBoundaries()
            {
                BaseVersion = baseVersion,
                OldestSupported = LowestSupported,
                NewestSupported = HighestSupported
            };
        }

        [HttpGet]
        public ActionResult BackFill()
        {
            BackFillViewModel viewModel = new BackFillViewModel();

            DataAccess dataAccess = new DataAccess();

            viewModel.BackFillBuild = (dataAccess).GetRandomBackFillBuild();
            viewModel.BackFillCount = (dataAccess).GetBackFillBuildsCount();
            viewModel.DisplayMessage = DisplayMessage.None;
            viewModel.IsSupported = true;

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult BackFill(BackFillViewModel backFillViewModel)
        {
            if (!string.IsNullOrWhiteSpace(backFillViewModel.FriendlyNameLong) &&
                !string.IsNullOrWhiteSpace(backFillViewModel.FriendlyNameShort) &&
                backFillViewModel.ReleaseDate.HasValue &&
                !string.IsNullOrWhiteSpace(backFillViewModel.ReferenceLink))
            {
                DataAccess dataAccess = new DataAccess();
                VersionInfo newVersionInfo = new VersionInfo()
                {
                    Major = backFillViewModel.BackFillBuild.Major,
                    Minor = backFillViewModel.BackFillBuild.Minor,
                    Build = backFillViewModel.BackFillBuild.Build,
                    Revision = backFillViewModel.BackFillBuild.Revision,
                    FriendlyNameLong = backFillViewModel.FriendlyNameLong,
                    FriendlyNameShort = backFillViewModel.FriendlyNameShort,
                    ReleaseDate = backFillViewModel.ReleaseDate.Value,
                    IsSupported = backFillViewModel.IsSupported,
                    ReferenceLinks = new List<string>() { backFillViewModel.ReferenceLink }
                };

                if (dataAccess.AddVersionInfo(newVersionInfo))
                {
                    // this is the success condition
                    //
                    // remove the back fill build
                    //
                    if (dataAccess.DeleteBackFillBuild(backFillViewModel.BackFillBuild))
                    {
                        // this is our final and end result success condition
                        //
                        // at this point we want to show success and then next 
                        // random build to fill
                        //
                        backFillViewModel = new BackFillViewModel();

                        backFillViewModel.BackFillBuild = (dataAccess).GetRandomBackFillBuild();
                        backFillViewModel.BackFillCount = (dataAccess).GetBackFillBuildsCount();
                        backFillViewModel.DisplayMessage = DisplayMessage.Success;
                        backFillViewModel.IsSupported = true;

                        return View(backFillViewModel);
                    }
                    else
                    {
                        // this is an error condition
                        // if we can't delete the back fill version then we need to keep it in 
                        // the build and then attempt to delete the version as we don't want to 
                        // have a vicious cycle of attempting to continue trying to fill an 
                        // existing build
                        //
                        dataAccess.DeleteVersionInfo(newVersionInfo);

                        // now we need to report the error *without* keeping the data
                        //
                        backFillViewModel = new BackFillViewModel();
                        backFillViewModel.DisplayMessage = DisplayMessage.Error;

                        return View(backFillViewModel);
                    }
                }
                else
                {
                    // something went wrong here so report the error
                    //
                    backFillViewModel = new BackFillViewModel();
                    backFillViewModel.DisplayMessage = DisplayMessage.Error;

                    return View(backFillViewModel);
                }
            }
            else
            {
                // data isn't valid so display an error with existing data 
                // so the user can reconnect
                //
                backFillViewModel.DisplayMessage = DisplayMessage.Error;
                return View(backFillViewModel);
            }
        }
    }
}

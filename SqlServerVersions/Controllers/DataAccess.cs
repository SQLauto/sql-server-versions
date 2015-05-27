using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SqlServerVersions.Models;
using System.Data;
using System.Data.SqlClient;
using SqlServerVersions.Logging;

namespace SqlServerVersions.Controllers
{
    public class DataAccess
    {
        private ILogger _logger;
#if DEBUG
        private string _connectionString = ConfigurationManager.ConnectionStrings["azuredb_dbg"].ConnectionString;
#else
        private string _connectionString = ConfigurationManager.ConnectionStrings["azuredb"].ConnectionString;
#endif

        public DataAccess()
        {
            _logger = new DbLogger(_connectionString);
        }

        private IEnumerable<VersionInfo> GetAllVersionInfo(int major, int minor, int build, int revision)
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGet";

                if (major >= 0)
                    SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                        {
                            Value = major
                        });

                if (minor >= 0)
                    SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                        {
                            Value = minor
                        });

                if (build >= 0)
                    SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                        {
                            Value = build
                        });

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }

                foreach (DataRow row in Output.Rows)
                    yield return new VersionInfo()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Major = Convert.ToInt32(row["Major"]),
                        Minor = Convert.ToInt32(row["Minor"]),
                        Build = Convert.ToInt32(row["Build"]),
                        Revision = Convert.ToInt32(row["Revision"]),
                        FriendlyNameShort = row["FriendlyNameShort"].ToString(),
                        FriendlyNameLong = row["FriendlyNameLong"].ToString(),
                        ReleaseDate = Convert.ToDateTime(row["ReleaseDate"]),
                        IsSupported = Convert.ToBoolean(row["IsSupported"]),
                        ReferenceLinks = GetReferenceLinks(
                            Convert.ToInt32(row["Major"]),
                            Convert.ToInt32(row["Minor"]),
                            Convert.ToInt32(row["Build"]),
                            Convert.ToInt32(row["Revision"]))
                    };
            }
        }
        private IEnumerable<string> GetReferenceLinks(int major, int minor, int build, int revision)
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetReferenceLinks";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = minor
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = build
                    });

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }

                foreach (DataRow row in Output.Rows)
                    yield return row["Href"].ToString();
            }
        }
        public IEnumerable<VersionInfo> GetVersionInfo()
        {
            return GetAllVersionInfo(-1, -1, -1, -1);
        }
        public IEnumerable<VersionInfo> GetVersionInfo(int major)
        {
            return GetAllVersionInfo(major, -1, -1, -1);
        }
        public IEnumerable<VersionInfo> GetVersionInfo(int major, int minor)
        {
            return GetAllVersionInfo(major, minor, -1, -1);
        }
        public VersionInfo GetVersionInfo(int major, int minor, int build, int revision)
        {
            IEnumerable<VersionInfo> AllVersionInfo = GetAllVersionInfo(major, minor, build, revision);
            
            if (AllVersionInfo.Count() == 0)
                return null;

            // track this version as being found because at this point we must
            // have a match
            //
            AddVersionSearchTracking(major, minor, build);

            return AllVersionInfo.First();
        }
        public IEnumerable<VersionInfo> GetTopRecentReleaseVersionInfo(int topCount)
        {
            return GetTopRecentReleaseVersionInfo(topCount, 0, 0);
        }
        public IEnumerable<VersionInfo> GetTopRecentReleaseVersionInfo(int topCount, int major, int minor)
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetTopRecentRelease";

                SqlCmd.Parameters.Add(new SqlParameter("@TopCount", SqlDbType.Int)
                    {
                        Value = topCount
                    });

                if (major > 0)
                {
                    SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                        {
                            Value = major
                        });

                    SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                        {
                            Value = minor
                        });
                }

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }
                finally
                {
                    sda.Dispose();
                    SqlCmd.Dispose();
                    DatabaseConnection.Dispose();
                }

                foreach (DataRow row in Output.Rows)
                    yield return new VersionInfo()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Major = Convert.ToInt32(row["Major"]),
                        Minor = Convert.ToInt32(row["Minor"]),
                        Build = Convert.ToInt32(row["Build"]),
                        Revision = Convert.ToInt32(row["Revision"]),
                        FriendlyNameShort = row["FriendlyNameShort"].ToString(),
                        FriendlyNameLong = row["FriendlyNameLong"].ToString(),
                        ReleaseDate = Convert.ToDateTime(row["ReleaseDate"]),
                        IsSupported = Convert.ToBoolean(row["IsSupported"]),
                        ReferenceLinks = GetReferenceLinks(
                            Convert.ToInt32(row["Major"]),
                            Convert.ToInt32(row["Minor"]),
                            Convert.ToInt32(row["Build"]),
                            Convert.ToInt32(row["Revision"])
                        )
                    };
            }
        }
        public VersionInfo GetMostRecentByMajorMinor(int major, int minor)
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetMostRecentByMajorMinor";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = minor
                    });

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return null;
                }
                finally
                {
                    sda.Dispose();
                    SqlCmd.Dispose();
                    DatabaseConnection.Dispose();
                }

                if (Output.Rows.Count == 0)
                    return null;

                return new VersionInfo()
                {
                    Id = Convert.ToInt32(Output.Rows[0]["Id"]),
                    Major = Convert.ToInt32(Output.Rows[0]["Major"]),
                    Minor = Convert.ToInt32(Output.Rows[0]["Minor"]),
                    Build = Convert.ToInt32(Output.Rows[0]["Build"]),
                    Revision = Convert.ToInt32(Output.Rows[0]["Revision"]),
                    FriendlyNameShort = Output.Rows[0]["FriendlyNameShort"].ToString(),
                    FriendlyNameLong = Output.Rows[0]["FriendlyNameLong"].ToString(),
                    ReleaseDate = Convert.ToDateTime(Output.Rows[0]["ReleaseDate"]),
                    IsSupported = Convert.ToBoolean(Output.Rows[0]["IsSupported"]),
                    ReferenceLinks = GetReferenceLinks(
                        Convert.ToInt32(Output.Rows[0]["Major"]),
                        Convert.ToInt32(Output.Rows[0]["Minor"]),
                        Convert.ToInt32(Output.Rows[0]["Build"]),
                        Convert.ToInt32(Output.Rows[0]["Revision"]))
                };
            }
        }
        public VersionInfo GetLowestSupportedByMajorMinor(int major, int minor)
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetLowestSupportedByMajorMinor";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = minor
                    });

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return null;
                }
                finally
                {
                    sda.Dispose();
                    SqlCmd.Dispose();
                    DatabaseConnection.Dispose();
                }

                if (Output.Rows.Count == 0)
                    return null;

                return new VersionInfo()
                {
                    Id = Convert.ToInt32(Output.Rows[0]["Id"]),
                    Major = Convert.ToInt32(Output.Rows[0]["Major"]),
                    Minor = Convert.ToInt32(Output.Rows[0]["Minor"]),
                    Build = Convert.ToInt32(Output.Rows[0]["Build"]),
                    Revision = Convert.ToInt32(Output.Rows[0]["Revision"]),
                    FriendlyNameShort = Output.Rows[0]["FriendlyNameShort"].ToString(),
                    FriendlyNameLong = Output.Rows[0]["FriendlyNameLong"].ToString(),
                    ReleaseDate = Convert.ToDateTime(Output.Rows[0]["ReleaseDate"]),
                    IsSupported = Convert.ToBoolean(Output.Rows[0]["IsSupported"]),
                    ReferenceLinks = GetReferenceLinks(
                        Convert.ToInt32(Output.Rows[0]["Major"]),
                        Convert.ToInt32(Output.Rows[0]["Minor"]),
                        Convert.ToInt32(Output.Rows[0]["Build"]),
                        Convert.ToInt32(Output.Rows[0]["Revision"]))
                };
            }
        }
        public IEnumerable<VersionInfo> GetMajorMinorReleases()
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetMajorMinorReleases";

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }
                finally
                {
                    sda.Dispose();
                    SqlCmd.Dispose();
                    DatabaseConnection.Dispose();
                }

                foreach (DataRow row in Output.Rows)
                    yield return new VersionInfo()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Major = Convert.ToInt32(row["Major"]),
                        Minor = Convert.ToInt32(row["Minor"]),
                        Build = Convert.ToInt32(row["Build"]),
                        Revision = Convert.ToInt32(row["Revision"]),
                        FriendlyNameShort = row["FriendlyNameShort"].ToString(),
                        FriendlyNameLong = row["FriendlyNameLong"].ToString(),
                        ReleaseDate = Convert.ToDateTime(row["ReleaseDate"]),
                        IsSupported = Convert.ToBoolean(row["IsSupported"]),
                        ReferenceLinks = GetReferenceLinks(
                            Convert.ToInt32(row["Major"]),
                            Convert.ToInt32(row["Minor"]),
                            Convert.ToInt32(row["Build"]),
                            Convert.ToInt32(row["Revision"])
                        )
                    };
            }
        }
        public IEnumerable<VersionInfo> GetRecentAndOldestSupportedVersions()
        {
            DataTable Output = new DataTable();

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(SqlCmd))
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionGetRecentAndOldestSupported";

                //SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                //    {
                //        Value = major
                //    });
                //SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                //    {
                //        Value = minor
                //    });

                try
                {
                    sda.Fill(Output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }
                finally
                {
                    sda.Dispose();
                    SqlCmd.Dispose();
                    DatabaseConnection.Dispose();
                }

                foreach (DataRow row in Output.Rows)
                    yield return new VersionInfo()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Major = Convert.ToInt32(row["Major"]),
                        Minor = Convert.ToInt32(row["Minor"]),
                        Build = Convert.ToInt32(row["Build"]),
                        Revision = Convert.ToInt32(row["Revision"]),
                        FriendlyNameShort = row["FriendlyNameShort"].ToString(),
                        FriendlyNameLong = row["FriendlyNameLong"].ToString(),
                        ReleaseDate = Convert.ToDateTime(row["ReleaseDate"]),
                        IsSupported = Convert.ToBoolean(row["IsSupported"]),
                        ReferenceLinks = GetReferenceLinks(
                            Convert.ToInt32(row["Major"]),
                            Convert.ToInt32(row["Minor"]),
                            Convert.ToInt32(row["Build"]),
                            Convert.ToInt32(row["Revision"])
                        )
                    };
            }
        }

        public bool AddVersionInfo(VersionInfo newVersionInfo)
        {
            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionAdd";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Major
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Minor
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Build
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@Revision", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Revision
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@FriendlyNameShort", SqlDbType.VarChar, 32)
                    {
                        Value = newVersionInfo.FriendlyNameShort
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@FriendlyNameLong", SqlDbType.VarChar, 128)
                    {
                        Value = newVersionInfo.FriendlyNameLong
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@ReleaseDate", SqlDbType.Date)
                    {
                        Value = newVersionInfo.ReleaseDate
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@IsSupported", SqlDbType.Bit)
                    {
                        Value = newVersionInfo.IsSupported
                    });

                try
                {
                    DatabaseConnection.Open();
                    SqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return false;
                }
                finally
                {
                    DatabaseConnection.Dispose();
                }

                // try to add the reference links, and if that fails then we need 
                // to do a programmatic rollback of the added version, because this 
                // operation wouldn't be atomic then and also fail the method
                //
                if (!AddReferenceLinks(newVersionInfo))
                {
                    DeleteVersionInfo(newVersionInfo);
                    return false;
                }

                // all was successful if we got to this point
                //
                return true;
            }
        }
        private bool AddReferenceLinks(VersionInfo newVersionInfo)
        {
            // short circuit here if there are no reference links to add 
            // then just return true as that is ok behavior
            //
            if (newVersionInfo.ReferenceLinks == null || 
                newVersionInfo.ReferenceLinks.Count() == 0)
                return true;

            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionAddReferenceLink";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Minor
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = newVersionInfo.Build
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@NewReferenceLink", SqlDbType.VarChar, 2000));

                // with the iterative nature of the reference links, we will 
                // open up the connection and keep it open for all of the redundant 
                // stored proc calls
                //
                try
                {
                    DatabaseConnection.Open();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return false;
                }

                foreach (string ReferenceLink in newVersionInfo.ReferenceLinks)
                {
                    SqlCmd.Parameters["@NewReferenceLink"].Value = ReferenceLink;

                    try
                    {
                        SqlCmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        // this is a failure condition and we need to fail the method and 
                        // return false
                        //
                        DatabaseConnection.Dispose();
                        SqlCmd.Dispose();

                        return false;
                    }
                }

                // at this point we are done with the connection so lets cleanup
                //
                DatabaseConnection.Dispose();
                SqlCmd.Dispose();

                // if we run into no problems here it is a implicit success
                // so just return true
                //
                return true;
            }
        }

        public bool ModifyVersionInfo(int major, int minor, int build, VersionInfo modifiedVersionInfo)
        {
            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionModify";

                SqlCmd.Parameters.Add(new SqlParameter("@MajorOld", SqlDbType.Int)
                    {
                        Value = major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@MinorOld", SqlDbType.Int)
                    {
                        Value = minor
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@BuildOld", SqlDbType.Int)
                    {
                        Value = build
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = modifiedVersionInfo.Major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = modifiedVersionInfo.Minor
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = modifiedVersionInfo.Build
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Revision", SqlDbType.Int)
                    {
                        Value = modifiedVersionInfo.Revision
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@FriendlyNameShort", SqlDbType.VarChar, 32)
                    {
                        Value = modifiedVersionInfo.FriendlyNameShort
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@FriendlyNameLong", SqlDbType.VarChar, 128)
                    {
                        Value = modifiedVersionInfo.FriendlyNameLong
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@ReleaseDate", SqlDbType.Date)
                    {
                        Value = modifiedVersionInfo.ReleaseDate
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@IsSupported", SqlDbType.Bit)
                    {
                        Value = modifiedVersionInfo.IsSupported
                    });

                SqlCmd.Parameters.Add(new SqlParameter("@ReturnVal", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    });

                try
                {
                    DatabaseConnection.Open();
                    SqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return false;
                }
                finally
                {
                    DatabaseConnection.Dispose();
                }

                if (SqlCmd.Parameters["@ReturnVal"].Value == DBNull.Value)
                    return false;

                if (Convert.ToInt32(SqlCmd.Parameters["@ReturnVal"].Value) != 0)
                    return false;

                return true;
            }
        }

        public void DeleteVersionInfo(VersionInfo versionInfo)
        {
            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.VersionRemove";

                SqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = versionInfo.Major
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = versionInfo.Minor
                    });
                SqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = versionInfo.Build
                    });

                try
                {
                    DatabaseConnection.Open();
                    SqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                }
                finally
                {
                    DatabaseConnection.Dispose();
                    SqlCmd.Dispose();
                }
            }
        }

        public IEnumerable<VersionBuild> GetBackFillBuilds()
        {
            DataTable output = new DataTable();

            using (SqlConnection databaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand sqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlCmd))
            {
                sqlCmd.Connection = databaseConnection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "dbo.BuildGetAllBackFill";

                try
                {
                    sda.Fill(output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    yield break;
                }

                foreach (DataRow row in output.Rows)
                    yield return new VersionBuild()
                    {
                        Major = Convert.ToInt32(row["Major"]),
                        Minor = Convert.ToInt32(row["Minor"]),
                        Build = Convert.ToInt32(row["Build"]),
                        Revision = Convert.ToInt32(row["Revision"])
                    };
            }
        }
        public VersionBuild GetRandomBackFillBuild()
        {
            DataTable output = new DataTable();

            using (SqlConnection databaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand sqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlCmd))
            {
                sqlCmd.Connection = databaseConnection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "dbo.BuildGetRandomBackFill";

                try
                {
                    sda.Fill(output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return null;
                }

                if (output.Rows.Count == 0)
                    return null;
                else
                    return new VersionBuild()
                    {
                        Major = Convert.ToInt32(output.Rows[0]["Major"]),
                        Minor = Convert.ToInt32(output.Rows[0]["Minor"]),
                        Build = Convert.ToInt32(output.Rows[0]["Build"]),
                        Revision = Convert.ToInt32(output.Rows[0]["Revision"])
                    };
            }
        }
        public int GetBackFillBuildsCount()
        {
            IEnumerable<VersionBuild> builds = GetBackFillBuilds();
            if (builds == null)
                return 0;
            else
                return builds.Count();
        }
        public bool DeleteBackFillBuild(VersionBuild versionBuild)
        {
            using (SqlConnection databaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = databaseConnection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "dbo.BuildRemoveBackFill";

                sqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = versionBuild.Major
                    });
                sqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = versionBuild.Minor
                    });
                sqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = versionBuild.Build
                    });

                try
                {
                    databaseConnection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return false;
                }

                return true;
            }
        }

        public VersionBuild GetBackFillBuild(int major, int minor, int build)
        {
            using (SqlConnection databaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand sqlCmd = new SqlCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlCmd))
            {
                sqlCmd.Connection = databaseConnection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "dbo.BuildGetBackFillBuild";

                sqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = major
                    });

                sqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = minor
                    });

                sqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = build
                    });

                DataTable output = new DataTable();

                try
                {
                    sda.Fill(output);
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                    return null;
                }

                if (output.Rows.Count == 0)
                    return null;
                else
                    return new VersionBuild()
                    {
                        Major = Convert.ToInt32(output.Rows[0]["Major"]),
                        Minor = Convert.ToInt32(output.Rows[0]["Minor"]),
                        Build = Convert.ToInt32(output.Rows[0]["Build"]),
                        Revision = Convert.ToInt32(output.Rows[0]["Revision"])
                    };
            }
        }
        public bool IsBackFillBuild(int major, int minor, int build)
        {
            return GetBackFillBuild(major, minor, build) != null;
        }

        private void AddVersionSearchTracking(int major, int minor, int build)
        {
            using (SqlConnection databaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand sqlCmd = new SqlCommand())
            {
                sqlCmd.Connection = databaseConnection;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "dbo.VersionAddSearchTracking";

                sqlCmd.Parameters.Add(new SqlParameter("@Major", SqlDbType.Int)
                    {
                        Value = major
                    });
                sqlCmd.Parameters.Add(new SqlParameter("@Minor", SqlDbType.Int)
                    {
                        Value = minor
                    });
                sqlCmd.Parameters.Add(new SqlParameter("@Build", SqlDbType.Int)
                    {
                        Value = build
                    });

                try
                {
                    databaseConnection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.LogMessage(new LogEntry() { Message = ex.Message, StackTrace = ex.StackTrace });
                }
            }
        }
    }
}
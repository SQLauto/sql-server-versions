using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SqlServerVersions.Logging
{
    public class DbLogger : ILogger
    {
        private string _connectionString;

        public DbLogger(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty");

            _connectionString = connectionString;

            if (!IsLoggerValid())
                throw new InvalidLoggerException();
        }

        public void LogMessage(LogEntry logEntry)
        {
            LogMessageToDatabase(logEntry);
        }

        public bool IsLoggerValid()
        {
            try
            {
                ConnectToLoggingDatabase();
            }
            catch
            {
                return false;
            }

            // if we get to this point then we were able to connect
            //
            return true;
        }

        /// <summary>
        /// attempt to open up a connect with the database to test connectivity
        /// </summary>
        private void ConnectToLoggingDatabase()
        {
            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
                DatabaseConnection.Open();
        }

        /// <summary>
        /// store the message in the database to persist
        /// </summary>
        /// <param name="logEntry">log entry</param>
        private void LogMessageToDatabase(LogEntry logEntry)
        {
            using (SqlConnection DatabaseConnection = new SqlConnection(_connectionString))
            using (SqlCommand SqlCmd = new SqlCommand())
            {
                SqlCmd.Connection = DatabaseConnection;
                SqlCmd.CommandType = CommandType.StoredProcedure;
                SqlCmd.CommandText = "dbo.ErrorLogAddMessage";

                SqlCmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.NVarChar)
                    {
                        Value = logEntry.Message
                    });

                if (!string.IsNullOrWhiteSpace(logEntry.MessageType))
                    SqlCmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.NVarChar)
                        {
                            Value = logEntry.MessageType
                        });

                if (!string.IsNullOrWhiteSpace(logEntry.StackTrace))
                    SqlCmd.Parameters.Add(new SqlParameter("@StackTrace", SqlDbType.NVarChar)
                        {
                            Value = logEntry.StackTrace
                        });

                DatabaseConnection.Open();
                SqlCmd.ExecuteNonQuery();
            }
        }
    }
}
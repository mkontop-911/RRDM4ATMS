using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
// using System.IO;


namespace RRDMFileAgentClasses
{
    public class RRDMAgentLog
    {

        public int SeqNo;
        public string SystemOfOrigin;
        public string SourceFileID;
        public string FileName;
        public int FileSize;
        public DateTime DateTimeReceived;
        public string FileHASH;
        public int LineCount;
        public string ArchivedPath;
        public int Status;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Uses AgentConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["AgentConnectionString"].ConnectionString;


       
        /// <summary>
        /// Truncate [dbo].[ReconcFileAgentLog] -- for POC purposes only
        /// </summary>
        public void TruncateTable()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdTruncate = "TRUNCATE TABLE [dbo].[ReconcFileAgentLog] ";
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdTruncate, conn))
                    {
                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                }
        }

        /// <summary>
        /// INSERT new record into RRDMAgentLog
        /// </summary>
        public int Insert()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdInsert = " INSERT INTO [dbo].[ReconcFileAgentLog] " +
                                    "( " +
                                    "[SystemOfOrigin], [SourceFileID], [FileName], [FileSize], [DateTimeReceived], " +
                                    "[FileHASH], [LineCount], [ArchivedPath], [Status] " +
                                    ")" +
                                " VALUES ( " +
                                    "@SystemOfOrigin, @SourceFileID, @FileName, @FileSize, @DateTimeReceived, @FileHASH, @LineCount, @ArchivedPath, @Status) " +
                                " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@SourceFileID", SourceFileID);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeReceived", DateTimeReceived);
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);
                        cmd.Parameters.AddWithValue("@ArchivedPath", ArchivedPath);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        SeqNo = (int)cmd.ExecuteScalar();
                        // int rows = cmd.ExecuteNonQuery();

                        if (SeqNo != 0)
                        {
                            ErrorFound = false;
                            ErrorOutput = "";
                        }
                        else
                        {
                            ErrorFound = true;
                            ErrorOutput = "An error occured while INSERTING in [ReconcFileAgentLog]... ";
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    SeqNo = 0;
                    ErrorFound = true;
                    ErrorOutput = "An error occured while INSERTING in [ReconcFileAgentLog]... " + ex.Message;
                }
            return (SeqNo);
        }

        /// <summary>
        /// UPDATE record in RRDMAgentLog
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Update(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ReconcFileAgentLog] SET "
                                    + " SystemOfOrigin = @SystemOfOrigin, SourceFileID = @SourceFileID, FileName = @FileName, "
                                    + " FileSize = @FileSize, DateTimeReceived = @DateTimeReceived, FileHASH = @FileHash, "
                                    + " LineCount = @LineCount, ArchivedPath = @ArchivedPath, Status = @Status "
                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@SourceFileID", SourceFileID);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeReceived", DateTimeReceived);
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);
                        cmd.Parameters.AddWithValue("@ArchivedPath", ArchivedPath);
                        cmd.Parameters.AddWithValue("@Status", Status);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while UPDATING [ReconcFileAgentLog]... " + ex.Message;
                }
        }

        /// <summary>
        /// DELETE record in RRDMAgentLog
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Delete(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[ReconcFileAgentLog] "
                                                        + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while DELETING from [ReconcFileAgentLog]... " + ex.Message;
                }
        }

        /// <summary>
        /// GetRecord given its SeqNo
        /// </summary>
        /// <param name="SeqNo"></param>
        public void GetRecordBySeqNo(Int32 SeqNo)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from [ReconcFileAgentLog]... ";

            string connectionString = ConfigurationManager.ConnectionStrings["AgentConnectionString"].ConnectionString;

            string SqlString = "SELECT * FROM [dbo].[ReconcFileAgentLog]"
                            + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ErrorFound = false;
                            ErrorOutput = "";

                            SeqNo = (int)rdr["SeqNo"];
                            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                            SourceFileID = (string)rdr["SourceFileID"];
                            FileName = (string)rdr["FileName"];
                            FileSize = (int)rdr["FileSize"];
                            DateTimeReceived = (DateTime)rdr["DateTimeReceived"];
                            FileHASH = (string)rdr["FileHASH"];
                            LineCount = (int)rdr["LineCount"];
                            Status = (int)rdr["Status"];
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING from [ReconcFileAgentLog]... " + ex.Message;
                }
        }

        /// <summary>
        /// Check if an entry with the specific FileHASH value exists in the table
        /// If it exists, the column values are copied in the properties of the class
        /// </summary>
        /// <param name="FileHash"></param>
        public void GetRecordByFileHASH(string FileHash)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [dbo].[ReconcFileAgentLog]"
                            + " WHERE FileHASH = @FileHASH ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FileHASH", FileHash);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            RecordFound = true;
                            while (rdr.Read())
                            {
                                SeqNo = (int)rdr["SeqNo"];
                                SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                                SourceFileID = (string)rdr["SourceFileID"];
                                FileName = (string)rdr["FileName"];
                                FileSize = (int)rdr["FileSize"];
                                DateTimeReceived = (DateTime)rdr["DateTimeReceived"];
                                FileHASH = (string)rdr["FileHASH"];
                                LineCount = (int)rdr["LineCount"];
                                Status = (int)rdr["Status"];
                            }
                        }
                        // Close Reader
                        rdr.Close();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING by FileHASH from [ReconcAgentLog]... " + ex.Message;
                }
        }
    }
}

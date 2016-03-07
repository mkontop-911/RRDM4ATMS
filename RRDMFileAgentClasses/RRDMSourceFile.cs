using System;
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
    public class RRDMSourceFile
    {
        public int SeqNo;
        public string SystemOfOrigin;
        public bool Enabled;
        public string SourceFileID;
        public int LayoutType;
        public string LayoutID;
        public string FileNameMask;
        public string ArchiveDirectory;
        public string Type;
        public string InitialTableName;
        public string WorkingTableName;
        public string SourceDirectory;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;


        public void ReadSourceFileRecord(string SysOfOrigin, string SrcFileID)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from RRDMSoureFile... ";

            string SqlString = "SELECT * FROM [dbo].[ReconcSourceFiles]"
                            + " WHERE SystemOfOrigin = @SysOfOrigin AND SourceFileID = @SrcFileID ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SysOfOrigin", SysOfOrigin);
                        cmd.Parameters.AddWithValue("@SrcFileID", SrcFileID);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ErrorFound = false;
                            ErrorOutput = "";

                            SeqNo = (int)rdr["SeqNo"];
                            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
                            Enabled = (bool)rdr["Enabled"];
                            SourceFileID = (string)rdr["SourceFileID"];
                            LayoutType = (int)rdr["LayoutType"];
                            LayoutID = (string)rdr["LayoutID"];
                            FileNameMask = (string)rdr["FileNameMask"];
                            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
                            Type = (string)rdr["Type"];
                            InitialTableName = (string)rdr["InitialTableName"];
                            WorkingTableName = (string)rdr["WorkingTableName"];
                            // TargetTableName = (string)rdr["TargetTableName"];
                            SourceDirectory = (string)rdr["SourceDirectory"];
                            Operator = (string)rdr["Operator"];
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
                    RecordFound = false;
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING from RRDMSourceFile table... " + ex.Message;
                }

        }
    }
}

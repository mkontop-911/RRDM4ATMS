using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
// using System.IO;


namespace RRDM4ATMs
{
    public class RRDMOutputFileMonitorLog : Logger
    {
        public RRDMOutputFileMonitorLog() : base() { }

        public int SeqNo;

        public int RMCycleNo; 
     
        public string SystemOfOrigin;

        public string OutputFile_Id;
        public string OutputFile_Version;

        public string StatusVerbose;
        public string FileName;
        public int FileSize;
        public DateTime DateTimeCreated;
    
        public string FileHASH;
        public int LineCount;
      
        public string DestinationPath;
        public string ArchivedPath;
        public int Status; // 1 Created OK 
                           // 0 Problem with creation 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable DataTableFileMonitorLog = new DataTable();

        // Uses AgentConnection String
        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        // Read Fields 
        private void ReadFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            RMCycleNo = (int)rdr["RMCycleNo"];
            SystemOfOrigin = (string)rdr["SystemOfOrigin"];

            OutputFile_Id = (string)rdr["OutputFile_Id"];
            OutputFile_Version = (string)rdr["OutputFile_Version"];
            
            StatusVerbose = (string)rdr["StatusVerbose"];
            FileName = (string)rdr["FileName"];
            FileSize = (int)rdr["FileSize"];
            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];
      
            FileHASH = (string)rdr["FileHASH"];
            LineCount = (int)rdr["LineCount"];
         
            DestinationPath = (string)rdr["DestinationPath"];
            ArchivedPath = (string)rdr["ArchivedPath"];
            Status = (int)rdr["Status"];

        }
        // READ AND FILL UP TABLE through File Id and Version
        public void ReadDataTableFileMonitorLogByFileIdAndVersion(string InOutputFile_Id, string InOutputFile_Version)
        {
          
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();
      
            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[OutputFileMonitorLog]"
               + " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version  "
               + " ORDER By DateTimeCreated DESC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@OutputFile_Id", InOutputFile_Id);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@OutputFile_Version", InOutputFile_Version);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableFileMonitorLog);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //InsertWReport72(InSignedId);
        }


        /// <summary>
        /// Truncate [dbo].[ReconcFileAgentLog] -- for POC purposes only
        /// </summary>
        public void TruncateTable()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdTruncate = "TRUNCATE TABLE [dbo].[OutputFileMonitorLog] ";
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdTruncate, conn))
                    {
                        cmd.ExecuteNonQuery();
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
        /// INSERT new record into OutputFileMonitorLog
        /// </summary>
        public int InsertFileRecordLog()
        {
            
            ErrorFound = false;
            ErrorOutput = "";
            //RMCycleNo
            string cmdInsert = " INSERT INTO [dbo].[OutputFileMonitorLog] "
                                    +"( " 
                                    + "[RMCycleNo],  "
                                    + "[SystemOfOrigin], [OutputFile_Id], [OutputFile_Version], "
                                    + "[StatusVerbose],[FileName], [FileSize], [DateTimeCreated], "
                                    + " "
                                    + "[FileHASH], [LineCount],  "
                                    + "[DestinationPath], [ArchivedPath], [Status] "
                                    + ")"
                                    + " VALUES ( "
                                + "@RMCycleNo,  "
                                    + "@SystemOfOrigin, @OutputFile_Id, @OutputFile_Version,"
                                    + " @StatusVerbose, @FileName, @FileSize, @DateTimeCreated,  @FileHASH, @LineCount,"
                                    + " @DestinationPath, @ArchivedPath, @Status ) " 
                                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";


            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);
                        cmd.Parameters.AddWithValue("@OutputFile_Id", OutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", OutputFile_Version);
                        
                        cmd.Parameters.AddWithValue("@StatusVerbose", StatusVerbose);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeCreated", DateTimeCreated);
                   
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);
                        
                        cmd.Parameters.AddWithValue("@DestinationPath", DestinationPath);
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
                            ErrorOutput = "An error occured while INSERTING in [OutputFileMonitorLog]... ";
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
                    ErrorOutput = "An error occured while INSERTING in [OutputFileMonitorLog]... " + ex.Message;
                }
            return (SeqNo);
        }

        /// <summary>
        /// UPDATE record in RRDMOutputFileMonitorLog
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
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[OutputFileMonitorLog] SET "
                                    + " SystemOfOrigin = @SystemOfOrigin, " 
                                    + " OutputFile_Id = @OutputFile_Id , OutputFile_Version = @OutputFile_Version ,"
                                    + " StatusVerbose = @StatusVerbose , FileName = @FileName, "
                                    + " FileSize = @FileSize, DateTimeCreated = @DateTimeCreated, " 
                                    + " FileHASH = @FileHash,  "
                                    + " LineCount = @LineCount, "
                                    + " DestinationPath = @DestinationPath, "
                                     + " ArchivedPath = @ArchivedPath, "
                                    + " Status = @Status "
                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);

                        cmd.Parameters.AddWithValue("@OutputFile_Id", OutputFile_Id);
                        cmd.Parameters.AddWithValue("@OutputFile_Version", OutputFile_Version);
                        
                        cmd.Parameters.AddWithValue("@StatusVerbose", StatusVerbose);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeCreated", DateTimeCreated);
                      
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);

                        cmd.Parameters.AddWithValue("@DestinationPath", DestinationPath);
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
                    ErrorOutput = "An error occured while UPDATING [OutputFileMonitorLog]... " + ex.Message;
                }
        }

        /// <summary>
        /// DELETE record in OutputFileMonitorLog
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
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[OutputFileMonitorLog] "
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
                    ErrorOutput = "An error occured while DELETING from [OutputFileMonitorLog]... " + ex.Message;
                }
        }

        /// <summary>
        /// GetRecord given its SeqNo
        /// </summary>
        /// <param name="SeqNo"></param>
        public void GetRecordBySeqNo(int SeqNo)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from [OutputFileMonitorLog]... ";

            // string connectionString = AppConfig.GetConnectionString("AgentConnectionString");

            string SqlString = "SELECT * FROM [dbo].[OutputFileMonitorLog]"
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

                            ReadFields(rdr);

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
                    ErrorOutput = "An error occured while READING from [OutputFileMonitorLog]... " + ex.Message;
                }
        }


        /// <summary>
        /// Check if an entry with the specific FileHASH value exists in the table
        /// and that the status = 1 (meaning the file was processed successfully)
        /// If it exists, the column values are copied in the properties of the class
        /// </summary>
        /// <param name="FileHash"></param>
        public void GetRecordByFileHASH(string FileHash)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [dbo].[OutputFileMonitorLog]"
                            + " WHERE FileHASH = @FileHASH AND Status = 1 ";

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
                                ReadFields(rdr);

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
                    ErrorOutput = "An error occured while READING by FileHASH from [OutputFileMonitorLog]... " + ex.Message;
                }
        }

       
    }
}



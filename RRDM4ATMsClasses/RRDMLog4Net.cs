using System;
using System.Security.Principal;
using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMLog4Net
    {
        public int SeqNo;
        public DateTime Date;       
        public string Logger;
        public string Message;
        public string Parameters;
        public string Operator;

        public int ErrorNo; 

        string ErrorString;
        //public string Old_ErrorString;

        // Define the data table 
        public DataTable ErrorsSelected = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Methods 
        // READ View ATMs Main vs Users For ALL ATMS specific authorised user 
        // FILL UP A TABLE 


        // Methods 
        // READ Errors Loger Table 
        // FILL UP A TABLE 

        public void ReadRRDMLog4NetAndFillTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsSelected = new DataTable();
            ErrorsSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ErrorsSelected.Columns.Add("SeqNo", typeof(int));
            ErrorsSelected.Columns.Add("Date", typeof(DateTime));
            ErrorsSelected.Columns.Add("Logger", typeof(string));
            ErrorsSelected.Columns.Add("Message", typeof(string));
            ErrorsSelected.Columns.Add("Parameters", typeof(string));
           
                SqlString = "SELECT *"
                     + " FROM [ATMS].[dbo].[ErrorCaptureLogRRDM]" 
                     //+ " WHERE Operator=@Operator "
                      + " ORDER BY SeqNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            Date = (DateTime)rdr["Date"];

                            Logger = (string)rdr["Logger"];

                            Message = (string)rdr["Message"];
                            Parameters = (string)rdr["Parameters"];
                            Operator = (string)rdr["Operator"];
                            
                            DataRow RowSelected = ErrorsSelected.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Date"] = Date;

                            RowSelected["Logger"] = (string)rdr["Logger"];
                            RowSelected["Message"] = (string)rdr["Message"];
                        

                            // ADD ROW
                            ErrorsSelected.Rows.Add(RowSelected);

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

                    CatchDetails(ex);
                }
        }

        // Methods 
        // READ SPECIFIC ERROR
        // 
        public void ReadRRDMLog4NetSpecific(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
              + " FROM [ATMS].[dbo].[ErrorCaptureLogRRDM] "
              + " WHERE SeqNo=@SeqNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            SeqNo = (int)rdr["SeqNo"];
                            Date = (DateTime)rdr["Date"];

                            Logger = (string)rdr["Logger"];

                            Message = (string)rdr["Message"];
                            Parameters = (string)rdr["Parameters"];
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

                    CatchDetails(ex);

                }
        }


        // Methods 
        // CreateAndInsert 
        // 
        public void CreateAndInsertRRDMLog4NetMessage(Exception pException, string InLogger, string InParameters)
        {
            //Initialise Fields 
            //
            Logger = InLogger;
            Parameters = InParameters; 

            StringBuilder lErrorStr = new StringBuilder();

            lErrorStr.Append("ERROR_MESSAGE: ");
            lErrorStr.Append(pException.Message);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("SOURCE: ");
            lErrorStr.Append(pException.Source);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("TARGET_TYPE: ");
            lErrorStr.Append(pException.TargetSite.ReflectedType.Name);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("TARGET_METHOD: ");
            lErrorStr.Append(pException.TargetSite.Name.ToString());
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("STACKTRACE: ");
            lErrorStr.Append(pException.StackTrace);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("PARAMETERS: ");
            lErrorStr.Append(Parameters);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("USER: ");
            lErrorStr.Append(WindowsIdentity.GetCurrent().Name);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("MACHINE NAME: ");
            lErrorStr.Append(Environment.MachineName);
            lErrorStr.Append(Environment.NewLine);

            ErrorString = lErrorStr.ToString();
     
            //
            // Insert Error String
            //
            ErrorNo = InsertInRRDMLog4Net(ErrorString);
            ReadRRDMLog4NetSpecific(ErrorNo - 1);
            if (ErrorString == Message)
            {
                //System.Windows.Forms.MessageBox.Show("There is a system error  "
                //  + " . Application will be aborted! Call support to take care. ");
                //Environment.Exit(0);
            }
        }
     
        //
        // INSERT New Record in Main Table 
        //
        public int InsertInRRDMLog4Net(string InMessage)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ErrorCaptureLogRRDM]"
                + " ([Date], "
                + " [Logger],"
                + " [Message],"
                + " [Parameters]"
                +" )"
                + " VALUES"
                + " (@Date,"
                + " @Logger,"
                + " @Message ,"
                + " @Parameters "
                + ")"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        // Only necessary fileds.. the rest will be update by JTM

                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Logger", Logger);
                        cmd.Parameters.AddWithValue("@Message", InMessage);
                        cmd.Parameters.AddWithValue("@Parameters", Parameters);
                      
                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);
                }
            return (SeqNo);
        }

        // Methods 
        // CreateAndInsert 
        // 
        public void CreateAndInsertRRDMLog4NetMessage_2(Exception pException, string InLogger
                                                                    , string InParameters, string InUserId)
        {
            //Initialise Fields 
            //
            Logger = InLogger;
            Parameters = InParameters;

            StringBuilder lErrorStr = new StringBuilder();

            lErrorStr.Append("ERROR_MESSAGE: ");
            lErrorStr.Append(pException.Message);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("SOURCE: ");
            lErrorStr.Append(pException.Source);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("TARGET_TYPE: ");
            lErrorStr.Append(pException.TargetSite.ReflectedType.Name);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("TARGET_METHOD: ");
            lErrorStr.Append(pException.TargetSite.Name.ToString());
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("STACKTRACE: ");
            lErrorStr.Append(pException.StackTrace);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("PARAMETERS: ");
            lErrorStr.Append(Parameters);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("USER: ");
            lErrorStr.Append(WindowsIdentity.GetCurrent().Name);
            lErrorStr.Append(Environment.NewLine);

            lErrorStr.Append("MACHINE NAME: ");
            lErrorStr.Append(Environment.MachineName);
            lErrorStr.Append(Environment.NewLine);

            ErrorString = lErrorStr.ToString();


            //
            // Insert Error String
            //
            ErrorNo = InsertInRRDMLog4Net_2(ErrorString);
            ReadRRDMLog4NetSpecific(ErrorNo - 1);
            if (ErrorString == Message)
            {
                //System.Windows.Forms.MessageBox.Show("There is a system error  "
                //  + " . Application will be aborted! Call support to take care. ");
                //Environment.Exit(0);
            }
        }

        //
        // INSERT New Record in Main Table 
        //
        public int InsertInRRDMLog4Net_2(string InMessage)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ErrorCaptureLogRRDM]"
                + " ([Date], "
                + " [Logger],"
                + " [Message],"
                + " [Parameters]"
                + " )"
                + " VALUES"
                + " (@Date,"
                + " @Logger,"
                + " @Message ,"
                + " @Parameters "
                + ")"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        // Only necessary fileds.. the rest will be update by JTM

                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Logger", Logger);
                        cmd.Parameters.AddWithValue("@Message", InMessage);
                        cmd.Parameters.AddWithValue("@Parameters", Parameters);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);
                }
            return (SeqNo);
        }
        // Catch Details 
        //
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                //System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                       //  + " . Application will be aborted! Call controller to take care. ");
            }
        }
    }
}



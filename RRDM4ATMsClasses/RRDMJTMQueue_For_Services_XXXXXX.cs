using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    /// <summary> JTMQueueStruct
    /// Declare the structure to contain the record fields
    /// </summary>
    public struct JTMQueueStruct_For_Services
    {
        public int MsgID;
        public DateTime MsgDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;
        public string ServiceId;
        public string ServiceName;
        public int Priority;

        public int Stage;
        public int ResultCode;
        public string ResultMessage;
        public DateTime ProcessStart;

        public DateTime ProcessEnd;
        public string Operator;
    }

    #region JTM Stage Class
    public static class JTMQueueStage_For_Services
    {
        public const int Const_InQueue = 0;
        public const int Const_WorkInProgress = 1;
        public const int Const_TransferInProgress = 2;
        public const int Const_TransferFinished = 3;
        public const int Const_WaitingForParsing = 4;
        public const int Const_ParserInProgress = 5;
        public const int Const_ParserFinished = 6;
        public const int Const_Aborted = 98;
        public const int Const_Finished = 99;
    
        private struct JTMStageStruct
        {
            public int num;
            public string message;
            public JTMStageStruct(int num, string message)
            {
                this.num = num;
                this.message = message;
            }
        }

        private static JTMStageStruct[] STAGE_LIST = new JTMStageStruct[] 
        {
            new JTMStageStruct(Const_InQueue, "In Queue"), 
            new JTMStageStruct(Const_WorkInProgress, "RetrievedFromQueue"), 
            new JTMStageStruct(Const_TransferInProgress, "TransferInProgress"), 
            new JTMStageStruct(Const_TransferFinished, "TransferFinished"), 
            new JTMStageStruct(Const_WaitingForParsing, "WaitingForParsing"), 
            new JTMStageStruct(Const_ParserInProgress, "ParserInProgress"), 
            new JTMStageStruct(Const_ParserFinished, "ParserFinished"), 
            new JTMStageStruct(Const_Aborted, "AbortedDueToError"),
            new JTMStageStruct(Const_Finished, "RequestFinished") 
        };

        public static string getStageFromNumber(int errNum)
        {
            foreach (JTMStageStruct er in STAGE_LIST)
            {
                if (er.num == errNum) return er.message;
            }
            return "Stage: Unknown, " + errNum;
        }
    }
    #endregion

    public static class JTMQueueResult_For_Services
    {
        //  Result Codes
        //  0 - Success
        //  1 - Error
        public const int Success = 0;
        public const int Failure = 1;
    }

    public static class JTMQueueCommand_For_Services
    {
        //  Commands
        public const string Cmd_FETCH = "FETCH";
        // public const string Cmd_FETCHDEL = "FETCHDEL";
        public const string Cmd_ATMSTATUS = "ATMSTATUS";
    }

    public class RRDMJTMQueue_For_Services_XXXXXX
    {
        public JTMQueueStruct_For_Services QueueRec = new JTMQueueStruct_For_Services();

        #region Properies

        public int MsgID;
        public DateTime MsgDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;
        // InCommand : StartService
        //           : EndService

        public string ServiceId; // 11 = Start loading Journals, 12 = start loading IST etc Parameter xxx
                                 // Parameter 915
        public string ServiceName; 

        public int Priority;
     
        public int Stage;

        public int ResultCode;
        //  -1 - Not started yet
        //  0 - Success
        //  1 - Error
        public string ResultMessage;
        public DateTime ProcessStart;
       
        public DateTime ProcessEnd;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        #endregion

        // Define the data table 
        public DataTable QueueServiceTable = new DataTable();

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

        // READ a single JTMQueue row which is the oldest in the queue and with the highest priority
        public void ReadSingleJTMQueueByPriority()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT TOP 1 * FROM [dbo].[JTMQueue_For_Services] " +
                               " WHERE Stage = @Stage" +
                               " ORDER BY [Priority], [MsgID] ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Stage", JTMQueueStage.Const_InQueue); // get only unprocessed records

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read record Details (fill in both the QueueRec structure and the free standing properties)
                            QueueRec.MsgID = MsgID = (int)rdr["MsgID"];
                            QueueRec.MsgDateTime = MsgDateTime = (DateTime)rdr["MsgDateTime"];
                            QueueRec.RequestorID = RequestorID = (string)rdr["RequestorID"];
                            QueueRec.RequestorMachine = RequestorMachine = (string)rdr["RequestorMachine"];
                            QueueRec.Command = Command = (string)rdr["Command"];
                            QueueRec.ServiceId = ServiceId = (string)rdr["ServiceId"];
                            QueueRec.ServiceName = ServiceName = (string)rdr["ServiceName"];
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                       
                            QueueRec.Stage = Stage = (int)rdr["Stage"];
                            QueueRec.ResultCode = ResultCode = (int)rdr["ResultCode"];
                            QueueRec.ResultMessage = ResultMessage = (string)rdr["ResultMessage"];
                            QueueRec.ProcessStart = ProcessStart = (DateTime)rdr["ProcessStart"];
                            QueueRec.ProcessEnd = ProcessEnd = (DateTime)rdr["ProcessEnd"];
                            QueueRec.Operator = Operator = (string)rdr["Operator"];
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

        // READ JTMQueue by MsgID
        public void ReadJTMQueueByMsgID(int InMsgID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                  + " FROM [dbo].[JTMQueue_For_Services] "
                  + " WHERE MsgID = @MsgID";
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MsgID", InMsgID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read record Details (fill in both the QueueRec structure and the free standing properties)
                            QueueRec.MsgID = MsgID = (int)rdr["MsgID"];
                            QueueRec.MsgDateTime = MsgDateTime = (DateTime)rdr["MsgDateTime"];
                            QueueRec.RequestorID = RequestorID = (string)rdr["RequestorID"];
                            QueueRec.RequestorMachine = RequestorMachine = (string)rdr["RequestorMachine"];
                            QueueRec.Command = Command = (string)rdr["Command"];
                            QueueRec.ServiceId = ServiceId = (string)rdr["ServiceId"];
                            QueueRec.ServiceName = ServiceName = (string)rdr["ServiceName"];
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                         
                            QueueRec.Stage = Stage = (int)rdr["Stage"];
                            QueueRec.ResultCode = ResultCode = (int)rdr["ResultCode"];
                            QueueRec.ResultMessage = ResultMessage = (string)rdr["ResultMessage"];
                            QueueRec.ProcessStart = ProcessStart = (DateTime)rdr["ProcessStart"];
                            QueueRec.ProcessEnd = ProcessEnd = (DateTime)rdr["ProcessEnd"];
                            QueueRec.Operator = Operator = (string)rdr["Operator"];
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

       

        // Insert  Record In JTMQueue Using Struct
        public void InsertRecordInJTMQueueUsingStruct(JTMQueueStruct_For_Services JTMQRec)
        {
            MsgID = QueueRec.MsgID = JTMQRec.MsgID;
            MsgDateTime = QueueRec.MsgDateTime = JTMQRec.MsgDateTime;
            RequestorID = QueueRec.RequestorID = JTMQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = JTMQRec.RequestorMachine;
            Command = QueueRec.Command = JTMQRec.Command;
            ServiceId = QueueRec.ServiceId = JTMQRec.ServiceId;
            ServiceName = QueueRec.ServiceName = JTMQRec.ServiceName;

            Priority = QueueRec.Priority = JTMQRec.Priority;
         
          
            Stage = QueueRec.Stage = JTMQRec.Stage;
            ResultCode = QueueRec.ResultCode = JTMQRec.ResultCode;
            ResultMessage = QueueRec.ResultMessage = JTMQRec.ResultMessage;
            ProcessStart = QueueRec.ProcessStart = JTMQRec.ProcessStart;
          
            Operator = QueueRec.Operator = JTMQRec.Operator;

            this.InsertNewRecordInJTMQueue();
        }

        // Insert NEW Record in JTMQueue

        public int InsertNewRecordInJTMQueue()
        {
          
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[JTMQueue_For_Services]"
                + " ([MsgDateTime],[RequestorID],[RequestorMachine],[Command],"
                + "[ServiceId],[ServiceName],[Priority],"
                + "[Operator] )"
                + " VALUES"
                + " (@MsgDateTime,@RequestorID,@RequestorMachine,@Command,"
                + "@ServiceId,@ServiceName,@Priority,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        // Only necessary fileds.. the rest will be update by JTM
                        cmd.Parameters.AddWithValue("@MsgDateTime", MsgDateTime);
                        cmd.Parameters.AddWithValue("@RequestorID", RequestorID);
                        cmd.Parameters.AddWithValue("@RequestorMachine", RequestorMachine);
                        cmd.Parameters.AddWithValue("@Command", Command);
                        cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                   
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        MsgID = (int)cmd.ExecuteScalar();
                     
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    ErrorFound = true;

                    CatchDetails(ex);
                }

            return MsgID;
        }

        // UPDATE Update Record In JTMQueue Using Struct
        public void UpdateRecordInJTMQueueUsingStruct(JTMQueueStruct_For_Services JTMQRec)
        {
            MsgID = QueueRec.MsgID = JTMQRec.MsgID;
            MsgDateTime = QueueRec.MsgDateTime = JTMQRec.MsgDateTime;
            RequestorID = QueueRec.RequestorID = JTMQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = JTMQRec.RequestorMachine;
            Command = QueueRec.Command = JTMQRec.Command;
            ServiceId = QueueRec.ServiceId = JTMQRec.ServiceId;
            ServiceName = QueueRec.ServiceName = JTMQRec.ServiceName;
            Priority = QueueRec.Priority = JTMQRec.Priority;
         
           
            Stage = QueueRec.Stage = JTMQRec.Stage;
            ResultCode = QueueRec.ResultCode = JTMQRec.ResultCode;
            ResultMessage = QueueRec.ResultMessage = JTMQRec.ResultMessage;
            ProcessStart = QueueRec.ProcessStart = JTMQRec.ProcessStart;
          
            ProcessEnd = QueueRec.ProcessEnd = JTMQRec.ProcessEnd;
            Operator = QueueRec.Operator = JTMQRec.Operator;

            this.UpdateRecordInJTMQueueByMsgID(MsgID);
        }

        // UPDATE Update Record In JTMQueue By MsgID
        public void UpdateRecordInJTMQueueByMsgID(int InMsgID)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.JTMQueue_For_Services SET "
                             + " MsgDateTime = @MsgDateTime, RequestorID = @RequestorID, RequestorMachine = @RequestorMachine,"
                             + "Command = @Command, ServiceId = @ServiceId,ServiceName = @ServiceName, Priority = @Priority,"
                             + "Stage = @Stage, ResultCode = @ResultCode, "
                             + "ResultMessage = @ResultMessage, "
                             + "ProcessStart = @ProcessStart,  "
                             + "ProcessEnd = @ProcessEnd, Operator = @Operator  "
                             + " WHERE MsgID = @MsgID", conn))
                    {

                        cmd.Parameters.AddWithValue("@MsgID", InMsgID);
                        cmd.Parameters.AddWithValue("@MsgDateTime", MsgDateTime);
                        cmd.Parameters.AddWithValue("@RequestorID", RequestorID);
                        cmd.Parameters.AddWithValue("@RequestorMachine", RequestorMachine);

                        cmd.Parameters.AddWithValue("@Command", Command);
                        cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                     
                        cmd.Parameters.AddWithValue("@Stage", Stage);
                        cmd.Parameters.AddWithValue("@ResultCode", ResultCode);
                        cmd.Parameters.AddWithValue("@ResultMessage", ResultMessage);

                        cmd.Parameters.AddWithValue("@ProcessStart", ProcessStart);
                       
                        cmd.Parameters.AddWithValue("@ProcessEnd", ProcessEnd);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        cmd.ExecuteNonQuery();
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

        // DELETE Record In JTMQueue By MsgID
        public void DeleteRecordInJTMQueueByMsgID(int InMsgID)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.JTMQueue_For_Services "
                            + " WHERE MsgID = @MsgID ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MsgID", InMsgID);
                                      
                        cmd.ExecuteNonQuery();
                       
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
        }
   
        // TODO - Remove this when testing is done!!!
        // Delete ALL Records in RRDMJTMQueue table (for TESTING  ONLY!!!)
        public int DeleteAllRecordsInJTMQueue()
        {
            int rows = 0;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.JTMQueue_For_Services ", conn))
                    {
                        rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return (rows);
        }

        // Catch Details 
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
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }

        
        }
    }
}

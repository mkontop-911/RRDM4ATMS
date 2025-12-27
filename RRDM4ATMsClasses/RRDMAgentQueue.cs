using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    /// <summary> AgentQueueStruct
    /// Declare the structure to contain the table fields
    /// </summary>
    public struct AgentQueueStruct
    {
        public int ReqID;
        public DateTime ReqDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;
        public string ServiceId;
        public string ServiceName;
        public int Priority;
        public int ReqStatusCode;
        public int CmdStatusCode;
        public string CmdStatusMessage;
        public DateTime CmdExecStarted;
        public DateTime CmdExecFinished;
        public string Operator;
        public int OriginalReqID;
        public string OriginalRequestorID;
        public bool MessageSent; 
    }


    #region Agent Request Status Class
    public static class AgentStatus
    {
        public const int Req_WaitInQueue = 0;
        public const int Req_WorkInProgress = 1;
        public const int Req_Invalid = 97;
        public const int Req_Aborted = 98;
        public const int Req_Finished = 99;
    
        private struct AgentStatusStruct
        {
            public int statusCode;
            public string statusText;
            public AgentStatusStruct(int status, string text)
            {
                this.statusCode = status;
                this.statusText = text;
            }
        }

        private static AgentStatusStruct[] STATUS_LIST = new AgentStatusStruct[]
        {
            new AgentStatusStruct(Req_WaitInQueue, "Waiting in Agent Queue"),
            new AgentStatusStruct(Req_WorkInProgress, "Request Processing in Progress"),
            new AgentStatusStruct(Req_Invalid, "Invalid Request Command"),
            new AgentStatusStruct(Req_Aborted, "Request Processing Aborted because of Error(s)"),
            new AgentStatusStruct(Req_Finished, "Successful Completion") 
        };

        public static string getStatusTextFromCode(int statusCode)
        {
            foreach (AgentStatusStruct st in STATUS_LIST)
            {
                if (st.statusCode == statusCode) return st.statusText;
            }
            return "Stage: Unknown, " + statusCode;
        }
    }
    #endregion

    public static class AgentRequestCommand
    {
        //  Commands
        public const string Cmd_ServiceStart = "SERVICE_START";
        public const string Cmd_ServiceStartMonitor = "SERVICE_START_AND_MONITOR";
        public const string Cmd_ServiceStop = "SERVICE_STOP";
        public const string Cmd_ServiceStatus = "SERVICE_STATUS";
    }

    public class RRDMAgentQueue : Logger
    {
        public RRDMAgentQueue() : base() { }

        public AgentQueueStruct QueueRec = new AgentQueueStruct();

        #region Properies

        public int ReqID;
        public DateTime ReqDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command; // "SERVICE_START", "SERVICE_STOP", "SERVICE_STATUS"
        public string ServiceId;    // Parameter 915, Occurance ID
        public string ServiceName;  // Parameter 915, Occurance Name
        public int Priority;        // 0: Highest
        public int ReqStatusCode;   // 0:Waiting in Queue, 1:Processing in Progress,
                                    // 97: Invalid Command, 98:Requset Aborted/Canceled
                                    // 99: Finished..
        public int CmdStatusCode;   // -1: Not started yet, 0: Success, 1:Service NOT installed, ...
        public string CmdStatusMessage;

        //public DateTime ReqProcessStart;
        //public DateTime ReqProcessStop;
        public DateTime CmdExecStarted;
        public DateTime CmdExecFinished;

        public string Operator; // need to input

        public int OriginalReqID; // used in AgentMonitor when cmd=STARTANDMONITOR
        public string OriginalRequestorID; // used in AgentMonitor when cmd=STARTANDMONITOR
        public bool MessageSent; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        #endregion

        // Define the data table 
        public DataTable AgentServiceTable = new DataTable();
        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // READ a single AgentQueue row which is the oldest in the queue and with the highest priority
        public void ReadSingleAgentRequestByPriority()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT TOP 1 * FROM [dbo].[AgentQueue] " +
                               " WHERE ReqStatusCode = @ReqStatusCode" +
                               " ORDER BY [Priority], [ReqID] ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReqStatusCode", AgentStatus.Req_WaitInQueue); // get only unprocessed records

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read record Details (fill in both the QueueRec structure and the free standing properties)
                            QueueRec.ReqID = ReqID = (int)rdr["ReqID"];
                            QueueRec.ReqDateTime = ReqDateTime = (DateTime)rdr["ReqDateTime"];
                            QueueRec.RequestorID = RequestorID = (string)rdr["RequestorID"];
                            QueueRec.RequestorMachine = RequestorMachine = (string)rdr["RequestorMachine"];
                            QueueRec.Command = Command = (string)rdr["Command"];
                            QueueRec.ServiceId = ServiceId = (string)rdr["ServiceId"];
                            QueueRec.ServiceName = ServiceName = (string)rdr["ServiceName"];
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                            QueueRec.ReqStatusCode = ReqStatusCode = (int)rdr["ReqStatusCode"];
                            QueueRec.CmdStatusCode = CmdStatusCode = (int)rdr["CmdStatusCode"];
                            QueueRec.CmdStatusMessage = CmdStatusMessage = (string)rdr["CmdStatusMessage"];
                            QueueRec.CmdExecStarted = CmdExecStarted = (DateTime)rdr["CmdExecStarted"];
                            QueueRec.CmdExecFinished = CmdExecFinished = (DateTime)rdr["CmdExecFinished"];
                            QueueRec.Operator = Operator = (string)rdr["Operator"];
                            QueueRec.OriginalReqID = OriginalReqID = (int)rdr["OriginalReqId"];
                            QueueRec.OriginalRequestorID = OriginalRequestorID = (string)rdr["OriginalRequestorID"];
                            QueueRec.MessageSent = MessageSent = (bool)rdr["MessageSent"];
                            
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

        // READ AgentQueue by Selection Criteria
        public void ReadAgentQueueBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                  + " FROM [dbo].[AgentQueue] "
                  + InSelectionCriteria; 
               //   + " WHERE ReqID = @ReqID";
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                      //  cmd.Parameters.AddWithValue("@ReqID", InReqID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read record Details (fill in both the QueueRec structure and the free standing properties)
                            QueueRec.ReqID = ReqID = (int)rdr["ReqID"];
                            QueueRec.ReqDateTime = ReqDateTime = (DateTime)rdr["ReqDateTime"];
                            QueueRec.RequestorID = RequestorID = (string)rdr["RequestorID"];
                            QueueRec.RequestorMachine = RequestorMachine = (string)rdr["RequestorMachine"];
                            QueueRec.Command = Command = (string)rdr["Command"];
                            QueueRec.ServiceId = ServiceId = (string)rdr["ServiceId"];
                            QueueRec.ServiceName = ServiceName = (string)rdr["ServiceName"];
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                            QueueRec.ReqStatusCode = ReqStatusCode = (int)rdr["ReqStatusCode"];
                            QueueRec.CmdStatusCode = CmdStatusCode = (int)rdr["CmdStatusCode"];
                            QueueRec.CmdStatusMessage = CmdStatusMessage = (string)rdr["CmdStatusMessage"];
                            QueueRec.CmdExecStarted = CmdExecStarted = (DateTime)rdr["CmdExecStarted"];
                            QueueRec.CmdExecFinished = CmdExecFinished = (DateTime)rdr["CmdExecFinished"];
                            QueueRec.Operator = Operator = (string)rdr["Operator"];
                            QueueRec.OriginalReqID = OriginalReqID = (int)rdr["OriginalReqId"];
                            QueueRec.OriginalRequestorID = OriginalRequestorID = (string)rdr["OriginalRequestorID"];
                            QueueRec.MessageSent = MessageSent = (bool)rdr["MessageSent"];
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

        // Insert  Record In AgentQueue Using Struct
        public void InsertRecordInAgentQueueUsingStruct(AgentQueueStruct AgentQRec)
        {
            ReqID = QueueRec.ReqID = AgentQRec.ReqID;
            ReqDateTime = QueueRec.ReqDateTime = AgentQRec.ReqDateTime;
            RequestorID = QueueRec.RequestorID = AgentQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = AgentQRec.RequestorMachine;
            Command = QueueRec.Command = AgentQRec.Command;
            ServiceId = QueueRec.ServiceId = AgentQRec.ServiceId;
            ServiceName = QueueRec.ServiceName = AgentQRec.ServiceName;
            Priority = QueueRec.Priority = AgentQRec.Priority;
            ReqStatusCode = QueueRec.ReqStatusCode = AgentQRec.ReqStatusCode;
            CmdStatusCode = QueueRec.CmdStatusCode = AgentQRec.CmdStatusCode;
            CmdStatusMessage = QueueRec.CmdStatusMessage = AgentQRec.CmdStatusMessage;
            CmdExecStarted = QueueRec.CmdExecStarted = AgentQRec.CmdExecStarted;
            CmdExecFinished = QueueRec.CmdExecFinished = AgentQRec.CmdExecFinished;
            Operator = QueueRec.Operator = AgentQRec.Operator;
            OriginalReqID = QueueRec.OriginalReqID = AgentQRec.OriginalReqID;
            OriginalRequestorID = QueueRec.OriginalRequestorID = AgentQRec.OriginalRequestorID;

            this.InsertNewRecordInAgentQueue();
        }

        // Insert NEW Record in AgentQueue
        public int InsertNewRecordInAgentQueue()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[AgentQueue]"
                + " ([ReqDateTime],[RequestorID],[RequestorMachine],[Command],"
                + "[ServiceId],[ServiceName],[Priority],"
                + "[Operator], [OriginalReqID], [OriginalRequestorID])"
                + " VALUES"
                + " (@ReqDateTime,@RequestorID,@RequestorMachine,@Command,"
                + "@ServiceId,@ServiceName,@Priority,"
                + " @Operator, @OriginalReqID, @OriginalRequestorID ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        // Only necessary fileds.. the rest will be update by JTM
                        cmd.Parameters.AddWithValue("@ReqDateTime", ReqDateTime);
                        cmd.Parameters.AddWithValue("@RequestorID", RequestorID);
                        cmd.Parameters.AddWithValue("@RequestorMachine", RequestorMachine);
                        cmd.Parameters.AddWithValue("@Command", Command);
                        cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@OriginalReqID", OriginalReqID);
                        cmd.Parameters.AddWithValue("@OriginalRequestorID", OriginalRequestorID);

                        //rows number of record got updated
                        ReqID = (int)cmd.ExecuteScalar();
                     
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

            return ReqID;
        }

        // UPDATE Update Record In AgentQueue Using Struct
        public void UpdateRecordInAgentQueueUsingStruct(AgentQueueStruct AgentQRec)
        {
            ReqID = QueueRec.ReqID = AgentQRec.ReqID;
            ReqDateTime = QueueRec.ReqDateTime = AgentQRec.ReqDateTime;
            RequestorID = QueueRec.RequestorID = AgentQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = AgentQRec.RequestorMachine;
            Command = QueueRec.Command = AgentQRec.Command;
            ServiceId = QueueRec.ServiceId = AgentQRec.ServiceId;
            ServiceName = QueueRec.ServiceName = AgentQRec.ServiceName;
            Priority = QueueRec.Priority = AgentQRec.Priority;
            ReqStatusCode = QueueRec.ReqStatusCode = AgentQRec.ReqStatusCode;
            CmdStatusCode = QueueRec.CmdStatusCode = AgentQRec.CmdStatusCode;
            CmdStatusMessage = QueueRec.CmdStatusMessage = AgentQRec.CmdStatusMessage;
            CmdExecStarted = QueueRec.CmdExecStarted = AgentQRec.CmdExecStarted;
            CmdExecFinished = QueueRec.CmdExecFinished = AgentQRec.CmdExecFinished;
            Operator = QueueRec.Operator = AgentQRec.Operator;
            // OriginalReqID = QueueRec.OriginalReqID = AgentQRec.OriginalReqID;
            // OriginalRequestorID = QueueRec.OriginalRequestorID = AgentQRec.OriginalRequestorID;

            this.UpdateRecordInAgentQueueByReqID(ReqID);
        }

        // UPDATE Update Record In AgentQueue By MsgID
        public void UpdateRecordInAgentQueueByReqID(int InReqID)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.AgentQueue SET "
                             + " ReqDateTime = @ReqDateTime, RequestorID = @RequestorID, RequestorMachine = @RequestorMachine,"
                             + "Command = @Command, ServiceId = @ServiceId,ServiceName = @ServiceName, Priority = @Priority,"
                             + "ReqStatusCode= @ReqStatusCode, CmdStatusCode = @CmdStatusCode, "
                             + "CmdStatusMessage = @CmdStatusMessage, "
                             + "CmdExecStarted = @CmdExecStarted, "
                             + "CmdExecFinished = @CmdExecFinished, "
                             + "Operator = @Operator  "
                             // + "OriginalReqID = @OriginalReqID, "
                             // + "OriginalRequestorID = @OriginalRequestorID, "
                             + " WHERE ReqID = @ReqID", conn))
                    {

                        cmd.Parameters.AddWithValue("@ReqID", InReqID);
                        cmd.Parameters.AddWithValue("@ReqDateTime", ReqDateTime);
                        cmd.Parameters.AddWithValue("@RequestorID", RequestorID);
                        cmd.Parameters.AddWithValue("@RequestorMachine", RequestorMachine);

                        cmd.Parameters.AddWithValue("@Command", Command);
                        cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                        cmd.Parameters.AddWithValue("@ServiceName", ServiceName);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                     
                        cmd.Parameters.AddWithValue("@ReqStatusCode", ReqStatusCode);
                        cmd.Parameters.AddWithValue("@CmdStatusCode", CmdStatusCode);
                        cmd.Parameters.AddWithValue("@CmdStatusMessage", CmdStatusMessage);
                        cmd.Parameters.AddWithValue("@CmdExecStarted", CmdExecStarted);
                        cmd.Parameters.AddWithValue("@CmdExecFinished", CmdExecStarted);
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        // cmd.Parameters.AddWithValue("@OriginalReqID", OriginalReqID);
                        // cmd.Parameters.AddWithValue("@OriginalRequestorID", OriginalRequestorID);

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


        // UPDATE Update Record In AgentQueue for MessageSent
        public void UpdateRecordInAgentQueueForMessageSent(int InReqID)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.AgentQueue SET "
                             + " MessageSent = 1 "
                             + " WHERE ReqID = @ReqID", conn))
                    {

                        cmd.Parameters.AddWithValue("@ReqID", InReqID);
                     
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


        // DELETE Record In AgentQueue By ReqID
        public void DeleteRecordInAgentQueueByMsgID(int InReqID)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.AgentQueue "
                            + " WHERE ReqID = @ReqID ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReqID", InReqID);
                                      
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
        // Delete ALL Records in RRDMAgentQueue table (for TESTING  ONLY!!!)
        public int DeleteAllRecordsInAgentQueue()
        {
            int rows = 0;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.AgentQueue ", conn))
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

       
    }
}



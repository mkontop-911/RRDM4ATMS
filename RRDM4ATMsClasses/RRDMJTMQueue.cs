using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    /// <summary> JTMQueueStruct
    /// Declare the structure to contain the record fields
    /// </summary>
    public struct JTMQueueStruct
    {
        public int MsgID;
        public DateTime MsgDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;
        public int Priority;
        public string BatchID;
        public string AtmNo;
        public string BankID;
        public string BranchNo;
        public string ATMIPAddress;
        public string ATMMachineName;
        public bool ATMWindowsAuth;
        public string ATMAccessID;
        public string ATMAccessPassword;
        public string TypeOfJournal;
        public string SourceFileName;
        public string SourceFilePath;
        public string DestnFileName;
        public string DestnFilePath;
        public string DestnFileHASH;
        public int Stage;
        public int ResultCode;
        public string ResultMessage;
        public DateTime ProcessStart;
        public DateTime FileUploadStart;
        public DateTime FileUploadEnd;
        public DateTime FileParseStart;
        public DateTime FileParseEnd;
        public DateTime ProcessEnd;
        public string Operator;
    }

    #region JTM Stage Class
    public static class JTMQueueStage
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

    public static class JTMQueueResult
    {
        //  Result Codes
        //  0 - Success
        //  1 - Error
        public const int Success = 0;
        public const int Failure = 1;
    }

    public static class JTMQueueCommand
    {
        //  Commands
        public const string Cmd_FETCH = "FETCH";
        public const string Cmd_FETCHDEL = "FETCHDEL";
        public const string Cmd_ATMSTATUS = "ATMSTATUS";
    }

    public class RRDMJTMQueue
    {
        public JTMQueueStruct QueueRec = new JTMQueueStruct();

        #region Properies
        public int MsgID;
        public DateTime MsgDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;
        public int Priority;
        public string BatchID;

        public string AtmNo;
        public string BankID;
        public string BranchNo;
        public string ATMIPAddress;
        public string ATMMachineName;

        public bool ATMWindowsAuth;
        public string ATMAccessID;
        public string ATMAccessPassword;

        public string TypeOfJournal;
        public string SourceFileName;
        public string SourceFilePath;
        public string DestnFileName;
        public string DestnFilePath;
        public string DestnFileHASH;
        public int Stage;


        public int ResultCode;
        public string ResultMessage;
        public DateTime ProcessStart;
        public DateTime FileUploadStart;
        public DateTime FileUploadEnd;
        public DateTime FileParseStart;
        public DateTime FileParseEnd;
        public DateTime ProcessEnd;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        #endregion

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

        // READ a single JTMQueue row which is the oldest in the queue and with the highest priority
        public void ReadSingleJTMQueueByPriority()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT TOP 1 * FROM [dbo].[JTMQueue] " +
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
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                            QueueRec.BatchID = BatchID = (string)rdr["BatchID"];
                            QueueRec.AtmNo = AtmNo = (string)rdr["AtmNo"];
                            QueueRec.BankID = BankID = (string)rdr["BankID"];
                            QueueRec.BranchNo = BranchNo = (string)rdr["BranchNo"];
                            QueueRec.ATMIPAddress = ATMIPAddress = (string)rdr["ATMIPAddress"];
                            QueueRec.ATMMachineName = ATMMachineName = (string)rdr["ATMMachineName"];
                            QueueRec.ATMWindowsAuth = ATMWindowsAuth = (bool)rdr["ATMWindowsAuth"];
                            QueueRec.ATMAccessID = ATMAccessID = (string)rdr["ATMAccessID"];
                            QueueRec.ATMAccessPassword = ATMAccessPassword = (string)rdr["ATMAccessPassword"];
                            QueueRec.TypeOfJournal = TypeOfJournal = (string)rdr["TypeOfJournal"];
                            QueueRec.SourceFileName = SourceFileName = (string)rdr["SourceFileName"];
                            QueueRec.SourceFilePath = SourceFilePath = (string)rdr["SourceFilePath"];
                            QueueRec.DestnFileName = DestnFileName = (string)rdr["DestnFileName"];
                            QueueRec.DestnFilePath = DestnFilePath = (string)rdr["DestnFilePath"];
                            QueueRec.DestnFileHASH = DestnFileHASH = (string)rdr["DestnFileHASH"];
                            QueueRec.Stage = Stage = (int)rdr["Stage"];
                            QueueRec.ResultCode = ResultCode = (int)rdr["ResultCode"];
                            QueueRec.ResultMessage = ResultMessage = (string)rdr["ResultMessage"];
                            QueueRec.ProcessStart = ProcessStart = (DateTime)rdr["ProcessStart"];
                            QueueRec.FileUploadStart = FileUploadStart = (DateTime)rdr["FileUploadStart"];
                            QueueRec.FileUploadEnd = FileUploadEnd = (DateTime)rdr["FileUploadEnd"];
                            QueueRec.FileParseStart = FileParseStart = (DateTime)rdr["FileParseStart"];
                            QueueRec.FileParseEnd = FileParseEnd = (DateTime)rdr["FileParseEnd"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReadSingleJTMQueueByPriority().... " + ex.Message;
                }
        }

        // READ JTMQueue by MsgID
        public void ReadJTMQueueByMsgID(int InMsgID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                  + " FROM [dbo].[JTMQueue] "
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
                            QueueRec.Priority = Priority = (int)rdr["Priority"];
                            QueueRec.BatchID = BatchID = (string)rdr["BatchID"];
                            QueueRec.AtmNo = AtmNo = (string)rdr["AtmNo"];
                            QueueRec.BankID = BankID = (string)rdr["BankID"];
                            QueueRec.BranchNo = BranchNo = (string)rdr["BranchNo"];
                            QueueRec.ATMIPAddress = ATMIPAddress = (string)rdr["ATMIPAddress"];
                            QueueRec.ATMMachineName = ATMMachineName = (string)rdr["ATMMachineName"];
                            QueueRec.ATMWindowsAuth = ATMWindowsAuth = (bool)rdr["ATMWindowsAuth"];
                            QueueRec.ATMAccessID = ATMAccessID = (string)rdr["ATMAccessID"];
                            QueueRec.ATMAccessPassword = ATMAccessPassword = (string)rdr["ATMAccessPassword"];
                            QueueRec.TypeOfJournal = TypeOfJournal = (string)rdr["TypeOfJournal"];
                            QueueRec.SourceFileName = SourceFileName = (string)rdr["SourceFileName"];
                            QueueRec.SourceFilePath = SourceFilePath = (string)rdr["SourceFilePath"];
                            QueueRec.DestnFileName = DestnFileName = (string)rdr["DestnFileName"];
                            QueueRec.DestnFilePath = DestnFilePath = (string)rdr["DestnFilePath"];
                            QueueRec.DestnFileHASH = DestnFileHASH = (string)rdr["DestnFileHASH"];
                            QueueRec.Stage = Stage = (int)rdr["Stage"];
                            QueueRec.ResultCode = ResultCode = (int)rdr["ResultCode"];
                            QueueRec.ResultMessage = ResultMessage = (string)rdr["ResultMessage"];
                            QueueRec.ProcessStart = ProcessStart = (DateTime)rdr["ProcessStart"];
                            QueueRec.FileUploadStart = FileUploadStart = (DateTime)rdr["FileUploadStart"];
                            QueueRec.FileUploadEnd = FileUploadEnd = (DateTime)rdr["FileUploadEnd"];
                            QueueRec.FileParseStart = FileParseStart = (DateTime)rdr["FileParseStart"];
                            QueueRec.FileParseEnd = FileParseEnd = (DateTime)rdr["FileParseEnd"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured ReadJTMQueueByMsgID(int InMsgID)......... " + ex.Message;
                }
        }

        // Insert  Record In JTMQueue Using Struct
        public void InsertRecordInJTMQueueUsingStruct(JTMQueueStruct JTMQRec)
        {
            MsgID = QueueRec.MsgID = JTMQRec.MsgID;
            MsgDateTime = QueueRec.MsgDateTime = JTMQRec.MsgDateTime;
            RequestorID = QueueRec.RequestorID = JTMQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = JTMQRec.RequestorMachine;
            Command = QueueRec.Command = JTMQRec.Command;
            Priority = QueueRec.Priority = JTMQRec.Priority;
            BatchID = QueueRec.BatchID = JTMQRec.BatchID;
            AtmNo = QueueRec.AtmNo = JTMQRec.AtmNo;
            BankID = QueueRec.BankID = JTMQRec.BankID;
            BranchNo = QueueRec.BranchNo = JTMQRec.BranchNo;
            ATMIPAddress = QueueRec.ATMIPAddress = JTMQRec.ATMIPAddress;
            ATMMachineName = QueueRec.ATMMachineName = JTMQRec.ATMMachineName;
            ATMWindowsAuth = QueueRec.ATMWindowsAuth = JTMQRec.ATMWindowsAuth;
            ATMAccessID = QueueRec.ATMAccessID = JTMQRec.ATMAccessID;
            ATMAccessPassword = QueueRec.ATMAccessPassword = JTMQRec.ATMAccessPassword;
            TypeOfJournal = QueueRec.TypeOfJournal = JTMQRec.TypeOfJournal;
            SourceFileName = QueueRec.SourceFileName = JTMQRec.SourceFileName;
            SourceFilePath = QueueRec.SourceFilePath = JTMQRec.SourceFilePath;
            DestnFileName = QueueRec.DestnFileName = JTMQRec.DestnFileName;
            DestnFilePath = QueueRec.DestnFilePath = JTMQRec.DestnFilePath;
            DestnFileHASH = QueueRec.DestnFileHASH = JTMQRec.DestnFileHASH;
            Stage = QueueRec.Stage = JTMQRec.Stage;
            ResultCode = QueueRec.ResultCode = JTMQRec.ResultCode;
            ResultMessage = QueueRec.ResultMessage = JTMQRec.ResultMessage;
            ProcessStart = QueueRec.ProcessStart = JTMQRec.ProcessStart;
            FileUploadStart = QueueRec.FileUploadStart = JTMQRec.FileUploadStart;
            FileUploadEnd = QueueRec.FileUploadEnd = JTMQRec.FileUploadEnd;
            FileParseStart = QueueRec.FileParseStart = JTMQRec.FileParseStart;
            FileParseEnd = QueueRec.FileParseEnd = JTMQRec.FileParseEnd;
            ProcessEnd = QueueRec.ProcessEnd = JTMQRec.ProcessEnd;
            Operator = QueueRec.Operator = JTMQRec.Operator;

            this.InsertNewRecordInJTMQueue();
        }

        // Insert NEW Record in JTMQueue
        public int InsertNewRecordInJTMQueue()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[JTMQueue]"
                + " ([MsgDateTime],[RequestorID],[RequestorMachine],[Command],[Priority],"
                + " [BatchID],"
                + "[AtmNo], [BankID], [BranchNo], [ATMIPAddress],"
                + "[ATMMachineName],[ATMWindowsAuth],[ATMAccessID],[ATMAccessPassword], "
                + "[TypeOfJournal],[SourceFileName],[SourceFilePath],[DestnFilePath], "
                + "[Operator] )"
                + " VALUES"
                + " (@MsgDateTime,@RequestorID,@RequestorMachine,@Command,@Priority,"
                + "@BatchID,"
                + "@AtmNo ,@BankID, @BranchNo, @ATMIPAddress,"
                + "@ATMMachineName,@ATMWindowsAuth,@ATMAccessID,@ATMAccessPassword, "
                + "@TypeOfJournal,@SourceFileName,@SourceFilePath,@DestnFilePath, "
                + " @Operator )"
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
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                        cmd.Parameters.AddWithValue("@BatchID", BatchID);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankID", BankID);
                        cmd.Parameters.AddWithValue("@BranchNo", BranchNo);
                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);
                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);

                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);
                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);
                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        // MsgID = (int)cmd.ExecuteScalar();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            ErrorFound = false;
                            ErrorOutput = "";
                        }
                        else
                        {
                            ErrorFound = true;
                            ErrorOutput = "An error occured while INSERTING in [RRDMJTMQueue]... ";
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MsgID = 0;
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in InsertNewRecordInJTMQueue()............. " + ex.Message;
                }
            return (MsgID);
        }

        // UPDATE Update Record In JTMQueue Using Struct
        public void UpdateRecordInJTMQueueUsingStruct(JTMQueueStruct JTMQRec)
        {
            MsgID = QueueRec.MsgID = JTMQRec.MsgID;
            MsgDateTime = QueueRec.MsgDateTime = JTMQRec.MsgDateTime;
            RequestorID = QueueRec.RequestorID = JTMQRec.RequestorID;
            RequestorMachine = QueueRec.RequestorMachine = JTMQRec.RequestorMachine;
            Command = QueueRec.Command = JTMQRec.Command;
            Priority = QueueRec.Priority = JTMQRec.Priority;
            BatchID = QueueRec.BatchID = JTMQRec.BatchID;
            AtmNo = QueueRec.AtmNo = JTMQRec.AtmNo;
            BankID = QueueRec.BankID = JTMQRec.BankID;
            BranchNo = QueueRec.BranchNo = JTMQRec.BranchNo;
            ATMIPAddress = QueueRec.ATMIPAddress = JTMQRec.ATMIPAddress;
            ATMMachineName = QueueRec.ATMMachineName = JTMQRec.ATMMachineName;
            ATMWindowsAuth = QueueRec.ATMWindowsAuth = JTMQRec.ATMWindowsAuth;
            ATMAccessID = QueueRec.ATMAccessID = JTMQRec.ATMAccessID;
            ATMAccessPassword = QueueRec.ATMAccessPassword = JTMQRec.ATMAccessPassword;
            TypeOfJournal = QueueRec.TypeOfJournal = JTMQRec.TypeOfJournal;
            SourceFileName = QueueRec.SourceFileName = JTMQRec.SourceFileName;
            SourceFilePath = QueueRec.SourceFilePath = JTMQRec.SourceFilePath;
            DestnFileName = QueueRec.DestnFileName = JTMQRec.DestnFileName;
            DestnFilePath = QueueRec.DestnFilePath = JTMQRec.DestnFilePath;
            DestnFileHASH = QueueRec.DestnFileHASH = JTMQRec.DestnFileHASH;
            Stage = QueueRec.Stage = JTMQRec.Stage;
            ResultCode = QueueRec.ResultCode = JTMQRec.ResultCode;
            ResultMessage = QueueRec.ResultMessage = JTMQRec.ResultMessage;
            ProcessStart = QueueRec.ProcessStart = JTMQRec.ProcessStart;
            FileUploadStart = QueueRec.FileUploadStart = JTMQRec.FileUploadStart;
            FileUploadEnd = QueueRec.FileUploadEnd = JTMQRec.FileUploadEnd;
            FileParseStart = QueueRec.FileParseStart = JTMQRec.FileParseStart;
            FileParseEnd = QueueRec.FileParseEnd = JTMQRec.FileParseEnd;
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
                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.JTMQueue SET "
                             + " MsgDateTime = @MsgDateTime, RequestorID = @RequestorID, RequestorMachine = @RequestorMachine,"
                             + "Command = @Command, Priority = @Priority,"
                             + "BatchID = @BatchID,"
                             + "AtmNo = @AtmNo, BankID = @BankID, BranchNo = @BranchNo, ATMIPAddress = @ATMIPAddress,"
                             + "ATMMachineName = @ATMMachineName, ATMWindowsAuth = @ATMWindowsAuth, "
                             + "ATMAccessID = @ATMAccessID, ATMAccessPassword = @ATMAccessPassword, "
                             + "TypeOfJournal = @TypeOfJournal, SourceFileName = @SourceFileName, SourceFilePath = @SourceFilePath,"
                             + "DestnFileName = @DestnFileName, DestnFilePath = @DestnFilePath, DestnFileHASH = @DestnFileHASH,"
                             + "Stage = @Stage, ResultCode = @ResultCode, "
                             + "ResultMessage = @ResultMessage, "
                             + "ProcessStart = @ProcessStart, FileUploadStart = @FileUploadStart, FileUploadEnd = @FileUploadEnd, "
                             + " FileParseStart = @FileParseStart, FileParseEnd = @FileParseEnd, ProcessEnd = @ProcessEnd, Operator = @Operator  "
                             + " WHERE MsgID = @MsgID", conn))
                    {

                        cmd.Parameters.AddWithValue("@MsgID", InMsgID);
                        cmd.Parameters.AddWithValue("@MsgDateTime", MsgDateTime);
                        cmd.Parameters.AddWithValue("@RequestorID", RequestorID);
                        cmd.Parameters.AddWithValue("@RequestorMachine", RequestorMachine);

                        cmd.Parameters.AddWithValue("@Command", Command);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                        cmd.Parameters.AddWithValue("@BatchID", BatchID);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankID", BankID);
                        cmd.Parameters.AddWithValue("@BranchNo", BranchNo);
                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);
                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);

                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);
                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);
                        cmd.Parameters.AddWithValue("@DestnFileName", DestnFileName);
                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);
                        cmd.Parameters.AddWithValue("@DestnFileHASH", DestnFileHASH);

                        cmd.Parameters.AddWithValue("@Stage", Stage);
                        cmd.Parameters.AddWithValue("@ResultCode", ResultCode);
                        cmd.Parameters.AddWithValue("@ResultMessage", ResultMessage);
                        cmd.Parameters.AddWithValue("@ProcessStart", ProcessStart);
                        cmd.Parameters.AddWithValue("@FileUploadStart", FileUploadStart);
                        cmd.Parameters.AddWithValue("@FileUploadEnd", FileUploadEnd);
                        cmd.Parameters.AddWithValue("@FileParseStart", FileParseStart);
                        cmd.Parameters.AddWithValue("@FileParseEnd", FileParseEnd);
                        cmd.Parameters.AddWithValue("@ProcessEnd", ProcessEnd);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in UpdateRecordInJTMQueueUsingByMsgID()..... " + ex.Message;

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
                        new SqlCommand("DELETE FROM dbo.JTMQueue "
                            + " WHERE MsgID = @MsgID ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MsgID", InMsgID);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DeleteRecordInJTMQueueByMsgID(int InMsgID............. " + ex.Message;
                }
            }
        }

        /// <summary> InvokeStoredProcedure 
        /// 
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="BnkID"></param>
        /// <param name="Atm"></param>
        /// <param name="Branch"></param>
        /// <param name="FullName"></param>
        /// <returns></returns>
        public int InvokeStoredProcedure(string SPName, string BnkID, string Atm, string Branch, string FullName)
        {
            int rc = -1;
            ErrorFound = false;
            ErrorOutput = "";
            int ReturnCode = -1;
            string connectionString = ConfigurationManager.ConnectionStrings["ATMSJournalsConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@BankID", BnkID));
                    cmd.Parameters.Add(new SqlParameter("@AtmNo", Atm));
                    cmd.Parameters.Add(new SqlParameter("@BranchNo", Branch));
                    cmd.Parameters.Add(new SqlParameter("@FullPath", FullName));

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    // execute the command
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH
                    rc = (int)cmd.Parameters["@ReturnCode"].Value;

                }
                catch (Exception ex)
                {
                    rc = -1;
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in RRDMJTMQueue:InvokeStoredProcedure! The error message reads:\n" + ex.Message;
                }
            }
            return (rc);
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
                        new SqlCommand("DELETE FROM dbo.JTMQueue ", conn))
                    {
                        rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DeleteAllRecordsInJTMQueue()... " + ex.Message;
                }
            return (rows);
        }

    }

}

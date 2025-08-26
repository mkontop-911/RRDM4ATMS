using System;
using System.Text;
//
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class ReconcInitialTable
    {
        public int SeqNo;
        public string OriginFileName;
        public int OriginalRecordId;
        public string MatchingCateg;
        public string Origin;
        public string TransTypeAtOrigin;
        public string TerminalId;
        public int TransType;
        public string TransDescr;
        public string CardNumber;
        public string AccNo;
        public string TransCurr;
        public decimal TransAmt;
        public decimal AmtFileBToFileC;
        public DateTime TransDate;
        public int TraceNo;
        public int RRNumber;
        public string ResponseCode;
        public string T24RefNumber;
        public bool Processed;
        public int ProcessedAtRMCycle;
        public string Operator;

        public bool ErrorFound;
        public string ErrorOutput;

        // Uses ReconConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;


        //#region Create UNIV InMem DataTable
        //public static DataTable CreateUNIV_MemTable()
        //{
        //    DataTable InMemDataTbl = new DataTable();
        //    try
        //    {
        //        InMemDataTbl.Columns.Add("SeqNo", typeof(int));
        //        InMemDataTbl.Columns.Add("OriginFileName", typeof(string));
        //        InMemDataTbl.Columns.Add("OriginalRecordId", typeof(int));
        //        InMemDataTbl.Columns.Add("MatchingCateg", typeof(string));
        //        InMemDataTbl.Columns.Add("Origin", typeof(string));
        //        InMemDataTbl.Columns.Add("TransTypeAtOrigin", typeof(string));
        //        InMemDataTbl.Columns.Add("TerminalId", typeof(string));
        //        InMemDataTbl.Columns.Add("TransType", typeof(int));
        //        InMemDataTbl.Columns.Add("TransDescr", typeof(string));
        //        InMemDataTbl.Columns.Add("CardNumber", typeof(string));
        //        InMemDataTbl.Columns.Add("AccNo", typeof(string));
        //        InMemDataTbl.Columns.Add("TransCurr", typeof(string));
        //        InMemDataTbl.Columns.Add("TransAmt", typeof(decimal));
        //        InMemDataTbl.Columns.Add("AmtFileBToFileC", typeof(decimal));
        //        InMemDataTbl.Columns.Add("TransDate", typeof(DateTime));
        //        InMemDataTbl.Columns.Add("TraceNo", typeof(int));
        //        InMemDataTbl.Columns.Add("RRNumber", typeof(int));
        //        InMemDataTbl.Columns.Add("ResponseCode", typeof(string));
        //        InMemDataTbl.Columns.Add("T24RefNumber", typeof(string));
        //        InMemDataTbl.Columns.Add("Processed", typeof(bool));
        //        InMemDataTbl.Columns.Add("ProcessedAtRMCycle", typeof(int));
        //        InMemDataTbl.Columns.Add("Operator", typeof(string));
        //    }
        //    catch (Exception ex)
        //    {
        //        //string msg = string.Format("Exception encountered while creating the In-Memory table! The message reads: {0}", ex.Message);
        //        //RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
        //        CatchDetails(ex);
        //        return (null);
        //    }
        //    return InMemDataTbl;
        //}
        //#endregion

        //// Used only to initialize db for testing
        //public void TruncateTable(string TableName)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string cmdTruncate = "TRUNCATE TABLE [dbo].[" + TableName + "] ";
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(cmdTruncate, conn))
        //            {
        //                cmd.ExecuteNonQuery();
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = string.Format("An error occured while TRUNCATING table {0}\nThe error message is:\n{1}", TableName, ex.Message);
        //            CatchDetails(ex);
        //        }
        //}

        //public void InsertSingle(string TWInitialTableName)
        //{
        //    ErrorFound = false;
        //    ErrorOutput = "";
        //    string cmdInsert = "INSERT INTO [dbo].[" + TWInitialTableName + "] " +
        //                                "( " +
        //                                "[OriginFileName], " +
        //                                "[OriginalRecordId], " +
        //                                "[MatchingCateg], " +
        //                                "[Origin], " +
        //                                "[TransTypeAtOrigin], " +
        //                                "[TerminalId], " +
        //                                "[TransType], " +
        //                                "[TransDescr], " +
        //                                "[CardNumber], " +
        //                                "[AccNo], " +
        //                                "[TransCurr], " +
        //                                "[TransAmt], " +
        //                                "[AmtFileBToFileC], " +
        //                                "[TransDate], " +
        //                                "[TraceNo], " +
        //                                "[RRNumber], " +
        //                                "[ResponseCode], " +
        //                                "[T24RefNumber], " +
        //                                "[Processed], " +
        //                                "[ProcessedAtRMCycle], " +
        //                                "[Operator]" +
        //                                ") " +
        //                            "VALUES " +
        //                                "( " +
        //                                "@OriginFileName, " +
        //                                "@OriginalRecordId, " +
        //                                "@MatchingCateg, " +
        //                                "@Origin, " +
        //                                "@TransTypeAtOrigin, " +
        //                                "@TerminalId, " +
        //                                "@TransType, " +
        //                                "@TransDescr, " +
        //                                "@CardNumber, " +
        //                                "@AccNo, " +
        //                                "@TransCurr, " +
        //                                "@TransAmt, " +
        //                                "@AmtFileBToFileC, " +
        //                                "@TransDate, " +
        //                                "@TraceNo, " +
        //                                "@RRNumber, " +
        //                                "@ResponseCode, " +
        //                                "@T24RefNumber, " +
        //                                "@Processed, " +
        //                                "@ProcessedAtRMCycle, " +
        //                                "@Operator " +
        //                                ") ";


        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
        //                cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
        //                cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
        //                cmd.Parameters.AddWithValue("@Origin", Origin);
        //                cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
        //                cmd.Parameters.AddWithValue("@TerminalId", TerminalId);
        //                cmd.Parameters.AddWithValue("@TransType", TransType);
        //                cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
        //                cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
        //                cmd.Parameters.AddWithValue("@AccNo", AccNo);
        //                cmd.Parameters.AddWithValue("@TransCurr", TransCurr);
        //                cmd.Parameters.AddWithValue("@TransAmt", TransAmt);
        //                cmd.Parameters.AddWithValue("@AmtFileBToFileC", AmtFileBToFileC);
        //                cmd.Parameters.AddWithValue("@TransDate", TransDate);
        //                cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
        //                cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
        //                cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);
        //                cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);
        //                cmd.Parameters.AddWithValue("@Processed", Processed);
        //                cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", ProcessedAtRMCycle);
        //                cmd.Parameters.AddWithValue("@Operator", Operator);

        //                int rows = cmd.ExecuteNonQuery();
        //                if (rows > 0)
        //                {
        //                    ErrorFound = false;
        //                    ErrorOutput = "";
        //                }
        //                else
        //                {
        //                    ErrorFound = true;
        //                    ErrorOutput = string.Format("An error occured while INSERTING a new row in {0}", TWInitialTableName);
        //                }
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = string.Format("An error occured while INSERTING in {0}\nThe error message is:\n{1}", TWInitialTableName, ex.Message);
        //            CatchDetails(ex);
        //        }
        //}

        //public void BulkInsertFromDataTable(DataTable DataTbl, string TWInitialTableName)
        //{
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
        //            {
        //                //SqlBulkCopyColumnMapping SeqNo = new SqlBulkCopyColumnMapping("SeqNo", "SeqNo");
        //                //bulkCopy.ColumnMappings.Add(SeqNo);
        //                //SqlBulkCopyColumnMapping Origin = new SqlBulkCopyColumnMapping("Origin", "Origin");
        //                //bulkCopy.ColumnMappings.Add(Origin);
        //                //SqlBulkCopyColumnMapping OriginFileName = new SqlBulkCopyColumnMapping("OriginFileName", "OriginFileName");
        //                //bulkCopy.ColumnMappings.Add(OriginFileName);
        //                //SqlBulkCopyColumnMapping OriginalRecordId = new SqlBulkCopyColumnMapping("OriginalRecordId", "OriginalRecordId");
        //                //bulkCopy.ColumnMappings.Add(OriginalRecordId);
        //                //SqlBulkCopyColumnMapping MatchingCateg = new SqlBulkCopyColumnMapping("MatchingCateg", "MatchingCateg");
        //                //bulkCopy.ColumnMappings.Add(MatchingCateg);
        //                //SqlBulkCopyColumnMapping TransTypeAtOrigin = new SqlBulkCopyColumnMapping("TransTypeAtOrigin", "TransTypeAtOrigin");
        //                //bulkCopy.ColumnMappings.Add(TransTypeAtOrigin);
        //                //SqlBulkCopyColumnMapping TerminalId = new SqlBulkCopyColumnMapping("TerminalId", "TerminalId");
        //                //bulkCopy.ColumnMappings.Add(TerminalId);
        //                //SqlBulkCopyColumnMapping TransType = new SqlBulkCopyColumnMapping("TransType", "TransType");
        //                //bulkCopy.ColumnMappings.Add(TransType);
        //                //SqlBulkCopyColumnMapping TransDescr = new SqlBulkCopyColumnMapping("TransDescr", "TransDescr");
        //                //bulkCopy.ColumnMappings.Add(TransDescr);
        //                //SqlBulkCopyColumnMapping CardNumber = new SqlBulkCopyColumnMapping("CardNumber", "CardNumber");
        //                //bulkCopy.ColumnMappings.Add(CardNumber);
        //                //SqlBulkCopyColumnMapping AccNo = new SqlBulkCopyColumnMapping("AccNo", "AccNo");
        //                //bulkCopy.ColumnMappings.Add(AccNo);
        //                //SqlBulkCopyColumnMapping TransCurr = new SqlBulkCopyColumnMapping("TransCurr", "TransCurr");
        //                //bulkCopy.ColumnMappings.Add(TransCurr);
        //                //SqlBulkCopyColumnMapping TransAmt = new SqlBulkCopyColumnMapping("TransAmt", "TransAmt");
        //                //bulkCopy.ColumnMappings.Add(TransAmt);
        //                //SqlBulkCopyColumnMapping AmtFileBToFileC = new SqlBulkCopyColumnMapping("AmtFileBToFileC", "AmtFileBToFileC");
        //                //bulkCopy.ColumnMappings.Add(AmtFileBToFileC);
        //                //SqlBulkCopyColumnMapping TransDate = new SqlBulkCopyColumnMapping("TransDate", "TransDate");
        //                //bulkCopy.ColumnMappings.Add(TransDate);
        //                //SqlBulkCopyColumnMapping TraceNo = new SqlBulkCopyColumnMapping("TraceNo", "TraceNo");
        //                //bulkCopy.ColumnMappings.Add(TraceNo);
        //                //SqlBulkCopyColumnMapping RRNumber = new SqlBulkCopyColumnMapping("RRNumber", "RRNumber");
        //                //bulkCopy.ColumnMappings.Add(RRNumber);
        //                //SqlBulkCopyColumnMapping ResponseCode = new SqlBulkCopyColumnMapping("", "ResponseCode");
        //                //bulkCopy.ColumnMappings.Add(ResponseCode);
        //                //SqlBulkCopyColumnMapping T24RefNumber = new SqlBulkCopyColumnMapping("", "T24RefNumber");
        //                //bulkCopy.ColumnMappings.Add(T24RefNumber);
        //                //SqlBulkCopyColumnMapping Processed = new SqlBulkCopyColumnMapping("Processed", "Processed");
        //                //bulkCopy.ColumnMappings.Add(Processed);
        //                //SqlBulkCopyColumnMapping ProcessedAtRMCycle = new SqlBulkCopyColumnMapping("ProcessedAtRMCycle", "ProcessedAtRMCycle");
        //                //bulkCopy.ColumnMappings.Add(ProcessedAtRMCycle);
        //                //SqlBulkCopyColumnMapping Operator = new SqlBulkCopyColumnMapping("Operator", "Operator");
        //                //bulkCopy.ColumnMappings.Add(Operator);

        //                try
        //                {
        //                    bulkCopy.DestinationTableName = TWInitialTableName;
        //                    bulkCopy.WriteToServer(DataTbl);
        //                    ErrorFound = false;
        //                }
        //                catch (Exception ex)
        //                {
        //                    ErrorFound = true;
        //                    ErrorOutput = ex.Message;
        //                }
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            // CatchDetails(ex);
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = ex.Message;
        //        }
        //    }
        //}

        //public void DeleteRecordsByOriginFile(string FUID, string tblName)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("DELETE FROM [dbo].[" + tblName + "] WHERE OriginFileName =  @Org ", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Org", FUID);
        //                cmd.ExecuteNonQuery();
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }

        //}


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

#if DEBUG
            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");
#endif
        }

    }
}

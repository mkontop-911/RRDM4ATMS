using System;
using System.Text;
//
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class ReconcCore_Banking_T24_RAW : Logger
    {
        public ReconcCore_Banking_T24_RAW() : base() { }

        public const string TableName = "Core_Banking_T24_Txns_RAW";

        public int SeqNo;
        public string Origin;
        public string OriginFileName;
        public int OriginalRecordId;
        public DateTime TransDate;
        public int TransTraceNumber;
        public string TerminalIdentification;
        public string RECTYPE;
        public string RECSEQNUM;
        public string TERMINALID;
        public string TRANSDATETIME;
        public string TRANSCURR;
        public string TRANSAMT;
        public string TRANSCODE;
        public string TRANSTYPE;
        public string CARDNUM;
        public string ACCOUNTNUM;
        public string TRANSDESC;
      //  public string T24REFNUM;
        public string TRACENUM;
        public string AUTHCODE;
        public string RESPCODE;
        public string Operator;


        public bool ErrorFound;
        public string ErrorOutput;

        // Uses ReconConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;


        // Used only to initialize db for testing
        public void TruncateTable()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdTruncate = "TRUNCATE TABLE [dbo].[" + TableName + "] ";
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
                    ErrorOutput = string.Format("An error occured while TRUNCATING table {0}\nThe error message is:\n{1}", TableName, ex.Message);
                    CatchDetails(ex);
                }
        }

        #region Create Memory Table for RAW
        public static DataTable CreateMemoryTable()
        {
            string memTableName = TableName + "_MemTable";
            DataTable InMemDataTbl = new DataTable(memTableName);
            InMemDataTbl.Columns.Add("SeqNo", typeof(int));
            InMemDataTbl.Columns.Add("Origin", typeof(string));
            InMemDataTbl.Columns.Add("OriginFileName", typeof(string));
            InMemDataTbl.Columns.Add("OriginalRecordId", typeof(string));
            InMemDataTbl.Columns.Add("TransDate", typeof(DateTime));
            InMemDataTbl.Columns.Add("TransTraceNumber", typeof(int));
            // TerminalIdentification = TERMINALID
            InMemDataTbl.Columns.Add("TerminalIdentification", typeof(string));
            // The following are extracted "AS-IS" from the source file
            InMemDataTbl.Columns.Add("RECTYPE", typeof(string));
            InMemDataTbl.Columns.Add("RECSEQNUM", typeof(string));
            InMemDataTbl.Columns.Add("TERMINALID", typeof(string));
            InMemDataTbl.Columns.Add("TRANSDATETIME", typeof(string));
            InMemDataTbl.Columns.Add("TRANSCURR", typeof(string));
            InMemDataTbl.Columns.Add("TRANSAMT", typeof(string));
            InMemDataTbl.Columns.Add("TRANSCODE", typeof(string));
            InMemDataTbl.Columns.Add("TRANSTYPE", typeof(string));
            InMemDataTbl.Columns.Add("CARDNUM", typeof(string));
            InMemDataTbl.Columns.Add("ACCOUNTNUM", typeof(string));
            InMemDataTbl.Columns.Add("TRANSDESC", typeof(string));
        //    InMemDataTbl.Columns.Add("T24REFNUM", typeof(string));
            InMemDataTbl.Columns.Add("TRACENUM", typeof(string));
            InMemDataTbl.Columns.Add("AUTHCODE", typeof(string));
            InMemDataTbl.Columns.Add("RESPCODE", typeof(string));
            InMemDataTbl.Columns.Add("Operator", typeof(string));
            return InMemDataTbl;
        }
        #endregion

        public void InsertSingle(string InTableName)
        {
            ErrorFound = false;
            ErrorOutput = "";
            string cmdInsert = "INSERT INTO [dbo].[" + TableName + "] " +
                                        "( " +
                                        "[Origin], " +
                                        "[OriginFileName], " +
                                        "[OriginalRecordId], " +
                                        "[TransDate], " +
                                        "[TransTraceNumber], " +
                                        "[TerminalIdentification], " +
                                        "[RECTYPE], " +
                                        "[RECSEQNUM], " +
                                        "[TERMINALID], " +
                                        "[TRANSDATETIME], " +
                                        "[TRANSCURR], " +
                                        "[TRANSAMT], " +
                                        "[TRANSCODE], " +
                                        "[TRANSTYPE], " +
                                        "[CARDNUM], " +
                                        "[ACCOUNTNUM], " +
                                        "[TRANSDESC], " +
                                      //  "[T24REFNUM], " +
                                        "[TRACENUM], " +
                                        "[AUTHCODE], " +
                                        "[RESPCODE], " +
                                        "[Operator]" +
                                        ") " +
                                    "VALUES " +
                                        "( " +
                                        "@Origin, " +
                                        "@OriginFileName, " +
                                        "@OriginalRecordId, " +
                                        "@TransDate, " +
                                        "@TransTraceNumber, " +
                                        "@TerminalIdentification, " +
                                        "@RECTYPE, " +
                                        "@RECSEQNUM, " +
                                        "@TERMINALID, " +
                                        "@TRANSDATETIME, " +
                                        "@TRANSCURR, " +
                                        "@TRANSAMT, " +
                                        "@TRANSCODE, " +
                                        "@TRANSTYPE, " +
                                        "@CARDNUM, " +
                                        "@ACCOUNTNUM, " +
                                        "@TRANSDESC, " +
                                    //    "@T24REFNUM, " +
                                        "@TRACENUM, " +
                                        "@AUTHCODE, " +
                                        "@RESPCODE, " +
                                        "@Operator, " +
                                        ") ";


            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@TransTraceNumber", TransTraceNumber);
                        cmd.Parameters.AddWithValue("@TerminalIdentification", TerminalIdentification);
                        cmd.Parameters.AddWithValue("RECTYPE", RECTYPE);
                        cmd.Parameters.AddWithValue("RECSEQNUM", RECSEQNUM);
                        cmd.Parameters.AddWithValue("TERMINALID", TERMINALID);
                        cmd.Parameters.AddWithValue("TRANSDATETIME", TRANSDATETIME);
                        cmd.Parameters.AddWithValue("TRANSCURR", TRANSCURR);
                        cmd.Parameters.AddWithValue("TRANSAMT", TRANSAMT);
                        cmd.Parameters.AddWithValue("TRANSCODE", TRANSCODE);
                        cmd.Parameters.AddWithValue("TRANSTYPE", TRANSTYPE);
                        cmd.Parameters.AddWithValue("CARDNUM", CARDNUM);
                        cmd.Parameters.AddWithValue("ACCOUNTNUM", ACCOUNTNUM);
                        cmd.Parameters.AddWithValue("TRANSDESC", TRANSDESC);
                     //   cmd.Parameters.AddWithValue("T24REFNUM", T24REFNUM);
                        cmd.Parameters.AddWithValue("TRACENUM", TRACENUM);
                        cmd.Parameters.AddWithValue("AUTHCODE", AUTHCODE);
                        cmd.Parameters.AddWithValue("RESPCODE", RESPCODE);
                        cmd.Parameters.AddWithValue("Operator", RESPCODE);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            ErrorFound = false;
                            ErrorOutput = "";
                        }
                        else
                        {
                            ErrorFound = true;
                            ErrorOutput = string.Format("An error occured while INSERTING a new row in {0}", TableName);
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    ErrorFound = true;
                    ErrorOutput = string.Format("An error occured while INSERTING in {0}\nThe error message is:\r\n{1}", TableName, msg);
                    CatchDetails(ex);
                }
        }

        public void BulkInsertFromDataTable(DataTable DataTbl)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        try
                        {
                            bulkCopy.DestinationTableName = TableName;
                            bulkCopy.WriteToServer(DataTbl);
                            ErrorFound = false;
                        }
                        catch (Exception ex)
                        {
                            ErrorFound = true;
                            ErrorOutput = ex.Message;
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    ErrorFound = true;
                    ErrorOutput = msg;
                    CatchDetails(ex);
                }
            }
        }

        public void DeleteRecordsByOriginFile(string FUID)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[" + TableName + "] WHERE OriginFileName =  @Org ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Org", FUID);
                        cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    ErrorFound = true;
                    ErrorOutput = msg;
                    CatchDetails(ex);
                }
        }

    

    }
}

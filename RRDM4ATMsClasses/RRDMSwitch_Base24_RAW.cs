using System;
using System.Text;
//
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class ReconcSwitch_Base24_RAW : Logger
    {
        public ReconcSwitch_Base24_RAW() : base() { }

        public const string TableName = "Switch_Base24_Txns_RAW";

        public int SeqNo;
        public string Origin;
        public string OriginFileName;
        public int OriginalRecordId;
        public DateTime TransDate;
        public int TransTraceNumber;
        public string TerminalIdentification;
        public string W_DATE_EPEX;
        public string W_REC;
        public string W_TRAN_DATE;
        public string W_TRAN_TIME;
        public string W_PAN;
        public string W_TERM_ID;
        public string W_FROM_ACCT;
        public string W_TO_ACCT;
        public string W_POSTDAT;
        public string W_T_CDE;
        public string W_AMT_1;
        public string W_CODE;
        public string W_SEQ_NUM;
        public string W_AUTH_ID;
        public string FILLER;
        public string W_COUNTRY;
        public string W_CURRENCY;
        public string W_TERMINAL_CAPABILITY;
        public string W_CARD_TYPE;
        public string W_DATE_EPEX2;

        public bool ErrorFound;
        public string ErrorOutput;

        // Uses ReconConnection String
        string connectionString = AppConfig.GetConnectionString("ReconConnectionString");


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
            // TransDate = transform(W_TRAN_DATE)
            InMemDataTbl.Columns.Add("TransDate", typeof(DateTime));
            // TransTraceNumber = transform(W_SEQ_NUM)
            InMemDataTbl.Columns.Add("TransTraceNumber", typeof(int));
            // TerminalIdentification = W_TERM_ID
            InMemDataTbl.Columns.Add("TerminalIdentification", typeof(string));
            // The following are extracted "AS-IS" from the source file
            InMemDataTbl.Columns.Add("W_DATE_EPEX", typeof(string));
            InMemDataTbl.Columns.Add("W_REC", typeof(string));
            InMemDataTbl.Columns.Add("W_TRAN_DATE", typeof(string));
            InMemDataTbl.Columns.Add("W_TRAN_TIME", typeof(string));
            InMemDataTbl.Columns.Add("W_PAN", typeof(string));
            InMemDataTbl.Columns.Add("W_TERM_ID", typeof(string));
            InMemDataTbl.Columns.Add("W_FROM_ACCT", typeof(string));
            InMemDataTbl.Columns.Add("W_TO_ACCT", typeof(string));
            InMemDataTbl.Columns.Add("W_POSTDAT", typeof(string));
            InMemDataTbl.Columns.Add("W_T_CDE", typeof(string));
            InMemDataTbl.Columns.Add("W_AMT_1", typeof(string));
            InMemDataTbl.Columns.Add("W_CODE", typeof(string));
            InMemDataTbl.Columns.Add("W_SEQ_NUM", typeof(string));
            InMemDataTbl.Columns.Add("W_AUTH_ID", typeof(string));
            InMemDataTbl.Columns.Add("FILLER", typeof(string));
            InMemDataTbl.Columns.Add("W_COUNTRY", typeof(string));
            InMemDataTbl.Columns.Add("W_CURRENCY", typeof(string));
            InMemDataTbl.Columns.Add("W_TERMINAL_CAPABILITY", typeof(string));
            InMemDataTbl.Columns.Add("W_CARD_TYPE", typeof(string));
            InMemDataTbl.Columns.Add("W_DATE_EPEX2", typeof(string));
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
                                        "[W_DATE_EPEX], " +
                                        "[W_REC], " +
                                        "[W_TRAN_DATE], " +
                                        "[W_TRAN_TIME], " +
                                        "[W_PAN], " +
                                        "[W_TERM_ID], " +
                                        "[W_FROM_ACCT], " +
                                        "[W_TO_ACCT], " +
                                        "[W_POSTDAT], " +
                                        "[W_T_CDE], " +
                                        "[W_AMT_1], " +
                                        "[W_CODE], " +
                                        "[W_SEQ_NUM], " +
                                        "[W_AUTH_ID], " +
                                        "[FILLER], " +
                                        "[W_COUNTRY], " +
                                        "[W_CURRENCY], " +
                                        "[W_TERMINAL_CAPABILITY], " +
                                        "[W_CARD_TYPE], " +
                                        "[W_DATE_EPEX2], " +
                                        ") " +
                                    "VALUES " +
                                        "( " +
                                        "@Origin, " +
                                        "@OriginFileName, " +
                                        "@OriginalRecordId, " +
                                        "@TransDate, " +
                                        "@TransTraceNumber, " +
                                        "@TerminalIdentification, " +
                                        "@W_DATE_EPEX, " +
                                        "@W_REC, " +
                                        "@W_TRAN_DATE, " +
                                        "@W_TRAN_TIME, " +
                                        "@W_PAN, " +
                                        "@W_TERM_ID, " +
                                        "@W_FROM_ACCT, " +
                                        "@W_TO_ACCT, " +
                                        "@W_POSTDAT, " +
                                        "@W_T_CDE, " +
                                        "@W_AMT_1, " +
                                        "@W_CODE, " +
                                        "@W_SEQ_NUM, " +
                                        "@W_AUTH_ID, " +
                                        "@FILLER, " +
                                        "@W_COUNTRY, " +
                                        "@W_CURRENCY, " +
                                        "@W_TERMINAL_CAPABILITY, " +
                                        "@W_CARD_TYPE, " +
                                        "@W_DATE_EPEX2, " +
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
                        cmd.Parameters.AddWithValue("@W_DATE_EPEX", W_DATE_EPEX);
                        cmd.Parameters.AddWithValue("@W_REC", W_REC);
                        cmd.Parameters.AddWithValue("@W_TRAN_DATE", W_TRAN_DATE);
                        cmd.Parameters.AddWithValue("@W_TRAN_TIME", W_TRAN_TIME);
                        cmd.Parameters.AddWithValue("@W_PAN", W_PAN);
                        cmd.Parameters.AddWithValue("@W_TERM_ID", W_TERM_ID);
                        cmd.Parameters.AddWithValue("@W_FROM_ACCT", W_FROM_ACCT);
                        cmd.Parameters.AddWithValue("@W_TO_ACCT", W_TO_ACCT);
                        cmd.Parameters.AddWithValue("@W_POSTDAT", W_POSTDAT);
                        cmd.Parameters.AddWithValue("@W_T_CDE", W_T_CDE);
                        cmd.Parameters.AddWithValue("@W_AMT_1", W_AMT_1);
                        cmd.Parameters.AddWithValue("@W_CODE", W_CODE);
                        cmd.Parameters.AddWithValue("@W_SEQ_NUM", W_SEQ_NUM);
                        cmd.Parameters.AddWithValue("@W_AUTH_ID", W_AUTH_ID);
                        cmd.Parameters.AddWithValue("@FILLER", FILLER);
                        cmd.Parameters.AddWithValue("@W_COUNTRY", W_COUNTRY);
                        cmd.Parameters.AddWithValue("@W_CURRENCY", W_CURRENCY);
                        cmd.Parameters.AddWithValue("@W_TERMINAL_CAPABILITY", W_TERMINAL_CAPABILITY);
                        cmd.Parameters.AddWithValue("@W_CARD_TYPE", W_CARD_TYPE);
                        cmd.Parameters.AddWithValue("@W_DATE_EPEX2", W_DATE_EPEX2);

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



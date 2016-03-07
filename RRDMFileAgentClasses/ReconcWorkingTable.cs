using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
using System.Data.SqlClient;
using System.Configuration;

namespace RRDMFileAgentClasses
{
    public class ReconcWorkingTable
    {
        public int SeqNo;
        public string OriginFileName;
        public int OriginalRecordId;
        public string RMCateg;
        public int RMCycle;
        // public int UniqueRecordId; not to be filled 
        public string Origin;
        public string TransTypeAtOrigin;
        public string Product;
        public string CostCentre;

        public string TerminalId;
        public int TransType;
        public string TransDescr;
        public string CardNumber;
        public string AccNumber;
        public string TransCurr;
        public decimal TransAmount; // SQL Decimal
        public DateTime TransDate;
        public int AtmTraceNo;
        public int RRNumber;
        public int ResponseCode;
        public string T24RefNumber;
        
        /*
         * Not relevant to the RRDM File Agent
        public bool Matched;
        public string MatchMask;
        public DateTime SystemMatchingDtTm;
        public string MatchedType;
        public string UnMatchedType;
        public int MetaExceptionId;
        public bool ActionByUser;
        public string UserId;
        public string Authoriser;
        public DateTime AuthoriserDtTm;
        public bool RemainsForMatching;
        public bool WaitingForUpdating;
        */

        public bool OpenRecord;
        public string Operator;

        public bool ErrorFound;
        public string ErrorOutput;

        // Uses ReconConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;



        public void TruncateTable(string TableName)
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
                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = string.Format("An error occured while TRUNCATING table {0}\nThe error message is:\n{1}", TableName, ex.Message);
                }
        }


        public void InsertSingle(string TWInitialTableName)
        {

            ErrorFound = false;
            ErrorOutput = "";


            string cmdInsert = "INSERT INTO [dbo].[" + TWInitialTableName + "] " + 
                                    "( " +
                                    "[OriginFileName], [OriginalRecordId], " +
                                    "[RMCateg], [RMCycle], [Origin], [TransTypeAtOrigin], [Product], [CostCentre], " +
                                    "[TerminalId], [TransType], [TransDescr], " +
                                    "[CardNumber], [AccNumber], [TransCurr], [TransAmount], [TransDate], [AtmTraceNo], " +
                                    "[RRNumber], [ResponseCode], [T24RefNumber], [OpenRecord], [Operator] " +
                                    ")" +
                                " VALUES ( " +
                                    "@OriginFileName, @OriginalRecordId, " +
                                    "@RMCateg, @RMCycle, @Origin, @TransTypeAtOrigin, @Product, @CostCentre, " +
                                    "@TerminalId, @TransType, @TransDescr, " +
                                    "@CardNumber, @AccNumber, @TransCurr, @TransAmount, @TransDate, @AtmTraceNo, " +
                                    "@RRNumber, @ResponseCode, @T24RefNumber, @OpenRecord, @Operator )";
            
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);
                        // cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@AccNumber", AccNumber);
                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);
                        cmd.Parameters.AddWithValue("@TransAmount", TransAmount);
                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);
                        cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            ErrorFound = false;
                            ErrorOutput = "";
                        }
                        else
                        {
                            ErrorFound = true;
                            ErrorOutput = string.Format("An error occured while INSERTING a new row in {0}", TWInitialTableName);
                            
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = string.Format("An error occured while INSERTING in {0}\nThe error message is:\n{1}", TWInitialTableName, ex.Message);
                }
        }
    }
}

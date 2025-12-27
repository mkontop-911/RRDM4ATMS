using System;
//using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTempTablesForReportsNOSTRO : Logger
    {
        public RRDMTempTablesForReportsNOSTRO() : base() { }

        public int SeqNo;
        public int ColorNo;

        public int MatchingNo;
        public string Origin;
        public string DONE;

        public string TxnType;

        public DateTime  ValueDate;
        public DateTime EntryDate;

        public string DRCR ;
        public string Ccy ; 
        public decimal Amt;

        public string OurRef;
        public string TheirRef;
        public string OtherDetails;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Insert Report64
        ////
        public void InsertReport64(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows; 

        string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport64] "
                + " ([UserId], "
                + " [SeqNo],"
                + " [ColorNo],"
                + " [MatchingNo], "
                + " [Origin], "
                + " [DONE],"
                + " [TxnType],"
                + " [ValueDate],"
                + " [EntryDate], "
                + " [DRCR], "
                 + " [Ccy], "
                + " [Amt], "
                + " [OurRef],"
                + " [TheirRef],"
                + " [OtherDetails]) "
                + " VALUES "
                + "(@UserId,"
                + "@SeqNo,"
                + "@ColorNo,"
                 + "@MatchingNo,"
                  + "@Origin,"
                + "@DONE,"
                + "@TxnType,"
                + "@ValueDate,"
                + "@EntryDate,"
                + "@DRCR,"
                 + "@Ccy,"
                + "@Amt,"
                + "@OurRef,"
                + "@TheirRef,"
                + "@OtherDetails)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@ColorNo", ColorNo);

                        cmd.Parameters.AddWithValue("@MatchingNo", MatchingNo);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@DONE", DONE);

                        cmd.Parameters.AddWithValue("@TxnType", TxnType);

                        cmd.Parameters.AddWithValue("@ValueDate", ValueDate);
                        cmd.Parameters.AddWithValue("@EntryDate", EntryDate);

                        cmd.Parameters.AddWithValue("@DRCR", DRCR);
                     
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amt", Amt);

                        cmd.Parameters.AddWithValue("@OurRef", OurRef);
                        cmd.Parameters.AddWithValue("@TheirRef", TheirRef);
                        cmd.Parameters.AddWithValue("@OtherDetails", OtherDetails);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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

        //
        // DELETE Report64
        //
        public void DeleteReport64(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport64] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

                    CatchDetails(ex);

                }
        }

        // Insert Report65 By DAtes 
        ////
        public string Status;
        public string Settled;
        public decimal StmtBal;  
      
        public void InsertReport65(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport65] "
                    + " ([UserId], "
                    + " [SeqNo],"
                    + " [Status],"
                    + " [Settled],"
                    + " [MatchingNo], "
                    + " [Origin], "
                    + " [TxnType],"
                    + " [ValueDate],"
                    + " [EntryDate], "
                    + " [DRCR], "
                    + " [Ccy], "
                    + " [Amt], "
                    + " [StmtBal], "
                    + " [OurRef],"
                    + " [TheirRef],"
                    + " [OtherDetails]) "
                    + " VALUES "
                    + "(@UserId,"
                    + "@SeqNo,"
                    + "@Status,"
                    + "@Settled,"
                    + "@MatchingNo,"
                    + "@Origin,"
                    + "@TxnType,"
                    + "@ValueDate,"
                    + "@EntryDate,"
                    + "@DRCR,"
                    + "@Ccy,"
                    + "@Amt,"
                    + "@StmtBal,"
                    + "@OurRef,"
                    + "@TheirRef,"
                    + "@OtherDetails)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Settled", Settled);
                     
                        cmd.Parameters.AddWithValue("@MatchingNo", MatchingNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@TxnType", TxnType);

                        cmd.Parameters.AddWithValue("@ValueDate", ValueDate);
                        cmd.Parameters.AddWithValue("@EntryDate", EntryDate);
                        cmd.Parameters.AddWithValue("@DRCR", DRCR);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amt", Amt);
                        cmd.Parameters.AddWithValue("@StmtBal", StmtBal);

                        cmd.Parameters.AddWithValue("@OurRef", OurRef);
                        cmd.Parameters.AddWithValue("@TheirRef", TheirRef);
                        cmd.Parameters.AddWithValue("@OtherDetails", OtherDetails);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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

        //
        // DELETE Report65
        //
        public void DeleteReport65(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport65] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

                    CatchDetails(ex);

                }
        }

        // Insert Report66 = Visa Settlement
        ////
        public string MatchedType;
        public string CardNo;
        public string AccNo;
        public string RRN;
        public string TermId;
        public string CategoryId; 
        public void InsertReport66(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport66] "
                    + " ([UserId], "
                    + " [SeqNo],"
                    + " [ColorNo],"
                     + " [CategoryId],"
                    + " [MatchingNo], "
                    + " [MatchedType], "
                    + " [Origin], "
                    + " [CardNo], "             
                    + " [DONE],"
                    + " [TxnType],"
                    + " [EntryDate], "
                    + " [DRCR], "                 
                    + " [Amt], "
                    + " [Ccy], "
                    + " [RRN],"
                    + " [OtherDetails],"
                        + " [TermId], "
                    + " [AccNo]) "
                    + " VALUES "
                    + "(@UserId,"
                    + "@SeqNo,"
                    + "@ColorNo,"
                    + "@CategoryId,"
                    + "@MatchingNo,"
                    + "@MatchedType,"
                    + "@Origin,"
                    + "@CardNo,"
                    + "@DONE,"
                    + "@TxnType,"
                    + "@EntryDate,"
                    + "@DRCR,"                   
                    + "@Amt,"
                    + "@Ccy,"
                    + "@RRN,"
                    + "@OtherDetails,"
                    + "@TermId,"
                    + "@AccNo )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@ColorNo", ColorNo);

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@MatchingNo", MatchingNo);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                      
                        cmd.Parameters.AddWithValue("@DONE", DONE);

                        cmd.Parameters.AddWithValue("@TxnType", TxnType);

                        cmd.Parameters.AddWithValue("@EntryDate", EntryDate);

                        cmd.Parameters.AddWithValue("@DRCR", DRCR);

                        cmd.Parameters.AddWithValue("@Amt", Amt);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        
                        cmd.Parameters.AddWithValue("@RRN", RRN);
                        cmd.Parameters.AddWithValue("@OtherDetails", OtherDetails);
                        cmd.Parameters.AddWithValue("@TermId", TermId);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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

        //
        // DELETE Report66
        //
        public void DeleteReport66(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport66] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

                    CatchDetails(ex);

                }
        }
        // E_FINANCE BRANCH TOTALS

        //Tr.CategoryId = (string)TableBranchesTotals.Rows[K]["CategoryId"];
        public int TXNs_Matched_E_FIN;
        public decimal Matched_E_FIN;
        public int TXNs_UnMatched_E_FIN;
        public decimal UnMatched_E_FIN;

        public int TXNs_Matched_GL;
        public decimal Matched_GL;
        public int TXNs_UnMatched_GL;
        public decimal UnMatched_GL;
        public void InsertReport77(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport77] "
                    + " ([UserId], "
                    + " [CategoryId],"  
                     
                    + " [TXNs_Matched_E_FIN], "
                    + " [Matched_E_FIN], "
                    + " [TXNs_UnMatched_E_FIN], "
                    + " [UnMatched_E_FIN], "

                     + " [TXNs_Matched_GL], "
                    + " [Matched_GL], "
                    + " [TXNs_UnMatched_GL], "
                    + " [UnMatched_GL]) "
                    + " VALUES "
                    + "(@UserId,"
                    + "@CategoryId,"    
                            
                    + "@TXNs_Matched_E_FIN,"
                    + "@Matched_E_FIN,"
                    + "@TXNs_UnMatched_E_FIN,"
                    + "@UnMatched_E_FIN,"

                    + "@TXNs_Matched_GL,"
                    + "@Matched_GL,"
                    + "@TXNs_UnMatched_GL,"
                    + "@UnMatched_GL )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                        cmd.Parameters.AddWithValue("@TXNs_Matched_E_FIN", TXNs_Matched_E_FIN);
                        cmd.Parameters.AddWithValue("@Matched_E_FIN", Matched_E_FIN);

                        cmd.Parameters.AddWithValue("@TXNs_UnMatched_E_FIN", TXNs_UnMatched_E_FIN);
                        cmd.Parameters.AddWithValue("@UnMatched_E_FIN", UnMatched_E_FIN);

                        cmd.Parameters.AddWithValue("@TXNs_Matched_GL", TXNs_Matched_GL);
                        cmd.Parameters.AddWithValue("@Matched_GL", Matched_GL);

                        cmd.Parameters.AddWithValue("@TXNs_UnMatched_GL", TXNs_UnMatched_GL);
                        cmd.Parameters.AddWithValue("@UnMatched_GL", UnMatched_GL);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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

        //
        // DELETE Report66
        //
        public void DeleteReport77(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport77] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

                    CatchDetails(ex);
                }
        }
     
    }
}



using System;
//using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTempTablesForReportsITMX : Logger
    {
        public RRDMTempTablesForReportsITMX() : base() { }

        public string SignedId; 
        public string BankId;
        public string FeesEntity; 
        public string FullName;
        public decimal DebitAmount;
        public decimal CreditAmount;
        public decimal Difference;
        public decimal NetFees;
        
        public string OtherBank; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Insert Report30
        ////
      
        public int SettlementCycle;
        public DateTime Date;
        public decimal DebitedAmount;
        public decimal CreditedAmount;

        public void InsertReport30(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport30] "
                + " ([SettlementCycle], "
                + " [UserId],"
                + " [Date],"
                + " [DebitedAmount],"
                + " [CreditedAmount], "
                + " [Difference]  ) "
                + " VALUES "
                + "(@SettlementCycle,"
                + " @UserId,"
                 + " @Date,"
                + " @DebitedAmount,"
                + " @CreditedAmount,"
                + " @Difference )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@SettlementCycle", SettlementCycle);
                        cmd.Parameters.AddWithValue("@Date", Date);

                        cmd.Parameters.AddWithValue("@DebitedAmount", DebitedAmount);
                        cmd.Parameters.AddWithValue("@CreditedAmount", CreditedAmount);

                        cmd.Parameters.AddWithValue("@Difference", Difference);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        // Insert BanksDrCrAmounts
        //
        public void InsertReport31(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport31] "
                + " ([BankId], "
                 + " [UserId],"
                + " [FullName],"
                + " [DebitAmount],"
                + " [CreditAmount], "
                + " [Difference]  ) "
                + " VALUES " 
                + "(@BankId,"
                + "@UserId,"
                  + "@FullName,"
                + "@DebitAmount,"
                + "@CreditAmount,"
                + "@Difference )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@FullName", FullName);

                        cmd.Parameters.AddWithValue("@DebitAmount", DebitAmount);
                        cmd.Parameters.AddWithValue("@CreditAmount", CreditAmount);

                        cmd.Parameters.AddWithValue("@Difference", Difference);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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

        // Insert FEES DrCr FEES Amounts
        //
        public void InsertReport41(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport41] "
                + " ([FeesEntity], "
                   + " [UserId],"
                + " [FullName],"
                + " [SettlementCycle],"
                + " [Date],"
                + " [DebitAmount],"
                + " [CreditAmount], "
                + " [NetFees]  ) "
                + " VALUES "
                + "(@FeesEntity,"
                  + "@UserId,"
                + "@FullName,"
                + "@SettlementCycle,"
                + "@Date,"
                + "@DebitAmount,"
                + "@CreditAmount,"
                + "@NetFees )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@FeesEntity", FeesEntity);
                        cmd.Parameters.AddWithValue("@FullName", FullName);

                        cmd.Parameters.AddWithValue("@SettlementCycle", SettlementCycle);
                        cmd.Parameters.AddWithValue("@Date", Date);

                        cmd.Parameters.AddWithValue("@DebitAmount", DebitAmount);
                        cmd.Parameters.AddWithValue("@CreditAmount", CreditAmount);

                        cmd.Parameters.AddWithValue("@NetFees", NetFees);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        // DELETE report 30
        //
        public void DeleteAllTableEntriesReport30(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport30] "
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
        //
        // DELETE report 31
        //
        public void DeleteAllTableEntriesReport31(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport31] "
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

        //
        // DELETE report 41
        //
        public void DeleteAllTableEntriesReport41(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport41] "
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

        // Insert InsertReport32
        //
        public void InsertReport32(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReportSettlement32] "
                + " ([BankId], "
                 + " [UserId],"
                + " [OtherBank],"
                + " [DebitAmount],"
                + " [CreditAmount], "
                + " [Difference]  ) "
                + " VALUES "
                + "(@BankId,"
                   + "@UserId,"
                + "@OtherBank,"
                + "@DebitAmount,"
                + "@CreditAmount,"
                + "@Difference )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@OtherBank", OtherBank);

                        cmd.Parameters.AddWithValue("@DebitAmount", DebitAmount);
                        cmd.Parameters.AddWithValue("@CreditAmount", CreditAmount);

                        cmd.Parameters.AddWithValue("@Difference", Difference);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        // Insert InsertReport42
        //
        public string BankA;
        public string BankB;
        public DateTime DateTmCreated;
        public decimal DRAmount;
        public decimal CRAmount;

        public void InsertReport42(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport42] "
                + " ([FeesEntity], "
                   + " [UserId],"
                + " [BankA],"
                + " [BankB],"
                + " [DRAmount], "
                + " [CRAmount], "
                + " [NetFees], "
                + " [DateTmCreated], "
                + " [SettlementCycle]  ) "
                + " VALUES "
                + "(@FeesEntity,"
                  + "@UserId,"
                + "@BankA,"
                + "@BankB,"
                + "@DRAmount,"
                 + "@CRAmount,"
                  + "@NetFees,"
                   + "@DateTmCreated,"
                + "@SettlementCycle )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@FeesEntity", FeesEntity);

                        cmd.Parameters.AddWithValue("@BankA", BankA);
                        cmd.Parameters.AddWithValue("@BankB", BankB);

                        cmd.Parameters.AddWithValue("@DRAmount", DRAmount);

                        cmd.Parameters.AddWithValue("@CRAmount", CRAmount);

                        cmd.Parameters.AddWithValue("@NetFees", NetFees);
                        cmd.Parameters.AddWithValue("@DateTmCreated", DateTmCreated);

                        cmd.Parameters.AddWithValue("@SettlementCycle", SettlementCycle);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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

        // Insert InsertReport42
        //
        public string LayerName;
        public string FromAmount;
        public string ToAmount;
        public decimal TotalFees;
        public string EntityA;
        public decimal FeesEntityA;
        public string EntityB;
        public decimal FeesEntityB;
        public string EntityC;
        public decimal FeesEntityC; 

        public void InsertReport43(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";
//            EntityA nvarchar(20)    Unchecked
//FeesEntityA decimal(18, 2)  Unchecked
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport43] "
                + " ([LayerName], "
                   + " [UserId],"
                + " [FromAmount],"
                + " [ToAmount],"
                + " [TotalFees], "
                + " [EntityA], "
                + " [FeesEntityA], "
                + " [EntityB], "
                + " [FeesEntityB], "
                + " [EntityC], "
                + " [FeesEntityC]  ) "
                + " VALUES "
                + "(@LayerName,"
                  + "@UserId,"
                + "@FromAmount,"
                + "@ToAmount,"
                + "@TotalFees,"
                + "@EntityA,"
                + "@FeesEntityA,"
                + "@EntityB,"
                + "@FeesEntityB,"
                + "@EntityC,"
                + "@FeesEntityC )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@LayerName", LayerName);

                        cmd.Parameters.AddWithValue("@FromAmount", FromAmount);
                        cmd.Parameters.AddWithValue("@ToAmount", ToAmount);

                        cmd.Parameters.AddWithValue("@TotalFees", TotalFees);

                        cmd.Parameters.AddWithValue("@EntityA", EntityA);
                        cmd.Parameters.AddWithValue("@FeesEntityA", FeesEntityA);

                        cmd.Parameters.AddWithValue("@EntityB", EntityB);
                        cmd.Parameters.AddWithValue("@FeesEntityB", FeesEntityB);

                        cmd.Parameters.AddWithValue("@EntityC", EntityC);
                        cmd.Parameters.AddWithValue("@FeesEntityC", FeesEntityC);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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

        public int TNXS; 
        public decimal TotalAmount; 
        public decimal Avg_Amnt;
        public int UnMatched;
        public int TxnsFees;
        public decimal Fees;
        public decimal Avg_FeesAmnt; 

        public void InsertReport44(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";
     
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport44] "
                + " ([UserId], "
                + " [date],"
                + " [TNXS],"
                + " [TotalAmount],"
                + " [Avg_Amnt],"
                + " [UnMatched], "
                + " [TxnsFees], "
                + " [Fees], "
                + " [Avg_FeesAmnt]  ) "
                + " VALUES "
                + "(@UserId,"
                 + "@date,"
                + "@TNXS,"
                + "@TotalAmount,"
                + "@Avg_Amnt,"
                + "@UnMatched,"
                + "@TxnsFees,"
                + "@Fees,"
                + "@Avg_FeesAmnt )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@date", Date);
                       
                        cmd.Parameters.AddWithValue("@TNXS", TNXS);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@Avg_Amnt", Avg_Amnt);

                        cmd.Parameters.AddWithValue("@UnMatched", UnMatched);

                        cmd.Parameters.AddWithValue("@TxnsFees", TxnsFees);
                        cmd.Parameters.AddWithValue("@Fees", Fees);

                        cmd.Parameters.AddWithValue("@Avg_FeesAmnt", Avg_FeesAmnt);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        public int perYear;
        public int perMonth; 

        public void InsertReport45(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";
     
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport45] "
                + " ([UserId], "
                 + " [perMonth], "
                + " [TNXS],"
                + " [TotalAmount],"
                + " [Avg_Amnt],"
                + " [UnMatched], "
                + " [TxnsFees], "
                + " [Fees], "
                + " [Avg_FeesAmnt]  ) "
                + " VALUES "
                + "(@UserId,"
                 + "@perMonth,"
                + "@TNXS,"
                + "@TotalAmount,"
                + "@Avg_Amnt,"
                + "@UnMatched,"
                + "@TxnsFees,"
                + "@Fees,"
                + "@Avg_FeesAmnt )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@perYear", perYear);
                        cmd.Parameters.AddWithValue("@perMonth", perMonth);
                        cmd.Parameters.AddWithValue("@TNXS", TNXS);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@Avg_Amnt", Avg_Amnt);

                        cmd.Parameters.AddWithValue("@UnMatched", UnMatched);

                        cmd.Parameters.AddWithValue("@TxnsFees", TxnsFees);
                        cmd.Parameters.AddWithValue("@Fees", Fees);

                        cmd.Parameters.AddWithValue("@Avg_FeesAmnt", Avg_FeesAmnt);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        // Insert InsertReport34
        //

        public int RecordId;
        public string Status;
        public string ReconcCategory;
        public string RecCycle;
        public string Ccy;
        public string Amount;

        public string DebitMASK;
        public string CreditMASK;

        public DateTime ExecutionTxnDtTm; 

        public void InsertReport34(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport34MasterTxns] "
                + " ([RecordId], "
                   + " [UserId],"
                + " [Status],"
                + " [ReconcCategory],"
                + " [RecCycle], "
                 + " [Ccy],"
                + " [Amount], "
                 + " [DebitMASK],"
                + " [CreditMASK], "
                + " [ExecutionTxnDtTm]  ) "
                + " VALUES "
                + "(@RecordId,"
                 + "@UserId,"
                + "@Status,"
                + "@ReconcCategory,"
                + "@RecCycle,"
                 + "@Ccy,"
                + "@Amount,"
                    + "@DebitMASK,"
                + "@CreditMASK,"
                + "@ExecutionTxnDtTm )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@RecordId", RecordId);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@ReconcCategory", ReconcCategory);

                        cmd.Parameters.AddWithValue("@RecCycle", RecCycle);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                  
                        cmd.Parameters.AddWithValue("@DebitMASK", DebitMASK);
                        cmd.Parameters.AddWithValue("@CreditMASK", CreditMASK);
                        cmd.Parameters.AddWithValue("@ExecutionTxnDtTm", ExecutionTxnDtTm);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
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
        // DELETE Report32
        //
        public void DeleteReport32(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReportSettlement32] "
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

        //
        // DELETE Report42
        //
        public void DeleteReport42(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport42] "
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

        //
        // DELETE Report43
        //
        public void DeleteReport43(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport43] "
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

        //
        // DELETE Report44
        //
        public void DeleteReport44(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport44] "
                            + " WHERE UserId = @UserId  ", conn))
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

        //
        // DELETE Report45
        //
        public void DeleteReport45(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport45] "
                            + " WHERE UserId = @UserId  ", conn))
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
        //
        // DELETE Report34
        //
        public void DeleteReport34(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport34MasterTxns] "
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



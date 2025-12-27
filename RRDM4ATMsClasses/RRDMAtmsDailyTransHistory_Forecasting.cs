using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsDailyTransHistory_Forecasting : Logger
    {
        public RRDMAtmsDailyTransHistory_Forecasting() : base() { }

        public int SeqNo; 
        public string AtmNo;
        public int OrdersCycleNo; 
        public int ReplOrderId;

        public DateTime FutureDt;

        public decimal Est_DispensedAmt;
        public decimal Est_DepAmount;
        public DateTime CreationDateTime; 

        public string Operator; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Read Record Fields
        private void ReadHstFields(SqlDataReader rdr)
        {
            // Read Details
            SeqNo = (int)rdr["SeqNo"];
            AtmNo = (string)rdr["AtmNo"];

            OrdersCycleNo = (int)rdr["OrdersCycleNo"];
            ReplOrderId = (int)rdr["ReplOrderId"];
            FutureDt = (DateTime)rdr["FutureDt"];

            Est_DispensedAmt = (decimal)rdr["Est_DispensedAmt"];

            Est_DepAmount = (decimal)rdr["Est_DepAmount"];

            CreationDateTime = (DateTime)rdr["CreationDateTime"];

            Operator = (string)rdr["Operator"];
        }

        // READ Record for a specific day  


        public void ReadTransHistory_Forecasting(string InAtmNo, string InOperator, DateTime InFutureDt)
        {
            int Count = 0; 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // For the same day it be two records
            // One for one Cycle number and the other for the other Cycle Number
            // EACH record has the Cycle Number in order to facilate the UNDO cycle operation 

            int T_DrTransactions = 0;
            decimal T_DispensedAmt = 0;

            int T_CrTransactions = 0;
            decimal T_DepAmount = 0;

            string SqlString = "SELECT * "
                 + " FROM [dbo].[AtmDispAmtsByDay_Forecasting] "
                 + " WHERE AtmNo = @AtmNo AND Operator = @Operator AND FutureDt = @FutureDt "
                 + " "
                 ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@FutureDt", InFutureDt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadHstFields(rdr);

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

        //// Find Dispensed Total, between two days 
        //public int TotalRecords = 0;
        //public decimal ReadTotalDispForDaysRange(string InAtmNo, DateTime InDateFrom, DateTime InDateTo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    decimal TotalDisp = 0;
        //    TotalRecords = 0; 

        //    string SqlString = "SELECT DispensedAmt "
        //  + " FROM [dbo].[AtmDispAmtsByDay] "
        //  + " WHERE AtmNo = @AtmNo AND (Dt>= @DateFrom AND Dt <= @DateTo)  "
        //    + "  ";
          
        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
        //                cmd.Parameters.AddWithValue("@DateTo", InDateTo);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    TotalRecords = TotalRecords + 1; 
        //                    // Read Details
        //                    TotalDisp = TotalDisp + (decimal)rdr["DispensedAmt"];

        //                }

        //                // Close Reader
        //                rdr.Close();

        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchMethod(ex);
        //        }

        //    return TotalDisp; 
        //}
       
      
        //
        // Insert new forecasting record 
        //
        public void InsertTransHistory_ForecastingRecord(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [AtmDispAmtsByDay_Forecasting]"
                + " ([AtmNo],"
                + " [OrdersCycleNo],"
                + " [ReplOrderId], "
                + " [FutureDt], "
                + " [Est_DispensedAmt], "
                + " [Est_DepAmount], "
                + " [CreationDateTime], "
                + " [Operator] )"
                + " VALUES "
                + " (@AtmNo,"
                + " @OrdersCycleNo, "
                + " @ReplOrderId, "
                + " @FutureDt, "
                + " @Est_DispensedAmt, "
                + " @Est_DepAmount, "
                + " @CreationDateTime, "
                + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@OrdersCycleNo", OrdersCycleNo);
                        cmd.Parameters.AddWithValue("@ReplOrderId", ReplOrderId);
                        cmd.Parameters.AddWithValue("@FutureDt", FutureDt);

                        cmd.Parameters.AddWithValue("@Est_DispensedAmt", Est_DispensedAmt);
                        cmd.Parameters.AddWithValue("@Est_DepAmount", Est_DepAmount);

                        cmd.Parameters.AddWithValue("@CreationDateTime", CreationDateTime);
                      
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        cmd.CommandTimeout = 200;
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
        //
        // UPDATE PreEstimated Dispensed
        // 
        public void UpdatePreEstimatedDispensed(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[AtmDispAmtsByDay] SET "
                            + " PreEstimated = @PreEstimated"
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                       
                     //   cmd.Parameters.AddWithValue("@PreEstimated", PreEstimated);
                        
                        rows = cmd.ExecuteNonQuery();
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



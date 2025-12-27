using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsDailyTransHistory : Logger
    {
        public RRDMAtmsDailyTransHistory() : base() { }
        // DECLARE Class FIELDS 
        public int SeqNo; 
        public string AtmNo;
        public string BankId;
//        Dt date    Unchecked
//        LoadedAtRMCycle int Unchecked
        public DateTime Dt;
        public int LoadedAtRMCycle; 
 
        public int DrTransactions; 
        public decimal DispensedAmt; 
       
        public decimal PreEstimated; 

        public int CrTransactions; 
        public decimal DepAmount; 

        public decimal C301DailyMaintAmount; 
        public decimal C303ReplTimeCost;
        public decimal C307OverheadCost; 
        public decimal C308CostOfMoney;
        public decimal C309CostOfInvest; 

        public int R401CommTran; 
        public decimal R401CommAmount; 

        public int R402CommTran; 
        public decimal R402CommAmount; 
        
        public int R403CommTran; 
        public decimal R403CommAmount; 

        public int R404CommTran; 
        public decimal R404CommAmount; 

        public int R405CommTran; 
        public decimal R405CommAmount; 
         
        public int RecordType;
        public string Description; 
        public int NumberOfTrans; 
        public decimal Amount;
        public DateTime DateCreated;

        public string Operator; 

        DateTime WDt;

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
            BankId = (string)rdr["BankId"];
            Dt = (DateTime)rdr["Dt"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];

            DrTransactions = (int)rdr["DrTransactions"];
            DispensedAmt = (decimal)rdr["DispensedAmt"];

            PreEstimated = (decimal)rdr["PreEstimated"];
            CrTransactions = (int)rdr["CrTransactions"];
            DepAmount = (decimal)rdr["DepAmount"];

            C301DailyMaintAmount = (decimal)rdr["C301DailyMaintAmount"];
            C303ReplTimeCost = (decimal)rdr["C303ReplTimeCost"];
            C307OverheadCost = (decimal)rdr["C307OverheadCost"];
            C308CostOfMoney = (decimal)rdr["C308CostOfMoney"];
            C309CostOfInvest = (decimal)rdr["C309CostOfInvest"];

            R401CommTran = (int)rdr["R401CommTran"];
            R401CommAmount = (decimal)rdr["R401CommAmount"];

            R402CommTran = (int)rdr["R402CommTran"];
            R402CommAmount = (decimal)rdr["R402CommAmount"];

            R403CommTran = (int)rdr["R403CommTran"];
            R403CommAmount = (decimal)rdr["R403CommAmount"];

            R404CommTran = (int)rdr["R404CommTran"];
            R404CommAmount = (decimal)rdr["R404CommAmount"];

            R405CommTran = (int)rdr["R405CommTran"];
            R405CommAmount = (decimal)rdr["R405CommAmount"];

            Operator = (string)rdr["Operator"];
        }

        // READ Record for a specific day  


        public void ReadTransHistory(string InAtmNo, string InOperator, DateTime InDt)
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
                 + " FROM [dbo].[AtmDispAmtsByDay] "
                 + " WHERE AtmNo = @AtmNo AND Operator = @Operator AND Dt = @Dt"
                 + " ORDER By AtmNo, LoadedAtRMCycle "
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
                        cmd.Parameters.AddWithValue("@Dt", WDt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadHstFields(rdr);

                            Count = Count + 1; 

                            T_DrTransactions = T_DrTransactions + DrTransactions;
                            T_DispensedAmt = T_DispensedAmt + DispensedAmt ;

                            T_CrTransactions = T_CrTransactions + CrTransactions;
                            T_DepAmount = T_DepAmount + DepAmount;

                        }

                        // Close Reader
                        rdr.Close();

                    }

                    // Close conn
                    conn.Close();

                    DrTransactions = T_DrTransactions;
                    DispensedAmt = T_DispensedAmt;
                    CrTransactions = T_CrTransactions ;
                    DepAmount = T_DepAmount ;

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        // Find Dispensed Total, between two days 
        public int TotalRecords = 0;
        public decimal ReadTotalDispForDaysRange(string InAtmNo, DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal TotalDisp = 0;
            TotalRecords = 0; 

            string SqlString = "SELECT DispensedAmt "
          + " FROM [dbo].[AtmDispAmtsByDay] "
          + " WHERE AtmNo = @AtmNo AND (Dt>= @DateFrom AND Dt <= @DateTo)  "
            + "  ";
          
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalRecords = TotalRecords + 1; 
                            // Read Details
                            TotalDisp = TotalDisp + (decimal)rdr["DispensedAmt"];

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

            return TotalDisp; 
        }
        //
        // READ Record for a specific day  
        //
        public void ReadTransHistory_Dispensed_Deposited(string InAtmNo, DateTime InDt, int InLoadedAtRMCycle)
        {
            int dd = InDt.Day;
            int mm = InDt.Month;
           // int yyyy = InDt.Year;

         
            WDt = InDt;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[AtmDispAmtsByDay] "
          + " WHERE AtmNo = @AtmNo AND Dt = @Dt AND LoadedAtRMCycle = @LoadedAtRMCycle";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Dt", WDt);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        // Read table 
                        cmd.CommandTimeout = 200; 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details

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
        //
        // UPDATE Daily Stats
        // 
        public void UpdateDailyStatsPerAtm(string InAtmNo, DateTime InDt)
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
                            + " DrTransactions = @DrTransactions"
                             + " ,DispensedAmt = @DispensedAmt"
                              + " ,CrTransactions = @CrTransactions"
                               + " ,DepAmount = @DepAmount"
                            + " WHERE AtmNo = @AtmNo AND Dt = @Dt ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Dt", InDt);

                        cmd.Parameters.AddWithValue("@DrTransactions", DrTransactions);
                        cmd.Parameters.AddWithValue("@DispensedAmt", DispensedAmt);

                        cmd.Parameters.AddWithValue("@CrTransactions", CrTransactions);
                        cmd.Parameters.AddWithValue("@DepAmount", DepAmount);
                        cmd.CommandTimeout = 200;
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

        //
        // Insert NEW daily Trans 
        //
        public void InsertTransHistory_With_Default(string InAtmNo, DateTime InDateTime)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [AtmDispAmtsByDay]"
                + " ([AtmNo],[BankId],[Dt],[LoadedAtRMCycle],"
                + " [DrTransactions],[DispensedAmt],"
                + " [CrTransactions], [DepAmount],"
                + " [Operator] )"
                + " VALUES "
                + " (@AtmNo,@BankId,@Dt,@LoadedAtRMCycle,"
                + " @DrTransactions, @DispensedAmt,"
                + " @CrTransactions, @DepAmount,"         
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
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Dt", Dt);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

                        cmd.Parameters.AddWithValue("@DrTransactions", DrTransactions);
                        cmd.Parameters.AddWithValue("@DispensedAmt", DispensedAmt);

                        cmd.Parameters.AddWithValue("@CrTransactions", CrTransactions);
                        cmd.Parameters.AddWithValue("@DepAmount", DepAmount);

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
                       
                        cmd.Parameters.AddWithValue("@PreEstimated", PreEstimated);
                        
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
        //
        // Insert NEW Estimated Dispensed
        //
        public void InsertTransHistory_With_PreEstimated(string InAtmNo, DateTime InDate)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [AtmDispAmtsByDay]"
                + " ([AtmNo],[BankId],[Dt],[LoadedAtRMCycle],"
                + " [PreEstimated],"
                + " [Operator] )"
                + " VALUES "
                + " (@AtmNo,@BankId,@Dt,@LoadedAtRMCycle,"
                + " @PreEstimated, "
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
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Dt", Dt);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

                        cmd.Parameters.AddWithValue("@PreEstimated", PreEstimated);
                   
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

        // Insert NEW Trans 
        //
        public void InsertTransHistory(string InAtmNo, DateTime PreviousDt, decimal InDispensedAmt )
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [AtmDispAmtsByDay]"
                + " ([AtmNo],[BankId],[Dt],[LoadedAtRMCycle],[DrTransactions],[DispensedAmt],"
                +    " [PreEstimated], [CrTransactions], [DepAmount],"
                + " [C301DailyMaintAmount],"
                + " [C303ReplTimeCost],"
                + " [C307OverheadCost],"
                + " [C308CostOfMoney],"
                + " [C309CostOfInvest],"
                + " [R401CommTran], [R401CommAmount],"
                + " [R402CommTran], [R402CommAmount],"
                + " [R403CommTran], [R403CommAmount],"
                + " [R404CommTran], [R404CommAmount],"
                + " [R405CommTran], [R405CommAmount], [Operator] )"
                + " VALUES "
                + " (@AtmNo,@BankId,@Dt,@LoadedAtRMCycle,@DrTransactions, @DispensedAmt,"
                +   " @PreEstimated, @CrTransactions, @DepAmount,"
                + " @C301DailyMaintAmount,"
                + " @C303ReplTimeCost,"
                + " @C307OverheadCost,"
                + " @C308CostOfMoney,"
                + " @C309CostOfInvest,"
                + " @R401CommTran, @R401CommAmount,"
                + " @R402CommTran, @R402CommAmount,"
                + " @R403CommTran, @R403CommAmount,"
                + " @R404CommTran, @R404CommAmount,"
                +" @R405CommTran, @R405CommAmount, @Operator )"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Dt", Dt);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
             
                        cmd.Parameters.AddWithValue("@DrTransactions", DrTransactions);
                        cmd.Parameters.AddWithValue("@DispensedAmt", DispensedAmt);

                        cmd.Parameters.AddWithValue("@PreEstimated", PreEstimated);
                        cmd.Parameters.AddWithValue("@CrTransactions", CrTransactions);
                        cmd.Parameters.AddWithValue("@DepAmount", DepAmount);
                       
                        cmd.Parameters.AddWithValue("@C301DailyMaintAmount", C301DailyMaintAmount);
                        cmd.Parameters.AddWithValue("@C303ReplTimeCost", C303ReplTimeCost);
                        cmd.Parameters.AddWithValue("@C307OverheadCost", C307OverheadCost);
                        cmd.Parameters.AddWithValue("@C308CostOfMoney", C308CostOfMoney);
                        cmd.Parameters.AddWithValue("@C309CostOfInvest", C309CostOfInvest);

                        cmd.Parameters.AddWithValue("@R401CommTran", R401CommTran);
                        cmd.Parameters.AddWithValue("@R401CommAmount", R401CommAmount);

                        cmd.Parameters.AddWithValue("@R402CommTran", R402CommTran);
                        cmd.Parameters.AddWithValue("@R402CommAmount", R402CommAmount);

                        cmd.Parameters.AddWithValue("@R403CommTran", R403CommTran);
                        cmd.Parameters.AddWithValue("@R403CommAmount", R403CommAmount);

                        cmd.Parameters.AddWithValue("@R404CommTran", R404CommTran);
                        cmd.Parameters.AddWithValue("@R404CommAmount", R404CommAmount);

                        cmd.Parameters.AddWithValue("@R405CommTran", R405CommTran);
                        cmd.Parameters.AddWithValue("@R405CommAmount", R405CommAmount);

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

        // Insert NEW Trans 
        //
        public void InsertTransHistoryByType(string InAtmNo, DateTime PreviousDt)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [AtmsDailyStats]"
                + " ([AtmNo],[BankId],[Dt],[RecordType],[Description],"
                + " [NumberOfTrans],[Amount],[DateCreated], [Operator])"
                + " VALUES "
                + " (@AtmNo,@BankId,@Dt,@RecordType,@Description,"
                + " @NumberOfTrans,@Amount,@DateCreated, @Operator)";
           
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                   
                        cmd.Parameters.AddWithValue("@Dt", Dt);
                        cmd.Parameters.AddWithValue("@RecordType", RecordType);
                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@NumberOfTrans", NumberOfTrans);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
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
// Copy history By Atm 
        public void CopyTransHistory( string InOperator,string InAtmNo, string TargetBankId, string TargetAtmNo,bool TargetPrive)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[AtmDispAmtsByDay] "
          + " WHERE Operator = @Operator AND  AtmNo = @AtmNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details

                            AtmNo = (string)rdr["AtmNo"];
                            BankId = (string)rdr["BankId"];
                            Dt = (DateTime)rdr["Dt"];
                            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
                     
                            DrTransactions = (int)rdr["DrTransactions"];
                            DispensedAmt = (decimal)rdr["DispensedAmt"];

                            PreEstimated = (decimal)rdr["PreEstimated"];
                            CrTransactions = (int)rdr["CrTransactions"];
                            DepAmount = (decimal)rdr["DepAmount"];

                            C301DailyMaintAmount = (decimal)rdr["C301DailyMaintAmount"];
                            C303ReplTimeCost = (decimal)rdr["C303ReplTimeCost"];
                            C307OverheadCost = (decimal)rdr["C307OverheadCost"];
                            C308CostOfMoney = (decimal)rdr["C308CostOfMoney"];
                            C309CostOfInvest = (decimal)rdr["C309CostOfInvest"];

                            R401CommTran = (int)rdr["R401CommTran"];
                            R401CommAmount = (decimal)rdr["R401CommAmount"];

                            R402CommTran = (int)rdr["R402CommTran"];
                            R402CommAmount = (decimal)rdr["R402CommAmount"];

                            R403CommTran = (int)rdr["R403CommTran"];
                            R403CommAmount = (decimal)rdr["R403CommAmount"];

                            R404CommTran = (int)rdr["R404CommTran"];
                            R404CommAmount = (decimal)rdr["R404CommAmount"];

                            R405CommTran = (int)rdr["R405CommTran"];
                            R405CommAmount = (decimal)rdr["R405CommAmount"];

                            // insert
                            Operator = TargetBankId; 
                            AtmNo = TargetAtmNo;
                        //    Prive = TargetPrive; 

                            InsertTransHistory(AtmNo, Dt, DispensedAmt); 

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

     

    }
}



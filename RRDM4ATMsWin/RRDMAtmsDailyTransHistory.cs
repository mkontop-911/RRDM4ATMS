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

namespace RRDM4ATMsWin
{
    class RRDMAtmsDailyTransHistory
    {
        // DECLARE Class FIELDS 
        public string AtmNo;
        public string BankId;
        public DateTime DtTm;
        public int Year; 
 
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

   //     public decimal LastWorking; // Money Dispensed for Normal
   //     public decimal LYearDispSame;  // Money Dispensed at the same date
    //    public decimal LYearDispNear; // Money Dispensed at weekend or holiday
    //    public decimal LMonthDispSame; // Money Dispensed 
   //   public decimal LMonthDispNear; // Money Dispensed 

   //     public bool Holiday;

     //   public int HolId;
   //     public bool Weekend;

        DateTime WDt;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Record for a specific day  

        public void ReadTransHistory(string InAtmNo, string InOperator, DateTime InDtTm)
        {
            int dd = InDtTm.Day;
            int mm = InDtTm.Month;
            int yyyy = InDtTm.Year;

       //     LastWorking = 0 ; // Money Dispensed for Normal
        //    LYearDispSame = 0 ;  // Money Dispensed at the same date
        //    LYearDispNear= 0 ;// Money Dispensed at weekend or holiday
        //    LMonthDispSame= 0 ;// Money Dispensed 
        //    LMonthDispNear= 0 ; // Money Dispensed 

            WDt = InDtTm;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[AtmDispAmtsByDay] "
          + " WHERE AtmNo = @AtmNo AND Operator = @Operator AND DtTm = @DtTm";

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
                        cmd.Parameters.AddWithValue("@DtTm", WDt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details

                            AtmNo = (string)rdr["AtmNo"];
                            BankId = (string)rdr["BankId"];
                            DtTm = (DateTime)rdr["DtTm"];
                            Year = (int)rdr["Year"];

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
                    ErrorOutput = "An error occured in ATMsDailyTransHistory Class............. " + ex.Message;
                }
        }

        // This functionality is for new ATMs where past data are not available except yestredays   

        public void ReadTransHistoryYesterday(string InAtmNo, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[AtmDispAmtsByDay] "
          + " WHERE AtmNo = @AtmNo AND Operator = @Operator"
            + "  ORDER By [DtTm] ASC ";
          
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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details

                            AtmNo = (string)rdr["AtmNo"];
                            BankId = (string)rdr["BankId"];
                            DtTm = (DateTime)rdr["DtTm"];
                            Year = (int)rdr["Year"];
                      //      Prive = (bool)rdr["Prive"];

                            DispensedAmt = (decimal)rdr["DispensedAmt"];            
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
                    ErrorOutput = "An error occured in ATMsDailyTransHistory Class............. " + ex.Message;
                }
        }
     
        
        // Insert NEW Trans 
        //
        public void InsertTransHistory(string InAtmNo, DateTime PreviousDt, decimal InDispensedAmt )
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [AtmDispAmtsByDay]"
                + " ([AtmNo],[BankId],[DtTm],[Year],[DrTransactions],[DispensedAmt],"
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
                + " (@AtmNo,@BankId,@DtTm,@Year,@DrTransactions, @DispensedAmt,"
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
                        cmd.Parameters.AddWithValue("@DtTm", DtTm);
                        cmd.Parameters.AddWithValue("@Year", Year);
             
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMsDailyTransHistory Class............. " + ex.Message;
                }
        }

        // Insert NEW Trans 
        //
        public void InsertTransHistoryByType(string InAtmNo, DateTime PreviousDt)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [AtmsDailyStats]"
                + " ([AtmNo],[BankId],[DtTm],[RecordType],[Description],"
                + " [NumberOfTrans],[Amount],[DateCreated], [Operator])"
                + " VALUES "
                + " (@AtmNo,@BankId,@DtTm,@RecordType,@Description,"
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
                   
                        cmd.Parameters.AddWithValue("@DtTm", DtTm);
                        cmd.Parameters.AddWithValue("@RecordType", RecordType);
                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@NumberOfTrans", NumberOfTrans);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMsDailyTransHistory Class............. " + ex.Message;
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
                            DtTm = (DateTime)rdr["DtTm"];
                            Year = (int)rdr["Year"];
                     
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

                            InsertTransHistory(AtmNo, DtTm, DispensedAmt); 

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
                    ErrorOutput = "An error occured in ATMsDailyTransHistory Class............. " + ex.Message;
                }
        }


    }
}

using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMITMX_DailyBankTotals : Logger
    {
        public RRDMITMX_DailyBankTotals() : base() { }

        public int SeqNo;

      
        public string BankId;
        public int ProductId;
        public DateTime DtTm;
   
        public int NoOfTnxs;
        public decimal TnxsAmount;

        public int NumberOfUnMatched;

        public int NoOfTxnsFees;      
        public decimal TnxsAmountFees;

        public string Operator;

        // Define the data tables 
        public DataTable MISITMXTable = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Methods 
        // READ BanksToBanksDailyTotals
      
        // FILL UP A TABLE
        //
        public void ReadTableTotalsForDaily(string InOperator, string InSignedId, string InBankId, 
                          DateTime InFromDtTm, DateTime InToDtTm, bool InAllBanks, bool InPerMonth)
        {
            //ShowAction11Daily(string Operator, string InBankId, DateTime InFromDtTm, DateTime InToDtTm)
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MISITMXTable = new DataTable();
            MISITMXTable.Clear();

            TotalSelected = 0;

            if (InAllBanks == false & InPerMonth == false)
            {
                SqlString =
                   "SELECT CAST(DtTm AS Date) AS Date,"
                   + " SUM(NoOfTnxs) AS TNXS, SUM(TnxsAmount) As Amount, Avg(TnxsAmount/NoOfTnxs) AS Avg_Amnt,"
                   + " SUM(NumberOfUnMatched) As UnMatched, "
                   + " SUM(NoOfTxnsFees) AS TxnsFees, SUM(TnxsAmountFees) As Fees, Avg_FeesAmnt = SUM(TnxsAmountFees)/SUM(NoOfTxnsFees) "
                   + " FROM [ATMS].[dbo].[ITMX_DailyBankTotals] "
                   + " WHERE Operator = @Operator AND BankId = @BankId AND (DtTm >= @FromDtTm AND DtTm <= @ToDtTm)"
                   + " GROUP BY CAST(DtTm AS Date) "
                   + " ORDER BY CAST(DtTm AS Date) ASC ";
            }

            if (InAllBanks == true & InPerMonth == false)
            {
                SqlString =
                   "SELECT CAST(DtTm AS Date) AS Date,"
                   + " SUM(NoOfTnxs) AS TNXS, SUM(TnxsAmount) As Amount, Avg(TnxsAmount/NoOfTnxs) AS Avg_Amnt,"
                   + " SUM(NumberOfUnMatched) As UnMatched, "
                   + " SUM(NoOfTxnsFees) AS TxnsFees, SUM(TnxsAmountFees) As Fees, Avg_FeesAmnt = SUM(TnxsAmountFees)/SUM(NoOfTxnsFees) "
                   + " FROM [ATMS].[dbo].[ITMX_DailyBankTotals] "
                   + " WHERE Operator = @Operator AND (DtTm >= @FromDtTm AND DtTm <= @ToDtTm)"
                   + " GROUP BY CAST(DtTm AS Date) "
                   + " ORDER BY CAST(DtTm AS Date) ASC ";
            }

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                   

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                    if (InAllBanks == false)
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankId", InBankId);
                    }
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDtTm", InFromDtTm);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDtTm", InToDtTm);

                    sqlAdapt.Fill(MISITMXTable);

                  
                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();

                    //Clear Table 
                    Tr.DeleteReport44(InSignedId); 

                    int I = 0;

                    while (I <= (MISITMXTable.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.Date = (DateTime)MISITMXTable.Rows[I]["Date"];
                        Tr.TNXS = (int)MISITMXTable.Rows[I]["TNXS"];
                        
                        Tr.TotalAmount = (decimal)MISITMXTable.Rows[I]["Amount"];
                        Tr.Avg_Amnt = (decimal)MISITMXTable.Rows[I]["Avg_Amnt"];

                        Tr.UnMatched = (int)MISITMXTable.Rows[I]["UnMatched"];

                        Tr.TxnsFees = (int)MISITMXTable.Rows[I]["TxnsFees"];
                        Tr.Fees = (decimal)MISITMXTable.Rows[I]["Fees"];
                        Tr.Avg_FeesAmnt = (decimal)MISITMXTable.Rows[I]["Avg_FeesAmnt"];

                        Tr.InsertReport44(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                    }
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // Methods 
        // READ BanksToBanksDailyTotals
       
        // FILL UP A TABLE
        //
        public void ReadTableTotalsForMonthly(string InOperator,string InSignedId ,string InBankId,
                          DateTime InFromDtTm, DateTime InToDtTm, bool InAllBanks, bool InPerMonth)
        {
            //ShowAction11Daily(string Operator, string InBankId, DateTime InFromDtTm, DateTime InToDtTm)
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MISITMXTable = new DataTable();
            MISITMXTable.Clear();

            TotalSelected = 0;

            if (InAllBanks == false & InPerMonth == true)
            {
                SqlString =
                    "SELECT perYear = DATEPART(YEAR,DtTm),perMonth = DATEPART(MONTH,DtTm),"
                    + " SUM(NoOfTnxs) AS TNXS, SUM(TnxsAmount) As Amount, Avg(TnxsAmount/NoOfTnxs) AS Avg_Amnt,"
                    + " SUM(NumberOfUnMatched) As UnMatched, "
                    + " SUM(NoOfTxnsFees) AS TxnsFees, SUM(TnxsAmountFees) As Fees, Avg_FeesAmnt = SUM(TnxsAmountFees)/SUM(NoOfTxnsFees) "
                    + " FROM [ATMS].[dbo].[ITMX_DailyBankTotals] "
                    + " WHERE Operator = @Operator AND BankId = @BankId AND (DtTm >= @FromDtTm AND DtTm <= @ToDtTm)"
                    + " GROUP BY DATEPART(YEAR,DtTm), DATEPART(MONTH,DtTm)  ";
            }

            if (InAllBanks == true & InPerMonth == true)
            {
                SqlString =
                    "SELECT perYear = DATEPART(YEAR,DtTm),perMonth = DATEPART(MONTH,DtTm),"
                    + " SUM(NoOfTnxs) AS TNXS, SUM(TnxsAmount) As Amount, Avg(TnxsAmount/NoOfTnxs) AS Avg_Amnt,"
                    + " SUM(NumberOfUnMatched) As UnMatched, "
                    + " SUM(NoOfTxnsFees) AS TxnsFees, SUM(TnxsAmountFees) As Fees, Avg_FeesAmnt = SUM(TnxsAmountFees)/SUM(NoOfTxnsFees) "
                    + " FROM [ATMS].[dbo].[ITMX_DailyBankTotals] "
                    + " WHERE Operator = @Operator AND (DtTm >= @FromDtTm AND DtTm <= @ToDtTm)"
                      + " GROUP BY DATEPART(YEAR,DtTm), DATEPART(MONTH,DtTm)  ";
            }

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    { 

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                    if (InAllBanks == false)
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankId", InBankId);
                    }
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDtTm", InFromDtTm);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDtTm", InToDtTm);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(MISITMXTable);


                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();
                    //RRDMBanks Ba = new RRDMBanks();
    

                    //Clear Table 
                    Tr.DeleteReport45(InSignedId);

                    int I = 0;

                    while (I <= (MISITMXTable.Rows.Count - 1))
                    {

                        RecordFound = true;

                        Tr.perYear = (int)MISITMXTable.Rows[I]["perYear"];
                        Tr.perMonth = (int)MISITMXTable.Rows[I]["perMonth"];

                        Tr.TNXS = (int)MISITMXTable.Rows[I]["TNXS"];

                        Tr.TotalAmount = (decimal)MISITMXTable.Rows[I]["Amount"];
                        Tr.Avg_Amnt = (decimal)MISITMXTable.Rows[I]["Avg_Amnt"];

                        Tr.UnMatched = (int)MISITMXTable.Rows[I]["UnMatched"];

                        Tr.TxnsFees = (int)MISITMXTable.Rows[I]["TxnsFees"];
                        Tr.Fees = (decimal)MISITMXTable.Rows[I]["Fees"];
                        Tr.Avg_FeesAmnt = (decimal)MISITMXTable.Rows[I]["Avg_FeesAmnt"];

                        Tr.InsertReport45(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                    }
                }


                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }



    }
}



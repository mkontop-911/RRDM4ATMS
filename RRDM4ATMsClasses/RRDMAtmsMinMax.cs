using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsCostClass : Logger
    {
        public RRDMAtmsCostClass() : base() { }
        // Variables
        // ATM MAIN FIELDS 
        public string AtmNo;
        public string BankId;
   
        public DateTime ManifactureDt;
        public DateTime PurchaseDt;
        public DateTime DueServiceDt;
        public DateTime LastServiceDt;

        public decimal PurchaseCost;

        public int MaintenanceCd;
        public decimal AnnualMaint;

        public decimal CitOnCall;
        public decimal CitAnnual;
        public string Operator; 

    //    string SqlString; // Do not delete

        // DATATable for Grid 
        // public DataTable GridDays = new DataTable();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Reader Fields 
        private void CostReaderFields(SqlDataReader rdr)
        {
            AtmNo = (string)rdr["AtmNo"];

            BankId = (string)rdr["BankId"];

            ManifactureDt = (DateTime)rdr["ManifactureDt"];
            PurchaseDt = (DateTime)rdr["PurchaseDt"];
            DueServiceDt = (DateTime)rdr["DueServiceDt"];
            LastServiceDt = (DateTime)rdr["LastServiceDt"];

            PurchaseCost = (decimal)rdr["PurchaseCost"];
            MaintenanceCd = (int)rdr["MaintenanceCd"];

            AnnualMaint = (decimal)rdr["AnnualMaint"];
            CitOnCall = (decimal)rdr["CitOnCall"];
            CitAnnual = (decimal)rdr["CitAnnual"];
            Operator = (string)rdr["Operator"];
        }

        // Methods 
        // READ TableATMsPhys
        // 
        public void ReadTableATMsCostSpecific(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[AtmsCost] "
          + " WHERE AtmNo=@AtmNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            CostReaderFields(rdr);

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
        // UPDATE ATMs table of cost 
        //
        public void UpdateTableATMsCost(string InAtmNo, string InBankId)
        {
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = 
                        new SqlCommand("UPDATE [dbo].[ATMsCost] SET "
                    + " [AtmNo]=@AtmNo, [BankId]=@BankId,"
                    + " [ManifactureDt]=@ManifactureDt, [PurchaseDt]=@PurchaseDt, [DueServiceDt]=@DueServiceDt, [LastServiceDt]=@LastServiceDt,"
                    + " [PurchaseCost]=@PurchaseCost, [MaintenanceCd]=@MaintenanceCd,"
                    + " [AnnualMaint]=@AnnualMaint, [CitOnCall]=@CitOnCall, [CitAnnual]=@CitAnnual "
                    + " WHERE AtmNo= @AtmNo AND BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                      //  cmd.Parameters.AddWithValue("@Prive", InPrive);

                        cmd.Parameters.AddWithValue("@ManifactureDt", ManifactureDt);
                        cmd.Parameters.AddWithValue("@PurchaseDt", PurchaseDt);
                        cmd.Parameters.AddWithValue("@DueServiceDt", DueServiceDt);
                        cmd.Parameters.AddWithValue("@LastServiceDt", LastServiceDt);

                        cmd.Parameters.AddWithValue("@PurchaseCost", PurchaseCost);

                        cmd.Parameters.AddWithValue("@MaintenanceCd", MaintenanceCd);

                        cmd.Parameters.AddWithValue("@AnnualMaint", AnnualMaint);
                        cmd.Parameters.AddWithValue("@CitOnCall", CitOnCall);
                        cmd.Parameters.AddWithValue("@CitAnnual", CitAnnual);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            // outcome = " ATMs Table UPDATED ";
                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            //  return outcome;

        }
        // INSERT New Record in cost table  
        public void InsertTableATMsCost(string InAtmNo, string InBankId)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = 
            "INSERT INTO [dbo].[ATMsCost]" 
                + " ([AtmNo], [BankId], " 
                + " [ManifactureDt], [PurchaseDt], [DueServiceDt], [LastServiceDt]," 
                + " [PurchaseCost], [MaintenanceCd], [AnnualMaint], [CitOnCall], [CitAnnual], [Operator])"
                + " VALUES (@AtmNo, @BankId," 
                + " @ManifactureDt, @PurchaseDt, @DueServiceDt, @LastServiceDt," 
                + " @PurchaseCost, @MaintenanceCd, @AnnualMaint, @CitOnCall, @CitAnnual, @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                  //      cmd.Parameters.AddWithValue("@Prive", InPrive);

                        cmd.Parameters.AddWithValue("@ManifactureDt", ManifactureDt);
                        cmd.Parameters.AddWithValue("@PurchaseDt", PurchaseDt);
                        cmd.Parameters.AddWithValue("@DueServiceDt", DueServiceDt);
                        cmd.Parameters.AddWithValue("@LastServiceDt", LastServiceDt);

                        cmd.Parameters.AddWithValue("@PurchaseCost", PurchaseCost);

                        cmd.Parameters.AddWithValue("@MaintenanceCd", MaintenanceCd);

                        cmd.Parameters.AddWithValue("@AnnualMaint", AnnualMaint);
                        cmd.Parameters.AddWithValue("@CitOnCall", CitOnCall);
                        cmd.Parameters.AddWithValue("@CitAnnual", CitAnnual);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                         //   outcome = " Record Inserted ";
                        }

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

    }
}



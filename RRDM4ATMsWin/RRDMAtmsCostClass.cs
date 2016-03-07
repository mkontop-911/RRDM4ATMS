using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    class RRDMAtmsCostClass
    {
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

    //    string SqlString; // Do not delete

        // DATATable for Grid 
        // public DataTable GridDays = new DataTable();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DateTime NullPastDate = new DateTime(1950, 11, 21);

        // Methods 
        // READ TableATMsPhys
        // 
        public void ReadTableATMsCostSpecific(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TableAtmsCost] "
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
                    ErrorOutput = "An error occured in ATMsCostClass............. " + ex.Message;
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
                        new SqlCommand("UPDATE [dbo].[TableATMsCost] SET "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMsCostClass............. " + ex.Message;
                }

            //  return outcome;

        }
        // INSERT New Record in cost table  
        public void InsertTableATMsCost(string InAtmNo, string InBankId)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = 
            "INSERT INTO [dbo].[TableATMsCost]" 
                + " ([AtmNo], [BankId], " 
                + " [ManifactureDt], [PurchaseDt], [DueServiceDt], [LastServiceDt]," 
                + " [PurchaseCost], [MaintenanceCd], [AnnualMaint], [CitOnCall], [CitAnnual])" 
                + " VALUES (@AtmNo, @BankId," 
                + " @ManifactureDt, @PurchaseDt, @DueServiceDt, @LastServiceDt," 
                + " @PurchaseCost, @MaintenanceCd, @AnnualMaint, @CitOnCall, @CitAnnual)";

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMsCostClass............. " + ex.Message;
                }
        }
    }
}

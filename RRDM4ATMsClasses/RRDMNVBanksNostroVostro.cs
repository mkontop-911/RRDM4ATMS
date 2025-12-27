using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMNVBanksNostroVostro : Logger
    {
        public RRDMNVBanksNostroVostro() : base() { }

        public string BankId;
        public string BankName;
        public string ContactName;
        public string Mobile; 
        public string Email;
        public DateTime DtTmCreated;
        public string Operator;

        // Define the data table 
        public DataTable ExternalBanksDataTable = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // READ BANK 

        public void ReadBank(string InBankId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[NVBanksExternals] "
                   + " WHERE BankId = @BankId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BankId = (string)rdr["BankId"];
                            BankName = (string)rdr["BankName"];

                            ContactName = (string)rdr["ContactName"];
                            Mobile = (string)rdr["Mobile"];
                            Email = (string)rdr["Email"];

                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
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

                    CatchDetails(ex);

                }
        }

       
        //
        // READ Banks AND Fill table 
        //

        public void ReadBanksForDataTable(string InSelectionCriteria )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //InMode = 1 ... is ITMX
            //InMode = 2 ... is Clearing Bank = central Bank

            ExternalBanksDataTable = new DataTable();
            ExternalBanksDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ExternalBanksDataTable.Columns.Add("BankId", typeof(string));
            ExternalBanksDataTable.Columns.Add("BankName", typeof(string));
            ExternalBanksDataTable.Columns.Add("ContactName", typeof(string));
            ExternalBanksDataTable.Columns.Add("DtTmCreated", typeof(DateTime));

            string SqlString = "SELECT *"
                        + " FROM [ATMS].[dbo].[NVBanksExternals] "
                        + InSelectionCriteria;
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Bank Details
                            BankId = (string)rdr["BankId"];
                            BankName = (string)rdr["BankName"];

                            ContactName = (string)rdr["ContactName"];
                            Mobile = (string)rdr["Mobile"];
                            Email = (string)rdr["Email"];

                            DtTmCreated = (DateTime)rdr["DtTmCreated"];
                            Operator = (string)rdr["Operator"];
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = ExternalBanksDataTable.NewRow();

                            RowSelected["BankId"] = BankId;
                            RowSelected["BankName"] = BankName;
                            RowSelected["ContactName"] = ContactName;
                            RowSelected["DtTmCreated"] = DtTmCreated;
                          
                            // ADD ROW
                            ExternalBanksDataTable.Rows.Add(RowSelected);

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
        // GET External BANKS 
        //
        public ArrayList GetExternalBanksNames(string InOperator)
        {
            //USED ONLY TO DEFINE CATEGORIES IT IS NOT USED FOR OTHER 
            ArrayList ExternalBanksIdsList = new ArrayList
            {
                "SelectEntity"
            };

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
         + " FROM [ATMS].[dbo].[NVBanksExternals] "
          + " WHERE Operator = @Operator";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            BankId = (string)rdr["BankId"];
                            BankName = (string)rdr["BankName"];
                        
                            ExternalBanksIdsList.Add(BankId);
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

            return ExternalBanksIdsList;
        }

        // Insert NEW BANK 
        //
        public void InsertExternalBank(string InBankId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVBanksExternals] "
                + " ([BankId],[BankName],"
                + " [ContactName],"
                + " [Mobile], "
                + " [Email], "
                + " [DtTmCreated], "
                + " [Operator]  ) "
                + " VALUES (@BankId,@BankName,"
                + " @ContactName,"
                + " @Mobile,"
                + " @Email,"
                + " @DtTmCreated,"
                + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@BankName", BankName);

                        cmd.Parameters.AddWithValue("@ContactName", ContactName);

                        cmd.Parameters.AddWithValue("@Mobile", Mobile);                   
                        cmd.Parameters.AddWithValue("@Email", Email);

                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

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
        // UPDATE BANK
        // 
        public void UpdateExternalBank(string InBankId)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVBanksExternals] SET "
                            + " BankName = @BankName, "
                            + " ContactName = @ContactName, "
                            + " Mobile = @Mobile,"
                            + " Email = @Email "
                            + " WHERE BankId = @BankId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@BankName", BankName);

                        cmd.Parameters.AddWithValue("@ContactName", ContactName);

                        cmd.Parameters.AddWithValue("@Mobile", Mobile);
                        cmd.Parameters.AddWithValue("@Email", Email);

                       

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();
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
// DELETE BANK
        //
        public void DeleteBankEntry(string InBankId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVBanksExternals] "
                            + " WHERE BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        //rows number of record got updated

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

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ErrorsIdCharacteristics] "
                            + " WHERE BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        //rows number of record got updated

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


    }
}



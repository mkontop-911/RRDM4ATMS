using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMNVCurrentCcyRates : Logger
    {
        public RRDMNVCurrentCcyRates() : base() { }

        public int SeqNo;
        public int RMCycleNo;
        public string Ccy;
        public string CcyName;
        public decimal CcyRate;
        public DateTime UpdatedDateTime;
        public string Operator;

        // Define the data table 
        public DataTable TableCurrentCcyRates = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ NVCurrentCcyRates
        // FILL UP A TABLE
        //
        public void ReadNVCurrentCcyRatesFillTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableCurrentCcyRates = new DataTable();
            TableCurrentCcyRates.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableCurrentCcyRates.Columns.Add("SeqNo", typeof(int));
            TableCurrentCcyRates.Columns.Add("Ccy", typeof(string));
            TableCurrentCcyRates.Columns.Add("CcyName", typeof(string));
            TableCurrentCcyRates.Columns.Add("CcyRate", typeof(decimal));     
            TableCurrentCcyRates.Columns.Add("UpdatedDateTime", typeof(DateTime));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[CurrentCcyRates] "
                    + " WHERE Operator = @Operator "
                    + " ORDER BY Ccy ASC";

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

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            Ccy = (string)rdr["Ccy"];
                            CcyName = (string)rdr["CcyName"];
                            CcyRate = (decimal)rdr["CcyRate"];
                            UpdatedDateTime = (DateTime)rdr["UpdatedDateTime"];
                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableCurrentCcyRates.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Ccy"] = Ccy;
                            RowSelected["CcyName"] = CcyName;
                            RowSelected["CcyRate"] = CcyRate;
                            RowSelected["UpdatedDateTime"] = UpdatedDateTime;
                            // ADD ROW
                            TableCurrentCcyRates.Rows.Add(RowSelected);

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
        // Methods 
        // READ NVCurrentCcyRates SeqNo
        // 
        //
        public void ReadNVCurrentCcyRatesBySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[CurrentCcyRates] "
                      + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            RMCycleNo = (int)rdr["RMCycleNo"];
                            Ccy = (string)rdr["Ccy"];
                            CcyName = (string)rdr["CcyName"];
                            CcyRate = (decimal)rdr["CcyRate"];
                            UpdatedDateTime = (DateTime)rdr["UpdatedDateTime"];
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
        // Methods 
        // READ NVCurrentCcyRates Specific 
        // 
        //
        public void ReadNVCurrentCcyRatesById(string InOperator, string InCcy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[CurrentCcyRates] "
                      + " WHERE Operator = @Operator AND Ccy = @Ccy ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Ccy", InCcy);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            RMCycleNo = (int)rdr["RMCycleNo"];
                            Ccy = (string)rdr["Ccy"];
                            CcyName = (string)rdr["CcyName"];
                            CcyRate = (decimal)rdr["CcyRate"];
                            UpdatedDateTime = (DateTime)rdr["UpdatedDateTime"];
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
        // Insert NewCcyRate
        //
        public int InsertNewCcyRate()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[CurrentCcyRates] "
                + " ([Ccy],"
                + " [RMCycleNo],"
                + " [CcyName],"
                + " [CcyRate],"
                + " [UpdatedDateTime],"
                + " [Operator] )"
                + " VALUES"
                + " (@Ccy,"
                + " @RMCycleNo,"
                + " @CcyName,"
                + " @CcyRate,"
                + " @UpdatedDateTime,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                    
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);
                        cmd.Parameters.AddWithValue("@CcyName", CcyName);
                        cmd.Parameters.AddWithValue("@CcyRate", CcyRate);
                        cmd.Parameters.AddWithValue("@UpdatedDateTime", UpdatedDateTime);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    ErrorFound = true;

                    CatchDetails(ex);
                }

            return SeqNo;
        }
        // 
        // UPDATE Ccy Record 
        // 
        public void UpdateCcyRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[CurrentCcyRates] SET "
                            + " CcyName = @CcyName, CcyRate = @CcyRate ,"
                            +" UpdatedDateTime = @UpdatedDateTime "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@CcyName", CcyName);
                        cmd.Parameters.AddWithValue("@CcyRate", CcyRate);
                        cmd.Parameters.AddWithValue("@UpdatedDateTime", UpdatedDateTime);

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
        // DELETE Ccy
        //
        public void DeleteCcyEntry(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[CurrentCcyRates] "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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

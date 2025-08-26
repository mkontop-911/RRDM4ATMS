using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMNVAlerts : Logger
    {
        public RRDMNVAlerts() : base() { }

        public int SeqNo;
        public string InternalAccNo;
        public string ExternalBankID;
        public string ExternalAccNo;
        public string Ccy;
        public DateTime DateTimeCreated;

        public decimal RangeAmtFrom;
        public decimal RangeAmtTo;
        public int LimitDays;

        public string Operator;

        // Define the data table 
        public DataTable TableAlertsLayers = new DataTable();

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
        // READ NVAlerts
        // FILL UP A TABLE
        //
        public void ReadNVAlertsFillTable(string InOperator, string InExternalBankID, string InExternalAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableAlertsLayers = new DataTable();
            TableAlertsLayers.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableAlertsLayers.Columns.Add("SeqNo", typeof(int));
            TableAlertsLayers.Columns.Add("BankID", typeof(string));
            TableAlertsLayers.Columns.Add("ExternalAccNo", typeof(string));
            TableAlertsLayers.Columns.Add("Ccy", typeof(string));
            TableAlertsLayers.Columns.Add("RangeAmtFrom", typeof(decimal));
            TableAlertsLayers.Columns.Add("RangeAmtTo", typeof(decimal));
            TableAlertsLayers.Columns.Add("LimitDays", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVAlerts] "
                    + " WHERE Operator = @Operator "
                    + " AND ExternalBankID = @ExternalBankID AND ExternalAccNo =@ExternalAccNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];
                            LimitDays = (int)rdr["LimitDays"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableAlertsLayers.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["BankID"] = ExternalBankID;
                            RowSelected["ExternalAccNo"] = ExternalAccNo;
                            RowSelected["Ccy"] = Ccy;

                            RowSelected["RangeAmtFrom"] = RangeAmtFrom;
                            RowSelected["RangeAmtTo"] = RangeAmtTo;
                            RowSelected["LimitDays"] = LimitDays;
                            // ADD ROW
                            TableAlertsLayers.Rows.Add(RowSelected);

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
        // READ NVAlerts SeqNo
        // 
        //
        public void ReadNVAlertBySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
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
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];
                            LimitDays = (int)rdr["LimitDays"];

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
        // READ NVAlerts Selection Creteria
        // 
        //
        public void ReadNVAlertBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
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
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];
                            LimitDays = (int)rdr["LimitDays"];

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
        // READ Alerts BY SELECTION CRITERIA and INSERT NEW BANK ACCOUNT 
        // 
        //
        public void ReadRuleBySelectionCriteriaAndInsert
            (string InSelectionCriteria, string InInternalAccno, string InNewExternalBank, string InNewExternalAcc, string InCcy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
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
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];
                            LimitDays = (int)rdr["LimitDays"];

                            Operator = (string)rdr["Operator"];

                            InternalAccNo = InInternalAccno;
                            ExternalBankID = InNewExternalBank;
                            ExternalAccNo = InNewExternalAcc;
                            Ccy = InCcy;

                            InsertNewAlert(); 

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
        // READ Alerts By Range 
        // 
        //
        public void ReadNVAlertsById(string InOperator,
                         decimal InRangeAmtfrom, decimal InRangeAmtTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
                      + " WHERE Operator = @Operator AND RangeAmtfrom = @RangeAmtfrom AND  RangeAmtTo = @RangeAmtTo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RangeAmtFrom", InRangeAmtfrom);
                        cmd.Parameters.AddWithValue("@RangeAmtTo", InRangeAmtTo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];
                            LimitDays = (int)rdr["LimitDays"];

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
        // Find Days in Range 
        // 
        //
        public void ReadNVFindDaysInRangeInternal(string InOperator,
                         string InInternalAccNo, string InExternalBankID, 
                         decimal InAmt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
                      + " WHERE Operator = @Operator " 
                      + " AND InternalAccNo = @InternalAccNo AND ExternalBankID = @ExternalBankID"
                      + " AND @InAmt > RangeAmtfrom AND  @InAmt <= RangeAmtTo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InInternalAccNo);
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@InAmt", InAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];

                            LimitDays = (int)rdr["LimitDays"];

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
        // Find Days in Range 
        // 
        //
        public void ReadNVFindDaysInRangeExternal(string InOperator,
                         string InExternalAccNo, string InExternalBankID,
                         decimal InAmt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVAlerts] "
                      + " WHERE Operator = @Operator "
                      + " AND ExternalAccNo = @ExternalAccNo AND ExternalBankID = @ExternalBankID"
                      + " AND @InAmt > RangeAmtfrom AND  @InAmt <= RangeAmtTo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@InAmt", InAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            Ccy = (string)rdr["Ccy"];
                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];

                            RangeAmtFrom = (decimal)rdr["RangeAmtFrom"];
                            RangeAmtTo = (decimal)rdr["RangeAmtTo"];

                            LimitDays = (int)rdr["LimitDays"];

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
        // Insert NewAlerts
        //
        public int InsertNewAlert()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVAlerts] "
                  + " ([InternalAccNo],"
                + " [ExternalBankID],"
                + " [ExternalAccNo],"
                + " [Ccy],"
                + " [RangeAmtFrom],"
                + " [RangeAmtTo],"
                + " [LimitDays],"
                + " [Operator] )"
                + " VALUES"
                + " (@InternalAccNo,"
                + " @ExternalBankID,"
                + " @ExternalAccNo,"
                + " @Ccy,"
                + " @RangeAmtFrom,"
                + " @RangeAmtTo,"
                + " @LimitDays,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@ExternalBankID", ExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@RangeAmtFrom", RangeAmtFrom);
                        cmd.Parameters.AddWithValue("@RangeAmtTo", RangeAmtTo);
                        cmd.Parameters.AddWithValue("@LimitDays", LimitDays);
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
        // UPDATE Alerts Record 
        // 
        public void UpdateAlertRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVAlerts] SET "
                            + " RangeAmtFrom = @RangeAmtFrom, RangeAmtTo = @RangeAmtTo ,"
                            + " LimitDays = @LimitDays "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@RangeAmtFrom", RangeAmtFrom);
                        cmd.Parameters.AddWithValue("@RangeAmtTo", RangeAmtTo);
                        cmd.Parameters.AddWithValue("@LimitDays", LimitDays);

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
        // DELETE Alert
        //
        public void DeleteAlertEntry(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVAlerts] "
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

        //
        // DELETE ALerts for Bank / Acc
        //
        public void DeleteALertsEntriesforBankAcc(string InExternalBankID, string InExternalAccNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVAlerts] "
                            + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
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

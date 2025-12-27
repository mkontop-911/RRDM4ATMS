using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMITMXFeesVersions : Logger
    {
        public RRDMITMXFeesVersions() : base() { }

        public int SeqNo;

        public string Product;
        public string VersionId;
        public string Description;
        public string DividingMethod;
        public DateTime FromDate;
        public DateTime ToDate;

        public bool Locked;
        public string UserId;
        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        public int LastSeqNoVersion;

        // Define the data tables 

        public DataTable TableFeesVersions = new DataTable();

        public int TotalSelected;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Methods 
        // READ Fees Versions
        // FILL UP A TABLE
        //
        public void ReadFeesVersionsAndFeelTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFeesVersions = new DataTable();
            TableFeesVersions.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableFeesVersions.Columns.Add("SeqNo", typeof(int));
            TableFeesVersions.Columns.Add("Product", typeof(string));
            TableFeesVersions.Columns.Add("VersionId", typeof(string));
            TableFeesVersions.Columns.Add("Description", typeof(string));
            TableFeesVersions.Columns.Add("FromDate", typeof(DateTime));
            TableFeesVersions.Columns.Add("ToDate", typeof(DateTime));
            TableFeesVersions.Columns.Add("Locked", typeof(bool));
            TableFeesVersions.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ITMXFeesVersions] "
                   + InSelectionCriteria
                   + " ORDER BY SeqNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            Product = (string)rdr["Product"];
                            VersionId = (string)rdr["VersionId"];
                            Description = (string)rdr["Description"];
                            DividingMethod = (string)rdr["DividingMethod"];
                            FromDate = (DateTime)rdr["FromDate"];
                            ToDate = (DateTime)rdr["ToDate"];
                            Locked = (bool)rdr["Locked"];
                            UserId = (string)rdr["UserId"];
                            Operator = (string)rdr["Operator"];



                            //FILL IN TABLE
                            DataRow RowSelected = TableFeesVersions.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["Product"] = Product;
                            RowSelected["VersionId"] = VersionId;
                            RowSelected["Description"] = Description;

                            RowSelected["FromDate"] = FromDate;
                            RowSelected["ToDate"] = ToDate;
                            RowSelected["Locked"] = Locked;

                            RowSelected["UserId"] = UserId;

                            // ADD ROW
                            TableFeesVersions.Rows.Add(RowSelected);
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
        // READ FeesVersion  by Seq no  
        // 
        //
        public void ReadFeesVersionbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesVersions] "
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
                            Product = (string)rdr["Product"];
                            VersionId = (string)rdr["VersionId"];
                            Description = (string)rdr["Description"];
                            DividingMethod = (string)rdr["DividingMethod"];
                            FromDate = (DateTime)rdr["FromDate"];
                            ToDate = (DateTime)rdr["ToDate"];
                            Locked = (bool)rdr["Locked"];
                            UserId = (string)rdr["UserId"];
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
        // Find Last ToDate 
        // 
        //
        public void ReadFeesVersionToFindLastSeqAndToDate(string InOperator, string InProduct)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesVersions] "
                    + " WHERE Operator = @Operator AND Product = @Product";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Product", InProduct);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            Product = (string)rdr["Product"];
                            VersionId = (string)rdr["VersionId"];
                            Description = (string)rdr["Description"];
                            DividingMethod = (string)rdr["DividingMethod"];
                            FromDate = (DateTime)rdr["FromDate"];
                            ToDate = (DateTime)rdr["ToDate"];
                            Locked = (bool)rdr["Locked"];
                            UserId = (string)rdr["UserId"];
                            Operator = (string)rdr["Operator"];

                            LastSeqNoVersion = SeqNo;
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
        // Find Position In Grid
        // 
        //
        public int PositionInGrid;
        public void ReadFeesVersionToFindPositionOfSeqNo(string InOperator,string InProduct , int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            PositionInGrid = -1;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesVersions] "
                    + " WHERE Operator = @Operator AND Product = @Product";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Product", InProduct);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            Product = (string)rdr["Product"];
                            PositionInGrid = PositionInGrid + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            if (SeqNo == InSeqNo)
                            {
                                break;
                            }

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
        // Find Position In Grid
        // 
        //
        public bool CurrentVersionFound; 
        public void ReadFeesVersionToFindPositionOfCurrentVersion(string InOperator, string InProduct)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CurrentVersionFound = false; 
            PositionInGrid = -1;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesVersions] "
                    + " WHERE Operator = @Operator AND Product = @Product";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Product", InProduct);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Product = (string)rdr["Product"];
                            VersionId = (string)rdr["VersionId"];
                            Description = (string)rdr["Description"];
                            DividingMethod = (string)rdr["DividingMethod"];
                            FromDate = (DateTime)rdr["FromDate"];
                            ToDate = (DateTime)rdr["ToDate"];
                            Locked = (bool)rdr["Locked"];
                            UserId = (string)rdr["UserId"];
                            Operator = (string)rdr["Operator"];

                            PositionInGrid = PositionInGrid + 1;

                            if (BetweenWithEqualBoarders(DateTime.Now.Date, FromDate.Date, ToDate.Date) == true)
                            {
                                CurrentVersionFound = true;
                                break;
                            }
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

        public static bool BetweenWithEqualBoarders(DateTime input, DateTime date1, DateTime date2)
        {
            return (input >= date1 && input <= date2);
        }
        // Insert FeesVersion
        //
        public int InsertFeesVersion()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ITMXFeesVersions] "
                    + "([Product], [VersionId], [Description], [DividingMethod], "
                    + " [FromDate], [ToDate],"
                    + " [Locked], [UserId], "
                    + " [Operator] )"
                    + " VALUES (@Product, @VersionId, @Description, @DividingMethod,"
                    + " @FromDate, @ToDate,"
                    + " @Locked, @UserId, "
                    + " @Operator )"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@VersionId", VersionId);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DividingMethod", DividingMethod);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);

                        cmd.Parameters.AddWithValue("@Locked", Locked);
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();
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
            return SeqNo;
        }

        // UPDATE FeesVersion
        // 
        public void UpdateFeesVersion(int InSeqNo)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ITMXFeesVersions] SET "
                            + " Product = @Product, VersionId = @VersionId,"
                            + " Description = @Description, DividingMethod = @DividingMethod, "
                            + " FromDate = @FromDate, ToDate = @ToDate, "
                            + " Locked = @Locked, UserId = @UserId "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@VersionId", VersionId);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DividingMethod", DividingMethod);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);

                        cmd.Parameters.AddWithValue("@Locked", Locked);
                        cmd.Parameters.AddWithValue("@UserId", UserId);

                       
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
        // DELETE Fees Version
        //
        public void DeleteFeesVersion(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ITMXFeesVersions] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        
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



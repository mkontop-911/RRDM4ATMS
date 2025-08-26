using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMITMXSettlementCycles : Logger
    {
        public RRDMITMXSettlementCycles() : base() { }

        public int ITMXSettlementCycle;
        public string JobCategory; // An Installation can start one category for matcing and then the next 
        public DateTime CreatedDate;
        public string Description;

        public int LastProcessedRecord;
        public bool ActionByUser;
        public bool ActionByAuth;
        public string SettlementUser;
        public string AuthoriserUser;
        public DateTime AuthDateTm;
        public string Operator;

        // Define the data table 
        public DataTable TableITMXDailyJobCycles = new DataTable();

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
        // READ ITMXDailyJobCycles
        // FILL UP A TABLE
        //
        public void ReadITMXSettlementCyclesFillTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableITMXDailyJobCycles = new DataTable();
            TableITMXDailyJobCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableITMXDailyJobCycles.Columns.Add("SettlementCycle", typeof(int));
            TableITMXDailyJobCycles.Columns.Add("Category", typeof(string));

            TableITMXDailyJobCycles.Columns.Add("DateTime", typeof(DateTime));
            TableITMXDailyJobCycles.Columns.Add("Description", typeof(string));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXSettlementCycles] "
                    + " WHERE Operator = @Operator "
                    + " ORDER BY ITMXSettlementCycle DESC";

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

                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            JobCategory = (string)rdr["JobCategory"];
                            CreatedDate = (DateTime)rdr["CreatedDate"];
                            Description = (string)rdr["Description"];

                            LastProcessedRecord = (int)rdr["LastProcessedRecord"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            ActionByAuth = (bool)rdr["ActionByAuth"];

                            SettlementUser = (string)rdr["SettlementUser"];
                            AuthoriserUser = (string)rdr["AuthoriserUser"];

                            AuthDateTm = (DateTime)rdr["AuthDateTm"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableITMXDailyJobCycles.NewRow();

                            RowSelected["SettlementCycle"] = ITMXSettlementCycle;
                            RowSelected["Category"] = JobCategory;
                            RowSelected["DateTime"] = CreatedDate;
                            RowSelected["Description"] = Description;
                            // ADD ROW
                            TableITMXDailyJobCycles.Rows.Add(RowSelected);

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
        // READ ITMXDailyJobCycles
        // FILL UP A TABLE
        //
        public void ReadITMXDailySettlementToFindTotals(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementCycles] "
                      + " WHERE Operator = @Operator  "
                      + " ";

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

                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            JobCategory = (string)rdr["JobCategory"];
                            CreatedDate = (DateTime)rdr["CreatedDate"];
                            Description = (string)rdr["Description"];

                            LastProcessedRecord = (int)rdr["LastProcessedRecord"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            ActionByAuth = (bool)rdr["ActionByAuth"];

                            SettlementUser = (string)rdr["SettlementUser"];
                            AuthoriserUser = (string)rdr["AuthoriserUser"];

                            AuthDateTm = (DateTime)rdr["AuthDateTm"];

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
        // READ ReconcCategoriesMatchingSessions Specific 
        // 
        //
        public void ReadITMXSettlementCyclesById(string InOperator, int InITMXSettlementCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ITMXSettlementCycles] "
                      + " WHERE Operator = @Operator AND ITMXSettlementCycle = @ITMXSettlementCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ITMXSettlementCycle = (int)rdr["ITMXSettlementCycle"];
                            JobCategory = (string)rdr["JobCategory"];
                            CreatedDate = (DateTime)rdr["CreatedDate"];
                            Description = (string)rdr["Description"];

                            LastProcessedRecord = (int)rdr["LastProcessedRecord"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            ActionByAuth = (bool)rdr["ActionByAuth"];

                            SettlementUser = (string)rdr["SettlementUser"];
                            AuthoriserUser = (string)rdr["AuthoriserUser"];

                            AuthDateTm = (DateTime)rdr["AuthDateTm"];

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


        // UPDATE Settlement Cycle 
        // 
        public void UpdateSettlementCycle(int InITMXSettlementCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ITMXSettlementCycles]  SET "
                             + " LastProcessedRecord = @LastProcessedRecord,"
                             + " ActionByUser = @ActionByUser,"
                             + " ActionByAuth = @ActionByAuth,"
                             + " SettlementUser = @SettlementUser, "
                             + " AuthoriserUser = @AuthoriserUser,"
                             + " AuthDateTm = @AuthDateTm "
                             + " WHERE ITMXSettlementCycle = @ITMXSettlementCycle", conn))
                    {
                        cmd.Parameters.AddWithValue("@ITMXSettlementCycle", InITMXSettlementCycle);
                        cmd.Parameters.AddWithValue("@LastProcessedRecord", LastProcessedRecord);
                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                        cmd.Parameters.AddWithValue("@ActionByAuth", ActionByAuth);
                        cmd.Parameters.AddWithValue("@SettlementUser", SettlementUser);
                        cmd.Parameters.AddWithValue("@AuthoriserUser", AuthoriserUser);
                        cmd.Parameters.AddWithValue("@AuthDateTm", AuthDateTm);

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

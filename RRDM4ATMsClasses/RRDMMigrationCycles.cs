using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMMigrationCycles : Logger
    {
        public RRDMMigrationCycles() : base() { }


        public int SeqNo;
        
        public DateTime StartDateTm;
        public DateTime FinishDateTm;

        public string Description;

        public int InvalidInExcelRecords;
        public int OpenedNewAtms;
        public int UpdatedATMs;

        public string ExcelId;

        public int ProcessStage; // 0: Excel Not read 1 : Excel Read, 2: Excel Updated

        public string UserId; 

        public string Operator;

        // Define the data table 
        public DataTable TableMigrationCycles = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
          
            StartDateTm = (DateTime)rdr["StartDateTm"];
            FinishDateTm = (DateTime)rdr["FinishDateTm"];

            Description = (string)rdr["Description"];

            InvalidInExcelRecords = (int)rdr["InvalidInExcelRecords"];
            OpenedNewAtms = (int)rdr["OpenedNewAtms"];
            UpdatedATMs = (int)rdr["UpdatedATMs"];

            ExcelId = (string)rdr["ExcelId"];

            ProcessStage = (int)rdr["ProcessStage"];

            UserId = (string)rdr["UserId"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ITMXMigrationCycles
        // FILL UP A TABLE
        //
        public void ReadMigrationCyclesFillTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMigrationCycles = new DataTable();
            TableMigrationCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableMigrationCycles.Columns.Add("Cycle", typeof(int));
           
            TableMigrationCycles.Columns.Add("StartDateTm", typeof(string));
            TableMigrationCycles.Columns.Add("FinishDateTm", typeof(string));
            
            TableMigrationCycles.Columns.Add("Description", typeof(string));

            TableMigrationCycles.Columns.Add("ExcelErrors", typeof(int));
            TableMigrationCycles.Columns.Add("NewAtms", typeof(int));
            TableMigrationCycles.Columns.Add("UpdatedATMs", typeof(int));

            TableMigrationCycles.Columns.Add("Status", typeof(string));

            TableMigrationCycles.Columns.Add("UserId", typeof(string));

            TableMigrationCycles.Columns.Add("ExcelId", typeof(string));

         
            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MigrationCycles] "
                    + " WHERE Operator = @Operator "
                    + " ORDER BY SeqNo DESC";

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

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableMigrationCycles.NewRow();

                            RowSelected["Cycle"] = SeqNo;
                           
                            RowSelected["StartDateTm"] = StartDateTm.ToString();
                            if (FinishDateTm.Date == NullPastDate.Date)
                            {
                                RowSelected["FinishDateTm"] = "Not Ended yet";
                            }
                            else
                            {
                                RowSelected["FinishDateTm"] = FinishDateTm.ToString();
                            }
                            
                            RowSelected["Description"] = Description;

                            RowSelected["ExcelErrors"] = InvalidInExcelRecords;
                            RowSelected["NewAtms"] = OpenedNewAtms;
                            RowSelected["UpdatedATMs"] = UpdatedATMs;

                            if (ProcessStage == 0)
                            {
                                RowSelected["Status"] = "Cycle Not Started";
                            }

                            if (ProcessStage == 1)
                            {
                                RowSelected["Status"] = "Excel Read";
                            }
                            if (ProcessStage == 2)
                            {
                                RowSelected["Status"] = "Migration Completed";
                            }

                            RowSelected["UserId"] = UserId;

                            RowSelected["ExcelId"] = ExcelId;

                            // ADD ROW
                            TableMigrationCycles.Rows.Add(RowSelected);

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
        // READ MigrationCycles to find the latest one 
        // 
        //
        public void ReadLastReconcMigrationCycle(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[MigrationCycles] "
                      + " WHERE Operator = @Operator"
                      + " ORDER BY SeqNo DESC";

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

                            ReadTableFields(rdr);

                            break;

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
        // READ MigrationCycles
        // 
        //
        public void ReadMigrationCyclesByOperator(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[MigrationCycles] "
                      + " WHERE Operator = @Operator "
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

                            ReadTableFields(rdr);

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
        // READ  
        // 
        //
        public void ReadMigrationCyclesById(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[MigrationCycles] "
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

                            ReadTableFields(rdr);
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
        // READ  by Selection Criteria 
        // 
        //
        //public void ReadMigrationCyclesBySelectionCriteria(string InSelectionCriteria)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    SqlString = "SELECT *"
        //              + " FROM [ATMS].[dbo].[MigrationCycles] "
        //              + InSelectionCriteria ;

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                //cmd.Parameters.AddWithValue("@Operator", InOperator);
        //                //cmd.Parameters.AddWithValue("@MigrationCycle", InMigrationCycle);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    TotalSelected = TotalSelected + 1;

        //                    ReadTableFields(rdr);
        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);

        //        }
        //}

        // Insert NewMigrationCycle
        public int InsertNewMigrationCycle()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MigrationCycles] "
                + " ( "
                + " [StartDateTm],"
                + " [FinishDateTm]," 
                + " [ProcessStage],"
                + " [UserId],"
                + " [Operator] )"
                + " VALUES"
                + " ( "
                + " @StartDateTm,"
                + " @FinishDateTm,"
                + " @ProcessStage,"
                + " @UserId,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            //+ " SELECT MsgID  = CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDateTm", StartDateTm);
                        cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);
                      
                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
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

                    CatchDetails(ex); 
                }

            return SeqNo;
        }

        // UPDATE  MigrationCycle
        // 
        public void UpdateMigrationCycle(int InSeqNo)
        {
            //int rows; 
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MigrationCycles] SET "
                            + " FinishDateTm = @FinishDateTm, "
                            + " Description = @Description, "
                            + " InvalidInExcelRecords = @InvalidInExcelRecords,  "
                            + " OpenedNewAtms = @OpenedNewAtms,  "
                            + " UpdatedATMs = @UpdatedATMs,  "
                            + " ExcelId = @ExcelId,  "
                            + " ProcessStage = @ProcessStage,  "
                            + " UserId = @UserId  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);

                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@InvalidInExcelRecords", InvalidInExcelRecords);
                        cmd.Parameters.AddWithValue("@OpenedNewAtms", OpenedNewAtms);
                        cmd.Parameters.AddWithValue("@UpdatedATMs", UpdatedATMs);
                        cmd.Parameters.AddWithValue("@ExcelId", ExcelId);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);
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

        

    }
}

using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_Cit_ExcelOutputCycles : Logger
    {
        public RRDM_Cit_ExcelOutputCycles() : base() { }

        public int SeqNo;

        public string CitId;
        public string Description;

        public string OrdersFunction;
               // It has two Values
               // "ATMsinNeed" and
               // "PrepareMoneyIn"
     
        public DateTime CreatedDateTm;

        public string ExcelIdAndLocation;
        public int ExcelRecords;
        public decimal ExcelAmount;

        public bool SendByEmail;
        public DateTime SendDateTm;

        public string MakerId;
        public DateTime AuthorisedDateTm;
        public string AuthoriserId;

        public int ProcessStage; // 1 at Makers
                                 // 2 at Authoriser
                                 // 3 Completed 
        public string Operator;

        
        // Define the data table 
        public DataTable TableExcelOutputCycles = new DataTable();

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

            CitId = (string)rdr["CitId"];
            Description = (string)rdr["Description"];

            OrdersFunction = (string)rdr["OrdersFunction"];
           
            CreatedDateTm = (DateTime)rdr["CreatedDateTm"];

            ExcelIdAndLocation = (string)rdr["ExcelIdAndLocation"];
            ExcelRecords = (int)rdr["ExcelRecords"];
            ExcelAmount = (decimal)rdr["ExcelAmount"];

            SendByEmail = (bool)rdr["SendByEmail"];
            SendDateTm = (DateTime)rdr["SendDateTm"];

            MakerId = (string)rdr["MakerId"];
            AuthorisedDateTm = (DateTime)rdr["AuthorisedDateTm"];
            AuthoriserId = (string)rdr["AuthoriserId"];

            ProcessStage = (int)rdr["ProcessStage"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ExcelOutputCycles
        // FILL UP A TABLE
        //
        public void ReadExcelOutputCyclesFillTable(string InOperator, string InSignedId ,string InCitId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableExcelOutputCycles = new DataTable();
            TableExcelOutputCycles.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                    + " WHERE Operator = @Operator AND CitId = @CitId "
                    + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                       
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CitId", InCitId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableExcelOutputCycles);

                        // Close conn
                        conn.Close();

                        InsertReport(InOperator, InSignedId, TableExcelOutputCycles);

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        // Insert 
        public void InsertReport(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                //Clear REPORT Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport79();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport79]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }
        //
        // Methods 
        // READ  
        // 
        //
        public void ReadExcelOutputCyclesBySelectionCriteria(string  InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                      + InSelectionCriteria;  

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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
        // READ  
        // 
        //
        public void ReadExcelOutputCyclesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                      + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                
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
        // READ  
        // 
        //
        public void ReadExcelOutputCyclesByCutOffDate(DateTime InCreatedDateTm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                      + " WHERE CreatedDateTm = @CreatedDateTm ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CreatedDateTm", InCreatedDateTm);

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
        // UPDATE Cit Out Cycles
        //
        public void Update_Cit_ExcelOutputCycles(string InCitId, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE Cit_ExcelOutputCycles SET "
                            + "[Description]=@Description,"
                            + "[OrdersFunction]=@OrdersFunction,"
                            + "[CreatedDateTm] =@CreatedDateTm,"
                            + "[ExcelIdAndLocation] = @ExcelIdAndLocation,"
                            + " [ExcelRecords] = @ExcelRecords, "
                            + " [ExcelAmount] = @ExcelAmount, "
                            + " [SendByEmail] = @SendByEmail,"
                            + " [SendDateTm] = @SendDateTm,"
                            + " [MakerId] = @MakerId,"
                            + "[AuthorisedDateTm] = @AuthorisedDateTm,"
                            + " [AuthoriserId] = @AuthoriserId,"
                            + " [ProcessStage] = @ProcessStage "
                            + " WHERE CitId= @CitId AND SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@OrdersFunction", OrdersFunction);

                        cmd.Parameters.AddWithValue("@CreatedDateTm", CreatedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);

                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);

                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);

                        cmd.Parameters.AddWithValue("@SendDateTm", SendDateTm);

                        cmd.Parameters.AddWithValue("@MakerId", MakerId);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);
                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //    outcome = " ATMs Table UPDATED ";
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
        // Insert ExcelOutputCycle
        public int InsertExcelOutputCycle()
        {
            
            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Cit_ExcelOutputCycles] "
                + " ( "
                + " [CitId],"
                + " [Description],"
                + " [OrdersFunction],"
                + " [CreatedDateTm],"               
                + " [MakerId],"
                + " [ProcessStage],"
                + " [Operator] )"
                + " VALUES"
                + " ( "
                + " @CitId,"
                + " @Description,"
                + " @OrdersFunction,"
                + " @CreatedDateTm,"          
                + " @MakerId,"
                + " @ProcessStage,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            //+ " SELECT MsgID  = CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Description", Description);

                        cmd.Parameters.AddWithValue("@OrdersFunction", OrdersFunction);

                        cmd.Parameters.AddWithValue("@CreatedDateTm", CreatedDateTm);
                  
                        cmd.Parameters.AddWithValue("@MakerId", MakerId);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);
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
        public void UpdateLoadExcelCycle(int InSeqNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelOutputCycles] SET "
                            + " AuthorisedDateTm = @AuthorisedDateTm "
                            + " , ExcelRecords = @ExcelRecords "
                            + " ,InExcelRecords = @InExcelRecords  "

                            + " ,SendByEmail = @SendByEmail  "
                            + " ,NotInG4S = @NotInG4S  "
                            + " ,PresenterNumberEqual = @PresenterNumberEqual  "
                            + " ,PresenterDiff = @PresenterDiff  "
                            + " ,ShortFound = @ShortFound  "

                            + " ,ExcelAmount = @ExcelAmount  "
                            + " ,ShortAmt = @ShortAmt  "

                            + " ,ExcelIdAndLocation = @ExcelIdAndLocation  "
                            + " ,ProcessStage = @ProcessStage "
                            + " ,IsReversed = @IsReversed  "
                            + " ,MakerId = @MakerId  "
                            + " ,AuthoriserId = @AuthoriserId  "

                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);

                      

                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);
                       
                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);
                       

                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);

                        cmd.Parameters.AddWithValue("@MakerId", MakerId);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);

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

        // UPDATE  MigrationCycle by CutOff
        // 
        public void UpdateLoadExcelCycleByCutOff(DateTime InCreatedDateTm)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelOutputCycles] SET "
                            + " AuthorisedDateTm = @AuthorisedDateTm "
                            + " , ExcelRecords = @ExcelRecords "
                            + " ,InExcelRecords = @InExcelRecords  "

                            + " ,SendByEmail = @SendByEmail  "
                            + " ,NotInG4S = @NotInG4S  "
                            + " ,PresenterNumberEqual = @PresenterNumberEqual  "
                            + " ,PresenterDiff = @PresenterDiff  "
                            + " ,ShortFound = @ShortFound  "

                            + " ,ExcelAmount = @ExcelAmount  "
                            + " ,ShortAmt = @ShortAmt  "

                            + " ,ExcelIdAndLocation = @ExcelIdAndLocation  "
                            + " ,ProcessStage = @ProcessStage "
                            + " ,IsReversed = @IsReversed  "
                            + " ,MakerId = @MakerId  "
                            + " ,AuthoriserId = @AuthoriserId  "

                            + " WHERE CreatedDateTm = @CreatedDateTm", conn))
                    {
                        cmd.Parameters.AddWithValue("@CreatedDateTm", InCreatedDateTm);

                        cmd.Parameters.AddWithValue("@AuthorisedDateTm", AuthorisedDateTm);

                        cmd.Parameters.AddWithValue("@ExcelRecords", ExcelRecords);

                      
                        cmd.Parameters.AddWithValue("@SendByEmail", SendByEmail);
                      
                        cmd.Parameters.AddWithValue("@ExcelAmount", ExcelAmount);
                       

                        cmd.Parameters.AddWithValue("@ExcelIdAndLocation", ExcelIdAndLocation);

                      
                        cmd.Parameters.AddWithValue("@MakerId", MakerId);
                        cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);

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

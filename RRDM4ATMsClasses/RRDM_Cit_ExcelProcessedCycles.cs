using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_Cit_ExcelProcessedCycles : Logger
    {
        public RRDM_Cit_ExcelProcessedCycles() : base() { }



        public int SeqNo;

        public string CitId;

        public DateTime StartDateTm;
        public DateTime FinishDateTm;

        public int ProcessStage;// 
                                // 1 : Process at starting stage 
                                // 2 : " at Validation Stage";
                                // 3 : "Excel entries cycle  Updated

        public string UserId; 

        public string AuthoriserId;

        public int RMCycle; 

        public string Operator;

        // Define the data table 
        public DataTable TableExcelLoadCycles = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CitId = (string)rdr["CitId"];


            StartDateTm = (DateTime)rdr["StartDateTm"];
            FinishDateTm = (DateTime)rdr["FinishDateTm"];

            ProcessStage = (int)rdr["ProcessStage"];


            UserId = (string)rdr["UserId"];

            AuthoriserId = (string)rdr["AuthoriserId"];

            RMCycle = (int)rdr["RMCycle"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ExcelLoadCycles
        // FILL UP A TABLE
        //
        public void ReadExcelLoadCyclesFillTable(string InOperator, string InSignedId ,string InCitId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //Clear REPORT Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport71(InSignedId);

            TableExcelLoadCycles = new DataTable();
            TableExcelLoadCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableExcelLoadCycles.Columns.Add("LoadingCycle", typeof(int));


            TableExcelLoadCycles.Columns.Add("StartDateTm", typeof(string));
            TableExcelLoadCycles.Columns.Add("FinishDateTm", typeof(string));
            


            TableExcelLoadCycles.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
                    + " WHERE Operator = @Operator AND CitId = @CitId "
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
                        cmd.Parameters.AddWithValue("@CitId", InCitId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableExcelLoadCycles.NewRow();

                            RowSelected["LoadingCycle"] = SeqNo;
                         
                            RowSelected["StartDateTm"] = StartDateTm.ToString();

                            if (FinishDateTm.Date == NullPastDate.Date)
                            {
                                RowSelected["FinishDateTm"] = "Not Ended yet";
                            }
                            else
                            {
                                RowSelected["FinishDateTm"] = FinishDateTm.ToString();
                            }
                          
                            RowSelected["UserId"] = InSignedId;

                            // ADD ROW
                            TableExcelLoadCycles.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertReport(InOperator, InSignedId, TableExcelLoadCycles); 
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Methods 
        // READ ExcelLoadCycles
        // FILL UP A TABLE
        //
        public void DeleteAuthorisationsDuringUndo(int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableExcelLoadCycles = new DataTable();
            TableExcelLoadCycles.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
                    + " WHERE RMCycle = @RMCycle "
                    + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableExcelLoadCycles);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            int I = 0; 

            while (I <= (TableExcelLoadCycles.Rows.Count - 1))
            {

                //DELETE AUTHORISATION  RECORDs 
                int WSeqNo = (int)TableExcelLoadCycles.Rows[I]["SeqNo"];

                int WRMCycle = (int)TableExcelLoadCycles.Rows[I]["RMCycle"];

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.DeleteAuthorisationRecord_By_RMCycle(WSeqNo); 

                I++; // Read Next entry of the table 

            }


        }
        // 
        public void ReadExcelLoadCyclesFillTable_Feeding(string InOperator, string InSignedId, string InCitId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //Clear REPORT Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport71(InSignedId);

            TableExcelLoadCycles = new DataTable();
            TableExcelLoadCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            
            TableExcelLoadCycles.Columns.Add("SeqNo", typeof(int));

            TableExcelLoadCycles.Columns.Add("RMCycle", typeof(int));

            TableExcelLoadCycles.Columns.Add("StartDateTm", typeof(string));
            TableExcelLoadCycles.Columns.Add("FinishDateTm", typeof(string));


            TableExcelLoadCycles.Columns.Add("MakerId", typeof(string));

            TableExcelLoadCycles.Columns.Add("AuthoriserId", typeof(string));



            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
                    + " WHERE Operator = @Operator AND CitId = @CitId "
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
                        cmd.Parameters.AddWithValue("@CitId", InCitId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableExcelLoadCycles.NewRow();

                            //TableExcelLoadCycles.Columns.Add("SeqNo", typeof(int));

                            //TableExcelLoadCycles.Columns.Add("RMCycle", typeof(int));


                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["RMCycle"] = RMCycle;


                            RowSelected["StartDateTm"] = StartDateTm.ToString();

                            if (FinishDateTm.Date == NullPastDate.Date)
                            {
                                RowSelected["FinishDateTm"] = "Not Ended yet";
                            }
                            else
                            {
                                RowSelected["FinishDateTm"] = FinishDateTm.ToString();
                            }

                            //if (IsReversed == true)
                            //{
                            //    RowSelected["Status"] = "Excel is Reversed";
                            //}
                            RowSelected["MakerId"] = UserId; // this the maker
                            RowSelected["AuthoriserId"] = AuthoriserId; // AuthoriserId

                            // ADD ROW
                            TableExcelLoadCycles.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                  // InsertReport(InOperator, InSignedId, TableExcelLoadCycles);
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
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport71]";

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
        public void ReadExcelLoadCyclesBySelectionCriteria(string  InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
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
        public void ReadExcelLoadCyclesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
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


        // Insert ExcelLoadCycle
        public int InsertExcelLoadCycle()
        {
            
            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Cit_ExcelProcessCycles] "
                + " ( "
                + " [CitId],"
                + " [StartDateTm],"
            
                + " [ProcessStage],"
                + " [UserId], "
                + " [RMCycle],"
                + " [Operator] )"
                + " VALUES"
                + " ( "
                + " @CitId,"

                + " @StartDateTm,"

                + " @ProcessStage,"
                + " @UserId,"
                + " @RMCycle,"
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



                        cmd.Parameters.AddWithValue("@StartDateTm", StartDateTm);



                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@RMCycle",RMCycle);
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

        // UPDATE  Loaded Cycle
        // 
        public void UpdateLoadExcelCycleAtLoadingStep(int InSeqNo)
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelProcessCycles] SET "

                            + " ProcessStage = @ProcessStage  "

                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

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


        // UPDATE  Load Cycle
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
                        new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelProcessCycles] SET "

                            + " FinishDateTm = @FinishDateTm "

                            + " ,ProcessStage = @ProcessStage "

                            + " ,UserId = @UserId  "
                            + " ,AuthoriserId = @AuthoriserId  "

                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);

                        cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
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

        //// UPDATE  MigrationCycle by CutOff
        //// 
        //public void UpdateLoadExcelCycleByCutOff(DateTime InCut_Off_Date)
        //{

        //    //int rows; 
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE [ATMS].[dbo].[Cit_ExcelInputCycles] SET "
        //                    + " FinishDateTm = @FinishDateTm "
        //                    + " , ValidInExcelRecords = @ValidInExcelRecords "
        //                    + " ,InvalidInExcelRecords = @InvalidInExcelRecords  "

        //                    + " ,NotInBank = @NotInBank  "
        //                    + " ,NotInG4S = @NotInG4S  "
        //                    + " ,PresenterNumberEqual = @PresenterNumberEqual  "
        //                    + " ,PresenterDiff = @PresenterDiff  "
        //                    + " ,ShortFound = @ShortFound  "

        //                    + " ,PresenterDiffAmt = @PresenterDiffAmt  "
        //                    + " ,ShortAmt = @ShortAmt  "

        //                    + " ,ExcelId = @ExcelId  "
        //                    + " ,ProcessStage = @ProcessStage "
        //                    + " ,IsReversed = @IsReversed  "
        //                    + " ,UserId = @UserId  "
        //                    + " ,AuthoriserId = @AuthoriserId  "

        //                    + " WHERE Cut_Off_Date = @Cut_Off_Date", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

        //                cmd.Parameters.AddWithValue("@FinishDateTm", FinishDateTm);

        //                cmd.Parameters.AddWithValue("@ValidInExcelRecords", ValidInExcelRecords);

        //                cmd.Parameters.AddWithValue("@InvalidInExcelRecords", InvalidInExcelRecords);

        //                cmd.Parameters.AddWithValue("@NotInBank", NotInBank);
        //                cmd.Parameters.AddWithValue("@NotInG4S", NotInG4S);
        //                cmd.Parameters.AddWithValue("@PresenterNumberEqual", PresenterNumberEqual);
        //                cmd.Parameters.AddWithValue("@PresenterDiff", PresenterDiff);
        //                cmd.Parameters.AddWithValue("@ShortFound", ShortFound);

        //                cmd.Parameters.AddWithValue("@PresenterDiffAmt", PresenterDiffAmt);
        //                cmd.Parameters.AddWithValue("@ShortAmt", ShortAmt);

        //                cmd.Parameters.AddWithValue("@ExcelId", ExcelId);

        //                cmd.Parameters.AddWithValue("@ProcessStage", ProcessStage);
        //                cmd.Parameters.AddWithValue("@IsReversed", IsReversed);
        //                cmd.Parameters.AddWithValue("@UserId", UserId);
        //                cmd.Parameters.AddWithValue("@AuthoriserId", AuthoriserId);

        //                cmd.ExecuteNonQuery();

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



        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }
            // Environment.Exit(0);
        }
    }
}



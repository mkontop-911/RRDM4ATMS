using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingCategoriesVsSourcesFiles : Logger
    {
        public RRDMMatchingCategoriesVsSourcesFiles() : base() { }
        //
        // RECORD FIELDS
        //
        public int SeqNo;

        public string CategoryId;
        public string SourceFileName;
        public bool PrimaryFile;
        public bool IsTargetSystem; // Like VISA AUTHORISATIONS 

        public DateTime LastInFileDtTm;
        public DateTime LastMatchingDtTm;
       
        public int RMCycle;
        public DateTime ExpectedDate;

        public bool IsReadThisCycle; 

        public int ProcessMode;
        //
        //                  if (ProcessMode == -2)
        //                  {
        //                      WProcessModeInWords = "Under Loading process";
        //                  }
        //                  if (ProcessMode == -1) => WHEN FILE HAS BEEN LOADED FILES to CATEGORIES  COMBINATIONS ARE UPDATED WITH THIS 
        //                  {
        //                      WProcessModeInWords = "Ready For Match"; // FILE JUST LOADED AND IT IS READY
        //                  }
        //                  if (ProcessMode == 0)
        //                  {
        //                      WProcessModeInWords = "Under Matching Process"; // Manching has started
        //                  }
        //                  if (ProcessMode == 1)
        //                  {
        //                      WProcessModeInWords = "Matching Finished"; // ONCE MATCHING CATEGORY FINISHES ALL CATEGORIES TO FILES COMBINATIONS are turned to 1 
        //                  }
        //
        // A FILE IS LOADED ONLY IF PROCESS MODE = 1 for All Occurances in all Matching Categories
        // OR if 
        // PROCESS MODE = -1 for All Occurances in all Matching Categories
        //
        // Before starting file loading then you turn Process Mode = -2 = Under Matching Process for ALL
        //
        // After loading you turn it to -1

        // Eg: Mode of operation
        // In the night matching is done and Process mode has been turn to 1
        // During the day a number of files or versions of the same file reach RRDM
        // Therefore Process Mode can be 1 or -1 
        // It can also be -2 at the time of Loading
        // At night RRDM finds all Process mode = -1 , it turns it to 0 while Matching and to 1 after finish.
        // Matching is done for each category separately. 
        // During matching is turn to 0 only for this category and after finishing to 1 only for this category. 
        //
        // WORKING FIELDS 
        //

        public string SourceFileNameA;
        public string SourceFileNameB;
        public string SourceFileNameC;
        public string SourceFileNameD;
        public string SourceFileNameE;

        //string OutReturnedMessage ;
        //int OutReturnedValue ;

        bool KontoAvailable; 

        public int TotalRecords;

        public string WProcessModeInWords;

        public int TotalMinusOne = 0;
        public int TotalZero = 0;
        public int TotalOne = 0;

        // Define the data table 
        public DataTable RMCategoryFilesDataFiles = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Reader Fields 
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];
            SourceFileName = (string)rdr["SourceFileName"];
            PrimaryFile = (bool)rdr["PrimaryFile"];
            IsTargetSystem = (bool)rdr["IsTargetSystem"];

            LastInFileDtTm = (DateTime)rdr["LastInFileDtTm"];
            LastMatchingDtTm = (DateTime)rdr["LastMatchingDtTm"];

            RMCycle = (int)rdr["RMCycle"];
            ExpectedDate = (DateTime)rdr["ExpectedDate"];

            IsReadThisCycle = (bool)rdr["IsReadThisCycle"];

            ProcessMode = (int)rdr["ProcessMode"];
        }

        //
        // READ Category vs Source Files TABLE 
        //
        public void ReadReconcCategoryVsSourcesANDFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMCategoryFilesDataFiles = new DataTable();
            RMCategoryFilesDataFiles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMCategoryFilesDataFiles.Columns.Add("SeqNo", typeof(int));
            RMCategoryFilesDataFiles.Columns.Add("RMCategId", typeof(string));
            RMCategoryFilesDataFiles.Columns.Add("FileName", typeof(string));
            RMCategoryFilesDataFiles.Columns.Add("ProcessMode", typeof(string));
            RMCategoryFilesDataFiles.Columns.Add("LastInFileDtTm", typeof(DateTime));
            RMCategoryFilesDataFiles.Columns.Add("LastMatchingDtTm", typeof(DateTime));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
               + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Table is Ready to Match";
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished";
                            }

                            DataRow RowSelected = RMCategoryFilesDataFiles.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["RMCategId"] = CategoryId;
                            RowSelected["FileName"] = SourceFileName;
                            RowSelected["ProcessMode"] = WProcessModeInWords;
                            RowSelected["LastInFileDtTm"] = LastInFileDtTm;
                            RowSelected["LastMatchingDtTm"] = LastMatchingDtTm;

                            // ADD ROW
                            RMCategoryFilesDataFiles.Rows.Add(RowSelected);

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
                    ErrorOutput = ex.Message;

                    CatchDetails(ex);
                }
        }

        //
        // READ Category vs Source Files TABLE 
        // AND DO 
        public void AssignNextLoadingDate(string InOperator, int InWReconcCycleNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMCategoryFilesDataFiles = new DataTable();
            RMCategoryFilesDataFiles.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT *"
              + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
             // + " WHERE Left(CategoryId, 3) = 'ETI' " + str.Substring(0, 3)
              ; 

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(RMCategoryFilesDataFiles);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            RRDMReconcJobCycles Rcycle = new RRDMReconcJobCycles(); 

            int I = 0;

            while (I <= (RMCategoryFilesDataFiles.Rows.Count - 1))
            {

                RecordFound = true;

                int SaveSeqNo = SeqNo = (int)RMCategoryFilesDataFiles.Rows[I]["SeqNo"];

                

                    UpdateReconcCategoryVsSourceRecordWithRmCycleNo(SeqNo, InWReconcCycleNo, InCut_Off_Date.Date); 

                //}

                    I++; // Read Next entry of the table 

            }
        }

        public void AssignNextLoadingDate_MOBILE(string InJobCategory, string InOperator, int InWReconcCycleNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMCategoryFilesDataFiles = new DataTable();
            RMCategoryFilesDataFiles.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT *"
              + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
              + " WHERE Left(CategoryId, 3) = '" + InJobCategory.Substring(0, 3) + "'"
              ;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(RMCategoryFilesDataFiles);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            RRDMReconcJobCycles Rcycle = new RRDMReconcJobCycles();

            int I = 0;

            while (I <= (RMCategoryFilesDataFiles.Rows.Count - 1))
            {

                RecordFound = true;

                int SaveSeqNo = SeqNo = (int)RMCategoryFilesDataFiles.Rows[I]["SeqNo"];

                //ReadReconcCategoriesVsSourcebySeqNo(SeqNo);

                //int CurrentCycle = RMCycle;

                ////if (IsReadThisCycle == true || CurrentCycle == 0)
                ////if ( CurrentCycle == 0)
                ////{
                //    // Next Cycle Info
                //    Rcycle.FindNextCycle(InOperator, CurrentCycle);

                // Update Source File Record  

                UpdateReconcCategoryVsSourceRecordWithRmCycleNo(SeqNo, InWReconcCycleNo, InCut_Off_Date.Date);

                //}

                I++; // Read Next entry of the table 

            }
        }


        //
        // READ Category vs Source Files TABLE of files 
        //
        public void ReadReconcCategoryVsSourcesANDFillTableofFiles(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMCategoryFilesDataFiles = new DataTable();
            RMCategoryFilesDataFiles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
        
            RMCategoryFilesDataFiles.Columns.Add("FileName", typeof(string));
          
            //RMCategoryFilesDataFiles.Columns.Add("LastMatchingDtTm", typeof(DateTime));


            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
                +  InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            DataRow RowSelected = RMCategoryFilesDataFiles.NewRow();
                    
                            RowSelected["FileName"] = SourceFileName;
                            
                            //RowSelected["LastMatchingDtTm"] = LastMatchingDtTm;

                            // ADD ROW
                            RMCategoryFilesDataFiles.Rows.Add(RowSelected);

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
        // READ Category vs Source Files ALL for this category 
        //
        public void ReadReconcCategoriesVsSourcesAll(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0 ;

            SourceFileNameA = "";
            SourceFileNameB = "";
            SourceFileNameC = "";
            SourceFileNameD = "";
            SourceFileNameE = "";

            string SqlString = "SELECT *"
                + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId "
               + " ORDER By SeqNo";
               

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalRecords = TotalRecords + 1 ;

                            ReaderFields(rdr);

                            if (TotalRecords == 1)
                            {
                                SourceFileNameA = SourceFileName; 
                            }

                            if (TotalRecords == 2)
                            {
                                SourceFileNameB = SourceFileName;
                            }
                            if (TotalRecords == 3)
                            {
                                SourceFileNameC = SourceFileName;
                            }
                            if (TotalRecords == 4)
                            {
                                SourceFileNameD = SourceFileName;
                            }
                            if (TotalRecords == 5)
                            {
                                SourceFileNameE = SourceFileName;
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
        // READ Category vs Source Files 
        //
        public void ReadReconcCategoriesVsSources(string InCategoryId, string InSourceFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId AND SourceFileName = @SourceFileName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileName", InSourceFileName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);
                            if (ProcessMode == -2)
                            {
                                WProcessModeInWords = "Under Loading process";
                            }

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished";
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
        // READ by Source File and check if it is a Target System
        //
        public void ReadReconcCategoriesVsSourcesBySourceFile(string InSourceFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
               + " WHERE SourceFileName = @SourceFileName";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileName", InSourceFileName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

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
        // READ Category vs Source Files by SeqNo 
        //
        public void ReadReconcCategoriesVsSourcebySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
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

                            ReaderFields(rdr);

                            if (ProcessMode == -1)
                            {
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                WProcessModeInWords = "Under Matching Process";
                            }
                            if (ProcessMode == 1)
                            {
                                WProcessModeInWords = "Matching Finished";
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
        // Execute choreographer 
        //
        public void ReadAndCallKontoToReconcile()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Check that KONTO is available 

            ReadIfKontoAvailable();
            if (KontoAvailable == false)
            {
                return; 
            }

            string SqlString = "SELECT DISTINCT [CategoryId]"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
                + " ORDER BY CategoryId " ;

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

                            ReaderFields(rdr);

                            ReadReconcCategoriesVsSourcebyCategory(CategoryId);

                            if (TotalZero == 0 & TotalOne == 0)
                            {
                                
                                //// Call Konto 
                                //StoreProcKontoReconciliation(CategoryId);
                                //// Make -1 to 0 
                                //if (ErrorFound == false & OutReturnedValue == 0)
                                //{
                                //  UpdateReconcCategoryVsSourceRecordProcessCodeToZero(CategoryId);                 
                                //}

                                UpdateReconcCategoryVsSourceRecordProcessCodeToZero(CategoryId);

                                break;
                            }
                            else
                            {
                                // do nothing
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
        // IS KONTO AVAILABLE 
        //
        public void ReadIfKontoAvailable()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Check that KONTO is available 

            KontoAvailable = true; 

            string SqlString = "SELECT DISTINCT [CategoryId]"
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
                + " ORDER BY CategoryId ";

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

                            ReaderFields(rdr);

                            ReadReconcCategoriesVsSourcebyCategory(CategoryId);

                            if (TotalZero > 0 )
                            {

                                KontoAvailable = false;

                                break;
                            }
                            else
                            {
                                // do nothing
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
        // READ Category vs Source Files by Category 
        //
        public void ReadReconcCategoriesVsSourcebyCategory(string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalMinusOne = 0;
            TotalZero = 0;
            TotalOne = 0;

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
               + " WHERE CategoryId = @CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            if (SourceFileName == "Atms_Journals_Txns")
                            {
                                continue; 
                            }

                            if (ProcessMode == -1)
                            {
                                TotalMinusOne = TotalMinusOne + 1; 
                                WProcessModeInWords = "Ready For Match";
                            }
                            if (ProcessMode == 0)
                            {
                                TotalZero = TotalZero + 1; 
                                WProcessModeInWords = "Under Matching Process";
                                
                            }
                            if (ProcessMode == 1)
                            {
                                TotalOne = TotalOne + 1; 
                                WProcessModeInWords = "Matching Finished";
                                
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
        // UPDATE ProcessMode with Zero = in process 
        // 
        public void UpdateReconcCategoryVsSourceRecordProcessCodeToZero(string InCategoryId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " ProcessMode = @ProcessMode "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@ProcessMode", 0);

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
        // UPDATE ProcessMode with Zero = in process 
        // 
        public void UpdateReconcCategoryVsSourceRecordProcessCodeToZeroForCycle(int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " ProcessMode = @ProcessMode "
                            + " WHERE RMCycle = @RMCycle And SourceFileName <> 'Atms_Journals_Txns'", conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@ProcessMode", 0);

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
        // UPDATE ProcessMode with One = ready 
        // 

        public void UpdateReconcCategoryVsSourceRecordProcessCodeToOne(string InCategoryId)
        {
            // Alecos: what is InCategoryId for when the update is done with the CategoryId member??????
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " LastMatchingDtTm = @LastMatchingDtTm, "
                              + " IsReadThisCycle = @IsReadThisCycle, "
                              + " ProcessMode = @ProcessMode "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@LastMatchingDtTm",DateTime.Now);

                        cmd.Parameters.AddWithValue("@IsReadThisCycle", true);

                        cmd.Parameters.AddWithValue("@ProcessMode", 1);
          
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

        // TESTING UPDATE ALL FILES WITH -1 , files read and are ready for matching
        // UPDATE ProcessMode with -1 = ready for matching
        // 

        public void UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(string InSourceFileName, int InRMCycle)
        {
            // Alecos: what is InCategoryId for when the update is done with the CategoryId member??????
            // ALECOS HAS NOTHING TO DO WITH CATEGORIES BECAUSE FILES ARE LOADED BY RRDM
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " LastInFileDtTm = @LastInFileDtTm, "
                              + " ProcessMode = @ProcessMode "
                            + " WHERE RMCycle = @RMCycle AND SourceFileName = @SourceFileName", conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@SourceFileName", InSourceFileName);
                        cmd.Parameters.AddWithValue("@LastInFileDtTm", DateTime.Now);

                        cmd.Parameters.AddWithValue("@ProcessMode", -1);

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
        // UPDATE RMCycle and Expected Date 
        // 

        public void UpdateReconcCategoryVsSourceRecordWithRmCycleNo(int InSeqNo, int InRMCycle, DateTime InDate)
        {
            // Alecos: what is InCategoryId for when the update is done with the CategoryId member??????
            ErrorFound = false;
            ErrorOutput = "";

            string SelectionCriteria;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " RMCycle = @NewRMCycle, "
                              + " ExpectedDate = @ExpectedDate, "
                               + " ProcessMode = 0 , "
                              + " IsReadThisCycle = @IsReadThisCycle "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@NewRMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@ExpectedDate", InDate.Date);
                        cmd.Parameters.AddWithValue("@IsReadThisCycle", false);

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
        // UPDATE If RMCycle is deleted  
        // 

        public void UpdateReconcCategoryVsSourceRecordForDeleteRmCycleNo(int InRMCycle, int InNewRMCycle, DateTime InDate)
        {
            // Alecos: what is InCategoryId for when the update is done with the CategoryId member??????
            ErrorFound = false;
            ErrorOutput = "";

            string SelectionCriteria;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " ExpectedDate = @ExpectedDate, "
                              + " RMCycle = @NewRMCycle, "
                              + " IsReadThisCycle = @IsReadThisCycle "
                              + " WHERE RMCycle = @RMCycle", conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@NewRMCycle", InNewRMCycle);
                        cmd.Parameters.AddWithValue("@ExpectedDate", InDate.Date);
                        cmd.Parameters.AddWithValue("@IsReadThisCycle", true);

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
        // UPDATE ProcessMode to One for all categories of where SourceFileName is used!
        // 
        // Alecos
        public void UpdateReconcCategoryVsSourceRecordProcessCodeForSourceFileName(string InSourceFileName, int InProcessMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            LastInFileDtTm = DateTime.Now;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                            + " ProcessMode = @ProcessMode,  LastInFileDtTm = @LastInFileDtTm"
                            + " WHERE SourceFileName = @SourceFileName", conn))
                        {
                        cmd.Parameters.AddWithValue("@SourceFileName", InSourceFileName);
                        cmd.Parameters.AddWithValue("@ProcessMode", InProcessMode);
                        cmd.Parameters.AddWithValue("@LastInFileDtTm", LastInFileDtTm);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();
                        }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                }
        }

        // Insert Category Vs Source Record 
        //
        public void InsertReconcCategoryVsSourceRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
                    + "([CategoryId], "
                    + " [SourceFileName])"
                    + " VALUES (@CategoryId, "
                    + " @SourceFileName)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        //cmd.Parameters.AddWithValue("@TechnicalSourceName", TechnicalSourceName);
                                      
                      
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


        // UPDATE Category
        // 
        public void UpdateReconcCategoryVsSourceRecord(string InOperator, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] SET "
                              + " PrimaryFile = @PrimaryFile, IsTargetSystem = @IsTargetSystem "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@PrimaryFile", PrimaryFile);
                        cmd.Parameters.AddWithValue("@IsTargetSystem", IsTargetSystem);

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
        // DELETE Category Vs Source Record 
        //
        public void DeleteReconcCategoryVsSourceRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles]"
                            + " WHERE SeqNo = @SeqNo ", conn))
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


        //// Not needed any more
        //// UPDATE ProcessMode using SeqNo
        //// 
        //public void AgentUpdateReconcCategoryVsSourcesProcessMode(int InSeqNo, int InProcessMode)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    LastInFileDtTm = DateTime.Now;

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE [dbo].[MatchingCategoriesVsSourceFiles]"
        //                + " SET ProcessMode = @ProcessMode,  LastInFileDtTm = @LastInFileDtTm" 
        //                + " WHERE SeqNo = @SeqNo", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@ProcessMode", InProcessMode);
        //                cmd.Parameters.AddWithValue("@LastInFileDtTm", LastInFileDtTm);
        //                cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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

        // For demo purposes
        public void FileMonitorReconcCategoryVsSourcesReset()
        {
            ProcessMode = 1;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[MatchingCategoriesVsSourceFiles] SET ProcessMode = @ProcessMode", conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
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

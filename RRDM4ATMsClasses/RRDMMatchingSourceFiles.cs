using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Globalization;

namespace RRDM4ATMs
{
    public class RRDMMatchingSourceFiles : Logger
    {
        public RRDMMatchingSourceFiles() : base() { }

        public int SeqNo;

        public string SystemOfOrigin;
        public bool Enabled;
        public string SourceFileId;

        public string LayoutId;
        public string Delimiter;

        public int LinesInHeader;
        public int LinesInTrailer;

        public string FileNameMask;
        public string ArchiveDirectory;
        public string ExceptionsDirectory;

        public string Type; // USED FOR SEQUENCE IN FILE

        public string InportTableName;
        public string WorkingTableName;

        public string SourceDirectory;

        public string TableStructureId;

        public bool IsMoveToMatched; 

        //public int ProcessMode;
        //
        //                  if (ProcessMode == -2)
        //                  {
        //                      WProcessModeInWords = "Under Loading process";
        //                  }
        //                  if (ProcessMode == -1) => WHEN FILE HAS BEEN LOADED FILES to CATEGORIES  COMBINATIONS ARE UPDATED WITH THIS 
        //                  {
        //                      WProcessModeInWords = "Ready For Match"; // FILE JUST LOADED AND IT IS READY
        //                  }


        public string Operator;

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        // Define the data table 
        public DataTable SourceFilesDataTable = new DataTable();

        public DataTable Table_Files_In_Dir = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // Read Fields 
        //
        private void ReadTblFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
            Enabled = (bool)rdr["Enabled"];
            SourceFileId = (string)rdr["SourceFileId"];
            LayoutId = (string)rdr["LayoutId"];
            Delimiter = (string)rdr["Delimiter"];
            LinesInHeader = (int)rdr["LinesInHeader"];
            LinesInTrailer = (int)rdr["LinesInTrailer"];

            FileNameMask = (string)rdr["FileNameMask"];

            ArchiveDirectory = (string)rdr["ArchiveDirectory"];
            ExceptionsDirectory = (string)rdr["ExceptionsDirectory"];
            Type = (string)rdr["Type"];
            InportTableName = (string)rdr["InportTableName"];
            WorkingTableName = (string)rdr["WorkingTableName"];
            SourceDirectory = (string)rdr["SourceDirectory"];

            TableStructureId = (string)rdr["TableStructureId"];

            IsMoveToMatched = (bool)rdr["IsMoveToMatched"];

            Operator = (string)rdr["Operator"];
        }


        //
        // READ Source File 
        //
        public void ReadReconcSourceFilesToFillDataTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SourceFilesDataTable = new DataTable();
            SourceFilesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            SourceFilesDataTable.Columns.Add("SeqNo", typeof(int));
            SourceFilesDataTable.Columns.Add("FileSeq", typeof(string));
            SourceFilesDataTable.Columns.Add("SourceFile_ID", typeof(string));
            SourceFilesDataTable.Columns.Add("OriginSystem", typeof(string));
            SourceFilesDataTable.Columns.Add("Type", typeof(string));
            SourceFilesDataTable.Columns.Add("SourceDirectory", typeof(string));
            SourceFilesDataTable.Columns.Add("DbTblName", typeof(string));
            SourceFilesDataTable.Columns.Add("TableStructureId", typeof(string));
            

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
                + " WHERE " + InSelectionCriteria
                + " Order By Type  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        ///*cmd.Parameters.AddWithValue("@Ope*/rator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTblFields(rdr);

                            //FILL TABLE 
                            DataRow RowSelected = SourceFilesDataTable.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["FileSeq"] = Type;
                            RowSelected["SourceFile_ID"] = SourceFileId;
                            RowSelected["OriginSystem"] = SystemOfOrigin;
                            RowSelected["Type"] = Type;
                            RowSelected["SourceDirectory"] = SourceDirectory;
                            RowSelected["DbTblName"] = InportTableName;
                            RowSelected["TableStructureId"] = TableStructureId;
                            

                            // ADD ROW
                            SourceFilesDataTable.Rows.Add(RowSelected);

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
        // READ Source File and fill Table FULL
        //
        public void ReadReconcSourceFilesToFillDataTable_FULL(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SourceFilesDataTable = new DataTable();
            SourceFilesDataTable.Clear();

            TotalSelected = 0;

            TotalSelected = 0;

            string SqlString = "SELECT *"
                          + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
                           +  InSelectionCriteria
                           + " Order By Type  ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                       
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(SourceFilesDataTable);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        //
        // READ Source File 
        //
        public bool IsPresentInDirectory;
        public string IsGood;// YES, NO , N/A 
        public string Dir_Comment;

        public int TotalFiles;
        public int TotalReady;
        string LastLoadedDate; 

        bool FirstCall; 

        public void ReadReconcSourceFilesToFillDataTableForExistanceInDir(string InOperator, string InSelectionCriteria
                                             , int InRMCycleNo, DateTime InDateExpected)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;
            TotalReady = 0;

            FirstCall = true; // leave here 

            RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            SourceFilesDataTable = new DataTable();
            SourceFilesDataTable.Clear();


            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
                + " WHERE " + InSelectionCriteria // SELECT ALL ACTIVE
                + " Order By TYPE ASC ";


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(SourceFilesDataTable);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            try
            {
                // READ EACH FILE , CHECK IF AVAILABLE and Insert them in Table
                int I = 0;

                while (I <= (SourceFilesDataTable.Rows.Count - 1))
                {
                    TotalFiles = TotalFiles + 1;

                    int WSeqNo = (int)SourceFilesDataTable.Rows[I]["SeqNo"];
                    string WSourceFileId = (string)SourceFilesDataTable.Rows[I]["SourceFileId"];
                    string WSourceDirectory = (string)SourceFilesDataTable.Rows[I]["SourceDirectory"];
                    string WFileNameMask = (string)SourceFilesDataTable.Rows[I]["FileNameMask"];

                    Rfm.ReadRecordByLoadedByCycle(WSourceFileId);

                    LastLoadedDate = Rfm.DateTimeReceived.ToString();

                    // 

                    Rfm.DateExpected = InDateExpected;

                    // Check ******************* and fill table in the Method called
                    // 

                    CheckIfFileInDirectory(InOperator, WSourceFileId, WSourceDirectory, Rfm.DateExpected, WFileNameMask, 1);

            //TotalReady ;
                    //

                    I = I + 1;

                }
            }
            catch (Exception ex)
            {
              //  conn.Close();
                CatchDetails(ex);
            }

        }

        public void ReadReconcSourceFilesToFillDataTable(string InOperator, string InSelectionCriteria
                                      , int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;
            TotalReady = 0;

            FirstCall = true; // leave here 

            RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            SourceFilesDataTable = new DataTable();
            SourceFilesDataTable.Clear();


            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
                + " WHERE " + InSelectionCriteria // SELECT ALL ACTIVE
                + " Order By TYPE ASC ";


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(SourceFilesDataTable);

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
        public void CheckIfMinimumFilesExists(string InOperator, string InSourceFileId, string InSourceDirectory, DateTime InDateExpected, string InFileNameMask, int InMode)
        {
            long File_length;

            IsPresentInDirectory = false; // True or False
            IsGood = "NO"; // YES, NO , N/A 
            Dir_Comment = "Dir_Comment"; // eg Good to process , No File, Wrong file etc  
            try
            {
                string[] allFiles = Directory.GetFiles(InSourceDirectory, "*.*");
                if (allFiles == null || allFiles.Length == 0)
                {
                    IsPresentInDirectory = false;
                    FullFileName = "File Not Found In ..DIR.." + InSourceDirectory;
                    Dir_Comment = "Directory " + InSourceDirectory + " is empty.";
                    IsGood = "NO";
                    // FullFileName = "File Not Found";
                    if (WMode == 1) AddToTable();

                    return;
                }

                string[] specificFiles = { };


                switch (WShortBank)
                {
                    case "BDC":
                        {
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");

                            break;
                        }
                    case "AUD":
                        {
                            //if(WSourceFileId == "CIT_Speed_Excel")
                            //{
                            //    specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???.???"); // for Excel 
                            //}
                            //else
                            //{
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");
                            // }


                            break;
                        }
                    case "ABE":
                        {

                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???.???");

                            break;
                        }


                    default:
                        {
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");

                            break;
                        }
                }

                if (specificFiles == null || specificFiles.Length == 0)
                {
                    IsPresentInDirectory = false;
                    Dir_Comment = "Directory " + InSourceDirectory + " contains file(s) but not with the correct pattern.";
                    IsGood = "NO";

                    FullFileName = "File Not Found..IN.." + InSourceDirectory;

                    if (WMode == 1) AddToTable();

                    return;
                }

            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }
        }
        string FullFileName;
        int TotalSelected_InDir;
        DateTime WExpDate;
        string HASHValue ;
        public int WFutureFiles;
        string WSourceFileId;
        int WMode;
        string WShortBank = ""; 
        public void CheckIfFileInDirectory(string InOperator, string InSourceFileId, string InSourceDirectory, DateTime InDateExpected, string InFileNameMask, int InMode)
        {
            WExpDate = InDateExpected;

            WSourceFileId = InSourceFileId; 

            HASHValue = "NO_Value";

                // If InMode = 1 = Fill table
                // If InMode = 2 = Just check 
            WMode = InMode;

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(InOperator);

            WShortBank = Ba.ShortName; // BDC or ABE 

            if (FirstCall == true & WMode == 1)
            {
                Table_Files_In_Dir = new DataTable();
                Table_Files_In_Dir.Clear();

                TotalSelected_InDir = 0;

                WFutureFiles = 0; 

                TotalReady = 0;

                // DATA TABLE ROWS DEFINITION 
                Table_Files_In_Dir.Columns.Add("SeqNo", typeof(int));
                
                Table_Files_In_Dir.Columns.Add("SourceFileId", typeof(string));
                Table_Files_In_Dir.Columns.Add("FullFileName", typeof(string));
                Table_Files_In_Dir.Columns.Add("IsPresent", typeof(bool));
                Table_Files_In_Dir.Columns.Add("IsGood", typeof(string));
                Table_Files_In_Dir.Columns.Add("Comment", typeof(string));
                Table_Files_In_Dir.Columns.Add("DateExpected", typeof(string));
                Table_Files_In_Dir.Columns.Add("LastLoaded", typeof(string));
                Table_Files_In_Dir.Columns.Add("HASHValue", typeof(string));

                FirstCall = false; 
            }
           
            RecordFound = false;
            ErrorFound = false;

            if (WSourceFileId == "Atms_Journals_Txns")
            {
                
                IsPresentInDirectory = true;
                Dir_Comment = " # of Journals In Dir.. Loaded" ;
                IsGood = "YES";
                // ADD ROW

                FullFileName = "General Journal File Name";
                if  (WMode == 1) AddToTable();

                return;
            }
            // 
            // Validations
            // existance of a file
            // Existance of the right file
            // Correct day <= Less than the expected date
            // Not read before based on hashing value.
            // Empty file

            //C:\RRDM\FilePool\TabDel

            //  ConvertExcelToTabText see next method



            long File_length; 

            IsPresentInDirectory = false; // True or False
            IsGood = "NO"; // YES, NO , N/A 
            Dir_Comment = "Dir_Comment"; // eg Good to process , No File, Wrong file etc  
            try
            {
                string[] allFiles = Directory.GetFiles(InSourceDirectory, "*.*");
                if (allFiles == null || allFiles.Length == 0)
                {
                    IsPresentInDirectory = false;
                    FullFileName = "File Not Found In ..DIR.."+ InSourceDirectory;
                    Dir_Comment = "Directory " + InSourceDirectory + " is empty.";
                    IsGood = "NO";
                    // FullFileName = "File Not Found";
                    if (WMode == 1) AddToTable();

                    return;
                }

                string[] specificFiles = {}; 
                

                switch (WShortBank)
                {
                    case "BDC":
                        {
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");

                            break;
                        }
                    case "EGA":
                        {
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");

                            break;
                        }
                    case "AUD":
                        {
                            //if(WSourceFileId == "CIT_Speed_Excel")
                            //{
                            //    specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???.???"); // for Excel 
                            //}
                            //else
                            //{
                                specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");
                           // }


                            break;
                        }
                    case "ABE":
                        {
                           
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???.???");

                            break;
                        }


                    default:
                        {
                            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???");

                            break;
                        }
                }

                if (specificFiles == null || specificFiles.Length == 0)
                {
                    IsPresentInDirectory = false;
                    Dir_Comment = "Directory " + InSourceDirectory + " contains file(s) but not with the correct pattern.";
                    IsGood = "NO";

                    FullFileName = "File Not Found..IN.."+ InSourceDirectory;

                    if (WMode == 1) AddToTable();

                    return;
                }

                string pattern = "";

                string pattern2 = "";

                foreach (string file in specificFiles)
                {
                    //int FileLen = file.Length;
                    
                    File_length = new System.IO.FileInfo(file).Length;
                    if (File_length < 500)
                    {
                        File.Delete(file);
                        continue; 
                    }

                    
                    switch (WShortBank)
                    {
                        case "BDC":
                            {
                                pattern = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}";

                                break;
                            }
                        case "EGA":
                            {
                                pattern = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}";

                                break;
                            }
                        case "AUD":
                            {
                                //if (WSourceFileId == "CIT_Speed_Excel")
                                //{
                                //    pattern2 = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}" + ".xls"; // for Excel 
                                //}
                                //else
                                //{
                                    pattern = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}";
                               // }
 

                                break;
                            }
                        case "ABE":
                            {
                                //pattern = WSourceFileId + @"_[0-9]{8}\.[0-9]{3}" + ".txt";
                                pattern = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}" + ".txt";
                                pattern2 = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}" + ".csv";
                                // pattern = WSourceFileId + @"_[0-9]{8}\_[0-9]{3}" + ".txt";
                                break;
                            }


                        default:
                            {
                                pattern = WSourceFileId + "_[0-9]{8}\\.[0-9]{3}";

                                break;
                            }
                    }
                    //try
                    //{
                    //    if (Regex.IsMatch(file, pattern))
                    //    {
                    //        var resultLastThreeDigits = file.Substring(file.Length - 3);
                    //        string result1 = file.Substring(file.Length - 12);
                    //        string result2 = result1.Substring(0, 8);
                    //    }
                    //}
                    //catch (Exception exf)
                    //{
                    //    CatchDetails(exf);
                    //}


                    if (Regex.IsMatch(file, pattern) || (WShortBank == "AUD" & Regex.IsMatch(file, pattern2)) || (WShortBank == "ABE" & Regex.IsMatch(file, pattern2)) )
                    {
                        // Found a file that matches!
                        var resultLastThreeDigits = "";
                        string result1 = "";
                        string result2 = "";
                        
                        switch (WShortBank)
                        {
                            case "BDC":
                                {
                                    resultLastThreeDigits = file.Substring(file.Length - 3);
                                    result1 = file.Substring(file.Length - 12);
                                    result2 = result1.Substring(0, 8);

                                    break;
                                }
                            case "EGA":
                                {
                                    resultLastThreeDigits = file.Substring(file.Length - 3);
                                    result1 = file.Substring(file.Length - 12);
                                    result2 = result1.Substring(0, 8);

                                    break;
                                }
                            case "AUD":
                                {
                                    //(WSourceFileId == "CIT_Speed_Excel")
                                    //if (WSourceFileId == "CIT_Speed_Excel")
                                    //{
                                    //    resultLastThreeDigits = file.Substring(file.Length - 7);
                                    //    result1 = file.Substring(file.Length - 16);
                                    //    result2 = result1.Substring(0, 8);
                                    //}
                                    //else
                                    //{
                                        resultLastThreeDigits = file.Substring(file.Length - 3);
                                        result1 = file.Substring(file.Length - 12);
                                        result2 = result1.Substring(0, 8);
                                   // }


                                    break;
                                }
                            case "ABE":
                                {
                                    resultLastThreeDigits = file.Substring(file.Length - 7);
                                    result1 = file.Substring(file.Length - 16);
                                    result2 = result1.Substring(0, 8);

                                    break;
                                }


                            default:
                                {
                                    resultLastThreeDigits = file.Substring(file.Length - 3);
                                    result1 = file.Substring(file.Length - 12);
                                    result2 = result1.Substring(0, 8);
                                    break;
                                }
                        }


                        DateTime FileDATEresult;
                        
                        if (DateTime.TryParseExact(result2, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                        {
                            
                        }

                        //if (FileDATEresult != WExpDate)
                        //{
                        //    WFutureFiles = WFutureFiles + 1;
                        //    continue;
                        //}
                        if (FileDATEresult > WExpDate)
                        {
                            WFutureFiles = WFutureFiles + 1;
                            continue;
                        }
                        else
                        {
                            // Means Date is less or equal to expected which is OK 
                            // OK 
                        }

                        FullFileName = InSourceDirectory + "\\" + WSourceFileId + "_" + FileDATEresult.ToString("yyyyMMdd") + "." + resultLastThreeDigits;
                        if (File.Exists(FullFileName))
                        {

                            RRDMReconcFileMonitorLog Fl = new RRDMReconcFileMonitorLog();
                            // File EXIST AND It IS VALID 



                            HASHValue = FileHASH.BytesToString(FileHASH.GetHashSha256(FullFileName));
                            
                            Fl.GetRecordByFileHASH(HASHValue);
                            if (Fl.RecordFound)
                            {
                                // FILE READ BEFORE
                                IsPresentInDirectory = true;
                                Dir_Comment = "File with Pattern " + WSourceFileId + " found in " + InSourceDirectory +
                                              " but it was read before .under the name:"+Environment.NewLine +Fl.ArchivedPath;
                                IsGood = "NO";
                                //
                                if (WMode == 1) AddToTable();
                            }
                            else
                            {
                                Fl.GetRecordByFileName(FullFileName );
                                if (Fl.RecordFound == true & Fl.Status == 1)
                                {
                                    // Same name read before
                                    IsPresentInDirectory = true;
                                    Dir_Comment = "File with Pattern " + WSourceFileId + " found in " + InSourceDirectory +
                                                  " but it was read before .under the same name At cycle:" + Environment.NewLine + Fl.RMCycleNo.ToString();
                                    IsGood = "NO";
                                    //
                                    if (WMode == 1) AddToTable();
                                }
                                else
                                {
                                    Gp.ReadParametersSpecificNm(InOperator, "918", InSourceFileId);
                                    int ExpectedSize = (int)Gp.Amount;

                                    //ExpectedSize = 100; 

                                    if (ExpectedSize != 0)
                                    {
                                        // Make file check minimum length
                                        if (File_length < ExpectedSize * 1000)
                                        {
                                            // File size is not as expected
                                            IsPresentInDirectory = true;
                                            Dir_Comment = "File with Pattern " + WSourceFileId + " found in " + InSourceDirectory +
                                                     " but it size is less than GB :" + File_length.ToString() + Environment.NewLine + Fl.ArchivedPath;
                                            IsGood = "NO";
                                            //
                                            if (WMode == 1) AddToTable();
                                        }
                                        else
                                        {
                                            IsPresentInDirectory = true;
                                            IsGood = "YES";
                                            Dir_Comment = "File found as expected!";

                                            if (WMode == 1) AddToTable();
                                        }
                                    }
                                    else
                                    {
                                        IsPresentInDirectory = true;
                                        IsGood = "YES";
                                        Dir_Comment = "File found as expected!";

                                        if (WMode == 1) AddToTable();
                                    }

                                }

                            }
                           
                        }
                        else
                        {
                            // FILE EXIST BUT WITH DIFFERENT NAME
                            // Check if less than the expected Date 

                            IsPresentInDirectory = false;
                            Dir_Comment = "File with Pattern " + WSourceFileId + " found in " + InSourceDirectory +
                                          " the date does not match the one supplied";
                            IsGood = "NO";

                            if (WMode == 1) AddToTable();

                        }
                    }
                    else
                    {
                        IsPresentInDirectory = false;
                        Dir_Comment = "File with Pattern " + WSourceFileId + " not found in " + InSourceDirectory;
                        IsGood = "NO";

                        FullFileName = "File Not Found";
                        if (WMode == 1) AddToTable();
                    }

                }

            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

        }

        // Check If File is dublicate 
        public bool CheckIfFileIsDublicate(string InFile)
        {
            // CHECK FOR DUBLILATES AND DELETE  

            bool Dublicate = false; 

            try
            {
                HASHValue = FileHASH.BytesToString(FileHASH.GetHashSha256(InFile));
                RRDMReconcFileMonitorLog Fl = new RRDMReconcFileMonitorLog();
                Fl.GetRecordByFileHASH(HASHValue);
                if (Fl.RecordFound)
                {
                    // Journal READ BEFORE
                    Dublicate = true; 
                }
                else
                {

                }
            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

            return Dublicate; 


        }

        // Add to Table
        public void AddToTable()
        {
            
            RecordFound = false;
            ErrorFound = false;
            // 
            try
            {
                //FILL TABLE 
                DataRow RowSelected = Table_Files_In_Dir.NewRow();

                TotalSelected_InDir = TotalSelected_InDir + 1;

                RowSelected["SeqNo"] = TotalSelected_InDir;
           
                RowSelected["SourceFileId"] = WSourceFileId;
                RowSelected["FullFileName"] = FullFileName;
                RowSelected["IsPresent"] = IsPresentInDirectory;
                RowSelected["IsGood"] = IsGood;
                RowSelected["Comment"] = Dir_Comment;
                RowSelected["DateExpected"] = WExpDate.ToShortDateString();
                RowSelected["LastLoaded"] = LastLoadedDate;
                RowSelected["HASHValue"] = HASHValue;

                Table_Files_In_Dir.Rows.Add(RowSelected);


                if (IsGood == "YES")
                {
                    TotalReady = TotalReady + 1;
                }

            }
            catch (Exception exf)
            {
               // CatchDetails(exf);
            }

        }
        // HASHING
        public class FileHASH
        {
            // Compute the file's hash.
            public static byte[] GetHashSha256(string filename)
            {
                byte[] hashval;
                // The cryptographic service provider.
                SHA256 Sha256 = SHA256.Create();

                using (FileStream stream = File.OpenRead(filename))
                {
                    hashval = Sha256.ComputeHash(stream);
                    stream.Close();
                    return (hashval);
                }
            }

            // Return a byte array as a sequence of hex values.
            public static string BytesToString(byte[] bytes)
            {
                string result = "";
                foreach (byte b in bytes) result += b.ToString("x2");
                return result;
            }

        }
        // Convert Excel 
        public void ConvertExcelToTabText(string InSourceFullFileId)
        {
            string FullFileName;
            RecordFound = false;
            ErrorFound = false;
            // 
            try
            {
                Microsoft.Office.Interop.Excel.Application myExcel;
                Microsoft.Office.Interop.Excel.Workbook myWorkbook;
                Microsoft.Office.Interop.Excel.Worksheet worksheet;

                string myInputFile = InSourceFullFileId;
                string myOutFile = "C:\\RRDM\\FilePool\\TabDel\\testout.txt";
                myExcel = new Microsoft.Office.Interop.Excel.Application();
                myExcel.Workbooks.Open(myInputFile, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                myWorkbook = myExcel.ActiveWorkbook;
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)myWorkbook.Worksheets[1];
                myWorkbook.SaveAs(myOutFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlTextWindows, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                myWorkbook.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                myExcel.Quit();

            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

        }
        //
        // READ Source File by name to find technical 
        //
        public void ReadReconcSourceFilesByFileId(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
               + " WHERE SourceFileId = @SourceFileId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTblFields(rdr);

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
        // TRUNCATE ALL FILES 
        //
        
        public void ReadFilesANDTruncate(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            string PhysicalName; 

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingSourceFiles] "
               + " WHERE Operator = @Operator ";

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

                            ReadTblFields(rdr);

                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + SourceFileId; 
                            Bio.TruncateTable(PhysicalName);
                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + SourceFileId;
                            Bio.TruncateTable(PhysicalName);
                            PhysicalName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + SourceFileId;
                            Bio.TruncateTable(PhysicalName);
                           
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

        // UNDO STATISTICS 

        //
        // UNDO FILES FOR CYCLE 
        //
        public string ProgressText_2 = ""; 
        public void ReadFilesAND_UNDO_For_Cycle(string InOperator, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            ProgressText_2 = "";
            string PhysicalName;

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[MatchingSourceFiles] "
               + " WHERE Operator = @Operator ";

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

                            ReadTblFields(rdr);

                            // DO THE JOB FOR BULK FILES 
                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + SourceFileId+"_ALL";
                            Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only(PhysicalName, InReconcCycleNo);

                            // DO THE JOB OTHER RRDM STD FILES 
                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + SourceFileId;
                            Bio.UNDO_Table_For_Cycle(PhysicalName, InReconcCycleNo);

                            if (Bio.ProgressText_3 != "")
                            ProgressText_2 = ProgressText_2 + Bio.ProgressText_3 + Environment.NewLine;
                          
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

        public void ReadFilesAND_UNDO_For_Cycle_MOBILE(string InApplication, string InOperator, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            ProgressText_2 = "";
            string PhysicalName;

            string SqlString = " SELECT * "
               + " FROM [ATMS].[dbo].[MatchingSourceFiles] "
               + " WHERE Operator = @Operator AND SystemOfOrigin = @SystemOfOrigin ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", InApplication);
                       // [SystemOfOrigin]
        // Read table 

                       SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadTblFields(rdr);

                            // SystemOfOrigin Equivalent to DATA BASE NAME eg QAHERA 

                            // DO THE JOB FOR BULK FILES 
                            string DB = SystemOfOrigin; 
                            PhysicalName = DB+".[dbo].BULK_" + SourceFileId + "_ALL";
                            //Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only(PhysicalName, InReconcCycleNo);
                            Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only_MOBILE(DB, PhysicalName, InReconcCycleNo);


                            // DO THE JOB OTHER RRDM STD FILES 
                            PhysicalName = DB + ".[dbo]." + SourceFileId;
                            Bio.UNDO_Table_For_Cycle_MOBILE(DB, PhysicalName, InReconcCycleNo);

                            if (Bio.ProgressText_3 != "")
                                ProgressText_2 = ProgressText_2 + Bio.ProgressText_3 + Environment.NewLine;

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


        public void ReadFilesAND_UNDO_For_Cycle_File(string InOperator, int InReconcCycleNo, string InFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            ProgressText_2 = "";
            string PhysicalName;

            

            // DO THE JOB FOR BULK FILES - The BULK ALL 
                            
                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InFileId + "_ALL";
                            Bio.UNDO_Table_For_Cycle_Delete_Loaded_Only(PhysicalName, InReconcCycleNo);

                            // DO THE JOB OTHER RRDM STD FILES 
                            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + InFileId;
                            Bio.UNDO_Table_For_Cycle(PhysicalName, InReconcCycleNo);

                            if (Bio.ProgressText_3 != "")
                                ProgressText_2 = ProgressText_2 + Bio.ProgressText_3 + Environment.NewLine;

                //        }

                //        // Close Reader
                //        rdr.Close();
                //    }

                //    // Close conn
                //    conn.Close();
                //}
                //catch (Exception ex)
                //{
                //    conn.Close();

                //    CatchDetails(ex);
                //}
        }


        public void ReadSourceFileRecordByOriginAndFileID(string SysOfOrigin, string SrcFileID)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [dbo].[MatchingSourceFiles]"
                            + " WHERE SystemOfOrigin = @SysOfOrigin AND SourceFileID = @SrcFileID ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SysOfOrigin", SysOfOrigin);
                        cmd.Parameters.AddWithValue("@SrcFileID", SrcFileID);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ErrorFound = false;
                            ErrorOutput = "";

                            ReadTblFields(rdr);
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
        // READ ReconcSourceFiles by SeqNo
        //
        public void ReadReconcSourceFilesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingSourceFiles]"
               + " WHERE SeqNo = @SeqNo";

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

                            ReadTblFields(rdr);
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

        // Insert File Field Record 
        //
        public void InsertReconcSourceFileRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingSourceFiles]"
                    + "([SystemOfOrigin], [Enabled],[SourceFileId],"
                    + " [LayoutId], [Delimiter], [LinesInHeader], [LinesInTrailer], [FileNameMask],  "
                    + " [ArchiveDirectory], [ExceptionsDirectory],  "
                    + " [Type], [InportTableName], [WorkingTableName],"
                    + " [SourceDirectory],  "
                    + " [TableStructureId],  "
                    + " [IsMoveToMatched],  "
                    + " [Operator] )"
                    + " VALUES (@SystemOfOrigin, @Enabled, @SourceFileId,"
                    + " @LayoutId, @Delimiter, @LinesInHeader, @LinesInTrailer, @FileNameMask,"
                    + " @ArchiveDirectory, @ExceptionsDirectory, "
                    + "@Type, @InportTableName, @WorkingTableName,"
                    + " @SourceDirectory,  "
                    + " @TableStructureId,  "
                    + " @IsMoveToMatched,  "
                    + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@Enabled", Enabled);
                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);
                        cmd.Parameters.AddWithValue("@LayoutId", LayoutId);
                        cmd.Parameters.AddWithValue("@Delimiter", Delimiter);
                        cmd.Parameters.AddWithValue("@LinesInHeader", LinesInHeader);
                        cmd.Parameters.AddWithValue("@LinesInTrailer", LinesInTrailer);

                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);
                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);

                        cmd.Parameters.AddWithValue("@ExceptionsDirectory", ExceptionsDirectory);

                        cmd.Parameters.AddWithValue("@Type", Type);
                        cmd.Parameters.AddWithValue("@InportTableName", InportTableName);
                        cmd.Parameters.AddWithValue("@WorkingTableName", WorkingTableName);
                        cmd.Parameters.AddWithValue("@SourceDirectory", SourceDirectory);

                        cmd.Parameters.AddWithValue("@TableStructureId", TableStructureId);

                        cmd.Parameters.AddWithValue("@IsMoveToMatched", IsMoveToMatched);

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

        // UPDATE File 
        // 
        public void UpdateReconcSourceFileRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingSourceFiles] SET "
                              + " SystemOfOrigin = @SystemOfOrigin, Enabled = @Enabled, "
                              + " SourceFileId = @SourceFileId, "
                              + " LayoutId = @LayoutId, Delimiter = @Delimiter, LinesInHeader = @LinesInHeader, "
                              + " LinesInTrailer = @LinesInTrailer, FileNameMask = @FileNameMask, "
                              + " ArchiveDirectory = @ArchiveDirectory, ExceptionsDirectory = @ExceptionsDirectory, "
                              + " Type = @Type, InportTableName = @InportTableName, "
                              + " WorkingTableName = @WorkingTableName, "
                              + " SourceDirectory = @SourceDirectory,"
                              + " IsMoveToMatched = @IsMoveToMatched,"
                              + " Operator = @Operator  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@Enabled", Enabled);
                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);
                        cmd.Parameters.AddWithValue("@LayoutId", LayoutId);
                        cmd.Parameters.AddWithValue("@Delimiter", Delimiter);
                        cmd.Parameters.AddWithValue("@LinesInHeader", LinesInHeader);
                        cmd.Parameters.AddWithValue("@LinesInTrailer", LinesInTrailer);
                        cmd.Parameters.AddWithValue("@FileNameMask", FileNameMask);

                        cmd.Parameters.AddWithValue("@ArchiveDirectory", ArchiveDirectory);

                        cmd.Parameters.AddWithValue("@ExceptionsDirectory", ExceptionsDirectory);

                        cmd.Parameters.AddWithValue("@Type", Type);
                        cmd.Parameters.AddWithValue("@InportTableName", InportTableName);
                        cmd.Parameters.AddWithValue("@WorkingTableName", WorkingTableName);
                        cmd.Parameters.AddWithValue("@SourceDirectory", SourceDirectory);

                        cmd.Parameters.AddWithValue("@IsMoveToMatched", IsMoveToMatched);

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
        // DELETE file 
        //
        public void DeleteFileFieldRecord(int InSeqNo, string InSourceFileId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingSourceFiles] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
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

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                            + " WHERE SourceFileId =  @SourceFileId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

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

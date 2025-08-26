using System;
using System.Data;
using System.Text;
using System.IO;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;


namespace RRDM4ATMs
{
    public class RRDMJournalReadTxns_Text_Class : Logger
    {
        public RRDMJournalReadTxns_Text_Class() : base() { }

        string WJournalId;

        public int SeqNo;
        public string Source;
        public string Comments;
        public string BankId;
        public string AtmNo;
        public string TraceNo;
        public DateTime TransDate;
        public TimeSpan TransTime;
        public int TraceNumber;
        public string TxtLine;

        public int Sessionstart;
        public int StartTxn;
        public int EndTxn;
        public int SessionEnd;

        public int FuId;

        public int RuId;

        public int RowNumber;
        public string CardNo;
        public string AccNo;
        public int TranType;

        public string Descr;
        public string CurNm;
        public decimal TranAmnt;
        public int ErrId;

        public string Operator;

        string SqlString;

        // Define the data table 
        public DataTable TraceJournalLinesDataTable = new DataTable();


        public DataTable JournalLines = new DataTable();

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // Convert Journal
        // Insert a sequence number in-front of each line 
        //
        public int LineCounter;
        public string ConvertJournal(string fullFileName)
        {

            // string fileName_In;
            string fileName_Out = "";
            string fileDir;
            string msg;

            try
            {
                fileDir = Path.GetDirectoryName(fullFileName);
                string fileOut = Path.GetFileName(fullFileName);
                fileOut += ".jln";
                fileName_Out = Path.Combine(fileDir, fileOut);

                FileStream fs = null;
                int lineNumber = 1000000;

                // Read in lines from file.
                string[] lineArrayIn = File.ReadAllLines(fullFileName, Encoding.GetEncoding(1253));
                //string[] lineArrayIn = File.ReadAllLines(fullFileName);
                int lineCount = lineArrayIn.Length;
                string[] lineArrayOut = new string[lineCount];

                for (LineCounter = 0; LineCounter < lineCount; LineCounter++)
                {
                    lineNumber++;
                    lineArrayOut[LineCounter] = string.Format("{0,0000000} {1}", lineNumber, lineArrayIn[LineCounter]);

                }
                //File.WriteAllLines(fileName_Out, lineArrayOut, Encoding.GetEncoding(1253));
                File.WriteAllLines(fileName_Out, lineArrayOut, Encoding.GetEncoding(1253));

            }
            catch (Exception ex)
            {
                ErrorFound = true;
                ErrorOutput = string.Format("Exception encountered while creating the JLN file for source file: {0}\r\nThe message is:\r\n{1}\r\nThe StackTrace is:\r\n{2}", fullFileName, ex.Message, ex.StackTrace);
                fileName_Out = "";
                CatchDetails(ex);
            }
            return fileName_Out;
        }

        // Truncate  Temp
        //
        public void TruncateTempTable(string InFile)
        {
            string SQLCmd = "TRUNCATE TABLE " + InFile;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                    return;
                }
        }

        //////
        ////// Move file to the Archive directory
        //////
        ////public bool MoveFileToArchiveDirectory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InFullPath)
        ////{

        ////    bool Success = false;
        ////    RRDMGasParameters Gp = new RRDMGasParameters();

        ////    string ParId = "920";
        ////    string OccurId = "14";
        ////    string CopiedFile;
        ////    Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

        ////    RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

        ////    Msf.ReadReconcSourceFilesByFileId(InTableA);

        ////    string DestinationName = Msf.SourceFileId;


        ////    string WorkingDirectory = Gp.OccuranceNm + InReversedCut_Off_Date + "_"
        ////                                                + InReconcCycleNo.ToString() + "\\"
        ////                                                + DestinationName.ToString() + "\\";

        ////    try
        ////    {
        ////        bool WSuccess = CreateDirectory(WorkingDirectory);

        ////        if (WSuccess == true)
        ////        {
        ////            CopiedFile = CopyFileFromOneDirectoryToAnother(InFullPath, WorkingDirectory);

        ////        }

        ////        if (CopySuccess == true)
        ////        {
        ////            DeleteFileFromDirectory(InFullPath);
        ////        }

        ////        Success = true;
        ////    }
        ////    catch (Exception exf)
        ////    {
        ////        CatchDetails(exf);
        ////    }

        ////    return Success;

        ////}

        //////
        ////// Create Excel Directory if not Available 
        //////
        ////public bool CreateExcelDirectory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InFullPath)
        ////{

        ////    bool Success = false;
        ////    RRDMGasParameters Gp = new RRDMGasParameters();

        ////    string ParId = "920";
        ////    string OccurId = "14";
        ////    string CopiedFile;
        ////    Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

        ////    RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

        ////    Msf.ReadReconcSourceFilesByFileId(InTableA);

        ////    string DestinationName = Msf.SourceFileId;


        ////    string WorkingDirectory = Gp.OccuranceNm + InReversedCut_Off_Date + "_"
        ////                                                + InReconcCycleNo.ToString() + "\\"
        ////                                                + DestinationName.ToString() + "\\";

        ////    try
        ////    {
        ////        bool WSuccess = CreateDirectory(WorkingDirectory);

        ////        if (WSuccess == true)
        ////        {
        ////            CopiedFile = CopyFileFromOneDirectoryToAnother(InFullPath, WorkingDirectory);

        ////        }

        ////        if (CopySuccess == true)
        ////        {
        ////            DeleteFileFromDirectory(InFullPath);
        ////        }

        ////        Success = true;
        ////    }
        ////    catch (Exception exf)
        ////    {
        ////        CatchDetails(exf);
        ////    }

        ////    return Success;

        ////}

        //////
        ////// For this cycle find all files and move them to the origin 
        //////
        ////public bool LoopFor_MoveFile_From_Archive_ToOrigin_Directory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date)
        ////{
        ////    bool Success = false;
        ////    RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        ////    RRDMGasParameters Gp = new RRDMGasParameters();

        ////    string ParId = "920";
        ////    string OccurId = "14"; // FilesArchives

        ////    Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

        ////    string WorkingDirectory_A = Gp.OccuranceNm + InReversedCut_Off_Date
        ////        + "_" + InReconcCycleNo.ToString() + "\\";

        ////    string WSelectionCriteria = " Operator='" + InOperator + "'";
        ////    Rs.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

        ////    int I = 0;

        ////    while (I <= (Rs.SourceFilesDataTable.Rows.Count - 1))
        ////    {

        ////        string SourceFileId = (string)Rs.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
        ////        string SourceDirectory = (string)Rs.SourceFilesDataTable.Rows[I]["SourceDirectory"];

        ////        string InTableA = SourceFileId;

        ////        string WorkingDirectory_B = Gp.OccuranceNm + InReversedCut_Off_Date
        ////       + "_" + InReconcCycleNo.ToString() + "\\"
        ////        + InTableA + "\\";

        ////        Undo_MoveFile_From_Archive_ToOrigin_Directory(InOperator, InReconcCycleNo, InReversedCut_Off_Date, InTableA, WorkingDirectory_B);

        ////        I = I + 1;

        ////    }

        ////    // DELETE The root directory at Loop finished

        ////    bool WithCopy = false;
        ////    DeleteDirectoryWithCopy(WorkingDirectory_A, WithCopy, "");


        ////    Success = true;

        ////    return Success;
        ////}

        //////
        ////// Move file From Archive directory to the Origin 
        //////
        ////public bool Undo_MoveFile_From_Archive_ToOrigin_Directory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InWorkingDirectory_B)
        ////{

        ////    bool Success = false;
        ////    string CopiedFile;


        ////    // Take all from 
        ////    // Read one by one and 

        ////    try
        ////    {
        ////        // Find files in Archive
        ////        if (Directory.Exists(InWorkingDirectory_B))
        ////        {
        ////            string[] allFiles = Directory.GetFiles(InWorkingDirectory_B, "*.*");
        ////            if (allFiles.Length > 0)
        ////            {
        ////                foreach (string fileToCopy in allFiles)
        ////                {

        ////                    //string DestinationfileId = Path.GetFileName(fileToCopy);

        ////                    RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

        ////                    Msf.ReadReconcSourceFilesByFileId(InTableA);

        ////                    string DestinationDirectory = Msf.SourceDirectory;

        ////                    // bool WSuccess = Jt.CreateDirectory(WorkingDirectory);


        ////                    CopiedFile = CopyFileFromOneDirectoryToAnother(fileToCopy, DestinationDirectory);

        ////                    if (CopySuccess == true)
        ////                    {
        ////                        DeleteFileFromDirectory(fileToCopy);
        ////                    }
        ////                }
        ////            }
        ////            string[] allFiles_2 = Directory.GetFiles(InWorkingDirectory_B, "*.*");
        ////            if (allFiles_2.Length == 0)
        ////            {
        ////                bool WithCopy = false;
        ////                DeleteDirectoryWithCopy(InWorkingDirectory_B, WithCopy, "");
        ////            }

        ////            Success = true;
        ////        }


        ////    }
        ////    catch (Exception exf)
        ////    {
        ////        CatchDetails(exf);
        ////    }

        ////    return Success;

        ////}

        //////
        ////// Create Directory 
        //////
        ////public bool CreateDirectory(string InPathString)
        ////{
        ////    bool Success = false;

        ////    try
        ////    {

        ////        if (System.IO.Directory.Exists(InPathString))
        ////        {
        ////            //System.IO.Directory.CreateDirectory(InPathString);
        ////            Success = true;
        ////        }
        ////        else
        ////        {
        ////            System.IO.Directory.CreateDirectory(InPathString);
        ////            Success = true;
        ////        }

        ////        // System.IO.Directory.Delete(InPathString);

        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);
        ////        Success = false;

        ////    }
        ////    // return FullDestination;

        ////    return Success;
        ////}

        //////
        ////// Delete Directory 
        //////
        ////public bool DeleteDirectoryWithCopy(string InPathString, bool InWithCopy, string InCopyDestination)
        ////{
        ////    bool Success = false;
        ////    int Count = 0;

        ////    try
        ////    {

        ////        if (Directory.Exists(InPathString))
        ////        {
        ////            //  DirectoryInfo parentDirectory = new DirectoryInfo(InPathString);

        ////            //  parentDirectory.Attributes = FileAttributes.Normal;
        ////            //MessageBox.Show("Directory Exists" + InPathString);

        ////            string[] files = Directory.GetFiles(InPathString);

        ////            if (files.Length > 0)
        ////            {
        ////                foreach (string file in files)
        ////                {
        ////                    Count = Count + 1;
        ////                    //MessageBox.Show("Before Setting Atributes to Normal:" + file);
        ////                    // File.SetAttributes(file, FileAttributes.Normal);
        ////                    if (InWithCopy == true)
        ////                    {
        ////                        //if (Count<5)
        ////                        //MessageBox.Show("Copy call routine starts for:" + file); 
        ////                        CopyFileFromOneDirectoryToAnother(file, InCopyDestination);
        ////                    }
        ////                    if (CopySuccess == true || InWithCopy == false)
        ////                    {
        ////                        //if (Count < 5)
        ////                        //MessageBox.Show("Copy file Was Succesful:" + file);
        ////                        File.Delete(file);
        ////                    }
        ////                    else
        ////                    {
        ////                        //if (Count < 5)
        ////                        MessageBox.Show("Copy file Was UN-Succesful:" + file);
        ////                    }

        ////                }

        ////                string[] files2 = Directory.GetFiles(InPathString);

        ////                if (files2.Length == 0)
        ////                {
        ////                    Directory.Delete(InPathString);
        ////                    Success = true;
        ////                }


        ////            }
        ////            else
        ////            {
        ////                Directory.Delete(InPathString);
        ////                Success = true;

        ////            }

        ////        }
        ////        else
        ////        {
        ////            MessageBox.Show("Directory doesnot exist" + InPathString);
        ////        }

        ////        //System.IO.Directory.Delete(InPathString);

        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);
        ////        Success = false;

        ////    }
        ////    // return FullDestination;

        ////    return Success;
        ////}

        //////
        ////// Delete Records within directory
        //////
        ////public bool DeleteFilesWithinDirectory(string InPathString)
        ////{
        ////    bool Success = false;
        ////    int Count = 0;

        ////    try
        ////    {

        ////        if (Directory.Exists(InPathString))
        ////        {
        ////            //  DirectoryInfo parentDirectory = new DirectoryInfo(InPathString);

        ////            //  parentDirectory.Attributes = FileAttributes.Normal;
        ////            //MessageBox.Show("Directory Exists" + InPathString);

        ////            string[] files = Directory.GetFiles(InPathString);

        ////            if (files.Length > 0)
        ////            {
        ////                foreach (string file in files)
        ////                {
        ////                    Count = Count + 1;

        ////                    File.Delete(file);

        ////                }
        ////            }
        ////        }
        ////        else
        ////        {
        ////            MessageBox.Show("Directory doesnot exist" + InPathString);
        ////        }

        ////        //System.IO.Directory.Delete(InPathString);

        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);
        ////        Success = false;

        ////    }
        ////    // return FullDestination;

        ////    return Success;
        ////}

        //////private static void DeleteDirectoryFiles(string target_dir)
        //////{
        //////    string[] files = Directory.GetFiles(target_dir);
        //////    string[] dirs = Directory.GetDirectories(target_dir);

        //////    foreach (string file in files)
        //////    {
        //////        File.SetAttributes(file, FileAttributes.Normal);
        //////        File.Delete(file);
        //////    }

        //////    foreach (string dir in dirs)
        //////    {
        //////        DeleteDirectoryFiles(dir);
        //////    }
        //////}

        //////
        ////// Copy from Archiving to working directory
        //////
        ////public bool CopySuccess;

        ////public string CopyFileFromOneDirectoryToAnother(string InSourceFile, string InTargetDirectory)
        ////{

        ////    // string fileName_In;
        ////    string FullDestination = "";
        ////    CopySuccess = false;

        ////    try
        ////    {
        ////        //   string fileToCopy = "c:\\myFolder\\myFile.txt";
        ////        string fileToCopy = InSourceFile;
        ////        //string destinationDirectory = "c:\\myDestinationFolder\\";
        ////        string destinationDirectory = InTargetDirectory;

        ////        string DestinationfileId = Path.GetFileName(fileToCopy);

        ////        FullDestination = Path.Combine(destinationDirectory, DestinationfileId);

        ////        //MessageBox.Show("Before Actual Copy:" + fileToCopy);
        ////        //MessageBox.Show("Full Destination.." + FullDestination);
        ////        //  File.SetAttributes(fileToCopy, FileAttributes.Normal);

        ////        File.Copy(fileToCopy, FullDestination);

        ////        CopySuccess = true;

        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);

        ////    }
        ////    return FullDestination;
        ////}

        //////
        ////// Delete File 
        //////
        ////public bool DeleteFileFromDirectory(string InSourceFile)
        ////{
        ////    bool Success = false;
        ////    try
        ////    {
        ////        File.Delete(InSourceFile);
        ////        Success = true;
        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);

        ////    }

        ////    return Success;

        ////}

        //////
        ////// Copy from Archiving to working directory
        //////
        ////string stringToPrint;

        ////public void PrintFromSourceJournal(string InSourceFile, string TargetDirectory)
        ////{

        ////    // string fileName_In;


        ////    try
        ////    {

        ////        using (FileStream stream = new FileStream(InSourceFile, FileMode.Open))
        ////        using (StreamReader reader = new StreamReader(stream))
        ////        {
        ////            stringToPrint = reader.ReadToEnd();
        ////        }

        ////        //int charactersOnPage = 0;
        ////        //int linesPerPage = 0;

        ////        //// Sets the value of charactersOnPage to the number of characters 
        ////        //// of stringToPrint that will fit within the bounds of the page.
        ////        //e.Graphics.MeasureString(stringToPrint, this.Font,
        ////        //    e.MarginBounds.Size, StringFormat.GenericTypographic,
        ////        //    out charactersOnPage, out linesPerPage);

        ////        //// Draws the string within the bounds of the page
        ////        //e.Graphics.DrawString(stringToPrint, this.Font, Brushes.Black,
        ////        //    e.MarginBounds, StringFormat.GenericTypographic);

        ////        //// Remove the portion of the string that has been printed.
        ////        //stringToPrint = stringToPrint.Substring(charactersOnPage);

        ////        //// Check to see if more pages are to be printed.
        ////        //e.HasMorePages = (stringToPrint.Length > 0);




        ////    }
        ////    catch (Exception ex)
        ////    {

        ////        CatchDetails(ex);

        ////    }

        ////}


        //
        // READ Merge File based on catd no  
        //
        public void ReadJournalTextDataTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            try
            {
                //Int32 LastDigit = InTraceNumber % 10;

                //if (LastDigit == 0)
                //{
                //    // OK
                //}
                //else
                //{
                //    InTraceNumber = (InTraceNumber - LastDigit) + 1;
                //}


                switch (InOperator)
                {
                    case "ETHNCY2N":
                        {
                            WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
                            break;
                        }
                    case "CRBAGRAA":
                        {
                            WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
                            break;
                        }
                    default:
                        {
                            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
                            break;
                        }
                }


                TraceJournalLinesDataTable = new DataTable();
                TraceJournalLinesDataTable.Clear();

                TotalSelected = 0;

                // DATA TABLE ROWS DEFINITION 
                TraceJournalLinesDataTable.Columns.Add("LineNo", typeof(int));
                TraceJournalLinesDataTable.Columns.Add("Journal Line", typeof(string));

                SqlString = "SELECT ISNULL(TxtLine, '') AS TxtLine "
                   + " FROM " + WJournalId
                   + " WHERE " + InSelectionCriteria
                   + " Order by TraceNumber, RuId ";
                using (SqlConnection conn =
                              new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            //cmd.Parameters.AddWithValue("@Operator", InOperator);
                            //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            //cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {

                                RecordFound = true;

                                TotalSelected = TotalSelected + 1;

                                //BankId = (string)rdr["BankId"];
                                //AtmNo = (string)rdr["AtmNo"];

                                //TraceNumber = (int)rdr["TraceNumber"];

                                TxtLine = (string)rdr["TxtLine"];

                                //TransDate = (DateTime)rdr["TransDate"];

                                //FuId = (int)rdr["FuId"];
                                //RuId = (int)rdr["RuId"];
                                //Operator = (string)rdr["Operator"];


                                //FILL TABLE 
                                DataRow RowSelected = TraceJournalLinesDataTable.NewRow();

                                RowSelected["LineNo"] = TotalSelected;
                                RowSelected["Journal Line"] = TxtLine;

                                // ADD ROW
                                TraceJournalLinesDataTable.Rows.Add(RowSelected);
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
            catch (Exception ex)
            {

                //  conn.Close();

                CatchDetails(ex);

            }
        }


        //
        // READ Merge File based on catd no  
        //
        public void ReadJournalTextDataTableRange(string InOperator, string InAtmNo, int InTraceNumber, int InVariance)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InOperator == "ETHNCY2N")
            {
                WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }
            if (InOperator == "CRBAGRAA")
            {
                WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }

            int FromTrace;
            int ToTrace;

            FromTrace = InTraceNumber - (InVariance * 10);
            ToTrace = InTraceNumber + (InVariance * 10);

            //Int32 LastDigit = InTraceNumber % 10;

            //if (LastDigit == 0)
            //{
            //    // OK
            //}
            //else
            //{
            //    InTraceNumber = (InTraceNumber - LastDigit) + 1;
            //}

            TraceJournalLinesDataTable = new DataTable();
            TraceJournalLinesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TraceJournalLinesDataTable.Columns.Add("LineNo", typeof(int));
            TraceJournalLinesDataTable.Columns.Add("Journal Line", typeof(string));

            SqlString = "SELECT ISNULL(TxtLine, '') AS TxtLine "
               + " FROM " + WJournalId
               + " WHERE Operator=@Operator AND AtmNo=@AtmNo AND (TraceNumber >= @FromTrace AND TraceNumber <= @ToTrace) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@FromTrace", FromTrace);
                        cmd.Parameters.AddWithValue("@ToTrace", ToTrace);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            //BankId = (string)rdr["BankId"];
                            //AtmNo = (string)rdr["AtmNo"];

                            //TraceNumber = (int)rdr["TraceNumber"];

                            TxtLine = (string)rdr["TxtLine"];

                            //TransDate = (DateTime)rdr["TransDate"];

                            //FuId = (int)rdr["FuId"];
                            //RuId = (int)rdr["RuId"];
                            //Operator = (string)rdr["Operator"];


                            //FILL TABLE 
                            DataRow RowSelected = TraceJournalLinesDataTable.NewRow();

                            RowSelected["LineNo"] = TotalSelected;
                            RowSelected["Journal Line"] = TxtLine;

                            // ADD ROW
                            TraceJournalLinesDataTable.Rows.Add(RowSelected);
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
        // READ Merge File based on Unique trace no  
        //
        public void ReadJournalTextByTrace(string InOperator, string InAtmNo, int InTraceNumber
              , DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //if (InOperator == "BCAIEGCX")
            //{
                return;
            //}


            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            //}
            if (InOperator == "CRBAGRAA")
            {
                WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }
            else
            {
                WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }

            Int32 LastDigit = InTraceNumber % 10;

            if (LastDigit == 0)
            {
                // OK
            }
            else
            {
                InTraceNumber = (InTraceNumber - LastDigit) + 1;
            }

            SqlString = "SELECT "
                + " BankId AS BankId "
                + " ,AtmNo AS AtmNo "
                 + " ,TraceNo AS TraceNo "
                + " ,TransDate AS TransDate "
                 + " ,TransTime AS TransTime "
                + " ,TraceNumber AS TraceNumber"
                 + " ,ISNULL(TxtLine, '') AS TxtLine "
                   + " ,FuId AS FuId "
                + " ,RuId AS RuId"
                 + " ,Operator AS Operator "
               + " FROM " + WJournalId
               + " WHERE Operator=@Operator AND AtmNo=@AtmNo"
               + " AND TraceNumber = @TraceNumber AND TransDate = @TransDate"
               + " Order by RuId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];

                            TraceNo = (string)rdr["TraceNo"];

                            TransDate = (DateTime)rdr["TransDate"];

                            TransTime = (TimeSpan)rdr["TransTime"];

                            TraceNumber = (int)rdr["TraceNumber"];

                            TxtLine = (string)rdr["TxtLine"];

                            FuId = (int)rdr["FuId"];
                            RuId = (int)rdr["RuId"];
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

        public void ReadJournalTextByTrace_HST(string InOperator, string InAtmNo, int InTraceNumber
       , DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //if (InOperator == "BCAIEGCX")
            //{
            return;
            //}


            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            //}
            if (InOperator == "CRBAGRAA")
            {
                WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }
            else
            {
                WJournalId = "[ATM_MT_Journals_AUDI_HST].[dbo].[tblHstEjText]";
            }

            Int32 LastDigit = InTraceNumber % 10;

            if (LastDigit == 0)
            {
                // OK
            }
            else
            {
                InTraceNumber = (InTraceNumber - LastDigit) + 1;
            }

            SqlString = "SELECT "
                + " BankId AS BankId "
                + " ,AtmNo AS AtmNo "
                 + " ,TraceNo AS TraceNo "
                + " ,TransDate AS TransDate "
                 + " ,TransTime AS TransTime "
                + " ,TraceNumber AS TraceNumber"
                 + " ,ISNULL(TxtLine, '') AS TxtLine "
                   + " ,FuId AS FuId "
                + " ,RuId AS RuId"
                 + " ,Operator AS Operator "
               + " FROM " + WJournalId
               + " WHERE Operator=@Operator AND AtmNo=@AtmNo"
               + " AND TraceNumber = @TraceNumber AND TransDate = @TransDate"
               + " Order by RuId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];

                            TraceNo = (string)rdr["TraceNo"];

                            TransDate = (DateTime)rdr["TransDate"];

                            TransTime = (TimeSpan)rdr["TransTime"];

                            TraceNumber = (int)rdr["TraceNumber"];

                            TxtLine = (string)rdr["TxtLine"];

                            FuId = (int)rdr["FuId"];
                            RuId = (int)rdr["RuId"];
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

        public void ReadJournalText_HST_ByDate_Find_Fuid(string InOperator, string InAtmNo
                                                                , DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
         
            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            
            SqlString = "SELECT TOP (1) "
                + " BankId AS BankId "
                + " ,AtmNo AS AtmNo "    
                   + " ,FuId AS FuId "
               + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + "  AND TranDate <= @TranDate"
               + " Order By TranDate DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TranDate", InTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];

                           /// TranDate = (DateTime)rdr["TransDate"];

                            //TransTime = (TimeSpan)rdr["TransTime"];

                            //TraceNumber = (int)rdr["TraceNumber"];

                            //TxtLine = (string)rdr["TxtLine"];

                            FuId = (int)rdr["FuId"];
                            //RuId = (int)rdr["RuId"];
                            //Operator = (string)rdr["Operator"];
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
        // Call Pambos store procedure to create Journal Lines 
        //
        public void CreateJournalLinesBasedonGivenFuid(string InOperator, string InSignedId, string InAtmNo
                , int InFuid)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int ReturnCode = -1;
            string WJournalTxtFile = "";
            string EjournalTypeId = "";

            string ErrorText = "";
            string ErrorReference = "";

           
            // GET Journal Id 
            //
            RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();
            
            Flog.GetRecordByFuid(InFuid);
            if (Flog.RecordFound == true)
            {
                if (Flog.SystemOfOrigin == "ATMs")
                {
                    WJournalTxtFile = Flog.ArchivedPath;
                }
            }
            else
            {
                // NOT Found 
                ErrorFound = true;
                return;
            }
            //
            // Journal Type 
            //
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(InAtmNo);
            EjournalTypeId = Ac.EjournalTypeId;

            string connectionString_AUDI = ConfigurationManager.ConnectionStrings
                  ["JournalsConnectionString_AUDI"].ConnectionString;

            //#region Create a new file with sequence number in front of each line
            ////Add sequence number in front of each line of the line
            string jlnFullPathName;
            RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            jlnFullPathName = Jrt.ConvertJournal(WJournalTxtFile); // Converted File 
                                                                   // LineCount = Jrt.LineCounter;
                                                                   //#endregion
                                                                   //WJournalTxtFile = WJournalTxtFile + ".jln";

            string SPName = "[ATM_MT_Journals_AUDI].[dbo].[stp_Create_Journal_Text]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_AUDI))
            {
                try
                {
                    int ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@AtmNo", InAtmNo));
                    cmd.Parameters.Add(new SqlParameter("@Fuid", InFuid));
                    cmd.Parameters.Add(new SqlParameter("@FullPath", jlnFullPathName));
                    //cmd.Parameters.Add(new SqlParameter("@FullPath", WJournalTxtFile));
                    cmd.Parameters.Add(new SqlParameter("@JournalType", EjournalTypeId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                       File.Delete(jlnFullPathName);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


        }

        //
      
        //
        // Fill Up Table from one Ruid to Ruid
        //
        public void ReadJournalAndFillTableFrom_Fuid_Ruid_To_Fuid_Ruid(string InOperator, string InSignedId, string InAtmNo
                , int InFuid_A, int InRuid_A, int InFuid_B, int InRuid_B, int InMode
               )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string OrderBy = "";

            JournalLines = new DataTable();
            JournalLines.Clear();

            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";
            
            if (InMode == 5 || InMode == 7)
            {
                // DEFINE OR , AND 
                string Selection = "";
                if (InFuid_A == InFuid_B)
                {
                    Selection = "  ((Fuid = @Fuid_A AND Ruid>= @Ruid_A) AND (Fuid = @Fuid_B AND Ruid <= @Ruid_B)) ";
                    OrderBy = "ORDER by Fuid ASC , RuId ASC"; // B to come second 
                }

                if (InFuid_A != InFuid_B)
                {
                    Selection = "  ((Fuid = @Fuid_A AND Ruid>= @Ruid_A) OR (Fuid = @Fuid_B AND Ruid <= @Ruid_B)) ";
                    if (InFuid_B > InFuid_A) OrderBy = "ORDER by Fuid ASC , RuId ASC"; // B to come second 
                    else OrderBy = "ORDER by Fuid Desc, ruid Asc"; // B to come first 
                }

                SqlString = "SELECT AtmNo,ISNULL(fuid, 0) AS Journal_id"
                        + ", ISNULL(Ruid, 0) AS Journal_LN"
                        + ", ISNULL(TxtLine, '') AS TxtLine "
                  + " FROM " + WJournalId
                  + " WHERE "
                  + Selection
                  + OrderBy;
            }

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        if (InMode == 5 || InMode == 7)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Fuid_A", InFuid_A);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Ruid_A", InRuid_A);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Fuid_B", InFuid_B);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Ruid_B", InRuid_B);
                        }
                        //}

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(JournalLines);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (JournalLines.Rows.Count > 0)
            {
                // REPORT TABLE
                CreateReportTable(InSignedId);

            }

        }

        public int ReadJournalByFuidAndFindNumberOfLines(int InFuid) 
             
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string OrderBy = "";


            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";

            SqlString = "SELECT TOP(1) RUID "
                  + " FROM " + WJournalId
                  + " WHERE FuId = @FuId "
                  + "Order By RUID DESC " ;


            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FuId", InFuid);
                      
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                          
                            RuId = (int)rdr["RuId"];
                          
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

            return RuId; 

        }
        //
        // Fill Up Table from one Ruid to Ruid
        //
        public void ReadJournalAndFillTableFrom_Fuid(string InOperator, string InSignedId, string InAtmNo
                , int InFuid, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JournalLines = new DataTable();
            JournalLines.Clear();

            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            //}
            //if (InOperator == "CRBAGRAA")
            //{
            //    WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            //}

            //if (InOperator == "BCAIEGCX")
            //{
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText]";
            //}

            SqlString = "SELECT ISNULL(fuid, 0) AS Journal_id, ISNULL(Ruid, 0) AS Journal_LN,  AtmNo, ISNULL(TxtLine, '') AS TxtLine, ISNULL(TransDate,'1901-01-01') AS Tr_Date, "
                 + "   ISNULL(TraceNo, 0) AS TraceNo, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                 + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                     + " FROM " + WJournalId
                                  + " WHERE FuId = @FuId "
                                   + " Order by RuId ";
            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FuId", InFuid);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(JournalLines);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (JournalLines.Rows.Count > 0)
            {
                // REPORT TABLE
                CreateReportTable(InSignedId);

            }

        }

        //
        // Fill Up Table for fuid
        //
        public void ReadJournalAndFillTableFrom_Fuid_Short(string InOperator, string InSignedId, string InAtmNo
                , int InFuid, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JournalLines = new DataTable();
            JournalLines.Clear();

            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            //}
            //if (InOperator == "CRBAGRAA")
            //{
            //    WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            //}

            //if (InOperator == "BCAIEGCX")
            //{
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";
            //}

            SqlString = "SELECT AtmNo,ISNULL(fuid, 0) AS Journal_id"
                        + ", ISNULL(Ruid, 0) AS Journal_LN"
                        + ", ISNULL(TxtLine, '') AS TxtLine "
                     //      + ",  ISNULL(TxtLine, '') AS TxtLine "
                     + " FROM " + WJournalId
                                  + " WHERE FuId = @FuId "
                                   + " Order by RuId ";
            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FuId", InFuid);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(JournalLines);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (JournalLines.Rows.Count > 0)
            {
                // REPORT TABLE
                CreateReportTable(InSignedId);

            }

        }

        // Creat Print File 
        private void CreateReportTable(string InSignedId)
        {
            int Journal_id;
            int Journal_LN;
            string TxtLine;
            DataTable JournalLinesSelected = new DataTable();
            JournalLinesSelected = new DataTable();
            JournalLinesSelected.Clear();

            JournalLinesSelected.Columns.Add("UserId", typeof(string));
            JournalLinesSelected.Columns.Add("Journal_id", typeof(int));
            JournalLinesSelected.Columns.Add("Journal_LN", typeof(int));
            JournalLinesSelected.Columns.Add("TxtLine", typeof(string));
            //JournalLinesSelected.Columns.Add("TraceNo", typeof(string));

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport75(InSignedId);

            int I = 0;

            while (I <= (JournalLines.Rows.Count - 1))
            {

                //          [atmno]
                //,[fuid]
                //,[ruid]
                //,[txtline]
                Journal_id = (int)JournalLines.Rows[I]["Journal_id"];
                Journal_LN = (int)JournalLines.Rows[I]["Journal_LN"];
                TxtLine = (string)JournalLines.Rows[I]["TxtLine"];

                // Fill In Table
                //
                DataRow RowSelected = JournalLinesSelected.NewRow();

                RowSelected["UserId"] = InSignedId;
                RowSelected["Journal_id"] = Journal_id;
                RowSelected["Journal_LN"] = Journal_LN;
                RowSelected["TxtLine"] = TxtLine;


                // ADD ROW
                JournalLinesSelected.Rows.Add(RowSelected);

                I++; // Read Next entry of the table 

            }

            //
            //Insert Records For Report WReport75
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionString))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport75]";

                        foreach (var column in JournalLinesSelected.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(JournalLinesSelected);
                    }

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();

                    CatchDetails(ex);
                }


        }
        public DataTable DataTableTXN_Balancing = new DataTable();
        public DataTable DataTableJournalsSourceStats_2 = new DataTable();
        public DataTable DataTableJournalsSourceStats_1 = new DataTable();
      
        public int TotalTXNS;
        public int SuccessfulTotal;
        public int BadResponse;
        public int GoodTransfersTotal;
        public int BadTransfersTotal;
        public int FlexTimeOutTotal;
        // READ AND FILL UP TABLE through JobCycleNo
        public void ReadDataTableForResponseCodes(int InJobCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableJournalsSourceStats_1 = new DataTable();
            DataTableJournalsSourceStats_1.Clear();

            //// DATA TABLE ROWS DEFINITION 
            ///
            string SqlString = " SELECT Source as ResponseCode , TransactionType,count(*) as Number "
                               + " FROM[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                              + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle  AND "
                              + " (Source = '000'  "
                              + " OR Source = '005'  "
                              + " OR Source = '008'  "
                              + " OR Source = '091'  "
                              + " OR  Source = '096'  "
                              + " OR Source = '103' "
                              + " OR Source = '105' "
                               + " OR Source = '900' "
                               + " ) "
                             + " group by Source, TransactionType  "
                             + " ORDER By Source";

            /*
             Response	Description	Number
            105	RCPT PRNTR OUT PAPER     	933
            000	Successful	              15582
            096	SYSTEM ERROR             	305
            005	FLEX TIMEOUT               1253
            091	ISS TIMEOUT              	291
            008	ISS TIMEOUT	                 26
            900	HOST DOWN                	304
            103	UNABLE TO DISP             2437
             */


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@LoadedAtRMCycle", InJobCycleNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        //sqlAdapt.UpdateCommand.CommandTimeout = 300;  // seconds

                        sqlAdapt.Fill(DataTableJournalsSourceStats_1);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            string ResponseCode;
            string Description;
            int TransactionType;
            int CountNumber;

            TotalTXNS = 0;
            SuccessfulTotal = 0;
            GoodTransfersTotal = 0;
            BadTransfersTotal = 0;
            FlexTimeOutTotal = 0;

            DataTableJournalsSourceStats_2 = new DataTable();
            DataTableJournalsSourceStats_2.Clear();

            // DATA TABLE ROWS DEFINITION 
            DataTableJournalsSourceStats_2.Columns.Add("ResponseCode", typeof(string));
            DataTableJournalsSourceStats_2.Columns.Add("Case Description", typeof(string));
            DataTableJournalsSourceStats_2.Columns.Add("Transaction Type", typeof(string));
            DataTableJournalsSourceStats_2.Columns.Add("Count", typeof(int));

            int I = 0;

            while (I <= (DataTableJournalsSourceStats_1.Rows.Count - 1))
            {
                // 

                ResponseCode = (string)DataTableJournalsSourceStats_1.Rows[I]["ResponseCode"];
                //Description = (string)DataTableJournalsSourceStats_1.Rows[I]["Description"];
                TransactionType = (int)DataTableJournalsSourceStats_1.Rows[I]["TransactionType"];
                CountNumber = (int)DataTableJournalsSourceStats_1.Rows[I]["Number"];

                TotalTXNS = TotalTXNS + CountNumber;

                if (ResponseCode == "000")
                {
                    SuccessfulTotal = SuccessfulTotal + CountNumber;
                }

                DataRow RowGrid = DataTableJournalsSourceStats_2.NewRow();

                RowGrid["ResponseCode"] = ResponseCode;
                /*
             Response	Description	Number
            105	RCPT PRNTR OUT PAPER     	933
            000	Successful	              15582
            096	SYSTEM ERROR             	305
            005	FLEX TIMEOUT               1253
            091	ISS TIMEOUT              	291
            008	ISS TIMEOUT	                 26
            900	HOST DOWN                	304
            103	UNABLE TO DISP             2437
             */

                switch (ResponseCode)
                {
                    case "000": // 
                        {
                            RowGrid["Case Description"] = "Successful";
                            break;
                        }
                    case "105": // 
                        {
                            RowGrid["Case Description"] = "RCPT PRNTR OUT PAPER";
                            break;
                        }
                    case "096": // 
                        {
                            RowGrid["Case Description"] = "System Error";
                            break;
                        }
                    case "005": // 
                        {
                            RowGrid["Case Description"] = "FLEX TIMEOUT";
                            FlexTimeOutTotal = FlexTimeOutTotal + CountNumber;
                            break;
                        }
                    case "091": // 
                        {
                            RowGrid["Case Description"] = "ISS TIMEOUT";
                            break;
                        }
                    case "008": // 
                        {
                            RowGrid["Case Description"] = "ISS TIMEOUT";
                            break;
                        }
                    case "900": // 
                        {
                            RowGrid["Case Description"] = "HOST DOWN";
                            break;
                        }
                    case "103": // 
                        {
                            RowGrid["Case Description"] = "UNABLE TO DISP ";
                            break;
                        }

                    default:
                        {
                            RowGrid["Case Description"] = "Not Specified";
                            break;
                        }
                }

                switch (TransactionType)
                {
                    case 99: // From physical check
                        {
                            RowGrid["Transaction Type"] = "Other Than Money ";
                            break;
                        }
                    case 11: // 
                        {
                            RowGrid["Transaction Type"] = "WithDrawls ";
                            break;
                        }
                    case 33: // 
                        {
                            if (ResponseCode == "000")
                            {
                                GoodTransfersTotal = GoodTransfersTotal + CountNumber;
                            }
                            else
                            {
                                BadTransfersTotal = BadTransfersTotal + CountNumber;
                            }

                            RowGrid["Transaction Type"] = "Transfers ";
                            break;
                        }
                    case 23: // 
                        {
                            RowGrid["Transaction Type"] = "Deposits BNA ";
                            break;
                        }
                    default:
                        {
                            RowGrid["Transaction Type"] = "Mot Specified";
                            break;
                        }
                }

                RowGrid["Count"] = CountNumber;

                DataTableJournalsSourceStats_2.Rows.Add(RowGrid);

                I++;
            }



            //   InsertWReport72(InSignedId);
        }

        public void ReadTXN_BALANCING(string InATM_NO, DateTime InDateTmFrom, DateTime InDateTmTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableTXN_Balancing = new DataTable();
            DataTableTXN_Balancing.Clear();

            //// DATA TABLE ROWS DEFINITION 
            ///
            string SqlString = " SELECT * "      
                              + "  FROM [ATM_MT_Journals_AUDI].[dbo].[TransactionSummary_NEW] "
                              + " WHERE ATMNo = @ATM_NO "
                              + " AND trandatetime > @trandatetime_From AND trandatetime<= @trandatetime_To "
                             + " ORDER By trandatetime,  trace ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ATM_NO", InATM_NO);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_From", InDateTmFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_To", InDateTmTo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        //sqlAdapt.UpdateCommand.CommandTimeout = 300;  // seconds

                        sqlAdapt.Fill(DataTableTXN_Balancing);

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

        public void ReadTXN_OriginalInError(string InATM_NO, DateTime InDateTmFrom, DateTime InDateTmTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableTXN_Balancing = new DataTable();
            DataTableTXN_Balancing.Clear();

            //// DATA TABLE ROWS DEFINITION 
            ///
            string SqlString = " SELECT  "
                              + "SeqNo"
                               + ",AtmNo"
                                + ",FuID"
                                 + ",Trandatetime"
                                  + ",TranDesc"
                                   + ",Currency"
                                    + ",CAmount"
                                     + ",Trace"
                                      + ",AUTHNUM"
                                       + ",UTRNNO"
                                        + ",CardNum"
                                          + ",Comments"
                                           + ",PowerInterup"
                                            + ",Acct1"
                                             + ",Result"
                                              + ",RON1"
                                               + ",RON5"
                                                + ",RON10"
                                                   + ",RON20"
                                                      + ",RON50"
                                                       + ",RON100"
                                                        + ",RON200"
                                                        + ",RON500"
                              + "  FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                              + " WHERE AtmNo = @ATM_NO AND  result <> 'OK' AND (TranDesc = 'DEPOSIT' OR TranDesc = 'CHECK STATUS') "
                              + " AND trandatetime > @trandatetime_From AND trandatetime<= @trandatetime_To "
                             + " ORDER by Trandatetime ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ATM_NO", InATM_NO);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_From", InDateTmFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@trandatetime_To", InDateTmTo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        //sqlAdapt.UpdateCommand.CommandTimeout = 300;  // seconds

                        sqlAdapt.Fill(DataTableTXN_Balancing);

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


        public DataTable DataTableJournalsWrongDeposits = new DataTable();
        // READ Deposits and identify the ones with wrong Repl Cycle 
        public void ReadDataTableForWrongReplAtDeposits(int InJobCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // Examine Last 15 Cycles
            int WJobCycleNo = InJobCycleNo - 5; 

            DataTableJournalsWrongDeposits = new DataTable();
            DataTableJournalsWrongDeposits.Clear();

            DataTableJournalsWrongDeposits.Columns.Add("WATMNo", typeof(string));
            DataTableJournalsWrongDeposits.Columns.Add("WReplCycle", typeof(int));
            DataTableJournalsWrongDeposits.Columns.Add("WLoadedAtRMCycle", typeof(int));
            DataTableJournalsWrongDeposits.Columns.Add("WSM_DATE_TIME", typeof(DateTime));
            
            ////JournalLinesSelected.Columns.Add("Journal_LN", typeof(int));
            ////JournalLinesSelected.Columns.Add("TxtLine", typeof(string));
            ///  ,[FuId]
            //,[AtmNo]
            //,[SM_DATE_TIME]
            //,[TYPE]
            //,[Currency]
            //,[FaceValue]
            //,[CASSETTE]
            //,[RETRACT]
            //,[RECYCLED]
            //,[NCR_DepositsDispensed]
            //,[ReplCycle]
            //,[Processed]
            //,[LoadedAtRMCycle]

            //// DATA TABLE ROWS DEFINITION 
            ///
            string SqlString = " SELECT AtmNo , ReplCycle, FuId, LoadedAtRMCycle, SM_DATE_TIME "
                               + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
                              + " WHERE LoadedAtRMCycle > @LoadedAtRMCycle   " 
                             + " ORDER By AtmNo, ReplCycle, Fuid ";

            //AtmNo , ReplCycle, FuId
            string WAtmNo = "";
            int WReplCycle = 0;
            int WFuId = 0;
            int WLoadedAtRMCycle = 0;
            DateTime WSM_DATE_TIME; 

            string PreviousWAtmNo = "";
            int PreviousWReplCycle = 0;
            int PreviousWFuId = 0;

            bool NewWAtmNo = false;
            bool NewWReplCycle = false;
            bool NewWFuId = false;

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", WJobCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            
                            //AtmNo , ReplCycle, FuId
                            WAtmNo = (string)rdr["AtmNo"];
                            WReplCycle = (int)rdr["ReplCycle"];
                            WFuId = (int)rdr["FuId"];
                            WLoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
                            WSM_DATE_TIME = (DateTime)rdr["SM_DATE_TIME"];

                            if (WAtmNo != PreviousWAtmNo)
                            {
                                PreviousWAtmNo = WAtmNo;
                                NewWAtmNo = true;
                                PreviousWReplCycle = WReplCycle;
                                PreviousWFuId = WFuId;
                            }
                            else
                            {
                                NewWAtmNo = false;
                            }
                            if (NewWAtmNo == false & WReplCycle != PreviousWReplCycle)
                            {
                                PreviousWReplCycle = WReplCycle;
                                NewWReplCycle = true;
                                PreviousWFuId = WFuId;
                            }
                            else
                            {
                                NewWReplCycle = false;
                            }
                            if (NewWAtmNo == false & NewWReplCycle == false & WFuId != PreviousWFuId)
                            {
                                PreviousWFuId = WFuId;
                                NewWFuId = true;

                                // HERE IS THE PROBLEM 
                                DataRow RowSelected = DataTableJournalsWrongDeposits.NewRow();

                                RowSelected["WATMNo"] = WAtmNo;
                                RowSelected["WReplCycle"] = WReplCycle;
                                RowSelected["WLoadedAtRMCycle"] = WLoadedAtRMCycle;
                                RowSelected["WSM_DATE_TIME"] = WSM_DATE_TIME;

                                DataTableJournalsWrongDeposits.Rows.Add(RowSelected);

                            }
                            else
                            {
                                NewWFuId = false;
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


            //string ResponseCode;
            //string Description;
            //int TransactionType;
            //int CountNumber;

            //TotalTXNS = 0;
            //SuccessfulTotal = 0;
            //GoodTransfersTotal = 0;
            //BadTransfersTotal = 0;
            //FlexTimeOutTotal = 0;

            //DataTableJournalsSourceStats_2 = new DataTable();
            //DataTableJournalsSourceStats_2.Clear();

            //// DATA TABLE ROWS DEFINITION 
            //DataTableJournalsSourceStats_2.Columns.Add("ResponseCode", typeof(string));
            //DataTableJournalsSourceStats_2.Columns.Add("Case Description", typeof(string));
            //DataTableJournalsSourceStats_2.Columns.Add("Transaction Type", typeof(string));
            //DataTableJournalsSourceStats_2.Columns.Add("Count", typeof(int));

            //int I = 0;

            //while (I <= (DataTableJournalsSourceStats_1.Rows.Count - 1))
            //{
            //    // 

            //    ResponseCode = (string)DataTableJournalsSourceStats_1.Rows[I]["ResponseCode"];
            //    //Description = (string)DataTableJournalsSourceStats_1.Rows[I]["Description"];
            //    TransactionType = (int)DataTableJournalsSourceStats_1.Rows[I]["TransactionType"];
            //    CountNumber = (int)DataTableJournalsSourceStats_1.Rows[I]["Number"];

            //    TotalTXNS = TotalTXNS + CountNumber;

            //    if (ResponseCode == "000")
            //    {
            //        SuccessfulTotal = SuccessfulTotal + CountNumber;
            //    }

            //    DataRow RowGrid = DataTableJournalsSourceStats_2.NewRow();

            //    RowGrid["ResponseCode"] = ResponseCode;
            //    /*
            // Response	Description	Number
            //105	RCPT PRNTR OUT PAPER     	933
            //000	Successful	              15582
            //096	SYSTEM ERROR             	305
            //005	FLEX TIMEOUT               1253
            //091	ISS TIMEOUT              	291
            //008	ISS TIMEOUT	                 26
            //900	HOST DOWN                	304
            //103	UNABLE TO DISP             2437
            // */

            //    switch (ResponseCode)
            //    {
            //        case "000": // 
            //            {
            //                RowGrid["Case Description"] = "Successful";
            //                break;
            //            }
            //        case "105": // 
            //            {
            //                RowGrid["Case Description"] = "RCPT PRNTR OUT PAPER";
            //                break;
            //            }
            //        case "096": // 
            //            {
            //                RowGrid["Case Description"] = "System Error";
            //                break;
            //            }
            //        case "005": // 
            //            {
            //                RowGrid["Case Description"] = "FLEX TIMEOUT";
            //                FlexTimeOutTotal = FlexTimeOutTotal + CountNumber;
            //                break;
            //            }
            //        case "091": // 
            //            {
            //                RowGrid["Case Description"] = "ISS TIMEOUT";
            //                break;
            //            }
            //        case "008": // 
            //            {
            //                RowGrid["Case Description"] = "ISS TIMEOUT";
            //                break;
            //            }
            //        case "900": // 
            //            {
            //                RowGrid["Case Description"] = "HOST DOWN";
            //                break;
            //            }
            //        case "103": // 
            //            {
            //                RowGrid["Case Description"] = "UNABLE TO DISP ";
            //                break;
            //            }

            //        default:
            //            {
            //                RowGrid["Case Description"] = "Not Specified";
            //                break;
            //            }
            //    }

            //    switch (TransactionType)
            //    {
            //        case 99: // From physical check
            //            {
            //                RowGrid["Transaction Type"] = "Other Than Money ";
            //                break;
            //            }
            //        case 11: // 
            //            {
            //                RowGrid["Transaction Type"] = "WithDrawls ";
            //                break;
            //            }
            //        case 33: // 
            //            {
            //                if (ResponseCode == "000")
            //                {
            //                    GoodTransfersTotal = GoodTransfersTotal + CountNumber;
            //                }
            //                else
            //                {
            //                    BadTransfersTotal = BadTransfersTotal + CountNumber;
            //                }

            //                RowGrid["Transaction Type"] = "Transfers ";
            //                break;
            //            }
            //        case 23: // 
            //            {
            //                RowGrid["Transaction Type"] = "Deposits BNA ";
            //                break;
            //            }
            //        default:
            //            {
            //                RowGrid["Transaction Type"] = "Mot Specified";
            //                break;
            //            }
            //    }

            //    RowGrid["Count"] = CountNumber;

            //    DataTableJournalsSourceStats_2.Rows.Add(RowGrid);

            //    I++;
            //}



            //   InsertWReport72(InSignedId);
        }
        //
        // READ Merge File based on Unique trace no  to find minimum Fuid Ruid 
        //
        public void ReadJournalTxnsByTraceAndFind_Start_End(string InOperator, string InAtmNo, int InTraceNumber
              , DateTime InTranDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]"; 

            SqlString = "SELECT SeqNo"
                + " ,BankId AS BankId "
              + " ,AtmNo AS AtmNo "
               + " ,TraceNumber AS TraceNumber"
              + " ,TranDate AS TransDate "
               + " ,StartTxn AS StartTxn "
               + " ,EndTxn AS EndTxn "
                 + " ,FuId AS FuId "
              + " ,RuId AS RuId"
             + " FROM " + WJournalId
             + " WHERE AtmNo=@AtmNo"
             + " AND TraceNumber = @TraceNumber AND CAST(TranDate AS Date) = @TranDate "
             + " Order by RuId ASC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate.Date);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];

                            TransDate = (DateTime)rdr["TransDate"];
                            TraceNumber = (int)rdr["TraceNumber"];

                            StartTxn = (int)rdr["StartTxn"];
                            EndTxn = (int)rdr["EndTxn"];

                            FuId = (int)rdr["FuId"];
                            RuId = (int)rdr["RuId"];

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
        // READ Journal Hst to find session start and end
        //
        public DateTime FuidTranDateTime;
        public void ReadJournalTxnsBySeqNoAndFind_Start_End(string InOperator, int InSeqNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Mode 1 normal 
            // Mode 2 history 

            if (InMode == 1)
            {
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            }
            if (InMode == 2)
            {
                WJournalId = "[ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns]";
            }



            SqlString = "SELECT "
                  + "Fuid "
                  //           +"ISNULL(TRanDate, '') AS TRanDate,"
                  //+ "ISNULL(TranTime, '') AS trantime,
                  + ",TranDate"
                  + ",TranTime"
                 + ",Sessionstart "
                 + ",StartTxn "
                  + ",EndTxn "
                   + ",SessionEnd "
              + " FROM " + WJournalId
              + " WHERE SeqNo=@SeqNo"
              + "";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            FuId = (int)rdr["Fuid"];

                            //           +"ISNULL(TRanDate, '') AS TRanDate,"
                            //+ "ISNULL(TranTime, '') AS trantime,

                            FuidTranDateTime = (DateTime)rdr["TranDate"];
                            TimeSpan Time = (TimeSpan)rdr["TranTime"];

                            FuidTranDateTime = FuidTranDateTime.Add(Time);

                            Sessionstart = (int)rdr["Sessionstart"];

                            StartTxn = (int)rdr["StartTxn"];
                            EndTxn = (int)rdr["EndTxn"];

                            SessionEnd = (int)rdr["SessionEnd"];

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


        public void ReadJournalTxnsBySeqNoAndFind_Start_End_ROM(string InOperator, int InSeqNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Mode 1 normal 
            // Mode 2 history 

            if (InMode == 1)
            {
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM]";
            }
            if (InMode == 2)
            {
                WJournalId = "[ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns]";
            }



            SqlString = "SELECT "
                  + "Fuid "
                  //           +"ISNULL(TRanDate, '') AS TRanDate,"
                  //+ "ISNULL(TranTime, '') AS trantime,
                  + ",TranDate"
                  + ",TranTime"
                 + ",Sessionstart "
                 + ",StartTxn "
                  + ",EndTxn "
                   + ",SessionEnd "
              + " FROM " + WJournalId
              + " WHERE SeqNo=@SeqNo"
              + "";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            FuId = (int)rdr["Fuid"];

                            //           +"ISNULL(TRanDate, '') AS TRanDate,"
                            //+ "ISNULL(TranTime, '') AS trantime,

                            FuidTranDateTime = (DateTime)rdr["TranDate"];
                            TimeSpan Time = (TimeSpan)rdr["TranTime"];

                            FuidTranDateTime = FuidTranDateTime.Add(Time);

                            Sessionstart = (int)rdr["Sessionstart"];

                            StartTxn = (int)rdr["StartTxn"];
                            EndTxn = (int)rdr["EndTxn"];

                            SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst to see 
        //

        public void ReadJournalTxnsByParameters(string InOperator, string InAtmNo, int InTraceNumber, decimal InCAmount
                                                  , string InCardNum, DateTime InTranDate
                                                  )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //{
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            //}
            //SqlString1_1 = "SELECT SeqNo, atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
            //     + "ISNULL(currency, '') AS currency,
            SqlString = "SELECT SeqNo, ISNULL(Source, '') AS Source"
                + ", ISNULL(Comments, '') AS Comments "
                + ", ISNULL(FuId, '') AS FuId "
                 + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + " AND TraceNumber = @TraceNumber"
               + " AND CAmount = @CAmount"
              // + " AND CardNum = @CardNum"
               + " AND TranDate = @TranDate"
               + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@CAmount", InCAmount);
                        //cmd.Parameters.AddWithValue("@CardNum", InCardNum);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];
                            FuId = (int)rdr["FuId"];
                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst to see 
        //

        public void ReadJournalTxnsByParametersROM(string InOperator, string InAtmNo, int InTraceNumber, decimal InCAmount
                                                  , string InCardNum, DateTime InTranDate
                                                  )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //{
            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM]";
            //}
            //SqlString1_1 = "SELECT SeqNo, atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
            //     + "ISNULL(currency, '') AS currency,
            SqlString = "SELECT SeqNo, ISNULL(Source, '') AS Source"
                + ", ISNULL(Comments, '') AS Comments "
                + ", ISNULL(FuId, '') AS FuId "
                 + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + " AND TraceNumber = @TraceNumber"
               + " AND CAmount = @CAmount"
               // + " AND CardNum = @CardNum"
               + " AND TranDate = @TranDate"
               + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@CAmount", InCAmount);
                        //cmd.Parameters.AddWithValue("@CardNum", InCardNum);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];
                            FuId = (int)rdr["FuId"];
                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst to see 
        //

        public void ReadJournalTxnsByParameters_HST(string InOperator, string InAtmNo, int InTraceNumber, decimal InCAmount
                                                  , string InCardNum, DateTime InTranDate
                                                  )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "CRBAGRAA")
            //{
            //    WJournalId = "[ATMS_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "BCAIEGCX")
            //{
            WJournalId = "[ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns]";
            //}
            //SqlString1_1 = "SELECT SeqNo, atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
            //     + "ISNULL(currency, '') AS currency,
            SqlString = "SELECT SeqNo, ISNULL(Source, '') AS Source"
                + ", ISNULL(Comments, '') AS Comments "
                + ", ISNULL(FuId, '') AS FuId "
                 + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + " AND TraceNumber = @TraceNumber"
               + " AND CAmount = @CAmount"
               // + " AND CardNum = @CardNum"
               + " AND TranDate = @TranDate"
               + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        cmd.Parameters.AddWithValue("@CAmount", InCAmount);
                        cmd.Parameters.AddWithValue("@CardNum", InCardNum);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];
                            FuId = (int)rdr["FuId"];
                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst to see 
        //

        public void ReadJournalTxnsBySeqNo(int InSeqNo)                                 
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            
            //SqlString1_1 = "SELECT SeqNo, atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
            //     + "ISNULL(currency, '') AS currency,
            SqlString = "SELECT SeqNo, ISNULL(Source, '') AS Source"
                + ", ISNULL(Comments, '') AS Comments "
                + ", ISNULL(FuId, '') AS FuId "
                 + " FROM " + WJournalId
               + " WHERE SeqNo=@SeqNo"
               + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];
                            FuId = (int)rdr["FuId"];
                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst to see if exist and it is in error 
        //
        //  SELECT TOP(1000) [atmno]
        //,[Supplier]
        //,[EjournalTypeId]
        //  FROM[ATM_MT_Journals_AUDI].[dbo].[tmpATMs_Journal_Type]
        public void ReadJournal_tmpATMs_Journal_TypeAndUpdateAtms(string InOperator)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            SqlString = "SELECT atmno, Supplier"
                + ", EjournalTypeId"
                 + " FROM [ATM_MT_Journals_AUDI].[dbo].[tmpATMs_Journal_Type]"
               + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            string atmno = (string)rdr["atmno"];
                            string Supplier = (string)rdr["Supplier"];
                            string EjournalTypeId = (string)rdr["EjournalTypeId"];

                            Ac.UpdateEjournalTypeId(atmno, Supplier, EjournalTypeId);

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
        // Truncate Table
        //

        public void TruncateTable(string InOperator, string InPhysicalTableA)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Truncate Table 

            string SQLCmd = "TRUNCATE TABLE " + InPhysicalTableA;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
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
        // READ Journal  Hst AND find the previous succesful
        //

        public void ReadJournalTxnsByParametersPreviousSuccesful(string InOperator, string InAtmNo
                                                  , DateTime InTranDate
                                                  )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "CRBAGRAA")
            //{
            //    WJournalId = "[ATMS_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "BCAIEGCX")
            //{
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            //}

            SqlString = "SELECT * "
                 + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + " AND TranDate <= @TranDate"
               + " AND TranTime < @Time"
                  + " AND Result = 'OK' "
                     + " AND TransactionType = 11 "
                     + " ORDER BY TranDate DESC, TranTime DESC "
               + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate.Date);
                        cmd.Parameters.AddWithValue("@Time", InTranDate.AddMinutes(-5).TimeOfDay);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];

                            DateTime TranDate = (DateTime)rdr["TranDate"];

                            TimeSpan Time = (TimeSpan)rdr["TranTime"];

                            Counter = Counter + 1;

                            if (Counter == 1) break;

                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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
        // READ Journal  Hst AND find the Next succesful
        //

        public void ReadJournalTxnsByParametersNextSuccesful(string InOperator, string InAtmNo
                                                  , DateTime InTranDate
                                                  )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            //if (InOperator == "ETHNCY2N")
            //{
            //    WJournalId = "[ATM_MT_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "CRBAGRAA")
            //{
            //    WJournalId = "[ATMS_Journals].[dbo].[tblHstAtmTxns]";
            //}
            //if (InOperator == "BCAIEGCX")
            //{
                WJournalId = "[ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]";
            //}

            SqlString = "SELECT * "
                 + " FROM " + WJournalId
               + " WHERE AtmNo=@AtmNo"
               + " AND TranDate >= @TranDate"
               + " AND TranTime > @Time"
                  + " AND Result = 'OK' "
                     + " AND TransactionType = 11 "
                     + " ORDER BY TranDate ASC, TranTime ASC "
               + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TranDate", InTranDate.Date);
                        cmd.Parameters.AddWithValue("@Time", InTranDate.AddMinutes(5).TimeOfDay);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            Source = (string)rdr["Source"];
                            Comments = (string)rdr["Comments"];

                            DateTime TranDate = (DateTime)rdr["TranDate"];
                            TimeSpan Time = (TimeSpan)rdr["TranTime"];

                            Counter = Counter + 1;

                            if (Counter == 1) break;

                            //Sessionstart = (int)rdr["Sessionstart"];

                            //StartTxn = (int)rdr["StartTxn"];
                            //EndTxn = (int)rdr["EndTxn"];

                            //SessionEnd = (int)rdr["SessionEnd"];

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


        ////
        //// READ Merge File based on Unique trace no  to find minimum Fuid Ruid 
        ////
        //public void ReadJournalTextByTraceAndFind_Fuid_Ruid_MAX(string InOperator, string InAtmNo, int InTraceNumber
        //      , DateTime InTransDate)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    if (InOperator == "ETHNCY2N")
        //    {
        //        WJournalId = "[ATM_Journals_Diebold].[dbo].[tblHstEjText]";
        //    }
        //    if (InOperator == "CRBAGRAA")
        //    {
        //        WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
        //    }   

        //    string SqlString = "SELECT  "
        //          + " BankId AS BankId "
        //        + " ,AtmNo AS AtmNo "
        //         + " ,TraceNo AS TraceNo "
        //        + " ,TransDate AS TransDate "
        //         + " ,TransTime AS TransTime "
        //        + " ,TraceNumber AS TraceNumber"
        //         + " ,ISNULL(TxtLine, '') AS TxtLine "
        //           + " ,FuId AS FuId "
        //        + " ,RuId AS RuId"
        //         + " ,Operator AS Operator "
        //       + " FROM " + WJournalId
        //       + " WHERE Operator=@Operator AND AtmNo=@AtmNo"
        //       + " AND TraceNumber = @TraceNumber AND CAST(TransDate AS Date) = @TransDate "
        //       + " Order by RuId DESC";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Operator", InOperator);
        //                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
        //                cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
        //                cmd.Parameters.AddWithValue("@TransDate", InTransDate.Date);
        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    BankId = (string)rdr["BankId"];
        //                    AtmNo = (string)rdr["AtmNo"];

        //                    TraceNo = (string)rdr["TraceNo"];

        //                    TransDate = (DateTime)rdr["TransDate"];

        //                    TransTime = (TimeSpan)rdr["TransTime"];

        //                    TraceNumber = (int)rdr["TraceNumber"];

        //                    TxtLine = (string)rdr["TxtLine"];

        //                    FuId = (int)rdr["FuId"];
        //                    RuId = (int)rdr["RuId"];
        //                    Operator = (string)rdr["Operator"];

        //                    break;
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
        //
        // READ Journal by Trace Number which is not unique to find Unique Trace Number
        // Needed input is trace number , AtmNo, Date range
        //
        public int ReadJournalByTraceToFindUniqueTrace(string InOperator, string InAtmNo, int InIntTraceNo,
                                 DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InOperator == "ETHNCY2N")
            {
                WJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }
            if (InOperator == "CRBAGRAA")
            {
                WJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }

            string SqlString = "SELECT *"
                + " FROM " + WJournalId
               + " WHERE Operator=@Operator AND AtmNo=@AtmNo AND CAST(TraceNo AS INT) = @TraceNo "
               + " AND (TransDate >= @DateFrom AND TransDate <= @Dateto) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InIntTraceNo);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);


                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TraceNumber = (int)rdr["TraceNumber"];
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
            return TraceNumber;
        }

        public void UpdateEjTextFromHst(
            int InFuid,
            int InStartTrxn,
            int InEndTrxn,
            string InCardNo,
            string InAccNo,
            string InDescr,
            string InCurNm,
            decimal InTranAmnt

            )
        {

            int RowsUpdated = 0;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals].[dbo].[tblHstEjText] SET "
                            + " CardNo = @CardNo, AccNo = @AccNo,  "
                            + " Descr = @Descr, CurNm = @CurNm, TranAmnt = @TranAmnt  "
                            + " WHERE Fuid = @Fuid AND Ruid >= @StartTrxn AND Ruid<= @EndTrxn ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Fuid", InFuid);
                        cmd.Parameters.AddWithValue("@StartTrxn", InStartTrxn);
                        cmd.Parameters.AddWithValue("@EndTrxn", InEndTrxn);

                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);

                        cmd.Parameters.AddWithValue("@Descr", InDescr);

                        cmd.Parameters.AddWithValue("@CurNm", InCurNm);
                        cmd.Parameters.AddWithValue("@TranAmnt", InTranAmnt);


                        RowsUpdated = cmd.ExecuteNonQuery();


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
}

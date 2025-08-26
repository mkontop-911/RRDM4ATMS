using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RRDM4ATMs
{
    public class RRDM_EXCEL_AND_Directories : Logger
    {
        public RRDM_EXCEL_AND_Directories() : base() { }

        // Export DataTable into an excel file with field names in the header line
        // - Save excel file without ever making it visible if filepath is given
        // - Don't save excel file, just make it visible if no filepath is given
        public void ExportToExcel(DataTable tbl, string InWorkingDir ,string excelFilePath = null)
        {
            try
            {
                Type officeType = Type.GetTypeFromProgID("Excel.Application");
                if (officeType == null)
                {
                    //no Excel installed
                    MessageBox.Show("Excel not installed. Export operation will terminate.");
                    return;
                }
                else
                {
                    //Excel installed
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
                //throw new Exception("ExportToExcel: \n" + ex.Message);
            }
            try
            {
                if (tbl == null || tbl.Columns.Count == 0)
                    throw new Exception("ExportToExcel: Null or empty input table!\n");

                

                //Jt.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);
                //string WorkingDirectory = Gp.OccuranceNm + InReversedCut_Off_Date + "_"
                //                                        + InReconcCycleNo.ToString() + "\\"
                //                                        + DestinationName.ToString() + "\\";

            
                    bool WSuccess = CreateDirectory(InWorkingDir);


                    // load excel, and create a new workbook
                    var excelApp = new Excel.Application();
                excelApp.Workbooks.Add();

                // single worksheet
                Excel._Worksheet workSheet = excelApp.ActiveSheet;

                // column headings
                for (var i = 0; i < tbl.Columns.Count; i++)
                {
                    workSheet.Cells[1, i + 1] = tbl.Columns[i].ColumnName;
                }

                // rows
                for (var i = 0; i < tbl.Rows.Count; i++)
                {
                    // to do: format datetime values before printing
                    for (var j = 0; j < tbl.Columns.Count; j++)
                    {
                        workSheet.Cells[i + 2, j + 1] = tbl.Rows[i][j];
                    }
                }

                // check file path
                if (!string.IsNullOrEmpty(excelFilePath))
                {
                    try
                    {
                        workSheet.SaveAs(excelFilePath);
                        excelApp.Quit();
                        MessageBox.Show("Excel file saved as "+Environment.NewLine
                                       + excelFilePath
                                       );
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("ExportToExcel: Excel file could not be saved! Check filepath.\n"
                        //                    + ex.Message);
                        MessageBox.Show("ExportToExcel: Excel file could not be saved! Check filepath.\n"
                                            + ex.Message);
                    }
                }
                else
                { // no file path is given
                    excelApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
                //throw new Exception("ExportToExcel: \n" + ex.Message);
            }
        }

        public void ConvertToTapDelimiterFile(string InExcelId)
        {
            try
            {

                //  string oXLFileName = @"C:\RRDM\FilePool\Egypt_123_NET\BDC_Trans_Of_Day 24-10-2021.XLS";
                string oXLFileName = @InExcelId;
                var oXLApp = new Microsoft.Office.Interop.Excel.Application();

                Microsoft.Office.Interop.Excel.Workbooks oWorkBooks = oXLApp.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook oMyXLFile = oWorkBooks.Open(oXLFileName);
                // oMyXLFile.SaveAs(@"C:\RRDM\FilePool\Egypt_123_NET\BDC_Trans_Of_Day 24-10-2021.txt", Microsoft.Office.Interop.Excel.XlFileFormat.xlCurrentPlatformText);
                oMyXLFile.SaveAs(@InExcelId+".099", Microsoft.Office.Interop.Excel.XlFileFormat.xlCurrentPlatformText);
                oMyXLFile.Close();
                oXLApp.Quit();

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
                //throw new Exception("ExportToExcel: \n" + ex.Message);
            }
           
        }
        public bool RecordFound; 
        public bool CreateTapFromFile(string InFileName, string InFilePath)
        {
            // SEE Form80b3 
            bool IsSuccess = false; 
            string ATMSconnectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
           
                string SqlString = " SELECT * FROM " + InFileName;
            try
            {
                using (SqlConnection connection = new SqlConnection(ATMSconnectionString))
                {
                    // Open the connection
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(SqlString, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Path for the output file
                        string filePath = InFilePath;
                        // string CreatedFile = "C:\\RRDM\\Working\\TAB_File_Actions_This_Cycle" + InRMCycle.ToString() + ".txt";

                        // Create a StreamWriter to write to the file
                        using (StreamWriter writer = new StreamWriter(filePath))
                          //  StreamWriter outputFile = new StreamWriter(@CreatedFile);
                        {
                            // Write column headers
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write(reader.GetName(i));
                                if (i < reader.FieldCount - 1)
                                    writer.Write('\t'); // Add tab delimiter
                            }
                            writer.WriteLine();

                            // Write rows
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    writer.Write(reader[i].ToString());
                                    if (i < reader.FieldCount - 1)
                                        writer.Write('\t'); // Add tab delimiter
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            IsSuccess = true;

            return IsSuccess;

        }



        //public void Export_Ctr_Excel(DataTable tablelist, string excelFilename)
        //{
        //    // Here is main process
        //    Microsoft.Office.Interop.Excel.Application objexcelapp = new Microsoft.Office.Interop.Excel.Application();
        //    objexcelapp.Application.Workbooks.Add(Type.Missing);
        //    objexcelapp.Columns.AutoFit();
        //    for (int i = 1; i < tablelist.Columns.Count + 1; i++)
        //    {
        //        Microsoft.Office.Interop.Excel.Range xlRange = (Microsoft.Office.Interop.Excel.Range)objexcelapp.Cells[1, i];
        //        xlRange.Font.Bold = -1;
        //        xlRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //        xlRange.Borders.Weight = 1d;
        //        xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        //        objexcelapp.Cells[1, i] = tablelist.Columns[i - 1].ColumnName;
        //    }
        //    /*For storing Each row and column value to excel sheet*/
        //    for (int i = 0; i < tablelist.Rows.Count; i++)
        //    {
        //        for (int j = 0; j < tablelist.Columns.Count; j++)
        //        {
        //            if (tablelist.Rows[i][j] != null)
        //            {
        //                Microsoft.Office.Interop.Excel.Range xlRange = (Microsoft.Office.Interop.Excel.Range)objexcelapp.Cells[i + 2, j + 1];
        //                xlRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
        //                xlRange.Borders.Weight = 1d;
        //                objexcelapp.Cells[i + 2, j + 1] = tablelist.Rows[i][j].ToString();
        //            }
        //        }
        //    }
        //    objexcelapp.Columns.AutoFit(); // Auto fix the columns size
        //    System.Windows.Forms.Application.DoEvents();
        //    if (Directory.Exists("C:\\TempKonto\\")) // Folder dic
        //    {
        //        objexcelapp.ActiveWorkbook.SaveCopyAs("C:\\TempKonto\\" + excelFilename + ".xlsx");
        //    }
        //    else
        //    {
        //        Directory.CreateDirectory("C:\\TempKonto\\");
        //        objexcelapp.ActiveWorkbook.SaveCopyAs("C:\\TempKonto\\" + excelFilename + ".xlsx");
        //    }
        //    objexcelapp.ActiveWorkbook.Saved = true;
        //    System.Windows.Forms.Application.DoEvents();
        //    foreach (Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
        //    {
        //        proc.Kill();
        //    }
        //}

        public void CreateA_DelimtereFile(DataTable inDataTable )
        {
           
        }

        
        //
        // Move file to the Archive directory
        //
        public bool MoveFileToArchiveDirectory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InFullPath)
        {

            bool Success = false;
            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "14";
            string CopiedFile;
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            Msf.ReadReconcSourceFilesByFileId(InTableA);

            string DestinationName = Msf.SourceFileId;


            string WorkingDirectory = Gp.OccuranceNm + InReversedCut_Off_Date + "_"
                                                        + InReconcCycleNo.ToString() + "\\"
                                                        + DestinationName.ToString() + "\\";

            try
            {
                bool WSuccess = CreateDirectory(WorkingDirectory);

                if (WSuccess == true)
                {
                    CopiedFile = CopyFileFromOneDirectoryToAnother(InFullPath, WorkingDirectory);

                }

                if (CopySuccess == true)
                {
                    DeleteFileFromDirectory(InFullPath);
                }

                Success = true;
            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

            return Success;

        }

        //
        // Create Excel Directory if not Available 
        //
        public bool CreateExcelDirectory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InFullPath)
        {

            bool Success = false;
            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "14";
            string CopiedFile;
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            Msf.ReadReconcSourceFilesByFileId(InTableA);

            string DestinationName = Msf.SourceFileId;


            string WorkingDirectory = Gp.OccuranceNm + InReversedCut_Off_Date + "_"
                                                        + InReconcCycleNo.ToString() + "\\"
                                                        + DestinationName.ToString() + "\\";

            try
            {
                bool WSuccess = CreateDirectory(WorkingDirectory);

                if (WSuccess == true)
                {
                    CopiedFile = CopyFileFromOneDirectoryToAnother(InFullPath, WorkingDirectory);

                }

                if (CopySuccess == true)
                {
                    DeleteFileFromDirectory(InFullPath);
                }

                Success = true;
            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

            return Success;

        }

        //
        // For this cycle find all files and move them to the origin 
        //
        public bool LoopFor_MoveFile_From_Archive_ToOrigin_Directory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date)
        {
            bool Success = false;
            RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "14"; // FilesArchives

            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            string WorkingDirectory_A = Gp.OccuranceNm + InReversedCut_Off_Date
                + "_" + InReconcCycleNo.ToString() + "\\";

            string WSelectionCriteria = " Operator='" + InOperator + "'";
            Rs.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;

            while (I <= (Rs.SourceFilesDataTable.Rows.Count - 1))
            {

                string SourceFileId = (string)Rs.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string SourceDirectory = (string)Rs.SourceFilesDataTable.Rows[I]["SourceDirectory"];

                string InTableA = SourceFileId;

                string WorkingDirectory_B = Gp.OccuranceNm + InReversedCut_Off_Date
               + "_" + InReconcCycleNo.ToString() + "\\"
                + InTableA + "\\";

                Undo_MoveFile_From_Archive_ToOrigin_Directory(InOperator, InReconcCycleNo, InReversedCut_Off_Date, InTableA, WorkingDirectory_B);

                I = I + 1;

            }

            // DELETE The root directory at Loop finished

            bool WithCopy = false;
            DeleteDirectoryWithCopy(WorkingDirectory_A, WithCopy, "");

            Success = true;

            return Success;
        }

        //
        // For this cycle find all files and move them to the origin 
        //
        public bool LoopFor_MoveFile_From_Archive_ToOrigin_Directory_MOBILE(string InApplication, string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date)
        {
            bool Success = false;
            RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "14"; // FilesArchives

            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            string WorkingDirectory_A = Gp.OccuranceNm + InReversedCut_Off_Date
                + "_" + InReconcCycleNo.ToString() + "\\";

            string WSelectionCriteria = "Empty";

            if (InApplication == "ETISALAT" || InApplication == "QAHERA" || InApplication == "IPN" || InApplication == "EGATE")
            {
                string Filter1 = "";
                // Get only the e_MOBILE

                WSelectionCriteria = "Operator = '" + InOperator + "' AND Enabled = 1 AND SystemOfOrigin='" + InApplication + "' ";

            }
            Rs.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;

            while (I <= (Rs.SourceFilesDataTable.Rows.Count - 1))
            {

                string SourceFileId = (string)Rs.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string SourceDirectory = (string)Rs.SourceFilesDataTable.Rows[I]["SourceDirectory"];

                string InTableA = SourceFileId;

                string WorkingDirectory_B = Gp.OccuranceNm + InReversedCut_Off_Date
               + "_" + InReconcCycleNo.ToString() + "\\"
                + InTableA + "\\";

                Undo_MoveFile_From_Archive_ToOrigin_Directory(InOperator, InReconcCycleNo, InReversedCut_Off_Date, InTableA, WorkingDirectory_B);

                I = I + 1;

            }

            // DELETE The root directory at Loop finished

            bool WithCopy = false;
            DeleteDirectoryWithCopy(WorkingDirectory_A, WithCopy, "");

            Success = true;

            return Success;
        }


        //
        // For this cycle move file to the origin 
        //
        public bool MoveFile_From_Archive_ToOrigin_Directory(string InOperator, int InReconcCycleNo, 
                                                        string InReversedCut_Off_Date, string InFileId)
        {
            bool Success = false;
            RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "920";
            string OccurId = "14"; // FilesArchives

            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            string WorkingDirectory_A = Gp.OccuranceNm + InReversedCut_Off_Date
                + "_" + InReconcCycleNo.ToString() + "\\";

            string WSelectionCriteria = " Operator='" + InOperator + "' AND  SourceFileId ='" + InFileId + "'";
            Rs.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;
            // One File ....
            while (I <= (Rs.SourceFilesDataTable.Rows.Count - 1))
            {

                string SourceFileId = (string)Rs.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string SourceDirectory = (string)Rs.SourceFilesDataTable.Rows[I]["SourceDirectory"];

                string InTableA = SourceFileId;

                string WorkingDirectory_B = Gp.OccuranceNm + InReversedCut_Off_Date
               + "_" + InReconcCycleNo.ToString() + "\\"
                + InTableA + "\\";

                Undo_MoveFile_From_Archive_ToOrigin_Directory(InOperator, InReconcCycleNo, InReversedCut_Off_Date, 
                                                                                  InTableA, WorkingDirectory_B);
            
                I = I + 1;

            }

            // DELETE The root directory at Loop finished
            //MessageBox.Show("To be developed 73923");
            //if (I == 3)
            //{

            //    bool WithCopy = false;
            //    DeleteDirectoryWithCopy(WorkingDirectory_A, WithCopy, "");

            //}

            Success = true;

            return Success;
        }



        //
        // Move file From Archive directory to the Origin 
        //
        public bool Undo_MoveFile_From_Archive_ToOrigin_Directory(string InOperator, int InReconcCycleNo, string InReversedCut_Off_Date, string InTableA, string InWorkingDirectory_B)
        {

            bool Success = false;
            string CopiedFile;


            // Take all from 
            // Read one by one and 

            try
            {
                // Find files in Archive
                if (Directory.Exists(InWorkingDirectory_B))
                {
                    string[] allFiles = Directory.GetFiles(InWorkingDirectory_B, "*.*");
                    if (allFiles.Length > 0)
                    {
                        foreach (string fileToCopy in allFiles)
                        {

                            //string DestinationfileId = Path.GetFileName(fileToCopy);

                            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

                            Msf.ReadReconcSourceFilesByFileId(InTableA);

                            string DestinationDirectory = Msf.SourceDirectory;

                            // bool WSuccess = Jt.CreateDirectory(WorkingDirectory);


                            CopiedFile = CopyFileFromOneDirectoryToAnother(fileToCopy, DestinationDirectory);

                            if (CopySuccess == true)
                            {
                                DeleteFileFromDirectory(fileToCopy);
                            }
                        }
                    }
                    string[] allFiles_2 = Directory.GetFiles(InWorkingDirectory_B, "*.*");
                    if (allFiles_2.Length == 0)
                    {
                        bool WithCopy = false;
                        DeleteDirectoryWithCopy(InWorkingDirectory_B, WithCopy, "");
                    }

                    Success = true;
                }


            }
            catch (Exception exf)
            {
                CatchDetails(exf);
            }

            return Success;

        }

        //
        // Create Directory 
        //
        public bool CreateDirectory(string InPathString)
        {
            bool Success = false;

            try
            {

                if (System.IO.Directory.Exists(InPathString))
                {
                    //System.IO.Directory.CreateDirectory(InPathString);
                    Success = true;
                }
                else
                {
                    System.IO.Directory.CreateDirectory(InPathString);
                    Success = true;
                }

                // System.IO.Directory.Delete(InPathString);

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                Success = false;

            }
            // return FullDestination;

            return Success;
        }

        //
        // Delete Directory 
        //
        public bool DeleteDirectoryWithCopy(string InPathString, bool InWithCopy, string InCopyDestination)
        {
            bool Success = false;
            int Count = 0;

            try
            {

                if (Directory.Exists(InPathString))
                {
                    //  DirectoryInfo parentDirectory = new DirectoryInfo(InPathString);

                    //  parentDirectory.Attributes = FileAttributes.Normal;
                    //MessageBox.Show("Directory Exists" + InPathString);

                    string[] files = Directory.GetFiles(InPathString);

                    if (files.Length > 0)
                    {
                        foreach (string file in files)
                        {
                            Count = Count + 1;
                            //MessageBox.Show("Before Setting Atributes to Normal:" + file);
                            // File.SetAttributes(file, FileAttributes.Normal);
                            if (InWithCopy == true)
                            {
                                //if (Count<5)
                                //MessageBox.Show("Copy call routine starts for:" + file); 
                                CopyFileFromOneDirectoryToAnother(file, InCopyDestination);
                            }
                            if (CopySuccess == true || InWithCopy == false)
                            {
                                //if (Count < 5)
                                //MessageBox.Show("Copy file Was Succesful:" + file);
                                File.Delete(file);
                            }
                            else
                            {
                                //if (Count < 5)
                                MessageBox.Show("Copy file Was UN-Succesful:" + file);
                            }

                        }

                        string[] files2 = Directory.GetFiles(InPathString);

                        if (files2.Length == 0)
                        {
                            Directory.Delete(InPathString);
                            Success = true;
                        }


                    }
                    else
                    {
                        Directory.Delete(InPathString);
                        Success = true;

                    }

                }
                else
                {
                    MessageBox.Show("Directory doesnot exist" + InPathString);
                }

                //System.IO.Directory.Delete(InPathString);

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                Success = false;

            }
            // return FullDestination;

            return Success;
        }

        //
        // Delete Records within directory
        //
        public bool DeleteFilesWithinDirectory(string InPathString)
        {
            bool Success = false;
            int Count = 0;

            try
            {

                if (Directory.Exists(InPathString))
                {
                    //  DirectoryInfo parentDirectory = new DirectoryInfo(InPathString);

                    //  parentDirectory.Attributes = FileAttributes.Normal;
                    //MessageBox.Show("Directory Exists" + InPathString);

                    string[] files = Directory.GetFiles(InPathString);

                    if (files.Length > 0)
                    {
                        foreach (string file in files)
                        {
                            Count = Count + 1;

                            File.Delete(file);

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Directory doesnot exist" + InPathString);
                }

                //System.IO.Directory.Delete(InPathString);

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                Success = false;

            }
            // return FullDestination;

            return Success;
        }

       
        //
        // Copy from Archiving to working directory
        //
        public bool CopySuccess;

        public string CopyFileFromOneDirectoryToAnother(string InSourceFile, string InTargetDirectory)
        {

            // string fileName_In;
            string FullDestination = "";
            CopySuccess = false;

            try
            {
                //   string fileToCopy = "c:\\myFolder\\myFile.txt";
                string fileToCopy = InSourceFile;
                //string destinationDirectory = "c:\\myDestinationFolder\\";
                string destinationDirectory = InTargetDirectory;

                string DestinationfileId = Path.GetFileName(fileToCopy);

                FullDestination = Path.Combine(destinationDirectory, DestinationfileId);

                //MessageBox.Show("Before Actual Copy:" + fileToCopy);
                //MessageBox.Show("Full Destination.." + FullDestination);
                //  File.SetAttributes(fileToCopy, FileAttributes.Normal);

                File.Copy(fileToCopy, FullDestination);

                CopySuccess = true;

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }
            return FullDestination;
        }

        //
        // Delete File 
        //
        public bool DeleteFileFromDirectory(string InSourceFile)
        {
            bool Success = false;
            try
            {
                File.Delete(InSourceFile);
                Success = true;
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            return Success;

        }

        //
        // Copy from Archiving to working directory
        //
        string stringToPrint;

        public void PrintFromSourceJournal(string InSourceFile, string TargetDirectory)
        {

            // string fileName_In;


            try
            {

                using (FileStream stream = new FileStream(InSourceFile, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    stringToPrint = reader.ReadToEnd();
                }

                //int charactersOnPage = 0;
                //int linesPerPage = 0;

                //// Sets the value of charactersOnPage to the number of characters 
                //// of stringToPrint that will fit within the bounds of the page.
                //e.Graphics.MeasureString(stringToPrint, this.Font,
                //    e.MarginBounds.Size, StringFormat.GenericTypographic,
                //    out charactersOnPage, out linesPerPage);

                //// Draws the string within the bounds of the page
                //e.Graphics.DrawString(stringToPrint, this.Font, Brushes.Black,
                //    e.MarginBounds, StringFormat.GenericTypographic);

                //// Remove the portion of the string that has been printed.
                //stringToPrint = stringToPrint.Substring(charactersOnPage);

                //// Check to see if more pages are to be printed.
                //e.HasMorePages = (stringToPrint.Length > 0);




            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

        }

    }
}

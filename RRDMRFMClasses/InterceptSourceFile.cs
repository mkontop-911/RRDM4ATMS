using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRDM4ATMs;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace RRDMRFMClasses
{
    public class InterceptSourceFile
    {
        ////#region Static members
        ////static string argOrigin = "";        // argument 1
        ////static string argSourceFileID = "";  // argument 2
        //static string argOperator = "";      // argument 3

        //static bool glbRawImport = false;    // derived from glbOrigin (if argOrigin==ATMs)
        //static bool glbArchiveEnabled = false;    // read from GAS Parameters

        ////static int glbRFMSleep;              // GAS parameters
        //static string glbStoredProc = "";    // GAS parameters; used if glbRawImport=true
        //// static RRDMMatchingSourceFiles rsf;
        ////#endregion

        #region Intercept File
        public static ProcessFileResult InterceptFile(string fileFullName, string ATM_No, string eJournalType, int LinesInHeader, int LinesInTrailer)
        {
            bool retValue;
            string msg;
            long fileSize = 0;
            int LineCount = 0;
            int ejFUID = 0;

            bool isSuccess = false;
            string FileUID;
            string HASHValue;
            string Origin = RFMStartPoint.rsf.SystemOfOrigin;
            string SourceFileID = RFMStartPoint.rsf.SourceFileId;
            int RFMLSeqNo = 0; // ReconcFileMonitorLog record id
            string originalFullFileName = fileFullName;
            string fileName = Path.GetFileName(fileFullName);

            string dateOfFile = "";
            DateTime expectedDate = DateTime.Now;

            ProcessFileResult retVal = ProcessFileResult.Success;

            RRDMReconcFileMonitorLog rfml = new RRDMReconcFileMonitorLog();
            rfml.DateTimeReceived = DateTime.Now;

            // Get RMCycle number
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            rfml.RMCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(RFMStartPoint.argOperator, "ATMs");

            RRDMMatchingCategoriesVsSourcesFiles rcs;

            FileInfo fInf = new FileInfo(fileFullName);
            fileSize = fInf.Length;

            #region Check if File is Empty

            if (fileSize == 0)
            {
                // Empty file ... abort
                // Move file to Exceptions dir so it is not processed again...
                string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);

                // Log the error
                msg = string.Format("Processing of intercepted file {0} stopped! The file is empty!" +
                                     "\r\nThe file has been moved to '{1}'.",
                                     Path.GetFileName(fileName), failedFileName);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                #region Insert a row in the File Monitor Log table
                rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                rfml.FileName = fileName;
                rfml.FileSize = (Int32)fileSize;
                // rfml.DateTimeReceived = DateTime.Now; set at entry to the methodrfml.DateTimeReceived = DateTime.Now;
                rfml.DateExpected = expectedDate;
                rfml.DateOfFile = dateOfFile;
                rfml.FileHASH = "";
                rfml.Status = 0;
                rfml.ArchivedPath = "";
                rfml.ExceptionPath = failedFileName;
                rfml.stpFuid = 0;
                rfml.LineCount = 0;
                //msg = string.Format("The contents of the file have the same signature as that of file '{0}' which has already been processed!. ",
                //                     Path.GetFileName(rfml.FileName));
                rfml.StatusVerbose = "Empty File! " + Path.GetFileName(fileFullName);

                // RMCycle number already set...

                RFMLSeqNo = rfml.Insert();
                if (rfml.ErrorFound)
                {
                    msg = string.Format("Processing stopped! " +
                                        "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                         rfml.ErrorOutput);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return ProcessFileResult.SQL_Error;
                }
                #endregion

                return ProcessFileResult.IO_Error;
            }
            #endregion

            dateOfFile = "";
            try
            {
                DateTime dfDT;
                dfDT = RFMFunctions.eXractDateFromString(Path.GetFileName(fileName));
                dateOfFile = dfDT.ToString("yyyy-MM-dd");
            }
            catch (DivideByZeroException ex)
            {
                dateOfFile = "Invalid!";
            }
            catch (Exception ex)
            {
                dateOfFile = "Invalid!";
            }

            #region Record the event in the EventLog ..
            msg = string.Format("\r\nNew File: {0} from OriginSystem: {1} and SourceFileID: {2} was intercepted!", fileName, Origin, SourceFileID);
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
            #endregion

            // Calculate SHA256 hash value

            HASHValue = FileHASH.BytesToString(FileHASH.GetHashSha256(fileFullName));

            #region Expected Date
            rcs = new RRDMMatchingCategoriesVsSourcesFiles();
            rcs.ReadReconcCategoriesVsSourcesBySourceFile(SourceFileID);
            if (rcs.RecordFound)
            {
                string dateF;

                expectedDate = rcs.ExpectedDate;
                string dateForm1 = string.Format("{0:0000}{1:00}{2:00}", expectedDate.Year, expectedDate.Month, expectedDate.Day);
                string dateForm2 = string.Format("{0:00}{1:00}{2:0000}", expectedDate.Day, expectedDate.Month, expectedDate.Year);

                if (fileName.Contains(dateForm1))
                {
                    dateF = dateForm1;

                }
                if (fileName.Contains(dateForm2))
                {
                    dateF = dateForm2;
                }

                if (RFMStartPoint.glbRawImport != true) // Check only for non-eJournals
                {
                    if (!fileName.Contains(dateForm1) && !fileName.Contains(dateForm2))
                    {
                        // NOT the expected date  Move file to exceptions dir and return
                        string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);

                        ////change extension of the source file to'.NotExpected' so it is not processed again...
                        //try
                        //{
                        //    failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".NotExpected"); // Network drives?
                        //    File.Move(fileFullName, failedFileName);
                        //}
                        //catch (IOException ex)
                        //{
                        //    //The destination file already exists. 
                        //    File.Delete(fileFullName);
                        //}
                        //catch (Exception ex)
                        //{
                        //    // Rename failed! Same filename and extension must already exist. Delete this one...
                        //    msg = string.Format("Exception while renaming file [{0}] to [{1}]! Exception Message: {2}", fileFullName, failedFileName, ex.Message);
                        //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                        //    File.Delete(fileFullName);
                        //}

                        // Log the error
                        msg = string.Format("Processing of intercepted file stopped! The filename's date signature is not the one expected! " +
                                             "\r\nThe expected filename should contain either '{0}' or '{1}'." +
                                             "\r\nThe intercepted file has been moved to '{2}'.",
                                             dateForm1, dateForm2, failedFileName);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                        #region Insert a row in the File Monitor Log table
                        rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                        rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                        rfml.FileName = fileName;
                        rfml.FileSize = (Int32)fileSize;
                        rfml.DateExpected = expectedDate;
                        rfml.DateOfFile = dateOfFile;
                        rfml.FileHASH = HASHValue;
                        rfml.Status = 0;
                        //msg = string.Format("The filename should contain either '{0}' or '{1}'. ", dateForm1, dateForm2);
                        //if (dateOfFile == "Invalid!")
                        //{
                        //    msg += string.Format("Instead in contained an invalid date signature!");
                        //}
                        //else
                        //{
                        //    msg += string.Format("Instead in contained: {0}", dateOfFile);
                        //}
                        msg = " Expected Date: " + expectedDate.ToShortDateString();
                        if (dateOfFile == "Invalid!")
                        {
                            msg += (" Found: Invalid 'date' value in the filename!");
                        }
                        else
                        {
                            msg += string.Format(" Found: {0}", dateOfFile);
                        }

                        rfml.StatusVerbose = "Not Expected! " + Path.GetFileName(fileFullName) + msg;
                        rfml.ArchivedPath = "";
                        rfml.ExceptionPath = failedFileName;
                        rfml.LineCount = 0;
                        rfml.stpFuid = 0;
                        // RMCycle number already set...

                        RFMLSeqNo = rfml.Insert();
                        if (rfml.ErrorFound)
                        {
                            msg = string.Format("Processing stopped! " +
                                                "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                                 rfml.ErrorOutput);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                            return ProcessFileResult.SQL_Error;
                        }
                        #endregion

                        return ProcessFileResult.NotExpected;
                    }
                }
            }
            else
            {
                // Expected Date not available (Record not found! )

                if (ATM_No == "") // Not ATMs, abort
                {
                    // Move file to Exceptions dir so it is not processed again...
                    string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);
                    //
                    //try
                    //{
                    //    failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".Failed"); // Network drives?
                    //    File.Move(fileFullName, failedFileName);
                    //}
                    //catch (IOException ex)
                    //{
                    //    //The destination file already exists. 
                    //    File.Delete(fileFullName);
                    //}
                    //catch (Exception ex)
                    //{
                    //    // Rename failed! Same filename and extension must already exist. Delete this one...
                    //    msg = string.Format("Exception while renaming file [{0}] to [{1}]! Exception Message: {2}", fileFullName, failedFileName, ex.Message);
                    //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                    //    File.Delete(fileFullName);
                    //}

                    // Log the error
                    msg = string.Format("Processing of intercepted file stopped! \r\n" +
                                        "Could not establish the the 'Expected Date' of the file because no record was found in 'MatchingCategoriesVsSourceFile' table!." +
                                        "\r\nThe file has been moved to '{0}'.", failedFileName);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                    #region Insert a row in the File Monitor Log table
                    rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                    rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                    rfml.FileName = fileName;
                    rfml.FileSize = (Int32)fileSize;
                    // rfml.DateTimeReceived = DateTime.Now; set at entry to the method
                    rfml.DateExpected = expectedDate;
                    rfml.DateOfFile = dateOfFile;
                    rfml.FileHASH = HASHValue;
                    rfml.Status = 0;
                    //msg = string.Format("Could not establish the the 'Expected Date' of the file. No record of Origin='{0}' and FileID'{1}' was found in the database.",
                    //    RFMStartPoint.rsf.SystemOfOrigin, RFMStartPoint.rsf.SourceFileId);
                    msg = " Expected Date: Could not establish!";

                    rfml.StatusVerbose = "Not Expected! " + Path.GetFileName(fileFullName) + msg;
                    rfml.ArchivedPath = "";
                    rfml.ExceptionPath = failedFileName;
                    rfml.LineCount = 0;
                    rfml.stpFuid = 0;


                    // RMCycle number already set...

                    RFMLSeqNo = rfml.Insert();
                    if (rfml.ErrorFound)
                    {
                        msg = string.Format("Processing stopped! " +
                                            "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                             rfml.ErrorOutput);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        return ProcessFileResult.SQL_Error;
                    }
                    #endregion

                    return ProcessFileResult.NotExpected;
                }
            }
            #endregion

            #region Check if the file has already been processed
            // Use the HASH to retieve a row from table
            rfml.GetRecordByFileHASH(HASHValue);
            if (rfml.ErrorFound)
            {
                // error reading ... abort
                msg = string.Format("Processing stopped! " +
                                     "\nCould not validate the file HASH value because the database reports:\n{0}", rfml.ErrorOutput);

                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Error;
            }
            else
            {
                if (rfml.RecordFound && rfml.Status != 0)
                {
                    // int originalRfmlStatus = rfml.Status;

                    // FileHASH already exists ... 
                    // Move file to Exceptions dir so it is not processed again...
                    string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);

                    ////change extension of the source file to'.Duplicate' so it is not processed again...
                    //try
                    //{
                    //    failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".Duplicate"); // Network drives?
                    //    File.Move(fileFullName, failedFileName);
                    //}
                    //catch (IOException ex)
                    //{
                    //    //The destination file already exists. 
                    //    File.Delete(fileFullName);
                    //}
                    //catch (Exception ex)
                    //{
                    //    // Rename failed! Same filename and extension must already exist. Delete this one...
                    //    msg = string.Format("Exception while renaming file [{0}] to [{1}]! Exception Message: {2}", fileFullName, failedFileName, ex.Message);
                    //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                    //    File.Delete(fileFullName);
                    //}

                    // Log the error
                    msg = string.Format("Processing of intercepted file stopped! Cannot process the same file twice!" +
                                         "\r\nThe file HASH value of the intercepted file [{0}] already exists in the database for a file named [{1}]." +
                                         "\r\nThe file has been moved to '{2}'.",
                                         fileName, rfml.FileName, failedFileName);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                    #region Insert a row in the File Monitor Log table
                    rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                    rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                    rfml.FileName = fileName;
                    rfml.FileSize = (Int32)fileSize;
                    // rfml.DateTimeReceived = DateTime.Now; set at entry to the methodrfml.DateTimeReceived = DateTime.Now;
                    rfml.DateExpected = expectedDate;
                    rfml.DateOfFile = dateOfFile;
                    rfml.FileHASH = HASHValue;

                    // rfml.Status = originalRfmlStatus;
                    rfml.Status = 0;

                    rfml.ArchivedPath = "";
                    rfml.ExceptionPath = failedFileName;
                    rfml.stpFuid = 0;
                    rfml.LineCount = 0;
                    //msg = string.Format("The contents of the file have the same signature as that of file '{0}' which has already been processed!. ",
                    //                     Path.GetFileName(rfml.FileName));
                    rfml.StatusVerbose = "Duplicate! " + Path.GetFileName(fileFullName);

                    // RMCycle number already set...

                    RFMLSeqNo = rfml.Insert();
                    if (rfml.ErrorFound)
                    {
                        msg = string.Format("Processing stopped! " +
                                            "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                             rfml.ErrorOutput);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        return ProcessFileResult.SQL_Error;
                    }
                    #endregion

                    return ProcessFileResult.IsDuplicate;
                }
            }
            #endregion

            #region Check if system is ready to accept the new file
            // Check in the ReconcCategoryVsSourceFiles
            // Categories where this file participates in must all be in either ProcessMode = 1 or ProcessMode = -1 state.
            //
            string SelectionCriteria = string.Format("SourceFileName = '{0}' AND ProcessMode <> 1 AND ProcessMode <> -1", RFMStartPoint.rsf.SourceFileId);

            rcs = new RRDMMatchingCategoriesVsSourcesFiles();

            rcs.ReadReconcCategoryVsSourcesANDFillTable(SelectionCriteria);

            if (rcs.ErrorFound)
            {
                // SQL error ... abort
                msg = string.Format("Processing stopped! " +
                                    "\nCould not read from the 'RRDMMatchingCategoriesVsSourcesFiles' table!\nSelection Criteria: {0}\nThe received message is:\n{1}",
                                     SelectionCriteria, rcs.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Error;
            }
            if (rcs.RecordFound) // meaning at least one file in the category has pending matching actions (is neither 1 nor -1)
            {
                // A number of categories are not in ProcessMode = 1 state
                string msg1 = string.Format("Processing stopped! " +
                                     "\nA number of Reconciliation Categories which depend on the intercepted file '{0}' are still in process!\n",
                                     fileFullName);
                foreach (DataRow dr in rcs.RMCategoryFilesDataFiles.Rows)
                {
                    msg1 = msg1 + string.Format("Category ID: {0}\tSrcFileId: {1}\tState: '{2}'\n", dr["RMCategId"], dr["FileName"], dr["ProcessMode"]);
                }
                // ToDo:
                // Avoid Recording the event (for this File) in subsequent iterations of top level While-Loop
                // Add an entry to a global collection/array and check before recording event; delete entry if ready for processing
                RFMFunctions.RecordEventMsg(msg1, EventLogEntryType.Error);

                #region Insert a row in the File Monitor Log table
                if (!rfml.RecordFound) // only once
                {
                    rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                    rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                    rfml.FileName = fileName;
                    rfml.FileSize = (Int32)fileSize;
                    // rfml.DateTimeReceived = DateTime.Now; set at entry to the method
                    rfml.DateExpected = expectedDate;
                    rfml.DateOfFile = dateOfFile;
                    rfml.FileHASH = HASHValue;
                    rfml.Status = 0;


                    //msg = string.Format("A number of Reconciliation Categories which depend on the intercepted file '{0}' are still in process! ",
                    //                     Path.GetFileName(rfml.FileName));
                    rfml.StatusVerbose = "Reconciliation(s) Pending! " + Path.GetFileName(fileFullName);
                    rfml.ArchivedPath = "";
                    rfml.LineCount = 0;

                    rfml.stpFuid = 0;
                    rfml.ExceptionPath = "";

                    // RMCycle number already set...

                    RFMLSeqNo = rfml.Insert();
                    if (rfml.ErrorFound)
                    {
                        msg = string.Format("Processing stopped! " +
                                            "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                             rfml.ErrorOutput);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        return ProcessFileResult.SQL_Error;
                    }
                }
                #endregion

                return ProcessFileResult.ReconciliationInProgress;
            }
            #endregion

            #region TODO: Validations
            // ToDo: Validate, Transform file, ...; needs extra design
            #endregion

            #region Insert a row in the File Monitor Log table
            // This will be updated with the remaining values afer processing
            // We INSERT now in order to get the FileUID to save as OriginFileName in the corresponding 'Initial-UNIV/RAW' table
            // The FileUID is in the form 'FUID#######' where ####### is the SeqNo padded with zeroes
            // Fill in the column values
            //RRDMReconcFileMonitorLog rfml = new RRDMReconcFileMonitorLog();
            rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
            rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
            rfml.FileName = fileName;
            rfml.FileSize = (Int32)fileSize;
            // rfml.DateTimeReceived = DateTime.Now; set at entry to the method
            rfml.DateExpected = expectedDate;
            rfml.DateOfFile = dateOfFile;
            rfml.FileHASH = HASHValue;
            rfml.Status = 0; // 
            rfml.StatusVerbose = "";
            rfml.ArchivedPath = "";
            rfml.LineCount = 0;

            rfml.stpFuid = 0;
            rfml.ExceptionPath = "";

            // RMCycle number already set...

            RFMLSeqNo = rfml.Insert(); // store RFMLSeqNo; will be used in subsequent 'UPDATE'
            if (rfml.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                    "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                     rfml.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Error;
            }
            #endregion

            #region Process Source File
            FileUID = string.Format("FUID{0}", RFMLSeqNo.ToString("0000000"));

            if (RFMStartPoint.glbRawImport != true)
            {
                // Get List of eligible ATMs
                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.ReadAtmAndFillTableForImportValidation(RFMStartPoint.argOperator);
                DataTable ATMsTbl = Ac.ATMsDetailsDataTable;

                switch (SourceFileID)
                {
                    case "Switch_Base24_Txns":
                        {
                            // Read a delimiter file and insert records in table
                            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

                            // Ms.ReadReconcSourceFilesByFileId("Switch_Base24_Txns");
                            // TESTING
                            // ToDo: Update database....
                            Ms.ReadReconcSourceFilesByFileId("Switch_Base24_Txns");
                            if (Ms.LayoutId == "DelimiterFile")
                            {
                                RRDMMatchingTxns_InGeneralTables_BDC Del = new RRDMMatchingTxns_InGeneralTables_BDC();
                            //    Del.InsertRecordsInTableFromTextFile_InBulk(Ms.SourceFileId, Ms.SourceDirectory, Ms.InportTableName, Ms.Delimiter);

                                if (Del.ErrorFound)
                                {
                                    isSuccess = false;

                                    msg = string.Format("Processing source file {0} encountered errors in 'InsertRecordsInTableFromTextFile_InBulk'!\r\nThe error reads: {1}",
                                        originalFullFileName, Del.ErrorOutput);
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                }
                                else
                                {
                                    // LineCount = Del.stpLineCount ; // ToDo: The method to return this value
                                    LineCount = 10; 
                                    isSuccess = true;
                                }
                            }
                            else
                            {
                                ProcessSourceFile_Switch_Base24 pSFObj = new ProcessSourceFile_Switch_Base24(RFMStartPoint.argOperator, Origin, FileUID,
                                                                                                             SourceFileID, fileFullName,
                                                                                                             LinesInHeader, LinesInTrailer,
                                                                                                             RFMStartPoint.rsf.InportTableName,
                                                                                                             ATMsTbl, RFMStartPoint.glbMaxPagingRows);
                                LineCount = pSFObj.LinesProcessed;
                                isSuccess = pSFObj.IsSuccess;
                            }
                            break;
                        }
                    case "Core_Banking_T24_Txns":
                        {
                            // Read a delimiter file and insert records in table
                            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();
                            Ms.ReadReconcSourceFilesByFileId("Core_Banking_T24_Txns");
                            if (Ms.LayoutId == "DelimiterFile")
                            {
                                RRDMMatchingTxns_InGeneralTables_BDC Del = new RRDMMatchingTxns_InGeneralTables_BDC();
                            //    Del.InsertRecordsInTableFromTextFile_InBulk(Ms.SourceFileId, Ms.SourceDirectory, Ms.InportTableName, Ms.Delimiter);

                                if (Del.ErrorFound)
                                {
                                    isSuccess = false;
                                    msg = string.Format("Processing source file {0} encountered errors in 'InsertRecordsInTableFromTextFile_InBulk'!\r\nThe error reads: {1}",
                                        originalFullFileName, Del.ErrorOutput);
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                                }
                                else
                                {
                                    // LineCount = Del.stpLineCount; // ToDo: The method to return this value
                                    LineCount = 10;
                                    isSuccess = true;

                                }
                            }
                            else
                            {
                                ProcessSourceFile_Core_Banking_T24 pSFObj = new ProcessSourceFile_Core_Banking_T24(RFMStartPoint.argOperator, Origin, FileUID,
                                                                                                               SourceFileID, fileFullName,
                                                                                                               LinesInHeader, LinesInTrailer,
                                                                                                               RFMStartPoint.rsf.InportTableName,
                                                                                                               ATMsTbl, RFMStartPoint.glbMaxPagingRows);
                                LineCount = pSFObj.LinesProcessed;
                                isSuccess = pSFObj.IsSuccess;
                            }
                            break;
                        }
                    default:
                        {
                            //ProcessSourceFile_Generic pSFObj = new ProcessSourceFile_Generic(RFMStartPoint.argOperator, Origin, FileUID, SourceFileID, fileFullName, RFMStartPoint.rsf.InportTableName);
                            //LineCount = pSFObj.LinesProcessed;
                            //isSuccess = pSFObj.IsSuccess;
                            isSuccess = false;
                            break;
                        }
                }
            }
            else
            {
                ejFUID = ProcessImportRAWJournal.ProcessLocalJournalFile(RFMStartPoint.glbStoredProc, eJournalType, RFMStartPoint.argOperator, ATM_No, fileFullName);
                if (ejFUID != -1)
                {
                    isSuccess = true;
                }
            }

            if (!isSuccess)
            {
                // Move file to Exceptions dir so it is not processed again...
                string fRen = RFMFunctions.MoveFileToExceptionsDir(fileFullName);

                // Log the error
                if (!RFMStartPoint.glbRawImport)
                {
                    msg = string.Format("Processing source file {0} stopped because of encountered errors! \r\nThe file has been renamed to '{1}'. The errors were recorded in '{2}.Log'.",
                                         originalFullFileName, fRen, fRen);
                }
                else
                {
                    msg = string.Format("Processing source EJ file {0} stopped because of encountered errors! \r\nThe file has been renamed to '{1}'.",
                                         originalFullFileName, fRen);
                }
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);


                rfml.FileHASH = HASHValue;
                rfml.Status = 0;
                rfml.StatusVerbose = "Parsing Process Failed! " + Path.GetFileName(rfml.FileName);
                rfml.ArchivedPath = "";
                // To do
                //rfml.ExceptionPath = 
                rfml.LineCount = LineCount;
                rfml.stpFuid = ejFUID;


                rfml.Update(RFMLSeqNo);
                if (rfml.ErrorFound)
                {
                    msg = string.Format("Processing stopped! " +
                                        "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                         rfml.ErrorOutput);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return ProcessFileResult.SQL_Error;
                }

                return ProcessFileResult.ProcessingError;
            }
            #endregion

            #region Update ProcessMode in the ReconcCategoriesVsSources table
            //Set the ProcessMode of SourceFileId in ALL Categories to -1!
            RRDMMatchingCategoriesVsSourcesFiles rcs1 = new RRDMMatchingCategoriesVsSourcesFiles();
            rcs1.UpdateReconcCategoryVsSourceRecordProcessCodeForSourceFileName(RFMStartPoint.rsf.SourceFileId, -1);

            if (rcs1.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                     "\nCould not UPDATE the ProcessMode column of the 'RRDMMatchingCategoriesVsSourcesFiles' table row for SourceFileId: '{0}'\nThe received message is:\n{1}",
                                     RFMStartPoint.rsf.SourceFileId, rcs1.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Error;
            }
            #endregion

            #region Archive the file and then delete
            // Get the file size
            // FileInfo f = new FileInfo(fileFullName);
            // fileSize = (int)f.Length;

            // Archive
            string archFullPath = "";
            if (RFMStartPoint.glbArchiveEnabled)
            {
                archFullPath = RFMStartPoint.rsf.ArchiveDirectory + "\\" + fileName;
                retValue = RFMFunctions.ArchiveFile(fileFullName, archFullPath);
                if (retValue)
                {
                    // Delete source file
                    try
                    {
                        File.Delete(fileFullName);
                    }
                    catch (IOException)
                    {
                        retVal = ProcessFileResult.IO_Error_SourceFleMove;
                    }
                }
                else
                {
                    archFullPath = "";
                    retVal = ProcessFileResult.IO_Error_ArchiveFile;
                }
            }
            else
            {
                archFullPath = "";
                // Delete source file
                try
                {
                    File.Delete(fileFullName);
                }
                catch (IOException)
                {
                    retVal = ProcessFileResult.IO_Error_SourceFleMove;
                }
            }
            #endregion

            #region Update FileMonitorLog row in the File Monitor Log table
            // Fill in the column values that were not set in the INSERT above
            //rfml.SystemOfOrigin = rsf.SystemOfOrigin;
            //rfml.SourceFileID = rsf.SourceFileId;
            //rfml.FileName = fileName;
            //rfml.FileSize = fileSize;
            //rfml.DateTimeReceived = DateTime.Now;
            //rfml.FileHASH = HASHValue;
            //rfml.Status = 1; // 
            //rfml.ArchivedPath = archFullPath;
            //rfml.LineCount = LineCount;

            rfml.FileHASH = HASHValue;
            rfml.Status = 1;
            rfml.StatusVerbose = "Processed Successfully! " + Path.GetFileName(fileFullName);
            rfml.ArchivedPath = archFullPath;
            rfml.ExceptionPath = "";
            rfml.LineCount = LineCount;
            rfml.stpFuid = ejFUID;

            rfml.Update(RFMLSeqNo);
            if (rfml.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                        "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                     rfml.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Error;
            }
            #endregion

            return retVal;
        }
        #endregion

        #region ToDo: Validate File
        private static bool ValidateFile(string FileName)
        {
            //Todo: Validate source file
            return (true);
        }
        #endregion


    }
}

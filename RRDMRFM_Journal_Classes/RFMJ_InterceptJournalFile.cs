using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRDM4ATMs;
using System.IO;
using System.Diagnostics;
using System.Data;

namespace RRDMRFM_Journal_Classes
{
    
    public class InterceptJournalFile
    {
        
        private int ThreadIndx;

        public InterceptJournalFile(int indx)
        {
            ThreadIndx = indx;
        }

        #region Intercept File
        public ProcessFileResult InterceptFile(string fileFullPath, string ATM_No, string eJournalType)
        {
            // bool retValue;
            string msg;
            long fileSize = 0;
            int LineCount = 0;
            int stpRC = 0;

            bool isSuccess = false;
            string FileUID;
            string HASHValue;
            string Origin = RfmjServer.rsf.SystemOfOrigin;
            string SourceFileID = RfmjServer.rsf.SourceFileId;
            int RFMLSeqNo = 0; // ReconcFileMonitorLog record id
            string originalFullFileName = fileFullPath;
            string fileName = Path.GetFileName(fileFullPath);
            string resultArchFilePath = "";

            string dateOfFile = "";
            DateTime expectedDate = DateTime.Now;

            ProcessFileResult retVal = ProcessFileResult.Success;
            RRDMReconcFileMonitorLog rfml = new RRDMReconcFileMonitorLog();
            rfml.DateTimeReceived = DateTime.Now;

            #region Update the Thread worksheet with Status, Step
            int stg = RfmjActionStage.Const_Step_2_InProgress;
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Step_2_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Step_2_Start = DateTime.Now;
            #endregion
           
            #region Get Reconciliation Cycle and Cutoff Date
            string JobCategory = "ATMs";
            DateTime cutOffDate;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int ReconCycle = 0;
            ReconCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(RfmjServer.argOperator, JobCategory);
            if (ReconCycle != 0)
            {
                cutOffDate = Rjc.Cut_Off_Date.Date;
                rfml.RMCycleNo = ReconCycle;
            }
            else
            {
                msg = string.Format("Stopped processing file: {0} from OriginSystem: {1}! \r\n" +
                                    "Could not identify the Reconciliation Cycle! The Windows Event and/or the application Log may contain futher details...",
                                     Path.GetFileNameWithoutExtension(fileFullPath), RfmjServer.argOrigin);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                // We return without signaling back to the caller that this thread has started! The caller will time out..
                RfmjThreadRegistry.ChangeThreadStatus(ThreadIndx, StatusOfThread.Aborting);
                return ProcessFileResult.ReconciliationCycleError;
            }
            #endregion

            #region Check if File is Empty
            try
            {
                FileInfo fInf = new FileInfo(fileFullPath);
                fileSize = fInf.Length;
            }
            catch (FileNotFoundException ex)
            {
                msg = string.Format("Exception: File not Found [{0}]! [{1}]", fileFullPath, ex.Message);
                // EventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                return ProcessFileResult.ATMJournalSkipped;
            }
            catch (Exception ex)
            {
                string msg1 = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    msg1 = msg1 + "\\r\\n" + ex1.Message;
                    ex1 = ex1.InnerException;
                }

                msg = string.Format("Exception: FileInfo [{0}]! \r\nDetails:\r\n{1}", fileFullPath, msg1);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.ATMJournalSkipped;
            }

            if (fileSize == 0)
            {
                // Empty file ... abort
                // Move file to Exceptions dir so it is not processed again...
                string failedFileName = RFMJFunctions.MoveFileToExceptionsByCycle(fileFullPath, ReconCycle, cutOffDate);

                // Log the error
                msg = string.Format("Processing of intercepted file {0} stopped! The file is empty!" +
                                     "\r\nThe file has been moved to '{1}'.",
                                     Path.GetFileName(fileName), failedFileName);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                #region Insert a row in the File Monitor Log table
                rfml.SystemOfOrigin = RfmjServer.rsf.SystemOfOrigin;
                rfml.SourceFileID = RfmjServer.rsf.SourceFileId;
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
                rfml.StatusVerbose = "Empty File! " + Path.GetFileName(fileFullPath);

                // RMCycle number already set...

                RFMLSeqNo = rfml.Insert();
                if (rfml.ErrorFound)
                {
                    msg = string.Format("Processing stopped! " +
                                        "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                         rfml.ErrorOutput);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    return ProcessFileResult.SQL_Exception;
                }
                #endregion

                return ProcessFileResult.File_Empty;
            }
            #endregion

            #region Extract Date from filename
            dateOfFile = "";
            try
            {
                DateTime dfDT;
                dfDT = RFMJFunctions.eXractDateFromString(Path.GetFileName(fileName));
                dateOfFile = dfDT.ToString("yyyy-MM-dd");
            }
            catch (DivideByZeroException)
            {
                dateOfFile = "Invalid!";
            }
            catch (Exception)
            {
                dateOfFile = "Invalid!";
            }
            if (dateOfFile == "Invalid!")
            {
                msg = string.Format("File [{0}] has an Invalid Name (Date must be formatted as: YYYYMMDD).", fileName);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                // Move to Exceptions Dir
                string fRen = RFMJFunctions.MoveFileToExceptionsByCycle(fileFullPath, ReconCycle, cutOffDate);
                msg = string.Format("Processing source EJ file [{0}] stopped because of encountered errors! \r\nThe file has been moved to '{1}'.",
                       originalFullFileName, fRen);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                return (ProcessFileResult.InvalidFileDateFormat);
            }
            #endregion

            #region Record the event in the EventLog ..
            msg = string.Format("\r\nNew File: {0} from OriginSystem: {1} and SourceFileID: {2} was intercepted!", fileName, Origin, SourceFileID);
            EventLogging.RecordEventMsg(msg, EventLogEntryType.Information);
            #endregion

            #region Calculate SHA256 hash value
            // Calculate SHA256 hash value
            HASHValue = FileHASH.BytesToString(FileHASH.GetHashSha256(fileFullPath));
            #endregion

            #region Check if there is already a thread processing a file with same HASH
            if (RfmjThreadRegistry.IsHASHBeingProcessed(HASHValue))
            {
                msg = string.Format("Another file with the same HASH value is currrently being processed! Processing of file {0} is abandoned!", fileFullPath);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.BeingProcessedByAnotherThread;
            }
            RfmjThreadRegistry.SetHASHValue(this.ThreadIndx, HASHValue);
            #endregion

            #region Expected Date
            RRDMMatchingCategoriesVsSourcesFiles rcs;
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

                #region // 1: (This program is ONLY for ATM Journals; 'Expected Date' is not checked!!!!!!)
                //if (RFMStartPoint.glbRawImport != true) // Check only for non-eJournals
                //{
                //    if (!fileName.Contains(dateForm1) && !fileName.Contains(dateForm2))
                //    {
                //        // NOT the expected date  Move file to exceptions dir and return
                //        string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);

                //        ////change extension of the source file to'.NotExpected' so it is not processed again...
                //        //try
                //        //{
                //        //    failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".NotExpected"); // Network drives?
                //        //    File.Move(fileFullName, failedFileName);
                //        //}
                //        //catch (IOException ex)
                //        //{
                //        //    //The destination file already exists. 
                //        //    File.Delete(fileFullName);
                //        //}
                //        //catch (Exception ex)
                //        //{
                //        //    // Rename failed! Same filename and extension must already exist. Delete this one...
                //        //    msg = string.Format("Exception while renaming file [{0}] to [{1}]! Exception Message: {2}", fileFullName, failedFileName, ex.Message);
                //        //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                //        //    File.Delete(fileFullName);
                //        //}

                //        // Log the error
                //        msg = string.Format("Processing of intercepted file stopped! The filename's date signature is not the one expected! " +
                //                             "\r\nThe expected filename should contain either '{0}' or '{1}'." +
                //                             "\r\nThe intercepted file has been moved to '{2}'.",
                //                             dateForm1, dateForm2, failedFileName);
                //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                //        #region Insert a row in the File Monitor Log table
                //        rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                //        rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                //        rfml.FileName = fileName;
                //        rfml.FileSize = (Int32)fileSize;
                //        rfml.DateExpected = expectedDate;
                //        rfml.DateOfFile = dateOfFile;
                //        rfml.FileHASH = HASHValue;
                //        rfml.Status = 0;
                //        //msg = string.Format("The filename should contain either '{0}' or '{1}'. ", dateForm1, dateForm2);
                //        //if (dateOfFile == "Invalid!")
                //        //{
                //        //    msg += string.Format("Instead in contained an invalid date signature!");
                //        //}
                //        //else
                //        //{
                //        //    msg += string.Format("Instead in contained: {0}", dateOfFile);
                //        //}
                //        msg = " Expected Date: " + expectedDate.ToShortDateString();
                //        if (dateOfFile == "Invalid!")
                //        {
                //            msg += (" Found: Invalid 'date' value in the filename!");
                //        }
                //        else
                //        {
                //            msg += string.Format(" Found: {0}", dateOfFile);
                //        }

                //        rfml.StatusVerbose = "Not Expected! " + Path.GetFileName(fileFullName) + msg;
                //        rfml.ArchivedPath = "";
                //        rfml.ExceptionPath = failedFileName;
                //        rfml.LineCount = 0;
                //        rfml.ParsedFuid = 0;
                //        // RMCycle number already set...

                //        RFMLSeqNo = rfml.Insert();
                //        if (rfml.ErrorFound)
                //        {
                //            msg = string.Format("Processing stopped! " +
                //                                "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                //                                 rfml.ErrorOutput);
                //            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                //            return ProcessFileResult.SQL_Error;
                //        }
                //        #endregion

                //        return ProcessFileResult.NotExpected;
                //    }
                //}
                #endregion
            }
            else
            {
                #region // 2: (This program is ONLY for ATM Journals; 'Expected Date' is not checked!!!!!!)
                // Expected Date not available (Record not found! )

                //if (ATM_No == "") // Not ATMs, abort
                //{
                //    // Move file to Exceptions dir so it is not processed again...
                //    string failedFileName = RFMFunctions.MoveFileToExceptionsDir(fileFullName);
                //    //
                //    //try
                //    //{
                //    //    failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".Failed"); // Network drives?
                //    //    File.Move(fileFullName, failedFileName);
                //    //}
                //    //catch (IOException ex)
                //    //{
                //    //    //The destination file already exists. 
                //    //    File.Delete(fileFullName);
                //    //}
                //    //catch (Exception ex)
                //    //{
                //    //    // Rename failed! Same filename and extension must already exist. Delete this one...
                //    //    msg = string.Format("Exception while renaming file [{0}] to [{1}]! Exception Message: {2}", fileFullName, failedFileName, ex.Message);
                //    //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                //    //    File.Delete(fileFullName);
                //    //}

                //    // Log the error
                //    msg = string.Format("Processing of intercepted file stopped! \r\n" +
                //                        "Could not establish the the 'Expected Date' of the file because no record was found in 'MatchingCategoriesVsSourceFile' table!." +
                //                        "\r\nThe file has been moved to '{0}'.", failedFileName);
                //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                //    #region Insert a row in the File Monitor Log table
                //    rfml.SystemOfOrigin = RFMStartPoint.rsf.SystemOfOrigin;
                //    rfml.SourceFileID = RFMStartPoint.rsf.SourceFileId;
                //    rfml.FileName = fileName;
                //    rfml.FileSize = (Int32)fileSize;
                //    // rfml.DateTimeReceived = DateTime.Now; set at entry to the method
                //    rfml.DateExpected = expectedDate;
                //    rfml.DateOfFile = dateOfFile;
                //    rfml.FileHASH = HASHValue;
                //    rfml.Status = 0;
                //    //msg = string.Format("Could not establish the the 'Expected Date' of the file. No record of Origin='{0}' and FileID'{1}' was found in the database.",
                //    //    RFMStartPoint.rsf.SystemOfOrigin, RFMStartPoint.rsf.SourceFileId);
                //    msg = " Expected Date: Could not establish!";

                //    rfml.StatusVerbose = "Not Expected! " + Path.GetFileName(fileFullName) + msg;
                //    rfml.ArchivedPath = "";
                //    rfml.ExceptionPath = failedFileName;
                //    rfml.LineCount = 0;
                //    rfml.ParsedFuid = 0;


                //    // RMCycle number already set...

                //    RFMLSeqNo = rfml.Insert();
                //    if (rfml.ErrorFound)
                //    {
                //        msg = string.Format("Processing stopped! " +
                //                            "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                //                             rfml.ErrorOutput);
                //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                //        return ProcessFileResult.SQL_Error;
                //    }
                //    #endregion

                //    return ProcessFileResult.NotExpected;
                //}
                #endregion 2
            }
            #endregion

            #region Check HASH to find out if the file has already been processed
            // Use the HASH to retieve a row from table
            rfml.GetRecordByFileHASH(HASHValue);
            if (rfml.ErrorFound)
            {
                // error reading ... abort
                msg = string.Format("Processing stopped! " +
                                     "\nCould not validate the file HASH value because the database reports:\n{0}", rfml.ErrorOutput);

                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Exception;
            }
            else
            {
                // if (rfml.RecordFound && rfml.Status != 0)
                if (rfml.RecordFound && rfml.Status == 1)
                {
                    // int originalRfmlStatus = rfml.Status;

                    // FileHASH already exists ... 
                    // Move file to Exceptions dir so it is not processed again...

                    string failedFileName = RFMJFunctions.MoveFileToExceptionsByCycle(fileFullPath, ReconCycle, cutOffDate);

                    // Log the error
                    msg = string.Format("Processing of intercepted file stopped! Cannot process the same file twice!" +
                                         "\r\nThe file HASH value of the intercepted file [{0}] already exists in the database for a file named [{1}]." +
                                         "\r\nThe file has been moved to '{2}'.",
                                         fileName, rfml.FileName, failedFileName);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                    #region Insert a row in the File Monitor Log table
                    rfml.SystemOfOrigin = RfmjServer.rsf.SystemOfOrigin;
                    rfml.SourceFileID = RfmjServer.rsf.SourceFileId;
                    rfml.FileName = fileName;
                    rfml.FileSize = (Int32)fileSize;
                    // rfml.DateTimeReceived = DateTime.Now; set at entry to the methodrfml.DateTimeReceived = DateTime.Now;
                    rfml.DateExpected = expectedDate;
                    rfml.DateOfFile = dateOfFile;
                    rfml.FileHASH = HASHValue;
                    rfml.RMCycleNo = ReconCycle;

                    // rfml.Status = originalRfmlStatus;
                    rfml.Status = 0;

                    rfml.ArchivedPath = "";
                    rfml.ExceptionPath = failedFileName;
                    rfml.stpFuid = 0;
                    rfml.LineCount = 0;
                    //msg = string.Format("The contents of the file have the same signature as that of file '{0}' which has already been processed!. ",
                    //                     Path.GetFileName(rfml.FileName));
                    rfml.StatusVerbose = "Duplicate! " + Path.GetFileName(fileFullPath);

                    // RMCycle number already set...

                    RFMLSeqNo = rfml.Insert();
                    if (rfml.ErrorFound)
                    {
                        msg = string.Format("Processing stopped! " +
                                            "An error occured while INSERTING in the ReconcFileMonitorLog table. The error message is : {0}",
                                             rfml.ErrorOutput);
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        return ProcessFileResult.SQL_Exception;
                    }
                    #endregion

                    return ProcessFileResult.IsDuplicate;
                }
            }
            #endregion

            #region Check if system is ready to accept the new file
            /*
            // Check in the ReconcCategoryVsSourceFiles
            // Categories where this file participates in must all be in either ProcessMode = 1 or ProcessMode = -1 state.
            //
            string SelectionCriteria = string.Format("SourceFileName = '{0}' AND ProcessMode <> 1 AND ProcessMode <> -1", RfmjServer.rsf.SourceFileId);

            rcs = new RRDMMatchingCategoriesVsSourcesFiles();

            rcs.ReadReconcCategoryVsSourcesANDFillTable(SelectionCriteria);

            if (rcs.ErrorFound)
            {
                // SQL error ... abort
                msg = string.Format("Processing stopped! " +
                                    "\nCould not read from the 'RRDMMatchingCategoriesVsSourcesFiles' table!\nSelection Criteria: {0}\nThe received message is:\n{1}",
                                     SelectionCriteria, rcs.ErrorOutput);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Exception;
            }
            if (rcs.RecordFound) // meaning at least one file in the category has pending matching actions (is neither 1 nor -1)
            {
                // A number of categories are not in ProcessMode = 1 state
                string msg1 = string.Format("Processing stopped! " +
                                     "\nA number of Reconciliation Categories which depend on the intercepted file '{0}' are still in process!\n",
                                     fileFullPath);
                foreach (DataRow dr in rcs.RMCategoryFilesDataFiles.Rows)
                {
                    msg1 = msg1 + string.Format("Category ID: {0}\tSrcFileId: {1}\tState: '{2}'\n", dr["RMCategId"], dr["FileName"], dr["ProcessMode"]);
                }
                // ToDo:
                // Avoid Recording the event (for this File) in subsequent iterations of top level While-Loop
                // Add an entry to a global collection/array and check before recording event; delete entry if ready for processing
                EventLogging.RecordEventMsg(msg1, EventLogEntryType.Error);

                #region Insert a row in the File Monitor Log table
                if (!rfml.RecordFound) // only once
                {
                    rfml.SystemOfOrigin = RfmjServer.rsf.SystemOfOrigin;
                    rfml.SourceFileID = RfmjServer.rsf.SourceFileId;
                    rfml.FileName = fileName;
                    rfml.FileSize = (Int32)fileSize;
                    // rfml.DateTimeReceived = DateTime.Now; set at entry to the method
                    rfml.DateExpected = expectedDate;
                    rfml.DateOfFile = dateOfFile;
                    rfml.FileHASH = HASHValue;
                    rfml.Status = 0;


                    //msg = string.Format("A number of Reconciliation Categories which depend on the intercepted file '{0}' are still in process! ",
                    //                     Path.GetFileName(rfml.FileName));
                    rfml.StatusVerbose = "Reconciliation(s) Pending! " + Path.GetFileName(fileFullPath);
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
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        return ProcessFileResult.SQL_Exception;
                    }
                }
                #endregion

                return ProcessFileResult.ReconciliationInProgress;
            }
            */
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
            rfml.SystemOfOrigin = RfmjServer.rsf.SystemOfOrigin;
            rfml.SourceFileID = RfmjServer.rsf.SourceFileId;
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
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Exception;
            }
            #endregion

            #region Create a new file with sequence number in front of each line
            // Add sequence number in front of each line of the line
            string jlnFullPathName;
            RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            jlnFullPathName = Jrt.ConvertJournal(fileFullPath); // Converted File 
            if (Jrt.ErrorFound)
            {
                msg = Jrt.ErrorOutput;
                // Log the error
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                return ProcessFileResult.JLN_IO_Error;
            }
            LineCount = Jrt.LineCounter;
            #endregion

            #region Process Source File
            FileUID = string.Format("FUID{0}", RFMLSeqNo.ToString("0000000"));
            ProcessJournalFile pJF = new ProcessJournalFile(ThreadIndx);
            // Process Journal 
            stpRC = pJF.ProcessLocalJournalFile(RfmjServer.glbStoredProc, eJournalType, RfmjServer.argOperator, ATM_No, jlnFullPathName);

            // Delete the "jln" file irrespective of success or failure of the processing...
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "930";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(RfmjServer.argOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
                File.Delete(jlnFullPathName);

            // pJF.stpErrorReference
            if (stpRC != -1)
            {
                isSuccess = true;

                // Call Loading of Txns

                ParId = "931";
                OccurId = "1";
                Gp.ReadParametersSpecificId(RfmjServer.argOperator, ParId, OccurId, "", "");
                if (Gp.OccuranceNm == "YES")
                {
                    msg = string.Format("Bypassing JrNew.ReadJournal_Txns_And_Insert_In_Pool");
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);

                    isSuccess = true;
                    /*
                    // Call Tranastions loading after Pambos
                    RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2 JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2();
                    int WAtmsRecGroup = 0;
                    int WMode = 3;
                    int WSignRecordNo = 931;
                    
                    JrNew.ReadJournal_Txns_And_Insert_In_Pool("RFMSysUser", WSignRecordNo, RfmjServer.argOperator, WAtmsRecGroup, ATM_No, stpRC, WMode);
                    if (JrNew.Major_ErrorFound)
                    {
                        isSuccess = false;
                        // Log the error
                        msg = string.Format("Processing source EJ file {0} encountered errors in '{1}'!\r\nThe error output reads: {2}",
                                               originalFullFileName, "ReadJournal_Txns_And_Insert_In_Pool", JrNew.ErrorOutput);
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    }
                    */
                }

            }

            if (!isSuccess)
            {
                // Move file to Exceptions dir so it is not processed again...
                string fRen = RFMJFunctions.MoveFileToExceptionsByCycle(fileFullPath, rfml.RMCycleNo, cutOffDate);

                // Log the error
                msg = string.Format("Processing source EJ file {0} stopped because of encountered errors! \r\nThe file has been renamed to '{1}'.",
                                       originalFullFileName, fRen);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                rfml.FileHASH = HASHValue;
                rfml.Status = 0;
                rfml.StatusVerbose = "Parsing Process Failed! " + Path.GetFileName(rfml.FileName);
                rfml.ArchivedPath = "";
                // To do
                //rfml.ExceptionPath = 
                rfml.ExceptionPath = fRen;
                rfml.LineCount = LineCount;

                rfml.stpFuid = pJF.stpReturnFUID;
                rfml.stpReturnCode = pJF.stpReturnCode;
                rfml.stpErrorText = pJF.stpErrorText;
                rfml.stpReferenceCode = pJF.stpErrorReference;

                // rfml.stpReturnCode = 0;

                rfml.Update(RFMLSeqNo);
                if (rfml.ErrorFound)
                {
                    msg = string.Format("Processing stopped! " +
                                        "An error occured while UPDATING the ReconcFileMonitorLog table. The error message is : {0}",
                                         rfml.ErrorOutput);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    return ProcessFileResult.SQL_Exception;
                }
                return ProcessFileResult.SP_Error;
            }
            #endregion

            #region Update ProcessMode in the ReconcCategoriesVsSources table
            //Set the ProcessMode of SourceFileId in ALL Categories to -1!
            RRDMMatchingCategoriesVsSourcesFiles rcs1 = new RRDMMatchingCategoriesVsSourcesFiles();
            rcs1.UpdateReconcCategoryVsSourceRecordProcessCodeForSourceFileName(RfmjServer.rsf.SourceFileId, -1);

            if (rcs1.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                     "\nCould not UPDATE the ProcessMode column of the 'RRDMMatchingCategoriesVsSourcesFiles' table row for SourceFileId: '{0}'\nThe received message is:\n{1}",
                                      RfmjServer.rsf.SourceFileId, rcs1.ErrorOutput);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Exception;
            }
            #endregion

            #region Archive the file and then delete
            // Archive
            string archFullPath = "";
            if (RfmjServer.glbArchiveEnabled)
            {
                archFullPath = RfmjServer.rsf.ArchiveDirectory + "\\" + fileName;
                resultArchFilePath = RFMJFunctions.ArchiveFileByCycle(fileFullPath, archFullPath, rfml.RMCycleNo, cutOffDate);
                if (!string.IsNullOrEmpty(resultArchFilePath))
                {
                    archFullPath = resultArchFilePath;
                    // Delete source file
                    try
                    {
                        File.Delete(fileFullPath);
                    }
                    catch (IOException ex)
                    {
                        msg = string.Format("An IO exception occured while moving the source file to the Archive. The file copied to the Archive but could not be deleted from the FilePool\r\n" +
                            "The file is {0}\r\nThe Exception Trace is:\r\n{1}", fileFullPath, ex.StackTrace);
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        retVal = ProcessFileResult.IO_Error_SourceFileDelete;
                    }
                }
                else
                {
                    archFullPath = "";
                    msg = string.Format("An IO exception occured while moving the source file to the Archive. The file is {0}", fileFullPath);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    retVal = ProcessFileResult.IO_Error_ArchiveFile;
                }
            }
            else
            {
                archFullPath = "";
                // Delete source file
                try
                {
                    File.Delete(fileFullPath);
                }
                catch (IOException ex)
                {
                    msg = string.Format("An IO exception occured while moving the source file to the Archive.\r\n" + 
                        "The file copied to the Archive but could not be deleted from the FilePool\r\n" +
                        "The file is {0}\r\nThe Exception Trace is:\r\n{1}", fileFullPath, ex.StackTrace);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    retVal = ProcessFileResult.IO_Error_SourceFileDelete;
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
            rfml.StatusVerbose = "Processed Successfully! " + Path.GetFileName(fileFullPath);
            rfml.ArchivedPath = resultArchFilePath;
            rfml.ExceptionPath = "";
            rfml.LineCount = LineCount;

            // rfml.stpFuid = stpRC;
            rfml.stpFuid = pJF.stpReturnFUID;
            rfml.stpReturnCode = pJF.stpReturnCode;
            rfml.stpErrorText = pJF.stpErrorText;
            rfml.stpReferenceCode = pJF.stpErrorReference;

            rfml.Update(RFMLSeqNo);
            if (rfml.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                        "An error occured while UPDATING the ReconcFileMonitorLog table. The error message is : {0}",
                                     rfml.ErrorOutput);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return ProcessFileResult.SQL_Exception;
            }
            #endregion

            #region Update the Thread Status/Step
            stg = RfmjActionStage.Const_Step_2_Finished;
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Step_2_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndx].workSheet.Step_2_End = DateTime.Now;
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

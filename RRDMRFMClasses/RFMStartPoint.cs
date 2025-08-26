using System;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Threading;

using RRDM4ATMs;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RRDMRFMClasses
{
    public class RFMServer
    {
        public Thread StartRFMThread(string orgn, string srcId, string oprtr)
        {
            RFMStartPoint.argOrigin = orgn;
            RFMStartPoint.argSourceFileID = srcId;
            RFMStartPoint.argOperator = oprtr;


            // Event Handle to wait on for thread to start 
            AutoResetEvent hThreadStartedEvent = new AutoResetEvent(false);

            // Start the RFM Server thread
            ParameterizedThreadStart ts = new ParameterizedThreadStart(RFMStartPoint.RFMStartup);

            Thread oT = new Thread(ts);
            oT.Name = "RFMServer";
            oT.Start(hThreadStartedEvent); // pass the event handler by which the thread will signal back...

            // Wait for the thread to signal back 
            // this is how we make sure that the thread initialized properly and is ready to go..
            bool ThrStarted = hThreadStartedEvent.WaitOne(10000);
            if (ThrStarted)
                return (oT);
            else
                return null;
        }
    }

    public static class RFMStartPoint
    {
        #region Static members
        const string PROGRAM_MUTEX_NAME = "Global\\RRDMRFM";
        private static string instanceMutexName = "";

        public static string argOrigin = "";        // argument 1
        public static string argSourceFileID = "";  // argument 2
        public static string argOperator = "";      // argument 3
        public static bool glbRawImport = false;    // derived from glbOrigin (if argOrigin==ATMs)

        public static bool glbArchiveEnabled = false;    // read from GAS Parameters
        public static int glbRFMSleep;              // GAS parameters
        public static string glbStoredProc = "";    // GAS parameters; used if glbRawImport=true
        public static int glbMaxPagingRows = 100;   // GAS parameters

        public static RRDMMatchingSourceFiles rsf;

        public static bool SignalToStop = false; // For handling [Ctrl+C] [Ctrl+Break] and Service STOP
        #endregion


        public static void RFMStartup(object hwStartedEvent)
        {
            // These have been set earlier by the caller
            //argOrigin = _argOrigin;
            //argSourceFileID = _argSourceFileID;
            //argOperator = _argOperator;

            bool IsTheOnlyInstance;
            instanceMutexName = PROGRAM_MUTEX_NAME + "_" + argOrigin + "_" + argSourceFileID;
            try
            {
                Mutex InstanceMutex = new Mutex(true, instanceMutexName, out IsTheOnlyInstance);
            }
            catch (UnauthorizedAccessException ex)
            {
                string msg = string.Format("MUTEX exception: {0}", ex.Message);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                IsTheOnlyInstance = false;
            }

            if (IsTheOnlyInstance == false)
            {
                string msg = string.Format("{0}\r\nAnother instance of this program is already running!\r\nOnly one instance is allowed to run at any time!",
                                           instanceMutexName);
                if (Environment.UserInteractive)
                {
                    MessageBox.Show(msg, "RRDMRFM", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                }
                return;
            }

            #region // For ATM Journals we use the RRDMRFM_Journal Service
            //if (argOrigin == "ATMs")
            //{
            //    glbRawImport = true;
            //}
            #endregion

            // Write an 'information' event in the EventLog
            string mesg = string.Format("Starting RRDM Reconciliation File Monitor  with the following parameters:\n   SystemOfOrigin: {0}\n   SourceFileID: {1}\n   Operator: {2}", argOrigin, argSourceFileID, argOperator);
            RFMFunctions.RecordEventMsg(mesg, EventLogEntryType.Information);


            #region Read the Sleep parameters from database and validate
            RRDMGasParameters Gp = new RRDMGasParameters();
            // How long to sleep before searching for files again (seconds)
            Gp.ReadParametersSpecificId(argOperator, "913", "1", "", "");
            if (!Gp.ErrorFound)
                if (Gp.RecordFound)
                {
                    glbRFMSleep = (int)Gp.Amount;
                }
                else
                {
                    glbRFMSleep = 1; // set to 1 sec
                }
            else
            {
                // Database error!
                string msg = string.Format("Processing stopped!\nError reading parameter: 913 Occur: 1 from parameters table!\nOperator is: '{0}'", argOperator);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            #endregion

            #region Read the MaxPagingRows parameter from database and validate
            // RRDMGasParameters Gp = new RRDMGasParameters();
            // MaxPagingRows
            Gp.ReadParametersSpecificId(argOperator, "913", "3", "", "");
            if (!Gp.ErrorFound)
                if (Gp.RecordFound)
                {
                    glbMaxPagingRows = (int)Gp.Amount;
                }
                else
                {
                    glbMaxPagingRows = 50; // set to 100
                }
            else
            {
                // Database error!
                string msg = string.Format("Processing stopped!\nError reading parameter: 913 Occur: 3 from parameters table!\nOperator is: '{0}'", argOperator);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            #endregion

            if (glbRawImport)
            {
                #region Get name of Stored procedure for Raw Import

                try
                {
                    glbStoredProc = "";
                    Gp.ReadParametersSpecificId(argOperator, "913", "2", "", "");
                    if (Gp.RecordFound)
                    {
                        glbStoredProc = Gp.OccuranceNm;
                        if (string.IsNullOrEmpty(glbStoredProc))
                        {
                            string msg = string.Format("Processing stopped!\nError while getting Stored Procedure Name for Raw Import of '{0}' from parameters table!\nOperator is: '{1}'\r\n The retrieve value was empty!}",
                                                        argSourceFileID, argOperator);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                            return;
                        }
                    }
                    else
                    {
                        string msg = string.Format("Processing stopped!\nError while getting Stored Procedure Name for Raw Import of '{0}' from parameters table!\nOperator is: '{1}'\nThe exception reads: {2}",
                                                    argSourceFileID, argOperator, Gp.ErrorOutput);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Processing stopped!\nException while getting Stored Procedure Name for Raw Import of '{0}' from parameters table!\nOperator is: '{1}'\nThe exception reads: {2}",
                                                argSourceFileID, argOperator, ex.Message);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return;
                }
                #endregion
            }

            #region Read the record from MatchingSourceFiles and check if Source and Archive dirs exist           
            // Read the record from from RRDMMatchingSourceFiles 
            rsf = new RRDMMatchingSourceFiles();
            rsf.ReadSourceFileRecordByOriginAndFileID(argOrigin, argSourceFileID);
            if (rsf.ErrorFound || !rsf.RecordFound)
            {
                string msg = string.Format("Processing stopped!\nError reading FileID: {0}  of  Origin: {1} from the [MatchingSourceFiles] table! \nThe received error message is: \n{2}", argSourceFileID, argOrigin, rsf.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            // Check if Source dir exist
            if (!Directory.Exists(rsf.SourceDirectory))
            {
                string msg = string.Format("Processing stopped!\nCould not locate the following directory:\n{0}", rsf.SourceDirectory);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }

            // Check if Archive dir exist
            if (!string.IsNullOrEmpty(rsf.ArchiveDirectory)) // if not present in GASParameters then no Archiving takes place
            {
                // Check if Archive dir exist
                if (!Directory.Exists(rsf.ArchiveDirectory))
                {
                    string msg = string.Format("Processing stopped!\nCould not locate the following directory:\n{0}", rsf.ArchiveDirectory);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return;
                }
                glbArchiveEnabled = true;
            }

            // Check if Exceptions dir exist; if not, create!
            if (!Directory.Exists(rsf.ExceptionsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(rsf.ExceptionsDirectory);
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Processing stopped!\nCould not create the following directory:\n{0}\r\nThe exception reads: {1}",
                                     rsf.ExceptionsDirectory, ex.Message);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return;
                }
            }
            #endregion

            // Signal back that this thread has started
            ((AutoResetEvent)hwStartedEvent).Set();


            #region Show signs that the program started and is waiting for source files
            if (Environment.UserInteractive)
            {
                Console.WriteLine("");
                Console.WriteLine(string.Format("  Ready to process files masked as  {0}  coming into  {1}", rsf.FileNameMask, rsf.SourceDirectory));
                Console.WriteLine(string.Format("  MaxPagingRows set to {0}", glbMaxPagingRows));
                Console.WriteLine("");
                Console.WriteLine("  Press [ CTRL + C ] to end this program....");
                Console.WriteLine("");
            }
            #endregion

            #region Loop in search of files until CTRL+C or program receives termination signal
            while (!SignalToStop)
            {
                string ATM_No = "";
                string eJournalType = "";
                string pattern = rsf.SourceFileId + "_[0-9]{8}\\.[0-9]{3}";

                string fileMask = rsf.SourceFileId + "_????????.???";
                string[] FileArray = Directory.GetFiles(rsf.SourceDirectory, fileMask);
                try
                {
                    if (FileArray.Length > 0)
                        Array.Sort(FileArray, StringComparer.InvariantCultureIgnoreCase);

                    foreach (string fullFileName in FileArray)
                    {
                        if (SignalToStop) { return; }

                        string fileName = Path.GetFileName(fullFileName);
                        // match using regular expression (fileMask)
                        if (Regex.IsMatch(fileName, pattern))
                        {

                            #region // Raw Import of Journals -- DEPRECATED
                            //string ATM_No = "";
                            //if (glbRawImport)
                            //{
                            //    // ToDo: parameterize extraction of ATM Number from file name..
                            //    ATM_No = fileName.Substring(0, 8);

                            //    // Get Journal Type from specific ATM record
                            //    RRDMAtmsClass aT = new RRDMAtmsClass();
                            //    aT.ReadAtm(ATM_No);
                            //    if (!aT.ErrorFound)
                            //    {
                            //        eJournalType = aT.EjournalTypeId;
                            //    }
                            //    else
                            //    {
                            //        eJournalType = "";

                            //        // Move file to Exceptions dir so it is not processed again...
                            //        string failedName = RFMFunctions.MoveFileToExceptionsDir(fullFileName);

                            //        string msg = string.Format("Stopped processing file: {0} from OriginSystem: {1}! \r\n" +
                            //            "The file has been renamed to: '{2}'\r\n" +
                            //            "Could not identify the Journal Type. Check the Windows Event Log for futher details!",
                            //            Path.GetFileNameWithoutExtension(fullFileName), argOrigin, failedName);
                            //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                            //        continue; // re-loop
                            //    }
                            //}
                            #endregion

                            ProcessFileResult retVal = InterceptSourceFile.InterceptFile(fullFileName, ATM_No, eJournalType, rsf.LinesInHeader, rsf.LinesInTrailer);

                        #region Finish up by writing an event in the Event Log
                        switch (retVal)
                        {
                            case ProcessFileResult.Success:
                                {
                                    string msg = string.Format("The file: {0} from OriginSystem: {1}  was processed successfully!", fileName, argOrigin);
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
                                    continue; // repeat the loop as long as there files to process
                                }
                            case ProcessFileResult.ReconciliationInProgress:
                                {
                                    // do not report this
                                    break;
                                }
                            case ProcessFileResult.IO_Error_SourceFleMove:
                            case ProcessFileResult.IO_Error_ArchiveFile:
                                {
                                    string msg = string.Format("Processing file: {0} from OriginSystem: {1}! was successfull " +
                                                                "but errors were encountered while moving to its final folder! " +
                                                                "Check the Windows Event Log or details",
                                                                fileName, argOrigin);
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                    break;
                                }
                            case ProcessFileResult.IsDuplicate:
                            case ProcessFileResult.NotExpected:
                                {
                                    // Already logged
                                    break;
                                }
                            case ProcessFileResult.ProcessingError:
                            case ProcessFileResult.IO_Error:
                            case ProcessFileResult.SQL_Error:
                            default:
                                {
                                    string msg = string.Format("Stopped processing file: {0} from OriginSystem: {1}! " +
                                                        "Errors were encountered! Check the Windows Event Log and/or the file {2}.Failed.Log for details",
                                                         fileName, argOrigin, Path.GetFileNameWithoutExtension(fileName));
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                    break;
                                }
                        }
                            #endregion

                        }
                    }

                    // IMPORTANT
                    //{
                    //    // ToDo: MatchingCategoriesVsSourceFiles, ProcessMode for the category
                    //}

                }
                catch (Exception ex)
                {
                    //Terminate
                    string msg = string.Format("Terminating Instance because of an exception!\n  Origin {0}\n  SourceFileID: {1}\n The exception message is:\n{2}",
                                  argOrigin, argSourceFileID, ex.Message);
                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                    return;
                }

                // No files to process, sleep (RfmSleep is in seconds, so loop as many times and sleep for one second each)
                int loopCount = glbRFMSleep; // 
                for (int i = 0; i < loopCount; i++)
                {
                    if (!SignalToStop)
                        Thread.Sleep(1000);
                    else
                        break;
                }
            } // end while
            #endregion
        }
    }
}

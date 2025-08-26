using System;
using System.IO;
using System.Threading;

using System.Diagnostics;
using System.Xml;
using System.Net.Mail;
using System.Xml.Serialization;
using RRDM4ATMs;

namespace RRDMRFM_Journal_Classes
{

    public class RfmjWorker
    {
        /// <summary>
        /// RmfjWorker class
        /// </summary>
        /// <remarks>
        /// The server creates a new Worker instance for each incoming file.
        /// RfmjWorkerThread() is the method that carries out the processing.
        /// Upon instantiation the constructor sets the value of the ThreadIndex variable
        /// which is the slot in the array allocated to the thread's instance
        /// (every thread instance has its own slot in the array)
        /// </remarks>

        // The index to the Thread Array slot allocated for the instance
        private int ThreadIndex;
        private string SourceFileName;
        private string SourceFileDir;
        // Event Handle to wait on for this thread to carry on after initializing
        // Instantiated per thread and signaled by RfmjServer
        public volatile EventWaitHandle hClearToGoEvent;


        #region RfmjWorker constructor
        public RfmjWorker(int Index, string fullFileName)
        {
            // Set the value of ThreadIndex
            ThreadIndex = Index;

            // Set values for filename and dir
            SourceFileName = Path.GetFileName(fullFileName);
            SourceFileDir = Path.GetDirectoryName(fullFileName);

            // Create (nonsignaled) the handle to wait on. Will be set by the thread creator (RfmjServer)
            hClearToGoEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        #endregion

        #region RfmjWorkerThread
        public void RfmjWorkerThread(object hwStartedEvent)
        {
            Thread oT = Thread.CurrentThread;
            bool AbortError = false;

            #region Update Status and Stage in ThreadArray and then signal back that we started
            string ATM_No = SourceFileName.Substring(0, 8);

            // Set oThread, ATM_No and Status fields of the corresponding ThreadArray slot
            RfmjThreadRegistry.SetAtmNumber(ThreadIndex, ATM_No);
            RfmjThreadRegistry.SetThreadObjHandle(ThreadIndex, oT);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.SourceFileName = SourceFileName;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.SourceFileDir = SourceFileDir;

            // Set this before we signal back to the server thread
            RfmjThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Running);

            // Signal back that this thread has started
            ((AutoResetEvent)hwStartedEvent).Set();

            // Wait for EBZServer to give us the go ahead...
            bool ClearToGo = hClearToGoEvent.WaitOne();

            if (RfmjServer.Abort_Abort == true)
            {
                // Abort_Abort is raised! do not continue!!
                RfmjThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Canceled);
                return;
            }
            #endregion

            #region Do the actual work
            int stg = RfmjActionStage.Const_Step_1_InProgress;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_1_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_1_Start = DateTime.Now;

            string eJournalType = "";

            string srcFileName = SourceFileName;
            string srcFileDir = SourceFileDir;
            string fullFileName = Path.Combine(srcFileDir, srcFileName);

            #region Get Journal Type from specific ATM record
            RRDMAtmsClass aT = new RRDMAtmsClass();
            aT.ReadAtm(ATM_No);
            if (aT.RecordFound)
            {
                eJournalType = aT.EjournalTypeId;
            }
            else
            {
                // ATM not found... Create!!!
                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.CreateNewAtmBasedOnGeneral_Model(RfmjServer.argOperator, ATM_No, srcFileName);
                if (Ac.ErrorFound == true)
                {

                    eJournalType = "";

                    // Move file to Exceptions dir so it is not processed again...

                    string failedName = RFMJFunctions.MoveFileToExceptionsDir(fullFileName);

                    string msg = string.Format("Stopped processing file: {0} from OriginSystem: {1}! \r\n" +
                                                "The file has been renamed to: '{2}'\r\n" +
                                                "Could not identify the Journal Type. Check the Windows Event Log for futher details!",
                                                Path.GetFileNameWithoutExtension(fullFileName), RfmjServer.argOrigin, failedName);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                }
                else
                {
                    RRDMAtmsClass aT1 = new RRDMAtmsClass();
                    aT1.ReadAtm(ATM_No);
                    if (aT1.RecordFound)
                    {
                        eJournalType = aT1.EjournalTypeId;
                    }
                    // eJournalType = "Wincor_01";
                }
            }
            #endregion

            #region InterceptFile
            InterceptJournalFile iJF = new InterceptJournalFile(ThreadIndex);

            ProcessFileResult retVal = iJF.InterceptFile(fullFileName, ATM_No, eJournalType);

            switch (retVal)
            {
                case ProcessFileResult.SQL_Exception:
                    {
                        string msg = string.Format("Thread Aborting beacause of an SQL Error! File: [{0}]!", fullFileName);
                        msg = msg + "\r\nExamine earlier entries in the Log to establish the reason.";
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        AbortError = true;
                        break;
                    }
                case ProcessFileResult.JLN_IO_Error:
                    {
                        string msg = string.Format("Thread Aborting beacause of an IO Error. File: [{0}]!", fullFileName);
                        msg = msg + "\r\nExamine earlier entries in the Log to establish the reason.";
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        AbortError = true;
                        break;
                    }
                case ProcessFileResult.IO_Error_ArchiveFile:
                case ProcessFileResult.IO_Error_SourceFileDelete:
                    {
                        string msg = string.Format("Thread Aborting beacause of an IO exception! File: [{0}]!", fullFileName);
                        msg = msg + "\r\nExamine earlier entries in the Log to establish the reason.";
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        AbortError = true;
                        break;
                    }
                case ProcessFileResult.ATMJournalSkipped:
                    {
                        // Force cycle to repeat without waiting for the timer to expire
                        RfmjServer.ResetCycle = true;
                        break;
                    }
                case ProcessFileResult.File_Empty:
                    {
                        string msg = string.Format("Thread Aborting beacause the file is empty!\r\nFile: [{0}]!", fullFileName);
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        break;
                    }
                case ProcessFileResult.InvalidFileDateFormat:
                case ProcessFileResult.ReconciliationInProgress:
                case ProcessFileResult.ReconciliationCycleError:
                case ProcessFileResult.IsDuplicate:
                case ProcessFileResult.BeingProcessedByAnotherThread:
                case ProcessFileResult.SP_Error:
                    {
                        // Do nothing... Thread will finish normally
                        // (Errors already logged in Event Log)
                        break;
                    }
            }
            #endregion

            #region Update the Thread Status/Step
            stg = RfmjActionStage.Const_Step_1_Finished;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_1_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_1_End = DateTime.Now;
            #endregion

            #region Cleanup
            // Release our slot...
            RfmjThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Finished);

            // Signal to stop if IO Error..
            RfmjServer.Abort_Abort = AbortError;
            #endregion
            
            #endregion
        }
        #endregion
    }

}

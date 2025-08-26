using System;
using System.IO;
using System.Threading;

using System.Diagnostics;
using System.ComponentModel;

using RRDM4ATMs;

namespace RRDMJTMClasses
{

    /// <summary> JTMWorker
    /// </summary>
    public class JTMWorker
    {
        #region JTMWorker
        /// <summary>
        /// JTMWorker class
        /// </summary>
        /// <remarks>
        /// The server creates a new JTMWorker instance for each incoming request.
        /// JTMWorkerThread() is the method that carries out the request.
        /// Upon instantiation the constructor sets the value of the ThreadIndex variable
        /// which is the slot in the array allocated to the thread's instance
        /// (every thread instance has its own slot in the array)
        /// </remarks>

        // The index to the Thread Array slot allocated for the instance
        private int ThreadIndex;

        // Event Handle to wait on for this thread to carry on after initializing
        // Instantiated per thread and signaled by JTMServer
        public volatile EventWaitHandle hClearToGoEvent;

        /// <summary>
        /// JTMWorker
        /// </summary>
        /// <remarks>
        /// JTMWorker constructor
        /// </remarks>
        /// <param name="Index"></param>
        public JTMWorker(int Index)
        {
            // Set the value of ThreadIndex
            ThreadIndex = Index;
            // Create (nonsignaled) the handle to wait on. Will be set by the thread creator (JTMServer)
            hClearToGoEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        #region JTMWorkerThread
        public void JTMWorkerThread(object hwStartedEvent)
        {
            string msg;
            // bool success = false;
            bool UpdSuccess = false;

            Thread oT = Thread.CurrentThread;

            #region Update Status in ThreadArray and Stage in SQL Rec, then signal back that we started

            // Set oThread and Status fields of the corresponding ThreadArray slot
            JTMThreadRegistry.SetThreadObjHandle(ThreadIndex, oT);
            JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Running);

            // Update the Stage field of the record in SQL (so it will not be re-read)
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_WorkInProgress;
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ProcessStart = DateTime.Now;
            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
            if (UpdSuccess != true)
            {   // This piece of code is unreachable...
                // UpdateRecordInSQL will have thrown the JTMCustomException
                return;
            }

            JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Running);

            // Signal back that this thread has started
            ((AutoResetEvent)hwStartedEvent).Set();

            // Wait for JTMServer to give us the go ahead...
            bool ClearToGo = hClearToGoEvent.WaitOne();

            if (JTMThreadRegistry.Abort_Abort == true)
            {
                // Abort_Abort is raised! do not continue!!
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Canceled);
                return;
            }

            msg = string.Format("Processing Request --> MsgID: [{0}] Command: [{1}] ATM No: [{2}] MachineName: [{3}] Requestor: [{4}]",
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.RequestorID);
            EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Information);
            #endregion

            #region Do the actual work
            #region Try
            try
            {
                switch (JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command)
                {
                    case JTMQueueCommand.Cmd_ATMSTATUS:
                        {
                            bool rc = false;
                            rc = CheckATMStatus();
                            break;
                        }
                    case JTMQueueCommand.Cmd_FETCH:
                        // case JTMQueueCommand.Cmd_FETCHDEL:
                        {
                            bool rc = false;
                            rc = ProcessJournalRequest();
                            break;
                        }
                    default:
                        {
                            #region Invalid Command
                            msg = string.Format("Invalid COMMAND [{0}] in request with MsgID:{1} ",
                                                 JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command,
                                                 JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID);
                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                            JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);

                            EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                            // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                            #endregion
                            break;
                        }
                }

                // Release our slot...
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Finished);
            }
            #endregion

            #region Catch

            catch (ThreadAbortException ex)
            {
                msg = string.Format("Source: [{0}] --> The worker thread servicing MsgID [{1}] received signal to rerminate!",
                                     ex.Source, JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Aborting);
            }
            catch (JTMCustomException ex)
            {
                msg = string.Format("Source: [{0}] --> The worker thread servicing MsgID [{1}] encountered a problem and cannot continue! The error reads:\n{2}",
                                     ex.Source, JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID, ex.ToString());
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Aborting);

                if (ex.JTMFatal)
                {
                    msg = "Program is terminating!!";
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    JTMThreadRegistry.RaiseAbortFlag();
                }
            }
            catch (Exception ex)
            {
                msg = string.Format("Source: [{0}] --> The worker thread servicing MsgID [{1}] encountered a problem and cannot continue! The error reads:\n{2}",
                                     ex.Source, JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID, ex.ToString());
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                msg = "Program is terminating!!";
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Aborting);

                JTMThreadRegistry.RaiseAbortFlag();
            }
            #endregion
            #endregion
        }
        #endregion
        #endregion

        #region Functions
        /* ----- Functions -------------------------------------*/
        /* ==================================================== */

        #region CheckATMStatus()
        /// <summary>
        /// CheckATMStatus()
        /// </summary>
        /// <remarks>
        /// Checks if we can connect to the remote machine
        /// </remarks>
        /// <returns>bool</returns>
        public bool CheckATMStatus()
        {
            bool success = false;
            bool UpdSuccess = false;
            // string msg;
            int i = 0;

            string ContextUser = string.Format("{0}\\{1}", Environment.UserDomainName.Trim(), Environment.UserName.Trim());

            // TODO Exceptions will propagate upstream... (e.g. SQLUpdate, ThreadAbort,....) and will be treated as fatal
            do // with retries...
            {
                success = CheckConnection(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID,
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword,
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth);
                if (success) break;
                Thread.Sleep(JTMThreadRegistry.JTM_FETCH_RetryWaitTime);
                i++;
            } while (i < JTMThreadRegistry.JTM_FETCH_Retries && JTMThreadRegistry.Abort_Abort != true);


            if (success == true)
            {
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ProcessEnd = DateTime.Now;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_Finished;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Success;
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
            }
            else
            {
                // Failure to connect
                success = false;
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_Finished;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = string.Format("Failure to connect to {0} using UserID:{1}",
                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth ? ContextUser : JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID);
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
            }
            return (success);
        }
        #endregion

        #region CheckConnection
        /// <summary>  
        /// CheckConnection
        /// </summary>
        /// <param name="RemoteMachine"></param>
        /// <param name="UserName">
        /// format: Domain | Machine Name \UserName</param>
        /// <param name="Password"></param>
        /// <param name="WinAuth"></param>
        /// <returns>
        /// true if successfull
        /// false if not
        /// </returns>
        public static bool CheckConnection(string ATMNo, string RemoteMachine, string UserName, string Password, bool WinAuth)
        {
            bool ret = false;
            string msg;
            string UserID = UserName;
            string pwd = Password;

            if (JTMThreadRegistry.Abort_Abort == true)
                return (false);
            try
            {
                if (WinAuth == true) // Windows Authentication
                {
                    UserID = null;
                    pwd = null;
                }
                using (JTMRemote.EstablishConnection(RemoteMachine, UserID, pwd))
                {
                    ret = true;
                }
            }
            catch (Win32Exception ex)
            {
                ret = false;
                msg = string.Format("An error occured in establishing a connection to remote:{0} using UserName:{1}!" +
                                    "\nThe exception reads: Source:{2} ErrorCode:{3} ErrorMessage:{4}",
                                    RemoteMachine, ((UserID == null) ? "none" : UserName), ex.Source, ex.NativeErrorCode.ToString(), ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (ret);
        }
        #endregion

        #region ProcessJournalRequest()
        /// <summary>
        /// ProcessJournalRequest()
        /// </summary>
        /// <returns>bool</returns>
        public bool ProcessJournalRequest()
        {
            string msg;
            bool success = false;
            bool UpdSuccess = false;
            string Cmd = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command;
            // bool BackupFlag = ((Cmd == JTMQueueCommand.Cmd_FETCHDEL) ? true : false);
            bool BackupFlag = false;

            // Check for Abort_Abort at each step
            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region Fetch the Journal file
            // Fetch the Journal file
            if (JTMThreadRegistry.JTM_MaxJournalBackups > 0)
                BackupFlag = true;
            success = this.ProcessJournalFetch(BackupFlag);
            if (!success) return (false);
            #endregion

            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region Execute The SQL SP
            // Execute The SQL SP
            // Impose serialization of access here ...
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_WaitingForParsing;
            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
            // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException

            lock (JTMThreadRegistry.SQLLockSP)
            {
                if (JTMThreadRegistry.Abort_Abort != true) //  Once started we should not interrupt it
                {
                    success = this.InvokeSQLSP();
                }
                else
                {
                    success = false;
                    msg = string.Format("Skiped parsing stage (InvokeSQLSP) for MsgID:{0} - ATM:{1} becasue system was being terminated!", JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID, JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo);
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                }
            }
            if (!success) return (false);
            #endregion

            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region Move received eJournal file to Archive
            // move the received file in the Archive
            success = this.MoveToArchive();
            if (!success)
            {
                string FileThatFailedToMove = string.Format("{0}\\{1}",
                                   JTMThreadRegistry.JTM_FilePoolRoot,
                                   JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
                                   );
                msg = string.Format("Failed to move file {0} to the archive after successfull processing!", FileThatFailedToMove);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }

            #endregion

            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region CloseUp

            // Update Identification Details. This is done only once, here!!!
            DateTime dt = DateTime.Now;
            // JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.DateLastUpdated = dt;
            // JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.LoadingCompleted = dt;
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.QueueRecId = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID; // update
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileUploadRequestDt = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgDateTime; // update
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileParseEnd = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileParseEnd; // update value
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultCode = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode; // update
            if (JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode == 0)
            {
                JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultMessage = "Journal File Loaded and Parsed!";
            }
            else
            {
                JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultMessage = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage; // update
            }
            success = this.UpdateIndentificationDetailsRecord();

            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ProcessEnd = dt;
            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
            // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException

            #endregion

            return true;
        }
        #endregion

        #region ProcessJournalFetch()
        /// <summary>
        /// FetchJournalFromATM
        /// </summary>
        /// <returns>bool</returns>
        public bool ProcessJournalFetch(bool BackupFlag)
        {
            // string msg;
            bool success = false;
            bool UpdSuccess = false;
            string SrcFilePath;
            string DestFileHASH;
            string DestFullFileName;

            // ContextUser is for reporting purposes only
            string ContextUser = string.Format("{0}\\{1}", Environment.UserDomainName.Trim(), Environment.UserName.Trim());

            // Do not proceed further if in Aborting phase...
            if (JTMThreadRegistry.Abort_Abort == true)
                return (false);

            #region Transfer Journal from ATM
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_TransferInProgress;
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileUploadStart = DateTime.Now;
            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
            // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException

            SrcFilePath = @"\\" + JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName + @"\"
                                + JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFilePath;
            // Get File from ATM
            DestFullFileName = this.FETCHJournalFile(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName.Trim(),
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID.Trim(),
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword.Trim(),
                                          SrcFilePath,
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFileName.Trim(),
                                          // JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFilePath.Trim(),
                                          JTMThreadRegistry.JTM_FilePoolRoot, // ignore 'destfilepath' passed in the request (queue); use the globally defined one.
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command.Trim(),
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth,
                                          BackupFlag
                                         );

            if (DestFullFileName != null)
            {
                success = true;
                DestFileHASH = FileHASH.BytesToString(FileHASH.GetHashSha256(DestFullFileName));

                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName = Path.GetFileName(DestFullFileName);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileHASH = DestFileHASH;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileUploadEnd = DateTime.Now;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_TransferFinished;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Success;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = "Journal Log Transfered with success";
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException

                #region Trim file if Diebold
                switch (JTMThreadRegistry.ThreadArray[ThreadIndex].Req.TypeOfJournal)
                {
                    case "DBLD01":
                        {
                            int lineCount = -1;
                            lineCount = TrimDieboldFile(DestFullFileName);
                            if (lineCount > 1) // File must have at least two lines
                            {
                                success = true;
                                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Success;
                                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = "Journal Log Transfered and valid data extracted with success";
                                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                            }
                            else
                            {
                                success = false;
                                string msg = string.Format("Journal Log Transfered with success but JTM failed to extract valid data from Diebold eJournal file! ATMNo:[{0}], MachineName:[{1}] File:[{2}]",
                                                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                                                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                                                            Path.GetFileName(DestFullFileName));
                                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                            }
                            break;
                        }
                    default:
                        {
                            // Currently only Diebold files are trimmed....
                            break;
                        }
                }
                #endregion

            }
            else
            {
                // Failure to fetch file
                success = false;
                string msg = string.Format("Failure to transfer the file {0}\\{1} from {2}",
                                            SrcFilePath,
                                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFileName,
                                            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
            }
            #endregion
            return (success);
        }
        #endregion

        #region FETCHJournalFile
        /// <summary>  
        /// FETCHJournalFile
        /// </summary>
        /// <param name="RemoteMachine"></param>
        /// <param name="UserName">
        /// format: Domain | Machine Name \UserName</param>
        /// <param name="Password"></param>
        /// <param name="SrcPath">
        /// format: Share\dir...</param>
        /// <param name="SrcFile"></param>
        /// <param name="DstPath"></param>
        /// <param name="Cmd"></param>
        /// <param name="WinAuth"></param>
        /// <param name="DoBackup"></param>
        /// <returns>
        /// The fully qualified destination file name if successfull
        /// null if not</returns>
        /// </returns>
        public string FETCHJournalFile(string ATMNo, string RemoteMachine, string UserName, string Password, string SrcPath, string SrcFile, string DstPath, string Cmd, bool WinAuth, bool DoBackup)
        {
            // string msg;
            string ReceivedFullFileName;
            bool OvrWr = true;
            string DstFile;
            string srcFullPath;
            string dstFullPath;

            DateTime dt = DateTime.Now;
            //DstFile = string.Format("{0}-{1}-{2}{3}",
            //                         ATMNo.Trim(),
            //                         Path.GetFileNameWithoutExtension(SrcFile.Trim()),
            //                         dt.ToString("yyyyMMdd-HHmmss.ffffff"),
            //                         Path.GetExtension(SrcFile.Trim())
            //                       );
            DstFile = string.Format("{0}-{1}{2}",
                                     ATMNo.Trim(),
                                     dt.ToString("yyyyMMdd-HHmmss.fff"),
                                     Path.GetExtension(SrcFile.Trim())
                                   );
            srcFullPath = Path.Combine(SrcPath, SrcFile);
            dstFullPath = Path.Combine(DstPath, DstFile);

            if (JTMThreadRegistry.Abort_Abort == true)
                return (null);


            ReceivedFullFileName = GetJournalFileWithRetries(srcFullPath, dstFullPath, OvrWr, DoBackup);


            //if (WinAuth == true)
            //{
            //    ReceivedFullFileName = GetJournalFileWithRetries(srcFullPath, dstFullPath, OvrWr, DoBackup);
            //}
            //else
            //{
            //try
            //{
            //    using (JTMRemote.EstablishConnection(RemoteMachine, UserName, Password))
            //    {
            //        ReceivedFullFileName = GetJournalFileWithRetries(srcFullPath, dstFullPath, OvrWr, DoBackup);
            //    }
            //}
            //catch (Win32Exception ex)
            //{
            //    ReceivedFullFileName = null;
            //    msg = string.Format("Error in JTMWorker:FETCHJournalFile while connecting to [{0}] using UserName:{1}!" +
            //                        "\nThe error reads: Source:{2} ErrorCode:{3} ErrorMessage:{4}",
            //                         RemoteMachine, UserName, ex.Source, ex.NativeErrorCode.ToString(), ex.Message);
            //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            //}
            ////catch (Exception ex)
            //// Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

            //}
            return (ReceivedFullFileName);
        }
        #endregion

        #region GetJournalFileWithRetries
        /// <summary>
        /// GetJournalFileWithRetries
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <param name="dstFullPath"></param>
        /// <param name="OvrWr"></param>
        /// <returns>string</returns>
        public string GetJournalFileWithRetries(string srcFullPath, string dstFullPath, bool OvrWr, bool DoBackup)
        {
            string CopiedFullFileName = null;

            if (JTMThreadRegistry.Abort_Abort == true)
                return (null);

            int i = JTMThreadRegistry.JTM_FETCH_Retries;
            do
            {
                CopiedFullFileName = this.GetJournalFile(srcFullPath, dstFullPath, OvrWr, DoBackup);
                if (CopiedFullFileName != null) break;
                Thread.Sleep(JTMThreadRegistry.JTM_FETCH_RetryWaitTime);
                i--;
            } while (i > 0);
            if (CopiedFullFileName == null)
            {
                string msg = string.Format("Failed to FETCH file {0} after {1} attemps!", srcFullPath, JTMThreadRegistry.JTM_FETCH_Retries);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (CopiedFullFileName);
        }
        #endregion

        #region GetJournalFile
        /// <summary>GetJournalFile
        /// Copies the ATM Journal file to the destination
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="dstFullPath"></param>
        /// <param name="OvrWr"></param>
        /// <returns>The fully qualified destination file name if successfull, null if not</returns>
        public string GetJournalFile(string srcPath, string dstFullPath, bool OvrWr, bool DoBackup)
        {
            // Case with InitEJ - deprecated
            ///*
            //1. Check connectivity
            //2. Execute PSEXEC to invoke InitEJ
            //3. Chain Rename
            //4. Transfer
            //*/

            // Case without InitEJ
            /*
            1. Check connectivity
            2. Use PSExec to Copy active journal file to EJRCPY.LOG
            3. Chain Rename
            4. Transfer
            */

            DateTime dtStart1;
            DateTime dtFinish1;
            DateTime dtStart2;
            DateTime dtFinish2;
            DateTime dtStart3;
            DateTime dtFinish3;
            string srcFullPath = srcPath.Replace(":", "$");
            // int PSExecReturnCode = -1;
            bool success;
            string msg;
            string RenamedSource = null;
            string pathEJRCPY = Path.Combine(Path.GetDirectoryName(srcFullPath), "EJRCPY.LOG");
            string DestFullFileName = dstFullPath;


            int MaxJBck = JTMThreadRegistry.JTM_MaxJournalBackups;

            if (JTMThreadRegistry.Abort_Abort == true)
                return (null);


            // 1. Check connectivity
            success = CheckConnection(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth);
            if (!success)
            {
                // Failure to connect
                return null;
            }

            // Case with InitEJ
            // Uncomment this and comment out '2. try/catch' ...below.
            //// 2.Execute PSEXEC to invoke InitEJ
            //success = CallInitEJ(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
            //         JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID,
            //         JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword,
            //         JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth);
            //if (!success)
            //    return null;

            // 2.Execute PSEXEC to invoke "cmd /c copy..."
            dtStart1 = DateTime.Now;
            string ATM_LocalSrc = Path.Combine(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFilePath, JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFileName);
            string ATM_LocalDst = Path.Combine(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.SourceFilePath, "EJRCPY.LOG");
            success = CallCmdToCopy(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword,
                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth,
                     ATM_LocalSrc, ATM_LocalDst);
            dtFinish1 = DateTime.Now;
            if (!success)
                return null;

            try
            {
                string lpUserID = null;
                string lpPassword = null;

                if (!JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMWindowsAuth)
                {
                    lpUserID = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID;
                    lpPassword = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessPassword;
                }

                using (JTMRemote.EstablishConnection(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName, lpUserID, lpPassword))
                {

                    //// 2. Copy active journal file to EJRCPY.LOG
                    //try
                    //{
                    //    dtStart1 = DateTime.Now;
                    //    pathEJRCPY = Path.Combine(Path.GetDirectoryName(srcFullPath), "EJRCPY.LOG");
                    //    File.Copy(srcFullPath, pathEJRCPY, OvrWr);
                    //    dtFinish1 = DateTime.Now;

                    //}
                    //catch (UnauthorizedAccessException ex)
                    //{
                    //    DestFullFileName = null;
                    //    msg = string.Format("Access permission error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, pathEJRCPY, ex.Message);
                    //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    //    return (DestFullFileName);
                    //}
                    //catch (ArgumentException ex)
                    //{
                    //    DestFullFileName = null;
                    //    msg = string.Format("Invalid filename error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, pathEJRCPY, ex.Message);
                    //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    //    return (DestFullFileName);
                    //}
                    //catch (NotSupportedException ex)
                    //{
                    //    DestFullFileName = null;
                    //    msg = string.Format("Invalid filename format error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, pathEJRCPY, ex.Message);
                    //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    //    return (DestFullFileName);
                    //}
                    //catch (IOException ex)
                    //{
                    //    DestFullFileName = null;
                    //    msg = string.Format("I/O error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, pathEJRCPY, ex.Message);
                    //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    //    return (DestFullFileName);
                    //}
                    //// catch (Exception ex)
                    //// Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...


                    // 3. Chain Rename
                    if (DoBackup)
                    {
                        // Case of InitEJ
                        //RenamedSource = ChainRename(srcFullPath, MaxJBck);
                        dtStart2 = DateTime.Now;
                        RenamedSource = ChainRename(pathEJRCPY, MaxJBck);
                        dtFinish2 = DateTime.Now;
                    }


                    // 4.Transfer
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(RenamedSource))
                        {
                            dtStart3 = DateTime.Now;
                            File.Copy(RenamedSource, DestFullFileName, OvrWr);
                            dtFinish3 = DateTime.Now;
                        }
                        else
                        {
                            // Case of InitEJ
                            // File.Copy(srcFullPath, DestFullFileName, OvrWr);
                            return (null);
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        DestFullFileName = null;
                        msg = string.Format("Access permission error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, dstFullPath, ex.Message);
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    }
                    catch (ArgumentException ex)
                    {
                        DestFullFileName = null;
                        msg = string.Format("Invalid filename error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, dstFullPath, ex.Message);
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    }
                    catch (NotSupportedException ex)
                    {
                        DestFullFileName = null;
                        msg = string.Format("Invalid filename format error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, dstFullPath, ex.Message);
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    }
                    catch (IOException ex)
                    {
                        DestFullFileName = null;
                        msg = string.Format("I/O error while copying file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, dstFullPath, ex.Message);
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    }
                    // catch (Exception ex)
                    // Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

                }
            }
            catch (Win32Exception ex)
            {
                DestFullFileName = null;
                msg = string.Format("Error in JTMWorker:GetJournalFile while connecting to [{0}] using UserName:{1}!" +
                                    "\nThe error reads: Source:{2} ErrorCode:{3} ErrorMessage:{4}",
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                                     JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMAccessID,
                                     ex.Source, ex.NativeErrorCode.ToString(), ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            // Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

            return (DestFullFileName);
        }
        #endregion

        #region InvokeSQLSP
        /// <summary>
        /// Execute SQL Stored Procedure
        /// </summary>
        /// <returns>
        /// bool
        /// </returns>
        public bool InvokeSQLSP()
        {
            // Execute the SP that will Parse the file and Import in SQL
            int ret;
            string msg;
            bool IsSuccessful = false;
            bool UpdSuccess = false;
            string FullName;
            long FileSize = 0;

            string ContextUser = string.Format("{0}\\{1}", Environment.UserDomainName.Trim(), Environment.UserName.Trim());


            FullName = System.IO.Path.Combine(JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFilePath,
                                              JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName);

            try
            {
                FileInfo f = new FileInfo(FullName);
                FileSize = f.Length;
            }
            catch (IOException ex)
            {
                // Input file problem 
                IsSuccessful = false;
                msg = string.Format("Skipped execution of the Stored Procedure on SQL because of input file error!.\nThe file is {0}.\nThe error message reads:\n{1} ",
                                     FullName, ex.Message);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Aborting);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                return (false);
            }

            string ProcedureName = JTMThreadRegistry.JTM_ParserSP;
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_ParserInProgress;
            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileParseStart = DateTime.Now;
            UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
            // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException

            if (FileSize == 0)
            {
                // File has zero length ... return
                IsSuccessful = false;
                msg = string.Format("Skipped execution of the Stored Procedure on SQL because of empty input file!.\nThe file is: {0}", FullName);
                JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                return (false);
            }
            else
            {
                RRDMJTMQueue JQ1 = new RRDMJTMQueue();
                string InputFileName = null;

                if (string.IsNullOrEmpty(JTMThreadRegistry.JTM_SQLRelativeFilePoolPath))
                {
                    InputFileName = FullName;
                }
                else
                {
                    InputFileName = Path.Combine(JTMThreadRegistry.JTM_SQLRelativeFilePoolPath, Path.GetFileName(FullName));
                }

                //ret = JQ1.InvokeStoredProcedure(
                //                        ProcedureName,
                //                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.BankID,
                //                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                //                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.BranchNo,
                //                        InputFileName);
                ret = JQ1.InvokeStoredProcedure(
                                        ProcedureName,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.TypeOfJournal,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.BankID,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                                        InputFileName);

                if (!JQ1.ErrorFound)
                {
                    if (ret == 0)
                    {
                        IsSuccessful = true;
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_ParserFinished;
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileParseEnd = DateTime.Now;
                        UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                    }
                    else
                    {
                        IsSuccessful = false;
                        msg = string.Format("The Stored Procedure returned an error! Input file is:{0}\n. RerurnCode: {1}",
                                             InputFileName, ret);
                        JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                        UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                        // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                    }
                }
                else
                {
                    // Failure of SP
                    IsSuccessful = false;
                    msg = string.Format("Failure in executing the Stored Procedure on SQL! Input fle is:{0}. The error message reads: {1}",
                                         InputFileName, JQ1.ErrorOutput);
                    JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                    // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                }
            }
            return (IsSuccessful);
        }
        #endregion

        #region UpdateJTMQueueRecordInSQL
        /// <summary>
        /// Updates record in SQL using struct
        /// </summary>
        /// <param name="QRec"></param>
        /// <returns>bool</returns>
        public bool UpdateJTMQueueRecordInSQL(JTMQueueStruct QRec)
        {
            bool result = false;

            RRDMJTMQueue JQ = new RRDMJTMQueue();

            JQ.UpdateRecordInJTMQueueUsingStruct(QRec);
            if (JQ.ErrorFound != true)
            {
                result = true;
            }
            else
            {
                result = false;
                string msg = string.Format("A fatal error occured in JTMWorker.UpdateRecordInJTMQueueUsingStruct()! The program will terminate...");
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                // The caller must handle the exception and signal for termination....
                JTMCustomException ex = new JTMCustomException(JQ.ErrorOutput);
                ex.JTMCode = JTMQueueResult.Failure;
                ex.JTMMessage = msg;
                ex.JTMSource = "UpdateJTMQueueRecordInSQL";
                ex.JTMFatal = true;
                throw (ex);
            }

            return (result);
        }
        #endregion

        #region Update record in JTMIdentificationDetails table
        /// <summary>
        /// UpdateIndentificationDetailsRecord
        /// </summary>
        /// <returns>bool</returns>
        public bool UpdateIndentificationDetailsRecord()
        {
            // The record is kept in the ThreadArray element pointed to by this thread's ThreadIndex private member
            string msg;
            bool success = false;

            RRDMJTMIdentificationDetailsClass JdUpd = new RRDMJTMIdentificationDetailsClass();

            JdUpd.SeqNo = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SeqNo;
            JdUpd.QueueRecId = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.QueueRecId;
            JdUpd.AtmNo = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.AtmNo;
            JdUpd.DateLastUpdated = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.DateLastUpdated;
            JdUpd.UserId = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.UserId;
            JdUpd.LoadingScheduleID = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.LoadingScheduleID;
            JdUpd.ATMIPAddress = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ATMIPAddress;
            JdUpd.ATMMachineName = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ATMMachineName;
            JdUpd.ATMWindowsAuth = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ATMWindowsAuth;
            JdUpd.ATMAccessID = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ATMAccessID;
            JdUpd.ATMAccessPassword = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ATMAccessPassword;
            JdUpd.TypeOfJournal = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.TypeOfJournal;
            JdUpd.SourceFileName = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SourceFileName;
            JdUpd.SourceFilePath = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SourceFilePath;
            JdUpd.DestnFilePath = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.DestnFilePath;
            JdUpd.FileUploadRequestDt = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileUploadRequestDt;
            JdUpd.FileParseEnd = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileParseEnd;
            JdUpd.LoadingCompleted = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.LoadingCompleted;
            JdUpd.ResultCode = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultCode;
            JdUpd.ResultMessage = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultMessage;
            JdUpd.Operator = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.Operator;
            // JdUpd.RecordFound = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.RecordFound;
            // JdUpd.ErrorFound = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ErrorFound;
            // JdUpd.ErrorOutput = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ErrorOutput;
            JdUpd.SWVersion = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SWVersion;
            JdUpd.SWDate = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SWDate;
            JdUpd.SWDCategory = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.SWDCategory;
            JdUpd.TypeOfSWD = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.TypeOfSWD;

            JdUpd.UpdateRecordInJTMIdentificationDetailsByID(JdUpd.SeqNo);
            if (!JdUpd.ErrorFound)
            {
                success = true;
            }
            else
            {
                success = false;
                msg = string.Format("Error in updating JTMIdentificationDetails for ATM:{0}. The error message reads: \n{1} ",
                                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                                    JdUpd.ErrorOutput);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (success);
        }
        #endregion

        #region ChainRename
        /// <summary>
        /// BackupAndRecreate
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <param name="MaxBckFileCount"></param>
        /// <returns>New name of original file (EJRCPY.LOG.001.BAK)</returns>
        public string ChainRename(string srcFullPath, int MaxBckFileCount)
        {
            string NewFileName = null;
            string msg;
            string srcFile;         //  "EJRCPY.LOG";
            string srcDir;          //  @"C:\Program Files\Advance NDC\Data";
            string FileSearchMask;  //  "EJRCPY.LOG.???.BAK";
            string FileBackupMask;  //  "EJRCPY.LOG.{0}.BAK";

            string[] fArray;
            int FileCount = 0;
            string fileName;
            string prev;
            string next;
            string SrcLogFile;
            string DstBakFile;
            int LastIndx = 0;
            int p = 0;

            srcFile = Path.GetFileName(srcFullPath);
            srcDir = Path.GetDirectoryName(srcFullPath);
            FileSearchMask = string.Format("{0}.???.BAK", srcFile);
            FileBackupMask = string.Format("{0}.{1}.BAK", srcFile, "{0}");

            try
            {
                fArray = Directory.GetFiles(srcDir, FileSearchMask);
                FileCount = fArray.Length;

                #region Delete the files over the MaxBckFileCount 
                if (MaxBckFileCount == 0) // No backup is to be taken... delete any leftovers
                {
                    for (int i = 0; i < FileCount; i++)
                    {
                        fileName = fArray[i];
                        // Console.WriteLine("Delete: {0}", fileName);
                        File.Delete(fileName);
                    }
                    NewFileName = srcFullPath;
                    return (NewFileName);
                }

                // This will handle the case where the JTM parameter (JTM_MaxJournalBackups) is reduced 
                // It also deletes the MaxBckFileCount'th file to make room for the renaming
                if (MaxBckFileCount <= FileCount)
                {
                    for (int i = FileCount; i >= MaxBckFileCount; i--)
                    {
                        fileName = fArray[i - 1];
                        File.Delete(fileName);
                    }
                    LastIndx = MaxBckFileCount - 1;
                }
                else
                {
                    LastIndx = FileCount;
                }
                #endregion

                #region Rename older files
                if (LastIndx > 0)
                {
                    // Rename older files in reverse sequence (eg: 3->4, 2->3, 1->2)
                    for (p = LastIndx; p > 0; p--)
                    {
                        prev = Path.Combine(srcDir, (string.Format(FileBackupMask, (p).ToString("000"))));
                        next = Path.Combine(srcDir, (string.Format(FileBackupMask, (p + 1).ToString("000"))));
                        File.Move(prev, next);
                    }
                }
                #endregion

                #region Rename the actual file to 001
                // Rename the actual file to 001
                SrcLogFile = Path.Combine(srcDir, srcFile);
                DstBakFile = Path.Combine(srcDir, (string.Format(FileBackupMask, "001")));
                File.Move(SrcLogFile, DstBakFile);
                NewFileName = DstBakFile; // NewFileName is the actual file we will be transfering...
                #endregion
            }
            #region Catch
            catch (UnauthorizedAccessException ex)
            {
                NewFileName = null;
                msg = string.Format("Access permission error in ChainRename Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (ArgumentException ex)
            {
                NewFileName = null;
                msg = string.Format("Invalid filename error in ChainRename Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (NotSupportedException ex)
            {
                NewFileName = null;
                msg = string.Format("Invalid filename format error in ChainRename Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (IOException ex)
            {
                NewFileName = null;
                msg = string.Format("I/O error in ChainRename Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...
            #endregion

            return (NewFileName);
        }
        #endregion

        #region CreateEmptyFile
        //public bool CreateEmptyFile(string srcFullPath)
        //{
        //    bool success = false;
        //    string msg;
        //    try
        //    {
        //        using (FileStream stream = File.Create(srcFullPath))
        //        {
        //            success = true;
        //        }
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Access permission error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Invalid filename error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (NotSupportedException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Invalid filename format error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (IOException ex)
        //    {
        //        success = false;
        //        msg = string.Format("I/O error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    //catch (Exception ex)
        //    //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

        //    return (success);
        //}
        #endregion

        #region MoveToArchive
        /// <summary>
        /// Move Journal file to the Archive Dir
        /// </summary>
        /// <returns>bool</returns>
        public bool MoveToArchive()
        {
            string msg;
            bool success = false;

            //string srcFullPath = string.Format("{0}\\{1}\\{2}",
            //                                           JTMThreadRegistry.JTM_FilePoolRoot,
            //                                           JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
            //                                           JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
            //                                           );
            //string ArchivePath = string.Format("{0}\\{1}\\{2}",
            //                                           JTMThreadRegistry.JTM_ArchiveRoot,
            //                                           JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
            //                                           JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
            //                                           );
            string srcFullPath = string.Format("{0}\\{1}",
                                           JTMThreadRegistry.JTM_FilePoolRoot,
                                           JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
                                           );
            string ArchivePath = string.Format("{0}\\{1}",
                                                       JTMThreadRegistry.JTM_ArchiveRoot,
                                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
                                                       );

            try
            {
                File.Move(srcFullPath, ArchivePath);
                success = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                success = false;
                msg = string.Format("Access permission error while moving file {0} to {1}!\nThe error message reads:\n{2}", srcFullPath, ArchivePath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (ArgumentException ex)
            {
                success = false;
                msg = string.Format("Invalid filename error while moving file {0} to {1}!\nThe error message reads: \n{2}", srcFullPath, ArchivePath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (NotSupportedException ex)
            {
                success = false;
                msg = string.Format("Invalid filename format error while moving file {0} to {1}!\nThe error message reads: \n{2}", srcFullPath, ArchivePath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (IOException ex)
            {
                success = false;
                msg = string.Format("I/O error while moving file {0} to {1}!\nThe error message reads: \n{2}", srcFullPath, ArchivePath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

            return (success);
        }
        #endregion

        #region DeleteTheFile
        /// <summary>
        /// DeleteTheFile
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <returns>bool</returns>
        //public bool DeleteTheFile(string srcFullPath)
        //{
        //    bool success = false;
        //    string msg;

        //    try
        //    {
        //        File.Delete(srcFullPath);
        //        success = true;
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Access permission error while deleting file {0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Invalid filename error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (NotSupportedException ex)
        //    {
        //        success = false;
        //        msg = string.Format("Invalid filename format error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    catch (IOException ex)
        //    {
        //        success = false;
        //        msg = string.Format("I/O error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
        //        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
        //    }
        //    //catch (Exception ex)
        //    //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upstream...

        //    return (success);
        //}
        #endregion

        #region PSEXEC / InitEJ
        public bool CallInitEJ(string ATMMachineName, string ATMUser, string ATMPassword, bool WinAuth)
        {
            int PSExecReturnCode = -1;
            string PSXeqArg;

            // Sample PSExec invocation
            // C:\Tools\SysInternal\psexec \\ATMXP-04 -u UserId -p password "c:\Program Files\Advance NDC\InitEJ.exe"
            //string PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} \"c:\\Program Files\\Advance NDC\\InitEJ.exe\"", ATMMachineName, ATMUser, ATMPassword);

            if (!WinAuth)
                PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} \"{3}\"", ATMMachineName, ATMUser, ATMPassword, JTMThreadRegistry.JTM_InitEJPath);
            else
                PSXeqArg = string.Format("\\\\{0} \"{1}\"", ATMMachineName, JTMThreadRegistry.JTM_InitEJPath);

            PSExecReturnCode = StartPSExec(PSXeqArg);

            if (PSExecReturnCode == 0)
                return true;
            else
                return false;
        }

        public int StartPSExec(string arguments)
        {
            int rc = -1;
            string msg;
            // static string PSExecFileName = @"c:\Tools\SysInternal\PSexec.exe";
            try
            {
                Process oProcess = new Process();
                oProcess.EnableRaisingEvents = false;
                oProcess.StartInfo.CreateNoWindow = true;
                oProcess.StartInfo.UseShellExecute = false;
                oProcess.StartInfo.RedirectStandardOutput = true;
                oProcess.StartInfo.FileName = JTMThreadRegistry.JTM_PSExecPath;
                oProcess.StartInfo.Arguments = arguments;
                oProcess.Start();

                while (!oProcess.HasExited)
                {
                    Thread.Sleep(1000);
                }
                rc = oProcess.ExitCode;
            }
            catch (InvalidOperationException ex)
            {
                rc = -1;
                msg = string.Format("InvalidOperationException while starting PSEXEC for ATM:{0}. The exception message reads: \n{1} ",
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (Win32Exception ex)
            {
                rc = -1;
                msg = string.Format("Win32Exception while starting PSEXEC for ATM:{0}. The exception message reads: \n{1} ",
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (rc);
        }
        #endregion

        #region PSEXEC / Cmd Copy
        public bool CallCmdToCopy(string ATMMachineName, string ATMUser, string ATMPassword, bool WinAuth, string LocalSrc, string LocalDst)
        {
            // LocalSrc = c:\Diebold\EDC\EDCLocal.dat || c:\Program Files\Advance NDC\Data\EJData.log
            int PSExecReturnCode = -1;
            string PSXeqArg;

            // Sample PSExec invocation
            // psexec \\ATMXP-04 -u Username -p password cmd /c copy /y "c:\Diebold\EDC\EDCLocal.dat" "c:\Diebold\EDC\EJRCPY.LOG"
            // psexec \\ATMXP-01 -u UserName -p password cmd /c copy / y "c:\Program Files\Advance NDC\Data\EJData.Log" "c:\Program Files\Advance NDC\Data"\EJRCPY.LOG"
            // string PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} cmd /c copy /y \"{3}\" \"{4}\", ATMMachineName, ATMUser, ATMPassword, LocalSrc, LocalDst);


            if (!WinAuth)
                PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} cmd /c copy /y \"{3}\" \"{4}\"", ATMMachineName, ATMUser, ATMPassword, LocalSrc, LocalDst);
            else
                PSXeqArg = string.Format("\\\\{0} cmd /c copy /y \"{1}\" \"{2}\"", ATMMachineName, LocalSrc, LocalDst);

            PSExecReturnCode = PSExecCmdCopy(PSXeqArg);

            if (PSExecReturnCode == 0)
                return true;
            else
                return false;
        }

        public int PSExecCmdCopy(string arguments)
        {
            int rc = -1;
            string msg;
            // static string PSExecFileName = @"c:\Tools\SysInternal\PSexec.exe";
            try
            {
                Process oProcess = new Process();
                oProcess.EnableRaisingEvents = false;
                oProcess.StartInfo.CreateNoWindow = true;
                oProcess.StartInfo.UseShellExecute = false;
                oProcess.StartInfo.RedirectStandardOutput = true;
                oProcess.StartInfo.FileName = JTMThreadRegistry.JTM_PSExecPath;
                oProcess.StartInfo.Arguments = arguments;
                oProcess.Start();

                while (!oProcess.HasExited)
                {
                    Thread.Sleep(500);
                }
                rc = oProcess.ExitCode;
            }
            catch (InvalidOperationException ex)
            {
                rc = -1;
                msg = string.Format("InvalidOperationException while starting PSEXEC for ATM:{0}. The exception message reads: \n{1} ",
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (Win32Exception ex)
            {
                rc = -1;
                msg = string.Format("Win32Exception while starting PSEXEC for ATM:{0}. The exception message reads: \n{1} ",
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (rc);
        }
        #endregion

        #region Trim Diebold File
        /// <summary>
        /// TrimDieboldFile(string srcFile)
        /// </summary>
        /// <param name="srcFile"></param>
        /// <returns>Number of lines</returns>
        private static int TrimDieboldFile(string srcFile)
        {
            // Creates a temprary file where the useful data is copied to.
            // On success, renames the temporary file to the original file name, having first deleted the original file.
            int i;
            int counter = 0;
            int PrevLineSeq = -1;
            int CurrLineSeq;
            string srcLine;
            string strSeqNo;
            string dstFile;

            StreamReader inpFile = null;
            StreamWriter outFile = null;
            dstFile = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetRandomFileName());
            try
            {
                // Read the file line by line and output into the temporary file
                inpFile = new System.IO.StreamReader(srcFile);
                outFile = new System.IO.StreamWriter(dstFile);

                srcLine = inpFile.ReadLine();
                counter++;
                do
                {
                    outFile.WriteLine(srcLine);
                    srcLine = inpFile.ReadLine();
                    if (srcLine != null)
                    {
                        strSeqNo = srcLine.Substring(1, 6); // position 2 to 7 holds the Diebold eJournal sequence number
                        if (Int32.TryParse(strSeqNo, out i))
                        {
                            CurrLineSeq = i;
                            if (CurrLineSeq == PrevLineSeq + 1)
                            {
                                PrevLineSeq = CurrLineSeq;
                                counter++;
                            }
                            else
                            {
                                break; // not in sequence; must have reached the end of valid data. Break the loop...
                            }
                        }
                        else
                        {
                            break; // not a valid integer; must have reached the end of valid data. Break the loop...
                        }
                    }
                } while (srcLine != null);

                inpFile.Close();
                outFile.Close();
                File.Delete(srcFile);
                File.Move(dstFile, srcFile);
            }
            catch (Exception ex)
            {
                inpFile.Close();
                outFile.Close();
                if (File.Exists(dstFile))
                    File.Delete(dstFile);

                string msg = string.Format("Encountered an Exception while extracting valid data from Diebold eJournal [{0]]!\nThe error message reads:\n{1}", srcFile, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                return (-1);
            }
            return (counter);
        }
        #endregion

        #endregion
    }

}

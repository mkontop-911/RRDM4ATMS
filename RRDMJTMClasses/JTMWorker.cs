using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Data.SqlClient;
using System.Configuration;

using System.Diagnostics;
using System.ComponentModel;
using System.Security.Cryptography;

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

            msg = string.Format("Processing Request --> MsgID:{0} Command:{1} ATM:\\\\{2}\\{3} Requestor:{4}",
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgID,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Command,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ATMMachineName,
                  JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
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
                    case JTMQueueCommand.Cmd_FETCHDEL:
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
                Thread.Sleep(JTMThreadRegistry.JTM_FETCH_RetryTimeout);
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
                if (WinAuth == true) // Windows Authentication, no need for JTMRemote
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
            bool BackupFlag = ((Cmd == JTMQueueCommand.Cmd_FETCHDEL) ? true : false);

            // Check for Abort_Abort at each step
            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region Fetch the Journal file
            // Fetch the Journal file
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

            #region Handle received Journal file (FETCHDEL: move to Archive, FETCH: delete)
            if (Cmd == JTMQueueCommand.Cmd_FETCHDEL)
            {   // move the received file in the Archive
                success = this.MoveToArchive();
                if (!success)
                {
                    string FileThatFailedToMove = string.Format("{0}\\{1}\\{2}",
                                       JTMThreadRegistry.JTM_FilePoolRoot,
                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
                                       );
                    msg = string.Format("Failed to move file {0} to the archive after successfull processing!", FileThatFailedToMove);
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                }
            }
            else
            {   // delete the received file
                // TODO --- why not keep it??????????
                string FileToDelete = string.Format("{0}\\{1}\\{2}",
                    JTMThreadRegistry.JTM_FilePoolRoot,
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName);
                success = this.DeleteTheFile(FileToDelete);
                if (!success)
                {
                    msg = string.Format("Failed to delete file {0} after successfull processing!", FileToDelete);
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                }
            }
            #endregion

            if (JTMThreadRegistry.Abort_Abort == true) return (false);

            #region CloseUp

            // Update Identification Details. This is done only once, here!!!
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.DateLastUpdated = DateTime.Now;
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileUploadRequestDt = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.MsgDateTime;
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.FileParseEnd = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileParseEnd;
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultCode = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode;
            JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultMessage = JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage;

            success = this.UpdateIndentificationDetailsRecord();

            JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ProcessEnd = DateTime.Now;
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
                                          JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFilePath.Trim(),
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
                UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
            }
            else
            {
                // Failure to fetch file
                success = false;
                string msg = string.Format("Failure to copy the file {0}\\{1} from {2}",
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
            string msg;
            string ReceivedFullFileName;
            bool OvrWr = true;
            string DstFile;
            string srcFullPath;
            string dstFullPath;

            DateTime dt = DateTime.Now;
            DstFile = string.Format("{0}-{1}-{2}{3}",
                                     ATMNo.Trim(),
                                     Path.GetFileNameWithoutExtension(SrcFile.Trim()),
                                     dt.ToString("yyyyMMdd-HHmmss.ffffff"),
                                     Path.GetExtension(SrcFile.Trim())
                                   );
            srcFullPath = Path.Combine(SrcPath, SrcFile);
            dstFullPath = Path.Combine(DstPath, DstFile);

            if (JTMThreadRegistry.Abort_Abort == true)
                return (null);

            if (WinAuth == true)
            {
                ReceivedFullFileName = this.GetJournalFileWithRetries(srcFullPath, dstFullPath, OvrWr, DoBackup);
            }
            else
            {
                try
                {
                    using (JTMRemote.EstablishConnection(RemoteMachine, UserName, Password))
                    {
                        ReceivedFullFileName = this.GetJournalFileWithRetries(srcFullPath, dstFullPath, OvrWr, DoBackup);
                    }
                }
                catch (Win32Exception ex)
                {
                    ReceivedFullFileName = null;
                    msg = string.Format("Error in JTMWorker:FETCHJournalFile while connecting to [{0}] using UserName:{1}!" +
                                        "\nThe error reads: Source:{2} ErrorCode:{3} ErrorMessage:{4}",
                                         RemoteMachine, UserName, ex.Source, ex.NativeErrorCode.ToString(), ex.Message);
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                }
                //catch (Exception ex)
                // Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

            }
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
                Thread.Sleep(JTMThreadRegistry.JTM_FETCH_RetryTimeout);
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
        /// <param name="srcFullPath"></param>
        /// <param name="dstFullPath"></param>
        /// <param name="OvrWr"></param>
        /// <returns>
        /// The fully qualified destination file name if successfull
        /// null if not</returns>
        public string GetJournalFile(string srcFullPath, string dstFullPath, bool OvrWr, bool DoBackup)
        {
            // bool success;
            string RenamedSource = null;
            string msg;
            string DestFullFileName = dstFullPath;
            
            int MaxJBck = JTMThreadRegistry.JTM_MaxJournalBackups;

            if (JTMThreadRegistry.Abort_Abort == true)
                return (null);

            if (DoBackup)
            {
                RenamedSource = this.BackupAndRecreate(srcFullPath, MaxJBck);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(RenamedSource))
                {
                    File.Copy(RenamedSource, DestFullFileName, OvrWr);
                }
                else
                {
                    File.Copy(srcFullPath, DestFullFileName, OvrWr);
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
            // Any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

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
            bool success = false;
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
                success = false;
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
                success = false;
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

                ret = JQ1.InvokeStoredProcedure(
                                        ProcedureName,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.BankID,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo,
                                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.BranchNo,
                                        FullName);

                if (JQ1.ErrorFound != true)
                {
                    if (ret == 0)
                    {
                        success = true;
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.Stage = JTMQueueStage.Const_ParserFinished;
                        JTMThreadRegistry.ThreadArray[ThreadIndex].Req.FileParseEnd = DateTime.Now;
                        UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                    }
                    else
                    {
                        success = false;
                        msg = string.Format("The Stored Procedure returned an error! Input file is:{0}\n. RerurnCode: {1}",
                                             FullName, ret);
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
                    success = false;
                    msg = string.Format("Failure in executing the Stored Procedure on SQL! Input fle is:{0}. The error message reads: {1}",
                                         FullName, JQ1.ErrorOutput);
                    JTMThreadRegistry.ChangeThreadStatus(ThreadIndex, StatusOfThread.Stopping);
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultCode = JTMQueueResult.Failure;
                    JTMThreadRegistry.ThreadArray[ThreadIndex].Req.ResultMessage = msg;
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                    UpdSuccess = this.UpdateJTMQueueRecordInSQL(JTMThreadRegistry.ThreadArray[ThreadIndex].Req);
                    // if (UpdSuccess != true) { ; } // UpdateRecordInSQL will have thrown the JTMCustomException
                }
            }
            return (success);
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
            JdUpd.AtmNo = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.AtmNo;
            JdUpd.DateLastUpdated = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.DateLastUpdated;
            JdUpd.UserId = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.UserId;
            JdUpd.BatchID = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.BatchID;
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
            JdUpd.ResultCode = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultCode;
            JdUpd.ResultMessage = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ResultMessage;
            JdUpd.Operator = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.Operator;
            JdUpd.RecordFound = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.RecordFound;
            JdUpd.ErrorFound = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ErrorFound;
            JdUpd.ErrorOutput = JTMThreadRegistry.ThreadArray[ThreadIndex].IdentClass.ErrorOutput;

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
                                    JdUpd.ResultMessage);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            return (success);
        }
        #endregion

        #region BackupAndRecreate
        /// <summary>
        /// BackupAndRecreate
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <param name="MaxBckFileCount"></param>
        /// <returns>New name of original file</returns>
        public string BackupAndRecreate(string srcFullPath, int MaxBckFileCount)
        {
            string NewFileName = null;
            string msg;
            bool rc = false;
            string srcFile;         //  "EJDATA.LOG";
            string srcDir;          //  @"C:\RRDM\ATM";
            string FileSearchMask;  //  "EJDATA.LOG.???.BAK";
            string FileBackupMask;  //  "EJDATA.LOG.{0}.BAK";

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

                // Delete the files over the MaxBckFileCount 
                // No backup is to be taken... delete any leftovers
                if (MaxBckFileCount == 0)
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

                if (LastIndx > 0)
                {
                    // Rename older files in reverse sequence (eg: 3->4, 2->3, 1->2)
                    for (p = LastIndx; p > 0; p--)
                    {
                        prev = Path.Combine(srcDir, (string.Format(FileBackupMask, (p).ToString("000"))));
                        next = Path.Combine(srcDir, (string.Format(FileBackupMask, (p + 1).ToString("000"))));
                        //if (Environment.UserInteractive)
                        //{
                        //    Console.WriteLine("Rename {0}  to  {1}", prev, next);
                        //}
                        File.Move(prev, next);
                    }
                }

                // Rename the actual Log file to 001
                // TODO rethink about renaming actual LOG file ??? !!!!!!!!!!!!!!!!!!!
                SrcLogFile = Path.Combine(srcDir, srcFile);
                DstBakFile = Path.Combine(srcDir, (string.Format(FileBackupMask, "001")));
                File.Move(SrcLogFile, DstBakFile);
                NewFileName = DstBakFile; // NewFileName is the actual file we will be retrieving...

                // Create an empty file 
                // TODO rethink about creating empty files...
                // the disruption to the ATM operation is only between the previous line (File.Move) and this one (CreateEmptyFile)
                rc = this.CreateEmptyFile(srcFullPath);
                if (rc != true)
                {
                    // TODO .... Will the ATM must recover by itself?????????????
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                NewFileName = null;
                msg = string.Format("Access permission error in BackupAndRecreate Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (ArgumentException ex)
            {
                NewFileName = null;
                msg = string.Format("Invalid filename error in BackupAndRecreate Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (NotSupportedException ex)
            {
                NewFileName = null;
                msg = string.Format("Invalid filename format error in BackupAndRecreate Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (IOException ex)
            {
                NewFileName = null;
                msg = string.Format("I/O error in BackupAndRecreate Log file:{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

            return (NewFileName);
        }
        #endregion

        #region CreateEmptyFile
        public bool CreateEmptyFile(string srcFullPath)
        {
            bool success = false;
            string msg;
            try
            {
                using (FileStream stream = File.Create(srcFullPath))
                {
                    success = true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                success = false;
                msg = string.Format("Access permission error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (ArgumentException ex)
            {
                success = false;
                msg = string.Format("Invalid filename error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (NotSupportedException ex)
            {
                success = false;
                msg = string.Format("Invalid filename format error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (IOException ex)
            {
                success = false;
                msg = string.Format("I/O error in CreateEmptyFile():{0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

            return (success);
        }
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

            string srcFullPath = string.Format("{0}\\{1}\\{2}",
                                                       JTMThreadRegistry.JTM_FilePoolRoot,
                                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
                                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.DestnFileName.Trim()
                                                       );
            string ArchivePath = string.Format("{0}\\{1}\\{2}",
                                                       JTMThreadRegistry.JTM_ArchiveRoot,
                                                       JTMThreadRegistry.ThreadArray[ThreadIndex].Req.AtmNo.Trim(),
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
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

            return (success);
        }
        #endregion

        #region DeleteTheFile
        /// <summary>
        /// DeleteTheFile
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <returns>bool</returns>
        public bool DeleteTheFile(string srcFullPath)
        {
            bool success = false;
            string msg;

            try
            {
                File.Delete(srcFullPath);
                success = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                success = false;
                msg = string.Format("Access permission error while deleting file {0}!\nThe error message reads:\n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (ArgumentException ex)
            {
                success = false;
                msg = string.Format("Invalid filename error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (NotSupportedException ex)
            {
                success = false;
                msg = string.Format("Invalid filename format error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            catch (IOException ex)
            {
                success = false;
                msg = string.Format("I/O error while deleting file {0}!\nThe error message reads: \n{1}", srcFullPath, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }
            //catch (Exception ex)
            //TODO any other type of exception (e.g. threadabort, JTMCustom,..., will propagate upsteram...

            return (success);
        }
        #endregion

        #endregion
    }
        
}

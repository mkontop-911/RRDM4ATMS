using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

using RRDM4ATMs;

namespace RRDMJTMClasses
{
    public class JTMServer
    {
        // Event Handle to wait on for thread to start 
        private static AutoResetEvent hThreadStartedEvent;

        string connectionString = ConfigurationManager.ConnectionStrings["ATMsConnectionString"].ConnectionString;

        public Thread StartJTM()
        {
            // Start the JTM Server thread
            ThreadStart ts = new ThreadStart(JTMServerThread);
            Thread oT = new Thread(ts);
            oT.Name = "JTMServer";

            // TODO investigate IsBackground
            // oT.IsBackground = true; 

            oT.Start();
            return (oT);
        }

        //public void JTMServerThread()
        private void JTMServerThread()
        {
            string msg;
            string ValMsg = "";
            string ValMsgFmt = @"ParamId:{0}, OccuranceId:{1} --> {2}";
            // From ATMs parameter tables
            int JTM_MaxThreadNumber;
            int JTM_FETCH_RetryWaitTime;
            int JTM_FETCH_Retries;
            int JTM_SleepWaitNewRequest;
            int JTM_SleepWaitEmptyThreadSlot;
            int JTM_StartThreadTimeout;
            int JTM_ThreadMonitorInterval;
            int JTM_ThreadAbortWait;
            int JTM_MaxThreadLifeSpan;

            string JTM_FilePoolRoot = "";
            string JTM_ArchiveRoot = "";
            string JTM_ParserSP = "";
            int JTM_MaxJournalBackups;
            string JTM_PSExecPath = "";
            string JTM_InitEJPath = "";

            #region Get AppSettings["JTMOperator", "SQLRelativeFilePoolPath"]
            string Operator = null;
            string SQLRelativeFilePoolPath = null;
            try
            {
                Operator = ConfigurationManager.AppSettings["JTMOperator"];
                SQLRelativeFilePoolPath = ConfigurationManager.AppSettings["SQLRelativeFilePoolPath"];
            }
            catch (ConfigurationErrorsException ex)
            {
                msg = string.Format("Terminating because of a configuration file error while reading AppSettings values. The error reads:\n{0}", ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                if (Environment.UserInteractive)
                    Console.WriteLine(msg);
                return;
            }
            catch (Exception ex)
            {
                msg = string.Format("Terminating because of a configuration file error! The error reads:\n{0}", ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                if (Environment.UserInteractive)
                    Console.WriteLine(msg);
                return;
            }

            if (string.IsNullOrEmpty(Operator))
            {
                msg = "Terminating because of an error in reading the 'Operator' value from the configuration file!";
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                if (Environment.UserInteractive)
                    Console.WriteLine(msg);
                return;
            }
            #endregion

            #region Read Parameters from database and validate

            // TODO Consider reading all parameters in one go...

            RRDMGasParameters Gp = new RRDMGasParameters();

            // Max Threads to start
            Gp.ReadParametersSpecificId(Operator, "910", "1", "", "");
            JTM_MaxThreadNumber = (int)Gp.Amount;
            if (JTM_MaxThreadNumber == 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "1", "Invalid MaxWorkerThreads!\n");

            // How many times to try to fetch a journal file
            Gp.ReadParametersSpecificId(Operator, "910", "2", "", "");
            JTM_FETCH_Retries = (int)Gp.Amount;
            if (JTM_FETCH_Retries < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "2", "Invalid FetchRetries!\n");

            // How much to wait between fetch retries
            Gp.ReadParametersSpecificId(Operator, "910", "3", "", "");
            JTM_FETCH_RetryWaitTime = (int)Gp.Amount;
            if (JTM_FETCH_RetryWaitTime < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "3", "Invalid FetchTimeout!\n");

            // How long to wait for an available thread slot
            Gp.ReadParametersSpecificId(Operator, "910", "4", "", "");
            JTM_SleepWaitEmptyThreadSlot = (int)Gp.Amount;
            if (JTM_SleepWaitEmptyThreadSlot < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "4", "Invalid SleepWaitEmptyThreadSlot!\n");

            // How long to wait for thread to start (ms)
            Gp.ReadParametersSpecificId(Operator, "910", "5", "", "");
            JTM_StartThreadTimeout = (int)Gp.Amount;
            if (JTM_StartThreadTimeout < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "5", "Invalid StartThreadTimeout!\n");

            // How long to wait before querying for new requests ((ms)
            Gp.ReadParametersSpecificId(Operator, "910", "6", "", "");
            JTM_SleepWaitNewRequest = (int)Gp.Amount;
            if (JTM_SleepWaitNewRequest < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "6", "Invalid SleepWaitNewRequest!\n");

            // ThreadWatch iteration interval (ms)
            Gp.ReadParametersSpecificId(Operator, "910", "7", "", "");
            JTM_ThreadMonitorInterval = (int)Gp.Amount;
            if (JTM_ThreadMonitorInterval < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "7", "Invalid ThreadMonitorInterval!\n");

            // How long to wait for threads to finish after Abort_Abort is set
            Gp.ReadParametersSpecificId(Operator, "910", "8", "", "");
            JTM_ThreadAbortWait = (int)Gp.Amount;
            if (JTM_ThreadAbortWait < 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "8", "Invalid ThreadAbortWait!\n");

            // Max Life span of thread (in seconds)
            Gp.ReadParametersSpecificId(Operator, "910", "9", "", "");
            JTM_MaxThreadLifeSpan = (int)Gp.Amount;
            if (JTM_MaxThreadLifeSpan <= 0)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "910", "9", "Invalid MaxThreadLifeSpan!\n");

            // Root dir of FilePool
            Gp.ReadParametersSpecificId(Operator, "911", "1", "", "");
            JTM_FilePoolRoot = Gp.OccuranceNm;
            if (!Directory.Exists(JTM_FilePoolRoot))
                ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "1", "Invalid or non-existent 'FilePoolRoot' directory!\n");

            // Root dir of Archive
            Gp.ReadParametersSpecificId(Operator, "911", "2", "", "");
            JTM_ArchiveRoot = Gp.OccuranceNm;
            if (!Directory.Exists(JTM_ArchiveRoot))
                ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "2", "Invalid or non-existent 'ArchiveRoot' directory!\n");

            // Stored Procedure for Parsing
            Gp.ReadParametersSpecificId(Operator, "911", "3", "", "");
            JTM_ParserSP = Gp.OccuranceNm;
            if (string.IsNullOrEmpty(JTM_ParserSP))
                ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "3", "Invalid name of Parser Stored Procedure!\n");

            // Max backups to keep on the ATM
            Gp.ReadParametersSpecificId(Operator, "911", "4", "", "");
            JTM_MaxJournalBackups = (int)Gp.Amount;
            if (JTM_MaxJournalBackups < 1)
                ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "4", "Invalid MaxJournalBackups!\n");


            // Path of PSExec program on machine running JTM .. should exist in the same directory as .this
            string val = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            JTM_PSExecPath = Path.Combine(val, "PSExec.exe");
            if (!File.Exists(JTM_PSExecPath))
            {
                ValMsg = ValMsg + string.Format("Could not locate PSEXEC [{0}]!\n", JTM_PSExecPath);
            }

            //Gp.ReadParametersSpecificId(Operator, "911", "5", "", "");
            //JTM_PSExecPath = Gp.OccuranceNm;
            //if (string.IsNullOrEmpty(JTM_PSExecPath))
            //{
            //    ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "5", "Invalid path for the PSEXEC.EXE program!\n");
            //}
            //else if (!File.Exists(JTM_PSExecPath))
            //{
            //    ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "5", "Could not locate PSEXEC [{0}]!\n", JTM_PSExecPath);
            //}

            // Case of InitEJ - Uncomment 911,6
            //// Path of InitEJ.EXE program on ATMs
            //Gp.ReadParametersSpecificId(Operator, "911", "6", "", "");
            //JTM_InitEJPath = Gp.OccuranceNm;
            //if (string.IsNullOrEmpty(JTM_ParserSP))
            //    ValMsg = ValMsg + string.Format(ValMsgFmt, "911", "6", "Invalid path for the InitEJ.EXE program!\n");

            // Check if any of the above caused an error...
            if (!string.IsNullOrEmpty(ValMsg))
            {
                msg = string.Format("The program terminates because of invalid parameters. Details:\n{0}", ValMsg);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                if (Environment.UserInteractive)
                    Console.WriteLine(msg);
                return;
            }

            // Log the starting parameters...
            string fstr = "JTM stared with the following parameters:\n" +
                          "MaxThreads:{0}\n" +
                          "FETCH_Retries:{1}\n" +
                          "FETCH_RetryWaitTime:{2}\n" +
                          "SleepWaitNewRequest:{3}ms\n" +
                          "SleepWaitEmptyThreadSlot:{4}ms\n" +
                          "ThreadStartTimeout:{5}ms\n" +
                          "ThreadWatchInterval:{6}ms\n" +
                          "ThreadAbortWaitTime:{7}ms\n" +
                          "MaxThreadLifeSpan:{8}sec\n" +
                          "FilePoolRoot:{9}\n" +
                          "ArchiveRoot:{10}\n" +
                          "Max Journal Backup files at ATM:{11}\n" +
                          "Stored Procedure for Parsing:{12}\n" +
                          "Path to PSEXEC.EXE:{13}\n" +
                          // "Path to INITEJ.EXE on ATMs:{14}\n" +
                          "FilePoolRoot Relative to SQL:{15}"
                          ;

            msg = string.Format(fstr, JTM_MaxThreadNumber,
                                      JTM_FETCH_Retries,
                                      JTM_FETCH_RetryWaitTime,
                                      JTM_SleepWaitNewRequest,
                                      JTM_SleepWaitEmptyThreadSlot,
                                      JTM_StartThreadTimeout,
                                      JTM_ThreadMonitorInterval,
                                      JTM_ThreadAbortWait,
                                      JTM_MaxThreadLifeSpan,
                                      JTM_FilePoolRoot,
                                      JTM_ArchiveRoot,
                                      JTM_MaxJournalBackups,
                                      JTM_ParserSP,
                                      JTM_PSExecPath,
                                      JTM_InitEJPath,
                                      SQLRelativeFilePoolPath);
            EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Information);

            #endregion

            #region Instantiate JTMThreadRegistry
            // Instantiate the class to manage the threads -- This must happen only once!!
            JTMThreadRegistry ThrRegistry = new JTMThreadRegistry(
                                                JTM_MaxThreadNumber,
                                                JTM_FETCH_Retries,
                                                JTM_FETCH_RetryWaitTime,
                                                JTM_SleepWaitNewRequest,
                                                JTM_SleepWaitEmptyThreadSlot,
                                                JTM_StartThreadTimeout,
                                                JTM_ThreadMonitorInterval,
                                                JTM_ThreadAbortWait,
                                                JTM_MaxThreadLifeSpan,
                                                JTM_FilePoolRoot,
                                                JTM_ArchiveRoot,
                                                JTM_MaxJournalBackups,
                                                JTM_ParserSP,
                                                JTM_PSExecPath,
                                                JTM_InitEJPath,
                                                SQLRelativeFilePoolPath);
            #endregion

            // Register ourselves
            JTMThreadRegistry.oJTMServerThread = Thread.CurrentThread;

            // Start the thread that will monitor the worker threads
            JTMThreadRegistry.oJTMThreadMonitor = StartThreadMonitor(ThrRegistry);

            // Start the ConsoleDisplay thread
            if (Environment.UserInteractive)
            {
                JTMThreadRegistry.oJTMConsoleDisplayThread = StartConsoleDisplayThread(ThrRegistry);
            }

            // Instantiate the event to wait on for worker threads to signal that they started
            hThreadStartedEvent = new AutoResetEvent(false);

            // Enter the server loop to process incoming requests...
            try
            {
                while (JTMThreadRegistry.Abort_Abort != true) // exit the loop if Abort_Abort is raised
                {
                    int Indx = -1;
                    JTMWorker oWThrClass;

                    // Read from the JTMQueue table
                    RRDMJTMQueue JtmQ = new RRDMJTMQueue();
                    JtmQ.ReadSingleJTMQueueByPriority(); // Members include the JTMQueueStruct element

                    // If SQL returned error, Log and throw a new JTMCustomException to terminate
                    if (JtmQ.ErrorFound)
                    {
                        msg = string.Format("Scanning for new requests from SQL returned an error! The program terminates!!!\nThe error reads:\n{0}",
                                            JtmQ.ErrorOutput);
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                        JTMCustomException exnew = new JTMCustomException(msg);
                        exnew.JTMCode = JTMQueueResult.Failure;
                        exnew.JTMMessage = " The program terminates because of an SQL error!";
                        exnew.JTMSource = "JTMServerThread";
                        exnew.JTMFatal = true; // The caller must handle the exception and signal for termination....
                        throw (exnew);
                    }

                    if (JtmQ.RecordFound)
                    {
                        #region Process the request
                        // Avoid concurrent operations on the same ATM
                        if (JTMThreadRegistry.ATM_HasRequestInProgress(JtmQ.AtmNo))
                        {
                            msg = string.Format("Request [{0}] for ATM [{1}] is postponed! Another request is already in progress!" , JtmQ.Command, JtmQ.AtmNo);
                            EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Warning);
                            Thread.Sleep(JTM_SleepWaitNewRequest);
                            continue;
                        }

                        switch (JtmQ.Command)
                        {
                            case JTMQueueCommand.Cmd_FETCH:
                            // case JTMQueueCommand.Cmd_FETCHDEL:
                            case JTMQueueCommand.Cmd_ATMSTATUS:
                                {
                                    #region FETCH / ATMSTATUS

                                    // Get an empty slot in the ThreaArray
                                    Indx = JTMThreadRegistry.GetEmptySlot();
                                    while (Indx == -1)
                                    {
                                        // Sleep for a while and loop until one is available
                                        Thread.Sleep(JTM_SleepWaitEmptyThreadSlot);
                                        Indx = JTMThreadRegistry.GetEmptySlot();
                                    }

                                    // Fill in the ThreadArray strucrure at Indx with the record retrieved from SQL
                                    CopySqlToThreadArrayRecord(Indx, JtmQ);

                                    #region Get the corresponding IdentificationDetails record
                                    RRDMJTMIdentificationDetailsClass IdentDetClass = new RRDMJTMIdentificationDetailsClass();
                                    JTMThreadRegistry.ThreadArray[Indx].IdentClass = IdentDetClass;
                                    JTMThreadRegistry.ThreadArray[Indx].IdentClass.ReadJTMIdentificationDetailsByAtmNo(JtmQ.AtmNo);

                                    if (!JTMThreadRegistry.ThreadArray[Indx].IdentClass.RecordFound)
                                    {
                                        // stop here, do not start a new thread
                                        msg = string.Format("Error while reading the ATM record (ReadJTMIdentificationDetailsByAtmNo)!\n The ATM is {0}\nThe error message reads:\n{1}",
                                                             JtmQ.AtmNo,
                                                             JTMThreadRegistry.ThreadArray[Indx].IdentClass.ErrorOutput);
                                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                                        // Mark the Queue Rec so as not to be re-processed
                                        JTMThreadRegistry.ThreadArray[Indx].Req.Stage = JTMQueueStage.Const_Aborted;
                                        JTMThreadRegistry.ThreadArray[Indx].Req.ResultCode = JTMQueueResult.Failure;
                                        JTMThreadRegistry.ThreadArray[Indx].Req.ResultMessage = msg;
                                        JtmQ.Stage = JTMQueueStage.Const_Aborted;
                                        JtmQ.ResultCode = JTMQueueResult.Failure;
                                        if (!JTMThreadRegistry.ThreadArray[Indx].IdentClass.ErrorFound)
                                        {
                                            msg = string.Format("ATM [{0}] not found in JTMIdentificationDetails!", JtmQ.AtmNo);
                                        }
                                        JtmQ.ResultMessage = msg;
                                        JtmQ.UpdateRecordInJTMQueueByMsgID(JtmQ.MsgID);

                                        // Mark the slot for release
                                        JTMThreadRegistry.ThreadArray[Indx].Status = StatusOfThread.Canceled;

                                        break;
                                    }
                                    #endregion

                                    // Create a unique name for the thread
                                    DateTime dt = DateTime.Now;
                                    string tName = string.Format("JTM{0}-{1}-{2}",
                                        JtmQ.MsgID.ToString("00000000"), JtmQ.AtmNo, dt.Ticks.ToString("000000000000000000000000"));

                                    // Start the thread instance that will handle this JTMQueue record
                                    oWThrClass = null;
                                    oWThrClass = StartWorkerThread(Indx, tName, hThreadStartedEvent);
                                    if (oWThrClass != null)
                                    {
                                        // Raise the ClearToGo signal so that the thread carries on
                                        oWThrClass.hClearToGoEvent.Set();
                                    }
                                    else
                                    {
                                        // TODO Check if this code is reachable
                                        msg = "RRDMJTMSvc is aborting! Check for preceeding error messages to identify the cause..";
                                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                                        break;
                                    }
                                    #endregion
                                    break;
                                }
                            case "RESET":
                                //{
                                //    //TODO remove .... this is only for TESTING 
                                //    JTMThreadRegistry.RaiseAbortFlag();
                                //    break;
                                //}
                            default:
                                {
                                    // Invalid command - Log and discard
                                    msg = string.Format("Invalid command [{0}]!", JtmQ.Command);
                                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                                    JtmQ.Stage = JTMQueueStage.Const_Finished;
                                    JtmQ.ResultCode = JTMQueueResult.Failure;
                                    JtmQ.ResultMessage = msg;
                                    JtmQ.UpdateRecordInJTMQueueByMsgID(JtmQ.MsgID);
                                    break;
                                }
                        }
                        #endregion
                    }
                    else // No record found
                    {
                        Thread.Sleep(JTM_SleepWaitNewRequest);
                    }
                } // End of loop
            }
            catch (JTMCustomException ex)
            {
                string msgfmt = "JTM exception! The program will try to clean up all resources before terminating!\n" +
                                "JTM Source:{0}\n" +
                                "JTM Error code:{1}\n" +
                                "JTM message: {2}\n" +
                                "Original Message:\n{3}";
                msg = string.Format(msgfmt, ex.JTMSource, ex.JTMCode, ex.JTMMessage, ex.Message);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Information);

                if (ex.JTMFatal)
                    JTMThreadRegistry.RaiseAbortFlag();
            }
            catch (Exception ex)
            {
                string msgfmt = "The program encountered an error!! Terminating...\n" +
                                "Error Source:{0}\n" +
                                "Error Message: {1}\n" +
                                "Stack: {2}\n" +
                                "Exception: {3}";
                msg = string.Format(msgfmt, ex.Source, ex.Message, ex.StackTrace, ex.ToString());
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Information);
            }
            finally
            {
                // Raise the Abort_Abort flag
                JTMThreadRegistry.RaiseAbortFlag();

                // Wait for the ThreadMonitor thread to finish
                if (JTMThreadRegistry.oJTMThreadMonitor != null)
                {
                    JTMThreadRegistry.oJTMThreadMonitor.Join(); // wait for JTMCleanerThread to finish 
                }

                msg = "RRDMJTMSvc service stopped!";
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
            }

        }
        /* ===  End of JTMServer() === */
        /* =========================== */


        /// <summary> StartWorkerThread
        /// 
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="ThrName"></param>
        /// <param name="hCallerWaitEvent"></param>
        /// <returns>JTMWorker</returns>
        public static JTMWorker StartWorkerThread(int Index, string ThrName, AutoResetEvent hCallerWaitEvent)
        {
            string msg;
            JTMWorker instQJ = null;

            // Instantiate a new Woker class (The constructor will set its ThreadSlot private member to Index)
            JTMWorker oWorker = new JTMWorker(Index);

            // Start a new thread.. the object passed as a parameter is the AutoResetEvent 
            // on which the thread will signal back that it has started with success
            ParameterizedThreadStart pts = new ParameterizedThreadStart(oWorker.JTMWorkerThread);
            Thread oT = new Thread(pts);
            oT.Name = ThrName;
            try
            {
                oT.Start(hCallerWaitEvent);
            }
            catch (OutOfMemoryException ex)
            {
                instQJ = null;
                msg = string.Format("Could not start a new thread because of an 'Out of Memory' exception!! The program terminates!!!\nThe exception reads:\n{0}",
                                    ex.ToString());
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                JTMCustomException exnew = new JTMCustomException(msg);
                exnew.JTMCode = JTMQueueResult.Failure;
                exnew.JTMMessage = "OutOfMemory exception while starting a new worker thread! The program terminates";
                exnew.JTMSource = "StartWorkerThread";
                exnew.JTMFatal = true; // The caller must handle the exception and signal for termination....
                throw (exnew);
            }
            // Any other exception will be propagated upstream..


            // Wait for the thread to signal back 
            // this is how we make sure that the thread initialized properly and is ready to go..
            bool ThrStarted = hCallerWaitEvent.WaitOne(JTMThreadRegistry.JTM_StartWorkerThreadTimeout);
            if (ThrStarted)
            {
                // started within the time interval defined by JTM_StartWorkerThreadTimeout
                instQJ = oWorker;
            }
            else
            {
                instQJ = null;
                // The thread did not signal back...
                // Failed to start thread in the specified time interval
                msg = string.Format("Failed to start thread in the specified time interval of {0} milliseconds!", JTMThreadRegistry.JTM_StartWorkerThreadTimeout);
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                // This is a fatal error and the caller must handle the exception and signal for termination....
                JTMCustomException ex = new JTMCustomException(msg);
                ex.JTMCode = JTMQueueResult.Failure;
                ex.JTMMessage = "Failed to start a new worker thread! The program terminates";
                ex.JTMSource = "StartWorkerThread";
                ex.JTMFatal = true;
                throw (ex);
                // The exception will be propagated upstream and will be treaded as fatal!!!!
            }

            return (instQJ);
        }

        /// <summary> StartThreadMonitor
        /// 
        /// </summary>
        /// <returns>Thread</returns>
        public static Thread StartThreadMonitor(JTMThreadRegistry ThR)
        {
            Thread oT = null;
            //rc = false;
            if (ThR != null)
            {
                ThreadStart ts = new ThreadStart(ThR.JTMThreadMonitor);
                oT = new Thread(ts);
                oT.Name = "JTMThreadMonitor";
                try
                {
                    oT.Start();
                }
                catch (OutOfMemoryException ex)
                {
                    oT = null;
                    string msg = string.Format("Could not start the ThreadMonitor thread because of an 'Out of Memory' exception!! The program terminates!!!\nThe exception reads:\n{0}",
                                        ex.ToString());
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                    JTMCustomException exnew = new JTMCustomException(msg);
                    exnew.JTMCode = JTMQueueResult.Failure;
                    exnew.JTMMessage = "OutOfMemory exception while starting the ThreadMonitor thread! The program terminates";
                    exnew.JTMSource = "StartThreadMonitor";
                    exnew.JTMFatal = true; // The caller must handle the exception and signal for termination....
                    throw (exnew);
                }
            }
            return (oT);
        }

        /// <summary> StartConsoleDisplayThread
        /// 
        /// </summary>
        /// <param name="ThR"></param>
        /// <returns>Thread</returns>
        public static Thread StartConsoleDisplayThread(JTMThreadRegistry ThR)
        {
            Thread oT = null;
            //rc = false;
            if (ThR != null)
            {
                ThreadStart ts = new ThreadStart(ThR.JTMConsoleDisplay);
                oT = new Thread(ts);
                oT.Name = "JTMConsoleDisplay";
                try
                {
                    oT.Start();
                }
                catch (OutOfMemoryException ex)
                {
                    oT = null;
                    string msg = string.Format("Could not start the ConsoleDisplay thread because of an 'Out of Memory' exception!! The program terminates!!!\nThe exception reads:\n{0}",
                                        ex.ToString());
                    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);

                    JTMCustomException exnew = new JTMCustomException(msg);
                    exnew.JTMCode = JTMQueueResult.Failure;
                    exnew.JTMMessage = "OutOfMemory exception while starting the ConsoleDisplay thread! The program terminates";
                    exnew.JTMSource = "StartConsoleDisplay";
                    exnew.JTMFatal = true; // The caller must handle the exception and signal for termination....
                    throw (exnew);
                }
            }
            return (oT);
        }

        /// <summary> CopySqlRecordToArrayRecord
        /// 
        /// </summary>
        /// <param name="Indx"></param>
        /// <param name="JtmQ"></param>
        public static void CopySqlToThreadArrayRecord(int Indx, RRDMJTMQueue JtmQ)
        {
            JTMThreadRegistry.ThreadArray[Indx].Req.MsgID = JtmQ.MsgID;
            JTMThreadRegistry.ThreadArray[Indx].Req.MsgDateTime = JtmQ.MsgDateTime;
            JTMThreadRegistry.ThreadArray[Indx].Req.RequestorID = JtmQ.RequestorID;
            JTMThreadRegistry.ThreadArray[Indx].Req.RequestorMachine = JtmQ.RequestorMachine;
            JTMThreadRegistry.ThreadArray[Indx].Req.Command = JtmQ.Command;
            JTMThreadRegistry.ThreadArray[Indx].Req.Priority = JtmQ.Priority;
            // JTMThreadRegistry.ThreadArray[Indx].Req.BatchID = JtmQ.BatchID;
            JTMThreadRegistry.ThreadArray[Indx].Req.AtmNo = JtmQ.AtmNo;
            JTMThreadRegistry.ThreadArray[Indx].Req.BankID = JtmQ.BankID;
            JTMThreadRegistry.ThreadArray[Indx].Req.BranchNo = JtmQ.BranchNo;
            JTMThreadRegistry.ThreadArray[Indx].Req.ATMIPAddress = JtmQ.ATMIPAddress;
            JTMThreadRegistry.ThreadArray[Indx].Req.ATMMachineName = JtmQ.ATMMachineName;
            JTMThreadRegistry.ThreadArray[Indx].Req.ATMWindowsAuth = JtmQ.ATMWindowsAuth;
            JTMThreadRegistry.ThreadArray[Indx].Req.ATMAccessID = JtmQ.ATMAccessID;
            JTMThreadRegistry.ThreadArray[Indx].Req.ATMAccessPassword = JtmQ.ATMAccessPassword;
            JTMThreadRegistry.ThreadArray[Indx].Req.TypeOfJournal = JtmQ.TypeOfJournal;
            JTMThreadRegistry.ThreadArray[Indx].Req.SourceFileName = JtmQ.SourceFileName;
            JTMThreadRegistry.ThreadArray[Indx].Req.SourceFilePath = JtmQ.SourceFilePath;
            JTMThreadRegistry.ThreadArray[Indx].Req.DestnFileName = JtmQ.DestnFileName;
            JTMThreadRegistry.ThreadArray[Indx].Req.DestnFilePath = JtmQ.DestnFilePath;
            JTMThreadRegistry.ThreadArray[Indx].Req.DestnFileHASH = JtmQ.DestnFileHASH;
            JTMThreadRegistry.ThreadArray[Indx].Req.Stage = JtmQ.Stage;
            JTMThreadRegistry.ThreadArray[Indx].Req.ResultCode = JtmQ.ResultCode;
            JTMThreadRegistry.ThreadArray[Indx].Req.ResultMessage = JtmQ.ResultMessage;
            JTMThreadRegistry.ThreadArray[Indx].Req.FileUploadStart = JtmQ.FileUploadStart;
            JTMThreadRegistry.ThreadArray[Indx].Req.FileUploadEnd = JtmQ.FileUploadEnd;
            JTMThreadRegistry.ThreadArray[Indx].Req.FileParseStart = JtmQ.FileParseStart;
            JTMThreadRegistry.ThreadArray[Indx].Req.FileParseEnd = JtmQ.FileParseEnd;
            JTMThreadRegistry.ThreadArray[Indx].Req.ProcessEnd = JtmQ.ProcessEnd;
            JTMThreadRegistry.ThreadArray[Indx].Req.Operator = JtmQ.Operator;
        }

    }
}
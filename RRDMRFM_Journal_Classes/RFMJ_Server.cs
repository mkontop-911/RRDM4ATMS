using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;
using System.Security.Principal;
using System.Data;
using System.Data.SqlClient;
using RRDM4ATMs;
using System.Windows.Forms;

namespace RRDMRFM_Journal_Classes
{

    public class RfmjOperParams
    {
        public bool IsSuccess { get; set; }
        public string errorMsg { get; set; }

        // Startup directory 
        public string RfmjStartDir { get; set; }

        // File Locations
        public string RfmjFilePool { get; set; }
        public string RfmjArchivePool { get; set; }
        public string RfmjExceptionsPool { get; set; }
        public string RfmjFileNameMask { get; set; }

        // public string RfmjEventSource { get; set; }

        // Refresh Rates
        public int RfmjRefreshInterval { get; set; }

        // Thread Parameters
        public int RfmjMaxThreadNumber { get; set; }
        public int RfmjSleepWaitEmptyThreadSlot { get; set; }
        public int RfmjMaxThreadLifeSpan { get; set; }
        public int RfmjThreadAbortWait { get; set; }
        public int RfmjStartWorkerThreadTimeout { get; set; }
        public int RfmjThreadMonitorInterval { get; set; }

        public string RfmjSQLRelativeFilePoolPath { get; set; }
        public string RfmjStoredProcedure { get; set; }
    }

    public class RfmjServer
    {
        const string PROGRAM_MUTEX_NAME = "Global\\RRDMRFMJ";

        #region Static members
        // private static string instanceMutexName = "";

        // Server Operational Parameters
        public static RfmjOperParams RfmjOp { get; set; }

        public static string argOrigin = "";        // argument 1
        public static string argSourceFileID = "";  // argument 2
        public static string argOperator = "";      // argument 3

        // public static bool glbRawImport = false;    // derived from glbOrigin (if argOrigin==ATMs)

        public static bool glbArchiveEnabled = false;    // read from GAS Parameters
        public static int glbRFMJSleep;              // GAS parameters
        public static string glbStoredProc = "";    // GAS parameters; used if glbRawImport=true
        public static RRDMMatchingSourceFiles rsf;

        public static bool SignalToStop = false; // For handling [Ctrl+C] [Ctrl+Break] and Service STOP
        public static bool ResetCycle = false; // For cases when we do not want to wait for the timeout to expire when reading files from the pool


        // Timer to wait on for threads to abort
        public static volatile System.Timers.Timer AbortTimer;
        public static volatile bool AbortThreadTimerElapsed;

        // Abort Flag (set when controlled exit is requested)
        public static volatile bool Abort_Abort;

        // Directory where executable is in
        // public static string executableDirectory;
        #endregion

        #region Public Members
        public bool status { get; set; }
        public string statusMsg { get; set; }

        // Thread Registry
        public RfmjThreadRegistry ThrRegistry;

        // Thread object for the Server Main Thread
        public Thread Rfmj_ServerMainThread;
        #endregion

        #region Private Members
        // Thread object for the ThreadMonitor 
        private static Thread Rfmj_ThreadMonitor;

        // For use in logging events in the EventLog
        // public static volatile string RfmjEventSource = "RfmjServer"; // Updated when parameters are read

        #endregion

        #region RfmjServer Constructor
        public RfmjServer()
        {
            #region Only one instance alowed 
            bool IsTheOnlyInstance;

            try
            {
                Mutex InstanceMutex = new Mutex(true, PROGRAM_MUTEX_NAME, out IsTheOnlyInstance);
            }
            catch (UnauthorizedAccessException ex)
            {
                string msg = string.Format("MUTEX exception: {0}", ex.Message);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                IsTheOnlyInstance = false;
            }

            if (IsTheOnlyInstance == false)
            {
                string msg = string.Format("{0}\r\nAnother instance of this program is already running!\r\nOnly one instance is allowed to run at any time!",
                                           PROGRAM_MUTEX_NAME);
                if (Environment.UserInteractive)
                {
                    MessageBox.Show(msg, "RRDMRFMJ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                }
                status = false;
                statusMsg = msg;
                return;
            }
            #endregion

            #region Get AppSettings 
            RfmjOp = new RfmjOperParams();
            // RfmjOp.RfmjEventSource = "RRDMFileMonitor";

            // Get the directory where the exexuting assembly is
            //   1. If implemented as a service, the location where the service executebles and DLLs are located
            //   2. If implemented as interactive, the location of the program's executables and DLLs are located
            string dirName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            // executableDirectory = dirName;
            RfmjOp.RfmjStartDir = dirName;

            RfmjParameters Rfmj_Params = new RfmjParameters(argOperator);
            if (Rfmj_Params.IsSuccess == false)
            {
                status = RfmjOp.IsSuccess = false;
                statusMsg = RfmjOp.errorMsg = string.Format("Exception while getting program settings!\r\nThe exception message reads:\r\n{0}", Rfmj_Params.errorMsg);
                EventLogging.MessageOut(statusMsg, EventLogEntryType.Error);
                return;
            }
            else
            {
                // Log the starting parameters...
                string fstr = "RFMJ stared with the following parameters:\n" +
                              "MaxThreads:{0}\n" +
                              "SleepWaitEmptyThreadSlot:{1}ms\n" +
                              "ThreadStartTimeout:{2}ms\n" +
                              "ThreadMonitorInterval:{3}ms\n" +
                              "ThreadAbortWaitTime:{4}ms\n" +
                              "MaxThreadLifeSpan:{5}sec\n" +
                              "Stored Procedure for Parsing:{6}\n" +
                              "Refresh Interval:{7}\n" +
                              "FilePoolRoot Relative to SQL:{8}"
                              ;

                string msg1 = string.Format(fstr, Rfmj_Params.RFMJ_MaxThreadNumber,
                                          Rfmj_Params.RFMJ_SleepWaitEmptyThreadSlot,
                                          Rfmj_Params.RFMJ_StartWorkerThreadTimeout,
                                          Rfmj_Params.RFMJ_ThreadMonitorInterval,
                                          Rfmj_Params.RFMJ_ThreadAbortWait,
                                          Rfmj_Params.RFMJ_MaxThreadLifeSpan,
                                          Rfmj_Params.Rfmj_StoredProcedure,
                                          Rfmj_Params.RFMJ_RefreshInterval,
                                          Rfmj_Params.SQLRelativeFilePoolPath);
                EventLogging.MessageOut(msg1, EventLogEntryType.Information);
            }

            // Put parameters in RfmjOp
            // Worker Threads parameters
            RfmjOp.RfmjMaxThreadNumber = Rfmj_Params.RFMJ_MaxThreadNumber;
            RfmjOp.RfmjSleepWaitEmptyThreadSlot = Rfmj_Params.RFMJ_SleepWaitEmptyThreadSlot;
            RfmjOp.RfmjStartWorkerThreadTimeout = Rfmj_Params.RFMJ_StartWorkerThreadTimeout;
            RfmjOp.RfmjThreadMonitorInterval = Rfmj_Params.RFMJ_ThreadMonitorInterval;
            RfmjOp.RfmjThreadAbortWait = Rfmj_Params.RFMJ_ThreadAbortWait;
            RfmjOp.RfmjMaxThreadLifeSpan = Rfmj_Params.RFMJ_MaxThreadLifeSpan;
            RfmjOp.RfmjStoredProcedure = glbStoredProc = Rfmj_Params.Rfmj_StoredProcedure;
            RfmjOp.RfmjRefreshInterval = Rfmj_Params.RFMJ_RefreshInterval; // in seconds
            RfmjOp.RfmjSQLRelativeFilePoolPath = Rfmj_Params.SQLRelativeFilePoolPath;

            #region Read the record from MatchingSourceFiles and check if Source/Archive/Exceptions dirs exist           
            // Read the record from from RRDMMatchingSourceFiles 
            rsf = new RRDMMatchingSourceFiles();
            rsf.ReadSourceFileRecordByOriginAndFileID(argOrigin, argSourceFileID);
            if (rsf.ErrorFound || !rsf.RecordFound)
            {
                string msg = string.Format("Processing stopped!\nError reading FileID: {0}  of  Origin: {1} from the [MatchingSourceFiles] table! \nThe received error message is: \n{2}", argSourceFileID, argOrigin, rsf.ErrorOutput);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                status = RfmjOp.IsSuccess = false;
                statusMsg = RfmjOp.errorMsg = msg;
                return;
            }

            // Check if Source dir exist
            if (!Directory.Exists(rsf.SourceDirectory))
            {
                string msg = string.Format("Processing stopped!\nCould not locate the following directory:\n{0}", rsf.SourceDirectory);

                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                status = RfmjOp.IsSuccess = false;
                statusMsg = RfmjOp.errorMsg = msg;
                return;
            }
            RfmjOp.RfmjFilePool = rsf.SourceDirectory;
            RfmjOp.RfmjFileNameMask = rsf.FileNameMask;

            // Check if Archive dir exist
            if (!string.IsNullOrEmpty(rsf.ArchiveDirectory)) // if not present in GASParameters then no Archiving takes place
            {
                // Check if Archive dir exist
                if (!Directory.Exists(rsf.ArchiveDirectory))
                {
                    string msg = string.Format("Processing stopped!\nCould not locate the following directory:\n{0}", rsf.ArchiveDirectory);
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    status = RfmjOp.IsSuccess = false;
                    statusMsg = RfmjOp.errorMsg = msg;
                    return;
                }
                glbArchiveEnabled = true;
                RfmjOp.RfmjArchivePool = rsf.ArchiveDirectory;
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
                    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                    status = RfmjOp.IsSuccess = false;
                    statusMsg = RfmjOp.errorMsg = msg;
                    return;
                }
            }
            RfmjOp.RfmjExceptionsPool = rsf.ExceptionsDirectory;
            #endregion


            // ToDo: Validate parameters

            #endregion

            status = RfmjOp.IsSuccess = true;
            statusMsg = RfmjOp.errorMsg = "";
        }
        #endregion

        #region Server Launcher
        public Thread RfmjServerLauncher()
        {
            // Event Handle to wait on for thread to start 
            AutoResetEvent evServerStarted = new AutoResetEvent(false);

            // Start the Server thread
            ParameterizedThreadStart ts = new ParameterizedThreadStart(ServerMainThread);

            Thread oT = new Thread(ts);
            oT.Name = "RfmjServer";
            oT.Start(evServerStarted); // pass the event handler by which the thread will signal back...

            // Wait for the thread to signal back 
            // this is how we make sure that the thread initialized properly and is ready to go..
            bool ThrStarted = evServerStarted.WaitOne(10000); //ToDo: parameterize
            if (ThrStarted)
                return (oT);
            else
                return null;
        }
        #endregion

        #region ServerMainThread()
        private void ServerMainThread(object serverStartedEvent)
        {
            bool xpResult;
            string xpMsg;
            // Event Handle to wait on for the worker thread to start 
            AutoResetEvent hWorkerStartedEvent = new AutoResetEvent(false);

            #region Instantiate ThreadRegistry
            // Instantiate the class to manage the threads -- This must happen only once!!
            ThrRegistry = new RfmjThreadRegistry(RfmjOp.RfmjMaxThreadNumber);

            // Reset the Abort flag
            Abort_Abort = false;
            #endregion

            #region Start the Server Main Thread
            // Register ourselves
            Rfmj_ServerMainThread = Thread.CurrentThread;

            // Start the thread that will monitor the worker threads
            Rfmj_ThreadMonitor = StartThreadMonitor(ThrRegistry);

            // Signal back to the launcher that this thread has started
            ((AutoResetEvent)serverStartedEvent).Set();

            //Write to Log
            string argums = string.Format("ServerMainThread() started with: Origin={0}, SourceID={1}, Operator={2}", argOrigin, argSourceFileID, argOperator);
            EventLogging.RecordEventMsg(argums, EventLogEntryType.Information);
            #endregion

            #region Enable xp_cmdshell
            xpMsg = "Enabling xp_cmdshell";
            EventLogging.RecordEventMsg(xpMsg, EventLogEntryType.Information);
            xpResult = SqlXpCmdshell(true);
            #endregion

            #region Enter the server loop to process incoming files...
            try
            {
                #region While Loop
                while (Abort_Abort != true) // enter the loop if Abort_Abort not raised
                {
                    #region While Loop
                    int Indx = -1;
                    RfmjWorker oWThrClass;

                    string[] FileArray = GetArrayofFiles(RfmjOp.RfmjFilePool, RfmjOp.RfmjFileNameMask);
                    if (FileArray.Length > 1)
                    {
                        Array.Sort(FileArray); // propably not needed (use in case file system is not NTFS)
                    }

                    ResetCycle = false;

                    #region ForEach ...
                    foreach (string fullFileName in FileArray)
                    {
                        if (Abort_Abort) { break; }

                        string fileName = Path.GetFileName(fullFileName);

                        #region Check if there is already a thread processing the file 
                        if (RfmjThreadRegistry.IsFileBeingProcessed(fileName))
                        {
                            continue;
                        }
                        #endregion

                        #region Check if there is already a thread processing another journal for this ATM
                        string ATM_No = fileName.Substring(0, 8);
                        if (RfmjThreadRegistry.IsATM_BeingProcessed(ATM_No))
                        {
                            ResetCycle = true;
                            continue;
                        }
                        #endregion

                        #region Start Worker
                        // Get an empty slot in the ThreaArray
                        Indx = RfmjThreadRegistry.GetEmptySlot();
                        while (Indx == -1)
                        {
                            // Sleep for a while and loop until one is available
                            Thread.Sleep(RfmjOp.RfmjSleepWaitEmptyThreadSlot);
                            Indx = RfmjThreadRegistry.GetEmptySlot();
                        }

                        // Create a unique name for the thread
                        DateTime dt = DateTime.Now;
                        string tName = string.Format("RFMJ{0}", RfmjFunctions.GenerateNewID());

                        //#region debuging3
                        //using (StreamWriter sw = File.AppendText(debugpath))
                        //{
                        //    string dbgTxt = String.Format("Thread\t{0}\t{1}", Indx, fullFileName);
                        //    sw.WriteLine(dbgTxt);
                        //}
                        //#endregion

                        // Start the thread instance that will handle this action
                        oWThrClass = null;
                        oWThrClass = StartWorkerThread(Indx, tName, hWorkerStartedEvent, fullFileName);
                        if (oWThrClass != null)
                        {
                            // Raise the ClearToGo signal so that the thread carries on
                            oWThrClass.hClearToGoEvent.Set();
                        }
                        else
                        {
                            // TODO: Check if this code is reachable
                            string msg = "RfmjServer could not start a new thread and is aborting! Check for preceeding error messages to identify the cause..";
                            EventLogging.MessageOut(msg, EventLogEntryType.Error);
                            break;
                        }
                        #endregion
                    } // end foreach()
                    #endregion

                    // Wait until timeout expires before repeating this loop, unless ResetCycle is 'true'
                    if (ResetCycle == false)
                    {
                        // Split the RefreshInterval into 1sec sleep intervals; RefreshInterval is in seconds.
                        int sleepTime = 1000;
                        for (int k = 0; k < RfmjServer.RfmjOp.RfmjRefreshInterval; k++)
                        {
                            if (Abort_Abort)
                                break;
                            Thread.Sleep(sleepTime);
                        }
                    }
                    else
                    {
                        // Wait for all threads to terminate
                        int thrdAliveCount = RfmjThreadRegistry.RFMJ_MaxThreadCount;
                        while (thrdAliveCount != 0)
                        {
                            int sleepTime = 1000;
                            Thread.Sleep(sleepTime);
                            thrdAliveCount = RfmjThreadRegistry.GetActiveThreadNumber();
                        }
                    }
                    #endregion
                } // end While()
                #endregion
            }
            catch (RfmjFunctions.RfmjCustomException ex)
            {
                #region catch RfmjCustomException
                // ToDo: currently we treat all 'RfmjCustomException' as 'fatal'
                string msgfmt = "RFMJ exception! Terminating...\n" +
                                "RFMJ Source:{0}\n" +
                                "RFMJ Error code:{1}\n" +
                                "RFMJ message: {2}\n" +
                                "Original Message:\n{3}";
                string msg = string.Format(msgfmt, ex.cexSource, ex.cexCode, ex.cexMessage, ex.Message);
                EventLogging.MessageOut(msg, EventLogEntryType.Error);
                #endregion
            }
            catch (Exception ex)
            {
                #region catch Exception
                string msgfmt = "The program encountered an error!! Terminating...\n" +
                                "Error Source:{0}\n" +
                                "Error Message: {1}\n" +
                                "Stack: {2}\n" +
                                "Exception: {3}";
                string msg = string.Format(msgfmt, ex.Source, ex.Message, ex.StackTrace, ex.ToString());
                EventLogging.MessageOut(msg, EventLogEntryType.Error);
                #endregion
            }
            finally
            {
                #region finally
                // Raise the Abort_Abort flag
                RfmjThreadRegistry.RaiseAbortFlag();

                string msg = "RfmjServer received signal to terminate.\r\n" +
                             "The program will try to clean up all resources before terminating...\r\n";
                EventLogging.MessageOut(msg, EventLogEntryType.Information);

                // Wait for the ThreadMonitor thread to finish
                if (Rfmj_ThreadMonitor != null)
                {
                    Rfmj_ThreadMonitor.Join(); // wait for EBZCleanerThread to finish 
                }

                // Write to Log (Execution)
                // ToDo
                #endregion
            }
            #endregion

            #region Disable xp_cmdshell
            xpMsg = "Disabling xp_cmdshell";
            EventLogging.RecordEventMsg(xpMsg, EventLogEntryType.Information);
            xpResult = SqlXpCmdshell(false);
            #endregion
        }
        #endregion

        #region SqlXpCmdshell
        private bool SqlXpCmdshell(bool Enable)
        {
          
            bool retCode = true;
            bool AlecosInclude = false;
            if (AlecosInclude == true)
            {
                string RCT;
                string RCT_Start = "ATM_MT_Journals_AUDI.[dbo].[stp_Start_CMD_Shell]";
                string RCT_Stop = "ATM_MT_Journals_AUDI.[dbo].[stp_Stop_CMD_Shell]";
                if (Enable)
                {
                    RCT = RCT_Start;
                }
                else
                {
                    RCT = RCT_Stop;
                }
                string connectionString = ConfigurationManager.ConnectionStrings["ReconcRawImportConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (SqlConnection sqlconn = new SqlConnection(connectionString))
                        {
                            sqlconn.Open();
                            using (SqlCommand cmd = new SqlCommand(RCT, sqlconn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                // Parameters
                                cmd.CommandTimeout = 100;  // seconds
                                int rows = cmd.ExecuteNonQuery();
                            }
                            // Close connection
                            sqlconn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        retCode = false;
                        string msg = string.Format("Error executing Stored Procedure {0}\r\nException Details:\r\n{1}", RCT, ex.Message);
                        if (Environment.UserInteractive)
                        {
                            MessageBox.Show(msg, "RRDMRFMJ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        }
                    }
                }
            }
            
            return (retCode);
        }
        #endregion

        #region Get List of files in the pool
        private string[] GetArrayofFiles(string pool, string filemask)
        {
            string[] fileArr = new string[] { };

            try
            {
                fileArr = Directory.GetFiles(RfmjOp.RfmjFilePool, RfmjOp.RfmjFileNameMask);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    msg += ex1.Message;
                    ex1 = ex1.InnerException;
                }

                EventLogging.MessageOut(msg, EventLogEntryType.Error);

                RfmjFunctions.RfmjCustomException ex2 = new RfmjFunctions.RfmjCustomException(msg);
                ex2.cexCode = 3;
                ex2.cexMessage = "RfmjServer GetFiles() exception"; // + Environment.NewLine + msg;
                ex2.cexSource = "RfmjServer main thread";
                ex2.cexFatal = true;
                throw (ex2);
            }
            return fileArr;
        }

        /*
        catch (IOException ex) { }
        catch (DirectoryNotFoundException ex) { }
        catch (PathTooLongException ex) { }
        catch (UnauthorizedAccessException ex) { }
        catch (ArgumentException ex) { }
        catch (ArgumentNullException ex) { }
        catch (ArgumentOutOfRangeException ex) { }
        */
        #endregion

        #region StartWorkerThread()
        public RfmjWorker StartWorkerThread(int Index, string ThrName, AutoResetEvent hCallerWaitEvent, string fullFileName)
        {
            string msg;
            RfmjWorker instQJ = null;

            // Instantiate a new Woker class (The constructor will set its ThreadSlot private member to Index)
            RfmjWorker oWorker = new RfmjWorker(Index, fullFileName);

            // Start a new thread.. the object passed as a parameter is the AutoResetEvent 
            // on which the thread will signal back that it has started with success
            ParameterizedThreadStart pts = new ParameterizedThreadStart(oWorker.RfmjWorkerThread);
            Thread oT = new Thread(pts);
            oT.Name = ThrName;

            oT.Start(hCallerWaitEvent);

            // Wait for the thread to signal back 
            // this is how we make sure that the thread initialized properly and is ready to go..
            bool ThrStarted = hCallerWaitEvent.WaitOne(RfmjOp.RfmjStartWorkerThreadTimeout);
            if (ThrStarted)
            {
                // started within the time interval defined by StartWorkerThreadTimeout
                instQJ = oWorker;
            }
            else
            {
                instQJ = null;
                // The thread did not signal back...
                // Failed to start thread in the specified time interval
                msg = string.Format("Failed to start thread in the specified time interval of {0} milliseconds!", RfmjOp.RfmjStartWorkerThreadTimeout);
                EventLogging.MessageOut(msg, EventLogEntryType.Error);

                // This is a fatal error and the caller must handle the exception and signal for termination....
                RfmjFunctions.RfmjCustomException ex = new RfmjFunctions.RfmjCustomException(msg);
                ex.cexCode = 2;
                ex.cexMessage = "Failed to start a new worker thread!";
                ex.cexSource = "StartWorkerThread";
                ex.cexFatal = true;
                throw (ex);
                // The exception will be propagated upstream and will be treaded as fatal!!!!
            }
            return (instQJ);
        }
        #endregion

        #region StartThreadMonitor()
        public Thread StartThreadMonitor(RfmjThreadRegistry ThR)
        {
            Thread oT = null;
            //rc = false;
            if (ThR != null)
            {
                ThreadStart ts = new ThreadStart(ThreadMonitor);
                oT = new Thread(ts);
                oT.Name = "RFMJThreadMonitor";

                oT.Start();
            }
            return (oT);
        }
        #endregion

        #region ThreadMonitor
        /// <summary> Thread to monitor for ABORT and cleanups 
        /// 
        /// <remarks>
        /// 1. Re-initializes the ThreadArray slot of dead threads
        /// 2. Checks for the Abort_Abort flag 
        ///    a. Checks if Worker threads finished within the MaxLifeSpan time interval and kills them if still alive
        ///    b. Stops the ConsoleDisplay thread if alive
        /// </remarks>
        /// </summary>
        public void ThreadMonitor()
        {
            long DtTmTicks = 0;
            long ThreadAge = 0; // in ticks
            long LifeSpanInTicks = RfmjOp.RfmjMaxThreadLifeSpan * 10000; // MaxLifeSpan is in ms, 1ms = 10000ticks

            Rfmj_ThreadMonitor = Thread.CurrentThread;
            while (true)
            {
                // Check if Abort flag is raised!
                if (Abort_Abort == true)
                {
                    AbortAllWorkerThreads();
                    return;
                }

                DtTmTicks = DateTime.Now.Ticks;

                for (int i = 0; i < RfmjOp.RfmjMaxThreadNumber; i++)
                {
                    Thread oT = RfmjThreadRegistry.ThreadArray[i].oThread;
                    if (oT != null)
                    {
                        if (!oT.IsAlive)
                        {
                            // Thread is not alive, initialize the slot 
                            oT = null;
                            RfmjThreadRegistry.ThreadArray[i].workSheet = default(RfmjThreadWorkSheet);
                            RfmjThreadRegistry.ThreadArray[i].Status = StatusOfThread.Available;
                            RfmjThreadRegistry.ThreadArray[i].StartTime = DateTime.MinValue;
                            RfmjThreadRegistry.ThreadArray[i].oThread = null;
                        }
                        else
                        {
                            // Thread IsAlive, so check for its status and age
                            switch (RfmjThreadRegistry.ThreadArray[i].Status)
                            {
                                case StatusOfThread.Finished:
                                case StatusOfThread.Canceled:
                                case StatusOfThread.Stopping:
                                case StatusOfThread.Aborting:
                                    {
                                        // ThreadAge = DtTmTicks - eBizThreadRegistry.ThreadArray[i].StartTime;
                                        ThreadAge = DtTmTicks - RfmjThreadRegistry.ThreadArray[i].StartTime.Ticks;
                                        // Lived more than its allowed LifeSpan?
                                        if (ThreadAge > LifeSpanInTicks)
                                        {
                                            // Abort() will bring the thread into the !IsAlive state and
                                            // a next iteration will clear its slot
                                            oT.Abort();
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        // do nothing
                                        break;
                                    }
                            }
                        }
                    }
                }
                Thread.Sleep(RfmjOp.RfmjThreadMonitorInterval);
            }
        }

        /// <summary> void AbortAllWorkerThreads() 
        /// 
        /// <remarks>
        /// Wait a speciied time interval for threads in progress to stop ...
        /// Kill the remaining
        /// </remarks>
        /// </summary>
        public void AbortAllWorkerThreads()
        {
            // Raise the Abort_Abort flag ... just in case
            RfmjThreadRegistry.RaiseAbortFlag();

            // Create the timer to wait on for threads to abort before returning to the caller...
            AbortTimer = new System.Timers.Timer(RfmjOp.RfmjThreadAbortWait);
            AbortTimer.Elapsed += OnAbortTimerEvent;
            AbortTimer.AutoReset = false;
            AbortTimer.Enabled = true;

            // Wait for threads to stop or timer to to elapse
            AbortThreadTimerElapsed = false;

            int AliveCount = RfmjOp.RfmjMaxThreadNumber;
            while (AliveCount != 0 && AbortThreadTimerElapsed == false)
            {
                AliveCount = 0;
                for (int i = 0; i < RfmjOp.RfmjMaxThreadNumber; i++)
                {
                    Thread oT = RfmjThreadRegistry.ThreadArray[i].oThread;
                    if (oT != null)
                    {
                        if (oT.IsAlive)
                        {
                            AliveCount++;
                        }
                    }
                }

                if (AliveCount == 0)
                    break;

                Thread.Sleep(1);
            }

            // Check if there are threads that are stil running 
            // If so, kill them. (this chunk run only if timer elapsed)
            if (AbortThreadTimerElapsed)
            {
                for (int i = 0; i < RfmjOp.RfmjMaxThreadNumber; i++)
                {
                    if (RfmjThreadRegistry.ThreadArray[i].oThread != null)
                    {
                        if (RfmjThreadRegistry.ThreadArray[i].oThread.IsAlive)
                        {
                            RfmjThreadRegistry.ThreadArray[i].oThread.Abort();
                        }
                    }
                }
            }

            AbortTimer.Stop();
            AbortTimer.Dispose();
            return;
        }

        private void OnAbortTimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            AbortThreadTimerElapsed = true;
        }
        #endregion
    }
}
using System;
using System.Threading;
using System.Diagnostics;

using RRDM4ATMs;


namespace RRDMJTMClasses
{
    /// <summary> Enumeration of the states a worker thread can be in
    /// </summary>
    public enum StatusOfThread
    {
        Unknown,    // Reserved for queries with invalid Index
        Available,  // Constructor sets the array slots to this
        Reserved,   // Set when allocating a free slot in ThreadArray
        Running,
        Stopping,   // Set for normal termination
        Canceled,   // Set when thread stops because of Abort_Abort
        Aborting,   // Set after an exception
        Finished
    }

    /// <summary> ThreadRec struct 
    /// <remarks>
    /// ThreadRec: structure containing information for the operation and management of threads 
    /// </remarks>
    /// </summary>
    public struct ThreadRec // Structure containing information for the operation and management of a thread 
    {
        public Thread oThread;
        public long StartTime; // in Ticks, to calculate the threads LifeSpan
        public StatusOfThread Status;
        public JTMQueueStruct Req; // The request (queue) record in the form of a struct. Declared in RRDMJTMQueue.cs
        public RRDMJTMIdentificationDetailsClass IdentClass; // the class itself, declared in RRDMJTMIdentificationDetailsClass.cs
    }

    public class JTMThreadRegistry
    {
        // JTM Parameters, Initialized by the constructor at instantiation
        public static int JTM_MaxThreadCount;
        public static int JTM_FETCH_RetryWaitTime; // sec
        public static int JTM_FETCH_Retries;
        public static int JTM_SleepWaitNewRequest; // ms
        public static int JTM_SleepWaitEmptyThreadSlot; //ms
        public static int JTM_StartWorkerThreadTimeout; //ms
        public static int JTM_ThreadMonitorInterval; // ms
        public static int JTM_ThreadAbortWait; //ms
        public static int JTM_MaxThreadLifeSpan; // sec
        public static string JTM_FilePoolRoot;
        public static string JTM_ArchiveRoot;
        public static string JTM_ParserSP;
        public static int JTM_MaxJournalBackups;
        public static string JTM_PSExecPath;
        public static string JTM_InitEJPath;
        public static string JTM_SQLRelativeFilePoolPath;

        // Thread object for the JTMServerThread
        public static volatile Thread oJTMServerThread;

        // Thread object for the JTMThreadMonitor 
        public static volatile Thread oJTMThreadMonitor;

        // Thread object for the JTMConsoleDisplay thread  
        public static volatile Thread oJTMConsoleDisplayThread;

        // Timer to wait on for threads to abort
        public static System.Timers.Timer AbortTimer;
        public static volatile bool AbortThreadTimerElapsed;

        /// <summary> static ThreadRec[] ThreadArray;
        /// 
        /// <remarks>
        /// Array of thread records (globally visible)
        /// Every thread gets a slot (ThreadRec) in this array for the duration of its life.
        /// Serialization of access to this array is through the ThreadLock object
        /// </remarks>
        /// </summary>
        public static volatile ThreadRec[] ThreadArray;

        // For use in logging events in the EventLog
        public const string JTMEventSource = "RRDMJTM";

        /// <summary> ThreadArrayLock
        /// 
        /// <remarks>
        /// To serialize access of threads to the ThreadSlot array
        /// </remarks>
        /// </summary>
        public static volatile object ThreadArrayLock;

        /// <summary> SQLLockSP  
        /// 
        /// <remarks>
        /// Serializes access to the SQL Stored procedure
        /// </remarks>
        /// </summary>
        public static volatile object SQLLockSP;

        /// <summary> ConsoleDisplayLock 
        /// 
        /// <remarks>
        /// Serializes access to the file contining the info for display by the ConsoleDisplay thread
        /// </remarks>
        /// </summary>
        public static volatile object ConsoleDisplayLock;

        // Abort Flag (set when controlled exit is requested)
        public static volatile bool Abort_Abort;

        /// <summary> JTMThreadRegistry Contructor
        /// The JTMThreadRegistry class is instantiated only once!
        /// The constructor initializes (size and contents) the ThreadSlot array of structures and the operational parameters
        /// </summary>
        /// <param name="ThreadCount"></param>
        public JTMThreadRegistry(int ThreadCount,
                                 int FETCH_Retries,
                                 int FETCH_RetryTimeout,
                                 int SleepWaitNewRequest,
                                 int SleepWaitEmptyThreadSlot,
                                 int StartThreadTimeout,
                                 int ThreadMonitorInterval,
                                 int ThreadAbortWait,
                                 int MaxThreadLifeSpan,
                                 string FilePoolRoot,
                                 string ArchiveRoot,
                                 int MaxJournalBackups,
                                 string ParserSP,
                                 string JTMPSExecPath,
                                 string JTMInitEJPath,
                                 string SQLRelativeFilePoolPath)
        {
            // Assign values to operational parameters
            JTM_MaxThreadCount = ThreadCount;
            JTM_FETCH_Retries = FETCH_Retries;
            JTM_FETCH_RetryWaitTime = FETCH_RetryTimeout;
            JTM_SleepWaitNewRequest = SleepWaitNewRequest;
            JTM_SleepWaitEmptyThreadSlot = SleepWaitEmptyThreadSlot;
            JTM_StartWorkerThreadTimeout = StartThreadTimeout;
            JTM_ThreadMonitorInterval = ThreadMonitorInterval;
            JTM_ThreadAbortWait = ThreadAbortWait;
            JTM_MaxThreadLifeSpan = MaxThreadLifeSpan;
            JTM_FilePoolRoot = FilePoolRoot;
            JTM_ArchiveRoot = ArchiveRoot;
            JTM_MaxJournalBackups = MaxJournalBackups;
            JTM_ParserSP = ParserSP;
            JTM_PSExecPath = JTMPSExecPath;
            JTM_InitEJPath = JTMInitEJPath;
            JTM_SQLRelativeFilePoolPath = SQLRelativeFilePoolPath;

            // Create the ThreadArray
            ThreadArray = new ThreadRec[ThreadCount]; // as many slots as ThreadCount
            for (int i = 0; i < ThreadCount; i++)
            {
                ThreadArray[i].Req = new JTMQueueStruct();
                // ThreadArray[i].IdentClass // a new instance is created when needed
                ThreadArray[i].Status = StatusOfThread.Available;
            }

            // create the serialization objects
            ThreadArrayLock = new object();
            SQLLockSP = new object();
            ConsoleDisplayLock = new object();

            // thread handles; will be set when the threads are created
            oJTMServerThread = null;
            oJTMThreadMonitor = null;
            oJTMConsoleDisplayThread = null;

            // raised for the controlled termination of the threads and the program
            Abort_Abort = false;
        }

        /// <summary>  GetEmptySlot()
        /// 
        /// <remarks>
        /// Returns the Index of an available slot in the ThreadArray of structures
        /// The ThreadLock object guarantees that only one instance of this code is running at any given moment
        /// The array elements are initialized and the slot's State is set to Reserved before returning.
        /// </remarks>
        /// </summary>
        /// <returns>
        /// int
        /// Index to the array slot, -1 if no slot is available
        /// </returns>
        public static int GetEmptySlot()
        {
            lock (ThreadArrayLock) // serialize access
            {
                int Index = -1;
                for (int i = 0; i < JTM_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].Status == StatusOfThread.Available || ThreadArray[i].Status == StatusOfThread.Finished)
                    {
                        // Initialize the slot
                        ThreadArray[i].Req = default(JTMQueueStruct);
                        ThreadArray[i].Status = StatusOfThread.Reserved;
                        Index = i;
                        break;
                    }
                }
                return (Index);
            }
        }


        public static bool ATM_HasRequestInProgress(string AtmNo)
        {
            lock (ThreadArrayLock) // serialize access
            {
                bool ret = false;

                for (int i = 0; i < JTM_MaxThreadCount; i++)
                {
                    //if (ThreadArray[i].Status == StatusOfThread.Available || ThreadArray[i].Status == StatusOfThread.Finished)
                    if (ThreadArray[i].Req.AtmNo == AtmNo)
                    {
                        if (ThreadArray[i].Status != StatusOfThread.Available)
                            ret = true;
                        break;
                    }
                }
                return (ret);
            }
        }


        /// <summary> ChangeThreadStatus
        /// Change the status of the thread in the ThreadArray
        /// </summary>
        /// <param name="Index">
        /// Array slot
        /// </param>
        /// <param name="NewStatus">
        /// Use values from 'StatusOfThread' enumeration
        /// </param>
        /// <returns>
        /// int 
        /// </returns>
        public static bool ChangeThreadStatus(int Index, StatusOfThread NewStatus)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < JTM_MaxThreadCount)
                {
                    ThreadArray[Index].Status = NewStatus;
                    success = true;
                }
                else
                {
                    success = false;
                }
                return (success);
            }
        }

        public static void RaiseAbortFlag()
        {
            lock (ThreadArrayLock)
            {
                Abort_Abort = true;
            }
            return;
        }

        /// <summary> SetThreadObjHandle
        /// Set the oThread and StartTime elements of the specified slot in ThreadArray
        /// </summary>
        /// <param name="Index">
        /// Array slot
        /// </param>
        /// <param name="oT">
        /// Thread object
        /// </param>
        /// <returns>
        /// </returns>
        public static bool SetThreadObjHandle(int Index, Thread oT)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < JTM_MaxThreadCount)
                {
                    DateTime dt = DateTime.Now;
                    ThreadArray[Index].oThread = oT;
                    ThreadArray[Index].StartTime = dt.Ticks;
                    success = true;
                }
                else
                {
                    success = false;
                }
                return (success);
            }
        }

        #region JTMThreadMonitor

        private static void OnAbortTimerEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            AbortThreadTimerElapsed = true;
        }

        /// <summary> Thread to monitor for ABORT and cleanups 
        /// 
        /// <remarks>
        /// 1. Re-initializes the ThreadArray slot of dead threads
        /// 2. Checks for the Abort_Abort flag 
        ///    a. Waits for WorkerThreads to finish within the MaxLifeSpan time interval and kills them if still alive
        ///    b. Stops the ConsoleDisplay thread if alive
        /// </remarks>
        /// </summary>
        public void JTMThreadMonitor()
        {
            long DtTmTicks = 0;
            long ThreadAge = 0; // in ticks
            long LifeSpanInTicks = JTM_MaxThreadLifeSpan * 10000000; // MaxLifeSpan is in seconds, 1sec = 1000ms, 1ms = 10000ticks

            JTMThreadRegistry.oJTMThreadMonitor = Thread.CurrentThread;
            while (true)
            {
                // Check if Abort flag is raised!
                if (JTMThreadRegistry.Abort_Abort == true)
                {
                    AbortAllWorkerThreads();

                    // Kill the ConsoleDisplayThread, if alive;
                    if (JTMThreadRegistry.oJTMConsoleDisplayThread != null)
                        if (JTMThreadRegistry.oJTMConsoleDisplayThread.IsAlive)
                            JTMThreadRegistry.oJTMConsoleDisplayThread.Abort();

                    return;
                }

                DtTmTicks = DateTime.Now.Ticks;

                for (int i = 0; i < JTM_MaxThreadCount; i++)
                {
                    Thread oT = ThreadArray[i].oThread;
                    if (oT != null)
                    {
                        if (!oT.IsAlive)
                        {
                            // Thread is not alive, initialize the slot 
                            oT = null;
                            ThreadArray[i].Req = default(JTMQueueStruct);
                            ThreadArray[i].IdentClass = default(RRDMJTMIdentificationDetailsClass);
                            ThreadArray[i].Status = StatusOfThread.Available;
                            ThreadArray[i].StartTime = 0L;
                        }
                        else
                        {
                            // Thread IsAlive, so check for its status and age
                            switch (ThreadArray[i].Status)
                            {
                                case StatusOfThread.Finished:
                                case StatusOfThread.Canceled:
                                case StatusOfThread.Stopping:
                                case StatusOfThread.Aborting:
                                    {
                                        ThreadAge = DtTmTicks - ThreadArray[i].StartTime;
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
                Thread.Sleep(JTMThreadRegistry.JTM_ThreadMonitorInterval);
            }
        }

        /// <summary> void AbortAllWorkerThreads() 
        /// 
        /// <remarks>
        /// Wait for threads in progress to stop for a speciied amount of time
        /// Kill the remaining
        /// </remarks>
        /// </summary>
        public static void AbortAllWorkerThreads()
        {
            // JTMThreadRegistry.RaiseAbortFlag();
            string msg = "\nEntered AbortAllWorkerThreads ....";
            EventLogging.MessageOut(JTMEventSource, msg, EventLogEntryType.Information);

            // Create the timer to wait on for threads to abort before returning to the caller...
            AbortTimer = new System.Timers.Timer(JTM_ThreadAbortWait);
            AbortTimer.Elapsed += OnAbortTimerEvent;
            AbortTimer.AutoReset = false;
            AbortTimer.Enabled = true;

            // Wait for threads to stop or timer to timeout to elapse
            AbortThreadTimerElapsed = false;
            AbortTimer.Enabled = true;

            int AliveCount = JTM_MaxThreadCount;
            while (AliveCount != 0 && AbortThreadTimerElapsed != true)
            {
                AliveCount = 0;
                for (int i = 0; i < JTM_MaxThreadCount; i++)
                {
                    Thread oT = ThreadArray[i].oThread;
                    if (oT != null)
                    {
                        if (oT.IsAlive)
                            AliveCount++;
                    }
                }
                GC.KeepAlive(AbortTimer); // to prevent garbage collection from occurring before the method ends. 

                DisplayThreadInfo(JTMThreadRegistry.JTM_MaxThreadCount);
                Thread.Sleep(1);
            }

            // Check if there are threads that are stil running 
            // If so, kill them.
            for (int i = 0; i < JTM_MaxThreadCount; i++)
            {
                if (ThreadArray[i].oThread != null)
                {
                    if (ThreadArray[i].oThread.IsAlive)
                    {
                        ThreadArray[i].oThread.Abort();
                    }
                }
            }

            AbortTimer.Dispose();
            return;
        }

        #endregion

        #region JTMConsoleDisplay

        /// <summary>
        /// JTMConsoleDisplay
        /// <remarks>
        /// In the Console version of the program, EventLogging.MessageOut writes out information into a file, 
        /// which this thread reads, displays and then deletes.
        /// </remarks>
        /// </summary>
        public void JTMConsoleDisplay()
        {
            Thread thisThread = Thread.CurrentThread;
            JTMThreadRegistry.oJTMConsoleDisplayThread = thisThread;

            while (true)
            {
                if (JTMThreadRegistry.ConsoleDisplayLock != null) // called before instantiation of ThreadRegistry
                {
                    lock (JTMThreadRegistry.ConsoleDisplayLock)
                    {
                        EventLogging.DisplayFileContents();
                    }
                }
                lock (ThreadArrayLock)
                {
                    DisplayThreadInfo(JTMThreadRegistry.JTM_MaxThreadCount);
                }

                // TODO consider using different wait interval
                Thread.Sleep(JTMThreadRegistry.JTM_ThreadMonitorInterval);
            }
        }

        public static void DisplayThreadInfo(int JTM_MaxThreadNumber)
        {
            string sOut = "";
            string stg = "";
            string Alive = "";
            if (Environment.UserInteractive)
            {
                // Console.WriteLine("\nWaiting for new requests!");
                Console.WriteLine("\n    Slot MsgID    IsAlive Status     ATM     Command   Stage");
                Console.WriteLine("    ---- -------- ------- ---------- ------- --------- ----------------");
                for (int j = 0; j < JTM_MaxThreadNumber; j++)
                {
                    if (JTMThreadRegistry.ThreadArray[j].oThread != null)
                    {
                        stg = JTMQueueStage.getStageFromNumber(JTMThreadRegistry.ThreadArray[j].Req.Stage);
                        Alive = ((JTMThreadRegistry.ThreadArray[j].oThread.IsAlive) ? "  Yes  " : "  No   ");
                        sOut = string.Format("    {0, 4} {1} {2, -7} {3, -10} {4, -7} {5, -9} {6}",
                                            j.ToString(),
                                            JTMThreadRegistry.ThreadArray[j].Req.MsgID.ToString("00000000"),
                                            Alive,
                                            JTMThreadRegistry.ThreadArray[j].Status,
                                            JTMThreadRegistry.ThreadArray[j].Req.AtmNo,
                                            JTMThreadRegistry.ThreadArray[j].Req.Command,
                                            (JTMThreadRegistry.ThreadArray[j].oThread.IsAlive) ? stg : "");
                    }
                    else
                    {
                        stg = "";
                        Alive = "  No   ";
                        sOut = string.Format("    {0, 4} {1} {2, -7} {3, -10} {4, -7} {5, -9} {6}",
                                            j.ToString(),
                                            JTMThreadRegistry.ThreadArray[j].Req.MsgID.ToString("00000000"),
                                            Alive,
                                            JTMThreadRegistry.ThreadArray[j].Status,
                                            JTMThreadRegistry.ThreadArray[j].Req.AtmNo,
                                            JTMThreadRegistry.ThreadArray[j].Req.Command,
                                            stg);
                    }
                    Console.WriteLine(sOut);
                }
            }
        }
        #endregion
    }

}

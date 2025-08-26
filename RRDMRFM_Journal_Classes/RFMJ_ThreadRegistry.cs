using System;
using System.IO;
using System.Threading;

namespace RRDMRFM_Journal_Classes
{
    #region RfmjThread work sheet (structure)
    public struct RfmjThreadWorkSheet // structure to contain individual thread accessories
    {
        public volatile string AtmNo; 
        public volatile int Stage;
        public volatile int ResultCode;
        public volatile string ResultMessage;
        public volatile string SourceFileName;
        public volatile string SourceFileDir;
        public volatile string HashValue;
        public DateTime ProcessStart;
        public string Step_1_Descr;
        public DateTime Step_1_Start;
        public DateTime Step_1_End;
        public string Step_2_Descr;
        public DateTime Step_2_Start;
        public DateTime Step_2_End;
        public string Step_3_Descr;
        public DateTime Step_3_Start;
        public DateTime Step_3_End;
        public string Step_4_Descr;
        public DateTime Step_4_Start;
        public DateTime Step_4_End;
        public DateTime ProcessEnd;
        // Consider:
        // Stored procedure name
        // ATM type
        // ....
    }
    #endregion

    #region ThreadRec struct 
    public struct ThreadRec // Structure containing information for the operation and management of a thread 
    {
        public Thread oThread;
        public DateTime StartTime;
        public StatusOfThread Status;
        public RfmjThreadWorkSheet workSheet;
    }
    #endregion

    public class RfmjWorkerSheets // for console UI
    {
        public volatile int maxWorkerThreads;
        public volatile ThreadRec[] threadRec;
    }

    public class RfmjThreadRegistry
    {
        // RFMJ Operational Parameters, Initialized by the constructor at instantiation
        public static volatile int RFMJ_MaxThreadCount;

        // Array of thread records. Every thread gets a slot (ThreadRec) in this array for the duration of its life.
        // Serialization of access to this array is through the ThreadLock object
        public static volatile ThreadRec[] ThreadArray;

        // ThreadArrayLock, Used to serialize access of threads to the ThreadSlot array
        public static volatile object ThreadArrayLock;


        #region RfmjThreadRegistry Constructor
        // The RfmjThreadRegistry class is instantiated only once!
        // The constructor initializes (size and contents) the ThreadSlot array of structures and the operational parameters
        public RfmjThreadRegistry(int ThreadCount)
        {
            // Assign values to operational parameters
            RFMJ_MaxThreadCount = ThreadCount;

            // Create the ThreadArray
            ThreadArray = new ThreadRec[ThreadCount]; // as many slots as ThreadCount
            for (int i = 0; i < ThreadCount; i++)
            {
                ThreadArray[i].workSheet = new RfmjThreadWorkSheet();
                ThreadArray[i].Status = StatusOfThread.Available;
                ThreadArray[i].oThread = null;
            }

            // create the serialization objects
            ThreadArrayLock = new object();
        }
        #endregion

        #region GetEmptySlot()
        // Returns the Index of an available slot in the ThreadArray of structures
        // The ThreadLock object guarantees that only one instance of this code is running at any given moment
        // The array elements are initialized and the slot's State is set to Reserved before returning.
        // Returns: Index to the array slot, -1 if no slot is available
        /// </returns>
        public static int GetEmptySlot()
        {
            lock (ThreadArrayLock) // serialize access
            {
                int Index = -1;
                for (int i = 0; i <RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].Status == StatusOfThread.Available || ThreadArray[i].Status == StatusOfThread.Finished)
                    {
                        // Initialize the slot
                        ThreadArray[i].workSheet = default(RfmjThreadWorkSheet);
                        ThreadArray[i].Status = StatusOfThread.Reserved;
                        Index = i;
                        break;
                    }
                }
                return (Index);
            }
        }
        #endregion

        #region ChangeThreadStatus()
        // Change the status of the thread in the ThreadArray
        public static bool ChangeThreadStatus(int Index, StatusOfThread NewStatus)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < RFMJ_MaxThreadCount)
                {
                    ThreadArray[Index].Status = NewStatus;
                    success = true;
                }
                return (success);
            }
        }
        #endregion

        #region SetThreadObjHandle()
        // Set the oThread and StartTime elements of the specified slot in ThreadArray
        public static bool SetThreadObjHandle(int Index, Thread oT)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < RFMJ_MaxThreadCount)
                {
                    // DateTime dt = DateTime.Now;
                    ThreadArray[Index].oThread = oT;
                    ThreadArray[Index].StartTime = DateTime.Now;  // dt.Ticks;
                    success = true;
                }
                return (success);
            }
        }
        #endregion

        #region SetHASHValue()
        // Set the HASH Value of the file being processed
        public static bool SetHASHValue(int Index, string value)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < RFMJ_MaxThreadCount)
                {
                    // DateTime dt = DateTime.Now;
                    ThreadArray[Index].workSheet.HashValue = value;
                    success = true;
                }
                return (success);
            }
        }
        #endregion

        #region SetAtmNumber()
        // Set the ATM being processed
        public static bool SetAtmNumber(int Index, string value)
        {
            lock (ThreadArrayLock)
            {
                bool success = false;
                if (Index >= 0 && Index < RFMJ_MaxThreadCount)
                {
                    // DateTime dt = DateTime.Now;
                    ThreadArray[Index].workSheet.AtmNo = value;
                    success = true;
                }
                return (success);
            }
        }
        #endregion

        #region // SetSourceFileInThreadWorksheet
        //public static bool SetSourceFileInThreadWorksheet(int Indx, string fullFileName)
        //{
        //    lock (ThreadArrayLock)
        //    {
        //        bool success = false;
        //        if (Indx >= 0 && Indx < RFMJ_MaxThreadCount)
        //        {
        //            ThreadArray[Indx].workSheet.SourceFileName = Path.GetFileName(fullFileName);
        //            ThreadArray[Indx].workSheet.SourceFileDir = Path.GetDirectoryName(fullFileName);
        //            success = true;
        //        }
        //        return (success);
        //    }
        //}
        #endregion

        #region Get Worker Sheets
        // Get the Worker Sheets
        public static RfmjWorkerSheets GetWorkerSheets()
        {
            lock (ThreadArrayLock)
            {
                RfmjWorkerSheets sheets = new RfmjWorkerSheets();
                sheets.maxWorkerThreads = RFMJ_MaxThreadCount;
                sheets.threadRec = new ThreadRec[RFMJ_MaxThreadCount];
                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    sheets.threadRec[i].oThread = null;
                    if (ThreadArray[i].oThread != null)
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            sheets.threadRec[i].oThread = ThreadArray[i].oThread;
                        }
                    sheets.threadRec[i].StartTime = ThreadArray[i].StartTime;
                    sheets.threadRec[i].Status = ThreadArray[i].Status;
                    sheets.threadRec[i].workSheet.AtmNo = ThreadArray[i].workSheet.AtmNo;
                    sheets.threadRec[i].workSheet.ProcessEnd = ThreadArray[i].workSheet.ProcessEnd;
                    sheets.threadRec[i].workSheet.ProcessStart = ThreadArray[i].workSheet.ProcessStart;
                    sheets.threadRec[i].workSheet.ResultCode = ThreadArray[i].workSheet.ResultCode;
                    sheets.threadRec[i].workSheet.ResultMessage = ThreadArray[i].workSheet.ResultMessage;
                    sheets.threadRec[i].workSheet.SourceFileName = ThreadArray[i].workSheet.SourceFileName;
                    sheets.threadRec[i].workSheet.SourceFileDir = ThreadArray[i].workSheet.SourceFileDir;
                    sheets.threadRec[i].workSheet.HashValue = ThreadArray[i].workSheet.HashValue;
                    sheets.threadRec[i].workSheet.Stage = ThreadArray[i].workSheet.Stage;
                    sheets.threadRec[i].workSheet.Step_1_Descr = ThreadArray[i].workSheet.Step_1_Descr;
                    sheets.threadRec[i].workSheet.Step_1_Start = ThreadArray[i].workSheet.Step_1_Start;
                    sheets.threadRec[i].workSheet.Step_1_End = ThreadArray[i].workSheet.Step_1_End;
                    sheets.threadRec[i].workSheet.Step_2_Descr = ThreadArray[i].workSheet.Step_2_Descr;
                    sheets.threadRec[i].workSheet.Step_2_Start = ThreadArray[i].workSheet.Step_2_Start;
                    sheets.threadRec[i].workSheet.Step_2_End = ThreadArray[i].workSheet.Step_2_End;
                    sheets.threadRec[i].workSheet.Step_3_Descr = ThreadArray[i].workSheet.Step_3_Descr;
                    sheets.threadRec[i].workSheet.Step_3_Start = ThreadArray[i].workSheet.Step_3_Start;
                    sheets.threadRec[i].workSheet.Step_3_End = ThreadArray[i].workSheet.Step_3_End;
                    sheets.threadRec[i].workSheet.Step_4_Descr = ThreadArray[i].workSheet.Step_4_Descr;
                    sheets.threadRec[i].workSheet.Step_4_Start = ThreadArray[i].workSheet.Step_4_Start;
                    sheets.threadRec[i].workSheet.Step_4_End = ThreadArray[i].workSheet.Step_4_End;
                }
                return (sheets);
            }
        }
        #endregion

        #region Get the number of Alive Threads
        public int GetAliveThreadCount()
        {
            int AliveThreadCount = 0;
            lock (ThreadArrayLock)
            {
                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].oThread != null)
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            AliveThreadCount++;
                        }
                }
                return (AliveThreadCount);
            }
        }
        #endregion

        #region IsFileBeingProcessed
        // Check if there is a thread handling a specific file
        public static bool IsFileBeingProcessed(string fileName)
        {
            lock (ThreadArrayLock)
            {
                bool isBeingProcessed = false;

                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].oThread != null)
                    {
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            if ((ThreadArray[i].Status != StatusOfThread.Available) || (ThreadArray[i].Status != StatusOfThread.Finished))
                            {
                                if (ThreadArray[i].workSheet.SourceFileName == fileName)
                                {
                                    isBeingProcessed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                return (isBeingProcessed);
            }
        }
        #endregion

        #region IsATM_BeingProcessed
        // Check if there is a thread handling a specific file
        public static bool IsATM_BeingProcessed(string AtmNumber)
        {
            lock (ThreadArrayLock)
            {
                bool isBeingProcessed = false;

                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].oThread != null)
                    {
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            if ((ThreadArray[i].Status != StatusOfThread.Available) || (ThreadArray[i].Status != StatusOfThread.Finished))
                            {
                                if (ThreadArray[i].workSheet.AtmNo == AtmNumber)
                                {
                                    isBeingProcessed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                return (isBeingProcessed);
            }
        }
        #endregion

        #region IsHASHBeingProcessed
        // Check if there is a thread handling a specific file
        public static bool IsHASHBeingProcessed(string hashValue)
        {
            lock (ThreadArrayLock)
            {
                bool isBeingProcessed = false;

                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].oThread != null)
                    {
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            if ((ThreadArray[i].Status != StatusOfThread.Available) || (ThreadArray[i].Status != StatusOfThread.Finished))
                            {
                                if (ThreadArray[i].workSheet.HashValue == hashValue)
                                {
                                    isBeingProcessed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                return (isBeingProcessed);
            }
        }
        #endregion

        #region GetActiveThreadNumber
        // Return the number of active threads
        public static int GetActiveThreadNumber()
        {
            lock (ThreadArrayLock)
            {
                int count = 0;
                for (int i = 0; i < RFMJ_MaxThreadCount; i++)
                {
                    if (ThreadArray[i].oThread != null)
                    {
                        if (ThreadArray[i].oThread.IsAlive)
                        {
                            if ((ThreadArray[i].Status != StatusOfThread.Available) || (ThreadArray[i].Status != StatusOfThread.Finished))
                            {
                                count++;
                            }
                        }
                    }
                }
                return (count);
            }
        }
        #endregion

        #region RaiseAbortFlag()
        public static void RaiseAbortFlag()
        {
            lock (ThreadArrayLock)
            {
                RfmjServer.Abort_Abort = true;
            }
            return;
        }
        #endregion
    }
}

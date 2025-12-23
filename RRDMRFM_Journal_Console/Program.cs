using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RRDMRFM_Journal_Classes;


namespace RRDMRFM_Journal_Console
{
    class Program
    {
        const string PROGRAM_MUTEX_NAME = "RFMJSERVICEINSTANCE";
        static Mutex InstanceMutex;
        static bool IsTheOnlyInstance;

        #region Static members
        private static string argOrigin = "";        // argument 1
        private static string argSourceFileID = "";  // argument 2
        private static string argOperator = "";      // argument 3
        #endregion

        static void Main(string[] args)
        {
            // Check if another instance of this program is running
            InstanceMutex = new Mutex(true, PROGRAM_MUTEX_NAME, out IsTheOnlyInstance);
            if (IsTheOnlyInstance == false)
            {
                string msg = "\r\n    Terminating because another instance of this service is already active!!";
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                Console.WriteLine(msg);
                Console.WriteLine("\r\n    Press ENTER to exit..");
                Console.ReadLine();
                return;
            }

            #region Read parameters passed to the program
            if (args.Length != 3)
            {
                string msg = "Processing stopped!\nInvalid number of parameters passed! Arguments are:\n   [SystemOfOrigin]\n   [SourcrFileID]\n   [Operator]\n\nPress OK to exit...";
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            else
            {
                RfmjServer.argOrigin = argOrigin = args[0];
                RfmjServer.argSourceFileID = argSourceFileID = args[1];
                RfmjServer.argOperator = argOperator = args[2];
            }
            #endregion

            #region Set Title
            if (Environment.UserInteractive)
            {
                // Set the console window title
                string m = string.Format("RRDM Reconciliation File Monitor - System:{0} - Filetype:{1} - Operator:{2}", argOrigin, argSourceFileID, argOperator);
                Console.Title = m;
            }
            #endregion

            // Prepare for handling CTRL+C and CTRL+BREAK
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            // RfmjServer instantiates the server thread and retruns its handle
            // Server parameters are read during instantiation and RfmjServer.RfmjOp is populated.
            Thread oT;
            RfmjServer St = new RfmjServer();
            if (St.status != true)
            {
                Console.WriteLine("Failed to start RfmjServer()!\r\nThe status message reads:\r\n{0}", St.statusMsg);
                Console.WriteLine("\n\n Press ENTER to exit! \n\n");
                Console.ReadLine();
                return;
            }

            // Launch the server main thread and get its handle
            oT = St.RfmjServerLauncher();
            if (oT == null)
            {
                Console.WriteLine("Failed to start RfmjServer()!\r\nThe status message reads:\r\n{0}", St.statusMsg);
                Console.WriteLine("\n\n Press ENTER to exit! \n\n");
                Console.ReadLine();
                return;
            }

            string fstr = "RFMJ stared with the following parameters:\n" +
                              "MaxThreads:{0}\n" +
                              "SleepWaitEmptyThreadSlot:{1}ms\n" +
                              "ThreadStartTimeout:{2}ms\n" +
                              "ThreadMonitorInterval:{3}ms\n" +
                              "ThreadAbortWaitTime:{4}ms\n" +
                              "MaxThreadLifeSpan:{5}sec\n" +
                              "Stored Procedure for Parsing:{6}\n" +
                              "Refresh Interval:{7}\n" +
                              "FilePoolRoot Relative to SQL:{8}\n" +
                              "Operator is:{9}"
                              ;
            //
            string msg1 = string.Format(fstr, RfmjServer.RfmjOp.RfmjMaxThreadNumber,
                               RfmjServer.RfmjOp.RfmjSleepWaitEmptyThreadSlot,
                                RfmjServer.RfmjOp.RfmjStartWorkerThreadTimeout,
                                RfmjServer.RfmjOp.RfmjThreadMonitorInterval,
                                RfmjServer.RfmjOp.RfmjThreadAbortWait,
                                RfmjServer.RfmjOp.RfmjMaxThreadLifeSpan,
                                RfmjServer.RfmjOp.RfmjStoredProcedure,
                                RfmjServer.RfmjOp.RfmjRefreshInterval,
                                RfmjServer.RfmjOp.RfmjSQLRelativeFilePoolPath,
                                RfmjServer.argOperator);

          //  Rfmj_Params.Operator);
            Console.WriteLine(msg1);
            Console.WriteLine(Environment.NewLine);
            //Console.WriteLine("Press ENTER to continue...");
            //Console.ReadLine();

            // Wait until the Server ends, check this every 500 ms
            while (!St.Rfmj_ServerMainThread.Join(500))
            {
                string sOut = "";
                string stg = "";
                string Alive = "";

                RfmjWorkerSheets sh = RfmjThreadRegistry.GetWorkerSheets();

                Console.Clear();
                msg1 = string.Format(fstr, RfmjServer.RfmjOp.RfmjMaxThreadNumber,
                         RfmjServer.RfmjOp.RfmjSleepWaitEmptyThreadSlot,
                          RfmjServer.RfmjOp.RfmjStartWorkerThreadTimeout,
                          RfmjServer.RfmjOp.RfmjThreadMonitorInterval,
                          RfmjServer.RfmjOp.RfmjThreadAbortWait,
                          RfmjServer.RfmjOp.RfmjMaxThreadLifeSpan,
                          RfmjServer.RfmjOp.RfmjStoredProcedure,
                          RfmjServer.RfmjOp.RfmjRefreshInterval,
                          RfmjServer.RfmjOp.RfmjSQLRelativeFilePoolPath,
                RfmjServer.argOperator);

                Console.WriteLine(msg1);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("\tSlot IsAlive Status     FileName                        Stage");
                Console.WriteLine("\t---- ------- ---------- -------------------------       -----");
                for (int j = 0; j < RfmjThreadRegistry.RFMJ_MaxThreadCount; j++)
                {
                    if (RfmjThreadRegistry.ThreadArray[j].oThread != null)
                    {
                        /*
        public const int Const_Intercepted = 0;
        public const int Const_WorkInProgress = 1;
        public const int Const_StageOneInProgress = 2;
        public const int Const_StageOneFinished = 3;
        public const int Const_StageTwoWaitingToStart = 4;
        public const int Const_StageTwoInProgress = 5;
        public const int Const_StageTwoFinished = 6;
        public const int Const_StageThreeInProgress = 7;
        public const int Const_StageThreeFinished = 8;
        public const int Const_Aborted = 98;
        public const int Const_Finished = 99;
                        */
                        string stgDescr = "";
                        stg = RfmjActionStage.getStageFromNumber(RfmjThreadRegistry.ThreadArray[j].workSheet.Stage);
                        switch (RfmjThreadRegistry.ThreadArray[j].workSheet.Stage)
                        {
                            case RfmjActionStage.Const_Step_1_InProgress:
                            case RfmjActionStage.Const_Step_1_Finished:
                                {
                                    stgDescr = RfmjThreadRegistry.ThreadArray[j].workSheet.Step_1_Descr;
                                    break;
                                }
                            case RfmjActionStage.Const_Step_2_InProgress:
                            case RfmjActionStage.Const_Step_2_Finished:
                                {
                                    stgDescr = RfmjThreadRegistry.ThreadArray[j].workSheet.Step_2_Descr;
                                    break;
                                }
                            case RfmjActionStage.Const_Step_3_InProgress:
                            case RfmjActionStage.Const_Step_3_Finished:
                                {
                                    stgDescr = RfmjThreadRegistry.ThreadArray[j].workSheet.Step_3_Descr;
                                    break;
                                }
                            case RfmjActionStage.Const_Step_4_InProgress:
                            case RfmjActionStage.Const_Step_4_Finished:
                                {
                                    stgDescr = RfmjThreadRegistry.ThreadArray[j].workSheet.Step_4_Descr;
                                    break;
                                }
                            default:
                                {
                                    stgDescr = "";
                                    break;
                                }

                        }
                        Alive = ((RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? "  Yes  " : "  No   ");
                        // sOut = string.Format("\t{0, 4} {1, -7} {2, -10} {3, -25} {4, -5} {5,2} {6}",
                         sOut = string.Format("\t{0, 4} {1, -7} {2, -10} {3, -25}       {4, -5} {5}",
                                            j.ToString(),
                                            Alive,
                                            (RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? RfmjThreadRegistry.ThreadArray[j].Status : StatusOfThread.Unknown,
                                            (RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? RfmjThreadRegistry.ThreadArray[j].workSheet.SourceFileName : "",
                                            (RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? RfmjThreadRegistry.ThreadArray[j].workSheet.Stage : 0,
                                            // (RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? stg : "",
                                            (RfmjThreadRegistry.ThreadArray[j].oThread.IsAlive) ? stgDescr : "");

                    }
                    else
                    {
                        stg = "";
                        Alive = "  No   ";
                        sOut = string.Format("\t{0, 4} {1, -7} {2, -10} {3}",
                                            j.ToString(),
                                            "Free",
                                            "",
                                            "");
                    }
                    Console.WriteLine(sOut);
                }

            }

            // Release the mutex
            InstanceMutex.ReleaseMutex();

            //Console.WriteLine("\n\n Press ENTER to exit! \n\n");
            //Console.ReadLine();

            Console.WriteLine("\n\n Terminated ... \n\n");
            Thread.Sleep(500);

        }

        #region ConsoleCtrlCheck

        // Declare the SetConsoleCtrlHandler function as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumeration type for the control messages sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        /// <summary>
        /// ConsoleCtrlCheck
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                    //case CtrlTypes.CTRL_CLOSE_EVENT:
                    {
                        // Log...
                        //string msg = "Terminating because of CTRL+C or CTRL+BREAK!";
                        //EventLogging.MessageOut(RfmjServer.eBzEventSource, msg, EventLogEntryType.Error);

                        // CTRLKeyPressed = true; // Not really needed as we raise the abort flag instead

                        // Raise the Abort_Abort flag
                        RfmjThreadRegistry.RaiseAbortFlag();
                        break;
                    }
            }
            return true;
        }
        #endregion
    }
}

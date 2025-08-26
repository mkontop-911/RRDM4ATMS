using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
//
using System.Windows.Forms; // for MessageBox
using System.Diagnostics;
using System.Runtime.InteropServices;
//
using RRDM4ATMs;
using RRDMAgent_Classes;


namespace RRDMAgent_Console
{
    class Program
    {

        const string PROGRAM_MUTEX_NAME = "RRDMAGENT";
        private static string instanceMutexName = "";

        #region Static members
        // private static string argOrigin = "";        // argument 1
        // private static string argSourceFileID = "";  // argument 2
        private static string argOperator = "";      // argument 3
        #endregion

        #region Main Program()
        static void Main(string[] args)
        {
            #region Read parameters passed to the program
            if (args.Length != 1)
            {
                string msg = "Processing stopped!\nInvalid number of parameters passed! Arguments are:\n   [SystemOfOrigin]\n   [SourcrFileID]\n   [Operator]\n\nPress OK to exit...";
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            else
            {
                RRDMAgentStartPoint.argOperator = argOperator = args[0];
            }
            #endregion

            #region Set Title
            if (Environment.UserInteractive)
            {
                // Set the console window title
                string m = string.Format("RRDM Agent - Operator:{0}", argOperator);
                Console.Title = m;
            }
            #endregion

            bool IsTheOnlyInstance;
            instanceMutexName = PROGRAM_MUTEX_NAME;
            Mutex InstanceMutex = new Mutex(true, instanceMutexName, out IsTheOnlyInstance);

            if (IsTheOnlyInstance == false)
            {
                string msg = string.Format("{0}\r\nAnother instance of this program is already running!\r\nOnly one instance is allowed to run at any time!",
                                           instanceMutexName);
                MessageBox.Show(msg, "RRDMAGENT", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }


            #region Prepare for handling CTRL+C and CTRL+BREAK
            // Prepare for handling CTRL+C and CTRL+BREAK
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            #endregion

            RRDMAgentServer agntS = new RRDMAgentServer(); 
            Thread _threadObj = agntS.StartRRDMAgentThread(argOperator);
            if (_threadObj != null)
            {
                _threadObj.Join();
            }
            else
            {
                string msg = string.Format("\r\n Failed to start the RRDM Agent main Service Thread! \r\nThe program terminates!\r\n");
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
            }
            Console.WriteLine("Press ENTER to exit!");
            Console.ReadLine();
        }
        /* ================= End of Main() ============ */
        #endregion

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
        /// <returns>true/false</returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                    {
                        RRDMAgentStartPoint.SignalToStop = true;
                        string msg = "Terminating because of CTRL_LOGOFF_EVENT!";
                        AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                        break;
                    }
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    {
                        RRDMAgentStartPoint.SignalToStop = true;
                        string msg = "Terminating because of CTRL_SHUTDOWN_EVENT!";
                        AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                        break;
                    }
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                default:
                    {
                        RRDMAgentStartPoint.SignalToStop = true;
                        string msg = "Terminating because of CTRL_C_EVENT | CTRL_BREAK_EVENT | CTRL_CLOSE_EVENT!";
                        AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                        break;
                    }
            }
            return true;
        }
        #endregion

    }
}

using System;
using System.Threading;
using System.Diagnostics;

using System.Runtime.InteropServices;


// using RRDM4ATMs;
using RRDMJTMClasses;


namespace RRDMJTM
{
    class Program
    {
        const string PROGRAM_MUTEX_NAME = "JTMSERVICEINSTANCE";
        static Mutex InstanceMutex;
        static bool IsTheOnlyInstance;

        // static bool CTRLKeyPressed = false; // No necessary since we will be raqising the abort flag instead

        static void Main(string[] args)
        {
            // Check if another instance of this program is running
            InstanceMutex = new Mutex(true, PROGRAM_MUTEX_NAME, out IsTheOnlyInstance);
            if (IsTheOnlyInstance == false)
            {
                string msg = "Terminating because another instance of this service is already active!!";
                EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                Console.WriteLine(msg);
                Console.WriteLine("Press ENTER to exit..");
                Console.ReadLine();
                return;
            }

            // Prepare for handling CTRL+C and CTRL+BREAK
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            // Initialize the file used to display thread info and messages
            EventLogging.ClearFileContents();

            // StartJTM launches the JTMServer thread and retruns its handle
            Thread oT;
            JTMServer St = new JTMServer();
            oT = St.StartJTM();

            // Wait until JTMServer ends
            oT.Join();

            Console.WriteLine("\n\n Press ENTER to exit! \n\n");
            Console.ReadLine();

            // Release the mutex
            InstanceMutex.ReleaseMutex();
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
                        string msg = "Terminating because of CTRL+C or CTRL+BREAK!";
                        EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                        
                        // CTRLKeyPressed = true; // Not really needed as we raise the abort flag instead

                        // Raise the Abort_Abort flag
                        JTMThreadRegistry.RaiseAbortFlag();
                        break;
                    }
            }
            return true;
        }
        #endregion
    }
}

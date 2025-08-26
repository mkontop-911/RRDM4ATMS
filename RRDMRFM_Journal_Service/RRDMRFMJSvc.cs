using System;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using RRDMRFM_Journal_Classes;

namespace RRDMRFMJService
{

    public partial class RRDMRFMJSvc : ServiceBase
    {
        private volatile static string argOrigin;
        private volatile static string argSourceFileID;
        private volatile static string argOperator;
        private volatile Thread ThreadObj = null;

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceStatus
        {
            public uint dwServiceType;
            public ServiceState dwCurrentState;
            public uint dwControlsAccepted;
            public uint dwWin32ExitCode;
            public uint dwServiceSpecificExitCode;
            public uint dwCheckPoint;
            public uint dwWaitHint;
        };


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        //public RRDMRFMSvc(string _argOrigin, string _argSourceFileID, string _argOperator)
        public RRDMRFMJSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            // System.Diagnostics.Debugger.Launch();

            //string[] imagePathArgs = Environment.GetCommandLineArgs();

            //if (imagePathArgs.Length != 4)
            //{
            //    string msg = "Could not start the service!\r\nInvalid number of parameters passed! Service arguments are: [SystemOfOrigin] [SourcrFileID] [Operator]";
            //    EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
            //    return;
            //}

            //RfmjServer.argOrigin = argOrigin = args[1];
            //RfmjServer.argSourceFileID = argSourceFileID = args[2];
            //RfmjServer.argOperator = argOperator = args[3];
            //// RfmjServer.argOperator = argOperator = "ETHNCY2N";

            argOrigin = RfmjServer.argOrigin;
            argSourceFileID = RfmjServer.argSourceFileID;
            argOperator = RfmjServer.argOperator;

            string m = string.Format("RRDM Reconciliation File Monitor - System:{0} - Filetype:{1} - Operator:{2}", argOrigin, argSourceFileID, argOperator);
            EventLogging.RecordEventMsg(m, EventLogEntryType.Information);

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 30000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // RfmjServer instantiates the server thread and retruns its handle
            // Server parameters are read during instantiation and RfmjServer.RfmjOp is populated.
            Thread _threadObj;
            RfmjServer St = new RfmjServer();
            if (St.status != true)
            {
                Console.WriteLine("Failed to instantiate RfmjServer()!\r\nThe status message reads:\r\n{0}", St.statusMsg);
                string msg = string.Format("Failed to instantiate RfmjServer()!\r\nThe status message reads:\r\n{0}", St.statusMsg);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }

            // Launch the server main thread and get its handle
            _threadObj = St.RfmjServerLauncher();
            if (_threadObj != null)
            {
                ThreadObj = _threadObj;

                string fstr = "RFMJ stared with the following parameters:\n" +
                    "\tMaxThreads:{0}\n" +
                    "\tSleepWaitEmptyThreadSlot:{1}ms\n" +
                    "\tThreadStartTimeout:{2}ms\n" +
                    "\tThreadMonitorInterval:{3}ms\n" +
                    "\tThreadAbortWaitTime:{4}ms\n" +
                    "\tMaxThreadLifeSpan:{5}sec\n" +
                    "\tStored Procedure for Parsing:{6}\n" +
                    "\tRefresh Interval:{7}\n" +
                    "\tFilePoolRoot Relative to SQL:{8}";

                string msg1 = string.Format(fstr, RfmjServer.RfmjOp.RfmjMaxThreadNumber,
                                         RfmjServer.RfmjOp.RfmjSleepWaitEmptyThreadSlot,
                                          RfmjServer.RfmjOp.RfmjStartWorkerThreadTimeout,
                                          RfmjServer.RfmjOp.RfmjThreadMonitorInterval,
                                          RfmjServer.RfmjOp.RfmjThreadAbortWait,
                                          RfmjServer.RfmjOp.RfmjMaxThreadLifeSpan,
                                          RfmjServer.RfmjOp.RfmjStoredProcedure,
                                          RfmjServer.RfmjOp.RfmjRefreshInterval,
                                          RfmjServer.RfmjOp.RfmjSQLRelativeFilePoolPath);
                EventLogging.RecordEventMsg(msg1, EventLogEntryType.Information);

                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                string msg = string.Format("\r\nService started successfully!");
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Information);
            }
            else
            {
                string msg = string.Format("Failed to launch RfmjServer()!\r\nThe status message reads:\r\n{0}", St.statusMsg);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                return;
            }

        }

        protected override void OnStop()
        {
            int thrdTimeout = 20000; // ToDo
            string msg = "Service Stopping...";
            EventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
            
            RfmjThreadRegistry.RaiseAbortFlag();

            // TODO consider parameterizing timeout
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = (uint)thrdTimeout;
            serviceStatus.dwCheckPoint = 0;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            bool rc = false;
            int maxJoinLoops = thrdTimeout / 1000;
            for (int i = 0; i < maxJoinLoops; i++) 
            {
                rc = ThreadObj.Join(1000);
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = (uint)thrdTimeout;
                serviceStatus.dwCheckPoint++;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            msg = "Service Stopped!";
            EventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
        }

    }
}

using System;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using RRDMRFMClasses;

namespace RRDMRFMService
{

    public partial class RRDMRFMSvc : ServiceBase
    {
        private static string argOrigin;
        private static string argSourceFileID;
        private static string argOperator;
        private Thread ThreadObj = null;

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
        public RRDMRFMSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string[] imagePathArgs = Environment.GetCommandLineArgs();
            string msg;

            if (imagePathArgs.Length != 4)
            {
                msg = "Could not start the service!\r\nInvalid number of parameters passed! Service arguments are: [SystemOfOrigin] [SourcrFileID] [Operator]";
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            }

            argOrigin = imagePathArgs[1];
            argSourceFileID = imagePathArgs[2];
            argOperator = imagePathArgs[3];
            // Write starting parameters in Log
            msg = string.Format("Service starting with parameters: [{0}][{1}][{2}]",
                                       argOrigin, argSourceFileID, argOperator);
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);


            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 30000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Start the server thread
            RFMServer rfmS = new RFMServer();
            Thread _threadObj = rfmS.StartRFMThread(argOrigin, argSourceFileID, argOperator);
            if (_threadObj != null)
            {
                ThreadObj = _threadObj;
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                msg = string.Format("\r\nService started successfully!");
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
            }
            else
            {
                msg = string.Format("\r\nFailed to start the Service Thread!\r\nThe service is STOPPED!");
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
        }

        protected override void OnStop()
        {
            int thrdTimeout = 20000; // ToDo
            string msg = "Service Stopping...";
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Warning);

            RFMStartPoint.SignalToStop = true;

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
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Warning);
        }

    }
}

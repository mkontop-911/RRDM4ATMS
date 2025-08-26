using System;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

using RRDMAgent_Classes;

namespace RRDMAgent_Service
{

    public partial class RRDMAgentSvc : ServiceBase
    {
        private volatile static string argOperator;
        private volatile static Thread ThreadObj = null;

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

        public RRDMAgentSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string msg = string.Format("OnStart: The RRDMAgent Service is starting...");
            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Information);

            string[] imagePathArgs = Environment.GetCommandLineArgs();

            if (imagePathArgs.Length != 2)
            {
                msg = "OnStart: Could not start the RRDMAgent service!\r\nRequired parameter [Operator] is missing";
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
            }

            argOperator = imagePathArgs[1];


            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 30000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Start the server thread
            RRDMAgentServer rfmS = new RRDMAgentServer();
            Thread _threadObj = rfmS.StartRRDMAgentThread(argOperator);
            if (_threadObj != null)
            {
                ThreadObj = _threadObj;
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);

                msg = string.Format("OnStart: The RRDMAgent Service started successfully!");
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Information);
            }
            else
            {
                msg = string.Format("OnStart: Failed to start the RRDMAgent Service Thread (OnStart)!\r\nThe service is STOPPED!");
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }
        }

        protected override void OnStop()
        {
            int thrdTimeout = 20000; // ToDo
            string msg = "OnStop: The RRDMAgent Service is Stopping ...";
            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);

            RRDMAgentStartPoint.SignalToStop = true;

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
                if (rc == true) break;
                serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
                serviceStatus.dwWaitHint = (uint)thrdTimeout;
                serviceStatus.dwCheckPoint++;
                SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            }

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            msg = "OnStop: The RRDMAgent Service Stopped!";
            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
        }

    }
}

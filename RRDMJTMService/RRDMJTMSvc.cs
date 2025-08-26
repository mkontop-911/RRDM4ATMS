using System;
using System.ServiceProcess;
using System.Threading;
using System.Runtime.InteropServices;

using RRDMJTMClasses;

namespace RRDMJTMService
{

    public partial class RRDMJTMSvc : ServiceBase
    {
        private JTMServer St;
        private Thread oServerThread;

        // TODO - to avoid both console and service running at the same time
        //private const string PROGRAM_MUTEX_NAME = "JTMSERVICEINSTANCE";
        //private bool IsTheOnlyInstance;
        //private  Mutex InstanceMutex;


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
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        public RRDMJTMSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 2000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // TODO investigate for service ??
            //InstanceMutex = new Mutex(true, PROGRAM_MUTEX_NAME, out IsTheOnlyInstance);

            //if (IsTheOnlyInstance == false)
            //{
            //    // It could be the RRDMJTM console application...
            //    string msg = "Terminating because another instance of this service is already active!!";
            //    EventLogging.MessageOut(JTMThreadRegistry.JTMEventSource, msg, EventLogEntryType.Error);
                
            //    return;
            //}

            // Start the server thread
            St = new JTMServer();
            oServerThread = St.StartJTM();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            // TODO consider parameterizing timeout
            serviceStatus.dwWaitHint = 15000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // InstanceMutex.ReleaseMutex();
                
            JTMThreadRegistry.RaiseAbortFlag();
            
            // wait until the Server thread is done..
            while (oServerThread.IsAlive)
            {
                // TODO consider parameterizing timeout
                Thread.Sleep(100);
            }
        }

    }
}

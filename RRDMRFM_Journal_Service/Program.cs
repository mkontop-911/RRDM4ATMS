using System.Diagnostics;
using System.ServiceProcess;

using RRDMRFM_Journal_Classes;

namespace RRDMRFM_Journal_Service
{
    static class Program
    {

        #region Static members
        private static string argOrigin = "";        // argument 1
        private static string argSourceFileID = "";  // argument 2
        private static string argOperator = "";      // argument 3
        #endregion

        /// <summary>
        /// The main entry point for the service
        /// </summary>
        static void Main(string[] args)
        {
            #region Read parameters passed to service
            if (args.Length != 3)
            {
                string msg = "Could not start the service!\nInvalid number of parameters passed! Arguments are: [SystemOfOrigin] [SourcrFileID] [Operator]";
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            else
            {
                argOrigin = args[0];
                argSourceFileID = args[1];
                argOperator = args[2];
                RfmjServer.argOrigin = argOrigin;
                RfmjServer.argSourceFileID = argSourceFileID;
                RfmjServer.argOperator = argOperator;
            }
            #endregion

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RRDMRFMJService.RRDMRFMJSvc() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

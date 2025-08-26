using System.Diagnostics;
using System.ServiceProcess;

using RRDMRFMClasses;

namespace RRDMRFMService
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
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            //else
            //{
            //    argOrigin = RFMStartPoint.argOrigin = args[0];
            //    argSourceFileID = RFMStartPoint.argSourceFileID = args[1];
            //    argOperator = RFMStartPoint.argOperator = args[2];
            //}
            else
            {
                argOrigin = args[0];
                argSourceFileID = args[1];
                argOperator = args[2];
            }
            #endregion

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                // new RRDMRFMSvc(argOrigin, argSourceFileID, argOperator)
                new RRDMRFMSvc() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

using System.Diagnostics;
using System.ServiceProcess;

using RRDMAgent_Classes;

namespace RRDMAgent_Service
{
    static class Program
    {

        #region Static members
        private static string argOperator = "";
        #endregion

        /// <summary>
        /// The main entry point for the service
        /// </summary>
        static void Main(string[] args)
        {
            #region Read parameters passed to service
            if (args.Length != 1)
            {
                string msg = "Could not start the service!\nInvalid number of parameters passed! Argument is [Operator]";
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            else
            {
                argOperator = args[0];
            }
            #endregion

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RRDMAgentSvc()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}



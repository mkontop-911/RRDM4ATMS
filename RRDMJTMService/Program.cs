using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RRDMJTMService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the service
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RRDMJTMSvc() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

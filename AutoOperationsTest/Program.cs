using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoOperations;
using Microsoft.Extensions.Configuration;

namespace AutoOperationsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("AutoOperations_appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Inject configuration into AutoOperations library
            RRDM_Auto_Load_Match.ConfigureConfiguration(configuration);

            // Execute the auto operation
            RRDM_Auto_Load_Match aut = new RRDM_Auto_Load_Match();
            aut.MasterAuto("Controller");
        }
    }
}

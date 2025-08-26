using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoOperations;

namespace AutoOperationsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // AutoOperation aut = new AutoOperation();

            RRDM_Auto_Load_Match aut = new RRDM_Auto_Load_Match(); 

            aut.MasterAuto("Controller"); 
        }
    }
}

using System;
using System.Windows.Forms;
using RRDM4ATMs;
using RRDM4ATMsWin;

namespace RRDM4ATMsWin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersAndFillDataTable_101();
            string WOperator = Gp.Operator;

            if (WOperator == "BCAIEGCX")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form40());
            }

            if (WOperator == "ALPHA_CY")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form40_ALPHA_CY());
            }

        }
    }
}

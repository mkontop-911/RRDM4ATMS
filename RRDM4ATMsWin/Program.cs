using System;
using System.Windows.Forms;
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
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form40());
        }
    }
}

using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMImages : Logger
    {
        public RRDMImages() : base() { }

        public Bitmap RRDMLogo;

        //
        // Methods 
        // READ MatchingJobCycles
        // FILL UP A TABLE
        //
        public Bitmap GetRRDMLogo()
        {
            try
            {
                string myLogo = System.IO.Path.Combine(Application.StartupPath, @"C:\DEVELOPMENT\RRDM4ATMS\RRDM4ATMsWin\Resources\logo2.png");
                RRDMLogo = new System.Drawing.Bitmap(myLogo);
            }
            catch (Exception ex)
            {
                HandleException(ex); 
            }
            return RRDMLogo;
        }

// Handle Exception 
        private void HandleException(Exception ex)
        {
            CatchDetails(ex);
        }

    }
}

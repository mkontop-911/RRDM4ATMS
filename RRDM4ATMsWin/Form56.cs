using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Drawing.Imaging;
using Microsoft.Reporting.WinForms;

namespace RRDM4ATMsWin
{
    public partial class Form56 : Form
    {

        string WSignedId;
        int WSignRecordNo;
        string WBankId;
 //       bool WPrive;
        string WAtmNo;
        int WSesNo;

        Bitmap Image1;
        Bitmap Image2;
        Bitmap Image3;
        Bitmap Image4;

        string WPhysicalInspectComm;
        string WReplUserComm; 

        string WReplGenComment; 
        string WReconcComment; 
       
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        public Form56(string InSignedId, int InSignRecordNo, string InBankId, string InAtmNo, int InSesNo,
            Bitmap ScreenA, Bitmap ScreenB, Bitmap ScreenC, Bitmap ScreenD, string InPhysicalInspectComm, string InReplUserComm, string InReplGenComment, string InReconcComment)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
          //  WPrive = InPrive;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            Image1 = ScreenA;
            Image2 = ScreenB;
            Image3 = ScreenC;       
            Image4 = ScreenD;

            WPhysicalInspectComm = InPhysicalInspectComm;
            WReplUserComm = InReplUserComm; 

            WReplGenComment = InReplGenComment; 
            WReconcComment = InReconcComment; 
                    
            InitializeComponent();
            Us.ReadUsersRecord(WSignedId);
            Am.ReadAtmsMainSpecific(WAtmNo);
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); 

        }

        private void Form56_Load(object sender, EventArgs e)
        {
             //pass parmeters to report
           
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen1", ConvertImageToBase64(Image1, ImageFormat.Png)));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen2", ConvertImageToBase64(Image2, ImageFormat.Png)));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen3", ConvertImageToBase64(Image3, ImageFormat.Png)));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen4", ConvertImageToBase64(Image4, ImageFormat.Png)));

                reportViewer1.LocalReport.SetParameters(new ReportParameter("TodayDate", DateTime.Now.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("AtmNo", WAtmNo));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("SessionNo", WSesNo.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("BranchId", Am.BranchName));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("BankId", WBankId));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("UserId", WSignedId.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("UserName", Us.UserName));

                reportViewer1.LocalReport.SetParameters(new ReportParameter("SesStart", Ta.SesDtTimeStart.ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("SesEnd", Ta.SesDtTimeEnd.ToString()));

                reportViewer1.LocalReport.SetParameters(new ReportParameter("PhysicalInspect", WPhysicalInspectComm));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("ReplUserComment", WReplUserComm));

                reportViewer1.LocalReport.SetParameters(new ReportParameter("ReplGenComment", WReplGenComment));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("ReconcComment", WReconcComment));

                this.reportViewer1.RefreshReport();

                System.Drawing.Printing.PageSettings pp = new System.Drawing.Printing.PageSettings();
                pp.Margins = new System.Drawing.Printing.Margins(20, 3, 20, 20);
                this.reportViewer1.SetPageSettings(pp);
            /*
                try
                {
                    // 
            }

            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
             */

        }

        private static string ConvertImageToBase64(Bitmap image, ImageFormat format)
        {
            byte[] imageArray;

            using (System.IO.MemoryStream imageStream = new System.IO.MemoryStream())
            {
                image.Save(imageStream, format);
                imageArray = new byte[imageStream.Length];
                imageStream.Seek(0, System.IO.SeekOrigin.Begin);
                imageStream.Read(imageArray, 0, (int)imageStream.Length);
            }

            return Convert.ToBase64String(imageArray);
        }
    }
}

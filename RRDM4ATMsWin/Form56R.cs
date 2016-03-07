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
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R : Form
    {
      /*  int WSignedNo;
        int WSignRecordNo;
        string WBankId;
        bool WPrive;
        string WAtmNo;
        int WSesNo; */

        Bitmap Image1;
        Bitmap Image2;
        Bitmap Image3;
        Bitmap Image4;

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        public Form56R(Bitmap ScreenA, Bitmap ScreenB, Bitmap ScreenC, Bitmap ScreenD)
        {
         /*   WSignedNo = InSignedNo;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
            WPrive = InPrive;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo; */

            Image1 = ScreenA;
            Image2 = ScreenB;
            Image3 = ScreenC;
            Image4 = ScreenD;
            InitializeComponent();

     //       Us.ReadUsersRecord(WSignedNo);
      //      Am.ReadAtmsMainSpecific(WAtmNo);
      //      Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); 
        }

        private void Form56R_Load(object sender, EventArgs e)
        {
            
            //pass parmeters to report
            try
            {

                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
                string RSReportName = "/ReplenishmentReport";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

            reportViewer1.ServerReport.SetParameters(new ReportParameter("Screen1", ConvertImageToBase64(Image1, ImageFormat.Png)));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("Screen2", ConvertImageToBase64(Image2, ImageFormat.Png)));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("Screen3", ConvertImageToBase64(Image3, ImageFormat.Png)));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("Screen4", ConvertImageToBase64(Image4, ImageFormat.Png)));

            reportViewer1.ServerReport.SetParameters(new ReportParameter("BranchNumb", "880"));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("BranchNm", "Lykavitos"));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("AtmNo", "ATMNo"));
            reportViewer1.ServerReport.SetParameters(new ReportParameter("UserNm", "Nicos Ioannou"));

            System.Drawing.Printing.PageSettings pp = new System.Drawing.Printing.PageSettings();
            pp.Margins = new System.Drawing.Printing.Margins(20, 3, 20, 20);
            this.reportViewer1.SetPageSettings(pp);

            this.reportViewer1.RefreshReport();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

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

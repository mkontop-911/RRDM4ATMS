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
    public partial class Form57 : Form
    {
       string WSignedId;
        int WSignRecordNo;
        string WBankId;
     //   bool WPrive;
        string WAtmNo;
        int WSesNo;

        Bitmap Image1;
        
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 

        public Form57(string InSignedId, int InSignRecordNo, string InBankId, string InAtmNo, int InSesNo, Bitmap ScreenD)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
          //  WPrive = InPrive;
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            Image1 = ScreenD;
           
            
            InitializeComponent();
            Us.ReadUsersRecord(WSignedId);
            Am.ReadAtmsMainSpecific(WAtmNo);
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo); 

        }

        private void Form57_Load(object sender, EventArgs e)
        {
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen1", ConvertImageToBase64(Image1, ImageFormat.Png)));
         //  reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen2", ConvertImageToBase64(Image2, ImageFormat.Png)));
         //   reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen3", ConvertImageToBase64(Image3, ImageFormat.Png)));
        //    reportViewer1.LocalReport.SetParameters(new ReportParameter("Screen4", ConvertImageToBase64(Image4, ImageFormat.Png)));
            
            reportViewer1.LocalReport.SetParameters(new ReportParameter("TodayDate", DateTime.Now.ToString()));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("AtmNo", WAtmNo));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("SessionNo", WSesNo.ToString ()));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("BranchId", Am.BranchName));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("BankId", WBankId));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("UserId", WSignedId.ToString ()));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("UserName", Us.UserName));

            reportViewer1.LocalReport.SetParameters(new ReportParameter("SesStart", Ta.SesDtTimeStart.ToString()));
        //    reportViewer1.LocalReport.SetParameters(new ReportParameter("SesEnd", Ta.SesDtTimeEnd.ToString()));

            this.reportViewer1.RefreshReport();

            System.Drawing.Printing.PageSettings pp=new System.Drawing.Printing.PageSettings();
            pp.Margins = new System.Drawing.Printing.Margins(20, 3, 20, 20);
            this.reportViewer1.SetPageSettings(pp);

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

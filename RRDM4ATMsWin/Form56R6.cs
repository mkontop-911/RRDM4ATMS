using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R6 : Form
    {
        // Show transactions for this Session 
        string WD11; // Bank Id 
        string WD12; // AtmNo
        int WD14; // SesNo 

        public Form56R6(string InD11, string InD12, int InD14 )
             
        {
            WD11 = InD11;
            WD12 = InD12;
            WD14 = InD14;
           
            InitializeComponent();
        }

        private void Form56R6_Load(object sender, EventArgs e)
        {
            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
             
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/ATMTransactions";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************
                reportViewer1.ServerReport.SetParameters(new ReportParameter("BankId", WD11));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("AtmNo", WD12));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("SesNo", WD14.ToString()));
               
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

        
    }
}

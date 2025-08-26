using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R18 : Form
    {
        // REPORT FOR NOTES

        string WHeader;
        string WInBankIdLogo;
        public Form56R18(string InHeader, string InBankIdLogo)
        {
            WHeader = InHeader;
            WInBankIdLogo = InBankIdLogo;

            InitializeComponent();
        }

        private void Form56R18_Load(object sender, EventArgs e)
        {

            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
                
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/NotesPrinting_ALL";
              //  string RSReportName = RsDir + "/Form2_Content_Printing_ALL";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Header", WHeader));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("InBankIdLogo", WInBankIdLogo));

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

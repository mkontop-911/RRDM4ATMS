using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R16 : Form
    {
        // REPORT FOR Trace 

        string RSReportName;


        string WOperator;
        string WTraceNumber; 
        string WTraceDtTm;
        string WAtmNo;
        string WUserId; 

        public Form56R16(string InOperator, string InTraceNumber, string InTraceDtTm, string InAtmNo, string InUserId)
        {
            WOperator = InOperator;
            WTraceNumber = InTraceNumber;
            WTraceDtTm = InTraceDtTm;
            WAtmNo = InAtmNo;
            WUserId = InUserId; 

            InitializeComponent();
        }

        private void Form56R16_Load(object sender, EventArgs e)
        {

            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];

                //if (WOperator == "ETHNCY2N") RSReportName = "/TraceJournalPrinting_NBG";
                //else RSReportName = "/TraceJournalPrinting";
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/TraceJournalPrinting_BDC";
               // RSReportName = "/TraceJournalPrinting_NBG";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Operator", WOperator));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("TraceNumber", WTraceNumber));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("TraceDtTm", WTraceDtTm));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("AtmNo", WAtmNo));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("UserId", WUserId));

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

using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R17 : Form
    {
        // REPORT FOR REPL Cycle  

        string WOperator;
        string WAtmNo;
        string WTraceStart;
        string WTraceEnd;
        string WTraceDtTm;
        
        //(WOperator, WAtmNo, WPrintTraceStart, WPrintTraceEnd , WPrintTraceDtTm);
        public Form56R17(string InOperator, string InAtmNo, string InTraceStart, string InTraceEnd  , string InTraceDtTm)
        {
            WOperator = InOperator;
            WAtmNo = InAtmNo;
            WTraceStart = InTraceStart;
            WTraceEnd = InTraceEnd;
            WTraceDtTm = InTraceDtTm;
            
            InitializeComponent();
        }

        private void Form56R17_Load(object sender, EventArgs e)
        {
            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
              
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/ReplCycleJournalPrinting";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Operator", WOperator));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("AtmNo", WAtmNo));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("TraceStart", WTraceStart));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("TraceEnd", WTraceEnd));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("TraceDtTm", WTraceDtTm));
                

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

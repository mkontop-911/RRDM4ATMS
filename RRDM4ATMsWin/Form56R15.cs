using System;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Configuration;


namespace RRDM4ATMsWin
{
    public partial class Form56R15 : Form
    {
        // REPORT FOR Instructions to CITs 
        string WCitId;
        string WOperator;
        string WInstructions;
        
        public Form56R15(string InCitId, string InOperator, string InInstructions)
        {
            WCitId = InCitId;
            WOperator = InOperator;
            WInstructions = InInstructions;
            
            InitializeComponent();
        }

        private void Form56R15_Load(object sender, EventArgs e)
        {
            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/InstructionsToCit";
              
                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************

                reportViewer1.ServerReport.SetParameters(new ReportParameter("CitId", WCitId));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Operator", WOperator));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Instructions", WInstructions));

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

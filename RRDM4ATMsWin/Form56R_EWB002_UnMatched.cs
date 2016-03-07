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
using System.Collections;
using Microsoft.Reporting.WinForms;
using System.Configuration;


namespace RRDM4ATMsWin
{
    public partial class Form56R_EWB002_UnMatched : Form
    {
        // Show transactions for this Session 
        string WD11; // Operator
        string WD12; // RMCateg
        string WD13; // RMCycle

        public Form56R_EWB002_UnMatched(string InD11, string InD12, string InD13)
        {
            WD11 = InD11;
            WD12 = InD12;
            WD13 = InD13;
            InitializeComponent();
        }

        private void Form56R_EWB002_UnMatched_Load(object sender, EventArgs e)
        {
            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
                string RSReportName = "/EWBankTransUnMatched";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************
                reportViewer1.ServerReport.SetParameters(new ReportParameter("Operator", WD11));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("PRMCateg", WD12));
                reportViewer1.ServerReport.SetParameters(new ReportParameter("PRMCycle", WD13));

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

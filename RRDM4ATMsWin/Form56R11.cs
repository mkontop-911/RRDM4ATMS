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
    public partial class Form56R11 : Form
    {
        string WOperator;

        public Form56R11(string InOperator)
        {
            WOperator = InOperator; 
            
            InitializeComponent();
        }

        private void Form56R11_Load(object sender, EventArgs e)
        {

            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
                string RSReportName = "/ATMsBasic";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************

                reportViewer1.ServerReport.SetParameters(new ReportParameter("Operator", WOperator));

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

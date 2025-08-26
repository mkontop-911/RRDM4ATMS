using System;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R44ITMX : Form
    {
        string WR1;
        string WR2;
        string WR3;
        string WR4;
        string WR5; 

        //  string WR13;

        public Form56R44ITMX(string R1, string R2, string R3,string R4, string R5)
        {
            WR1 = R1;
            WR2 = R2;
            WR3 = R3;
            WR4 = R4;
            WR5 = R5;

            InitializeComponent();
        }

        private void Form56R44ITMX_Load(object sender, EventArgs e)
        {
            //ShowReport();
            ArrayList reportParam = new ArrayList();
            reportParam = ReportDefaultPatam();
            ReportParameter[] param = new ReportParameter[reportParam.Count];
            for (int k = 0; k < reportParam.Count; k++)
            {
                param[k] = (ReportParameter)reportParam[k];
            }
            //// pass crendentitilas
            //rptViewer.ServerReport.ReportServerCredentials = 
            //  new ReportServerCredentials("uName", "PassWORD", "doMain");

            //pass parmeters to report
            try
            {
                string RSUri = ConfigurationManager.AppSettings["ReportServerUri"];
             
                string RsDir = ConfigurationManager.AppSettings["ReportsDir"];
                string RSReportName = RsDir + "/ITMXReport44";

                // Set the processing mode for the ReportViewer to Remote
                reportViewer1.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = reportViewer1.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl = new Uri(RSUri);
                serverReport.ReportPath = RSReportName;

                // ***********************

                reportViewer1.ServerReport.SetParameters(param); //Set Report Parameters
                reportViewer1.ShowParameterPrompts = false;
                //this.reportViewer1.RefreshReport();

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

        private ArrayList ReportDefaultPatam()
        {
            ArrayList arrLstDefaultParam = new ArrayList();
            arrLstDefaultParam.Add(CreateReportParameter("Heading", WR1));
            arrLstDefaultParam.Add(CreateReportParameter("FromDate", WR2));
            arrLstDefaultParam.Add(CreateReportParameter("ToDate", WR3));
            arrLstDefaultParam.Add(CreateReportParameter("InBankIdLogo", WR4));
            arrLstDefaultParam.Add(CreateReportParameter("InUserId", WR5));
            return arrLstDefaultParam;
        }
        private ReportParameter CreateReportParameter(string paramName, string pramValue)
        {
            ReportParameter aParam = new ReportParameter(paramName, pramValue);
            return aParam;
        }

    }
}

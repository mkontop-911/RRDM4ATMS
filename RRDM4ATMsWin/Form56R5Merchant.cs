using System;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R5Merchant : Form
    {
        string WD11;
        string WD12;
        string WD13;
        string WD14;
        string WD15;
        string WD16;
        string WD17;
        string WD18;
        string WD19;
       
        string WHeader;
        string WBankIdLogo;
       

        public Form56R5Merchant(string InD11, string InD12, string InD13, string InD14, string InD15, 
            string InD16, string InD17,
                        string InD18, string InD19,
                        string InHeader, string InBankIdLogo
                        )
        {
            WD11 = InD11;
            WD12 = InD12;
            WD13 = InD13;
            WD14 = InD14;
            WD15 = InD15;
            WD16 = InD16;
            WD17 = InD17;

            WD18 = InD18;
            WD19 = InD19;
        
            WHeader = InHeader;

            WBankIdLogo = InBankIdLogo;

            InitializeComponent();
        }

        private void Form56R5_Load(object sender, EventArgs e)
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
                string RSReportName = RsDir + "/MerchantDetails";

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
            arrLstDefaultParam.Add(CreateReportParameter("D11", WD11));
            arrLstDefaultParam.Add(CreateReportParameter("D12", WD12));
            arrLstDefaultParam.Add(CreateReportParameter("D13", WD13));
            arrLstDefaultParam.Add(CreateReportParameter("D14", WD14));
            arrLstDefaultParam.Add(CreateReportParameter("D15", WD15));
            arrLstDefaultParam.Add(CreateReportParameter("D16", WD16));
            arrLstDefaultParam.Add(CreateReportParameter("D17", WD17));
            arrLstDefaultParam.Add(CreateReportParameter("D18", WD18));
            arrLstDefaultParam.Add(CreateReportParameter("D19", WD19));
     
            arrLstDefaultParam.Add(CreateReportParameter("Header", WHeader));
            arrLstDefaultParam.Add(CreateReportParameter("InBankIdLogo", WBankIdLogo));
           

            return arrLstDefaultParam;
        }
        private ReportParameter CreateReportParameter(string paramName, string pramValue)
        {
            ReportParameter aParam = new ReportParameter(paramName, pramValue);
            return aParam;
        }
    }
}

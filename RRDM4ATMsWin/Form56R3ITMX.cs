using System;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R3ITMX : Form
    {
        string WR1;
        string WR2;
        string WR3;
        string WR4;
        string WR5;
        //string WR6;
        string WR7;
        string WR8;
        string WR9;
        string WR10;
        string WR11;

        string WR12;
        string WR13;
        string WR14;
       

        string WR21; // User Id

        string WR15;
        string WR16;

        string WR17;

        //  string WR13;

        public Form56R3ITMX(string R1, string R2, string R3, string R4, string R5, 
                        string R7, string R8, string R9, string R10, string R11,
                        string R12, string R13, string R14, string R21, string R15, string R16, string R17)
        {
            WR1 = R1;
            WR2 = R2;
            WR3 = R3;
            WR4 = R4;
            WR5 = R5;
            //WR6 = R6;
            WR7 = R7;
            WR8 = R8;
            WR9 = R9;
            WR10 = R10;
            WR11 = R11;

            WR12 = R12;
            WR13 = R13;
            WR14 = R14;

            WR21 = R21; // User Id 

            WR15 = R15;
            WR16 = R16;
            WR17 = R17;

            InitializeComponent();
        }

        private void Form56R3_Load(object sender, EventArgs e)
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
                string RSReportName = RsDir + "/TransactionsToBePostedITMX";

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
            arrLstDefaultParam.Add(CreateReportParameter("SwiftId", WR1));
            arrLstDefaultParam.Add(CreateReportParameter("BankNm", WR2));
            arrLstDefaultParam.Add(CreateReportParameter("BranchNm", WR3));
            arrLstDefaultParam.Add(CreateReportParameter("UniqueId", WR4));
            arrLstDefaultParam.Add(CreateReportParameter("TransTypeA", WR5));
            //arrLstDefaultParam.Add(CreateReportParameter("CardNumber", WR6));
            arrLstDefaultParam.Add(CreateReportParameter("AccountNoA", WR7));
            arrLstDefaultParam.Add(CreateReportParameter("Amount", WR8));
            arrLstDefaultParam.Add(CreateReportParameter("DescrA", WR9));
            arrLstDefaultParam.Add(CreateReportParameter("DispNo", WR10));
            arrLstDefaultParam.Add(CreateReportParameter("DispDescr", WR11));

            arrLstDefaultParam.Add(CreateReportParameter("TransTypeB", WR12));
            arrLstDefaultParam.Add(CreateReportParameter("AccountNoB", WR13));
            arrLstDefaultParam.Add(CreateReportParameter("DescrB", WR14));

            arrLstDefaultParam.Add(CreateReportParameter("UserNm", WR21));
            arrLstDefaultParam.Add(CreateReportParameter("Requestor", WR15));
            arrLstDefaultParam.Add(CreateReportParameter("Authoriser", WR16));
            arrLstDefaultParam.Add(CreateReportParameter("InBankIdLogo", WR17));
            return arrLstDefaultParam;
        }
        private ReportParameter CreateReportParameter(string paramName, string pramValue)
        {
            ReportParameter aParam = new ReportParameter(paramName, pramValue);
            return aParam;
        }
    }
}

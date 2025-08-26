using System;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Reporting.WinForms;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form56R5 : Form
    {
        string WD11;
        string WD12;
        string WD13;
        string WD14;
        string WD15;
        string WD16;
        string WD17;
        string WD21;
        string WD22;
        string WD23;
        string WD24;
        string WD25;
        string WD26;
        string WD27;
        string WD28;
        string WD29;
        string WD30;
        string WD31;
        string WD32;
        string WD33;
        string WD34;
        string WD35;
        string WD36;
        string WD37;
        string WD38;

        string WD40;
        string WD41;
        string WBankIdLogo;
        string WD39;
        string WD45;
        string WD46;
        string WD47;
        string WD48;
        string WD50;

        public Form56R5(string InD11, string InD12, string InD13, string InD14, string InD15, string InD16, string InD17,
                        string InD21, string InD22, string InD23, string InD24,
                        string InD25, string InD26, string InD27, string InD28,
                        string InD29, string InD30, string InD31, string InD32,
                        string InD33, string InD34, string InD35, string InD36,
                        string InD37, string InD38,
                        string InD40, string InD41, string InBankIdLogo, string InD39,
                        string InD45, string InD46, string InD47, string InD48,
                        string InD50
                        )
        {
            WD11 = InD11;
            WD12 = InD12;
            WD13 = InD13;
            WD14 = InD14;
            WD15 = InD15;
            WD16 = InD16;
            WD17 = InD17;

            WD21 = InD21;
            WD22 = InD22;
            WD23 = InD23;
            WD24 = InD24;
            WD25 = InD25;
            WD26 = InD26;

            WD27 = InD27;
            WD28 = InD28;
            WD29 = InD29;
            WD30 = InD30;
            WD31 = InD31;
            WD32 = InD32;

            WD33 = InD33;
            WD34 = InD34;
            WD35 = InD35;
            WD36 = InD36;

            WD37 = InD37;
            WD38 = InD38;

            WD40 = InD40;
            WD41 = InD41;

            WBankIdLogo = InBankIdLogo;

            WD39 = InD39;

            WD45 = InD45;
            WD46 = InD46;
            WD47 = InD47;
            WD48 = InD48;

            WD50 = InD50;


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
                string RSReportName = RsDir + "/Dispute01";

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
            arrLstDefaultParam.Add(CreateReportParameter("D21", WD21));
            arrLstDefaultParam.Add(CreateReportParameter("D22", WD22));
            arrLstDefaultParam.Add(CreateReportParameter("D23", WD23));
            arrLstDefaultParam.Add(CreateReportParameter("D24", WD24));

            arrLstDefaultParam.Add(CreateReportParameter("D25", WD25));
            arrLstDefaultParam.Add(CreateReportParameter("D26", WD26));
            arrLstDefaultParam.Add(CreateReportParameter("D27", WD27));
            arrLstDefaultParam.Add(CreateReportParameter("D28", WD28));

            arrLstDefaultParam.Add(CreateReportParameter("D29", WD29));
            arrLstDefaultParam.Add(CreateReportParameter("D30", WD30));
            arrLstDefaultParam.Add(CreateReportParameter("D31", WD31));
            arrLstDefaultParam.Add(CreateReportParameter("D32", WD32));

            arrLstDefaultParam.Add(CreateReportParameter("D33", WD33));
            arrLstDefaultParam.Add(CreateReportParameter("D34", WD34));
            arrLstDefaultParam.Add(CreateReportParameter("D35", WD35));
            arrLstDefaultParam.Add(CreateReportParameter("D36", WD36));

            arrLstDefaultParam.Add(CreateReportParameter("D37", WD37));
            arrLstDefaultParam.Add(CreateReportParameter("D38", WD38));

            arrLstDefaultParam.Add(CreateReportParameter("D40", WD40));
            arrLstDefaultParam.Add(CreateReportParameter("D41", WD41));
            arrLstDefaultParam.Add(CreateReportParameter("InBankIdLogo", WBankIdLogo));
            arrLstDefaultParam.Add(CreateReportParameter("D39", WD39));

            arrLstDefaultParam.Add(CreateReportParameter("D45", WD45));
            arrLstDefaultParam.Add(CreateReportParameter("D46", WD46));
            arrLstDefaultParam.Add(CreateReportParameter("D47", WD47));
            arrLstDefaultParam.Add(CreateReportParameter("D48", WD48));
            //arrLstDefaultParam.Add(CreateReportParameter("D50", WD50));

            return arrLstDefaultParam;
        }
        private ReportParameter CreateReportParameter(string paramName, string pramValue)
        {
            ReportParameter aParam = new ReportParameter(paramName, pramValue);
            return aParam;
        }
    }
}

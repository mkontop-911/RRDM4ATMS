using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form200JobCycles_GL_Mobile : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        string WCategoryId;

        DateTime WCut_Off_Date;

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        //RRDMMatchingCategoriesSessions Mcs = new RRDMMatchingCategoriesSessions();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMHolidays Ho = new RRDMHolidays();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string W_Application;

        string ProcessName;
        string Message;
        int Mode;

        DateTime SavedStartDt;

        //string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        string WJobCateg;

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles_GL_Mobile(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InJobCateg)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WJobCateg= W_Application = InJobCateg;

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //*****************************************
            //
            //*****************************************

            textBoxMsgBoard.Text = "Job Cycles ";

            // ....

            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
              + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");

            WJobCycleNo = 0;

        }
       
        int WReconcCycleNo;
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCateg);

            if (Rjc.RecordFound == true)
            {
                label4.Show();

                textBoxCutOff.Show();
                textBoxCutOff.Text = Rjc.Cut_Off_Date.Date.ToShortDateString();
            }
            else
            {
                label4.Hide();
                textBoxCutOff.Hide();
            }

            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='"+ WJobCateg+"'";
            Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);

            ShowGrid1();

        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus;
        string WTableId;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WReconcCycleNo = WJobCycleNo = (int)rowSelected.Cells[0].Value;

            //labelHeaderRight.Text = "GL TOTALS FOR CYCLE:_" + WJobCycleNo.ToString(); 

            textBoxReconcCycle.Text = WJobCycleNo.ToString();

            // Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            //string WSelectionCriteria = " WHERE RMCycle=" + WJobCycleNo;
            //Cgl.ReadSettlementToFillTable_By_Identity(WOperator, WSignedId,
            //                                                    WJobCycleNo, 1);

            buttonGL_ENTRIES.Text = "GL ENTRIES FOR_"+ WJobCycleNo;

            buttonGL_ENTRIES.Show(); 


           // ShowGrid2();

        }

        // Row Enter second grid
        //int WUniqueRecordId = 0;
        // int WSeqNo;
        string WMatchMask_2;
        string WCategory_2;

        int WSeqNo;
        string WMatchMask_3;

        DateTime WSettlement_Date ;
        string W_Identity ;
      
       

        // Show Grid1
        private void ShowGrid1()
        {

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            dataGridView1.Columns[0].Width = 60; // JobCycle;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // JobCategory;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 120; // StartDateTm.ToString();
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 120; // FinishDateTm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; //  Description
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 120; // "Status"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Show Grid2
        
       

        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                 "(ReadMsg = 0 AND ToAllAtms = 1)"
             + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


       

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

      
        // Auditors Report 
        int WRowIndexLeft;
        string WFunction;
        

        private void NForm80b3_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form200JobCycles_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
       
        // Category Details 
        private void buttonCategoriesDetails_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Discrepancies for the Selected Cycle"+Environment.NewLine
                + "AND withing group of Categories" + Cgl.CategoryGroup + Environment.NewLine
                + "Will be shown"
                ); 

            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3 NForm80b3;

            string WFunction = "View";

            int Type = 28;

            WCategoryId = Cgl.MatchingCateg; 

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", 
                WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, Cgl.Settlement_DATE, Cgl.CategoryGroup, "");
          //  int InSettlementDate, string CategoriesCombined, string GL_AccountNo)
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();

           
        }

        // Excel 
        private void buttonIndentities_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            if (Cgl.Table_Settlement_DATE_Identity.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to Export to excel");
                return;
            }

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;

            ExcelPath = "C:\\RRDM\\Working\\Files_123_Dpt_RMCycle_" + WReconcCycleNo.ToString() + "_" + ExcelDATE + ".xls";
            WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Cgl.Table_Settlement_DATE_BIN, ExcelPath);
        }

        // Excel Creation 
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            //String Output;

            //// System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\Test.tab");
            //System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\\RRDM\\Working\\Test_TAB");

            //// looping code
            //Output = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\r\n",
            //                     "Panicos1", "Panicos2", "Panicos3", "Panicos4", "Panicos5", "Panicos6");


            //OutStream.Write(Output);
            //// end looping code


            //OutStream.Close();

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            if (Cgl.Table_Settlement_DATE_BIN.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to Export to excel");
                return; 
            }

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;

            ExcelPath = "C:\\RRDM\\Working\\Files_123_Dpt_RMCycle_"+ WReconcCycleNo.ToString()+"_" + ExcelDATE + ".xls";
            WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Cgl.Table_Settlement_DATE_BIN, ExcelPath);
        }

        private void buttonSET_VS_CAP_Click(object sender, EventArgs e)
        {
            WCategoryId = Cgl.MatchingCateg;

            Form200_SET_Vs_CAP NForm200_SET_Vs_CAP;

            NForm200_SET_Vs_CAP = new Form200_SET_Vs_CAP(WSignedId, WOperator
                                                                     , WCategoryId, WJobCycleNo);
            NForm200_SET_Vs_CAP.ShowDialog();


            

            //NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "",
            //    WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, Cgl.Settlement_DATE, Cgl.CategoryGroup, "");
            ////  int InSettlementDate, string CategoriesCombined, string GL_AccountNo)
            //NForm80b3.FormClosed += NForm80b3_FormClosed;
            //NForm80b3.ShowDialog();
        }
// SHOw GL
        private void button_GL_ENTRIES_Click(object sender, EventArgs e)
        {
           
        }

        private void buttonGL_ENTRIES_Click(object sender, EventArgs e)
        {
            // SHOW GL Entries
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 8; // SHOW GL ENTRIES
            string WMatchedCat = "NO_Cat";
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat, "", WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }

        private void buttonDisputes_Click(object sender, EventArgs e)
        {
            // SHOW DISPUTE Entries
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 14; // SHOW DISPUTE ENTRIES
            string WMatchedCat = "NO_Cat";
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat, "", WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }

        private void buttonSurplus_Click(object sender, EventArgs e)
        {
            // SHOW Surplus Entries
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 15; // SHOW Surplus Entries
            string WMatchedCat = "NO_Cat";
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat, "", WReconcCycleNo, WMode, W_Application);
            NForm78d_Discre_MOBILE.ShowDialog();
        }
    }
}

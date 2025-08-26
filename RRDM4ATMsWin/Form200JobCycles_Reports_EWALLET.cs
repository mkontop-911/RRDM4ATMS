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
    public partial class Form200JobCycles_Reports_EWALLET : Form
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

        //RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMHolidays Ho = new RRDMHolidays();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //string ProcessName;
        //string Message;
        //int Mode;

        //DateTime SavedStartDt;

        //string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

       // string WMatchingRunningGroup;
        string WJobCategory;

        string W_Application; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles_Reports_EWALLET(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InJobCategory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            W_Application =  WJobCategory = InJobCategory;

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

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);

            int Mode = 1; // From the administrator

            if (Ba.ShortName == "ABE")
            {
                buttonAll_TXNS.Hide();
                //buttonAllRepl.Hide();
              //  buttonAllOutstandingRepl.Hide();
                buttonCUT_OFF_SUMMARY.Hide(); 
            }

            if (Ba.ShortName == "EGA")
            {
                buttonNotSettledALLCycles.Hide();
                buttonActionsALLCycles.Hide();
                buttonCUT_OFF_SUMMARY.Hide();
                buttonDisputePreInvestigation.Hide();
                buttonTEST_4_GRIDS.Hide();
                buttonGL_AND_Disputes.Hide(); 
            }


        }
        
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {
           
            int WReconcCycleNo;
           
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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

            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='"+ WJobCategory+ "'";
            Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);

            ShowGrid1(); 
          
        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus;
        string WTableId;
        DateTime HST_DATE;
        bool Is_In_HST; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WJobCycleNo = (int)rowSelected.Cells[0].Value;

            textBoxReconcCycle.Text = WJobCycleNo.ToString(); 

            //Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            // FIND HISTORY DATE
            Is_In_HST = false;
            string ParamId = "853";
            string OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true & Gp.OccuranceNm != "")
            {

                if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
                              , System.Globalization.DateTimeStyles.None, out HST_DATE))
                {
                    textBoxHST_DATE.Text = HST_DATE.ToShortDateString();

                    textBoxHST_DATE.Show();
                    label_HST.Show();

                    if (WCut_Off_Date.Date <= HST_DATE)
                    {
                        Is_In_HST = true;
                    }

                }
            }
            else
            {
                textBoxHST_DATE.Hide();
                label_HST.Hide();
            }

            // Show the two Grids on the right

           if ( Environment.MachineName == "RRDM-PANICOS")
            {
                // OK Continue 
            }
           else
            {
                // SET it accordingly not to be used for not authorised BDC
                Is_In_HST = false; 
            }


            if (Is_In_HST == false)
            {
                Mpa.ReadTablePoolDataToGetMaskTableMasks_1_E_Wallet(WJobCycleNo, W_Application); 
            }
            else
            {
                // In History
               // ReadTablePoolDataToGetMaskTableMasks_1_HST_E_Wallet(int InRMCycleNo, string W_Application)
                Mpa.ReadTablePoolDataToGetMaskTableMasks_1_HST_E_Wallet(WJobCycleNo, W_Application);
            }
                     
            ShowGrid2();

            if (Is_In_HST == false)
            {
                Mpa.ReadTablePoolDataToGetMaskTableMasks_2_E_Wallet(WJobCycleNo, W_Application);
            }
            else
            {
                // In History
                Mpa.ReadTablePoolDataToGetMaskTableMasks_2_HST_E_Wallet(WJobCycleNo, W_Application);
            }
            ShowGrid3();

            panel4.Show();
        }

        // Row Enter second grid
        //int WUniqueRecordId = 0;
        //int WSeqNo;
        string WMatchMask_2 ;
        string WCategory_2 ;

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WMatchMask_2 = (string)rowSelected.Cells[0].Value;
            WCategory_2 = (string)rowSelected.Cells[1].Value;

            
        }
        string WMatchMask_3;
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            WMatchMask_3 = (string)rowSelected.Cells[0].Value;
        }

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
        private void ShowGrid2()
        {

            dataGridView2.DataSource = Mpa.TableMasks_1.DefaultView;


            dataGridView2.Columns[0].Width = 80; 
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 80;
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 80;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        // Show Grid3
        private void ShowGrid3()
        {

            dataGridView3.DataSource = Mpa.TableMasks_2.DefaultView;

            dataGridView3.Columns[0].Width = 80;
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 80;
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


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
       
       
        RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
        // Show Matching Files 

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // Presenter cases
        private void buttonPresenterCases_Click(object sender, EventArgs e)
        {
            string FileId = "Switch_IST_Txns";
            Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WJobCycleNo
                                                                                 , WCut_Off_Date, 0);
            NForm78d_FileRecords_IST_PRESENTER.ShowDialog();
        }
     
        // Not Settled transactions 
        private void buttonNotSettled_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 18;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();


        }

        private void buttonCUT_OFF_SUMMARY_Click(object sender, EventArgs e)
        {
            // SHOW CUT OFF GL SUMMARY
            //
            // Mode 6 are 
            Form78d_CUT_OFF_GL NForm78d_CUT_OFF_GL;

            int WMode = 6; // CUT OFF GL Summary for Categories
            string WMatchingCateg = "";
            string WAtmNo = ""; 
            NForm78d_CUT_OFF_GL = new Form78d_CUT_OFF_GL(WOperator, WSignedId, WCut_Off_Date, WMatchingCateg, WAtmNo
                           , WJobCycleNo, WMode);
            NForm78d_CUT_OFF_GL.ShowDialog();
        }

       
        // Auditors Report 
        int WRowIndexLeft;
        string WFunction; 
        private void buttonAuditorsReport_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 17;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application , NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();
        }

        private void NForm80b3_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form200JobCycles_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// Discrepancies 
        private void buttonDiscrepancies_Click(object sender, EventArgs e)
        {

            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 19;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();

        }
        // Show line of grid 2 
        private void buttonShowLine_2_Click(object sender, EventArgs e)
        {

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 20;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategory_2, WJobCycleNo, WMatchMask_2, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();

            //Form80b3_MOBILE NForm80b3_MOBILE;

            //WFunction = "View";

            //int Type = 20;

            //string WUniqueId = WMatchMask_2;

            //NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategory_2, WJobCycleNo, WUniqueId, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            //NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            //NForm80b3_MOBILE.ShowDialog();
        }
        // Show line of grid 3
        private void buttonShowLine_3_Click(object sender, EventArgs e)
        {
            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 21;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategory_2, WJobCycleNo, WMatchMask_2, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();

            //Form80b3 NForm80b3;

            //WFunction = "View";

            //int Type = 21;

            //string WUniqueId = WMatchMask_3;

            //NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", "", WJobCycleNo, WUniqueId, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            //NForm80b3.FormClosed += NForm80b3_FormClosed;
            //NForm80b3.ShowDialog();
        }
// Actions this Cycle
        private void buttonActionsThisCycle_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 25;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();
        }
// not settled all Cycles
        private void buttonNotSettledALLCycles_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;
            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 26;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();
        }
// Actions ALL Cycles 
        private void buttonActionsALLCycles_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3_MOBILE NForm80b3_MOBILE;

            WFunction = "View";

            int Type = 27;

            NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator, W_Application, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
            NForm80b3_MOBILE.ShowDialog();
        }
// All Transactions this Cycle
        private void button1_Click(object sender, EventArgs e)
        {
            // READ ALL IN THIS CYCLE
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = " WHERE RMCycle =" + WJobCycleNo + " AND STAGE = '03'";

            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {

                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                int WMode2 = 1; // 

                Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId, Aoc.Occurance
                                                             , Aoc.OriginWorkFlow, WMode2);
                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;
            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();
        }
// ALL REPLENISHMENTS THIS CYCLE
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 2; // All Repl this cycle 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                , WJobCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// All Outstanding to be replenished 
        private void buttonAllOutstandingRepl_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 4; // All Outstanding for Repl 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                   , WJobCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// View Categories 
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW ALL Categories 
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 4; // show all categories  
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, "", WJobCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog(); 
        }
// GL AND DISPUTE ENTRIES
        private void buttonGL_AND_Disputes_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL_Mobile NForm200JobCycles_GL_Mobile;

            string WJobCateg = W_Application;
            NForm200JobCycles_GL_Mobile = new Form200JobCycles_GL_Mobile(WSignedId, WSignRecordNo, "", WOperator, WJobCateg);
            NForm200JobCycles_GL_Mobile.ShowDialog();
        }
// Dispute pre investigation 
        private void buttonDisputePreInvestigation_Click(object sender, EventArgs e)
        {
            Form3_PreInv_MOBILE NForm3_PreInv_MOBILE;
            //Form3_PreInv_MOBILE(string InSignedId, string InOperator, string InApplication ,int InRMCycle)
            NForm3_PreInv_MOBILE = new Form3_PreInv_MOBILE(WOperator, WSignedId, W_Application, WJobCycleNo);
            NForm3_PreInv_MOBILE.ShowDialog();
        }
// 4 grids 
        private void buttonTEST_4_GRIDS_Click(object sender, EventArgs e)
        {

            Form78d_MOBILE_4_Grids NForm78d_MOBILE_4_Grids;
            int WMode = 8; //
            string WMatchedCat = "ETI310";
            NForm78d_MOBILE_4_Grids = new Form78d_MOBILE_4_Grids(WOperator, WSignedId, WMatchedCat, WJobCycleNo, WMode, W_Application);
            NForm78d_MOBILE_4_Grids.ShowDialog();

        }
    }
}

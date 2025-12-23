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
    public partial class Form200JobCycles_Reports_AUDI : Form
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


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string ProcessName;
        string Message;
        int Mode;

        DateTime SavedStartDt;

        //string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        string WMatchingRunningGroup;

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles_Reports_AUDI(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WMatchingRunningGroup = InMatchingRunningGroup;

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
        string WJobCategory = "ATMs";
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo;

            Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne_Second_version(WOperator, WJobCategory);
            //ReadLastReconcJobCycleATMsAndNostroWithMinusOne_Second_version
            WReconcCycleNo = Rjc.JobCycle;

            //WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
  
           
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
             
            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='ATMs'";
            Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);

            ShowGrid1(); 
          
        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus;
        string WTableId; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WJobCycleNo = (int)rowSelected.Cells[0].Value;

            textBoxReconcCycle.Text = WJobCycleNo.ToString(); 

            Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            Mpa.ReadTablePoolDataToGetMaskTableMasks_1(WJobCycleNo); 
            
            ShowGrid2();

            Mpa.ReadTablePoolDataToGetMaskTableMasks_2(WJobCycleNo);

            ShowGrid3();
        }

        // Row Enter second grid
        int WUniqueRecordId = 0;
        int WSeqNo;
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

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 18;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();


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

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 17;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
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

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 19;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
           
        }
        // Show line of grid 2 
        private void buttonShowLine_2_Click(object sender, EventArgs e)
        {
            
            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 20;

            string WUniqueId = WMatchMask_2;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategory_2, WJobCycleNo, WUniqueId, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
        }
        // Show line of grid 3
        private void buttonShowLine_3_Click(object sender, EventArgs e)
        {
            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 21;

            string WUniqueId = WMatchMask_3;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", "", WJobCycleNo, WUniqueId, 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
        }
// Actions this Cycle
        private void buttonActionsThisCycle_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 25;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
        }
// not settled all Cycles
        private void buttonNotSettledALLCycles_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 26;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
        }
// Actions ALL Cycles 
        private void buttonActionsALLCycles_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            Form80b3 NForm80b3;

            WFunction = "View";

            int Type = 27;

            NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WJobCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
            NForm80b3.FormClosed += NForm80b3_FormClosed;
            NForm80b3.ShowDialog();
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

        private void buttonReplReport_Click(object sender, EventArgs e)
        {
            string WOrigin = "Our Atms";

            WJobCategory = "ATMs";

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }
            //RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();

            //Gl.UpdateCalculatedGL_For_All_ATMs(WReconcCycleNo, Rjc.Cut_Off_Date); 
            RRDMAccountsClass Acc = new RRDMAccountsClass();
            Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, Rjc.Cut_Off_Date);

            Form503_SesCombined NForm503_SesCombined;
            int Mode = 1;
            string TemoAtmNo = "";
            int TempReplCycle = 0;
            NForm503_SesCombined = new Form503_SesCombined(WSignedId, WSignRecordNo, WOperator, WOrigin, WReconcCycleNo, TemoAtmNo, TempReplCycle, Mode);
            NForm503_SesCombined.ShowDialog();

            return;


            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
            // Make Selection Of Validated Entries 
            int TempMode = 2; // Bank entries 
            string SelectionCriteria = " WHERE RMCycle = " + WReconcCycleNo;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records to update");
                return;
            }

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                // GET ALL fields

                //    RecordFound = true;
                int WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];
                string WAtmNo = (string)G4.DataTableG4SEntries.Rows[I]["AtmNo"];
                int WSesNo = (int)G4.DataTableG4SEntries.Rows[I]["ProcessedAtReplCycleNo"];
                decimal WUnloadedMachine = (decimal)G4.DataTableG4SEntries.Rows[I]["UnloadedMachine"];
                decimal WCash_Loaded_Machine = (decimal)G4.DataTableG4SEntries.Rows[I]["Cash_Loaded"];
                decimal WUnloadedMachineDep = (decimal)G4.DataTableG4SEntries.Rows[I]["Deposits"];
                string WMask = (string)G4.DataTableG4SEntries.Rows[I]["Mask"];

                DateTime ReplDate = (DateTime)G4.DataTableG4SEntries.Rows[I]["ReplDateG4S"];

                I++; // Read Next entry of the table 

            }

            string P1 = "Replenishment Report for Audi";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_G4S ReportATMSReplCycles = new Form56R69ATMS_G4S(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }
// GL View 
        private void buttonGL_View_Click(object sender, EventArgs e)
        {
            //string WJobCategory = "ATMs";

            //int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            //if (WReconcCycleNo == 0)
            //{
            //    if (Environment.UserInteractive)
            //    {
            //        MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
            //        return;
            //    }
            //}
            //else
            //{

            //}

            // 
            // UPDATE GL ENTRIES
            //
            MessageBox.Show("At this point we will calculate ATMs GL balances "+Environment.NewLine
                + "And compare them with the Banks Books GL balances "
                ); 

            DateTime Test_Cut_Off_Date = new DateTime(2021, 09, 05); // Testing date 
            RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();
            Gl.UpdateCalculatedGL_For_All_ATMs(WJobCycleNo, Test_Cut_Off_Date);

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            // FOR AUDI TYPE WE LOAD GL AND WE ALSO USE OTHER FORM For Replenishment 
            // 
            RRDMGasParameters Gp = new RRDMGasParameters();
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            // Manage accounts for individual ATM
            // ATM Cash Balance
            // ATM Excess Account
            // ATM Shortage 
            // If not found we insert
            Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, Rjc.Cut_Off_Date);

            if (AudiType == true)
            {
                // GL FILE WAS LOADED 
                Form503_GL_STATUS NForm503_GL_STATUS;
                int Mode = 1;
                NForm503_GL_STATUS = new Form503_GL_STATUS(WSignedId, WSignRecordNo, WOperator, WJobCycleNo, WCut_Off_Date, Mode);
                NForm503_GL_STATUS.ShowDialog();
            }

        }
// Link Categories 
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW ALL Categories 
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 4; // show all categories  
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, "", WJobCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog();

        }
    }
}

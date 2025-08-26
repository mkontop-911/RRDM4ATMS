using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.Text;

using System.Drawing;

namespace RRDM4ATMsWin
{
    public partial class Form67_Cycle_Rich_Picture_2 : Form
    {

        public bool FoundRecord;

        bool AudiType;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        RRDMActions_GL Ag = new RRDMActions_GL();
        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMGasParameters Gp = new RRDMGasParameters();

        // RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 
        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WSignedId;
        string WOperator;

        string WAtmNo;
        int WReplCycleNo;

        int WRMCycleNo;

        DateTime WFromDt;
        DateTime WToDt;

        int WMode;

        public Form67_Cycle_Rich_Picture_2(string InSignedId, string InOperator, string InAtmNo,
            int InSesNo, int InRMCycleNo, DateTime InFromDt, DateTime InToDt, int InMode)
        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WReplCycleNo = InSesNo;

            WRMCycleNo = InRMCycleNo;

            WFromDt = InFromDt;
            WToDt = InToDt;

            WMode = InMode; 

            // 12 Excess management Report as Osman requested
            // 13 Excess for a single ATM within 
            // 14 Shortage Reports 
            // 15 Shortage for a Single ATM within 


            InitializeComponent();

            if (WMode == 1)
            {
                WSelectionCriteria = "";
            }

            AudiType = false;
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

        }
        //
        // With LOAD LOAD DATA GRID
        //
        string WSelectionCriteria;
        private void Form67_Load(object sender, EventArgs e)
        {

           

            // ALL EXCESS By date Range 
            if (WMode == 12)
            {

                WSelectionCriteria = " ";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection_2(WSignedId, WOperator, WSelectionCriteria, WFromDt, WToDt, WMode);

                labelHeaderLeft.Text = "ALL EXCESS PER ATM FROM.." + WFromDt.ToShortDateString()
                                            + "..TO.." + WToDt.ToShortDateString();

                //labelActions.Hide();
                //panelActions.Hide(); // Hide Actions
           
                //buttonActionsThisCycle.Hide();
                //buttonTXNsThisCycle.Hide();

                labelTExcess.Show();
                textBoxTExcess.Show();
                labelTRefund.Show();
                textBoxTRefund.Show();
                // Hide Shortage
                labelTShortage.Hide();
                textBoxTShortage.Hide();
                labelTRecovered.Hide();
                textBoxTRecovered.Hide();

                textBoxTExcess.Text = Aoc.TotalExcess.ToString("#,##0.00");
                textBoxTRefund.Text = Aoc.TotalRefunded.ToString("#,##0.00");

                ShowGrid_1();
            }

            // ALL Shortage By date Range 
            if (WMode == 14)
            {

                WSelectionCriteria = " ";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection_2(WSignedId, WOperator, WSelectionCriteria, WFromDt, WToDt, WMode);

                labelHeaderLeft.Text = "ALL Shortage CYCLES FROM.." + WFromDt.ToShortDateString()
                                            + "..TO.." + WToDt.ToShortDateString();

                //labelActions.Hide();
                //panelActions.Hide(); // Hide Actions

                //buttonActionsThisCycle.Hide();
                //buttonTXNsThisCycle.Hide();

                textBoxTShortage.Text = Aoc.TotalExcess.ToString("#,##0.00");
                textBoxTRefund.Text = Aoc.TotalRefunded.ToString("#,##0.00");
                // Show Shortage
                labelTShortage.Show();
                textBoxTShortage.Show();
                labelTRecovered.Show();
                textBoxTRecovered.Show();
                // Hide Excess
                labelTExcess.Hide();
                textBoxTExcess.Hide();
                labelTRefund.Hide();
                textBoxTRefund.Hide();

                textBoxTShortage.Text = Aoc.TotalShortages.ToString("#,##0.00");
                textBoxTRecovered.Text = Aoc.TotalRecovered.ToString("#,##0.00");

                ShowGrid_1();
            }


        }

        // On Row Enter
        string WWAtmNo;
        
        
        string Maker;
        string Auth;
        string Cit_Id;

        // Row Enter
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WWAtmNo = (string)dataGridView2.Rows[e.RowIndex].Cells["Atm_No"].Value;
            //WWRepl_No = (int)dataGridView2.Rows[e.RowIndex].Cells["Repl_No"].Value;

            //SM_DATE_S = (string)dataGridView2.Rows[e.RowIndex].Cells["End_DATE"].Value;
            //Rec_DATE_S = (string)dataGridView2.Rows[e.RowIndex].Cells["Rec_DATE"].Value;


            //Maker = (string)dataGridView2.Rows[e.RowIndex].Cells["Maker"].Value;
            //Auth = (string)dataGridView2.Rows[e.RowIndex].Cells["Auth"].Value;
            Cit_Id = (string)dataGridView2.Rows[e.RowIndex].Cells["Cit_Id"].Value;

            //textBoxATMNo.Text = WWAtmNo;
            //textBoxReplCycle.Text = WWRepl_No.ToString();

            //textBoxSM_Date.Text = SM_DATE_S;
            //textBoxRec_Date.Text = Rec_DATE_S;

            //textBoxMaker.Text = Maker;
            //textBoxAuth.Text = Auth;
            //textBoxCit_Id.Text = Cit_Id; 

            int ATM_Mode = 0; 
            if (WMode == 12)
            {
                ATM_Mode = 13; 
            }
            if (WMode == 14)
            {
                ATM_Mode = 15;
            }
            // SHOW PER ATM 
            WSelectionCriteria = " WHERE A.AtmNo ='" + WWAtmNo +"'";
            Aoc.ReadRichPicture_ALL_ATMs_By_Selection_3_Per_ATM(WSignedId, WOperator, WSelectionCriteria, WFromDt, WToDt, ATM_Mode);
            
            ShowGrid_2(); 
        }

        int WWRepl_No;

        DateTime SM_Datetime_Start;
        DateTime SM_Datetime_End;
        // atm
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WWAtmNo = (string)dataGridView3.Rows[e.RowIndex].Cells["Atm_No"].Value;
            WWRepl_No = (int)dataGridView3.Rows[e.RowIndex].Cells["Repl_No"].Value;
        
            SM_Datetime_Start = (DateTime)dataGridView3.Rows[e.RowIndex].Cells["Start_DATE"].Value;
            SM_Datetime_End = (DateTime)dataGridView3.Rows[e.RowIndex].Cells["End_DATE"].Value;

            labelCyclesPerATM.Text = "ALL CYCLES FOR THE ATM.." + WWAtmNo;
        }

        // Show Grid

        private void ShowGrid_1()
        {


            dataGridView2.DataSource = Aoc.TxnsTableAllCycles_Details.DefaultView;
            dataGridView2.Show();

            if (Aoc.TxnsTableAllCycles_Details.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to SHOW");
                textBoxTotal.Text = Aoc.TxnsTableAllCycles_Details.Rows.Count.ToString();
                return;
            }
            else
            {
                textBoxTotal.Text = Aoc.TxnsTableAllCycles_Details.Rows.Count.ToString();
            }

           
            //// GRID
            dataGridView2.Columns[0].Width = 70; // ATM_No
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 85; // EXCESS or Shortage
            dataGridView2.Columns[1].DefaultCellStyle.Format = "#,##0.00";
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[2].Width = 85; // Refunded or Recovered
            dataGridView2.Columns[2].DefaultCellStyle.Format = "#,##0.00";
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[3].Width = 85; // Net Amount
            dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 60; // total  excess or shoratge 
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 60; //Total Repl 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 60; // Perce 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 70; // CIT ID
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            //dataGridView2.Columns[7].Width = 60; //Settled 
            //dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[8].Width = 70; // Settled Date
            //dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void ShowGrid_2()
        {

            dataGridView3.DataSource = Aoc.TxnsTableAllCycles_Details_2.DefaultView;
            dataGridView3.Show();

            if (Aoc.TxnsTableAllCycles_Details_2.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to SHOW for this Atm");
                textBoxTotalLinesATM.Text = Aoc.TxnsTableAllCycles_Details_2.Rows.Count.ToString();
                return;
            }
            else
            {
                textBoxTotalLinesATM.Text = Aoc.TxnsTableAllCycles_Details_2.Rows.Count.ToString();
            }

            //// GRID
            dataGridView3.Columns[0].Width = 70; // ATM_No
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[0].Visible = false; 

            dataGridView3.Columns[1].Width = 70; // Repl_No
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 72; // CYCLE START DATE 
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[2].HeaderText = "Start_Date";

            dataGridView3.Columns[3].Width = 72; // End_DATE
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[3].HeaderText = "End_Date";

            dataGridView3.Columns[4].Width = 72; // EXCESS or Shortage
            dataGridView3.Columns[4].DefaultCellStyle.Format = "#,##0.00";
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[5].Width = 72; // Refunded or Recovered
            dataGridView3.Columns[5].DefaultCellStyle.Format = "#,##0.00";
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[6].Width = 72; // Net Amount
            dataGridView3.Columns[6].DefaultCellStyle.Format = "#,##0.00";
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView3.Columns[7].Width = 50; //Settled 
            dataGridView3.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[8].Width = 70; // Settled Date
            dataGridView3.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }


        // Finish 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }



        // All Actions 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo
                + "' AND ReplCycle =" + WWRepl_No + "";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_2_OSMAN(WSelectionCriteria);

            Aoc.ReadActionsOccurancesAndFillTable_Small_2_OSMAN(WSelectionCriteria);

            // string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
        // All Accounting 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Wait till we implement in proper Data base ");
            return;
            // READ ALL IN THIS CYCLE
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo + "' AND ReplCycle =" + WWRepl_No + " AND OriginWorkFlow in ('Replenishment', 'Dispute')";
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
        string WActionId;
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
     
        // EXPORT TO EXCEL
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();
            string Id="";
            if (WMode == 12)
            {
                Id = "_Excess_".ToString();
            }
            if (WMode == 14)
            {
                Id = "_Shortage_".ToString();
            }
            //MessageBox.Show("Excel will be created in RRDM Working Directory");

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Aoc.TxnsTableAllCycles_Details, WorkingDir, ExcelPath);
        }

       
        // Actions this Cycle 
        private void buttonActionsThisCycle_Click(object sender, EventArgs e)
        {
            //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
            string WSelectionCriteria = "WHERE ActionId IN ('84','85', '86', '79') "
                + " AND RMCycle =" + WRMCycleNo;

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            // string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
        // Transactions this Cycle 
        private void buttonTXNsThisCycle_Click(object sender, EventArgs e)
        {
            // READ ALL IN THIS CYCLE
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = " WHERE RMCycle =" + WRMCycleNo
                + " AND OriginWorkFlow in ('Replenishment', 'Dispute')"
                + " AND  ActionId IN ('84','85', '86', '79') "
                ;
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
        // Show SM lines 
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WWRepl_No == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WWAtmNo + "' AND RRDM_ReplCycleNo =" + WWRepl_No
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WWAtmNo, WWRepl_No, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WWAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }
        }

        private void linkLabelShowCycleTXNS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
        // ATMs Excess
        private void linkLabelATMExcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Wait till we implement in proper Data base ");
            return;
        }
        // UnMatched
        private void linkLabelShowUnMatched_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Wait till we implement in proper Data base ");
            return;
        }
// atm 
        private void buttonExportForAtm_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();
            string Id = "";
            if (WMode == 12)
            {
                Id = "_Excess_"+ WWAtmNo;
            }
            if (WMode == 14)
            {
                Id = "_Shortage_" + WWAtmNo;
            }
            //MessageBox.Show("Excel will be created in RRDM Working Directory");

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Aoc.TxnsTableAllCycles_Details, WorkingDir, ExcelPath);
        }
// actions
        private void buttonALL_Actions_Click(object sender, EventArgs e)
        {
            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo
               + "' AND ReplCycle =" + WWRepl_No + "";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals_2_OSMAN(WSelectionCriteria);

            Aoc.ReadActionsOccurancesAndFillTable_Small_2_OSMAN(WSelectionCriteria);

            // string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
// all accounting
        private void buttonALL_ACoounting_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Wait till we implement in proper Data base ");
            return;
            // READ ALL IN THIS CYCLE
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo + "' AND ReplCycle =" + WWRepl_No + " AND OriginWorkFlow in ('Replenishment', 'Dispute')";
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
// SHOW SM LINES 
        private void linkLabelSM_LINES_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (WWRepl_No == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WWAtmNo + "' AND RRDM_ReplCycleNo =" + WWRepl_No
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WWAtmNo, WWRepl_No, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WWAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }
        }

        private void linkLabelUnmatched_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";
          
            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, 0 , WOperator, WFunction, 0,
                                                WAtmNo, SM_Datetime_Start, SM_Datetime_End, 2
                );
            NForm80b2.Show();
            MessageBox.Show("Wait till we implement in proper Data base ");
            return;
        }
    }
}


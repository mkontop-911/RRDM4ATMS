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
    public partial class Form67_Cycle_Rich_Picture : Form
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
        
        public Form67_Cycle_Rich_Picture(string InSignedId, string InOperator, string InAtmNo, 
            int InSesNo,int InRMCycleNo, DateTime InFromDt, DateTime InToDt, int InMode)
        {
            WSignedId = InSignedId;
          
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WReplCycleNo = InSesNo;

            WRMCycleNo = InRMCycleNo;

            WFromDt = InFromDt;
            WToDt = InToDt;

            WMode = InMode; // 1 Show for ALL ATMs In Suspense  
                            // 2 Show Replenished this RM Cycle 
                            // 3 Show for one ATM and one Repl Cycle
                            // 4 Show All ATMs Cycles that are not replenished yet 
                            // 5 Show all Replenishments for this ATM
                            // 6 Show all Oustanding Cycles for All ATMS by dates Range
                            // 7 Show Replenished this RM Cycle by Authoriser 
                            // 8 Show for ALL ATMs In Suspense for the Maker   
                            // 9 Show this Cycle that are not replenished yet 
                            // 35 Show all Replenishments for this ATM by dates Range


            InitializeComponent();

            int ComMode = 5 ; 
            comboBoxActionNm.DataSource = Ag.ReadTableToGet_ActionNm_Array_List(WOperator, ComMode);
            comboBoxActionNm.DisplayMember = "DisplayValue";

            // Force Matching Reason
            Gp.ParamId = "714";
            comboBoxReasonOfAction.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReasonOfAction.DisplayMember = "DisplayValue";

            if ( WMode == 1 )
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

            if (AudiType == true)
            {
                labelExcess.Text = "ATM_Openning_Excess";
                labelShortage.Text = "ATM_Openning_Shortage";
            }

        }
        //
        // With LOAD LOAD DATA GRID
        //
        string WSelectionCriteria;
        private void Form67_Load(object sender, EventArgs e)
        {

            if (WMode == 1)
            {
                WSelectionCriteria = " ";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria
                        , WFromDt, WToDt, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL CYCLES for in Excess and Shortage " + " FROM.." + WFromDt.ToShortDateString()
                                            + "..TO.." + WToDt.ToShortDateString();
                ;
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
                return; 
                //ReadRichPicture_ALL_ATMs_By_Selection_2_Actions(string InSignedId, string InOperator, string InSelectionCriteria, DateTime InFromDt,
                // DateTime InToDt, int InMode)
                WSelectionCriteria = "";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection_2_Actions(WSignedId,WOperator ,WSelectionCriteria, WFromDt, WToDt, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                ShowGrid_4();
            }

            if (WMode == 2)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ProcessMode IN ( 1, 2) AND ReconcAtRMCycle =" + WRMCycleNo  ;
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator,WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL ATMS REPLENISHED THIS CYCLE :.."+ WRMCycleNo.ToString(); 

                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                ShowGrid();
            }

          

            if (WMode == 3)
            {
               // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE AtmNo ='"+WAtmNo +"' AND SesNo=" + WReplCycleNo;
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "REPLENISHEMENT FOR ATM :.."+WAtmNo+ " AND REPL CYCLE :.." + WReplCycleNo.ToString();
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
            }
            // ALL with Process Mode = 0 
            if (WMode == 4)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ProcessMode IN ( 0, -5, -6) ";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId,  WOperator,WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose(); 
                    return; 
                }

                labelHeaderLeft.Text = "CURRENTLY OUTSTANDING TO BE REPLENISHED";
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
            }
            // ALL with Process Mode = 0 
            if (WMode == 5)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ATMNo='"+WAtmNo+"'";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL REPLENISHMENTS FOR THIS ATM.."+ WAtmNo;
                //labelActions.Hide();
                //panelActions.Hide(); // Hide Actions

                //buttonActionsThisCycle.Hide();
                //buttonTXNsThisCycle.Hide();

                ShowGrid();
            }

            // ALL For this ATM By date Range 
            if (WMode == 35)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ATMNo='" + WAtmNo + "'";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria
                        , WFromDt, WToDt, WMode);

                                    if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL CYCLES for."+ WAtmNo+ " FROM.." + WFromDt.ToShortDateString()
                                            + "..TO.." + WToDt.ToShortDateString();
                ;
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
            }

            // ALL Outstanding By date Range 
            if (WMode == 6)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ATMNo='" + WAtmNo + "'";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria,WFromDt,WToDt, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL OUTSTANDING CYCLES FROM.." + WFromDt.ToShortDateString()
                                            + "..TO.." + WToDt.ToShortDateString(); 
                                                            ;
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
            }

            if (WMode == 7)
            {
                //  (Maker = 'Panicos' OR Authoriser = 'Panicos')  ";
                WSelectionCriteria = " WHERE ProcessMode IN ( 1, 2) AND ReconcAtRMCycle =" + WRMCycleNo
                    + " AND ( Maker='" + WSignedId + "' OR Authoriser ='" + WSignedId + "')";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL ATMS REPL/AUTH THIS CYCLE :.." + WRMCycleNo.ToString() + "..By.." + WSignedId;

                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                ShowGrid();
            }
            if (WMode == 8)
            {
                //  (Maker = 'Panicos' OR Authoriser = 'Panicos')  ";
                WSelectionCriteria = "";
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "ALL ATMS REPL/AUTH THIS CYCLE :.." + WRMCycleNo.ToString() + "..By.." + WSignedId;

                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                ShowGrid();
            }

            if (WMode == 9)
            {
                // +" ORDER By AtmNo , SesNo  ";
                WSelectionCriteria = " WHERE ProcessMode IN ( 0, -5, -6) AND LoadedAtRMCycle="+ WRMCycleNo;
                Aoc.ReadRichPicture_ALL_ATMs_By_Selection(WSignedId, WOperator, WSelectionCriteria, NullPastDate, NullPastDate, WMode);

                if (Aoc.IsSemaphore == true)
                {
                    this.Dispose();
                    return;
                }

                labelHeaderLeft.Text = "OUTSTANDING TO BE REPLENISHED For this Cycle:.."+WRMCycleNo.ToString();
                labelActions.Hide();
                panelActions.Hide(); // Hide Actions

                buttonActionsThisCycle.Hide();
                buttonTXNsThisCycle.Hide();

                ShowGrid();
            }

            textBoxUnload.Text =Aoc.TotalUnload.ToString("#,##0.00");
            textBoxTotalDeposits.Text = Aoc.TotalDeposits.ToString("#,##0.00");
            textBoxLoad.Text = Aoc.Totalload.ToString("#,##0.00");
            
            textBoxTExcess.Text = Aoc.TotalExcess.ToString("#,##0.00");
            textBoxTShortage.Text = Aoc.TotalShortages.ToString("#,##0.00");
            textBoxTDispute.Text = Aoc.TotalDisputeShortages.ToString("#,##0.00");

        }

        // On Row Enter
        string WWAtmNo;
        int WWRepl_No;

        DateTime SM_DATE ;
        DateTime Rec_DATE ;
        string Maker ;
        string Auth ;
        string Cit_Id ;

        // Row Enter
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WWAtmNo = (string)dataGridView2.Rows[e.RowIndex].Cells["ATM_No"].Value;
            WWRepl_No = (int)dataGridView2.Rows[e.RowIndex].Cells["Repl_No"].Value;

            
            //SM_DATE = (DateTime)dataGridView2.Rows[e.RowIndex].Cells["SM_DATE"].Value;
            //Rec_DATE = (DateTime)dataGridView2.Rows[e.RowIndex].Cells["Rec_DATE"].Value;
            //Maker = (string)dataGridView2.Rows[e.RowIndex].Cells["Maker"].Value;
            //Auth = (string)dataGridView2.Rows[e.RowIndex].Cells["Auth"].Value;
            //Cit_Id = (string)dataGridView2.Rows[e.RowIndex].Cells["Cit_Id"].Value;
    
            ShowLineDetails(); 
           
        }

        // Show Grid

        private void ShowGrid()
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
            //TxnsTableAllCycles_Details.Columns.Add("ATM_No", typeof(string));
            //TxnsTableAllCycles_Details.Columns.Add("Repl_No", typeof(int));
            //TxnsTableAllCycles_Details.Columns.Add("ProcessMode", typeof(int));

            //TxnsTableAllCycles_Details.Columns.Add("SM_DATE", typeof(DateTime)); // SesDtTimeEnd

            //TxnsTableAllCycles_Details.Columns.Add("Cit_Id", typeof(string)); // CIT
            //TxnsTableAllCycles_Details.Columns.Add("Group", typeof(string)); // Group
            //TxnsTableAllCycles_Details.Columns.Add("Owner", typeof(string)); // Owner


            // GRID
            dataGridView2.Columns[0].Width = 70; // ATM_No
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 70; // Repl_No
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 50; // ProcessMode
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 100; // CYCLE START DATE 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].HeaderText = "Repl_Date";

            dataGridView2.Columns[4].Width = 100; // SM_DATE
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].HeaderText = "Repl_Date";

            dataGridView2.Columns[5].Width = 70; // Cit_Id
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 70; // Group
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[7].Width = 70; // Owner
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[2].Width = 60; //CB_TXNs
            //dataGridView2.Columns[2].DefaultCellStyle.Format = "#,###";
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[3].Width = 120; // CB_TXNs_AMT 
            //dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }

        private void ShowGrid_4()
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
            //TxnsTableAllCycles_Details.Columns.Add("ATM_No", typeof(string));
            //TxnsTableAllCycles_Details.Columns.Add("Repl_No", typeof(int));
            //TxnsTableAllCycles_Details.Columns.Add("ProcessMode", typeof(int));

            //TxnsTableAllCycles_Details.Columns.Add("SM_DATE", typeof(DateTime)); // SesDtTimeEnd

            //TxnsTableAllCycles_Details.Columns.Add("Cit_Id", typeof(string)); // CIT
            //TxnsTableAllCycles_Details.Columns.Add("Group", typeof(string)); // Group
            //TxnsTableAllCycles_Details.Columns.Add("Owner", typeof(string)); // Owner


            // GRID
            //dataGridView2.Columns[0].Width = 70; // ATM_No
            //dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[1].Width = 70; // Repl_No
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[2].Width = 50; // ProcessMode
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Columns[3].Width = 100; // CYCLE START DATE 
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Columns[3].HeaderText = "Repl_Date";

            //dataGridView2.Columns[4].Width = 100; // SM_DATE
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Columns[4].HeaderText = "Repl_Date";

            //dataGridView2.Columns[5].Width = 70; // Cit_Id
            //dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[6].Width = 70; // Group
            //dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[7].Width = 70; // Owner
            //dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView2.Columns[2].Width = 60; //CB_TXNs
            //dataGridView2.Columns[2].DefaultCellStyle.Format = "#,###";
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[3].Width = 120; // CB_TXNs_AMT 
            //dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }

        private void ShowLineDetails()
        {
            Aoc.ReadActionsOccurancesTo_RichPicture_One_ATM(WWAtmNo, WWRepl_No);



            textBoxATMNo.Text = WWAtmNo;
            textBoxReplCycle.Text = WWRepl_No.ToString();

            textBoxSM_Date.Text = SM_DATE.ToShortDateString();
            textBoxRec_Date.Text = Rec_DATE.ToShortDateString();
            textBoxMaker.Text = Maker;
            textBoxAuth.Text = Auth;
            textBoxCit_Id.Text = Cit_Id;

            textBoxWaitForDisputeNo.Text = Aoc.WaitForDisputeNo.ToString();
            textBoxWaitForDisputeAmt.Text = Aoc.WaitForDisputeAmt.ToString("#,##0.00");

            textBoxWaitAndOpenDisputeNo.Text = Aoc.WaitAndOpenDisputeNo.ToString();
            textBoxWaitAndOpenDisputeAmt.Text = Aoc.WaitAndOpenDisputeAmt.ToString("#,##0.00");

            textBoxWaitAndSettledDisputeNo.Text = Aoc.WaitAndSettledDisputeNo.ToString();
            textBoxWaitAndSettledDisputeAmt.Text = Aoc.WaitAndSettledDisputeAmt.ToString("#,##0.00");

            textBoxNoWaitDisputeNo.Text = Aoc.NoWaitDisputeNo.ToString();
            textBoxNoWaitDisputeAmt.Text = Aoc.NoWaitDisputeAmt.ToString("#,##0.00");

            textBoxNoWaitSettledDisputeNo.Text = Aoc.NoWaitSettledDisputeNo.ToString();
            textBoxNoWaitSettledDisputeAmt.Text = Aoc.NoWaitSettledDisputeAmt.ToString("#,##0.00");

            // CIT Opening Balance 
            textBoxCIT_Excess.Text = Aoc.Excess.ToString("#,##0.00");
            textBoxCIT_Shortage.Text = Aoc.CIT_Shortage.ToString("#,##0.00");

            // CURRENT Excess and Shortage 
            textBoxExcessBal.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
            textBoxShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");

            textBoxReturns.Text = Aoc.CIT_Returns.ToString("#,##0.00");

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

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

           // string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
// All Accounting 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {
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
        private void buttonAction_Click(object sender, EventArgs e)
        {
            int WRowIndex = dataGridView2.SelectedRows[0].Index;
            bool ValidAction = false;

            RRDMActions_GL Ag = new RRDMActions_GL();
            Ag.ReadActionByActionNm(WOperator, comboBoxActionNm.Text);

            WActionId = Ag.ActionId;

            if (comboBoxReasonOfAction.Text == "Select Reason")
            {
                MessageBox.Show("Please Select Reason For taking this Action");
                return;
            }

            decimal DoubleEntryAmt = 0; 

            // FIRST DOUBLE ENTRY 
            if (decimal.TryParse(textBoxEnteredAmt.Text, out DoubleEntryAmt))
            {
                MessageBox.Show("Amount of : " + DoubleEntryAmt.ToString("#,##0.00")+Environment.NewLine
                    + "Will Be:" + Environment.NewLine
                    + comboBoxActionNm.Text
                        );
            }
            else
            {
                MessageBox.Show(textBoxEnteredAmt.Text, "Please enter a valid number!");

                return;
            }

            //            79_CREDIT_CIT SHORTAGE GL/DR_EXCESS GL
            //            86_CREDIT Branch Shortage / DEBIT_CIT Account
            //85_CREDIT Branch Shortage / DEBIT_Branch_Profit & Loss
            //84_DEBIT Branch Excess / CREDIT_Branch_Profit & Loss
            //textBoxExcessBal.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
            //textBoxShortage.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");

            if (WActionId == "86" || WActionId == "85" || WActionId == "79")
            {
                // Input amount cannot be creater than Shortage 
                if (DoubleEntryAmt > -(Aoc.Current_ShortageBalance))
                {
                    MessageBox.Show("Input Amount is creater than Shortage");
                    return; 
                }
            }
            if (WActionId == "84" )
            {
                // Input amount cannot be creater than Shortage 
                if (DoubleEntryAmt > Aoc.Current_ExcessBalance)
                {
                    MessageBox.Show("Input Amount is creater than Excess");
                    return;
                }
            }

            //textBoxATMNo.Text = WWAtmNo;
            //textBoxReplCycle.Text = WRepl_No.ToString();
            int WUniqueRecordId = WWRepl_No; // SesNo 
            string WCcy = "EGP";
            //DoubleEntryAmt = Na.Balances1.CountedBal;
            string WMaker_ReasonOfAction = comboBoxReasonOfAction.Text;
            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                  WActionId, WUniqueRecordIdOrigin,
                                                  WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                  WWAtmNo, WWRepl_No, WMaker_ReasonOfAction, "Replenishment");

            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            // UPDATE OCCURANCES FOR REPLENISHMENT RECORDS AS CONFIRMED
            // For done within this cycle 

            string WStage = "02"; // Confirmed by maker 
            Aoc.UpdateOccurancesStage("Replenishment", WWRepl_No, WStage, DateTime.Now, WRMCycleNo, WSignedId);
            // Also Authoriser 
            // Also Update Stage as "03"
            Aoc.UpdateOccurancesForAuthoriser("Replenishment", WWRepl_No, WSignedId, 999, WSignedId);

            MessageBox.Show("Action with Id:.." + WActionId + Environment.NewLine
                    + "Has Been Created");

            //  Make the Check 
            // UPDATE Ta

            Aoc.ReadActionsOccurancesTo_RichPicture_One_ATM(WWAtmNo, WWRepl_No);

            if (Aoc.Current_ShortageBalance < 0
                || Aoc.WaitForDisputeNo < Aoc.WaitAndSettledDisputeNo
                || Aoc.NoWaitDisputeNo < Aoc.NoWaitSettledDisputeNo)
            {
                Ta.ReadSessionsStatusTraces(WWAtmNo, WWRepl_No);
                Ta.Repl1.DiffRepl = true;
                Ta.UpdateSessionsStatusTraces(WWAtmNo, WWRepl_No);
                if (Aoc.Current_ShortageBalance < 0)
                {
                    MessageBox.Show("There is Shortage remaining " + Environment.NewLine
                   + "");
                }
                else
                {
                    MessageBox.Show("There is still difference in Disputes " + WActionId + Environment.NewLine
                   + "");
                }
                
            }
            else
            {
                Ta.ReadSessionsStatusTraces(WWAtmNo, WWRepl_No);
                Ta.Repl1.DiffRepl = false;
                Ta.UpdateSessionsStatusTraces(WWAtmNo, WWRepl_No);
            }

            // Show NEW Line Details
            ShowLineDetails();
            
        }
        // UNDO ACTION
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            MessageBox.Show("System will Undo any Action with Id 79, 84,85,86" + Environment.NewLine
                          + " Done For this Cycle.." + WWRepl_No
                          );

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt("Replenishment", WWRepl_No, "79");
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt("Replenishment", WWRepl_No, "84");
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt("Replenishment", WWRepl_No, "85");
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt("Replenishment", WWRepl_No, "86");

            // Show NEW Line Details
            ShowLineDetails();

        }

        // EXPORT TO EXCEL
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = "Outstanding for CIT".ToString();

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Aoc.TxnsTableAllCycles_Details, WorkingDir, ExcelPath);
        }

        //
        // Catch Details
        //
        private static void CatchDetails(Exception ex, bool InIgnore)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            if (InIgnore == true)
            {
                // Do Nothing 
            }
            else
            {
                Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

            }

        }
// Actions this Cycle 
        private void buttonActionsThisCycle_Click(object sender, EventArgs e)
        {
            //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
            string WSelectionCriteria = "WHERE ActionId IN ('84','85', '86', '79') " 
                + " AND RMCycle =" + WRMCycleNo ;

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
    }
}


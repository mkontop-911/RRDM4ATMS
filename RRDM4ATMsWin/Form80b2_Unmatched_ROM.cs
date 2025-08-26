using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80b2_Unmatched_ROM : Form
    {
        //RRDMReconcMatchedUnMatchedVisaAuthorClass Mpa = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 
       // RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingTxns_MasterPoolATMs_ROM Mpa = new RRDMMatchingTxns_MasterPoolATMs_ROM();

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        RRDMReconcJobCycles Dj = new RRDMReconcJobCycles();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;
        // NOTES END 

        string WSortCriteria;

        int WUniqueNo;
        string WRRN;

        DateTime FromDt;
        DateTime ToDt;

        //int WMaskRecordId;
        string WInputField;

        string WFilter1;
        //string WFilter2;
        string WFilterFinal;

        string WSelectionString;

        bool FirstCycle;

        string WMask;
        string WSubString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        //string WCategoryId;
        int WRMCycleNo;
        //int WInMaskRecordId;
        string WFunction;

        string WTerminalId;
        DateTime WDateFrom;
        DateTime WDateTo;

        int WSelection;

        public Form80b2_Unmatched_ROM
            (string InSignedId, int InSignRecordNo, string InOperator, string InFunction,
            int InRMCycleNo, string InTerminalId, DateTime InDateFrom, DateTime InDateTo, int InSelection)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WFunction = InFunction; // "View"
            WRMCycleNo = InRMCycleNo; 
            WTerminalId = InTerminalId;
            WDateFrom = InDateFrom;
            WDateTo = InDateTo;

            WSelection = InSelection; // 1 is unmatched this cycle
                                      // 2 Unmatched for ATM for certain dates and time used for replenishment
                                      // 3 Presenter Errors for ATM for certain dates and time used for replenishment
                                      // 4 Include SOLO 

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            //Select For this RMCycle 

            if (WRMCycleNo == 0 & WSelection == 1)
            {
                string WJobCategory = "ATMs";
                Dj.ReadLastReconcJobCycle_Closed_Cycle(WOperator, WJobCategory);
                WRMCycleNo = Dj.JobCycle;
            }

            //SHOW FOR THIS CYCLE
          

            if (WFunction == "View")
            {
                textBoxMsgBoard.Text = "View Only";
            }

            FirstCycle = true;
            // SELECT
            comboBoxFilter.Items.Add("UnMatched this Cycle");

            comboBoxFilter.Items.Add("UnMatched-All With No Action");

            comboBoxFilter.Items.Add("UnMatched-All In Process");

            comboBoxFilter.Items.Add("UnMatched Settled this Cycle");

            comboBoxFilter.Items.Add("UnMatched Settled All Cycles");

            comboBoxFilter.Items.Add("Not Passing Matching Process Yet");

            comboBoxFilter.Items.Add("UnMatched Or Solo at Replenishment Cycle");

            comboBoxFilter.Items.Add("Presenter Errors For Replenishment Cycle");

            comboBoxFilter.Items.Add("UnMatched Or Solo at Replenishment Cycle");
           

            if (WSelection == 1)
            {
                comboBoxFilter.Text = "UnMatched this Cycle";
            }
            if (WSelection == 2)
            {
                comboBoxFilter.Text = "UnMatched Or Solo at Replenishment Cycle";

                labelStep1.Text = "UnMatched TXNS for ATM.." + WTerminalId
                              + " From.." + WDateFrom.ToString()
                              + " To.." + WDateTo.ToString();

                label22.Hide();
                panel10.Hide();
                label16.Hide();
                panel4.Hide();
            }
            if (WSelection == 3)
            {
                comboBoxFilter.Text = "Presenter Errors For Replenishment Cycle";

                labelStep1.Text = "Presenter Error TXNS for ATM.." + WTerminalId
                              + " From.." + WDateFrom.ToString()
                              + " To.." + WDateTo.ToString();

                label22.Hide();
                panel10.Hide();
                label16.Hide();
                panel4.Hide();
            }
            if (WSelection == 4)
            {
                comboBoxFilter.Text = "UnMatched Or Solo at Replenishment Cycle";

                labelStep1.Text = "UnMatched Or SOLO TXNS for ATM.." + WTerminalId
                              + " From.." + WDateFrom.ToString()
                              + " To.." + WDateTo.ToString();

                label22.Hide();
                panel10.Hide();
                label16.Hide();
                panel4.Hide();
            }




            // SORT 

            comboBoxSort.Items.Add("CardNo");

            comboBoxSort.Items.Add("AccountNo");

            comboBoxSort.Items.Add("SeqNo");

            comboBoxSort.Text = "SeqNo";

            //TEST

            panel5.Hide();

            if (WSelection == 1)
            {
                WSelectionString = " WHERE IsMatchingDone = 1 AND Matched = 0 AND MatchingAtRMCycle =" + WRMCycleNo;
                Mpa.ReadUnMatchedTxnsMasterPoolATMsTotals(WSelectionString, 1);

                textBox4.Text = Mpa.TotalUnMatched.ToString();
                textBox1.Text = Mpa.TotalUnMatchedWithNoAction.ToString();
                textBox2.Text = Mpa.TotalUnMatchedInProcess.ToString();
                textBox3.Text = Mpa.TotalUnMatchedSettled.ToString();

            }

            FirstCycle = false;
        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {

            if (checkBoxUnique.Checked == false) // Not Unique 
            //if (comboBoxUnique.Text == "NoUnique")
            {
                if (comboBoxSort.Text == "UniqueNo") WSortCriteria = " ORDER BY MaskRecordId DESC";
                if (comboBoxSort.Text == "SeqNo") WSortCriteria = " ORDER BY SeqNo DESC";
                if (comboBoxSort.Text == "AccountNo") WSortCriteria = " ORDER BY AccNumber DESC";

                if (comboBoxFilter.Text == "UnMatched this Cycle")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 1 AND Matched = 0 AND MatchingAtRMCycle = " + WRMCycleNo;
                }
                //Mpa.SeqNo06 = 95
                if (comboBoxFilter.Text == "UnMatched Or Solo at Replenishment Cycle")
                {
                    WFilter1 = " WHERE (IsMatchingDone = 1 AND Origin='Our Atms' AND ( (Matched = 0 AND TXNSRC='1') OR (SeqNo06 = 95)"
                        + " OR (Matched =1 AND  MetaExceptionId = 55)))"
                                + "AND TerminalId ='" + WTerminalId + "'  ";
                }

                if (comboBoxFilter.Text == "Presenter Errors For Replenishment Cycle")
                {
                   // WFilter1 = " WHERE IsMatchingDone = 1 AND Matched = 1 AND "
                        WFilter1 = " WHERE  "
                                        + " TerminalId ='" + WTerminalId
                                        + "' AND  MetaExceptionId = 55  ";

                }

                if (comboBoxFilter.Text == "UnMatched-All With No Action")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 1 AND Matched = 0 AND ActionType = '00' ";
                }

                if (comboBoxFilter.Text == "UnMatched-All In Process")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 1 AND Matched = 0 AND ActionType != '00' AND SettledRecord = 0 ";
                }

                if (comboBoxFilter.Text == "UnMatched Settled this Cycle")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 1 AND Matched = 0 AND ActionType != '00' AND SettledRecord = 1 AND MatchingAtRMCycle = " + WRMCycleNo;
                }

                if (comboBoxFilter.Text == "UnMatched Settled All Cycles")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 1 ";
                }

                if (comboBoxFilter.Text == "Not Passing Matching Process Yet")
                {
                    WFilter1 = " Where Operator ='" + WOperator + "' AND IsMatchingDone = 0 AND Matched = 0 ";
                }

            }
            else
            {
                // Unique
                if (radioButtonCard.Checked == false & radioButtonAccount.Checked == false & radioButtonUniqueNo.Checked == false)
                {
                    MessageBox.Show("Please select and Continue ");
                    return;
                }

                if (textBoxInputField.Text == "")
                {
                    if (WFunction != "Investigation")
                        MessageBox.Show("Please enter value!");
                    return;
                }

                WInputField = textBoxInputField.Text;

                if (radioButtonCard.Checked == true) // Card 
                {
                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND CardNumber ='" + WInputField + "' AND Matched = 0 ";  // Only open
                }

                if (radioButtonAccount.Checked == true) // Account 
                {
                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND AccNumber ='" + WInputField + "' AND Matched = 0 ";  // Only open
                }

                if (radioButtonUniqueNo.Checked == true) // UniqueNumber  
                {
                    WFilter1 = " Where Operator ='" + WOperator + "'" + " AND MaskRecordId ='" + WInputField + "' AND Matched = 0 ";  // Only open
                }

            }

            // FILL TABLE AND SHOW
            //if (radioButtonOnlyThisCatandCycle.Checked == true) // Only this Categ and Cycle both
            //{
            WFilterFinal = WFilter1;

            //WSortCriteria = "";

            //No Dates Are selected

            if (WSelection == 1)
            {
                FromDt = NullPastDate;
                ToDt = NullPastDate;
            }

            if (WSelection == 2 || WSelection == 3 || WSelection == 4)
            {
                FromDt = WDateFrom;
                ToDt = WDateTo;
            }

            int Mode = 1;

            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, Mode, WFilterFinal, WSortCriteria, FromDt, ToDt, 2);

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                
                MessageBox.Show("No unmatched transactions for this cycle... " + WRMCycleNo);

                panel7.Hide();
                panel21.Hide();
                label11.Hide();
                textBoxMask.Hide();
                this.Dispose();
                return;
            }
            else
            {
                panel7.Show();
                panel21.Show();
                label11.Show();
                textBoxMask.Show();

                //TEST
                if (textBoxInputField.Text == "9962224889" & dataGridView1.Rows.Count > 2)
                {
                    int WRow1 = 3;
                    dataGridView1.Rows[WRow1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
                }

                textBoxLines.Text = dataGridView1.Rows.Count.ToString();

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";
                
                dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

                dataGridView1.Columns[0].Width = 40; // MaskRecordId
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Visible = false;

                dataGridView1.Columns[1].Width = 40; // Status
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 40; //  Done
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[3].Width = 40; //  Action
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[4].Width = 70; // Terminal
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[5].Width = 50; // Terminal Type, ATM, POS etc 
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 90; // Descr
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[7].Width = 40; // Err
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[8].Width = 40; // Mask
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[9].Width = 90; // Account
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[10].Width = 50; // Ccy
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[11].Width = 80; // Amount
                dataGridView1.Columns[11].DefaultCellStyle = style;
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.Red;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

                dataGridView1.Columns[12].Width = 140; // Date
                dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[13].Width = 70; // Trace
                dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[14].Width = 90; // RRNumber
                dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[15].Width = 50; // Trans Type
                dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[15].HeaderText = "Trans Type";

                FirstCycle = false;
            }
        }

        // On ROW ENTER 
        // On ROW ENTER 
        int WWUniqueRecordId;
        bool IsReversal;
        bool JournalFound;
        string Message;
        string DiscrepancyMessage;

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WUniqueNo = (int)rowSelected.Cells[0].Value;
            WFilterFinal = " Where  UniqueRecordId = " + WUniqueNo;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WFilterFinal, 1);

            if (Mpa.Origin == "Our Atms")
            {
                button3.Show();
                button4.Show();
                button5.Show();
                buttonPOS.Hide();

                IsReversal = false;
                Message = "";

                RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

                Jt.ReadJournalTextByTrace(WOperator, Mpa.TerminalId, Mpa.MasterTraceNo, Mpa.TransDate.Date);

                if (Jt.RecordFound == true)
                {
                    JournalFound = true;

                    Message = "";
                }
                else
                {
                    JournalFound = false;
                }


            }
            else
            {
                // JCC 
                button3.Hide();
                button4.Hide();
                button5.Show();
                buttonPOS.Show();
            }


            IsReversal = false;

            DiscrepancyMessage = "";
            // 
            if (Mpa.MatchMask.Contains("R") == true)
            {
                DiscrepancyMessage = "There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";
                if (Mpa.MetaExceptionId == 55 )
                {
                    DiscrepancyMessage = "Presenter + There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";
                }
                

                IsReversal = true;
            }

            // NOTES START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // NOTES END

            WWUniqueRecordId = Mpa.UniqueRecordId;

            textBox10.Text = Mpa.RMCateg;
            textBox11.Text = Mpa.TerminalId;

            textBoxRmCycle.Text = Mpa.MatchingAtRMCycle.ToString();
            textBoxCardNo.Text = Mpa.CardNumber;
            textBoxAccNo.Text = Mpa.AccNumber;
            textBoxCurr.Text = Mpa.TransCurr;
            textBoxAmnt.Text = Mpa.TransAmount.ToString("#,##0.00");
            textBoxDtTm.Text = Mpa.TransDate.ToString();
            textBoxTraceNo.Text = Mpa.TraceNoWithNoEndZero.ToString();
            textBoxRRNumber.Text = Mpa.RRNumber.ToString();
            textBoxTXNSRC.Text = Mpa.TXNSRC;
            textBoxTXNDEST.Text = Mpa.TXNDEST;

            if (Mpa.Origin == "Our Atms")
            {
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);
                if (Ta.RecordFound & Ta.ProcessMode > 0)
                {
                    buttonReplPlay.Enabled = true;
                    buttonReplPlay.BackColor = Color.White;
                }
                else
                {
                    buttonReplPlay.Enabled = false;
                    buttonReplPlay.BackColor = Color.Silver;
                }

                button3.Show();
                button4.Show();
                button5.Show();
                label26.Show();
                label25.Show();
                label19.Show();

                //Tp.ReadInPoolTransSpecific(Mpa.OriginalRecordId); // Read Transactions details 
                //ATMTrans = true;
            }
            else
            {
                //buttonReplPlay.Hide();
                buttonReplPlay.Enabled = false;
                buttonReplPlay.BackColor = Color.Silver;
                button3.Hide();
                button4.Hide();
                button5.Hide();
                label26.Hide();
                label25.Hide();
                label19.Hide();
                //ATMTrans = false;
            }

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            if (Rcs.EndReconcDtTm == NullPastDate)
            {
                buttonReconcPlay.Enabled = false;
                buttonReconcPlay.BackColor = Color.Silver;
            }
            else
            {
                buttonReconcPlay.Enabled = true;
                buttonReconcPlay.BackColor = Color.White;
            }

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, "EWB110", Mpa.MatchingAtRMCycle);
            if (Rcs.EndReconcDtTm == NullPastDate)
            {
                //buttonReconcCash.Enabled = false;
                //buttonReconcCash.BackColor = Color.Silver;
            }
            else
            {
                //buttonReconcCash.Enabled = true;
                //buttonReconcCash.BackColor = Color.White;
            }

            textBoxMask.Text = Mpa.MatchMask;

            // Check for exceptions 
            if (Mpa.ActionType != "00")
            {
                labelAction.Show();
                panel3.Show();

                //Ec.ReadErrorsTableSpecific(Mpa.MetaExceptionNo);
            }
            else
            {
                labelAction.Hide();
                panel3.Hide();
            }

            Tp.ReadTransToBePostedSpecificByUniqueRecordId(Mpa.UniqueRecordId);
            if (Tp.RecordFound == true)
            {
              //  label27.Show();
                panel8.Show();

                textBoxCreated.Text = Tp.OpenDate.ToString();
                if (Tp.ActionDate != NullPastDate) textBoxPosted.Text = Tp.ActionDate.ToString();
                else textBoxPosted.Text = "Not Posted yet.";
            }
            else
            {
               // label27.Hide();
                panel8.Hide();
            }

            if (Mpa.ActionType == "04")
            {
               // label27.Show();
                panelForceMatched.Show();
                textBoxForceReason.Text = Mpa.MatchedType;

            }
            else
            {
                panelForceMatched.Hide();
            }

            if (Mpa.Matched == false)
            {
                if (Message == "")
                    textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                else textBoxUnMatchedType.Text = Message;
            }
            else
            {
                textBoxUnMatchedType.Text = "Matched";
                if (Mpa.MetaExceptionId == 55)
                {
                    textBoxUnMatchedType.Text = textBoxUnMatchedType.Text + " But Presenter error"; 
                }
            }
            
            if (Mpa.UnMatchedType == "DUPLICATE")
            {
                if (Mpa.MatchMask == "") WMask = "000";
                else WMask = Mpa.MatchMask;
            }
            else
            {
                WMask = Mpa.MatchMask;
            }

            ShowMask(WMask);

            if (DiscrepancyMessage == "")
            {
                textBoxUnMatchedType.Text = Mpa.UnMatchedType;
                if (Mpa.Matched == true)
                {
                    textBoxUnMatchedType.Text = "Matched";
                    if (Mpa.MetaExceptionId == 55)
                    {
                        textBoxUnMatchedType.Text = textBoxUnMatchedType.Text + " But Presenter error";
                    }
                }
                
                //else textBoxUnMatchedType.Text = "Not Matched";
            }
            else textBoxUnMatchedType.Text = DiscrepancyMessage;

            WRRN = Mpa.RRNumber;

            // Check if dispute already registered for this transaction 

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            // Check if already exist
            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                textBox9.Text = Dt.DisputeNumber.ToString();
                //WFoundDisp = Dt.DisputeNumber;
                if (WFunction == "Investigation" & WWUniqueRecordId == 0)
                    MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                labelDispute.Show();
                textBox9.Show();
                buttonRegisterDispute.Hide();
            }
            else
            {
                labelDispute.Hide();
                textBox9.Hide();
                if (WFunction == "Investigation")
                    buttonRegisterDispute.Show();
            }


            textBoxInputField.Text = Mpa.CardNumber;
        }
        //
        // Show info for Mask
        //
        public void ShowMask(string InMask)
        {

            //****************************************************************************
            //Translate MASK 
            //****************************************************************************

            WMask = InMask;

            if (WMask == "")
            {
                WMask = "EEE";
            }
            // First Line
            if (Mpa.FileId01 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Mpa.FileId01;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);

                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Mpa.FileId02 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Mpa.FileId02;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);

                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Mpa.FileId03 != "")
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Mpa.FileId03;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);

                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }

            // Forth Line 
            if (Mpa.FileId04 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Mpa.FileId04;
                labelFileD.Show();
                WSubString = WMask.Substring(3, 1);

                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Mpa.FileId05 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Mpa.FileId05;
                labelFileE.Show();
                WSubString = WMask.Substring(4, 1);


                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Mpa.FileId06 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Mpa.FileId06;
                labelFileF.Show();
                WSubString = WMask.Substring(5, 1);

                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
        }


        // Show Exception 
        private void buttonShowException_Click(object sender, EventArgs e)
        {
            Form24 NForm24;
            bool Replenishment = true;
            int ErrNo = Mpa.MetaExceptionNo;
            string SearchFilter = "ErrNo =" + ErrNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }
        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }
        string P1;
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print 

            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo
            if (checkBoxUnique.Checked == false)
            {
                P1 = comboBoxFilter.Text + " Transactions";
            }
            if (checkBoxUnique.Checked == true)
            {
                P1 = "Transactions for unique Selection:" + Mpa.UniqueRecordId.ToString();
            }

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R54ATMS ReportATMS54 = new Form56R54ATMS(P1, P2, P3, P4, P5);
            ReportATMS54.Show();
        }

        // SHOW Selection 
        private void buttonShowSelection_Click(object sender, EventArgs e)
        {
            //
            // Validation for invalid characters 
            //
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
              (@"^[a-zA-Z0-9]*$");

            if (expr.IsMatch(textBoxInputField.Text))
            {
                //   MessageBox.Show("field");
            }
            else
            {
                MessageBox.Show("invalid Characters In Input Field");
                return;
            }

            Form80b_Load(this, new EventArgs());

        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // ON COMBO CHANGE LOAD 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFilter.Text == "UnMatched this Cycle") labelStep1.Text = "UnMatched this Cycle : " + WRMCycleNo.ToString();
            if (comboBoxFilter.Text == "UnMatched-All With No Action") labelStep1.Text = "UnMatched-All With No Action";
            if (comboBoxFilter.Text == "UnMatched-All In Process") labelStep1.Text = "UnMatched-All In Process";
            if (comboBoxFilter.Text == "UnMatched Settled this Cycle") labelStep1.Text = "UnMatched Settled this Cycle : " + WRMCycleNo.ToString();
            if (comboBoxFilter.Text == "UnMatched Settled All Cycles") labelStep1.Text = "UnMatched Settled All Cycles";

            if (FirstCycle == false)
            {
                Form80b_Load(this, new EventArgs());
            }
        }



        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // Unique search 
        private void checkBoxUnique_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == true)
            {
                panel5.Show();
                panel9.Hide();
            }
            else
            {
                panel5.Hide();
                panel9.Show();

                radioButtonCard.Checked = false;
                radioButtonAccount.Checked = false;
                radioButtonUniqueNo.Checked = false;

                textBoxInputField.Text = "";
            }
        }

        // EXPAND GRID
        private void buttonExpandGridRight_Click(object sender, EventArgs e)
        {

        }
        // Show Trans to Be posted 
        private void buttonTranPosted_Click(object sender, EventArgs e)
        {
            Form78 NForm78;

            Mpa.ReadInPoolTransSpecificUniqueRecordId(Mpa.OriginalRecordId, 2);
            if (Mpa.RecordFound & Mpa.ActionType != "00")
            {
                NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
                                                    "", 0, Mpa.UniqueRecordId, 1);
                NForm78.ShowDialog();
            }

            else
            {
                MessageBox.Show("No Transactions/actions were taken for this.");
                return;
            }

        }
        // Replenishment Play 
        private void buttonReplPlay_Click(object sender, EventArgs e)
        {
            Form51 NForm51;
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View only for replenishment already done  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);
            if (Ta.RecordFound & Ta.ProcessMode > 0)
            {
                //
                // Find out if ATM is Recycling 
                //
                RRDMGasParameters Gp = new RRDMGasParameters();
                bool IsRecycle = false;

                string ParId2 = "948";
                string OccurId2 = "1"; // 
                                       //RRDMGasParameters Gp = new RRDMGasParameters(); 
                Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
                if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                {
                    RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                    SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(Mpa.TerminalId, Mpa.ReplCycleNo);
                    if (SM.RecordFound == true)
                    {
                        // Check if Reccyle 
                        if (SM.is_recycle == "Y")
                        {
                            IsRecycle = true;
                        }
                    }
                }

                if (IsRecycle == true)
                {
                    // Recycle Type 
                    Form51_Recycle NForm51_Recycle;
                    NForm51_Recycle = new Form51_Recycle(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    //NForm51_Recycle.FormClosed += NForm5_FormClosed;
                    NForm51_Recycle.ShowDialog();
                }
                else
                {
                    // Current Bank De Caire Type 
                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    //NForm51.FormClosed += NForm5_FormClosed;
                    NForm51.ShowDialog();
                }
                //NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                //NForm51.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Replenishement done for this ATM and this Replen Cycle");
                return;
            }

        }

        // PlayBack For reconciliation cash CASH 
        private void buttonReconcCash_Click(object sender, EventArgs e)
        {
            if (Mpa.RMCateg == "EWB102")
            {
                RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                Form71 NForm71;
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ProcessNo = 54; // View only for reconciliation already done  
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);
                if (Ta.RecordFound & Ta.ProcessMode > 1)
                {
                    NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo);
                    NForm71.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No Reconciliation done for this ATM and Replen Cycle");
                    return;
                }
            }

        }
        // Reconciliation Play 
        private void buttonReconcPlay_Click(object sender, EventArgs e)
        {
            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            if (Rcs.EndReconcDtTm == NullPastDate)
            {
                MessageBox.Show("Reconciliation Not done yet!");
                return;
            }


            // Update Us Process number
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54; // View Only 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form271 NForm271;

            NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
            //NForm271.FormClosed += NForm271_FormClosed;
            NForm271.ShowDialog(); ;


        }
        // Text From Journal 
        private void button3_Click(object sender, EventArgs e)
        {

            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + Mpa.UniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
            {
                MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                 + "Select Journal Lines Near To this"
                                 );
                return;
            }

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            if (Mpa.TraceNoWithNoEndZero == 0)
            {
                MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                return;
            }
            else
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;
                WSeqNoB = Mpa.OriginalRecordId;
            }

            //
            // Bank De Caire
            //
            Form67_ROM NForm67_ROM;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_ROM = new Form67_ROM(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_ROM.ShowDialog();
            //}
            //else
            //{
            //    Form67 NForm67;

            //    int Mode = 5; // Specific

            //    //NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, Mode);
            //    NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            //    NForm67.ShowDialog();
            //}


        }
        // Full Journal
        private void button4_Click(object sender, EventArgs e)
        {
            Form67 NForm67;
            RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            String JournalId = "";
            int WTraceNo = Mpa.AtmTraceNo;
            int Mode; // FULL

            //if (WOperator == "BCAIEGCX")
            //{
                int WSeqNoA = 0;
                int WSeqNoB = 0;

                Jt.ReadJournalTxnsByParameters(WOperator, Mpa.TerminalId, Mpa.AtmTraceNo*10, Mpa.TransAmount, Mpa.CardNumber, Mpa.TransDate.Date);

                if (Jt.RecordFound)
                {
                    WSeqNoA = Jt.SeqNo;
                    WSeqNoB = Jt.SeqNo;

                }
                else
                {
                    MessageBox.Show("There is no recognisable valid record in Journal" + Environment.NewLine
                                  + "Search in Journals to find the occurance!"
                                );

                    Form200cATMs NForm200cATMs;

                    NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, Mpa.TerminalId);
                    NForm200cATMs.ShowDialog();

                    return;
                }
                //
                // Bank De Caire
                //
                Form67_BDC NForm67_BDC;

                Mode = 3; // Specific Journal 
                string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
                if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
                return;
            //}


            //if (WOperator == "CRBAGRAA")
            //{
            //    JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            //    //Tp.ReadInPoolTransSpecific(Mpa.OriginalRecordId);

            //    //Mode = 3;

            //    //Ta.ReadSessionsStatusTraces(Mpa.TerminalId, Mpa.ReplCycleNo);

            //    //WTraceNo = Ta.FirstTraceNo;

            //    //Jt.ReadJournalTextByTrace(Mpa.Operator, Mpa.TerminalId, WTraceNo, Mpa.TransDate.Date);

            //    //int FileInJournal = Jt.FuId;

            //    //// WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            //    //NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, FileInJournal, Mpa.TerminalId, Ta.FirstTraceNo, Ta.LastTraceNo, Mpa.TransDate.Date,NullPastDate ,Mode);
            //    //NForm67.Show();
            //    if (Mpa.FuID > 0)
            //    {
            //        Mode = 3;
            //        NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, Mpa.FuID, Mpa.TerminalId, Ta.FirstTraceNo, Ta.LastTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            //        NForm67.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("There is no recognisable valid record in Journal" + Environment.NewLine
            //                       + "Search in Journals to find the occurance!"
            //                     );

            //        Form200cATMs NForm200cATMs;

            //        NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, Mpa.TerminalId);
            //        NForm200cATMs.ShowDialog();
            //    }

            //}
            //else
            //{
            //    if (Mpa.FuID > 0)
            //    {
            //        Mode = 3;
            //        NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, Mpa.FuID, Mpa.TerminalId, Ta.FirstTraceNo, Ta.LastTraceNo, Mpa.TransDate.Date, NullPastDate, Mode);
            //        NForm67.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("There is no recognisable valid record in Journal" + Environment.NewLine
            //                       + "Search in Journal to find the occurance!"
            //                     );

            //        Form200cATMs NForm200cATMs;

            //        NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, Mpa.TerminalId);
            //        NForm200cATMs.ShowDialog();
            //    }

            //}

        }
        // SHOW Video Clip
        private void button5_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
        // Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            Form5 NForm5;
            int From = 2; // Coming from Pre-Investigattion ATMs 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, Mpa.UniqueRecordId, 0, 0, "", From, "ATM");
            NForm5.ShowDialog();
            this.Dispose();
        }
        // Expand 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78b NForm78b;
            string WHeader = "LIST OF TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form80b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        // Source Records
        private void buttonSourceReords_Click(object sender, EventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles;

            //if (WSignedId == "1007_BDO")
            //{
            //    NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //    NForm78d_AllFiles.ShowDialog();
            //}
            //else
            //{
            //    NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //    NForm78d_AllFiles.ShowDialog();
            //}
            // Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
                //case "CRBAGRAA":
                //    {
                //        // DEMO MODE

                //        NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                //        NForm78d_AllFiles.ShowDialog();

                //        break;
                //    }
                //case "ETHNCY2N":
                //    {

                //        NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                //        NForm78d_AllFiles.ShowDialog();

                //        break;
                //    }
                //case "BCAIEGCX":
                //    {
                        NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId,1);

                        NForm78d_AllFiles_BDC_3.ShowDialog();
                    //    break;
                    //}
            //}

        }
        // POS information
        private void buttonPOS_Click(object sender, EventArgs e)
        {
            //if (Mpa.Origin == "Our Atms")
            //{
                MessageBox.Show("Not Allowed Operation");
                return;
            //}
            //if (WOperator == "BCAIEGCX")
            //{
            //    MessageBox.Show("Not Allowed Operation for this Bank");
            //    return;
            //}


            RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();

            bool WRecordFound =
                Msr.FindRawRecordFromMasterRecord
                  (WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle,
                  Mpa.TransDate.Date,
                   Mpa.TerminalId, Mpa.TraceNoWithNoEndZero, Mpa.RRNumber, 2);
            //(WOperator, Mpa.FileId01, Mpa.MatchingAtRMCycle, Mpa.TerminalId, Mpa.RRNumber, 2);

            string WHeader = "MERCHANT DETAILS FOR Transaction with RRN : " + Mpa.RRNumber.ToString();
            string WCardNumber = Mpa.CardNumber;
            string WAccNo = Mpa.AccNumber;
            string WDateTime = Mpa.TransDate.ToString();
            string WAmount = Mpa.TransAmount.ToString("#,##0.00");
            string WCcy = Mpa.TransCurr;
            string WAutorisationCd = Msr.AuthorisationId;
            string WMerchantId = Msr.MerchantId;
            string WMerchantName = Msr.MerchantNm;
            string WTranDescription = Msr.TranDescription;

            Form2Merchant NForm2Merchant;

            NForm2Merchant = new Form2Merchant(WOperator, WHeader, WCardNumber, WAccNo,
                                             WDateTime, WAmount, WCcy, WAutorisationCd,
                                                    WMerchantId, WMerchantName, WTranDescription);
            NForm2Merchant.Show();
        }
        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            WSubString = WMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            // Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //}

            string SelectionCriteria;
            int SaveSeqNo ;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = Mpa.SeqNo;

            DateTime WDtA = Mpa.TransDate;
            DateTime WDtB = Mpa.TransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-2);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";

            if (In_HST == true)
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
            }
            else
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            }

            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(2);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";


                if (In_HST == true)
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
                }
                else
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
                }

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");

                    WSeqNoB = 0;
                    // Reestablish Mpa Data
                    //
                    //SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    //return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                if (In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                }

                // Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            }

            //   Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (WSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }
        // ALL Actions

        private void buttonAllActions_Click(object sender, EventArgs e)
        {

            string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
            //*******************************************

        }
        // ALL Accounting 
        private void buttonAllAccounting_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            Aoc.ClearTableTxnsTableFromAction();

            string WSelectionCriteria = "WHERE UniqueKey =" + Mpa.UniqueRecordId + " AND UniqueKeyOrigin = 'Master_Pool' ";

            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                if (Aoc.Is_GL_Action == true)
                {

                    int WMode2 = 1; // DO NOT Create transaction in pool 
                    string WCallerProcess = "Reconciliation";
                    Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                 Aoc.ActionId, Aoc.Occurance, WCallerProcess, WMode2);
                }

                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;

            string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;

            // Aoc.ReadActionsTxnsCreateTableByUniqueKey(WUniqueRecordIdOrigin, Dt.UniqueRecordId, "All");

            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;
            //Form14b_All_Actions NForm14b_All_Actions;
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }
    }
}

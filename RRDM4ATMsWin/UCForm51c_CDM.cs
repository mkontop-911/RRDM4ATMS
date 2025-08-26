using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;

namespace RRDM4ATMsWin
{
    public partial class UCForm51c_CDM : UserControl
    {
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        Form24 NForm24; // Errors 

        Form67 NForm67; // Journal 

        Form5 NForm5; // Dispute form 

        string WActionId;

        //  DataTable DepositsTran = new DataTable();

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //int WErrNo;
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;

        int WUniqueRecordId;

        string ATMSuspence;
        string BranchExcess;
        string BranchIntermediary;
        string BranchSettlement;

        int WReconcCycleNo;

        int WErrId;

        int WTraceNo;

        string WBankId;

        bool ViewWorkFlow;

        //string SQLString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();
        //RRDMDepositsClass Da = new RRDMDepositsClass();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51c_CDM_Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (WReconcCycleNo == 0)
            {
                MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                return;
            }

            WBankId = Ac.BankId;

            try
            {
                // ................................
                // Handle View ONLY 
                //
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
                {
                    ViewWorkFlow = true;

                    //  buttonUpdate.Hide();
                }

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

            // Force Matching Reason
            Gp.ParamId = "714";
            comboBoxReasonOfAction.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReasonOfAction.DisplayMember = "DisplayValue";

            //TEST
            guidanceMsg = "Push Update and Move to Next step or Use Override!";

            if (ViewWorkFlow == true) guidanceMsg = "View Only!";
            int WMode = 1;

           
            WMode = 2;
            string TempCurrency = "818";
            
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            Mpa.ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, TempCurrency, WMode, 2);


            // Show Cash Deposits 
            textBoxTotalNumberDeposits.Text = Mpa.TotNoBNA.ToString();
            textBoxTotalDepositsValue.Text = Mpa.TotValueBNA.ToString("#,##0.00");

        }

        public void SetScreen()
        {
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int Mode = 1;
            //string SelectionCriteria = " WHERE IsMatchingDone = 1 AND Matched = 1 "
            //                    + "AND TerminalId ='" + WAtmNo
            //                    + "' AND ((MetaExceptionId = 55 "
            //                    + " OR MetaExceptionId = 225 OR MetaExceptionId = 226) "
            //                    + " OR ActionType <>'00') ";
            //string WSortCriteria = "";
            //Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, Mode,
            //    SelectionCriteria, WSortCriteria, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            dataGridView1.DataSource = Mpa.MatchingMasterDataTableATMs.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                dataGridView1.Hide();
                //textBox1.Text = "NOT FOUND ERRORS IN JOURNAL";
                return;
            }

            // Update STEP BASED ON TOTAL OUTSTANDINGs

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Mpa.TotalOutstanding == 0)
            {
                Usi.ReplStep4_Updated = true;
            }
            else
            {
                Usi.ReplStep4_Updated = false;
            }

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            ShowGrid();
            //dataGridView1.DataSource = Err.ErrorsTable.DefaultView;

            //if (dataGridView1.Rows.Count == 0)
            //{
            //    dataGridView1.Hide();
            //    textBox1.Text = "NOT FOUND ERRORS IN JOURNAL";
            //    return;
            //}



            //Show Errors 
            //ReadErrorsAndFillTable(string InOperator, string InUser, string InFilter)
            //string WFilter = " AtmNo ='" + WAtmNo + "' AND SesNo =" + WSesNo + " AND (ErrId = 55 OR ErrId = 225 OR ErrId = 226)";
            //Er.ReadErrorsAndFillTable(WOperator, WSignedId, WFilter);

            //if (Er.RecordFound == true)
            //{
            //    dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

            //    ShowGrid();
            //}
            //else
            //{
            //    MessageBox.Show("No ERRORS");
            //    return;
            //}

        }

        // ROW ENTER 
        int WErrNo;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WUniqueRecordId = (int)rowSelected.Cells[0].Value;
            // Read Txn 
            Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId, 2);

            WActionId = Mpa.ActionType;
            //
            ShowNote_3();
            //
            textBoxTxnType.Text = Mpa.TransDescr;
            textBoxMachineType.Text = Mpa.TransTypeAtOrigin;
            textBoxTrace.Text = Mpa.TraceNoWithNoEndZero.ToString();
            textBoxCcy.Text = Mpa.TransCurr;
            textBoxTxnAmt.Text = Mpa.TransAmount.ToString("#,##0.00");
            textBox14.Text = Mpa.MatchMask;
            if (Mpa.TransType > 20)
            {
                textBoxInvalid.Text = (Mpa.TransAmount - Mpa.DepCount).ToString("#,##0.00");
            }
            else
            {
                textBoxInvalid.Text = Mpa.DepCount.ToString("#,##0.00");
            }

            textBoxTxnDate.Text = Mpa.TransDate.ToString();
            textBoxAccountNo.Text = Mpa.AccNumber;

            int Mode = 2; // only for this unique number
            Er.ReadAllErrorsTableToFindTotalsForSuspectAndFake(Mpa.TerminalId, Mpa.ReplCycleNo, Mpa.UniqueRecordId, Mode);

            // Read to find totals for this transaction 

            textBoxTotalSuspect.Text = Er.Total_ErrId_225.ToString();
            textBoxTotalFake.Text = Er.Total_ErrId_226.ToString();
            textBoxPresenter.Text = Er.Total_ErrId_55.ToString();

            Mode = 1; // only 
            Er.ReadAllErrorsTableToFindTotalsForSuspectAndFake(Mpa.TerminalId, Mpa.ReplCycleNo, 0, Mode);

            // Read to find totals for this transaction 

            textBoxCycleSusp.Text = Er.Total_ErrId_225.ToString();
            textBoxCycleFake.Text = Er.Total_ErrId_226.ToString();
            textBoxCyclePresenter.Text = Er.Total_ErrId_55.ToString();
            textBoxCycleSuspectValue.Text = Er.Total_ErrId_225_Value.ToString();
            textBoxCycleFakeValue.Text = Er.Total_ErrId_226_Value.ToString();
            textBoxCyclePresenterValue.Text = Er.Total_ErrId_55_Value.ToString();

            textBoxTotalOutstanding.Text = Er.TotalOutstanding.ToString();


            // 
            // Build the presenter error 
            Er.ReadErrorsTableSpecific(WErrNo);

            if (Mpa.MetaExceptionId == 55 || WActionId == "08")
            {
                //
                // THIS IS PRESENTER ERROR
                //
                if (Mpa.MetaExceptionId == 55)
                    labelSuspectAndFake.Text = "PRESENTER ERROR-IN JOURNAL";

                if (WActionId == "08")
                    labelSuspectAndFake.Text = "POSSIBLE PRESENTER ERROR-TO BE VERIFY";

                panelPresenter.Show();

                labelPresenter.Show();
                textBoxPresenter.Show();

                textBoxInvalid.Hide();
                labelFake.Hide();

                // NOTES START  
                Order = "Descending";
                WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
                if (Cn.RecordFound == true)
                {
                    labelNumberNotes3.Text = Cn.TotalNotes.ToString();
                }
                else labelNumberNotes3.Text = "0";


                linkLabelSourceRecords.Show();


                WTraceNo = Mpa.TraceNoWithNoEndZero;

                WErrId = Mpa.MetaExceptionId;

                labelErrDesc.Text = "Presenter Error";

                textBox23.Text = Mpa.TransAmount.ToString("#,##0.00");
                // textBox24.Text = Er.CurDes.ToString();

                // Get GL Accounts
                GetGLAtmAccounts();

                if (Mpa.TXNDEST == "1")
                {
                    textBoxCredit_1.Text = "CREDIT CUSTOMER ";

                    textBoxAccFirst.Text = Mpa.AccNumber;

                    textBox39.Text = "DEBIT BRANCH Intermediary ";
                    textBoxAccSecond.Text = BranchIntermediary;

                    // Action Id = 95
                }
                if (Mpa.TXNDEST != "1")
                {
                    textBoxCredit_1.Text = "CREDIT SETTLEMENT ACC ";

                    textBoxAccFirst.Text = BranchSettlement;

                    textBox39.Text = "DEBIT BRANCH EXCESS ";
                    textBoxAccSecond.Text = BranchExcess;
                    // Action Id = 96
                }


                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                textBoxExcessBal.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");

                Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    labelDisputeId.Show();

                    labelDisputeId.Text = "Dispute Id :" + Dt.DisputeNumber.ToString();

                }
                else
                {
                    labelDisputeId.Hide();
                }

                switch (Mpa.ActionType)
                {
                    case "00":
                        {
                            buttonApplyAction.Show();
                            pictureBoxTakeAction.Show();
                            radioSystemSuggest.Checked = false;
                            radioButtonMoveToDispute.Checked = false;
                            radioButtonNoAction.Checked = false;

                            buttonApplyAction.Show();
                            pictureBoxTakeAction.Hide();

                            buttonUndoAction.Hide();
                            pictureBoxActionDone.Hide();

                            break;
                        }
                    case "95":
                    case "96": // Customer credited 
                        {
                            buttonUndoAction.Show();
                            pictureBoxActionDone.Show();

                            radioSystemSuggest.Checked = true;
                            radioButtonMoveToDispute.Checked = false;
                            radioButtonNoAction.Checked = false;

                            buttonApplyAction.Hide();
                            pictureBoxTakeAction.Hide();

                            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, Mpa.ActionType);
                            comboBoxReasonOfAction.Text = Aoc.Maker_ReasonOfAction;
                            //comboBoxReasonOfAction.Text = Mpa.Comments;
                            // buttonPOS.Show();
                            //buttonPOS.Text = "Merchant Info";
                            break;
                        }
                    case "05":
                        {
                            // Dispute case
                            buttonUndoAction.Show();
                            pictureBoxActionDone.Show();

                            radioSystemSuggest.Checked = false;
                            radioButtonMoveToDispute.Checked = true;
                            radioButtonNoAction.Checked = false;

                            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, Mpa.ActionType);
                            comboBoxReasonOfAction.Text = Aoc.Maker_ReasonOfAction;

                            buttonApplyAction.Hide();
                            pictureBoxTakeAction.Hide();
                            break;
                        }
                    case "04":
                        {
                            // Do nothing case 
                            buttonUndoAction.Show();
                            pictureBoxActionDone.Show();

                            radioSystemSuggest.Checked = false;
                            radioButtonMoveToDispute.Checked = false;
                            radioButtonNoAction.Checked = true;

                            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, Mpa.ActionType);
                            comboBoxReasonOfAction.Text = Aoc.Maker_ReasonOfAction;

                            buttonApplyAction.Hide();
                            pictureBoxTakeAction.Hide();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                //}

            }
            else
            {

                panelPresenter.Hide();
                labelPresenter.Hide();
                textBoxPresenter.Hide();
            }
            //
            // DEPOSITS 
            //
            if (Er.ErrId == 225 || Er.ErrId == 226)
            {
                //  labelSuspectAndFake.Show();
                panelSuspectAndFake.Show();
                // Initialise
                radioButtonBad.Checked = false;
                radioButtonGood.Checked = false;

                textBoxSerialId.Text = Er.NoteSerialId;
                textBoxFaceValue.Text = Er.NoteFaceValue.ToString();

                if (Er.UnderAction == false)
                {
                    textBoxUserComment.Text = "";
                }

                if (Er.ErrId == 225)
                {
                    // 225 means suspect
                    labelSuspectAndFake.Text = "CHECK ON SUSPECT NOTE";
                    radioButtonBad.Text = "Note Cannot Be accepted - customer will debited";
                    radioButtonGood.Text = "Note is acceptable";
                }
                if (Er.ErrId == 226)
                {
                    // 226 means fake
                    labelSuspectAndFake.Text = "CHECK ON FAKE NOTE";
                    radioButtonBad.Text = "Note is Fake";
                    radioButtonGood.Text = "Note is NOT Fake";
                }
            }
            else
            {
                //labelSuspectAndFake.Hide();
                panelSuspectAndFake.Hide();
            }

            if (Mpa.MetaExceptionId != 55
                & Mpa.MetaExceptionId != 225
                 & Mpa.MetaExceptionId != 226
                 & WActionId != "08"
                )

            {
                labelSuspectAndFake.Text = "OTHER EXCEPTION";

                // if ActionType != "00" and Authorised 
                // if ActionType != "00" and NOT Authorised 
                // if ActionType = "00" and Matched = false 

                RRDMActions_GL Ag = new RRDMActions_GL();

                string Message = "";

                Message = "This is other type of exception listed here for information!" + Environment.NewLine;

                if (Mpa.ActionType != "00" & Mpa.Authoriser != "")
                {
                    Ag.ReadActionByActionId(WOperator, Mpa.ActionType,1);
                    Message = Message + "Action Taken and Authorised " + Environment.NewLine;
                    Message = Message + "The Action is :  " + Environment.NewLine;
                    Message = Message + Ag.ActionNm + Environment.NewLine;
                }
                if (Mpa.ActionType != "00" & Mpa.Authoriser == "")
                {
                    Ag.ReadActionByActionId(WOperator, Mpa.ActionType,1);
                    Message = Message + "Action Taken and NOT Authorised " + Environment.NewLine;
                    Message = Message + "The Action is :  " + Environment.NewLine;
                    Message = Message + Ag.ActionNm + Environment.NewLine;
                }
                if (Mpa.ActionType == "00")
                {
                    Message = Message + "This is an unmatched record " + Environment.NewLine;
                    Message = Message + "No Action has been taken!" + Environment.NewLine;
                }
                MessageBox.Show(Message);
            }
        }

        // Show Grid 
        public void ShowGrid()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  Done
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 80; // Terminal
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Terminal Type, ATM, POS etc 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 120; // Descr
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // Err
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 40; // Mask
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; // Account
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].Visible = false;

            dataGridView1.Columns[9].Width = 50; // Ccy
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 80; // Amount
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[11].Width = 140; // Date
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 70; // ActionType
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


        }

        // Journal 
        private void button3_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            string SingleChoice;

            string SelectionCriteria = " WHERE UniqueRecordId =" + Er.UniqueRecordId;

            // Show Lines of journal 

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

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
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // Show Error 
        private void button5_Click(object sender, EventArgs e)
        {
            string SelectionCriteria = " Where UniqueRecordId =" + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            bool Deposits = true;
            string SearchFilter = "ErrNo = " + Mpa.MetaExceptionNo;

            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Deposits, SearchFilter);
            NForm24.ShowDialog();
        }

        // Update coming from Suspect or Fake
        private void buttonUpdateNote_Click(object sender, EventArgs e)
        {
            if (radioButtonBad.Checked == false
                & radioButtonGood.Checked == false)
            {
                MessageBox.Show("Please Make Selection");
                return;
            }

            Er.ReadErrorsTableSpecific(WErrNo);

            if (Er.ErrId == 225)
            {
                // Suspect was expected to be good
                // Customer already credited
                if (radioButtonBad.Checked == true)
                {
                    // Debit customer with this Note
                    // Update metaexception with customer data
                    // To be debited after authorisation 

                    // Update Error Table
                    // 
                    Er.OpenErr = true;
                    Er.UnderAction = true; // if this then create txns

                    Er.ManualAct = false;

                    Er.DisputeAct = false;

                    Er.ActionDtTm = DateTime.Now;
                    Er.ActionSesNo = WSesNo;
                    Er.ActionRMCycle = WReconcCycleNo;

                    Er.UserComment = textBoxUserComment.Text;

                    Er.DrCust = true;
                    Er.CrCust = false;
                    Er.CustAccNo = Mpa.AccNumber;

                    Er.DrAtmCash = false;
                    Er.CrAtmCash = true;
                    Er.AccountNo1 = BranchExcess;

                    Er.DrAtmSusp = false;
                    Er.CrAtmSusp = false;
                    Er.AccountNo2 = "";

                    MessageBox.Show("Customer will be debited");

                    Er.UpdateErrorsTableSpecific(WErrNo); // Update error   

                }
                if (radioButtonGood.Checked == true)
                {
                    // This what is expected 
                    // Update metaexception with No action
                    Er.OpenErr = true;
                    Er.UnderAction = false;

                    Er.ManualAct = true;

                    Er.DisputeAct = false;

                    Er.ActionDtTm = DateTime.Now;
                    Er.ActionSesNo = WSesNo;
                    Er.ActionRMCycle = WReconcCycleNo;

                    Er.UserComment = textBoxUserComment.Text;

                    Er.DrCust = false;
                    Er.CrCust = false;
                    Er.CustAccNo = "";

                    Er.DrAtmCash = false;
                    Er.CrAtmCash = false;
                    Er.AccountNo1 = "";

                    Er.DrAtmSusp = false;
                    Er.CrAtmSusp = false;
                    Er.AccountNo2 = "";

                    MessageBox.Show("No effect on customer");

                    Er.UpdateErrorsTableSpecific(WErrNo); // Update error 
                }
            }

            if (Er.ErrId == 226) // Fake
            {
                // Fake was expected to be bad
                // Customer already not credited
                if (radioButtonBad.Checked == true)
                {
                    // Debit customer with this Note
                    // Update metaexception with customer data
                    // To be debited after authorisation 

                    // Update Error 
                    // 
                    Er.OpenErr = true;
                    Er.UnderAction = true;

                    Er.ManualAct = true;

                    Er.DisputeAct = false;

                    Er.ActionDtTm = DateTime.Now;
                    Er.ActionSesNo = WSesNo;
                    Er.ActionRMCycle = WReconcCycleNo;

                    Er.UserComment = textBoxUserComment.Text;

                    Er.DrCust = false;
                    Er.CrCust = false;
                    Er.CustAccNo = "";

                    Er.DrAtmCash = false;
                    Er.CrAtmCash = false;
                    Er.AccountNo1 = "";

                    Er.DrAtmSusp = false;
                    Er.CrAtmSusp = false;
                    Er.AccountNo2 = "";

                    MessageBox.Show("No effect on customer");

                    Er.UpdateErrorsTableSpecific(WErrNo); // Update error    

                }
                if (radioButtonGood.Checked == true)
                {
                    // This was not expected 
                    // take action and credit the customer
                    Er.OpenErr = true;
                    Er.UnderAction = true;

                    Er.ManualAct = false;

                    Er.DisputeAct = false;

                    Er.ActionDtTm = DateTime.Now;
                    Er.ActionSesNo = WSesNo;
                    Er.ActionRMCycle = WReconcCycleNo;

                    Er.UserComment = textBoxUserComment.Text;

                    Er.DrCust = false;
                    Er.CrCust = true;
                    Er.CustAccNo = Mpa.AccNumber;

                    Er.DrAtmCash = true;
                    Er.CrAtmCash = false;
                    Er.AccountNo1 = BranchExcess;

                    Er.DrAtmSusp = false;
                    Er.CrAtmSusp = false;
                    Er.AccountNo2 = "";

                    MessageBox.Show("Customer will be credited");

                    Er.UpdateErrorsTableSpecific(WErrNo); // Update error     

                }
            }


            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = 1;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            SetScreen();

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        // Selected bad 
        private void radioButtonBad_CheckedChanged(object sender, EventArgs e)
        {
            if (Er.ErrId == 225)
            {
                // Suspect was expected to be good
                // Customer already credited
                // Now we find that it is no good

                textBoxUserComment.Text = "Note is not acceptable." + Environment.NewLine
                                          + "Customer will be debited!." + Environment.NewLine
                                        ;

                textBoxUserComment.ForeColor = Color.Red;

            }
            if (Er.ErrId == 226)
            {
                // FAKE was expected to be Bad
                // 

                textBoxUserComment.Text = "Fake was checked." + Environment.NewLine
                                          + "Customer will be informed!.";

                textBoxUserComment.ForeColor = Color.Black;
            }
        }
        // Selected Good
        private void radioButtonGood_CheckedChanged(object sender, EventArgs e)
        {
            if (Er.ErrId == 225)
            {

                // This what is expected 
                // Update metaexception with No action
                textBoxUserComment.Text = "Note is acceptable." + Environment.NewLine
                                          + "Customer will NOT be debited!." + Environment.NewLine
                                        ;
                textBoxUserComment.ForeColor = Color.Black;
            }
            if (Er.ErrId == 226)
            {
                // FAKE is Good 
                // Customer will be credited

                textBoxUserComment.Text = "Note is not Fake it is a good Note." + Environment.NewLine
                                          + "Customer will be credited!." + Environment.NewLine
                                        ;

                textBoxUserComment.ForeColor = Color.Red;
            }
        }
        //
        // Apply action on Presenter 
        //

        Form14b NForm14b;

        string UniqueRecordIdOrigin; 

        private void buttonApplyAction_Click(object sender, EventArgs e)
        {

            if (radioSystemSuggest.Checked == false
                & radioButtonMoveToDispute.Checked == false
                & radioButtonNoAction.Checked == false
                )
            {
                MessageBox.Show(" IF YOU WANT TO ACT PRESS A SELECTION BUTTON");
                return;
            }
            else
            {
                if (comboBoxReasonOfAction.Text == "Select Reason")
                {
                    MessageBox.Show("Please Select Reason Of Action");
                    return;
                }

                if (radioSystemSuggest.Checked == true)
                {
                    if (Mpa.TXNDEST == "1")
                    {
                        // Flexcube
                        WActionId = "95"; // 
                    }
                    else
                    {
                        // Other than Flexcube
                        // Goes to settlement 
                        WActionId = "96"; // 
                    }    
                }
                if (radioButtonMoveToDispute.Checked == true) // for credit the customer
                {

                    WActionId = "05";
                }
                if (radioButtonNoAction.Checked == true) // 
                {
                    // 04_Force Matching(Settle it with No Action)
                    WActionId = "04";
                }
            }

            if (WActionId == "95" || WActionId == "96")
            {
                UniqueRecordIdOrigin = "Master_Pool";

                NForm14b = new Form14b(WSignedId, WOperator,
                                     UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                           WActionId, comboBoxReasonOfAction.Text,Mpa.TransAmount, "Replenishment", WSesNo);

                NForm14b.FormClosed += NForm14b_FormClosed;
                NForm14b.ShowDialog();

                int WRowIndex = dataGridView1.SelectedRows[0].Index;
                //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            }

            // dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;


            // Dispute 
            if (WActionId == "05")
            {
                //MessageBox.Show("The funds will be moved to Intermediate account" + Environment.NewLine
                //                 + "Also an internal dispute connected to the maker name will be oppened" + Environment.NewLine
                //                 );

                string SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    MessageBox.Show(" Dispute already open for this Error");
                    return;
                }
                Form5 NForm5;
                int From = 7; // From pre - dispute investigation 
                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, WUniqueRecordId, Mpa.TransAmount, 0, "From Reconciliation", From, "ATM");
                NForm5.FormClosed += NForm5_FormClosed;
                NForm5.ShowDialog();

                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.MatchedType = "Move To Dispute";

                Mpa.ActionType = WActionId;

               // Mpa.Comments = Mpa.Comments + "," + comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                int WRowIndex = dataGridView1.SelectedRows[0].Index;
                //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                // dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }

            // Force matched 
            if (WActionId == "04")
            {
                string SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.MatchedType = "Settle with no action";

                Mpa.ActionType = "04";

               // Mpa.Comments = Mpa.Comments +","+ comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

                // Update RM Cycle
                // For having these for Form271a
                //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRMCategoryId, WRMCycleNo);

                //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;

                //Rcs.UpdateReconcCategorySessionWithAuthorClosing(WRMCategoryId, WRMCycleNo);

                MessageBox.Show("Record with UniqueRecordId : " + WUniqueRecordId.ToString() + Environment.NewLine
                    + " Has been settled by force");

                int WRowIndex = dataGridView1.SelectedRows[0].Index;
                // int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                //  dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }
            
        }

        private void NForm14b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ComingFromMeta = true;
            // UPDATE TRANSACTION 
            
            if (NForm14b.Confirmed == true)
            {
                string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.MetaExceptionNo = 0;
                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.ActionType = WActionId; // 

                //Mpa.Comments = Mpa.Comments + " ," + comboBoxReasonOfAction.Text;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);
                
            }
            else
            {
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(UniqueRecordIdOrigin, Mpa.UniqueRecordId,
                                                                                                WActionId);
            }

        }



        //// Form5 Close
        //void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        //{

        //    Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
        //    if (Dt.RecordFound == true)
        //    {
        //        DisputeOpened = true;
        //        labelDisputeId.Show();
        //        textBoxDisputeId.Show();

        //        textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
        //    }
        //    else
        //    {

        //        DisputeOpened = false;

        //    }
        //}

        // Return From Dispute
        bool DisputeOpened;
        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                labelDisputeId.Show();

                labelDisputeId.Text = "Dispute Id :.." + Dt.DisputeNumber.ToString();
            }
            else
            {
                labelDisputeId.Hide();
                DisputeOpened = false;

                MessageBox.Show("Dispute was not opened! ");
                return;

            }
        }
        // UNDO presenter
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            UndoAction();
        }
        // Undo Note
        private void buttonUndoForNote_Click(object sender, EventArgs e)
        {
            UndoAction();
        }
        private void UndoAction()
        {
            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            if (Mpa.Authoriser != "")
            {
                MessageBox.Show("This Record is authorised" + Environment.NewLine
                           + "Means that action already taken and finalised" + Environment.NewLine
                           + "You can not apply Undo."
                       );
                return;
            }

            WActionId = Mpa.ActionType;

            if (Mpa.ActionType == "05") // This for Dispute 
            {
                RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                //comboBoxForceMatching.Text = "Select Reason";
                Dt.ReadDisputeTranByUniqueRecordId(WUniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    //Di.ReadDispute(Dt.DisputeNumber);

                    //Delete dispute with its Txns 

                    Di.DeleteDisputeRecord(Dt.DisputeNumber);

                    labelDisputeId.Hide();

                }
            }

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            // ReadActionsOccurancesByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId);
            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, WActionId);
            if (Aoc.RecordFound == true)
            {
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID("Master_Pool", WUniqueRecordId, WActionId);
            }

            Mpa.MetaExceptionNo = 0;
            Mpa.ActionByUser = false;
            Mpa.UserId = WSignedId;
            Mpa.ActionType = "00";

            Mpa.MatchedType = "";

            Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 2);

            // BY FORCE 

            if (Mpa.ActionType == "04") // This is the Force Matching case 
            {
                comboBoxReasonOfAction.Text = "Select Reason";
            }

            // Dispute 


            SetScreen();

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));



            //// THIS IS THE OLD CODE 
            //if (Er.TotalSelected > 0)
            //{
            //    // There are errors for this WUniqueRecordId

            //    Er.DeleteErrorRecordByErrNo(Mpa.MetaExceptionId);

            //    if (Er.TotalSelected == 1)
            //    {
            //        Mpa.MetaExceptionNo = 0;
            //        Mpa.ActionByUser = false;
            //        Mpa.UserId = WSignedId;
            //        Mpa.ActionType = "00";

            //        Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);
            //    }
            //    else
            //    {
            //        // They were more than one Exceptions
            //        Er.ReadErrorsTableSpecificByUniqueRecordId(WUniqueRecordId);
            //        Mpa.MetaExceptionNo = Er.ErrNo;

            //        Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, WUniqueRecordId, 1);
            //    }

            //}


        }
        // ATM GL Accounts
        private void GetGLAtmAccounts()
        {

            BranchExcess = "";
            BranchIntermediary = "";
            BranchSettlement = ""; 

            //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

            //Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

            //if (Acc.RecordFound == true)
            //{
            //    ATMSuspence = Acc.AccNo;
            //}
            //else
            //{
            //    MessageBox.Show("ATM Suspense Account Not Found");
            //}
            RRDMUsersRecords Ua = new RRDMUsersRecords();
            Ua.ReadUsersRecord(WSignedId);
            //Ua.Branch = "015";
            Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(WOperator, "40", Ua.Branch); // 40 for Branch Excess

            if (Acc.RecordFound == true)
            {
                BranchExcess = Acc.AccNo;
            }
            else
            {
                MessageBox.Show("Branch Excess Account Not Found");
            }

            Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(WOperator, "52", Ua.Branch); // 52 for Branch Excess

            if (Acc.RecordFound == true)
            {
                BranchIntermediary = Acc.AccNo;
            }
            else
            {
                MessageBox.Show("Branch Intermediary Account Not Found");
            }
            //
            Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(WOperator, "53", Ua.Branch); // 52 for Branch Excess

            if (Acc.RecordFound == true)
            {
                BranchSettlement = Acc.AccNo;
            }
            else
            {
                MessageBox.Show("Branch Settlement Account Not Found");
            }

            

        }
        // Source Records
        private void linkLabelSourceRecords_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
            Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

            //switch (WOperator)
            //{
            //    case "CRBAGRAA":
            //        {
            //            // DEMO MODE

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "ETHNCY2N":
            //        {

            //            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

            //            NForm78d_AllFiles.ShowDialog();

            //            break;
            //        }
            //    case "BCAIEGCX":
            //        {
                        NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, 1);

                        NForm78d_AllFiles_BDC_3.ShowDialog();
            //            break;
            //        }
            //}

        }
        // Path
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("No Testing data for this Demo");
        }

        // Show Note 3
        private void ShowNote_3()
        {
            // NOTES START  
            string Order = "Descending";
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
        // Notes 3
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            string WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            SetScreen();
        }
        // Show Deposits Txns 
        private void buttonShowDepositTxns_Click(object sender, EventArgs e)
        {
            Form38_CDM NForm38_CDM;

            NForm38_CDM = new Form38_CDM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm38_CDM.Show();
        }
        //// If no Action is true 
        //private void radioButtonNoAction_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButtonNoAction.Checked == true)
        //    {
        //        labelForceMatching.Show();
        //        comboBoxReasonOfAction.Show();
        //    }
        //    else
        //    {
        //        labelForceMatching.Hide();
        //        comboBoxReasonOfAction.Hide();
        //    }
        //}
        // GL TXNS 

        private void buttonGLTxns_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo + " AND OriginWorkFlow ='Replenishment'";
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

        private void radioButtonNoAction_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}


using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class UCForm271c : UserControl
    {

        public decimal WGlOpenBalance;
        public decimal WRRDMJournalBal;
        public decimal WBalanceWithActions;

        decimal WOpenAndMatched;

        decimal textBoxGrandDIFF_Value; 

        //Form75 NForm75;
        Form14a NForm14;

        string WCcy;

        //Form79 NForm79;
        Form5 NForm5;

        int WRow;

        string errfilter;

        //int LastTrace;
        bool HostRecordForThisError;

        int WErrNo;
        int WTraceNo;
        int WTranNo;
        int WErrId;

        int WProcess;

        bool DisputeOpened;

        bool ViewWorkFlow;
       
    //    string WCurrNm;
  
        decimal AmountForSuspense;


        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCategATMsAtRMCycles RAtms = new RRDMReconcCategATMsAtRMCycles(); 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMHostTransClass Ht = new RRDMHostTransClass();

        //RRDMUsersAccessRights Us = new RRDMUsersAccessRights(); 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
        //   string WUserOperator; 
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int WSeqNo;
        string WMainCateg;
        //int WRowIndex;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WRecCategory;
        int WRunningJobNo;

        public void UCForm271cPar(string InSignedId, int InSignRecordNo, string InOperator, string InRecCategory, int InRunningJobNo)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WRecCategory = InRecCategory;

            WRunningJobNo = InRunningJobNo;
            InitializeComponent();

            Us.ReadUsersRecord(WSignedId);
            string WBankId = Us.BankId;
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WBankId);
            WCcy = Ba.BasicCurName;

            // FIND STATUS
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            WMainCateg = WRecCategory.Substring(0, 4);

            if (WRecCategory == "EWB311")
            {
                // LEAVE THIS HERE 
                // DO NOT MOVE IT FROM HERE

                // Change labels 


                // Hide First Set of balances
                label6.Hide();
                panel2.Hide();

                label30.Text = "Open+Matched";
                label26.Text = "GL Balance Adj";

                label24.Text = "Open+Matched";
                label23.Text = "GL Balanc Today";
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRecCategory, WRunningJobNo); 
                //Rcs.ReadMatchingCategoriesSessionsByRunningJobNo(WOperator, WCategMatchingSession);

                WProcess = 4; // Include corrected errors 
                Rcs.ReadAllErrorsFromCategSessionGL
                    (WRecCategory, WRunningJobNo, Rcs.MatchedTransAmt ,(Rcs.GlTodaysBalance + Rcs.MatchedTransAmt + Rcs.SettledUnMatchedAmtDefault ) , WProcess);

                // Journal is the GL

                labelCurr.Text = WCcy;

                textBox8.Text = Rcs.GlYesterdaysBalance.ToString("#,##0.00");
                WOpenAndMatched = Rcs.GlYesterdaysBalance + Rcs.MatchedTransAmt+ Rcs.SettledUnMatchedAmtDefault;
                textBox7.Text = WOpenAndMatched.ToString("#,##0.00");

                textBox6.Text = Rcs.GlTodaysBalance.ToString("#,##0.00");
                //textBox6.Text = Rs.GlTodaysBalance.ToString("#,##0.00");
                textBoxGrandDIFF_Value = (WOpenAndMatched - Rcs.GlTodaysBalance); 
                textBoxGrandDIFF.Text = textBoxGrandDIFF_Value.ToString("#,##0.00");         
                
            }

            SetScreen(); 
        }

        // SHOW SCREEN 

        public void SetScreen()
        {

            //// READ FOR CATEGORY 
            // THIS SHOULD BE LEFT HERE FOR "EWB102"
            // Do Not MOVE it 
          
            if (WMainCateg == "RECA")
            {
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRecCategory, WRunningJobNo);
                RAtms.ReadReconcCategATMsAtRMCyclesCategoriesATMsRMCycleForTotals(WOperator, WRecCategory, WRunningJobNo);
                // INCLUDE IN BALANCES ANY CORRECTED ERRORS
                WProcess = 4; // Include corrected errors 

                Rcs.ReadAllErrorsFromCategSessionForAllAtmsWithErrors
                          (WRecCategory, WRunningJobNo, (RAtms.AllAtmsOpeningBalance 
                          + RAtms.AllAtmsMatchedAmtAtMatching + RAtms.AllAtmsMatchedAmtAtDefault), WProcess);

                // REQUEST TO RECONCILE FIRST SET OF BALANCES
                // GETthe Forced Matched

                string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                           + WRecCategory + "' AND MatchingAtRMCycle =" + WRunningJobNo
                           //+ " AND TerminalId ='" + Er.AtmNo + "'"
                           + " AND IsMatchingDone = 1 ";

                Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);
                // Journal is the GL

                labelCurr.Text = WCcy;

                textBox8.Text = Rcs.GlYesterdaysBalance.ToString("#,##0.00");
                decimal WRRDMJournalBalCateg = RAtms.AllAtmsJournalAmt; 
                //decimal WRRDMJournalBalCateg = Rs.GlYesterdaysBalance + Rs.MatchedTransAmt + Rs.NotMatchedTransAmt;
                textBox7.Text = WRRDMJournalBalCateg.ToString("#,##0.00");
                decimal Temp1 = Rcs.BanksClosedBalAdjWithErrors
                             + Mpa.TotalForcedMatchedAmountAndInJournal + Mpa.TotalMoveToDisputeAmt;
                textBox6.Text = Temp1.ToString("#,##0.00");
                textBoxGrandDIFF.Text = (WRRDMJournalBalCateg - Temp1).ToString("#,##0.00");
                textBoxGrandDIFF_Value = (WRRDMJournalBalCateg - Temp1);
             

                //if ((WRRDMJournalBalCateg - (Rcs.BanksClosedBalAdjWithErrors + Mpa.TotalForcedMatchedAmount )) == 0)
                //textBox6.Text = Rcs.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                //textBox4.Text = (WRRDMJournalBalCateg - Rcs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");

                if ((WRRDMJournalBalCateg - Temp1) == 0)
                {
                    label32.Hide();
                    panel6.Hide(); 
                 
                    Er.ReadAllErrorsTableForCounters(WOperator, WRecCategory, "", 0, "");
                    if (textBoxGrandDIFF_Value == 0)
                    //if (Er.NumOfErrors == Er.ErrUnderAction + Er.ErrDisputeAction + Er.NumOfOpenErrorsLess100 + Er.NumOfOpenErrorsBetween200And300 )
                    {
                        UpdateReconcStatus(1); // Reconciled 
                    }
                    else
                    {
                        UpdateReconcStatus(2); // Not Reconciled 
                    }
                }
                else
                {
                    //label15.Text = "OUTSTANDING DIFF : " + (WRRDMJournalBalCateg 
                    //    - (Rcs.BanksClosedBalAdjWithErrors + Mpa.TotalForcedMatchedAmount)).ToString("#,##0.00");
                    label15.Text = "OUTSTANDING DIFF : " + (WRRDMJournalBalCateg
                       - Rcs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");
                    label32.Show();
                    panel6.Show();
                    UpdateReconcStatus(2);

                    textBox9.Text = RAtms.ReadReconcCategoriesATMsRMCycleToFindATMInDiff(WOperator, WRecCategory, WRunningJobNo, 3);
                   
                }
                
            }


         ///   WCurrNm = Rcs.GlCurrency;

            try
            {
                // Show errors table 
                if (WMainCateg == "RECA")
                    errfilter = "CategoryId='" + WRecCategory + "' AND CurDes ='" + WCcy
                    //+ "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5) AND RMCycle<=" + WRunningJobNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionRMCycle =" + WRunningJobNo + "))  ";
                + "'" + " AND (ErrType = 2 OR ErrType = 4 OR ErrType = 5) "
                + " AND RMCycle<=" + WRunningJobNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionRMCycle =" + WRunningJobNo + "))  ";

                if (WRecCategory == "EWB311") 
                    errfilter = "CategoryId='" + WRecCategory + "' AND CurDes ='" + WCcy
                  + "'" + " AND (ErrType = 2 OR ErrType = 5) AND RMCycle<=" + WRunningJobNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionRMCycle =" + WRunningJobNo + "))  ";

                Er.ReadErrorsAndFillTable(WOperator, WSignedId, errfilter);

                dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

                dataGridView1.Columns[0].Width = 60; // ExcNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 80; // AtmNo
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].Width = 120; //  Desc
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

                dataGridView1.Columns[3].Width = 80; // Card
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[4].Width = 50; // Ccy
                //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 60; // Amount
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[6].Width = 50; // NeedAction
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 50; // UnderAction
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //dataGridView1.Columns[7].Width = 50; //DisputeAct

                dataGridView1.Columns[8].Width = 40; // ManualAct
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[9].Width = 80; // 

                //dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);

                //ErrorsTable.Columns.Add("ExcNo", typeof(int));
                //ErrorsTable.Columns.Add("AtmNo", typeof(string));
                //ErrorsTable.Columns.Add("Desc", typeof(string));
                //ErrorsTable.Columns.Add("Card", typeof(string));
                //ErrorsTable.Columns.Add("Ccy", typeof(string));
                //ErrorsTable.Columns.Add("Amount", typeof(string));
                //ErrorsTable.Columns.Add("NeedAction", typeof(string));
                //ErrorsTable.Columns.Add("UnderAction", typeof(string));
                //ErrorsTable.Columns.Add("DisputeAct", typeof(string));
                ////ErrorsTable.Columns.Add("ManualAct", typeof(bool));
                //ErrorsTable.Columns.Add("DateTime", typeof(DateTime));
                //ErrorsTable.Columns.Add("TransDescr", typeof(string));
                //ErrorsTable.Columns.Add("UserComment", typeof(string));


            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
            // Dissable buttons for view only 
            if (ViewWorkFlow == true)
            {
                guidanceMsg = "View only!";
                ChangeBoardMessage(this, new EventArgs());
                buttonApplyAction.Enabled = false; // Apply
                //buttonUndoAction.Enabled = false; // Undo
                buttonCreateError.Enabled = false; // Create error 

                buttonApplyAction.Hide();
                //buttonUndoAction.Hide();
                buttonCreateError.Hide();
                pictureBox3.Hide();
            }
            else
            {
                guidanceMsg = "Select Exception No And Take action ";
                ChangeBoardMessage(this, new EventArgs());
            }

                       //WGlOpenBalance = Rs.GlYesterdaysBalance;
            //WBalanceWithActions = Rs.GlTodaysBalance;
            
        }
        // 
        // SHOW SELECTED ERROR 
        //
        // SHOW SELECTED ERROR 

        // ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            panel4.Show(); // SHOW PANEL WITH Its fields 

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            textBox28.Text = rowSelected.Cells[0].Value.ToString();
            WErrNo = (int)rowSelected.Cells[0].Value;

            // Find Errors details 
            Er.ReadErrorsTableSpecific(WErrNo);

            label6.Text = "ATM : " + Er.AtmNo + " BEFORE ACTION/S";  

            if (WRecCategory == Er.CategoryId)
            {
                textBox2.Hide();

                WTranNo = Er.UniqueRecordId;

                WTraceNo = Er.TraceNo;

                WErrId = Er.ErrId;

                if (WErrId > 100)
                {
                    HostRecordForThisError = true;

                    if (WErrId == 175) // Error for missing at Host
                    {
                        HostRecordForThisError = false;
                    }
                    // Read Host and find Full Card Number and Full Account Number

                    if (HostRecordForThisError == true)
                    {
                        Ht.ReadHostTransTraceNo(WOperator, WRecCategory, WTraceNo);

                        if (Ht.RecordFound == true)
                        {
                            Er.CardNo = Ht.CardNo;
                            Er.CustAccNo = Ht.AccNo;
                            Er.FullCard = true;

                            Er.UpdateErrorsTableSpecific(WErrNo);
                        }
                        else
                        {
                            Er.FullCard = false;
                        }
                    }
                }

                textBox29.Text = " OPEN ";
                textBox22.Text = Er.ErrDesc;
                textBox27.Text = Er.DateTime.ToString();
                textBox37.Text = Er.TraceNo.ToString();
                textBox21.Text = Er.CircularDesc;

                textBox23.Text = Er.ErrAmount.ToString("#,##0.00");
                textBox24.Text = Er.CurDes.ToString();

                // Read to find transactions
                if (Er.ErrId != 198) // 198 is the correction for suspense has nothing to do with transactions 
                {
                    // VALIDATION OF TRACES STATUS 
                    //MessageBox.Show("Check if needed");
                    //Tc.ReadInPoolTransSpecific(Er.UniqueRecordId);
                    //// Check to see if in Host for Errors reported in ATM 

                    //buttonShowTrans.Show();
                    textBox25.Text = Er.CardNo;
                    textBox26.Text = Er.CustAccNo;
                }

                else // Error = 198
                {
                    textBox37.Text = " N/A ";
                    //       textBox30.Text = " N/A ";
                    buttonShowTrans.Hide();
                    textBox25.Text = " N/A ";
                    textBox26.Text = " N/A ";
                }


                if (Er.DrCust == true)
                {
                    textBox38.Text = " DEBIT CUSTOMER ";
                }
                if (Er.CrCust == true)
                {
                    textBox38.Text = " CREDIT CUSTOMER ";
                }
                if (Er.DrAtmCash == true)
                {
                    textBox39.Text = " DEBIT BANK ACCOUNT ";
                }
                if (Er.CrAtmCash == true)
                {
                    textBox39.Text = " CREDIT BANK ACCOUNT ";
                }

                if (Er.DrCust == false & Er.CrCust == false)
                {
                    if (Er.DrAtmCash == true)
                    {
                        textBox38.Text = " DEBIT BANK ACCOUNT ";
                    }
                    if (Er.CrAtmCash == true)
                    {
                        textBox38.Text = " CREDIT BANK ACCOUNT ";
                    }

                    if (Er.DrAtmSusp == true)
                    {
                        textBox39.Text = " DEBIT BANK SUSPENSE";
                    }
                    if (Er.CrAtmSusp == true)
                    {
                        textBox39.Text = " CREDIT BANK SUSPENSE";
                    }

                }

                if (Er.TraceNo > 0)
                {
                    string SelectionCriteria = "Where UniqueRecordId = " + Er.UniqueRecordId; 
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);
                    
                    //Tc.ReadInPoolAtmTrace(WCategory, Er.TraceNo);

                    Gp.ReadParametersSpecificId(WOperator, "705", Mpa.TargetSystem.ToString(), "", "");
                    textBox1.Text = Gp.OccuranceNm;
                }

                if (Er.TraceNo == 0) // ERROR CREATED FOR SUSPENSE 
                {
                    textBox1.Text = "Suspense";
                }

                textBox32.Text = Er.UserComment;

                if (Er.UnderAction == true || Er.DisputeAct == true || Er.ManualAct)
                {
                    if (Er.DisputeAct)
                    {
                        radioButtonMoveToDispute.Checked = true;
                    }
                    if (Er.ManualAct)
                    {
                        radioButtonPostpone.Checked = true;
                    }
                    if (Er.ManualAct != true & Er.DisputeAct != true)
                    {
                        radioSystemSuggest.Checked = true;
                    }

                    // Action taken 
                    label8.Text = "Action Taken"; // DONE
                    pictureBox3.Hide(); // Take Action
                    buttonApplyAction.Hide();
                    //buttonUndoAction.Show();
                    guidanceMsg = "Action taken. Undo if you have changed your mind.";
                    ChangeBoardMessage(this, new EventArgs());
                   
                }
                else
                {
                    // Initialise Radio Buttons 
                    //
                    radioSystemSuggest.Checked = false;
                    radioButtonForceClose.Checked = false;
                    radioButtonPostpone.Checked = false;
                    radioButtonMoveToDispute.Checked = false;
                    // ACtion not taken yet
                    label8.Text = "Action Not Taken"; 
                    pictureBox3.Show(); // Take Action
                    buttonApplyAction.Show();
                    //buttonUndoAction.Hide();
                    guidanceMsg = "Study error and decide action.";
                    ChangeBoardMessage(this, new EventArgs());
                    
                }

                // Show Dispute 

                Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    labelDisputeId.Show();
                    textBoxDisputeId.Show();
                    //buttonMoveToDispute.Hide();
                    textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
                }
                else
                {
                    labelDisputeId.Hide();
                    textBoxDisputeId.Hide();
                    //buttonMoveToDispute.Show();
                }

                //   if (Pa.TraceNo > Na.MaxTraceTarget) textBoxMsgBoard.Text = "DECIDE ACTION NOTING CORRESPONDING HOST TRANS not AVAIL";
            }

            // SHOW BALANCES 

            if (WMainCateg == "RECA")
            {
                RAtms.ReadReconcCategoriesATMsRMCycleSpecificAtmforReconcCatTOTALS(WOperator, WRecCategory, WRunningJobNo, Er.AtmNo);

                WProcess = 4; // Include corrected errors 

                //ReadAllErrorsTableFromCategSessionForATM(string InCategoryId, int InActionRMCycle, string InAtmNo,  decimal InBanksClosedBal, int InFunction)
                RAtms.ReadAllErrorsTableFromCategSessionForATMAddErrors
                    (WRecCategory, WRunningJobNo, Er.AtmNo, (RAtms.TotOpeningBalance + RAtms.TotMatchedAmtAtMatching + RAtms.TotMatchedAmtAtDefault), WProcess);

                // GETthe Forced Matched
             
                string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                           + WRecCategory + "' AND MatchingAtRMCycle =" + WRunningJobNo 
                           + " AND TerminalId ='" + Er.AtmNo +"'"
                           + " AND IsMatchingDone = 1 ";

                Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

                labelCurr.Text = RAtms.Currency;

                textBox3.Text = RAtms.TotOpeningBalance.ToString("#,##0.00");
                WRRDMJournalBal = RAtms.TotOpeningBalance + RAtms.TotJournalAmt;
                textBox5.Text = WRRDMJournalBal.ToString("#,##0.00");
                textBox33.Text = (RAtms.TotOpeningBalance + RAtms.TotMatchedAmtAtMatching + RAtms.TotMatchedAmtAtDefault) .ToString("#,##0.00");
          
                textBox34.Text = (WRRDMJournalBal - (RAtms.TotOpeningBalance + RAtms.TotMatchedAmtAtMatching + RAtms.TotMatchedAmtAtDefault)).ToString("#,##0.00");

                // Working fields

                //WCurrNm = RAtms.Currency;
                WGlOpenBalance = RAtms.TotOpeningBalance;
                //WBalanceWithActions = RAtms.BanksClosedBalAdjWithErrors + Mpa.TotalForcedMatchedAmount;
                WBalanceWithActions = RAtms.BanksClosedBalAdjWithErrors 
                    + Mpa.TotalForcedMatchedAmountAndInJournal
                    + Mpa.TotalMoveToDisputeAmt; 

                textBox19.Text = WGlOpenBalance.ToString("#,##0.00");
                textBox18.Text = WRRDMJournalBal.ToString("#,##0.00");
                textBox36.Text = WBalanceWithActions.ToString("#,##0.00");
                textBoxDiffForAtm.Text = (WRRDMJournalBal - WBalanceWithActions).ToString("#,##0.00");


                //panel3.Hide(); // HIDE REFRESHED BALANCES 
                //label22.Hide();

                //panel4.Hide(); // HIDE ERROR DETAILS 

            }
            else
            {
                // Category EWB311

                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WRecCategory, WRunningJobNo);

                // Hide First Set of balances
                label6.Hide();
                panel2.Hide();
                labelCurr.Text = WCcy;

                WProcess = 4; // Include corrected errors 
                Rcs.ReadAllErrorsFromCategSessionGL
                    (WRecCategory, WRunningJobNo, (Rcs.MatchedTransAmt + Rcs.SettledUnMatchedAmtDefault), Rcs.GlTodaysBalance, WProcess);

                // Journal is the GL
                textBox19.Text = Rcs.GlYesterdaysBalance.ToString("#,##0.00");
                WOpenAndMatched = Rcs.GlYesterdaysBalance + Rcs.MatchedTransAdjWithErrors;
                textBox18.Text = WOpenAndMatched.ToString("#,##0.00");

                textBox36.Text = Rcs.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                //textBox6.Text = Rs.GlTodaysBalance.ToString("#,##0.00");
                textBoxDiffForAtm.Text = (WOpenAndMatched - Rcs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");


                // Working fields

                //WCurrNm = WCcy;
                WGlOpenBalance = Rcs.GlYesterdaysBalance;
                WBalanceWithActions = Rcs.GlTodaysBalance;


                panel3.Show(); // HIDE REFRESHED BALANCES 
                label22.Show();

                panel4.Show(); // HIDE ERROR DETAILS 


                if ((WOpenAndMatched - Rcs.BanksClosedBalAdjWithErrors) == 0)
                {
                    label32.Hide();
                    panel6.Hide();
                    Er.ReadAllErrorsTableForCounters(WOperator, WRecCategory, "", 0, "");

                    if (textBoxGrandDIFF_Value == 0)
                        //if (Er.NumOfErrors == Er.ErrUnderAction)
                    {
                        UpdateReconcStatus(1); // Reconciled 
                    }
                    else
                    {
                        UpdateReconcStatus(2); // NOT Reconciled 
                    }

                }
                else
                {
                    label15.Text = "OUTSTANDING DIFF : " + (WOpenAndMatched - Rcs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");
                    label32.Show();
                    panel6.Show();
                    UpdateReconcStatus(2);
                }
            }

        }

        // ACTION CHOSEN AND APPLY 
        // REFRESH BALANCES change error status 
        //

        private void buttonApplyAction_Click(object sender, EventArgs e)
        {
            if (radioSystemSuggest.Checked == false & radioButtonMoveToDispute.Checked == false & radioButtonPostpone.Checked == false & radioButtonForceClose.Checked == false)
            {
                MessageBox.Show(" IF YOU WANT TO ACT PRESS A DECISION BUTTON");
                return;
            }

            if (radioButtonMoveToDispute.Checked)
            {
                if (textBox32.Text == "")
                {
                    MessageBox.Show("Please enter comment why you are moving error to dispute ");
                    return;
                }
            }

            if (Er.UnderAction == true)
            {
                MessageBox.Show(" Action Was already taken. UNDO first and Then Take a new action ");
                return;
            }

            if (radioSystemSuggest.Checked)
            {
                if (Er.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO first and Then Take a new action ");
                    return;
                }

                WRow = dataGridView1.SelectedRows[0].Index;

                
                // SHOW REFRESHED BALANCES 
                panel3.Show();
                label22.Show();

                // Update Error Table
                // 
                Er.OpenErr = true;
                Er.UnderAction = true;
                if (radioButtonMoveToDispute.Checked == true)
                {
                    Er.DisputeAct = true;
                }
                Er.ManualAct = false;
                if (String.IsNullOrEmpty(textBox32.Text))
                {
                    Er.UserComment = "";
                }
                else Er.UserComment = textBox32.Text;

                Er.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen(); 

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            }
// Move to Dispute 
            if (radioButtonMoveToDispute.Checked)
            {
                WRow = dataGridView1.SelectedRows[0].Index;

                Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
                if (Dt.RecordFound == true)
                {
                    MessageBox.Show(" Dispute already open for this Error");
                    return;
                }

                int From = 7; // From pre - dispute investigation 
                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Er.CardNo, Er.UniqueRecordId, Er.ErrAmount, 0, textBox32.Text, From, "ATM");
                NForm5.FormClosed += NForm5_FormClosed;
                NForm5.ShowDialog();

                if (DisputeOpened == false)
                {
                    MessageBox.Show("Dispute was not opened! ");
                    return;
                }

                // Update Error Table
                // 
                Er.OpenErr = true;
                Er.UnderAction = false;
                Er.DisputeAct = true;
                Er.ManualAct = false;
                if (String.IsNullOrEmpty(textBox32.Text))
                {
                    Er.UserComment = "";
                }
                else Er.UserComment = textBox32.Text;

                Er.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
               
            }     


            if (radioButtonPostpone.Checked)
            {

                if (Er.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO if you want a new action ");
                    return;
                }
                // Update Error Table
                // FOR THIS MANUAL ACTION
                Er.OpenErr = true;
                Er.UnderAction = true;
                Er.DisputeAct = false;
                Er.ManualAct = true;

                Er.UpdateErrorsTableSpecific(WErrNo);

                guidanceMsg = " YOU have decided that for this error a manual action will be taken ";
                ChangeBoardMessage(this, new EventArgs());

                // Show errors table 
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            }

            if (radioButtonForceClose.Checked)
            {
                MessageBox.Show("Function not develop yet.");

                if (Er.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO if you want a new action");
                    return;
                }

                return;

            }

        }

        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {

            Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                DisputeOpened = true;
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                //buttonMoveToDispute.Hide();
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {

                DisputeOpened = false;
                //labelDisputeId.Hide();
                //textBoxDisputeId.Hide();
                //buttonMoveToDispute.Show();
            }
        }


        // UNDO ACTION 

        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox28.Text))
            {
                MessageBox.Show("Choose Error No to Undo ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }     

            WErrNo = int.Parse(textBox28.Text);

            WRow = dataGridView1.SelectedRows[0].Index;

            if (Er.UnderAction == true & Er.ManualAct == false)
            {

                // INITIALISE USER COMMENT 

                textBox32.Text = "";

                // Update Error Table
                // 
                Er.OpenErr = true;
                Er.UnderAction = false;
                Er.DisputeAct = false;
                Er.ManualAct = false;
                Er.UserComment = "";
                Er.FullCard = false;
                Er.UpdateErrorsTableSpecific(WErrNo);

                // Return Value back to possitive 
                //   ErrAmount = -ErrAmount; 

                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

                //if (Er.ErrAmount < 0)
                //{
                //    Er.ErrAmount = -Er.ErrAmount; // Make it possitive 
                //}

                // Show Panel 3 if hidden.

                if (textBox19.Text == "" & textBox18.Text == "" & textBox36.Text == "")
                {
                    // Do nothing 
                }
                else // Show balances 
                {
                    panel3.Show();
                    label22.Show();
                }

                guidanceMsg = " The ACTION that was taken is now UNDO. MAKE YOUR NEXT CHOICE";
                ChangeBoardMessage(this, new EventArgs());

            }
            else // Manual Action
            {
                // Update Error Table
                // 
                Er.OpenErr = true;
                Er.UnderAction = false;
                Er.DisputeAct = false;
                Er.ManualAct = false;
                Er.UserComment = "";
                Er.UpdateErrorsTableSpecific(WErrNo); // UPDATE ERROR TABLE 

                // Show errors table 
                int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
                //
                radioSystemSuggest.Checked = false;
                radioButtonForceClose.Checked = false;
                radioButtonPostpone.Checked = false;
                radioButtonMoveToDispute.Checked = false;

                // Show Panel 3 if hidden.

                if (textBox19.Text == "" & textBox18.Text == "" & textBox36.Text == "")
                {
                    // Do nothing 
                }
                else // Show balances 
                {
                    panel3.Show();
                    label22.Show();
                }

                guidanceMsg = " The ACTION that was taken is now UNDO. MAKE YOUR NEXT CHOICE";
                ChangeBoardMessage(this, new EventArgs());

            }
            Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                Di.ReadDispute(Dt.DisputeNumber);
                if (Di.DispType == 5)
                {
                    //Delete dispute 

                    Di.DeleteDisputeRecord(Dt.DisputeNumber);

                    textBoxDisputeId.Hide();
                    labelDisputeId.Hide();
                }
            }

        }

        // Go To SHOW transactions 
        // 
        private void buttonShowTrans_Click(object sender, EventArgs e)
        {
            Form80b NForm80b;

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WRecCategory, WRunningJobNo,"",Er.UniqueRecordId,4,"View","", 0);
            NForm80b.ShowDialog();
        }

        // GO TO CREATE ERROR

        private void buttonCreateError_Click(object sender, EventArgs e)
        {
            // error to be created has no relation with any atm. it moves overall difference to suspense
            // If it is known to be related with any ATM or error move to disputes then write it in notes 

            AmountForSuspense = decimal.Parse(textBoxDiffForAtm.Text); 

            if (AmountForSuspense > 0)
            {
                int MaskRecordId = 0;
               
                NForm14 = new Form14a(WSignedId, WRecCategory, WRunningJobNo, MaskRecordId, Er.AtmNo, Er.SesNo, 
                                                                                         WOperator, AmountForSuspense, WCcy);
                NForm14.FormClosed += NForm14_FormClosed;
                NForm14.Show();
            }
            else
            {
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                if (Usi.ReconcDifferenceStatus > 1 )
                {
                    MessageBox.Show("NO AMOUNT FOR SUSPENSE FOR THIS ATM . Choose the ATM shown in ATTENTION Panel", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               
                return;
            }

           
        }

        // FORM14 HAS CREATED THE SUSPENSE ERROR 
        void NForm14_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();
        }

        void NForm79_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen(); 
            // SET ROW Selection POSITIONING 
            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

        }
        // Update Reconciliation Status 
        public void UpdateReconcStatus(int InReconStatus)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = InReconStatus;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }
// Print Actions 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            string SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRecCategory + "'"
                      + "  AND MatchingAtRMCycle =" + WRunningJobNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            string WSortCriteria = "Order By TerminalId, SeqNo ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);

            string P1 = "Transactions For Reconciliation :" + WRecCategory
                         + " AND Cycle : " + WRunningJobNo.ToString();

            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
              
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WRecCategory, WRunningJobNo, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }

            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
    }
}

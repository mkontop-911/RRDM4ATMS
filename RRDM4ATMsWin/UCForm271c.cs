using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class UCForm271c : UserControl
    {

        public decimal WGlOpenBalance;
        public decimal WRRDMJournalBal;
        public decimal WBalanceWithActions;

        //Form75 NForm75;
        Form14a NForm14;

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

        bool DisputeOpenned;

        bool ViewWorkFlow;
       
        string WCurrNm;
  
        decimal AmountForSuspense;


        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;


        RRDMReconcCategoriesMatchingSessions Rs = new RRDMReconcCategoriesMatchingSessions();

        RRDMReconcCategATMsRMCycles RAtms = new RRDMReconcCategATMsRMCycles(); 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMHostTransClass Ht = new RRDMHostTransClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
        //   string WUserOperator; 
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //int WSeqNo;

        //int WRowIndex;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategory;
        int WCategMatchingSession;

        public void UCForm271cPar(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InCategMathingSession)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategory = InCategory;

            WCategMatchingSession = InCategMathingSession;
            InitializeComponent();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }    

            if (WCategory == "EWB311")
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
                label23.Text = "GL Balanc Adj"; 

                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WCategMatchingSession);


                WProcess = 4; // Include corrected errors 
                Rs.ReadAllErrorsTableFromCategSessionGL
                    (WCategory, WCategMatchingSession, Rs.MatchedTransAmt ,Rs.GlTodaysBalance , WProcess);

                // Journal is the GL

                labelCurr.Text = Rs.GlCurrency;

                textBox8.Text = Rs.GlYesterdaysBalance.ToString("#,##0.00");
                decimal WOpenAndMatched = Rs.GlYesterdaysBalance + Rs.MatchedTransAdjWithErrors;
                textBox7.Text = WOpenAndMatched.ToString("#,##0.00");

                textBox6.Text = Rs.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                //textBox6.Text = Rs.GlTodaysBalance.ToString("#,##0.00");
                textBox4.Text = (WOpenAndMatched - Rs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");

                if ((WOpenAndMatched - Rs.BanksClosedBalAdjWithErrors) == 0)
                {
                    Er.ReadAllErrorsTableForCounters(WOperator, WCategory, "");

                    if (Er.NumOfErrors == Er.ErrUnderAction)
                    {
                        UpdateReconcStatus(1);
                    }
                    else
                    {
                        UpdateReconcStatus(2);
                    }


                }
                else UpdateReconcStatus(2);
            }

            //panel3.Hide(); // HIDE REFRESHED BALANCES 
            //label22.Hide();

            //panel4.Hide(); // HIDE ERROR DETAILS  

            SetScreen(); 
        }

        // SHOW SCREEN 

        public void SetScreen()
        {
           
            //// READ FOR CATEGORY 
            // THIS SHOULD BE LEFT HERE FOR "EWB102"
            // Do Not MOVE it 

            if (WCategory == "EWB102")
            {
                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WCategMatchingSession);

                // INCLUDE IN BALANCES ANY CORRECTED ERRORS

                WProcess = 4; // Include corrected errors 

                Rs.ReadAllErrorsTableFromCategSessionForAllAtmsWithErrors
                          (WCategory, WCategMatchingSession, (Rs.GlYesterdaysBalance + Rs.MatchedTransAmt), WProcess);

                // REQUEST TO RECONCILE FIRST SET OF BALANCES

                // Journal is the GL

                labelCurr.Text = Rs.GlCurrency;

                textBox8.Text = Rs.GlYesterdaysBalance.ToString("#,##0.00");
                decimal WRRDMJournalBalCateg = Rs.GlYesterdaysBalance + Rs.MatchedTransAmt + Rs.NotMatchedTransAmt;
                textBox7.Text = WRRDMJournalBalCateg.ToString("#,##0.00");
                textBox6.Text = Rs.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                textBox4.Text = (WRRDMJournalBalCateg - Rs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");

                if ((WRRDMJournalBalCateg - Rs.BanksClosedBalAdjWithErrors) == 0)
                {
                    Er.ReadAllErrorsTableForCounters(WOperator, WCategory, "");

                    if (Er.NumOfErrors == Er.ErrUnderAction)
                    {
                        UpdateReconcStatus(1);
                    }
                    else
                    {
                        UpdateReconcStatus(2);
                    }
                }
                else UpdateReconcStatus(2);
            }


            WCurrNm = Rs.GlCurrency;

            try
            {
                // Show errors table 
                if (WCategory == "EWB102") 
                    errfilter = "CategoryId='" + WCategory + "' AND CurDes ='" + WCurrNm
                    + "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5) AND SesNo<=" + WCategMatchingSession + " AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + WCategMatchingSession + "))  ";

                if (WCategory == "EWB311") 
                    errfilter = "CategoryId='" + WCategory + "' AND CurDes ='" + WCurrNm
                  + "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5) AND RMCycle<=" + WCategMatchingSession + " AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + WCategMatchingSession + "))  ";

                Er.ReadErrorsAndFillTable(WOperator, errfilter);

                dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

                dataGridView1.Columns[0].Width = 40; // ExcNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 80; // Desc
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 70; //  Card
                //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

                dataGridView1.Columns[3].Width = 50; // Ccy

                dataGridView1.Columns[4].Width = 80; // Amount
                //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

                dataGridView1.Columns[5].Width = 50; // NeedAction
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[6].Width = 50; // UnderAction 
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //dataGridView1.Columns[7].Width = 50; // ManualAct

                dataGridView1.Columns[7].Width = 90; // DateTime

                dataGridView1.Columns[8].Width = 80; // TransDescr
                
                //dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);

        
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
                buttonUndoAction.Enabled = false; // Undo
                buttonCreateError.Enabled = false; // Create error 

                buttonApplyAction.Hide();
                buttonUndoAction.Hide();
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

            if (WCategory == Er.CategoryId)
            {
                textBox2.Hide();

                WTranNo = Er.TransNo;

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
                        Ht.ReadHostTransTraceNo(WOperator, WCategory, WTraceNo);

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
                    Tc.ReadInPoolTransSpecific(Er.TransNo);
                    // Check to see if in Host for Errors reported in ATM 
                   

                    buttonShowTrans.Show();
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
                    Tc.ReadInPoolAtmTrace(WCategory, Er.TraceNo);

                    Gp.ReadParametersSpecificId(WOperator, "705", Tc.SystemTarget.ToString(), "", "");
                    textBox1.Text = Gp.OccuranceNm;
                }

                if (Er.TraceNo == 0) // ERROR CREATED FOR SUSPENSE 
                {
                    textBox1.Text = "Suspense";
                }

                textBox32.Text = Er.UserComment;

                if (Er.UnderAction == true)
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
                    buttonUndoAction.Show();
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
                    buttonUndoAction.Hide();
                    guidanceMsg = "Study error and decide action.";
                    ChangeBoardMessage(this, new EventArgs());
                    
                }

                // Show Dispute 

                Dt.ReadDisputeTranForInPool(Er.MaskRecordId);
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

            if (WCategory == "EWB102")
            {
                RAtms.ReadReconcCategoriesATMsRMCycleSpecific(WOperator, WCategory, WCategMatchingSession, Er.AtmNo);

                WProcess = 4; // Include corrected errors 

                //ReadAllErrorsTableFromCategSessionForATM(string InCategoryId, int InActionSes, string InAtmNo,  decimal InBanksClosedBal, int InFunction)
                RAtms.ReadAllErrorsTableFromCategSessionForATMAddErrors
                    (WCategory, WCategMatchingSession, Er.AtmNo, (RAtms.OpeningBalance + RAtms.TotalMatchedAmt), WProcess);

                labelCurr.Text = RAtms.Currency;

                textBox3.Text = RAtms.OpeningBalance.ToString("#,##0.00");
                WRRDMJournalBal = RAtms.OpeningBalance + RAtms.TotalJournalAmt;
                textBox5.Text = WRRDMJournalBal.ToString("#,##0.00");
                textBox33.Text = (RAtms.OpeningBalance + RAtms.TotalMatchedAmt) .ToString("#,##0.00");
                //textBox33.Text = RAtms.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                textBox34.Text = (WRRDMJournalBal - (RAtms.OpeningBalance + RAtms.TotalMatchedAmt)).ToString("#,##0.00");


                // Working fields

                WCurrNm = RAtms.Currency;
                WGlOpenBalance = RAtms.OpeningBalance;
                WBalanceWithActions = RAtms.BanksClosedBalAdjWithErrors;

                textBox19.Text = WGlOpenBalance.ToString("#,##0.00");
                textBox18.Text = WRRDMJournalBal.ToString("#,##0.00");
                textBox36.Text = WBalanceWithActions.ToString("#,##0.00");
                textBox35.Text = (WRRDMJournalBal - WBalanceWithActions).ToString("#,##0.00");


                //panel3.Hide(); // HIDE REFRESHED BALANCES 
                //label22.Hide();

                //panel4.Hide(); // HIDE ERROR DETAILS 

            }
            else
            {
                // Category EWB311

                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WCategMatchingSession);

                // Hide First Set of balances
                label6.Hide();
                panel2.Hide();
                labelCurr.Text = Rs.GlCurrency;

                WProcess = 4; // Include corrected errors 
                Rs.ReadAllErrorsTableFromCategSessionGL
                    (WCategory, WCategMatchingSession, Rs.MatchedTransAmt, Rs.GlTodaysBalance, WProcess);

                // Journal is the GL
                textBox19.Text = Rs.GlYesterdaysBalance.ToString("#,##0.00");
                decimal WOpenAndMatched = Rs.GlYesterdaysBalance + Rs.MatchedTransAdjWithErrors;
                textBox18.Text = WOpenAndMatched.ToString("#,##0.00");

                textBox36.Text = Rs.BanksClosedBalAdjWithErrors.ToString("#,##0.00");
                //textBox6.Text = Rs.GlTodaysBalance.ToString("#,##0.00");
                textBox35.Text = (WOpenAndMatched - Rs.BanksClosedBalAdjWithErrors).ToString("#,##0.00");


                // Working fields

                WCurrNm = Rs.GlCurrency;
                WGlOpenBalance = Rs.GlYesterdaysBalance;
                WBalanceWithActions = Rs.GlTodaysBalance;


                panel3.Show(); // HIDE REFRESHED BALANCES 
                label22.Show();

                panel4.Show(); // HIDE ERROR DETAILS 
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

                Dt.ReadDisputeTranForInPool(Er.MaskRecordId);
                if (Dt.RecordFound == true)
                {
                    MessageBox.Show(" Dispute already open for this Error");
                    return;
                }

                int From = 7; // From pre - dispute investigation 
                NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Er.CardNo, Er.MaskRecordId, Er.ErrAmount, 0, textBox32.Text, From, "ATM");
                NForm5.FormClosed += NForm5_FormClosed;
                NForm5.ShowDialog();

                if (DisputeOpenned == false)
                {
                    MessageBox.Show("Dispute was not oppenned! ");
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

            Dt.ReadDisputeTranForInPool(Er.MaskRecordId);
            if (Dt.RecordFound == true)
            {
                DisputeOpenned = true;
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                //buttonMoveToDispute.Hide();
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {

                DisputeOpenned = false;
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
            Dt.ReadDisputeTranForInPool(Er.MaskRecordId);
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

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, WCategory, WCategMatchingSession,Er.MaskRecordId,"View");
            NForm80b.ShowDialog();
        }

        // GO TO CREATE ERROR

        private void buttonCreateError_Click(object sender, EventArgs e)
        {
            if (WRRDMJournalBal - WBalanceWithActions != 0)
            {
                AmountForSuspense = WRRDMJournalBal - WBalanceWithActions;
                NForm14 = new Form14a(WSignedId, WCategory, WCategMatchingSession, WOperator, AmountForSuspense, WCurrNm);
                NForm14.FormClosed += NForm14_FormClosed;
                NForm14.Show();
            }
            else MessageBox.Show("NO AMOUNT FOR SUSPENSE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            Us.ReadSignedActivityByKey(WSignRecordNo);
            Us.ReconcDifferenceStatus = InReconStatus;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }
    }
}

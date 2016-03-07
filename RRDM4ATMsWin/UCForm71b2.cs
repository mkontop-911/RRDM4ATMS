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
    public partial class UCForm71b2 : UserControl
    {
        //TRANSFER FROM PREVIOUS FORM BALANCES AND OTHER INFO 


        Form75 NForm75;
        Form14a NForm14;

        Form79 NForm79;
        Form5 NForm5;

        int WRow;

        string errfilter;

        int LastTrace;
        bool HostRecordForThisError;

        int WErrNo;
        int WTraceNo;
        int WTranNo;
        int WErrId;

        //decimal WTempAdj; 

        int WProcess;

        bool DisputeOpenned;

        bool ViewWorkFlow;
        //    bool Secondary; 
        //      bool RecordFound;

        //    int WCurrCd; 
        string WCurrNm;
        decimal WCountedBal; decimal WAtmBalance; decimal WRepToReplBal;
        decimal WBalGel; 

        decimal AmountForSuspense;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMNotesBalances Na = new RRDMNotesBalances(); // Activate Class 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Activate Class 

        RRDMErrorsClassWithActions Pa = new RRDMErrorsClassWithActions(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMHostTransClass Ht = new RRDMHostTransClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        //RRDMPostedTrans Pt = new RRDMPostedTrans(); 

        // EVENT Handler
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;
        //
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
        int WReconcile;

        public void UCForm71b2Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WReconcile = 1; // 1 = firsts set of OF BALANCE ... We only have one

            InitializeComponent();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            WProcess = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 

            // GET ATM TRACES FROM TRACES 

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            //
            // SHOW VALUES AND BALANCES 
            // 

            guidanceMsg = "Select ERROR from table and make decision.";
            ChangeBoardMessage(this, new EventArgs());

            // REQUEST TO RECONCILE FIRST SET OF BALANCES

            if (WReconcile == 1)
            {
                label21.Text = Na.Balances1.CurrNm;

                textBox3.Text = Na.Balances1.CountedBal.ToString("#,##0.00");
                textBox5.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
              

                textBox33.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

                textBox8.Text = Na.BalDiff1.Machine.ToString("#,##0.00");
                textBox34.Text = (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj).ToString("#,##0.00");

                WCurrNm = Na.Balances1.CurrNm;
                WCountedBal = Na.Balances1.CountedBal;
                WAtmBalance = Na.Balances1.MachineBal;
                WRepToReplBal = Na.Balances1.ReplToRepl;
                WBalGel = Na.Balances1.HostBalAdj;
                
               
            }

            panel3.Hide(); // HIDE REFRESHED BALANCES 
            label22.Hide();

            panel4.Hide(); // HIDE ERROR DETAILS 

            SetScreen(); 
        }


        public void SetScreen()
        {
            try
            {
                // Show errors table 
                errfilter = "AtmNo='" + WAtmNo + "' AND CurDes ='" + WCurrNm
                    + "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5) AND SesNo<=" + WSesNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + WSesNo + "))  ";
                errorsTableBindingSource.Filter = errfilter;
                this.errorsTableTableAdapter.Fill(this.aTMSDataSet65.ErrorsTable);

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
            
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
            Pa.ReadErrorsTableSpecific(WErrNo);

            if (WAtmNo == Pa.AtmNo)
            {
                textBox2.Hide();

                WTranNo = Pa.TransNo;

                WTraceNo = Pa.TraceNo;

                WErrId = Pa.ErrId;

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
                        Ht.ReadHostTransTraceNo(WOperator, WAtmNo, WTraceNo);

                        if (Ht.RecordFound == true)
                        {
                            Pa.CardNo = Ht.CardNo;
                            Pa.CustAccNo = Ht.AccNo;
                            Pa.FullCard = true;

                            Pa.UpdateErrorsTableSpecific(WErrNo);
                        }
                        else
                        {
                            Pa.FullCard = false;
                        }

                    }
                }

                textBox29.Text = " OPEN ";
                textBox22.Text = Pa.ErrDesc;
                textBox27.Text = Pa.DateTime.ToString();
                textBox37.Text = Pa.TraceNo.ToString();
                textBox21.Text = Pa.CircularDesc;

                textBox23.Text = Pa.ErrAmount.ToString("#,##0.00");
                textBox24.Text = Pa.CurDes.ToString();

                // Read to find transactions
                if (Pa.ErrId != 198) // 198 is the correction for suspense has nothing to do with transactions 
                {
                    // VALIDATION OF TRACES STATUS 
                    Tc.ReadInPoolTransSpecific(Pa.TransNo);
                    // Check to see if in Host for Errors reported in ATM 
                    if (Tc.RecordFound == true & (Pa.ErrId == 55 || Pa.ErrId == 175 || Pa.ErrId == 185)) // 55 is presenter error and 175 = missing at Host
                    {
                        if (Tc.SystemTarget == 1) LastTrace = Na.SystemTargets1.LastTrace;
                        if (Tc.SystemTarget == 2) LastTrace = Na.SystemTargets2.LastTrace;
                        if (Tc.SystemTarget == 3) LastTrace = Na.SystemTargets3.LastTrace;
                        if (Tc.SystemTarget == 4) LastTrace = Na.SystemTargets4.LastTrace;
                        if (Tc.SystemTarget == 5) LastTrace = Na.SystemTargets5.LastTrace;

                        if (Pa.TraceNo > LastTrace) // This only happens for presenter error
                        {
                            //   MessageBox.Show(" HOST files Not Available yet ");
                            textBox2.Show();
                            textBox2.Text = "HOST files Not Available yet. You can still take action provided you check Host posting.";
                            HostRecordForThisError = false;
                        }
                        if (Pa.TraceNo <= LastTrace & Pa.ErrId == 55) // Presenter Error read Host File 
                        {
                            // Trace No is within Host file . Check if record exist 
                            Ht.ReadHostTransTraceNo(WOperator, WAtmNo, Pa.TraceNo);

                            if (Ht.RecordFound == true)
                            {
                                Pa.CardNo = Ht.CardNo;
                                Pa.CustAccNo = Ht.AccNo;
                                Pa.FullCard = true;

                                Pa.UpdateErrorsTableSpecific(WErrNo);

                            }
                            else // RECORD FOUND IN HOST 
                            {
                                MessageBox.Show(" Customer was not updated. ");
                                textBox2.Show();
                                textBox2.Text = "Customer was not  updated on Central Systems.";
                                HostRecordForThisError = false;
                                // Do you want to update details?? 
                            }
                        }

                        if (Pa.TraceNo <= LastTrace & (Pa.ErrId == 175 & Pa.FullCard == false))// Missing at Host  
                        {

                            WRow = dataGridView1.SelectedRows[0].Index;
                            // Update Error with Full Card
                            NForm79 = new Form79(WErrNo, Pa.ErrDesc, Pa.CardNo, Pa.CustAccNo);
                            NForm79.FormClosed += NForm79_FormClosed;
                            NForm79.Show();
                        }

                    }

                    button6.Show();
                    textBox25.Text = Pa.CardNo;
                    textBox26.Text = Pa.CustAccNo;
                }

                else // Error = 198
                {
                    textBox37.Text = " N/A ";
                    //       textBox30.Text = " N/A ";
                    button6.Hide();
                    textBox25.Text = " N/A ";
                    textBox26.Text = " N/A ";
                }


                if (Pa.DrCust == true)
                {
                    textBox38.Text = " DEBIT CUSTOMER ";
                }
                if (Pa.CrCust == true)
                {
                    textBox38.Text = " CREDIT CUSTOMER ";
                }
                if (Pa.DrAtmCash == true)
                {
                    textBox39.Text = " DEBIT HOST CASH ";
                }
                if (Pa.CrAtmCash == true)
                {
                    textBox39.Text = " CREDIT HOST CASH ";
                }

                if (Pa.DrCust == false & Pa.CrCust == false)
                {
                    if (Pa.DrAtmCash == true)
                    {
                        textBox38.Text = " DEBIT HOST CASH ";
                    }
                    if (Pa.CrAtmCash == true)
                    {
                        textBox38.Text = " CREDIT HOST CASH ";
                    }

                    if (Pa.DrAtmSusp == true)
                    {
                        textBox39.Text = " DEBIT HOST SUSPENSE";
                    }
                    if (Pa.CrAtmSusp == true)
                    {
                        textBox39.Text = " CREDIT HOST SUSPENSE";
                    }

                }

                if (Pa.TraceNo > 0)
                {
                    Tc.ReadInPoolAtmTrace(WAtmNo, Pa.TraceNo);

                    Gp.ReadParametersSpecificId(WOperator, "705", Tc.SystemTarget.ToString(), "", "");
                    textBox1.Text = Gp.OccuranceNm;
                }

                if (Pa.TraceNo == 0) // ERROR CREATED FOR SUSPENSE 
                {
                    textBox1.Text = "Suspense";
                }

                textBox32.Text = Pa.UserComment;

                if (Pa.UnderAction == true || Pa.DisputeAct == true || Pa.ManualAct)
                {
                    if (Pa.DisputeAct)
                    {
                        radioButtonMoveToDispute.Checked = true;
                    }
                    if (Pa.ManualAct)
                    {
                        radioButtonPostpone.Checked = true;
                    }
                    if (Pa.ManualAct != true & Pa.DisputeAct != true)
                    {
                        radioSystemSuggest.Checked = true;
                    }

                    // Action taken 
                    label5.Text = "Action Taken"; // DONE
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
                    label5.Text = "Action Not Taken"; // 
                    pictureBox3.Show(); // Take Action
                    buttonApplyAction.Show();
                    buttonUndoAction.Hide();

                    guidanceMsg = "Study error and decide action.";
                    ChangeBoardMessage(this, new EventArgs());
                   
                }

                // Show Dispute 

                Dt.ReadDisputeTranForInPool(Pa.MaskRecordId);
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

        }


        // ACTION CHOSEN AND APPLY 
        // REFRESH BALANCES change error status 
        //
        //buttonApplyAction_Click(this, new EventArgs());
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
            if (Pa.UnderAction == true)
            {
                MessageBox.Show(" Action Was already taken. UNDO first and Then Take a new action ");
                return;
            }
            //
            // Do as system suggest 
            //
            if (radioSystemSuggest.Checked)
            {
                
                WRow = dataGridView1.SelectedRows[0].Index;

                // check Dispute 


                if (Pa.TraceNo > LastTrace) // This only happens for presenter error
                {
                    MessageBox.Show(" HOST files Not Available yet. You cannot take action on this error. ");
                    textBox2.Show();
                    textBox2.Text = "HOST files Not Available yet. You can still take action provided you check Host posting.";
                    HostRecordForThisError = false;
                    return;
                }

                if (Pa.ErrId == 55 & WSesNo != Pa.SesNo) // Presenter Error
                {
                    // This an old Error
                    MessageBox.Show(" This is an Old Error. Transactions will be created but Balances will not be affected.");

                    // Update Error Table
                    // 
                    Pa.OpenErr = true;
                    Pa.UnderAction = true;
                    if (radioButtonMoveToDispute.Checked == true)
                    {
                        Pa.DisputeAct = true;
                    }
                    Pa.ManualAct = false;
                    if (String.IsNullOrEmpty(textBox32.Text))
                    {
                        Pa.UserComment = "";
                    }
                    else Pa.UserComment = textBox32.Text;

                    Pa.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                    SetScreen();

                    // SET ROW Selection POSITIONING 
                    dataGridView1.Rows[WRow].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                }

                if ((Pa.DrCust == true & Pa.CrAtmCash == true) || (Pa.CrAtmCash == true & Pa.DrAtmSusp == true))
                {
                    Pa.ErrAmount = -Pa.ErrAmount;
                }

                if (Pa.MainOnly == false) // eg Presenter Error 
                {
                    WAtmBalance = WAtmBalance + Pa.ErrAmount;
                    WRepToReplBal = WRepToReplBal + Pa.ErrAmount;
                    WBalGel = WBalGel + Pa.ErrAmount;
                }
                if (Pa.MainOnly == true) // eg ONLY MAINFRAME AFFECTED 
                {
                    WBalGel = WBalGel + Pa.ErrAmount;
                }
                // SHOW REFRESHED BALANCES 
                panel3.Show();
                label22.Show();

                textBox19.Text = WCountedBal.ToString("#,##0.00");
                textBox18.Text = WAtmBalance.ToString("#,##0.00");
           
                textBox36.Text = WBalGel.ToString("#,##0.00");

                textBox15.Text = (WCountedBal - WAtmBalance).ToString("#,##0.00");
              
                textBox35.Text = (WCountedBal - WBalGel).ToString("#,##0.00");

                // Update Error Table
                // 
                Pa.OpenErr = true;
                Pa.UnderAction = true;
             
                Pa.DisputeAct = false;
                
                Pa.ManualAct = false;
                if (String.IsNullOrEmpty(textBox32.Text))
                {
                    Pa.UserComment = "";
                }
                else Pa.UserComment = textBox32.Text;

                Pa.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                if (Pa.ErrAmount < 0)
                {
                    Pa.ErrAmount = -Pa.ErrAmount; // Turn it to its original value 
                }

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));


                if ((WCountedBal - WAtmBalance) == 0 & (WCountedBal - WRepToReplBal) == 0 & (WCountedBal - WBalGel) == 0)
                {

                    WProcess = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 

                    if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
                    {
                        //   NO DIFFERENCE AT ATM AND HOST 
                        guidanceMsg = " See Refreshed Balances. They do Reconcile. Move to Next ";
                        ChangeBoardMessage(this, new EventArgs());


                    }
                    else  // There is other tan this difference  
                    {
                        guidanceMsg = " You have more items to Reconcile. ";
                        ChangeBoardMessage(this, new EventArgs());
                    }
                }
                else
                {
                    guidanceMsg = " SEE Refreshed Balances. They do not reconcile - You have to take additional action.";
                    ChangeBoardMessage(this, new EventArgs());

                }

            }

            // Dispute 
            // Move to dispute without affecting balances 

            if (radioButtonMoveToDispute.Checked)
            {
                WRow = dataGridView1.SelectedRows[0].Index;

                    Dt.ReadDisputeTranForInPool(Pa.MaskRecordId);
                    if (Dt.RecordFound == true)
                    {
                        MessageBox.Show(" Dispute already open for this Error");
                        return;
                    }

                    int From = 5; // From reconciliation for cash
                    NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Pa.CardNo, Pa.TransNo, Pa.ErrAmount, 0, textBox32.Text, From, "ATM");
                    NForm5.FormClosed += NForm5_FormClosed;
                    NForm5.ShowDialog();

                    if (DisputeOpenned == false)
                    {
                        MessageBox.Show("Dispute was not oppenned! ");
                        return;
                    }

                    // Update Error Table
                    // 
                    Pa.OpenErr = true;
                    Pa.UnderAction = false;
                    Pa.DisputeAct = true;
                    Pa.ManualAct = false;
                    if (String.IsNullOrEmpty(textBox32.Text))
                    {
                        Pa.UserComment = "";
                    }
                    else Pa.UserComment = textBox32.Text;

                    Pa.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                    SetScreen();

                    // SET ROW Selection POSITIONING 
                    dataGridView1.Rows[WRow].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
            
            }

            // Postpone 
            if (radioButtonPostpone.Checked)
            {

                if (Pa.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO if you want a new action ");
                    return;
                }

                WRow = dataGridView1.SelectedRows[0].Index;
                // Update Error Table
                // FOR THIS MANUAL ACTION
                Pa.OpenErr = true;
                Pa.UnderAction = true;
                Pa.DisputeAct = false;
                Pa.ManualAct = true;

                Pa.UpdateErrorsTableSpecific(WErrNo);

                guidanceMsg = " YOU have decided that for this error a manual action will be taken ";
                ChangeBoardMessage(this, new EventArgs());


                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
            }

            if (radioButtonForceClose.Checked)
            {
                MessageBox.Show("Function not develop yet.");

                if (Pa.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO if you want a new action");
                    return;
                }

                return;

            }

        }
   

        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Show Dispute 

            Dt.ReadDisputeTranForInPool(Pa.MaskRecordId);
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

            WRow = dataGridView1.SelectedRows[0].Index;

            if (Pa.ErrId == 55 & WSesNo != Pa.SesNo) // Presenter Error
            {
                // This an old Error
                MessageBox.Show(" This is an Old Error.");

                // INITIALISE USER COMMENT 

                textBox32.Text = "";

                // Update Error Table
                // 
                Pa.OpenErr = true;
                Pa.UnderAction = false;
                Pa.DisputeAct = false;
                Pa.ManualAct = false;
                Pa.UserComment = "";
                Pa.FullCard = false;

                Pa.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            }

            WErrNo = int.Parse(textBox28.Text);

            WRow = dataGridView1.SelectedRows[0].Index;

            if (Pa.UnderAction == true & Pa.ManualAct == false)
            {
                if ((Pa.DrCust == true & Pa.CrAtmCash == true) || (Pa.CrAtmCash == true & Pa.DrAtmSusp == true))
                {
                    // leave balance as is 
                }
                else Pa.ErrAmount = -Pa.ErrAmount;

                if (Pa.MainOnly == false) // eg Presenter Error 
                {
                    WAtmBalance = WAtmBalance + Pa.ErrAmount;
                    WRepToReplBal = WRepToReplBal + Pa.ErrAmount;
                    WBalGel = WBalGel + Pa.ErrAmount;
                    //WABalGel = WABalGel + Pa.ErrAmount;
                }
                if (Pa.MainOnly == true) // eg Double Entry 
                {
                    WBalGel = WBalGel + Pa.ErrAmount;
                    //WABalGel = WABalGel + Pa.ErrAmount;
                }

                textBox19.Text = WCountedBal.ToString("#,##0.00");
                textBox18.Text = WAtmBalance.ToString("#,##0.00");
        
                textBox36.Text = WBalGel.ToString("#,##0.00");

                textBox15.Text = (WCountedBal - WAtmBalance).ToString("#,##0.00");
        
                textBox35.Text = (WCountedBal - WBalGel).ToString("#,##0.00");

                if ((WCountedBal - WAtmBalance) == 0 & (WCountedBal - WRepToReplBal) == 0 & (WCountedBal - WBalGel) == 0)
                {
                    // THIS BALANCE AND ERRORS RECONCILE 
                    guidanceMsg = " BALANCES RECONCILE - YOU CAN MOVE TO NEXT ";
                    ChangeBoardMessage(this, new EventArgs());

                }
                else
                {
                    guidanceMsg = " BALANCES DO NOT RECONCILE - TAKE ACTIONS TO CORRECT SITUATION ";
                    ChangeBoardMessage(this, new EventArgs());

                }

                // INITIALISE USER COMMENT 

                textBox32.Text = "";

                // Update Error Table
                // 
                Pa.OpenErr = true;
                Pa.UnderAction = false;
                Pa.DisputeAct = false;
                Pa.ManualAct = false;
                Pa.UserComment = "";
                Pa.FullCard = false;
                Pa.UpdateErrorsTableSpecific(WErrNo);

                // Return Value back to possitive 
                //   ErrAmount = -ErrAmount; 

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
                // Return Error Amount to its original value

                if (Pa.ErrAmount < 0)
                {
                    Pa.ErrAmount = -Pa.ErrAmount; // Make it possitive 
                }

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
                Pa.OpenErr = true;
                Pa.UnderAction = false;
                Pa.DisputeAct = false;
                Pa.ManualAct = false;
                Pa.UserComment = "";
                Pa.UpdateErrorsTableSpecific(WErrNo); // UPDATE ERROR TABLE 

                // Show errors table 

                errorsTableBindingSource.Filter = errfilter;
                this.errorsTableTableAdapter.Fill(this.aTMSDataSet65.ErrorsTable);

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                //         dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRow));
                // Initialise Radio Buttons 
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
            Dt.ReadDisputeTranForInPool(Pa.MaskRecordId);
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
        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox28.Text))
            {
                MessageBox.Show("Choose An Error Please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string FilterATM = "AtmNo='" + WAtmNo + "'" + " AND SesNo =" + WSesNo + " AND AtmTraceNo=" + WTraceNo;

            String FilterHost = "AtmNo='" + WAtmNo + "'" + " AND TraceNumber=" + WTraceNo;

            if (WErrId < 200)
            {
                NForm75 = new Form75(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, FilterATM, FilterHost, WTranNo);
                NForm75.Show();
            }
            else
            {
                MessageBox.Show("This is an error created by system and do not have transactions");
            }
            // NForm29 = new Form29(WAtmNo, WSesNo, Pa.TraceNo);
            //   NForm29.Show();
        }

        //
        // GO TO CREATE ERROR
        //
        private void buttonCreateError_Click(object sender, EventArgs e)
        {
            if (WCountedBal - WBalGel != 0)
            {
                AmountForSuspense = WCountedBal - WBalGel;
                NForm14 = new Form14a(WSignedId, WAtmNo, WSesNo, WOperator, AmountForSuspense, WCurrNm);
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

   

     

  
     
    }
}

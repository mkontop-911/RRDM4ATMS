using System;
using System.Windows.Forms;
using RRDM4ATMs;
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
        int WUniqueRecordId;
        int WErrId;

        //decimal WTempAdj; 

        int WProcess;

        bool DisputeOpened;

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

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions(); // Make class availble 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMReconcCategories Rc = new RRDMReconcCategories();

        string WReconcCategoryId;

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
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
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

                Er.ReadErrorsAndFillTable(WOperator, WSignedId, errfilter);

                dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

                if (dataGridView1.Rows.Count == 0)
                {
                    Form2 MessageForm = new Form2("No Entries availble");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

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
            Er.ReadErrorsTableSpecific(WErrNo);

            Ac.ReadAtm(Er.AtmNo);

            Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);

            WReconcCategoryId = Rc.CategoryId; 

            if (WAtmNo == Er.AtmNo)
            {
                textBox2.Hide();

                WUniqueRecordId = Er.UniqueRecordId;

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
                        Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId,1); 
                     
                        if (Mpa.RecordFound == true)
                        {
                            Er.CardNo = Mpa.CardNumber;
                            Er.CustAccNo = Mpa.AccNumber; 
                            Er.FullCard = true;

                            Er.UpdateErrorsTableSpecific(WErrNo);
                        }
                        else
                        {
                            Er.FullCard = false;
                        }

                    }
                }

                // Find Recnciliation 
                
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
                    //Tc.ReadInPoolTransSpecific(Er.UniqueRecordId);
                    // Check to see if in Host for Errors reported in ATM 
                    if (Tc.RecordFound == true & (Er.ErrId == 55 || Er.ErrId == 175 || Er.ErrId == 185)) // 55 is presenter error and 175 = missing at Host
                    {
                        if (Tc.SystemTarget == 1) LastTrace = Na.SystemTargets1.LastTrace;
                        if (Tc.SystemTarget == 2) LastTrace = Na.SystemTargets2.LastTrace;
                        if (Tc.SystemTarget == 3) LastTrace = Na.SystemTargets3.LastTrace;
                        if (Tc.SystemTarget == 4) LastTrace = Na.SystemTargets4.LastTrace;
                        if (Tc.SystemTarget == 5) LastTrace = Na.SystemTargets5.LastTrace;

                        if (Er.TraceNo > LastTrace) // This only happens for presenter error
                        {
                            //   MessageBox.Show(" HOST files Not Available yet ");
                            textBox2.Show();
                            textBox2.Text = "HOST files Not Available yet. You can still take action provided you check Host posting.";
                            HostRecordForThisError = false;
                        }
                        if (Er.TraceNo <= LastTrace & Er.ErrId == 55) // Presenter Error read Host File 
                        {
                            // Trace No is within Host file . Check if record exist 
                            Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId,1);

                            if (Mpa.RecordFound == true)
                            {
                                Er.CardNo = Mpa.CardNumber;
                                Er.CustAccNo = Mpa.AccNumber;
                                Er.FullCard = true;

                                Er.UpdateErrorsTableSpecific(WErrNo);
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

                        if (Er.TraceNo <= LastTrace & (Er.ErrId == 175 & Er.FullCard == false))// Missing at Host  
                        {

                            WRow = dataGridView1.SelectedRows[0].Index;
                            // Update Error with Full Card
                            NForm79 = new Form79(WErrNo, Er.ErrDesc, Er.CardNo, Er.CustAccNo);
                            NForm79.FormClosed += NForm79_FormClosed;
                            NForm79.Show();
                        }
                    }

                    button6.Show();
                    textBox25.Text = Er.CardNo;
                    textBox26.Text = Er.CustAccNo;
                }
                else // Error = 198
                {
                    textBox37.Text = " N/A ";
                    //       textBox30.Text = " N/A ";
                    button6.Hide();
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
                    textBox39.Text = " DEBIT HOST CASH ";
                }
                if (Er.CrAtmCash == true)
                {
                    textBox39.Text = " CREDIT HOST CASH ";
                }

                if (Er.DrCust == false & Er.CrCust == false)
                {
                    if (Er.DrAtmCash == true)
                    {
                        textBox38.Text = " DEBIT HOST CASH ";
                    }
                    if (Er.CrAtmCash == true)
                    {
                        textBox38.Text = " CREDIT HOST CASH ";
                    }

                    if (Er.DrAtmSusp == true)
                    {
                        textBox39.Text = " DEBIT HOST SUSPENSE";
                    }
                    if (Er.CrAtmSusp == true)
                    {
                        textBox39.Text = " CREDIT HOST SUSPENSE";
                    }

                }

                if (Er.TraceNo > 0)
                {
                    Tc.ReadInPoolAtmTrace(WAtmNo, Er.TraceNo);

                    Gp.ReadParametersSpecificId(WOperator, "705", Tc.SystemTarget.ToString(), "", "");
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
            if (Er.UnderAction == true)
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


                if (Er.TraceNo > LastTrace) // This only happens for presenter error
                {
                    MessageBox.Show(" HOST files Not Available yet. You cannot take action on this error. ");
                    textBox2.Show();
                    textBox2.Text = "HOST files Not Available yet. You can still take action provided you check Host posting.";
                    HostRecordForThisError = false;
                    return;
                }

                if (Er.ErrId == 55 & WSesNo != Er.SesNo) // Presenter Error
                {
                    // This an old Error
                    MessageBox.Show(" This is an Old Error. Transactions will be created but Balances will not be affected.");

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

                    SetScreen();

                    // SET ROW Selection POSITIONING 
                    dataGridView1.Rows[WRow].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                }

                if ((Er.DrCust == true & Er.CrAtmCash == true) || (Er.CrAtmCash == true & Er.DrAtmSusp == true))
                {
                    Er.ErrAmount = -Er.ErrAmount;
                }

                if (Er.MainOnly == false) // eg Presenter Error 
                {
                    WAtmBalance = WAtmBalance + Er.ErrAmount;
                    WRepToReplBal = WRepToReplBal + Er.ErrAmount;
                    WBalGel = WBalGel + Er.ErrAmount;
                }
                if (Er.MainOnly == true) // eg ONLY MAINFRAME AFFECTED 
                {
                    WBalGel = WBalGel + Er.ErrAmount;
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
                Er.OpenErr = true;
                Er.UnderAction = true;
             
                Er.DisputeAct = false;
                
                Er.ManualAct = false;
                if (String.IsNullOrEmpty(textBox32.Text))
                {
                    Er.UserComment = "";
                }
                else Er.UserComment = textBox32.Text;

                Er.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                if (Er.ErrAmount < 0)
                {
                    Er.ErrAmount = -Er.ErrAmount; // Turn it to its original value 
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

                    Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
                    if (Dt.RecordFound == true)
                    {
                        MessageBox.Show(" Dispute already open for this Error");
                        return;
                    }

                    int From = 5; // From reconciliation for cash
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

            // Postpone 
            if (radioButtonPostpone.Checked)
            {

                if (Er.UnderAction == true)
                {
                    MessageBox.Show(" Action Was already taken. UNDO if you want a new action ");
                    return;
                }

                WRow = dataGridView1.SelectedRows[0].Index;
                // Update Error Table
                // FOR THIS MANUAL ACTION
                Er.OpenErr = true;
                Er.UnderAction = true;
                Er.DisputeAct = false;
                Er.ManualAct = true;

                Er.UpdateErrorsTableSpecific(WErrNo);

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
            // Show Dispute 

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

            WRow = dataGridView1.SelectedRows[0].Index;

            if (Er.ErrId == 55 & WSesNo != Er.SesNo) // Presenter Error
            {
                // This an old Error
                MessageBox.Show(" This is an Old Error.");

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

                Er.UpdateErrorsTableSpecific(WErrNo); // Update errors 

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            }

            WErrNo = int.Parse(textBox28.Text);

            WRow = dataGridView1.SelectedRows[0].Index;

            if (Er.UnderAction == true & Er.ManualAct == false)
            {
                if ((Er.DrCust == true & Er.CrAtmCash == true) || (Er.CrAtmCash == true & Er.DrAtmSusp == true))
                {
                    // leave balance as is 
                }
                else Er.ErrAmount = -Er.ErrAmount;

                if (Er.MainOnly == false) // eg Presenter Error 
                {
                    WAtmBalance = WAtmBalance + Er.ErrAmount;
                    WRepToReplBal = WRepToReplBal + Er.ErrAmount;
                    WBalGel = WBalGel + Er.ErrAmount;
                    //WABalGel = WABalGel + Pa.ErrAmount;
                }
                if (Er.MainOnly == true) // eg Double Entry 
                {
                    WBalGel = WBalGel + Er.ErrAmount;
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
                Er.OpenErr = true;
                Er.UnderAction = false;
                Er.DisputeAct = false;
                Er.ManualAct = false;
                Er.UserComment = "";
                Er.FullCard = false;
                Er.UpdateErrorsTableSpecific(WErrNo);

                // Return Value back to possitive 
                //   ErrAmount = -ErrAmount; 

                SetScreen();

                // SET ROW Selection POSITIONING 
                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
                // Return Error Amount to its original value

                if (Er.ErrAmount < 0)
                {
                    Er.ErrAmount = -Er.ErrAmount; // Make it possitive 
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
                Er.OpenErr = true;
                Er.UnderAction = false;
                Er.DisputeAct = false;
                Er.ManualAct = false;
                Er.UserComment = "";
                Er.UpdateErrorsTableSpecific(WErrNo); // UPDATE ERROR TABLE 

                // Show errors table 

                Er.ReadErrorsAndFillTable(WOperator, WSignedId, errfilter);

                dataGridView1.DataSource = Er.ErrorsTable.DefaultView;

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
        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox28.Text))
            {
                MessageBox.Show("Choose An Error Please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (WErrId < 200)
            {
                NForm75 = new Form75(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, WUniqueRecordId);
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
                int RMCycle = 0;
                NForm14 = new Form14a(WSignedId, "EWB110", Er.RMCycle, Er.UniqueRecordId, WAtmNo, WSesNo, WOperator, AmountForSuspense, WCurrNm);
            
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

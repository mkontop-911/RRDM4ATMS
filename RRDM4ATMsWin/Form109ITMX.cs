using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text; 

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form109ITMX : Form
    {
        Form110 NForm110; // Authoriser
        Form112 NForm112;
        
        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;

        RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();
        RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();
        RRDMErrorsClassWithActionsITMX Ec = new RRDMErrorsClassWithActionsITMX();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int TransType;
        bool ErrorReturn;
        bool DisputeAuthorisation; 

        string StageDescr;

        Bitmap SCREENinitial;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WDisputeNumber;
        int WDispTranNo; 
     
        int WMaskRecordId;

        int WSource; 
        int WAuthorSeqNumber;

        public Form109ITMX(string InSignedId, int InSignRecordNo, string InOperator, int InDisputeNumber, int InDispTranNo, int InMaskRecordId, int InSource)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WDisputeNumber = InDisputeNumber;
            WDispTranNo = InDispTranNo;

            WMaskRecordId = InMaskRecordId;

            WSource = InSource; // 1 = comes from Dispute management, 2 = comes from Authorisation Management, 
                                // 11 comes from dispute management for settled dispute transaction to view only 
             
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

         //   labelStep1.Text = "Investigation for Dispute No: " + WDispNo;
            textBoxMsgBoard.Text = "Choose Action to be taken. Based on action a transaction to be posted will be created. ";

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false; 
            }
            else NormalProcess = true; 
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageTwo(WDisputeNumber, WDispTranNo, WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
            }
            //************************************************************
            //************************************************************

            SetScreen(); 
        }

        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            // CHECK IF Dispute ELECTRONIC AUTHORISATION IS NEEDED
            string ParId = "260";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            // NO Cash In Management                                         
            if (Gp.OccuranceNm == "YES")
            {
                DisputeAuthorisation = true;
            }
            else
            {
                DisputeAuthorisation = false;
            }
            //
            // SET MAIN SCREEN
            //
            Dt.ReadDisputeTran(WDispTranNo);

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId); 

            label3.Text = "Transaction is in : " + Mp.Ccy;
            textBox1.Text = WMaskRecordId.ToString();
          if (Dt.TxnCode == 11)
            {
                textBox9.Text = Mp.MobileRequestor;
                textBox3.Text = Mp.AccountRequestor;
                radioButton3.Checked = true;

            }
            if (Dt.TxnCode == 21)
            {
                radioButton1.Checked = true; 
                textBox9.Text = Mp.MobileBeneficiary;
                textBox3.Text = Mp.AccountBeneficiary;
            }
            textBox10.Text = Mp.Amount.ToString("#,##0.00");
            textBox11.Text = Mp.Particulars;
            label18.Text = "Date : " + Mp.ExecutionTxnDtTm.ToString();

            Di.ReadDispute(Dt.DisputeNumber);
            label15.Text = "Customer : " + Di.CustName;

            textBox5.Text = Dt.DisputedAmt.ToString("#,##0.00");

            // NOTES for Attachements 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + WDisputeNumber.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            if (Dt.DisputeActionId > 0)
            {
                if (Dt.DisputeActionId == 1) // CREDIT CUSTOMER
                {
                    radioButton1.Checked = true;
                }
            
                if (Dt.DisputeActionId == 3) // DEBIT CUSTOMER 
                {
                    radioButton3.Checked = true;
                }
              
                if (Dt.DisputeActionId == 5) // POSTPONED FOR CERTAIN DATE TIME
                {
                    radioButton5.Checked = true;
                    dateTimePicker1.Value = Dt.PostDate;
                }
                if (Dt.DisputeActionId == 6)
                {
                    radioButton6.Checked = true;
                }
                if (Dt.DisputeActionId == 7)
                {
                    radioButton7.Checked = true;
                }
            }

            if (Dt.ReasonForAction > 0)
            {
                if (Dt.ReasonForAction == 1)
                {
                    radioButton11.Checked = true;
                }
                if (Dt.ReasonForAction == 2)
                {
                    radioButton12.Checked = true;
                }
                if (Dt.ReasonForAction == 3)
                {
                    radioButton13.Checked = true;
                }
                if (Dt.ReasonForAction == 4)
                {
                    radioButton14.Checked = true;
                }            
            }

            if (Dt.ClosedDispute == true & WSource != 11) // DISPUTE transaction is closed and need to reopen
            {
                checkBox1.Show();
                panel4.Hide();
                panel3.Hide();
                label2.Hide();
                label4.Hide();
               
                buttonAuthor.Hide();
                buttonFinish.Show();
                textBoxMsgBoard.Text = "Check box to ReOpen Disputed Transaction. Comments should be input too.";
                return;
            }
            else
            {
                checkBox1.Hide();
            }
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(panel2.Width, panel2.Height);
            panel2.DrawToBitmap(memoryImage, panel2.ClientRectangle);
            SCREENinitial = memoryImage;
            //*****************************************************
            //*****************************************************
            // Set authorisation section
            //
            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        //  guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        //buttonNotesRead.Hide();
                        //buttonNotesUpdate.Show();
                    }

                    // Show Authorisation record 
                    ShowAuthorisationInfo();
                }
            }

            if (NormalProcess & DisputeAuthorisation == true) // Normal Reconciliation with authorisation 
            {
                // Main buttons
                buttonAuthor.Show();
                buttonRefresh.Hide();
                buttonFinish.Hide();
            } 

            if (NormalProcess & DisputeAuthorisation == false) // Normal Reconciliation without authorisation 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                //buttonAuthorisations.Hide();
            }

            if (NormalProcess) // Normal Reconciliation 
            {
                // Do not show Authorisation Section this will be shown after authorisation 
                labelAutherHeader.Hide();
                panelAuthor.Hide();
            }

            if (NormalProcess & Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorisation was rejected - mode has turned to 2
            {
                // Show Authorisation section
                labelAutherHeader.Show();
                panelAuthor.Show();
                ShowAuthorisationInfo();

                // Close Authorisation record 
                //
                Ap.Stage = 5;
                Ap.OpenRecord = false;

                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
            }

            if ((WViewFunction == true & DisputeAuthorisation == true) || (WViewFunction == true & DisputeAuthorisation == true)
                  || WAuthoriser == true) // Comes from Author
            {
                ShowAuthorisationInfo();
            }
            else
            {
                if (WViewFunction == true & DisputeAuthorisation == false)
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();

                    // Do not show Authorisation Section 
                    labelAutherHeader.Hide();
                    panelAuthor.Hide();
                }
            }        
        }
 
        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {
            //Bitmap SCREENb;
            //System.Drawing.Bitmap memoryImage;
            //memoryImage = new System.Drawing.Bitmap(panel2.Width, panel2.Height);
            //panel2.DrawToBitmap(memoryImage, panel2.ClientRectangle);
            //SCREENb = memoryImage;

            //AuditTrailClass At = new AuditTrailClass();

            //if (AuditTrailUniqueID.Equals(""))
            //{
            //    AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            //}
            //else
            //{
            //    At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            //}
        }
// Update 

        private void ButtonFinish_Click(object sender, EventArgs e)
        {

            ErrorReturn = false;

            //Close Authorization Record
            // create a connection object
            using (var scope = new System.Transactions.TransactionScope())
                try
                {
                    // Update Dispute Tran 
                    // Here we update the dispute and its transaction
                    //   
                    InputValidationAndUpdate("Update"); 

                    if (ErrorReturn == true) return;

                    if (DisputeAuthorisation == true)
                    {
                        Ap.Stage = 5;
                        Ap.OpenRecord = false;

                        Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);
                    }

                    // Create transactions to be posted 

                    if (Dt.DisputeActionId < 5)
                    {
                        Ec.CreateTransTobepostedfromDisputes(WOperator, WSignedId, Ap.Authoriser, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                        textBoxMsgBoard.Text = "A Transaction to be posted is created. ";

                        //MessageBox.Show("No transactions to be posted");
                        Form2 MessageForm = new Form2("Action on Dispute Transaction Updated -" + Environment.NewLine
                                                       + "Transaction to be posted was created as a result" + Environment.NewLine
                                                       );
                        MessageForm.ShowDialog();

                        scope.Complete(); 

                        this.Dispose();
                    }
                }
                catch (Exception ex)
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

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                        + " . Application will be aborted! Call controller to take care. ");

                    Environment.Exit(0);
                }
            finally
                {
                    scope.Dispose();
                }
            //AUDIT TRAIL 
            string AuditCategory = "Operation";
            string AuditSubCategory = "Disputes";
            string AuditAction = "Update";
            string Message = "Action Updated";
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

        }
       
        // Input Validation and updating 
        private void InputValidationAndUpdate(string InFunction)
        {

            Dt.ReadDisputeTran(WDispTranNo);
            if (checkBox1.Checked == true & Dt.ClosedDispute == true)
            {     
                checkBox1.Hide();
                checkBox1.Checked = false;
                panel4.Show();
                panel3.Show();
                label2.Show();
                label4.Show();
                textBoxMsgBoard.Text = "Choose Action to be taken. Based on action a transaction will be created. ";

                Dt.ClosedDispute = false;
                Dt.OpenDispTran = Dt.OpenDispTran + 1; 
                Dt.DecidedAmount = 0;
                Dt.UpdateDisputeTranRecord(WDispTranNo);

                Di.ReadDispute(Dt.DisputeNumber);
                Di.Active = true;
                Di.UpdateDisputeRecord(Dt.DisputeNumber);

                Ap.DeleteAuthorisationRecord(Dt.AuthorKey); 

                MessageBox.Show(" Tran is now active");
                buttonAuthor.Show();
                buttonFinish.Hide();
                ErrorReturn = true;
                return;
            }

            if (Dt.ClosedDispute == true & checkBox1.Checked == false)
            {
                MessageBox.Show("Dispute Tran is closed. Open it to proceed");
                ErrorReturn = true;
                return;
            }
            // CHECK INPUT 
            if (radioButton1.Checked == false  & radioButton3.Checked == false &
                          radioButton6.Checked == false & radioButton7.Checked == false)
            {
                MessageBox.Show(" Please Choose Dispute Action");
                ErrorReturn = true;
                return;
            }
            //else
            //{

            //    Dt.UpdateDisputeTranRecord(WMaskRecordId);
            //}

            if (radioButton11.Checked == false & radioButton12.Checked == false & radioButton13.Checked == false &
                radioButton14.Checked == false )
            {
                MessageBox.Show(" Please Choose Dispute Reason");
                ErrorReturn = true;
                return;
            }

            // DISPUTE ACTION ID
            //
            if (radioButton1.Checked == true)  // CREDIT CUSTOMER 
            {
                Dt.DisputeActionId = 1;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 21;
            }

            if (radioButton3.Checked == true)  // DEDIT CUSTOMER 
            {
              
                Dt.DisputeActionId = 3;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 11;
                //     Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //    textBoxMsgBoard.Text = "A Transaction to be posted is created. ";
            }

           
            if (radioButton5.Checked == true)
            {
                if (radioButton5.Checked == true) Dt.DisputeActionId = 5; // Postponed 
                Dt.PostDate = dateTimePicker1.Value;
                if (DateTime.Now >= Dt.PostDate.Date) 
                {
                    MessageBox.Show("Please enter valid date greater than today!");
                    ErrorReturn = true;
                    return;
                }

            }
           
            if (radioButton6.Checked == true) Dt.DisputeActionId = 6; // Not accepted
            if (radioButton7.Checked == true) Dt.DisputeActionId = 7; // Legal action 

            // DISPUTE REASON 
            if (radioButton11.Checked == true) Dt.ReasonForAction = 1;
            if (radioButton12.Checked == true) Dt.ReasonForAction = 2;
            if (radioButton13.Checked == true) Dt.ReasonForAction = 3;
            if (radioButton14.Checked == true) Dt.ReasonForAction = 4;
        

            if (InFunction == "Update" &
                (Dt.DisputeActionId == 1 || Dt.DisputeActionId == 2 || Dt.DisputeActionId == 3
                || Dt.DisputeActionId == 4 || Dt.DisputeActionId == 6 )
               )
            {
                Dt.ClosedDispute = true;
            }
            if (InFunction == "Authorization")
            {
                Dt.ChooseAuthor = true; 
            }

            // UPDATE Disputes Transaction record
            Dt.UpdateDisputeTranRecord(WDispTranNo);
            if (Dt.ErrorFound)
            {
                MessageBox.Show(Dt.ErrorOutput, "System Error During Updating",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorReturn = true;
                return; 
            }

            Dt.ReadAllTranForDispute(Dt.DisputeNumber);

            if (Dt.OpenDispTran == 0)
            {
                Di.ReadDispute(Dt.DisputeNumber);

                Di.CloseDate = DateTime.Now;

                Di.Active = false;

                Di.UpdateDisputeRecord(Dt.DisputeNumber);

                if (Di.ErrorFound)
                {
                    MessageBox.Show("ERROR", Dt.ErrorOutput);
                    ErrorReturn = true;
                    return;
                }
                
            }             
        }    

        // AUTHORISER SECTION 
        // Authorise 

        private void ButtonAutho(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES"); 
        }
       

        // Reject Authorization 

        private void buttonReject_Click(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");
        }

// FINISH Update Authorization Record 
     
        private void UpdateAuthorRecord(string InDecision)
        {
            Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
            if (Ap.OpenRecord == true)
            {
                Ap.AuthDecision = InDecision;
                if (textBoxComment.TextLength > 0)
                {
                    Ap.AuthComment = textBoxComment.Text;
                }
                Ap.DateAuthorised = DateTime.Now;
                Ap.Stage = 3;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);

                if (InDecision == "YES")
                {
                    MessageBox.Show("Authorization ACCEPTED! by : " + labelAuthoriser.Text);
                    this.Dispose();
                }
                if (InDecision == "NO")
                {
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return; 
            }      

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {
            Dt.ReadDisputeTran(WMaskRecordId);

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);

            if ((Ap.RecordFound == true & Ap.OpenRecord == true)
                   || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            {
                labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

                if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
                if (Ap.Stage == 2) StageDescr = "Authoriser got the message. He will get action.";
                if (Ap.Stage == 3) StageDescr = "Authoriser took action. Requestor must act. ";
                if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                {
                    StageDescr = "Authorization accepted. Ready for Finish";
                }
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

                labelAuthStatus.Text = "Current Status : " + StageDescr;

                Us.ReadUsersRecord(Ap.Requestor);
                labelRequestor.Text = "Requestor : " + Us.UserName;

                Us.ReadUsersRecord(Ap.Authoriser);
                labelAuthoriser.Text = "Authoriser : " + Us.UserName;

                textBoxComment.Text = Ap.AuthComment;

                WAuthorSeqNumber = Ap.SeqNumber;
            }
            else
            {
                return;
            }

            labelAutherHeader.Show();
            panelAuthor.Show();

            if (WViewFunction == true) // For View only 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                buttonFinish.Hide();
                // Authoriser
                buttonAuthorise.Hide();
                buttonReject.Hide();
                textBoxComment.ReadOnly = true;
            }

            if (WAuthoriser == true & (Ap.Stage == 2 || Ap.Stage == 3)) // For Authoriser from author management 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                buttonFinish.Hide();
                // Authoriser
                buttonAuthorise.Show();
                buttonReject.Show();
                textBoxComment.ReadOnly = false;
            }

            if (WAuthoriser == true & Ap.Stage == 4) // For Authoriser from author management 
            {
                // Main buttons
                buttonAuthor.Hide();
                buttonRefresh.Hide();
                buttonFinish.Hide();
                // Authoriser
                buttonAuthorise.Hide();
                buttonReject.Hide();
                textBoxComment.ReadOnly = true;
            }

            if (WRequestor == true || NormalProcess) // For Requestor from author management 
            {
                if (Ap.Stage < 3) // Not authorise yet
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Show();
                    buttonFinish.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }
                if (Ap.Stage == 4 & Ap.AuthDecision == "YES") // Authorised and accepted
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    buttonFinish.Show();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;

                }
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorised but rejected
                {
                    // Main buttons
                    buttonAuthor.Show();
                    buttonRefresh.Hide();
                    buttonFinish.Hide();
                    // Authoriser
                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }
            }

        }

        // REFRESH 

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            SetScreen();
        }
        // 
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            ErrorReturn = false;

            // Check if Already in authorization process

            Dt.ReadDisputeTran(WMaskRecordId);

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // NOT COMMING FROM REOPENED DISPUTE 
            {
                MessageBox.Show("This Dispute Record Already has authorization record!");
                return;
            }

            // Validate input 
            InputValidationAndUpdate("Authorisation");
            string WOrigin = "Dispute Action";
            string WAtmNo = "";
            int WReplCycle = 0 ; 
            if (ErrorReturn == true) return;
            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110= new Form110(WSignedId, WSignRecordNo, WOperator, 
                WOrigin, WMaskRecordId, WAtmNo, WReplCycle, AuthorSeqNumber,0,"" ,0,"Normal"); 
            NForm110.FormClosed +=NForm110ITMX_FormClosed;
            NForm110.ShowDialog();

        }

        void NForm110ITMX_FormClosed(object sender, FormClosedEventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true; 

            Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
            
            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";
            }

            SetScreen();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
// History of authorisation 
       

        private void buttonHistory_Click(object sender, EventArgs e)
        {
            string WAtmNo = "";
            int WReplCycle = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WReplCycle, Di.DispId, Dt.DispTranNo, "", 0);
            NForm112.ShowDialog();
        }

// Attached Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            string SearchP4 = "";
            string WMode;
            if (WViewFunction == false) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();
        }

     
        
    }
}


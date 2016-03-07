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
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form109 : Form
    {
        Form110 NForm110; // Authoriser
        Form112 NForm112;
        
        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess; 

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
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
     //   string AuditTrailUniqueID = "";

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WDisputeNumber;
        int WDispTranNo; 
     
        int WTranNo;

        int WSource; 
        int WAuthorSeqNumber;

        public Form109(string InSignedId, int InSignRecordNo, string InOperator, int InDisputeNumber, int InDispTranNo, int InTranNo, int InSource)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WDisputeNumber = InDisputeNumber;
            WDispTranNo = InDispTranNo;

            WTranNo = InTranNo;

            WSource = InSource; // 1 = comes from Dispute management, 2 = comes from Authorisation Management, 
                                // 11 comes from dispute management for settled dispute transaction to view only 
             
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

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
          
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

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
            Dt.ReadDisputeTran(WTranNo);

            label3.Text = "Transaction is in : " + Dt.CurrencyNm;
            textBox1.Text = WTranNo.ToString();
          
            textBox9.Text = Dt.CardNo;
            textBox3.Text = Dt.AccNo;
            textBox10.Text = Dt.TranAmount.ToString("#,##0.00");
            textBox11.Text = Dt.TransDesc;
            label18.Text = "Date : " + Dt.TranDate.ToString();
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
                if (Dt.DisputeActionId == 2)
                {
                    radioButton2.Checked = true;
                    textBox14.Text = Dt.DecidedAmount.ToString();
                }
                if (Dt.DisputeActionId == 3) // DEBIT CUSTOMER 
                {
                    radioButton3.Checked = true;
                }
                if (Dt.DisputeActionId == 4)
                {
                    radioButton4.Checked = true;
                    textBox4.Text = Dt.DecidedAmount.ToString();
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
                if (Dt.ReasonForAction == 5)
                {
                    radioButton15.Checked = true;
                }
                if (Dt.ReasonForAction == 6)
                {
                    radioButton16.Checked = true;
                }
            }

            textBoxActionComments.Text = Dt.ActionComment;

            if (Dt.ClosedDispute == true & WSource != 11) // DISPUTE transaction is closed and need to reopen
            {
                checkBox1.Show();
                panel4.Hide();
                panel3.Hide();
                label2.Hide();
                label4.Hide();
                textBoxActionComments.Text = "";
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
                        Us.ReadSignedActivityByKey(WSignRecordNo);
                        Us.ProcessNo = 2; // Return to stage 2  
                        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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
                labelAuthHeading.Hide();
                panelAuthor.Hide();
            }

            if (NormalProcess & Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorisation was rejected - mode has turned to 2
            {
                // Show Authorisation section
                labelAuthHeading.Show();
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
                    labelAuthHeading.Hide();
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

            // Update Dispute Tran 
            InputValidationAndUpdate("Update");

            if (ErrorReturn == true) return;

            //Close Authorization Record

            if (DisputeAuthorisation == true)
            {
                Ap.Stage = 5;
                Ap.OpenRecord = false;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);
            }

            // Create transactions to be posted 

            if (Dt.DisputeActionId < 5)
            {
                Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                textBoxMsgBoard.Text = "A Transaction to be posted is created. ";

                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("Action on Dispute Transaction Updated -" + Environment.NewLine
                                               + "Transaction to be posted was created as a result" + Environment.NewLine
                                               );
                MessageForm.ShowDialog();

                this.Dispose();
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

            Dt.ReadDisputeTran(WTranNo);
            if (checkBox1.Checked == true & Dt.ClosedDispute == true)
            {
                if (textBoxActionComments.Text.Length < 3)
                {
                    MessageBox.Show(" Please Enter Comments");
                    ErrorReturn = true;
                    return;
                }

                checkBox1.Hide();
                checkBox1.Checked = false;
                panel4.Show();
                panel3.Show();
                label2.Show();
                label4.Show();
                textBoxMsgBoard.Text = "Choose Action to be taken. Based on action a transaction will be created. ";

                Dt.ReadDisputeTran(WTranNo);

                Dt.ClosedDispute = false;
                Dt.OpenDispTran = Dt.OpenDispTran + 1; 
                Dt.DecidedAmount = 0;
                Dt.UpdateDisputeTranRecord(WTranNo);

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
            if (radioButton1.Checked == false & radioButton2.Checked == false & radioButton3.Checked == false &
                 radioButton4.Checked == false & radioButton5.Checked == false &
                radioButton6.Checked == false & radioButton7.Checked == false)
            {
                MessageBox.Show(" Please Choose Dispute Action");
                ErrorReturn = true;
                return;
            }

            if (radioButton11.Checked == false & radioButton12.Checked == false & radioButton13.Checked == false &
                radioButton14.Checked == false & radioButton15.Checked == false & radioButton16.Checked == false)
            {
                MessageBox.Show(" Please Choose Dispute Reason");
                ErrorReturn = true;
                return;
            }

            if (textBoxActionComments.Text.Length < 3)
            {
                MessageBox.Show(" Please Enter Comments");
                ErrorReturn = true;
                return;
            }

            // DISPUTE ACTION ID
            //
            if (radioButton1.Checked == true)  // CREDIT CUSTOMER 
            {
                textBox14.Text = "";
                textBox4.Text = "";
                Dt.DisputeActionId = 1;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 21;
            }

            if (radioButton2.Checked == true)
            {
                textBox4.Text = "";
                if (decimal.TryParse(textBox14.Text, out Dt.DecidedAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBox14.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                if (Dt.DecidedAmount > Dt.DisputedAmt)
                {
                    MessageBox.Show(textBox14.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                Dt.DisputeActionId = 2;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 21;
                //     Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //     textBoxMsgBoard.Text = "A Transaction to be posted is created. "; 
            }

            if (radioButton3.Checked == true)  // DEDIT CUSTOMER 
            {
                textBox14.Text = "";
                textBox4.Text = "";
                Dt.DisputeActionId = 3;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 11;
                //     Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //    textBoxMsgBoard.Text = "A Transaction to be posted is created. ";
            }

            if (radioButton4.Checked == true)
            {
                textBox14.Text = "";

                if (decimal.TryParse(textBox4.Text, out Dt.DecidedAmount))
                {
                }
                else
                {
                    MessageBox.Show(textBox4.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                if (Dt.DecidedAmount > Dt.DisputedAmt)
                {
                    MessageBox.Show(textBox14.Text, "Please enter valid money amount!");
                    ErrorReturn = true;
                    return;
                }
                Dt.DisputeActionId = 4;
                Dt.DecidedAmount = Dt.DisputedAmt;
                TransType = 11;
                //    Ec.CreateTransTobepostedfromDisputes(WSignedId, Dt.DispTranNo, TransType, Dt.DecidedAmount);
                //    textBoxMsgBoard.Text = "A Transaction to be posted is created. ";

            }

            if (radioButton5.Checked == true)
            {
                if (radioButton5.Checked == true) Dt.DisputeActionId = 5; // Postponed 
                Dt.PostDate = dateTimePicker1.Value;
                if (DateTime.Now >= Dt.PostDate.Date) 
                {
                    MessageBox.Show(textBox4.Text, "Please enter valid date greater than today!");
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
            if (radioButton15.Checked == true) Dt.ReasonForAction = 5;
            if (radioButton16.Checked == true) Dt.ReasonForAction = 6;

            Dt.ActionComment = textBoxActionComments.Text;

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
            Dt.UpdateDisputeTranRecord(WTranNo);
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
            Dt.ReadDisputeTran(WTranNo);

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

            labelAuthHeading.Show();
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

            Dt.ReadDisputeTran(WTranNo);

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
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WReplCycle, AuthorSeqNumber, "Normal"); 
            NForm110.FormClosed +=NForm110_FormClosed;
            NForm110.ShowDialog();

        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 56) WRequestor = true; // Requestor
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
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WReplCycle, Di.DispId, Dt.DispTranNo);
            NForm112.ShowDialog();
        }

        string SavedComments; 
// Attached Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            SavedComments = textBoxActionComments.Text; 
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            string SearchP4 = "";
            string WMode;
            if (WViewFunction == false) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();

            textBoxActionComments.Text = SavedComments; 
        }

     
        
    }
}

// End Author
//*****************************
//if (DisputeAuthorisation == true)
//{
//    Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
//    if (Ap.RecordFound == true)
//    {

//        WAuthorSeqNumber = Ap.SeqNumber;
//        // Exception to the above statement 
//        // This condition occurs only if authorization record has been deleted from requestor
//        // Therefore we make zero not to show authorization details 
//        if (Dt.ClosedDispute == false & Ap.OpenRecord == false)
//        {
//            WAuthorSeqNumber = 0;
//        }

//        labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

//        if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
//        if (Ap.Stage == 2) StageDescr = "Authoriser got the message.";
//        if (Ap.Stage == 3) StageDescr = "Authoriser took action";
//        if (Ap.Stage == 4 & Ap.AuthDecision == "YES") StageDescr = "Authorization accepted. Ready for updating";
//        if (Ap.Stage == 4 & Ap.AuthDecision == "NO") StageDescr = "Authorization REJECTED.";
//        if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

//        labelAuthStatus.Text = "Current Status : " + StageDescr;

//        Us.ReadUsersRecord(Ap.Requestor);
//        labelRequestor.Text = "Requestor : " + Us.UserName;

//        Us.ReadUsersRecord(Ap.Authoriser);
//        labelAuthoriser.Text = "Authoriser : " + Us.UserName;

//        textBoxComment.Text = Ap.AuthComment;
//    }
//    else
//    {
//        WAuthorSeqNumber = 0;
//    }

//    //     if (WAuthorSeqNumber>0) Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);


//    //******************************************
//    // If Comming for Authorization only  or to view 
//    //*****************************************
//    if (WSource == 1 & WAuthorSeqNumber == 0)
//    {
//        ButtonFinish.Hide();

//        if (Dt.ClosedDispute == true)
//        {
//            ButtonFinish.Show();
//            buttonAuthor.Hide();
//        }
//    }

//    if (WSource == 1 & WAuthorSeqNumber > 0 || WSource == 2 & Ap.Stage == 1 & WSignedId == Ap.Requestor || WSource == 11
//           || WSource == 2 & Ap.Stage == 2 & WSignedId == Ap.Requestor || WSource == 11
//           || WSource == 2 & Ap.Stage == 3 & WSignedId == Ap.Authoriser)
//    {
//        // THERE IS AUTHORIZATION RECORD
//        labelAuthHeading.Show();
//        panelAuthor.Show();
//        panel2.Enabled = false;
//        textBoxComment.ReadOnly = true;
//        //        panel7.Enabled = false;
//        buttonRefresh.Show();
//        buttonAuthorise.Hide();
//        buttonReject.Hide();
//        buttonAuthor.Hide();
//        ButtonFinish.Hide();


//        textBoxMsgBoard.Text = "View Information";

//    }

//    //******************************************
//    // Show Authorizer to approve or not 
//    //*****************************************
//    if (WSource == 2 & Ap.Stage == 2 & WSignedId == Ap.Authoriser)
//    {
//        labelAuthHeading.Show();
//        panelAuthor.Show();
//        panel2.Enabled = false;
//        buttonRefresh.Hide();
//        buttonAuthor.Hide();
//        ButtonFinish.Hide();

//        textBoxMsgBoard.Text = "Please take authorization/reject action";

//    }
//    //*********************************************

//    //******************************************
//    // If Comming for Authorization only  
//    //*****************************************
//    if ((WSource == 2 & Ap.Stage == 4 & WSignedId == Ap.Requestor)
//        || (WSource == 1 & Ap.Stage == 4 & WSignedId == Ap.Requestor))
//    {
//        Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
//        if (Ap.AuthDecision == "YES")
//        {
//            //Update 

//            panel2.Enabled = false;

//            labelAuthHeading.Show();
//            buttonRefresh.Hide();
//            buttonAuthorise.Hide();
//            buttonReject.Hide();

//            ButtonFinish.Show();
//            buttonAuthor.Hide();

//            panelAuthor.Show();
//            textBoxComment.ReadOnly = true;

//            textBoxMsgBoard.Text = "Action is Authorised. You can now update";

//        }
//        if (Ap.AuthDecision == "NO")
//        {
//            // Not allowed to Update 
//            panel2.Enabled = true;
//            panel6.Enabled = false;

//            labelAuthHeading.Show();
//            panelAuthor.Show();
//            textBoxComment.ReadOnly = true;
//            buttonAuthorise.Hide();
//            buttonReject.Hide();

//            buttonAuthor.Show();
//            ButtonFinish.Hide();

//            // Close Authorisation record 
//            //
//            Ap.Stage = 5;
//            Ap.OpenRecord = false;

//            Ap.UpdateAuthorisationRecord(Ap.SeqNumber);


//            textBoxMsgBoard.Text = "Action is rejected. You can change action and try again if you want.  ";
////        }

//    }
//    //*********************************************
//}
//else
//{
//    buttonRefresh.Hide();
//    buttonAuthor.Hide();
//    ButtonFinish.Show();

//    labelAuthHeading.Hide();
//    panelAuthor.Hide();
//}
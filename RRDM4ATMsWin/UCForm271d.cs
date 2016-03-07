using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm271d : UserControl
    {
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        Form110 NForm110;
        Form112 NForm112;

        //bool ReconciliationAuthor;
        string StageDescr;
        int WAuthorSeqNumber;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;


        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        int WOutstandingErrors;
        int WOutstandingUnMatched;
        string WMessage; 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //
        int WDifStatus;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        string WCategory;
        int WRMCycle; 

        public void UCForm271dPar(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            WCategory = WAtmNo;
            WRMCycle = WSesNo; 

            InitializeComponent();

            WMessage = "";

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat"); //

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {

                WDifStatus = Ap.DifferenceStatus;
            }
            else
            {
                Us.ReadSignedActivityByKey(WSignRecordNo);
                WDifStatus = Us.ReconcDifferenceStatus;

            }

            // Check if outstanding Unmatched or Outstanding exceptions

            Ec.ReadAllErrorsTableForCounters(WOperator, WCategory, "");

            WOutstandingErrors = Ec.NumOfErrors - (Ec.ErrUnderAction + Ec.ErrUnderManualAction);

            if (WOutstandingErrors > 0)
            {
                WDifStatus = 8;
                WMessage = "There are Outstanding Meta Exceptions. "; 
            }

            string SearchingStringLeft = "Operator ='" + WOperator
                                         + "' AND RMCateg ='" + WCategory + "' AND OpenRecord = 1 AND MetaExceptionNo = 0 ";

            string WhatFile = "UnMatched";
            string WSortValue = "SeqNo";
            Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, SearchingStringLeft, WhatFile, WSortValue);

            WOutstandingUnMatched = Rm.TotalSelected;

            if (WOutstandingUnMatched > 0)
            {
                WDifStatus = 9;
                if (WOutstandingErrors > 0)
                {
                    WMessage = "There are Outstanding UnMatched Records. " + WMessage;
                }
                else
                {
                    WMessage = "There are Outstanding UnMatched Records. " + WMessage;
                }
                
            }
            
            if (WDifStatus == 1 ) // Everything is in Order 
            {
                textBox1.Show();
                textBox4.Hide();
                //guidanceMsg = " Well Done !!! No differences. ";

                ////STAVROS
                //ChangeBoardMessage(this, new EventArgs());
            }

            if (WDifStatus > 1) // ERRORS STILL 
            {
                //   textBoxMsg.Text = " THERE IS WORK TO BE DONE! SOME OUTSTANDING ARE REMAINING ";
                textBox1.Hide();
                textBox4.Show();

                if (WOutstandingUnMatched > 0 || WOutstandingErrors > 0)
                {
                    textBox4.Text = "We Recommend To Pay Attention. " + WMessage;

                    textBox4.ForeColor = Color.Red;
                }
            }

            // Update Step
            Us.ReadSignedActivityByKey(WSignRecordNo);
            WDifStatus = Us.ReconcDifferenceStatus;
            Us.StepLevel = 3;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

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
                Ap.GetMessageOne(WAtmNo, WSesNo, "Reconciliation", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
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
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageOne(WAtmNo, WSesNo, "ReconciliationCat", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
                ////STAVROS
                ChangeBoardMessage(this, new EventArgs());
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Reconc closing stage for" + " Category: " + WAtmNo + " Matching SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    //label1.Hide();

                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                }
                else
                {
                    buttonNotes2.Show();
                    labelNumberNotes2.Show();

                }
            }
            else
            {
                buttonNotes2.Show();
                labelNumberNotes2.Show();
            }

          

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo,"ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        Us.ReadSignedActivityByKey(WSignRecordNo);
                        Us.ProcessNo = 2; // Return to stage 2  
                        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                    }

                   
                }

            }

            // Show Authorisation record 
            ShowAuthorisationInfo();

         

        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");
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
                labelAuthHeading.Show();
                labelAuthHeading.Text = "AUTHORISER's SECTION FOR ATM : " + WAtmNo;
                panelAuthor.Show();

                if (WViewFunction == true) // For View only 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    //buttonAuthorisations.Hide();
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
                    //buttonAuthorisations.Hide();
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
                    //buttonAuthorisations.Hide();
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
                        //buttonAuthorisations.Show();
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
                        //buttonAuthorisations.Hide();
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
                        //buttonAuthorisations.Hide();
                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                }
            }
            else
            {
                // THIS IS THE NORMAL ... You do not show the AUTH box 
                if (NormalProcess & WRequestor == false) // Normal Reconciliation 
                {
                    // Do not show Authorisation Section this will be shown after authorisation 
                    labelAuthHeading.Hide();
                    panelAuthor.Hide();
                    buttonRefresh.Hide();
                }
            }

        }
        // Authorise - choose authoriser 
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            // Check if Already in authorization process

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Replenishment Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
            //    InputValidationAndUpdate("Authorisation");

            //      if (ErrorReturn == true) return;
            int WTranNo = 0;

            string WOrigin = "ReconciliationCat";


            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber, "Normal");
            NForm110.FormClosing += NForm110_FormClosing;
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

           guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
            ChangeBoardMessage(this, e);
        }

        void NForm110_FormClosing(object sender, FormClosingEventArgs e)
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

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationCat");

            if (WRequestor == true & Ap.Stage == 1)
            {
                guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
                ChangeBoardMessage(this, e);
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                guidanceMsg = "Authorisation made. Workflow can finish! ";
                ChangeBoardMessage(this, e);
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                guidanceMsg = "Please make authorisation ";
                ChangeBoardMessage(this, e);
            }

            SetScreen();

        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
        // REFRESH 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ShowAuthorisationInfo();
        }

        // AUthorisation section - Authorise 
        private void buttonAuthorise_Click(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES");

            guidanceMsg = "Authorisation Made - Accepted ";
            ChangeBoardMessage(this, e);

            ShowAuthorisationInfo();
        }

        // Reject 
        private void buttonReject_Click(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

            guidanceMsg = "Authorisation Made - Rejected ";
            ChangeBoardMessage(this, e);

            ShowAuthorisationInfo();
        }
        // Update Authorization Record 
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
                    //this.Dispose();
                }
                if (InDecision == "NO")
                {
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    //   this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return;
            }
        }
        // HISTORY FOR AUTHORISATIONS 

        // Button History 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WSesNo, WDisputeNo, WDisputeTranNo);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Reconc closing stage for" + " Category: " + WAtmNo + " Matching SesNo: " + WSesNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen(); 
        }

    }
}

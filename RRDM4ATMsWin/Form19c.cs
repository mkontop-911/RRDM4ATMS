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
    public partial class Form19c : Form
    {
        Form110 NForm110;
        Form112 NForm112;

        bool ReconciliationAuthor;
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

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //
     
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public Form19c(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            //WForceMatchingCommnet = InForceMatchingCommnet; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelUserId.Text = InSignedId;
            
            // Read Commnets 
            Us.ReadSignedActivityByKey(WSignRecordNo);

            textBoxForceMatchingInfo.Text = Us.GeneralUsedComment; 

            textBoxForceMatchingInfo.Show();

            textBoxMsgBoard.Text = " Proceed with Authorization Process. ";    
          
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
                Ap.GetMessageOne(WAtmNo, WSesNo, "Replenishment", WAuthoriser, WRequestor, Reject);
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
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Force Matching for" + " Category: " + WAtmNo + " Matching SesNo: " + WSesNo;
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

            // CHECK IF Reconciliation ELECTRONIC AUTHORISATION IS NEEDED
            string ParId = "262";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");


            if (Gp.OccuranceNm == "YES") // Electronic needed 
            {
                ReconciliationAuthor = true;
                buttonAuthor.Show();
            }
            else
            {
                ReconciliationAuthor = false;
                buttonAuthor.Hide();
            }

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");

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

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                    }

                    // Show Authorisation record 
                    ShowAuthorisationInfo();
                }

            }

            if (NormalProcess & ReconciliationAuthor == true) // Normal Reconciliation with authorisation 
            {
                // Main buttons
                buttonAuthor.Show();
                buttonRefresh.Hide();
                //buttonAuthorisations.Hide();
            }

            if (NormalProcess & ReconciliationAuthor == false) // Normal Reconciliation without authorisation 
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

            if ((WViewFunction == true & ReconciliationAuthor == true) || (WViewFunction == true & ReconciliationAuthor == true)
                 || WAuthoriser == true) // Comes from Author
            {
                ShowAuthorisationInfo();
            }
            else
            {
                if (WViewFunction == true & ReconciliationAuthor == false)
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

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");
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
        // Authorise - choose authoriser 
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            // Check if Already in authorization process

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Reconciliation Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
            //    InputValidationAndUpdate("Authorisation");

            //      if (ErrorReturn == true) return;
            int WTranNo = 0;

            string WOrigin = "ForceMatchingCat";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber, "Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            //guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
            //ChangeBoardMessage(this, e);
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

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";
                //guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
                //ChangeBoardMessage(this, e);
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";
                //guidanceMsg = "Authorisation made. Workflow can finish! ";
                //ChangeBoardMessage(this, e);
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";
                //guidanceMsg = "Please make authorisation ";
                //ChangeBoardMessage(this, e);
            }

            SetScreen();

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

            textBoxMsgBoard.Text = "Authorisation Made - Accepted ";
            //guidanceMsg = "Authorisation Made - Accepted ";
            //ChangeBoardMessage(this, e);

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

            textBoxMsgBoard.Text = "Authorisation Made - Rejected ";
            //guidanceMsg = "Authorisation Made - Rejected ";
            //ChangeBoardMessage(this, e);

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
            WParameter4 = "Force Matching for" + " Category: " + WAtmNo + " Matching SesNo: " + WSesNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
// FINISH 

        bool ReplenishmentAuthorNoRecordYet; 
        bool ReplenishmentAuthorDone;
        bool ReplenishmentAuthorOutstanding; 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");
            if (Ap.RecordFound == true)
            {
                ReplenishmentAuthorNoRecordYet = false;
                if (Ap.Stage == 3 || Ap.Stage == 4)
                {
                    ReplenishmentAuthorDone = true;
                }
                else
                {
                    ReplenishmentAuthorOutstanding = true;
                }
            }
            else
            {
                ReplenishmentAuthorNoRecordYet = true;
            }


            if (WAuthoriser == true & ReplenishmentAuthorDone == true) // Coming from authoriser and authoriser done  
            {
                this.Close();
                return;
            }

            if (WRequestor == true & ReplenishmentAuthorDone == false) // Coming from authoriser and authoriser not done 
            {
                this.Close();
                return;
            }

            if (WRequestor == false & WAuthoriser == false & ReplenishmentAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
            {
                MessageBox.Show("MSG946 - Authorisation outstanding");
                return;
            }

            if (Us.ProcessNo == 1 & ReplenishmentAuthorOutstanding == true) // Cancel with repl outstanding 
            {
                MessageBox.Show("MSG946 - Authorisation outstanding");
                return;
            }

            if (WAuthoriser == true & ReplenishmentAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
            {
                MessageBox.Show("MSG946 - Authorisation outstanding");
                return;
            }

            if (ReplenishmentAuthorDone == true)
            {
                // UPDATE RMCategory Cycle with Authoriser
                // DO GL transactions

                this.Close(); 

            }

            
        }
    }
}

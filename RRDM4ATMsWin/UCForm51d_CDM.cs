using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51d_CDM : UserControl
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

        //   string connectionString = ConfigurationManager.ConnectionStrings
        //     ["ATMSConnectionString"].ConnectionString;


        //RRDMNotesBalances Na = new RRDMNotesBalances(); // Activate Class 

        //RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Activate Class 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        //     ErrorsClass Pa = new ErrorsClass(); // Make class availble 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //
        int WDifStatus;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51d_CDM_Par(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment"); //
  
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                WDifStatus = Usi.ReconcDifferenceStatus; 

            if (WDifStatus == 1) // Everything is in Order 
            {
                textBox1.Show();
                textBox4.Hide();
                pictureBox1.Hide();

                guidanceMsg = " Well Done !!! No differences. ";

                //STAVROS
                ChangeBoardMessage(this, new EventArgs());
            }

            if (WDifStatus > 1) // ERRORS STILL 
            {
                //   textBoxMsg.Text = " THERE IS WORK TO BE DONE! SOME OUTSTANDING ARE REMAINING ";
                textBox1.Hide();
                textBox4.Show();
                pictureBox1.Show();
                pictureBox1.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }

            // Update Step of-running user 
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            WDifStatus = Usi.ReconcDifferenceStatus;
            Usi.StepLevel = 6;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

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
                Ap.GetMessageOne(WAtmNo, WSesNo, "Replenishment", WAuthoriser, WRequestor, Reject);
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

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
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
            Usi.ReadSignedActivityByKey(WSignRecordNo);

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
         

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        //  guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 1; // Return to stage 1 : Replenishment   
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        // Update STEP

                        Usi.ReadSignedActivityByKey(WSignRecordNo);

                        Usi.ReplStep1_Updated = true;
                        Usi.ReplStep2_Updated = true;
                        Usi.ReplStep3_Updated = true;
                        Usi.ReplStep4_Updated = true;
                        Usi.ReplStep5_Updated = true;

                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");
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
                if ((Ap.Stage == 3 || Ap.Stage == 4) & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";
                if (Ap.Stage == 5 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

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

            string WOrigin = "Replenishment";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber,0,"",0 ,"Normal");
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

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) 
            {
                WRequestor = true; // Requestor
                ViewWorkFlow = true;
            }
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

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
        // REFRESH 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

            if (Ap.Stage < 3)
            {
                MessageBox.Show("Authoriser didn't take action yet.");
                return;
            }

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
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WSesNo, WDisputeNo, WDisputeTranNo, "",0);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }

    }
}

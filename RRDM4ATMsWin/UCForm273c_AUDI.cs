using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm273c_AUDI : UserControl
    {
      //  public event EventHandler ChangeBoardMessage;
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

      //  string WBankId;
        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDM_Cit_ExcelOutputCycles Cec = new RRDM_Cit_ExcelOutputCycles();


        RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

      //  string WSelectionCriteria;

        int WDifStatus;

        int WOrdersCycle;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        string WCitId;
        string WOrigin;
        string WFunction; 

        //(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InOutputCycleNo, string InFunction)
        public void UCForm273c_AUDI_Par(string InSignedId, int InSignRecordNo, string InOperator, string InCitId, int InOutputCycleNo, string InFunction)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WOrdersCycle = InOutputCycleNo;

            WCitId = WCategoryId = InCitId;

            WFunction = InFunction; 

            WOrigin = "ReplOrders";

            //   WCitId = InCitId; 

            InitializeComponent();

            // Set Working Date 

            //labelToday.Text = DateTime.Now.ToShortDateString();

            //pictureBox1.BackgroundImage = appResImg.logo2;

            //labelUserId.Text = InSignedId;

            //labelStep1.Text = "Authorisation for Orders Cycle.." + WOrdersCycle + " for CIT:" + WCitId;

            //Us.ReadUsersRecord(WSignedId);
            //WBankId = Us.BankId;

            // Read Stats 

            Ro.ReadReplActionsForCounters(WCitId, WOrdersCycle);

            // 
            // Make the textBoxMessage
            //

            if (Ro.NumberOfActiveOrders > 0)
            {
                textBoxSummary.Text = "Excel will be created and send to CIT " + Environment.NewLine
                               + "------------------------"  + Environment.NewLine
                               + "ATMs to be replenished :" + Ro.NumberOfActiveOrders + Environment.NewLine
                                 + "------------------------" + Environment.NewLine
                               + "FIND THE AMOUNTS BELOW" + Environment.NewLine
                                + "------------------------" + Environment.NewLine
                               + "Total Insured Amt:.." + Ro.TotalInsuredAmount.ToString("#,##0.00") + Environment.NewLine
                               + "Total Suggested by RRDM Amt:.." + Ro.TotalSystemAmount.ToString("#,##0.00") + Environment.NewLine
                                + "------------------------" + Environment.NewLine
                               + "Total Replenishment Amt:.." + Ro.TotalNewAmount.ToString("#,##0.00") + Environment.NewLine
                                 + "------------------------" + Environment.NewLine
                               + "FINAL NOTE" + Environment.NewLine
                               + "Cancelled Orders..:" + Ro.CanceledOrders + Environment.NewLine
                               + "";
            }
            else
            {
                textBoxSummary.Text = "Excel will not be created " + Environment.NewLine
                               + "No Active Orders Created to include in Excel" 
                               + "";
            }
           

            // Update Step

            //Usi.ReadSignedActivityByKey(WSignRecordNo);
            //WDifStatus = Usi.ReconcDifferenceStatus;
            //Usi.StepLevel = 2;
            //Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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
                Ap.GetMessageReconCateg(WCategoryId, WOrdersCycle, "ReplOrders", WAuthoriser, WRequestor, Reject);
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
                Ap.GetMessageReconCateg(WCategoryId, WOrdersCycle, "ReplOrders", WAuthoriser, WRequestor, Reject);
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Closing stage for" + " Category: " + WCategoryId + " Order Cycle: " + WOrdersCycle;
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
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
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

            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WOrdersCycle, "ReplOrders"); //

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        // guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {

                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                    }

                }

            }

            // Show Authorisation record 
            ShowAuthorisationInfo();

          //  ChangeBoardMessage(this, new EventArgs());
        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WOrdersCycle, "ReplOrders"); //
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

                if (Ap.Stage == 5 & Ap.AuthDecision == "YES") StageDescr = "Authorisation process is completed";
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
                labelAuthHeading.Text = "AUTHORISER's SECTION FOR CATEGORY : " + WCategoryId;
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

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategoryId;

            Usi.WFieldNumeric1 = WOrdersCycle;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WOrdersCycle);
            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReplOrders");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationRecordByRMCategoryAndRMCycle(WCategoryId, WOrdersCycle);
            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReplOrders");

            if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            {
                MessageBox.Show("This Cycle Already has authorization record!" + Environment.NewLine
                                         + "Go to Pending Authorisations process."
                                                          );
                return;
            }

            // Validate input 
            //    InputValidationAndUpdate("Authorisation");

            //      if (ErrorReturn == true) return;
            int WTranNo = 0;

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management

            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, "",0, AuthorSeqNumber, WDifStatus,WCategoryId, WOrdersCycle, "Normal");
            //NForm110.FormClosing += NForm110_FormClosing;
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            //Form110(string InSignedId, int SignRecordNo, string InOperator, string InOrigin,
            //          int InTraNo, string InAtmNo, int InReplCycle,
            //          int InAuthorSeqNumber, int InDifStatus, string InRMCategory, int InRMCycle, string InFunction)

            //guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
            //ChangeBoardMessage(this, e);
            guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
           // ChangeBoardMessage(this, e);
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
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WOrdersCycle, "ReplOrders"); //


            if (WRequestor == true & Ap.Stage == 1)
            {
                guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
                //ChangeBoardMessage(this, e);
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                guidanceMsg = "Authorisation made. Workflow can finish! ";
                
            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                guidanceMsg = "Please make authorisation ";
                //ChangeBoardMessage(this, e);
            }

            SetScreen();


        }
        // REFRESH 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle
             (WCategoryId, WOrdersCycle, "ReplOrders");

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

            ShowAuthorisationInfo();

            // Update all orders as Authorised 
            Ro.UpdateReplActionsForAuthorised(WSignedId, WCitId, WOrdersCycle);
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
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", 0, WDisputeNo, WDisputeTranNo, WCategoryId, WOrdersCycle);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Closing stage for" + " Category: " + WCategoryId + " Order Cycle: " + WOrdersCycle;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
// Print ORDERS
        private void buttonPrintActions_Click(object sender, EventArgs e)
        {
            PrintOrders();
        }
// ORDERS
        private void PrintOrders()
        {
            string P1 = "REPLENISHMENT ORDERS CYCLE:" + WOrdersCycle.ToString() + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = WOrdersCycle.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }
    }
}

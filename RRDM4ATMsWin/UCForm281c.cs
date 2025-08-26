using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm281c : UserControl
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

        //int WOutstandingErrors;
        int WOutstandingUnMatched;
        string WMessage;

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

        RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //
        int WDifStatus;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WReconcCategoryId;
        int WReconcCycleNo;

        public void UCForm281cPar(string InSignedId, int SignRecordNo, string InOperator, string InReconcCategoryId, int InReconcCycleNo)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WReconcCategoryId = InReconcCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            InitializeComponent();

            WMessage = "";
            //Load Data Table For Grid
            int Mode = 2 ; 
            Mre.ReadMatchingReconcExceptionsInfoToFillTable(WOperator, WReconcCategoryId, WReconcCycleNo, Mode);
            //Show Table
            ShowGrid();

            //Ap.ReadAuthorizationForReplenishmentReconcSpecific(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat"); //

            //if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
            //{

            //    WDifStatus = Ap.DifferenceStatus;
            //}
            //else
            //{
            //    Us.ReadSignedActivityByKey(WSignRecordNo);
            //    WDifStatus = Us.ReconcDifferenceStatus;
            //}

            // Check if outstanding Unmatched or Outstanding exceptions

            Mp.ReadMatchingTxnsMasterPoolForTotalsForUnMatched(WOperator, WReconcCategoryId, WReconcCycleNo);

            WOutstandingUnMatched = Mp.TotalOutstandingForAction;

            if (WOutstandingUnMatched > 0)
            {

                WDifStatus = 9;

                WMessage = "There are Outstanding UnMatched Records. " + WMessage;

            }
            else
            {
                WDifStatus = 1;
            }

            if (WDifStatus == 1) // Everything is in Order 
            {
                textBox1.Show();
                textBox4.Hide();
            }

            if (WDifStatus > 1) // ERRORS STILL 
            {
                //   textBoxMsg.Text = " THERE IS WORK TO BE DONE! SOME OUTSTANDING ARE REMAINING ";
                textBox1.Hide();
                textBox4.Show();


                textBox4.Text = "We Recommend To Pay Attention. " + WMessage;

                textBox4.ForeColor = Color.Red;

            }

            // Update Step
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            WDifStatus = Usi.ReconcDifferenceStatus;
            Usi.StepLevel = 2;
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
                Ap.GetMessageReconCateg(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat", WAuthoriser, WRequestor, Reject);
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
                Ap.GetMessageReconCateg(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
                ////STAVROS
                ChangeBoardMessage(this, new EventArgs());
            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Reconc closing stage for" + " Category: " + WReconcCategoryId + " Matching SesNo: " + WReconcCycleNo;
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



            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        guidanceMsg = " Finish Authorisation .";
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



        }

        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");
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

                if (WOperator == "ITMX")
                {
                    RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                    Rcs.ReadReconcCategoriesByCategoryIdForName(WReconcCategoryId);
                    labelAuthHeading.Text = "AUTHORISER's SECTION FOR Category : " + Rcs.CategoryName;
                }
                else
                {
                    labelAuthHeading.Text = "AUTHORISER's SECTION FOR ATM : " + WReconcCategoryId;
                }


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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");

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
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WReconcCategoryId, WReconcCycleNo, AuthorSeqNumber
                , WDifStatus, WReconcCategoryId, WReconcCycleNo, "Normal");
            //NForm110.FormClosing += NForm110_FormClosing;
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            guidanceMsg = "A message was sent to authoriser. Refresh for progress monitoring.";
            ChangeBoardMessage(this, e);
        }

        //void NForm110_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //************************************************************
        //    //************************************************************
        //    // AUTHOR PART
        //    //************************************************************
        //    WViewFunction = false;
        //    WAuthoriser = false;
        //    WRequestor = false;
        //    NormalProcess = false;

        //    Us.ReadSignedActivityByKey(WSignRecordNo);

        //    if (Us.ProcessNo == 56) WRequestor = true; // Requestor
        //    else NormalProcess = true;

        //    Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");

        //    if (WRequestor == true & Ap.Stage == 1)
        //    {
        //        guidanceMsg = "Message was sent to authoriser. Refresh for progress ";
        //        ChangeBoardMessage(this, e);
        //    }

        //    if (WRequestor == true & Ap.Stage == 4)
        //    {
        //        guidanceMsg = "Authorisation made. Workflow can finish! ";
        //        ChangeBoardMessage(this, e);
        //    }

        //    if (NormalProcess) // Orginator has deleted authoriser 
        //    {
        //        guidanceMsg = "Please make authorisation ";
        //        ChangeBoardMessage(this, e);
        //    }

        //    SetScreen();

        //}

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

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WReconcCategoryId, WReconcCycleNo, "ReconciliationCat");

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
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WReconcCategoryId, WReconcCycleNo, WDisputeNo, WDisputeTranNo, WReconcCategoryId, WReconcCycleNo);
            NForm112.ShowDialog();
        }

        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Reconc closing stage for" + " Category: " + WReconcCategoryId + " Matching SesNo: " + WReconcCycleNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        // Show Grid Left 
        public void ShowGrid()
        {

            dataGridView1.DataSource = Mre.DataTableMatchingExceptions.DefaultView;

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Origin
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  TxnType
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 40; // Ccy
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // Amount
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 140; // Particulars
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 140; // UnMatchedName
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[7].Width = 120; // System Recommendation
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 210; // Action Taken
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[9].Width = 120; // ExecutionDtTm
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            // DATA TABLE ROWS DEFINITION 
            //DataTableMatchingExceptions.Columns.Add("RecordId", typeof(int));
            //DataTableMatchingExceptions.Columns.Add("Origin", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("TxnType", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("Ccy", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("Amount", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("Particulars", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("UnMatchedName", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("System Recommendation", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("Action Taken", typeof(string));
            //DataTableMatchingExceptions.Columns.Add("ExecutionDtTm", typeof(string));

        }

        private void labelNumberNotes2_Click(object sender, EventArgs e)
        {

        }
    }
}

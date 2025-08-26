using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVConfirmed : Form
    {
       
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

        string WModeNotes;

        int WUniqueMatchingNo; 

        //int WMode;

        string WOrigin;

        string W4DigitMainCateg;

        //int WMode;
        //DateTime FromDt;
        //DateTime ToDt;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //int WOutstandingErrors;
        //int WOutstandingUnMatched;

        string LDONE; 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        //RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcJobCycles Dj = new RRDMReconcJobCycles();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMErrorsClassWithActionsITMX Ec = new RRDMErrorsClassWithActionsITMX();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        int WRcsSeqNo; 
        string Ccy ;
        string InternalAcc ;
        string ExternalAcc ;
        string WStmtTrxReferenceNumber; 

        //
        int WDifStatus;
        //string W4DigitMainCateg; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;
        string WCategoryId; 
        int WReconcCycleNo;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime WDate = new DateTime(2015, 02, 15);
        //(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo);
        public Form291NVConfirmed(string InSignedId, int SignRecordNo, string InOperator, 
                                  string InSubSystem, string InReconcCategoryId, int InReconcCycleNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem;
            WCategoryId = InReconcCategoryId;
            WReconcCycleNo = InReconcCycleNo;

            W4DigitMainCateg = WCategoryId.Substring(0, 4);

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            Rcs.ReadNVReconcCategorySessionByCatAndRunningJobNo(WOperator,
                                                 WCategoryId, WReconcCycleNo);
            WRcsSeqNo = Rcs.SeqNo; 
            Ccy = Rcs.NostroCcy;
            InternalAcc = Rcs.InternalAccNo;
            ExternalAcc = Rcs.ExternalAccNo;
            WStmtTrxReferenceNumber = Rcs.StmtTrxReferenceNumber; 
      
            label6.Text= "PAIR OF ACCOUNTS: " + Mc.CategoryName;
            textBox11.Text = Ccy;

        }

        // LOAD SCREEN 
        private void Form291_Load(object sender, EventArgs e)
        {
           
            int Mode = 2;
            //ExternalAccno = "ALPHA67890";
            //InternalAccNo = "HELLENIC_67890";
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, ExternalAcc, InternalAcc, "", NullPastDate, "");

            //Show Table
            ShowGrid();

            // Check if outstanding Unmatched or Outstanding exceptions
         
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategoryId;
            Usi.WFieldNumeric1 = WReconcCycleNo;
            Usi.StepLevel = 0;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

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
                Ap.GetMessageReconCateg(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching", WAuthoriser, WRequestor, Reject);
                //guidanceMsg = Ap.MessageOut;
            }

            //************************************************************
            //************************************************************

            SetScreen();

        }

        int WSeqNo;
        string SelectionCriteria;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
          
            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            WUniqueMatchingNo = Se.UniqueMatchingNo; 

            textBox1.Text = Se.UniqueMatchingNo.ToString();
            textBox2.Text = Se.StmtLineValueDate.ToString();
            textBox3.Text = Se.StmtLineAmt.ToString("#,##0.00");

            if (Se.ActionType == "00")
            {
                buttonUnDo.Show();
                buttonReDo.Hide();
            }
            if (Se.ActionType == "08")
            {
                buttonUnDo.Hide();
                buttonReDo.Show();
            }

            int WColorNo = (int)rowSelected.Cells[1].Value;

            if (WColorNo == 11)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DimGray;
            }
            else if (WColorNo == 12)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
            }
        }

        //UNDO Default 
        int WRowIndex;

        private void buttonUnDo_Click(object sender, EventArgs e)
        {
            //
            //
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Se.ActionType = "08"; // Action Type 8 = it was default and it will become 
            Se.UpdateUndoConfirmedExternal(WOperator, WUniqueMatchingNo);
            Se.UpdateUndoConfirmedInternal(WOperator, WUniqueMatchingNo);

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            // Load Grid
            Form291_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        //Redo Default 
        private void buttonReDo_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Se.ActionType = "00"; // 
            Se.UpdateUndoConfirmedExternal(WOperator, WUniqueMatchingNo);
            Se.UpdateUndoConfirmedInternal(WOperator, WUniqueMatchingNo);

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            // Load Grid
            Form291_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
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
                Ap.GetMessageReconCateg(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching", WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;

            }

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // 
           
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

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        textBoxMsgBoard.Text = "Finish Authorisation .";
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

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching");
            //if ((Ap.RecordFound == true & Ap.OpenRecord == true)
            //       || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            //       || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
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

                //if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

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
                    Rcs.ReadReconcCategoriesByCategoryIdForName("AllCategories");
                    labelAuthHeading.Text = "AUTHORISER's SECTION FOR Category : " + Rcs.CategoryName;
                }
                else
                {
                    labelAuthHeading.Text = "AUTHORISER's SECTION FOR ATM : " + "AllCategories";
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


        private void buttonAuthor_Click_1(object sender, EventArgs e)
        {
            // Check if Already in authorization process

            //Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("AllCategories",
            //                                                      WReconcCycleNo, "ConfirmNostroMatching");
            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId,
                                                                WReconcCycleNo, "ConfirmNostroMatching");

            if (Ap.RecordFound == true & Ap.OpenRecord == true)
            {
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                }
            }

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching");

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

            string WOrigin = "ConfirmNostroMatching";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin,
                                     WTranNo, "", 0,
                                     AuthorSeqNumber,0, WCategoryId, WReconcCycleNo, "Normal");
            //NForm110.FormClosing += NForm110_FormClosing;
            //Form110(string InSignedId, int SignRecordNo, string InOperator, string InOrigin,
            //          int InTraNo, string InAtmNo, int InReplCycle,
            //          int InAuthorSeqNumber, int InDifStatus, string InRMCategory, int InRMCycle, string InFunction)
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();

            textBoxMsgBoard.Text = "A message was sent to authoriser. Refresh for progress monitoring.";
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

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching");

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
        // REFRESH 

        private void buttonRefresh_Click_1(object sender, EventArgs e)
        {
            ShowAuthorisationInfo();
        }

        // AUthorisation section - Authorise 

        private void buttonAuthorise_Click_1(object sender, EventArgs e)
        {
            UpdateAuthorRecord("YES");

            textBoxMsgBoard.Text = "Authorisation Made - Accepted ";


            ShowAuthorisationInfo();
        }


        // Reject 

        private void buttonReject_Click_1(object sender, EventArgs e)
        {
            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

            textBoxMsgBoard.Text = "Authorisation Made - Rejected ";

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

        private void buttonAuthHistory_Click_1(object sender, EventArgs e)
        {
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WCategoryId, WReconcCycleNo, WDisputeNo, WDisputeTranNo, WCategoryId, WReconcCycleNo);
            NForm112.ShowDialog();
        }
        // Show Grid Left 
        public void ShowGrid()
        {

            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Records For Confirmation");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = true;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = true;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                int WColorNo = (int)row.Cells[1].Value;

                if (WColorNo == 11)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WColorNo == 12)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

       
        // NOTES 
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Default Approval stage for Job Cycle No: " + WReconcCycleNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WModeNotes = "Read";
            else WModeNotes = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeNotes, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        //Finish 
        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;
        bool ReconciliationAuthorOutstanding;

        //int TranSelected;
        //string WDone;
        //int WMaskRecordId;

        int LSeqNo ;
        string LOrigin ;

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            //**************************************************
            //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
            //**************************************************

            if (WViewFunction == true) // Coming from View only 
            {
                this.Close();
                return;
            }

            // FINISH - Make validationsfor Authorisations  

            //  ReconciliationAuthorNeeded = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategoryId, WReconcCycleNo, "ConfirmNostroMatching");
            
            if (Ap.RecordFound == true)
            {
                ReconciliationAuthorNoRecordYet = false;
                if (Ap.Stage == 3 || Ap.Stage == 4 || Ap.Stage == 5)
                {
                    ReconciliationAuthorDone = true;
                }
                else
                {
                    ReconciliationAuthorOutstanding = true;
                }
            }
            else
            {
                ReconciliationAuthorNoRecordYet = true;
            }


            if (WAuthoriser == true & ReconciliationAuthorDone == true) // Coming from authoriser and authoriser done  
            {
                this.Close();
                return;
            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorDone == true) //  
            {
                this.Close();
                return;
            }

            if (WRequestor == true & ReconciliationAuthorDone == false) // Coming from authoriser and authoriser not done 
            {
                this.Close();
                return;
            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
            {
                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                             == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (Usi.ProcessNo == 2 & ReconciliationAuthorOutstanding == true) // Cancel with repl outstanding 
            {

                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                              == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (WAuthoriser == true & ReconciliationAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
            {
                MessageBox.Show("MSG946 - Authorisation outstanding");
                return;
            }

            if ((Usi.ProcessNo == 2 || WRequestor == true) & ReconciliationAuthorDone == true) // Everything is fined .
            {

            }

            //
            //**************************************************************************
            //**************************************************************************
            // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
            //**************************************************************************
            //***********************************************************************
            //**************** USE TRANSACTION SCOPE
            //***********************************************************************

            // create a connection object
            //using (var scope = new System.Transactions.TransactionScope())
            try
            {

                // Update authorisation record  

                if (Ap.RecordFound == true & Ap.Stage == 4)
                {
                    // Update stage 
                    //
                    Ap.Stage = 5;
                    Ap.OpenRecord = false;

                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);

                    //**************************************************************************
                    // Read Grid and update as needed with default adjustment and settled
                    //
                    //**************************************************************************

                    //TranSelected = 0;

                    int K = 0;
                    //
                    //UPDATE FOOTER 
                    //
                    while (K <= (dataGridView1.Rows.Count - 1))
                    {
                        LSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;
                        LOrigin = (string)dataGridView1.Rows[K].Cells["Origin"].Value;
                        LDONE = (string)dataGridView1.Rows[K].Cells["DONE"].Value;

                        int MatchingNo = (int)dataGridView1.Rows[K].Cells["MatchingNo"].Value;
                        string TransType = (string)dataGridView1.Rows[K].Cells["DR/CR"].Value;
                        decimal  Amt = (decimal)dataGridView1.Rows[K].Cells["Amt"].Value;

                        if (LOrigin == "EXTERNAL")
                        {
                            SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + LSeqNo;

                            Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                            if (LDONE == "YES")  
                            {
                                Se.ActionType = "00";
                                Se.SettledRecord = true;
                                Se.UpdateExternalFooter(WOperator, SelectionCriteria);
                                // Create Transaction to be posted
                            }

                            if (LDONE == "NO")
                            {
                                
                                Se.Matched = false;
                                Se.ToBeConfirmed = false;
                                Se.ActionType = "00";
                                Se.MatchedType = "Manual";
                                Se.UpdateExternalFooter(WOperator, SelectionCriteria);

                            }                

                        }

                        if (LOrigin == "INTERNAL" || LOrigin == "WAdjustment")
                        {
                            SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + LSeqNo;

                            Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                            if (LDONE == "YES")
                            {
                                Se.ActionType = "00";
                                Se.SettledRecord = true;
                                Se.UpdateInternalFooter(WOperator, SelectionCriteria);

                                if (LOrigin == "WAdjustment")
                                {
                                    
                                    Ec.CreateTransTobepostedfromNOSTRO(WSignedId, Ap.Authoriser ,WCategoryId ,InternalAcc,
                                                LSeqNo ); 
                                }
                                // Create Transaction to be posted 
                            }

                            if (LDONE == "NO")
                            {
                                if (LOrigin == "WAdjustment")
                                {
                                    Se.DeleteAdjustmentsForInternal(LSeqNo);
                                }
                                else
                                {
                                    Se.Matched = false;
                                    Se.ToBeConfirmed = false;
                                    Se.ActionType = "00";
                                    Se.MatchedType = "Manual";
                                    Se.UpdateInternalFooter(WOperator, SelectionCriteria);
                                }                        
                             
                            }
                           
                        }

                        K++; // Read Next entry of the table 
                    }
                    Ccy = Rcs.NostroCcy;
                    InternalAcc = Rcs.InternalAccNo;
                    ExternalAcc = Rcs.ExternalAccNo;
                    WStmtTrxReferenceNumber = Rcs.StmtTrxReferenceNumber;
                    // Find Totals for EXTERNAL except of Alerts with date < Working date

                    //if (W4DigitMainCateg == "EWB5") WMode = 20;
                    //if (W4DigitMainCateg == "EWB8") WMode = 21;

                    Se.ReadNVExternalStatements_LinesForTotals(WOperator, WSignedId, WSubSystem, WReconcCycleNo,
                                            ExternalAcc, WStmtTrxReferenceNumber, WDate);

                    // Find ALerts and Disputes 
                    int Mode = 12; // All Un matched this cycle 
                    Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, Mode, WReconcCycleNo, "", "", "", WDate, "");

                    Rcs.UpdateReconcCategorySessionWithAutomaticMatchTotals(WRcsSeqNo,
                          Se.TotalNumberProcessed, Se.MatchedDefault, Se.AutoButToBeConfirmed,
                          Se.UnMatchedAmt, Se.MatchedFromAutoToBeConfirmed, Se.MatchedFromManualToBeConfirmed, Se.OutstandingAlerts, Se.OutstandingDisputes, DateTime.Now);

                    Form2 MessageForm = new Form2("Default Adjustments were finalised for Cycle No: " + WReconcCycleNo + Environment.NewLine
                                                   + "Actions are taken and Unmatched records are marked as settled." + Environment.NewLine
                                                   );
                    MessageForm.ShowDialog();
                  
                    this.Close();

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
            //finally
            //{
            //    scope.Dispose();
            //}

            this.Dispose();
        }
// Account Status 
        private void button1_Click(object sender, EventArgs e)
        {
            Form291NVAccountStatus NForm291NVAccountStatus;

            int WRunningCycle = 503;
            int Mode = 1; 
            NForm291NVAccountStatus = new Form291NVAccountStatus(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRunningCycle, Mode);
            NForm291NVAccountStatus.Show();
        }
// View Transactions 
        private void button2_Click(object sender, EventArgs e)
        {
            Form80bNV NForm80bNV;

            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo,
                                              0, "View", NullPastDate, NullPastDate);
            NForm80bNV.ShowDialog();
        }
    }
}

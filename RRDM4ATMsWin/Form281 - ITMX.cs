using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;

namespace RRDM4ATMsWin
{
    public partial class Form281 : Form
    {

        int StepNumber = 1;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56

        //   bool Reject; 

        //  bool ViewWorkFlow ;

        //  bool ReconciliationAuthorNeeded;
        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        //RRDMMatchingCategoriesSessions Rs = new RRDMMatchingCategoriesSessions();

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX(); 

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();


        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();


        //       string WUserBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCategory;
        int WReconcCycleNo;

        public Form281(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InReconcCycleNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategory = InCategory;
            WReconcCycleNo = InReconcCycleNo;

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserBankId = Us.Operator;
            // ========================================================

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            if (WCategory == "EWB110")
            {
                label2.Text = "Latest Repl Cycle";
            }
            else
            {
                label2.Text = "Latest RM Cycle";
            }

            labelATMno.Text = WCategory;
            labelSessionNo.Text = WReconcCycleNo.ToString();

            if (WOperator == "ITMX")
            {
                Rcs.ReadReconcCategoriesByCategoryIdForName(WCategory); 
                label1.Text = "Category";
                labelATMno.Text = Rcs.CategoryName;
                label2.Text = "Category Cycle";
                labelSessionNo.Text = WReconcCycleNo.ToString(); 
            }

            // STEPLEVEL

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
           
            Usi.WFieldChar1 = WCategory;
            Usi.WFieldNumeric1 = WReconcCycleNo;
            Usi.StepLevel = 0;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            
            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            //------------------------------------------------------------

            bool Reject = false;

            // View only 
            if (WViewFunction == true)  // Viewing only ,,,  Cycle workflow had finished and we want to view work 
            {
                // Go to Step A 

                //Start with STEP 1
                UCForm281a Step281a = new UCForm281a();

                //Step281a.ChangeBoardMessage += Step281a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step281a, 0, 0);
                Step281a.Dock = DockStyle.Fill;
                Step281a.UCForm281aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                Step281a.SetScreen();
                textBoxMsgBoard.Text = Step281a.guidanceMsg;

                textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;

                StepNumber = 1;

                //   ViewWorkFlow = true;

            }

            // Coming from Authoriser OR Requestor 
            if (WAuthoriser == true || WRequestor == true)  // 55: Coming from authoriser , 56: Coming from requestor 
            {
                // READ AND UPDATE REJECT BEFORE AUTH RECORD CLOSES 

                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WReconcCycleNo, "ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (WRequestor == true & Ap.Stage == 4 & Ap.AuthDecision == "NO") Reject = true;
                }

                UCForm281c Step281c = new UCForm281c();

                Step281c.ChangeBoardMessage += Step281c_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step281c, 0, 0);
                Step281c.Dock = DockStyle.Fill;
                Step281c.UCForm281cPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                textBoxMsgBoard.Text = Step281c.guidanceMsg;

                // At this point authorisation record had closed if reject

                Ap.GetMessageOne(WCategory, WReconcCycleNo, "ReconciliationCat", WAuthoriser, WRequestor, Reject);

                textBoxMsgBoard.Text = Ap.MessageOut;

                labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep3.ForeColor = Color.White;

                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 16, FontStyle.Bold);

                // Step to 4
                StepNumber = 3;

                //   ViewWorkFlow = true;

                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;
            }

            if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) // Follow Reconciliation Workflow - NORMAL 2 is for single and 5 for bulk 
            {

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                // START RECONCILIATION 
                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategory, WReconcCycleNo); 
                Rcs.StartReconcDtTm = DateTime.Now;
                Rcs.UpdateReconcCategorySessionStartDate(WCategory, WReconcCycleNo);

                //Start with STEP 1
                UCForm281a Step281a = new UCForm281a();

                //Step281a.ChangeBoardMessage += Step281a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step281a, 0, 0);
                Step281a.Dock = DockStyle.Fill;
                Step281a.UCForm281aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                Step281a.SetScreen();
                textBoxMsgBoard.Text = Step281a.guidanceMsg;

                if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) textBoxMsgBoard.Text = "Review and go to Next";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;
            }
        }

        // NEXT STEP 
        //
        private void buttonNext_Click(object sender, EventArgs e)
        {
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

            //-----------------------------------------------------------

            if (StepNumber == 1)
            {
                // STEPLEVEL// Check Feasibility to move to next step 

                //if (Us.StepLevel < 1)
                //{
                //    MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                //    return;
                //}

                //GetMainBodyImage("UCForm281a");

                UCForm281b Step281b = new UCForm281b();

                //Step281b.ChangeBoardMessage += Step281b_ChangeBoardMessage;

                Step281b.Dock = DockStyle.Fill;

                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                tableLayoutPanelMain.Controls.Add(Step281b, 0, 0);
                Step281b.UCForm281bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo, 1);
                Step281b.SetScreen();
                textBoxMsgBoard.Text = Step281b.guidanceMsg;

                if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) textBoxMsgBoard.Text = "Examine Exceptions and Create Meta Exceptions.";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                labelStep1.ForeColor = labelStep3.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep2.ForeColor = Color.White;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                StepNumber++;
                buttonBack.Visible = true;
                buttonNext.Visible = true;

                if (Usi.ProcessNo == 5)
                {
                    StepNumber++;
                }
            }
            else
            {
                if (StepNumber == 2)
                {
                    //// STEPLEVEL// Check Feasibility to move to next step 

                    if (Usi.StepLevel == 2 & Usi.ProcessStatus > 1)
                    {
                        if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                        {
                            // Application.Exit();
                        }
                        else return;
                    }
                    UCForm281c Step281c = new UCForm281c();

                    Step281c.ChangeBoardMessage += Step281c_ChangeBoardMessage;

                    Step281c.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step281c, 0, 0);
                    Step281c.UCForm281cPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                    Step281c.SetScreen();
                    textBoxMsgBoard.Text = Step281c.guidanceMsg;

                    if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) textBoxMsgBoard.Text = "Apply Meta Exceptions as needed.";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep2.ForeColor = labelStep1.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep1.Font.Style);
                    labelStep3.ForeColor = Color.White;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 16, FontStyle.Bold);

                    StepNumber++;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Finish";
                    buttonNext.Visible = true;

                    if (Usi.ProcessNo == 5)
                    {
                        buttonNext.Visible = true;
                        buttonNext.Text = "Finish";
                    }
                    if (StepNumber == 3) // FINISH BUTTON WAS PUSHED - FINAL WORK IS DONE HERE 
                    {
                        // Do the final Work 
                    }

                    }
                else
                {
                    if (StepNumber == 3)
                    {
                        //// STEPLEVEL// Check Feasibility to move to next step 

                        //if (Us.StepLevel == 3 & Us.ProcessStatus > 1)
                        //{
                        //    if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        //          == DialogResult.Yes)
                        //    {
                        //        // Application.Exit();
                        //    }
                        //    else return;
                        //}

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

                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WReconcCycleNo, "ReconciliationCat");
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
                        using (var scope = new System.Transactions.TransactionScope())
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
                                    // UPDATE EXCEPTION RECORD AND MASTER RECORD 
                                    //
                                    //**************************************************************************
                                    Mre.CreateActionsforMatchingReconcExceptionsInfoClassForALLNotAuthor(WOperator, WCategory);
                                  
                                    //
                                    Mre.ReadMatchingReconcExceptionsInfoForTotals(WOperator, WCategory, WReconcCycleNo);
                                    //
                                    // Update Reconciliation Cycle 
                                    //
                                    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategory, WReconcCycleNo);

                                    Rcs.NumberSettledUnMatchedWorkFlow = Mre.TotalExceptionsDoneAndAuthorised; 
                                    Rcs.RemainReconcExceptions = Mre.TotalExceptionsOutstandingToBeAuthorised;
                                    Rcs.EndReconcDtTm = DateTime.Now;

                                    Rcs.UpdateReconcCategorySessionWithAuthorClosing(WCategory, WReconcCycleNo);

                                    //MessageBox.Show("No transactions to be posted");
                                    Form2 MessageForm = new Form2("Reconciliation Workflow has finished for: " + WCategory + Environment.NewLine
                                                                   + "Actions were taken as a result" + Environment.NewLine
                                                                   );
                                    MessageForm.ShowDialog();
                                    // COMPLETE SCOPE
                                    //
                                    scope.Complete();

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
                            finally
                            {
                                scope.Dispose();
                            }

                    }
                    else
                    {
                        
                    }
                }

            }
            if (StepNumber == 1 & Usi.ProcessNo == 8)
            {
                //  GetMainBodyImage("UCForm51d");

                //   Form57 Report2 = new Form57(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, SCREENd);
                //    Report2.Show();
            }
        }

        // Stavros
        // EVENT HANDLER FOR MESSAGE

        void Step281a_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm281a)tableLayoutPanelMain.Controls["UCForm281a"]).guidanceMsg;
        }
        void Step281b_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm281b)tableLayoutPanelMain.Controls["UCForm281bnew"]).guidanceMsg;
        }

        void Step281c_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm281c)tableLayoutPanelMain.Controls["UCForm281c"]).guidanceMsg;
        }

        void Step281d_ChangeBoardMessage(object sender, EventArgs e)
        {

            //textBoxMsgBoard.Text = ((UCForm281d)tableLayoutPanelMain.Controls["UCForm281d"]).guidanceMsg;
        }

        //
        // GO BACK BACK... BACK
        // GO BACK .. BACK ... BACK 
        private void buttonBack_Click(object sender, EventArgs e)
        {
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

            //-----------------------------------------------------------

            if (StepNumber == 2)
            {
                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                UCForm281a Step281a = new UCForm281a();
                //Step281a.ChangeBoardMessage += Step281a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step281a, 0, 0);
                Step281a.Dock = DockStyle.Fill;
                Step281a.UCForm281aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                Step281a.SetScreen();
                textBoxMsgBoard.Text = Step281a.guidanceMsg;

                if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                labelStep2.ForeColor = labelStep3.ForeColor;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                labelStep1.ForeColor = Color.White;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);
                //         textBoxMsgBoard.Text = "INSERT DATA AND UPDATE";
                StepNumber--;
                buttonBack.Visible = false;
            }
            else
            {

                if (StepNumber == 3)
                {
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    UCForm281b Step281b = new UCForm281b();
                    //Step281b.ChangeBoardMessage += Step281b_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step281b, 0, 0);
                    Step281b.Dock = DockStyle.Fill;
                    Step281b.UCForm281bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo, 1);
                    Step281b.SetScreen();
                    textBoxMsgBoard.Text = Step281b.guidanceMsg;

                    if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep3.ForeColor = labelStep2.ForeColor;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                    StepNumber--;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Next >";
                }
                else
                {
                    if (StepNumber == 4)
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm281c Step281c = new UCForm281c();
                        Step281c.ChangeBoardMessage += Step281c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step281c, 0, 0);
                        Step281c.Dock = DockStyle.Fill;
                        Step281c.UCForm281cPar(WSignedId, WSignRecordNo, WOperator, WCategory, WReconcCycleNo);
                        Step281c.SetScreen();
                        textBoxMsgBoard.Text = Step281c.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        //labelStep4.ForeColor = labelStep3.ForeColor;
                        //labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 16, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Next >";
                    }
                    else
                    {
                        if (StepNumber == 5)
                        {

                        }
                    }
                }
            }
        }



        // Cancel button 

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit reconciliation process?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // THIS IMPROVES PERFORMANCE 
        // 
        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }
        // Cancel
        private void Cancel_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;

namespace RRDM4ATMsWin
{
    public partial class SWD_Form271 : Form
    {

        int StepNumber;

        //bool Step2AndStep3Omitted = false;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56

        //string WSelectionCriteria;

        //  bool ViewWorkFlow ;

        //  bool ReconciliationAuthorNeeded;
        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        //RRDMMatchingCategoriesSessions Rcs = new RRDMMatchingCategoriesSession

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

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
        int WRMCycle;

        public SWD_Form271(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategory = InCategory;
            WRMCycle = InRMCycle;

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserBankId = Us.Operator;
            // ========================================================

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            //labelCatId.Text = WCategory;
            //labelCycleNo.Text = WRMCycle.ToString();

            StepNumber = 1;

            // STEPLEVEL
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategory;
            Usi.WFieldNumeric1 = WRMCycle;

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
                SWD_UCForm271a Step271a = new SWD_UCForm271a();

                //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.SWD_UCForm271aPar(WSignedId, WSignRecordNo, WOperator);
                Step271a.SetScreen();
                //textBoxMsgBoard.Text = Step271a.guidanceMsg;

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
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WRMCycle, "SWDSession");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (WRequestor == true & Ap.Stage == 4 & Ap.AuthDecision == "NO") Reject = true;
                }

                SWD_UCForm271b Step271b = new SWD_UCForm271b();

                //Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                Step271b.Dock = DockStyle.Fill;
                Step271b.SWD_UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, WCategory, WRMCycle);
                textBoxMsgBoard.Text = Step271b.guidanceMsg;

                // At this point authorisation record had closed if reject

                Ap.GetMessageOne(WCategory, WRMCycle, "SWDSession", WAuthoriser, WRequestor, Reject);

                textBoxMsgBoard.Text = Ap.MessageOut;

                //labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep2.ForeColor = Color.White;

                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                // Step to 2
                StepNumber = 2;

                //   ViewWorkFlow = true;

                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;
            }

            if (Usi.ProcessNo == 7) // SWD Session
            {

                //Start with STEP 1
                //Start with STEP 1
                SWD_UCForm271a Step271a = new SWD_UCForm271a();

                //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.SWD_UCForm271aPar(WSignedId, WSignRecordNo, WOperator);
                Step271a.SetScreen();

                textBoxMsgBoard.Text = "Review and Act";

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
                // Check if ... 
            }

            if (StepNumber == 1)
            {

                // STEPLEVEL// Check Feasibility to move to next step 
               
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.ReconcDifferenceStatus == 0 & Usi.ProcessNo == 7)
                {
                    MessageBox.Show("You Cannot Move to Authorisation. Make updating of data!");
                    return; 
                }
               
                SWD_UCForm271b Step271b = new SWD_UCForm271b();

                //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;

                Step271b.Dock = DockStyle.Fill;

                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                Step271b.SWD_UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, WCategory, WRMCycle);
                Step271b.SetScreen();

                textBoxMsgBoard.Text = "Complete the Reconciliation Process";

                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                //labelStep1.ForeColor = labelStep3.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                //labelStep2.ForeColor = Color.White;
                //labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                labelStep2.ForeColor = Color.White;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                StepNumber++;
                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;
            }
            else
            {
                if (StepNumber == 2)
                {
                    //**************************************************
                    //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
                    //**************************************************
                    
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    WRMCycle = Usi.WFieldNumeric1;

                    if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 

                    if (WViewFunction == true) // Coming from View only 
                    {
                        this.Close();
                        return;
                    }

                    // FINISH - Make validationsfor Authorisations  


                    //  ReconciliationAuthorNeeded = true;

                    Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WRMCycle, "SWDSession");
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

                    if (Usi.ProcessNo == 7 & ReconciliationAuthorDone == true) //  
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

                                // Create the transactions as a result of actions taken on Errors
                                // 
                                // THE BELOW METHOD CREATES THE TRANSACTIONS TO BE POSTED 
                                // Updates also the reconcilation record and ATMs reconciliation 

                                RRDMSWDPackagesDistributionSessions Ds 
                                                      = new RRDMSWDPackagesDistributionSessions();

                                //Us.ReadSignedActivityByKey(WSignRecordNo);

                                Ds.ReadSWDPackDistrSesbyPackDistrSesNo(WOperator, WRMCycle);
                                Ds.Approver = Ap.Authoriser;
                                Ds.UpdateSWDPackDistrSes(WOperator, WRMCycle);

                                string WMessage = "Updating and Authorisation has been done for : " + Environment.NewLine
                                   + " SWD Category :" + Ds.SWDCategoryId + Environment.NewLine
                                   + " For Pack ....:" + Ds.PackageId + Environment.NewLine
                                   + " For Session .:" + Ds.PackDistrSesNo;

                                Form2 MessageForm = new Form2(WMessage);
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

                SWD_UCForm271a Step271a = new SWD_UCForm271a();
                //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.SWD_UCForm271aPar(WSignedId, WSignRecordNo, WOperator);
                Step271a.SetScreen();
                //textBoxMsgBoard.Text = Step271a.guidanceMsg;

                if (Usi.ProcessNo == 7) textBoxMsgBoard.Text = "Review Information";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true)
                                                     textBoxMsgBoard.Text = "View only";

                labelStep2.ForeColor = labelStep1.ForeColor;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                labelStep1.ForeColor = Color.White;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);
                //         textBoxMsgBoard.Text = "INSERT DATA AND UPDATE";
                StepNumber--;
                buttonBack.Visible = false;

                buttonNext.Text = "Next";
                buttonNext.Visible = true;
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

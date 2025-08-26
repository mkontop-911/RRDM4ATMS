using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;

namespace RRDM4ATMsWin
{
    public partial class Form51_CDM : Form
    {
        Bitmap SCREENa;
        Bitmap SCREENb;
        Bitmap SCREENc;
        Bitmap SCREENd;
        Bitmap SCREENe;

        string ReconcComment;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56
        bool AutherPresent;

        string RRDMCashManagement;
        //  CASH MANAGEMENT Prior Replenishment Workflow  
        string CashEstPriorReplen; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

        int StepNumber = 1;

        bool NoDeposits;

        bool ViewWorkFlow;

        bool ReplenishmentAuthorNeeded;

        bool ReplenishmentAuthorNoRecordYet;
        bool ReplenishmentAuthorOutstanding;
        bool ReplenishmentAuthorDone;

        int WAtmsReconcGroup;

        string WReconcCategoryId;

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public Form51_CDM(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserOperator = Us.Operator;
            // ========================================================
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelATMno.Text = WAtmNo;
            labelSessionNo.Text = WSesNo.ToString();
            labelToday.Text = DateTime.Now.ToShortDateString();

            // STEPLEVEL

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 0) Usi.ProcessNo = 1;
            Usi.StepLevel = 0;
            Usi.ReplStep1_Updated = false;
            Usi.ReplStep2_Updated = false;
            Usi.ReplStep3_Updated = false;
            Usi.ReplStep4_Updated = false;
            //Usi.ReplStep5_Updated = false;

            //  Usi.WFieldNumeric1 = 0;
            //  Usi.WFieldNumeric11 = 0;
            Usi.WFieldNumeric12 = 0;

            //cmd.Parameters.AddWithValue("@WFieldChar1", WFieldChar1);
            //cmd.Parameters.AddWithValue("@WFieldChar2", WFieldChar2);

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

            //-----------------------------------------------------------

            //------------------------------------------------------------

            bool Reject = false;

            // Check for DEPOSITS function for this ATM
            //Ac.ReadAtm(WAtmNo);
            //if (Ac.ChequeReader == false & Ac.DepoReader == false & Ac.EnvelopDepos == false) // NO DEPOSITS
            //{
            //    NoDeposits = true;
            //}
            //WAtmsReconcGroup = Ac.AtmsReconcGroup;

            //RRDMReconcCategories Rc = new RRDMReconcCategories();

            //Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

            //WReconcCategoryId = Rc.CategoryId;


            // View only 
            // View only 
            if (WViewFunction == true)  // Viewing only ,,, Repl Cycle workflow had finished and we want t
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                labelToday.Text = Ta.SesDtTimeEnd.ToShortDateString();

            }
            // Coming from Requestor 
            if (WRequestor == true)  // 56: Coming from requestor 
            {
                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");

                if (WRequestor == true & Ap.RecordFound == true & Ap.OpenRecord == true
                    & Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Reject = true;

                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.ProcessNo = 1;

                    WRequestor = false;

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                    MessageBox.Show("Authorisation has been rejected." + Environment.NewLine
                        + "Authorisation Comment is below: " + Environment.NewLine
                        + Ap.AuthComment
                                   );
                }
                else
                {
                    if (Ap.RecordFound == true & Ap.OpenRecord == true
                          & Ap.Stage == 4)
                    {
                        MessageBox.Show("Go through the workflow." + Environment.NewLine
                         + "See Authoriser action at the last step." + Environment.NewLine
                         + "At the last step press finish to complete."
                     );
                    }
                }
            }

            if (WAuthoriser == true)  // 55: Coming from Authoriser
            {
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.OpenRecord == true
                    & Ap.Stage == 2)
                {
                    MessageBox.Show("Go through the workflow." + Environment.NewLine
                               + "Take Action at the last step."
                    );
                }

            }

            //if (Usi.ProcessNo == 5 || Usi.ProcessNo == 31 || Usi.ProcessNo == 26) Usi.ProcessNo = 1;

            if (Usi.ProcessNo == 1
                  || Usi.ProcessNo == 54 // ViewOnly 
                || Usi.ProcessNo == 55 // Authoriser
                || Usi.ProcessNo == 56 // Requestor    
                ) // Follow Replenishment Workflow
                  //if (Us.ProcessNo == 1 || Us.ProcessNo == 5 || Us.ProcessNo == 31 || Us.ProcessNo == 26) // Follow Replenishment Workflow
            {
                // REPLENISHMENT STARTS
                if (Usi.ProcessNo == 1)
                {
                    Ta.UpdateTracesStartRepl(WSignedId, WAtmNo, WSesNo);
                }

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
                else ViewWorkFlow = false;

                ////Start with STEP 1
                //UCForm51a_CDM Step51a_CDM = new UCForm51a_CDM();
                //// Stavros
                //Step51a_CDM.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                //tableLayoutPanelMain.Controls.Add(Step51a_CDM, 0, 0);
                //Step51a_CDM.Dock = DockStyle.Fill;
                //Step51a_CDM.UCForm51a_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                //Step51a_CDM.SetScreen();
                //textBoxMsgBoard.Text = Step51a_CDM.guidanceMsg;

                ////
                // STEPLEVEL// Check Feasibility to move to next step 
                //GetMainBodyImage("UCForm51b");
              

                UCForm51c_CDM Step51c_CDM = new UCForm51c_CDM();
                //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                Step51c_CDM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51c_CDM, 0, 0);
                Step51c_CDM.Dock = DockStyle.Fill;
                Step51c_CDM.UCForm51c_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51c_CDM.SetScreen();
                textBoxMsgBoard.Text = Step51c_CDM.guidanceMsg;

                //

                if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;

                if (Usi.ReplStep1_Updated == true & ViewWorkFlow == false)
                {
                    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                }
            }

        }
        /*
        void Step71a_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).guidanceMsg;
        }
         */

        //**************************************************************************
        //NEXT
        //**************************************************************************

        private void buttonNext_Click_1(object sender, EventArgs e)
        {

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 


            Usi.ReadSignedActivityByKey(WSignRecordNo);

            //StepNumber = Us.StepLevel; 

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
            else ViewWorkFlow = false;

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 1 || ViewWorkFlow == true)
            {
            }
            else
            {
                return;
            }

            //if (StepNumber == 1 & Na.Balances1.OpenBal == 0 & (Usi.ReplStep1_Updated == true || ViewWorkFlow == true))
            //{
            //    StepNumber = 2; // Jump step

            //    labelStep1.ForeColor = labelStep3.ForeColor;
            //    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

            //    Usi.ReadSignedActivityByKey(WSignRecordNo);

            //    Usi.ReplStep2_Updated = true;

            //    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            //}

            //if (StepNumber == 2 & NoDeposits == true)
            //{
            //    StepNumber = 3; // Jump if no deposits
            //}

            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                case 1:
                    {

                        // STEPLEVEL// Check Feasibility to move to next step 

                        if (Usi.ReplStep1_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                        //GetMainBodyImage("UCForm51a");

                        UCForm51b_CDM Step51b_CDM = new UCForm51b_CDM();

                        //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        Step51b_CDM.ChangeBoardMessage += Step51b_ChangeBoardMessage;

                        Step51b_CDM.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        Step51b_CDM.UCForm51b_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                        textBoxMsgBoard.Text = Step51b_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        if (Usi.ReplStep2_Updated == true & ViewWorkFlow == false)
                        {
                            textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                        }

                        tableLayoutPanelMain.Controls.Add(Step51b_CDM, 0, 0);

                        labelStep1.ForeColor = labelStep3.ForeColor;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);

                        StepNumber++; // STEP becomes two
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;
                    }

                case 2:
                    {

                        if (Usi.ReplStep2_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                        // STEPLEVEL// Check Feasibility to move to next step 
                        //GetMainBodyImage("UCForm51b");
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm51c_CDM Step51c_CDM = new UCForm51c_CDM();
                        //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        Step51c_CDM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51c_CDM, 0, 0);
                        Step51c_CDM.Dock = DockStyle.Fill;
                        Step51c_CDM.UCForm51c_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        Step51c_CDM.SetScreen();
                        textBoxMsgBoard.Text = Step51c_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        if (Usi.ReplStep3_Updated == true & ViewWorkFlow == false)
                        {
                            textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                        }

                        labelStep2.ForeColor = labelStep1.ForeColor;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;

                    }
                case 3:
                    {

                        // last step - Make validationsfor Authorisations  

                        UCForm51d_CDM Step51d_CDM = new UCForm51d_CDM();
                        Step51d_CDM.ChangeBoardMessage += Step51d_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51d_CDM, 0, 0);
                        Step51d_CDM.Dock = DockStyle.Fill;
                        Step51d_CDM.UCForm51d_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51d_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep3.ForeColor = labelStep1.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep4.ForeColor = Color.White;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Finish";

                        break;
                    }

                case 6:
                    {
                        // THE FINISH BUTTON WAS ACTIVATED

                        //**************************************************
                        //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
                        //**************************************************

                        if (WViewFunction == true) // Coming from View only 
                        {
                            this.Close();
                            return;
                        }

                        // CHECK IF Replenishment ELECTRONIC AUTHORISATION IS NEEDED

                        string ParId = "261";
                        string OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                        if (Gp.OccuranceNm == "YES") // Electronic authorization needed  
                        {
                            AutherPresent = true;
                        }

                        if (AutherPresent) // Electronic authorization needed  
                        {
                            ReplenishmentAuthorNeeded = true;

                            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "Replenishment");
                            if (Ap.RecordFound == true)
                            {
                                ReplenishmentAuthorNoRecordYet = false;
                                if ((Ap.Stage == 3 || Ap.Stage == 4))
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

                            if (Ap.AuthDecision == "NO" & ReplenishmentAuthorDone == true)
                            {
                                this.Close();
                                return;
                            }
                            // Keep this 
                            if (Usi.ProcessNo == 1 & ReplenishmentAuthorDone == true) //  
                            {
                                this.Close();
                                return;
                            }

                            if (Usi.ProcessNo == 1 & ReplenishmentAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
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

                            if (Usi.ProcessNo == 1 & ReplenishmentAuthorOutstanding == true) // Cancel with repl outstanding 
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

                            if (WAuthoriser == true & ReplenishmentAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
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

                            if (WRequestor == true & ReplenishmentAuthorDone == true & Ap.Stage == 4
                              & Ap.AuthDecision == "YES") // Everything is fined .
                            {
                                // Everything OK to move to transactions
                            }
                            else
                            {
                                MessageBox.Show("MSG946 - Authorisation outstanding");
                                this.Dispose();
                                return;
                            }

                        }
                        //
                        // IF YOU HAVE REACH TILL HERE UPDATING WILL TAKE  PLACE 
                        //

                        if (Ap.RecordFound == true & Ap.Stage != 4)
                        {
                            // Program Logic error  
                            //
                            MessageBox.Show("Program Logic error");
                            return;
                        }
                        //************************************************************
                        //************************************************************
                        // Update authorisation record  
                        //************************************************************
                        //************************************************************

                        // create a connection object
                        using (var scope = new System.Transactions.TransactionScope())
                            try
                            {
                                if (Ap.RecordFound == true & Ap.Stage == 4)
                                {
                                    // Update stage 
                                    //
                                    Ap.Stage = 5;
                                    Ap.OpenRecord = false;

                                    Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                                }

                                //GetMainBodyImage("UCForm51e");
                                //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                //if ((Ac.CitId == "1000" & AutherPresent & Ac.TypeOfRepl != "50"))
                                if (AutherPresent)
                                {
                                    // Create the transactions 
                                    // DR/CR Till , DR/CR ATM Cash 
                                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                                    //                        
                                    //
                                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                                    string WJobCategory = "ATMs";
                                    int WReconcCycleNo;

                                    WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                                    // Create the transactions 
                                    // DR/CR Till , DR/CR ATM Cash 
                                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                                    //                        
                                    //
                                    ////
                                }


                                //  ClassCashInOut Cout = new ClassCashInOut(); // 

                                if (WAtmNo == "AB104") // Create Transactions For CASH taken out of the ATM .. 
                                {

                                    // THIS FUNCTIONALITY HAS BEEN MOVED TO Form52 

                                    //    Cout.InsertTranForCashOut(WSignedId, WSignRecordNo, WOperator, WPrive, WAtmNo, WSesNo);

                                }

                                // UPDATE TRACES WITH FINISH 
                                // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                                int Mode = 1; // Before reconciliation 
                                if (WOperator == "CRBAGRAA")
                                {
                                    //Ta.UpdateTracesFinishReplOrReconc(WAtmNo, WSesNo, WSignedId, Mode);
                                    Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, WSignedId, WSignedId, WReconcCategoryId);
                                }
                                else
                                {
                                    // NBG CASE 
                                    // UPDATE THAT RRDM REPLENISHMENT HAS FINISHED
                                    // SET Ta Process Mode to 1 = ready for GL Reconciliation
                                    // UPDATE Bank Record with counted inputed amount 

                                    //Ta.UpdateTracesFinishRepl_From_Form51_NBG(WAtmNo, WSesNo, WSignedId, WReconcCategoryId);

                                    Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, WSignedId, WSignedId, WReconcCategoryId);
                                }

                                // READ Ta to see if differences 

                                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                                string SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
                                Pi.ReadPhysicalInspectionRecordsToSeeIfAlert(SelectionCriteria);
                                //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

                                //Am.UpdateAtmsMain(WAtmNo);

                                RRDMReconcJobCycles Djc = new RRDMReconcJobCycles();
                                Djc.ReadLastReconcJobCycle(WOperator);

                                if (Ta.Repl1.DiffRepl == false & Ta.Recon1.DiffReconcEnd == false & Ta.Is_Updated_GL == true)
                                {
                                    // There no differences to reconcile

                                    ReconcComment = "EVERYTHING INCLUDING HOST FILES RECONCILE";
                                }

                                if (Ta.Repl1.DiffRepl == true || (Ta.Recon1.DiffReconcEnd == true & Ta.Is_Updated_GL == true))
                                {
                                    // There differences to reconcile

                                    ReconcComment = "NEED TO GO TO RECONCILIATION PROCESS";

                                    //  UPDATE RECONCILIATION Outstanding
                                    //if (WOperator != "ETHNCY2N" & Djc.JobCycle != 0)
                                    //{
                                    // FOR NBG THIS IS UPDATED DURING EXCEL LOADING 
                                    //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WReconcCategoryId, Djc.JobCycle);

                                    //Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;
                                    //Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                                    //Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WReconcCategoryId, Djc.JobCycle);
                                    //}
                                }

                                if (Ta.Recon1.DiffReconcEnd == true & Ta.Is_Updated_GL == false)
                                {
                                    // There differences to reconcile but host not available

                                    ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
                                }

                                if (Ta.Recon1.DiffReconcEnd == false & Ta.Is_Updated_GL == false)
                                {
                                    // There differences to reconcile

                                    ReconcComment = "HOST FILES NOT AVAILABLE YET";
                                }
                                if (Pi.InspectionAlert == true)
                                {
                                    ReconcComment = "Inspection Alert issue exist";
                                }

                                if (Na.ReplUserComment == "")
                                {
                                    Na.ReplUserComment = "No User Comment was inserted.";
                                }

                                if (ReplenishmentAuthorNeeded == false) // No authorisation 
                                {
                                    if (NoDeposits == false)
                                    {
                                        // PRINT ALL IMAGES INCLUDING DEPOSITS 
                                        Form56 Report1 = new Form56(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo,
                                                        SCREENa, SCREENb, SCREENc, SCREENd, "", Na.ReplUserComment, Ta.ReplGenComment, ReconcComment);
                                        Report1.Show();
                                    }
                                    else
                                    {
                                        // Print without the deposits
                                    }

                                    MessageBox.Show("You have completed the replenishment workflow with: " + WAtmNo + Environment.NewLine
                                                                  + " Move to the next one if any one for replenishment.");

                                }
                                else // Authorisation took place 
                                {

                                    //MessageBox.Show
                                    Form2 MessageForm = new Form2("You have completed the replenishment for: " + WAtmNo + Environment.NewLine
                                                                   + "Transactions are created." + Environment.NewLine
                                                                   + "Move to the next replenishment if any.");
                                    MessageForm.ShowDialog();

                                }

                                this.Close();

                                scope.Complete();

                                return;
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

                        break;

                    }

                default:
                    {

                        break;
                    }
            }

            // 

            try
            {

                if (StepNumber == 1 & Usi.ProcessNo == 8) // THIS THE FINISH FOR WFUNCTION = 8 
                {
                    GetMainBodyImage("UCForm51d");

                    MessageBox.Show("You have completed the replenishment IN MONEY. Report will be printed.");

                    this.Close();

                    Form57 Report2 = new Form57(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, SCREENd);
                    Report2.Show();
                }
            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }


        }



        // Stavros
        // EVENT HANDLER FOR MESSAGE
        void Step51a_ChangeBoardMessage(object sender, EventArgs e)
        {
            //  textBoxMsgBoard.Text = ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).guidanceMsg;
        }
        //
        // GO BACK 
        // 

        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
            else ViewWorkFlow = false;

            //bool Comesfrom4 = false;

            //if (StepNumber == 4)
            //{
            //    if (NoDeposits == true)
            //    {
            //        StepNumber = 3; // decrease by ONE ! 
            //        Comesfrom4 = true;

            //    }
            //    if (Na.Balances1.OpenBal == 0)
            //    {
            //        if (NoDeposits == true) StepNumber = 2; // decrease by Two 
            //        Comesfrom4 = true;
            //    }
            //}
            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                case 2:
                    {
                        UCForm51a_CDM Step51a_CDM = new UCForm51a_CDM();
                        Step51a_CDM.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51a_CDM, 0, 0);
                        Step51a_CDM.Dock = DockStyle.Fill;
                        Step51a_CDM.UCForm51a_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        Step51a_CDM.SetScreen();
                        textBoxMsgBoard.Text = Step51a_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                        labelStep2.ForeColor = labelStep3.ForeColor;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep1.ForeColor = Color.White;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 22, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = false;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Next >";

                        break;
                    }
                case 3:
                    {
                        UCForm51b_CDM Step51b_CDM = new UCForm51b_CDM();
                        Step51b_CDM.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51b_CDM, 0, 0);
                        Step51b_CDM.Dock = DockStyle.Fill;
                        Step51b_CDM.UCForm51b_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51b_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                        labelStep3.ForeColor = labelStep2.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);


                        StepNumber--;

                        buttonBack.Visible = true;
                        buttonNext.Visible = true;
                        buttonNext.Text = "Next >";
                        break;
                    }
                case 4:
                    {
                        UCForm51c_CDM Step51c_CDM = new UCForm51c_CDM();
                        Step51c_CDM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51c_CDM, 0, 0);
                        Step51c_CDM.Dock = DockStyle.Fill;
                        Step51c_CDM.UCForm51c_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51c_CDM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep4.ForeColor = labelStep3.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;
                        break;
                    }
                case 5:
                    {
                        // Finish
                        MessageBox.Show("Corrective Transactions will be created.");
                        this.Dispose(); 
                        return;

                        //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        //MessageBox.Show("Corrective Transactions will be created.");
                        //return;
                        ////  CASH MANAGEMENT External?? 
                        //Ac.ReadAtm(WAtmNo);
                        //string ParId = "202";
                        //string OccurId = "1";
                        //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        //RRDMCashManagement = Gp.OccuranceNm;
                        ////  CASH MANAGEMENT Prior Replenishment Workflow  
                        //ParId = "211";
                        //OccurId = "1";
                        //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        //CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THROUGHT AN ACTION 

                        //if ((Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen != "YES") & Ac.TypeOfRepl != "50"))
                        //{
                        //    // RRDM Cash Management is needed  
                        //    UCForm51d Step51d = new UCForm51d();
                        //    Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;

                        //    tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                        //    Step51d.Dock = DockStyle.Fill;
                        //    Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        //    textBoxMsgBoard.Text = Step51d.guidanceMsg;

                        //    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        //    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        //    //labelStep5.ForeColor = labelStep4.ForeColor;
                        //    //labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                        //    labelStep4.ForeColor = Color.White;
                        //    labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                        //    StepNumber--;
                        //    buttonBack.Visible = true;
                        //    buttonNext.Visible = true;

                        //    buttonNext.Text = "Next >";
                        //}
                        //else
                        //{
                        //    // RRDM Cash Management is NOT needed  
                        //    // Cash management is done by other Package OR Replenishment is for preset Max. 
                        //    // CIT != 1000 .... Order was created 
                        //    // Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl == "50" Order Was created and email was sent 

                        //    UCForm51d2 Step51d2 = new UCForm51d2();

                        //    Step51d2.ChangeBoardMessage += Step51d2_ChangeBoardMessage;
                        //    tableLayoutPanelMain.Controls.Add(Step51d2, 0, 0);
                        //    Step51d2.Dock = DockStyle.Fill;
                        //    Step51d2.UCForm51d2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        //    textBoxMsgBoard.Text = Step51d2.guidanceMsg;

                        //    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        //    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        //    //labelStep5.ForeColor = labelStep4.ForeColor;
                        //    //labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                        //    labelStep4.ForeColor = Color.White;
                        //    labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                        //    //labelStep2.ForeColor = labelStep1.ForeColor;
                        //    //labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        //    //labelStep3.ForeColor = labelStep1.ForeColor;
                        //    //labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        //    //labelStep5.ForeColor = labelStep4.ForeColor;
                        //    //labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                        //    //labelStep4.ForeColor = Color.White;
                        //    //labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);


                        //    StepNumber--;

                        //    buttonBack.Visible = true;
                        //    buttonNext.Visible = true;

                        //    buttonNext.Text = "Next >";
                        //}

                        break;

                    }
              
                default:
                    {

                        break;
                    }
            }


        }

        // MESSAGES
        void Step51e_ChangeBoardMessage(object sender, EventArgs e)

        {
            textBoxMsgBoard.Text = ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).guidanceMsg;
        }

        // MESSAGES
        void Step51f_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51f)tableLayoutPanelMain.Controls["UCForm51f"]).guidanceMsg;
        }

        void Step51d_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).guidanceMsg;
        }

        void Step51d2_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).guidanceMsg;
        }

        // void Step51d2_ChangeBoardMessage(object sender, EventArgs e)
        //  {
        //     textBoxMsgBoard.Text = ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).guidanceMsg;
        // }

        void Step51c_ChangeBoardMessage(object sender, EventArgs e)
        {
            //  textBoxMsgBoard.Text = ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).guidanceMsg;
        }

        void Step51b_ChangeBoardMessage(object sender, EventArgs e)
        {
            //  textBoxMsgBoard.Text = ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).guidanceMsg;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ButtonMessages_Click(object sender, EventArgs e)
        {


        }


        private void GetMainBodyImage(String ControlName)
        {
            System.Drawing.Bitmap memoryImage;

            if (ControlName.Equals("UCForm51a"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).Width, ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).Height);
                ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).DrawToBitmap(memoryImage, ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).ClientRectangle);
                SCREENa = memoryImage;
            }
            if (ControlName.Equals("UCForm51b"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).Width, ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).Height);
                ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).DrawToBitmap(memoryImage, ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).ClientRectangle);
                SCREENb = memoryImage;
            }
            if (ControlName.Equals("UCForm51c"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).Width, ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).Height);
                ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).DrawToBitmap(memoryImage, ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).ClientRectangle);
                SCREENc = memoryImage;
            }
            if (ControlName.Equals("UCForm51d"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).Width, ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).Height);
                ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).DrawToBitmap(memoryImage, ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).ClientRectangle);
                SCREENd = memoryImage;
            }

            if (ControlName.Equals("UCForm51d2"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).Width, ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).Height);
                ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).DrawToBitmap(memoryImage, ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).ClientRectangle);
                SCREENd = memoryImage;
            }
            if (ControlName.Equals("UCForm51e"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).Width, ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).Height);
                ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).DrawToBitmap(memoryImage, ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).ClientRectangle);
                SCREENe = memoryImage;
            }

        }
        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }

        private void Form51_Load(object sender, EventArgs e)
        {

        }
    }
}

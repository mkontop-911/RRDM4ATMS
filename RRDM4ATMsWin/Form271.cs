using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Security.Principal;

namespace RRDM4ATMsWin
{
    public partial class Form271 : Form
    {

        int StepNumber;

        bool Step2Omitted = false;
        //bool Step3Omitted = false;

        bool BDC231_Normal;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56

        bool IsOriginAtms;
        bool It_Belongs_To_Groups; // There are Groups 

        string WSelectionCriteria;

        //  bool ViewWorkFlow ;

        //  bool ReconciliationAuthorNeeded;
        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        //RRDMMatchingCategoriesSessions Rcs = new RRDMMatchingCategoriesSession

        RRDMMatchingCategories Rc = new RRDMMatchingCategories();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        bool AudiType;


        //       string WUserBankId;
       bool Is_Presenter_InReconciliation;


        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCategory;
        int WRMCycle;

        public Form271(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategory = InCategory;
            WRMCycle = InRMCycle;

            BDC231_Normal = false;

            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserBankId = Us.Operator;
            // ========================================================

            RRDMReconcCategories Rc = new RRDMReconcCategories();

            Rc.ReadReconcCategorybyCategId(WOperator, WCategory);

            if (WCategory.Substring(0, 4) == "RECA")
            {
                It_Belongs_To_Groups = true; // There are Groups 
            }
            else
            {
                // Incoming File is distributed to many dates 
                It_Belongs_To_Groups = false; // No Groups
            }

            if (Rc.Origin == "Our Atms")
            {
                IsOriginAtms = true;
            }
            else
            {
                IsOriginAtms = false;
            }

            // Set Working Date 

            pictureBox1.BackgroundImage = appResImg.logo2;

            StepNumber = 1;

            labelCatId.Text = WCategory;
            labelCycleNo.Text = WRMCycle.ToString();
            Rjc.ReadReconcJobCyclesById(WOperator, WRMCycle);
            labelCutOffDate.Text = Rjc.Cut_Off_Date.ToShortDateString();

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(InOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;


                }
                else
                {
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
            }

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

            // RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            //------------------------------------------------------------

            bool Reject = false;

            // Presenter Role ???
            //RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }


            // Requestor 
            if (WRequestor == true)  // 56: Coming from requestor 
            {
                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WRMCycle, "ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true
                    & Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    Reject = true;

                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.ProcessNo = 2;

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
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WRMCycle, "ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true
                    & Ap.Stage == 2)
                {
                    MessageBox.Show("Go through the workflow." + Environment.NewLine
                 + "Take Action at the last step."
                    );
                }

            }


            if (Usi.ProcessNo == 2 // Normal 
            || Usi.ProcessNo == 5 // Bulk 
            || Usi.ProcessNo == 54 // ViewOnly 
            || Usi.ProcessNo == 55 // Authoriser
            || Usi.ProcessNo == 56 // Requestor          
            )
            {

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                // START RECONCILIATION 
                if (Usi.ProcessNo == 2)
                {
                    Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategory, WRMCycle);
                    Rcs.StartReconcDtTm = DateTime.Now;
                    Rcs.UpdateReconcCategorySessionStartDate(WCategory, WRMCycle);
                    // UPDATE OLD ERRORS WITH MAIN ONLY 
                    // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
                    Ec.UpdateOldErrorsWithMainOnly(WOperator, WCategory, WRMCycle);
                }
               

                //Start with STEP 1
                if (It_Belongs_To_Groups == true) // There are Groups = ATMS 
                {
                    UCForm271a Step271a = new UCForm271a();

                    //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                    Step271a.Dock = DockStyle.Fill;
                    Step271a.UCForm271aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                    Step271a.SetScreen();
                    textBoxMsgBoard.Text = Step271a.guidanceMsg;

                    if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) textBoxMsgBoard.Text = "Review and go to Next";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep1.ForeColor = Color.White;
                    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);

                    labelStep2.ForeColor = labelStep3.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                    //labelStep4.ForeColor = labelStep3.ForeColor;
                    //labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep3.Font.Style);

                    buttonNext.Visible = true;
                }
                else
                {
                    // CATEGORIES 
                    UCForm271a_JCC Step271a_JCC = new UCForm271a_JCC();

                    //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step271a_JCC, 0, 0);
                    Step271a_JCC.Dock = DockStyle.Fill;
                    Step271a_JCC.UCForm271a_JCC_Par(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                    Step271a_JCC.SetScreen();
                    textBoxMsgBoard.Text = Step271a_JCC.guidanceMsg;

                    if (Usi.ProcessNo == 2 || Usi.ProcessNo == 5) textBoxMsgBoard.Text = "Review and go to Next";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep1.ForeColor = Color.White;
                    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);

                    labelStep2.ForeColor = labelStep3.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                    //labelStep4.ForeColor = labelStep3.ForeColor;
                    //labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep3.Font.Style);

                    buttonNext.Visible = true;
                }

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
            if (StepNumber == 1 || StepNumber == 2)
            {
                // Check if any remaining After Fast Track 
                string SelectionCriteria = "";
                string SelectionCriteria2 = "";
                if (StepNumber == 1)
                {
                    if (Is_Presenter_InReconciliation == true)
                    {
                        SelectionCriteria = " WHERE Operator ='" + WOperator
                          + "' AND RMCateg ='" + WCategory + "'"
                          + "  AND MatchingAtRMCycle =" + WRMCycle
                          + "  AND IsMatchingDone = 1 "
                          + "  AND FastTrack = 0   AND ( (Matched = 0 AND ActionType != '07')  " +
                          " OR (Matched = 1 AND MetaExceptionId = 55))";
                    }
                    else
                    {
                        SelectionCriteria = " WHERE Operator ='" + WOperator
                          + "' AND RMCateg ='" + WCategory + "'"
                          + "  AND MatchingAtRMCycle =" + WRMCycle
                          + "  AND IsMatchingDone = 1 "
                          + "  AND FastTrack = 0   AND Matched = 0 AND ActionType != '07'  AND MetaExceptionId <> 55 ";
                    }
                    
                }

                if (StepNumber == 2)
                {
                    if (Is_Presenter_InReconciliation == true)
                    {
                        SelectionCriteria2 = " WHERE Operator ='" + WOperator
                         + "' AND RMCateg ='" + WCategory + "'"
                         + "  AND MatchingAtRMCycle =" + WRMCycle
                         + "  AND IsMatchingDone = 1 "
                             + " AND ( "
                             +"(Matched = 0 "
                         + "  AND ActionType = '00' AND MetaExceptionId <> 55 )"
                         + "  OR (Matched = 1 "
                         + "  AND ActionType = '00' AND MetaExceptionId = 55 )"
                         + "  ) ";

                    }
                    else
                    {
                        SelectionCriteria2 = " WHERE Operator ='" + WOperator
                         + "' AND RMCateg ='" + WCategory + "'"
                         + "  AND MatchingAtRMCycle =" + WRMCycle
                         + "  AND IsMatchingDone = 1 "
                         //+ "  AND FastTrack = 0 " 
                         + "  AND Matched = 0 "
                         + "  AND (ActionType = '00' AND MetaExceptionId <> 55 )  ";
                    }
                        

                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria2, 1);

                    if (Mpa.RecordFound == true)
                    {
                        MessageBox.Show("Take Actions For All Before You Move To Next Step!");
                        return;
                    }

                    SelectionCriteria = " WHERE Operator ='" + WOperator
                      + "' AND RMCateg ='" + WCategory + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycle
                      + "  AND IsMatchingDone = 1 "
                      //+ "  AND FastTrack = 0 " 
                      + "  AND Matched = 0 "
                      + "  AND (ActionType != '07' AND ActionType != '04' AND ActionType != '05' AND ActionType != '06') ";

                }


                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

                if (Mpa.RecordFound == false)
                {

                    if (StepNumber == 1) Step2Omitted = true;
                    // if (StepNumber == 2) Step3Omitted = true;
                    // Jump Step
                    StepNumber = 2;
                    //   RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.ReconcDifferenceStatus = 1; // No Difference
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
                else
                {
                    Step2Omitted = false;
                }
            }

            if (StepNumber == 1)
            {
                if (AudiType == true)
                {
                    UCForm271b_FAB Step271b = new UCForm271b_FAB();
                    Step271b.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                    Step271b.UCForm271bPar_FAB(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, 1, "", 0);
                    Step271b.SetScreen();
                    textBoxMsgBoard.Text = Step271b.guidanceMsg;

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
                    UCForm271b Step271b = new UCForm271b();
                    //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;

                    Step271b.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                    Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, 1, "", 0);
                    Step271b.SetScreen();
                    textBoxMsgBoard.Text = Step271b.guidanceMsg;

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

            }
            else
            {
                if (StepNumber == 2)
                {
                    UCForm271d Step271d = new UCForm271d();

                    //Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;

                    Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;

                    Step271d.Dock = DockStyle.Fill;
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step271d, 0, 0);

                    Step271d.UCForm271dPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, WCategory, WRMCycle);
                    textBoxMsgBoard.Text = Step271d.guidanceMsg;

                    if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Complete the Reconciliation Process";

                    if (WViewFunction == true) textBoxMsgBoard.Text = "View only";

                    if (Step2Omitted == true)
                    {
                        labelStep1.ForeColor = labelStep3.ForeColor;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                    }

                    labelStep2.ForeColor = labelStep1.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                    labelStep3.ForeColor = Color.White;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 16, FontStyle.Bold);

                    StepNumber++;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Finish";
                    buttonNext.Visible = true;

                }
                else
                {
                    // FINISH BUTTOM 
                    if (StepNumber == 3) // FINISH BUTTON WAS PUSHED - FINAL WORK IS DONE HERE 
                    {
                        //// STEPLEVEL// Check Feasibility to move to next step 

                        if (Usi.StepLevel == 3 & Usi.ProcessStatus > 1)
                        {
                            if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                  == DialogResult.Yes)
                            {
                                // Application.Exit();
                            }
                            else return;
                        }

                        //**************************************************
                        //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
                        //**************************************************

                        if (WViewFunction == true) // Coming from View only 
                        {
                            this.Dispose();
                            return;
                        }

                        // FINISH - Make validationsfor Authorisations  

                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WRMCycle, "ReconciliationCat");
                        if (Ap.RecordFound == true)
                        {
                            ReconciliationAuthorNoRecordYet = false;

                            if (Ap.Stage == 3 || Ap.Stage == 4)
                            {
                                ReconciliationAuthorDone = true;
                            }
                            else
                            {
                                ReconciliationAuthorOutstanding = true;
                            }

                            if (Ap.AuthDecision == "NO" & ReconciliationAuthorDone == true)
                            {
                                this.Close();
                                return;
                            }
                            // Authorisation Record Exist 
                            if (WRequestor == true & Ap.AuthDecision == "YES" & Ap.Stage != 4 // To cover Local authorisation
                                || WRequestor == true & Ap.AuthDecision == "" // No action by authoriser yet
                                ) // Coming from requestor 
                            {
                                this.Close();
                                return;
                            }
                        }
                        else
                        {
                            ReconciliationAuthorNoRecordYet = true;
                        }


                        if (WAuthoriser == true & ReconciliationAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
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
                            //MessageBox.Show("MSG946 - Authorisation outstanding");
                            //this.Dispose();
                            //return;
                        }

                        if (WRequestor == true & ReconciliationAuthorNoRecordYet == true) // Cancel by authoriser without making authorisation.
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
                            //MessageBox.Show("MSG947 - Authorisation outstanding");
                            //this.Dispose();
                            //return;
                        }

                      

                        if (Usi.ProcessNo == 2 & ReconciliationAuthorDone == true) //  
                        {
                            this.Dispose();
                            return;
                        }

                        if (WRequestor == true & ReconciliationAuthorDone == false) // Coming from authoriser and authoriser not done 
                        {
                            this.Dispose();
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


                       

                        //
                       

                        int TotalPairs = 0;
                        string WSortCriteria = " ";

                        string SelectionCriteria;

                       

                        //***********************************************************************
                        //**************** 
                        //***********************************************************************
                        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                        RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();
                        // create a connection object
                        //using (var scope = new System.Transactions.TransactionScope())
                        try
                        {
                            // Update authorisation record  

                            if (WAuthoriser == true & Ap.RecordFound == true & (Ap.Stage == 3 || Ap.Stage == 4)
                                || WRequestor == true & Ap.Stage == 4  // Local Authorisation 
                                )
                            {
                               

                                // FIND CURRENT CUTOFF CYCLE
                                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                                string WJobCategory = "ATMs";

                                int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                                // HERE YOU CALL STORE PROCEDURE TO DEAL WITH ALL NO GL ACTIONS

                                int ReturnCode = -20;
                                //string ProgressText = "";
                                string ErrorReference = "";

                                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings
                                  ["ATMSConnectionString"].ConnectionString;

                                int ret = -1;

                                string SPName = "[ATMS].[dbo].[stp_ReconcUpdates_NON_GL_ACTIONS]";

                                using (SqlConnection conn2 = new SqlConnection(connectionString))
                                {
                                    try
                                    {
                                        conn2.Open();


                                        SqlCommand cmd = new SqlCommand(SPName, conn2);

                                        cmd.CommandType = CommandType.StoredProcedure;

                                        // the first are input parameters
                                        cmd.Parameters.Add(new SqlParameter("@RMCateg", WCategory));

                                        cmd.Parameters.Add(new SqlParameter("@RMCycle", WReconcCycleNo));

                                        cmd.Parameters.Add(new SqlParameter("@Requestor", Ap.Requestor));

                                        cmd.Parameters.Add(new SqlParameter("@Authoriser", Ap.Authoriser));

                                        cmd.Parameters.Add(new SqlParameter("@AuthSeqNumber", Ap.SeqNumber));

                                        // the following are output parameters

                                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                                        retCode.Direction = ParameterDirection.Output;
                                        retCode.SqlDbType = SqlDbType.Int;
                                        cmd.Parameters.Add(retCode);

                                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                                        retErrorReference.Direction = ParameterDirection.Output;
                                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
                                        retErrorReference.Size = 3000;
                                        cmd.Parameters.Add(retErrorReference);

                                        // execute the command
                                        cmd.CommandTimeout = 1800;  // seconds
                                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
                                       // ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                                        conn2.Close();

                                    }
                                    catch (Exception ex)
                                    {
                                        conn2.Close();
                                        string Error = ex.Message;
                                        CatchDetails(ex);
                                    }
                                }

                                if (ret == 0)
                                {

                                    // OK
                                    //MessageBox.Show("VALID CALL" + Environment.NewLine
                                    // + ProgressText);
                                }
                                else
                                {
                                    // NOT OK
                                    //MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_ITMX" + InTableId + Environment.NewLine
                                    //         + ProgressText);
                                }


                                TotalPairs = 0;

                                Aoc.ClearTableTxnsTableFromAction();

                                SelectionCriteria = " WHERE RMCateg='" + WCategory + "' AND MatchingAtRMCycle =" + WRMCycle
                                                                        + " AND  OriginWorkFlow = 'Reconciliation' AND Stage <> '03' AND Maker='" + Ap.Requestor + "' ";
                                Aoc.ReadActionsOccurancesAndFillTable_Big(SelectionCriteria);

                                int I = 0;
                                // We have read all Actions Occurances 
                                while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                                {
                                    //    RecordFound = true;
                                    int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];
                                    //bool WSettled = (bool)Aoc.TableActionOccurances_Big.Rows[I]["Settled"];

                                    // Update authoriser 
                                    Aoc.UpdateOccurancesForAuthoriser_2(WSeqNo, Ap.Authoriser, Ap.SeqNumber);

                                    Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                                    if (Aoc.Is_GL_Action == true)
                                    {

                                        int WMode2 = 2; // DO TRANSACTION
                                        string WCallerProcess = "Reconciliation";
                                        Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey,
                                                                                     Aoc.ActionId, Aoc.Occurance, WCallerProcess, WMode2);
                                        TotalPairs = TotalPairs + 1;

                                    }
 
                                    // Update Mpa
                                    Mpa.ReadInPoolTransSpecificUniqueRecordId(Aoc.UniqueKey, 1);

                                    if (Mpa.RecordFound == true & Mpa.Matched == false & Aoc.Occurance == 1)
                                    // We say Matched == false in order to exclude all Presenter 
                                    {
                                        // 04 are the forced matched and 06 are the move to pool 
                                        // THESE WILL BE HANDLE LATER IN THE PROCESS
                                        // Update Reconciliation Record
                                        Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(Mpa.Operator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
                                        Rcs.SettledUnMatchedAmtWorkFlow = Rcs.SettledUnMatchedAmtWorkFlow + Mpa.TransAmount;
                                        //Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberSettledUnMatchedWorkFlow + 1;
                                        //Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions - 1;
                                        Rcs.UpdateReconcCategorySessionWithAuthorClosing(Mpa.RMCateg, Mpa.MatchingAtRMCycle);

                                        // Update ATMs Matched
                                        if (Mpa.Origin == "Our Atms")
                                        {
                                            Ratms.ReadReconcCategoriesATMsRMCycleSpecific(Mpa.Operator, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Mpa.TerminalId);
                                            if (Ratms.RecordFound == true)
                                            {
                                                Ratms.MatchedAmtAtWorkFlow = Ratms.MatchedAmtAtWorkFlow + Mpa.TransAmount;
                                                Ratms.UpdateReconcCategoriesATMsRMCycleForAtmForAdjusted(Mpa.TerminalId, Mpa.RMCateg, Mpa.MatchingAtRMCycle, Ratms.MatchedAmtAtDefault, Ratms.MatchedAmtAtWorkFlow);
                                            }
                                        }
                                    }
                                    Mpa.ActionType = Aoc.ActionId;
                                    Mpa.ActionByUser = true;
                                    Mpa.UserId = Ap.Requestor;
                                    Mpa.Authoriser = Ap.Authoriser;
                                    Mpa.AuthoriserDtTm = DateTime.Now;

                                    Mpa.SettledRecord = true;

                                    WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                                    Mpa.UpdateMatchingTxnsMasterPoolATMsForcedMatched(WOperator, WSelectionCriteria, 1);

                                    string WStage = "02"; // Confirmed by maker 
                                    Aoc.UpdateOccurancesStage("Master_Pool", Aoc.UniqueKey, WStage, DateTime.Now, WReconcCycleNo, Ap.Requestor);
                                    // AND MAKE "03"
                                    Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Aoc.UniqueKey, Ap.Authoriser, Ap.SeqNumber, Ap.Requestor);

                                    I = I + 1;
                                }

                               
                                //**************************************************************************
                                //**************************************************************************

                                //  UPDATE RECONCILIATION CLOSING DATE
                                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategory, WRMCycle);

                                Rcs.NumberSettledUnMatchedWorkFlow = Rcs.NumberOfUnMatchedRecs; // This is the case since we have reached here 
                                Rcs.RemainReconcExceptions = 0; // This is the case since we came here 
                               
                                Rcs.EndReconcDtTm = DateTime.Now;

                                Rcs.UpdateReconcCategorySessionWithAuthorClosing(WCategory, WRMCycle);
                                //
                                // Update authorisation record stage 
                                //
                                Ap.Stage = 5;
                                Ap.OpenRecord = false;

                                Ap.UpdateAuthorisationRecord(Ap.SeqNumber); 

                                // COMPLETE SCOPE
                                //
                                //this.Close();

                                // scope.Complete();

                                //return;

                            }

                        }
                        catch (Exception ex)
                        {
                           // conn2.Close();
                            string Error = ex.Message;
                            CatchDetails(ex);
                        }
                        //finally
                        //{
                        //    scope.Dispose();
                        //}

                        //MessageBox.Show("No transactions to be posted");
                        if (TotalPairs > 0)
                        {
                            Form2 MessageForm = new Form2("Reconciliation Workflow has finished for: " + WCategory + Environment.NewLine
                                                      + "Transactions to be posted were created as a result" + Environment.NewLine
                                                      + "Total number of created pair-transactions = " + TotalPairs.ToString() + Environment.NewLine
                                                      );
                            MessageForm.ShowDialog();
                        }
                        else
                        {
                            Form2 MessageForm = new Form2("Reconciliation Workflow has finished for: " + WCategory + Environment.NewLine
                                                     + "There aren't any transactions to be posted." + Environment.NewLine
                                                     );
                            MessageForm.ShowDialog();
                        }

                        this.Dispose();

                        // END for finish here


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

        void Step271a_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm271a)tableLayoutPanelMain.Controls["UCForm271a"]).guidanceMsg;
        }
        void Step271b_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm271b)tableLayoutPanelMain.Controls["UCForm271b"]).guidanceMsg;
        }

        void Step271c_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm271c)tableLayoutPanelMain.Controls["UCForm271c"]).guidanceMsg;
        }

        void Step271d_ChangeBoardMessage(object sender, EventArgs e)
        {

            //textBoxMsgBoard.Text = ((UCForm271d)tableLayoutPanelMain.Controls["UCForm271d"]).guidanceMsg;
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

            if (StepNumber == 3 & Step2Omitted == true)
            {
                StepNumber = 2;

                buttonNext.Text = "NEXT >";
            }


            //-----------------------------------------------------------

            if (StepNumber == 2 || (StepNumber == 3 & Step2Omitted == true))
            {
                if (It_Belongs_To_Groups == true)
                {
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    UCForm271a Step271a = new UCForm271a();
                    //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                    Step271a.Dock = DockStyle.Fill;
                    Step271a.UCForm271aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                    Step271a.SetScreen();
                    textBoxMsgBoard.Text = Step271a.guidanceMsg;

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
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    UCForm271a_JCC Step271a_JCC = new UCForm271a_JCC();
                    //Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step271a_JCC, 0, 0);
                    Step271a_JCC.Dock = DockStyle.Fill;
                    Step271a_JCC.UCForm271a_JCC_Par(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                    Step271a_JCC.SetScreen();
                    textBoxMsgBoard.Text = Step271a_JCC.guidanceMsg;

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
                


            }
            else
            {

                if (StepNumber == 3 & Step2Omitted == false)
                {
                    if (AudiType == true)
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm271b_FAB Step271b = new UCForm271b_FAB();
                        //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                        Step271b.Dock = DockStyle.Fill;
                        Step271b.UCForm271bPar_FAB(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, 1, "", 0);
                        Step271b.SetScreen();
                        textBoxMsgBoard.Text = Step271b.guidanceMsg;

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
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm271b Step271b = new UCForm271b();
                        //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                        Step271b.Dock = DockStyle.Fill;
                        Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, 1, "", 0);
                        Step271b.SetScreen();
                        textBoxMsgBoard.Text = Step271b.guidanceMsg;

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
                }
                else
                {

                }


            }
        }

        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append(WindowsIdentity.GetCurrent().Name);
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
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

        //public bool BDC231_Not_Normal1 { get => BDC231_Normal; set => BDC231_Normal = value; }

        // Cancel
        private void Cancel_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

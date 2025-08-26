using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;
using System.Data;

namespace RRDM4ATMsWin
{
    public partial class Form51_Recycle : Form
    {
        //Bitmap SCREENa;
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

        int WRMCycle;

        string ParId;
        string OccurId;

        int StepNumber ;

        bool Deposits;

        bool Exceptions;

        string PRX;

        bool Recycling;

        bool OnlyLoading;

        bool ViewWorkFlow;

        bool Based_On_IST;

        bool ReplenishmentAuthorNeeded;

        bool ReplenishmentAuthorNoRecordYet;
        bool ReplenishmentAuthorOutstanding;
        bool ReplenishmentAuthorDone;

        int WAtmsReconcGroup;

        string WReconcCategoryId;

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

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

        public Form51_Recycle(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
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
            labelReplDate.Text = DateTime.Now.ToShortDateString();

            StepNumber = 1;

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }


            // STEPLEVEL

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.StepLevel = 0;
            Usi.ReplStep1_Updated = false;
            Usi.ReplStep2_Updated = false;
            Usi.ReplStep3_Updated = false;
            Usi.ReplStep4_Updated = false;
            Usi.ReplStep5_Updated = false;
            Usi.ReplStep6_Updated = false;

            //  Usi.WFieldNumeric1 = 0;
            //  Usi.WFieldNumeric11 = 0;
            Usi.WFieldNumeric12 = 0;

            //cmd.Parameters.AddWithValue("@WFieldChar1", WFieldChar1);
            //cmd.Parameters.AddWithValue("@WFieldChar2", WFieldChar2);

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //************************************************************
            // Check if it should be based on IST
            ParId = "944";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES") Based_On_IST = true;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = "ATMs";
            WRMCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

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

            bool Reject = false;
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            // Check for DEPOSITS function for this ATM
            SM.Read_SM_AND_FillTable_Deposits(WAtmNo, WSesNo);

            if (SM.RecordFound == true)
            {
                Deposits = true;
            }
            else
            {
                Deposits = false;
            }

            Ac.ReadAtm(WAtmNo);

            if (Ac.DepoRecycling == true) // Deposit Recycling
            {
                Recycling = true;
            }
            else
            {
                Recycling = false;
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            if (Ta.RecordFound == false)
            {
                MessageBox.Show("Replenishment Cycle Doesnt exist");
                return; 
            }

            // Get the date 
            labelCycleStart.Text = Ta.SesDtTimeStart.ToShortDateString();
            labelReplDate.Text = Ta.SesDtTimeEnd.ToShortDateString();

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int Mode = 1;
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            if (Mpa.TotalSelected > 0)
            {
                Exceptions = true;
            }
            else
            {
                Exceptions = false;
            }


            WAtmsReconcGroup = Ac.AtmsReconcGroup;

            RRDMReconcCategories Rc = new RRDMReconcCategories();

            Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

            WReconcCategoryId = Rc.CategoryId;


            if (Usi.ProcessNo == 8 || Usi.ProcessNo == 9)  // Follow Showing Money in 
            {
                //Start with STEP 1
                UCForm51d Step51d = new UCForm51d();
                // Stavros
                Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                Step51d.Dock = DockStyle.Fill;
                Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                textBoxMsgBoard.Text = Step51d.guidanceMsg;

                labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep5.ForeColor = Color.White;
                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                if (Usi.ProcessNo == 9)
                {
                    labelStep5.Text = "In Money are shown for action";
                }

                buttonNext.Visible = true;

                buttonNext.Text = "Finish";

                // SHOW labelStep4 and hide the rest. 
                // Hide the rest

                labelStep1.Hide();
                labelStep2.Hide();
                labelStep3.Hide();
                labelStep6.Hide();

                label5.Hide();
                label4.Hide();
                label9.Hide();
                label11.Hide();
            }

            // View only 
            // View only 
            if (WViewFunction == true)  // Viewing only ,,, Repl Cycle workflow had finished and we want t
            {
                labelCycleStart.Text = Ta.SesDtTimeEnd.ToShortDateString();

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

            if (Usi.ProcessNo == 5 || Usi.ProcessNo == 31 || Usi.ProcessNo == 26) Usi.ProcessNo = 1;

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

                //Start with STEP 1
                UCForm51a_Recycle Step51a = new UCForm51a_Recycle();
                // Stavros
                Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                Step51a.Dock = DockStyle.Fill;
                Step51a.UCForm51a_Recycle_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51a.SetScreen();
                textBoxMsgBoard.Text = Step51a.guidanceMsg;

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
            //
            // Case For money in
            //
            if ((Usi.ProcessNo == 8 || Usi.ProcessNo == 9) & buttonNext.Text == "Finish")
            {
                this.Dispose();
            }

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

            if (Na.Balances1.OpenBal == 0)
            {
                OnlyLoading = true;
            }
            else
            {
                OnlyLoading = false;
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 1 || ViewWorkFlow == true)
            {
                // Continue
            }
            else
            {
                return;
            }

            if (StepNumber == 1 & OnlyLoading == true & (Usi.ReplStep1_Updated == true
                                              || ViewWorkFlow == true))
            {
                StepNumber = 2; //  step

                labelStep1.ForeColor = labelStep3.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep2_Updated = true;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }

            if (StepNumber == 2 & Deposits == false)
            {

                StepNumber = 3; // 

                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep3_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }
            if (StepNumber == 3 & Exceptions == false)
            {
                // Move one step
                StepNumber = 4; // 
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep4_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }

            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                case 1: // From physical check
                    {

                        // STEPLEVEL// Check Feasibility to move to next step 

                        if (Usi.ReplStep1_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                       // GetMainBodyImage("UCForm51a");

                        UCForm51b Step51b = new UCForm51b();

                        //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        Step51b.ChangeBoardMessage += Step51b_ChangeBoardMessage;

                        Step51b.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        Step51b.SetScreen();
                        textBoxMsgBoard.Text = Step51b.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        if (Usi.ReplStep2_Updated == true & ViewWorkFlow == false)
                        {
                            textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                        }

                        tableLayoutPanelMain.Controls.Add(Step51b, 0, 0);

                        labelStep1.ForeColor = labelStep3.ForeColor;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber++; // STEP becomes two
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;
                    }

                case 2: // From Money in
                    {
                        // THIS IS DEPOSITS
                        if (Usi.ReplStep2_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                        //// STEPLEVEL// Check Feasibility to move to next step 
                        //GetMainBodyImage("UCForm51b");
                        //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        //UCForm51c Step51c = new UCForm51c();
                        ////   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        //Step51c.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        //tableLayoutPanelMain.Controls.Add(Step51c, 0, 0);
                        //Step51c.Dock = DockStyle.Fill;
                        //Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        //textBoxMsgBoard.Text = Step51c.guidanceMsg;

                        //if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        //if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        //if (Usi.ReplStep3_Updated == true & ViewWorkFlow == false)
                        //{
                        //    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                        //}

                        // THIS GETS DEPOSITS FROM 
                        //
                        GetMainBodyImage("UCForm51b");
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm51c_SM_Recycle Step51c_SM = new UCForm51c_SM_Recycle();
                        
                        Step51c_SM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51c_SM, 0, 0);
                        Step51c_SM.Dock = DockStyle.Fill;
                        Step51c_SM.UCForm51c_Recycle_Par_SM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51c_SM.guidanceMsg;

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
                case 3: // From Deposits or Money in if there are exceptions
                    {
                        // CALL SAME AS IN Form271b
                        // YOU ARE At 3 and you move to handle Exceptions
                        // MOVE To Step 4
                       // Coming from money in 
                        if ((Usi.ReplStep2_Updated == false & Deposits == false) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }
                        // Coming from deposits
                        if ((Usi.ReplStep3_Updated == false & Deposits == true ) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }
                        if (Exceptions == true) // GO TOWARDS 271b
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            UCForm271b Step271b = new UCForm271b();
                            //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                            tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                            Step271b.Dock = DockStyle.Fill;
                            string WMatchingCateg = PRX + "20X";
                            string RMCategory = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                            Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, RMCategory, WRMCycle, 2, WAtmNo, WSesNo);
                            Step271b.SetScreen();
                            textBoxMsgBoard.Text = Step271b.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                            if (Usi.ReplStep3_Updated == true & ViewWorkFlow == false)
                            {
                                textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                            }
                            labelStep2.ForeColor = labelStep1.ForeColor;
                            labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep3.ForeColor = labelStep3.ForeColor;
                            labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                            labelStep4.ForeColor = Color.White;
                            labelStep4.Font = new Font(labelStep4.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber++;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;

                        }
                        else
                        {
                            // SPECIALISED SCREEN FOR DEPOSITS 
                            // STEPLEVEL// Check Feasibility to move to next step 
                            if ((Usi.ReplStep3_Updated == false & Deposits == true) & ViewWorkFlow == false)
                            {
                                // There are deposits and no updating 
                                MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                                return;
                            }
                            if ((Usi.ReplStep2_Updated == false & Deposits == false) & ViewWorkFlow == false)
                            {
                                // There are deposits and no updating 
                                MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                                return;
                            }

                            // THERE ARE DEPOSITS
                            if (Deposits == true)
                            {
                                GetMainBodyImage("UCForm51c_SM_Recycle");
                                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                            }

                            // THERE ARE no DEPOSITS
                            if (Deposits == false & Na.Balances1.OpenBal > 0)
                            {
                                GetMainBodyImage("UCForm51b");
                                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                            }

                            // STEPLEVEL// Check Feasibility to move to next step 
                            //GetMainBodyImage("UCForm51b");
                            //  tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            UCForm51c_CDM Step51c_CDM = new UCForm51c_CDM();
                           
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
                            labelStep3.ForeColor = labelStep3.ForeColor;
                            labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                            labelStep4.ForeColor = Color.White;
                            labelStep4.Font = new Font(labelStep4.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber++;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;

                        }

                        break;

                    }
                case 4: // From Deposits or from Cash 
                    {
                        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                        // Matching is done but not Settled 
                        int Mode = 4; // See if actions not completed before you move to the nexr 
                        Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                                       Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

                        //string SelectionCriteria = " WHERE  "
                        //               + " TerminalId ='" + WAtmNo + "' "
                        //               + "  AND [ReplCycleNo] = " + WSesNo
                        //               + " AND (MetaExceptionId = 55 "
                        //              // + " OR MetaExceptionId = 225 OR MetaExceptionId = 226 "
                        //               + " OR (Matched = 0 AND ActionType = '08')) " // Move from reconciliation
                        //               + "  AND (ActionType = '00' OR ActionType = '08') "
                        //               ;


                        //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                        if (Mpa.RecordFound == true)
                        {
                            MessageBox.Show("Take Actions For All Before You Move To Next Step!");
                            return;
                        }

                        if (Usi.ReplStep2_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }
                        if (Usi.ReplStep3_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        //  CASH MANAGEMENT External?? 
                        ParId = "202";
                        OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        RRDMCashManagement = Gp.OccuranceNm;
                        //  CASH MANAGEMENT Prior Replenishment Workflow  
                        ParId = "211";
                        OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THROUGHT AN ACTION 

                        if ((Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen != "YES") & Ac.TypeOfRepl != "50"))
                        {
                            // RRDM Cash Management is needed  
                            {
                                // Cash Management is needed 
                                UCForm51d Step51d = new UCForm51d();

                                Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;

                                //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                                Step51d.Dock = DockStyle.Fill;
                                Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                textBoxMsgBoard.Text = Step51d.guidanceMsg;

                                if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                {
                                    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                }

                                labelStep3.ForeColor = labelStep1.ForeColor;
                                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep4.Font.Style);

                                labelStep4.ForeColor = labelStep1.ForeColor;
                                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                                labelStep5.ForeColor = Color.White;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                                StepNumber++;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;
                            }
                        }
                        else
                        {
                            if (RRDMCashManagement == "NO")
                            {
                                if (WOperator == "AUDBEGCA")
                                {
                                    //
                                    // 
                                    //
                                    UCForm51d2_AUD Step51d2_AUD = new UCForm51d2_AUD();

                                    Step51d2_AUD.ChangeBoardMessage += Step51d2_AUD_ChangeBoardMessage;

                                    //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                    tableLayoutPanelMain.Controls.Add(Step51d2_AUD, 0, 0);
                                    Step51d2_AUD.Dock = DockStyle.Fill;
                                    Step51d2_AUD.UCForm51d2Par_AUD(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                    textBoxMsgBoard.Text = Step51d2_AUD.guidanceMsg;

                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                    if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                    {
                                        textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                    }

                                    labelStep2.ForeColor = labelStep1.ForeColor;
                                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                                    labelStep3.ForeColor = labelStep1.ForeColor;
                                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep4.Font.Style);

                                    labelStep4.ForeColor = labelStep1.ForeColor;
                                    labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                                    labelStep5.ForeColor = Color.White;
                                    labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                                    StepNumber++;
                                    buttonBack.Visible = true;
                                    buttonNext.Visible = true;
                                }
                                else
                                {
                                    //
                                    // NO CASH MANAGEMENT 
                                    //
                                    UCForm51d2_BDC Step51d2_BDC = new UCForm51d2_BDC();

                                    Step51d2_BDC.ChangeBoardMessage += Step51d2_BDC_ChangeBoardMessage;

                                    //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                    tableLayoutPanelMain.Controls.Add(Step51d2_BDC, 0, 0);
                                    Step51d2_BDC.Dock = DockStyle.Fill;
                                    Step51d2_BDC.UCForm51d2Par_BDC(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                    textBoxMsgBoard.Text = Step51d2_BDC.guidanceMsg;

                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                    if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                    {
                                        textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                    }

                                    labelStep2.ForeColor = labelStep1.ForeColor;
                                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                                    labelStep3.ForeColor = labelStep1.ForeColor;
                                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep4.Font.Style);

                                    labelStep4.ForeColor = labelStep1.ForeColor;
                                    labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                                    labelStep5.ForeColor = Color.White;
                                    labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                                    StepNumber++;
                                    buttonBack.Visible = true;
                                    buttonNext.Visible = true;
                                }
                                
                            }
                            else
                            {
                                // RRDM Cash Management is NOT needed  
                                // Cash management is done by other Package OR Replenishment is for preset Max. 
                                // CIT != 1000 .... Order was created 
                                // Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl == "50" Order Was created and email was sent 

                                UCForm51d2 Step51d2 = new UCForm51d2();

                                Step51d2.ChangeBoardMessage += Step51d2_ChangeBoardMessage;

                                //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                tableLayoutPanelMain.Controls.Add(Step51d2, 0, 0);
                                Step51d2.Dock = DockStyle.Fill;
                                Step51d2.UCForm51d2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                textBoxMsgBoard.Text = Step51d2.guidanceMsg;

                                if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                {
                                    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                }

                                labelStep3.ForeColor = labelStep1.ForeColor;
                                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep4.Font.Style);

                                labelStep4.ForeColor = labelStep1.ForeColor;
                                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                                labelStep5.ForeColor = Color.White;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                                StepNumber++;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;
                            }


                        } // else here

                        break;
                    }
                case 5: // From Money in 
                    {

                        if (Usi.ReplStep5_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }
                        // CASH MANAGEMENT
                        string ParId = "202";
                        string OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                        // RRDM Cash Management is used                                         
                        if ((Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen != "YES") & Ac.TypeOfRepl != "50"))
                        {

                            GetMainBodyImage("UCForm51d");
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        }
                        else // RRDM Cash Management is NOT Used 
                        {
                            if (RRDMCashManagement == "NO")
                            {
                                if (WOperator == "AUDBEGCA")
                                {
                                    GetMainBodyImage("UCForm51d2_AUD");
                                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                                }
                                else
                                {
                                    GetMainBodyImage("UCForm51d2_BDC");
                                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                                }
                                   
                            }
                            else
                            {
                                GetMainBodyImage("UCForm51d2");
                                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                            }

                        }

                        UCForm51e_Recycle Step51e = new UCForm51e_Recycle();
                        //Step51e.ChangeBoardMessage += Step51e_ChangeBoardMessage;

                        tableLayoutPanelMain.Controls.Add(Step51e, 0, 0);
                        Step51e.Dock = DockStyle.Fill;
                        Step51e.UCForm51ePar_Recycle(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51e.guidanceMsg;

                        labelStep5.ForeColor = labelStep3.ForeColor;
                        labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                        labelStep6.ForeColor = Color.White;
                        labelStep6.Font = new Font(labelStep6.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;


                        break;

                    }

                case 6: // From Summary
                    {
                        // last step - Make validationsfor Authorisations  

                        UCForm51f Step51f = new UCForm51f();
                        Step51f.ChangeBoardMessage += Step51f_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51f, 0, 0);
                        Step51f.Dock = DockStyle.Fill;
                        Step51f.UCForm51fPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51f.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                        labelStep6.ForeColor = labelStep5.ForeColor;
                        labelStep6.Font = new Font(labelStep7.Font.FontFamily, 10, labelStep6.Font.Style);
                        labelStep7.ForeColor = Color.White;
                        labelStep7.Font = new Font(labelStep7.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Finish";

                        break;

                    }

                case 7: // From Authorisation
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
                                if (Ap.Stage == 4)
                                {

                                    ReplenishmentAuthorDone = true;
                                }
                                else
                                {
                                    if (Ap.Stage == 3 & WAuthoriser == true) // leave the 3 here too 
                                    {
                                        ReplenishmentAuthorDone = true;
                                    }
                                    else
                                    {
                                        ReplenishmentAuthorOutstanding = true;
                                    }

                                }
                                if (Ap.AuthDecision == "NO" & ReplenishmentAuthorDone == true)
                                {
                                    this.Close();
                                    return;
                                }
                                // Authorisation Record Exist 
                                if (WRequestor == true & Ap.AuthDecision == "YES"
                                    || WRequestor == true & Ap.AuthDecision == "" // No action by authoriser yet
                                    ) // Coming from requestor 
                                {
                                    this.Close();
                                    return;
                                }
                            }
                            else
                            {
                                ReplenishmentAuthorNoRecordYet = true;
                            }


                            //if (WAuthoriser == true & ReplenishmentAuthorDone == true) // Coming from authoriser and authoriser done  
                            //{
                            //    this.Close();
                            //    return;
                            //}

                            if (WRequestor == true & ReplenishmentAuthorDone == false) // Coming from requestor and authoriser not done 
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

                            if (WAuthoriser == true & ReplenishmentAuthorDone == true & (Ap.Stage == 3 || Ap.Stage == 4)
                              & Ap.AuthDecision == "YES") // Everything is fine .
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


                        // create a connection object
                        //   using (var scope = new System.Transactions.TransactionScope())
                        try
                        {
                            string WStage = "";
                            if (Ap.RecordFound == true & (Ap.Stage == 3 || Ap.Stage == 4))
                            {
                                // Update stage 
                                //
                                Ap.Stage = 5;
                                Ap.OpenRecord = false;

                                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                                //
                                // Update HERE so you will have authoriser during Transaction creation
                                //
                                Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, Ap.Requestor,
                                   Ap.Authoriser, WReconcCategoryId);
                            }

                            //GetMainBodyImage("UCForm51e");
                            //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            //if ((Ac.CitId == "1000" & AutherPresent & Ac.TypeOfRepl != "50"))
                            if (AutherPresent)
                            {
                                //
                                // We update record due to unique key 
                                //


                                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                                string WJobCategory = "ATMs";
                                int WReconcCycleNo;

                                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();


                                // Create Table of Txns 
                                // READ ALL IN THIS CYCLE
                                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                                string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                                                      + "' AND ReplCycle =" + WSesNo + " AND Maker ='"
                                                      + Ap.Requestor + "' AND Stage In ('01','02')";

                                Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

                                Aoc.ClearTableTxnsTableFromAction();

                                int I = 0;

                                while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                                {

                                    int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                                    // Update authoriser 
                                    // Update authoriser 
                                    Aoc.UpdateOccurancesForAuthoriser_2(WSeqNo, Ap.Authoriser, Ap.SeqNumber);

                                    Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                                    // Create Txn In Pool - Trans to be updated 
                                    //
                                    if (Aoc.Is_GL_Action == true)
                                    {
                                        // Create transactions for Repl
                                        int WMode2 = 2; // 
                                        string WCallerProcess = "Replenishment";
                                        Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId
                                                                                     , Aoc.Occurance
                                                                                     , WCallerProcess, WMode2);
                                    }

                                    //if (Aoc.ActionId == "89" || Aoc.ActionId == "90")
                                    //{
                                    //    WStage = "02"; // Confirmed by maker 
                                    //    Aoc.UpdateOccurancesStage("Replenishment", Mpa.ReplCycleNo, WStage, DateTime.Now, WReconcCycleNo, WSignedId);
                                    //    // Also Authoriser 
                                    //    // Also Make Stage = "03"
                                    //    Aoc.UpdateOccurancesForAuthoriser("Replenishment", Mpa.ReplCycleNo, Ap.Authoriser, Ap.SeqNumber, WSignedId);
                                    //}

                                    if (Aoc.UniqueKeyOrigin == "Master_Pool")
                                    {
                                        // THIS IS FOR PRESENTER ERROR
                                        WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                                        Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                                        Mpa.ActionByUser = true;
                                        Mpa.UserId = Ap.Requestor;
                                        Mpa.Authoriser = Ap.Authoriser;
                                        Mpa.AuthoriserDtTm = DateTime.Now;

                                        Mpa.SettledRecord = true;

                                        WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                                        Mpa.UpdateMatchingTxnsMasterPoolATMsForcedMatched(WOperator, WSelectionCriteria, 1);

                                        WStage = "02"; // Confirmed by maker 
                                        Aoc.UpdateOccurancesStage("Master_Pool", Aoc.UniqueKey, WStage, DateTime.Now, WReconcCycleNo, Ap.Requestor);
                                        // Authoriser 
                                        // Also Make stage = "03"
                                        Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Aoc.UniqueKey, Ap.Authoriser, Ap.SeqNumber, Ap.Requestor);

                                    }

                                    //********************************************
                                    // HERE WE CREATE THE ENTRIES AS PER BDC NEEDS
                                    //********************************************

                                    I = I + 1;
                                }

                                DataTable TempTxnsTableFromAction;
                                TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

                                // UPDATE OCCURANCES FOR REPLENISHMENT RECORDS AS CONFIRMED
                                // For done within this cycle 

                                WStage = "02"; // Confirmed by maker 
                                Aoc.UpdateOccurancesStage("Replenishment", WSesNo, WStage, DateTime.Now, WReconcCycleNo, Ap.Requestor);
                                // Also Authoriser 
                                // Also Update Stage as "03"
                                Aoc.UpdateOccurancesForAuthoriser("Replenishment", WSesNo, Ap.Authoriser, Ap.SeqNumber, Ap.Requestor);

                                // UPDATE Ta
                                Aoc.ReadActionsOccurancesTo_RichPicture_One_ATM(WAtmNo, WSesNo);

                                if (Aoc.Current_ShortageBalance < 0
                                    || Aoc.WaitForDisputeNo < Aoc.WaitAndSettledDisputeNo
                                    || Aoc.NoWaitDisputeNo < Aoc.NoWaitSettledDisputeNo)
                                {
                                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                    Ta.Repl1.DiffRepl = true;
                                    Ta.ReconcAtRMCycle = WRMCycle;
                                    Ta.Recon1.RecFinDtTm = DateTime.Now;
                                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                                }
                                else
                                {
                                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                    Ta.Repl1.DiffRepl = false;
                                    Ta.ReconcAtRMCycle = WRMCycle;
                                    Ta.Recon1.RecFinDtTm = DateTime.Now;
                                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                                }

                            }



                            // UPDATE TRACES WITH FINISH 
                            // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                            int Mode = 1; // Before reconciliation 
                            if (WOperator == "CRBAGRAA")
                            {
                                //Ta.UpdateTracesFinishReplOrReconc(WAtmNo, WSesNo, WSignedId, Mode);
                                Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, WSignedId,
                                    Ap.Authoriser, WReconcCategoryId);
                            }
                            else
                            {

                            }



                            // READ Ta to see if differences 

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                            string SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
                            Pi.ReadPhysicalInspectionRecordsToSeeIfAlert(SelectionCriteria);
                            //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);



                            if (WOperator == "AUDBEGCA")
                            {
                                // UPDATE 
                                RRDMSessionsDataCombined Sc = new RRDMSessionsDataCombined();

                                string WSelectionCriteria = " WHERE AtmNo='" + WAtmNo + "' "
                                                            + " AND SesNo=" + WSesNo;
                                Sc.ReadSessionsDataCombinedBySelectionCriteria(WSelectionCriteria);

                                Sc.ProcessMode = Ta.ProcessMode;
                                //
                                // UPDATE THE RECORD based on the sequence number 
                                //
                                Sc.Update_SessionsDataCombined(Sc.SeqNo);
                            }

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

                            }

                            if (Ta.Recon1.DiffReconcEnd == true & Ta.Is_Updated_GL == false)
                            {
                                // There differences to reconcile but host not available

                                ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
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

                        if (ReplenishmentAuthorNeeded == false) // No authorisation 
                        {
                            if (Deposits == true)
                            {
                                // PRINT ALL IMAGES INCLUDING DEPOSITS 
                                //Form56 Report1 = new Form56(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo,
                                //                SCREENa, SCREENb, SCREENc, SCREENd, "", Na.ReplUserComment, Ta.ReplGenComment, ReconcComment);
                                //Report1.Show();
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

                        this.Dispose();

                        // scope.Complete();

                        // return;

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
            textBoxMsgBoard.Text = ((UCForm51a_Recycle)tableLayoutPanelMain.Controls["UCForm51a_Recycle"]).guidanceMsg;
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

            //  bool Comesfrom4 = false;

            //if (StepNumber == 2 & Deposits == false)
            //{

            //    StepNumber = 3; // 

            //    Usi.ReadSignedActivityByKey(WSignRecordNo);
            //    Usi.ReplStep3_Updated = true;
            //    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //}
            //if (StepNumber == 3 & Exceptions == false)
            //{
            //    // Move one step
            //    StepNumber = 4; // 
            //    Usi.ReadSignedActivityByKey(WSignRecordNo);
            //    Usi.ReplStep4_Updated = true;
            //    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //}

            if (StepNumber == 5 & Exceptions == false)
            {
                StepNumber = 4;
            }
            if (StepNumber == 4 & Deposits == false)
            {
                StepNumber = 3;
            }
            if (StepNumber == 3 & OnlyLoading == true)
            {
                StepNumber = 2;
            }

            //if (StepNumber == 4)
            //{
            //    if (Deposits == false)
            //    {
            //        StepNumber = 3; // decrease by ONE ! 
            //        Comesfrom4 = true;

            //    }
            //    if (OnlyLoading == true)
            //    {
            //        if (Deposits == false) StepNumber = 2; // decrease by Two 
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
    
                        UCForm51a_Recycle Step51a = new UCForm51a_Recycle();
                        Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                        Step51a.Dock = DockStyle.Fill;
                        Step51a.UCForm51a_Recycle_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        Step51a.SetScreen();
                        textBoxMsgBoard.Text = Step51a.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                        labelStep5.ForeColor = labelStep6.ForeColor;
                        labelStep5.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep4.ForeColor = labelStep6.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep3.ForeColor = labelStep6.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep2.ForeColor = labelStep6.ForeColor;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep1.ForeColor = Color.White;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = false;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Next >";

                        break;
                    }
                case 3:
                    {
                        UCForm51b Step51b = new UCForm51b();
                        Step51b.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51b, 0, 0);
                        Step51b.Dock = DockStyle.Fill;
                        Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51b.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep5.ForeColor = labelStep6.ForeColor;
                        labelStep5.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep4.ForeColor = labelStep6.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep3.ForeColor = labelStep6.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 18, FontStyle.Bold);


                        StepNumber--;

                        buttonBack.Visible = true;
                        buttonNext.Visible = true;
                        buttonNext.Text = "Next >";
                        break;
                    }
                case 4:
                    {
                        //UCForm51c Step51c = new UCForm51c();
                        //Step51c.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        //tableLayoutPanelMain.Controls.Add(Step51c, 0, 0);
                        //Step51c.Dock = DockStyle.Fill;
                        //Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        //textBoxMsgBoard.Text = Step51c.guidanceMsg;

                        UCForm51c_SM_Recycle Step51c_SM = new UCForm51c_SM_Recycle();
                        Step51c_SM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51c_SM, 0, 0);
                        Step51c_SM.Dock = DockStyle.Fill;
                        Step51c_SM.UCForm51c_Recycle_Par_SM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51c_SM.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep4.ForeColor = labelStep6.ForeColor;
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
                        if (Based_On_IST == true)
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            UCForm271b Step271b = new UCForm271b();
                            //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                            tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                            Step271b.Dock = DockStyle.Fill;
                            string WMatchingCateg = PRX + "20X";
                            string RMCategory = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                            Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, RMCategory, WRMCycle, 2, WAtmNo, WSesNo);
                            Step271b.SetScreen();
                            textBoxMsgBoard.Text = Step271b.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                            labelStep5.ForeColor = labelStep6.ForeColor;
                            labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                            labelStep4.ForeColor = Color.White;
                            labelStep4.Font = new Font(labelStep4.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber--;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;
                        }
                        else
                        {
                            UCForm51c_CDM Step51c_CDM = new UCForm51c_CDM();
                            Step51c_CDM.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                            tableLayoutPanelMain.Controls.Add(Step51c_CDM, 0, 0);
                            Step51c_CDM.Dock = DockStyle.Fill;
                            Step51c_CDM.UCForm51c_CDM_Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            Step51c_CDM.SetScreen();
                            textBoxMsgBoard.Text = Step51c_CDM.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                            labelStep5.ForeColor = labelStep6.ForeColor;
                            labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                            labelStep4.ForeColor = Color.White;
                            labelStep4.Font = new Font(labelStep4.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber--;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;
                        }



                        break;
                    }
                case 6:
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        ////  CASH MANAGEMENT?? 

                        //  CASH MANAGEMENT External?? 
                        Ac.ReadAtm(WAtmNo);
                        string ParId = "202";
                        string OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        RRDMCashManagement = Gp.OccuranceNm;
                        //  CASH MANAGEMENT Prior Replenishment Workflow  
                        ParId = "211";
                        OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THROUGHT AN ACTION 

                        if ((Ac.CitId == "1000" & (RRDMCashManagement == "YES" & CashEstPriorReplen != "YES") & Ac.TypeOfRepl != "50"))
                        {
                            // RRDM Cash Management is needed  
                            UCForm51d Step51d = new UCForm51d();
                            Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;

                            tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                            Step51d.Dock = DockStyle.Fill;
                            Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                            textBoxMsgBoard.Text = Step51d.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                            labelStep6.ForeColor = labelStep5.ForeColor;
                            labelStep6.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                            labelStep5.ForeColor = Color.White;
                            labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber--;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;

                            buttonNext.Text = "Next >";
                        }
                        else
                        {
                            if (RRDMCashManagement == "NO")
                            {
                                //
                                // NO CASH MANAGEMENT 
                                //
                                if (WOperator == "AUDBEGCA")
                                {
                                    //
                                    // NO CASH MANAGEMENT 
                                    //
                                    UCForm51d2_AUD Step51d2_AUD = new UCForm51d2_AUD();

                                    Step51d2_AUD.ChangeBoardMessage += Step51d2_AUD_ChangeBoardMessage;

                                    //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                    tableLayoutPanelMain.Controls.Add(Step51d2_AUD, 0, 0);
                                    Step51d2_AUD.Dock = DockStyle.Fill;
                                    Step51d2_AUD.UCForm51d2Par_AUD(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                    textBoxMsgBoard.Text = Step51d2_AUD.guidanceMsg;

                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                    //if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                    //{
                                    //    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                    //}
                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                                    labelStep6.ForeColor = labelStep5.ForeColor;
                                    labelStep6.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                                    labelStep5.ForeColor = Color.White;
                                    labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);


                                    StepNumber--;

                                    buttonBack.Visible = true;
                                    buttonNext.Visible = true;

                                    buttonNext.Text = "Next >";
                                }
                                else
                                {
                                    UCForm51d2_BDC Step51d2_BDC = new UCForm51d2_BDC();

                                    Step51d2_BDC.ChangeBoardMessage += Step51d2_BDC_ChangeBoardMessage;

                                    //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                    tableLayoutPanelMain.Controls.Add(Step51d2_BDC, 0, 0);
                                    Step51d2_BDC.Dock = DockStyle.Fill;
                                    Step51d2_BDC.UCForm51d2Par_BDC(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                    textBoxMsgBoard.Text = Step51d2_BDC.guidanceMsg;

                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                                    //if (Usi.ReplStep4_Updated == true & ViewWorkFlow == false)
                                    //{
                                    //    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                                    //}
                                    if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                                    labelStep6.ForeColor = labelStep5.ForeColor;
                                    labelStep6.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                                    labelStep5.ForeColor = Color.White;
                                    labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);


                                    StepNumber--;

                                    buttonBack.Visible = true;
                                    buttonNext.Visible = true;

                                    buttonNext.Text = "Next >";
                                }

                               
                            }
                            else
                            {
                                // RRDM Cash Management is NOT needed  
                                // Cash management is done by other Package OR Replenishment is for preset Max. 
                                // CIT != 1000 .... Order was created 
                                // Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl == "50" Order Was created and email was sent 

                                UCForm51d2 Step51d2 = new UCForm51d2();

                                Step51d2.ChangeBoardMessage += Step51d2_ChangeBoardMessage;
                                tableLayoutPanelMain.Controls.Add(Step51d2, 0, 0);
                                Step51d2.Dock = DockStyle.Fill;
                                Step51d2.UCForm51d2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                textBoxMsgBoard.Text = Step51d2.guidanceMsg;

                                if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                labelStep6.ForeColor = labelStep5.ForeColor;
                                labelStep6.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                                labelStep5.ForeColor = Color.White;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 18, FontStyle.Bold);

                                //labelStep2.ForeColor = labelStep1.ForeColor;
                                //labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                                //labelStep3.ForeColor = labelStep1.ForeColor;
                                //labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                                //labelStep5.ForeColor = labelStep4.ForeColor;
                                //labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                                //labelStep4.ForeColor = Color.White;
                                //labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);


                                StepNumber--;

                                buttonBack.Visible = true;
                                buttonNext.Visible = true;

                                buttonNext.Text = "Next >";
                            }

                        }

                        break;

                    }
                case 7:
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        // RRDM ssummary
                        UCForm51e_Recycle Step51e = new UCForm51e_Recycle();
                        //Step51e.ChangeBoardMessage += Step51e_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51e, 0, 0);
                        Step51e.Dock = DockStyle.Fill;
                        Step51e.UCForm51ePar_Recycle(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51e.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep7.ForeColor = labelStep6.ForeColor;
                        labelStep7.Font = new Font(labelStep7.Font.FontFamily, 10, labelStep7.Font.Style);
                        labelStep6.ForeColor = Color.White;
                        labelStep6.Font = new Font(labelStep6.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Next";

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
        void Step51d2_BDC_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).guidanceMsg;
        }

        void Step51d2_AUD_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).guidanceMsg;
        }

        // void Step51d2_ChangeBoardMessage(object sender, EventArgs e)
        //  {
        //     textBoxMsgBoard.Text = ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).guidanceMsg;
        // }

        void Step51c_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51c_SM_Recycle)tableLayoutPanelMain.Controls["UCForm51c_SM_Recycle"]).guidanceMsg;
        }

        void Step51b_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).guidanceMsg;
        }


        private void GetMainBodyImage(String ControlName)
        {
            System.Drawing.Bitmap memoryImage;

            if (ControlName.Equals("UCForm51a"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).Width, ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).Height);
                //((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).DrawToBitmap(memoryImage, ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).ClientRectangle);
                //SCREENa = memoryImage;
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
            if (ControlName.Equals("UCForm51d2_BDC"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).Width, ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).Height);
                ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).DrawToBitmap(memoryImage, ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).ClientRectangle);
                SCREENd = memoryImage;
            }
            if (ControlName.Equals("UCForm51d2_AUD"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).Width, ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).Height);
                ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).DrawToBitmap(memoryImage, ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).ClientRectangle);
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

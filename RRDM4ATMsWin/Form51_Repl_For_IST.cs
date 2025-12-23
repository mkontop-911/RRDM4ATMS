using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Text;
using System.Data;

namespace RRDM4ATMsWin
{
    public partial class Form51_Repl_For_IST : Form
    {
        //Bitmap SCREENa;
        //Bitmap SCREENb;
        //Bitmap SCREENc;
        //Bitmap SCREENd;
        //Bitmap SCREENe;

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

        bool FOREX_Deposits;

        bool Exceptions;

        //string PRX;

        //bool ToxicJournal;
       // bool MissingJournal;

        //bool Recycling;

       // bool OnlyLoading;

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

        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        string W_CIT_Excel_STATUS;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
        public bool W_IsFromExcel; 

        public Form51_Repl_For_IST(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo, bool IsFromExcel)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            W_IsFromExcel = IsFromExcel; 

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserOperator = Us.Operator;
            // ========================================================
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelATMno.Text = WAtmNo;
            labelSessionNo.Text = WSesNo.ToString();
            //labelReplDate.Text = DateTime.Now.ToShortDateString();

            //this.WindowState = FormWindowState.Maximized;

            if (W_IsFromExcel == true)
            {
                string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo;
                Ce.Read_CIT_Excel_Table_BySelectionCriteria(WSelectionCriteria);

                W_CIT_Excel_STATUS = Ce.STATUS;
            }

            StepNumber = 1;

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
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            if (Ta.RecordFound == false)
            {
                MessageBox.Show("Replenishment Cycle Doesnt exist");
                return;
            }
            else
            {
               // ALL MARK this way are related
               // This is an indication that the authorisation passes from here
                Ta.Stats1.NoOfCheques = 1; // 
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                
            }

                // Get the date 
                labelCycleStart.Text = Ta.SesDtTimeStart.ToShortDateString();
            labelReplDate.Text = Ta.SesDtTimeEnd.ToShortDateString();

            bool Reject = false;
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            // Check through IST for FOREX DEPOSITS function for this ATM
            string WTableId = "Switch_IST_Txns";
            int WMode = 15; 
            Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);
           
            int rows = Mgt.DataTableAllFields.Rows.Count;

            if (rows>0)
            {
                FOREX_Deposits = true;
            }
            else
            {
                WTableId = "tblMatchingTxnsMasterPoolATMs";
                WMode = 16;
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);
                rows = Mgt.DataTableAllFields.Rows.Count;
                if (rows>0)
                {
                    FOREX_Deposits = true;
                }
                else
                {
                    FOREX_Deposits = false;
                }
                
            }
           
          
            //
            //ToxicJournal = false;
            //if (Ta.ProcessMode == -6)
            //{
            //    // Journal exist and numbers in cassettes do not reconciled 
            //    ToxicJournal = true; 
            //}
            //
            //MissingJournal = false;
            //if (Ta.ProcessMode == -5)
            //{
            //    // Journal is missing 
            //    MissingJournal = true; 
            //}

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int Mode = 1;
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            if (Mpa.TotalSelected > 0)
            {
                // Exceptions to show for replenishment 
                // EXCEPTIONS SUCH AS PRESENTER ERRORS
                Exceptions = true;
            }
            else
            {
                Exceptions = false;
            }
            Ac.ReadAtm(WAtmNo); 
            WAtmsReconcGroup = Ac.AtmsReconcGroup;

            RRDMReconcCategories Rc = new RRDMReconcCategories();

            Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

            WReconcCategoryId = Rc.CategoryId;
       

            // View only 
            // View only 
            if (WViewFunction == true)  // Viewing only ,,, Repl Cycle workflow had finished and we want t
            {
               // labelCycleStart.Text = Ta.SesDtTimeEnd.ToShortDateString();

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

           
            // YOU GOT INTO at the beggining
            // You show the first screen 
            // YOU activate next 
            // BY PRESSING NEXT BASED WHETHER THERE FOREX OR EXCEPTIONS YOU ACT Accordingly 
            if ((Usi.ProcessNo == 1
                || Usi.ProcessNo == 54 // ViewOnly 
              || Usi.ProcessNo == 55 // Authoriser
              || Usi.ProcessNo == 56) // Requestor 
             // & ToxicJournal == true 
              ) // Follow Replenishment Workflow
                //if (Us.ProcessNo == 1 || Us.ProcessNo == 5 || Us.ProcessNo == 31 || Us.ProcessNo == 26) // Follow Replenishment Workflow
            {
                if (W_CIT_Excel_STATUS == "04" & StepNumber == 1 & Usi.ProcessNo == 54)
                {
                    // MEANS THAT THIS WAS AUTO Replenished
                    //MessageBox.Show("This Replenishment Cycle Was Done Auto During Excel Loading" + Environment.NewLine
                    //                + "The Auto Involves Only the Shown Screen" + Environment.NewLine
                    //                + "The Maker is the Controller and the Checker is the System" + Environment.NewLine
                    //                + "View and Press Finish" + Environment.NewLine
                    //                     );
                    //this.Dispose();
                }
                // REPLENISHMENT STARTS update start in Ta.
                if (Usi.ProcessNo == 1)
                {
                    Ta.UpdateTracesStartRepl(WSignedId, WAtmNo, WSesNo);
                }

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
                else ViewWorkFlow = false;

                //Start with STEP 1
                UCForm51b_IST_Based Step51b_IST_Based = new UCForm51b_IST_Based();

                //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                Step51b_IST_Based.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51b_IST_Based, 0, 0);
               
                Step51b_IST_Based.Dock = DockStyle.Fill;

                //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                Step51b_IST_Based.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, W_IsFromExcel);
                Step51b_IST_Based.SetScreen();
                textBoxMsgBoard.Text = Step51b_IST_Based.guidanceMsg;

                if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                if (Usi.ReplStep1_Updated == true & ViewWorkFlow == false)
                {
                    textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                }

                tableLayoutPanelMain.Controls.Add(Step51b_IST_Based, 0, 0);
       
                labelStep1.ForeColor = Color.White;
                labelStep1.Font = new Font(labelStep2.Font.FontFamily, 18, FontStyle.Bold);
                //labelStep2.ForeColor = labelStep3.ForeColor;
                //labelStep2.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

                StepNumber= 1; // STEP becomes two
                buttonBack.Visible = false;
                buttonNext.Visible = true;

                if (W_CIT_Excel_STATUS == "04" & StepNumber == 1)
                {
                    // MEANS THAT THIS WAS AUTO Replenished
                    MessageBox.Show("This Replenishment Cycle Was Done Auto During Excel Loading" + Environment.NewLine
                                    + "The Auto Involves Only the Shown Screen" + Environment.NewLine
                                         );
                    buttonNext.Text = "Finish";
                    StepNumber = 5;
                    //this.Dispose();
                }

            }

        }


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

            //if (Na.Balances1.OpenBal == 0)
            //{
            //    OnlyLoading = true;
            //}
            //else
            //{
            //    OnlyLoading = false;
            //}

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 1 || ViewWorkFlow == true)
            {
                // Continue
            }
            else
            {
                return;
            }

            int WorkingStep1=0; 
            
            if (StepNumber == 1 & FOREX_Deposits == false & Exceptions == false)
            {

                WorkingStep1 = 4; // Go to the end 
                // Update Steps
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep2_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep3_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }
            if (StepNumber == 1 & FOREX_Deposits == true)
            {

                WorkingStep1 = 2; // Go to forex
            }

            if (StepNumber == 1 & FOREX_Deposits == false & Exceptions == true)
            {

                WorkingStep1 = 3; // Go to exceptions

                // Update Steps
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep2_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }

            if (StepNumber == 2 & Exceptions == true)
            {

                // Move one step
                WorkingStep1 = 3; // move to exceptions
                //Usi.ReadSignedActivityByKey(WSignRecordNo);
                //Usi.ReplStep3_Updated = true;
                //Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }

            if (StepNumber == 2 & Exceptions == false)
            {
              
                // Move one step
                WorkingStep1 = 4; // move to authorisation 
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReplStep3_Updated = true;
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }

            if (StepNumber == 3 & Exceptions == true)
            {
                // CHECK THAT Actions Has been taken to all Exceptions 
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                // Matching is done but not Settled 
                int Mode = 4; // See if actions not completed before you move to the nexr 
                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                               Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

                if (Mpa.RecordFound == true)
                {
                    MessageBox.Show("Take Actions For All Before You Move To Next Step!");
                    return;
                }
                else
                {
                    // Move one step
                    WorkingStep1 = 4; // MOVE to Authorisation 
                    Usi.ReadSignedActivityByKey(WSignRecordNo);
                    Usi.ReplStep3_Updated = true;
                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }


            }
            //StepNumber =5;
            if (StepNumber == 5 || StepNumber == 4)
            {
                WorkingStep1 = StepNumber;
            }
            
            StepNumber = WorkingStep1; 
            //
            // SWITCH StepNumber
            //
            //StepNumber = 2; 
            switch (StepNumber)
            {
                case 1: // 
                    {

                        // STEPLEVEL// Check Feasibility to move to next step 

                        UCForm51b_IST_Based Step51b_IST_Based = new UCForm51b_IST_Based();
                        Step51b_IST_Based.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51b_IST_Based, 0, 0);
                        Step51b_IST_Based.Dock = DockStyle.Fill;
                        Step51b_IST_Based.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, W_IsFromExcel);
                        textBoxMsgBoard.Text = Step51b_IST_Based.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep1.ForeColor = Color.White;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 18, FontStyle.Bold);

                        labelStep2.ForeColor = Color.LightSteelBlue;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                        labelStep3.ForeColor = Color.LightSteelBlue;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        labelStep4.ForeColor = Color.LightSteelBlue;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);


                        StepNumber = 1;
                        buttonBack.Visible = false;
                        buttonNext.Visible = true;
                        break;
                    }

                case 2: // FOREX DEPOSITS
                    {
                        // THIS IS DEPOSITS
                        if (Usi.ReplStep1_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 1; 
                            return;
                        }

                     
                        // 
                        //
                        GetMainBodyImage("UCForm51b");
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm51c_FOREX Step51c_FOREX = new UCForm51c_FOREX();
                        //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        Step51c_FOREX.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51c_FOREX, 0, 0);
                        Step51c_FOREX.Dock = DockStyle.Fill;
                        Step51c_FOREX.UCForm51cPar_FOREX(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, W_IsFromExcel);
                        textBoxMsgBoard.Text = Step51c_FOREX.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        if (Usi.ReplStep2_Updated == true & ViewWorkFlow == false)
                        {
                            textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                        }
                        labelStep1.ForeColor = Color.LightSteelBlue; 
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                        labelStep3.ForeColor = Color.LightSteelBlue;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        StepNumber= 2 ;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;

                    }
                case 3: // EXCEPTIONS From Deposits or Money in if there are exceptions
                    {
                        // CALL SAME AS IN Form271b
                        // YOU ARE At 3 and you move to handle Exceptions
                        // MOVE To Step 4
                       // Coming from money in 
                        if ((Usi.ReplStep1_Updated == false & FOREX_Deposits == false) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 1;
                            return;
                        }
                        // Coming from deposits
                        if ((Usi.ReplStep2_Updated == false & FOREX_Deposits == true ) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 2; 
                            return;
                        }
                        if (Exceptions == true) // GO TOWARDS 271b
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            UCForm271b Step271b = new UCForm271b();
                            //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                            tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                            Step271b.Dock = DockStyle.Fill;
                            //string WMatchingCateg = PRX + "20X";
                            //string RMCategory = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                            Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WReconcCategoryId, WRMCycle, 2, WAtmNo, WSesNo);
                            Step271b.SetScreen();
                            textBoxMsgBoard.Text = Step271b.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                            if (Usi.ReplStep3_Updated == true & ViewWorkFlow == false)
                            {
                                textBoxMsgBoard.Text = "INPUT DATA HAS BEEN UPDATED - Move to next step.";
                            }
                            labelStep1.ForeColor = Color.LightSteelBlue;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                            labelStep2.ForeColor = Color.LightSteelBlue;
                            labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep3.ForeColor = Color.White;
                            labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                            StepNumber = 3 ;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;

                        }
                        break;
                    }
                case 4: // authorisation screen From Summary
                    {
                        // FROM Basic GENERAL ONE 
                        if ((Usi.ReplStep1_Updated == false) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 2;
                            return;
                        }
                        // From FOREX
                        if ((Usi.ReplStep2_Updated == false & FOREX_Deposits == true) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 2;
                            return;
                        }
                        // Coming exceptions

                        if ((Usi.ReplStep3_Updated == false & Exceptions == true) & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            StepNumber = 3;
                            return;
                        }
                        UCForm51f Step51f = new UCForm51f();
                        Step51f.ChangeBoardMessage += Step51f_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51f, 0, 0);
                        Step51f.Dock = DockStyle.Fill;
                        Step51f.UCForm51fPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51f.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";
                        textBoxMsgBoard.Text = "Authorisation Process.";

                        //labelStep6.ForeColor = labelStep5.ForeColor;
                        //labelStep6.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep6.Font.Style);
                        labelStep1.ForeColor = Color.LightSteelBlue;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep2.ForeColor = Color.LightSteelBlue;
                        labelStep2.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep3.ForeColor = Color.LightSteelBlue;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        labelStep4.ForeColor = Color.White;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber = 4;
                        StepNumber = 5;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Finish";

                        break;

                    }

                case 5: // From Authorisation
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
                                if (WRequestor == true & Ap.AuthDecision == "" // No action by authoriser yet
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

                            if ((WAuthoriser == true || WRequestor ==true) & ReplenishmentAuthorDone == true & (Ap.Stage == 3 || Ap.Stage == 4)
                              & Ap.AuthDecision == "YES") // Everything is fine .
                            {
                                // Everything OK to move to transactions
                            }
                            else
                            {
                                //MessageBox.Show("MSG946 - Authorisation outstanding");
                                //this.Dispose();
                                //return;
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

                                        Mpa.ActionType = Aoc.ActionId; 
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
                                    //Ta.Stats1.NoOfCheques = 1; // ALREADY DONE
                                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                                    
                                }
                                else
                                {
                                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                                    Ta.Repl1.DiffRepl = false;
                                    Ta.ReconcAtRMCycle = WRMCycle;
                                    Ta.Recon1.RecFinDtTm = DateTime.Now;
                                    //Ta.Stats1.NoOfCheques = 1;
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

                            // UPDATE EXCEL ENTRIES with Process = 03 and Date of Replenishment 
                            if (W_IsFromExcel == true)
                            {
                                RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();
                                string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo;
                                Ce.Read_CIT_Excel_Table_BySelectionCriteria(WSelectionCriteria);
                                // STATUS 
                                //ReplCompletionDt
                                Ce.STATUS = "03";
                                Ce.ReplCompletionDt = DateTime.Now; 
                                Ce.Update_STATUS_And_ReplCompletion(Ce.SeqNo);

                                // ALSO UPDATE FILES RECORDS AS PROCESS
                                // INTERIM FILES "CIT_SWITCH_TXNS" and "CIT_JOURNAL_TXNS"
                                Ce.UpdateRecordsAsProcessed(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd); 

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
                            //if (Deposits == true)
                            //{
                            //    // PRINT ALL IMAGES INCLUDING DEPOSITS 
                            //    Form56 Report1 = new Form56(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo,
                            //                    SCREENa, SCREENb, SCREENc, SCREENd, "", Na.ReplUserComment, Ta.ReplGenComment, ReconcComment);
                            //    Report1.Show();
                            //}
                            //else
                            //{
                            //    // Print without the deposits
                            //}

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

                    //Form57 Report2 = new Form57(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, SCREENd);
                    //Report2.Show();
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
            textBoxMsgBoard.Text = ((UCForm51a)tableLayoutPanelMain.Controls["UCForm51a"]).guidanceMsg;
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
            
            int WorkingStep = 0; 
            // StepNumber = 5
            if (StepNumber == 5 & Exceptions == false & FOREX_Deposits == false)
            {
                WorkingStep = 1;
            }
            if (StepNumber == 5 & Exceptions == true )
            {
                WorkingStep = 3;
            }
            if (StepNumber == 5 & Exceptions == false & FOREX_Deposits == true)
            {
                WorkingStep = 2;
            }

            // StepNumber = 3 // we are at exceptions 
            if (StepNumber == 3 & FOREX_Deposits == true) //& FOREX_Deposits == true
            {
                WorkingStep = 2 ;
            }
            
            if (StepNumber == 3 & FOREX_Deposits == false)
            {
                WorkingStep = 1;
            }
            // StepNumber = 2 we are at FOREX
            if (StepNumber == 2 )
            {
                WorkingStep = 1;
            }

            //if (StepNumber == 2)
            //{
            //    WorkingStep = 2;
            //}

            StepNumber = WorkingStep; 

            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                //case 2:
                //    {
                //        UCForm51a Step51a = new UCForm51a();
                //        Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                //        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                //        tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                //        Step51a.Dock = DockStyle.Fill;
                //        Step51a.UCForm51aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                //        Step51a.SetScreen();
                //        textBoxMsgBoard.Text = Step51a.guidanceMsg;

                //        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                //        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";


                       
                //        labelStep4.ForeColor = labelStep4.ForeColor;
                //        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep2.Font.Style);
                //        labelStep3.ForeColor = labelStep4.ForeColor;
                //        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                //        labelStep2.ForeColor = labelStep4.ForeColor;
                //        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                //        labelStep1.ForeColor = Color.White;
                //        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 18, FontStyle.Bold);

                //        StepNumber--;
                //        buttonBack.Visible = false;
                //        buttonNext.Visible = true;

                //        buttonNext.Text = "Next >";

                //        break;
                //    }
                case 1:
                    {
                        UCForm51b_IST_Based Step51b_IST_Based = new UCForm51b_IST_Based();
                        Step51b_IST_Based.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51b_IST_Based, 0, 0);
                        Step51b_IST_Based.Dock = DockStyle.Fill;
                        Step51b_IST_Based.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, W_IsFromExcel);
                        textBoxMsgBoard.Text = Step51b_IST_Based.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep1.ForeColor = Color.White;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 18, FontStyle.Bold);

                        labelStep2.ForeColor = Color.LightSteelBlue;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                        labelStep3.ForeColor = Color.LightSteelBlue;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        labelStep4.ForeColor = Color.LightSteelBlue;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);


                        StepNumber = 1; 
                        buttonBack.Visible = false;
                        buttonNext.Visible = true;

                        buttonNext.Text = "Next >";

                        break;
                    }
                case 2:
                    {
                        
                        UCForm51c_FOREX Step51c_FOREX = new UCForm51c_FOREX();
                        Step51c_FOREX.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step51c_FOREX, 0, 0);
                        Step51c_FOREX.Dock = DockStyle.Fill;
                        Step51c_FOREX.UCForm51cPar_FOREX(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, W_IsFromExcel);
                        textBoxMsgBoard.Text = Step51c_FOREX.guidanceMsg;

                        if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep1.ForeColor = Color.LightSteelBlue;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep1.Font.FontFamily, 18, FontStyle.Bold);

                        labelStep3.ForeColor = Color.LightSteelBlue;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);

                        labelStep4.ForeColor = Color.LightSteelBlue;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);

                        StepNumber = 2;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;
                        buttonNext.Text = "Next >";
                        break;
                    }
                case 3:
                    {
                        if (Based_On_IST == true)
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            UCForm271b Step271b = new UCForm271b();
                            //Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                            tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                            Step271b.Dock = DockStyle.Fill;
                            //string WMatchingCateg = PRX + "20X";
                            //string RMCategory = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                            Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WReconcCategoryId, WRMCycle, 2, WAtmNo, WSesNo);
                            Step271b.SetScreen();
                            textBoxMsgBoard.Text = Step271b.guidanceMsg;

                            if (Usi.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                            if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                            labelStep1.ForeColor = Color.LightSteelBlue;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);

                            labelStep2.ForeColor = Color.LightSteelBlue;
                            labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);

                            labelStep3.ForeColor = Color.White;
                            labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                            labelStep4.ForeColor = Color.LightSteelBlue;
                            labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);


                            StepNumber = 3;
                            buttonBack.Visible = true;
                            buttonNext.Visible = true;
                            buttonNext.Text = "Next >";
                        }
                       
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
            textBoxMsgBoard.Text = ((UCForm51d2_BDC_IST)tableLayoutPanelMain.Controls["UCForm51d2_BDC_IST"]).guidanceMsg;
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
            textBoxMsgBoard.Text = ((UCForm51c_FOREX)tableLayoutPanelMain.Controls["UCForm51c_FOREX"]).guidanceMsg;
        }

        void Step51b_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51b_IST_Based)tableLayoutPanelMain.Controls["UCForm51b_IST_Based"]).guidanceMsg;
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
                //memoryImage = new System.Drawing.Bitmap(((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).Width, ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).Height);
                //((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).DrawToBitmap(memoryImage, ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).ClientRectangle);
                //SCREENb = memoryImage;
            }
            if (ControlName.Equals("UCForm51c"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).Width, ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).Height);
                //((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).DrawToBitmap(memoryImage, ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).ClientRectangle);
                //SCREENc = memoryImage;
            }
            if (ControlName.Equals("UCForm51d"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).Width, ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).Height);
                //((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).DrawToBitmap(memoryImage, ((UCForm51d)tableLayoutPanelMain.Controls["UCForm51d"]).ClientRectangle);
                //SCREENd = memoryImage;
            }

            if (ControlName.Equals("UCForm51d2"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).Width, ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).Height);
                //((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).DrawToBitmap(memoryImage, ((UCForm51d2)tableLayoutPanelMain.Controls["UCForm51d2"]).ClientRectangle);
                //SCREENd = memoryImage;
            }
            if (ControlName.Equals("UCForm51d2_BDC"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).Width, ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).Height);
                //((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).DrawToBitmap(memoryImage, ((UCForm51d2_BDC)tableLayoutPanelMain.Controls["UCForm51d2_BDC"]).ClientRectangle);
                //SCREENd = memoryImage;
            }
            if (ControlName.Equals("UCForm51d2_AUD"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).Width, ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).Height);
                //((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).DrawToBitmap(memoryImage, ((UCForm51d2_AUD)tableLayoutPanelMain.Controls["UCForm51d2_AUD"]).ClientRectangle);
                //SCREENd = memoryImage;
            }
            if (ControlName.Equals("UCForm51e"))
            {
                //memoryImage = new System.Drawing.Bitmap(((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).Width, ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).Height);
                //((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).DrawToBitmap(memoryImage, ((UCForm51e)tableLayoutPanelMain.Controls["UCForm51e"]).ClientRectangle);
                //SCREENe = memoryImage;
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

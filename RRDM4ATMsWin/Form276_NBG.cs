using RRDM4ATMs;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RRDM4ATMsWin
{
    public partial class Form276_NBG : Form
    {

        int StepNumber;

        //bool Step2AndStep3Omitted = false;
        //bool Step3Omitted = false;

        // Working Fields 
        bool WViewFunction; // 54
        bool WAuthoriser; // 55
        bool WRequestor;  // 56

        string WFunction = "";

        DateTime WDTm;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSelectionCriteria;

        bool ReconciliationAuthorNoRecordYet;
        bool ReconciliationAuthorDone;

        bool ReconciliationAuthorOutstanding;

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        string WCategory;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCitId;

        int WLoadingCycle;
        //  string WFunction;

        public Form276_NBG(string InSignedId, int InSignRecordNo, string InOperator, string InCitId, int InLoadingCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCitId = InCitId;

            WLoadingCycle = InLoadingCycle;

            //       WFunction = InFunction;

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserBankId = Us.Operator;
            // ========================================================

            WCategory = WCitId;

            // Set Working Date 


            pictureBox1.BackgroundImage = appResImg.logo2;

            StepNumber = 1;

            labelCitId.Text = WCitId;

            // STEPLEVEL
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.WFieldChar1 = WCategory;
            Usi.WFieldNumeric1 = WLoadingCycle;

            //Usi.StepLevel = 0;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            // Load Cycle
            //         

            labelExcelCycle.Text = WLoadingCycle.ToString();

            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

            labelExcelDate.Text = Cec.StartDateTm.ToShortDateString();

            if (WOperator == "CRBAGRAA")
            {
                WDTm = new DateTime(2014, 02, 28);
            }
            else
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                int WReconcCycleNo;

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo == 0)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
                DateTime WCut_Off_Date = Rjc.Cut_Off_Date;
                WDTm = WCut_Off_Date;
            }

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

          

            // Requestor 
            if (WRequestor == true)  // 56: Coming from requestor 
            {
                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WLoadingCycle, "LoadingExcel");

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

            if (Usi.ProcessNo == 2 // Follow Reconciliation Workflow - NORMAL 2
                  || Usi.ProcessNo == 54 // ViewOnly 
                || Usi.ProcessNo == 55 // Authoriser
                || Usi.ProcessNo == 56 // Requestor   
                )
            {
                //Start with STEP 1


                //   tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                UCForm276a_NBG Step276a_NBG = new UCForm276a_NBG();

                //  Step276a_NBG.ChangeBoardMessage += Step276a_NBG_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step276a_NBG, 0, 0);
                Step276a_NBG.Dock = DockStyle.Fill;
                Step276a_NBG.UCForm276a_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);

                Step276a_NBG.SetScreen();
                textBoxMsgBoard.Text = Step276a_NBG.guidanceMsg;

                if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review and go to Next";
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

            if (StepNumber == 1 & Usi.ProcessNo == 2)
            {
                // Check if RRDM WAS POPULATED
                if (Usi.WFieldNumeric12 == 45)
                {

                }
                else
                {
                    MessageBox.Show("Populate RRDM with Excel Entries " + Environment.NewLine
                                     + "before moving to the next Step"
                                   );
                    return;
                }
            }

            if (StepNumber == 2 & Usi.ProcessNo == 2)
            {
                // Check if Validation button was pressed by user
                if (Usi.WFieldNumeric12 == 46)
                {

                }
                else
                {
                    MessageBox.Show("Validate Data " + Environment.NewLine
                                     + "before moving to the next Step"
                                   );
                    return;
                }
            }

            //
            // SWITCH StepNumber
            //
            switch (StepNumber)
            {
                case 1:
                    {


                        UCForm276b_NBG Step276b_NBG = new UCForm276b_NBG();

                        Step276b_NBG.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step276b_NBG, 0, 0);
                        Step276b_NBG.UCForm276b_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);
                        Step276b_NBG.SetScreen();
                        textBoxMsgBoard.Text = Step276b_NBG.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep1.ForeColor = labelStep3.ForeColor;
                        labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                        // Step Becomes 2
                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        break;
                    }

                case 2:
                    {
                        //tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm276c_NBG Step276c_NBG = new UCForm276c_NBG();

                        Step276c_NBG.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step276c_NBG, 0, 0);
                        Step276c_NBG.UCForm276c_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);
                        Step276c_NBG.SetScreen();
                        textBoxMsgBoard.Text = Step276c_NBG.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";


                        labelStep2.ForeColor = labelStep1.ForeColor;
                        labelStep2.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);


                        // Step Becomes 3
                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Finish";
                        buttonNext.Visible = true;

                        break;
                    }
                case 3:
                    {
                        //**************************************************
                        //FINISH BUTTON .... HANDLE AUTHORISATION VALIDATION 
                        //**************************************************

                        if (WViewFunction == true) // Coming from View only 
                        {
                            this.Dispose();
                            return;
                        }

                        // FINISH - Make validationsfor Authorisations  


                        //  ReconciliationAuthorNeeded = true;

                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCategory, WLoadingCycle, "LoadingExcel");
                        if (Ap.RecordFound == true)
                        {
                            ReconciliationAuthorNoRecordYet = false;
                            if ((Ap.Stage == 3 || Ap.Stage == 4 || Ap.Stage == 5) )
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
                            this.Dispose();
                            return;
                        }

                        if (Ap.AuthDecision == "NO" & ReconciliationAuthorDone == true)
                        {
                            this.Dispose();
                            return;
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
                            if (MessageBox.Show("Warning: Authorisation outstanding "
                                + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
                            this.Dispose();
                            return;
                        }

                        if (WRequestor == true & ReconciliationAuthorDone == true & Ap.Stage == 4
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

                        //
                        //**************************************************************************
                        //**************************************************************************
                        // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
                        //**************************************************************************
                        //***********************************************************************
                        //**************** USE TRANSACTION SCOPE
                        //***********************************************************************

                        // create a connection object
                        //   using (var scope = new System.Transactions.TransactionScope())
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
                                //
                                // Update and create TXNS
                                //
                                FinishLoadingWithUpdates();

                                // HERE YOU FINALISE TRANSACTIONS 

                                // END OF FINISH
                                this.Dispose();

                                // COMPLETE SCOPE
                                //
                                //scope.Complete();

                                //this.Close();

                            }

                        }
                        catch (Exception ex)
                        {
                            CatchDetails(ex);
                        }
                        //finally
                        //{
                        //    scope.Dispose();
                        //}
                        // END for finish here

                        break;
                    }

                default:
                    {
                        break;
                    }
            }

        }

        // Stavros
        // EVENT HANDLER FOR MESSAGE

        //void Step276c_NBG_ChangeBoardMessage(object sender, EventArgs e)
        //{
        //    textBoxMsgBoard.Text = ((UCForm271a)tableLayoutPanelMain.Controls["UCForm271a"]).guidanceMsg;
        //}

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

            bool Jump_Step2 = false;
            if (StepNumber == 3)
            {
                // Check if Orders have been created

                // Read Orders Counters
                bool DontKnowReason = true; 
                if (DontKnowReason == false)
                {
                    Ro.ReadReplActionsForCounters(WCitId, WLoadingCycle);

                    if (Ro.NumberOfActiveOrders == 0)
                    {
                        // Move one step 
                        StepNumber = 2;
                        Jump_Step2 = true;
                    }
                    else
                    {
                        Jump_Step2 = false;
                    }
                }
                

            }

            //-----------------------------------------------------------
            switch (StepNumber)
            {
                case 2:
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm276a_NBG Step276a_NBG = new UCForm276a_NBG();

                        //  Step276a_NBG.ChangeBoardMessage += Step276a_NBG_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step276a_NBG, 0, 0);
                        Step276a_NBG.Dock = DockStyle.Fill;
                        Step276a_NBG.UCForm276a_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);
                        // Step276a_NBG.UCForm276a_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, WFunction);
                        Step276a_NBG.SetScreen();
                        textBoxMsgBoard.Text = Step276a_NBG.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Review and go to Next";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        if (Jump_Step2 == false)
                        {
                            labelStep2.ForeColor = labelStep3.ForeColor;
                            labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep1.ForeColor = Color.White;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);

                        };

                        if (Jump_Step2 == true)
                        {
                            labelStep3.ForeColor = labelStep2.ForeColor;
                            labelStep3.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                            labelStep1.ForeColor = Color.White;
                            labelStep1.Font = new Font(labelStep1.Font.FontFamily, 16, FontStyle.Bold);


                        };

                        buttonNext.Visible = true;

                        buttonBack.Visible = false;

                        StepNumber--;

                        break;
                    }
                case 3:
                    {

                        UCForm276b_NBG Step276b_NBG = new UCForm276b_NBG();

                        Step276b_NBG.Dock = DockStyle.Fill;

                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step276b_NBG, 0, 0);
                        Step276b_NBG.UCForm276b_NBG_Par(WSignedId, WSignRecordNo, WOperator, WCitId, WLoadingCycle);
                        Step276b_NBG.SetScreen();
                        textBoxMsgBoard.Text = Step276b_NBG.guidanceMsg;

                        if (Usi.ProcessNo == 2) textBoxMsgBoard.Text = "Examine And Act on Orders if needed.";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep3.ForeColor = labelStep2.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Next >";

                        break;
                    }
                default:
                    {
                        break;
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



        // UPDATE AND FINISH BUTTON

        private void FinishLoadingWithUpdates()
        {

            // Update RRDM Replenishmet record
            //
            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RRDMAccountsClass Acc = new RRDMAccountsClass();

            RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();

            int TotalInDiff = 0;

            string WJobCategory = "ATMs";
            int WReconcCycleNo;
            string Message;

            decimal WUnloadedMachine ;

            decimal WDiffGL;

            decimal WUnloadedMachineDep ;

            decimal WCash_Loaded_Machine ;

            int WSeqNo = 0; 

            bool ShowMessage = true;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            string WReconCategGroup;
            string WAtmNo = "";
            int WSesNo = 0;
            // Read all Outstanding Matched Entries from G4S file
            // Make a loop and update 

            // Make Selection Of Validated Entries 
            int TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId 
                + "' "
                 + " AND (Mask = '11' OR Mask = 'AA') AND ProcessMode <> 1 " ;
           // + " AND (Mask = '11' OR Mask = 'AA') AND LoadingExcelCycleNo=" + WLoadingCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records to update");
                return;
            }

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                // GET ALL fields

                //    RecordFound = true;
                WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];
                WAtmNo = (string)G4.DataTableG4SEntries.Rows[I]["AtmNo"];
                WSesNo = (int)G4.DataTableG4SEntries.Rows[I]["ProcessedAtReplCycleNo"];
                decimal WUnloadedCounted = (decimal)G4.DataTableG4SEntries.Rows[I]["UnloadedCounted"];
                decimal WCash_Loaded_CIT = (decimal)G4.DataTableG4SEntries.Rows[I]["Cash_Loaded"];
                decimal WDepositsCounted = (decimal)G4.DataTableG4SEntries.Rows[I]["Deposits"];
                string WMask = (string)G4.DataTableG4SEntries.Rows[I]["Mask"];


                if (WMask == "11")
                {
                    try
                    {
                        // READ ALL FIELDS 
                        G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, 1); // Read CIT Record

                        Ac.ReadAtm(WAtmNo);
                        int WAtmsReconcGroup = Ac.AtmsReconcGroup;

                        Rc.ReadReconcCategorybyGroupId(WAtmsReconcGroup);

                        string WReconcCategoryId = Rc.CategoryId;

                        WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                        string WStage = "";
                       
                            //
                            // Update HERE so you will have authoriser during Transaction creation
                            //
                            Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, WSignedId,
                               Ap.Authoriser, WReconcCategoryId);
                        
                        WJobCategory = "ATMs";
                        // int WReconcCycleNo;

                        WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();


                        // Create Table of Txns 
                        // READ ALL IN THIS CYCLE
                        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                        string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                                              + "' AND ReplCycle =" + WSesNo + " AND Maker ='"
                                              + WSignedId + "' AND Stage In ('01','02')";

                        Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

                        Aoc.ClearTableTxnsTableFromAction();

                        int K = 0;

                        while (K <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
                        {

                            WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[K]["SeqNo"];

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

                            if (Aoc.UniqueKeyOrigin == "Master_Pool")
                            {
                                // THIS IS FOR PRESENTER ERROR
                                WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                                Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                                Mpa.ActionByUser = true;
                                Mpa.UserId = WSignedId;
                                Mpa.Authoriser = Ap.Authoriser;
                                Mpa.AuthoriserDtTm = DateTime.Now;

                                Mpa.SettledRecord = true;

                                WSelectionCriteria = " WHERE UniqueRecordId =" + Aoc.UniqueKey;
                                Mpa.UpdateMatchingTxnsMasterPoolATMsForcedMatched(WOperator, WSelectionCriteria, 1);

                                WStage = "02"; // Confirmed by maker 
                                Aoc.UpdateOccurancesStage("Master_Pool", Aoc.UniqueKey, WStage, DateTime.Now, WReconcCycleNo, WSignedId);
                                // Authoriser 
                                // Also Make stage = "03"
                                Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Aoc.UniqueKey, Ap.Authoriser, Ap.SeqNumber, WSignedId);

                            }

                            //********************************************
                            // HERE WE CREATE THE ENTRIES AS PER BDC NEEDS
                            //********************************************

                            K = K + 1;
                        }

                        DataTable TempTxnsTableFromAction;
                        TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

                        // UPDATE OCCURANCES FOR REPLENISHMENT RECORDS AS CONFIRMED
                        // For done within this cycle 

                        WStage = "02"; // Confirmed by maker 
                        Aoc.UpdateOccurancesStage("Replenishment", WSesNo, WStage, DateTime.Now, WReconcCycleNo, WSignedId);
                        // Also Authoriser 
                        // Also Update Stage as "03"
                        Aoc.UpdateOccurancesForAuthoriser("Replenishment", WSesNo, Ap.Authoriser, Ap.SeqNumber, WSignedId);

                        //
                        Aoc.ReadActionsOccurancesTo_RichPicture_One_ATM(WAtmNo, WSesNo);

                        // UPDATE Ta
                        if (Aoc.Current_ShortageBalance < 0
                            || Aoc.WaitForDisputeNo < Aoc.WaitAndSettledDisputeNo
                            || Aoc.NoWaitDisputeNo < Aoc.NoWaitSettledDisputeNo)
                        {
                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                            Ta.Repl1.DiffRepl = true;
                            Ta.ReconcAtRMCycle = WReconcCycleNo;
                            Ta.Recon1.RecFinDtTm = DateTime.Now;
                            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                        }
                        else
                        {
                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                            Ta.Repl1.DiffRepl = false;
                            Ta.ReconcAtRMCycle = WReconcCycleNo;
                            Ta.Recon1.RecFinDtTm = DateTime.Now;
                            Ta.ReplGenComment = "Repl Finalised Through Excel Process no.."+ WLoadingCycle.ToString(); 
                            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                        }

                        // Insert Authorisation Record for this ATM and SesNo

                        Ap.Requestor = Ap.Requestor;
                        Ap.Authoriser = Ap.Authoriser;
                        Ap.Origin = "Replenishment";
                        Ap.TranNo = 0;
                        Ap.DisputeNumber = 0;
                        Ap.DisputeTransaction = 0;
                        Ap.AtmNo = WAtmNo;
                        Ap.ReplCycle = WSesNo;
                        Ap.DateOriginated = DateTime.Now;
                        Ap.Stage = 5;
                        Ap.DifferenceStatus = 0;
                        Ap.RMCategory = ""; 
                        Ap.RMCycle = WReconcCycleNo;
                        Ap.Operator = WOperator; 

                        int TempSeqNo = Ap.InsertAuthorizationRecord();


                        Ap.AuthDecision = "YES";
                        Ap.AuthComment = "Approved During Excel Loading"; 
                        Ap.DateAuthorised = DateTime.Now;
                        Ap.Stage = 5;

                        Ap.UpdateAuthorisationRecord(TempSeqNo); 

                        // Update Notes and Values 

                        Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                        
                        Na.InUserDate = DateTime.Now;
                        Na.Cit_ExcelUpdatedDate = DateTime.Now;
                        Na.Cit_UnloadedCounted = WUnloadedCounted;
                        Na.Cit_Over = G4.OverFound;
                        Na.Cit_Short = G4.ShortFound;
                        Na.Cit_Loaded = WCash_Loaded_CIT;

                        // UPDATE OTHER USED FIELDS IN NA Record 
                        Na.InReplAmount = G4.Cash_Loaded;
                        Na.ReplAmountSuggest = G4.Cash_Loaded;
                        Na.ReplAmountTotal = G4.Cash_Loaded;
                        Na.InsuranceAmount = G4.Cash_Loaded;

                        Na.ReplUserComment = G4.RemarksG4S;

                        Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                        // UPDATE CASSETTES 

                        Na.UpdateSessionsNotesAndValuesWithCountedFromCITExcel(WAtmNo, WSesNo);

                        // Physical Inspection Data set it as true 

                        Pi.UpdateSessionsPhysicalInspectionRecord(WAtmNo, WSesNo, true);

                        // Update G4.Record.With Process Mode == 1

                        G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);
                        G4.ProcessMode_Load = 1; // 
                        G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 1);

                        // Read and Update Update corresponding Banks record  
                        G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 2);
                        G4.ProcessMode_Load = 1; // 
                        G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2);


                    }
                    catch (Exception ex)
                    {
                        CatchDetails(ex);
                    }

                }
                else
                {
                    // Do not make transactions and make process mode to 4

                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            
                    Ta.ProcessMode = 4;
                    Ta.ReplGenComment = "At Excel Loading we have found differences" + Environment.NewLine
                        + "Manual Replenishment is needed " ;

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                    // Update G4.Record.With Process Mode == 1

                    G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);
                    G4.ProcessMode_Load = 1; // 
                    G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 1);

                    // Read and Update Update corresponding Banks record  
                    G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 2);
                    G4.ProcessMode_Load = 1; // 
                    G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2);
                }

                I++; // Read Next entry of the table 

            }

            //
            //
            // FINALLY UPDATE G4S Records 
            //

            

            // Update Cycle Record
            Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

            TotalsAfterValidation(); // Call totals to assign values 

            //Cec.PresenterNumberEqual = TotalPresenterEqual;

            //Cec.PresenterDiffAmt = TotalPresenterDiffAmt;  

            // SET THESE TEMPORARILY TO ZERO

            //Cec.ValidInExcelRecords = WTotal11;
            //Cec.InvalidInExcelRecords = WTotalAA;
            //Cec.NotInBank = WTotal10;

            //Cec.ShortFound = G4.TotalShort;

            //Cec.PresenterDiff = G4.TotalPresenter;

            //Cec.PresenterNumberEqual = 0;

            ////Cec.PresenterDiffAmt = 0;

            //Cec.AtmsInGl_Differ = TotalInDiff;

            Cec.ProcessStage = 3;

            Cec.FinishDateTm = DateTime.Now;

            Cec.UpdateLoadExcelCycle(WLoadingCycle);


            bool testlevelone = false; 

            if (testlevelone == true)
            {
                // Get Unloaded Machine from Second = Bank record 
                SelectionCriteria = " WHERE AtmNo ='" + WAtmNo
                           + "' AND LoadingExcelCycleNo =" + WLoadingCycle;
                TempMode = 2;
                G4.ReadCIT_G4S_Repl_EntriesBySelectionCriteria(SelectionCriteria, TempMode);
                WUnloadedMachine = G4.UnloadedMachine;

                // GET ALL fields from first 
                TempMode = 1;
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, TempMode);

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                decimal UnlMachine = Na.Balances1.MachineBal;

                Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                if (G4.Gl_Balance_At_CutOff > 0)
                {
                    // Insert record in GL File 
                    Gadj.OriginFileName = G4.OriginFileName;
                    Gadj.OriginalRecordId = G4.SeqNo;
                    Gadj.Cut_Off_Date = G4.Cut_Off_date;
                    Gadj.MatchingCateg = "";
                    Gadj.AtmNo = G4.AtmNo;
                    Gadj.Origin = "BANK";
                    Gadj.TransTypeAtOrigin = "GL Entry";

                    string ATMSuspence = "";
                    string ATMCash = "";

                    //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Suspense");

                    if (Acc.RecordFound == true)
                    {
                        ATMSuspence = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Suspense Account Not Found for ATM :" + WAtmNo);
                    }

                    Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Cash");
                    if (Acc.RecordFound == true)
                    {
                        ATMCash = Acc.AccNo;
                    }
                    else
                    {
                        MessageBox.Show("ATM Cash Account Not Found for ATM :" + WAtmNo);
                    }

                    Gadj.GL_AccountNo = ATMCash;
                    Gadj.Ccy = Ac.DepCurNm;

                    if (WOperator == "ETHNCY2N")
                    {
                        // Make correction of input General Ledger
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff - G4.Cash_Loaded + WUnloadedMachine;
                    }
                    else
                    {
                        Gadj.GL_Balance = G4.Gl_Balance_At_CutOff;
                    }

                    Gadj.DateCreated = DateTime.Now;
                    Gadj.Processed = false;

                    Gadj.ProcessedAtRMCycle = WReconcCycleNo;
                    Gadj.Operator = WOperator;
                    // THIS IS DONE FOR NATIONAL BANK
                    int GLSeqNo = Gadj.Insert_GL_Balances();
                }

                // ****************************
                // Set ANd Find Basic Information 
                // ****************************          

                WOperator = Ac.Operator;

                Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);

                WReconCategGroup = Rc.CategoryId;

                //**********************************

                // Update Notes and Values  

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                Na.InUserDate = DateTime.Now;
                Na.Cit_ExcelUpdatedDate = DateTime.Now;
                Na.Cit_UnloadedCounted = G4.UnloadedCounted;
                Na.Cit_Over = G4.OverFound;
                Na.Cit_Short = G4.ShortFound;
                Na.Cit_Loaded = G4.Cash_Loaded;

                // UPDATE OTHER USED FIELDS IN NA Record 
                Na.InReplAmount = G4.Cash_Loaded;
                Na.ReplAmountSuggest = G4.Cash_Loaded;
                Na.ReplAmountTotal = G4.Cash_Loaded;
                Na.InsuranceAmount = G4.Cash_Loaded;

                Na.ReplUserComment = G4.RemarksG4S;

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // Find if for this ATM there is GL difference

                decimal GL_Adjusted = 0;

                GL_Adjusted = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_First_Method(WOperator, WAtmNo, WSesNo, G4.ReplDateG4S);

                if (Gadj.IsDataFound == true)
                {
                    // THIS ASSIGNMENT ALready Done in Gadj.FindAdjusted_GL_Balance
                    //Na.Is_GL_Adjusted = true;
                    //Na.GL_Bal_Repl_Adjusted = GL_Adjusted;

                    // REFRESH Na
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                }
                else
                {
                    // SET TO THIS for this new ATM 
                    // 

                    GL_Adjusted = G4.UnloadedCounted;

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    if (Na.IsNewAtm == true)
                    {
                        Na.Is_GL_Adjusted = true;
                    }
                    else
                    {
                        Na.Is_GL_Adjusted = false;
                    }

                    Na.GL_Bal_Repl_Adjusted = G4.UnloadedCounted;
                }

                // Initialise

                Na.DiffAtAtmLevel_Cit = false;
                Na.DiffAtHostLevel_Cit = false;
                Na.DiffWithErrors_Cit = false;

                if (G4.OverFound > 0 || G4.ShortFound > 0)
                {
                    Na.DiffAtAtmLevel_Cit = true;
                }
                if (G4.UnloadedCounted != GL_Adjusted)
                {
                    Na.DiffAtHostLevel_Cit = true;
                }
                Ec.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

                if (Ec.TotalErrorsAmtLess100 > 0)
                {
                    Na.DiffWithErrors_Cit = true;
                }

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                // UPDATE TRACES WITH FINISH 
                // Update all fields and Reconciliation mode = 2 if all reconcile and Host files available 
                // After "Replenishement and Before reconciliation 

                Ta.UpdateTracesFinishRepl_From__G4S(WAtmNo, WSesNo, WSignedId, WReconCategGroup);

                // READ LATEST FROM Ta
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                if (Ta.ProcessMode == 1)
                {
                    TotalInDiff = TotalInDiff + 1;
                }

                // READ AGAIN G4 Record 
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);

                // ************************************************************
                // CHECK GL DIFFERENCES 
                // ************************************************************

                // Ready for replenishment 

                // Compare GL_Adjusted VS G4.UnloadedCounted 

                if (G4.UnloadedCounted != GL_Adjusted || G4.OverFound != 0 || G4.ShortFound != 0 || Ec.NumOfErrors > 0)
                {
                    // There is difference

                    Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WReconCategGroup, WReconcCycleNo);

                    Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                    Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff + 1;

                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WReconCategGroup, WReconcCycleNo);

                }
                else
                {
                    Am.ReadAtmsMainSpecific(WAtmNo);
                    Am.Maker = "N/A";
                    Am.Authoriser = "N/A";
                    Am.UpdateAtmsMain(WAtmNo);
                }

                //
                // FINALLY UPDATE G4S Records 
                //

                // Update G4.Record.With Process Mode == 1
                G4.ProcessMode_Load = 1; // Updated Mode
                G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, 1);

                // Read and Update Update corresponding Banks record  
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 2);
                G4.ProcessMode_Load = 1; // 
                G4.UpdateCIT_G4S_Repl_EntriesRecord(G4.SeqNo, 2);

                // CREATE TRANSACTIONS TO BE POSTED FOR REPLENISHMENT 
                // Whole Amount Goes to ATM Cash 
                if (G4.CITId != "1000")
                {
                    // Transactions = 1000 were made during replenishment
                    Ec.CreateTransTobepostedfromReplenishment_CIT(WOperator, WAtmNo, WSesNo, WSignedId, WSignedId, Na.Cit_UnloadedCounted, Na.Cit_Loaded);
                }


                //  

                // Update Cycle Record
                Cec.ReadExcelLoadCyclesBySeqNo(WLoadingCycle);

                TotalsAfterValidation(); // Call totals to assign values 

                //Cec.PresenterNumberEqual = TotalPresenterEqual;

                //Cec.PresenterDiffAmt = TotalPresenterDiffAmt;  

                // SET THESE TEMPORARILY TO ZERO

                //Cec.PresenterNumberEqual = 0;

                //Cec.PresenterDiffAmt = 0;

                //Cec.AtmsInGl_Differ = TotalInDiff;

                Cec.ProcessStage = 3;

                Cec.FinishDateTm = DateTime.Now;

                Cec.UpdateLoadExcelCycle(WLoadingCycle);

                if (TotalInDiff > 0)
                {
                    Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                              + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                              + "There is need for GL reconciliation " + Environment.NewLine
                                              + "For :" + TotalInDiff.ToString() + "..ATM/s " + Environment.NewLine
                                              );
                    MessageForm.ShowDialog();
                }
                else
                {
                    Form2 MessageForm = new Form2("Updating Done! " + Environment.NewLine
                                             + "Transactions for ATMs Cash GL account created for posting. " + Environment.NewLine
                                             + "Not found differences."
                                             );
                    MessageForm.ShowDialog();
                }
                RRDMUserSignedInRecords Usr = new RRDMUserSignedInRecords();
                Usr.ReadSignedActivity(WSignedId);

            }


        }

       

        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances_WithDrawels(string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;

            //int WFunction = 2;
            //Na.ReadSessionsNotesAndValues(WWAtmNo, WWSesNo, WFunction); // Read Values from NOTES

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }
            //
            if (WOperator == "AUDBEGCA")
                HybridRepl = false;

            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction;

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";

            if (HybridRepl == false)
            {
                // FIRST DOUBLE ENTRY 
                WActionId = "25"; // 25_DEBIT_ CIT Account/CR_AtmCash (UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment"

                WMaker_ReasonOfAction = "Un Load From ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                      WWAtmNo, WWSesNo, WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }


            //
            // CLEAR PREVIOUS ACTIONS FOR THIS REPLENISHMENT
            //
            if (HybridRepl == false)
            {
                WActionId = "29";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "39";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "30";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            // Delete create Dispute Shortage

            if (HybridRepl == false)
            {
                WActionId = "87";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "77";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "88";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            if (InDiffGL == 0)
            {
                // do nothing
            }

            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //         + "Will be moved to the Branch excess account "
                //    );
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "30"; //30_CREDIT Branch Excess / DR_AtmCash(UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //        + "Will be moved to the Branch shortage account "
                //         );
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
                }
                if (HybridRepl == true)
                {
                    WActionId = "39"; // 29_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo,
                                                      WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            // Handle Any Balance In Action Occurances 
            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo
                       + "' AND ReplCycle =" + WWSesNo
                       + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            if (Aoc.Current_DisputeShortage != 0)
            {
                //MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
                //         + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
                //         + "Look at the resulted transactions");


                decimal CIT_Shortage = 0;
                decimal Shortage = 0;
                decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
                decimal WExcess = Aoc.Excess;

                if (HybridRepl == false)
                {
                    CIT_Shortage = -(Aoc.CIT_Shortage);
                }
                if (HybridRepl == true)
                {
                    Shortage = -(Aoc.CIT_Shortage);
                }


                if (WExcess > 0)
                {
                    if (WExcess >= Dispute_Shortage)
                    {
                        // A
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = Dispute_Shortage;
                        WMaker_ReasonOfAction = "Settle Dispute Shortage";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }
                    else
                    {   // A
                        // Use all amount of Excess
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = WExcess; // Use all amount iin Excess
                        WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                        // The rest you take it from Shortage

                        decimal TempDiff1 = Dispute_Shortage - WExcess;
                        if (TempDiff1 > 0)
                        {
                            // Diff1 goes to Shortage
                            // B
                            if (HybridRepl == false)
                            {
                                WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            if (HybridRepl == true)
                            {
                                WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            // 
                            WUniqueRecordId = WWSesNo; // SesNo 
                            WCcy = "EGP";
                            DoubleEntryAmt = TempDiff1;
                            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                  WActionId, WUniqueRecordIdOrigin,
                                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                                  , WMaker_ReasonOfAction, "Replenishment");

                            TEMPTableFromAction = Aoc.TxnsTableFromAction;
                        }

                    }
                }

                if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
                    || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
                    )
                {
                    // 
                    if (HybridRepl == false)
                    {
                        WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                    }
                    if (HybridRepl == true)
                    {
                        WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
                    }
                    // 
                    WUniqueRecordId = WWSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Dispute_Shortage;
                    WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

            }

        }

        // Create Action Occurances
       
        public void CreateActions_Occurances_Dep(string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            decimal CassetteAmt;
            decimal RetractedAmt;
            //

           

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";
            string WMaker_ReasonOfAction;

            //DoubleEntryAmt = CassetteAmt + RetractedAmt;
            // FIRST DOUBLE ENTRY 
            if (HybridRepl == false)
            {
                WActionId = "26"; // 26_CREDIT CIT Account/DR_AtmCash (DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";

                //DoubleEntryAmt = Na.Balances1.CountedBal;
                WMaker_ReasonOfAction = "UnLoad Deposits";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                WActionId = "26";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }


            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "37";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            
            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT excess account ");
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT shortage account ");
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }
                if (HybridRepl == true)
                {
                    WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            //WTotalForCust = SM.TotalForCust;
            //WTotalCommision = SM.TotalCommision;
            //if (HybridRepl == false)
            //{
            //    if (WTotalForCust > 0)
            //    {
            //        // CREATE TRANSACTIONS FOR FOREX 
            //        DoubleEntryAmt = WTotalForCust;
            //        // FIRST DOUBLE ENTRY 
            //        WActionId = "33"; // 33_CREDIT_FOREX_INTERMEDIARY/DR_ATM CASH
            //                          // WUniqueRecordIdOrigin = "Replenishment";
            //        WUniqueRecordId = WSesNo; // SesNo 
            //        WCcy = "EGP";
            //        //DoubleEntryAmt = Na.Balances1.CountedBal;
            //        WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                              WActionId, WUniqueRecordIdOrigin,
            //                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                              , WMaker_ReasonOfAction, "Replenishment");


            //        TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //        // FOREX
            //        // FOREX 
            //        // FOREX
            //        if (WTotalCommision > 0)
            //        {
            //            // CREATE TRANSACTIONS FOR FOREX Commision 
            //            DoubleEntryAmt = WTotalCommision;
            //            // FIRST DOUBLE ENTRY 
            //            WActionId = "34"; // 34_CREDIT_FOREX_INTERMEDIARY/DR_Commision
            //                              // WUniqueRecordIdOrigin = "Replenishment";
            //            WUniqueRecordId = WSesNo; // SesNo 
            //            WCcy = "EGP";
            //            //DoubleEntryAmt = Na.Balances1.CountedBal;
            //            WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                  WActionId, WUniqueRecordIdOrigin,
            //                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                  , WMaker_ReasonOfAction, "Replenishment");


            //            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //        }

            //        //// CREATE TRANSACTIONS FOR FOREX CIT
            //        //DoubleEntryAmt = WTotalForCust + WTotalCommision;
            //        //// FIRST DOUBLE ENTRY 
            //        //WActionId = "35"; // 35_CREDIT_CIT ACCOUNT GL/DR_Forex_Intermidiary(DEPOSITS)
            //        //                  // WUniqueRecordIdOrigin = "Replenishment";
            //        //WUniqueRecordId = WSesNo; // SesNo 
            //        //WCcy = "EGP";
            //        ////DoubleEntryAmt = Na.Balances1.CountedBal;
            //        //WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //        //                                      WActionId, WUniqueRecordIdOrigin,
            //        //                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //        //                                      , WMaker_ReasonOfAction, "Replenishment");


            //        //TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //    }
            //}

        }

        public void CreateActions_Occurances_Load(string InAtmNo, int InSesNo , decimal InCashInAmount, decimal InDiffGL)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo; 

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            // Make transaction if CIT
            if (HybridRepl == false)
            {
                // Create 
                // load transaction for CIT and Bank

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WActionId;
                // string WUniqueRecordIdOrigin ;
                int WUniqueRecordId;
                string WCcy;
                decimal DoubleEntryAmt;

                // FIRST DOUBLE ENTRY 
                WActionId = "24"; // 24_CREDIT CIT Account/DR_AtmCash (LOAD)

                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InCashInAmount;
                string WMaker_ReasonOfAction = "Load ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;

            }

        }
        // Totals after updating
        int WTotal11;
        int WTotalAA;
        int WTotal10;
        int WTotal01;
        private void TotalsAfterValidation()
        {

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            WTotal11 = 0 ;
            WTotalAA = 0;
            WTotal10 = 0;
            WTotal01 = 0;

            int TempMode = 1;
            string SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND LoadingExcelCycleNo =" + WLoadingCycle;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_Totals(WOperator, WSignedId,
                                                       SelectionCriteria, TempMode);
            if ((G4.Total11 + G4.TotalAA + G4.Total01) > 0)
            {

            }
            else
            {

            }

            //textBoxTotal11.Text = G4.TotalNotProcessed.ToString();
            //WTotal11 = Cec.ValidInExcelRecords = G4.Total11;
            //WTotalAA = Cec.InvalidInExcelRecords = G4.TotalAA;
            //WTotal10 = Cec.NotInBank = G4.Total10;

            //Cec.ShortFound = G4.TotalShort;

            //Cec.PresenterDiff = G4.TotalPresenter;

            //// TotalPresenterAmt = 0;
            //Cec.ShortAmt = G4.TotalShortAmt;

            SelectionCriteria = " WHERE CITId = '" + WCitId + "' AND ProcessMode = -2  ";
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            //Cec.NotInG4S = G4.DataTableG4SEntries.Rows.Count;

        }

        //
        // Catch Details
        //
        private static void CatchDetails(Exception ex)
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

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }
        }
    }
}

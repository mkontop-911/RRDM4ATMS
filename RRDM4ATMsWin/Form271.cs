using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;

namespace RRDM4ATMsWin
{
    public partial class Form271 : Form
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

        RRDMReconcCategoriesMatchingSessions Rs = new RRDMReconcCategoriesMatchingSessions(); 

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMNotesBalances Na = new RRDMNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();


        //       string WUserBankId;

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

            InitializeComponent();

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserBankId = Us.Operator;
            // ========================================================

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            if (WCategory == "EWB110")
            {
                label2.Text = "Latest Repl Cycle";
            }
            else
            {
                label2.Text = "Latest RM Cycle"; 
            }

            labelATMno.Text = WCategory;
            labelSessionNo.Text = WRMCycle.ToString();

            // STEPLEVEL

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.StepLevel = 0;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //------------------------------------------------------------

            bool Reject = false;

            // View only 
            if (WViewFunction == true)  // Viewing only ,,,  Cycle workflow had finished and we want to view work 
            {
                // Go to Step A 

                //Start with STEP 1
                UCForm271a Step271a = new UCForm271a();

                Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.UCForm271aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                Step271a.SetScreen();
                textBoxMsgBoard.Text = Step271a.guidanceMsg;

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
                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WCategory, WRMCycle, "ReconciliationCat");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (WRequestor == true & Ap.Stage == 4 & Ap.AuthDecision == "NO") Reject = true;
                }

                UCForm271d Step271d = new UCForm271d();

                Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271d, 0, 0);
                Step271d.Dock = DockStyle.Fill;
                Step271d.UCForm271dPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                textBoxMsgBoard.Text = Step271d.guidanceMsg;

                // At this point authorisation record had closed if reject

                Ap.GetMessageOne(WCategory, WRMCycle, "ReconciliationCat" , WAuthoriser, WRequestor, Reject);

                textBoxMsgBoard.Text = Ap.MessageOut;

                labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep4.ForeColor = Color.White;
                
                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 16, FontStyle.Bold);

                // Step to 4
                StepNumber = 4; 

                //   ViewWorkFlow = true;

                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;
            }

            if (Us.ProcessNo == 2 || Us.ProcessNo == 5) // Follow Reconciliation Workflow - NORMAL 2 is for single and 5 for bulk 
            {

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                // START RECONCILIATION 
                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycle); 
                Rs.StartReconcDtTm = DateTime.Now; 
                Rs.UpdateCategRMCycleWithReconStartDate(WCategory, WRMCycle); 
                // UPDATE OLD ERRORS WITH MAIN ONLY 
                // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
                Ec.UpdateOldErrorsWithMainOnly(WOperator, WCategory, WRMCycle);

                //Start with STEP 1
                UCForm271a Step271a = new UCForm271a();

                Step271a.ChangeBoardMessage +=Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.UCForm271aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                Step271a.SetScreen();
                textBoxMsgBoard.Text = Step271a.guidanceMsg;

                if (Us.ProcessNo == 2 || Us.ProcessNo == 5) textBoxMsgBoard.Text = "Review and go to Next";
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

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (StepNumber == 1)
            {
                // STEPLEVEL// Check Feasibility to move to next step 

                //if (Us.StepLevel < 1)
                //{
                //    MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                //    return;
                //}

                //GetMainBodyImage("UCForm271a");

               

                UCForm271b Step271b = new UCForm271b();

                Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;

                Step271b.Dock = DockStyle.Fill;

                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle, 1);
                Step271b.SetScreen();
                textBoxMsgBoard.Text = Step271b.guidanceMsg;

                if (Us.ProcessNo == 2 || Us.ProcessNo == 5) textBoxMsgBoard.Text = "Examine Exceptions and Create Meta Exceptions.";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                labelStep1.ForeColor = labelStep3.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep2.ForeColor = Color.White;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 16, FontStyle.Bold);

                StepNumber++;
                buttonBack.Visible = true;
                buttonNext.Visible = true;

                if (Us.ProcessNo == 5)
                {
                    StepNumber++;
                }
            }
            else
            {
                if (StepNumber == 2)
                {
                    UCForm271c Step271c = new UCForm271c();

                    Step271c.ChangeBoardMessage += Step271c_ChangeBoardMessage;

                    Step271c.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step271c, 0, 0);
                    Step271c.UCForm271cPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                    Step271c.SetScreen();
                    textBoxMsgBoard.Text = Step271c.guidanceMsg;

                    if (Us.ProcessNo == 2 || Us.ProcessNo == 5) textBoxMsgBoard.Text = "Apply Meta Exceptions as needed.";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep2.ForeColor = labelStep4.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep1.Font.Style);
                    labelStep3.ForeColor = Color.White;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 16, FontStyle.Bold);

                    StepNumber++;
                    buttonBack.Visible = true;
                    buttonNext.Visible = true;

                    if (Us.ProcessNo == 5)
                    {
                        buttonNext.Visible = true;
                        buttonNext.Text = "Finish";
                    }

                }
                else
                {
                    if (StepNumber == 3)
                    {
                        //// STEPLEVEL// Check Feasibility to move to next step 

                        if (Us.StepLevel == 3 & Us.ProcessStatus > 1)
                        {
                            if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                  == DialogResult.Yes)
                            {
                                // Application.Exit();
                            }
                            else return;
                        }
                        

                        //if (Us.ProcessNo == 5)
                        //{
                        //    this.Close();

                        //    return;
                        //}

                        UCForm271d Step271d = new UCForm271d();

                        //Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;

                        Step271d.ChangeBoardMessage += Step271d_ChangeBoardMessage;
                       
                        Step271d.Dock = DockStyle.Fill;
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                        tableLayoutPanelMain.Controls.Add(Step271d, 0, 0);

                        Step271d.UCForm271dPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                        textBoxMsgBoard.Text = Step271d.guidanceMsg;

                        if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Complete the Reconciliation Process";

                        if (WViewFunction == true) textBoxMsgBoard.Text = "View only";

                        labelStep3.ForeColor = labelStep2.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep4.ForeColor = Color.White;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 16, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Text = "Finish";
                        buttonNext.Visible = true;
                    }
                    else
                    {
                        if (StepNumber == 4) // FINISH BUTTON WAS PUSHED - FINAL WORK IS DONE HERE 
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

                            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WCategory, WRMCycle, "ReconciliationCat");
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

                            if (WRequestor == true & ReconciliationAuthorDone == false) // Coming from authoriser and authoriser not done 
                            {
                                this.Close();
                                return;
                            }

                            if (Us.ProcessNo == 2 & ReconciliationAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
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

                            if (Us.ProcessNo == 2 & ReconciliationAuthorOutstanding == true) // Cancel with repl outstanding 
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

                            if ((Us.ProcessNo == 2 || WRequestor == true) & ReconciliationAuthorDone == true) // Everything is fined .
                            {

                            }

                            //
                            //**************************************************************************
                            //**************************************************************************
                            // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
                            //**************************************************************************

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

                                Ec.ReadAllErrorsTableForPostingTrans(WOperator, WCategory, "", WSignedId, Ap.Authoriser, WRMCycle);

                                // *************************************************************************
                                // Reads Trans To be posted and creates Posted Trans to be considered later 
                                // *************************************************************************
                                RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

                                int ActionCd2 = 6; // Create Trans To be posted without closing the Trans To be posted
                                                   // With = 4 this is closed 
                                // Call Class 

                                Tc.ReadTransToBePostedAllAndCreatePostedTrans(WSignedId, WOperator, ActionCd2);
                                if (Tc.RecordFound == true)
                                {
                                    //MessageBox.Show("Total number of created transactions = " + Tc.TotTransactions.ToString()); 
                                }
                                else
                                {
                                    MessageBox.Show("No open trans to be posted available");
                                    return;
                                }
                                //**************************************************************************
                                //**************************************************************************

                                //Ec.ReadAllErrorsTableForUpdatingTheUnMatchedTrans(WOperator, WCategory, WRMCycle, WSignedId, Ap.Authoriser);

                                //
                                // Update RMCycle 

                                Ec.ReadAllErrorsTableForCounters(WOperator, WCategory, "");
                                //
                                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycle);

                                Rs.RemainReconcExceptions = Ec.NumOfErrors;

                                Rs.EndReconcDtTm = DateTime.Now;

                                Rs.UpdateCategRMCycleWithAuthorClosing(WCategory, WRMCycle);

                                //
                                Rc.ReadReconcCategorybyCategId(WOperator, WCategory);

                                Rc.ReconcDtTm = DateTime.Now;

                                if (Rs.RemainReconcExceptions > 0 || (Rs.NumberOfUnMatchedRecs - Rs.SettledUnMatched) > 0)
                                {
                                    Rc.ReconcStatus = "Category Has Outstanding Differences";
                                }
                                else
                                {
                                    Rc.ReconcStatus = "Category Has No Differences";
                                }

                                string SearchingStringLeft = "Operator ='" + WOperator
                                             + "' AND RMCateg ='" + WCategory + "' AND OpenRecord = 1  ";

                                string WhatFile = "UnMatched";
                                string WSortValue = "SeqNo";
                                Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, SearchingStringLeft, WhatFile, WSortValue);

                                Rc.OutstandingUnMatched = Rm.TotalSelected;

                                Rc.UpdateCategory(WOperator, WCategory);

                                //MessageBox.Show("No transactions to be posted");
                                Form2 MessageForm = new Form2("Reconciliation Workflow has finished for: " + WCategory + Environment.NewLine
                                                               + "Transactions to be posted were created as a result" + Environment.NewLine
                                                               );
                                MessageForm.ShowDialog();

                    
                                this.Close();

                                return;

                            }

                          
                        }

                    }

                }

               

            }
            if (StepNumber == 1 & Us.ProcessNo == 8)
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

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (StepNumber == 2)
            {
                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                UCForm271a Step271a = new UCForm271a();
                Step271a.ChangeBoardMessage += Step271a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step271a, 0, 0);
                Step271a.Dock = DockStyle.Fill;
                Step271a.UCForm271aPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                Step271a.SetScreen();
                textBoxMsgBoard.Text = Step271a.guidanceMsg;

                if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
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

                    UCForm271b Step271b = new UCForm271b();
                    Step271b.ChangeBoardMessage += Step271b_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step271b, 0, 0);
                    Step271b.Dock = DockStyle.Fill;
                    Step271b.UCForm271bPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle,1);
                    Step271b.SetScreen();
                    textBoxMsgBoard.Text = Step271b.guidanceMsg;

                    if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
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

                        UCForm271c Step271c = new UCForm271c();
                        Step271c.ChangeBoardMessage += Step271c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step271c, 0, 0);
                        Step271c.Dock = DockStyle.Fill;
                        Step271c.UCForm271cPar(WSignedId, WSignRecordNo, WOperator, WCategory, WRMCycle);
                        Step271c.SetScreen();
                        textBoxMsgBoard.Text = Step271c.guidanceMsg;

                        if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep4.ForeColor = labelStep3.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep3.Font.Style);
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

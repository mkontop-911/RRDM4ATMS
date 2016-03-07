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
    public partial class Form71 : Form
    {
        Bitmap SCREENa;
        Bitmap SCREENb1;
        Bitmap SCREENb2;

        //bool ExistanceOfDiff;
        bool OneBalanceSet; 

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

      //  bool RecordFound;
        //     bool SeriousMsg = false;
       
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMNotesBalances Na = new RRDMNotesBalances (); 

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess(); 




 //       string WUserBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
  
        string WAtmNo;
        int WSesNo;

        public Form71(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
        
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            // ================USER BANK =============================
       //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
        //    WUserBankId = Us.Operator;
            // ========================================================

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelATMno.Text = WAtmNo;
            labelSessionNo.Text = WSesNo.ToString();

            // STEPLEVEL

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.StepLevel = 0;
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WProcess = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 


            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WProcess); // CALL TO MAKE BALANCES AVAILABLE 


            if (Na.BalSets == 1)
            {
                OneBalanceSet = true;
                labelStep2.Text = "Take Actions"; 
            }
            else
            {
                OneBalanceSet = false; 
            }

            // Initialise Existance of Difference 
            //

            bool ExistanceOfDiff = false; 

            if (Na.BalDiff1.AtmLevel == true || Na.BalDiff1.HostLevel == true || Na.Balances1.ErrOutstanding > 0)
            {
              
                ExistanceOfDiff = true;
                
            }
            else
            {
               
                ExistanceOfDiff = false;
              
            }

            if (ExistanceOfDiff == true)
            {
            }


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
                UCForm71a Step71a = new UCForm71a();

                Step71a.ChangeBoardMessage += Step71a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step71a, 0, 0);
                Step71a.Dock = DockStyle.Fill;
                Step71a.UCForm71aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step71a.SetScreen();
                textBoxMsgBoard.Text = Step71a.guidanceMsg;
              
                textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;

                StepNumber = 1;

             //   ViewWorkFlow = true;

            }
        
            // Coming from Authoriser OR Requestor 
            if (WAuthoriser == true || WRequestor == true )  // 55: Coming from authoriser , 56: Coming from requestor 
            {
               // READ AND UPDATE REJECT BEFORE AUTH RECORD CLOSES 

                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Reconciliation");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (WRequestor == true & Ap.Stage == 4 & Ap.AuthDecision == "NO") Reject = true; 
                }

                UCForm71c Step71c = new UCForm71c();

                Step71c.ChangeBoardMessage += Step71c_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step71c, 0, 0);
                Step71c.Dock = DockStyle.Fill;
                Step71c.UCForm71cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                textBoxMsgBoard.Text = Step71c.guidanceMsg;

                // At this point authorisation record had closed if reject

                Ap.GetMessageOne(WAtmNo, WSesNo, "Reconciliation", WAuthoriser, WRequestor, Reject);

                textBoxMsgBoard.Text = Ap.MessageOut;

                labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep3.ForeColor = Color.White;
                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 22, FontStyle.Bold);

                StepNumber = 3;

             //   ViewWorkFlow = true;

                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;
            }

            if (Us.ProcessNo == 2 || Us.ProcessNo == 5) // Follow Reconciliation Workflow - NORMAL 2 is for single and 5 for bulk 
            {
               
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                // START RECONCILIATION 
                Ta.UpdateTracesStartReconc(WOperator, WSignedId, WAtmNo, WSesNo); // UPDATE TRACES THAT RECONCILIATION HAD STARTED
                // UPDATE OLD ERRORS WITH MAIN ONLY 
                // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
                Ec.UpdateOldErrorsWithMainOnly(WOperator, WAtmNo, WSesNo);

                //Start with STEP 1
                UCForm71a Step71a = new UCForm71a();

                Step71a.ChangeBoardMessage += Step71a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step71a, 0, 0);
                Step71a.Dock = DockStyle.Fill;
                Step71a.UCForm71aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step71a.SetScreen();
                textBoxMsgBoard.Text = Step71a.guidanceMsg;

                if (Us.ProcessNo == 2 || Us.ProcessNo == 5) textBoxMsgBoard.Text = "Review and go to Next";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;
                buttonNext.Text = "Next>"; 
            }
        }

        void Step71a_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).guidanceMsg;
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
            //Us.WFieldChar1 = "ViewFromBULK"; 
            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (StepNumber == 1 )
            {
                // STEPLEVEL// Check Feasibility to move to next step 

                //if (Us.StepLevel < 1)
                //{
                //    MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                //    return;
                //}

                GetMainBodyImage("UCForm71a");

                if (OneBalanceSet == false) // More than one Balance sets
                {

                    UCForm71b1 Step71b1 = new UCForm71b1();

                    Step71b1.ChangeBoardMessage += Step71b1_ChangeBoardMessage;

                    Step71b1.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step71b1, 0, 0);
                    Step71b1.UCForm71b1Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    Step71b1.SetScreen();
                    textBoxMsgBoard.Text = Step71b1.guidanceMsg;

                    //   if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "View and go to reconciliation if needed";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep1.ForeColor = labelStep3.ForeColor;
                    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);
                }

                if (OneBalanceSet == true) // More than one Balance sets
                {

                    UCForm71b2 Step71b2 = new UCForm71b2();

                    Step71b2.ChangeBoardMessage += Step71b2_ChangeBoardMessage;

                    Step71b2.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    tableLayoutPanelMain.Controls.Add(Step71b2, 0, 0);
                    Step71b2.UCForm71b2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    Step71b2.SetScreen();
                    textBoxMsgBoard.Text = Step71b2.guidanceMsg;

                    if (Us.ProcessNo == 2 || Us.ProcessNo == 5) textBoxMsgBoard.Text = "Apply MetaExceptions";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) 
                    {
                        textBoxMsgBoard.Text = "View only";
                        labelStep2.Text = "Taken Actions"; 
                    } 

                    labelStep1.ForeColor = labelStep3.ForeColor;
                    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);
                    
                }


                StepNumber++;
                buttonBack.Visible = true;
                buttonNext.Visible = true;
                
                if (Us.ProcessNo == 5 || Us.WFieldChar1 == "ViewFromBULK")
                {
                    buttonNext.Visible = true;
                    buttonNext.Text = "Finish";
                    buttonBack.Visible = false; 
                }
            }
            else
            {

                if (StepNumber == 2)
                {
                    //// STEPLEVEL// Check Feasibility to move to next step 

                    //if (Us.StepLevel < 2)
                    //{
                    //    MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                    //    return;
                    //}

                    if (Us.StepLevel == 2 & Us.ProcessStatus>1)
                    {
                        if (MessageBox.Show("You Still have differences...Are you sure you want to move to next step?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                              == DialogResult.Yes)
                        {
                            // Application.Exit();
                        }
                        else return; 
                    }            

                    GetMainBodyImage("UCForm71b");
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    if (Us.ProcessNo == 5 || Us.WFieldChar1 == "ViewFromBULK")
                    {

                        this.Close();

                        return; 
                    }

                    UCForm71c Step71c = new UCForm71c();
                
                    Step71c.ChangeBoardMessage += Step71c_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step71c, 0, 0);
                    Step71c.Dock = DockStyle.Fill;
                    Step71c.UCForm71cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    textBoxMsgBoard.Text = Step71c.guidanceMsg;

                    if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Complete the Reconciliation Process";
                   
                    if (WViewFunction == true ) textBoxMsgBoard.Text = "View only";

                    labelStep2.ForeColor = labelStep1.ForeColor;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                    labelStep3.ForeColor = Color.White;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 22, FontStyle.Bold);

                    StepNumber++;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Finish"; 
                    buttonNext.Visible = true;
                }
                else
                {
                    if (StepNumber == 3) // FINISH BUTTON WAS PUSHED - FINAL WORK IS DONE HERE 
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
                        
                            // CHECK IF  ELECTRONIC AUTHORISATION IS NEEDED
                            string ParId = "262";
                            string OccurId = "1";
                            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                            if (Gp.OccuranceNm == "YES") // Electronic authorization needed  
                            {
                              //  ReconciliationAuthorNeeded = true;

                                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Reconciliation");
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
                                    MessageBox.Show("MSG946 - Authorisation outstanding");
                                    return; 
                                    //if (MessageBox.Show("MSG946 - You are cancelling the Reconciliation workflow without Authorisation." + Environment.NewLine
                                    //                 + " Do you want to cancel it? ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    //               == DialogResult.Yes)
                                    //{
                                    //    this.Close();
                                    //    return;

                                    //}
                                    //else
                                    //{
                                    //    return;
                                    //}

                                }

                                if (Us.ProcessNo == 2 & ReconciliationAuthorOutstanding == true) // Cancel with repl outstanding 
                                {

                                    MessageBox.Show("MSG946 - Authorisation outstanding");
                                    return; 

                                  

                                }

                                if (WAuthoriser == true & ReconciliationAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
                                {
                                    MessageBox.Show("MSG946 - Authorisation outstanding");
                                    return; 
                                  

                                }

                                if ((Us.ProcessNo == 2 || WRequestor == true) & ReconciliationAuthorDone == true) // Everything is fined .
                                {
                                   

                                }

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
                            }   

                     
                        // Create the transactions as a result of actions taken on Errors
                        //
                        // THE BELOW METHOD CREATES THE TRANSACTIONS TO BE POSTED 
                            Ec.ReadAllErrorsTableForPostingTrans(WOperator, "EWB110", WAtmNo, WSignedId, Ap.Authoriser, WSesNo); 
                        //
                        // Update Traces at Finish
                        //
                        // UPDATE TRACES WITH FINISH RECONC
                        int Mode = 2; // After reconciliation 
                        Ta.UpdateTracesFinishReplOrReconc(WAtmNo, WSesNo, WSignedId, Mode);

                        // READ Ta to see if differences AND SEND EMAIL ALERTS

                        Ta.ReadSessionsStatusTraces(WAtmNo, Ta.LastReplCyclId);

                        // READ TO UPDATE AM
                        Am.ReadAtmsMainSpecific(WAtmNo); // 

                        if (Ta.Recon1.DiffReconcEnd == true)
                        {
                            Am.ReconcDiff = true;
                        }
                        else
                        {
                            Am.ReconcDiff = false;
                        }

                        Am.SessionsInDiff = Ta.SessionsInDiff;

                        Ec.ReadAllErrorsTableForCounters(WOperator, "EWB110", WAtmNo);
                        Am.ErrOutstanding = Ec.NumOfErrors;

                        Am.ReconcCycleNo = Ta.SesNo;
                        Am.ReconcDt = Ta.Recon1.RecFinDtTm;
                            // Register the amount 
                        Am.CurrNm1 = Ta.Diff1.CurrNm1;
                        Am.DiffCurr1 = Ta.Diff1.DiffCurr1;

                        Am.LastUpdated = DateTime.Now;

                        if (Am.CurrentSesNo == Ta.SesNo)
                        {
                            Am.ProcessMode = Ta.ProcessMode;
                        }

                        Am.UpdateAtmsMain(WAtmNo); 

                        if (Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == true)
                        {
                            // There differences to reconcile

                            //  ReconcComment = "EVERYTHING INCLUDING HOST FILES RECONCILE";
                        }

                        if (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == true)
                        {
                            // There differences to reconcile

                            //  ReconcComment = "NEED TO GO TO RECONCILIATION PROCESS";
                        }

                        if (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == false)
                        {
                            // There differences to reconcile

                            //    ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
                        }

                        if (Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == false)
                        {
                            // There differences to reconcile

                            //  ReconcComment = "HOST FILES NOT AVAILABLE YET";
                        }


                        MessageBox.Show(" Reconciliation Workflow has finished for: " + WAtmNo  +Environment.NewLine
                                                         +" Move to next reconciliation if any. ");
                        this.Close();

                        return; 

                      //  GetMainBodyImage("UCForm71c");

                      //  Form56 Report1 = new Form56(WSignedId, WSignRecordNo, WBankId, WPrive, WAtmNo, WSesNo, SCREENa, SCREENb, SCREENc);
                      //  Report1.Show();

                    }
                    else
                    {
                        if (StepNumber == 4)
                        {
                            // STEPLEVEL// Check Feasibility to move to next step 

                            Us.ReadSignedActivityByKey(WSignRecordNo);

                            if (Us.StepLevel < 4)
                            {
                                MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
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
        void Step71_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).guidanceMsg;
        }


        void Step71b1_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71b1)tableLayoutPanelMain.Controls["UCForm71b1"]).guidanceMsg;
        }

        void Step71b2_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71b2)tableLayoutPanelMain.Controls["UCForm71b2"]).guidanceMsg;
        }

        void Step71c_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm71c)tableLayoutPanelMain.Controls["UCForm71c"]).guidanceMsg;
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

                UCForm71a Step71a = new UCForm71a();
                Step71a.ChangeBoardMessage += Step71_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step71a, 0, 0);
                Step71a.Dock = DockStyle.Fill;
                Step71a.UCForm71aPar(WSignedId, WSignRecordNo, WOperator,  WAtmNo, WSesNo);
                Step71a.SetScreen();
                textBoxMsgBoard.Text = Step71a.guidanceMsg;

                if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                labelStep2.ForeColor = labelStep3.ForeColor;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                labelStep1.ForeColor = Color.White;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 22, FontStyle.Bold);
       //         textBoxMsgBoard.Text = "INSERT DATA AND UPDATE";
                StepNumber--;
                buttonBack.Visible = false;
            }
            else
            {

                if (StepNumber == 3)
                {
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    if(OneBalanceSet == false)
                    {
                    UCForm71b1 Step71b1 = new UCForm71b1();
                    Step71b1.ChangeBoardMessage += Step71b1_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step71b1, 0, 0);
                    Step71b1.Dock = DockStyle.Fill;
                    Step71b1.UCForm71b1Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    Step71b1.SetScreen();
                    textBoxMsgBoard.Text = Step71b1.guidanceMsg;

                    if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                    if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                    labelStep3.ForeColor = labelStep2.ForeColor;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);
                    }

                    if (OneBalanceSet == true)
                    {
                        UCForm71b2 Step71b2 = new UCForm71b2();
                        Step71b2.ChangeBoardMessage += Step71b2_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step71b2, 0, 0);
                        Step71b2.Dock = DockStyle.Fill;
                        Step71b2.UCForm71b2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        Step71b2.SetScreen();
                        textBoxMsgBoard.Text = Step71b2.guidanceMsg;

                        if (Us.ProcessNo == 2) textBoxMsgBoard.Text = "Review Information";
                        if (WViewFunction == true || WAuthoriser == true || WRequestor == true) textBoxMsgBoard.Text = "View only";

                        labelStep3.ForeColor = labelStep2.ForeColor;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                        labelStep2.ForeColor = Color.White;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);
                    }

                    StepNumber--;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Next >";
                }
                else
                {
                    if (StepNumber == 4)
                    {
                       
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
   
        private void GetMainBodyImage(String ControlName)
        {
            System.Drawing.Bitmap memoryImage;

            if (ControlName.Equals("UCForm71a"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).Width, ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).Height);
                ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).DrawToBitmap(memoryImage, ((UCForm71a)tableLayoutPanelMain.Controls["UCForm71a"]).ClientRectangle);
                SCREENa = memoryImage;
            }
            if (ControlName.Equals("UCForm71b1"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm71b1)tableLayoutPanelMain.Controls["UCForm71b1"]).Width, ((UCForm71b1)tableLayoutPanelMain.Controls["UCForm71b1"]).Height);
                ((UCForm71b1)tableLayoutPanelMain.Controls["UCForm71b1"]).DrawToBitmap(memoryImage, ((UCForm71b1)tableLayoutPanelMain.Controls["UCForm71b1"]).ClientRectangle);
                SCREENb1 = memoryImage;
            }

            if (ControlName.Equals("UCForm71b2"))
            {
                memoryImage = new System.Drawing.Bitmap(((UCForm71b2)tableLayoutPanelMain.Controls["UCForm71b2"]).Width, ((UCForm71b2)tableLayoutPanelMain.Controls["UCForm71b2"]).Height);
                ((UCForm71b2)tableLayoutPanelMain.Controls["UCForm71b2"]).DrawToBitmap(memoryImage, ((UCForm71b2)tableLayoutPanelMain.Controls["UCForm71b2"]).ClientRectangle);
                SCREENb2 = memoryImage;
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

    }
}

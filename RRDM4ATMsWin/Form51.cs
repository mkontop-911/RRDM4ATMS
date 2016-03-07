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
    public partial class Form51 : Form
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
    //    bool Reject; 

    //    int NoOfsteps = 5; 
        
        int StepNumber = 1;

        bool NoDeposits; 

        bool ViewWorkFlow;

        bool ReplenishmentAuthorNeeded;
 
        bool ReplenishmentAuthorNoRecordYet;
        bool ReplenishmentAuthorOutstanding;
        bool ReplenishmentAuthorDone;


        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMNotesBalances Na = new RRDMNotesBalances();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess(); 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WAtmNo;
        int WSesNo;

        public Form51(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // ================USER BANK =============================
            //     Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //    WUserOperator = Us.Operator;
            // ========================================================

            labelATMno.Text = WAtmNo;
            labelSessionNo.Text = WSesNo.ToString();

            // STEPLEVEL

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.StepLevel = 0;
            Us.ReplStep1_Updated = false;
            Us.ReplStep2_Updated = false;
            Us.ReplStep3_Updated = false;
            Us.ReplStep4_Updated = false;
            Us.ReplStep5_Updated = false; 

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

            // Check for DEPOSITS function for this ATM
            Ac.ReadAtm(WAtmNo);
            if (Ac.ChequeReader == false & Ac.DepoReader == false & Ac.EnvelopDepos == false) // NO DEPOSITS
            {
                NoDeposits = true;
            }

            if (Us.ProcessNo == 8 || Us.ProcessNo == 9)  // Follow Showing Money in 
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
                labelStep4.ForeColor = Color.White;
                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                if (Us.ProcessNo == 9)
                {
                    labelStep4.Text = "In Money are shown for action";
                }

                buttonNext.Visible = true;

                buttonNext.Text = "Finish";

                // SHOW labelStep4 and hide the rest. 
                // Hide the rest

                labelStep1.Hide();
                labelStep2.Hide();
                labelStep3.Hide();
                labelStep5.Hide();

                label5.Hide();
                label4.Hide();
                label9.Hide();
                label11.Hide();              

            }
            // View only 
            // View only 
            if (WViewFunction == true)  // Viewing only ,,, Repl Cycle workflow had finished and we want t
            {

                // Go to Step A 

                UCForm51a Step51a = new UCForm51a();

                Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                Step51a.Dock = DockStyle.Fill;
                Step51a.UCForm51aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51a.SetScreen();
                textBoxMsgBoard.Text = Step51a.guidanceMsg;
               
                textBoxMsgBoard.Text = "View only";

                buttonNext.Visible = true;

                StepNumber = 1;

                ViewWorkFlow = true; 

            }
            // Coming from Authoriser OR Requestor 
            if (WAuthoriser == true || WRequestor == true)  // 55: Coming from authoriser , 56: Coming from requestor 
            {
                // READ AND UPDATE REJECT BEFORE AUTH RECORD CLOSES 

                Reject = false;
                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (WRequestor == true & Ap.Stage == 4 & Ap.AuthDecision == "NO") Reject = true;
                }
            
                UCForm51f Step51f = new UCForm51f();
                Step51f.ChangeBoardMessage += Step51f_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51f, 0, 0);
                Step51f.Dock = DockStyle.Fill;
                Step51f.UCForm51fPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                textBoxMsgBoard.Text = Step51f.guidanceMsg;

                // At this point authorisation record had closed if reject

                Ap.GetMessageOne(WAtmNo, WSesNo, "Replenishment", WAuthoriser, WRequestor, Reject);

                textBoxMsgBoard.Text = Ap.MessageOut;

                labelStep1.ForeColor = labelStep2.ForeColor;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                labelStep6.ForeColor = Color.White;
                labelStep6.Font = new Font(labelStep6.Font.FontFamily, 22, FontStyle.Bold);

                buttonBack.Visible = true;
                buttonNext.Text = "Finish";
                buttonNext.Visible = true;

                StepNumber = 6 ;

                ViewWorkFlow = true;

            }

            if (Us.ProcessNo == 5 || Us.ProcessNo == 31 || Us.ProcessNo == 26) Us.ProcessNo = 1; 

            if (Us.ProcessNo == 1 ) // Follow Replenishment Workflow
                //if (Us.ProcessNo == 1 || Us.ProcessNo == 5 || Us.ProcessNo == 31 || Us.ProcessNo == 26) // Follow Replenishment Workflow
            {
                // REPLENISHMENT STARTS

                Ta.UpdateTracesStartRepl(WSignedId, WAtmNo, WSesNo);

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //Start with STEP 1
                UCForm51a Step51a = new UCForm51a();
                // Stavros
                Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                Step51a.Dock = DockStyle.Fill;
                Step51a.UCForm51aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51a.SetScreen();
                textBoxMsgBoard.Text = Step51a.guidanceMsg;

                buttonNext.Visible = true;
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
            if ((Us.ProcessNo == 8 || Us.ProcessNo == 9) & buttonNext.Text == "Finish")
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

            Us.ReadSignedActivityByKey(WSignRecordNo);

            //StepNumber = Us.StepLevel; 

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
            else ViewWorkFlow = false;

            try
            {

                if (StepNumber == 1 & (Us.ProcessNo == 1 || ViewWorkFlow == true )) // Replenishment for 1= individual, 5 = Group 26: CIT individual , 31:CIT with Group 
                {
                    // STEPLEVEL// Check Feasibility to move to next step 

                    if (Us.ReplStep1_Updated == false & ViewWorkFlow == false)
                    {
                        MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                        return;
                    }

                    GetMainBodyImage("UCForm51a");


                    UCForm51b Step51b = new UCForm51b();

                    //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                    Step51b.ChangeBoardMessage += Step51b_ChangeBoardMessage;

                    Step51b.Dock = DockStyle.Fill;

                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                    Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    Step51b.SetScreen();
                    textBoxMsgBoard.Text = Step51b.guidanceMsg;

                    if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                    tableLayoutPanelMain.Controls.Add(Step51b, 0, 0);

                    labelStep1.ForeColor = labelStep3.ForeColor;
                    labelStep1.Font = new Font(labelStep1.Font.FontFamily, 10, labelStep1.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);

                    StepNumber++; // STEP becomes two
                    buttonBack.Visible = true;
                    buttonNext.Visible = true;

                }
                else
                {
                  
                    if (StepNumber == 2 & (Us.ProcessNo == 1 || ViewWorkFlow == true))
                    {

                        Us.ReadSignedActivityByKey(WSignRecordNo);

                        if (Us.ReplStep2_Updated == false & ViewWorkFlow == false)
                        {
                            MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                            return;
                        }

                    }

                    if (StepNumber == 2 & NoDeposits == true)
                    {
                        StepNumber = 3; // Jump if no deposits
                    } 

                    if (StepNumber == 2 & (Us.ProcessNo == 1 || ViewWorkFlow == true))
                    {
                        // STEPLEVEL// Check Feasibility to move to next step 
                        GetMainBodyImage("UCForm51b");
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm51c Step51c = new UCForm51c();
                        //   Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                        Step51c.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51c, 0, 0);
                        Step51c.Dock = DockStyle.Fill;
                        Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51c.guidanceMsg;

                        if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep2.ForeColor = labelStep1.ForeColor;
                        labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber++;
                        buttonBack.Visible = true;
                        buttonNext.Visible = true;

                        //if (Us.ProcessNo == 31 & NoDeposits == false || Us.ProcessNo == 26 & NoDeposits == false)
                        //{
                        //    buttonNext.Text = "Finish";
                        //}
                    }
                    else
                    {
                        if (StepNumber == 3 & (Us.ProcessNo == 1 || ViewWorkFlow == true))
                        {
                            // STEPLEVEL// Check Feasibility to move to next step 

                            Us.ReadSignedActivityByKey(WSignRecordNo);

                            if ((Us.ReplStep3_Updated == false & NoDeposits == false) & ViewWorkFlow == false)
                            {
                                // There are deposits and no updating 
                                MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                                return;
                            }


                                // THERE ARE DEPOSITS
                                if (NoDeposits == false)
                                {
                                    GetMainBodyImage("UCForm51c");
                                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                                }

                                // THERE ARE no DEPOSITS
                                if (NoDeposits == true)
                                {
                                    GetMainBodyImage("UCForm51b");
                                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();
                                }
                             //  CASH MANAGEMENT External?? 
                            string ParId = "202";
                            string OccurId = "1";
                            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                            if ((Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl != "50"))
                            {
                                // RRDM Cash Management is needed  
                                {
                                    // Cash Management is needed 
                                    UCForm51d Step51d = new UCForm51d();

                                    Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;
                                    tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                                    Step51d.Dock = DockStyle.Fill;
                                    Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                    textBoxMsgBoard.Text = Step51d.guidanceMsg;

                                    if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                    labelStep3.ForeColor = labelStep2.ForeColor;
                                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                                    labelStep4.ForeColor = Color.White;
                                    labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

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

                                tableLayoutPanelMain.Controls.Add(Step51d2, 0, 0);
                                Step51d2.Dock = DockStyle.Fill;
                                Step51d2.UCForm51d2Par(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);

                                textBoxMsgBoard.Text = Step51d2.guidanceMsg;

                                if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                labelStep2.ForeColor = labelStep1.ForeColor;
                                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                                labelStep3.ForeColor = labelStep1.ForeColor;
                                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                                labelStep4.ForeColor = Color.White;
                                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                                StepNumber++;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;
                            }

                        }
                        else
                        {
                            if (StepNumber == 4 & (Us.ProcessNo == 1 || ViewWorkFlow == true))
                            {
                                // STEPLEVEL// Check Feasibility to move to next step 

                                Us.ReadSignedActivityByKey(WSignRecordNo);

                                if (Us.ReplStep4_Updated == false & ViewWorkFlow == false)
                                {
                                    MessageBox.Show("YOU MUST UPDATE THIS STEP BEFORE YOU MOVE TO NEXT");
                                    return;
                                }
                                // CASH MANAGEMENT
                                string ParId = "202";
                                string OccurId = "1";
                                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                                // RRDM Cash Management is used                                         
                                if ((Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl != "50"))
                                {
                                                                   
                                    GetMainBodyImage("UCForm51d");
                                }
                                else // RRDM Cash Management is NOT Used 
                                {
                                    GetMainBodyImage("UCForm51d2");
                                }

                                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                UCForm51e Step51e = new UCForm51e();
                                Step51e.ChangeBoardMessage += Step51e_ChangeBoardMessage;
                                tableLayoutPanelMain.Controls.Add(Step51e, 0, 0);
                                Step51e.Dock = DockStyle.Fill;
                                Step51e.UCForm51ePar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                textBoxMsgBoard.Text = Step51e.guidanceMsg;

                                labelStep4.ForeColor = labelStep3.ForeColor;
                                labelStep4.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                                labelStep5.ForeColor = Color.White;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 22, FontStyle.Bold);

                                StepNumber++;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;

                                
                            }
                            else
                            {
                                //bool ReplenishmentAuthorNeeded;
                                //bool ReplenishmentAuthorNoRecordYet;
                                //bool ReplenishmentAuthorOutstanding;
                                //bool ReplenishmentAuthorDone;

                            

                                if (StepNumber == 5) // FINISH - Make validationsfor Authorisations  
                                {

                                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                                    UCForm51f Step51f = new UCForm51f();
                                    Step51f.ChangeBoardMessage += Step51f_ChangeBoardMessage;
                                    tableLayoutPanelMain.Controls.Add(Step51f, 0, 0);
                                    Step51f.Dock = DockStyle.Fill;
                                    Step51f.UCForm51fPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                    textBoxMsgBoard.Text = Step51f.guidanceMsg;

                                    if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                    labelStep5.ForeColor = labelStep4.ForeColor;
                                    labelStep5.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                                    labelStep6.ForeColor = Color.White;
                                    labelStep6.Font = new Font(labelStep6.Font.FontFamily, 22, FontStyle.Bold);

                                    StepNumber++;
                                    buttonBack.Visible = true;
                                    buttonNext.Visible = true;

                                    buttonNext.Text = "Finish";

                                }
                                else
                                {
                                    if (StepNumber == 6) // THE FINISH BUTTON WAS ACTIVATED
                                    {
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

                                            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "Replenishment");
                                            if (Ap.RecordFound == true)
                                            {
                                                ReplenishmentAuthorNoRecordYet = false;
                                                if (Ap.Stage == 3 || Ap.Stage == 4)
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

                                            if (Us.ProcessNo == 1 & ReplenishmentAuthorNoRecordYet == true) // Cancel by originator without request for authorisation 
                                            {
                                                MessageBox.Show("MSG946 - Authorisation outstanding");
                                                return;

                                            }

                                            if (Us.ProcessNo == 1 & ReplenishmentAuthorOutstanding == true) // Cancel with repl outstanding 
                                            {
                                                MessageBox.Show("MSG946 - Authorisation outstanding");
                                                return;

                                            }

                                            if (WAuthoriser == true & ReplenishmentAuthorOutstanding == true) // Cancel by authoriser without making authorisation.
                                            {
                                                MessageBox.Show("MSG946 - Authorisation outstanding");
                                                return;
                                               
                                            }

                                            if ((Us.ProcessNo == 1 || WRequestor == true) & ReplenishmentAuthorDone == true) // Everything is fined .
                                            {
                                                

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

                                            // Update authorisation record  

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

                                                Ec.CreateTransTobepostedfromReplenishment(WOperator, WAtmNo, WSesNo, WSignedId, Na.Balances1.CountedBal, Na.ReplAmountTotal);


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
                                            Ta.UpdateTracesFinishReplOrReconc(WAtmNo, WSesNo, WSignedId, Mode);

                                            // READ Ta to see if differences 

                                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                                            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                                            Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo);

                                            Am.ReadAtmsMainSpecific(WAtmNo);

                                            Am.LastReplDt = DateTime.Now;

                                            //TEST 
                                            if (WAtmNo == "AB102" || WAtmNo == "ServeUk102" || WAtmNo == "ABC501")
                                            {
                                                DateTime WDTm = new DateTime(2014, 02, 28);
                                                Am.LastReplDt = WDTm;
                                            }
                                            if (WAtmNo == "AB104" || WAtmNo == "ABC502")
                                            {
                                                DateTime WDTm = new DateTime(2014, 02, 13);
                                                Am.LastReplDt = WDTm;
                                            }


                                            if (Ta.Repl1.DiffRepl == true)
                                            {
                                                Am.ReconcDiff = true;
                                                // Register the amount 
                                                Am.CurrNm1 = Ta.Diff1.CurrNm1;
                                                Am.DiffCurr1 = Ta.Diff1.DiffCurr1;
                                            }
                                            else
                                            {
                                                Am.ReconcDiff = false;
                                                Am.CurrNm1 = "";
                                                Am.DiffCurr1 = 0;
                                            }


                                            Am.SessionsInDiff = Ta.SessionsInDiff;
                                            Am.ReplCycleNo = Ta.SesNo;

                                            Ec.ReadAllErrorsTableForCounters(WOperator,"EWB110" , WAtmNo);
                                            Am.ErrOutstanding = Ec.NumOfErrors;

                                            if (Am.CurrentSesNo == Ta.SesNo)
                                            {
                                                Am.ProcessMode = Ta.ProcessMode;
                                            }

                                            Am.UpdateAtmsMain(WAtmNo);

                                            if (Ta.Repl1.DiffRepl == false & Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == true)
                                            {
                                                // There no differences to reconcile

                                                ReconcComment = "EVERYTHING INCLUDING HOST FILES RECONCILE";
                                            }

                                            if (Ta.Repl1.DiffRepl == true || (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == true))
                                            {
                                                // There differences to reconcile

                                                ReconcComment = "NEED TO GO TO RECONCILIATION PROCESS";
                                            }

                                            if (Ta.Recon1.DiffReconcEnd == true & Ta.UpdatedBatch == false)
                                            {
                                                // There differences to reconcile but host not available

                                                ReconcComment = "NEED OF RECONCILIATION BUT HOST FILES NOT AVAILABLE YET";
                                            }

                                            if (Ta.Recon1.DiffReconcEnd == false & Ta.UpdatedBatch == false)
                                            {
                                                // There differences to reconcile

                                                ReconcComment = "HOST FILES NOT AVAILABLE YET";
                                            }

                                            if (Na.PhysicalCheck1.OtherSuspComm == "")
                                            {
                                                Na.PhysicalCheck1.OtherSuspComm = "No Physical Inspection problem exist";
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
                                                                    SCREENa, SCREENb, SCREENc, SCREENd, Na.PhysicalCheck1.OtherSuspComm, Na.ReplUserComment, Ta.ReplGenComment, ReconcComment);
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
                                            return;
                                        
                                    }
                                }

                                
                            }
                        }

                    }

                }


                if (StepNumber == 1 & Us.ProcessNo == 8) // THIS THE FINISH FOR WFUNCTION = 8 
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

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management 
            if (Us.ProcessNo == 56) WRequestor = true; // Requestor

            //-----------------------------------------------------------

            if (WViewFunction || WAuthoriser || WRequestor) ViewWorkFlow = true;
                                                       else ViewWorkFlow = false;

            if (StepNumber == 2)
            {
                tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                UCForm51a Step51a = new UCForm51a();
                Step51a.ChangeBoardMessage += Step51a_ChangeBoardMessage;
                tableLayoutPanelMain.Controls.Add(Step51a, 0, 0);
                Step51a.Dock = DockStyle.Fill;
                Step51a.UCForm51aPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                Step51a.SetScreen();
                textBoxMsgBoard.Text = Step51a.guidanceMsg;

                if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                labelStep2.ForeColor = labelStep3.ForeColor;
                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                labelStep1.ForeColor = Color.White;
                labelStep1.Font = new Font(labelStep1.Font.FontFamily, 22, FontStyle.Bold);
                
                StepNumber--;
                buttonBack.Visible = false;
                buttonNext.Visible = true;

                buttonNext.Text = "Next >";
            }
            else
            {
                if (StepNumber == 4)
                {

                    if (NoDeposits == true)
                    {
                        labelStep4.ForeColor = labelStep3.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                        StepNumber = 3; // decrease by ONE ! 
                    }
                }

                if (StepNumber == 3)
                {
                    tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                    UCForm51b Step51b = new UCForm51b();
                    Step51b.ChangeBoardMessage += Step51b_ChangeBoardMessage;
                    tableLayoutPanelMain.Controls.Add(Step51b, 0, 0);
                    Step51b.Dock = DockStyle.Fill;
                    Step51b.UCForm51bPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                    textBoxMsgBoard.Text = Step51b.guidanceMsg;

                    if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                    if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                    labelStep3.ForeColor = labelStep2.ForeColor;
                    labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                    labelStep2.ForeColor = Color.White;
                    labelStep2.Font = new Font(labelStep2.Font.FontFamily, 22, FontStyle.Bold);

                    StepNumber--;
                    buttonBack.Visible = true;
                    buttonNext.Text = "Next >";
                }
                else
                {
                    if (StepNumber == 4)
                    {
                        tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                        UCForm51c Step51c = new UCForm51c();
                        Step51c.ChangeBoardMessage += Step51c_ChangeBoardMessage;
                        tableLayoutPanelMain.Controls.Add(Step51c, 0, 0);
                        Step51c.Dock = DockStyle.Fill;
                        Step51c.UCForm51cPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                        textBoxMsgBoard.Text = Step51c.guidanceMsg;

                        if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                        if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                        labelStep4.ForeColor = labelStep3.ForeColor;
                        labelStep4.Font = new Font(labelStep4.Font.FontFamily, 10, labelStep4.Font.Style);
                        labelStep3.ForeColor = Color.White;
                        labelStep3.Font = new Font(labelStep3.Font.FontFamily, 18, FontStyle.Bold);

                        StepNumber--;
                        buttonBack.Visible = true;
                    }
                    else
                    {
                        if (StepNumber == 5)
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            //  CASH MANAGEMENT?? 
                            string ParId = "202";
                            string OccurId = "1";
                            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                            Ac.ReadAtm(WAtmNo); 
                                
                            if ( (Ac.CitId == "1000" & Gp.OccuranceNm == "YES" & Ac.TypeOfRepl != "50") )
                            {
                                // RRDM Cash Management is needed  
                                UCForm51d Step51d = new UCForm51d();
                                Step51d.ChangeBoardMessage += Step51d_ChangeBoardMessage;
                                tableLayoutPanelMain.Controls.Add(Step51d, 0, 0);
                                Step51d.Dock = DockStyle.Fill;
                                Step51d.UCForm51dPar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                textBoxMsgBoard.Text = Step51d.guidanceMsg;

                                if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                labelStep5.ForeColor = labelStep4.ForeColor;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                                labelStep4.ForeColor = Color.White;
                                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);

                                StepNumber--;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;

                                buttonNext.Text = "Next >";
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

                                if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "Push Update and Move to Next step or Use Override!";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                labelStep2.ForeColor = labelStep1.ForeColor;
                                labelStep2.Font = new Font(labelStep2.Font.FontFamily, 10, labelStep2.Font.Style);
                                labelStep3.ForeColor = labelStep1.ForeColor;
                                labelStep3.Font = new Font(labelStep3.Font.FontFamily, 10, labelStep3.Font.Style);
                                labelStep5.ForeColor = labelStep4.ForeColor;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 10, labelStep5.Font.Style);
                                labelStep4.ForeColor = Color.White;
                                labelStep4.Font = new Font(labelStep4.Font.FontFamily, 22, FontStyle.Bold);
                               

                                StepNumber--;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;

                                buttonNext.Text = "Next >";
                            }

                        }

                        if (StepNumber == 6)
                        {
                            tableLayoutPanelMain.GetControlFromPosition(0, 0).Dispose();

                            
                                // RRDM ssummary
                                UCForm51e Step51e = new UCForm51e();
                                Step51e.ChangeBoardMessage += Step51e_ChangeBoardMessage;
                                tableLayoutPanelMain.Controls.Add(Step51e, 0, 0);
                                Step51e.Dock = DockStyle.Fill;
                                Step51e.UCForm51ePar(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                                textBoxMsgBoard.Text = Step51e.guidanceMsg;

                                if (Us.ProcessNo == 1) textBoxMsgBoard.Text = "View and Update if needed";
                                if (ViewWorkFlow == true) textBoxMsgBoard.Text = "View only";

                                labelStep6.ForeColor = labelStep5.ForeColor;
                                labelStep6.Font = new Font(labelStep6.Font.FontFamily, 10, labelStep6.Font.Style);
                                labelStep5.ForeColor = Color.White;
                                labelStep5.Font = new Font(labelStep5.Font.FontFamily, 22, FontStyle.Bold);

                                StepNumber--;
                                buttonBack.Visible = true;
                                buttonNext.Visible = true;

                                buttonNext.Text = "Next";

                        }
                    }
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
            textBoxMsgBoard.Text = ((UCForm51c)tableLayoutPanelMain.Controls["UCForm51c"]).guidanceMsg;
        }

        void Step51b_ChangeBoardMessage(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = ((UCForm51b)tableLayoutPanelMain.Controls["UCForm51b"]).guidanceMsg;
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

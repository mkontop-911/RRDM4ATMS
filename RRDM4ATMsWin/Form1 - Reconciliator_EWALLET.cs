//multilingual
using RRDM4ATMs;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
//24-04-2015 Alecos
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form1ATMs_Reconciliator_EWALLET : Form
    {
        public EventHandler LoggingOut;

        //Form108 NForm108;
        Form8 NForm8;
        Form12 NForm12;
        Form13 NForm13;
        Form15 NForm15;

        Form200 NForm200;
        Form21MIS NForm21MIS;

        Form5 NForm5;
        Form3 NForm3;

        string WBankId;

        //Form116 NForm116; // Dispute pre investigation 

        Form18 NForm18; // CIT Providers  

        Form136 NForm136; // Datagrid showing all Country Banks for a Group 

        Form10 NForm10;

        Form9 NForm9;

        Form143 NForm143;
        Form45 NForm45;
        Form47 NForm47;
        // Form49 NForm49;
        //Form50 NForm50;

        Form52 NForm52;
        Form53 NForm53;
        Form152 NForm152;

        Form54 NForm54;
        Form55 NForm55;

        Form63 NForm63;

        Form78 NForm78;
        Form81 NForm81;

        Form85 NForm85;

        Form191 NForm191;
        Form192 NForm192;
        Form193 NForm193;

        Form19a NForm19;

        Form300 NForm300;

        Form112 NForm112;

        bool CheckBoxAllowMsgsWithin;

        string MsgFilter;

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        string WOrigin = "Our Atms";
        //string WApplication; 

        // NOTES 
        string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WSelectionCriteria;

        int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form1ATMs_Reconciliator_EWALLET(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            //labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            // Replenishment.Hide(); 
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    if (Usi.WFieldNumeric11 == 15)
                    {
                        W_Application = "EGATE";
                    }
                    labelStep1.Text = "Reconciliator Menu_" + W_Application;
                }
                else
                {
                    W_Application = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                        //  T24Version = true;
                        labelStep1.Text = "Controller's Menu-VER_T24_" + W_Application;
                    }
                    else
                    {
                        labelStep1.Text = "Controller's Menu-_" + W_Application;
                    }

                }
            }

             
            RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();
            Usr.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, W_Application);

            if (Usr.Authoriser == true)
            {
                CheckBoxAllowMsgsWithin = true;
                checkBoxAllowMsgs.Show();

                if (Usr.MsgsAllowed == true) checkBoxAllowMsgs.Checked = true;
                else checkBoxAllowMsgs.Checked = false;

            }
            else
            {
                checkBoxAllowMsgs.Hide();
            }

            Us.ReadUsersRecord(WSignedId); 

            //if (Us.Branch == "015" || Us.Branch == "0015")
            //{
            //    buttonGL_Totals.Hide(); 
            //}
            //else
            //{
            //    //Replenishment.Hide();
            //    Reconciliation.Hide(); 
            //    Authorisation.Hide();
            //    TXNS_POSTING.Hide(); 
            //}

         

           

            //  CASH MANAGEMENT Prior Replenishment Workflow  ... Show or Hide Button 
            string ParId = "211";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

           

            //labelUSERno.Text = WSignedId;

            MsgFilter =
                   "(ReadMsg = 0 AND ToAllAtms = 1)"
               + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(button51, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(button52, "Communicate with today's controller.");

            checkBox1.Checked = false;

            //****************************************
            //DESCRIPTIONS 
            //****************************************
            toolTipHeadings.ShowAlways = true;

          
        }

        
        DateTime WCut_Off_Date;
        string WJobCategory ;
        int WReconcCycleNo;
        string W_Application; 
        private void FormMainScreen_Load(object sender, EventArgs e)
        {
           
          

            // Reset  
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();


            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);
            if (WReconcCycleNo != 0)
            {
                WCut_Off_Date = Rjc.Cut_Off_Date;
            }
            else
            {
                WCut_Off_Date = NullPastDate;
            }



            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();

           
            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            Ap.ReadAuthorizationsUserTotal(WSignedId);
            if (Ap.RecordFound == true)
            {
                //labelAuthRecords.Text = Ap.TotalNumberforUser.ToString();
                checkBoxAuthRecords.Show();
                checkBoxAuthRecords.Checked = true;
            }
            else checkBoxAuthRecords.Hide();


            ////*************************************************************************
            ////Check if outstanding transactions to be posted
            ////*************************************************************************
            RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

            //RRDMUpdateGrids Ug = new RRDMUpdateGrids();

            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//
            string AtmNo = "";
            string FromFunction = "";

            FromFunction = "General";


            RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();


            string SelectionCriteria = "Operator ='" + WOperator + "'"
                               + " AND OpenRecord = 1 ";

            bool WithDate = false;
            Tc.ReadAllTransToBePostedToSeeIfAny(WSignedId, SelectionCriteria);

            if (Tc.RecordFound = true)
            {
                //checkBoxTransToBePosted.Text = Tc.TotalSelected.ToString();

                checkBoxTransToBePosted.Show();
                checkBoxTransToBePosted.Checked = true;

            }
            else checkBoxTransToBePosted.Hide();

           

            //**************************************************************************
            //CHECK FOR DISPUTES FOR THE SIGNED PERSON
            //**************************************************************************
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

            SelectionCriteria = "Operator ='" + WOperator + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";

            WithDate = false;

            Di.ReadDisputesInTable(WOperator, WSignedId, "", NullPastDate, WithDate, 11);

            if (Di.TotalSelected > 0)
            {
                //labelOutstandingDisputes.Text = Di.TotalSelected.ToString();
                checkBoxOutstandingDisputes.Show();
                checkBoxOutstandingDisputes.Checked = true;
            }
            else checkBoxOutstandingDisputes.Hide();

            //**************************************************************************
            //END
            //**************************************************************************

            toolTipHeadings.ShowAlways = true;

            ////Define Banks 
            //toolTipButtons.SetToolTip(labelConfiguration, "Infrastructure" + Environment.NewLine
            //                             + "Definition of what is needed for system to operate."
            //                             );

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;
                //Define Banks 


               
                //View of categories Txns 
                toolTipButtons.SetToolTip(button11, "View Txns history of all categories." + Environment.NewLine
                                              + "This can be served as an audit trail of all  " + Environment.NewLine
                                              + "Txns, processes, exceptions and actions.");

                //View all unmatched for the running Cycle and others 
                toolTipButtons.SetToolTip(button38, "View All Unmatched Txns " + Environment.NewLine
                                              + "Unmatched for this cycle with or without actions on them" + Environment.NewLine
                                              + "Historical unmatched with the associated actions");

                //// Reconciliation for categories 
                //toolTipButtons.SetToolTip(button85, "Reconciliation is done By RM Category." + Environment.NewLine
                //                              + "This button will display the RM Categories that have exceptions and the owner has right to access" + Environment.NewLine
                //                              + "Through a workflow reconciliation is performed");


              


             

                //Trans to be posted  
                toolTipButtons.SetToolTip(button35, "The transactions are created from different processes within the system such as:" + Environment.NewLine
                                             + "During Replenishment for money in and out from ATMs." + Environment.NewLine
                                             + "Actions on MetaExceptions in the reconciliation process." + Environment.NewLine
                                             + "Actions to settle a dispute." + Environment.NewLine
                                             + "Actions related with CIT providers" + Environment.NewLine
                                             + "Transactions can be automatically posted or Vouchers are printed for manual posting"
                                             );

               
                //NEW JOB CYCLES 
                toolTipButtons.SetToolTip(button48, "Every morning the Reconciliation Officer " + Environment.NewLine
                                             + "through this button can perform the reconciliation process " + Environment.NewLine
                                             + "for which he is the owner. "
                                             );

               

                
                // System ERRORS       
                toolTipButtons.SetToolTip(button95, "During System operation a system error might occur." + Environment.NewLine
                                             + "In such remote possibility information is provided to assist IT programmers." + Environment.NewLine
                                             + "Information can beviewed or send by email" + Environment.NewLine
                                             );

               
              
                
                //Dispute Pre-Investigation      
                toolTipButtons.SetToolTip(button67, "Before Dispute is registered a pre-investigation can be made to resolve it." + Environment.NewLine
                                             + "Information about journal, replenishment , reconciliation, errors and video clips " + Environment.NewLine
                                             + "related to the disputed transaction is readily available."
                                             );

                //Dispute Registration       
                toolTipButtons.SetToolTip(button68, "Register new Disputes." + Environment.NewLine
                                             + "A Dispute is related to one or many transactions." + Environment.NewLine
                                             + "A Dispute form is printed to be signed by both the customer and the Bank."
                                             );

                //Dispute Management        
                toolTipButtons.SetToolTip(button69, "The Dispute management is based on a workflow." + Environment.NewLine
                                             + "Investigation is made and actions are taken." + Environment.NewLine
                                             + "Transactions to be posted based on decision are generated."
                                             );

                //Authorization Management        
                toolTipButtons.SetToolTip(button75, "The Pending Authorizations are manage here." + Environment.NewLine
                                             + "You can go and Authorise , or Update action after authorization is made." + Environment.NewLine
                                             + "Also if authorizer is not availble you can undo(Delete) authorization request."
                                             );

                
                //Departure from quality indicators     
                toolTipButtons.SetToolTip(button37, "Check if transactions were updated by Cashier." + Environment.NewLine
                                             + "System suggest and Cashier must act.."
                                             );           
            }
            else
            {

            }
        }

        // Log Out 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Go to FORM40
            LoggingOut(sender, e);

        }

        // Form Closed

        private void FormMainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoggingOut(sender, e);
        }
        // Disable / enable Ballon Info 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                toolTipButtons.Active = true;
                FormMainScreen_Load(this, new EventArgs());
            }
            else
            {
                toolTipButtons.Active = false;
                //     FormMainScreen_Load(this, new EventArgs());
            }
        }

     

        void NForm152_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // Dispute Pre Investigation 
        private void button67_Click(object sender, EventArgs e)
        {
            //Form3_PreInv NForm3_PreInv;

            //NForm3_PreInv = new Form3_PreInv(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            //NForm3_PreInv.ShowDialog();

            Form3_PreInv_MOBILE NForm3_PreInv_MOBILE;
            //Form3_PreInv_MOBILE(string InSignedId, string InOperator, string InApplication ,int InRMCycle)
            NForm3_PreInv_MOBILE = new Form3_PreInv_MOBILE(WOperator, WSignedId, W_Application, WReconcCycleNo);
            NForm3_PreInv_MOBILE.ShowDialog();


            //NForm116 = new Form116(WSignedId, WSignRecordNo, WOperator);
            //NForm116.ShowDialog();
        }

        // DISPUTE REGISTRATION  
        private void button68_Click(object sender, EventArgs e)
        {
            int From = 1;
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, "", 0, 0, 0, "", From, "");
            NForm5.ShowDialog(); ;
        }

        // MANAGE DISPUTES 
        private void button69_Click(object sender, EventArgs e)
        {
            int Mode; // 
            Mode = 11; // Owner User 
            Mode = 12; // Dispute Manager

            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

            //Uar.ReadUsersVsApplicationsVsRolesByApplication_For_Disputes_Min_User("ATMS/CARDS");

            Di.ReadDisputeOwnerTotal(WSignedId);

            Uar.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, "ATMS/CARDS");


            string WSecurityLevel = Uar.SecLevel;

            if (WSecurityLevel == "04" || WSecurityLevel == "06" || Di.DisputeOwnerTotal > 0)
            {
                // OK These are allowed , Security office and Security manager 
                // "04" =  Dispute Officer , "06" = Dispute Manager
                if (WSecurityLevel == "04")
                {
                    Mode = 11;
                }
                if (WSecurityLevel == "06")
                {
                    Mode = 12;
                }
            }
            else
            {
                MessageBox.Show("You are not allowed for this selection" + Environment.NewLine
                    + "You must be dispute officer or a dispute manager " + Environment.NewLine
                    + "OR Must be a reconciliation officer with disputes assign to you. " + Environment.NewLine
                    );
                return;
            }

            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, "", NullPastDate, NullPastDate, Mode);
            NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();

           
        }

        void NForm3_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // AUthorize Decisions 

        private void button75_Click(object sender, EventArgs e)
        {
            string TempAtmNo = "";
            int TempSesNo = 0;
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "Normal", TempAtmNo, TempSesNo, WDisputeNo, WDisputeTranNo, "", 0);
            NForm112.FormClosed += NForm112_FormClosed;
            NForm112.ShowDialog();
        }

        void NForm112_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

     
       

        void NForm63_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // Message from Controller 

        private void button51_Click(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                   "(ReadMsg = 0 AND ToAllAtms = 1)"
               + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(button51, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {

                toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        /*
        private void buttonMsgs_Click(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();
        }
         */
        // Todays Controller 

        private void button52_Click(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }



        //
        //SYSTEM ERRORS 
        //
        private void button95_Click(object sender, EventArgs e)
        {
            Form82 NForm82;
            NForm82 = new Form82(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm82.ShowDialog(); ;
        }



        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //CheckForMessages(); // Check for messages from controller

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

            if (Ap.IsServerConnected() == true )
            {
                // Everything is good ... continue 
            }
            else
            {
                return;  // SQL is not available 
            }
            

            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            Ap.ReadAuthorizationsUserTopOne(WSignedId);
            if (Ap.RecordFound == true)
            {
                //checkBoxAuthRecords.Text = Ap.TotalNumberforUser.ToString();
                checkBoxAuthRecords.Show();
            }
            else checkBoxAuthRecords.Hide();

        }

        int NumberMsgs = 0;

        private void labelNumberMsgs_Click(object sender, EventArgs e)
        {

        }
   
        // MANAGE TRANSACTIONS TO BE POSTED
        // These are created after reconciliation 
        private void button35_Click(object sender, EventArgs e)
        {
            int Mode = 1;
            NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator, "", 0, 0, Mode);
            NForm78.FormClosed += NForm78_FormClosed;
            NForm78.ShowDialog(); ;
        }
        void NForm78_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
       
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
       
       
        //
        // GO to Matching of outstanding postings 
        //
        private void button37_Click(object sender, EventArgs e)
        {

            WAction = 1;
            // TESTING 
            if (WAction == 1)
            {
                // RETURN till final testing 
                return;
            }

            NForm19 = new Form19a(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm19.FormClosed += NForm19_FormClosed;
            NForm19.ShowDialog(); ;
        }

        void NForm19_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }




        // Software version details 
        private void button6_Click(object sender, EventArgs e)
        {

            AboutBox1 about = new AboutBox1();
            about.ShowDialog();

        }
       
       
        void NForm80a1_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
        // TXNS RECONCILIATION
        private void button48_Click(object sender, EventArgs e)
        {

            Form80a1_EWALLET NForm80a1_EWALLET;

            string WFunction = "Reconc";
            string WRMCategory = "ALL";
            string WhatBank = "";
            NForm80a1_EWALLET = new Form80a1_EWALLET(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank,W_Application);
            NForm80a1_EWALLET.FormClosed += NForm80a2_FormClosed;
            NForm80a1_EWALLET.ShowDialog();
        }
       

        void NForm80a2_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
       
        // Mass Categories and matching cycles 

        private void button11_Click(object sender, EventArgs e)
        {


            Form80a1_EWALLET NForm80a1_EWALLET;

            string WFunction = "View";
            string WRMCategory = "ALL";
            string WhatBank = "";
            NForm80a1_EWALLET = new Form80a1_EWALLET(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank, W_Application);
            NForm80a1_EWALLET.FormClosed += NForm80a2_FormClosed;
            NForm80a1_EWALLET.ShowDialog();
        }
       
       
       
        // Timer two 
        private void timer2_Tick(object sender, EventArgs e)
        {
            // It operates every ten seconds 

        }
       
        // UNMATCHED Transactions 
        private void button38_Click(object sender, EventArgs e)
        {
            if (WSignedId == "1005")
            {
                MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
                    + "Please use user 1006.");
                return;
            }

            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = "";
            int WReconcCycleNo ;

            if (WOperator == "CRBAGRAA")
            {
                WReconcCycleNo = 205;
            }
            else
            {
                WJobCategory = W_Application;

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            }
            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, WReconcCycleNo,
                "",NullPastDate,NullPastDate,1
                );
            NForm80b2.ShowDialog();
        }


    
        // Allow messages for authorisers 
        private void checkBoxAllowMsgs_CheckedChanged(object sender, EventArgs e)
        {

            if (CheckBoxAllowMsgsWithin == true)
            {
                CheckBoxAllowMsgsWithin = false;
                return;
            }

            RRDMUsers_Applications_Roles Urs = new RRDMUsers_Applications_Roles();
            Urs.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, W_Application);

            if (Urs.Authoriser == true)
            {
                if (checkBoxAllowMsgs.Checked == true)
                {
                    Urs.MsgsAllowed = true;
                    MessageBox.Show("From now on you will receive messages for authorisation!");
                }
                else
                {
                    Urs.MsgsAllowed = false;
                    MessageBox.Show("From now on you will NOT receive messages for authorisation!");
                }

                //Us.UpdateUser(WSignedId);

            }
            else
            {
                MessageBox.Show("This function is only for Authorisers!");
            }

        }
        // REVERSALS
        private void button82_Click(object sender, EventArgs e)
        {
            Form78Rev NForm78Rev;
            int Mode = 1;
            NForm78Rev = new Form78Rev(WSignedId, WSignRecordNo, WOperator, "", 0, 0, Mode);
            NForm78Rev.FormClosed += NForm78_FormClosed;
            NForm78Rev.ShowDialog(); ;
        }
     
    
        // Creation Of Output files 
        private void button18_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Output_Files NForm200JobCycles_Output_Files;

            string RunningGroup = "ATMs";
            NForm200JobCycles_Output_Files = new Form200JobCycles_Output_Files(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles_Output_Files.FormClosed += NForm152_FormClosed;
            NForm200JobCycles_Output_Files.ShowDialog();
        }

// CATEGORY TRANSACTIONS VIEW 
        private void button24_Click_1(object sender, EventArgs e)
        {
            Form80a3_EWALLET NForm80a3_EWALLET;
            string WFunction = "View";
            string Category = "All";
            
            string WhatBank = WOperator;

            NForm80a3_EWALLET = new Form80a3_EWALLET(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            NForm80a3_EWALLET.FormClosed += NForm80a1_FormClosed;

            NForm80a3_EWALLET.ShowDialog();
        }

// GL Totals 
        private void buttonGL_Totals_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL NForm200JobCycles_GL;

            string RunningGroup = "ATMs";
            NForm200JobCycles_GL = new Form200JobCycles_GL(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles_GL.ShowDialog();
        }


// View Reports 
        private void buttonCycleReports_Click(object sender, EventArgs e)
        {
            // We will go to EWALLET 
            //
            Form200JobCycles_Reports_EWALLET NForm200JobCycles_Reports_EWALLET;

           string RunningGroup = W_Application;
            NForm200JobCycles_Reports_EWALLET = new Form200JobCycles_Reports_EWALLET(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm200JobCycles_Reports_EWALLET.ShowDialog();

           // Form200JobCycles_Reports_EWALLET
        }
// All Outstanding 
        private void linkLabelOutstanding_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 4; // All Outstanding for Repl 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                   , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// Link Replenished and Authorised by this person 
        private void linkLabelAuthorised_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 7; // All Repl this cycle by user 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// this cycle only
        private void linkLabelThisCycleOnly_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            int Mode = 9; // All Outstanding for Repl 
            string WAtmNo = "";
            int WSesNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                   , WReconcCycleNo, NullPastDate, NullPastDate, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// 

// GL ENTRIES 
        private void button_GL_ENTRIES_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL_Mobile NForm200JobCycles_GL_Mobile;

            string WJobCateg = W_Application;
            NForm200JobCycles_GL_Mobile = new Form200JobCycles_GL_Mobile(WSignedId, WSignRecordNo, WSecLevel, WOperator, WJobCateg);
            NForm200JobCycles_GL_Mobile.ShowDialog();
            // SHOW GL Entries
            //
            //Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            //int WMode = 8; // SHOW GL ENTRIES
            //string WMatchedCat = "NO_Cat"; 
            //NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WMatchedCat, "", WReconcCycleNo, WMode, W_Application);
            //NForm78d_Discre_MOBILE.ShowDialog();
        }
    }
}

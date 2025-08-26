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
    public partial class Form1ATMs_Reconciliator : Form
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

        bool T24Version; 

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
        string WApplication; 

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

        public Form1ATMs_Reconciliator(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            //labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            T24Version = false;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                WApplication = Usi.SignInApplication;

                if (WApplication == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        WApplication = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        WApplication = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        WApplication = "IPN";
                    }
                    labelStep1.Text = "Controller's Menu-Mobile_" + WApplication;
                }
                else
                {
                    WApplication = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                        T24Version = true;
                        labelStep1.Text = "T24_MAIN MENU " + WApplication + " (" + WSignedId + ")";
                    }
                    else
                    {
                        labelStep1.Text = "MAIN MENU " + WApplication + " (" + WSignedId + ")";
                    }

                }
            }



            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();
            Usr.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, Usi.SignInApplication);

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

            if (Us.Branch == "015" || Us.Branch == "0001")
            {
                buttonGL_Totals.Hide(); 
            }
            else
            {
                Replenishment.Hide();
                Reconciliation.Hide(); 
                Authorisation.Hide();
                TXNS_POSTING.Hide(); 
            }     

            if (WSignedId == "1032-Level2"
                || WSignedId == "1033-Level3"
                || WSignedId == "1034-Level4"
                || WSignedId == "1035-Level5"
                || WSignedId == "NBG_005")
            {
                RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
                string MainFormId = "Form1 - ATMs - Operational";

                // Replenishement
                foreach (Control c in Replenishment.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                               //    + " AND Operator ='" + WOperator + "'"
                                               ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                        if (Ur.RecordFound == true)
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }

                    }
                }
                // Reconciliation 
                foreach (Control c in Reconciliation.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                               //   + " AND Operator ='" + WOperator + "'"
                                               ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                        if (Ur.RecordFound == true)
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }

                    }
                }

            
                // Disputes 
                foreach (Control c in Disputes.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                               //   + " AND Operator ='" + WOperator + "'"
                                               ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                        if (Ur.RecordFound == true)
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }

                    }
                }

                // Monitoring
                foreach (Control c in Monitoring.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                               //   + " AND Operator ='" + WOperator + "'"
                                               ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                        if (Ur.RecordFound == true)
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }

                    }
                }

               
                // Authorisation
                foreach (Control c in TXNS_POSTING.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                               //   + " AND Operator ='" + WOperator + "'"
                                               ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                        if (Ur.RecordFound == true)
                        {
                            c.Enabled = true;
                        }
                        else
                        {
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }

                    }
                }
            }

            //  CASH MANAGEMENT Prior Replenishment Workflow  ... Show or Hide Button 
            string ParId = "211";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

            if (CashEstPriorReplen != "YES") // If No calculation before Replenishment is needed
            {
                //button94.Enabled = false;
                button94.BackColor = Color.Violet;
                button94.ForeColor = Color.Black;
            }

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

            toolTipButtons.SetToolTip(labelReplenishement,
                                            "Five step Replenishement Workflow." + Environment.NewLine
                                          + "Physical Inspection" + Environment.NewLine
                                          + "Cash Management" + Environment.NewLine
                                          + "Four eye electronic Authorisation" + Environment.NewLine
                                          + "Capture Cards Management" + Environment.NewLine
                                          + "Deposits Management" + Environment.NewLine
                                          + "E-Journal Readily available" + Environment.NewLine
                                          + ".............."
                                         );

            toolTipButtons.SetToolTip(labelReconciliation,
                                          "Work Alocation for Reconciliation categories in difference." + Environment.NewLine
                                          + "Workflow to reconcile Reconciliation Categories" + Environment.NewLine
                                          + "Workflow to reconcile ATMs machine cash vs General Ledger Cash" + Environment.NewLine
                                          + "Four eye electronic Authorisation" + Environment.NewLine
                                          + "Rich Transaction Reconciliation History" + Environment.NewLine
                                          + ".............."
                                         );


            toolTipButtons.SetToolTip(labelDisputes,
                                        "Disputes can be originated from different places within the system." + Environment.NewLine
                                          + "Dispute Pre-investication provides to Branch, Call Centre, BackOffice" + Environment.NewLine
                                          + "makes readily available all information about txns, errors, actions" + Environment.NewLine
                                          + "Journal lines are readily available too" + Environment.NewLine
                                          + "Dispute Registration and Management moves disputes to Back Office for management and decisions" + Environment.NewLine
                                          + ".............."
                                         );

            toolTipButtons.SetToolTip(labelMonitoring,
                                         "A Dashboard is available to the controller for central monitoring " + Environment.NewLine
                                          + "Monitors Replenishement, Matching of files status and Reconciliation" + Environment.NewLine
                                          + "Prints Daily reports" + Environment.NewLine
                                          + "Monitors personnel performance and communicates with them " + Environment.NewLine
                                          + "Audit trail " + Environment.NewLine
                                          + ".............."
                                         );

            
            toolTipButtons.SetToolTip(labelAuthorisation,
                                        "Management of Authorisations for " + Environment.NewLine
                                          + "Requestors and Authorisers ." + Environment.NewLine
                                          + ".............."
                                         );

            toolTipButtons.SetToolTip(labelPosting,
                                          "All created entries are passing through this pool " + Environment.NewLine
                                          + "Authorised personel can print for manual posting ." + Environment.NewLine
                                          + "OR automatic posting through a build interface can be performed ." + Environment.NewLine
                                          + ".............."
                                         );
        }

        ////
        //// source code 
        //// Code Snippet
        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}
        DateTime WCut_Off_Date;
        string WJobCategory = "ATMs";
        int WReconcCycleNo;
        private void FormMainScreen_Load(object sender, EventArgs e)
        {
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

           

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            WCut_Off_Date = Rjc.Cut_Off_Date;

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

           // string WCitId = "1000";
            // Create table with the ATMs this user can access
           // Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
           // Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            //-----------------------------------------------------------------// 

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
            // END OF FUNCTION
            //**************************************************************************
            //
            // Unmatched Trans to be posted Grid
            //**************************************************************************
            //SelectionCriteria = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "' AND HostMatched = 0 AND ActionCd2 = 1";

            //WithDate = false;
            //Tc.ReadAllTransToBePostedToSeeIfAny(WSignedId, SelectionCriteria, NullPastDate, WithDate);

            //if (Tc.RecordFound == true)
            //{
            //    //labelTransUnMatched.Text = Tc.TotalSelected.ToString();

            //    checkBoxTransUnMatched.Show();
            //    checkBoxTransUnMatched.Checked = true;
            //}
            //else checkBoxTransUnMatched.Hide();

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


              

                //Training material  
                toolTipButtons.SetToolTip(button56, "View training material." + Environment.NewLine
                                             + "System and operational workflows are available." + Environment.NewLine
                                             + "Circulars, Videos and other documentation can be available."
                                             );

                //Calculation of Money In Prior to Replenishment
                toolTipButtons.SetToolTip(button94, "Change parameter 211 to activate " + Environment.NewLine
                                            + "RRDM system, based on the latest journal info, presents to ATM operator Officer " + Environment.NewLine
                                            + "how much money to load to ATM prior to Replenishment workflow."
                                            + "This functionality is only available if the proper parameter is set up."
                                            );

                //Replenishment
                toolTipButtons.SetToolTip(button57, "Replenishment is a workflow of 5 self-explanatory steps." + Environment.NewLine
                                            + "At the end a Replenishment Report is printed for signing."
                                            );

                
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


                //Captured cards
                toolTipButtons.SetToolTip(button10, "View captured cards info for management." + Environment.NewLine
                                             + "Updated (up to the minute) information is available." + Environment.NewLine
                                             + "A Captured Card form for signing is prepared. ");


                //MY Transactions 
                //toolTipButtons.SetToolTip(button59, "View the up to the minute transactions." + Environment.NewLine
                //                             + "Different searching criteria can be applied." + Environment.NewLine
                //                             + "Transactions files, ejournals and video clips are available."
                //                             );

                //Deposits reconciliation 
                toolTipButtons.SetToolTip(button34, "Deposits of all kinds are displayed." + Environment.NewLine
                                             + "Differences can be resolved or moved to Disputes system."
                                             );

                //My ATMS operation  
                toolTipButtons.SetToolTip(button15, "View the Atms current and historical operational status." + Environment.NewLine
                                             + "Through this you can search and view info for all Replenishment Cycles by period." + Environment.NewLine
                                             + "Google Maps show the location of the ATM."
                                             );

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

                //DELAYS 
                toolTipButtons.SetToolTip(button13, "Delays may occur due :" + Environment.NewLine
                                             + "Reconciliation Categories of previous Cycles with unmatched records. " + Environment.NewLine
                                             + "Delay in loading a Journal due to communication lines." + Environment.NewLine
                                             + "Not arriving of a file either from the Bank or from external source."
                                             );

                
                //E-Journal Drilling 
                toolTipButtons.SetToolTip(button91, "All ejournals are available for interrogation." + Environment.NewLine
                                             + "Up to the last minute information is available." + Environment.NewLine
                                             + "E Journals are transformed into formatted information of different categories." + Environment.NewLine
                                             + "Transactions of any kind, errors, captured cards, replenishment process."
                                             );

                
                // System ERRORS       
                toolTipButtons.SetToolTip(button95, "During System operation a system error might occur." + Environment.NewLine
                                             + "In such remote possibility information is provided to assist IT programmers." + Environment.NewLine
                                             + "Information can beviewed or send by email" + Environment.NewLine
                                             );

               
                //Today's reports  
                toolTipButtons.SetToolTip(button16, "View a list of all daily reports." + Environment.NewLine
                                             + "A number of other reports can be created on customer request."
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

        // Authorised Personnel 

        private void button54_Click(object sender, EventArgs e)
        {
            //TESTING
            /*
            if (WSecLevel < 6)
            {
                MessageBox.Show(" YOU CANNOT ACCESS PERSONEL MAINTENANCE");
                return;
            }
             */
            // Sixth which is zero is CitId
            NForm13 = new Form13(WSignedId, WSignRecordNo, WOperator, "1000", 1);
            NForm13.ShowDialog(); 
        }

        // Replenishement 
        // REPLENISHMENT ..... 
        // ..........
        // SINGLES
        // WFunction = 1 Normal branch ATM

        // GROUPS
        // 5 Normal Group belonging to Bank . 
        private void button57_Click(object sender, EventArgs e)
        {
            
            // Check if Replenishment = Reconciliation 
            // Centralised Reconciliation
            bool Recon_Equal_Repl = false; 
            string ParId = "939";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                Recon_Equal_Repl = true;
            }
            else
            {
                Recon_Equal_Repl = false;
            }
            //
            // Check if can make replenishment
            //
            
            if (Recon_Equal_Repl == true)
            {

            }
            else
            {
                Ba.ReadUserAccessToAtms(WSignedId); // READ ALL TABLE ENTRIES TO SEE IF ONE TO ONE 

                if (Ba.NoOfAtmsRepl == 0 & Ba.NoOfGroupsRepl == 0)
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT ");
                    return;
                }

            }

            if (Ba.NoOfGroupsRepl == 0) WAction = 1; // Replenishment for individual of more than one ATM,
            //Us.ReadUsersRecord(WSignedId); // READ USER DATA
            //if (Ba.NoOfGroupsRepl > 0) WAction = 5; // Replenishment for Group of ATM 

            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.FormClosed += NForm152_FormClosed;
            NForm152.ShowDialog();
        }

        void NForm152_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // Reconciliation 
        // RECONCILIATION ..... 
        // ........
        // 

        private void button58_Click(object sender, EventArgs e)
        {
            Ba.ReadUserAccessToAtms(WSignedId); // READ TO SEE GROUP OR ATM 

            if (Ba.NoOfAtmsReconc == 0 & Ba.NoOfGroupsReconc == 0)
            {
                MessageBox.Show(" YOU ARE NOT AUTHORIZE FOR RECONCILIATION ");
                return;
            }

            if (Ba.NoOfGroupsReconc > 0)
            {
                MessageBox.Show(" YOU HAVE A GROUP. Push the GROUP Button please ");
                return;
            }

            if (Ba.NoOfAtmsReconc > 0)
            {
                WAction = 2; // Reconciliation for individual ATMs

                NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
                NForm152.FormClosed += NForm152_FormClosed;
                NForm152.ShowDialog(); ;
            }
        }

        // Capture Cards Management 
        private void button10_Click(object sender, EventArgs e)
        {
            if (Environment.UserName != "Admin")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT-System under investigation ");
                return;
            }
            if (WSignedId == "1006")
            {
                MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
                    + "Please use user 1005.");
                return;
            }
            WAction = 3; // Cards Management 
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }

        // CIRCULARS AND VIDEO FOR TRAININGS 
        private void button56_Click(object sender, EventArgs e)
        {
            NForm300 = new Form300();
            NForm300.ShowDialog(); ;
        }


        // Controller's STATS
        private void button12_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int Wf = 0;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, Wf);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {   // 0 stands for CitNo... CitNo>0 when calling from Form18 (Cit View)

                NForm200 = new Form200(WSignedId, WSignRecordNo, WSecLevel, WOperator, "1000");
                NForm200.ShowDialog(); ;
            }

        }

        // Dispute Pre Investigation 
        private void button67_Click(object sender, EventArgs e)
        {
            Form3_PreInv NForm3_PreInv;

            NForm3_PreInv = new Form3_PreInv(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            NForm3_PreInv.ShowDialog();


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

            if (WSecurityLevel == "03" || WSecurityLevel == "04" || WSecurityLevel == "06" || Di.DisputeOwnerTotal > 0)
            {
                // OK These are allowed , Security office and Security manager 
                // "04" =  Dispute Officer , "06" = Dispute Manager
                if (WSecurityLevel == "03" || WSecurityLevel == "04")
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

            //int Mode ; // 
            //Mode = 11; // Owner User 
            //Mode = 12; // Dispute Manager

            //RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();
            //RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

            //Di.ReadDisputeOwnerTotal(WSignedId); 

            //Uar.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, "ATMS/CARDS");

            //string WSecurityLevel = Uar.SecLevel;

            //if (WSecurityLevel == "04" || WSecurityLevel == "06" || Di.DisputeOwnerTotal>0)
            //{
            //    // OK These are allowed , Security office and Security manager 
            //}
            //else
            //{
            //    MessageBox.Show("You are not allowed for this selection" + Environment.NewLine
            //        + "You must be dispute officer or a dispute manager " + Environment.NewLine
            //        + "OR Must be a reconciliation officer with disputes assign to you. " + Environment.NewLine
            //        );
            //    return;            
            //}

            //NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, "", NullPastDate,NullPastDate, Mode);
            //NForm3.FormClosed += NForm3_FormClosed;
            //NForm3.ShowDialog();
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

        // GO TO BANKS 
        private void button13_Click(object sender, EventArgs e)
        {
            /*
            if (WSecLevel < 9 )
            {
                 MessageBox.Show(" ONLY GLOBAL GAS MASTER USER IS ALLOWED TO GO TO BANKS ");
                 return; 
            }
             */
            NForm143 = new Form143(WSignedId, WSignRecordNo, WOperator);
            NForm143.ShowDialog(); ;

            //  this.Hide(); 
        }
        // MAINTENACE OF GROUPS 
        private void button14_Click(object sender, EventArgs e)
        {
            /*
            if (WSecLevel < 6)
            {
                MessageBox.Show(" ONLY BANK GAS MASTER USER IS ALLOWED TO GO TO BANKS ");
                return;
            }
             */
            NForm45 = new Form45(WSignedId, WSignRecordNo, WOperator);
            NForm45.ShowDialog();
            //   this.Hide(); 
        }
        //
        // SHOW MY ATMs 
        //
        private void button15_Click(object sender, EventArgs e)
        {

            WAction = 1; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator, "", WAction);
            NForm47.ShowDialog(); ;

        }

        //
        // ERRORS MANAGEMENT 
        // 

        private void button25_Click(object sender, EventArgs e)
        {

           

            WAction = 2; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator, "", WAction);
            NForm47.ShowDialog(); ;
            // this.Close();
        }

        void NForm49_FormClosed(object sender, FormClosedEventArgs e)
        {
            //   MainScreen_Load(this, new EventArgs());
        }

        // Refresh Testing data 
        //
        //bool FromRefreshTestingData ;

        private void button22_Click(object sender, EventArgs e)
        {
            // CALL RRDM CLASS TO TRUNCATE THE TABLES

            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            Msf.ReadFilesANDTruncate(WOperator);

            // CALL STORE PROCEDURE TO UPDATE STANDARD TABLES
            // 
            string connectionString = ConfigurationManager.ConnectionStrings
              ["ATMSConnectionString"].ConnectionString;

            string RCT = "ATMS.[dbo].[Stp_Refresh_Testing_Data]";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
            //FromRefreshTestingData = true;
            //FormMainScreen_Load(this, new EventArgs());
 

            

            //*************************************************************

            // RESET UNIQUE KEY

            string connectionString2 = ConfigurationManager.ConnectionStrings
              ["ReconConnectionString"].ConnectionString;

            string RCT2 = "[RRDM_Reconciliation_ITMX].[dbo].[usp_ResetUniqueId]";

            using (SqlConnection conn2 =
               new SqlConnection(connectionString2))
                try
                {
                    conn2.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT2, conn2))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            MessageBox.Show("Testing Data have been initialised");
            //FromRefreshTestingData = true;
            FormMainScreen_Load(this, new EventArgs());

           // MessageBox.Show("Testing Data have been initialised");

        }
        //
        // THIS GOES TO ATMS IN NEED
        //
        private void button60_Click(object sender, EventArgs e)
        {
            //if (WSecLevel  4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS - Sec Level 2, 3 and 4");
            //    return;
            //}

            //TEST
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm("AB102");

            if (Ac.TypeOfRepl == "20" & Ac.CitId == "2000" & Ac.CashInType == "At Defined Maximum")
            {
                // It OK 
            }
            else
            {
                MessageBox.Show("Run Refresh Testing Data" + Environment.NewLine
                                + "Set For ATM102 " + Environment.NewLine
                                 + "Type Of Repl = 20 " + Environment.NewLine
                                  + "Repl Operator = 2000 " + Environment.NewLine
                                   + "Cash In Type = At Defined Maximum " + Environment.NewLine
                                + "Then Try Again"
                                );
                return;
            }

            Ac.ReadAtm("AB104");

            if (Ac.TypeOfRepl == "20" & Ac.CitId == "2000" & Ac.CashInType == "At Defined Maximum")
            {
                // It OK 
            }
            else
            {
                MessageBox.Show("Run Refresh Testing Data" + Environment.NewLine
                                + "Set For ATM104 " + Environment.NewLine
                                 + "Type Of Repl = 20 " + Environment.NewLine
                                  + "Repl Operator = 2000 " + Environment.NewLine
                                   + "Cash In Type = At Defined Maximum " + Environment.NewLine
                                + "Then Try Again"
                                );
                return;
            }


            Form50b NForm50b;
            string Function = "ATMsInNeed";
            NForm50b = new Form50b(WSignedId, WSignRecordNo, WOperator, Function);
            NForm50b.ShowDialog();
        }

        // Prepare Money In 
        private void button94_Click(object sender, EventArgs e)
        {
           

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

            string Function = "PrepareMoneyIn";
            string WCitId = "1000";
            //
            // CREATE CYCLE
            //
            CreateNewOrdersCycle(WCitId, Function); 

            // If Cycle Exist check if Authorisation process exist
            //
          
            // For NBG check 
            if (WOperator == "ETHNCY2N" ||  WOperator == "CRBAGRAA")
            {
                CheckIfAnyForNBG_FIX(WCitId, Function, WSignedId);

                if (NBG_Repl == 0)
                {
                    MessageBox.Show("No ATM ready to replenish for NBG Branch.");
                    return; 
                }

            }
            string WSelectionCriteria = " Where CitId='" + WCitId + "' AND OrdersFunction='" + Function + "' AND ProcessStage != 3 ";
            Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);
            if (Coc.RecordFound == true)
            {
                // There is an open Cycle
                // Check LAST RECORD if Already in authorization process

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WCitId, Coc.SeqNo, "ReplOrders"); //
                if (Ap.RecordFound == true & Ap.Stage < 5 & Ap.OpenRecord == true) // Already exist Repl authorisation 
                {
                    MessageBox.Show("This Order Process Already has authorization record!" + Environment.NewLine
                                             + "Go to Pending Authorisations process to complete.");

                    return;
                }

            }

            // STEPLEVEL
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form273_AUDI NForm273_AUDI;

            NForm273_AUDI = new Form273_AUDI(WSignedId, WSignRecordNo, WOperator, WCitId, WOrdersCycle, Function);
            NForm273_AUDI.ShowDialog();
        }

        //
        // METHOD THAT IS NEEDED FOR ORDERS
        //

        int WProcessStage;
        DateTime WOutputDate;
        int WOrdersCycle;

        private void CreateNewOrdersCycle(string InCitId, string InFunction)
        {
            RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();
            RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();
            // If not already exist and Open create a new one
            string WSelectionCriteria = " Where CitId='" + InCitId + "' AND OrdersFunction='" + InFunction + "' AND ProcessStage != 3 ";
            Coc.ReadExcelOutputCyclesBySelectionCriteria(WSelectionCriteria);
            if (Coc.RecordFound == true)
            {
                // There is an open Cycle
                WOrdersCycle = Coc.SeqNo;
                WProcessStage = Coc.ProcessStage;
                WOutputDate = Coc.CreatedDateTm;

                // Delete all orders for this Open Cycle

                Ro.DeleteReplOrderForThisCycle(WOrdersCycle);

                // Delete from ReplOrders_Pre_ATMs
                RRDMReplOrders_Pre_ATMs Rpre = new RRDMReplOrders_Pre_ATMs();
                Rpre.DeleteReplOrders_Pre_ATMs(WOrdersCycle);
            }
            else
            {
                // Create a new cycle

                Coc.CitId = InCitId;
                Coc.Description = "Orders Cycle for CIT.." + InCitId;
                Coc.OrdersFunction = InFunction;
                Coc.CreatedDateTm = WOutputDate = DateTime.Now;
                Coc.MakerId = WSignedId;
                Coc.ProcessStage = 1;
                Coc.Operator = WOperator;

                WOrdersCycle = Coc.InsertExcelOutputCycle();

                WProcessStage = 1;

            }
        }

        //
        // Check if any to replenish
        //
        bool Temp_NBG_FIX;
        int NBG_Repl;
        private void CheckIfAnyForNBG_FIX(string InCitId, string InFunction, string InSignedId)
        {
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, "",
            //                                InFunction, InCitId);

            NBG_Repl = 0 ;
            //-------------------
            int J = 0;
            //
            // LOOP 
            //
            while (J <= Am.TableATMsMainSelected.Rows.Count - 1)
            {
                if (InFunction == "PrepareMoneyIn")
                {
                    // THIS A TEMPORARY FIX FOR NBG
                    if (WOperator == "ETHNCY2N" ||  WOperator == "CRBAGRAA") Temp_NBG_FIX = true;
                    else Temp_NBG_FIX = false;

                }
                int WCurrentSesNo = 0;
                // Read next ATM
                string WAtmNo = (string)Am.TableATMsMainSelected.Rows[J]["AtmNo"];
                Ac.ReadAtm(WAtmNo);
                if (Ac.ActiveAtm == true)
                {
                    Ta.FindNextAndLastReplCycleId(WAtmNo);
                    if (Ta.RecordFound == true)
                    {
                        WCurrentSesNo = Ta.Last_1; // Current open session
                        if (Temp_NBG_FIX == true)
                        {
                            if (Ta.ReplOutstanding > 0)
                            {
                                WCurrentSesNo = Ta.NextReplNo;
                                NBG_Repl = NBG_Repl + 1;
                            }
                            else
                            {
                                //  NBG_No_Repl = NBG_No_Repl + 1; 
                            }
                        }
                        // Initialise variables
                        J++;
                        continue;
                    }
                    else
                    {
                        // ATM without first SM
                        J++;
                        continue;
                    }
                }
                else
                {
                    J++;
                    continue;
                }
            }
        }

        void NForm50b_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // GO TO MANAGE ACTIONS 
        private void button61_Click(object sender, EventArgs e)
        {
            //TEST
            /*
            if (WSecLevel > 4)
            {
                MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS - Sec Level 3 and 4");
                return;
            }
             */
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }

            NForm63 = new Form63(WSignedId, WSignRecordNo, WOperator);
            NForm63.FormClosed += NForm63_FormClosed;
            NForm63.ShowDialog(); ;
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

        // My Transactions 
        private void button59_Click_1(object sender, EventArgs e)
        {
          
            WAction = 7; // Go to Print Transactions by session or period  
            NForm52 = new Form52(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm52.ShowDialog(); ;
        }


        //
        // G4S Can calculate what money to put in before going for replenishment 
        //
        private void button62_Click(object sender, EventArgs e)
        {
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }
            WAction = 8; // 
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }


        //PRESENT AND MANAGE DEPOSITS
        private void button34_Click(object sender, EventArgs e)
        {
            //if (Environment.UserName != "Admin")
            //{
            //    MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT-System under investigation ");
            //    return;
            //}
            //if (WSignedId == "1006")
            //{
            //    MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
            //        + "Please use user 1005.");
            //    return;
            //}
            // Go to FORM52 
            // SHOW deposits
            // See them on Journal
            // Input amounts for exceptions

            // Move exceptions to soft disputes 

            // Print report for approval and filing 

            WAction = 4;
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }

        //
        // NEW FORM ... NEW FORM 
        // 
        // EXCEPTIONS Figuers for replenishemnt 
        private void button55_Click(object sender, EventArgs e)
        {
            WAction = 1;
            NForm10 = new Form10(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm10.ShowDialog(); ;
        }


        // MIS REPORTS 

        private void button63_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  


            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 1;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 1;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }

        // MIS REPORTS - PERSONNEL PERFORMANCE 

        private void button65_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  


            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 2;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 2;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }


        // MIS REPORTS - ATMS DAILY BUSINESS 


        private void button64_Click(object sender, EventArgs e)
        {
            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 3;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 3;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }

        // MIS REPORTS - ATMS PROFITABILITY  

        private void button20_Click(object sender, EventArgs e)
        {
            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 6;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 6;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }
        // CIT PROVIDERS 
        private void button26_Click(object sender, EventArgs e)
        {
            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 4;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 4;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }

        // DISPUTES MIS 
        private void button17_Click(object sender, EventArgs e)
        {

            // 
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            Ba.ReadNoBanksInGroup(Ba.GroupName);
            if (Ba.BanksInGroup > 1)
            {
                int WF = 5;
                NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                NForm136.ShowDialog(); ;
            }

            if (Ba.BanksInGroup == 1)
            {
                WAction = 5;

                NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                NForm21MIS.ShowDialog(); ;
            }

        }



        private void button23_Click(object sender, EventArgs e)
        {
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }

        // General Audit trail 
        // This will be based on screen image captured 

        private void button19_Click(object sender, EventArgs e)
        {
            Form76 NForm76 = new Form76(WSignedId, WSignRecordNo, WOperator);
            NForm76.ShowDialog(); ;
            //MessageBox.Show("General Audit trail this will be based on screen image captured + record registration id");  
        }
        // WORD 
        private void button24_Click(object sender, EventArgs e)
        {
            // Form66 newForm66 = new Form66();
            // newForm66.ShowDialog(); ;
        }

        // DAILY REPORTS 
        private void button16_Click(object sender, EventArgs e)
        {
            int Wf = 0;
            NForm9 = new Form9(WSignedId, WSignRecordNo, WSecLevel, WOperator, Wf);
            NForm9.ShowDialog(); ;

        }
    
        // ATMs Migration 
        private void button30_Click(object sender, EventArgs e)
        {
            RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

            int Mode = 23; // Deposits 
            //Tc.ReadInPoolTransFromPoolAndInsertInMasterPoolATMS(Mode);
        }


        // ATMs Controller's Health Check
        private void button31_Click(object sender, EventArgs e)
        {

            NForm81 = new Form81(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm81.ShowDialog(); ;
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


        // DEPATURE FROM QUALITY INDICATORS
        private void button32_Click(object sender, EventArgs e)
        {
            NForm193 = new Form193(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm193.ShowDialog(); ;
        }
        // CIT Providers 
        private void button33_Click(object sender, EventArgs e)
        {
            /*  int ParSec = 100 + WSecLevel ; // 103 say 
              Gp.ReadParametersSpecificNm(ParSec, "button33");
              if (Gp.RecordFound == false)
              {
                  MessageBox.Show("Not Authorised to access this button"); 
              }
             */
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }
            WAction = 1;
            NForm18 = new Form18(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm18.ShowDialog(); ;
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
            //int TempCount = Ap.CheckOutstandingAuthorizationsStage1_with_Update(WSignedId);
            ////Ap.CheckOutstandingAuthorizationsStage1(WSignedId); // There is record for authorizer with Stage 1 
            ////  if (Ap.RecordFound == true)
            //if (TempCount > 0)// There is record for authorizer with Stage 1 
            //{
            //    RRDMUsers_Applications_Roles Urs = new RRDMUsers_Applications_Roles();
            //    Urs.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, WApplication);

            //    if (Urs.MsgsAllowed == true)
            //    {
            //        //Console.Beep(2000, 500);
            //        //Console.Beep(3000, 1000);
            //        //Console.Beep(2000, 500);
            //        //MessageBox.Show(new Form { TopMost = true },
            //        //               "You have requests for authorization. " );
            //    }
            //}
            //TempCount = Ap.CheckOutstandingAuthorizationsStage3_with_Update(WSignedId);
            ////Ap.CheckOutstandingAuthorizationsStage3(WSignedId); // Threre is record for Requestor with stage 3 
            ////if (Ap.RecordFound == true)
            //if (TempCount > 0)// There is record for authorizer with Stage 3 
            //{
            //    //Console.Beep(2000, 500);
            //    //Console.Beep(3000, 1000);
            //    //Console.Beep(2000, 500);
            //    //MessageBox.Show(new Form { TopMost = true },
            //    //                   "Authorization has been made." );
            //}

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

        private void CheckForMessages()
        {
            return; 
            //SQL command to get number of unread messages
            RRDMControllerMsgClass cmc = new RRDMControllerMsgClass();
            int NewNumberMsgs = cmc.CountUnreadMSGs(WSignedId);

            //If different than existing then update the number and notify use
            int OldNumberOfMsgs = NumberMsgs;

            NumberMsgs = NewNumberMsgs;

            if (NumberMsgs == 0)
            {
                labelNumberMsgs.Text = "0";
                labelNumberMsgs.Visible = false;
            }
            else
            {
                labelNumberMsgs.Visible = true;
                labelNumberMsgs.Text = NumberMsgs.ToString();
            }

            if (OldNumberOfMsgs < NewNumberMsgs)
            {

                MessageBox.Show(new Form { TopMost = true }, "You have new messages.");
            }
        }
        private void labelNumberMsgs_Click(object sender, EventArgs e)
        {

        }
        //
        // PARAMETERS MANAGEMENT 
        //
        private void button21_Click(object sender, EventArgs e)
        {
            NForm191 = new Form191(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm191.ShowDialog();
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
        // DEFINE QUALITY INDICATORS 
        private void button36_Click(object sender, EventArgs e)
        {
            NForm192 = new Form192(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm192.ShowDialog(); ;
        }
        // ACCOUNTS MANAGEMENT 
        private void button18_Click_1(object sender, EventArgs e)
        {
            NForm85 = new Form85(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85.ShowDialog(); ;
        }
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // MIS FOR DISPUTES 
        private void button27_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Closed Disputes by Officer.");
        }
        // E_JOURNAL DRILLING 

        private void button91_Click(object sender, EventArgs e)
        {
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT-System under investigation ");
                return;
            }
            NForm53 = new Form53(WSignedId, WSignRecordNo, WOperator);
            NForm53.ShowDialog(); ;
        }
        //
        // System Performance
        //
        private void button74_Click(object sender, EventArgs e)
        {
            WAction = 1;

            NForm8  = new Form8(WSignedId, WOperator, "", WAction);
            NForm8.ShowDialog(); ;

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

        private void button73_Click(object sender, EventArgs e)
        {
            if (HasInternet())
            {
                try
                {
                    // Read the URL from app.config
                    string MainURL = ConfigurationManager.AppSettings["RRDMMapsMainURL"];
                    // Invoke default browser
                    ProcessStartInfo sInfo = new ProcessStartInfo(MainURL);
                    Process.Start(sInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maps cannot be shown. There might be a problem with Google Maps" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("There is no Internet Connection");
            }

        }

        // Copy Trans from AB102 to ServeUk102
        private void button1_Click(object sender, EventArgs e)
        {
            RRDMJTMQueue Jq = new RRDMJTMQueue();

            Jq.ReadJTMQueueByMsgID(1);

            Jq.UpdateRecordInJTMQueueByMsgID(1);

            Jq.InsertNewRecordInJTMQueue();

            //// STAVROS CALCULATION 
            //NForm2Testing = new Form2Testing();
            //NForm2Testing.ShowDialog(); ;

            //     TransAndPostedClass Tc = new TransAndPostedClass();
            //     HostTransClass Ht = new HostTransClass();
            //     AtmsDailyTransHistory Ah = new AtmsDailyTransHistory(); 
            //     GasParameters Gp = new GasParameters(); 
            //      string BankId = "ServeUk";
            //      string AtmNo = "ServeUk102";
            //     string BankId = "CRBAGRAA";

            //      Gp.CopyParameters("ServeUk","CRBAGRAA");
            //      string AtmNo = "AB102";
            //      int SesNo = 6694 ;
            //      string TargetBank = "ServeUk";
            //      string TargetAtm = "ServeUk102";
            //      int TargetSesNo = 6695 ;
            //       bool TargetPrive = false; 

            // Pool Transactions
            //          Tc.CopyInPoolTrans(BankId, AtmNo, SesNo, TargetBank, TargetAtm, TargetSesNo, TargetPrive);

            // Host Trans 
            //  Ht.CopyHostTrans(BankId, AtmNo, TargetBank, TargetAtm, TargetPrive);

            // Copy History 
            //          Ah.CopyTransHistory(BankId, AtmNo, TargetBank, TargetAtm, TargetPrive);

            //       Ht.CopyFromPoolToTrans();// Copies on in pool record to host file 

            //      MessageBox.Show("Trans copied from: " + AtmNo + " To :" + TargetAtm) ; 

        }
        // Future MIS
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Transaction Mixed + Balance enquiries + Withdrawls + Deposits + Etc.");
        }
        // Future MIS
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Service Level per Model. Is NCR faster than Diebold? ");
        }
        // Future MIS 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Destination of Transactions , what Own systems , other destinations not own? ");
        }
        // Future MIS 
#pragma warning disable IDE1006 // Naming Styles
        private void button80_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            MessageBox.Show("Future development " + "Authorisers = the Good , the Bad and the Neutral ");
        }
        // Future Module 
        private void button76_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + "Module that provides information to the Banks' corporate WareHouse. ");
        }
       


        // Money Notes quality 
        private void button81_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + Environment.NewLine
                + "Based on Reject Bin volumes quality of insert Notes is measured " + Environment.NewLine
                + "Excess by the machine Reject is an evidence that quality of Notes is not that good!");
        }

        // Software version details 
        private void button6_Click(object sender, EventArgs e)
        {

            AboutBox1 about = new AboutBox1();
            about.ShowDialog();

        }
        // Make RED 
        private void button2_Click_1(object sender, EventArgs e)
        {
            // china bank for Journal and CVV
            Color Red = Color.Red;

            button15.BackColor = Red;
            button10.BackColor = Red;
            button34.BackColor = Red;
           // button59.BackColor = Red;

            button91.BackColor = Red;
            //button25.BackColor = Red;
            ////button34.BackColor = Red;
            //button73.BackColor = Red;
            //button12.BackColor = Red;

            //button26.BackColor = Red;
            //button64.BackColor = Red;
            //button65.BackColor = Red;

            //button28.BackColor = Red;
            //button18.BackColor = Red;
            //button29.BackColor = Red;
            //button55.BackColor = Red;

            //button33.BackColor = Red;

            //button60.BackColor = Red;
            //button61.BackColor = Red;
        }
        // Make Silver 
        private void button3_Click_1(object sender, EventArgs e)
        {
            Color Silver = Color.Silver;
            Color Black = Color.Black;
            button57.BackColor = Silver;
            button57.ForeColor = Black;
            //button58.BackColor = Silver;
            //button58.ForeColor = Black;
            button34.BackColor = Silver;
            button34.ForeColor = Black;
            button67.BackColor = Silver;
            button67.ForeColor = Black;
            button68.BackColor = Silver;
            button68.ForeColor = Black;
            button69.BackColor = Silver;
            button69.ForeColor = Black;
            //button63.BackColor = Silver;
            //button63.ForeColor = Black;
            //button20.BackColor = Silver;
            //button20.ForeColor = Black;
            //button17.BackColor = Silver;
            //button17.ForeColor = Black;
            button75.BackColor = Silver;
            button75.ForeColor = Black;
            button35.BackColor = Silver;
            button35.ForeColor = Black;
            button37.BackColor = Silver;
            button37.ForeColor = Black;
            //button32.BackColor = Silver;
            //button32.ForeColor = Black;
            //button74.BackColor = Silver;
            //button74.ForeColor = Black;
            //button31.BackColor = Silver;
            //button31.ForeColor = Black;
            button11.BackColor = Silver;
            button11.ForeColor = Black;
        }
        // Start Reporting Services 
        private void button4_Click_1(object sender, EventArgs e)
        {
            // PLEASE DO NOT DELETE THIS
            //Form56R11 AtmsBasic = new Form56R11(WOperator);
            //AtmsBasic.Show();
        }
        // Check Internet - SMS SMS SMS
        private void button5_Click(object sender, EventArgs e)
        {

            //bulksms_dotnetlib.CBSMS_Http smstext = new bulksms_dotnetlib.CBSMS_Http();

            //string smsg = "Your ATM AB104 has a Presenter Error. Kat please send me sms to 00357 99 622248 if you receive this SMS.";

            //// without 00 or +
            //string stel = "639989729852"; // FOR International 

            ////string stel = "99473358"; // For Cyprus 

            //// les than 11 characters and more than 3
            //string ssenderid = "RRDM ATMs";
            //string sLogin = "rrmd";
            //string sPwd = "rrmd9";
            //string sid = System.Guid.NewGuid().ToString();
            //string sresult = "";
            //string sbuf = "http://www.altavie.com.cy/getbsms/receive.aspx?id=" + sid;
            //string statuscode = "";
            //char[] sep = { '=' };

            //sbuf += "&msisdn=" + stel + "&sms=" + smsg + "&srvno=" + ssenderid + "&Provider=Cyta&login=" + sLogin + "&pwd=" + sPwd;

            //sresult = smstext.HTTPSend(sbuf);

            //Array a = sresult.Split(sep);

            //string schara = a.GetValue(0).ToString().Trim();

            //Array aa = sresult.Split('|');

            //string sres = a.GetValue(0).ToString().Substring(0, 2).Trim();

            //switch (sres)
            //{
            //    //0: In progress
            //    case "OK":
            //        statuscode = "D";
            //        break;

            //    default:
            //        statuscode = "U";
            //        break;
            //}

            //MessageBox.Show("SMS Has been sent to : " + stel); 


            ///
            // CREATE IN POOL FOR KONTO
            //
            //
            //RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass(); 
            //Tp.ReadInPoolTransSpecificForKONTO(); 

            //return;

            RRDMMatchingCategoriesVsSourcesFiles Rs = new RRDMMatchingCategoriesVsSourcesFiles();

            Rs.ReadAndCallKontoToReconcile();

            if (HasInternet())
            {
                MessageBox.Show("There is Internet Connection");
            }
            else
            {
                MessageBox.Show("There is NO Internet Connection");
            }

            RRDMEmailClass2 Em = new RRDMEmailClass2();

            string Recipient = "panicos.michael@cablenet.com.cy";

            string Subject = "Replenish ATM";

            string WEmailContent = "Testing Internet";

            Em.SendEmail(WOperator, Recipient, Subject, WEmailContent);
            if (Em.MessageSent == true)
            {
                MessageBox.Show("NO Problem with Google Email.");
            }
            else
            {
                MessageBox.Show("There is problem with Google Email." + Environment.NewLine
                    + " Email is used at differrent locations at the same time." + Environment.NewLine
                    + " Google has blocked acount for this reason");
            }
        }

        private bool HasInternet()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
    

        void NForm80a1_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
        // TXNS RECONCILIATION
        private void button48_Click(object sender, EventArgs e)
        {
            if (WSignedId == "1005")
            {
                MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
                    + "Please use user 1006.");
                return;
            }

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            //if (WCut_Off_Date != DateTime.Now.Date)
            //{
            //    if (WCut_Off_Date < DateTime.Now.Date.AddDays(-7))
            //    {
            //        // Do nothing
            //    }
            //    else
            //    {
            //        MessageBox.Show("Manager Didn't Start A New Cut Off." + Environment.NewLine
            //                       + "Please Inform Him To Do So Before You Start.");
            //    }

            //}

            Form80a1 NForm80a1;

            string WFunction = "Reconc";
            string WRMCategory = "ALL";
            string WhatBank = "";
            NForm80a1 = new Form80a1(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank);
            NForm80a1.FormClosed += NForm80a2_FormClosed;
            NForm80a1.ShowDialog();
        }
        // ATMS CASH RECONCILIATION
        private void button92_Click(object sender, EventArgs e)
        {
            Form80a2 NForm80a2;

            string WFunction = "Reconc";
            string WRMCategory = "";
            string WhatBank = "";
            NForm80a2 = new Form80a2(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank);
            NForm80a2.FormClosed += NForm80a2_FormClosed;
            NForm80a2.ShowDialog();

            //Form80a NForm80a;        
            //string WFunction = "Reconc";
            //string WRMCategory = "EWB110" ;
            //NForm80a = new Form80a(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory);
            //NForm80a.FormClosed += NForm80a_FormClosed;
            //NForm80a.ShowDialog();
        }

        void NForm80a2_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
        void NForm80a_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // Mass Categories and matching cycles 

        private void button11_Click(object sender, EventArgs e)
        {
            if (WSignedId == "1005")
            {
                MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
                    + "Please use user 1006.");
                return;
            }

            Form80a1 NForm80a1;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WBankId;

            NForm80a1 = new Form80a1(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            NForm80a1.FormClosed += NForm80a1_FormClosed;

            NForm80a1.ShowDialog();

        }
        // DEFINITION OF PRODUCTS LIFE CYCLE 
        private void button83_Click(object sender, EventArgs e)
        {
            Form502a NForm502;

            NForm502 = new Form502a(WSignedId, WSignRecordNo, WOperator);
            NForm502.ShowDialog();
        }
        // DEFINE CATEGORIES  
        private void button84_Click(object sender, EventArgs e)
        {
            Form503 NForm503;
            int Mode = 1; //  IS ATMS
            NForm503 = new Form503(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            NForm503.ShowDialog();
        }
        // Work Allocation 
        private void button86_Click(object sender, EventArgs e)
        {
            if (WSignedId == "1005")
            {
                MessageBox.Show("For this Demo this user cannot perform this function." + Environment.NewLine
                    + "Please use user 1006.");
                return;
            }
            Form80cATMs NForm80cATMs;
            string WFunction = "Allocate";
            NForm80cATMs = new Form80cATMs(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm80cATMs.ShowDialog();
        }

        // E-Journal loading 
        private void button89_Click(object sender, EventArgs e)
        {
            Form200bATMs NForm200bATMs;

            string RunningGroup = "ITMX-FT";
            NForm200bATMs = new Form200bATMs(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm200bATMs.ShowDialog();
        }

        // Journal Loading Status 
        private void button96_Click(object sender, EventArgs e)
        {
            Form200cATMs NForm200cATMs;

            NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, WSecLevel, WOperator, "");
            NForm200cATMs.ShowDialog();
        }
        // Define ERROR IDs and Rules 
        private void button87_Click(object sender, EventArgs e)
        {
            Form66 NForm66;
            int WFunction = 1;
            NForm66 = new Form66(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm66.ShowDialog();
        }
      
        // MAKE KINA
        private void button7_Click(object sender, EventArgs e)
        {
            // china bank for Journal and CVV
            Color Red = Color.Red;

            button15.BackColor = Red;
            button10.BackColor = Red;
            button34.BackColor = Red;
           // button59.BackColor = Red;

            button91.BackColor = Red;
          
            button57.BackColor = Red;
           
            //button34.BackColor = Red;
            button35.BackColor = Red;


            button75.BackColor = Red;


            button16.BackColor = Red;

           
            labelStep1.Text = "Main Menu - Primer";

        }
        // Make WEB
        private void button93_Click(object sender, EventArgs e)
        {
            // Make WHAT IS NECESSARY TO BE WEB
            Color Red = Color.Red;
            button15.BackColor = Red;
            button57.BackColor = Red;
            button10.BackColor = Red;
            button34.BackColor = Red;
           // button59.BackColor = Red;
            button91.BackColor = Red;
            button75.BackColor = Red;
            //button25.BackColor = Red;

            button35.BackColor = Red;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            FormHelp help = new FormHelp();
            help.Show();
        }
        //
        // E-Journal Loading Service 
        //
        private void button90_Click(object sender, EventArgs e)
        {
            RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
            RRDMJTMQueue Jq = new RRDMJTMQueue();
            RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();
            RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

            string WAtmNo = "";

            string WCommand;
            string InMode;

            int WPriority;

            // At reasonable time intervals this service starts 

            //string connectionString = ConfigurationManager.ConnectionStrings
            //  ["ATMSConnectionString"].ConnectionString;

            //string RCT = "ATMS.[dbo].[Stp_Refresh_Testing_Data]";

            //using (SqlConnection conn2 =
            //   new SqlConnection(connectionString))
            //    try
            //    {
            //        conn2.Open();
            //        using (SqlCommand cmd =
            //           new SqlCommand(RCT, conn2))
            //        {
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            // Parameters

            //            int rows = cmd.ExecuteNonQuery();
            //            //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
            //            //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

            //        }
            //        // Close conn
            //        conn2.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
            //    }


            try
            {
                // SUMMARY
                // FIRST PROCESS
                // 
                // Read all ATMs Journal Identification Details where ready to be loaded
                // Records with ResultCode = 1 = ready and 
                // Next Loading date < Current date and time. 
                // 
                // Insert In queue to be read by Alecos 
                // 
                // Set ResultCode to -1 
                // 
                // ALECOS PROCESS 
                // 
                // Reads all active in Queue and loads journal and parse it
                // 
                // Upon Loading Sets ResultCode = 0  
                // 
                // SECOND PROCESS 
                // 
                // Read all ATMs Journal Identification Details where ready to be loaded
                // Records with ResultCode = 0 = Journal is loaded and parsed by Alecos 
                // 
                // For each record with ResultCode = 0 
                // Read the parsed records and update RRDM
                // 
                // Calculate next Loading Date Time 
                // Set ResultCode to 1 
                // 

                //
                //
                // Process 
                //
                //


                // InCommand : Fetch
                //           : GETDEL
                // InMode    : SingleAtm
                //           : AllReadyForLoading
                //           : GetAllLoaded

                // All ATM
                WCommand = "FETCH";
                InMode = "AllReadyForLoading";

                WPriority = 1; // Highest priority  

                Jq.InsertRecordsInJTMQueue(WSignedId, WCommand, InMode, WPriority, WAtmNo);
                if (Jq.ErrorFound == true)
                {
                    return;
                }

                MessageBox.Show("Total Records Inserted In Queue For Loading = " + Jq.TotalInserted.ToString());
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

            bool RecordFound = false;
            bool ErrorFound = false;
            string ErrorOutput = "";

            try
            {
                MessageBox.Show("System Ready to read from Loaded." + Environment.NewLine
                  + "Activate Loading Service. " + Environment.NewLine
                  + "Upon Finishing press OK to proceed  "
                                        );

                InMode = "GetAllLoaded"; // Result code = 0 - Set by Alecos 
                Jd.ReadJTMIdentificationDetailsToFillFullTable(InMode, WAtmNo);

                if (Jd.RecordFound == true)
                {
                    RecordFound = true;

                    // Read all records of created Table and insert in Queque 

                    int I = 0;

                    MessageBox.Show("Tolal Parsed By JTM :" + Jd.ATMsJournalDetailsTable.Rows.Count.ToString());

                    while (I <= (Jd.ATMsJournalDetailsTable.Rows.Count - 1))
                    {
                        int WSeqNo = (int)Jd.ATMsJournalDetailsTable.Rows[I]["SeqNo"];

                        WAtmNo = (string)Jd.ATMsJournalDetailsTable.Rows[I]["AtmNo"];

                        // UPDATE RRDM 

                        //UpDate Atm with latest status of transactions from Journal 
                        Aj.UpdateLatestEjStatusForSpecificAtm
                                        (WSignedId, WSignRecordNo, WOperator, WAtmNo);
                        //
                        // READ AND UPDATE ATM JOURNAL DETAILS  
                        // 
                        Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

                        // Calculate Next Loading date 
                        // Last loaded Date not available then insert current date 
                        //
                        DateTime WLastParsedDtTm = Jd.FileParseEnd; // Set working date as per Loaded date
                        //

                        Jd.NextLoadingDtTm = Js.ReadCalculatedNextEventDateTm(WOperator, Jd.LoadingScheduleID,
                                                                    DateTime.Now, WLastParsedDtTm);

                        Jd.LoadingCompleted = DateTime.Now;
                        Jd.ResultCode = 1;
                        Jd.ResultMessage = "Ready";

                        Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(WAtmNo);
                        if (Jd.ErrorFound == true)
                        {
                            ErrorFound = true;
                            return;
                        }
                        I++;
                    }
                }
                else
                {
                    RecordFound = false;
                    MessageBox.Show("No Records Found For RRDM.");
                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }

        // Timer two 
        private void timer2_Tick(object sender, EventArgs e)
        {
            // It operates every ten seconds 

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

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            //Environment.Exit(0);
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
                WJobCategory = "ATMs";

                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            }
            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, WReconcCycleNo,
                "",NullPastDate,NullPastDate,1
                );
            NForm80b2.ShowDialog();
        }


        // Close current And open new one 
        private void button50_Click(object sender, EventArgs e)
        {

            Form200JobCycles NForm200JobCycles;

            string RunningGroup = "ATMs";
            NForm200JobCycles = new Form200JobCycles(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles.FormClosed += NForm152_FormClosed;
            NForm200JobCycles.ShowDialog();

        }
        // Help One
        private void checkBoxHelp1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp1.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "GENERAL ON REPLENISHEMENT WORKFLOW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Needed Money";
                buttonHelp2.Text = "Follow Replenishment WorkFlow";
                buttonHelp3.Text = "Authorised and Posting of TXNs";

            }
            else
            {
                // False
                if (checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false & checkBoxHelp6.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }

        // Help 2
        private void checkBoxHelp2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp2.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "REPLENISHEMENT WORKFLOW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp1.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Physical Check";
                buttonHelp2.Text = "Input Counted money";
                buttonHelp3.Text = "Reload ATM";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp3.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false & checkBoxHelp6.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }

        // Help 3
        private void checkBoxHelp3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp3.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "DEPOSITS WORKFLOW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Check Discrepancies";
                buttonHelp2.Text = "Move to Disputes";
                buttonHelp3.Text = "Resolution";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false & checkBoxHelp6.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }

        }
        // Four 
        private void checkBoxHelp4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp4.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "RECONCILIATION WORKFLOW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Close/Open Job Cycle";
                buttonHelp2.Text = "Work Allocation";
                buttonHelp3.Text = "Actions on Discepancies";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp5.Checked == false & checkBoxHelp6.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // Five 
        private void checkBoxHelp5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp5.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "DISPUTES WORKFLOW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Pre-Investigation";
                buttonHelp2.Text = "Registration";
                buttonHelp3.Text = "Dispute Management";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp6.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // Sixth
        private void checkBoxHelp6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp6.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "MONITORING";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Monitor Dashboard";
                buttonHelp2.Text = "Monitor Quality";
                buttonHelp3.Text = "Send Messages";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // JOURNAL LOADING STATUS
        private void button96_Click_1(object sender, EventArgs e)
        {
            Form200cATMs NForm200cATMs;

            NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, WSecLevel, WOperator, "");
            NForm200cATMs.ShowDialog();
        }
        // CONTROLS LISTING 
        private void button13_Click_1(object sender, EventArgs e)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
            string WSelectionCriteria;

            string MainFormId = "Form1 - ATMs - Operational";

            //Clear Table 
            Tr.DeleteReport57(WSignedId);

            Ur.IsUpdated = false;

            Ur.UpdateUsersAccessRightsInitialiseWithIsUpdated(MainFormId);
            // REPLENISHEMENT
            foreach (Control c in Replenishment.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Replenishment.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                          + " AND PanelName ='" + Tr.PanelName + "'"
                                          + " AND ButtonName ='" + Tr.ButtonName + "'"
                                          + " AND Operator ='" + WOperator + "'"
                                          ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }

                    }
                }
            }


            // RECONCILIATION           
            foreach (Control c in Reconciliation.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Reconciliation.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                         + " AND PanelName ='" + Tr.PanelName + "'"
                                         + " AND ButtonName ='" + Tr.ButtonName + "'"
                                         + " AND Operator ='" + WOperator + "'"
                                         ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }
                    }
                }
            }

            // DISPUTES

            foreach (Control c in Disputes.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Disputes.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                        + " AND PanelName ='" + Tr.PanelName + "'"
                                        + " AND ButtonName ='" + Tr.ButtonName + "'"
                                        + " AND Operator ='" + WOperator + "'"
                                        ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }
                    }
                }
            }

            // MONITORING
            //
            foreach (Control c in Monitoring.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Monitoring.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                        + " AND PanelName ='" + Tr.PanelName + "'"
                                        + " AND ButtonName ='" + Tr.ButtonName + "'"
                                        + " AND Operator ='" + WOperator + "'"
                                        ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }
                    }
                }
            }

           

            // TXNs Posting 
            foreach (Control c in TXNS_POSTING.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = TXNS_POSTING.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                        + " AND PanelName ='" + Tr.PanelName + "'"
                                        + " AND ButtonName ='" + Tr.ButtonName + "'"
                                        + " AND Operator ='" + WOperator + "'"
                                        ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }
                    }
                }
            }
            // Authorisation 
            foreach (Control c in Authorisation.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Authorisation.Name;
                    Tr.ButtonName = c.Name;
                    Tr.ButtonText = c.Text;
                    if (c.Text != "")
                    {
                        Tr.InsertReport57(WOperator, WSignedId);

                        WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
                                        + " AND PanelName ='" + Tr.PanelName + "'"
                                        + " AND ButtonName ='" + Tr.ButtonName + "'"
                                        + " AND Operator ='" + WOperator + "'"
                                        ;

                        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
                        if (Ur.RecordFound == true)
                        {
                            // Only update

                            Ur.ButtonText = c.Text;
                            Ur.IsUpdated = true;

                            Ur.UpdateUsersAccessRights(Ur.SeqNo);
                        }
                        else
                        {
                            // Insert 

                            Ur.MainFormId = Tr.MainFormId;
                            Ur.PanelName = Tr.PanelName;
                            Ur.ButtonName = Tr.ButtonName;
                            Ur.ButtonText = Tr.ButtonText;

                            Ur.SecLevel02 = false;
                            Ur.SecLevel03 = false;
                            Ur.SecLevel04 = false;
                            Ur.SecLevel05 = false;
                            Ur.SecLevel06 = false;
                            Ur.SecLevel07 = false;
                            Ur.SecLevel08 = false;
                            Ur.SecLevel09 = false;
                            Ur.SecLevel10 = false;
                            Ur.SecLevel11 = false;
                            Ur.SecLevel12 = false;
                            Ur.SecLevel13 = false;
                            Ur.SecLevel14 = false;

                            Ur.IsUpdated = true;

                            Ur.Operator = WOperator;

                            Ur.InsertAccessRecord(Ur.ButtonName);
                        }
                    }
                }
            }

            string P1 = "Buttons DETAILS For Form1ATMs - Operational";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WBankId;
            string P5 = WSignedId;

            Form56R57ATMS ReportATMS57 = new Form56R57ATMS(P1, P2, P3, P4, P5);
            ReportATMS57.Show();


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
            Urs.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, WApplication);

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
        // CASH RECONCILATION 
        private void button6_Click_1(object sender, EventArgs e)
        {
            Form80a2 NForm80a2;

            string WFunction = "Reconc";
            string WRMCategory = "";
            string WhatBank = "TEST";
            NForm80a2 = new Form80a2(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank);
            NForm80a2.FormClosed += NForm80a2_FormClosed;
            NForm80a2.ShowDialog();
        }
        // View Cash Reconciliation 
        private void button9_Click(object sender, EventArgs e)
        {
            Form80a2 NForm80a2;

            string WFunction = "VIEW";
            string WRMCategory = "";
            string WhatBank = "TEST";
            NForm80a2 = new Form80a2(WSignedId, WSignRecordNo, WOperator, WFunction, WRMCategory, WhatBank);
            NForm80a2.FormClosed += NForm80a2_FormClosed;
            NForm80a2.ShowDialog();
        }
        // Excel Loading 
        private void button14_Click_1(object sender, EventArgs e)
        {
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }
            Form18_ExcelMainMenu NForm18_ExcelMainMenu;
            WAction = 2;
            NForm18_ExcelMainMenu = new Form18_ExcelMainMenu(WSignedId, WSignRecordNo, WSecLevel, WOperator, WCut_Off_Date, WAction);
            NForm18_ExcelMainMenu.ShowDialog();
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
// CIT ORDERING
        private void button21_Click_1(object sender, EventArgs e)
        {
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }
            Form18_CIT_Orders NForm18_CIT_Orders; 
            WAction = 1;
            NForm18_CIT_Orders = new Form18_CIT_Orders(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm18_CIT_Orders.ShowDialog(); ;
        }
// GL SECOND METHOD
        private void button23_Click_1(object sender, EventArgs e)
        {
            Form52_GL_W2 NForm52_GL_W2;

            int Mode = 2; // update 
            DateTime WDate = DateTime.Now.Date;
            NForm52_GL_W2 = new Form52_GL_W2(WOperator, WSignedId, "5000", WDate);
            NForm52_GL_W2.ShowDialog();
        }
// CATEGORY TRANSACTIONS VIEW 
        private void button24_Click_1(object sender, EventArgs e)
        {
            Form80a3 NForm80a3;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WBankId;

            NForm80a3 = new Form80a3(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            NForm80a3.FormClosed += NForm80a1_FormClosed;

            NForm80a3.ShowDialog();
        }
// Show Journals 
        private void buttonJournals_Click(object sender, EventArgs e)
        {
            Form200cATMs NForm200cATMs;

            NForm200cATMs = new Form200cATMs(WSignedId, WSignRecordNo, "5", WOperator, "");
            NForm200cATMs.ShowDialog();
        }
// GL Totals 
        private void buttonGL_Totals_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL NForm200JobCycles_GL;

            string RunningGroup = "ATMs";
            NForm200JobCycles_GL = new Form200JobCycles_GL(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles_GL.ShowDialog();
        }
// IST View
        private void buttonIST_View_Click(object sender, EventArgs e)
        {

            Form3_PreInv_IST NForm3_PreInv_IST;

            NForm3_PreInv_IST = new Form3_PreInv_IST(WSignedId, WSignRecordNo, WSecLevel, WOperator,WReconcCycleNo);

            NForm3_PreInv_IST.ShowDialog();

        }
// Create SM for Baddies 
        private void buttonCreateSM_Click(object sender, EventArgs e)
        {
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }
            Form154 NForm154;
            int WMode = 1;
            NForm154 = new Form154(WOperator, WSignedId, WReconcCycleNo, WMode,"");
            NForm154.ShowDialog();
        }
// Shortages 
        private void buttonCitShortages_Click(object sender, EventArgs e)
        {
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }
           
            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            Uar.ReadUsersVsApplicationsVsRolesByUser(WSignedId); 

            if (Uar.Authoriser == true)
            {
                DateTime NullPastDate = new DateTime(1900, 01, 01);

                Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

                Form67_BDC NForm67_BDC;

                int Mode = 1; // All Repl Cycles with Outstanding 
                string WAtmNo = "";
                int WSesNo = 0;
                NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo,
                    WReconcCycleNo, NullPastDate, NullPastDate, Mode);
                NForm67_Cycle_Rich_Picture.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Allowed Operation"+Environment.NewLine
                    + "Only Checkers are allowed. "
                    );
                return; 
            }
           
        }
// View Reports 
        private void buttonCycleReports_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Reports NForm200JobCycles_Reports;

            string RunningGroup = "ATMs";
            NForm200JobCycles_Reports = new Form200JobCycles_Reports(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm200JobCycles_Reports.ShowDialog();
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
        private void button28_Click(object sender, EventArgs e)
        {
            //
            WAction = 7; // Go to Print Transactions by session or period  
            NForm52 = new Form52(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm52.ShowDialog(); ;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            NForm53 = new Form53(WSignedId, WSignRecordNo, WOperator);
            NForm53.ShowDialog();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            WAction = 4; // Show INFO FOR ATMS By MAKER
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator, "", WAction);
            NForm47.ShowDialog(); ;
        }
// Manual creation of transaction from authoriser 
        private void buttonManual_Click(object sender, EventArgs e)
        {

            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            Uar.ReadUsersVsApplicationsVsRolesByUser(WSignedId);

            if (Uar.Authoriser == true)
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                string WJobCategory = "ATMs";
                int WReconcCycleNo;
                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                if (WReconcCycleNo != 0)
                {
                    WCut_Off_Date = Rjc.Cut_Off_Date;
                }
                else
                {
                    WCut_Off_Date = NullPastDate;
                }

                Form503_Insert_Manual_To_Mpa NForm503_Insert_Manual_To_Mpa;

                int WMode = 1;

                NForm503_Insert_Manual_To_Mpa = new Form503_Insert_Manual_To_Mpa(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WMode, T24Version);
                NForm503_Insert_Manual_To_Mpa.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not Allowed Operation" + Environment.NewLine
                    + "Only Checkers are allowed. "
                    );
                return;
            }       
        }

        private void buttonMgmtReporting_Click(object sender, EventArgs e)
        {
            Form3_Management NForm3_Management;

            NForm3_Management = new Form3_Management(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            NForm3_Management.ShowDialog();

        }
// CIt Excel Replenishment 
        private void buttonReplByCIT_Click(object sender, EventArgs e)
        {
            bool Recon_Equal_Repl = false;
            string ParId = "939";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                Recon_Equal_Repl = true;
            }
            else
            {
                Recon_Equal_Repl = false;
            }
            //
            // Check if can make replenishment
            //

            if (Recon_Equal_Repl == true)
            {

            }
            else
            {
                Ba.ReadUserAccessToAtms(WSignedId); // READ ALL TABLE ENTRIES TO SEE IF ONE TO ONE 

                if (Ba.NoOfAtmsRepl == 0 & Ba.NoOfGroupsRepl == 0)
                {
                    MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT ");
                    return;
                }

            }

            if (Ba.NoOfGroupsRepl == 0) WAction = 1; // Replenishment for individual of more than one ATM,
            //Us.ReadUsersRecord(WSignedId); // READ USER DATA
            //if (Ba.NoOfGroupsRepl > 0) WAction = 5; // Replenishment for Group of ATM 
            Form276_CIT_Replenish_FirstStep NForm276_CIT_Replenish;
            NForm276_CIT_Replenish = new Form276_CIT_Replenish_FirstStep(WSignedId, WSignRecordNo, WOperator, WAction);
            //  NForm152.FormClosed += NForm152_FormClosed;
            NForm276_CIT_Replenish.ShowDialog(); 
        }
    }
}

//multilingual
using RRDM4ATMs;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
//24-04-2015 Alecos
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form_ATMS_Controller_EMR : Form
    {
        public EventHandler LoggingOut;

        Form108 NForm108;
        //Form8 NForm8;
        Form12 NForm12;
        Form13 NForm13;
        Form15 NForm15;

        Form10 NForm10;

        Form143 NForm143;
        Form45 NForm45;

       // Form152 NForm152;

        Form54 NForm54;
        Form55 NForm55;

        string WJobCategory; 

      //  Form78 NForm78;
        Form81 NForm81;

        //Form85 NForm85;

      //  Form191 NForm191;
     //   Form192 NForm192;

        Form300 NForm300;

       // Form112 NForm112;

        string MsgFilter;

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        string WOrigin = "Our Atms";

        // NOTES 
       // string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

      //  string WParameter4 = "Main Menu - Issues For System";
       // string WSearchP4 = "";

       
        int WReconcCycleNo;

      //  bool WJournalLoadingStarted;
     //   int WQueueId;

        int WAction;
        string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

       // #region Form_ATMS_ADMIN
        public Form_ATMS_Controller_EMR(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();


            // this.WindowState = FormWindowState.Maximized;

            pictureBox1.BackgroundImage = appResImg.logo2;

            if (WOperator == "BDACEGCA")
            {
                //buttonPresenterCases.Enabled = false; 
               // buttonAccessRights.Enabled = false; 
                buttonCreateSM.Hide(); 
                //buttonGL_TOTALS.Hide(); 
                buttonCitShortages.Hide(); 
            }


            //WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            //DateTime WCut_Off_Date = Rjc.Cut_Off_Date;


            WJobCategory = "ATMs";
            //labelCycleNo.Text = WReconcCycleNo.ToString();
            //labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WSignedId == "Admin-Level10"
                 || WSignedId == "Admin-Level11")
            {
                RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();

                string MainFormId = "Form1 - ATMs - ADMIN";

                // Parameters
                foreach (Control c in Parameters.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                              + " AND Operator='" + WOperator + "'";

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
                // Entities
                foreach (Control c in Entities.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                             + " AND ButtonName='" + c.Name + "'"
                                             + " AND SecLevel" + WSecLevel + "= 1 "
                                             + " AND Operator='" + WOperator + "'";

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

                // ReconcDefinition
                foreach (Control c in ReconcDefinition.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                              + " AND ButtonName='" + c.Name + "'"
                                              + " AND SecLevel" + WSecLevel + "= 1 "
                                              + " AND Operator='" + WOperator + "'";

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
                // AdminQuality
                foreach (Control c in AdminQuality.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                               + " AND ButtonName='" + c.Name + "'"
                                               + " AND SecLevel" + WSecLevel + "= 1 "
                                               + " AND Operator='" + WOperator + "'";

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
                // AdminAuthorisation
                //foreach (Control c in AdminAuthorisation.Controls)
                //{
                //    if (c is Button)
                //    {

                //        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                //                             + " AND ButtonName='" + c.Name + "'"
                //                             + " AND SecLevel" + WSecLevel + "= 1 "
                //                             + " AND Operator='" + WOperator + "'";

                //        Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);

                //        if (Ur.RecordFound == true)
                //        {
                //            c.Enabled = true;
                //        }
                //        else
                //        {
                //            c.Enabled = false;
                //            c.BackColor = Color.Silver;
                //            c.ForeColor = Color.DarkGray;
                //        }

                //    }
                //}
            }


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

            toolTipButtons.SetToolTip(label2, "INFRASTRUCTURE" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );

            //toolTipButtons.SetToolTip(label3, "OPERATION" + Environment.NewLine
            //                             + "Through the below buttons you operate the system."
            //                             );

            toolTipButtons.SetToolTip(labelParameters, "DISPUTES" + Environment.NewLine
                                        + "Register and manage disputes."
                                        );

            toolTipButtons.SetToolTip(label5, "QUALITY" + Environment.NewLine
                                       + "Audit Trail and Exception Reports for KPI."
                                       );
            //toolTipButtons.SetToolTip(label11, "ELECTRONIC AUTHORISATIONS" + Environment.NewLine
            //                           + "Management of Authorisations for " + Environment.NewLine
            //                           + "Requestors and Authorisers ."
            //                           );
        }
       // #endregion
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
        private void FormMainScreen_Load(object sender, EventArgs e)
        {
            // Reset 

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
            //string AtmNo = "";
            //string FromFunction = "";

            //FromFunction = "General";

            //string WCitId = "1000";
            // Create table with the ATMs this user can access
            // Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            //Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            // See buttons CIT Management 
            //
            string ParId = "955";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            
            if (Gp.RecordFound & Gp.OccuranceNm == "YES" & WOperator != "ALPHA_CY")
            {
                buttonExcelLoading.Show();
                //buttonCIT_Mgmt.Show();
                //buttonReplReport.Show();
                //buttonGL_View.Show();
                buttonAudiReports.Show(); 
            }
            else
            {
                buttonExcelLoading.Hide();
                buttonCIT_Mgmt.Hide();
                buttonReplReport.Hide();
                buttonGL_View.Hide();
               // buttonAudiReports.Hide();
            }

            toolTipHeadings.ShowAlways = true;

            //Define Banks 
            toolTipButtons.SetToolTip(label2, "Infrastructure" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;
                //Define Banks 
                //toolTipButtons.SetToolTip(button13, "Definition of Banks based on SWIFT number." + Environment.NewLine
                //                             + "Also define a Group of Banks."
                //                             );

                //ATMs Maintenance  
                toolTipButtons.SetToolTip(button53, "Display all ATMs." + Environment.NewLine
                                             + "Add, Update, Delete and View details." + Environment.NewLine
                                             + "Use the Add Like functionality for quick creation of a new ATM." + Environment.NewLine
                                             + "Use of Google Maps to define the ATM location."
                                             );

                //Access Rights  
                //toolTipButtons.SetToolTip(buttonAccessRights, "Functions and Roles are Displayed." + Environment.NewLine
                //                             + "The Administrator alocates rights per role." + Environment.NewLine
                //                             + "By pressing update these are saved and applied in system" + Environment.NewLine
                //                             + "Roles are assigned to users."
                //                             );

                //Migration Cycles 
                //toolTipButtons.SetToolTip(button9, "ATMs migration takes place in cycles." + Environment.NewLine
                //                             + "Cycles with the associated information are listed." + Environment.NewLine
                //                             + "Printing facility is provided for the migration records"
                //                             );
                ////Migration 
                //toolTipButtons.SetToolTip(button30, "Data are loaded from excel" + Environment.NewLine
                //                             + "Upon uploading the new ATMs are created." + Environment.NewLine
                //                             + "Report is prepared to show the migration activity."
                //                             );

                ////Holidays and special days definition.   
                //toolTipButtons.SetToolTip(button28, "Define Holidays and Special Days." + Environment.NewLine
                //                             + "For simplicity and efficiency next year's holidays are based on the previous year."
                //                             );

                //Users and CIT providers.   
                toolTipButtons.SetToolTip(button54, "Define Users and Cash In Transit Providers (CIT)." + Environment.NewLine
                                             + "Define access role levels." + Environment.NewLine
                                             + "Set access rights at functionality and ATM level."
                                             );

                //Accounts Management    
                //toolTipButtons.SetToolTip(button18, "Define General Ledger accounts per ATM." + Environment.NewLine
                //                             + "Based on these Posted Transactions are created."
                //                             );

                //ATM Groups    
                toolTipButtons.SetToolTip(button14, "Allocate ATMs into groups." + Environment.NewLine
                                             + "These can be assigned to CIT providers."
                                             );

                //Matched Days    
                //toolTipButtons.SetToolTip(button29, "For Cash In calculation previous turnover of similar(matched) days is used." + Environment.NewLine
                //                             + "ATM Controller once a month defines the matched days of next period by ATMs category."
                //                             );

                //Parameters Setting    
                //toolTipButtons.SetToolTip(button21, "Define System Parameters."
                //                             );

                //Matched Days exceptions   
                //toolTipButtons.SetToolTip(button55, "Define Matched days for ATMs exceptions." + Environment.NewLine
                //                             + " % Decrease or increase of matched days turnover is updated."
                //                             );


                //Software Distribution categories  
                //toolTipButtons.SetToolTip(button23, "Categories are defined per supplier and model" + Environment.NewLine
                //                             + " that have different SW characteristics."
                //                             );

                //Software Distribution Package Definition
                //toolTipButtons.SetToolTip(button15, "Package are defined." + Environment.NewLine
                //                             + " Files and supporting documentation."
                //                             );

                //Software Distribution Sessions
                //toolTipButtons.SetToolTip(button16, "Distribution Sessions per package are defined." + Environment.NewLine
                //                             + " Sessions are defined for Pre-production, Pilot and production"
                //                             );

                //Software Distribution View History
                //toolTipButtons.SetToolTip(button12, "Past History is shown"
                //                             );

                //Software Distribution DashBoard
                //toolTipButtons.SetToolTip(button17, " At any time the status is shown for all work done." + Environment.NewLine
                //                             + " Atms with version difference is shown"
                //                             );




                ////Files and Fields Definition    
                //toolTipButtons.SetToolTip(button88, "Define new Files and Fileds." + Environment.NewLine
                //                             + "The definition is ised for loading of files." + Environment.NewLine
                //                             + "Consequently the loaded files are used for the matching process."
                //                             );
                ////Definition of RM Categories     
                //toolTipButtons.SetToolTip(button84, "Define Matching and Reconciliation categories (RM Categories)." + Environment.NewLine
                //                               + "RM Categories is the backbone of the matching and Reconciliation Process." + Environment.NewLine
                //                             + "You must follow an organised naming convention for ease of operation."
                //                             );
                ////MetaExceptions Rule Definition    
                //toolTipButtons.SetToolTip(button87, "Define MetaExceptions Rule definition." + Environment.NewLine
                //                             + "During matching exceptions (unmatched transactions are created." + Environment.NewLine
                //                             + "At the stage of reconciliation these are converted to MetaExceptions." + Environment.NewLine
                //                             + "The MetaExceptions have rules embetted in them." + Environment.NewLine
                //                             + "The system actions are based on these rules." 
                //                             );
                ////Definition of RM Categories Stages     
                //toolTipButtons.SetToolTip(button83, "RM Categories Matching are based on stages from matching one file to the next " + Environment.NewLine
                //                             + "Here you dynamically define the stages." + Environment.NewLine
                //                             + "For each stage you define the files and the matching fields for each file Pair." + Environment.NewLine
                //                             + "The Next Form to this one defines the file characteristics for this category and also the matching Masks."
                //                             );

                //Files and Fields Definition    
                //toolTipButtons.SetToolTip(button39, "Define new Files and Fileds." + Environment.NewLine
                //                             + "The definition is used for loading of files." + Environment.NewLine
                //                             + "Consequently the loaded files are used for the matching process."
                //                             );
                //Definition of RM Categories     
                //toolTipButtons.SetToolTip(button84, "Define Matching and Reconciliation categories (RM Categories)." + Environment.NewLine
                //                               + "RM Categories is the backbone of the matching and Reconciliation Process." + Environment.NewLine
                //                             + "You must follow an organised naming convention for ease of operation."
                //                             );
                //MetaExceptions Rule Definition    
                //toolTipButtons.SetToolTip(button87, "Define MetaExceptions Rule definition." + Environment.NewLine
                //                             + "During matching exceptions (unmatched transactions are created." + Environment.NewLine
                //                             + "At the stage of reconciliation these are converted to MetaExceptions." + Environment.NewLine
                //                             + "The MetaExceptions have rules embetted in them." + Environment.NewLine
                //                             + "The system actions are based on these rules."
                //                             );
                //Definition of RM Categories Stages     
                //toolTipButtons.SetToolTip(button83, "RM Categories Matching are based on stages from matching one file to the next " + Environment.NewLine
                //                             + "Here you dynamically define the stages." + Environment.NewLine
                //                             + "For each stage you define the files and the matching fields for each file Pair." + Environment.NewLine
                //                             + "The Next Form to this one defines the file characteristics for this category and also the matching Masks."
                //                             );
                //Definition of Categories Vs Bins      
                //toolTipButtons.SetToolTip(button24, "Here you define the Bins that are assigned on this category " + Environment.NewLine
                //                             + "Eg If the target system is the Banking system then you define" + Environment.NewLine
                //                             + "the ones that go to Banking system ." + Environment.NewLine
                //                             + "Like debit cards say."
                //                             );

                // Event Loading Schedules   
                //toolTipButtons.SetToolTip(button89, "Events of loading or other are defined to run periodically" + Environment.NewLine
                //                             + "Per minutes, per day per Week, per month" + Environment.NewLine
                //                             + "ATMs and Matching categories are connected with these event schedules" + Environment.NewLine
                //                             + "Action is taken when the event schedule is activated "
                //                             );
                //Journal Loading Status    
                //toolTipButtons.SetToolTip(button96, "For each ATM the loading Status is displayed " + Environment.NewLine
                //                             + "A second table displays all loading historical activity." + Environment.NewLine
                //                             );

                //NEW JOB CYCLES 
                toolTipButtons.SetToolTip(button49, "Definition Of Reconciliation Categories" + Environment.NewLine
                                             + "Based on these reconciliation is made. " + Environment.NewLine
                                             + "ATMs are divided in groups. " + Environment.NewLine
                                             + "Each group has a Reconciliation Category. " + Environment.NewLine
                                             + "Each Reconc Category may have more than one Matching Categories. "
                                             );

                ////NEW JOB CYCLES 
                //toolTipButtons.SetToolTip(button50, "Every Morning Manager does :" + Environment.NewLine
                //                             + "Close the current Job Cycle and opens a new one. " + Environment.NewLine
                //                             + "From now on the reconciliation can start. "
                //                             );

                //Journal Loading Service   
                //toolTipButtons.SetToolTip(button90, "This Service Runs Periodically as defined in parameters " + Environment.NewLine
                //                             + "Checks what for what ATMs E-Journal to be loaded" + Environment.NewLine
                //                             + "Gives orders to the loading service" + Environment.NewLine
                //                             + "Loading Service loads and parse E-Journal"
                //                             );

                //Quality Parameters setting   
                //toolTipButtons.SetToolTip(button36, "Set Quality key indicators." + Environment.NewLine
                //                             + "Depature from these indicators is reported by the system."
                //                             );

                //ATMs migration process 
                //toolTipButtons.SetToolTip(button30, "From excel or file ATMs availble details move to temporary area." + Environment.NewLine
                //                             + "Information is viewed and upadted using ATMs maintenance functionality." + Environment.NewLine
                //                             + "Upon finalisation we have an active ATM."
                //                             );

                // System Performance analysis         
                //toolTipButtons.SetToolTip(button74, "Review critical process performance." + Environment.NewLine
                //                             + "An analysis is displayed and alerts are sent when beyond predefined limits."
                //                             );

                // System ERRORS       
                toolTipButtons.SetToolTip(button95, "During System operation a system error might occur." + Environment.NewLine
                                             + "In such remote possibility information is provided to assist IT programmers." + Environment.NewLine
                                             + "Information can beviewed or send by email" + Environment.NewLine
                                             );

                //System Audit Trail     
                //toolTipButtons.SetToolTip(button19, "View activity of system maintenance." + Environment.NewLine
                //                             + "Use search to view past changes on system."
                //                             );

                //System health Check     
                //toolTipButtons.SetToolTip(button31, "The Controller must perform some operation on system to keep it healthy." + Environment.NewLine
                //                             + "The delayed or not done work is displayed."
                //                             );

                //Authorization Management        
                //toolTipButtons.SetToolTip(button75, "The Pending Authorizations are manage here." + Environment.NewLine
                //                             + "You can go and Authorise , or Update action after authorization is made." + Environment.NewLine
                //                             + "Also if authorizer is not availble you can undo(Delete) authorization request."
                //                             );

                //MIS - Refresed testing data         
                //toolTipButtons.SetToolTip(button22, "Data are refreshed and Demostration of system can start from the beggining." + Environment.NewLine
                //                             + "AB102 is ready for replenishement, reconciliation and trans posting." + Environment.NewLine
                //                             + "While AB104, with production rich journal, has already been reconciled." + Environment.NewLine
                //                             + "It is ready to show capture cards, transactions, eJournal Drilling down." + Environment.NewLine
                //                             + "Also the dispute total cycle reaching the posting of transactions can be demostrated."
                //                             );

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
        //
        // GO TO ATMS NEW AND MAINTENance 
        // 
        private void button53_Click(object sender, EventArgs e)
        {
            //TEST
            if (WSecLevel == "10" // Administrator
                || WSecLevel == "08" // Controller
                || WSignedId == "1005"
                || WSignedId == "ADMIN-APEX"
                || WSignedId == "SERVE31" || WSignedId == "500")
            {
                NForm108 = new Form108(WSignedId, WSignRecordNo, WOperator);
                NForm108.ShowDialog(); ;
            }
            else
            {
                //   NMessageBoxCustom = new MessageBoxCustom(" YOU CANNOT ACCESS ATMS MAINTENANCE");
                //   NMessageBoxCustom.ShowDialog(); ; 
                MessageBox.Show(" YOU CANNOT ACCESS ATMS MAINTENANCE");
                return;
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
            NForm13.ShowDialog(); ;
        }



        void NForm152_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }



        // CIRCULARS AND VIDEO FOR TRAININGS 
        private void button56_Click(object sender, EventArgs e)
        {
            NForm300 = new Form300();
            NForm300.ShowDialog(); ;
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
            NForm45.ShowDialog(); ;
            //   this.Hide(); 
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
          // Todays Controller 

        private void button52_Click(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }



        //
        // NEW FORM ... NEW FORM 
        // 
        // EXCEPTIONS Figuers for replenishemnt 
        private void button55_Click(object sender, EventArgs e)
        {
            WAction = 1;
            NForm10 = new Form10(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm10.ShowDialog();
        }

        // General Audit trail 
        // This will be based on screen image captured 

        private void button19_Click(object sender, EventArgs e)
        {
            Form76 NForm76 = new Form76(WSignedId, WSignRecordNo, WOperator);
            NForm76.ShowDialog(); ;
            //MessageBox.Show("General Audit trail this will be based on screen image captured + record registration id");  
        }

        // MATCHED DATES
        private void button29_Click(object sender, EventArgs e)
        {
            WAction = 1; // Per Month
            NForm12 = new Form12(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm12.ShowDialog();
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

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //CheckForMessages(); // Check for messages from controller

           // return; 

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

            if (Ap.IsServerConnected() == true)
            {
                // Everything is good ... continue 
            }
            else
            {
                MessageBox.Show("SQL not available. Please try later"); 
                return;  // SQL is not available 
            }

            //Ap.CheckOutstandingAuthorizationsStage1_with_Update(WSignedId); 

            //Ap.CheckOutstandingAuthorizationsStage1(WSignedId); // There is record for authorizer with Stage 1 
            //if (Ap.RecordFound == true)
            //{
            //    Console.Beep(2000, 500);
            //    Console.Beep(3000, 1000);
            //    Console.Beep(2000, 500);
            //    MessageBox.Show(new Form { TopMost = true }, "You have requests for authorization. From : " + Ap.Requestor
            //        );
            //}

            //Ap.CheckOutstandingAuthorizationsStage3(WSignedId); // Threre is record for Requestor with stage 3 
            //if (Ap.RecordFound == true)
            //{
            //    Console.Beep(2000, 500);
            //    Console.Beep(3000, 1000);
            //    Console.Beep(2000, 500);
            //    MessageBox.Show(new Form { TopMost = true }, "Authorization has been made. by : " + Ap.Authoriser);
            //}

            // Read The latest info from Cycle 

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;
            bool WJournalLoadingStarted = Rjc.SpareBool_1;
            int WQueueId = Rjc.SpareInt_1;

            RRDMAgentQueue Aq = new RRDMAgentQueue();



            if (WJournalLoadingStarted == true & WQueueId > 0)
            {

                string WSelectionCriteria = " WHERE OriginalReqId =" + WQueueId
                                  + " AND OriginalRequestorID ='" + WSignedId + "'";
                Aq.ReadAgentQueueBySelectionCriteria(WSelectionCriteria);
                if (Aq.RecordFound == true & Aq.MessageSent == false)
                {
                    //
                    // Update as messagesent = true
                    //
                    // Save date 
                    int SavedSeqNo = Aq.ReqID;
                    DateTime SavedDateFinish = Aq.CmdExecStarted; // From Record Just read


                    // Get Date from 
                    WSelectionCriteria = " Where ReqID =" + WQueueId; // This is the requested Id
                    Aq.ReadAgentQueueBySelectionCriteria(WSelectionCriteria);

                    DateTime SavedDateStart = Aq.CmdExecStarted;

                    if (SavedDateFinish >= SavedDateStart)
                    {
                        // Update Record
                        Aq.UpdateRecordInAgentQueueForMessageSent(SavedSeqNo);

                        //*******************************
                        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();
                        Flog.ReadLoadedFilesByCycleNumber_All(WReconcCycleNo);

                        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
                        int Mode = 5; // Updating Action 
                        string ProcessName = "LoadingOfJournals";
                        string Message = "Loading Of.."+Flog.Journal_Total.ToString()+"..Journals Finishes - Request Id " 
                                       + Aq.ReqID.ToString();

                        Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedDateStart, SavedDateFinish, Message, WSignedId, WReconcCycleNo);
                        //*******************************

                        Console.Beep(2000, 500);
                        Console.Beep(3000, 1000);
                        Console.Beep(2000, 500);

                        MessageBox.Show(new Form { TopMost = true }, "Journals Loading Has Finished!");

                        Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

                        Rjc.SpareBool_1 = false; // Journal Loading strts
                        Rjc.SpareInt_1 = 0; // Requested service

                        Rjc.UpdateSpecialFields(WReconcCycleNo);

                        // STOP 

                        // ***************************
                        //
                        // CALL STORE PROCEDURE TO STOP_CMD_Shell 
                        // THIS WAS STARTED at The START of  LOADING OF JOURNAL
                        string connectionString_AUDI = ConfigurationManager.ConnectionStrings
                             ["JournalsConnectionString_AUDI"].ConnectionString;

                        string RCT = "ATM_MT_Journals_AUDI.[dbo].[stp_Stop_CMD_Shell]";

                        using (SqlConnection conn =
                           new SqlConnection(connectionString_AUDI))
                            try
                            {
                                conn.Open();
                                using (SqlCommand cmd =
                                   new SqlCommand(RCT, conn))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    // Parameters
                                    cmd.CommandTimeout = 1800;  // seconds
                                    int rows = cmd.ExecuteNonQuery();

                                }
                                // Close conn
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                            }

                        //*******************************

                    }


                }
                {

                }
            }
            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            //Ap.ReadAuthorizationsUserTotal(WSignedId);
            //if (Ap.RecordFound == true)
            //{
            //    //checkBoxAuthRecords.Text = Ap.TotalNumberforUser.ToString();
            //    checkBoxAuthRecords.Show();
            //}
            //else checkBoxAuthRecords.Hide();

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
     
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
           // button13.BackColor = Red;
            button53.BackColor = Red;
            button54.BackColor = Red;
            // button21.BackColor = Red;

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
            //button36.BackColor = Silver;
            //button36.ForeColor = Black;

            //button75.BackColor = Silver;
            //button75.ForeColor = Black;

            //button74.BackColor = Silver;
            //button74.ForeColor = Black;
            //button31.BackColor = Silver;
            //button31.ForeColor = Black;
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
        // Assist - show Panel4 
        //private void checkBox2_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBoxAssist.Checked == true)
        //    {
        //        panel4.Show();
        //        textBox1.Show();
        //    }
        //    else
        //    {
        //        panel4.Hide();
        //        textBox1.Hide();
        //    }
        //}

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
            int Mode = 1;
            NForm503 = new Form503(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            NForm503.ShowDialog();
        }

        // Define ERROR IDs and Rules 
        private void button87_Click(object sender, EventArgs e)
        {
            if (Environment.UserName != "Panicos Michael")
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO CIT-System under investigation ");
                return;
            }
            Form66 NForm66;
            int WFunction = 1;
            NForm66 = new Form66(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm66.ShowDialog();
        }
        // Mapping of fields for Bank File Vs RRDM matching files 
        private void button88_Click(object sender, EventArgs e)
        {


        }
       
        // Reconciliation Categories 
        private void button49_Click(object sender, EventArgs e)
        {
            Form504 NForm504;
            int Mode = 1;
          //  NForm504 = new Form504(WSignedId, WSignRecordNo, WOperator, W_Application, Mode);
          //  NForm504.ShowDialog();
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

      
        // Check ready to load and Match
        private void button42_Click(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);

            int Mode = 1; // From the administrator

            if (Ba.ShortName == "ALP")
            {

                Form502_Load_And_Match_ALP NForm502_Check_Loading_ALP;

                NForm502_Check_Loading_ALP = new Form502_Load_And_Match_ALP(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Check_Loading_ALP.ShowDialog();
            }
        }
        // GET ATM INFO THROUGH IST
        private void button43_Click(object sender, EventArgs e)
        {

            if (HasInternet())
            {
                //   MessageBox.Show("There is Internet Connection");

            }
            else
            {
                MessageBox.Show("There is NO Internet Connection" + Environment.NewLine
                    + "You Cannot proceed"
                    );
                return;
            }

            Form78d_IST NForm78d_IST;

            NForm78d_IST = new Form78d_IST(WOperator, WSignedId);
            NForm78d_IST.ShowDialog();
        }
       
        //  Operational Processes 
        private void button45_Click(object sender, EventArgs e)
        {
            Form8_Traces_Oper NForm8_Traces_Oper;
            NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, WSecLevel, WOperator,1);
            NForm8_Traces_Oper.ShowDialog();
        }
        // DELETE Dublicates From Journal 
        private void button46_Click(object sender, EventArgs e)
        {
            RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
            int Count1 = 0;
            int Count2 = 0;
            string SourceFileId = "Atms_Journals_Txns";

            Rs.ReadReconcSourceFilesByFileId(SourceFileId);

            string InSourceDirectory = Rs.SourceDirectory;

            string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

            if (allJournals.Length == 0)
            {
                MessageBox.Show(" There are no files for loading");
                //  textBox1.Text = "0";
                return;
            }
            else
            {
                // DELETE jln
                foreach (string file in allJournals)
                {
                    string myFilePath = @file;
                    string ext = Path.GetExtension(myFilePath);

                    if (ext == ".jln")
                    {
                        File.Delete(file);
                        Count1 = Count1 + 1;
                    }
                }
                MessageBox.Show("Deleted Journals .jln..=.." + Count1.ToString());
                // After this check you 
                // Check if Dublicate and delete 
                allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                if (allJournals.Length == 0)
                {
                    MessageBox.Show(" There are no files for loading");
                    //textBox1.Text = "0";
                    return;
                }
                foreach (string file in allJournals)
                {
                    if (Rs.CheckIfFileIsDublicate(file) == true)
                    {
                        // Delete File 
                        File.Delete(file);
                        Count2 = Count2 + 1;
                    }

                }
            }

            MessageBox.Show("Deleted Journals already loaded.=.." + Count2.ToString());
        }
       
        //
        // for viewing 
        //
        private void button48_Click(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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
            // 

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "851";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string UnderDevelopment = Gp.OccuranceNm;

            ParId = "852";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string IsProductionEnv = Gp.OccuranceNm;

            if (IsProductionEnv == "YES" & UnderDevelopment == "YES")
            {
                MessageBox.Show(" UNDER CONTRUCTION... ");
                return;
            }


            int Mode = 2; // FOR VIEWING 
            Form502_Load_And_Match_BDC NForm502_Check_Loading;

            NForm502_Check_Loading = new Form502_Load_And_Match_BDC(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
            NForm502_Check_Loading.ShowDialog();

        }
        // VIEW FILES 
        private void button50_Click_1(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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


            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "851";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string UnderDevelopment = Gp.OccuranceNm;

            ParId = "852";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string IsProductionEnv = Gp.OccuranceNm;

            if (IsProductionEnv == "YES" & UnderDevelopment == "YES")
            {
                MessageBox.Show(" UNDER CONTRUCTION... ");
                return;
            }

            int Mode = 2; // FOR VIEWING 
            Form502_Files_Loaded NForm502_Files_Loaded;
            // string InSignedId, int SignRecordNo, string InOperator
            NForm502_Files_Loaded = new Form502_Files_Loaded(WSignedId, WSignRecordNo, WOperator, "ATMs");
            NForm502_Files_Loaded.ShowDialog();



        }
       
        //
        // Job Cycles 
        //
        private void button85_Click(object sender, EventArgs e)
        {
            Form200JobCycles NForm200JobCycles;

            string RunningGroup = "ATMs";
            NForm200JobCycles = new Form200JobCycles(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles.FormClosed += NForm200JobCycles_FormClosed; 
            NForm200JobCycles.ShowDialog();
        }

        private void NForm200JobCycles_FormClosed(object sender, FormClosedEventArgs e)
        {
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();

           // FormMainScreen_Load(this, new EventArgs());
        }

        //
        // Work allocation 
        //
        private void button86_Click(object sender, EventArgs e)
        {

            Form80cATMs NForm80cATMs;
            string WFunction = "Allocate";
            NForm80cATMs = new Form80cATMs(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm80cATMs.ShowDialog();
        }

        private void checkBoxAssist_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAssist.Checked == true)
            {
                panel7.Show();
                panel7.BringToFront();
                // textBoxVersionDate.Show();
            }
            else
            {
                panel7.Hide();
                panel7.SendToBack();
                // textBoxVersionDate.Hide();
            }
        }
        // refresh
        private void button22_Click_1(object sender, EventArgs e)
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
                        cmd.CommandTimeout = 1800;  // seconds
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


        }
        // sofware version 
        private void buttonSWVersion_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

     
// Category view
        private void button24_Click(object sender, EventArgs e)
        {
           
            Form80a3 NForm80a3;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WOperator;

            NForm80a3 = new Form80a3(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            //NForm80a3.FormClosed += NForm80a1_FormClosed;

            NForm80a3.ShowDialog();
        }

// Dispute preinvestigation 
        private void button67_Click(object sender, EventArgs e)
        {
            Form3_PreInv NForm3_PreInv;

            NForm3_PreInv = new Form3_PreInv(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            NForm3_PreInv.ShowDialog();

        }

        private void buttonIST_View_Click(object sender, EventArgs e)
        {
            Form3_PreInv_IST NForm3_PreInv_IST;

            NForm3_PreInv_IST = new Form3_PreInv_IST(WSignedId, WSignRecordNo, WSecLevel, WOperator, WReconcCycleNo);

            NForm3_PreInv_IST.ShowDialog();
        }
// CIT Shortages
        private void buttonCitShortages_Click(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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
// Create SM for lost Journal 
        private void buttonCreateSM_Click(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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
            NForm154 = new Form154(WOperator, WSignedId,WReconcCycleNo , WMode,"");
            NForm154.ShowDialog();
        }
// Allow sign In 
        private void buttonAllowSignIn_Click(object sender, EventArgs e)
        {
            // SET IT
            // UPDATE
            //
            string ParId = "105";
            string OccurId = "1";
            RRDMGasParameters Gp = new RRDMGasParameters();

            string IsYESNO = "YES";

            // Update Occurance 
            Gp.UpdateGasParamByParamIdAndOccur(WOperator, ParId, OccurId, IsYESNO);

            MessageBox.Show("Users can now sign in RRDM"); 
        }
// Not sign in 
        private void buttonNoSignIn_Click(object sender, EventArgs e)
        {
            // SET IT
            // UPDATE
            //
            string ParId = "105";
            string OccurId = "1";
            RRDMGasParameters Gp = new RRDMGasParameters();

            string IsYESNO = "NO";

            // Update Occurance 
            Gp.UpdateGasParamByParamIdAndOccur(WOperator, ParId, OccurId, IsYESNO);

            MessageBox.Show("Users can not sign In"+Environment.NewLine
                + "Only the controller can sign in "
                );
        }
// ATMs and Cycles 
        private void buttonATMsAndCycles_Click(object sender, EventArgs e)
        {
            Form108_Cycles NForm108_Cycles;
            NForm108_Cycles = new Form108_Cycles(WSignedId, WSignRecordNo, WOperator);
            NForm108_Cycles.ShowDialog(); ;
        }

// Excel Loading 
        private void buttonExcelLoading_Click(object sender, EventArgs e)
        {
            Form18_ExcelMainMenu NForm18_ExcelMainMenu;
            WAction = 2;
            
            NForm18_ExcelMainMenu = new Form18_ExcelMainMenu(WSignedId, WSignRecordNo, WSecLevel, WOperator,WCut_Off_Date ,WAction);
            NForm18_ExcelMainMenu.FormClosed += NForm112_FormClosed;
            NForm18_ExcelMainMenu.ShowDialog();
        }
// CIT MANAGEMENT 
        private void buttonCIT_Mgmt_Click(object sender, EventArgs e)
        {
            Form18 NForm18;
            WAction = 1;
            NForm18 = new Form18(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm18.ShowDialog(); ;
        }

        private void buttonReplReport_Click(object sender, EventArgs e)
        {
            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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
            //RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();

            //Gl.UpdateCalculatedGL_For_All_ATMs(WReconcCycleNo, Rjc.Cut_Off_Date); 
            RRDMAccountsClass Acc = new RRDMAccountsClass(); 
            Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, Rjc.Cut_Off_Date); 

            Form503_SesCombined NForm503_SesCombined;
            int Mode = 1;
            string TemoAtmNo = "";
            int TempReplCycle = 0; 
            NForm503_SesCombined = new Form503_SesCombined(WSignedId, WSignRecordNo, WOperator, WOrigin, WReconcCycleNo, TemoAtmNo, TempReplCycle, Mode);
            NForm503_SesCombined.ShowDialog();

            return; 


            WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries(); 
            // Make Selection Of Validated Entries 
            int TempMode = 2; // Bank entries 
            string SelectionCriteria = " WHERE RMCycle = " + WReconcCycleNo;

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
                int WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];
                string WAtmNo = (string)G4.DataTableG4SEntries.Rows[I]["AtmNo"];
                int WSesNo = (int)G4.DataTableG4SEntries.Rows[I]["ProcessedAtReplCycleNo"];
                decimal WUnloadedMachine = (decimal)G4.DataTableG4SEntries.Rows[I]["UnloadedMachine"];
                decimal WCash_Loaded_Machine = (decimal)G4.DataTableG4SEntries.Rows[I]["Cash_Loaded"];
                decimal WUnloadedMachineDep = (decimal)G4.DataTableG4SEntries.Rows[I]["Deposits"];
                string WMask = (string)G4.DataTableG4SEntries.Rows[I]["Mask"];

                DateTime ReplDate = (DateTime)G4.DataTableG4SEntries.Rows[I]["ReplDateG4S"];

               


                I++; // Read Next entry of the table 

            }

            string P1 = "Replenishment Report for Audi";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_G4S ReportATMSReplCycles = new Form56R69ATMS_G4S(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }
// GL View 
        private void buttonGL_View_Click(object sender, EventArgs e)
        {
            //RRDM_EXCEL_AND_Directories Xl = new RRDM_EXCEL_AND_Directories();

            //    Xl.ConvertToTapDelimiterFile();
           

            //return; 
            string WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

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

            // 
            // UPDATE GL ENTRIES
            //
            MessageBox.Show("At this point we will calculate ATMs GL balances " + Environment.NewLine
                + "And compare them with the Banks Books GL balances "
                );

            DateTime Test_Cut_Off_Date = new DateTime(2021, 09, 05);
            RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();
            Gl.UpdateCalculatedGL_For_All_ATMs(WReconcCycleNo, Test_Cut_Off_Date);


            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            // FOR AUDI TYPE WE LOAD GL AND WE ALSO USE OTHER FORM For Replenishment 
            // 
            RRDMGasParameters Gp = new RRDMGasParameters();
            bool AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
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

            RRDMAccountsClass Acc = new RRDMAccountsClass(); 
            Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, Rjc.Cut_Off_Date);

            if (AudiType == true)
            {
                // GL FILE WAS LOADED 
                Form503_GL_STATUS NForm503_GL_STATUS;
                int Mode = 1;
                NForm503_GL_STATUS = new Form503_GL_STATUS(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Rjc.Cut_Off_Date, Mode);
                NForm503_GL_STATUS.ShowDialog();
            }

            return;
        }
// View AUDI Reports 
        private void buttonAudiReports_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Reports_AUDI NForm200JobCycles_Reports_AUDI;

            string RunningGroup = "ATMs";
            NForm200JobCycles_Reports_AUDI = new Form200JobCycles_Reports_AUDI(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm200JobCycles_Reports_AUDI.ShowDialog();
        }
// Parameters 
        private void button21_Click(object sender, EventArgs e)
        {
            Form191 NForm191; 
            NForm191 = new Form191(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm191.ShowDialog();
        }
// PENDING AUTHORISATIONS
        private void button75_Click(object sender, EventArgs e)
        {
            string TempAtmNo = "";
            int TempSesNo = 0;
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            Form112 NForm112;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "Normal", TempAtmNo, TempSesNo, WDisputeNo, WDisputeTranNo, "", 0);
            NForm112.FormClosed += NForm112_FormClosed;
            NForm112.ShowDialog();
        }
        void NForm112_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }
// BT
        private void buttonBT_ROM_Click(object sender, EventArgs e)
        {
            string WAtmNo = "EJ017002";
            int WSeqNo = 676; 
            //Form51_TXN_Cassettes_BAL Cb = new Form51_TXN_Cassettes_BAL();
            Form51_TXN_Cassettes_BAL NForm51_TXN_Cassettes_BAL;
            NForm51_TXN_Cassettes_BAL = new Form51_TXN_Cassettes_BAL(WOperator,WSignedId, WAtmNo, WSeqNo);
            NForm51_TXN_Cassettes_BAL.ShowDialog();
            //Form502_Load_And_Match_BOC NForm502_Check_Loading_BOC;
            return;
            WReconcCycleNo = 200;

            Form502_Load_And_Match_AUD NForm502_Check_Loading_AUD;

            NForm502_Check_Loading_AUD = new Form502_Load_And_Match_AUD(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, 1);
            NForm502_Check_Loading_AUD.ShowDialog();

            //NForm502_Check_Loading_BOC = new Form502_Load_And_Match_BOC(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, 1);
            //NForm502_Check_Loading_BOC.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Reports NForm200JobCycles_Reports;

            string RunningGroup = WOperator;
            NForm200JobCycles_Reports = new Form200JobCycles_Reports(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm200JobCycles_Reports.ShowDialog();
        }
// Dispute Pre Investigation 
        private void buttonDisputePreInv_Click(object sender, EventArgs e)
        {
            Form3_PreInv NForm3_PreInv;

            NForm3_PreInv = new Form3_PreInv(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            NForm3_PreInv.ShowDialog();
        }
// Cycles Vs Replenishements 
        private void buttonCycles_Vs_Replen_Click(object sender, EventArgs e)
        {
            Form80a3_Alpha NForm80a3_Alpha;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WOperator;

            NForm80a3_Alpha = new Form80a3_Alpha(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            //NForm80a3.FormClosed += NForm80a1_FormClosed;

            NForm80a3_Alpha.ShowDialog();
        }
    }
}

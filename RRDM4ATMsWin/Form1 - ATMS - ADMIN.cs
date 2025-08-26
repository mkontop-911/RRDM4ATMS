using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using System.IO;


//24-04-2015 Alecos
using System.Diagnostics;
using System.Text;

//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form_ATMS_ADMIN : Form
    {
        public EventHandler LoggingOut;

        Form108 NForm108;
        Form8 NForm8;
        Form12 NForm12;
        Form13 NForm13;
        Form15 NForm15;

        Form10 NForm10;

        Form143 NForm143;
        Form45 NForm45;

        Form152 NForm152;

        Form54 NForm54;
        Form55 NForm55;

        Form78 NForm78;
        Form81 NForm81;

        Form85 NForm85;

        Form191 NForm191;
        Form192 NForm192;

        Form300 NForm300;

        Form112 NForm112;

        string MsgFilter;

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        string WOrigin = "Our Atms";

        string W_Application;
        bool T24Version;

        //if (radioButtonATMS_CARDS.Checked == true) W_Application = "ATMS/CARDS";
        //  if (radioButton_e_MOBILE.Checked == true) W_Application = "e_MOBILE";

        // NOTES 
        string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WJobCategory = "ATMs";
        int WReconcCycleNo;

        bool WJournalLoadingStarted;
        int WQueueId;

        int WAction;
        string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        #region Form_ATMS_ADMIN
        public Form_ATMS_ADMIN(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();


            // this.WindowState = FormWindowState.Maximized;

            pictureBox1.BackgroundImage = appResImg.logo2;

            SWDistribution.Hide();
            AdminAuthorisation.Hide(); 

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();

            
            buttonDefineMakerChecker.Show(); 
           
                
            T24Version = false;


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
                    labelStep1.Text = "Configuration Menu _" + W_Application;
                }
                else
                {
                    W_Application = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                        T24Version = true;
                        labelStep1.Text = "Configuration Menu-Mobile_" + W_Application;
                    }
                    else
                    {
                        labelStep1.Text = "Configuration Menu-Mobile_" + W_Application;
                    }

                }
            }

            if (W_Application == "ATMs")
            {
                buttonCatForMobile.Hide();
                buttonMatchingCateg.Show();
            }
            else
            {
                buttonCatForMobile.Show();
                buttonMatchingCateg.Hide(); 
            }

            // Reset 

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

                // Migration 
                foreach (Control c in Migration.Controls)
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

                // E_Journals
                foreach (Control c in E_Journals.Controls)
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

                // SWDistribution
                foreach (Control c in SWDistribution.Controls)
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

                // Matching
                foreach (Control c in Matching.Controls)
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
                foreach (Control c in AdminAuthorisation.Controls)
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

            toolTipButtons.SetToolTip(label3, "OPERATION" + Environment.NewLine
                                         + "Through the below buttons you operate the system."
                                         );

            toolTipButtons.SetToolTip(labelParameters, "DISPUTES" + Environment.NewLine
                                        + "Register and manage disputes."
                                        );

            toolTipButtons.SetToolTip(label5, "QUALITY" + Environment.NewLine
                                       + "Audit Trail and Exception Reports for KPI."
                                       );
            toolTipButtons.SetToolTip(label11, "ELECTRONIC AUTHORISATIONS" + Environment.NewLine
                                       + "Management of Authorisations for " + Environment.NewLine
                                       + "Requestors and Authorisers ."
                                       );
        }
        #endregion
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

        private void FormMainScreen_Load(object sender, EventArgs e)
        {

            // NOTES 
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";

            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            //Ap.ReadAuthorizationsUserTotal(WSignedId);
            //if (Ap.RecordFound == true)
            //{
            //    //labelAuthRecords.Text = Ap.TotalNumberforUser.ToString();
            //    checkBoxAuthRecords.Show();
            //    checkBoxAuthRecords.Checked = true;
            //}
            //else checkBoxAuthRecords.Hide();

            if (Environment.MachineName != "DESKTOP-77PU6PG")
            {
                button34.Hide();
                button35.Hide();
                //button37.Hide();
            }


            ////*************************************************************************
            ////Check if outstanding transactions to be posted
            ////*************************************************************************
          //  RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

            //RRDMUpdateGrids Ug = new RRDMUpdateGrids();

            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//
          //  string AtmNo = "";
            string FromFunction = "";

            FromFunction = "General";

            //string WCitId = "1000";
            //// Create table with the ATMs this user can access
            //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            ////-----------------UPDATE LATEST TRANSACTIONS----------------------//
            //// Update latest transactions from Journal 
            //Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);



            toolTipHeadings.ShowAlways = true;

            //Define Banks 
            toolTipButtons.SetToolTip(label2, "Infrastructure" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;
                //Define Banks 
                toolTipButtons.SetToolTip(button13, "Definition of Banks based on SWIFT number." + Environment.NewLine
                                             + "Also define a Group of Banks."
                                             );

                //ATMs Maintenance  
                toolTipButtons.SetToolTip(button53, "Display all ATMs." + Environment.NewLine
                                             + "Add, Update, Delete and View details." + Environment.NewLine
                                             + "Use the Add Like functionality for quick creation of a new ATM." + Environment.NewLine
                                             + "Use of Google Maps to define the ATM location."
                                             );

                //Access Rights  
                toolTipButtons.SetToolTip(button20, "Functions and Roles are Displayed." + Environment.NewLine
                                             + "The Administrator alocates rights per role." + Environment.NewLine
                                             + "By pressing update these are saved and applied in system" + Environment.NewLine
                                             + "Roles are assigned to users."
                                             );

                //Migration Cycles 
                toolTipButtons.SetToolTip(button9, "ATMs migration takes place in cycles." + Environment.NewLine
                                             + "Cycles with the associated information are listed." + Environment.NewLine
                                             + "Printing facility is provided for the migration records"
                                             );
                //Migration 
                toolTipButtons.SetToolTip(button30, "Data are loaded from excel" + Environment.NewLine
                                             + "Upon uploading the new ATMs are created." + Environment.NewLine
                                             + "Report is prepared to show the migration activity."
                                             );

                //Holidays and special days definition.   
                toolTipButtons.SetToolTip(button28, "Define Holidays and Special Days." + Environment.NewLine
                                             + "For simplicity and efficiency next year's holidays are based on the previous year."
                                             );

                //Users and CIT providers.   
                toolTipButtons.SetToolTip(button54, "Define Users and Cash In Transit Providers (CIT)." + Environment.NewLine
                                             + "Define access role levels." + Environment.NewLine
                                             + "Set access rights at functionality and ATM level."
                                             );

                //Accounts Management    
                toolTipButtons.SetToolTip(button18, "Define General Ledger accounts per ATM." + Environment.NewLine
                                             + "Based on these Posted Transactions are created."
                                             );

                //ATM Groups    
                toolTipButtons.SetToolTip(button14, "Allocate ATMs into groups." + Environment.NewLine
                                             + "These can be assigned to CIT providers."
                                             );

                //Matched Days    
                toolTipButtons.SetToolTip(button29, "For Cash In calculation previous turnover of similar(matched) days is used." + Environment.NewLine
                                             + "ATM Controller once a month defines the matched days of next period by ATMs category."
                                             );

                //Parameters Setting    
                toolTipButtons.SetToolTip(button21, "Define System Parameters."
                                             );

                //Matched Days exceptions   
                toolTipButtons.SetToolTip(button55, "Define Matched days for ATMs exceptions." + Environment.NewLine
                                             + " % Decrease or increase of matched days turnover is updated."
                                             );


                //Software Distribution categories  
                toolTipButtons.SetToolTip(button23, "Categories are defined per supplier and model" + Environment.NewLine
                                             + " that have different SW characteristics."
                                             );

                //Software Distribution Package Definition
                toolTipButtons.SetToolTip(button15, "Package are defined." + Environment.NewLine
                                             + " Files and supporting documentation."
                                             );

                //Software Distribution Sessions
                toolTipButtons.SetToolTip(button16, "Distribution Sessions per package are defined." + Environment.NewLine
                                             + " Sessions are defined for Pre-production, Pilot and production"
                                             );

                //Software Distribution View History
                toolTipButtons.SetToolTip(button12, "Past History is shown"
                                             );

                //Software Distribution DashBoard
                toolTipButtons.SetToolTip(button17, " At any time the status is shown for all work done." + Environment.NewLine
                                             + " Atms with version difference is shown"
                                             );




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
                toolTipButtons.SetToolTip(button39, "Define new Files and Fileds." + Environment.NewLine
                                             + "The definition is used for loading of files." + Environment.NewLine
                                             + "Consequently the loaded files are used for the matching process."
                                             );
                //Definition of RM Categories     
                toolTipButtons.SetToolTip(buttonMatchingCateg, "Define Matching and Reconciliation categories (RM Categories)." + Environment.NewLine
                                               + "RM Categories is the backbone of the matching and Reconciliation Process." + Environment.NewLine
                                             + "You must follow an organised naming convention for ease of operation."
                                             );
                //MetaExceptions Rule Definition    
                toolTipButtons.SetToolTip(button87, "Define MetaExceptions Rule definition." + Environment.NewLine
                                             + "During matching exceptions (unmatched transactions are created." + Environment.NewLine
                                             + "At the stage of reconciliation these are converted to MetaExceptions." + Environment.NewLine
                                             + "The MetaExceptions have rules embetted in them." + Environment.NewLine
                                             + "The system actions are based on these rules."
                                             );
                //Definition of RM Categories Stages     
                toolTipButtons.SetToolTip(button83, "RM Categories Matching are based on stages from matching one file to the next " + Environment.NewLine
                                             + "Here you dynamically define the stages." + Environment.NewLine
                                             + "For each stage you define the files and the matching fields for each file Pair." + Environment.NewLine
                                             + "The Next Form to this one defines the file characteristics for this category and also the matching Masks."
                                             );
                //Definition of Categories Vs Bins      
                toolTipButtons.SetToolTip(button24, "Here you define the Bins that are assigned on this category " + Environment.NewLine
                                             + "Eg If the target system is the Banking system then you define" + Environment.NewLine
                                             + "the ones that go to Banking system ." + Environment.NewLine
                                             + "Like debit cards say."
                                             );

                // Event Loading Schedules   
                toolTipButtons.SetToolTip(button89, "Events of loading or other are defined to run periodically" + Environment.NewLine
                                             + "Per minutes, per day per Week, per month" + Environment.NewLine
                                             + "ATMs and Matching categories are connected with these event schedules" + Environment.NewLine
                                             + "Action is taken when the event schedule is activated "
                                             );
                //Journal Loading Status    
                toolTipButtons.SetToolTip(button96, "For each ATM the loading Status is displayed " + Environment.NewLine
                                             + "A second table displays all loading historical activity." + Environment.NewLine
                                             );

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
                toolTipButtons.SetToolTip(button90, "This Service Runs Periodically as defined in parameters " + Environment.NewLine
                                             + "Checks what for what ATMs E-Journal to be loaded" + Environment.NewLine
                                             + "Gives orders to the loading service" + Environment.NewLine
                                             + "Loading Service loads and parse E-Journal"
                                             );

                //Quality Parameters setting   
                toolTipButtons.SetToolTip(button36, "Set Quality key indicators." + Environment.NewLine
                                             + "Depature from these indicators is reported by the system."
                                             );

                //ATMs migration process 
                toolTipButtons.SetToolTip(button30, "From excel or file ATMs availble details move to temporary area." + Environment.NewLine
                                             + "Information is viewed and upadted using ATMs maintenance functionality." + Environment.NewLine
                                             + "Upon finalisation we have an active ATM."
                                             );

                // System Performance analysis         
                toolTipButtons.SetToolTip(button74, "Review critical process performance." + Environment.NewLine
                                             + "An analysis is displayed and alerts are sent when beyond predefined limits."
                                             );

                // System ERRORS       
                toolTipButtons.SetToolTip(button95, "During System operation a system error might occur." + Environment.NewLine
                                             + "In such remote possibility information is provided to assist IT programmers." + Environment.NewLine
                                             + "Information can beviewed or send by email" + Environment.NewLine
                                             );

                //System Audit Trail     
                toolTipButtons.SetToolTip(button19, "View activity of system maintenance." + Environment.NewLine
                                             + "Use search to view past changes on system."
                                             );

                //System health Check     
                toolTipButtons.SetToolTip(button31, "The Controller must perform some operation on system to keep it healthy." + Environment.NewLine
                                             + "The delayed or not done work is displayed."
                                             );

                //Authorization Management        
                toolTipButtons.SetToolTip(button75, "The Pending Authorizations are manage here." + Environment.NewLine
                                             + "You can go and Authorise , or Update action after authorization is made." + Environment.NewLine
                                             + "Also if authorizer is not availble you can undo(Delete) authorization request."
                                             );

                //MIS - Refresed testing data         
                toolTipButtons.SetToolTip(button22, "Data are refreshed and Demostration of system can start from the beggining." + Environment.NewLine
                                             + "AB102 is ready for replenishement, reconciliation and trans posting." + Environment.NewLine
                                             + "While AB104, with production rich journal, has already been reconciled." + Environment.NewLine
                                             + "It is ready to show capture cards, transactions, eJournal Drilling down." + Environment.NewLine
                                             + "Also the dispute total cycle reaching the posting of transactions can be demostrated."
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
        //
        // GO TO ATMS NEW AND MAINTENance 
        // 
        private void button53_Click(object sender, EventArgs e)
        {
            //TEST
           
                NForm108 = new Form108(WSignedId, WSignRecordNo, WOperator);
                NForm108.ShowDialog(); ;

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



        // CIRCULARS AND VIDEO FOR TRAININGS 
        private void button56_Click(object sender, EventArgs e)
        {
            NForm300 = new Form300();
            NForm300.ShowDialog(); ;
        }




        // AUthorize Dispute Decisions 

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
            NForm45.ShowDialog(); ;
            //   this.Hide(); 
        }



        // Refresh Testing data 
        //
        //bool FromRefreshTestingData ;

        private void button22_Click(object sender, EventArgs e)
        {
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
            FormMainScreen_Load(this, new EventArgs());

            MessageBox.Show("Testing Data have been initialised");

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
            Form76_NEW NForm76_NEW = new Form76_NEW(WSignedId, WOperator, "");
            NForm76_NEW.ShowDialog(); ;
            //MessageBox.Show("General Audit trail this will be based on screen image captured + record registration id");  
        }

        // MATCHED DATES
        private void button29_Click(object sender, EventArgs e)
        {
            WAction = 1; // Per Month
            NForm12 = new Form12(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm12.ShowDialog();
        }
        //HOLIDDAYS AND SPECIAL DAYS
        private void button28_Click(object sender, EventArgs e)
        {
            /*
            ClassHolidays Ch = new ClassHolidays();

            string BankA = "ModelBak";
            string BankB = "ServeUk";

            Ch.CopyHolidays(BankA, BankB, DateTime.Now.Year);
             */

            WAction = 1;
            NForm15 = new Form15(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm15.ShowDialog(); 
        }
        // ATMs Migration 
        private void button30_Click(object sender, EventArgs e)
        {
            RRDMMigrationCycles Mc = new RRDMMigrationCycles();

            Mc.ReadLastReconcMigrationCycle(WOperator);

            if (Mc.RecordFound == false || (Mc.RecordFound == true & Mc.ProcessStage == 2))
            {
                MessageBox.Show("Please create a Migration Cycle");
                return;
            }
            else
            {
                // Found and Valid
                Form108MigrationExcel NForm108Excel;
                NForm108Excel = new Form108MigrationExcel(WSignedId, WSignRecordNo, WOperator, Mc.SeqNo);
                NForm108Excel.ShowDialog(); ;
            }

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
            CheckForMessages(); // Check for messages from controller

            //RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
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
                        string Message = "Loading Of.." + Flog.Journal_Total.ToString() + "..Journals Finishes - Request Id "
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

            // In production we set the parameter YES 
            NForm85 = new Form85(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85.ShowDialog();

        }
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //
        // System Performance
        //
        private void button74_Click(object sender, EventArgs e)
        {
            WAction = 1;

            NForm8 = new Form8(WSignedId, WOperator,"" ,WAction);
            NForm8.ShowDialog(); ;

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

        // NOTES
        private void buttonNotes_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, "Update", WSearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            // NOTES 
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";
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
            button13.BackColor = Red;
            button53.BackColor = Red;
            button54.BackColor = Red;
            button21.BackColor = Red;

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
            button36.BackColor = Silver;
            button36.ForeColor = Black;

            button75.BackColor = Silver;
            button75.ForeColor = Black;

            button74.BackColor = Silver;
            button74.ForeColor = Black;
            button31.BackColor = Silver;
            button31.ForeColor = Black;
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
        // Assist - show Panel4 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAssist.Checked == true)
            {
                panel4.Show();
                textBox1.Show();
            }
            else
            {
                panel4.Hide();
                textBox1.Hide();
            }
        }

        // DEFINITION OF MATCHING CATEGORIES
        private void button83_Click(object sender, EventArgs e)
        {
            Form502a NForm502;

            NForm502 = new Form502a(WSignedId, WSignRecordNo, WOperator);
            NForm502.ShowDialog();
        }
        //
        // DEFINE CATEGORIES 
        //
        private void button84_Click(object sender, EventArgs e)
        {
            Form503 NForm503;
            int Mode = 1; // Default for ATMs 

            if (W_Application == "e_MOBILE")
            {
                Mode = 7;
            }
            
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
        // MAKE KINA
        private void button7_Click(object sender, EventArgs e)
        {
            // china bank for Journal and CVV
            Color Red = Color.Red;
            button13.BackColor = Red;
            button53.BackColor = Red;
            button54.BackColor = Red;
            button21.BackColor = Red;

            button29.BackColor = Red;

            button75.BackColor = Red;
            button29.BackColor = Red;
            //button34.BackColor = Red;
            button55.BackColor = Red;
            button28.BackColor = Red;

            button18.BackColor = Red;

            labelStep1.Text = "Main Menu - Primer";

        }
        // Make WEB
        private void button93_Click(object sender, EventArgs e)
        {
            // Make WHAT IS NECESSARY TO BE WEB
            Color Red = Color.Red;


        }

        private void button8_Click(object sender, EventArgs e)
        {
            FormHelp help = new FormHelp();
            help.Show();
        }
        //GO TO OPER

        // Go to Define Banks and Accounts for Nostro 
        private void button10_Click(object sender, EventArgs e)
        {
            Form85NV NForm85NV;
            NForm85NV = new Form85NV(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85NV.ShowDialog();
        }

        // DEFINE NOSTRO MATCHING PAIRS 
        //private void button11_Click(object sender, EventArgs e)
        //{
        //    Form503 NForm503;
        //    int Mode = 5; // Only Nostro
        //    NForm503 = new Form503(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
        //    NForm503.ShowDialog();
        //}

        // Help 1 
        private void checkBoxHelp1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp1.Checked == true)
            {
                panelHosted.Show();
                panelHosted.BackColor = Color.SeaGreen;
                textBoxHelp1.BackColor = Color.SeaGreen;
                textBoxHelp2.BackColor = Color.SeaGreen;
                textBoxHelp3.BackColor = Color.SeaGreen;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Define Banks And Accounts";
                buttonHelp2.Text = "Make Matching Categories";
                buttonHelp3.Text = "Assign Owners and Matching Rules";
            }
            else
            {
                // False
                if (checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                }
            }
        }
        // Loading of Swift Statements 
        private void checkBoxHelp2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp2.Checked == true)
            {
                panelHosted.Show();
                panelHosted.BackColor = Color.Magenta;
                textBoxHelp1.BackColor = Color.Magenta;
                textBoxHelp2.BackColor = Color.Magenta;
                textBoxHelp3.BackColor = Color.Magenta;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Load Swift Statements";
                buttonHelp2.Text = "Update Daily Rates";
                buttonHelp3.Text = "Make Matching";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                }
            }
        }
        // Reconciliation Categories 
        private void button49_Click(object sender, EventArgs e)
        {
            Form504 NForm504;
            int Mode = 1; // Default for ATMs 

            if (W_Application == "QAHERA" || W_Application == "ETISALAT" || W_Application == "IPN" || W_Application == "EGATE" )
            {
                Mode = 7;
            }
          
            NForm504 = new Form504(WSignedId, WSignRecordNo, WOperator, W_Application, Mode, "");
            NForm504.ShowDialog();
        }
        // Running Cycles 
        private void button50_Click(object sender, EventArgs e)
        {
            Form200JobCycles NForm200JobCycles;

            string RunningGroup = "ATMs";
            NForm200JobCycles = new Form200JobCycles(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles.ShowDialog();
        }
        // Journal Schedules 
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
        // Journal Loading Service 
        // NEEDED ... DO NOT DELETE
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
                //********************************************************************
                // SUMMARY
                //********************************************************************
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

        // Help One

        private void checkBoxHelp1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBoxHelp1.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "GENERAL DEFINITIONS";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Parameters";
                buttonHelp2.Text = "Entities";
                buttonHelp3.Text = "Matching and Reconciliation";
            }
            else
            {
                // False
                if (checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }

        // Help 2
        private void checkBoxHelp2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBoxHelp2.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "FILES AND MATCHING DEFINITIONS";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp1.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Files/Fields and Rules";
                buttonHelp2.Text = "Matching Categories and Schedule";
                buttonHelp3.Text = "Matching Files, Fields,Stages";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp3.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
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
                labelHost.Text = "RECONCILIATION DEFINITIONS";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Groups and ATMs within";
                buttonHelp2.Text = "Reconc Cat. Vs Matching Cat";
                buttonHelp3.Text = "Owners";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
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
                labelHost.Text = "JOURNAL LOADING SET UP";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Info and schedule per ATM";
                buttonHelp2.Text = "Auto Order per ATM based on Loading Schedule";
                buttonHelp3.Text = "Auto E-Journals Loading";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp5.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
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
                labelHost.Text = "MONITORING OF QUALITY VIEW";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Parameters/Rules";
                buttonHelp2.Text = "Investigate Problems";
                buttonHelp3.Text = "Report Status";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // SW Distribution 
        private void checkBoxHelpSWD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelpSWD.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "SOFTWARE DISTRIBUTION";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Categories";
                buttonHelp2.Text = "Define Packages and their sessions";
                buttonHelp3.Text = "Authorise and Distribute";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false
                    & checkBoxHelp5.Checked == false & checkBoxMigration.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }

        // Migration
        private void checkBoxMigration_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMigration.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "ATMs MIGRATION";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Define Migration Cycle";
                buttonHelp2.Text = "Select and Load Excel";
                buttonHelp3.Text = "Update Or Create New ATMs";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false
                    & checkBoxHelp5.Checked == false & checkBoxHelpSWD.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }

        // ATMs ON MAP
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
        // ATMs On Map
        private void button11_Click_1(object sender, EventArgs e)
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
                MessageBox.Show("There is NO Internet Connection" + Environment.NewLine
                   + "You Cannot proceed"
                   );
                return;
            }

        }
        // Categories and Packages 
        private void button23_Click(object sender, EventArgs e)
        {
            SWDForm502Cat NSWDForm502Cat;

            NSWDForm502Cat = new SWDForm502Cat(WSignedId, WSignRecordNo, WOperator);
            NSWDForm502Cat.ShowDialog();
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            SWDForm502Pac NSWDForm502Pac;

            NSWDForm502Pac = new SWDForm502Pac(WSignedId, WSignRecordNo, WOperator);
            NSWDForm502Pac.ShowDialog();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // Update Us Process number
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 7; // 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            SWD_Form271 NSWD_Form271;

            NSWD_Form271 = new SWD_Form271(WSignedId, WSignRecordNo, WOperator, "SWDCat", 0);
            NSWD_Form271.ShowDialog();
        }

        // History
        private void button12_Click(object sender, EventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54;  // 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            SWD_Form271 NSWD_Form271;

            NSWD_Form271 = new SWD_Form271(WSignedId, WSignRecordNo, WOperator, "SWDCat", 0);
            NSWD_Form271.ShowDialog();
        }

        // DASH BOARD
        private void button17_Click(object sender, EventArgs e)
        {
            SWDForm502DashBoard NSWDForm502DashBoard;

            NSWDForm502DashBoard = new SWDForm502DashBoard(WSignedId, WSignRecordNo, WOperator);
            NSWDForm502DashBoard.ShowDialog();
        }
        // Access Rights 
        private void button20_Click(object sender, EventArgs e)
        {
            Form502AccessRights NForm502AccessRights;

            int Mode = 2; // View

            NForm502AccessRights = new Form502AccessRights(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502AccessRights.ShowDialog();
        }
        // CONTROLS  
        private void buttonControls_Click(object sender, EventArgs e)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
            string WSelectionCriteria;

            string MainFormId = "Form1 - ATMs - ADMIN";

            //Clear Table 
            Tr.DeleteReport57(WSignedId);

            Ur.IsUpdated = false;

            Ur.UpdateUsersAccessRightsInitialiseWithIsUpdated(MainFormId);
            // Parameters
            foreach (Control c in Parameters.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Parameters.Name;
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

            // Entities           
            foreach (Control c in Entities.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Entities.Name;
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

            // Migration

            foreach (Control c in Migration.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Migration.Name;
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

            // E_Journals
            //
            foreach (Control c in E_Journals.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = E_Journals.Name;
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

            // SWDistribution 
            foreach (Control c in SWDistribution.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = SWDistribution.Name;
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

            // Matching 
            foreach (Control c in Matching.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Matching.Name;
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

            // ReconcDefinition
            foreach (Control c in ReconcDefinition.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = ReconcDefinition.Name;
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

            // AdminQuality
            foreach (Control c in AdminQuality.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = AdminQuality.Name;
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

            // AdminAuthorisation
            foreach (Control c in AdminAuthorisation.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = AdminAuthorisation.Name;
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

            MessageBox.Show("Report of Rights Per Role will be printed!");

            string P1 = "Buttons DETAILS For : " + MainFormId;

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R57ATMS ReportATMS57 = new Form56R57ATMS(P1, P2, P3, P4, P5);
            ReportATMS57.Show();

        }
        //Micration Cycles 
        private void button9_Click(object sender, EventArgs e)
        {
            Form502MigrationCycles NForm502MigrationCycles;

            int Mode = 2; // update 

            NForm502MigrationCycles = new Form502MigrationCycles(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502MigrationCycles.ShowDialog();
        }

        // Categories Vs Bins 
        private void button24_Click_1(object sender, EventArgs e)
        {
            Form504b NForm504b;
            int WMode = 1;
            NForm504b = new Form504b(WSignedId, WSignRecordNo, WOperator, WMode);
            NForm504b.ShowDialog();
        }

        // Discrepancies for NBG
        private void button27_Click_1(object sender, EventArgs e)
        {
            // Testing File
            string FullFile = "C:\\RRDM\\FilePool\\AlecosJournalLines\\NB0553C1_20180313_EJ_WN.259";
            RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            Jrt.ConvertJournal(FullFile);
            return;

            string WMatchingCateg = "EWB103";
            Form3_ErrorsAndDiscre NForm3_ErrorsAndDiscre;
            NForm3_ErrorsAndDiscre = new Form3_ErrorsAndDiscre(WSignedId, WSignRecordNo, WSecLevel, WOperator, WMatchingCateg, 203);

            NForm3_ErrorsAndDiscre.ShowDialog();

            //Form78d NForm78d;

            //RRDMMatchingtblHstAtmTxns Mht = new RRDMMatchingtblHstAtmTxns();

            //string SelectionCriteria = " WHERE BankID = 'BNORPHMM' " 
            //          + " AND (TxtLine LIKE '%ERR112%' OR TxtLine LIKE '%ERR115%' )";

            //DateTime TempDate = new DateTime(2017, 05, 16); 

            //Mht.ReadtblHstAtmTxns(SelectionCriteria, TempDate); 

            //string WHeader = "LIST OF Errors For Date : " + TempDate.ToString();

            //NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator,
            //                       Mht.HstAtmTxnsDataTable, WHeader, "Atms-Admin-Errors");

            //NForm78d.ShowDialog();
        }
        // Definition of Universal File Structure 
        private void button33_Click(object sender, EventArgs e)
        {
            Form502_U_Str NForm502_Str;

            NForm502_Str = new Form502_U_Str(WSignedId, WSignRecordNo, WOperator);
            NForm502_Str.ShowDialog();
        }


        // ....................
        // Matching for National Bank
        // For ATMs and JCC

        private void button38_Click(object sender, EventArgs e)
        {

            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

            RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
            //Test This
            // Load all unprocessed records from Primary Tableto the Master Pool 
            //     

            string Message;
            bool ShowMessage;
            bool TestingLoading;
            int I;
            // SHOW MESSAGE 
            string ParId = "719";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                ShowMessage = true;
            }
            else
            {
                ShowMessage = false;
            }

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

            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = WCut_Off_Date.ToShortDateString();

            // Check if problem in Loading 
            RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

            Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WReconcCycleNo);

            if (Flog.DataTableFileMonitorLog.Rows.Count > 0)
            {
                // There is error
                MessageBox.Show("There was a problem during loading." + Environment.NewLine
                                + "Press OK to view."
                                );
                Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

                int Mode = 0; // only the errors

                NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WCut_Off_Date, Mode);
                NForm18_LoadedFilesStatus.ShowDialog();

                // Verification Message 
                if (MessageBox.Show("Do you want to continue despite the problem? " + Environment.NewLine
                                   , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                    // YES Proceed
                }
                else
                {
                    // Stop 
                    return;
                }

            }
            else
            {
                // No Error
                //    buttonERRORS.Hide();
            }

            MessageBox.Show("CUT OFF :.." + labelCutOff.Text + Environment.NewLine
                            + "Loading Journals and Matching will now start" + Environment.NewLine
                            + "Messages of progress will be desplayed during the process" + Environment.NewLine
                            + "Turn Parameter 719 to 'NO' if you wish not to display any messages"
                            );

            DateTime START = DateTime.Now;


            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM();
            Message = "Journal Loading Starts";
            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", "", DateTime.Now, DateTime.Now, Message);

            // INPORT ALL ENTRIES CREATED FROM JOURNAL 
            int WMode = 1;

            //
            // FOR EACH ATM GROUP LOAD TXNS
            //
            RRDMGroups Gr = new RRDMGroups();

            string SelectionCriteria = " WHERE Operator = '" + WOperator + "'";
            Gr.ReadGroupsAndFillTable(SelectionCriteria);

            I = 0;

            // LOOP FOR ALL ATM'S GROUPS

            int WGroup;

            while (I <= (Gr.TableGroupsOfAtms.Rows.Count - 1))
            {

                WGroup = (int)Gr.TableGroupsOfAtms.Rows[I]["GroupNo"];

                WMode = 1;

                // Load per Group

                JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, WGroup, "", 0, WMode);

                if (JrNew.Major_ErrorFound == true)
                {
                    MessageBox.Show("Error Found During Loading.");
                    return;
                }

                I++; // Read Next entry of the table - next Group

            }

            Message = "Journal Loading finishes... records" + JrNew.GrandTotalTxns.ToString();
            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", "", DateTime.Now, DateTime.Now, Message);

            //**********************************************************
            // Insert Message And show it if needed
            //

            Message =
                "RM Cycle : ...." + WReconcCycleNo + Environment.NewLine
                + "CUT Off Date : ..." + WCut_Off_Date.ToShortDateString() + Environment.NewLine
                            + "Parsed Journal Txns for all ATMs" + Environment.NewLine
                            + "moved to RRDM Data Repository." + Environment.NewLine
                            //+ "Total Records read.............: " + JrNew.TotalRecords.ToString() + Environment.NewLine
                            + "Total Txns Moved to Repository : " + JrNew.GrandTotalTxns.ToString();

            //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", "", DateTime.Now, DateTime.Now, Message);


            MessageBox.Show(Message);


            //*******************************************************
            TestingLoading = false;
            //*******************************************************

            if (TestingLoading == true)
            {
                MessageBox.Show("System is at Testing Loading mode. Matching will not be done.");
                return;
            }



            // Load File 
            //RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG Jr = new RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG();

            //RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM();

            //WAtmNo = "00147128";
            //WMode = 1;
            //Jr.ReadAndCreateTables(WSignedId, WSignRecordNo, WOperator,
            //                                         WAtmNo, WMode);

            //MessageBox.Show("Journal Txns For ATM " + WAtmNo + " LOADED.");

            //*********************************************************
            // LOOP For Matching Category
            // For each not ready for matching continue looping
            // For reach category ready for matching call matching class
            // .......Within Matching Class
            // .......Find Groups and populate working files for all ATMs within group
            // ...........Loop For each Group,
            // ...........For each ATM within Group populate working files,
            // ...........Make Matching for each group 
            // .......End Process by updating Matching and reconciliation Category
            // .......Update Full Card Number
            //*********************************************************
            // EXAMPLE: Ej-File , IST , Fiserv
            // If Matching Category = ready then: 
            // Loop: for Group 
            // Loop: for ATMs within Group 
            // 
            // Create Working Files . Each file contains all ATMs
            // 
            //
            // Update Mpa and Other Files for with trace number less than MINMAX
            // 

            //RRDMMatchingCategStageVsMatchingFields Mf = new RRDMMatchingCategStageVsMatchingFields();

            //Mf.CreateStringOfMatchingFieldsForStageA("EWB103", "Stage A");
            //string Temp = Mf.MatchingFieldsStageX;

            int NumberOfMatchedCategories = 0;

            Form78d NForm78d;

            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            string WMatchingCateg = "";
            bool ReadyCat;

            // LOOP FOR Matching Categories

            I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 
                WMatchingCateg = (string)Mc.TableMatchingCateg.Rows[I]["Identity"];
                WOrigin = (string)Mc.TableMatchingCateg.Rows[I]["Origin"];

                ReadyCat = false;

                SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Msf.ReadReconcCategoriesVsSourcebyCategory(WMatchingCateg);
                //
                // For DEMO PURPOSES
                //
                DateTime DemoForNBG333 = new DateTime(2018, 03, 19);
                //DateTime DemoForNBG333 = new DateTime(2018, 10, 19);
                if (WMatchingCateg == "NBG333" & WCut_Off_Date == DemoForNBG333
                    //   || WMatchingCateg == "NBG501"
                    )
                {
                    Msf.TotalZero = 0;
                    Msf.TotalOne = 0;

                    ShowMessage = true;
                }

                if (Msf.RecordFound == true & (Msf.TotalZero == 0 & Msf.TotalOne == 0))
                {
                    // Means that All are -1 (Journal is ecluded) 

                    //******************************************************************
                    //*****************************************************************
                    // Insert Message And show it if needed
                    //

                    Message =
                           "Matching Category :" + WMatchingCateg + Environment.NewLine
                         + "Ready for Matching." + Environment.NewLine
                         + "See following table where the Files to be mached are listed!";

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    if (ShowMessage == true)
                    {
                        MessageBox.Show(Message);
                    }
                    //****************************************************************
                    //******+++++++++++++++++++++++++++*******************************

                    // Update Journal with -1 and DateTime = now
                    if (WOrigin == "Our Atms")
                    {
                        Msf.UpdateReconcCategoryVsSourceRecordProcessCodeForSourceFileName("Atms_Journals_Txns", -1);
                    }

                    ReadyCat = true;

                    NumberOfMatchedCategories = NumberOfMatchedCategories + 1;

                    //WMatchingCateg = "EWB333";
                }
                else
                {

                    //******************************************************************
                    //*****************************************************************
                    // Insert Message And show it if needed
                    //
                    if (Msf.RecordFound == true)
                    {
                        Message =
                         "Matching Category :" + WMatchingCateg + Environment.NewLine
                                   + " Not Ready for Matching. " + Environment.NewLine
                                   + " File..." + Environment.NewLine
                                   + Msf.SourceFileName + Environment.NewLine
                                   + " ....not loaded yet!";
                    }
                    else
                    {
                        Message =
                                                "Matching Category :" + WMatchingCateg + Environment.NewLine
                                                          + " Has No Defined Files. "
                                                          + " ";
                    }


                    //Pt.InsertPerformanceTrace(WOperator, WOperator, 2 , "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    if (WOperator == "ETHNCY2N" & WMatchingCateg == "NBG101")
                    {
                        MessageBox.Show(Message);
                    }

                    if (ShowMessage == true)
                    {
                        MessageBox.Show(Message);
                    }
                    //****************************************************************
                    //******+++++++++++++++++++++++++++*******************************

                    ReadyCat = false;
                }
                #region Do MATCHING FOR THE READ CATEGORIES
                if (ReadyCat == true)
                {
                    Msf.ReadReconcCategoryVsSourcesANDFillTable(SelectionCriteria);

                    string WHeader = "LIST OF FILES FOR MATCHING CATEGORY : " + WMatchingCateg;

                    if (ShowMessage == true)
                    {
                        NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator,
                                           Msf.RMCategoryFilesDataFiles, WHeader, "Atms-Admin", WMatchingCateg, WReconcCycleNo);

                        NForm78d.ShowDialog();
                    }

                    //   
                    // Make Process = Zero which means that Category under reconciliation
                    // 
                    Msf.UpdateReconcCategoryVsSourceRecordProcessCodeToZero(WMatchingCateg);

                    Form78c NForm78c;
                    RRDMMatchingOfTxns_V02_MinMaxDt Mt = new RRDMMatchingOfTxns_V02_MinMaxDt();
                    //  RRDMMatchingOfTxns_V02_MinMaxDtt_TESTING_NBG501 MTest = new RRDMMatchingOfTxns_V02_MinMaxDtt_TESTING_NBG501();

                    bool TestingWorkingFiles = false;

                    //*****************************************************
                    // DO MATCHING 
                    //*****************************************************

                    Message = "Matching Starts";
                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                    if (WMatchingCateg == "NBG501")
                    {

                        //RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();
                        //Ms.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
                        //RRDM_BDC_MatchingTxns_InGeneralTables Del = new RRDM_BDC_MatchingTxns_InGeneralTables();
                        //Del.InsertRecordsInTableFromTextFile_InBulk(Ms.SourceFileId, Ms.SourceDirectory, Ms.SourceFileId, Ms.Delimiter);

                        //MTest.Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup(WOperator, WSignedId, WMatchingCateg,
                        //                                                     WReconcCycleNo, TestingWorkingFiles);
                    }
                    else
                    {
                        Mt.Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup(WOperator, WSignedId, WMatchingCateg,
                                                                                                     WReconcCycleNo, TestingWorkingFiles);
                    }

                    //TableThisCategoryGroups

                    Message = "Matching Finishes";
                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    if (Mt.TableUnMatchedCompressed.Rows.Count > 0)
                    {
                        if (ShowMessage == true)
                        {
                            WHeader = "LIST OF Dublicate And UnMatched";
                            NForm78c = new Form78c(WSignedId, WSignRecordNo, WOperator,
                                                  Mt.TableUnMatchedSaved, Mt.TableUnMatchedCompressed,
                                                  //Mt.TableWorkingFile01, Mt.TableWorkingFile02, Mt.TableWorkingFile03,
                                                  WHeader, "Form78", WMatchingCateg);

                            NForm78c.ShowDialog();
                        }
                    }

                    if (Mt.NumberOfUnmatchedForCategory > 0)
                    {

                        //******************************************************************
                        //*****************************************************************
                        // Insert Message And show it if needed
                        //
                        Message =
                              "ALL records for category " + WMatchingCateg + " have been processed" + Environment.NewLine
                                   + " Discrepancies were found " + Environment.NewLine
                                   + " Please do Reconciliation ";

                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                        if (ShowMessage == true)
                        {
                            MessageBox.Show(Message);
                        }
                        //****************************************************************
                        //******+++++++++++++++++++++++++++*******************************
                    }
                    if (Mt.NumberOfUnmatchedForCategory == 0)
                    {

                        //******************************************************************
                        //*****************************************************************
                        // Insert Message And show it if needed
                        //
                        Message =
                              "ALL records for category " + WMatchingCateg + " have been processed" + Environment.NewLine
                                   + " Discrepancies were NOT found ";

                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                        if (ShowMessage == true)
                        {
                            MessageBox.Show(Message);
                        }
                        //****************************************************************
                        //******+++++++++++++++++++++++++++*******************************
                    }

                    // Turn ProcessMode = 1 , Only for this Category
                    //
                    // ProcessMode = 1 = "Matching Finished" 
                    //
                    //+ " LastMatchingDtTm = @LastMatchingDtTm, 
                    //+ " IsReadThisCycle = @IsReadThisCycle, " = true 
                    //+ " ProcessMode = @ProcessMode "

                    Msf.UpdateReconcCategoryVsSourceRecordProcessCodeToOne(WMatchingCateg);
                }
                #endregion

                I++; // Read Next entry of the table ... Next Category 
            }


            //******************************************************************
            //*****************************************************************
            // Insert Message And show it if needed
            //
            Message =
                 NumberOfMatchedCategories.ToString() + " WHERE MATCHED" + Environment.NewLine
                           + "PROCEED TO RECONCILIATION";

            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

            if (ShowMessage == true)
            {
                MessageBox.Show(Message);
            }
            //****************************************************************
            //******+++++++++++++++++++++++++++*******************************
            Message = "Start :..." + START + Environment.NewLine
                      + "Finish :..." + DateTime.Now;
            MessageBox.Show(Message);

            FormMainScreen_Load(this, new EventArgs());
        }
        // New Version 
        private void button39_Click(object sender, EventArgs e)
        {
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

            //if (IsProductionEnv == "YES" & UnderDevelopment == "YES")
            //{
            //    Form502_V02 NForm502_V02;

            //    NForm502_V02 = new Form502_V02(WSignedId, WSignRecordNo, WOperator);
            //    NForm502_V02.ShowDialog();
            //    return; 
            //}

            Form502_V03 NForm502_V03;
            int Mode = 1; // Normal process
            NForm502_V03 = new Form502_V03(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502_V03.ShowDialog();

        }
        // TRACES PER CRITICAL PROCESSES
        private void button40_Click(object sender, EventArgs e)
        {
            Form8_Traces NForm8_Traces;
            NForm8_Traces = new Form8_Traces(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm8_Traces.ShowDialog();
        }

        private void button35_Click_1(object sender, EventArgs e)
        {

        }
        // Define Output File Structure
        private void button25_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Output_Files NForm200JobCycles_Output_Files;

            string RunningGroup = "ATMs";
            NForm200JobCycles_Output_Files = new Form200JobCycles_Output_Files(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles_Output_Files.FormClosed += NForm152_FormClosed;
            NForm200JobCycles_Output_Files.ShowDialog();

        }
        // Define OUTPUT File Id and Fields Mapping
        private void button26_Click(object sender, EventArgs e)
        {
            Form502_V02_OUT NForm502_V02_OUT;

            NForm502_V02_OUT = new Form502_V02_OUT(WSignedId, WSignRecordNo, WOperator);
            NForm502_V02_OUT.ShowDialog();
        }
        // Archiving cycles
        private void button27_Click(object sender, EventArgs e)
        {
            Form502ArchiveCycles NForm502ArchivingCycles;

            int Mode = 2; // update 

            NForm502ArchivingCycles = new Form502ArchiveCycles(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502ArchivingCycles.ShowDialog();
        }
        // Branches 
        private void button32_Click(object sender, EventArgs e)
        {

            Form503_Branch NForm503_Branch;
            int Mode = 1;
            NForm503_Branch = new Form503_Branch(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            NForm503_Branch.ShowDialog();
        }
        //
        // Group LOAD JOURNAL
        //

        int Group1;
        int Group2;
        int Group3;
        int Group4;
        int Group5;
        int Group6;
        int Group7;
        int Group8;
        int Group9;
        int Group10;
        int Group11;
        int Group12;
        int Group13;
        int Group14;
        int Group15;

        private void button34_Click(object sender, EventArgs e)
        {

            //RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            //Na.ReadSessionsNotesAndValues("00000560", 0, 2);
            //return; 
            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

            RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
            //
            // TEST SUPERVISOR MODE
            //
            //fuid = 25604

            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2 JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2();
            string SM_WAtmNo = "00000531";
            int WFuid = 25604;
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, 0, SM_WAtmNo, WFuid, 3);
            return;
            //Test This
            // Load all unprocessed records from Primary Tableto the Master Pool 
            //     

            // ************************
            // CREATE NOT PRESENT ATMS
            // ************************
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            string WSourceDirectory = "C:\\RRDM\\FilePool\\Atms_Journals_Txns";
            string[] allJournals = Directory.GetFiles(WSourceDirectory, "*.*");
            if (allJournals == null || allJournals.Length == 0)
            {
                MessageBox.Show("No Journals In Directory");

                return;
            }
            foreach (string file in allJournals)
            {
                //var resultLastThreeDigits = file.Substring(file.Length - 7);
                string result0 = file.Substring(file.Length - 11);
                string Temp = result0.Substring(0, 4);
                if (Temp == "_EJ_")
                {
                    // Valid
                    string result1 = file.Substring(file.Length - 7);
                    string Vendor = result1.Substring(0, 3);
                    string JournalName = file.Substring(file.Length - 28);
                    string WAtmNo = JournalName.Substring(0, 8);


                    Ac.ReadAtm(WAtmNo);
                    if (Ac.RecordFound == true)
                    {
                        // ATM Found
                    }
                    else
                    {
                        // Insert ATM_No
                        Ac.CreateNewAtmBasedOnGeneral_Model(WOperator, WAtmNo, JournalName);
                    }
                }
                else
                {
                    // Not Valid 
                }


            }
            //************************************
            //************************************

            return;



            Ac.ReadAtmFrom_Hst_AndCreate_ATM(WOperator);

            string WJobCategory = "ATMs";
            int WReconcCycleNo;
            string Message;
            int I;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();


            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (WReconcCycleNo == 0)
            {
                MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                return;
            }
            DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            labelCycleNo.Text = WReconcCycleNo.ToString();
            labelCutOff.Text = WCut_Off_Date.ToShortDateString();

            MessageBox.Show("CUT OFF :.." + labelCutOff.Text + Environment.NewLine
                            + "Loading Journals and Matching will now start" + Environment.NewLine
                            + "Messages of progress will be desplayed during the process" + Environment.NewLine
                            + "Turn Parameter 719 to 'NO' if you wish not to display any messages"
                            );

            DateTime START = DateTime.Now;

            // Truncate

            //   string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
            string recconConnString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;

            // INITIALISE EJ TEXT table
            RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();
            string TablePhysicalName = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";
            Ej.TruncateTable(WOperator, TablePhysicalName);

            Message = "Journal Loading Starts";
            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans-STARTS", "", DateTime.Now, DateTime.Now, Message);

            // INPORT ALL ENTRIES CREATED FROM JOURNAL 
            int WMode = 1;

            //
            // FOR EACH ATM GROUP LOAD TXNS
            //
            RRDMGroups Gr = new RRDMGroups();
            // RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();

            string SelectionCriteria = " WHERE Operator = '" + WOperator + "'";
            Gr.ReadGroupsAndFillTable(SelectionCriteria);

            I = 0;

            // LOOP FOR ALL ATM'S GROUPS

            int WGroup;
            pbProgressBar.Visible = true;
            resetProgressBar(15);
            this.pbProgressBar.Style = ProgressBarStyle.Blocks;
            while (I <= (Gr.TableGroupsOfAtms.Rows.Count - 1))
            {
                this.pbProgressBar.PerformStep();
                this.pbProgressBar.Refresh();
                updateProgressBar();

                WGroup = (int)Gr.TableGroupsOfAtms.Rows[I]["GroupNo"];
                // Group1 = WGroup;
                //// RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
                // JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group1, "", 0, 1);

                if (WGroup == 101)
                {
                    Group1 = WGroup;
                    Thread thr1 = new Thread(Method1);
                    thr1.Start();
                }
                if (WGroup == 102)
                {
                    Group2 = WGroup;
                    Thread thr2 = new Thread(Method2);
                    thr2.Start();

                }
                if (WGroup == 103)
                {
                    Group3 = WGroup;
                    Thread thr3 = new Thread(Method3);
                    thr3.Start();

                }
                if (WGroup == 104)
                {
                    Group4 = WGroup;
                    Thread thr4 = new Thread(Method4);
                    thr4.Start();

                }
                if (WGroup == 105)
                {
                    Group5 = WGroup;
                    Thread thr5 = new Thread(Method5);
                    thr5.Start();

                }
                if (WGroup == 106)
                {
                    Group6 = WGroup;
                    Thread thr6 = new Thread(Method6);
                    thr6.Start();

                }
                if (WGroup == 107)
                {
                    Group7 = WGroup;
                    Thread thr7 = new Thread(Method7);
                    thr7.Start();

                }

                if (WGroup == 108)
                {
                    Group8 = WGroup;
                    Thread thr8 = new Thread(Method8);
                    thr8.Start();

                }
                if (WGroup == 109)
                {
                    Group9 = WGroup;
                    Thread thr9 = new Thread(Method9);
                    thr9.Start();

                }
                if (WGroup == 110)
                {
                    Group10 = WGroup;
                    Thread thr10 = new Thread(Method10);
                    thr10.Start();

                }
                if (WGroup == 111)
                {
                    Group11 = WGroup;
                    Thread thr11 = new Thread(Method11);
                    thr11.Start();

                }
                if (WGroup == 112)
                {
                    Group12 = WGroup;
                    Thread thr12 = new Thread(Method12);
                    thr12.Start();

                }
                if (WGroup == 113)
                {
                    Group13 = WGroup;
                    Thread thr13 = new Thread(Method13);
                    thr13.Start();

                }
                if (WGroup == 114)
                {
                    Group14 = WGroup;
                    Thread thr14 = new Thread(Method14);
                    thr14.Start();

                }
                if (WGroup == 115)
                {
                    Group15 = WGroup;
                    Thread thr15 = new Thread(Method15);
                    thr15.Start();
                }


                //  WMode = 1;

                // Load per Group

                //   JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, WGroup, "", 0, WMode);

                //if (JrNew.Major_ErrorFound == true)
                //{
                //    MessageBox.Show("Error Found During Loading.");
                //    return;
                //}

                I++; // Read Next entry of the table - next Group

            }

            this.pbProgressBar.Style = ProgressBarStyle.Marquee;
            this.pbProgressBar.Visible = false;
            //Message = "Journal Loading finishes... records" + JrNew.GrandTotalTxns.ToString();
            //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans-FINISH", "", DateTime.Now, DateTime.Now, Message);

            //**********************************************************
            // Insert Message And show it if needed
            //

            //Message =
            //    "RM Cycle : ...." + WReconcCycleNo + Environment.NewLine
            //    + "CUT Off Date : ..." + WCut_Off_Date.ToShortDateString() + Environment.NewLine
            //                + "Parsed Journal Txns for all ATMs" + Environment.NewLine
            //                + "moved to RRDM Data Repository." + Environment.NewLine
            //                //+ "Total Records read.............: " + JrNew.TotalRecords.ToString() + Environment.NewLine
            //                + "Total Txns Moved to Repository : " + JrNew.GrandTotalTxns.ToString();

            ////Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", "", DateTime.Now, DateTime.Now, Message);


            //MessageBox.Show(Message);
        }

        // LOAD IST
        private void button35_Click_2(object sender, EventArgs e)
        {
            //int Mode = 2; // From the administrator
            //Form502_Load_And_Match NForm502_Check_Loading;

            //NForm502_Check_Loading = new Form502_Load_And_Match(WSignedId, WSignRecordNo, WOperator, 2562, Mode);
            //NForm502_Check_Loading.ShowDialog();
            RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

            La.CreateRRDM_Standard_Table_MT_103_AndOthers("MT_103", "", ""); 
             return; 

             //    string WOperator = "BCAIEGCX";
             // FIND CURRENT CUTOFF CYCLE
             RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            DateTime WCut_Off_Date;
            string WJobCategory = "ATMs";

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

                WCut_Off_Date = Rjc.Cut_Off_Date.Date;

                //  WNumberOfLoadingAndMatching = Rjc.NumberOfLoadingAndMatching;
            }
            //
            string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
            int DelCount;
            string SQLCmd = " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                + " WHERE TransDate  < '2019-01-01'   ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            // Read a delimiter file and insert records in table
            //
            pbProgressBar.Visible = true;
            this.pbProgressBar.MarqueeAnimationSpeed = 30;

            string Message = "Loading FILES starts";
            MessageBox.Show(Message);

           // RRDM_LoadFiles_InGeneral_BDC_2 Lf = new RRDM_LoadFiles_InGeneral_BDC_2();


            //resetProgressBar(100);
            //this.timer2.Interval = 2000;
            //this.timer2.Enabled = true;

            //for (int i = 1; i <= 100; i++)
            //{
            //    this.pbProgressBar.PerformStep();
            //    this.pbProgressBar.Refresh();
            //    updateProgressBar();
            //    System.Threading.Thread.Sleep(35);
            //}

            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            RRDMMatchingTxns_InGeneralTables_BDC Ld = new RRDMMatchingTxns_InGeneralTables_BDC();
            //
            // 
            // IST 01 / 07
            //Ms.ReadReconcSourceFilesByFileId("Credit_Card");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Credit_Card\\Credit_Card_20190702.001"; //       
            //    //Lf.InsertRecords_GTL_IST_With_Merchant(Ms.SourceFileId, WorkingDIR
            //    //                           , WReconcCycleNo);

            //    Lf.InsertRecords_GTL_Credit_Card(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}

            //// IST 02/07
            //Ms.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Switch_IST_Txns\\Switch_IST_Txns_20190702.001"; //       
            //    Lf.InsertRecords_GTL_IST(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            //// IST 03/07
            //Ms.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Switch_IST_Txns\\Switch_IST_Txns_20190703.001"; //       
            //    Lf.InsertRecords_GTL_IST(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}

            //RRDM_LoadFiles_InGeneral_BDC_2 Lf = new RRDM_LoadFiles_InGeneral_BDC_2();

            ////// 
            //Ms.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Switch_IST_Txns\\Switch_IST_Txns_20191217.001"; // 

            //    Lf.InsertRecords_GTL_IST(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}


            //Ms.ReadReconcSourceFilesByFileId("Flexcube");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Flexcube\\Flexcube_20200203.001"; // 

            //    Lf.InsertRecords_GTL_FLEXCUBE_2(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            ////// MASTER_CARD
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD\\MASTER_CARD_20191002.001"; // 

            //    Lf.InsertRecords_GTL_MASTER(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            ////// Flexcube 02
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD_POS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{

            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD_POS\\MASTER_CARD_POS_20191024.001"; // 

            //    Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}

            //// 123 20190702
            //Ms.ReadReconcSourceFilesByFileId("Egypt_123_NET");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Egypt_123_NET\\Egypt_123_NET_20200128.001"; //            
            //    Lf.InsertRecords_GTL_123_NET(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            // 123 20190702
            //Ms.ReadReconcSourceFilesByFileId("FAWRY");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\FAWRY\\FAWRY_20200102.001"; //            
            //    Lf.InsertRecords_GTL_FAWRY(Ms.SourceFileId, WorkingDIR
            //                               , WReconcCycleNo);
            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            //Credit_Card_20191230

            //Ms.ReadReconcSourceFilesByFileId("Credit_Card");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\Credit_Card\\Credit_Card_20191231.001"; //            

            //    Lf.InsertRecords_GTL_Credit_Card(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            // MASTER
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD\\MASTER_CARD_20190702.001"; ; //          
            //    Lf.InsertRecords_GTL_MASTER(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);
            //   // Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);
            //    //int r = Ld.stpReturnCode;
            //    //string D = Ld.stpErrorText;
            //}
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD\\MASTER_CARD_20200203.001"; ; //          
            //    Lf.InsertRecords_GTL_MASTER(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);
            //    // Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);
            //    //int r = Ld.stpReturnCode;
            //    //string D = Ld.stpErrorText;
            //}

            //// VISA
            //Ms.ReadReconcSourceFilesByFileId("VISA_CARD");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    ////    //  
            //    ////    string WorkingDIR = Ms.SourceDirectory;
            //    string WorkingDIR = "C:\\RRDM\\FilePool\\VISA_CARD\\VISA_CARD_20191104.001"; //           
            //    Lf.InsertRecords_GTL_VISA(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);
            //    ////    int r = Ld.stpReturnCode;
            //    ////    string D = Ld.stpErrorText;
            //}

            // MASTER POS
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD_POS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD_POS\\MASTER_CARD_POS_20190707.001"; ; //          
            //    Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD_POS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  MASTER_CARD_POS_20200121
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD_POS\\MASTER_CARD_POS_20200123.001"; ; //          
            //    Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}

            //Ms.ReadReconcSourceFilesByFileId("MEEZA_BDC_ATMS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  MASTER_CARD_POS_20200121
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MEEZA_BDC_ATMS\\MEEZA_BDC_ATMS_20191217.001"; ; //          
            //    Lf.InsertRecords_GTL_MEEZA_BDC_ATMS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            //Ms.ReadReconcSourceFilesByFileId("MEEZA_OTHER_ATMS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  MASTER_CARD_POS_20200121
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MEEZA_OTHER_ATMS\\MEEZA_OTHER_ATMS_20191217.001"; ; //          
            //    Lf.InsertRecords_GTL_MEEZA_OTHER_ATMS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            //Ms.ReadReconcSourceFilesByFileId("MEEZA_POS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  MASTER_CARD_POS_20200121
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MEEZA_POS\\MEEZA_POS_20191217.001";  //          
            //    Lf.InsertRecords_GTL_MEEZA_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}

            Ms.ReadReconcSourceFilesByFileId("NCR_FOREX");
            if (Ms.LayoutId == "DelimiterFile")
            {
                //  MASTER_CARD_POS_20200121

                string WorkingDIR = Ms.SourceDirectory;
                WorkingDIR = "C:\\RRDM\\FilePool\\NCR_FOREX\\NCR_FOREX_20191126.001";  //          
                //Lf.InsertRecords_GTL_NCR_FOREX(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

              //  int r = Lf.stpReturnCode;
              //  string D = Lf.stpErrorText;
            }


            //Ms.ReadReconcSourceFilesByFileId("MASTER_CARD_POS");
            //if (Ms.LayoutId == "DelimiterFile")
            //{
            //    //  
            //    string WorkingDIR = Ms.SourceDirectory;
            //    WorkingDIR = "C:\\RRDM\\FilePool\\MASTER_CARD_POS\\MASTER_CARD_POS_20200203.001"; ; //          
            //    Lf.InsertRecords_GTL_MASTER_CARD_POS(Ms.SourceFileId, WorkingDIR, WReconcCycleNo);

            //    int r = Lf.stpReturnCode;
            //    string D = Lf.stpErrorText;
            //}
            Message = "Loading of file/S FINISH";
            MessageBox.Show(Message);

            pbProgressBar.Visible = false;
            this.pbProgressBar.MarqueeAnimationSpeed = 0;
            //this.timer2.Enabled = false;
        }
        private void resetProgressBar(int pbMaxValue)
        {
            // Set Minimum to 1.
            this.pbProgressBar.Minimum = 1;
            // Set Maximum.
            this.pbProgressBar.Maximum = pbMaxValue;
            // Set the Initial Value.
            this.pbProgressBar.Value = 1;
            // Set the Step by which the progress bar will be updated.
            this.pbProgressBar.Step = 1;
        }

        private void updateProgressBar()
        {

            string sPercentage;
            Single x;
            Single y;
            Graphics gr;
            SizeF sz;

            sPercentage = ((int)(this.pbProgressBar.Value * 100 / this.pbProgressBar.Maximum)).ToString() + '%';
            gr = this.pbProgressBar.CreateGraphics();
            sz = gr.MeasureString(sPercentage, this.pbProgressBar.Font, this.pbProgressBar.Width);
            x = (this.pbProgressBar.Width / 2) - (sz.Width / 2);
            y = (this.pbProgressBar.Height / 2) - (sz.Height / 2);
            gr.DrawString(sPercentage, this.pbProgressBar.Font, Brushes.Black, x, y);
        }
        // Testing 
        DataTable TEMPTableFromAction;
     
        // Matched Dates previous YEAR
        private void button41_Click(object sender, EventArgs e)
        {
            WAction = 2; // Per YEAR
            NForm12 = new Form12(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm12.ShowDialog();
        }

        //private void timer2_Tick(object sender, EventArgs e)
        //{
        //    this.pbProgressBar.set;
        //}
        private void Method1()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group1, "", 0, 1);
        }
        private void Method2()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group2, "", 0, 1);
        }
        private void Method3()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group3, "", 0, 1);
        }
        private void Method4()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group4, "", 0, 1);
        }
        private void Method5()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group5, "", 0, 1);
        }
        private void Method6()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group6, "", 0, 1);
        }
        private void Method7()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group7, "", 0, 1);
        }
        private void Method8()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group8, "", 0, 1);
        }
        private void Method9()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group9, "", 0, 1);
        }
        private void Method10()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group10, "", 0, 1);
        }
        private void Method11()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group11, "", 0, 1);
        }
        private void Method12()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group12, "", 0, 1);
        }
        private void Method13()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group13, "", 0, 1);
        }
        private void Method14()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group14, "", 0, 1);
        }
        private void Method15()
        {
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC JrNew = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC();
            JrNew.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, Group15, "", 0, 1);
        }
        // Check ready to load and Match
        private void button42_Click(object sender, EventArgs e)
        {
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

            int Mode = 1; // From the administrator
            Form502_Load_And_Match_BDC NForm502_Check_Loading;

            NForm502_Check_Loading = new Form502_Load_And_Match_BDC(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
            NForm502_Check_Loading.ShowDialog();
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
        // CURRENCY RATES
        private void button44_Click(object sender, EventArgs e)
        {
            Form291NVCcyRatesDefinition NForm291NVCcyRatesDefinition;
            string WSubSystem = "ATMs";
            NForm291NVCcyRatesDefinition = new Form291NVCcyRatesDefinition(WSignedId, WSignRecordNo, WOperator, WSubSystem);
            NForm291NVCcyRatesDefinition.ShowDialog();
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
                textBox1.Text = "0";
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
                    textBox1.Text = "0";
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
        // Extra Fields Mapping 
        private void button47_Click(object sender, EventArgs e)
        {
            Form502_V03 NForm502_V03;
            int Mode = 2; // EXTRA process 
            NForm502_V03 = new Form502_V03(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502_V03.ShowDialog();
        }
        //
        // for viewing 
        //
        private void button48_Click(object sender, EventArgs e)
        {
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
                NForm502_Files_Loaded = new Form502_Files_Loaded(WSignedId, WSignRecordNo, WOperator, W_Application);
                NForm502_Files_Loaded.ShowDialog();
           


        }
        // Actions definition 
        private void button56_Click_1(object sender, EventArgs e)
        {
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
                MessageBox.Show("Under construction");
                return;
            }
            else
            {
                int Mode = 2; // FOR VIEWING 
                Form502_Actions_GL NForm502_Actions_GL;
                // string InSignedId, int SignRecordNo, string InOperator
                NForm502_Actions_GL = new Form502_Actions_GL(WSignedId, WSignRecordNo, WOperator, Mode);
                NForm502_Actions_GL.ShowDialog();

            }
        }
// Matching Categories Wallet 
        private void button37_Click(object sender, EventArgs e)
        {
            Form503_WALLET NForm503_WALLET;

            int  Mode = 7; // For e_mobile 

            NForm503_WALLET = new Form503_WALLET(WSignedId, WSignRecordNo, WOperator, W_Application, Mode);
            NForm503_WALLET.ShowDialog();
        }

        private void buttonDefineMakerChecker_Click(object sender, EventArgs e)
        {
            //Form503_Maker_Checker NForm503_Maker_Checker;
            //int Mode = 1;
            //NForm503_Maker_Checker = new Form503_Maker_Checker(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            //NForm503_Maker_Checker.ShowDialog();
            
            Form1ATMs_ADMIN_Users_Mgmt NForm1ATMs_ADMIN_Users_Mgmt; 
                // GO TO USERS MANAGEMENT 
                NForm1ATMs_ADMIN_Users_Mgmt = new Form1ATMs_ADMIN_Users_Mgmt(WSignedId, WSignRecordNo, WSecLevel, WOperator);
                NForm1ATMs_ADMIN_Users_Mgmt.LoggingOut += NForm1ATMs_LoggingOut;
                NForm1ATMs_ADMIN_Users_Mgmt.Show();
            
            }

        void NForm1ATMs_LoggingOut(object sender, EventArgs e)
        {
            //   SetValues();
            this.Dispose();
        }
    }

    }


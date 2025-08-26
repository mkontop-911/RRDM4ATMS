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
    public partial class Form_ATMS_Controller : Form
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

        //string WJobCategory;

        string W_Application;

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

        #region Form_ATMS_ADMIN
        public Form_ATMS_Controller(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
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
                buttonPresenterCases.Enabled = false;
                buttonAccessRights.Enabled = false;
                buttonCreateSM.Hide();
                buttonBaddies154.Hide();
                buttonGL_TOTALS.Hide();
                buttonCitShortages.Hide();
            }

            //WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            //DateTime WCut_Off_Date = Rjc.Cut_Off_Date;


            //WJobCategory = "ATMs";
            //labelCycleNo.Text = WReconcCycleNo.ToString();
            //labelCutOff.Text = Rjc.Cut_Off_Date.ToShortDateString();


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
                // ALECOS NEW GITHUB
                // Migration 
                //foreach (Control c in Migration.Controls)
                //{
                //    if (c is Button)
                //    {

                //        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                //                               + " AND ButtonName='" + c.Name + "'"
                //                               + " AND SecLevel" + WSecLevel + "= 1 "
                //                               + " AND Operator='" + WOperator + "'";

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

                // E_Journals
                //foreach (Control c in E_Journals.Controls)
                //{
                //    if (c is Button)
                //    {

                //        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                //                              + " AND ButtonName='" + c.Name + "'"
                //                              + " AND SecLevel" + WSecLevel + "= 1 "
                //                              + " AND Operator='" + WOperator + "'";

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

                //// SWDistribution
                //foreach (Control c in SWDistribution.Controls)
                //{
                //    if (c is Button)
                //    {

                //        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                //                               + " AND ButtonName='" + c.Name + "'"
                //                               + " AND SecLevel" + WSecLevel + "= 1 "
                //                               + " AND Operator='" + WOperator + "'";

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

                // Matching
                //foreach (Control c in Matching.Controls)
                //{
                //    if (c is Button)
                //    {

                //        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                //                              + " AND ButtonName='" + c.Name + "'"
                //                              + " AND SecLevel" + WSecLevel + "= 1 "
                //                              + " AND Operator='" + WOperator + "'";

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
        DateTime WCut_Off_Date;
        bool T24Version;

        private void FormMainScreen_Load(object sender, EventArgs e)
        {
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
                    labelStep1.Text = "Controller's Menu_" + W_Application;
                }
                else
                {
                    W_Application = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                        T24Version = true;
                        labelStep1.Text = "Controller's Menu-VER_T24_" + W_Application;
                    }
                    else
                    {
                        labelStep1.Text = "Controller's Menu-_" + W_Application;
                    }

                }
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

            // CLEAR PERFORMANCE TRACE

            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

            DateTime LimitDate = WCut_Off_Date.AddMonths(-6);
            Pt.DeleteLessThanDatePerformanceTrace(LimitDate);

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
            string ParId = "948";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                buttonExcelLoading.Show();
                buttonCIT_Mgmt.Show();
                buttonReplReport.Show();
                buttonGL_View.Show();
                buttonAudiReports.Show();
            }
            else
            {
                buttonExcelLoading.Hide();
                buttonCIT_Mgmt.Hide();
                buttonReplReport.Hide();
                buttonGL_View.Hide();
                buttonAudiReports.Hide();
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
                toolTipButtons.SetToolTip(buttonAccessRights, "Functions and Roles are Displayed." + Environment.NewLine
                                             + "The Administrator alocates rights per role." + Environment.NewLine
                                             + "By pressing update these are saved and applied in system" + Environment.NewLine
                                             + "Roles are assigned to users."
                                             );

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
                toolTipButtons.SetToolTip(button29, "For Cash In calculation previous turnover of similar(matched) days is used." + Environment.NewLine
                                             + "ATM Controller once a month defines the matched days of next period by ATMs category."
                                             );

                //Parameters Setting    
                //toolTipButtons.SetToolTip(button21, "Define System Parameters."
                //                             );

                //Matched Days exceptions   
                toolTipButtons.SetToolTip(button55, "Define Matched days for ATMs exceptions." + Environment.NewLine
                                             + " % Decrease or increase of matched days turnover is updated."
                                             );


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

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);
            WCut_Off_Date = Rjc.Cut_Off_Date;
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

                        // STOP 

                        bool Test11 = false;

                        if (Test11 == true)
                        {
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
            button13.BackColor = Red;
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
        // MAKE KINA
        private void button7_Click(object sender, EventArgs e)
        {
            // china bank for Journal and CVV
            Color Red = Color.Red;
            button13.BackColor = Red;
            button53.BackColor = Red;
            button54.BackColor = Red;
            //button21.BackColor = Red;

            button29.BackColor = Red;

            //button75.BackColor = Red;
            button29.BackColor = Red;
            //button34.BackColor = Red;
            button55.BackColor = Red;
            //button28.BackColor = Red;

            //button18.BackColor = Red;

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
        // NOSTRO HAS ITS OWN MENU_ NOW
        private void button11_Click(object sender, EventArgs e)
        {
            Form503 NForm503;
            int Mode = 5; // Only Nostro
            NForm503 = new Form503(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            NForm503.ShowDialog();
        }

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
                //checkBoxHelp2.Checked = false;
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
                if (checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                }
            }
        }

        // Reconciliation Categories 
        private void button49_Click(object sender, EventArgs e)
        {
            Form504 NForm504;
            int Mode = 1;
            NForm504 = new Form504(WSignedId, WSignRecordNo, WOperator, W_Application, Mode, "");
            NForm504.ShowDialog();
        }
        // Running Cycles 
        private void button50_Click(object sender, EventArgs e)
        {
            Form200JobCycles NForm200JobCycles;

            string RunningGroup = W_Application;
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
                //checkBoxHelp2.Checked = false;
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
                if (checkBoxHelp3.Checked == false
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
                labelHost.Text = "RECONCILIATION Groups DEFINITIONS";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                //checkBoxHelp2.Checked = false;
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
                if (checkBoxHelp1.Checked == false
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
                labelHost.Text = "LOAD AND MATCH";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                // checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxHelpSWD.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Check existance and load journals and files";
                buttonHelp2.Text = "Do Matching";
                buttonHelp3.Text = "Check results";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false
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
                //checkBoxHelp2.Checked = false;
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
                if (checkBoxHelp1.Checked == false
                    & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false
                    & checkBoxHelpSWD.Checked == false & checkBoxMigration.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // Work Allocation
        private void checkBoxHelpSWD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelpSWD.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                labelHost.Text = "Work alloacation";
                panelHosted.BackColor = Color.LavenderBlush;
                textBoxHelp1.BackColor = Color.LavenderBlush;
                textBoxHelp2.BackColor = Color.LavenderBlush;
                textBoxHelp3.BackColor = Color.LavenderBlush;
                //checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                checkBoxMigration.Checked = false;
                //checkBoxHelp6.Checked = false;
                buttonHelp1.Text = "Visit categories in difference";
                buttonHelp2.Text = "Decide and change owner";
                buttonHelp3.Text = "Continuesly monitor progress";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false
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
                //checkBoxHelp2.Checked = false;
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
                if (checkBoxHelp1.Checked == false
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

            //foreach (Control c in Migration.Controls)
            //{
            //    if (c is Button)
            //    {
            //        Tr.MainFormId = MainFormId;
            //        Tr.PanelName = Migration.Name;
            //        Tr.ButtonName = c.Name;
            //        Tr.ButtonText = c.Text;
            //        if (c.Text != "")
            //        {
            //            Tr.InsertReport57(WOperator, WSignedId);

            //            WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
            //                           + " AND PanelName ='" + Tr.PanelName + "'"
            //                           + " AND ButtonName ='" + Tr.ButtonName + "'"
            //                           + " AND Operator ='" + WOperator + "'"
            //                           ;

            //            Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
            //            if (Ur.RecordFound == true)
            //            {
            //                // Only update

            //                Ur.ButtonText = c.Text;
            //                Ur.IsUpdated = true;

            //                Ur.UpdateUsersAccessRights(Ur.SeqNo);
            //            }
            //            else
            //            {
            //                // Insert 

            //                Ur.MainFormId = Tr.MainFormId;
            //                Ur.PanelName = Tr.PanelName;
            //                Ur.ButtonName = Tr.ButtonName;
            //                Ur.ButtonText = Tr.ButtonText;

            //                Ur.SecLevel02 = false;
            //                Ur.SecLevel03 = false;
            //                Ur.SecLevel04 = false;
            //                Ur.SecLevel05 = false;
            //                Ur.SecLevel06 = false;
            //                Ur.SecLevel07 = false;
            //                Ur.SecLevel08 = false;
            //                Ur.SecLevel09 = false;
            //                Ur.SecLevel10 = false;
            //                Ur.SecLevel11 = false;
            //                Ur.SecLevel12 = false;
            //                Ur.SecLevel13 = false;
            //                Ur.SecLevel14 = false;

            //                Ur.IsUpdated = true;

            //                Ur.Operator = WOperator;

            //                Ur.InsertAccessRecord(Ur.ButtonName);
            //            }
            //        }
            //    }
            //}

            // E_Journals
            //
            //foreach (Control c in E_Journals.Controls)
            //{
            //    if (c is Button)
            //    {
            //        Tr.MainFormId = MainFormId;
            //        Tr.PanelName = E_Journals.Name;
            //        Tr.ButtonName = c.Name;
            //        Tr.ButtonText = c.Text;
            //        if (c.Text != "")
            //        {
            //            Tr.InsertReport57(WOperator, WSignedId);

            //            WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
            //                            + " AND PanelName ='" + Tr.PanelName + "'"
            //                            + " AND ButtonName ='" + Tr.ButtonName + "'"
            //                            + " AND Operator ='" + WOperator + "'"
            //                            ;

            //            Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
            //            if (Ur.RecordFound == true)
            //            {
            //                // Only update

            //                Ur.ButtonText = c.Text;
            //                Ur.IsUpdated = true;

            //                Ur.UpdateUsersAccessRights(Ur.SeqNo);
            //            }
            //            else
            //            {
            //                // Insert 

            //                Ur.MainFormId = Tr.MainFormId;
            //                Ur.PanelName = Tr.PanelName;
            //                Ur.ButtonName = Tr.ButtonName;
            //                Ur.ButtonText = Tr.ButtonText;

            //                Ur.SecLevel02 = false;
            //                Ur.SecLevel03 = false;
            //                Ur.SecLevel04 = false;
            //                Ur.SecLevel05 = false;
            //                Ur.SecLevel06 = false;
            //                Ur.SecLevel07 = false;
            //                Ur.SecLevel08 = false;
            //                Ur.SecLevel09 = false;
            //                Ur.SecLevel10 = false;
            //                Ur.SecLevel11 = false;
            //                Ur.SecLevel12 = false;
            //                Ur.SecLevel13 = false;
            //                Ur.SecLevel14 = false;

            //                Ur.IsUpdated = true;

            //                Ur.Operator = WOperator;

            //                Ur.InsertAccessRecord(Ur.ButtonName);
            //            }
            //        }
            //    }
            //}

            // SWDistribution 
            //foreach (Control c in SWDistribution.Controls)
            //{
            //    if (c is Button)
            //    {
            //        Tr.MainFormId = MainFormId;
            //        Tr.PanelName = SWDistribution.Name;
            //        Tr.ButtonName = c.Name;
            //        Tr.ButtonText = c.Text;
            //        if (c.Text != "")
            //        {
            //            Tr.InsertReport57(WOperator, WSignedId);

            //            WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
            //                           + " AND PanelName ='" + Tr.PanelName + "'"
            //                           + " AND ButtonName ='" + Tr.ButtonName + "'"
            //                           + " AND Operator ='" + WOperator + "'"
            //                           ;

            //            Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
            //            if (Ur.RecordFound == true)
            //            {
            //                // Only update

            //                Ur.ButtonText = c.Text;
            //                Ur.IsUpdated = true;

            //                Ur.UpdateUsersAccessRights(Ur.SeqNo);
            //            }
            //            else
            //            {
            //                // Insert 

            //                Ur.MainFormId = Tr.MainFormId;
            //                Ur.PanelName = Tr.PanelName;
            //                Ur.ButtonName = Tr.ButtonName;
            //                Ur.ButtonText = Tr.ButtonText;

            //                Ur.SecLevel02 = false;
            //                Ur.SecLevel03 = false;
            //                Ur.SecLevel04 = false;
            //                Ur.SecLevel05 = false;
            //                Ur.SecLevel06 = false;
            //                Ur.SecLevel07 = false;
            //                Ur.SecLevel08 = false;
            //                Ur.SecLevel09 = false;
            //                Ur.SecLevel10 = false;
            //                Ur.SecLevel11 = false;
            //                Ur.SecLevel12 = false;
            //                Ur.SecLevel13 = false;
            //                Ur.SecLevel14 = false;

            //                Ur.IsUpdated = true;

            //                Ur.Operator = WOperator;

            //                Ur.InsertAccessRecord(Ur.ButtonName);
            //            }
            //        }
            //    }
            //}

            // Matching 
            //foreach (Control c in Matching.Controls)
            //{
            //    if (c is Button)
            //    {
            //        Tr.MainFormId = MainFormId;
            //        Tr.PanelName = Matching.Name;
            //        Tr.ButtonName = c.Name;
            //        Tr.ButtonText = c.Text;
            //        if (c.Text != "")
            //        {
            //            Tr.InsertReport57(WOperator, WSignedId);

            //            WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
            //                              + " AND PanelName ='" + Tr.PanelName + "'"
            //                              + " AND ButtonName ='" + Tr.ButtonName + "'"
            //                              + " AND Operator ='" + WOperator + "'"
            //                              ;

            //            Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
            //            if (Ur.RecordFound == true)
            //            {
            //                // Only update

            //                Ur.ButtonText = c.Text;
            //                Ur.IsUpdated = true;

            //                Ur.UpdateUsersAccessRights(Ur.SeqNo);
            //            }
            //            else
            //            {
            //                // Insert 

            //                Ur.MainFormId = Tr.MainFormId;
            //                Ur.PanelName = Tr.PanelName;
            //                Ur.ButtonName = Tr.ButtonName;
            //                Ur.ButtonText = Tr.ButtonText;

            //                Ur.SecLevel02 = false;
            //                Ur.SecLevel03 = false;
            //                Ur.SecLevel04 = false;
            //                Ur.SecLevel05 = false;
            //                Ur.SecLevel06 = false;
            //                Ur.SecLevel07 = false;
            //                Ur.SecLevel08 = false;
            //                Ur.SecLevel09 = false;
            //                Ur.SecLevel10 = false;
            //                Ur.SecLevel11 = false;
            //                Ur.SecLevel12 = false;
            //                Ur.SecLevel13 = false;
            //                Ur.SecLevel14 = false;

            //                Ur.IsUpdated = true;

            //                Ur.Operator = WOperator;

            //                Ur.InsertAccessRecord(Ur.ButtonName);
            //            }
            //        }
            //    }
            //}

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
            //foreach (Control c in AdminAuthorisation.Controls)
            //{
            //    if (c is Button)
            //    {
            //        Tr.MainFormId = MainFormId;
            //        Tr.PanelName = AdminAuthorisation.Name;
            //        Tr.ButtonName = c.Name;
            //        Tr.ButtonText = c.Text;
            //        if (c.Text != "")
            //        {
            //            Tr.InsertReport57(WOperator, WSignedId);

            //            WSelectionCriteria = " Where MainFormId ='" + Tr.MainFormId + "'"
            //                              + " AND PanelName ='" + Tr.PanelName + "'"
            //                              + " AND ButtonName ='" + Tr.ButtonName + "'"
            //                              + " AND Operator ='" + WOperator + "'"
            //                              ;

            //            Ur.ReadUserAccessRightsBySelectionCriteria(WSelectionCriteria);
            //            if (Ur.RecordFound == true)
            //            {
            //                // Only update

            //                Ur.ButtonText = c.Text;
            //                Ur.IsUpdated = true;

            //                Ur.UpdateUsersAccessRights(Ur.SeqNo);
            //            }
            //            else
            //            {
            //                // Insert 

            //                Ur.MainFormId = Tr.MainFormId;
            //                Ur.PanelName = Tr.PanelName;
            //                Ur.ButtonName = Tr.ButtonName;
            //                Ur.ButtonText = Tr.ButtonText;

            //                Ur.SecLevel02 = false;
            //                Ur.SecLevel03 = false;
            //                Ur.SecLevel04 = false;
            //                Ur.SecLevel05 = false;
            //                Ur.SecLevel06 = false;
            //                Ur.SecLevel07 = false;
            //                Ur.SecLevel08 = false;
            //                Ur.SecLevel09 = false;
            //                Ur.SecLevel10 = false;
            //                Ur.SecLevel11 = false;
            //                Ur.SecLevel12 = false;
            //                Ur.SecLevel13 = false;
            //                Ur.SecLevel14 = false;

            //                Ur.IsUpdated = true;

            //                Ur.Operator = WOperator;

            //                Ur.InsertAccessRecord(Ur.ButtonName);
            //            }
            //        }
            //    }
            //}

            MessageBox.Show("Report of Rights Per Role will be printed!");

            string P1 = "Buttons DETAILS For : " + MainFormId;

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R57ATMS ReportATMS57 = new Form56R57ATMS(P1, P2, P3, P4, P5);
            ReportATMS57.Show();

        }

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

            // WJobCategory = "ATMs";
            int WReconcCycleNo;

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);
            if (WReconcCycleNo == 0)
            {
                MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                return;
            }
            WCut_Off_Date = Rjc.Cut_Off_Date;

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

        // TRACES PER CRITICAL PROCESSES
        private void button40_Click(object sender, EventArgs e)
        {
            Form8_Traces NForm8_Traces;
            NForm8_Traces = new Form8_Traces(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm8_Traces.ShowDialog();
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

        // Check ready to load and Match
        private void button42_Click(object sender, EventArgs e)
        {


            //WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            //DateTime HealthTargetDate;
            //bool Limit = false; 
            //HealthTargetDate = new DateTime(2050, 03, 24);
            //string ParId = "822"; // When version of files changes 
            //string OccurId = "04"; // FOR SYSTEM AVAILABILITY - haelth check 
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            //if (Gp.RecordFound)
            //{
            //    Limit = true; 
            //    try
            //    {
            //        HealthTargetDate = Convert.ToDateTime(Gp.OccuranceNm);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("822 parameter date is wrong");

            //        //ErrorFound = true;
            //        CatchDetails(ex);
            //    }

            //}
            //else
            //{
            //    Limit = false; 
            //    // Not found 
            //}

            //if (Limit == true)
            //{
            //    // Check if current cut off is greater than the limit date 
            //    if (Rjc.Cut_Off_Date > HealthTargetDate)
            //    {
            //        MessageBox.Show("CHECK WITH RRDM." + Environment.NewLine
            //            + "HEALTH OF SYSTEM MUST BE CHECKED..." + HealthTargetDate.ToShortDateString() + Environment.NewLine
            //       + "NOW SYSTEM IS NOT AVAILABLE " + Environment.NewLine
            //       + "IT WILL BE AVAILABLE AFTER CHECKING IS MADE" + Environment.NewLine
            //       //+ "If UAT is availble then creates needs of supporting it. " + Environment.NewLine
            //       );
            //        return;
            //    }
            //}

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);

            int Mode = 1; // From the administrator

            if (Ba.ShortName == "BDC")
            {
                // if (Environment.MachineName == "RRDM-PANICOS" & checkBox1.Checked == true)
                if (checkBox1.Checked == true)
                {
                    // If ENABLE INFO IS CHECKED
                    // Go to multithreading
                    Form502_Load_And_Match_BDC_MULTI NForm502_Check_Loading_MULTI;

                    NForm502_Check_Loading_MULTI = new Form502_Load_And_Match_BDC_MULTI(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                    NForm502_Check_Loading_MULTI.ShowDialog();
                }
                else
                {
                    if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" )
                    {
                        Form502_Load_And_Match_MOBILE NForm502_Load_And_Match_MOBILE;
                        NForm502_Load_And_Match_MOBILE = new Form502_Load_And_Match_MOBILE(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                        NForm502_Load_And_Match_MOBILE.ShowDialog();
                    }
                    else
                    {
                        bool PanicosCheckKey = false;

                        DateTime July20 = new DateTime(2029, 07, 20);

                        if (WCut_Off_Date >= July20)
                        {
                            PanicosCheckKey = true; // Start checking 
                            MessageBox.Show("WE WILL START CHECKING THE KEY-INFORM RRDM AT ONCE");
                            return;
                        }
                        else
                        {
                            PanicosCheckKey = false;
                        }

                        if (Environment.MachineName == "RRDM-PANICOS")
                        {
                            // Do not check key 
                            PanicosCheckKey = false;
                        }

                        PanicosCheckKey = false; // NO CHECKING 

                        if (PanicosCheckKey == true)
                        {
                            Form14b_Input_Key NForm14b_Input_Key;

                            NForm14b_Input_Key = new Form14b_Input_Key(WSignedId, WOperator, WCut_Off_Date, 1);
                            //NForm14b_Input_Key.FormClosed += NForm14b_Input_Key_FormClosed;
                            NForm14b_Input_Key.ShowDialog();

                            if (NForm14b_Input_Key.IsCorrectInputKey == true)
                            {
                                // Continue Work 
                            }
                            else
                            {
                                //MessageBox.Show("Input Key Not Correct");
                                return;
                            }
                        }


                        Form502_Load_And_Match_BDC NForm502_Check_Loading;

                        NForm502_Check_Loading = new Form502_Load_And_Match_BDC(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                        NForm502_Check_Loading.ShowDialog();
                    }

                }
            }

            if (Ba.ShortName == "ABE")
            {

                Form502_Load_And_Match_ABE NForm502_Check_Loading_ABE;

                NForm502_Check_Loading_ABE = new Form502_Load_And_Match_ABE(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Check_Loading_ABE.ShowDialog();
                //Form502_Load_And_Match_AUD NForm502_Check_Loading_AUD;

                //NForm502_Check_Loading_AUD = new Form502_Load_And_Match_AUD(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                //NForm502_Check_Loading_AUD.ShowDialog();

            }

            if (Ba.ShortName == "AUD")
            {

                Form502_Load_And_Match_AUD NForm502_Check_Loading_AUD;

                NForm502_Check_Loading_AUD = new Form502_Load_And_Match_AUD(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Check_Loading_AUD.ShowDialog();
            }

            if (Ba.ShortName == "EGA")
            {
                Form502_Load_And_Match_MOBILE NForm502_Load_And_Match_MOBILE;
                NForm502_Load_And_Match_MOBILE = new Form502_Load_And_Match_MOBILE(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Load_And_Match_MOBILE.ShowDialog();
            }
        }

        void NForm14b_Input_Key_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ComingFromMeta = true;
            // UPDATE TRANSACTION 
            //if (NForm14b_Input_Key.Confirmed == true)
            //{

            //}
            //else
            //{
            //    return; 
            //}


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
            NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, WSecLevel, WOperator, 1);
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
            // WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "IPN" || W_Application == "EGATE")
            {
                Form502_Load_And_Match_MOBILE NForm502_Load_And_Match_MOBILE;
                NForm502_Load_And_Match_MOBILE = new Form502_Load_And_Match_MOBILE(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Load_And_Match_MOBILE.ShowDialog();
            }
            else
            {
                Form502_Load_And_Match_BDC NForm502_Check_Loading;

                NForm502_Check_Loading = new Form502_Load_And_Match_BDC(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, Mode);
                NForm502_Check_Loading.ShowDialog();
            }




        }
        // VIEW FILES 
        private void button50_Click_1(object sender, EventArgs e)
        {
            //WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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

        //
        // Job Cycles 
        //
        private void button85_Click(object sender, EventArgs e)
        {
            Form200JobCycles NForm200JobCycles;

            //string RunningGroup = "ATMs";
            NForm200JobCycles = new Form200JobCycles(WSignedId, WSignRecordNo, WSecLevel, WOperator, W_Application);
            NForm200JobCycles.FormClosed += NForm200JobCycles_FormClosed;
            NForm200JobCycles.ShowDialog();
        }

        private void NForm200JobCycles_FormClosed(object sender, FormClosedEventArgs e)
        {
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);
            WCut_Off_Date = Rjc.Cut_Off_Date;

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
            if (W_Application == "ATMs")
            {
                if (MessageBox.Show("Do you want to delete all data for all Cycles?  " + Environment.NewLine
                               + "ARE YOU SURE ...? " + Environment.NewLine
                               , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                {
                    // YES Proceed
                    //MessageBox.Show("This process might take few minutes. " + Environment.NewLine
                    //               + "Wait till a final message is shown. ");
                }
                else
                {
                    // Stop 
                    return;
                }


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
            }


            // 
            // Clear data for MOBILE 
            // 
            bool RUN_Mobile = false;
            string RCT3 = "";
            if (W_Application == "ETISALAT")
            {
                RCT3 = "ATMS.[dbo].[Stp_Refresh_Testing_Data_ETISALAT]";
                RUN_Mobile = true;
            }
            if (W_Application == "QAHERA")
            {
                RCT3 = "ATMS.[dbo].[Stp_Refresh_Testing_Data_QAHERA]";
                RUN_Mobile = true;
            }
            if (W_Application == "IPN")
            {
                RCT3 = "ATMS.[dbo].[Stp_Refresh_Testing_Data_IPN]";
                RUN_Mobile = true;
            }

            if (RUN_Mobile == true)
            {
                int rows = -20;
                string connectionString3 = ConfigurationManager.ConnectionStrings
                  ["ATMSConnectionString"].ConnectionString;

                using (SqlConnection conn3 =
                   new SqlConnection(connectionString3))
                    try
                    {
                        conn3.Open();
                        using (SqlCommand cmd =
                           new SqlCommand(RCT3, conn3))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            // Parameters

                            rows = cmd.ExecuteNonQuery();
                            //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                            //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                        }
                        // Close conn
                        conn3.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }

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


        // Pesenter cases 
        private void buttonPresenterCases_Click(object sender, EventArgs e)
        {
            Form200JobCycles_Presenter NForm200JobCycles_Presenter;
            // Button Presenter Managemenet 
            string RunningGroup = "ATMs";
            NForm200JobCycles_Presenter = new Form200JobCycles_Presenter(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup, 1);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm200JobCycles_Presenter.ShowDialog();
        }
        // Cycle reports
        private void buttonCycleReports_Click(object sender, EventArgs e)
        {

            if (W_Application == "ATMs")
            {
                Form200JobCycles_Reports NForm200JobCycles_Reports;

                string RunningGroup = W_Application;
                NForm200JobCycles_Reports = new Form200JobCycles_Reports(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
                // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
                NForm200JobCycles_Reports.ShowDialog();
            }
            else
            {
                Form200JobCycles_Reports_EWALLET NForm200JobCycles_Reports_EWALLET;

                string RunningGroup = W_Application;
                NForm200JobCycles_Reports_EWALLET = new Form200JobCycles_Reports_EWALLET(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
                // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
                NForm200JobCycles_Reports_EWALLET.ShowDialog();
            }

        }
        // HOLIDAYS
        private void button28_Click_1(object sender, EventArgs e)
        {
            WAction = 1;
            NForm15 = new Form15(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm15.ShowDialog();
        }
        // Performance traces 
        private void button75_Click_1(object sender, EventArgs e)
        {
            Form8_Traces_Oper NForm8_Traces_Oper;
            NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, WSecLevel, WOperator, 1);
            NForm8_Traces_Oper.ShowDialog();
        }
        // Category view
        private void button24_Click(object sender, EventArgs e)
        {

            Form80a3 NForm80a3;
            string WFunction = "View";
            string Category = "All";

            string WhatBank = WOperator;

            NForm80a3 = new Form80a3(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

            NForm80a3.ShowDialog();
        }
        // GL Totals 
        private void buttonGL_TOTALS_Click(object sender, EventArgs e)
        {
            Form200JobCycles_GL NForm200JobCycles_GL;

            string RunningGroup = "ATMs";
            NForm200JobCycles_GL = new Form200JobCycles_GL(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
            NForm200JobCycles_GL.ShowDialog();
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
            //WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            //WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            Form153 NForm153;
            int WMode = 1;
            NForm153 = new Form153(WOperator, WSignedId, WReconcCycleNo, WMode);
            NForm153.ShowDialog();
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

            MessageBox.Show("Users can not sign In" + Environment.NewLine
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
        // Dublicates Authorisations 
        private void buttonDublAuth_Click(object sender, EventArgs e)
        {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            Ap.FindDuplicateRecordsInAuth();

            if (Ap.TableDublicates_Auth.Rows.Count > 1)
            {
                Form78d_BasedOnDataTable NForm78d_DublicateRecords;
                // textBoxFileId.Text
                int WMode = 3; //

                NForm78d_DublicateRecords = new Form78d_BasedOnDataTable(WOperator, WSignedId, "Authorizations", Ap.TableDublicates_Auth
                                                                                                            , WMode);
                NForm78d_DublicateRecords.Show();
            }
            else
            {
                MessageBox.Show("No Duplicates In Authorisations");
            }
        }

        // Excel Loading 
        private void buttonExcelLoading_Click(object sender, EventArgs e)
        {
            Form18_ExcelMainMenu NForm18_ExcelMainMenu;
            WAction = 2;

            NForm18_ExcelMainMenu = new Form18_ExcelMainMenu(WSignedId, WSignRecordNo, WSecLevel, WOperator, WCut_Off_Date, WAction);
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
            //WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            NForm503_SesCombined = new Form503_SesCombined(WSignedId, WSignRecordNo, WOperator, WOrigin, WReconcCycleNo, "", 0, Mode);
            NForm503_SesCombined.ShowDialog();

            return;


            //WJobCategory = "ATMs";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
        // Manual TXN Input
        private void buttonManualTxn_input_Click(object sender, EventArgs e)
        {
            Form503_Insert_Manual_To_Mpa NForm503_Insert_Manual_To_Mpa;

            int WMode = 1;

            NForm503_Insert_Manual_To_Mpa = new Form503_Insert_Manual_To_Mpa(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo, WMode, T24Version);
            // NForm200JobCycles_Presenter.FormClosed += NForm200JobCycles_FormClosed; ;
            NForm503_Insert_Manual_To_Mpa.ShowDialog();
        }
        // Form 154
        private void buttonBaddies154_Click(object sender, EventArgs e)
        {
            //WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application);

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
            NForm154 = new Form154(WOperator, WSignedId, WReconcCycleNo, WMode, "");
            NForm154.ShowDialog();
        }
        // Create JLN 
        private void button3_Click(object sender, EventArgs e)
        {

            //return; 

            //C:\RRDM\Archives\Atms_Journals_Txns\20221213_213\00015603_20221206_EJ_WCR.002
            //C:\RRDM\Archives\Atms_Journals_Txns\20221213_213\00015591_20221205_EJ_WCR.002
            //C:\RRDM\Archives\Atms_Journals_Txns\20221213_213\00015696_20221206_EJ_WCR.002
            //C:\RRDM\Archives\Atms_Journals_Txns\20221213_213\00000465_20221210_EJ_WCR.000
            //C:\RRDM\FilePool\Atms_Journals_Txns\01900501_20210831_EJ_NCR.001

            // "C:\RRDM\FilePool\Atms_Journals_Txns\RA122002_20250403_EJ_BT2.000"
            // RA209002_20250404_EJ_NCR.733
            string WJournalTxtFile = "C:\\RRDM\\FilePool\\Atms_Journals_Txns\\00017002_20250811_EJ_NCR.000";
             
            string jlnFullPathName;
            RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            jlnFullPathName = Jrt.ConvertJournal(WJournalTxtFile); // Converted File 
                                                                   // LineCount = Jrt.LineCounter

        }
        // TEST AUTO
        private void buttonTESTAUto_Click(object sender, EventArgs e)
        {
            string text = "Testing Auto";
            string caption = "Caption";
            int timeout = 5000;
            //AutoClosingMessageBox Ab = new AutoClosingMessageBox(text, caption, timeout);

            AutoClosingMessageBox.Show(text, caption, timeout);

            AutoClosingMessageBox.Show("Second Message", "Caption", timeout);

        }
        // SET SEMAPHONE TO NO
        private void buttonSetSema_NO_Click(object sender, EventArgs e)
        {
            RRDMGasParameters Gp = new RRDMGasParameters();
            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
            //string WOperator = "BCAIEGCX";
            string ParId = "555";
            string OccurId = "01";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            Gp.OccuranceNm = "NO";
            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // semaphoreObject.Release();
        }
        // test TPF Excel 
        private void button2_Click(object sender, EventArgs e)
        {
            RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = W_Application;

            string WCategory = "";

            if (W_Application == "ETISALAT")
            {
                WCategory = "ETI310";
            }
            if (W_Application == "QAHERA")
            {
                WCategory = "QAH310";
            }

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WCategory == "ETI310" || WCategory == "QAH310")
            {
                //  CREATE THE TWO EXCELS
                string SelectionCriteria = " WHERE MatchingCateg ='"
                           + WCategory + "' AND MatchingAtRMCycle =" + WReconcCycleNo
                           + " AND IsMatchingDone = 1 and Matched = 0 "
                           ;
                //No Dates Are selected
                DateTime NullPastDate = new DateTime(1900, 01, 01);
                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                string WSortCriteria = " ORDER BY TransDate ";

                int CallingMode = 3; // Create Excels 

                Mmob.ReadMatchingTxnsMaster_MOBILE_ByRangeAndFillTableForExcels(WOperator, WSignedId, CallingMode, SelectionCriteria,
                        WSortCriteria, FromDt, ToDt, 2, 0, W_Application);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = W_Application;
            string WCategory = "ETI310";
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WCategory == "ETI310" || WCategory == "QAH310")
            {
                //  CREATE THE TWO EXCELS
                string SelectionCriteria = " WHERE MatchingCateg ='"
                           + WCategory + "' AND MatchingAtRMCycle =" + WReconcCycleNo
                           + " AND IsMatchingDone = 1 and Matched = 0 "
                           ;
                //No Dates Are selected
                DateTime NullPastDate = new DateTime(1900, 01, 01);
                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                string WSortCriteria = " ORDER BY TransDate ";

                int CallingMode = 3; // Create Excels 

                Mmob.ReadMatchingTxnsMaster_MOBILE_ByRangeAndFillTableForExcels(WOperator, WSignedId, CallingMode, SelectionCriteria,
                        WSortCriteria, FromDt, ToDt, 2, 0, W_Application);
            }
        }
// Check Active Directory 
        private void buttonCheckActive_Click(object sender, EventArgs e)
        {
            Form40_Active_DIR NForm40_Active_DIR;

            NForm40_Active_DIR = new Form40_Active_DIR();

            NForm40_Active_DIR.ShowDialog();


            //Form3_PreInv NForm3_PreInv;

            //NForm3_PreInv = new Form3_PreInv(WSignedId, WSignRecordNo, WSecLevel, WOperator);

            //NForm3_PreInv.ShowDialog();
        }
// CHECK RECORDS IN IST
        private void buttonCheckIST_Click(object sender, EventArgs e)
        {
            // SHOW RECORDS IN IST 
            //
            Form78d_Discre NForm78d_Discre;
            int WMode = 21; // Show IST Records by Matching Categ and Net-DATE

            NForm78d_Discre = new Form78d_Discre(WOperator, WSignedId, "", WReconcCycleNo, WMode, "");
            NForm78d_Discre.ShowDialog();
        }
// Combine files 
        private void buttonReplaceCode_Click(object sender, EventArgs e)
        {
            string[] inputFiles = { "C:\\RRDM\\FilePool_ETI\\ETISALAT_TPF_TXNS\\ETISALAT_TPF_TXNS_20250109.001", "C:\\RRDM\\FilePool_ETI\\ETISALAT_TPF_TXNS\\ETISALAT_TPF_TXNS_20250110_No_header.002" }; // Add your file names here
            string outputFile = "C:\\RRDM\\FilePool_ETI\\ETISALAT_TPF_TXNS\\CombineTPF.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (string file in inputFiles)
                    {
                        if (File.Exists(file))
                        {
                            string[] lines = File.ReadAllLines(file);
                            foreach (string line in lines)
                            {
                                writer.WriteLine(line);
                            }
                          //  writer.WriteLine(); // Add a blank line between files (optional)
                        }
                    }
                }
                MessageBox.Show("File Created"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
// Correct the CIT Inconsistencies after Loanding 
        private void buttonCorrectCitExcel_Click(object sender, EventArgs e)
        {
            Form18_CIT_ExcelOutput_Alerts_BDC NForm18_CIT_ExcelOutput_Alerts_BDC;
            // string InSignedId, int SignRecordNo, string InOperator
            NForm18_CIT_ExcelOutput_Alerts_BDC = new Form18_CIT_ExcelOutput_Alerts_BDC(WSignedId, WSignRecordNo, WOperator);
            NForm18_CIT_ExcelOutput_Alerts_BDC.ShowDialog();
        }
// Divide files 
        private void buttonDivideFiles_Click(object sender, EventArgs e)
        {
            RRDM_DIVIDE_ZIP_FILES Div = new RRDM_DIVIDE_ZIP_FILES();
            string zipFilePath = "C:\\OSMAN_V2\\BIGFILE.zip";
            MessageBox.Show("Insert the big file in OSMAN_V2 directory which is in C:" + Environment.NewLine
                + "The inserted file must have the name BIGFILE.zip "
                + "The file is expected to be bigger than 40 GB "
                + "The creater parts will be in the same folder OSMAN_V2 "
                + "Their names will be BIGFILE.zip.part0000, BIGFILE.zip.part0001 etc "
                 ); 
            // Call method to create 
            Div.SplitZip(zipFilePath, 40);
            
        }
// Merge 
        private void buttonMergeFiles_Click(object sender, EventArgs e)
        {
            RRDM_DIVIDE_ZIP_FILES Div = new RRDM_DIVIDE_ZIP_FILES();
            string inputFile = "C:\\OSMAN_V2\\BIGFILE.zip";
            string outputFile = "C:\\OSMAN_V2\\TPF.zip";
            MessageBox.Show("The parts to merge must be in OSMAN_V2 directory which is in C:" + Environment.NewLine
              + "Their names BIGFILE.zip.part0000, BIGFILE.zip.part0001 etc "
              + "The created file will have the name TPF.zip under the same directory OSMAN_V2 "
               );
            Div.ReconstructZip(outputFile, inputFile);
           // Div.MergeFiles_ZIP(WSignedId);
        }
    }
}

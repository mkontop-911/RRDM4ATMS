using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

//24-04-2015 Alecos
using System.Diagnostics;
using System.Text;

//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form_NOSTRO_OPERATIONAL_Copy : Form
    {
        public EventHandler LoggingOut;
 
        Form13 NForm13;
 
        Form21MISNOSTRO NForm21MISNOSTRO;   // For general MIS 

        Form136 NForm136; // Datagrid showing all Country Banks for a Group 

        Form143 NForm143;

        Form54 NForm54;
        Form55 NForm55;

        Form78 NForm78;
      
        Form191 NForm191;
    
        Form193 NForm193;

        Form112 NForm112;

        string MsgFilter;

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        int WApplication_int; 

        bool FromVisa; 
        // NOTES 
        string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WOrigin;
        string WSubSystem;

        string WBankId;

        int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form_NOSTRO_OPERATIONAL_Copy(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
           
            if (Usi.SignInApplication == "NOSTRO" )
            {
                WApplication_int = 3 ;
                WSubSystem = "NostroReconciliation";
                WOrigin = "Nostro - Vostro"; 
            }
            if (Usi.SignInApplication == "POS SETTLEMENT")
            {
                WApplication_int = 4 ; 
                WSubSystem = "VisaReconciliation";
                WOrigin = "Visa";
                FromVisa = true;
                labelStep1.Text = "VISA SETTLEMENT";
                button14.Text = "VisaSe  Matching Categories ";
            }
            if (Usi.SignInApplication == "E_FINANCE RECONCILIATION")
            {
                WSubSystem = "E_FINANCE";
                WOrigin = "E_FINANCE";

                labelStep1.Text = "E_FINANCE For Branch"; 

                if (WSecLevel == "04")
                {
                    Us.ReadUsersRecord(WSignedId);

                    labelStep1.Text = "E_FINANCE For Branch :.." + Us.Branch;
                }

                if (WSecLevel == "05")
                {
                    Us.ReadUsersRecord(WSignedId);

                    labelStep1.Text = "E_FINANCE For Central Operation" ;
                }

                button14.Text = "E_FINANCE Matching Categories ";
                button41.Text = "E_FINANCE Matching Process ";
            }

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            string WSelectionCriteria = "Insert Data"; 
            // DISSABLE BUTTONS 
            if (WSelectionCriteria != "")
            {
                RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();

                string MainFormId = "Form1 - NOSTRO - OPERATIONAL";

                // NOSTRO_ADMIN
                foreach (Control c in NOSTRO_ADMIN.Controls)
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
                // Manager_Nostro
                foreach (Control c in Manager_Nostro.Controls)
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

                // ReconcNostro
                foreach (Control c in ReconcNostro.Controls)
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

                // Reconc_Visa
                foreach (Control c in Reconc_Visa.Controls)
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

                // Disputes_Nostro
                foreach (Control c in Disputes_Nostro.Controls)
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

                // Monistoring_Nostro
                foreach (Control c in Monistoring_Nostro.Controls)
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

                // MIS_Nostro
                foreach (Control c in MIS_Nostro.Controls)
                {
                    if (c is Button)
                    {

                        WSelectionCriteria = " Where MainFormId ='" + MainFormId + "'"
                                                  + " AND ButtonName='" + c.Name + "'"
                                                  + " AND SecLevel" + WSecLevel + "= 1 "
                                                   //  + " AND Operator ='" + WOperator + "'"
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
                // Quality_Nostro
                foreach (Control c in Quality_Nostro.Controls)
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

                // Authorisation_Nostro
                foreach (Control c in Authorisation_Nostro.Controls)
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

            //if (WSignedId == "7777")
            //{
            //    foreach (Control c in this.panel2.Controls)
            //    {
            //        if (c is Button)
            //        {
            //            string Temp = c.Name.Substring(6, 2);
            //            //  MessageBox.Show(c.Name);
            //            MessageBox.Show("Update this 804 parameter");
            //            Gp.ReadParametersSpecificId(WOperator, "804", Temp, "803", WSecLevel.ToString());

            //            if (Gp.RecordFound == true)
            //            {
            //                c.Enabled = true;

            //                MessageBox.Show(c.Name);
            //                MessageBox.Show(c.Text);
            //            }
            //            else
            //            {
            //                //  button18.Enabled = false;
            //                //  button18.BackColor = Color.Silver;
            //                //   button18.ForeColor = Color.DarkGray;
            //                c.Enabled = false;
            //                c.BackColor = Color.Silver;
            //                c.ForeColor = Color.DarkGray;
            //            }

            //        }
            //    }
            //}

            //  CASH MANAGEMENT Prior Replenishment Workflow  ... Show or Hide Button 
            string ParId = "211";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string CashEstPriorReplen = Gp.OccuranceNm; // IF YES THEN IS PRIOR THOUGHT AN ACTION 

            if (CashEstPriorReplen != "YES") // If No calculation before Replenishment is needed
            {
                //button94.Enabled = false;

            }

            labelUSERno.Text = WSignedId;



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

            toolTipButtons.SetToolTip(Reconc_Nostro, "OPERATION" + Environment.NewLine
                                         + "Through the below buttons you operate the system."
                                         );

            toolTipButtons.SetToolTip(label4, "MONITORING" + Environment.NewLine
                                         + "Central controller monitors all ATMs by area."
                                         );

            toolTipButtons.SetToolTip(label1, "DISPUTES" + Environment.NewLine
                                        + "Register and manage disputes."
                                        );
            toolTipButtons.SetToolTip(label10, "MIS" + Environment.NewLine
                                       + "Special Reports for management."
                                       );
            toolTipButtons.SetToolTip(label5, "QUALITY" + Environment.NewLine
                                       + "Audit Trail and Exception Reports for KPI."
                                       );
            toolTipButtons.SetToolTip(label11, "ELECTRONIC AUTHORISATIONS" + Environment.NewLine
                                       + "Management of Authorisations for " + Environment.NewLine
                                       + "Requestors and Authorisers ."
                                       );
        }

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

            string WCitId = "1000";
            // Create table with the ATMs this user can access
            Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

            //-----------------UPDATE LATEST TRANSACTIONS----------------------//
            // Update latest transactions from Journal 
            Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

            //-----------------------------------------------------------------// 

            RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();


            string SelectionCriteria = "Operator ='" + WOperator + "'"
                               + " AND OpenRecord = 1 ";

            bool WithDate = false;
            Tc.ReadAllTransToBePostedAndFillTable(WSignedId, SelectionCriteria, NullPastDate, WithDate);

            if (Tc.TotalSelected > 0)
            {
                //checkBoxTransToBePosted.Text = Tc.TotalSelected.ToString();

                checkBoxTransToBePosted.Show();
                checkBoxTransToBePosted.Checked = true;

            }
            else checkBoxTransToBePosted.Hide();

            //**************************************************************************
            // END OF FUNCTION
            //**************************************************************************

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

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;

                //Trans to be posted  
                toolTipButtons.SetToolTip(button35, "The transactions are created from different processes within the system such as:" + Environment.NewLine
                                             + "During Replenishment for money in and out from ATMs." + Environment.NewLine
                                             + "Actions on MetaExceptions in the reconciliation process." + Environment.NewLine
                                             + "Actions to settle a dispute." + Environment.NewLine
                                             + "Actions related with CIT providers" + Environment.NewLine
                                             + "Transactions can be automatically posted or Vouchers are printed for manual posting"
                                             );


                //Today's operation Status 
                toolTipButtons.SetToolTip(button12, "View current Replenishment and Reconciliation status." + Environment.NewLine
                                             + "Drilling down facilities are available for more details."
                                             );

                //Categories work allocation
                toolTipButtons.SetToolTip(button86, "RM Categories in difference are shown." + Environment.NewLine
                                             + "Based on the availability of personnel work(RM Categories) can be alocated." + Environment.NewLine
                                             + "At any time during the day these can be reviewed and realocation is made when necessary"
                                             );

                //Today's reports  
                toolTipButtons.SetToolTip(button16, "View a list of all daily reports." + Environment.NewLine
                                             + "A number of other reports can be created on customer request."
                                             );

                // Shows the ATMs on MAP         
                toolTipButtons.SetToolTip(button73, "Each ATM is marked on Map." + Environment.NewLine
                                             + "Double click on it to see the exact addreess and type."
                                             );

                //MIS- ATMs replenishment and Reconciliation  operational performance 
                toolTipButtons.SetToolTip(button63, "MIS - Reporting." + Environment.NewLine
                                             + "Useful management information for replenishment and reconciliation per period." + Environment.NewLine
                                             + "Drilling down to individual ATM is available for a more detailed analysis ."
                                             );



                //Departure from quality indicators     
                toolTipButtons.SetToolTip(button32, "View the expected level of quality defined and agreed with management." + Environment.NewLine
                                             + "Performance drivers can be: time taken for replenishment, number of errors kept outstanding etc."
                                             );

                //System Audit Trail     
                toolTipButtons.SetToolTip(button19, "View activity of system maintenance." + Environment.NewLine
                                             + "Use search to view past changes on system."
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

                //MIS - Historical Information on disputes        
                toolTipButtons.SetToolTip(button65, "View settled and outstanding disputes by period." + Environment.NewLine
                                             + "Drilling down facilities is available."
                                             );

                //MIS - Settled Disputes by officer         
                toolTipButtons.SetToolTip(button27, "The performance of each officer is measured against the Settled Disputes." + Environment.NewLine
                                             + "Drilling down facilities is available."
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



        // DISPUTE REGISTRATION  
        private void button68_Click(object sender, EventArgs e)
        {
            Form9NV NForm9NV;
            int Wf = 2;
            NForm9NV = new Form9NV(WSignedId, WSignRecordNo, WSecLevel, WOperator, Wf);
            NForm9NV.ShowDialog();
            //int From = 1;
            //NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator,"",  0, 0, 0 ,"",From,"");
            //NForm5.ShowDialog(); ;
        }

        // MANAGE DISPUTES 
        private void button69_Click(object sender, EventArgs e)
        {
            Form3_NOSTRO NForm3_NOSTRO;
            string Mode = "UPDATE";
            NForm3_NOSTRO = new Form3_NOSTRO(WSignedId, WSignRecordNo, WOperator,
                            Mode, 0);
            NForm3_NOSTRO.FormClosed += NForm3_NOSTRO_FormClosed;
            NForm3_NOSTRO.ShowDialog();
        }

        void NForm3_NOSTRO_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
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


        // Refresh Testing data 
        //
        //bool FromRefreshTestingData ;

        private void button22_Click(object sender, EventArgs e)
        {
            // 
            string connectionString = ConfigurationManager.ConnectionStrings
              ["ATMSConnectionString"].ConnectionString;

            string RCT = "ATMS.[dbo].[Stp_Refresh_Testing_Data_NOSTRO]";

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

            MessageBox.Show("Testing Data NOSTRO have been initialised");

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



        // MIS REPORTS - Key Indicators 

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

                this.NForm21MISNOSTRO = new Form21MISNOSTRO(WSignedId, WSignRecordNo, WSecLevel, WOperator, WSubSystem , WAction);
                this.NForm21MISNOSTRO.ShowDialog(); ;
            }

        }

        // MIS REPORTS - PERSONNEL PERFORMANCE 
        private void button65_Click_1(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  

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

                NForm21MISNOSTRO = new Form21MISNOSTRO(WSignedId, WSignRecordNo, WSecLevel, WOperator, WSubSystem , WAction);
                NForm21MISNOSTRO.ShowDialog(); ;
            }
        }
        //
        // NOSTRO Banks Indicators 
        //
        private void button11_Click_1(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  

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
                WAction = 3;

                NForm21MISNOSTRO = new Form21MISNOSTRO(WSignedId, WSignRecordNo, WSecLevel, WOperator, WSubSystem, WAction);
                NForm21MISNOSTRO.ShowDialog(); ;
            }
        }

        // AGING ANALYSIS
        private void button3_Click(object sender, EventArgs e)
        {

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
                WAction = 4;

                NForm21MISNOSTRO = new Form21MISNOSTRO(WSignedId, WSignRecordNo, WSecLevel, WOperator, WSubSystem, WAction);
                NForm21MISNOSTRO.ShowDialog(); ;
            }
        }

        // General Audit trail 
        // This will be based on screen image captured 

        private void button19_Click(object sender, EventArgs e)
        {
            Form76 NForm76 = new Form76(WSignedId, WSignRecordNo, WOperator);
            NForm76.ShowDialog(); ;
            //MessageBox.Show("General Audit trail this will be based on screen image captured + record registration id");  
        }


        // DAILY REPORTS 
        private void button16_Click(object sender, EventArgs e)
        {
            Form9NV NForm9NV;
            int Wf = 1;
            NForm9NV = new Form9NV(WSignedId, WSignRecordNo, WSecLevel, WOperator, Wf);
            NForm9NV.ShowDialog();

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



        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckForMessages(); // Check for messages from controller

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            Ap.CheckOutstandingAuthorizationsStage1(WSignedId); // There is record for authorizer with Stage 1 
            if (Ap.RecordFound == true)
            {
                Console.Beep(2000, 500);
                Console.Beep(3000, 1000);
                Console.Beep(2000, 500);
                MessageBox.Show(new Form { TopMost = true }, "You have requests for authorization. From : " + Ap.Requestor
                    );
            }

            Ap.CheckOutstandingAuthorizationsStage3(WSignedId); // Threre is record for Requestor with stage 3 
            if (Ap.RecordFound == true)
            {
                Console.Beep(2000, 500);
                Console.Beep(3000, 1000);
                Console.Beep(2000, 500);
                MessageBox.Show(new Form { TopMost = true }, "Authorization has been made. by : " + Ap.Authoriser);
            }

            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            Ap.ReadAuthorizationsUserTotal(WSignedId);
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
        // MIS FOR DISPUTES 
        private void button27_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Closed Disputes by Officer.");
        }


        // Show All Alerts 
        private void button73_Click(object sender, EventArgs e)
        {
            Form80bNV NForm80bNV;
            string WCategoryId = "ALL";
            int WReconcCycleNo = 0;
            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem  , WCategoryId, WReconcCycleNo,
                                              0, "ViewAllAlerts", NullPastDate, NullPastDate);

            NForm80bNV.ShowDialog();

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


        void NForm80aITMX_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }



        // Work Allocation 
        private void button86_Click(object sender, EventArgs e)
        {
            Form80cNV NForm80cNV;
            string WFunction = "Allocate";
            NForm80cNV = new Form80cNV(WSignedId, WSignRecordNo, WOperator, WFunction);
            NForm80cNV.ShowDialog();
        }


        private void button8_Click(object sender, EventArgs e)
        {
            FormHelp help = new FormHelp();
            help.Show();
        }

        // Nostro Manual 
        private void button24_Click_1(object sender, EventArgs e)
        {
            Form80aNostro NForm80aNostro;

            string WFunction = "ReconcNostro";

            string Category = "All";

            NForm80aNostro = new Form80aNostro(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aNostro.ShowDialog();
        }
        // Nostro To Be Confirmed 
        private void button15_Click_1(object sender, EventArgs e)
        {
            Form80aNostro NForm80aNostro;

            string WFunction = "Confirm Entries";

            string Category = "All";

            NForm80aNostro = new Form80aNostro(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aNostro.FormClosed += NForm80aNostro_FormClosed;
            NForm80aNostro.ShowDialog();
        }

        private void NForm80aNostro_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }

        // NOSTRO VIEW TXNs 
        private void button23_Click_1(object sender, EventArgs e)
        {
            Form80aNostro NForm80aNostro;

            string WFunction = "View";

            string Category = "All";

            NForm80aNostro = new Form80aNostro(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aNostro.ShowDialog();
        }
        // NOSTRO ACCOUNTS 
        private void button29_Click_1(object sender, EventArgs e)
        {
            // Show all Pairs 
            // Show statistics
            // Show last statement date 
            // Outstanding Unmatched
            // Owner Alerts 
            // Not printed transactions  

            Form291NVAccountStatusALL NForm291NVAccountStatusALL;

            int WRunningCycle = 203;
            int Mode = 5;

            NForm291NVAccountStatusALL = new Form291NVAccountStatusALL(WSignedId, WSignRecordNo, WOperator, WRunningCycle, Mode);
            NForm291NVAccountStatusALL.Show();
        }
        // NOSTRO MATCHED 
        private void button33_Click_1(object sender, EventArgs e)
        {
            Form80aNostro NForm80aNostro;

            string WFunction = "Matched";

            string Category = "All";

            NForm80aNostro = new Form80aNostro(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aNostro.ShowDialog();
        }
        // NOSTRO ALL OUTSTANDING ADJUSTMENTS 
        private void button28_Click_1(object sender, EventArgs e)
        {
            Form502aNostroAdjALL NForm502aNostroAdjALL;

            NForm502aNostroAdjALL = new Form502aNostroAdjALL(WSignedId, WSignRecordNo, WOperator, WSubSystem);
            NForm502aNostroAdjALL.ShowDialog();
        }

        // MAKE MATCHING 
      
        private void button34_Click_1(object sender, EventArgs e)
        {
            // MAKE MATCHING 
            Form291NVMakeSystemMatching NForm291NVMakeSystemMatching;

            NForm291NVMakeSystemMatching = new Form291NVMakeSystemMatching(WSignedId, WSignRecordNo, WOperator, WSubSystem);

            NForm291NVMakeSystemMatching.ShowDialog();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }
        // OUTSTANDING PAIR 
        private void button31_Click_1(object sender, EventArgs e)
        {
            Form80aNostro NForm80aNostro;

            string WFunction = "OutstandingAdjust";

            string Category = "All";

            NForm80aNostro = new Form80aNostro(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aNostro.ShowDialog();
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

            Form200NOSTRO NForm200NOSTRO;
            if (Ba.BanksInGroup == 1)
            {   // 0 stands for CitNo... CitNo>0 when calling from Form18 (Cit View)
                string RunningGroup = "NOSTRO";
                NForm200NOSTRO = new Form200NOSTRO(WSignedId, WSignRecordNo, WSecLevel, WOperator, WSubSystem ,RunningGroup);
                NForm200NOSTRO.ShowDialog();
            }
        }

        // OWN BANKS
        private void button21_Click_1(object sender, EventArgs e)
        {
            NForm143 = new Form143(WSignedId, WSignRecordNo, WOperator);
            NForm143.ShowDialog(); ;
        }
        // USERS 
        private void button54_Click_1(object sender, EventArgs e)
        {
            NForm13 = new Form13(WSignedId, WSignRecordNo, WOperator, "1000", WApplication_int);
            NForm13.ShowDialog(); ;
        }
        // Parameters 
        private void button27_Click_1(object sender, EventArgs e)
        {
            NForm191 = new Form191(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm191.ShowDialog();
        }
        // Vostro Banks and Accounts 
        private void button18_Click(object sender, EventArgs e)
        {
            Form85NV NForm85NV;
            NForm85NV = new Form85NV(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85NV.ShowDialog();
        }
        // Matching Pairs 
        private void button14_Click_1(object sender, EventArgs e)
        {
            Form503 NForm503;
            int Mode = 5; // Only Nostro
            if (labelStep1.Text == "VISA SETTLEMENT")
            {
                Mode = 6; // Visa Mode 
            }
          
            NForm503 = new Form503(WSignedId, WSignRecordNo, WOperator, WOrigin ,Mode);
            NForm503.ShowDialog();
        }
        // Currency Rates Definition 
        private void button13_Click_1(object sender, EventArgs e)
        {
            Form291NVCcyRatesDefinition NForm291NVCcyRatesDefinition;

            NForm291NVCcyRatesDefinition = new Form291NVCcyRatesDefinition(WSignedId, WSignRecordNo, WOperator, WSubSystem);
            NForm291NVCcyRatesDefinition.ShowDialog();
        }
        // Configure 1
        private void checkBoxHelp1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp1.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                panelHosted.BackColor = Color.BlueViolet;
                textBoxHelp1.BackColor = Color.BlueViolet;
                textBoxHelp2.BackColor = Color.BlueViolet;
                textBoxHelp3.BackColor = Color.BlueViolet;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Define Users";
                buttonHelp2.Text = "Internal and External Accounts";
                buttonHelp3.Text = "Categories and their owners";
            }
            else
            {
                // False
                if (checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // Configure 2
        private void checkBoxHelp2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp2.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                panelHosted.BackColor = Color.BlueViolet;
                textBoxHelp1.BackColor = Color.BlueViolet;
                textBoxHelp2.BackColor = Color.BlueViolet;
                textBoxHelp3.BackColor = Color.BlueViolet;
                checkBoxHelp1.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Matching Rules";
                buttonHelp2.Text = "Alert conditions";
                buttonHelp3.Text = "Currency Rates";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }


        private void checkBoxHelp3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHelp3.Checked == true)
            {
                panelHosted.Show();
                labelHost.Show();
                panelHosted.BackColor = Color.BlueViolet;
                textBoxHelp1.BackColor = Color.BlueViolet;
                textBoxHelp2.BackColor = Color.BlueViolet;
                textBoxHelp3.BackColor = Color.BlueViolet;
                checkBoxHelp2.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp5.Checked = false;
                if (FromVisa == true)
                {
                    buttonHelp1.Text = "Visa Files Loading";
                }          
                else 
                {
                    buttonHelp1.Text = "Statements Loading";
                }
                buttonHelp2.Text = "System Matching";
                buttonHelp3.Text = "Work Allocation";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false & checkBoxHelp4.Checked == false & checkBoxHelp5.Checked == false)
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
                panelHosted.BackColor = Color.BlueViolet;
                textBoxHelp1.BackColor = Color.BlueViolet;
                textBoxHelp2.BackColor = Color.BlueViolet;
                textBoxHelp3.BackColor = Color.BlueViolet;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp1.Checked = false;
                checkBoxHelp5.Checked = false;
                buttonHelp1.Text = "Manual Matching";
                buttonHelp2.Text = "Confirmed And Adjustments";
                buttonHelp3.Text = "Post Adjustments";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp5.Checked == false)
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
                panelHosted.BackColor = Color.BlueViolet;
                textBoxHelp1.BackColor = Color.BlueViolet;
                textBoxHelp2.BackColor = Color.BlueViolet;
                textBoxHelp3.BackColor = Color.BlueViolet;
                checkBoxHelp2.Checked = false;
                checkBoxHelp3.Checked = false;
                checkBoxHelp4.Checked = false;
                checkBoxHelp1.Checked = false;
                buttonHelp1.Text = "Monitor Work";
                buttonHelp2.Text = "Handle Unposted Adjustments";
                buttonHelp3.Text = "Handle Disputes";
            }
            else
            {
                // False
                if (checkBoxHelp1.Checked == false & checkBoxHelp2.Checked == false & checkBoxHelp3.Checked == false & checkBoxHelp4.Checked == false)
                {
                    panelHosted.Hide();
                    labelHost.Hide();
                }
            }
        }
        // View Print ALL
        private void button2_Click_2(object sender, EventArgs e)
        {
            Form80bNV NForm80bNV;
            string WCategoryId = "ALL";
            int WReconcCycleNo = 0;
            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WReconcCycleNo,
                                              0, "View", NullPastDate, NullPastDate);

            NForm80bNV.ShowDialog();
        }
// Matching For Cards 
        private void button41_Click(object sender, EventArgs e)
        {
            WSubSystem = "CardsSettlement"; 
            Form291NVMakeSystemMatching_Cards NForm291NVMakeSystemMatching_Cards;

            NForm291NVMakeSystemMatching_Cards = new Form291NVMakeSystemMatching_Cards(WSignedId, WSignRecordNo, WOperator, WSubSystem);

            NForm291NVMakeSystemMatching_Cards.ShowDialog();
        }

// Mnaual Operation for Cards 
        private void button43_Click(object sender, EventArgs e)
        {
            Form80aVisa_2 NForm80aVisa_2;

            string WFunction = "ReconcVisa";
            string WSubSystem = "CardsSettlement"; 
            string Category = "All";

            NForm80aVisa_2 = new Form80aVisa_2(WSignedId, WSignRecordNo, WOperator, WSubSystem, WFunction, Category, "");
            NForm80aVisa_2.ShowDialog();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            Form80aVisa NForm80aVisa;

            string WFunction = "Matched";

            string Category = "All";

            NForm80aVisa = new Form80aVisa(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aVisa.ShowDialog();
        }
// To Be Confirmed 
        private void button45_Click(object sender, EventArgs e)
        {
            Form80aVisa NForm80aVisa;
           
            string WFunction = "Confirm Entries";

            string Category = "All";

            NForm80aVisa = new Form80aVisa(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            NForm80aVisa.ShowDialog();
        }
// Define Access rights
        private void buttonDefineAccess_Click(object sender, EventArgs e)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
            string WSelectionCriteria;

            string MainFormId = "Form1 - NOSTRO - OPERATIONAL";

            //Clear Table 
            Tr.DeleteReport57(WSignedId);

            Ur.IsUpdated = false;

            Ur.UpdateUsersAccessRightsInitialiseWithIsUpdated(MainFormId);
            // NOSTRO_ADMIN
            foreach (Control c in NOSTRO_ADMIN.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = NOSTRO_ADMIN.Name;
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

            // Manager_Nostro           
            foreach (Control c in Manager_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Manager_Nostro.Name;
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

            // ReconcNostro
            foreach (Control c in ReconcNostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = ReconcNostro.Name;
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

            // Reconc_Visa           
            foreach (Control c in Reconc_Visa.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Reconc_Visa.Name;
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
            // Disputes_Nostro
            foreach (Control c in Disputes_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Disputes_Nostro.Name;
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

            // Monistoring_Nostro           
            foreach (Control c in Monistoring_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Monistoring_Nostro.Name;
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
            // MIS_Nostro
            foreach (Control c in MIS_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = MIS_Nostro.Name;
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

            // Quality_Nostro           
            foreach (Control c in Quality_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Quality_Nostro.Name;
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

            // Authorisation_Nostro        
            foreach (Control c in Authorisation_Nostro.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Authorisation_Nostro.Name;
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

            string P1 = "Buttons DETAILS For NOSTRO ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WBankId;
            string P5 = WSignedId;

            Form56R57ATMS ReportATMS57 = new Form56R57ATMS(P1, P2, P3, P4, P5);
            ReportATMS57.Show();

        }
// Access Rights 
        private void button20_Click(object sender, EventArgs e)
        {
            Form502AccessRights_NOSTRO NForm502AccessRights_NOSTRO;

            int Mode = 2; // View

            NForm502AccessRights_NOSTRO = new Form502AccessRights_NOSTRO(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502AccessRights_NOSTRO.ShowDialog();
        }
// loading code for E_Finance
        private void button_E_Finance_Click(object sender, EventArgs e)
        {
            // READ AN rtf file and turn it to txt
            string path = @"C:\KONTO\E-Finance 25-09-2018 E-finance (3).rtf";
            System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
            string rtfText = System.IO.File.ReadAllText(path);
            rtBox.Rtf = rtfText;
            string plainText = rtBox.Text;
            // Console.WriteLine(plainText);

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\KONTO\Output_E-Finance 25-09-2018_5.txt"))
            {
                file.WriteLine(plainText);
                file.Close();
            }

            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file2 =
                new System.IO.StreamReader(@"C:\KONTO\Output_E-Finance 25-09-2018_5.txt");
            while ((line = file2.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                counter++;
            }

            file2.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  
            System.Console.ReadLine();

            // Manipulate a line 

            //string myString = "2,160.00	201899000157346	25/09/2018";
            string myString = "0.00	 0.00	 16,797.88	201899000157584	25/09/2018 11:43 	114134416123793";
            string[] subStrings = myString.Split();

            foreach (string str in subStrings)
            {
                if (str != "")
                {
                    Console.WriteLine(str);
                }
            }

            // Create a working table 
        }
    }
}

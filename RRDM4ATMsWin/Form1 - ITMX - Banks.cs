using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;
using System.IO;

//24-04-2015 Alecos
using System.Diagnostics;


namespace RRDM4ATMsWin
{
    public partial class Form1ITMXBANKS : Form
    {
        public EventHandler LoggingOut;

        Form108 NForm108;
        Form8 NForm8;
        Form12 NForm12;
        Form13 NForm13;
        Form15 NForm15;

        Form21MIS NForm21MIS;

        Form3_ITMX NForm3ITMX;

        //Form116 NForm116; // Dispute pre investigation 

        Form18 NForm18; // CIT Providers  

        Form136 NForm136; // Datagrid showing all Country Banks for a Group 

        Form10 NForm10;

        Form9 NForm9;

        Form143 NForm143;
        Form45 NForm45;
        Form47 NForm47;
    
        Form52 NForm52;
        Form53 NForm53;
        Form152 NForm152;

        Form54 NForm54;
        Form55 NForm55;

        Form63 NForm63;
      
        Form78 NForm78;
        //Form81 NForm81;

        Form85 NForm85;

        Form191 NForm191;
        Form192 NForm192;
        Form193 NForm193;

        Form19a NForm19;

        Form300 NForm300;

        Form112 NForm112;

        string MsgFilter;

        RRDMUsersAccessToAtms Uatms = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();

        RRDMBanks Ba = new RRDMBanks(); 

        // NOTES 
        //string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WBankId;
        bool ITMXUser;
    
        int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form1ITMXBANKS(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;

            Ba.ReadBank(WBankId);

            if (Ba.Logo == null)
            {
                pictureBox3.Image = null;
                MessageBox.Show("No Logo assigned yet!");
            }
            else
            {
                MemoryStream ms = new MemoryStream(Ba.Logo);
                pictureBox3.Image = Image.FromStream(ms);
            }

            if (Us.Operator == WBankId)
            {
                ITMXUser = true;
                labelStep1.Text = "Main Menu for : " + WOperator;
            }
            else
            {
                ITMXUser = false;
                labelStep1.Text = "Main Menu for : " + WOperator + "/" + WBankId;

                checkBox6.Checked = true;

                checkBox2.Hide();
                checkBox3.Hide();
                checkBox4.Hide();
                checkBox5.Hide();              
            }
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (ITMXUser == false)
            {

                button45.Show(); 

                button97.Text = "VIEW " + WBankId;

                checkBox6.Checked = true;

            }

            if (WSignedId == "7777")
            {
                foreach (Control c in this.panel2.Controls)
                {
                    if (c is Button)
                    {
                        string Temp = c.Name.Substring(6, 2);
                        MessageBox.Show("Update this 804 parameter");
                        Gp.ReadParametersSpecificId(WOperator, "804", Temp, "803", WSecLevel.ToString());

                        if (Gp.RecordFound == true)
                        {
                            c.Enabled = true;

                            MessageBox.Show(c.Name);
                            MessageBox.Show(c.Text);
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

            toolTipButtons.SetToolTip(label2, "INFRASTRUCTURE" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );
            toolTipButtons.SetToolTip(labelInformation, "OPERATION" + Environment.NewLine
                                         + "Through the below buttons you operate the system."
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

        private void FormITMXBANKS_Load(object sender, EventArgs e)
        {
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

            string SelectionCriteria = "Operator ='" + WOperator + "'" + " AND AuthUser='" + WSignedId + "'"
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
            //
            // Unmatched Trans to be posted Grid
            //**************************************************************************
            SelectionCriteria = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "' AND HostMatched = 0 AND ActionCd2 = 1" + " AND BankId='" + WBankId + "'";

            WithDate = false;
            Tc.ReadAllTransToBePostedAndFillTable(WSignedId, SelectionCriteria, NullPastDate, WithDate);

            if (Tc.TotalSelected > 0)
            {
                //labelTransUnMatched.Text = Tc.TotalSelected.ToString();

                checkBoxTransUnMatched.Show();
                checkBoxTransUnMatched.Checked = true;
            }
            else checkBoxTransUnMatched.Hide();

            //**************************************************************************
            //CHECK FOR DISPUTES FOR THE SIGNED PERSON
            //**************************************************************************

            if (ITMXUser == false)
            {

                SelectionCriteria = "Operator ='" + WOperator + "'" + " AND BankId='" + WBankId + "'"
                                        + " AND Active = 1 ";

                WithDate = false;
                Di.ReadDisputesInTable(SelectionCriteria, NullPastDate, WithDate);

                if (Di.TotalSelected > 0)
                {
                    //labelOutstandingDisputes.Text = Di.TotalSelected.ToString();
                    checkBoxOutstandingDisputes.Show();
                    checkBoxOutstandingDisputes.Checked = true;
                }
                else checkBoxOutstandingDisputes.Hide();

            }
            else
            {

                SelectionCriteria = "Operator ='" + WOperator + "'" + " AND BankId='" + WBankId + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";

                WithDate = false;
                Di.ReadDisputesInTable(SelectionCriteria, NullPastDate, WithDate);

                if (Di.TotalSelected > 0)
                {
                    //labelOutstandingDisputes.Text = Di.TotalSelected.ToString();
                    checkBoxOutstandingDisputes.Show();
                    checkBoxOutstandingDisputes.Checked = true;
                }
                else checkBoxOutstandingDisputes.Hide();
            }
            //**************************************************************************
            //END
            //**************************************************************************

            toolTipHeadings.ShowAlways = true;

            //Define Banks 
            toolTipButtons.SetToolTip(label2, "Infrastructure" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;

                //Users and CIT providers.   
                toolTipButtons.SetToolTip(button54, "Define Users for this Bank." + Environment.NewLine
                                             + "Define access role levels." + Environment.NewLine
                                             + "Define Authorisers."
                                             );

                //Parameters Setting    
                toolTipButtons.SetToolTip(button21, "Define System Parameters."
                                             );

                //Quality Parameters setting   
                toolTipButtons.SetToolTip(button36, "Set Quality key indicators." + Environment.NewLine
                                             + "Depature from these indicators is reported by the system."
                                             );

                //Settlement Totals 
                toolTipButtons.SetToolTip(button99, "FT Settlement Totals are presented" + Environment.NewLine
                                             + "For each Settlement Cycle the totals for all Banks are shown." + Environment.NewLine
                                             + "For each Bank analysis is shown with all other Banks." + Environment.NewLine
                                             + "What it is shown it can be printed."
                                             );

                //Daily statement
                toolTipButtons.SetToolTip(button41, "For a range of Settlement Cycles" + Environment.NewLine
                                             + "the net position for a particular bank is shown." + Environment.NewLine
                                             + "A graph is shown as well to show balance fluctuation." + Environment.NewLine
                                             + "What it is shown it can be printed."
                                             );

                //Fees Settlement Totals 
                toolTipButtons.SetToolTip(button48, "Fees Settlement Totals are presented" + Environment.NewLine
                                             + "For each Settlement Cycle the fees totals for all Banks are shown." + Environment.NewLine
                                             + "For each Bank analysis is shown with all other Banks." + Environment.NewLine
                                             + "What it is shown it can be printed."
                                             );

                //Fees Daily statement
                toolTipButtons.SetToolTip(button49, "For a range of Settlement Cycles" + Environment.NewLine
                                             + "the net Fees position for a particular bank is shown." + Environment.NewLine
                                             + "A graph is shown as well to show balance fluctuation." + Environment.NewLine
                                             + "What it is shown it can be printed."
                                             );

                //View Fees 
                toolTipButtons.SetToolTip(button50, "View the Fees Versions And Layers" + Environment.NewLine
                                             + "Divition of fees to entities can been seen too." + Environment.NewLine
                                             + "What it is shown it can be printed."
                                             );

                // System Performance analysis         
                toolTipButtons.SetToolTip(button74, "Review critical process performance." + Environment.NewLine
                                             + "An analysis is displayed and alerts are sent when beyond predefined limits."
                                             );

                //Trans to be posted  
                toolTipButtons.SetToolTip(button35, "The transactions are created from different processes within the system such as:" + Environment.NewLine
                                             + "During Replenishment for money in and out from ATMs." + Environment.NewLine
                                             + "Actions on MetaExceptions in the reconciliation process." + Environment.NewLine
                                             + "Actions to settle a dispute." + Environment.NewLine
                                             + "Actions related with CIT providers" + Environment.NewLine
                                             + "Transactions can be automatically posted or Vouchers are printed for manual posting"
                                             );

                //Today's reports  
                toolTipButtons.SetToolTip(button16, "View a list of all daily reports." + Environment.NewLine
                                             + "A number of other reports can be created on customer request."
                                             );

                //Cut Off 
                toolTipButtons.SetToolTip(button45, "At cut off do:" + Environment.NewLine
                                             + "a) Create a new cut off record in RRDM" + Environment.NewLine
                                             + "b) Copy records of this Job Cycle to RRDM" + Environment.NewLine
                                             + "c) Create open tickets for files to be loaded" + Environment.NewLine
                                             + "d) Create settlement totals"
                                              );

                //Departure from quality indicators     
                toolTipButtons.SetToolTip(button32, "View the expected level of quality defined and agreed with management." + Environment.NewLine
                                             + "Performance drivers can be: time taken for replenishment, number of errors kept outstanding etc."
                                             );

                //System Audit Trail     
                toolTipButtons.SetToolTip(button19, "View activity of system maintenance." + Environment.NewLine
                                             + "Use search to view past changes on system."
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

                //MIS - Settled Disputes by officer         
                toolTipButtons.SetToolTip(button27, "The performance of each officer is measured against the Settled Disputes." + Environment.NewLine
                                             + "Drilling down facilities is available."
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

        private void FormMainScreenSwitch_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoggingOut(sender, e);
        }
        // Form Closed

        // Disable / enable Ballon Info 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                toolTipButtons.Active = true;
                FormITMXBANKS_Load(this, new EventArgs());
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
            if (WSecLevel == "10" || WSignedId == "1005" || WSignedId == "SERVE31" || WSignedId == "500")
            {
                NForm108 = new Form108(WSignedId, WSignRecordNo, WOperator);
                NForm108.ShowDialog(); ;
            }
            else
            {
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

            //if ( ITMXUser == true)
            //{
            //    // Sixth which is zero is CitId
            //    NForm13 = new Form13(WSignedId, WSignRecordNo, WOperator, "1000");
            //    NForm13.ShowDialog(); ;
            //}
            //else
            //{
            //    MessageBox.Show("Currently not allowed operation. "); 
            //}

            NForm13 = new Form13(WSignedId, WSignRecordNo, WOperator, "1000", 2);
            NForm13.ShowDialog(); ;

        }

        // Replenishement 
        // REPLENISHMENT ..... 
        // ..........
        // SINGLES
        // WFunction = 1 Normal branch ATM



        // Reconciliation 
        // RECONCILIATION ..... 
        // ........
        // 

        private void button58_Click(object sender, EventArgs e)
        {
            Uatms.ReadUserAccessToAtms(WSignedId); // READ TO SEE GROUP OR ATM 

            if (Uatms.NoOfAtmsReconc == 0 & Uatms.NoOfGroupsReconc == 0)
            {
                MessageBox.Show(" YOU ARE NOT AUTHORIZE FOR RECONCILIATION ");
                return;
            }

            if (Uatms.NoOfGroupsReconc > 0)
            {
                MessageBox.Show(" YOU HAVE A GROUP. Push the GROUP Button please ");
                return;
            }


            if (Uatms.NoOfAtmsReconc > 0)
            {
                WAction = 2; // Reconciliation for individual ATMs

                NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
                NForm152.FormClosed += NForm152_FormClosed;
                NForm152.ShowDialog(); ;
            }
        }

        private void NForm152_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormITMXBANKS_Load(this, new EventArgs());
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
            Form200ITMX NForm200ITMX;
            if (Ba.BanksInGroup == 1)
            {   // 0 stands for CitNo... CitNo>0 when calling from Form18 (Cit View)
                string RunningGroup = "ITMX-FT";
                NForm200ITMX = new Form200ITMX(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup);
                NForm200ITMX.ShowDialog(); ;
            }

        }

        // Dispute Pre Investigation 
        private void button67_Click(object sender, EventArgs e)
        {

            Form80bΙΤΜΧ NForm80bΙΤΜΧ;

            string WFunction = "DisputeInvestigation";

            NForm80bΙΤΜΧ = new Form80bΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, "", 0, 0, WFunction);
            //NForm80bΙΤΜΧ.FormClosed += NForm80b_FormClosed;
            NForm80bΙΤΜΧ.ShowDialog();
        }

        // DISPUTE REGISTRATION  
        private void button68_Click(object sender, EventArgs e)
        {
            Form5ITMX NForm5ITMX;
            int From = 1;
            string Origin = WBankId;

            NForm5ITMX = new Form5ITMX(WSignedId, WSignRecordNo, WOperator, "", 0, 0, 0, "", From, Origin);
            NForm5ITMX.ShowDialog(); ;
        }

        // MANAGE DISPUTES 
        private void button69_Click(object sender, EventArgs e)
        {
            string CustomerUniqueId = "";
            NForm3ITMX = new Form3_ITMX(WSignedId, WSignRecordNo, WOperator, CustomerUniqueId, "UPDATE");
            NForm3ITMX.FormClosed += NForm3ITMX_FormClosed;
            NForm3ITMX.ShowDialog();
        }

        void NForm3ITMX_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormITMXBANKS_Load(this, new EventArgs());
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
            FormITMXBANKS_Load(this, new EventArgs());
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
        //
        // SHOW MY ATMs 
        //
        private void button15_Click(object sender, EventArgs e)
        {
            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS");
            //    return;
            //}

            WAction = 1; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator, "", WAction);
            NForm47.ShowDialog(); ;

        }

        //
        // ERRORS MANAGEMENT 
        // 

        private void button25_Click(object sender, EventArgs e)
        {
            if (WOperator == "ITMX")
            {
                MessageBox.Show("Not available for this DEMO");
                return;
            }
            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS");
            //    return;
            //}


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
        //bool FromRefreshTestingData;
        private void button22_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
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
            FormITMXBANKS_Load(this, new EventArgs());

            MessageBox.Show("Testing Data have been initialised");

        }
        //
        // THIS GOES TO ATMS IN NEED
        //
        private void button60_Click(object sender, EventArgs e)
        {
            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS - Sec Level 2, 3 and 4");
            //    return;
            //}

            Form50b NForm50b;
            string Function = "ATMsInNeed";
            NForm50b = new Form50b(WSignedId, WSignRecordNo, WOperator, Function);
            NForm50b.ShowDialog();
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

            NForm63 = new Form63(WSignedId, WSignRecordNo, WOperator);
            NForm63.FormClosed += NForm63_FormClosed;
            NForm63.ShowDialog(); ;
        }

        void NForm63_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormITMXBANKS_Load(this, new EventArgs());
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
            WAction = 8; // 
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }


        //PRESENT AND MANAGE DEPOSITS
        private void button34_Click(object sender, EventArgs e)
        {
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
            if (WOperator == "ITMX")
            {
                MessageBox.Show("Not available for this DEMO");
                return;
            }
            int Wf = 0;
            NForm9 = new Form9(WSignedId, WSignRecordNo, WSecLevel, WOperator, Wf);
            NForm9.ShowDialog(); ;

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
            WAction = 1;
            NForm18 = new Form18(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm18.ShowDialog(); ;
        }


        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckForMessages(); // Check for messages from controller

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            Ap.CheckOutstandingAuthorizationsStage1(WSignedId);
            // There is record for authorizer with Stage 1
            // Read and change stage at the same stage  
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

            // ****************************************
            // Records for Disputes for this entity 
            // ****************************************

            string SelectionCriteria = " BankId ='" + Us.BankId + "' AND CreatedByEntity = '"
                                         + WOperator + "' AND Active =1 AND ReadByBank = 0 ";

            Di.ReadDisputesAndUpdateStageIfFound(SelectionCriteria);
            if (Di.RecordFound == true)
            {
                Console.Beep(2000, 500);
                Console.Beep(3000, 1000);
                Console.Beep(2000, 500);

                Form2MessageBoxITMX NForm2MessageBoxITMX;

                string Message1;
                string Message2;

                Message1 = "YOU HAVE A MESSAGE FROM ITMX RELATED WITH A DISPUTE TO BE RESOLVED! ";
                Message2 = Di.DispComments;

                NForm2MessageBoxITMX = new Form2MessageBoxITMX(Message1, Message2);
                NForm2MessageBoxITMX.ShowDialog();

            }

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
            FormITMXBANKS_Load(this, new EventArgs());
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
            NForm53 = new Form53(WSignedId, WSignRecordNo, WOperator);
            NForm53.ShowDialog(); ;
        }
        //
        // System Performance
        //
        private void button74_Click(object sender, EventArgs e)
        {
            WAction = 1;

            NForm8 = new Form8(WSignedId, WOperator, "", WAction);
            NForm8.ShowDialog(); ;

        }

        // GO to Matching of outstanding postings 

        private void button37_Click(object sender, EventArgs e)
        {
            if (WOperator == "ITMX")
            {
                MessageBox.Show("Not available for this DEMO");
                return;
            }
            WAction = 1;

            NForm19 = new Form19a(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm19.FormClosed += NForm19_FormClosed;
            NForm19.ShowDialog(); ;
        }

        void NForm19_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormITMXBANKS_Load(this, new EventArgs());
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
        private void button80_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + "Authorisers = the Good , the Bad and the Neutral ");
        }
        // Future Module 
        private void button76_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + "Module that provides information to the Banks' corporate WareHouse. ");
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
        
        //Call Categories 
        private void button97_Click(object sender, EventArgs e)
        {
            Form80a NForm80aITMX;
            string WFunction = "View";

            string Category = "All";

            string WhatBank = WBankId;

            NForm80aITMX = new Form80a(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);
            NForm80aITMX.ShowDialog();
        }
     
        // SETTLEMENT 
        private void button99_Click(object sender, EventArgs e)
        {
            Form201ITMX NForm201ITMX;

            NForm201ITMX = new Form201ITMX(WSignedId, WSignRecordNo, WOperator);
            NForm201ITMX.ShowDialog();
        }

       

       
        //Settlement TWO 
        private void button41_Click(object sender, EventArgs e)
        {
            Form202ITMX NForm202ITMX;

            NForm202ITMX = new Form202ITMX(WSignedId, WSignRecordNo, WOperator);
            NForm202ITMX.ShowDialog();
        }

        //Configure CheckBox
        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                panel6.Show();
                panel6.BackColor = Color.SeaGreen;
                textBox2.BackColor = Color.SeaGreen;
                textBox3.BackColor = Color.SeaGreen;
                textBox4.BackColor = Color.SeaGreen;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                button20.Text = "Categories";
                button11.Text = "Matching Stages";
                button14.Text = "Rules for Actions";
            }
            else
            {
                // False
                if (checkBox3.Checked == false & checkBox4.Checked == false & checkBox5.Checked == false & checkBox6.Checked == false)
                {
                    panel6.Hide();
                }
            }
        }
        // Loading and Allocation 
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                panel6.Show();
                panel6.BackColor = Color.Magenta;
                textBox2.BackColor = Color.Magenta;
                textBox3.BackColor = Color.Magenta;
                textBox4.BackColor = Color.Magenta;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                button20.Text = "Files Loading";
                button11.Text = "Matching";
                button14.Text = "Work Allocation";
            }
            else
            {
                // False
                if (checkBox2.Checked == false & checkBox4.Checked == false & checkBox5.Checked == false & checkBox6.Checked == false)
                {
                    panel6.Hide();
                }
            }
        }
        // Reconciliation 
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                panel6.Show();
                panel6.BackColor = Color.IndianRed;
                textBox2.BackColor = Color.IndianRed;
                textBox3.BackColor = Color.IndianRed;
                textBox4.BackColor = Color.IndianRed;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                button20.Text = "Reconciliation";
                button11.Text = "Authorised Actions";
                button14.Text = "Posting of Txns";
            }
            else
            {
                // False
                if (checkBox2.Checked == false & checkBox3.Checked == false & checkBox5.Checked == false & checkBox6.Checked == false)
                {
                    panel6.Hide();
                }
            }
        }
        // View and Disputes 
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                panel6.Show();
                panel6.BackColor = Color.BlueViolet;
                textBox2.BackColor = Color.BlueViolet;
                textBox3.BackColor = Color.BlueViolet;
                textBox4.BackColor = Color.BlueViolet;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox6.Checked = false;
                button20.Text = "Investigation";
                button11.Text = "Registration";
                button14.Text = "Manage Disputes";
            }
            else
            {
                // False
                if (checkBox2.Checked == false & checkBox3.Checked == false & checkBox4.Checked == false & checkBox6.Checked == false)
                {
                    panel6.Hide();
                }
            }
        }
        //Other Banks 
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                panel6.Show();
                panel6.BackColor = Color.Blue;
                textBox2.BackColor = Color.Blue;
                textBox3.BackColor = Color.Blue;
                textBox4.BackColor = Color.Blue;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                button20.Text = "Recieve ITMX Disputes";
                button11.Text = "Manage Dispites";
                button14.Text = "Register Disputes";
            }
            else
            {
                // False
                if (checkBox2.Checked == false & checkBox3.Checked == false & checkBox4.Checked == false & checkBox5.Checked == false)
                {
                    panel6.Hide();
                }
            }
        }

      

    
        //View and Disputes 
        private void button97_MouseHover(object sender, EventArgs e)
        {
            checkBox5.Checked = true;
            checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox6.Checked = false;
        }
        // disputes pre 
        private void button67_MouseHover(object sender, EventArgs e)
        {
            checkBox5.Checked = true;
            checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox6.Checked = false;
        }

        private void button68_MouseHover(object sender, EventArgs e)
        {
            checkBox5.Checked = true;
            checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox6.Checked = false;
        }

        private void button69_MouseHover(object sender, EventArgs e)
        {
            checkBox5.Checked = true;
            checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox6.Checked = false;
        }

   
//History for loading for Banks 
        private void button45_Click(object sender, EventArgs e)
        {
            Form200cITMX NForm200cITMX;

            string RunningGroup = "ITMX-FT";
            NForm200cITMX = new Form200cITMX(WSignedId, WSignRecordNo, WSecLevel, WOperator, RunningGroup, WBankId);
            NForm200cITMX.ShowDialog(); ;
        }
//Cut off
        private void button42_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Details of this functionality will be discussed with ITMX."); 
        }
// Fees totals
        private void button48_Click(object sender, EventArgs e)
        {
            Form201ITMXFEES NForm201ITMXFEES;

            NForm201ITMXFEES = new Form201ITMXFEES(WSignedId, WSignRecordNo, WOperator);
            //NForm201bITMXFEES.FormClosed += NForm201bITMX_FormClosed;
            NForm201ITMXFEES.ShowDialog();
        }
//Fees Statement
        private void button49_Click(object sender, EventArgs e)
        {
            Form202ITMXFEES NForm202ITMXFEES;

            NForm202ITMXFEES = new Form202ITMXFEES(WSignedId, WSignRecordNo, WOperator);
            NForm202ITMXFEES.ShowDialog();
        }
// Fees View
        private void button50_Click(object sender, EventArgs e)
        {
            Form503Fees NForm503Fees;

            string Mode = "View";

            NForm503Fees = new Form503Fees(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm503Fees.ShowDialog();
        }
//System errors
        private void button95_Click_1(object sender, EventArgs e)
        {
            Form82 NForm82;
            NForm82 = new Form82(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm82.ShowDialog(); ;
        }
// Unmatched
        private void button38_Click(object sender, EventArgs e)
        {
            Form80dΙΤΜΧ NForm80dΙΤΜΧ;

            string Mode = "View";

            NForm80dΙΤΜΧ = new Form80dΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm80dΙΤΜΧ.ShowDialog();
        }
//MIS
        private void button77_Click(object sender, EventArgs e)
        {
            WAction = 11;

            Form21MISITMX NForm21MISITMX;

            NForm21MISITMX = new Form21MISITMX(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm21MISITMX.ShowDialog(); 
        }
    }
}

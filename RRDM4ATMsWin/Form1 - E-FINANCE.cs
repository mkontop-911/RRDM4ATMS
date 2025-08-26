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
    public partial class Form1_E_FINANCE : Form
    {
        public EventHandler LoggingOut;

        string MainFormId; 

       Form54 NForm54;
        Form55 NForm55;

        Form112 NForm112;

        string MsgFilter;

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        bool FromVisa;
        // NOTES 
        string Order = "Descending";
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        string WOrigin;
        string WSubSystem;

       string WJobCategory;

        string WBankId;
        string WBranch;
        string WCategory;

     //   int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form1_E_FINANCE(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

         
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.SignInApplication == "VISA SETTLEMENT")
            {
                WSubSystem = "VisaReconciliation";
                WJobCategory = "CardsSettlement";
                WOrigin = "Visa";
                FromVisa = true;
                labelStep1.Text = "Visa Settlement Main Menu"; 
                labelStep1.Text = "VISA SETTLEMENT";
                label15.Text = "RECONC. VISA"; 
                button14.Text = "VisaSe  Matching Categories ";
            }
            if (Usi.SignInApplication == "E_FINANCE RECONCILIATION")
            {
                WSubSystem = "E_FINANCE";
                WOrigin = "E_FINANCE";

                WJobCategory = "E_FINANCE Reconciliation";
                labelStep1.Text = "E_FINANCE Reconciliation";
                label15.Text = "RECONC.E-FINANCE";
                Us.ReadUsersRecord(WSignedId);
                WBranch = Us.Branch;

                MainFormId = "Form1 - E-FINANCE RECONCILIATION";

                if (WSecLevel == "04")
                {
                    labelStep1.Text = "E_FINANCE For Branch :.." + WBranch;
                    WCategory = "BDC" + WBranch;
                }

                if (WSecLevel == "05")
                {
                    Us.ReadUsersRecord(WSignedId);

                    labelStep1.Text = "E_FINANCE For Central Operation";
                    WCategory = "ALL";
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

                // NOSTRO_ADMIN
                foreach (Control c in E_FINANCE_ADMIN.Controls)
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



                // Reconc_Visa
                foreach (Control c in Reconc_E_FINANCE.Controls)
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
                foreach (Control c in Monistoring_E_FINANCE.Controls)
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
                foreach (Control c in Authorisation_E_FINANCE.Controls)
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



            toolTipButtons.SetToolTip(label4, "MONITORING" + Environment.NewLine
                                         + "Central controller monitors all ATMs by area."
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

            //**************************************************************************
            //END
            //**************************************************************************

            toolTipHeadings.ShowAlways = true;

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;

                //Trans to be posted  


                //Today's operation Status 
                toolTipButtons.SetToolTip(button12, "View current Replenishment and Reconciliation status." + Environment.NewLine
                                             + "Drilling down facilities are available for more details."
                                             );


                //Today's reports  
                toolTipButtons.SetToolTip(button16, "View a list of all daily reports." + Environment.NewLine
                                             + "A number of other reports can be created on customer request."
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

            MessageBox.Show("Testing Data E-FINANCE have been initialised");

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

        void NForm78_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMainScreen_Load(this, new EventArgs());
        }


        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        // E_FIN CENTRAL 
        private void button12_Click(object sender, EventArgs e)
        {
          
            Form291NV_E_FIN_Central NForm291NV_E_FIN_Central;
           
            NForm291NV_E_FIN_Central = new Form291NV_E_FIN_Central(WSignedId, WSignRecordNo, WOperator);
            NForm291NV_E_FIN_Central.ShowDialog();
            
        }

        // Vostro Banks and Accounts 
        private void button18_Click(object sender, EventArgs e)
        {
            Form85_Visa NForm85_Visa;
            NForm85_Visa = new Form85_Visa(WSignedId, WSignRecordNo, WSecLevel, WOperator, WOrigin);
            NForm85_Visa.ShowDialog();
        }
        // Matching Pairs 
        private void button14_Click_1(object sender, EventArgs e)
        {
            Form503_Visa NForm503_Visa;
            int Mode = 5; // Only Nostro
            if (labelStep1.Text == "VISA SETTLEMENT")
            {
                Mode = 6; // Visa Mode 
            }

            NForm503_Visa = new Form503_Visa(WSignedId, WSignRecordNo, WOperator, WOrigin, Mode);
            NForm503_Visa.ShowDialog();
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

        // Matching For Cards OR E_FINANCE
        private void button41_Click(object sender, EventArgs e)
        {
            if (Usi.SignInApplication == "VISA SETTLEMENT")
            {
                WSubSystem = "CardsSettlement";
                WOrigin = "Visa";
                FromVisa = true;
            }
            if (Usi.SignInApplication == "E_FINANCE RECONCILIATION")
            {
                WSubSystem = "E_FINANCE";
                WOrigin = "E_FINANCE";

                if (WSecLevel == "04" || WSecLevel == "05")
                {
                    // OK... Access is allowed
                }
                else
                {
                    MessageBox.Show("ACCESS NOT ALLOWED");
                    return;
                }
            }

            Form291NVMakeSystemMatching_Cards NForm291NVMakeSystemMatching_Cards;

            NForm291NVMakeSystemMatching_Cards = new Form291NVMakeSystemMatching_Cards(WSignedId, WSignRecordNo, WOperator, WSubSystem);

            NForm291NVMakeSystemMatching_Cards.ShowDialog();
        }

        // Mnaual Operation for Cards 
        private void button43_Click(object sender, EventArgs e)
        {
            Form80aVisa_2 NForm80aVisa_2;

            string WFunction = "";
            string Category = "";

            if (WSubSystem == "VisaReconciliation")
            {
                WFunction = "ReconcVisa";
                Category = "All";
            }
            if (WSubSystem == "E_FINANCE")
            {
                WFunction = "Reconc_E_FIN";
                Category = "All";
                if (WSecLevel == "04" || WSecLevel == "05")
                {
                    // OK... Access is allowed
                }
                else
                {
                    MessageBox.Show("ACCESS NOT ALLOWED");
                    return;
                }
                if (WSecLevel == "04")
                {
                    Category = WCategory;
                }
            }

            NForm80aVisa_2 = new Form80aVisa_2(WSignedId, WSignRecordNo, WOperator, WSubSystem, WFunction, Category, WBranch);
            NForm80aVisa_2.ShowDialog();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            Form80aVisa_2 NForm80aVisa_2;

            string WFunction = "";
            string Category = "";

            if (WSubSystem == "VisaReconciliation")
            {
                WFunction = "Matched";
                Category = "All";
            }
            if (WSubSystem == "E_FINANCE")
            {
                WFunction = "Matched";
                Category = "All";
                if (WSecLevel == "04" || WSecLevel == "05")
                {
                    // OK... Access is allowed
                }
                else
                {
                    MessageBox.Show("ACCESS NOT ALLOWED");
                    return;
                }
                if (WSecLevel == "04")
                {
                    Category = WCategory;
                }
            }

            NForm80aVisa_2 = new Form80aVisa_2(WSignedId, WSignRecordNo, WOperator, WSubSystem, WFunction, Category, WBranch);
            NForm80aVisa_2.ShowDialog();


        }
        // To Be Confirmed 
        private void button45_Click(object sender, EventArgs e)
        {
            Form80aVisa_2 NForm80aVisa_2;

            string WFunction = "";
            string Category = "";

            if (WSubSystem == "VisaReconciliation")
            {
                WFunction = "Confirm Entries";
                Category = "All";
            }

            if (WSubSystem == "E_FINANCE")
            {
                WFunction = "Confirm Entries";
                Category = "All";
                if (WSecLevel == "04" || WSecLevel == "05")
                {
                    // OK... Access is allowed
                }
                else
                {
                    MessageBox.Show("ACCESS NOT ALLOWED");
                    return;
                }
                if (WSecLevel == "04")
                {
                    Category = WCategory;
                }
            }

            NForm80aVisa_2 = new Form80aVisa_2(WSignedId, WSignRecordNo, WOperator, WSubSystem, WFunction, Category, WBranch);
            NForm80aVisa_2.ShowDialog();

            //Form80aVisa NForm80aVisa;

            //string WFunction = "Confirm Entries";

            //string Category = "All";

            //NForm80aVisa = new Form80aVisa(WSignedId, WSignRecordNo, WOperator, WFunction, Category);
            //NForm80aVisa.ShowDialog();
        }
        // Define Access rights
        private void buttonDefineAccess_Click(object sender, EventArgs e)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMUsersAccessRights Ur = new RRDMUsersAccessRights();
            string WSelectionCriteria;

            //string MainFormId = "Form1 - E-FINANCE RECONCILIATION";

            //Clear Table 
            Tr.DeleteReport57(WSignedId);

            Ur.IsUpdated = false;

            Ur.UpdateUsersAccessRightsInitialiseWithIsUpdated(MainFormId);
            // E_FINANCE_ADMIN
            foreach (Control c in E_FINANCE_ADMIN.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = E_FINANCE_ADMIN.Name;
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

            // Reconc_E_FINANCE        
            foreach (Control c in Reconc_E_FINANCE.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Reconc_E_FINANCE.Name;
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

            // Monistoring_E_FINANCE       
            foreach (Control c in Monistoring_E_FINANCE.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Monistoring_E_FINANCE.Name;
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



            // Authorisation_E_FINANCE        
            foreach (Control c in Authorisation_E_FINANCE.Controls)
            {
                if (c is Button)
                {
                    Tr.MainFormId = MainFormId;
                    Tr.PanelName = Authorisation_E_FINANCE.Name;
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

            string P1 = "Buttons DETAILS For E-Finance ";

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
            Form502AccessRights_E_FINANCE NForm502AccessRights_E_FINANCE;

            int Mode = 2; // View

            NForm502AccessRights_E_FINANCE = new Form502AccessRights_E_FINANCE(WSignedId, WSignRecordNo, WOperator, Mode);
            NForm502AccessRights_E_FINANCE.ShowDialog();

          
        }
        // loading code for E_Finance
        private void button_E_Finance_Click(object sender, EventArgs e)
        {
            // READ AN rtf file and turn it to txt
            //string path = @"C:\KONTO\E-Finance 25-09-2018 E-finance (3).rtf";
            //System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
            //string rtfText = System.IO.File.ReadAllText(path);
            //rtBox.Rtf = rtfText;
            //string plainText = rtBox.Text;
            //// Console.WriteLine(plainText);

            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"C:\KONTO\Output_E-Finance 25-09-2018_13.txt"))
            //{
            //    file.WriteLine(plainText);
            //    file.Close();
            //}
            RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();
            int counterTxn = 0;
            int counterLines = 0;
            string line;
            string OldLine = "";

            string Tk3, Tk4, Tk5, Tk6, Tk7;
            Sec.InitialiseWordToTable(WOperator);
            // Read the file and display it line by line.  
            System.IO.StreamReader file2 =
                new System.IO.StreamReader(@"C:\KONTO\Output_E-Finance 25-09-2018_13.txt");
            while ((line = file2.ReadLine()) != null)
            {
                counterLines++; 
                if (line.Length > 5)
                {
                    Tk3 = Tk4 = Tk5 = Tk6 = Tk7 = ""; 
                    string FirstFour = line.Substring(2, 4);
                    if (FirstFour == "0.00" & line.Length > 50)
                    {
                        int K = 0; 
                        //System.Console.WriteLine(line);

                        string myString = line; 
                        string[] subStrings = myString.Split();

                        foreach (string str in subStrings)
                        {
                            if (str != "")
                            {
                                //Console.WriteLine(str);
                                K++;
                                if (K == 3) Tk3 = str;
                                if (K == 4) Tk4 = str;
                                if (K == 5) Tk5 = str;
                                if (K == 6) Tk6 = str;
                                if (K == 7) Tk7 = str;
                               
                            }
                        }
                        Sec.LoadRowToWordTable(Tk3, Tk4, Tk5, Tk6, Tk7);
                             
                        counterTxn++;
                    }
                    else
                    {
                        //if (line.Length > 5)
                        //{
                        //    if (line.Contains("جنيه مصري") & line.Contains("العملة"))
                        //    {
                        //        Console.WriteLine(counterLines);
                        //        Console.WriteLine(OldLine);
                        //        Console.WriteLine(line);

                        //        string myString = OldLine;
                        //        string[] subStrings = myString.Split();

                        //        foreach (string str in subStrings)
                        //        {
                        //            // EGP
                        //            //جنيه مصري
                        //            if (str != "")
                        //            {
                        //              //  MessageBox.Show(str);
                        //                    Console.WriteLine(str);
                        //            }
                        //        }

                        //        //  Console.WriteLine(temp);
                        //    }
                        //}
                        //OldLine = line;
                        //continue;
                       
                        ////سلمى عبد المنعم
                    }

                } 
               
            }

            file2.Close();
            System.Console.WriteLine("There were {0} lines.", counterTxn);
            // Suspend the screen.  
            System.Console.ReadLine();

            DataTable ViewTable = Sec.TableTxnsFromWord;

            Form291NV_E_FIN_Word_Table NForm291NV_E_FIN_Word_Table;
            NForm291NV_E_FIN_Word_Table = new Form291NV_E_FIN_Word_Table(WSignedId, WSignRecordNo, WOperator, ViewTable, "E-FIN-Transactions");

            NForm291NV_E_FIN_Word_Table.ShowDialog();

            // Manipulate a line 

            //string myString = "2,160.00	201899000157346	25/09/2018";
            //string myString = "0.00	 0.00	 16,797.88	201899000157584	25/09/2018 11:43 	114134416123793";
            //string[] subStrings = myString.Split();

            //foreach (string str in subStrings)
            //{
            //    if (str != "")
            //    {
            //        Console.WriteLine(str);
            //    }
            //}

            // Create a working table 
        }
    }
}

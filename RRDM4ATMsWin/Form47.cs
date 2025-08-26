using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using RRDM4ATMs;
// Alecos
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace RRDM4ATMsWin
{
    public partial class Form47 : Form
    {

        Form49 NForm49; // ERRORS MANAGEMENT STRING 

        Form48 NForm48; // Repl Cycles 

        Form42 NForm42; // Repl Actions 

        Form24 NForm24; // Errors

        Form78 NForm78; // Transactions to be posted outstanding 

        string filter;

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMTempAtmsLocation Tl = new RRDMTempAtmsLocation();

        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

        RRDMJTMQueue Jq = new RRDMJTMQueue();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        DateTime FutureDate = new DateTime(2050, 11, 21);

        DateTime NullDate = new DateTime(1901, 01, 01);

        string WAtmNo;
        int WSesNo;
        int WRow;
        int scrollPosition;
        string OnlineWithJornal;

        bool Recon_Equal_Repl;

        string WBankId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId;
        int WAction;

        public Form47(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId;
            WAction = InAction;  // If 1 is normal just watching - 
                                 // If 2 then is for errors management 
                                 // If 3 it comes from CIT Provider Form. 
                                 // if 4 shows all ATMs by Maker
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            //string TestingDate = Gp.OccuranceNm;
            //if (TestingDate == "YES")
            //    labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            //else
            labelToday.Text = DateTime.Now.ToShortDateString();

            label14.Text = WOperator;
            pictureBox1.BackgroundImage = appResImg.logo2;

            ParId = "268";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            OnlineWithJornal = Gp.OccuranceNm;

            // Check if Replenishment = Reconciliation 
            // Centralised Reconciliation
            ParId = "939";
            OccurId = "1";
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

        }

        // ON LOAD DO 
        private void Form47_Load(object sender, EventArgs e)
        {
            if (WAction == 1) textBoxMsgBoard.Text = "Current Atms Status. You can also go to current and historical information of Repl Cycles";
            if (WAction == 2)
            {
                textBoxMsgBoard.Text = "Go to next to view and make changes on errors";
                labelStep1.Text = " Errors Management Workflow";
                buttonNext.Visible = true;
            }
            if (WAction == 3) textBoxMsgBoard.Text = "This ATM is Replenished by CIT provider";

            if (WAction == 2)
            {
                label36.Hide();
                label45.Hide();
                textBoxOutstandingErrors.Hide();
                textBoxInProcessForAction.Hide();
                button10.Hide();
                button11.Hide();
                label23.Hide();
                panel4.Hide();
            }

            if (WAction == 1 || WAction == 3)
            {
                if (WAction == 3)
                {
                    labelStep1.Text = " Atms For CIT Provider : " + WCitId;

                    label2.Text = "ATMs BASIC INFORMATION ";
                }
                label12.Hide();
                panel5.Hide();
            }
            //-----------------ACCESS CONTROL TO WHAT ATMS TO SEE---------------//
            string AtmNo = "";
            string FromFunction = "";

            if (WAction == 1 || WAction == 2)
            {
                FromFunction = "General";
            }
            if (WAction == 3)
            {
                FromFunction = "FromCit";
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);


            if (Usi.SecLevel == "02")
            {
                // Branch Officer
                // ACCESS Only your ATMs
            }
            if (Usi.SecLevel == "04")
            {
                // Reconciliator Officer
                // Or Controller
                // ACCESS All ATMs
            }
            if (Usi.SecLevel == "03")
            {
                // Reconciliator Officer
                // Or Controller
                // ACCESS All ATMs
            }

            // Load groups of ATMs this user is the owner

            Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WSignedId,"" ,WAction);

            if (Ua.UserGroups_ToAtms_Table.Rows.Count > 0)
            {
                dataGridViewMyATMS.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;
                ShowGrid_ATMs_1();
            }
            else
            {
                bool WAuthoriser = false;
                RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

                Uar.ReadUsersVsApplicationsVsRolesByUser(WSignedId);

                if (Uar.Authoriser == true)
                {
                    WAuthoriser = true;
                }
                else
                {
                    WAuthoriser = false;
                }
                if (WAuthoriser == false)
                {
                    MessageBox.Show("NO ATMS For this User ");
                }
                
            }

        }

        // A ROW WAS CHOSEN FOR FURHER INFORMATION 
        //
        // ON ROW ENTER for Grid 
        private void dataGridViewMyATMS_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            //if (Recon_Equal_Repl == true & Usi.SecLevel == "03")
            //{
            WAtmNo = (string)rowSelected.Cells[2].Value;

            //WAction = 1; // Simulation that this came from branch 
            //}
            //else
            //{
            //    WAtmNo = (string)rowSelected.Cells[0].Value;
            //}

            Ta.FindIfReplCycleExist(WAtmNo);
            if (Ta.RecordFound == true)
            {
                dateTimePicker4.Value = Ta.SesDtTimeStart;
            }
            else
            {
                dateTimePicker4.Value = DateTime.Now;
            }

            dateTimePicker5.Value = DateTime.Now;

            //UpDate Atm with latest status of transactions from Journal 
            if (OnlineWithJornal == "YES")
            {
                Aj.UpdateLatestEjStatusForSpecificAtm(WSignedId, WSignRecordNo, WOperator, WAtmNo);
            }

            label26.Text = "CURRENT INFO FOR ATM : " + WAtmNo;

            Ac.ReadAtm(WAtmNo);

            Am.ReadAtmsMainSpecific(WAtmNo);
            WSesNo = Am.CurrentSesNo;

            // Set up the Bank for this ATM
            WBankId = Am.Operator;

            textBoxBank.Text = WBankId;

            if (Am.ProcessMode == -2)
            {
                if (Am.ProcessMode == -2)
                {
                    label26.Hide();
                    panel8.Hide();

                    label23.Hide();
                    panel4.Hide();

                    //MessageBox.Show("This Atm is not active yet");

                }
                //return;
            }
            else
            {
                label26.Show();
                panel8.Show();
                if (WAction != 2)
                {
                    label23.Show();
                    panel4.Show();
                }

            }

            textBoxBranch.Text = Ac.BranchName;

            if (WAction == 3) // This is coming from a CIT company 
            {
                Us.ReadUsersRecord(WCitId);
                textBoxOwnerUser.Text = Us.UserId;
                textBoxName.Text = Us.UserName;
                textBoxEmail.Text = Us.email;
                textBoxMobile.Text = Us.MobileNo;

            }
            else
            {
                Ua.FindUserForRepl(WAtmNo, 0);
                if (Us.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ua.UserId); // Get Info for User 

                }
                else
                {
                    Us.ReadUsersRecord(WSignedId);
                    Us.RecordFound = false; // Initialise Record Found 
                }

                textBoxOwnerUser.Text = Us.UserId;
                textBoxName.Text = Us.UserName;
                textBoxEmail.Text = Us.email;
                textBoxMobile.Text = Us.MobileNo;

            }

            textBoxReplCycleNo.Text = Am.CurrentSesNo.ToString();
            textBoxLastReplDt.Text = Am.LastReplDt.ToString();
            textBoxNxtReplDt.Text = Am.NextReplDt.ToString();
            textBoxCassettesAmnt.Text = Am.CurrCassettes.ToString("#,##0.00");
            textBoxDepositedAmnt.Text = Am.CurrentDeposits.ToString("#,##0.00");

            Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

            textBoxLastJournal.Text = Jd.FileParseEnd.ToString();

            textBoxLastReconcDt.Text = Am.ReconcDt.ToString();

            if (Am.GL_ReconcDiff == true)
            {
                textBoxReconcDiff.Text = "YES";
            }
            else
            {
                textBoxReconcDiff.Text = "NO";
            }

            textBoxCurrency.Text = Am.GL_CurrNm1;
            textBoxAmountInDiff.Text = Am.GL_DiffCurr1.ToString("#,##0.00");
            textBoxSessionsInDiff.Text = Am.SessionsInDiff.ToString();
            textBoxOutstandingErrors.Text = Am.ErrOutstanding.ToString();

            Ec.ReadAllErrorsTableForCounters(WOperator, "", WAtmNo, WSesNo, "All");

            textBoxInProcessForAction.Text = Ec.ErrUnderAction.ToString();

            if (Am.ErrOutstanding > 0 & WAction != 2)
            {
                button10.Show();
            }
            else
            {
                button10.Hide();
            }

            if (Ec.ErrUnderAction > 0)
            {
                button11.Show();
            }
            else
            {
                button11.Hide();
            }
            WSesNo = Am.CurrentSesNo;

            if (Am.ProcessMode == -1)
            {
                textBoxStatus.Text = "Atm is currently serving customers";
                if (WAtmNo == "AB104") textBoxStatus.Text = "Atm is currently serving customers." + " Examine if Replenishment or reconciliation is needed";
            }
            if (Am.ProcessMode == 0)
            {
                textBoxStatus.Text = "Atm is currently ready for replenishment";
            }
            if (Am.ProcessMode == 1)
            {
                textBoxStatus.Text = "Atm has been replenished";
            }
            if (Am.ProcessMode == 2)
            {
                textBoxStatus.Text = "Atm has been fully reconciled";
            }
            if (Am.ProcessMode == 3)
            {
                textBoxStatus.Text = "Atm has NOT been fully reconciled";
            }

            if (WAction == 2)
            {
                // READ ALL ERRORS AND SET COUNTER 

                Ec.ReadAllErrorsTableForCounters(WBankId, "", WAtmNo, WSesNo, "All");

                textBox18.Text = Ec.NumOfErrors.ToString();
                textBox17.Text = Ec.ErrUnderAction.ToString();
                textBox5.Text = Ec.ErrUnderManualAction.ToString();

            }
        }



        // Show Grid 1
        private void ShowGrid_ATMs_1()
        {

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("You are not the owner of any ATM.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }
            if (WAction != 4)
                this.dataGridViewMyATMS.Sort(dataGridViewMyATMS.Columns["InNeed"], System.ComponentModel.ListSortDirection.Descending);

            dataGridViewMyATMS.Columns[0].Width = 90; // User id 
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewMyATMS.Columns[0].Visible = true;

            dataGridViewMyATMS.Columns[1].Width = 60; // InNeed
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewMyATMS.Columns[2].Width = 90; // ATM No
            dataGridViewMyATMS.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[3].Width = 140; // AtmName
            dataGridViewMyATMS.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[4].Width = 90; // Repl Pending
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[5].Width = 90; // Repl Cycle
            dataGridViewMyATMS.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridViewMyATMS.Columns[6].Width = 120; // RespBranch
            dataGridViewMyATMS.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewMyATMS.Columns[7].Width = 150; // Branch Name
            dataGridViewMyATMS.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            foreach (DataGridViewRow row in dataGridViewMyATMS.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                bool WInNeed = (bool)row.Cells[1].Value;

                if (WInNeed == true)
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }


        }

        // Show Grid 2
        //private void ShowGrid_ATMs_2()
        //{

        //    if (dataGridViewMyATMS.Rows.Count == 0)
        //    {
        //        Form2 MessageForm = new Form2("You are not the owner of any ATM.");
        //        MessageForm.ShowDialog();

        //        this.Dispose();
        //        return;
        //    }
        //    //TableATMsMainSelected.Columns.Add("ATMNo", typeof(string));
        //    //TableATMsMainSelected.Columns.Add("ReplCycle", typeof(string));

        //    //TableATMsMainSelected.Columns.Add("ATMName", typeof(string));
        //    //TableATMsMainSelected.Columns.Add("RespBranch", typeof(string));
        //    //TableATMsMainSelected.Columns.Add("UserId", typeof(string));

        //    dataGridViewMyATMS.Columns[0].Width = 90; // ATM No
        //    dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //    //dataGridViewMyATMS.Columns[0].Visible = false;

        //    dataGridViewMyATMS.Columns[1].Width = 90; // ReplCycle
        //    dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        //    dataGridViewMyATMS.Columns[2].Width = 140; // AtmName

        //    dataGridViewMyATMS.Columns[3].Width = 120; // RespBranch

        //    dataGridViewMyATMS.Columns[4].Width = 150; // UserId 

        //    dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
        //    dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        //}

        // AN ATM WAS ENTERED 
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))

            {
                MessageBox.Show("Enter An ATM number Please");
                return;
            }
            else
            {
                WAtmNo = textBox1.Text;
            }

           

            if (RRDMInputValidationRoutines.IsAlfaNumeric(WAtmNo))
            {
                //   No Problem 
            }
            else
            {
                MessageBox.Show("invalid characters in ATM Number");
                return;
            }

            bool WAuthoriser = false;
            RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

            Uar.ReadUsersVsApplicationsVsRolesByUser(WSignedId);

            if (Uar.Authoriser == true)
            {
                WAuthoriser = true;
            }
            else
            {
                WAuthoriser = false;
            }


            // See if this ATM belongs to the user 

            if (WAuthoriser == false)
            {
                RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

                Ra.CheckRightToAccessAtm(WSignedId, WAtmNo);

                if (Ra.RecordFound == false)
                {
                    MessageBox.Show(" You are not authorised to access this ATM ");
                    return;
                }
                string FromFunction = "General";
                string WCitId = "";

                Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WSignedId, WAtmNo, WAction);

                if (Ua.UserGroups_ToAtms_Table.Rows.Count > 0)
                {
                    dataGridViewMyATMS.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;
                    ShowGrid_ATMs_1();
                }
                else
                {
                    MessageBox.Show("NO ATMS For this User ");
                }
            }
            else
            {
                // THIS AN AUTHORISER 

                Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WSignedId, WAtmNo, 5);

                if (Ua.UserGroups_ToAtms_Table.Rows.Count > 0)
                {
                    dataGridViewMyATMS.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;
                    ShowGrid_ATMs_1();
                }
                else
                {
                    MessageBox.Show("NO ATMS For this User ");
                }
            }

            

            //// Create table with the ATMs this user can access
            //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, WAtmNo, FromFunction, WCitId);

            //dataGridViewMyATMS.DataSource = Am.TableATMsMainSelected.DefaultView;

            //if (dataGridViewMyATMS.Rows.Count == 0)
            //{
            //    Form2 MessageForm = new Form2("You are not the owner of any ATM.");
            //    MessageForm.ShowDialog();

            //    this.Dispose();
            //    return;
            //}

            //dataGridViewMyATMS.Columns[0].Width = 70; // AtmNo
            //dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridViewMyATMS.Columns[1].Width = 70; // ReplCycle
            //dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridViewMyATMS.Columns[2].Width = 120; // AtmName

            //dataGridViewMyATMS.Columns[3].Width = 130; // RespBranch

            //dataGridViewMyATMS.Columns[4].Width = 70; // Auth User 

            //dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }


        // SHOW ERRORS FOR THIS ATM FOR THEIR MANAGEMENT 
        // Next for errors workflow 
        private void buttonNext_Click(object sender, EventArgs e)
        {


            NForm49 = new Form49(WOperator, WSignedId, WSignRecordNo, WBankId, WAtmNo);

            NForm49.FormClosed += NForm49_FormClosed;
            NForm49.ShowDialog();

        }

        void NForm49_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridViewMyATMS.SelectedRows[0].Index;
            scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form47_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // Refresh - Needed after a single ATM has been chosen
        private void button5_Click(object sender, EventArgs e)
        {

            if (WAction == 1 || WAction == 2)
            {
                {
                    filter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'";
                }
                if (WAction == 3)
                {
                    filter = "Operator ='" + WOperator + "' AND CitId ='" + WCitId + "'";
                }

                //atmsMainBindingSource.Filter = filter;
                //this.atmsMainTableAdapter.Fill(this.aTMSDataSet28.AtmsMain);

                //  DataGrid 

                Am.ReadAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, "");

                dataGridViewMyATMS.DataSource = Am.TableATMsMainSelected.DefaultView;

            }
        }
        // Show ATMS LOCATION 

        // ATM LOCATION 
        private void button9_Click(object sender, EventArgs e)
        {

            string SeqNoURL = ConfigurationManager.AppSettings["RRDMMapsGeoQueryURL"];

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WAtmNo);
            int WGroup = 1;
            int TempMode = 2;
            Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode, WGroup);

            Tl.UserId = WSignedId;
            Tl.AtmNo = WAtmNo;

            Tl.BankId = WOperator;
            Tl.Mode = 2;

            Tl.GroupNo = WGroup;
            Tl.GroupDesc = "Show ATM " + WAtmNo;

            Tl.DtTmCreated = DateTime.Now;

            Tl.Street = Ac.Street;
            Tl.Town = Ac.Town;
            Tl.District = Ac.District;
            Tl.PostalCode = Ac.PostalCode;
            Tl.Country = Ac.Country;

            Tl.Latitude = Ac.Latitude;
            Tl.Longitude = Ac.Longitude;

            Tl.ColorId = "2";
            Tl.ColorDesc = "Normal Color";

            Tl.SeqNo = Tl.InsertTempAtmLocationRecord();


            // Format the URL with the query string (ATMSeqNo=#)

            string QueryURL = SeqNoURL + "?ATMSeqNo=" + Tl.SeqNo.ToString();

            // Invoke default browser
            ProcessStartInfo sInfo = new ProcessStartInfo(QueryURL);
            Process.Start(sInfo);
        }

        // GO TO SHOW REPLENISHMENT CYCLES 
        private void button6_Click(object sender, EventArgs e)
        {


            if (radioButtonReplCycles.Checked == true)
            {
                NForm48 = new Form48(WSignedId, WSignRecordNo, WBankId, WAtmNo, dateTimePicker4.Value, dateTimePicker5.Value);
                NForm48.FormClosed += NForm48_FormClosed;
                NForm48.ShowDialog();
            }

            if (radioButtonActions.Checked == true)
            {
                NForm42 = new Form42(WSignedId, WSignRecordNo, WBankId, WAtmNo, dateTimePicker4.Value, dateTimePicker5.Value);
                NForm42.FormClosed += NForm42_FormClosed;
                NForm42.ShowDialog();
            }

            if (radioButtonAccounts.Checked == true)
            {
                string WAccName = comboBox1.Text;
                string WAccCurr = comboBox2.Text;
                Form31 NForm31 = new Form31(WSignedId, WSignRecordNo, WOperator, WCitId, WAccName, WAccCurr, 4,
                         dateTimePicker4.Value, dateTimePicker5.Value, WAtmNo);
                NForm31.FormClosed += NForm31_FormClosed;
                NForm31.ShowDialog();
            }

            if (radioButtonRMCategCycles.Checked == true)
            {
                Form80a NForm80aITMX;
                string WFunction = "View";
                string Category = "All";

                string WhatBank = WBankId;

                NForm80aITMX = new Form80a(WSignedId, WSignRecordNo, WOperator, WFunction, Category, WhatBank);

                NForm80aITMX.ShowDialog();
            }
        }

        void NForm31_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridViewMyATMS.SelectedRows[0].Index;
            scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form47_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        void NForm42_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridViewMyATMS.SelectedRows[0].Index;
            scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form47_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        void NForm48_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRow = dataGridViewMyATMS.SelectedRows[0].Index;
            scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form47_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRow].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // ALL OUTSTANDING ATM ERRORS 
        private void button10_Click(object sender, EventArgs e)
        {
            bool Mode = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "'" + " AND OpenErr =1";

            NForm24 = new Form24(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo, "", Mode, SearchFilter);
            NForm24.ShowDialog();
        }
        //
        // show suspense transactions
        //
        private void button11_Click(object sender, EventArgs e)
        {
            int Mode = 3;
            NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator, WAtmNo, 0, 0, Mode);

            NForm78.ShowDialog();
        }

        // FINISH 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Show Accounts 
        private void radioButtonAccounts_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAccounts.Checked == true)
            {
                panelAccounts.Show();

                RRDMComboClass Cc = new RRDMComboClass();

                comboBox1.DataSource = Cc.GetAtmAccs(WOperator, WAtmNo);
                comboBox1.DisplayMember = "DisplayValue";

                comboBox2.DataSource = Cc.GetAtmAccsCurr(WOperator, WAtmNo);
                comboBox2.DisplayMember = "DisplayValue";
            }
            else
            {
                panelAccounts.Hide();
            }
        }
        //
        // Refresh EJournal 
        // 
        private void buttonRefreshEJ_Click(object sender, EventArgs e)
        {
            //System.Windows.Forms.ProgressBar progressbar1;
            //progressBar1.Minimum = 0;
            //progressBar1.Maximum = 2000;
            //for (int i = 0; i < progressBar1.Maximum; i++)
            //{
            //    progressBar1.Value = i;
            //}

            if (OnlineWithJornal == "NO")
            {
                MessageBox.Show("The System is not Online with Journals" + Environment.NewLine
                    + "You cannot Refresh the Journal");

                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings
              ["ATMSConnectionString"].ConnectionString;

            string RCT = "ATMS.[dbo].[Stp_Refresh_Testing_Data]";

            using (SqlConnection conn2 =
               new SqlConnection(connectionString))
                try
                {
                    conn2.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn2))
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
            // 
            //***********************************************************************
            //**************** USE TRANSACTION SCOPE
            //***********************************************************************

            // create a connection object
            using (var scope = new System.Transactions.TransactionScope())
                try
                {
                    // SUMMARY
                    // STEP_SET_UP
                    // E-Journal Loading Schedules are defined (Set up) 
                    //
                    // STEP_A
                    // During ATM Creation a record is created in [JTMIdentificationDetails]
                    // based on E-Journal Characteristics and by defining the loading schedule
                    // Next Loading Date is defined 
                    //
                    // STEP_B
                    // At intervals a service is searching for Next Loading Dates < less than current 
                    // If this condition applies then a Journal loading is needed.
                    // If a Journal loading is needed then a record is created in Queue [JTMQueue]
                    // The master record in [JTMIdentificationDetails] is updated with -1 
                    //
                    // STEP_C
                    // Alecos who continuouly reads queue reads record , 
                    // uploads journal, parse it 
                    // and upon finishing he updates [JTMIdentificationDetails] is updated with 0 
                    //
                    // STEP_D
                    // RRDM Continuously reads [JTMIdentificationDetails] to see if 0 
                    // This is done for 40 seconds and then time out occurs 
                    // IF Journal Updated and Parsed then 
                    // RRDM Calls to create a new Repl Cycle if needed and inserts transactions in Pool  
                    //
                    // STEP_E 
                    // Updates [JTMIdentificationDetails] 
                    // Updates next date E-Journal to be loaded ... based on Schedule 
                    // with 1 = loaded and updated

                    string WCommand;
                    string InMode;

                    int WPriority;

                    // InCommand : Fetch
                    //           : GETDEL
                    // InMode    : SingleAtm
                    //           : AllReadyForLoading

                    // Check that ATM has ResultCode = 1 = ATM is not under Journal process
                    Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);
                    if (Jd.RecordFound == true)
                    {
                        if (Jd.ResultCode != 1)
                        {
                            // Stop process
                            MessageBox.Show("The ATM is under Journal Process");
                            return;
                        }
                    }

                    //SINGLE ATM
                    WCommand = "FETCH";
                    InMode = "SingleAtm";

                    WPriority = 1; // Highest priority  

                    Jq.InsertRecordsInJTMQueue(WSignedId, WCommand, InMode, WPriority, WAtmNo);
                    if (Jq.ErrorFound == true)
                    {
                        return;
                    }

                    scope.Complete();
                }
                catch (Exception ex)
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
                finally
                {
                    scope.Dispose();
                }
            //
            // Check When Journal is read
            //
            try
            {
                // Follow when ejournal will be read

                bool EjournalLoaded = false;

                int MaxSecs = 40;

                int I = 0;

                while (EjournalLoaded == false & I <= MaxSecs)
                {

                    // Read 
                    Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

                    if (Jd.RecordFound)
                    {
                        if (Jd.ResultCode == -1)
                        {
                            Jq.ReadJTMQueueByMsgID(Jd.QueueRecId);

                            //    public const int Const_InQueue = 0;
                            //public const int Const_WorkInProgress = 1;
                            //public const int Const_TransferInProgress = 2;
                            //public const int Const_TransferFinished = 3;
                            //public const int Const_WaitingForParsing = 4;
                            //public const int Const_ParserInProgress = 5;
                            //public const int Const_ParserFinished = 6;
                            //public const int Const_Aborted = 98;
                            //public const int Const_Finished = 99;

                            // Not Updated yet 
                            Thread.Sleep(1000); // Wait one second 

                            I = I + 1;

                        }
                        else
                        {
                            if (Jd.ResultCode == 0)
                            {
                                // Journal Updated
                                EjournalLoaded = true;

                                //UpDate Atm with latest status of transactions from Journal 
                                Aj.UpdateLatestEjStatusForSpecificAtm(WSignedId, WSignRecordNo, WOperator, WAtmNo);

                                // UPDATE ATM JOURNAL DETAILS   
                                Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

                                // Calculate Next Loading date 
                                // Last loaded Date not available then insert current date 
                                //

                                Jd.LoadingCompleted = DateTime.Now;
                                Jd.ResultCode = 1;
                                Jd.ResultMessage = "Ready";

                                Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(WAtmNo);
                                if (Jd.ErrorFound == true)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                // Problem with reading journal 
                                MessageBox.Show("It was a problem reading Journal. Inform the controller");
                            }
                        }
                    }
                }

                if (EjournalLoaded == true)
                {
                    WRow = dataGridViewMyATMS.SelectedRows[0].Index;
                    scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;
                    // Load Grid 
                    Form47_Load(this, new EventArgs());

                    dataGridViewMyATMS.Rows[WRow].Selected = true;
                    dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                    dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

                    MessageBox.Show("Journal Loaded for ATM : " + WAtmNo);
                }

                if (I > MaxSecs)
                {
                    // Time out 
                    MessageBox.Show("TIME OUT. EJournal was not read within time limit of seconds : " + MaxSecs.ToString());
                    return;
                }

            }
            catch (Exception ex)
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
                Environment.Exit(0);
            }


        }
        //REFRESH 
        private void button5_Click_1(object sender, EventArgs e)
        {
            Form47_Load(this, new EventArgs());
        }
        // SHow UnMatched Txns 
        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string WCategoryId = "";
            int WRMCycle = 0;
            Form271ViewAtmUnmatched NForm271ViewAtmUnmatched;
            NForm271ViewAtmUnmatched = new Form271ViewAtmUnmatched(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, WAtmNo, 0);
            //    NForm271FastTrack.FormClosed += NForm271FastTrack_FormClosed;
            NForm271ViewAtmUnmatched.ShowDialog();
        }
    }

}




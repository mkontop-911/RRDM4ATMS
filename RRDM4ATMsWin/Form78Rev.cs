using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form78Rev : Form
    {
        /// <summary>
        /// MANAGE ALL TRANSACTIONS CREATED DURING RECONCILIATION PROCESS
        /// VOUCHERS ARE PRINTED FOR UPDATING BY CASHIER 
        /// </summary>
        /// 

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMBanks Ba = new RRDMBanks();

        //DateTime NullPastDate = new DateTime(1950, 11, 21); 

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WDtFrom;
        DateTime WDtTo;

        Form112 NForm112;

        bool WMatching;
        bool WReplenishment;
        bool WReconciliation;
        bool WSettlement;
        bool WFeesSettlement; 

        //bool ITMXUser;
        string WBankId;

        int WDisputeNo;
        int WDisputeTranNo;

        string WRMCateg;
        int WRMCategCycle;

        int WMaskRecordId;

        bool WithDate;
        string Gridfilter;

        //int WRow;
        string WLineAtmNo;
        int WLineSesNo;

        int WPostedNo;
        int ErrNo;

        //string WOriginNmInAuther;

        bool WRange;

        string WSignedId;
        int WSignRecordNo;

        string WOperator;

        string WAtmNo;
        int WSesNo;

        int WErrNo;
        int WMode;

        public Form78Rev(string InSignedId, int InSignRecordNo, string InOperator,
            string InAtmNo, int InSesNo, int InErrNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            WAtmNo = InAtmNo; // It is blank if comes from Form1
            WSesNo = InSesNo; // It is >0 From Form83 and Form116

            WErrNo = InErrNo; // It is > 0 THIS TRUE WHEN WE WANT TO SHOW Just this

            WMode = InMode; // If = 3 comes from My ATMs Form47 , if 2 from Form83 and Form116, if 1 from Form1

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            labelUser.Text = InSignedId;
            pictureBox1.BackgroundImage = appResImg.logo2;

            //Preparing Selection Panel 
            Gp.ParamId = "424"; // Origin  
            comboBoxOrigin.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxOrigin.DisplayMember = "DisplayValue";

            radioButton1.Checked = true;

            Us.ReadUsersRecord(WSignedId);
            WBankId = Us.BankId;
            //ITMXUser = false;
            if (Us.Operator == WBankId)
            {
                //ITMXUser = true;
            }
            else
            {
                //ITMXUser = false;

                labelStep1.Text = "Trans Already Posted For our Bank : " + WBankId;
            }

            radioButton2.Checked = true; 
        }

        private void Form78_Load(object sender, EventArgs e)
        {

            try
            {
               
                // ===========================================
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

                // SHOW GRID

                ShowGrid("");
            }
            catch (Exception ex)
            {

                string exception = ex.ToString();

                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }
        // This method loads and shows the Grid 
        public void ShowGrid(string InGridfilter)
        {

            try
            {

                if (InGridfilter == "")
                {
                    // =============================================================
                    // Make Grid ready for all allow trans for this User 

                    InGridfilter = "Operator ='" + WOperator + "'"
                         + " AND OpenRecord = 0 ";

                    WithDate = false;

            
                }

                buttonExpandGrid.Hide();

                // 
                //
                //  DataGrid LOADING
                //
                //

                if (WRange == false)
                {
                    Tp.ReadAllTransToBePostedAndFillTable(WSignedId, InGridfilter, DateTime.Today, WithDate);
                }
                else
                {
                    // Table Tc.TransToBePostedSelected was filled in Show Button 
                }

                textBox21.Text = Tp.TotalSelected.ToString();

                dataGridView1.DataSource = Tp.TransToBePostedDataTable.DefaultView;

                dataGridView1.Columns[0].Width = 70; // 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
   
                dataGridView1.Columns[1].Visible = false; // SELECT

                dataGridView1.Columns[2].Width = 180; // 
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[3].Width = 70; // 
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[4].Width = 70; //
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[5].Width = 80; //
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[6].Width = 60; //
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[7].Width = 150; //

                dataGridView1.Columns[8].Width = 100; // 

                dataGridView1.Columns[9].Width = 60; //

                dataGridView1.Columns[10].Width = 60; //
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);


                if (dataGridView1.Rows.Count == 0 & WRange == false)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No transactions to be posted");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }
                if (dataGridView1.Rows.Count == 0 & WRange == true)
                {
                    MessageBox.Show("No transactions for this selection");

                    buttonExpandGrid.Hide();
                    return;
                }

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

                WPostedNo = (int)rowSelected.Cells[0].Value;

                WMatching = false; WReconciliation = false;
                WReplenishment = false;

                Tp.ReadTransToBePostedSpecific(WPostedNo);

                WRMCateg = Tp.RMCateg;
                WRMCategCycle = Tp.RMCategCycle;

                WMaskRecordId = Tp.UniqueRecordId;

                WLineAtmNo = Tp.AtmNo;
                WLineSesNo = Tp.SesNo;

                if (WOperator == "ITMX")
                {
                    label1.Hide();
                    label15.Hide();
                    textBox1.Hide();
                    textBox16.Hide(); 
                }

                // "01" OurATMS-Matching And Reconciliation 
                // "02" BancNet Matching And Reconciliation                               
                // "03" OurATMS-Reconc (In relation to replenishement )
                // "04" OurATMS-Repl
                // "05" Settlement
                // "06" FeesSettlement 
                // "07" Disputes 
                // "08" Instructions to CIT
                // "09" Nostro Adjustments 

                // ALSO NOTE IN AUTHORISATION RECORD WE USE Origin AS 
                //WOrigin == "Dispute Action"
                //WOrigin == "Replenishment" 
                //WOrigin == "Reconciliation"
                //WOrigin == "ReconciliationCat" 
                //WOrigin == "SettlementAuth" 
                //WOrigin == "SettlementFeesAuth" 
                //WOrigin == "ForceMatchingCat"
                //WOrigin == "ConfirmNostroMatching"

                WMatching = false;
                WReplenishment = false;
                WReconciliation = false;
                WSettlement = false;
                WFeesSettlement = false;

                if (Tp.OriginId == "01") // "01" OurATMS-Matching
                {
                    WMatching = true;
                    //WOriginNmInAuther = "ReconciliationCat";
                }
                if (Tp.OriginId == "02") // "02" BancNet and ITMX
                {
                    WMatching = true;
                    //WOriginNmInAuther = "ReconciliationCat";
                }

                if (Tp.OriginId == "04") // "04" OurATMS-Repl
                {
                    WReplenishment = true;
                    //WOriginNmInAuther = "Replenishment";
                }

                if (Tp.OriginId == "03") // "03" OurATMS-Reconc
                {
                    WReconciliation = true;
                    //WOriginNmInAuther = "ReconciliationBulk";
                }

                if (Tp.OriginId == "05") // "05" Settlement
                {
                    WSettlement = true;
                    //WOriginNmInAuther = "SettlementAuth";
                }

                if (Tp.OriginId == "06") // "06" Fees Settlement
                {
                    WFeesSettlement = true;
                    //WOriginNmInAuther = "SettlementFeesAuth";
                }

                if (Tp.OriginId == "07") // "07" Disputes 
                {
                    WDisputeNo = Tp.DisputeNo;
                    WDisputeTranNo = Tp.DispTranNo;
                    //WOriginNmInAuther = "Disputes";
                }

                if (Tp.OriginId != "07") // Not dispute 
                {
                    WDisputeNo = 0;
                    WDisputeTranNo = 0;
                }

                if (Tp.OriginId == "08") // S
                {
                    buttonAuthHistory.Hide();
                }
                else
                {
                    buttonAuthHistory.Show();
                }

                // SHOW ALL NEEDED DETAILS TO PAY ATTENTION ON
                //
                ErrNo = Tp.ErrNo;
                textBox1.Text = Tp.CardNo;

                textBox20.Text = Tp.OpenDate.ToString();

                if (Tp.AccNo == "Not Available")
                {
                    textBox4.ReadOnly = false;
                    textBox4.Text = "Input Account";

                    if (Tp.TransType == 11 || Tp.TransType == 12) textBox18.Text = "DR";
                    if (Tp.TransType == 21 || Tp.TransType == 22) textBox18.Text = "CR";

                    textBox7.ReadOnly = false;
                    textBox7.Text = "Input Account";

                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12) textBox19.Text = "DR";
                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22) textBox19.Text = "CR";
                }
                else
                {
                    textBox4.ReadOnly = true;
                    textBox4.Text = Tp.AccNo;

                    if (Tp.TransType == 11 || Tp.TransType == 12) textBox18.Text = "DR";
                    if (Tp.TransType == 21 || Tp.TransType == 22) textBox18.Text = "CR";

                    textBox7.ReadOnly = true;
                    textBox7.Text = Tp.AccNo2;

                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12) textBox19.Text = "DR";
                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22) textBox19.Text = "CR";
                }

                textBox2.Text = Tp.TransDesc;
                textBox3.Text = Tp.CurrDesc;
                textBox8.Text = Tp.TranAmount.ToString("#,##0.00");
                textBox10.Text = Tp.RemNo.ToString();
                textBox9.Text = Tp.RefNumb.ToString();

                Ec.ReadErrorsTableSpecific(ErrNo);

                textBox16.Text = Ec.ErrDesc;

                textBox17.Text = Tp.TransMsg;

                Gp.ReadParametersSpecificId(WOperator, "705", Tp.SystemTarget.ToString(), "", "");

                textBox15.Text = Gp.OccuranceNm;

                textBox14.Text = Tp.ActionBy.ToString();
                textBox13.Text = Tp.ActionCd2.ToString();
                if (Tp.ActionDate != NullPastDate) textBox12.Text = Tp.ActionDate.ToString();
                else textBox12.Text = "";

                if (Tp.ActionDate == NullPastDate)
                {
                    textBoxMsgBoard.Text = "Finalise action for this transaction";
                    // HIDE ACTION Fields 
                    pictureBox2.Hide();
                    label17.Hide();
                    label13.Hide();
                    label12.Hide();
                    label11.Hide();
                    textBox14.Hide();
                    textBox13.Hide();
                    label3.Hide();
                    textBox12.Hide();
                }
                else
                {
                    pictureBox2.Show();
                    label17.Show();
                    label13.Show();
                    label12.Show();
                    label11.Show();
                    textBox14.Show();
                    textBox13.Show();
                    label3.Show();
                    textBox12.Show();

                    if (Tp.ActionCd2 == 1)
                    {
                        textBoxMsgBoard.Text = "Action was finalised for this transaction";
                        label3.Text = "Finalised";
                    }
                    if (Tp.ActionCd2 == 2)
                    {
                        textBoxMsgBoard.Text = "Action was rejected for this transaction";
                        label3.Text = "Rejected";
                    }
                    if (Tp.ActionCd2 == 3)
                    {
                        textBoxMsgBoard.Text = "Action was postponed for this transaction";
                        label3.Text = "Postponed";
                    }
                }

                Tp.ReadTransToBePostedSpecificByUniqueRecordIdForReversal(Tp.UniqueRecordId);
                if (Tp.IsReversal == true)
                {
                    labelReversal.Show();
                }
                else
                {
                    labelReversal.Hide();
                }

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }
      
        // Show DATA GRID AS PER SELECTION 
        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == true)
            {
                if (radioButtonCard.Checked == false & radioButtonAccNo.Checked == false & radioButtonTraceNo.Checked == false
                    & radioButtonAtmNo.Checked == false & radioButtonDispNo.Checked == false)
                {
                    MessageBox.Show(" Please select a radio button ");
                    return;
                }

                if (textBox11.Text == "")
                {
                    MessageBox.Show(" Please enter value for the selection made");
                    return;
                }
                else
                {
                    if (RRDMInputValidationRoutines.IsAlfaNumeric(textBox11.Text))
                    {
                        //   No Problem 
                    }
                    else
                    {
                        MessageBox.Show("invalid Input");
                        return;
                    }
                }
            }

            WDtFrom = dateTimePicker1.Value.Date;
            WDtTo = dateTimePicker2.Value.Date;
            WDtFrom = WDtFrom.AddDays(-1);
            WDtTo = WDtTo.AddDays(1);

            if (radioButton1.Checked == true) // Opened 
            {
            }
            if (radioButton2.Checked == true) // Closed 
            {
            }

            if (comboBoxOrigin.Text != "N/A") // Origin was selected
            {
            }

            if (checkBoxUnique.Checked == true) // Unique is chosen 
            {
            }

            if (radioButtonCard.Checked == true) // It is card 
            {
            }

            if (radioButtonAccNo.Checked == true) // It is Account No
            {
            }

            if (radioButtonTraceNo.Checked == true) // It is Trace No
            {
                // Turn textBox to Numeric 
            }

            if (radioButtonAtmNo.Checked == true) // It is ATM No
            {
            }

            if (radioButtonDispNo.Checked == true) // It is Dispute No
            {
                // Turn textBox to Numeric 
            }

            if (radioButton1.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == false) // Opened and all
            {
                Gridfilter = "Operator ='" + WOperator
                           + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton2.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == false) // Close and all
            {
                Gridfilter = "Operator ='" + WOperator
                           + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 0";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton1.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == true
                & radioButtonAtmNo.Checked == true) // ATM 
            {
                Gridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'"
                           + " AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1 AND AtmNo ='" + textBox11.Text + "'";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton2.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == true
             & radioButtonAtmNo.Checked == true) // ATM 
            {
                Gridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'"
                           + " AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 2 AND AtmNo ='" + textBox11.Text + "'";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            WRange = true;

            ShowGrid(Gridfilter);

            buttonExpandGrid.Show(); // This button goes to show grid in bigger form 

        }
     

       
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Authorisation history
        private void button3_Click(object sender, EventArgs e)
        {

            //OurATMs-Matching-102

            if (WReplenishment == true || WReconciliation == true)
            {
                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WLineAtmNo, WLineSesNo, "Replenishment");
                if (Ap.RecordFound == true)
                {
                }
                else
                {
                    MessageBox.Show("No authorisation history for this testing ATM!");
                    return;
                }

                NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WLineAtmNo, WLineSesNo, WDisputeNo, WDisputeTranNo, WRMCateg, WRMCategCycle);
                NForm112.ShowDialog();
            }
            if (WMatching == true)
            {
                //TEST   
                // This part needs redesign

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(Tp.RMCateg, Tp.RMCategCycle, "ReconciliationCat");
                if (Ap.RecordFound == true)
                {

                }
                else
                {
                    MessageBox.Show("No authorisation history for this testing ATM!");
                    return;
                }

                NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", 0, WDisputeNo, WDisputeTranNo, WRMCateg, WRMCategCycle);
                NForm112.ShowDialog();
            }

            if (WSettlement == true || WFeesSettlement == true)
            {
                //TEST   
                // This part needs redesign

                if (WSettlement == true) Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(Tp.RMCateg, Tp.RMCategCycle, "SettlementAuth");
                if (WFeesSettlement == true) Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(Tp.RMCateg, Tp.RMCategCycle, "SettlementFeesAuth");
                if (Ap.RecordFound == true)
                {

                }
                else
                {
                    MessageBox.Show("No authorisation history for this testing ATM!");
                    return;
                }

                NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", 0, WDisputeNo, WDisputeTranNo, WRMCateg, WRMCategCycle);
                NForm112.ShowDialog();
            }


            if (Tp.OriginId == "07") // "07" Disputes
            {
                //TEST   
                // This part needs redesign

                WLineAtmNo = "";
                WLineSesNo = 0;
                WDisputeNo = Tp.DisputeNo;
                WDisputeTranNo = Tp.DispTranNo;
            }
            else
            {
                WDisputeNo = 0;
                WDisputeTranNo = 0;
            }
        }
        // Chenge Radio button 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked == false)
            {
                radioButtonCard.Checked = false;
                radioButtonAccNo.Checked = false;
                radioButtonTraceNo.Checked = false;
                radioButtonAtmNo.Checked = false;
                radioButtonDispNo.Checked = false;
                textBox11.Text = "";
            }
            else
            {
            }
        }
        // Show Big Grid 
        Form78b NForm78b;
        private void button4_Click(object sender, EventArgs e)
        {

            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "SELECTED TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Tp.TransToBePostedDataTable, WHeader, "Form78");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                Gridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'" + " AND AuthUser ='" + WSignedId
                         + "' AND AtmDtTime  > @WDtFrom AND AtmDtTime <@WDtTo AND OpenRecord = 1 AND PostedNo =" + NForm78b.WPostedNo.ToString() + " ";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);

                WRange = true;

                ShowGrid(Gridfilter);

                buttonExpandGrid.Show(); // This button goes to show grid in bigger form 
            }
        }

        // EXPAND GRID
     
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string WHeader = "LIST OF TRANSACTIONS TO BE POSTED";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Tp.TransToBePostedDataTable, WHeader, "Form78");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }
        //
        // Show Posted 
        //
        private void buttonShowPosted_Click(object sender, EventArgs e)
        {
            int WFunction = 5; // Show ALl open ready txns for posting 
         
            Form31 NForm31 = new Form31(WSignedId, WSignRecordNo, WOperator, "", "", "", WFunction,
                     dateTimePicker1.Value, dateTimePicker2.Value, "");
           
            NForm31.ShowDialog();
        }
// Print Selection
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "Transactions To Be Reversed";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R61ATMS ReportATMS61 = new Form56R61ATMS(P1, P2, P3, P4, P5);
            ReportATMS61.Show();
        }
//
// Move to Reversal
//
        private void buttonMoveToReversal_Click(object sender, EventArgs e)
        {
            Form78Rev2 NForm78Rev2;

            NForm78Rev2 = new Form78Rev2(WSignedId, WSignRecordNo, WOperator, Tp.TransToBePostedDataTable, 251);
            //NForm78Rev2.FormClosed += NForm78Reverse_FormClosed;
            NForm78Rev2.ShowDialog();
        }
    }
}

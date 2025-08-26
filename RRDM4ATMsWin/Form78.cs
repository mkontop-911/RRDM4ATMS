using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form78 : Form
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

        int WRow;
        string WLineAtmNo;
        int WLineSesNo;

        int WPostedNo;
        int ErrNo;

        string WOriginNmInAuther;

        string W_Application;
        bool T24Version; 

        bool WRange;

        string WSignedId;
        int WSignRecordNo;

        string WOperator;

        string WAtmNo;
        int WSesNo;

        int WUniqueRecordId;
        int WMode;

        public Form78(string InSignedId, int InSignRecordNo, string InOperator,
            string InAtmNo, int InSesNo, int InUniqueRecordId, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            WAtmNo = InAtmNo; // It is blank if comes from Form1
            WSesNo = InSesNo; // It is >0 From Form83 and Form116

            WUniqueRecordId = InUniqueRecordId; // It is > 0 THIS TRUE WHEN WE WANT TO SHOW Just this

            WMode = InMode; // If = 3 comes from My ATMs Form47 , if 2 from Form83 and Form116, if 1 from Form1

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            labelUser.Text = InSignedId;
            pictureBox1.BackgroundImage = appResImg.logo2;

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
                        MessageBox.Show("Operation for EGATE to be checked. "); // 
                        return; 
                    }
                    //labelStep1.Text = "Controller's Menu-Mobile_" + WApplication;
                }
                else
                {
                    W_Application = "ATMs";
                    if (Usi.WFieldNumeric11 == 10)
                    {
                        T24Version = true;
                       // labelStep1.Text = "T24_MAIN MENU " + WApplication + " (" + WSignedId + ")";
                    }
                    else
                    {
                       // labelStep1.Text = "MAIN MENU " + WApplication + " (" + WSignedId + ")";
                    }

                }
            }

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
                if (T24Version == true)
                {
                    labelStep1.Text = "T24_Trans to be Posted For our Bank : " + WBankId;
                }
                else
                {
                    labelStep1.Text = "Trans to be Posted For our Bank : " + WBankId;
                }
                
            }
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
                //Am.ReadViewAtmsMainForAuthUserAndFillTable(WOperator, WSignedId, WSignRecordNo, AtmNo, FromFunction, WCitId);

                ////-----------------UPDATE LATEST TRANSACTIONS----------------------//
                //// Update latest transactions from Journal 
                //Aj.UpdateLatestEjStatusVersion2(WSignedId, WSignRecordNo, WOperator, Am.TableATMsMainSelected);

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



                    //InGridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'"
                    //          + " AND (OpenRecord = 1 OR (OpenRecord = 0 AND GridFilterDate = @ToDay ))" ;

                    InGridfilter = "Operator ='" + WOperator + "'" 
                            + " AND CAST(OpenDate As Date) = @OpenDate ";

                    WithDate = true;
                    
                    // Change GridFilter if exception requests 

                    if (WSesNo > 0) // Show only for this Session 
                    {
                        textBox5.Hide();
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'" + " AND AtmNo ='" + WAtmNo + "' AND SesNo =" + WSesNo;

                        WithDate = false;
                        Tp.ReadAllTransToBePostedAndFillTable(WSignedId, InGridfilter, DateTime.Today, WithDate);
                    }

                    if (WUniqueRecordId > 0) // Show Only for this Error 
                    {
                        textBox5.Hide();
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'" + " AND UniqueRecordId =" + WUniqueRecordId;

                        WithDate = false;
                        Tp.ReadAllTransToBePostedAndFillTable(WSignedId, InGridfilter, DateTime.Today, WithDate);
                    }

                    if (WMode == 3) // ALL Open for a particular ATM 
                    {
                        textBox5.Hide();
                        panel3.Hide();
                        textBox6.Hide();
                        panel4.Hide();
                        InGridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'" + " AND AtmNo ='" + WAtmNo + "' AND OpenRecord = 1";

                        WithDate = false;
                        Tp.ReadAllTransToBePostedAndFillTable(WSignedId, InGridfilter, DateTime.Today, WithDate);

                    }
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

                //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);


                if (dataGridView1.Rows.Count == 0 & WRange == false)
                {
                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("No transactions for today. Select other day");
                    MessageForm.ShowDialog();

                    //this.Dispose();
                    //return;
                }
                if (dataGridView1.Rows.Count == 0 & WRange == true)
                {
                    MessageBox.Show("No transactions for this selection"+Environment.NewLine
                        + "Get in and try another day "
                        );

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
                    textBoxCardNumber.Hide();
                    textBoxDescr_2.Hide(); 
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
                    WOriginNmInAuther = "ReconciliationCat";
                }
                if (Tp.OriginId == "02") // "02" BancNet and ITMX
                {
                    WMatching = true;
                    WOriginNmInAuther = "ReconciliationCat";
                }

                if (Tp.OriginId == "04") // "04" OurATMS-Repl
                {
                    WReplenishment = true;
                    WOriginNmInAuther = "Replenishment";
                }

                if (Tp.OriginId == "03") // "03" OurATMS-Reconc
                {
                    WReconciliation = true;
                    WOriginNmInAuther = "ReconciliationBulk";
                }

                if (Tp.OriginId == "05") // "05" Settlement
                {
                    WSettlement = true;
                    WOriginNmInAuther = "SettlementAuth";
                }

                if (Tp.OriginId == "06") // "06" Fees Settlement
                {
                    WFeesSettlement = true;
                    WOriginNmInAuther = "SettlementFeesAuth";
                }

                if (Tp.OriginId == "07") // "07" Disputes 
                {
                    WDisputeNo = Tp.DisputeNo;
                    WDisputeTranNo = Tp.DispTranNo;
                    WOriginNmInAuther = "Disputes";
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
                // 
                if (Tp.OriginName == "Replenishment")
                {
                    WReplenishment = true;
                    WOriginNmInAuther = "Replenishment";
                }

                if (Tp.OriginName == "Reconciliation")
                {
                    WReconciliation = true;
                    WOriginNmInAuther = "Reconciliation";
                }

                // SHOW ALL NEEDED DETAILS TO PAY ATTENTION ON
                //
                ErrNo = Tp.ErrNo;
                textBoxCardNumber.Text = Tp.CardNo;

                textBoxTransDate.Text = Tp.OpenDate.ToString();

                textBoxTXNSRC.Text = Tp.TXNSRC;
                textBoxTXNDEST.Text = Tp.TXNDEST; 

                if (Tp.AccNo == "Not Available")
                {
                    textBoxAccNoA.ReadOnly = false;
                    textBoxAccNoA.Text = "Input Account";

                    if (Tp.TransType == 11 || Tp.TransType == 12) textBox18.Text = "DR";
                    if (Tp.TransType == 21 || Tp.TransType == 22) textBox18.Text = "CR";

                    textBoxAccNoB.ReadOnly = false;
                    textBoxAccNoB.Text = "Input Account";

                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12) textBox19.Text = "DR";
                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22) textBox19.Text = "CR";
                }
                else
                {
                    textBoxAccNoA.ReadOnly = true;
                    textBoxAccNoA.Text = Tp.AccNo;

                    if (Tp.TransType == 11 || Tp.TransType == 12) textBox18.Text = "DR";
                    if (Tp.TransType == 21 || Tp.TransType == 22) textBox18.Text = "CR";

                    textBoxAccNoB.ReadOnly = true;
                    textBoxAccNoB.Text = Tp.AccNo2;

                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12) textBox19.Text = "DR";
                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22) textBox19.Text = "CR";
                }

                textBoxDescr_1.Text = Tp.TransDesc;
                textBox3.Text = Tp.CurrDesc;
                textBox8.Text = Tp.TranAmount.ToString("#,##0.00");
                textBoxRRN.Text = Tp.RRNumber.ToString();
                textBoxTrace.Text = Tp.AtmTraceNo.ToString();

                Us.ReadUsersRecord(Tp.MakerUser);
                textBoxMaker.Text = Tp.MakerUser +" "+Us.UserName;

                Us.ReadUsersRecord(Tp.AuthUser);
                textBoxAuthoriser.Text = Tp.AuthUser + " " +Us.UserName;

                textBox33.Text = Tp.AtmNo;

                Ec.ReadErrorsTableSpecific(ErrNo);

                textBoxDescr_2.Text = Tp.TransDesc2;

                textBoxMakerCooment.Text = Tp.TransMsg;// Which is equal to Mpa.comment 

                Gp.ReadParametersSpecificId(WOperator, "705", Tp.SystemTarget.ToString(), "", "");

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                Aoc.ReadActionsOccuarnceBySeqNo(Tp.ActionSeqNo);

                textBoxActionNm.Text = Aoc.ActionNm ; 
                textBoxActionOccur.Text = Aoc.Occurance.ToString();

                textBoxAccName_1.Text = Aoc.AccName_1;
                textBoxAccName_2.Text = Aoc.AccName_2;

                if (Tp.IsReversal == true)
                {
                    labelReversal.Show();
                }
                else
                {
                    labelReversal.Hide();
                }

                textBox14.Text = Tp.ActionBy.ToString();
                textBox13.Text = Tp.ActionCd2.ToString();
                if (Tp.ActionDate != NullPastDate) textBox12.Text = Tp.ActionDate.ToString();
                else textBox12.Text = "";

                if (Tp.ActionDate == NullPastDate)
                {
                    textBoxMsgBoard.Text = "Finalise action for this transaction";
                    // HIDE ACTION Fields 
                    pictureBox2.Hide();
                    pictureBox3.Show();
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
                    pictureBox3.Hide();
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

                radioButton8.Checked = false;
                radioButton7.Checked = false;
                radioButton6.Checked = false;

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }
        // UPDate ACTION 
        private void button1_Click(object sender, EventArgs e)
        {

            WRow = dataGridView1.SelectedRows[0].Index;

            int ActionCd2 = 0;

            if (radioButton8.Checked == false & radioButton7.Checked == false & radioButton6.Checked == false & radioButtonPostTrans.Checked == false)
            {
                MessageBox.Show(" Please select a radio button ");
                return;
            }
            if (radioButtonPostTrans.Checked == true)
            {
                MessageBox.Show("This selection is not available. " + Environment.NewLine
                    + "System is connected directly with customers back end systems" + Environment.NewLine
                    + "and transactions are posted automatically" + Environment.NewLine
                    + "In cooperation with customer's IT department special development is needed." + Environment.NewLine
                    );
                   
                return;
            }

            if (radioButton8.Checked == true) ActionCd2 = 1; // Action finalised with voucher 
            if (radioButton7.Checked == true) ActionCd2 = 2; // Action rejected
            if (radioButton6.Checked == true) ActionCd2 = 3; // Action postponed 
            if (radioButtonPostTrans.Checked == true)
            {
                ActionCd2 = 4; // All to be posted with Web Services 
                // Call Class 

                Tp.ReadTransToBePostedAllAndCreatePostedTrans(WSignedId, WOperator, ActionCd2, "", "Form78");
                if (Tp.RecordFound == true)
                {
                    //MessageBox.Show("Total number of created transactions = " + Tc.TotTransactions.ToString()); 
                }
                else
                {
                    MessageBox.Show("No open trans to be posted available");
                    return;
                }

                ShowGrid("");

                if (Tp.RecordFound == true)
                {
                    MessageBox.Show("Total number of created transactions = " + Tp.TotPairTransactions.ToString());
                }

                //MessageBox.Show("An interface with the Banking system is not available yet.");
                return;
            }

            if (ActionCd2 == 1 & Tp.ActionCd2 > 0 || ActionCd2 == 2 & Tp.ActionCd2 > 0) // Action Already taken
            {
                if (MessageBox.Show("MSG789 - Action already taken. Do you want to proceed? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                    // Continue 
                }
                else
                {
                    return;
                }

            }

            Tp.ActionDate = DateTime.Now;

            Tp.GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

            Tp.OpenRecord = false; // Close Record 

            Tp.UpdateTransToBePostedAction1(WPostedNo, WSignedId, ActionCd2);

            Tp.ReadTransToBePostedSpecific(WPostedNo);

            textBox14.Text = Tp.ActionBy.ToString();
            textBox13.Text = Tp.ActionCd2.ToString();
            if (Tp.ActionDate != NullPastDate) textBox12.Text = Tp.ActionDate.ToString();
            else textBox12.Text = "";

            if (radioButton8.Checked == true) // Action with voucher 
            {
                // Print Transactions 
                PrintTrans();
                if (Tp.SystemTarget == 1)
                {
                    PrintJCC();
                }
            }

            if (ActionCd2 == 1)
            {
                MessageBox.Show(" Record Updated. Cashier vouchers are printed.");

                //// Close Error 
                //if (Tp.ErrNo > 0)
                //{
                //    Ec.ReadErrorsTableSpecific(Tp.ErrNo);
                //    Ec.ByWhom = WSignedId;
                //    Ec.ActionDtTm = DateTime.Now;
                // //   Ec.ActionSes = Tp.SesNo;
                //    Ec.OpenErr = false;
                //    Ec.UpdateErrorsTableSpecific(Tp.ErrNo);
                //}

            }
            if (ActionCd2 == 2) MessageBox.Show(" Record Updated. Action is rejected.");
            if (ActionCd2 == 3) MessageBox.Show(" Record Updated. Action is postponed.");
            if (ActionCd2 == 4) MessageBox.Show(" Record Updated. Action is taken with updating directly the banking system.");

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            ShowGrid("");

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

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

           
            //if (dateTimePicker1.Value.Date == dateTimePicker1.Value.Date)
            //{
            //    WDtFrom = dateTimePicker1.Value.Date;
            //    WDtTo = dateTimePicker2.Value.Date;
            //    //WDtFrom = WDtFrom.AddDays(-1);
            //    //WDtTo = WDtTo.AddDays(1);
            //}
            //else
            //{
                WDtFrom = dateTimePicker1.Value.Date;
                WDtTo = dateTimePicker2.Value.Date;
            //}
            

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
                           + "' AND CAST(OpenDate as Date)  >= @WDtFrom AND CAST(OpenDate as Date) <= @WDtTo AND OpenRecord = 1";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton2.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == false) // Close and all
            {
                Gridfilter = "Operator ='" + WOperator
                           + "' AND CAST(OpenDate as Date)  >= @WDtFrom AND CAST(OpenDate as Date) <= @WDtTo AND OpenRecord = 0";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton1.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == true
                & radioButtonAtmNo.Checked == true) // ATM 
            {
                Gridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'"
                           + " AND CAST(OpenDate as Date)  >= @WDtFrom AND CAST(OpenDate as Date) <= @WDtTo AND OpenRecord = 1 AND AtmNo ='" + textBox11.Text + "'";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            if (radioButton2.Checked == true & comboBoxOrigin.Text == "N/A" & checkBoxUnique.Checked == true
             & radioButtonAtmNo.Checked == true) // ATM 
            {
                Gridfilter = "Operator ='" + WOperator + "' AND BankId ='" + WBankId + "'"
                           + " AND CAST(OpenDate as Date)  >= @WDtFrom AND CAST(OpenDate as Date) <= @WDtTo AND OpenRecord = 2 AND AtmNo ='" + textBox11.Text + "'";

                Tp.ReadAllTransToBePostedRange(WSignedId, Gridfilter, WDtFrom, WDtTo);
            }

            WRange = true;

            ShowGrid(Gridfilter);

            buttonExpandGrid.Show(); // This button goes to show grid in bigger form 

        }
        //
        // Print Transactions to be posted 
        //
        String P4;
        String P11;
        private void PrintTrans()
        {
            if (WOperator == "ITMX" & WBankId != "ITMX" & WDisputeNo > 0)
            {
                // THE Request comes from BBL Bank 

                Us.ReadUsersRecord(WSignedId);

                Ba.ReadBank(Us.BankId);

                Tp.ReadTransToBePostedSpecific(WPostedNo); // TRANS TO BE POSTED 

                Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);

                WDisputeNo = Tp.DisputeNo;
                WDisputeTranNo = Tp.DispTranNo;


                String P1 = WOperator;
                String P2 = Ba.BankName;

                String P3 = Us.Branch;
                String P4 = Mp.MobileRequestor;
                String P5 = "";
                if (Tp.TransType == 21 || Tp.TransType == 22)
                {
                    P5 = "CREDIT";
                }
                if (Tp.TransType == 11 || Tp.TransType == 12)
                {
                    P5 = "DEBIT";
                }
                String P7 = Tp.AccNo;
                String P8 = Tp.TranAmount.ToString("#,##0.00");
                String P9 = Tp.TransDesc;
                String P10 = WDisputeNo.ToString();
                String P11 = "It is created by Dispute Management";

                // Second transaction in pair 

                String P12 = "";

                if (Tp.TransType2 == 21 || Tp.TransType2 == 22)
                {
                    P12 = "CREDIT";
                }
                if (Tp.TransType2 == 11 || Tp.TransType2 == 12)
                {
                    P12 = "DEBIT";
                }

                String P13 = Tp.AccNo2;

                String P14 = Tp.TransDesc2;

                String P21 = Us.UserName;

                String P15;

                String P16;

                String P17;

                //// This is from Disputes 
                //Ap.ReadAuthorizationForDisputeAndTransaction(WDisputeNo, WDisputeTranNo);

                Us.ReadUsersRecord(Tp.MakerUser);
                P15 = Us.UserName;

                Us.ReadUsersRecord(Tp.AuthUser);
                P16 = Us.UserName;
                P17 = Us.BankId;

                Form56R3ITMX ReportTrans = new Form56R3ITMX(P1, P2, P3, P4, P5, P7,
                                       P8, P9, P10, P11, P12, P13, P14, P21, P15, P16, P17);
                ReportTrans.Show();
            }
            else
            {
                if (WOperator == "ITMX")
                {
                    // THE Request comes from ITMX Personel 

                    Us.ReadUsersRecord(WSignedId);

                    Ba.ReadBank(Us.BankId);

                    Tp.ReadTransToBePostedSpecific(WPostedNo); // TRANS TO BE POSTED 

                    WDisputeNo = Tp.DisputeNo;
                    WDisputeTranNo = Tp.DispTranNo;

                    string P1 = WOperator;
                    String P2 = Ba.BankName;

                    String P3 = Us.Branch;
                    

                    if (WSettlement == true || WFeesSettlement == true)
                    {
                        if (WSettlement) P4 = "Settlement Txn";
                        if (WFeesSettlement) P4 = "FeesSettlement Txn";
                    }
                    else
                    {
                        Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);
                        P4 = Mp.MobileRequestor;
                    }
                    
                    String P5 = "";
                    if (Tp.TransType == 21 || Tp.TransType == 22)
                    {
                        P5 = "CREDIT";
                    }
                    if (Tp.TransType == 11 || Tp.TransType == 12)
                    {
                        P5 = "DEBIT";
                    }
                    String P7 = Tp.AccNo;
                    String P8 = Tp.TranAmount.ToString("#,##0.00");
                    String P9 = Tp.TransDesc;
                    String P10 = WDisputeNo.ToString();

                    if (WOriginNmInAuther == "SettlementAuth" || (WOriginNmInAuther == "SettlementFeesAuth"))
                    {
                       if (WOriginNmInAuther == "SettlementAuth") P11 = "It is created by Settlement process";
                       if (WOriginNmInAuther == "SettlementFeesAuth") P11 = "It is created by Fees Settlement process";
                    }
                    else
                    {
                        P11 = "It is created During Reconciliation by ITMX";
                    }

                    // Second transaction in pair 

                    String P12 = "";

                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22)
                    {
                        P12 = "CREDIT";
                    }
                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12)
                    {
                        P12 = "DEBIT";
                    }

                    String P13 = Tp.AccNo2;

                    String P14 = Tp.TransDesc2;

                    String P21 = Us.UserName;

                    String P15;

                    String P16;

                    string P17; 

                    // This is coming from ITMX 
                    //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(Tp.RMCateg, Tp.RMCategCycle, WOriginNmInAuther);

                    Us.ReadUsersRecord(Tp.MakerUser);
                    P15 = Us.UserName;

                    Us.ReadUsersRecord(Tp.AuthUser);
                    P16 = Us.UserName;
                    P17 = Us.BankId;

                    Form56R3ITMX ReportTrans = new Form56R3ITMX(P1, P2, P3, P4, P5, P7,
                                           P8, P9, P10, P11, P12, P13, P14, P21, P15, P16, P17);
                    ReportTrans.Show();
                }
                else
                {
                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WLineAtmNo);
                    RRDMUsersRecords Us = new RRDMUsersRecords();
                    Us.ReadUsersRecord(WSignedId);
                    RRDMBanks Ba = new RRDMBanks();
                    Ba.ReadBank(WOperator);
                    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

                    Tp.ReadTransToBePostedSpecific(WPostedNo);

                    Ec.ReadErrorsTableSpecific(Tp.ErrNo);

                    String P1 = WOperator;
                    String P2 = Ba.BankName;

                    String P3 = Us.Branch;
                    String P4 = WLineAtmNo;
                    String P5 = "";
                    if (Tp.TransType == 21 || Tp.TransType == 22)
                    {
                        P5 = "CREDIT";
                    }
                    if (Tp.TransType == 11 || Tp.TransType == 12)
                    {
                        P5 = "DEBIT";
                    }
                    String P6 = Tp.CardNo;
                    String P7 = Tp.AccNo;
                    String P8 = Tp.TranAmount.ToString("#,##0.00");
                    String P9 = Tp.TransDesc;
                    String P10 = Tp.ErrNo.ToString();
                    String P11;
                    if (Tp.ErrNo == 0)
                    {
                        P11 = "";
                    }
                    else
                    {
                        P11 = Ec.ErrDesc;
                    }

                    // Second transaction in pair 

                    String P12 = "";

                    //     Tc.ReadTransToBePostedTraceSequence(AtmNo, Tc.AtmTraceNo, 2);

                    if (Tp.TransType2 == 21 || Tp.TransType2 == 22)
                    {
                        P12 = "CREDIT";
                    }
                    if (Tp.TransType2 == 11 || Tp.TransType2 == 12)
                    {
                        P12 = "DEBIT";
                    }

                    String P13 = Tp.AccNo2;

                    String P14 = Tp.TransDesc2;

                    String P21 = Us.UserName;

                    String P15;

                    String P16;
                  

                    //Us.ReadUsersRecord(Ap.Requestor);
                    Us.ReadUsersRecord(Tp.MakerUser);
                    P15 = Us.UserName;

                    //Us.ReadUsersRecord(Ap.Authoriser);
                    Us.ReadUsersRecord(Tp.AuthUser);
                    P16 = Us.UserName;

                    Form56R3 ReportTrans = new Form56R3(P1, P2, P3, P4, P5, P6, P7,
                                           P8, P9, P10, P11, P12, P13, P14, P21, P15, P16);
                    ReportTrans.Show();
                }

            }

        }

        //
        // Print JCC Form  
        //
        private void PrintJCC()
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WLineAtmNo);
            RRDMUsersRecords Us = new RRDMUsersRecords();
            Us.ReadUsersRecord(WSignedId);
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

            Tp.ReadTransToBePostedSpecific(WPostedNo);

            Ec.ReadErrorsTableSpecific(Tp.ErrNo);

            String P1 = Ac.Branch;

            String P2 = Ac.BranchName;
            String P3 = WLineAtmNo;
            String P4 = "";
            if (Tp.TransType == 21 || Tp.TransType == 22)
            {
                P4 = "CREDIT";
            }
            if (Tp.TransType == 11 || Tp.TransType == 12)
            {
                P4 = "DEBIT";
            }
            String P5 = Tp.CardNo;

            String P6 = Tp.TranAmount.ToString();

            String P7 = Tp.AtmDtTime.ToString();

            String P8 = Tp.TranAmount.ToString("#,##0.00");

            String P9 = Tp.RefNumb.ToString();

            String P10 = Tp.AuthCode.ToString();

            //Ap.ReadAuthorizationForDisputeAndTransaction(WDisputeNo, WDisputeTranNo);

            Us.ReadUsersRecord(Tp.MakerUser);
            String P15 = Us.UserName;

            Us.ReadUsersRecord(Tp.AuthUser);
            String P16 = Us.UserName;


            String P21 = Us.UserName;

            String P22 = WBankId;

            Form56R4 ReportTrans = new Form56R4(P1, P2, P3, P4, P5, P6, P7,
                                   P8, P9, P10, P15, P16, P21, P22);
            ReportTrans.Show();

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
                //Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtmViewClose(WLineAtmNo, WLineSesNo, WOriginNmInAuther);
                //if (Ap.RecordFound == true)
                //{
                //}
                //else
                //{
                //    MessageBox.Show("No authorisation history for this testing ATM!");
                //    //return;
                //}

                if (WReplenishment == true)
                {
                    NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WLineAtmNo, WLineSesNo, WDisputeNo, WDisputeTranNo, WRMCateg, WRMCategCycle);
                    NForm112.ShowDialog();
                }

                if (WReconciliation == true)
                {
                    NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", "", WLineSesNo, WDisputeNo, WDisputeTranNo, WRMCateg, WRMCategCycle);
                    NForm112.ShowDialog();
                }  
            }
            else
            {
                MessageBox.Show("No available functionality! ");
            }
            return; 
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

            string P1 = "Transactions To Be Posted";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R61ATMS ReportATMS61 = new Form56R61ATMS(P1, P2, P3, P4, P5);
            ReportATMS61.Show();
        }
// Reconciliation Link 
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
      //      RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
      //      Mpa.ReadInPoolTransSpecificUniqueRecordId(Tp.UniqueRecordId,1); 
      //      RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
      //      Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
      //      if (Rcs.EndReconcDtTm == NullPastDate)
      //      {
      //          MessageBox.Show("Reconciliation Not done yet!");
      //          return;
      //      }

      //      // Update Us Process number
      //      RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
      //      Usi.ReadSignedActivityByKey(WSignRecordNo);
      //      Usi.ProcessNo = 54; // View Only 
      //      Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

      //      Form271 NForm271;

      //      NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Mpa.RMCateg, Mpa.MatchingAtRMCycle);
      ////      NForm271.FormClosed += NForm271_FormClosed;
      //      NForm271.ShowDialog();

            ////  Form80b_Load(this, new EventArgs());
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadSignedActivityByKey(WSignRecordNo);
            //Usi.ProcessNo = 54; // View Only 
            //Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //Form271 NForm271;

            //NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo);
            //NForm271.FormClosed += NForm271_FormClosed;
            //NForm271.ShowDialog(); ;

        }
// Show Bank De Caire GL 
        private void buttonBDC_GL_Click(object sender, EventArgs e)
        {
            // Find Current RM Cycle number 
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (dateTimePicker1.Value.Date != dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Selected dates must be the same");
                return; 
            }
            int WMode = 1;
            DateTime TempDt = dateTimePicker1.Value.Date; 
            Form78d_BDC_TO_POST NForm78D_BDC_TO_POST;
            NForm78D_BDC_TO_POST = new Form78d_BDC_TO_POST(WOperator, WSignedId
                                                                     , WReconcCycleNo, TempDt , WMode,T24Version);

            NForm78D_BDC_TO_POST.Show();
        }
// SHOW GIFU
        private void buttonGIFU_Click(object sender, EventArgs e)
        {
            // Find Current RM Cycle number 
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (dateTimePicker1.Value.Date != dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Selected dates must be the same");
                return;
            }
            int WMode = 2;
            DateTime TempDt = dateTimePicker1.Value.Date;
            Form78d_BDC_TO_POST NForm78D_BDC_TO_POST;
            NForm78D_BDC_TO_POST = new Form78d_BDC_TO_POST(WOperator, WSignedId
                                                                     , WReconcCycleNo, TempDt , WMode,T24Version);

            NForm78D_BDC_TO_POST.Show();
        }
// Settlement e-Mail
        private void buttonSettl_email_Click(object sender, EventArgs e)
        {
            // Find Current RM Cycle number 
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (dateTimePicker1.Value.Date != dateTimePicker2.Value.Date)
            {
                MessageBox.Show("Selected dates must be the same");
                return;
            }
            int WMode = 3;
            DateTime TempDt = dateTimePicker1.Value.Date;
            Form78d_BDC_TO_POST NForm78D_BDC_TO_POST;
            NForm78D_BDC_TO_POST = new Form78d_BDC_TO_POST(WOperator, WSignedId
                                                                     , WReconcCycleNo, TempDt , WMode, T24Version);

            NForm78D_BDC_TO_POST.Show();
        }
// Create Excel 
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = textBoxHeaderForTxns.Text;

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Tp.TransToBePostedDataTable, WorkingDir, ExcelPath);
        }
    }
}

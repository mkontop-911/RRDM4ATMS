using System;
using System.Windows.Forms;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;

using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form52_GL_W2 : Form
    {
     
        //Form31 NForm31;

        //DateTime FromDate;
        //DateTime ToDate;

        string SelectionCriteria;

    //    bool Presented = false; 

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

        //RRDM_CitExcelCycles Cec = new RRDM_CitExcelCycles();

        //RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        //RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2017, 09, 01);

        DateTime WDateFrom;
        DateTime WDateTo;

        int TempMode; 

        string WOperator;
        string WSignedId;

        string WCitId;

        DateTime WDate;

        public Form52_GL_W2(string InOperator, string InSignedId, string InCitId, DateTime InWorkingDate)
        {
            WSignedId = InSignedId;
            //WSignRecordNo = SignRecordNo;
            //WSecLevel = InSecLevel;
            WOperator = InOperator;

            WCitId = InCitId;

            WDate = InWorkingDate;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            //dateTimePickerFrom.Value = WDate.Date;
            //// Testing Value 
            //dateTimePickerFrom.Value = WorkingToday.Date;
            
            //dateTimePickerTo.Value = WDate.Date;

            label1.Text = "SELECTION FOR CIT : " + WCitId ;

            textBoxMsgBoard.Text = "View Banks Records. Make Selection.";
            TempMode = 2 ;
            SelectionCriteria = " ORDER BY AtmNo, SeqNo ";

        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {
            WDateFrom = WorkingToday.Date;
            WDateTo = WDate.Date;

            G4.ReadCIT_G4S_Repl_EntriesToFillDataTableAND_TotalsByDateRange(WOperator, WSignedId, "", WDateFrom, WDateTo, SelectionCriteria,TempMode );

            ShowGrid1();

        }
        //
        // ROW ENTER ON USER 
        //
       // bool RequestFromMatched;
        int RowSeqNo;
        string WAtmNo;
        int WSesNo;
        string ReplDate; 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            RowSeqNo = (int)rowSelected.Cells[0].Value;

            // Read Entries for Bank 
            TempMode = 2;
            G4.ReadCIT_G4S_Repl_EntriesBySeqNo(RowSeqNo, TempMode);
          
            WAtmNo = G4.AtmNo;
            WSesNo = G4.ReplCycleNo;

            ReplDate = G4.ReplDateG4S.ToShortDateString(); 

            textBoxAtm.Text = WAtmNo;
            textBoxReplCycle.Text = WSesNo.ToString();

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            Am.ReadAtmsMainSpecific(WAtmNo);

            textBoxCurrentRepl.Text = Am.CurrentSesNo.ToString();

            panel2.Hide();
        }


        // ACTION MESSAGES 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("These are the actions messages send to CIT Company");
            return;
        }


        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid1()
        {
            dataGridView1.DataSource = G4.DataTableG4SEntries.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No entries Available.");
                this.Dispose(); 
                //return;
            }
            else
            {
                textBoxTotalEnries.Text = dataGridView1.Rows.Count.ToString();
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 40; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Visible = false;

                dataGridView1.Columns[1].Width = 60; // CITId
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 100; //  OriginFileName
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].Visible = false;

                dataGridView1.Columns[3].Width = 60; // AtmNo
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].Visible = true;

                dataGridView1.Columns[4].Width = 80; // AtmName
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].Visible = false;

                dataGridView1.Columns[5].Width = 50; // LoadingCycleNo
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[5].Visible = false;

                dataGridView1.Columns[6].Width = 100; //ReplDateG4S
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[6].Visible = true;

                dataGridView1.Columns[7].Width = 50; // OrderId 
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[7].Visible = false;

                dataGridView1.Columns[8].Width = 0; // CreatedDate
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[8].Visible = false;

                dataGridView1.Columns[9].Width = 50; // IsDeposit
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[9].HeaderText = "Is Deposit";

                dataGridView1.Columns[10].Width = 70; // OpeningBalance
                dataGridView1.Columns[10].DefaultCellStyle = style;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[10].HeaderText = "Opening Balance";

                dataGridView1.Columns[11].Width = 70; //Dispensed
                dataGridView1.Columns[11].DefaultCellStyle = style;
                dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[12].Width = 70; // UnloadedMachine
                dataGridView1.Columns[12].DefaultCellStyle = style;
                dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[12].HeaderText = "Unloaded Machine";

                dataGridView1.Columns[13].Width = 70; // UnloadedCounted
                dataGridView1.Columns[13].DefaultCellStyle = style;
                dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[13].HeaderText = "Unloaded Counted";

                dataGridView1.Columns[14].Width = 70; // Cash_Loaded
                dataGridView1.Columns[14].DefaultCellStyle = style;
                dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[14].HeaderText = "Cash Loaded";

                dataGridView1.Columns[15].Width = 60; // Deposits
                dataGridView1.Columns[15].DefaultCellStyle = style;
                dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Columns[16].Width = 60; //OverFound
                dataGridView1.Columns[16].DefaultCellStyle = style;
                dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[16].HeaderText = "Over Found";

                dataGridView1.Columns[17].Width = 60; // ShortFound
                dataGridView1.Columns[17].DefaultCellStyle = style;
                dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[17].HeaderText = "Short Found";

                dataGridView1.Columns[18].Width = 50; // RemarksG4S
                dataGridView1.Columns[18].DefaultCellStyle = style;
                dataGridView1.Columns[18].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[18].HeaderText = "Remarks";

                dataGridView1.Columns[19].Width = 60; // PresentedErrors
                dataGridView1.Columns[19].DefaultCellStyle = style;
                dataGridView1.Columns[19].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[19].HeaderText = "Presented Errors";

            }

        }


        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Print What you VIEW 
        private void buttonP11_Click(object sender, EventArgs e)
        {
            string P1 = label1.Text; 

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Bank ReportATMSReplCycles = new Form56R69ATMS_Bank(P1, P2, P3, P4, P5);
            ReportATMSReplCycles.Show();
        }

// VIEW BY DATE 
        private void button2_Click(object sender, EventArgs e)
        {
            SelectionCriteria = ""; 
            Form18_Load(this, new EventArgs());
        }
        // Create Excel 

     //   DateTime Last_Cut_Off_Date;

     
// Show SM LINES 
        private void buttonShowSM_Click(object sender, EventArgs e)
        {

            int WTraceNo; 

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            WTraceNo = Ta.LastTraceNo;
            DateTime WDateA = Ta.SesDtTimeEnd.Date; 
            // Check Trace No
            Int32 LastDigit = WTraceNo % 10;

            if (LastDigit == 0)
            {
                // OK
                // It is just a transaction
            }
            else
            {
                // It is the supervisor mode
                WTraceNo = (WTraceNo - LastDigit) + 1;
            }

            Form67 NForm67;
            int Mode = 5; // Specific

            NForm67 = new Form67(WSignedId, 0, WOperator, 0, WAtmNo, WTraceNo, WTraceNo, WDateA, NullPastDate, Mode);
            NForm67.ShowDialog();
        }

// Make GL 
        private void buttonGL_Click(object sender, EventArgs e)
        {
            // THIS is a complete code for one ATM GL reconciliation
            // It runs after the last cutoff errors are corrected - say in the afternoon
            // Why? becuase these might create corrections on GL balance.
            // Presenter errors make no effect on the whole process 
            // because
            // We take into consideration the machine oppening balance and we do not by 
            // By any other way take into account the counted balance
            // ie presenter error is like any transaction

            panel2.Show(); 

            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMAccountsClass Acc = new RRDMAccountsClass();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
            RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions(); 

            Am.ReadAtmsMainSpecific(WAtmNo); 
            DateTime WorkingToday = new DateTime(2018, 03, 20);
            decimal CalculatedGL = 0;

            CalculatedGL = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_Second_Method
                                 (WOperator, WAtmNo,Am.CurrentSesNo, WorkingToday);

            if (Gadj.IsDataFound == true)
            {
                textBoxCutOff.Text = Gadj.Last_Cut_Off_Date.ToShortDateString();

                textBoxOpenBal.Text = Gadj.GlOpeningBalance.ToString("#,##0.00");
                textBoxDispensed.Text = Gadj.TotalDispensed.ToString("#,##0.00");
                textBoxCalculatedGL.Text = CalculatedGL.ToString("#,##0.00");

                textBoxRealGL.Text = Gadj.FoundGlBalance.ToString("#,##0.00");

                //Na.ReadSessionsNotesAndValues(WAtmNo, Am.CurrentSesNo, 4);

                // Here we get the errors for this particular Repl Cycle
                // These are the errors created by User during the reconciliation
                // They are affecting ATM GL
                // They happen at the last RM cycle and are corrected today. GL not updated  

                decimal GlErrors = Er.ReadAllErrorsTableToFindGL_Total(WAtmNo, Am.CurrentSesNo, Gadj.RMCycle);

                textBoxErrors.Text = GlErrors.ToString("#,##0.00"); // ERRORS

                textBoxAdjGL.Text = (Gadj.FoundGlBalance + GlErrors).ToString("#,##0.00");

                textBoxDiffGL.Text = (CalculatedGL - (Gadj.FoundGlBalance + GlErrors)).ToString("#,##0.00");

            }
        }
// View Dispensed 
        private void buttonViewDispensed_Click(object sender, EventArgs e)
        {
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            Am.ReadAtmsMainSpecific(WAtmNo);
            string P1 = "TXNs from Replenishement:.. "+ ReplDate + " till Latest Cut OFF:.."+ textBoxCutOff.Text + " for Atm:_" + WAtmNo;

            string P2 = WAtmNo;
            string P3 = Am.CurrentSesNo.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R_TXNsFrom_CutOff Report_Txns_CutOff_To_Repl = new Form56R_TXNsFrom_CutOff(P1, P2, P3, P4, P5);
            Report_Txns_CutOff_To_Repl.Show();
        }
// Print Corrections on GL 
        private void buttonActionsOnGL_Click(object sender, EventArgs e)
        {
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            Am.ReadAtmsMainSpecific(WAtmNo);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            // Matching is done but not Settled 
            string SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                      + " AND TerminalId ='" + WAtmNo + "'"
                      + " AND MatchingAtRMCycle =" + Am.CurrentSesNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      + " AND ActionType != '07' ";

            string WSortCriteria = "Order By TerminalId, SeqNo ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);

            string P1 = "Actions on exceptions For Replenishement Cycle : " + Am.CurrentSesNo.ToString();

            string P2 = "";
            string P3 = "";

            bool ViewWorkFlow = false; 

            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("NBG101", Am.CurrentSesNo, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;

                    P2 = "Not Found";

                    P3 = "Not Found";
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }

            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }

    }
}


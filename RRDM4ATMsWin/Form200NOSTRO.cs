using System;
using System.Windows.Forms;
using System.Drawing;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;



namespace RRDM4ATMsWin
{
    public partial class Form200NOSTRO : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        //Form22MIS NForm22MIS; 
        //Form24 NForm24; 

        // string AtmNo;
        //  int CurrentSesNo;

        //   string BankId;
        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        //RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingCategoriesITMX Mc = new RRDMMatchingCategoriesITMX();

        //RRDMMatchingCategoriesSessions Mcs = new RRDMMatchingCategoriesSessions();

        RRDMITMXLoadingFilesRegister Flr = new RRDMITMXLoadingFilesRegister();
        //
        //
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingCategories MC = new RRDMMatchingCategories();

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMNVDisputes Di = new RRDMNVDisputes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string SelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        string WSubSystem;

        string WMatchingRunningGroup;

        // Methods 
        // READ ATMs Main
        // 
        public Form200NOSTRO(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InSubSystem ,string InMatchingRunningGroup)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WSubSystem = InSubSystem;
            WMatchingRunningGroup = InMatchingRunningGroup;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //*****************************************
            //
            labelStep1.Text = "Loading,  Matching and Reconciliation Status For " + WOperator;
            //
            //*****************************************

            textBoxMsgBoard.Text = "The total picture Loading Matching and Reconc. is shown ";

            // ....

            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
              + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");

            // MAIN PROGRAM 
            //TESTING
            DateTime WReplDate = DateTime.Today;
            WReplDate = new DateTime(2014, 04, 18); // IF NOT TESTING THIS IS TODAY

            string SelectionCriteria = " WHERE Operator='" + WOperator + "'";
            Rjc.ReadReconcJobCyclesFillTable(WOperator);

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            dataGridView1.Columns[0].Width = 80; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // 
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 120; // 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //TableITMXDailyJobCycles.Columns.Add("ITMXJobCycle", typeof(int));
            //TableITMXDailyJobCycles.Columns.Add("JobCategory", typeof(string));

            //TableITMXDailyJobCycles.Columns.Add("StartedDate", typeof(DateTime));
            //TableITMXDailyJobCycles.Columns.Add("Description", typeof(string));

        }

        // ROW ENTER FOR JOB CYCLE 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WJobCycleNo = (int)rowSelected.Cells[0].Value;

            Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo); 
            if (Rjc.JobCategory != "NostroReconciliation")
            {
                MessageBox.Show("Run NOSTRO System Matching and try again");
                return; 
            }
            Rcs.ReadReconciliationSessionsTOTALSForDashBoard
                                   (WOperator, WSignedId, WJobCycleNo);

            textBox61.Text = Rcs.No_Categories.ToString();
            textBox62.Text = Rcs.LoadedStmts.ToString();
            textBox63.Text = Rcs.NotLoadedStmts.ToString();
            textBox64.Text = Rcs.CatUnMatched.ToString();
            textBox65.Text = Rcs.TotalNumberProcessed.ToString();

            int MatchedCategories = Rcs.No_Categories - Rcs.CatUnMatched;

            textBox71.Text = Rcs.MatchedDefault.ToString();
            textBox72.Text = Rcs.AutoButToBeConfirmed.ToString();
            textBox73.Text = (Rcs.TotalNumberProcessed - (Rcs.MatchedDefault + Rcs.AutoButToBeConfirmed)).ToString();

            textBox76.Text = Rcs.MatchedFromAutoToBeConfirmed.ToString();
            textBox77.Text = Rcs.MatchedFromManualToBeConfirmed.ToString();
            textBox78.Text = Rcs.NumberOfUnMatchedRecs.ToString();

            textBox81.Text = Rcs.UnMatchedAmt.ToString("#,##0.00");

            textBox86.Text = Rcs.OutstandingAlerts.ToString();
            textBox87.Text = Rcs.OutstandingDisputes.ToString();

            label49.Text = "RECONCILIATION  STATUS FOR JOB CYCLE NO :..." + WJobCycleNo;

            // 
            //REconciliation light 
            //

            Gp.ReadParametersSpecificId(WOperator, "603", "1", "", ""); // < is Green 
            int QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "1", "", ""); // > is Red 
            int QualityRange2 = (int)Gp.Amount;

            if (Rcs.CatUnMatched <= QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (Rcs.CatUnMatched >= QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if (Rcs.CatUnMatched > QualityRange1 & Rcs.CatUnMatched < QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }
           
            //
            //
            // Matching - Chart1

            int[] yvalues1 = { MatchedCategories, Rcs.CatUnMatched };
            string[] xvalues1 = { "Yes", "No" };

            // Set series members names for the X and Y values 
            chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1);

            //// RECONCILED - Chart2

            //int[] yvalues2 = { Rcs.TotalReconcDone, Rcs.TotalReconcNotDone, Rcs.TotalPreviousRunningCycleReconcNotDone };
            //string[] xvalues2 = { "Yes", "No Today", "No old" };

            //// Set series members names for the X and Y values 
            //chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2);

        }
        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
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

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {

                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Show Unmatched Pairs 
        private void buttonUnMatchedPairs_Click(object sender, EventArgs e)
        {
            labelMatchingGridHeader.Text = "UNMATCHED PAIRS"; 

            SelectionCriteria = " WHERE RunningJobNo =" + WJobCycleNo + " AND (TotalNumberProcessed -"
                + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 ";

            //SelectionCriteria = " WHERE  TotalNumberProcessed >0 ";
            Rcs.ReadNVReconcCategoriesSessionsSpecificRunningJobCycle(SelectionCriteria);

        
            ShowGrid2();

        }
        // Not loaded statements 
        private void buttonNotLoadedStmts_Click(object sender, EventArgs e)
        {
            labelMatchingGridHeader.Text = "PAIRS WITH STATEMENT NOT LOADED" ;

            SelectionCriteria = " WHERE RunningJobNo =" + WJobCycleNo + " AND StatementLoaded = 0 ";

            Rcs.ReadNVReconcCategoriesSessionsSpecificRunningJobCycle(SelectionCriteria);
          
            ShowGrid2(); 
        }

        // Show Grid RCS
        private void ShowGrid2()
        {
            labelReconcGridHeader.Show();
            dataGridView2.Show();

            dataGridView2.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {

                Form2 MessageForm = new Form2("No Records To Show");
                MessageForm.ShowDialog();

                dataGridView2.Hide();
                labelMatchingGridHeader.Hide();

                return;
            }
            else
            {
                dataGridView2.Show();
                labelMatchingGridHeader.Show();
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 40; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 70; // CategoryId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].Visible = true;

            dataGridView2.Columns[2].Width = 190; // Name
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[2].Visible = false;

            dataGridView2.Columns[3].Width = 60; // ccy 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 90; // MatchedAuto
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 90; // ToBeConfirmed
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 90; // UnMatched
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[7].Width = 120; // LocalAmt
            dataGridView2.Columns[7].DefaultCellStyle = style;
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[8].Width = 80; // Alerts
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[9].Width = 80; // Disputes
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[10].Width = 90; // OwnerId
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[11].Width = 150; // OwnerName
            dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[12].Width = 200; //StartManual
            dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[13].Width = 200; //EndManual
            dataGridView2.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
       
//
// EXPAND ALERTS 
//
        private void linkLabelALerts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80bNV NForm80bNV;
            string WCategoryId = "ALL";
           
            NForm80bNV = new Form80bNV(WSignedId, WSignRecordNo, WOperator, WSubSystem, WCategoryId, WJobCycleNo,
                                              0, "ViewAllAlerts", NullPastDate, NullPastDate);

            NForm80bNV.ShowDialog();
        }
// Show Disputes
        private void linkLabelShowDisputes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3_NOSTRO NForm3_NOSTRO;
            string Mode = "UPDATE";
            NForm3_NOSTRO = new Form3_NOSTRO(WSignedId, WSignRecordNo, WOperator,
                            Mode, 0);
         
            NForm3_NOSTRO.ShowDialog();
        }
    }
}

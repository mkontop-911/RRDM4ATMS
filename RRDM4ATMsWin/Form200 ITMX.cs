using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;



namespace RRDM4ATMsWin
{
    public partial class Form200ITMX : Form
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

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMITMXLoadingFilesRegister Flr = new RRDMITMXLoadingFilesRegister();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);
        

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
  
        string WMatchingRunningGroup; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200ITMX(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
       
            WMatchingRunningGroup = InMatchingRunningGroup;

            InitializeComponent();

            
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            //*****************************************
            //
            labelStep1.Text = "Loading,  Matching and Reconciliation Status For " + WOperator;
            //
            //*****************************************

            textBoxMsgBoard.Text = "The total picture Loading Matching and Reconc. is shown ";

            dataGridView2.Hide(); 

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

            label40.Text = "MATCHING STATUS FOR JOB CYCLE NO :.. " + WJobCycleNo;

            // Get information for loading of  files
            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + WJobCycleNo; 
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, SelectionCriteria);
            textBox4.Text = Flr.TotalReceived.ToString();
            textBox5.Text = Flr.TotalNotReceived.ToString();

            // Get stats for Matching equal or less to JobCycle 
            Mc.ReadReconcCategoriesForMatchingStatus(WOperator, WMatchingRunningGroup, WJobCycleNo);

            // NOW WE HAVE TWO TOTALS AND TWO TABLES 

            textBox35.Text = (Mc.TotalMatchingDone + Mc.TotalMatchingNotDone).ToString();
            textBox34.Text = Mc.TotalMatchingDone.ToString(); ;
            textBox33.Text = Mc.TotalMatchingNotDone.ToString();
           
            Rcs.ReadReconcCategoriesForReconcStatus(WOperator, WJobCycleNo);

            textBox28.Text = (Rcs.TotalReconcDone + Rcs.TotalReconcNotDone
                               + Rcs.TotalPreviousRunningCycleReconcNotDone).ToString();
            textBox29.Text = Rcs.TotalReconcDone.ToString();
            textBox30.Text = Rcs.TotalReconcNotDone.ToString();

            textBox1.Text = Rcs.TotalPreviousRunningCycleReconcNotDone.ToString();

            int TotalReconcNotDone = Rcs.TotalReconcNotDone + Rcs.TotalPreviousRunningCycleReconcNotDone;

            textBox2.Text = Rcs.TotalReconciledExceptions.ToString();
            textBox31.Text = Rcs.TotalNonReconciledExceptions.ToString();

            // 
            //Matching Trafic Light 
            //

            Gp.ReadParametersSpecificId(WOperator, "603", "9", "", ""); // < is Green 
            int QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "9", "", ""); // > is Red 
            int QualityRange2 = (int)Gp.Amount;

            if (Mc.TotalMatchingNotDone <= QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (Mc.TotalMatchingNotDone >= QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if (Mc.TotalMatchingNotDone > QualityRange1 & Mc.TotalUnMatchedRecords < QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }
            // 
            //Reconciliation Trafic Light 
            //
            Gp.ReadParametersSpecificId(WOperator, "603", "10", "", ""); // < is Green 
            QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "10", "", ""); // > is Red 
            QualityRange2 = (int)Gp.Amount;

            if (TotalReconcNotDone <= QualityRange1)
            {
                // Green
                pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (TotalReconcNotDone >= QualityRange2)
            {
                // Red 
                pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if (TotalReconcNotDone > QualityRange1 & Mc.TotalUnMatchedRecords < QualityRange2)
            {
                // Yellow 
                pictureBox3.BackgroundImage = appResImg.YELLOW_Repl;
            }

            //
            //
            // Matching - Chart1

            //int[] yvalues1 = { Mc.TotalMatchingDone, Mc.TotalMatchingNotDone };
            //string[] xvalues1 = { "Yes", "No" };

            //// Set series members names for the X and Y values 
            //chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1);

            //// RECONCILED - Chart2

            int[] yvalues2 = { Rcs.TotalReconcDone, Rcs.TotalReconcNotDone, Rcs.TotalPreviousRunningCycleReconcNotDone };
            string[] xvalues2 = { "Yes", "No Today", "No old" };

            // Set series members names for the X and Y values 
            chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2);

            //SHOW GRIDS 
            if (Mc.TotalMatchingDone > 0)
            {
                ShowMatched();
            }
            if (Mc.TotalMatchingNotDone > 0)
            {
            }
            
            ShowReconciled();


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
      
        // Show Categories where Matching is done 

        private void buttonMatching_Click(object sender, EventArgs e)
        {
            ShowMatched();
        }

        private void ShowMatched()
        {
            labelMatchingGridHeader.Text = "Matched"; 
            dataGridView2.DataSource = Mc.TableMatchingCategMatched.DefaultView;

            dataGridView2.Columns[0].Width = 80; // 
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 200; // 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 70; // last runn
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 80; // Matching_Dt
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 70; // OwnerId
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (dataGridView2.Rows.Count == 0)
            {
                dataGridView2.Hide();
                Form2 MessageForm = new Form2("There are no categories for this request.");
                MessageForm.ShowDialog();
                return;
            }
            else
            {
                dataGridView2.Show();
            }
        }

        // Show Categories where Matching is NOT DONE 
        private void buttonNoMatching_Click(object sender, EventArgs e)
        {
            labelMatchingGridHeader.Text = "Un-Matched";
            dataGridView2.DataSource = Mc.TableMatchingCategUnMatched.DefaultView;

            dataGridView2.Columns[0].Width = 80; // Identity
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 200; // Category_Name
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 70; // last runn
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 80; // Matching_Dt
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 70; // OwnerId
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //TableMatchingCategUnMatched.Columns.Add("Identity", typeof(string));
            //TableMatchingCategUnMatched.Columns.Add("Category_Name", typeof(string));
            //TableMatchingCategUnMatched.Columns.Add("LastRunningJob", typeof(string));
            //TableMatchingCategUnMatched.Columns.Add("Matching_Dt", typeof(DateTime));
            //TableMatchingCategUnMatched.Columns.Add("OwnerId", typeof(string));

            if (dataGridView2.Rows.Count == 0)
            {
                dataGridView2.Hide(); 
                Form2 MessageForm = new Form2("There are no categories for this request.");
                MessageForm.ShowDialog();            
                return;
            }
            else
            {
                dataGridView2.Show();
            }
        }
// Show Reconciled
        private void buttonReconciled_Click(object sender, EventArgs e)
        {
            ShowReconciled();

        }
        private void ShowReconciled()
        {
            labelReconcGridHeader.Text = "Reconciled"; 
            dataGridView3.DataSource = Rcs.TableReconciledCategories.DefaultView;

            dataGridView3.Columns[0].Width = 130; // CategoryNm
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 70; // RunningJobNo
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 80; // OwnerId
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            //TableReconciledCategories.Columns.Add("CategoryNm", typeof(string));
            //TableReconciledCategories.Columns.Add("RunningJobNo", typeof(int));
            //TableReconciledCategories.Columns.Add("OwnerId", typeof(string));

            if (dataGridView3.Rows.Count == 0)
            {
                dataGridView3.Hide();
                //Form2 MessageForm = new Form2("There are no categories for this request.");
                //MessageForm.ShowDialog();
                //return;
                labelReconcGridHeader.Hide();
                textBox3.Show(); 
            }
            else
            {
                labelReconcGridHeader.Show();
                dataGridView3.Show();
                textBox3.Hide();
            }

        }
        //Show Not Reconciled 
        private void buttonUnReconciled_Click(object sender, EventArgs e)
        {
            labelReconcGridHeader.Text = "Not-Reconciled";      

            dataGridView3.DataSource = Rcs.TableNonReconciledCategories.DefaultView;

            dataGridView3.Columns[0].Width = 130; // CategoryNm
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 70; // RunningJobNo
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 80; // OwnerId
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (dataGridView3.Rows.Count == 0)
            {
                dataGridView3.Hide();
                Form2 MessageForm = new Form2("There are no categories for this request.");
                MessageForm.ShowDialog();
                textBox3.Show();
                return;
            }
            else
            {
                dataGridView3.Show();
                textBox3.Hide(); 
            }
        }
//Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// SHOW NOT RECEIVED FILES 
        private void button1_Click(object sender, EventArgs e)
        {
            string SelectionCriteria = 
                " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + WJobCycleNo + " AND ReceivedCode <> '00' ";
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, SelectionCriteria);

            if (Flr.TotalNotReceived ==0)
            {
                Form2 MessageForm = new Form2("There are no items to show.");
                MessageForm.ShowDialog();

                return;

            }
            labelMatchingGridHeader.Text = "NOT RECEIVED FILES";
            dataGridView2.DataSource = Flr.TableFilesLoadingRegister.DefaultView;

            dataGridView2.Columns[0].Width = 80; // ITMXJobCycle
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter ;

            dataGridView2.Columns[1].Width = 50; // BankId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 130; // FileId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 100; // ExpectedDate
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 110; // Status
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 70; // ReceivedDate
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (dataGridView2.Rows.Count == 0)
            {
                labelMatchingGridHeader.Hide(); 
                dataGridView2.Hide();
                Form2 MessageForm = new Form2("There are no items to show.");
                MessageForm.ShowDialog();
         
                return;
            }
            else
            {
                dataGridView2.Show();
              
            }

            //TableFilesLoadingRegister.Columns.Add("ITMXJobCycle", typeof(int));
            //TableFilesLoadingRegister.Columns.Add("BankId", typeof(string));
            //TableFilesLoadingRegister.Columns.Add("ExpectedDate", typeof(DateTime));
            //TableFilesLoadingRegister.Columns.Add("Status", typeof(string));
            //TableFilesLoadingRegister.Columns.Add("ReceivedDate", typeof(string));
        }
    }
}

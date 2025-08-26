using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;


namespace RRDM4ATMsWin
{
    public partial class Form200bITMX : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;

        public bool Prive;
 
        int WJobCycleNo; 

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMITMXLoadingFilesRegister Flr = new RRDMITMXLoadingFilesRegister();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);
        //     string WBankId;

        string WSelectionCriteria; 
        string WFileId; 

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
  
        string WMatchingRunningGroup; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200bITMX(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup)
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
            labelStep1.Text = "Files Loading Status For " + WOperator;
            //
            //*****************************************

            textBoxMsgBoard.Text = "Loading Status. Use buttons for other Info. ";

            dataGridView2.Hide();
            button5.Hide();
            label1.Hide();
            button3.Hide();
            button4.Hide();
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

            string SelectionCriteria = " WHERE Operator='" + WOperator + "'";
            Rjc.ReadReconcJobCyclesFillTable(WOperator);

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            dataGridView1.Columns[0].Width = 80; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // 
            dataGridView1.Columns[1].Visible = false; 

            dataGridView1.Columns[2].Width = 180; // 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 180; // 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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

            label31.Text = "LOADING STATUS FOR JOB CYCLE NO :.. " + WJobCycleNo;
            label40.Text = "LOADING STATUS FOR JOB CYCLE NO :.. " + WJobCycleNo;

            // Get information for loading of  files
            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + WJobCycleNo; 
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, SelectionCriteria);
            textBox4.Text = Flr.TotalReceived.ToString();
            textBox5.Text = (Flr.TotalNotReceived+ Flr.TotalRejected).ToString();
           
            // 
            //Laoding  Trafic Light 
            //

            Gp.ReadParametersSpecificId(WOperator, "603", "9", "", ""); // < is Green 
            int QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "9", "", ""); // > is Red 
            int QualityRange2 = (int)Gp.Amount;

            if ((Flr.TotalNotReceived + Flr.TotalRejected) <= QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if ((Flr.TotalNotReceived + Flr.TotalRejected) >= QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            if ((Flr.TotalNotReceived + Flr.TotalRejected) > QualityRange1 & (Flr.TotalNotReceived + Flr.TotalRejected) < QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }
             
             //Loading - Chart1

            int[] yvalues1 = { Flr.TotalReceived, (Flr.TotalNotReceived + Flr.TotalRejected) };
            string[] xvalues1 = { "Yes", "No" };

            // Set series members names for the X and Y values 
            chart2.Series[0].Points.DataBindXY(xvalues1, yvalues1);

        }

        // On Row Enter the second 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WFileId = (string)rowSelected.Cells[2].Value;
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
 

        //Show LOADED 
        private void ShowLoaded()
        {
            WSelectionCriteria =
              " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + WJobCycleNo + " AND ReceivedCode = '00' ";
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, WSelectionCriteria);

            if (Flr.TotalReceived == 0)
            {
                Form2 MessageForm = new Form2("There are no items to show.");
                MessageForm.ShowDialog();

                return;

            }
            labelLoadingGridHeader.Show();
            labelLoadingGridHeader.Text = "RECEIVED FILES";
            dataGridView2.DataSource = Flr.TableFilesLoadingRegister.DefaultView;

            dataGridView2.Columns[0].Width = 80; // ITMXJobCycle
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 50; // BankId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 130; // FileId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 100; // ExpectedDate
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 100; // ReceivedDate
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 100; // Status
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (dataGridView2.Rows.Count == 0)
            {
                labelLoadingGridHeader.Hide();
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

        //Show NOT Loaded  
        private void ShowNotLoaded()
        {
            WSelectionCriteria =
              " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + WJobCycleNo + " AND ReceivedCode <> '00' ";
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, WSelectionCriteria);

            if (Flr.TotalNotReceived == 0)
            {
                Form2 MessageForm = new Form2("There are no items to show.");
                MessageForm.ShowDialog();

                return;
            }

            labelLoadingGridHeader.Show();

            labelLoadingGridHeader.Text = "NOT RECEIVED FILES";
            dataGridView2.DataSource = Flr.TableFilesLoadingRegister.DefaultView;

            dataGridView2.Columns[0].Width = 80; // ITMXJobCycle
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 50; // BankId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 130; // FileId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 100; // ExpectedDate
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 100; // ReceivedDate
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 100; // Status
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (dataGridView2.Rows.Count == 0)
            {
                labelLoadingGridHeader.Hide();
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

        //LOADED
        private void buttonLoaded_Click(object sender, EventArgs e)
        {
            button5.Show();
            label1.Show();
            button3.Show();
            button4.Show();

            ShowLoaded(); 
        }
//NOT LOADED FILES
        private void buttonNotLoaded_Click_1(object sender, EventArgs e)
        {
            button5.Show();
            label1.Show();
            button3.Show();
            button4.Show();

            ShowNotLoaded(); 
        }

        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//History 
        private void button5_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE Operator='" + WOperator + "' AND FileID ='" + WFileId +"'" ;
            Flr.ReadLoadingFilesRegisterHistoryFillTable(WOperator, WSelectionCriteria); 
            //
            Form78b NForm78b; 
            string WHeader = "SELECTED FILE " ;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Flr.TableFilesLoadingRegisterHistory, WHeader, "Form200bITMX");
            NForm78b.ShowDialog();
        }
//Send Email 
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Scope and containts of the email will be discussed with ITMX.");
            return; 
        }
//Send SMS 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Scope and containts of the email will be discussed with ITMX.");
            return;
        }
    }
}

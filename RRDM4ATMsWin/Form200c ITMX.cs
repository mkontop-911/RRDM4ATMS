using System;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
// Alecos
using System.Diagnostics;


namespace RRDM4ATMsWin
{
    public partial class Form200cITMX : Form
    {
        // Variables

        int WJobCycleNo; 

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

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
        string WBankId; 
  
        string WMatchingRunningGroup; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200cITMX(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InMatchingRunningGroup, string InBankId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
    
            WMatchingRunningGroup = InMatchingRunningGroup;
            WBankId = InBankId; 

            InitializeComponent();
         
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            //*****************************************
            //
            labelStep1.Text = "Files Loading Status For " + WBankId;
            //
            //*****************************************

            textBoxMsgBoard.Text = "Loading Status. Use buttons for other Info. ";

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

        }

        // ROW ENTER FOR JOB CYCLE 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WJobCycleNo = (int)rowSelected.Cells[0].Value;

            label40.Text = "LOADING STATUS FOR JOB CYCLE NO :.. " + WJobCycleNo;

            ShowFilesForThisBank(WJobCycleNo);      
        }

        // On Row Enter the second 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WFileId = (string)rowSelected.Cells[2].Value;
        }
      
        //Show LOADED 
        private void ShowFilesForThisBank(int InJobCycleNo)
        {
            // Get information for loading of  files
            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND ITMXJobCycle=" + InJobCycleNo + " AND BankId = '" + WBankId +"'";
            Flr.ReadLoadingFilesRegisterFillTable(WOperator, SelectionCriteria);

            if (Flr.TotalReceived == 0)
            {
                Form2 MessageForm = new Form2("There are no items to show.");
                MessageForm.ShowDialog();

                return;

            }
            labelMatchingGridHeader.Show();
            labelMatchingGridHeader.Text = "RECEIVED FILES";
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

        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//History 
        private void button5_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE Operator='" + WOperator + "' AND FileID ='" + WFileId +"'";
            Flr.ReadLoadingFilesRegisterHistoryFillTable(WOperator, WSelectionCriteria); 
            //
            Form78b NForm78b; 
            string WHeader = "SELECTED FILE " ;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Flr.TableFilesLoadingRegisterHistory, WHeader, "Form200bITMX");
            NForm78b.ShowDialog();
        }

    }
}

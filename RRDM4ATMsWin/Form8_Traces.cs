using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form8_Traces : Form
    {
        //Form16 NForm16;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        //    string WOperator;
     
        string WCriticalProcess;
     
        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;

        public Form8_Traces(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

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

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelStep1.Text = "Critical Processes - the traces are shown";

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            // LOAD CUT OFF DATES
            comboBox1.DataSource = Rjc.GetCut_Off_Dates_List(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            // Traces for Critical Process 
            Gp.ParamId = "352";
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            //comboBox2.Text = "KontoLoadJournal"; 

            textBoxMsgBoard.Text = " Make your selection and press button Show";

            label1.Hide();
            panel2.Hide();

        }
        // On Load form 
        private void Form8_Load(object sender, EventArgs e)
        {

        }
        // SHOW 
        private void button2_Click(object sender, EventArgs e)
        {

            WCriticalProcess = comboBox2.Text;

            DateTime WCutOff = DateTime.Parse(comboBox1.Text);         

            Pt.ReadPerformanceTraceAndFillTableForTraces(WOperator, WCutOff, WCriticalProcess);

            dataGridView1.DataSource = Pt.TableTraces.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Records to Show");
                return;
            }
            else
            {
                //dataGridView1.Show(); 
            }

            // System performance For Model ATM AB104
            // 

            label1.Show();
            panel2.Show();

            //ModelAtm = comboBox1.Text;

            label1.Text = "Traces for Process: " + WCriticalProcess;

            dataGridView1.Columns[0].Width = 80; // RecordNo;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 80; // Mode;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 80; // BankId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 80; // ProcessNm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 80; // AtmNo
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = false;

            dataGridView1.Columns[5].Width = 80; // Cut_Off_Date;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 100; // StartDT;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderText = "Date and Time";

            dataGridView1.Columns[7].Width = 100; // EndDT
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[7].Visible = false;

            dataGridView1.Columns[8].Width = 60; // Duration_Sec
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[8].Visible = false;

            dataGridView1.Columns[9].Width = 60; // Duration Mili 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[9].Visible = false;

            dataGridView1.Columns[10].Width = 60; // Counter
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[10].Visible = false;

            dataGridView1.Columns[11].Width = 1200; // Details
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            int WRecordNo = (int)rowSelected.Cells[0].Value;
            Pt.ReadPerformanceTraceRecNo(WRecordNo);

            textBoxMessageDetails.Text = Pt.Details;
        }
// Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = "TRACES OF CRITICAL Process " + WCriticalProcess;

            string P2 = "";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            Form56R73 ReportATMS73 = new Form56R73(P1, P2, P3, P4, P5);
            ReportATMS73.Show();
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Hide();
            panel2.Hide(); 
        }
    }
}

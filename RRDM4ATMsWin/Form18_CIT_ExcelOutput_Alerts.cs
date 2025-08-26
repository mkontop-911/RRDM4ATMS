using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ExcelOutput_Alerts : Form
    {
       
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass(); 
        //RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId; 
       
        public Form18_CIT_ExcelOutput_Alerts(string InSignedId, int InSignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
       
               // 
         
        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            //WSelectionCriteria = " Where ActiveRecord = 1 AND PassReplCycle = 0 AND NewEstReplDt < @NewEstReplDt "; 
            WSelectionCriteria = ""; 
            Ro.ReadReplActionsAndFillTable(WOperator, WSelectionCriteria, DateTime.Now.Date, DateTime.Now.Date, 3); 
         
            ShowGrid1();

        }
     

        // Row Enter 
       int WSeqNo ;
  
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            //labelCycle.Text = "Loading Cycle : " + WSeqNo.ToString(); 

            Ro.ReadReplActionsSpecific(WSeqNo); 

            textBoxOrdersCycleNo.Text = WSeqNo.ToString(); 

        }

        private void ShowGrid1()
        {
            
            dataGridView1.DataSource = Ro.TableReplOrders.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            dataGridView1.DataSource = Ro.TableReplOrders.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 70; // OrderNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 70; // AtmNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].HeaderText = "Atm No";

            dataGridView1.Columns[2].Width = 130; //  AtmName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].HeaderText = "Atm Name";

            dataGridView1.Columns[3].Width = 70; // ReplCycleNo
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderText = "Repl Cycle";

            dataGridView1.Columns[4].Width = 120; // NeedStatus
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].HeaderText = "Need Status";

            dataGridView1.Columns[5].Width = 90; // OrdersCycleNo
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderText = "Orders Cycle No";
            dataGridView1.Columns[5].Visible = false;
           
            dataGridView1.Columns[6].Width = 100; // CurrentCassettes
            dataGridView1.Columns[6].DefaultCellStyle = style;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[6].HeaderText = "Current Cassettes";

            dataGridView1.Columns[7].Width = 70; //DaysToLast
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderText = "Days To Last";

            dataGridView1.Columns[8].Width = 100; // LastReplDt
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].HeaderText = "Last Repl Dt";

            dataGridView1.Columns[9].Width = 100; // NewEstReplDt
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].HeaderText = "New Est ReplDt";

            dataGridView1.Columns[10].Width = 120; //ToLoadAmount
            dataGridView1.Columns[10].DefaultCellStyle = style;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[10].HeaderText = "To Load Amount";
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//
// print Loading cycles
//
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {

            string P1 = "EXCEL LOADING CYCLES FOR CIT :  " + WCitId;

            string P2 = "";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            Form56R79 ReportATMS79 = new Form56R79(P1, P2, P3, P4, P5);
            ReportATMS79.Show();

        }

/// <summary>
/// Cycle Details 
/// </summary>

        private void buttonCycleDetails_Click(object sender, EventArgs e)
        {
            //string P1 = "REPLENISHMENT ORDERS CYCLE:"+textBoxOrdersCycleNo.Text+" TO CIT:" + WCitId;

            //string P2 = WCitId;
            //string P3 = textBoxOrdersCycleNo.Text;
            //string P4 = WOperator;
            //string P5 = WSignedId;

            //Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            //Report_Repl_Orders.Show();

            string P1 = "REPLENISHMENT ORDERS CYCLE:" + textBoxOrdersCycleNo.Text + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = textBoxOrdersCycleNo.Text;
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }
    }
}

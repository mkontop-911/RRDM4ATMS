using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ExcelOutputCycles : Form
    {
       
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        //string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId; 
       
        public Form18_CIT_ExcelOutputCycles(string InSignedId, int InSignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCitId = InCitId; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
       
                //buttonAddNew.Hide(); 
                    
                //buttonAddNew.Show();
         
        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            Coc.ReadExcelOutputCyclesFillTable(WOperator, WSignedId, WCitId);
            ShowGrid1();

        }
     

        // Row Enter 
       int WSeqNo ;
  
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            //labelCycle.Text = "Loading Cycle : " + WSeqNo.ToString(); 

            Coc.ReadExcelOutputCyclesBySeqNo(WSeqNo);

            textBoxOrdersCycleNo.Text = WSeqNo.ToString(); 


        }

        private void ShowGrid1()
        {
            
            dataGridView1.DataSource = Coc.TableExcelOutputCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Cycles Available!");
                return;
            }

       
            //dataGridView1.Columns[0].Width = 60; // SeqNo
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[0].Visible = true;

            //dataGridView1.Columns[1].Width = 130; // StartDateTm
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].Visible = true;

            //dataGridView1.Columns[2].Width = 130; // FinishDateTm
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[2].Visible = true;

            //dataGridView1.Columns[3].Width = 200; // Description
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[3].Visible = true;

            //dataGridView1.Columns[4].Width = 80; // ExcelErrors
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[4].Visible = true;

            //dataGridView1.Columns[5].Width = 80; // NewAtms
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[5].Visible = true;

            //dataGridView1.Columns[6].Width = 80; // UpdatedATMs
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[6].Visible = true;

            //dataGridView1.Columns[7].Width = 150; // Status
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[8].Width = 140; // UserId
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[8].Visible = true;

            //dataGridView1.Columns[9].Width = 400; // Excel Id
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[9].Visible = true;

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
           
            string P1 = "REPLENISHMENT ORDERS CYCLE:.." + textBoxOrdersCycleNo.Text + " TO CIT:" + WCitId;

            string P2 = WCitId;
            string P3 = textBoxOrdersCycleNo.Text;
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R69ATMS_Repl_Orders Report_Repl_Orders = new Form56R69ATMS_Repl_Orders(P1, P2, P3, P4, P5);
            Report_Repl_Orders.Show();
        }

        private void buttonShowCycle_Click(object sender, EventArgs e)
        {
          
            string WStage = "";

            if (Coc.ProcessStage == 3) WStage = "Completed";
            if (Coc.ProcessStage == 2) WStage = "At Authoriser";
            if (Coc.ProcessStage == 1) WStage = "At Maker";


            if (Coc.ProcessStage != 3)
            {
                MessageBox.Show("Cycle Is not completed! " + Environment.NewLine
                    + "It is at the stage of " + WStage);
                return;
            }

            // Process
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Form273_AUDI NForm273_AUDI;

            NForm273_AUDI = new Form273_AUDI(WSignedId, WSignRecordNo, WOperator, WCitId, WSeqNo, "ATMsInNeed");
            NForm273_AUDI.ShowDialog();
        }
    }
}

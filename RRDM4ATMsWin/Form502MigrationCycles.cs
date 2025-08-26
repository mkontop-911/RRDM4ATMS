using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502MigrationCycles : Form
    {
        RRDMMigrationCycles Mc = new RRDMMigrationCycles();
      
        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        //string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode; 
       
        public Form502MigrationCycles(string InSignedId, int InSignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // 1 = view, 2 = Update

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

            labelUserId.Text = InSignedId;

            if (WMode == 1)
            {
                buttonAddNew.Hide(); 
              
            }
            if (WMode == 2)
            {
                buttonAddNew.Show();
            }     

        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 

           

            Mc.ReadMigrationCyclesFillTable(WOperator); 

            ShowGrid1();

        }
     
        // Row Enter 
       int WSeqNo ;
  
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            labelCycle.Text = "Migration CycleNo : " + WSeqNo.ToString();

            Mc.ReadMigrationCyclesById(WOperator, WSeqNo); 
            
        }

        private void ShowGrid1()
        {
            
            dataGridView1.DataSource = Mc.TableMigrationCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

       
            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 130; // StartDateTm
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 130; // FinishDateTm
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 200; // Description
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 80; // ExcelErrors
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 80; // NewAtms
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 80; // UpdatedATMs
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = true;

            dataGridView1.Columns[7].Width = 150; // Status
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 140; // UserId
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].Visible = true;

            dataGridView1.Columns[9].Width = 400; // Excel Id
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].Visible = true;

        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

// ADD New Cycle 
        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            // Read last and warn

            // Create new 

            Mc.ReadLastReconcMigrationCycle(WOperator);
          
            if (Mc.RecordFound & Mc.ProcessStage == 2 || Mc.RecordFound ==false)
            {
                // Last Excel Read .. You cannot open a new Cycle
                Mc.StartDateTm = DateTime.Now;
                Mc.FinishDateTm = NullPastDate;
            
                Mc.ProcessStage = 0;

                Mc.UserId = WSignedId; 

                Mc.Operator = WOperator; 

                int NewCycle = Mc.InsertNewMigrationCycle();

                Mc.ReadMigrationCyclesById(WOperator, NewCycle);

                Mc.Description = "Migration on :" + Mc.StartDateTm.ToShortDateString();

                Mc.UpdateMigrationCycle(NewCycle);
        
                //int WRowGrid1 = dataGridView1.SelectedRows[0].Index;

                Form502_Load(this, new EventArgs());

                //dataGridView1.Rows[WRowGrid1].Selected = true;
                //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));


            }
            else
            {
                MessageBox.Show("Last Cycle still active. You cannot open a new one.");
                return; 
            }

        }
//
// print cycle migrated 
//
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {
            if (Mc.ProcessStage == 2)
            {
                string P1 = "Migrated ATMs Results ";

                string P2 = WSeqNo.ToString();
                string P3 = "Third Par";
                string P4 = WOperator;
                string P5 = WSignedId;

                Form56R58ATMS ReportATMS58 = new Form56R58ATMS(P1, P2, P3, P4, P5);
                ReportATMS58.Show();
            }
            else
            {
                MessageBox.Show("This Migration Cycle Not Completed Yet!");
                return; 
            }

          
        }
    }
}

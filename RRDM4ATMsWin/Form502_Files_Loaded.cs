using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502_Files_Loaded : Form
    {
        RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
        RRDMReconcFileMonitorLog Rg = new RRDMReconcFileMonitorLog(); 
        RRDMGasParameters Gp = new RRDMGasParameters(); 
  
        string WSignedId;
        int WSignRecordNo;
        string W_Application; 
        string WOperator;
       
        public Form502_Files_Loaded(string InSignedId, int SignRecordNo, string InOperator, string InApplication)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            W_Application = InApplication; 
            WOperator = InOperator;
          
            InitializeComponent();
            
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

          
        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            string Filter1 = " Operator = '" + WOperator + "' AND Enabled = 1 AND (Right(SourceFileId, 5) <> 'TWINS' AND Right(SourceFileId, 5) <> 'TWIN')";
            Mf.ReadReconcSourceFilesToFillDataTable(Filter1); 
            
            dataGridView1.DataSource = Mf.SourceFilesDataTable.DefaultView;

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 60; // FileSeq"
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 170; // SourceFile_ID
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; // OriginSystem
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Type
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = false;

        }
        // Ro
        private void dataGrid1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            int WSeqNo = (int)rowSelected.Cells[0].Value;
            Mf.ReadReconcSourceFilesBySeqNo(WSeqNo); 
          
            Rg.ReadLoadedFiles_Fill_Table_By_FileId(WOperator, Mf.SourceFileId);

            if (Rg.DataTableFileMonitorLog.Rows.Count==0)
            {
                MessageBox.Show("NO FILES FOR THIS SELECTION");
                return; 
            }

            dataGridView2.DataSource = Rg.DataTableFileMonitorLog.DefaultView;

            //DataTableFileMonitorLog.Columns.Add("SeqNo", typeof(int));
            //DataTableFileMonitorLog.Columns.Add("FileName", typeof(string));
            //DataTableFileMonitorLog.Columns.Add("Status", typeof(string));

            //DataTableFileMonitorLog.Columns.Add("DateTimeReceived", typeof(string));
            //DataTableFileMonitorLog.Columns.Add("DateExpected", typeof(string));
            //DataTableFileMonitorLog.Columns.Add("Diff InDays", typeof(string));

            //DataTableFileMonitorLog.Columns.Add("LineCount", typeof(int));
            //DataTableFileMonitorLog.Columns.Add("stpErrorText", typeof(string));

            dataGridView2.Columns[0].Width = 60; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 400; // File Nme 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           
            dataGridView2.Columns[2].Width = 80; // Status 
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 120; // DateTimeReceived
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 80; // DateExpected
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 80; // Diff InDays
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 80; // LineCount
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            int WSeqNo = (int)rowSelected.Cells[0].Value;
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}

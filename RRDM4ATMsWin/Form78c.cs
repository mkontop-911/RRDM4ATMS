using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78c : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 

        // Define the data table 
        public DataTable WDataTable = new DataTable();
        public DataTable WDataTableCompressed = new DataTable();

        //public DataTable WTableWorkingFile01 = new DataTable();
        //public DataTable WTableWorkingFile02 = new DataTable();
        //public DataTable WTableWorkingFile03 = new DataTable();

        public int WPostedNo;
        public int UniqueIsChosen;

        public int WMaskRecordId; 

        public int WSelectedRow = 0;

        ////bool WithDate;
        //string Gridfilter; 
    
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WHeader;
        string WFromForm;
        string WMatchingCateg; 

        public Form78c(string InSignedId, int InSignRecordNo, string InOperator, 
            DataTable InDataTable, DataTable InDataTableCompressed,
            string InHeader, string InFromForm, string InMatchingCateg)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WHeader = InHeader;
            WFromForm = InFromForm;
            WMatchingCateg = InMatchingCateg; 

            WDataTable = new DataTable();
            WDataTable.Clear();

            WDataTableCompressed = new DataTable();
            WDataTableCompressed.Clear(); 

            WDataTable = InDataTable;
            WDataTableCompressed = InDataTableCompressed;

            //WTableWorkingFile01 = InTableWorkingFile01;
            //WTableWorkingFile02 = InTableWorkingFile02;
            //WTableWorkingFile03 = InTableWorkingFile03;

            InitializeComponent();

            labelNumberOfRecords.Text = "Number of Records = " + WDataTable.Rows.Count.ToString(); 

            labelWhatGrid.Text = WHeader; 

            if (WOperator == "ITMX")
            {
                button1.Hide(); 
            }
            if (WFromForm == "Form78d")
            {
                button1.Hide();
                label1.Hide();
                textBoxTrace.Hide();
                button3.Hide();
            }
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = WDataTable.DefaultView;
            // SHOW GRID
     
            ShowGridLeft();
          
        }

        // Show Grid Left 
        public void ShowGridLeft()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";
           
            dataGridView1.Columns[0].Width = 80; // "UserId"
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;  

            dataGridView1.Columns[1].Width = 80; // "OriginSeqNo"
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 90; // TransDate
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        
            dataGridView1.Columns[3].Width = 150; //  WCase
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 50; // Type
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 70; // DublInPos
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[6].Width = 70; // InPos
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 70; // NotInPos
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; // TerminalId
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
     
            dataGridView1.Columns[9].Width = 90; // TraceNo
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[9], ListSortDirection.Ascending);

            dataGridView1.Columns[10].Width = 90; // AccNo
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 110; //"TranAmt"
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[12].Width = 100; // "MatchingCateg"   
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[13].Width = 70; //"RMCycle"
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[14].Width = 300; // FileId
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[14].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }

        DateTime WTranDate;
        int WTraceNo;
        string WAtmNo;
        //string WMask;
        string WCase;
        int WRMCycle; 

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WTranDate = (DateTime)dataGridView1.Rows[e.RowIndex].Cells["TransDate"].Value;

            WCase = (string)dataGridView1.Rows[e.RowIndex].Cells["WCase"].Value;

            WTraceNo = (int)dataGridView1.Rows[e.RowIndex].Cells["TraceNo"].Value;

            //WMask = "";

            WAtmNo = (string)dataGridView1.Rows[e.RowIndex].Cells["TerminalId"].Value;

            WRMCycle = (int)dataGridView1.Rows[e.RowIndex].Cells["RMCycle"].Value;

            //WCase = (string)rowSelected.Cells[2].Value;

            //WTraceNo = (int)rowSelected.Cells[7].Value;

            // WMask = "";

            //WAtmNo = (string)rowSelected.Cells[6].Value;

            textBoxTrace.Text = WTraceNo.ToString();

        }
        // Show Files 
        private void button3_Click_1(object sender, EventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles;

            string WSelectionCriteria = " WHERE "
                                        +"TerminalId ='" + WAtmNo +"'"
                                        + " AND MatchingAtRMCycle =" + WRMCycle 
                                        + " AND (TraceNoWithNoEndZero =" + WTraceNo + " OR RRNumber = " + WTraceNo + ")"
                                       ;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,2);

            NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId );

            NForm78d_AllFiles.ShowDialog(); 
        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// List of Unmatched - Compressed 
        private void button1_Click(object sender, EventArgs e)
        {
            Form78d NForm78d;

            string WHeader = "LIST OF Matched And  UnMatched And Dublicate - Summary";
            NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator, 
                WDataTableCompressed, WHeader, "Form78cSummary", WMatchingCateg, WRMCycle);

            NForm78d.ShowDialog();
        }
// UPDATE Found discrepancies and print 
        private void button2_Click(object sender, EventArgs e)
        {

            string P1 = "Discrepancies ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R67ATMS ReportATMS67 = new Form56R67ATMS(P1, P2, P3, P4, P5);
            ReportATMS67.Show();
        }
// Show Working 01 = Journal 
        //private void button3_Click(object sender, EventArgs e)
        //{
            
        //    Form78d NForm78d;

        //    string WHeader = "LIST OF Journal Transactions";
        //    NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator, WTableWorkingFile01,
        //        WHeader, "Form78cTxns");

        //    NForm78d.Show();
        //}
// List of IST 
        //private void button4_Click(object sender, EventArgs e)
        //{
        //    Form78d NForm78d;

        //    string WHeader = "LIST OF IST Transactions";
        //    NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator, WTableWorkingFile02,
        //        WHeader, "Form78cTxns");

        //    NForm78d.Show();
        //}

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    Form78d NForm78d;

        //    string WHeader = "LIST OF Fiserv Transactions";
        //    NForm78d = new Form78d(WSignedId, WSignRecordNo, WOperator, WTableWorkingFile03,
        //              WHeader, "Form78cTxns");

        //    NForm78d.Show();
        //}

        // List of Fiserv

    }
}

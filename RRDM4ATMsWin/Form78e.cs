using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78e : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();
        RRDMUserSignedInRecords Usr = new RRDMUserSignedInRecords();


        // Define the data table 
       

        public int WPostedNo;
        public int UniqueIsChosen;

        public int WMaskRecordId; 

        public int WSelectedRow = 0;

        ////bool WithDate;
        //string Gridfilter; 
    
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WReconcCycleNo;
        DateTime WDate; 
        string WHeader;
        int WMode;
       // WReconcCycleNo, WCut_Off_Date, InHeader

        public Form78e(string InSignedId, int InSignRecordNo, string InOperator, int InReconcCycleNo,
            DateTime InDate, string InHeader, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WReconcCycleNo = InReconcCycleNo;
            WDate = InDate; 
            WHeader = InHeader;
            WMode = InMode;
            // Mode = 1 Show Bad and the ugly
            // Mode = 2 Show the Sign In Users
            // Mode = 3 Show the deposits with same Repl Cycle but different Fuid

            InitializeComponent();

            labelWhatGrid.Text = WHeader; 

            if (WMode == 3)
            {
                buttonExportToExcel.Show();

            }
            else
            {
                buttonExportToExcel.Hide();
            }

          
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WMode == 1)
            {
               
                Jt.ReadDataTableForResponseCodes(WReconcCycleNo);

                textBoxGood.Text = Jt.SuccessfulTotal.ToString("#,##0");
                textBoxBad.Text = (Jt.TotalTXNS- Jt.SuccessfulTotal).ToString("#,##0");
                int Tot1 = Jt.TotalTXNS - Jt.SuccessfulTotal;
                int Tot2 = Jt.TotalTXNS;
                textBoxPercent.Text= ((double)Tot1 / Tot2).ToString("#,##0.00");
               
                textBoxGoodTrasfers.Text = Jt.GoodTransfersTotal.ToString("#,##0");
                textBoxBadTransfers.Text = Jt.BadTransfersTotal.ToString("#,##0");

                textBoxTrasfersBadPerce.Text = ((double)Jt.BadTransfersTotal / Jt.GoodTransfersTotal).ToString("#,##0.00");

                //Jt.FlexTimeOutTotal
                textBox1.Text = Jt.FlexTimeOutTotal.ToString("#,##0");

                dataGridView1.DataSource = Jt.DataTableJournalsSourceStats_2.DefaultView;
                // SHOW GRID

                ShowGrid1();
            }

            if (WMode == 3)
            {

                Jt.ReadDataTableForWrongReplAtDeposits(WReconcCycleNo);

                //textBoxGood.Text = Jt.SuccessfulTotal.ToString("#,##0");
                //textBoxBad.Text = (Jt.TotalTXNS - Jt.SuccessfulTotal).ToString("#,##0");
                //int Tot1 = Jt.TotalTXNS - Jt.SuccessfulTotal;
                //int Tot2 = Jt.TotalTXNS;
                //textBoxPercent.Text = ((double)Tot1 / Tot2).ToString("#,##0.00");

                //textBoxGoodTrasfers.Text = Jt.GoodTransfersTotal.ToString("#,##0");
                //textBoxBadTransfers.Text = Jt.BadTransfersTotal.ToString("#,##0");

                //textBoxTrasfersBadPerce.Text = ((double)Jt.BadTransfersTotal / Jt.GoodTransfersTotal).ToString("#,##0.00");

                ////Jt.FlexTimeOutTotal
                //textBox1.Text = Jt.FlexTimeOutTotal.ToString("#,##0");

                dataGridView1.DataSource = Jt.DataTableJournalsWrongDeposits.DefaultView;
                // SHOW GRID

               // ShowGrid1();
            }


            if (WMode == 2)
            {

                Usr.ReadOpenUsers(WOperator);

                dataGridView1.DataSource = Usr.OpenUsersSelected.DefaultView;
                // SHOW GRID
                panel3.Hide(); 

                ShowGrid2();
            }

        }

        // Show Grid 1
        public void ShowGrid1()
        {
            
            dataGridView1.Columns[0].Width = 50; // ResponseCode
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 200; // Case Description
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           
            dataGridView1.Columns[2].Width = 200; //  Transaction Type
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 70; // Count
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        // Show Grid 2
        public void ShowGrid2()
        {
            
            dataGridView1.Columns[0].Width = 70; // UserId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 170; // UserName
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 140; //  DtTmIn
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 140; // DtTmOut
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
           
        }


// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Select this unique 
        private void button1_Click(object sender, EventArgs e)
        {
            UniqueIsChosen = 1; 
            this.Dispose(); 
        }
// UPDATE Found discrepancies and print 
        private void button2_Click(object sender, EventArgs e)
        {

            //string P1 = "Discrepancies ";

            //string P2 = "Second Par";
            //string P3 = "Third Par";
            //string P4 = WOperator;
            //string P5 = WSignedId;

            //Form56R67ATMS ReportATMS67 = new Form56R67ATMS(P1, P2, P3, P4, P5);
            //ReportATMS67.Show();
        }

        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Deposits_Bad" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Jt.DataTableJournalsWrongDeposits, WorkingDir, ExcelPath);
        }
    }
}

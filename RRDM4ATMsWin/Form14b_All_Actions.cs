using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
namespace RRDM4ATMsWin
{
    public partial class Form14b_All_Actions : Form
    {
        
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
       
       
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WNumberOfRecords; 

        string WSignedId;
        string WOperator;

        DataTable WTableFromActions;

        int WMode; 

        public Form14b_All_Actions(string InSignedId, string InOperator,
                               DataTable InTxnsFromActions, int InMode)         
        {
            WSignedId = InSignedId;
            WOperator = InOperator;

            WTableFromActions = InTxnsFromActions;
            WMode = InMode; // if 1 = Txns to be created
                            // if 2 = Actions taken - Big data table 
                            // if 3 = Actions taken - Small data table 
                            // if 4 = Actions taken - Small data table with Manual GL

            InitializeComponent();

            WNumberOfRecords = WTableFromActions.Rows.Count;

            textBoxTotal.Text = WNumberOfRecords.ToString();

            labelFormHeader.Text = "";

            if (WMode == 1)
            {
                labelFormHeader.Text = "Result of Actions";
                labelGridHeader.Text = "ACCOUNTING TRANSACTIONS"; 
            }
            if (WMode == 2)
            {
                labelFormHeader.Text = "Work Progress";
                labelGridHeader.Text = "ACTIONS LIST";
            }
            if (WMode == 3)
            {
                labelFormHeader.Text = "Work Progress";
                labelGridHeader.Text = "ACTIONS LIST";
            }
            if (WMode == 4)
            {
                labelFormHeader.Text = "Work Progress";
                labelGridHeader.Text = "ACTIONS LIST";
            }

        }

        // 
        private void Form14b_All_Actions_Load(object sender, EventArgs e)
        {

            dataGrid1.DataSource = WTableFromActions.DefaultView;

            if (WMode == 1)
            {
                ShowGrid_1();
            }
            if (WMode == 2)
            {
                ShowGrid_2();
            }
            if (WMode == 3)
            {
                ShowGrid_3();
            }

            if (WMode == 4)
            {
                ShowGrid_4();
            }

        }


        // FINISH 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGrid1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGrid1.Rows[e.RowIndex];
            // WSeqNoLoadedFile = (int)rowSelected.Cells[0].Value;
            labelFormHeader.Text = "";
        }

        private void ShowGrid_1()
        {
            dataGrid1.Columns[0].Width = 60; // SeqNumber
            dataGrid1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[1].Width = 60; // ActionId
            dataGrid1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[2].Width = 60; // Occurance 
            dataGrid1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[3].Width = 330; // ActionNm
            dataGrid1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[4].Width = 60; // Branch
            dataGrid1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[5].Width = 120; // AccNo
            dataGrid1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[6].Width = 50; // DR/CR
            dataGrid1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[7].Width = 120; // AccName
            dataGrid1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[8].Width = 90; // Amount
            dataGrid1.Columns[8].DefaultCellStyle.Format = "#,##0.00";
            dataGrid1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGrid1.Columns[9].Width = 350; // Description
            dataGrid1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[10].Width = 60; // Stage 
            dataGrid1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void ShowGrid_2()
        {

            dataGrid1.Columns[0].Width = 60; // SeqNumber
            dataGrid1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[1].Width = 120; // CardNo
            dataGrid1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[2].Width = 120; // AccountNo
            dataGrid1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[3].Width = 120; // Amount
            dataGrid1.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            dataGrid1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGrid1.Columns[4].Width = 120; // TransDate
            dataGrid1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[5].Width = 300; // ActionNm
            dataGrid1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[6].Width = 50; // Mask
            dataGrid1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[7].Width = 80; // Maker
            dataGrid1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[8].Width = 80; // Auth
            dataGrid1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void ShowGrid_3()
        {

            //TableActionOccurances_Small.Columns.Add("SeqNo", typeof(int));
            //TableActionOccurances_Small.Columns.Add("ActionId", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Occurance", typeof(string));
            //TableActionOccurances_Small.Columns.Add("ActionNm", typeof(string));
            //TableActionOccurances_Small.Columns.Add("ActionAtDateTime", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Amount", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Branch_1", typeof(string));
            //TableActionOccurances_Small.Columns.Add("AccNo_1", typeof(string));
            //TableActionOccurances_Small.Columns.Add("AccName_1", typeof(string));
            //TableActionOccurances_Small.Columns.Add("DR/CR_1", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Description_1", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Branch_2", typeof(string));
            //TableActionOccurances_Small.Columns.Add("AccNo_2", typeof(string));
            //TableActionOccurances_Small.Columns.Add("AccName_2", typeof(string));
            //TableActionOccurances_Small.Columns.Add("DR/CR_2", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Description_2", typeof(string));

            dataGrid1.Columns[0].Width = 60; // SeqNo
            dataGrid1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGrid1.Columns[0].Visible = false; 

            dataGrid1.Columns[1].Width = 50; // ActionId
            dataGrid1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid1.Columns[2].HeaderText = "Act ID";

            dataGrid1.Columns[2].Width = 50; // Occurance
            dataGrid1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGrid1.Columns[2].HeaderText = "Occur";

            dataGrid1.Columns[3].Width = 240; // ActionNm
            dataGrid1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[4].Width = 120; // ActionAtDateTime
            dataGrid1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[5].Width = 100; // Amount
            dataGrid1.Columns[5].DefaultCellStyle.Format = "#,##0.00";
            dataGrid1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGrid1.Columns[6].Width = 60; // Branch_1
            dataGrid1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[7].Width = 90; // AccNo_1
            dataGrid1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[8].Width = 70; // AccName_1
            dataGrid1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[9].Width = 60; // DR/CR_1
            dataGrid1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[10].Width = 240; // Description_1
            dataGrid1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[11].Width = 60; // Branch_2
            dataGrid1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[12].Width = 90; // AccNo_2
            dataGrid1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[13].Width = 70; // AccName_2
            dataGrid1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[14].Width = 60; // DR/CR_2
            dataGrid1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[15].Width = 240; // Description_2
            dataGrid1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void ShowGrid_4()
        {

            //TableActionOccurances_Small.Columns.Add("SeqNo", typeof(int));
            //TableActionOccurances_Small.Columns.Add("ActionId", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Occurance", typeof(string));
            //TableActionOccurances_Small.Columns.Add("ActionNm", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Action Reason", typeof(string));
            //TableActionOccurances_Small.Columns.Add("ActionDateTime", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Terminal", typeof(string));
            //TableActionOccurances_Small.Columns.Add("CardNo", typeof(string));
            //TableActionOccurances_Small.Columns.Add("AccNo", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Amount", typeof(string));
            //TableActionOccurances_Small.Columns.Add("TraceNo", typeof(string));
            //TableActionOccurances_Small.Columns.Add("RRNumber", typeof(string));
            //TableActionOccurances_Small.Columns.Add("TransDate", typeof(string));
            //TableActionOccurances_Small.Columns.Add("Stage", typeof(string));

            dataGrid1.Columns[0].Width = 60; // SeqNo
            dataGrid1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGrid1.Columns[0].Visible = false;

            dataGrid1.Columns[1].Width = 60; // ActionId
            dataGrid1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[2].Width = 60; // Occurance
            dataGrid1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGrid1.Columns[3].Width = 240; // ActionNm
            dataGrid1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[4].Width = 180; // Action Reason
            dataGrid1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[5].Width = 120; // ActionDateTime
            dataGrid1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[6].Width = 70; // Terminal
            dataGrid1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGrid1.Columns[7].Width = 90; // CardNo
            dataGrid1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
// Export to excel
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string YYYY = DateTime.Now.Year.ToString();
            string MM = DateTime.Now.Month.ToString();
            string DD = DateTime.Now.Day.ToString();
            string Min = DateTime.Now.Minute.ToString();

            //MessageBox.Show("Excel will be created in RRDM Working Directory");
            string Id = labelGridHeader.Text;

            string ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(WTableFromActions, WorkingDir, ExcelPath);
        }
    }
}

using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form271View_Table : Form
    {
     
        public DataTable WTempTable = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int WSelectedRow = 0;
        
        string WHeading;
        int WWhatGrid; 
       
        public Form271View_Table(string InHeading, DataTable InTempTable, int InWhatGrid )
        {
           
            WHeading = InHeading;
            WTempTable = InTempTable;
            WWhatGrid = InWhatGrid;
                       // 1: Comes for POS for investigating Adjustments

            InitializeComponent();

            labelHeading.Text = WHeading; 

            if (WWhatGrid == 1) ShowGrid_1();

        }
       
        // On ROW ENTER   

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

        //    WUniqueRecordId = (int)rowSelected.Cells[0].Value;

        }

        // Show Grid 

        public void ShowGrid_1()
        {

            dataGridView1.DataSource = WTempTable.DefaultView;

            if (WTempTable.Rows.Count == 0)
            {
                MessageBox.Show("Nothing To Show");

                this.Dispose();
                return; 
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //dataGridView1.Columns[0].Width = 60; // RecordId
            //dataGridView1.Columns[0].Name = "RecordId";
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 40; // Select
            //dataGridView1.Columns[1].Name = "Select";
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[2].Width = 50; // mask
            //dataGridView1.Columns[2].Name = "MatchMask";
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[3].Width = 70; // ActionType
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[4].Width = 100; // ActionDesc
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[5].Width = 60; // Settled
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[6].Width = 100; // date
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[7].Width = 140; // Descr
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[8].Width = 40; // Ccy
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[9].Width = 80; // Amount
            //dataGridView1.Columns[9].DefaultCellStyle = style;
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[9].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            //dataGridView1.Columns[10].Width = 60; // MetaExceptionId
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[11].Width = 90; // Card
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    //WSeqNo = (int)rowSelected.Cells[0].Value;
            //    bool WSelect = (bool)row.Cells[1].Value;
            //    string WActionType = (string)row.Cells[3].Value;

            //    if (WSelect == true & WActionType == "4")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }
            //    if (WSelect == true & WActionType == "1")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.Beige;
            //        row.DefaultCellStyle.ForeColor = Color.Black;

            //    }
            //    if (WSelect == false)
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //        row.DefaultCellStyle.ForeColor = Color.Black;
            //    }

            //}
        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        
    }
}


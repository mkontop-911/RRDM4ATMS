using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form78b : Form
    {
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        // Define the data table 
        public DataTable WTransToBePostedSelected = new DataTable();

        public int WPostedNo;
        public int UniqueIsChosen; 

        public int WSelectedRow = 0;

        ////bool WithDate;
        //string Gridfilter; 
    
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WHeader;
        string WFromForm; 



        public Form78b(string InSignedId, int InSignRecordNo, string InOperator, DataTable InTransToBePostedSelected, string InHeader, string InFromForm)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WHeader = InHeader;
            WFromForm = InFromForm; 

            WTransToBePostedSelected = new DataTable();
            WTransToBePostedSelected.Clear();

            WTransToBePostedSelected = InTransToBePostedSelected; 

            InitializeComponent();

            labelWhatGrid.Text = WHeader; 
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            // SHOW GRID

            dataGridView1.DataSource = WTransToBePostedSelected.DefaultView;

            if ( WFromForm == "Form502b" )
            {
                dataGridView1.Columns[0].Width = 50; // 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[1].Width = 200; // 
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            }
            else
            {
                dataGridView1.Columns[0].Width = 50; // 
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[1].Width = 50; // 
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[2].Width = 50; // 
            }


            //dataGridView1.Columns[3].Width = 80; //
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[4].Width = 80; //
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[5].Width = 80; //
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[6].Width = 80; //

            //dataGridView1.Columns[7].Width = 100; // 

            //dataGridView1.Columns[8].Width = 60; //

            //dataGridView1.Columns[9].Width = 60; //
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

// On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WPostedNo = (int)rowSelected.Cells[0].Value;

            labelSelected.Text = "Selected Item : " + WPostedNo.ToString();

            //WSelectedRow = e.RowIndex;

           
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

    }
}

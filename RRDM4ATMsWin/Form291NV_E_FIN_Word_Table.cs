using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form291NV_E_FIN_Word_Table : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        // Define the data table 
        public DataTable WDataTable = new DataTable();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 

        public int WPostedNo;
        public int UniqueIsChosen;

        public int WMaskRecordId; 

        public int WSelectedRow = 0;

        //bool FromGrid04;

        //bool FromGrid05;

        //bool FromOwnATMs; 

        ////bool WithDate;
        //string Gridfilter; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WHeader; 

        public Form291NV_E_FIN_Word_Table(string InSignedId, int InSignRecordNo, string InOperator, 
            DataTable InDataTable, string InHeader)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WHeader = InHeader;
        
            WDataTable = new DataTable();
            WDataTable.Clear();

            WDataTable = InDataTable;

            InitializeComponent();

            labelWhatGrid.Text = WHeader;

        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = WDataTable.DefaultView;
            ShowGrid01();
        }

        // Show Grid 01 
        public void ShowGrid01()
        {
            
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2"; 

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 90; // RMCategId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);


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

     

    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data;

namespace RRDM4ATMsWin
{
    public partial class Form2Grid : Form
    {
        // Define the data table 
        public DataTable WGridTable = new DataTable();

        string WHeader; 
        public Form2Grid(string InHeader, DataTable InTable )
        {
            WHeader = InHeader;
            WGridTable = InTable; 
            InitializeComponent();
        }
        // Load form 
        private void Form2MessageBox_Load(object sender, EventArgs e)
        {
          // MessageBox.Show(WMessage); 
            label1.Text = WHeader;
            dataGridView1.DataSource = WGridTable.DefaultView;

            dataGridView1.Columns[0].Width = 220; // 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // Click OK 
        private void OK_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
       

    }
}

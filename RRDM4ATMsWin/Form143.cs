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
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form143 : Form
    {
        Form44 NForm44;
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 
        string WChosenBank;
        int WFunctionNo;

        int WRow;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  bool WPrive;
        public Form143(string InSignedId, int InSignRecordNo, String InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBoxMsgBoard.Text = "Choose a Bank to take action on it or Add a new Bank";
        }

        private void Form143_Load(object sender, EventArgs e)
        {
                     
            string Filter = "Operator = '" + WOperator + "'";
            bANKSBindingSource.Filter = Filter;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);   
            this.bANKSTableAdapter.Fill(this.aTMSDataSet32.BANKS);

            if (WSignedId == "999")
            {
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
                this.bANKSTableAdapter.Fill(this.aTMSDataSet32.BANKS);
            }
            else
            {
            }

            if (dataGridView1.Rows.Count == 0)
            {
                buttonUpdate.Hide();
                button1.Hide();
                textBoxMsgBoard.Text = "Add the seed Bank/Company";
            }
            else
            {
                buttonUpdate.Show();
                button1.Show();
            } 
        }

        // CHOOSE A BANK 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WChosenBank = rowSelected.Cells[0].Value.ToString();
        }

        //
        // GO TO UPDATE CHOSEN BANK 
        //
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;
           
            WFunctionNo = 1;
            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog();
        }
      
        // On Form close 
        void NForm44_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form143_Load(this, new EventArgs());
            dataGridView1.Rows[WRow].Selected = true;
        }

        // GO TO OPEN A NEW BANK

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            WChosenBank = "";

            if (dataGridView1.Rows.Count == 0) // This is the process when Seed Bank will be opened 
            {
                WChosenBank = WOperator ;
            }

            WFunctionNo = 2;

            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog(); ;
        }
        // Delete 
        private void button1_Click(object sender, EventArgs e)
        {
           
         //   WRow = dataGridView1.SelectedRows[0].Index;
            WFunctionNo = 3;

            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog(); ;
        }
        
       

       
    }
}

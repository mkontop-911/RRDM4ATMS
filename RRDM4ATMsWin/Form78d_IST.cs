using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_IST : Form
    {
       
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WOperator;
        string WSignedId;
       
        public Form78d_IST(string InOperator, string InSignedId)
        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            InitializeComponent();
         
        }

        private void Form78b_Load(object sender, EventArgs e)
        {

            
        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
        }

        // SHOW ATM INFO 
        private void buttonShowATM_Click(object sender, EventArgs e)
        {
            RRDM_IST_WebService Ist = new RRDM_IST_WebService();
            int AtmNo; 
            if (int.TryParse(textBoxATMNo.Text, out AtmNo))
            {
            }
            else
            {
                MessageBox.Show(textBoxATMNo.Text, "Please enter a valid number. !" + Environment.NewLine 
                                + " FOR THIS DEMO WHICH IS CONNNECTED TO THE TESTING DB IT MUST BE 1 ");

                return;
            }
          
            Ist.ReadFieldsFromWebService(AtmNo);

            dataGridView1.DataSource = Ist.Table_IST_ATM.DefaultView;
            dataGridView2.DataSource = Ist.Table_IST_Cassettes; 

        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}

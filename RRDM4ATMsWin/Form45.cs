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

namespace RRDM4ATMsWin
{
    public partial class Form45 : Form
    {

     //   FormMainScreen NFormMainScreen;

        int WRowIndex;

        Form46 NForm46;
        int ChosenGroupNo;
        int WFunctionNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  bool WPrive;

        public Form45(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
       

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
        }

        // Load Command 

        private void Form45_Load(object sender, EventArgs e)
        {
            
                string errfilter = "Operator = '" + WOperator + "'";

                groupsBindingSource.Filter = errfilter;
       

                this.groupsTableAdapter.Fill(this.aTMSDataSet52.Groups);

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
           

        }


        // IF TEXT IS CHANGED 

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
                ChosenGroupNo = int.Parse(textBox1.Text);
                string errfilter = "Operator= '" + WOperator + "'" + " AND GroupNo =" + ChosenGroupNo.ToString();

                groupsBindingSource.Filter = errfilter;

                this.groupsTableAdapter.Fill(this.aTMSDataSet52.Groups);

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
           

        }


        // CHOOSE A GROUP 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            textBox9.Text = rowSelected.Cells[0].Value.ToString();
            ChosenGroupNo = int.Parse(textBox9.Text);
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            textBox9.Text = rowSelected.Cells[0].Value.ToString();
            ChosenGroupNo = int.Parse(textBox9.Text);
        }


        //
        // GO TO UPDATE CHOSEN GROUP 
        //

        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("CHOOSE A GROUP FROM TABLE PLEASE");

                return;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            ChosenGroupNo = int.Parse(textBox9.Text);

            WFunctionNo = 1;

            NForm46 = new Form46(WSignedId, WSignRecordNo, WOperator, ChosenGroupNo, WFunctionNo);
            NForm46 . FormClosed +=NForm46_FormClosed;
            NForm46.Show();

          
         //   this.Dispose();
        }

        


        // GO TO OPEN A NEW GROUP

        private void button7_Click(object sender, EventArgs e)
        {
            ChosenGroupNo = 0;

            WFunctionNo = 2;

            NForm46 = new Form46(WSignedId, WSignRecordNo, WOperator, ChosenGroupNo, WFunctionNo);
            NForm46.FormClosed += NForm46_FormClosed;
            NForm46.Show();
            //   this.Hide();
        }

        
       void NForm46_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form45_Load(this, new EventArgs());
        }


        // GO TO DELETE CHOSEN 
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("CHOOSE A GROUP FROM TABLE PLEASE");

                return;
            }

            ChosenGroupNo = int.Parse(textBox9.Text);

            WFunctionNo = 3;

            NForm46 = new Form46(WSignedId, WSignRecordNo, WOperator, ChosenGroupNo, WFunctionNo);
            NForm46 .FormClosed += NForm46_FormClosed;
            NForm46.Show();
        //    this.Dispose(); 
           

        }

        // REFRESED TABLE 

        private void button8_Click(object sender, EventArgs e)
        {
           
                string errfilter = "Operator = '" + WOperator + "'";

                groupsBindingSource.Filter = errfilter;

                this.groupsTableAdapter.Fill(this.aTMSDataSet52.Groups);
          
        }
        

        // Go Back 

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

    }
}

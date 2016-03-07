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
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form108 : Form
    {

        Form65 NForm65;

        int WRow;
        string WAtmNo;

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 

   //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
  //      string WBankId;
   //     bool WPrive;
        public Form108(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
         //   WPrive = InPrive;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBoxMsgBoard.Text = "Make your ATM and proceed with requested action"; 
        }

        private void Form108_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet64.TableATMsBasic' table. You can move, or remove it, as needed.
            this.tableATMsBasicTableAdapter.Fill(this.aTMSDataSet64.TableATMsBasic);
            // TODO: This line of code loads data into the 'aTMSDataSet20.TableATMsBasic' table. You can move, or remove it, as needed.
          
          
            try
            {
              //  if (WPrive == true)
              //  {
                    string filter = "Operator = '" + WOperator + "' OR AtmNo = 'ModelPrive' ";
                    tableATMsBasicBindingSource.Filter = filter;
                    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                    this.tableATMsBasicTableAdapter.Fill(this.aTMSDataSet64.TableATMsBasic);

                    if (dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("No ATMs for this user");
                        this.Dispose();
                        return;
                    }

                    dataGridView1.Rows[WRow].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
                   
            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }  

        }

        // Filter incoming data based on selection criteria
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WAtmNo = rowSelected.Cells[0].Value.ToString();
            textBox9.Text = WAtmNo;

        }


        // Go to Form7 to open new ATM like chosen 
        private void button3_Click(object sender, EventArgs e)
        {
          
            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("Choose An ATM number or Enter one that the New one is similar (Like) Please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //dataGridView1.Rows.Clear();
            }
            else
            {
                WRow = dataGridView1.SelectedRows[0].Index;
                //TEST
                WRow = 0;

                NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 3);
                NForm65.FormClosed += NForm65_FormClosed;
                NForm65.ShowDialog(); ;
            }
        }


        // Go to Form7 and Show Data of Chosen ATM 
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("Choose An ATM number or Enter one Please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //dataGridView1.Rows.Clear();
            }
            else
            {
                WRow = dataGridView1.SelectedRows[0].Index;

                NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 1);
                NForm65.FormClosed += NForm65_FormClosed;
                NForm65.ShowDialog(); ;
            }
        }

        void NForm65_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form108_Load(this, new EventArgs());
        }

        // Go to Form 7 and Update Chosen 
        private void button2_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "ModelPrive" || WAtmNo == "AtmNoPrive")
            {
                MessageBox.Show("You Cannot update this ATM. Go and make a New Like" 
                        + " this ATM and then Update the created one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            if (String.IsNullOrEmpty(textBox9.Text))
            {
                MessageBox.Show("Choose or Enter An ATM number Please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //dataGridView1.Rows.Clear();
            }
            else
            {
                WRow = dataGridView1.SelectedRows[0].Index;

                NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 2);
                NForm65.FormClosed += NForm65_FormClosed;
                NForm65.ShowDialog(); ;
            }
        }
        // Go to Form7 and open New ATM 
       
        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox9.Text = "9999";

            WRow = dataGridView1.SelectedRows[0].Index;

            NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 4);
            NForm65.FormClosed += NForm65_FormClosed;
            NForm65.ShowDialog(); ;

        }
// Finish 
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// DELETE ATM
        private void button6_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102" || WAtmNo == "Ab104")
            {
                MessageBox.Show("THIS IS TESTING ATM ... You cannot delete it");
                return;
            }

            Ac.ReadAtm(WAtmNo);

            if (Ac.ActiveAtm == true)
            {
                MessageBox.Show("An active ATM cannot be deleted");
                return;
            }
            else
            {
                if (MessageBox.Show("Warning: Do you want to delete this ATM?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                    Ac.DeleteNotActiveATMBasic(WAtmNo);

                    Form108_Load(this, new EventArgs());
                }
                else
                {
                    return; 
                }        
                
            }
           
        }     

    }
}

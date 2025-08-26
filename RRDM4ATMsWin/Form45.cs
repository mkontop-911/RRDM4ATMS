using System;
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

        string SelectionCriteria; 

        RRDMGroups Gr = new RRDMGroups(); 

        public Form45(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
       
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
        }

        // Load Command 

        private void Form45_Load(object sender, EventArgs e)
        {
            
                SelectionCriteria = " WHERE Operator = '" + WOperator + "'";

                Gr.ReadGroupsAndFillTable(SelectionCriteria);
            if (Gr.HasErrors)
            {
                MessageBox.Show(Gr.ErrorDetails);
                return;
            }

            dataGridView1.DataSource = Gr.TableGroupsOfAtms.DefaultView;

               if (dataGridView1.Rows.Count == 0)
               {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Groups To Display");
                MessageForm.ShowDialog();

                return;
               }
               else
            {
                // ELSE SHOW GRID
                ShowGridFields01();
            }
           
        }


        // IF TEXT IS CHANGED 

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (int.TryParse(textBox1.Text, out ChosenGroupNo))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number!");

                return;
            }
           // ChosenGroupNo = int.Parse(textBox1.Text);

                SelectionCriteria = " WHERE Operator= '" + WOperator + "'" + " AND GroupNo =" + ChosenGroupNo.ToString();

                Gr.ReadGroupsAndFillTable(SelectionCriteria);

                dataGridView1.DataSource = Gr.TableGroupsOfAtms.DefaultView;

               if (dataGridView1.Rows.Count == 0)
               {
                //MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("No Groups To Display");
                //MessageForm.ShowDialog();

                return;
               }
               else
               {
                // ELSE SHOW GRID
                 ShowGridFields01();
               }

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
            NForm46.ShowDialog();

        }


        // GO TO OPEN A NEW GROUP

        private void button7_Click(object sender, EventArgs e)
        {
            ChosenGroupNo = 0;

            WFunctionNo = 2;

            NForm46 = new Form46(WSignedId, WSignRecordNo, WOperator, ChosenGroupNo, WFunctionNo);
            NForm46.FormClosed += NForm46_FormClosed;
            NForm46.ShowDialog();
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
            NForm46.ShowDialog();

        }
        // Show Grid Left 
        public void ShowGridFields01()
        {
            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            //GroupNo = (int)rdr["GroupNo"];
            //BankId = (string)rdr["BankId"];
            //MoreThanOneBank = (bool)rdr["MoreThanOneBank"];
            //Stats = (bool)rdr["Stats"];
            //Replenishment = (bool)rdr["Replenishment"];
            //Reconciliation = (bool)rdr["Reconciliation"];
            //Description = (string)rdr["Description"];
            //DtTmCreated = (DateTime)rdr["DtTmCreated"];
            //Inactive = (bool)rdr["Inactive"];
            //Operator = (string)rdr["Operator"];

            dataGridView1.Columns[0].Width = 65; // "GroupNo"
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; // "BankId"
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 60; // /MoreThanOneBank
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 100; //  Stats
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderText = "Statistics";

            dataGridView1.Columns[4].Width = 100; // Replenishment
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 100; // Reconciliation
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 200; // Description
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Finish 
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}

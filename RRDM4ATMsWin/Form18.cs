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
    public partial class Form18 : Form
    {
        Form200 NForm20MIS;
        Form47 NForm47;
        Form13 NForm13;
        Form31 NForm31;

        DateTime FromDate;
        DateTime ToDate;

        RRDMComboClass Cc = new RRDMComboClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //TEST
        DateTime WorkingToday = new DateTime(2014, 07, 06);

        string WCitId;
        int WGroupNo;

        string filter; 

        string WAccName;
        string WAccCurr; 

        int WAction; 

  //      int WFunctionNo;
        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
    //    bool WPrive;


        public Form18(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
      
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            //TEST
            dateTimePicker1.Value = WorkingToday;
            dateTimePicker2.Value = WorkingToday;
            
            textBoxMsgBoard.Text = "View CIT operational and financial information"; 

     //       buttonNext.Hide();
          
        }

        // Load 
        private void Form18_Load(object sender, EventArgs e)
        {
            
            filter = "Operator = '" + WOperator + "' AND UserType ='CIT Company'";

            usersTableBindingSource.Filter = filter;
            this.usersTableTableAdapter.Fill(this.aTMSDataSet27.UsersTable);

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No available CIT providers.");
                this.Dispose();
                return;
            }

        }

        // ROW ENTER ON USER 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            string temp = rowSelected.Cells[0].Value.ToString();
            WCitId = temp;

           
            comboBox1.DataSource = Cc.GetUserAccs(WOperator, WCitId);
            comboBox1.DisplayMember = "DisplayValue";

            comboBox2.DataSource = Cc.GetUserAccsCurr(WOperator, WCitId);
            comboBox2.DisplayMember = "DisplayValue";

            label13.Text = temp;
            
            usersAtmTableBindingSource.Filter = "Operator = '" + WOperator + "' AND UserId ='" + WCitId + "'";
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet29.UsersAtmTable);

            //comboBox1.Text = "CIT Cash";
            //comboBox2.Text = "EUR"; 

            radioButton1.Checked = true; 

        }
    
        // GO TO TODAY STATUS
        private void button3_Click(object sender, EventArgs e)
        {
           
            NForm20MIS = new Form200(WSignedId, WSignRecordNo, WSecLevel, WOperator ,WCitId);
            NForm20MIS.FormClosed += NForm20MIS_FormClosed;

            NForm20MIS.ShowDialog();

        }

        void NForm20MIS_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form18_Load(this, new EventArgs());
        }
        //
        // Group Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            textBox1.Text = rowSelected.Cells[1].Value.ToString();
            if (textBox1.Text == "")
            {
            }
            else
            {
                WGroupNo = int.Parse(textBox1.Text);
            }
           
        }
        // SHOW THE ATMS 
        private void button1_Click(object sender, EventArgs e)
        {

            WAction = 3; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator, WCitId, WAction);
            NForm47.ShowDialog();
        }
        // SHOW USERS
        private void button5_Click(object sender, EventArgs e)
        {
            NForm13 = new Form13(WSignedId, WSignRecordNo, WOperator ,WCitId);
            NForm13.ShowDialog();
            
        }
        // PROCEED TO SHOW STATEMENT 
        private void button2_Click(object sender, EventArgs e)
        {
            // Last 7 days = WAction 1
            // Last Month = 2
            // Current Month = 3
            // Selected Dates = 4
            if (radioButton1.Checked == false & radioButton2.Checked == false 
                & radioButton3.Checked == false & radioButton4.Checked == false)
            {
                MessageBox.Show("Make your choice please!");
                return; 
            }

            if (radioButton1.Checked == true)
            {
                WAction = 1; // Last 7 days = WAction 1
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                FromDate = DateTime.Today.AddDays(-7);
                ToDate = DateTime.Today;
                //TEST
                FromDate = WorkingToday.AddDays(-7);
                ToDate = WorkingToday;
            }

            if (radioButton2.Checked == true)
            {
                WAction = 2; // Last Month = 2
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                var today = DateTime.Today;
                //TEST
                today = WorkingToday;
                var month = new DateTime(today.Year, today.Month, 1);
                var first = month.AddMonths(-1);
                var last = month.AddDays(-1);
                FromDate = first;
                ToDate = last;
            }

            if (radioButton3.Checked == true)
            {
                WAction = 3;  // Current Month = 3
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                var today = DateTime.Today;
                //TEST
                today = WorkingToday;
                var month = new DateTime(today.Year, today.Month, 1);
                FromDate = month;
                ToDate = DateTime.Today;
                //TEST
                ToDate = WorkingToday; 
            }

            if (radioButton4.Checked == true)
            {
                WAction = 4;  // Selected Dates = 4
                WAccName = comboBox1.Text;
                WAccCurr = comboBox2.Text; 
                FromDate = dateTimePicker1.Value;
                ToDate = dateTimePicker2.Value;

                if (ToDate < FromDate)
                {
                    MessageBox.Show("Please check dates. To date is less than from date");
                    return; 
                }
            }

            NForm31 = new Form31(WSignedId, WSignRecordNo, WOperator, WCitId, WAccName, WAccCurr, WAction, 
                    FromDate, ToDate, "" );
                NForm31.ShowDialog();
              
            
        }
        // ACTION MESSAGES 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("These are the actions messages send to CIT Company");
            return;
        }

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
           
    }
}


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
    public partial class Form13 : Form
    {
    //    FormMainScreen NFormMainScreen;
        Form20 NForm20; // UPDATE USER 
        Form119 NForm119;
        Form120 NForm120; 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 

        string WChosenUserId;
        string WUserName; 

        int WFunctionNo;

        int WRowIndex;

        string filter;
        int WSecLevel;
    
        string WOperator; 

        string WSignedId;
        int WSignRecordNo;  
        string WCitId;

        public Form13(string InSignedId, int SignRecordNo, string InOperator, string InCitId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
      
            WCitId = InCitId; 
            
            InitializeComponent();


            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            Us.ReadUsersRecord(InSignedId);
            WSecLevel = Us.SecLevel; 
            
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet57.UserVsAuthorizers' table. You can move, or remove it, as needed.
          
           
// Load Users 
            if (WSecLevel == 9)
            {
                labelStep1.Text = "Bank GAS Masters ";

                filter = "SecLevel = 7";
                usersTableBindingSource.Filter = filter;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);
                
            }

            if (WSecLevel == 7)
            {
                labelStep1.Text = "Maintenance Masters ";

                filter = "SecLevel = 6";
                usersTableBindingSource.Filter = filter;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);

            }

            if (WSecLevel == 6 & WCitId == "1000")
            {
                labelStep1.Text = "Users for Bank/Operator";

                filter = "Operator = '" + WOperator + "'"; 
                usersTableBindingSource.Filter = filter;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);

            }

            //TEST - Leave this here for testing purposes only 
            if (WSignedId == "1005")
            {
                
                if (WCitId == "1000")
                {
                    labelStep1.Text = "Users for Bank/Operator";
                    filter = "Operator = '" + WOperator + "'";
                }

                if (WCitId != "1000")
                {
                    labelStep1.Text = "Users for CIT : " + WCitId;
                    filter = "Operator = '" + WOperator + "' " + " AND CitId ='" + WCitId + "'";
                }

                usersTableBindingSource.Filter = filter;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);

            }

            if (WSecLevel == 6 & WCitId != "1000")
            {
                // Cit is inputed 
                labelStep1.Text = "Users for CIT : " + WCitId;

                filter = "Operator = '" + WOperator + "' " + " AND CitId ='" + WCitId + "'";
                usersTableBindingSource.Filter = filter;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);

            }
   
            textBoxMsgBoard.Text = " Choose a User and action ";

        }
        // Row for User Data Grid 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WChosenUserId = rowSelected.Cells[0].Value.ToString();
          
            textBox6.Text = WChosenUserId; 

            Us.ReadUsersRecord(WChosenUserId);

            WUserName = Us.UserName;

            usersAtmTableBindingSource.Filter = "Operator = '" + WOperator + "' " + " AND UserId ='" + WChosenUserId + "'";
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet8.UsersAtmTable);

            userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' "
                                           + " AND UserId ='" + WChosenUserId + "'" + " AND OpenRecord = 1 "; 
            this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet57.UserVsAuthorizers);
        }
               
        // USER NUmber was entered in field 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            WChosenUserId = textBox1.Text;

            filter = "Operator = '" + WOperator + "' " + " AND UserId ='" + WChosenUserId + "'";
            usersTableBindingSource.Filter = filter;
            this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);
            
        }
        
        // GO TO OPEN A NEW USER
        private void button2_Click(object sender, EventArgs e)
        {
            WChosenUserId = "";

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 1;

            NForm20 = new Form20(WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo);
            NForm20 .FormClosed +=NForm20_FormClosed;
            NForm20.ShowDialog();
            //this.Hide();
        }

       void NForm20_FormClosed(object sender, FormClosedEventArgs e)
         {
           Form13_Load(this, new EventArgs());

           dataGridView1.Rows[WRowIndex].Selected = true;
           dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }


        // GO TO UPDATE CHOSEN USER 

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("CHOOSE A USER FROM TABLE PLEASE");

                return;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 2;

            NForm20 = new Form20(WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo);
            NForm20.FormClosed +=NForm20_FormClosed;
            NForm20.ShowDialog();
            //this.Hide();
        }

        // View 
        private void buttonView_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("CHOOSE A USER FROM TABLE PLEASE");

                return;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 3 ;

            NForm20 = new Form20(WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo);
            NForm20.FormClosed += NForm20_FormClosed;
            NForm20.ShowDialog();

        } 

        // ACCESS TO ATMS AND GROUPS 
        private void button3_Click(object sender, EventArgs e)
        {
            if (Us.UserInactive == false)
            {
                WRowIndex = dataGridView1.SelectedRows[0].Index;
                NForm119 = new Form119(WSignedId, WSignRecordNo, WChosenUserId, WUserName);
                NForm119.FormClosed += NForm119_FormClosed;
                NForm119.ShowDialog();
            }
            else
            {
                MessageBox.Show("User is Inactive. ");
                return; 
            }
        }

        void NForm119_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
        // Go to Authorisers 
        private void button4_Click(object sender, EventArgs e)
        {
            if (Us.UserInactive == false)
            {
                WRowIndex = dataGridView1.SelectedRows[0].Index;
                NForm120 = new Form120(WSignedId, WSignRecordNo, WOperator, WChosenUserId);
                NForm120.FormClosed += NForm120_FormClosed;
                NForm120.ShowDialog();
            }
            else
            {
                MessageBox.Show("User is Inactive. ");
                return; 
            }
           
        }

        void NForm120_FormClosed(object sender, FormClosedEventArgs e)
        {
            

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
// FINISH 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Enter User name
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Not available functionality ");
            return; 
        }
// Enter ATM no 
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Not available functionality ");
            return; 
        }
// Branch 
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string WChosenBranch = textBox4.Text;

            filter = "Operator = '" + WOperator + "' " + " AND Branch ='" + WChosenBranch + "'";
            usersTableBindingSource.Filter = filter;
            this.usersTableTableAdapter.Fill(this.aTMSDataSet1.UsersTable);
        }

// Show all users 
        private void button5_Click(object sender, EventArgs e)
        {
            Form13_Load(this, new EventArgs());
        }
 
        
    }
}

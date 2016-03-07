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
    public partial class Form120 : Form
    {
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMUserVsAuthorizers Ua = new RRDMUserVsAuthorizers(); 

        string WAuthoriser;
    //    int WSeqNumber; 
        string WBranch; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WorkingUserId; 

        public Form120(string InSignedId, int InSignRecordNo, string InOperator, string InWorkingUserId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WorkingUserId = InWorkingUserId; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            comboBox1.Text = "0"; 

            comboBox2.Items.Add("Branch"); // Branch Authorisers 
            comboBox2.Items.Add("All"); // All Authorisers 

            comboBox2.Text = "Branch"; 

            Us.ReadUsersRecord(WorkingUserId); 
            
            WBranch = Us.Branch; 

            textBox1.Text = "AUTHORISERS FOR USER : " + WorkingUserId;
            textBoxMsgBoard.Text = "Select Authoriser to be moved to the user Authorisers"; 
        }

        private void Form120_Load(object sender, EventArgs e)
        {
           // AUTHORISERS FOR HIS BRANCH
            if (comboBox2.Text == "Branch")
            {
                usersTableBindingSource.Filter = "Operator = '" + WOperator + "' "
                                             + " AND Authoriser = 1 " + " AND Branch = '" + WBranch + "' ";
                this.usersTableTableAdapter.Fill(this.aTMSDataSet47.UsersTable);
            }

            // AUTHORISERS FOR ALL BRANCHES 

            if (comboBox2.Text == "All")
            {
                usersTableBindingSource.Filter = "Operator = '" + WOperator + "' "
                                             + " AND Authoriser = 1 " ;
                this.usersTableTableAdapter.Fill(this.aTMSDataSet47.UsersTable);
            }

            // AUTHORISERS FOR THIS USER 
            userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' "
                + " AND UserId ='" + WorkingUserId + "'" + " AND OpenRecord = 1 "; 
            this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet56.UserVsAuthorizers);

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WAuthoriser = rowSelected.Cells[0].Value.ToString();

            Us.ReadUsersRecord(WAuthoriser);

            LabelUserId.Text = Us.UserId;
            LabelUserNm.Text = Us.UserName; 

        }      

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WAuthoriser = rowSelected.Cells[0].Value.ToString();
        
            Ua.ReadUserVsAuthorizationSpecific(WorkingUserId,WAuthoriser); 
            label3.Text = Ua.Authoriser;
            label4.Text = Ua.AuthorName; 

        }
        // ADD Record 
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (Us.Branch != WBranch)
            {
                if (MessageBox.Show("Warning: Authoriser Branch is different than Working User Branch. Do you want to continue?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {

                }
                else
                {
                    return; 
                }

            }

            Ua.ReadUserVsAuthorizationSpecific(WorkingUserId, WAuthoriser);
            if (Ua.RecordFound == true)
            {
                MessageBox.Show("Authoriser already exist in user's authorisers");
                return; 
            }
              
            Ua.UserId = WorkingUserId;
            Ua.Authoriser = WAuthoriser;
            Ua.AuthorName = Us.UserName;
            Ua.TypeOfAuth = int.Parse(comboBox1.Text);
            Ua.DateOfInsert = DateTime.Now;
            Ua.Operator = WOperator; 

            Ua.InsertUserVsAuthorizationRecord();

            userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' " + " AND UserId ='" + WorkingUserId + "'";
            this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet56.UserVsAuthorizers);

            textBoxMsgBoard.Text = "Authoriser moved from Pool to User . "; 
        }
// REMOVE AUTHORISER RECORD
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Ua.DeleteUserAuthoriserRecord(Ua.UserId, Ua.Authoriser);

            userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' "
                + " AND UserId ='" + WorkingUserId + "'" + " AND OpenRecord = 1 ";
            this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet56.UserVsAuthorizers);

            textBoxMsgBoard.Text = "Authoriser removed from User . "; 
        }
//FINISH
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Selection criteria has changed 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form120_Load(this, new EventArgs()); 
        }
    }
}

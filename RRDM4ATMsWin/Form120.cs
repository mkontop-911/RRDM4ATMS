using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using RRDM4ATMsClasses;

namespace RRDM4ATMsWin
{
    public partial class Form120 : Form
    {
        
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserVsAuthorizers Ua = new RRDMUserVsAuthorizers();
        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles(); 

        string WAuthoriserLeft;
        string WAuthoriserRight;
        //    int WSeqNumber; 
        string WBranch;
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool Is_FirstCycle; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WorkingUserId;
        string WApplication;
        bool WUser_Is_Maker;

        public Form120(string InSignedId, int InSignRecordNo, string InOperator, string InWorkingUserId
            , string InApplication, bool In_User_Is_Maker)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WorkingUserId = InWorkingUserId;
            WApplication = InApplication;
            WUser_Is_Maker = In_User_Is_Maker;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            Is_FirstCycle = true;

            comboBox2.Items.Add("Branch"); // Branch Authorisers 
            comboBox2.Items.Add("All"); // All Authorisers 

            Us.ReadUsersRecord(WorkingUserId);

            WBranch = Us.Branch;

            textBox1.Text = "AUTHORISERS FOR USER : " + WorkingUserId;
            textBoxMsgBoard.Text = "Select Authoriser to be moved to the user Authorisers";

            comboBox2.Text = "Branch";

        }

        private void Form120_Load(object sender, EventArgs e)
        {
            Is_FirstCycle = false; 
            // AUTHORISERS FOR HIS BRANCH
            if (comboBox2.Text == "Branch")
            {
                int Mode = 2; // Authorisers 

                string SelectionCriteria = " WHERE Application ='" + WApplication + "' AND Authoriser = 1 ";
                Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, WBranch);

            }

            // AUTHORISERS FOR ALL BRANCHES 

            if (comboBox2.Text == "All")
            {
                int Mode = 2; // Authorisers 

                string SelectionCriteria = " WHERE Application ='" + WApplication + "' AND Authoriser = 1 ";
                Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, "");
            }

            dataGridView1.DataSource = Usr.UsersVsApplicationsVsRolesDataTable.DefaultView;
            ShowGrid1();

            // SHOW The Authorisers 

            string filterAutho = "Operator = '" + WOperator + "' "
                                       + " AND UserId ='" + WorkingUserId + "'" + " AND OpenRecord = 1 ";

            Ua.ReadUserVsAuthorizersFillDataTable(filterAutho);

            if (Ua.RecordFound == true)
            {
                ShowGrid2();
            }
            
        }
        int SeqLeft;


        // ROW ENTER
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            SeqLeft = (int)rowSelected.Cells[0].Value;

            Usr.ReadUsersVsApplicationsVsRolesBySeqNo(SeqLeft);

            WAuthoriserLeft = LabelUserId.Text = Usr.UserId;

            Us.ReadUsersRecord(Usr.UserId);
            LabelUserNm.Text = Us.UserName;
        }

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WAuthoriserRight = rowSelected.Cells[2].Value.ToString();
            //string WAuthoriserRight_Name = rowSelected.Cells[1].Value.ToString();
            //string WAuthoriserRight_Name_2 = rowSelected.Cells[2].Value.ToString();
            Ua.ReadUserVsAuthorizationSpecific(WorkingUserId, WAuthoriserRight);
            label3.Text = WAuthoriserRight;
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

            Ua.ReadUserVsAuthorizationSpecific(WorkingUserId, WAuthoriserLeft);
            if (Ua.RecordFound == true)
            {
                MessageBox.Show("Authoriser already exist in user's authorisers");
                return; 
            }
              
            Ua.UserId = WorkingUserId;
            Ua.Authoriser = WAuthoriserLeft;
            Ua.AuthorName = Us.UserName;
            Ua.TypeOfAuth = 0 ;
            Ua.DateOfInsert = DateTime.Now;
            Ua.Operator = WOperator; 

            if (WorkingUserId != WAuthoriserLeft)
            {
                Ua.InsertUserVsAuthorizationRecord();

                // update User with Maker Information 
                UpdateUserRecordByMaker(WorkingUserId);
            }
            else
            {
                // Equal is not allowed
                MessageBox.Show("You cannot assign yourself as authoriser.");
                return; 
            }

            // Load 
            Form120_Load(this, new EventArgs());

            //userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' " + " AND UserId ='" + WorkingUserId + "'";
            //this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet2.UserVsAuthorizers);

            textBoxMsgBoard.Text = "Authoriser moved from Pool to User . "; 
        }
// REMOVE AUTHORISER RECORD
        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Ua.DeleteUserAuthoriserRecord(Ua.UserId, Ua.Authoriser);

            //userVsAuthorizersBindingSource.Filter = "Operator = '" + WOperator + "' "
            //    + " AND UserId ='" + WorkingUserId + "'" + " AND OpenRecord = 1 ";
            //this.userVsAuthorizersTableAdapter.Fill(this.aTMSDataSet2.UserVsAuthorizers);
       
            // Load 
            Form120_Load(this, new EventArgs());

            textBoxMsgBoard.Text = "Authoriser removed from User . "; 
        }
        // Show first Grid
        private void ShowGrid1()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                // buttonAdd.Hide();
                return;
            }
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("SeqNo", typeof(int));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("UserId", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("Authoriser", typeof(bool));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("Application", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("SecLevel", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("RoleName", typeof(string));
            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 90; // UserId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 90; // Authoriser
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 110; // Application
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 90; // SecLevel
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 180; // RoleName
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        // Show second Grid
        private void ShowGrid2()
        {
            dataGridView2.DataSource = Ua.UsersToAuthorDataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }

            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 100; //  AuthorId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 150; // AuthoriserName
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 50; // TypeOfAuthor
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 100; // Date of insert 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void UpdateUserRecordByMaker(string WUserId)
        {
            if (WUser_Is_Maker == true)
            {
                // DO ... 

                // Means that changed took place 
                // We must update the USER record
                Us.ReadUsersRecord_FULL(WUserId);

                // Initialised Checker information on User Record

                if (Us.Is_Approved == false)
                {
                    // Maker already in operation
                    // just set up what is needed
                    Us.Is_NewAuthoriser = true;

                    Us.DtTimeOfChange = DateTime.Now;

                    Us.UpdateTail_For_Maker_Work(WUserId);
                }

                if (Us.Is_Approved == true)
                {
                    // Open Case 
                    Us.ApprovedByWhatChecker = "";
                    Us.ApproveOR_Reject = "";
                    Us.DtTimeOfApproving = NullPastDate;
                    Us.Is_Approved = false;
                    Us.UserInactive = true;

                    // UPDATE CHECKER INFORMATION

                    Us.UpdateTail_For_Checker_Work(WUserId);

                    // Initialised Maker information on User Record
                    Us.ChangedByWhatMaker = WSignedId; // is the Designated Maker
                    Us.DtTimeOfChange = DateTime.Now;

                    Us.Is_New_User = false;
                    Us.Is_NewAccessRights = false;
                    Us.Is_NewCategory = false;
                    Us.Is_NewAuthoriser = true;

                    // UPDATE MAKER INFORMATION

                    Us.UpdateTail_For_Maker_Work(WUserId);
                }


            }
        }

        //FINISH
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Selection criteria has changed 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Branch")
            {
                labelBranchName.Show(); 
                labelBranchName.Text = WBranch; 
            }
            else
            {
                labelBranchName.Hide();              
            }

            if (Is_FirstCycle == true)
            {
                // DO NOT ALLOW THE LOAD 
            }
            else
            {
                Form120_Load(this, new EventArgs()); 
            }
            //Form120_Load(this, new EventArgs()); 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}

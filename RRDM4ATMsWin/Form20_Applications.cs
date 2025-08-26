using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form20_Applications : Form
    {

        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string RolesParId;

        bool InternalChange;

        int WSeqNo;

        int WRowIndex;

        string WPrefix;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WUserId;
        bool WUser_Is_Maker; 

        public Form20_Applications(string InSignedId, int SignRecordNo, string InOperator,
                                                        string InUserId, bool In_User_Is_Maker)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WUserId = InUserId;

            WUser_Is_Maker = In_User_Is_Maker; 

            // InMode = 1 means is Branches

            if (WOperator == "ETHNCY2N")
            {
                WPrefix = WOperator.Substring(0, 3);
                WPrefix = "NBG";
            }

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = WSignedId;

            textBoxUserId.Text = WUserId;
            Us.ReadUsersRecord(WUserId);
            textBoxUserName.Text = Us.UserName;
            //InternalChange = true; 

            // Applications
            Gp.ParamId = "801";
            comboBoxApplications.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxApplications.DisplayMember = "DisplayValue";

            if (comboBoxApplications.Text == "ATMS/CARDS" || comboBoxApplications.Text == "FAWRY RECONCILIATION")
                RolesParId = "803";
            else RolesParId = "804";

            // Roles
            Gp.ParamId = RolesParId;
            comboBoxRoles.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxRoles.DisplayMember = "DisplayValue";

        }
        // Load 

        // 
        private void Form20_Applications_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter

            int Mode = 1;
            string Branch = "";
            string SelectionCriteria = " WHERE UserId ='" + WUserId + "' AND Operator='" + WOperator + "'";
            Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, Branch);

            dataGridView1.DataSource = Usr.UsersVsApplicationsVsRolesDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                // buttonAdd.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                checkBoxInsertNewApplication.Checked = true;
                return;
            }

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 90; // UserId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 90; // Application
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // RoleId
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 190; // RoleName
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 80; // 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            buttonAdd.Hide();
            buttonUpdate.Show();
            buttonDelete.Show();
        }

        // On Row Enter

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (checkBoxInsertNewApplication.Checked == true)
            {
                InternalChange = true;
                checkBoxInsertNewApplication.Checked = false;
            }
            else
            {
                InternalChange = false;
            }


            WSeqNo = (int)rowSelected.Cells[0].Value;

            Usr.ReadUsersVsApplicationsVsRolesBySeqNo(WSeqNo);

            comboBoxApplications.Text = Usr.Application;

            textBoxRoleId.Text = Usr.SecLevel;

            comboBoxRoles.Text = Usr.RoleName;

            checkBoxAuthoriser.Checked = Usr.Authoriser;
            checkBoxDisputeOfficer.Checked = Usr.DisputeOfficer;
            checkBoxReconcOfficer.Checked = Usr.ReconcOfficer;
            checkBoxReconcMgr.Checked = Usr.ReconcMgr;
            radioButton1.Checked = Usr.MsgsAllowed;
            radioButton2.Checked = Usr.MsgsAllowed;
            if (Usr.MsgsAllowed == true)
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }

            comboBoxApplications.Enabled = false;

            buttonUpdate.Show();
            buttonDelete.Show();
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            
            if (comboBoxApplications.Text == "Make Selection")
                {
                MessageBox.Show("Please Make Selection For Application");
                return; 
                }
            if (comboBoxRoles.Text == "Make Selection")
            {
                MessageBox.Show("Please Make Selection For Roles");
                return;
            }
            // Check if User Application Already Exists
            Usr.ReadUsersVsApplicationsVsRolesByApplication(WUserId, comboBoxApplications.Text);

            if (Usr.RecordFound == true & textBoxRoleId.Text != "08" & textBoxRoleId.Text != "10")
            {
                MessageBox.Show("Already Exist Application for this user ! ");
                return;
            }
            if (textBoxRoleId.Text == "03" || textBoxRoleId.Text == "07" || textBoxRoleId.Text == "04" || textBoxRoleId.Text == "06" || textBoxRoleId.Text == "08" || textBoxRoleId.Text == "10")
            {
                // OK Allowed 
                // Reconciliation Officer At Center(03)
                // Authoriser(07)
                // Dispute Officer(04)
                // Dispute  Manager(06)
                // Controller(08)
                // Administrator(10)

                if (textBoxRoleId.Text == "08" || textBoxRoleId.Text == "10")
                {
                    if (checkBoxAuthoriser.Checked || checkBoxDisputeOfficer.Checked 
                        || checkBoxReconcOfficer.Checked || checkBoxReconcMgr.Checked)
                    {
                        MessageBox.Show("Not allowed to select other rights for this Role ! ");
                        return;
                    }
                }

            }
            else
            {
                MessageBox.Show("Invalid Role"+ Environment.NewLine
                              + "Valid ones are : 03, 07, 04, 06, 08, 10"
                             );
                return; 
            }
            Usr.UserId = WUserId;
            Usr.Application = comboBoxApplications.Text;
            Usr.SecLevel = textBoxRoleId.Text;
            Usr.RoleName = comboBoxRoles.Text;
            Usr.UpdatedDate = DateTime.Now;
            Usr.Operator = WOperator;
            Usr.Authoriser = checkBoxAuthoriser.Checked;
            Usr.DisputeOfficer = checkBoxDisputeOfficer.Checked;
            Usr.ReconcOfficer = checkBoxReconcOfficer.Checked;
            Usr.ReconcMgr = checkBoxReconcMgr.Checked;
            if (radioButton1.Checked == true)
                Usr.MsgsAllowed = true;
            else Usr.MsgsAllowed = false;
            // Insert NEXT LOADING 

            WSeqNo = Usr.InsertUsersVsApplicationsVsRoles(Usr.Application);

            checkBoxInsertNewApplication.Checked = false;

            ////checkBoxMakeNewVersion.Checked = false;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form20_Applications_Load(this, new EventArgs());

            // update User with Maker Information 
            UpdateUserRecordByMaker(); 

            //dataGridView1.Rows[Mc.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Mc.PositionInGrid));

        }

        private void UpdateUserRecordByMaker()
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
                    Us.Is_NewAccessRights = true;
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
                    Us.Is_NewAccessRights = true;
                    Us.Is_NewCategory = false;
                    Us.Is_NewAuthoriser = false;

                    // UPDATE MAKER INFORMATION

                    Us.UpdateTail_For_Maker_Work(WUserId);
                }


            }
        }

        // UPDATE Application
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
           

            Usr.ReadUsersVsApplicationsVsRolesBySeqNo(WSeqNo);

            Usr.SecLevel = textBoxRoleId.Text;
            Usr.RoleName = comboBoxRoles.Text;
            if (Usr.SecLevel == "08" || Usr.SecLevel == "10")
            {
                MessageBox.Show("Not allowed updating for this Security Level");
                return; 
            }
            if (checkBoxAuthoriser.Checked == true) Usr.Authoriser = true;
            else Usr.Authoriser = false;
            if (checkBoxDisputeOfficer.Checked == true) Usr.DisputeOfficer = true;
            else Usr.DisputeOfficer = false;
            if (checkBoxReconcOfficer.Checked == true) Usr.ReconcOfficer = true;
            else Usr.ReconcOfficer = false;
            if (checkBoxReconcMgr.Checked == true) Usr.ReconcMgr = true;
            else Usr.ReconcMgr = false;
            //Usr.DisputeOfficer = checkBoxDisputeOfficer.Checked;
            //Usr.ReconcOfficer = checkBoxReconcOfficer.Checked;
            //Usr.ReconcMgr = checkBoxReconcMgr.Checked;
            if (radioButton1.Checked == true)
                Usr.MsgsAllowed = true;
            else Usr.MsgsAllowed = false;

            Usr.UpdatedDate = DateTime.Now;

            Usr.UpdateUsersVsApplicationsVsRoles(WSeqNo);

            MessageBox.Show("Updating Done!");

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "Application updated.";

            Form20_Applications_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            // update User with Maker Information 
            UpdateUserRecordByMaker();

        }

        // DELETE Application
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete(Close) this Application?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {

                Usr.DeleteUsersVsApplicationsVsRolesBySeqNo(WSeqNo);

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                MessageBox.Show("Deleted! Application :" + Usr.Application);

                textBoxMsgBoard.Text = "Application Deleted.";

                Form20_Applications_Load(this, new EventArgs());


                if (WRowIndex1 > 0)
                {
                    WRowIndex1 = WRowIndex1 - 1;
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }
            }
            else
            {
                return;
            }

            // update User with Maker Information 
            UpdateUserRecordByMaker();

        }



        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Matching Categories Definition");
            helpForm.ShowDialog();
        }

        // INSERT NEW APPLICATION
        private void checkBoxInsertNewApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }

            if (checkBoxInsertNewApplication.Checked == true)
            {
                buttonAdd.Show();

                buttonUpdate.Hide();
                buttonDelete.Hide();

                // Enable

                comboBoxApplications.Text = "Make Selection";

                textBoxRoleId.Text = "Select Role";

                comboBoxRoles.Text = "Make Selection";

                comboBoxApplications.Enabled = true;

                checkBoxAuthoriser.Checked = false;
                Usr.DisputeOfficer = checkBoxDisputeOfficer.Checked = false;
                Usr.ReconcOfficer = checkBoxReconcOfficer.Checked = false;
                Usr.ReconcMgr = checkBoxReconcMgr.Checked = false;

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();

                //dataGridView2.Enabled = true;
                //int WRowIndex1 = -1;

                //if (dataGridView1.Rows.Count > 0)
                //{
                //    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                //}

                //Form20_Applications_Load(this, new EventArgs());

                //if (dataGridView1.Rows.Count > 0)
                //{
                //    dataGridView1.Rows[WRowIndex1].Selected = true;
                //    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                //}

            }
        }

        // APPLICATIONS HAS BEEN CHANGED
        private void comboBoxApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }

            if (comboBoxApplications.Text == "ATMS/CARDS" || comboBoxApplications.Text == "e_MOBILE") RolesParId = "803";
            else RolesParId = "804";

            // Roles
            Gp.ParamId = RolesParId;
            comboBoxRoles.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxRoles.DisplayMember = "DisplayValue";
        }

        // 
        private void comboBoxRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            Gp.ReadParametersSpecificNm(WOperator, RolesParId, comboBoxRoles.Text);
            if (Gp.OccuranceId.Length == 1)
                textBoxRoleId.Text = "0" + Gp.OccuranceId;
            else textBoxRoleId.Text = Gp.OccuranceId;
        }
        // check authoriser
        private void checkBoxAuthoriser_CheckedChanged(object sender, EventArgs e)
        {
            RRDMUserVsAuthorizers Ua = new RRDMUserVsAuthorizers();
            bool OpenRecord;

            if (checkBoxAuthoriser.Checked == false)
            {
                OpenRecord = false;
                Ua.UpdateUserVsAuthorisationRecordOpenRecord(WUserId, OpenRecord);
                radioButton1.Hide();
                radioButton2.Hide();
            }

            if (checkBoxAuthoriser.Checked == true)
            {
                radioButton1.Show();
                radioButton2.Show();

                radioButton1.Checked = true;

                OpenRecord = true;
                Ua.UpdateUserVsAuthorisationRecordOpenRecord(WUserId, OpenRecord);
            }
        }

        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


    }
}

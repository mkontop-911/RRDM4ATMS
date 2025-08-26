using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using RRDM4ATMsClasses;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form13_NEW : Form
    {
        //    FormMainScreen NFormMainScreen;
        Form20_NEW NForm20_NEW; // UPDATE USER 
        //Form119 NForm119;
        //Form120 NForm120;

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUserVsAuthorizers UvsA = new RRDMUserVsAuthorizers();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMBanks Ba = new RRDMBanks();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        bool ViewWorkFlow = false;

        Bitmap SCREENinitial;
        int AuditTrailUniqueID = 0;

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;

        string WChosenUserId;
        string WUserName;

        bool IsGridEmpty;

        bool ITMXUser;

        bool Is_SignIn_Admin;
        bool Is_SignIn_Maker;
        bool Is_SignIn_Checker;

        int WFunctionNo;

        int WRowIndex;

        string filter;
        string WSecLevel;

        string WOperator;

        string WSignedId;
        int WSignRecordNo;
        string WCitId;
        int WApplication;
        bool W_IsForCIT; 

        public Form13_NEW(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InApplication, bool IsForCIT)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WCitId = InCitId;

            WApplication = InApplication;
            // 1 ATMs + JCC
            // 2 Cashless
            // 3 NOSTRO
            // 4 POS
            W_IsForCIT = IsForCIT; 


            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            if (WOperator == "ITMX")
            {
                labelHeader3.Hide();
                panel12.Hide();
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            WSecLevel = Usi.SecLevel;

            if (WSignedId == "NOSTROUser")
            {
                labelHeader3.Hide();
                panel12.Hide();
            }
            Us.ReadUsersRecord_Tail_Info(InSignedId);

            //if (Us.User_Is_Maker == true)
            //{
            //    ViewWorkFlow = true;
            //}
            //if (Us.User_Is_Checker == true)
            //{
            //    ViewWorkFlow = false;
            //}
            string Admin_MAker_Checker = ""; 
            if (WSignedId == "ADMIN-BDC")
            {
                Is_SignIn_Admin = true;
                labelStep1.Text = "ADMIN Manages Maker and Checker ";
                Admin_MAker_Checker = "Super_Admin:.."+ WSignedId;
            }

            if (Us.User_Is_Maker == true)
            {
                Is_SignIn_Maker = true;
                labelStep1.Text = "MAKER Manages Users and Access Rights ";
                Admin_MAker_Checker = "MAKER:.." + WSignedId;
                radioButtonApprove.Enabled = false;
                radioButtonNotApprove.Enabled = false;
            }

            if (Us.User_Is_Checker == true)
            {
                Is_SignIn_Checker = true;
                buttonDelete.Hide();
                button1.Hide(); // Update
                button2.Hide(); // ADD
                buttonOwnerCategories.Hide(); 

                buttonMaintainApplications.Hide();
                buttonMaintainAtms.Hide();
                buttonMaintainAuthorisers.Hide();
                labelStep1.Text = "CHECKER Approves MAKER Work ";
                Admin_MAker_Checker = "CHECKER:.." + WSignedId;
            }

            labelStep1.Text = "Users Management By : " + Admin_MAker_Checker; 

            comboBoxFilter.Items.Add("ALL_USERS");
            comboBoxFilter.Items.Add("OUTSTANDING_FOR_ACTION");
          
            comboBoxFilter.SelectedIndex = 0;

            // READ DETAILS OF SIGN IN USER
            //Us.ReadUsersRecord_FULL(WSignedId); // Reads FULL
           
            

        }
        string S_Application = "";
        private void Form13_Load(object sender, EventArgs e)
        {
            //refresh this
            Us.ReadUsersRecord(WSignedId);
            WSecLevel = Usi.SecLevel;
            
            //
            //Read Users and fill table 
            //
            S_Application = "";
            if (WApplication == 3)
            {
                S_Application = "NOSTRO";
            }
            if (WApplication == 4)
            {
                S_Application = "POS SETTLEMENT";
            }

            //labelStep1.Text = "Users for Bank/Operator";

            // READ DETAILS OF SIGN IN USER
            Us.ReadUsersRecord_FULL(WSignedId); // Reads FULL
                                                
            if (Is_SignIn_Admin == true)
            {
                labelHeader2.Hide();
                labelHeader3.Hide();
                labelHeader4.Hide();
                label2.Hide();
                panel11.Hide();
                panel12.Hide();
                panel13.Hide();
                panel14.Hide();
                panelChecker.Hide();
                checkBoxNewUser.Hide(); 

               
                if (W_IsForCIT == true)
                {
                    filter = "Operator = '" + WOperator + "' AND USerType = 'CIT Company' )";
                }
                else
                {
                    // GET ONLY THE MAKER AND CHECKERS DEFINED BY ADMIN
                    filter = "Operator = '" + WOperator + "' AND USerType = 'Employee' AND (User_Is_Maker = 1 OR User_Is_Checker = 1)";
                }
                Us.ReadUsersAndFillDataTable_Maker_Checker(WSignedId, WOperator, filter, S_Application); // Read User table 

                ShowGridUsers();

                panelChecker.Hide();
            }
            else
            {
                // GET based on COMBO
                //comboBoxFilter.Items.Add("ALL_USERS");
                //comboBoxFilter.Items.Add("Outstanding_For_Action");
                if (comboBoxFilter.Text== "ALL_USERS")
                {
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0  ";
                }
                if (comboBoxFilter.Text == "OUTSTANDING_FOR_ACTION")
                {
                    //
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND (Is_Approved = 0 and ChangedByWhatMaker <> '') " +
                        " AND User_Is_Maker = 0 AND User_Is_Checker = 0 ";
                }

                Us.ReadUsersAndFillDataTable_Maker_Checker(WSignedId, WOperator, filter, S_Application); // Read User table 

                ShowGridUsers();

               
            }

            textBoxMsgBoard.Text = " Select a User and action ";

        }
        // Row for User Data Grid 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            if (IsGridEmpty == true)
            {

                return;
            }
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (WSignedId == "ADMIN-BDC")
            {
                WChosenUserId = rowSelected.Cells[0].Value.ToString();
                panelChecker.Hide();
            }
            else
            {
                WChosenUserId = rowSelected.Cells[0].Value.ToString();
            }

            textBoxUserId.Text = WChosenUserId;

            Us.ReadUsersRecord_FULL(WChosenUserId); // Reads FULL

           // DEAL WITH THE checkBoxes
           // 1
            if (Us.Is_New_User == true  & Us.Is_Approved == false)
            {
                checkBoxNewUser.Show(); 
                
                checkBoxNewUser.Text = "Is A New User"; 
            }
            else
            {
                checkBoxNewUser.Hide();
            }
            if ( Us.HadToMakeInactive ==true)
            {
                checkBoxNewUser.Show();
                checkBoxNewUser.Text = "User Had to Be Made Inactive";
                
            }
            if (Us.HadToUndoInactive == true)
            {
                checkBoxNewUser.Show();
                checkBoxNewUser.Text = "User Had to Be Made Active";

            }
            
            // 2
            if (Us.Is_NewAccessRights == true & Us.Is_Approved == false)
            {
                checkBoxAccessRights.Show();
            }
            else
            {
                checkBoxAccessRights.Hide();
            }
            // 3
            if (Us.Is_NewCategory == true & Us.Is_Approved == false)
            {
                checkBoxCategories.Show();
            }
            else
            {
                checkBoxCategories.Hide();
            }
            // 4
            if (Us.Is_NewAuthoriser == true & Us.Is_Approved == false)
            {
                checkBoxAuthorisers.Show();
            }
            else
            {
                checkBoxAuthorisers.Hide();
            }

            WUserName = Us.UserName;

            // NOTES  START  
            Order = "Descending";
            WParameter4 = "UserRecord Notes:.." + WChosenUserId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
            // NOTES  END
            // HANDLE CHECKER PANEL
            //bool Is_SignIn_Admin;
            //bool Is_SignIn_Maker;
            //bool Is_SignIn_Checker;

            if (Is_SignIn_Maker == true & Us.Is_Approved == false)
            {
                radioButtonApprove.Checked = false;
                radioButtonNotApprove.Checked = true;

                radioButtonApprove.Enabled = false;
                radioButtonNotApprove.Enabled = false;
            }
            if (Is_SignIn_Maker == true )
            {
                buttonUpdateByChecker.Hide();
            }
            if (Is_SignIn_Checker == true )
            {
                if (Us.Is_Approved == false)
                {
                    radioButtonApprove.Enabled = true;
                    radioButtonNotApprove.Enabled = true;
                    radioButtonApprove.Checked = false;
                    radioButtonNotApprove.Checked = false;
                    buttonUpdateByChecker.Show();
                }

                if (Us.Is_Approved == true)
                {
                    radioButtonApprove.Enabled = false;
                    radioButtonNotApprove.Enabled = false;
                    buttonUpdateByChecker.Hide();
                }
            }

            // PER LINE USER NOW
            if (Us.Is_Approved == true)
            {
                // Not approve yet 
                radioButtonApprove.Checked = true;
                radioButtonNotApprove.Checked = false;
            }
        
            if (Us.ApproveOR_Reject == "Approved")
            {
                // DECISION YES
                radioButtonApprove.Checked = true;
                radioButtonNotApprove.Checked = false;
            }
            if (Us.ApproveOR_Reject == "Not_Approved")
            {
                // DECISION NO
                radioButtonApprove.Checked = false;
                radioButtonNotApprove.Checked = true;
            }

            if (WSignedId != "ADMIN-BDC")
            {
                panelChecker.Show();
            }
            


            if (Us.UserType != "Employee")
            {
                // Hide not needed buttons
                labelHeader2.Hide();
                labelHeader3.Hide();
                labelHeader4.Hide();
                panel11.Hide();
                panel12.Hide();
                panel13.Hide();
                panel14.Hide();
                label2.Hide();
                return;
            }
            else
            {
                if (WSignedId != "ADMIN-BDC")
                {
                    labelHeader2.Show();
                    labelHeader3.Show();
                    labelHeader4.Show();
                    panel11.Show();
                    panel12.Show();
                    panel13.Show();
                    panel14.Show();
                }
                else
                {
                    return;
                }

            }

            // Applications

            ShowGridApplications(WChosenUserId);

            Usr.ReadUsersVsApplicationsVsRolesByApplication(WChosenUserId, "ATMS/CARDS");

            if (Usr.RecordFound == true)
            {
                
                // Check if Replenishment = Reconciliation 
                RRDMGasParameters Gp = new RRDMGasParameters();
                string ParId = "939";
                string OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                if (Gp.OccuranceNm == "YES" & Usr.SecLevel == "03")
                {
                    // Security level is "03" then it gets a group of ATMs 
                    // 
                    Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WChosenUserId, "", 1);

                    labelLevelHigherThantwo.Hide();
                    dataGridView3.Show();
                    buttonMaintainAtms.Show();
                    labelHeader3.Show();
                    panel12.Show();
                    if (Ua.UserGroups_ToAtms_Table.Rows.Count > 0)
                    {
                        //labelLevelHigherThantwo.Hide();
                        dataGridView3.Show();
                        buttonMaintainAtms.Hide();
                        labelHeader3.Show();
                        panel12.Show();

                        ShowGridUserToAtms_Groups();
                    }
                    else
                    {

                        //dataGridView3.Hide();
                        labelHeader3.Hide();
                        panel12.Hide();

                    }
                }
                else
                {
                    if ((Usr.SecLevel == "03" || Usr.SecLevel == "04"))
                    {
                        // 
                        labelLevelHigherThantwo.Show();
                        dataGridView3.Hide();
                        buttonMaintainAtms.Hide();
                        labelHeader3.Show();
                        panel12.Show();
                    }
                    if ((Usr.SecLevel == "02"))
                    {
                        // 
                        labelLevelHigherThantwo.Hide();
                        dataGridView3.Show();
                        buttonMaintainAtms.Show();
                        labelHeader3.Show();
                        panel12.Show();
                        //USERS TO ATMS
                        // string filterATMs = "Operator = '" + WOperator + "' " + " AND UserId ='" + WChosenUserId + "'";

                        Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WChosenUserId, "", 1);

                        ShowGridUserToAtms_Groups();

                    }
                    if ((Usr.SecLevel == "06")
                        || (Usr.SecLevel == "07")
                        || (Usr.SecLevel == "08")
                        || (Usr.SecLevel == "09")
                        )
                    {
                        // 
                        labelHeader3.Hide();
                        panel12.Hide();

                    }


                }


            }
            else
            {
                dataGridView3.Hide();
                buttonMaintainAtms.Hide();
                labelHeader3.Hide();
                panel12.Hide();
            }

            ShowGridReconCateg(WChosenUserId);

            //USER TO AUTHORISERS
            string filterAutho = "Operator = '" + WOperator + "' "
                                          + " AND UserId ='" + WChosenUserId + "'" + " AND OpenRecord = 1 ";

            UvsA.ReadUserVsAuthorizersFillDataTable(filterAutho);

            ShowGridUsersVsAuthor();

            if (Us.UserType == "CIT Company")
            {
                //labelHeader2.Hide();
                //panel11.Hide();

                //labelHeader4.Hide();
                //panel13.Hide();

                //labelLevelHigherThantwo.Hide();
                //dataGridView3.Show();
                //buttonMaintainAtms.Show();
                //labelHeader3.Show();
                //panel12.Show();

                //   string filterATMs = "Operator = '" + WOperator + "' " + " AND UserId ='" + WChosenUserId + "'";

                //  Ua.ReadUserAccessToAtmsFillTable(filterATMs);

                //  ShowGridUserToAtms();
            }
            else
            {
                labelHeader2.Show();
                panel11.Show();

                labelHeader4.Show();
                panel14.Show();
            }

        }

        // Applications Row Enter 
        string WChosenApplication;
        string SelectedUserSecLevel;

        private void dataGridViewApplications_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewApplications.Rows[e.RowIndex];
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("SeqNo", typeof(int));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("UserId", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("Authoriser", typeof(bool));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("Application", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("SecLevel", typeof(string));
            //UsersVsApplicationsVsRolesDataTable.Columns.Add("RoleName", typeof(string));
            int SeqNo = (int)rowSelected.Cells[0].Value;
            WChosenUserId = (string)rowSelected.Cells[1].Value.ToString();
            WChosenApplication = (string)rowSelected.Cells[3].Value.ToString();
            SelectedUserSecLevel = (string)rowSelected.Cells[4].Value.ToString();

            //if (Application == "ATMS/CARDS" & (SelectedUserSecLevel == "02"|| SelectedUserSecLevel == "03"))
            //{
            //    // 
            //    labelLevelHigherThantwo.Show();
            //    dataGridView3.Hide();
            //    buttonMaintainAtms.Hide(); 
            //}
            //else
            //{
            //    labelLevelHigherThantwo.Hide();
            //    dataGridView3.Show();
            //    buttonMaintainAtms.Show();
            //}
        }

      
        // GO TO OPEN A NEW USER
        private void button2_Click(object sender, EventArgs e)
        {
            WChosenUserId = "";

            // WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 1;

            NForm20_NEW = new Form20_NEW(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication,W_IsForCIT);
            NForm20_NEW.FormClosed += NForm20_FormClosed;
            NForm20_NEW.ShowDialog();
            //this.Hide();
        }

        void NForm20_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsGridEmpty == true)
            {
                Form13_Load(this, new EventArgs());
                return;
            }
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "USER UPDATED";

            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

        }


        // GO TO UPDATE CHOSEN USER 

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxUserId.Text))
            {
                MessageBox.Show("CHOOSE A USER FROM TABLE PLEASE");

                return;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 2;

            NForm20_NEW = new Form20_NEW(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication, W_IsForCIT);
            NForm20_NEW.FormClosed += NForm20_FormClosed;
            NForm20_NEW.ShowDialog();
            //this.Hide();
        }

        // View 
        private void buttonView_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxUserId.Text))
            {
                MessageBox.Show("CHOOSE A USER FROM TABLE PLEASE");

                return;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 3;

            NForm20_NEW = new Form20_NEW(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication, W_IsForCIT);
            NForm20_NEW.FormClosed += NForm20_FormClosed;
            NForm20_NEW.ShowDialog();

        }

        // FINISH 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        
       

        //******************
        // SHOW GRID dataGridView1
        //******************

        private void ShowGridUsers()
        {
            if (Us.UsersInDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to show");
                dataGridView1.DataSource = Us.UsersInDataTable.DefaultView;// Show the empty
                textBoxUserId.Text = ""; 
                IsGridEmpty = true;
                return;
            }
            else
            {
                IsGridEmpty = false;
            }

            dataGridView1.DataSource = Us.UsersInDataTable.DefaultView;

            //if (dataGridView1.Rows.Count == 0)
            //{
            //    MessageBox.Show("Nothing to show"); 
            //    IsGridEmpty = true;
            //    return;
            //}
            //else
            //{
            //    IsGridEmpty = false;
            //}

            dataGridView1.Columns[0].Width = 120; // User Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 170; // User Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[2].Width = 100; //  email 
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ////dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            //dataGridView1.Columns[3].Width = 100; // Mobile
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[4].Width = 80; // date Open
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[5].Width = 50; // User Type
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[6].Width = 50; // Cit Id 
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        //******************
        // SHOW GRID dataGridView Applications 
        //******************
        private void ShowGridApplications(string InUserId)
        {
            int Mode = 1;
            string Branch = "";
            string SelectionCriteria = " WHERE UserId ='" + InUserId + "' AND Operator='" + WOperator + "'";
            Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, Branch);

            dataGridViewApplications.DataSource = Usr.UsersVsApplicationsVsRolesDataTable.DefaultView;

            if (dataGridViewApplications.Rows.Count == 0)
            {
                // No Applications 
                labelHeader2.Hide();
                panel11.Hide();
                return;
            }
            else
            {
                labelHeader2.Show();
                panel11.Show();
            }

            dataGridViewApplications.Columns[0].Width = 60; // Seq No
            dataGridViewApplications.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewApplications.Columns[0].Visible = false;

            dataGridViewApplications.Columns[1].Width = 80; // UserId
            dataGridViewApplications.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewApplications.Columns[1].Visible = false;

            dataGridViewApplications.Columns[2].Width = 70; // 
            dataGridViewApplications.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[3].Width = 70; //
            dataGridViewApplications.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewApplications.Columns[4].Width = 70; // sec level
            dataGridViewApplications.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[5].Width = 80; // Role Name
            dataGridViewApplications.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[5].Width = 70; // dispute officer
            dataGridViewApplications.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[5].Width = 70; // reconciliation officer
            dataGridViewApplications.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }

        //******************
        // SHOW dataGridViewReconciliationCateg
        //******************
        private void ShowGridReconCateg(string InUserId)
        {
            Rc.ReadReconcCategoriesAndFillTableByOwner(InUserId);

            dataGridViewReconciliationCateg.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridViewReconciliationCateg.Rows.Count == 0 & WSignedId != "ADMIN-BDC")
            {
                // No categories
                if (Is_SignIn_Maker == true)
                {
                    label2.Show();
                    panel13.Show();
                }
                if (Is_SignIn_Checker == true)
                {
                    label2.Hide();
                    panel13.Hide();
                }
                
                return;
            }
            else
            {
                label2.Show();
                panel13.Show();
            }
            //TableReconcCateg.Columns.Add("SeqNo", typeof(int));
            //TableReconcCateg.Columns.Add("CategoryId", typeof(string));
            //TableReconcCateg.Columns.Add("CategoryName", typeof(string));
            //TableReconcCateg.Columns.Add("Origin", typeof(string));
            //TableReconcCateg.Columns.Add("OpeningDateTm", typeof(int));
            dataGridViewReconciliationCateg.Columns[0].Width = 60; // Seq No
            dataGridViewReconciliationCateg.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewReconciliationCateg.Columns[0].Visible = false;

            dataGridViewReconciliationCateg.Columns[1].Width = 90; // CategoryId
            dataGridViewReconciliationCateg.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewReconciliationCateg.Columns[2].Width = 200; // CategoryName
            dataGridViewReconciliationCateg.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewReconciliationCateg.Columns[3].Width = 100; // Origin
            dataGridViewReconciliationCateg.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewReconciliationCateg.Columns[4].Width = 190; // OpeningDateTm
            dataGridViewReconciliationCateg.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;



        }
        //******************
        // SHOW GRID dataGridView3
        //******************
        //private void ShowGridUserToAtms()
        //{
        //    dataGridView3.DataSource = Ua.UsersToAtmsDataTable.DefaultView;

        //    if (dataGridView3.Rows.Count == 0)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        dataGridView3.Show();
        //        buttonMaintainAtms.Show();
        //    }
        //    dataGridView3.Columns[0].Width = 50; // Owner Id
        //    dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        //    dataGridView3.Columns[1].Width = 100; // Atm no
        //    dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        //    dataGridView3.Columns[2].Width = 200; //  ATM Name 
        //    dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        //    dataGridView3.Columns[3].Width = 50; //  Group of ATMs
        //    dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //    dataGridView3.Columns[3].Visible = false;

        //    dataGridView3.Columns[4].Width = 80; // Replenishment
        //    dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        //    dataGridView3.Columns[5].Width = 80; // Reconciliation 
        //    dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        //    dataGridView3.Columns[6].Width = 130; // Date of insert 
        //    dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        //}

        //******************
        // SHOW GRID dataGridView3 to detail ATMs
        //******************
        private void ShowGridUserToAtms_Groups()
        {
            // We keep it like this because we do not need this 
            // We will find out about this during UAT 
            labelHeader3.Hide();
            panel12.Hide();
            return;
            dataGridView3.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;

            if (dataGridView3.Rows.Count == 0 & WSignedId != "ADMIN-BDC")
            {
                labelHeader3.Hide();
                panel12.Hide();
                return;
            }
            else
            {
                labelHeader3.Show();
                panel12.Show();
                dataGridView3.Show();
                buttonMaintainAtms.Show();
            }

            dataGridView3.Columns[0].Width = 80; // Owner Id
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[1].Width = 60; // InNeed
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[2].Width = 60; //  AtmNo
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[3].Width = 150; //  AtmName
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView3.Columns[3].Visible = false;

            dataGridView3.Columns[4].Width = 60; // Repl Pending
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[5].Width = 80; // Repl Cycle
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[6].Width = 80; // Branch
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }


        //

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridUsersVsAuthor()
        {
            dataGridView4.DataSource = UvsA.UsersToAuthorDataTable.DefaultView;

            if (dataGridView4.Rows.Count == 0 & WSignedId != "ADMIN-BDC")
            {
                labelHeader4.Hide();
                panel14.Hide();
                return;
            }
            else
            {
                labelHeader4.Show();
                panel14.Show();
            }

            dataGridView4.Columns[0].Width = 50; // SeqNo

            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView3.Columns[1].Width = 120; // UserId
            //dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[1].Visible = false;

            dataGridView4.Columns[2].Width = 100; //  AuthorId
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView4.Columns[3].Width = 150; // AuthoriserName
            dataGridView4.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView4.Columns[4].Width = 50; // TypeOfAuthor
            dataGridView4.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[5].Width = 150; // Date of insert 
            dataGridView4.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // MaintainApplications
        private void buttonMaintainApplications_Click(object sender, EventArgs e)
        {
            // Get Initial Screen 
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
            Form20_Applications NForm20_Applications;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            NForm20_Applications = new Form20_Applications(WSignedId, WSignRecordNo, WOperator, textBoxUserId.Text, Is_SignIn_Maker);
            NForm20_Applications.FormClosed += NForm20_Applications_FormClosed;
            NForm20_Applications.ShowDialog();
        }
        // Maintain Atms
        private void buttonMaintainAtms_Click(object sender, EventArgs e)
        {
            if (Us.UserType == "CIT Company")
            {
                MessageBox.Show("This is a CIT company." + Environment.NewLine
                    + "Maintenance for it is done on the ATM definition form"
                    );
                return;
            }

            if (SelectedUserSecLevel != "02")
            {
                MessageBox.Show("The user doesnt have security level 02" + Environment.NewLine
                    + "Not allowed to maintain ATMs"
                    );
                return;
            }

            Form119 NForm119;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            NForm119 = new Form119(WOperator, WSignedId, WSignRecordNo, WChosenUserId, textBoxUserId.Text);
            NForm119.FormClosed += NForm119_FormClosed;
            NForm119.ShowDialog();

        }
        // MaintainAuthorisers
        private void buttonMaintainAuthorisers_Click(object sender, EventArgs e)
        {
            // Get Initial Screen 
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
            //Form20_Applications NForm20_Applications;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            //NForm20_Applications = new Form20_Applications(WSignedId, WSignRecordNo, WOperator, textBoxUserId.Text, Is_SignIn_Maker);
            //NForm20_Applications.FormClosed += NForm20_Applications_FormClosed;
            //NForm20_Applications.ShowDialog();
            Form120 NForm120;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            NForm120 = new Form120(WSignedId, WSignRecordNo, WOperator, WChosenUserId, WChosenApplication, Is_SignIn_Maker);
            NForm120.FormClosed += NForm120_FormClosed;
            //return; 
            NForm120.ShowDialog();
        }
        // On Close
        void NForm119_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            // Initialise USER 
            Us.ReadUsersRecord_FULL(WChosenUserId);
            //Is_New_User = (bool)rdr["Is_New_User"];
            //Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
           // Us.Is_NewCategory = false;
            //Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
           // Us.UpdateTail(WChosenUserId);
            if (Us.Is_NewCategory == true)
            {
                string WMessage = "USER.." + WChosenUserId + "..Updated for Rec Categ.. ";
                //AUDIT TRAIL 
                //AUDIT TRAIL 
                string AuditCategory = "Audit_Trail_For_User_Mgmt";
                string AuditSubCategory = WChosenUserId; // Name of user
                string AuditAction = "ADD OR UPDATE";
                string Message = WMessage;
                GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
            }
        }

        // On Close
        void NForm120_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            // Initialise USER 
            Us.ReadUsersRecord_FULL(WChosenUserId);
            //Is_New_User = (bool)rdr["Is_New_User"];
            //Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
            // Us.Is_NewCategory = false;
            //Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
            // Us.UpdateTail(WChosenUserId);
            if (Us.Is_NewAuthoriser == true)
            {
                string WMessage = "USER.." + WChosenUserId + "..Updated for Authoriser. ";
                //AUDIT TRAIL 
                //AUDIT TRAIL 
                string AuditCategory = "Audit_Trail_For_User_Mgmt";
                string AuditSubCategory = WChosenUserId; // Name of user
                string AuditAction = "ADD OR UPDATE";
                string Message = WMessage;
                GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
            }
        }

        // On Close
        void NForm20_Applications_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            // Initialise USER 
            Us.ReadUsersRecord_FULL(WChosenUserId);
            //Is_New_User = (bool)rdr["Is_New_User"];
            //Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
            // Us.Is_NewCategory = false;
            //Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
            // Us.UpdateTail(WChosenUserId);
            if (Us.Is_NewAccessRights == true)
            {
                string WMessage = "USER.." + WChosenUserId + "..Updated for User Access Rights.. ";
                //AUDIT TRAIL 
                //AUDIT TRAIL 
                string AuditCategory = "Audit_Trail_For_User_Mgmt";
                string AuditSubCategory = WChosenUserId; // Name of user
                string AuditAction = "ADD OR UPDATE";
                string Message = WMessage;
                GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
            }
        }


        // On Close
        void NForm504_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            // Initialise USER 
            Us.ReadUsersRecord_FULL(WChosenUserId);
            //Is_New_User = (bool)rdr["Is_New_User"];
            //Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
            // Us.Is_NewCategory = false;
            //Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
            // Us.UpdateTail(WChosenUserId);
            if (Us.Is_NewCategory == true)
            {
                string WMessage = "USER.." + WChosenUserId + "..Updated for Rec Categ.. ";
                //AUDIT TRAIL 
                //AUDIT TRAIL 
                string AuditCategory = "Audit_Trail_For_User_Mgmt";
                string AuditSubCategory = WChosenUserId; // Name of user
                string AuditAction = "ADD OR UPDATE";
                string Message = WMessage;
                GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
            }
        }
        // Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = "Users Details ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R78 Report78 = new Form56R78(P1, P2, P3, P4, P5);
            Report78.Show();
        }
        // DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Validation 
            RRDMUsersRecords Us = new RRDMUsersRecords();
            RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
            RRDMUserVsAuthorizers UvsA = new RRDMUserVsAuthorizers();
            RRDMReconcCategories Rc = new RRDMReconcCategories();
            RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            //****************

            if (WChosenUserId == "1000"
                || WChosenUserId == "2000"
                || WChosenUserId == "3000"
                || WChosenUserId == "4000"
                || WChosenUserId == "5000"
              )
            {
                MessageBox.Show("You cannot delete this user." + Environment.NewLine
                    );
                return;
            }

            Usr.ReadUsersVsApplicationsVsRolesByUser(WChosenUserId);

            if (Usr.RecordFound == true)
            {

                MessageBox.Show("The user is connected to Roles." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;

            }

            Ua.ReadUserAccessToAtms(WChosenUserId);

            if (Ua.RecordFound == true)
            {
                MessageBox.Show("The user is connected to ATM/s." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }
            //****************
            string InFilter = "  UserId='" + WChosenUserId + "' OR Authoriser ='" + WChosenUserId + "'";
            UvsA.ReadUserVsAuthorizersFillDataTable(InFilter);

            if (UvsA.RecordFound == true)
            {
                MessageBox.Show("The user is connected to Authoriser/s Or He is an Authoriser." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }
            //***************
            // Check if owner 
            Rc.ReadReconcCategorybyUserId(WOperator, WChosenUserId);
            if (Rc.RecordFound == true)
            {
                MessageBox.Show("The user is connected to Reconciliation Categories." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }

            // Check Reconc Category 
            Rcs.ReadReconcCategoriesSessionsBy_Userid(WChosenUserId);
            if (Rcs.RecordFound == true)
            {
                MessageBox.Show("The user is connected to history." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }
            // Check Dispute 
            Di.ReadDisputeOwnerTotal(WChosenUserId);
            if (Di.RecordFound == true)
            {
                MessageBox.Show("The user is connected to a Dispute." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }
            //
            // Check Actions
            //
            Aoc.ReadReadActionOccuranceByMakerORAuthor(WChosenUserId);

            if (Aoc.RecordFound == true)
            {
                MessageBox.Show("The user is connected to Actions." + Environment.NewLine
                    + "Please disconect the user from any dependancies before delete"
                    );
                return;
            }
            //
            // DELETE METHOD
            //
            if (MessageBox.Show("Warning: Do you want to delete the User?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {

                Us.DeleteUserById(WChosenUserId);

                MessageBox.Show("Deleted! User :.." + WChosenUserId);

                textBoxMsgBoard.Text = "USer Deleted.";

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form13_Load(this, new EventArgs());

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

        }
        // button Notes 3 
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "UserRecord Notes:.." + WChosenUserId;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UserRecord Notes:.." + WChosenUserId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
// If Filter change 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form13_Load(this, new EventArgs());
        }
// UPDATE DECISION BY CHECKER
        private void buttonUpdateByChecker_Click(object sender, EventArgs e)
        {

            // Get Initial Screen 
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;

            // Initialised Checker information on User Record


            Us.ApprovedByWhatChecker = WSignedId;
            if (radioButtonApprove.Checked == true)
            {
                Us.ApproveOR_Reject = "Approved";
                if (Us.HadToMakeInactive == true)
                {
                    Us.UserInactive = true;
                }
                else
                {
                    Us.UserInactive = false;
                }
                
                Us.Is_Approved = true;
            }
            if (radioButtonNotApprove.Checked == true)
            {
                Us.ApproveOR_Reject = "Not_Approved";
                Us.UserInactive = true;
                Us.Is_Approved = false;
            }
            Us.DtTimeOfApproving = DateTime.Now;

            // UPDATE CHECKER INFORMATION

            Us.UpdateTail_For_Checker_Work((textBoxUserId.Text).Trim());

            // Insert Record In History 

            Us.Insert_In_Maintenance_History((textBoxUserId.Text).Trim());
            string SaveCurrentUser = WChosenUserId; 

            Form13_Load(this, new EventArgs()); // Keep it here 

            string WMessage = "USER.." + SaveCurrentUser + "..Checker Auther.. ";
            //AUDIT TRAIL 
            //AUDIT TRAIL 
            string AuditCategory = "Audit_Trail_For_User_Mgmt";
            string AuditSubCategory = SaveCurrentUser; // Name of user
            string AuditAction = "ADD OR UPDATE";
            string Message = WMessage;
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

            // CLEAR CHANGES

            Us.ReadUsersRecord_FULL((SaveCurrentUser).Trim());

            // Clear Changes After saving 
            Us.Is_New_User = false;
            Us.Is_NewAccessRights = false;
            Us.Is_NewCategory = false;
            Us.Is_NewAuthoriser = false;
            
            Us.HadToUndoInactive = false; // turn to normal 
            
            // CLEAR MAKER INFORMATION

            Us.UpdateTail_For_Maker_Work((SaveCurrentUser).Trim());

        }

        private void buttonOwnerCategories_Click(object sender, EventArgs e)
        {
            // Get Initial Screen 
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;

            Form504 NForm504;
            int Mode = 1; // Default for ATMs 

            string W_Application = ""; 

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.WFieldNumeric11 == 11)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.WFieldNumeric11 == 12)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.WFieldNumeric11 == 13)
                    {
                        W_Application = "IPN";
                    }
                    labelStep1.Text = "Configuration Menu-Mobile_" + W_Application;
                  
                }
                else
                {
                    //buttonCatForMobile.Hide();
                    //buttonMatchingCateg.Show();
                }
            }

            if (W_Application == "QAHERA" || W_Application == "ETISALAT" || W_Application == "IPN")
            {
                Mode = 7;
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            // Initialise USER 
            Us.ReadUsersRecord_FULL(WChosenUserId);
            //Is_New_User = (bool)rdr["Is_New_User"];
            //Is_NewAccessRights = (bool)rdr["Is_NewAccessRights"];
            Us.Is_NewCategory = false;
            //Is_NewAuthoriser = (bool)rdr["Is_NewAuthoriser"];
            Us.UpdateTail_For_Maker_Work(WChosenUserId);

            NForm504 = new Form504(WSignedId, WSignRecordNo, WOperator, W_Application, Mode, WChosenUserId);
            NForm504.FormClosed += NForm504_FormClosed;
            NForm504.ShowDialog();
        }
// See User Audit Trail 
        private void linkLabelUserAuditTrail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form76_NEW NForm76_NEW = new Form76_NEW(WSignedId, WOperator, WChosenUserId);
            NForm76_NEW.ShowDialog();
        }

        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {

            Bitmap SCREENb;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENb = memoryImage;

            RRDM_AuditTrailClass_NEW At = new RRDM_AuditTrailClass_NEW();
            //AuditTrailUniqueID = 6; 
            if (AuditTrailUniqueID.Equals(0))
            {
                AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
                AuditTrailUniqueID = 0;
            }
            else
            {
                At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 
            // SHOW History FOR USER 
            //
            Form78d_Discre_MOBILE NForm78d_Discre_MOBILE;
            int WMode = 17; // FOR USER 
            NForm78d_Discre_MOBILE = new Form78d_Discre_MOBILE(WOperator, WSignedId, WChosenUserId, "", 0, WMode, "");
            NForm78d_Discre_MOBILE.ShowDialog();
        }
// Excel 
        private void buttonUsersOnExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Users" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Us.UsersInDataTable, WorkingDir, ExcelPath);
        }
        // Enter ID 
        int W_Length = 0; 
        private void textBoxEnterId_TextChanged(object sender, EventArgs e)      
        {
            if (comboBoxFilter.Text != "ALL_USERS")
            {
                MessageBox.Show("Filter should be ALL Users");
                return; 
            }
            textBoxUserNm.Text = "";
            W_Length = 0;
            if (textBoxEnterId.Text.Length> 0 )
            {

                W_Length = textBoxEnterId.Text.Length ;

                if (string.IsNullOrWhiteSpace(textBoxEnterId.Text))
                {
                    W_Length = 0; 
                }
            }

            if (W_IsForCIT == true)
            {
                filter = "Operator = '" + WOperator + "' AND USerType = 'CIT Company' )";
            }
            else
            {
                // GET ONLY THE MAKER AND CHECKERS DEFINED BY ADMIN
                if (W_Length == 0 )
                {
                    //if (comboBoxFilter.Text == "ALL_USERS")
                    //{
                    //    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0  ";
                    //}
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0  ";
                }
                else
                {
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0   "
                            + " AND Left(UserId," + W_Length.ToString() + ") = '" + textBoxEnterId.Text + "' ";
                }
                
            }
           Us.ReadUsersAndFillDataTable_Maker_Checker(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();
        }
        // Search by name
        private void textBoxUserNm_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxFilter.Text != "ALL_USERS")
            {
                MessageBox.Show("Filter should be ALL Users");
                return;
            }
            textBoxEnterId.Text = ""; 
            W_Length = 0;
            if (textBoxUserNm.Text.Length > 0)
            {

                W_Length = textBoxUserNm.Text.Length;

                if (string.IsNullOrWhiteSpace(textBoxUserNm.Text))
                {
                    W_Length = 0;
                }
            }

            if (W_IsForCIT == true)
            {
                filter = "Operator = '" + WOperator + "' AND USerType = 'CIT Company' )";
            }
            else
            {
                // GET ONLY THE MAKER AND CHECKERS DEFINED BY ADMIN
                if (W_Length == 0)
                {
                    //if (comboBoxFilter.Text == "ALL_USERS")
                    //{
                    //    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0  ";
                    //}
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0  ";
                }
                else
                {
                    filter = "Operator = '" + WOperator + "'  AND USerType = 'Employee' AND User_Is_Maker = 0 AND User_Is_Checker = 0   "
                            + " AND Left(UserName," + W_Length.ToString() + ") = '" + textBoxUserNm.Text + "' ";
                }

            }
            Us.ReadUsersAndFillDataTable_Maker_Checker(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();
        }
    }
}

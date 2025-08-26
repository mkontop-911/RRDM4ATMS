using System;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form13 : Form
    {
        //    FormMainScreen NFormMainScreen;
        Form20 NForm20; // UPDATE USER 
        //Form119 NForm119;
        //Form120 NForm120;

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUserVsAuthorizers UvsA = new RRDMUserVsAuthorizers();

        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        RRDMBanks Ba = new RRDMBanks();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

        string WChosenUserId;
        string WUserName;

        bool ITMXUser;

        int WFunctionNo;

        int WRowIndex;

        string filter;
        string WSecLevel;

        string WOperator;

        string WSignedId;
        int WSignRecordNo;
        string WCitId;
        int WApplication;

        public Form13(string InSignedId, int SignRecordNo, string InOperator, string InCitId, int InApplication)
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

        }

        private void Form13_Load(object sender, EventArgs e)
        {
            //refresh this
            Us.ReadUsersRecord(WSignedId);
            WSecLevel = Usi.SecLevel;

            // Load Users 
            if (WSecLevel == "09")
            {
                labelStep1.Text = "Bank GAS Masters ";

                filter = "SecLevel = '07'";
            }

            if (WSecLevel == "07")
            {
                labelStep1.Text = "Maintenance Masters ";

                filter = "SecLevel = '06'";
            }

            if (WSecLevel == "06" & WCitId == "1000")
            {
                labelStep1.Text = "Users for Bank/Operator";

                filter = "Operator = '" + WOperator + "'";

            }

            if (WApplication == 3 & WCitId == "1000")
            {
                labelStep1.Text = "Users for Nostro";

                filter = "Operator = '" + WOperator + "'";

            }

            //TEST - Leave this here for testing purposes only 
            if (
                   WSignedId == "ADMIN-APEX" || WSignedId == "ADMIN-ATMS"
                || WSignedId == "ITMXUser1"
                || WSignedId == "ADMIN-NBG"
                || WSignedId == "NOSTROUser" || WSignedId == "POS_USER1")
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

            }
            if (WSecLevel == "08" || // Controller
                WSecLevel == "10" || // Administrator 
                WSecLevel == "11")
            {
                // FOR Bank du Caire
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
            }

            //TEST - Leave this here for testing purposes only 
            if (WOperator == "ITMX" & ITMXUser == true)
            {
                //Show all Users for ITMX and the Seed Users for other 

                labelStep1.Text = "Users for ITMX AND Seed Users for other Banks";
                filter = "BankId = '" + Us.BankId + "'" + " OR (Operator = '" + WOperator + "' AND SecLevel = 6 ) ";
            }

            if (WOperator == "ITMX" & ITMXUser == false)
            {
                //Show all Users for ITMX and the Seed Users for other 

                labelStep1.Text = "Users for Bank " + Us.BankId;
                filter = "BankId = '" + Us.BankId + "'";
            }


            if (WSecLevel == "03" & WCitId != "1000")
            {
                // Cit is inputed 
                labelStep1.Text = "Users for CIT : " + WCitId;

                filter = "Operator = '" + WOperator + "' " + " AND CitId ='" + WCitId + "'";
            }
            //
            //Read Users and fill table 
            //
            string S_Application = "";
            if (WApplication == 3)
            {
                S_Application = "NOSTRO";
            }
            if (WApplication == 4)
            {
                S_Application = "POS SETTLEMENT";
            }

            labelStep1.Text = "Users for Bank/Operator";
            filter = "Operator = '" + WOperator + "'";

            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();

            textBoxMsgBoard.Text = " Choose a User and action ";

        }
        // Row for User Data Grid 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WChosenUserId = rowSelected.Cells[0].Value.ToString();

            textBoxUserId.Text = WChosenUserId;

            Us.ReadUsersRecord(WChosenUserId);

            
            WUserName = Us.UserName;

            if (Us.UserType != "Employee")
            {
                // Hide not needed buttons
                labelHeader2.Hide();
                labelHeader3.Hide();
                labelHeader4.Hide();
                label2.Hide(); 
                panel11.Hide();
                panel12.Hide();
                panel13.Hide();
                panel14.Hide();
                return; 
            }
            else
            {
                labelHeader2.Show();
                labelHeader3.Show();
                labelHeader4.Show();
                label2.Show();
                panel11.Show();
                panel12.Show();
                panel13.Show();
                panel14.Show();
            }

            // Applications

            ShowGridApplications(WChosenUserId);

            Usr.ReadUsersVsApplicationsVsRolesByApplication(WChosenUserId, "ATMS/CARDS");

            if (Usr.RecordFound == true)
            {
                // Usi.ReadSignedActivityByKey(WSignRecordNo);

                //string WChosenUserId_SecLevel = Usi.SecLevel;

                // Check if Replenishment = Reconciliation 
                RRDMGasParameters Gp = new RRDMGasParameters();
                string ParId = "939";
                string OccurId = "1";
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                if (Gp.OccuranceNm == "YES" & Usr.SecLevel == "03")
                {
                    // Security level is "03" then it gets a group of ATMs 
                    // 
                    Ua.ReadUserAccess_ToAtmsFillTable(WOperator,WChosenUserId, "" ,1); 

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

                        Ua.ReadUserAccess_ToAtmsFillTable(WOperator, WChosenUserId,"",1);

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

            int SeqNo = (int)rowSelected.Cells[0].Value;
            WChosenUserId = (string)rowSelected.Cells[1].Value.ToString();
            WChosenApplication = (string)rowSelected.Cells[2].Value.ToString();
            SelectedUserSecLevel = (string)rowSelected.Cells[3].Value.ToString();

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

        // USER NUmber was entered in field 
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            WChosenUserId = textBox1.Text;

            filter = "Operator = '" + WOperator + "' " + " AND UserId ='" + WChosenUserId + "'";
            //
            //Read Users and fill table 
            //
            string S_Application = "";
            if (WApplication == 3)
            {
                S_Application = "NOSTRO";
            }
            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();

        }

        // GO TO OPEN A NEW USER
        private void button2_Click(object sender, EventArgs e)
        {
            WChosenUserId = "";

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            WFunctionNo = 1;

            NForm20 = new Form20(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication);
            NForm20.FormClosed += NForm20_FormClosed;
            NForm20.ShowDialog();
            //this.Hide();
        }

        void NForm20_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "Matching Category updated.";

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

            NForm20 = new Form20(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication);
            NForm20.FormClosed += NForm20_FormClosed;
            NForm20.ShowDialog();
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

            NForm20 = new Form20(WOperator, WSignedId, WSignRecordNo, WChosenUserId, WFunctionNo, WApplication);
            NForm20.FormClosed += NForm20_FormClosed;
            NForm20.ShowDialog();

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
            //
            //Read Users and fill table 
            //
            string S_Application = "";
            if (WApplication == 3)
            {
                S_Application = "NOSTRO";
            }
            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();
        }

        // Show all users 
        private void button5_Click(object sender, EventArgs e)
        {
            Form13_Load(this, new EventArgs());
        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridUsers()
        {

            dataGridView1.DataSource = Us.UsersInDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            dataGridView1.Columns[0].Width = 120; // User Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 170; // User Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 100; //  email 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 100; // Mobile
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // date Open
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // User Type
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 50; // Cit Id 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

            dataGridViewApplications.Columns[1].Width = 90; // UserId
            dataGridViewApplications.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[2].Width = 140; // Application
            dataGridViewApplications.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[3].Width = 60; // RoleId
            dataGridViewApplications.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewApplications.Columns[4].Width = 190; // RoleName
            dataGridViewApplications.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridViewApplications.Columns[5].Width = 80; // 
            dataGridViewApplications.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridReconCateg(string InUserId)
        {
            Rc.ReadReconcCategoriesAndFillTableByOwner(InUserId); 
            
            dataGridViewReconciliationCateg.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridViewReconciliationCateg.Rows.Count == 0)
            {
                // No Applications 
                label2.Hide();
                panel13.Hide(); 
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
        // SHOW GRID dataGridView3 Groups 
        //******************
        private void ShowGridUserToAtms_Groups()
        {
            dataGridView3.DataSource = Ua.UserGroups_ToAtms_Table.DefaultView;

            if (dataGridView3.Rows.Count == 0)
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

            if (dataGridView4.Rows.Count == 0)
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
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[3].Width = 150; // AuthoriserName
            dataGridView4.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[4].Width = 50; // TypeOfAuthor
            dataGridView4.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[5].Width = 100; // Date of insert 
            dataGridView4.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
        // MaintainApplications
        private void buttonMaintainApplications_Click(object sender, EventArgs e)
        {
            bool Is_Maker = false; 
            Form20_Applications NForm20_Applications;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            NForm20_Applications = new Form20_Applications(WSignedId, WSignRecordNo, WOperator, textBoxUserId.Text, Is_Maker);
            NForm20_Applications.FormClosed += NForm119_FormClosed;
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
            Form120 NForm120;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            NForm120 = new Form120(WSignedId, WSignRecordNo, WOperator, WChosenUserId, WChosenApplication, false);
            NForm120.FormClosed += NForm119_FormClosed;
            NForm120.ShowDialog();
        }
        // On Close
        void NForm119_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form13_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
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
            string InFilter = "  UserId='" + WChosenUserId + "' OR Authoriser ='" + WChosenUserId + "'" ;
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
    }
}

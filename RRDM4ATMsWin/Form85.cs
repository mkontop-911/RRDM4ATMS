using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form85 : Form
    {
        string WUserId = "";

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMBank_Branches Br = new RRDMBank_Branches();
        
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        //    string WUserBankId;

        string filter1;
        //string filter2;

        int SeqNo2;
        string WBranchId;
        string WCategoryId;

        string WAtmNo;

        string WBankId;

        int WSeqNumber;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;

        string WOperator;

        public Form85(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;

            WOperator = InOperator;

            InitializeComponent();

            comboBoxCategory.Items.Add("ATMs");
            comboBoxCategory.Items.Add("Users And CITs");
            comboBoxCategory.Items.Add("Branches");
            comboBoxCategory.Items.Add("Matching Categories");

            comboBoxCategory.Text = "ATMs";

            textBoxMsgBoard.Text = "Choose ATM or CIT Provider to maintain accounts";

            Gp.ParamId = "201"; // Currencies 
            comboBoxCcy.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxCcy.DisplayMember = "DisplayValue";

            Gp.ParamId = "701"; // ACCount NM  
            comboBoxAccName.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxAccName.DisplayMember = "DisplayValue";

            labelAccounts.Text = "ACCOUNTS FOR ATM";

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
        }

        private void Form85_Load(object sender, EventArgs e)
        {

            //Check Accounts class
            RRDMAccountsClass Acc = new RRDMAccountsClass(); 
            int Mode = 1;
            string CategoryId = "";
            
            Acc.ReadAndFindInfoForAccountingTransactions(WOperator, WAtmNo, CategoryId, "", "", Mode);
            Mode = 1;

        }

        // SELECT CATEGORY 
        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategory.Text == "ATMs")
            {
                labelATM.Show();
                panel2.Show();

                labelUser.Hide();
                panel4.Hide();

                //filter1 = "Operator = '" + WOperator + "'";

                Ac.ReadAtmAndFillTableByOperator(WSignedId, WOperator);

                dataGridView1.DataSource = Ac.ATMsDetailsDataTable.DefaultView;

                if (dataGridView1.Rows.Count == 0)
                {
                    //label12.Hide();
                    //textBoxOwner.Hide(); 
                    //buttonChangeOwner.Hide();
                    return;
                }

                dataGridView1.Columns[0].Width = 100; //AtmNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

                dataGridView1.Columns[1].Width = 130; // Atms Name
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].Width = 80; // Branch Id
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                //ATMsDetailsDataTable.Columns.Add("AtmNo", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Atms Name", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Branch Id", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Branch Name", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Street", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Model", typeof(string));
                //ATMsDetailsDataTable.Columns.Add("Repl Type", typeof(string));
            }


            if (comboBoxCategory.Text == "Users And CITs")
            {
                labelATM.Hide();
                panel2.Hide();

                labelUser.Show();
                panel4.Show();
                filter1 = "Operator = '" + WOperator + "'";

                Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter1, "");

                dataGridView3.DataSource = Us.UsersInDataTable.DefaultView;

                if (dataGridView3.Rows.Count == 0)
                {
                    //label12.Hide();
                    //textBoxOwner.Hide(); 
                    //buttonChangeOwner.Hide();
                    return;
                }

                dataGridView3.Columns[0].Width = 120; // UserId
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[1].Width = 180; // UserName
                dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[2].Width = 130; // email
                dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                //// DATA TABLE ROWS DEFINITION 
                //UsersInDataTable.Columns.Add("UserId", typeof(string));
                //UsersInDataTable.Columns.Add("UserName", typeof(string));
                //UsersInDataTable.Columns.Add("email", typeof(string));
                //UsersInDataTable.Columns.Add("MobileNo", typeof(string));
                //UsersInDataTable.Columns.Add("DateOpen", typeof(string));
                //UsersInDataTable.Columns.Add("UserType", typeof(string));
                //UsersInDataTable.Columns.Add("CitId", typeof(string));

            }

            if (comboBoxCategory.Text == "Branches")
            {

                labelATM.Hide();
                panel2.Hide();

                labelUser.Show();
                panel4.Show();
               
                string SelectionCriteria =  " WHERE Operator = '" + WOperator + "'";
                Br.ReadBranchesAtmAndFillTable(WSignedId, SelectionCriteria);


                dataGridView3.DataSource = Br.BranchesDataTable.DefaultView;

                if (dataGridView3.Rows.Count == 0)
                {
                    //label12.Hide();
                    //textBoxOwner.Hide(); 
                    //buttonChangeOwner.Hide();
                    return;
                }

                dataGridView3.Columns[0].Width = 60; // Seq Number 
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[1].Width = 130; // Branc 
                dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[2].Width = 230; // Branch name 
                dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[3].Width = 230; // Branch name 
                dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView3.Columns[3].Visible = false; 

            }

            if (comboBoxCategory.Text == "Matching Categories")
            {
                //MessageBox.Show("Functionality under development");

                labelATM.Hide();
                panel2.Hide();

                labelUser.Show();
                panel4.Show();
              
                Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

                dataGridView3.DataSource = Mc.TableMatchingCateg.DefaultView;

                if (dataGridView3.Rows.Count == 0)
                {
                    //label12.Hide();
                    //textBoxOwner.Hide(); 
                    //buttonChangeOwner.Hide();
                    return;
                }

                dataGridView3.Columns[0].Width = 60; // 
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[1].Width = 100; // 
                dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[2].Width = 400; // 
                dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }
        // On ROW ENTER ATM 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //comboBox1.Text = "1000"; 
            //WCitId = comboBox1.Text; 
            WSeqNumber = 0;
            textBoxAccNo.Text = "";
            comboBoxCcy.Text = "";
            comboBoxAccName.Text = "";

            WAtmNo = (string)rowSelected.Cells[0].Value;

            Ac.ReadAtm(WAtmNo);

            WBankId = Ac.BankId;

            WBranchId = Ac.Branch;

            LoadAtmAccounts();

            labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo;
        }

        // ON ROW ENTER USERS AND CIT 
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            if (comboBoxCategory.Text == "Users And CITs")
            {
                WUserId = (string)rowSelected.Cells[0].Value;

                textBoxAccNo.Text = "";
                comboBoxCcy.Text = "";
                comboBoxAccName.Text = "";
                WBranchId = "";

                Us.ReadUsersRecord(WUserId);

                WBranchId = Us.Branch;

                LoadUserAccounts();

                labelAccounts.Text = "ACCOUNTS FOR USER or CIT : " + WUserId;
            }
            if (comboBoxCategory.Text == "Branches")
            {
                SeqNo2 = (int)rowSelected.Cells[0].Value;

                WUserId = "";
                textBoxAccNo.Text = "";
                comboBoxCcy.Text = "";
                comboBoxAccName.Text = "";

                Br.ReadBranchBySeqNo(SeqNo2);

                WBranchId = Br.BranchId;

                LoadBranchAccounts();

                labelAccounts.Text = "ACCOUNTS FOR Branch :.. " + WBranchId;
            }

            //"Matching Categories"
            if (comboBoxCategory.Text == "Matching Categories")
            {
                SeqNo2 = (int)rowSelected.Cells[0].Value;

                WUserId = "";
                textBoxAccNo.Text = "";
                comboBoxCcy.Text = "";
                comboBoxAccName.Text = "";

                // WCategoryId

                Mc.ReadMatchingCategorybySeqNoActive(WOperator, SeqNo2);

                WCategoryId = Mc.CategoryId;

                LoadCategoryAccounts();

                labelAccounts.Text = "ACCOUNTS FOR Branch :.. " + WBranchId;
            }
        }

        // ROW ENTER FOR ACCOUNT 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            if (dataGridView2.Rows.Count != 0)
            {
                WSeqNumber = (int)rowSelected.Cells[0].Value;
                Acc.ReadAndFindAccountBySeqNo(WSeqNumber);
                textBoxAccNo.Text = Acc.AccNo;
                comboBoxCcy.Text = Acc.CurrNm;
                comboBoxAccName.Text = Acc.AccName;

                textBoxShort.Text = Acc.ShortAccID;
                // CIT
                if (textBoxShort.Text == "35" || textBoxShort.Text == "50")
                {
                    labelCIT.Show();
                    textBoxCIT.Show();
                    textBoxCIT_NM.Show();
                    textBoxCIT.Text = Acc.EntityNo; // CIT Number 

                    Us.ReadUsersRecord(textBoxCIT.Text);

                    if (Us.RecordFound == true)
                    {
                        textBoxCIT_NM.Show();
                        textBoxCIT_NM.Text = Us.UserName;
                    }
                    else
                    {
                        textBoxCIT_NM.Hide();
                        MessageBox.Show("CIT Not found in Users"); 
                    }
                }
                else
                {
                    labelCIT.Hide();
                    textBoxCIT.Hide();
                    textBoxCIT_NM.Hide();
                }
                // CATEGORY
                if (textBoxShort.Text == "70")
                {
                    labelCategory.Show();
                    textBoxCatgoryId.Show();
                }
                else
                {
                    labelCategory.Hide();
                    textBoxCatgoryId.Hide();
                }
            }
        }

        // Load ATM Account Grid 
        private void LoadAtmAccounts()
        {
            
            //WBranchId = "";
            WUserId = "";
            WCategoryId = ""; 
            //filter2 = "Operator ='" + WOperator + "'" + " AND AtmNo = '" + WAtmNo + "'";

            Acc.ReadAccountsAndFillTableForAtmNo(WOperator, WAtmNo);

            dataGridView2.DataSource = Acc.AccountsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            ShowGrid();
           
            //accountsTableBindingSource.Filter = filter2;
            //this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);
        }

        // Load USER Account Grid 
        private void LoadUserAccounts()
        {
            //WAtmNo = "";
            //WCategoryId = "";
            //WBranchId = ""; 
            //filter2 = "Operator ='" + WOperator + "'" + " AND UserId ='" + WUserId + "'" + " AND AtmNo = '" + WAtmNo + "'";

            Acc.ReadAccountsAndFillTableForUserId(WOperator, WUserId);

            dataGridView2.DataSource = Acc.AccountsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            ShowGrid(); 
            //accountsTableBindingSource.Filter = filter2;
            //this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);

        }

        // Load Branch Account Grid 
        private void LoadBranchAccounts()
        {
            WAtmNo = "";
            WCategoryId = "";
            WUserId = "";
            //filter2 = "Operator ='" + WOperator + "'" + " AND UserId ='" + WUserId + "'" + " AND AtmNo = '" + WAtmNo + "'";

            Acc.ReadAccountsAndFillTableForBranchId(WOperator, WBranchId);

            dataGridView2.DataSource = Acc.AccountsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            ShowGrid(); 
            //accountsTableBindingSource.Filter = filter2;
            //this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);

        }

        // Load Category Account Grid 
        private void LoadCategoryAccounts()
        {
            WAtmNo = "";
            WBranchId = "";
            WUserId = "";
            //filter2 = "Operator ='" + WOperator + "'" + " AND UserId ='" + WUserId + "'" + " AND AtmNo = '" + WAtmNo + "'";

            Acc.ReadAccountsAndFillTableForCategoryId(WOperator, WCategoryId);

            dataGridView2.DataSource = Acc.AccountsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            ShowGrid(); 
            //accountsTableBindingSource.Filter = filter2;
            //this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);

        }
        // ADD ACCOUNT
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxAccNo.Text == "")
            {
                MessageBox.Show("Please enter the account number");
                return;
            }

            if (textBoxShort.Text == "35" || textBoxShort.Text == "50" )
            {
                // 35 and 50 is for CIT and 95 is for User Till

                Us.ReadUsersRecord(textBoxCIT.Text);

                WBranchId = "0001"; 


                if (Us.RecordFound == true)
                {
                   // OK
                }
                else
                {
                    MessageBox.Show("CIT Not found in Users");
                    return; 
                }
            }
            //// USER 
            if (textBoxShort.Text == "95")
            {
                // User Till
                Us.ReadUsersRecord(WUserId);

                if (Us.RecordFound == true)
                {
                    // OK
                }
                else
                {
                    MessageBox.Show("User Not found in Users");
                    return;
                }
            }

            string WCurrNm = comboBoxCcy.Text;
            string AccName = comboBoxAccName.Text;
            string AccNo = textBoxAccNo.Text;

            if (comboBoxCategory.Text == "ATMs")
            {
                Acc.BankId = WBankId;
                //  Acc.Prive = false;
            }
            else // THIS IS A CIT 
            {
                Acc.BankId = WOperator;
            }

            if (comboBoxCategory.Text == "ATMs")
            {
                Acc.AtmNo = WAtmNo;
                Acc.UserId = "";
            }
            else
            {
                Acc.AtmNo = "";
                Acc.UserId = "";
            }

            if (comboBoxCategory.Text == "Users And CITs")
            {
                Acc.AtmNo = "";
                WAtmNo = ""; 
               // Acc.UserId = "1000";
                //WBranchId = ""; 
            }
           

            Acc.ReadAndFindAccount_AND_Accno(WUserId, WBranchId, WCategoryId
                                              , WOperator, WAtmNo, WCurrNm, AccName, AccNo);

            if (Acc.RecordFound == true)
            {
                MessageBox.Show("This account already exist");
                return;
            }

            Acc.AccNo = textBoxAccNo.Text;
            Acc.CurrNm = comboBoxCcy.Text;
            Acc.AccName = comboBoxAccName.Text;

            Acc.ShortAccID = textBoxShort.Text;

            //public string ShortAccID; // 20 Customer Acc
            //                      // 30 for Atm Cash

            switch (Acc.ShortAccID)
            {
                case "30":
                case "31": // AUDI ATM Intermetiate
                case "32":
                case "33":
                    {
                       
                        Acc.EntityNm = "AtmNo";
                        Acc.EntityNo = WAtmNo;
                        break;
                    }
                case "35":
                    {
                        Acc.EntityNm = "CitId";
                        Acc.EntityNo = textBoxCIT.Text;
                        break;
                    }
                case "50":
                    {
                        Acc.EntityNm = "CitId";
                        Acc.EntityNo = textBoxCIT.Text;
                        break;
                    }
                case "53":
                    {
                        Acc.EntityNm = "BranchId";
                        Acc.EntityNo = ""; // We get Branch for the signed user  
                        break;
                    }
                case "51":
                    {
                        Acc.EntityNm = "BranchId";
                        Acc.EntityNo = ""; // We get Branch for the ATM no
                        break;
                    }
                case "70":
                    {
                        Acc.EntityNm = "CategoryId";
                        Acc.EntityNo = WCategoryId;

                        break;
                    }
                case "95":
                    {
                        Acc.EntityNm = "ATMUser";
                        
                        Acc.EntityNo = WUserId;

                        Acc.UserId = WUserId;

                        break;
                    }


                //case "User_Account":
                //    {
                //        Acc.ShortAccID = "37";

                //        Acc.EntityNm = "UserId";
                //        Acc.EntityNo = WUserId;
                //        break;
                //    }
                //case "Branch_Excess":
                //    {
                //        Acc.ShortAccID = "40";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Profit&Loss":
                //    {
                //        Acc.ShortAccID = "45";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Dispute_Shortage":
                //    {
                //        Acc.ShortAccID = "49";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Shortage":
                //    {
                //        Acc.ShortAccID = "50";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Differences":
                //    {
                //        Acc.ShortAccID = "51";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Intermediary":
                //    {
                //        Acc.ShortAccID = "52";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Settlement GL":
                //    {
                //        Acc.ShortAccID = "53";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }

                default:
                    {
                        Acc.EntityNm = "";
                        Acc.EntityNo = "";
                        break;
                    }
            }
            
            
            Acc.BranchId = WBranchId;
            Acc.CategoryId = WCategoryId;

            Acc.AccNoInternal = "";

            Acc.Operator = WOperator;

            // Insert Account
            Acc.InsertAccount();

            if (comboBoxCategory.Text == "ATMs")
            {
                LoadAtmAccounts();

                labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo;
            }
            else
            {
                
                if (comboBoxCategory.Text == "Users And CITs" )
                {
                    LoadUserAccounts();
                    labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;

                }
                if (comboBoxCategory.Text == "Branches")
                {
                    LoadBranchAccounts();
                    labelAccounts.Text = "ACCOUNTS FOR BRANCH  :.." + WBranchId;
                }
                if (comboBoxCategory.Text == "Matching Categories")
                {
                    LoadCategoryAccounts();
                    labelAccounts.Text = "ACCOUNTS FOR Category  :.." + WCategoryId;
                }
            }
        }
        // UPDATE ACCOUNT 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (textBoxAccNo.Text == "")
            {
                MessageBox.Show("Please enter the account number");
                return;
            }

            if (textBoxShort.Text == "35" || textBoxShort.Text == "50")
            {
                Us.ReadUsersRecord(textBoxCIT.Text);

                if (Us.RecordFound == true)
                {
                    // OK
                }
                else
                {
                    MessageBox.Show("CIT Not found in Users");
                    return;
                }
            }

            string WCurrNm = comboBoxCcy.Text;
            string AccName = comboBoxAccName.Text;
            string AccNo = textBoxAccNo.Text;

            int WRowIndex = dataGridView2.SelectedRows[0].Index;

            if (comboBoxCategory.Text == "ATMs")
            {
                WUserId = "1000";
            }
            else
            {
                WUserId = WUserId;
            }
           
            Acc.ReadAndFindAccount_AND_Accno(WUserId, WBranchId, WCategoryId
                                               , WOperator, WAtmNo, WCurrNm, AccName, AccNo);
            if (Acc.RecordFound == true)
            {
                MessageBox.Show("This account already exist");
                return;
            }


            Acc.AccNo = textBoxAccNo.Text;
            Acc.CurrNm = comboBoxCcy.Text;
            Acc.AccName = comboBoxAccName.Text;

            //public string ShortAccID; // 20 Customer Acc
            //                      // 30 for Atm Cash
            //                      // 35 CIT_Account
            //                      // 40 Branch_Excess
            //                      // 50 Branch_Shortage
            //                      // 70 Category_Account
            //                      // 90 Has to do with Swift 
            Acc.EntityNm = "";
            Acc.EntityNo = "";

            switch (Acc.ShortAccID)
            {
                case "30":
                    {

                        Acc.EntityNm = "AtmNo";
                        Acc.EntityNo = WAtmNo;
                        break;
                    }
                case "35":
                    {
                        Acc.EntityNm = "CitId";
                        Acc.EntityNo = textBoxCIT.Text;
                        break;
                    }
                case "50":
                    {
                        Acc.EntityNm = "CitId";
                        Acc.EntityNo = textBoxCIT.Text;
                        break;
                    }
                case "53":
                    {
                        Acc.EntityNm = "BranchId";
                        Acc.EntityNo = ""; // We get Branch for the signed user  
                        break;
                    }
                case "70":
                    {
                        Acc.EntityNm = "CategoryId";
                        Acc.EntityNo = WCategoryId;

                        break;
                    }

                //case "User_Account":
                //    {
                //        Acc.ShortAccID = "37";

                //        Acc.EntityNm = "UserId";
                //        Acc.EntityNo = WUserId;
                //        break;
                //    }
                //case "Branch_Excess":
                //    {
                //        Acc.ShortAccID = "40";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Profit&Loss":
                //    {
                //        Acc.ShortAccID = "45";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Dispute_Shortage":
                //    {
                //        Acc.ShortAccID = "49";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Shortage":
                //    {
                //        Acc.ShortAccID = "50";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Differences":
                //    {
                //        Acc.ShortAccID = "51";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Intermediary":
                //    {
                //        Acc.ShortAccID = "52";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }
                //case "Branch_Settlement GL":
                //    {
                //        Acc.ShortAccID = "53";

                //        Acc.EntityNm = "BranchId";
                //        Acc.EntityNo = WBranchId;

                //        break;
                //    }

                default:
                    {
                        break;
                    }
            }



            Acc.CategoryId = WCategoryId;

            Acc.UpdateAccount(WSeqNumber, WOperator);

            if (comboBoxCategory.Text == "ATMs")
            {
                LoadAtmAccounts();

                dataGridView2.Rows[WRowIndex].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo;
            }
            else
            {
                if (comboBoxCategory.Text == "Users And CITs")
                {
                    LoadUserAccounts();

                    dataGridView2.Rows[WRowIndex].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                    labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;

                }
                if (comboBoxCategory.Text == "Branches")
                {
                    LoadBranchAccounts();

                    dataGridView2.Rows[WRowIndex].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                    labelAccounts.Text = "ACCOUNTS FOR BRANCH  :.." + WBranchId;
                }

                if (comboBoxCategory.Text == "Matching Categories")
                {
                    LoadCategoryAccounts();

                    dataGridView2.Rows[WRowIndex].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                    labelAccounts.Text = "ACCOUNTS FOR CATEGORY ID  :.." + WCategoryId;
                }

            }
        }
        // DELETE ACCOUNT 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this account?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
            {
                Acc.DeleteAccount(WSeqNumber);

                if (comboBoxCategory.Text == "ATMs")
                {
                    LoadAtmAccounts();

                    labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo;
                }
                else
                {
                    if (comboBoxCategory.Text == "Users And CITs")
                    {
                        LoadUserAccounts();
                        labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;

                    }
                    if (comboBoxCategory.Text == "Branches")
                    {
                        LoadBranchAccounts();
                        labelAccounts.Text = "ACCOUNTS FOR BRANCH  :.." + WBranchId;
                    }
                    if (comboBoxCategory.Text == "Matching Categories")
                    {
                        LoadBranchAccounts();
                        labelAccounts.Text = "ACCOUNTS FOR Category  :.." + WCategoryId;
                    }
                }
               
            }
            else
            {
            }
        }

        private void ShowGrid()
        {
            //AccountsTable.Columns.Add("SeqNumber", typeof(int));
            //AccountsTable.Columns.Add("Branch", typeof(string));
            //AccountsTable.Columns.Add("Short", typeof(string));
            //AccountsTable.Columns.Add("AccNo", typeof(string));
            //AccountsTable.Columns.Add("CurrNm", typeof(string));
            //AccountsTable.Columns.Add("AccName", typeof(string));
            //AccountsTable.Columns.Add("AtmNo", typeof(string));

            dataGridView2.Columns[0].Width = 60; // SeqNumber
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 50; // Branch
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 50; // Short
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 80; // AccNo
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 50; // CurrNm
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 110; // AccName
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // test loading 
        private void button1_Click(object sender, EventArgs e)
        {
            Acc.ReadAllATMsAndUpdateAccNo(WOperator);
        }
// Initialise short Name 
        private void comboBoxAccName_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Gp.ReadParametersSpecificNm(WOperator, "701", comboBoxAccName.Text);

            textBoxShort.Text = Gp.OccuranceId;
            // CIT 
            if (Gp.OccuranceId == "35" || Gp.OccuranceId == "50")
            {
                labelCIT.Show();
                textBoxCIT.Show();
                textBoxCIT_NM.Show();
            }
            else
            {
                labelCIT.Hide();
                textBoxCIT.Hide();
                textBoxCIT_NM.Hide();
            }

           
            // CATEGORY
            if (Gp.OccuranceId == "70" )
            {
                labelCategory.Show();
                textBoxCatgoryId.Show();
            }
            else
            {
                labelCategory.Hide();
                textBoxCatgoryId.Hide();
            }
        }
// If CIT changes 
        private void textBoxCIT_TextChanged(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(textBoxCIT.Text);

            if (Us.RecordFound == true)
            {
                textBoxCIT_NM.Show();
                textBoxCIT_NM.Text = Us.UserName; 
            }
            else
            {
                textBoxCIT_NM.Hide();
            }
            

        }

    }
}

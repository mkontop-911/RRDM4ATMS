using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form85_Visa : Form
    {

        RRDMNVBanksNostroVostro Bnv = new RRDMNVBanksNostroVostro();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMBank_Branches Bb = new RRDMBank_Branches();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        string filter1;
        //string filter2;

        string WBankId;

        string BranchId; 
        int WSeqNumber;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOrigin; 
        string WOperator;

        public Form85_Visa(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InOrigin)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOrigin = InOrigin; 
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            if (WOrigin == "E_FINANCE")
            {
                label13.Show();
                textBoxBranchId.Show();
                label14.Show();
                textBoxBranchName.Show();
                panelVOSTRO.Hide(); 
            }
            else
            {
                // Visa 
                panelVOSTRO.Show();
                label13.Hide();
                textBoxBranchId.Hide();
                label14.Hide();
                textBoxBranchName.Hide();
            }

            textBoxMsgBoard.Text = "Choose Entity and OR Account for maintenance";

            Gp.ParamId = "201"; // Currencies 
            comboBoxCurrencies.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxCurrencies.DisplayMember = "DisplayValue";
        }

        private void Form85_Load(object sender, EventArgs e)
        {
            filter1 = " WHERE Operator = '" + WOperator + "'";

            Bnv.ReadBanksForDataTable(filter1);

            dataGridView1.DataSource = Bnv.ExternalBanksDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                //return;
            }

            dataGridView1.Columns[0].Width = 80; //BankId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[1].Width = 150; // BankName
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 150; // ContactName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 170; // DtTmCreated
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //ExternalBanksDataTable.Columns.Add("BankId", typeof(string));
            //ExternalBanksDataTable.Columns.Add("BankName", typeof(string));
            //ExternalBanksDataTable.Columns.Add("ContactName", typeof(string));
            //ExternalBanksDataTable.Columns.Add("DtTmCreated", typeof(DateTime));
        }


        // On ROW ENTER First Grid
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNumber = 0;
            checkBox2.Checked = false; 
            textBoxExternalAcno.Text = "";
            comboBoxCurrencies.Text = "";
            textBoxAccName.Text = "";
            textBoxInternalAcc.Text = "";

            WBankId = (string)rowSelected.Cells[0].Value;

            Bnv.ReadBank(WBankId);

            textBoxBankId.Text = Bnv.BankId;
            textBoxBankName.Text = Bnv.BankName;

            textBoxContact.Text = Bnv.ContactName;
            textBoxMobile.Text = Bnv.Mobile;
            textBoxEmail.Text = Bnv.Email;

            LoadBankAccounts(WBankId); // LOAD BANK ACCOUNTS

            labelAccounts.Text = "ACCOUNTS FOR BankId : " + WBankId;
        }


        // Load ATM Account Grid 
        private void LoadBankAccounts(string InBankId)
        {

            //filter2 = "Operator ='" + WOperator + "'" + " AND BankId = '" + WBankId + "'";

            Acc.ReadAccountsAndFillTableForBankId(WOperator,WBankId);

            dataGridView2.DataSource = Acc.AccountsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //label12.Hide();
                //textBoxOwner.Hide(); 
                //buttonChangeOwner.Hide();
                return;
            }

            dataGridView2.Columns[0].Width = 60; // SeqNumber
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 100; // AccNo
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 70; // BankId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 50; // CurrNm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 110; // AccName
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Visible = false; // ATMNo
        }

        // ROW ENTER FOR ACCOUNT 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            checkBox1.Checked = false;
            if (dataGridView2.Rows.Count != 0)
            {
              
                WSeqNumber = (int)rowSelected.Cells[0].Value;
                Acc.ReadAndFindAccountBySeqNo(WSeqNumber);

                if (WOrigin == "E_FINANCE")
                {
                    //GL_300
                    label13.Show();
                    textBoxBranchId.Show();
                    label14.Show();
                    textBoxBranchName.Show();
                    panelVOSTRO.Show();
                    BranchId = Acc.AccNo.Substring(3, Acc.AccNo.Length - 3);
                    textBoxBranchId.Text = BranchId;
                    Bb.ReadBranchByBranchId(BranchId); 
                    if (Bb.RecordFound == true)
                    {
                        textBoxBranchName.Text = Bb.BranchName; 
                    }
                }
                else
                {
                    // Visa 
                    panelVOSTRO.Show();
                    label13.Hide();
                    textBoxBranchId.Hide();
                    label14.Hide();
                    textBoxBranchName.Hide();
                }

                textBoxExternalAcno.Text = Acc.AccNo;
                comboBoxCurrencies.Text = Acc.CurrNm;
                textBoxAccName.Text = Acc.AccName;
                textBoxInternalAcc.Text = Acc.AccNoInternal;

                textBoxBranchId.ReadOnly = true; 

                buttonAddAcc.Hide();
                buttonUpdateAcc.Show();
                buttonDeleteAcc.Show();
            }
        }

        // ADD EXTERNAL Bank 
        private void buttonAddBank_Click(object sender, EventArgs e)
        {
            Bnv.BankId = textBoxBankId.Text;

            Bnv.ReadBank(Bnv.BankId);
            if (Bnv.RecordFound == true)
            {
                MessageBox.Show("This Bank already exist");
                return;
            }

            Bnv.BankName = textBoxBankName.Text;

            Bnv.ContactName = textBoxContact.Text;
            Bnv.Mobile = textBoxMobile.Text;
            Bnv.Email = textBoxEmail.Text;
            Bnv.DtTmCreated = DateTime.Now;
            Bnv.Operator = WOperator;

            Bnv.InsertExternalBank(Bnv.BankId);

            Form85_Load(this, new EventArgs());

        }
        // UPDATE EXTERNAL BANK
        private void buttonBank_Click(object sender, EventArgs e)
        {
            Bnv.ReadBank(WBankId);

            Bnv.BankName = textBoxBankName.Text;

            Bnv.ContactName = textBoxContact.Text;
            Bnv.Mobile = textBoxMobile.Text;
            Bnv.Email = textBoxEmail.Text;

            Bnv.UpdateExternalBank(WBankId);

            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form85_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }
        // DELETE EXTERNAL BANK 
        private void buttonDeleteBank_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Bank?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                Bnv.DeleteBankEntry(WBankId);

                Form85_Load(this, new EventArgs());
            }
            else
            {
            }

        }

        // ADD ACCOUNT
        private void buttonAdd_Click(object sender, EventArgs e)
        {
           
            if (textBoxBranchName.Text == "")
            {
                MessageBox.Show("Branch Name is missing!");
                return;
            }

            if (textBoxExternalAcno.Text == "" || textBoxInternalAcc.Text == "")
            {
                MessageBox.Show("Accounts Information is Missing");
                return;
            }

            Acc.AccNo = textBoxExternalAcno.Text;
            Acc.CurrNm = comboBoxCurrencies.Text;
            //  Acc.AccName = comboBoxCurrencies.Text +"Acc with " + WBankId;
            Acc.AccName = "PAIR:_" + textBoxInternalAcc.Text + "_AND_" + textBoxExternalAcno.Text;
            Acc.BankId = WBankId;

            Acc.AccNoInternal = textBoxInternalAcc.Text;

            Acc.ReadAndFindAccountSpecificForNostroVostro(Acc.AccNo, Acc.BankId);

            if (Acc.RecordFound == true)
            {
                MessageBox.Show("This account already exist");
                return;
            }

            Acc.AtmNo = "";
            Acc.UserId = "";

            Acc.Operator = WOperator;
            // INSERT ACCOUNT
            Acc.InsertAccount();

            // INSERT BRANCH if it is new
            // 
            if (WOrigin == "E_FINANCE")
            {
                Bb.ReadBranchByBranchId(textBoxBranchId.Text);
                if (Bb.RecordFound == false)
                {
                    //           
                    Bb.BankId = WOperator;
                    Bb.BranchId = textBoxBranchId.Text;
                    Bb.BranchName = textBoxBranchName.Text;
                    Bb.Street = "";
                    Bb.Town = "";
                    Bb.District = "";
                    Bb.PostalCode = "";
                    Bb.Country = "";
                    Bb.Latitude = Convert.ToDouble("14.53");
                    Bb.Longitude = Convert.ToDouble("14.53");
                    Bb.UpdatedDate = DateTime.Now;
                    Bb.Operator = WOperator;

                    Bb.InsertBranch(textBoxBranchId.Text);

                }

                // CREATE CATEGORY
                // Code taken From FORM503_Visa
                CreateCategory(Bb.BranchId, Acc.AccNo);
            }
         

            LoadBankAccounts(WBankId);

        }
        // UPDATE ACCOUNT 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            string CurrNm = comboBoxCurrencies.Text;
            string AccName = textBoxAccName.Text;

            int WRowIndex = dataGridView2.SelectedRows[0].Index;
            
            // Update Branch Name
            Bb.ReadBranchByBranchId(textBoxBranchId.Text);
            if (Bb.RecordFound == true)
            {
                // Check if had changed or better update in case of change 
                Bb.BranchName = textBoxBranchName.Text;
                Bb.UpdateBranch(Bb.SeqNo);    
            }
           
            Acc.ReadAndFindAccountBySeqNo(WSeqNumber);

            Acc.AccNo = textBoxExternalAcno.Text;
            Acc.CurrNm = comboBoxCurrencies.Text;
            Acc.AccName = "PAIR:_" + textBoxInternalAcc.Text + "_AND_" + textBoxExternalAcno.Text;

            Acc.AccNoInternal = textBoxInternalAcc.Text;

            Acc.UpdateAccount(WSeqNumber, WOperator);

            LoadBankAccounts(WBankId);

            dataGridView2.Rows[WRowIndex].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            labelAccounts.Text = "ACCOUNTS FOR ATM : " + WBankId;

        }
        // DELETE ACCOUNT 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this account?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
            {
                Acc.DeleteAccount(WSeqNumber);

                if (WOrigin == "E_FINANCE")
                {
                    RRDMMatchingCategories Mc = new RRDMMatchingCategories();
                    RRDMReconcCategories Rc = new RRDMReconcCategories();
                    RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

                    Mc.ReadMatchingCategoriesIdByPair(textBoxInternalAcc.Text, textBoxExternalAcno.Text );
                  
                    Mc.DeleteMatchingCategory(Mc.SeqNo, Mc.CategoryId);

                    Rc.DeleteReconcCategory(Mc.CategoryId);

                    RcMc.DeleteReconcCateqoriesVsMatchingCategoriesByReconcCategory(Mc.CategoryId);
       
                }
               

                MessageBox.Show("Deleted!");

                textBoxMsgBoard.Text = "Matching Category Deleted.";


                LoadBankAccounts(WBankId);

                labelAccounts.Text = "ACCOUNTS FOR BankId : " + WBankId;


                // DELETE CATEGORY 
                //
                //

            }
            else
            {
            }
        }

        // Clear fields For Bank 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBoxBankId.Text = "";
                textBoxBankName.Text = "";
                textBoxContact.Text = "";
                textBoxEmail.Text = "";
                textBoxMobile.Text = "";    
            }
        }

        // Clear fields For account 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (WOrigin == "E_FINANCE")
                {
                    label13.Show();
                    textBoxBranchId.Show();
                    label14.Show();
                    textBoxBranchName.Show();
                    textBoxBranchId.ReadOnly = false; 
                    textBoxBranchId.Text = "";
                    textBoxBranchName.Text = "";
                    panelVOSTRO.Hide();
                    checkBox1.Checked = false; 
                }
                else
                {
                    // Visa 
                    panelVOSTRO.Show();
                    label13.Hide();
                    textBoxBranchId.Hide();
                    label14.Hide();
                    textBoxBranchName.Hide();
                    textBoxExternalAcno.ReadOnly = false;
                    textBoxAccName.ReadOnly = false;
                    textBoxInternalAcc.ReadOnly = false;
                }
                textBoxExternalAcno.Text = "";
                textBoxAccName.Text = "";
                textBoxInternalAcc.Text = "";

                buttonAddAcc.Show();
                buttonUpdateAcc.Hide();
                buttonDeleteAcc.Hide();
            }
        }

  
// Change of Branch Id
        private void textBoxBranchId_TextChanged(object sender, EventArgs e)
        {
            panelVOSTRO.Show(); 
            if (textBoxBranchId.Text != "")
            {
                panelVOSTRO.Show(); 
                BranchId = textBoxBranchId.Text;
                Bb.ReadBranchByBranchId(BranchId);
                if (Bb.RecordFound == true)
                {
                    textBoxBranchName.Text = Bb.BranchName;
                }
                else
                {
                    textBoxBranchName.Text = ""; 
                }

                textBoxExternalAcno.Text = "GL_" + textBoxBranchId.Text; 
                comboBoxCurrencies.Text = "EGP"; 
                textBoxInternalAcc.Text = "E_FIN_" + textBoxBranchId.Text;
                textBoxAccName.Text = "PAIR:_" + textBoxInternalAcc.Text + "_AND_" + textBoxExternalAcno.Text;  
            }
            else
            {
                panelVOSTRO.Hide();
            }
        }
        // Create Category
        private void CreateCategory(string InBranchId, string InAccNo)
        {
            //
            // IT IS E-FINANCE 
            //
            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            RRDMReconcCategories Rc = new RRDMReconcCategories();
            // Code Taken from Form503_Visa
            Acc.ReadAndFindAccountSpecificForNostroVostro(InAccNo, WOperator); 
            
            Mc.CategoryId = "BDC" + InBranchId;
            Mc.CategoryName = Acc.AccName; 

            Mc.Origin = "E_FINANCE";

            Mc.ReconcMaster = true;

            Mc.TransTypeAtOrigin = "Various Trans";

            Mc.TargetSystemId = 0;
            Mc.TargetSystemNm = "";

            Mc.RunningJobGroup = "E_FINANCE Reconciliation";
            Mc.Product = "E_FINANCE PRODUCTS";
            Mc.CostCentre = "N/A";
            Mc.Periodicity = "Matching starts at fixed time ";
          //  Mc.GroupIdInFiles = "";
            Mc.Pos_Type = false;
            Mc.Currency = "EGP";
            Mc.EntityA = "";

            Mc.EntityB = "";
            Mc.GlAccount = "N/A";

            Mc.VostroBank = "E_FIN";
            Mc.VostroCurr = "EGP";

            Acc.AccNo = textBoxExternalAcno.Text;
            Acc.CurrNm = comboBoxCurrencies.Text;
            Acc.AccName = "PAIR:_" + textBoxInternalAcc.Text + "_AND_" + textBoxExternalAcno.Text;

            Acc.AccNoInternal = textBoxInternalAcc.Text;

            Mc.VostroAcc = Acc.AccNo;
            Mc.InternalAcc = Acc.AccNoInternal;

            if (WOrigin == "E_FINANCE")
            {
                Mc.CategoryName = "E_FINANCE For " + Mc.InternalAcc;
            }
            if (WOrigin == "Visa")
            {
                Mc.CategoryName = "Visa for " + Mc.InternalAcc;
            }

            Mc.Operator = WOperator;

          
            Mc.NextMatchingDt = NullPastDate; 

            int MSeqNo = Mc.InsertMatchingCategory();


            // Create Record in Reconciliation File 

            if (Mc.ReconcMaster == true)
            {
                Rc.CategoryId = Mc.CategoryId;

                Rc.CategoryName = Mc.CategoryName;

                Rc.Origin = Mc.Origin;

                Rc.AtmGroup = 0;

                Rc.IsOneMatchingCateg = true;

                Rc.HasOwner = false;
                Rc.OwnerUserID = "";

                Rc.OpeningDateTm = DateTime.Now;

                Rc.Operator = WOperator;

                int RSeqNo = Rc.InsertReconcCategory();

                MessageBox.Show("Record with id " + Mc.CategoryId + " Inserted In Matching Categories!" + Environment.NewLine
                                 + "Also A corresponding record was inserted in Reconciliation Categories" + Environment.NewLine
                                 + "Go and Assign Owner for Reconciliation"
                               );
            }
            else
            {
                MessageBox.Show("Record with id " + Mc.CategoryId + " Inserted!");
            }

            textBoxMsgBoard.Text = "New Matching Category inserted.";
        }
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
// Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            //
            // With the below instruction SQL WReport76 is loaded 
            //
            string SelectionCriteria = " WHERE Operator='" + WOperator + "'";
            Bb.ReadBranchesAtmAndFillTable(WSignedId, SelectionCriteria);

            string P1 = "Branches Vs E-Finance Details ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R76 Report76 = new Form56R76(P1, P2, P3, P4, P5);
            Report76.Show();
        }
    }
}

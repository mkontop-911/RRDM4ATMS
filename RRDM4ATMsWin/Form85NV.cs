using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form85NV : Form
    {

        RRDMNVBanksNostroVostro Bnv = new RRDMNVBanksNostroVostro();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMAccountsClass Acc = new RRDMAccountsClass();

        string filter1;
        //string filter2;

        string WBankId;

        int WSeqNumber;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;

        string WOperator;

        public Form85NV(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;

            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = WSignedId;

            textBoxMsgBoard.Text = "Choose Bank and OR Account for maintenance";

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


        // On ROW ENTER BANK
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
                textBoxExternalAcno.Text = Acc.AccNo;
                comboBoxCurrencies.Text = Acc.CurrNm;
                textBoxAccName.Text = Acc.AccName;
                textBoxInternalAcc.Text = Acc.AccNoInternal;
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
            if (textBoxExternalAcno.Text =="" || textBoxInternalAcc.Text =="")
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
            Acc.ShortAccID = "90";

            Acc.EntityNm = "Swift";
            Acc.EntityNo = "Swift";

            Acc.AtmNo = "";
            Acc.UserId = "";

            Acc.Operator = WOperator;

            Acc.InsertAccount();

            LoadBankAccounts(WBankId); 

        }
        // UPDATE ACCOUNT 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            string CurrNm = comboBoxCurrencies.Text;
            string AccName = textBoxAccName.Text;

            int WRowIndex = dataGridView2.SelectedRows[0].Index;

            Acc.ReadAndFindAccountBySeqNo(WSeqNumber);

            Acc.AccNo = textBoxExternalAcno.Text;
            Acc.CurrNm = comboBoxCurrencies.Text;
            Acc.AccName = "PAIR:_" + textBoxInternalAcc.Text + "_AND_" + textBoxExternalAcno.Text;

            Acc.AccNoInternal = textBoxInternalAcc.Text;

            Acc.ShortAccID = "90";

            Acc.EntityNm = "Swift";
            Acc.EntityNo = "Swift";

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


                LoadBankAccounts(WBankId);

                labelAccounts.Text = "ACCOUNTS FOR BankId : " + WBankId;

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
                textBoxExternalAcno.Text = "";
                textBoxAccName.Text = "";
                textBoxInternalAcc.Text = ""; 
            }
        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    
    }
}

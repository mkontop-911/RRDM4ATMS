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
    public partial class Form85 : Form
    {
        string WUserId;

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        //    string WUserBankId;

        string filter1;
        string filter2;

        string WAtmNo;
      
        string WBankId;
       
        int WSeqNumber;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;

        string WOperator;

        public Form85(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;

            WOperator = InOperator;

            InitializeComponent();

            comboBoxCategory.Items.Add("ATMs");
            comboBoxCategory.Items.Add("Users And CITs");
            comboBoxCategory.Items.Add("Reconciliation Categories");

            comboBoxCategory.Text = "ATMs";

            textBoxMsgBoard.Text = "Choose ATM or CIT Provider to maintain accounts";

            Gp.ParamId = "201"; // Currencies 
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            Gp.ParamId = "701"; // ACCount NM  
            comboBox3.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox3.DisplayMember = "DisplayValue";

            labelAccounts.Text = "ACCOUNTS FOR ATM";

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
        }

        private void Form85_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet12.TableATMsBasic' table. You can move, or remove it, as needed.
            this.tableATMsBasicTableAdapter.Fill(this.aTMSDataSet12.TableATMsBasic);
            

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

                filter1 = "Operator = '" + WOperator + "'";

                tableATMsBasicBindingSource.Filter = filter1;
                this.tableATMsBasicTableAdapter.Fill(this.aTMSDataSet12.TableATMsBasic);
            }

            
            if (comboBoxCategory.Text == "Users And CITs")
            {
                labelATM.Hide();
                panel2.Hide();

                labelUser.Show();
                panel4.Show();

                this.usersTableTableAdapter.Fill(this.aTMSDataSet16.UsersTable);
            }

            if (comboBoxCategory.Text == "Reconciliation Categories")
            {
                MessageBox.Show("Functionality under development"); 
            }
        }
// On ROW ENTER ATM 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            //comboBox1.Text = "1000"; 
            //WCitId = comboBox1.Text; 
            WSeqNumber = 0;
            textBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";

            WAtmNo = (string)rowSelected.Cells[0].Value;

            Ac.ReadAtm(WAtmNo);

            WBankId = Ac.BankId;


            LoadAtmAccounts();

            labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo; 
        }

// ON ROW ENTER USERS AND CIT 
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            WSeqNumber = 0;

            WUserId = (string)rowSelected.Cells[1].Value;

            textBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";

            Us.ReadUsersRecord(WUserId);


            LoadUserAccounts();

            labelAccounts.Text = "ACCOUNTS FOR USER or CIT : " + WUserId; 
        }

// ROW ENTER FOR ACCOUNT 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            if (dataGridView2.Rows.Count != 0)
            {
                WSeqNumber = (int)rowSelected.Cells[0].Value;
                Acc.ReadAndFindAccountBySeqNo(WSeqNumber);
                textBox1.Text = Acc.AccNo;
                comboBox2.Text = Acc.CurrNm;
                comboBox3.Text = Acc.AccName;

            }
        }

        // Load ATM Account Grid 
        private void LoadAtmAccounts()
        {

            filter2 = "Operator ='" + WOperator + "'" + " AND AtmNo = '" + WAtmNo + "'";
            accountsTableBindingSource.Filter = filter2;
            this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);
        }

        // Load USER Account Grid 
        private void LoadUserAccounts()
        {
            WAtmNo = "";
            filter2 = "Operator ='" + WOperator + "'" + " AND UserId ='" + WUserId + "'" + " AND AtmNo = '" + WAtmNo + "'";
            accountsTableBindingSource.Filter = filter2;
            this.accountsTableTableAdapter.Fill(this.aTMSDataSet62.AccountsTable);

        }
// ADD ACCOUNT
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Acc.AccNo = textBox1.Text;
            Acc.CurrNm = comboBox2.Text;
            Acc.AccName = comboBox3.Text;

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
                Acc.UserId = "1000";
            }
            else
            {
                Acc.AtmNo = "";
                Acc.UserId = WUserId;
            }

            Acc.ReadAndFindAccount(Acc.UserId, WOperator, WAtmNo, Acc.CurrNm, Acc.AccName);
            if (Acc.RecordFound == true)
            {
                MessageBox.Show("This account already exist");
                return;
            }
            Acc.Operator = WOperator;

            Acc.InsertAccount();

            if (comboBoxCategory.Text == "ATMs")
            {
                LoadAtmAccounts();

                labelAccounts.Text = "ACCOUNTS FOR ATM : " + WAtmNo;
            }
            else
            {
                LoadUserAccounts();
                labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;
            }
        }
// UPDATE ACCOUNT 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            string CurrNm = comboBox2.Text;
            string AccName = comboBox3.Text;

            int WRowIndex = dataGridView2.SelectedRows[0].Index;

            if (comboBoxCategory.Text == "ATMs")
            {
                WUserId = "1000";
            }
            else
            {
                //WUserId = WUserId;
            }
            Acc.ReadAndFindAccount(WUserId, WOperator, WAtmNo, CurrNm, AccName);
            if (Acc.RecordFound == true & Acc.SeqNumber != WSeqNumber)
            {
                MessageBox.Show("This account already exist");
                return;
            }

            Acc.AccNo = textBox1.Text;
            Acc.CurrNm = comboBox2.Text;
            Acc.AccName = comboBox3.Text;

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
                LoadUserAccounts();

                dataGridView2.Rows[WRowIndex].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;
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
                    LoadUserAccounts();
                    labelAccounts.Text = "ACCOUNTS FOR USER or CIT  : " + WUserId;
                }
            }
            else
            {
            }
        }
// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

    }
}

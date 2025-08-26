using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form66 : Form
    {
        int WErrId ;
        string WBankId ;
        int WRowIndex; 

        RRDMErrorsORExceptionsCharacteristics Ec = new RRDMErrorsORExceptionsCharacteristics();
        RRDMComboClass Cc = new RRDMComboClass();

        string WSignedId;
        int WSignRecordNo;
        //int WSecLevel;
        string WOperator;
        int WAction;

        public Form66(string InSignedId, int InSignRecordNo, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            //WSecLevel = InSecLevel;
            WOperator = InOperator;
            WAction = InAction;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            // Banks available for the seed bank 
            comboBoxBanks.DataSource = Cc.GetBanksIds(WOperator);
            comboBoxBanks.DisplayMember = "DisplayValue";

            // Values
            // 1 : Withdrawl EJournal Errors
            // 2 : Mainframe Withdrawl Errors
            // 3 : Deposit Errors EJournal 
            // 4 : Deposit Mainframe Errors
            // 5 : Created by user Errors = eg moving to suspense 
            // 6 : Empty 
            // 7 : Created System Errors 
            // 
            comboBox1.Items.Add("0 : Select");
            comboBox1.Items.Add("1 : Withdrawl EJournal Errors");
            comboBox1.Items.Add("2 : Mainframe Withdrawl Errors");
            comboBox1.Items.Add("3 : Deposit Errors EJournal");
            comboBox1.Items.Add("4 : Deposit Mainframe Errors");
            comboBox1.Items.Add("5 : Created by user Errors");
            comboBox1.Items.Add("6 : UnSpecified"); 
            comboBox1.Items.Add("7 : Created System Errors");
            comboBox1.Items.Add("8 : ITMX Origin");

            comboBox1.Text = "0 : Select";

        }

// LOAD Form 
        private void Form66_Load(object sender, EventArgs e)
        {
            Ec.ReadErrorsIDRecordsInTable(WOperator); 

            dataGridView1.DataSource = Ec.ExceptionsCharacteristicsTable.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Error Id 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Bank 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false; 
            dataGridView1.Columns[2].Width = 260; // Descr
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 70; // Action 

            dataGridView1.Columns[4].Width = 70; // Express     

        }

// On ROW Enter show the error details 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WErrId = (int)rowSelected.Cells[0].Value;
            WBankId = (string)rowSelected.Cells[1].Value;

            Ec.ReadErrorsIDRecord(WErrId, WBankId);

            textBox1.Text = Ec.ErrId.ToString();
            textBox2.Text = Ec.ErrDesc;

            comboBoxBanks.Text = WBankId; 

            if (Ec.ErrType == 1) comboBox1.Text = "1 : Withdrawl EJournal Errors";
            if (Ec.ErrType == 2) comboBox1.Text = "2 : Mainframe Withdrawl Errors";
            if (Ec.ErrType == 3) comboBox1.Text = "3 : Deposit Errors EJournal";
            if (Ec.ErrType == 4) comboBox1.Text = "4 : Deposit Mainframe Errors";
            if (Ec.ErrType == 5) comboBox1.Text = "5 : Created by user Errors";
            if (Ec.ErrType == 6) comboBox1.Text = "6 : UnSpecified";
            if (Ec.ErrType == 7) comboBox1.Text = "7 : Created System Errors";
         
            checkBox1.Checked = Ec.NeedAction;

            checkBoxDrCust.Checked = Ec.DrCust;
            checkBoxCrCust.Checked = Ec.CrCust;

            checkBox2.Checked = Ec.DrAtmCash;
            checkBox3.Checked = Ec.CrAtmCash;

            checkBox5.Checked = Ec.DrAtmSusp;
            checkBox6.Checked = Ec.CrAtmSusp;

            checkBox7.Checked = Ec.DrAccount3;
            checkBox8.Checked = Ec.CrAccount3;

            checkBoxExpress.Checked = Ec.TurboReconc;
            checkBoxMainOnly.Checked = Ec.MainOnly;

            textBoxCircular.Text = Ec.CircularDesc;

            // NOTES  
            Order = "Descending";
            WParameter4 = "Notes For " + "Error Id : " + WErrId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

        }

// If Dr 
        private void checkBoxDrCust_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDrCust.Checked == true & checkBoxCrCust.Checked == true)
            {
                checkBoxCrCust.Checked = false; 
                return; 
            }
        }
// If Cr 
        private void checkBoxCrCust_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDrCust.Checked == true & checkBoxCrCust.Checked == true)
            {
                checkBoxDrCust.Checked = false;
                return;
            }
        }
// 2 and 3 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true & checkBox3.Checked == true)
            {
                checkBox3.Checked = false;
                return;
            }
        }
// 3 and 2 
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true & checkBox3.Checked == true)
            {
                checkBox2.Checked = false;
                return;
            }
        }
// 5 and 6
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true & checkBox6.Checked == true)
            {
                checkBox6.Checked = false;
                return;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true & checkBox6.Checked == true)
            {
                checkBox5.Checked = false;
                return;
            }
        }
// 7 and 8 
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true & checkBox8.Checked == true)
            {
                checkBox8.Checked = false;
                return;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true & checkBox8.Checked == true)
            {
                checkBox7.Checked = false;
                return;
            }
        }
// 9 and 10 
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked == true & checkBox10.Checked == true)
            {
                checkBox10.Checked = false;
                return;
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked == true & checkBox10.Checked == true)
            {
                checkBox9.Checked = false;
                return;
            }
        }


        //**********************************************************************
        // START NOTES 
        //**********************************************************************
        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes(); 

// Add Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
                WErrId = int.Parse(textBox1.Text); 
                Form197 NForm197;
                string WParameter3 = "";
                string WParameter4 = "Notes For " + "Error Id : " + WErrId.ToString() ;
                string SearchP4 = "";
                string WMode = "Update";
                NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
                NForm197.FormClosed += NForm197_FormClosed;
                NForm197.ShowDialog();
        
        }
        // Number of Notes 
        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Notes For " + "Error Id : " + WErrId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
// Add new Error Id 
        private void buttonAdd_Click(object sender, EventArgs e)
        {         
           //
            //  Validation
            //
            if (int.TryParse(textBox1.Text, out Ec.ErrId))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number in field ErrorId. ");
                return;
            }

            Ec.ReadErrorsIDRecord(Ec.ErrId, WBankId);

            if (Ec.RecordFound == true)
            {
                MessageBox.Show(textBox1.Text, "This Error Id for this Bank Already exist.  ");
                return;
            }

            WErrId = Ec.ErrId; 

            Ec.ErrDesc = textBox2.Text;
            Ec.BankId = comboBoxBanks.Text;

            Ec.DateInserted = DateTime.Now; 

            if (comboBox1.Text == "1 : Withdrawl EJournal Errors") Ec.ErrType = 1 ;
            if (comboBox1.Text == "2 : Mainframe Withdrawl Errors") Ec.ErrType = 2;
            if (comboBox1.Text == "3 : Deposit Errors EJournal") Ec.ErrType = 3;
            if (comboBox1.Text == "4 : Deposit Mainframe Errors") Ec.ErrType = 4;
            if (comboBox1.Text == "5 : Created by user Errors") Ec.ErrType = 5;
            if (comboBox1.Text == "6 : UnSpecified") Ec.ErrType = 6;
            if (comboBox1.Text == "7 : Created System Errors") Ec.ErrType = 7;

            Ec.NeedAction = checkBox1.Checked;

            Ec.DrCust = checkBoxDrCust.Checked  ;
            Ec.CrCust = checkBoxCrCust.Checked  ;

            Ec.DrAtmCash = checkBox2.Checked ;
            Ec.CrAtmCash = checkBox3.Checked ; 

            Ec.DrAtmSusp = checkBox5.Checked  ;
            Ec.CrAtmSusp = checkBox6.Checked  ;

            Ec.DrAccount3 = checkBox7.Checked ;
            Ec.CrAccount3 = checkBox8.Checked ;

            Ec.TurboReconc = checkBoxExpress.Checked ;
            Ec.MainOnly = checkBoxMainOnly.Checked ;

            Ec.CircularDesc = textBoxCircular.Text ;

            Ec.Operator = WOperator ; 

            Ec.InsertErrorCharacteristics();

            Form66_Load(this, new EventArgs());

        }
// Update 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Ec.ReadErrorsIDRecord(WErrId, WBankId);

            WErrId = Ec.ErrId;

            Ec.ErrDesc = textBox2.Text;
            Ec.BankId = comboBoxBanks.Text;

            Ec.DateInserted = DateTime.Now;

            if (comboBox1.Text == "1 : Withdrawl EJournal Errors") Ec.ErrType = 1;
            if (comboBox1.Text == "2 : Mainframe Withdrawl Errors") Ec.ErrType = 2;
            if (comboBox1.Text == "3 : Deposit Errors EJournal") Ec.ErrType = 3;
            if (comboBox1.Text == "4 : Deposit Mainframe Errors") Ec.ErrType = 4;
            if (comboBox1.Text == "5 : Created by user Errors") Ec.ErrType = 5;
            if (comboBox1.Text == "6 : UnSpecified") Ec.ErrType = 6;
            if (comboBox1.Text == "7 : Created System Errors") Ec.ErrType = 7;

            Ec.NeedAction = checkBox1.Checked;

            Ec.DrCust = checkBoxDrCust.Checked;
            Ec.CrCust = checkBoxCrCust.Checked;

            Ec.DrAtmCash = checkBox2.Checked;
            Ec.CrAtmCash = checkBox3.Checked;

            Ec.DrAtmSusp = checkBox5.Checked;
            Ec.CrAtmSusp = checkBox6.Checked;

            Ec.DrAccount3 = checkBox7.Checked;
            Ec.CrAccount3 = checkBox8.Checked;

            Ec.TurboReconc = checkBoxExpress.Checked;
            Ec.MainOnly = checkBoxMainOnly.Checked;

            Ec.CircularDesc = textBoxCircular.Text;

            Ec.Operator = WOperator;

            Ec.UpdateErrorCharacteristics(WOperator, WErrId, WBankId); 

            Form66_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }

// DELETE  
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            

            if (MessageBox.Show("Warning: Do you want to delete this Error Id?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                Ec.DeleteErrorId(WErrId, WBankId); 

                Form66_Load(this, new EventArgs());
            }
            else
            {
                return;
            }

        }
//This close 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.IO;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form143 : Form
    {
        Form44 NForm44;
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMBanks Ba = new RRDMBanks();

        //string SelectionCriteria;

        string WChosenBank;
        int WFunctionNo;

        int WMode;

        int WRow;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        //  bool WPrive;
        public Form143(string InSignedId, int InSignRecordNo, String InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Choose a Bank to take action on it or Add a new Bank";

            Us.ReadUsersRecord(WSignedId); 

            Ba.ReadBank(Us.BankId);
            if (Ba.SettlementBank == true)
            {
                WMode = 2; // Settlement Bank
                buttonDelete.Hide();
                buttonAdd.Hide();
            }
            else
            {
                WMode = 1;
                buttonDelete.Show();
                buttonAdd.Show();
            }
        }

        private void Form143_Load(object sender, EventArgs e)
        {

            //if (WSignedId == "999") // RRDM MASTER USER 
            //{
            //    SelectionCriteria = "";
            //}
            //else
            //{
            //    SelectionCriteria = " WHERE Operator = '" + WOperator + "' ";
            //}

            Ba.ReadBanksForDataTableByOperator(WOperator, WMode);
            //Ba.ReadBanksForDataTable(SelectionCriteria, WMode);

            dataGridView1.DataSource = Ba.BanksDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                buttonUpdate.Hide();
                buttonDelete.Hide();
                textBoxMsgBoard.Text = "Add the seed Bank/Company";
            }
            else
            {
                buttonUpdate.Show();
                if (WMode == 1) buttonDelete.Show();
            }

            dataGridView1.Columns[0].Width = 100; // Bank Id 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Width = 150; // Short Name 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Width = 210; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Width = 100; // Country 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Width = 150; // Date of Opening 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            if (WMode == 2)
            {
                dataGridView1.Columns[5].Width = 70; // Ccy
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[6].Width = 110; // Settlement Acc
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[7].Width = 110; // Clearing Acc
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
          
          

            //BanksDataTable.Columns.Add("BankId", typeof(string));
            //BanksDataTable.Columns.Add("ShortName", typeof(string));
            //BanksDataTable.Columns.Add("Full Name", typeof(string));
            //BanksDataTable.Columns.Add("Country", typeof(string));
            //BanksDataTable.Columns.Add("DateInRRDM", typeof(string));

            //if (InMode == 2)
            //{
            //    BanksDataTable.Columns.Add("Ccy", typeof(string));
            //    BanksDataTable.Columns.Add("SettlementAcc", typeof(string));
            //    BanksDataTable.Columns.Add("Settl_Clearing", typeof(string));
            //}
        }

        // CHOOSE A BANK 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WChosenBank = rowSelected.Cells[0].Value.ToString();

            Ba.ReadBank(WChosenBank);


            if (Ba.Logo == null)
            {
                pictureBox2.Image = null;
                MessageBox.Show("No Logo assigned yet!");
            }
            else
            {
                MemoryStream ms = new MemoryStream(Ba.Logo);
                pictureBox2.Image = Image.FromStream(ms);
            }
        }

        //
        // GO TO UPDATE CHOSEN BANK 
        //
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

            WFunctionNo = 1;
            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog();
        }

        // On Form close 
        void NForm44_FormClosed(object sender, FormClosedEventArgs e)
        {

            WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form143_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // GO TO OPEN A NEW BANK

        private void buttonAdd_Click(object sender, EventArgs e)
        {

            WChosenBank = "";

            if (dataGridView1.Rows.Count == 0) // This is the process when Seed Bank will be opened 
            {
                WChosenBank = WOperator;
            }

            WFunctionNo = 2;

            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog(); ;
        }
        // Delete 
        private void button1_Click(object sender, EventArgs e)
        {

            //   WRow = dataGridView1.SelectedRows[0].Index;
            WFunctionNo = 3;

            NForm44 = new Form44(WSignedId, WSignRecordNo, WOperator, WChosenBank, WFunctionNo);
            NForm44.FormClosed += NForm44_FormClosed;
            NForm44.ShowDialog(); ;
        }
        //Finish
        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}

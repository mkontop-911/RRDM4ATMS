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

namespace RRDM4ATMsWin
{
    public partial class Form60 : Form
    {
        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        // NOTES 
        string Order;

        string SelectionCriteria;


        bool WithDate;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int WRowIndex;

        string WBankId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2150, 11, 21);

        //      string Gridfilter;
        int WDisputeNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    
        string WCardNo;

        public Form60(string InSignedId, int InSignRecordNo, string InOperator, string InCardNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
          
            WCardNo = InCardNo;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            labelStep1.Text = "Disputes for Card No: " + WCardNo;
      
        }
        // LOAD 
        private void Form60_Load(object sender, EventArgs e)
        {

            SelectionCriteria = "Operator ='" + WOperator + "' AND CardNo='" + WCardNo + "'";

            WithDate = false;
            Di.ReadDisputesInTable(SelectionCriteria, NullPastDate, WithDate);

            dataGridView1.DataSource = Di.DisputesSelected.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 70; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 120; // Name
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 60; // Type
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;         

            dataGridView1.Columns[3].Width = 90; // Origin 
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 100; // date 
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 70; // Oener 
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 70; // Bank
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 70; // resp branch 
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        // ON ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WDisputeNo = (int)rowSelected.Cells[0].Value;

            Di.ReadDispute(WDisputeNo);

            WBankId = Di.BankId;

            tbCustName.Text = Di.CustName;
            tbCardNo.Text = Di.CardNo;
            tbAccNo.Text = Di.AccNo;
            tbCustPhone.Text = Di.CustPhone;
            tbCustEmail.Text = Di.CustEmail;
            tbComments.Text = Di.DispComments;

            if (Di.HasOwner == true)
            {
                Us.ReadUsersRecord(Di.OpenByUserId);

                textBoxOwner.Text = Di.OpenByUserId + " " + Us.UserName;
            }
            else
            {
                textBoxOwner.Text = "Dispute has no Owner. ";
            }

            //TEST
            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator, TDispType, Di.DispType.ToString(), "", "");

            textBox4.Text = Gp.OccuranceNm;

            if (Di.DispType == 6) // Means Other type of Dispute 
            {
                label3.Show();
                textBox8.Show();
                textBox8.Text = Di.OtherDispTypeDescr;
            }
            else
            {
                label3.Hide();
                textBox8.Hide();
            }

            textBox1.Text = "A-Grade";
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            checkBox1.Checked = false;

            if (Di.Active == true)
            {
                checkBox2.Checked = true;
                textBox6.Hide();

                label13.Show();
                label15.Show();
                textBox7.Show();
                textBox5.Show();
            }
            else
            {
                checkBox2.Checked = false;
                textBox6.Show();

                label13.Hide();
                label15.Hide();
                textBox7.Hide();
                textBox5.Hide();

                pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT;
            }

            textBox2.Text = Di.TargetDate.ToString();
            // Time remaining 

            // DateTime Difference ;
            if (Di.Active == true)
            {
                TimeSpan Remain = Di.TargetDate - DateTime.Now;
                textBox7.Text = Remain.Days.ToString();
                textBox5.Text = Remain.TotalHours.ToString("#,##0.00");

                // ASSIGN TRAFFIC LIGHTS
                Gp.ReadParametersSpecificId(WOperator, "601", "4", "", ""); // < is Red 
                int QualityRange1 = (int)Gp.Amount;

                Gp.ReadParametersSpecificId(WOperator, "602", "4", "", ""); // > is Green 
                int QualityRange2 = (int)Gp.Amount;

                if (Remain.TotalHours > QualityRange2)
                {
                    pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT;
                }
                if (Remain.TotalHours >= QualityRange1 & Remain.TotalHours <= QualityRange2)
                {
                    pictureBox2.BackgroundImage = Properties.Resources.YELLOW;
                }
                if (Remain.TotalHours < QualityRange1)
                {
                    pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT;
                }
            }

            // NOTES for Attachements 
            Order = "Descending";
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            string filter = "DisputeNumber =" + WDisputeNo;

            disputesTransTableBindingSource.Filter = filter;
            //  dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Descending);
            this.disputesTransTableTableAdapter.Fill(this.aTMSDataSet54.DisputesTransTable);

        }

        // NOTES
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
            string SearchP4 = "";
            string WMode;
            if (Di.Active == true) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }
      
        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form60_Load(this, new EventArgs());
        }
//Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}

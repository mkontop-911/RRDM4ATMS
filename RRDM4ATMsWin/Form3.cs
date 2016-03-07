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
    public partial class Form3 : Form
    {
        Form4 NForm4;

        Form5 NForm5;

        Form3_DispOwners NForm3DispOwners; 

        //Form197 NForm197; 

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
     
        public Form3(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            
            textBoxMsgBoard.Text = "Please give priority on the delayed disputes";  
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet3.DisputesTransTable' table. You can move, or remove it, as needed.
            this.disputesTransTableTableAdapter.Fill(this.aTMSDataSet3.DisputesTransTable);
                SelectionCriteria = "Operator ='" + WOperator + "'";
            
                WithDate = false;
                Di.ReadDisputesInTable(SelectionCriteria, NullPastDate, WithDate); 

                dataGridView1.DataSource = Di.DisputesSelected.DefaultView;

                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

                dataGridView1.Columns[0].Width = 70; // Id
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[1].Width = 120; // Name
                //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns[2].Width = 60 ; // Type
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
                //dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.LightSlateGray;

                //// DATA TABLE ROWS DEFINITION 
                //DisputesSelected.Columns.Add("DispId", typeof(int));    // 0
                //DisputesSelected.Columns.Add("CustName", typeof(string)); // 1
                //DisputesSelected.Columns.Add("DispType", typeof(int)); // 2
                //DisputesSelected.Columns.Add("Origin", typeof(string)); // 3
                //DisputesSelected.Columns.Add("OpenDate", typeof(DateTime)); // 4
                //DisputesSelected.Columns.Add("OwnerId", typeof(string)); // 5
                //DisputesSelected.Columns.Add("BankId", typeof(string)); // 6 
                //DisputesSelected.Columns.Add("RespBranch", typeof(string)); // 7
        }

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
                buttonChangeOwner.Text = "Change Owner"; 
            }
            else
            {
                textBoxOwner.Text = "Dispute has no Owner. ";
                buttonChangeOwner.Text = "Assign Owner"; 
            }

            //TEST
            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator,TDispType, Di.DispType.ToString(), "", "");
         
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
                Gp.ReadParametersSpecificId(WOperator,"601", "4", "", ""); // < is Red 
                 int QualityRange1 = (int)Gp.Amount;

                 Gp.ReadParametersSpecificId(WOperator,"602", "4", "", ""); // > is Green 
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
            this.disputesTransTableTableAdapter.Fill(this.aTMSDataSet3.DisputesTransTable);

        }

        // Transactions for this dispute 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
        
     
        // GO TO NEXT STEP FURTHER INVESTIGATION 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if ( Di.OwnerId != WSignedId & Di.Active == true )
            {
                MessageBox.Show("Warning : You are not the owner of this Acive dispute.");
            }
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            NForm4 = new Form4(WSignedId, WSignRecordNo,WOperator, WBankId, WDisputeNo);
            NForm4.FormClosed += NForm4_FormClosed;
            NForm4.ShowDialog();
        }

        void NForm4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
        // UPDATE DATA Of DISPUTE IF NEEDED
        private void buttonUpdateData_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int From = 4; // Coming from Dispute management 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Di.CardNo, 0, 0, WDisputeNo,"" ,From, "");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog();
        }

        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
        // CALL TO ADD AND SEE NOTES 
        private void buttonNotes_Click(object sender, EventArgs e)
        {
            
        }


    
// Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
                Form197 NForm197;
                string WParameter3 = "";
                string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.DispId.ToString();
                string SearchP4 = "";
                string WMode ;
                if (Di.Active == true) WMode = "Update";
                else WMode = "Read";
                NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
                NForm197.FormClosed += NForm197_FormClosed;
                NForm197.ShowDialog();     
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

// Change Owner 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            if (Di.Active == false)
            {
                MessageBox.Show("This Dispute is settled! You cannot assign new owner. ");
                return; 
            }
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            NForm3DispOwners = new Form3_DispOwners(WSignedId, WSignRecordNo, WOperator, WDisputeNo);
            NForm3DispOwners.FormClosed += NForm3DispOwners_FormClosed;
            NForm3DispOwners.ShowDialog(); 
        }

        void NForm3DispOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

    
       
     
        //**********************************************************************
        // END NOTES 
        //**********************************************************************         
    }
}

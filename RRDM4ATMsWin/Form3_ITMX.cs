using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form3_ITMX : Form
    {

        Form109ITMX NForm109ITMX;

        Form5 NForm5;

        Form3_DispOwnersITMX NForm3DispOwnersNAITMX;

        //Form197 NForm197; 

        RRDMDisputesTableClassITMX Di = new RRDMDisputesTableClassITMX();

        RRDMDisputeTransactionsClassITMX Dt = new RRDMDisputeTransactionsClassITMX();

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        // NOTES 
        string Order;

        int WMaskRecordId;
        int WDispTranNo;

        bool SingleCustomer; 

        string SelectionCriteria;

        bool ITMXUser;

        bool WithDate;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int WRowIndex;

        string WBankId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2150, 11, 21);

        int WDisputeNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCustomerUniqueId;
        string WMode;  

        public Form3_ITMX(string InSignedId, int InSignRecordNo, string InOperator, string InCustomerUniqueId, string InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCustomerUniqueId = InCustomerUniqueId;
            WMode = InMode; // "VIEW" OR "UPDATE" 

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUserId.Text = WSignedId; 

            if (WCustomerUniqueId != "")
            {
                // Means we should show disputes for only this customer 
                SingleCustomer = true;
                textBoxMsgBoard.Text = "See the customers other Disputes.";
                labelStep1.Text = "Disputes Management For Customer : " + WCustomerUniqueId;
            }
            else
            {
                textBoxMsgBoard.Text = "Please give priority on the delayed disputes";

                Us.ReadUsersRecord(WSignedId);
                WBankId = Us.BankId;

                if (Us.Operator == WBankId)
                {
                    ITMXUser = true;
                    labelStep1.Text = "Disputes Management For Operator : " + WOperator;
                }
                else
                {
                    ITMXUser = false;
                    labelStep1.Text = "Disputes Management For our Bank : " + WBankId;
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

            if (SingleCustomer == true )
            {
                //Show Disputes only for this customer 
                SelectionCriteria = "Operator ='" + WOperator + "' AND CardNo='" + WCustomerUniqueId + "'";
                buttonNext.Hide();
                buttonChangeOwner.Hide(); 
            }
            else
            {
                if (ITMXUser == true)
                {
                    SelectionCriteria = "Operator ='" + WOperator + "'";
                }
                else
                {
                    SelectionCriteria = "BankId ='" + WBankId + "'";
                }
            }
           
            WithDate = false;
            Di.ReadDisputesInTable(SelectionCriteria, NullPastDate, WithDate);

            dataGridView1.DataSource = Di.DisputesSelected.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Disputes to Manage.");
                this.Dispose();
                return;
            }

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 70; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

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

            Dt.ReadAllTranForDispute(WDisputeNo); // Find totals - Transactions for dispute

            Di.ReadDispute(WDisputeNo); // Read Specific Dispute details 

            WBankId = Di.BankId;

            tbCustName.Text = Di.CustName;
            tbCardNo.Text = Di.CardNo;
            tbAccNo.Text = Di.AccNo;
            tbCustPhone.Text = Di.CustPhone;
            tbCustEmail.Text = Di.CustEmail;
            tbComments.Text = Di.DispComments;

            tbComments.ForeColor = Color.Red; 

            if (Di.HasOwner == true)
            {
                Us.ReadUsersRecord(Di.OwnerId);

                textBoxOwner.Text = Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Dispute has no Owner. ";
                buttonChangeOwner.Text = "Assign Owner";
            }

            //TEST
            string TDispType = "251"; // Parameter Id for dispute types

            Gp.ReadParametersSpecificId(WOperator, TDispType, Di.DispType.ToString(), "", "");

            textBox4.Text = Gp.OccuranceNm;

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

                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT;
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
                    pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT;
                }
                if (Remain.TotalHours >= QualityRange1 & Remain.TotalHours <= QualityRange2)
                {
                    pictureBox2.BackgroundImage = appResImg.YELLOW;
                }
                if (Remain.TotalHours < QualityRange1)
                {
                    pictureBox2.BackgroundImage = appResImg.RED_LIGHT;
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

            // Show GRID Of Dispute transactions 

            string filter = "DisputeNumber =" + WDisputeNo;

            Dt.ReadDisputeTransDataTable(filter);

            ShowGridDisputeTrans();

        }

        // Transactions for this dispute 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WDispTranNo = (int)rowSelected.Cells[0].Value;

            Dt.ReadDisputeTran(WDispTranNo);
            WMaskRecordId = Dt.MaskRecordId;

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, WMaskRecordId);

            textBoxITMXUniqueId.Text = Mp.ITMXUniqueTxnRef.ToString();

            if ((ITMXUser == true & WOperator != WBankId) || WMode == "VIEW")
            {
                buttonBank.Hide();
                buttonNext.Hide();
                textBoxMsgBoard.Text = "VIEW ONLY";
                buttonChangeOwner.Hide(); 
            }
            else
            {
                buttonBank.Show();
                if (SingleCustomer == false) buttonNext.Show();
                buttonBank.Text = "Investigate " + WBankId;
                buttonITMX.Text = "Investigate " + WOperator;          
                buttonChangeOwner.Show();
                textBoxMsgBoard.Text = "Please give priority on the delayed disputes";
            }
        }

        // GO TO NEXT STEP FURTHER INVESTIGATION 
        private void buttonNext_Click(object sender, EventArgs e)
        {

            if (Di.OwnerId != WSignedId & Di.Active == true)
            {
                MessageBox.Show("Error : You are not the owner of this dispute.");
                return;
            }


            WRowIndex = dataGridView1.SelectedRows[0].Index;


            if (Dt.OpenDispTran == 0)
            {

                if (MessageBox.Show("MSG986 - Dispute is already settled."
                    + "Do you want to continue to review action?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                               == DialogResult.Yes)
                {
                    int WOrigin = 1; // From Requestor 
                    NForm109ITMX = new Form109ITMX(WSignedId, WSignRecordNo, WOperator, WDisputeNo, Dt.DispTranNo, WMaskRecordId, WOrigin);
                    NForm109ITMX.FormClosed += NForm109ITMX_FormClosed;
                    NForm109ITMX.ShowDialog();
                }
                else
                {
                    return;
                }

            }
            else
            {
                //Ap.ReadAuthorizationForDisputeAndTransaction(WDispNo, WTranNo);
                //if (Ap.RecordFound == true)
                //{
                //    WAuthSeqNumber = Ap.SeqNumber;
                //}
                //else
                //{
                //    WAuthSeqNumber = 0 ;
                //}

                int WOrigin = 1;
                NForm109ITMX = new Form109ITMX(WSignedId, WSignRecordNo, WOperator, WDisputeNo, Dt.DispTranNo, WMaskRecordId, WOrigin);
                NForm109ITMX.FormClosed += NForm109ITMX_FormClosed;
                NForm109ITMX.ShowDialog();
            }
        }
        void NForm109ITMX_FormClosed(object sender, FormClosedEventArgs e)
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
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Di.CardNo, 0, 0, WDisputeNo, "", From, "");
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
            string WMode;
            if (Di.Active == true) WMode = "Update";
            else WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
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

            NForm3DispOwnersNAITMX = new Form3_DispOwnersITMX(WSignedId, WSignRecordNo, WOperator, WDisputeNo);
            NForm3DispOwnersNAITMX.FormClosed += NForm3DispOwners_FormClosed;
            NForm3DispOwnersNAITMX.ShowDialog();
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

        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridDisputeTrans()
        {
            dataGridView2.DataSource = Dt.DisputeTransDataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            dataGridView2.Columns[0].Width = 70; // DispTranNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 70; // MaskRecordId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 100; //  TranDate
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView2.Columns[3].Width = 80; // TranAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[4].Width = 80; // DisputedAmt
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[5].Width = 80; // DecidedAmount 
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //
            // DATA TABLE ROWS DEFINITION 
            //
            //ATMsDetailsDataTable.Columns.Add("DispTranNo", typeof(int));
            //ATMsDetailsDataTable.Columns.Add("MaskRecordId", typeof(int));
            //ATMsDetailsDataTable.Columns.Add("TranDate", typeof(DateTime));
            //ATMsDetailsDataTable.Columns.Add("TranAmount", typeof(string));
            //ATMsDetailsDataTable.Columns.Add("DisputedAmt", typeof(string));
            //ATMsDetailsDataTable.Columns.Add("DecidedAmount", typeof(string));
        }
        //Investigate ITMX 
        private void button3_Click(object sender, EventArgs e)
        {
            Form80bΙΤΜΧ NForm80bΙΤΜΧ;

            string WFunction = "DisputeInvestigation";

            NForm80bΙΤΜΧ = new Form80bΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, "", 0, WMaskRecordId, WFunction);
            //NForm80bΙΤΜΧ.FormClosed += NForm80b_FormClosed;
            NForm80bΙΤΜΧ.ShowDialog();
        }
        //Investigate in Bank's files
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("An interface with Bank Files can be built to investigate transaction");
            return;
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
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

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMUsers_Applications_Roles Uar = new RRDMUsers_Applications_Roles();

        // NOTES 
        string Order;

        string SelectionCriteria;

        string WSecurityLevel; // 5 is dispute officer, 6 is dispute manager

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
        string WField;
        DateTime WDtFrom;
        DateTime WDtTo;
        int WMode;

        public Form3(string InSignedId, int InSignRecordNo, string InOperator,
                          string InField, DateTime InDtFrom, DateTime InDtTo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WField = InField;
            WDtFrom = InDtFrom;
            WDtTo = InDtTo;
            WMode = InMode; // 14 Disputes for Branch
                            // 15 Disputes for FULL CARD
                            // 16 Disputes for Phone Number 
                            // 13 Unique Id
                            // 12 From Dispute Manager
                            // 11 From Owner 
            InitializeComponent();

            Uar.ReadUsersVsApplicationsVsRolesByApplication(WSignedId, "ATMS/CARDS");
            Di.ReadDisputeOwnerTotal(WSignedId);
            WSecurityLevel = Uar.SecLevel;

            if (WSecurityLevel == "04" || WSecurityLevel == "06" || Di.DisputeOwnerTotal > 0
                || WMode == 14 || WMode == 15 || WMode == 16)
            {
                // OK These are allowed , Security office and Security manager 
            }
            else
            {
                MessageBox.Show("You are not allowed for this selection" + Environment.NewLine
                    + "You must be dispute officer or a dispute manager " + Environment.NewLine
                    + "OR Must be a reconciliation officer with disputes assign to you. " + Environment.NewLine
                     + "OR You select disputes for Branch. " + Environment.NewLine
                    );

            }
            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            UserId.Text = InSignedId;

            textBoxMsgBoard.Text = "Please give priority on the delayed disputes";
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //
            // SWITCH WMode
            //
            switch (WMode)
            {
                case 14: // Show Diputes for Branch
                    {
                        labelStep1.Text = "DISPUTES created by Branch.." + WField;
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RespBranch ='" + WField + "'";
                        Di.ReadDisputesInTableByRangeAndSelectionCriteria(WOperator, WSignedId, SelectionCriteria,
                        WDtFrom, WDtTo);
                        break;
                    }
                case 15: // Show Diputes for Card
                    {
                        labelStep1.Text = "DISPUTES created for Card.." + WField;
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND CardNo ='" + WField + "'";
                        Di.ReadDisputesInTableByRangeAndSelectionCriteria(WOperator, WSignedId, SelectionCriteria,
                        WDtFrom, WDtTo);
                        break;
                    }
                case 16: // Show Diputes for Phone 
                    {
                        labelStep1.Text = "DISPUTES created for Phone.." + WField;
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND CustPhone ='" + WField + "'";
                        Di.ReadDisputesInTableByRangeAndSelectionCriteria(WOperator, WSignedId, SelectionCriteria,
                        WDtFrom, WDtTo);
                        break;
                    }
                case 13: // Show Diputes for Unique record Id
                    {
                        // Find Dispute Number            
                        // 
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND DispId =" + WField;

                        Di.ReadDisputesInTableByRangeAndSelectionCriteria(WOperator, WSignedId, SelectionCriteria,
                        WDtFrom, WDtTo);

                        break;
                    }
                case 12: // Show All for Manager
                    {
                        WithDate = false;
                        Di.ReadDisputesInTable(WOperator, WSignedId, "", NullPastDate, WithDate, WMode);
                        break;
                    }
                case 11: // Show all the owner has
                    {
                        WithDate = false;
                        Di.ReadDisputesInTable(WOperator, WSignedId, "", NullPastDate, WithDate, WMode);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            if (Di.DisputesSelected.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to show");
                this.Dispose();
                return;
            }
            // Data Source 
            dataGridView1.DataSource = Di.DisputesSelected.DefaultView;

            //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 70; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[1].Width = 150; // Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 70; // Type
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;         

            dataGridView1.Columns[3].Width = 70; // Settled
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 130; // 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 80; // Owner 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 70; // Bank
                                                 //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 100; // resp branch 
                                                  //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            //dataGridView1.Columns[5].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            //// DATA TABLE ROWS DEFINITION 

        }
        string WCreator;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WDisputeNo = (int)rowSelected.Cells[0].Value;

            Di.ReadDispute(WDisputeNo);

            WBankId = Di.BankId;
            WCreator = Di.DisputeCreatorId;
            tbCustName.Text = Di.CustName;
            tbCardNo.Text = Di.CardNo;
            tbAccNo.Text = Di.AccNo;
            tbCustPhone.Text = Di.CustPhone;
            tbCustEmail.Text = Di.CustEmail;
            tbComments.Text = Di.DispComments;

            if (Di.HasOwner == true)
            {
                Us.ReadUsersRecord(Di.OwnerId);

                textBoxOwner.Text = Di.OwnerId + " " + Us.UserName;
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
                textBoxStatus.Hide();

                label13.Show();
                label15.Show();
                textBox7.Show();
                textBox5.Show();
            }
            else
            {
                checkBox2.Checked = false;
                textBoxStatus.Show();

                label13.Hide();
                label15.Hide();
                textBox7.Hide();
                textBox5.Hide();

                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT;
            }

            textBoxTargetDate.Text = Di.TargetDate.ToString();
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

            //string filter = "DisputeNumber =" + WDisputeNo;

            Dt.ReadDisputeTransDataTable(WDisputeNo);

            ShowGridDisputeTrans();

        }

        // Transactions for this dispute 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            int WDispTranNo = (int)rowSelected.Cells[0].Value;

            Dt.ReadDisputeTran(WDispTranNo);

            if(Dt.ClosedDispute == false)
            {
                // Active txn Case 
                checkBox2.Checked = true;
                textBoxStatus.Hide();
            }
            else
            {
                // Close txn case 
                checkBox2.Checked = false;
                textBoxStatus.Show(); // settled dispute 
            }
            
            //if (Dt.ClosedDispute == true)
            //{
            string WActionDesc = "No Action Yet";
                if (Dt.DisputeActionId == 1) WActionDesc = "Customer was credited";
                if (Dt.DisputeActionId == 2) WActionDesc = "Customer was credited";
                if (Dt.DisputeActionId == 3) WActionDesc = "Customer was Debited";
                if (Dt.DisputeActionId == 4) WActionDesc = "Customer was Debited";
                if (Dt.DisputeActionId == 5) WActionDesc = "Postponed";
                if (Dt.DisputeActionId == 6) WActionDesc = "Dispute Txn Cancelled";

                textBoxTransStatus.Show();

            if (Dt.ClosedDispute == false)
            {
                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
                Ap.ReadAuthorizationForDisputeAndTransaction(Dt.DisputeNumber, Dt.DispTranNo);
                if (Ap.RecordFound == true) // OPEN AUTHORISATION
                {
                    WActionDesc = "No Action Yet But Under Authorisation Process";
                }
                //else
                //{
                //    WActionDesc = "No Action Yet";
                //}
                
            }

            //if (WActionDesc == "No Action Yet" )
            //{
            //    textBoxTransStatus.Text = "Action Type: " + WActionDesc;
            //}
            //else
            //{
                textBoxTransStatus.Text =
                     "Action Type: " + WActionDesc + Environment.NewLine
                    + "Date:..." + Dt.ActionDtTm.ToShortDateString() + Environment.NewLine
                    + "Maker :.." + Dt.AuthorOriginator + Environment.NewLine
                    + "Author:.." + Dt.Authoriser + Environment.NewLine;
            //}
                
           
        }

        // GO TO NEXT STEP FURTHER INVESTIGATION 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            Di.ReadDispute(WDisputeNo);
            if (Di.DispFrom == 7)
            {
                // Came from reconciliation or Replenishment workflow
                // Check if the workflow is closed
                Dt.ReadAllTranForDispute(Di.DispId);
              
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                Mpa.ReadInPoolTransSpecificUniqueRecordId(Dt.UniqueRecordId,2);
                if (Mpa.Authoriser == "")
                {
                    MessageBox.Show("This dispute was created during reconciliation"+Environment.NewLine
                       // + "The workflow is not completed yet." + Environment.NewLine
                       //  + "Finish the workflow please before you proceed." 
                        );
                   // return; 
                }
            }


            //if (Di.OwnerId.Trim().Equals(WSignedId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            //    & Di.Active == true)

            if (WSignedId.Trim().ToUpper() == Di.OwnerId.Trim().ToUpper())
            {
                ///
            }
            else
            {
                MessageBox.Show("Warning : You are not the owner of this Active dispute.");
            }
            //if (Di.OwnerId.Trim() != WSignedId.Trim() & Di.Active == true)
            //{
            //    MessageBox.Show("Warning : You are not the owner of this Active dispute.");
            //}
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            NForm4 = new Form4(WSignedId, WSignRecordNo, WOperator, WBankId, WDisputeNo);
            NForm4.FormClosed += NForm4_FormClosed;
            NForm4.ShowDialog();
        }
        int WRow ;
        int scrollPosition ;
        void NForm4_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            WRow = dataGridView1.SelectedRows[0].Index;
            scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
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
            if (WSecurityLevel != "06")
            {
                MessageBox.Show("You are not allowed to do this. You are not a Dispute Manager!");
                return;
            }

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
            
            dataGridView2.Columns[0].Width = 50; // DispTranNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].HeaderText = "Disp TranNo"; 

            dataGridView2.Columns[1].Width = 70; // UniqueRecordId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].HeaderText = "Disp TranNo";

            dataGridView2.Columns[2].Width = 100; //  TranDate
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView2.Columns[3].Width = 60; // TranAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[3].HeaderText = "Tran Amt";

            dataGridView2.Columns[4].Width = 60; // DisputedAmt
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].HeaderText = "Disputed";

            dataGridView2.Columns[5].Width = 60; // DecidedAmount
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[5].HeaderText = "Decided";

            dataGridView2.Columns[6].Width = 100; // ActionDate 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[6].HeaderText = "Action Date";

            dataGridView2.Columns[7].Width = 50; // ClosedDispute 
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].HeaderText = "Closed";

            dataGridView2.Columns[8].Width = 120; // Action Desc
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //DisputeTransDataTable.Columns.Add("DispTranNo", typeof(int));
            //DisputeTransDataTable.Columns.Add("UniqueRecordId", typeof(int));
            //DisputeTransDataTable.Columns.Add("TranDate", typeof(DateTime));
            //DisputeTransDataTable.Columns.Add("TranAmount", typeof(string));
            //DisputeTransDataTable.Columns.Add("DisputedAmt", typeof(string));
            //DisputeTransDataTable.Columns.Add("DecidedAmount", typeof(string));
            //DisputeTransDataTable.Columns.Add("ActionDate", typeof(string));
            //DisputeTransDataTable.Columns.Add("ClosedDispute", typeof(bool));
            //DisputeTransDataTable.Columns.Add("Action Desc", typeof(string));

            //
            // DATA TABLE ROWS DEFINITION 
            //

        }
        // Find Disputes of this card
        private void button2_Click(object sender, EventArgs e)
        {
            if (RRDMInputValidationRoutines.IsAlfaNumeric(textBox3.Text))
            {
                //   No Problem 
            }
            else
            {
                MessageBox.Show("invalid characters in Card Number");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
// Link to creator 
        private void link_Creator_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int WFunctionNo = 3;
            Form20 NForm20; 
            NForm20 = new Form20(WOperator, WSignedId, WSignRecordNo, WCreator, WFunctionNo, 0);
            NForm20.ShowDialog();
        }
    }
}

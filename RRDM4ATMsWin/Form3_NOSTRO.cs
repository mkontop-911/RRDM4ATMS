using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form3_NOSTRO : Form
    {

        //Form5 NForm5;

        //Form3_DispOwnersITMX NForm3DispOwnersNAITMX;

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMNVDisputes Di = new RRDMNVDisputes();
        RRDMNVDisputesTrans Dt = new RRDMNVDisputesTrans();
        RRDMNVMT995AndMT996Pool Mt = new RRDMNVMT995AndMT996Pool();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMBanks Ba = new RRDMBanks();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        // NOTES 
        string Order;

        int WReconcCycleNo;
        string WCategoryId;
        //int WMaskRecordId;
        int WDispTranNo;

        //bool SingleCustomer; 

        string SelectionCriteria;

        //bool ITMXUser;

        //bool WithDate;

        int WMTSeqNo;

        string WParameter4;
        string WSearchP4;
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        int WRowIndex;

        //string WBankId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2150, 11, 21);

        int WDispSeqNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WMode;
        int WInDispNo;

        public Form3_NOSTRO(string InSignedId, int InSignRecordNo, string InOperator,
                            string InMode, int inDispNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WMode = InMode; // "VIEW" OR "UPDATE" 

            WInDispNo = inDispNo;

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelToday.Text = DateTime.Now.ToShortDateString();
            labelUserId.Text = WSignedId;

            comboBoxSettlementReason.Items.Add("Select Reason"); // 
            comboBoxSettlementReason.Items.Add("Satisfied with reply"); // 
            comboBoxSettlementReason.Items.Add("Our mistake"); // 
            comboBoxSettlementReason.Items.Add("Teller will take action"); // 

            comboBoxSettlementReason.Text = "Select Reason";

            RRDMReconcJobCycles Djc = new RRDMReconcJobCycles();
            string WJobCategory = "NOSTRO";
            Djc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Djc.RecordFound == true)
            {
                WReconcCycleNo = Djc.JobCycle;
            }

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (WMode == "UPDATE")
            {
                SelectionCriteria = " WHERE Operator = '" + WOperator + "'";
            }
            if (WMode == "VIEW")
            {
                SelectionCriteria = " WHERE SeqNo = " + WInDispNo;
            }


            Di.ReadNVDisputesFillTable(SelectionCriteria);

            ShowGrid1(); // Disputes Grid


        }
        //
        // ON ROW ENTER FOR DISPUTES
        //
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WDispSeqNo = (int)rowSelected.Cells[0].Value;

            Di.ReadNVDisputesBySeqNo(WOperator, WDispSeqNo); // Read Specific Dispute details 

            // Find Matching Category ... Needed for change owner
            Mc.ReadMatchingCategorybyExternalAccNo(WOperator, Di.VostroBank, Di.ExternalAccNo);
            WCategoryId = Mc.CategoryId;
            // 

            // Fill textBoxs
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


            if (Di.Active == true)
            {
                checkBox2.Checked = true;
                textBox6.Hide();

                label13.Show();
                label15.Show();
                textBox7.Show();
                textBox5.Show();

                labelAction.Show();
                panelAction.Show();
            }
            else
            {
                checkBox2.Checked = false;
                textBox6.Show();

                label13.Hide();
                label15.Hide();
                textBox7.Hide();
                textBox5.Hide();

                labelAction.Hide();
                panelAction.Hide();

                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT;
            }

            textBox2.Text = Di.TargetDateTm.ToString();
            // Time remaining 

            // DateTime Difference ;
            if (Di.Active == true)
            {
                TimeSpan Remain = Di.TargetDateTm - DateTime.Now;
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
            WParameter4 = "Notes For Dispute " + "DispNo: " + Di.SeqNo.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            //********************************************************
            // Show GRID Of Dispute transactions 
            //********************************************************

            Dt.ReadNVDisputesTransFillTable(WDispSeqNo);

            ShowGridDisputeTrans();

            //********************************************************
            // Show Grid for MT Messages
            //********************************************************
            SelectionCriteria = " Where DispNo = " + WDispSeqNo;

            Mt.ReadNVMT995PoolFillTable(WOperator, SelectionCriteria);

            ShowGridMTs();

        }
        // 
        // ON Trans 
        // 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WDispTranNo = (int)rowSelected.Cells[0].Value;


        }

        // ROW ENTER FOR MT MESSAGES

        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];
            WMTSeqNo = (int)rowSelected.Cells[0].Value;

            Mt.ReadNVMT995PoolBySeqNo(WOperator, WMTSeqNo);

            textBoxMTMessageType.Text = Mt.MessageType;

            if (Mt.MessageType == "995")
            {
                labelMT12.Text = "Question Line 1";
                labelTag77.Text = "Question Line 2";
            }
            if (Mt.MessageType == "996")
            {
                labelMT12.Text = "Answer Line 1";
                labelTag77.Text = "Answer Line 2";
            }

            textBoxMTReceiver.Text = Mt.Receiver;

            textBoxTag20.Text = Mt.TrxReferenceNumber;
            textBoxTag21.Text = Mt.RelatedReference;

            textBoxTag75_1.Text = Mt.QueryLine1;
            textBoxTag75_2.Text = Mt.QueryLine2;

            textBoxTag77_1.Text = Mt.Narrative;

            textBoxTag11.Text = Mt.OriginalMessageMT + " " + Mt.OriginalMessageDate;

            //Form3_Load(this, new EventArgs());
        }



        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Notes For Dispute " + "DispNo: " + Di.SeqNo.ToString();
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

            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int Mode = 4;
            string W_Application = ""; 
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator, WCategoryId, W_Application, Mode, "");
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog();
        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }


        //**********************************************************************
        // END NOTES 
        //********************************************************************** 
        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGrid1()
        {
            dataGridView1.DataSource = Di.TableDisputes.DefaultView;

            dataGridView1.Columns[0].Width = 70; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 100; // OpenDate
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Black;

            dataGridView1.Columns[2].Width = 100; // TargetDateTm
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 160; // Description
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].DefaultCellStyle.ForeColor = Color.Black;

            dataGridView1.Columns[4].Width = 100; // IsExternal
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 70; // VostroBank
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 70; // ExternalAccNo
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }
        //******************
        // SHOW GRID dataGridView2
        //******************
        private void ShowGridDisputeTrans()
        {
            dataGridView2.DataSource = Dt.TableDisputeTxnEntries.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].Visible = false;


            dataGridView2.Columns[1].Width = 85;  // Origin
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[2].Width = 105;  // Acc No 
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].Visible = true;


            dataGridView2.Columns[3].Width = 40;  // Code 
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 90; //  ValueDate
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[5].Width = 90; // EntryDate
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 40; // DR/CR
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[7].Width = 100; // Amt
            dataGridView2.Columns[7].DefaultCellStyle = style;
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[8].Width = 130; // OurRef
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView2.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[9].Width = 100; // TheirRef
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[10].Width = 95; // OtherDetails
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        //******************
        // SHOW GRID dataGridView3 - MTs 
        //******************
        private void ShowGridMTs()
        {
            dataGridView3.DataSource = Mt.TableNVMT995Pool.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            //RowSelected["SeqNo"] = SeqNo;
            //RowSelected["BankID"] = MessageType;
            //RowSelected["Receiver"] = Receiver;
            //RowSelected["TrxReferenceNumber"] = TrxReferenceNumber;
            //RowSelected["RelatedReference"] = RelatedReference;
            //RowSelected["QueryLine1"] = QueryLine1;
            //RowSelected["Narrative"] = Narrative;
            //RowSelected["OriginalMessageMT"] = OriginalMessageMT;

            dataGridView3.Columns[0].Width = 50; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.Columns[0].Visible = false;

            dataGridView3.Columns[1].Width = 50;  // MessageType
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView3.Columns[2].Width = 60;  // Receiver
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.Columns[2].Visible = false;

            dataGridView3.Columns[3].Width = 100;  // TrxReference
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[4].Width = 100; //  RelatedReference
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView3.Columns[5].Width = 110; // QueryLine1
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[6].Width = 150; //Narrative
            dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[7].Width = 100; // OriginalMessageMT
            dataGridView3.Columns[7].DefaultCellStyle = style;
            dataGridView3.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView3.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView3.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }

        // Settle Dispute 
        private void buttonSettleDispute_Click(object sender, EventArgs e)
        {
            if (Di.OwnerId != WSignedId & Di.Active == true)
            {
                MessageBox.Show("Error : You are not the owner of this dispute.");
                return;
            }
            if (comboBoxSettlementReason.Text == "Select Reason")
            {
                MessageBox.Show("Please Select Reason! ");
                return;
            }

            Di.ReadNVDisputesBySeqNo(WOperator, WDispSeqNo);

            Di.SettledDate = DateTime.Now;
            Di.SettledReason = comboBoxSettlementReason.Text;
            Di.Active = false;

            Di.UpdateNVDisputesRecord(WDispSeqNo);

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }
        // Reminder 
        private void buttonReminder_Click(object sender, EventArgs e)
        {
            // Code 36

            Mt.ReadNVMT995PoolBySeqNo(WOperator, WMTSeqNo);

            textBoxMTMessageType.Text = Mt.MessageType;

            if (Mt.MessageType == "995")
            {
                // Compile reminder and update 

                //textBoxTag75_1.Text = ":75:/" + FirstTwoOfReason + "/" + SixCharInternalValDate;
                Mt.QueryLine1 = ":75:/" + "36" + "/";

                Mt.Narrative = "We appear not to have received your reply to date ";
                int MtNumber = Mt.InsertRecordInNVMT995Pool();
            }

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            MessageBox.Show("A new MT995 was sent to remind received Bank.");

        }

        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Load reply 
        // THIS IS for testing of work flow
        private void buttonLoadReply_Click(object sender, EventArgs e)
        {
            MessageBox.Show("For testing purposes we will load a message MT996");

            Mt.ReadNVMT995PoolBySeqNo(WOperator, 35);

            Mt.DispNo = WDispSeqNo;

            int MtNumber = Mt.InsertRecordInNVMT995Pool();

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form3_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            MessageBox.Show("A new MT996 is loaded.");

        }
    }
}

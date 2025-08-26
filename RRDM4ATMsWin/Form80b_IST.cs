using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80b_IST : Form
    {
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode;
        // NOTES END 
        string WMatchingCateg;

        string WRRN;

        string WFilterFinal;

        string WSelectionCriteria;
        string WTableId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        DateTime WDateTimeA;
        DateTime WDateTimeB;
        string WAtmNo;

        string WCategoryId;

        //bool ViewCatAndCycle; 

        int WRMCycleNo;

        string WStringUniqueId;
        int WIntUniqueId;
        int WUniqueIdType;
        string WFunction;
        string WIncludeNotMatchedYet;
        int WReplCycle;

        public Form80b_IST(string InSignedId, int InSignRecordNo, string InOperator,
                                                 DateTime InDateTimeA,
                                                 DateTime InDateTimeB,
                                                 string InAtmNo, string InCategoryId, int InRMCycleNo,
                                                 string InStringUniqueId, int InIntUniqueId, int InUniqueIdType,
                                                 string InFunction, string InIncludeNotMatchedYet, int InReplCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WDateTimeA = InDateTimeA;
            WDateTimeB = InDateTimeB;
            WAtmNo = InAtmNo;

            WCategoryId = InCategoryId; // This is blank we get the category from type and input field 
            WRMCycleNo = InRMCycleNo;

            WStringUniqueId = InStringUniqueId;
            WIntUniqueId = InIntUniqueId;

            WUniqueIdType = InUniqueIdType;

            // 1 Encrypted card

            //2 Mask CARD

            //3 Account

            //4 Trace No

            //5 RR Number

            //6 ATMNo

            //7 Category Processed

            //8 Category Non Processed

            //9 Force Matched 

            WTableId = "Switch_IST_Txns";

            WFunction = InFunction; // "Reconc", "View", "Investigation"       

            WIncludeNotMatchedYet = InIncludeNotMatchedYet;
            // if "1" = Include not matched yet
            // if ""  = Do not include not matched yet
            //
            WReplCycle = InReplCycle; // This is for WIntUniqueId = 8 

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            WSelectionCriteria = "";

            buttonTakeAction.Hide();

            if (WUniqueIdType == 1) // Encryptted CARD
            {
                labelStep1.Text = "Transactions for Encrypted Card :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE Card_Encrypted ='" + InStringUniqueId + "'";
            }

            if (WUniqueIdType == 2) // CARD
            {
                labelStep1.Text = "Transactions for Card :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE CardNumber ='" + InStringUniqueId + "'";
            }

            if (WUniqueIdType == 3) // Account
            {
                labelStep1.Text = "Transactions for AccNo :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE AccNo ='" + InStringUniqueId + "'";
                buttonTakeAction.Show();

            }

            if (WUniqueIdType == 4)
            {
                labelStep1.Text = "Transactions for TraceNo :.." + InIntUniqueId.ToString();
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE TraceNo =" + InIntUniqueId + " AND TerminalId ='" + InAtmNo + "'";
            }

            if (WUniqueIdType == 5)
            {
                labelStep1.Text = "Transactions for RRNumber :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE RRNumber ='" + InStringUniqueId + "' AND MatchingCateg ='" + InAtmNo + "'";
            }

            if (WUniqueIdType == 6)
            {
                labelStep1.Text = "Transactions for TerminalId :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE TerminalId ='" + InStringUniqueId + "' ";
            }

            if (WUniqueIdType == 7) // Category Process
            {
                labelStep1.Text = "Processed Transactions for Matching Category :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE MatchingCateg ='" + InStringUniqueId + "' AND Processed = 1 ";
            }

            if (WUniqueIdType == 8) // Category not Process
            {
                labelStep1.Text = "NON Processed Transactions for Matching Category :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE MatchingCateg ='" + InStringUniqueId + "' AND Processed = 0 AND ResponseCode = '0' AND TransAmt <> 0 ";
            }
            if (WUniqueIdType == 9) // Force Matched 
            {
                labelStep1.Text = "Force Matched TXNS Category :.." + InStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE MatchingCateg ='" + InStringUniqueId + "' AND Processed = 1 and Actiontype = '04' and Left(Comment,4) = 'POS_'  ";
            }

            if (WUniqueIdType == 8) // We show Aging for not processed
            {
                buttonAging2.Show();
                panelAging.Show();
            }
            else
            {
                buttonAging2.Hide();
                panelAging.Hide();
            }
        }
        // Load 
        private void Form80b_Load(object sender, EventArgs e)
        {

            // FILL TABLE AND SHOW // Only this Categ and Cycle both

            Mgt.ReadTrans_Table_For_SpecificTable(WTableId, WSelectionCriteria, WDateTimeA, WDateTimeB, 2);


            dataGridView1.DataSource = Mgt.DataTableSelectedFields.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No transactions for this selection");

                return;
            }
            else
            {
                textBoxLines.Text = dataGridView1.Rows.Count.ToString("#,###");
            }


            //+" SELECT [SeqNo] ,[TransType] ,[TransDescr] ,[CardNumber],[AccNo]  "
            //  + ",[TransCurr],[TransAmt] ,[TransDate],[TraceNo]  ,[RRNumber] ,[TerminalId]"
            //
            dataGridView1.Columns[0].Width = 40; // [SeqNo]
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 40; // [TransType]
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderText = "Type";

            dataGridView1.Columns[2].Width = 110; //TransDescr
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 100; //  CardNumber
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 100; //  CardEncrypted
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 100; //  AccNo
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // TransCurr
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].HeaderText = "Ccy";

            dataGridView1.Columns[7].Width = 100; //AMT 
            dataGridView1.Columns[7].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[8].Width = 110; // TransDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 60; // TraceNo
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 100; // RRNumber
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 70; // TerminalId
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            int tempUniqueNo = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                //bool WSelect = (bool)row.Cells[1].Value;

                tempUniqueNo = (int)row.Cells[0].Value;

                Dt.ReadDisputeTranByUniqueRecordId(tempUniqueNo);
                if (Dt.RecordFound == true)
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }

            }

            //if (dataGridView1.Rows.Count == 0)
            //{
            //    //MessageBox.Show("No transactions to be posted");
            //    Form2 MessageForm = new Form2("No transactions to be posted");
            //    MessageForm.ShowDialog();

            //    this.Dispose();
            //    return;
            //}

            // FirstCycle = false;
        }

        // On ROW ENTER 
        int WWUniqueRecordId;
        bool IsReversal;
        bool JournalFound;
        string Message;
        string DiscrepancyMessage;

        int WSeqNo;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)rowSelected.Cells[0].Value;
            WFilterFinal = " Where  SeqNo = " + WSeqNo;
            WTableId = "Switch_IST_Txns";
            Mgt.ReadTransSpecificFromSpecificTable_By_SeqNo(WTableId, WSeqNo, 2);

            panelAging.Hide();

            if (Mgt.TXNSRC == "1")
            {

                //buttonPOS.Hide();

                IsReversal = false;
                Message = "";

                RRDMJournalReadTxns_Text_Class Jt = new RRDMJournalReadTxns_Text_Class();

                Jt.ReadJournalTextByTrace(WOperator, Mgt.TerminalId, Mgt.TraceNo, Mgt.TransDate.Date);

                if (Jt.RecordFound == true)
                {
                    JournalFound = true;

                    Message = "";
                }
                else
                {
                    JournalFound = false;
                }

                // If First position is Zero check if TXN has been reversed

                // Check if reversal at Target
                string WWMask = Mgt.Mask;
                string TableId = "";

            }
            else
            {
                // JCC 

                // buttonPOS.Show();
            }

            IsReversal = false;

            DiscrepancyMessage = "";
            // 
            if (Mgt.Comment == "Reversals")
            {
                DiscrepancyMessage = "There is Reversal in the matching records. " + Environment.NewLine
                             + "If in position there is letter R then this was Reversed. " + Environment.NewLine
                             + "See them by pressing Source Records Button ";

                IsReversal = true;
            }

            // NOTES START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mgt.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // NOTES END

            //WWUniqueRecordId = Mgt.SeqNo;
            WMatchingCateg = Mgt.MatchingCateg;

            labelAging.Text = "AGING ANALYSIS FOR.." + WMatchingCateg;

            textBox10.Text = Mgt.MatchingCateg;
            textBox11.Text = Mgt.TerminalId;

            textBoxRmCycle.Text = Mgt.ProcessedAtRMCycle.ToString();
            textBoxDescr.Text = Mgt.TransDescr;
            textBoxCardNo.Text = Mgt.CardNumber;
            textBoxAccNo.Text = Mgt.AccNo;
            textBoxCurr.Text = Mgt.TransCurr;
            textBoxAmnt.Text = Mgt.TransAmt.ToString("#,##0.00");
            textBoxDtTm.Text = Mgt.TransDate.ToString();
            textBoxTraceNo.Text = Mgt.TraceNo.ToString();
            textBoxRRN.Text = Mgt.RRNumber.ToString();

            //        ,[Processed]
            //,[ProcessedAtRMCycle]
            //,[Mask]

            textBoxMask.Text = Mgt.Mask;

            if (Mgt.Mask == "011" & Mgt.ActionType == "04")
            {
                LabelForceMatched.Show();
            }
            else
            {
                LabelForceMatched.Hide();
            }

            if (Mgt.Processed == true)
            {
                textBoxProcessed.Text = "YES";

                buttonTakeAction.Hide();
                radioButtonToCust.Hide();
                radioButtonToRevenue.Hide();

            }
            else
            {
                textBoxProcessed.Text = "NO";
                if (WUniqueIdType == 3)
                {
                    buttonTakeAction.Show();
                    radioButtonToCust.Show();
                    radioButtonToRevenue.Show();
                }
                else
                {
                    buttonTakeAction.Hide();
                    radioButtonToCust.Hide();
                    radioButtonToRevenue.Hide();
                }

            }

            if (Mgt.Comment == "")
            {
                textBoxComment.Text = "No Comment";
            }
            else
            {
                textBoxComment.Text = Mgt.Comment;
            }

            textBoxAcceptorId.Text = Mgt.ACCEPTOR_ID;
            textBoxAcceptorNm.Text = Mgt.ACCEPTORNAME;

            // textBoxCAP_DATE.Text = Mgt.CAP_DATE.ToShortDateString();

            // FIX SEPT 2019
            Mc.ReadMatchingCategorybyActiveCategId(WOperator, Mgt.MatchingCateg);

            if (Mc.TWIN == true)
            {
                //  buttonTextFromJournal.Show();
                //  button4.Show();
                //labelTextFromJournal.Show(); 
                //button5.Show();
                //buttonPOS.Hide();
                //  linkLabelForRelationsToPresenterError.Show(); 
            }
            else
            {
                //  linkLabelForRelationsToPresenterError.Hide();
            }

            //Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, Mgt.MatchingCateg, Mgt.ProcessedAtRMCycle);
            //if (Rcs.RecordFound == true)
            //{
            //    //if (Rcs.EndReconcDtTm == NullPastDate)
            //    //{
            //    //    buttonReconcPlay.Enabled = false;
            //    //    buttonReconcPlay.BackColor = Color.Silver;
            //    //}
            //    //else
            //    //{
            //    //    buttonReconcPlay.Enabled = true;
            //    //    buttonReconcPlay.BackColor = Color.White;
            //    //    buttonTextFromJournal.Show();
            //    //}
            //}
            //else
            //{
            //    // Not Found or not Matching Yet 
            //    //buttonReconcPlay.Enabled = false;
            //    //buttonReconcPlay.BackColor = Color.Silver;
            //}

            //textBoxMask.Text = Mgt.Mask;


            WRRN = Mgt.RRNumber;

            // Check if dispute already registered for this transaction 

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            // Check if already exist
            Dt.ReadDisputeTranByUniqueRecordId(Mgt.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                textBox9.Text = Dt.DisputeNumber.ToString();
                linkLabelDispute.Show();
                //WFoundDisp = Dt.DisputeNumber;
                if (WFunction == "Investigation" & WWUniqueRecordId == 0)
                    MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                labelDispute.Show();
                textBox9.Show();
                buttonRegisterDispute.Hide();
            }
            else
            {
                labelDispute.Hide();
                textBox9.Hide();
                linkLabelDispute.Hide();
                if (WFunction == "Investigation")
                    buttonRegisterDispute.Show();
            }

            buttonRegisterDispute.Hide();

            //textBoxInputField.Text = Mpa.CardNumber;
        }


        // Aging 
        DateTime DateOfAge;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            DateOfAge = (DateTime)rowSelected.Cells[0].Value;

            textBoxDateAging.Text = DateOfAge.ToString();
        }

        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Mgt.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mgt.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }
        string P1;
        //private void buttonPrint_Click(object sender, EventArgs e)
        //{
        //    // Print 
        //    //comboBoxFilter.Items.Add("Matched");

        //    //comboBoxFilter.Items.Add("UnMatched");

        //    ////comboBoxFilter.Items.Add("UnMatchedAllCycles");

        //    //comboBoxFilter.Items.Add("Both");
        //    Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

        //    //if (checkBoxUnique.Checked == false)
        //    //{
        //    //   // P1 = comboBoxFilter.Text + " Transactions";
        //    //}
        //    //if (checkBoxUnique.Checked == true)
        //    //{
        //    //    P1 = "Transactions for unique Selection:" + textBoxUniqueID.Text;

        //    //    if (radioButtonAtm.Checked == true) P1 = "Transactions for Atm No :" + textBoxInputField.Text;
        //    //}

        //    string P2 = "Second Par";
        //    string P3 = "Third Par";
        //    string P4 = Us.BankId;
        //    string P5 = WSignedId;

        //    Form56R54ATMS ReportATMS54 = new Form56R54ATMS(P1, P2, P3, P4, P5);
        //    ReportATMS54.Show();
        //}
        bool FromShow;

        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        // Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We will decide if we can open a dispute from here");
            return;
            Form5 NForm5;
            int From = 2; // Coming from Pre-Investigattion ATMs 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mgt.CardNumber, WWUniqueRecordId, 0, 0, "", From, "ATM");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog();
        }

        private void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }


        // Link to disputes 
        Form3 NForm3;
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("We will decide if we need this link");
            return;
            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBox9.Text, NullPastDate, NullPastDate, 13);
            NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();
        }

        void NForm3_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        //
        // Decrypt
        private void button1_Click(object sender, EventArgs e)
        {
            string WCardNo = En.DecryptField(textBoxCardNo.Text);

            MessageBox.Show("The Decrypted Card Is: " + Environment.NewLine
                            + WCardNo);
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }


        // Show IST presenter errors
        private void buttonShow_IST_Presenter_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Functionality was moved to controller");
            //string FileId = "Switch_IST_Txns"; 
            //Form78d_FileRecords_IST_PRESENTER NForm78d_FileRecords_IST_PRESENTER;
            //NForm78d_FileRecords_IST_PRESENTER = new Form78d_FileRecords_IST_PRESENTER(WOperator, WSignedId, FileId, WDateTimeA, WDateTimeB, 0);
            //NForm78d_FileRecords_IST_PRESENTER.ShowDialog();

        }

        private void labelDemoHelp_Click(object sender, EventArgs e)
        {

        }
        // Aging analysis
        private void buttonAging_Click(object sender, EventArgs e)
        {

        }
        // Show Aging Details
        private void buttonShowAgingDetails_Click(object sender, EventArgs e)
        {

            Form78d_FileRecords_For_Date NForm78d_FileRecords_For_Date;
            // textBoxFileId.Text
            int WMode = 11; //

            bool XCategoryOnly = true;
            NForm78d_FileRecords_For_Date = new Form78d_FileRecords_For_Date(WOperator, WSignedId, WTableId, WMatchingCateg, DateOfAge, WMode);
            NForm78d_FileRecords_For_Date.Show();
        }


        private void buttonAging2_Click(object sender, EventArgs e)
        {
            panelAging.Show();
            //WDateTimeA = InDateTimeA;
            //WDateTimeB = InDateTimeB;
            string WTableId = "Switch_IST_Txns";
            Mgt.ReadTrans_Table_For_Aging(WTableId, WMatchingCateg, WDateTimeA, WDateTimeB);


            // FILL TABLE AND SHOW // 

            dataGridView2.DataSource = Mgt.TableAgingAnalysis.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("No transactions for this selection");

                return;
            }

            dataGridView2.Columns[0].Width = 70; //Date
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 60; //TXNs
            dataGridView2.Columns[1].DefaultCellStyle.Format = "#,###";
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView2.Columns[2].Width = 120; //AMT 
            dataGridView2.Columns[2].DefaultCellStyle.Format = "#,##0.00";
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        // EXPORT TO EXCEL 
        private void buttonExportToExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Meeza" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Mgt.DataTableSelectedFields, WorkingDir, ExcelPath);
        }
        // Take action Force Matched 
        readonly string connectionStringATMs = ConfigurationManager.ConnectionStrings
                   ["ATMSConnectionString"].ConnectionString;
        string WComment = ""; 
        private void buttonTakeAction_Click(object sender, EventArgs e)
        {
            if (WMatchingCateg == "BDC272" & WUniqueIdType == 3 & Mgt.Processed == false) // and account chosen 
            {
                if (radioButtonToCust.Checked == false & radioButtonToRevenue.Checked == false)
                {
                    MessageBox.Show("Please make selection of what action reason");
                    return;
                }
                if (radioButtonToCust.Checked == true)
                {
                    WComment = "POS_Funds Returned To Customer";
                }
                if (radioButtonToRevenue.Checked == true)
                {
                    WComment = "POS_Funds Returned To Revenue";
                }
                // CALL RRDM CLASS TO TRUNCATE THE TABLES
                if (MessageBox.Show("DO YOU WANT TO TAKE ACTION   " + Environment.NewLine
                                    + Environment.NewLine
                                   + WComment + "?????"+ Environment.NewLine
                                   , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                    // YES Proceed
                }
                else
                {
                    // Stop 
                    return;
                }

                //MessageBox.Show("For this record action will be taken as follows:" + Environment.NewLine
                //    + "Action will be created with Id=04 and reason as defined" + Environment.NewLine
                //     + "Master record will be created with Mask 011" + Environment.NewLine
                //      + "IST and Corebanking Record will be shown as processed" + Environment.NewLine
                //    );

                 
            }
            else
            {
                MessageBox.Show("Allowed Operation for BDC272");
                return;
            }

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            RRDMMatchingCategoriesVsSourcesFiles Mcsf = new RRDMMatchingCategoriesVsSourcesFiles();

            WTableId = "Switch_IST_Txns";
            Mgt.ReadTransSpecificFromSpecificTable_By_SeqNo(WTableId, WSeqNo, 2);
            int SeqNoInTwo = WSeqNo;
            WTableId = "Flexcube";
            WSelectionCriteria = "  WHERE MatchingCateg='" + WMatchingCateg + "'  AND RRNumber='" + Mgt.RRNumber + "'";
            Mgt.ReadTransSpecificFromBothTables_By_SelectionCriteria(WSelectionCriteria, WTableId);
            int SeqNoInThree = Mgt.SeqNo;
            //
            // Read again to get the correct record
            //
            WTableId = "Switch_IST_Txns";
            Mgt.ReadTransSpecificFromSpecificTable_By_SeqNo(WTableId, WSeqNo, 2);

            if (Mgt.RecordFound)
            {
                //WReplCycleNo = Mpa.ReadMatchingTxnsMasterPoolFirstRecordLessThanGivenDateTime(Mgt_BDC.TerminalId, Mgt_BDC.TransDate);
                int WReplCycleNo = 0;
                // SeqNoTwo = Mgt_BDC.SeqNo;
                Mpa.OriginalRecordId = Mgt.OriginalRecordId;
                Mpa.TerminalType = Mgt.TerminalType;
                //Mpa.OriginFileName = Mgt.OriginFileName;

                //Mpa.LoadedAtRMCycle = Mgt.LoadedAtRMCycle;

                Mpa.TransTypeAtOrigin = Mgt.TransTypeAtOrigin;

                Mpa.TerminalId = Mgt.TerminalId;
                Mpa.TransType = Mgt.TransType;
                Mpa.TransDescr = Mgt.TransDescr;

                Mpa.CardNumber = Mgt.CardNumber;

                Mpa.AccNumber = Mgt.AccNo;

                Mpa.TransCurr = Mgt.TransCurr;
                Mpa.TransAmount = Mgt.TransAmt;
                // +",[AmtFileBToFileC] " // AMOUNT_EQUIV
                //Mpa.SpareField = Mgt.AmtFileBToFileC.ToString(); // // AMOUNT_EQUIV

                Mpa.TransDate = Mgt.TransDate;
                Mpa.RRNumber = Mgt.RRNumber;
                Mpa.AUTHNUM = Mgt.AUTHNUM;

                //ReadWorkingFileBySelectionCriteria(TableId, WSelectionCriteria, LastTransDate);

                Mpa.OriginFileName = "IST";

                Mpa.LoadedAtRMCycle = Mgt.LoadedAtRMCycle;

                Mpa.MatchingAtRMCycle = WRMCycleNo;

                Mpa.MatchingCateg = WMatchingCateg;


                Mpa.RMCateg = WMatchingCateg;

                Mpa.UniqueRecordId = GetNextValue(connectionStringATMs);

                Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCateg);
                Mpa.Origin = Mc.Origin;

                Mpa.TargetSystem = 6;

                Mpa.Product = "";
                Mpa.CostCentre = "";

                Mpa.DepCount = 0;

                // TRACE
                Mpa.TraceNoWithNoEndZero = Mgt.TraceNo;
                Mpa.AtmTraceNo = Mgt.TraceNo;
                Mpa.MasterTraceNo = Mgt.TraceNo;

                Mpa.IsOwnCard = true;


                Mpa.ResponseCode = "0";
                Mpa.SpareField = "";
                Mpa.Comments = WComment;

                Mpa.IsMatchingDone = true;
                Mpa.Matched = false;
                Mpa.MatchMask = "011";
                Mpa.SystemMatchingDtTm = DateTime.Now;


                Mpa.UnMatchedType = "Not Matched_POS";

                Mpa.UnMatchedType = "";
                Mpa.MetaExceptionId = 0;
                Mpa.MetaExceptionNo = 0;
                Mpa.FastTrack = false;
                Mpa.ActionByUser = false;
                Mpa.UserId = "";
                Mpa.Authoriser = "";
                Mpa.AuthoriserDtTm = DateTime.Now;
                Mpa.ActionType = "";
                Mpa.NotInJournal = true;
                Mpa.WaitingForUpdating = false;
                Mpa.SettledRecord = false;
                Mpa.Operator = WOperator;

                string SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
                Mpa.FileId01 = Mcsf.SourceFileNameA;

                Mpa.FileId02 = Mcsf.SourceFileNameB;
                Mpa.SeqNo02 = SeqNoInTwo;
                Mpa.FileId03 = Mcsf.SourceFileNameC;

                Mpa.SeqNo03 = SeqNoInThree;


                Mpa.FileId04 = Mcsf.SourceFileNameD;
                Mpa.SeqNo04 = 0;
                Mpa.FileId05 = "";
                Mpa.SeqNo05 = 0;
                Mpa.FileId06 = "";
                Mpa.SeqNo06 = 0;

                Mpa.Card_Encrypted = Mgt.Card_Encrypted;
                Mpa.TXNSRC = Mgt.TXNSRC;
                Mpa.TXNDEST = Mgt.TXNDEST;

                Mpa.ACCEPTOR_ID = Mgt.ACCEPTOR_ID;
                Mpa.ACCEPTORNAME = Mgt.ACCEPTORNAME;
                Mpa.CAP_DATE = Mgt.CAP_DATE;

                Mpa.SET_DATE = Mgt.SET_DATE;

                // Find the right (proper cycle number)

                Mpa.ReplCycleNo = 0; // THIS CYCLE NO WAS FOUND FROM PREVIOUS TRACE ... See Code Above

                int NewSeqNo = Mpa.InsertTransMasterPoolATMs(Mpa.Operator);

                // Find Out the Replenishment Cycle No by using Last Trace Number


                // UPDATE INSERTED what was not updated by insert 
                SelectionCriteria = " WHERE  SeqNo =" + NewSeqNo;
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                Mpa.SeqNo01 = NewSeqNo;
                Mpa.IsMatchingDone = true;
                Mpa.Matched = false;
                Mpa.MatchedType = "Force Matched-POS";
                Mpa.MatchMask = "011";

                Mpa.SystemMatchingDtTm = DateTime.Now;

                Mpa.TraceNoWithNoEndZero = Mgt.TraceNo;

                Mpa.UserId = WSignedId;
                Mpa.ActionByUser = true;
                Mpa.Authoriser = "No Auther";

                Mpa.NotInJournal = true;

                Mpa.SettledRecord = true;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId, 1);

                // UPDATE RECORDS Involved 


                if (SeqNoInTwo > 0 & Mpa.Matched == false)
                {
                    // Update SourceTable_C IST 
                    string SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";
                    Mgt.UpdateRecordForMaskBySeqNumber_POS(SourceTable_B, SeqNoInTwo, Mpa.MatchMask, WRMCycleNo, Mpa.UniqueRecordId, WComment);

                }
                if (SeqNoInThree > 0 & Mpa.Matched == false)
                {
                    // Update record as not matched. 
                    // Update SourceTable_C
                    string SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo].[Flexcube]";
                    Mgt.UpdateRecordForMaskBySeqNumber_POS(SourceTable_C, SeqNoInThree, Mpa.MatchMask, WRMCycleNo, Mpa.UniqueRecordId, WComment);
                }
            }
            string WActionId = "04";
            string WMaker_ReasonOfAction = "";

            if (WActionId == "04")
            {
                // This is force Matching and other manual 
                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WUniqueRecordIdOrigin = "Master_Pool";
                if (radioButtonToCust.Checked == true)
                {
                    WMaker_ReasonOfAction = "POS_Funds Returned To Customer";
                }
                if (radioButtonToRevenue.Checked == true)
                {
                    WMaker_ReasonOfAction = "POS_Funds Returned To Revenue";
                }

                string WOriginWorkFlow = "Reconciliation";


                WOriginWorkFlow = "Reconciliation";


                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                    WActionId, WUniqueRecordIdOrigin,
                                                    Mpa.UniqueRecordId, Mpa.TransCurr, Mpa.TransAmount,
                                                    Mpa.TerminalId, Mpa.ReplCycleNo, WMaker_ReasonOfAction, WOriginWorkFlow);


                Aoc.UpdateOccurancesForAuthoriser("Master_Pool", Mpa.UniqueRecordId, "System", 9999, WSignedId);

                string SelectionCriteria = " WHERE  UniqueRecordId =" + Mpa.UniqueRecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                Mpa.ActionByUser = true;
                Mpa.UserId = WSignedId;

                Mpa.MatchedType = WMaker_ReasonOfAction;

                Mpa.ActionType = WActionId;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(WOperator, Mpa.UniqueRecordId, 2);

                MessageBox.Show("Record with UniqueRecordId : " + Mpa.UniqueRecordId.ToString() + Environment.NewLine
                    + "Has been dealt by Maker");

                int WRowIndex = dataGridView1.SelectedRows[0].Index;
                //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
                labelStep1.Text = "Transactions for AccNo :.." + WStringUniqueId;
                labelStep1.Text = labelStep1.Text + " From " + WDateTimeA.ToShortDateString() + " To " + WDateTimeB.ToShortDateString();
                WSelectionCriteria = " WHERE AccNo ='" + WStringUniqueId + "'";
                buttonTakeAction.Show();

                Form80b_Load(this, new EventArgs());

                dataGridView1.Rows[WRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                // dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;


            }
        }

        // Get Next Unique Id 
        static int GetNextValue(string InConnectionString)
        {
            int iResult = 0;

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[usp_GetNextUniqueId]";

            using (SqlConnection conn = new SqlConnection(InConnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        SqlParameter iNextValue = new SqlParameter("@iNextValue", SqlDbType.Int);
                        iNextValue.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(iNextValue);
                        cmd.ExecuteNonQuery();
                        string sResult = cmd.Parameters["@iNextValue"].Value.ToString();
                        int.TryParse(sResult, out iResult);

                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    // CatchDetails(ex, "615");
                }
            return iResult;
        }


    }
}


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form503_Insert_Manual_To_Mpa : Form
    {

        //
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

       // RRDMTempAtmsLocation Tl = new RRDMTempAtmsLocation();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // NOTES 
        string Order;
        string WParameter4;
        string WSearchP4;


        bool InternalChange;

        int WSeqNo;

        int WRowIndex;

        string WPrefix;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WReconcCycleNo;
        int WMode;
        bool WT24Version; 

        public Form503_Insert_Manual_To_Mpa(string InSignedId, int SignRecordNo, string InOperator,
                                              int InReconcCycleNo, int InMode,bool T24Version)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WReconcCycleNo = InReconcCycleNo;
            WMode = InMode;

            WT24Version = T24Version; 



            InitializeComponent();
            //dateTimePicker2.Format = DateTimePickerFormat.Custom;
            //dateTimePicker2.CustomFormat = "MM/dd/yyyy h:mm:ss tt";
            //timeOfDayPicker.Format = DateTimePickerFormat.Time;
            //timeOfDayPicker.ShowUpDown = true;

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = WSignedId;

            labelRmCycleId.Text = WReconcCycleNo.ToString();

            // Mpa.InsertTransMasterPoolATMs_2_Insert_Manually(WOperator);

            comboBoxType.Items.Add("Deposit"); // Deposit
            comboBoxType.Items.Add("Withdrawl"); // Withdrawl 

            // comboBoxCcy
            Gp.ParamId = "201"; // Currencies Second Cassettes 
            comboBoxCcy.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxCcy.DisplayMember = "DisplayValue";
            comboBoxCcy.Text = "EGP";

            // Districts
            //Gp.ParamId = "226";
            //comboBoxDistrict.DataSource = Gp.GetParamOccurancesNm(WOperator);
            //comboBoxDistrict.DisplayMember = "DisplayValue";



        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // TXNs table 

            Mpa.ReadTableManualTxnsTxns(WOperator);

            dataGridView1.DataSource = Mpa.ManualTXNSTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                buttonAdd.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                panel3.Hide(); 
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 60; // Cycle
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 90; // Card
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 90; // Account
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; // TransDesc
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[3].Visible = false; 

            dataGridView1.Columns[5].Width = 100; // DateTm
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 40; // Ccy
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 70; // Amount
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView1.Columns[11].Width = 80; // Amount
            //dataGridView1.Columns[11].DefaultCellStyle = style;
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[11].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridView1.Columns[8].Width = 70; // TraceNo
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[9].Width = 90; // RRN
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // Dispute
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 40; // Settled 
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            buttonAdd.Hide();
            buttonUpdate.Show();
            buttonDelete.Show();

        }
        // On Row Enter

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (checkBoxMakeNewEntry.Checked == true)
            {
                InternalChange = true;
                checkBoxMakeNewEntry.Checked = false;
            }
            else
            {
                InternalChange = false;
            }


            WSeqNo = (int)rowSelected.Cells[0].Value;
            string WSelectionCriteria = " WHERE SEQNO=" + WSeqNo;
            Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 1);

            //if (comboBoxType.Text == "Deposit")
            //{
            //    Mpa.TransType = 23;

            if (Mpa.TransType == 23)
            {
                comboBoxType.Text = "Deposit";
            }

            if (Mpa.TransType == 11)
            {
                comboBoxType.Text = "Withdrawl";
            }


            textBoxTermId.Text = Mpa.TerminalId;

            textBoxMaskPan.Text = Mpa.CardNumber;

            textBoxCateg.Text = Mpa.MatchingCateg;

            textBoxAccNo.Text = Mpa.AccNumber;

            textBoxAmnt.Text = Mpa.TransAmount.ToString("#,##0.00");

            comboBoxCcy.Text = Mpa.TransCurr; 

            textBoxTrace.Text = Mpa.TraceNoWithNoEndZero.ToString();

            textBoxRefNumber.Text = Mpa.RRNumber;

            dateTimePicker2.Value = Mpa.TransDate;
            panel3.Show();
            Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate);
            if(Ta.RecordFound ==true)
            {
                Mpa.ReplCycleNo = Ta.SesNo; 
            }

            textBoxReplCycle.Text = Mpa.ReplCycleNo.ToString();

            if (Mpa.ReplCycleNo > 0)
            {

                //WHERE Atmno = '00000550' AND ReplCycle = 2220 And Is_GL_Action = 1
                WSelectionCriteria = "WHERE AtmNo ='" + Mpa.TerminalId
                    + "' AND ReplCycle =" + Mpa.ReplCycleNo
                    + " AND (Stage = '03') ";

                Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

                textBox2.Text = Aoc.Current_ExcessBalance.ToString("#,##0.00");
                textBox3.Text = Aoc.Current_ShortageBalance.ToString("#,##0.00");

                //textBoxDispShort.Text = Aoc.Current_DisputeShortage.ToString("#,##0.00");
            }
            else
            {
                textBox2.Text = "0.00";
                textBox3.Text = "0.00";
            }


            // Check if already exist


            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
            // Check if already exist
            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            if (Dt.RecordFound == true & Dt.ClosedDispute == true & Mpa.ActionType == "95")
            {
               // There is a closed Dispute and money action was taken. 
                buttonCreateTXNSolo.Enabled = false;
            }
            else
            {
               
                buttonCreateTXNSolo.Enabled = true;
            }

            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
                linkLabelDispute.Show();
                //WFoundDisp = Dt.DisputeNumber;
                labelDispute.Show();
                textBoxDisputeId.Show();
                buttonRegisterDispute.Hide();

                buttonDeleteOpenDispute.Show(); 
            }
            else
            {
                textBoxDisputeId.Text = "";
                labelDispute.Hide();
                textBoxDisputeId.Hide();
                linkLabelDispute.Hide();
                buttonRegisterDispute.Show();
                buttonDeleteOpenDispute.Hide();

            }


            buttonUpdate.Show();
            buttonAdd.Hide();
            ////textBoxTermId.ReadOnly = false;
            //buttonUpdate.Hide();
            buttonDelete.Show();

            // dispute data
            panel3.Show();

            // Show grid 
            panel2.Show();
            label1.Show();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
           

            string WRMCateg;

            Mpa.TerminalId = textBoxTermId.Text.Trim();

            Ac.ReadAtm(Mpa.TerminalId);

            if (Ac.RecordFound == true)
            {
                WRMCateg = "RECATMS-" + Ac.AtmsReconcGroup;
            }
            else
            {
                MessageBox.Show("Not found ATM");
                return;
            }

            Mpa.CardNumber = textBoxMaskPan.Text;

            Mpa.MatchingCateg = textBoxCateg.Text.Trim();
            if (Mpa.MatchingCateg == "BDC201"
                || Mpa.MatchingCateg == "BDC202"
                 || Mpa.MatchingCateg == "BDC203"
                  || Mpa.MatchingCateg == "BDC204"
                   || Mpa.MatchingCateg == "BDC205"
                    || Mpa.MatchingCateg == "BDC206"
                    || Mpa.MatchingCateg == "BDC207"
                    || Mpa.MatchingCateg == "BDC208"
                    || Mpa.MatchingCateg == "BDC209"
                )
            {
                // It is OK
            }
            else
            {
                if (WT24Version == true & Mpa.MatchingCateg == "BDC101")
                {
                    // It is OK
                }
                else
                {
                    MessageBox.Show("Please enter a valid Category.(related to ATMS origin)");
                    return;
                }
                
            }

            //Mpa.AccNumber = textBoxAccNo.Text.Trim();

            //int length = textBoxAccNo.Text.Trim().Length; 

            if (Mpa.MatchingCateg == "BDC201" & textBoxAccNo.Text.Trim().Length != 14)
            {
                MessageBox.Show("Please enter a 14 character Account Number eg 00343020094422 ");
                return;
            }
            if (Mpa.MatchingCateg == "BDC101" & textBoxAccNo.Text.Trim().Length != 16)
            {
                MessageBox.Show("Please enter a 16 character Account Number eg 7700343020094422 ");
                return;
            }

            if (textBoxAccNo.Text.Trim().Length > 0)
            {
                if (Mpa.MatchingCateg == "BDC201" || Mpa.MatchingCateg == "BDC101")
                {
                    // For these allowed but not for REST
                }
                else
                {
                    MessageBox.Show("No allowed to insert Accno for this matching category ");
                    return;
                }
            }
           

            Mpa.AccNumber = textBoxAccNo.Text.Trim();

            if (decimal.TryParse(textBoxAmnt.Text, out decimal TransAmount))
            {
            }
            else
            {
                MessageBox.Show("Please enter amount in EGP ");
                return;
            }
            Mpa.TransAmount = TransAmount;
            if (comboBoxCcy.Text == "EGP")
            Mpa.TransCurr = "818";
            else
            {
                Mpa.TransCurr = comboBoxCcy.Text;
            }
            // Mpa.TraceNoWithNoEndZero = int.TryParse(textBox6.Text);


            if (int.TryParse(textBoxTrace.Text, out Mpa.TraceNoWithNoEndZero))
            {
            }
            else
            {
                if (textBoxTrace.Text != "")
                {
                    MessageBox.Show("Insert Correct Trace");
                    return;
                }
                else
                {
                    Mpa.TraceNoWithNoEndZero = 0;
                }

            }

            Mpa.RRNumber = textBoxRefNumber.Text.Trim();

            Mpa.TransDate = dateTimePicker2.Value;

            // OTHER FIELDS

            Mpa.OriginFileName = "ATMs Journals";
            Mpa.OriginalRecordId = 999999; // six 99999
            

            Mpa.RMCateg = WRMCateg; // 
            Mpa.LoadedAtRMCycle = WReconcCycleNo;
            Mpa.MatchingAtRMCycle = WReconcCycleNo;
            //Mpa.UniqueRecordId = 0;
            Mpa.Origin = "Our Atms";
            Mpa.TerminalType = "10";

            if (comboBoxType.Text == "Deposit")
            {
                Mpa.TransType = 23;
                Mpa.DepCount = Mpa.TransAmount;
                Mpa.TransDescr = "Deposit At Atm_Input_Manually";
            }
            else
            {
                Mpa.TransType = 11;
                Mpa.TransDescr = "Withdrawl At Atm_Input_Manually";
                Mpa.DepCount = 0;
            }

            Mpa.IsOwnCard = true;

            //     + " [MatchMask] ,"
            Mpa.NotInJournal = true;

            Mpa.AtmTraceNo = Mpa.TraceNoWithNoEndZero * 10;

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Mpa.ReplCycleNo = Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate);

            if (Mpa.ReplCycleNo >0)
            {
                //Mpa.ReplCycleNo = Ta.SesNo;
            }
            else
            {
                if (MessageBox.Show("Warning: There is no corresponding Replenishment Cycle! " + Environment.NewLine
                         + "Do you want to proceed? "
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
            }

            RRDMJournalReadTxns_Text_Class Jr = new RRDMJournalReadTxns_Text_Class();

            Jr.ReadJournalText_HST_ByDate_Find_Fuid(WOperator, Mpa.TerminalId, Mpa.TransDate);
            if (Jr.RecordFound == true)
            {
                Mpa.FuID = Jr.FuId;
            }
            else
            {
                Mpa.FuID = 0;
            }

            Mpa.IsMatchingDone = true;
            Mpa.Matched = false;
            Mpa.MatchMask = "010";

            Mpa.MetaExceptionId = 0;
            Mpa.MetaExceptionNo = 0;

            Mpa.FileId01 = "Atms_Journals_Txns";
            Mpa.FileId02 = "Switch_IST_Txns";

            if (Mpa.MatchingCateg == "BDC201")
            {
                //if (Mpa.AccNumber.Length != 14)
                //{
                //    MessageBox.Show("Please enter account number of 14 numbers");
                //    return;
                //}
                Mpa.FileId03 = "Flexcube";
                Mpa.TXNSRC = "1";
                Mpa.TXNDEST = "1";
            }
            else
            {
                Mpa.FileId03 = "";
                Mpa.TXNSRC = "1";
                Mpa.TXNDEST = "X";
            }

            Mpa.ResponseCode = "0";

            Mpa.Comments = "Manually Created Transaction";

            Mpa.MatchedType = "Manual";

            Mpa.Operator = WOperator;

            Mpa.ReadInPoolTransSpecificToCheckIfExist(Mpa.TerminalId, Mpa.TransAmount, Mpa.TransDate, Mpa.TraceNoWithNoEndZero); 
            if (Mpa.RecordFound == true)
            {
                MessageBox.Show("This Record Already Exist ");
                return; 
            }

            WSeqNo = Mpa.InsertTransMasterPoolATMs_2_Insert_Manually(WOperator);

            Form503_Load(this, new EventArgs());


        }

        // UPDATE TXN
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (textBoxDisputeId.Text == "")
            {
                // OK Continue
            }
            else
            {
                MessageBox.Show("Dispute is already opened"+Environment.NewLine
                    + "You Cannot make any Update on Txn"
                    );
                return; 
            }

            string WRMCateg;

            Mpa.TerminalId = textBoxTermId.Text.Trim();

            Ac.ReadAtm(Mpa.TerminalId);

            if (Ac.RecordFound == true)
            {
                WRMCateg = "RECATMS-" + Ac.AtmsReconcGroup;
            }
            else
            {
                MessageBox.Show("Not found ATM");
                return;
            }

            Mpa.CardNumber = textBoxMaskPan.Text;

            Mpa.MatchingCateg = textBoxCateg.Text.Trim();

            if (Mpa.MatchingCateg == "BDC201"
                || Mpa.MatchingCateg == "BDC202"
                 || Mpa.MatchingCateg == "BDC203"
                  || Mpa.MatchingCateg == "BDC204"
                   || Mpa.MatchingCateg == "BDC205"
                    || Mpa.MatchingCateg == "BDC206"
                    || Mpa.MatchingCateg == "BDC207"
                    || Mpa.MatchingCateg == "BDC208"
                    || Mpa.MatchingCateg == "BDC209"
                )
            {
                // It is OK
            }
            else
            {
                MessageBox.Show("Please enter a valid Category.(related to ATMS origin)");
                return;
            }

            //Mpa.AccNumber = textBoxAccNo.Text.Trim();

            //int length = textBoxAccNo.Text.Trim().Length; 

            if (Mpa.MatchingCateg == "BDC201" & textBoxAccNo.Text.Trim().Length != 14)
            {
                MessageBox.Show("Please enter a 14 character Account Number eg 00343020094422 ");
                return;
            }
            if (Mpa.MatchingCateg == "BDC101" & textBoxAccNo.Text.Trim().Length != 16)
            {
                MessageBox.Show("Please enter a 16 character Account Number eg 7700343020094422 ");
                return;
            }

            if (textBoxAccNo.Text.Trim().Length > 0)
            {
                if (Mpa.MatchingCateg == "BDC201" || Mpa.MatchingCateg == "BDC101")
                {
                    // For these allowed but not for REST
                }
                else
                {
                    MessageBox.Show("No allowed to insert Accno for this matching category ");
                    return;
                }
            }


            Mpa.AccNumber = textBoxAccNo.Text.Trim();

            if (decimal.TryParse(textBoxAmnt.Text, out decimal TransAmount))
            {
            }
            else
            {
                MessageBox.Show("Please enter amount in EGP ");
                return;
            }
            Mpa.TransAmount = TransAmount;
            if (comboBoxCcy.Text == "EGP")
                Mpa.TransCurr = "818";
            else
            {
                Mpa.TransCurr = comboBoxCcy.Text;
            }
            // Mpa.TraceNoWithNoEndZero = int.TryParse(textBox6.Text);


            if (int.TryParse(textBoxTrace.Text, out Mpa.TraceNoWithNoEndZero))
            {
            }
            else
            {
                if (textBoxTrace.Text != "")
                {
                    MessageBox.Show("Insert Correct Trace");
                    return;
                }
                else
                {
                    Mpa.TraceNoWithNoEndZero = 0;
                }

            }

            Mpa.RRNumber = textBoxRefNumber.Text.Trim();

            Mpa.TransDate = dateTimePicker2.Value;

            // OTHER FIELDS

            Mpa.OriginFileName = "ATMs Journals";
            Mpa.OriginalRecordId = 999999; // six 99999

            Mpa.RMCateg = WRMCateg; // 
            Mpa.LoadedAtRMCycle = WReconcCycleNo;
            Mpa.MatchingAtRMCycle = WReconcCycleNo;
            //Mpa.UniqueRecordId = 0;
            Mpa.Origin = "Our Atms";
            Mpa.TerminalType = "10";

          
            if (comboBoxType.Text == "Deposit")
            {
                Mpa.TransType = 23;
                Mpa.DepCount = Mpa.TransAmount;
                Mpa.TransDescr = "Deposit At Atm_Input_Manually";
            }
            else
            {
                Mpa.TransType = 11;
                Mpa.TransDescr = "Withdrawl At Atm_Input_Manually";
                Mpa.DepCount = 0;
            }

            Mpa.IsOwnCard = true;

            Mpa.NotInJournal = true;

            Mpa.AtmTraceNo = Mpa.TraceNoWithNoEndZero * 10;

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            Mpa.ReplCycleNo = Ta.ReadFindReplCycleForGivenDate(Mpa.TerminalId, Mpa.TransDate);

            if (Mpa.ReplCycleNo>0)
            {
                //Mpa.ReplCycleNo = Ta.SesNo;
            }
            else
            {
                if (MessageBox.Show("Warning: There is no corresponding Replenishment Cycle! " + Environment.NewLine
                         + "Do you want to proceed? "
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
               
            }

            RRDMJournalReadTxns_Text_Class Jr = new RRDMJournalReadTxns_Text_Class();

            Jr.ReadJournalText_HST_ByDate_Find_Fuid(WOperator, Mpa.TerminalId, Mpa.TransDate);
            if (Jr.RecordFound == true)
            {
                Mpa.FuID = Jr.FuId;
            }
            else
            {
                Mpa.FuID = 0;
            }

            Mpa.IsMatchingDone = true;
            Mpa.Matched = false;
            Mpa.MatchMask = "010";

            Mpa.MetaExceptionId = 0;
            Mpa.MetaExceptionNo = 0;


            Mpa.FileId01 = "Atms_Journals_Txns";
            Mpa.FileId02 = "Switch_IST_Txns";

            if (Mpa.MatchingCateg == "BDC201")
            {
                if (Mpa.AccNumber.Length < 14)
                {
                    MessageBox.Show("Please enter account number of 14 numbers");
                    return;
                }
                Mpa.FileId03 = "Flexcube";
                Mpa.TXNSRC = "1";
                Mpa.TXNDEST = "1";
            }
            else
            {
                Mpa.FileId03 = "";
                Mpa.TXNSRC = "1";
                Mpa.TXNDEST = "X";
            }

            Mpa.ResponseCode = "0";

            Mpa.Comments = "Manually Created Transaction";

            Mpa.Operator = WOperator;

            Mpa.UpdateMatchingTxnsMasterPoolATMsManual(WOperator, WSeqNo);


            MessageBox.Show("Updating Done!");

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "TXN updated.";

            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

        // DELETE TXN
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (textBoxDisputeId.Text == "")
            {
                // OK Continue
            }
            else
            {
                MessageBox.Show("Dispute is already opened" + Environment.NewLine
                    + "You Cannot Delete The Transaction"
                    );
                return;
            }

            if (MessageBox.Show("Warning: Do you want to delete this Record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                Mpa.DeleteManualBySeqNo(WSeqNo);
                
                MessageBox.Show("Record has been Deleted");

                textBoxMsgBoard.Text = "Record has been Deleted.";

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form503_Load(this, new EventArgs());



                if (WRowIndex1 > 0)
                {
                    WRowIndex1 = WRowIndex1 - 1;
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }
            }
            else
            {
                return;
            }
        }


        private void button52_Click(object sender, EventArgs e)
        {
           
        }

        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


     
        // Print
        private void buttonPrint_Click(object sender, EventArgs e)
        {

            //string P1 = " Details ";

            //string P2 = "Second Par";
            //string P3 = "Third Par";
            //string P4 = WOperator;
            //string P5 = WSignedId;

            //Form56R76 Report76 = new Form56R76(P1, P2, P3, P4, P5);
            //Report76.Show();
        }

      
        // NOTES BUTTON
        private void buttonNotes_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "Parameters";
            // string WParameter4 = WParameter4 = "Parameter Id:" + Gp.ParamId.ToString() + " Occurance Id: " + Gp.OccuranceId;
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            string WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";
        }
        // Link to Categories 
        private void linkLabelExpand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // SHOW ALL Categories 
            //
            Form78d_SlaveCategories NForm78d_SlaveCategories;
            int WMode = 4; // show all categories  
            NForm78d_SlaveCategories = new Form78d_SlaveCategories(WOperator, WSignRecordNo, WSignedId, "", WReconcCycleNo, WMode);
            NForm78d_SlaveCategories.ShowDialog();
        }
        // Make new entry 
        private void checkBoxMakeNewEntry_CheckedChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }

            if (checkBoxMakeNewEntry.Checked == true)
            {

                textBoxTermId.Text = "";
                textBoxMaskPan.Text = "";
                textBoxCateg.Text = "";
                textBoxAccNo.Text = "";
                textBoxAmnt.Text = "";
                textBoxTrace.Text = "";
                textBoxRefNumber.Text = ""; 

                buttonAdd.Show();
                //textBoxTermId.ReadOnly = false;
                buttonUpdate.Hide();
                buttonDelete.Hide();

                panel3.Hide();

                // Do not show grid 
                panel2.Hide();
                label1.Hide(); 

              
            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();

                // Show Dispute Information 
                panel3.Show();

                // Show grid 
                panel2.Show();
                label1.Show();

                //dataGridView2.Enabled = true;
                int WRowIndex1 = -1;

                if (dataGridView1.Rows.Count > 0)
                {
                    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                }

                Form503_Load(this, new EventArgs());

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }

            }
        }
        // Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            Form5 NForm5;
            int From = 2; // Coming from Pre-Investigattion ATMs 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Mpa.CardNumber, Mpa.UniqueRecordId, 0, 0, "", From, "ATM");
            NForm5.FormClosed += NForm5_FormClosed;
            NForm5.ShowDialog();
        }

        private void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            // Form80b_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        // link to dispute 
        private void linkLabelDispute_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 NForm3; 
            NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator, textBoxDisputeId.Text, NullPastDate, NullPastDate, 13);
            NForm3.FormClosed += NForm3_FormClosed;
            NForm3.ShowDialog();
        }
        void NForm3_FormClosed(object sender, FormClosedEventArgs e)
        {
            int WRow = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
        // Near Journal 
        int IntTrace;

        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 

            string WSubString = Mpa.MatchMask.Substring(0, 1);

            DateTime SavedTransDate = Mpa.TransDate;

            if (WSubString == "1")
            {
                IntTrace = Mpa.TraceNoWithNoEndZero;
            }

            bool In_HST = false;

            //if (Mpa.TransDate.Date <= HST_DATE)
            //{
            //    In_HST = true;
            //}
            // Show Lines of journal 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;
            //if (In_HST == true)
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            //}
            //else
            //{
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            //}

            string SelectionCriteria;
            int SaveSeqNo = WSeqNo;

            int WSeqNoA = 0;
            int WSeqNoB = 0;

            //DateTime TestingDate = new DateTime(2019, 01, 03);

            SaveSeqNo = Mpa.SeqNo;

            DateTime WDtA = Mpa.TransDate;
            DateTime WDtB = Mpa.TransDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-2);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";

            if (In_HST == true)
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
            }
            else
            {
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            }

            if (Mpa.RecordFound == true)
            {
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(2);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";


                if (In_HST == true)
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal_HST(SelectionCriteria, WDt, OrderBy, 2);
                }
                else
                {
                    Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
                }

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  

                }
                else
                {
                    MessageBox.Show("No Upper Limit. No Journal Lines to Show");

                    WSeqNoB = 0;
                    // Reestablish Mpa Data
                    //
                    //SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

                    //return;
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. No Journal Lines to Show");
                // Reestablish Mpa Data
                //
                SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

                if (In_HST == true)
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                }
                else
                {
                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
                }

                // Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
                return;
            }
            //
            // Reestablish Mpa Data
            //
            SelectionCriteria = " WHERE  SeqNo =" + SaveSeqNo;

            if (In_HST == true)
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(SelectionCriteria, 2);
            }
            else
            {
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);
            }

            //   Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            if (WSubString == "1")
            {
                WTraceRRNumber = IntTrace.ToString();
            }

            if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;

            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      SavedTransDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // 
        private void buttonDeleteOpenDispute_Click(object sender, EventArgs e)
        {
            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                if (Dt.ClosedDispute == true || Dt.DisputeActionId == 5)
                {
                    MessageBox.Show("Action had been taken." + Environment.NewLine
                    + "You cannot delete it"
                    );
                    return;
                }       
                else
                {
                    if (MessageBox.Show("Warning: Do you want to delete this dispute? " + Environment.NewLine
                         + " "
                         , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                  == DialogResult.Yes)
                    {
                        // YES Proceed
                        Dt.DeleteTransOfthisDispute(Dt.DisputeNumber);
                        Dt.DeletethisDispute(Dt.DisputeNumber);

                        // Delete Authorisation record if any 
                        RRDMAuthorisationProcess Au = new RRDMAuthorisationProcess(); 
                        Au.DeleteAuthorisationRecord_Disp(Dt.DisputeNumber, Dt.DispTranNo);

                        MessageBox.Show("Delete done!");

                        WRowIndex = dataGridView1.SelectedRows[0].Index;

                        int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

                        textBoxMsgBoard.Text = "Dispute Deleted.";

                        Form503_Load(this, new EventArgs());

                        dataGridView1.Rows[WRowIndex].Selected = true;
                        dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

                        dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;

                    }
                    else
                    {
                        // Stop 

                        return;
                    }
                    
                }
            }
        }
// Create SOLO Dispute 
        private void buttonCreateTXNSolo_Click(object sender, EventArgs e)
        {
            if (Mpa.ActionType != "00" & Mpa.SettledRecord == false)
            {
                // Transaction uder reconciliation or replenishment 
                MessageBox.Show("Record is not settled yet. " + Environment.NewLine
                 + "It might be under workflow process. " + Environment.NewLine
                 + "You cannot open a dispute. " + Environment.NewLine
                    );
                return;
            }
            int WDispNo = 0;
            int DispTranNo = 0;
            RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

            WDispNo = Di.Create_Pseudo_Dispute(WOperator, WSignedId, Mpa.UniqueRecordId, 111);

            Di.ReadDispute(WDispNo);

            Dt.Create_Pseudo_Dispute_TXN(WOperator, WSignedId, WDispNo, Mpa.UniqueRecordId, 111);

            Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
            DispTranNo = Dt.DispTranNo;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 2; // Return to stage 2  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WOrigin = 12; // From Requestor = Dispute Management 
            Form109 NForm109;

            NForm109 = new Form109(WSignedId, WSignRecordNo, WOperator, WDispNo, DispTranNo, Mpa.UniqueRecordId, WOrigin);
            NForm109.FormClosed += NForm5_FormClosed;
            NForm109.ShowDialog();
        }
    }
}

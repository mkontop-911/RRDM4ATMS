using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_FileRecords_Reversals : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDM_ReversalsTable Rv = new RRDM_ReversalsTable();
        //   RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
        // int SavedMode;
        //  bool InternalUse; 

        int WSeqNo;

        string WOperator;
        string WSignedId;
        string WTableId;

        DateTime WFromDate;
        DateTime WToDate;
        int WMode;

        public Form78d_FileRecords_Reversals(string InOperator, string InSignedId, string InTableId,
             DateTime InFromDate, DateTime InToDate, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WTableId = InTableId; // eg InTable == "IST"
            WFromDate = InFromDate;
            WToDate = InToDate;

            WMode = InMode; // 1: Reversals from Dt to Dt 
                            // 2: 
                            // 3: 
                            // 4 : Reversals for Deposits 

            WAtmNo = "";
            InitializeComponent();

        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WMode == 1)
            {
                labelWhatGrid.Text = "Reversals from:.." + WFromDate.ToShortDateString() + "..To.." + WToDate.ToShortDateString();

                Mgt.ReadTableAndFillTableWithReversals(WTableId, WFromDate, WToDate, WSignedId, WAtmNo,WMode, 1);

            }
            if (WMode == 3)
            {
                labelWhatGrid.Text = "Reversals for ATM.." + WAtmNo + ".. from:.." + WFromDate.ToShortDateString() + "..To.." + WToDate.ToShortDateString();

                Mgt.ReadTableAndFillTableWithReversals(WTableId, WFromDate, WToDate, WSignedId, WAtmNo ,WMode, 1);

            }

            if (WMode == 4)
            {
                labelWhatGrid.Text = "Reversals for Deposits.." + ".. from:.." + WFromDate.ToShortDateString() + "..To.." + WToDate.ToShortDateString();

                Mgt.ReadTableAndFillTableWithReversals(WTableId, WFromDate, WToDate, WSignedId, WAtmNo ,WMode, 1);

            }

            dataGridView1.DataSource = Mgt.DataTableAllFields.DefaultView;

            int rows = Mgt.DataTableAllFields.Rows.Count;
            textBoxRecords.Text = rows.ToString();
            if (rows == 0)
            {
                MessageBox.Show("No records to show");
                this.Dispose();
                return;
            }
            // SHOW GRID

            if (WMode == 4)
            {
                dataGridView2.DataSource = Mgt.DataTableSelectedFields.DefaultView;
                ShowGrid03(); 
            }

        }

        // On ROW ENTER 
        bool OwnATMs; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;
            string WCategory = (string)dataGridView1.Rows[e.RowIndex].Cells["MatchingCateg"].Value;

            string Cat_Three_Digits = WCategory.Substring(3, 3);

            if (Cat_Three_Digits == "201"
                || Cat_Three_Digits == "202"
                 || Cat_Three_Digits == "203"
                  || Cat_Three_Digits == "204"
                   || Cat_Three_Digits == "205"
                    || Cat_Three_Digits == "206"
                     || Cat_Three_Digits == "207"
                     || Cat_Three_Digits == "208"
                     || Cat_Three_Digits == "209"
                )
            {
                OwnATMs = true; 
            }
            else
            {
                OwnATMs = false; 
            }


            //MatchingCateg
            if (WMode == 1 & OwnATMs == true)
            {
                Mgt.ReadTableAndFillTableWithReversalsSeqNo_Second_Table(WTableId, WSignedId
                                      , WSeqNo, WMode, 1);

                dataGridView2.DataSource = Mgt.DataTableSelectedFields.DefaultView;
                ShowGrid03();

               
            }
           

            //string SelectionCriteria = " WHERE SEQNO ="+ WSeqNo; 

            //    Mgt.ReadTransSpecificFromSpecificTable_Order_By_Date(SelectionCriteria, Mgt.PhysicalFiledID);
            //    textBoxTerm.Text = Mgt.TerminalId;
           
            if ((WMode == 1 || WMode == 4) & OwnATMs == true) 
            {
                //
                labelDepStatus.Show();
                dataGridView2.Show();
                buttonJournal_Near.Show();
                buttonJournalLines.Show();
                buttonSourceReords.Show();
                buttonShowJournal.Show(); 

                label2.Hide();
                textBoxTerm.Hide();
                buttonShowTerm.Hide();
                buttonRefresh.Hide();
                buttonPrint.Hide();
            }
            else
            {
                labelDepStatus.Hide();
                dataGridView2.Hide();
                buttonJournal_Near.Hide();
                buttonJournalLines.Hide();
                buttonSourceReords.Hide();
                buttonShowJournal.Hide();
            }


        }
        DateTime WReversalDate;
        string WTerminalId;
        int WTrace;
        string WMatchingCat;
        decimal WTransAmt;
        string WCardNumber;
        string AcNo;
        int WSeqNo_2;
        int WFuid; 
        // Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (WMode == 4 || WMode == 1)
            {
                DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
                WSeqNo = (int)dataGridView2.Rows[e.RowIndex].Cells["SeqNo"].Value;
                WFuid = (int)dataGridView2.Rows[e.RowIndex].Cells["Fuid"].Value;
                string WSelectionCriteria = "WHERE SeqNo =" + WSeqNo;
                Rv.ReadReversalsBy_Selection_criteria(WSelectionCriteria);

                WReversalDate = Rv.TransDate_2;
                WTerminalId = Rv.TerminalId_2;
                WTrace = Rv.TraceNo_2;
                WMatchingCat = Rv.MatchingCateg;
                WTransAmt = Rv.TransAmt_2;
                WCardNumber = Rv.CardNumber_2;
                AcNo = Rv.AccNo_2;
                WSeqNo_2 = Rv.SeqNo_2;
            }
            else
            {

            }
        }



        // Show Grid 03 
        public void ShowGrid03()
        {

            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 50; // Fuid
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 60; // Atm No
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "";


            P1 = "Presenter Errors from:.." + WFromDate.ToShortDateString() + "..To.." + WToDate.ToShortDateString();

            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;


            if (WTableId == "Atms_Journals_Txns")
            {
                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
            }
            else
            {
                Form56R68ATMS_W_General_Files ReportATMS_General_Files = new Form56R68ATMS_W_General_Files(P1, P2, P3, P4, P5);
                ReportATMS_General_Files.Show();
            }


        }
        // Show Terminal
        bool InternalUse;
        string WAtmNo;
        private void buttonShowTerm_Click(object sender, EventArgs e)
        {
            InternalUse = true;
            WMode = 3;
            WAtmNo = textBoxTerm.Text;
            if (WAtmNo == "")
            {
                MessageBox.Show("Please enter the ATM no. ");
                return;
            }
            Form78b_Load(this, new EventArgs());
        }
        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            InternalUse = true;
            WMode = 1;

            WAtmNo = "";

            Form78b_Load(this, new EventArgs());
        }
        // NEAR JOURNALS 
        private void buttonJournal_Near_Click(object sender, EventArgs e)
        {
            // Here we search the journal lines close to the selected transaction
            // If selected already in Journal then we show the one before and the one after
            // If transaction not in journal say 011 then we show 5 minutes before and 5 minutes after. 
            // This is because the time at mainframe maybe different that this of ATM. 
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            //WReversalDate = Rv.TransDate_2;
            //WTerminalId = Rv.TerminalId_2;
            //WTrace = Rv.TraceNo_2;

            //bool LowerLimitPresent = false;
            //bool UpperLimitPresent = false;

            string SelectionCriteria;
            //  int SaveSeqNo = Mpa.SeqNo;

            int WSeqNoA = 0;
            int WSeqNoB = 0;


            //DateTime TestingDate = new DateTime(2019, 01, 03);

            //SaveSeqNo = Mpa.SeqNo;
            DateTime WDtA = WReversalDate;
            DateTime WDtB = WReversalDate;
            DateTime WDt;

            WDt = WDtA.AddMinutes(-1);

            // FIND THE LESS
            SelectionCriteria = " WHERE TerminalId ='" + WTerminalId
                              + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate < @TransDate  ";
            string OrderBy = "  ORDER By TransDate Desc";
            Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);
            if (Mpa.RecordFound == true)
            {
               // LowerLimitPresent = true; 
                // Assign Seq number for Pambos Journal table
                WSeqNoA = Mpa.OriginalRecordId;

                // FIND THE GREATEST that exist 
                WDt = WDtB.AddMinutes(1);
                SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId
                           + "' AND Origin = 'Our Atms' AND NotInJournal = 0 AND TransDate > @TransDate ";

                OrderBy = "  ORDER By TransDate ASC ";
                Mpa.ReadInPoolTransSpecificNearAtmJournal(SelectionCriteria, WDt, OrderBy, 2);

                if (Mpa.RecordFound == true)
                {
                    // 
                    WSeqNoB = Mpa.OriginalRecordId; // This is the SeqNo in Pambos Journal  
                    //UpperLimitPresent = true; 

                }
                else
                {
                   // MessageBox.Show("No Upper Limit. ");

                    //UpperLimitPresent = false;

                    WSeqNoB = 0;
                  
                }


            }
            else
            {
                MessageBox.Show("No Lower Limit. ");

                //LowerLimitPresent = false;

                return; 

               
            }
          
            //
            // Bank De Caire
            //
            Form67_BDC NForm67_BDC;

            int Mode = 5; // Specific range
            //string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
            //if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
           
            NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, 0, WTrace.ToString(), Mpa.TerminalId, WSeqNoA, WSeqNoB,
                                                      WReversalDate, NullPastDate, Mode);
            NForm67_BDC.ShowDialog();

        }
        // Show Journal Lines 
        private void buttonJournal_Click(object sender, EventArgs e)
        {

            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            // Show Lines of journal 
            //int WWUniqueRecordId = 0; 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;

            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            //WReversalDate = Rv.TransDate_2;
            //WTerminalId = Rv.TerminalId_2;
            //WTrace = Rv.TraceNo_2;
            //WMatchingCat = Rv.MatchingCateg;
            //WTransAmt = Rv.TransAmt_2;
            //WCardNumber = Rv.CardNumber_2;
            //AcNo = Rv.AccNo_2;

            Mpa.ReadInPoolTransSpecificDuringMatching_5(WMatchingCat, WTerminalId, WReversalDate.Date
                                                    , WTrace, WTransAmt
                                                    , WCardNumber, AcNo
                                                    , 1);

            if (Mpa.RecordFound == true)
            {
                if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
                {
                    MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                     + "Select Journal Lines Near To this"
                                     );
                    return;
                }

                int WSeqNoA = 0;
                int WSeqNoB = 0;

                if (Mpa.TraceNoWithNoEndZero == 0)
                {
                    MessageBox.Show("No Available Trace to show the Journal Lines for this Txn/Category ");
                    return;
                }
                else
                {
                    // Assign Seq number for Pambos Journal table
                    WSeqNoA = Mpa.OriginalRecordId;
                    WSeqNoB = Mpa.OriginalRecordId;
                }

                //
                // Bank De Caire
                //
                Form67_BDC NForm67_BDC;

                int Mode = 5; // Specific
                string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
                if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, WSeqNoA, WSeqNoB, Mpa.TransDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }


        }
        // Source Records
        private void buttonSourceReords_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            // Show Lines of journal 
            //int WWUniqueRecordId = 0; 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;

            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            //WReversalDate = Rv.TransDate_2;
            //WTerminalId = Rv.TerminalId_2;
            //WTrace = Rv.TraceNo_2;
            //WMatchingCat = Rv.MatchingCateg;
            //WTransAmt = Rv.TransAmt_2;
            //WCardNumber = Rv.CardNumber_2;
            //AcNo = Rv.AccNo_2;

            Mpa.ReadInPoolTransSpecificDuringMatching_5(WMatchingCat, WTerminalId, WReversalDate.Date
                                                    , WTrace, WTransAmt
                                                    , WCardNumber, AcNo
                                                    , 1);

            if (Mpa.RecordFound == true)
            {
                Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
                Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

                int Mode = 1;

                NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, Mpa.UniqueRecordId, Mode);
                //  NForm78d_AllFiles_BDC_3.FormClosed += NForm5_FormClosed;
                NForm78d_AllFiles_BDC_3.ShowDialog();
            }
            else
            {
                Form78d_AllFiles NForm78d_AllFiles; // BASED ON TRACE NUMBER
                Form78d_AllFiles_BDC_3 NForm78d_AllFiles_BDC_3; // BASED ON DIFFERENT VARIABLES

                //  WSeqNo_2 = Rv.SeqNo_2;

                int Mode = 2;

                NForm78d_AllFiles_BDC_3 = new Form78d_AllFiles_BDC_3(WOperator, WSignedId, WSeqNo_2, Mode);
                //  NForm78d_AllFiles_BDC_3.FormClosed += NForm5_FormClosed;
                NForm78d_AllFiles_BDC_3.ShowDialog();

                //MessageBox.Show("Unable to show");

            }

        }
        // Show Journal 
        private void buttonShowJournal_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            // Show Lines of journal 
            //int WWUniqueRecordId = 0; 
            //string SelectionCriteria = " WHERE UniqueRecordId =" + WWUniqueRecordId;

            //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            //WReversalDate = Rv.TransDate_2;
            //WTerminalId = Rv.TerminalId_2;
            //WTrace = Rv.TraceNo_2;
            //WMatchingCat = Rv.MatchingCateg;
            //WTransAmt = Rv.TransAmt_2;
            //WCardNumber = Rv.CardNumber_2;
            //AcNo = Rv.AccNo_2;

            Mpa.ReadInPoolTransSpecificDuringMatching_5(WMatchingCat, WTerminalId, WReversalDate.Date
                                                    , WTrace, WTransAmt
                                                    , WCardNumber, AcNo
                                                    , 1);

            if (Mpa.RecordFound == true)
            {
                if (Mpa.MatchMask == "001"
                || Mpa.MatchMask == "011"
                || Mpa.MatchMask == "010"
                || Mpa.MatchMask == "01"
                )
                {
                    MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                     + "Select Journal Lines Near To this"
                                     + "Or Not succesful go to all Journals"
                                     );
                    return;
                }
                else
                {
                    Form67_BDC NForm67_BDC;

                    int Mode = 3; // Specific Journal 
                    string WTraceRRNumber = Mpa.TraceNoWithNoEndZero.ToString();
                    if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
                    NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, Mpa.FuID, WTraceRRNumber, Mpa.TerminalId, 0, 0, Mpa.TransDate, NullPastDate, Mode);
                    NForm67_BDC.ShowDialog();
                }
            }
            else
            {
                if (WFuid>0)
                {
                    Form67_BDC NForm67_BDC;

                    int Mode = 3; // Specific Journal 
                   // string WTraceRRNumber = ""
                   // if (Mpa.TraceNoWithNoEndZero == 0 & Mpa.RRNumber != "") WTraceRRNumber = Mpa.RRNumber;
                    NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, WFuid, "", WTerminalId, 0, 0, WReversalDate, NullPastDate, Mode);
                    NForm67_BDC.ShowDialog();
                }
                else
                {
                    MessageBox.Show("This Txn has no journal entry" + Environment.NewLine
                                     + "Select Journal Lines Near To this"
                                     + "Or Not succesful go to all Journals"
                                     );
                }
               
            }

          
        }
    }
}


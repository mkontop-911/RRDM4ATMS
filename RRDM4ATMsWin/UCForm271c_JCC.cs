using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class UCForm271c_JCC : UserControl
    {

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingOfTxnsFindOriginRAW Msf = new RRDMMatchingOfTxnsFindOriginRAW();

        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        //   string WUserOperator; 
        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool ViewWorkFlow;

        // NOTES START
   //     string Order;
      //  string WParameter4;
       // string WSearchP4;
     //   string WMode;

        int CallingMode; // 

     //   string WSubString;
      //  string WMask;

        // NOTES END 


     //   DateTime LeftDt;

        int WUniqueRecordId;

        //int WMaskRecordIdLeft;

        string WCardNumberLeft;
        string WAccNumberLeft;
        string WTransCurrLeft;
        decimal WTransAmountLeft;
        int WTraceNo; 

        string SelectionCriteria;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WRMCategoryId;
        int WRMCycleNo;
        string WOperator;
        string WMainCateg;

        int WAction;

        public void UCForm271c_JCCPar(string InSignedId, int SignRecordNo, string InOperator, string InRMCategoryId, int InRMCycle, int InAction)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WRMCategoryId = InRMCategoryId;
            WRMCycleNo = InRMCycle;
            WOperator = InOperator;

            WMainCateg = WRMCategoryId.Substring(0, 4);

            WAction = InAction;  // 1 = Matching Actions 

            InitializeComponent();

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }

            //labelStep1.Text = "Invstigation and Force Matching for RM Category Id : " + WRMCategoryId;


            // Hide RIGHT PANELS GRID AND INFORMATION 
            //***************************************

            //panel3.Hide();
            panel7.Hide();
            textBox3.Hide();
            textBox14.Hide();

          
            label4.Hide();
            textBoxMaskLeft.Hide();

            if (WMainCateg == "EWB5")
            {
                MessageBox.Show("For visa this Form has to be changed . On the left we show the settlement transactions on the right the authorisation ");
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
                
            }

            if (WMainCateg == "RECA")
            {
                buttonJournal.Show();
            }
            else buttonJournal.Hide();


            if (WMainCateg == "RECA" || WMainCateg == "EWB3"
                || WMainCateg == "NBG3"
                )
            {
             
            }

            //comboBoxReason.Items.Add("See attached");
            //comboBoxReason.Items.Add("Small difference");
            //comboBoxReason.Text = "See attached";

            if (WMainCateg == "EWB5")
            {
                textBoxHeaderLeft.Text = "VISA SETTLEMENT TRANSACTIONS";
              
            }

        }


        // SHOW SCREEN 

        public void SetScreen()
        {

            try
            {
                
                    if (ViewWorkFlow == true) // View Only 
                    {
                        //UpdateExceptionIds
                       SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                                          +  WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo 
                                          + " AND IsMatchingDone = 1 AND FastTrack = 0 AND MetaExceptionNo > 0 "
                                          + " AND Matched = 0 AND ActionType != '07' ";
                        CallingMode = 1; // View
                    }
                    else
                    {
                        // Matching is done but not Settled 
                        SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                                  + "  AND MatchingAtRMCycle = " + WRMCycleNo 
                                  + "  AND IsMatchingDone = 1 AND FastTrack = 0 AND MetaExceptionNo > 0 "
                                  + "  AND Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '07' ";
                        CallingMode = 2; // Updating 
                    }

                    textBoxHeaderLeft.Text = "EXCEPTIONS (UNMATCHED ATM TRANSACTIONS)";
                    //buttonMovedToUnMatched.Hide();
              
   

                string WSortCriteria = "";

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                Mpa.ReadMatchingTxnsMasterPoolByCategoryAndCycleAndFillTable(SelectionCriteria,1);

                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='"
                      + WRMCategoryId + "' AND MatchingAtRMCycle =" + WRMCycleNo + " AND IsMatchingDone = 1 ";

                Mpa.ReadMatchingTxnsMasterPoolATMsTotals(SelectionCriteria,2);

                if (Mpa.TotalUnMatched - (Mpa.TotalActionsByUserDefaultAndManual 
                    + Mpa.TotalForcedMatched + Mpa.TotalMoveToDisputeNumber + Mpa.TotalFastTrack) == 0)
                {
                    UpdateReconcStatus(1); // Reconciled 
                }
                else
                {
                    UpdateReconcStatus(2); // Not Reconciled 
                }


                ShowGrid();

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }
        }

        // ON Left Grid Row Enter 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WUniqueRecordId = (int)rowSelected.Cells[0].Value;

            SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

            // NOTES 2 START  
          //  Order = "Descending";
         //   WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
         //   WSearchP4 = "";
        
            // NOTES 2 END


            WUniqueRecordId = Mpa.UniqueRecordId;

            WCardNumberLeft = Mpa.CardNumber;
            WAccNumberLeft = Mpa.AccNumber;
            WTransCurrLeft = Mpa.TransCurr;
            WTransAmountLeft = Mpa.TransAmount;
            WTraceNo = Mpa.TraceNoWithNoEndZero; 

            textBoxCardNoLeft.Text = WCardNumberLeft;
            textBoxAccNoLeft.Text = WAccNumberLeft;
            textBoxCurrLeft.Text = WTransCurrLeft;
            textBoxAmountLeft.Text = WTransAmountLeft.ToString("#,##0.00");
            textBoxDateLeft.Text = Mpa.TransDate.ToString();
            textBoxTraceNo.Text = WTraceNo.ToString(); 

            textBoxMaskLeft.Text = Mpa.MatchMask;
      
        }

        // Show Grid Left 
        public void ShowGrid()
        {
            //WTotalLeft = 0 ;

            //Keyboard.Focus(dataGridView1);

            dataGridView1.DataSource = Mpa.DataTableActionsTaken.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // MaskRecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // Status
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; //  Done
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 70; // Terminal
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 50; // Terminal Type, ATM, POS etc 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // Descr
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[6].Width = 40; // Err
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[7].Width = 40; // Mask
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[8].Width = 90; // Account
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView1.Columns[9].Width = 50; // Ccy
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[10].Width = 80; // Amount
            //dataGridView1.Columns[10].DefaultCellStyle = style;
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            //dataGridView1.Columns[11].Width = 120; // Date
            //dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[12].Width = 70; // ActionType
            //dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }
      
        //EXPAND GRID 
        Form78b NForm78b;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WRowIndex = dataGridView1.SelectedRows[0].Index;
            string WHeader = "UNMATCHED TRANSACTIONS FOR RM CATEGORY : " + WRMCategoryId;
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form271b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            //dataGridView1.Rows[NForm78b.WSelectedRow].Selected = true;
            if (NForm78b.UniqueIsChosen == 1)
            {
                SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                               + "  AND MatchingAtRMCycle = " + WRMCycleNo
                              + " AND SettledRecord = 0 AND RemainsForMatching = 0 AND  WUniqueRecordId = " + NForm78b.WPostedNo.ToString() + " ";

                CallingMode = 2;
                string WSortCriteria = "";

                //No Dates Are selected

                DateTime FromDt = NullPastDate;
                DateTime ToDt = NullPastDate;

                Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, CallingMode, SelectionCriteria, WSortCriteria, FromDt, ToDt,2);
                //Mpa.ReadMatchingTxnsMasterPoolAndFillTable(WOperator, CallingMode, SelectionCriteria, "");

                ShowGrid();

            }
        }


        // Show Part of Journal 
        private void buttonJournal_Click(object sender, EventArgs e)
        {

            // Show Lines of journal 
            string SelectionCriteria = " WHERE UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

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

        // TRANSACTION TRAIL FOR THE LEFT
        private void button1_Click(object sender, EventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles;

            if (WSignedId == "1007_BDO")
            {
                NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                NForm78d_AllFiles.ShowDialog();
            }
            else
            {
                //MessageBox.Show("You will see an Example for Trace 325668 " + Environment.NewLine
                //                        +" For exact cases Sign on with User 1007_BDO");

                //string TerminalId = "00005128";
                //int TempTraceNo = 325668 ;
                //string WorkMask = "001";
                //DateTime TransDate = new DateTime(2017, 05, 16);

                NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                NForm78d_AllFiles.ShowDialog();
                
            }
                
       
        }
       
        // Print Actions 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
          
            // Matching is done but not Settled 
            SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WRMCategoryId + "'"
                      + "  AND MatchingAtRMCycle =" + WRMCycleNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
                      + " AND ActionType != '07' ";

            string WSortCriteria = "Order By TerminalId, SeqNo ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);

            string P1 = "Transactions For Reconciliation :" + WRMCategoryId 
                         + " AND Cycle : " + WRMCycleNo.ToString();

            string P2 = "";
            string P3 = "";

            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle(WRMCategoryId, WRMCycleNo, "ReconciliationCat");

                if (Ap.RecordFound == true)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }
                else
                {
                    //ReconciliationAuthorNoRecordYet = true;
                }

            }
            else
            {
                Us.ReadUsersRecord(WSignedId);
                P2 = Us.UserName;
                P3 = "N/A";
            }
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R55ATMS ReportATMS55 = new Form56R55ATMS(P1, P2, P3, P4, P5);
            ReportATMS55.Show();
        }
// Video clip
        private void buttonVideoClip_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
// Update Reconciliation Status 
        public void UpdateReconcStatus(int InReconStatus)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = InReconStatus;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }
    }
}

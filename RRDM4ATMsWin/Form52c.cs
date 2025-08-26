using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Configuration;
using System.Text;
using System.Data;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form52c : Form
    {

        Form75 NForm75; // Reconciliation 

        Form112 NForm112; // Auth History 

        Form14a NForm14a; // Creating Meta 

        string WAtmNo;
        int WSesNo;

        int WRowIndexLeft;
        int WRowIndexRight;

        string StageDescr;
        int WAuthorSeqNumber;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;

        bool UnMatchedFound;

        bool ViewWorkFlow;
        string WMode;

        int Process;

        int I;

        bool WUnderAction;

        //bool RedoExpressPossible;

       
        int WErrNo;

        int WUniqueRecordId;

        int WErrId;

        int WTraceNo;

        //int ExpressMode;

        int WReconcGroupNo;

        DateTime Cut_Off_Date;

        //decimal WTempAdj ;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        DataTable CurrentATMsTableCashReconciliation = new DataTable();

        bool WithDate;

        //string Gridfilter;

        string WBankId;

        int WPreRMCycle;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

        RRDMAtmsClass Aa = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMJournalAndAllowUpdate Aj = new RRDMJournalAndAllowUpdate();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WRMCategory;

        int WRMCycle;

        bool WViewHistory;

        public Form52c(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycleNo, bool InViewHistory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WRMCategory = InCategory;

            WRMCycle = InRMCycleNo;

            WViewHistory = InViewHistory;
            // If TRUE then you see all history based on RMCycle that reconcilistion was made. 
            // If FALSE You see as normal 

            InitializeComponent();

            // Set Working Date 
           
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            UserId.Text = InSignedId;

            Rc.ReadReconcCategorybyCategId(WOperator, WRMCategory);

            WReconcGroupNo = Rc.AtmGroup;  // Used for Atms Main

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            //
            // Assign Category to be used during Recobciliation 
            //
            Usi.WFieldChar1 = WRMCategory;
            Usi.WFieldNumeric1 = WRMCycle;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            //  UPDATE Authorised User field in ATMs Main
            //
            Ug.ReadUsersAccessAtmAndUpdateMain(WSignedId, 2, "Reconc");
            Ug.ReadUsersAccessAtmAndUpdateMain(WSignedId, 1, "Reconc");

            //UnMatchedRecords = 0; 

            labelStep1.Text = "Reconc Of ATMs of counted Cash vs GL Cash and Presenter Errors Handling.";

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.ReadReconcJobCyclesById(WOperator, WRMCycle);

            Cut_Off_Date = Rjc.Cut_Off_Date;

            //************************************************************
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            //WRMCategory = Us.WFieldChar1;
            //WRMCycle = Us.WFieldNumeric1;

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor || WViewHistory)
            {
                NormalProcess = false;
                buttonCreateError.Enabled = false;
                buttonRedoExpress.Enabled = false;
                buttonUndoExpress.Enabled = false;
                buttonAuthor.Enabled = false;
                buttonRefresh.Enabled = false;
                buttonApplyAction.Enabled = false;
                buttonUndoAction.Enabled = false;
            }
            else
            {
                NormalProcess = true;
            }

            if (WAuthoriser == true)
            {
                //panel2.Location.X.Equals = 9; 
            }

            if (WViewFunction == true) textBoxMsgBoard.Text = "View Only!";
            if (WAuthoriser == true) textBoxMsgBoard.Text = "Authoriser to examine one by one and take action.";
            if (WRequestor == true) textBoxMsgBoard.Text = "Wait for Authoriser.Use Refresh if you wish.";
            if (NormalProcess == true) textBoxMsgBoard.Text = "Examine one by one and take actions.";

            //*************************************************** 
            //***************************************************
            // 
            //***************************************************
            //***************************************************
            if (WViewHistory == false)
            {

                Tuc.ReadToFindAtmsofThisGroupThatNeedReconciliation(WSignedId, WSignRecordNo, WOperator, WReconcGroupNo, 1);

                if (Tuc.TotalAtmsWithUnMatchedRecords > 0)
                {
                    UnMatchedFound = true;
                }
            }
            else
            {
                labelStep1.Text = "View History Of ATMs Cash account reconciliation.";
            }
        }
        //
        // LOAD
        //

        private void Form52b_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet61.ErrorsTable' table. You can move, or remove it, as needed.
            //this.errorsTableTableAdapter.Fill(this.aTMSDataSet61.ErrorsTable);
            if (WViewHistory == false)
            {
                if (UnMatchedFound == true)
                {
                    if (MessageBox.Show("Warning: There Are Outstanding UnMatched Records. " + Environment.NewLine + "DoYou want to proceed?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                    {
                        // Proceed 
                    }
                    else
                    {
                        this.Dispose();
                        return;
                    }
                }
            }


            // START RECONCILIATION 
            // START RECONCILIATION 
            if (NormalProcess == true)
            {

                Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WRMCategory, WRMCycle);

                if (Rcs.GL_StartReconcDtTm == NullPastDate)
                {
                    Rcs.GL_StartReconcDtTm = DateTime.Now;
                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WRMCategory, WRMCycle);
                }

            }

            // 
            // DataGrid 
            // SHOW ALL IN DIFERENCE or unmatched 
            // OR DELAYED REPLENISH
            // 
            WithDate = false;
            int WMode = 1;

            if (WViewHistory == true)
            {

                Ta.ReadReplCyclesWithDifferForThisAtmGroup_For_Viewing(WOperator, WSignedId, WReconcGroupNo, WRMCycle);

                CurrentATMsTableCashReconciliation = Ta.TableCashReconciliation;

            }
            else
            {

                Am.ReadAtmsMainForAuthUserAndFillTableForBulk
                         (WOperator, WSignedId, NullPastDate, WithDate, 10, WReconcGroupNo);

                CurrentATMsTableCashReconciliation = Am.TableATMsMainSelected;

                I = 0;
                DateTime JustNow = DateTime.Now; // In order to have the same date for all
                while (I <= (Am.TableATMsMainSelected.Rows.Count - 1))
                {

                    TAtmNo = (string)Am.TableATMsMainSelected.Rows[I]["AtmNo"];
                    TSesNo = (int)Am.TableATMsMainSelected.Rows[I]["ReplCycleNo"];
                    TMaker = (string)Am.TableATMsMainSelected.Rows[I]["Maker"];
                    decimal GL_Diff = (decimal)Am.TableATMsMainSelected.Rows[I]["GL Diff"];

                    Ta.ReadSessionsStatusTraces(TAtmNo, TSesNo);

                    Ta.Recon1.StartReconc = true;
                    Ta.Recon1.RecStartDtTm = JustNow;

                    Ta.UpdateSessionsStatusTraces(TAtmNo, TSesNo);

                    I++;
                }
            }

            dataGridViewMyATMS.DataSource = CurrentATMsTableCashReconciliation;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridViewMyATMS.Columns[0].Width = 90; // AtmNo
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridViewMyATMS.Sort(dataGridViewMyATMS.Columns[0], ListSortDirection.Ascending);

            dataGridViewMyATMS.Columns[1].Width = 80; // GL Open
            dataGridViewMyATMS.Columns[1].DefaultCellStyle = style;
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridViewMyATMS.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridViewMyATMS.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
            //dataGridViewMyATMS.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

            dataGridViewMyATMS.Columns[2].Width = 80; // GL At Repl
            dataGridViewMyATMS.Columns[2].DefaultCellStyle = style;
            dataGridViewMyATMS.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewMyATMS.Columns[3].Width = 80; // GL & Actions
            dataGridViewMyATMS.Columns[3].DefaultCellStyle = style;
            dataGridViewMyATMS.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewMyATMS.Columns[4].Width = 80; // Cash Unloaded
            dataGridViewMyATMS.Columns[4].DefaultCellStyle = style;
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewMyATMS.Columns[5].Width = 80; // GL Diff
            dataGridViewMyATMS.Columns[5].DefaultCellStyle = style;
            dataGridViewMyATMS.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridViewMyATMS.Columns[6].Width = 100; // Maker 
            dataGridViewMyATMS.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridViewMyATMS.Columns[7].Width = 80; // Authoriser
            dataGridViewMyATMS.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                MessageBox.Show("No ATMs to Reconcile");
                this.Close();
                return;
            }

        }

        // ROW ENTER FOR MY ATMS
        //string WPreviousAtmNo; // To correct the Bug 
        private void dataGridViewMyATMS_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            WAtmNo = (string)CurrentATMsTableCashReconciliation.Rows[e.RowIndex]["AtmNo"];
            WSesNo = (int)CurrentATMsTableCashReconciliation.Rows[e.RowIndex]["ReplCycleNo"];

            Ac.ReadAtm(WAtmNo); 
            if (Ac.CitId == "1000")
            {
                labelAtmOwner.Text = "Bank's ATM";
                buttonVewReplPlay.Enabled = true;
            }
            else
            {
                labelAtmOwner.Text = "This ATM is CIT's.." + Ac.CitId;
                buttonVewReplPlay.Enabled = false; 
            }
            
            if (WViewHistory == false)
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
            }
            else
            {
                // View 
                Am.Maker = "Maker Actions";
            }

            //WSesNo = Am.ReplCycleNo;

            // Set up Information for ATMs

            buttonUnMatched.Show();

            if (Am.Maker == "UnMatched")
            {

                panel4.Hide();

                labelExceptions.Hide();

                panelExceptions.Hide();

                buttonAuthor.Hide();

                labelBalances.Hide();
            }
            else
            {
                //buttonUnMatched.Hide();

                panel4.Show();
                labelExceptions.Show();
                panelExceptions.Show();

                labelBalances.Text = "Balances and Errors After " + Am.Maker + " is applied";

                WBankId = Am.BankId;

            }

            // Prepare Left 

            if (Am.Maker == "Maker Actions" || Am.Maker == "No Decision")
            {
                Process = 4;

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                textBox_GL_Open.Text = Na.Balances1.HostBal.ToString("#,##0.00");

                textBox_Cash_Unloaded.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

                textBoxAdjustedOfUnloaded.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

                textBoxDiffMachine.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");

                textBox_GL_Adj.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

                if (WOperator == "CRBAGRAA")
                {
                    textBox_GL_Diff.Text = (Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj)).ToString("#,##0.00");
                }
                else
                {
                    textBox_GL_Diff.Text = (Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj)).ToString("#,##0.00");
                }

                //
                //***************************************************************************

                textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

                if (WOperator == "CRBAGRAA")
                {
                    //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
                    if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj)) == 0 & Na.Balances1.ErrOutstanding == 0)
                    {

                        label12.Text = "ATM No : " + WAtmNo + " IS RECONCILED ";

                        Color Green = Color.Green;

                        label12.ForeColor = Green;

                    }
                    else
                    {
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT ATM";
                        }
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT BOTH";
                        }
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding > 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT BOTH";
                        }
                        if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT GL";
                        }
                        if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding != 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT ERRORS";
                        }

                        Color Red = Color.Red;

                        label12.ForeColor = Red;
                    }
                }
                else
                {
                    //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
                    if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj)) == 0 & Na.Balances1.ErrOutstanding == 0)
                    {

                        label12.Text = "ATM No : " + WAtmNo + " IS RECONCILED ";

                        Color Green = Color.Green;

                        label12.ForeColor = Green;

                    }
                    else
                    {
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT ATM";
                        }
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT BOTH";
                        }
                        if (Na.DiffAtAtmLevel == true & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding > 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT BOTH";
                        }
                        if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) != 0 & Na.Balances1.ErrOutstanding == 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT GL";
                        }
                        if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding != 0)
                        {
                            label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE AT ERRORS";
                        }

                        Color Red = Color.Red;

                        label12.ForeColor = Red;
                    }
                }



                if (NormalProcess)
                {
                    buttonCreateError.Show();
                }
                else
                {
                    buttonCreateError.Hide();
                    //buttonRedoExpress.Hide();
                    //buttonUndoExpress.Hide();
                }

                Ac.ReadAtm(WAtmNo);

                try
                {
                    // Show errors table 
                    //string filter = "AtmNo='" + WAtmNo + "' AND CurDes ='" + Na.Balances1.CurrNm
                    ////+ "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5)  AND SesNo<=" + WSesNo + " AND ( OpenErr=1  OR (OpenErr=0 AND ActionSes =" + WSesNo + "))  ";
                    //+ "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5)  AND SesNo<=" + WSesNo + "  ";

                    DateTime Last_Cut_Off_Date;
                    WPreRMCycle = 0;

                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                    Rjc.Find_GL_Cut_Off_Before_GivenDate(WOperator, Ta.SesDtTimeEnd.Date);
                    if (Rjc.RecordFound == true & Rjc.Counter == 0)
                    {
                        Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                        WPreRMCycle = Rjc.JobCycle;
                    }

                    string filter = /*"SELECT *"*/
                                    //+ " FROM [dbo].[ErrorsTable] "
                                    //+ " WHERE "
          " AtmNo ='" + WAtmNo + "'"
        + " AND SesNo <=" + WSesNo
        + " AND "
        + "("
        + " OpenErr = 1 AND ErrType = 5 " // GL Error 
        + " OR (OpenErr=1 AND DateTime < @DateTime )"
        + " OR (OpenErr=0 AND ActionRMCycle = " + WPreRMCycle + ")"
        + " OR (OpenErr=0 AND ActionRMCycle = " + WRMCycle + ")"
        + " )  ";
                    //+ " OR (OpenErr=0 AND ActionRMCycle > " + WRMCycle + " AND DateTime < @DateTime )" // To cover all from GL Balance at cut off till now
                    //+ " OR (OpenErr=0 AND ActionSesNo = " + WSesNo +")"

                    //Er.ReadErrorsAndFillTable(WOperator, WSignedId, filter);

                    Er.ReadErrorsAndFillTableFrom_Form52c(WOperator, WSignedId, filter, Ta.SesDtTimeEnd);

                    ShowErrorsGrid2();

                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();
                    //       MessageBox.Show(ex.ToString());
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }

                //------------------------------------------------------------
                // Authorization Part 
                //------------------------------------------------------------
                if (WAuthoriser || WRequestor)
                {
                    bool Reject = false;
                    Ap.GetMessageOne(WAtmNo, WSesNo, "ReconciliationBulk", WAuthoriser, WRequestor, Reject);
                    textBoxMsgBoard.Text = Ap.MessageOut;
                }


                //************************************************************
                //************************************************************

                SetScreen(); // For Autherisation AND NOTES 

                //************************************************************
                //************************************************************

            }

        }

        // ROW ENTER FOR TRACES
        //int WPreviousErrNo;
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            //textBox28.Text = rowSelected.Cells[0].Value.ToString();
            WErrNo = (int)rowSelected.Cells[0].Value;

            // Find Errors details 
            Er.ReadErrorsTableSpecific(WErrNo);

            if (WAtmNo == Er.AtmNo)
            {

                WUniqueRecordId = Er.UniqueRecordId;

                Mpa.ReadInPoolTransSpecificUniqueRecordId(WUniqueRecordId,1);

                if (Mpa.RecordFound == true)
                {

                    // NOTES START  

                    ShowNote_3(); 

                    //

                    if (Mpa.IsMatchingDone == true)
                    {
                        textBox5.Text = Mpa.MatchMask;
                    }
                    else
                    {
                        textBox5.Text = "No Matching yet";
                    }
                    checkBoxIsOwnCard.Show();
                    if (Mpa.TargetSystem >= 5)
                    {
                        checkBoxIsOwnCard.Checked = true;
                    }
                    else
                    {
                        checkBoxIsOwnCard.Checked = false;
                    }

                    linkLabelSourceRecords.Show();
                }
                else
                {
                    checkBoxIsOwnCard.Hide();

                    linkLabelSourceRecords.Hide();
                }

                WTraceNo = Er.TraceNo;

                WErrId = Er.ErrId;

                labelErrDesc.Text = Er.ErrDesc;

                textBox23.Text = Er.ErrAmount.ToString("#,##0.00");
                textBox24.Text = Er.CurDes.ToString();

                string ATMSuspence = "";
                string ATMCash = "";

                //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

                Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

                if (Acc.RecordFound == true)
                {
                    ATMSuspence = Acc.AccNo;
                }
                else
                {
                    MessageBox.Show("ATM Suspense Account Not Found");
                }

                Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Er.CurDes, "ATM Cash");
                if (Acc.RecordFound == true)
                {
                    ATMCash = Acc.AccNo;
                }
                else
                {
                    MessageBox.Show("ATM Cash Account Not Found");
                }

                if (Er.DrCust == true)
                {
                    textBox38.Text = " DEBIT CUSTOMER ";

                    textBox1.Text = Mpa.AccNumber;
                }
                if (Er.CrCust == true)
                {
                    textBox38.Text = " CREDIT CUSTOMER ";

                    textBox1.Text = Mpa.AccNumber;
                }
                if (Er.DrAtmCash == true)
                {
                    textBox39.Text = " DEBIT ATM CASH ";
                    textBox2.Text = ATMCash;
                }
                if (Er.CrAtmCash == true)
                {
                    textBox39.Text = " CREDIT ATM CASH ";
                    textBox2.Text = ATMCash;
                }

                if (Er.DrCust == false & Er.CrCust == false)
                {
                    if (Er.DrAtmCash == true)
                    {
                        textBox38.Text = " DEBIT ATM CASH ";
                        textBox1.Text = ATMCash;
                    }
                    if (Er.CrAtmCash == true)
                    {
                        textBox38.Text = " CREDIT ATM CASH ";
                        textBox1.Text = ATMCash;
                    }

                    if (Er.DrAtmSusp == true)
                    {
                        textBox39.Text = " DEBIT ATM SUSPENSE";
                        textBox2.Text = ATMSuspence;
                    }
                    if (Er.CrAtmSusp == true)
                    {
                        textBox39.Text = " CREDIT ATM SUSPENSE";
                        textBox2.Text = ATMSuspence;
                    }

                }

                if (Er.UnderAction == true)
                {
                    //  GENERIC 
                    radioSystemSuggest.Checked = true;
                }

                if (Er.DisputeAct)
                {
                    radioButtonMoveToDispute.Checked = true;
                    textBox38.Text = "N/A";
                    textBox39.Text = "N/A";
                }

                if (Er.ManualAct)
                {
                    radioButtonForceClose.Checked = true;
                    textBox38.Text = "N/A";
                    textBox39.Text = "N/A";
                }


                if (Er.UnderAction || Er.DisputeAct || Er.ManualAct)
                {
                    label3.Text = "Action.... taken!";

                    Color Green = Color.Green;

                    label3.ForeColor = Green;

                    pictureBox3.Hide(); // Take Action 

                    pictureBox2.Show(); // Done

                    buttonApplyAction.Hide();
                    buttonUndoAction.Show();
                }
                else
                {
                    radioSystemSuggest.Checked = false;
                    radioButtonMoveToDispute.Checked = false;

                    radioButtonDelete.Checked = false;
                    radioButtonForceClose.Checked = false;

                    label3.Text = "Action... Not taken yet!";

                    buttonApplyAction.Show();
                    buttonUndoAction.Hide();

                    Color Red = Color.Red;

                    label3.ForeColor = Red;

                    pictureBox3.Show(); // Take Action 

                    pictureBox2.Hide(); // Done

                }

                if (Er.UniqueRecordId > 0)
                {
                    Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
                    if (Dt.RecordFound == true)
                    {
                        labelDisputeId.Show();

                        labelDisputeId.Text = "Dispute Id :" + Dt.DisputeNumber.ToString();

                    }
                    else
                    {
                        labelDisputeId.Hide();
                    }
                }

            }

        }


        // DO ALL PRESENTER ERRORS
        // Set All presenter errors to Under Action 
        private void buttonRedoExpress_Click(object sender, EventArgs e)
        {

            // Update Error Table
            // Turn Under Action to True

            WithDate = false;

            Am.ReadAtmsMainForAuthUserAndDoAllPresenter
                                        (WOperator, WSignedId, NullPastDate, WithDate);

            //TotalSelected = 0;

            //ExpressTotal = 0;

            MessageBox.Show("Total Presenter Read : " + Am.TotalSelected.ToString() + Environment.NewLine
                            + "Total Action Taken" + Am.ExpressTotal.ToString()
                            );

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }

        // Undo ALL PRESENTER ERRORS
        private void buttonUndoExpress_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;


            WithDate = false;

            Am.ReadAtmsMainForAuthUserAnd_Un_Do_AllPresenter
                           (WOperator, WSignedId, NullPastDate, WithDate);

            Am.ReadAtmsMainSpecific(WAtmNo);
            Am.Maker = "No Decision";
            Am.UpdateAtmsMain(WAtmNo);

            // Update Error Table
            // Turn Under Action to False 

            WUnderAction = false;

            Er.UpdatePresenterErrorsWithChangeUnderAction(WOperator, WAtmNo, WSesNo, WUnderAction);

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }

        // Transaction Matching Path 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            if (WErrId < 200)
            {
                NForm75 = new Form75(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, WUniqueRecordId);
                NForm75.FormClosed += NForm75_FormClosed;
                NForm75.ShowDialog();
            }
            else
            {
                MessageBox.Show("This is an error created by system and do not have transactions");
            }
        }

        void NForm75_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
        }

        //==========================================================
        // Request Authorization
        //===========================================================
        private void buttonAuthor_Click(object sender, EventArgs e)
        {

            if (Am.MakerActionsTotal == 0 & Am.ExpressTotal == 0)
            {
                MessageBox.Show("There is nothing to authorise! ");
                return;
            }

            if (Am.NoDecisionTotal > 0)
            {
                if (MessageBox.Show("Warning: Outstanding items with no decision from Maker." + ". DoYou want to proceed?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                {
                    // Proceed
                }
                else
                {
                    return;
                }
            }

            // Validate if outstanding UN-RECONCILED 

            Am.ReadAtmsMainForAuthUserAndFillTableForBulk
                               (WOperator, WSignedId, NullPastDate, WithDate, 10, WReconcGroupNo);

            //ErrorsOutstandingAllAtms = 0;
            //
            I = 0;

            while (I <= (Am.TableATMsMainSelected.Rows.Count - 1))
            {

                TAtmNo = (string)Am.TableATMsMainSelected.Rows[I]["AtmNo"];
                TSesNo = (int)Am.TableATMsMainSelected.Rows[I]["ReplCycleNo"];
                TMaker = (string)Am.TableATMsMainSelected.Rows[I]["Maker"];
                decimal GL_Diff = (decimal)Am.TableATMsMainSelected.Rows[I]["GL Diff"];

                Na.ReadSessionsNotesAndValues(TAtmNo, TSesNo, 4);

                if (Na.Balances1.ErrOutstanding == 0 & GL_Diff == 0)
                {
                    // OK 
                }
                else
                {
                    // There is unreconciled 
                    MessageBox.Show("ATM : " + TAtmNo + " Not Reconciled yet. ");
                    return;
                }

                I++;
            }

            int WTranNo = 0;
            Form110 NForm110;

            string WOrigin = "ReconciliationBulk";
            //OriginId
            // "01" OurATMS-Matching
            // "02" BancNet Matching                               
            // "03" OurATMS-Reconc
            // "04" OurATMS-Repl
            // "05" Settlement
            // "07" Disputes 
            // "08" Settlement 

            //int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            //NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber, "Normal");
            //NForm110.FormClosed += NForm110_FormClosed;
            //NForm110.ShowDialog();
        }

        void NForm110_FormClosed(object sender, FormClosedEventArgs e)
        {
            //************************************************************
            //************************************************************
            // AUTHOR PART
            //************************************************************
            WViewFunction = false;
            WAuthoriser = false;
            WRequestor = false;
            NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "ReconciliationBulk");

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";

                buttonUndoExpress.Hide();
                buttonRedoExpress.Hide();

            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";

                buttonUndoExpress.Hide();
                buttonRedoExpress.Hide();


            }

            if (NormalProcess) // Orginator has deleted authoriser 
            {
                textBoxMsgBoard.Text = "Please make authorisation ";

            }


            SetScreen();
        }
        //*************************************
        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {


            // NOTES Per ATM
            string Order = "Descending";
            string WParameter4 = "Reconc closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";


            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    //label1.Hide();

                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                    //buttonNotes3.Hide();
                    //labelNumberNotes3.Hide();
                }
                else
                {
                    buttonNotes2.Show();
                    labelNumberNotes2.Show();
                    //buttonNotes3.Show();
                    //labelNumberNotes3.Show();

                }
            }
            else
            {
                buttonNotes2.Show();
                labelNumberNotes2.Show();
                buttonNotes3.Show();
                labelNumberNotes3.Show();
            }


            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "ReconciliationBulk");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        //  guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ProcessNo = 2; // Return to stage 2  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 
                        //WAuthNegative = true;

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
                        buttonNotes3.Show();
                        labelNumberNotes3.Show();
                    }
                    else
                    {
                        NormalProcess = false;
                    }
                }
            }

            ShowAuthorisationInfo();

        }
        //************************************************* 
        // Show Authorization information 
        //
        private void ShowAuthorisationInfo()
        {

            Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(WAtmNo, WSesNo, "ReconciliationBulk");
            if ((Ap.RecordFound == true & Ap.OpenRecord == true)
                   || (Ap.RecordFound == true & Ap.OpenRecord == false & Ap.Stage == 5))
            {
                labelDtAuthRequest.Text = "Date of Request : " + Ap.DateOriginated.ToString();

                if (Ap.Stage == 1) StageDescr = "Authoriser Not Available yet.";
                if (Ap.Stage == 2) StageDescr = "Authoriser got the message. He will take action.";
                if (Ap.Stage == 3) StageDescr = "Authoriser took action. Requestor must act. ";
                if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                {
                    StageDescr = "Authorization accepted. Ready for Finish";
                }
                if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                {
                    StageDescr = "Authorization REJECTED. ";
                    Color Red = Color.Red;
                    labelAuthStatus.ForeColor = Red;
                }

                if (Ap.Stage == 5) StageDescr = "Authorisation process is completed";

                labelAuthStatus.Text = "Current Status : " + StageDescr;

                Us.ReadUsersRecord(Ap.Requestor);
                labelRequestor.Text = "Requestor : " + Us.UserName;

                Us.ReadUsersRecord(Ap.Authoriser);
                labelAuthoriser.Text = "Authoriser : " + Us.UserName;

                textBoxComment.Text = Ap.AuthComment;

                WAuthorSeqNumber = Ap.SeqNumber;

                labelAuthHeading.Show();
                labelAuthHeading.Text = "AUTHORISER's SECTION FOR ATM : " + WAtmNo;
                panelAuthor.Show();

                if (WViewFunction == true) // For View only 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();

                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();


                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WAuthoriser == true & (Ap.Stage == 2 || Ap.Stage == 3)) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();


                    buttonAuthorise.Show();
                    buttonReject.Show();
                    textBoxComment.ReadOnly = false;
                }

                if (WAuthoriser == true & Ap.Stage == 4) // For Authoriser from author management 
                {
                    // Main buttons
                    buttonAuthor.Hide();
                    buttonRefresh.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();

                    buttonAuthorise.Hide();
                    buttonReject.Hide();
                    textBoxComment.ReadOnly = true;
                }

                if (WRequestor == true || NormalProcess) // For Requestor from author management 
                {
                    if (Ap.Stage < 3) // Not authorise yet
                    {
                        // Main buttons
                        buttonAuthor.Hide();
                        buttonRefresh.Show();

                        buttonRedoExpress.Hide();
                        buttonUndoExpress.Hide();

                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES") // Authorised and accepted
                    {
                        // Main buttons
                        buttonAuthor.Hide();
                        buttonRefresh.Hide();

                        buttonRedoExpress.Hide();
                        buttonUndoExpress.Hide();

                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;

                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO") // Authorised but rejected
                    {
                        // Main buttons
                        buttonAuthor.Show();
                        buttonRefresh.Hide();

                        // Authoriser
                        buttonAuthorise.Hide();
                        buttonReject.Hide();
                        textBoxComment.ReadOnly = true;
                    }
                }
            }
            else
            {
                // THIS IS THE NORMAL ... You do not show the AUTH box 
                if (NormalProcess & WRequestor == false) // Normal Reconciliation 
                {
                    // Do not show Authorisation Section this will be shown after authorisation 
                    labelAuthHeading.Hide();
                    panelAuthor.Hide();
                }
            }

        }

        // Update Authorization Record 
        private void UpdateAuthorRecord(string InDecision)
        {

            Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
            if (Ap.OpenRecord == true)
            {
                Ap.AuthDecision = InDecision;
                if (textBoxComment.TextLength > 0)
                {
                    Ap.AuthComment = textBoxComment.Text;
                }
                Ap.DateAuthorised = DateTime.Now;
                Ap.Stage = 3;

                Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);

                if (InDecision == "YES")
                {
                    MessageBox.Show("Authorization ACCEPTED! by : " + labelAuthoriser.Text);
                    //   this.Dispose();
                }
                if (InDecision == "NO")
                {
                    MessageBox.Show("Authorization REJECTED! by : " + labelAuthoriser.Text);
                    //   this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Authorization record is not open. Requestor has closed it.");
                return;
            }
        }

        // Authorise 
        private void buttonAuthorise_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            if (Am.Maker == "UnMatched")
            {
                MessageBox.Show("Not Allowed to Authorise Unmatched Maker Status.");
                return;
            }

            UpdateAuthorRecord("YES");

            // Read Line and update 

            Am.ReadAtmsMainSpecific(WAtmNo);

            Am.Authoriser = "Authorised";

            Am.UpdateAtmsMain(WAtmNo);

            textBoxMsgBoard.Text = "Authorisation Made - Accepted for this ATM";

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }

        // REJECT AUTH
        private void buttonReject_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            if (textBoxComment.TextLength < 5)
            {
                MessageBox.Show("Please input comment to explain rejection");
                return;
            }
            UpdateAuthorRecord("NO");

            // Read Line and update 

            Am.ReadAtmsMainSpecific(WAtmNo);

            Am.Authoriser = "Rejected";


            Am.UpdateAtmsMain(WAtmNo);

            textBoxMsgBoard.Text = "Authorisation Made - Rejected ";

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
        }
        // Author History 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WSesNo, WDisputeNo, WDisputeTranNo, WRMCategory, WRMCycle);
            NForm112.FormClosed += NForm112_FormClosed;
            NForm112.ShowDialog();
        }

        void NForm112_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
        }

        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Reconc closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            SetScreen();
        }


        // DECLARATIONS FOR FINISH 

        bool ReconciliationAuthorDone;

        string TAtmNo;
        int TSesNo;
        string TMaker;

        //
        // Finish Bulk for Originator and Authoriser 
        //
        int ActionsOnErrorsAllAtms;
        //int ErrorsOutstandingAllAtms;
        private void buttonFinishBulk_Click(object sender, EventArgs e)
        {

            if (WAuthoriser == true & Am.AuthNoDecisTotal == 0) // Coming from authoriser and authoriser done  
            {
                //UpdateAuthorRecord("YES");

                this.Close();
                return;
            }

            if (Usi.ProcessNo == 56 & Am.AuthNoDecisTotal > 0) // Coming from Requestor after author request was sent.  
            {
                this.Dispose();
                return;
            }

            if (Usi.ProcessNo == 54) // Coming from View only
            {
                this.Dispose();
                return;
            }

            if (WAuthoriser == true & Am.AuthNoDecisTotal > 0) // Cancel by authoriser without making authorisation.
            {
                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }
            }

            if (Usi.ProcessNo == 5 & Am.AuthNoDecisTotal == 0) // Cancel by originator without request for authorisation 
            {

                if (MessageBox.Show("Warning: Authorisation Not Done " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                     == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (Usi.ProcessNo == 5 & Am.AuthNoDecisTotal > 0) // Cancel by originator without request for authorisation 
            {

                if (MessageBox.Show("Warning: Authorisation outstanding " + ". Do You want to abort?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                     == DialogResult.Yes)
                {
                    this.Dispose();
                    return;
                }
                else
                {
                    return;
                }

            }

            if (Usi.ProcessNo == 5 & UnMatchedFound == true) // 
            {
                this.Close();
                return;
            }
            // LEAVE HERE DO NOT MOVE IT 
            // READ TO REFRESH TABLE 
            Am.ReadAtmsMainForAuthUserAndFillTableForBulk
                        (WOperator, WSignedId, NullPastDate, WithDate, 10, WReconcGroupNo);

            // create a connection object
            using (var scope = new System.Transactions.TransactionScope())
                try
                {  //
                   //

                    ActionsOnErrorsAllAtms = 0;
                    //ErrorsOutstandingAllAtms = 0;
                    //
                    I = 0;

                    while (I <= (Am.TableATMsMainSelected.Rows.Count - 1))
                    {

                        TAtmNo = (string)Am.TableATMsMainSelected.Rows[I]["AtmNo"];
                        TSesNo = (int)Am.TableATMsMainSelected.Rows[I]["ReplCycleNo"];
                        TMaker = (string)Am.TableATMsMainSelected.Rows[I]["Maker"];

                        ReconciliationAuthorDone = false;

                        Ap.ReadAuthorizationForReplenishmentReconcSpecificForAtm(TAtmNo, TSesNo, "ReconciliationBulk");
                        if (Ap.RecordFound == true)
                        {

                            if ((Ap.Stage == 3 || Ap.Stage == 4) & Ap.AuthDecision == "YES")
                            {
                                ReconciliationAuthorDone = true;
                            }
                            else
                            {
                                ReconciliationAuthorDone = false;

                            }
                        }

                        //
                        //**************************************************************************
                        //**************************************************************************
                        // IF YOU CAME TILL HERE THEN RECONCILIATION WILL BE COMPLETED WITH UPDATING 
                        //**************************************************************************

                        if (ReconciliationAuthorDone == true)
                        {
                            // Update authorisation record  

                            if (Ap.Stage == 4)
                            {
                                // Update stage 
                                //
                                Ap.Stage = 5;
                                Ap.OpenRecord = false;

                                Ap.UpdateAuthorisationRecord(Ap.SeqNumber);
                                if (Ap.RMCategory != "")
                                {
                                    WRMCategory = Ap.RMCategory;
                                    WRMCycle = Ap.RMCycle;
                                }
                            }
                            //
                            // THE BELOW METHOD CREATES THE TRANSACTIONS TO BE POSTED 
                            string WOriginId = "01";
                            string WOriginName = "OurATMs-" + WRMCategory;

                            Er.ReadAllErrorsTableForPostingTrans(WOperator, WRMCategory, TAtmNo, WSignedId, 
                                Ap.Authoriser, WRMCycle, TMaker,
                                WOriginId, WOriginName
                                );

                            ActionsOnErrorsAllAtms = ActionsOnErrorsAllAtms + Er.TotalSelected;
                            // Update Traces at Finish
                            //
                            // UPDATE TRACES AND ATMs Main WITH FINISH RECONC .... different Info
                            // THIS IS A METHOD THAT UPDATES DIRECTLY
                            // NO NEED TO READ Ta

                            Ta.UpdateTracesFinishReconc_From_Form52c(TAtmNo, TSesNo, WSignedId, Ap.Authoriser, WRMCycle);

                            // READ Ta to see if differences AND SEND EMAIL ALERTS

                            Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WRMCategory, WRMCycle);

                            //Rcs.GL_Original_Atms_Cash_Diff = Rcs.GL_Original_Atms_Cash_Diff + 1;

                            Rcs.GL_Remain_Atms_Cash_Diff = Rcs.GL_Remain_Atms_Cash_Diff - 1;

                            Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WRMCategory, WRMCycle);
                        }
                        else
                        {
                        }

                        I++;
                    }
                    // UPDATE COMPLETION 

                    Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WRMCategory, WRMCycle);

                    Rcs.GL_EndReconcDtTm = DateTime.Now;

                    Rcs.UpdateReconcCategorySession_ForAtms_Cash_Diff(WRMCategory, WRMCycle);


                    //MessageBox.Show("No transactions to be posted");
                    Form2 MessageForm = new Form2("Reconciliation Workflow for ATMs has finished." + Environment.NewLine
                                                   + "Transactions to be posted were created as a result" + Environment.NewLine
                                                   );
                    MessageForm.ShowDialog();

                    scope.Complete();

                    this.Close();

                }
                catch (Exception ex)
                {
                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                        + " . Application will be aborted! Call controller to take care. ");
                    //Environment.Exit(0);
                }
                finally
                {
                    scope.Dispose();
                }

        }

        // 
        void NForm174_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
        }
        // Show UnMatched
        private void buttonUnMatched_Click(object sender, EventArgs e)
        {

            //
            // Check if outstanding unmatched for this ATM 
            //
            string SearchingStringLeft = " WHERE Operator ='" + WOperator + "'"
                          //+ "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                          + " AND TerminalId ='" + WAtmNo + "' AND (ActionType = '01' OR ActionType = '02')  ";

            string WSortValue = "";
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable(WOperator, WSignedId, 1, SearchingStringLeft, WSortValue, NullPastDate, NullPastDate,2);

            if (Mpa.MatchingMasterDataTableATMs.Rows.Count == 0)
            {
                MessageBox.Show("No Records to show. ");
                return;
            }

            Form78b NForm78b;

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;
            string WHeader = "SELECTED TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Mpa.MatchingMasterDataTableATMs, WHeader, "Form52b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog();
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            WRowIndexRight = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRowIndexRight].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));

        }
        // View Replenishment PlayBack 
        int SaveProcessNo;
        private void buttonVewReplPlay_Click(object sender, EventArgs e)
        {
            Form51 NForm51;

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            SaveProcessNo = Usi.ProcessNo;
            Usi.ProcessNo = 54; // View only for replenishment already done  
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm51.FormClosed += NForm51_FormClosed;
            NForm51.ShowDialog();

        }
        void NForm51_FormClosed(object sender, FormClosedEventArgs e)
        {
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = SaveProcessNo; // Return to original 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

        }

        //******************
        // SHOW GRID
        //******************
        private void ShowErrorsGrid2()
        {
            dataGridView2.DataSource = Er.ErrorsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //MessageBox.Show("No available errors with this search! ");

                labelExceptions.Hide();
                panelExceptions.Hide();

                return;
            }
            else
            {
                labelExceptions.Show();
                panelExceptions.Show();
            }

            dataGridView2.Columns[0].Width = 40; // ExcNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 100; // Desc
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 80; //  Card
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView2.Columns[3].Width = 50; // Ccy
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[4].Width = 80; // Amount
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 50; // NeedAction
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[6].Width = 50; // UnderAction 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[7].Width = 50; // ManualAct

            dataGridView2.Columns[7].Width = 90; // DateTime

            dataGridView2.Columns[8].Width = 100; // TransDescr

        

            //dataGridView1.Rows[WRowIndex].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }
        // Balance Sheet of GL 
        private void linkLabelBalanceSheet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("This will show GL as per last nights which is (-) " + Environment.NewLine
                + " + Txns till morning " + Environment.NewLine
                + " (+) or (-) Txns due to reconciliation " + Environment.NewLine
                + " (-) Txns due to presented errors " + Environment.NewLine
                + " (=) to Adjusted GL which should be equal to Counted Money " + Environment.NewLine
                + " If Not Equal then move differences to Dispute OR Profit and loss" + Environment.NewLine
                );
        }

        private void labelNumberNotes2_Click(object sender, EventArgs e)
        {

        }
        // Create Error to cover difference in GL 
        private void buttonCreateError_Click(object sender, EventArgs e)
        {
            Am.ReadAtmsMainSpecific(WAtmNo);
            if (Am.ProcessMode == -2)
            {
                MessageBox.Show("This ATm is not active!");
                return;
            }

            if (Am.Maker == "Express")
            {
                MessageBox.Show("Undo Express Before you proceed!");
                return;
            }

            // CHECK THAT ALL ERRORS UNDER ACTION BEFORE YOU PROCEED 

            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 



            // GO TO RECONCILIATION 
            // UPDATE INTENTED FUNCTION 

            // UPDATE INTENTED FUNCTION 

            //textBox_GL_Diff.Text = (Na.Balances1.CountedBal - (-Na.Balances1.HostBalAdj)).ToString("#,##0.00");

            if (Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj) != 0)
            {

                decimal AmountForSuspense = Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj);

                NForm14a = new Form14a(WSignedId, WRMCategory, WRMCycle, Er.UniqueRecordId, WAtmNo, WSesNo, WOperator, AmountForSuspense, Am.GL_CurrNm1);

                NForm14a.FormClosed += NForm14_FormClosed;
                NForm14a.Show();
            }
            else MessageBox.Show("NO AMOUNT FOR SUSPENSE", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        // FORM14 HAS CREATED THE SUSPENSE ERROR 
        void NForm14_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (NForm14a.MetaNumber > 0)

            {
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.Maker = "Maker Actions";

                Am.UpdateAtmsMain(WAtmNo);

                WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

                int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

                Form52b_Load(this, new EventArgs());

                dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
                dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

                dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
            }

        }
        // Apply Action
        private void buttonApplyAction_Click(object sender, EventArgs e)
        {
            if (radioSystemSuggest.Checked == false & radioButtonMoveToDispute.Checked == false
                & radioButtonDelete.Checked == false & radioButtonForceClose.Checked == false)
            {
                MessageBox.Show(" IF YOU WANT TO ACT PRESS A SELECTION BUTTON");
                return;
            }

            Er.ReadErrorsTableSpecific(WErrNo);

            if (radioButtonDelete.Checked)
            {
                if (Er.ErrId != 598)
                {
                    MessageBox.Show("YOU ARE NOT ALLOWED TO DELETE THIS ERROR TYPE!");
                    return;
                }

                if (MessageBox.Show("Warning:  " + "Do You want to Delete This Error?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
                {
                    // YES 
                    Er.DeleteErrorRecordByErrNo(WErrNo);


                    // If No Error Under Action then change Maker 
                    Er.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

                    if (Er.ErrUnderAction == 0 & Er.ErrUnderManualAction == 0)
                    {
                        Am.ReadAtmsMainSpecific(WAtmNo);
                        Am.Maker = "No Decision";
                        Am.UpdateAtmsMain(WAtmNo);
                    }

                    WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

                    int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

                    Form52b_Load(this, new EventArgs());

                    dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
                    dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

                    dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

                    return;
                }
                else
                {
                    // NO 
                    return;
                }
            }

            if (Er.UnderAction == true)
            {
                MessageBox.Show(" Action Was already taken. UNDO first and Then Take a new action ");
                return;
            }
            //
            // Do as system suggest 
            //
            if (radioSystemSuggest.Checked)
            {
                // check Dispute 

                if (Er.ErrId == 55) // Presenter Error
                {

                    if (textBox5.Text == "111" || textBox5.Text == "11")
                    {
                        // This is valid 
                    }
                    else
                    {
                        // This is not valid 
                        MessageBox.Show("Transactions do not matched." + Environment.NewLine
                                  + "You cannot proceed."
                                   );
                        return;
                    }

                    //
                    //
                    MessageBox.Show("The system had checked that Presenter Error = GL_Difference." + Environment.NewLine
                                   + "Customer will be credited and ATM Cash will be debited."
                                    );
                }


                // Update Error Table
                // 
                Er.OpenErr = true;
                Er.UnderAction = true;

                Er.ManualAct = false;

                Er.ActionDtTm = DateTime.Now;
                Er.ActionSesNo = WSesNo;
                Er.ActionRMCycle = WRMCycle;

                Er.UserComment = "As suggested by system";

                Er.UpdateErrorsTableSpecific(WErrNo); // Update errors              

                if (Am.Maker == "No Decision")
                {
                    Am.ReadAtmsMainSpecific(WAtmNo);
                    Am.Maker = "Maker Actions";
                    Am.UpdateAtmsMain(WAtmNo);
                }

                int WProcess = 4;

                Na.ReadSessionsNotesAndValues(Am.AtmNo, Am.ReplCycleNo, WProcess);
                if (WOperator == "CRBAGRAA")
                {
                    if ((Na.Balances1.CountedBal - (Na.Balances1.HostBalAdj) == 0) & Na.Balances1.ErrOutstanding == 0)
                    {

                        // This is valid

                        MessageBox.Show("This Action Brought this ATM Into Reconciliation!" + Environment.NewLine
                            + "NO GL Difference! " + Environment.NewLine
                            + "NO Outstanding Errors! ");

                    }
                }
                else
                {
                    if ((Na.Balances1.CountedBal - (-Na.Balances1.HostBalAdj) == 0) & Na.Balances1.ErrOutstanding == 0)
                    {

                        // This is valid

                        MessageBox.Show("This Action Brought this ATM Into Reconciliation!" + Environment.NewLine
                            + "NO GL Difference! " + Environment.NewLine
                            + "NO Outstanding Errors! ");

                    }
                }


                WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

                int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

                WRowIndexRight = dataGridView2.SelectedRows[0].Index;

                Form52b_Load(this, new EventArgs());

                dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
                dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

                dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

                // SET ROW Selection POSITIONING 
                dataGridView2.Rows[WRowIndexRight].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));


            }
            else
            {
                // FORCE TO CLOSE 
                if (radioButtonForceClose.Checked == true)
                {
                    // Update Error Table
                    // 

                    Er.ByWhom = WSignedId;
                    Er.ActionDtTm = DateTime.Now;
                    Er.ActionSesNo = WSesNo;
                    Er.ActionRMCycle = WRMCycle;
                    Er.OpenErr = false;
                    Er.UnderAction = false;

                    Er.ManualAct = true;

                    Er.UserComment = "This presenter error will not be handled";

                    Er.UpdateErrorsTableSpecific(WErrNo); // Update errors  

                    if (Am.Maker == "No Decision")
                    {
                        Am.ReadAtmsMainSpecific(WAtmNo);
                        Am.Maker = "Maker Actions";
                        Am.UpdateAtmsMain(WAtmNo);
                    }

                    int WProcess = 4;

                    Na.ReadSessionsNotesAndValues(Am.AtmNo, Am.ReplCycleNo, WProcess);

                    if ((Na.Balances1.CountedBal - (-Na.Balances1.HostBalAdj) == 0) & Na.Balances1.ErrOutstanding == 0)
                    {

                        // This is valid

                        MessageBox.Show("This Action Brought this ATM Into Reconciliation!" + Environment.NewLine
                            + "NO GL Difference! " + Environment.NewLine
                            + "NO Outstanding Errors! ");

                    }

                    WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

                    int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

                    WRowIndexRight = dataGridView2.SelectedRows[0].Index;

                    Form52b_Load(this, new EventArgs());

                    dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
                    dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

                    dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

                    // SET ROW Selection POSITIONING 
                    dataGridView2.Rows[WRowIndexRight].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexRight));
                }
                else
                {
                    // Move it to dispute
                    // Open a Dispute
                    // DR/CR ATM Cash
                    // CR/DR ATMs DISPUTE
                    // AT THE END OF CYCLE CREATE THE TRANSACTIONS AND THE DISPUTE AUTOMATICALLY.   
                    if (radioButtonMoveToDispute.Checked == true)
                    {

                        //NeedAction = (bool)rdr["NeedAction"];
                        //OpenErr = (bool)rdr["OpenErr"];
                        //FullCard = (bool)rdr["FullCard"];

                        //UnderAction = (bool)rdr["UnderAction"];
                        //DisputeAct = (bool)rdr["DisputeAct"];
                        //ManualAct = (bool)rdr["ManualAct"];

                        MessageBox.Show(
                                "A Dispute will be created" + Environment.NewLine
                              + "The ATM cash will be affected accordingly (DR or CR)" + Environment.NewLine
                              + "The Dispute Account for this ATM(or a general Dispute account)" + Environment.NewLine
                              + "Will be affected equivalently "
                                );

                        Form5 NForm5;
                        if (Er.ErrAmount == 0)
                        {
                            MessageBox.Show("No difference to be disputed");
                            return;
                        }
                        int From = 5; // From GL Reconciliation 
                        NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Er.CardNo, Er.UniqueRecordId, 0, 0, "", From, "ATM");
                        NForm5.FormClosed += NForm5_FormClosed;
                        NForm5.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Develop program." + Environment.NewLine
                                                                   + "");
                    }
                }
            }

        }
        void NForm5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dt.ReadDisputeTranByUniqueRecordId(Er.UniqueRecordId);
            if (Dt.RecordFound == true)
            {
                labelDisputeId.Show();
                textBoxDisputeId.Show();
                //buttonMoveToDispute.Hide();
                textBoxDisputeId.Text = Dt.DisputeNumber.ToString();
            }
            else
            {
                labelDisputeId.Hide();
                textBoxDisputeId.Hide();
                //buttonMoveToDispute.Show();
            }
        }
        // UNDO ACTION 
        private void buttonUndoAction_Click(object sender, EventArgs e)
        {
            // Update Error Table
            // 
            Er.OpenErr = true;
            Er.UnderAction = false;
            Er.ActionSesNo = 0;
            Er.ActionRMCycle = 0;
            Er.ManualAct = false;

            Er.UserComment = "";

            Er.UpdateErrorsTableSpecific(WErrNo); // Update errors 

            // If No Error Under Action then change Maker 
            Er.ReadAllErrorsTableForCounterReplCycle(WOperator, WAtmNo, WSesNo);

            if (Er.ErrUnderAction == 0 & Er.ErrUnderManualAction == 0)
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
                Am.Maker = "No Decision";
                Am.UpdateAtmsMain(WAtmNo);
            }

            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            int WRow = dataGridView2.SelectedRows[0].Index;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

            // SET ROW Selection POSITIONING 
            dataGridView2.Rows[WRow].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
        }
        // Show Note 3
        private void ShowNote_3()
        {
            // NOTES START  
            string Order = "Descending";
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
          
        //
        //
        // Notes 3 
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Mpa.UniqueRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            SetScreen();
        }


        // SHOW ERROR DETAILS 
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form24 NForm24;
            bool Replenishment = true;
            int ErrNo = Mpa.MetaExceptionNo;
            string SearchFilter = "ErrNo =" + ErrNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, Mpa.TerminalId, Mpa.ReplCycleNo, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }
        // Mouse Down
        //Code Snippet

        //Point PanelMouseDownLocation;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left) PanelMouseDownLocation = e.Location;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            //panel1.Left += e.X - PanelMouseDownLocation.X;

            //panel1.Top += e.Y - PanelMouseDownLocation.Y;
        }

        private void container_Scroll(object sender, ScrollEventArgs e)
        {
            //    Point pos = new Point(panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);
            //    if (e.Type == ScrollEventType.ThumbTrack)
            //    {
            //        if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            //            pos.X = e.NewValue;
            //        else
            //            pos.Y = e.NewValue;


            //        panel1.AutoScrollPosition = pos;
            //        panel2.Location = pos;
            //    }
        }
        // Print ALL Actions 
        private void button3_Click(object sender, EventArgs e)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport70(WSignedId);

            int I = 0;

            I = 0;

            while (I <= (CurrentATMsTableCashReconciliation.Rows.Count - 1))
            {

                TAtmNo = (string)CurrentATMsTableCashReconciliation.Rows[I]["AtmNo"];
                TSesNo = (int)CurrentATMsTableCashReconciliation.Rows[I]["ReplCycleNo"];

                string filter = "AtmNo='" + TAtmNo + "' AND CurDes ='" + Na.Balances1.CurrNm
                   + "'" + " AND (ErrType = 1  OR ErrType = 5)  AND SesNo<=" + TSesNo + " AND ( OpenErr=1  OR (OpenErr=0 AND ActionRMCycle =" + WRMCycle + "))  ";

                Er.ReadErrorsAndFillTableForReport(WOperator, WSignedId, filter);

                Er.InsertReport(WOperator, WSignedId, Er.ErrorsTableReport);

                I++; // Read Next entry of the table 

            }


            if (CurrentATMsTableCashReconciliation.Rows.Count > 0)
            {
                string P1 = "Actions For Cash Reconciliation For ATMs Group: " + WRMCategory;
                string P2 = "";
                string P3 = "";
                if (ViewWorkFlow == true || WRequestor || WAuthoriser)
                {
                    Us.ReadUsersRecord(Ap.Requestor);
                    P2 = Us.UserName;
                    Us.ReadUsersRecord(Ap.Authoriser);
                    P3 = Us.UserName;
                }

                if (NormalProcess == true)
                {
                    Us.ReadUsersRecord(WSignedId);
                    P2 = Us.UserName;
                    P3 = "N/A";
                }

                string P4 = WOperator;
                string P5 = WSignedId;

                Form56R70 ReportATMS70 = new Form56R70(P1, P2, P3, P4, P5);
                ReportATMS70.Show();
            }

        }
        // SOURCE RECORDS

        private void linkLabelSourceRecords_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78d_AllFiles NForm78d_AllFiles;

            if (WOperator == "CRBAGRAA")
            {
                // DEMO MODE

                NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                NForm78d_AllFiles.ShowDialog();
            }
            else
            {

                NForm78d_AllFiles = new Form78d_AllFiles(WOperator, WSignedId, Mpa.UniqueRecordId);

                NForm78d_AllFiles.ShowDialog();

            }
        }
    }
}

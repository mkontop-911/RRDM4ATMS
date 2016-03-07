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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form52b : Form
    {
       
        Form71 NForm71; // Reconciliation 

        Form75 NForm75; // Reconciliation 

        //Form62 NForm62; // Show Transactions 

        Form112 NForm112; // Auth History 

       
        string WAtmNo;
        int WSesNo;

        int WRowIndexLeft; 

     
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

        string WRMCategory;

        int WRMCycle; 

        int Process;

        int I ; 

        bool WUnderAction;

        bool RedoExpressPossible; 

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //int WRow1;
        int WRow2;

        int WErrNo; 

        int WTranNo ;

        int WErrId;

        int WTraceNo ;

        int ExpressMode;

        //decimal WTempAdj ;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        bool WithDate; 

        string Gridfilter;

        string WBankId;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMNotesBalances Na = new RRDMNotesBalances();

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMReconcCountersClass Rco = new RRDMReconcCountersClass();

        RRDMTurboReconcClass Tuc = new RRDMTurboReconcClass();

        RRDMRightToAccessAtm Ra = new RRDMRightToAccessAtm();

        RRDMAtmsClass Aa = new RRDMAtmsClass();

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMReconcCategoriesMatchingSessions Rs = new RRDMReconcCategoriesMatchingSessions();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

       
        RRDMCaseNotes Cn = new RRDMCaseNotes(); 

      
        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();

 
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        
        int WAction;
        public Form52b(string InSignedId, int InSignRecordNo, string InOperator, int Action)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAction = Action;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            UserId.Text = InSignedId ;


            // WAction codes 
            // 1 : Replenishment of ATMs  => This functionality has gone in Form152
            // 2 : Rconciliation of individual ATms => This functionality has gone in Form152
            // 3 : Captured Cards Management => This functionality has gone in Form152
            // 4 : Manage Deposits => This functionality has gone in Form152
            // 5 : Repl Group of Atms 
            // 7 : My transactions 
            // 8 : Calculate Money in => This functionality has gone in Form152
            // 11 : Group Reconciliation 
       
            if (WAction == 11) // Group Reconciliation and View 
            {
                //  UPDATE Authorised User field in ATMs Main
                //
                Ug.ReadUsersAccessAtmAndUpdateMain(WSignedId, 2, "Reconc");
                Ug.ReadUsersAccessAtmAndUpdateMain(WSignedId, 1, "Reconc");

               
                
                //UnMatchedRecords = 0; 

                labelStep1.Text = "Category ID EWB110. Reconc Of ATMs for cash vs GL and Presenter Errors.";

                //************************************************************
                //************************************************************
                // AUTHOR PART 
                // Comes from FORM112 = Author management 
                //************************************************************

                WViewFunction = false; // 54
                WAuthoriser = false;   // 55
                WRequestor = false;    // 56 
                NormalProcess = false;

                Us.ReadSignedActivityByKey(WSignRecordNo);

                WRMCategory = Us.WFieldChar1;
                WRMCycle = Us.WFieldNumeric1; 

                if (Us.ProcessNo == 54) WViewFunction = true;// ViewOnly 
                if (Us.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
                if (Us.ProcessNo == 56) WRequestor = true; // Requestor

                if (WViewFunction || WAuthoriser || WRequestor)
                {
                    NormalProcess = false;
                }
                else NormalProcess = true;

                if (WViewFunction == true) textBoxMsgBoard.Text = "View Only!";
                if (WAuthoriser == true) textBoxMsgBoard.Text = "Authoriser to examine one by one and take action.";
                if (WRequestor == true) textBoxMsgBoard.Text = "Wait for Authoriser.Use Refresh if you wish.";
                if (NormalProcess == true) textBoxMsgBoard.Text = "Examine one by one and take actions.";

                // START RECONCILIATION 
                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycle);
                Rs.StartReconcDtTm = DateTime.Now;
                Rs.UpdateCategRMCycleWithReconStartDate(WRMCategory, WRMCycle);

                ExpressMode = 1; // Without Updating of Ta only ATMs Main and Errors 

                // Do the Turbo / Express mode 

                Tuc.ReadUpdateSessionsStatusTracesForTurbo(WSignedId, WSignRecordNo, WOperator, ExpressMode);

                if (Tuc.TotalAtmsWithUnMatchedRecords > 0)
                {
                    UnMatchedFound = true; 
                }

            } 
        }
//
// LOAD
//
        private void Form52b_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet61.ErrorsTable' table. You can move, or remove it, as needed.
            //this.errorsTableTableAdapter.Fill(this.aTMSDataSet61.ErrorsTable);

            if (UnMatchedFound == true)
            {
                if (MessageBox.Show("Warning: There Are Outstanding UnMatched Records. Total = " + Tuc.TotalUnMatchedRecords.ToString() + ". DoYou want to proceed?  ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
            
            if (WAction == 11) // Group Reconciliation 
            {
                //Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'" + " AND ReconcDiff = 1";

                Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'" + " AND (ReconcDiff = 1 OR Maker = 'UnMatched') AND Maker <> 'UnReplenish'";

                //Gridfilter = "Operator ='" + WOperator + "' AND AuthUser ='" + WSignedId + "'" ;

                Rco.ReadSessionsStatusTracesCounters(WSignedId, WSignRecordNo, WOperator);
              
            // 
            //
            //  DataGrid 
            // SHOW ALL IN DIFERENCE or unmatched 
            // OR DELAYED REPLENISH 
            // 
            WithDate = false;

            Am.ReadAtmsMainForAuthUserAndFillTableForBulk(Gridfilter, NullPastDate, WithDate, WAction);  

            dataGridViewMyATMS.DataSource = Am.ATMsMainSelected.DefaultView;

            dataGridViewMyATMS.Columns[0].Width = 70; // AtmNo
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridViewMyATMS.Sort(dataGridViewMyATMS.Columns[0], ListSortDirection.Ascending);

            dataGridViewMyATMS.Columns[1].Width = 70; // ReplCycle
            dataGridViewMyATMS.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;
     
            dataGridViewMyATMS.Columns[2].Width = 140; // AtmName
            dataGridViewMyATMS.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridViewMyATMS.Columns[3].Width = 100; // RespBranch
            dataGridViewMyATMS.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridViewMyATMS.Columns[4].Width = 100; // Maker 
            dataGridViewMyATMS.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridViewMyATMS.Columns[5].Width = 80; // Authoriser

            if (dataGridViewMyATMS.Rows.Count == 0)
            {
                MessageBox.Show("No ATMs to Reconcile");
                this.Close();
                return;
            }

            }

        }

// ROW ENTER FOR MY ATMS
        //string WPreviousAtmNo; // To correct the Bug 
        private void dataGridViewMyATMS_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridViewMyATMS.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

            Am.ReadAtmsMainSpecific(WAtmNo);

            WSesNo = Am.ReplCycleNo;

            //--------------------------------------------------------------
            // VAlidation Part for Replenishment done 
            //--------------------------------------------------------------

   
            //------------------------------------------------------------
            // Authorization Part 
            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageOne(WAtmNo, WSesNo, "ReconciliationBulk", WAuthoriser, WRequestor, Reject);
                textBoxMsgBoard.Text = Ap.MessageOut;
            }
            // CHECK WHETHER THE ATM CAN BE DONE IN EXPRESS MODE AND HIDE EXPRESS BUTTONS 
            WUnderAction = true;

            Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

            Process = 5;

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

            //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

            //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
            if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
            {

                label12.Text = "ATM No :" + WAtmNo + " Reconciles ";
                RedoExpressPossible = true; 
            }
            else
            {
                RedoExpressPossible = false; 

                WUnderAction = false;

                Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

            }


            //************************************************************
            //************************************************************

            SetScreen(); // For Autherisation 

            //************************************************************
            //************************************************************

            // Set up Information for ATMs

            if (Am.Maker == "UnMatched")
            {
                buttonUnMatched.Show(); 

                panel4.Hide();

                label17.Hide();

                panel3.Hide(); 

                buttonManualReconc.Hide();
                buttonRedoExpress.Hide();
                buttonUndoExpress.Hide();

                buttonAuthor.Hide();

                labelBalances.Hide();
   
            }
            else
            {
                buttonUnMatched.Hide(); 

                panel4.Show();
                label17.Show();
                panel3.Show(); 

                labelBalances.Text = "Balances and Errors After " + Am.Maker + " is applied";

                WBankId = Am.BankId;
               
            }
           

            if (Am.Maker == "No Decision")
            {
                ////////
                WUnderAction = false;

                Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

                Process = 4;

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                textBox4.Text = Na.Balances1.HostBal.ToString("#,##0.00");

                textBox14.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

                textBox1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

                textBox2.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");


                textBox13.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

                textBox3.Text = (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj).ToString("#,##0.00");
                //
                //***************************************************************************

                textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

                //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
                if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
                {

                    label12.Text = "ATM No : " + WAtmNo + " IS RECONCILED ";

                    Color Green = Color.Green;

                    label12.ForeColor = Green;
                }
                else
                {
                    label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE ";

                    Color Red = Color.Red;

                    label12.ForeColor = Red;
                }

               

                textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

                if (NormalProcess)
                {
                    buttonManualReconc.Show();
                    if (RedoExpressPossible == true) buttonRedoExpress.Show();
                    else buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();
                }
                else
                {
                    buttonManualReconc.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();
                }
                
            }

            if (Am.Maker == "Express" ) // INCLUDE IN BALANCES ANY POSSIBLE CORRECTED ERRORS
            {             
            // Update Error Table
            // Turn Under Action to True

            WUnderAction = true ;

            Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction); 

                Process = 5;

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                textBox4.Text = Na.Balances1.HostBal.ToString("#,##0.00");

                textBox14.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

                textBox1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

                textBox2.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");

                textBox13.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

                textBox3.Text = (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj).ToString("#,##0.00");
                //
                //***************************************************************************

                textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

                //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
                if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
                {

                    label12.Text = "ATM No : " + WAtmNo + " IS RECONCILED ";

                    Color Green = Color.Green;

                    label12.ForeColor = Green;
                }
                else
                {
                    label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE ";

                    Color Red = Color.Red;

                    label12.ForeColor = Red;             

                }

                //textBoxOutstandingErr.Text = (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances).ToString(); 
                if (NormalProcess)
                {
                    buttonManualReconc.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Show();
                }
                else
                {
                    buttonManualReconc.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();
                }
               
            }

            if (Am.Maker == "Maker Actions") // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            {         

                Process = 4;

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

                //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

                textBox4.Text = Na.Balances1.HostBal.ToString("#,##0.00");

                textBox14.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

                textBox1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

                textBox2.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");


                textBox13.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

                textBox3.Text = (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj).ToString("#,##0.00");
                //
                //***************************************************************************

                textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

                //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
                if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
                {
               

                    label12.Text = "ATM No : " + WAtmNo + " IS RECONCILED ";

                    Color Green = Color.Green;

                    label12.ForeColor = Green;
                }
                else
                {
                    label12.Text = "ATM No : " + WAtmNo + " DO NOT RECONCILE ";

                    Color Red = Color.Red;

                    label12.ForeColor = Red;
                }


                if (NormalProcess)
                {
                    buttonManualReconc.Show();
                    if (RedoExpressPossible == true) buttonRedoExpress.Show();
                    else buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();
                }
                else
                {
                    buttonManualReconc.Hide();
                    buttonRedoExpress.Hide();
                    buttonUndoExpress.Hide();
                }
              
            }

          
            if (Am.ProcessMode == -2)
            {
                label17.Hide();
                dataGridView2.Hide();
           
                buttonManualReconc.Hide();
              
                textBoxMsgBoard.Text = "This ATM is not active yet! It will become automatically active when money is added. ";

                MessageBox.Show("This ATM is not active yet!");

                return;
            }

            if (WAction == 11) // FIND GROUP NUMBER For group reconciliation 
            {
                Ac.ReadAtm(WAtmNo);
                //textBox6.Text = Ac.AtmsReconcGroup.ToString();
            }

            try
            {
                // Show errors table 
                string filter = "AtmNo='" + WAtmNo + "' AND CurDes ='" + Na.Balances1.CurrNm
                    + "'" + " AND (ErrType = 1 OR ErrType = 2 OR ErrType = 5) AND SesNo<=" + WSesNo + " AND (OpenErr=1 OR (OpenErr=0 AND ActionSes =" + WSesNo + "))  ";
                errorsTableBindingSource.Filter = filter;
                this.errorsTableTableAdapter.Fill(this.aTMSDataSet61.ErrorsTable);

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

        }

// ROW ENTER FOR TRACES
        int WPreviousErrNo; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            //textBox28.Text = rowSelected.Cells[0].Value.ToString();
            WErrNo = (int)rowSelected.Cells[0].Value;

            if (WErrNo == WPreviousErrNo)
            {
                return;
            }
            else
            {
                WPreviousErrNo = WErrNo;
            }

            // Find Errors details 
            Er.ReadErrorsTableSpecific(WErrNo);

            if (WAtmNo == Er.AtmNo)
            {

                WTranNo = Er.TransNo;

                WTraceNo = Er.TraceNo;

                WErrId = Er.ErrId;


                textBox23.Text = Er.ErrAmount.ToString("#,##0.00");
                textBox24.Text = Er.CurDes.ToString();     

                if (Er.DrCust == true)
                {
                    textBox38.Text = " DEBIT CUSTOMER ";
                }
                if (Er.CrCust == true)
                {
                    textBox38.Text = " CREDIT CUSTOMER ";
                }
                if (Er.DrAtmCash == true)
                {
                    textBox39.Text = " DEBIT HOST CASH ";
                }
                if (Er.CrAtmCash == true)
                {
                    textBox39.Text = " CREDIT HOST CASH ";
                }

                if (Er.DrCust == false & Er.CrCust == false)
                {
                    if (Er.DrAtmCash == true)
                    {
                        textBox38.Text = " DEBIT HOST CASH ";
                    }
                    if (Er.CrAtmCash == true)
                    {
                        textBox38.Text = " CREDIT HOST CASH ";
                    }

                    if (Er.DrAtmSusp == true)
                    {
                        textBox39.Text = " DEBIT HOST SUSPENSE";
                    }
                    if (Er.CrAtmSusp == true)
                    {
                        textBox39.Text = " CREDIT HOST SUSPENSE";
                    }

                }

                if (Er.UnderAction == true)
                {
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
                }
                else
                {
                    radioSystemSuggest.Checked = false;
                    radioButtonMoveToDispute.Checked = false;

                    radioButtonPostpone.Checked = false;
                    radioButtonForceClose.Checked = false;

                    label3.Text = "Action... Not taken yet!"; 
                }

                if (Er.MaskRecordId > 0)
                {
                    Dt.ReadDisputeTranForInPool(Er.MaskRecordId);
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

// Undo Express
        private void buttonUndoExpress_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            Am.ReadAtmsMainSpecific(WAtmNo);
            Am.Maker = "No Decision";
            Am.UpdateAtmsMain(WAtmNo);

            // Update Error Table
            // Turn Under Action to False 

            WUnderAction = false;

            Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;


        }
// Redo Express
        private void buttonRedoExpress_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            // Update Error Table
            // Turn Under Action to True

            WUnderAction = true;

            Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

            Process = 5;

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, Process);

            //  If by considering all errors there is success in no difference then we update the ERRORS and Sessions 

            //if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & (Na.Balances1.ErrOutstanding - Na.ErrorsAdjastingBalances) == 0)
            if (Na.DiffAtAtmLevel == false & (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj) == 0 & Na.Balances1.ErrOutstanding == 0)
            {

                label12.Text = "ATM No :" + WAtmNo + " Reconciles ";
            }
            else
            {
                MessageBox.Show("This ATM cannot be Express. It doesnt reconcile with actions on Errors.");

                // Undo what you have done

                WUnderAction = false;

                Er.UpdateErrorsWithChangeUnderAction(WOperator, WAtmNo, WUnderAction);

                return;
            }

            textBox4.Text = Na.Balances1.HostBal.ToString("#,##0.00");

            textBox14.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

            textBox1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");

            textBox2.Text = (Na.Balances1.CountedBal - Na.Balances1.MachineBal).ToString("#,##0.00");


            textBox13.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");

            textBox3.Text = (Na.Balances1.CountedBal - Na.Balances1.HostBalAdj).ToString("#,##0.00");
            //
            //***************************************************************************

            textBoxOutstandingErr.Text = Na.Balances1.ErrOutstanding.ToString();

           

            Am.ReadAtmsMainSpecific(WAtmNo);

            Am.Maker = "Express";

            Am.UpdateAtmsMain(WAtmNo);   

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

        }

        // PROCEED TO MANUAL PER ATM
        private void buttonManualReconc_Click(object sender, EventArgs e)
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
            //Keep Row Selection positioning 
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;
            WRow2 = dataGridView2.SelectedRows[0].Index;


            // REPLENISHMENT PROCESS CODES
            // SINGLES
            // WFunction = 1 Normal branch ATM
            // 25 Off site ATM = cassettes are ready and go in ATM
            // 26 Belongs to external - CIT 
            // GROUPS
            // 5 Normal Group belonging to Bank . 
            // 30 Offsite Group belonging to Bank
            // 31 Group belonging to - CIT 


            if (WAction == 11) // Reconciliation FOR Groups 
            {
                // GO TO RECONCILIATION 
                // UPDATE INTENTED FUNCTION 

                // UPDATE INTENTED FUNCTION 

                Us.ReadSignedActivityByKey(WSignRecordNo);

                Us.ProcessNo = 5;

                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm71 = new Form71(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo);
                NForm71.FormClosed += NForm71_FormClosed;
                NForm71.ShowDialog(); ;

            }
        }

        void NForm71_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Us.ProcessNo == 5)
            {
                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.Maker = "Maker Actions";

                Am.UpdateAtmsMain(WAtmNo);
            }

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

        }



// See Transactions 
        private void button6_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            string FilterATM = "AtmNo='" + WAtmNo + "'" + " AND SesNo =" + WSesNo + " AND AtmTraceNo=" + WTraceNo;

            String FilterHost = "AtmNo='" + WAtmNo + "'" + " AND TraceNumber=" + WTraceNo;

            if (WErrId < 200)
            {
                NForm75 = new Form75(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, FilterATM, FilterHost, WTranNo);
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
            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

// GO TO View only flow 
        private void button1_Click_1(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            Us.ReadSignedActivityByKey(WSignRecordNo);

            Us.ProcessNo = 54;
            Us.WFieldChar1 = "ViewFromBULK"; 

            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            NForm71 = new Form71(WSignedId, WSignRecordNo, WBankId, WAtmNo, WSesNo);
            NForm71.FormClosed += NForm71_FormClosed;
            NForm71.ShowDialog(); 

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

            // Validate if outstanding no decision  
       
            int WTranNo = 0;
            Form110 NForm110; 

            string WOrigin = "ReconciliationBulk";

            int AuthorSeqNumber = 0; // This is used >0 when calling from Authorization management 
            NForm110 = new Form110(WSignedId, WSignRecordNo, WOperator, WOrigin, WTranNo, WAtmNo, WSesNo, AuthorSeqNumber, "Normal");
            NForm110.FormClosed += NForm110_FormClosed;
            NForm110.ShowDialog();
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

            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 56) WRequestor = true; // Requestor
            else NormalProcess = true;

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationBulk");

            if (WRequestor == true & Ap.Stage == 1)
            {
                textBoxMsgBoard.Text = "Message was sent to authoriser. Refresh for progress ";

                buttonUndoExpress.Hide();
                buttonRedoExpress.Hide();
                buttonManualReconc.Hide(); 
            }

            if (WRequestor == true & Ap.Stage == 4)
            {
                textBoxMsgBoard.Text = "Authorisation made. Workflow can finish! ";

                buttonUndoExpress.Hide();
                buttonRedoExpress.Hide();
                buttonManualReconc.Hide(); 
               
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


            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Reconc closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (WViewFunction == true || WAuthoriser == true || WRequestor == true) // THIS is not normal process 
            {
                ViewWorkFlow = true;

                if (Cn.TotalNotes == 0)
                {
                    //label1.Hide();

                    buttonNotes2.Hide();
                    labelNumberNotes2.Hide();
                }
                else
                {
                    buttonNotes2.Show();
                    labelNumberNotes2.Show();

                }
            }
            else
            {
                buttonNotes2.Show();
                labelNumberNotes2.Show();
            }


            if (WRequestor == true) // Comes from Author management Requestor
            {
                // Check Authorisation 

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationBulk");

                if (Ap.RecordFound == true & Ap.OpenRecord == true)
                {
                    if (Ap.Stage == 4 & Ap.AuthDecision == "YES")
                    {
                        //  guidanceMsg = " Finish Authorisation .";
                    }
                    if (Ap.Stage == 4 & Ap.AuthDecision == "NO")
                    {
                        Us.ReadSignedActivityByKey(WSignRecordNo);
                        Us.ProcessNo = 2; // Return to stage 2  
                        Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                        NormalProcess = true; // TURN TO NORMAL TO SHOW WHAT IS NEEDED 
                        //WAuthNegative = true;

                        buttonNotes2.Show();
                        labelNumberNotes2.Show();
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

            Ap.ReadAuthorizationForReplenishmentReconcSpecific(WAtmNo, WSesNo, "ReconciliationBulk");
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
                    buttonManualReconc.Hide();

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
                    buttonManualReconc.Hide();

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
                    buttonManualReconc.Hide();

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
                        buttonManualReconc.Hide();

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
                        buttonManualReconc.Hide();

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
                    //this.Dispose();
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

            textBoxMsgBoard.Text = "Authorisation Made - Accepted ";

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;

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

            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// Author History 
        private void buttonAuthHistory_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;

            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "History", WAtmNo, WSesNo, WDisputeNo, WDisputeTranNo);
            NForm112.FormClosed += NForm112_FormClosed;
            NForm112.ShowDialog();
        }

        void NForm112_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
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
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }


// DECLARATIONS FOR FINISH 
       
        bool ReconciliationAuthorDone; 
        
        string TAtmNo ;
        int TSesNo ;
        string TMaker ;
       

// Finish Bulk for Originator and Authoriser 
        private void buttonFinishBulk_Click(object sender, EventArgs e)
        {
            
            if (WAuthoriser == true & Am.AuthNoDecisTotal == 0) // Coming from authoriser and authoriser done  
            {
                this.Close();
                return;
            }

            if (Us.ProcessNo == 56 & Am.AuthNoDecisTotal > 0) // Coming from Requestor after author request was sent.  
            {
                this.Close();
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

            if (Us.ProcessNo == 5 & Am.AuthNoDecisTotal == 0) // Cancel by originator without request for authorisation 
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

            if (Us.ProcessNo == 5 & Am.AuthNoDecisTotal > 0) // Cancel by originator without request for authorisation 
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

            if (Us.ProcessNo == 5 & UnMatchedFound == true ) // 
            {


                this.Close();
                    return;
              

            }

            I = 0;

            while (I <= (Am.ATMsMainSelected.Rows.Count - 1))
            {

                TAtmNo = (string)Am.ATMsMainSelected.Rows[I]["AtmNo"];
                TSesNo = (int)Am.ATMsMainSelected.Rows[I]["ReplCycle"];
                TMaker = (string)Am.ATMsMainSelected.Rows[I]["Maker"];

                ReconciliationAuthorDone = false;

                Ap.ReadAuthorizationForReplenishmentReconcSpecific(TAtmNo, TSesNo, "ReconciliationBulk");
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



                    if (TMaker == "Express")
                    {
                        // REad errors one by one and update the Under action status 
                        Er.ReadAllErrorsTableForTurboAction(WOperator, TAtmNo); // READ AND UPDATE WITHIN 
                    }


                    //
                    // THE BELOW METHOD CREATES THE TRANSACTIONS TO BE POSTED 
                    Er.ReadAllErrorsTableForPostingTrans(WOperator, "EWB110", TAtmNo, WSignedId, Ap.Authoriser, TSesNo);
                    //
                    // Update Traces at Finish
                    //
                    // UPDATE TRACES WITH FINISH RECONC
                    int Mode = 2; // After reconciliation 
                    Ta.UpdateTracesFinishReplOrReconc(TAtmNo, TSesNo, WSignedId, Mode);

                    // READ Ta to see if differences AND SEND EMAIL ALERTS

                    Ta.ReadSessionsStatusTraces(TAtmNo, Ta.LastReplCyclId);

                    // READ TO UPDATE AM
                    Am.ReadAtmsMainSpecific(TAtmNo); // 

                    if (Ta.Recon1.DiffReconcEnd == true)
                    {
                        Am.ReconcDiff = true;
                    }
                    else
                    {
                        Am.ReconcDiff = false;
                    }

                    Am.SessionsInDiff = Ta.SessionsInDiff;

                    Er.ReadAllErrorsTableForCounters(WOperator, "EWB110", TAtmNo);
                    Am.ErrOutstanding = Er.NumOfErrors;

                    Am.ReconcCycleNo = Ta.SesNo;
                    Am.ReconcDt = Ta.Recon1.RecFinDtTm;
                    // Register the amount 
                    Am.CurrNm1 = Ta.Diff1.CurrNm1;
                    Am.DiffCurr1 = Ta.Diff1.DiffCurr1;

                    Am.LastUpdated = DateTime.Now;

                    if (Am.CurrentSesNo == Ta.SesNo)
                    {
                        Am.ProcessMode = Ta.ProcessMode;
                    }

                    Am.UpdateAtmsMain(TAtmNo); 
                }
                else
                {
                }
               

                I++;
            }
            // UPDATE COMPLETION 
            if (WRMCategory == "EWB110")
            {
                Er.ReadAllErrorsTableForCounters(WOperator, WRMCategory, "");

                Rs.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycle);

                Rs.RemainReconcExceptions = 0 ;

                Rs.EndReconcDtTm = DateTime.Now; 

                Rs.UpdateCategRMCycleWithAuthorClosing(WRMCategory, WRMCycle);

                Rc.ReadReconcCategorybyCategId(WOperator, WRMCategory);

                Rc.ReconcDtTm = DateTime.Now;

                if (Rs.RemainReconcExceptions > 0)
                {
                    Rc.ReconcStatus = "Category Has Outstanding Differences";
                }
                else
                {
                    Rc.ReconcStatus = "Category Has No Differences";
                }

                Rc.UpdateCategory(WOperator, WRMCategory);
            }

            //MessageBox.Show("No transactions to be posted");
            Form2 MessageForm = new Form2("Reconciliation Workflow for ATMs has finished." + Environment.NewLine
                                           + "Transactions to be posted were created as a result" + Environment.NewLine
                                           );
            MessageForm.ShowDialog();

            this.Close();
           

        }

// 
        void NForm174_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// Show UnMatched
        private void buttonUnMatched_Click(object sender, EventArgs e)
        {
            RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

            //
            // Check if outstanding unmatched for this ATM 
            //
            string SearchingStringLeft = "Operator ='" + WOperator
                          + "' AND (RMCateg ='EWB102' OR RMCateg ='EWB103' OR RMCateg ='EWB104' OR RMCateg ='EWB105' OR RMCateg ='EWB106') "
                          + " AND TerminalId ='" + WAtmNo + "' AND (OpenRecord = 1 AND ActionType <> '4') ";

            string WhatFile = "UnMatched";
            string WSortValue = "SeqNo";
            Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, SearchingStringLeft, WhatFile, WSortValue);

            Form78b NForm78b;
           
            WRowIndexLeft = dataGridViewMyATMS.SelectedRows[0].Index;
            string WHeader = "SELECTED TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rm.RMDataTableLeft, WHeader, "Form52b");
            NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            int scrollPosition = dataGridViewMyATMS.FirstDisplayedScrollingRowIndex;

            Form52b_Load(this, new EventArgs());

            dataGridViewMyATMS.Rows[WRowIndexLeft].Selected = true;
            dataGridViewMyATMS_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridViewMyATMS.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// View Replenishment PlayBack 
        private void buttonVewReplPlay_Click(object sender, EventArgs e)
        {
                Form51 NForm51; 
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for replenishment already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
                NForm51.ShowDialog();

        }  

    }
}

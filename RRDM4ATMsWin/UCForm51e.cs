using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51e : UserControl
    {
      
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        Form24 NForm24;
        Form26 NForm26;

        bool ViewWorkFlow; 

        bool ExistanceOfDiffNotes;

        string WMode; 

        int WFunction;

        // NOTES 
        string Order;

        string WParameter4 ;
        string WSearchP4 ;

        bool WNotesRead;
        bool WNotesUpdate; 

        RRDMNotesBalances Na = new RRDMNotesBalances(); // Class Notes 

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); // Class Traces 

        RRDMAtmsClass Ac = new RRDMAtmsClass(); // Class ATMs 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // Main Class

        RRDMDepositsClass Da = new RRDMDepositsClass(); // Contains all Deposits and Cheques 

        RRDMGasParameters Gp = new RRDMGasParameters(); // Get parameters 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMCaseNotes Cn = new RRDMCaseNotes(); 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

  
        string WAtmNo;
        int WSesNo;

        public void UCForm51ePar(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
     
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            Us.ReadSignedActivityByKey(WSignRecordNo);

            //************************************************************
            //TRACE AUTHORISATION
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************
          
            Us.ReadSignedActivityByKey(WSignRecordNo);

            // NOTES 
            Order = "Descending";
            WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 11 || Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }


            //************************************************************
            //************************************************************

            SetScreen(); 

        }
        //*************************************
        // Set Screen
        //*************************************
        public void SetScreen()
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Repl Closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";


            //*****************************************************************
            // Author
            //*****************************************************************

            if (ViewWorkFlow == true )
            {
                WNotesRead = true;
                WNotesUpdate = false;
            }
            else
            {
                WNotesRead = false;
                WNotesUpdate = true; 
            }

           

            // ================USER BANK =============================
            // Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            //   WUserOperator = Us.Operator;
            // ========================================================

            button5.Hide();
            button2.Hide();
            button4.Hide();


            // Show Total Balances for DEPOSITS 

            WFunction = 2; //  BALANCES 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL TO MAKE BALANCES AVAILABLE 

            Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo); // READ PHYSICAL CHECK

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            // SHOW BALANCES OF CASSETTES 

            ShowBalances(); // SHOW BALANCES 

            Ac.ReadAtm(WAtmNo);

            if (Ac.ChequeReader == false & Ac.DepoReader == false & Ac.EnvelopDepos == false)
            {
                labelNoDeposits.Visible = true;
                tableLayoutPanel2.Hide();
                tableLayoutPanel1.Hide();
            }
            else
            {
                labelNoDeposits.Visible = false;

                label18.Text = label18.Text + " - " + Da.DepositsMachine1.CurrNm;

                textBox4.Text = (Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej).ToString("#,##0.00");
                textBox6.Text = (Da.ChequesCount1.Amount).ToString("#,##0.00");

                textBox24.Text = (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej).ToString("#,##0.00");
                textBox25.Text = (Da.ChequesMachine1.Amount).ToString("#,##0.00");

                textBox26.Text = ((Da.DepositsCount1.Amount + Da.DepositsCount1.AmountRej) - (Da.DepositsMachine1.Amount + Da.DepositsMachine1.AmountRej)).ToString();
                textBox27.Text = ((Da.ChequesCount1.Amount) - (Da.ChequesMachine1.Amount)).ToString();
            }

            

            // Capture Cards
            //
            textBox9.Text = Na.CaptCardsMachine.ToString();
            textBox34.Text = Na.CaptCardsCount.ToString();

            textBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString(); // Captured Differences 

          //  textBox1.Text = Na.ReplUserComment;

            // Replenishment

            Am.ReadAtmsMainSpecific(WAtmNo);

            textBox3.Text = Am.NextReplDt.Date.DayOfWeek.ToString() + "  " + Am.NextReplDt.ToString();

            //TEST
            DateTime WDTm = new DateTime(2014, 02, 28);

            if (WAtmNo == "AB104" || WAtmNo == "ABC502")
            {

                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                WDTm = Ta.SesDtTimeEnd.Date;

            }

            //  DateTime WDTm = DateTime.Today;

            int DaysTillRepl = Am.NextReplDt.DayOfYear - WDTm.DayOfYear;

            //TEST
            // Take it out at production 
            if (DaysTillRepl > 7)
            {
                DaysTillRepl = 2; 
            }

            if (DaysTillRepl <= 0)
            {
                DaysTillRepl = 1;
            }

            textBox19.Text = Na.ReplAmountTotal.ToString("#,##0.00"); // Show Replenishment Amount        

            textBox20.Text = DaysTillRepl.ToString(); // Show Days till replenishemnt

            textBox15.Text = (Na.ReplAmountTotal / DaysTillRepl).ToString("#,##0.00");  // Daily Average 

            textBox23.Text = Na.ReplAmountSuggest.ToString("#,##0.00"); // Suggested total 

            // Insurance box
            textBox18.Text = Na.ReplAmountTotal.ToString("#,##0.00");
            textBox8.Text = Na.InsuranceAmount.ToString("#,##0.00");
            textBox1.Text = (Na.ReplAmountTotal - Na.InsuranceAmount).ToString("#,##0.00");


            Gp.ReadParametersSpecificId(WOperator, "603", "7", "", ""); // < is Green 
            int QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "7", "", ""); // > is Red 
            int QualityRange2 = (int)Gp.Amount;

            decimal diff = Na.ReplAmountSuggest - Na.ReplAmountTotal; // Difference than recommended 

            if (diff < 0)
            {
                diff = -diff;
            }

            decimal Ratio = (diff / Na.ReplAmountSuggest) * 100;

            int RatioInt = (int)Ratio;

            if (RatioInt <= QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
            }

            if (RatioInt >= QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (RatioInt > QualityRange1 & RatioInt < QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            // INSURANCE TRAFIC LIGHT ALERT 

          

            Gp.ReadParametersSpecificId(WOperator, "603", "8", "", ""); // < is Green Insurance 
            QualityRange1 = (int)Gp.Amount;

            Gp.ReadParametersSpecificId(WOperator, "604", "8", "", ""); // > is Red Insurance 
            QualityRange2 = (int)Gp.Amount;

            diff = Na.InsuranceAmount - Na.ReplAmountTotal; // Difference than recommended 

            if (diff >= 0)
            {
                // Green
                pictureBox3.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
            }
            else
            {
                // diff is < 0
                // Make Diff possitive
                diff = -diff;
                
                Ratio = (diff / Na.InsuranceAmount) * 100;

                RatioInt = (int)Ratio;

                if (RatioInt <= QualityRange1)
                {
                    // Green
                    pictureBox3.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
                }

                if (RatioInt >= QualityRange2)
                {
                    // Red 
                    pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
                }
                if (RatioInt > QualityRange1 & RatioInt < QualityRange2)
                {
                    // Yellow 
                    pictureBox3.BackgroundImage = Properties.Resources.YELLOW_Repl;
                }
            }

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            if (Da.DiffInDeposits == true || Da.DiffInCheques == true)
            {
                //   ExistanceOfDiffDep = true;
            }

            if (Na.PhysicalCheck1.Problem == true)
            {
                //     ExistanceOfDiffDep = true;
                textBox5.Text = "YES";
                buttonNotes.Show();
                labelNumberNotes.Show(); 
               //  textBox43.Text = Na.PhysicalCheck1.OtherSuspComm;
            }
            else
            {
                textBox5.Text = "NO";

                buttonNotes.Hide();
                labelNumberNotes.Hide(); 

              //  textBox43.Hide();
            }

            if (Na.NumberOfErrJournal > 0)
            {
                //   ExistanceOfDiffDep = true;
            }

            if (Na.NumberOfErrHost > 0)
            {
                //   ExistanceOfDiffDep = true;
            }

            if (Na.BalDiff1.AtmLevel == true || Na.BalDiff1.HostLevel == true || Na.ErrJournalThisCycle > 0)
            {
                ExistanceOfDiffNotes = true;
                if (Na.BalDiff1.AtmLevel == true)
                {
                    //     checkBox1.Checked = true;
                    //      button3.Show();
                }
                if (Na.BalDiff1.HostLevel == true)
                {
                    // checkBox6.Checked = true;
                }
            }
            else
            {
                //  textBox14.Text = "RECONCILED";
                //   button1.Hide();
            }

            if (Na.BalSets >= 2)
            {

                if (Na.BalDiff2.AtmLevel == true || Na.BalDiff2.HostLevel == true || Na.ErrJournalThisCycle > 0)
                {
                    ExistanceOfDiffNotes = true;
                }

            }
            if (Na.BalSets >= 3)
            {
                if (Na.BalDiff3.AtmLevel == true || Na.BalDiff3.HostLevel == true || Na.ErrJournalThisCycle > 0)
                {
                    ExistanceOfDiffNotes = true;
                }
            }
            if (Na.BalSets == 4)
            {

                if (Na.BalDiff4.AtmLevel == true || Na.BalDiff4.HostLevel == true || Na.ErrJournalThisCycle > 0)
                {
                    ExistanceOfDiffNotes = true;
                }
            }

            if (ExistanceOfDiffNotes == false) // Everything is in Order
            {
                //    textBox5.Hide();

                if (Na.NumberOfErrJournal > 0)
                {
                    guidanceMsg = " Attention is needed. ";

                    textBox2.Text = "This Repl Cycle has no difference at ATM level. However there are old errors which are outstanding. ";

                    pictureBox1.BackgroundImage = Properties.Resources.YELLOW_Repl;

                    // Update diffreneces 
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ReconcDifferenceStatus = 2 ; // No differences 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
                else
                {
                    guidanceMsg = " Well Done !!! No issues to deal with!!!";

                    textBox2.Text = "There are no differences or issues to deal with";

                    pictureBox1.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;

                    // Update diffreneces 
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ReconcDifferenceStatus = 1; // No differences 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
            }

            if (ExistanceOfDiffNotes == true)
            {
                // Level 1 Differences matched with presenter errors = Yellow
                // Level 2 Differences do not matched with presenter Error = Red  

                if (Na.BalDiff1.Machine == Na.Balances1.PresenterValue)
                {
                    guidanceMsg = "WARNING: There are differences but your presented errors is of the same value.";
                    textBox2.Text = "WARNING: There are differences but your presented errors is of the same value ="
                                     + Na.Balances1.PresenterValue + " Go to reconciliation process and take the needed actions.";
                    pictureBox1.BackgroundImage = Properties.Resources.YELLOW_Repl;

                    // Update diffreneces 
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ReconcDifferenceStatus = 1; // No differences 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }

                if (Na.BalDiff1.Machine != Na.Balances1.PresenterValue)
                {
                    guidanceMsg = "ERROR: THERE ARE DIFFERENCES.";

                    textBox2.Text = "ERROR: THERE ARE DIFFERENCES. Your presented errors which are "
                        + Na.Balances1.PresenterValue + " do not match the difference. You go to recociliation process and take actions. ";

                    pictureBox1.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

                    // Update diffreneces 
                    Us.ReadSignedActivityByKey(WSignRecordNo);
                    Us.ReconcDifferenceStatus = 2; // there are  differences 
                    Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
                // HANDLE ALL OTHER ERRORS WHICH ARE NOT PRESENTER 
                if (Na.Balances1.PresenterValue > 0)
                {
                }
            }
            //
            // Update Session Traces with Last evaluation comment 
            //
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            Ta.ReplGenComment = textBox2.Text;
            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

            //if (WAuthoriser == true) guidanceMsg = "View only.";
            //if (WAuthoriser == true) guidanceMsg = "Review and make authorisation.";
            //if (WRequestor == true) guidanceMsg = "Review and Update if authorisation completed.";
        }

        //
        // COMMIT BUTTON 
        // 
        private void button1_Click_1(object sender, EventArgs e)
        {
            WFunction = 4; // INCLUDE IN BALANCES ANY CORRECTED ERRORS 
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL
            
            // UPDATE SESSION TRACES 
            //
            if (Na.DiffAtAtmLevel == false & Na.DiffAtHostLevel == false & Na.DiffWithErrors == false)
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                //   NO DIFFERENCE AT ATM AND HOST 
                //   MessageBox.Show(" NO NEED TO GO TO RECONCILIATION Function ");
           //     WStatus = 1; // Everything reconcile 

                Ta.Repl1.DiffRepl = false;
                Ta.Repl1.ErrsRepl = false;
                Ta.Recon1.SignIdReconc = WSignedId; // Find out 
                Ta.Recon1.DelegRecon = false;
                Ta.Recon1.StartReconc = true;
                Ta.Recon1.FinishReconc = true;
                Ta.Recon1.RecStartDtTm = DateTime.Now;
                Ta.Recon1.RecFinDtTm = DateTime.Now;
                Ta.Recon1.DiffReconcStart = false;
                Ta.Recon1.DiffReconcEnd = false;
                Ta.NumOfErrors = 0;
                Ta.ErrOutstanding = 0;
                Ta.BalSetsNo = Na.BalSets;

                Ta.SessionsInDiff = 0;
                Ta.LatestBatchNo = Na.HBatchNo;

                if (Na.SystemTargets1.LastTrace < Ta.FirstTraceNo & Na.SystemTargets2.LastTrace < Ta.FirstTraceNo &
               Na.SystemTargets3.LastTrace < Ta.FirstTraceNo & Na.SystemTargets4.LastTrace < Ta.FirstTraceNo &
               Na.SystemTargets5.LastTrace < Ta.FirstTraceNo)
                {
                    Ta.UpdatedBatch = false; // HOST FILES NOT RECEIVED YET 
                }
                else
                {
                    Ta.UpdatedBatch = true; // HOST FILES RECEIVED 
                }

                //
                // UPDATE SESSION TRACES
                //
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            }
            else
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

             //   DiffRepl = true;
                if (Na.DiffAtAtmLevel == true || Na.DiffWithErrors == true)
                {
                    Ta.Repl1.ErrsRepl = true;
                    Ta.Repl1.DiffRepl = true;
                }
                //
                // UPDATE SESSION TRACES
                //

                Ta.Recon1.SignIdReconc = Ta.Repl1.SignIdRepl;
                //      Ta.Recon1.DelegRecon = false;

                Ta.Recon1.DiffReconcStart = true;
                Ta.Recon1.DiffReconcEnd = true;
                Ta.NumOfErrors = Na.NumberOfErrors;
                Ta.ErrOutstanding = Na.NumberOfErrors;
                Ta.BalSetsNo = Na.BalSets;

                if (Na.DiffAtHostLevel == true)
                {
            
                    Ta.Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
                    Ta.Diff1.DiffCurr1 = Na.BalDiff1.HostAdj;
               
                    Ta.Diff1.CurrNm2 = Na.BalDiff2.CurrNm;
                    Ta.Diff1.DiffCurr2 = Na.BalDiff2.HostAdj;
               
                    Ta.Diff1.CurrNm3 = Na.BalDiff3.CurrNm;
                    Ta.Diff1.DiffCurr3 = Na.BalDiff3.HostAdj;
                
                    Ta.Diff1.CurrNm4 = Na.BalDiff4.CurrNm;
                    Ta.Diff1.DiffCurr4 = Na.BalDiff4.HostAdj;
                }

                if (Na.DiffAtHostLevel == false & Na.DiffAtAtmLevel == true)
                {
               
                    Ta.Diff1.CurrNm1 = Na.BalDiff1.CurrNm;
                    Ta.Diff1.DiffCurr1 = Na.BalDiff1.Machine;
                 
                    Ta.Diff1.CurrNm2 = Na.BalDiff2.CurrNm;
                    Ta.Diff1.DiffCurr2 = Na.BalDiff2.Machine;
                
                    Ta.Diff1.CurrNm3 = Na.BalDiff3.CurrNm;
                    Ta.Diff1.DiffCurr3 = Na.BalDiff3.Machine;
                 
                    Ta.Diff1.CurrNm4 = Na.BalDiff4.CurrNm;
                    Ta.Diff1.DiffCurr4 = Na.BalDiff4.Machine;
                }

                Ta.LatestBatchNo = Na.HBatchNo;
                Ta.BalSetsNo = Na.BalSets;
                Ta.SessionsInDiff = Ta.SessionsInDiff + 1;

                if (Na.SystemTargets1.LastTrace < Ta.FirstTraceNo & Na.SystemTargets2.LastTrace < Ta.FirstTraceNo &
                Na.SystemTargets3.LastTrace < Ta.FirstTraceNo & Na.SystemTargets4.LastTrace < Ta.FirstTraceNo &
                Na.SystemTargets5.LastTrace < Ta.FirstTraceNo)
                {
                    Ta.UpdatedBatch = false; // HOST FILES NOT RECEIVED YET 
                }
                else
                {
                    Ta.UpdatedBatch = true; // HOST FILES RECEIVED 
                }

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                
            }

        }

        
        public void ShowBalances()
        {
            // FILL ALL FIELDS FOR ALL CURRENCIES 

            if (Na.BalSets >= 1)
            {
                label29.Text = " Currency " + Na.Balances1.CurrNm + ":";
                label13.Text = label29.Text;
                textBox28.Text = Na.Balances1.CountedBal.ToString("#,##0.00");
                textBox32.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
                textBox33.Text = Na.BalDiff1.Machine.ToString("#,##0.00");
            }

            if (Na.BalSets >= 2)
            {
                label19.Text = Na.Balances2.CurrNm;
                label14.Text = label19.Text;
                textBox30.Text = Na.Balances2.CountedBal.ToString("#,##0.00");
                textBox35.Text = Na.Balances2.MachineBal.ToString("#,##0.00");
                textBox36.Text = Na.BalDiff2.Machine.ToString("#,##0.00");
            }
            if (Na.BalSets >= 3)
            {
                label25.Text = Na.Balances3.CurrNm;
                label15.Text = label25.Text;
                textBox31.Text = Na.Balances3.CountedBal.ToString("#,##0.00");
                textBox37.Text = Na.Balances3.MachineBal.ToString("#,##0.00");
                textBox38.Text = Na.BalDiff3.Machine.ToString("#,##0.00");
            }

            if (Na.BalSets == 4)
            {
                label28.Text = Na.Balances4.CurrNm;
              //  label12.Text = label28.Text;
                textBox29.Text = Na.Balances4.CountedBal.ToString("#,##0.00");
                textBox39.Text = Na.Balances4.MachineBal.ToString("#,##0.00");
                textBox40.Text = Na.BalDiff4.Machine.ToString("#,##0.00");
            }

            // SHOW ... SHOW ... SHOW 

        //    panel6.Show();
        //    label24.Show();
            label19.Show(); textBox30.Show(); textBox35.Show(); textBox36.Show();
            label25.Show(); textBox31.Show(); textBox37.Show(); textBox38.Show();
            label28.Show(); textBox29.Show(); textBox39.Show(); textBox40.Show();
            label14.Show();
            label15.Show(); 
        //    label12.Show(); 

            if (Na.BalSets == 1)
            {
                label19.Hide(); textBox30.Hide(); textBox35.Hide(); textBox36.Hide();
                label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
                label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
                label14.Hide();
                label15.Hide();
              //  label12.Hide(); 
            }
            if (Na.BalSets == 2)
            {
                label25.Hide(); textBox31.Hide(); textBox37.Hide(); textBox38.Hide();
                label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
                label15.Hide();
            //    label12.Hide(); 
            }
            if (Na.BalSets == 3)
            {
                label28.Hide(); textBox29.Hide(); textBox39.Hide(); textBox40.Hide();
              //  label12.Hide(); 
            }
            if (Na.BalSets == 4)
            {
                // HIDE Nothing
            }
        }

        // Show Errors 

        private void button5_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrType = 1 ) AND OpenErr =1";
            NForm24 = new Form24( WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }

        // GO TO CAPTURED CARDS 

        private void button4_Click(object sender, EventArgs e)
        {
            NForm26 = new Form26(WSignedId, WSignRecordNo, WOperator,  WAtmNo, WSesNo);
            NForm26.Show();
        }

      

// Notes Read - Inspection 
        private void buttonNotes_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Physical Ispection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, "Read", SearchP4);
            NForm197.ShowDialog();
        }
// Notes for final assesment 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter4 = "Repl Closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string WParameter3 = WAtmNo;
            string SearchP4 = "";
            if (WNotesUpdate) WMode = "Update";
            if (WNotesRead) WMode = "Read";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            SetScreen(); 
           
        }
      
    }
}

using System;
using System.Windows.Forms;
//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51d2 : UserControl
    {
        Decimal CashInAmount;

        bool GLDifference; 

        //    DateTime WReplDate;

        // GL AMT 101600.00

        //   decimal WCurrentBal;

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        //string WHolidaysVersion;
        bool ViewWorkFlow;

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        RRDMMatchingTxns_InGeneralTables_BDC Gt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMHolidays Ch = new RRDMHolidays();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();

        //string WJobCategory = "ATMs";

        int WReconcCycleNo;

        decimal Txns_To_Repl_Value;

        DateTime SesStart;
        DateTime SesEnd;

        DateTime WCAP_DATE; 

        DateTime Last_Cut_Off_Date;

        decimal LoadedAmount;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51d2Par(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            try
            {
                // ................................
                // Handle View ONLY 
                //
                RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords(); 
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
                {
                    ViewWorkFlow = true;

                    buttonUpdate.Hide();
                }

                //************************************************
                // FIND REPLENISHEMENT CAP_DATE
                // FIND TRANSACTIONS UP TO REPLENISHEMENT 
                // Read_AND_Find_Repl_CAP_DATE(string InTerminalId, DateTime InDateFrom, DateTime InDateTo)
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                SesStart = Ta.SesDtTimeStart;
                SesEnd = Ta.SesDtTimeEnd;
                string OrderString = "ORDER BY TransDate DESC";
                WCAP_DATE = Gt.Read_AND_Find_Repl_CAP_DATE(WAtmNo, SesStart, SesEnd, OrderString,2);

              //  Txns_To_Repl_Value = Gt.ReadTrans_Totals_ForCurrent_CAP_DATE_Upto_Replenishement(WAtmNo, WCAP_DATE, SesEnd);

                textBoxAfterCut.Text = Txns_To_Repl_Value.ToString(); 

                //************************************************

                Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                if (Ra.RecordFound)
                {
                    textBoxOrderNo.Text = Ra.ReplOrderNo.ToString();
                    textBoxOrderDate.Text = Ra.DateInsert.ToString();
                    textBoxIssueById.Text = Ra.AuthUser;
                    Us.ReadUsersRecord(Ra.AuthUser);
                    textBoxIssueByName.Text = Us.UserName;
                    textBoxInsurAmount.Text = Ra.InsuredAmount.ToString("#,##0.00");
                    textBoxSysSaidAmt.Text = Ra.SystemAmount.ToString("#,##0.00");
                    textBoxMoneyIn.Text = Ra.NewAmount.ToString("#,##0.00");
                    // Find Loaded Amount as per journal 
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    Na.ReadSessionsNotesAndValues(WAtmNo, Na.NextSes, 2);

                    LoadedAmount = Na.ReplAmountTotal;
                    textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");

                }
                else
                {
                    if (WAtmNo == "AB104")
                    {
                        MessageBox.Show("No Action Record Available. Go in Code of UCForm51d2 and change REPL Cycle.");
                    }
                    else
                    {
                        //MessageBox.Show("No Action Record Available. Complete repenishment with Applying Overrride.");
                    }

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    Na.ReadSessionsNotesAndValues(WAtmNo, Na.NextSes, 2);

                    LoadedAmount = Na.ReplAmountTotal;
                    textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");

                }

                if ((Ra.NewAmount - LoadedAmount) != 0)
                {
                    labelAlertForLoading.Show();
                    textBoxAlert.Show();
                }
                else
                {
                    labelAlertForLoading.Hide();
                 
                    textBoxAlert.Hide();
                }

                //
                // FIND WHETHER GL WAS UPDATED 
                //

                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                Rjc.Find_GL_Cut_Off_Before_GivenDate(InOperator, Ta.SesDtTimeEnd.Date);
                if (Rjc.RecordFound == true & Rjc.Counter == 0)
                {
                    // Cut off of previous to Replenishment 
                    Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                    //IsDataFound = true;
                    textBox_GL_Date.Text = Last_Cut_Off_Date.ToShortDateString();

                    if ( WAtmNo == "NB0553C1" & textBox_GL_Date.Text == "18/03/2018" & ViewWorkFlow == false)
                    {
                        MessageBox.Show("This Demo. GL= 101,600"); 
                    }
                }
                else
                {
                    // Previous date Cut Off Not found = new ATM
                    // Date is set to be 
                    label13.Hide();
                    panel3.Hide();
                    Last_Cut_Off_Date = Ta.SesDtTimeEnd.Date;
                    //IsDataFound = true;
                    textBox_GL_Date.Text = Last_Cut_Off_Date.ToShortDateString();
                    MessageBox.Show("Migration Just Started." + Environment.NewLine
                                   + "GL Cannot be checked!" + Environment.NewLine
                                   + "Review the loaded money and move to the next step."
                                    );

                    ButtonGLStatusUsed = true; 

                    return; 
                }

                // Move to next step   
                Gadj.Read_GL_Balances_And_AtmNo_And_Cut_Off_Date(InAtmNo, Last_Cut_Off_Date);
                if (Gadj.RecordFound == true)
                {
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 4);
                    textBoxNet_GL_NBG.ReadOnly = true;
                    if (WOperator == "ETHNCY2N")
                    {
                        // Make correction of input General Ledger
                        //Gadj.GL_Balance = InGlAmount - LoadedAmount + Na.Balances1.MachineBal;
                        InGlAmount  = Gadj.GL_Balance + LoadedAmount - Na.Balances1.MachineBal ;
                        textBoxEnteredGL.Text = InGlAmount.ToString();
                        if (ViewWorkFlow == true) textBoxEnteredGL.Enabled = false; 
                    }
                    else
                    {
                        InGlAmount = Gadj.GL_Balance ;
                        textBoxEnteredGL.Text = InGlAmount.ToString();
                    }

                    ShowGLAnalysis();
                }
                else
                {
                    
                }

                int WFunction = 2; //  BALANCES 
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // CALL TO MAKE BALANCES AVAILABLE 

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

            //TEST
            guidanceMsg = "Push Update and Move to Next step or Use Override!";

            if (ViewWorkFlow == true) guidanceMsg = "View Only!";

        }


        // Update 

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
           
            if (ButtonGLStatusUsed == false)
            {
                MessageBox.Show("Please make GL Check!");
                return; 
            }

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.ReadSessionsNotesAndValues(WAtmNo, Na.NextSes, 2);

            LoadedAmount = Na.ReplAmountTotal;
            textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");

            if (decimal.TryParse(textBoxJournal.Text, out CashInAmount))
            {
            }
         
            if (CashInAmount == 0 & Ra.NewAmount == 0)
            {
                MessageBox.Show("No Money to Update!");
                return;
            }
            

            if (CashInAmount == 0)
            {
                CashInAmount = Ra.NewAmount;
            }

            // Update Na data Bases with money in 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.InUserDate = DateTime.Now;

            Na.InReplAmount = CashInAmount;

            Na.InsuranceAmount = Ra.InsuredAmount;

            Na.ReplAmountTotal = CashInAmount;

            if (Ra.NewAmount == 0) // In case there is no Order 
            {
                Na.ReplAmountSuggest = CashInAmount;
            }
            else
            {
                Na.ReplAmountSuggest = Ra.NewAmount;
            }

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            //
            // Update Actions table 
            // 
            Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);
            if (Ra.RecordFound)
            {
                Ra.ActiveRecord = false; 
                Ra.PassReplCycle = true;
                Ra.PassReplCycleDate = DateTime.Now;
                Ra.CashInAmount = CashInAmount;
                Ra.InMoneyReal = CashInAmount;
                Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);
            }

            //textBoxMoneyIn.Text = Na.InReplAmount.ToString("#,##0.00");

            // STEPLEVEL

            // Update STEP
          
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep5_Updated = true;

           // if (GLDifference == true) Usi.WFieldNumeric12 = 41; 

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            guidanceMsg = "Input data has been updated. Move to Next step.";

            ChangeBoardMessage(this, new EventArgs());
        }
        //
        // GL Status
        //
        bool ButtonGLStatusUsed ; 
        private void button_GL_Status_Click(object sender, EventArgs e)
        {
            ShowGLAnalysis();
         //   ButtonGLStatusUsed = true; 
        }
        // Show GL ANALYSIS
        decimal InGlAmount;
        private void ShowGLAnalysis()
        {
            RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMAccountsClass Acc = new RRDMAccountsClass();
         
            decimal AdjGlAmount;
            decimal LoadedMoney;

            ButtonGLStatusUsed = true;

            if (textBoxEnteredGL.Text == "") MessageBox.Show("Please enter the last CutOff GL Amt!");

            if (decimal.TryParse(textBoxEnteredGL.Text, out InGlAmount))
            {
            }
            else
            {
                MessageBox.Show(textBoxEnteredGL.Text, "Please enter a valid number!");

                return;
            }
            if (decimal.TryParse(textBoxJournal.Text, out LoadedMoney))
            {
            }
            else
            {
                MessageBox.Show(textBoxJournal.Text, "Please enter a valid number!");

                return;
            }

            panel5.Show();

            textBoxEnteredGL.Text = InGlAmount.ToString("#,##0.00");

            // Make data ready

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.Find_GL_Cut_Off_Before_GivenDate(WOperator, Ta.SesDtTimeEnd.Date);
            if (Rjc.RecordFound == true & Rjc.Counter == 0)
            {
                // Cut off of previous to Replenishment 
                Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                WReconcCycleNo = Rjc.JobCycle;
                //IsDataFound = true;
            }
            else
            {
                //IsDataFound = false;
            }

            Ac.ReadAtm(WAtmNo); // Read Information for ATM 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 4);

            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            // Delete previous record 
            Gadj.Delete_GL_Entry(WAtmNo, Last_Cut_Off_Date);

            Na.Is_GL_Adjusted = false;
            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // Prepare and Insert new record in GL File 
            //
            Gadj.OriginFileName = "From Repl";
            Gadj.OriginalRecordId = 0;
            Gadj.Cut_Off_Date = Last_Cut_Off_Date;
            Gadj.MatchingCateg = "";
            Gadj.AtmNo = WAtmNo;
            Gadj.Origin = "BANK";
            Gadj.TransTypeAtOrigin = "GL Entry";

            string ATMSuspence = "";
            string ATMCash = "";

            //if (Mpa.TargetSystem == 1) Acc.ReadAndFindAccount("1000", WOperator, WAtmNo, Er.CurDes, "ATM Suspense");

            Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Suspense");

            if (Acc.RecordFound == true)
            {
                ATMSuspence = Acc.AccNo;
            }
            else
            {
                MessageBox.Show("ATM Suspense Account Not Found for ATM :" + WAtmNo);
            }

            Acc.ReadAndFindAccount("1000", "", "", WOperator, WAtmNo, Ac.DepCurNm, "ATM Cash");
            if (Acc.RecordFound == true)
            {
                ATMCash = Acc.AccNo;
            }
            else
            {
                MessageBox.Show("ATM Cash Account Not Found for ATM :" + WAtmNo);
            }

            Gadj.GL_AccountNo = ATMCash;
            Gadj.Ccy = Ac.DepCurNm;

            if (WOperator == "ETHNCY2N")
            {
                // Make correction of input General Ledger
                Gadj.GL_Balance = InGlAmount - LoadedMoney + Na.Balances1.MachineBal;
            }
            else
            {
                Gadj.GL_Balance = InGlAmount;
            }

            Gadj.DateCreated = DateTime.Now;
            Gadj.Processed = false;

            Gadj.ProcessedAtRMCycle = WReconcCycleNo;
            Gadj.Operator = WOperator;

            int GLSeqNo = Gadj.Insert_GL_Balances();

            textBoxNet_GL_NBG.Text = Gadj.GL_Balance.ToString("#,##0.00");

            // Find if for this ATM there is GL difference

            decimal GL_Adjusted = 0;

            GL_Adjusted = InGlAmount - Txns_To_Repl_Value; 

            GL_Adjusted = Gadj.FindAdjusted_GL_Balance_AND_Update_Session_First_Method(WOperator, WAtmNo, WSesNo, Ta.SesDtTimeEnd.Date);

            if (Gadj.IsDataFound == true)
            {
                
                textBoxAfterCut.Text = Gadj.TotalDispensed.ToString("#,##0.00");

                if (Gadj.TotalDispensed == 0)
                {
                    buttonPrintTxnsGl.Hide();
                }
                else
                {
                    buttonPrintTxnsGl.Show();
                }

                if (Na.TotalOnErrorsAmt != 0)
                {
                    buttonActionsOnGL.Show();
                }
                else
                {
                    buttonActionsOnGL.Hide();
                }

                textBoxErrors.Text = Na.TotalOnErrorsAmt.ToString("#,##0.00"); // ERRORS within the last Cut Off

                textBoxAdjRepl.Text = (GL_Adjusted + Na.TotalOnErrorsAmt).ToString("#,##0.00");

                textBoxCounted.Text = Na.Balances1.CountedBal.ToString("#,##0.00");

                textBoxDifference.Text = (Na.Balances1.CountedBal - (GL_Adjusted + Na.TotalOnErrorsAmt)).ToString("#,##0.00");
                if ((Na.Balances1.CountedBal - (GL_Adjusted + Na.TotalOnErrorsAmt)) != 0)
                {
                    labelAlertGL.Show();
                    GLDifference = true;

                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.WFieldNumeric12 = 41;

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                }
                else
                {
                    GLDifference = false; 
                    labelAlertGL.Hide();
                    Usi.ReadSignedActivityByKey(WSignRecordNo);

                    Usi.WFieldNumeric12 = 0;

                    Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions(); 
                    Ec.ReadAllErrorsTableForCounters(WOperator, "", WAtmNo, WSesNo, "");
                    if (Ec.TotalErrorsAmtLess100 > 0 )
                    {
                        textBoxNote.Text = "There is presenter error" +Environment.NewLine 
                            + " of the value of :.." + Ec.TotalErrorsAmtLess100.ToString(); 
                    }
                    else
                    {
                        textBoxNote.Text = "No note available" ;
                    }

                }
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 4); 
                textBoxHostAdj_Na.Text = Na.Balances1.HostBalAdj.ToString("#,##0.00");
            }
            else
            {
                MessageBox.Show(" GL adjustments not found");
                return;
            }

        }

        // Print Txns Affecting GL
        private void buttonPrintTxnsGl_Click(object sender, EventArgs e)
        {
            string P1 = "TXNs from CutOff GL Till Replenishment for Atm:_"+ WAtmNo;

            string P2 = WAtmNo;
            string P3 = WSesNo.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R_TXNsFrom_CutOff Report_Txns_CutOff_To_Repl = new Form56R_TXNsFrom_CutOff(P1, P2, P3, P4, P5);
            Report_Txns_CutOff_To_Repl.Show();
        }
// Print Actions if any 
        private void buttonActionsOnGL_Click(object sender, EventArgs e)
        {
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs(); 
            // Matching is done but not Settled 
            string SelectionCriteria = " WHERE Operator ='" + WOperator + "'"
                      + " AND TerminalId ='" + WAtmNo + "'"
                      + " AND MatchingAtRMCycle =" + WReconcCycleNo
                      + " AND IsMatchingDone = 1 AND Matched = 0  "
                      + " AND ActionType != '7' ";

            string WSortCriteria = "Order By TerminalId, SeqNo ";

            Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
                                                                                     WSortCriteria,1);

            string P1 = "Actions on exceptions For Reconciliation Cycle : " + WReconcCycleNo.ToString();

            string P2 = "";
            string P3 = "";
            if (ViewWorkFlow == true)
            {

                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

                Ap.ReadAuthorizationForReplenishmentReconcSpecificForCategoryAndCycle("NBG101", WReconcCycleNo, "ReconciliationCat");

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
               
                    P2 = "Not Found";
                 
                    P3 = "Not Found";
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
// Show All Unmatched for this ATM uhis Cycle 
      
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string WCategoryId = "";
            int WRMCycle = 0;

            Form271ViewAtmUnmatched NForm271ViewAtmUnmatched;
            NForm271ViewAtmUnmatched = new Form271ViewAtmUnmatched(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRMCycle, WAtmNo, WSesNo);
         
            NForm271ViewAtmUnmatched.ShowDialog();
        }
    }
}

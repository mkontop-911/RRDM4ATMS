using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51e : UserControl
    {
        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        Form24 NForm24;
        Form26 NForm26;
        int WTotalCassetteNotes;
        decimal WTotalCassetteMoney;

        int WRetractTotalNotes;
        decimal WRetractTotalMoney;

        bool ViewWorkFlow;
        bool ExistanceOfDiffNotes;

        decimal DifferenceInLoadedMoney;

        string WMode;

        int WFunction;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        bool WNotesRead;
        bool WNotesUpdate;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        RRDMAtmsClass Ac = new RRDMAtmsClass(); // Class ATMs 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // Main Class

        RRDMDepositsClass Da = new RRDMDepositsClass(); // Contains all Deposits and Cheques 

        RRDMGasParameters Gp = new RRDMGasParameters(); // Get parameters 

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMSessionsPhysicalInspection Pi = new RRDMSessionsPhysicalInspection();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // RRDMGasParameters Gp = new RRDMGasParameters(); 
        bool AudiType = false;

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

            //************************************************************
            //TRACE AUTHORISATION
            //************************************************************
            // AUTHOR PART 
            // Comes from FORM112 = Author management 
            //************************************************************

            // NOTES 
            Order = "Descending";
            WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
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
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 11 || Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
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

            bool GlDiff;
            //
            // If GL Alert
            //
            if (Usi.WFieldNumeric12 == 41)
            {
                GlDiff = true;
                labelGLAlert.Show();
                pictureBoxGLAlert.Show();
                pictureBoxGLAlert.BackgroundImage = appResImg.RED_LIGHT_Repl;
            }
            else
            {
                GlDiff = false;
                labelGLAlert.Hide();
                pictureBoxGLAlert.Hide();
            }

            //*****************************************************************
            // Author
            //*****************************************************************

            if (ViewWorkFlow == true)
            {
                WNotesRead = true;
                WNotesUpdate = false;
            }
            else
            {
                WNotesRead = false;
                WNotesUpdate = true;
            }

            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
            //RRDMGasParameters Gpr = new RRDMGasParameters();
            AudiType = false;
            int IsAmountOneZero;
            Gp.ReadParametersSpecificId(WOperator, "945", "4", "", ""); // 
            if (Gp.RecordFound == true)
            {
                IsAmountOneZero = (int)Gp.Amount;

                if (IsAmountOneZero == 1)
                {
                    // Transactions will be done at the end 
                    AudiType = true;
                    // buttonGLTxns.Hide();
                }
                else
                {
                    // buttonGLTxns.Show();
                    AudiType = false;
                }
            }
            else
            {
                AudiType = false;
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

            //Na.ReadSessionsNotesAndValues3PhyCheck(WAtmNo, WSesNo); // READ PHYSICAL CHECK
            if (Na.Balances1.OpenBal == 0)
            {
                label2.Hide(); panel3.Hide();
                label18.Hide(); panel4.Hide();

                tableLayoutPanel8.Hide();
                tableLayoutPanel7.Hide();

                button4.Hide();
            }
            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            //RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "218";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string From_SM = Gp.OccuranceNm;

            bool DepositsFound = false;
            string WCcy = "EGP";

            if (From_SM == "YES")
            {
                // Find the first fuid to avoid duplication created by two the same journals
                //
                SM.Read_SM_AND_Get_First_fuid(WAtmNo, WSesNo);

                if (SM.RecordFound == true)
                {
                    // Fuid found
                    DepositsFound = true;

                    // Get the totals from SM and not from Mpa            
                    // GET TABLE
                    SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, WSesNo, SM.Fuid);
                    //
                    //***********************
                    //
                    string SM_SelectionCriteria1 = " WHERE AtmNo ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                                  + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                     ;

                    SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

                    if (SM.RecordFound == true)
                    {
                        // Get other Totals 

                        int WDEP_COUNTERFEIT = SM.DEP_COUNTERFEIT;
                        int WDEP_SUSPECT = SM.DEP_SUSPECT;

                    }
                    int WRECYCLEDTotalNotes;
                    decimal WRECYCLEDTotalMoney;

                    //int WTotalNCR_DepositsDispensedNotes;
                    //decimal WTotalNCR_DepositsDispensedMoney;

                    // Read Table 
                    int K = 0;
                    int I = 0;


                    while (I <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
                    {
                        // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                        WCcy = (string)SM.DataTable_SM_Deposits.Rows[I]["Ccy"];

                        if (WCcy.Trim() == "")
                        {
                            I = I + 1;
                            continue;
                        }
                        WTotalCassetteNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteNotes"];
                        WTotalCassetteMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteMoney"];

                        WRetractTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTNotes"];
                        WRetractTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTMoney"];

                        WRECYCLEDTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDNotes"];
                        WRECYCLEDTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDMoney"];

                        //WTotalNCR_DepositsDispensedNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedNotes"];
                        //WTotalNCR_DepositsDispensedMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedMoney"];

                        K = K + 1;

                        if (K == 1)
                        {
                            // TOTAL REMAIN IN CASSETTES 
                            // THIS IS THE FORMULA !!!!!!!!!!!!!!!!!!!
                            // (WTotalMoneyDecimal+WRECYCLEDTotalMoney) -  WTotalNCR_DepositsDispensedMoney
                            textBoxCcy1CassetteAmount.Text = ((WTotalCassetteMoney
                                + WRetractTotalMoney)).ToString("#,##0.00");


                            SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                            textBoxCcy1CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                            textBoxCcy1CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - ((WTotalCassetteMoney
                                + WRetractTotalMoney))).ToString("#0.00");

                        }

                        I = I + 1;
                    }

                }

            }
            else
            {
                //MessageBox.Show("No Deposits Found");
                // return;
            }


            // SHOW BALANCES OF CASSETTES 

            ShowBalances(); // SHOW BALANCES 

            Ac.ReadAtm(WAtmNo);

            if (DepositsFound == false)
            {
                labelNoDeposits.Visible = true;
                tableLayoutPanel2.Hide();
                tableLayoutPanel1.Hide();
            }
            else
            {
                labelNoDeposits.Visible = false;

                label18.Text = label18.Text + " - " + WCcy;

            }

            // Capture Cards
            //
            textBox9.Text = Na.CaptCardsMachine.ToString();
            textBox34.Text = Na.CaptCardsCount.ToString();

            textBox10.Text = (Na.CaptCardsCount - Na.CaptCardsMachine).ToString(); // Captured Differences 

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

            decimal diff;
            decimal Ratio;

            int RatioInt;
            //
            // Check different than recommended 
            //
            if (Na.ReplAmountSuggest > 0)
            {
                diff = Na.ReplAmountSuggest - Na.ReplAmountTotal; // Difference than recommended 

                if (diff < 0)
                {
                    diff = -diff;
                }

                if (diff != 0)
                {
                    DifferenceInLoadedMoney = diff;
                    // Red 
                    pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
                }
                else
                {
                    pictureBox2.Hide();
                    label24.Hide();
                }

                Ratio = (diff / Na.ReplAmountSuggest) * 100;

                RatioInt = (int)Ratio;
            }
            else
            {
                pictureBox2.Hide();
                label24.Hide();
                label49.Hide();
                textBox3.Hide();
            }

            //if (RatioInt <= QualityRange1)
            //{
            //    // Green
            //    pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            //}

            //if (RatioInt >= QualityRange2)
            //{
            //    // Red 
            //    pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;
            //}
            //if (RatioInt > QualityRange1 & RatioInt < QualityRange2)
            //{
            //    // Yellow 
            //    pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            //}

            // INSURANCE TRAFIC LIGHT ALERT 

            if (Na.InsuranceAmount > 0)
            {
                Gp.ReadParametersSpecificId(WOperator, "603", "8", "", ""); // < is Green Insurance 
                QualityRange1 = (int)Gp.Amount;

                Gp.ReadParametersSpecificId(WOperator, "604", "8", "", ""); // > is Red Insurance 
                QualityRange2 = (int)Gp.Amount;

                diff = Na.InsuranceAmount - Na.ReplAmountTotal; // Difference than recommended 

                if (diff >= 0)
                {
                    // Green
                    pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
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
                        pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }

                    if (RatioInt >= QualityRange2)
                    {
                        // Red 
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;
                    }
                    if (RatioInt > QualityRange1 & RatioInt < QualityRange2)
                    {
                        // Yellow 
                        pictureBox3.BackgroundImage = appResImg.YELLOW_Repl;
                    }
                }
            }

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            if (Da.DiffInDeposits == true || Da.DiffInCheques == true)
            {
                //   ExistanceOfDiffDep = true;
            }

            // Check if Physical Inspection Data 

            string SelectionCriteria = " ATMNo='" + WAtmNo + "' AND SesNo = " + WSesNo;
            Pi.ReadPhysicalInspectionRecordsToSeeIfAlert(SelectionCriteria);
            if (Pi.InspectionAlert == true)
            {
                textBox5.Text = "YES";
                buttonNotes.Show();
                labelNumberNotes.Show();
            }
            else
            {
                textBox5.Text = "NO";
                if (labelNumberNotes.Text != "")
                {
                    buttonNotes.Show();
                    labelNumberNotes.Show();
                }
                else
                {
                    buttonNotes.Hide();
                    labelNumberNotes.Hide();
                }
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


                if (DifferenceInLoadedMoney > 0)
                {
                    guidanceMsg = " Attention is needed. ";

                    textBoxResult.Text = "Warning. There is difference of loaded money. " + Environment.NewLine
                        + "Investigate the case and act accordingly.";


                    pictureBox1.BackgroundImage = appResImg.YELLOW_Repl;
                }
                else
                {
                    if (GlDiff == true)
                    {
                        guidanceMsg = " Well Done !!! No issues to deal with...";

                        textBoxResult.Text = "There are no differences at ATM level." + Environment.NewLine
                                             + "However there is difference in General Ledger";

                        pictureBox1.BackgroundImage = appResImg.YELLOW_Repl;
                    }
                    else
                    {
                        guidanceMsg = " Well Done !!! No issues to deal with...";

                        textBoxResult.Text = "There are no differences or issues to deal with.";

                        pictureBox1.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }
                }
                // Update diffreneces 
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReconcDifferenceStatus = 1; // No differences 
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            }

            if (ExistanceOfDiffNotes == true)
            {
                // Level 1 Differences matched with presenter errors = Yellow
                // Level 2 Differences do not matched with presenter Error = Red  

                // HANDLE ALL OTHER ERRORS WHICH ARE NOT PRESENTER 
                if (Na.Balances1.PresenterValue > 0)
                {
                    if (Na.BalDiff1.Machine == Na.Balances1.PresenterValue)
                    {
                        guidanceMsg = "WARNING: There are differences but your presented errors is of the same value.";
                        textBoxResult.Text = "WARNING: There are differences but your presented errors is of the same value ="
                                         + Na.Balances1.PresenterValue + "  Action of Presenter error was taken";
                        pictureBox1.BackgroundImage = appResImg.YELLOW_Repl;

                        // Update diffreneces 
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ReconcDifferenceStatus = 1; // No differences 
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }

                    if (Na.BalDiff1.Machine != Na.Balances1.PresenterValue)
                    {
                        guidanceMsg = "ERROR: THERE ARE DIFFERENCES.";

                        textBoxResult.Text = "ERROR: THERE ARE DIFFERENCES. Your presented errors which are "
                            + Na.Balances1.PresenterValue + " do not match the difference. You go to recociliation process and take actions. ";

                        pictureBox1.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        // Update diffreneces 
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ReconcDifferenceStatus = 2; // there are  differences 
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }
                }
                else
                {
                    //
                    // Presenter Error == 0 
                    //
                    if (Na.BalDiff1.Machine != 0)
                    {
                        guidanceMsg = "ERROR: There are differences of what you had counted against what ATM is reporting.";
                        textBoxResult.Text = "ERROR: There are differences of what you had counted against what ATM is reporting." + Environment.NewLine
                                         + "The amount in difference is..:.." + Na.BalDiff1.Machine;
                        pictureBox1.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        // Update diffreneces 
                        Usi.ReadSignedActivityByKey(WSignRecordNo);
                        Usi.ReconcDifferenceStatus = 2; // There are differences  
                        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                    }

                }



            }
            //
            // Update Session Traces with Last evaluation comment 
            //
            if (ViewWorkFlow == true)
            {
                // No Update 
            }
            else
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                Ta.ReplGenComment = textBoxResult.Text;
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            }


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
                    Ta.Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 
                }
                else
                {
                    Ta.Is_Updated_GL = true; // HOST FILES RECEIVED 
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
                    Ta.Is_Updated_GL = false; // HOST FILES NOT RECEIVED YET 
                }
                else
                {
                    Ta.Is_Updated_GL = true; // HOST FILES RECEIVED 
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
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }

        // GO TO CAPTURED CARDS 

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Change call code to go to capture Card");

            //NForm26 = new Form26(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            //NForm26.Show();
        }



        // Notes Read - Inspection 
        private void buttonNotes_Click_1(object sender, EventArgs e)
        {

            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Physical Inspection for " + "AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, "Read", SearchP4);
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
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            SetScreen();

        }
        // GL Txns 
        private void buttonGLTxns_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
                  + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute') ";
            Aoc.ReadActionsOccurancesAndFillTable_Big(WSelectionCriteria);

            Aoc.ClearTableTxnsTableFromAction();

            int I = 0;

            while (I <= (Aoc.TableActionOccurances_Big.Rows.Count - 1))
            {

                int WSeqNo = (int)Aoc.TableActionOccurances_Big.Rows[I]["SeqNo"];

                Aoc.ReadActionsOccuarnceBySeqNo(WSeqNo);

                int WMode2 = 1; // 

                Aoc.ReadActionsTxnsCreateTableByUniqueKey(Aoc.UniqueKeyOrigin, Aoc.UniqueKey, Aoc.ActionId, Aoc.Occurance
                                                             , Aoc.OriginWorkFlow, WMode2);
                I = I + 1;
            }

            DataTable TempTxnsTableFromAction;
            TempTxnsTableFromAction = Aoc.TxnsTableFromAction;

            Form14b_All_Actions NForm14b_All_Actions;

            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, TempTxnsTableFromAction, 1);
            NForm14b_All_Actions.ShowDialog();

        }
        // All Actions 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "";

            WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
               + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute') ";


            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            //string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();


        }
    }
}

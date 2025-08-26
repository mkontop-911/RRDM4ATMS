using System;
using System.Data;
using System.Windows.Forms;
//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51d2_AUD : UserControl
    {
        //
        // THIS IS FOR GL reconciliation at ATM level without Cash Management
        //
        Decimal CashInAmount;

        //bool GLDifference;

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

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime DateSesStart;
        DateTime DateSesEnd;

        bool AudiType ;

        DateTime WCAP_DATE_1;
        DateTime WCAP_DATE_2;

        DateTime Last_Cut_Off_Date;

        DataTable GL_1 = new DataTable();
        DataTable GL_2 = new DataTable();

        decimal WG4_Cash_Loaded;

        decimal WGlCash ;
        decimal WGlInter ;
        decimal WGlExcess ;
        decimal WGlShortage ;

        decimal DefaultManual; 

        //decimal TotalDrAfter = 0;
        //decimal TotalCrAfter = 0;
        //decimal TotalDrExceptionsAfter;
        //decimal TotalCrExceptionsAfter;

        decimal LoadedAmount;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51d2Par_AUD(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
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
                    buttonGL_1.Hide();
                    buttonShowBalances.Hide();
                }
                else
                {
                    buttonUpdate.Show();
                    buttonGL_1.Show();
                    buttonShowBalances.Show();
                    // textBoxAlert.Text = "Please Enter the amount loaded by Cit";
                }

                //************************************************
                // FIND REPLENISHEMENT CAP_DATE
                // FIND TRANSACTIONS UP TO REPLENISHEMENT 
                // Read_AND_Find_Repl_CAP_DATE(string InTerminalId, DateTime InDateFrom, DateTime InDateTo)
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                DateSesStart = Ta.SesDtTimeStart;
                DateSesEnd = Ta.SesDtTimeEnd;
                //
                // Find If AUDI Type 
                // If found and it is 1 is Audi Type If Zero then is normal 
                // FOR AUDI TYPE WE LOAD GL AND WE ALSO USE OTHER FORM For Replenishment 
                // 
                //RRDMGasParameters Gp = new RRDMGasParameters();
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
                    }
                    else
                    {
                        AudiType = false;
                    }
                }
                else
                {
                    AudiType = false;
                }
                //
                // FIND CAP_DATE
                //
                string OrderString = "ORDER BY TransDate ASC"; // GET the Beggining Gap_Date of CYCLE
                WCAP_DATE_1 = Gt.Read_AND_Find_Repl_CAP_DATE(WAtmNo, DateSesStart, DateSesEnd, OrderString, 2);

                WCAP_DATE_2 = DateSesEnd.Date;
                //
                // READ FEEDING AMOUNT
                //
                RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
                int WMode = 1; 
                G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, WMode);

                if (G4.RecordFound == true)
                {
                    WG4_Cash_Loaded = G4.Cash_Loaded;
                    textBoxInAmount.Text = WG4_Cash_Loaded.ToString();

                    labelFeedingRecord.Hide(); 
                }
                else
                {
                    labelFeedingRecord.Hide();
                    // labelFeedingRecord.Show();
                    // 
                }
                //
                // READ GL RECORD DETAILS
                //
                RRDMGL_Balances_Atms_Daily_AUDI Gl = new RRDMGL_Balances_Atms_Daily_AUDI();

                Gl.ReadGL_Balances_Atms_Daily_ATM_CUT_OFF(WAtmNo, WCAP_DATE_2);

                if (Gl.RecordFound == true)
                {
                    WGlCash = Gl.GL_Bal_ATM_Cash;
                    WGlInter = Gl.GL_Bal_ATM_Inter;
                    WGlExcess = Gl.GL_Bal_ATM_Excess;
                    WGlShortage = Gl.GL_Bal_ATM_Short;

                    textBoxEnteredGL.Text = WGlCash.ToString();

                    textBoxCash.Text = WGlCash.ToString("#,##0.00");
                    textBoxExcess.Text = WGlExcess.ToString("#,##0.00");
                    textBoxShortage.Text = WGlShortage.ToString("#,##0.00");
                    textBoxIntermediate.Text = WGlInter.ToString("#,##0.00");

                    labelGlRecordAlert.Hide(); 
                }
                else
                {
                    labelGlRecordAlert.Show();
                    // 
                }

                textBox_GL_Date_1.Text = WCAP_DATE_2.ToShortDateString();

                label13.Show();
                panel3.Show();
             
                //************************************************
                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.ReadAtm(WAtmNo);

                textBoxInAmount.Text = Ac.InsurOne.ToString("#,##0.00");

                // Find Loaded Amount as per journal 
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                textBoxInAmount.Text = Na.InReplAmount.ToString("#,##0.00");

                int NextReplCycle = Na.NextSes;
                // Find if cash added
                RRDMRepl_SupervisorMode_Details Sm = new RRDMRepl_SupervisorMode_Details();


                Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                LoadedAmount = // After date Cap_date
                              Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                            + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                            + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                            + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue
                              ;
                textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");
                textBoxInAmount.Text = LoadedAmount.ToString("#,##0.00");

            }
            catch (Exception ex)
            {

                string exception = ex.ToString();
                //       MessageBox.Show(ex.ToString());
                MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

            }

           

            //TEST
            guidanceMsg = "Press Update and Move to Next step or Use Override!";

            if (ViewWorkFlow == true) guidanceMsg = "View Only!";

            ShowGLFromBooks();
            ShowGLOfTheReplenishmentCycle();

        }
        //
        // Update 
        //
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (ShowAudi == false)
            {
                MessageBox.Show("Please press show Audi button before you move to update");
                return; 
            }

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            // GET DEFAULT MANUAL

            DefaultManual = (Na.Balances1.MachineBal + DepositsCountedInCycle);

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WAtmNo);

            decimal TempEnteredCore = 0; 

            if (decimal.TryParse(textBoxEnteredCore.Text, out TempEnteredCore))
            {
            }

            if (TempEnteredCore != DefaultManual)
            {
                if (MessageBox.Show("Warning: Entered Core Manual amount differs than default(expected)"+Environment.NewLine
                    +"Do you want to continue?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                {
                   // Continue
                }
                else
                {
                    return;
                }
            }

            //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            if (decimal.TryParse(textBoxInAmount.Text, out CashInAmount))
            {
            }

            textBoxDiffFromJournal.Text = (LoadedAmount - CashInAmount).ToString("#,##0.00");


            if (CashInAmount == 0 & Ra.NewAmount == 0)
            {
                MessageBox.Show("No Money to Update!");
                return;
            }

            if (CashInAmount == 0)
            {
                CashInAmount = Ra.NewAmount;
            }

            if (CashInAmount != LoadedAmount)
            {
                // textBoxAlert.Show();
                labelAlertForLoading.Show();
            }
            else
            {
                // textBoxAlert.Hide();
                labelAlertForLoading.Hide();
            }
            // Update Na data Bases with money in 

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.InUserDate = DateTime.Now;

            Na.InReplAmount = CashInAmount;

            Na.InsuranceAmount = Ac.InsurOne;

            Na.ReplAmountTotal = CashInAmount;


            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

          
            // CREATE ACTIONS OCCURANCES


            CreateActions_Occurances(CashInAmount);

           

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep4_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            guidanceMsg = "Input data has been updated. Move to Next step.";

            ChangeBoardMessage(this, new EventArgs());
        }
        //
        // GL Status
        //
        bool ButtonGLStatusUsed;

        // Show GL ANALYSIS
        decimal InGlAmount;

        // Print Txns Affecting GL
        private void buttonPrintTxnsGl_Click(object sender, EventArgs e)
        {
            string P1 = "TXNs from CutOff GL Till Replenishment for Atm:_" + WAtmNo;

            string P2 = WAtmNo;
            string P3 = WSesNo.ToString();
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R_TXNsFrom_CutOff Report_Txns_CutOff_To_Repl = new Form56R_TXNsFrom_CutOff(P1, P2, P3, P4, P5);
            Report_Txns_CutOff_To_Repl.Show();
        }

        // Show Transactions DEBITS 
        private void buttonShowTxnsGlDebit_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 11);

            NForm78D_ATMRecords.Show();
        }
        // Show Transactions CREDITS
        private void buttonShowCredit_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 12);

            NForm78D_ATMRecords.Show();
        }
        // 

        // Show SM Lines 
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

            string SM_SelectionCriteria1 = " WHERE atmno ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + WSesNo
                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                 ;

            SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, WSesNo, 2);

            if (SM.RecordFound == true)
            {
                Form67_BDC NForm67_BDC;

                int Mode = 7; // Given Fuid and Ruid 
                string WTraceRRNumber = "";
                NForm67_BDC = new Form67_BDC(WSignedId, 0, WOperator, SM.Fuid, WTraceRRNumber, WAtmNo
                    , SM.sessionstart_ruid, SM.sessionend_ruid, NullPastDate, NullPastDate, Mode);
                NForm67_BDC.ShowDialog();
            }
            else
            {
                MessageBox.Show("Not found records");
            }


        }
        // show debits 
        private void buttonShowDebits_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            DateTime DateTmSesStart = Ta.SesDtTimeEnd;
            // CHECK IN IST THAT WE HAVE THE CAP_DATE at the time of replenishment 
            string OrderString = "ORDER BY TransDate DESC";
            WCAP_DATE_2 = Gt.Read_AND_Find_Repl_CAP_DATE(WAtmNo, DateSesStart, DateSesEnd, OrderString, 2);

            //TotalWithdrawls = 0;
            //TotalDeposits = 0;
            if (Gt.RecordFound == true)
            {
            }

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateTmSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 8);

            NForm78D_ATMRecords.Show();
        }
        // SHOW DEPOSITS 
        private void buttonShowDeposits_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            DateTime DateTmSesStart = Ta.SesDtTimeEnd;
            // CHECK IN IST THAT WE HAVE THE CAP_DATE at the time of replenishment 
            string OrderString = "ORDER BY TransDate DESC";
            WCAP_DATE_2 = Gt.Read_AND_Find_Repl_CAP_DATE(WAtmNo, DateSesStart, DateSesEnd, OrderString, 2);

            //TotalWithdrawls = 0;
            //TotalDeposits = 0;
            if (Gt.RecordFound == true)
            {
            }

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateTmSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 9);

            NForm78D_ATMRecords.Show();
        }

        private void buttonExceptions_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            DateTime DateTmSesStart = Ta.SesDtTimeEnd;
            // CHECK IN IST THAT WE HAVE THE CAP_DATE at the time of replenishment 
            string OrderString = "ORDER BY TransDate DESC";
            WCAP_DATE_2 = Gt.Read_AND_Find_Repl_CAP_DATE(WAtmNo, DateSesStart, DateSesEnd,
                OrderString, 2);

            //TotalWithdrawls = 0;
            //TotalDeposits = 0;
            if (Gt.RecordFound == true)
            {
            }

            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId,
                       WAtmNo, DateTmSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 10);

            NForm78D_ATMRecords.Show();
        }
        //// Show GL Status 2 
        //private void buttonShowGLSTATUS_2_Click(object sender, EventArgs e)
        //{
        //    //panel6.Show();
        //    // Do this for Current CAP DATE and Keep Table 
        //    Gt.ReadTrans_Table_FromReplenishmentToEND_CAP_DATE_(WAtmNo,
        //               WCAP_DATE_2, DateSesEnd, 2);

        //    // SAVE TABLE GL_2 ...Gt.DataTableAllFields; 
        //    GL_2 = Gt.DataTableAllFields;

        //    string SelectionCriteria = "";
        //    int WUniqueRecordId;
        //    bool WIsMatchingDone;
        //    bool WMatched;
        //    int WTransType;
        //    decimal WTransAmount;
        //    string WMatchMask;
        //    string WActionType;
        //    string WResponseCode;
        //    int WMetaExceptionId;
        //    bool WNotInJournal;

        //    TotalDrAfter = 0;
        //    TotalCrAfter = 0;
        //    TotalDrExceptionsAfter = 0;
        //    TotalCrExceptionsAfter = 0;

        //    int I = 0;

        //    while (I <= (GL_2.Rows.Count - 1))
        //    {

        //        // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
        //        WUniqueRecordId = (int)GL_2.Rows[I]["UniqueRecordId"];
        //        WIsMatchingDone = (bool)GL_2.Rows[I]["IsMatchingDone"];
        //        WMatched = (bool)GL_2.Rows[I]["Matched"];
        //        WTransType = (int)GL_2.Rows[I]["TransType"];
        //        WTransAmount = (decimal)GL_2.Rows[I]["TransAmount"];
        //        WMatchMask = (string)GL_2.Rows[I]["MatchMask"];
        //        WActionType = (string)GL_2.Rows[I]["ActionType"];
        //        WResponseCode = (string)GL_2.Rows[I]["ResponseCode"];
        //        WMetaExceptionId = (int)GL_2.Rows[I]["MetaExceptionId"];
        //        WNotInJournal = (bool)GL_2.Rows[I]["NotInJournal"];

        //        if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
        //        {
        //            // it is in journal 
        //            if (WTransType == 11)
        //            {
        //                TotalDrAfter = TotalDrAfter + WTransAmount;
        //            }
        //            if (WTransType == 23)
        //            {
        //                TotalCrAfter = TotalCrAfter + WTransAmount;
        //            }
        //        }

        //        if (WIsMatchingDone == true & WMatched == false)
        //        {
        //            // Get all exceptions in Journal and not 
        //            Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", WUniqueRecordId, WActionType);

        //            if (Aoc.RecordFound == true & Aoc.Stage == "03")
        //            {
        //                // Record fund with Action 
        //                // Everything is OK 

        //                if (Aoc.ShortAccID_1 == "30")
        //                {
        //                    if (Aoc.GL_Sign_1 == "DR")
        //                    {
        //                        TotalDrExceptionsAfter = TotalDrExceptionsAfter + Aoc.DoubleEntryAmt;
        //                    }
        //                    if (Aoc.GL_Sign_1 == "CR")
        //                    {
        //                        TotalCrExceptionsAfter = TotalCrExceptionsAfter + Aoc.DoubleEntryAmt;
        //                    }
        //                }
        //                if (Aoc.ShortAccID_2 == "30")
        //                {
        //                    if (Aoc.GL_Sign_2 == "DR")
        //                    {
        //                        TotalDrExceptionsAfter = TotalDrExceptionsAfter + Aoc.DoubleEntryAmt;
        //                    }
        //                    if (Aoc.GL_Sign_2 == "CR")
        //                    {
        //                        TotalCrExceptionsAfter = TotalCrExceptionsAfter + Aoc.DoubleEntryAmt;
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                // Although Exception no record with action found 
        //                // 
        //                if (Aoc.RecordFound == true & Aoc.Stage != "03")
        //                    MessageBox.Show("Exception for UniqueRecordId.." + WUniqueRecordId.ToString() + Environment.NewLine
        //                    + "Not settled - no action was taken"
        //                    );
        //                if (Aoc.RecordFound == false
        //                    & WActionType == "00"
        //                    )
        //                    MessageBox.Show("Exception for UniqueRecordId.." + WUniqueRecordId.ToString() + Environment.NewLine
        //                    + "No action was taken"
        //                    );
        //                if (Aoc.RecordFound == false
        //                    & WActionType == "04"
        //                    )
        //                    MessageBox.Show("Exception for UniqueRecordId.." + WUniqueRecordId.ToString() + Environment.NewLine
        //                    + "Was Forced Matched"
        //                    );
        //                if (Aoc.RecordFound == false
        //                    & WActionType == "05"
        //                    )
        //                    MessageBox.Show("Exception for UniqueRecordId.." + WUniqueRecordId.ToString() + Environment.NewLine
        //                    + "Was Moved To Disputes"
        //                    );
        //                if (Aoc.RecordFound == false
        //                    & WActionType == "06"
        //                    )
        //                    MessageBox.Show("Exception for UniqueRecordId.." + WUniqueRecordId.ToString() + Environment.NewLine
        //                    + "Was Moved To Pool"
        //                    );
        //            }


        //        }

        //        I = I + 1;
        //    }



        //    decimal InGLBalance;
        //    decimal CalculatedGL;
        //    decimal DifferenceInGl;

        //    textBoxDebitsAfterLoading.Text = TotalDrAfter.ToString("#,##0.00");
        //    textBoxCreditsAfterLoading.Text = TotalCrAfter.ToString("#,##0.00");
        //    textBoxDrDiscrepanciesAfterLoading.Text = TotalDrExceptionsAfter.ToString("#,##0.00");
        //    textBoxCrDiscrepanciesAfterLoading.Text = TotalCrExceptionsAfter.ToString("#,##0.00");

        //    if (decimal.TryParse(textBoxInAmount.Text, out CashInAmount))
        //    {
        //    }
        //    if (decimal.TryParse(textBoxInGLBalance.Text, out InGLBalance))
        //    {
        //        // This OK
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please enter correct GL Balance");
        //    }
        //    // LOADED AMT - Withdrawls after laoding + Deposits After loading
        //    // + DR Discrepancies + CR Discrepancies = Closing Balance
        //    CalculatedGL = CashInAmount
        //                   - TotalDrAfter
        //                   + TotalCrAfter
        //                   - TotalDrExceptionsAfter
        //                   + TotalCrExceptionsAfter;

        //    DifferenceInGl = InGLBalance - CalculatedGL;

        //    textBoxDifferenceInGL.Text = DifferenceInGl.ToString("#,##0.00");

        //    if (DifferenceInGl != 0)
        //    {
        //        labelAlert.Show();

        //        Usi.ReadSignedActivityByKey(WSignRecordNo);
        //        Usi.WFieldNumeric12 = 41;
        //        Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        //    }
        //    else
        //    {
        //        labelAlert.Hide();
        //    }

        //}
        decimal InGLBalance_1;
        // Show GL From The Moment of loading till yesterdays CutOff - One day before today 
        private void buttonGL_1_Click(object sender, EventArgs e)
        {

            if (decimal.TryParse(textBoxEnteredGL.Text, out InGLBalance_1))
            {
                // This OK
            }
            else
            {
                MessageBox.Show("Please enter valid GL Balance");
                return;
            }

            // Keep it for Back amd Foreward 
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cit_Over = InGLBalance_1;  // Kept here for foreward and backward 

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);



            ShowGLFromBooks();


        }
        // Show GL difference Of GL amount as in Bank's books
        private void ShowGLFromBooks()
        {
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
            if (Na.Cit_Over != 0)
            {
                textBoxEnteredGL.Text = Na.Cit_Over.ToString();
                InGLBalance_1 = Na.Cit_Over;
            }

            //panel5.Show();

            // *************GL FOR BDC*****************JOURNAL*************
            // Do this for previous Cap Date and keep table for Ejournal
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE(WAtmNo,
                       WCAP_DATE_2, DateSesStart, 2);

            // SAVE TABLE GL_1 ...Gt.DataTableAllFields; 
            GL_1 = Gt.DataTableAllFields;

            // GET THE TOTALS OUT OF THE TABLE
            string SelectionCriteria = "";
            int WUniqueRecordId;
            bool WIsMatchingDone;
            bool WMatched;
            int WTransType;
            decimal WTransAmount;
            string WMatchMask;
            string WActionType;
            string WResponseCode;
            int WMetaExceptionId;
            bool WNotInJournal;

            decimal TotalDrAfter_1 = 0;
            decimal TotalCrAfter_1 = 0;
            //TotalDrExceptionsAfter = 0; // actions has been take to exceptions 
            // Therefore are included in GL Account
            //TotalCrExceptionsAfter = 0;

            int I = 0;

            while (I <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WUniqueRecordId = (int)GL_1.Rows[I]["UniqueRecordId"];
                WIsMatchingDone = (bool)GL_1.Rows[I]["IsMatchingDone"];
                WMatched = (bool)GL_1.Rows[I]["Matched"];
                WTransType = (int)GL_1.Rows[I]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[I]["TransAmount"];
                WMatchMask = (string)GL_1.Rows[I]["MatchMask"];
                WActionType = (string)GL_1.Rows[I]["ActionType"];
                WResponseCode = (string)GL_1.Rows[I]["ResponseCode"];
                WMetaExceptionId = (int)GL_1.Rows[I]["MetaExceptionId"];
                WNotInJournal = (bool)GL_1.Rows[I]["NotInJournal"];

                //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                //{
                // it is in journal 
                if (WTransType == 11)
                {
                    TotalDrAfter_1 = TotalDrAfter_1 + WTransAmount;
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_1 = TotalCrAfter_1 + WTransAmount;
                }
                //}

                I = I + 1;
            }


            decimal CalculatedGL_1;
            decimal DifferenceInGl_1;


            int WFunction = 1;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 

            textBox2.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
            textBox3.Text = TotalDrAfter_1.ToString("#,##0.00");
            textBox4.Text = TotalCrAfter_1.ToString("#,##0.00");


            // LOADED AMT - Withdrawls after laoding + Deposits After loading
            // + DR Discrepancies + CR Discrepancies = Closing Balance
            CalculatedGL_1 = Na.Balances1.OpenBal
                           - TotalDrAfter_1
                           + TotalCrAfter_1
                            ;

            textBox5.Text = CalculatedGL_1.ToString("#,##0.00");

            DifferenceInGl_1 = InGLBalance_1 - CalculatedGL_1;

            textBoxDifference.Text = DifferenceInGl_1.ToString("#,##0.00");

            if (DifferenceInGl_1 != 0)
            {
                labelAlertGL_1.Show();

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.WFieldNumeric12 = 41;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                WFunction = 2;
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);

                Na.GL_Balance = DifferenceInGl_1; // We put here the GL difference

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            }
            else
            {
                labelAlertGL_1.Hide();
            }

            ///
            /////**************************
            ///****************************
             // *************GL FOR BDC*****************IST*************
            // Do this for previous Cap Date and keep table for IST
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_IST(WAtmNo,
                       WCAP_DATE_2, DateSesStart, 2);

            // SAVE TABLE GL_1 ...Gt.DataTableAllFields; 
            GL_1 = Gt.DataTableAllFields;


            I = 0;

            decimal TotalDrAfter_2 = 0;
            decimal TotalCrAfter_2 = 0;

            while (I <= (GL_1.Rows.Count - 1))
            {

                WTransType = (int)GL_1.Rows[I]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[I]["TransAmt"];

                WResponseCode = (string)GL_1.Rows[I]["ResponseCode"];

                if (WTransType == 11)
                {
                    TotalDrAfter_2 = TotalDrAfter_2 + WTransAmount;
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_2 = TotalCrAfter_2 + WTransAmount;
                }
                //}

                I = I + 1;
            }

            decimal InGLBalance_2;
            decimal CalculatedGL_2;
            decimal DifferenceInGl_2;

            WFunction = 1;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 

            textBox11.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
            textBox12.Text = TotalDrAfter_2.ToString("#,##0.00");
            textBox13.Text = TotalCrAfter_2.ToString("#,##0.00");

            // LOADED AMT - Withdrawls after laoding + Deposits After loading
            // + DR Discrepancies + CR Discrepancies = Closing Balance
            CalculatedGL_2 = Na.Balances1.OpenBal
                           - TotalDrAfter_2
                           + TotalCrAfter_2
                            ;

            textBox14.Text = CalculatedGL_2.ToString("#,##0.00");

            DifferenceInGl_2 = InGLBalance_1 - CalculatedGL_2;

            textBox15.Text = DifferenceInGl_2.ToString("#,##0.00");

            if (DifferenceInGl_2 != 0)
            {
                labeldifIST.Show();

                //Usi.ReadSignedActivityByKey(WSignRecordNo);

                //Usi.WFieldNumeric12 = 41;

                //Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
            }
            else
            {
                labeldifIST.Hide();
            }

            textBoxDiff_DR.Text = (TotalDrAfter_1 - TotalDrAfter_2).ToString("#,##0.00");
            textBoxDiff_CR.Text = (TotalCrAfter_1 - TotalCrAfter_2).ToString("#,##0.00");

        }

        decimal DepositsCountedInCycle;
        decimal CountedRemainsWithdrawls; 
        // 
        private void ShowGLOfTheReplenishmentCycle()
        {
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
            if (Na.Cit_Short != 0)
            {
                textBoxEnteredCore.Text = Na.Cit_Short.ToString();
                ManualCoreBal = Na.Cit_Short;
            }

            // panel8.Show(); // PANEL FOR AUDI

            // GET THE TOTALS OUT OF THE TABLE
            string SelectionCriteria = "";
            int WUniqueRecordId;
            bool WIsMatchingDone;
            bool WMatched;
            int WTransType;
            decimal WTransAmount;
            string WMatchMask;
            string WActionType;
            string WResponseCode;
            int WMetaExceptionId;
            bool WNotInJournal;

            decimal TotalDrAfter_1 = 0;
            decimal TotalCrAfter_1 = 0;

            decimal TotalDrAfter_2 = 0;
            decimal TotalCrAfter_2 = 0;

            int I;


            //********HERE COMES AUDI*******************
            //
            // *************GL FOR AUDI*****************JOURNAL*************
            //
            // Opening Balance for all AUDI
            textBox30.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
            textBox40.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
            textBox50.Text = Na.Balances1.OpenBal.ToString("#,##0.00");

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_Mpa(WAtmNo,
                      Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            GL_1 = Gt.DataTableAllFields;

            decimal TotalDrAfter_3 = 0;
            decimal TotalCrAfter_3 = 0;

            I = 0;

            while (I <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WUniqueRecordId = (int)GL_1.Rows[I]["UniqueRecordId"];
                WTransType = (int)GL_1.Rows[I]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[I]["TransAmount"];

                if (WTransType == 11)
                {
                    TotalDrAfter_3 = TotalDrAfter_3 + WTransAmount;
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_3 = TotalCrAfter_3 + WTransAmount;
                }

                I = I + 1;
            }

            textBox31.Text = TotalDrAfter_3.ToString("#,##0.00");
            textBox32.Text = TotalCrAfter_3.ToString("#,##0.00");

            decimal Remaining_1 = Na.Balances1.OpenBal
                          - TotalDrAfter_3
                          + TotalCrAfter_3
                           ;
            textBox33.Text = Remaining_1.ToString("#,##0.00");

            // *************GL FOR AUDI*****************IST*************

            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_IST(WAtmNo,
                     Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            GL_1 = Gt.DataTableAllFields;

            // CORE 
            decimal TotalDrAfter_4 = 0;
            decimal TotalCrAfter_4 = 0;
            decimal TotalDrEMR203 = 0;
            decimal TotalCrEMR203 = 0;

            I = 0;

            while (I <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WUniqueRecordId = (int)GL_1.Rows[I]["UniqueRecordId"];
                string WMatchingCateg = (string)GL_1.Rows[I]["MatchingCateg"];
                WTransType = (int)GL_1.Rows[I]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[I]["TransAmt"];

                //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                //{
                // it is in journal 
                if (WTransType == 11)
                {
                    TotalDrAfter_4 = TotalDrAfter_4 + WTransAmount;
                    if (WMatchingCateg == "EMR203")
                    {
                        TotalDrEMR203 = TotalDrEMR203 + WTransAmount;
                    }
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_4 = TotalCrAfter_4 + WTransAmount;
                    if (WMatchingCateg == "EMR203")
                    {
                        TotalCrEMR203 = TotalCrEMR203 + WTransAmount;
                    }
                }
                //}

                I = I + 1;
            }

            decimal Remaining_2 = Na.Balances1.OpenBal
                          - TotalDrAfter_4
                          + TotalCrAfter_4
                           ;


            // FILL UP THE IST COLUMN

            textBox41.Text = TotalDrAfter_4.ToString("#,##0.00");
            textBox42.Text = TotalCrAfter_4.ToString("#,##0.00");
            textBox43.Text = Remaining_2.ToString("#,##0.00");
           
            // *************GL FOR AUDI**************COUNTED****************

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
            
            CountedRemainsWithdrawls = Na.Balances1.CountedBal;
            decimal WithdrawlsInCycle = Na.Balances1.OpenBal - CountedRemainsWithdrawls;

            textBox51.Text = WithdrawlsInCycle.ToString("#,##0.00");  // Withdrawls

            RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();
            string WCcy = "EGP";
            SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);
            // HERE ARE THE COUNTED DEPOSITS
            DepositsCountedInCycle = SM.TotalCassetteAmountCount;
            textBox52.Text = DepositsCountedInCycle.ToString("#,##0.00");

            decimal Remaining_3 = Na.Balances1.OpenBal - WithdrawlsInCycle + DepositsCountedInCycle;
            textBox53.Text = Remaining_3.ToString("#,##0.00");  // Remains counted 

            // FIND COREBANKING TOTAL
            // NOTE THAT ERM203 is missing and contains the Master deposits and the prepaid. 
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_COREBANKING(WAtmNo,
                     Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            GL_1 = Gt.DataTableAllFields;

            // CORE 
            decimal TotalDrAfter_5 = 0;
            decimal TotalCrAfter_5 = 0;

            I = 0;

            while (I <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WUniqueRecordId = (int)GL_1.Rows[I]["UniqueRecordId"];
                WTransType = (int)GL_1.Rows[I]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[I]["TransAmt"];

                //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                //{
                // it is in journal 
                if (WTransType == 11)
                {
                    TotalDrAfter_5 = TotalDrAfter_5 + WTransAmount;
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_5 = TotalCrAfter_5 + WTransAmount;
                }
                //}

                I = I + 1;
            }

            decimal CoreBankingBal = Na.Balances1.OpenBal
                          - TotalDrAfter_5
                          + TotalCrAfter_5
                           ;

            // Other Text 
            textBox64.Text = CoreBankingBal.ToString("#,##0.00"); // COREBANKING WITHOUT PREPAID AND MASTERCARD DEPOSITS
            textBox65.Text = TotalDrEMR203.ToString("#,##0.00");
            textBox66.Text = TotalCrEMR203.ToString("#,##0.00");
            // Correct the Remaining_3
            decimal CorrectedCore = CoreBankingBal - TotalDrEMR203 + TotalCrEMR203;

            // ASSIGN CORE
            textBox34.Text = CorrectedCore.ToString("#,##0.00");
            textBox44.Text = CorrectedCore.ToString("#,##0.00");
            if (ManualCoreBal == 0)
            {
                textBox54.Text = CorrectedCore.ToString("#,##0.00");
                textBox55.Text = (Remaining_3 - CorrectedCore).ToString("#,##0.00");
            }
            else
            {
                textBox54.Text = ManualCoreBal.ToString("#,##0.00");
                textBox55.Text = (Remaining_3 - ManualCoreBal).ToString("#,##0.00");
            }


            // ASSIGN DIFF
            textBox35.Text = (Remaining_1 - CorrectedCore).ToString("#,##0.00");
            textBox45.Text = (Remaining_2 - CorrectedCore).ToString("#,##0.00");

            if (ViewWorkFlow == true)
            {
                // DO NOT UPDATE
            }
            else
            {
                // UPDATE 
                RRDMSessionsDataCombined Sc = new RRDMSessionsDataCombined();

                string WSelectionCriteria = " WHERE AtmNo='" + WAtmNo + "' "
                                            + " AND SesNo=" + WSesNo;
                Sc.ReadSessionsDataCombinedBySelectionCriteria(WSelectionCriteria);
                //Sc.OpenBal2
                //Sc.Remaining2
                //Sc.Excess2;
                //Sc.Shortage2;

                Sc.OpenBal2 = Na.Balances1.OpenBal;
                Sc.WithDrawls2 = WithdrawlsInCycle; // 
                Sc.Deposits2 = DepositsCountedInCycle; //
                Sc.Remaining2 = Remaining_3;
                if (ManualCoreBal == 0)
                {
                    Sc.GL_BalanceFromCore2 = CorrectedCore;
                }
                else
                {
                    Sc.GL_BalanceFromCore2 = ManualCoreBal;
                }

                Sc.Excess2 = 0;
                Sc.Shortage2 = 0;

                if (Remaining_3 > Sc.GL_BalanceFromCore2)
                {
                    Sc.Excess2 = Remaining_3 - Sc.GL_BalanceFromCore2;
                }

                if (Remaining_3 < Sc.GL_BalanceFromCore2)
                {
                    Sc.Shortage2 = Sc.GL_BalanceFromCore2 - Remaining_3;
                }

                if (Remaining_3 != Sc.GL_BalanceFromCore2)
                {
                    if (Sc.Excess2 != 0)
                    {
                        Sc.Remark2 = "There Is Excess";
                    }
                    if (Sc.Shortage2 != 0)
                    {
                        Sc.Remark2 = "There Is Shortage";
                    }
                }
                else
                {
                    Sc.Remark2 = "No difference";
                }


                Sc.CapturedCards2 = Na.CaptCardsCount;
                //
                // UPDATE THE RECORD based on the sequence number 
                //
                Sc.Update_SessionsDataCombined(Sc.SeqNo);

            }



        }

        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances(decimal InCashInAmount)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(WAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            // Make transaction if CIT
            if (HybridRepl == false)
            {
                // UPDATE 
                RRDMSessionsDataCombined Sc = new RRDMSessionsDataCombined();

                string WSelectionCriteria = " WHERE AtmNo='" + WAtmNo + "' "
                                            + " AND SesNo=" + WSesNo;
                Sc.ReadSessionsDataCombinedBySelectionCriteria(WSelectionCriteria);
                //Sc.OpenBal2 //****  This is the old one 
                //Sc.Remaining2
                //Sc.Excess2;
                //Sc.Shortage2;

                //Sc.OpenBal2 = Na.Balances1.OpenBal;
                //Sc.WithDrawls2 = WithdrawlsInCycle; // 
                //Sc.Deposits2 = DepositsCountedInCycle; //
                //Sc.Remaining2 = Remaining_3;
                //if (ManualCoreBal == 0)
                //{
                //    Sc.GL_BalanceFromCore2 = CorrectedCore;
                //}
                //else
                //{
                //    Sc.GL_BalanceFromCore2 = ManualCoreBal;
                //}
                
                // Create 
                // load transaction for CIT and Bank

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WActionId;
                // string WUniqueRecordIdOrigin ;
                int WUniqueRecordId;
                string WCcy;
                decimal DoubleEntryAmt;

                // Delete all previous 
                WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo;
                WActionId = "15";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

                WActionId = "18";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

                WActionId = "19";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

                WActionId = "20";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

                WActionId = "21";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

                //
                // THIS IS THE SECOND LEG ENTRY FOR AUDI LOADING
                //
                WActionId = "15"; // 15_CREDIT Intermediate/DR_AtmCash (LOAD)
                //15.1_DR Transitory/CR Intermediate
                //15.2_DR ATM Cash/CR Transitory

                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InCashInAmount;
                string WMaker_ReasonOfAction = "Load ATM - Second Leg";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;

                // REMAINS FOR CIT ACTIONS 
                WActionId = "18"; // 18_DR CIT/CR Intermediate
                
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = CountedRemainsWithdrawls + DepositsCountedInCycle;
                DoubleEntryAmt = Sc.Remaining2;
                WMaker_ReasonOfAction = "DR CIT";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;

                // REMAINS FOR ATM CASH
                WActionId = "19"; // 19_CR ATM Cash with the Remain+Deposits

                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = Sc.GL_BalanceFromCore2;
                WMaker_ReasonOfAction = "CR ATM CASH";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;

                if (Sc.Excess2 > 0)
                {
                    // Make Excess Entries
                    WActionId = "20"; // 20_DR Intermediate/CR Excess

                    WUniqueRecordId = WSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Sc.Excess2;
                    WMaker_ReasonOfAction = "CR ATM CASH";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }
                if (Sc.Shortage2 > 0)
                {
                    // Make Shortage Entries
                    WActionId = "21"; // 21_DR Shortage/CR Intermediate

                    WUniqueRecordId = WSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Sc.Shortage2;
                    WMaker_ReasonOfAction = "CR ATM CASH";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

            }

        }
        // GL TXNS 
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
        decimal JournalAmt;
        private void textBoxInAmount_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxInAmount.Text, out CashInAmount))
            {
            }
            if (decimal.TryParse(textBoxJournal.Text, out JournalAmt))
            {
            }
            textBoxDiffFromJournal.Text = (CashInAmount - JournalAmt).ToString("#,##0.00");
            if (CashInAmount != JournalAmt)
            {
                // textBoxAlert.Show();
                labelAlertForLoading.Show();
            }
            else
            {
                // textBoxAlert.Hide();
                labelAlertForLoading.Hide();
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep5_Updated = false;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }

        // journal change 
        private void textBoxJournal_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxInAmount.Text, out CashInAmount))
            {
            }
            if (decimal.TryParse(textBoxJournal.Text, out JournalAmt))
            {
            }
            textBoxDiffFromJournal.Text = (CashInAmount - JournalAmt).ToString("#,##0.00");
            if (CashInAmount != JournalAmt)
            {
                // textBoxAlert.Show();
                labelAlertForLoading.Show();
            }
            else
            {
                // textBoxAlert.Hide();
                labelAlertForLoading.Hide();
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep5_Updated = false;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }
        // Show Descrepancies this cycle 
        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2
                                                  );
            NForm80b2.Show();
        }
        // see the DR 
        private void buttonDrIst_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 11);

            NForm78D_ATMRecords.Show();
        }
        // see the 
        private void buttonCRIst_Click(object sender, EventArgs e)
        {
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId,
                WTableId, WAtmNo, DateSesStart, WCAP_DATE_2, WSesNo, NullPastDate, 12 );

            NForm78D_ATMRecords.Show();
        }
        decimal ManualCoreBal;
        bool ShowAudi; 
        private void buttonShowBalances_Click(object sender, EventArgs e)
        {

            if (decimal.TryParse(textBoxEnteredCore.Text, out ManualCoreBal))
            {
                // This OK
            }
            else
            {
                MessageBox.Show("Please enter valid GL Balance");
                return;
            }


            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Na.Cit_Short = ManualCoreBal;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            ShowGLOfTheReplenishmentCycle();

            ShowAudi = true; 


        }
        // Default Corebanking
        private void checkBoxDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDefault.Checked == true)
            {
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                textBoxEnteredCore.Text = (Na.Balances1.MachineBal + DepositsCountedInCycle).ToString();

                ShowAudi = false;
            }
        }
// ALL Actions 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WSelectionCriteria;

            WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo
               + " AND (OriginWorkFlow ='Replenishment' OR OriginWorkFlow ='Dispute') ";

            Aoc.ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            //string WUniqueRecordIdOrigin = "Master_Pool";

            Form14b_All_Actions NForm14b_All_Actions;
            int WMode = 3; // Actions 
            NForm14b_All_Actions = new Form14b_All_Actions(WSignedId, WOperator, Aoc.TableActionOccurances_Small, WMode);
            NForm14b_All_Actions.ShowDialog();
        }
// Withdrawls JNL 
        private void buttonDR_JNL_Click(object sender, EventArgs e)
        {
           
            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd
                                                                                                        , WSesNo, NullPastDate, 2);

            NForm78D_ATMRecords.ShowDialog();
        }
// Deposits JNL 
        private void buttonCR_JRN_Click(object sender, EventArgs e)
        {
            string WTableId = "Atms_Journals_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd
                                                                                                          , WSesNo, NullPastDate, 3);

            NForm78D_ATMRecords.ShowDialog();
            //Form38_CDM NForm38_CDM; 
            //NForm38_CDM = new Form38_CDM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            //NForm38_CDM.Show();
        }
// DR CORE
        private void buttonDR_COR_Click(object sender, EventArgs e)
        {
            if (WOperator == "BCAIEGCX")
            {
                // Call for Bank de Caire
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "Flexcube";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd,
                                         WSesNo, NullPastDate, 2);

                NForm78D_ATMRecords.Show();
            }
            else
            {
                // Call For Corebanking 
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "COREBANKING";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd,
                                      WSesNo, NullPastDate, 2);

                NForm78D_ATMRecords.Show();
            }
        }
// CR CORE
        private void buttonCR_COR_Click(object sender, EventArgs e)
        {
            if (WOperator == "BCAIEGCX")
            {
                // Call for Bank de Caire
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "Flexcube";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd,
                                               WSesNo, NullPastDate, 3);

                NForm78D_ATMRecords.Show();
            }
            else
            {
                // Call For Corebanking 
                if (WSesNo == 0)
                {
                    MessageBox.Show("Nothing to show");
                    return;
                }

                string WTableId = "COREBANKING";

                Form78d_ATMRecords NForm78D_ATMRecords;
                NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateSesStart, DateSesEnd,
                                    WSesNo, NullPastDate, 3);

                NForm78D_ATMRecords.Show();
            }
        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{

    public partial class UCForm51b_IST_Based : UserControl
    {
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        // Working Fields 
        bool WViewFunction;
        bool WAuthoriser;
        bool WRequestor;

        bool NormalProcess;

        int WFunction;

        bool ViewWorkFlow;
        string WMode;

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        bool AudiType;

        bool IsRecycle;

        bool Exceptions;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Activate Class 

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();

        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMRepl_SupervisorMode_Details Sm = new RRDMRepl_SupervisorMode_Details();

        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();


        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //bool IsFrom_Excel;

        //
        //int WDifStatus;
        bool FOREX_Deposits;
        DateTime DateTmSesStart;
        DateTime DateTmSesEnd;
        bool ToxicJournal;

        decimal OpeningBal;
        decimal TotalDebit_IST;
        decimal TotalCredit_IST;

        decimal TotalReversals_IST;

        bool InternalChange;
        bool Show_No_Reversals;
        bool FirstCycle;

        decimal Corrected_DR_IST;

        decimal Remains_IST_DR;
        decimal Remains_IST_CR;

        decimal LoadedAmount;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
        bool W_IsFromExcel; 

        public void UCForm51bPar(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo, bool IsFromExcel)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            W_IsFromExcel = IsFromExcel; 

            InitializeComponent();

            IsRecycle = false;

            InternalChange = false;

            //FirstCycle = true; 
            //panel_JournalInfo.BackColor = System.Drawing.Color.LightGray; 

            //radioButtonShow.Checked = true; 


            //************************************************************
            WViewFunction = false; // 54
            WAuthoriser = false;   // 55
            WRequestor = false;    // 56 
            NormalProcess = false;

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54) WViewFunction = true;// ViewOnly 
            if (Usi.ProcessNo == 55) WAuthoriser = true;// Authoriser from author management          
            if (Usi.ProcessNo == 56) WRequestor = true; // Requestor

            if (WViewFunction || WAuthoriser || WRequestor)
            {
                NormalProcess = false;
                buttonUpdate.Hide();
                linkLabelCorrectTheCycle.Hide();

                textBox_Input_Amnt.Enabled = false;
                textBoxCitInputDeposits.Enabled = false;

                textBoxJournal.Enabled = false;
                textBoxInAmount_Excel_Amt.Enabled = false;
            }
            else
            {
                NormalProcess = true;
                buttonUpdate.Enabled = true;

                textBox_Input_Amnt.Enabled = true;
                textBoxCitInputDeposits.Enabled = true;

                textBoxJournal.Enabled = true;
                textBoxInAmount_Excel_Amt.Enabled = true;
            }


            //------------------------------------------------------------
            if (WAuthoriser || WRequestor)
            {
                bool Reject = false;
                Ap.GetMessageOne(WAtmNo, WSesNo, "Replenishment", WAuthoriser, WRequestor, Reject);
                guidanceMsg = Ap.MessageOut;
            }


            //************************************************************
            // Initalise input 
            InputRemainAmt = 0;
            InputDepositsAmt = 0;

            // *********** WE KEEP THE FOLLOWING IN Na
            //Na.ReplAmountTotal ... we keep the openning balance if changed
            // textBox_Input_Amnt.Text = Na.Cit_UnloadedCounted.ToString("##0.00");
            // textBoxCitInputDeposits.Text = Na.GL_Bal_Repl_Adjusted.ToString("##0.00");
            //************************************************************

            SetScreen();
        }

        //*************************************
        // Set Screen
        //*************************************
        decimal JournalCassettes;
        decimal JournalExpected;
        decimal SM_RemainingDep;
        decimal SM_TotalDeposits;
        decimal JournalPresenter;

        public void SetScreen()
        {
            // FOR BANK DE CAIRE 
            OpeningBal = 850000; // FIXED
            string ParId2 = "948";
            string OccurId2 = "1"; // 
            //RRDMGasParameters Gp = new RRDMGasParameters(); 
            Gp.ReadParametersSpecificId(WOperator, ParId2, OccurId2, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // CHECK IF RECYCLING
                SM.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                if (SM.RecordFound == true)
                {
                    // Check if Reccyle 
                    if (SM.is_recycle == "Y")
                    {
                        IsRecycle = true;

                        //checkBox_Allow_Balance.Checked = true;
                        labelRecycleNote.Show();

                        labelRemain.Text = "Remains for both" + Environment.NewLine
                                            + "Cassettes+Deposits";
                        //labelDiffCR.Text = "Difference" + Environment.NewLine
                        //                   + "Cassettes+Deposits";
                        labelDeposits.Text = "Deposits for" + Environment.NewLine
                                            + "Recycling";

                        labelRemainCR.Hide();
                        textBoxRemainingCR.Hide();

                        labelDiffCR.Hide();
                        textBoxIST_DiffDep.Hide();

                        //labelGuadingMessage.Show();
                        //panelGuidingInfo.Show(); 
                    }
                    else
                    {
                        checkBox_Allow_Balance.Checked = false;
                        labelRecycleNote.Hide();
                        // labelGuadingMessage.Hide();
                        // panelGuidingInfo.Hide();
                    }
                }
            }
            // GET DATES 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            DateTmSesStart = Ta.SesDtTimeStart;
            DateTmSesEnd = Ta.SesDtTimeEnd;
            if (W_IsFromExcel == true)
            {
                string WSelectionCriteria = " WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo;
                Ce.Read_CIT_Excel_Table_BySelectionCriteria(WSelectionCriteria);

                if (Ce.RecordFound == true)
                {
                    // Input from Excel 
                  
                    WFunction = 2;
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 

                    Na.Cit_UnloadedCounted = Ce.CIT_Total_Returned;

                    Na.GL_Bal_Repl_Adjusted = Ce.CIT_Total_Deposit_Local_Ccy;

                    Na.InUserDate = DateTime.Now;

                    Na.InReplAmount = Ce.CIT_Total_Replenished;

                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                    textBox_Input_Amnt.ReadOnly = true;
                    textBoxCitInputDeposits.ReadOnly = true;
                    textBoxInAmount_Excel_Amt.ReadOnly = true;

                }
                else
                {
                    // Manual input
                   // IsFrom_Excel = false;
                }
            }
           

            //CIT_Total_Returned = (decimal)rdr["CIT_Total_Returned"];
            //CIT_Total_Deposit_Local_Ccy = (decimal)rdr["CIT_Total_Deposit_Local_Ccy"];

            // Populate Header
            labelFromDt.Text = "From: " + Ta.SesDtTimeStart.ToString();
            labelToDt.Text = "To...: " + Ta.SesDtTimeEnd.ToString();
            // For header
            WFunction = 1;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 

            textBoxType1.Text = Convert.ToInt32(Na.Cassettes_1.FaceValue).ToString();
            textBoxType2.Text = Convert.ToInt32(Na.Cassettes_2.FaceValue).ToString();
            textBoxType3.Text = Convert.ToInt32(Na.Cassettes_3.FaceValue).ToString();
            textBoxType4.Text = Convert.ToInt32(Na.Cassettes_4.FaceValue).ToString();

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction);
            if (Na.GL_Balance == 0)
            {
                Na.GL_Balance = OpeningBal; // Keep the Change here
                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);
            }
            else
            {
                Na.GL_Balance = OpeningBal; // Keep the Change here
                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);// 
            }

            // Has FOREX? 

            // Check through IST for FOREX DEPOSITS function for this ATM
            checkBoxHasForex.Checked = false;
            string WTableId = "Switch_IST_Txns";
            int WMode = 15;
            Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);

            int rows = Mgt.DataTableAllFields.Rows.Count;

            if (rows > 0)
            {
                FOREX_Deposits = true;
                checkBoxHasForex.Checked = true;
            }
            else
            {
                WTableId = "tblMatchingTxnsMasterPoolATMs";
                WMode = 16;
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, WMode, 2);
                rows = Mgt.DataTableAllFields.Rows.Count;
                if (rows > 0)
                {
                    FOREX_Deposits = true;
                    checkBoxHasForex.Checked = true;
                }
                else
                {
                    FOREX_Deposits = false;
                }

            }


            // HAS presenter
            //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            int Mode = 1;
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            if (Mpa.TotalSelected > 0)
            {
                // Exceptions to show for replenishment 
                Exceptions = true;
                checkboxHasExceptions.Checked = true;
            }
            else
            {
                Exceptions = false;
                checkboxHasExceptions.Checked = false;
            }



            //if (IsRecycle == true)
            //{
            //    panelDeposits.Show();
            //}
            //else
            //{
            //    panelDeposits.Hide();
            //}


            ToxicJournal = false;
            if (Ta.ProcessMode == -6)
            {
                ToxicJournal = true;
                labelJournalHeading.Text = "JOURNAL Is TOXIC=>Information cannot be shown";
                panel_JournalInfo.Hide();
                //textBoxOpenBal.ReadOnly = false;
                panelWarning.Hide();
                panel_Journal_Excess.Hide();
                labelJournalExcess.Hide();
            }

            // SHOW CIT ID AND NAME
            Ac.ReadAtm(WAtmNo);
            int WAtmsReconcGroup = Ac.AtmsReconcGroup;
            labelGroup.Text = "Belongs to.." + WAtmsReconcGroup.ToString();
            //Ac.CitId

            Us.ReadUsersRecord(Ac.CitId);

            labelCitId.Text = "CIT_ID: " + Ac.CitId;
            labelCIT_NAME.Text = "NAME: " + Us.UserName;

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
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
            Usi.ReadSignedActivityByKey(WSignRecordNo);

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

            // Opening balance 
            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // REPL TO REP IS DONE WITHIN THIS CLASS 
            InternalChange = false;
            if (Na.GL_Balance > 0)
            {
                // Means that manual balance was inputed 
                InternalChange = true;
                checkBox_Allow_Balance.Checked = true;
                OpeningBal = Na.GL_Balance;

            }
            else
            {
                //OpeningBal = Na.Balances1.OpenBal;// this is wrong for recycling

                if (Na.PreSes == 0)
                {

                    Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                    OpeningBal = // After date Cap_date
                                  Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                                + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                                + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                                + Sm.ATM_total4 * Na.Cassettes_4.FaceValue
                                  ;
                    //MessageBox.Show("The Openning Balance is not available" + Environment.NewLine
                    //   + "Input Open Balance Manually" +
                    //    "");

                }
                else
                {
                    Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                    OpeningBal = // After date Cap_date
                                  Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                                + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                                + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                                + Sm.ATM_total4 * Na.Cassettes_4.FaceValue
                                  ;
                    //Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, Na.PreSes);
                    //OpeningBal = // After date Cap_date
                    //    //Sm.ATM_cassette1 
                    //              Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                    //            + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                    //            + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                    //            + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue
                    //              ;
                    //textBoxInAmount_Excel_Amt.Text = LoadedAmount.ToString("#,##0.00");
                    //textBoxJournal.Text = LoadedAmount.ToString("#,##0.00");
                }


            }

            //FROM IST
            WMode = 1;
            WTableId = "Switch_IST_Txns";

            if (WMode == 1)
            {
                Mgt.ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(WSignedId, WTableId, WAtmNo, DateTmSesStart, DateTmSesEnd, WMode, 2);

                TotalDebit_IST = Mgt.TotalDebit;
                TotalCredit_IST = Mgt.TotalCredit;
            }

            // FIND THE ONES THAT WERE REVERSED during reconciliation (Actions 91,92) 

            Mode = 5;
            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_Actions_91_92(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 1);

            if (Show_No_Reversals == true)
            {
                Mpa.TotalDebit = 0;
                Mpa.TotalCredit = 0;
            }

            //     public decimal TotalDebit;
            //public decimal TotalCredit;
            TotalReversals_IST = Mpa.TotalDebit;

            Corrected_DR_IST = TotalDebit_IST - TotalReversals_IST;

            if (IsRecycle == true)
            {
                // If Recycle we add all deposits on Opening balance and we subtract all Withdrawls
                // This gives us the remains for both the Cassettes + Deposits 
                Remains_IST_DR = (OpeningBal + (TotalCredit_IST + Mpa.TotalCredit)) - Corrected_DR_IST;
                // REMAINS CANNOT BE DEFINED FROM IST FOR DEPOSITS

            }
            else
            {
                // Not Recycle 
                Remains_IST_DR = OpeningBal - Corrected_DR_IST;

                Remains_IST_CR = TotalCredit_IST + Mpa.TotalCredit;
                //Remains_IST_CR 
            }
            // INPUT FIGURES ... LEAVE IT HERE
            textBox_Input_Amnt.Text = Na.Cit_UnloadedCounted.ToString("#,##0.00");
            textBoxCitInputDeposits.Text = Na.GL_Bal_Repl_Adjusted.ToString("#,##0.00");

            if (Na.Is_GL_Adjusted == true)
            {
                radioButtonGLBased_Journal.Checked = true;
            }
            ;

            // Populate textBoxes 
            if (OpeningBal != 850000)
            {
                OpeningBal = 850000;
            }

            textBoxOpenBal.Text = OpeningBal.ToString("#,##0.00"); // Leave the opening balance here 

            textBoxDR_IST.Text = TotalDebit_IST.ToString("#,##0.00");

            textBoxReversals.Text = TotalReversals_IST.ToString("#,##0.00");

            textBoxReversals.Show();

            textBoxRemaining.Text = Remains_IST_DR.ToString("#,##0.00");

            if (Remains_IST_DR < 0)
            {
                labelInvalid.Show();
            }
            else
            {
                labelInvalid.Hide();
            }
            // TEXTBOX for Deposits
            //textBoxRemainingCR.Text = Remains_IST_CR.ToString("#,##0.00");
            textBoxDeposits.Text = TotalCredit_IST.ToString("#,##0.00");

            textBoxReversalsDep.Text = Mpa.TotalCredit.ToString("#,##0.00"); // Not defined yet 

            if (IsRecycle == true)
            {
                textBoxRemainingCR.Text = "Not Defined in IST";
                //textBoxRemainingCR.Text = textBoxCitInputDeposits.Text;
            }
            else
            {
                textBoxRemainingCR.Text = (Remains_IST_CR).ToString("#,##0.00");
            }

            // Journal Section 

            textBoxMachineBal.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
            JournalCassettes = Na.Balances1.MachineBal;
            JournalPresenter = Na.Balances1.PresenterValue;
            textBoxPresenter.Text = JournalPresenter.ToString("#,##0.00");

            JournalExpected = (Na.Balances1.MachineBal + Na.Balances1.PresenterValue);

            textBoxExpected.Text = JournalExpected.ToString("#,##0.00");

            GetDeposits();

            if (DepositsEGPFound == true)
            {
                // Remaining Deposits for Recycling 
                SM_TotalDeposits = WTotalMoneyDecimal;
                //textBoxTotalDeposits.Text = SM_TotalDeposits.ToString("#,##0.00");
                //textBoxRecycleAmt.Text = WRECYCLEDTotalMoney.ToString("#,##0.00");

                SM_RemainingDep = WTotal_SM_Money;
                textBoxSM_deposits.Text = SM_RemainingDep.ToString("#,##0.00");

            }
            else
            {
                textBoxSM_deposits.Text = "0.00";
            }
            // Haddle input figures
            HandleInput(0);
            //
            // Deal with Money In
            //


            //Sm.Read_SM_Record_For_AddedCash(WAtmNo, NextReplCycle, WCAP_DATE_2); 
            //decimal CashAdded = // After date Cap_date
            //              Sm.Total_cashaddtype1 * Na.Cassettes_1.FaceValue
            //            + Sm.Total_cashaddtype2 * Na.Cassettes_2.FaceValue
            //            + Sm.Total_cashaddtype3 * Na.Cassettes_3.FaceValue
            //            + Sm.Total_cashaddtype4 * Na.Cassettes_4.FaceValue
            //              ;
            //Na.ReadSessionsNotesAndValues(WAtmNo, NextReplCycle, 2);
            //LoadedAmount = Na.Balances1.OpenBal - CashAdded; 

            Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
            LoadedAmount = // After date Cap_date
                          Sm.cashaddtype1 * Na.Cassettes_1.FaceValue
                        + Sm.cashaddtype2 * Na.Cassettes_2.FaceValue
                        + Sm.cashaddtype3 * Na.Cassettes_3.FaceValue
                        + Sm.cashaddtype4 * Na.Cassettes_4.FaceValue
                          ;

            if (LoadedAmount != 850000)
            {
                LoadedAmount = 850000;
            }
            textBoxInAmount_Excel_Amt.Text = LoadedAmount.ToString();
            textBoxJournal.Text = LoadedAmount.ToString();

            textBoxDiffFromJournal_IST.Text = (LoadedAmount - LoadedAmount).ToString("#,##0.00");
            label_Alert.Hide();
        }
        // GET DEPOSITS 
        int WTotalNotes;
        decimal WTotalMoneyDecimal;

        int WRetractTotalNotes;
        decimal WRetractTotalMoney;


        int WRECYCLEDTotalNotes;
        decimal WRECYCLEDTotalMoney;

        int WNCRTotalNotes;
        decimal WNCRTotalMoney;

        int WTotal_SM_Notes;
        decimal WTotal_SM_Money;

        bool DepositsEGPFound;
        private void GetDeposits()
        {
            DepositsEGPFound = false;
            //
            // FIND DEPOSITS EGP FROM JOURNAL READ
            //
            string ParId = "218";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string From_SM = Gp.OccuranceNm;

            if (From_SM == "YES")
            {
                // Find the first fuid to avoid duplication created by two the same journals
                //
                SM.Read_SM_AND_Get_First_fuid(WAtmNo, WSesNo);

                if (SM.RecordFound == true)
                {
                    // Fuid found
                }
                else
                {
                    //MessageBox.Show("No Deposits Found");
                    return;
                }
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


                // Read Table 
                int L = 1;
                int I = 0;


                while (I <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
                {
                    // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                    string TCcy = (string)SM.DataTable_SM_Deposits.Rows[I]["Ccy"];
                    string WCcy = TCcy.Trim();
                    if (WCcy == "" || WCcy != "EGP")
                    {
                        I = I + 1;
                        continue;
                    }

                    DepositsEGPFound = true;
                    // Calculate for all 
                    WTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteNotes"];
                    WTotalMoneyDecimal = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteMoney"];

                    WRetractTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTNotes"];
                    WRetractTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTMoney"];

                    //WTotalNotes = WTotalNotes;
                    //WTotalMoneyDecimal = WTotalMoneyDecimal;
                    //WTotalNotes = WTotalNotes + WRetractTotalNotes;
                    //WTotalMoneyDecimal = WTotalMoneyDecimal + WRetractTotalMoney;

                    WRECYCLEDTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDNotes"];
                    WRECYCLEDTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDMoney"];

                    WNCRTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedNotes"];
                    WNCRTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedMoney"];

                    WTotal_SM_Notes = ((WTotalNotes + WRetractTotalNotes + WRECYCLEDTotalNotes) - WNCRTotalNotes);
                    WTotal_SM_Money = ((WTotalMoneyDecimal + WRetractTotalMoney + WRECYCLEDTotalMoney) - WNCRTotalMoney);

                    I = I + 1;
                    // Bring the EGP first 

                }
            }
        }

        decimal InputRemainAmt;
        decimal InputOpenningBalance;
        decimal ISTDifferenceDR;
        decimal ISTDifferenceCR;
        decimal JournalDifferenceDR;
        decimal JournalDifferenceCR;
        decimal TotalJournalDifferenceForRecycling;
        // Input Withdrawls 
        private void textBox_Input_Amnt_TextChanged(object sender, EventArgs e)
        {
            InputRemainAmt = 0;
            HandleInput(1);
        }
        decimal InputDepositsAmt;
        // Input Deposits 
        private void textBoxCitInputDeposits_TextChanged(object sender, EventArgs e)
        {
            InputDepositsAmt = 0;
            HandleInput(2);
        }
        // OPEN BALANCE FOR TOXIC
        private void textBoxOpenBal_TextChanged(object sender, EventArgs e)
        {

            //if (checkBox_Allow_Balance.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxOpenBal.Text, out InputOpenningBalance))
            //    {
            //        // continue 
            //        OpeningBal = InputOpenningBalance;
            //    }
            //    else
            //    {
            //        if (textBoxOpenBal.Text == "")
            //        {

            //        }
            //        else
            //        {
            //            MessageBox.Show("Please enter valid Openning Balance amount in EGP ");
            //            return;
            //        }

            //    }

            //    // SetScreen();
            //}



        }

        // IST
        decimal T_ExcessDR_IST;
        decimal T_ExcessCR_IST;
        decimal T_ShortageDR_IST;
        decimal T_ShortageCR_IST;

        decimal Total_T_Excess_IST;
        decimal Total_T_Shortage_IST;

        decimal NetTotalExcess_IST;
        decimal NetTotalShortage_IST;
        // JLN
        decimal T_ExcessDR_JNL;
        decimal T_ExcessCR_JNL;
        decimal T_ShortageDR_JNL;
        decimal T_ShortageCR_JNL;

        decimal Total_T_Excess_JNL;
        decimal Total_T_Shortage_JNL;

        decimal NetTotalExcess_JNL;
        decimal NetTotalShortage_JNL;

        private void HandleInput(int Origin)
        {
            // Origin == 0 takes both Input remain and Deposits
            // if 1 is input cassettes
            // If 2 is deposits only 

            // DRs 
            if (Origin == 1 || Origin == 0)
            {
                if (decimal.TryParse(textBox_Input_Amnt.Text, out InputRemainAmt))
                {
                    // continue
                }
                else
                {
                    if (textBox_Input_Amnt.Text == "")
                    {

                    }
                    else
                    {
                        MessageBox.Show("Please enter valid remain cassettes amount in EGP ");
                        return;
                    }

                }
            }

            //
            // Deposits 
            //
            if (Origin == 2 || Origin == 0)
            {
                if (decimal.TryParse(textBoxCitInputDeposits.Text, out InputDepositsAmt))
                {
                    // continue 
                }
                else
                {
                    if (textBoxCitInputDeposits.Text == "")
                    {

                    }
                    else
                    {
                        MessageBox.Show("Please enter valid remain Depoits amount in EGP ");
                        return;
                    }

                }
            }
            // SAVE AMOUNTS 
            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 

            if (Origin == 1)
            {
                Na.Cit_UnloadedCounted = InputRemainAmt;
            }

            if (Origin == 2)
            {
                Na.GL_Bal_Repl_Adjusted = InputDepositsAmt;
            }
            //if (ToxicJournal == true)
            //{
            //if (checkBox_Allow_Balance.Checked == true)
            //{
            //    Na.GL_Balance = InputOpenningBalance; // Keep the Change here
            //}
            //else
            //{
            //    Na.GL_Balance = 0;
            //}

            //}

            Na.InUserDate = DateTime.Now;

            Na.InReplAmount = CashInAmount;

            // Na.InsuranceAmount = Ac.InsurOne;

            //Na.ReplAmountTotal = CashInAmount;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);


            // DR Difference 
            if (IsRecycle == true)
            {
                ISTDifferenceDR = (InputRemainAmt + InputDepositsAmt) - Remains_IST_DR;
            }
            else
            {
                ISTDifferenceDR = InputRemainAmt - Remains_IST_DR;
            }

            // CR Difference --- DEPOSITS 
            if (IsRecycle == true)
            {
                // AMOUNT CANNOT BE DEFINED 
                // ISTDifferenceCR = (InputRemainAmt + InputRemainDepositsAmt) - Remains_IST;
            }
            else
            {
                ISTDifferenceCR = InputDepositsAmt - Remains_IST_CR;
            }

            textBoxDifference.Text = ISTDifferenceDR.ToString("#,##0.00");
            // Initialise IST
            decimal tempZero = 0;
            textBoxExcessDR_IST.Text = tempZero.ToString("#,##0.00");
            textBoxShortageDR_IST.Text = tempZero.ToString("#,##0.00");
            textBoxExcessCR_IST.Text = tempZero.ToString("#,##0.00");
            textBoxShortageCR_IST.Text = tempZero.ToString("#,##0.00");
            // Initialise JLN
            textBoxExcessDR_JNL.Text = tempZero.ToString("#,##0.00");
            textBoxShortageDR_JNL.Text = tempZero.ToString("#,##0.00");
            textBoxExcessCR_JNL.Text = tempZero.ToString("#,##0.00");
            textBoxShortageCR_JNL.Text = tempZero.ToString("#,##0.00");
            // NET EXCESS AND SHORTAGE
            T_ExcessDR_IST = 0;
            T_ExcessCR_IST = 0;
            T_ShortageDR_IST = 0;
            T_ShortageCR_IST = 0;

            NetTotalExcess_IST = 0;
            NetTotalShortage_IST = 0;
            // JOURNAL
            T_ExcessDR_JNL = 0;
            T_ExcessCR_JNL = 0;
            T_ShortageDR_JNL = 0;
            T_ShortageCR_JNL = 0;

            NetTotalExcess_JNL = 0;
            NetTotalShortage_JNL = 0;

            // Set the values Excess IST
            if (ISTDifferenceDR > 0)
            {
                textBoxExcessDR_IST.Text = ISTDifferenceDR.ToString("#,##0.00");
                T_ExcessDR_IST = ISTDifferenceDR;
            }

            if (ISTDifferenceDR < 0)
            {
                decimal TempDifference = -ISTDifferenceDR;
                textBoxShortageDR_IST.Text = TempDifference.ToString("#,##0.00");
                T_ShortageDR_IST = TempDifference;
            }
            // SAME For Deposits
            if (ISTDifferenceCR > 0)
            {
                textBoxExcessCR_IST.Text = ISTDifferenceCR.ToString("#,##0.00");
                T_ExcessCR_IST = ISTDifferenceCR;
            }

            if (ISTDifferenceCR < 0)
            {
                decimal TempDifference = -ISTDifferenceCR;
                textBoxShortageCR_IST.Text = TempDifference.ToString("#,##0.00");
                T_ShortageCR_IST = TempDifference;
            }

            // DEFINE NET 
            Total_T_Excess_IST = T_ExcessDR_IST + T_ExcessCR_IST;
            Total_T_Shortage_IST = T_ShortageDR_IST + T_ShortageCR_IST;

            textBoxExcessTotal_IST.Text = Total_T_Excess_IST.ToString("#,##0.00");
            textBoxShortageTotal_IST.Text = Total_T_Shortage_IST.ToString("#,##0.00");

            if (Total_T_Excess_IST > Total_T_Shortage_IST)
            {
                labelExcessORShortage_IST.Text = "Excess";
                NetTotalExcess_IST = Total_T_Excess_IST - Total_T_Shortage_IST;
                textBoxNetResult_IST.Text = NetTotalExcess_IST.ToString("#,##0.00");

            }
            if (Total_T_Shortage_IST > Total_T_Excess_IST)
            {
                labelExcessORShortage_IST.Text = "Shortage";
                NetTotalShortage_IST = Total_T_Shortage_IST - Total_T_Excess_IST;
                textBoxNetResult_IST.Text = NetTotalShortage_IST.ToString("#,##0.00");
            }

            if (Total_T_Shortage_IST == Total_T_Excess_IST)
            {
                labelExcessORShortage_IST.Text = "NONE";
                textBoxNetResult_IST.Text = "0.00";
            }

            if (IsRecycle == true)
            {
                textBoxIST_DiffDep.Text = "Not Defined";
            }
            else
            {
                textBoxIST_DiffDep.Text = ISTDifferenceCR.ToString("#,##0.00");
            }

            // JOURNAL SECTION
            if (IsRecycle == true)
            {

                TotalJournalDifferenceForRecycling = (InputRemainAmt + InputDepositsAmt) - (JournalExpected + SM_RemainingDep);
            }
            else
            {

            }

            // recycling and not recycling 
            textBoxCITInputJournal.Text = InputRemainAmt.ToString("#,##0.00");

            //Journal
            JournalDifferenceDR = InputRemainAmt - JournalCassettes;
            textBoxJournalDifference.Text = JournalDifferenceDR.ToString("#,##0.00");

            // Deposits
            textBoxCitInputDep.Text = InputDepositsAmt.ToString();

            // 
            JournalDifferenceCR = InputDepositsAmt - SM_RemainingDep;
            textBoxDiffDeposits_JLN.Text = JournalDifferenceCR.ToString("#,##0.00");

            // Set the values Excess Journal
            if (JournalDifferenceDR > 0)
            {
                textBoxExcessDR_JNL.Text = JournalDifferenceDR.ToString("#,##0.00");
                T_ExcessDR_JNL = JournalDifferenceDR;
            }

            if (JournalDifferenceDR < 0)
            {
                decimal TempDifference = -JournalDifferenceDR;
                textBoxShortageDR_JNL.Text = TempDifference.ToString("#,##0.00");
                T_ShortageDR_JNL = TempDifference;
            }
            // SAME For Deposits
            if (JournalDifferenceCR > 0)
            {
                textBoxExcessCR_JNL.Text = JournalDifferenceCR.ToString("#,##0.00");
                T_ExcessCR_JNL = JournalDifferenceCR;
            }

            if (JournalDifferenceCR < 0)
            {
                decimal TempDifference = -JournalDifferenceCR;
                textBoxShortageCR_JNL.Text = TempDifference.ToString("#,##0.00");
                T_ShortageCR_JNL = TempDifference;
            }

            // DEFINE NET For JNL
            Total_T_Excess_JNL = T_ExcessDR_JNL + T_ExcessCR_JNL;
            Total_T_Shortage_JNL = T_ShortageDR_JNL + T_ShortageCR_JNL;

            textBoxExcessTotal_JNL.Text = Total_T_Excess_JNL.ToString("#,##0.00");
            textBoxShortageTotal_JNL.Text = Total_T_Shortage_JNL.ToString("#,##0.00");

            if (Total_T_Excess_JNL > Total_T_Shortage_JNL)
            {
                labelExcessORShortage_JNL.Text = "Excess";
                NetTotalExcess_JNL = Total_T_Excess_JNL - Total_T_Shortage_JNL;
                textBoxNetResult_JNL.Text = NetTotalExcess_JNL.ToString("#,##0.00");

            }
            if (Total_T_Shortage_JNL > Total_T_Excess_JNL)
            {
                labelExcessORShortage_JNL.Text = "Shortage";
                NetTotalShortage_JNL = Total_T_Shortage_JNL - Total_T_Excess_JNL;
                textBoxNetResult_JNL.Text = NetTotalShortage_JNL.ToString("#,##0.00");
            }

            if (Total_T_Shortage_JNL == Total_T_Excess_JNL)
            {
                labelExcessORShortage_JNL.Text = "NONE";
                textBoxNetResult_JNL.Text = "0.00";
            }



            bool Diff = false;

            if (IsRecycle == true & TotalJournalDifferenceForRecycling != ISTDifferenceDR)
            {
                Diff = true;
            }
            // Cancel the true if difference is equal to presenter for recycling
            if (IsRecycle == true & JournalPresenter == ISTDifferenceDR)
            {
                Diff = false;
            }

            if (IsRecycle == false & (JournalDifferenceDR != ISTDifferenceDR || JournalDifferenceCR != ISTDifferenceCR))
            {
                Diff = true;
            }
            // Cancel the true if difference is equal to presenter for not recycling 
            if (IsRecycle == false & (JournalPresenter == ISTDifferenceDR || JournalDifferenceCR == ISTDifferenceCR))
            {
                Diff = false;
            }

            // Report Difference 
            if (Diff == true)
            {
                textBoxWarning.Text = "Two mathods differ. Please investigate" + Environment.NewLine
                    + "Check eg if actions have been taken to all TXNs." + Environment.NewLine
                    + "If not all journals are loaded.. " + Environment.NewLine
                    + "then maybe presenter errors are missing"
                    ;
                label8.Show();
                textBoxWarning.Show();
                pictureBox1.BackgroundImage = appResImg.YELLOW_Repl;
                pictureBox1.Show();
            }
            else
            {
                label8.Hide();
                textBoxWarning.Hide();
                pictureBox1.Hide();
            }

        }


        // NOTES 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
            string SearchP4 = "";
            if (ViewWorkFlow == true) WMode = "Read";
            else WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();
            SetScreen();
        }
        // GL Txns 
        private void buttonGLTxns_Click(object sender, EventArgs e)
        {
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

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
        // All Actions Taken 
        private void buttonAllActions_Click(object sender, EventArgs e)
        {
            // RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
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
        // Show IST Txns 
        private void buttonShowIST_TXNs_Click(object sender, EventArgs e)
        {
            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 1);

            NForm78D_ATMRecords.Show();
        }
        // SM Lines 
        private void buttonShowSM_Click(object sender, EventArgs e)
        {
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
        // Show All Discrepancies  
        private void buttonShowErrors_Click(object sender, EventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, DateTmSesStart, DateTmSesEnd, 2
                );
            NForm80b2.Show();
        }
        // UPDATE
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (radioButtonNoShow.Checked == true)
            {
                MessageBox.Show("The Radio Buttom for exclution of Reversals is pressed." + Environment.NewLine
                    + "Not Allowed!!! "
                    );
                return;
            }

            if (InputRemainAmt == 0 & Remains_IST_DR != 0)

            {
                MessageBox.Show("There is no input for DR for updating" + Environment.NewLine
                    + "Please input cassettes counted amounts " + Environment.NewLine
                    + "If zero from CIT then you can continue... "
                    );
                //return;
            }
            // Check if actions Already taken from presenter errors action screen

            string WSelectionCriteria = " WHERE AtmNo = '" + WAtmNo + "' AND ReplCycle =" + WSesNo + " AND ActionId in ('90','92', '95', '96')  AND OriginWorkFlow = 'Replenishment' ";
            Aoc.ReadCheckActionsOccuarnceBySelectionCriteria(WSelectionCriteria);

            if (Aoc.RecordFound == true)
            {
                MessageBox.Show(
                    "You are trying to update with new figures ... but ..." + Environment.NewLine
                    + "You have done an action on the presenter screen." + Environment.NewLine
                    + "Undo action you have taken on Presenter screen" + Environment.NewLine
                    + "Then come back to this screen and press update " + Environment.NewLine
                    + "Only this way the new counted figures will be accepted." + Environment.NewLine
                    + "Then proceed to the next " + Environment.NewLine
                    + "Take action on presenter "
                    );

                MessageBox.Show(
                    "The new figures has not be updated" + Environment.NewLine
                    );

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep1_Updated = true;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                return;
            }

            if (radioButtonGLBased_Journal.Checked == true)
            {
                // Check that note is inserted
                // NOTES for final comment 
                Order = "Descending";
                WParameter4 = "Replenishment closing stage for" + " AtmNo: " + WAtmNo + " SesNo: " + WSesNo;
                WSearchP4 = "";
                Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);

                if (Cn.RecordFound == true)
                {
                    // Notes Inserted 
                    // ALL OK 
                }
                else
                {
                    MessageBox.Show("Please insert note of explanation for Journal selection");
                    return;
                }
            }

            // CREATE GL TRANSACTIONS UNLOADED
            CreateActions_Occurances_Unloaded();

            // CREATE GL TRANSACTIONS DEPOSITS
            if (InputDepositsAmt == 0 & Remains_IST_CR != 0)

            {
                MessageBox.Show("There is no input for Deposits for updating" + Environment.NewLine
                    + "Please input deposits counted amounts" + Environment.NewLine
                    + "If zero from CIT then you can continue... "
                    );
                //return;
            }

            // Create Transactions for deposits 

            //InputDepositsAmt = ISTDifferenceCR 
            CreateActions_OccurancesDeposits(InputDepositsAmt);

            // Disputes handling 
            CreateActions_Occurances_Excess_Short_Disputes(); 


            // CREATE Transactions for Cash in 

            if (CashInAmount > 0)
            {
                CreateActions_Occurances(CashInAmount);
            }


            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 

            Na.Cit_UnloadedCounted = InputRemainAmt;
            Na.GL_Bal_Repl_Adjusted = InputDepositsAmt;

            if (radioButtonGLBased_Journal.Checked == true)
            {
                Na.Is_GL_Adjusted = true;
            }
            else
            {
                Na.Is_GL_Adjusted = false;
            }

            Na.InUserDate = DateTime.Now;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

            // UPDATE STEP

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep1_Updated = true;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            guidanceMsg = "Updating completed. - Move to next step.";
            ChangeBoardMessage(this, e);

            MessageBox.Show("Updating Done!");

        }
        // REFRESH INPUT BALANCE 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (checkBox_Allow_Balance.Checked == true)
            {
                if (decimal.TryParse(textBoxOpenBal.Text, out OpeningBal))
                {
                    if (OpeningBal == 0)
                    {
                        MessageBox.Show("Opening Balance is set to zero. Not allowed");
                        return;
                    }
                    // continue
                    WFunction = 2;
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 
                    Na.GL_Balance = OpeningBal; // Keep the Change here
                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);
                }
                else
                {
                    if (textBoxOpenBal.Text == "")
                    {
                        MessageBox.Show("Opening Balance is blank. Not allowed");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid Open Balance amount in EGP ");
                        return;
                    }

                }
            }

            SetScreen();
        }
        // Loaded Amount excel change 
        decimal CashInAmount;
        decimal JournalAmt;
        private void textBoxInAmount_Excel_Amt_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxInAmount_Excel_Amt.Text, out CashInAmount))
            {
            }

            textBoxInAmount_Excel_Amt.Text = CashInAmount.ToString();

            if (decimal.TryParse(textBoxJournal.Text, out JournalAmt))
            {
            }
            textBoxDiffFromJournal_IST.Text = (CashInAmount - JournalAmt).ToString();
            if (CashInAmount != JournalAmt)
            {
                // textBoxAlert.Show();
                label_Alert.Show();
            }
            else
            {
                // textBoxAlert.Hide();
                label_Alert.Hide();
            }

            WFunction = 2;
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 

            Na.InReplAmount = CashInAmount;

            Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);
        }
        // If this changes
        private void textBoxJournal_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxInAmount_Excel_Amt.Text, out CashInAmount))
            {
            }

            textBoxInAmount_Excel_Amt.Text = CashInAmount.ToString();

            if (decimal.TryParse(textBoxJournal.Text, out JournalAmt))
            {
            }
            textBoxDiffFromJournal_IST.Text = (CashInAmount - JournalAmt).ToString("#,##0.00");
            if (CashInAmount != JournalAmt)
            {
                // textBoxAlert.Show();
                label_Alert.Show();
            }
            else
            {
                // textBoxAlert.Hide();
                label_Alert.Hide();
            }
        }
        // Create Action Occurances
        //DataTable TEMPTableFromAction;
        //string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances_Unloaded()
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            Ac.ReadAtm(WAtmNo);
            bool HybridRepl = false;
            if (W_IsFromExcel == true)
            {
                // continue 
                HybridRepl = false;
            }
            else
            {
                
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
                if (WOperator == "AUDBEGCA")
                    HybridRepl = false;
            }
            
            //
           

            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction;

            DoubleEntryAmt = InputRemainAmt;

            WUniqueRecordId = WSesNo; // SesNo 
            WCcy = "EGP";

            //
            // CLEAR PREVIOUS ACTIONS FOR THIS REPLENISHMENT
            //
            WUniqueRecordIdOrigin = "Replenishment";
            WUniqueRecordId = WSesNo;
            WActionId = "25";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            if (HybridRepl == false)
            {
                WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo;
                WActionId = "29";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //if (HybridRepl == true)
            //{
            //    WActionId = "39";
            //    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //}

            ////
            //WActionId = "30";
            //Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            // Delete create Dispute Shortage

            //if (HybridRepl == false)
            //{
            //    WActionId = "87";
            //    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //}

            //if (HybridRepl == true)
            //{
            //    WActionId = "77";
            //    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //}

            ////
            //WActionId = "88";
            //Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);



            if (HybridRepl == false & DoubleEntryAmt != 0)
            {
                // FIRST DOUBLE ENTRY 
                WActionId = "25"; // 25_DEBIT_ CIT Account/CR_AtmCash (UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment"

                WMaker_ReasonOfAction = "UnLoad From ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                      WAtmNo, WSesNo, WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                //if (DoubleEntryAmt == 0)
                //{
                //    WUniqueRecordIdOrigin = "Replenishment";
                //    WUniqueRecordId = WSesNo;
                //    WActionId = "25";
                //    Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
                //}
            }
            ////
            //// CREATE GL for Excess or shortage
            ////
            //decimal DiffGL = 0;

            //if (radioButtonGLBased_IST.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxNetResult_IST.Text, out DiffGL))
            //    {
            //    }
            //    else
            //    {
            //        //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

            //        //return;
            //    }
            //}

            //if (radioButtonGLBased_Journal.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxNetResult_JNL.Text, out DiffGL))
            //    {
            //    }
            //    else
            //    {
            //        //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

            //        //return;
            //    }
            //}


            //if (DiffGL == 0)
            //{
            //    // do nothing
            //}



            //if (DiffGL > 0 ) 
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
            //             + "Will be moved to the Branch excess account "
            //        );
            //    // Move to Excess 
            //    // SECOND DOUBLE ENTRY 
            //    WActionId = "30"; //30_CREDIT Branch Excess / DR_AtmCash(UNLOAD)
            //                      // WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = DiffGL;
            //    WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                          , WMaker_ReasonOfAction, "Replenishment");

            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}
            //if (DiffGL < 0 )
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
            //            + "Will be moved to the Branch shortage account "
            //             );
            //    // Move to Shortage
            //    // SECOND DOUBLE ENTRY 
            //    if (HybridRepl == false)
            //    {
            //        WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
            //    }
            //    if (HybridRepl == true)
            //    {
            //        WActionId = "39"; // 29_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
            //    }

            //    //WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = -DiffGL; // Turn it to positive 
            //    WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo,
            //                                          WMaker_ReasonOfAction, "Replenishment");
            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}

            // Handle Any Balance In Action Occurances 
            //string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo
            //           + "' AND ReplCycle =" + WSesNo
            //           + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            //Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            //if (Aoc.Current_DisputeShortage != 0)
            //{
            //    MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
            //             + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
            //             + "Look at the resulted transactions");


            //    decimal CIT_Shortage = 0;
            //    decimal Shortage = 0;
            //    decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
            //    decimal WExcess = Aoc.Excess;

            //    if (HybridRepl == false)
            //    {
            //        CIT_Shortage = -(Aoc.CIT_Shortage);
            //    }
            //    if (HybridRepl == true)
            //    {
            //        Shortage = -(Aoc.CIT_Shortage);
            //    }


            //    if (WExcess > 0)
            //    {
            //        if (WExcess >= Dispute_Shortage)
            //        {
            //            // A
            //            WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
            //                              // 
            //            WUniqueRecordId = WSesNo; // SesNo 
            //            WCcy = "EGP";
            //            DoubleEntryAmt = Dispute_Shortage;
            //            WMaker_ReasonOfAction = "Settle Dispute Shortage";
            //            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                  WActionId, WUniqueRecordIdOrigin,
            //                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                  , WMaker_ReasonOfAction, "Replenishment");

            //            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //        }
            //        else
            //        {   // A
            //            // Use all amount of Excess
            //            WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
            //                              // 
            //            WUniqueRecordId = WSesNo; // SesNo 
            //            WCcy = "EGP";
            //            DoubleEntryAmt = WExcess; // Use all amount iin Excess
            //            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
            //            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                  WActionId, WUniqueRecordIdOrigin,
            //                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                  , WMaker_ReasonOfAction, "Replenishment");

            //            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //            // The rest you take it from Shortage

            //            decimal TempDiff1 = Dispute_Shortage - WExcess;
            //            if (TempDiff1 > 0)
            //            {
            //                // Diff1 goes to Shortage
            //                // B
            //                if (HybridRepl == false)
            //                {
            //                    WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
            //                }

            //                if (HybridRepl == true)
            //                {
            //                    WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
            //                }

            //                // 
            //                WUniqueRecordId = WSesNo; // SesNo 
            //                WCcy = "EGP";
            //                DoubleEntryAmt = TempDiff1;
            //                WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
            //                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                      WActionId, WUniqueRecordIdOrigin,
            //                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                      , WMaker_ReasonOfAction, "Replenishment");

            //                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //            }

            //        }
            //    }

            //    if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
            //        || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
            //        )
            //    {
            //        // 
            //        if (HybridRepl == false)
            //        {
            //            WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
            //        }
            //        if (HybridRepl == true)
            //        {
            //            WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
            //        }
            //        // 
            //        WUniqueRecordId = WSesNo; // SesNo 
            //        WCcy = "EGP";
            //        DoubleEntryAmt = Dispute_Shortage;
            //        WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
            //        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                              WActionId, WUniqueRecordIdOrigin,
            //                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                              , WMaker_ReasonOfAction, "Replenishment");

            //        TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //    }

            //    //label43.Show();
            //    //textBoxDisp.Show();
            //    //textBoxDisp.Text = "0.00";
            //}

        }

        // Create Action Occurances
        //DataTable TEMPTableFromAction;
        //string WUniqueRecordIdOrigin = "Replenishment";
        //InputDepositsAmt = ISTDifferenceCR 
        public void CreateActions_OccurancesDeposits(decimal WInputDepositsAmt)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            // RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            decimal CassetteAmt;
            decimal RetractedAmt;
            //
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

            DoubleEntryAmt = WInputDepositsAmt;
            WUniqueRecordId = WSesNo; // SesNo 
            WCcy = "EGP";
            string WMaker_ReasonOfAction;

            // CLEAR ALL PREVIOUS ACTIONS
            WActionId = "26";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "37";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);


            //DoubleEntryAmt = CassetteAmt + RetractedAmt;
            // FIRST DOUBLE ENTRY 
            if (HybridRepl == false & DoubleEntryAmt != 0)
            {
                WActionId = "26"; // 26_CREDIT CIT Account/DR_AtmCash (DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";

                //DoubleEntryAmt = Na.Balances1.CountedBal;
                WMaker_ReasonOfAction = "UnLoad Deposits";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                //WActionId = "26";
                //Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }



            //DiffGL = 0;

            //decimal CassetteAmtDiff = WISTDifferenceCR;
            decimal RetractedAmtDiff;

            //decimal DiffGL = 0;

            //// DEPOSITS

            //if (radioButtonGLBased_IST.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxIST_DiffDep.Text, out DiffGL))
            //    {
            //    }
            //    else
            //    {
            //        //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

            //        //return;
            //    }
            //}

            //if (radioButtonGLBased_Journal.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxDiffDeposits_JLN.Text, out DiffGL))
            //    {
            //    }
            //    else
            //    {
            //        //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

            //        //return;
            //    }
            //}
            //// DiffGL = WISTDifferenceCR;
            ////DiffGL = CassetteAmtDiff + RetractedAmtDiff;

            //if (DiffGL == 0)
            //{
            //    // do nothing
            //}

            //if (DiffGL > 0 & NetTotalExcess_IST != 0)
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
            //            + "Will be moved to the CIT excess account ");
            //    // Move to Excess 
            //    // SECOND DOUBLE ENTRY 
            //    WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
            //                      // WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = DiffGL;
            //    WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                          , WMaker_ReasonOfAction, "Replenishment");

            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}
            //if (DiffGL < 0 & NetTotalExcess_IST != 0)
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
            //            + "Will be moved to the CIT shortage account ");
            //    // Move to Shortage
            //    // SECOND DOUBLE ENTRY 
            //    if (HybridRepl == false)
            //    {
            //        WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
            //    }
            //    if (HybridRepl == true)
            //    {
            //        WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
            //    }

            //    //WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = -DiffGL; // Turn it to positive 
            //    WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                          , WMaker_ReasonOfAction, "Replenishment");
            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}


        }
        public void CreateActions_Occurances_Excess_Short_Disputes()
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            Ac.ReadAtm(WAtmNo);
            bool HybridRepl = false;
            if (W_IsFromExcel == true)
            {
                // continue 
                HybridRepl = false;
            }
            else
            {

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
                if (WOperator == "AUDBEGCA")
                    HybridRepl = false;
            }

            ////


            string WActionId = "";
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId = WSesNo;
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction;

          
            //
            // DELETE PREVIOUS EXCESS and Shortage  
            if (HybridRepl == true)
            {
                WActionId = "39";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "30";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            //// Delete create Dispute Shortage

            if (HybridRepl == false)
            {
                WActionId = "87";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "77";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "88";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            
            //
            // CREATE GL for Excess or shortage
            //
            decimal DiffGL = 0;

            //Total_T_Excess_IST = T_ExcessDR_IST + T_ExcessCR_IST;
            //Total_T_Shortage_IST = T_ShortageDR_IST + T_ShortageCR_IST;
            DiffGL = Total_T_Excess_IST - Total_T_Shortage_IST; 
           

            //if (radioButtonGLBased_IST.Checked == true)
            //{
            //    if (decimal.TryParse(textBoxNetResult_IST.Text, out DiffGL))
            //    {
            //    }
            //    else
            //    {
            //        //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

            //        //return;
            //    }
            //}

            if (radioButtonGLBased_Journal.Checked == true)
            {
                if (decimal.TryParse(textBoxNetResult_JNL.Text, out DiffGL))
                {
                }
                else
                {
                    //MessageBox.Show(textBoxDiffExp.Text, "Please enter a valid number for Expected!");

                    //return;
                }
            }


            if (DiffGL == 0)
            {
                // do nothing
            }
            // PANICOS START
            // FOR DEPOSITS 
            //
            //if (DiffGL > 0 & NetTotalExcess_IST != 0)
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
            //            + "Will be moved to the CIT excess account ");
            //    // Move to Excess 
            //    // SECOND DOUBLE ENTRY 
            //    WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
            //                      // WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = DiffGL;
            //    WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                          , WMaker_ReasonOfAction, "Replenishment");

            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}
            //if (DiffGL < 0 & NetTotalExcess_IST != 0)
            //{
            //    MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
            //            + "Will be moved to the CIT shortage account ");
            //    // Move to Shortage
            //    // SECOND DOUBLE ENTRY 
            //    if (HybridRepl == false)
            //    {
            //        WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
            //    }
            //    if (HybridRepl == true)
            //    {
            //        WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
            //    }

            //    //WUniqueRecordIdOrigin = "Replenishment";
            //    WUniqueRecordId = WSesNo; // SesNo 
            //    WCcy = "EGP";
            //    DoubleEntryAmt = -DiffGL; // Turn it to positive 
            //    WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
            //    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                          WActionId, WUniqueRecordIdOrigin,
            //                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                          , WMaker_ReasonOfAction, "Replenishment");
            //    TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //}

            // Panicos END 

            if (DiffGL > 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                         + "Will be moved to the Branch excess account "
                    );
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "30"; //30_CREDIT Branch Excess / DR_AtmCash(UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = DiffGL;
                WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (DiffGL < 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#,##0.00") + Environment.NewLine
                        + "Will be moved to the Branch shortage account "
                         );
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
                }
                if (HybridRepl == true)
                {
                    WActionId = "39"; // 39_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -DiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo,
                                                      WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            // Handle Any Balance In Action Occurances 
            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo
                       + "' AND ReplCycle =" + WSesNo
                       + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            if (Aoc.Current_DisputeShortage != 0)
            {
                MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
                         + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
                         + "Look at the resulted transactions");


                decimal CIT_Shortage = 0;
                decimal Shortage = 0;
                decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
                decimal WExcess = Aoc.Excess;

                if (HybridRepl == false)
                {
                    CIT_Shortage = -(Aoc.CIT_Shortage);
                }
                if (HybridRepl == true)
                {
                    Shortage = -(Aoc.CIT_Shortage);
                }


                if (WExcess > 0)
                {
                    if (WExcess >= Dispute_Shortage)
                    {
                        // A
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = Dispute_Shortage;
                        WMaker_ReasonOfAction = "Settle Dispute Shortage";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }
                    else
                    {   // A
                        // Use all amount of Excess
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = WExcess; // Use all amount iin Excess
                        WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                        // The rest you take it from Shortage

                        decimal TempDiff1 = Dispute_Shortage - WExcess;
                        if (TempDiff1 > 0)
                        {
                            // Diff1 goes to Shortage
                            // B
                            if (HybridRepl == false)
                            {
                                WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            if (HybridRepl == true)
                            {
                                WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            // 
                            WUniqueRecordId = WSesNo; // SesNo 
                            WCcy = "EGP";
                            DoubleEntryAmt = TempDiff1;
                            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                  WActionId, WUniqueRecordIdOrigin,
                                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                                  , WMaker_ReasonOfAction, "Replenishment");

                            TEMPTableFromAction = Aoc.TxnsTableFromAction;
                        }

                    }
                }

                if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
                    || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
                    )
                {
                    // 
                    if (HybridRepl == false)
                    {
                        WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                    }
                    if (HybridRepl == true)
                    {
                        WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
                    }
                    // 
                    WUniqueRecordId = WSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Dispute_Shortage;
                    WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

                //label43.Show();
                //textBoxDisp.Show();
                //textBoxDisp.Text = "0.00";
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
                // Create 
                // load transaction for CIT and Bank

                // RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WActionId;
                // string WUniqueRecordIdOrigin ;
                int WUniqueRecordId;
                string WCcy;
                decimal DoubleEntryAmt;

                // FIRST DOUBLE ENTRY 
                WActionId = "24"; // 24_CREDIT CIT Account/DR_AtmCash (LOAD)

                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InCashInAmount;
                string WMaker_ReasonOfAction = "Load ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;

            }

        }

        
        // show discrepancies
        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            int WSelection = 4; // include SOLO

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, DateTmSesStart, DateTmSesEnd, WSelection
                );
            NForm80b2.Show();
        }
        // Correct the cycle
        private void linkLabelCorrectTheCycle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // YOU CAN CORRECT THE DATES AND OPENNING BALANCE
            string WJobCategory = "ATMs";
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {

            }
            Form154 NForm154;
            int WMode = 1;
            NForm154 = new Form154(WOperator, WSignedId, WReconcCycleNo, WMode, WAtmNo);
            NForm154.FormClosed += NForm154_FormClosed;
            NForm154.ShowDialog();
        }

        private void NForm154_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetScreen();
        }

        // show the deposits
        private void buttonShowDeposits_Click(object sender, EventArgs e)
        {
            string WTableId = "Switch_IST_Txns";

            Form78d_ATMRecords NForm78D_ATMRecords;
            NForm78D_ATMRecords = new Form78d_ATMRecords(WOperator, WSignedId, WTableId, WAtmNo, DateTmSesStart
                               , DateTmSesEnd, WSesNo, NullPastDate, 3);

            NForm78D_ATMRecords.Show();
        }
        // Show SM Lines
        private void linkLabelSMLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
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
                NForm67_BDC.Show();
            }
            else
            {
                MessageBox.Show("Not found records");
            }
        }
        // Allow opening Balance
        private void checkBox_Allow_Balance_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Allow_Balance.Checked == true)
            {
                if (InternalChange == false)
                {
                    MessageBox.Show("Press Refresh After You Change Opening Balance");
                }

                textBoxOpenBal.ReadOnly = false;
                buttonRefresh.Show();
            }
            else
            {
                WFunction = 2;
                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, WFunction); // 
                Na.GL_Balance = 0; // Keep the Change here
                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                Sm.Read_SM_Record_Specific_By_ATMno_ReplCycle(WAtmNo, WSesNo);
                OpeningBal = // After date Cap_date
                              Sm.ATM_total1 * Na.Cassettes_1.FaceValue
                            + Sm.ATM_total2 * Na.Cassettes_2.FaceValue
                            + Sm.ATM_total3 * Na.Cassettes_3.FaceValue
                            + Sm.ATM_total4 * Na.Cassettes_4.FaceValue;
                ;

                textBoxOpenBal.ReadOnly = true;
                SetScreen();

                buttonRefresh.Hide();
            }

        }

        // Show 
        private void radioButtonNoShow_CheckedChanged(object sender, EventArgs e)
        {
            Show_No_Reversals = true;

            SetScreen();
        }
        // Not Show
        private void radioButtonShow_CheckedChanged(object sender, EventArgs e)
        {
            // if (FirstEntry == )
            Show_No_Reversals = false;

            SetScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WSesNo == 0)
            {
                MessageBox.Show("Nothing to show");
                return;
            }
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, 0,
                                                WAtmNo, DateTmSesStart, DateTmSesEnd, 2
                );
            NForm80b2.Show();
        }

        private void radioButtonGLBased_IST_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGLBased_IST.Checked == true)
            {
                panel_IST.BackColor = System.Drawing.Color.White;
                panel_IST_Deposits.BackColor = System.Drawing.Color.White;
                panel_IST_Excess.BackColor = System.Drawing.Color.White;
                panel_JournalInfo.BackColor = System.Drawing.Color.LightGray;
                panel_Journal_Deposits.BackColor = System.Drawing.Color.LightGray;
                panel_Journal_Excess.BackColor = System.Drawing.Color.LightGray;

            }

        }

        private void radioButtonGLBased_Journal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGLBased_Journal.Checked == true)
            {
                panel_IST.BackColor = System.Drawing.Color.LightGray;
                panel_IST_Deposits.BackColor = System.Drawing.Color.LightGray;
                panel_IST_Excess.BackColor = System.Drawing.Color.LightGray;
                panel_JournalInfo.BackColor = System.Drawing.Color.White;
                panel_Journal_Deposits.BackColor = System.Drawing.Color.White;
                panel_Journal_Excess.BackColor = System.Drawing.Color.White;

                //MessageBox.Show("By selecting journal "+Environment.NewLine
                //    +"you must insert a note to explain why you have to do this"
                //    ); 
            }
        }
        // show cassettes 
        private void linkLabel_SM_Cassettes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form51_SM_Cassettes NForm51_SM_Cassettes;
            NForm51_SM_Cassettes = new Form51_SM_Cassettes(WOperator, WSignedId, WAtmNo, WSesNo);
            NForm51_SM_Cassettes.Show();
        }
    }
}

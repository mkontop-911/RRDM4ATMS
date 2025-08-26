using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm51c_SM : UserControl
    {
        // Working variables

        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        bool IsCcy_1;
        bool IsCcy_2;
        bool IsCcy_3;
        bool IsCcy_4;
        bool IsCcy_5;
        bool IsCcy_6;
        string Ccy_1;
        string Ccy_2;
        string Ccy_3;
        string Ccy_4;
        string Ccy_5;
        string Ccy_6;

        bool Is_L_2_Done;
        bool Is_L_3_Done;
        bool Is_L_4_Done;
        bool Is_L_5_Done;
        bool Is_L_6_Done;

        decimal WTotalForCust;
        decimal WTotalCommision;

        int WTotal_SM_Notes;
        decimal WTotal_SM_Money;

        int WMode;

        bool AudiType; 

        Form38_CDM NForm38_CDM;

        Form24 NForm24;

        bool ViewWorkFlow;

        bool WSetScreen;

        string From_SM;

        DateTime WDateFrom;
        DateTime WDateTo; 

        //    int Process;

        //    NotesBalances Na = new NotesBalances(); // Class Notes 
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Class Traces 

        RRDMDepositsClass Da = new RRDMDepositsClass(); // Contains all Deposits and Cheques 

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // see if deposits errors 

        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        //    int WFunction;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;

        public void UCForm51cPar_SM(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            InitializeComponent();

            //
            // Find If AUDI Type 
            // If found and it is 1 is Audi Type If Zero then is normal 
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
                    buttonShowGL.Hide();

                    // Set deeposits
                    RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
                    // Read G4
                    G4.ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(WAtmNo, WSesNo, 1);

                    if (G4.RecordFound == true)
                    {
                        // 
                        // Update Daposits Counted
                        //
                        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

                        if (G4.Deposits > 0)
                        {
                            // Insert Record For COUNTED 
                            string Ccy_1 = "EGP";
                            SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_1);

                            SM.TotalCassetteNotesCount = G4.Deposits_Notes_Denom_1 + G4.Deposits_Notes_Denom_2 + G4.Deposits_Notes_Denom_3 + G4.Deposits_Notes_Denom_4;
                            SM.TotalCassetteAmountCount = G4.Deposits;

                            SM.TotalRetractedNotesCount = 0;
                            SM.TotalRetractedAmountCount = 0;

                            SM.TotalRecycledNotesCount = 0;
                            SM.TotalRecycledAmountCount = 0;
                            // UPDATE Record 
                            SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_1);

                            buttonUseSMfigures.Hide();
                            //button5.Hide();
                            //buttonShowErrors.Show();
                            //buttonShowTxns.Show();
                            //buttonShowGL.Show();
                            //buttonShowNotes.Show();

                            //      textBoxDepositsCount1Trans.ReadOnly = true;
                            textBoxCcy1CassetteNotesCount.ReadOnly = false;
                            textBoxCcy1CassetteAmountCount.ReadOnly = false;
                            textBoxCcy1RetractedAmountCount.ReadOnly = false;
                            textBoxCcy1RecycledNotesCount.ReadOnly = false;
                            textBoxCcy1RecycledAmountCount.ReadOnly = false;


                        }
                    }

                }
                else
                {     
                    AudiType = false;
                    buttonShowGL.Show();
                }
            }
            else
            {
                AudiType = false;
            }


            tableLayoutPanel11.Dock = DockStyle.Top;

            WSetScreen = true;

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 12 || Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
            else
            {
                button5.Show();
            }

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            //
            // Check if Deposit from SM
            //
            int Mode = 2;

            Mpa.ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(WOperator, WSignedId, Mode, WAtmNo,
                                           Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

            if (Mpa.MatchingMasterDataTableATMs.Rows.Count > 0)
            {
                // ALERT for Suspect
                panel4.Show();
                labelSuspect.Show();
            }
            else
            {
                panel4.Hide();
                labelSuspect.Hide();
            }


            string ParId = "218";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            From_SM = Gp.OccuranceNm;

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
                    MessageBox.Show("No Deposits Found");
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

                int WTotalNotes;
                decimal WTotalMoneyDecimal;

                int WRetractTotalNotes;
                decimal WRetractTotalMoney;


                int WRECYCLEDTotalNotes;
                decimal WRECYCLEDTotalMoney;

                // Read Table 
                int L = 1; // leave it 1
                int I = 0;


                while (I <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
                {
                    // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                    string WCcy = (string)SM.DataTable_SM_Deposits.Rows[I]["Ccy"];
                    WCcy = WCcy.Trim(); 
                    if (WCcy == "")
                    {
                        I = I + 1;
                        continue;
                    }
                    WTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteNotes"];
                    WTotalMoneyDecimal = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalCassetteMoney"];

                    WRetractTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTNotes"];
                    WRetractTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRETRACTMoney"];

                    WTotalNotes = WTotalNotes + WRetractTotalNotes;
                    WTotalMoneyDecimal = WTotalMoneyDecimal + WRetractTotalMoney;
                    //WTotalNotes = WTotalNotes + WRetractTotalNotes;
                    //WTotalMoneyDecimal = WTotalMoneyDecimal + WRetractTotalMoney;

                    WRECYCLEDTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDNotes"];
                    WRECYCLEDTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalRECYCLEDMoney"];

                    //WTotal_SM_Notes = ((WTotalNotes + WRetractTotalNotes + WRECYCLEDTotalNotes) - WNCRTotalNotes);
                    //WTotal_SM_Money = ((WTotalMoneyDecimal + WRetractTotalMoney + WRECYCLEDTotalMoney) - WNCRTotalMoney);

                    // Bring the EGP first 

                    if (WCcy != "EGP")
                    {
                        L = L + 1;
                    }

                    if (WCcy == "EGP")
                    {
                        // Fill the first set
                        IsCcy_1 = true;
                        Ccy_1 = WCcy;

                        labelCurrency1.Text = WCcy;

                        //textBoxCcy1CassetteNotes.Text = WTotalNotes.ToString("#0");
                        //textBoxCcy1CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy1CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy1CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy1RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy1RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy1RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy1RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy1CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy1CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy1RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy1RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy1RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy1RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");

                        // MAKE CHECK WITH Mpa

                        WMode = 2;
                        string TempCurrency = "";  
                        if(WCcy.Trim() == "EGP")
                        {
                            TempCurrency = "818"; 
                        }

                        Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                        WDateFrom = Ta.SesDtTimeStart;
                        WDateTo = Ta.SesDtTimeEnd;

                        Mpa.ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(WAtmNo, WDateFrom, WDateTo, TempCurrency, WMode, 2);

                        int TotNoBNA = Mpa.TotNoBNA;
                        decimal TotValueBNA = Mpa.TotValueBNA;

                        if (TotValueBNA != WTotalMoneyDecimal )
                        {
                            panel4.Show(); 
                            panelDepDiff.Show();
                            textBoxDep_SM.Text = WTotalMoneyDecimal.ToString("#,##0.00");
                            textBoxDep_Jrn.Text = TotValueBNA.ToString("#,##0.00");
                            textBoxDiff.Text = (WTotalMoneyDecimal-TotValueBNA).ToString("#,##0.00");
                        }
                        else
                        {
                            //panelDepDiff.Hide();
                            panel4.Show();
                            panelDepDiff.Show();
                            textBoxDep_SM.Text = WTotalMoneyDecimal.ToString("#,##0.00");
                            textBoxDep_Jrn.Text = TotValueBNA.ToString("#,##0.00");
                            textBoxDiff.Text = (WTotalMoneyDecimal - TotValueBNA).ToString("#,##0.00");
                        }

                        if (L == 1) // Means that EGP is first currency
                        {
                            labelCurrency2.Hide();
                            tableLayoutPanel2_1.Hide();
                            tableLayoutPanel2_2.Hide();
                        }


                    }
                    if (L == 2 & Is_L_2_Done == false)
                    {
                        // Fill the first set
                        Is_L_2_Done = true;
                        IsCcy_2 = true;
                        Ccy_2 = WCcy;

                        labelCurrency2.Text = WCcy;
                        textBoxCcy2CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy2CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy2RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy2RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy2RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy2RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        labelCurrency2.Show();
                        tableLayoutPanel2_1.Show();
                        tableLayoutPanel2_2.Show();

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy2CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy2CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy2RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy2RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy2RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy2RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");

                    }
                    if (L == 3 & Is_L_3_Done == false)
                    {
                        // Fill the first set
                        Is_L_3_Done = true;
                        IsCcy_3 = true;
                        Ccy_3 = WCcy;

                        labelCurrency3.Text = WCcy;
                        textBoxCcy3CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy3CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy3RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy3RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy3RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy3RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        panelTwo.Show();
                        labelCurrency3.Show();
                        tableLayoutPanel3_1.Show();
                        tableLayoutPanel3_2.Show();

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy3CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy3CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy3RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy3RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy3RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy3RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");
                    }

                    if (L == 4 & Is_L_4_Done == false)
                    {
                        // Fill the first set
                        Is_L_4_Done = true;
                        IsCcy_4 = true;
                        Ccy_4 = WCcy;

                        labelCurrency4.Text = WCcy;
                        textBoxCcy4CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy4CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy4RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy4RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy4RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy4RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        panelTwo.Show();
                        labelCurrency4.Show();
                        tableLayoutPanel4_1.Show();
                        tableLayoutPanel4_2.Show();

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy4CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy4CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy4RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy4RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy4RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy4RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");
                    }
                    if (L == 5 & Is_L_5_Done == false)
                    {
                        // Fill the first set
                        Is_L_5_Done = true;
                        IsCcy_5 = true;
                        Ccy_5 = WCcy;

                        labelCurrency5.Text = WCcy;
                        textBoxCcy5CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy5CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy5RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy5RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy5RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy5RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        panelThree.Show();
                        labelCurrency5.Show();
                        tableLayoutPanel5_1.Show();
                        tableLayoutPanel5_2.Show();

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy5CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy5CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy5RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy5RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy5RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy5RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");
                    }
                    if (L == 6 & Is_L_6_Done == false)
                    {
                        // Fill the first set
                        Is_L_6_Done = true;
                        IsCcy_6 = true;
                        Ccy_6 = WCcy;

                        labelCurrency6.Text = WCcy;
                        textBoxCcy6CassetteNotes.Text = WTotalNotes.ToString("#0");
                        textBoxCcy6CassetteAmount.Text = WTotalMoneyDecimal.ToString("#0.00");

                        textBoxCcy6RetractedNotes.Text = WRetractTotalNotes.ToString("#0");
                        textBoxCcy6RetractedAmount.Text = WRetractTotalMoney.ToString("#0.00");

                        textBoxCcy6RecycledNotes.Text = WRECYCLEDTotalNotes.ToString("#0");
                        textBoxCcy6RecycledAmount.Text = WRECYCLEDTotalMoney.ToString("#0.00");

                        panelThree.Show();
                        labelCurrency6.Show();
                        tableLayoutPanel6_1.Show();
                        tableLayoutPanel6_2.Show();

                        SM.Read_SM_AND_Get_CountedByCcy(WAtmNo, WSesNo, WCcy);

                        textBoxCcy6CassetteNotesCount.Text = SM.TotalCassetteNotesCount.ToString("#0");
                        textBoxCcy6CassetteAmountCount.Text = SM.TotalCassetteAmountCount.ToString("#0.00");

                        textBoxCcy6RetractedNotesCount.Text = SM.TotalRetractedNotesCount.ToString("#0");
                        textBoxCcy6RetractedAmountCount.Text = SM.TotalRetractedAmountCount.ToString("#0.00");

                        textBoxCcy6RecycledNotesCount.Text = SM.TotalRecycledNotesCount.ToString("#0");
                        textBoxCcy6RecycledAmountCount.Text = SM.TotalRecycledAmountCount.ToString("#0.00");
                    }

                    I = I + 1;
                }

            }
            // Get Totals For FOREX 
            // A. Local Money given
            // B. Total Commision 
            //if (IsCcy_2 == true)
            //{
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                SM.Read_ForexChildALL_Get_Totals(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd);
                if (SM.RecordFound == true)
                {
                    panel4.Show();
                    labelForex.Show();
                    panelForex.Show();
                    WTotalForCust = SM.TotalForCust;
                    WTotalCommision = SM.TotalCommision;
                    textBoxTotalForCust.Text = WTotalForCust.ToString("#0.00");
                    textBoxTotalCommision.Text = WTotalCommision.ToString("#0.00");
                }
                else
                {
                    labelForex.Hide();
                    panelForex.Hide();
                    WTotalForCust = 0;
                    WTotalCommision = 0;
                }

            //}
            //else
            //{
            //    labelForex.Hide();
            //    panelForex.Hide();
            //    WTotalForCust = 0;
            //    WTotalCommision = 0;
            //}

            //******************
            Na.ReadAllErrorsTable(WAtmNo, WSesNo);

            if (Na.NumberOfErrDep > 0)
            {
                buttonShowErrors.Show();
            }
            else buttonShowErrors.Hide();

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(WAtmNo);

            if (Ac.DepoRecycling == true)
            {
                labelCashDep.Text = "NON RECYCLED DEPOSITS";
            }


            // GET totals For Suspect Notes and Fake
            //
            Mode = 1; // only 
            Er.ReadAllErrorsTableToFindTotalsForSuspectAndFake(WAtmNo, WSesNo, 0, Mode);

            Da.ReadDepositsSessionsNotesAndValuesDeposits(WAtmNo, WSesNo);

            // Differences 

            ValidateInputAndShowDifferences();

            // Show Total Balances 

            guidanceMsg = " INPUT DATA AND UPDATE";

            // Handle request from Reconciliation 

            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (ViewWorkFlow == true)  // If 13 the request came from Reconciliation and not from Replenishemnt 
            {
                buttonUseSMfigures.Hide();
                button5.Hide();
                buttonShowErrors.Show();
                buttonShowTxns.Show();
                buttonShowGL.Show();
                buttonShowNotes.Show();

                //      textBoxDepositsCount1Trans.ReadOnly = true;
                textBoxCcy1CassetteNotesCount.ReadOnly = true;
                textBoxCcy1CassetteAmountCount.ReadOnly = true;
                textBoxCcy1RetractedAmountCount.ReadOnly = true;
                textBoxCcy1RecycledNotesCount.ReadOnly = true;
                textBoxCcy1RecycledAmountCount.ReadOnly = true;


                //    guidanceMsg = " View only "; // Moved to form51
            }

            WSetScreen = false;

        }
        // 
        // SHOW ATMs FIGURES AS COUNTED 
        //
        private void button6_Click(object sender, EventArgs e)
        {
            button5.Show(); // Show Update button 
                            //    textBoxDepositsDiff1Trans.Text = "0";
                            //textBoxCcy1CassetteNotesDiff.Text = "0";
                            //textBoxCcy1CassetteAmountDiff.Text = "0.00";
                            //textBoxCcy1RetractedNotesDiff.Text = "0";
                            //textBoxCcy1RetractedAmountDiff.Text = "0.00";
                            //textBoxCcy1RecycledNotesDiff.Text = "0";
                            //textBoxCcy1RecycledAmountDiff.Text = "0.00";

            textBoxCcy1CassetteNotesCount.Text = textBoxCcy1CassetteNotes.Text;
            textBoxCcy1CassetteAmountCount.Text = textBoxCcy1CassetteAmount.Text;

            textBoxCcy1RetractedNotesCount.Text = textBoxCcy1RetractedNotes.Text;
            textBoxCcy1RetractedAmountCount.Text = textBoxCcy1RetractedAmount.Text;

            textBoxCcy1RecycledNotesCount.Text = textBoxCcy1RecycledNotes.Text;
            textBoxCcy1RecycledAmountCount.Text = textBoxCcy1RecycledAmount.Text;

            if (IsCcy_2 == true)
            {
                textBoxCcy2CassetteNotesCount.Text = textBoxCcy2CassetteNotes.Text;
                textBoxCcy2CassetteAmountCount.Text = textBoxCcy2CassetteAmount.Text;

                textBoxCcy2RetractedNotesCount.Text = textBoxCcy2RetractedNotes.Text;
                textBoxCcy2RetractedAmountCount.Text = textBoxCcy2RetractedAmount.Text;

                textBoxCcy2RecycledNotesCount.Text = textBoxCcy2RecycledNotes.Text;
                textBoxCcy2RecycledAmountCount.Text = textBoxCcy2RecycledAmount.Text;
            }

            if (IsCcy_3 == true)
            {
                textBoxCcy3CassetteNotesCount.Text = textBoxCcy3CassetteNotes.Text;
                textBoxCcy3CassetteAmountCount.Text = textBoxCcy3CassetteAmount.Text;

                textBoxCcy3RetractedNotesCount.Text = textBoxCcy3RetractedNotes.Text;
                textBoxCcy3RetractedAmountCount.Text = textBoxCcy3RetractedAmount.Text;

                textBoxCcy3RecycledNotesCount.Text = textBoxCcy3RecycledNotes.Text;
                textBoxCcy3RecycledAmountCount.Text = textBoxCcy3RecycledAmount.Text;
            }

            if (IsCcy_4 == true)
            {
                textBoxCcy4CassetteNotesCount.Text = textBoxCcy4CassetteNotes.Text;
                textBoxCcy4CassetteAmountCount.Text = textBoxCcy4CassetteAmount.Text;

                textBoxCcy4RetractedNotesCount.Text = textBoxCcy4RetractedNotes.Text;
                textBoxCcy4RetractedAmountCount.Text = textBoxCcy4RetractedAmount.Text;

                textBoxCcy4RecycledNotesCount.Text = textBoxCcy4RecycledNotes.Text;
                textBoxCcy4RecycledAmountCount.Text = textBoxCcy4RecycledAmount.Text;
            }
            if (IsCcy_5 == true)
            {
                textBoxCcy5CassetteNotesCount.Text = textBoxCcy5CassetteNotes.Text;
                textBoxCcy5CassetteAmountCount.Text = textBoxCcy5CassetteAmount.Text;

                textBoxCcy5RetractedNotesCount.Text = textBoxCcy5RetractedNotes.Text;
                textBoxCcy5RetractedAmountCount.Text = textBoxCcy5RetractedAmount.Text;

                textBoxCcy5RecycledNotesCount.Text = textBoxCcy5RecycledNotes.Text;
                textBoxCcy5RecycledAmountCount.Text = textBoxCcy5RecycledAmount.Text;
            }
            if (IsCcy_6 == true)
            {
                textBoxCcy6CassetteNotesCount.Text = textBoxCcy6CassetteNotes.Text;
                textBoxCcy6CassetteAmountCount.Text = textBoxCcy6CassetteAmount.Text;

                textBoxCcy6RetractedNotesCount.Text = textBoxCcy6RetractedNotes.Text;
                textBoxCcy6RetractedAmountCount.Text = textBoxCcy6RetractedAmount.Text;

                textBoxCcy6RecycledNotesCount.Text = textBoxCcy6RecycledNotes.Text;
                textBoxCcy6RecycledAmountCount.Text = textBoxCcy6RecycledAmount.Text;
            }

            ValidateInputAndShowDifferences();

            guidanceMsg = " VERIFY FIGURES, CHANGE THEM IF NEEDED AND PRESS UPDATE";


            Usi.ReadSignedActivityByKey(WSignRecordNo);

            Usi.ReplStep3_Updated = false;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            ChangeBoardMessage(this, e);

        }


        //
        // With UPDATE BUTTOM Update ONLY AND SHOW DIFF AND BALANCES 
        //

        private void button5_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBoxCcy1CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
            {
                //  MessageBox.Show(textBox10.Text, "The input number is correct!");
            }
            else
            {
                MessageBox.Show(textBoxCcy1CassetteAmountCount.Text, "Please enter a valid number!");
                return;
            }

            // DEPOSITED AMOUNT
            if (SM.TotalCassetteAmountCount == 0)
            {
                if (checkBoxAcceptZero.Checked == false)
                {
                    MessageBox.Show("Please enter Deposited Amount !" + Environment.NewLine
                                    + "Input values is zero" + Environment.NewLine
                                     + "Check the Box Accept Zero if really want to enter zero" + Environment.NewLine
                                    );
                    return;
                }
                else
                {
                    // Continue check box Accept Zero is Checked. 
                    // Continue 
                }
            }

            //
            // AUDI OR AUDI 
            // 
            if (AudiType == true)
            {

                // 
                ValidateInputAndShowDifferences();

                // Update STEP

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep3_Updated = true;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //button5.Hide(); 

                
                    guidanceMsg = " Updating done .. Move to next step. ";
                

                ChangeBoardMessage(this, e);
            }
            else
            {
                // VALIDATE
                ValidateInputAndShowDifferences();

                // CREATE ACTIONS 
                CreateActions_Occurances();

                // Update STEP

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep3_Updated = true;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                //button5.Hide(); 

                if (DiffGL != 0)
                {
                    guidanceMsg = " Updating done but with differences! - Move to next step. ";
                }
                else
                {
                    guidanceMsg = " Updating done .. No Differences !- Move to next step. ";
                }

                ChangeBoardMessage(this, e);
            }


            

        }

        protected override CreateParams CreateParams
        {

            get
            {

                CreateParams handleParam = base.CreateParams;

                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED        

                return handleParam;

            }

        }
        // SHOW ERRORS 
        private void button3_Click(object sender, EventArgs e)
        {
            bool Replenishment = true;
            //     string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299) AND OpenErr =1";
            string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrId > 200 AND ErrId < 299)";
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            NForm24.Show();
        }

        // Differences
        private void ValidateInputAndShowDifferences()
        {
            if (IsCcy_1 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_1);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_1);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy1CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy1CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                
                // 
                if (decimal.TryParse(textBoxCcy1CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy1CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                //// DEPOSITED AMOUNT
                //if (SM.TotalCassetteAmountCount == 0)
                //{
                //    MessageBox.Show("Please enter Deposited Amount!");
                //    return;
                //}

                // Retracted 
                if (int.TryParse(textBoxCcy1RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy1RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy1RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy1RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy1RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy1RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy1RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy1RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy1CassetteNotes;
                decimal Ccy1CassetteAmount;

                int Ccy1RetractedNotes;
                decimal Ccy1RetractedAmount;

                int Ccy1RecycledNotes;
                decimal Ccy1RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy1CassetteNotes.Text, out Ccy1CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy1CassetteAmount.Text, out Ccy1CassetteAmount)) ;

                if (int.TryParse(textBoxCcy1RetractedNotes.Text, out Ccy1RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy1RetractedAmount.Text, out Ccy1RetractedAmount)) ;

                if (int.TryParse(textBoxCcy1RecycledNotes.Text, out Ccy1RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy1RecycledAmount.Text, out Ccy1RecycledAmount)) ;


                textBoxCcy1CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy1CassetteNotes).ToString();
                textBoxCcy1CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy1CassetteAmount).ToString("#0.00");

                textBoxCcy1RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy1RetractedNotes).ToString();
                textBoxCcy1RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy1RetractedAmount).ToString("#0.00");

                textBoxCcy1RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy1RecycledNotes).ToString();
                textBoxCcy1RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy1RecycledAmount).ToString("#0.00");

                // UPDATE COUNTED

                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_1);

            }

            if (IsCcy_2 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_2);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_2);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy2CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy2CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                // 
                if (decimal.TryParse(textBoxCcy2CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy2CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                // Retracted 
                if (int.TryParse(textBoxCcy2RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy2RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy2RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy2RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy2RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy2RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy2RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy2RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy2CassetteNotes;
                decimal Ccy2CassetteAmount;

                int Ccy2RetractedNotes;
                decimal Ccy2RetractedAmount;

                int Ccy2RecycledNotes;
                decimal Ccy2RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy2CassetteNotes.Text, out Ccy2CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy2CassetteAmount.Text, out Ccy2CassetteAmount)) ;

                if (int.TryParse(textBoxCcy2RetractedNotes.Text, out Ccy2RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy2RetractedAmount.Text, out Ccy2RetractedAmount)) ;

                if (int.TryParse(textBoxCcy2RecycledNotes.Text, out Ccy2RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy2RecycledAmount.Text, out Ccy2RecycledAmount)) ;


                textBoxCcy2CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy2CassetteNotes).ToString("#0");
                textBoxCcy2CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy2CassetteAmount).ToString("#0.00");

                textBoxCcy2RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy2RetractedNotes).ToString("#0");
                textBoxCcy2RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy2RetractedAmount).ToString("#0.00");

                textBoxCcy2RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy2RecycledNotes).ToString("#0");
                textBoxCcy2RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy2RecycledAmount).ToString("#0.00");

                // UPDATE COUNTED

                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_2);


            }

            if (IsCcy_3 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_3);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_3);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy3CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy3CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                // 
                if (decimal.TryParse(textBoxCcy3CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy3CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                // Retracted 
                if (int.TryParse(textBoxCcy3RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy3RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy3RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy3RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy3RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy3RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy3RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy3RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy3CassetteNotes;
                decimal Ccy3CassetteAmount;

                int Ccy3RetractedNotes;
                decimal Ccy3RetractedAmount;

                int Ccy3RecycledNotes;
                decimal Ccy3RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy3CassetteNotes.Text, out Ccy3CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy3CassetteAmount.Text, out Ccy3CassetteAmount)) ;

                if (int.TryParse(textBoxCcy3RetractedNotes.Text, out Ccy3RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy3RetractedAmount.Text, out Ccy3RetractedAmount)) ;

                if (int.TryParse(textBoxCcy3RecycledNotes.Text, out Ccy3RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy3RecycledAmount.Text, out Ccy3RecycledAmount)) ;


                textBoxCcy3CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy3CassetteNotes).ToString("#0");
                textBoxCcy3CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy3CassetteAmount).ToString("#0.00");

                textBoxCcy3RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy3RetractedNotes).ToString("#0");
                textBoxCcy3RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy3RetractedAmount).ToString("#0.00");

                textBoxCcy3RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy3RecycledNotes).ToString("#0");
                textBoxCcy3RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy3RecycledAmount).ToString("#0.00");

                // UPDATE COUNTED

                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_3);

            }

            if (IsCcy_4 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_4);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_4);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy4CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy4CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                // 
                if (decimal.TryParse(textBoxCcy4CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy4CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                // Retracted 
                if (int.TryParse(textBoxCcy4RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy4RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy4RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy4RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy4RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy4RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy4RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy4RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy4CassetteNotes;
                decimal Ccy4CassetteAmount;

                int Ccy4RetractedNotes;
                decimal Ccy4RetractedAmount;

                int Ccy4RecycledNotes;
                decimal Ccy4RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy4CassetteNotes.Text, out Ccy4CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy4CassetteAmount.Text, out Ccy4CassetteAmount)) ;

                if (int.TryParse(textBoxCcy4RetractedNotes.Text, out Ccy4RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy4RetractedAmount.Text, out Ccy4RetractedAmount)) ;

                if (int.TryParse(textBoxCcy4RecycledNotes.Text, out Ccy4RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy4RecycledAmount.Text, out Ccy4RecycledAmount)) ;


                textBoxCcy4CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy4CassetteNotes).ToString("#0");
                textBoxCcy4CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy4CassetteAmount).ToString("#0.00");

                textBoxCcy4RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy4RetractedNotes).ToString("#0");
                textBoxCcy4RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy4RetractedAmount).ToString("#0.00");

                textBoxCcy4RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy4RecycledNotes).ToString("#0");
                textBoxCcy4RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy4RecycledAmount).ToString("#0.00");
                // UPDATE
                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_4);

            }
            if (IsCcy_5 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_5);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_5);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy5CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy5CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                // 
                if (decimal.TryParse(textBoxCcy5CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy5CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                // Retracted 
                if (int.TryParse(textBoxCcy5RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy5RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy5RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy5RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy5RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy5RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy5RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy5RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy5CassetteNotes;
                decimal Ccy5CassetteAmount;

                int Ccy5RetractedNotes;
                decimal Ccy5RetractedAmount;

                int Ccy5RecycledNotes;
                decimal Ccy5RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy5CassetteNotes.Text, out Ccy5CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy5CassetteAmount.Text, out Ccy5CassetteAmount)) ;

                if (int.TryParse(textBoxCcy5RetractedNotes.Text, out Ccy5RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy5RetractedAmount.Text, out Ccy5RetractedAmount)) ;

                if (int.TryParse(textBoxCcy5RecycledNotes.Text, out Ccy5RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy5RecycledAmount.Text, out Ccy5RecycledAmount)) ;


                textBoxCcy5CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy5CassetteNotes).ToString("#0");
                textBoxCcy5CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy5CassetteAmount).ToString("#0.00");

                textBoxCcy5RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy5RetractedNotes).ToString("#0");
                textBoxCcy5RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy5RetractedAmount).ToString("#0.00");

                textBoxCcy5RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy5RecycledNotes).ToString("#0");
                textBoxCcy5RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy5RecycledAmount).ToString("#0.00");
                // UPDATE
                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_5);

            }

            if (IsCcy_6 == true)
            {
                // Check if there is record for counted 
                SM.Read_SM_Record_SpecificDepositsCounted(WAtmNo, WSesNo, Ccy_6);
                if (SM.RecordFound == true)
                {
                    // OK 
                }
                else
                {
                    // Insert empty Record 
                    SM.InsertCountedRecord(WAtmNo, WSesNo, Ccy_6);
                }
                // Cassettes
                if (int.TryParse(textBoxCcy6CassetteNotesCount.Text, out SM.TotalCassetteNotesCount))
                {
                    // MessageBox.Show(textBox9.Text, "The input number is correct!"); 
                }
                else
                {
                    MessageBox.Show(textBoxCcy6CassetteNotesCount.Text, "Please enter a valid number!");
                    return;
                }
                // 
                if (decimal.TryParse(textBoxCcy6CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
                {
                    //  MessageBox.Show(textBox10.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy6CassetteAmountCount.Text, "Please enter a valid number!");
                    return;
                }

                // Retracted 
                if (int.TryParse(textBoxCcy6RetractedNotesCount.Text, out SM.TotalRetractedNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy6RetractedNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy6RetractedAmountCount.Text, out SM.TotalRetractedAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy6RetractedAmountCount.Text, "Please enter a valid number!");
                    return;
                }
                // RECYCLED 
                // 
                if (int.TryParse(textBoxCcy6RecycledNotesCount.Text, out SM.TotalRecycledNotesCount))
                {
                    //  MessageBox.Show(textBox11.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy6RecycledNotesCount.Text, "Please enter a valid number!");
                    return;
                }

                if (decimal.TryParse(textBoxCcy6RecycledAmountCount.Text, out SM.TotalRecycledAmountCount))
                {
                    //  MessageBox.Show(textBox1.Text, "The input number is correct!");
                }
                else
                {
                    MessageBox.Show(textBoxCcy6RecycledAmountCount.Text, "Please enter a valid number!");
                    return;
                }


                int Ccy6CassetteNotes;
                decimal Ccy6CassetteAmount;

                int Ccy6RetractedNotes;
                decimal Ccy6RetractedAmount;

                int Ccy6RecycledNotes;
                decimal Ccy6RecycledAmount;
                //****************************************


                if (int.TryParse(textBoxCcy6CassetteNotes.Text, out Ccy6CassetteNotes)) ;
                if (decimal.TryParse(textBoxCcy6CassetteAmount.Text, out Ccy6CassetteAmount)) ;

                if (int.TryParse(textBoxCcy6RetractedNotes.Text, out Ccy6RetractedNotes)) ;
                if (decimal.TryParse(textBoxCcy6RetractedAmount.Text, out Ccy6RetractedAmount)) ;

                if (int.TryParse(textBoxCcy6RecycledNotes.Text, out Ccy6RecycledNotes)) ;
                if (decimal.TryParse(textBoxCcy6RecycledAmount.Text, out Ccy6RecycledAmount)) ;


                textBoxCcy6CassetteNotesDiff.Text = (SM.TotalCassetteNotesCount - Ccy6CassetteNotes).ToString("#0");
                textBoxCcy6CassetteAmountDiff.Text = (SM.TotalCassetteAmountCount - Ccy6CassetteAmount).ToString("#0.00");

                textBoxCcy6RetractedNotesDiff.Text = (SM.TotalRetractedNotesCount - Ccy6RetractedNotes).ToString("#0");
                textBoxCcy6RetractedAmountDiff.Text = (SM.TotalRetractedAmountCount - Ccy6RetractedAmount).ToString("#0.00");

                textBoxCcy6RecycledNotesDiff.Text = (SM.TotalRecycledNotesCount - Ccy6RecycledNotes).ToString("#0");
                textBoxCcy6RecycledAmountDiff.Text = (SM.TotalRecycledAmountCount - Ccy6RecycledAmount).ToString("#0.00");
                // UPDATE
                SM.Update_SM_Deposit_analysis_Counted(WAtmNo, WSesNo, Ccy_6);

            }
        }



        // Show Deposits 
        private void button1_Click(object sender, EventArgs e)
        {

            NForm38_CDM = new Form38_CDM(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo);
            NForm38_CDM.Show();
        }
        //
        // My Count Change


        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            SetSteplevel();
        }

        // SET step level to need update if changes 
        private void SetSteplevel()
        {
            if (WSetScreen == false)
            {
                // Update STEP

                Usi.ReadSignedActivityByKey(WSignRecordNo);

                Usi.ReplStep3_Updated = false;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                button5.Show();
            }
        }

        private void textBox33_TextChanged_1(object sender, EventArgs e)
        {

        }

        // Show 
        decimal DiffGL;
        private void buttonShowNotes_Click(object sender, EventArgs e)
        {
            Form78d_DepositedNotes NForm78d_DepositedNotes;
            NForm78d_DepositedNotes = new Form78d_DepositedNotes(WOperator, WSignedId, WAtmNo, WSesNo, 2);
            NForm78d_DepositedNotes.Show();
        }
        // Create Action Occurances
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances()
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            decimal CassetteAmt;
            decimal RetractedAmt;
            //

            //  decimal RecycledAmt;

            if (decimal.TryParse(textBoxCcy1CassetteAmountCount.Text, out SM.TotalCassetteAmountCount))
            {
            }
            else
            {
            }
            if (decimal.TryParse(textBoxCcy1RetractedAmount.Text, out RetractedAmt))
            {
            }
            else
            {
            }

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

            DoubleEntryAmt = SM.TotalCassetteAmountCount;
            WUniqueRecordId = WSesNo; // SesNo 
            WCcy = "EGP";
            string WMaker_ReasonOfAction;

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
                WActionId = "26";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }


            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "37";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            DiffGL = 0;

            decimal CassetteAmtDiff;
            decimal RetractedAmtDiff;

            if (decimal.TryParse(textBoxCcy1CassetteAmountDiff.Text, out CassetteAmtDiff))
            {
            }
            else
            {
            }
            if (decimal.TryParse(textBoxCcy1RetractedAmountDiff.Text, out RetractedAmtDiff))
            {
            }
            else
            {
            }

            DiffGL = CassetteAmtDiff;
            //DiffGL = CassetteAmtDiff + RetractedAmtDiff;

            if (DiffGL == 0)
            {
                // do nothing
            }

            if (DiffGL > 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                        + "Will be moved to the CIT excess account ");
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = DiffGL;
                WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (DiffGL < 0)
            {
                MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                        + "Will be moved to the CIT shortage account ");
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }
                if (HybridRepl == true)
                {
                    WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -DiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            //WTotalForCust = SM.TotalForCust;
            //WTotalCommision = SM.TotalCommision;
            if (HybridRepl == false)
            {
                if (WTotalForCust > 0)
                {
                    // CREATE TRANSACTIONS FOR FOREX 
                    DoubleEntryAmt = WTotalForCust;
                    // FIRST DOUBLE ENTRY 
                    WActionId = "33"; // 33_CREDIT_FOREX_INTERMEDIARY/DR_ATM CASH
                                      // WUniqueRecordIdOrigin = "Replenishment";
                    WUniqueRecordId = WSesNo; // SesNo 
                    WCcy = "EGP";
                    //DoubleEntryAmt = Na.Balances1.CountedBal;
                    WMaker_ReasonOfAction = "UnLoad Deposits Forex";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");


                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                    // FOREX
                    // FOREX 
                    // FOREX
                    if (WTotalCommision > 0)
                    {
                        // CREATE TRANSACTIONS FOR FOREX Commision 
                        DoubleEntryAmt = WTotalCommision;
                        // FIRST DOUBLE ENTRY 
                        WActionId = "34"; // 34_CREDIT_FOREX_INTERMEDIARY/DR_Commision
                                          // WUniqueRecordIdOrigin = "Replenishment";
                        WUniqueRecordId = WSesNo; // SesNo 
                        WCcy = "EGP";
                        //DoubleEntryAmt = Na.Balances1.CountedBal;
                        WMaker_ReasonOfAction = "UnLoad Deposits Forex";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");


                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }

                    //// CREATE TRANSACTIONS FOR FOREX CIT
                    //DoubleEntryAmt = WTotalForCust + WTotalCommision;
                    //// FIRST DOUBLE ENTRY 
                    //WActionId = "35"; // 35_CREDIT_CIT ACCOUNT GL/DR_Forex_Intermidiary(DEPOSITS)
                    //                  // WUniqueRecordIdOrigin = "Replenishment";
                    //WUniqueRecordId = WSesNo; // SesNo 
                    //WCcy = "EGP";
                    ////DoubleEntryAmt = Na.Balances1.CountedBal;
                    //WMaker_ReasonOfAction = "UnLoad Deposits Forex";
                    //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                    //                                      WActionId, WUniqueRecordIdOrigin,
                    //                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
                    //                                      , WMaker_ReasonOfAction, "Replenishment");


                    //TEMPTableFromAction = Aoc.TxnsTableFromAction;

                }
            }  

        }


        // HOW GL TILL NOW 
        private void buttonShowGL_Click(object sender, EventArgs e)
        {
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string WSelectionCriteria = "WHERE AtmNo ='" + WAtmNo + "' AND ReplCycle =" + WSesNo + " AND OriginWorkFlow ='Replenishment'";
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

        // Link to exceptions
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
        // Show differences 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ValidateInputAndShowDifferences();
        }

        private void buttonSeeForex_Click(object sender, EventArgs e)
        {
            int Mode = 3;
            Form78d_DepositedNotes NForm78d_DepositedNotes;
            NForm78d_DepositedNotes = new Form78d_DepositedNotes(WOperator, WSignedId, WAtmNo, WSesNo, Mode);
            NForm78d_DepositedNotes.Show();

        }
        // Link to unmatched this cycle 
        private void linkLabelUnmatchedTxns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
            Form80b2_Unmatched NForm80b2;
            string WFunction = "View";

            // Show For Current Cycle number 
            NForm80b2 = new Form80b2_Unmatched(WSignedId, WSignRecordNo, WOperator, WFunction, WSesNo,
                                                WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2
                                                  );
            NForm80b2.Show();

        }
    }
}

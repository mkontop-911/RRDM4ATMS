using System;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMRepl_SupervisorMode_Master : Logger
    {
        public RRDMRepl_SupervisorMode_Master() : base() { }

        public bool RecordFound;
        public bool Major_ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMSessionsNewSession Sa = new RRDMSessionsNewSession(); // NEW SESSION CLASS 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMCashInOut Ct = new RRDMCashInOut();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDMPerformanceTraceClass Perform = new RRDMPerformanceTraceClass();

        RRDMReplStatsClass Rs = new RRDMReplStatsClass();


        public DataTable TableAtms = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        // ********INSERT BY PANICOS**************
        DateTime BeforeCallDtTime;
        // *************************BY PANICOS********************************


        string Message;

        bool ShowMessage;

        DateTime TRanDate;

        bool IsRepeat; // When a record comes for a journal we thought is lost . 
                       // and we have already created a record for it 

        int WSaveSesNo;

        int Cassette1;
        int Cassette2;
        int Cassette3;
        int Cassette4;

        //int CurrentSessionNo;

        int TraceNo;

        //   bool OwnCard;
        //  bool CreateReversal;

        //   string SqlString1_1 = "";
        //  string SqlString1_2 = "";

        //   string SqlString = "";

        int MasterTraceNo = 0;


        int StartTrxn;
        int EndTrxn;

        int OldTraceNo;

        // string Wtrandesc;

        // string WCitId;

        int WSesNo;

        int WPreviousSesNo;

        string CurrDesc;

        public int TotalValidRecords = 0;
        public int TotalTxns = 0;
        public int GrandTotalTxns;

        int WLoadedAtRMCycle;


        string WSignedId;
        int WSignRecordNo;
        //string WBankId;

        string WOperator;
        //  int WAtmsReconcGroup;
        string WAtmNo;
        int WFuid;

        int WMode;
        bool From_Form153;
        public void CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(string InSignedId, int InSignRecordNo, string InOperator,
                                                             int InMode, string InFrom)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;


            WMode = InMode; // 3 Normal Process that takes the last in progress cycle and creates a new one
                            // ..... 3 is called from the loading Form  
                            // 3 is used also if the last Journal is lost. Process Mode = -1 // Called from Form153
                            // 2 Create a Replenishment Cycle for A missing Journal or delayed journalin the middle
                            //   You do not create second cycle 
                            // 4 is USED for correction coming from 153/154
            if (InFrom == "Form153")
            {
                From_Form153 = true;
            }
            else
            {
                From_Form153 = false;
            }

            string SQLString;

            try
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                // int WReconcCycleNo;

                WLoadedAtRMCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            // Check if Supervisor Mode is active
            string ParId = "720";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string SM_Management = Gp.OccuranceNm;

            try
            {

                if ((WMode == 3 || WMode == 2 || WMode == 4) & SM_Management == "YES")
                {
                    //
                    // Change the NULLs from Pambos 
                    // To be removed after Pambos clear up 
                    //RRDM_Supervisor_Mode SM = new RRDM_Supervisor_Mode();
                    SM.Update_SM_Record_NULL_Values_ONE(WLoadedAtRMCycle); // This is done for non processed
                    SM.Update_SM_Record_NULL_Values_TWO();
                    SM.Update_SM_Record_NULL_Values_THREE();

                    string SM_Mode;
                    // Then Read all records insert them in table and process accordingly. 
                    string SM_SelectionCriteria2 = " WHERE "
                                              + " RRDM_Processed = 0 AND (FlagValid = 'Y' OR FlagValid = 'A') "
                                                 ;
                    string SM_OrderBy = " ORDER By AtmNo, SeqNo, SM_dateTime_Start ASC ";

                    SM.ReadT_SM_AND_FillTable(SM_SelectionCriteria2, SM_OrderBy);

                    int I = 0;

                    while (I <= (SM.DataTable_SM.Rows.Count - 1))
                    {

                        IsRepeat = false;
                        Correct_Process5 = false;
                        Correct_Process6 = false;
                        Correct_Process0 = false;
                        //
                        // This table contains all Superviosr mode Actions by ATM
                        // Here we are creating the necessary records for the SM 
                        //
                        int WSeqNo = (int)SM.DataTable_SM.Rows[I]["SeqNo"];
                        //
                        // Check Here if Journal is valid for Totals 
                        //
                        SM.Read_SM_Record_Specific(WSeqNo); // read fields and also if valid 

                        // At this call we check that if the record is correct 
                        // If not valid we mark it as -6 

                        WAtmNo = SM.AtmNo;

                        //if (WAtmNo == "00000828")
                        //{
                        //    // OK Move on 
                        //}
                        //else
                        //{
                        //    I++; // Read Next entry of the table 
                        //    continue;
                        //}

                        //if (WAtmNo == "00000828")
                        //{
                        //    int temp = 4;
                        //}

                        if (SM.AdditionalCash == "N") // THIS IS NOT FOR ADDING CASH ONLY
                        {
                            // HERE WE HAVE ONLY THE ONES THAT HAVE FULL REPLENISHMENT = FlagValid = 'Y'
                            // This is a normal replenishement where cash is cleared 

                            // SM-START
                            SM_Mode = "SM-START";
                            TRanDate = SM.SM_dateTime_Start; // Set TRanDate as it is needed

                            if (From_Form153 == true)
                            {
                                // Do not correct date
                                // Already trim by user 
                            }
                            else
                            {
                                TRanDate = TRanDate.AddMinutes(2); // Add minutes to avoid cases 
                                                                   // Where previous txn has the same time
                                                                   // As the supervisor mode
                            }

                            TraceNo = Convert.ToInt32(SM.previous_Repl_trace);
                            //TraceNo = Convert.ToInt32(SM.previous_Repl_trace);

                            Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                            if (IsRepeat == false) //
                            {
                                // If there is repeat gets the value from previous method. 
                                // SM-TOTALS - WHAT ORIGINAL WAS IN CASSETTES AND START DATE AS Last Clear
                                SM_Mode = "SM-TOTALS";
                                Cassette1 = SM.ATM_total1;
                                Cassette2 = SM.ATM_total2;
                                Cassette3 = SM.ATM_total3;
                                Cassette4 = SM.ATM_total4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-CASSETTE - What cassettes currently have 
                                SM_Mode = "SM-CASSETTE";
                                Cassette1 = SM.ATM_cassette1;
                                Cassette2 = SM.ATM_cassette2;
                                Cassette3 = SM.ATM_cassette3;
                                Cassette4 = SM.ATM_cassette4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-REJECTED - What had gone in the rejected tray 
                                SM_Mode = "SM-REJECTED";
                                Cassette1 = SM.ATM_Rejected1;
                                Cassette2 = SM.ATM_Rejected2;
                                Cassette3 = SM.ATM_Rejected3;
                                Cassette4 = SM.ATM_Rejected4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-REMAINING ( Cassettes + Rejected ) 
                                // We do not keep this in RRDM We already have Cassettes and the Rejected 
                                SM_Mode = "SM-REMAINING";
                                Cassette1 = SM.ATM_Remaining1;
                                Cassette2 = SM.ATM_Remaining2;
                                Cassette3 = SM.ATM_Remaining3;
                                Cassette4 = SM.ATM_Remaining4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-DISPENCED - Money given to people 
                                SM_Mode = "SM-DISPENCED";
                                Cassette1 = SM.ATM_Dispensed1;
                                Cassette2 = SM.ATM_Dispensed2;
                                Cassette3 = SM.ATM_Dispensed3;
                                Cassette4 = SM.ATM_Dispensed4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-CASH ADDED - Here we create a new Replenishment Cycle and we update the 
                                SM_Mode = "SM-CASH ADDED";
                                Cassette1 = SM.cashaddtype1;
                                Cassette2 = SM.cashaddtype2;
                                Cassette3 = SM.cashaddtype3;
                                Cassette4 = SM.cashaddtype4;
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                                // SM-END - HERE we create the new Replenishment Cycle 
                                SM_Mode = "SM-END";
                                TRanDate = SM.SM_dateTime_Finish; // Set TRanDate as it is needed

                                if (From_Form153 == true)
                                {
                                    // Do not correct date
                                    // Already trim by user 
                                }
                                else
                                {
                                    TRanDate = TRanDate.AddMinutes(-1); // Subtract minutes to avoid cases 
                                                                        // Where next  txn has the same time
                                                                        // As the supervisor mode finish 
                                }

                                TraceNo = Convert.ToInt32(SM.after_Repl_trace);
                                Method_HandlingSupervisorMode(WAtmNo, SM_Mode);
                            }

                            // Update SM record as process. 

                            SM.RRDM_ReplCycleNo = WSaveSesNo;
                            SM.RRDM_DateTime_Created = DateTime.Now;
                            SM.LoadedAtRMCycle = WLoadedAtRMCycle;
                            SM.RRDM_Processed = true;

                            SM.Update_SM_Record(WSeqNo);

                            // UPDATE HERE "SM_Deposit_analysis with Replenishment Cycle"
                            SM.Update_SM_Deposit_analysis(WAtmNo, SM.Fuid);
                            // Replenishment dates
                            Ta.ReadSessionsStatusTraces(WAtmNo, WSaveSesNo);
                            Ta.Repl1.ReplStartDtTm = SM.SM_dateTime_Start;
                            Ta.Repl1.ReplFinDtTm = SM.SM_dateTime_Finish;
                            Ta.UpdateSessionsStatusTraces(WAtmNo, WSaveSesNo);

                            // DEAL WITH RECORDS IN RELATION TO BANK RECORD FOR CIT EXCEL 

                            //**************************************************************************************************
                            //Insert Basic Info For G4S RECORD - BANK RECORD - SESSION UNDER COMPLETION 
                            //************************************************************************************************** 
                            // Create CIT Entries in Creating SM Master
                            ParId = "949"; // Create CIT Entries in Creating SM Master
                            OccurId = "1";
                            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                            //
                            // UPDATE ORDERS IF ANY 
                            // IF YES a) UPDATE BANK RECORD TO BE USED FOR EXCEL
                            // b) CREATE TRANSACTIONS FOR CIT
                            //
                            if (Gp.RecordFound & Gp.OccuranceNm == "YES" & Ta.ProcessMode == 0)
                            {

                                //
                                // UPDATE Action TABLE WITH ACTUAL  MONEY Loaded
                                // Old SessionNo 
                                // 
                                Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSaveSesNo);

                                if (Ro.RecordFound)
                                {
                                    // IF ORDER HAS BEEN CREATED FOR THIS REPL CYCLE
                                    Ro.PassReplCycle = true;
                                    Ro.PassReplCycleDate = Ta.Repl1.ReplFinDtTm;
                                    Ro.InMoneyReal = CashLoaded; // from ATMs Registers

                                    Ro.ActiveRecord = false;

                                    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);
                                }

                                RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();
                                RRDMDepositsClass Da = new RRDMDepositsClass();


                                // Get the totals from SM and not from Mpa            
                                // GET TABLE
                                // SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, WSesNo, SM.Fuid);
                                bool DepositsDone = false;

                                string WCurrency = "EGP";
                                SM.Read_SM_AND_FillTable_Deposits_2_FOR_Ccy(WAtmNo, WSaveSesNo, WCurrency);

                                // decimal TotalDeposits = Da.DepositsMachine1.Amount + Da.ChequesMachine1.Amount + Da.DepositsMachine1.Envelops;
                                if (SM.RecordFound == true)
                                {
                                    DepositsDone = true;

                                    Da.DepNotesMach = SM.TotalCassetteNotes;
                                    Da.DepositsMachine1.Amount = SM.TotalCassetteMoney;

                                    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSaveSesNo); // Update Totals 
                                }

                                // UPDATE Na with Capture Cards if any 

                                RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();

                                Cc.ReadCaptureCardsTableByDatesTm_Range(WOperator, WAtmNo,
                                  Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 20);

                                if (Cc.CaptureCardsNo > 0)
                                {
                                    //Na.ReadNotesSesionsBySesNo(WSaveSesNo);

                                    Na.ReadSessionsNotesAndValues(WAtmNo, WSaveSesNo, 12);

                                    Na.CaptCardsMachine = Cc.CaptureCardsNo;

                                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSaveSesNo);
                                }

                                if (Ac.CitId != "1000")
                                {
                                    // This ATM Is COVERED BY GROUP 4
                                    int G4Record_ATM;
                                    int G4Record_CDM = 0;
                                    bool IsDeposit = false;
                                    IsDeposit = false;

                                    // The below goes through the G4 class. 
                                    G4Record_ATM = Insert_G4S_Record(WAtmNo, WSaveSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record

                                    //if (DepositsDone == true)
                                    //{
                                    //    IsDeposit = true;
                                    //    G4Record_CDM = Insert_G4S_Record(WAtmNo, WSaveSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record
                                    //}

                                    //**************************************************************************************************
                                    //UPDATE THE G4S RECORD - SESSION UNDER COMPLETION 
                                    //************************************************************************************************** 

                                    int TempMode = 2; // Bank FILE
                                    G4.ReadCIT_G4S_Repl_EntriesBySeqNo(G4Record_ATM, TempMode); // Note that this entry has SeqNo = SesNo

                                    if (G4.RecordFound == true)
                                    {
                                        if (Ro.RecordFound == true)
                                        {
                                            G4.OrderNo = Ro.ReplOrderNo;
                                            G4.OrderToBeLoaded = Ro.SystemAmount;
                                        }
                                        else
                                        {
                                            G4.OrderNo = 0;
                                        }

                                        G4.OpeningBalance = OpenBalance;

                                        G4.Dispensed = Dispensed;

                                        G4.UnloadedMachine = CashInCassettes + Rejected;

                                        G4.UnloadedCounted = 0; // This is Zero

                                        G4.Deposits_Notes_Denom_1 = 0;

                                        G4.Deposits = SM.TotalCassetteMoney;

                                        G4.Cash_Loaded = CashLoaded;


                                        //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                                        //G4.PresentedErrors = Na.Balances1.PresenterValue;

                                        G4.OtherJournalErrors = 0;

                                        G4.RemarksRRDM = "";

                                        G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_ATM, TempMode);

                                        //**************************************************************************************************
                                        //Insert Posted Transactions For CIT  for CASH Un=loaded and Loaded
                                        // DURING EXCEL Loading we do the same. 
                                        // THINK ABOUT THIS 
                                        //************************************************************************************************** 
                                        Ac.ReadAtm(WAtmNo);
                                        IsDeposit = false;

                                        if (Ac.CitId != "1000")
                                        {
                                            Insert_Posted_Txns_For_CIT(Ac.CitId, WAtmNo,
                                                 WSaveSesNo, G4.UnloadedMachine + G4.Deposits, G4.Cash_Loaded,
                                                G4.Cut_Off_date, WLoadedAtRMCycle,
                                                              IsDeposit);  // Post the Txns
                                        }
                                        bool dontknowwhy = true;
                                        if (dontknowwhy == false)
                                        {
                                            if (DepositsDone == true)
                                            {
                                                TempMode = 2; // Bank FILE
                                                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(G4Record_CDM, TempMode); // Note that this entry has SeqNo = SesNo

                                                if (G4.RecordFound == true)
                                                {
                                                    if (Ro.RecordFound == true)
                                                    {
                                                        //G4.OrderNo = Ra.ReplActNo;
                                                        //G4.OrderToBeLoaded = Ra.SystemAmount;
                                                    }
                                                    else
                                                    {
                                                        G4.OrderNo = 0;
                                                    }

                                                    G4.OpeningBalance = 0;

                                                    G4.Dispensed = 0;

                                                    G4.UnloadedMachine = 0;

                                                    G4.UnloadedCounted = 0; // This is Zero

                                                    G4.Cash_Loaded = 0;

                                                    G4.Deposits = SM.TotalCassetteMoney;

                                                    //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                                                    //G4.PresentedErrors = Na.Balances1.PresenterValue;

                                                    G4.OtherJournalErrors = 0;

                                                    G4.RemarksRRDM = "";

                                                    G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_CDM, TempMode);

                                                    //**************************************************************************************************
                                                    //Insert Posted Transactions For CIT  for Deposit Machines 
                                                    // The CIT Account is updated as well as the Bank's account 
                                                    // DURING EXCEL Loading we do the same. 
                                                    // THINK ABOUT THIS 
                                                    //************************************************************************************************** 
                                                    Ac.ReadAtm(WAtmNo);
                                                    IsDeposit = true;

                                                    if (Ac.CitId != "1000")
                                                    {
                                                        Insert_Posted_Txns_For_CIT(Ac.CitId, WAtmNo,
                                                             WSaveSesNo, G4.UnloadedMachine, G4.Cash_Loaded,
                                                            TRanDate, WLoadedAtRMCycle,
                                                                          IsDeposit);  // Post the Txns
                                                    }
                                                }
                                            }
                                        }



                                    }



                                } // HERE FINISH the CIT

                            }


                        }
                        // ********************
                        // The Record has only additional Cash 
                        // ********************
                        if (SM.AdditionalCash == "Y") // ADDITIONAL CASH
                        {


                            SM.RRDM_ReplCycleNo = WSesNo;
                            SM.RRDM_DateTime_Created = DateTime.Now;
                            SM.RRDM_Processed = true;
                            SM.LoadedAtRMCycle = WLoadedAtRMCycle;
                            SM.Update_SM_Record(WSeqNo);

                            //I = I + 1;
                            //continue; 

                            //// FIND ACTIVE REPL CYCLE 
                            ////
                            //Ta.FindNextAndLastReplCycleId(WAtmNo);

                            //if (Ta.RecordFound == true)
                            //{
                            //    //ReplCyclefound = true;
                            //    WSesNo = Ta.LastNo;
                            //    WPreviousSesNo = Ta.PreSes;

                            //    // "SM-ADDITIONAL-CASH ADDED"
                            //    SM_Mode = "SM-ADDITIONAL-CASH ADDED";
                            //    Cassette1 = SM.cashaddtype1;
                            //    Cassette2 = SM.cashaddtype2;
                            //    Cassette3 = SM.cashaddtype3;
                            //    Cassette4 = SM.cashaddtype4;
                            //    Method_HandlingSupervisorMode(WAtmNo, SM_Mode);

                            //    // Update SM record as process. 

                            //    SM.RRDM_ReplCycleNo = WSesNo;
                            //    SM.RRDM_DateTime_Created = DateTime.Now;
                            //    SM.RRDM_Processed = true;
                            //    SM.LoadedAtRMCycle = WLoadedAtRMCycle;
                            //    SM.Update_SM_Record(WSeqNo);
                            //}
                            //else
                            //{
                            //    WSesNo = 0; // this true if only new operation 
                            //                // Update SM record as process. 

                            //    SM.RRDM_ReplCycleNo = WSesNo;
                            //    SM.RRDM_DateTime_Created = DateTime.Now;
                            //    SM.RRDM_Processed = true;
                            //    SM.LoadedAtRMCycle = WLoadedAtRMCycle;
                            //    SM.Update_SM_Record(WSeqNo);
                            //    //ReplCyclefound = false;
                            //}

                        }

                        I++; // Read Next entry of the table 
                    }
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            return;

        }

       
        //
        // Handling Supervisor Mode Cases
        //
        string W_SM_Mode;
        private void Method_HandlingSupervisorMode(string WAtmNo, string In_SM_Mode)
        {
            //*************************************************************
            // DEAL WITH NON TRANSACTIONS 
            // *******************
            // SM_Mode
            // *******************
            // SM-START
            // SM-CASSETTE
            // SM-REJECTED
            // SM-REMAINING
            // SM-DISPENCED
            // SM-TOTALS
            // SM-CASH ADDED
            // SM-END

            //*************************************************************
            W_SM_Mode = In_SM_Mode;

            // SUPERVISOR MODE STARTS - THE OLD REPL CYCLE ENDS
            if (W_SM_Mode == "SM-START") // SUPERVISOR MODE START = THIS IS THE END OF REPL CYCLE 
            {
                //Message = "Start Replenishment For ATM : " + WAtmNo + " Done on : " + TRanDate.ToString();

                //Perform.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", WMatchingCategoryId, BeforeCallDtTime, DateTime.Now, Message);
                //if (ShowMessage == true)
                //{
                //    System.Windows.Forms.MessageBox.Show(Message);
                //}

                Method_SupervisorMode_Start(WAtmNo);
            }

            if (W_SM_Mode == "SM-CASSETTE") // Cassette equivalent Gas Remaining = What it is in cassettes 
            {
                // HERE WE NEED DATE AND TIME 
                Method_SupervisorMode_CASSETTE(WAtmNo);
            }

            if (W_SM_Mode == "SM-REJECTED") // Rejected
            {
                Method_SupervisorMode_REJECTED(WAtmNo);
            }

            if (W_SM_Mode == "SM-REMAINING") // Cash OUT FROM MACHINE ( CASSETTE + REJECTED )
            {
                // IGNORE THIS WE HAVE ALL INFO ELSE WHERE
                // Method_SupervisorMode_REMAINING(WAtmNo);
                // CALL CASH OUT CLASS 
                // Create transaction for 
                // a) ATM Pool
                // b) CIT provider STATEMENT 

                Ct.InsertTranForCashOut(WSignedId, WSignRecordNo, WOperator,
                    WAtmNo, WSesNo, StartTrxn, EndTrxn, MasterTraceNo);
            }

            if (W_SM_Mode == "SM-DISPENCED") // Dispensed
            {
                Method_SupervisorMode_DISPENCED(WAtmNo);
            }

            // Totals Line 
            if (W_SM_Mode == "SM-TOTALS")
            {

                // House Keeping now that we have all info Available 
                Ac.ReadAtm(WAtmNo);


                //if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no SesNo No 
                //{
                //    Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                //    OpenBalance = Na.Cassettes_1.InNotes * Na.Cassettes_1.FaceValue
                //        + Na.Cassettes_2.InNotes * Na.Cassettes_2.FaceValue
                //        + Na.Cassettes_3.InNotes * Na.Cassettes_3.FaceValue
                //        + Na.Cassettes_4.InNotes * Na.Cassettes_4.FaceValue;
                //}

                OpenBalance = Cassette1 * Ac.FaceValue_11
                                 + Cassette2 * Ac.FaceValue_12
                                 + Cassette3 * Ac.FaceValue_13
                                 + Cassette4 * Ac.FaceValue_14;

                // Update only for New ATM Money in cassettes and update Na 

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 12);

                if (IsNewATM == true & WMode == 3) Na.IsNewAtm = true;
                else Na.IsNewAtm = false;

                Na.Cassettes_1.InNotes = Cassette1;
                Na.Cassettes_2.InNotes = Cassette2;
                Na.Cassettes_3.InNotes = Cassette3;
                Na.Cassettes_4.InNotes = Cassette4;

                Na.ReplAmountTotal = OpenBalance;
                Na.InReplAmount = OpenBalance;

                Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);


                //  

                //}
            }



            // Supervisor Mode Cash Added 
            if (W_SM_Mode == "SM-CASH ADDED") // Cash ADDED 
            {
                Ac.ReadAtm(WAtmNo);

                CashLoaded = Cassette1 * Ac.FaceValue_11
                           + Cassette2 * Ac.FaceValue_12
                           + Cassette3 * Ac.FaceValue_13
                           + Cassette4 * Ac.FaceValue_14;

                if (WSesNo == 0 & OpenBalance == 0)
                {
                    // 
                    // Create new Psaudo Session for this BRAND NEW ATM
                    // 
                    // THIS THE CASE WHERE THERE IS ONLY CASH ADDED IN THE Supervisor Mode
                    // 
                    IsNewATM = true;

                    // If it is equal to zero it means this is the first time and there is no SesNo No 
                    // MAKE ATM ACTIVE 
                    Ac.ReadAtm(WAtmNo);
                    Ac.ActiveAtm = true;
                    Ac.UpdateAtmsBasic(WAtmNo);

                    // Create a pseaudo cycle  

                    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
                                                                                                        // => NEW cycle
                    WSesNo = Sa.NewSessionNo;
                }

                // Method_SupervisorMode_CASHADDED(WAtmNo); Method_SupervisorMode_AdditionalCash_ADDED_To_Open_SessionNo(string InAtmNo, decimal InCashLoaded)

            }

            // SUPERVISOR MODE ENDS - A NEW REPL CYCLE of Transactions STARTS
            if (W_SM_Mode == "SM-END") // YESTERDAYS SM 
            {
                // All operation had been moved to Cash Added

                // At the right END we do the following 

                if (Ac.NoCassettes > 0)
                {
                    Method_SupervisorMode_ATM_FINISH_SM(WAtmNo);
                }
                if (Ac.NoCassettes == 0)
                {
                    // THIS IS CASH DEPOSIT MACHINE 
                    Method_SupervisorMode_END_CDM(WAtmNo);
                }

            }

            // Supervisor Mode ADDITIONAL Cash Added 
            if (W_SM_Mode == "SM-ADDITIONAL-CASH ADDED") // ADDITIONAL Cash ADDED 
            {
                Method_SupervisorMode_AdditionalCash_ADDED_To_Open_SessionNo(WAtmNo);
            }

        }

        decimal OpenBalance;
        decimal Dispensed;
        decimal Rejected;
        decimal CashInCassettes;
        decimal CashLoaded; // Loaded 
        bool IsNewATM;
        DateTime SMStart;
        int SMStartTraceNo;
        bool Correct_Process5;
        bool Correct_Process6;
        bool Correct_Process0;
        bool FirstButCameLate;
        bool IsCameLate;

        // Supervisor Mode Start 
        private void Method_SupervisorMode_Start(string InAtmNo)
        {
            // Initialise Balances 
            OpenBalance = 0;
            Dispensed = 0;
            Rejected = 0;
            CashInCassettes = 0;
            CashLoaded = 0;

            IsNewATM = false;
            IsRepeat = false;
            Correct_Process6 = false;
            Correct_Process0 = false;
            Correct_Process5 = false;
            FirstButCameLate = false;
            IsCameLate = false;

            // Check if correction on SM 

            if (SM.RRDM_ReplCycleNo > 0)
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, SM.RRDM_ReplCycleNo);

                if (Ta.RecordFound == true)
                {
                    if (Ta.ProcessMode == -6)
                    {
                        // This a correction on Cycle 
                        Correct_Process6 = true;
                        WSesNo = Ta.SesNo;

                    }
                    if (Ta.ProcessMode == 0 & WMode == 4)
                    {
                        // This is a normal one 
                        Correct_Process0 = true;
                        WSesNo = Ta.SesNo;

                    }
                }
            }


            // Check if it is before the start of ATM Life 
            // Read Last Record
            Ta.ReadSessionsStatusTracesToFindFirstRecord(WAtmNo);

            if (Ta.RecordFound == true)
            {
                if (SM.SM_LAST_CLEARED.Date < Ta.SesDtTimeStart.Date & SM.SM_LAST_CLEARED.Date != NullPastDate)
                {
                    // Means is a journal that came late and already there are open cycles 
                    // You treat it as normal but you do not open a process mode = -1 
                    FirstButCameLate = true;
                }
            }
            else
            {
                FirstButCameLate = false;
            }

            Ta.FindNextAndLastReplCycleId(WAtmNo);

            if (Ta.RecordFound == true & FirstButCameLate == false & (WMode == 3 || WMode == 2)
                & Correct_Process6 == false & Correct_Process0 == false)
            {
                //ReplCyclefound = true;
                WSesNo = Ta.Last_1;
                Ta.ReadSessionsStatusTraces(WAtmNo, Ta.Last_1);
                WPreviousSesNo = Ta.PreSes;

                // CASE A of read invalids 

                if (SM.SM_LAST_CLEARED.Date > Ta.SesDtTimeStart.Date & Correct_Process6 == false)
                {
                    // There is a Gap In Journals 
                    // Work on the previous 
                    // Close the last -1 with code -5 
                    // Then move to the next complete
                    Ta.ReplGenComment = "Missing Journal with of Repl..at.." + SM.SM_LAST_CLEARED.ToString();

                    Ta.ProcessMode = -5; // Temporary out of action. 

                    // Correct end date too
                    Ta.SesDtTimeEnd = SM.SM_LAST_CLEARED;

                    Ta.UpdateSessionsStatusTraces(WAtmNo, Ta.Last_1);

                    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
                                                                                                        // => NEW cycle
                    WSesNo = Sa.NewSessionNo;
                }

                // CASE B

                if (SM.SM_LAST_CLEARED.Date < Ta.SesDtTimeStart.Date & Correct_Process6 == false & SM.SM_LAST_CLEARED.Date != NullPastDate)
                {
                    // FIND if the corresponding record

                    Ta.ReadSessionsStatusTracesToFindSesNoBasedDateAndProcessCode(WAtmNo, SM.SM_LAST_CLEARED.Date);

                    if (Ta.RecordFound == true & Ta.ProcessMode == -5)
                    {
                        Correct_Process5 = true;
                        WSesNo = Ta.SesNo;
                    }

                    if (Ta.RecordFound == true & Ta.ProcessMode != -5)
                    {
                        // If Process = 0 then no problem 
                        // If process = 2 then we leave it as it is
                        // SM.Read_SM_Record_Specific_By_Selection();
                        string TempSortBy = " ORDER BY SeqNo ASC ";
                        int SeqNo1 = SM.Read_SM_Find_Cycle_No_From_End_Date(WAtmNo, SM.SM_dateTime_Start, TempSortBy);

                        TempSortBy = " ORDER BY SeqNo DESC ";
                        int SeqNo2 = SM.Read_SM_Find_Cycle_No_From_End_Date(WAtmNo, SM.SM_dateTime_Start, TempSortBy);

                        SM.Read_SM_Record_Specific(SeqNo2);

                        // UPDATE fuid and the rest from second real record 

                        SM.Update_SM_RecordFromNewData(SeqNo1);

                        if (Ta.ProcessMode == 2)
                        {
                            // Check if incomming figures are the same as old record. 
                        }
                        //WMode = 2;
                        WSaveSesNo = WSesNo = Ta.SesNo;

                        IsRepeat = true;

                        // Do not do any processing 

                    }
                    // Late or created New Journal from 153
                    if (Ta.RecordFound == false & Ta.ProcessMode != -5)
                    {
                        // Late Journal 

                        Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
                                                                                                            // => NEW cycle
                        WSesNo = Sa.NewSessionNo;

                        IsCameLate = true;

                    }

                }

                //if (SM.SM_dateTime_Finish < Ta.SesDtTimeStart)
                //{
                //    // Turn WMode to 2
                //    // NOT AFFECT Existing but to create a new 
                //    WMode = 2;
                //    WSesNo = 0;
                //}

            }
            else
            {
                if (Correct_Process6 == false)
                {
                    if (Correct_Process0 == false)
                        WSesNo = 0; // For New ATM or when missing Journal
                }

            }

            //SMStart = TRanDate;
            SMStartTraceNo = TraceNo;

            // HERE WE NEED SesNo
            if (WSesNo == 0)
            {
                IsNewATM = true;
                // If it is equal to zero it means this is the first time and there is no SesNo No 
                // OR Case of Missing Cycle 
                // MAKE ATM ACTIVE 
                Ac.ReadAtm(WAtmNo);
                Ac.ActiveAtm = true;
                Ac.UpdateAtmsBasic(WAtmNo);

                // Create a pseaudo cycle  

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
                                                                                                    // => NEW cycle
                WSesNo = Sa.NewSessionNo;

            }
            // HERE WE NEED SesNo
            if (IsRepeat == false)
            {
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);


                if (SM.SM_LAST_CLEARED == NullPastDate)
                {
                    if (Ta.SesDtTimeStart == NullPastDate)
                    {
                        Ta.SesDtTimeStart = SM.SM_dateTime_Start;
                    }
                    else
                    {
                        Ta.SesDtTimeStart = Ta.SesDtTimeStart;
                    }

                }
                else
                {
                    if (From_Form153 == true)
                    {
                        // Do not correct date
                        // Already trim by user 
                        Ta.SesDtTimeStart = SM.SM_LAST_CLEARED;
                    }
                    else
                    {
                        Ta.SesDtTimeStart = SM.SM_LAST_CLEARED.AddMinutes(2);
                    }

                }
                if (From_Form153 == true)
                {
                    // Do not correct date
                    // Already trim by user 
                    Ta.SesDtTimeEnd = SM.SM_dateTime_Finish;
                }
                else
                {
                    Ta.SesDtTimeEnd = SM.SM_dateTime_Finish.AddMinutes(2);
                }

                Ta.Repl1.ReplStartDtTm = SM.SM_dateTime_Start;
                Ta.Repl1.ReplFinDtTm = SM.SM_dateTime_Finish;
                Ta.SM_LAST_CLEARED = SM.SM_LAST_CLEARED;

                if (Ta.ProcessMode == -5)
                {
                    Ta.ReplGenComment = Ta.ReplGenComment + " but came later at RMCycle.." + WLoadedAtRMCycle.ToString();
                }
                if (Ta.ProcessMode == -6)
                {
                    Ta.ReplGenComment = Ta.ReplGenComment + " but came later Corrected at RMCycle.." + WLoadedAtRMCycle.ToString();
                }

                if (Ta.ProcessMode == 0)
                {
                    Ta.ReplGenComment = " Values from Form 153 at RMCycle.." + WLoadedAtRMCycle.ToString();
                }

                if (Ta.ProcessMode == 2)
                {
                    // Do nothing
                }
                else
                {
                    Ta.ProcessMode = 0; // We turn from -1 to 0 
                                        // OR from -5 to 0 
                }

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

            }


        }

        // Supervisor Mode Cash Dispensed 
        private void Method_SupervisorMode_DISPENCED(string InAtmNo)
        {
            //// HERE WE NEED SesNo
            //if (WSesNo == 0)
            //{
            //    IsNewATM = true;
            //    // If it is equal to zero it means this is the first time and there is no SesNo No 
            //    // MAKE ATM ACTIVE 
            //    Ac.ReadAtm(WAtmNo);
            //    Ac.ActiveAtm = true;
            //    Ac.UpdateAtmsBasic(WAtmNo);

            //    // Create a pseaudo cycle  

            //    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
            //                                                                                        // => NEW cycle
            //    WSesNo = Sa.NewSessionNo;

            //}
            //// HERE WE NEED SesNo

            //// Update Replenishment Start 
            //Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            //Ta.Repl1.ReplStartDtTm = SMStart;
            //Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            //Ta.Repl1.ReplFinDtTm = InTRanDate;
            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12);

            if (IsNewATM == true & WMode == 3) Na.IsNewAtm = true;
            else Na.IsNewAtm = false;

            Na.Cassettes_1.DispNotes = Cassette1;
            Na.Cassettes_2.DispNotes = Cassette2;
            Na.Cassettes_3.DispNotes = Cassette3;
            Na.Cassettes_4.DispNotes = Cassette4;

            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);

            Dispensed = Na.Cassettes_1.DispNotes * Na.Cassettes_1.FaceValue
                        + Na.Cassettes_2.DispNotes * Na.Cassettes_2.FaceValue
                        + Na.Cassettes_3.DispNotes * Na.Cassettes_3.FaceValue
                        + Na.Cassettes_4.DispNotes * Na.Cassettes_4.FaceValue;
        }

        // Supervisor Mode Cash REJECTED
        private void Method_SupervisorMode_REJECTED(string InAtmNo)
        {
            // HERE WE NEED SesNo
            //if (WSesNo == 0)
            //{
            //    IsNewATM = true;
            //    // If it is equal to zero it means this is the first time and there is no SesNo No 
            //    // MAKE ATM ACTIVE 
            //    Ac.ReadAtm(WAtmNo);
            //    Ac.ActiveAtm = true;
            //    Ac.UpdateAtmsBasic(WAtmNo);

            //    // Create a pseaudo cycle  

            //    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
            //                                                                                        // => NEW cycle
            //    WSesNo = Sa.NewSessionNo;
            //}
            // HERE WE NEED SesNo

            if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no SesNo No 
            {
                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12);

                if (IsNewATM == true & WMode == 3) Na.IsNewAtm = true;
                else Na.IsNewAtm = false;

                Na.Cassettes_1.RejNotes = Cassette1;
                Na.Cassettes_2.RejNotes = Cassette2;
                Na.Cassettes_3.RejNotes = Cassette3;
                Na.Cassettes_4.RejNotes = Cassette4;

                Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);

                Rejected = Na.Cassettes_1.RejNotes * Na.Cassettes_1.FaceValue
                         + Na.Cassettes_2.RejNotes * Na.Cassettes_2.FaceValue
                         + Na.Cassettes_3.RejNotes * Na.Cassettes_3.FaceValue
                         + Na.Cassettes_4.RejNotes * Na.Cassettes_4.FaceValue;
            }

        }

        // Supervisor Mode Cash CASSETTE
        private void Method_SupervisorMode_CASSETTE(string InAtmNo)
        {
            // HERE WE NEED SesNo
            //if (WSesNo == 0)
            //{
            //    IsNewATM = true;

            //    // If it is equal to zero it means this is the first time and there is no SesNo No 
            //    // MAKE ATM ACTIVE 
            //    Ac.ReadAtm(WAtmNo);
            //    Ac.ActiveAtm = true;
            //    Ac.UpdateAtmsBasic(WAtmNo);

            //    // Create a pseaudo cycle  

            //    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 
            //                                                                                        // => NEW cycle
            //    WSesNo = Sa.NewSessionNo;
            //}


            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12);

            if (IsNewATM == true & WMode == 3) Na.IsNewAtm = true;
            else Na.IsNewAtm = false;
            //
            // Leave here : The Cassettes  we put them in the remaining 
            //
            Na.Cassettes_1.RemNotes = Cassette1; // Equivalent what it is in cassettes NOT REJected 
            Na.Cassettes_2.RemNotes = Cassette2;
            Na.Cassettes_3.RemNotes = Cassette3;
            Na.Cassettes_4.RemNotes = Cassette4;

            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);

            CashInCassettes = Na.Cassettes_1.RemNotes * Na.Cassettes_1.FaceValue
                              + Na.Cassettes_2.RemNotes * Na.Cassettes_2.FaceValue
                              + Na.Cassettes_3.RemNotes * Na.Cassettes_3.FaceValue
                              + Na.Cassettes_4.RemNotes * Na.Cassettes_4.FaceValue;
        }

        // Supervisor Mode Cash  REMAINING ( CASSETTE + REJECTED )
        private void Method_SupervisorMode_REMAINING(string InAtmNo)
        {
            //// Unloaded FROM MACHINE ( CASSETTE + REJECTED )
            //

            // DO NOTHING 
        }

        // Supervisor Mode CASH ADDED
        int TraceNoCashIn;
        private void Method_SupervisorMode_CASHADDED_To_New_SessionNo(string InAtmNo, decimal InCashLoaded)
        {

            // Update Session Notes record

            //Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2); // NEW Session

            //if (IsNewATM == true) Na.IsNewAtm = true;

            Na.Cassettes_1.InNotes = Cassette1;
            Na.Cassettes_2.InNotes = Cassette2;
            Na.Cassettes_3.InNotes = Cassette3;
            Na.Cassettes_4.InNotes = Cassette4;

            Na.ReplAmountTotal = InCashLoaded;
            Na.InReplAmount = InCashLoaded;
            //= Na.Cassettes_1.InNotes * Na.Cassettes_1.FaceValue
            // + Na.Cassettes_2.InNotes * Na.Cassettes_2.FaceValue
            // + Na.Cassettes_3.InNotes * Na.Cassettes_3.FaceValue
            // + Na.Cassettes_4.InNotes * Na.Cassettes_4.FaceValue;

            Na.UpdateSessionsNotesAndValues_CASH_In(InAtmNo, WSesNo); // NEW SESSION 

            //Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);
            // UPDATE Am
            //Am.ReadAtmsMainSpecific(InAtmNo);

            //Am.CurrCassettes = Na.Balances1.MachineBal;
            //Am.CurrentDeposits = 0;

            //Am.UpdateAtmsMain(InAtmNo);
            //
            // INSERT TRANSACTION WITH CASH ADDED
            // 
            // Build amount

            //Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

            TraceNoCashIn = OldTraceNo;

            if (Na.BalSets >= 1)
            {
                // CALL METHOD TO ADD TRANSACTION FOR CASH ADDED FOR THE NEW SESSION

                //InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                //    Na.Balances1.CurrNm, Na.Balances1.OpenBal, StartTrxn, EndTrxn);

                //if (WCitId != "1000") // IF ATM is replenished by CIT provider then Do create a trans in CIT Statement 
                //{
                //    Ct.InsertTranForCashInCit(WSignedId, WSignRecordNo, WOperator, InAtmNo, WSesNo);
                //}
            }

            if (Na.BalSets >= 2)
            {
                TraceNoCashIn = TraceNo + 1; // Last digit becomes 7 
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                     Na.Balances2.CurrNm, Na.Balances2.OpenBal, StartTrxn, EndTrxn);
            }
            if (Na.BalSets >= 3)
            {
                TraceNoCashIn = TraceNo + 2;
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                     Na.Balances3.CurrNm, Na.Balances3.OpenBal, StartTrxn, EndTrxn);
            }

            if (Na.BalSets == 4)
            {
                TraceNoCashIn = TraceNo + 3;
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                      Na.Balances4.CurrNm, Na.Balances4.OpenBal, StartTrxn, EndTrxn);
            }

        }

        private void Method_SupervisorMode_AdditionalCash_ADDED_To_Open_SessionNo(string InAtmNo)
        {

            // Update Session Notes record

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12); // NEW Session

            //if (IsNewATM == true) Na.IsNewAtm = true;

            Na.Cassettes_1.InNotes = Na.Cassettes_1.InNotes + Cassette1;
            Na.Cassettes_2.InNotes = Na.Cassettes_2.InNotes + Cassette2;
            Na.Cassettes_3.InNotes = Na.Cassettes_3.InNotes + Cassette3;
            Na.Cassettes_4.InNotes = Na.Cassettes_4.InNotes + Cassette4;

            Na.ReplAmountTotal = Na.InReplAmount
            = Na.Cassettes_1.InNotes * Na.Cassettes_1.FaceValue
            + Na.Cassettes_2.InNotes * Na.Cassettes_2.FaceValue
            + Na.Cassettes_3.InNotes * Na.Cassettes_3.FaceValue
            + Na.Cassettes_4.InNotes * Na.Cassettes_4.FaceValue;

            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo); // Current Session

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12);
            // UPDATE Am
            Am.ReadAtmsMainSpecific(InAtmNo);

            Am.CurrCassettes = Na.Balances1.MachineBal;
            Am.CurrentDeposits = 0;

            Am.UpdateAtmsMain(InAtmNo);
            //
            // INSERT TRANSACTION WITH CASH ADDED
            // 
            // Build amount

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 12);

            TraceNoCashIn = OldTraceNo;

            if (Na.BalSets >= 1)
            {
                // CALL METHOD TO ADD TRANSACTION FOR CASH ADDED FOR THE NEW SESSION

                //InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                //    Na.Balances1.CurrNm, Na.Balances1.OpenBal, StartTrxn, EndTrxn);

                //if (WCitId != "1000") // IF ATM is replenished by CIT provider then Do create a trans in CIT Statement 
                //{
                //    Ct.InsertTranForCashInCit(WSignedId, WSignRecordNo, WOperator, InAtmNo, WSesNo);
                //}
            }

            if (Na.BalSets >= 2)
            {
                TraceNoCashIn = TraceNo + 1; // Last digit becomes 7 
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                     Na.Balances2.CurrNm, Na.Balances2.OpenBal, StartTrxn, EndTrxn);
            }
            if (Na.BalSets >= 3)
            {
                TraceNoCashIn = TraceNo + 2;
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                     Na.Balances3.CurrNm, Na.Balances3.OpenBal, StartTrxn, EndTrxn);
            }

            if (Na.BalSets == 4)
            {
                TraceNoCashIn = TraceNo + 3;
                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                      Na.Balances4.CurrNm, Na.Balances4.OpenBal, StartTrxn, EndTrxn);
            }

        }
        //*********************************************************************
        // Supervisor Mode CASH ADDED -- END 
        // HOUSE-KEEPING FOR EVERYTHING FOR ATM 
        //*********************************************************************

        private void Method_SupervisorMode_ATM_FINISH_SM(string InAtmNo)
        {
            // THIS THE FINISH OF SUPERVISOR MODE 

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            //*************************************************************************************************
            // ADDITIONAL CASH ADDED
            //*************************************************************************************************

            if (CashLoaded > 0 & OpenBalance > 0 & IsNewATM == false)
            {
                // Pambos will create a new entry to show this

                // We add the new amount in the Cassettes of this ATM 
            }

            //*****************************************************************************
            // USUAL DAILY CAse FOR AN ATM - NewATM == false OR NEW ATM == True 
            //*****************************************************************************

            if (
                     (OpenBalance > 0 & CashLoaded > 0 & IsNewATM == true) // OLD ATM bu New In RRDM ... Psaudo Session Created 
                  || (OpenBalance > 0 & CashLoaded > 0 & IsNewATM == false) // Normal Case 
                  || (OpenBalance == 0 & CashLoaded > 0 & IsNewATM == true) // Brand New ATM - NO Psaudo Session Was created  
                  || (OpenBalance == 0 & CashLoaded > 0 & IsNewATM == false) // to cover error in journal 
                  || (CashLoaded == 0 & IsNewATM == false) // Old ATM Money Unloaded and no New Loaded (ATM was set out of service)
                  || (CashLoaded == 0 & IsNewATM == true) // New but no Cash Loaded     //(CashLoaded > 0 & OpenBalance > 0 & IsNewATM == false)
                                                          // || (CashLoaded > 0 & OpenBalance > 0 & IsNewATM == true) // New ATM
            )
            {
                // FINALISE THE CYCLE 
                //  SessionEnd = true; 

                // AT this point Deposits must update Notes and values
                //RRDMDepositsClass Da = new RRDMDepositsClass();

                //bool DepositsDone = false;

                //Da.ReadDepositsTotals_NEW(WAtmNo, WSesNo); // Find Totals 

                //decimal TotalDeposits = Da.DepositsMachine1.Amount + Da.ChequesMachine1.Amount + Da.DepositsMachine1.Envelops;
                //if (TotalDeposits > 0)
                //{
                //    DepositsDone = true;
                //    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo); // Update Totals 
                //}

                // Close current Repl Cycle and continue process 
                // Turn status to 0 = ready for Repl Cycle Workflow 

                Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);

                if (IsNewATM == true)
                {
                    // During new ATM we open two Sessions
                    // 
                    Ta.ProcessMode = 0; // Ready to be replenished either from Branch or through the Excel
                    // Also set ... ReplCycleNo ... 
                    // This will change when replenishment is made by user or CIT 
                    // End of Form51 to understand 

                    // Update Na with
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 12);
                    Na.ReplAmountTotal = OpenBalance;
                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                    // Update AM 
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.ReplCycleNo = WSesNo;
                    Am.ProcessMode = -1;
                    Am.LastReplDt = TRanDate;

                    Am.UpdateAtmsMain(WAtmNo);

                }
                else
                {
                    // Old ATM 
                    // Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                }

                Ta.LastTraceNo = TraceNo - 9 + 6;

                // Stats of Session

                Ta.Stats1.NoOfTranCash = Ta.Stats1.NoOfTranCash + TotalTxns;

                //Ta.Stats1.NoOfTranDepCash = Ta.Stats1.NoOfTranDepCash + Da.DepositsMachine1.Trans;

                //Ta.Stats1.NoOfTranDepCheq = Ta.Stats1.NoOfTranDepCheq + Da.ChequesMachine1.Trans;

                // END OF REPLENISHMENT 

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                ////
                //// UPDATE Action TABLE WITH ACTUAL  MONEY Loaded
                //// Old SessionNo 
                //// 
                //Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                //if (Ro.RecordFound)
                //{
                //    // ,[PassReplCycle]
                //    //,[PassReplCycleDate]
                //    //,[CashInAmount]
                //    //,[InMoneyReal]
                //    Ro.PassReplCycle = true;
                //    Ro.PassReplCycleDate = Ta.Repl1.ReplFinDtTm;
                //    Ro.InMoneyReal = CashLoaded; // from ATMs Registers

                //    Ro.ActiveRecord = false;

                //    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);
                //}
                //**************************************************************************************************
                //Insert Basic Info For G4S RECORD - BANK RECORD - SESSION UNDER COMPLETION 
                //************************************************************************************************** 
                //if (Ac.CitId != "1000")
                //{
                //    // This ATM Is COVERED BY GROUP 4
                //    int G4Record_ATM;
                //    int G4Record_CDM = 0;
                //    bool IsDeposit = false;
                //    IsDeposit = false;

                //    G4Record_ATM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record

                //    if (DepositsDone == true)
                //    {
                //        IsDeposit = true;
                //        G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record
                //    }

                //    //**************************************************************************************************
                //    //UPDATE THE G4S RECORD - SESSION UNDER COMPLETION 
                //    //************************************************************************************************** 

                //    int TempMode = 2; // Bank FILE
                //    G4.ReadCIT_G4S_Repl_EntriesSeqNo(G4Record_ATM, TempMode); // Note that this entry has SeqNo = SesNo

                //    if (G4.RecordFound == true)
                //    {
                //        if (Ro.RecordFound == true)
                //        {
                //            G4.OrderNo = Ro.ReplOrderNo;
                //            G4.OrderToBeLoaded = Ro.SystemAmount;
                //        }
                //        else
                //        {
                //            G4.OrderNo = 0;
                //        }

                //        G4.OpeningBalance = OpenBalance;

                //        G4.Dispensed = Dispensed;

                //        G4.UnloadedMachine = CashInCassettes + Rejected;

                //        G4.UnloadedCounted = 0; // This is Zero

                //        G4.Cash_Loaded = CashLoaded;

                //        //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                //        //G4.PresentedErrors = Na.Balances1.PresenterValue;

                //        G4.OtherJournalErrors = 0;

                //        G4.RemarksRRDM = "";

                //        G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_ATM, TempMode);

                //        //**************************************************************************************************
                //        //Insert Posted Transactions For CIT  for CASH Un=loaded and Loaded
                //        // DURING EXCEL Loading we do the same. 
                //        // THINK ABOUT THIS 
                //        //************************************************************************************************** 
                //        Ac.ReadAtm(WAtmNo);
                //        IsDeposit = false;

                //        if (Ac.CitId != "1000")
                //        {
                //            Insert_Posted_Txns_For_CIT(Ac.CitId, WAtmNo,
                //                 WSesNo, G4.UnloadedMachine, G4.Cash_Loaded,
                //                TRanDate, WLoadedAtRMCycle,
                //                              IsDeposit);  // Post the Txns
                //        }

                //        if (DepositsDone == true)
                //        {
                //            TempMode = 2; // Bank FILE
                //            G4.ReadCIT_G4S_Repl_EntriesSeqNo(G4Record_CDM, TempMode); // Note that this entry has SeqNo = SesNo

                //            if (G4.RecordFound == true)
                //            {
                //                if (Ro.RecordFound == true)
                //                {
                //                    //G4.OrderNo = Ra.ReplActNo;
                //                    //G4.OrderToBeLoaded = Ra.SystemAmount;
                //                }
                //                else
                //                {
                //                    G4.OrderNo = 0;
                //                }

                //                G4.OpeningBalance = 0;

                //                G4.Dispensed = 0;

                //                G4.UnloadedMachine = 0;

                //                G4.UnloadedCounted = 0; // This is Zero

                //                G4.Cash_Loaded = 0;

                //                G4.Deposits = TotalDeposits;

                //                //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                //                //G4.PresentedErrors = Na.Balances1.PresenterValue;

                //                G4.OtherJournalErrors = 0;

                //                G4.RemarksRRDM = "";

                //                G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_CDM, TempMode);

                //                //**************************************************************************************************
                //                //Insert Posted Transactions For CIT  for Deposit Machines 
                //                // The CIT Account is updated as well as the Bank's account 
                //                // DURING EXCEL Loading we do the same. 
                //                // THINK ABOUT THIS 
                //                //************************************************************************************************** 
                //                Ac.ReadAtm(WAtmNo);
                //                IsDeposit = true;

                //                if (Ac.CitId != "1000")
                //                {
                //                    Insert_Posted_Txns_For_CIT(Ac.CitId, WAtmNo,
                //                         WSesNo, G4.UnloadedMachine, G4.Cash_Loaded,
                //                        TRanDate, WLoadedAtRMCycle,
                //                                      IsDeposit);  // Post the Txns
                //                }
                //            }
                //        }


                //    }



                //} // HERE FINISH the CIT

                //****************************************************************************
                // PREVIOUS SESSION IS NOW COMPLETED 
                // AND NOW 
                // Create New Session 
                //****************************************************************************

                WSaveSesNo = WSesNo;

                //CashLoaded = Cassette1 * Ac.FaceValue_11
                //       + Cassette2 * Ac.FaceValue_12
                //       + Cassette3 * Ac.FaceValue_13
                //       + Cassette4 * Ac.FaceValue_14;

                if (CashLoaded >= 0 & WMode == 3 & Correct_Process5 == false & FirstButCameLate == false
                             & Correct_Process6 == false & Correct_Process0 == false & IsCameLate == false)
                {
                    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 

                    //CurrentSessionNo = Sa.NewSessionNo;
                    WSesNo = Sa.NewSessionNo;

                    // Update Session Notes Record

                    Method_SupervisorMode_CASHADDED_To_New_SessionNo(WAtmNo, CashLoaded);

                    Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                    Ta.SesDtTimeEnd = Ta.SesDtTimeStart;

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                }

                //Message = "END Replenishment For ATM : " + WAtmNo + " Done on : " + TRanDate.ToString();

                if (SM.IsValid == false)
                {
                    // There errors in SM numbers 
                    Ta.ReadSessionsStatusTraces(WAtmNo, WSaveSesNo);

                    Ta.ReplGenComment = "Toxic Supervisor Mode totals Or Dates. Correct them. ";
                    //SM_LAST_CLEARED == NullPastDate

                    Ta.ProcessMode = -6; // Temporary out of action. 

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSaveSesNo);

                }
            }


            //*****************************************************************************
            // OTHER CASES NOT TO BE CONSIDERED 
            //*****************************************************************************

            if (Dispensed == 0 & Rejected == 0 & CashInCassettes == 0 & CashLoaded == 0)
            {
                // this is the case where the SM has a START and END ONLY 
                // => Not to be considered supervisor mode. 
                // Note that opening balance might be available because is calculated differently   
                return;
            }


            if (CashLoaded == 0 & OpenBalance > 0)
            {
                // AND Cassettes extracted then normal case without Loading Money

                // ATM maygo to other location 

                // => No new cycle

                // But record in G4S
            }

            if (CashLoaded == 0 & OpenBalance > 0)
            {
                // ATM Was oppened totals were printed but not cassettes replased

                // Pambos to ignore this case => Print Totals without replenishment 

                // => No new replenishment cyccle 
                // => No Action  
            }

            /// ********************************************************************
            /// UPDATE REPLENISHMENT STATS 
            /// ********************************************************************

            Rs.InsertInAtmsStatsProcess(WOperator, InAtmNo, WSesNo, CashLoaded);
            // Within method we find the information of previous Repl Cycle through the previous 
            // WSesNo and we update the statistics based on the information of Na and Ta

        }
        //*********************************************************************
        // Supervisor Mode END 
        // HOUSE KEEPING FOR EVERYTHING FOR CDM ONLY
        //*********************************************************************

        private void Method_SupervisorMode_END_CDM(string InAtmNo)
        {
            // THIS THE FINISH OF SUPERVISOR MODE 

            //*****************************************************************************
            // CHECK DEPOSITS 
            // READ TOTALS THE SAME WAY AS PER ATM
            // BAMBOS SHOULD READ 
            // Create entry as you for ATMs But for deposits only 
            // New Session is created as needed
            //*****************************************************************************

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();


            //*****************************************************************************
            // USUAL DAILY CAse FOR AN CDM
            //*****************************************************************************

            if (WSesNo > 0)
            {
                // FINALISE THE CYCLE 
                //  SessionEnd = true; 

                // AT this point Deposits must update Notes and values
                RRDMDepositsClass Da = new RRDMDepositsClass();

                bool DepositsDone = false;

                Da.ReadDepositsTotals_NEW(WAtmNo, WSesNo); // Find Totals 

                decimal TotalDeposits = Da.DepositsMachine1.Amount + Da.ChequesMachine1.Amount + Da.DepositsMachine1.Envelops;

                if (TotalDeposits > 0)
                {
                    DepositsDone = true;
                    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo); // Update Totals 
                }

                // Close current Repl Cycle and continue process 
                // Turn status to 0 = ready for Repl Cycle Workflow 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                // UPDATE BEFORE NEW 
                // Update closing date ... END DATE 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                Ta.SesDtTimeEnd = TRanDate;
                // Assign  with last digit = 6 // Currently we are at 9 
                Ta.LastTraceNo = TraceNo - 9 + 6;
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                //
                // UPDATE Action TABLE WITH ACTUAL  MONEY Loaded
                // Old SessionNo 
                // 
                Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                if (Ro.RecordFound)
                {
                    Ro.InMoneyReal = CashLoaded;

                    Ro.ActiveRecord = false;

                    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);
                }

                //**************************************************************************************************
                //Insert Basic Info For G4S RECORD - PREVIOUS SESSION 
                //************************************************************************************************** 
                //int G4Record_ATM;
                int G4Record_CDM = 0;
                bool IsDeposit = false;
                //G4Record_ATM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadingCycleNo, IsDeposit);  // Create G4S Record
                if (DepositsDone == true)
                {
                    IsDeposit = true;
                    G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record
                }

                //**************************************************************************************************
                //UPDATE THE G4S RECORD - PREVIOUS SESSION 
                //************************************************************************************************** 

                if (DepositsDone == true)
                {
                    int TempMode = 2; // Bank FILE
                    G4.ReadCIT_G4S_Repl_EntriesBySeqNo(G4Record_CDM, TempMode); // Note that this entry has SeqNo = SesNo

                    if (G4.RecordFound == true)
                    {
                        if (Ro.RecordFound == true)
                        {
                            //G4.OrderNo = Ra.ReplActNo;
                            //G4.OrderToBeLoaded = Ra.SystemAmount;
                        }
                        else
                        {
                            G4.OrderNo = 0;
                        }

                        G4.OpeningBalance = 0;

                        G4.Dispensed = 0;

                        G4.UnloadedMachine = 0;

                        G4.UnloadedCounted = 0; // This is Zero

                        G4.Cash_Loaded = 0;

                        G4.Deposits = TotalDeposits;

                        //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                        //G4.PresentedErrors = Na.Balances1.PresenterValue;

                        G4.OtherJournalErrors = 0;

                        G4.RemarksRRDM = "";

                        G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_CDM, TempMode);
                    }
                }

                //****************************************************************************
                // Create New Session 
                //****************************************************************************

                WSaveSesNo = WSesNo;

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 

                //CurrentSessionNo = Sa.NewSessionNo;

                WSesNo = Sa.NewSessionNo;
                ////*********************************************
                //// Call cash Added for New Session
                ////******************************************** 
                if (SM.IsValid == false)
                {
                    // There errors in SM numbers 
                    Ta.ReplGenComment = "Toxic Supervisor Mode totals Or Dates.";

                    Ta.ProcessMode = -6; // Temporary out of action. 

                    Ta.UpdateSessionsStatusTraces(WAtmNo, WSaveSesNo);

                    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 

                }
            }

            //*************************************************************************************************
            // NEW CDM CASE 
            //*************************************************************************************************

            if (WSesNo == 0)
            {
                IsNewATM = true;
                // MAKE ATM ACTIVE 
                Ac.ReadAtm(WAtmNo);
                Ac.ActiveAtm = true;
                Ac.UpdateAtmsBasic(WAtmNo);


                // AT this point Deposits must update Notes and values
                RRDMDepositsClass Da = new RRDMDepositsClass();

                bool DepositsDone = false;

                Da.ReadDepositsTotals_NEW(WAtmNo, WSesNo); // Find Totals 

                decimal TotalDeposits = Da.DepositsMachine1.Amount + Da.ChequesMachine1.Amount + Da.DepositsMachine1.Envelops;

                if (TotalDeposits > 0)
                {
                    DepositsDone = true;
                    Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo); // Update Totals 
                }

                // Create a pseaudo cycle  

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 

                // => NEW cycle
                WSesNo = Sa.NewSessionNo;


                // Turn status to 0 = ready for Repl Cycle Workflow 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
                // UPDATE BEFORE NEW 
                // Update closing date ... END DATE 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                Ta.SesDtTimeEnd = TRanDate;
                // Assign  with last digit = 6 // Currently we are at 9 
                Ta.LastTraceNo = TraceNo - 9 + 6;
                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                // UPDATE Action TABLE WITH ACTUAL  MONEY Loaded
                // Old SessionNo 
                // 
                Ro.Read_Latest_ReplActionsForAtm(WAtmNo);

                if (Ro.RecordFound)
                {
                    Ro.InMoneyReal = CashLoaded;

                    Ro.ActiveRecord = false;

                    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);
                }

                //*************************************************************
                // Create G4S
                //*************************************************************

                int G4Record_CDM = 0;
                bool IsDeposit = true;
                G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadedAtRMCycle, IsDeposit);  // Create G4S Record

                //**************************************************************************************************
                //UPDATE THE G4S RECORD - PREVIOUS SESSION 
                //************************************************************************************************** 

                //RRDM_CIT_G4S_Repl_Entries G4 = new RRDM_CIT_G4S_Repl_Entries();

                int TempMode = 2; // Bank FILE

                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(G4Record_CDM, TempMode);// Note that WSesNo = SeqNo

                if (G4.RecordFound == true)
                {
                    if (Ro.RecordFound == true)
                    {
                        //G4.OrderNo = Ra.ReplActNo;
                        //G4.OrderToBeLoaded = Ra.SystemAmount;
                    }
                    else
                    {
                        G4.OrderNo = 0;
                    }

                    G4.OpeningBalance = 0;

                    G4.Dispensed = 0;

                    G4.UnloadedMachine = 0;

                    G4.UnloadedCounted = 0; // This is Zero

                    G4.Cash_Loaded = 0;

                    G4.Deposits = TotalDeposits;

                    //Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                    //G4.PresentedErrors = Na.Balances1.PresenterValue;

                    G4.OtherJournalErrors = 0;

                    G4.RemarksRRDM = "";

                    G4.UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(G4Record_CDM, TempMode);
                }
                //*****************************************************************************
                // NEW SESSION
                //*****************************************************************************
                WSaveSesNo = WSesNo;

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadedAtRMCycle); // The necessary records are created and the new Session No is available 

                //CurrentSessionNo = Sa.NewSessionNo;

                WSesNo = Sa.NewSessionNo;
                //// Call cash Added for New Session 
                //Method_SupervisorMode_CASHADDED(WAtmNo);
                ////

                //ReplCyclefound = true;

            }

        }
        //
        // INSERT G4S Record 
        // 
        private int Insert_G4S_Record(string InAtmNo, int InSessionNo,
            DateTime InDate, int InCycleNo, bool InIsDeposit)
        {
            //**************************************************************************************************
            // CREATE THE G4S (BANK) RECORD 
            //************************************************************************************************** 

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

            Ac.ReadAtm(WAtmNo);

            G4.CITId = Ac.CitId;
            G4.OriginFileName = "OrininFlNm";
            G4.AtmNo = WAtmNo;
            G4.AtmName = Ac.AtmName;
            G4.LoadingExcelCycleNo = 0; // Excel Not loaded yet
            G4.ReplDateG4S = InDate; // Taken from record read
            G4.OrderNo = 0;
            G4.CreatedDate = DateTime.Now;

            if (InIsDeposit == true)
            {
                G4.IsDeposit = true; // It is a deposit Record for deposits 
            }
            else
            {
                G4.IsDeposit = false; // It is not a deposit record  
            }

            G4.OpeningBalance = 0;

            G4.Dispensed = 0;

            G4.UnloadedMachine = 0;

            G4.UnloadedCounted = 0; // This is Zero

            G4.Cash_Loaded = 0;

            G4.Deposits = 0;

            G4.OverFound = 0;
            G4.ShortFound = 0;

            Ec.ReadAllErrorsTableForCounters(WOperator, "", InAtmNo, InSessionNo, "");
            if (Ec.RecordFound == false)
            {
                // try if Session no was zero
                //Ec.ReadAllErrorsTableForCounters(WOperator, "", InAtmNo, 0, "");
            }

            G4.PresentedErrors = Ec.TotalErrorsAmtLess100; // Presented errors 

            G4.ReplCycleNo = InSessionNo;
            G4.RemarksG4S = "";
            G4.Operator = Ac.Operator;
            Ta.ReadSessionsStatusTraces(WAtmNo, InSessionNo);
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            // Rjc.Find_GL_Cut_Off_Before_GivenDate(Ta.Operator, Ta.SesDtTimeEnd.Date);
            Rjc.ReadReconcJobCyclesById(Ta.Operator, InCycleNo);
            if (Rjc.RecordFound == true)
            {
                // Cut off of prvious to Replenishment 
                G4.Cut_Off_date = Rjc.Cut_Off_Date;
                G4.LoadedAtRMCycle = InCycleNo;
                //WReconcCycleNo = Rjc.JobCycle;
                //IsDataFound = true;
            }
            else
            {
                G4.Cut_Off_date = NullPastDate;
                //IsDataFound = false;
            }

            G4.Gl_Balance_At_CutOff = 0;

            Na.ReadSessionsNotesAndValues(WAtmNo, InSessionNo, 12);

            G4.Load_FaceValue_1 = decimal.ToInt32(Na.Cassettes_1.FaceValue);
            G4.Load_Cassette_1 = Na.Cassettes_1.InNotes;
            G4.Load_FaceValue_2 = decimal.ToInt32(Na.Cassettes_2.FaceValue);
            G4.Load_Cassette_2 = Na.Cassettes_2.InNotes;
            G4.Load_FaceValue_3 = decimal.ToInt32(Na.Cassettes_3.FaceValue);
            G4.Load_Cassette_3 = Na.Cassettes_3.InNotes;
            G4.Load_FaceValue_4 = decimal.ToInt32(Na.Cassettes_4.FaceValue);
            G4.Load_Cassette_4 = Na.Cassettes_4.InNotes;

            G4.Un_Load_FaceValue_1 = decimal.ToInt32(Na.Cassettes_1.FaceValue);
            G4.Un_Load_Cassette_1 = Na.Cassettes_1.RemNotes + Na.Cassettes_1.RejNotes;
            G4.Un_Load_FaceValue_2 = decimal.ToInt32(Na.Cassettes_2.FaceValue);
            G4.Un_Load_Cassette_2 = Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes;
            G4.Un_Load_FaceValue_3 = decimal.ToInt32(Na.Cassettes_3.FaceValue);
            G4.Un_Load_Cassette_3 = Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes;
            G4.Un_Load_FaceValue_4 = decimal.ToInt32(Na.Cassettes_4.FaceValue);
            G4.Un_Load_Cassette_4 = Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes;

            // READ DEPOSITS 

            G4.Deposits = 0;

            G4.Deposits_Notes_Denom_1 = 0;
            G4.Deposits_Notes_Denom_2 = 0;
            G4.Deposits_Notes_Denom_3 = 0;
            G4.Deposits_Notes_Denom_4 = 0;

            G4.ExcelDate = NullPastDate;

            int Mode = 2;

            int SeqNo = G4.InsertCIT_G4S_Repl_EntriesRecord(Mode);

            return SeqNo;
        }
        //
        // INSERT Posted Txns for CIT 
        // 
        private void Insert_Posted_Txns_For_CIT(string InCitId, string InAtmNo,
                          int InSessionNo, decimal InUnLoaded, decimal InLoaded,
                          DateTime InDate, int InCycleNo, bool InIsDeposit)
        {
            Na.ReadSessionsNotesAndValues(InAtmNo, InSessionNo, 12);

            string BankCashAcc;
            string CitCashAcc;


            //return;

            // There are new ways of accessing accno and acccurrency 

            // string WUser = "1000";
            // Acc.ReadAndFindAccount("1000", WOperator, "", Ac.DepCurNm, "User or CIT Cash");
            //// Acc.ReadAndFindAccountSpecificForNostroVostro(BankAccNo, string InOperator);
            // if (Acc.RecordFound == true)
            // {
            //     BankCashAcc = Acc.AccNo;
            // }
            // else
            // {
            //     BankCashAcc = "Not Defined";
            // }

            BankCashAcc = "Bank Vaults for CIT " + InCitId;

            string WUser = InCitId;


            Acc.ReadAndFindAccount(WUser, "", "", WOperator, "", Ac.DepCurNm, "User or CIT Cash");
            if (Acc.RecordFound == true)
            {
                CitCashAcc = Acc.AccNo;

                CurrDesc = Acc.CurrNm;

            }
            else
            {
                CitCashAcc = "Not Defined";
            }


            RRDMPostedTrans Pt = new RRDMPostedTrans();

            // DO A Pair for UnLoaded
            if (InUnLoaded > 0)
            {
                // First Entry 
                // CR CIT CASH
                //
                Pt.TranToBePostedKey = 9999;
                Pt.Origin = "UnLoaded By Cit";
                Pt.UserId = WSignedId;
                Pt.AccNo = WUser;
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 11; // DR CIT Cash 
                Pt.TransDesc = "CIT has Unloaded Money from ATM";
                //TEST
                Pt.CurrDesc = Ac.DepCurNm;
                Pt.TranAmount = InUnLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

                Pt.RMCycle = InCycleNo;

                Pt.InsertTran(Pt.TranToBePostedKey, Pt.Origin);

                // Posted Second transaction 
                // SECOND Entry 
                // DR BANK's CASH
                //
                Pt.TranToBePostedKey = 9998;
                Pt.Origin = "UnLoaded By Cit";
                Pt.UserId = WSignedId;
                Pt.AccNo = BankCashAcc;
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 21; //Credit Bank Cash 
                Pt.TransDesc = "CIT has Unloaded Money from ATM";
                //TEST
                Pt.CurrDesc = Ac.DepCurNm;
                Pt.TranAmount = InUnLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

                Pt.RMCycle = InCycleNo;

                Pt.InsertTran(Pt.TranToBePostedKey, Pt.Origin);
            }
            // FOR LOADED
            if (InLoaded > 0)
            {
                // First Entry 
                // CR CIT CASH
                //
                Pt.TranToBePostedKey = 9999;
                Pt.Origin = "Loaded By Cit";
                Pt.UserId = WSignedId;
                Pt.AccNo = WUser; // We set as the number of the CIT
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 21; // CR CIT Cash 
                Pt.TransDesc = "CIT has loaded Money to ATM";
                //TEST
                Pt.CurrDesc = Ac.DepCurNm;
                Pt.TranAmount = InLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

                Pt.RMCycle = InCycleNo;

                Pt.InsertTran(Pt.TranToBePostedKey, Pt.Origin);

                // Posted Second transaction 
                // SECOND Entry 
                // DR BANK's CASH
                //
                Pt.TranToBePostedKey = 9998;
                Pt.Origin = "Loaded By Cit";
                Pt.UserId = WSignedId;
                Pt.AccNo = BankCashAcc;
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 11; // Debit Bank Cash 
                Pt.TransDesc = "CIT has loaded Money to ATM";
                //TEST
                Pt.CurrDesc = Ac.DepCurNm;
                Pt.TranAmount = InLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

                Pt.RMCycle = InCycleNo;

                Pt.InsertTran(Pt.TranToBePostedKey, Pt.Origin);
            }



        }
        ////
        //// UPDATE Processed = true in Hst Table 
        //// 
        //public void UpdatetblHstAtmTxnsProcessedToTrue(int InSeqNo)
        //{

        //    Major_ErrorFound = false;
        //    ErrorOutput = "";

        //    //int rows;

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
        //                      + " SET Processed = 1 "
        //                    + " WHERE SeqNo <= @SeqNo", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

        //                cmd.ExecuteNonQuery();

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //}


        //
        // Insert Build and Insert Transaction during Creation of Session 
        //
        private void InsertTranInPool(string InAtmNo, int WSesNo, int InTraceNo, int InMasterTraceNo,
                                      string InCurrNm, decimal InTranAmount, int Start, int End)
        {
            //
            // BUILD THE TRANSACTION 
            //

            Tc.CurrDesc = InCurrNm;
            Tc.TranAmount = InTranAmount;

            Tc.AtmTraceNo = InTraceNo;
            Tc.EJournalTraceNo = InTraceNo;
            Tc.MasterTraceNo = InMasterTraceNo;

            Tc.UniqueRecordId = Gu.GetNextValue();

            Tc.OriginName = "OurATMs-Repl : " + InAtmNo;

            Tc.AtmNo = InAtmNo;
            Tc.SesNo = WSesNo;
            Tc.BankId = WOperator;

            Tc.BranchId = Ac.Branch;

            Tc.AtmDtTime = DateTime.Now; // THIS IS A TEMPORARY SOLUTION 
                                         //Tc.HostDtTime = DateTime.Now;

            Tc.SystemTarget = 9; // System Transaction 
            Tc.RMCateg = "EWB110";
            Tc.TransType = 22; // Deposit By ATM 
            Tc.TransDesc = "Cash Added To ATM";
            Tc.CardNo = "";
            Tc.CardOrigin = 1;

            Acc.ReadAndFindAccount("1000", "", "", WOperator, InAtmNo, InCurrNm, "ATM Cash");
            if (Acc.RecordFound == false)
            {
                Tc.AccNo = "NoATMCashAcc";

                Message = "Account not found for ATMNo: " + InAtmNo;
                Perform.InsertPerformanceTrace(WOperator, WOperator, 2, "Load Atms Journal Txns", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);
                if (Environment.UserInteractive)
                {
                    if (ShowMessage == true)
                    {
                        //System.Windows.Forms.MessageBox.Show(Message);
                    }
                }
            }
            else
            {
                Tc.AccNo = Acc.AccNo;  // Cash account No
            }

            Tc.AuthCode = 0;
            Tc.RefNumb = 0;
            Tc.RemNo = 0;

            Tc.TransMsg = "";
            Tc.AtmMsg = "";
            Tc.ErrNo = 0;
            Tc.StartTrxn = Start;
            Tc.EndTrxn = End;
            Tc.SuccTran = true;

            // ADD THE TRANSACTION 
            Tc.InsertTransInPool(WOperator, InAtmNo);
        }



    }
}

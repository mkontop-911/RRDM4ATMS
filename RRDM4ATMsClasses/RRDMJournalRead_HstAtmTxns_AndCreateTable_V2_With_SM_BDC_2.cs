using System;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2 : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2() : base() { }

        public bool RecordFound;
        public bool Major_ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        RRDMDepositsClass Da = new RRDMDepositsClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMSessionsNewSession Sa = new RRDMSessionsNewSession(); // NEW SESSION CLASS 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMCashInOut Ct = new RRDMCashInOut();

        RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs();

        RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReplOrdersClass Ro = new RRDMReplOrdersClass();

        RRDMPerformanceTraceClass Perform = new RRDMPerformanceTraceClass();

        RRDMReplStatsClass Rs = new RRDMReplStatsClass();

        RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();

        RRDMGroups Gr = new RRDMGroups();

        public DataTable HstAtmTxnsDataTable = new DataTable();

        public DataTable TempTableMpa = new DataTable();

        public DataTable TableAtms = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        // ********INSERT BY PANICOS**************
        DateTime BeforeCallDtTime;
        // *************************BY PANICOS********************************

        string WReconCategoryId;

        string WMatchingCategoryId = "";

        int WSeqNo;

        int MaxSeqNo;

        int UniqueRecordId;

        string Message;

        bool ShowMessage;

        DateTime TRanDate;
        DateTime PreTranDate;

       // DateTime OLD_TRanDate; // NEEDED to Cover transactions within the same minute

        string trandesc;

        string txtline; 

        //int WSaveSesNo;

        //int Cassette1;
        //int Cassette2;
        //int Cassette3;
        //int Cassette4;

        //int CurrentSessionNo;

        int TraceNo;

        bool OwnCard;
        bool CreateReversal;

        string SqlString1_1 = "";
        string SqlString1_2 = "";

        string SqlString = "";

        int MasterTraceNo;

        string TraceNoWithNoEndZero; // Read as String 

        int StartTrxn;
        int EndTrxn;

        int OldTraceNo;

        string Wtrandesc;

        string WCitId;

        int WSesNo;

        //int WPreviousSesNo;

        int fuid;

        string CardNo;
        string AccNo;

        int TransactionType;

        string CurrDesc;
        decimal TranAmount;

        int CommissionCode;
        decimal CommissionAmount;

        int CardTargetSystem;

        string CardCaptured;

        string PresError;

        string PRX; 

        public int TotalValidRecords = 0;
        public int TotalTxns = 0;
        public int GrandTotalTxns;

        string SuspectDesc;
        string CardCapturedMes;

        int DrTransactions;
        decimal DispensedAmt;
        int CrTransactions;
        decimal DepAmount;

        //int InternalCounter;

        int WLoadedAtRMCycle;

        //DateTime WCut_Off_Date;

        string Result;

        string ResponseCode;

        string WSignedId;
        int WSignRecordNo;
        //string WBankId;

        string WOperator;
        int WAtmsReconcGroup;
        string WAtmNo;
        int WFuid;

        int WMode;

        public void ReadJournal_Txns_And_Insert_In_Pool(string InSignedId, int InSignRecordNo, string InOperator,
                                                             int InAtmsReconcGroup, string InAtmNo, int InFuid, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            WAtmsReconcGroup = InAtmsReconcGroup;

            WAtmNo = InAtmNo;

            WMode = InMode; // 1 Group of ATMs
                            // 2 Single ATM with zero Fuid
                            // 3 Single ATM with Fuid for multithreading
                            // 4 ALL ATMS 

            if (WMode==3)
            {
                Major_ErrorFound = false; 
                return;
            }
            WFuid = InFuid; // If mode = 3 then this has a value ... it gets a value when called by Alecos after Bambos is called and a Fuid is given.
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

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }



            // If call by Alecos = Mode = 3 then before reading the transactions check 
            // if for this FUI there is Replenishement through Pambos Table. 
            // If there Update old Cycle and Create new. => Update Bambos with the old and the new Repl Cycle No. 
            // Keep date and old and new Cycle Number
            // Read all transactions of this Fui and Update any less date with Old Repl Cycle and any greater than date with the new 
            // ''''''''''''''''''''''''''''''
            // If for this Fui there is no replenishement 
            // Read the last replenishement and find Date of it and Repl Cycle number 
            // During transactions reading there are txns for this ATM < than last Cycle number then update them with the repl cycle number
            string ParId = "720";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string SM_Management = Gp.OccuranceNm;

                  
            // For this ATM Group
            // Read transactions from Pambos EJournal file
            // Read Per ATM , sort transactions by date time. 
            // 
            // 1) Update Mpa Trans
            // 2) Update Captured Cards
            // 3) Update cassettes
            // 4) Update First Last Trace or maybe not? Yes it is better here than in replenishment workflow 
            // 5) Create New Supervisor Mode Cycles
            // 6) Update Daily statistics
            // 7) Update CIT orders with exact Loading amount and cassettes

            if (WMode == 1) Gr.InsertGroupDuringLoading(WAtmsReconcGroup);


            // SHOW MESSAGE 
            ParId = "719";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                ShowMessage = true;
            }
            else
            {
                ShowMessage = false;
            }

            DateTime NullDate = new DateTime(1900, 01, 01);
            //

            RecordFound = false;
            Major_ErrorFound = false;
            ErrorOutput = "";

            //  PreStartTrxn = 0;

            // FIND THE ATMS 

            if (WMode == 1)
            {
                // Get All Atms
                SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup";
            }
            if (WMode == 2 & WAtmNo != "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmNo = @AtmNo ";
            }
            if (WMode == 3 & WAtmNo != "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmNo = @AtmNo ";
            }

            if (WMode == 4 & WAtmNo == "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator ";
            }

            TableAtms = new DataTable();
            TableAtms.Clear();

            using (SqlConnection conn =
                 new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        if (WMode == 1)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmsReconcGroup", WAtmsReconcGroup);
                        }

                        if (WMode == 2 || WMode == 3)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);
                        }
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableAtms);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            // loop
            try
            {
                // LOOP FOR ATMS

                DateTime StartAtmsDate = DateTime.Now;

                int I = 0;

                while (I <= (TableAtms.Rows.Count - 1))
                {

                    RecordFound = true;
                    bool IsRomania = false;
                    WAtmNo = (string)TableAtms.Rows[I]["AtmNo"];

                    // ****************INSERT BY PANICOS**************
                    // Initialise for next
                    BeforeCallDtTime = DateTime.Now;
                    // *************************END INSERT BY PANICOS********************************


                    Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                    WOperator = Ac.Operator;

                    Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);
                    WReconCategoryId = Rc.CategoryId;

                    WCitId = Ac.CitId;

                    // INITIALISE AND Define the data table 

                    TotalValidRecords = 0;
                    TotalTxns = 0;
                    MaxSeqNo = 0;

                    HstAtmTxnsDataTable = new DataTable();
                    HstAtmTxnsDataTable.Clear();

                    TempTableMpa = new DataTable();
                    TempTableMpa.Clear();

                    MpaDataTableDefinition();
                    if (WAtmNo == "EJ017002")
                    {
                        IsRomania = true; 

                        SqlString1_1 = " SELECT SeqNo, ISNULL(atmno, '') AS atmno, ISNULL(TraceNumber, 0) AS TraceNumber,  ISNULL(Trace, '') As TraceNoWithNoEndZero"
                    + ",ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc, "
                 + "ISNULL(currency, '') AS currency, ISNULL(CAmount, 0 ) AS CAmount, ISNULL(cardnum, '') AS cardnum,"
                  + "ISNULL(Sessionstart, 0) AS starttxn,ISNULL(SessionEnd, 0) AS endtxn, "
                  + " ISNULL(txtline, '') AS txtline, "
                  //+ "ISNULL(TranTime, '') AS trantime,"
                  + "ISNULL(acct1, '') AS acct1, "
                  + "ISNULL(acct2, '') AS acct2, "
                  + "ISNULL(Result, '') AS Result, ISNULL(Source, '') AS ResponseCode,"
                  + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
                  + "ISNULL(PresenterError, '') AS PresError, "
                   + " ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
                  + "ISNULL(Type1, 0) AS Type1, "
                  + "ISNULL(Type2, 0) AS Type2, "
                  + "ISNULL(Type3, 0) AS Type3, "
                  + "ISNULL(Type4, 0) AS Type4, "
                  + " Processed, "
                  + " ISNULL(CardTargetSystem, 0) AS CardTargetSystem, "
                  + " ISNULL(TransactionType, 0) AS TransactionType, "
                  + " ISNULL(CommissionCode, 0) AS CommissionCode, "
                  + " ISNULL(CommissionAmount, 0) AS CommissionAmount, "
                  + " ISNULL(Trandatetime, '') AS TRanDate "

                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] ";
                    }
                    else
                    {
                        SqlString1_1 = "SELECT SeqNo, ISNULL(atmno, '') AS atmno, ISNULL(TraceNumber, 0) AS TraceNumber,  ISNULL(Trace, '') As TraceNoWithNoEndZero"
                    + ",ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc, "
                 + "ISNULL(currency, '') AS currency, ISNULL(CAmount, 0 ) AS CAmount, ISNULL(cardnum, '') AS cardnum,"
                  + "ISNULL(Sessionstart, 0) AS starttxn,ISNULL(SessionEnd, 0) AS endtxn, "
                  + " ISNULL(txtline, '') AS txtline, "
                  //+ "ISNULL(TranTime, '') AS trantime,"
                  + "ISNULL(acct1, '') AS acct1, "
                  + "ISNULL(acct2, '') AS acct2, "
                  + "ISNULL(Result, '') AS Result, ISNULL(Source, '') AS ResponseCode,"
                  + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
                  + "ISNULL(PresenterError, '') AS PresError, "
                   + " ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
                  + "ISNULL(Type1, 0) AS Type1, "
                  + "ISNULL(Type2, 0) AS Type2, "
                  + "ISNULL(Type3, 0) AS Type3, "
                  + "ISNULL(Type4, 0) AS Type4, "
                  + " Processed, "
                  + " ISNULL(CardTargetSystem, 0) AS CardTargetSystem, "
                  + " ISNULL(TransactionType, 0) AS TransactionType, "
                  + " ISNULL(CommissionCode, 0) AS CommissionCode, "
                  + " ISNULL(CommissionAmount, 0) AS CommissionAmount, "
                  + " ISNULL(Trandatetime, '') AS TRanDate "

                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] ";
                    }
                    
                    //  + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] ";
                    //+ " WHERE Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                    //+ " ORDER by AtmNo, TranDate, TranTime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          

                    if (WMode == 1 || WMode == 2)
                    {
                        // Covering groups of ATMs
                        SqlString1_2 =
                         " WHERE AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          
                        //+ "  ORDER by Atmno, fuid, RuId "; // NO NEED To Cover Trace less

                    }

                    //if (WMode == 3)
                    //{
                    //    // Covering only one ATM with known Fuid
                    //    SqlString1_2 =
                    //     "  WHERE fuid=@fuid  AND AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                    //     + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          
                    //                                                         //+ "  ORDER by Atmno, fuid, RuId "; // NO NEED To Cover Trace less 
                    //}

                    if (WMode == 3)
                    {
                        // Covering only one ATM with known Fuid
                        SqlString1_2 =
                         "  WHERE fuid=@fuid AND TransactionType <> 99 and result = 'OK' "
                         + " ORDER BY RuID ";
                    }

                    if (WMode == 4)
                    {
                        // Covering only one ATM with known Fuid
                        SqlString1_2 =
                         "  WHERE AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK' "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber ";
                    }

                    SqlString = SqlString1_1 + SqlString1_2;

                    using (SqlConnection conn =
                       new SqlConnection(connectionString))
                        try
                        {
                            conn.Open();
                            //Create an Sql Adapter that holds the connection and the command
                            using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                            {

                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@atmno", WAtmNo);

                                sqlAdapt.SelectCommand.CommandTimeout = 300;

                                //Create a datatable that will be filled with the data retrieved from the command
                                if (WMode == 3)
                                {
                                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@fuid", WFuid);
                                }
                                
                                sqlAdapt.Fill(HstAtmTxnsDataTable);

                                // Close conn
                                conn.Close();

                                if (HstAtmTxnsDataTable.Rows.Count == 0)
                                {
                                    // ONLY FUID - Update all records as processed
                                    if (WMode == 3)
                                    {
                                        // Based on fuid
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(WAtmNo, WFuid);
                                    }

                                    I = I + 1;

                                    continue;
                                }

                                PreTranDate = NullPastDate;
                                DrTransactions = 0;
                                DispensedAmt = 0;
                                CrTransactions = 0;
                                DepAmount = 0;

                                int K = 0;

                                while (K <= (HstAtmTxnsDataTable.Rows.Count - 1))
                                {
                                    // GET Table fields - Line by Line
                                    //
                                    int SeqNo = (int)HstAtmTxnsDataTable.Rows[K]["SeqNo"];

                                    TransactionType = (int)HstAtmTxnsDataTable.Rows[K]["TransactionType"];

                                    Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];
                                    TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];

                                    CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];

                                    if ( TranAmount > 0 || CardCaptured == "CARD CAPTURED")
                                    {
                                        if(CardCaptured == "CARD CAPTURED" || SeqNo == 14212 )
                                        {
                                            CardCaptured = CardCaptured; 
                                        }
                                        // OK
                                        // If CARD Captured we do special processing 
                                        //// TEMPORARY FOR AUDI
                                        //if (CardCaptured == "CARD CAPTURED")
                                        //{
                                        //    TransactionType = 39; // means is not a valid transaction
                                        //}
                                    }
                                    else
                                    {
                                        K = K + 1;
                                        continue;
                                    }


                                    RecordFound = true;

                                    TotalValidRecords = TotalValidRecords + 1;


                                    WSeqNo = (int)HstAtmTxnsDataTable.Rows[K]["SeqNo"];


                                    TraceNo = (int)HstAtmTxnsDataTable.Rows[K]["TraceNumber"];

                                    TraceNoWithNoEndZero = (string)HstAtmTxnsDataTable.Rows[K]["TraceNoWithNoEndZero"];

                                    fuid = (int)HstAtmTxnsDataTable.Rows[K]["fuid"];
                                    trandesc = (string)HstAtmTxnsDataTable.Rows[K]["trandesc"];

                                    CardNo = (string)HstAtmTxnsDataTable.Rows[K]["cardnum"];

                                    txtline = (string)HstAtmTxnsDataTable.Rows[K]["txtline"];

                                    //Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];

                                    ResponseCode = ((string)HstAtmTxnsDataTable.Rows[K]["ResponseCode"]).Trim(); 

                                    if (ResponseCode == "000" || ResponseCode == "00" )
                                    {
                                        ResponseCode = "0";
                                    }
                                    //else
                                    //{
                                    //    ResponseCode ="0"; 
                                    //}

                                    if (TransactionType == 11 || TransactionType == 33)
                                    {
                                        AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct1"];
                                    } 
                                    else
                                    {
                                        // This is deposit
                                        AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct2"];
                                    }

                                    StartTrxn = (int)HstAtmTxnsDataTable.Rows[K]["starttxn"];
                                    EndTrxn = (int)HstAtmTxnsDataTable.Rows[K]["endtxn"];
                                    //************************************************************
                                    //// Check if this falls within the previous Trace Number 


                                    MasterTraceNo = TraceNo;

                                    Int32 LastDigit = MasterTraceNo % 10;

                                    if (LastDigit == 0)
                                    {
                                        // OK
                                    }
                                    else
                                    {
                                        //Entries from Supervisor mode ( 5 and 7 digit)
                                        MasterTraceNo = (MasterTraceNo - LastDigit) + 1;
                                    }
                                    //**************************************************************
                                    //Reconciled = (Convert.ToInt32)HstAtmTxnsDataTable.Rows[K]["Reconciled"];

                                    //Reconciled = 0;

                                    TRanDate = (DateTime)HstAtmTxnsDataTable.Rows[K]["TRanDate"];  // HERE WE HAVE THE FULL
                                    if (TRanDate == NullDate)
                                    {
                                        TRanDate = DateTime.Now;
                                    }

                                    WSesNo = 0; 


                                    // 

                                    if (TransactionType < 40 & TransactionType != 0)
                                    {
                                        // These are for money Transactions 
                                        CurrDesc = (string)HstAtmTxnsDataTable.Rows[K]["currency"];
                                        //TEST .... REMOVE IT AFTER WE HAVE JOURNALS WITH PHP 
                                        //CurrDesc = Ac.DepCurNm;
                                        //TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];

                                       // CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];
                                        PresError = (string)HstAtmTxnsDataTable.Rows[K]["PresError"];
                                        //
                                        // We check for Suspect Notes
                                        //
                                        SuspectDesc = (string)HstAtmTxnsDataTable.Rows[K]["SuspectDesc"];

                                        CardCapturedMes = (string)HstAtmTxnsDataTable.Rows[K]["CardCapturedMes"];

                                        CommissionCode = (int)HstAtmTxnsDataTable.Rows[K]["CommissionCode"];
                                        CommissionAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["CommissionAmount"];
                                        CardTargetSystem = (int)HstAtmTxnsDataTable.Rows[K]["CardTargetSystem"];

                                    }

                                    

                                    //********************************************
                                    // PREPARE READ MONEY TRANSACTIONS
                                    //********************************************

                                    if (
                                        (
                                           TransactionType == 11  // trandesc == "WITHDRAWAL" OR trandesc == "ΑΝΑΛΗΨΗ"
                                        || TransactionType == 23 // trandesc == "DEPOSIT_BNA" // BNA // 
                                        || TransactionType == 24 // trandesc == "DEPOSIT" // Cheque Deposit // 
                                        || TransactionType == 25  //trandesc == "ΚΑΤΑΘΕΣΗ" // Envelop Deposit // 
                                        || TransactionType == 33  //trandesc == "TRANSFE" // Transfer //
                                        )
                                        & Result.Substring(0, 2) == "OK" & TranAmount > 0)
                                    {

                                        GrandTotalTxns = GrandTotalTxns + 1;

                                        TotalTxns = TotalTxns + 1;

                                        Wtrandesc = trandesc;

                                        // FIND WORKING CATEGORY

                                        WMatchingCategoryId = "";

                                        WMatchingCategoryId = PRX + "299"; //LEAVE LIKE THIS

                                        OwnCard = false;

                                        //if (WOperator == "BCAIEGCX")
                                        //{
                                        //    // Bank de Caire

                                        //    if (CardNo.Substring(0, 6) == "526402" )
                                        //    {
                                        //        //if (CardNo.Substring(0, 6) == "507803")
                                        //        //{
                                        //        //    WMatchingCategoryId = "BDC201";
                                        //        //}
                                        //        WMatchingCategoryId = PRX+"299"; //LEAVE LIKE THIS
                                        //        OwnCard = true;
                                        //    }
                                        //    else
                                        //    {
                                        //        WMatchingCategoryId = PRX + "299"; // ALL OTHERS 
                                        //        OwnCard = false; 
                                        //    }

                                        //}
                                        //else
                                        //{
                                        //    if (WOperator == "BDACEGCA")
                                        //    {
                                        //        // Agreculture Bank 

                                        //        if (CardNo.Substring(0, 6) == "489322")
                                        //        {
                                        //            //if (CardNo.Substring(0, 6) == "507803")
                                        //            //{
                                        //            //    WMatchingCategoryId = "BDC201";
                                        //            //}
                                        //            WMatchingCategoryId = PRX + "299"; // LEAVE LIKE THIS
                                        //        }
                                        //        else
                                        //        {
                                        //            WMatchingCategoryId = PRX + "299"; // ALL OTHERS 
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, CardNo.Substring(0, 6), "Our Atms", 12);
                                        //        if (Mcb.RecordFound)
                                        //        {
                                        //            // Own Card
                                        //            WMatchingCategoryId = Mcb.CategoryId;
                                        //            OwnCard = true;
                                        //        }
                                        //        else
                                        //        {

                                        //            // Other Card 
                                        //            Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
                                        //            if (Mc.RecordFound == true)
                                        //            {
                                        //                WMatchingCategoryId = Mc.CategoryId;
                                        //            }
                                        //            OwnCard = false;
                                        //        }

                                        //    }
                                        //}


                                        UniqueRecordId = Gu.GetNextValue();
                                       
                                        // FILL Data Table 
                                        DataRow RowSelected = TempTableMpa.NewRow();

                                        RowSelected["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                        RowSelected["OriginalRecordId"] = WSeqNo;
                                        RowSelected["UniqueRecordId"] = UniqueRecordId;

                                        RowSelected["MatchingCateg"] = WMatchingCategoryId;

                                        RowSelected["FuID"] = fuid;

                                        RowSelected["SeqNo01"] = WSeqNo;
                           
                                        RowSelected["FileId01"] = "";
                                        RowSelected["FileId02"] = "";
                                        RowSelected["FileId03"] = "";
                                        RowSelected["FileId04"] = "";
                                        RowSelected["FileId05"] = "";

                                        RowSelected["Card_Encrypted"] = "";

                                        RowSelected["TXNSRC"] = "1";
                                        RowSelected["TXNDEST"] = "";

                                        RowSelected["ACCEPTOR_ID"] = "";
                                        RowSelected["ACCEPTORNAME"] = "";


                                        //Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);
                                        //if (Mc.RecordFound == false)
                                        //{
                                        //    // For testing

                                        //    Mc.TransTypeAtOrigin = "Not Defined";
                                        //    Mc.Product = "Not Defined";
                                        //    Mc.CostCentre = "Not Defined";
                                        //    Mc.TargetSystemId = 0;
                                        //}

                                        RowSelected["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                        RowSelected["Origin"] = "Our Atms";
                                        //RowSelected["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                        //RowSelected["Product"] = Mc.Product;
                                        //RowSelected["CostCentre"] = Mc.CostCentre;
                                        //RowSelected["TargetSystem"] = Mc.TargetSystemId;
                                        RowSelected["TransTypeAtOrigin"] = "";
                                        RowSelected["Product"] = "";
                                        RowSelected["CostCentre"] = "";
                                        RowSelected["TargetSystem"] = 1;

                                        decimal DepCount = 0;

                                        if (TransactionType < 20) // "WITHDRAWAL"
                                        {
                                            DepCount = 0;
                                        }

                                        if (TransactionType > 20 & TransactionType < 30)
                                        {
                                            DepCount = TranAmount;
                                        }

                                        RowSelected["LoadedAtRMCycle"] = WLoadedAtRMCycle;
                                        RowSelected["MatchingAtRMCycle"] = 0;
                                        RowSelected["TerminalId"] = WAtmNo;
                                        RowSelected["TransType"] = TransactionType;
                                        RowSelected["DepCount"] = DepCount;

                                        RowSelected["TransDescr"] = Wtrandesc;
                                        RowSelected["CardNumber"] = CardNo;

                                        RowSelected["IsOwnCard"] = OwnCard;

                                        RowSelected["AccNumber"] = AccNo;
                                        RowSelected["TransCurr"] = CurrDesc;

                                        RowSelected["TransAmount"] = TranAmount;
                                        RowSelected["TransDate"] = TRanDate;
                                        RowSelected["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
                                        RowSelected["AtmTraceNo"] = TraceNo;

                                        RowSelected["MasterTraceNo"] = MasterTraceNo;
                                        //*******************************************
                                        // PREPARE AND INSERT ERROR
                                        //*******************************************

                                        int ErrorNoInserted = 0;

                                        CreateReversal = false;

                                        if (Handle_Suspects(WAtmNo, int.Parse(TraceNoWithNoEndZero),
                                                    fuid) == true)
                                        {
                                            SuspectDesc = "SUSPECT FOUND_TEST";
                                        }


                                        if (PresError == "PresenterError" || SuspectDesc == "SUSPECT FOUND")
                                        {
                                            // THERE IS ERROR 
                                            // INSERT ERROR

                                            if (PresError == "PresenterError")
                                            {
                                                Ec.ErrId = 55;

                                                ParId = "421"; // Check if this Bank makes reversal if presenter 
                                                OccurId = "1";
                                                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                                                if (Gp.OccuranceNm == "YES")
                                                {
                                                    CreateReversal = true;
                                                }
                                                else
                                                {
                                                    CreateReversal = false;
                                                }

                                            }
                                            if (SuspectDesc == "SUSPECT FOUND")
                                            {
                                                Ec.ErrId = 225;
                                            }

                                            Ec.BankId = WOperator;
                                            Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                                            Am.ReadAtmsMainSpecific(WAtmNo);

                                            // INITIALISED WHAT IS NEEDED 

                                            Ec.CategoryId = WMatchingCategoryId;

                                            Ec.RMCycle = WLoadedAtRMCycle;
                                            Ec.UniqueRecordId = UniqueRecordId;

                                            Ec.AtmNo = WAtmNo;
                                            Ec.SesNo = WSesNo;
                                            Ec.DateInserted = DateTime.Now;
                                            Ec.DateTime = TRanDate;
                                            Ec.BranchId = Ac.Branch;
                                            Ec.ByWhom = WSignedId;

                                            Ec.CurDes = CurrDesc;
                                            Ec.ErrAmount = TranAmount;

                                            Ec.TraceNo = TraceNo;
                                            Ec.CardNo = CardNo;

                                            Ec.TransType = TransactionType;
                                            Ec.TransDescr = Wtrandesc;

                                            Ec.CustAccNo = AccNo;

                                            Ec.DatePrinted = NullPastDate;

                                            Ec.OpenErr = true;

                                            Ec.CitId = Am.CitId;

                                            Ec.Operator = WOperator;

                                            ErrorNoInserted = Ec.InsertError(); // INSERT ERROR                               

                                        }

                                        int TempMetaExceptionId = 0;

                                        if (ErrorNoInserted > 0)
                                        {
                                            TempMetaExceptionId = Ec.ErrId;
                                        }
                                        else
                                        {
                                            TempMetaExceptionId = 0;
                                        }

                                        RowSelected["MetaExceptionNo"] = ErrorNoInserted;
                                        RowSelected["MetaExceptionId"] = TempMetaExceptionId;

                                        RowSelected["RRNumber"] = "0";
                                        RowSelected["ResponseCode"] = ResponseCode;
                                        RowSelected["Operator"] = WOperator;
                                        RowSelected["ReplCycleNo"] = WSesNo; // WSesNo

                                        // ADD ROW
                                        TempTableMpa.Rows.Add(RowSelected);

                                        // Try this
                                        if (CreateReversal == true)
                                        {
                                            DataRow RowSelected2 = TempTableMpa.NewRow();

                                            RowSelected2["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                            RowSelected2["OriginalRecordId"] = TraceNo;
                                            RowSelected2["UniqueRecordId"] = UniqueRecordId;

                                            RowSelected2["MatchingCateg"] = WMatchingCategoryId;

                                            RowSelected2["FuID"] = fuid;

                                            RowSelected["SeqNo01"] = WSeqNo;
                                            RowSelected2["FileId01"] = "";
                                            RowSelected2["FileId02"] = "";
                                            RowSelected2["FileId03"] = "";
                                            RowSelected2["FileId04"] = "";
                                            RowSelected2["FileId05"] = "";

                                            RowSelected2["Card_Encrypted"] = "";
                                            RowSelected2["TXNSRC"] = "1";
                                            RowSelected2["TXNDEST"] = "";

                                            RowSelected2["ACCEPTOR_ID"] = "";
                                            RowSelected2["ACCEPTORNAME"] = "";

                                            //Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);

                                            RowSelected2["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                            RowSelected2["Origin"] = "Our Atms";
                                            RowSelected2["TransTypeAtOrigin"] = "";
                                            RowSelected2["Product"] = "";
                                            RowSelected2["CostCentre"] = "";
                                            RowSelected2["TargetSystem"] = 0;

                                            RowSelected2["LoadedAtRMCycle"] = WLoadedAtRMCycle;
                                            RowSelected2["MatchingAtRMCycle"] = 0;
                                            RowSelected2["TerminalId"] = WAtmNo;
                                            RowSelected2["TransType"] = 22;         // REVERSAL CODE
                                            RowSelected2["DepCount"] = DepCount;

                                            RowSelected2["TransDescr"] = "Reversal Entry For Trace No : " + TraceNo.ToString();
                                            RowSelected2["CardNumber"] = CardNo;
                                            RowSelected2["IsOwnCard"] = OwnCard;

                                            RowSelected2["AccNumber"] = AccNo;
                                            RowSelected2["TransCurr"] = CurrDesc;

                                            RowSelected2["TransAmount"] = TranAmount;
                                            RowSelected2["TransDate"] = TRanDate;
                                            RowSelected2["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
                                            RowSelected2["AtmTraceNo"] = TraceNo;

                                            RowSelected2["MasterTraceNo"] = MasterTraceNo;

                                            RowSelected2["MetaExceptionNo"] = ErrorNoInserted;
                                            RowSelected2["MetaExceptionId"] = TempMetaExceptionId;

                                            RowSelected2["RRNumber"] = "0";
                                            RowSelected2["ResponseCode"] = ResponseCode;
                                            RowSelected2["Operator"] = WOperator;
                                            RowSelected2["ReplCycleNo"] = WSesNo; // WSesNo

                                            TempTableMpa.Rows.Add(RowSelected2);
                                        }

                                    }

                                    // UPDATE DEPOSITS WITH REPLCycle
                                    if (TransactionType == 23)
                                    {
                                      //  Da.Update_Deposit_Txns_Analysis_ReplCycleNo(WAtmNo, WSesNo, int.Parse(TraceNoWithNoEndZero), TRanDate);

                                    }

                                    // Deal with Captured cards

                                    if (CardCaptured == "CARD CAPTURED") // Trace 
                                    {
                                        Method_Captured_Cards(CardNo, CardCapturedMes);
                                    }
                                    // 
                                    // UPDATE STATISTICS
                                    //
                                    if (PreTranDate.Date == NullPastDate)
                                    {
                                        PreTranDate = TRanDate.Date;
                                    }

                                    if (TRanDate.Date != PreTranDate.Date)
                                    {
                                        // Update Totals
                                        Method_UPdate_Stats_For_ATM_Trans(WAtmNo, PreTranDate.Date);
                                        // Nullify old 
                                        DrTransactions = 0;
                                        DispensedAmt = 0;
                                        CrTransactions = 0;
                                        DepAmount = 0;

                                        PreTranDate = TRanDate;
                                    }

                                    // Update Totals
                                    if (TransactionType >= 10 & TransactionType < 20)
                                    {
                                        DrTransactions = DrTransactions + 1;
                                        DispensedAmt = DispensedAmt + TranAmount;
                                    }
                                    else
                                    {
                                        if (TransactionType >= 20 & TransactionType <= 30)
                                        {
                                            // Over 20 = Deposits 
                                            CrTransactions = CrTransactions + 1;
                                            DepAmount = DepAmount + TranAmount;
                                        }

                                    }

                                    K++; // Read Next entry of the table 
                                }


                                // ****************INSERT BY PANICOS**************
                                Message = "READ PAMBOS AND CREATE MEMORY TABLE FOR ATM..." + WAtmNo + " NUMBER OF RECs.." + K.ToString();

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "PROCESS HST", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************
                                // Update MPA with entries in table 
                                if (TotalValidRecords > 0)
                                {
                                    // RECORDS READ AND PROCESSED 
                                    //TableMpa
                                    using (SqlConnection conn2 =
                                                   new SqlConnection(connectionString))
                                        try
                                        {
                                            conn2.Open();

                                            using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                                            {
                                                s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";
                                                
                                                foreach (var column in TempTableMpa.Columns)
                                                    s.ColumnMappings.Add(column.ToString(), column.ToString());

                                                s.BulkCopyTimeout = 350;
                                                s.WriteToServer(TempTableMpa);
                                            }
                                            conn2.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            conn2.Close();

                                            Major_ErrorFound = true;

                                            CatchDetails(ex);

                                            //if (Environment.UserInteractive)
                                            //{
                                            //    System.Windows.Forms.MessageBox.Show("Cancelled While Inerting Table In Pool");

                                            //}

                                            return;
                                        }
                                }

                                // ****************INSERT BY PANICOS**************
                                Message = "INSERT HST DATA TABLE IN Mpa .. for ATM" + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "INSERT HST IN Mpa", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************

                                if (TotalValidRecords > 0)
                                {
                                    // 
                                    // UPDATE STATISTICS  FOR DAILY DEBITS AND CREDITS
                                    // 
                                    Method_UPdate_Stats_For_ATM_Trans(WAtmNo, PreTranDate.Date);
                                    //++++++++++++++++++++++++++++++++++   
                                    // Updating as Processed all records read 
                                    //++++++++++++++++++++++++++++++++++
                                    if (WMode == 1 || WMode == 2)
                                    {
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm(WAtmNo, MaxSeqNo);
                                    }
                                    // ONLY FUID
                                    if (WMode == 3)
                                    {
                                        // Based on fuid
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(WAtmNo, WFuid);
                                    }

                                }
                                // ****************INSERT BY PANICOS**************
                                Message = "UPDATE HST as process..For ATM.. " + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "UPDATE HST AS PROCESSED", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                //BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************
                            }

                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            CatchDetails(ex);
                        }
                    // 
                    // Update after all are finished 
                    // 
                    if (TotalValidRecords > 0)
                    {
                        try
                        {

                            // 
                            // We Update the last ATM
                            //if (WSesNo > 0)
                            //{
                            //    Method_UpdateOldAtm(WAtmNo);
                            //}

                        }
                        catch (Exception ex)
                        {
                            CatchDetails(ex);
                        }
                    }


                    I++; // Read Next ATM entry of the table 

                }

                // ****************INSERT BY PANICOS**************
                //Message = "ALL ATMs Loaded for Group : " + WAtmsReconcGroup.ToString();

                //Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "Load ALL ATMs", WAtmNo, StartAtmsDate, DateTime.Now, Message);
                if (WMode == 1)
                {
                    if (Environment.UserInteractive)
                    {
                        //System.Windows.Forms.MessageBox.Show(StartGroupDateTime.ToString() + Environment.NewLine
                        //                             + DateTime.Now.ToString() + Environment.NewLine
                        //                           + Message
                        //                            );
                    }


                    Gr.DeleteGroupFromCounter(WAtmsReconcGroup);

                    int Sum = Gr.ReadRowsInGroupTable();

                    if (Sum == 0)
                    {
                        if (Environment.UserInteractive)
                        {
                            // System.Windows.Forms.MessageBox.Show("ALL GROUPS LOADED");
                        }


                    }

                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
                //CatchDetails(ex);

            }
        }

        public void ReadJournal_Txns_And_Insert_In_Pool_ROM(string InSignedId, int InSignRecordNo, string InOperator,
                                                            int InAtmsReconcGroup, string InAtmNo, int InFuid, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            WAtmsReconcGroup = InAtmsReconcGroup;

            WAtmNo = InAtmNo;

            WMode = InMode; // 1 Group of ATMs
                            // 2 Single ATM with zero Fuid
                            // 3 Single ATM with Fuid for multithreading
                            // 4 ALL ATMS 

            if (WMode == 3)
            {
                Major_ErrorFound = false;
                return;
            }
            WFuid = InFuid; // If mode = 3 then this has a value ... it gets a value when called by Alecos after Bambos is called and a Fuid is given.
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

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }



            // If call by Alecos = Mode = 3 then before reading the transactions check 
            // if for this FUI there is Replenishement through Pambos Table. 
            // If there Update old Cycle and Create new. => Update Bambos with the old and the new Repl Cycle No. 
            // Keep date and old and new Cycle Number
            // Read all transactions of this Fui and Update any less date with Old Repl Cycle and any greater than date with the new 
            // ''''''''''''''''''''''''''''''
            // If for this Fui there is no replenishement 
            // Read the last replenishement and find Date of it and Repl Cycle number 
            // During transactions reading there are txns for this ATM < than last Cycle number then update them with the repl cycle number
            string ParId = "720";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string SM_Management = Gp.OccuranceNm;


            // For this ATM Group
            // Read transactions from Pambos EJournal file
            // Read Per ATM , sort transactions by date time. 
            // 
            // 1) Update Mpa Trans
            // 2) Update Captured Cards
            // 3) Update cassettes
            // 4) Update First Last Trace or maybe not? Yes it is better here than in replenishment workflow 
            // 5) Create New Supervisor Mode Cycles
            // 6) Update Daily statistics
            // 7) Update CIT orders with exact Loading amount and cassettes

            if (WMode == 1) Gr.InsertGroupDuringLoading(WAtmsReconcGroup);


            // SHOW MESSAGE 
            ParId = "719";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                ShowMessage = true;
            }
            else
            {
                ShowMessage = false;
            }

            DateTime NullDate = new DateTime(1900, 01, 01);
            //

            RecordFound = false;
            Major_ErrorFound = false;
            ErrorOutput = "";

            //  PreStartTrxn = 0;

            // FIND THE ATMS 

            if (WMode == 1)
            {
                // Get All Atms
                SqlString = "SELECT *"
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmsReconcGroup = @AtmsReconcGroup";
            }
            if (WMode == 2 & WAtmNo != "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmNo = @AtmNo ";
            }
            if (WMode == 3 & WAtmNo != "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator AND AtmNo = @AtmNo ";
            }

            if (WMode == 4 & WAtmNo == "")
            {   //
                // Get Single Atm
                //
                SqlString = "SELECT * "
                 + " FROM [ATMS].[dbo].[AtmsMain] "
                 + " Where Operator = @Operator ";
            }

            TableAtms = new DataTable();
            TableAtms.Clear();

            using (SqlConnection conn =
                 new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);
                        if (WMode == 1)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmsReconcGroup", WAtmsReconcGroup);
                        }

                        if (WMode == 2 || WMode == 3)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);
                        }
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableAtms);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            // loop
            try
            {
                // LOOP FOR ATMS

                DateTime StartAtmsDate = DateTime.Now;

                int I = 0;

                while (I <= (TableAtms.Rows.Count - 1))
                {

                    RecordFound = true;

                    WAtmNo = (string)TableAtms.Rows[I]["AtmNo"];

                    // ****************INSERT BY PANICOS**************
                    // Initialise for next
                    BeforeCallDtTime = DateTime.Now;
                    // *************************END INSERT BY PANICOS********************************


                    Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                    WOperator = Ac.Operator;

                    Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);
                    WReconCategoryId = Rc.CategoryId;

                    WCitId = Ac.CitId;

                    // INITIALISE AND Define the data table 

                    TotalValidRecords = 0;
                    TotalTxns = 0;
                    MaxSeqNo = 0;

                    HstAtmTxnsDataTable = new DataTable();
                    HstAtmTxnsDataTable.Clear();

                    TempTableMpa = new DataTable();
                    TempTableMpa.Clear();

                    MpaDataTableDefinition();

                    SqlString1_1 = "SELECT SeqNo, ISNULL(atmno, '') AS atmno, ISNULL(TraceNumber, 0) AS TraceNumber,  ISNULL(Trace, '') As TraceNoWithNoEndZero"
                    + ",ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc, "
                 + "ISNULL(currency, '') AS currency, ISNULL(CAmount, 0 ) AS CAmount, ISNULL(cardnum, '') AS cardnum,"
                  + "ISNULL(Sessionstart, 0) AS starttxn,ISNULL(SessionEnd, 0) AS endtxn, "
                  + " ISNULL(txtline, '') AS txtline, "
                  //+ "ISNULL(TranTime, '') AS trantime,"
                  + "ISNULL(acct1, '') AS acct1, "
                  + "ISNULL(acct2, '') AS acct2, "
                  + "ISNULL(Result, '') AS Result, ISNULL(Source, '') AS ResponseCode,"
                  + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
                  + "ISNULL(PresenterError, '') AS PresError, "
                   + " ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
                  + "ISNULL(Type1, 0) AS Type1, "
                  + "ISNULL(Type2, 0) AS Type2, "
                  + "ISNULL(Type3, 0) AS Type3, "
                  + "ISNULL(Type4, 0) AS Type4, "
                  + " Processed, "
                  + " ISNULL(CardTargetSystem, 0) AS CardTargetSystem, "
                  + " ISNULL(TransactionType, 0) AS TransactionType, "
                  + " ISNULL(CommissionCode, 0) AS CommissionCode, "
                  + " ISNULL(CommissionAmount, 0) AS CommissionAmount, "
                  + " ISNULL(Trandatetime, '') AS TRanDate "

                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] ";
                    //  + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] ";
                    //+ " WHERE Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                    //+ " ORDER by AtmNo, TranDate, TranTime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          

                    if (WMode == 1 || WMode == 2)
                    {
                        // Covering groups of ATMs
                        SqlString1_2 =
                         " WHERE AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          
                        //+ "  ORDER by Atmno, fuid, RuId "; // NO NEED To Cover Trace less

                    }

                    //if (WMode == 3)
                    //{
                    //    // Covering only one ATM with known Fuid
                    //    SqlString1_2 =
                    //     "  WHERE fuid=@fuid  AND AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                    //     + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          
                    //                                                         //+ "  ORDER by Atmno, fuid, RuId "; // NO NEED To Cover Trace less 
                    //}

                    if (WMode == 3)
                    {
                        // Covering only one ATM with known Fuid
                        SqlString1_2 =
                         "  WHERE fuid=@fuid AND TransactionType <> 99 and result = 'OK' "
                         + " ORDER BY RuID ";
                    }

                    if (WMode == 4)
                    {
                        // Covering only one ATM with known Fuid
                        SqlString1_2 =
                         "  WHERE AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK' "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber ";
                    }

                    SqlString = SqlString1_1 + SqlString1_2;

                    using (SqlConnection conn =
                       new SqlConnection(connectionString))
                        try
                        {
                            conn.Open();
                            //Create an Sql Adapter that holds the connection and the command
                            using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                            {

                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@atmno", WAtmNo);

                                sqlAdapt.SelectCommand.CommandTimeout = 300;

                                //Create a datatable that will be filled with the data retrieved from the command
                                if (WMode == 3)
                                {
                                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@fuid", WFuid);
                                }

                                sqlAdapt.Fill(HstAtmTxnsDataTable);

                                // Close conn
                                conn.Close();

                                if (HstAtmTxnsDataTable.Rows.Count == 0)
                                {
                                    // ONLY FUID - Update all records as processed
                                    if (WMode == 3)
                                    {
                                        // Based on fuid
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(WAtmNo, WFuid);
                                    }

                                    I = I + 1;

                                    continue;
                                }

                                PreTranDate = NullPastDate;
                                DrTransactions = 0;
                                DispensedAmt = 0;
                                CrTransactions = 0;
                                DepAmount = 0;

                                int K = 0;

                                while (K <= (HstAtmTxnsDataTable.Rows.Count - 1))
                                {
                                    // GET Table fields - Line by Line
                                    //
                                    int SeqNo = (int)HstAtmTxnsDataTable.Rows[K]["SeqNo"];

                                    TransactionType = (int)HstAtmTxnsDataTable.Rows[K]["TransactionType"];

                                    Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];
                                    TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];

                                    CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];

                                    if (TranAmount > 0 || CardCaptured == "CARD CAPTURED")
                                    {
                                        if (CardCaptured == "CARD CAPTURED" || SeqNo == 14212)
                                        {
                                            CardCaptured = CardCaptured;
                                        }
                                        // OK
                                        // If CARD Captured we do special processing 
                                        //// TEMPORARY FOR AUDI
                                        //if (CardCaptured == "CARD CAPTURED")
                                        //{
                                        //    TransactionType = 39; // means is not a valid transaction
                                        //}
                                    }
                                    else
                                    {
                                        K = K + 1;
                                        continue;
                                    }


                                    RecordFound = true;

                                    TotalValidRecords = TotalValidRecords + 1;


                                    WSeqNo = (int)HstAtmTxnsDataTable.Rows[K]["SeqNo"];


                                    TraceNo = (int)HstAtmTxnsDataTable.Rows[K]["TraceNumber"];

                                    TraceNoWithNoEndZero = (string)HstAtmTxnsDataTable.Rows[K]["TraceNoWithNoEndZero"];

                                    fuid = (int)HstAtmTxnsDataTable.Rows[K]["fuid"];
                                    trandesc = (string)HstAtmTxnsDataTable.Rows[K]["trandesc"];

                                    CardNo = (string)HstAtmTxnsDataTable.Rows[K]["cardnum"];

                                    txtline = (string)HstAtmTxnsDataTable.Rows[K]["txtline"];

                                    //Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];

                                    ResponseCode = ((string)HstAtmTxnsDataTable.Rows[K]["ResponseCode"]).Trim();

                                    if (ResponseCode == "000" || ResponseCode == "00")
                                    {
                                        ResponseCode = "0";
                                    }
                                    //else
                                    //{
                                    //    ResponseCode ="0"; 
                                    //}

                                    if (TransactionType == 11 || TransactionType == 33)
                                    {
                                        AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct1"];
                                    }
                                    else
                                    {
                                        // This is deposit
                                        AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct2"];
                                    }

                                    StartTrxn = (int)HstAtmTxnsDataTable.Rows[K]["starttxn"];
                                    EndTrxn = (int)HstAtmTxnsDataTable.Rows[K]["endtxn"];
                                    //************************************************************
                                    //// Check if this falls within the previous Trace Number 


                                    MasterTraceNo = TraceNo;

                                    Int32 LastDigit = MasterTraceNo % 10;

                                    if (LastDigit == 0)
                                    {
                                        // OK
                                    }
                                    else
                                    {
                                        //Entries from Supervisor mode ( 5 and 7 digit)
                                        MasterTraceNo = (MasterTraceNo - LastDigit) + 1;
                                    }
                                    //**************************************************************
                                    //Reconciled = (Convert.ToInt32)HstAtmTxnsDataTable.Rows[K]["Reconciled"];

                                    //Reconciled = 0;

                                    TRanDate = (DateTime)HstAtmTxnsDataTable.Rows[K]["TRanDate"];  // HERE WE HAVE THE FULL
                                    if (TRanDate == NullDate)
                                    {
                                        TRanDate = DateTime.Now;
                                    }

                                    WSesNo = 0;


                                    // 

                                    if (TransactionType < 40 & TransactionType != 0)
                                    {
                                        // These are for money Transactions 
                                        CurrDesc = (string)HstAtmTxnsDataTable.Rows[K]["currency"];
                                        //TEST .... REMOVE IT AFTER WE HAVE JOURNALS WITH PHP 
                                        //CurrDesc = Ac.DepCurNm;
                                        //TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];

                                        // CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];
                                        PresError = (string)HstAtmTxnsDataTable.Rows[K]["PresError"];
                                        //
                                        // We check for Suspect Notes
                                        //
                                        SuspectDesc = (string)HstAtmTxnsDataTable.Rows[K]["SuspectDesc"];

                                        CardCapturedMes = (string)HstAtmTxnsDataTable.Rows[K]["CardCapturedMes"];

                                        CommissionCode = (int)HstAtmTxnsDataTable.Rows[K]["CommissionCode"];
                                        CommissionAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["CommissionAmount"];
                                        CardTargetSystem = (int)HstAtmTxnsDataTable.Rows[K]["CardTargetSystem"];

                                    }



                                    //********************************************
                                    // PREPARE READ MONEY TRANSACTIONS
                                    //********************************************

                                    if (
                                        (
                                           TransactionType == 11  // trandesc == "WITHDRAWAL" OR trandesc == "ΑΝΑΛΗΨΗ"
                                        || TransactionType == 23 // trandesc == "DEPOSIT_BNA" // BNA // 
                                        || TransactionType == 24 // trandesc == "DEPOSIT" // Cheque Deposit // 
                                        || TransactionType == 25  //trandesc == "ΚΑΤΑΘΕΣΗ" // Envelop Deposit // 
                                        || TransactionType == 33  //trandesc == "TRANSFE" // Transfer //
                                        )
                                        & Result.Substring(0, 2) == "OK" & TranAmount > 0)
                                    {

                                        GrandTotalTxns = GrandTotalTxns + 1;

                                        TotalTxns = TotalTxns + 1;

                                        Wtrandesc = trandesc;

                                        // FIND WORKING CATEGORY

                                        WMatchingCategoryId = "";

                                        WMatchingCategoryId = PRX + "299"; //LEAVE LIKE THIS

                                        OwnCard = false;

                                        //if (WOperator == "BCAIEGCX")
                                        //{
                                        //    // Bank de Caire

                                        //    if (CardNo.Substring(0, 6) == "526402" )
                                        //    {
                                        //        //if (CardNo.Substring(0, 6) == "507803")
                                        //        //{
                                        //        //    WMatchingCategoryId = "BDC201";
                                        //        //}
                                        //        WMatchingCategoryId = PRX+"299"; //LEAVE LIKE THIS
                                        //        OwnCard = true;
                                        //    }
                                        //    else
                                        //    {
                                        //        WMatchingCategoryId = PRX + "299"; // ALL OTHERS 
                                        //        OwnCard = false; 
                                        //    }

                                        //}
                                        //else
                                        //{
                                        //    if (WOperator == "BDACEGCA")
                                        //    {
                                        //        // Agreculture Bank 

                                        //        if (CardNo.Substring(0, 6) == "489322")
                                        //        {
                                        //            //if (CardNo.Substring(0, 6) == "507803")
                                        //            //{
                                        //            //    WMatchingCategoryId = "BDC201";
                                        //            //}
                                        //            WMatchingCategoryId = PRX + "299"; // LEAVE LIKE THIS
                                        //        }
                                        //        else
                                        //        {
                                        //            WMatchingCategoryId = PRX + "299"; // ALL OTHERS 
                                        //        }

                                        //    }
                                        //    else
                                        //    {
                                        //        Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, CardNo.Substring(0, 6), "Our Atms", 12);
                                        //        if (Mcb.RecordFound)
                                        //        {
                                        //            // Own Card
                                        //            WMatchingCategoryId = Mcb.CategoryId;
                                        //            OwnCard = true;
                                        //        }
                                        //        else
                                        //        {

                                        //            // Other Card 
                                        //            Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
                                        //            if (Mc.RecordFound == true)
                                        //            {
                                        //                WMatchingCategoryId = Mc.CategoryId;
                                        //            }
                                        //            OwnCard = false;
                                        //        }

                                        //    }
                                        //}


                                        UniqueRecordId = Gu.GetNextValue();

                                        // FILL Data Table 
                                        DataRow RowSelected = TempTableMpa.NewRow();

                                        RowSelected["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                        RowSelected["OriginalRecordId"] = WSeqNo;
                                        RowSelected["UniqueRecordId"] = UniqueRecordId;

                                        RowSelected["MatchingCateg"] = WMatchingCategoryId;

                                        RowSelected["FuID"] = fuid;

                                        RowSelected["SeqNo01"] = WSeqNo;

                                        RowSelected["FileId01"] = "";
                                        RowSelected["FileId02"] = "";
                                        RowSelected["FileId03"] = "";
                                        RowSelected["FileId04"] = "";
                                        RowSelected["FileId05"] = "";

                                        RowSelected["Card_Encrypted"] = "";

                                        RowSelected["TXNSRC"] = "1";
                                        RowSelected["TXNDEST"] = "";

                                        RowSelected["ACCEPTOR_ID"] = "";
                                        RowSelected["ACCEPTORNAME"] = "";


                                        //Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);
                                        //if (Mc.RecordFound == false)
                                        //{
                                        //    // For testing

                                        //    Mc.TransTypeAtOrigin = "Not Defined";
                                        //    Mc.Product = "Not Defined";
                                        //    Mc.CostCentre = "Not Defined";
                                        //    Mc.TargetSystemId = 0;
                                        //}

                                        RowSelected["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                        RowSelected["Origin"] = "Our Atms";
                                        //RowSelected["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                        //RowSelected["Product"] = Mc.Product;
                                        //RowSelected["CostCentre"] = Mc.CostCentre;
                                        //RowSelected["TargetSystem"] = Mc.TargetSystemId;
                                        RowSelected["TransTypeAtOrigin"] = "";
                                        RowSelected["Product"] = "";
                                        RowSelected["CostCentre"] = "";
                                        RowSelected["TargetSystem"] = 1;

                                        decimal DepCount = 0;

                                        if (TransactionType < 20) // "WITHDRAWAL"
                                        {
                                            DepCount = 0;
                                        }

                                        if (TransactionType > 20 & TransactionType < 30)
                                        {
                                            DepCount = TranAmount;
                                        }

                                        RowSelected["LoadedAtRMCycle"] = WLoadedAtRMCycle;
                                        RowSelected["MatchingAtRMCycle"] = 0;
                                        RowSelected["TerminalId"] = WAtmNo;
                                        RowSelected["TransType"] = TransactionType;
                                        RowSelected["DepCount"] = DepCount;

                                        RowSelected["TransDescr"] = Wtrandesc;
                                        RowSelected["CardNumber"] = CardNo;

                                        RowSelected["IsOwnCard"] = OwnCard;

                                        RowSelected["AccNumber"] = AccNo;
                                        RowSelected["TransCurr"] = CurrDesc;

                                        RowSelected["TransAmount"] = TranAmount;
                                        RowSelected["TransDate"] = TRanDate;
                                        RowSelected["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
                                        RowSelected["AtmTraceNo"] = TraceNo;

                                        RowSelected["MasterTraceNo"] = MasterTraceNo;
                                        //*******************************************
                                        // PREPARE AND INSERT ERROR
                                        //*******************************************

                                        int ErrorNoInserted = 0;

                                        CreateReversal = false;

                                        if (Handle_Suspects(WAtmNo, int.Parse(TraceNoWithNoEndZero),
                                                    fuid) == true)
                                        {
                                            SuspectDesc = "SUSPECT FOUND_TEST";
                                        }


                                        if (PresError == "PresenterError" || SuspectDesc == "SUSPECT FOUND")
                                        {
                                            // THERE IS ERROR 
                                            // INSERT ERROR

                                            if (PresError == "PresenterError")
                                            {
                                                Ec.ErrId = 55;

                                                ParId = "421"; // Check if this Bank makes reversal if presenter 
                                                OccurId = "1";
                                                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                                                if (Gp.OccuranceNm == "YES")
                                                {
                                                    CreateReversal = true;
                                                }
                                                else
                                                {
                                                    CreateReversal = false;
                                                }

                                            }
                                            if (SuspectDesc == "SUSPECT FOUND")
                                            {
                                                Ec.ErrId = 225;
                                            }

                                            Ec.BankId = WOperator;
                                            Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                                            Am.ReadAtmsMainSpecific(WAtmNo);

                                            // INITIALISED WHAT IS NEEDED 

                                            Ec.CategoryId = WMatchingCategoryId;

                                            Ec.RMCycle = WLoadedAtRMCycle;
                                            Ec.UniqueRecordId = UniqueRecordId;

                                            Ec.AtmNo = WAtmNo;
                                            Ec.SesNo = WSesNo;
                                            Ec.DateInserted = DateTime.Now;
                                            Ec.DateTime = TRanDate;
                                            Ec.BranchId = Ac.Branch;
                                            Ec.ByWhom = WSignedId;

                                            Ec.CurDes = CurrDesc;
                                            Ec.ErrAmount = TranAmount;

                                            Ec.TraceNo = TraceNo;
                                            Ec.CardNo = CardNo;

                                            Ec.TransType = TransactionType;
                                            Ec.TransDescr = Wtrandesc;

                                            Ec.CustAccNo = AccNo;

                                            Ec.DatePrinted = NullPastDate;

                                            Ec.OpenErr = true;

                                            Ec.CitId = Am.CitId;

                                            Ec.Operator = WOperator;

                                            ErrorNoInserted = Ec.InsertError(); // INSERT ERROR                               

                                        }

                                        int TempMetaExceptionId = 0;

                                        if (ErrorNoInserted > 0)
                                        {
                                            TempMetaExceptionId = Ec.ErrId;
                                        }
                                        else
                                        {
                                            TempMetaExceptionId = 0;
                                        }

                                        RowSelected["MetaExceptionNo"] = ErrorNoInserted;
                                        RowSelected["MetaExceptionId"] = TempMetaExceptionId;

                                        RowSelected["RRNumber"] = "0";
                                        RowSelected["ResponseCode"] = ResponseCode;
                                        RowSelected["Operator"] = WOperator;
                                        RowSelected["ReplCycleNo"] = WSesNo; // WSesNo

                                        // ADD ROW
                                        TempTableMpa.Rows.Add(RowSelected);

                                        // Try this
                                        if (CreateReversal == true)
                                        {
                                            DataRow RowSelected2 = TempTableMpa.NewRow();

                                            RowSelected2["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                            RowSelected2["OriginalRecordId"] = TraceNo;
                                            RowSelected2["UniqueRecordId"] = UniqueRecordId;

                                            RowSelected2["MatchingCateg"] = WMatchingCategoryId;

                                            RowSelected2["FuID"] = fuid;

                                            RowSelected["SeqNo01"] = WSeqNo;
                                            RowSelected2["FileId01"] = "";
                                            RowSelected2["FileId02"] = "";
                                            RowSelected2["FileId03"] = "";
                                            RowSelected2["FileId04"] = "";
                                            RowSelected2["FileId05"] = "";

                                            RowSelected2["Card_Encrypted"] = "";
                                            RowSelected2["TXNSRC"] = "1";
                                            RowSelected2["TXNDEST"] = "";

                                            RowSelected2["ACCEPTOR_ID"] = "";
                                            RowSelected2["ACCEPTORNAME"] = "";

                                            //Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);

                                            RowSelected2["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                            RowSelected2["Origin"] = "Our Atms";
                                            RowSelected2["TransTypeAtOrigin"] = "";
                                            RowSelected2["Product"] = "";
                                            RowSelected2["CostCentre"] = "";
                                            RowSelected2["TargetSystem"] = 0;

                                            RowSelected2["LoadedAtRMCycle"] = WLoadedAtRMCycle;
                                            RowSelected2["MatchingAtRMCycle"] = 0;
                                            RowSelected2["TerminalId"] = WAtmNo;
                                            RowSelected2["TransType"] = 22;         // REVERSAL CODE
                                            RowSelected2["DepCount"] = DepCount;

                                            RowSelected2["TransDescr"] = "Reversal Entry For Trace No : " + TraceNo.ToString();
                                            RowSelected2["CardNumber"] = CardNo;
                                            RowSelected2["IsOwnCard"] = OwnCard;

                                            RowSelected2["AccNumber"] = AccNo;
                                            RowSelected2["TransCurr"] = CurrDesc;

                                            RowSelected2["TransAmount"] = TranAmount;
                                            RowSelected2["TransDate"] = TRanDate;
                                            RowSelected2["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
                                            RowSelected2["AtmTraceNo"] = TraceNo;

                                            RowSelected2["MasterTraceNo"] = MasterTraceNo;

                                            RowSelected2["MetaExceptionNo"] = ErrorNoInserted;
                                            RowSelected2["MetaExceptionId"] = TempMetaExceptionId;

                                            RowSelected2["RRNumber"] = "0";
                                            RowSelected2["ResponseCode"] = ResponseCode;
                                            RowSelected2["Operator"] = WOperator;
                                            RowSelected2["ReplCycleNo"] = WSesNo; // WSesNo

                                            TempTableMpa.Rows.Add(RowSelected2);
                                        }

                                    }

                                    // UPDATE DEPOSITS WITH REPLCycle
                                    if (TransactionType == 23)
                                    {
                                        //  Da.Update_Deposit_Txns_Analysis_ReplCycleNo(WAtmNo, WSesNo, int.Parse(TraceNoWithNoEndZero), TRanDate);

                                    }

                                    // Deal with Captured cards

                                    if (CardCaptured == "CARD CAPTURED") // Trace 
                                    {
                                        Method_Captured_Cards(CardNo, CardCapturedMes);
                                    }
                                    // 
                                    // UPDATE STATISTICS
                                    //
                                    if (PreTranDate.Date == NullPastDate)
                                    {
                                        PreTranDate = TRanDate.Date;
                                    }

                                    if (TRanDate.Date != PreTranDate.Date)
                                    {
                                        // Update Totals
                                        Method_UPdate_Stats_For_ATM_Trans(WAtmNo, PreTranDate.Date);
                                        // Nullify old 
                                        DrTransactions = 0;
                                        DispensedAmt = 0;
                                        CrTransactions = 0;
                                        DepAmount = 0;

                                        PreTranDate = TRanDate;
                                    }

                                    // Update Totals
                                    if (TransactionType >= 10 & TransactionType < 20)
                                    {
                                        DrTransactions = DrTransactions + 1;
                                        DispensedAmt = DispensedAmt + TranAmount;
                                    }
                                    else
                                    {
                                        if (TransactionType >= 20 & TransactionType <= 30)
                                        {
                                            // Over 20 = Deposits 
                                            CrTransactions = CrTransactions + 1;
                                            DepAmount = DepAmount + TranAmount;
                                        }

                                    }

                                    K++; // Read Next entry of the table 
                                }


                                // ****************INSERT BY PANICOS**************
                                Message = "READ PAMBOS AND CREATE MEMORY TABLE FOR ATM..." + WAtmNo + " NUMBER OF RECs.." + K.ToString();

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "PROCESS HST", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************
                                // Update MPA with entries in table 
                                if (TotalValidRecords > 0)
                                {
                                    // RECORDS READ AND PROCESSED 
                                    //TableMpa
                                    using (SqlConnection conn2 =
                                                   new SqlConnection(connectionString))
                                        try
                                        {
                                            conn2.Open();

                                            using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                                            {
                                                s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

                                                foreach (var column in TempTableMpa.Columns)
                                                    s.ColumnMappings.Add(column.ToString(), column.ToString());

                                                s.BulkCopyTimeout = 350;
                                                s.WriteToServer(TempTableMpa);
                                            }
                                            conn2.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            conn2.Close();

                                            Major_ErrorFound = true;

                                            CatchDetails(ex);

                                            //if (Environment.UserInteractive)
                                            //{
                                            //    System.Windows.Forms.MessageBox.Show("Cancelled While Inerting Table In Pool");

                                            //}

                                            return;
                                        }
                                }

                                // ****************INSERT BY PANICOS**************
                                Message = "INSERT HST DATA TABLE IN Mpa .. for ATM" + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "INSERT HST IN Mpa", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************

                                if (TotalValidRecords > 0)
                                {
                                    // 
                                    // UPDATE STATISTICS  FOR DAILY DEBITS AND CREDITS
                                    // 
                                    Method_UPdate_Stats_For_ATM_Trans(WAtmNo, PreTranDate.Date);
                                    //++++++++++++++++++++++++++++++++++   
                                    // Updating as Processed all records read 
                                    //++++++++++++++++++++++++++++++++++
                                    if (WMode == 1 || WMode == 2)
                                    {
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm(WAtmNo, MaxSeqNo);
                                    }
                                    // ONLY FUID
                                    if (WMode == 3)
                                    {
                                        // Based on fuid
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(WAtmNo, WFuid);
                                    }

                                }
                                // ****************INSERT BY PANICOS**************
                                Message = "UPDATE HST as process..For ATM.. " + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "UPDATE HST AS PROCESSED", WAtmNo, BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                //BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************
                            }

                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            CatchDetails(ex);
                        }
                    // 
                    // Update after all are finished 
                    // 
                    if (TotalValidRecords > 0)
                    {
                        try
                        {

                            // 
                            // We Update the last ATM
                            //if (WSesNo > 0)
                            //{
                            //    Method_UpdateOldAtm(WAtmNo);
                            //}

                        }
                        catch (Exception ex)
                        {
                            CatchDetails(ex);
                        }
                    }


                    I++; // Read Next ATM entry of the table 

                }

                // ****************INSERT BY PANICOS**************
                //Message = "ALL ATMs Loaded for Group : " + WAtmsReconcGroup.ToString();

                //Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "Load ALL ATMs", WAtmNo, StartAtmsDate, DateTime.Now, Message);
                if (WMode == 1)
                {
                    if (Environment.UserInteractive)
                    {
                        //System.Windows.Forms.MessageBox.Show(StartGroupDateTime.ToString() + Environment.NewLine
                        //                             + DateTime.Now.ToString() + Environment.NewLine
                        //                           + Message
                        //                            );
                    }


                    Gr.DeleteGroupFromCounter(WAtmsReconcGroup);

                    int Sum = Gr.ReadRowsInGroupTable();

                    if (Sum == 0)
                    {
                        if (Environment.UserInteractive)
                        {
                            // System.Windows.Forms.MessageBox.Show("ALL GROUPS LOADED");
                        }


                    }

                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
                //CatchDetails(ex);

            }
        }

        private void MpaDataTableDefinition()
        {

            TempTableMpa.Columns.Add("OriginFileName", typeof(string));
            TempTableMpa.Columns.Add("OriginalRecordId", typeof(int));
            TempTableMpa.Columns.Add("UniqueRecordId", typeof(int));

            TempTableMpa.Columns.Add("MatchingCateg", typeof(string));

            TempTableMpa.Columns.Add("SeqNo01", typeof(int));
            TempTableMpa.Columns.Add("FileId01", typeof(string));
            TempTableMpa.Columns.Add("FileId02", typeof(string));
            TempTableMpa.Columns.Add("FileId03", typeof(string));
            TempTableMpa.Columns.Add("FileId04", typeof(string));
            TempTableMpa.Columns.Add("FileId05", typeof(string));

            TempTableMpa.Columns.Add("RMCateg", typeof(string));
            TempTableMpa.Columns.Add("Origin", typeof(string));
            TempTableMpa.Columns.Add("TransTypeAtOrigin", typeof(string));

            TempTableMpa.Columns.Add("Product", typeof(string));
            TempTableMpa.Columns.Add("CostCentre", typeof(string));
            TempTableMpa.Columns.Add("TargetSystem", typeof(int));

            TempTableMpa.Columns.Add("LoadedAtRMCycle", typeof(int));
            TempTableMpa.Columns.Add("MatchingAtRMCycle", typeof(int));
            TempTableMpa.Columns.Add("TerminalId", typeof(string));
            TempTableMpa.Columns.Add("TransType", typeof(int));
            TempTableMpa.Columns.Add("DepCount", typeof(int));

            TempTableMpa.Columns.Add("TransDescr", typeof(string));
            TempTableMpa.Columns.Add("CardNumber", typeof(string));

            TempTableMpa.Columns.Add("IsOwnCard", typeof(bool));

            TempTableMpa.Columns.Add("AccNumber", typeof(string));
            TempTableMpa.Columns.Add("TransCurr", typeof(string));

            TempTableMpa.Columns.Add("TransAmount", typeof(decimal));
            TempTableMpa.Columns.Add("TransDate", typeof(DateTime));
            TempTableMpa.Columns.Add("TraceNoWithNoEndZero", typeof(int));
            TempTableMpa.Columns.Add("AtmTraceNo", typeof(int));

            TempTableMpa.Columns.Add("MasterTraceNo", typeof(int));
            TempTableMpa.Columns.Add("MetaExceptionNo", typeof(int));

            TempTableMpa.Columns.Add("MetaExceptionId", typeof(int));
            TempTableMpa.Columns.Add("RRNumber", typeof(string));

            TempTableMpa.Columns.Add("FuID", typeof(int));

            TempTableMpa.Columns.Add("ResponseCode", typeof(string));
            TempTableMpa.Columns.Add("Operator", typeof(string));

            TempTableMpa.Columns.Add("ReplCycleNo", typeof(int));

            TempTableMpa.Columns.Add("Card_Encrypted", typeof(string));

            TempTableMpa.Columns.Add("TXNSRC", typeof(string));
            TempTableMpa.Columns.Add("TXNDEST", typeof(string));

            TempTableMpa.Columns.Add("ACCEPTOR_ID", typeof(string));
            TempTableMpa.Columns.Add("ACCEPTORNAME", typeof(string));

        }

    
        //
        // Captured Cards 
        //
        RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();
        private void Method_Captured_Cards(string CardNo, string CardCapturedMes)
        {
            // Insert details in Capture cards table 

            Cc.AtmNo = WAtmNo;
            Cc.BankId = WOperator;

            Cc.BranchId = Ac.Branch;
            Cc.SesNo = WSesNo;
            Cc.TraceNo = TraceNo;
            Cc.MasterTraceNo = MasterTraceNo;
            Cc.CardNo = CardNo;
            Cc.CaptDtTm = TRanDate;
            Cc.CaptureCd = 12;
            Cc.ReasonDesc = CardCapturedMes;
            Cc.ActionDtTm = NullFutureDate;
            Cc.CustomerNm = "";
            Cc.ActionComments = "";
            Cc.ActionCode = 0;

            Cc.LoadedAtRMCycle = WLoadedAtRMCycle; 

            Cc.OpenRec = true;

            Cc.Operator = WOperator;

            Cc.InsertCapturedCard(WAtmNo);
        }

        //
        // UPDATE STATS FOR ATM
        //
        private void Method_UPdate_Stats_For_ATM_Trans(string InAtmNo, DateTime InTranDate)
        {
            // Insert details in Capture cards table 

            return; 

            //Ah.ReadTransHistory_Dispensed_Deposited(InAtmNo, InTranDate.Date);
            //if (Ah.RecordFound == true)
            //{
            //    Ah.DrTransactions = Ah.DrTransactions + DrTransactions;
            //    Ah.DispensedAmt = Ah.DispensedAmt + DispensedAmt;

            //    // Over 20 = Deposits 
            //    Ah.CrTransactions = Ah.CrTransactions + CrTransactions;
            //    Ah.DepAmount = Ah.DepAmount + DepAmount;

            //    Ah.UpdateDailyStatsPerAtm(InAtmNo, TRanDate.Date);
            //}
            //else
            //{

            //    Ah.AtmNo = InAtmNo;
            //    Ah.BankId = WOperator;
            //    Ah.Dt = InTranDate.Date;
            //    Ah.LoadedAtRMCycle = InTranDate.Year;

            //    Ah.DrTransactions = 0;
            //    Ah.DispensedAmt = 0;
            //    Ah.CrTransactions = 0;
            //    Ah.DepAmount = 0;

            //    Ah.DrTransactions = Ah.DrTransactions + DrTransactions;
            //    Ah.DispensedAmt = Ah.DispensedAmt + DispensedAmt;

            //    Ah.CrTransactions = Ah.CrTransactions + CrTransactions;
            //    Ah.DepAmount = Ah.DepAmount + DepAmount;

            //    Ah.Operator = WOperator;

            //    Ah.InsertTransHistory_With_Default(InAtmNo, TRanDate.Date);
            //}

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
        // UPDATE Processed = true in Hst Table for this ATM
        // 
        public void UpdatetblHstAtmTxnsProcessedToTrueForThisAtm(string InAtmNo, int InSeqNo)
        {

            Major_ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            //int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                              + " SET Processed = 1 "
                            + " WHERE AtmNo = @AtmNo AND SeqNo <= @SeqNo AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        Count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // UPDATE Processed = true in Hst Table for this ATM and Fuid
        // 
        public void UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(string InAtmNo, int InFuid)
        {

            Major_ErrorFound = false;
            ErrorOutput = "";

            //int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] "
                              + " SET Processed = 1 "
                            + " WHERE AtmNo = @AtmNo AND fuid = @fuid AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@fuid", InFuid);

                        cmd.CommandTimeout = 300; 
                        cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        // PROCEDURE TO READ EJOURNAL FROM ATM AND COPY IT IN DIRECTORY FOR KONTO TO READ
        private void AtmEjournalCopyToDirectory(string InBankId, string InAtmNo, string InIpAddress, int InAction)
        {
            // In 
            // .NET PROCESS 
            // If InAction = 1 = Just copy and do not intialise ATM
            // If InAction = 2 = Copy and Initialise ATM Journal. 
            // Copy To Directory for Konto always
            // If InAction = 2 then copy Backup too. 

            // Process: 1) read journal, 2) insert it in Konto Directory 3) Keep a copy in backup directory
            //           4) Initialise the ATM, 5) Finish. 

            /*
            string sourcePath = @"C:\Journals\TESTING THREE FILES";
            string destinationPath = @"C:\Journals\KONTO";
            string sourceFileName = "EJDATA.LOG.10.1.86.16.20140213.030935.2 - Copy.LOG";
            string destinationFileName = "ATM 1" + " EJDATA.LOG.10.1.86.16.20140213.030935.2.LOG";
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = System.IO.Path.Combine(destinationPath, destinationFileName);

            if (!System.IO.Directory.Exists(destinationPath))
            {
                System.IO.Directory.CreateDirectory(destinationPath);
            }
            System.IO.File.Copy(sourceFile, destinationFile, true);

            //Delete source file
            System.IO.File.Delete(sourcePath + @"\" + sourceFileName);
            */
        }


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

        //
        // Handle Suspects
        //
        public DataTable SuspectDataTable = new DataTable();

        private bool Handle_Suspects(string InAtmNo, int InTraceNoWithNoEndZero,
                                      int InFuId)
        {

            bool SuspectFound = false;
            //
            // Handle Suspects
            //
            SuspectDataTable = new DataTable();
            SuspectDataTable.Clear();

            string SQLString_Susp = " SELECT * "
                                  + " FROM [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis]"
                                  + " WHERE AtmNo = @AtmNo AND FuId = @FuId AND TraceNo = @TraceNo"
                                  + " AND (Suspect = 1 OR Fake = 1) ";

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SQLString_Susp, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FuId", InFuId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNo", InTraceNoWithNoEndZero);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(SuspectDataTable);

                        // Close conn
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            // READ SUSPECT TABLE AND CREATE ERRORS

            if (SuspectDataTable.Rows.Count == 0)
            {
                SuspectFound = false;
            }
            else
            {
                SuspectFound = true;

                // Create the Errors

                int K = 0;

                while (K <= (SuspectDataTable.Rows.Count - 1))
                {
                    // GET Table fields - Line by Line
                    //
                    //                FuId    int Unchecked
                    //AtmNo nvarchar(20)    Unchecked
                    //CardNo  nvarchar(20)    Unchecked
                    //AccNo   nvarchar(30)    Unchecked
                    //TraceNo int Unchecked
                    //TransDateTime   datetime    Unchecked
                    //Currency    nvarchar(10)    Unchecked
                    //FaceValue   int Unchecked
                    //notes   int Checked
                    //Normal  bit Unchecked
                    //Suspect bit Unchecked
                    //Fake    bit Unchecked
                    //SerialNo    nvar Unchecked
                    //SuspDescription nvarchar(150)   Unchecked
                    //ReplCycle   int Unchecked
                    //Processed   bit Unchecked

                    RecordFound = true;

                    TotalValidRecords = TotalValidRecords + 1;

                    string Currency = (string)SuspectDataTable.Rows[K]["Currency"];

                    int FaceValue = (int)SuspectDataTable.Rows[K]["FaceValue"];

                    int notes = (int)SuspectDataTable.Rows[K]["notes"]; // It has the value of 1

                    bool Suspect = (bool)SuspectDataTable.Rows[K]["Suspect"];

                    bool Fake = (bool)SuspectDataTable.Rows[K]["Fake"];

                    string SerialNo_Char = (string)SuspectDataTable.Rows[K]["SerialNo_Char"]; // It has the value of 1

                    string Susp_Fake_Descr = (string)SuspectDataTable.Rows[K]["Susp_Fake_Descr"]; // It has the value of 1

                    K = K + 1;

                }
            }


            return SuspectFound;

        }


       
    }
}

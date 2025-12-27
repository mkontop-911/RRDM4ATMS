using System;
using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM() : base() { }

        public bool RecordFound;
        public bool Major_ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

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

        public DataTable HstAtmTxnsDataTable = new DataTable();

        public DataTable TempTableMpa = new DataTable();

        public DataTable TableAtms = new DataTable();
        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);
        readonly DateTime NullFutureDate = new DateTime(2050, 11, 21);

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
  
        string trandesc;

        int WSaveSesNo;

        int Cassette1;
        int Cassette2;
        int Cassette3;
        int Cassette4;

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

        int WPreviousSesNo;

        int fuid;

        string CardNo;
        string AccNo;

        int PreStartTrxn = 0;
        int PreEndTrxn = 0;
        int PreMasterTraceNo = 0;
        int PreFuid;

        int TransactionType;

        string CurrDesc;
        decimal TranAmount;

        int CommissionCode;
        decimal CommissionAmount;

        int CardTargetSystem;

        string CardCaptured;

        string PresError;

        public int TotalRecords = 0;
        public int TotalTxns = 0;
        public int GrandTotalTxns;

        string SuspectDesc;
        string CardCapturedMes;

        int DrTransactions;
        decimal DispensedAmt;
        int CrTransactions;
        decimal DepAmount;

        int WLoadingCycleNo;

        //DateTime WCut_Off_Date;

        string Result;

        string WSignedId;
        int WSignRecordNo;
        //string WBankId;

        string WOperator;
        int WAtmsReconcGroup;
        string WAtmNo;
        int WFuid; 

        int WMode;

        public void ReadJournal_Txns_And_Insert_In_Pool(string InSignedId, int InSignRecordNo, string InOperator, 
                                                             int InAtmsReconcGroup,string InAtmNo , int InFuid,int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            WAtmsReconcGroup = InAtmsReconcGroup;

            WAtmNo = InAtmNo; 

            WMode = InMode; // 1 Group of ATMs
                            // 2 Single ATM with zero Fuid
                            // 3 Single ATM with Fuid for multithreading
            WFuid = InFuid;// If mode = 3 then this has a value ... it gets a value when called by Alecos after Bambos is called and a Fuid is given.
            string SQLString;

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
            try
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
               // int WReconcCycleNo;

                WLoadingCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                DateTime WCut_Off_Date = Rjc.Cut_Off_Date;

            }
            catch (Exception ex)
            {
               
                CatchDetails(ex);

            }  

            // SHOW MESSAGE 
            string ParId = "719";
            string OccurId = "1";
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
            //// DATA TABLE ROWS DEFINITION 
            //
            // THIS TABLE IS FOR Mpa.
            //
            //TempTableMpa = new DataTable();
            //TempTableMpa.Clear();

            // 
            // AT this point UPDATED KONTO Files are ready to be read by GAS  
            // 

            RecordFound = false;
            Major_ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0;
            TotalTxns = 0;
            MaxSeqNo = 0;
            PreStartTrxn = 0;

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

            try
            {
                // LOOP FOR ATMS

                DateTime StartAtmsDate = DateTime.Now;

                int I = 0;

                while (I <= (TableAtms.Rows.Count - 1))
                {

                    RecordFound = true;

                    WAtmNo = (string)TableAtms.Rows[I]["AtmNo"];

                    Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                    WOperator = Ac.Operator;

                    Rc.ReadReconcCategorybyGroupId(Ac.AtmsReconcGroup);
                    WReconCategoryId = Rc.CategoryId;

                    WCitId = Ac.CitId;

                    Ta.FindNextAndLastReplCycleId(WAtmNo);

                    if (Ta.RecordFound == true)
                    {
                        //ReplCyclefound = true;
                        WSesNo = Ta.LastNo;
                        WPreviousSesNo = Ta.PreSes;
                    }
                    else
                    {
                        WSesNo = 0;

                        //ReplCyclefound = false;
                    }

                    PreStartTrxn = 0;

                    // ********INSERT BY PANICOS**************
                    BeforeCallDtTime = DateTime.Now;
                    // *************************BY PANICOS********************************

                    // Define the data table 

                    HstAtmTxnsDataTable = new DataTable();
                    HstAtmTxnsDataTable.Clear();

                    TempTableMpa = new DataTable();
                    TempTableMpa.Clear();

                    MpaDataTableDefinition();

                    SqlString1_1 = "SELECT SeqNo, atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
                    + "ISNULL(currency, '') AS currency, CAmount,ISNULL(cardnum, '') AS cardnum,"
                    + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
                    + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
                    + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
                    + "ISNULL(PresenterError, '') AS PresError, ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
                    + "ISNULL(Type1, 0) AS Type1, "
                    + "ISNULL(Type2, 0) AS Type2, "
                    + "ISNULL(Type3, 0) AS Type3, "
                    + "ISNULL(Type4, 0) AS Type4, "
                    + " Processed, "
                    + " ISNULL(CardTargetSystem, 0) AS CardTargetSystem, "
                    + " ISNULL(TransactionType, 0) AS TransactionType, "
                    + " ISNULL(CommissionCode, 0) AS CommissionCode, "
                    + " ISNULL(CommissionAmount, 0) AS CommissionAmount "
                        + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "; 
                        //+ " WHERE Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                        //+ " ORDER by AtmNo, TranDate, TranTime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          
                       
                    if (WMode==1 || WMode == 2)
                    {
                        // Covering groups of ATMs
                        SqlString1_2 = 
                         " WHERE AtmNo = @AtmNo AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          

                    }

                    if (WMode == 3)
                    {
                        // Covering only one ATM with known Fuid
                        SqlString1_2 = 
                         "  WHERE AtmNo = @AtmNo AND fuid=@fuid AND Processed = 0 AND TransactionType <> 99 AND Result = 'OK'  "
                         + " ORDER BY TranDate, NCRtrantime , TraceNumber "; // Leave it as is .. this covers if Trace Number restarts from one.                                                          

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
                                //Create a datatable that will be filled with the data retrieved from the command
                                if (WMode==3)
                                {
                                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@fuid", WFuid);
                                }
                                sqlAdapt.Fill(HstAtmTxnsDataTable);

                                // Close conn
                                conn.Close();

                                if (HstAtmTxnsDataTable.Rows.Count == 0)
                                {
                                    I = I + 1;
                                    continue;
                                }

                                // ****************INSERT BY PANICOS**************
                                Message = "READ HST AND INSERT IN MEMORY TABLE..." + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "Read HST", "NBG101", BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************


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

                                    RecordFound = true;

                                    TotalRecords = TotalRecords + 1;

                                    //
                                    // Assign NEW Old
                                    //
                                    //OldATMNo = WAtmNo;

                                    WSeqNo = (int)HstAtmTxnsDataTable.Rows[K]["SeqNo"];

                                    // FIND MAXIMUM SEQUENCE
                                    if (MaxSeqNo < WSeqNo) MaxSeqNo = WSeqNo;

                                    TransactionType = (int)HstAtmTxnsDataTable.Rows[K]["TransactionType"];

                                    if (TraceNo > 0) OldTraceNo = TraceNo;

                                    TraceNo = (int)HstAtmTxnsDataTable.Rows[K]["TraceNumber"];

                                    TraceNoWithNoEndZero = (string)HstAtmTxnsDataTable.Rows[K]["TraceNoWithNoEndZero"];

                                    fuid = (int)HstAtmTxnsDataTable.Rows[K]["fuid"];
                                    trandesc = (string)HstAtmTxnsDataTable.Rows[K]["trandesc"];

                                    Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];
                                    CardNo = (string)HstAtmTxnsDataTable.Rows[K]["cardnum"];

                                    AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct1"];
                                    StartTrxn = (int)HstAtmTxnsDataTable.Rows[K]["starttxn"];
                                    EndTrxn = (int)HstAtmTxnsDataTable.Rows[K]["endtxn"];
                                    //************************************************************
                                    //// Check if this falls within the previous Trace Number 

                                    //THIS CODE CREATES THE MASTER VALUES

                                    if (StartTrxn < PreStartTrxn & fuid == PreFuid)
                                    {
                                        System.Windows.Forms.MessageBox.Show("Wrong Sequence Of "
                                             + "  AtmNo" + WAtmNo + Environment.NewLine
                                            + "  fuid at..." + fuid.ToString() + Environment.NewLine
                                            + "  Pre - Start Txn Rui.." + PreStartTrxn.ToString() + Environment.NewLine
                                            + "  Current - Start ....." + StartTrxn.ToString() + Environment.NewLine
                                            + " OLD Trace ..." + PreMasterTraceNo.ToString() + Environment.NewLine
                                            + " NEW Trace ..." + TraceNo.ToString()
                                            );

                                    }

                                    if (StartTrxn == PreStartTrxn & EndTrxn == PreEndTrxn)
                                    {
                                        MasterTraceNo = PreMasterTraceNo;
                                    }
                                    else MasterTraceNo = TraceNo;

                                    //// Keep the previuos values 
                                    //// 
                                    PreStartTrxn = StartTrxn;
                                    PreEndTrxn = EndTrxn;
                                    PreFuid = fuid;
                                    PreMasterTraceNo = MasterTraceNo;

                                    //Change MasterTraceNo if last digit not equal to zero 

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

                                    TRanDate = (DateTime)HstAtmTxnsDataTable.Rows[K]["TRanDate"];
                                    if (TRanDate == NullDate)
                                    {
                                        TRanDate = DateTime.Now;
                                    }

                                    TimeSpan Time = (TimeSpan)HstAtmTxnsDataTable.Rows[K]["trantime"];
                                    TRanDate = TRanDate.Add(Time);

                                    if (TransactionType < 30 & TransactionType != 0)
                                    {
                                        // These are for money Transactions 
                                        CurrDesc = (string)HstAtmTxnsDataTable.Rows[K]["currency"];
                                        //TEST .... REMOVE IT AFTER WE HAVE JOURNALS WITH PHP 
                                        CurrDesc = Ac.DepCurNm;
                                        TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];

                                        CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];
                                        PresError = (string)HstAtmTxnsDataTable.Rows[K]["PresError"];
                                        SuspectDesc = (string)HstAtmTxnsDataTable.Rows[K]["SuspectDesc"];
                                        CardCapturedMes = (string)HstAtmTxnsDataTable.Rows[K]["CardCapturedMes"];

                                        CommissionCode = (int)HstAtmTxnsDataTable.Rows[K]["CommissionCode"];
                                        CommissionAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["CommissionAmount"];
                                        CardTargetSystem = (int)HstAtmTxnsDataTable.Rows[K]["CardTargetSystem"];

                                        //Jr.UpdateEjTextFromHst(
                                        // fuid,
                                        // StartTrxn,
                                        // EndTrxn,
                                        // CardNo,
                                        // AccNo,
                                        // trandesc,
                                        // CurrDesc,
                                        // TranAmount
                                        //         );

                                    }

                                    if (TransactionType == 77 & trandesc != "SM-START" & trandesc != "SM-END")
                                    {
                                        Cassette1 = (int)HstAtmTxnsDataTable.Rows[K]["Type1"];
                                        Cassette2 = (int)HstAtmTxnsDataTable.Rows[K]["Type2"];
                                        Cassette3 = (int)HstAtmTxnsDataTable.Rows[K]["Type3"];
                                        Cassette4 = (int)HstAtmTxnsDataTable.Rows[K]["Type4"];
                                    }

                                    //*******************************************************
                                    // Handle Supervisor Mode Cases 
                                    //*******************************************************
                                    if (TransactionType == 77)
                                    {
                                        Method_HandlingSupervisorMode(WAtmNo);
                                    }

                                    //********************************************
                                    // PREPARE READ MONEY TRANSACTIONS
                                    //********************************************

                                    if (
                                        (
                                           TransactionType == 11  // trandesc == "WITHDRAWAL" OR trandesc == "???????"
                                        || TransactionType == 23 // trandesc == "DEPOSIT_BNA" // BNA // 
                                        || TransactionType == 24 // trandesc == "DEPOSIT" // Cheque Deposit // 
                                        || TransactionType == 25  //trandesc == "????T?S?" // Envelop Deposit // 
                                        )
                                        & Result.Substring(0, 2) == "OK" & TranAmount > 0)
                                    {

                                        GrandTotalTxns = GrandTotalTxns + 1;

                                        TotalTxns = TotalTxns + 1;

                                        Wtrandesc = trandesc;

                                        // FIND WORKING CATEGORY

                                        WMatchingCategoryId = "";

                                        Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, CardNo.Substring(0, 6), "Our Atms", 12);
                                        if (Mcb.RecordFound)
                                        {
                                            // Own Card
                                            WMatchingCategoryId = Mcb.CategoryId;
                                            OwnCard = true;
                                        }
                                        else
                                        {
                                            // Other Card 
                                            Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
                                            if (Mc.RecordFound == true)
                                            {
                                                WMatchingCategoryId = Mc.CategoryId;
                                            }
                                            OwnCard = false;
                                        }

                                        UniqueRecordId = Gu.GetNextValue();

                                        // FILL Data Table 
                                        DataRow RowSelected = TempTableMpa.NewRow();

                                        RowSelected["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                        RowSelected["OriginalRecordId"] = TraceNo;
                                        RowSelected["UniqueRecordId"] = UniqueRecordId;

                                        RowSelected["MatchingCateg"] = WMatchingCategoryId;

                                        RowSelected["FuID"] = fuid;

                                        Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCategoryId);

                                        RowSelected["FileId01"] = Mcs.SourceFileNameA;
                                        RowSelected["FileId02"] = Mcs.SourceFileNameB;
                                        RowSelected["FileId03"] = Mcs.SourceFileNameC;
                                        RowSelected["FileId04"] = Mcs.SourceFileNameD;
                                        RowSelected["FileId05"] = Mcs.SourceFileNameE;

                                        Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);

                                        RowSelected["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                        RowSelected["Origin"] = Mc.Origin;
                                        RowSelected["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                        RowSelected["Product"] = Mc.Product;
                                        RowSelected["CostCentre"] = Mc.CostCentre;
                                        RowSelected["TargetSystem"] = Mc.TargetSystemId;

                                        decimal DepCount = 0;

                                        if (TransactionType < 20) // "WITHDRAWAL"
                                        {
                                            DepCount = 0;
                                        }

                                        if (TransactionType > 20 & TransactionType < 30)
                                        {
                                            DepCount = TranAmount;
                                        }

                                        RowSelected["LoadedAtRMCycle"] = WLoadingCycleNo;
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

                                        if (PresError == "PRESENTER ERROR" || SuspectDesc == "SUSPECT FOUND")
                                        {
                                            // THERE IS ERROR 
                                            // INSERT ERROR

                                            if (PresError == "PRESENTER ERROR")
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

                                            Ec.CategoryId = WReconCategoryId;

                                            Ec.RMCycle = WLoadingCycleNo;
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

                                        RowSelected["RRNumber"] = 0;
                                        RowSelected["ResponseCode"] = "";
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

                                            RowSelected2["FileId01"] = Mcs.SourceFileNameA;
                                            RowSelected2["FileId02"] = Mcs.SourceFileNameB;
                                            RowSelected2["FileId03"] = Mcs.SourceFileNameC;
                                            RowSelected2["FileId04"] = Mcs.SourceFileNameD;
                                            RowSelected2["FileId05"] = Mcs.SourceFileNameE;

                                            //Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);

                                            RowSelected2["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                            RowSelected2["Origin"] = Mc.Origin;
                                            RowSelected2["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                            RowSelected2["Product"] = Mc.Product;
                                            RowSelected2["CostCentre"] = Mc.CostCentre;
                                            RowSelected2["TargetSystem"] = Mc.TargetSystemId;

                                            RowSelected2["LoadedAtRMCycle"] = WLoadingCycleNo;
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

                                            RowSelected2["RRNumber"] = 0;
                                            RowSelected2["ResponseCode"] = "";
                                            RowSelected2["Operator"] = WOperator;
                                            RowSelected2["ReplCycleNo"] = WSesNo; // WSesNo

                                            TempTableMpa.Rows.Add(RowSelected2);
                                        }

                                    }

                                    // Deal with Captured cards

                                    if (CardCaptured == "CAPTURED CARD") // Trace 10043050
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
                                Message = "PROCESS RECORDS AND CREATE DATA_TABLE FOR ATM..." + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "PROCESS HST", "NBG101", BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************
                                // Update MPA with entries in table 
                                if (TempTableMpa.Rows.Count > 0)
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

                                                s.WriteToServer(TempTableMpa);
                                            }
                                            conn2.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            conn2.Close();

                                            Major_ErrorFound = true;

                                            CatchDetails(ex);

                                            System.Windows.Forms.MessageBox.Show("Cancelled While Inerting Table In Pool");

                                            return;
                                        }
                                }

                                // ****************INSERT BY PANICOS**************
                                Message = "INSERT HST DATA TABLE IN Mpa .. for ATM" + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "INSERT HST IN MPA", "NBG101", BeforeCallDtTime, DateTime.Now, Message);

                                // Initialise for next
                                BeforeCallDtTime = DateTime.Now;
                                // *************************END INSERT BY PANICOS********************************

                                if (HstAtmTxnsDataTable.Rows.Count > 0)
                                {
                                    // 
                                    // UPDATE STATISTICS  FOR DAILY DEBITS AND CREDITS
                                    // 
                                    Method_UPdate_Stats_For_ATM_Trans(WAtmNo, PreTranDate.Date);
                                    //++++++++++++++++++++++++++++++++++   
                                    // Updating as Processed all records read 
                                    //++++++++++++++++++++++++++++++++++
                                    if (WMode ==1 || WMode == 2)
                                    {
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm(WAtmNo, MaxSeqNo);
                                    }
                                    if (WMode == 3)
                                    {
                                        // Based on fuid
                                        UpdatetblHstAtmTxnsProcessedToTrueForThisAtm_And_Fuid(WAtmNo, WFuid);
                                    }

                                }
                                // ****************INSERT BY PANICOS**************
                                Message = "UPDATE HST as process..For ATM.. " + WAtmNo;

                                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "UPDATE HST AS PROCESS", "NBG101", BeforeCallDtTime, DateTime.Now, Message);

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
                    if (TempTableMpa.Rows.Count > 0)
                    {
                        try
                        {

                            // 
                            // We Update the last ATM
                            if (WSesNo > 0)
                            {
                                Method_UpdateOldAtm(WAtmNo);
                            }

                        }
                        catch (Exception ex)
                        {
                            CatchDetails(ex);
                        }
                    }


                    I++; // Read Next ATM entry of the table 

                }

                // ****************INSERT BY PANICOS**************
                Message = "ALL ATMs Loading of HST Txns ";

                Perform.InsertPerformanceTrace(WOperator, WOperator, 1, "Load ALL ATMs", "NBG101", StartAtmsDate, DateTime.Now, Message);


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
            TempTableMpa.Columns.Add("RRNumber", typeof(int));

            TempTableMpa.Columns.Add("FuID", typeof(int));

            TempTableMpa.Columns.Add("ResponseCode", typeof(string));
            TempTableMpa.Columns.Add("Operator", typeof(string));

            TempTableMpa.Columns.Add("ReplCycleNo", typeof(int));
        }

        // UPDATE DATA FOR EACH ATM AT the end of processing 
        private void Method_UpdateOldAtm(string InAtmNo)
        {

            // UPDATE AM and Na AND Ta 
            //*************************************************
            // Current balance, current deposits for AM and Na
            //*************************************************

            if (WSesNo == 0)
            {
                // UN ATM WITHOUT A SUPERVISOR MODE YET 
                return;
            }

            if (Sa.ZeroOccurance == true)
            {
                // Update records that have Zero repl number 
                Mpa.UpdateZeroReplCycleWithNewReplCycle(WAtmNo, Sa.SesNoAfterZero);
                RRDMErrorsClassWithActions Err = new RRDMErrorsClassWithActions();
                RRDM_Cut_Off_LastSeqNumbers Coff = new RRDM_Cut_Off_LastSeqNumbers();
                Err.UpdateAllPresenterErrorsWithNewSessionNumber(WAtmNo, Sa.SesNoAfterZero);
                Coff.UpdateCut_Off_EntriesWhenReplIsZero(WAtmNo, Sa.SesNoAfterZero);
                Cc.UpdateCapturedCardsWhichHadZeroSesNo(WAtmNo, Sa.SesNoAfterZero);

                Sa.ZeroOccurance = false;
            }
            Am.ReadAtmsMainSpecific(InAtmNo);

            Am.LastUpdated = DateTime.Now;

            Am.UpdateAtmsMain(InAtmNo);

            //******************************

            Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);

            if (TraceNo > 0)
            {
                Ta.LastTraceNo = TraceNo;
            }
            else
            {
                Ta.LastTraceNo = OldTraceNo;
            }

            Ta.SesDtTimeEnd = TRanDate;  // Continuously update the SesDtTime end 

            Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

            // Check if this is the next for replenishment
            //
            Ta.FindNextAndLastReplCycleId(WAtmNo);

            int WNextReplNo = Ta.NextReplNo;

            // Update Atms main with latest information 
            //
            UpdatedAtmsMain(InAtmNo, WNextReplNo);

        }

        // UPDATE DATA FOR EACH ATMs 
        public void UpdatedAtmsMain(string InAtmNo, int InReplCycleNo)
        {
            decimal OpeningBalance;
            //decimal ReplTernOver;

            Am.ReadAtmsMainSpecific(InAtmNo);

            Na.ReadSessionsNotesAndValues(InAtmNo, Am.CurrentSesNo, 2);
            OpeningBalance = Na.Balances1.OpenBal;
            //ReplTernOver = Na.Balances1.ReplToRepl;

            // UPDATE AM and Na AND Ta 
            //*************************************************
            // Current balance, current deposits for AM and Na

            Ec.ReadAllErrorsTableForCounters(WOperator, "", InAtmNo, Am.CurrentSesNo, "");
            Am.ErrOutstanding = Ec.NumOfErrors;
            Am.ProcessMode = Ta.ProcessMode;
            //Am.CurrCassettes = OpeningBalance - ReplTernOver;

            Da.ReadDepositsTotals(InAtmNo, Am.CurrentSesNo); // Find Totals from deposit transactions 

            Da.UpdateDepositsNaWithMachineTotals(InAtmNo, Am.CurrentSesNo); // Update Totals 

            Da.ReadDepositsSessionsNotesAndValuesDeposits(InAtmNo, Am.CurrentSesNo);

            Am.CurrentDeposits = Da.DepositsMachine1.Amount + Da.DepositsMachine1.EnvAmount;

            Na.ReadSessionsNotesAndValues(InAtmNo, Am.CurrentSesNo, 2);

            Am.CurrCassettes = Na.Balances1.MachineBal;

            Am.LastUpdated = DateTime.Now;

            if (InReplCycleNo > 0) Am.ReplCycleNo = InReplCycleNo;

            Am.UpdateAtmsMain(InAtmNo);

            //******************************

            Ta.ReadSessionsStatusTraces(InAtmNo, Am.CurrentSesNo);

            //Ta.LastTraceNo = TraceNo;

            Ta.Stats1.NoOfTranCash = Ta.Stats1.NoOfTranCash + TotalTxns;

            Ta.Stats1.NoOfTranDepCash = Ta.Stats1.NoOfTranDepCash + Da.DepositsMachine1.Trans;

            Ta.Stats1.NoOfTranDepCheq = Ta.Stats1.NoOfTranDepCheq + Da.ChequesMachine1.Trans;

            Ta.SesDtTimeEnd = TRanDate;  // Continuously update the SesDtTime end 

            Ta.UpdateSessionsStatusTraces(InAtmNo, Am.CurrentSesNo);

            //TEST
            DateTime WDtTm = DateTime.Today;
            //WDtTm = new DateTime(2014, 02, 14);

            if (Am.LastDispensedHistor < WDtTm)
            {
                // Dispensed History 

                RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
                // UPDATE TRANS HISTORY RECORD 

                //Tc.ReadUpdateTransForDispensedHistory(WOperator, InAtmNo, Am.LastDispensedHistor, WDtTm);

                if (Tc.RecordFound == true)
                {
                    Am.ReadAtmsMainSpecific(InAtmNo);
                    Am.LastDispensedHistor = WDtTm;
                    Am.UpdateAtmsMain(InAtmNo);
                }

            }

            TotalTxns = 0;
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

        //
        // Handling Supervisor Mode Cases
        //

        private void Method_HandlingSupervisorMode(string WAtmNo)
        {
            //*************************************************************
            // DEAL WITH NON TRANSACTIONS 
            //*************************************************************

            // SUPERVISOR MODE STARTS - THE OLD REPL CYCLE ENDS
            if (trandesc == "SM-START" & TransactionType == 77) // SUPERVISOR MODE START = THIS IS THE END OF REPL CYCLE 
            {
                Message = "Start Replenishment For ATM : " + WAtmNo + " Done on : " + TRanDate.ToString();

                Perform.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", WMatchingCategoryId, BeforeCallDtTime, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

                Method_SupervisorMode_Start(WAtmNo);
            }

            if (trandesc == "SM-CASSETTE") // Cassette equivalent Gas Remaining = What it is in cassettes 
            {
                // HERE WE NEED DATE AND TIME 
                Method_SupervisorMode_CASSETTE(WAtmNo);
            }

            if (trandesc == "SM-REJECTED") // Rejected
            {
                Method_SupervisorMode_REJECTED(WAtmNo);
            }

            if (trandesc == "SM-REMAINING") // Cash OUT FROM MACHINE ( CASSETTE + REJECTED )
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

            if (trandesc == "SM-DISPENCED") // Dispensed
            {
                Method_SupervisorMode_DISPENCED(WAtmNo);
            }

            // Totals Line 
            if (trandesc == "SM-TOTALS")
            {
                if (IsNewATM == true)
                {
                    // If ATM is new we calculate Open Balance as per below 
                    Ac.ReadAtm(WAtmNo);

                    OpenBalance = Cassette1 * Ac.FaceValue_11
                                 + Cassette2 * Ac.FaceValue_12
                                 + Cassette3 * Ac.FaceValue_13
                                 + Cassette4 * Ac.FaceValue_14;

                    // Update only for New ATM Money in cassettes and update Na 

                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2); // NEW Session

                    if (IsNewATM == true) Na.IsNewAtm = true;

                    Na.Cassettes_1.InNotes = Cassette1;
                    Na.Cassettes_2.InNotes = Cassette2;
                    Na.Cassettes_3.InNotes = Cassette3;
                    Na.Cassettes_4.InNotes = Cassette4;

                    Na.ReplAmountTotal = OpenBalance;
                    Na.InReplAmount = OpenBalance;

                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo); // NEW SESSION 

                }
            }

            // Supervisor Mode Cash Added 
            if (trandesc == "SM-CASH ADDED") // Cash ADDED 
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

                    Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 
                                                                                                       // => NEW cycle
                    WSesNo = Sa.NewSessionNo;
                }

                // Method_SupervisorMode_CASHADDED(WAtmNo);

            }

            // SUPERVISOR MODE ENDS - A NEW REPL CYCLE of Transactions STARTS
            if (trandesc == "SM-END" & TransactionType == 77) // YESTERDAYS SM 
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

        }

        decimal OpenBalance;
        decimal Dispensed;
        decimal Rejected;
        decimal CashInCassettes;
        decimal CashLoaded; // Loaded 
        bool IsNewATM;
        DateTime SMStart;
        int SMStartTraceNo;

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

            Ta.FindNextAndLastReplCycleId(WAtmNo);

            if (Ta.RecordFound == true)
            {
                //ReplCyclefound = true;
                WSesNo = Ta.LastNo;
                WPreviousSesNo = Ta.PreSes;
            }
            else
            {
                WSesNo = 0;

                //ReplCyclefound = false;
            }

            SMStart = TRanDate;
            SMStartTraceNo = TraceNo;

            if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no SesNo No 
            {
                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                OpenBalance = Na.Cassettes_1.InNotes * Na.Cassettes_1.FaceValue
                    + Na.Cassettes_2.InNotes * Na.Cassettes_2.FaceValue
                    + Na.Cassettes_3.InNotes * Na.Cassettes_3.FaceValue
                    + Na.Cassettes_4.InNotes * Na.Cassettes_4.FaceValue;
            }
        }

        // Supervisor Mode Cash Dispensed 
        private void Method_SupervisorMode_DISPENCED(string InAtmNo)
        {
            // HERE WE NEED SesNo
            if (WSesNo == 0)
            {
                IsNewATM = true;
                // If it is equal to zero it means this is the first time and there is no SesNo No 
                // MAKE ATM ACTIVE 
                Ac.ReadAtm(WAtmNo);
                Ac.ActiveAtm = true;
                Ac.UpdateAtmsBasic(WAtmNo);

                // Create a pseaudo cycle  

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 
                                                                                                   // => NEW cycle
                WSesNo = Sa.NewSessionNo;

            }
            // HERE WE NEED SesNo

            // Update Replenishment Start 
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            Ta.Repl1.ReplStartDtTm = SMStart;
            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            //Ta.Repl1.ReplFinDtTm = InTRanDate;
            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

            if (IsNewATM == true) Na.IsNewAtm = true;

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
            if (WSesNo == 0)
            {
                IsNewATM = true;
                // If it is equal to zero it means this is the first time and there is no SesNo No 
                // MAKE ATM ACTIVE 
                Ac.ReadAtm(WAtmNo);
                Ac.ActiveAtm = true;
                Ac.UpdateAtmsBasic(WAtmNo);

                // Create a pseaudo cycle  

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 
                                                                                                   // => NEW cycle
                WSesNo = Sa.NewSessionNo;
            }
            // HERE WE NEED SesNo

            if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no SesNo No 
            {
                Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                if (IsNewATM == true) Na.IsNewAtm = true;

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
            if (WSesNo == 0)
            {
                IsNewATM = true;

                // If it is equal to zero it means this is the first time and there is no SesNo No 
                // MAKE ATM ACTIVE 
                Ac.ReadAtm(WAtmNo);
                Ac.ActiveAtm = true;
                Ac.UpdateAtmsBasic(WAtmNo);

                // Create a pseaudo cycle  

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, SMStartTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 
                                                                                                   // => NEW cycle
                WSesNo = Sa.NewSessionNo;
            }


            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

            if (IsNewATM == true) Na.IsNewAtm = true;

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

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2); // NEW Session

            //if (IsNewATM == true) Na.IsNewAtm = true;

            Na.Cassettes_1.InNotes = Cassette1;
            Na.Cassettes_2.InNotes = Cassette2;
            Na.Cassettes_3.InNotes = Cassette3;
            Na.Cassettes_4.InNotes = Cassette4;

            Na.ReplAmountTotal = Na.InReplAmount = InCashLoaded;
            //= Na.Cassettes_1.InNotes * Na.Cassettes_1.FaceValue
            // + Na.Cassettes_2.InNotes * Na.Cassettes_2.FaceValue
            // + Na.Cassettes_3.InNotes * Na.Cassettes_3.FaceValue
            // + Na.Cassettes_4.InNotes * Na.Cassettes_4.FaceValue;

            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo); // NEW SESSION 

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);
            // UPDATE Am
            Am.ReadAtmsMainSpecific(InAtmNo);

            Am.CurrCassettes = Na.Balances1.MachineBal;
            Am.CurrentDeposits = 0;

            Am.UpdateAtmsMain(InAtmNo);
            //
            // INSERT TRANSACTION WITH CASH ADDED
            // 
            // Build amount

            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

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
                  || (CashLoaded == 0 & IsNewATM == false) // Old ATM Money Unloaded and no New Loaded (ATM was set out of service)
                  || (CashLoaded == 0 & IsNewATM == true) // New but no Cash Loaded     //(CashLoaded > 0 & OpenBalance > 0 & IsNewATM == false)
                                                          // || (CashLoaded > 0 & OpenBalance > 0 & IsNewATM == true) // New ATM
            )
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
                    Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
                    Na.ReplAmountTotal = OpenBalance;
                    Na.UpdateSessionsNotesAndValues(WAtmNo, WSesNo);

                    // Update AM 
                    Am.ReadAtmsMainSpecific(WAtmNo);

                    Am.ReplCycleNo = WSesNo;

                    Am.LastReplDt = TRanDate;

                    Am.UpdateAtmsMain(WAtmNo);

                }
                else
                {
                    // Old ATM 
                    Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                }

                Ta.LastTraceNo = TraceNo - 9 + 6;

                // Stats of Session

                Ta.Stats1.NoOfTranCash = Ta.Stats1.NoOfTranCash + TotalTxns;

                Ta.Stats1.NoOfTranDepCash = Ta.Stats1.NoOfTranDepCash + Da.DepositsMachine1.Trans;

                Ta.Stats1.NoOfTranDepCheq = Ta.Stats1.NoOfTranDepCheq + Da.ChequesMachine1.Trans;

                Ta.SesDtTimeEnd = TRanDate;  // Continuously update the SesDtTime end 

                // END OF REPLENISHMENT 

                Ta.Repl1.FinishRepl = true;

                Ta.Repl1.ReplFinDtTm = TRanDate;

                Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                //
                // UPDATE Action TABLE WITH ACTUAL  MONEY Loaded
                // Old SessionNo 
                // 
                Ro.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSesNo);

                if (Ro.RecordFound)
                {
                    // ,[PassReplCycle]
                    //,[PassReplCycleDate]
                    //,[CashInAmount]
                    //,[InMoneyReal]
                    Ro.PassReplCycle = true;
                    Ro.PassReplCycleDate = Ta.Repl1.ReplFinDtTm; 
                    Ro.InMoneyReal = CashLoaded; // from ATMs Registers

                    Ro.ActiveRecord = false;

                    Ro.UpdateReplActionsForAtm(WAtmNo, Ro.ReplOrderNo);
                }
               
              
                //**************************************************************************************************
                //Insert Basic Info For G4S RECORD - SESSION UNDER COMPLETION 
                //************************************************************************************************** 
                int G4Record_ATM;
                int G4Record_CDM = 0;
                bool IsDeposit = false;

                G4Record_ATM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadingCycleNo, IsDeposit);  // Create G4S Record

                if (DepositsDone == true)
                {
                    IsDeposit = true;
                    G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadingCycleNo, IsDeposit);  // Create G4S Record
                }

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
                             WSesNo, G4.UnloadedMachine, G4.Cash_Loaded,
                            TRanDate, WLoadingCycleNo,
                                          IsDeposit);  // Post the Txns
                    }

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

                            G4.Deposits = TotalDeposits;

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
                                     WSesNo, G4.UnloadedMachine, G4.Cash_Loaded,
                                    TRanDate, WLoadingCycleNo,
                                                  IsDeposit);  // Post the Txns
                            }
                        }
                    }

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

                    if (CashLoaded > 0)
                    {
                        Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 

                        //CurrentSessionNo = Sa.NewSessionNo;
                        WSesNo = Sa.NewSessionNo;

                        // Update Session Notes Record

                        Method_SupervisorMode_CASHADDED_To_New_SessionNo(WAtmNo, CashLoaded);

                    }

                    Message = "END Replenishment For ATM : " + WAtmNo + " Done on : " + TRanDate.ToString();

                    Perform.InsertPerformanceTrace(WOperator, WOperator, 2, "LoadTrans", WMatchingCategoryId, BeforeCallDtTime, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                    //ReplCyclefound = true;
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

            Rs.InsertInAtmsStatsProcess(WOperator, InAtmNo, WSesNo,0);
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
                    G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadingCycleNo, IsDeposit);  // Create G4S Record
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

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 

                //CurrentSessionNo = Sa.NewSessionNo;

                WSesNo = Sa.NewSessionNo;
                ////*********************************************
                //// Call cash Added for New Session
                ////******************************************** 
                //Method_SupervisorMode_CASHADDED(WAtmNo);

                //ReplCyclefound = true;
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

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 

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
                G4Record_CDM = Insert_G4S_Record(WAtmNo, WSesNo, TRanDate, WLoadingCycleNo, IsDeposit);  // Create G4S Record

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

                Sa.CreateNewSession(WAtmNo, WSignedId, TRanDate, OldTraceNo, WLoadingCycleNo); // The necessary records are created and the new Session No is available 

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
        private int Insert_G4S_Record(string InAtmNo, int InSessionNo, DateTime InDate, int InCycleNo, bool InIsDeposit)
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
            G4.OrderNo = 0 ;
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
                Ec.ReadAllErrorsTableForCounters(WOperator, "", InAtmNo, 0, "");
            }

            G4.PresentedErrors = Ec.TotalErrorsAmtLess100;

            G4.ReplCycleNo = InSessionNo;
            G4.RemarksG4S = "";
            G4.Operator = Ac.Operator;
            Ta.ReadSessionsStatusTraces(WAtmNo, InSessionNo);
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            Rjc.Find_GL_Cut_Off_Before_GivenDate(Ta.Operator, Ta.SesDtTimeEnd.Date);
            if (Rjc.RecordFound == true & Rjc.Counter == 0)
            {
                // Cut off of prvious to Replenishment 
                G4.Cut_Off_date = Rjc.Cut_Off_Date;
                //WReconcCycleNo = Rjc.JobCycle;
                //IsDataFound = true;
            }
            else
            {
                G4.Cut_Off_date = NullPastDate;
                //IsDataFound = false;
            }

            G4.Gl_Balance_At_CutOff = 0;

            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);
          
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
            G4.Un_Load_FaceValue_2 = decimal.ToInt32(Na.Cassettes_1.FaceValue);
            G4.Un_Load_Cassette_2 = Na.Cassettes_2.RemNotes + Na.Cassettes_2.RejNotes;
            G4.Un_Load_FaceValue_3 = decimal.ToInt32(Na.Cassettes_1.FaceValue);
            G4.Un_Load_Cassette_3 = Na.Cassettes_3.RemNotes + Na.Cassettes_3.RejNotes;
            G4.Un_Load_FaceValue_4 = decimal.ToInt32(Na.Cassettes_1.FaceValue);
            G4.Un_Load_Cassette_4 = Na.Cassettes_4.RemNotes + Na.Cassettes_4.RejNotes;

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
            Na.ReadSessionsNotesAndValues(InAtmNo, InSessionNo, 2); 
            
            string BankCashAcc;
            string CitCashAcc; 
           
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
              
            }
            else
            {
                CitCashAcc = "Not Defined";
            }

         
            RRDMPostedTrans Pt = new RRDMPostedTrans();

            // DO A Pair for UnLoaded
            if (InUnLoaded > 0 )
            {
                // First Entry 
                // CR CIT CASH
                //
                Pt.TranToBePostedKey = 9999;
                Pt.Origin = "UnLoaded By Cit";
                Pt.UserId = WSignedId;
                Pt.AccNo = CitCashAcc;
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 11; // DR CIT Cash 
                Pt.TransDesc = "CIT has Unloaded Money from ATM";
                //TEST
                Pt.CurrDesc = CurrDesc;
                Pt.TranAmount = InUnLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

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
                Pt.CurrDesc = CurrDesc;
                Pt.TranAmount = InUnLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

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
                Pt.AccNo = CitCashAcc;
                Pt.AtmNo = InAtmNo;
                Pt.ReplCycle = InSessionNo;
                Pt.BankId = WOperator;

                Pt.TranDtTime = InDate;
                Pt.TransType = 21; // CR CIT Cash 
                Pt.TransDesc = "CIT has loaded Money to ATM";
                //TEST
                Pt.CurrDesc = CurrDesc;
                Pt.TranAmount = InLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

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
                Pt.CurrDesc = CurrDesc;
                Pt.TranAmount = InLoaded;
                Pt.ValueDate = InDate;
                Pt.OpenRecord = true;

                Pt.Operator = WOperator;

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
        // UPDATE Processed = true in Hst Table for this ATM
        // 
        public void UpdatetblHstAtmTxnsProcessedToTrueForThisAtm(string InAtmNo, int InSeqNo)
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
                        new SqlCommand("UPDATE [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
                              + " SET Processed = 1 "
                            + " WHERE AtmNo = @AtmNo AND SeqNo <= @SeqNo AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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
                        new SqlCommand("UPDATE [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
                              + " SET Processed = 1 "
                            + " WHERE AtmNo = @AtmNo AND fuid = @fuid AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@fuid", InFuid);

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
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
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



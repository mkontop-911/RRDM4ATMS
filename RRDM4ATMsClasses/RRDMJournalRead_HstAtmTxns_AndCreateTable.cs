using System;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable() : base() { }

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMGasParameters Gp = new RRDMGasParameters();
        int UniqueRecordId;

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMSessionsNewSession Sa = new RRDMSessionsNewSession(); // NEW SESSION CLASS 
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMCashInOut Ct = new RRDMCashInOut();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        //    E_JournalTxtClass Ejc = new E_JournalTxtClass(); 

        //RRDMErrorsFromKontoToMatchingClass Em = new RRDMErrorsFromKontoToMatchingClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass();

        public DataTable HstAtmTxnsDataTable = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        int WSaveSesNo;

        string WCitId;
        string WOperator;

        int WLastTraceNo;
        int WSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WBankId;

        string WAtmNo;
        int WMode;

        public void ReadAndCreateTables(string InSignedId, int InSignRecordNo, string InBankId,
            string InAtmNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WBankId = InBankId;

            WAtmNo = InAtmNo;
            WMode = InMode;

            // Read transactions from EJournal file
            // 1) Update InPool Trans
            // 2) Update Captured Cards
            // 3) Update cassettes
            // 4) Update Fist Last Trace or maybe not? Yes it is better here than in replenishment workflow 

            int I = 0;

            DateTime SmStartDtTime;
            DateTime SmEndDtTime;

            DateTime TRanDate;

            int TraceNo;

            int fuid;
            string trandesc;

            string CardNo;
            string AccNo;

            int StartTrxn;
            int EndTrxn;

            int PreStartTrxn = 0;
            int PreEndTrxn = 0;
            int PreMasterTraceNo = 0;

            //int EJournalTraceNo;

            int MasterTraceNo;

            //int Reconciled;
            int TransactionType;

            string CurrDesc;
            decimal TranAmount;

            int CommissionCode;
            decimal CommissionAmount;

            int CardTargetSystem;
            string RMCateg;

            string CardCaptured;

            string PresError;

            string SuspectDesc;
            string CardCapturedMes;

            string Result;

            int Cassette1;
            int Cassette2;
            int Cassette3;
            int Cassette4;

            int CurrentSessionNo;

            string SqlString1 = "";
            string SqlString2 = ""; 

            DateTime NullDate = new DateTime(1900, 01, 01);

            Ac.ReadAtm(InAtmNo); // Read Information for ATM 

            WOperator = Ac.Operator;

            WCitId = Ac.CitId;

            bool KontoReady = true;

            if (KontoReady == true & WAtmNo == "AB104" & WSignedId == "501") // DO THIS FOR TESTING USER 
            {
                //TEST

                // STEP A . Copy from ATM EJournal from ATM directory to Konto Directory

                int WAction = 1;

                string WIpAddress = "123432";

                AtmEjournalCopyToDirectory(WBankId, WAtmNo, WIpAddress, WAction); // Copy ATM file

                //TEST 
                // STEP B . Procedures reads EJournal from Konto directory and parse it. 
                // CALL Test procedures
                string JournalType = "NCR01";

                //TEST
                // 1. READ First Journal - KONTO 1
                string PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.1.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);

                // 2.... Create File for host - KONTO 2

                // 3.... Create Error Conditions ( One double at Hosst and one missing) - KONTO 3 

                // 4.... Create Host file with compltete trace number KONTO 4

                // 5.....Check results 
                //TESTING
                //MessageBox.Show("Check results for HOST FILE CREATION"); 

                // 6...... Matching Files 
                // CALL PROCEDURE FOR MATCHING THE FILES - HOST AND ATMS - KONTO 5 
                StoreProcKontoMatchingOfFiles(WBankId, WAtmNo);

                //TESTING
                //MessageBox.Show("Check Errors File Status, Host last trace numbers and Errors created"); 

                // 7...... Continue with the rest  

                //TEST
                PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.2.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);
                //TEST
                PathName = @"C:\ATMBackUps\KONTO\EJDATA.LOG.10.1.86.16.20140213.030935.3.LOG";
                StoreProcKontoEJournal(WBankId, WAtmNo, JournalType, PathName);

                KontoReady = false;

                //MessageBox.Show("YOU are Testing User 501. NOW RUN MANUALLY THE PROCEDURE START"); 
            }

            bool ReplCyclefound = false;

            // STEP C : Update In Pool with transactions 

            // FIND LATEST REPLCYCLE AND LAST read Trace Number 

            Ta.FindNextAndLastReplCycleId(WAtmNo);

            if (Ta.RecordFound == true)
            {
                ReplCyclefound = true;
                WSesNo = Ta.LastNo;
                WLastTraceNo = Ta.LastTraceNo;
                //TEST          
            }
            else
            {
                // THIS IS THE FIRST TIME ATM WAS INSTALLED IN GAS 
                WSesNo = 0;
                WLastTraceNo = 0;

                RecordFound = false;
                ErrorFound = false;
                ErrorOutput = "";

                bool First = true;

                if (WAtmNo == "AtmArodes")
                {
                    SqlString1 = "SELECT atmno, TraceNumber, ISNULL(trandesc, '') AS trandesc,"
                                  + "ISNULL(Result, '') AS Result,TransactionType"
                                  + " FROM [ATMS_Journals_Arodes].[dbo].[tblHstAtmTxns] "
                                  + " WHERE atmno = @atmno AND trandesc = 'SM-CASH ADDED'"
                                   + " order by TraceNumber";

                }
                else
                {
                    SqlString1 = "SELECT atmno, TraceNumber, ISNULL(trandesc, '') AS trandesc,"
                                  + "ISNULL(Result, '') AS Result,TransactionType"
                                  + " FROM [ATMS_Journals].[dbo].[tblHstAtmTxns] "
                                  + " WHERE atmno = @atmno AND trandesc = 'SM-CASH ADDED'"
                                   + " order by TraceNumber";
                }

                using (SqlConnection conn =
                                       new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString1, conn))
                        {
                            cmd.Parameters.AddWithValue("@atmno", WAtmNo);
                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                if (First == true)
                                {
                                    TraceNo = Convert.ToInt32(rdr["TraceNumber"]);
                                    WLastTraceNo = TraceNo - 1; // Cash In has value in last digit therefore - 10
                                    First = false;
                                }

                            }

                            // Close Reader
                            rdr.Close();
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
            // AT this point UPDATED KONTO Files are ready to be read by GAS  
            // 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Define the data table 

            HstAtmTxnsDataTable = new DataTable();
            HstAtmTxnsDataTable.Clear();

            if (WAtmNo == "AtmArodes")
            {
             SqlString2 = "SELECT atmno, TraceNumber, ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
             + "ISNULL(currency, '') AS currency, CAmount,ISNULL(cardnum, '') AS cardnum,"
             + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
             + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
             + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
             + "ISNULL(PresenterError, '') AS PresError, ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
             + "Type1, type2, type3, type4, Processed, CardTargetSystem,  TransactionType,"
             + "CommissionCode, CommissionAmount"
                 + " FROM [ATMS_Journals_Arodes].[dbo].[tblHstAtmTxns] "
                 + " WHERE atmno = @atmno AND TraceNumber>@LastTraceNo"
                 + " ORDER by TraceNumber";
            }
            else
            {
            SqlString2 = "SELECT atmno, TraceNumber, ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
              + "ISNULL(currency, '') AS currency, CAmount,ISNULL(cardnum, '') AS cardnum,"
              + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
              + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
              + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
              + "ISNULL(PresenterError, '') AS PresError, ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
              + "Type1, type2, type3, type4, Processed, CardTargetSystem,  TransactionType,"
              + "CommissionCode, CommissionAmount"
                  + " FROM [ATMS_Journals].[dbo].[tblHstAtmTxns] "
                  + " WHERE atmno = @atmno AND TraceNumber>@LastTraceNo"
                  + " ORDER by TraceNumber";
            }

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@atmno", WAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@LastTraceNo", WLastTraceNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(HstAtmTxnsDataTable);

                        // Close conn
                        conn.Close();

                    if (HstAtmTxnsDataTable.Rows.Count > 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Loading of records from Journal. PHP and Euro * 10 is used. To be removed after testing. ");
                    } 

                    int K = 0;

                    while (K <= (HstAtmTxnsDataTable.Rows.Count - 1))
                    {
                        // GET Table fields - Line by Line
                        //
                        RecordFound = true;

                        TraceNo = (int)HstAtmTxnsDataTable.Rows[K]["TraceNumber"];
                        fuid = (int)HstAtmTxnsDataTable.Rows[K]["fuid"];
                        trandesc = (string)HstAtmTxnsDataTable.Rows[K]["trandesc"];

                        if (trandesc == "ΚΑΤΑΘΕΣΗ")
                        {
                            trandesc = "DEP CHEQUES";
                        }
                        if (trandesc == "DEPOSIT")
                        {
                            trandesc = "DEP CHEQUES";
                        }
                        Result = (string)HstAtmTxnsDataTable.Rows[K]["Result"];
                        CardNo = (string)HstAtmTxnsDataTable.Rows[K]["cardnum"];

                        AccNo = (string)HstAtmTxnsDataTable.Rows[K]["acct1"];
                        StartTrxn = (int)HstAtmTxnsDataTable.Rows[K]["starttxn"];
                        EndTrxn = (int)HstAtmTxnsDataTable.Rows[K]["endtxn"];
                        //************************************************************
                        //// Check if this falls within the previous Trace Number 
                        //THIS CODE CREATES THE MASTER VALUES
                        if (StartTrxn == PreStartTrxn & EndTrxn == PreEndTrxn)
                        {
                            MasterTraceNo = PreMasterTraceNo;
                        }
                        else MasterTraceNo = TraceNo;

                        //// Keep the previuos values 
                        //// 
                        PreStartTrxn = StartTrxn;
                        PreEndTrxn = EndTrxn;
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

                        TransactionType = (int)HstAtmTxnsDataTable.Rows[K]["TransactionType"];
                        TRanDate = (DateTime)HstAtmTxnsDataTable.Rows[K]["TRanDate"];
                        if (TRanDate == NullDate)
                        {
                            TRanDate = DateTime.Now;
                        }
                        TimeSpan Time = (TimeSpan)HstAtmTxnsDataTable.Rows[K]["trantime"];
                        TRanDate = TRanDate.Add(Time);

                        CurrDesc = (string)HstAtmTxnsDataTable.Rows[K]["currency"];
                        //TEST .... REMOVE IT AFTER WE HAVE JOURNALS WITH PHP 
                        CurrDesc = Ac.DepCurNm; 
                        TranAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["camount"];
                        //TEST 
                        TranAmount = TranAmount * 10 ; 

                        CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];
                        PresError = (string)HstAtmTxnsDataTable.Rows[K]["PresError"];
                        SuspectDesc = (string)HstAtmTxnsDataTable.Rows[K]["SuspectDesc"];
                        CardCapturedMes = (string)HstAtmTxnsDataTable.Rows[K]["CardCapturedMes"];

                        CommissionCode = (int)HstAtmTxnsDataTable.Rows[K]["CommissionCode"];
                        CommissionAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["CommissionAmount"];
                        CardTargetSystem = (int)HstAtmTxnsDataTable.Rows[K]["CardTargetSystem"];

                        //TEST
                        if (CardTargetSystem == 1)
                        {
                            RMCateg = "EWB101"; // This goes to BancNet
                        }
                        else
                        {
                            RMCateg = "EWB102"; // This goes to T24
                        }

                        Cassette1 = (int)HstAtmTxnsDataTable.Rows[K]["Type1"];
                        Cassette2 = (int)HstAtmTxnsDataTable.Rows[K]["Type2"];
                        Cassette3 = (int)HstAtmTxnsDataTable.Rows[K]["Type3"];
                        Cassette4 = (int)HstAtmTxnsDataTable.Rows[K]["Type4"];

                        // CHECK FIRST RECORD OF JOURNAL 

                        if ((ReplCyclefound == false & trandesc == "SM-CASH ADDED") // CASE OF NEW ATM => Create the FIRST  Repl Cycle record 
                             || (ReplCyclefound == true & trandesc == "SM-CASH ADDED")) // CASE OF Creating a New Repl Cycle 
                        {

                            // At this point create a new Repl Cycle ( NEW SESSION)
                            // with process mode = -1
                            // Set UP the WSesNo
                            // Update first trace number 

                            if (ReplCyclefound == false & trandesc == "SM-CASH ADDED") // MAKE ATM ACTIVE 
                            {
                                Ac.ReadAtm(WAtmNo);
                                Ac.ActiveAtm = true;
                                Ac.UpdateAtmsBasic(WAtmNo);
                            }

                            WSaveSesNo = WSesNo;

                            Sa.CreateNewSession(WAtmNo, WSignedId,TRanDate, 0 ,0); // The necessary records are created and the new Session No is available 

                            CurrentSessionNo = Sa.NewSessionNo;

                            WSesNo = Sa.NewSessionNo;

                            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
                            Ta.FirstTraceNo = TraceNo;
                            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);

                            ReplCyclefound = true;

                        }

                        // Deal with Captured cards

                        if (CardCaptured == "CAPTURED CARD") // Trace 10043050
                        {
                            // Insert details in Capture cards table 
                            RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();

                            Cc.AtmNo = WAtmNo;
                            Cc.BankId = WBankId;

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

                        // AT THIS POINT WE HAVE A Repl Cycle record 
                        // We continue reading and processing the Transactions (DR and CR) 

                        Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                        Ta.LastTraceNo = TraceNo;

                        if (trandesc == "WITHDRAWAL" & Result.Substring(0, 2) == "OK")
                        {
                            Ta.SesDtTimeEnd = TRanDate;  // Continuously update the SesDtTime end 
                        }
                        Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                        if (trandesc == "SM-TOTALS")
                        {
                            //  SessionEnd = true; // Last digit = 6 

                            // AT this point Deposits must update Notes and values
                            RRDMDepositsClass Da = new RRDMDepositsClass();

                            Da.ReadDepositsTotals(WAtmNo, WSesNo); // Find Totals 
                            Da.UpdateDepositsNaWithMachineTotals(WAtmNo, WSesNo); // Update Totals 

                            // Close current Repl Cycle and continue process 
                            // Turn status to 0 = ready for Repl Cycle Workflow 
                            Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                            Ta.ProcessMode = 0; // from -1 we have made it 0 : Repl Cycle Record is ready for Repl Cycle workfow 
                            Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);
                        }

                        if ((trandesc == "WITHDRAWAL"
                            || trandesc == "DEPOSIT_BNA" || trandesc == "DEPOSIT" || trandesc == "DEP CHEQUES")
                            & Result.Substring(0, 2) == "OK")
                        {
                            // MAP THE INPUT
                            I = I + 1;

                            Ac.ReadAtm(WAtmNo);
                            UniqueRecordId = GetNextValue(connectionString);

                            Mpa.OriginFileName = "ATM:" + WAtmNo + " Journal";
                            Mpa.OriginalRecordId = TraceNo;
                            Mpa.UniqueRecordId = UniqueRecordId;

                            Gp.ReadParametersSpecificOccuranceFromRelated(Ac.BankId, "430", "400", CardNo.Substring(0, 6)); 
                      
                            if (Gp.RecordFound == true)
                            {
                                Mpa.TargetSystem = int.Parse(Gp.OccuranceId); 

                                //string SelectionCriteria = " WHERE Origin ='Our Atms' AND TargetSystemId = " 
                                //                         + Mpa.TargetSystem;

                                Mc.ReadMatchingCategoryBySelectionCriteria(Mpa.MatchingCateg, Mpa.TargetSystem, 11);
                                if(Mc.RecordFound == true)
                                {                  
                                    Mpa.MatchingCateg = Mc.CategoryId;

                                    Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                    Mpa.Origin = Mc.Origin;
                                        Mpa.TerminalType = "10"; 
                                    Mpa.TransTypeAtOrigin = Mc.TransTypeAtOrigin;
                                    Mpa.Product = Mc.Product;
                                    Mpa.CostCentre = Mc.CostCentre;
                                }
                            }
                            else
                            {
                                // Goes To BancNet 
                                Mpa.MatchingCateg = "EWB101";
                                Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                              
                                    Mc.ReadMatchingCategoryBySelectionCriteria(Mpa.MatchingCateg, Mpa.TargetSystem, 12);

                                    Mpa.Origin = Mc.Origin;
                                    Mpa.TerminalType = "10";
                                    Mpa.TransTypeAtOrigin = Mc.TransTypeAtOrigin;
                                Mpa.Product = Mc.Product;
                                Mpa.CostCentre = Mc.CostCentre;
                                Mpa.TargetSystem = Mc.TargetSystemId; 
                            }

                            Mpa.MatchingAtRMCycle = 0 ; // This is to fill up during matching process 

                            Mpa.TerminalId = WAtmNo ;

                            if (trandesc == "WITHDRAWAL")
                            {
                                Mpa.TransType = 11; // If Withdrawl 
                                Mpa.DepCount = 0;
                            }

                            if (trandesc == "DEPOSIT_BNA")
                            {
                                Mpa.TransType = 23; // Deposit Cash 
                                Mpa.DepCount = TranAmount;
                            }
                            if (trandesc == "DEPOSIT")
                            {
                                Mpa.TransType = 24; // Deposit Cheque 
                                Mpa.DepCount = TranAmount;
                            }
                            if (trandesc == "DEP CHEQUES")
                            {
                                Mpa.TransType = 25; // Deposit Envelops 
                                Mpa.DepCount = TranAmount;
                            }                         
                        
                            Mpa.TransDescr = trandesc;

                            Mpa.CardNumber = CardNo;

                            Mpa.AccNumber = AccNo;

                            Mpa.TransCurr = CurrDesc;
                            Mpa.TransAmount = TranAmount;

                            Mpa.TransDate = TRanDate;

                            Mpa.AtmTraceNo = TraceNo;

                            Mpa.MasterTraceNo = MasterTraceNo;

                            Mpa.MetaExceptionNo = 0 ;

                            if (PresError == "PRESENTER ERROR")
                            {
                                Mpa.MetaExceptionNo = 55;
                            }
                            if (SuspectDesc == "SUSPECT FOUND")
                            {
                                Mpa.MetaExceptionNo = 225;
                            }

                            if (Mpa.MetaExceptionNo > 0)
                            {
                                Ec.ReadErrorsTableSpecific(Mpa.MetaExceptionNo);
                                Mpa.MetaExceptionId = Ec.ErrId;
                            }

                            Mpa.RRNumber = "0";

                            Mpa.ResponseCode = "";

                            Mpa.Operator = WOperator ;

                            Mpa.ReplCycleNo = WSesNo;

                            Mpa.Matched = false;
                           
                            Mpa.MatchMask = "";
                            Mpa.FileId01 = "";
                            Mpa.FileId02 = "";
                            Mpa.FileId03 = "";

                            Mpa.Comments = "";

                            Mpa.InsertTransMasterPoolATMs(WOperator);
                            //
                            // BUILD THE POOL TRANSACTION 
                            // TO BE DELETED AFTER TESTING OF MASTER

                            Tc.OriginName = "OurATMs";
                            Tc.RMCateg = RMCateg;
                            Tc.AtmTraceNo = TraceNo;
                            Tc.EJournalTraceNo = TraceNo;
                            Tc.MasterTraceNo = MasterTraceNo;

                            Tc.UniqueRecordId = UniqueRecordId;

                            if (trandesc == "WITHDRAWAL")
                            {
                                Tc.TransType = 11; // If Withdrawl 
                                Tc.DepCount = 0;
                            }

                            if (trandesc == "DEPOSIT_BNA")
                            {
                                Tc.TransType = 23; // Deposit Cash 
                                Tc.DepCount = TranAmount;
                            }
                            if (trandesc == "DEPOSIT")
                            {
                                Tc.TransType = 24; // Deposit Cheque 
                                Tc.DepCount = TranAmount;
                            }
                            if (trandesc == "DEP CHEQUES")
                            {
                                Tc.TransType = 25; // Deposit Envelops 
                                Tc.DepCount = TranAmount;
                            }

                            Tc.AtmNo = InAtmNo;
                            Tc.SesNo = WSesNo;
                            Tc.BankId = WBankId;

                            Tc.BranchId = Ac.Branch;

                            Tc.AtmDtTime = TRanDate;
                            //Tc.HostDtTime = TRanDate;

                            Tc.SystemTarget = CardTargetSystem;

                            Tc.TransDesc = trandesc;
                            Tc.CardNo = CardNo;
                            Tc.CardOrigin = 1;

                            Tc.AccNo = AccNo;

                            Tc.CurrDesc = CurrDesc;

                            Tc.TranAmount = TranAmount;
                            Tc.AuthCode = 0;
                            Tc.RefNumb = 0;
                            Tc.RemNo = 0;

                            Tc.TransMsg = "";
                            Tc.AtmMsg = "";
                            Tc.ErrNo = 0;

                            Tc.StartTrxn = StartTrxn;
                            Tc.EndTrxn = EndTrxn;

                            Tc.CommissionCode = CommissionCode;
                            Tc.CommissionAmount = CommissionAmount;

                            if (Result.Substring(0, 2) == "OK") Tc.SuccTran = true;
                            else Tc.SuccTran = false;

                            // ADD THE TRANSACTION 
                            Tc.InsertTransInPool(WOperator, InAtmNo);

                            //   PresError = (string)rdr["PresError"];

                            // SuspectDesc = (string)rdr["SuspectDesc"];

                            if (PresError == "PRESENTER ERROR" || SuspectDesc == "SUSPECT FOUND")
                            {
                                Tc.ReadTranForTrace(WAtmNo, TraceNo); // To get the Transaction No

                                if (PresError == "PRESENTER ERROR")
                                {
                                    Ec.ErrId = 55;
                                }
                                if (SuspectDesc == "SUSPECT FOUND")
                                {
                                    Ec.ErrId = 225;
                                }

                                Ec.BankId = WBankId;
                                Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                                Am.ReadAtmsMainSpecific(WAtmNo);

                                // INITIALISED WHAT IS NEEDED 

                                Ec.CategoryId = "EWB110";
                                Ec.RMCycle = Tc.RMCategCycle;
                                Ec.UniqueRecordId = Tc.UniqueRecordId;

                                Ec.AtmNo = WAtmNo;
                                Ec.SesNo = WSesNo;
                                Ec.DateInserted = DateTime.Now;
                                Ec.DateTime = TRanDate;
                                Ec.BranchId = Ac.Branch;
                                Ec.ByWhom = WSignedId;

                                Ec.CurDes = Tc.CurrDesc;
                                Ec.ErrAmount = Tc.TranAmount;

                                Ec.TraceNo = Tc.AtmTraceNo;
                                Ec.CardNo = Tc.CardNo;
                              
                                Ec.TransType = Tc.TransType;
                                Ec.TransDescr = Tc.TransDesc;

                                Ec.CustAccNo = Tc.AccNo;

                                Ec.DatePrinted = NullPastDate;

                                Ec.OpenErr = true;

                                Ec.CitId = Am.CitId;

                                Ec.Operator = WOperator;

                                int ErrorNoInserted = Ec.InsertError(); // INSERT ERROR

                                // Update Error Number in transaction 

                                Tc.UpdateTransErrNo(Tc.TranNo, ErrorNoInserted);
                            }
                        }

                        if (trandesc == "SM-CASH ADDED") // Cash ADDED 
                        {
                            // 
                            // UPdate Cassettes
                            // 
                            Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                            Ta.FirstTraceNo = TraceNo;
                            Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                            // Update Notes record

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                            Na.Cassettes_1.InNotes = Cassette1;
                            Na.Cassettes_2.InNotes = Cassette2;
                            Na.Cassettes_3.InNotes = Cassette3;
                            Na.Cassettes_4.InNotes = Cassette4;

                            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                            //
                            // Read Updated NEW 
                            //
                            Na.ReadSessionsNotesAndValues(InAtmNo, WSaveSesNo, 2);
                            decimal SaveMoneyIn = Na.Balances1.OpenBal;

                            // READ OLD - Previous data

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSaveSesNo, 2);

                            // UPDATE Action TABLE WITH REAL MONEY 
                            Ra.ReadReplActionsForAtmReplCycleNo(WAtmNo, WSaveSesNo);

                            if (Ra.RecordFound)
                            {
                                Ra.InMoneyReal = Na.Balances1.OpenBal;

                                Ra.ActiveRecord = false;

                                Ra.UpdateReplActionsForAtm(WAtmNo, Ra.ReplOrderNo);
                            }

                            if (SaveMoneyIn != Na.Balances1.OpenBal)
                            {
                                Ra.ActiveRecord = false;
                                //MessageBox.Show(" MsgNo: 456387 An email will be sent to Controller. Money said in Replenishment different of what actual in"); 
                            }
                            //
                            // INSERT TRANSACTION WITH CASH ADDED
                            // 
                            // Build amount

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                            int TraceNoCashIn = TraceNo;

                            if (Na.BalSets >= 1)
                            {
                                // CALL METHOD TO ADD TRANSACTION FOR CASH ADDED 

                                InsertTranInPool(InAtmNo, WSesNo, TraceNoCashIn, MasterTraceNo,
                                    Na.Balances1.CurrNm, Na.Balances1.OpenBal, StartTrxn, EndTrxn);

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
                        // SUPERVISOR MODE STARTS - THE OLD REPL CYCLE ENDS
                        if (trandesc == "SM-START" & TransactionType == 77) // SUPERVISOR MODE START = THIS IS THE END OF REPL CYCLE 
                        {
                            // Update the right Trace with the start
                            // Get TraceNo with zero last digit 
                            // Look and find Traces with this trace within 
                            // Update Start time for this Traces Record
                            // When we find start means that the yesterdays Repl Cycle has Ended 
                            // Therefore Ta must be updated 

                            SmStartDtTime = TRanDate;
                            if (WSesNo > 0) // If it is equal to zero it means this is the first time and there is no Trace No 
                            {
                                Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                                Ta.SesDtTimeEnd = SmStartDtTime;
                                Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);
                            }

                        }
                        // SUPERVISOR MODE ENDS - A NEW REPL CYCLE STARTS
                        if (trandesc == "SM-END" & TransactionType == 77) // YESTERDAYS SM 
                        {
                            // Update the Right Trace with the end 
                            // HERE WE FIND THE DURATION OF THE REPL CYCLE
                            // Remember at Cash in we have created a new Ta = New Repl Cycle 
                            // We must set up the time when this Ta starts 

                            SmEndDtTime = TRanDate; // THIS THE YESTERDAYS SM  

                            Ta.ReadSessionsStatusTraces(InAtmNo, WSesNo);
                            Ta.SesDtTimeStart = SmEndDtTime;

                            Ta.UpdateSessionsStatusTraces(InAtmNo, WSesNo);

                            RRDMReplStatsClass Rs = new RRDMReplStatsClass();
                            Rs.InsertInAtmsStatsProcess(WOperator, InAtmNo, WSesNo,0);
                            // Within method we find the information of previous Repl Cycle through the previous 
                            // WSesNo and we update the statistics based on the information of Na and Ta

                        }

                        if (trandesc == "SM-DISPENCED") // Dispensed
                        {

                            // HERE WE NEED DATE AND TIME 

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                            Na.Cassettes_1.DispNotes = Cassette1;
                            Na.Cassettes_2.DispNotes = Cassette2;
                            Na.Cassettes_3.DispNotes = Cassette3;
                            Na.Cassettes_4.DispNotes = Cassette4;

                            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                        }

                        if (trandesc == "SM-REJECTED") // Rejected
                        {
                            // HERE WE NEED DATE AND TIME 

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                            Na.Cassettes_1.RejNotes = Cassette1;
                            Na.Cassettes_2.RejNotes = Cassette2;
                            Na.Cassettes_3.RejNotes = Cassette3;
                            Na.Cassettes_4.RejNotes = Cassette4;

                            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                        }

                        if (trandesc == "SM-CASSETTE") // Cassette equivalent Gas Remaining = What it is in cassettes 
                        {

                            // HERE WE NEED DATE AND TIME 

                            Na.ReadSessionsNotesAndValues(InAtmNo, WSesNo, 2);

                            Na.Cassettes_1.RemNotes = Cassette1; // Equivalent what it is in cassettes NOT REJected 
                            Na.Cassettes_2.RemNotes = Cassette2;
                            Na.Cassettes_3.RemNotes = Cassette3;
                            Na.Cassettes_4.RemNotes = Cassette4;

                            Na.UpdateSessionsNotesAndValues(InAtmNo, WSesNo);
                        }

                        if (trandesc == "SM-REMAINING") // Cash OUT FROM MACHINE ( CASSETTE + REJECTED )
                        {
                            // CALL CASH OUT CLASS 
                            // Create transaction for 
                            // a) ATM Pool
                            // b) CIT provider STATEMENT 

                            Ct.InsertTranForCashOut(WSignedId, WSignRecordNo, WOperator,
                                InAtmNo, WSesNo, StartTrxn, EndTrxn, MasterTraceNo);

                        }


                        K++; // Read Next entry of the table 
                    }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }



            // Read Konto Errors table and create Errors in GAS
            // READ Errors 

            //Em.ReadInsertMatchedErrors(WAtmNo);

            // UPDATE ATMsMain with Number of outstanding errors 

            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

            Am.ReadAtmsMainSpecific(WAtmNo);

            Ec.ReadAllErrorsTableForCounters(WOperator, "EWB110", WAtmNo, WSesNo, "");
            Am.ErrOutstanding = Ec.NumOfErrors;
            Am.ProcessMode = Ta.ProcessMode;

            Am.UpdateAtmsMain(WAtmNo);

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
        // CALL KONTO Procedure to read ejournal from directory and insert it in Files
        //
        private void StoreProcKontoEJournal(string InBankId, string InAtmNo, string InJournalType, string InPathName)
        {

            // Call Store procedure for reading ATM ejournal and preparing the files

            // Read Ejournal and create Data bases  

            string RCT = "[ATMS_Journals].[dbo].[stp_00_Run_Process]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@FullPath", InPathName);


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
        // CALL KONTO Procedure to Make Reconciliation of Host Files with ATM Journal records  
        //
        private void StoreProcKontoMatchingOfFiles(string InBankId, string InAtmNo)
        {

            // Call Store procedure for reading ATM ejournal and preparing the files

            // Read Ejournal and create Data bases  

            string RCT = "[ATMS_Journals].[dbo].[usp_H_Reconciliation_Step_00]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

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

            Tc.UniqueRecordId = GetNextValue(connectionString);

            Tc.OriginName = "OurATMs-Repl : " + InAtmNo;

            Tc.AtmNo = InAtmNo;
            Tc.SesNo = WSesNo;
            Tc.BankId = WBankId;

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

                System.Windows.Forms.MessageBox.Show("Account not found for ATMNo: " + InAtmNo);

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
        // Get Next Unique Id 
        static int GetNextValue(string InConnectionString)
        {
            int iResult = 0;

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[usp_GetNextUniqueId]";

            using (SqlConnection conn = new SqlConnection(InConnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        SqlParameter iNextValue = new SqlParameter("@iNextValue", SqlDbType.Int);
                        iNextValue.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(iNextValue);
                        cmd.ExecuteNonQuery();
                        string sResult = cmd.Parameters["@iNextValue"].Value.ToString();
                        int.TryParse(sResult, out iResult);

                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    //CatchDetails(ex);
                }
            return iResult;
        }
      
    }
}

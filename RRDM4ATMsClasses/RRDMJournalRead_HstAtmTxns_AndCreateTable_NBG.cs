using System;
using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable_NBG() : base() { }
        /// <summary>
        /// THIS CLASS READS ALL Process = false from Pambos 
        /// and inserts TXNS in Mpa
        /// Then Updates to Process = true; 
        /// </summary>
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        //bool OwnCard; 
        bool CreateReversal;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber(); 

        public DataTable HstAtmTxnsDataTable = new DataTable();

        public DataTable TempTableMpa = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //int WSaveSesNo;

        //int FirstTrace;

        public int TotalRecords;
        public int TotalTxns; 

        int fuid;

        int UniqueRecordId;

        string OldATMNo;
        //string NewATMNo;

        int TraceNo;

        string TraceNoWithNoEndZero;

        string Wtrandesc;

        string WCitId;
        string WOperator;

        //int WLastTraceNo;
        //int WSesNo;

        string WSignedId;
        int WSignRecordNo;
        string WBankId;

        string WAtmNo;
        int WMode;
        int WLoadingCycleNo;

        public void ReadJournalAndCreateTables(string InSignedId, int InSignRecordNo, string InBankId, int InReconcCycleNo,
                                                          int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WBankId = InBankId;

            WMode = InMode;

            // Read transactions from EJournal file
            // 

            WLoadingCycleNo = InReconcCycleNo;
           
            //int I = 0;

            DateTime TRanDate;

            //int FirstTrace; 
            int WSesNo = 0; // Currently is as such 

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

            string CardCaptured;

            string PresError;

            string SuspectDesc;
            string CardCapturedMes;

            string Result;

            int Cassette1;
            int Cassette2;
            int Cassette3;
            int Cassette4;

            string SqlString = "";

            TempTableMpa = new DataTable();
            TempTableMpa.Clear();

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

            TempTableMpa.Columns.Add("ResponseCode", typeof(string));
            TempTableMpa.Columns.Add("Operator", typeof(string));

            TempTableMpa.Columns.Add("ReplCycleNo", typeof(int));

            DateTime NullDate = new DateTime(1900, 01, 01);

          
            // 
            // AT this point UPDATED KONTO Files are ready to be read by GAS  
            // 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalRecords = 0;
            TotalTxns = 0; 

            // Define the data table 

            HstAtmTxnsDataTable = new DataTable();
            HstAtmTxnsDataTable.Clear();

            SqlString = "SELECT atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
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
                  + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
                //+ " WHERE atmno = @atmno AND Processed = 0 AND CAmount > 0 AND TransactionType <> 99 AND Result = 'OK'  "
                + " WHERE Processed = 0 AND CAmount > 0 AND TransactionType <> 99 AND Result = 'OK'  "
                + " ORDER by AtmNo, TraceNumber";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                       
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@atmno", WAtmNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(HstAtmTxnsDataTable);

                        // Close conn
                        conn.Close();

                        if (HstAtmTxnsDataTable.Rows.Count == 0)
                        {
                            return; 
                        }

                        // Initailise 


                        OldATMNo = (string)HstAtmTxnsDataTable.Rows[0]["atmno"]; 

                        int K = 0;

                        while (K <= (HstAtmTxnsDataTable.Rows.Count - 1))
                        {
                            // GET Table fields - Line by Line
                            //

                            TotalRecords = TotalRecords + 1; 

                            WAtmNo = (string)HstAtmTxnsDataTable.Rows[K]["atmno"];

                            if (WAtmNo != OldATMNo)
                            {
                                //
                                // Update Old ATMs records as processed
                                //
                                UpdatetblHstAtmTxnsProcessedToTrue(OldATMNo, TraceNo);
                            }
                            OldATMNo = WAtmNo;

                            Ac.ReadAtm(WAtmNo); // Read Information for ATM 

                            WOperator = Ac.Operator;

                            WCitId = Ac.CitId;

                            RecordFound = true;

                            TraceNo = (int)HstAtmTxnsDataTable.Rows[K]["TraceNumber"];

                            //if (K == 0)
                            //{
                            //    FirstTrace = TraceNo; 
                            //}

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

                            CardCaptured = (string)HstAtmTxnsDataTable.Rows[K]["CardCaptured"];
                            PresError = (string)HstAtmTxnsDataTable.Rows[K]["PresError"];
                            SuspectDesc = (string)HstAtmTxnsDataTable.Rows[K]["SuspectDesc"];
                            CardCapturedMes = (string)HstAtmTxnsDataTable.Rows[K]["CardCapturedMes"];

                            CommissionCode = (int)HstAtmTxnsDataTable.Rows[K]["CommissionCode"];
                            CommissionAmount = (decimal)HstAtmTxnsDataTable.Rows[K]["CommissionAmount"];
                            CardTargetSystem = (int)HstAtmTxnsDataTable.Rows[K]["CardTargetSystem"];

                            Cassette1 = (int)HstAtmTxnsDataTable.Rows[K]["Type1"];
                            Cassette2 = (int)HstAtmTxnsDataTable.Rows[K]["Type2"];
                            Cassette3 = (int)HstAtmTxnsDataTable.Rows[K]["Type3"];
                            Cassette4 = (int)HstAtmTxnsDataTable.Rows[K]["Type4"];

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

                                TotalTxns = TotalTxns + 1; 

                                Wtrandesc = trandesc;

                                // FIND WORKING CATEGORY

                                string WMatchingCategoryId = "";

                                Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator,0, CardNo.Substring(0, 6), "Our Atms", 12);
                                if (Mcb.RecordFound)
                                {
                                    // Own Card
                                    WMatchingCategoryId = Mcb.CategoryId;
                                    //OwnCard = true; 
                                }
                                else
                                {
                                    // Other Card 
                                    Mc.ReadMatchingCategorybyGetsAllOtherBINS(WOperator);
                                    if (Mc.RecordFound == true)
                                    {
                                        WMatchingCategoryId = Mc.CategoryId;
                                    }
                                    //OwnCard = false;
                                }

                                UniqueRecordId = Gu.GetNextValue();

                                // FILL Data Table 
                                DataRow RowSelected = TempTableMpa.NewRow();

                                RowSelected["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                RowSelected["OriginalRecordId"] = TraceNo;
                                RowSelected["UniqueRecordId"] = UniqueRecordId;

                                RowSelected["MatchingCateg"] = WMatchingCategoryId;

                                Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCategoryId);

                                RowSelected["FileId01"] = Mcs.SourceFileNameA;
                                RowSelected["FileId02"] = Mcs.SourceFileNameB;
                                RowSelected["FileId03"] = Mcs.SourceFileNameC;
                                RowSelected["FileId04"] = Mcs.SourceFileNameD;
                                RowSelected["FileId05"] = Mcs.SourceFileNameE;

                                //Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                //string SelectionCriteria = " WHERE CategoryId = '"
                                //                            + Mpa.MatchingCateg + "'";
                                Mc.ReadMatchingCategoryBySelectionCriteria(WMatchingCategoryId, 0, 12);

                                RowSelected["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                RowSelected["Origin"] = Mc.Origin;
                                RowSelected["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                RowSelected["Product"] = Mc.Product;
                                RowSelected["CostCentre"] = Mc.CostCentre;
                                RowSelected["TargetSystem"] = Mc.TargetSystemId;

                                decimal DepCount = 0 ;  

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

                                        CreateReversal = true;
                                    }
                                    if (SuspectDesc == "SUSPECT FOUND")
                                    {
                                        Ec.ErrId = 225;
                                    }

                                    Ec.BankId = WBankId;
                                    Ec.ReadErrorsIDRecord(Ec.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

                                    Am.ReadAtmsMainSpecific(WAtmNo);

                                    // INITIALISED WHAT IS NEEDED 

                                    Ec.CategoryId = WMatchingCategoryId;
                                    Ec.RMCycle = WLoadingCycleNo;
                                    Ec.UniqueRecordId = UniqueRecordId;

                                    Ec.AtmNo = WAtmNo;
                                    Ec.SesNo = WSesNo ;
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
                                    RowSelected["LoadedAtRMCycle"] = WLoadingCycleNo;
                                    RowSelected2["MatchingAtRMCycle"] = 0;
                                    RowSelected2["TerminalId"] = WAtmNo;
                                    RowSelected2["TransType"] = 23;
                                    RowSelected2["DepCount"] = DepCount;

                                    RowSelected2["TransDescr"] = "Reversal Entry For Trace No : " + TraceNo.ToString();
                                    RowSelected2["CardNumber"] = CardNo;
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

                            K++; // Read Next entry of the table 
                        }

                        //TestTableMpa
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

                                CatchDetails(ex);
                            }
                        //++++++++++++++++++++++++++++++++++   
                        // Updating for last ATM read 
                        //++++++++++++++++++++++++++++++++++
                        UpdatetblHstAtmTxnsProcessedToTrue(WAtmNo, TraceNo);
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        // UPDATE Processed = true in Hst Table 
        // 
        public void UpdatetblHstAtmTxnsProcessedToTrue(string InAtmNo, int InTraceNumber)
        {

            ErrorFound = false;
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
                            //+ " WHERE AtmNo = @AtmNo And (TraceNumber >= @FirstTrace And TraceNumber<=@LastTrace", conn))
                            + " WHERE AtmNo = @AtmNo AND Processed = 0 AND TraceNumber <= @TraceNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);
                        //cmd.Parameters.AddWithValue("@FirstTrace", InFirstTrace);
                        //cmd.Parameters.AddWithValue("@LastTrace", InLastTrace);

                      
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

        //// Get Next Unique Id 
        //static int GetNextValue(string InConnectionString)
        //{
        //    int iResult = 0;

        //    string RCT = "[RRDM_Reconciliation].[dbo].[usp_GetNextUniqueId]";

        //    using (SqlConnection conn = new SqlConnection(InConnectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(RCT, conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                // Parameters
        //                SqlParameter iNextValue = new SqlParameter("@iNextValue", SqlDbType.Int);
        //                iNextValue.Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add(iNextValue);
        //                cmd.ExecuteNonQuery();
        //                string sResult = cmd.Parameters["@iNextValue"].Value.ToString();
        //                int.TryParse(sResult, out iResult);

        //                //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
        //                //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //    return iResult;
        //}
       
    }
}



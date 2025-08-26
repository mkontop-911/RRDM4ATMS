using System;
using System.Data;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable_BDO : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable_BDO() : base() { }
        /// <summary>
        /// THIS CLASS READS ALL Process = false from Pambos 
        /// and inserts TXNS in Mpa
        /// Then Updates to Process = true; 
        /// </summary>
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
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

        //int TraceNo;

        int fuid;

        int UniqueRecordId;

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

        public void ReadAndCreateTables(string InSignedId, int InSignRecordNo, string InBankId,
            string InAtmNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WBankId = InBankId;

            WAtmNo = InAtmNo;
            WMode = InMode;

            // Read transactions from EJournal file
            // 1) Update Mpa Trans


            int I = 0;

            DateTime TRanDate;

            //int FirstTrace; 

            int TraceNo;

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

            TempTableMpa.Columns.Add("ResponseCode", typeof(int));
            TempTableMpa.Columns.Add("Operator", typeof(string));

            TempTableMpa.Columns.Add("ReplCycleNo", typeof(int));

            DateTime NullDate = new DateTime(1900, 01, 01);

            Ac.ReadAtm(InAtmNo); // Read Information for ATM 

            WOperator = Ac.Operator;

            WCitId = Ac.CitId;
            // 
            // AT this point UPDATED KONTO Files are ready to be read by GAS  
            // 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Define the data table 

            HstAtmTxnsDataTable = new DataTable();
            HstAtmTxnsDataTable.Clear();

            SqlString = "SELECT atmno, TraceNumber,Trace As TraceNoWithNoEndZero ,ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc,"
            + "ISNULL(currency, '') AS currency, CAmount,ISNULL(cardnum, '') AS cardnum,"
            + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
            + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
            + "ISNULL(CardCaptured, '') AS CardCaptured, ISNULL(CardCapturedMES, '') AS CardCapturedMes,"
            + "ISNULL(PresenterError, '') AS PresError, ISNULL(SuspectDesc, '') AS SuspectDesc,ISNULL(SuspectNotes,'') AS SuspectNotes,"
            + "Type1, type2, type3, type4, Processed,  ISNULL(CardTargetSystem, 0) AS CardTargetSystem,  TransactionType,"
             + "CommissionCode, CommissionAmount"
                + " FROM [ATM_Journals_Diebold].[dbo].[tblHstAtmTxns] "
                + " WHERE atmno = @atmno AND Processed = 0 AND CAmount > 0 AND Result = 'OK'  "
                + " ORDER by TraceNumber";

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

                        sqlAdapt.Fill(HstAtmTxnsDataTable);

                        // Close conn
                        conn.Close();

                        int K = 0;

                        while (K <= (HstAtmTxnsDataTable.Rows.Count - 1))
                        {
                            // GET Table fields - Line by Line
                            //
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

                            //if (TraceNoWithNoEndZero == "00447498" )
                            //{
                            //    System.Windows.Forms.MessageBox.Show(" trace : " + TraceNoWithNoEndZero);
                            //}

                            if ((trandesc == "WDRSA" || trandesc == "WDRCA" || trandesc == "EPLUS"
                                || trandesc == "DPSA" || trandesc == "TFR**" || trandesc == "BPTXN"
                                || trandesc == "PMRCA" || trandesc == "PMRCH" || trandesc == "PMRSA"
                                || trandesc == "DPCA"
                                )
                                & Result.Substring(0, 2) == "OK" & TranAmount > 0)
                            {

                                if (trandesc == "WDRSA") Wtrandesc = "WDRSA-Withdrawal";
                                if (trandesc == "WDRCA") Wtrandesc = "WDRCA-Withdrawal";
                                if (trandesc == "EPLUS") Wtrandesc = "EPLUS-Reloading SM E-Plus Card";

                                if (trandesc == "DPSA") Wtrandesc = "DPSA-Deposits SA";
                                if (trandesc == "TFR**") Wtrandesc = "TFR**-Transfer";
                                if (trandesc == "BPTXN") Wtrandesc = "BPTXN-Bill Payment";

                                if (trandesc == "PMRCA") Wtrandesc = "Prepaid Mob Reload from Current Account";
                                if (trandesc == "PMRCH") Wtrandesc = "Prepaid Mob Reload from Cash Card";
                                if (trandesc == "PMRSA") Wtrandesc = "Prepaid Mob Reload from Savings Account";

                                if (trandesc == "DPCA") Wtrandesc = "DPCA-Deposits CA";

                                // MAP THE INPUT
                                I = I + 1;

                                //Ac.ReadAtm(WAtmNo);
                                UniqueRecordId = Gu.GetNextValue();

                                //Mpa.OriginFileName = "ATM:" + WAtmNo + " Journal";
                                //Mpa.OriginalRecordId = TraceNo;
                                //Mpa.UniqueRecordId = UniqueRecordId;

                                // FILL Data Table 
                                DataRow RowSelected = TempTableMpa.NewRow();

                                RowSelected["OriginFileName"] = "ATM:" + WAtmNo + " Journal";
                                RowSelected["OriginalRecordId"] = TraceNo;
                                RowSelected["UniqueRecordId"] = UniqueRecordId;


                                //                          (BIN = '011310'
                                //OR BIN = '486290'
                                //OR BIN = '489504'
                                //OR BIN = '519463'
                                //OR BIN = '521069'
                                //OR BIN = '522452'
                                //OR BIN = '531832' )

                                //OR BIN = '011311'
                                //OR BIN = '121212'
                                //OR BIN = '526727'
                                //OR BIN = '592116'
                                //OR BIN = '601853'
                                //CardNo.Substring(0, 6) == "011310"
                                //                             || CardNo.Substring(0, 6) == "011311"
                                //                             || CardNo.Substring(0, 6) == "121212"
                                //                             || CardNo.Substring(0, 6) == "486290"
                                //                             || CardNo.Substring(0, 6) == "489504"
                                //                             || CardNo.Substring(0, 6) == "519463"
                                //                             || CardNo.Substring(0, 6) == "521069"
                                //                             || CardNo.Substring(0, 6) == "522452"
                                //                             || CardNo.Substring(0, 6) == "526727"
                                //                             || CardNo.Substring(0, 6) == "531832"
                                //                             || CardNo.Substring(0, 6) == "592116"
                                //                             || CardNo.Substring(0, 6) == "601853"
                                if (
                                    CardNo.Substring(0, 6) == "011310"
                                    || CardNo.Substring(0, 6) == "486290"
                                    || CardNo.Substring(0, 6) == "489504"
                                    || CardNo.Substring(0, 6) == "519463"
                                    || CardNo.Substring(0, 6) == "521069"
                                    || CardNo.Substring(0, 6) == "522452"
                                    || CardNo.Substring(0, 6) == "531832"
                                    //|| CardNo.Substring(0, 6) == "011311" --- 2
                                    || (CardNo.Substring(0, 6) == "121212" & trandesc != "BPTXN")// 
                                                                                                 //|| CardNo.Substring(0, 6) == "526727" ---- 166
                                                                                                 //|| CardNo.Substring(0, 6) == "592116"  ----- 0 
                                    || CardNo.Substring(0, 6) == "601853" //
                                    )
                                {
                                    Mpa.MatchingCateg = "EWB103";
                                    //Mcs.ReadReconcCategoriesVsSourcesAll("EWB103");

                                    //Mpa.FileId01 = Mcs.SourceFileNameA;
                                    //Mpa.FileId02 = Mcs.SourceFileNameB;
                                    //Mpa.FileId03 = Mcs.SourceFileNameC;
                                    //Mpa.FileId04 = Mcs.SourceFileNameD;
                                    //Mpa.FileId05 = Mcs.SourceFileNameE;

                                }
                                else
                                {
                                    Mpa.MatchingCateg = "EWB104";

                                    //Mcs.ReadReconcCategoriesVsSourcesAll(Mpa.MatchingCateg);

                                    //Mpa.FileId01 = Mcs.SourceFileNameA;
                                    //Mpa.FileId02 = Mcs.SourceFileNameB;
                                    //Mpa.FileId03 = Mcs.SourceFileNameC;
                                    //Mpa.FileId04 = Mcs.SourceFileNameD;
                                    //Mpa.FileId05 = Mcs.SourceFileNameE;
                                }

                                RowSelected["MatchingCateg"] = Mpa.MatchingCateg;

                                Mcs.ReadReconcCategoriesVsSourcesAll(Mpa.MatchingCateg);

                                RowSelected["FileId01"] = Mcs.SourceFileNameA;
                                RowSelected["FileId02"] = Mcs.SourceFileNameB;
                                RowSelected["FileId03"] = Mcs.SourceFileNameC;
                                RowSelected["FileId04"] = Mcs.SourceFileNameD;
                                RowSelected["FileId05"] = Mcs.SourceFileNameE;

                                //Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                                //string SelectionCriteria = " WHERE CategoryId = '"
                                //                            + Mpa.MatchingCateg + "'";
                                Mc.ReadMatchingCategoryBySelectionCriteria(Mpa.MatchingCateg, Mpa.TargetSystem, 12);

                                //Mpa.Origin = Mc.Origin;
                                //Mpa.TransTypeAtOrigin = Mc.TransTypeAtOrigin;
                                //Mpa.Product = Mc.Product;
                                //Mpa.CostCentre = Mc.CostCentre;
                                //Mpa.TargetSystem = Mc.TargetSystemId;

                                RowSelected["RMCateg"] = "RECATMS-" + Ac.AtmsReconcGroup.ToString();
                                RowSelected["Origin"] = Mc.Origin;
                                RowSelected["TransTypeAtOrigin"] = Mc.TransTypeAtOrigin;
                                RowSelected["Product"] = Mc.Product;
                                RowSelected["CostCentre"] = Mc.CostCentre;
                                RowSelected["TargetSystem"] = Mc.TargetSystemId;

                                //Mpa.MatchingAtRMCycle = 0; // This is to fill up during matching process 

                                Mpa.TerminalId = WAtmNo;

                                if (trandesc == "WDRSA" || trandesc == "PMRCA" || trandesc == "PMRCH" || trandesc == "PMRSA")
                                {
                                    Mpa.TransType = 11; // If Withdrawl 
                                    Mpa.DepCount = 0;

                                }

                                if (trandesc == "DPSA")
                                {
                                    Mpa.TransType = 23; // Deposit Cash 
                                    Mpa.DepCount = TranAmount;
                                }
                                if (trandesc == "TFR**")
                                {
                                    Mpa.TransType = 24; // Transfer 
                                    Mpa.DepCount = 0;
                                }
                                if (trandesc == "BPTXN")
                                {
                                    Mpa.TransType = 11; // Bill Payment 
                                    Mpa.DepCount = 0;
                                }

                                RowSelected["MatchingAtRMCycle"] = 0;
                                RowSelected["TerminalId"] = WAtmNo;
                                RowSelected["TransType"] = Mpa.TransType;
                                RowSelected["DepCount"] = Mpa.DepCount;

                                //Mpa.TransDescr = Wtrandesc;
                                //Mpa.CardNumber = CardNo;
                                //Mpa.AccNumber = AccNo;
                                //Mpa.TransCurr = CurrDesc;

                                RowSelected["TransDescr"] = Wtrandesc;
                                RowSelected["CardNumber"] = CardNo;
                                RowSelected["AccNumber"] = AccNo;
                                RowSelected["TransCurr"] = CurrDesc;

                                //Mpa.TransAmount = TranAmount;
                                //Mpa.TransDate = TRanDate;
                                //Mpa.TraceNoWithNoEndZero = int.Parse(TraceNoWithNoEndZero);
                                //Mpa.AtmTraceNo = TraceNo;

                                //Mpa.MasterTraceNo = MasterTraceNo;

                                RowSelected["TransAmount"] = TranAmount;
                                RowSelected["TransDate"] = TRanDate;
                                RowSelected["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
                                RowSelected["AtmTraceNo"] = TraceNo;

                                RowSelected["MasterTraceNo"] = MasterTraceNo;

                                Mpa.MetaExceptionNo = 0;

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
                                else
                                {
                                    Mpa.MetaExceptionId = 0;
                                }

                                RowSelected["MetaExceptionNo"] = Mpa.MetaExceptionNo;
                                RowSelected["MetaExceptionId"] = Mpa.MetaExceptionId;

                                //Mpa.RRNumber = 0;

                                //Mpa.ResponseCode = 0;

                                //Mpa.Operator = WOperator;

                                //Mpa.ReplCycleNo = WSesNo;

                                //Mpa.Matched = false;

                                //Mpa.MatchMask = "";

                                //Mpa.Comments = "";

                                RowSelected["RRNumber"] = 0;
                                RowSelected["ResponseCode"] = 0;
                                RowSelected["Operator"] = WOperator;
                                RowSelected["ReplCycleNo"] = 0; // WSesNo

                                //RowSelected["Matched"] = false;
                                //RowSelected["MatchMask"] = "";
                                //RowSelected["Comments"] = "";

                                ///
                                /// INSERT TRANSACTION
                                /// 
                                //   Mpa.InsertTransMasterPoolATMs(WOperator, WAtmNo);

                                // ADD ROW
                                TempTableMpa.Rows.Add(RowSelected);

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

                        UpdatetblHstAtmTxnsProcessedToTrue(WAtmNo, fuid);
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
        public void UpdatetblHstAtmTxnsProcessedToTrue(string InAtmNo, int Infuid)
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
                        new SqlCommand("UPDATE [ATM_Journals_Diebold].[dbo].[tblHstAtmTxns] SET "
                              + " Processed = 1 "
                            //+ " WHERE AtmNo = @AtmNo And (TraceNumber >= @FirstTrace And TraceNumber<=@LastTrace", conn))
                            +" WHERE AtmNo = @AtmNo AND Processed = 0 AND fuid <= @fuid ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@fuid", Infuid);
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

using System;
using System.Data;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_ALPHA : Logger
    {
        public RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_ALPHA() : base() { }

        public bool RecordFound;
        public bool Major_ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

      

        RRDMGasParameters Gp = new RRDMGasParameters();

     

        public DataTable HstAtmTxnsDataTable = new DataTable();

        public DataTable TempTableMpa = new DataTable();

        public DataTable TableAtms = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime NullFutureDate = new DateTime(2050, 11, 21);

       

        bool ShowMessage;

        DateTime TRanDate;
        DateTime PreTranDate;

     

        int TraceNo;

      int MasterTraceNo;

    

        int WSesNo;

     
        string PRX; 

        public int TotalValidRecords = 0;
        public int TotalTxns = 0;
        public int GrandTotalTxns;

   

        int WLoadedAtRMCycle;

      
        string WSignedId;
        int WSignRecordNo;
        
        string WOperator;
        DateTime WCut_Off_Date;

        int WRMCycle; 
   
        string WAtmNo;
      

        public void ReadJournal_Txns_And_Insert_In_Pool(string InSignedId, int InSignRecordNo, string InOperator,
                                                             int InRMCycle)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

         
            WRMCycle = InRMCycle; 

            string SQLString;

            try
            {
                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

                string WJobCategory = "ATMs";
                // int WReconcCycleNo;

                WLoadedAtRMCycle = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

                WCut_Off_Date = Rjc.Cut_Off_Date;

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

          //  if (WMode == 1) Gr.InsertGroupDuringLoading(WAtmsReconcGroup);


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

            // LOAD Mpa
            int Counter = 0;

            // FILL Data Table 
        
            string SQLCmd = "INSERT INTO "
                         + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            + "( "

            + " [OriginFileName] " // 1
            + ",[OriginalRecordId] " // seqno // 2
            + ",[UniqueRecordId] " // 3

          //  + ",[MatchingCateg] " // BDC299 // 4
             + ",[FuID] " // fuid // 5
              + ",[SeqNo01] " // WSeqNo // 6 

            + ",[TXNSRC] " // "1" // 7.1
            + ",[TXNDEST] " // "0" // 7.2 
            + ",[Origin] " // Our ATM // 8

            + ",[LoadedAtRMCycle] " // 9 
            + ",[TerminalId] " // 10
            + ",[TransType] " // 11

            + ",[TransDescr] " // 12
            + ",[CardNumber] " // 13

            + ",[AccNumber] " // 14
            + ",[TransCurr] " // 15 
            + ",[TransAmount] " // 16 

             + ",[DepCount] " //17 
 

            + ",[TransDate] " // 18

            + ",[TraceNoWithNoEndZero] " // 19
            + ",[AtmTraceNo] " // 20
            + ",[MasterTraceNo] " // 21

          
            + ",[MetaExceptionId] " // 22

           

            + ",[ResponseCode] "  // 23 
            + ", [Operator] " //24

            + ") "
            //
            + " SELECT  "

              + " 'ATMs Journals' " // 1
            + ", SeqNo " // seqno // 2
           + ", NEXT VALUE FOR [RRDM_Reconciliation_ITMX].dbo.ReconcSequence " //3

           // + ",@MatchingCateg " // BDC299 //4
             + ",ISNULL(fuid, 0) AS fuid " //5
              + ", SeqNo " // SeqNo01 //6

               + ",'1' " // Transaction source //7.1
               + ",'0' " // TXNDEST // 7.2 
            + ",'Our Atms' " // Our ATM //8

             + ",@LoadedAtRMCycle " //9
            + ",ISNULL(atmno, '') " //10
            + ",ISNULL(TransactionType, 0) " //11

            // + ",[TransDescr] "
            + " ,ISNULL(trandesc, '') " //12
                                        // + ",[CardNumber] "
            + " ,ISNULL(TRIM(cardnum), '') " //13
                                       //  + ",[AccNumber] "
            + ",case " //14
                 + " when TransactionType = 11 THEN ISNULL(acct1, '') "
                 + " when TransactionType = 33 THEN ISNULL(acct1, '') "
                 + " else ISNULL(acct2, '') "
             + "end "

            //+ ",[TransCurr] "
            //+ ", Left(Currency, 3) "
            + ", ISNULL(Left(Currency, 3), '')  " //15

             // + ",[TransAmount] "
             + " ,ISNULL(CAmount, 0 )  " //16


              // + ",[DepCount] "

              + ",case " //17
                 + " when TransactionType < 20 THEN 0 "
                 + " when (TransactionType > 20 AND TransactionType < 30) THEN ISNULL(CAmount, 0 ) "
                 + " else 0 "
             + "end "

               + " ,ISNULL(Trandatetime, '') " //18
           
            + ", ISNULL(TraceNumber, 0)/10 " ///19
           
            + ", ISNULL(TraceNumber, 0)/10 " //20
                                             // + ",[MasterTraceNo] "
             + ", ISNULL(TraceNumber, 0)/10 " //21

           
             + ",case " // 22
         
                  + " when (PresenterError = 'PresenterError' AND TransactionType <> 23) THEN 55 "
                 + " when (SuspectDesc = 'SUSPECT FOUND') THEN 225 "
                 + " else 0 "
             + " end "
           
              + ",case " // 23 Response Code
                 + " when (ISNULL(Source, '') = '000') THEN '0' "
                  + " when (ISNULL(Source, '') = '00') THEN '0' "
                 + " else 0 "
             + " end "
           
            + ", @Operator " // 24

           + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns]" 
           + " WHERE Processed = 0 and Result = 'OK' and TransactionType <> 99 and TransactionType <> 0 and CAmount >0 "
           //  + " AND TransDate <=@TransDate "
           ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchingCateg", PRX + "202");  // default category 

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", WRMCycle);

                        //cmd.Parameters.AddWithValue("@TransDate ", MaxDt01_Minus);

                       // cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                      //  cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                      //  cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                      //  cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);

                        cmd.CommandTimeout = 1200;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();

                    //System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                    //             + "..:.." + Counter.ToString());

                }
                catch (Exception ex)
                {
                    conn.Close();

                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("Cancel While Loading Hst table AUDI:");
                    }
                    CatchDetails(ex);
                }
            //UPDATE ORIGINAL RECORDS as Processed
            UpdatetSourceTxnsProcessed(WRMCycle);

            //**********************************************************
            // UPDATE CATEGORY ID BASED ON BINS 
            //**********************************************************
            RRDM_LoadFiles_InGeneral_EMR_ALPHA Lf_Alpha = new RRDM_LoadFiles_InGeneral_EMR_ALPHA();

            Lf_Alpha.MethodToUpdateCategoriesALPHA(WOperator, "tblMatchingTxnsMasterPoolATMs");


            //SQLCmd = "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //            + "  SET MatchingCateg = case "  // SET UP THE CATEGORY 
            //            + " WHEN " // CREDIT CARD
            //            + "( Left(CardNumber,6) = '526764' "
            //            + " OR Left(CardNumber,8) = '53281675' "
            //             + " OR Left(CardNumber,8) = '53239590' "
            //              + " OR Left(CardNumber,8) = '53239524' "
            //            + ") "
            //              + " THEN '" + PRX + "302' " // CREDIT Card
            //            + " WHEN " // Debit CARD
            //            + " ( Left(CardNumber,6) = '537485' "
            //            + " OR Left(CardNumber,8) = '53239519' "
            //             + " OR Left(CardNumber,6) = '510215' "
            //                + " OR Left(CardNumber,8) = '53239513' "
            //              + " OR Left(CardNumber,8) = '51508802' "
            //            + ") "
            //            + " THEN '" + PRX + "304' " // Debit Card

            //      + " ELSE '" + PRX + "306' "
            //     + " end "
            //            ;


            //using (SqlConnection conn = new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.StatisticsEnabled = true;
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {

            //            cmd.CommandTimeout = 350;  // seconds
            //            Counter = cmd.ExecuteNonQuery();
            //            var stats = conn.RetrieveStatistics();
            //            //commandExecutionTimeInMs = (long)stats["ExecutionTime"];


            //        }
            //        // Close conn
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
            //        //stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
            //        //stpReturnCode = -1;

            //        //stpReferenceCode = stpErrorText;
            //        CatchDetails(ex);

            //        return;
            //    }


            //// Update RMCateg
            //// Initialise counter 
            //Counter = 0;

            //SQLCmd = "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //          + " SET RMCateg =  MatchingCateg "
            //          + " WHERE IsMatchingDone = 0 AND Origin <> 'Our Atms'  "
            //          ;

            //using (SqlConnection conn = new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.StatisticsEnabled = true;
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {

            //            cmd.CommandTimeout = 350;  // seconds
            //            Counter = cmd.ExecuteNonQuery();
            //            var stats = conn.RetrieveStatistics();
            //            //commandExecutionTimeInMs = (long)stats["ExecutionTime"];

            //        }
            //        // Close conn
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
                   
            //        CatchDetails(ex);

            //        //return;
            //    }

           


            //
            // UPDATE Group of ATMS
            //
            // Initialise counter 
            Counter = 0;
            //KONTO Time Taken = 98 seconds 
            // We need this. ???? Or we can do this while loading or during changing of ATM group 
            // Check this 
            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + " RMCateg = 'RECATMS-' + CAST(t2.AtmsReconcGroup As Char) "

    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
    + " INNER JOIN  [ATMS].[dbo].[ATMsFields] t2 "

    + " ON "
    + " t1.TerminalId = t2.AtmNo "
    + " WHERE  (t1.IsMatchingDone = 0  AND t1.Origin='Our Atms' ) "; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                       // commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;

                    conn.Close();

                   // stpErrorText = stpErrorText + "Cancel At Master updated with ATM Group ...";
                    CatchDetails(ex);
                    //return;
                }

            

            RRDMMatchingCategories Mc = new RRDMMatchingCategories();

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            // LOOP FOR Matching Categories
          

            int I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 
                string WMatchingCateg = (string)Mc.TableMatchingCateg.Rows[I]["Identity"];

                RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();

                Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

                //
                SQLCmd = "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                          + " SET FileId01 =  @FileId01, FileId02 =  @FileId02, FileId03 =  @FileId03, FileId04 =  @FileId04  "
                          + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg    "
                          ;

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", WMatchingCateg);

                            cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                            cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                            cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                            cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);
                            cmd.CommandTimeout = 350;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                       
                        CatchDetails(ex);

                        // return;
                    }

                I++; // Read Next entry of the table ... Next Category 
            }

            
            //
            // UPDATE GL ACCOUNTS 
            //
            RRDMAccountsClass Acc = new RRDMAccountsClass();
            
                if (PRX == "EMR")
                    Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, WCut_Off_Date);


            return; 

            //  PreStartTrxn = 0;

            // FIND THE ATMS 

           
        }


        public void ReadJournal_Txns_And_Insert_In_Pool_ROM(string InSignedId, int InSignRecordNo, string InOperator,
                                                             int InRMCycle, bool IsRomania)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;

            WOperator = InOperator;

            // WAtmsReconcGroup = InAtmsReconcGroup;

            //  WAtmNo = InAtmNo;

            //WMode = InMode; // 1 Group of ATMs
            // 2 Single ATM with zero Fuid
            // 3 Single ATM with Fuid for multithreading
            //WFuid = InFuid; // If mode = 3 then this has a value ... it gets a value when called by Alecos after Bambos is called and a Fuid is given.

            WRMCycle = InRMCycle;

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

            //WLoadedAtRMCycle = 200; 

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

            //  if (WMode == 1) Gr.InsertGroupDuringLoading(WAtmsReconcGroup);


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

            // LOAD Mpa
            int Counter = 0;

            // FILL Data Table 

            string SQLCmd = "INSERT INTO "
                         + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            + "( "

            + " [OriginFileName] " // 1
            + ",[OriginalRecordId] " // seqno // 2
            + ",[UniqueRecordId] " // 3

            // RowSelected["MatchingCateg"] = WMatchingCategoryId; // Pambos = BDC299 
            // RowSelected["FuID"] = fuid;
            // RowSelected["SeqNo01"] = WSeqNo; // Pambos = to find out  
            + ",[MatchingCateg] " // BDC299 // 4
             + ",[FuID] " // fuid // 5
              + ",[SeqNo01] " // WSeqNo // 6 

            // RowSelected["TXNSRC"] = "1";
            // RowSelected["Origin"] = "Our Atms";

            + ",[TXNSRC] " // "1" // 7.1
            + ",[TXNDEST] " // "0" // 7.2 
            + ",[Origin] " // Our ATM // 8

            // RowSelected["LoadedAtRMCycle"] = WLoadedAtRMCycle;
            // RowSelected["TerminalId"] = WAtmNo;
            // RowSelected["TransType"] = TransactionType;

            + ",[LoadedAtRMCycle] " // 9 
            + ",[TerminalId] " // 10
            + ",[TransType] " // 11

            // RowSelected["TransDescr"] = Wtrandesc;
            // RowSelected["CardNumber"] = CardNo;

            // RowSelected["AccNumber"] = AccNo;
            // RowSelected["TransCurr"] = CurrDesc;

            // RowSelected["TransAmount"] = TranAmount;
            + ",[TransDescr] " // 12
            + ",[CardNumber] " // 13

            + ",[AccNumber] " // 14
            + ",[TransCurr] " // 15 
            + ",[TransAmount] " // 16 

            // decimal DepCount = 0;

            // if (TransactionType < 20) // "WITHDRAWAL"
            // {
            //     DepCount = 0;
            // }

            // if (TransactionType > 20 & TransactionType < 30)
            // {
            //     DepCount = TranAmount;
            // }
            // RowSelected["DepCount"] = DepCount;

             + ",[DepCount] " //17 

            // RowSelected["TransDate"] = TRanDate; // Pambos: DATE AND TIME 
            // RowSelected["TraceNoWithNoEndZero"] = int.Parse(TraceNoWithNoEndZero);
            // RowSelected["AtmTraceNo"] = TraceNo;

            // RowSelected["MasterTraceNo"] = MasterTraceNo;  // Pambos = TraceNo 

            + ",[TransDate] " // 18

            + ",[TraceNoWithNoEndZero] " // 19
            + ",[AtmTraceNo] " // 20
            + ",[MasterTraceNo] " // 21

            // RowSelected["MetaExceptionId"] = 0;

            // if (PresError == "PresenterError")
            // {
            //     RowSelected["MetaExceptionId"] = 55;
            // }
            // if (SuspectDesc == "SUSPECT FOUND")
            // {
            //     RowSelected["MetaExceptionId"] = 225;
            // }
            // //RowSelected["MetaExceptionId"] = TempMetaExceptionId; // Pambos 55 for presented 225 for susp found

            + ",[MetaExceptionId] " // 22

            // RowSelected["ResponseCode"] = ResponseCode;
            // RowSelected["Operator"] = WOperator;

            + ",[ResponseCode] "  // 23 
            + ", [Operator] " //24
            + ", [UTRNNO] " //25
            + ", [AUTHNUM] " //26
             + ", [Comments] " //27

            + ") "
            //
            + " SELECT  "

              + " 'ATMs Journals' " // 1
            + ", SeqNo " // seqno // 2
           + ", NEXT VALUE FOR [RRDM_Reconciliation_ITMX].dbo.ReconcSequence " //3

            + ",@MatchingCateg " // BDC299 //4
             + ",ISNULL(fuid, 0) AS fuid " //5
              + ", SeqNo " // SeqNo01 //6

               + ",'1' " // Transaction source //7.1
               + ",'0' " // TXNDEST // 7.2 
            + ",'Our Atms' " // Our ATM //8

             + ",@LoadedAtRMCycle " //9
            + ",ISNULL(atmno, '') " //10
            + ",ISNULL(TransactionType, 0) " //11

            // + ",[TransDescr] "
            + " ,ISNULL(trandesc, '') " //12
                                        // + ",[CardNumber] "
            + " ,ISNULL(TRIM(cardnum), '') " //13
            //+ ", CardNum = replace(CardNum, 'X', '.')"
            //  + ",[AccNumber] "
            + ",case " //14
                 + " when TransactionType = 11 THEN ISNULL(acct1, '') "
                 + " when TransactionType = 33 THEN ISNULL(acct1, '') "
                 + " else ISNULL(acct2, '') "
             + "end "

            //+ ",[TransCurr] "
            + ", ISNULL(currency, '')  " //15

             // + ",[TransAmount] "
             + " ,ISNULL(CAmount, 0 )  " //16


              // + ",[DepCount] "

              + ",case " //17
                 + " when TransactionType < 20 THEN 0 "
                 + " when (TransactionType > 20 AND TransactionType < 30) THEN ISNULL(CAmount, 0 ) "
                 + " else 0 "
             + "end "

               // + ",[TransDate] " // *************************
               + " ,ISNULL(Trandatetime, '') " //18
            //  + ",[TraceNoWithNoEndZero] "
            + ", ISNULL(TraceNumber, 0)/10 " ///19
            //+ ",[AtmTraceNo] "
            + ", ISNULL(TraceNumber, 0)/10 " //20
                                             // + ",[MasterTraceNo] "
             + ", ISNULL(TraceNumber, 0)/10 " //21

             // + ",[MetaExceptionId] "

             + ",case " // 22
                        //  and TransactionType = 23
                        // + " when (PresenterError = 'PresenterError') THEN 55 "
                  + " when (PresenterError = 'PresenterError' AND TransactionType <> 23) THEN 55 "
                 + " when (SuspectDesc = 'SUSPECT FOUND') THEN 225 "
                 + " else 0 "
             + " end "
              //  + ",[ResponseCode] "
              + ",case " // 23
                 + " when (ISNULL(Source, '') = '000') THEN '0' "
                  + " when (ISNULL(Source, '') = '00') THEN '0' "
                 + " else '0' "
             + " end "

            + ", @Operator " // 24
            + ", UTRNNO " // 25
              + ", [AUTHNUM] " //26
             + ", [Comments] " //27
           + " FROM [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM]"
           + " WHERE (Result = 'OK' and TransactionType <> 99 and CAmount >0) "
           + " OR (Result = 'OK' AND TranDesc = 'REPELNISMENT' )  "
           // + " AND PROCESSED = 0 "  // Temporary comment out. 

           ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchingCateg", PRX + "203");  // default category 

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", WRMCycle);

                        //cmd.Parameters.AddWithValue("@TransDate ", MaxDt01_Minus);

                        // cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                        //  cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                        //  cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                        //  cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);

                        cmd.CommandTimeout = 1200;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();

                    //System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                    //             + "..:.." + Counter.ToString());

                }
                catch (Exception ex)
                {
                    conn.Close();

                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("Cancel While Loading Hst table AUDI:");
                    }
                    CatchDetails(ex);
                }
            //UPDATE ORIGINAL RECORDS
            if (IsRomania == true)
            {
                
                UpdateErrorsROM(WRMCycle);
                UpdatetSourceTxnsProcessedROM(WRMCycle);

                RRDM_Journal_TransactionSummary Ts = new RRDM_Journal_TransactionSummary();
                //Ts.ReadTXN_AND_CORRECT_FIGURES("");
            }
            

            return;

            //  PreStartTrxn = 0;

            // FIND THE ATMS 


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

          //  Cc.BranchId = Ac.Branch;
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
        // UPDATE Processed = true in Source Table  
        // 
        public void UpdatetSourceTxnsProcessed(int InRMCycle)
        {

            //ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxns] " 
                              + " SET Processed = 1  "
                       + " WHERE Processed = 0 ", conn))
                    {

                        //cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycle);
                        cmd.CommandTimeout = 350;  // seconds
                        rows = cmd.ExecuteNonQuery();
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
        public void UpdatetSourceTxnsProcessedROM(int InRMCycle)
        {

            //ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[tblHstAtmTxnsROM] "
                              + " SET Processed = 1  "
                       + " WHERE Processed = 0 ", conn))
                    {

                        //cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycle);
                        cmd.CommandTimeout = 350;  // seconds
                        rows = cmd.ExecuteNonQuery();
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

        public void UpdateErrorsROM(int InRMCycle)
        {

            //ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                              + " SET [ResponseCode] = '200'  "
                              + "  ,[CardNumber] = ''  "
                              + " , TransDescr = 'REPLENISHMENT'  "
                      
                       + " WHERE TransDescr = 'REPELNISMENT' ", conn))
                    {

                        //cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycle);
                        cmd.CommandTimeout = 350;  // seconds
                        rows = cmd.ExecuteNonQuery();
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



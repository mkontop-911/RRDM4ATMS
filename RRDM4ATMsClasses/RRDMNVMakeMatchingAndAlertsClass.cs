using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMNVMakeMatchingAndAlertsClass : Logger
    {
        public RRDMNVMakeMatchingAndAlertsClass() : base() { }

        public int SeqNo;
        public string Origin;
        public string StmtTrxReferenceNumber; //* 
        public string StmtExternalBankID;
        public string StmtAccountID;
        public int StmtNumber;

        public int StmtSequenceNumber;

        public DateTime StmtLineValueDate;
        public DateTime StmtLineEntryDate;

        public bool StmtLineIsReversal;
        public bool StmtLineIsDebit;

        public string StmtLineFundsCode;
        public decimal StmtLineAmt;

        public string StmtLineTrxType;
        public string StmtLineTrxCode;
        public string StmtLineRefForAccountOwner;

        public string StmtLineRefForServicingBank;
        public string StmtLineSuplementaryDetails;
        public string StmtLineRaw;

        // TRAILER
        public bool Matched;
        public int MatchedRunningCycle;
        public bool ToBeConfirmed;
        public int UniqueMatchingNo;
        public DateTime SystemMatchingDtTm;

        public string MatchedType; // USED FOR MATCHED TRANSACTIONS 

        public string UnMatchedType; // USED FOR UN-MATCHED TRANSACTIONS 

        public bool IsException;

        public int ExceptionId;
        // 12 Long standing transaction - open Dispute and sent 995 
        // 13 Difference in amount - open Dispute and sent 995  
        // 14 Fantom txn - open Dispute andsent 995 
        // 15 Transaction omited by teller - Open Dispute and send email to teller 

        public int ExceptionNo;

        public bool ActionByUser;

        public string UserId;
        public string Authoriser;

        public DateTime AuthoriserDtTm;

        public string ActionType; // 0 ... No Action Taken 
                                  // 1 ... Meta Exception Suggested By System 
                                  // 2 ... Meta Exception Manual 
                                  // 3 ... Match it with other - MANUAL at Form502aNostro
                                  // 4 ... Postpone 
                                  // 5 ... Close it 
                                  // 7 ... Default By System
                                  // 8 ... UnDo Default

        public bool SettledRecord; // Currently only for UnMatched 

        //        ADJUSTMENT 
        public bool IsAdjustment;

        public string Ccy;
        public decimal CcyRate;
        public string AdjGlAccount;
        public bool IsAdjClosed;

        public string Operator;

        // END 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string OldAccNo;

        //int TotalUnMatched;
        //int TotalDefaultMatched;
        //int TotalToBeConfirmed;


        //public int TotalMatched = 0;
        //public decimal TotalAmountMatched = 0;

        //public decimal TotalAmountUnMatched;

        //public int TotalOutstandingForAction;
        //public decimal TotalOutstandingAmountForAction;

        //public int TotalDefaultAction;
        //public decimal TotalAmountDefault;

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();
        RRDMNVRulesForMatchingClass Ru = new RRDMNVRulesForMatchingClass();

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 


        //************************************************
        //

        public DataTable TableNVStatement_Lines_Both = new DataTable();

        public DataTable TableBalTolerance = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        public DataTable TableUnMatchedEntriesExternal = new DataTable();
        int WMode;

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();
        //string WAdditionalCriteria;
        //DateTime ValueDateFrom;
        //DateTime ValueDateTo;
        //decimal AmountFrom;
        //decimal AmountTo;
        int ExternalSeqNo;
        int InternalSeqNo;
        int WUnique;

        decimal DifAmount;
        decimal AbsDifAmount;

        int DiffInValDays;
        bool ValidValueDt;
        bool ValidAmt;
        string WCriteriaForLIKE;
        string SelectionCriteria;
        string DRCR;
        string PartialRef;
        bool WToBeConfirmed;
        decimal WAdjAmt;
        string WCcy;
        decimal WCcyRate;

        string WExternalAccNo;
        string WInternalAccNo;
        string WCategoryId;
        int WRcsSeqNo;
        int WReconcCycleNo;

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        public void MatchedExternalsToInternals(string InOperator, string InSignedId,
                                              string InSubSystem, int InReconcCycleNo,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WSubSystem = InSubSystem; 

            OldAccNo = "";
            WReconcCycleNo = InReconcCycleNo;

            TableUnMatchedEntriesExternal = new DataTable();
            TableUnMatchedEntriesExternal.Clear();
           
            TotalSelected = 0;
          //
            // READ External entries not matched yet 
            // Sorted by External Account Number 
            //
                SqlString =
                    " SELECT * FROM[ATMS].[dbo].[NVStatement_Lines_External] "
                    + " Where Matched = 0  AND SubSystem = @SubSystem AND  StmtLineEntryDate <= @StmtLineEntryDate "
                    + " ORDER BY StmtExternalBankID, StmtAccountID "
                    ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                 

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SubSystem", InSubSystem);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@StmtLineEntryDate", InWDate);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableUnMatchedEntriesExternal);

                    // Close conn
                    conn.Close();


                    int I = 0;

                    while (I <= (TableUnMatchedEntriesExternal.Rows.Count - 1))
                    {

                        // For each enry in table Update records. 

                        // READ 
                        ExternalSeqNo = (int)TableUnMatchedEntriesExternal.Rows[I]["SeqNo"];

                        StmtExternalBankID = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtExternalBankID"];
                        WExternalAccNo = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtAccountID"];

                        StmtLineFundsCode = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineFundsCode"];
                        StmtLineValueDate = (DateTime)TableUnMatchedEntriesExternal.Rows[I]["StmtLineValueDate"];

                        StmtLineIsReversal = (bool)TableUnMatchedEntriesExternal.Rows[I]["StmtLineIsReversal"];

                        StmtLineIsDebit = (bool)TableUnMatchedEntriesExternal.Rows[I]["StmtLineIsDebit"];

                        StmtLineFundsCode = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineFundsCode"];
                        StmtLineAmt = (decimal)TableUnMatchedEntriesExternal.Rows[I]["StmtLineAmt"];

                        StmtLineTrxType = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineTrxType"];
                        StmtLineTrxCode = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineTrxCode"];

                        StmtLineRefForAccountOwner = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineRefForAccountOwner"];
                        StmtLineRefForServicingBank = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineRefForServicingBank"];
                        StmtLineSuplementaryDetails = (string)TableUnMatchedEntriesExternal.Rows[I]["StmtLineSuplementaryDetails"];

                        Operator = (string)TableUnMatchedEntriesExternal.Rows[I]["Operator"];

                        if (WExternalAccNo != OldAccNo)
                        {

                            Rcs.ReadReconcCategorySessionByRunningJobNoAndAccount(InOperator, WExternalAccNo, InReconcCycleNo);
                            WRcsSeqNo = Rcs.SeqNo;
                            WCategoryId = Rcs.CategoryId;
                            WInternalAccNo = Rcs.InternalAccNo;
                            WCcy = Rcs.NostroCcy;
                            WCcyRate = Rcs.NostroCcyRate;

                            OldAccNo = WExternalAccNo;
                        }

                        //TotalUnMatched = TotalUnMatched + 1;

                        StmtLineAmt = -StmtLineAmt; // Reverse sign

                        WMode = 31; // Check if the record has identical Internal unmatched
                                    // This is the case of double in External 
                                    // EG 110 Inernal and two records external 110 and 110.

                        Se.ReadNVStatement_Lines_InternalBySelectionCriteria2(WMode,
                                                                          WInternalAccNo,
                                                                          StmtLineRefForAccountOwner,
                                                                          StmtLineValueDate,
                                                                          StmtLineAmt,
                                                                          InWDate, "");

                        if (Se.RecordFound) // SUCCESS FOR ALL IDENTICAL 
                        {
                            InternalSeqNo = Se.SeqNo; // Current SeqNo  

                            // Update Footer for External and Internal 

                            WToBeConfirmed = false;
                            WAdjAmt = 0;
                            UpdateFooterForExternalAndInternal(InSignedId, ExternalSeqNo,
                                      InternalSeqNo, WToBeConfirmed,
                                      WAdjAmt, StmtLineRefForAccountOwner);

                        }

                        if (Se.RecordFound == false) // First Failure 
                        {

                            if (StmtLineIsDebit == true)
                        {
                            DRCR = "DR";
                        }
                        else DRCR = "CR";

                        // Check if tolerance in either Amt or dates 
                        Ru.ReadRuleForTxnCode(InSignedId, InOperator,
                                     StmtExternalBankID, WExternalAccNo,
                                     StmtLineTrxCode, DRCR);

                        if (Ru.RecordFound == true)
                        {
                            //WAmtTolerance = false;
                            //WValDtTolerance = false;
                            //WValueDaysBefore = 0;
                            //WValueDaysAfter = 0;
                            //WPartialReference = false;
                            //WPartialRefLength = 0;
                        }

                        if (Ru.WAmtTolerance == true || Ru.WValDtTolerance == true)
                        {

                            WMode = 32; // Search Reference For full Search Reference Number 
                            SelectionCriteria = " AND StmtLineRefForAccountOwner ='" + StmtLineRefForAccountOwner + "'";
                            Se.ReadNVStatement_Lines_InternalBySelectionCriteria2(WMode,
                                                                        WInternalAccNo,
                                                                        StmtLineRefForAccountOwner,
                                                                        StmtLineValueDate,
                                                                        StmtLineAmt,
                                                                        InWDate, SelectionCriteria);

                            if (Se.RecordFound == true)
                            {
                                InternalSeqNo = Se.SeqNo; // Current SeqNo  
                                                          //WMatchingType = "SystemDefault"; 
                                MakeFurtherMatching(InSignedId, InOperator, StmtExternalBankID, WExternalAccNo);
                            }
                            else
                            {
                                // Try Partial 
                                int RefLength = StmtLineRefForAccountOwner.Length;
                                if (Ru.WPartialReference == true)
                                {
                                    // Find Partial 
                                    int K = 0;
                                    while ((K + Ru.WPartialRefLength) <= RefLength)
                                    {
                                        PartialRef = StmtLineRefForAccountOwner.Substring(K, Ru.WPartialRefLength);

                                        WMode = 32; // Search Reference For LIKE PARTIAL Search 
                                        WCriteriaForLIKE = " AND StmtLineRefForAccountOwner LIKE '%" + PartialRef + "%'";
                                        Se.ReadNVStatement_Lines_InternalBySelectionCriteria2(WMode,
                                                                                    WInternalAccNo,
                                                                                    StmtLineRefForAccountOwner,
                                                                                    StmtLineValueDate,
                                                                                    StmtLineAmt,
                                                                                    InWDate, WCriteriaForLIKE);
                                        if (Se.RecordFound == true & Se.NumberOfRecords == 1)
                                        {
                                            InternalSeqNo = Se.SeqNo; // Current SeqNo  
                                                                      // Make job 
                                            MakeFurtherMatching(InSignedId, InOperator, StmtExternalBankID, WExternalAccNo);
                                            K = 16;
                                        }

                                        K++;
                                    }
                                }
                            }
                        }

                    }

                        I++; // Read Next entry of the table 

                    }

                    // LAST UPDATING 
                    if (I == (TableUnMatchedEntriesExternal.Rows.Count))
                    {
                        //TotalRemainedUnMatched = TotalUnMatched - (TotalDefaultMatched + TotalToBeConfirmed);

                        //// Update Reconciliation Category Session
                        ////
                        //Rcs.UpdateReconcCategorySessionWithAutomaticMatchTotals(WRcsSeqNo,
                        //    TotalDefaultMatched, TotalToBeConfirmed, TotalRemainedUnMatched, TotalRemainedUnMatched);
                    }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        string WSubSystem; 
        public void IndentifyAndUpdateAlertsInBothInternalAndExternal(string InOperator, string InSignedId,
                                            string InSubSystem, int InReconcCycleNo,
                                            DateTime InWDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WSubSystem = InSubSystem; 

            OldAccNo = "";
            WReconcCycleNo = InReconcCycleNo;

            TableNVStatement_Lines_Both = new DataTable();
            TableNVStatement_Lines_Both.Clear();

            TotalSelected = 0;
          
            //if (InMode == 71) // CARDS 
            //{
                SqlString =
                  " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                + " WHERE  SubSystem = @SubSystem "
                + " AND [Matched] = 0  AND StmtLineEntryDate <= @StmtLineEntryDate "
                + " UNION ALL"
                + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                + " WHERE  SubSystem =  @SubSystem "
                + " AND [Matched] = 0 AND StmtLineEntryDate <= @StmtLineEntryDate "
                + " ORDER BY StmtExternalBankID, Ccy, Origin  ASC";
            //}
            //if (InMode == 72) // NOSTRO 
            //{
            //    SqlString =
            //      " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
            //    + " WHERE   SubSystem = 'NostroReconciliation' "
            //    + " AND [Matched] = 0  AND StmtLineEntryDate <= @StmtLineEntryDate "
            //    + " UNION ALL"
            //    + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
            //    + " WHERE  SubSystem = 'NostroReconciliation' "
            //    + " AND [Matched] = 0 AND StmtLineEntryDate <= @StmtLineEntryDate "
            //    + " ORDER BY StmtExternalBankID, Ccy, Origin  ASC";
            //}

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SubSystem", InSubSystem);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@StmtLineEntryDate", InWDate);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableNVStatement_Lines_Both);

                    // Close conn
                    conn.Close();

                    int I = 0;

                    while (I <= (TableNVStatement_Lines_Both.Rows.Count - 1))
                    {

                        // For each enry in table Update records. 

                        // READ 
                        SeqNo = (int)TableNVStatement_Lines_Both.Rows[I]["SeqNo"];

                        Origin = (string)TableNVStatement_Lines_Both.Rows[I]["Origin"];

                        StmtLineEntryDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["StmtLineEntryDate"];

                        StmtExternalBankID = (string)TableNVStatement_Lines_Both.Rows[I]["StmtExternalBankID"];
                        StmtAccountID = (string)TableNVStatement_Lines_Both.Rows[I]["StmtAccountID"];

                        StmtLineAmt = (decimal)TableNVStatement_Lines_Both.Rows[I]["StmtLineAmt"];

                        decimal AbsAmount = Math.Abs(StmtLineAmt);
                        int DiffInEntryDays = Convert.ToInt32((StmtLineEntryDate - InWDate).TotalDays);
                        int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

                        // Call and check Alerts

                        RRDMNVAlerts Ale = new RRDMNVAlerts();

                        if (Origin == "INTERNAL")
                        {
                            Ale.ReadNVFindDaysInRangeInternal(InOperator,
                                                StmtAccountID, StmtExternalBankID,
                                                AbsAmount);
                            if (Ale.RecordFound == true)
                            {
                                // Update Trailer 
                                if (AbsDiffInEntryDays >= Ale.LimitDays)
                                {
                                    // Update External
                                    // By ExternalSeqNo  
                                    SelectionCriteria = " WHERE SeqNo =" + SeqNo;

                                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                                    Se.IsException = true;
                                    // 88 Alert - Long Standing Transaction  
                                    Se.ExceptionId = 88; // Long standing transaction 
                                    // Open Dispute 
                                    // Update Se.ExceptionNo 
                                    Se.UpdateInternalFooter(Operator, SelectionCriteria);

                                    //System.Windows.Forms.MessageBox.Show("There is ALert");
                                }
                            }
                        }

                        if (Origin == "EXTERNAL")
                        {
                            Ale.ReadNVFindDaysInRangeExternal(InOperator,
                                                 StmtAccountID, StmtExternalBankID,
                                                 AbsAmount);
                            if (Ale.RecordFound == true)
                            {
                                // Update Trailer 
                                if (AbsDiffInEntryDays >= Ale.LimitDays)
                                {
                                    // Update External
                                    // By ExternalSeqNo  
                                    SelectionCriteria = " WHERE SeqNo =" + SeqNo;

                                    Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                                    Se.IsException = true;
                                    Se.ExceptionId = 12; // Long standing transaction 
                                    // Open Dispute 
                                    // Update Se.ExceptionNo 
                                    Se.UpdateExternalFooter(Operator, SelectionCriteria);

                                    System.Windows.Forms.MessageBox.Show("There is ALert");
                                }
                            }
                        }

                        I++; // Read Next entry of the table 

                    }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        //
        // Methods 
        // 
        // (InSignedId, InOperator,
        //InExternalBankID, InExternalAccNo,
        string WMatchingType;
        int WSeqNo;
        bool AmtWithDiff;
        bool ValDtWithDiff;
        public void MakeFurtherMatching(string InSignedId, string InOperator,
                                   string InExternalBankID, string InExternalAccNo)
        {
            try
            {
                DifAmount = StmtLineAmt - Se.StmtLineAmt;
                AbsDifAmount = Math.Abs(DifAmount);
                DiffInValDays = Convert.ToInt32((StmtLineValueDate - Se.StmtLineValueDate).TotalDays);
                int AbsDiffInValDays = Math.Abs(DiffInValDays);

                if (DiffInValDays == 0)
                {
                    ValidValueDt = true;
                    ValDtWithDiff = false;
                }
                else
                {
                    ValidValueDt = false;
                    ValDtWithDiff = true;
                }

                if (AbsDifAmount == 0)
                {
                    ValidAmt = true;
                    AmtWithDiff = false;
                }
                else
                {
                    ValidAmt = false;
                    AmtWithDiff = true;
                }


                if (AbsDifAmount != 0 & Ru.WAmtTolerance == true)
                {
                    // Valid searching 
                    // Look what Range it is 
                    // Check if tolerance in either Amt or dates 
                    Ru.ReadRuleForTxnCodeAndAmtRange(InSignedId, InOperator,
                                 InExternalBankID, InExternalAccNo,
                                 StmtLineTrxCode, DRCR, AbsDifAmount);

                    if (Ru.RecordFound == true)
                    {
                        WSeqNo = Ru.SeqNo;
                        WMatchingType = Ru.MatchedType;
                        ValidAmt = true;
                        // Update Ru.WValueDaysAfter or before 
                    }

                }

                if (DiffInValDays != 0 & Ru.WValDtTolerance == true & ValidAmt == true)
                {
                    // check if within Acceptable 
                    // + Days After
                    // - Days Before
                    if (AbsDifAmount == 0)
                    {
                        Ru.ReadRuleForTxnCodeAndValueDateDiff(InSignedId, InOperator,
                               InExternalBankID, InExternalAccNo,
                               StmtLineTrxCode, DRCR, AbsDiffInValDays, DiffInValDays);

                        if (Ru.RecordFound == true)
                        {
                            WSeqNo = Ru.SeqNo;
                            WMatchingType = Ru.MatchedType;
                            ValidValueDt = true;
                            // Update Ru.WValueDaysAfter or before 
                        }
                    }
                    else
                    {
                        if (DiffInValDays > 0)
                        {
                            if (DiffInValDays <= Ru.WValueDaysAfter)
                            {
                                // OK 
                                ValidValueDt = true;
                            }
                            else
                            {
                                ValidValueDt = false;
                                // NOT OK 
                            }
                        }
                        if (DiffInValDays < 0)
                        {
                            if ((-DiffInValDays) <= Ru.WValueDaysBefore)
                            {
                                // OK 
                                ValidValueDt = true;
                            }
                            else
                            {
                                // NOT OK 
                                ValidValueDt = false;
                            }
                        }
                    }

                }

                if (ValidAmt == true & ValidValueDt == true)
                {
                    // Find Matched type based on the Amt and Value in difference 
                    if (AmtWithDiff == true)
                    {
                        // Used WMatchingType Or find from associated SeqNo 
                    }
                    if (AmtWithDiff == false & ValDtWithDiff == false)
                    {
                        // Used Default action 
                    }
                    if (AmtWithDiff == false & ValDtWithDiff == true)
                    {
                        // ????? Maybe we get associated SeqNo 
                    }
                    // Match transactions based on Matching Type 
                    // Create Adjsustment entry 

                    //    Ru.MatchedType = "SystemDefault";              
                    //    Ru.MatchedType = "ToBeConfirmed";    
                    //    Ru.MatchedType = "OnlyManual";

                    if (WMatchingType == "SystemDefault")
                    {
                        WToBeConfirmed = false;
                    }
                    if (WMatchingType == "AutoButToBeConfirmed")
                    {
                        WToBeConfirmed = true;
                    }

                    WAdjAmt = DifAmount;
                    UpdateFooterForExternalAndInternal(InSignedId, ExternalSeqNo,
                              InternalSeqNo, WToBeConfirmed,
                              WAdjAmt, PartialRef
                              );
                }

            }
            catch (Exception ex)
            {


                CatchDetails(ex);
            }
        }
        // UpdateFooterForExternalAndInternal(InSignedId, 
        //                                    ExternalSeqNo, InternalSeqNo, WToBeConfirmed);   
        public void UpdateFooterForExternalAndInternal(string InSignedId, int InExternalSeqNo,
                                      int InInternalSeqNo, bool InWToBeConfirmed,
                                      decimal InAmt, string InReference)
        {
            try
            {
                // Get Unique Id 
                WUnique = Gu.GetNextValue();

                // Update External
                // By ExternalSeqNo  
                SelectionCriteria = " WHERE SeqNo =" + InExternalSeqNo;

                Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

                Se.Matched = true;
                Se.MatchedRunningCycle = WReconcCycleNo;

                if (InWToBeConfirmed == false)
                {
                    Se.ToBeConfirmed = false;
                    Se.ActionType = "0";
                    Se.MatchedType = "SystemDefault";

                    Se.UniqueMatchingNo = WUnique;

                    Se.UserId = InSignedId;

                    Se.SettledRecord = true;
                }
                if (InWToBeConfirmed == true)
                {
                    Se.ToBeConfirmed = true;
                    Se.ActionType = "0";
                    Se.MatchedType = "AutoButToBeConfirmed";

                    //TotalToBeConfirmed = TotalToBeConfirmed + 1;

                    Se.UniqueMatchingNo = WUnique;

                    Se.UserId = InSignedId;

                    Se.SettledRecord = false;
                }

                Se.UpdateExternalFooter(Operator, SelectionCriteria);
                // Update Internal
                // By InternalSeqNo
                SelectionCriteria = " WHERE SeqNo =" + InInternalSeqNo;

                Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                Se.Matched = true;
                Se.MatchedRunningCycle = WReconcCycleNo;

                if (InWToBeConfirmed == false)
                {
                    Se.ToBeConfirmed = false;
                    Se.ActionType = "0";
                    Se.MatchedType = "SystemDefault";

                    Se.UniqueMatchingNo = WUnique;

                    Se.UserId = InSignedId;

                    Se.SettledRecord = true;
                }
                if (InWToBeConfirmed == true)
                {
                    Se.ToBeConfirmed = true;
                    Se.ActionType = "0";
                    Se.MatchedType = "AutoButToBeConfirmed";

                    Se.UniqueMatchingNo = WUnique;

                    Se.UserId = InSignedId;

                    Se.SettledRecord = false;
                }

                Se.UpdateInternalFooter(Operator, SelectionCriteria);

                // Make Transaction
                if (InAmt != 0)
                {
                    SelectionCriteria = " WHERE SeqNo =" + InInternalSeqNo;

                    Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

                    //Se.StmtAccountID = InternalAcc;
                    Se.Origin = "WAdjustment";

                    Se.SubSystem = WSubSystem; 

                    Se.ActionType = "0"; // 

                    Se.StmtLineAmt = InAmt;

                    Se.CcyRate = 0;

                    if (Se.StmtLineAmt > 0)
                    {
                        Se.StmtLineIsDebit = true;
                    }

                    if (Se.StmtLineAmt < 0)
                    {
                        Se.StmtLineIsDebit = false;
                    }

                    Se.StmtLineSuplementaryDetails = InReference;

                    Se.IsAdjustment = true;
                    Se.Ccy = WCcy;

                    Se.CcyRate = WCcyRate;
                    Se.AdjGlAccount = "ProfitLoss";
                    Se.IsAdjClosed = false;

                    int InsertedId = Se.InsertNewRecordInInternalStatement();
                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex); 
            }
        }



    }
}



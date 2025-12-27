using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMNVMakeMatchingAndAlertsClass_Cards : Logger
    {
        public RRDMNVMakeMatchingAndAlertsClass_Cards() : base() { }

        ///
        /// Header 
        ///
        public int SeqNo;
        public string Origin;
        public string StmtTrxReferenceNumber; //* 
        public string StmtExternalBankID;
        public string StmtAccountID;
        public int StmtNumber;

        public int StmtSequenceNumber;
        /// <summary>
        /// Main Body 
        /// </summary>
        // 1.Txn Info
        public string MerchantId;
        public string MerchantName;
        public string TypeOfTerminal;
        public string TerminalId;
        public DateTime TxnEntryDate;
        public DateTime TxnValueDate;
        public string TxnCode;
        public string TxnDesc;
        // 2.Matching Fields 
        public string RRN;
        public decimal TxnAmt; // Say 110
        public string TxnCcy; // Say Stg       
        // 3.Local equivalent 
        public decimal TxnCcyRate; // Say 1.1434
        public decimal InternalTxtAmtCharged;
        // 4.Local Recalculation At Settlement Received Date
        public DateTime SettlementReceivedDate;
        public decimal NewTxnCcyRate;
        public decimal NewInternalTxtAmt;

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
        RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();
       
        RRDMNVRulesForMatchingClass Ru = new RRDMNVRulesForMatchingClass();

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable TableNVStatement_Lines_Both = new DataTable();

        public DataTable TableBalTolerance = new DataTable();

        public int TotalSelected;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

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
           
                SqlString =
                    " SELECT * FROM[ATMS].[dbo].[NVCards_External_File_Setllement] "
                    + " Where Matched = 0  AND SubSystem = @SubSystem AND  TxnEntryDate <= @TxnEntryDate "
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
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@TxnEntryDate", InWDate);

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

                        TxnValueDate = (DateTime)TableUnMatchedEntriesExternal.Rows[I]["TxnValueDate"];

                        TxnCode = (string)TableUnMatchedEntriesExternal.Rows[I]["TxnCode"];

                        TxnAmt = (decimal)TableUnMatchedEntriesExternal.Rows[I]["TxnAmt"];

                        RRN = (string)TableUnMatchedEntriesExternal.Rows[I]["RRN"];
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

                        TxnAmt = -TxnAmt; // Reverse sign

                  
                        if (TxnCode == "11" || TxnCode == "12" || TxnCode == "13")
                        {
                            DRCR = "DR";
                        }
                        else DRCR = "CR";

                        // Check if tolerance in either Amt or dates 
                        Ru.ReadRuleForTxnCode(InSignedId, InOperator,
                                     StmtExternalBankID, WExternalAccNo,
                                     TxnCode, DRCR);

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
                            SelectionCriteria = " AND RRN ='" + RRN + "'";
                            Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria2(WMode,
                                                                        WInternalAccNo,
                                                                        RRN,
                                                                        TxnValueDate,
                                                                        TxnAmt,
                                                                        InWDate, SelectionCriteria);

                            if (Sec.RecordFound == true)
                            {
                                InternalSeqNo = Sec.SeqNo; // Current SeqNo  
                                                          //WMatchingType = "SystemDefault"; 
                                MakeFurtherMatching(InSignedId, InOperator, StmtExternalBankID, WExternalAccNo);
                            }
                            else
                            {
                                // Try Partial 
                                int RefLength = RRN.Length;
                                if (Ru.WPartialReference == true)
                                {
                                    // Find Partial 
                                    int K = 0;
                                    while ((K + Ru.WPartialRefLength) <= RefLength)
                                    {
                                        PartialRef = RRN.Substring(K, Ru.WPartialRefLength);

                                        WMode = 32; // Search Reference For LIKE PARTIAL Search 
                                        WCriteriaForLIKE = " AND RRN LIKE '%" + PartialRef + "%'";
                                        Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria2(WMode,
                                                                                    WInternalAccNo,
                                                                                    RRN,
                                                                                    TxnValueDate,
                                                                                    TxnAmt,
                                                                                    InWDate, WCriteriaForLIKE);
                                        if (Sec.RecordFound == true & Sec.NumberOfRecords == 1)
                                        {
                                            InternalSeqNo = Sec.SeqNo; // Current SeqNo  
                                                                      // Make job 
                                            MakeFurtherMatching(InSignedId, InOperator, StmtExternalBankID, WExternalAccNo);
                                            K = 16;
                                        }

                                        K++;
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
         
                SqlString =
                  " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                + " WHERE  SubSystem = @SubSystem "
                + " AND [Matched] = 0  AND TxnEntryDate <= @TxnEntryDate "
                + " UNION ALL"
                + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                + " WHERE  SubSystem =  @SubSystem "
                + " AND [Matched] = 0 AND TxnEntryDate <= @TxnEntryDate "
                + " ORDER BY StmtExternalBankID, Origin  ASC";
          

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                 
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@SubSystem", InSubSystem);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@TxnEntryDate", InWDate);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableNVStatement_Lines_Both);

                    // Close conn
                    conn.Close();


                    int I = 0;

                    while (I <= (TableNVStatement_Lines_Both.Rows.Count - 1))
                    {

                        // For each entry in table Update records. 

                        // READ 
                        SeqNo = (int)TableNVStatement_Lines_Both.Rows[I]["SeqNo"];

                        Origin = (string)TableNVStatement_Lines_Both.Rows[I]["Origin"];

                        TxnEntryDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["TxnEntryDate"];

                        StmtExternalBankID = (string)TableNVStatement_Lines_Both.Rows[I]["StmtExternalBankID"];
                        StmtAccountID = (string)TableNVStatement_Lines_Both.Rows[I]["StmtAccountID"];

                        TxnAmt = (decimal)TableNVStatement_Lines_Both.Rows[I]["TxnAmt"];

                        Operator = (string)TableNVStatement_Lines_Both.Rows[I]["Operator"];

                        decimal AbsAmount = Math.Abs(TxnAmt);
                        int DiffInEntryDays = Convert.ToInt32((TxnEntryDate - InWDate).TotalDays);
                        int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

                        // Call and check Alerts

                        RRDMGasParameters Gp = new RRDMGasParameters();

                        Gp.ReadParametersSpecificId(Operator, "452", "1", "", "");
                        int Temp = ((int)Gp.Amount);

                        if (AbsDiffInEntryDays > Temp)
                        {
                            if (Origin == "INTERNAL")
                            {
                                // Update External
                                // By ExternalSeqNo  
                                SelectionCriteria = " WHERE SeqNo =" + SeqNo;

                                Sec.ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);

                                Sec.IsException = true;
                                // 88 Alert - Long Standing Transaction  
                                Sec.ExceptionId = 88; // Long standing transaction 

                                // Update Se.ExceptionNo 
                                Sec.UpdateInternalFooter(Operator, SelectionCriteria);
                            }

                            if (Origin == "EXTERNAL")
                            {
                                // Update External
                                // By ExternalSeqNo  
                                SelectionCriteria = " WHERE SeqNo =" + SeqNo;

                                Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

                                Sec.IsException = true;
                                Sec.ExceptionId = 12; // Long standing transaction 

                                // Update Se.ExceptionNo 
                                Sec.UpdateExternalFooter(Operator, SelectionCriteria);

                          //      System.Windows.Forms.MessageBox.Show("There is ALert");

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
                DifAmount = TxnAmt - Sec.TxnAmt;
                AbsDifAmount = Math.Abs(DifAmount);
                DiffInValDays = Convert.ToInt32((TxnValueDate - Sec.TxnValueDate).TotalDays);
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
                                 TxnCode, DRCR, AbsDifAmount);

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
                               TxnCode, DRCR, AbsDiffInValDays, DiffInValDays);

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

                Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

                Sec.Matched = true;
                Sec.MatchedRunningCycle = WReconcCycleNo;

                if (InWToBeConfirmed == false)
                {
                    Sec.ToBeConfirmed = false;
                    Sec.ActionType = "0";
                    Sec.MatchedType = "SystemDefault";

                    Sec.UniqueMatchingNo = WUnique;

                    Sec.UserId = InSignedId;

                    Sec.SettledRecord = true;
                }
                if (InWToBeConfirmed == true)
                {
                    Sec.ToBeConfirmed = true;
                    Sec.ActionType = "0";
                    Sec.MatchedType = "AutoButToBeConfirmed";

                    //TotalToBeConfirmed = TotalToBeConfirmed + 1;

                    Sec.UniqueMatchingNo = WUnique;

                    Sec.UserId = InSignedId;

                    Sec.SettledRecord = false;
                }

                Sec.UpdateExternalFooter(Operator, SelectionCriteria);
                // Update Internal
                // By InternalSeqNo
                SelectionCriteria = " WHERE SeqNo =" + InInternalSeqNo;

                Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

                Sec.Matched = true;
                Sec.MatchedRunningCycle = WReconcCycleNo;

                if (InWToBeConfirmed == false)
                {
                    Sec.ToBeConfirmed = false;
                    Sec.ActionType = "0";
                    Sec.MatchedType = "SystemDefault";

                    Sec.UniqueMatchingNo = WUnique;

                    Sec.UserId = InSignedId;

                    Sec.SettledRecord = true;
                }
                if (InWToBeConfirmed == true)
                {
                    Sec.ToBeConfirmed = true;
                    Sec.ActionType = "0";
                    Sec.MatchedType = "AutoButToBeConfirmed";

                    Sec.UniqueMatchingNo = WUnique;

                    Sec.UserId = InSignedId;

                    Sec.SettledRecord = false;
                }

                Sec.UpdateInternalFooter(Operator, SelectionCriteria);

                // Make Transaction
                if (InAmt != 0)
                {
                    SelectionCriteria = " WHERE SeqNo =" + InInternalSeqNo;

                    Sec.ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);

                    //Se.StmtAccountID = InternalAcc;
                    Sec.Origin = "WAdjustment";

                    Sec.SubSystem = WSubSystem; 

                    Sec.ActionType = "0"; // 

                    Sec.TxnAmt = InAmt;

                    //Sec.CcyRate = 0;

                    if (Sec.TxnAmt > 0)
                    {
                        Sec.TxnCode = "21";
                    }

                    if (Sec.TxnAmt < 0)
                    {
                        Sec.TxnCode = "11";
                    }

                    Sec.StmtLineSuplementaryDetails = InReference;

                    Sec.IsAdjustment = true;
                    Sec.TxnCcy = WCcy;

                    //Sec.CcyRate = WCcyRate;
                    Sec.AdjGlAccount = "ProfitLoss";
                    Sec.IsAdjClosed = false;

                    int InsertedId = Sec.InsertNewRecordInInternalStatement();
                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex); 
            }
        }


    }
}





using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMNVCardsBothAuthorAndSettlement : Logger
    {
        public RRDMNVCardsBothAuthorAndSettlement() : base() { }
        ///
        /// Header 
        ///
        public int SeqNo;
        public string Origin;
        public string CategoryId;
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
        // 4.Local Recalculation Every Time is Matching is made 
        public decimal NewTxnCcyRate;
        public decimal NewInternalTxtAmt;

        public string StmtLineSuplementaryDetails;
        public string StmtLineRaw;

        public string PCODE;

        // SOURCE 

        public string SubSystem;
        public string CardNo;
        public string AccNo; // Charged Acc
        public bool IsLocalCcyAcc;

        // TRAILER
        public bool Matched;
        public int MatchedRunningCycle;
        public bool ToBeConfirmed;
        public int UniqueMatchingNo;
        public DateTime SystemMatchingDtTm;

        public string MatchedType; // USED FOR MATCHED TRANSACTIONS 
                                   // "SystemDefault"       
                                   // "AutoButToBeConfirmed"
                                   // "ManualToBeConfirmed"

        public string UnMatchedType; // USED FOR UN-MATCHED TRANSACTIONS 

        public bool IsException;
        public int ExceptionId; // This will be related with MT995 code or internal to the bank code 
        // 01 We appear not to have debited so far
        // 02 We appear not to have credited so far  
        // 06 This transaction does not appear in your statement of account 
        // 47 Difference in amount - open Dispute and sent 995  
        // 12 Difference in Value Date 
        // 14 Fantom txn - open Dispute and sent 995 
        // 15 Transaction omited by teller - Open Dispute and send email to teller
        // 88 Alert - Long Standing Transaction  
        public int ExceptionNo; // eg Dispute Number 

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

        public string AdjGlAccount;
        public bool IsAdjClosed;

        public string Operator;

        // END 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        string SQLStmtInternalLines = "[ATMS].[dbo].[NVCards_Internal_File_Authorisation]";

        //************************************************
        //

        public DataTable TableNVCards_Records_Both = new DataTable();

        public DataTable TableNVCards_Records_Single = new DataTable();

        public DataTable TableManyToManySummary = new DataTable();

        public DataTable TableManyToManyTxns = new DataTable();

        public DataTable TableAgingAnalysis = new DataTable();

        public DataTable TableBranchesTotals = new DataTable();

        public int TotalSelected;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Reader Fields 
        private void GetReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            Origin = (string)rdr["Origin"];
            CategoryId = (string)rdr["CategoryId"];
            StmtTrxReferenceNumber = (string)rdr["StmtTrxReferenceNumber"];

            StmtExternalBankID = (string)rdr["StmtExternalBankID"];
            StmtAccountID = (string)rdr["StmtAccountID"];
            StmtNumber = (int)rdr["StmtNumber"];

            StmtSequenceNumber = (int)rdr["StmtSequenceNumber"];

            MerchantId = (string)rdr["MerchantId"];
            MerchantName = (string)rdr["MerchantName"];
            TypeOfTerminal = (string)rdr["TypeOfTerminal"];
            TerminalId = (string)rdr["TerminalId"];

            TxnEntryDate = (DateTime)rdr["TxnEntryDate"];
            TxnValueDate = (DateTime)rdr["TxnValueDate"];

            TxnCode = (string)rdr["TxnCode"];
            TxnDesc = (string)rdr["TxnDesc"];
            RRN = (string)rdr["RRN"];

            TxnAmt = (decimal)rdr["TxnAmt"];

            TxnCcy = (string)rdr["TxnCcy"];

            TxnCcyRate = (decimal)rdr["TxnCcyRate"];
            InternalTxtAmtCharged = (decimal)rdr["InternalTxtAmtCharged"];

            NewTxnCcyRate = (decimal)rdr["NewTxnCcyRate"];
            NewInternalTxtAmt = (decimal)rdr["NewInternalTxtAmt"];

            StmtLineSuplementaryDetails = (string)rdr["StmtLineSuplementaryDetails"];
            StmtLineRaw = (string)rdr["StmtLineRaw"];

            PCODE = (string)rdr["PCODE"];

            SubSystem = (string)rdr["SubSystem"];
            CardNo = (string)rdr["CardNo"];
            AccNo = (string)rdr["AccNo"];
            IsLocalCcyAcc = (bool)rdr["IsLocalCcyAcc"];

            Matched = (bool)rdr["Matched"];
            MatchedRunningCycle = (int)rdr["MatchedRunningCycle"];

            ToBeConfirmed = (bool)rdr["ToBeConfirmed"];
            UniqueMatchingNo = (int)rdr["UniqueMatchingNo"];

            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

            MatchedType = (string)rdr["MatchedType"];

            UnMatchedType = (string)rdr["UnMatchedType"];

            IsException = (bool)rdr["IsException"];
            ExceptionId = (int)rdr["ExceptionId"];
            ExceptionNo = (int)rdr["ExceptionNo"];

            ActionByUser = (bool)rdr["ActionByUser"];
            UserId = (string)rdr["UserId"];
            Authoriser = (string)rdr["Authoriser"];

            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

            ActionType = (string)rdr["ActionType"];

            SettledRecord = (bool)rdr["SettledRecord"];

            IsAdjustment = (bool)rdr["IsAdjustment"];

            AdjGlAccount = (string)rdr["AdjGlAccount"];
            IsAdjClosed = (bool)rdr["IsAdjClosed"];

            Operator = (string)rdr["Operator"];
        }
        //
        // Methods 
        // READ EXTERNAL
        // 
        //
        public void ReadNVCards_External_File_SetllementBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                      + " "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            GetReaderFields(rdr);

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



        // Methods 
        // READ Specific by UniqueRecordId
        // INTERNAL 
        //
        public void ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                      + " "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            GetReaderFields(rdr);

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
        // Methods 
        // READ Specific by Selection both (merge files)
        // 
        //
        public void ReadNVFromBothVisaAutherAndSettlementBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
             + InSelectionCriteria
             + " UNION ALL"
             + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
             + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            GetReaderFields(rdr);

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
        // Methods 
        // READ Range ... DATES 
        // FILL UP A TABLE LEFT 
        //
        int OldNumber = 0;
        //bool NewLine;
        int ColorNo;

        public int OutstandingAlerts;
        public int OutstandingDisputes;

        public void ReadNVCardRecordsForBothByMode(string InOperator, string InSignedId, string InSubSystem, int InMode,
                                           int InRunningJobNo, string InExternalAccno, string InternalAccNo,
                                           string InOrderCriteria, DateTime InTxnEntryDate, string InReference)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // InSubSystem
            // InSubSystem =="CardsSettlement"
            // InSubSystem == "NostroReconciliation"

            OutstandingAlerts = 0;
            OutstandingDisputes = 0;

            // PAIR ... PAIR .... PAIR 
            // If InMode = 1 Read Matched for this Running Cycle  for Pair
            // If InMode = 2 Read Matched To be confirmed all Running Cycles for this Pair
            // if InMode = 3 Read all Unmatched  all Running Cycles with Action Type = 0 (No working date)
            // if InMode = 4 Read all Unmatched  with action type = 3 
            // If (InMode == 5) // Unmatched less or equal working date for pair 
            // if (InMode == 6) // Unmatched with reference like
            // if (InMode == 7) // Read All with IsAdjustment = true for this account 
            // if (InMode == 57) // Read All with IsAdjustment = true for all Internal Accounts 
            // if (InMode == 8) // Read All with IsAdjustment = true and with Action Type = 3 

            // CYCLE  .. CYCLE 
            // if (InMode == 9)  // Read All Matched this Cycle 
            // if (InMode == 10) // Read All to be confirmed this Cycle (not only pair) 
            // if (InMode == 11) // Read All UNmatched  this Cycle (not only pair) 
            // if (InMode == 12) // Read All UNmatched  this Cycle (not only pair) with Alerts 

            // ALL
            // if (InMode == 15) // Read All Matched with reference like 
            // 

            OldNumber = 0;
            //NewLine = false;
            ColorNo = 11;

            // Find Matching Category Id


            TableNVCards_Records_Both = new DataTable();
            TableNVCards_Records_Both.Clear();
            TotalSelected = 0;

            // VISA 
            if (InSubSystem == "CardsSettlement" || InSubSystem == "E_FINANCE")
            {
                // DATA TABLE ROWS DEFINITION 
                TableNVCards_Records_Both.Columns.Add("SeqNo", typeof(int));
                TableNVCards_Records_Both.Columns.Add("ColorNo", typeof(int));
                TableNVCards_Records_Both.Columns.Add("CategoryId", typeof(string));
                TableNVCards_Records_Both.Columns.Add("MatchingNo", typeof(int));
                TableNVCards_Records_Both.Columns.Add("MatchedType", typeof(string));

                TableNVCards_Records_Both.Columns.Add("Origin", typeof(string));
                TableNVCards_Records_Both.Columns.Add("CardNo", typeof(string)); // 

                TableNVCards_Records_Both.Columns.Add("DONE", typeof(string));

                TableNVCards_Records_Both.Columns.Add("Code", typeof(string));
                //TableNVCards_Records_Both.Columns.Add("ValueDate", typeof(DateTime)); // 
                TableNVCards_Records_Both.Columns.Add("EntryDate", typeof(DateTime));
                TableNVCards_Records_Both.Columns.Add("DR/CR", typeof(string));
                TableNVCards_Records_Both.Columns.Add("Amt", typeof(decimal)); // 
                TableNVCards_Records_Both.Columns.Add("Ccy", typeof(string));

                TableNVCards_Records_Both.Columns.Add("RRN", typeof(string)); // 

                TableNVCards_Records_Both.Columns.Add("OtherDetails", typeof(string));

                TableNVCards_Records_Both.Columns.Add("MerchantId", typeof(string));
                TableNVCards_Records_Both.Columns.Add("MerchantName", typeof(string)); // 

                TableNVCards_Records_Both.Columns.Add("TermId", typeof(string)); // 

                TableNVCards_Records_Both.Columns.Add("GLAccount", typeof(string));
                TableNVCards_Records_Both.Columns.Add("AccNo", typeof(string)); // 

            }

            if (InMode == 1) // Matched 
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND StmtAccountID = @StmtAccountID "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE[MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND StmtAccountID = @InternalAccNo "
                   + " AND ((Origin = 'INTERNAL' AND Matched = 1 AND SettledRecord = 1 ) OR (Origin = 'WAdjustment' AND [Matched] = 1 AND [SettledRecord] = 1 AND IsAdjClosed = 0)) "
                   + " ORDER BY CategoryId, [UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 2) // To be confirmed this pair 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @StmtAccountID  AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " ORDER BY CategoryId, [UniqueMatchingNo], Origin ASC";
            }
            if (InMode == 3) // Unmatched 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0)) AND ActionType = '0'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0))  AND ActionType = '0'"
                 + ")"
                 + "SELECT * from cte "
                 + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 4) // UnMatched with ActionType = '3'
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '3'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '3'";
            }
            if (InMode == 5) // Unmatched less or equal working date // Note that to be confirmed is consider as unmatched
            {
                SqlString =
                  " ; with cte "
                  + " as "
                  + "("
                  + " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                  + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                  + " AND [Matched] = 0  AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
                  + " UNION ALL"
                  + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                  + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                  + " AND [Matched] = 0 AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
                  + ")"
                  + "SELECT * from cte "
                  + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 51) // Unmatched less or equal working date // Confirmed are out
            {

                SqlString =
               " ; with cte "
               + " as "
               + "("
               + " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
               + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
               + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0)) AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
               + " UNION ALL"
               + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
               + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
               + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0)) AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
               + ")"
               + "SELECT * from cte "
               + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 6) // Unmatched with reference like 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '0' AND TxnEntryDate <= @TxnEntryDate "
                + ")"
                + "SELECT * from cte "
                + " WHERE RRN LIKE '%" + InReference + "%' "
                + " OR StmtLineSuplementaryDetails LIKE '%" + InReference + "%' "
                + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 7) // Show All Adjustments not closed and Action type = 0 for this Internal Account 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                      + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                      + " AND IsAdjustment = 1 AND ActionType = '0'"
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  "
                      + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 57) // Show All Adjustments not closed and Action type = 0 for all Internal Accounts 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                      + " WHERE IsAdjustment = 1 AND ActionType = '0' AND SubSystem = @SubSystem "
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  "
                      + " ORDER BY StmtAccountID Asc ";
            }
            if (InMode == 8) // Show All Adjustments not closed and Action type = 0 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                      + " WHERE StmtAccountID = @InternalAccNo "
                      + " AND IsAdjustment = 1 AND ActionType = '3'"
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  ";
            }

            if (InMode == 9) // Auto Matched THIS Cycle 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND ((Origin = 'INTERNAL' AND Matched = 1 AND SettledRecord = 1 ) OR (Origin = 'WAdjustment' AND [Matched] = 1 AND [SettledRecord] = 1 AND IsAdjClosed = 0)) "
                   + " ORDER BY CategoryId, [UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 10) // This Cycle to be confirmend 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE  [MatchedRunningCycle] <= @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE  [MatchedRunningCycle] <= @MatchedRunningCycle  AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                    + " ORDER BY CategoryId, [UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 11) // All Remaining UnMatched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE SubSystem = @SubSystem  "
                   + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0))  AND TxnEntryDate <= @TxnEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE  SubSystem = @SubSystem  "
                   + " AND ([Matched] = 0 OR (Matched = 1 AND SettledRecord = 0)) AND TxnEntryDate <= @TxnEntryDate "
                   + " ORDER BY CategoryId,  Origin  , TxnAmt ASC";
            }

            if (InMode == 12) // The Alerts for pair 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE  StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem  "
                   + " AND [Matched] = 0  AND IsException = 1 AND TxnEntryDate <= @TxnEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                   + " AND [Matched] = 0  AND IsException = 1 AND TxnEntryDate <= @TxnEntryDate "
                   + " ORDER BY StmtExternalBankID, Origin  ASC";
            }


            if (InMode == 13) // The Alerts for all
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE  SubSystem = @SubSystem   "
                   + " AND [Matched] = 0  AND IsException = 1 AND TxnEntryDate <= @TxnEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE SubSystem = @SubSystem   "
                   + " AND [Matched] = 0  AND IsException = 1 AND TxnEntryDate <= @TxnEntryDate "
                   + " ORDER BY StmtExternalBankID,  Origin  ASC";
            }

            if (InMode == 15) // Matched with reference like 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem  "
                 + " AND  [Matched] = 1 AND [SettledRecord] = 1 "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND  [Matched] = 1 AND [SettledRecord] = 1 "
                + ")"
                + "SELECT * from cte "
                + " WHERE RRN LIKE '%" + InReference + "%' "
                + " OR StmtLineSuplementaryDetails LIKE '%" + InReference + "%' "
                + " ORDER BY " + InOrderCriteria;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubSystem", InSubSystem);
                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@StmtAccountID", InExternalAccno);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);

                        if (InMode == 5 || InMode == 6 || InMode == 11 || InMode == 12 || InMode == 13)
                        {
                            cmd.Parameters.AddWithValue("@TxnEntryDate", InTxnEntryDate);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);

                            if (InMode == 12 || InMode == 13) // Alerts 
                            {
                                if (IsException == true)
                                {
                                    OutstandingAlerts = OutstandingAlerts + 1;
                                    if (ExceptionNo > 0 & ExceptionNo < 88)
                                    // Note that Alerts with 88 are created for Alerts identified during matching
                                    // Exception No for disputes are created during dispute creation and are less than 88
                                    {
                                        OutstandingDisputes = OutstandingDisputes + 1;
                                    }
                                }
                            }

                            if (OldNumber == 0 || OldNumber != UniqueMatchingNo)
                            {
                                if (OldNumber != 0)
                                {
                                    //NewLine = true;
                                    if (ColorNo == 11) ColorNo = 12;
                                    else ColorNo = 11;
                                }

                                OldNumber = UniqueMatchingNo;
                            }
                            //
                            // Fill Table Line for VISA
                            //
                            if (InSubSystem == "CardsSettlement" || InSubSystem == "E_FINANCE")
                            {

                                DataRow RowSelected = TableNVCards_Records_Both.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["ColorNo"] = ColorNo;
                                RowSelected["CategoryId"] = CategoryId;
                                RowSelected["MatchingNo"] = UniqueMatchingNo;
                                RowSelected["MatchedType"] = MatchedType;

                                RowSelected["Origin"] = Origin;

                                RowSelected["CardNo"] = CardNo;

                                if (InMode == 2 & ActionType == "0") RowSelected["DONE"] = "YES";
                                if (InMode == 2 & ActionType == "8") RowSelected["DONE"] = "NO";

                                RowSelected["Code"] = TxnCode;
                                //RowSelected["ValueDate"] = TxnValueDate;
                                RowSelected["EntryDate"] = TxnEntryDate;

                                if (TxnCode == "11" || TxnCode == "12" || TxnCode == "13") RowSelected["DR/CR"] = "DR";
                                else RowSelected["DR/CR"] = "CR";

                                RowSelected["Amt"] = TxnAmt;

                                RowSelected["Ccy"] = TxnCcy;

                                RowSelected["RRN"] = "  " + RRN;

                                RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;
                                RowSelected["TermID"] = TerminalId;

                                RowSelected["GLAccount"] = AdjGlAccount;
                                RowSelected["AccNo"] = AccNo;

                                RowSelected["MerchantId"] = MerchantId;
                                RowSelected["MerchantName"] = MerchantName;

                                // ADD ROW

                                TableNVCards_Records_Both.Rows.Add(RowSelected);

                                TotalSelected = TotalSelected + 1;
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    if (InMode == 4) return; // THIS SHOULDNT GO To Print process

                    ////ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsNOSTRO Tr = new RRDMTempTablesForReportsNOSTRO();

                    if (InSubSystem == "CardsSettlement" || InSubSystem == "E_FINANCE" || InSubSystem == "")
                    {

                        //Clear Table 
                        Tr.DeleteReport66(InSignedId);

                        int I = 0;

                        while (I <= (TableNVCards_Records_Both.Rows.Count - 1))
                        {

                            Tr.SeqNo = (int)TableNVCards_Records_Both.Rows[I]["SeqNo"];
                            Tr.ColorNo = (int)TableNVCards_Records_Both.Rows[I]["ColorNo"];
                            Tr.CategoryId = (string)TableNVCards_Records_Both.Rows[I]["CategoryId"];
                            Tr.MatchingNo = (int)TableNVCards_Records_Both.Rows[I]["MatchingNo"];
                            Tr.MatchedType = (string)TableNVCards_Records_Both.Rows[I]["MatchedType"];

                            Tr.Origin = (string)TableNVCards_Records_Both.Rows[I]["Origin"];
                            Tr.CardNo = (string)TableNVCards_Records_Both.Rows[I]["CardNo"];

                            if (InMode == 2) Tr.DONE = (string)TableNVCards_Records_Both.Rows[I]["DONE"];
                            else Tr.DONE = "";
                            Tr.TxnType = (string)TableNVCards_Records_Both.Rows[I]["Code"];

                            Tr.EntryDate = (DateTime)TableNVCards_Records_Both.Rows[I]["EntryDate"];
                            Tr.DRCR = (string)TableNVCards_Records_Both.Rows[I]["DR/CR"];
                            Tr.Amt = (decimal)TableNVCards_Records_Both.Rows[I]["Amt"];
                            Tr.Ccy = (string)TableNVCards_Records_Both.Rows[I]["Ccy"];

                            Tr.RRN = (string)TableNVCards_Records_Both.Rows[I]["RRN"];
                            Tr.OtherDetails = (string)TableNVCards_Records_Both.Rows[I]["OtherDetails"];
                            Tr.TermId = (string)TableNVCards_Records_Both.Rows[I]["TermId"];
                            Tr.AccNo = (string)TableNVCards_Records_Both.Rows[I]["AccNo"];

                            // Insert record for printing 
                            //
                            Tr.InsertReport66(InSignedId);

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
        // READ and make matching Many To Many 
        // This includes one to one
        // FILL UP A TABLE 
        //
        int WUnique;

        string SelectionCriteria;

        string PreviousReference;

        public void ReadNVStatements_LinesAndMatchManyToMany(string InOperator, string InSignedId, string InSubSystem, int InReconcCycleNo,
                                                                       string InExternalAccno, string InternalAccNo, DateTime InFromDt, DateTime InWorkingDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OldNumber = 0;
            //NewLine = false;

            TableManyToManySummary = new DataTable();
            TableManyToManySummary.Clear();
            TotalSelected = 0;

            //if (InMode == 20) // VISA SETTLEMENT  
            //{
            SqlString =
            " ; with cte "
            + " as "
            + "("
            + " SELECT RRN, TxnCcy ,SUM([TxnAmt])  As A, COUNT(SeqNo) As B "
            + " FROM[ATMS].[dbo].[NVCards_External_File_Setllement]"
            + " WHERE StmtAccountID = @ExternalAccID AND SubSystem = @SubSystem "
            + "  AND Matched = 0 AND TxnEntryDate <= @WorkingDt AND TxnAmt <> 0 "
            + " Group By RRN , TxnCcy "
            + " UNION ALL"
            + " SELECT RRN, TxnCcy, SUM([TxnAmt]) As A, COUNT(SeqNo) As B "
            + " FROM[ATMS].[dbo].[NVCards_Internal_File_Authorisation]  "
            + " WHERE StmtAccountID = @InternalAccID AND SubSystem =  @SubSystem "
            + " AND Matched = 0 AND TxnEntryDate <= @WorkingDt AND TxnAmt <> 0 "
            + " Group By RRN , TxnCcy "
            + " )"
            + " SELECT RRN As Reference, TxnCcy As TxnCcy,"
            + " SUM(A) As Difference, SUM(B) As Lines "
            + " From cte group by RRN, TxnCcy Having SUM(A) = 0 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SubSystem", InSubSystem);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ExternalAccID", InExternalAccno);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@InternalAccID", InternalAccNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDt", InWorkingDt);

                        sqlAdapt.Fill(TableManyToManySummary);

                        // Close conn
                        conn.Close();

                        // Read Table 
                        if (TableManyToManySummary.Rows.Count == 0)
                        {
                            TableManyToManyTxns = new DataTable();
                            TableManyToManyTxns.Clear();
                        }

                        int I = 0;

                        while (I <= (TableManyToManySummary.Rows.Count - 1))
                        {
                            //    RecordFound = true;

                            string WReference = (string)TableManyToManySummary.Rows[I]["Reference"];
                            string WTxnCcy = (string)TableManyToManySummary.Rows[I]["TxnCcy"];
                            //int WLines = (int)TableManyToManySummary.Rows[I]["Lines"];

                            int WMode = I;
                            // FIND ALL RECORDS WITH SAME REFERENCE AND FILL TableManyToManyTxns DETAILS
                            ReadAndFillTableManyToManyTxns(InOperator, InSignedId, WMode,
                                      InExternalAccno, InternalAccNo, WTxnCcy, InWorkingDt, WReference);

                            I++; // Read Next entry of the table 

                        }
                        //
                        // UPDATE FOOTER WITH THE MATCHED ENTRIES
                        // 
                        if (TableManyToManyTxns.Rows.Count > 0)
                        {

                            int K = 0;

                            PreviousReference = "";

                            while (K <= (TableManyToManyTxns.Rows.Count - 1))
                            {

                                int WSeqNo = (int)TableManyToManyTxns.Rows[K]["SeqNo"];

                                string WOrigin = (string)TableManyToManyTxns.Rows[K]["Origin"];

                                string WRRN = (string)TableManyToManyTxns.Rows[K]["RRN"];

                                if (WRRN != PreviousReference)
                                {
                                    // Get Unique Id 
                                    WUnique = Gu.GetNextValue();
                                    // Assign ref
                                    PreviousReference = WRRN;
                                }

                                if (WOrigin == "EXTERNAL")
                                {
                                    SelectionCriteria = " WHERE SeqNo =" + WSeqNo;
                                    ReadNVCards_External_File_SetllementBySelectionCriteria(SelectionCriteria);
                                    Matched = true;
                                    MatchedRunningCycle = InReconcCycleNo;

                                    ToBeConfirmed = false;
                                    ActionType = "0";
                                    MatchedType = "SystemDefault";

                                    UniqueMatchingNo = WUnique;

                                    UserId = InSignedId;

                                    SettledRecord = true;

                                    UpdateExternalFooter(Operator, SelectionCriteria);

                                }

                                if (WOrigin == "INTERNAL")
                                {
                                    SelectionCriteria = " WHERE SeqNo =" + WSeqNo;
                                    ReadNVCards_Internal_File_AuthorisationBySelectionCriteria(SelectionCriteria);
                                    Matched = true;
                                    MatchedRunningCycle = InReconcCycleNo;

                                    ToBeConfirmed = false;
                                    ActionType = "0";
                                    MatchedType = "SystemDefault";

                                    UniqueMatchingNo = WUnique;

                                    UserId = InSignedId;

                                    SettledRecord = true;

                                    UpdateInternalFooter(Operator, SelectionCriteria);

                                }

                                K++; // Read Next entry of the table 

                            }

                        }
                        else
                        {
                            //System.Windows.Forms.MessageBox.Show("Number of trxs = " + TableManyToManyTxns.Rows.Count.ToString());
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
        // READ INTERNAL OR EXTERNAL 
        // RReadAndFillTableManyToManyTxns
        // FILL UP A TABLE LEFT 
        //

        public void ReadAndFillTableManyToManyTxns(string InOperator, string InSignedId, int InMode,
                                   string InExternalAccno, string InternalAccNo, string InTxnCcy, DateTime InWorkingDt, string InReference)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OldNumber = 0;
            //NewLine = false;

            decimal StmtBal = 0;
            if (InMode == 0)
            {
                TableManyToManyTxns = new DataTable();
                TableManyToManyTxns.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableManyToManyTxns.Columns.Add("SeqNo", typeof(int));
                TableManyToManyTxns.Columns.Add("Status", typeof(string));
                TableManyToManyTxns.Columns.Add("Setl", typeof(string));
                TableManyToManyTxns.Columns.Add("MatchingNo", typeof(int));
                TableManyToManyTxns.Columns.Add("Origin", typeof(string));

                TableManyToManyTxns.Columns.Add("Code", typeof(string));

                //TableManyToManyTxns.Columns.Add("ValueDate", typeof(DateTime));
                TableManyToManyTxns.Columns.Add("EntryDate", typeof(DateTime));
                TableManyToManyTxns.Columns.Add("DR/CR", typeof(string));
                TableManyToManyTxns.Columns.Add("Ccy", typeof(string));
                TableManyToManyTxns.Columns.Add("Amt", typeof(decimal));
                TableManyToManyTxns.Columns.Add("StmtBal", typeof(decimal));

                TableManyToManyTxns.Columns.Add("RRN", typeof(string));

                TableManyToManyTxns.Columns.Add("OtherDetails", typeof(string));

            }

            TotalSelected = 0;

            SqlString =
             " ; with cte "
             + " as "
             + "("
            + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
             + " WHERE StmtAccountID = @ExternalAccno "
             + " AND Matched = 0 AND TxnEntryDate <= @WorkingDt "
             + " UNION ALL"
             + " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
             + " WHERE StmtAccountID = @InternalAccNo "
             + " AND Matched = 0 AND TxnEntryDate <= @WorkingDt "
            + ")"
            + "SELECT * from cte "
            + " WHERE RRN = @RRN And TxnCcy = @TxnCcy "
            + " ORDER BY Origin ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RRN", InReference);
                        cmd.Parameters.AddWithValue("@ExternalAccno", InExternalAccno);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@TxnCcy", InTxnCcy);
                        cmd.Parameters.AddWithValue("@WorkingDt", InWorkingDt);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);

                            // Fill Table Line 

                            DataRow RowSelected = TableManyToManyTxns.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            if (Matched == true) RowSelected["Status"] = "M";
                            else
                            {
                                RowSelected["Status"] = "U";
                            }

                            if (IsException == true) RowSelected["Status"] = "A";

                            if (SettledRecord == true) RowSelected["Setl"] = "YES";
                            else RowSelected["Setl"] = "NO";

                            if (Matched == true) RowSelected["MatchingNo"] = UniqueMatchingNo;
                            else RowSelected["MatchingNo"] = 0;
                            RowSelected["Origin"] = Origin;

                            RowSelected["Code"] = TxnCode;

                            RowSelected["EntryDate"] = TxnEntryDate;

                            if (TxnCode == "11" || TxnCode == "12" || TxnCode == "13") RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";

                            RowSelected["Ccy"] = TxnCcy;

                            RowSelected["Amt"] = TxnAmt;

                            RowSelected["StmtBal"] = StmtBal;

                            RowSelected["RRN"] = RRN;

                            RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;

                            // ADD ROW

                            TableManyToManyTxns.Rows.Add(RowSelected);

                            TotalSelected = TotalSelected + 1;
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
        // READ INTERNAL OR EXTERNAL 
        // READ by Range ... DATES 
        // FILL UP A TABLE LEFT 
        //

        public void ReadNVStatements_LinesByRangeAndDates(string InOperator, string InSignedId, string InSubSystem, int InMode,
                                   string InExternalAccno, string InternalAccNo, DateTime InFromDt, DateTime InToDt, string InReference, int InMatchingNo, decimal InAmt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OldNumber = 0;
            //NewLine = false;

            decimal StmtBal = 0;

            TableNVCards_Records_Single = new DataTable();
            TableNVCards_Records_Single.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableNVCards_Records_Single.Columns.Add("SeqNo", typeof(int));
            TableNVCards_Records_Single.Columns.Add("Status", typeof(string));
            TableNVCards_Records_Single.Columns.Add("Setl", typeof(string));
            TableNVCards_Records_Single.Columns.Add("MatchingNo", typeof(int));
            TableNVCards_Records_Single.Columns.Add("Origin", typeof(string));

            TableNVCards_Records_Single.Columns.Add("Code", typeof(string));

            TableNVCards_Records_Single.Columns.Add("EntryDate", typeof(DateTime));
            TableNVCards_Records_Single.Columns.Add("DR/CR", typeof(string));
            TableNVCards_Records_Single.Columns.Add("Ccy", typeof(string));
            TableNVCards_Records_Single.Columns.Add("Amt", typeof(decimal));
            TableNVCards_Records_Single.Columns.Add("StmtBal", typeof(decimal));

            TableNVCards_Records_Single.Columns.Add("OurRef", typeof(string));

            TableNVCards_Records_Single.Columns.Add("OtherDetails", typeof(string));

            if (InMode == 11) // INTERNAL 
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                   + " AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " Order By SeqNo ";
            }

            if (InMode == 12) // EXTERNAL 
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                   + " AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " Order By SeqNo ";
            }

            if (InMode == 17) // UniqueMatchingNo
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE UniqueMatchingNo = " + InMatchingNo
                + " ORDER BY Origin ";
            }


            if (InMode == 18) // TxnAmt
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE ABS(TxnAmt) = " + InAmt
                + " ORDER BY Origin ";
            }

            if (InMode == 19) // Reference like 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE RRN LIKE '%" + InReference + "%' "
                + " OR StmtLineSuplementaryDetails LIKE '%" + InReference + "%' "
                + " ORDER BY Origin ";
            }

            if (InMode == 20) // Reference identical 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND TxnEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE RRN ='" + InReference + "' "
                + " ORDER BY Origin ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubSystem", InSubSystem);
                        cmd.Parameters.AddWithValue("@ExternalAccno", InExternalAccno);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                        cmd.Parameters.AddWithValue("@ToDt", InToDt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);

                            // Fill Table Line 

                            DataRow RowSelected = TableNVCards_Records_Single.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            if (Matched == true) RowSelected["Status"] = "M";
                            else
                            {
                                RowSelected["Status"] = "U";
                            }

                            if (IsException == true) RowSelected["Status"] = "A";

                            if (SettledRecord == true) RowSelected["Setl"] = "YES";
                            else RowSelected["Setl"] = "NO";

                            if (Matched == true) RowSelected["MatchingNo"] = UniqueMatchingNo;
                            else RowSelected["MatchingNo"] = 0;
                            RowSelected["Origin"] = Origin;

                            RowSelected["Code"] = TxnCode;

                            RowSelected["EntryDate"] = TxnEntryDate;

                            if (TxnCode == "11" || TxnCode == "12" || TxnCode == "13") RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";

                            RowSelected["Ccy"] = TxnCcy;
                            RowSelected["Amt"] = TxnAmt;

                            RowSelected["StmtBal"] = StmtBal;

                            RowSelected["OurRef"] = RRN;


                            RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;

                            // ADD ROW

                            TableNVCards_Records_Single.Rows.Add(RowSelected);

                            TotalSelected = TotalSelected + 1;
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    ////ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsNOSTRO Tr = new RRDMTempTablesForReportsNOSTRO();

                    //Clear Table 
                    Tr.DeleteReport65(InSignedId);

                    int I = 0;

                    while (I <= (TableNVCards_Records_Single.Rows.Count - 1))
                    {
                        //    RecordFound = true;


                        Tr.SeqNo = (int)TableNVCards_Records_Single.Rows[I]["SeqNo"];
                        Tr.Status = (string)TableNVCards_Records_Single.Rows[I]["Status"];
                        Tr.Settled = (string)TableNVCards_Records_Single.Rows[I]["Setl"];
                        Tr.MatchingNo = (int)TableNVCards_Records_Single.Rows[I]["MatchingNo"];
                        Tr.Origin = (string)TableNVCards_Records_Single.Rows[I]["Origin"];

                        Tr.TxnType = (string)TableNVCards_Records_Single.Rows[I]["Code"];
                        Tr.ValueDate = DateTime.Now;
                        Tr.EntryDate = (DateTime)TableNVCards_Records_Single.Rows[I]["EntryDate"];

                        Tr.DRCR = (string)TableNVCards_Records_Single.Rows[I]["DR/CR"];
                        Tr.Ccy = (string)TableNVCards_Records_Single.Rows[I]["Ccy"];
                        Tr.Amt = (decimal)TableNVCards_Records_Single.Rows[I]["Amt"];
                        Tr.StmtBal = (decimal)TableNVCards_Records_Single.Rows[I]["StmtBal"];

                        Tr.OurRef = (string)TableNVCards_Records_Single.Rows[I]["OurRef"];

                        Tr.OtherDetails = (string)TableNVCards_Records_Single.Rows[I]["OtherDetails"];

                        // Insert record for printing 
                        // By Dates 
                        Tr.InsertReport65(InSignedId);

                        I++; // Read Next entry of the table 

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
        // READ Range ... DATES 
        // FILL UP A TABLE LEFT 
        //

        public void ReadNVStatements_LinesByRangeAndDates(string InOperator, string InSignedId, int InMode,
                                   string InExternalAccno, string InternalAccNo, DateTime InFromDt, DateTime InToDt, int InUniqueMatchingNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // if InMode = 13 Read Matched By Range 
            // if InMode = 14 Read Unmatched 

            // If InMode = 21 Read all Matched Partially 

            OldNumber = 0;
            //NewLine = false;
            ColorNo = 11;

            TableNVCards_Records_Both = new DataTable();
            TableNVCards_Records_Both.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableNVCards_Records_Both.Columns.Add("SeqNo", typeof(int));
            TableNVCards_Records_Both.Columns.Add("ColorNo", typeof(int));
            TableNVCards_Records_Both.Columns.Add("CategoryId", typeof(string));
            TableNVCards_Records_Both.Columns.Add("MatchingNo", typeof(int));
            TableNVCards_Records_Both.Columns.Add("MatchedType", typeof(string));

            TableNVCards_Records_Both.Columns.Add("Origin", typeof(string));
            TableNVCards_Records_Both.Columns.Add("CardNo", typeof(string)); // 

            TableNVCards_Records_Both.Columns.Add("DONE", typeof(string));

            TableNVCards_Records_Both.Columns.Add("Code", typeof(string));
            //TableNVCards_Records_Both.Columns.Add("ValueDate", typeof(DateTime)); // 
            TableNVCards_Records_Both.Columns.Add("EntryDate", typeof(DateTime));
            TableNVCards_Records_Both.Columns.Add("DR/CR", typeof(string));
            TableNVCards_Records_Both.Columns.Add("Amt", typeof(decimal)); // 
            TableNVCards_Records_Both.Columns.Add("Ccy", typeof(string));

            TableNVCards_Records_Both.Columns.Add("RRN", typeof(string)); // 

            TableNVCards_Records_Both.Columns.Add("OtherDetails", typeof(string));
            TableNVCards_Records_Both.Columns.Add("TermId", typeof(string)); // 

            TableNVCards_Records_Both.Columns.Add("GLAccount", typeof(string));
            TableNVCards_Records_Both.Columns.Add("AccNo", typeof(string)); // 

            if (InMode == 13) // Matched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 14) // UnMatched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 0 "
                   + " ORDER By Origin ASC";
            }

            if (InMode == 15) // Alerts for pair within range
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (TxnEntryDate > @FromDt AND TxnEntryDate < @ToDt) "
                   + " AND [Matched] = 0 AND IsException = 1 AND ExceptionNo = 0"
                   + " ORDER By Origin ASC";

            }

            if (InMode == 16) // Alerts for ALL within range
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " ORDER By Origin ASC";

            }

            if (InMode == 21) // Unique Partially Matched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @ExternalAccNo  "
                   + " AND UniqueMatchingNo = @UniqueMatchingNo "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo  "
                   + " AND UniqueMatchingNo = @UniqueMatchingNo "
                   + " ORDER BY Origin ASC";
            }

            if (InMode == 3)
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID "
                 + " AND [Matched] = 0 AND ActionType = '0'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo "
                 + " AND [Matched] = 0  AND ActionType = '0'";
            }

            if (InMode == 4)
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                 + " WHERE StmtAccountID = @StmtAccountID "
                 + " AND [Matched] = 0 AND ActionType = '3'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                 + " WHERE StmtAccountID = @InternalAccNo "
                 + " AND [Matched] = 0  AND ActionType = '3'";
            }

            if (InMode == 27) // UnMatched Based on Matchurity dt 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (TxnValueDate > @FromDt AND TxnValueDate < @ToDt) "
                   + " AND [Matched] = 0 "
                   + " ORDER By Origin ASC";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == 16)
                        {
                            // No Parameters 
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ExternalAccno", InExternalAccno);
                            cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                            if (InMode == 21)
                            {
                                cmd.Parameters.AddWithValue("@UniqueMatchingNo", InUniqueMatchingNo);
                            }
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);

                            if (InMode == 13)
                            {
                                if (OldNumber == 0 || OldNumber != UniqueMatchingNo)
                                {
                                    if (OldNumber != 0)
                                    {
                                        //NewLine = true;
                                        if (ColorNo == 11) ColorNo = 12;
                                        else ColorNo = 11;
                                    }

                                    OldNumber = UniqueMatchingNo;
                                }
                                //else NewLine = false;
                            }
                            else
                            {
                                if (Origin == "EXTERNAL")
                                {
                                    ColorNo = 11;
                                }
                                else ColorNo = 12;
                            }

                            // Fill Table Line 

                            DataRow RowSelected = TableNVCards_Records_Both.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ColorNo"] = ColorNo;
                            RowSelected["CategoryId"] = CategoryId;
                            RowSelected["MatchingNo"] = UniqueMatchingNo;
                            RowSelected["MatchedType"] = MatchedType;

                            RowSelected["Origin"] = Origin;

                            RowSelected["CardNo"] = CardNo;

                            if (InMode == 2 & ActionType == "0") RowSelected["DONE"] = "YES";
                            if (InMode == 2 & ActionType == "8") RowSelected["DONE"] = "NO";

                            RowSelected["Code"] = TxnCode;
                            //RowSelected["ValueDate"] = TxnValueDate;
                            RowSelected["EntryDate"] = TxnEntryDate;

                            if (TxnCode == "11" || TxnCode == "12" || TxnCode == "13") RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";

                            RowSelected["Amt"] = TxnAmt;

                            RowSelected["Ccy"] = TxnCcy;

                            RowSelected["RRN"] = "  " + RRN;

                            RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;
                            RowSelected["TermID"] = TerminalId;

                            RowSelected["GLAccount"] = AdjGlAccount;
                            RowSelected["AccNo"] = AccNo;

                            // ADD ROW

                            TableNVCards_Records_Both.Rows.Add(RowSelected);

                            TotalSelected = TotalSelected + 1;
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    ////ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsNOSTRO Tr = new RRDMTempTablesForReportsNOSTRO();

                    //Clear Table 
                    Tr.DeleteReport66(InSignedId);

                    int I = 0;

                    while (I <= (TableNVCards_Records_Both.Rows.Count - 1))
                    {

                        Tr.SeqNo = (int)TableNVCards_Records_Both.Rows[I]["SeqNo"];
                        Tr.ColorNo = (int)TableNVCards_Records_Both.Rows[I]["ColorNo"];
                        Tr.CategoryId = (string)TableNVCards_Records_Both.Rows[I]["CategoryId"];
                        Tr.MatchingNo = (int)TableNVCards_Records_Both.Rows[I]["MatchingNo"];
                        Tr.MatchedType = (string)TableNVCards_Records_Both.Rows[I]["MatchedType"];

                        Tr.Origin = (string)TableNVCards_Records_Both.Rows[I]["Origin"];
                        Tr.CardNo = (string)TableNVCards_Records_Both.Rows[I]["CardNo"];

                        if (InMode == 2) Tr.DONE = (string)TableNVCards_Records_Both.Rows[I]["DONE"];
                        else Tr.DONE = "";
                        Tr.TxnType = (string)TableNVCards_Records_Both.Rows[I]["Code"];

                        Tr.EntryDate = (DateTime)TableNVCards_Records_Both.Rows[I]["EntryDate"];
                        Tr.DRCR = (string)TableNVCards_Records_Both.Rows[I]["DR/CR"];
                        Tr.Amt = (decimal)TableNVCards_Records_Both.Rows[I]["Amt"];
                        Tr.Ccy = (string)TableNVCards_Records_Both.Rows[I]["Ccy"];

                        Tr.RRN = (string)TableNVCards_Records_Both.Rows[I]["RRN"];
                        Tr.OtherDetails = (string)TableNVCards_Records_Both.Rows[I]["OtherDetails"];
                        Tr.TermId = (string)TableNVCards_Records_Both.Rows[I]["TermId"];
                        Tr.AccNo = (string)TableNVCards_Records_Both.Rows[I]["AccNo"];

                        // Insert record for printing 
                        //
                        Tr.InsertReport66(InSignedId);

                        I++; // Read Next entry of the table 

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
        // READ 
        // FILL UP A TABLE LEFT 
        //
        public decimal InternalAccBalance;
        public decimal ExternalAccBalance;

        public decimal UnMatchedInternalCR;
        public decimal UnMatchedInternalDR;
        public decimal UnMatchedExternalCR;
        public decimal UnMatchedExternalDR;

        public decimal MatchedAdjustmentsOpenNegative;
        public decimal MatchedAdjustmentsOpenPositive;

        public int InternalAccTxns;
        public int ExternalAccTxns;

        public int UnMatchedInternalCRTxns;
        public int UnMatchedInternalDRTxns;
        public int UnMatchedExternalCRTxns;
        public int UnMatchedExternalDRTxns;
        public int MatchedAdjustmentsOpenTxns;

        public int ToBeConfirmedTxns;

        public void ReadNVStatements_LinesForTotals(string InOperator, string InSignedId, int InMode,
                                                string InExternalAccno, string InternalAccNo, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            InternalAccBalance = 0;
            ExternalAccBalance = 0;

            UnMatchedInternalCR = 0;
            UnMatchedInternalDR = 0;
            UnMatchedExternalCR = 0;
            UnMatchedExternalDR = 0;

            MatchedAdjustmentsOpenNegative = 0;
            MatchedAdjustmentsOpenPositive = 0;

            MatchedAdjustmentsOpenTxns = 0;

            InternalAccTxns = 0;
            ExternalAccTxns = 0;

            UnMatchedInternalCRTxns = 0;
            UnMatchedInternalDRTxns = 0;
            UnMatchedExternalCRTxns = 0;
            UnMatchedExternalDRTxns = 0;



            // If InMode = 1 Read ALL 

            if (InMode == 1)
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @StmtAccountID AND TxnEntryDate <= @TxnEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo AND TxnEntryDate <= @TxnEntryDate"
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@StmtAccountID", InExternalAccno);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@TxnEntryDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);

                            //
                            // Find Totals  
                            //

                            if (Origin == "EXTERNAL")
                            {
                                ExternalAccBalance = ExternalAccBalance + TxnAmt;
                                ExternalAccTxns = ExternalAccTxns + 1;

                                if (Matched == false & TxnAmt < 0)
                                {
                                    decimal TempExt = -TxnAmt;
                                    UnMatchedExternalCR = UnMatchedExternalCR + TempExt;
                                    UnMatchedExternalCRTxns = UnMatchedExternalCRTxns + 1;
                                }

                                if (Matched == false & TxnAmt > 0)
                                {
                                    UnMatchedExternalDR = UnMatchedExternalDR + TxnAmt;
                                    UnMatchedExternalDRTxns = UnMatchedExternalDRTxns + 1;
                                }
                            }

                            if (Origin == "INTERNAL" || Origin == "WAdjustment")
                            {
                                InternalAccBalance = InternalAccBalance + TxnAmt;
                                InternalAccTxns = InternalAccTxns + 1;

                                if (Matched == false & TxnAmt < 0)
                                {
                                    decimal TempInt = -TxnAmt;
                                    UnMatchedInternalCR = UnMatchedInternalCR + TempInt;
                                    UnMatchedInternalCRTxns = UnMatchedInternalCRTxns + 1;
                                }

                                if (Matched == false & TxnAmt > 0)
                                {
                                    UnMatchedInternalDR = UnMatchedInternalDR + TxnAmt;
                                    UnMatchedInternalDRTxns = UnMatchedInternalDRTxns + 1;
                                }
                            }

                            if (Origin == "WAdjustment" & IsAdjClosed == false)
                            {
                                MatchedAdjustmentsOpenTxns = MatchedAdjustmentsOpenTxns + 1;
                                if (TxnAmt < 0)
                                {
                                    MatchedAdjustmentsOpenNegative = MatchedAdjustmentsOpenNegative + TxnAmt;
                                }
                                if (TxnAmt > 0)
                                {
                                    MatchedAdjustmentsOpenPositive = MatchedAdjustmentsOpenPositive + TxnAmt;
                                }
                            }

                            if (Matched == true & SettledRecord == false)
                            {
                                ToBeConfirmedTxns = ToBeConfirmedTxns + 1;
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
        // INTERNAL THIS CYCLE
        public int INT_Processed_Txns;
        public decimal INT_Processed_Amt;
        public int INT_Matched_Txns;
        public decimal INT_Matched_Amount;
        public int INT_Unmatched_Txns;
        public decimal INT_Unmatched_Amount;

        // EXTERNAL THIS CYCLE
        public int EXT_Processed_Txns;
        public decimal EXT_Processed_Amt;
        public int EXT_Matched_Txns;
        public decimal EXT_Matched_Amount;
        public int EXT_Unmatched_Txns;
        public decimal EXT_Unmatched_Amount;

        public void ReadNVStatements_LinesForTotals_2(string InOperator, string InSignedId, int InMode,
                                                string InExternalAccno, string InternalAccNo, int InMatchedRunningCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableNVCards_Records_Both = new DataTable();
            TableNVCards_Records_Both.Clear();

            // INTERNAL THIS CYCLE
            INT_Processed_Txns = 0;
            INT_Processed_Amt = 0;
            INT_Matched_Txns = 0;
            INT_Matched_Amount = 0;
            INT_Unmatched_Txns = 0;
            INT_Unmatched_Amount = 0;

            // EXTERNAL THIS CYCLE
            EXT_Processed_Txns = 0;
            EXT_Processed_Amt = 0;
            EXT_Matched_Txns = 0;
            EXT_Matched_Amount = 0;
            EXT_Unmatched_Txns = 0;
            EXT_Unmatched_Amount = 0;

            ToBeConfirmedTxns = 0;
            // If InMode = 1 Read ALL 

            if (InMode == 1)
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
                   + " WHERE StmtAccountID = @StmtAccountID "
                   + " AND (SettledRecord = 0 OR MatchedRunningCycle = @MatchedRunningCycle) "
                   + " UNION ALL"
                   + " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE StmtAccountID = @InternalAccNo "
                   + " AND (SettledRecord = 0 OR MatchedRunningCycle = @MatchedRunningCycle) "
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@StmtAccountID", InExternalAccno);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchedRunningCycle", InMatchedRunningCycle);

                        sqlAdapt.Fill(TableNVCards_Records_Both);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableNVCards_Records_Both.Rows.Count - 1))
                        {

                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableNVCards_Records_Both.Rows[I]["SeqNo"];
                            Origin = (string)TableNVCards_Records_Both.Rows[I]["Origin"];

                            TxnAmt = (decimal)TableNVCards_Records_Both.Rows[I]["TxnAmt"];
                            TxnCcy = (string)TableNVCards_Records_Both.Rows[I]["TxnCcy"];

                            Matched = (bool)TableNVCards_Records_Both.Rows[I]["Matched"];
                            MatchedRunningCycle = (int)TableNVCards_Records_Both.Rows[I]["MatchedRunningCycle"];
                            ToBeConfirmed = (bool)TableNVCards_Records_Both.Rows[I]["ToBeConfirmed"];
                            UniqueMatchingNo = (int)TableNVCards_Records_Both.Rows[I]["UniqueMatchingNo"];
                            SettledRecord = (bool)TableNVCards_Records_Both.Rows[I]["SettledRecord"];

                            Operator = (string)TableNVCards_Records_Both.Rows[I]["Operator"];

                            if (Origin == "INTERNAL" || Origin == "WAdjustment")
                            {
                                INT_Processed_Txns = INT_Processed_Txns + 1;
                                INT_Processed_Amt = INT_Processed_Amt + Math.Abs(TxnAmt);

                                if (Matched == true)
                                {
                                    // Matched records
                                    INT_Matched_Txns = INT_Matched_Txns + 1;
                                    INT_Matched_Amount = INT_Matched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == false & SettledRecord == false)
                                {
                                    // UnMatched records
                                    INT_Unmatched_Txns = INT_Unmatched_Txns + 1;
                                    INT_Unmatched_Amount = INT_Unmatched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == true & SettledRecord == false)
                                {
                                    ToBeConfirmedTxns = ToBeConfirmedTxns + 1;
                                }
                            }

                            if (Origin == "EXTERNAL")
                            {
                                EXT_Processed_Txns = EXT_Processed_Txns + 1;
                                EXT_Processed_Amt = EXT_Processed_Amt + Math.Abs(TxnAmt);

                                if (Matched == true)
                                {
                                    // Matched records
                                    EXT_Matched_Txns = EXT_Matched_Txns + 1;
                                    EXT_Matched_Amount = EXT_Matched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == false & SettledRecord == false)
                                {
                                    // UnMatched records
                                    EXT_Unmatched_Txns = EXT_Unmatched_Txns + 1;
                                    EXT_Unmatched_Amount = EXT_Unmatched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == true & SettledRecord == false)
                                {
                                    ToBeConfirmedTxns = ToBeConfirmedTxns + 1;
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

        // BRANCHES TOTALS

        public decimal GT_INT_Branches = 0;
        public decimal GT_INT_Matched_Txns = 0;
        public decimal GT_INT_Matched_Amt = 0;
        public decimal GT_INT_Unmatched_Branches = 0;
        public decimal GT_INT_Unmathed_Txns = 0;
        public decimal GT_INT_UnMatched_Amt = 0;

        public decimal GT_EXT_Branches = 0;
        public decimal GT_EXT_Matched_Txns = 0;
        public decimal GT_EXT_Matched_Amt = 0;
        public decimal GT_EXT_Unmatched_Branches = 0;
        public decimal GT_EXT_Unmathed_Txns = 0;
        public decimal GT_EXT_UnMatched_Amt = 0;


        public void ReadNV_E_FIN_Branches_Totals(string InOperator, string InSignedId,
                                                                               int InMatchedRunningCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            GT_INT_Branches = 0;
            GT_INT_Matched_Txns = 0;
            GT_INT_Matched_Amt = 0;
            GT_INT_Unmatched_Branches = 0;
            GT_INT_Unmathed_Txns = 0;
            GT_INT_UnMatched_Amt = 0;

            GT_EXT_Branches = 0;
            GT_EXT_Matched_Txns = 0;
            GT_EXT_Matched_Amt = 0;
            GT_EXT_Unmatched_Branches = 0;
            GT_EXT_Unmathed_Txns = 0;
            GT_EXT_UnMatched_Amt = 0;


            TableNVCards_Records_Both = new DataTable();
            TableNVCards_Records_Both.Clear();

            TableBranchesTotals = new DataTable();
            TableBranchesTotals.Clear();

            TableBranchesTotals.Columns.Add("CategoryId", typeof(string));
            TableBranchesTotals.Columns.Add("TXNs Matched E-FIN", typeof(int));
            TableBranchesTotals.Columns.Add("Matched E-FIN", typeof(decimal));
            TableBranchesTotals.Columns.Add("TXNs UnMatched E-FIN", typeof(int));
            TableBranchesTotals.Columns.Add("UnMatched E-FIN", typeof(decimal));
            TableBranchesTotals.Columns.Add("**********", typeof(string));
            TableBranchesTotals.Columns.Add("TXNs Matched GL", typeof(int));
            TableBranchesTotals.Columns.Add("Matched GL", typeof(decimal));
            TableBranchesTotals.Columns.Add("TXNs UnMatched GL", typeof(int));
            TableBranchesTotals.Columns.Add("UnMatched GL", typeof(decimal));

            // READ ALL TXNS AND CREATE TABLE OF BRANCH TOTALS

            SqlString =
                 " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
               + " WHERE  "
               + "  (SettledRecord = 0 OR MatchedRunningCycle = @MatchedRunningCycle)  AND SubSystem = 'E_FINANCE'"
               + " UNION ALL"
               + " SELECT * FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
               + " WHERE  "
               + "  (SettledRecord = 0 OR MatchedRunningCycle = @MatchedRunningCycle)  AND SubSystem = 'E_FINANCE'"
               + " ORDER BY CategoryId , Origin ASC";


            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchedRunningCycle", InMatchedRunningCycle);

                        sqlAdapt.Fill(TableNVCards_Records_Both);

                        // Close conn
                        conn.Close();
                        //
                        int TxnsRows = TableNVCards_Records_Both.Rows.Count;

                        string OldCategoryId = "";
                        if (TxnsRows > 0)
                        {
                            InitialiseTotals();

                            OldCategoryId = (string)TableNVCards_Records_Both.Rows[0]["CategoryId"];
                        }

                        int I = 0;

                        while (I <= (TxnsRows - 1))
                        {

                            // For each entry in table Update records. 

                            // READ 

                            CategoryId = (string)TableNVCards_Records_Both.Rows[I]["CategoryId"];
                            if (CategoryId != OldCategoryId)
                            {
                                // Category has been changed
                                // Insert Row in Branches table
                                // Initialise totals
                                AddEntryToTableBranchTotals(OldCategoryId);
                                InitialiseTotals();
                                OldCategoryId = CategoryId;
                            }

                            SeqNo = (int)TableNVCards_Records_Both.Rows[I]["SeqNo"];

                            Origin = (string)TableNVCards_Records_Both.Rows[I]["Origin"];

                            TxnAmt = (decimal)TableNVCards_Records_Both.Rows[I]["TxnAmt"];
                            TxnCcy = (string)TableNVCards_Records_Both.Rows[I]["TxnCcy"];

                            Matched = (bool)TableNVCards_Records_Both.Rows[I]["Matched"];
                            MatchedRunningCycle = (int)TableNVCards_Records_Both.Rows[I]["MatchedRunningCycle"];
                            ToBeConfirmed = (bool)TableNVCards_Records_Both.Rows[I]["ToBeConfirmed"];
                            UniqueMatchingNo = (int)TableNVCards_Records_Both.Rows[I]["UniqueMatchingNo"];
                            SettledRecord = (bool)TableNVCards_Records_Both.Rows[I]["SettledRecord"];

                            Operator = (string)TableNVCards_Records_Both.Rows[I]["Operator"];

                            if (Origin == "INTERNAL" || Origin == "WAdjustment")
                            {
                                INT_Processed_Txns = INT_Processed_Txns + 1;
                                INT_Processed_Amt = INT_Processed_Amt + Math.Abs(TxnAmt);

                                if (Matched == true)
                                {
                                    // Matched records
                                    INT_Matched_Txns = INT_Matched_Txns + 1;
                                    INT_Matched_Amount = INT_Matched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == false & SettledRecord == false)
                                {
                                    // UnMatched records
                                    INT_Unmatched_Txns = INT_Unmatched_Txns + 1;
                                    INT_Unmatched_Amount = INT_Unmatched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == true & SettledRecord == false)
                                {
                                    ToBeConfirmedTxns = ToBeConfirmedTxns + 1;
                                }
                            }

                            if (Origin == "EXTERNAL")
                            {
                                EXT_Processed_Txns = EXT_Processed_Txns + 1;
                                EXT_Processed_Amt = EXT_Processed_Amt + Math.Abs(TxnAmt);

                                if (Matched == true)
                                {
                                    // Matched records
                                    EXT_Matched_Txns = EXT_Matched_Txns + 1;
                                    EXT_Matched_Amount = EXT_Matched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == false & SettledRecord == false)
                                {
                                    // UnMatched records
                                    EXT_Unmatched_Txns = EXT_Unmatched_Txns + 1;
                                    EXT_Unmatched_Amount = EXT_Unmatched_Amount + Math.Abs(TxnAmt);
                                }
                                if (Matched == true & SettledRecord == false)
                                {
                                    ToBeConfirmedTxns = ToBeConfirmedTxns + 1;
                                }
                            }

                            I++; // Read Next entry of the table 

                            if (I == TxnsRows)
                            {
                                // Last Branch
                                // Add Totals for last branch 
                                AddEntryToTableBranchTotals(OldCategoryId);
                            }

                        }
                    }
          
                    ////ReadTable And Insert In Sql Table
                    // For Report 
                    RRDMTempTablesForReportsNOSTRO Tr = new RRDMTempTablesForReportsNOSTRO();

                        //Clear Table 
                        Tr.DeleteReport77(InSignedId);

                        int K = 0;

                        while (K <= (TableBranchesTotals.Rows.Count - 1))
                        {

                        Tr.CategoryId = (string)TableBranchesTotals.Rows[K]["CategoryId"];
                        Tr.TXNs_Matched_E_FIN = (int)TableBranchesTotals.Rows[K]["TXNs Matched E-FIN"];
                        Tr.Matched_E_FIN = (decimal)TableBranchesTotals.Rows[K]["Matched E-FIN"];
                        Tr.TXNs_UnMatched_E_FIN = (int)TableBranchesTotals.Rows[K]["TXNs UnMatched E-FIN"];
                        Tr.UnMatched_E_FIN = (decimal)TableBranchesTotals.Rows[K]["UnMatched E-FIN"];

                        Tr.TXNs_Matched_GL = (int)TableBranchesTotals.Rows[K]["TXNs Matched GL"];
                        Tr.Matched_GL = (decimal)TableBranchesTotals.Rows[K]["Matched GL"];
                        Tr.TXNs_UnMatched_GL = (int)TableBranchesTotals.Rows[K]["TXNs UnMatched GL"];
                        Tr.UnMatched_GL = (decimal)TableBranchesTotals.Rows[K]["UnMatched GL"];

                        // Insert record for printing 
                        //
                        Tr.InsertReport77(InSignedId);

                            K++; // Read Next entry of the table 

                        }
                    
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        // Initialise totals 


        private void InitialiseTotals()
        {
            // INITIALISE BRANCH TOTALS

            INT_Processed_Txns = 0;
            INT_Processed_Amt = 0;
            INT_Matched_Txns = 0;
            INT_Matched_Amount = 0;
            INT_Unmatched_Txns = 0;
            INT_Unmatched_Amount = 0;

            // EXTERNAL THIS CYCLE
            EXT_Processed_Txns = 0;
            EXT_Processed_Amt = 0;
            EXT_Matched_Txns = 0;
            EXT_Matched_Amount = 0;
            EXT_Unmatched_Txns = 0;
            EXT_Unmatched_Amount = 0;

            ToBeConfirmedTxns = 0;
        }

        // Add Entry To table Branch Totals
        private void AddEntryToTableBranchTotals(string OldCategoryId)
        {
            // If Category Changes then Update Table 

            // Grand Totals Updating 
            if ((INT_Matched_Amount + INT_Unmatched_Amount) > 0) GT_INT_Branches = GT_INT_Branches + 1;

            GT_INT_Matched_Txns = GT_INT_Matched_Txns + INT_Matched_Txns; 
            GT_INT_Matched_Amt = GT_INT_Matched_Amt + INT_Matched_Amount;

            if (INT_Unmatched_Amount > 0) GT_INT_Unmatched_Branches = GT_INT_Unmatched_Branches + 1;
            GT_INT_Unmathed_Txns = GT_INT_Unmathed_Txns + INT_Unmatched_Txns; 
            GT_INT_UnMatched_Amt = GT_INT_UnMatched_Amt + INT_Unmatched_Amount;

            if ((EXT_Matched_Amount + EXT_Unmatched_Amount) > 0) GT_EXT_Branches = GT_EXT_Branches + 1;

            GT_EXT_Matched_Txns = GT_EXT_Matched_Txns + EXT_Matched_Txns;
            GT_EXT_Matched_Amt = GT_EXT_Matched_Amt + EXT_Matched_Amount;

            if (EXT_Unmatched_Amount > 0) GT_EXT_Unmatched_Branches = GT_EXT_Unmatched_Branches + 1;
            GT_EXT_Unmathed_Txns = GT_EXT_Unmathed_Txns + EXT_Unmatched_Txns;
            GT_EXT_UnMatched_Amt = GT_EXT_UnMatched_Amt + EXT_Unmatched_Amount;

            // ADD ENTRY TO TABLE 

            DataRow RowSelected2 = TableBranchesTotals.NewRow();

            RowSelected2["CategoryId"] = OldCategoryId;
        
            RowSelected2["TXNs Matched E-FIN"] = INT_Matched_Txns;
            RowSelected2["Matched E-FIN"] = INT_Matched_Amount.ToString();
            RowSelected2["TXNs UnMatched E-FIN"] = INT_Unmatched_Txns; 
            RowSelected2["UnMatched E-FIN"] = INT_Unmatched_Amount.ToString();

            RowSelected2["**********"] = "**********";
          
            RowSelected2["TXNs Matched GL"] = EXT_Matched_Txns;
            RowSelected2["Matched GL"] = EXT_Matched_Amount.ToString();
            RowSelected2["TXNs UnMatched GL"] = EXT_Unmatched_Txns;
            RowSelected2["UnMatched GL"] = EXT_Unmatched_Amount.ToString();

            // ADD ROW
            TableBranchesTotals.Rows.Add(RowSelected2);

        }

        public int TotalNumberProcessed;
        public int MatchedDefault;
        public int AutoButToBeConfirmed;
        public decimal UnMatchedAmt;

        public int MatchedFromAutoToBeConfirmed;
        public int MatchedFromManualToBeConfirmed;
        // Find Totals 
        public void ReadNVExternalStatements_LinesForTotals(string InOperator, string InSignedId, string InSubSystem, int InMatchedRunningCycle,
                                                string InExternalAccno, string InStmtTrxReferenceNumber, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalNumberProcessed = 0;
            MatchedDefault = 0;
            AutoButToBeConfirmed = 0;
            UnMatchedAmt = 0;

            MatchedFromAutoToBeConfirmed = 0;
            MatchedFromManualToBeConfirmed = 0;

            //if (InMode == 20) // CARDS 
            //{
            SqlString =
           " SELECT * FROM [ATMS].[dbo].[NVCards_External_File_Setllement]"
         + " WHERE StmtAccountID = @StmtAccountID  AND SubSystem = @SubSystem "
         + " AND StmtTrxReferenceNumber = @StmtTrxReferenceNumber "
         + " AND (SettledRecord = 0 OR MatchedRunningCycle = @MatchedRunningCycle) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SubSystem", InSubSystem);
                        cmd.Parameters.AddWithValue("@StmtAccountID", InExternalAccno);
                        cmd.Parameters.AddWithValue("@StmtTrxReferenceNumber", InStmtTrxReferenceNumber);
                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", InMatchedRunningCycle);
                        //cmd.Parameters.AddWithValue("@TxnEntryDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            GetReaderFields(rdr);
                            //
                            // Find Totals  
                            //
                            TotalNumberProcessed = TotalNumberProcessed + 1;

                            if (Matched == true & SettledRecord == true & MatchedType == "SystemDefault")
                            {
                                MatchedDefault = MatchedDefault + 1;
                            }
                            if (Matched == true & MatchedType == "AutoButToBeConfirmed")
                            {
                                AutoButToBeConfirmed = AutoButToBeConfirmed + 1;
                            }
                            if (SettledRecord == false)
                            {
                                if (TxnAmt > 0)
                                    UnMatchedAmt = UnMatchedAmt + TxnAmt;
                                if (TxnAmt < 0)
                                    UnMatchedAmt = UnMatchedAmt + (-TxnAmt);
                            }

                            if (Matched == true & SettledRecord == true & MatchedType == "AutoButToBeConfirmed")
                            {
                                MatchedFromAutoToBeConfirmed = MatchedFromAutoToBeConfirmed + 1;
                            }
                            if (Matched == true & SettledRecord == true & MatchedType == "ManualToBeConfirmed")
                            {
                                MatchedFromManualToBeConfirmed = MatchedFromManualToBeConfirmed + 1;
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
        // Search in Internal 
        //
        // Methods 
        // READ Specific by UniqueRecordId
        // INTERNAL 
        //
        public int NumberOfRecords;
        public void ReadNVCards_Internal_File_AuthorisationBySelectionCriteria2(int InMode,
                                            string InternalAccNo,
                                            string InRRN,
                                            DateTime InTxnValueDate,
                                            decimal InTxnAmt,
                                            DateTime InWDate,
                                            string InSelectionCriteria
                                            )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfRecords = 0;

            if (InMode == 31) // All Identical 
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    + " AND RRN = @RRN  "
                    //+ " AND StmtLineValueDate = @StmtLineValueDate "
                    + " AND TxnAmt = @TxnAmt  "
                    + " AND TxnEntryDate <= @TxnEntryDate AND Matched = 0  "
                    ;
            }
            if (InMode == 32) // Find if ANY Reference Like 
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    + " AND TxnEntryDate <= @TxnEntryDate AND Matched = 0  "
                    + InSelectionCriteria;
            }


            if (InMode == 55) // All Identical 
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    //+ " AND (StmtLineValueDate >= @InDtFrom AND StmtLineValueDate <= @InDtTo)  "
                    + " AND (TxnAmt >= @InAmtFrom AND TxnAmt <= @InAmtTo)  "
                    + " AND TxnEntryDate <= @TxnEntryDate AND Matched = 0  "
                    ;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == 31)
                        {
                            cmd.Parameters.AddWithValue("@StmtAccountID", InternalAccNo);

                            cmd.Parameters.AddWithValue("@RRN", InRRN);
                            //cmd.Parameters.AddWithValue("@StmtLineValueDate", InStmtLineValueDate);
                            cmd.Parameters.AddWithValue("@TxnAmt", InTxnAmt);

                            cmd.Parameters.AddWithValue("@TxnEntryDate", InWDate);
                        }

                        if (InMode == 32)
                        {
                            cmd.Parameters.AddWithValue("@StmtAccountID", InternalAccNo);

                            cmd.Parameters.AddWithValue("@TxnEntryDate", InWDate);
                        }


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            NumberOfRecords = NumberOfRecords + 1;

                            GetReaderFields(rdr);

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
        // Methods 
        // ReadInternalStatementAndCreateAgingFigures
        // Per Reconciliation Category
        //

        public void ReadInternalStatementAndCreateAgeingFigures(string InOperator, string InSignedId, int InMode,
                                   string InCategoryId, string InCategoryName,
                                   string InternalAccNo, string InCcy, decimal InCcyRate, DateTime InTestingDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int DiffInEntryDays;

            decimal Zero_To_4 = 0;
            decimal Four_To_7 = 0;
            decimal Seven_To_15 = 0;
            decimal Fifteen_To_30 = 0;
            decimal Thirty_To_60 = 0;
            decimal More_Than_60 = 0;
            decimal GrandTotal = 0;

            if (InMode == 0)
            {
                TableAgingAnalysis = new DataTable();
                TableAgingAnalysis.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableAgingAnalysis.Columns.Add("PairID", typeof(string));
                TableAgingAnalysis.Columns.Add("Name", typeof(string));
                TableAgingAnalysis.Columns.Add("Ccy", typeof(string));
                TableAgingAnalysis.Columns.Add("GrandTotal", typeof(decimal));
                TableAgingAnalysis.Columns.Add("0 - 4", typeof(decimal));
                TableAgingAnalysis.Columns.Add("4 - 7", typeof(decimal));
                TableAgingAnalysis.Columns.Add("7 - 15", typeof(decimal));
                TableAgingAnalysis.Columns.Add("15 - 30", typeof(decimal));
                TableAgingAnalysis.Columns.Add("30 - 60", typeof(decimal));
                TableAgingAnalysis.Columns.Add("> 60", typeof(decimal));

                TotalSelected = 0;
            }

            SqlString =
                 " SELECT  "
               + " StmtAccountID,"
               + " TxnAmt,"
               + " Matched "
               + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
               + " WHERE StmtAccountID = @InternalAccNo  "
               + " AND [Matched] = 0 ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            StmtAccountID = (string)rdr["StmtAccountID"];
                            TxnValueDate = (DateTime)rdr["TxnValueDate"];
                            TxnAmt = (decimal)rdr["TxnAmt"];
                            Matched = (bool)rdr["Matched"];

                            DiffInEntryDays = Convert.ToInt32((InTestingDate.Date - TxnValueDate).TotalDays);
                            //int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

                            if (DiffInEntryDays > 0)
                            {

                                if (DiffInEntryDays > 0 & DiffInEntryDays <= 4)
                                    Zero_To_4 = Zero_To_4 + Math.Abs(TxnAmt);

                                if (DiffInEntryDays > 7 & DiffInEntryDays <= 15)
                                    Seven_To_15 = Seven_To_15 + Math.Abs(TxnAmt);

                                if (DiffInEntryDays > 15 & DiffInEntryDays <= 30)
                                    Fifteen_To_30 = Fifteen_To_30 + Math.Abs(TxnAmt);

                                if (DiffInEntryDays > 30 & DiffInEntryDays <= 60)
                                    Thirty_To_60 = Thirty_To_60 + Math.Abs(TxnAmt);

                                if (DiffInEntryDays > 60)
                                    More_Than_60 = More_Than_60 + Math.Abs(TxnAmt);
                            }

                            GrandTotal = GrandTotal + Math.Abs(TxnAmt);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    // Fill Table Line 

                    DataRow RowSelected = TableAgingAnalysis.NewRow();

                    RowSelected["PairID"] = InCategoryId;
                    RowSelected["Name"] = InCategoryName;
                    RowSelected["Ccy"] = InCcy;
                    RowSelected["GrandTotal"] = GrandTotal * InCcyRate;

                    RowSelected["0 - 4"] = Zero_To_4 * InCcyRate;
                    RowSelected["4 - 7"] = Four_To_7 * InCcyRate;
                    RowSelected["7 - 15"] = Seven_To_15 * InCcyRate;
                    RowSelected["15 - 30"] = Fifteen_To_30 * InCcyRate;
                    RowSelected["30 - 60"] = Thirty_To_60 * InCcyRate;
                    RowSelected["> 60"] = More_Than_60 * InCcyRate;

                    // ADD ROW

                    TableAgingAnalysis.Rows.Add(RowSelected);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // READ ALL UNMATCHED INTERNAL RECORDS
        // AND UPDATE WITH CURRENT RATES
        // 
        public void ReadInternalAndUpdateCurrentRates(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMNVCurrentCcyRates Cr = new RRDMNVCurrentCcyRates();

            TableNVCards_Records_Both = new DataTable();
            TableNVCards_Records_Both.Clear();

            TotalSelected = 0;

            SqlString =
             " SELECT * "
           + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
           + " WHERE [Matched] = 0  ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", InOrigin);

                        sqlAdapt.Fill(TableNVCards_Records_Both);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableNVCards_Records_Both.Rows.Count - 1))
                        {


                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableNVCards_Records_Both.Rows[I]["SeqNo"];
                            TxnAmt = (decimal)TableNVCards_Records_Both.Rows[I]["TxnAmt"];
                            TxnCcy = (string)TableNVCards_Records_Both.Rows[I]["TxnCcy"];
                            TxnCcyRate = (decimal)TableNVCards_Records_Both.Rows[I]["TxnCcyRate"];
                            InternalTxtAmtCharged = (decimal)TableNVCards_Records_Both.Rows[I]["InternalTxtAmtCharged"];
                            IsLocalCcyAcc = (bool)TableNVCards_Records_Both.Rows[I]["IsLocalCcyAcc"];

                            Operator = (string)TableNVCards_Records_Both.Rows[I]["Operator"];

                            if (IsLocalCcyAcc == true)
                            {

                                Cr.ReadNVCurrentCcyRatesById(Operator, TxnCcy);

                                NewTxnCcyRate = Cr.CcyRate;

                                NewInternalTxtAmt = TxnAmt * NewTxnCcyRate;
                            }
                            else
                            {
                                NewTxnCcyRate = 1;

                                NewInternalTxtAmt = TxnAmt * NewTxnCcyRate;
                            }


                            UpdateInternalsWithCurrentRates(Operator, SeqNo,
                                                            NewTxnCcyRate, NewInternalTxtAmt);

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
        // UPDATE Internal with current Rates 
        //
        // 
        public void UpdateInternalsWithCurrentRates(string InOperator, int InSeqNo,
                          decimal InNewTxnCcyRate, decimal InNewInternalTxtAmt)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                            + " SET "
                            + " [NewTxnCcyRate] = @NewTxnCcyRate, [NewInternalTxtAmt] = @NewInternalTxtAmt "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@NewTxnCcyRate", InNewTxnCcyRate);
                        cmd.Parameters.AddWithValue("@NewInternalTxtAmt", InNewInternalTxtAmt);

                        //rows number of record got updated

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

        // UPDATE Undo Confirmed  External 
        //
        // Set Action type to 0 or 8  
        public void UpdateUndoConfirmedExternal(string InOperator, int InUniqueMatchingNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_External_File_Setllement] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + " WHERE UniqueMatchingNo = @UniqueMatchingNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueMatchingNo", InUniqueMatchingNo);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        //rows number of record got updated

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

        // UPDATE Undo Confirmed  Internal 
        //
        // Set Action type to 0 or 8 
        public void UpdateUndoConfirmedInternal(string InOperator, int InUniqueMatchingNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + " WHERE UniqueMatchingNo = @UniqueMatchingNo ", conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueMatchingNo", InUniqueMatchingNo);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        //rows number of record got updated

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

        // UPDATE
        //
        // Set Action type for the selection criteria 

        //string InExternalAccno, string InternalAccNo)
        public void UpdateActionTypeExternal(string InOperator, string InSelectionCriteria,
                 DateTime InTxnValueDate, string InActionType, DateTime InWdateTm)
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
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_External_File_Setllement] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + InSelectionCriteria + " AND TxnEntryDate <= @TxnEntryDate", conn))
                    {
                        cmd.Parameters.AddWithValue("@TxnEntryDate", InWdateTm);
                        cmd.Parameters.AddWithValue("@ActionType", InActionType);

                        //rows number of record got updated
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

        // UPDATE   Internal 
        //
        // Set Action type 
        public void UpdateActionTypeInternal(string InOperator, string InSelectionCriteria,
                 DateTime InTxnValueDate, string InActionType, DateTime InWdateTm)
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
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + InSelectionCriteria + " AND TxnEntryDate <= @TxnEntryDate", conn))
                    {
                        cmd.Parameters.AddWithValue("@TxnEntryDate", InWdateTm);
                        cmd.Parameters.AddWithValue("@ActionType", InActionType);

                        //rows number of record got updated

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

        // UPdate Adjustment 
        public void UpdateAdjustmentSpecific(int InSeqNo2)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation] "
                            + " SET "
                            + " [TxnCode] = @TxnCode, [TxnAmt] = @TxnAmt, "
                            + " [IsAdjustment]=@IsAdjustment, "
                            + " [Ccy]=@Ccy, "
                            + " [AdjGlAccount]=@AdjGlAccount, "
                            + " [IsAdjClosed]=@IsAdjClosed, "
                            + " [StmtLineSuplementaryDetails]=@StmtLineSuplementaryDetails "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo2);
                        cmd.Parameters.AddWithValue("@TxnCode", TxnCode);
                        cmd.Parameters.AddWithValue("@TxnAmt", TxnAmt);
                        cmd.Parameters.AddWithValue("@StmtLineSuplementaryDetails", StmtLineSuplementaryDetails);

                        cmd.Parameters.AddWithValue("@IsAdjustment", IsAdjustment);

                        cmd.Parameters.AddWithValue("@TxnCcy", TxnCcy);

                        cmd.Parameters.AddWithValue("@AdjGlAccount", AdjGlAccount);
                        cmd.Parameters.AddWithValue("@IsAdjClosed", IsAdjClosed);

                        //rows number of record got updated

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



        // UPDATE EXTERNAL RECORDS Footer 
        // 
        public void UpdateExternalFooter(string InOperator, string InSelectionCriteria)
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
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_External_File_Setllement]"
                            + " SET "
                            + " [Matched] = @Matched, "
                            + " [MatchedRunningCycle] = @MatchedRunningCycle,"
                            + " [ToBeConfirmed] = @ToBeConfirmed,"
                            + " [UniqueMatchingNo] = @UniqueMatchingNo,"
                            + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                            + " [IsException] = @IsException, [ExceptionId] = @ExceptionId, [ExceptionNo] = @ExceptionNo, "
                            + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                            + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                            + " [SettledRecord] = @SettledRecord "
                            + InSelectionCriteria, conn))
                    {

                        cmd.Parameters.AddWithValue("@Matched", Matched);

                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", MatchedRunningCycle);
                        cmd.Parameters.AddWithValue("@ToBeConfirmed", ToBeConfirmed);
                        cmd.Parameters.AddWithValue("@UniqueMatchingNo", UniqueMatchingNo);

                        cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                        cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                        cmd.Parameters.AddWithValue("@IsException", IsException);
                        cmd.Parameters.AddWithValue("@ExceptionId", ExceptionId);
                        cmd.Parameters.AddWithValue("@ExceptionNo", ExceptionNo);

                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                        //rows number of record got updated

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


        // UPDATE EXTERNAL RECORDS Footer 
        // 
        public void UpdateInternalFooter(string InOperator, string InSelectionCriteria)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                            + " SET "
                            + " [Matched] = @Matched, "
                            + " [MatchedRunningCycle] = @MatchedRunningCycle,"
                            + " [ToBeConfirmed] = @ToBeConfirmed,"
                            + " [UniqueMatchingNo] = @UniqueMatchingNo,"
                            + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                              + " [IsException] = @IsException, [ExceptionId] = @ExceptionId, [ExceptionNo] = @ExceptionNo, "
                            + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                            + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                            + " [SettledRecord] = @SettledRecord "
                            + InSelectionCriteria, conn))
                    {

                        cmd.Parameters.AddWithValue("@Matched", Matched);

                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", MatchedRunningCycle);
                        cmd.Parameters.AddWithValue("@ToBeConfirmed", ToBeConfirmed);
                        cmd.Parameters.AddWithValue("@UniqueMatchingNo", UniqueMatchingNo);

                        cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                        cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                        cmd.Parameters.AddWithValue("@IsException", IsException);
                        cmd.Parameters.AddWithValue("@ExceptionId", ExceptionId);
                        cmd.Parameters.AddWithValue("@ExceptionNo", ExceptionNo);

                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                        //rows number of record got updated

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

        // UPDATE StmtLineSuplementaryDetails for Internal 
        // 
        public void UpdateInternalStmtLineSuplementaryDetails(string InOperator, string InSelectionCriteria)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                            + " SET "
                            + " [StmtLineSuplementaryDetails] = @StmtLineSuplementaryDetails "
                            + InSelectionCriteria, conn))
                    {

                        cmd.Parameters.AddWithValue("@StmtLineSuplementaryDetails", StmtLineSuplementaryDetails);

                        //rows number of record got updated

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

        // UPDATE MATCHED TO UNMATCHED EXTERNAL Footer 
        // 
        public void UpdateExternalFooterMatchedToUnmatched(string InOperator, string InSelectionCriteria)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_External_File_Setllement]"
                            + " SET "
                            + " [Matched] = @Matched, "
                            + " [ToBeConfirmed] = @ToBeConfirmed,"
                            + " [MatchedType] = @MatchedType, "
                            + " [ActionType] = @ActionType,"
                            + " [SettledRecord] = @SettledRecord "
                            + InSelectionCriteria, conn))
                    {

                        cmd.Parameters.AddWithValue("@Matched", Matched);

                        cmd.Parameters.AddWithValue("@ToBeConfirmed", ToBeConfirmed);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                        //rows number of record got updated

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

        // UPDATE MATCHED TO UNMATCHED EXTERNAL Footer 
        // 
        public void UpdateInternalFooterMatchedToUnmatched(string InOperator, string InSelectionCriteria)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                            + " SET "
                            + " [Matched] = @Matched, "
                            + " [ToBeConfirmed] = @ToBeConfirmed,"
                            + " [MatchedType] = @MatchedType, "
                            + " [ActionType] = @ActionType,"
                            + " [SettledRecord] = @SettledRecord "
                            + InSelectionCriteria, conn))
                    {

                        cmd.Parameters.AddWithValue("@Matched", Matched);

                        cmd.Parameters.AddWithValue("@ToBeConfirmed", ToBeConfirmed);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                        //rows number of record got updated

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
        // Methods 
        // READ TOTALs UnMatched for external 
        // 
        //

        public int TotalAutoMatchedThisCycle;

        public int TotalAutoToBeConfirmedMatchedThisCycle;

        public int TotalManualMatchedThisCycle;

        public int TotalRemainUnMatchedThisCycle;

        public int TotalRemainUnMacthedPreviousCycles;

        public void ReadMatchingTxnsTotals(string InOperator, int InMatchedRunningCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalAutoMatchedThisCycle = 0;
            TotalAutoToBeConfirmedMatchedThisCycle = 0;
            TotalManualMatchedThisCycle = 0;

            TotalRemainUnMatchedThisCycle = 0;

            TotalRemainUnMacthedPreviousCycles = 0;

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                   + " WHERE Operator = @Operator AND MatchedRunningCycle = @MatchedRunningCycle ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", InMatchedRunningCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            Matched = (bool)rdr["Matched"];
                            MatchedRunningCycle = (int)rdr["MatchedRunningCycle"];

                            ToBeConfirmed = (bool)rdr["ToBeConfirmed"];
                            UniqueMatchingNo = (int)rdr["UniqueMatchingNo"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            IsException = (bool)rdr["IsException"];
                            ExceptionId = (int)rdr["ExceptionId"];
                            ExceptionNo = (int)rdr["ExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            SettledRecord = (bool)rdr["SettledRecord"];

                            IsAdjustment = (bool)rdr["IsAdjustment"];

                            AdjGlAccount = (string)rdr["AdjGlAccount"];
                            IsAdjClosed = (bool)rdr["IsAdjClosed"];

                            Operator = (string)rdr["Operator"];

                            // USED FOR MATCHED TRANSACTIONS 
                            // "SystemDefault"       
                            // "AutoButToBeConfirmed"
                            // "ManualToBeConfirmed"

                            // Update Totals 
                            if (Matched == true)
                            {
                                if (MatchedType == "SystemDefault") TotalAutoMatchedThisCycle = TotalAutoMatchedThisCycle + 1;
                                if (MatchedType == "AutoButToBeConfirmed" & SettledRecord == true) TotalAutoToBeConfirmedMatchedThisCycle = TotalAutoToBeConfirmedMatchedThisCycle + 1;
                                if (MatchedType == "ManualToBeConfirmed" & SettledRecord == true) TotalManualMatchedThisCycle = TotalManualMatchedThisCycle + 1;
                            }
                            if (Matched == false) // UnMatched 
                            {


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


        // Insert NEW Record in Internal Statement
        public int InsertNewRecordInInternalStatement()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVCards_Internal_File_Authorisation] " +
                " ([Origin], [StmtTrxReferenceNumber]," +
                " [StmtExternalBankID],[StmtAccountID], [StmtNumber], [StmtSequenceNumber], " +
                " [MerchantId],[MerchantName], [TypeOfTerminal], [TerminalId], " +
                " [TxnValueDate],[TxnEntryDate], [TxnCode], [TxnDesc], " +
                " [RRN],[TxnAmt], [TxnCcy], [TxnCcyRate], " +
                " [InternalTxtAmtCharged], [NewTxnCcyRate], [NewInternalTxtAmt], [StmtLineSuplementaryDetails], " +
                " [StmtLineRaw], [SubSystem], [CardNo], [AccNo]," +
                " [IsLocalCcyAcc], " +
                " [Matched], [MatchedRunningCycle], [ToBeConfirmed], [UniqueMatchingNo], [SystemMatchingDtTm]," +
                " [MatchedType], [UnMatchedType], [IsException], [ExceptionId], [ExceptionNo], " +
                " [ActionByUser], [UserId], [Authoriser], [AuthoriserDtTm], [ActionType], [SettledRecord]," +
                " [IsAdjustment], [AdjGlAccount], [IsAdjClosed]," +
                 "[Operator]) "
                + " VALUES (@Origin, @StmtTrxReferenceNumber, @StmtExternalBankID, @StmtAccountID, " +
                " @StmtNumber, @StmtSequenceNumber, " +
                " @MerchantId,@MerchantName, @TypeOfTerminal, @TerminalId, " +
                " @TxnValueDate,@TxnEntryDate, @TxnCode, @TxnDesc, " +
                " @RRN,@TxnAmt, @TxnCcy, @TxnCcyRate, " +
                " @InternalTxtAmtCharged, @NewTxnCcyRate, @NewInternalTxtAmt, @StmtLineSuplementaryDetails, " +
                " @StmtLineRaw, @SubSystem, @CardNo, @AccNo," +
                " @IsLocalCcyAcc, " +
                " @Matched, @MatchedRunningCycle, " +
                " @ToBeConfirmed, @UniqueMatchingNo, @SystemMatchingDtTm, @MatchedType, @UnMatchedType," +
                " @IsException, @ExceptionId, @ExceptionNo, @ActionByUser, @UserId, @Authoriser, " +
                " @AuthoriserDtTm, @ActionType, @SettledRecord," +
                " @IsAdjustment, @AdjGlAccount, @IsAdjClosed," +
                " @Operator) "
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@StmtTrxReferenceNumber", StmtTrxReferenceNumber);
                        cmd.Parameters.AddWithValue("@StmtExternalBankID", StmtExternalBankID);
                        cmd.Parameters.AddWithValue("@StmtAccountID", StmtAccountID);
                        cmd.Parameters.AddWithValue("@StmtNumber", StmtNumber);
                        cmd.Parameters.AddWithValue("@StmtSequenceNumber", StmtSequenceNumber);

                        cmd.Parameters.AddWithValue("@MerchantId", MerchantId);
                        cmd.Parameters.AddWithValue("@MerchantName", MerchantName);
                        cmd.Parameters.AddWithValue("@TypeOfTerminal", TypeOfTerminal);
                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);

                        cmd.Parameters.AddWithValue("@TxnValueDate", TxnValueDate);
                        cmd.Parameters.AddWithValue("@TxnEntryDate", TxnEntryDate);
                        cmd.Parameters.AddWithValue("@TxnCode", TxnCode);
                        cmd.Parameters.AddWithValue("@TxnDesc", TxnDesc);

                        cmd.Parameters.AddWithValue("@RRN", RRN);
                        cmd.Parameters.AddWithValue("@TxnAmt", TxnAmt);
                        cmd.Parameters.AddWithValue("@TxnCcy", TxnCcy);
                        cmd.Parameters.AddWithValue("@TxnCcyRate", TxnCcyRate);

                        cmd.Parameters.AddWithValue("@InternalTxtAmtCharged", InternalTxtAmtCharged);
                        cmd.Parameters.AddWithValue("@NewTxnCcyRate", NewTxnCcyRate);
                        cmd.Parameters.AddWithValue("@NewInternalTxtAmt", NewInternalTxtAmt);
                        cmd.Parameters.AddWithValue("@StmtLineSuplementaryDetails", StmtLineSuplementaryDetails);

                        cmd.Parameters.AddWithValue("@StmtLineRaw", StmtLineRaw);
                        cmd.Parameters.AddWithValue("@SubSystem", SubSystem);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);

                        cmd.Parameters.AddWithValue("@IsLocalCcyAcc", IsLocalCcyAcc);

                        cmd.Parameters.AddWithValue("@Matched", Matched);
                        cmd.Parameters.AddWithValue("@MatchedRunningCycle", MatchedRunningCycle);
                        cmd.Parameters.AddWithValue("@ToBeConfirmed", ToBeConfirmed);
                        cmd.Parameters.AddWithValue("@UniqueMatchingNo", UniqueMatchingNo);
                        cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                        cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);
                        cmd.Parameters.AddWithValue("@IsException", IsException);
                        cmd.Parameters.AddWithValue("@ExceptionId", ExceptionId);
                        cmd.Parameters.AddWithValue("@ExceptionNo", ExceptionNo);
                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);
                        cmd.Parameters.AddWithValue("@IsAdjustment", IsAdjustment);

                        cmd.Parameters.AddWithValue("@AdjGlAccount", AdjGlAccount);
                        cmd.Parameters.AddWithValue("@IsAdjClosed", IsAdjClosed);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return SeqNo;
        }

        //
        // Methods 
        // READ TOTALs FOR View UnMatched
        // FILL UP A TABLE
        //
        public int TotalUnMatchedWithNoAction;
        public int TotalUnMatchedInProcess;
        public int TotalUnMatchedSettled;

        public void ReadUnMatchedTxnsMasterPoolATMsTotals(string InSelectionString)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //TotalUnMatchedThisCycle = 0;
            TotalUnMatchedWithNoAction = 0;
            TotalUnMatchedInProcess = 0;
            TotalUnMatchedSettled = 0;

            SqlString = "SELECT *"
                   + " FROM " + SQLStmtInternalLines
                   + InSelectionString;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            GetReaderFields(rdr);

                            // Update Totals 

                            //TotalUnMatchedThisCycle = TotalUnMatchedThisCycle + 1;

                            if (Matched == false & ActionType == "0") // UnMatched with No action
                            {
                                TotalUnMatchedWithNoAction = TotalUnMatchedWithNoAction + 1;
                            }
                            if (Matched == false & ActionType != "0" & SettledRecord == false) // UnMatched with action but unsettled
                            {
                                TotalUnMatchedInProcess = TotalUnMatchedInProcess + 1;
                            }
                            if (Matched == false & ActionType != "0" & SettledRecord == true) // UnMatched Settled
                            {
                                TotalUnMatchedSettled = TotalUnMatchedSettled + 1;
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
        // DELETE Adjustment specific 
        //
        public void DeleteAdjustmentsForInternal(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + "[ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                    + " WHERE SeqNo =  @SeqNo AND Origin = 'WAdjustment'", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

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
        // DELETE Adjustments For Internal where actiontype =3; 
        //
        public void DeleteAdjustmentsForInternal(string InInternalAcc)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVCards_Internal_File_Authorisation]"
                            + " WHERE StmtAccountID =  @StmtAccountID AND Origin = 'WAdjustment' AND ActionType = '3' AND SettledRecord = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@StmtAccountID", InInternalAcc);

                        //rows number of record got updated

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
        // Load Word to the table  
        //
        public DataTable TableTxnsFromWord = new DataTable();
        public void InitialiseWordToTable(string WOperator)
        {
            ErrorFound = false;
            ErrorOutput = "";
            // ADD ENTRY TO TABLE 

            try
            {
                TableTxnsFromWord = new DataTable();
                TableTxnsFromWord.Clear();

                TableTxnsFromWord.Columns.Add("BranchId", typeof(string));
                TableTxnsFromWord.Columns.Add("Trans Amount", typeof(string));
                TableTxnsFromWord.Columns.Add("Ref one", typeof(string));
                TableTxnsFromWord.Columns.Add("Date", typeof(string));
                TableTxnsFromWord.Columns.Add("Time", typeof(string));
                TableTxnsFromWord.Columns.Add("Ref Two", typeof(string));
              
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }
        public void LoadRowToWordTable(string InTk3, string InTk4, string InTk5, string InTk6, string InTk7)
        {
            ErrorFound = false;
            ErrorOutput = "";
            // ADD ENTRY TO TABLE 

            try
            {
                DataRow RowSelected2 = TableTxnsFromWord.NewRow();
             
                RowSelected2["BranchId"] = "NotFound";

                RowSelected2["Trans Amount"] = InTk3;
                RowSelected2["Ref one"] = InTk4;
                RowSelected2["Date"] = InTk5;
                RowSelected2["Time"] = InTk6;
                RowSelected2["Ref Two"] = InTk7;

                // ADD ROW
                TableTxnsFromWord.Rows.Add(RowSelected2);
            }
            catch (Exception ex)
            {
             CatchDetails(ex);
            }
          
       
        }

      

    }
}



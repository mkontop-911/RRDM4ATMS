using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMNVStatement_Lines_InternalAndExternal : Logger
    {
        public RRDMNVStatement_Lines_InternalAndExternal() : base() { }

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

        // SOURCE 

        public string SubSystem;
        public string AccOrCardNo;

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

        public string ActionType; // 00 ... No Action Taken 
                                  // 01 ... Meta Exception Suggested By System 
                                  // 02 ... Meta Exception Manual 
                                  // 03 ... Match it with other - MANUAL at Form502aNostro
                                  // 04 ... Postpone 
                                  // 05 ... Close it 
                                  // 07 ... Default By System
                                  // 08 ... UnDo Default

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

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
   
        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //string SQLStmtInternalLines = "[ATMS].[dbo].[NVStatement_Lines_Internal]";

        //************************************************
        //

        public DataTable TableNVStatement_Lines_Both = new DataTable();

        public DataTable TableNVStatement_Lines_Single = new DataTable();

        public DataTable TableManyToManySummary = new DataTable();

        public DataTable TableManyToManyTxns = new DataTable();

        public DataTable TableAgingAnalysis = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Reader Fields 
        private void GetReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            Origin = (string)rdr["Origin"];
            StmtTrxReferenceNumber = (string)rdr["StmtTrxReferenceNumber"];

            StmtExternalBankID = (string)rdr["StmtExternalBankID"];
            StmtAccountID = (string)rdr["StmtAccountID"];
            StmtNumber = (int)rdr["StmtNumber"];

            StmtSequenceNumber = (int)rdr["StmtSequenceNumber"];

            StmtLineValueDate = (DateTime)rdr["StmtLineValueDate"];
            StmtLineEntryDate = (DateTime)rdr["StmtLineEntryDate"];
            StmtLineIsReversal = (bool)rdr["StmtLineIsReversal"];
            StmtLineIsDebit = (bool)rdr["StmtLineIsDebit"];

            StmtLineFundsCode = (string)rdr["StmtLineFundsCode"];
            StmtLineAmt = (decimal)rdr["StmtLineAmt"];

            StmtLineTrxType = (string)rdr["StmtLineTrxType"];
            StmtLineTrxCode = (string)rdr["StmtLineTrxCode"];
            StmtLineRefForAccountOwner = (string)rdr["StmtLineRefForAccountOwner"];

            StmtLineRefForServicingBank = (string)rdr["StmtLineRefForServicingBank"];
            StmtLineSuplementaryDetails = (string)rdr["StmtLineSuplementaryDetails"];
            StmtLineRaw = (string)rdr["StmtLineRaw"];

            SubSystem = (string)rdr["SubSystem"];
            AccOrCardNo = (string)rdr["AccOrCardNo"];

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

            Ccy = (string)rdr["Ccy"];
            CcyRate = (decimal)rdr["CcyRate"];
            AdjGlAccount = (string)rdr["AdjGlAccount"];
            IsAdjClosed = (bool)rdr["IsAdjClosed"];

            Operator = (string)rdr["Operator"];
        }
        //
        // Methods 
        // READ EXTERNAL
        // 
        //
        public void ReadNVStatement_Lines_ExternalBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVStatement_Lines_External]"
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
        public void ReadNVStatement_Lines_InternalBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal] "
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
        public void ReadNVStatements_Lines_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
             + InSelectionCriteria
             + " UNION ALL"
             + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
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

        public void ReadNVStatements_LinesByMode(string InOperator, string InSignedId, string InSubSystem, int InMode,
                                           int InRunningJobNo, string InExternalAccno, string InternalAccNo,
                                           string InOrderCriteria, DateTime InStmtLineEntryDate, string InReference)

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

            TableNVStatement_Lines_Both = new DataTable();
            TableNVStatement_Lines_Both.Clear();
            TotalSelected = 0;

            // NOSTRO 
            if (InSubSystem == "NostroReconciliation" || InSubSystem == "")
            {
                // DATA TABLE ROWS DEFINITION 
                TableNVStatement_Lines_Both.Columns.Add("SeqNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("ColorNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("MatchingNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("MatchedType", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Origin", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("AccNo", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("DONE", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Code", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("ValueDate", typeof(DateTime));
                TableNVStatement_Lines_Both.Columns.Add("EntryDate", typeof(DateTime));
                TableNVStatement_Lines_Both.Columns.Add("DR/CR", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("Amt", typeof(decimal));

                TableNVStatement_Lines_Both.Columns.Add("OurRef", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("TheirRef", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("OtherDetails", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Ccy", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("CcyRate", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("GLAccount", typeof(string));
            }
            // VISA 
            if (InSubSystem == "CardsSettlement" || InSubSystem == "E_FINANCE")
            {

                // DATA TABLE ROWS DEFINITION 
                TableNVStatement_Lines_Both.Columns.Add("SeqNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("ColorNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("MatchingNo", typeof(int));
                TableNVStatement_Lines_Both.Columns.Add("MatchedType", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Origin", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("CardNo", typeof(string)); // 

                TableNVStatement_Lines_Both.Columns.Add("DONE", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Code", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("ValueDate", typeof(DateTime)); // 
                TableNVStatement_Lines_Both.Columns.Add("EntryDate", typeof(DateTime));
                TableNVStatement_Lines_Both.Columns.Add("DR/CR", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("Amt", typeof(decimal)); // 

                TableNVStatement_Lines_Both.Columns.Add("RRN", typeof(string)); // 
                TableNVStatement_Lines_Both.Columns.Add("TermId", typeof(string)); // 
                TableNVStatement_Lines_Both.Columns.Add("OtherDetails", typeof(string));

                TableNVStatement_Lines_Both.Columns.Add("Ccy", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("CcyRate", typeof(string));
                TableNVStatement_Lines_Both.Columns.Add("GLAccount", typeof(string));
            }

            if (InMode == 1) // Matched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND StmtAccountID = @StmtAccountID "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE[MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND StmtAccountID = @InternalAccNo "
                   + " AND ((Origin = 'INTERNAL' AND Matched = 1 AND SettledRecord = 1 ) OR (Origin = 'WAdjustment' AND [Matched] = 1 AND [SettledRecord] = 1 AND IsAdjClosed = 0)) "
                + " ORDER BY [UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 2) // To be confirmed this pair 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @StmtAccountID  AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }
            if (InMode == 3) // Unmatched 
            {
                SqlString =
              " ; with cte "
                 + " as "
                 + "("
                + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '00'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '00'"
                + ")"
                + "SELECT * from cte "
                  + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 4) // UnMatched with ActionType = '03'
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '03'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '03'";
            }
            if (InMode == 5) // Unmatched less or equal working date 
            {
                SqlString =
              " ; with cte "
                 + " as "
                 + "("
                + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '00' AND StmtLineEntryDate <= @StmtLineEntryDate "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '00' AND StmtLineEntryDate <= @StmtLineEntryDate "
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
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0 AND ActionType = '00' AND StmtLineEntryDate <= @StmtLineEntryDate "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                 + " AND [Matched] = 0  AND ActionType = '00' AND StmtLineEntryDate <= @StmtLineEntryDate "
                + ")"
                + "SELECT * from cte "
                + " WHERE StmtLineRefForAccountOwner LIKE '%" + InReference + "%' "
                + " OR StmtLineRefForServicingBank LIKE '%" + InReference + "%' "
                + " OR StmtLineSuplementaryDetails LIKE '%" + InReference + "%' "
                + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 7) // Show All Adjustments not closed and Action type = 0 for this Internal Account 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                      + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem "
                      + " AND IsAdjustment = 1 AND ActionType = '00'"
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  "
                      + " ORDER BY " + InOrderCriteria;
            }
            if (InMode == 57) // Show All Adjustments not closed and Action type = 0 for all Internal Accounts 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                      + " WHERE IsAdjustment = 1 AND ActionType = '00' AND SubSystem = @SubSystem "
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  "
                      + " ORDER BY StmtAccountID Asc ";
            }
            if (InMode == 8) // Show All Adjustments not closed and Action type = 0 
            {
                SqlString =
                    " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                      + " WHERE StmtAccountID = @InternalAccNo "
                      + " AND IsAdjustment = 1 AND ActionType = '03'"
                      + " AND ((Origin = 'INTERNAL' AND Matched = 0 ) OR (Origin = 'WAdjustment'  AND SettledRecord = 1 AND IsAdjClosed = 0))  ";
            }

            if (InMode == 9) // Auto Matched THIS Cycle 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE [MatchedRunningCycle] = @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND ((Origin = 'INTERNAL' AND Matched = 1 AND SettledRecord = 1 ) OR (Origin = 'WAdjustment' AND [Matched] = 1 AND [SettledRecord] = 1 AND IsAdjClosed = 0)) "
                + " ORDER BY [UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 10) // This Cycle to be confirmend 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE  [MatchedRunningCycle] <= @MatchedRunningCycle AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE  [MatchedRunningCycle] <= @MatchedRunningCycle  AND SubSystem = @SubSystem "
                   + " AND [Matched] = 1 AND [ToBeConfirmed] = 1 AND [SettledRecord] = 0 "
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 11) // All Remaining UnMatched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE SubSystem = @SubSystem  "
                   + " AND [Matched] = 0  AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE  SubSystem = @SubSystem  "
                   + " AND [Matched] = 0 AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " ORDER BY StmtExternalBankID, Ccy, Origin  ASC";
            }

            if (InMode == 12) // The Alerts for pair 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE  StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem  "
                   + " AND [Matched] = 0  AND IsException = 1 AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                   + " AND [Matched] = 0  AND IsException = 1 AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " ORDER BY StmtExternalBankID, Ccy, Origin  ASC";
            }


            if (InMode == 13) // The Alerts for all
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE  SubSystem = @SubSystem   "
                   + " AND [Matched] = 0  AND IsException = 1 AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE SubSystem = @SubSystem   "
                   + " AND [Matched] = 0  AND IsException = 1 AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " ORDER BY StmtExternalBankID, Ccy, Origin  ASC";
            }

            if (InMode == 15) // Matched with reference like 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = @SubSystem  "
                 + " AND  [Matched] = 1 AND [SettledRecord] = 1 "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND  [Matched] = 1 AND [SettledRecord] = 1 "
                + ")"
                + "SELECT * from cte "
                + " WHERE StmtLineRefForAccountOwner LIKE '%" + InReference + "%' "
                + " OR StmtLineRefForServicingBank LIKE '%" + InReference + "%' "
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
                            cmd.Parameters.AddWithValue("@StmtLineEntryDate", InStmtLineEntryDate);
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
                            // Fill Table Line for NOSTRO
                            //
                            if (InSubSystem == "NostroReconciliation" || InSubSystem == "")
                            {

                                DataRow RowSelected = TableNVStatement_Lines_Both.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["ColorNo"] = ColorNo;

                                RowSelected["MatchingNo"] = UniqueMatchingNo;
                                RowSelected["MatchedType"] = MatchedType;

                                RowSelected["Origin"] = Origin;

                                RowSelected["AccNo"] = StmtAccountID;

                                //RowSelected["DONE"] = "YES";

                                if (InMode == 2 & (ActionType == "00" || ActionType == "0")) RowSelected["DONE"] = "YES";
                                if (InMode == 2 & ActionType == "08") RowSelected["DONE"] = "NO";

                                RowSelected["Code"] = StmtLineTrxCode;
                                RowSelected["ValueDate"] = StmtLineValueDate;
                                RowSelected["EntryDate"] = StmtLineEntryDate;

                                if (StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                                else RowSelected["DR/CR"] = "CR";

                                RowSelected["Amt"] = StmtLineAmt;

                                RowSelected["OurRef"] = "  " + StmtLineRefForAccountOwner;

                                RowSelected["TheirRef"] = StmtLineRefForServicingBank;
                                RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;

                                RowSelected["Ccy"] = Ccy;
                                RowSelected["CcyRate"] = CcyRate;
                                RowSelected["GLAccount"] = AdjGlAccount;

                                // ADD ROW

                                TableNVStatement_Lines_Both.Rows.Add(RowSelected);

                                TotalSelected = TotalSelected + 1;
                            }

                            //
                            // Fill Table Line for VISA
                            //
                            if (InSubSystem == "CardsSettlement" || InSubSystem == "E_FINANCE")
                            {

                                DataRow RowSelected = TableNVStatement_Lines_Both.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["ColorNo"] = ColorNo;

                                RowSelected["MatchingNo"] = UniqueMatchingNo;
                                RowSelected["MatchedType"] = MatchedType;

                                RowSelected["Origin"] = Origin;

                                RowSelected["CardNo"] = AccOrCardNo;

                                if (InMode == 2 & ActionType == "00") RowSelected["DONE"] = "YES";
                                if (InMode == 2 & ActionType == "08") RowSelected["DONE"] = "NO";

                                RowSelected["Code"] = StmtLineTrxCode;
                                RowSelected["ValueDate"] = StmtLineValueDate;
                                RowSelected["EntryDate"] = StmtLineEntryDate;

                                if (StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                                else RowSelected["DR/CR"] = "CR";

                                RowSelected["Amt"] = StmtLineAmt;

                                RowSelected["RRN"] = "  " + StmtLineRefForAccountOwner;

                                RowSelected["TermID"] = StmtLineRefForServicingBank;
                                RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;

                                RowSelected["Ccy"] = Ccy;
                                RowSelected["CcyRate"] = CcyRate;
                                RowSelected["GLAccount"] = AdjGlAccount;

                                // ADD ROW

                                TableNVStatement_Lines_Both.Rows.Add(RowSelected);

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

                    if (InSubSystem == "NostroReconciliation" || InSubSystem == "")
                    {

                        //Clear Table 
                        Tr.DeleteReport64(InSignedId);

                        int I = 0;

                        while (I <= (TableNVStatement_Lines_Both.Rows.Count - 1))
                        {
                            //    RecordFound = true;

                            Tr.SeqNo = (int)TableNVStatement_Lines_Both.Rows[I]["SeqNo"];
                            Tr.ColorNo = (int)TableNVStatement_Lines_Both.Rows[I]["ColorNo"];
                            Tr.MatchingNo = (int)TableNVStatement_Lines_Both.Rows[I]["MatchingNo"];
                            Tr.Origin = (string)TableNVStatement_Lines_Both.Rows[I]["Origin"];

                            if (InMode == 2) Tr.DONE = (string)TableNVStatement_Lines_Both.Rows[I]["DONE"];
                            else Tr.DONE = "";

                            Tr.TxnType = (string)TableNVStatement_Lines_Both.Rows[I]["Code"];

                            Tr.ValueDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["ValueDate"];
                            Tr.EntryDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["EntryDate"];

                            Tr.DRCR = (string)TableNVStatement_Lines_Both.Rows[I]["DR/CR"];
                            Tr.Ccy = (string)TableNVStatement_Lines_Both.Rows[I]["Ccy"];
                            Tr.Amt = (decimal)TableNVStatement_Lines_Both.Rows[I]["Amt"];

                            Tr.OurRef = (string)TableNVStatement_Lines_Both.Rows[I]["OurRef"];
                            Tr.TheirRef = (string)TableNVStatement_Lines_Both.Rows[I]["TheirRef"];
                            Tr.OtherDetails = (string)TableNVStatement_Lines_Both.Rows[I]["OtherDetails"];

                            // Insert record for printing 
                            //
                            Tr.InsertReport64(InSignedId);

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
        + " SELECT StmtLineRefForAccountOwner, StmtLineValueDate ,SUM([StmtLineAmt])  As A, COUNT(SeqNo) As B "
        + " FROM[ATMS].[dbo].[NVStatement_Lines_External]"
        + " WHERE StmtAccountID = @ExternalAccID AND SubSystem = @SubSystem "
        + "  AND [Matched] = 0 AND StmtLineEntryDate <= @WorkingDt "
        + " Group By StmtLineRefForAccountOwner , StmtLineValueDate "
        + " UNION ALL"
        + " SELECT StmtLineRefForAccountOwner, StmtLineValueDate, SUM([StmtLineAmt]) As A, COUNT(SeqNo) As B "
        + " FROM[ATMS].[dbo].[NVStatement_Lines_Internal]  "
        + " WHERE StmtAccountID = @InternalAccID AND SubSystem =  @SubSystem "
        + " AND [Matched] = 0 AND StmtLineEntryDate <= @WorkingDt "
        + " Group By StmtLineRefForAccountOwner , StmtLineValueDate "
        + " )"
        + " SELECT StmtLineRefForAccountOwner As Reference, StmtLineValueDate As ValueDt, " 
        + "        SUM(A)As Difference, SUM(B) As Lines "
        + " From cte group by StmtLineRefForAccountOwner, StmtLineValueDate Having SUM(A) = 0 ";
       
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

                    int I = 0;

                    while (I <= (TableManyToManySummary.Rows.Count - 1))
                    {
                        //    RecordFound = true;

                        string WReference = (string)TableManyToManySummary.Rows[I]["Reference"];
                        DateTime WValueDt = (DateTime)TableManyToManySummary.Rows[I]["ValueDt"];

                        int WMode = I;

                        ReadAndFillTableManyToManyTxns(InOperator, InSignedId, WMode,
                                  InExternalAccno, InternalAccNo, WValueDt, InWorkingDt, WReference);

                        I++; // Read Next entry of the table 

                    }

                    if (TableManyToManyTxns.Rows.Count > 0)
                    {
                        //System.Windows.Forms.MessageBox.Show("Number of trxs = " + TableManyToManyTxns.Rows.Count.ToString());

                        int K = 0;

                        PreviousReference = "";

                        while (K <= (TableManyToManyTxns.Rows.Count - 1))
                        {

                            int WSeqNo = (int)TableManyToManyTxns.Rows[K]["SeqNo"];

                            string WOrigin = (string)TableManyToManyTxns.Rows[K]["Origin"];

                            string WReference = (string)TableManyToManyTxns.Rows[K]["OurRef"];

                            if (WReference != PreviousReference)
                            {
                                // Get Unique Id 
                                WUnique = Gu.GetNextValue();
                                // Assign ref
                                PreviousReference = WReference;
                            }

                            if (WOrigin == "EXTERNAL")
                            {
                                SelectionCriteria = " WHERE SeqNo =" + WSeqNo;
                                ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);
                                Matched = true;
                                MatchedRunningCycle = InReconcCycleNo;


                                ToBeConfirmed = false;
                                ActionType = "00";
                                MatchedType = "SystemDefault";

                                UniqueMatchingNo = WUnique;

                                UserId = InSignedId;

                                SettledRecord = true;

                                UpdateExternalFooter(Operator, SelectionCriteria);

                            }

                            if (WOrigin == "INTERNAL")
                            {
                                SelectionCriteria = " WHERE SeqNo =" + WSeqNo;
                                ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);
                                Matched = true;
                                MatchedRunningCycle = InReconcCycleNo;

                                ToBeConfirmed = false;
                                ActionType = "00";
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
                                   string InExternalAccno, string InternalAccNo, DateTime InValueDt, DateTime InWorkingDt, string InReference)
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

                TableManyToManyTxns.Columns.Add("ValueDate", typeof(DateTime));
                TableManyToManyTxns.Columns.Add("EntryDate", typeof(DateTime));
                TableManyToManyTxns.Columns.Add("DR/CR", typeof(string));
                TableManyToManyTxns.Columns.Add("Ccy", typeof(string));
                TableManyToManyTxns.Columns.Add("Amt", typeof(decimal));
                TableManyToManyTxns.Columns.Add("StmtBal", typeof(decimal));

                TableManyToManyTxns.Columns.Add("OurRef", typeof(string));
                TableManyToManyTxns.Columns.Add("TheirRef", typeof(string));
                TableManyToManyTxns.Columns.Add("OtherDetails", typeof(string));

            }

            TotalSelected = 0;

            SqlString =
             " ; with cte "
             + " as "
             + "("
            + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
             + " WHERE StmtAccountID = @ExternalAccno "
             + " AND StmtLineEntryDate <= @WorkingDt "
             + " UNION ALL"
             + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
             + " WHERE StmtAccountID = @InternalAccNo "
             + " AND StmtLineEntryDate <= @WorkingDt "
            + ")"
            + "SELECT * from cte "
            + " WHERE StmtLineRefForAccountOwner = @Reference And Cast(StmtLineValueDate As Date) = @ValueDt "
            + " ORDER BY Origin ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Reference", InReference);
                        cmd.Parameters.AddWithValue("@ExternalAccno", InExternalAccno);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@ValueDt", InValueDt);
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

                            RowSelected["Code"] = StmtLineTrxCode;

                            RowSelected["ValueDate"] = StmtLineValueDate;
                            RowSelected["EntryDate"] = StmtLineEntryDate;

                            if (StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";
                            RowSelected["Ccy"] = Ccy;
                            RowSelected["Amt"] = StmtLineAmt;

                            RowSelected["StmtBal"] = StmtBal;

                            RowSelected["OurRef"] = StmtLineRefForAccountOwner;

                            RowSelected["TheirRef"] = StmtLineRefForServicingBank;
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

            TableNVStatement_Lines_Single = new DataTable();
            TableNVStatement_Lines_Single.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableNVStatement_Lines_Single.Columns.Add("SeqNo", typeof(int));
            TableNVStatement_Lines_Single.Columns.Add("Status", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("Setl", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("MatchingNo", typeof(int));
            TableNVStatement_Lines_Single.Columns.Add("Origin", typeof(string));

            TableNVStatement_Lines_Single.Columns.Add("Code", typeof(string));

            TableNVStatement_Lines_Single.Columns.Add("ValueDate", typeof(DateTime));
            TableNVStatement_Lines_Single.Columns.Add("EntryDate", typeof(DateTime));
            TableNVStatement_Lines_Single.Columns.Add("DR/CR", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("Ccy", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("Amt", typeof(decimal));
            TableNVStatement_Lines_Single.Columns.Add("StmtBal", typeof(decimal));

            TableNVStatement_Lines_Single.Columns.Add("OurRef", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("TheirRef", typeof(string));
            TableNVStatement_Lines_Single.Columns.Add("OtherDetails", typeof(string));

            if (InMode == 11) // INTERNAL 
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                   + " AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " Order By SeqNo ";
            }

            if (InMode == 12) // EXTERNAL 
            {
                SqlString =
                     " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                   + " AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " Order By SeqNo ";
            }

            if (InMode == 17) // UniqueMatchingNo
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE UniqueMatchingNo = " + InMatchingNo
                + " ORDER BY Origin ";
            }


            if (InMode == 18) // StmtLineAmt
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE ABS(StmtLineAmt) = " + InAmt
                + " ORDER BY Origin ";
            }

            if (InMode == 19) // Reference like 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE StmtLineRefForAccountOwner LIKE '%" + InReference + "%' "
                + " OR StmtLineRefForServicingBank LIKE '%" + InReference + "%' "
                + " OR StmtLineSuplementaryDetails LIKE '%" + InReference + "%' "
                + " ORDER BY Origin ";
            }

            if (InMode == 20) // Reference identical 
            {
                SqlString =
                 " ; with cte "
                 + " as "
                 + "("
                + " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @ExternalAccno AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo AND SubSystem = @SubSystem  "
                 + " AND StmtLineEntryDate <= @ToDt "
                + ")"
                + "SELECT * from cte "
                + " WHERE StmtLineRefForAccountOwner ='" + InReference + "' "
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

                            DataRow RowSelected = TableNVStatement_Lines_Single.NewRow();

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

                            RowSelected["Code"] = StmtLineTrxCode;

                            RowSelected["ValueDate"] = StmtLineValueDate;
                            RowSelected["EntryDate"] = StmtLineEntryDate;

                            if (StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";
                            RowSelected["Ccy"] = Ccy;
                            RowSelected["Amt"] = StmtLineAmt;

                            RowSelected["StmtBal"] = StmtBal;

                            RowSelected["OurRef"] = StmtLineRefForAccountOwner;

                            RowSelected["TheirRef"] = StmtLineRefForServicingBank;
                            RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;

                            // ADD ROW

                            TableNVStatement_Lines_Single.Rows.Add(RowSelected);

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

                    while (I <= (TableNVStatement_Lines_Single.Rows.Count - 1))
                    {
                        //    RecordFound = true;


                        Tr.SeqNo = (int)TableNVStatement_Lines_Single.Rows[I]["SeqNo"];
                        Tr.Status = (string)TableNVStatement_Lines_Single.Rows[I]["Status"];
                        Tr.Settled = (string)TableNVStatement_Lines_Single.Rows[I]["Setl"];
                        Tr.MatchingNo = (int)TableNVStatement_Lines_Single.Rows[I]["MatchingNo"];
                        Tr.Origin = (string)TableNVStatement_Lines_Single.Rows[I]["Origin"];

                        Tr.TxnType = (string)TableNVStatement_Lines_Single.Rows[I]["Code"];
                        Tr.ValueDate = (DateTime)TableNVStatement_Lines_Single.Rows[I]["ValueDate"];
                        Tr.EntryDate = (DateTime)TableNVStatement_Lines_Single.Rows[I]["EntryDate"];

                        Tr.DRCR = (string)TableNVStatement_Lines_Single.Rows[I]["DR/CR"];
                        Tr.Ccy = (string)TableNVStatement_Lines_Single.Rows[I]["Ccy"];
                        Tr.Amt = (decimal)TableNVStatement_Lines_Single.Rows[I]["Amt"];
                        Tr.StmtBal = (decimal)TableNVStatement_Lines_Single.Rows[I]["StmtBal"];

                        Tr.OurRef = (string)TableNVStatement_Lines_Single.Rows[I]["OurRef"];
                        Tr.TheirRef = (string)TableNVStatement_Lines_Single.Rows[I]["TheirRef"];
                        Tr.OtherDetails = (string)TableNVStatement_Lines_Single.Rows[I]["OtherDetails"];

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

            TableNVStatement_Lines_Both = new DataTable();
            TableNVStatement_Lines_Both.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableNVStatement_Lines_Both.Columns.Add("SeqNo", typeof(int));
            TableNVStatement_Lines_Both.Columns.Add("ColorNo", typeof(int));
            TableNVStatement_Lines_Both.Columns.Add("MatchingNo", typeof(int));
            TableNVStatement_Lines_Both.Columns.Add("Origin", typeof(string));

            TableNVStatement_Lines_Both.Columns.Add("Code", typeof(string));

            TableNVStatement_Lines_Both.Columns.Add("ValueDate", typeof(DateTime));
            TableNVStatement_Lines_Both.Columns.Add("EntryDate", typeof(DateTime));
            TableNVStatement_Lines_Both.Columns.Add("DR/CR", typeof(string));
            TableNVStatement_Lines_Both.Columns.Add("Ccy", typeof(string));
            TableNVStatement_Lines_Both.Columns.Add("Amt", typeof(decimal));
            TableNVStatement_Lines_Both.Columns.Add("OurRef", typeof(string));
            TableNVStatement_Lines_Both.Columns.Add("TheirRef", typeof(string));
            TableNVStatement_Lines_Both.Columns.Add("OtherDetails", typeof(string));

            TableNVStatement_Lines_Both.Columns.Add("CcyRate", typeof(string));
            TableNVStatement_Lines_Both.Columns.Add("GLAccount", typeof(string));

            if (InMode == 13) // Matched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 1 AND [SettledRecord] = 1"
                   + " ORDER BY[UniqueMatchingNo], Origin ASC";
            }

            if (InMode == 14) // UnMatched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 0 "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 0 "
                   + " ORDER By Origin ASC";
            }

            if (InMode == 15) // Alerts for pair within range
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @ExternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (StmtLineEntryDate > @FromDt AND StmtLineEntryDate < @ToDt) "
                   + " AND [Matched] = 0 AND IsException = 1 AND ExceptionNo = 0"
                   + " ORDER By Origin ASC";

            }

            if (InMode == 16) // Alerts for ALL within range
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE [Matched] = 0  AND IsException = 1 AND ExceptionNo = 0"
                   + " ORDER By Origin ASC";

            }

            if (InMode == 21) // Unique Partially Matched 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @ExternalAccNo  "
                   + " AND UniqueMatchingNo = @UniqueMatchingNo "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo  "
                   + " AND UniqueMatchingNo = @UniqueMatchingNo "
                   + " ORDER BY Origin ASC";
            }

            if (InMode == 3)
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID "
                 + " AND [Matched] = 0 AND ActionType = '00'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo "
                 + " AND [Matched] = 0  AND ActionType = '00'";
            }

            if (InMode == 4)
            {
                SqlString =
                   " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                 + " WHERE StmtAccountID = @StmtAccountID "
                 + " AND [Matched] = 0 AND ActionType = '03'"
                 + " UNION ALL"
                 + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                 + " WHERE StmtAccountID = @InternalAccNo "
                 + " AND [Matched] = 0  AND ActionType = '03'";
            }

            if (InMode == 27) // UnMatched Based on Matchurity dt 
            {
                SqlString =
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND (StmtLineValueDate > @FromDt AND StmtLineValueDate < @ToDt) "
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

                            DataRow RowSelected = TableNVStatement_Lines_Both.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ColorNo"] = ColorNo;
                            RowSelected["MatchingNo"] = UniqueMatchingNo;
                            RowSelected["Origin"] = Origin;

                            RowSelected["Code"] = StmtLineTrxCode;

                            RowSelected["ValueDate"] = StmtLineValueDate;
                            RowSelected["EntryDate"] = StmtLineEntryDate;

                            if (StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";

                            RowSelected["Ccy"] = Ccy;

                            RowSelected["Amt"] = StmtLineAmt;

                            RowSelected["OurRef"] = StmtLineRefForAccountOwner;

                            RowSelected["TheirRef"] = StmtLineRefForServicingBank;
                            RowSelected["OtherDetails"] = StmtLineSuplementaryDetails;


                            RowSelected["CcyRate"] = CcyRate;
                            RowSelected["GLAccount"] = AdjGlAccount;

                            // ADD ROW

                            TableNVStatement_Lines_Both.Rows.Add(RowSelected);

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
                    Tr.DeleteReport64(InSignedId);

                    int I = 0;

                    while (I <= (TableNVStatement_Lines_Both.Rows.Count - 1))
                    {
                        //    RecordFound = true;

                        Tr.SeqNo = (int)TableNVStatement_Lines_Both.Rows[I]["SeqNo"];
                        Tr.ColorNo = (int)TableNVStatement_Lines_Both.Rows[I]["ColorNo"];
                        Tr.MatchingNo = (int)TableNVStatement_Lines_Both.Rows[I]["MatchingNo"];
                        Tr.Origin = (string)TableNVStatement_Lines_Both.Rows[I]["Origin"];

                        if (InMode == 2) Tr.DONE = (string)TableNVStatement_Lines_Both.Rows[I]["DONE"];
                        else Tr.DONE = "";

                        Tr.TxnType = (string)TableNVStatement_Lines_Both.Rows[I]["Code"];
                        Tr.ValueDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["ValueDate"];
                        Tr.EntryDate = (DateTime)TableNVStatement_Lines_Both.Rows[I]["EntryDate"];

                        Tr.DRCR = (string)TableNVStatement_Lines_Both.Rows[I]["DR/CR"];
                        Tr.Ccy = (string)TableNVStatement_Lines_Both.Rows[I]["Ccy"];
                        Tr.Amt = (decimal)TableNVStatement_Lines_Both.Rows[I]["Amt"];

                        Tr.OurRef = (string)TableNVStatement_Lines_Both.Rows[I]["OurRef"];
                        Tr.TheirRef = (string)TableNVStatement_Lines_Both.Rows[I]["TheirRef"];
                        Tr.OtherDetails = (string)TableNVStatement_Lines_Both.Rows[I]["OtherDetails"];

                        // Insert record for printing 
                        //
                        Tr.InsertReport64(InSignedId);

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
                     " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_External]"
                   + " WHERE StmtAccountID = @StmtAccountID AND StmtLineEntryDate <= @StmtLineEntryDate "
                   + " UNION ALL"
                   + " SELECT* FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
                   + " WHERE StmtAccountID = @InternalAccNo AND StmtLineEntryDate <= @StmtLineEntryDate"
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
                        cmd.Parameters.AddWithValue("@StmtLineEntryDate", InDate);

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
                                ExternalAccBalance = ExternalAccBalance + StmtLineAmt;
                                ExternalAccTxns = ExternalAccTxns + 1;

                                if (Matched == false & StmtLineAmt < 0)
                                {
                                    decimal TempExt = -StmtLineAmt;
                                    UnMatchedExternalCR = UnMatchedExternalCR + TempExt;
                                    UnMatchedExternalCRTxns = UnMatchedExternalCRTxns + 1;
                                }

                                if (Matched == false & StmtLineAmt > 0)
                                {
                                    UnMatchedExternalDR = UnMatchedExternalDR + StmtLineAmt;
                                    UnMatchedExternalDRTxns = UnMatchedExternalDRTxns + 1;
                                }
                            }

                            if (Origin == "INTERNAL" || Origin == "WAdjustment")
                            {
                                InternalAccBalance = InternalAccBalance + StmtLineAmt;
                                InternalAccTxns = InternalAccTxns + 1;

                                if (Matched == false & StmtLineAmt < 0)
                                {
                                    decimal TempInt = -StmtLineAmt;
                                    UnMatchedInternalCR = UnMatchedInternalCR + TempInt;
                                    UnMatchedInternalCRTxns = UnMatchedInternalCRTxns + 1;
                                }

                                if (Matched == false & StmtLineAmt > 0)
                                {
                                    UnMatchedInternalDR = UnMatchedInternalDR + StmtLineAmt;
                                    UnMatchedInternalDRTxns = UnMatchedInternalDRTxns + 1;
                                }
                            }

                            if (Origin == "WAdjustment" & IsAdjClosed == false)
                            {
                                MatchedAdjustmentsOpenTxns = MatchedAdjustmentsOpenTxns + 1;
                                if (StmtLineAmt < 0)
                                {
                                    MatchedAdjustmentsOpenNegative = MatchedAdjustmentsOpenNegative + StmtLineAmt;
                                }
                                if (StmtLineAmt > 0)
                                {
                                    MatchedAdjustmentsOpenPositive = MatchedAdjustmentsOpenPositive + StmtLineAmt;
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
        public int TotalNumberProcessed;
        public int MatchedDefault;
        public int AutoButToBeConfirmed;
        public decimal UnMatchedAmt;

        public int MatchedFromAutoToBeConfirmed;
        public int MatchedFromManualToBeConfirmed;
        // Find Totals 
        public void ReadNVExternalStatements_LinesForTotals(string InOperator, string InSignedId, string InSubSystem ,int InMatchedRunningCycle,
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
               " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
             + " WHERE StmtAccountID = @StmtAccountID  AND SubSystem = @SubSystem "
             + " AND StmtTrxReferenceNumber = @StmtTrxReferenceNumber "
             + " AND StmtLineEntryDate <= @StmtLineEntryDate ";
            //}

            //if (InMode == 21) // Nostro
            //{
            //    SqlString =
            //   " SELECT * FROM [ATMS].[dbo].[NVStatement_Lines_External]"
            // + " WHERE StmtAccountID = @StmtAccountID AND SubSystem = 'NostroReconciliation'"
            // + " AND StmtTrxReferenceNumber = @StmtTrxReferenceNumber "
            // + " AND StmtLineEntryDate <= @StmtLineEntryDate ";
            //}


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
                        //cmd.Parameters.AddWithValue("@MatchedRunningCycle", InMatchedRunningCycle);
                        cmd.Parameters.AddWithValue("@StmtLineEntryDate", InDate);

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
                                if (StmtLineAmt > 0)
                                    UnMatchedAmt = UnMatchedAmt + StmtLineAmt;
                                if (StmtLineAmt < 0)
                                    UnMatchedAmt = UnMatchedAmt + (-StmtLineAmt);
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
        public void ReadNVStatement_Lines_InternalBySelectionCriteria2(int InMode,
                                            string InternalAccNo,
                                            string InStmtLineRefForAccountOwner,
                                            DateTime InStmtLineValueDate,
                                            decimal InStmtLineAmt,
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
                    + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    + " AND StmtLineRefForAccountOwner = @StmtLineRefForAccountOwner  "
                    + " AND StmtLineValueDate = @StmtLineValueDate "
                    + " AND StmtLineAmt = @StmtLineAmt  "
                    + " AND StmtLineEntryDate <= @StmtLineEntryDate AND Matched = 0  "
                    ;
            }
            if (InMode == 32) // Find if ANY Reference Like 
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    + " AND StmtLineEntryDate <= @StmtLineEntryDate AND Matched = 0  "
                    + InSelectionCriteria;
            }

            //if (InMode == 31) // All Identical 
            //{
            //    SqlString = "SELECT *"
            //        + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal] "
            //        + " Where StmtAccountID = @StmtAccountID  "
            //        + " AND StmtLineRefForServicingBank = @StmtLineRefForServicingBank  "
            //        + " AND StmtLineValueDate = @StmtLineValueDate "
            //        + " AND StmtLineAmt = @StmtLineAmt  "
            //        + " AND StmtLineEntryDate <= @StmtLineEntryDate AND Matched = 0  "
            //        ;
            //}

            if (InMode == 55) // All Identical 
            {
                SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal] "
                    + " Where StmtAccountID = @StmtAccountID  "
                    + " AND (StmtLineValueDate >= @InDtFrom AND StmtLineValueDate <= @InDtTo)  "
                    + " AND (StmtLineAmt >= @InAmtFrom AND StmtLineAmt <= @InAmtTo)  "
                    + " AND StmtLineEntryDate <= @StmtLineEntryDate AND Matched = 0  "
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

                            cmd.Parameters.AddWithValue("@StmtLineRefForAccountOwner", InStmtLineRefForAccountOwner);
                            cmd.Parameters.AddWithValue("@StmtLineValueDate", InStmtLineValueDate);
                            cmd.Parameters.AddWithValue("@StmtLineAmt", InStmtLineAmt);

                            cmd.Parameters.AddWithValue("@StmtLineEntryDate", InWDate);
                        }

                        if (InMode == 32)
                        {
                            cmd.Parameters.AddWithValue("@StmtAccountID", InternalAccNo);

                            cmd.Parameters.AddWithValue("@StmtLineEntryDate", InWDate);
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

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }
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
               + " StmtLineValueDate,"
               + " StmtLineAmt,"
               + " Matched "
               + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
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
                            StmtLineValueDate = (DateTime)rdr["StmtLineValueDate"];
                            StmtLineAmt = (decimal)rdr["StmtLineAmt"];
                            Matched = (bool)rdr["Matched"];

                            DiffInEntryDays = Convert.ToInt32((InTestingDate.Date - StmtLineValueDate).TotalDays);
                            //int AbsDiffInEntryDays = Math.Abs(DiffInEntryDays);

                            if (DiffInEntryDays > 0)
                            {

                                if (DiffInEntryDays > 0 & DiffInEntryDays <= 4)
                                    Zero_To_4 = Zero_To_4 + Math.Abs(StmtLineAmt);

                                if (DiffInEntryDays > 7 & DiffInEntryDays <= 15)
                                    Seven_To_15 = Seven_To_15 + Math.Abs(StmtLineAmt);

                                if (DiffInEntryDays > 15 & DiffInEntryDays <= 30)
                                    Fifteen_To_30 = Fifteen_To_30 + Math.Abs(StmtLineAmt);

                                if (DiffInEntryDays > 30 & DiffInEntryDays <= 60)
                                    Thirty_To_60 = Thirty_To_60 + Math.Abs(StmtLineAmt);

                                if (DiffInEntryDays > 60)
                                    More_Than_60 = More_Than_60 + Math.Abs(StmtLineAmt);
                            }

                            GrandTotal = GrandTotal + Math.Abs(StmtLineAmt);
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

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }

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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_External] "
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

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal] "
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

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }
                }
        }

        // UPDATE
        //
        // Set Action type for the selection criteria 

        //string InExternalAccno, string InternalAccNo)
        public void UpdateActionTypeExternal(string InOperator, string InSelectionCriteria,
                 DateTime InStmtLineValueDate, string InActionType, DateTime InWdateTm)
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_External] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + InSelectionCriteria + " AND StmtLineEntryDate <= @StmtLineEntryDate", conn))
                    {
                        cmd.Parameters.AddWithValue("@StmtLineValueDate", InStmtLineValueDate);
                        cmd.Parameters.AddWithValue("@StmtLineEntryDate", InWdateTm);
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
                 DateTime InStmtLineValueDate, string InActionType, DateTime InWdateTm)
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal] "
                            + " SET "
                            + " [ActionType] = @ActionType "
                            + InSelectionCriteria + " AND StmtLineEntryDate <= @StmtLineEntryDate", conn))
                    {
                        cmd.Parameters.AddWithValue("@StmtLineValueDate", InStmtLineValueDate);
                        cmd.Parameters.AddWithValue("@StmtLineEntryDate", InWdateTm);
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal] "
                            + " SET "
                            + " [StmtLineIsDebit] = @StmtLineIsDebit, [StmtLineAmt] = @StmtLineAmt, "
                            + " [IsAdjustment]=@IsAdjustment, "
                            + " [Ccy]=@Ccy, "
                            + " [CcyRate]=@CcyRate, "
                            + " [AdjGlAccount]=@AdjGlAccount, "
                            + " [IsAdjClosed]=@IsAdjClosed, "
                            + " [StmtLineSuplementaryDetails]=@StmtLineSuplementaryDetails "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo2);
                        cmd.Parameters.AddWithValue("@StmtLineIsDebit", StmtLineIsDebit);
                        cmd.Parameters.AddWithValue("@StmtLineAmt", StmtLineAmt);
                        cmd.Parameters.AddWithValue("@StmtLineSuplementaryDetails", StmtLineSuplementaryDetails);

                        cmd.Parameters.AddWithValue("@IsAdjustment", IsAdjustment);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@CcyRate", CcyRate);
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_External]"
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal]"
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal]"
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_External]"
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
                            "UPDATE [ATMS].[dbo].[NVStatement_Lines_Internal]"
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
                   + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal]"
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

                            Ccy = (string)rdr["Ccy"];
                            CcyRate = (decimal)rdr["CcyRate"];
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

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVStatement_Lines_Internal] " +
                " ([Origin], [StmtTrxReferenceNumber]," +
                " [StmtExternalBankID],[StmtAccountID], [StmtNumber], [StmtSequenceNumber], [StmtLineValueDate], " +
                "[StmtLineEntryDate], [StmtLineIsReversal], [StmtLineIsDebit], [StmtLineFundsCode], " +
                "[StmtLineAmt], [StmtLineTrxType], [StmtLineTrxCode], [StmtLineRefForAccountOwner]," +
                " [StmtLineRefForServicingBank], [StmtLineSuplementaryDetails], " +
                " [SubSystem], " +
                "[Matched], [MatchedRunningCycle], [ToBeConfirmed], [UniqueMatchingNo], [SystemMatchingDtTm]," +
                " [MatchedType], [UnMatchedType], [IsException], [ExceptionId], [ExceptionNo], " +
                "[ActionByUser], [UserId], [Authoriser], [AuthoriserDtTm], [ActionType], [SettledRecord]," +
                " [IsAdjustment], [Ccy],[CcyRate], [AdjGlAccount], [IsAdjClosed]," +
                 "[Operator]) "
                + " VALUES (@Origin, @StmtTrxReferenceNumber, @StmtExternalBankID, @StmtAccountID, " +
                "@StmtNumber, @StmtSequenceNumber, @StmtLineValueDate, @StmtLineEntryDate, " +
                "@StmtLineIsReversal, @StmtLineIsDebit, @StmtLineFundsCode, @StmtLineAmt, @StmtLineTrxType," +
                " @StmtLineTrxCode, @StmtLineRefForAccountOwner, @StmtLineRefForServicingBank, " +
                "@StmtLineSuplementaryDetails," +
                " @SubSystem, " +
                " @Matched, @MatchedRunningCycle, " +
                "@ToBeConfirmed, @UniqueMatchingNo, @SystemMatchingDtTm, @MatchedType, @UnMatchedType," +
                " @IsException, @ExceptionId, @ExceptionNo, @ActionByUser, @UserId, @Authoriser, " +
                "@AuthoriserDtTm, @ActionType, @SettledRecord," +
                "@IsAdjustment, @Ccy, @CcyRate,@AdjGlAccount, @IsAdjClosed," +
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
                        cmd.Parameters.AddWithValue("@StmtLineValueDate", StmtLineValueDate);
                        cmd.Parameters.AddWithValue("@StmtLineEntryDate", StmtLineEntryDate);
                        cmd.Parameters.AddWithValue("@StmtLineIsReversal", StmtLineIsReversal);
                        cmd.Parameters.AddWithValue("@StmtLineIsDebit", StmtLineIsDebit);
                        cmd.Parameters.AddWithValue("@StmtLineFundsCode", StmtLineFundsCode);
                        cmd.Parameters.AddWithValue("@StmtLineAmt", StmtLineAmt);
                        cmd.Parameters.AddWithValue("@StmtLineTrxType", StmtLineTrxType);
                        cmd.Parameters.AddWithValue("@StmtLineTrxCode", StmtLineTrxCode);
                        cmd.Parameters.AddWithValue("@StmtLineRefForAccountOwner", StmtLineRefForAccountOwner);
                        cmd.Parameters.AddWithValue("@StmtLineRefForServicingBank", StmtLineRefForServicingBank);
                        cmd.Parameters.AddWithValue("@StmtLineSuplementaryDetails", StmtLineSuplementaryDetails);
                        cmd.Parameters.AddWithValue("@SubSystem", SubSystem);
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
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@CcyRate", CcyRate);
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
                   + " FROM [ATMS].[dbo].[NVStatement_Lines_Internal]" 
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

                            if (Matched == false & ActionType == "00") // UnMatched with No action
                            {
                                TotalUnMatchedWithNoAction = TotalUnMatchedWithNoAction + 1;
                            }
                            if (Matched == false & ActionType != "00" & SettledRecord == false) // UnMatched with action but unsettled
                            {
                                TotalUnMatchedInProcess = TotalUnMatchedInProcess + 1;
                            }
                            if (Matched == false & ActionType != "00" & SettledRecord == true) // UnMatched Settled
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVStatement_Lines_Internal]" 
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
        // DELETE Adjustments For Internal where actiontype =03; 
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVStatement_Lines_Internal]" 
                            + " WHERE StmtAccountID =  @StmtAccountID AND Origin = 'WAdjustment' AND ActionType = '03' AND SettledRecord = 0 ", conn))
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

      
  
    }
}

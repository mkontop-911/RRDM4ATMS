using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMMatchingTxns_MasterPoolATMs_ROM : Logger
    {
        public RRDMMatchingTxns_MasterPoolATMs_ROM() : base() { }

        public int SeqNo;

        public string OriginFileName; //* 

        public int OriginalRecordId;

        public string MatchingCateg;
        public string RMCateg;

        public int LoadedAtRMCycle;
        public int MatchingAtRMCycle;

        //MaskRecordId
        //UniqueRecordId
        public int UniqueRecordId;
        //public int UniqueRecordId;
        //public int MaskRecordId;

        public string Origin; //

        public string TerminalType; // "10" = ATM
                                    // "20" = POS
                                    // "30" = CDM
                                    // 
        public int TargetSystem; //
                                 // 1 JCC
                                 // 3 American 
                                 // 5 Our Banking system
                                 // 9 Our Banking system 
        public string TransTypeAtOrigin;//
        public string Product;//
        public string CostCentre;//

        public string TerminalId;
        public int TransType;
        // 11 Withdrawl
        // 21 Deposit
        // 23 BNA
        // 24 Envelope
        // 25 Cheques

        public string TransDescr;
        public string CardNumber;
        public bool IsOwnCard;
        public string AccNumber;

        public string TransCurr;
        public decimal TransAmount;
        public decimal DepCount;

        public DateTime TransDate;

        public int TraceNoWithNoEndZero;

        public int AtmTraceNo;

        public int MasterTraceNo;

        public string RRNumber; // denoted as character

        public string AUTHNUM;

        public int FuID;

        public string ResponseCode;
        public string SpareField; // USED to keep the partner SeqNo

        public string Comments;

        public bool IsMatchingDone;

        public bool Matched;
        public string MatchMask;
        public DateTime SystemMatchingDtTm;

        public string MatchedType; // USED FOR MATCHED TRANSACTIONS 

        public string UnMatchedType; // USED FOR UN-MATCHED TRANSACTIONS 

        public int MetaExceptionId;

        public int MetaExceptionNo;

        public bool FastTrack;

        public bool ActionByUser;

        public string UserId;
        public string Authoriser;

        public DateTime AuthoriserDtTm;

        public string ActionType; // 00 ... No Action Taken 
                                  // 01 ... Meta Exception Suggested By System 
                                  // 11 ... DEBIT Customer
                                  // 21 ... CREDIT Customer
                                  // 02 ... Meta Exception Manual 
                                  // 03 ... Move case to cash reconciliation 
                                  // 04 ... Force Match - Broken Disc Case  
                                  // 05 ... Move To Dispute 
                                  // 06 ... Move To pool => to be matched next day
                                  // 07 ... Default By System
                                  // 08 ... Move case to Replenishment Reconciliation 
                                  // 09 ... Move to Suspense

        public bool NotInJournal; // NotInJournal = 0 
        public bool WaitingForUpdating; // USED FOR MATCHED AUTHORISATIONS 

        public bool SettledRecord; // Currently only for UnMatched 

        public string Operator;

        public string FileId01;
        public int SeqNo01;
        public string FileId02;
        public int SeqNo02;
        public string FileId03;
        public int SeqNo03;
        public string FileId04;
        public int SeqNo04;
        public string FileId05;
        public int SeqNo05;
        public string FileId06;
        public int SeqNo06;
        public string Card_Encrypted;
        public string TXNSRC;
        public string TXNDEST;

        public string ACCEPTOR_ID;
        public string ACCEPTORNAME;

        public DateTime CAP_DATE;
        public int Minutes_Date;
        public DateTime SET_DATE;
        public DateTime Net_TransDate;

        public string UTRNNO;

        public int ReplCycleNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int TotalMatched = 0;
        public decimal TotalAmountMatched = 0;

        public int TotalUnMatched;
        public decimal TotalAmountUnMatched;

        public int TotalNotSettled;
        public decimal TotalNotSettledAmt;

        public int TotalDefaultActionBySystem;
        public decimal TotalAmountDefaultActionBySystem;

        public int TotalActionsByUserDefaultAndManual;
        public decimal TotalAmountByUserDefaultAndManual;

        public int TotalForcedMatched;
        public decimal TotalForcedMatchedAmount;

        public int TotalFastTrack;
        public decimal TotalFastTrackAmount;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
        RRDMMatchingMasksVsMetaExceptions Rme = new RRDMMatchingMasksVsMetaExceptions();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();
        


        //RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //string MatchingMasterFileId = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

        //************************************************
        //

        public DataTable MatchingMasterDataTableATMs = new DataTable();

        public DataTable RMDataTableRight = new DataTable();

        public DataTable DataTableActionsTaken = new DataTable();

        public DataTable DataTableAtmsTotals = new DataTable();

        public DataTable GlAdjustmentTxns = new DataTable();

        public DataTable MaxDates_By_ATM = new DataTable();

        public DataTable TableFullFromMaster = new DataTable();

        public DataTable MpaTable = new DataTable();

        public DataTable TableMasks_1 = new DataTable();
        public DataTable TableMasks_2 = new DataTable();

        public DataTable ManualTXNSTable = new DataTable();

        public int TotalSelected;

        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        readonly string connectionStringITMX = ConfigurationManager.ConnectionStrings
        ["ReconConnectionString"].ConnectionString;


        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OriginFileName = (string)rdr["OriginFileName"];

            OriginalRecordId = (int)rdr["OriginalRecordId"];

            MatchingCateg = (string)rdr["MatchingCateg"];
            RMCateg = (string)rdr["RMCateg"];

            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            MatchingAtRMCycle = (int)rdr["MatchingAtRMCycle"];

            UniqueRecordId = (int)rdr["UniqueRecordId"];

            Origin = (string)rdr["Origin"];

            TerminalType = (string)rdr["TerminalType"];

            TargetSystem = (int)rdr["TargetSystem"];

            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
            Product = (string)rdr["Product"];
            CostCentre = (string)rdr["CostCentre"];

            TerminalId = (string)rdr["TerminalId"];

            TransType = (int)rdr["TransType"];

            TransDescr = (string)rdr["TransDescr"];

            CardNumber = (string)rdr["CardNumber"];

            IsOwnCard = (bool)rdr["IsOwnCard"];

            AccNumber = (string)rdr["AccNumber"];

            TransCurr = (string)rdr["TransCurr"];
            TransAmount = (decimal)rdr["TransAmount"];
            DepCount = (decimal)rdr["DepCount"];

            TransDate = (DateTime)rdr["TransDate"];

            TraceNoWithNoEndZero = (int)rdr["TraceNoWithNoEndZero"];

            AtmTraceNo = (int)rdr["AtmTraceNo"];
            MasterTraceNo = (int)rdr["MasterTraceNo"];

            RRNumber = (string)rdr["RRNumber"];

            AUTHNUM = (string)rdr["AUTHNUM"];

            FuID = (int)rdr["FuID"];

            ResponseCode = (string)rdr["ResponseCode"];
            SpareField = (string)rdr["SpareField"];
            Comments = (string)rdr["Comments"];

            IsMatchingDone = (bool)rdr["IsMatchingDone"];

            Matched = (bool)rdr["Matched"];
            MatchMask = (string)rdr["MatchMask"];

            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

            MatchedType = (string)rdr["MatchedType"];

            UnMatchedType = (string)rdr["UnMatchedType"];

            MetaExceptionId = (int)rdr["MetaExceptionId"];

            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

            FastTrack = (bool)rdr["FastTrack"];

            ActionByUser = (bool)rdr["ActionByUser"];
            UserId = (string)rdr["UserId"];
            Authoriser = (string)rdr["Authoriser"];

            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

            ActionType = (string)rdr["ActionType"];

            NotInJournal = (bool)rdr["NotInJournal"];
            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

            SettledRecord = (bool)rdr["SettledRecord"];

            Operator = (string)rdr["Operator"];

            FileId01 = (string)rdr["FileId01"];
            SeqNo01 = (int)rdr["SeqNo01"];

            FileId02 = (string)rdr["FileId02"];
            SeqNo02 = (int)rdr["SeqNo02"];

            FileId03 = (string)rdr["FileId03"];
            SeqNo03 = (int)rdr["SeqNo03"];

            FileId04 = (string)rdr["FileId04"];
            SeqNo04 = (int)rdr["SeqNo04"];

            FileId05 = (string)rdr["FileId05"];
            SeqNo05 = (int)rdr["SeqNo05"];

            FileId06 = (string)rdr["FileId06"];
            SeqNo06 = (int)rdr["SeqNo06"];

            ReplCycleNo = (int)rdr["ReplCycleNo"];
            Card_Encrypted = (string)rdr["Card_Encrypted"];
            TXNSRC = (string)rdr["TXNSRC"];
            TXNDEST = (string)rdr["TXNDEST"];

            ACCEPTOR_ID = (string)rdr["ACCEPTOR_ID"];
            ACCEPTORNAME = (string)rdr["ACCEPTORNAME"];

            CAP_DATE = (DateTime)rdr["CAP_DATE"];
            SET_DATE = (DateTime)rdr["SET_DATE"];
            Net_TransDate = (DateTime)rdr["Net_TransDate"];
            UTRNNO = (string)rdr["UTRNNO"];
        }

        // Read Field Names 
        //
        //
        // 
        //
        public DataTable OutputFieldsDataTable = new DataTable();
        public void ReadTablePoolDataToGetFieldNames(string InSelectionCriteria)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            OutputFieldsDataTable = new DataTable();
            OutputFieldsDataTable.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 


            //SqlString = "SELECT *"
            //   + " FROM [ATMS].[dbo].[OutputFileDefinition]"
            //   + InSelectionCriteria
            //   + " Order By  OutputFile_Id, OutputFile_Version";

            SqlString = " SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale "
            + " FROM sys.columns "
            + " WHERE[object_id] = OBJECT_ID('[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]')"
            + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(OutputFieldsDataTable);

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
        //
        //
        // 
        //

        public void ReadTablePoolDataToGetTableByCriteria(string InSelectionCriteria, int In_DB_Mode)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In_DB_Mode = 1 then it is a unique data base = the unmatched
            // In_DB_Mode = 2 Then we need both.

            MpaTable = new DataTable();
            MpaTable.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 
            if (In_DB_Mode == 1)
            {
                SqlString = " SELECT * "
                           + "FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                           + InSelectionCriteria
                           + " ";
            }


            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                + "  ";
            }

            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(MpaTable);

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
        // FILL MASK 1
        //

        public void ReadTablePoolDataToGetMaskTableMasks_1(int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_1 = new DataTable();
            TableMasks_1.Clear();

            SqlString =
             " SELECT MatchMask, MatchingCateg, Count(*) as number "
             + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask, MatchingCateg "
             + " order by MatchingCateg ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_1);

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

        public void ReadTablePoolDataToGetMaskTableMasks_1_HST(int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_1 = new DataTable();
            TableMasks_1.Clear();

            SqlString =
             " SELECT MatchMask, MatchingCateg, Count(*) as number "
             + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask, MatchingCateg "
             + " order by MatchingCateg ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                       /// sqlAdapt.CommandTimeout = 350;
                         sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_1);

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


        public void ReadTablePoolDataToGetMaskTableMasks_2(int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_2 = new DataTable();
            TableMasks_2.Clear();

            SqlString =
             " SELECT MatchMask,  Count(*) as number "
             + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask "
             + " ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_2);

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

        public void ReadTablePoolDataToGetMaskTableMasks_2_HST(int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_2 = new DataTable();
            TableMasks_2.Clear();

            SqlString =
             " SELECT MatchMask,  Count(*) as number "
             + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask "
             + " ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_2);

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
        // FILL MASK 1
        //

        public void ReadTablePoolDataToGetMaskTableMasks_1_E_Wallet(int InRMCycleNo, string W_Application)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_1 = new DataTable();
            TableMasks_1.Clear();

            SqlString =
             " SELECT MatchMask, MatchingCateg, Count(*) as number "
             + " FROM " + W_Application + ".[dbo]." + W_Application + "_TPF_TXNS_MASTER"
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask, MatchingCateg "
             + " order by MatchingCateg ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_1);

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
     //   _E_Wallet(int InRMCycleNo, string W_Application)
        public void ReadTablePoolDataToGetMaskTableMasks_1_HST_E_Wallet(int InRMCycleNo, string W_Application)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_1 = new DataTable();
            TableMasks_1.Clear();

            SqlString =
             " SELECT MatchMask, MatchingCateg, Count(*) as number "
                + " FROM " + W_Application + "_HST.[dbo]." + W_Application + "_TPF_TXNS_MASTER"
             //+ " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask, MatchingCateg "
             + " order by MatchingCateg ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        /// sqlAdapt.CommandTimeout = 350;
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_1);

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


        public void ReadTablePoolDataToGetMaskTableMasks_2_E_Wallet(int InRMCycleNo, string W_Application)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_2 = new DataTable();
            TableMasks_2.Clear();

            SqlString =
             " SELECT MatchMask,  Count(*) as number "
             + " FROM " + W_Application + ".[dbo]." + W_Application + "_TPF_TXNS_MASTER"
             //+ " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask "
             + " ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_2);

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

        public void ReadTablePoolDataToGetMaskTableMasks_2_HST_E_Wallet(int InRMCycleNo, string W_Application)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMasks_2 = new DataTable();
            TableMasks_2.Clear();

            SqlString =
             " SELECT MatchMask,  Count(*) as number "
                + " FROM " + W_Application + "_HST.[dbo]." + W_Application + "_TPF_TXNS_MASTER"
             //+ " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
             + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND Matched = 0 "
             + " group by MatchMask "
             + " ";


            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycleNo);

                        sqlAdapt.Fill(TableMasks_2);

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
        //
        //
        // Deposits
        // For ATM and Repl Cycle
        //
        public int TotNoBNA;
        public decimal TotValueBNA;
        public decimal TotCountedBNA;
        public decimal TotDiffBNA;

        public int TotNoCh;
        public decimal TotValueCh;
        public decimal TotCountedCh;
        public decimal TotDiffCh;

        public decimal TotNoEnv;
        public decimal TotValueEnv;
        public decimal TotCountedEnv;
        public decimal TotDiffEnv;

        public DataTable DepositsTranTable = new DataTable();

        public void ReadTableDepositsTxnsByAtmNoAndReplCycle_EGP(string InAtmNo, DateTime InDateFrom, DateTime InDateTo,
                                                          string InTransCurr, int InMode, int In_DB_Mode)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InTransCurr == "EGP")
            {
                InTransCurr = "818";
            }

            // InMode 1 all currencies
            // InMode 2 only one currency

            TotNoBNA = 0;
            TotValueBNA = 0;
            TotCountedBNA = 0;
            TotDiffBNA = 0;

            TotNoCh = 0;
            TotValueCh = 0;
            TotCountedCh = 0;
            TotDiffCh = 0;

            TotNoEnv = 0;
            TotValueEnv = 0;
            TotCountedEnv = 0;
            TotDiffEnv = 0;

            DepositsTranTable = new DataTable();
            DepositsTranTable.Clear();

            // DATA TABLE ROWS DEFINITION 
            DepositsTranTable.Columns.Add("UniqueId", typeof(int));
            DepositsTranTable.Columns.Add("TraceNo", typeof(int));
            DepositsTranTable.Columns.Add("Card", typeof(string));
            DepositsTranTable.Columns.Add("Account", typeof(string));

            DepositsTranTable.Columns.Add("TransDesc", typeof(string));
            DepositsTranTable.Columns.Add("DateTm", typeof(DateTime));

            DepositsTranTable.Columns.Add("CurrNm", typeof(string));
            DepositsTranTable.Columns.Add("Amount", typeof(decimal));

            DepositsTranTable.Columns.Add("Counted", typeof(decimal));

            DepositsTranTable.Columns.Add("Differ", typeof(decimal));

            //      DepositsTran.Columns.Add("SuspectNumber", typeof(int));

            DepositsTranTable.Columns.Add("OK", typeof(bool));
            DepositsTranTable.Columns.Add("Error", typeof(bool));

            DepositsTranTable.Columns.Add("Comments", typeof(string));

            if (In_DB_Mode == 1)
            {
                if (InMode == 1)
                {
                    SqlString = "Select * "
                   + "FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                   + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo  AND Origin = 'Our Atms' "
                   + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)";
                }
                if (InMode == 2)
                {
                    SqlString = "Select * "
                   + "FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                   + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo AND Origin = 'Our Atms' AND TransCurr = @TransCurr "
                   + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)";
                }

            }
            if (In_DB_Mode == 2)
            {
                if (InMode == 1)
                {
                    SqlString =
                     " WITH MergedTbl AS "
                   + " ( "
                   + " SELECT *  "
                   + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                   + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo AND Origin = 'Our Atms' "
                   + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)"
                   + " UNION ALL  "
                   + " SELECT * "
                   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                  + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo AND Origin = 'Our Atms' "
                   + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)"
                   + " ) "
                   + " SELECT * FROM MergedTbl "
                   + "  ";

                }
                if (InMode == 2)
                {
                    SqlString =
                       " WITH MergedTbl AS "
                     + " ( "
                     + " SELECT *  "
                     + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                     + " WHERE TerminalId = @AtmNo AND Origin = 'Our Atms' "
                     + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                     + " AND TransCurr = @TransCurr "
                     + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)"
                     + " UNION ALL  "
                     + " SELECT * "
                     + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                     + " WHERE TerminalId = @AtmNo AND Origin = 'Our Atms' "
                     + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                     + "AND TransCurr = @TransCurr "
                     + " AND (TransType = 23 OR TransType = 24 OR TransType = 25)"
                     + " ) "
                     + " SELECT * FROM MergedTbl "
                     + "  ";
                }
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);
                       // cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@TransCurr", InTransCurr);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowGrid = DepositsTranTable.NewRow();

                            RowGrid["UniqueId"] = UniqueRecordId;
                            RowGrid["TraceNo"] = AtmTraceNo;
                            RowGrid["Card"] = CardNumber;
                            RowGrid["Account"] = AccNumber;

                            RowGrid["TransDesc"] = TransDescr;
                            RowGrid["DateTm"] = TransDate;

                            RowGrid["CurrNm"] = TransCurr;

                            RowGrid["Amount"] = TransAmount;

                            RowGrid["Counted"] = DepCount;

                            RowGrid["Differ"] = TransAmount - DepCount;

                            if ((TransAmount - DepCount) == 0)
                            {
                                RowGrid["OK"] = true;
                            }
                            else
                            {
                                RowGrid["OK"] = false;
                                //    RowGrid["SuspectNumber"] = SpareField;        
                            }

                            if (MetaExceptionNo > 0)
                            {
                                RowGrid["Error"] = true;
                            }
                            else RowGrid["Error"] = false;

                            RowGrid["Comments"] = Comments;

                            if (TransType == 23) // "DEPOSIT BNA"
                            {
                                TotNoBNA = TotNoBNA + 1;
                                TotValueBNA = TotValueBNA + TransAmount;
                                TotCountedBNA = TotCountedBNA + DepCount;
                                TotDiffBNA = TotCountedBNA - TotValueBNA;
                            }
                            if (TransType == 24) //TransDesc == "DEP CHEQUES")
                            {
                                TotNoCh = TotNoCh + 1;
                                TotValueCh = TotValueCh + TransAmount;
                                TotCountedCh = TotCountedCh + DepCount;
                                TotDiffCh = TotCountedCh - TotValueCh;
                            }
                            if (TransType == 25) // Envelope Deposit                            
                            {
                                TotNoEnv = TotNoEnv + 1;
                                TotValueEnv = TotValueEnv + TransAmount;
                                TotCountedEnv = TotCountedEnv + DepCount;
                                TotDiffEnv = TotCountedEnv - TotValueEnv;
                            }

                            DepositsTranTable.Rows.Add(RowGrid);
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


        public void ReadTableManualTxnsTxns(string WOperator)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

            ManualTXNSTable = new DataTable();
            ManualTXNSTable.Clear();

            // DATA TABLE ROWS DEFINITION 
            ManualTXNSTable.Columns.Add("SeqNo", typeof(int));

            ManualTXNSTable.Columns.Add("Cycle", typeof(int));

            ManualTXNSTable.Columns.Add("Card", typeof(string));
            ManualTXNSTable.Columns.Add("Account", typeof(string));

            ManualTXNSTable.Columns.Add("TransDesc", typeof(string));

            ManualTXNSTable.Columns.Add("DateTm", typeof(DateTime));

            ManualTXNSTable.Columns.Add("Ccy", typeof(string));
            ManualTXNSTable.Columns.Add("Amount", typeof(decimal));

            ManualTXNSTable.Columns.Add("TraceNo", typeof(int));
            ManualTXNSTable.Columns.Add("RRN", typeof(string));

            ManualTXNSTable.Columns.Add("OpenDispute", typeof(bool));

            ManualTXNSTable.Columns.Add("Settled", typeof(bool));

            SqlString = "Select * "
           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
           + " WHERE OriginalRecordId = 999999 "
           + " Order By SeqNo Desc " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        //cmd.Parameters.AddWithValue("@TransCurr", InTransCurr);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowGrid = ManualTXNSTable.NewRow();

                            //ManualTXNSTable.Columns.Add("SeqNo", typeof(int));

                            //ManualTXNSTable.Columns.Add("Card", typeof(string));
                            //ManualTXNSTable.Columns.Add("Account", typeof(string));

                            //ManualTXNSTable.Columns.Add("TransDesc", typeof(string));

                            //ManualTXNSTable.Columns.Add("DateTm", typeof(DateTime));

                            //ManualTXNSTable.Columns.Add("CurrNm", typeof(string));
                            //ManualTXNSTable.Columns.Add("Amount", typeof(decimal));

                            //ManualTXNSTable.Columns.Add("TraceNo", typeof(int));
                            //ManualTXNSTable.Columns.Add("RRN", typeof(string));
                            RowGrid["SeqNo"] = SeqNo;
                            RowGrid["Cycle"] = LoadedAtRMCycle;
                            
                            RowGrid["Card"] = CardNumber;
                            RowGrid["Account"] = AccNumber; 
                            RowGrid["TransDesc"] = TransDescr;

                            RowGrid["DateTm"] = TransDate;

                            RowGrid["Ccy"] = TransCurr;

                            RowGrid["Amount"] = TransAmount;

                            RowGrid["TraceNo"] = TraceNoWithNoEndZero;

                            RowGrid["RRN"] = RRNumber;

                            // Check if already exist
                            Dt.ReadDisputeTranByUniqueRecordId(UniqueRecordId);
                            if (Dt.RecordFound == true)
                            {
                                RowGrid["OpenDispute"] = true;
                            }
                            else
                            {
                                RowGrid["OpenDispute"] = false;
                            }

                            RowGrid["Settled"] = SettledRecord;

                            ManualTXNSTable.Rows.Add(RowGrid);
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
        // Delete Manual
        public void DeleteManualBySeqNo(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
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
        // Deposits
        // For ATM and Repl Cycle
        //

        public DataTable TableSuspectAndPresenter = new DataTable();

        public void ReadTableOfErrorsAtATM_Suspect_Presenter_ByReplCycle(string InAtmNo, int InSesNo, int In_DB_Mode)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotNoBNA = 0;
            TotValueBNA = 0;
            TotCountedBNA = 0;
            TotDiffBNA = 0;

            TotNoCh = 0;
            TotValueCh = 0;
            TotCountedCh = 0;
            TotDiffCh = 0;

            TotNoEnv = 0;
            TotValueEnv = 0;
            TotCountedEnv = 0;
            TotDiffEnv = 0;

            TableSuspectAndPresenter = new DataTable();
            TableSuspectAndPresenter.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableSuspectAndPresenter.Columns.Add("UniqueId", typeof(int));
            TableSuspectAndPresenter.Columns.Add("TraceNo", typeof(int));
            TableSuspectAndPresenter.Columns.Add("Card", typeof(string));
            TableSuspectAndPresenter.Columns.Add("Account", typeof(string));

            TableSuspectAndPresenter.Columns.Add("TransDesc", typeof(string));
            TableSuspectAndPresenter.Columns.Add("DateTm", typeof(DateTime));

            TableSuspectAndPresenter.Columns.Add("CurrNm", typeof(string));
            TableSuspectAndPresenter.Columns.Add("Amount", typeof(decimal));

            TableSuspectAndPresenter.Columns.Add("Counted", typeof(decimal));

            TableSuspectAndPresenter.Columns.Add("Differ", typeof(decimal));

            TableSuspectAndPresenter.Columns.Add("SuspectNumber", typeof(int));

            TableSuspectAndPresenter.Columns.Add("Matched", typeof(bool));
            TableSuspectAndPresenter.Columns.Add("Error", typeof(bool));

            TableSuspectAndPresenter.Columns.Add("Comments", typeof(string));
            string SQLString = "";
            if (In_DB_Mode == 1)
            {
                SQLString = "Select * "
                + "FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo "
                + " AND (MetaExceptionId = 55 OR MetaExceptionId = 225)";
            }

            if (In_DB_Mode == 2)
            {
                SQLString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo "
                + " AND (MetaExceptionId = 55 OR MetaExceptionId = 225)"
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @AtmNo AND ReplCycleNo = @SesNo "
                + " AND (MetaExceptionId = 55 OR MetaExceptionId = 225)"
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowGrid = TableSuspectAndPresenter.NewRow();

                            RowGrid["UniqueId"] = UniqueRecordId;
                            RowGrid["TraceNo"] = AtmTraceNo;
                            RowGrid["Card"] = CardNumber;
                            RowGrid["Account"] = AccNumber;

                            RowGrid["TransDesc"] = TransDescr;
                            RowGrid["DateTm"] = TransDate;

                            RowGrid["CurrNm"] = TransCurr;

                            RowGrid["Amount"] = TransAmount;

                            RowGrid["Counted"] = DepCount;

                            RowGrid["Differ"] = TransAmount - DepCount;

                            if ((TransAmount - DepCount) == 0)
                            {
                                RowGrid["Matched"] = true;
                            }
                            else
                            {
                                RowGrid["Matched"] = false;
                                RowGrid["SuspectNumber"] = SpareField;
                            }

                            if (MetaExceptionNo > 0)
                            {
                                RowGrid["Error"] = true;
                            }
                            else RowGrid["Error"] = false;

                            RowGrid["Comments"] = Comments;

                            if (TransType == 23) // "DEPOSIT BNA"
                            {
                                TotNoBNA = TotNoBNA + 1;
                                TotValueBNA = TotValueBNA + TransAmount;
                                TotCountedBNA = TotCountedBNA + DepCount;
                                TotDiffBNA = TotCountedBNA - TotValueBNA;
                            }
                            if (TransType == 24) //TransDesc == "DEP CHEQUES")
                            {
                                TotNoCh = TotNoCh + 1;
                                TotValueCh = TotValueCh + TransAmount;
                                TotCountedCh = TotCountedCh + DepCount;
                                TotDiffCh = TotCountedCh - TotValueCh;
                            }
                            if (TransType == 25) // Envelope Deposit                            
                            {
                                TotNoEnv = TotNoEnv + 1;
                                TotValueEnv = TotValueEnv + TransAmount;
                                TotCountedEnv = TotCountedEnv + DepCount;
                                TotDiffEnv = TotCountedEnv - TotValueEnv;
                            }

                            TableSuspectAndPresenter.Rows.Add(RowGrid);
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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Unique Number 
        //
        public void ReadInPoolTransSpecificUniqueRecordId(int InUniqueRecordId, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE UniqueRecordId = @UniqueRecordId";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE UniqueRecordId = @UniqueRecordId"
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE UniqueRecordId = @UniqueRecordId"
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);
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
        // READ SPECIFIC TRANSACTION to see if in Matched Transactions
        //
        public void ReadInPoolTransSpecificToSeeIfInMatchedAndDeleteItFromPrimary
                   (string InTerminalId, int InTraceNoWithNoEndZero, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int WMinutes_Date = 0;
            int WSeqNo = 0;

            SqlString = " SELECT Minutes_Date "
             + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
             + " WHERE SeqNo = @SeqNo"
             + "  ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WMinutes_Date = (int)rdr["Minutes_Date"];
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

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // SELECT FROM MATCHED ? It will take too long! 
            SqlString = " SELECT SeqNo "
              + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId"
              + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero"
              + " AND Minutes_Date = @Minutes_Date"
              + " AND Origin = 'Our Atms' ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@Minutes_Date", WMinutes_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WSeqNo = (int)rdr["SeqNo"];
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

            if (RecordFound == true)
            {
                // Must delete the record 

                using (SqlConnection conn =
                   new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                                + " WHERE SeqNo = @SeqNo ", conn))
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
        }

        //
        // READ SPECIFIC TRANSACTION to see if in Matched Transactions
        //
        public int ReadInPoolTransSpecificToSeeIfDepositForReversals
                   (string InMatchingCateg, string InTerminalId, int InTraceNoWithNoEndZero, decimal InTransAmount, DateTime InTransDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            SeqNo = 0;

            SqlString = " SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE MatchingCateg = @MatchingCateg"
              + " AND TerminalId = @TerminalId"
              + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero"
              + " AND TransAmount = @TransAmount"
              + " AND TransDate = @TransDate"
              + " AND Origin = 'Our Atms'  ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return SeqNo;

        }

        //
        // READ SPECIFIC TRANSACTION to see if in Matched Transactions
        //
        public DateTime ReadInPoolTransSpecificToSeeIfDepositForReversalsLastMatchedDate
                   (string InMatchingCateg, string InTerminalId, DateTime InTransDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            SeqNo = 0;

            DateTime ReturnDate = NullPastDate;

            //SqlString = " SELECT * "
            //  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            //  + " WHERE MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId"
            //  + " "
            //  //+ " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero"
            //  //+ " AND TransAmount = @TransAmount"
            //  + " AND Net_TransDate <= @TransDate AND Origin = 'Our Atms' "
            //  + "  " +
            //  " Order By TransctionDate Desc ";

            SqlString =
                  " WITH MergedTbl AS "
                + " ( "
                + " SELECT TOP (1) * "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId "
                + " AND TransDate <= @TransDate AND Origin = 'Our Atms' and IsMatchingDone = 1  "
                + " Order by TransDate Desc "
                + " UNION ALL  "
                + " SELECT TOP (1) * "
                + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId "
                + " AND TransDate <= @TransDate AND Origin = 'Our Atms' and IsMatchingDone = 1 "
                 + " Order by TransDate Desc "
                + " ) "
                + " SELECT TOP (1) * FROM MergedTbl "
                 + " Order by TransDate Desc "
                + "  ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        //cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        //cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.CommandTimeout = 700;

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            ReturnDate = TransDate;

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

            return ReturnDate;

        }
        //
        // FIND TOTAL PRESENTER PER Cycle/RM CATEGORY  
        //

        public int PresenterSettled;
        public decimal PresenterSettledAmt;
        public int Presenter_Not_Settled;
        public decimal Presenter_Not_SettledAmt;
        public decimal TotalPresenter;

        public void ReadPoolAndFindTotals_Presenter_PerRMCategory(int InMatchingAtRMCycle, string InRMCateg, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 All Not Settled Presenter
            // InMode = 2 Settled and not Settled for a cycle 

            PresenterSettled = 0;
            Presenter_Not_Settled = 0;
            PresenterSettledAmt = 0;
            Presenter_Not_SettledAmt = 0;
            TotalPresenter = 0;

            if (InMode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE RMCateg = @RMCateg AND MetaExceptionId = 55 AND SettledRecord = 0 ";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE MatchingAtRMCycle = @MatchingAtRMCycle AND RMCateg = @RMCateg AND MetaExceptionId = 55 ";
            }



            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (SettledRecord == true)
                            {
                                PresenterSettled = PresenterSettled + 1;
                                PresenterSettledAmt = PresenterSettledAmt + TransAmount;

                            }
                            else
                            {
                                Presenter_Not_Settled = Presenter_Not_Settled + 1;
                                Presenter_Not_SettledAmt = Presenter_Not_SettledAmt + +TransAmount;
                            }

                            TotalPresenter = TotalPresenter + TransAmount;
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
        // Correct data Rcs 
        public void ReadPoolAndFindTotals_Presenter_Unmatched(string InOperator, int InMatchingAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();

            SqlString =
           " SELECT * "
           + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
           + " WHERE Origin = 'Our Atms' "
           + " AND MetaExceptionId = 55 "
           + " AND Matched = 0  AND MatchingAtRMCycle =@MatchingAtRMCycle "
           //  + " Group by MatchingAtRMCycle,  RMCateg "
           + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);

                        sqlAdapt.Fill(RMDataTableRight);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();

            int I = 0;

            while (I <= (RMDataTableRight.Rows.Count - 1))
            {


                int WMatchingAtRMCycle = (int)RMDataTableRight.Rows[I]["MatchingAtRMCycle"];
                string WRMCateg = (string)RMDataTableRight.Rows[I]["RMCateg"];

                decimal WTransAmount = (decimal)RMDataTableRight.Rows[I]["TransAmount"];

                string WTerminalId = (string)RMDataTableRight.Rows[I]["TerminalId"];

                //int WPresUnmatched = (int)RMDataTableRight.Rows[I]["PresUnmatched"];

                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(InOperator, WRMCateg, WMatchingAtRMCycle);

                Rcs.NumberOfUnMatchedRecs = Rcs.NumberOfUnMatchedRecs - 1;
                if (Rcs.NumberOfUnMatchedRecs > 0) Rcs.Difference = true;
                else Rcs.Difference = false;
                Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions - 1;

                Rcs.NotMatchedTransAmt = Rcs.NotMatchedTransAmt - WTransAmount;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess(WRMCateg, WMatchingAtRMCycle);


                Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                               (InOperator, WRMCateg, WMatchingAtRMCycle, WTerminalId);
                if (Ratms.RecordFound)
                {
                    Ratms.UnMatchedAmt = Ratms.UnMatchedAmt - WTransAmount;
                    Ratms.NumberOfUnMatchedRecs = Ratms.NumberOfUnMatchedRecs - 1; // Subtruct one 

                    Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(WTerminalId, WRMCateg, WMatchingAtRMCycle);
                }




                I++; // Read Next entry of the table 

            }

        }
        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Selection Criteria 
        //

        public void ReadInPoolTransSpecificBySelectionCriteria(string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl ORDER By TransDate  "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Minutes_Date = (int)rdr["Minutes_Date"];


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
        int TotalCounted;
        public int ReadInPoolTransBySelectionCriteria_Find_Total(string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalCounted = 0;

            if (In_DB_Mode == 2)
            {
                SqlString =
           " WITH MergedTbl AS "
+ " ("
+ "SELECT seqNo "
+ "FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
+ InSelectionCriteria
+ " UNION ALL "
+ "  SELECT seqNo "
+ "  FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
+ InSelectionCriteria
+ ")"
+ "  SELECT Count(*) As TotalCounted FROM MergedTbl ";

            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalCounted = (int)rdr["TotalCounted"];

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

            return TotalCounted;
        }


        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Date
        //
        public void ReadInPoolTransSpecificNearAtmJournal(string InSelectionCriteria, DateTime InTransDate, string InOrderBy, int In_DB_Mode)
        {
            //SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId AND Origin = 'Our Atms'
            //                    + "' AND NotInJournal = 0 AND TransDate < @TransDate  ORDER By TransDate Desc ";
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
              + InOrderBy;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InOrderBy
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 
                        cmd.CommandTimeout = 60;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

                            if (Counter == 2) break;
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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Date
        //
        public void ReadInPoolTransSpecificNearAtmJournal_HST(string InSelectionCriteria, DateTime InTransDate, string InOrderBy, int In_DB_Mode)
        {
            //SelectionCriteria = " WHERE TerminalId ='" + Mpa.TerminalId AND Origin = 'Our Atms'
            //                    + "' AND NotInJournal = 0 AND TransDate < @TransDate  ORDER By TransDate Desc ";
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
              + InOrderBy;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InOrderBy
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 
                        cmd.CommandTimeout = 60;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

                            if (Counter == 2) break;
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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on MINMAX During Matching - SPECIFIC DATE and Time
        //
        public void ReadInPoolTransSpecificDuringMatching(string InMatchingCateg, string InTerminalId, DateTime InTransDate, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND TransDate = @TransDate "
              + " AND IsMatchingDone = 0 "
              + " ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on MINMAX During Matching - SPECIFIC DATE and Time
        //
        //public void ReadInPoolTransSpecificDuringMatching_NO_ATM(string InMatchingCateg, DateTime InTransDate, int In_DB_Mode)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";
        //    int Counter = 0;
        //    SqlString = "SELECT *"
        //      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //      + " Where MatchingCateg = @MatchingCateg AND TransDate = @TransDate AND IsMatchingDone = 0 "
        //      + " ";
        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

        //                cmd.Parameters.AddWithValue("@TransDate", InTransDate);

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    ReadFieldsInTable(rdr);

        //                    Counter = Counter + 1;

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            CatchDetails(ex);
        //        }
        //}  // Find Max dateTime for Mpa with No ATM But Category

        public DateTime ReadAndFindMaxDateTimeForMpa_NonATM(string InFile, string InMatchingCateg, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime MaxDt = NullPastDate;
            if (In_DB_Mode == 1)
            {
                SqlString =
           " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
           + " FROM " + InFile
           + " WHERE  "
                       + "    ([MatchingCateg] = @MatchingCateg)  "
                       + " AND ([IsMatchingDone] = 0) "
                       + " AND (ResponseCode = '0') ";
            }


            using (SqlConnection conn =
                       new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                        if (MaxDt != NullPastDate) RecordFound = true;
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return MaxDt;

          
        }

        public DateTime ReadAndFindMaxDateTimeForMpa_NonATM_2(string InFile, string InMatchingCateg, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime MaxDt = NullPastDate;
            if (In_DB_Mode == 1)
            {
                SqlString =
           " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
           + " FROM " + InFile
           + " WHERE  "
                       + "    ([MatchingCateg] = @MatchingCateg)  "
                       + " AND ([IsMatchingDone] = 0) AND [TXNDEST] IN ('0','','123','124' ,'1', '4') "
                       + " AND (ResponseCode = '0') ";
            }


            using (SqlConnection conn =
                       new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                        if (MaxDt != NullPastDate) RecordFound = true;
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return MaxDt;


        }

        public void ReadAndFindRRNUMBER_NO_ATM(string InTable, string InMatchingCateg, DateTime InTransDate, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (In_DB_Mode == 1)
            {
                SqlString =
        " SELECT * "
        + " FROM " + InTable
        + " WHERE MatchingCateg = @MatchingCateg AND IsMatchingDone = 0 AND ResponseCode = '0' AND TransDate = @TransDate "
                  + " ";
            }


            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);


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


        // Find Max dateTime for Mpa with No ATM But Category

        public void ReadAndFindRecordLessThanDate_NonATM(string InFile, string InMatchingCateg
                                                     , DateTime InLimitDate, int In_DB_Mode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (In_DB_Mode == 1)
            {
                SqlString =
            " SELECT  TOP 1 * "
            + " FROM " + InFile
                        + " WHERE  ([MatchingCateg] = @MatchingCateg)  "
                        + " AND ([IsMatchingDone] = 0)  "
                        + " AND (ResponseCode = '0')"
                        + " AND (TransDate <= @TransDate)"
                        + " ORDER BY TransDate DESC "
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
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InLimitDate);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            break;
                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            // return MaxDt;
        }


        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on MINMAX During Matching - SPECIFIC Parameters
        //
        public DateTime ReadInPoolTransSpecificDuringMatching_2(string InMatchingCateg, string InTerminalId, DateTime InTransDate
                                                     , int InTraceNoWithNoEndZero, decimal InTransAmount
                                                     , string InCardNumber, string InAccNumber, int In_DB_Mode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where "
              + " IsMatchingDone = 0 "
              + " AND MatchingCateg = @MatchingCateg "
              + " AND Net_TransDate = @TransDate "
              + " AND TerminalId = @TerminalId "
              + " AND CardNumber = @CardNumber "
              + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
              + " AND TransAmount = @TransAmount "
                           + " ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //    cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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

            return TransDate;
        }


        //
        // READ SPECIFIC TRANSACTION FROM IN POOL Less than Given and Find SET_DATE
        //
        public void ReadInPoolTransLessThanGiven_To_FIND_SET_DATE(string InMatchingCateg, DateTime InTransDate, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT TOP 1 SET_DATE "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where "
              + " IsMatchingDone = 1 AND MATCHED = 1"
              + " AND MatchingCateg = @MatchingCateg "
              + " AND TransDate < @TransDate "
                           + " ORDER BY TransDate DESC ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        //    cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //ReadFieldsInTable(rdr);

                            SET_DATE = (DateTime)rdr["SET_DATE"];


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
        // READ SPECIFIC TRANSACTION FROM IN POOL 
        //
        public DateTime ReadInPoolTransSpecificDuringMatching_3(string InMatchingCateg, string InTerminalId, DateTime InTransDate
                                                     , int InTraceNoWithNoEndZero, decimal InTransAmount
                                                     , string InCardNumber, string InAccNumber
                                                     , int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                           + " AND TransAmount = @TransAmount "
                           + " ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                           + " AND TransAmount = @TransAmount "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                           + " AND TransAmount = @TransAmount "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //    cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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

            return TransDate;
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL 
        //
        public DateTime ReadInPoolTransSpecificDuringMatching_4(string InMatchingCateg, string InTerminalId, DateTime InTransDate
                                                     , string InRRNumber, decimal InTransAmount
                                                     , string InCardNumber, string InAccNumber
                                                     , int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmount = @TransAmount "
                           + " AND CardNumber = @CardNumber "
                           //  + " AND AccNumber = @AccNumber "
                           + " ";
            }
            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmount = @TransAmount "
                           + " AND CardNumber = @CardNumber "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId AND IsMatchingDone = 1 "
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmount = @TransAmount "
                           + " AND CardNumber = @CardNumber "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //    cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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

            return TransDate;
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL UNMATCHED
        //
        public void ReadInPoolTransSpecificDuringMatching_5(string InMatchingCateg, string InTerminalId, DateTime InTransDate
                                                     , int InTraceNoWithNoEndZero, decimal InTransAmount
                                                     , string InCardNumber, string InAccNumber
                                                     , int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where MatchingCateg = @MatchingCateg AND TerminalId = @TerminalId "
                 //+ "  AND IsMatchingDone = 1 AND MATCHED = 0"
                 + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                           + " AND TransAmount = @TransAmount  "
                           + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //    cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on MINMAX During Matching - SPECIFIC Parameters
        //
        public DateTime ReadInPoolTransSpecificDuringMatching_2_NO_ATM(string InMatchingCateg
                                                    , string InCardNumber
                                                    , decimal InTransAmount
                                                    , string InRRNumber
                                                    , DateTime InTransDate
                                                        , int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Counter = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " Where MatchingCateg = @MatchingCateg AND IsMatchingDone = 0 "
               + " AND CardNumber = @CardNumber"
                + " AND TransAmount = @TransAmount"
                 + " AND Net_TransDate = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " ";
            }
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate.Date);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            Counter = Counter + 1;

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

            return TransDate;
        }

        //
        // READ TRANSACTION FROM IN POOL GREATER THAN CRITERIA
        //

        public decimal ReadInPoolTransTotalGreaterThan(string InTerminalId, string InMatchingCateg, int InSeqNo,
            DateTime InTransDate, int InReplCycleNo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal DispensedTotal = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                DispensedTotal = DispensedTotal + TransAmount;
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
            return DispensedTotal;
        }

        //
        // READ TRANSACTIONS BY ATM/CATEGORY FROM IN POOL LESS OR EQUAL TO CRITERIA 
        //

        public decimal ReadInPoolTransTotal_Less_Than(string InTerminalId, string InMatchingCateg, int InSeqNo
            , DateTime InTransDate, int InReplCycleNo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal DispensedTotal = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND SeqNo < @SeqNo AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND SeqNo < @SeqNo AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND SeqNo < @SeqNo AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                DispensedTotal = DispensedTotal + TransAmount;
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
            return DispensedTotal;
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on ATM, Trace No and Date
        //

        public int ReadInPoolTransFindUniqueNumber(string InTerminalId, int InTraceNoWithNoEndZero
            , DateTime InTransDateFrom, DateTime InTransDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            UniqueRecordId = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                    + " AND (TransDate >= @TransDateFrom AND TransDate <= @TransDateTo) ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE TerminalId = @TerminalId AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                    + " AND (TransDate >= @TransDateFrom AND TransDate <= @TransDateTo) "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId = @TerminalId AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                    + " AND (TransDate >= @TransDateFrom AND TransDate <= @TransDateTo) "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@TransDateFrom", InTransDateFrom);
                        cmd.Parameters.AddWithValue("@TransDateTo", InTransDateTo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return UniqueRecordId;
        }

        //
        // READ  TRANSACTION FROM IN POOL based on criteria to fill Table
        // TXNs After Cut off till replenishement 
        //

        public decimal ReadInPoolTransTotalGreaterThanFillTable(string InTerminalId, string InMatchingCateg,
                            int InSeqNo, DateTime InTransDate, int InReplCycleNo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            GlAdjustmentTxns = new DataTable();
            GlAdjustmentTxns.Clear();

            decimal DispensedTotal = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND TransDate > @TransDate AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        sqlAdapt.Fill(GlAdjustmentTxns);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            //InsertWReportAtmRepl(InSignedId);
            int I = 0;

            while (I <= (GlAdjustmentTxns.Rows.Count - 1))
            {

                RecordFound = true;

                UniqueRecordId = (int)GlAdjustmentTxns.Rows[I]["UniqueRecordId"];

                //TerminalId = (string)MatchingMasterDataTableATMs.Rows[I]["MatchMask"];

                //Partial Updating ... so read to get all fields 
                string WSelectionCriteria = " WHERE  UniqueRecordId =" + UniqueRecordId;
                ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 1);

                if (TransType == 11)
                {
                    DispensedTotal = DispensedTotal + TransAmount;
                }

                I++; // Read Next entry of the table 

            }

            // DELETE ANY RELATED 
            if (GlAdjustmentTxns.Rows.Count > 0)
            {
                DeleteAdjustementEntries(InTerminalId, InReplCycleNo, 2);
                // INSERT IN TABLE
                InsertReport(TerminalId, ReplCycleNo, GlAdjustmentTxns);

            }

            return DispensedTotal;
        }

        //
        // update SM 
        //

        public int ReadInPoolTransAndGetMaxDatesByATM(DateTime InLOW_LimitDate)
        {
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDates_By_ATM = new DataTable();
            MaxDates_By_ATM.Clear();
            //ISNULL(MAX (TransDate), '1900-01-01') As Max_Date
            //  SqlString = "SELECT Terminalid, MAX (TransDate) AS Max_Date "
            SqlString = "SELECT Terminalid, ISNULL(MAX (TransDate), '1900-01-01') As Max_Date "
            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
          + " WHERE TXNSRC = 1 AND Origin = 'Our Atms' AND Net_TransDate > @Net_TransDate"
          + " Group By TerminalId";

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //Create a datatable that will be filled with the data retrieved from the command
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Net_TransDate", InLOW_LimitDate);
                        sqlAdapt.SelectCommand.CommandTimeout = 500;
                        sqlAdapt.Fill(MaxDates_By_ATM);
                        // RecordFound = true; 
                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    CatchDetails(ex);
                }


            //InsertWReportAtmRepl(InSignedId);
            int I = 0;
            string WAtmNo;
            DateTime Max_Date;

            while (I <= (MaxDates_By_ATM.Rows.Count - 1))
            {

                WAtmNo = (string)MaxDates_By_ATM.Rows[I]["Terminalid"];
                Max_Date = (DateTime)MaxDates_By_ATM.Rows[I]["Max_Date"];
                // UPDATE FOR Process Mode = -1 
                Ta.UpdateTracesWithMaxDate(WAtmNo, Max_Date);

                I++; // Read Next entry of the table 

            }

            return MaxDates_By_ATM.Rows.Count;
        }

        //
        // READ  TRANSACTION FROM IN POOL based on criteria to fill Table
        // TXNs from Replenishement to Cut Off 
        //

        public decimal ReadInPoolTransTotal_LESS_ThanFillTable(string InTerminalId, string InMatchingCateg,
                            int InSeqNo, DateTime InTransDate, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            GlAdjustmentTxns = new DataTable();
            GlAdjustmentTxns.Clear();

            decimal DispensedTotal = 0;

            SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId AND MatchingCateg = @MatchingCateg "
                    + " AND SeqNo <= @SeqNo AND ReplCycleNo = @ReplCycleNo "
                    + " AND (NotInJournal = 0  "
                    + " OR ( NotInJournal = 1 AND ReplCycleNo > 0  AND ActionType = '04')) "
                    + " ORDER BY TransDate ";

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        sqlAdapt.Fill(GlAdjustmentTxns);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            //InsertWReportAtmRepl(InSignedId);
            int I = 0;

            while (I <= (GlAdjustmentTxns.Rows.Count - 1))
            {

                RecordFound = true;

                UniqueRecordId = (int)GlAdjustmentTxns.Rows[I]["UniqueRecordId"];

                TransType = (int)GlAdjustmentTxns.Rows[I]["TransType"];
                TransAmount = (decimal)GlAdjustmentTxns.Rows[I]["TransAmount"];

                //TerminalId = (string)MatchingMasterDataTableATMs.Rows[I]["MatchMask"];

                //Partial Updating ... so read to get all fields 
                //string WSelectionCriteria = " WHERE  UniqueRecordId =" + UniqueRecordId;
                //ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria);

                if (TransType == 11)
                {
                    DispensedTotal = DispensedTotal + TransAmount;
                }

                I++; // Read Next entry of the table 

            }

            // DELETE ANY RELATED 
            if (GlAdjustmentTxns.Rows.Count > 0)
            {
                DeleteAdjustementEntries(InTerminalId, InReplCycleNo, 2);
                // INSERT IN TABLE
                InsertReport(TerminalId, ReplCycleNo, GlAdjustmentTxns);
            }

            return DispensedTotal;
        }

        // Insert 
        public void InsertReport(string InTerminalId, int InReplCycleNo, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[Working_Master_Pool_Adjustments]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }

        //
        // DELETE Adjustment Entries
        //
        public void DeleteAdjustementEntries(string InTerminalId, int InReplCycleNo, int In_DB_Mode)
        {
            ErrorFound = false;
            ErrorOutput = "";
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Working_Master_Pool_Adjustments] "
                                + " WHERE TerminalId = @TerminalId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

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
        //
        // DELETE Duplicates 
        //
        public int Count;
        public void DeleteDuplicates_NCR_Vision(int InLoadedAtRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";
            Count = 0;

            string CMD_Details = " WITH cte AS( "
                      + " SELECT "
                      + "  SeqNo, "
                      + "  TerminalId, "
                      + "  TraceNoWithNoEndZero, "
                      + "   TransAmount, "
                      + "   Minutes_Date, "
                      + "   CardNumber, "
                      + "   ROW_NUMBER() OVER( "
                      + "       PARTITION BY "
                      + "       TerminalId, "
                      + "       TraceNoWithNoEndZero, "
                      + "       TransAmount, "
                      + "       Minutes_Date, "
                      + "       CardNumber "
                      + "   ORDER BY "
                      + "     TerminalId, "
                      + "     TraceNoWithNoEndZero, "
                      + "     TransAmount, "
                      + "     Minutes_Date, "
                      + "     CardNumber "
                      + "    ) row_num "
                      + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " where IsMatchingDone = 0 AND Origin = 'Our Atms' "
                      + " ) "
                      + " DELETE FROM cte "
                      + " WHERE row_num > 1 ";
            //string SqlCommand = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //                + " WHERE MatchingCateg = @MatchingCateg "; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(CMD_Details, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        //rows number of record got updated
                        cmd.CommandTimeout = 300;
                        Count = cmd.ExecuteNonQuery();
                        //  Count = cmd.ExecuteNonQueryAsync(); 
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
        // Read By Selection Criteria UNIQUE 
        // 
        //
        public void ReadMatchingTxnsMasterPoolBySelectionCriteria(string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " "
                      + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

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
                        cmd.CommandTimeout = 75;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            break;
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

        public void ReadMatchingTxnsMasterPoolBySelectionCriteria_HST(string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " "
                      + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

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
                        cmd.CommandTimeout = 75;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            break;
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

        //public void ReadMatchingTxnsMasterPoolBySelectionCriteria_2(string InSelectionCriteria, int In_DB_Mode
        //               )
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    TotalSelected = 0;
        //    if (In_DB_Mode == 1)
        //    {
        //        SqlString = "SELECT * "
        //              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //              + " "
        //              + InSelectionCriteria;
        //    }

        //    if (In_DB_Mode == 2)
        //    {
        //        SqlString =
        //           " WITH MergedTbl AS "
        //         + " ( "
        //         + " SELECT *  "
        //         + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //      + InSelectionCriteria
        //        + " UNION ALL  "
        //         + " SELECT * "
        //         + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //        + InSelectionCriteria
        //         + " ) "
        //         + " SELECT * FROM MergedTbl "
        //         + "  ";
        //    }

        //    //using (SqlConnection conn =
        //    //              new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    TotalSelected = TotalSelected + 1;

        //                    ReadFieldsInTable(rdr);

        //                    break;
        //                }
        //                // Close Reader
        //                rdr.Close();
        //            }
        //            // Close conn
        //          //  conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //          //  conn.Close();

        //            CatchDetails(ex);
        //        }
        //}
        //
        // READ SPECIFIC ALL TRACES THAT HAVE SAME MASTERTraceNo and Fill table 
        //
        public DataTable InPoolMasterTraceNoDataTable = new DataTable();
        //
        public void ReadInPoolTransByMasterTraceNoDataTable(string InTerminalId, int InMasterTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            InPoolMasterTraceNoDataTable = new DataTable();
            InPoolMasterTraceNoDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            InPoolMasterTraceNoDataTable.Columns.Add("UniqueRecordId", typeof(int));
            InPoolMasterTraceNoDataTable.Columns.Add("AtmTraceNo", typeof(int));
            InPoolMasterTraceNoDataTable.Columns.Add("MasterTraceNo", typeof(int));

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " WHERE TerminalId = @TerminalId AND MasterTraceNo = @MasterTraceNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@MasterTraceNo", InMasterTraceNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            //
                            //Fill In Table
                            //
                            DataRow RowSelected = InPoolMasterTraceNoDataTable.NewRow();

                            RowSelected["UniqueRecordId"] = UniqueRecordId;
                            RowSelected["AtmTraceNo"] = AtmTraceNo;
                            RowSelected["MasterTraceNo"] = MasterTraceNo;
                            // ADD ROW
                            InPoolMasterTraceNoDataTable.Rows.Add(RowSelected);
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
        // READ first record of category and cycle 
        //
        public void ReadMatchingTxnsMasterPoolFirstRecordFound(string InSelectionCriteria
                                                                               , int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " "
                      //+ InSelectionCriteria + " AND CAST(TransDate AS Date) = @TransDate";
                      + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            break;

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
        // READ first record for a Particular ATM and Less Than A Trace Number to get the Replenishment cycle 
        //
        public int ReadMatchingTxnsMasterPoolFirstRecordLessThanGivenTraceNumber(string InTerminalId, int InTraceNoWithNoEndZero)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int WReplNo = 0;

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " Where TerminalId = @TerminalId AND TraceNoWithNoEndZero < @TraceNoWithNoEndZero "
                      + " ORDER by TraceNoWithNoEndZero DESC ";
            //+ InSelectionCriteria + " AND CAST(TransDate AS Date) = @TransDate";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (ReplCycleNo > 0)
                            {
                                WReplNo = ReplCycleNo;
                                break;
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
            return WReplNo;
        }

        //
        // Methods 
        // READ first record for a Particular ATM and Less Than A Trace Number to get the Replenishment cycle 
        //
        public int ReadMatchingTxnsMasterPoolFirstRecordLessThanGivenDateTime(string InTerminalId, DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int WReplNo = 0;

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " Where TerminalId = @TerminalId AND TransDate < @TransDate "
                      + " ORDER by TransDate DESC ";
            //+ InSelectionCriteria + " AND CAST(TransDate AS Date) = @TransDate";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            //if (ReplCycleNo > 0)
                            //{
                            WReplNo = ReplCycleNo;
                            break;
                            //}

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
            return WReplNo;
        }

        //
        // Methods 
        // READ Range ... DATES 
        // FILL UP A TABLE LEFT 
        //

        //bool DefaultAction;
        public decimal TotalAmountSelected;
        public int TotalOutstanding;
        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                          string InSortCriteria, DateTime InFromDt, DateTime InToDt, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            TotalSelected = 0;

            TotalAmountSelected = 0;

            TotalOutstanding = 0;

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();


            // DATA TABLE ROWS DEFINITION 

            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
                MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
                MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            }

            if (InMode == 3)
            {
                MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            }
            
            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
           
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
           
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UTRNNO", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));


            MatchingMasterDataTableATMs.Columns.Add("TransType", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Action", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
          
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Comments", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UnMatchedType", typeof(string));

            if (In_DB_Mode == 1)
            {
                if (InFromDt != NullPastDate)
                {
                    SqlString =
                        "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                        + " "
                         + InSortCriteria;
                }
                if (InFromDt == NullPastDate)
                {
                    SqlString = "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                         + " "
                         + InSortCriteria;
                }
            }

            if (In_DB_Mode == 2)
            {

                if (InFromDt != NullPastDate)
                {
                    SqlString =
                    " WITH TempTbl AS "
                    + " ( "
                    + " SELECT *  "
                    + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                    + " UNION ALL  "
                    + " SELECT * "
                    + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                    + " ) "
                    + " SELECT * FROM TempTbl "
                    + InSortCriteria
                    + "  ";

                }
                if (InFromDt == NullPastDate)
                {
                    SqlString =
                   " WITH TempTbl AS "
                   + " ( "
                   + " SELECT *  "
                   + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " UNION ALL  "
                   + " SELECT * "
                   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " ) "
                   + " SELECT * FROM TempTbl "
                     + InSortCriteria
                    + "  ";

                }
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }


                        // Read table 
                        cmd.CommandTimeout = 150;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            TotalSelected = TotalSelected + 1;

                            TotalAmountSelected = TotalAmountSelected + TransAmount;

                            if (ActionType == "00")
                            {
                                TotalOutstanding = TotalOutstanding + 1;
                            }

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            if (InMode == 1 || InMode == 2 || InMode == 5)
                            {
                                RowSelected["RecordId"] = UniqueRecordId;

                                if (ActionType == "00")
                                {
                                    RowSelected["Done"] = "NO";
                                }
                                else
                                {
                                    // Eg = to 1
                                    RowSelected["Done"] = "YES";
                                }

                                if (Matched == true)
                                {
                                    RowSelected["Status"] = "M";

                                }
                                else
                                {
                                    RowSelected["Status"] = "U";

                                }
                            }

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount;
                                RowSelected["RecordId"] = UniqueRecordId;
                            }
                            
                            //if (ActionType != "00")
                            //{
                            //    Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(UniqueRecordId);
                            //    if (Aoc.RecordFound == true)
                            //    {
                            //        RowSelected["Action"] = Aoc.ActionId; // find the first action for this unique
                            //    }
                            //    else
                            //    {
                            //        RowSelected["Action"] = ActionType; // this part will not happen 
                            //    }
                            //}
                            //else
                            //{
                            //    RowSelected["Action"] = ActionType; // equal to zero 
                            //}

                            RowSelected["Action"] = ActionType;

                            RowSelected["Terminal"] = TerminalId;

                            RowSelected["Type"] = "N/A";
                            if (TerminalType == "10")
                            {
                                RowSelected["Type"] = "ATM";
                            }
                            if (TerminalType == "20")
                            {
                                RowSelected["Type"] = "POS";
                            }
                            if (TerminalType == "30")
                            {
                                RowSelected["Type"] = "WEB";
                            }

                            RowSelected["MatchingCateg"] = MatchingCateg;

                            RowSelected["Trace"] = TraceNoWithNoEndZero;
                           
                            RowSelected["RRNumber"] = RRNumber;
                            RowSelected["UTRNNO"] = UTRNNO;

                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;

                            RowSelected["TransType"] = TransType;

                            
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["ReplCycleNo"] = ReplCycleNo;

                            RowSelected["UserId"] = InSignedId;

                            RowSelected["Comments"] = Comments;
                            RowSelected["UnMatchedType"] = UnMatchedType;

                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    //InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        public void ReadMatchingTxnsMasterPoolByRangeAndFillTableFrom271b_VIEW(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                        string InSortCriteria, DateTime InFromDt, DateTime InToDt, int In_DB_Mode, int InActionOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool ValidRecord = false; 

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            TotalSelected = 0;

            TotalAmountSelected = 0;

            TotalOutstanding = 0;

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();


            // DATA TABLE ROWS DEFINITION 

            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
                MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
                MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            }

            if (InMode == 3)
            {
                MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            }
            MatchingMasterDataTableATMs.Columns.Add("Action", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TransType", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Comments", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UnMatchedType", typeof(string));

            if (In_DB_Mode == 1)
            {
                if (InFromDt != NullPastDate)
                {
                    SqlString =
                        "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                        + " "
                         + InSortCriteria;
                }
                if (InFromDt == NullPastDate)
                {
                    SqlString = "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                         + " "
                         + InSortCriteria;
                }
            }

            if (In_DB_Mode == 2)
            {

                if (InFromDt != NullPastDate)
                {
                    SqlString =
                    " WITH TempTbl AS "
                    + " ( "
                    + " SELECT *  "
                    + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                    + " UNION ALL  "
                    + " SELECT * "
                    + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                    + " ) "
                    + " SELECT * FROM TempTbl "
                    + InSortCriteria
                    + "  ";

                }
                if (InFromDt == NullPastDate)
                {
                    SqlString =
                   " WITH TempTbl AS "
                   + " ( "
                   + " SELECT *  "
                   + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " UNION ALL  "
                   + " SELECT * "
                   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " ) "
                   + " SELECT * FROM TempTbl "
                     + InSortCriteria
                    + "  ";

                }
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }


                        // Read table 
                        cmd.CommandTimeout = 150;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (ActionType != "00")
                            {
                                if (InActionOrigin == 1) // Reconciliation 
                                {
                                    Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(UniqueRecordId, "Reconciliation");
                                    if (Aoc.RecordFound == true)
                                    {
                                        // If FOUND then it is VALID record
                                        ValidRecord = true; 
                                    }
                                    else
                                    {
                                        // NOT VALID
                                        ValidRecord = false;
                                    }
                                }
                                if (InActionOrigin == 2) // Replenishment 
                                {
                                    Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(UniqueRecordId, "Replenishment");
                                    if (Aoc.RecordFound == true)
                                    {
                                        // If FOUND then it is VALID record 
                                        ValidRecord = true;
                                    }
                                    else
                                    {
                                        // NOT VALID
                                        ValidRecord = false;
                                    }
                                }

                            }
                          // Deal Only with the valid records 
                            if (ValidRecord == true)
                            {
                                TotalSelected = TotalSelected + 1;

                                TotalAmountSelected = TotalAmountSelected + TransAmount;

                                if (ActionType == "00")
                                {
                                    TotalOutstanding = TotalOutstanding + 1;
                                }

                                // Fill Table 

                                DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                                if (InMode == 1 || InMode == 2 || InMode == 5)
                                {
                                    RowSelected["RecordId"] = UniqueRecordId;

                                    if (ActionType == "00")
                                    {
                                        RowSelected["Done"] = "NO";
                                    }
                                    else
                                    {
                                        // Eg = to 1
                                        RowSelected["Done"] = "YES";
                                    }

                                    if (Matched == true)
                                    {
                                        RowSelected["Status"] = "M";

                                    }
                                    else
                                    {
                                        RowSelected["Status"] = "U";

                                    }
                                }

                                if (InMode == 3)
                                {
                                    RowSelected["Select"] = false;
                                    RowSelected["DisputedAmnt"] = TransAmount;
                                    RowSelected["RecordId"] = UniqueRecordId;
                                }

                                if (ActionType != "00")
                                {
                                    if (InActionOrigin == 1) // Reconciliation 
                                    {
                                        Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(UniqueRecordId, "Reconciliation");
                                        if (Aoc.RecordFound == true)
                                        {
                                            RowSelected["Action"] = Aoc.ActionId; // find the first action for this unique
                                        }
                                        else
                                        {
                                            RowSelected["Action"] = ActionType; // this part will not happen 
                                        }
                                    }
                                    if (InActionOrigin == 2) // Replenishment 
                                    {
                                        Aoc.ReadActionsOccurancesByUniqueRecordIdFirstAction(UniqueRecordId, "Replenishment");
                                        if (Aoc.RecordFound == true)
                                        {
                                            RowSelected["Action"] = Aoc.ActionId; // find the first action for this unique
                                        }
                                        else
                                        {
                                            RowSelected["Action"] = ActionType; // this part will not happen 
                                        }
                                    }

                                }
                                else
                                {
                                    RowSelected["Action"] = ActionType; // equal to zero 
                                }


                                RowSelected["Terminal"] = TerminalId;

                                RowSelected["Type"] = "N/A";
                                if (TerminalType == "10")
                                {
                                    RowSelected["Type"] = "ATM";
                                }
                                if (TerminalType == "20")
                                {
                                    RowSelected["Type"] = "POS";
                                }
                                if (TerminalType == "30")
                                {
                                    RowSelected["Type"] = "WEB";
                                }


                                RowSelected["Trace"] = TraceNoWithNoEndZero;

                                RowSelected["RRNumber"] = RRNumber;

                                RowSelected["Descr"] = TransDescr;
                                RowSelected["Err"] = MetaExceptionId;
                                RowSelected["Mask"] = MatchMask;
                                RowSelected["Account"] = AccNumber;
                                RowSelected["Ccy"] = TransCurr;
                                RowSelected["Amount"] = TransAmount;
                                RowSelected["Date"] = TransDate;

                                RowSelected["TransType"] = TransType;

                                RowSelected["MatchingCateg"] = MatchingCateg;
                                RowSelected["RMCateg"] = RMCateg;
                                RowSelected["Card"] = CardNumber;
                                RowSelected["ReplCycleNo"] = ReplCycleNo;

                                RowSelected["UserId"] = InSignedId;

                                RowSelected["Comments"] = Comments;
                                RowSelected["UnMatchedType"] = UnMatchedType;

                                // ADD ROW

                                MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                            }


                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable_HST(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                         string InSortCriteria, DateTime InFromDt, DateTime InToDt, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            TotalAmountSelected = 0;

            TotalOutstanding = 0;

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();


            // DATA TABLE ROWS DEFINITION 

            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
                MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
                MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            }

            if (InMode == 3)
            {
                MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            }
            MatchingMasterDataTableATMs.Columns.Add("Action", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TransType", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Comments", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UnMatchedType", typeof(string));

            if (In_DB_Mode == 1)
            {
                if (InFromDt != NullPastDate)
                {
                    SqlString =
                        "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                        + " "
                         + InSortCriteria;
                }
                if (InFromDt == NullPastDate)
                {
                    SqlString = "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                         + " "
                         + InSortCriteria;
                }
            }

            if (In_DB_Mode == 2)
            {

                if (InFromDt != NullPastDate)
                {
                    SqlString =
                    " WITH TempTbl AS "
                    + " ( "
                    + " SELECT *  "
                    + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (Net_TransDate >= @FromDt AND Net_TransDate <= @ToDt) "
                    + " UNION ALL  "
                    + " SELECT * "
                    + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria // Includes Where
                    + " AND (Net_TransDate > @FromDt AND Net_TransDate < @ToDt) "
                    + " ) "
                    + " SELECT * FROM TempTbl "
                    + InSortCriteria
                    + "  ";

                }
                if (InFromDt == NullPastDate)
                {
                    SqlString =
                   " WITH TempTbl AS "
                   + " ( "
                   + " SELECT *  "
                   + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " UNION ALL  "
                   + " SELECT * "
                   + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + InSelectionCriteria // Includes Where
                   + " ) "
                   + " SELECT * FROM TempTbl "
                     + InSortCriteria
                    + "  ";

                }
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }


                        // Read table 
                        cmd.CommandTimeout = 150;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            TotalSelected = TotalSelected + 1;

                            TotalAmountSelected = TotalAmountSelected + TransAmount;

                            if (ActionType == "00")
                            {
                                TotalOutstanding = TotalOutstanding + 1;
                            }

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            if (InMode == 1 || InMode == 2 || InMode == 5)
                            {
                                RowSelected["RecordId"] = UniqueRecordId;

                                if (ActionType == "00")
                                {
                                    RowSelected["Done"] = "NO";
                                }
                                else
                                {
                                    // Eg = to 1
                                    RowSelected["Done"] = "YES";
                                }

                                if (Matched == true)
                                {
                                    RowSelected["Status"] = "M";

                                }
                                else
                                {
                                    RowSelected["Status"] = "U";

                                }
                            }

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount;
                                RowSelected["RecordId"] = UniqueRecordId;
                            }

                            RowSelected["Action"] = ActionType;
                            RowSelected["Terminal"] = TerminalId;

                            RowSelected["Type"] = "N/A";
                            if (TerminalType == "10")
                            {
                                RowSelected["Type"] = "ATM";
                            }
                            if (TerminalType == "20")
                            {
                                RowSelected["Type"] = "POS";
                            }
                            if (TerminalType == "30")
                            {
                                RowSelected["Type"] = "WEB";
                            }


                            RowSelected["Trace"] = TraceNoWithNoEndZero;

                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;

                            RowSelected["TransType"] = TransType;

                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["ReplCycleNo"] = ReplCycleNo;

                            RowSelected["UserId"] = InSignedId;

                            RowSelected["Comments"] = Comments;
                            RowSelected["UnMatchedType"] = UnMatchedType;

                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }


        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable_REPL(string InOperator, string InSignedId, int InMode, string InTerminalId,
                                           DateTime InFromDt, DateTime InToDt, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            TotalAmountSelected = 0;

            TotalOutstanding = 0;

            // If InMode = 1 Only 55 
            // If InMode = 2 Only Deposits issues 
            // If InMode = 3 Both
            // If InMode = 4 is only exceptions but checking actions too
            // If InMode = 5 is only exceptions and Actions = 91 OR 92 were taken at Reconciliation 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();


            // DATA TABLE ROWS DEFINITION 


            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TransType", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));

            if (In_DB_Mode == 1 & InMode == 3)
            {

                SqlString =
                 " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId =@TerminalId "
                + " AND  ( MetaExceptionId in (55, 225, 226)  "
                 + " OR (Matched = 0 AND ActionType = '08' )" // Move from reconciliation
                  + " OR (Matched = 0 AND SeqNo06 = 8 ) ) " // At replenishment when we take action on 08

                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ORDER  By MetaExceptionId DESC ";
            }

            if (In_DB_Mode == 1 & InMode == 4)
            {

                SqlString =
                 " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId =@TerminalId "
                  + " AND ( MetaExceptionId in (55, 225, 226) OR (Matched = 0 AND ActionType = '08') ) "
                  // 08 Move from reconciliation "
                  + "  AND (ActionType = '00' OR ActionType = '08') "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ORDER  By MetaExceptionId DESC ";
            }

            if (In_DB_Mode == 2 & InMode == 1)
            {

                SqlString =
                " WITH TempTbl AS "
                + " ( "
                + " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE " //IsMatchingDone = 1 
                + "  TerminalId =@TerminalId "
                + " AND ( "
                   + " (Matched = 0 AND ActionType = '08') " // Move from Reconciliation 
                   + " OR (Matched = 0 AND ActionType = '05' ) " // Coming from Dispute
                    + " OR (Matched = 0 AND SeqNo06 = 8 ) " // At replenishment when we take action on 08
                   + " OR (Matched = 1 AND MetaExceptionId = 55)"
                   + " OR (Matched = 0 AND MetaExceptionId = 55)"
                    + " )"
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " UNION ALL  "
                + " SELECT * "
                + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE IsMatchingDone = 1  AND TerminalId =@TerminalId "
                  + " AND (Matched = 1  AND  MetaExceptionId = 55 ) "
                  + "  "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ) "
                + " SELECT * FROM TempTbl "

                + " ORDER  By MetaExceptionId DESC ";
            }

            if (In_DB_Mode == 2 & InMode == 4)
            {

                SqlString =
                " WITH TempTbl AS "
                + " ( "
                + " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE " //IsMatchingDone = 1 
                + "  TerminalId =@TerminalId "
                   + " AND ( MetaExceptionId in (55, 225, 226) OR (Matched = 0 AND ActionType = '08') ) "
                  // 08 Move from reconciliation "
                  + "  AND (ActionType = '00' OR ActionType = '08') "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " UNION ALL  "
                + " SELECT * "
                + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE IsMatchingDone = 1  AND TerminalId =@TerminalId "
                    + " AND ( MetaExceptionId in (55, 225, 226) OR (Matched = 0 AND ActionType = '08') ) "
                  // 08 Move from reconciliation "
                  + "  AND (ActionType = '00' OR ActionType = '08') "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ) "
                + " SELECT * FROM TempTbl "

                + " ORDER  By MetaExceptionId DESC ";
            }

            if (In_DB_Mode == 2 & InMode == 2)
            {

                SqlString =
                " WITH TempTbl AS "
                + " ( "
                + " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE IsMatchingDone = 1 "
                + " AND TerminalId =@TerminalId "
               + " AND ( "
               + "  (Matched = 0 AND MetaExceptionId in (225, 226) ) "
                + "  OR (Matched = 1 AND MetaExceptionId in (225, 226) ) "
                + " ) "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " UNION ALL  "
                + " SELECT * "
                + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE IsMatchingDone = 1  AND TerminalId =@TerminalId "
                  + " AND Matched = 1  AND  (MetaExceptionId in ( 225, 226) )  "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ) "
                + " SELECT * FROM TempTbl "

                + " ORDER  By MetaExceptionId DESC ";
            }

            if (In_DB_Mode == 2 & InMode == 3)
            {

                SqlString =
                " WITH TempTbl AS "
                + " ( "
                + " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE IsMatchingDone = 1 "
                + " AND TerminalId =@TerminalId "
               + " AND ( "
               + "  (Matched = 0 AND (ActionType = '00'OR ActionType = '08' ) "
                + "  OR (Matched = 1 AND MetaExceptionId in (55, 225, 226) )) "
                + " ) "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " UNION ALL  "
                + " SELECT * "
                + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE IsMatchingDone = 1  AND TerminalId =@TerminalId "
                  + " AND Matched = 1  AND  (MetaExceptionId in (55, 225, 226) )  "
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ) "
                + " SELECT * FROM TempTbl "

                + " ORDER  By MetaExceptionId DESC ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                        cmd.CommandTimeout = 350;  

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            ReadFieldsInTable(rdr);


                            TotalSelected = TotalSelected + 1;

                            TotalAmountSelected = TotalAmountSelected + TransAmount;

                            if (
                                ActionType == "08" // To be examined for possible presenter 
                                || (ActionType == "00" & MetaExceptionId == 55)
                                || (ActionType == "00" & MetaExceptionId == 225)
                                || (ActionType == "00" & MetaExceptionId == 226)
                                )
                            {
                                TotalOutstanding = TotalOutstanding + 1;
                            }

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            if (InMode == 1 || InMode == 2 || InMode == 5)
                            {
                                RowSelected["RecordId"] = UniqueRecordId;
                                if (Matched == true)
                                {
                                    RowSelected["Status"] = "M";
                                    RowSelected["Done"] = "YES";
                                    if (
                                ActionType == "08" // To be examined for possible presenter 
                                || (ActionType == "00" & MetaExceptionId == 55)
                                || (ActionType == "00" & MetaExceptionId == 225)
                                || (ActionType == "00" & MetaExceptionId == 226)
                                )
                                    {
                                        RowSelected["Done"] = "NO";
                                    }
                                }
                                else
                                {
                                    RowSelected["Status"] = "U";

                                    if (ActionType == "00")
                                    {
                                        RowSelected["Done"] = "NO";
                                    }
                                    else
                                    {
                                        // Eg = to 1
                                        RowSelected["Done"] = "YES";
                                    }
                                }
                            }

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount;
                                RowSelected["RecordId"] = UniqueRecordId;
                            }

                            RowSelected["Terminal"] = TerminalId;

                            RowSelected["Type"] = "N/A";
                            if (TerminalType == "10")
                            {
                                RowSelected["Type"] = "ATM";
                            }
                            if (TerminalType == "20")
                            {
                                RowSelected["Type"] = "POS";
                            }
                            if (TerminalType == "30")
                            {
                                RowSelected["Type"] = "WEB";
                            }


                            RowSelected["Trace"] = TraceNoWithNoEndZero;

                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;

                            RowSelected["TransType"] = TransType;

                            RowSelected["ActionType"] = ActionType;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["ReplCycleNo"] = ReplCycleNo;

                            RowSelected["UserId"] = InSignedId;
                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public decimal TotalDebit;
        public decimal TotalCredit;
        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable_Actions_91_92(string InOperator, string InSignedId, int InMode, string InTerminalId,
                                           DateTime InFromDt, DateTime InToDt, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            TotalAmountSelected = 0;

            TotalOutstanding = 0;

            TotalDebit = 0;
            TotalCredit=0;

        // If InMode = 5 is only exceptions and Actions = 91 OR 92 were taken at Reconciliation 

        MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();


            // DATA TABLE ROWS DEFINITION 


            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TransType", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));

            if (In_DB_Mode == 1 & InMode == 5)
            {

                SqlString =
                 " SELECT *  "
                + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " WHERE TerminalId =@TerminalId "
                + " AND (Matched = 0 AND ActionType in ( '91', '92') )" // Money actions like reversals 
                  + " AND  SettledRecord = 1 AND MatchMask in ('011', '01', '1R', '1R0', '1RR') " // 
                + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                + " ORDER  By MetaExceptionId DESC ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                        cmd.CommandTimeout = 350;

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;


                            ReadFieldsInTable(rdr);


                            TotalSelected = TotalSelected + 1;

                            if (TransType == 11)
                            {
                                TotalDebit = TotalDebit + TransAmount;
                            }
                            if (TransType == 23)
                            {
                                TotalCredit = TotalCredit + TransAmount;
                            }
                            TotalAmountSelected = TotalAmountSelected + TransAmount;

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                          
                            RowSelected["RecordId"] = UniqueRecordId;

                            RowSelected["Status"] = "U";
                            RowSelected["Done"] = "YES";


                            RowSelected["Terminal"] = TerminalId;

                            RowSelected["Type"] = "N/A";
                            if (TerminalType == "10")
                            {
                                RowSelected["Type"] = "ATM";
                            }
                            if (TerminalType == "20")
                            {
                                RowSelected["Type"] = "POS";
                            }
                            if (TerminalType == "30")
                            {
                                RowSelected["Type"] = "WEB";
                            }


                            RowSelected["Trace"] = TraceNoWithNoEndZero;

                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;

                            RowSelected["TransType"] = TransType;

                            RowSelected["ActionType"] = ActionType;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["ReplCycleNo"] = ReplCycleNo;

                            RowSelected["UserId"] = InSignedId;
                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    InsertReport(InOperator, InSignedId, InMode);
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

        //bool DefaultAction;
        public int PageFirstSeqNo;
        public int PageLastSeqNo;
        public int NumberOfRecords;
        string SqlString_TopRecords;

        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                          string InSortCriteria, DateTime InFromDt, DateTime InToDt, int InPageSize, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
                MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
                MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            }

            if (InMode == 3)
            {
                MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
                MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            }
            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));

            if (In_DB_Mode == 1)
            {
                if (InFromDt != NullPastDate)
                {
                    SqlString = "SELECT *"
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                         + " "
                         + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                        + " "
                         + InSortCriteria;
                }
                if (InFromDt == NullPastDate)
                {
                    SqlString_TopRecords = "SELECT TOP " + InPageSize + " * "
                          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                          + " "
                          + InSelectionCriteria
                          + " "
                          + InSortCriteria;
                }
            }

            if (In_DB_Mode == 2)
            {
                if (InFromDt != NullPastDate)
                {
                    SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                  + InSelectionCriteria
                          + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                  + InSortCriteria
                    + "  ";
                }
                if (InFromDt == NullPastDate)
                {
                    SqlString_TopRecords =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + "SELECT TOP " + InPageSize + " * FROM MergedTbl "
                 + InSortCriteria
                 + "  ";
                }
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString_TopRecords, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            if (TotalSelected == 1)
                            {
                                PageFirstSeqNo = SeqNo;
                            }
                            else
                            {
                                PageLastSeqNo = SeqNo;
                            }

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            if (InMode == 1 || InMode == 2 || InMode == 5)
                            {
                                RowSelected["RecordId"] = UniqueRecordId;
                                if (Matched == true)
                                {
                                    RowSelected["Status"] = "M";
                                    RowSelected["Done"] = "YES";
                                }
                                else
                                {
                                    RowSelected["Status"] = "U";

                                    if (ActionType == "00")
                                    {
                                        RowSelected["Done"] = "NO";
                                    }
                                    else
                                    {
                                        // Eg = to 1
                                        RowSelected["Done"] = "YES";
                                    }
                                }
                            }

                            if (InMode == 3)
                            {
                                RowSelected["Select"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount;
                                RowSelected["RecordId"] = UniqueRecordId;
                            }

                            RowSelected["Terminal"] = TerminalId;

                            RowSelected["Type"] = "N/A";
                            if (TerminalType == "10")
                            {
                                RowSelected["Type"] = "ATM";
                            }
                            if (TerminalType == "20")
                            {
                                RowSelected["Type"] = "POS";
                            }
                            if (TerminalType == "30")
                            {
                                RowSelected["Type"] = "WEB";
                            }
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;
                            RowSelected["Trace"] = TraceNoWithNoEndZero;
                            RowSelected["ActionType"] = ActionType;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["ReplCycleNo"] = ReplCycleNo;
                            RowSelected["UserId"] = InSignedId;

                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    //   InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        //
        // READ TABLE BY PAGING 
        //
        public void ReadMatchingTxnsMasterPoolByRangeAndFillTable_Paging_WEB(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                         string InSortCriteria, DateTime InFromDt, DateTime InToDt, int InPageSize)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
                MatchingMasterDataTableATMs.Columns.Add("Status", typeof(string));
                MatchingMasterDataTableATMs.Columns.Add("Done", typeof(string));
            }

            //if (InMode == 3)
            //{
            //    MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
            //    MatchingMasterDataTableATMs.Columns.Add("DisputedAmnt", typeof(decimal));
            //    MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            //}
            MatchingMasterDataTableATMs.Columns.Add("Terminal", typeof(string));
            //  MatchingMasterDataTableATMs.Columns.Add("Type", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Err", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Mask", typeof(string));
            //    MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Trace", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            // MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            //   MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            //   MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            //   MatchingMasterDataTableATMs.Columns.Add("ReplCycleNo", typeof(string));
            //   MatchingMasterDataTableATMs.Columns.Add("UserId", typeof(string));

            if (InFromDt != NullPastDate)
            {
                SqlString = "SELECT *"
                     + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                     + " "
                     + InSelectionCriteria
                      + " AND(TransDate > @FromDt AND TransDate < @ToDt) "
                    + " "
                     + InSortCriteria;
            }
            if (InFromDt == NullPastDate)
            {
                SqlString_TopRecords = "SELECT TOP " + InPageSize + " * "
                      + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " "
                      + InSelectionCriteria
                      + " "
                      + InSortCriteria;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString_TopRecords, conn))
                    {
                        if (InFromDt != NullPastDate)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadFieldsInTable(rdr);

                            if (TotalSelected == 1)
                            {
                                PageFirstSeqNo = SeqNo;
                            }
                            else
                            {
                                PageLastSeqNo = SeqNo;
                            }

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            if (InMode == 1 || InMode == 2 || InMode == 5)
                            {
                                RowSelected["RecordId"] = UniqueRecordId;
                                if (Matched == true)
                                {
                                    RowSelected["Status"] = "M";
                                    RowSelected["Done"] = "YES";
                                }
                                else
                                {
                                    RowSelected["Status"] = "U";

                                    if (ActionType == "00")
                                    {
                                        RowSelected["Done"] = "NO";
                                    }
                                    else
                                    {
                                        // Eg = to 1
                                        RowSelected["Done"] = "YES";
                                    }
                                }
                            }

                            //if (InMode == 3)
                            //{
                            //    RowSelected["Select"] = false;
                            //    RowSelected["DisputedAmnt"] = TransAmount;
                            //    RowSelected["RecordId"] = UniqueRecordId;
                            //}

                            RowSelected["Terminal"] = TerminalId;

                            //RowSelected["Type"] = "N/A";
                            //if (TerminalType == "10")
                            //{
                            //    RowSelected["Type"] = "ATM";
                            //}
                            //if (TerminalType == "20")
                            //{
                            //    RowSelected["Type"] = "POS";
                            //}
                            //if (TerminalType == "30")
                            //{
                            //    RowSelected["Type"] = "WEB";
                            //}
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Err"] = MetaExceptionId;
                            RowSelected["Mask"] = MatchMask;
                            //    RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;
                            RowSelected["Trace"] = TraceNoWithNoEndZero;
                            RowSelected["ActionType"] = ActionType;
                            //RowSelected["MatchingCateg"] = MatchingCateg;
                            //RowSelected["RMCateg"] = RMCateg;
                            //RowSelected["Card"] = CardNumber;
                            //RowSelected["ReplCycleNo"] = ReplCycleNo;
                            //RowSelected["UserId"] = InSignedId;

                            // ADD ROW

                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    //  InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void ReadMatchingTxnsMasterPoolBySelection_Paging_Max(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                         string InSortCriteria, DateTime InFromDt, DateTime InToDt, int InPageSize)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            NumberOfRecords = 0;

            string SqlString_Max = "SELECT COUNT(*) As NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + " "
                    + InSelectionCriteria
                    + " "
                    + InSortCriteria;

            using (SqlConnection conn1 =
                        new SqlConnection(connectionString))
                try
                {
                    conn1.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString_Max, conn1))
                    {
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            NumberOfRecords = (int)rdr["NumberOfRecords"];
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn1.Close();

                }
                catch (Exception ex)
                {
                    conn1.Close();

                    CatchDetails(ex);
                }


        }
        //
        // FILL TABLE FOR UCForm271C_JCC
        //
        public void ReadMatchingTxnsMasterPoolByCategoryAndCycleAndFillTable(string InSelectionCriteria, int In_DB_Mode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ActionTakenDescr = "";

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            // If InMode = 1 then is normal without action 
            // If InMode = 2 then we examine action taken ... locate and meta exception no
            // if InMode = 3 then this comes from Dispute registration 

            // if InMode = 5 Update meta exception number and settled transaction if it is a default action 

            DataTableActionsTaken = new DataTable();
            DataTableActionsTaken.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            DataTableActionsTaken.Columns.Add("RecordId", typeof(int));

            DataTableActionsTaken.Columns.Add("Terminal", typeof(string));
            DataTableActionsTaken.Columns.Add("Descr", typeof(string));

            DataTableActionsTaken.Columns.Add("Mask", typeof(string));
            DataTableActionsTaken.Columns.Add("Card", typeof(string));
            DataTableActionsTaken.Columns.Add("Account", typeof(string));
            DataTableActionsTaken.Columns.Add("Ccy", typeof(string));
            DataTableActionsTaken.Columns.Add("Amount", typeof(decimal));
            DataTableActionsTaken.Columns.Add("Date", typeof(DateTime));
            DataTableActionsTaken.Columns.Add("Action_Taken", typeof(string));

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " "
                 + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            TotalSelected = TotalSelected + 1;
                            // Fill Table 

                            DataRow RowSelected = DataTableActionsTaken.NewRow();

                            RowSelected["RecordId"] = UniqueRecordId;

                            RowSelected["Terminal"] = TerminalId;
                            RowSelected["Descr"] = TransDescr;

                            RowSelected["Mask"] = MatchMask;

                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Ccy"] = TransCurr;
                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Date"] = TransDate;
                            //comboBoxActionType.Items.Add("11_DEBIT Customer");
                            //    comboBoxActionType.Items.Add("21_CREDIT Customer");
                            //public string ActionType; // 00 ... No Action Taken 
                            //                          // 01 ... Meta Exception Suggested By System 
                            //                          // 11 ... DEBIT Customer
                            //                          // 21 ... CREDIT Customer

                            //                          // 02 ... Meta Exception Manual 
                            //                          // 03 ... 
                            //                          // 04 ... Force Match - Broken Disc Case  
                            //                          // 05 ... Move To Dispute 
                            //                          // 06 ... Move to Pool
                            //                          // 07 ... Default By System
                            //                          // 08 ... UnDo Default
                            //                          // 09 ... Move to Suspense 
                            //                          // 10 ... A new one 
                            if (ActionType == "00")
                            {
                                // Now Action Yet
                                ActionTakenDescr = "No Action Taken Yet";
                            }
                            else
                            {

                                Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", UniqueRecordId, ActionType);
                                ActionTakenDescr = Aoc.ActionNm;

                            }

                            RowSelected["Action_Taken"] = ActionTakenDescr;

                            // ADD ROW

                            DataTableActionsTaken.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // Insert printing record and update meta exception 

                    //InsertReport(InOperator, InSignedId, InMode);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // FILL TABLE FOR_SET_VS_CAP_DATE
        //
        public DataTable DataTable_SET_VS_CAP = new DataTable();

        public void ReadMatchingTxnsMasterPool_FOR_SET_VS_CAP_DATE_Fill_Table(string InMatchingCateg, int InMatchingAtRMCycle)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ActionTakenDescr = "";

            DataTable_SET_VS_CAP = new DataTable();
            DataTable_SET_VS_CAP.Clear();
            TotalSelected = 0;


            SqlString =
               " WITH MergedTbl AS "
             + " ( "
             + " SELECT [SET_DATE], [CAP_DATE], TransCurr,TransAmount  "
             + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
             + " WHERE MatchingCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle AND IsMatchingDone = 1 "
             + " UNION ALL  "
             + " SELECT [SET_DATE], [CAP_DATE],TransCurr, TransAmount "
             + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
             + " WHERE MatchingCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle AND IsMatchingDone = 1 "
             + " ) "
             + " SELECT  "
             + " [SET_DATE], [CAP_DATE],TransCurr, count(*) AS TotalTXNS, SUM(TransAmount) As TotalAmount "
             + " FROM MergedTbl "
             + " GROUP by  SET_DATE, CAP_DATE,TransCurr";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);

                        sqlAdapt.Fill(DataTable_SET_VS_CAP);

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

        public DataTable DataTable_SET_VS_CAP_TXNS = new DataTable();
        public void ReadMatchingTxnsMasterPool_FOR_SET_VS_CAP_DATE_Fill_Table_TXNS(string InMatchingCateg, int InMatchingAtRMCycle
                                                                         , DateTime InSET_DATE, DateTime InCAP_DATE, string InTransCurr)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ActionTakenDescr = "";

            DataTable_SET_VS_CAP_TXNS = new DataTable();
            DataTable_SET_VS_CAP_TXNS.Clear();
            TotalSelected = 0;


            SqlString =
               " WITH MergedTbl AS "
             + " ( "
             + " SELECT SeqNo, CAP_DATE, TransDate,TransCurr ,TransAmount, RRNumber,TraceNoWithNoEndZero , TerminalId, [LoadedAtRMCycle],[MatchingAtRMCycle]  "
             + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
             + " WHERE MatchingCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle AND IsMatchingDone = 1 "
              + " AND SET_DATE = @SET_DATE AND CAP_DATE = @CAP_DATE AND TransCurr = @TransCurr "
             + " UNION ALL  "
             + " SELECT SeqNo, CAP_DATE, TransDate,TransCurr ,TransAmount, RRNumber,TraceNoWithNoEndZero , TerminalId, [LoadedAtRMCycle],[MatchingAtRMCycle]  "
             + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            + " WHERE MatchingCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle AND IsMatchingDone = 1 "
              + " AND SET_DATE = @SET_DATE AND CAP_DATE = @CAP_DATE AND TransCurr = @TransCurr "
             + " ) "
             + " SELECT * "
             + " FROM MergedTbl "
             + " ORDER By TransDate";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SET_DATE", InSET_DATE);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransCurr", InTransCurr);

                        sqlAdapt.Fill(DataTable_SET_VS_CAP_TXNS);

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
        // Insert 
        private void InsertReport(string InOperator, string InSignedId, int InMode)
        {
            if (InMode == 1 || InMode == 2 || InMode == 5) // DISPUTE NOT INCLUDED
            {
                ////ReadTable And Insert In Sql Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport54(InSignedId);

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn2 =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn2.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport54]";

                            foreach (var column in MatchingMasterDataTableATMs.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(MatchingMasterDataTableATMs);
                        }
                        conn2.Close();
                    }
                    catch (Exception ex)
                    {
                        conn2.Close();

                        CatchDetails(ex);
                    }

                //int I = 0;

                //while (I <= (MatchingMasterDataTableATMs.Rows.Count - 1))
                //{
                //    //    RecordFound = true;
                //    UniqueRecordId = Tr.RecordId = (int)MatchingMasterDataTableATMs.Rows[I]["RecordId"];
                //    Tr.Status = (string)MatchingMasterDataTableATMs.Rows[I]["Status"];
                //    Tr.Done = (string)MatchingMasterDataTableATMs.Rows[I]["Done"];

                //    Tr.ATMNo = (string)MatchingMasterDataTableATMs.Rows[I]["Terminal"];
                //    Tr.Descr = (string)MatchingMasterDataTableATMs.Rows[I]["Descr"];
                //    Tr.Card = (string)MatchingMasterDataTableATMs.Rows[I]["Card"];

                //    Tr.Account = (string)MatchingMasterDataTableATMs.Rows[I]["Account"];
                //    Tr.Ccy = (string)MatchingMasterDataTableATMs.Rows[I]["Ccy"];
                //    Tr.Amount = (decimal)MatchingMasterDataTableATMs.Rows[I]["Amount"];

                //    Tr.Date = (DateTime)MatchingMasterDataTableATMs.Rows[I]["Date"];

                //    Tr.MatchingCateg = (string)MatchingMasterDataTableATMs.Rows[I]["MatchingCateg"];
                //    Tr.RMCateg = (string)MatchingMasterDataTableATMs.Rows[I]["RMCateg"];

                //    Tr.ExceptID = (int)MatchingMasterDataTableATMs.Rows[I]["Err"];
                //    MatchMask = (string)MatchingMasterDataTableATMs.Rows[I]["Mask"];
                //    ActionType = (string)MatchingMasterDataTableATMs.Rows[I]["ActionType"];

                //    // Insert record for printing 
                //    //
                //    Tr.InsertReport54(InSignedId);

                //    I++; // Read Next entry of the table 

                //}

            }
        }


        // READ UNMATCHED TXNS FOR FAST TRACK
        public void ReadMatchingTxnsMasterPoolAndFillTableFastTrack(string InOperator, string InSignedId, string InSelectionCriteria,
                                         string InSortCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAccountsClass Acc = new RRDMAccountsClass();

            RRDMActions_GL Ag = new RRDMActions_GL();

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
            MatchingMasterDataTableATMs.Columns.Add("MatchMask", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ActionDesc", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Settled", typeof(bool));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Exception", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ATMNo", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("FirstEntry", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("FirstEntryAccno", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("SecondEntry", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("SecondEntryAccno", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TraceNo", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " "
                 + InSelectionCriteria
                 + " "
                 + InSortCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            //if (UniqueRecordId == 815564)
                            //{
                            //    RowSelected["RecordId"] = UniqueRecordId;
                            //}

                            RowSelected["RecordId"] = UniqueRecordId;

                            //if (ActionType == "00") RowSelected["Select"] = false;
                            //else
                            //{
                            //    if (FastTrack == true) RowSelected["Select"] = true;
                            //    else RowSelected["Select"] = false;
                            //}

                            RowSelected["MatchMask"] = MatchMask;
                            RowSelected["ActionType"] = ActionType;
                            if (ActionType == "00")
                            {
                                RowSelected["ActionDesc"] = "No Action Yet";
                                RowSelected["Select"] = false;
                            }

                            Ag.ReadActionByActionId(Operator, ActionType, 1);
                            if (ActionType != "00")
                            {
                                RowSelected["ActionDesc"] = Ag.ActionNm;
                                RowSelected["Select"] = true;
                            }

                            if (SettledRecord == true)
                            {
                                RowSelected["Settled"] = true;

                            }
                            else
                            {
                                RowSelected["Settled"] = false;
                            }

                            RowSelected["Date"] = TransDate;
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Ccy"] = TransCurr;

                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Exception"] = MetaExceptionId;

                            RowSelected["Card"] = CardNumber;

                            RowSelected["Account"] = AccNumber;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["ATMNo"] = TerminalId;

                            if (Origin == "Our Atms")
                            {
                                // initialise
                                RowSelected["FirstEntry"] = "N/A";
                                RowSelected["FirstEntryAccno"] = "N/A";

                                RowSelected["SecondEntry"] = "N/A";
                                RowSelected["SecondEntryAccno"] = "N/A";

                                Er.ReadErrorsTableSpecificByUniqueRecordId(UniqueRecordId);

                                if (Er.RecordFound == true)
                                {
                                    RowSelected["Descr"] = Er.ErrDesc;
                                    RowSelected["Ccy"] = Er.CurDes;

                                    RowSelected["Amount"] = Er.ErrAmount;

                                    string ATMSuspence = "Not Found";
                                    string ATMCash = "Not Found";

                                    Acc.ReadAndFindAccount("1000", "", "", Operator, TerminalId, TransCurr, "ATM Suspense");

                                    if (Acc.RecordFound == true)
                                    {
                                        ATMSuspence = Acc.AccNo;
                                    }

                                    Acc.ReadAndFindAccount("1000", "", "", Operator, TerminalId, TransCurr, "ATM Cash");
                                    if (Acc.RecordFound == true)
                                    {
                                        ATMCash = Acc.AccNo;
                                    }

                                    if (Er.DrCust == true)
                                    {
                                        RowSelected["FirstEntry"] = "DEBIT CUSTOMER ";
                                        RowSelected["FirstEntryAccno"] = AccNumber;
                                    }
                                    if (Er.CrCust == true)
                                    {
                                        RowSelected["FirstEntry"] = " CREDIT CUSTOMER ";
                                        RowSelected["FirstEntryAccno"] = AccNumber;
                                    }
                                    if (Er.DrAtmCash == true)
                                    {
                                        RowSelected["SecondEntry"] = "DEBIT ATM CASH ";
                                        RowSelected["SecondEntryAccno"] = ATMCash;
                                    }
                                    if (Er.CrAtmCash == true)
                                    {
                                        RowSelected["SecondEntry"] = "CREDIT ATM CASH ";
                                        RowSelected["SecondEntryAccno"] = ATMCash;
                                    }

                                    if (Er.DrCust == false & Er.CrCust == false)
                                    {
                                        //
                                        // GL TRXN ONLY
                                        //
                                        if (Er.DrAtmCash == true)
                                        {
                                            RowSelected["FirstEntry"] = " DEBIT ATM CASH ";
                                            RowSelected["FirstEntryAccno"] = ATMCash;
                                        }
                                        if (Er.CrAtmCash == true)
                                        {
                                            RowSelected["FirstEntry"] = "CREDIT ATM CASH ";
                                            RowSelected["FirstEntryAccno"] = ATMCash;
                                        }

                                        if (Er.DrAtmSusp == true)
                                        {
                                            RowSelected["SecondEntry"] = " DEBIT ATM SUSPENSE";
                                            RowSelected["SecondEntryAccno"] = ATMSuspence;
                                        }
                                        if (Er.CrAtmSusp == true)
                                        {
                                            RowSelected["SecondEntry"] = " CREDIT ATM SUSPENSE";
                                            RowSelected["SecondEntryAccno"] = ATMSuspence;
                                        }
                                    }
                                }

                                RowSelected["TraceNo"] = TraceNoWithNoEndZero;
                                RowSelected["RRNumber"] = RRNumber;
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show("Complete Code For JCC ");

                                // initialise
                                RowSelected["FirstEntry"] = "N/A";
                                RowSelected["FirstEntryAccno"] = "N/A";

                                RowSelected["SecondEntry"] = "N/A";
                                RowSelected["SecondEntryAccno"] = "N/A";

                                Er.ReadErrorsTableSpecificByUniqueRecordId(UniqueRecordId);

                                if (Er.RecordFound == true)
                                {
                                    RowSelected["Descr"] = Er.ErrDesc;
                                    RowSelected["Ccy"] = Er.CurDes;

                                    RowSelected["Amount"] = Er.ErrAmount;

                                    RRDMMatchingCategories Mc = new RRDMMatchingCategories();
                                    Mc.ReadMatchingCategorybyActiveCategId(InOperator, MatchingCateg);
                                    string JccSuspence = "CategorySuspence";
                                    string CategoryGlAccount = Mc.GlAccount;

                                    if (Er.DrCust == true)
                                    {
                                        RowSelected["FirstEntry"] = "DEBIT CUSTOMER ";
                                        RowSelected["FirstEntryAccno"] = AccNumber;
                                    }
                                    if (Er.CrCust == true)
                                    {
                                        RowSelected["FirstEntry"] = " CREDIT CUSTOMER ";
                                        RowSelected["FirstEntryAccno"] = Er.CustAccNo;
                                    }
                                    if (Er.DrAtmCash == true)
                                    {
                                        RowSelected["SecondEntry"] = "DEBIT Category GL Account ";
                                        RowSelected["SecondEntryAccno"] = CategoryGlAccount;
                                    }
                                    if (Er.CrAtmCash == true)
                                    {
                                        RowSelected["SecondEntry"] = "CREDIT Category GL Account ";
                                        RowSelected["SecondEntryAccno"] = CategoryGlAccount;
                                    }
                                    // Category GL 
                                    if (Er.DrAccount3 == true)
                                    {
                                        RowSelected["SecondEntry"] = "DEBIT Origin Category ";
                                        RowSelected["SecondEntryAccno"] = Er.AccountNo3;
                                    }
                                    if (Er.CrAccount3 == true)
                                    {
                                        RowSelected["SecondEntry"] = "CREDIT Origin Category ";
                                        RowSelected["SecondEntryAccno"] = Er.AccountNo3;
                                    }

                                    if (Er.DrCust == false & Er.CrCust == false)
                                    {
                                        //
                                        // GL TRXN ONLY
                                        //
                                        if (Er.DrAtmCash == true)
                                        {
                                            RowSelected["FirstEntry"] = " DEBIT Category  ";
                                            RowSelected["FirstEntryAccno"] = CategoryGlAccount;
                                        }
                                        if (Er.CrAtmCash == true)
                                        {
                                            RowSelected["FirstEntry"] = "CREDIT Category";
                                            RowSelected["FirstEntryAccno"] = CategoryGlAccount;
                                        }

                                        if (Er.DrAtmSusp == true)
                                        {
                                            RowSelected["SecondEntry"] = " DEBIT SUSPENSE";
                                            RowSelected["SecondEntryAccno"] = JccSuspence;
                                        }
                                        if (Er.CrAtmSusp == true)
                                        {
                                            RowSelected["SecondEntry"] = " CREDIT SUSPENSE";
                                            RowSelected["SecondEntryAccno"] = JccSuspence;
                                        }
                                    }
                                }

                                RowSelected["TraceNo"] = TraceNoWithNoEndZero;
                                RowSelected["RRNumber"] = RRNumber;
                            }

                            // ADD ROW
                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                            TotalSelected = TotalSelected + 1;
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();


                    int WMode = 2;

                    InsertReportFastTrack(InOperator, InSignedId, WMode);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // READ UNMATCHED TXNS SHort 
        public void ReadMatchingTxnsMasterPoolAndFillTableUnmatched_Short(string InOperator, string InSignedId, string InSelectionCriteria,
                                         string InSortCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " "
                 + InSelectionCriteria
                 + " "
                 + InSortCriteria;
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            RowSelected["RecordId"] = UniqueRecordId;

                            // ADD ROW
                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

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


        // READ POOL And Fill Table General 
        public void ReadMatchingTxnsMasterPoolAndFillTableGeneral(string InOperator, string InSignedId, string InSelectionCriteria,
                                         string InSortCriteria, int In_DB_Mode, bool InFirstCycle, DateTime InSettlementDate, int InFunction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ATMNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Encrypted_Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("TraceNo", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
            MatchingMasterDataTableATMs.Columns.Add("MatchMask", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Settled", typeof(bool));

            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ActionDesc", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Exception", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("TXNSRC", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TXNDEST", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Maker", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Author", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("CAP_DATE", typeof(DateTime));

            MatchingMasterDataTableATMs.Columns.Add("SET_DATE", typeof(DateTime));

            MatchingMasterDataTableATMs.Columns.Add("Comments", typeof(string));

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " "
                 + InSelectionCriteria
                 + " "
                 + InSortCriteria;
            }

            if (In_DB_Mode == 2 & InFirstCycle == false)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            if (In_DB_Mode == 2 & InFirstCycle == true)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT TOP 1000 *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT TOP 1000 * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT TOP 1000 * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            if (In_DB_Mode == 2 & InFirstCycle == false & (InFunction == 28 || InFunction == 29))
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT  *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            // String Output = "";

            // System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\Test.tab");


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        if (InFunction == 28 || InFunction == 29)
                        {
                            cmd.Parameters.AddWithValue("@SET_DATE", InSettlementDate.Date);
                        }

                        cmd.CommandTimeout = 120;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            RowSelected["RecordId"] = UniqueRecordId;


                            RowSelected["MatchMask"] = MatchMask;
                            RowSelected["ActionType"] = ActionType;

                            if (Matched == true & MetaExceptionId != 55)
                            {
                                RowSelected["ActionDesc"] = "No Action Needed";
                            }
                            else
                            {
                                if (ActionType == "00")
                                {
                                    RowSelected["ActionDesc"] = "No Action Yet";
                                    // RowSelected["Select"] = false;
                                }
                                else
                                {
                                    Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", UniqueRecordId, ActionType);
                                    if (Aoc.RecordFound == true)
                                    {
                                        if (Aoc.Stage == "03")
                                        {
                                            RowSelected["ActionDesc"] = Aoc.ActionNm;
                                            TransAmount = Aoc.DoubleEntryAmt; 
                                        }
                                        else
                                        {
                                            RowSelected["ActionDesc"] = "NotCompleted..." + Aoc.ActionNm;
                                        }

                                    }
                                    else
                                    {
                                        RowSelected["ActionDesc"] = "Not Defined";
                                    }

                                }

                            }

                            if (SettledRecord == true)
                            {
                                RowSelected["Settled"] = true;
                            }
                            else
                            {
                                RowSelected["Settled"] = false;
                            }

                            RowSelected["Date"] = TransDate;
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Ccy"] = TransCurr;

                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Exception"] = MetaExceptionId;

                            RowSelected["Card"] = CardNumber;
                            RowSelected["Encrypted_Card"] = Card_Encrypted;

                            RowSelected["Account"] = AccNumber;


                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;

                            RowSelected["TXNSRC"] = TXNSRC;
                            RowSelected["TXNDEST"] = TXNDEST;

                            RowSelected["ATMNo"] = TerminalId;

                            RowSelected["TraceNo"] = TraceNoWithNoEndZero;
                            RowSelected["RRNumber"] = "'" + RRNumber;

                            RowSelected["Maker"] = UserId;
                            RowSelected["Author"] = Authoriser;

                            RowSelected["CAP_DATE"] = CAP_DATE;

                            RowSelected["SET_DATE"] = SET_DATE;

                            RowSelected["Comments"] = Comments;


                            // ADD ROW
                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                            // if (InFunction == 16 & InFirstCycle == false)
                            // {

                            // looping code
                            ///   Output = Output + String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\r\n",
                            //                        UniqueRecordId, TransDate, TransDescr, TransAmount, CardNumber, AccNumber);

                            // }

                            TotalSelected = TotalSelected + 1;
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\\RRDM\\Working\\Test_TAB_01.txt");

                    // OutStream.Write(Output);
                    // end looping code


                    // OutStream.Close();

                    int WMode = 2;

                    InsertReportGeneral(InOperator, InSignedId, WMode);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // READ POOL And Fill Table General from HST
        public void ReadMatchingTxnsMasterPoolAndFillTableGeneral_HST(string InOperator, string InSignedId, string InSelectionCriteria,
                                         string InSortCriteria, int In_DB_Mode, bool InFirstCycle, DateTime InSettlementDate, int InFunction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            MatchingMasterDataTableATMs.Columns.Add("RecordId", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("ATMNo", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Descr", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Encrypted_Card", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Account", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Ccy", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Amount", typeof(decimal));
            MatchingMasterDataTableATMs.Columns.Add("Date", typeof(DateTime));
            MatchingMasterDataTableATMs.Columns.Add("TraceNo", typeof(int));
            MatchingMasterDataTableATMs.Columns.Add("RRNumber", typeof(string));

            //MatchingMasterDataTableATMs.Columns.Add("Select", typeof(bool));
            MatchingMasterDataTableATMs.Columns.Add("MatchMask", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Settled", typeof(bool));

            MatchingMasterDataTableATMs.Columns.Add("ActionType", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("ActionDesc", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Exception", typeof(int));

            MatchingMasterDataTableATMs.Columns.Add("MatchingCateg", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("RMCateg", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("TXNSRC", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("TXNDEST", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("Maker", typeof(string));
            MatchingMasterDataTableATMs.Columns.Add("Author", typeof(string));

            MatchingMasterDataTableATMs.Columns.Add("CAP_DATE", typeof(DateTime));

            MatchingMasterDataTableATMs.Columns.Add("SET_DATE", typeof(DateTime));

            MatchingMasterDataTableATMs.Columns.Add("Comments", typeof(string));

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " "
                 + InSelectionCriteria
                 + " "
                 + InSortCriteria;
            }

            if (In_DB_Mode == 2 & InFirstCycle == false)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            if (In_DB_Mode == 2 & InFirstCycle == true)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT TOP 1000 *  "
                 + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT TOP 1000 * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT TOP 1000 * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            if (In_DB_Mode == 2 & InFirstCycle == false & (InFunction == 28 || InFunction == 29))
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT  *  "
                 + " FROM[RRDM_Reconciliation_ITMX_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + InSortCriteria
                + "  ";
            }

            // String Output = "";

            // System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\Test.tab");


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        if (InFunction == 28 || InFunction == 29)
                        {
                            cmd.Parameters.AddWithValue("@SET_DATE", InSettlementDate.Date);
                        }

                        cmd.CommandTimeout = 120;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            // Fill Table 

                            DataRow RowSelected = MatchingMasterDataTableATMs.NewRow();

                            RowSelected["RecordId"] = UniqueRecordId;


                            RowSelected["MatchMask"] = MatchMask;
                            RowSelected["ActionType"] = ActionType;

                            if (Matched == true & MetaExceptionId != 55)
                            {
                                RowSelected["ActionDesc"] = "No Action Needed";
                            }
                            else
                            {
                                if (ActionType == "00")
                                {
                                    RowSelected["ActionDesc"] = "No Action Yet";
                                    // RowSelected["Select"] = false;
                                }
                                else
                                {
                                    Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", UniqueRecordId, ActionType);
                                    if (Aoc.RecordFound == true)
                                    {
                                        if (Aoc.Stage == "03")
                                        {
                                            RowSelected["ActionDesc"] = Aoc.ActionNm;
                                            TransAmount = Aoc.DoubleEntryAmt;
                                        }
                                        else
                                        {
                                            RowSelected["ActionDesc"] = "NotCompleted..." + Aoc.ActionNm;
                                        }

                                    }
                                    else
                                    {
                                        RowSelected["ActionDesc"] = "Not Defined";
                                    }

                                }

                            }

                            if (SettledRecord == true)
                            {
                                RowSelected["Settled"] = true;
                            }
                            else
                            {
                                RowSelected["Settled"] = false;
                            }

                            RowSelected["Date"] = TransDate;
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Ccy"] = TransCurr;

                            RowSelected["Amount"] = TransAmount;
                            RowSelected["Exception"] = MetaExceptionId;

                            RowSelected["Card"] = CardNumber;
                            RowSelected["Encrypted_Card"] = Card_Encrypted;

                            RowSelected["Account"] = AccNumber;


                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCateg"] = RMCateg;

                            RowSelected["TXNSRC"] = TXNSRC;
                            RowSelected["TXNDEST"] = TXNDEST;

                            RowSelected["ATMNo"] = TerminalId;

                            RowSelected["TraceNo"] = TraceNoWithNoEndZero;
                            RowSelected["RRNumber"] = "'" + RRNumber;

                            RowSelected["Maker"] = UserId;
                            RowSelected["Author"] = Authoriser;

                            RowSelected["CAP_DATE"] = CAP_DATE;

                            RowSelected["SET_DATE"] = SET_DATE;

                            RowSelected["Comments"] = Comments;


                            // ADD ROW
                            MatchingMasterDataTableATMs.Rows.Add(RowSelected);

                            // if (InFunction == 16 & InFirstCycle == false)
                            // {

                            // looping code
                            ///   Output = Output + String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\r\n",
                            //                        UniqueRecordId, TransDate, TransDescr, TransAmount, CardNumber, AccNumber);

                            // }

                            TotalSelected = TotalSelected + 1;
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  System.IO.StreamWriter OutStream = new System.IO.StreamWriter(@"C:\\RRDM\\Working\\Test_TAB_01.txt");

                    // OutStream.Write(Output);
                    // end looping code


                    // OutStream.Close();

                    int WMode = 2;

                    InsertReportGeneral(InOperator, InSignedId, WMode);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }




        // READ POOL And Fill Table General 
        public string ReadMatchingTxnsMasterPoolAndFillTableGeneral_CSV(string InOperator, string InSignedId, string InSelectionCriteria,
                                         string InSortCriteria, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;
            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string CreatedFile = "C:\\RRDM\\Working\\TAB_File_" + InMatchingCateg + ".txt";

            //RRDMAccountsClass Acc = new RRDMAccountsClass();
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            SqlString =
               " WITH MergedTbl AS "
             + " ( "
             + " SELECT * "
            // + " SELECT UniqueRecordId  "
            //+ " , TerminalId  "
            //+ " , TransDescr  "
            //+ " , CardNumber  "
            //+ " , AccNumber  "
            //+ " , TransCurr  "
            //+ " , TransAmount  "
            //+ " , TransDate  "
            //+ " , TraceNoWithNoEndZero  "
            //   + " , RRNumber  "
            //+ " , MatchMask  "
            //+ " , RMCateg  "
            //+ " , TXNSRC  "
            //+ " , TXNDEST  "
            + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
          + InSelectionCriteria
            + " UNION ALL  "
            + " SELECT * "
             //+ " SELECT UniqueRecordId  "
             // + " , TerminalId  "
             // + " , TransDescr  "
             // + " , CardNumber  "
             // + " , AccNumber  "
             // + " , TransCurr  "
             // + " , TransAmount  "
             // + " , TransDate  "
             // + " , TraceNoWithNoEndZero  "
             //    + " , RRNumber  "
             // + " , MatchMask  "
             // + " , RMCateg  "
             // + " , TXNSRC  "
             // + " , TXNDEST  "
             + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            + InSelectionCriteria
             + " ) "
             + " SELECT * FROM MergedTbl "
             + InSortCriteria
            + "  ";

            StreamWriter outputFile = new StreamWriter(@CreatedFile);

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        //if (InFunction == 28 || InFunction == 29)
                        //{
                        //    cmd.Parameters.AddWithValue("@CAP_DATE", InSettlementDate.Date);
                        //}

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            string strline = "";

                            TotalSelected = TotalSelected + 1;

                            if (TotalSelected == 1)
                            {
                                // Make headings
                                strline = "UniqueRecordId" + "\t"
                                                   + "TerminalId" + "\t"
                                                   + "TransDescr" + "\t"
                                                   + "CardNumber" + "\t"
                                                   + "AccNumber" + "\t"
                                                   + "TransCurr" + "\t"
                                                   + "TransAmount" + "\t"
                                                   + "TransDate" + "\t"
                                                   + "TraceNo" + "\t"
                                                   + "RRNumber" + "\t"
                                                   + "MatchMask" + "\t"
                                                   + "RMCateg" + "\t"
                                                   + "TXNSRC" + "\t"
                                                   + "TXNDEST" + "\t"
                                                    + "Settlement";
                                outputFile.WriteLine(strline);

                            }

                            if (ActionType != "00")
                            {
                                Aoc.ReadActionsOccurancesByUniqueKey("Master_Pool", UniqueRecordId, ActionType);
                                if (Aoc.RecordFound == true)
                                {
                                    if (Aoc.Stage == "03")
                                    {
                                        TransAmount = Aoc.DoubleEntryAmt;
                                    }
                                }
                            }
                           
                            

                            strline = UniqueRecordId + "\t"
                                                   + TerminalId + "\t"
                                                   + TransDescr + "\t"
                                                   + CardNumber + "\t"
                                                   + AccNumber + "\t"
                                                   + TransCurr + "\t"
                                                   + TransAmount + "\t"
                                                   + TransDate + "\t"
                                                   + TraceNoWithNoEndZero + "\t"
                                                   + RRNumber + "\t"
                                                   + MatchMask + "\t"
                                                   + RMCateg + "\t"
                                                   + TXNSRC + "\t"
                                                   + TXNDEST + "\t"
                                                   + CAP_DATE;
                            outputFile.WriteLine(strline);


                        }

                        outputFile.Close();
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    RecordFound = false;
                    conn.Close();

                    CatchDetails(ex);

                }
            return CreatedFile;
        }


        // Read Fields In Table 
        private void ReadFieldsInTable2(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OriginFileName = (string)rdr["OriginFileName"];

            OriginalRecordId = (int)rdr["OriginalRecordId"];

            MatchingCateg = (string)rdr["MatchingCateg"];
            RMCateg = (string)rdr["RMCateg"];

            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
            MatchingAtRMCycle = (int)rdr["MatchingAtRMCycle"];

            UniqueRecordId = (int)rdr["UniqueRecordId"];

            Origin = (string)rdr["Origin"];

            TerminalType = (string)rdr["TerminalType"];

            TargetSystem = (int)rdr["TargetSystem"];

            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
            Product = (string)rdr["Product"];
            CostCentre = (string)rdr["CostCentre"];

            TerminalId = (string)rdr["TerminalId"];

            TransType = (int)rdr["TransType"];

            TransDescr = (string)rdr["TransDescr"];

            CardNumber = (string)rdr["CardNumber"];

            IsOwnCard = (bool)rdr["IsOwnCard"];

            AccNumber = (string)rdr["AccNumber"];

            TransCurr = (string)rdr["TransCurr"];
            TransAmount = (decimal)rdr["TransAmount"];
            DepCount = (decimal)rdr["DepCount"];

            TransDate = (DateTime)rdr["TransDate"];

            TraceNoWithNoEndZero = (int)rdr["TraceNoWithNoEndZero"];

            AtmTraceNo = (int)rdr["AtmTraceNo"];
            MasterTraceNo = (int)rdr["MasterTraceNo"];

            RRNumber = (string)rdr["RRNumber"];

            AUTHNUM = (string)rdr["AUTHNUM"];

            FuID = (int)rdr["FuID"];

            ResponseCode = (string)rdr["ResponseCode"];
            SpareField = (string)rdr["SpareField"];
            Comments = (string)rdr["Comments"];

            IsMatchingDone = (bool)rdr["IsMatchingDone"];

            Matched = (bool)rdr["Matched"];
            MatchMask = (string)rdr["MatchMask"];

            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

            MatchedType = (string)rdr["MatchedType"];

            UnMatchedType = (string)rdr["UnMatchedType"];

            MetaExceptionId = (int)rdr["MetaExceptionId"];

            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

            FastTrack = (bool)rdr["FastTrack"];

            ActionByUser = (bool)rdr["ActionByUser"];
            UserId = (string)rdr["UserId"];
            Authoriser = (string)rdr["Authoriser"];

            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

            ActionType = (string)rdr["ActionType"];

            NotInJournal = (bool)rdr["NotInJournal"];
            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

            SettledRecord = (bool)rdr["SettledRecord"];

            Operator = (string)rdr["Operator"];

            FileId01 = (string)rdr["FileId01"];
            SeqNo01 = (int)rdr["SeqNo01"];

            FileId02 = (string)rdr["FileId02"];
            SeqNo02 = (int)rdr["SeqNo02"];

            FileId03 = (string)rdr["FileId03"];
            SeqNo03 = (int)rdr["SeqNo03"];

            FileId04 = (string)rdr["FileId04"];
            SeqNo04 = (int)rdr["SeqNo04"];

            FileId05 = (string)rdr["FileId05"];
            SeqNo05 = (int)rdr["SeqNo05"];

            FileId06 = (string)rdr["FileId06"];
            SeqNo06 = (int)rdr["SeqNo06"];

            ReplCycleNo = (int)rdr["ReplCycleNo"];
            Card_Encrypted = (string)rdr["Card_Encrypted"];
            TXNSRC = (string)rdr["TXNSRC"];
            TXNDEST = (string)rdr["TXNDEST"];

            ACCEPTOR_ID = (string)rdr["ACCEPTOR_ID"];
            ACCEPTORNAME = (string)rdr["ACCEPTORNAME"];

            CAP_DATE = (DateTime)rdr["CAP_DATE"];
            SET_DATE = (DateTime)rdr["SET_DATE"];
            Net_TransDate = (DateTime)rdr["Net_TransDate"];
        }

        // READ POOL And Fill Table General 
        public string ReadFromWorking_4_AndFill_CSV(string InOperator, string InSignedId, int InRMCycle)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int UniqueRecordId;
            string ActionId;
            string Occurance;

            string ActionNm;
            string ActionReason;
            string ActionDateTime;
            string MatchMask;

            string Trans_Descr;
            string Terminal;
            string CardNo;
            string AccNo;

            string Amount;
            string TraceNo;
            string RRNumber;
            string TransDate;

            string Maker;
            string Author;
            string MatchingCateg;
            string MatchingCycle;

            string OriginWorkFlow;
            string RMCateg;
            string ReplCycle;

            string TXNSRC;
            string TXNDEST;
            int SeqNo;
            int RMCycle;

            string UserId;

            TotalSelected = 0;

            string CreatedFile = "C:\\RRDM\\Working\\TAB_File_Actions_This_Cycle" + InRMCycle.ToString() + ".txt";

            //RRDMAccountsClass Acc = new RRDMAccountsClass();
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            SqlString =
             " SELECT * "
            + " FROM [ATMS].[dbo].[WReport55_4] "
            + " where UserId = @UserId and RMCycle = @RMCycle  ";


            StreamWriter outputFile = new StreamWriter(@CreatedFile);

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 
                        //if (InFunction == 28 || InFunction == 29)
                        //{
                        cmd.Parameters.AddWithValue("@UserId", InSignedId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        //}

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            ActionId = (string)rdr["ActionId"];
                            Occurance = (string)rdr["Occurance"];

                            //string ActionNm;
                            //string ActionReason;
                            //string ActionDateTime;
                            //string MatchMask;

                            ActionNm = (string)rdr["ActionNm"];
                            ActionReason = (string)rdr["ActionReason"];
                            ActionDateTime = (string)rdr["ActionDateTime"];
                            MatchMask = (string)rdr["MatchMask"];
                            //string Trans_Descr;
                            //string Terminal;
                            //string CardNo;
                            //string AccNo;

                            Trans_Descr = (string)rdr["Trans_Descr"];
                            Terminal = (string)rdr["Terminal"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            //string Amount;
                            //string TraceNo;
                            //string RRNumber;
                            //string TransDate;

                            Amount = (string)rdr["Amount"];
                            TraceNo = (string)rdr["TraceNo"];
                            RRNumber = (string)rdr["RRNumber"];
                            TransDate = (string)rdr["TransDate"];
                            //string Maker;
                            //string Author;
                            //string MatchingCateg;
                            //string MatchingCycle;

                            Maker = (string)rdr["Maker"];
                            Author = (string)rdr["Author"];
                            MatchingCateg = (string)rdr["MatchingCateg"];
                            MatchingCycle = (string)rdr["MatchingCycle"];


                            //string OriginWorkFlow;
                            //string RMCateg;
                            //string ReplCycle;

                            OriginWorkFlow = (string)rdr["OriginWorkFlow"];
                            RMCateg = (string)rdr["RMCateg"];
                            ReplCycle = (string)rdr["ReplCycle"];

                            //string TXNSRC;
                            //string TXNDEST;
                            //int SeqNo;
                            //int RMCycle;
                            TXNSRC = (string)rdr["TXNSRC"];
                            TXNDEST = (string)rdr["TXNDEST"];
                            SeqNo = (int)rdr["SeqNo"];
                            RMCycle = (int)rdr["RMCycle"];

                            //string UserId;
                            UserId = (string)rdr["UserId"];

                            string strline = "";

                            TotalSelected = TotalSelected + 1;

                            if (TotalSelected == 1)
                            {
                                // Make headings
                                strline = "UniqueRecordId" + "\t"
                                                   + "ActionId" + "\t"
                                                   + "Occurance" + "\t"
                                                   //string ActionNm;
                                                   //string ActionReason;
                                                   //string ActionDateTime;
                                                   //string MatchMask;
                                                   + "ActionNm" + "\t"
                                                   + "ActionReason" + "\t"
                                                   + "ActionDateTime" + "\t"
                                                   + "MatchMask" + "\t"
                                                   //string Trans_Descr;
                                                   //string Terminal;
                                                   //string CardNo;
                                                   //string AccNo;
                                                   + "Trans_Descr" + "\t"
                                                   + "Terminal" + "\t"
                                                   + "CardNo" + "\t"
                                                   + "AccNo" + "\t"
                                                   //string Amount;
                                                   //string TraceNo;
                                                   //string RRNumber;
                                                   //string TransDate;

                                                   + "Amount" + "\t"
                                                   + "TraceNo" + "\t"
                                                   + "RRNumber" + "\t"
                                                    + "TransDate" + "\t"
                                                    //string Maker;
                                                    //string Author;
                                                    //string MatchingCateg;
                                                    //string MatchingCycle;
                                                    + "Maker" + "\t"
                                                   + "Author" + "\t"
                                                   + "MatchingCateg" + "\t"
                                                    + "MatchingCycle" + "\t"

                                                     //string OriginWorkFlow;
                                                     //string RMCateg;
                                                     //string ReplCycle;

                                                     + "OriginWorkFlow" + "\t"
                                                   + "RMCateg" + "\t"
                                                    + "ReplCycle" + "\t"

                                                   //string TXNSRC;
                                                   //string TXNDEST;
                                                   //int SeqNo;
                                                   //int RMCycle;
                                                   + "TXNSRC" + "\t"
                                                  + "TXNDEST" + "\t"
                                                  + "SeqNo" + "\t"
                                                   + "RMCycle" + "\t"
                                                  //string UserId;
                                                  + "UserId";
                                outputFile.WriteLine(strline);

                            }

                            strline = UniqueRecordId + "\t"
                                                  + ActionId + "\t"
                                                   + Occurance + "\t"
                                                   //string ActionNm;
                                                   //string ActionReason;
                                                   //string ActionDateTime;
                                                   //string MatchMask;
                                                   + ActionNm + "\t"
                                                   + ActionReason + "\t"
                                                   + ActionDateTime + "\t"
                                                   + MatchMask + "\t"
                                                   //string Trans_Descr;
                                                   //string Terminal;
                                                   //string CardNo;
                                                   //string AccNo;
                                                   + Trans_Descr + "\t"
                                                   + Terminal + "\t"
                                                   + CardNo + "\t"
                                                   + AccNo + "\t"
                                                   //string Amount;
                                                   //string TraceNo;
                                                   //string RRNumber;
                                                   //string TransDate;

                                                   + Amount + "\t"
                                                   + TraceNo + "\t"
                                                   + RRNumber + "\t"
                                                    + TransDate + "\t"
                                                    //string Maker;
                                                    //string Author;
                                                    //string MatchingCateg;
                                                    //string MatchingCycle;
                                                    + Maker + "\t"
                                                   + Author + "\t"
                                                   + MatchingCateg + "\t"
                                                    + MatchingCycle + "\t"

                                                    //string OriginWorkFlow;
                                                    //string RMCateg;
                                                    //string ReplCycle;

                                                    + OriginWorkFlow + "\t"
                                                    + RMCateg + "\t"
                                                    + ReplCycle + "\t"

                                                   //string TXNSRC;
                                                   //string TXNDEST;
                                                   //int SeqNo;
                                                   //int RMCycle;
                                                   + TXNSRC + "\t"
                                                  + TXNDEST + "\t"
                                                  + SeqNo + "\t"
                                                   + RMCycle + "\t"
                                                  //string UserId;
                                                  + UserId;
                            outputFile.WriteLine(strline);


                        }

                        outputFile.Close();
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    RecordFound = false;
                    conn.Close();

                    CatchDetails(ex);

                }
            return CreatedFile;
        }




        //
        // Insert  General
        //
        private void InsertReportGeneral(string InOperator, string InSignedId, int InMode)
        {
            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                ////ReadTable And Insert In Sql Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport55_2(InSignedId);
                int I = 0;
                try
                {
                    while (I <= (MatchingMasterDataTableATMs.Rows.Count - 1))
                    {

                        RecordFound = true;

                        UniqueRecordId = Tr.RecordId = (int)MatchingMasterDataTableATMs.Rows[I]["RecordId"];
                        if (UniqueRecordId == 120293)
                        {
                            UniqueRecordId = 120293;
                        }
                        //Tr.Select = (bool)MatchingMasterDataTableATMs.Rows[I]["Select"];
                        Tr.MatchMask = (string)MatchingMasterDataTableATMs.Rows[I]["MatchMask"];

                        Tr.ActionType = (string)MatchingMasterDataTableATMs.Rows[I]["ActionType"];
                        Tr.ActionDesc = (string)MatchingMasterDataTableATMs.Rows[I]["ActionDesc"];
                        Tr.Settled = (bool)MatchingMasterDataTableATMs.Rows[I]["Settled"];

                        Tr.Date = (DateTime)MatchingMasterDataTableATMs.Rows[I]["Date"];
                        Tr.Descr = (string)MatchingMasterDataTableATMs.Rows[I]["Descr"];
                        Tr.Ccy = (string)MatchingMasterDataTableATMs.Rows[I]["Ccy"];
                        Tr.Amount = (decimal)MatchingMasterDataTableATMs.Rows[I]["Amount"];

                        Tr.TraceNo = (int)MatchingMasterDataTableATMs.Rows[I]["TraceNo"];
                        Tr.RRNumber = (string)MatchingMasterDataTableATMs.Rows[I]["RRNumber"];

                        Tr.Exception = (int)MatchingMasterDataTableATMs.Rows[I]["Exception"];

                        Tr.Card = (string)MatchingMasterDataTableATMs.Rows[I]["Card"];

                        Tr.Account = (string)MatchingMasterDataTableATMs.Rows[I]["Account"];

                        Tr.MatchingCateg = (string)MatchingMasterDataTableATMs.Rows[I]["MatchingCateg"];
                        Tr.RMCateg = (string)MatchingMasterDataTableATMs.Rows[I]["RMCateg"];

                        Tr.TXNSRC = (string)MatchingMasterDataTableATMs.Rows[I]["TXNSRC"];
                        Tr.TXNDEST = (string)MatchingMasterDataTableATMs.Rows[I]["TXNDEST"];


                        Tr.ATMNo = (string)MatchingMasterDataTableATMs.Rows[I]["ATMNo"];

                        Tr.CAP_DATE = (DateTime)MatchingMasterDataTableATMs.Rows[I]["CAP_DATE"];

                        Tr.SET_DATE = (DateTime)MatchingMasterDataTableATMs.Rows[I]["SET_DATE"];

                        // Insert record for printing 
                        //
                        Tr.InsertReport55_2(InSignedId);

                        I++; // Read Next entry of the table 

                    }

                }
                catch (Exception ex)
                {
                    CatchDetails(ex);
                }

            }
        }



        // Insert  FAST TRACK
        private void InsertReportFastTrack(string InOperator, string InSignedId, int InMode)
        {
            if (InMode == 1 || InMode == 2 || InMode == 5)
            {
                ////ReadTable And Insert In Sql Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport55(InSignedId);

                int I = 0;

                while (I <= (MatchingMasterDataTableATMs.Rows.Count - 1))
                {

                    RecordFound = true;

                    UniqueRecordId = Tr.RecordId = (int)MatchingMasterDataTableATMs.Rows[I]["RecordId"];

                    Tr.Select = (bool)MatchingMasterDataTableATMs.Rows[I]["Select"];
                    Tr.MatchMask = (string)MatchingMasterDataTableATMs.Rows[I]["MatchMask"];

                    Tr.ActionType = (string)MatchingMasterDataTableATMs.Rows[I]["ActionType"];
                    Tr.ActionDesc = (string)MatchingMasterDataTableATMs.Rows[I]["ActionDesc"];
                    Tr.Settled = (bool)MatchingMasterDataTableATMs.Rows[I]["Settled"];

                    Tr.Date = (DateTime)MatchingMasterDataTableATMs.Rows[I]["Date"];
                    Tr.Descr = (string)MatchingMasterDataTableATMs.Rows[I]["Descr"];
                    Tr.Ccy = (string)MatchingMasterDataTableATMs.Rows[I]["Ccy"];
                    Tr.Amount = (decimal)MatchingMasterDataTableATMs.Rows[I]["Amount"];

                    Tr.TraceNo = (int)MatchingMasterDataTableATMs.Rows[I]["TraceNo"];
                    Tr.RRNumber = (string)MatchingMasterDataTableATMs.Rows[I]["RRNumber"];

                    Tr.Exception = (int)MatchingMasterDataTableATMs.Rows[I]["Exception"];

                    Tr.Card = (string)MatchingMasterDataTableATMs.Rows[I]["Card"];

                    Tr.Account = (string)MatchingMasterDataTableATMs.Rows[I]["Account"];

                    Tr.MatchingCateg = (string)MatchingMasterDataTableATMs.Rows[I]["MatchingCateg"];
                    Tr.RMCateg = (string)MatchingMasterDataTableATMs.Rows[I]["RMCateg"];

                    Tr.ATMNo = (string)MatchingMasterDataTableATMs.Rows[I]["ATMNo"];

                    Tr.FirstEntry = (string)MatchingMasterDataTableATMs.Rows[I]["FirstEntry"];
                    Tr.FirstEntryAccno = (string)MatchingMasterDataTableATMs.Rows[I]["FirstEntryAccno"];
                    Tr.SecondEntry = (string)MatchingMasterDataTableATMs.Rows[I]["SecondEntry"];
                    Tr.SecondEntryAccno = (string)MatchingMasterDataTableATMs.Rows[I]["SecondEntryAccno"];

                    // Insert record for printing 
                    //
                    Tr.InsertReport55(InSignedId);

                    I++; // Read Next entry of the table 

                }

            }
        }


        // UpdateExceptionIds
        public void UpdateExceptionIds(string InOperator, string InSignedId, int InMode, string InSelectionCriteria,
                                          string InSortCriteria, DateTime InFromDt, DateTime InToDt, int In_DB_Mode)
        {
            // The below code is Outdated
            // We do not use exception IDs now.
            // Based on Actions we act accordingly
            return;
            // InMode = 2 = Updating
            if (InMode == 1 || InMode == 2 || InMode == 5)
            {

                MatchingMasterDataTableATMs = new DataTable();
                MatchingMasterDataTableATMs.Clear();
                TotalSelected = 0;

                RecordFound = false;

                if (In_DB_Mode == 1)
                {
                    SqlString = "SELECT *"
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                      + " "
                      + InSelectionCriteria
                      + " "
                      + InSortCriteria;
                }

                if (In_DB_Mode == 2)
                {
                    SqlString =
                       " WITH MergedTbl AS "
                     + " ( "
                     + " SELECT *  "
                     + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                  + InSelectionCriteria
                    + " UNION ALL  "
                     + " SELECT * "
                     + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + InSelectionCriteria
                     + " ) "
                     + " SELECT * FROM MergedTbl "
                     + InSortCriteria
                    + "  ";
                }

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                            //Create a datatable that will be filled with the data retrieved from the command

                            sqlAdapt.Fill(MatchingMasterDataTableATMs);

                            // Close conn
                            conn.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                int I = 0;

                while (I <= (MatchingMasterDataTableATMs.Rows.Count - 1))
                {

                    RecordFound = true;

                    UniqueRecordId = (int)MatchingMasterDataTableATMs.Rows[I]["UniqueRecordId"];

                    MatchMask = (string)MatchingMasterDataTableATMs.Rows[I]["MatchMask"];

                    TerminalId = (string)MatchingMasterDataTableATMs.Rows[I]["TerminalId"];

                    //Partial Updating ... so read to get all fields 
                    string WSelectionCriteria = " WHERE  UniqueRecordId =" + UniqueRecordId;
                    ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria, 1);

                    if (MatchMask == "AA"
                        || MatchMask == "AAA"
                         || MatchMask == "0AA"
                          || MatchMask == "AA0"
                        )
                    {
                        // Skip
                    }
                    else
                    {
                        // UPDATE META EXCEPTION NUMBER 
                        if (InMode == 2 & ActionType == "00")
                        {
                            if (TransType < 20)
                            {
                                Rme.ReadMatchingMaskRecordbyMaskId(InOperator, MatchingCateg, MatchMask, 11);
                            }
                            if (TransType > 20)
                            {
                                Rme.ReadMatchingMaskRecordbyMaskId(InOperator, MatchingCateg, MatchMask, 21);
                            }


                            if (Rme.RecordFound)
                            {
                                UnMatchedType = Rme.MaskName;

                                MetaExceptionId = Rme.MetaExceptionId;
                            }
                            else
                            {
                                UnMatchedType = "Not Specified";
                                MetaExceptionId = 0;
                            }

                            UpdateMatchingTxnsMasterPoolATMsFooter(InOperator, UniqueRecordId, 1);

                        }
                        //
                        // UPDATE META EXCEPTION NUMBER and Action Type = 7 = default 
                        //
                        if (InMode == 5 & ActionType == "00")
                        {
                            Rme.ReadMatchingMaskRecordbyMaskId(InOperator, MatchingCateg, MatchMask, TransType);

                            if (Rme.RecordFound)
                            {
                                UnMatchedType = Rme.MaskName;
                                MetaExceptionId = Rme.MetaExceptionId;

                                //DefaultAction = false;
                                Er.ReadErrorsIDRecord(Rme.MetaExceptionId, InOperator);

                                if (Er.RecordFound)
                                {
                                    if (Er.TurboReconc == true)
                                    {
                                        //  DefaultAction = true;
                                        ActionType = "07";
                                    }
                                    else ActionType = "00";
                                }
                            }
                            else
                            {
                                UnMatchedType = "Not Specified";
                                MetaExceptionId = 0;
                            }

                            UpdateMatchingTxnsMasterPoolATMsFooter(InOperator, UniqueRecordId, 1);

                        }
                    }


                    I++; // Read Next entry of the table 

                }

            }
        }

        //
        // UPDATE Mpa with IsMatchingDone with ATM 
        // 
        //public void UpdateMpaRecordsAsProcessed_With_ATM(string InMatchingCateg, int InAtmTraceNo,
        //                string InTerminalId, int InMatchingAtRMCycle, string InMatchMask, DateTime InLastDateTime)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    //int rows;

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {

        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE "
        //                           + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //                           + " SET "
        //                           + " [IsMatchingDone] = 1 , "
        //                           + " [SystemMatchingDtTm] = @SystemMatchingDtTm ,  "
        //                           + " [MatchingAtRMCycle] = @MatchingAtRMCycle ,"
        //                           + " [Matched] = 1 ,"
        //                           + " [MatchMask] = @MatchMask ,"
        //                           + " [SettledRecord] = 1 "
        //                + " WHERE MatchingCateg = @MatchingCateg "
        //                + " AND TraceNoWithNoEndZero <= @TraceNoWithNoEndZero "
        //                + " AND TerminalId = @TerminalId AND TransDate <= @TransDate AND IsMatchingDone = 0 ", conn))
        //            {

        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InAtmTraceNo);
        //                cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
        //                cmd.Parameters.AddWithValue("@SystemMatchingDtTm", DateTime.Now);
        //                cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
        //                cmd.Parameters.AddWithValue("@MatchMask", InMatchMask);
        //                cmd.Parameters.AddWithValue("@TransDate", InLastDateTime); // THIS IS NEEDED..   
        //                                                                           // We want to cover the small traces when Traces counter start from 1 

        //                //rows number of record got updated

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
        // UPDATE Mpa with IsMatchingDone with ATM 
        // 
        public void UpdateMpaRecordsAsProcessed_With_ATM_V02(string InMatchingCateg,
                        string InTerminalId, int InMatchingAtRMCycle, string InMatchMask, DateTime InMinMaxDt, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [IsMatchingDone] = 1 , "
                                       + " [SystemMatchingDtTm] = @SystemMatchingDtTm ,  "
                                       + " [MatchingAtRMCycle] = @MatchingAtRMCycle ,"
                                       + " [Matched] = 1 ,"
                                       + " [MatchMask] = @MatchMask ,"
                                       + " [SettledRecord] = 1 "
                            + " WHERE "
                            + "     ([IsMatchingDone] = 0) "
                            + " AND ([MatchingCateg] = @MatchingCateg) "
                            + " AND (TerminalId = @TerminalId)"
                            + " AND (ResponseCode = '0')"
                            + " AND (TransDate <= @TransDate) ", conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            //cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InAtmTraceNo);
                            cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                            cmd.Parameters.AddWithValue("@MatchMask", InMatchMask);
                            cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt); // THIS IS NEEDED..   
                                                                                   // We want to cover the small traces when Traces counter start from 1 

                            //rows number of record got updated

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
        }

        //
        // UPDATE outstanding Mpa with new group id  
        // 
        public void UpdateMpaRecordsWithNewGroupNumber(string InTerminalId, int InGroupNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string WRMCateg = "RECATMS-" + InGroupNo.ToString();

            int rows;

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE "
                                   + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                   + " SET "
                                   + " [RMCateg] = @RMCateg "
                        + " WHERE "
                        + " (IsMatchingDone = 0) "
                        + " AND (TerminalId = @TerminalId)"
                        + "  ", conn))
                    {

                        //WHERE TerminalId = '00000530' and IsMatchingDone = 0 and RMCateg = 'RECATMS-108'                        
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        cmd.Parameters.AddWithValue("@RMCateg", WRMCateg);

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
        // UPDATE Mpa with IsMatchingDone 
        // 
        public void UpdateMpaRecordsAsProcessed_NO_ATM(string InMatchingCateg, DateTime InTransDate,
                                            int InMatchingAtRMCycle, string InMatchMask, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [IsMatchingDone] = 1 , "
                                       + " [SystemMatchingDtTm] = @SystemMatchingDtTm ,  "
                                       + " [MatchingAtRMCycle] = @MatchingAtRMCycle ,"
                                       + " [Matched] = 1 ,"
                                       + " [MatchMask] = @MatchMask ,"
                                       + " [SettledRecord] = 1 "
                            + " WHERE MatchingCateg = @MatchingCateg "
                            + " AND TransDate <= @TransDate "
                            + " AND IsMatchingDone = 0 ", conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                            cmd.Parameters.AddWithValue("@MatchMask", InMatchMask);

                            //rows number of record got updated

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
        }

        // UPDATE Mpa with IsMatchingDone 
        // 
        public void UpdateMpaRecordsAsProcessed_NO_ATM_No_Date(string InMatchingCateg, DateTime InTransDate,
                                            int InMatchingAtRMCycle, string InMatchMask, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [IsMatchingDone] = 1 , "
                                       + " [SystemMatchingDtTm] = @SystemMatchingDtTm ,  "
                                       + " [MatchingAtRMCycle] = @MatchingAtRMCycle ,"
                                       + " [Matched] = 1 ,"
                                       + " [MatchMask] = @MatchMask ,"
                                       + " [SettledRecord] = 1 "
                            + " WHERE MatchingCateg = @MatchingCateg "
                            //+ " AND TransDate <= @TransDate "
                            + " AND IsMatchingDone = 0 ", conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            //cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                            cmd.Parameters.AddWithValue("@MatchMask", InMatchMask);

                            //rows number of record got updated
                            cmd.CommandTimeout = 200;
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
        }
        //
        // UPDATE Mpa with IsMatchingDone 
        // 
        //public string SpareField; // USED to keep the partner SeqNo

        //public string Comments;
        public void UpdateMpaRecordsWithPartnerDetails_POS(int InSeqNo, string InAccNumber, string InPartnerSeqNo,
                                                decimal InCH_Amount, string InComments, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [AccNumber] = @AccNumber "
                                       + " ,[SpareField] = @SpareField "
                                       + " ,[DepCount] = @DepCount " // Save the Charged Amount for rate 
                                       + " ,[Comments] = @Comments "
                            + " WHERE SeqNo = @SeqNo "
                            + "  ", conn))
                        {

                            cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                            cmd.Parameters.AddWithValue("@AccNumber", InAccNumber);
                            cmd.Parameters.AddWithValue("@SpareField", InPartnerSeqNo);
                            cmd.Parameters.AddWithValue("@DepCount", InCH_Amount);
                            cmd.Parameters.AddWithValue("@Comments", InComments);

                            //rows number of record got updated

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
        }

        public void UpdateMpaRecordsAsProcessed_NO_ATM_But_POS(string InMatchingCateg,
                                            int InMatchingAtRMCycle, string InMatchMask, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [IsMatchingDone] = 1 , "
                                       + " [SystemMatchingDtTm] = @SystemMatchingDtTm ,  "
                                       + " [MatchingAtRMCycle] = @MatchingAtRMCycle ,"
                                       + " [Matched] = 1 ,"
                                       + " [MatchMask] = @MatchMask ,"
                                       + " [SettledRecord] = 1 "
                            + " WHERE MatchingCateg = @MatchingCateg "
                            // + " AND TransDate <= @TransDate "
                            //+ " AND (ResponseCode = '0' OR ResponseCode = '112' OR ResponseCode = '200000' )"
                            + " AND IsMatchingDone = 0 ", conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", DateTime.Now);
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InMatchingAtRMCycle);
                            cmd.Parameters.AddWithValue("@MatchMask", InMatchMask);

                            //rows number of record got updated

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
        }

        //
        // UPDATE Mpa with ReplCycle 
        // 
        public void UpdateMpaRecordsWithReplCycle(string InOperator, string InSignedId
                                          , string InAtmNo, DateTime InDtFrom, DateTime InDtTo
                                                     , int InSesNo, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [ReplCycleNo] = @ReplCycleNo "
                            + " WHERE TerminalId = @TerminalId  AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                            + " AND NotInJournal = 0  " // We only update the ones which are valid in journal 
                            + " AND ResponseCode = '0' AND TXNSRC = '1' AND(TransType = 11 OR TransType = 23) "
                            + " AND Origin = 'Our Atms' "
                            , conn))
                        {

                            cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);

                            cmd.Parameters.AddWithValue("@DateFrom", InDtFrom);

                            cmd.Parameters.AddWithValue("@DateTo", InDtTo);

                            cmd.Parameters.AddWithValue("@ReplCycleNo", InSesNo);


                            //rows number of record got updated
                            cmd.CommandTimeout = 550;
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



            if (In_DB_Mode == 2)
            {
                // UPDATE FIRST TABLE
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [ReplCycleNo] = @ReplCycleNo "
                            + " WHERE TerminalId = @TerminalId  AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                            + " AND NotInJournal = 0  AND ResponseCode = '0' AND TXNSRC = '1' AND(TransType = 11 OR TransType = 23) "
                            + " AND Origin = 'Our Atms' "
                            , conn))
                        {

                            cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);

                            cmd.Parameters.AddWithValue("@DateFrom", InDtFrom);

                            cmd.Parameters.AddWithValue("@DateTo", InDtTo);

                            cmd.Parameters.AddWithValue("@ReplCycleNo", InSesNo);


                            //rows number of record got updated
                            cmd.CommandTimeout = 550;
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

                // UPDATE SECOND TABLE
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_Matched_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [ReplCycleNo] = @ReplCycleNo "
                            + " WHERE TerminalId = @TerminalId  AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                            + " AND NotInJournal = 0  AND ResponseCode = '0' AND TXNSRC = '1' AND(TransType = 11 OR TransType = 23) "
                            + " AND Origin = 'Our Atms' "
                            , conn))
                        {

                            cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);

                            cmd.Parameters.AddWithValue("@DateFrom", InDtFrom);

                            cmd.Parameters.AddWithValue("@DateTo", InDtTo);

                            cmd.Parameters.AddWithValue("@ReplCycleNo", InSesNo);


                            //rows number of record got updated
                            cmd.CommandTimeout = 550;
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
        }
        //
        // UPDATE Mpa with Action Type
        // 
        public void UpdateMpaRecordsWithActionType(int InUniqueRecordId, string InActionType
                                          , string InComments, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;


            if (In_DB_Mode == 2)
            {
                // UPDATE FIRST TABLE
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                       + " [ActionType] = @ActionType "
                                       + " ,[Comments] = @Comments "
                            + " WHERE UniqueRecordId = @UniqueRecordId "
                            , conn))
                        {

                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@ActionType", InActionType);
                            cmd.Parameters.AddWithValue("@Comments", InComments);

                            //rows number of record got updated

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

                // UPDATE SECOND TABLE
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE "
                                       + " [RRDM_Reconciliation_Matched_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                       + " SET "
                                        + " [ActionType] = @ActionType "
                                       + " ,[Comments] = @Comments "
                            + " WHERE UniqueRecordId = @UniqueRecordId "
                            , conn))
                        {

                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@ActionType", InActionType);
                            cmd.Parameters.AddWithValue("@Comments", InComments);

                            //rows number of record got updated

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
        }
        //
        // Methods 
        // READ TOTALs
        // FILL UP A TABLE
        //

        public int TotalForcedMatchedAndInJournal;
        public decimal TotalForcedMatchedAmountAndInJournal;

        public int TotalMoveToPool;
        public decimal TotalMoveToPoolAmount;

        public int TotalMoveToDisputeNumber;
        public decimal TotalMoveToDisputeAmt;

        public int TotalMoveToSuspense;
        public decimal TotalMoveToSuspenseAmt;

        public int TotalPresenterWithAction;
        public decimal TotalPresenterWithActionAmt;

        public void ReadMatchingTxnsMasterPoolATMsTotals(string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalMatched = 0;
            TotalAmountMatched = 0;

            TotalUnMatched = 0;
            TotalAmountUnMatched = 0;

            TotalNotSettled = 0;
            TotalNotSettledAmt = 0;

            TotalDefaultActionBySystem = 0;
            TotalAmountDefaultActionBySystem = 0;

            TotalActionsByUserDefaultAndManual = 0;
            TotalAmountByUserDefaultAndManual = 0;

            TotalForcedMatched = 0;
            TotalForcedMatchedAmount = 0;

            TotalForcedMatchedAndInJournal = 0;
            TotalForcedMatchedAmountAndInJournal = 0;

            TotalMoveToPool = 0;
            TotalMoveToPoolAmount = 0;

            TotalMoveToDisputeNumber = 0;
            TotalMoveToDisputeAmt = 0;

            TotalMoveToSuspense = 0;
            TotalMoveToSuspenseAmt = 0;

            TotalFastTrack = 0;
            TotalFastTrackAmount = 0;

            TotalPresenterWithAction = 0;
            TotalPresenterWithActionAmt = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                   + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + InSelectionCriteria
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionCriteria
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

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
                        cmd.CommandTimeout = 75;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            // Update Totals 
                            if (Matched == true)
                            {
                                TotalMatched = TotalMatched + 1;
                                TotalAmountMatched = TotalAmountMatched + TransAmount;

                                if (ActionType != "00" & MetaExceptionId == 55)
                                {
                                    TotalPresenterWithAction = TotalPresenterWithAction + 1;
                                    TotalPresenterWithActionAmt = TotalPresenterWithActionAmt + TransAmount;
                                }
                            }
                            if (Matched == false) // UnMatched 
                            {
                                if (ActionType == "07" & SettledRecord == true)
                                {
                                    TotalDefaultActionBySystem = TotalDefaultActionBySystem + 1;
                                    TotalAmountDefaultActionBySystem = TotalAmountDefaultActionBySystem + TransAmount;
                                }
                                TotalUnMatched = TotalUnMatched + 1;
                                TotalAmountUnMatched = TotalAmountUnMatched + TransAmount;
                                if (SettledRecord == false)
                                {
                                    // ACTION not taken yet 
                                    TotalNotSettled = TotalNotSettled + 1;
                                    TotalNotSettledAmt = TotalNotSettledAmt + TransAmount;
                                }
                                if (ActionType != "00" & ActionType != "07" & ActionType != "04" & ActionType != "05"
                                    & ActionType != "06" & ActionType != "09")
                                {
                                    TotalActionsByUserDefaultAndManual = TotalActionsByUserDefaultAndManual + 1;
                                    TotalAmountByUserDefaultAndManual = TotalAmountByUserDefaultAndManual + TransAmount;
                                }
                                if (ActionType == "04")
                                {
                                    TotalForcedMatched = TotalForcedMatched + 1;
                                    TotalForcedMatchedAmount = TotalForcedMatchedAmount + TransAmount;
                                }
                                if (ActionType == "04" & NotInJournal == false)
                                {
                                    TotalForcedMatchedAndInJournal = TotalForcedMatchedAndInJournal + 1;
                                    TotalForcedMatchedAmountAndInJournal = TotalForcedMatchedAmountAndInJournal + TransAmount;
                                }

                                if (ActionType == "05")
                                {
                                    TotalMoveToDisputeNumber = TotalMoveToDisputeNumber + 1;
                                    TotalMoveToDisputeAmt = TotalMoveToDisputeAmt + TransAmount;
                                }

                                if (ActionType == "06")
                                {
                                    TotalMoveToPool = TotalMoveToPool + 1;
                                    TotalMoveToPoolAmount = TotalMoveToPoolAmount + TransAmount;
                                }

                                if (ActionType == "09")
                                {
                                    TotalMoveToSuspense = TotalMoveToSuspense + 1;
                                    TotalMoveToSuspenseAmt = TotalMoveToSuspenseAmt + TransAmount;
                                }

                                if (FastTrack == true)
                                {
                                    TotalFastTrack = TotalFastTrack + 1;
                                    TotalFastTrackAmount = TotalFastTrackAmount + TransAmount;
                                }



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
        // Methods 
        // READ TOTALs FOR View UnMatched
        // FILL UP A TABLE
        //
        public int TotalUnMatchedWithNoAction;
        public int TotalUnMatchedInProcess;
        public int TotalUnMatchedSettled;

        public void ReadUnMatchedTxnsMasterPoolATMsTotals(string InSelectionString, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatched = 0;
            TotalUnMatchedWithNoAction = 0;
            TotalUnMatchedInProcess = 0;
            TotalUnMatchedSettled = 0;
            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                   + InSelectionString;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
             + InSelectionString
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + InSelectionString
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                 + "  ";
            }

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

                            ReadFieldsInTable(rdr);

                            // Update Totals 

                            TotalUnMatched = TotalUnMatched + 1;

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


        // Methods 
        // READ And 
        // FILL UP A TABLE
        //
        public void ReadMatchingDiscrepanciesFillTableByATM_RMCycle(string InMatchingCateg, int InRMCycle, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFullFromMaster = new DataTable();
            TableFullFromMaster.Clear();

            if (In_DB_Mode == 1)
            {
                // DATA TABLE ROWS DEFINITION 
                if (InMode == 2)
                {
                    SqlString = "SELECT TerminalId, count(*) as Discrepancies "
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                       + " where Origin = 'Our Atms'and MatchingAtRMCycle = @MatchingAtRMCycle and Matched = 0 AND ResponseCode = '0'  "
                       + " Group by TerminalId Order by TerminalId   ";
                }
                if (InMode == 3)
                {
                    SqlString = "SELECT * "
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                       + " where RMCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle and Matched = 0 AND ResponseCode = '0'  "
                       + " Order by TerminalId, TransDate   ";
                }
                if (InMode == 4)
                {
                    SqlString = "SELECT * "
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                       + " where RMCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle and Matched = 1  AND ResponseCode = '0' "
                       + " Order by TerminalId, TransDate   ";
                }

            }

            if (In_DB_Mode == 2 & InMode == 4)
            {
                SqlString =
                   " WITH MergedTbl AS "
                 + " ( "
                 + " SELECT *  "
                 + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " where RMCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle and Matched = 1  AND ResponseCode = '0' "
                + " UNION ALL  "
                 + " SELECT * "
                 + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                 + " where RMCateg = @MatchingCateg AND MatchingAtRMCycle = @MatchingAtRMCycle and Matched = 1 AND ResponseCode = '0'  "
                 + " ) "
                 + " SELECT * FROM MergedTbl "
                  + " Order by TerminalId, TransDate   "
                + "  ";
            }


            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);
                        if (InMode == 3 || InMode == 4)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 190;   // seconds
                        sqlAdapt.Fill(TableFullFromMaster);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //
            if (InMode != 2)
            {
                string DestinationFile =
              "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

                TruncateFile(DestinationFile);
                //
                InsertReportMatched(DestinationFile, InRMCycle, TableFullFromMaster);
            }


        }


        // Methods 
        // READ And 
        // FILL UP A TABLE
        //
        public void ReadMatchingDiscrepanciesFillTableByATM_CutoffDate(int InRMCycle, DateTime InCutoffdate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFullFromMaster = new DataTable();
            TableFullFromMaster.Clear();

            //" SELECT * , " + MatchingString + " As WMatchingString "
            //+ " FROM " + InFileIdA + " c1"
            //+ " WHERE NOT EXISTS (SELECT * "
            //+ " FROM " + InFileIdB + " c2 "
            //+ " WHERE " + InMatchingFields + ")";
            SqlString = "SELECT TerminalId,TransDate ,Net_TransDate, TransAmount, TraceNoWithNoEndZero"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] c1 "
                       + " WHERE Net_TransDate = @Net_TransDate and TXNSRC = '1' AND LEFT(TransDescr, 5) <> 'FOREX' "
                       + " AND NOT EXISTS ( Select TerminalId, Net_TransDate, TransAmt, TraceNo   "
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] c2 "
                       + " WHERE c1.TerminalId = c2.TerminalId"
                       + " AND c1.Net_TransDate = c2.Net_TransDate"
                       + " AND c1.TransAmount = c2.TransAmt "
                       + " AND c1.TraceNoWithNoEndZero = c2.TraceNo "
                       + "  ) "
                       + " Order by TerminalId, TransDate   ";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Net_TransDate", InCutoffdate);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 190;   // seconds
                        sqlAdapt.Fill(TableFullFromMaster);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //


        }

        // 
        // 
        // Read Transactions that never reach RRDM yet 
        //
        public void ReadNotLoadedYet(int InRMCycle, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFullFromMaster = new DataTable();
            TableFullFromMaster.Clear();

            if (In_DB_Mode == 1)
            {
                // DATA TABLE ROWS DEFINITION 

                SqlString =
                   " SELECT * "
                   + " FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                   + " WHERE Origin = 'Our Atms' AND MatchedType = 'Force Matching Due to GAP JOURNAL' AND Comments = '' "
                   + " AND MatchingAtRMCycle > @MatchingAtRMCycle "
                   + "  ORDER By MatchingAtRMCycle,TerminalId, TransDate ";

            }


            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 180;   // seconds
                        sqlAdapt.Fill(TableFullFromMaster);

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

        // Insert 
        public void InsertReportMatched(string InDestinationFile, int InReplCycleNo, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = InDestinationFile;

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.BulkCopyTimeout = 350;

                            s.WriteToServer(InTable);

                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }

        // Truncate file 
        private void TruncateFile(string InTable)
        {
            // Truncate  

            string SQLCmd = "TRUNCATE TABLE " + InTable;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
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
        //
        // UPDATE  SettledRecord for Force Matched 
        // 
        public void UpdateMatchingTxnsMasterPoolATMsForcedMatched(string InOperator, string InSelectionCriteria, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows;
            if (In_DB_Mode == 1 || In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + "  ActionType = @ActionType, "
                                + " [ActionByUser] = @ActionByUser,"
                                + " [UserId] = @UserId,"
                                + " [Authoriser] = @Authoriser,"
                                + " [AuthoriserDtTm] = @AuthoriserDtTm,"
                                + " [SettledRecord] = @SettledRecord, "
                                + " [MetaExceptionId] = @MetaExceptionId "
                                + InSelectionCriteria, conn))
                        {
                            cmd.Parameters.AddWithValue("@ActionType", ActionType);
                            cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                            cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                            cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                            
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
            // UPDATE RECORD IN CASE WAS MOVED TO MATCHED TXNS 
            //
            if (In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + "  ActionType = @ActionType, "
                                + " [ActionByUser] = @ActionByUser,"
                                + " [UserId] = @UserId,"
                                + " [Authoriser] = @Authoriser,"
                                + " [AuthoriserDtTm] = @AuthoriserDtTm,"
                                + " [SettledRecord] = @SettledRecord, "
                                + " [MetaExceptionId] = @MetaExceptionId "
                                + InSelectionCriteria, conn))
                        {
                            cmd.Parameters.AddWithValue("@ActionType", ActionType);
                            cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);
                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                            cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                            cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);
                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
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


        //
        //
        // UPDATE  SettledRecord for Force Matched 
        // 
        public void UpdateMatchingTxnsMasterPoolATMs_SOLO_ACTION(string InOperator, string InSelectionCriteria, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int count = 0; 

            //int rows;
            if (In_DB_Mode == 1 || In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        //Mpa.SeqNo06 = 95;
                        //Mpa.SeqNo05 = WReconcCycleNo;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [SeqNo05] = @SeqNo05,"
                                + " [SeqNo06] = @SeqNo06 "
                                + InSelectionCriteria, conn))
                        {
                            cmd.Parameters.AddWithValue("@SeqNo05", SeqNo05);
                            cmd.Parameters.AddWithValue("@SeqNo06", SeqNo06);

                            //rows number of record got updated

                            count = cmd.ExecuteNonQuery();

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
            // UPDATE RECORD IN CASE WAS MOVED TO MATCHED TXNS 
            //
            if (In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                 + " [SeqNo05] = @SeqNo05,"
                                + " [SeqNo06] = @SeqNo06 "
                                + InSelectionCriteria, conn))
                        {
                            cmd.Parameters.AddWithValue("@SeqNo05", SeqNo05);
                            cmd.Parameters.AddWithValue("@SeqNo06", SeqNo06);

                            //rows number of record got updated

                            count = cmd.ExecuteNonQuery();

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


        //
        //
        // UPDATE  New Repl Cycle for those with zero 
        //
        // THIS IS CANCELLED
        // 
        public void UpdateZeroReplCycleWithNewReplCycle(string InAtmNo, int InReplCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " SET "
                            + " [ReplCycleNo] = @ReplCycleNo "
                            + " Where TerminalId = @TerminalId and ReplCycleNo = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        //rows number of record got updated
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
        //
        // UPDATE  
        // 
        public void UpdateMatchingTxnsMasterPoolATMsComments(string InSelectionCriteria, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [Comments] = @Comments "
                                + " ,[UnMatchedType] = @UnMatchedType "
                                + InSelectionCriteria, conn))
                        {
                            cmd.Parameters.AddWithValue("@Comments", Comments);
                            cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

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

        //
        // UPDATE  Trans Currency 
        // 
        public void UpdateMatchingTxnsMasterPoolATMsCurrency(string WOperator)
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
                            "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " SET "
                            + "  [TransCurr] = '818' "
                            + "  WHERE [TransCurr] = 'EGP' ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@Comments", Comments);
                        //cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

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
        // UPDATE  RECORDS Footer 
        // 

        public void UpdateMatchingTxnsMasterPoolATMsFooter(string InOperator, int InUniqueRecordId, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            if (In_DB_Mode == 1 || In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [IsMatchingDone] = @IsMatchingDone,[AtmTraceNo] = @AtmTraceNo,"
                                + " [Matched] = @Matched, [MatchMask] = @MatchMask,"
                                + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                                + " [MetaExceptionId] = @MetaExceptionId, [MetaExceptionNo] = @MetaExceptionNo, "
                                + " [FastTrack] = @FastTrack,"
                                 + " [Comments] = @Comments,"
                                + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                                + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                                + " [NotInJournal] = @NotInJournal, [WaitingForUpdating] = @WaitingForUpdating, "
                                + " [SettledRecord] = @SettledRecord "
                                + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@IsMatchingDone", IsMatchingDone);

                            cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                            cmd.Parameters.AddWithValue("@Matched", Matched);
                            cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                            cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                            cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                            cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);
                            cmd.Parameters.AddWithValue("@Comments", Comments);

                            cmd.Parameters.AddWithValue("@FastTrack", FastTrack);
                            cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                            cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                            cmd.Parameters.AddWithValue("@ActionType", ActionType);

                            cmd.Parameters.AddWithValue("@NotInJournal", NotInJournal);
                            cmd.Parameters.AddWithValue("@WaitingForUpdating", WaitingForUpdating);

                            cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                            //rows number of record got updated
                            cmd.CommandTimeout = 100;
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
            if (In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [IsMatchingDone] = @IsMatchingDone,[AtmTraceNo] = @AtmTraceNo,"
                                + " [Matched] = @Matched, [MatchMask] = @MatchMask,"
                                + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                                + " [MetaExceptionId] = @MetaExceptionId, [MetaExceptionNo] = @MetaExceptionNo, "
                                + " [FastTrack] = @FastTrack,"
                                + " [Comments] = @Comments,"
                                + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                                + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                                + " [NotInJournal] = @NotInJournal, [WaitingForUpdating] = @WaitingForUpdating, "
                                + " [SettledRecord] = @SettledRecord "
                                + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@IsMatchingDone", IsMatchingDone);

                            cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                            cmd.Parameters.AddWithValue("@Matched", Matched);
                            cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                            cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                            cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                            cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);
                            cmd.Parameters.AddWithValue("@Comments", Comments);

                            cmd.Parameters.AddWithValue("@FastTrack", FastTrack);
                            cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                            cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                            cmd.Parameters.AddWithValue("@ActionType", ActionType);

                            cmd.Parameters.AddWithValue("@NotInJournal", NotInJournal);
                            cmd.Parameters.AddWithValue("@WaitingForUpdating", WaitingForUpdating);

                            cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                            //rows number of record got updated

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
        }

        //
        // UPDATE  RECORDS Footer 
        // 
        public void UpdateMatchingTxnsMasterPoolATMsFooterFastTruck(string InOperator, int InUniqueRecordId
                , int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;

            using (SqlConnection conn =
               new SqlConnection(connectionString))

                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " SET "
                            + " [MatchedType] = @MatchedType, "
                             + " [ActionType] = @ActionType,"
                            + " [FastTrack] = @FastTrack,"
                            + " [ActionByUser] = @ActionByUser,"
                            + " [UserId] = @UserId "
                            + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType); //
                                                                                  // cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@FastTrack", FastTrack); //
                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser); //

                        cmd.Parameters.AddWithValue("@UserId", UserId); //


                        //rows number of record got updated

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




        ////
        //// UPDATE  SeqNo06 ... temp field for presenter action during management
        //// 
        public void UpdateMatchingTxnsMasterPoolATMs_SeqNo06(int InUniqueRecordId, int InSeqNo06, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            if (In_DB_Mode == 1 || In_DB_Mode == 2)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [SeqNo06] = @SeqNo06 "
                                + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@SeqNo06", InSeqNo06);
                            //rows number of record got updated

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
        }
        //    if (In_DB_Mode == 2)
        //    {
        //        using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //            try
        //            {

        //                conn.Open();
        //                using (SqlCommand cmd =
        //                    new SqlCommand(
        //                         "UPDATE " + "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
        //                        + " SET "
        //                        + " [SeqNo06] = @SeqNo06 "
        //                        + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
        //                    cmd.Parameters.AddWithValue("@SeqNo06", InSeqNo06);

        //                    //rows number of record got updated

        //                    rows = cmd.ExecuteNonQuery();

        //                }
        //                // Close conn
        //                conn.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                conn.Close();
        //                CatchDetails(ex);
        //            }
        //    }
        //}


        //
        // UPDATE  RECORDS Footer by Selection Criteria 
        // 
        public void UpdateMatchingTxnsMasterPoolATMsFooterBySelection(string InMatchingCateg
                                                              , string InTerminalId
                                                              , decimal InTransAmount
                                                              , int InTraceNoWithNoEndZero
                                                              , string InRRNumber
                                                              , string InCardNumber
                                                              , DateTime InTransDate
                                                            , int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            if (In_DB_Mode == 1)
            {
                string SQLCmd = "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " SET "
                            + " [IsMatchingDone] = @IsMatchingDone,[AtmTraceNo] = @AtmTraceNo, [Matched] = @Matched, [MatchMask] = @MatchMask,"
                            + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                            + " [MetaExceptionId] = @MetaExceptionId, [MetaExceptionNo] = @MetaExceptionNo, "
                            + " [FastTrack] = @FastTrack,"
                            + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                            + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                            + " [NotInJournal] = @NotInJournal, [WaitingForUpdating] = @WaitingForUpdating, "
                            + " [SettledRecord] = @SettledRecord "
                            + " WHERE MatchingCateg = @MatchingCateg "
                            + " AND TerminalId = @TerminalId "
                            + " AND TransAmount = @TransAmount "
                            + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero "
                            + " AND RRNumber = @RRNumber "
                            + " AND CardNumber = @CardNumber"
                            + " AND CAST(TransDate AS Date) = @TransDate "
                             ;

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                            cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                            cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);
                            cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                            cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                            cmd.Parameters.AddWithValue("@TransDate", InTransDate.Date);


                            cmd.Parameters.AddWithValue("@IsMatchingDone", IsMatchingDone);

                            cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                            cmd.Parameters.AddWithValue("@Matched", Matched);
                            cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                            cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                            cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                            cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                            cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                            cmd.Parameters.AddWithValue("@FastTrack", FastTrack);
                            cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                            cmd.Parameters.AddWithValue("@UserId", UserId);
                            cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                            cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                            cmd.Parameters.AddWithValue("@ActionType", ActionType);

                            cmd.Parameters.AddWithValue("@NotInJournal", NotInJournal);
                            cmd.Parameters.AddWithValue("@WaitingForUpdating", WaitingForUpdating);

                            cmd.Parameters.AddWithValue("@SettledRecord", SettledRecord);

                            //rows number of record got updated

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
        }


        //
        // UPDATE   Footer matching records SeqNos
        // 
        public void UpdateSeqNosInMpa(string InOperator, int InUniqueRecordId, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            //int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [SeqNo01] = @SeqNo01, "
                                + " [SeqNo02] = @SeqNo02, "
                                + " [SeqNo03] = @SeqNo03 "
                                + " WHERE UniqueRecordId = @UniqueRecordId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@SeqNo01", SeqNo01);
                            cmd.Parameters.AddWithValue("@SeqNo02", SeqNo02);
                            cmd.Parameters.AddWithValue("@SeqNo03", SeqNo03);

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

        //
        // UPDATE   Footer matching records with AGING 
        // 
        public void UpdateMpaWithDetailsFromIST_Aging(int InSeqNo, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " [MatchingCateg] = @MatchingCateg, "
                                + " [AccNumber] = @AccNumber, "
                                + " [CardNumber] = @CardNumber, "
                                + " [TransDate] = @TransDate, "
                                + " [TXNSRC] = @TXNSRC, "
                                + " [TXNDEST] = @TXNDEST, "
                                + " [RRNumber] = @RRNumber, "
                                + " [Card_Encrypted] = @Card_Encrypted, "
                                + " [CAP_DATE] = @CAP_DATE, "
                                + " [SET_DATE] = @SET_DATE "
                                + " WHERE SeqNo = @SeqNo ", conn))
                        {
                            cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                            cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                            cmd.Parameters.AddWithValue("@AccNumber", AccNumber);
                            cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                            cmd.Parameters.AddWithValue("@TransDate", TransDate);
                            cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                            cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);
                            cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                            cmd.Parameters.AddWithValue("@Card_Encrypted", Card_Encrypted);
                            cmd.Parameters.AddWithValue("@CAP_DATE", CAP_DATE);
                            cmd.Parameters.AddWithValue("@SET_DATE", SET_DATE);

                            //rows number of record got updated

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
        }
        //
        // UPDATE Card And ACCOUNT if Not in T24
        // 
        public void UpdateTranWithCard_Account(int InUniqueRecordId, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int Row;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " SET "
                            + " CardNumber = @CardNumber, AccNumber = @AccNumber"
                            + " WHERE UniqueRecordId = @UniqueRecordId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@AccNumber", AccNumber);

                        //rows number of record got updated

                        Row = cmd.ExecuteNonQuery();

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
        // UPDATE Transaction for Deposit 
        // 
        public void UpdateTransforDeposits(int InUniqueRecordId, int In_DB_Mode)
        {

            ErrorFound = false;
            ErrorOutput = "";
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                + " Comments = @Comments, DepCount = @DepCount"
                                + " WHERE UniqueRecordId = @UniqueRecordId", conn))
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                            cmd.Parameters.AddWithValue("@Comments", Comments);
                            cmd.Parameters.AddWithValue("@DepCount", DepCount);

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
        // Insert TRANS FROM EJOURNAL TO IN POOL TO BE UPDATED 
        // 
        // 
        public int InsertTransMasterPoolATMs(string InOperator)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                + " ( "
                + " [OriginFileName] , "
                + " [OriginalRecordId] ,"
                + " [MatchingCateg] ,"
                + " [RMCateg] ,"
                + " [LoadedAtRMCycle], "
                + " [MatchingAtRMCycle], "
                + " [UniqueRecordId],"
                + " [Origin] ,"
                + " [TerminalType] ,"
                + " [TargetSystem] ,"
                + " [TransTypeAtOrigin] ,"
                + " [Product] ,"
                + " [CostCentre] ,"
                + " [TerminalId] ,"
                + " [TransType] ,"
                + " [TransDescr] ,"
                + " [CardNumber] ,"
                + " [IsOwnCard] ,"
                + " [AccNumber] ,"
                + " [TransCurr] ,"
                + " [TransAmount] ,"
                + " [DepCount] ,"
                + " [TransDate] ,"
                + " [TraceNoWithNoEndZero] ,"
                + " [AtmTraceNo] ,"
                + " [MasterTraceNo] ,"
                + " [Matched] ,"
                + " [MatchMask] ,"
                + " [MetaExceptionId] ,"
                + " [MetaExceptionNo] ,"
                + " [FileId01] ,"
                  + " [SeqNo01] ,"
                + " [FileId02] ,"
                 + " [SeqNo02] ,"
                + " [FileId03] ,"
                  + " [SeqNo03] ,"
                + " [RRNumber] ,"
                + " [ResponseCode] ,"
                + " [Comments] "
                + " ,[Operator]"
                + " ,[ReplCycleNo] "
                  + " ,[Card_Encrypted] "
                    + " ,[TXNSRC] "
                      + " ,[TXNDEST] "
                       + " ,[ACCEPTOR_ID] "
                      + " ,[ACCEPTORNAME] "
                         + " ,[CAP_DATE] "
                         + " ,[SET_DATE] "
                         + " ,[UTRNNO] "
            + " )"
                + " VALUES "
                 + " ("
                + " @OriginFileName , "
                + " @OriginalRecordId ,"
                + " @MatchingCateg ,"
                + " @RMCateg ,"
                + " @LoadedAtRMCycle, "
                + " @MatchingAtRMCycle, "
                + " @UniqueRecordId,"
                + " @Origin ,"
                 + " @TerminalType ,"
                + " @TargetSystem ,"
                + " @TransTypeAtOrigin ,"
                + " @Product ,"
                + " @CostCentre ,"
                + " @TerminalId ,"
                + " @TransType ,"
                + " @TransDescr ,"
                + " @CardNumber ,"
                + " @IsOwnCard ,"
                + " @AccNumber ,"
                + " @TransCurr ,"
                + " @TransAmount ,"
                + " @DepCount ,"
                + " @TransDate ,"
                + " @TraceNoWithNoEndZero ,"
                + " @AtmTraceNo ,"
                + " @MasterTraceNo ,"
                + " @Matched ,"
                + " @MatchMask ,"
                + " @MetaExceptionId ,"
                + " @MetaExceptionNo ,"
                + " @FileId01 ,"
                 + " @SeqNo01 ,"
                + " @FileId02 ,"
                  + " @SeqNo02 ,"
                + " @FileId03 ,"
                   + " @SeqNo03 ,"
                + " @RRNumber ,"
                + " @ResponseCode ,"
                + " @Comments ,"
                + " @Operator  "
                + " ,@ReplCycleNo "
                  + " ,@Card_Encrypted "
                    + " ,@TXNSRC "
                      + " ,@TXNDEST "
                          + " ,@ACCEPTOR_ID "
                      + " ,@ACCEPTORNAME "
                         + " , @CAP_DATE "
                         + " , @SET_DATE "
                         + " ,@UTRNNO "
                      + ") "
               + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", TerminalType);
                        cmd.Parameters.AddWithValue("@TargetSystem", TargetSystem);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);

                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@IsOwnCard", IsOwnCard);
                        cmd.Parameters.AddWithValue("@AccNumber", AccNumber);

                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);

                        cmd.Parameters.AddWithValue("@TransAmount", TransAmount);
                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", TraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@MasterTraceNo", MasterTraceNo);

                        cmd.Parameters.AddWithValue("@Matched", Matched);
                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                        cmd.Parameters.AddWithValue("@FileId01", FileId01);
                        cmd.Parameters.AddWithValue("@SeqNo01", SeqNo01);
                        cmd.Parameters.AddWithValue("@FileId02", FileId02);
                        cmd.Parameters.AddWithValue("@SeqNo02", SeqNo02);
                        cmd.Parameters.AddWithValue("@FileId03", FileId03);
                        cmd.Parameters.AddWithValue("@SeqNo03", SeqNo03);

                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);

                        cmd.Parameters.AddWithValue("@Comments", Comments);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@Card_Encrypted", Card_Encrypted);

                        cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);

                        cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);

                        cmd.Parameters.AddWithValue("@ACCEPTOR_ID", ACCEPTOR_ID);
                        cmd.Parameters.AddWithValue("@ACCEPTORNAME", ACCEPTORNAME);
                        cmd.Parameters.AddWithValue("@CAP_DATE", CAP_DATE);
                        cmd.Parameters.AddWithValue("@SET_DATE", SET_DATE);
                        cmd.Parameters.AddWithValue("@UTRNNO", UTRNNO);

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
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Unique Number 
        //
        public void ReadInPoolTransSpecificToCheckIfExist(string InTerminalId, decimal InTransAmount, DateTime InTransDate, int InTraceNoWithNoEndZero)
        //(Mpa.TerminalId, Mpa.TransAmount, Mpa.TransDate, Mpa.TraceNoWithNoEndZero); 
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
          
                SqlString = "SELECT * "
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
              + " WHERE TerminalId = @TerminalId "
               + " AND  TransAmount = @TransAmount"
                 + " AND  TransDate = @TransDate"
                   + " AND  TraceNoWithNoEndZero = @TraceNoWithNoEndZero"
              ;
            

            //if (In_DB_Mode == 2)
            //{
            //    SqlString =
            //       " WITH MergedTbl AS "
            //     + " ( "
            //     + " SELECT *  "
            //     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            //   + " WHERE UniqueRecordId = @UniqueRecordId"
            //     + " UNION ALL  "
            //     + " SELECT * "
            //     + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            //   + " WHERE UniqueRecordId = @UniqueRecordId"
            //     + " ) "
            //     + " SELECT * FROM MergedTbl "
            //     + "  ";
            //}

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransAmount", InTransAmount);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNoWithNoEndZero);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                           // ReadFieldsInTable(rdr);
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

        // CREATE TRANSACTIONS MANUAL INPUT 
        // 
        // NOT FINALISED YET
        public int InsertTransMasterPoolATMs_2_Insert_Manually(string InOperator)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO " + "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                    + " ( "
                    + " [OriginFileName] , "
                    + " [OriginalRecordId] ,"
                    + " [MatchingCateg] ,"
                    + " [RMCateg] ,"
                    + " [LoadedAtRMCycle], "
                    + " [MatchingAtRMCycle], "
                    + " [UniqueRecordId],"
                    + " [Origin] ,"
                    + " [TerminalType] ,"
                    //+ " [TargetSystem] ,"
                    //+ " [TransTypeAtOrigin] ,"
                    //+ " [Product] ,"
                    //+ " [CostCentre] ,"
                    + " [TerminalId] ,"
                    + " [TransType] ,"
                    + " [TransDescr] ,"
                    + " [CardNumber] ,"
                    + " [IsOwnCard] ,"
                    + " [AccNumber] ,"
                    + " [TransCurr] ,"
                    + " [TransAmount] ,"
                    + " [DepCount] ,"
                    + " [TransDate] ,"
                    + " [TraceNoWithNoEndZero] ,"
                    + " [AtmTraceNo] ,"
                   + " [NotInJournal] ,"

                   + " [FuID] ,"
                   + " [IsMatchingDone] ,"
                    + " [Matched] ,"
                    + " [MatchMask] ,"

                    + " [MatchedType] ,"

                    + " [MetaExceptionId] ,"
                    + " [MetaExceptionNo] ,"
                    + " [FileId01] ,"
                    + " [FileId02] ,"
                    + " [FileId03] ,"
                    + " [RRNumber] ,"
                    + " [ResponseCode] ,"
                    + " [Comments] ,"

                  + " [TXNSRC] ,"
                    + " [TXNDEST] ,"
                    + " [Operator],"
                    + " [ReplCycleNo] "
                    + " )"
                    + " VALUES "
                     + " ("
                    + " @OriginFileName , "
                    + " @OriginalRecordId ,"
                    + " @MatchingCateg ,"
                    + " @RMCateg ,"
                    + " @LoadedAtRMCycle, "
                    + " @MatchingAtRMCycle, "
                     //  + " @UniqueRecordId,"
                     + " NEXT VALUE FOR [RRDM_Reconciliation_ITMX].dbo.ReconcSequence ," //3
                    + " @Origin ,"
                     + " @TerminalType ,"
                    //+ " @TargetSystem ,"
                    //+ " @TransTypeAtOrigin ,"
                    //+ " @Product ,"
                    //+ " @CostCentre ,"
                    + " @TerminalId ,"
                    + " @TransType ,"
                    + " @TransDescr ,"
                    + " @CardNumber ,"
                    + " @IsOwnCard ,"
                    + " @AccNumber ,"
                    + " @TransCurr ,"
                    + " @TransAmount ,"
                    + " @DepCount ,"
                    + " @TransDate ,"
                    + " @TraceNoWithNoEndZero ,"
                    + " @AtmTraceNo ,"
                      + " @NotInJournal ,"
                      + " @FuID ,"
                      + " @IsMatchingDone ,"
                    + " @Matched ,"
                    + " @MatchMask ,"
                      + " @MatchedType ,"
                    + " @MetaExceptionId ,"
                    + " @MetaExceptionNo ,"
                    + " @FileId01 ,"
                    + " @FileId02 ,"
                    + " @FileId03 ,"
                    + " @RRNumber ,"
                    + " @ResponseCode ,"
                    + " @Comments ,"
                     + " @TXNSRC ,"
                      + " @TXNDEST ,"
                    + " @Operator , "
                    + " @ReplCycleNo ) "
                   + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
                        //cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", TerminalType);

                        //cmd.Parameters.AddWithValue("@TargetSystem", TargetSystem);
                        //cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        //cmd.Parameters.AddWithValue("@Product", Product);
                        //cmd.Parameters.AddWithValue("@CostCentre", CostCentre);

                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@IsOwnCard", IsOwnCard);
                        cmd.Parameters.AddWithValue("@AccNumber", AccNumber);

                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);

                        cmd.Parameters.AddWithValue("@TransAmount", TransAmount);
                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", TraceNoWithNoEndZero);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@NotInJournal", NotInJournal);

                        cmd.Parameters.AddWithValue("@FuID", FuID);

                        cmd.Parameters.AddWithValue("@IsMatchingDone", IsMatchingDone);
                        cmd.Parameters.AddWithValue("@Matched", Matched);
                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                      
                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                        cmd.Parameters.AddWithValue("@FileId01", FileId01);
                        cmd.Parameters.AddWithValue("@FileId02", FileId02);
                        cmd.Parameters.AddWithValue("@FileId03", FileId03);

                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);

                        cmd.Parameters.AddWithValue("@Comments", Comments);

                        cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                        cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

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

        // UPDATE  
        // 
        public void UpdateMatchingTxnsMasterPoolATMsManual(string InOperator, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int DB_Mode = 1; 
            //int rows;
            if (DB_Mode == 1)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(
                                "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                                + " SET "
                                  + " [OriginFileName]= @OriginFileName , "
                    + " [OriginalRecordId]= @OriginalRecordId ,"
                    + " [MatchingCateg]= @MatchingCateg ,"
                    + " [RMCateg]= @RMCateg ,"
                    + " [LoadedAtRMCycle]= @LoadedAtRMCycle , "
                    + " [MatchingAtRMCycle]= @MatchingAtRMCycle , "
                    //+ " [UniqueRecordId]= @UniqueRecordId ,"
                    + " [Origin]= @Origin ,"
                    + " [TerminalType]= @TerminalType ,"

                    + " [TerminalId]= @TerminalId ,"
                    + " [TransType]= @TransType ,"
                    + " [TransDescr]= @TransDescr ,"
                    + " [CardNumber]= @CardNumber ,"
                    + " [IsOwnCard]= @IsOwnCard ,"
                    + " [AccNumber]= @AccNumber ,"
                    + " [TransCurr]= @TransCurr ,"
                    + " [TransAmount]= @TransAmount ,"
                    + " [DepCount]= @DepCount ,"
                    + " [TransDate]= @TransDate ,"
                    + " [TraceNoWithNoEndZero]= @TraceNoWithNoEndZero ,"
                    + " [AtmTraceNo]= @AtmTraceNo ,"
                   + " [NotInJournal]= @NotInJournal ,"

                   + " [FuID]= @FuID ,"
                   + " [IsMatchingDone]= @IsMatchingDone ,"
                    + " [Matched]= @Matched ,"
                    + " [MatchMask]= @MatchMask ,"

                    + " [MatchedType]= @MatchedType ,"

                    + " [MetaExceptionId]= @MetaExceptionId ,"
                    + " [MetaExceptionNo]= @MetaExceptionNo ,"
                    + " [FileId01]= @FileId01 ,"
                    + " [FileId02]= @FileId02 ,"
                    + " [FileId03]= @FileId03 ,"
                    + " [RRNumber]= @RRNumber ,"
                    + " [ResponseCode]= @ResponseCode ,"
                    + " [Comments]= @Comments ,"

                  + " [TXNSRC]= @TXNSRC ,"
                    + " [TXNDEST]= @TXNDEST ,"
                    + " [Operator]= @Operator ,"
                    + " [ReplCycleNo]= @ReplCycleNo "
                                + " WHERE SeqNo =@SeqNo" , conn))
                        {
                            cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                            cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                            cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                            cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                            cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
                            //cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

                            cmd.Parameters.AddWithValue("@Origin", Origin);
                            cmd.Parameters.AddWithValue("@TerminalType", TerminalType);

                            //cmd.Parameters.AddWithValue("@TargetSystem", TargetSystem);
                            //cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                            //cmd.Parameters.AddWithValue("@Product", Product);
                            //cmd.Parameters.AddWithValue("@CostCentre", CostCentre);

                            cmd.Parameters.AddWithValue("@TerminalId", TerminalId);

                            cmd.Parameters.AddWithValue("@TransType", TransType);
                            cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
                            cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                            cmd.Parameters.AddWithValue("@IsOwnCard", IsOwnCard);
                            cmd.Parameters.AddWithValue("@AccNumber", AccNumber);

                            cmd.Parameters.AddWithValue("@TransCurr", TransCurr);

                            cmd.Parameters.AddWithValue("@TransAmount", TransAmount);
                            cmd.Parameters.AddWithValue("@DepCount", DepCount);

                            cmd.Parameters.AddWithValue("@TransDate", TransDate);
                            cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", TraceNoWithNoEndZero);
                            cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                            cmd.Parameters.AddWithValue("@NotInJournal", NotInJournal);

                            cmd.Parameters.AddWithValue("@FuID", FuID);

                            cmd.Parameters.AddWithValue("@IsMatchingDone", IsMatchingDone);
                            cmd.Parameters.AddWithValue("@Matched", Matched);
                            cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                            cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                            cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);

                            cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                            cmd.Parameters.AddWithValue("@FileId01", FileId01);
                            cmd.Parameters.AddWithValue("@FileId02", FileId02);
                            cmd.Parameters.AddWithValue("@FileId03", FileId03);

                            cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                            cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);

                            cmd.Parameters.AddWithValue("@Comments", Comments);

                            cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                            cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);

                            cmd.Parameters.AddWithValue("@Operator", Operator);
                            cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

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
}

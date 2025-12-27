using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace RRDM4ATMs
{
    public class RRDMMatchingOfTxns_V02_MinMaxDt : Logger
    {
        public RRDMMatchingOfTxns_V02_MinMaxDt() : base() { }
        /// <summary>
        /// FILE 01 - tblMatchingTxnsMasterPoolATMs
        /// </summary>
        /// 
        // Record Fields 

        public int SeqNo;
        public string MatchingCateg;
        public int RMCycle;
        public string TerminalId;
        public int TransType;
        public string TransDescr;
        public string CardNumber;
        public string AccNo;
        public string TransCurr;
        public decimal TransAmt;
        public decimal AmtFileBToFileC;
        public DateTime TransDate;
        public int TraceNo;
        public int RRNumber;

        public int OriginSeqNo;

        string WOrigin;

        //string WSelectionCriteria;

        string WJobCategory = "ATMs";

        //public DateTime TxnDate; 

        public string WDepCurNm;

        // Define the data tables 
        public DataTable TableThisCategoryGroups = new DataTable();

        public DataTable TableThisGroupAtms = new DataTable();

        public DataTable TableUnMatched = new DataTable();
        public DataTable TableUnMatchedSaved = new DataTable();
        public int TotalSelected;

        string Message;

        // Define the data table 
        public DataTable TableUnMatchedCompressed = new DataTable();

        public int TotalSelected2;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        bool ShowMessage;

        // Define the data table 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();
        RRDMMatchingOfTxnsFindOriginRAW Msf = new RRDMMatchingOfTxnsFindOriginRAW();
        RRDMMatchingCategoriesVsSourcesFiles Mcsf = new RRDMMatchingCategoriesVsSourcesFiles();
        RRDMMatchingSourceFiles Msourcef = new RRDMMatchingSourceFiles();
        RRDMMatchingCategStageVsMatchingFields Mf = new RRDMMatchingCategStageVsMatchingFields();

        RRDMMatchingTxns_WorkingTables Mtw = new RRDMMatchingTxns_WorkingTables();
        RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();

        RRDMMatchingReconcExceptionsInfoAnyTables Mre = new RRDMMatchingReconcExceptionsInfoAnyTables();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        string SelectionCriteria;

        DateTime WCut_Off_Date;

        int NumberOfTables;

        string SourceTable_A;
        string SourceTable_B;
        string SourceTable_C;

        // Table Fields 
        public string TableId;
        public string WCase;
        public int Type;
        public int DublInPos;
        public int NotInPos;
        public string UserId;

        public int NumberOfUnmatchedForCategory;

        //int MaxTrace01;
        //int MaxTrace02;
        //int MaxTrace03;
        //int MaxTrace;
        //int MinMaxTrace;

        DateTime MaxDt01;
        DateTime MaxDt02;
        DateTime MaxDt03;
        int MaxDt01_TraceNo;
        int MaxDt02_TraceNo;
        int MaxDt03_TraceNo;
        //DateTime MaxDt;
        DateTime MinMaxDt;
        DateTime MinMaxDt_01;
        DateTime MinMaxDt_02;
        DateTime MinMaxDt_03;

        bool FromOwnATMs;

        //bool TestingTwoFiles; 

        string WMatchingCategoryId;
        int WRMCycle;
        int WGroupId;
        string WReconcCategoryId;
        string WSignedId;
        string WOperator;
        bool WTestingWorkingFiles;

        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        public void Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup
                                      (string InOperator, string InSignedId,
                                       string InMatchingCategoryId,
                                       int InRMCycle, bool InTestingWorkingFiles)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WMatchingCategoryId = InMatchingCategoryId;
            WRMCycle = InRMCycle;
            WTestingWorkingFiles = InTestingWorkingFiles;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

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

            int TempCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            WCut_Off_Date = Rjc.Cut_Off_Date;

            #region Assign Files as defined in Matching Category

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCategoryId);

            WOrigin = Mc.Origin;

            if (Mc.ReconcMaster == true)
            {
                FromOwnATMs = false;
            }
            else
            {
                FromOwnATMs = true;
            }

            Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCategoryId);
            NumberOfTables = Mcsf.TotalRecords;

            if (NumberOfTables == 3 & FromOwnATMs == true)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameC);
                SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 3 & FromOwnATMs == false)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                // Move Records from A to Master Pool

                Mgt.ReadUniversalAndCreateMasterPoolRecords(WOperator, SourceTable_A, WRMCycle);

                // Change SourceTable_A to master Pool 

                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameC);
                SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }
// Copy records to Twins For Bank Du Caire
            if (NumberOfTables == 3 & FromOwnATMs == true 
                & (WOperator == "BCAIEGCX" & WMatchingCategoryId == "BDC_201"))
            {
                // CREATE Twins for Bank Du Caire
                string Twin_1 = "Table123";
                string Condition = "123NetWork";

                Mgt.CopyRecordsToTwin(SourceTable_C, Twin_1, Condition);

                string Twin_2 = "MasterCard";
                string Condition2 = "MasterCard";

                Mgt.CopyRecordsToTwin(SourceTable_C, Twin_2, Condition);

                // Update Category file 
                // BDC_202
                // BDC_203
                // That Twins has been read

            }

            if (NumberOfTables == 2 & FromOwnATMs == true)
            {

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 2 & FromOwnATMs == false)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

                // Move Records from A to Master Pool
                Mgt.ReadUniversalAndCreateMasterPoolRecords(WOperator, SourceTable_A, WRMCycle);

                // Change SourceTable_A to master Pool 

                SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";
            }


            #endregion

            #region Make Matching For All Groups

            #region Read And Find All Groups of this Category 
            // Make Loop for all Groups of this category

            // Read All Groups that have this Matching category 
            // Fill Up a Table 

            NumberOfUnmatchedForCategory = 0;

            if (FromOwnATMs == true)
            {
                TableThisCategoryGroups = new DataTable();
                TableThisCategoryGroups.Clear();

                SqlString =
                   " SELECT MatchingCategoryId , GroupId, ReconcCategoryId "
                   + " FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                   + " WHERE MatchingCategoryId = @MatchingCategoryId "
                   + " ORDER BY GroupId ";

                using (SqlConnection conn =
                 new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCategoryId", WMatchingCategoryId);

                            //Create a datatable that will be filled with the data retrieved from the command

                            sqlAdapt.Fill(TableThisCategoryGroups);

                            // Close conn
                            conn.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                Message = "Groups identified within category : " + WMatchingCategoryId;
                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }

            #endregion
            #region Loop for Groups of this matching category , make matching and update sessions 

            if (FromOwnATMs == true)
            {
                int I = 0;

                // LOOP FOR GROUPS OF THIS MATCHING CATEGORY

                while (I <= (TableThisCategoryGroups.Rows.Count - 1))
                {

                    WMatchingCategoryId = (string)TableThisCategoryGroups.Rows[I]["MatchingCategoryId"];
                    WGroupId = (int)TableThisCategoryGroups.Rows[I]["GroupId"];
                    WReconcCategoryId = (string)TableThisCategoryGroups.Rows[I]["ReconcCategoryId"];
                    //******************************************************
                    // CREATE FILES FOR THE SPECIFIC GROUP BASED ON MINMAX DATE TIME
                    // Find corresponding datetimes in corresponding files
                    // Create table with Cut Off last Seq Numbers
                    // UPDATE SOURCE FILES AS PROCESSED
                    //******************************************************
                    CreateWorkingTables(WOperator, WSignedId, WMatchingCategoryId, WReconcCategoryId,
                                                                                 WRMCycle, WGroupId);

                    if (NumberOfLoadedToWorkingTablesATMs > 0)
                    {
                        //*******************************
                        // MAkE MATCHING For this Group 
                        //******************************

                        MakeMatchingByGroupOrCategory(WOperator, WSignedId, WMatchingCategoryId, WReconcCategoryId, WRMCycle, WGroupId);

                        //************************
                        // FINALLY - UPDATING AFTER MATCHING 
                        //************************

                        UpdatingAfterMatching_OurATMs(WOperator, WMatchingCategoryId, WReconcCategoryId);

                    }

                    I++; // Read Next entry of the table - next Group

                }
            }
            else
            {
                //*************************
                // From NOT_ATMs
                //************************* 
                WGroupId = 0;

                //******************************************************
                // CREATE FILES FOR THE SPECIFIC CATEGORY BASED ON MINMAX
                // NO GROUP
                // UPDATE SOURCE FILES AS PROCESSED          
                //******************************************************

                WReconcCategoryId = WMatchingCategoryId;

                CreateWorkingTables(WOperator, WSignedId,
                                                  WMatchingCategoryId, WReconcCategoryId,
                                                                 WRMCycle, WGroupId);

                if (NumberOfLoadedToWorkingTablesATMs > 0)
                {
                    //*******************************
                    // MAkE MATCHING
                    //******************************
                    MakeMatchingByGroupOrCategory(WOperator, WSignedId, WMatchingCategoryId, WReconcCategoryId, WRMCycle, WGroupId);

                    //************************
                    // FINALLY - UPDATING AFTER MATCHING 
                    //************************
                    UpdatingAfterMatching_NOT_OurATMs(WOperator, WMatchingCategoryId, WReconcCategoryId);
                }

            }


            #endregion

            #endregion

            //#region UpdateCategory VS Source Files that Matching is Done  

            //Mcsf.UpdateReconcCategoryVsSourceRecord(WOperator, 66);     

            //#endregion

        }
        //
        // CREATE FILES FOR THE SPECIFIC GROUP, 
        //
        int NumberOfLoadedToWorkingTablesATMs = 0;
        int WAtmsReconcGroup;
        string WAtmNo;
        bool RecordsToMatch;
        string PrimaryMatchingFieldNm;

        int MaxPrimaryMatchingField_01;
        int MaxPrimaryMatchingField_02;
        int MaxPrimaryMatchingField_03;
        //DateTime LastDateTime;
        public void CreateWorkingTables(string InOperator, string InSignedId,
                                                  string InMatchingCateg, string InReconcCateg,
                                                  int WRMCycle, int InAtmsReconcGroup)
        {
            #region Initialise Working Tables

            NumberOfLoadedToWorkingTablesATMs = 0;
            int I = 0;

            // Initiliase Working Files 

            // Initialise File 01
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);
            // Initialise File 02
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);

            if (NumberOfTables == 3)
            {
                // Initialise File 03
                TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";

                if (WTestingWorkingFiles == false)
                    Mtw.TruncateInWorkingFile(TableId);
            }

            #endregion

            #region For this Group Find all ATMs

            // Make Loop for this Group 
            if (FromOwnATMs == true)
            {
                //*****************************************
                // LOOP - LOOP - LOOP
                //***************************************** 
                // Read All ATMs belonging to this Group 
                // Fill Up a Table 
                TableThisGroupAtms = new DataTable();
                TableThisGroupAtms.Clear();

                SqlString =
                      " SELECT AtmsReconcGroup , AtmNo, DepCurNm"
                    + " FROM [ATMS].[dbo].[ATMsFields] "
                    + " WHERE AtmsReconcGroup = @AtmsReconcGroup ";

                using (SqlConnection conn =
                 new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmsReconcGroup", InAtmsReconcGroup);
                            //Create a datatable that will be filled with the data retrieved from the command

                            sqlAdapt.Fill(TableThisGroupAtms);

                            // Close conn
                            conn.Close();

                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }
            }

            #endregion

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            #region For Each ATM Find MinMax DateTime
            if (FromOwnATMs == true)
            {
                RecordsToMatch = false;

                while (I <= (TableThisGroupAtms.Rows.Count - 1))
                {
                    RecordFound = true;

                    WAtmsReconcGroup = (int)TableThisGroupAtms.Rows[I]["AtmsReconcGroup"];
                    WAtmNo = (string)TableThisGroupAtms.Rows[I]["AtmNo"];

                    //*****************************************************
                    // CHECK IF THERE ARE RECORDS TO MATCHED FOR THE THREE FILES
                    //*****************************************************
                    // Find MaxDate for unmatched records
                    //
                    // Find MINMAX Trace for this ATM

                    MaxDt01 = NullPastDate;
                    MaxDt02 = NullPastDate;
                    if (NumberOfTables == 3)
                    {
                        MaxDt03 = NullPastDate;
                    }

                    TableId = SourceTable_A;
                    MaxDt01 = Mgt.ReadAndFindMaxDateTimeForMpa(TableId, InMatchingCateg, WAtmNo);

                    TableId = SourceTable_B;
                    MaxDt02 = Mgt.ReadAndFindMaxDt(TableId, InMatchingCateg, WAtmNo);
                    if (NumberOfTables == 3)
                    {
                        TableId = SourceTable_C;
                        MaxDt03 = Mgt.ReadAndFindMaxDt(TableId, InMatchingCateg, WAtmNo);
                    }

                    if (NumberOfTables == 2)
                    {
                        if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate)
                        {
                            // No records for one of files 
                            I++; // Read Next entry of the table 
                            continue;
                        }
                    }
                    if (NumberOfTables == 3)
                    {
                        if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate || MaxDt03 == NullPastDate)
                        {
                            // No records for one of files 
                            I++; // Read Next entry of the table 
                            continue;
                        }
                    }

                    WDepCurNm = (string)TableThisGroupAtms.Rows[I]["DepCurNm"];


                    // Find MINMAX Trace for this ATM

                    //*********************************
                    // Find CORRESPONDING SEQ FOR MAX DATES
                    //*********************************
                    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";
                    TableId = SourceTable_A;
                    //MaxDt01 = Mgt.ReadAndFindMaxDateTimeForMpa(TableId, InMatchingCateg, WAtmNo);
 
                    MaxDt01_TraceNo = Mgt.ReadAndFindTraceNoFromMaxDateTimeForMpa(TableId, InMatchingCateg, WAtmNo, MaxDt01);

                    // Find MINMAX Trace for File 02
                    //FileId = "[ALECOS-Results].[dbo].[IntblBankSwitchTxns]";
                    TableId = SourceTable_B;

                 //   MaxDt02 = Mgt.ReadAndFindMaxDt(TableId, InMatchingCateg, WAtmNo);
                    MaxDt02_TraceNo = Mgt.ReadAndFindTraceForMaxDt(TableId, InMatchingCateg, WAtmNo, MaxDt02);

                    if (NumberOfTables == 3)
                    {
                        // Find MINMAX Trace for File 03
                        //FileId = "[ALECOS-Results].[dbo].[IntblBankingSystemTxns]";
                        TableId = SourceTable_C;
                        //ReadAndFindMaxTraceNo(TableId, InMatchingCateg, WAtmNo);
                     //   MaxDt03 = Mgt.ReadAndFindMaxDt(TableId, InMatchingCateg, WAtmNo);
                        MaxDt03_TraceNo = Mgt.ReadAndFindTraceForMaxDt(TableId, InMatchingCateg, WAtmNo, MaxDt03);
                    }

                    //
                    // UPDATE TABLE WITH CUT OFF information 
                    // Read Last Trace TARGET file and find corresponding in Mpa
                    // The Target file last file read

                    RRDM_Cut_Off_LastSeqNumbers Coff = new RRDM_Cut_Off_LastSeqNumbers();

                    Coff.DefineLastTraces(WOperator, WMatchingCategoryId, TableId, WAtmNo, WRMCycle);
                    if (Coff.NoTxns == true)
                    {
                        I++;
                        continue;
                    }

                    MinMaxDt = NullPastDate;

                    if (NumberOfTables == 3)
                    {
                        if (MaxDt01 != NullPastDate & MaxDt02 != NullPastDate & MaxDt03 != NullPastDate)
                        {
                            DateTime[] array1 = { MaxDt01, MaxDt02, MaxDt03 };

                            //
                            // Find minimum number.
                            // 

                            MinMaxDt = array1.Where(a => a != NullPastDate).Min();
                            //
                            // Standardise dates
                            //
                            if (MinMaxDt == MaxDt01)
                            {

                                // Based On Trace of First file Number find the exact DateTime of Transaction
                                // MaxDt01_TraceNo

                                MinMaxDt_01 = MinMaxDt;

                                TableId = SourceTable_B;
                                MinMaxDt_02 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt01_TraceNo);

                                TableId = SourceTable_C;
                                MinMaxDt_03 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt01_TraceNo);

                            }
                            if (MinMaxDt == MaxDt02)
                            {

                                // Based On Trace Number find the exact DateTime of Transaction
                                // MaxDt02_TraceNo
                                TableId = SourceTable_A;
                                MinMaxDt_01 = Mgt.ReadAndFindDateTimeForTraceMpa(TableId, InMatchingCateg, WAtmNo, MaxDt02_TraceNo);

                                MinMaxDt_02 = MinMaxDt;

                                TableId = SourceTable_C;
                                MinMaxDt_03 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt02_TraceNo);
                            }
                            if (MinMaxDt == MaxDt03)
                            {
                                // Based On Trace Number find the exact DateTime of Transaction
                                // MaxDt03_TraceNo
                                TableId = SourceTable_A;
                                MinMaxDt_01 = Mgt.ReadAndFindDateTimeForTraceMpa(TableId, InMatchingCateg, WAtmNo, MaxDt03_TraceNo);

                                TableId = SourceTable_B;
                                MinMaxDt_02 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt03_TraceNo);

                                MinMaxDt_03 = MinMaxDt;

                            }
                        }
                    }

                    if (NumberOfTables == 2)
                    {
                        if (MaxDt01 != NullPastDate & MaxDt02 != NullPastDate)
                        {
                            DateTime[] array1 = { MaxDt01, MaxDt02 };

                            //
                            // Find minimum number.
                            // 

                            MinMaxDt = array1.Where(a => a != NullPastDate).Min();

                            // Standardise dates

                            // Standardise dates

                            if (MinMaxDt == MaxDt01)
                            {

                                // Based On Trace of First file Number find the exact DateTime of Transaction

                                MinMaxDt_01 = MinMaxDt;

                                TableId = SourceTable_B;
                                MinMaxDt_02 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt01_TraceNo);

                                TableId = SourceTable_C;
                                MinMaxDt_03 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt01_TraceNo);

                            }
                            if (MinMaxDt == MaxDt02)
                            {

                                // Based On Trace Number find the exact DateTime of Transaction
                                TableId = SourceTable_A;
                                MinMaxDt_01 = Mgt.ReadAndFindDateTimeForTraceMpa(TableId, InMatchingCateg, WAtmNo, MaxDt02_TraceNo);

                                MinMaxDt_02 = MinMaxDt;

                                TableId = SourceTable_C;
                                MinMaxDt_03 = Mgt.ReadAndFindDateTimeForTrace(TableId, InMatchingCateg, WAtmNo, MaxDt02_TraceNo);
                            }

                        }
                    }

                    #endregion

                    #region For Each ATM Insert Records in Working Files and set original records as processed

                    if (MinMaxDt != NullPastDate)
                    {
                        RecordsToMatch = true;

                        NumberOfLoadedToWorkingTablesATMs = NumberOfLoadedToWorkingTablesATMs + 1;

                        // Call stored Procedure to create files
                        // Insert Records In File01Y from Mpa
                        if (WTestingWorkingFiles == false)
                              Mtw.PopulateWorkingFile_ATMs_Working01("01", SourceTable_A, WMatchingCategoryId, WRMCycle, WAtmNo, MinMaxDt_01);
                        // Insert Records In File02Y from IST
                        if (WTestingWorkingFiles == false)
                            //InsertRecordsInWorkingFile_02(connectionString, WMatchingCategoryId, WAtmNo, MinMaxTrace, WRMCycle);
                            Mtw.PopulateWorkingFile_ATMs_V02("02", SourceTable_B, WMatchingCategoryId, WRMCycle, WAtmNo, MinMaxDt_02);

                        if (NumberOfTables == 3)
                        {
                            // Insert Records In File03Y from Fiserv
                            if (WTestingWorkingFiles == false)
                                //InsertRecordsInWorkingFile_03(connectionString, WMatchingCategoryId, WAtmNo, MinMaxTrace, WRMCycle);
                                Mtw.PopulateWorkingFile_ATMs_V02("03", SourceTable_C, WMatchingCategoryId, WRMCycle, WAtmNo, MinMaxDt_03);
                        }
                        //
                        // Call to update source files 
                        //
                        // Update SourceTable_A with Processed = 1 and RMCycle
                        string MatchMask = "";
                        if (NumberOfTables == 3)
                        {
                            MatchMask = "111";
                        }
                        if (NumberOfTables == 2)
                        {
                            MatchMask = "11";
                        }

                        Mpa.UpdateMpaRecordsAsProcessed_With_ATM_V02(WMatchingCategoryId, WAtmNo, WRMCycle, MatchMask, MinMaxDt_01,1);

                        // Update SourceTable_B with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                        TableId = SourceTable_B;

                        Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCategoryId, TableId, MinMaxDt_02, WAtmNo, WRMCycle);

                        if (NumberOfTables == 3)
                        {
                            // Update SourceTable_C with Processed = 1 and RMCycle
                            //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                            TableId = SourceTable_C;
                            Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCategoryId, TableId, MinMaxDt_03, WAtmNo, WRMCycle);

                        }

                        //System.Windows.Forms.MessageBox.Show("Txns added in Working Files for AtmNo :"
                        //                              + WAtmNo);

                    }
                    //
                    // DEAL WITH THIS = NO JOURNAL YET FOR THIS ATM
                    //
                    if (MinMaxDt == NullPastDate & MaxDt01 == NullPastDate)
                    {

                        string SelectionCriteria = " WHERE TerminalId ='" + WAtmNo + "'";

                        Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);
                        if (Mpa.RecordFound == true)
                        {
                            // ATM is active 
                        }
                        else
                        {
                            // ATM is not active
                            // Consider all it transactions as processed

                            //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                            TableId = SourceTable_B;

                            Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCategoryId, TableId, MaxDt02, WAtmNo, WRMCycle);

                            if (NumberOfTables == 3)
                            {
                                // Update SourceTable_C with Processed = 1 and RMCycle
                                //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                                TableId = SourceTable_C;
                                Mgt.UpdateSourceTablesAsProcessed_ATMs_V02(WMatchingCategoryId, TableId, MaxDt03, WAtmNo, WRMCycle);
                            }

                        }

                    }

                    I++; // Read Next entry of the table 

                }

                if (NumberOfLoadedToWorkingTablesATMs > 0)
                {
                    Message = "Cut Off Date : .. " + WCut_Off_Date.ToShortDateString() + Environment.NewLine
                                                    + "Txns added in Working Files for "
                                                    + NumberOfLoadedToWorkingTablesATMs.ToString() + " ATMs " + Environment.NewLine
                                                    + " of Group No: " + InAtmsReconcGroup.ToString();

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }
                }
            }

            #endregion

            #region Find MINMax for Tables in the Category that comes not from Our ATMs and Insert Records in Working Files

            //********************
            // NOT ATMs PROCESSING
            //********************
            if (FromOwnATMs == false)
            {
                //*****************************************
                // NO LOOP - NO LOOP - NO LOOP
                //***************************************** 
                RecordsToMatch = false;

                PrimaryMatchingFieldNm = "RRNumber";
                // Find MINMAX Trace for this ATM

                MaxPrimaryMatchingField_01 = 0;
                MaxPrimaryMatchingField_02 = 0;

                if (NumberOfTables == 3)
                {
                    MaxPrimaryMatchingField_03 = 0;
                }

                //*********************************
                // Find MINMAX Trace for File Mpa
                //*********************************
                //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATM
                TableId = SourceTable_A;

                MaxPrimaryMatchingField_01 = Mgt.ReadAndFindMaxTraceNoForMpa_NO_ATM(TableId, PrimaryMatchingFieldNm, InMatchingCateg);


                // Find MAX  for Table 02

                TableId = SourceTable_B;
                //ReadAndFindMaxPrimaryMatchingField(TableId, PrimaryMatchingFieldNm, InMatchingCateg);


                MaxPrimaryMatchingField_02 = Mgt.ReadAndFindMaxPrimaryMatchingField(TableId, PrimaryMatchingFieldNm, InMatchingCateg);


                if (NumberOfTables == 3)
                {
                    // Find MAX  for Table 03
                    TableId = SourceTable_C;
                    //ReadAndFindMaxPrimaryMatchingField(TableId, PrimaryMatchingFieldNm, InMatchingCateg);


                    MaxPrimaryMatchingField_03 = Mgt.ReadAndFindMaxPrimaryMatchingField(TableId, PrimaryMatchingFieldNm, InMatchingCateg);

                }

                int MinMaxPrimaryMatchingField = 0;

                if (NumberOfTables == 3)
                {
                    if (MaxPrimaryMatchingField_01 > 0 & MaxPrimaryMatchingField_02 > 0 & MaxPrimaryMatchingField_03 > 0)
                    {
                        int[] array1 = { MaxPrimaryMatchingField_01, MaxPrimaryMatchingField_02, MaxPrimaryMatchingField_03 };

                        //
                        // Find minimum number.
                        // 

                        MinMaxPrimaryMatchingField = array1.Where(a => a > 0).Min();

                    }
                }

                if (NumberOfTables == 2)
                {
                    if (MaxPrimaryMatchingField_01 > 0 & MaxPrimaryMatchingField_02 > 0)
                    {
                        int[] array1 = { MaxPrimaryMatchingField_01, MaxPrimaryMatchingField_02 };

                        //
                        // Find minimum number.
                        // 

                        MinMaxPrimaryMatchingField = array1.Where(a => a > 0).Min();

                    }
                }

                #region For Category Insert Records in Working Files and set original records as processed

                if (MinMaxPrimaryMatchingField > 0)
                {
                    NumberOfLoadedToWorkingTablesATMs = NumberOfLoadedToWorkingTablesATMs + 1;

                    RecordsToMatch = true;

                    // Call stored Procedure to create files
                    // Insert Records In File01Y from Mpa
                    if (WTestingWorkingFiles == false)
               //     Mtw.PopulateWorkingFile_NOT_ATMs("01", SourceTable_A, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                  //  Mtw.PopulateWorkingFile_ATMs_Working01_NOT_ATMs("01", SourceTable_A, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                    // Insert Records In Working File02 

                    if (WTestingWorkingFiles == false)
                        Mtw.PopulateWorkingFile_NOT_ATMs("02", SourceTable_B, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);

                    if (NumberOfTables == 3)
                    {
                        // Insert Records In Working File03

                        if (WTestingWorkingFiles == false)
                            Mtw.PopulateWorkingFile_NOT_ATMs("03", SourceTable_C, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                    }

                    //
                    // Call to update source files 
                    //
                    // Update SourceTable_A with Processed = 1 and RMCycle
                    string MatchMask = "";
                    if (NumberOfTables == 3)
                    {
                        MatchMask = "111";
                    }
                    if (NumberOfTables == 2)
                    {
                        MatchMask = "11";
                    }

                  //  Mpa.UpdateMpaRecordsAsProcessed_NO_ATM(WMatchingCategoryId, MinMaxPrimaryMatchingField, WRMCycle, MatchMask);

                    //TableId = SourceTable_A;
                    //Mgt.UpdateSourceTablesAsProcessed(WMatchingCategoryId, TableId, MinMaxPrimaryMatchingField, WRMCycle, MatchMask);

                    // Update SourceTable_B with Processed = 1 and RMCycle
                    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                    TableId = SourceTable_B;
             //       Mgt.UpdateSourceTablesAsProcessed_NonAtms(WMatchingCategoryId, TableId, MinMaxPrimaryMatchingField, WRMCycle, MatchMask);

                    if (NumberOfTables == 3)
                    {
                        // Update SourceTable_C with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                        TableId = SourceTable_C;
             //           Mgt.UpdateSourceTablesAsProcessed_NonAtms(WMatchingCategoryId, TableId, MinMaxPrimaryMatchingField, WRMCycle, MatchMask);
                    }

                    Message = "Txns added in Working Files for Matching Category :"
                                                  + InMatchingCateg;

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                }

            }

            #endregion

            #endregion

        }


        // Fields of Working files 
        private void WorkingTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            RMCycle = (int)rdr["RMCycle"];
            TerminalId = (string)rdr["TerminalId"];
            TransType = (int)rdr["TransType"];
            TransDescr = (string)rdr["TransDescr"];
            CardNumber = (string)rdr["CardNumber"];
            AccNo = (string)rdr["AccNo"];
            TransCurr = (string)rdr["TransCurr"];
            TransAmt = (decimal)rdr["TransAmt"];
            AmtFileBToFileC = (decimal)rdr["AmtFileBToFileC"];
            TransDate = (DateTime)rdr["TransDate"];
            TraceNo = (int)rdr["TraceNo"];
            RRNumber = (int)rdr["RRNumber"];

        }

        // Make Matching 
        private void MakeMatchingByGroupOrCategory(string InOperator, string InSignedId, string InMatchingCateg, string InReconcCateg, int InRMCycle, int InAtmsReconcGroup)
        {

            #region Make Mathing for this Group which includes all of its ATMs

            if (FromOwnATMs == true)
            {

                //  MAKE MATCHING AND Update a record per ATM + Rcs (Reconciliation Category)
                //
                if (RecordsToMatch == true)
                {
                    Message = " Category : " + InMatchingCateg + Environment.NewLine
                                    + " Group : " + InAtmsReconcGroup.ToString() + Environment.NewLine
                                    + " MATCHING PROCESS WILL START FOR THIS GROUP  ";

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                    // ******************************************************************
                    // MAKE MATCHING , Create Report, Save Discrepancies After Summarisation
                    //*******************************************************************

                    if (NumberOfTables == 3)
                    {
                        MakeMatchingOf_3_Working_Tables(WOperator, WSignedId,
                                                                      InMatchingCateg, InReconcCateg,
                                                                      WRMCycle);
                    }
                    if (NumberOfTables == 2)
                    {
                        MakeMatchingOf_2_Working_Tables(WOperator, WSignedId,
                                                                      InMatchingCateg, InReconcCateg,
                                                                      WRMCycle);
                    }

                    // *****************************************************************************
                    // Final Message  
                    // *****************************************************************************
                    Message = "Matching completed for GROUP:"
                                                          + WAtmsReconcGroup.ToString();

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                }
            }

            #endregion


            #region Make Mathing for this Category - No coming from our ATMs

            if (FromOwnATMs == false)
            {
                //  MAKE MATCHING AND Update a record per ATM + Rcs (Reconciliation Category)
                //
                if (RecordsToMatch == true)
                {
                    Message = "Category : " + InMatchingCateg + Environment.NewLine
                                    + "MATCHING PROCESS WILL START FOR THIS CATEGORY  ";

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                    // ******************************************************************
                    // MAKE MATCHING , Create Report, Save Discrepancies After Summarisation
                    //*******************************************************************

                    if (NumberOfTables == 3)
                    {
                        MakeMatchingOf_3_Working_Tables(WOperator, WSignedId,
                                                                      InMatchingCateg, InReconcCateg,
                                                                      WRMCycle);
                    }
                    if (NumberOfTables == 2)
                    {
                        MakeMatchingOf_2_Working_Tables(WOperator, WSignedId,
                                                                      InMatchingCateg, InReconcCateg,
                                                                      WRMCycle);
                    }

                    // *****************************************************************************
                    // Final Message  
                    // *****************************************************************************
                    Message = "MATCHING COMPLETED FOR CATEGORY :"
                                                          + WMatchingCategoryId;

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                }
            }

            #endregion
        }
        // Updating after Matching 
        private void UpdatingAfterMatching_OurATMs(string InOperator, string InMatchingCateg, string InReconcCateg)
        {
            // *********************************************************************
            // Update a Reconciliation record per ATM + Rcs (Reconciliation Category)
            // ******************************************************************
            SelectionCriteria = "WHERE RMCateg='" + InReconcCateg + "'"
                                    + " AND MatchingCateg='" + InMatchingCateg + "'"
                                    + " AND IsMatchingDone = 1"
                                    + " AND MatchingAtRMCycle =" + WRMCycle;

            ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_OurATMs(WOperator, WRMCycle, SelectionCriteria);

            // *****************************************************************************
            // Update [tblMatchingTxnsMasterPoolATMs] mask card number with full card number 
            // *****************************************************************************
            //  Check Parameters if wish from cardnumber masked to full 
            // 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "266";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                // IF YES THEN UPDATE MASK CARD WITH FULL 

                if (NumberOfTables == 2)
                {
                    // Second File = Destination File ... SourceTable_B
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_B);

                }

                if (NumberOfTables == 3)
                {
                    // Second File = Destination File ... SourceTable_C
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_C);
                }

            }
        }

        // Updating after Matching 
        private void UpdatingAfterMatching_NOT_OurATMs(string InOperator, string InMatchingCateg, string InReconcCateg)
        {
            // *********************************************************************
            // Update a Reconciliation record per ATM + Rcs (Reconciliation Category)
            // ******************************************************************

            SelectionCriteria = "WHERE RMCateg='" + InReconcCateg + "'"
                                + " AND MatchingCateg='" + InMatchingCateg + "'"
                                + " AND IsMatchingDone = 1"
                                + " AND MatchingAtRMCycle =" + WRMCycle;

            ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_NOT_OurATMs(WOperator, WRMCycle, SelectionCriteria);


            // *****************************************************************************
            // Update [tblMatchingTxnsMasterPoolATMs] mask card number with full card number 
            // *****************************************************************************
            //  Check Parameters if wish from cardnumber masked to full 
            // 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "266";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                // IF YES THEN UPDATE MASK CARD WITH FULL 

                if (NumberOfTables == 2)
                {
                    // Second File = Destination File ... SourceTable_B
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_B);

                }

                if (NumberOfTables == 3)
                {
                    // Second File = Destination File ... SourceTable_C
                    UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_C);
                }

            }
        }
        //
        int NumberOfDublicates;
        int NumberOfUnmatched;
        string MatchingFields;
        int WPosA;
        int WPosB; // If NOT FOUND IN THIS POSITION

        string TableX;
        string TableY;

        string ListMatchingFields;
        string OnMatchingFields;
        //
        // TABLE UNMATCHED FIELDS DEFINITION
        //
        private void UnmatchedTableFieldsDefinition()
        {
            TableUnMatched.Columns.Add("UserId", typeof(string));
            TableUnMatched.Columns.Add("OriginSeqNo", typeof(int));
            TableUnMatched.Columns.Add("TransDate", typeof(DateTime));

            TableUnMatched.Columns.Add("WCase", typeof(string));
            TableUnMatched.Columns.Add("Type", typeof(int));
            TableUnMatched.Columns.Add("DublInPos", typeof(int));
            TableUnMatched.Columns.Add("InPos", typeof(int));
            TableUnMatched.Columns.Add("NotInPos", typeof(int));
            TableUnMatched.Columns.Add("TerminalId", typeof(string));
            TableUnMatched.Columns.Add("TraceNo", typeof(int));
            TableUnMatched.Columns.Add("AccNo", typeof(string));
            TableUnMatched.Columns.Add("TransAmt", typeof(decimal));
            TableUnMatched.Columns.Add("MatchingCateg", typeof(string));
            TableUnMatched.Columns.Add("RMCycle", typeof(int));
            TableUnMatched.Columns.Add("FileId", typeof(string));
        }

        //*************************************
        // MAKE MATCHING FOR THREE WORKING FILES 
        //*************************************
        public void MakeMatchingOf_3_Working_Tables
            (string InOperator, string InSignedId,
                  string InMatchingCateg, string InReconcCateg,
                                        int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;
            // Table Fields Definition
            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            //string WCase;

            // Dublicates 
            // Dublicates in A : Journal
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            //// Dublicates in B : Switch
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            // Dublicates in C : BANKING File 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "Dublicate In File03";
            WPos = 3;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified  :"
                                                  + NumberOfDublicates;

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Do Matching based on the created working files

            // STAGE A
            // In A and Not In B
            // In B and Not In A 

            // STAGE B
            // In B and Not In C
            // In C and Not In B

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            // ******************************
            // In B and Not In A
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File02 And Not In File01";
            WPosA = 2;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            // ******************************
            // In B and Not In C
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "In File02 And Not In File03";
            WPosA = 2;
            WPosB = 3; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            // ******************************
            // In C and Not In B
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File03 And Not In File02";

            WPosA = 3;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);
            //
            // SHOW MESSAGES FOR UNMATCHED
            //
            if (NumberOfUnmatched > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;

                Message = "UNMATCHED IDENTIFIED :"
                                                  + NumberOfUnmatched.ToString();

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }
            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records ";

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 
            Tr.DeleteReport67(WSignedId);
            //
            //Insert Records For Report WReport67
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionString))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport67]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    CatchDetails(ex);
                }


            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies

            //
            //Insert Records For MatchingDiscrepancies - Level 1
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionString))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    CatchDetails(ex);
                }
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {

                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    ReadExceptionsAndCompressAndUpdatefor_3_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                //if (FromOwnATMs == true)
                //{
                // OUR ATMs + JCC

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                //}
                //else
                //{
                //    // NOT OUR ATMs 

                //    ReadPrimaryAndFindDiscrepanciesfromMatching_Not_OurATMs(MatchingCateg, RMCycle);

                //}

            }

            #endregion
        }

        //*************************************
        // MAKE MATCHING FOR TWO WORKING FILES 
        //*************************************
        public void MakeMatchingOf_2_Working_Tables
          (string InOperator, string InSignedId,
                string InMatchingCateg, string InReconcCateg,
                                      int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;

            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            string FileIdA;
            string FileIdB;
            //string WCase;

            // Dublicates 
            // Dublicates in A : 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            // Dublicates in B : 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified :"
                                                  + NumberOfDublicates.ToString();

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }

            #endregion

            #region Do Matching based on the created working files

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            // ******************************
            // In B and Not In A
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File02 And Not In File01";
            WPosA = 2;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            if (NumberOfUnmatched > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;
                Message = "UnMatched identified : "
                                                  + NumberOfUnmatched.ToString();

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }
            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records found. ";

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 
            Tr.DeleteReport67(WSignedId);

            //Insert Records For Report WReport67
            using (SqlConnection conn2 =
                           new SqlConnection(connectionString))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport67]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    CatchDetails(ex);
                }

            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies
            //Insert Records For MatchingDiscrepancies
            using (SqlConnection conn2 =
                           new SqlConnection(connectionString))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }

                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    CatchDetails(ex);
                }
            //
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            //
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    ReadExceptionsAndCompressAndUpdatefor_2_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                //if (FromOwnATMs == true)
                //{
                // OUR ATMs And JCC

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                //}
                //else
                //{
                //    // NOT OUR ATMs 

                //    ReadPrimaryAndFindDiscrepanciesfromMatching_Not_OurATMs(MatchingCateg, RMCycle);

                //}

            }
            #endregion
        }

        //
        // Methods 
        // Find Duplicate file X 
        // FILL UP A TABLE
        // GROUP OF ATMS / Category 
        public void FindDuplicateAddTable(string InFileId, string InCase, int InPos, string InListMatchingFields, string IN_OnMatchingFields)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString =
       " SELECT * "
       //+ " y.SeqNo, y.MatchingCateg, y.RMCycle, y.TerminalId, y.TraceNo, y.AccNo, y.TransAmt, y.TransDate "
       + " FROM " + InFileId + " y"
       + " INNER JOIN "
       + " ( "
       + " SELECT "
       + InListMatchingFields + " , COUNT(*) AS CountOf"
       //  + " TerminalId, TraceNo, AccNo,  TransAmt, TransDate, COUNT(*) AS CountOf"
       + " FROM  " + InFileId
       //+ " GROUP BY TerminalId, TraceNo, AccNo, TransAmt, TransDate "
       + " GROUP BY " + InListMatchingFields
       + " HAVING COUNT(*)>1 "
       + " ) dt"
       + " ON " + IN_OnMatchingFields
       //+ " ON y.TerminalId=dt.TerminalId AND y.TraceNo= dt.TraceNo AND y.AccNo=dt.AccNo "
       //     + " AND y.TransAmt=dt.TransAmt AND y.TransDate =dt.TransDate "
       + " ";

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WorkingTableFields(rdr);

                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfDublicates = NumberOfDublicates + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["WCase"] = InCase;
                            RowSelected["Type"] = 1; // Dublicate
                            RowSelected["DublInPos"] = InPos;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = 0;
                            RowSelected["TerminalId"] = TerminalId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["TraceNo"] = TraceNo;
                            }
                            else
                            {
                                // Not From Our ATMs 
                                RowSelected["TraceNo"] = RRNumber;
                            }
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = InFileId;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);

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


        // Find Exist In A But Not In B 
        public void FindNotExistAddTableTableXToTableY(string InFileIdA,
                                string InFileIdB, string InCase, int InPos, int NotInPos, string InMatchingFields)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString =

            " SELECT * "
             + " FROM " + InFileIdA + " c1"
             + " WHERE NOT EXISTS(SELECT * "
             + " FROM " + InFileIdB + " c2 "
             + " WHERE " + InMatchingFields + ")";

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            WorkingTableFields(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfUnmatched = NumberOfUnmatched + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;

                            RowSelected["WCase"] = InCase;
                            RowSelected["Type"] = 4; // Not In 
                            RowSelected["DublInPos"] = 0;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = NotInPos;
                            RowSelected["TerminalId"] = TerminalId;
                            if (FromOwnATMs == true)
                            {
                                RowSelected["TraceNo"] = TraceNo;
                            }
                            else
                            {
                                // Not From Our ATMs 
                                RowSelected["TraceNo"] = RRNumber;
                            }
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["TransAmt"] = TransAmt;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = TableId;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);

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


        bool ZeroInOne;
        bool ZeroInTwo;
        bool ZeroInThree;
        int LastTraceNo = 0;
        string LastTerminalId = "";
        string LastAccNo;
        decimal LastTransAmt;
        int LastType;
        DateTime LastTransDate;

        bool NotSameAccount;
        bool NotSameAmt;

        int FirstSeqNo;

        bool FoundInOne;
        bool FoundInTwo;
        bool FoundInThree;
        int InPos;

        // Read detail umpatched Cases and Summarise them and Update (2  Files) 
        public void ReadExceptionsAndCompressAndUpdatefor_2_Tables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool CheckAccNo = false;

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "717";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES") CheckAccNo = true;

            FoundInOne = false;
            FoundInTwo = false;

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;

            TotalSelected2 = 0;

            SqlString =
            " SELECT  *"
            + " FROM [ATMS].[dbo].[WReport67] "
            + " WHERE Type = 4 AND UserId = @UserId "
            + " ORDER By TerminalId, TransDate, traceno, WCase, NotInPos ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //////ReadTable And Insert In Sql Table 
            //RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            ////Clear Table 
            //Tr.DeleteReport67(WSignedId);

            int I = 0;
            int K = TableUnMatched.Rows.Count;

            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;
                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];

                if ((TraceNo != LastTraceNo)
                    || AccNo != LastAccNo
                    || TransAmt != LastTransAmt
                    || I == K - 1)
                {
                    if (K == 1) FirstSeqNo = SeqNo;

                    if (CheckAccNo == true)
                    {
                        if (AccNo != LastAccNo & TraceNo == LastTraceNo)
                        {

                            NotSameAccount = true;
                        }
                    }

                    if (TransAmt != LastTransAmt & TraceNo == LastTraceNo)
                    {
                        NotSameAmt = true;
                    }

                    if ((LastTraceNo > 0 & TraceNo != LastTraceNo)) //  
                    {

                        char[] WMask = { '0', '0' };
                        if (FoundInOne == true) WMask[0] = '1';
                        if (FoundInTwo == true) WMask[1] = '1';
                        //if (ZeroInThree == true) WMask[2] = '0';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            WWMask = "AA";
                        }

                        // Our ATMs and cards
                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                        FoundInOne = false;
                        FoundInTwo = false;


                        NotSameAccount = false;
                        NotSameAmt = false;

                    }

                    if (I == K - 1) // LAST RECORD
                    {
                        if (InPos == 1) FoundInOne = true;
                        if (InPos == 2) FoundInTwo = true;

                        LastTraceNo = TraceNo;
                        LastTransDate = TransDate;
                        LastTerminalId = TerminalId;
                        LastAccNo = AccNo;
                        LastTransAmt = TransAmt;
                        LastType = Type;

                        char[] WMask = { '0', '0' };
                        if (FoundInOne == true) WMask[0] = '1';
                        if (FoundInTwo == true) WMask[1] = '1';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            WWMask = "AA";
                        }
                        // LAST RECORD

                        // OUR ATMs AND JCC
                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                    }

                    if (I != K - 1) FirstSeqNo = SeqNo;
                    LastTraceNo = TraceNo;
                    LastTransDate = TransDate;
                    LastTerminalId = TerminalId;
                    LastAccNo = AccNo;
                    LastTransAmt = TransAmt;
                    LastType = Type;
                }

                if (I != K - 1) // If not LAST RECORD
                {
                    if (InPos == 1) FoundInOne = true;
                    if (InPos == 2) FoundInTwo = true;
                }

                I++; // Read Next entry of the table 

            }

        }

        // Read detail umpatched Cases and Summarise them and Update (3  Tables) 
        public void ReadExceptionsAndCompressAndUpdatefor_3_Tables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool CheckAccNo = false;

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "717";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES") CheckAccNo = true;

            FoundInOne = false;
            FoundInTwo = false;
            FoundInThree = false;
            ZeroInOne = false;
            ZeroInTwo = false;
            ZeroInThree = false;

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;

            TotalSelected2 = 0;

            SqlString =
            " SELECT  *"
            + " FROM [ATMS].[dbo].[WReport67] "
            + " WHERE Type = 4 AND UserId = @UserId "
            + " ORDER By TerminalId, traceno, WCase, NotInPos ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //////ReadTable And Insert In Sql Table 

            int I = 0;
            int K = TableUnMatched.Rows.Count;

            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;
                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                //string LastAccNo;
                //decimal LastTransAmt;

                if (TraceNo != LastTraceNo
                    || AccNo != LastAccNo
                    || TransAmt != LastTransAmt
                    || I == K - 1
                    //|| TerminalId != LastTerminalId
                    )
                {
                    if (K == 1) FirstSeqNo = SeqNo;

                    // This condition is defined in parameter 717
                    if (CheckAccNo == true)
                    {
                        if (AccNo != LastAccNo & TraceNo == LastTraceNo)
                        {
                            NotSameAccount = true;
                        }
                    }

                    //if (TraceNo == 7823)
                    //{
                    //    System.Windows.Forms.MessageBox.Show("Trace :" + TraceNo.ToString());
                    //}

                    if (TransAmt != LastTransAmt & TraceNo == LastTraceNo)
                    {
                        NotSameAmt = true;
                    }

                    if ((LastTraceNo > 0 & TraceNo != LastTraceNo)) //  
                    {

                        //if (InPos == 1) FoundInOne = true;
                        //if (InPos == 2) FoundInTwo = true;
                        //if (InPos == 3) FoundInThree = true;
                        //if (NotInPos == 1) ZeroInOne = true;
                        //if (NotInPos == 2) ZeroInTwo = true;
                        //if (NotInPos == 3) ZeroInThree = true;

                        if (FoundInOne || FoundInTwo || FoundInThree
                                            || ZeroInOne || ZeroInTwo || ZeroInThree
                                            & LastTransAmt > 0)
                        { }

                        if (FoundInOne == true & ZeroInOne == true)
                        {
                            ZeroInOne = false;
                        }
                        if (FoundInTwo == true & ZeroInTwo == true)
                        {
                            ZeroInTwo = false;
                        }
                        if (FoundInThree == true & ZeroInThree == true)
                        {
                            ZeroInThree = false;
                        }

                        char[] WMask = { '0', '0', '0' };

                        if (FoundInOne == true || (ZeroInOne == false & ZeroInTwo == false))
                            WMask[0] = '1';

                        if (FoundInTwo == true || ZeroInTwo == false) WMask[1] = '1';

                        if (FoundInThree == true || (ZeroInThree == false & ZeroInTwo == false))
                            WMask[2] = '1';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            if (WWMask == "111") WWMask = "AAA";
                            if (WWMask == "101") WWMask = "A0A";
                            if (WWMask == "011") WWMask = "0AA";
                        }

                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                        //
                        // INITIALISE VARIABLES
                        //
                        FoundInOne = false;
                        FoundInTwo = false;
                        FoundInThree = false;
                        ZeroInOne = false;
                        ZeroInTwo = false;
                        ZeroInThree = false;

                        NotSameAccount = false;
                        NotSameAmt = false;

                    }
                    if (I == K - 1) // LAST RECORD
                    {
                        if (InPos == 1) FoundInOne = true;
                        if (InPos == 2) FoundInTwo = true;
                        if (InPos == 3) FoundInThree = true;
                        if (NotInPos == 1) ZeroInOne = true; // We set Found In one = false to cover cases of differrent amount
                        if (NotInPos == 2) ZeroInTwo = true;
                        if (NotInPos == 3) ZeroInThree = true;

                        if (FoundInOne == true & ZeroInOne == true)
                        {
                            ZeroInOne = false;
                        }
                        if (FoundInTwo == true & ZeroInTwo == true)
                        {
                            ZeroInTwo = false;
                        }
                        if (FoundInThree == true & ZeroInThree == true)
                        {
                            ZeroInThree = false;
                        }

                        FirstSeqNo = SeqNo;
                        LastTraceNo = TraceNo;
                        LastTransDate = TransDate;
                        LastTerminalId = TerminalId;
                        LastAccNo = AccNo;
                        LastTransAmt = TransAmt;
                        LastType = Type;

                        char[] WMask = { '0', '0', '0' };

                        if (FoundInOne == true || (ZeroInOne == false & ZeroInTwo == false))
                            WMask[0] = '1';

                        if (FoundInTwo == true || ZeroInTwo == false)
                            WMask[1] = '1';

                        if (FoundInThree == true || (ZeroInThree == false & ZeroInTwo == false))
                            WMask[2] = '1';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            if (WWMask == "111") WWMask = "AAA";
                            if (WWMask == "101") WWMask = "A0A";
                            if (WWMask == "011") WWMask = "0AA";
                        }

                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                    }

                    // SAVE PREVIOUS 
                    if (I != K - 1) FirstSeqNo = SeqNo;
                    LastTraceNo = TraceNo;
                    LastTransDate = TransDate;
                    LastTerminalId = TerminalId;
                    LastAccNo = AccNo;
                    LastTransAmt = TransAmt;
                    LastType = Type;
                }


                if (InPos == 1) FoundInOne = true;
                if (InPos == 2) FoundInTwo = true;
                if (InPos == 3) FoundInThree = true;

                if (NotInPos == 1) ZeroInOne = true; // We set Found In one = false to cover cases of differrent amount
                if (NotInPos == 2) ZeroInTwo = true;
                if (NotInPos == 3) ZeroInThree = true;

                I++; // Read Next entry of the table 

            }

        }

        // Insert and Update exceptions  
        private void InsertAndUpdateTablesFromOurAtms_AND_JCC(string WWMask)
        {
            // Insert Or Update Footer 

            if (MatchingCateg == "NBG333")
            {
                string maskpanicos = "000";
            }

            string WSelectionCriteria = "";
            // READ DETAILS FROM OTHER WORKING TABLES 
            if (WWMask == "10"
                || WWMask == "101"
                || WWMask == "110"
                || WWMask == "100"
                || WWMask == "AA"
                || WWMask == "AAA"
                || WWMask == "A0A"
                )
            {
                Mpa.RecordFound = false;
                if (FromOwnATMs == true)
                {
                    SelectionCriteria = " WHERE  MatchingCateg ='" + MatchingCateg + "'"
                                  + " AND  MatchingAtRMCycle =" + RMCycle
                                  + " AND  TraceNoWithNoEndZero =" + LastTraceNo
                                  + " AND  TerminalId ='" + LastTerminalId + "'";
                }
                else
                {
                    SelectionCriteria = " WHERE  MatchingCateg ='" + MatchingCateg + "'"
                                  + " AND  MatchingAtRMCycle =" + RMCycle
                                  + " AND  RRNumber =" + LastTraceNo;
                }

                Mpa.ReadMatchingTxnsMasterPoolFirstRecordFound(SelectionCriteria,1);

                if (Mpa.RecordFound == true)
                {
                    //SelectionCriteria = " WHERE  SeqNo =" + OriginalSeqNo;
                    //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria);

                    if (WWMask == "110")
                    {
                        // GET INFO from corresonding fields 
                        //
                        // Take From Second
                        TableId = SourceTable_B; // Switch
                        WSelectionCriteria = " WHERE TerminalId ='" + LastTerminalId + "'"
                                                       + " AND  ProcessedAtRMCycle =" + RMCycle
                                                       + " AND TraceNo =" + LastTraceNo;

                        Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, TableId);

                        Mpa.CardNumber = Mgt.CardNumber;
                        Mpa.AccNumber = Mgt.AccNo;

                        Mpa.UpdateTranWithCard_Account(Mpa.UniqueRecordId,1);

                    }

                    Mpa.Matched = false;
                    Mpa.MatchMask = WWMask;
                    Mpa.MatchedType = "No Match";

                    if (WWMask == "AA" || WWMask == "AAA" || WWMask == "A0A")
                    {

                        if (NotSameAccount == true)
                        {
                            Mpa.UnMatchedType = "Alert-Not Same Account";
                        }
                        if (NotSameAmt == true)
                        {
                            Mpa.UnMatchedType = "Alert-Not Same Amount";
                        }
                        if (NotSameAccount == true & NotSameAmt == true)
                        {
                            Mpa.UnMatchedType = "Alert-Not Same Account And Amount";
                        }



                    }
                    else
                    {
                        Mpa.UnMatchedType = "Not Matched";
                    }

                    Mpa.SystemMatchingDtTm = DateTime.Now;

                    Mpa.UserId = "";
                    Mpa.ActionByUser = false;
                    Mpa.Authoriser = "";

                    Mpa.SettledRecord = false;

                    Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId,1);
                }
            }

            //
            // First Journal record Missing 
            //  
            //
            if (WWMask == "01"
                  || WWMask == "011"
                  || WWMask == "010"
                  || WWMask == "001"
                  || WWMask == "0AA"
                  )
            {
                if (WWMask == "01")
                {
                    TableId = SourceTable_B;

                }
                if (WWMask == "011" || WWMask == "0AA")
                {
                    // Take From Target 
                    TableId = SourceTable_C;
                }
                if (WWMask == "010")
                {
                    // Take From Second
                    TableId = SourceTable_B;
                }
                if (WWMask == "001")
                {
                    // Take From Target 
                    TableId = SourceTable_C;
                }

                // Find Out the Replenishment Cycle No by using Last Trace Number

                int WReplCycleNo = Mpa.ReadMatchingTxnsMasterPoolFirstRecordLessThanGivenTraceNumber(LastTerminalId, LastTraceNo);

                // GET INFO from corresonding fields 
                //
                WSelectionCriteria = " WHERE TerminalId ='" + LastTerminalId + "'"
                                               + " AND  ProcessedAtRMCycle =" + RMCycle
                                               + " AND TraceNo =" + LastTraceNo;

                Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, TableId);

                int SeqNoTwo = Mgt.SeqNo;
                //ReadWorkingFileBySelectionCriteria(TableId, WSelectionCriteria, LastTransDate);

                Mpa.OriginFileName = "ATM:" + LastTerminalId + " Journal";

                Mpa.LoadedAtRMCycle = WRMCycle;

                Mpa.MatchingAtRMCycle = WRMCycle;

                Mpa.OriginalRecordId = Mgt.OriginalRecordId;
                Mpa.MatchingCateg = WMatchingCategoryId;

                RRDMAtmsClass Ac = new RRDMAtmsClass();
                Ac.ReadAtm(LastTerminalId);

                Mpa.RMCateg = "RECATMS-" + Ac.AtmsReconcGroup.ToString();

                Mpa.MatchingAtRMCycle = RMCycle;
                Mpa.UniqueRecordId = GetNextValue(connectionString);

                Mpa.Origin = WOrigin;

                Mpa.TerminalType = Mgt.TerminalType;

                Mpa.TargetSystem = 6;
                Mpa.TransTypeAtOrigin = Mgt.TransTypeAtOrigin;
                Mpa.Product = "";
                Mpa.CostCentre = "";
                Mpa.TerminalId = Mgt.TerminalId;
                Mpa.TransType = Mgt.TransType;
                Mpa.TransDescr = Mgt.TransDescr;
                Mpa.CardNumber = Mgt.CardNumber;

                Mpa.AccNumber = Mgt.AccNo;

                Mpa.TransCurr = Mgt.TransCurr;
                Mpa.TransAmount = Mgt.TransAmt;
                Mpa.DepCount = 0;
                Mpa.TransDate = Mgt.TransDate;

                if (FromOwnATMs == true)
                {
                    Mpa.TraceNoWithNoEndZero = LastTraceNo;
                    Mpa.AtmTraceNo = LastTraceNo * 10;
                    Mpa.MasterTraceNo = LastTraceNo * 10;

                    RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs();

                    Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, Mpa.CardNumber.Substring(0, 6), "Our Atms", 12);
                    if (Mcb.RecordFound)
                    {
                        // Own Card

                        Mpa.IsOwnCard = true;
                    }
                    else
                    {
                        // Other Card 

                        Mpa.IsOwnCard = false;
                    }
                }
                else
                {
                    Mpa.TraceNoWithNoEndZero = 0;
                    Mpa.AtmTraceNo = 0;
                    Mpa.MasterTraceNo = 0;
                    RRDMMatchingCategoriesVsBINs Mcb = new RRDMMatchingCategoriesVsBINs();

                    Mcb.ReadMatchingCategoriesVsBINsBySelectionCriteria(WOperator, 0, Mpa.CardNumber.Substring(0, 6), "Our Atms", 12);
                    if (Mcb.RecordFound)
                    {
                        // Own Card

                        Mpa.IsOwnCard = true;
                    }
                    else
                    {
                        // Other Card 

                        Mpa.IsOwnCard = false;
                    }
                }

                Mpa.RRNumber = Mgt.RRNumber;

                Mpa.ResponseCode = "";
                Mpa.SpareField = "";
                Mpa.Comments = "";

                Mpa.IsMatchingDone = true;
                Mpa.Matched = false;
                Mpa.MatchMask = WWMask;
                Mpa.SystemMatchingDtTm = DateTime.Now;
                Mpa.MatchedType = "No Match";
                if (WWMask == "0AA")
                {

                    if (NotSameAccount == true)
                    {
                        Mpa.UnMatchedType = "Alert-Not Same Account";
                    }
                    if (NotSameAmt == true)
                    {
                        Mpa.UnMatchedType = "Alert-Not Same Amount";
                    }
                    if (NotSameAccount == true & NotSameAmt == true)
                    {
                        Mpa.UnMatchedType = "Alert-Not Same Account And Amount";
                    }

                }
                else
                {
                    Mpa.UnMatchedType = "Not Matched";
                }

                Mpa.UnMatchedType = "";
                Mpa.MetaExceptionId = 0;
                Mpa.MetaExceptionNo = 0;
                Mpa.FastTrack = false;
                Mpa.ActionByUser = false;
                Mpa.UserId = "";
                Mpa.Authoriser = "";
                Mpa.AuthoriserDtTm = DateTime.Now;
                Mpa.ActionType = "";
                Mpa.NotInJournal = true;
                Mpa.WaitingForUpdating = false;
                Mpa.SettledRecord = false;
                Mpa.Operator = WOperator;

                Mpa.FileId01 = Mcsf.SourceFileNameA;
                Mpa.SeqNo01 = 0;
                Mpa.FileId02 = Mcsf.SourceFileNameB;
                Mpa.SeqNo02 = 0;
                Mpa.FileId03 = Mcsf.SourceFileNameC;
                Mpa.SeqNo03 = 0;
                Mpa.FileId04 = Mcsf.SourceFileNameD;
                Mpa.SeqNo04 = 0;
                Mpa.FileId05 = "";
                Mpa.SeqNo05 = 0;
                Mpa.FileId06 = "";
                Mpa.SeqNo06 = 0;

                // Find the right (proper cycle number)


                Mpa.ReplCycleNo = WReplCycleNo; // THIS CYCLE NO WAS FOUND FROM PREVIOUS TRACE ... See Code Above

                int NewSeqNo = Mpa.InsertTransMasterPoolATMs(Mpa.Operator);

                // UPDATE INSERTED
                SelectionCriteria = " WHERE  SeqNo =" + NewSeqNo;
                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                Mpa.IsMatchingDone = true;
                Mpa.Matched = false;
                Mpa.MatchMask = WWMask;
                Mpa.MatchedType = "NotInJournal-by System";
                Mpa.SystemMatchingDtTm = DateTime.Now;

                //Mpa.TraceNoWithNoEndZero = LastTraceNo;

                Mpa.UserId = "";
                Mpa.ActionByUser = false;
                Mpa.Authoriser = "";

                Mpa.NotInJournal = true;

                Mpa.SettledRecord = false;

                Mpa.UpdateMatchingTxnsMasterPoolATMsFooter(Mpa.Operator, Mpa.UniqueRecordId,1);
            }
            // READ DETAILS




        }
        //// Insert and Update exceptions  
        //private void InsertAndUpdateTablesFrom_NOT_OurAtms(string WWMask)
        //{

        //    // 
        //    if (WWMask == "10"
        //       || WWMask == "101"
        //       || WWMask == "110"
        //       || WWMask == "100"
        //       )

        //    {
        //        // A: Read From Primary the associated Record

        //        WSelectionCriteria = " WHERE "
        //                                   + " ProcessedAtRMCycle =" + RMCycle
        //                                   + " AND RRNumber =" + LastTraceNo;

        //        Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, SourceTable_A);

        //        int SeqNoOne = Mgt.SeqNo;

        //        //*******************************************
        //        // B: Set up and Insert Exception
        //        //******************************************
        //        Mre.UniqueRecordId = GetNextValue(connectionString);
        //        Mre.MatchingCateg = WMatchingCategoryId;
        //        Mpa.LoadedAtRMCycle = WRMCycle;
        //        Mre.MatchingAtRMCycle = WRMCycle;
        //        Mre.Matched = false;

        //        Mre.MatchMask = WWMask;

        //        Mre.SystemMatchingDtTm = DateTime.Now;
        //        Mre.MatchedType = "";
        //        Mre.UnMatchedType = "";
        //        Mre.MetaExceptionId = 0;

        //        Mre.MetaExceptionNo = 0;
        //        Mre.ActionByUser = false;
        //        Mre.UserId = "";
        //        Mre.Authoriser = "";

        //        Mre.AuthoriserDtTm = NullPastDate;
        //        Mre.ActionType = "0";

        //        Mre.SettledRecord = false;

        //        Mre.Operator = WOperator;

        //        Mre.Table01 = SourceTable_A;
        //        Mre.Table02 = SourceTable_B;
        //        Mre.Table03 = "";
        //        Mre.Table04 = "";

        //        int InsertRecordId = Mre.InsertMatchingReconcExceptionsInfo();

        //        //*************************************************
        //        // C: Update Footer of Tables
        //        //*************************************************

        //        Mgt.Mask = WWMask;
        //        Mgt.ItHasException = true;
        //        Mgt.UniqueRecordId = Mre.UniqueRecordId;

        //        Mgt.UpdateSourceTablesFooter(SeqNoOne, SourceTable_A);

        //        //Mgt.UpdateSourceTablesFooter(SeqNoTwo, SourceTable_B);

        //    }
        //    if (WWMask == "01"
        //          || WWMask == "011"
        //          || WWMask == "010"
        //          || WWMask == "001"
        //          )
        //    {
        //        if (WWMask == "01")
        //        {
        //            TableId = SourceTable_B;

        //        }
        //        if (WWMask == "011")
        //        {
        //            // Take From Target 
        //            TableId = SourceTable_C;
        //        }
        //        if (WWMask == "010")
        //        {
        //            // Take From Second
        //            TableId = SourceTable_B;
        //        }
        //        if (WWMask == "001")
        //        {
        //            // Take From Target 
        //            TableId = SourceTable_C;
        //        }
        //        // A: Read From Secondary the associated Record

        //        WSelectionCriteria = " WHERE "
        //                               + " ProcessedAtRMCycle =" + RMCycle
        //                               + " AND RRNumber =" + LastTraceNo;

        //        Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, TableId);

        //        int SeqNoTwo = Mgt.SeqNo;
        //        //*****************************************
        //        // Insert Corresponding Record in Primary
        //        //*****************************************
        //        int SeqNoOne = Mgt.InsertRecordInPrimaryTable(SourceTable_A);


        //        //*******************************************
        //        // B: Set up and Insert Exception
        //        //******************************************
        //        Mre.UniqueRecordId = GetNextValue(connectionString);
        //        Mre.MatchingCateg = WMatchingCategoryId;
        //        Mre.MatchingAtRMCycle = WRMCycle;
        //        Mre.Matched = false;

        //        Mre.MatchMask = WWMask;

        //        Mre.SystemMatchingDtTm = DateTime.Now;
        //        Mre.MatchedType = "";
        //        Mre.UnMatchedType = "Missing Record/s";
        //        Mre.MetaExceptionId = 0;

        //        Mre.MetaExceptionNo = 0;
        //        Mre.ActionByUser = false;
        //        Mre.UserId = "";
        //        Mre.Authoriser = "";

        //        Mre.AuthoriserDtTm = NullPastDate;
        //        Mre.ActionType = "0";

        //        Mre.SettledRecord = false;

        //        Mre.Operator = WOperator;

        //        Mre.Table01 = SourceTable_A;
        //        Mre.Table02 = SourceTable_B;
        //        Mre.Table03 = "";
        //        Mre.Table04 = "";

        //        //
        //        // Insert Exception
        //        //

        //        int InsertRecordId = Mre.InsertMatchingReconcExceptionsInfo();

        //        //*************************************************
        //        // C: Update Footer of Tables
        //        //*************************************************

        //        Mgt.Mask = WWMask;
        //        Mgt.ItHasException = true;
        //        Mgt.UniqueRecordId = Mre.UniqueRecordId;

        //        Mgt.UpdateSourceTablesFooter(SeqNoOne, SourceTable_A);

        //        Mgt.UpdateSourceTablesFooter(SeqNoTwo, SourceTable_B);

        //    }



        //}

        int CounterDublInPosOne;
        int CounterDublInPosTwo;
        int CounterDublInPosThree;
        //*****************************************
        // UPDATE Dublicates For Two OR three 
        //***************************************** 
        public void ReadDublicatesAndCompressAndUpdateExceptionsforTables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TotalSelected2 = 0;

            SqlString =
            " SELECT  *"
            + " FROM [ATMS].[dbo].[WReport67] " // leave it as is 
            + " WHERE Type = 1 AND UserId = @UserId "              /* Dublicate  */
            + " ORDER By TerminalId, traceno, WCase ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            LastTraceNo = 0;
            LastTerminalId = "";
            int I = 0;
            int K = TableUnMatched.Rows.Count;
            // READ AND UPDATE Mpa
            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;
                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                Type = (int)TableUnMatched.Rows[I]["Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmt = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];

                if (TraceNo != LastTraceNo || I == K - 1)
                {
                    if (K == 1) FirstSeqNo = SeqNo;

                    if ((LastTraceNo > 0 & TraceNo != LastTraceNo)) //    
                    {
                        // Do action for previous 

                        // OUR ATMs
                        UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC();



                        CounterDublInPosOne = 0;
                        CounterDublInPosTwo = 0;
                        CounterDublInPosThree = 0;

                    }


                    if (I == K - 1) // last record in table 
                    {
                        // It is last of a group with same trace no 
                        // Or the only one 

                        if (DublInPos == 1) CounterDublInPosOne = CounterDublInPosOne + 1;
                        if (DublInPos == 2) CounterDublInPosTwo = CounterDublInPosTwo + 1;
                        if (DublInPos == 3) CounterDublInPosThree = CounterDublInPosThree + 1;

                        LastTraceNo = TraceNo;
                        LastTransDate = TransDate;
                        LastTerminalId = TerminalId;
                        //LastAccNo = AccNo;
                        LastTransAmt = TransAmt;
                        LastType = Type;


                        // OUR ATMs
                        UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC();



                    }


                    if (I != K - 1) FirstSeqNo = SeqNo;
                    LastTraceNo = TraceNo;
                    LastTransDate = TransDate;
                    LastTerminalId = TerminalId;
                    //LastAccNo = AccNo;
                    LastTransAmt = TransAmt;
                    LastType = Type;
                }
                else
                {

                }

                if (DublInPos == 1) CounterDublInPosOne = CounterDublInPosOne + 1;
                if (DublInPos == 2) CounterDublInPosTwo = CounterDublInPosTwo + 1;
                if (DublInPos == 3) CounterDublInPosThree = CounterDublInPosThree + 1;

                I++; // Read Next entry of the table 

            }
            ////
            //// CREATE COPRESSED TABLE
            //// 

            //if (FromOwnATMs == true)
            //{
            //    // OUR ATMs

            //    ReadMpaAndFindDiscrepanciesfromMatching_OurATMs(MatchingCateg, RMCycle);

            //}
            //else
            //{
            //    // NOT OUR ATMs 

            //    ReadMpaAndFindDiscrepanciesfromMatching_Not_OurATMs(MatchingCateg, RMCycle);

            //}


        }
        //
        // Update Primary Table with dublicates - FROM Our ATMs 
        //
        private void UpdateDuplicatesInPrimaryTableFromOurATMs_And_JCC()
        {
            // UPDATE Mpa
            //
            string SelectionCriteria2 = "";

            Mpa.RecordFound = false;
            if (FromOwnATMs == true)
            {
                SelectionCriteria2 = " WHERE  MatchingCateg ='" + MatchingCateg + "'"
                                              + " AND  MatchingAtRMCycle =" + RMCycle
                                              + " AND  TraceNoWithNoEndZero =" + LastTraceNo
                                              + " AND  TerminalId ='" + LastTerminalId + "'";
            }
            else
            {
                SelectionCriteria2 = " WHERE  MatchingCateg ='" + MatchingCateg + "'"
                                              + " AND  MatchingAtRMCycle =" + RMCycle
                                              + " AND  RRNumber =" + LastTraceNo;
            }

            Mpa.ReadMatchingTxnsMasterPoolFirstRecordFound(SelectionCriteria2,1);

            if (Mpa.RecordFound == false)
            {
                Message = " 34356: There is an error.. record not found.";

                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            //if (Mpa.MatchMask == "")
            //{
            //    if (NumberOfTables == 2) Mpa.MatchMask = "11";
            //    if (NumberOfTables == 2) Mpa.MatchMask = "111";
            //}

            char[] WMask = Mpa.MatchMask.ToCharArray();

            if (CounterDublInPosOne == 2) WMask[0] = '2';
            if (CounterDublInPosOne == 3) WMask[0] = '3';
            if (CounterDublInPosOne == 4) WMask[0] = '4';
            if (CounterDublInPosOne == 5) WMask[0] = '5';

            if (CounterDublInPosTwo == 2) WMask[1] = '2';
            if (CounterDublInPosTwo == 3) WMask[1] = '3';
            if (CounterDublInPosTwo == 4) WMask[1] = '4';
            if (CounterDublInPosTwo == 5) WMask[1] = '5';

            if (CounterDublInPosThree == 2) WMask[2] = '2';
            if (CounterDublInPosThree == 3) WMask[2] = '3';
            if (CounterDublInPosThree == 4) WMask[2] = '4';
            if (CounterDublInPosThree == 5) WMask[2] = '5';

            string WWMask = new string(WMask);
            // Update Footer 

            SelectionCriteria = " WHERE  SeqNo =" + Mpa.SeqNo; // Basic information 
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

            Mpa.Matched = false;
            Mpa.MatchMask = WWMask;
            Mpa.MatchedType = "by System";
            Mpa.SystemMatchingDtTm = DateTime.Now;

            Mpa.UserId = "";
            Mpa.ActionByUser = false;
            Mpa.Authoriser = "";

            Mpa.SettledRecord = false;

            Mpa.UpdateMatchingTxnsMasterPoolATMsFooterBySelection(Mpa.MatchingCateg
                , Mpa.TerminalId
                , Mpa.TransAmount
                , Mpa.TraceNoWithNoEndZero
                , Mpa.RRNumber
                , Mpa.CardNumber
                , Mpa.TransDate.Date
                ,1 ); // Update all records 
        }

        ////
        //// Update Primary Table with dublicates - FROM NOT Our ATMs 
        ////
        //private void UpdateDuplicatesInPrimaryTableFrom_NOT_OurATMs(int InRRNumber)
        //{
        //    //*****************************************
        //    // UPDATE Primary Table 
        //    //*****************************************
        //    bool CreateException = false;
        //    int SeqNoPrimary;
        //    int UniqueRecordId;

        //    Mgt.RecordFound = false;

        //    // READ PRIMARY TABLE RECORD TO FIND MASK

        //    WSelectionCriteria = " WHERE "
        //                                  + " ProcessedAtRMCycle =" + RMCycle
        //                                  + " AND RRNumber =" + InRRNumber;

        //    Mgt.ReadTransSpecificFromSpecificWorking(WSelectionCriteria, SourceTable_A);

        //    SeqNoPrimary = Mgt.SeqNo;
        //    UniqueRecordId = Mgt.UniqueRecordId;

        //    //Mpa.ReadMatchingTxnsMasterPoolFirstRecordFound(SelectionCriteria, LastTransDate);
        //    if (Mgt.RecordFound == false)
        //    {
        //        System.Windows.Forms.MessageBox.Show(" 48595 : There is an error record not found.");
        //    }

        //    if (Mgt.Mask == "11" || Mgt.Mask == "111")
        //    {
        //        // Update Primary but create exception too 
        //        CreateException = true;
        //    }
        //    else
        //    {
        //        // Update only primary 
        //        CreateException = false;
        //    }

        //    char[] WMask = Mgt.Mask.ToCharArray();

        //    if (CounterDublInPosOne == 2) WMask[0] = '2';
        //    if (CounterDublInPosOne == 3) WMask[0] = '3';
        //    if (CounterDublInPosOne == 4) WMask[0] = '4';
        //    if (CounterDublInPosOne == 5) WMask[0] = '5';

        //    if (CounterDublInPosTwo == 2) WMask[1] = '2';
        //    if (CounterDublInPosTwo == 3) WMask[1] = '3';
        //    if (CounterDublInPosTwo == 4) WMask[1] = '4';
        //    if (CounterDublInPosTwo == 5) WMask[1] = '5';

        //    if (CounterDublInPosThree == 2) WMask[2] = '2';
        //    if (CounterDublInPosThree == 3) WMask[2] = '3';
        //    if (CounterDublInPosThree == 4) WMask[2] = '4';
        //    if (CounterDublInPosThree == 5) WMask[2] = '5';

        //    string WWMask = new string(WMask);

        //    if (CreateException == true)
        //    {
        //        //*******************************************
        //        // B: Set up and Insert Exception
        //        //******************************************

        //        UniqueRecordId = Mre.UniqueRecordId = GetNextValue(connectionString);
        //        Mre.MatchingCateg = WMatchingCategoryId;
        //        Mre.MatchingAtRMCycle = WRMCycle;
        //        Mre.Matched = false;

        //        Mre.MatchMask = WWMask;

        //        Mre.SystemMatchingDtTm = DateTime.Now;
        //        Mre.MatchedType = "";
        //        Mre.UnMatchedType = "Dublicates";
        //        Mre.MetaExceptionId = 0;

        //        Mre.MetaExceptionNo = 0;
        //        Mre.ActionByUser = false;
        //        Mre.UserId = "";
        //        Mre.Authoriser = "";

        //        Mre.AuthoriserDtTm = NullPastDate;
        //        Mre.ActionType = "0";

        //        Mre.SettledRecord = false;

        //        Mre.Operator = WOperator;

        //        Mre.Table01 = SourceTable_A;
        //        Mre.Table02 = SourceTable_B;
        //        Mre.Table03 = "";
        //        Mre.Table04 = "";

        //        //
        //        // Insert Exception
        //        //

        //        int InsertRecordId = Mre.InsertMatchingReconcExceptionsInfo();

        //    }

        //    //*************************************************
        //    // C: Update Footer of Tables
        //    //*************************************************

        //    Mgt.Mask = WWMask;
        //    Mgt.ItHasException = true;
        //    Mgt.UniqueRecordId = UniqueRecordId;

        //    Mgt.UpdateSourceTablesFooter(SeqNoPrimary, SourceTable_A);

        //}


        // Find Discrepancies from Our ATMs Mpa 
        //
        // public DataTable TableUnMatchedCompressed = new DataTable();
        public void ReadMpaAndFindDiscrepanciesfromMatching_Cards(string InMatchingCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;
            if (FromOwnATMs == true)
            {
                SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCateg + "'"
                                        + " AND MatchingAtRMCycle = " + RMCycle
                                        + " AND IsMatchingDone = 1 AND Matched = 0 "
                                        + " Order by TransDate , TraceNoWithNoEndZero";

                SqlString =
                " SELECT  SeqNo,  "
                + " CAST(TransDate AS Date) As Date,"
                + " TraceNoWithNoEndZero,  "
                + " Matched,  "
                + " MatchMask,  "
                + " TerminalId,  "
                + " TransDescr,  "
                + " AccNumber,  "
                + " TransAmount,  "
                + " SettledRecord,  "
                + " Comments  "
                //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                + " FROM " + SourceTable_A
                + SelectionCriteria;

            }
            else
            {
                SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCateg + "'"
                                        + " AND MatchingAtRMCycle = " + RMCycle
                                        + " AND IsMatchingDone = 1 AND Matched = 0 "
                                        + " Order by TransDate ";
                SqlString =
                " SELECT  SeqNo,  "
                + " CAST(TransDate AS Date) As Date,"
                + " RRNumber ,  "
                + " Matched,  "
                + " MatchMask,  "
                + " TransDescr,  "
                + " AccNumber,  "
                + " TransAmount,  "
                + " SettledRecord,  "
                + " Comments  "
                //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                + " FROM " + SourceTable_A
                + SelectionCriteria;
            }


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatchedCompressed);

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
        // READ Working File by Selection Criteria
        //
        //
        //
        //int TSeqNo;
        //string TMatchingCateg;
        //int TRMCycle;
        //string TTerminalId;
        //int TTransType;
        //string TTransDescr;
        //string TCardNumber;
        //string TAccNo;
        //string TTransCurr;
        //decimal TTransAmt;
        //decimal TAmtFileBToFileC;
        //DateTime TTransDate;
        //int TTraceNo;
        //int TRRNumber;

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

                   // CatchDetails(ex);
                }
            return iResult;
        }

        //// For each ATM in Group Insert Records In Working File 01 - Leave it this for Mpa. 
        //static void InsertRecordsInWorkingFile_01(string InConnectionString, string InMatchingCateg, string InAtmNo,
        //                                                int InRMCycle, DateTime InMinMaxDt)
        //{

        //    string RCT = "[ATMs].[dbo].[Stp_Create_Working_Table_01_V02]";

        //    using (SqlConnection conn = new SqlConnection(InConnectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(RCT, conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                // Parameters
        //                SqlParameter pMatchingCateg = new SqlParameter("@MatchingCateg", SqlDbType.Char);
        //                pMatchingCateg.Direction = ParameterDirection.Input;
        //                pMatchingCateg.Value = InMatchingCateg;
        //                cmd.Parameters.Add(pMatchingCateg);
        //                SqlParameter pAtmNo = new SqlParameter("@AtmNo", SqlDbType.Char);
        //                pAtmNo.Direction = ParameterDirection.Input;
        //                pAtmNo.Value = InAtmNo;
        //                cmd.Parameters.Add(pAtmNo);
        //                //SqlParameter pTraceNo = new SqlParameter("@TraceNo", SqlDbType.Int);
        //                //pTraceNo.Direction = ParameterDirection.Input;
        //                //pTraceNo.Value = InTraceNo;
        //                //cmd.Parameters.Add(pTraceNo);
        //                SqlParameter pRMCycle = new SqlParameter("@RMCycle", SqlDbType.Int);
        //                pRMCycle.Direction = ParameterDirection.Input;
        //                pRMCycle.Value = InRMCycle;
        //                cmd.Parameters.Add(pRMCycle);
        //                SqlParameter pLastDateTime = new SqlParameter("@LastDateTime", SqlDbType.DateTime);
        //                pLastDateTime.Direction = ParameterDirection.Input;
        //                pLastDateTime.Value = InMinMaxDt;
        //                cmd.Parameters.Add(pLastDateTime);
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

        //// For each ATM in Group Insert Records In Working File 01 - Leave it this for Mpa. 
        //static void InsertRecordsInWorkingFile_01_NO_Terminal(string InConnectionString, string InMatchingCateg,
        //                                                int InTraceNo, int InRMCycle)
        //{

        //    string RCT = "[ATMs].[dbo].[Stp_Create_Working_Table_01_NO_Terminal]";

        //    using (SqlConnection conn = new SqlConnection(InConnectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(RCT, conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                // Parameters
        //                SqlParameter pMatchingCateg = new SqlParameter("@MatchingCateg", SqlDbType.Char);
        //                pMatchingCateg.Direction = ParameterDirection.Input;
        //                pMatchingCateg.Value = InMatchingCateg;
        //                cmd.Parameters.Add(pMatchingCateg);
        //                SqlParameter pTraceNo = new SqlParameter("@TraceNo", SqlDbType.Int);
        //                pTraceNo.Direction = ParameterDirection.Input;
        //                pTraceNo.Value = InTraceNo;
        //                cmd.Parameters.Add(pTraceNo);
        //                SqlParameter pRMCycle = new SqlParameter("@RMCycle", SqlDbType.Int);
        //                pRMCycle.Direction = ParameterDirection.Input;
        //                pRMCycle.Value = InRMCycle;
        //                cmd.Parameters.Add(pRMCycle);
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

        public int TotalUnMatchedWithNoAction;
        public int TotalUnMatchedInProcess;
        public int TotalUnMatchedSettled;
        public int TotalUnMatched;

        string RMCateg;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        // Define the data table 
        public DataTable DataTableAtmsTotals = new DataTable();

        // DO THE FINAL UPDATINGS ON ATMs Totals and Reconciliation Session totals 
        public void ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_OurATMs(string InOperator, int InRMCycle, string InSelectionCriteria)
        {
            int NoOfTxns;
            decimal TotAmount;

            //decimal JournalAmt = 0;
            decimal TotalGroupUnMatchedAmt = 0;
            decimal TotalGroupUnMatchedAmtNotInJournal = 0;
            int TotalGroupUnMatchedTxns = 0;

            decimal TotalGroupMatchedAmt = 0;
            int TotalGroupMatchedTxns = 0;


            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAtmsTotals = new DataTable();
            DataTableAtmsTotals.Clear();

            TotalUnMatched = 0;
            TotalUnMatchedWithNoAction = 0;
            TotalUnMatchedInProcess = 0;
            TotalUnMatchedSettled = 0;

            SqlString =
               " SELECT RMCateg, MatchingCateg, TerminalId, Count(SeqNo) As NoOfTxns, "
             + " sum(TransAmount) as TotAmount , Matched, NotInJournal "
              //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
              + " FROM " + SourceTable_A
             + InSelectionCriteria
             + " Group by RMCateg, MatchingCateg, TerminalId, Matched, NotInJournal "
             + " order by TerminalId, Matched, NotInJournal ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableAtmsTotals);

                        // Close conn
                        conn.Close();

                        // For two testing ATMs four records in table are created 
                        // Matched and Unmatched for each ATM 

                        int I = 0;

                        while (I <= (DataTableAtmsTotals.Rows.Count - 1))
                        {

                            RecordFound = true;

                            RMCateg = (string)DataTableAtmsTotals.Rows[I]["RMCateg"];
                            MatchingCateg = (string)DataTableAtmsTotals.Rows[I]["MatchingCateg"];
                            TerminalId = (string)DataTableAtmsTotals.Rows[I]["TerminalId"];
                            NoOfTxns = (int)DataTableAtmsTotals.Rows[I]["NoOfTxns"];
                            TotAmount = (decimal)DataTableAtmsTotals.Rows[I]["TotAmount"];
                            bool Matched = (bool)DataTableAtmsTotals.Rows[I]["Matched"];
                            bool NotInJournal = (bool)DataTableAtmsTotals.Rows[I]["NotInJournal"];

                            // ATMS Updating 
                            // Read specific record 
                            Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                                 (WOperator, RMCateg, InRMCycle, TerminalId);
                            if (Ratms.RecordFound)
                            {
                                // Update Record By Adding
                                if (Matched == false)
                                {

                                    Ratms.UnMatchedAmt = Ratms.UnMatchedAmt + TotAmount;
                                    Ratms.NumberOfUnMatchedRecs = Ratms.NumberOfUnMatchedRecs + NoOfTxns;

                                    TotalGroupUnMatchedAmt = TotalGroupUnMatchedAmt + TotAmount;
                                    TotalGroupUnMatchedTxns = TotalGroupUnMatchedTxns + NoOfTxns;
                                    if (NotInJournal == false)
                                    {
                                        // In Journal 
                                        Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                    }

                                    if (NotInJournal == true)
                                    {
                                        TotalGroupUnMatchedAmtNotInJournal = TotalGroupUnMatchedAmtNotInJournal + TotAmount;
                                    }

                                }
                                if (Matched == true)
                                {
                                    if (NotInJournal == false)
                                    {
                                        // In Journal 
                                        Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                    }

                                    Ratms.MatchedAmtAtMatching = Ratms.MatchedAmtAtMatching + TotAmount;
                                    Ratms.NumberOfMatchedRecs = Ratms.NumberOfMatchedRecs + NoOfTxns;

                                    TotalGroupMatchedAmt = TotalGroupMatchedAmt + TotAmount;
                                    TotalGroupMatchedTxns = TotalGroupMatchedTxns + NoOfTxns;
                                }

                                Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(TerminalId, RMCateg, InRMCycle);
                            }
                            else
                            {
                                // Insert New Record with the corresponding values 

                                Ratms.ReconcCategoryId = WReconcCategoryId;
                                //Ratms.MatchingCategoryId = MatchingCateg;

                                //Ratms.MinMaxTrace = MinMaxTrace;
                                Ratms.AtmGroup = WAtmsReconcGroup;

                                Ratms.RMCycle = InRMCycle;
                                Ratms.AtmNo = TerminalId;
                                Ratms.CreatedDtTm = DateTime.Now;
                                Ratms.Currency = WDepCurNm;

                                Ratms.OwnerUserID = WSignedId;

                                Ratms.OpeningBalance = 0;

                                Ratms.JournalAmt = 0;
                                Ratms.MatchedAmtAtMatching = 0;
                                Ratms.MatchedAmtAtDefault = 0;
                                Ratms.MatchedAmtAtWorkFlow = 0;
                                Ratms.NumberOfMatchedRecs = 0;

                                Ratms.UnMatchedAmt = 0;
                                Ratms.NumberOfUnMatchedRecs = 0;
                                Ratms.Operator = WOperator;


                                Ratms.InsertReconcCategATMSRecord(TerminalId, RMCateg, InRMCycle);

                                Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                                (WOperator, RMCateg, InRMCycle, TerminalId);
                                if (Ratms.RecordFound)
                                {
                                    // Update Record By Adding
                                    if (Matched == false)
                                    {

                                        Ratms.UnMatchedAmt = Ratms.UnMatchedAmt + TotAmount;
                                        Ratms.NumberOfUnMatchedRecs = Ratms.NumberOfUnMatchedRecs + NoOfTxns;

                                        TotalGroupUnMatchedAmt = TotalGroupUnMatchedAmt + TotAmount;
                                        TotalGroupUnMatchedTxns = TotalGroupUnMatchedTxns + NoOfTxns;
                                        if (NotInJournal == false)
                                        {
                                            // In Journal 
                                            Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                        }

                                        if (NotInJournal == true)
                                        {
                                            TotalGroupUnMatchedAmtNotInJournal = TotalGroupUnMatchedAmtNotInJournal + TotAmount;
                                        }

                                    }
                                    if (Matched == true)
                                    {
                                        if (NotInJournal == false)
                                        {
                                            // In Journal 
                                            Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                        }

                                        Ratms.MatchedAmtAtMatching = Ratms.MatchedAmtAtMatching + TotAmount;
                                        Ratms.NumberOfMatchedRecs = Ratms.NumberOfMatchedRecs + NoOfTxns;

                                        TotalGroupMatchedAmt = TotalGroupMatchedAmt + TotAmount;
                                        TotalGroupMatchedTxns = TotalGroupMatchedTxns + NoOfTxns;
                                    }

                                    Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(TerminalId, RMCateg, InRMCycle);
                                }

                            }

                            I++; // Read Next entry of the table 

                        }

                        // At this point Update Rcs
                        // RECONCILIATION CATEGORIES SESSIONS 

                        Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, RMCateg, InRMCycle);

                        if (Rcs.RecordFound == false)
                        {
                            Message = "Reconciliation Category not found for : "
                                                                           + WReconcCategoryId;

                            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                            if (ShowMessage == true)
                            {
                                System.Windows.Forms.MessageBox.Show(Message);
                            }

                        }

                        Rcs.GlTodaysBalance = Rcs.GlTodaysBalance + TotalGroupMatchedAmt
                                                       + TotalGroupUnMatchedAmt - TotalGroupUnMatchedAmtNotInJournal;
                        Rcs.MatchedTransAmt = Rcs.MatchedTransAmt + TotalGroupMatchedAmt;
                        Rcs.NotMatchedTransAmt = Rcs.NotMatchedTransAmt + TotalGroupUnMatchedAmt;

                        Rcs.NumberOfMatchedRecs = Rcs.NumberOfMatchedRecs + TotalGroupMatchedTxns;
                        Rcs.NumberOfUnMatchedRecs = Rcs.NumberOfUnMatchedRecs + TotalGroupUnMatchedTxns;
                        if (Rcs.NumberOfUnMatchedRecs > 0) Rcs.Difference = true;
                        else Rcs.Difference = false;
                        Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions + TotalGroupUnMatchedTxns;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess(RMCateg, InRMCycle);

                        // Update Matching Category as Matched. 
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, RMCateg, MatchingCateg);

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

        }

        // DO THE FINAL UPDATINGS Reconciliation Session totals 
        public void ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_NOT_OurATMs(string InOperator, int InRMCycle, string InSelectionCriteria)
        {
            int NoOfTxns;
            decimal TotAmount;

            //decimal JournalAmt = 0;
            decimal TotalUnMatchedAmt = 0;
            int TotalUnMatchedTxns = 0;

            decimal TotalMatchedAmt = 0;
            int TotalMatchedTxns = 0;

            bool Matched;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //  SqlString =
            //  " SELECT RMCateg, MatchingCateg, TerminalId, Count(SeqNo) As NoOfTxns, "
            //+ " sum(TransAmount) as TotAmount , Matched, NotInJournal "
            // //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            // + " FROM " + SourceTable_A
            //+ InSelectionCriteria
            //+ " Group by RMCateg, MatchingCateg, TerminalId, Matched, NotInJournal "
            //+ " order by TerminalId, Matched, NotInJournal ";

            SqlString =
               " SELECT MatchingCateg, Count(SeqNo) As NoOfTxns, "
             + " sum(TransAmount) as TotAmount , Matched "
             + " FROM " + SourceTable_A
             + InSelectionCriteria
             + " Group by MatchingCateg, Matched "
             + " order by Matched ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableAtmsTotals);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

            // For two testing ATMs four records in table are created 
            // Matched and Unmatched for each 

            // READ TABLE ENTRIES AND UPDATE RECONCILIATION SESSION

            try
            {
                int I = 0;

                while (I <= (DataTableAtmsTotals.Rows.Count - 1))
                {

                    RecordFound = true;

                    MatchingCateg = (string)DataTableAtmsTotals.Rows[I]["MatchingCateg"];
                    NoOfTxns = (int)DataTableAtmsTotals.Rows[I]["NoOfTxns"];
                    TotAmount = (decimal)DataTableAtmsTotals.Rows[I]["TotAmount"];
                    Matched = (bool)DataTableAtmsTotals.Rows[I]["Matched"];

                    if (Matched == false)
                    {
                        TotalUnMatchedAmt = TotalUnMatchedAmt + TotAmount;
                        TotalUnMatchedTxns = TotalUnMatchedTxns + NoOfTxns;
                    }
                    else
                    {
                        TotalMatchedAmt = TotalMatchedAmt + TotAmount;
                        TotalMatchedTxns = TotalMatchedTxns + NoOfTxns;
                    }

                    I++; // Read Next entry of the table 

                }

                // At this point Update Rcs
                // RECONCILIATION CATEGORIES SESSIONS 

                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WReconcCategoryId, InRMCycle);

                if (Rcs.RecordFound == false)
                {
                    Message = "Reconciliation Category not found for : "
                                                                   + WReconcCategoryId;

                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
                    if (ShowMessage == true)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                }

                Rcs.GlTodaysBalance = Rcs.GlTodaysBalance + TotalMatchedAmt
                                               + TotalUnMatchedAmt;
                Rcs.MatchedTransAmt = Rcs.MatchedTransAmt + TotalMatchedAmt;
                Rcs.NotMatchedTransAmt = Rcs.NotMatchedTransAmt + TotalUnMatchedAmt;

                Rcs.NumberOfMatchedRecs = Rcs.NumberOfMatchedRecs + TotalMatchedTxns;
                Rcs.NumberOfUnMatchedRecs = Rcs.NumberOfUnMatchedRecs + TotalUnMatchedTxns;
                if (Rcs.NumberOfUnMatchedRecs > 0) Rcs.Difference = true;
                else Rcs.Difference = false;
                Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions + TotalUnMatchedTxns;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess(WReconcCategoryId, InRMCycle);

                // Update Matching Category as Matched. ( WReconcCategoryId = MatchingCategory )
                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, WReconcCategoryId);
                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, WReconcCategoryId);
                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, WReconcCategoryId);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

        }


        //
        // UPDATE Mpa Masked Card with full card Number  
        // 
        public void UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(string InMatchingCateg, int InRMCycle, string InTableA, string InTableB)
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
                        new SqlCommand("UPDATE t1 "
     + " SET t1.[CardNumber] = t2.CardNumber, t1.[AccNumber] = t2.AccNo "
     + " from " + InTableA + " t1 "
      + " inner join " + InTableB + "  t2 "
     + " on t1.TraceNoWithNoEndZero = t2.TraceNo "
     + " AND t1.TransAmount = t2.TransAmt "
     + " AND t1.TerminalId = t2.TerminalId "
     + " WHERE t1.MatchingCateg = @MatchingCateg AND t1.MatchingAtRMCycle = @MatchingAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

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
        //// UPDATE Mpa Masked Card with full card Number  
        //// 
        //public void UpdatePrimaryWithFullCardNumber_NOT_OurATMs(string InMatchingCateg, int InRMCycle, string InTableA, string InTableB)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string CommandString =
        //                " UPDATE t1 "
        //                + " SET t1.CardNumber = t2.CardNumber "
        //                + " from " + InTableA + " t1 "
        //                + " inner join " + InTableB + "  t2 "
        //                + " on t1.RRNumber = t2.RRNumber "
        //                + " AND t1.TransAmount = t2.TransAmt "
        //                + " WHERE t1.MatchingCateg = @MatchingCateg AND t1.MatchingAtRMCycle = @MatchingAtRMCycle ";

        //    int rows;

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(CommandString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

        //                //rows number of record got updated

        //                rows = cmd.ExecuteNonQuery();

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


    }
}




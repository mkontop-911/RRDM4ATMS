using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
namespace RRDM4ATMs
{
    public class RRDMNVReconcCategoriesSessions : Logger
    {
        public RRDMNVReconcCategoriesSessions() : base() { }

        public int SeqNo;

        public string CategoryId;
        public string CategoryName;
        public string Origin;
        public int RunningJobNo;
        public string InternalAccNo;
        public string ExternalBank;
        public string ExternalAccNo;

        public DateTime StartDailyProcess;
        // Loading Rates
        public string NostroCcy;
        public decimal NostroCcyRate;
        //Loading Sayement
        public bool StatementLoaded;
        public string StmtTrxReferenceNumber;
        public int StmtLines;

        public int TotalNumberProcessed;
        public int MatchedDefault;
        public int AutoButToBeConfirmed;
        public int OutstandingAlerts;
        public int OutstandingDisputes;

        public DateTime FinishDailyProcess;

        public DateTime StartManualDt;
        public int ManualToBeConfirmed;
        public int MatchedFromAutoToBeConfirmed;
        public int MatchedFromManualToBeConfirmed;
        public DateTime FinishManualDt;

        public decimal UnMatchedAmt;

        public string OwnerId;
        public string MPComment; 
        public bool OpenRecord;
        public string Operator;

        public int NumberOfUnMatchedRecs;

        public int TotalFiles;

        // Define the data table 
        public DataTable TableReconcSessionsDistinct = new DataTable();

        // Define the data table 
        public DataTable TableReconcSessionsPerCategory = new DataTable();

        public DataTable TableReconciledCategories = new DataTable();
        public DataTable TableNonReconciledCategories = new DataTable();

        RRDMNVCurrentCcyRates Cr = new RRDMNVCurrentCcyRates();
        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();
        RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();

        public int TotalSelected;
        public int TotalReconcDone;
        public int TotalReconcNotDone;
        public int TotalPreviousRunningCycleReconcNotDone;
        public int TotalReconciledExceptions;
        public int TotalNonReconciledExceptions;

        public int TotalUnMatchedRecs;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        //    string WhatFile = "[ATMS].[dbo].[NVReconcCategoriesSessions]";

        //
        // READ FIELDS 
        //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];
            CategoryName = (string)rdr["CategoryName"];
            Origin = (string)rdr["Origin"];

            RunningJobNo = (int)rdr["RunningJobNo"];
            InternalAccNo = (string)rdr["InternalAccNo"];
            ExternalBank = (string)rdr["ExternalBank"];
            ExternalAccNo = (string)rdr["ExternalAccNo"];

            StartDailyProcess = (DateTime)rdr["StartDailyProcess"];

            NostroCcy = (string)rdr["NostroCcy"];
            NostroCcyRate = (decimal)rdr["NostroCcyRate"];

            StatementLoaded = (bool)rdr["StatementLoaded"];
            StmtTrxReferenceNumber = (string)rdr["StmtTrxReferenceNumber"];
            StmtLines = (int)rdr["StmtLines"];

            TotalNumberProcessed = (int)rdr["TotalNumberProcessed"];
            MatchedDefault = (int)rdr["MatchedDefault"];
            AutoButToBeConfirmed = (int)rdr["AutoButToBeConfirmed"];
            OutstandingAlerts = (int)rdr["OutstandingAlerts"];
            OutstandingDisputes = (int)rdr["OutstandingDisputes"];

            FinishDailyProcess = (DateTime)rdr["FinishDailyProcess"];

            StartManualDt = (DateTime)rdr["StartManualDt"];
            ManualToBeConfirmed = (int)rdr["ManualToBeConfirmed"];
            MatchedFromAutoToBeConfirmed = (int)rdr["MatchedFromAutoToBeConfirmed"];
            MatchedFromManualToBeConfirmed = (int)rdr["MatchedFromManualToBeConfirmed"];
            FinishManualDt = (DateTime)rdr["FinishManualDt"];

            UnMatchedAmt = (decimal)rdr["UnMatchedAmt"];

            OwnerId = (string)rdr["OwnerId"];
            //MPComment = (string)rdr["MPComment"];
            OpenRecord = (bool)rdr["OpenRecord"];
            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ReconcCategoriesSessions
        // FILL UP A TABLE
        //
        public void ReadNVReconcCategoriesSessionsSpecificCat(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerCategory = new DataTable();
            TableReconcSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcSessionsPerCategory.Columns.Add("SeqNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("RunningJobNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("StartManual", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("EndManual", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("UnMatchedRecs", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId "
                    + " ORDER BY RunningJobNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReaderFields(rdr);

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

                            DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["RunningJobNo"] = RunningJobNo;
                            RowSelected["CategoryId"] = CategoryId;
                            if (StartManualDt == NullPastDate)
                            {
                                RowSelected["StartManual"] = "Not Started";
                                RowSelected["EndManual"] = "Not Started";
                            }
                            else
                            {
                                RowSelected["StartManual"] = StartManualDt.ToString();
                                if (FinishManualDt == NullPastDate)
                                {
                                    RowSelected["EndManual"] = "Not Finish";
                                }
                                else RowSelected["EndManual"] = FinishManualDt.ToString();
                            }


                            RowSelected["UnMatchedRecs"] = NumberOfUnMatchedRecs;


                            // ADD ROW
                            TableReconcSessionsPerCategory.Rows.Add(RowSelected);

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
        // READ ReconcCategoriesSessions
        // FILL UP A TABLE
        //
        public void ReadNVReconcCategoriesSessionsSpecificRunningJobCycle
            (string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMUsersRecords Us = new RRDMUsersRecords();

            TableReconcSessionsPerCategory = new DataTable();
            TableReconcSessionsPerCategory.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcSessionsPerCategory.Columns.Add("SeqNo", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("CategoryId", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("Name", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("Ccy", typeof(string));

            TableReconcSessionsPerCategory.Columns.Add("MatchedAuto", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("ToBeConfirmed", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("UnMatched", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("LocalAmt", typeof(decimal));

            TableReconcSessionsPerCategory.Columns.Add("Alerts", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("Disputes", typeof(int));
            TableReconcSessionsPerCategory.Columns.Add("OwnerId", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("OwnerName", typeof(string));

            TableReconcSessionsPerCategory.Columns.Add("StartManual", typeof(string));
            TableReconcSessionsPerCategory.Columns.Add("EndManual", typeof(string));

            SqlString = "SELECT *"
                    + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                    + InSelectionCriteria
                    + " ORDER BY CategoryId ASC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReaderFields(rdr);

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

                            DataRow RowSelected = TableReconcSessionsPerCategory.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["CategoryId"] = CategoryId;
                            RowSelected["Name"] = CategoryName;
                            RowSelected["Ccy"] = NostroCcy;
                            RowSelected["MatchedAuto"] = MatchedDefault;
                            RowSelected["ToBeConfirmed"] = AutoButToBeConfirmed + ManualToBeConfirmed;
                            RowSelected["UnMatched"] = NumberOfUnMatchedRecs;
                            RowSelected["LocalAmt"] = UnMatchedAmt * NostroCcyRate;

                            RowSelected["Alerts"] = OutstandingAlerts;
                            RowSelected["Disputes"] = OutstandingDisputes;


                            RowSelected["OwnerId"] = OwnerId;
                            if (OwnerId != "")
                            {
                                Us.ReadUsersRecord(OwnerId);
                                RowSelected["OwnerName"] = Us.UserName;
                            }
                            else
                            {
                                RowSelected["OwnerName"] = "Reconc Pair has no Owner.";
                            }

                            if (StartManualDt == NullPastDate)
                            {
                                RowSelected["StartManual"] = "Not Started";
                                RowSelected["EndManual"] = "Not Started";
                            }
                            else
                            {
                                RowSelected["StartManual"] = StartManualDt.ToString();
                                if (FinishManualDt == NullPastDate)
                                {
                                    RowSelected["EndManual"] = "Not Finish";
                                }
                                else RowSelected["EndManual"] = FinishManualDt.ToString();
                            }

                            // ADD ROW
                            TableReconcSessionsPerCategory.Rows.Add(RowSelected);
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
        // READ ReconcCategoriesSessions by SeqNo
        //
        //
        public void ReadNVReconcCategoriesSessionsBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                    + " WHERE SeqNo = @SeqNo ";

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

                            TotalSelected = TotalSelected + 1;

                            ReaderFields(rdr);

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

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
        // READ ReconcCategoriesSessions by Selection Criteria
        //
        //
        public void ReadNVReconcCategoriesSessionsBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                    + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

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
        // READ ReconcCategories and find the ones with matching and others no matching    
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesForReconcStatus(string InOperator, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconciledCategories = new DataTable();
            TableReconciledCategories.Clear();

            TableNonReconciledCategories = new DataTable();
            TableNonReconciledCategories.Clear();

            TotalSelected = 0;
            TotalReconcDone = 0;
            TotalReconcNotDone = 0;
            TotalPreviousRunningCycleReconcNotDone = 0;
            TotalReconciledExceptions = 0;
            TotalNonReconciledExceptions = 0;

            // DATA TABLE ROWS DEFINITION - Reconciled  
            TableReconciledCategories.Columns.Add("CategoryNm", typeof(string));
            TableReconciledCategories.Columns.Add("RunningJobNo", typeof(int));
            TableReconciledCategories.Columns.Add("OwnerId", typeof(string));

            // DATA TABLE ROWS DEFINITION - Not Reconciled  

            TableNonReconciledCategories.Columns.Add("CategoryNm", typeof(string));
            TableNonReconciledCategories.Columns.Add("RunningJobNo", typeof(int));
            TableNonReconciledCategories.Columns.Add("OwnerId", typeof(string));

            SqlString = "SELECT *"
                       + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                    + " WHERE Operator = @Operator "
                    + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            ReaderFields(rdr);


                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

                            if (NumberOfUnMatchedRecs == 0 & RunningJobNo == InRunningJobNo)
                            {
                                //Records Reconciled
                                TotalReconcDone = TotalReconcDone + 1;

                                TotalReconciledExceptions = TotalReconciledExceptions + NumberOfUnMatchedRecs;
                                //public int TotalNonReconciledExceptions;

                                DataRow RowSelected = TableReconciledCategories.NewRow();

                                RowSelected["CategoryNm"] = CategoryName;
                                //RowSelected["Category_Name"] = CategoryName;
                                RowSelected["RunningJobNo"] = RunningJobNo;
                                RowSelected["OwnerId"] = OwnerId;

                                // ADD ROW
                                TableReconciledCategories.Rows.Add(RowSelected);
                            }
                            if (NumberOfUnMatchedRecs > 0)
                            {
                                if (RunningJobNo <= InRunningJobNo)
                                {
                                    TotalNonReconciledExceptions = TotalNonReconciledExceptions + NumberOfUnMatchedRecs;

                                    DataRow RowSelected = TableNonReconciledCategories.NewRow();

                                    RowSelected["CategoryNm"] = CategoryName;
                                    //RowSelected["Category_Name"] = CategoryName;
                                    RowSelected["RunningJobNo"] = RunningJobNo;
                                    RowSelected["OwnerId"] = OwnerId;

                                    // ADD ROW
                                    TableNonReconciledCategories.Rows.Add(RowSelected);
                                    //Records Reconciled
                                    if (RunningJobNo == InRunningJobNo)
                                    {
                                        // This Running Cycle 
                                        TotalReconcNotDone = TotalReconcNotDone + 1;
                                    }
                                    else
                                    {
                                        // Previous Running Cycles 
                                        TotalPreviousRunningCycleReconcNotDone = TotalPreviousRunningCycleReconcNotDone + 1;
                                    }
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
        // READ ReconcCategoriesSessions Specific by RunningJobNo
        // 
        //
        public void ReadReconcCategorySessionByRunningJobNoAndAccount(string InOperator, string InExternalAccNo, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND ExternalAccNo = @ExternalAccNo  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            ReaderFields(rdr);

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

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
        // READ ReconcCategoriesSessions Specific by RunningJobNo by Category Id 
        // 
        //
        public void ReadNVReconcCategorySessionByCatAndRunningJobNo(string InOperator, string InCategoryId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString = "SELECT *"
                       + " FROM  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                       + " WHERE Operator = @Operator AND CategoryId = @CategoryId  AND RunningJobNo = @RunningJobNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReaderFields(rdr);

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

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
        // READ ReadReconciliationSessionsTOTALSForDashBoard
        // 
        //
        public int No_Categories;
        public int LoadedStmts;
        public int NotLoadedStmts;
        public int CatUnMatched;
        public void ReadReconciliationSessionsTOTALSForDashBoard
                      (string InOperator, string InSignedId, int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalFiles = 0;

            SqlString =
               " SELECT "
               + " Count(CategoryId) As No_Categories,"
               + " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
                + " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
               + " SUM(TotalNumberProcessed) As TotalNumberProcessed,"
               + " SUM(MatchedDefault) As MatchedDefault,"
               + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed,"
               + " SUM(OutstandingAlerts) As OutstandingAlerts,"
               + " SUM(OutstandingDisputes) As OutstandingDisputes,"
               + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
               + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
               + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
               + " UnMatchedNo = SUM(TotalNumberProcessed - "
               + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
               + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
               + " SUM(UnMatchedAmt*[NostroCcyRate]) As UnMatchedAmt "
               + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
               + " Where RunningJobNo = @RunningJobNo "
               ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            No_Categories = (int)rdr["No_Categories"];

                            LoadedStmts = (int)rdr["LoadedStmts"];
                            NotLoadedStmts = (int)rdr["NotLoadedStmts"];

                            TotalNumberProcessed = (int)rdr["TotalNumberProcessed"];
                            MatchedDefault = (int)rdr["MatchedDefault"];
                            AutoButToBeConfirmed = (int)rdr["AutoButToBeConfirmed"];
                            OutstandingAlerts = (int)rdr["OutstandingAlerts"];
                            OutstandingDisputes = (int)rdr["OutstandingDisputes"];

                            ManualToBeConfirmed = (int)rdr["ManualToBeConfirmed"];
                            MatchedFromAutoToBeConfirmed = (int)rdr["MatchedFromAutoToBeConfirmed"];
                            MatchedFromManualToBeConfirmed = (int)rdr["MatchedFromManualToBeConfirmed"];

                            UnMatchedAmt = (decimal)rdr["UnMatchedAmt"]; // Local Amt 

                            NumberOfUnMatchedRecs = (int)rdr["UnMatchedNo"];
                            CatUnMatched = (int)rdr["CatUnMatched"];

                            NumberOfUnMatchedRecs =
                              TotalNumberProcessed
                              - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed);

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
        // READ Reconciliation Sessions for this Cycle
        // AND UpdateRates and totals 
        // 


        public DataTable TableReconcSessionsPerRunningJobCycle = new DataTable();
        public void UpdateReconciliationSessionsForNostroVostroForRates(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InOrigin,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;

            SqlString =

                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {



                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];

                            // Find Rate 

                            Cr.ReadNVCurrentCcyRatesById(InOperator, NostroCcy);
                            // Cr.CcyRate
                            //// Find UnMatched for Internal and External with date < Working date
                            //Se.ReadNVStatements_LinesForTotalsAbsUnMatched(InOperator, InSignedId, 1,
                            //                        ExternalAccNo, InternalAccNo, InWDate);

                            //public decimal UnMatchedAbsAmt;
                            // Update Reconciliation Session
                            UpdateReconcCategorySessionWithRate(SeqNo,
                                   Cr.CcyRate);

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
        // MakeManyToManyMatching
        // 
        //int WMode;
        public void MakeManyToManyMatching(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InSubSystem,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //InSubSystem; // Used For Transactions 
            // "CardsSettlement"
            // "NostroReconciliation"

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;
            // 
            SqlString =

                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            if (InSubSystem == "E_FINANCE")
            {
                // Find Branch for this User
                RRDMUsersRecords Us = new RRDMUsersRecords();
                Us.ReadUsersRecord(InSignedId);
                string WBranch = Us.Branch; 
                string Application = "E_FINANCE RECONCILIATION"; 
                RRDMUsers_Applications_Roles Urs = new RRDMUsers_Applications_Roles();
                Urs.ReadUsersVsApplicationsVsRolesByApplication(InSignedId, Application);

                if (Urs.SecLevel == "04")
                {
                 // Only one branch 
               SqlString =
               " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
               + " Where RunningJobNo = @RunningJobNo AND CategoryId ='"+ "BDC" + WBranch +"'"
               + " ORDER BY CategoryId "
               ;
                }
            }
           
            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];

                            string W4DigitMainCateg;

                            W4DigitMainCateg = CategoryId.Substring(0, 4);

                            //if (InSubSystem == "NostroReconciliation") WMode = 20;

                            if (InSubSystem == "NostroReconciliation")
                            {
                                Se.ReadNVStatements_LinesAndMatchManyToMany(InOperator, InSignedId, InSubSystem, InRunningJobNo, ExternalAccNo, InternalAccNo, InWDate, InWDate);
                            }

                            if (InSubSystem == "CardsSettlement" || InSubSystem == "MASTER")
                            {
                                InSubSystem = "MASTER";
                                Sec.ReadNVStatements_LinesAndMatchManyToMany(InOperator, InSignedId, InSubSystem, InRunningJobNo, ExternalAccNo, InternalAccNo, InWDate, InWDate);
                            }

                            if (InSubSystem == "E_FINANCE")
                            {
                                Sec.ReadNVStatements_LinesAndMatchManyToMany(InOperator, InSignedId, InSubSystem, InRunningJobNo, ExternalAccNo, InternalAccNo, InWDate, InWDate);
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
        // ReadAgingTable
        // 
        public DataTable TableAgeingAnalysis = new DataTable();
        public void ReadAgeingTable(string InOperator, string InSignedId,
                                              int InRunningJobNo, DateTime InTestingDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;

            SqlString =

                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {



                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];
                            NostroCcyRate = (decimal)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcyRate"];

                            Se.ReadInternalStatementAndCreateAgeingFigures(InOperator, InSignedId, I,
                                CategoryId, CategoryName,
                                       InternalAccNo, NostroCcy, NostroCcyRate, InTestingDate);

                            I++; // Read Next entry of the table 

                        }

                        TableAgeingAnalysis = new DataTable();
                        TableAgeingAnalysis.Clear();

                        TableAgeingAnalysis = Se.TableAgingAnalysis;
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // UPDATE Totals for confirmed and not confirmed 
        // 
        // 

        public void UpdateReconciliationSessionsForNostroVostroForTotals(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InSubSystem,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;

            SqlString =
                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions] "
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {


                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];

                            FinishDailyProcess = (DateTime)TableReconcSessionsPerRunningJobCycle.Rows[I]["FinishDailyProcess"];

                            StatementLoaded = (bool)TableReconcSessionsPerRunningJobCycle.Rows[I]["StatementLoaded"];
                            StmtTrxReferenceNumber = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["StmtTrxReferenceNumber"];

                            //string W4DigitMainCateg;

                            //W4DigitMainCateg = CategoryId.Substring(0, 4);

                            //if (InOrigin == "Visa Settlement") WMode = 20;
                            //if (InOrigin == "Nostro - Vostro") WMode = 21;
                            //
                            // Find Totals for EXTERNAL except of Alerts with date < Working date
                            Se.ReadNVExternalStatements_LinesForTotals(InOperator, InSignedId, InSubSystem, InRunningJobNo,
                                                    ExternalAccNo, StmtTrxReferenceNumber, InWDate);

                            // Find ALerts and Disputes 
                            int Mode = 12; // All this cycle 

                            Se.ReadNVStatements_LinesByMode(InOperator, InSignedId, CategoryId, Mode, InRunningJobNo, InternalAccNo, ExternalAccNo, "", InWDate, "");


                            UpdateReconcCategorySessionWithAutomaticMatchTotals(SeqNo,
                               Se.TotalNumberProcessed, Se.MatchedDefault, Se.AutoButToBeConfirmed,
                               Se.UnMatchedAmt, Se.MatchedFromAutoToBeConfirmed, Se.MatchedFromManualToBeConfirmed, Se.OutstandingAlerts, Se.OutstandingDisputes, FinishDailyProcess);

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
        // UPDATE Totals for confirmed and not confirmed 
        // 
        // 

        public void UpdateReconciliationSessionsForCardsForTotals(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InSubSystem,
                                              DateTime InWDate)
        {
            //RRDMNVCardsBothAuthorAndSettlement Sec = new RRDMNVCardsBothAuthorAndSettlement();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;

            SqlString =
                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions] "
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            Origin = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["Origin"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];

                            FinishDailyProcess = (DateTime)TableReconcSessionsPerRunningJobCycle.Rows[I]["FinishDailyProcess"];

                            StatementLoaded = (bool)TableReconcSessionsPerRunningJobCycle.Rows[I]["StatementLoaded"];
                            StmtTrxReferenceNumber = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["StmtTrxReferenceNumber"];

                            if (Origin == "Nostro - Vostro")
                            {
                                // Find Totals for EXTERNAL except of Alerts with date < Working date
                                Se.ReadNVExternalStatements_LinesForTotals(InOperator, InSignedId, InSubSystem, InRunningJobNo,
                                                        ExternalAccNo, StmtTrxReferenceNumber, InWDate);

                                // Find ALerts and Disputes 
                                int Mode = 12; // All this cycle 

                                Se.ReadNVStatements_LinesByMode(InOperator, InSignedId, CategoryId, Mode, InRunningJobNo, InternalAccNo, ExternalAccNo, "", InWDate, "");

                                UpdateReconcCategorySessionWithAutomaticMatchTotals(SeqNo,
                                     Se.TotalNumberProcessed, Se.MatchedDefault, Se.AutoButToBeConfirmed,
                                     Se.UnMatchedAmt, Se.MatchedFromAutoToBeConfirmed, Se.MatchedFromManualToBeConfirmed, Se.OutstandingAlerts, Se.OutstandingDisputes, FinishDailyProcess);


                            }

                            if (Origin == "Visa Settlement" || Origin == "Master Card")
                            {

                                Sec.ReadNVExternalStatements_LinesForTotals(InOperator, InSignedId, InSubSystem, InRunningJobNo,
                                                      ExternalAccNo, StmtTrxReferenceNumber, InWDate);

                                // Find ALerts and Disputes 
                                int Mode = 12; // All this cycle 

                                Sec.ReadNVCardRecordsForBothByMode(InOperator, InSignedId, CategoryId, Mode, InRunningJobNo, InternalAccNo, ExternalAccNo, "", InWDate, "");

                                UpdateReconcCategorySessionWithAutomaticMatchTotals(SeqNo,
                                     Sec.TotalNumberProcessed, Sec.MatchedDefault, Sec.AutoButToBeConfirmed,
                                     Sec.UnMatchedAmt, Sec.MatchedFromAutoToBeConfirmed, Sec.MatchedFromManualToBeConfirmed, Sec.OutstandingAlerts, Sec.OutstandingDisputes, FinishDailyProcess);

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
        // READ Reconciliation Sessions for this Cycle
        // AND Update Loading Statements 
        // 
        public void LoadStatementsOrVisaFiles(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InOrigin,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcSessionsPerRunningJobCycle = new DataTable();
            TableReconcSessionsPerRunningJobCycle.Clear();

            TotalSelected = 0;

            SqlString =
                " SELECT * FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
                + " Where RunningJobNo = @RunningJobNo "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {


                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TableReconcSessionsPerRunningJobCycle);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcSessionsPerRunningJobCycle.Rows.Count - 1))
                        {
                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcSessionsPerRunningJobCycle.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["CategoryName"];

                            InternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["InternalAccNo"];
                            ExternalAccNo = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["ExternalAccNo"];
                            //NostroCcy = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["NostroCcy"];

                            //StatementLoaded = (bool)TableReconcSessionsPerRunningJobCycle.Rows[I]["StatementLoaded"];
                            //StmtTrxReferenceNumber = (string)TableReconcSessionsPerRunningJobCycle.Rows[I]["StmtTrxReferenceNumber"];
                            StatementLoaded = true;
                            if (ExternalAccNo == "ALPHA67890"
                                    || ExternalAccNo == "City12345USD"
                                    || ExternalAccNo == "VISASettl-PHP"
                                    || ExternalAccNo == "GL_280"
                                    || ExternalAccNo == "GL_300"
                                    || ExternalAccNo == "MASTER_POS_001"
                                    )
                            {
                                if (ExternalAccNo == "ALPHA67890") StmtTrxReferenceNumber = "MT950-0001";
                                if (ExternalAccNo == "City12345USD") StmtTrxReferenceNumber = "MT950-0002";
                                if (ExternalAccNo == "VISASettl-PHP") StmtTrxReferenceNumber = "MT950-0001";
                                if (ExternalAccNo == "GL_280") StmtTrxReferenceNumber = "MT950-0001";
                                if (ExternalAccNo == "GL_300") StmtTrxReferenceNumber = "MT950-0001";
                                if (ExternalAccNo == "MASTER_POS_001") StmtTrxReferenceNumber = "MT950-0001";

                                StmtLines = 9;
                            }
                            else
                            {
                                StmtTrxReferenceNumber = "No Loaded Stmt";
                                StmtLines = 0;
                            }

                            UpdateLoadStatementInfo(SeqNo);

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
        // Insert Reconciliation Category Session 
        public int InsertNVReconcCategoriesSessionRecord()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO  [ATMS].[dbo].[NVReconcCategoriesSessions]"
                + " ([CategoryId],"
                + " [CategoryName],"
                + " [Origin],"
                + " [RunningJobNo],"
                + " [StartDailyProcess],"
                + " [InternalAccNo],"
                + " [ExternalBank],"
                + " [ExternalAccNo],"
                + " [NostroCcy],"
                + " [OwnerId],"
                + " [Operator] )"
                + " VALUES"
                + " (@CategoryId,"
                + " @CategoryName,"
                + " @Origin,"
                + " @RunningJobNo,"
                + " @StartDailyProcess,"
                + " @InternalAccNo,"
                + " @ExternalBank,"
                + " @ExternalAccNo,"
                + " @NostroCcy,"
                + " @OwnerId,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@RunningJobNo", RunningJobNo);
                        cmd.Parameters.AddWithValue("@StartDailyProcess", StartDailyProcess);

                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@ExternalBank", ExternalBank);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);
                        cmd.Parameters.AddWithValue("@NostroCcy", NostroCcy);

                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
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
        // UPDATE RATE ANd UnMatched 
        // 
        public void UpdateReconcCategorySessionWithRate(int InSeqNo,
                   decimal InNostroCcyRate)
        {

            ErrorFound = false;
            ErrorOutput = "";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " NostroCcyRate = @NostroCcyRate "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@NostroCcyRate", InNostroCcyRate);

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
        // UPDATE Loaded Statement 
        // 
        public void UpdateLoadStatementInfo(int InSeqNo)
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
                        new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " StatementLoaded = @StatementLoaded, "
                             + " StmtTrxReferenceNumber = @StmtTrxReferenceNumber, "
                              + " StmtLines = @StmtLines "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@StatementLoaded", StatementLoaded);
                        cmd.Parameters.AddWithValue("@StmtTrxReferenceNumber", StmtTrxReferenceNumber);
                        cmd.Parameters.AddWithValue("@StmtLines", StmtLines);

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
        // UPDATE AutomaticMatchTotals
        // 
        public void UpdateReconcCategorySessionWithAutomaticMatchTotals(int InSeqNo,
                   int InTotalNumberProcessed, int InMatchedDefault, int InAutoButToBeConfirmed, decimal InUnMatchedAmt,
                   int InMatchedFromAutoToBeConfirmed, int InMatchedFromManualToBeConfirmed,
                  int InOutstandingAlerts, int InOutstandingDisputes, DateTime InFinishDailyProcess)
        {


            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " TotalNumberProcessed = @TotalNumberProcessed, MatchedDefault = @MatchedDefault,  "
                            + " AutoButToBeConfirmed = @AutoButToBeConfirmed, "
                            + " UnMatchedAmt = @UnMatchedAmt, "
                            + " MatchedFromAutoToBeConfirmed = @MatchedFromAutoToBeConfirmed, MatchedFromManualToBeConfirmed = @MatchedFromManualToBeConfirmed, "
                            + " OutstandingAlerts = @OutstandingAlerts, OutstandingDisputes = @OutstandingDisputes, "
                            + " FinishDailyProcess = @FinishDailyProcess  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@TotalNumberProcessed", InTotalNumberProcessed);
                        cmd.Parameters.AddWithValue("@MatchedDefault", InMatchedDefault);
                        cmd.Parameters.AddWithValue("@AutoButToBeConfirmed", InAutoButToBeConfirmed);
                        cmd.Parameters.AddWithValue("@UnMatchedAmt", InUnMatchedAmt);
                        cmd.Parameters.AddWithValue("@MatchedFromAutoToBeConfirmed", InMatchedFromAutoToBeConfirmed);
                        cmd.Parameters.AddWithValue("@MatchedFromManualToBeConfirmed", InMatchedFromManualToBeConfirmed);
                        cmd.Parameters.AddWithValue("@OutstandingAlerts", InOutstandingAlerts);
                        cmd.Parameters.AddWithValue("@OutstandingDisputes", InOutstandingDisputes);
                        cmd.Parameters.AddWithValue("@FinishDailyProcess", InFinishDailyProcess);

                        cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    RRDMLog4Net Log = new RRDMLog4Net();

                    CatchDetails(ex);
                }
        }
        // 
        // UPDATE ReconcCategoriesSession Owner Id
        // 
        public void UpdateReconcCategorySessionWithReconWithOwner(string InCategoryId, int InRunningJobNo, string InOwnerId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " OwnerId = @OwnerId "
                            + " WHERE RunningJobNo = @RunningJobNo AND CategoryId = @CategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@OwnerId", InOwnerId);

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
        // UPDATE ReconcCategoriesSessions Owner 
        // 
        public void UpdateReconcCategorySessionStartDate(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " StartReconcDtTm = @StartReconcDtTm "
                            + " WHERE CategoryId = @CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@StartManualDt", StartManualDt);

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
        // UPDATE ReconcCategoriesSessions at Reconciliation closing 
        // 
        public void UpdateReconcCategorySessionWithAuthorClosing(string InCategoryId, int InRunningJobNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                         new SqlCommand("UPDATE  [ATMS].[dbo].[NVReconcCategoriesSessions]" + " SET "
                            + " EndReconcDtTm = @EndReconcDtTm ,"
                            + " SettledUnMatchedAmtDefault = @SettledUnMatchedAmtDefault ,SettledUnMatchedAmtWorkFlow = @SettledUnMatchedAmtWorkFlow ,"
                            + " NumberSettledUnMatchedDefault = @NumberSettledUnMatchedDefault ,NumberSettledUnMatchedWorkFlow = @NumberSettledUnMatchedWorkFlow ,"
                            + " RemainReconcExceptions = @RemainReconcExceptions "
                            + " WHERE CategoryId =@CategoryId AND RunningJobNo = @RunningJobNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobNo", InRunningJobNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

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



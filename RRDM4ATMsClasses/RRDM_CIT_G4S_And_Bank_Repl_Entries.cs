using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDM_CIT_G4S_And_Bank_Repl_Entries : Logger
    {
        public RRDM_CIT_G4S_And_Bank_Repl_Entries() : base() { }

        public int SeqNo;

        public string CITId; // 

        public string OriginFileName;

        public string AtmNo; // 

        public string AtmName;

        public int LoadingExcelCycleNo;
        public DateTime ExcelDate;
        public DateTime ReplDateG4S;
        public int OrderNo;

        public DateTime CreatedDate;

        public bool IsDeposit;

        public decimal OpeningBalance;
        public decimal Dispensed;
        public decimal UnloadedMachine; // Cassettes + Rejected Tray
        public decimal UnloadedCounted;
        public decimal Cash_Loaded;
        public decimal Deposits;
        public decimal OverFound;
        public decimal ShortFound;
        public string RemarksG4S;  // The data from G4S are completed 

        public decimal PresentedErrors; // Taken From RRDM
        public decimal AmtCheckedForMatching; //  System chcks if matched records = ALl files being updated 
        public decimal OtherJournalErrors; // Taken From RRDM
        public decimal OrderToBeLoaded; // Money as per order
        public string RemarksRRDM; // Taken From RRDM

        public int ProcessMode_Load; // -2 Just loaded For G4S and Journal 
                                     // 0 Validated  and it is ready for updating  means that Mask = 11
                                     // 1 Counted Totals Updated on RRDM
                                     // AFter Transaction cretaion this turns to 2 
        public int ProcessMode_UnLoad; // After TXN Creaation this turns to 2 

        public int ReplCycleNo; // This is done when Process Mode is turned to 1 

        public string Mask; // Matching Mask 
        public DateTime Cut_Off_date; // 
        public decimal Gl_Balance_At_CutOff;

        public int Load_FaceValue_1;
        public int Load_Cassette_1;
        public int Load_FaceValue_2;
        public int Load_Cassette_2;
        public int Load_FaceValue_3;
        public int Load_Cassette_3;
        public int Load_FaceValue_4;
        public int Load_Cassette_4;

        public int Un_Load_FaceValue_1;
        public int Un_Load_Cassette_1;
        public int Un_Load_FaceValue_2;
        public int Un_Load_Cassette_2;
        public int Un_Load_FaceValue_3;
        public int Un_Load_Cassette_3;
        public int Un_Load_FaceValue_4;
        public int Un_Load_Cassette_4;

        public int Dep_FaceValue_1;
        public int Deposits_Notes_Denom_1;
        public int Dep_FaceValue_2;
        public int Deposits_Notes_Denom_2;
        public int Dep_FaceValue_3;
        public int Deposits_Notes_Denom_3;
        public int Dep_FaceValue_4;
        public int Deposits_Notes_Denom_4;

        public DateTime Repl_Load_Excel_Date;
        public int Repl_Load_Status;// 2: Loaded from Excel
                                    // 4: Valid At Maker (Ready for transactions creation) 
                                    // 6: Invalid At Maker - Not Ready for transactions creation
                                    // 8: Completed Transactions are made
        public string Repl_Load_Action; // Make comment as per status

        public DateTime Repl_UnLoad_Excel_Date;
        public int Repl_UnLoad_Status;// 2: Loaded from Excel
                                      // 4: Valid At Maker (Ready for transactions creation) 
                                      // 6: Invalid At Maker - Not Ready for transactions creation
                                      // 8: Completed Transactions are made
                                      // 10: Move to Replenishment workflow no transactions are made 
        public string Repl_UnLoad_Action; // Make comment as per status

        public decimal SM_Loaded;
        public decimal SM_Unloaded_Cassette;
        public decimal SM_Unloaded_Deposits;

        public int LoadedAtRMCycle;
        public string Operator;

        // ****************************

        string SqlString;

        public decimal Loaded_Total;
        public int Loaded_ATMS;

        public int TotalNotProcessed;
        public int Total11;
        public int TotalAA;
        public int Total10;
        public int Total01;

        public int TotalShort;
        public int TotalPresenter;

        public decimal TotalShortAmt;
        public decimal TotalPresenterAmt;

       // RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();

        RRDM_Cit_ExcelProcessedCycles Cec = new RRDM_Cit_ExcelProcessedCycles();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDM_GL_Balances_For_Categories_And_Atms Gadj = new RRDM_GL_Balances_For_Categories_And_Atms();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();


        RRDMAccountsClass Acc = new RRDMAccountsClass();


        RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMSessionsDataCombined Sc = new RRDMSessionsDataCombined();
        RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();
        RRDMMatchingTxns_InGeneralTables_BDC Gt = new RRDMMatchingTxns_InGeneralTables_BDC();

        // Define the data table 
        public DataTable DataTableG4SEntries = new DataTable();

        //public DataTable MatchingFieldsDataTable = new DataTable();

        public DataTable DataTableG4SEntriesSelectedForExcel = new DataTable();
        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        string TableA = "[RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_G4S_Repl_Entries] ";

        string TableB = "[RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_Bank_Repl_Entries] ";

        //
        // Reader fields 
        //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CITId = (string)rdr["CITId"]; // AUDI

            OriginFileName = (string)rdr["OriginFileName"]; // AUDI

            ExcelDate = (DateTime)rdr["ExcelDate"];

            AtmNo = (string)rdr["AtmNo"]; // AUDI
            AtmName = (string)rdr["AtmName"]; // AUDI Location 

            LoadingExcelCycleNo = (int)rdr["LoadingExcelCycleNo"]; // AUDI
            ReplDateG4S = (DateTime)rdr["ReplDateG4S"];
            OrderNo = (int)rdr["OrderNo"];       // Order Number

            CreatedDate = (DateTime)rdr["CreatedDate"];      // AUDI

            IsDeposit = (bool)rdr["IsDeposit"];

            OpeningBalance = (decimal)rdr["OpeningBalance"];
            Dispensed = (decimal)rdr["Dispensed"];
            UnloadedMachine = (decimal)rdr["UnloadedMachine"];
            UnloadedCounted = (decimal)rdr["UnloadedCounted"];
            Cash_Loaded = (decimal)rdr["Cash_Loaded"];         // AUDI

            Deposits = (decimal)rdr["Deposits"];

            OverFound = (decimal)rdr["OverFound"];
            ShortFound = (decimal)rdr["ShortFound"];

            RemarksG4S = (string)rdr["RemarksG4S"];
            PresentedErrors = (decimal)rdr["PresentedErrors"];
            AmtCheckedForMatching = (decimal)rdr["AmtCheckedForMatching"];
            OtherJournalErrors = (decimal)rdr["OtherJournalErrors"];

            OrderToBeLoaded = (decimal)rdr["OrderToBeLoaded"];

            RemarksRRDM = (string)rdr["RemarksRRDM"];

            ProcessMode_Load = (int)rdr["ProcessMode_Load"];
            ProcessMode_UnLoad = (int)rdr["ProcessMode_UnLoad"];

            ReplCycleNo = (int)rdr["ReplCycleNo"]; // AUDI

            Mask = (string)rdr["Mask"];

            Cut_Off_date = (DateTime)rdr["Cut_Off_date"];

            Gl_Balance_At_CutOff = (decimal)rdr["Gl_Balance_At_CutOff"];

            Load_FaceValue_1 = (int)rdr["Load_FaceValue_1"];
            Load_Cassette_1 = (int)rdr["Load_Cassette_1"];
            Load_FaceValue_2 = (int)rdr["Load_FaceValue_2"];
            Load_Cassette_2 = (int)rdr["Load_Cassette_2"];
            Load_FaceValue_3 = (int)rdr["Load_FaceValue_3"];
            Load_Cassette_3 = (int)rdr["Load_Cassette_3"];
            Load_FaceValue_4 = (int)rdr["Load_FaceValue_4"];
            Load_Cassette_4 = (int)rdr["Load_Cassette_4"];

            Un_Load_FaceValue_1 = (int)rdr["Un_Load_FaceValue_1"];
            Un_Load_Cassette_1 = (int)rdr["Un_Load_Cassette_1"];
            Un_Load_FaceValue_2 = (int)rdr["Un_Load_FaceValue_2"];
            Un_Load_Cassette_2 = (int)rdr["Un_Load_Cassette_2"];
            Un_Load_FaceValue_3 = (int)rdr["Un_Load_FaceValue_3"];
            Un_Load_Cassette_3 = (int)rdr["Un_Load_Cassette_3"];
            Un_Load_FaceValue_4 = (int)rdr["Un_Load_FaceValue_4"];
            Un_Load_Cassette_4 = (int)rdr["Un_Load_Cassette_4"];

            Dep_FaceValue_1 = (int)rdr["Dep_FaceValue_1"];
            Deposits_Notes_Denom_1 = (int)rdr["Deposits_Notes_Denom_1"];
            Dep_FaceValue_2 = (int)rdr["Dep_FaceValue_2"];
            Deposits_Notes_Denom_2 = (int)rdr["Deposits_Notes_Denom_2"];
            Dep_FaceValue_3 = (int)rdr["Dep_FaceValue_3"];
            Deposits_Notes_Denom_3 = (int)rdr["Deposits_Notes_Denom_3"];
            Dep_FaceValue_4 = (int)rdr["Dep_FaceValue_4"];
            Deposits_Notes_Denom_4 = (int)rdr["Deposits_Notes_Denom_4"];

            Repl_Load_Excel_Date = (DateTime)rdr["Repl_Load_Excel_Date"];
            Repl_Load_Status = (int)rdr["Repl_Load_Status"];
            Repl_Load_Action = (string)rdr["Repl_Load_Action"];

            Repl_UnLoad_Excel_Date = (DateTime)rdr["Repl_UnLoad_Excel_Date"];
            Repl_UnLoad_Status = (int)rdr["Repl_UnLoad_Status"];
            Repl_UnLoad_Action = (string)rdr["Repl_UnLoad_Action"];

            SM_Loaded = (decimal)rdr["SM_Loaded"];
            SM_Unloaded_Cassette = (decimal)rdr["SM_Unloaded_Cassette"];
            SM_Unloaded_Deposits = (decimal)rdr["SM_Unloaded_Deposits"];

            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];  // AUDI 

            Operator = (string)rdr["Operator"]; //  AUDI


        }

        string WTableId;
        //
        // READ CIT_G4S_Repl_Entries to fill table 
        //
        int SaveMode;
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable(string InOperator, string InSignedId, string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            SaveMode = InMode;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM " + WTableId
               + InSelectionCriteria;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableG4SEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            InsertWReportAtmRepl(InSignedId);
        }
        // Show table Feeding
        public decimal WTotalLoaded;
        public int WTotalEntries;
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding(string InOperator, string InSignedId, string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            SaveMode = InMode;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            TotalSelected = 0;

            SqlString = "SELECT SeqNo, Repl_Load_Excel_Date as ExcelDay,AtmNo "
                       + " , Cash_Loaded As FeedingCash  "
                       + " , Load_FaceValue_1, Load_Cassette_1  "
                       + " , Load_FaceValue_2, Load_Cassette_2  "
                       + " , Load_FaceValue_3, Load_Cassette_3  "
                       + " , Load_FaceValue_4, Load_Cassette_4  "
                   + " FROM " + WTableId
                   + InSelectionCriteria;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableG4SEntries);

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

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT ISNULL(SUM(Cash_Loaded),0) As TotalLoaded, Count(*) As TotalEntries"
                   + " FROM " + WTableId
                    + InSelectionCriteria;

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

                            //                     public decimal WTotalLoaded;
                            //public int TotalEntries;

                            WTotalLoaded = (decimal)rdr["TotalLoaded"];

                            WTotalEntries = (int)rdr["TotalEntries"];

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
        public int WTotalValidEntries;
        public int WTotalInvalidEntries;
        public decimal WTotalValidAmt;
        public decimal WTotalInvalidAmt;

        // LOAD ENTRIES
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding_WithMatchStatus(string InOperator, string InSignedId, string InSelectionCriteria, int InMode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            bool GoodForTable = false;

            int AlertDays = 0;
            //
            // Set up the table 
            //
            WTableId = TableA;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            // DATA TABLE ROWS DEFINITION 
            DataTableG4SEntries.Columns.Add("SeqNo", typeof(int));
            DataTableG4SEntries.Columns.Add("Matched", typeof(bool));
            DataTableG4SEntries.Columns.Add("Repl_Load_Status", typeof(int));

            DataTableG4SEntries.Columns.Add("AtmNo", typeof(string));
            DataTableG4SEntries.Columns.Add("FeededCash", typeof(decimal));
            //DataTableG4SEntries.Columns.Add("CIT_Unloaded", typeof(decimal));

            DataTableG4SEntries.Columns.Add("Comment", typeof(string));
            //  [ReplDateG4S]
            DataTableG4SEntries.Columns.Add("CIT_Repl_DATE", typeof(DateTime));

            DataTableG4SEntries.Columns.Add("ExcelDay", typeof(DateTime));
            DataTableG4SEntries.Columns.Add("Load_FaceValue_1", typeof(int));
            DataTableG4SEntries.Columns.Add("Load_Cassette_1", typeof(int));

            DataTableG4SEntries.Columns.Add("Load_FaceValue_2", typeof(int));
            DataTableG4SEntries.Columns.Add("Load_Cassette_2", typeof(int));

            DataTableG4SEntries.Columns.Add("Load_FaceValue_3", typeof(int));
            DataTableG4SEntries.Columns.Add("Load_Cassette_3", typeof(int));

            DataTableG4SEntries.Columns.Add("Load_FaceValue_4", typeof(int));
            DataTableG4SEntries.Columns.Add("Load_Cassette_4", typeof(int));



            WTotalValidEntries = 0;
            WTotalInvalidEntries = 0;

            WTotalValidAmt = 0;
            WTotalInvalidAmt = 0;

            // int Color = 11;

            SqlString = " SELECT * "
               + " FROM " + WTableId
               + InSelectionCriteria;

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            DataRow RowSelected = DataTableG4SEntries.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            if (Cash_Loaded == SM_Loaded)
                            {
                                RowSelected["Matched"] = true;
                                WTotalValidEntries = WTotalValidEntries + 1;
                                WTotalValidAmt = WTotalValidAmt + Cash_Loaded;


                            }
                            else
                            {
                                RowSelected["Matched"] = false;
                                WTotalInvalidEntries = WTotalInvalidEntries + 1;
                                WTotalInvalidAmt = WTotalInvalidAmt + Cash_Loaded;

                            }

                            RowSelected["Repl_Load_Status"] = Repl_Load_Status;

                            //RowSelected["CIT_Unloaded"] = UnloadedCounted;
                            RowSelected["Comment"] = RemarksG4S ;

                            RowSelected["AtmNo"] = AtmNo;

                            RowSelected["FeededCash"] = Cash_Loaded;

                            //  [ReplDateG4S]

                            RowSelected["CIT_Repl_DATE"] = ReplDateG4S.Date;


                            RowSelected["ExcelDay"] = Repl_Load_Excel_Date;


                            RowSelected["Load_FaceValue_1"] = Load_FaceValue_1;
                            RowSelected["Load_Cassette_1"] = Load_Cassette_1;

                            RowSelected["Load_FaceValue_2"] = Load_FaceValue_2;
                            RowSelected["Load_Cassette_2"] = Load_Cassette_2;

                            RowSelected["Load_FaceValue_3"] = Load_FaceValue_3;
                            RowSelected["Load_Cassette_3"] = Load_Cassette_3;

                            RowSelected["Load_FaceValue_4"] = Load_FaceValue_4;
                            RowSelected["Load_Cassette_4"] = Load_Cassette_4;



                            DataTableG4SEntries.Rows.Add(RowSelected);



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


        // UNLOAD ENTRIES
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_UNLOAD_WithMatchStatus(string InOperator, string InSignedId, string InSelectionCriteria, int InMode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            bool GoodForTable = false;

            int AlertDays = 0;
            //
            // Set up the table 
            //
            WTableId = TableA;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();


            // DATA TABLE ROWS DEFINITION 
            DataTableG4SEntries.Columns.Add("SeqNo", typeof(int));
            DataTableG4SEntries.Columns.Add("Matched", typeof(bool));

            DataTableG4SEntries.Columns.Add("Repl_UnLoad_Status", typeof(int));

            DataTableG4SEntries.Columns.Add("AtmNo", typeof(string));

            DataTableG4SEntries.Columns.Add("UnloadedCounted", typeof(decimal));
            DataTableG4SEntries.Columns.Add("Deposits", typeof(decimal));
            DataTableG4SEntries.Columns.Add("PresentedErrors", typeof(decimal));

            DataTableG4SEntries.Columns.Add("Comment", typeof(string));
            
            DataTableG4SEntries.Columns.Add("CIT_Repl_DATE", typeof(DateTime));

            DataTableG4SEntries.Columns.Add("ExcelDay", typeof(DateTime));


            DataTableG4SEntries.Columns.Add("Repl_UnLoad_Action", typeof(string));

            DataTableG4SEntries.Columns.Add("Un_Load_FaceValue_1", typeof(int));
            DataTableG4SEntries.Columns.Add("Un_Load_Cassette_1", typeof(int));

            DataTableG4SEntries.Columns.Add("Un_Load_FaceValue_2", typeof(int));
            DataTableG4SEntries.Columns.Add("Un_Load_Cassette_2", typeof(int));

            DataTableG4SEntries.Columns.Add("Un_Load_FaceValue_3", typeof(int));
            DataTableG4SEntries.Columns.Add("Un_Load_Cassette_3", typeof(int));

            DataTableG4SEntries.Columns.Add("Un_Load_FaceValue_4", typeof(int));
            DataTableG4SEntries.Columns.Add("Un_Load_Cassette_4", typeof(int));


            DataTableG4SEntries.Columns.Add("Dep_FaceValue_1", typeof(int));
            DataTableG4SEntries.Columns.Add("Deposits_Notes_Denom_1", typeof(int));

            DataTableG4SEntries.Columns.Add("Dep_FaceValue_2", typeof(int));
            DataTableG4SEntries.Columns.Add("Deposits_Notes_Denom_2", typeof(int));

            DataTableG4SEntries.Columns.Add("Dep_FaceValue_3", typeof(int));
            DataTableG4SEntries.Columns.Add("Deposits_Notes_Denom_3", typeof(int));

            DataTableG4SEntries.Columns.Add("Dep_FaceValue_4", typeof(int));
            DataTableG4SEntries.Columns.Add("Deposits_Notes_Denom_4", typeof(int));

            WTotalValidEntries = 0;
            WTotalInvalidEntries = 0;

            WTotalValidAmt = 0;
            WTotalInvalidAmt = 0;

            // int Color = 11;

            SqlString = " SELECT * "
               + " FROM " + WTableId
               + InSelectionCriteria;

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            DataRow RowSelected = DataTableG4SEntries.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            if (Repl_UnLoad_Status == 4 & PresentedErrors ==0)
                            {
                                RowSelected["Matched"] = true;
                                WTotalValidEntries = WTotalValidEntries + 1;
                                WTotalValidAmt = WTotalValidAmt + UnloadedCounted + Deposits;
                            }
                            else
                            {
                                RowSelected["Matched"] = false;
                                WTotalInvalidEntries = WTotalInvalidEntries + 1;
                                WTotalInvalidAmt = WTotalInvalidAmt + UnloadedCounted + Deposits;

                            }
                            //Repl_UnLoad_Status
                            RowSelected["Repl_UnLoad_Status"] = Repl_UnLoad_Status;
                         
                            RowSelected["AtmNo"] = AtmNo;


                            RowSelected["UnloadedCounted"] = UnloadedCounted;
                            RowSelected["Deposits"] = Deposits;
                            RowSelected["PresentedErrors"] = PresentedErrors;

                            RowSelected["Comment"] = RemarksG4S;
                            //  [ReplDateG4S]

                            RowSelected["CIT_Repl_DATE"] = ReplDateG4S.Date;


                            RowSelected["ExcelDay"] = Repl_Load_Excel_Date;

                            // Action
                            RowSelected["Repl_UnLoad_Action"] = Repl_UnLoad_Action;

                            // Based on the parameter we we do the below 
                            // If CIT+Presenter = with SM 

                            if (PresentedErrors > 0 & Repl_UnLoad_Status == 4)
                            {

                                RowSelected["Repl_UnLoad_Action"] = "It will be moved to Repl Workflow due to presenter ";
                            }


                            RowSelected["Un_Load_FaceValue_1"] = Un_Load_FaceValue_1;
                            RowSelected["Un_Load_Cassette_1"] = Load_Cassette_1;

                            RowSelected["Un_Load_FaceValue_2"] = Load_FaceValue_2;
                            RowSelected["Un_Load_Cassette_2"] = Load_Cassette_2;

                            RowSelected["Un_Load_FaceValue_3"] = Load_FaceValue_3;
                            RowSelected["Un_Load_Cassette_3"] = Load_Cassette_3;

                            RowSelected["Un_Load_FaceValue_4"] = Load_FaceValue_4;
                            RowSelected["Un_Load_Cassette_4"] = Load_Cassette_4;

                            RowSelected["Dep_FaceValue_1"] = Dep_FaceValue_1;
                            RowSelected["Deposits_Notes_Denom_1"] = Deposits_Notes_Denom_1;

                            RowSelected["Dep_FaceValue_2"] = Dep_FaceValue_2;
                            RowSelected["Deposits_Notes_Denom_2"] = Deposits_Notes_Denom_2;

                            RowSelected["Dep_FaceValue_3"] = Dep_FaceValue_3;
                            RowSelected["Deposits_Notes_Denom_3"] = Deposits_Notes_Denom_3;

                            RowSelected["Dep_FaceValue_4"] = Dep_FaceValue_4;
                            RowSelected["Deposits_Notes_Denom_4"] = Deposits_Notes_Denom_4;

                            RowSelected["Repl_UnLoad_Status"] = Repl_UnLoad_Status;

                            DataTableG4SEntries.Rows.Add(RowSelected);



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



        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_Feeding_Alerts(string InOperator)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            bool GoodForTable = false;

            int AlertDays = 0;
            //
            // Set up the table 
            //
            WTableId = TableA;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            // DATA TABLE ROWS DEFINITION 
            DataTableG4SEntries.Columns.Add("SeqNo", typeof(int));
            DataTableG4SEntries.Columns.Add("Color", typeof(int));
            DataTableG4SEntries.Columns.Add("AtmNo", typeof(string));
            DataTableG4SEntries.Columns.Add("CITId", typeof(string));
            DataTableG4SEntries.Columns.Add("OriginFileName", typeof(string));
            DataTableG4SEntries.Columns.Add("OrderNo", typeof(string));
            DataTableG4SEntries.Columns.Add("Cash_Loaded", typeof(decimal));
            DataTableG4SEntries.Columns.Add("CreatedDate", typeof(DateTime));

            DataTableG4SEntries.Columns.Add("ExpectedDate", typeof(DateTime));

            DataTableG4SEntries.Columns.Add("AlertDays", typeof(int));

            DataTableG4SEntries.Columns.Add("CurrentCash", typeof(decimal));

            TotalSelected = 0;

            int Color = 11;

            SqlString = "SELECT * "
                     + " FROM " + WTableId
               + "  WHERE (ProcessMode_Load = 1 OR ProcessMode_UnLoad = 1) "; // Not replenished yet

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            Ac.ReadAtm(AtmNo);
                            if (Ac.RecordFound == true)
                            {

                                int Difference = (DateTime.Now.Date - CreatedDate.Date).Days;

                                if (Difference > Ac.ReplAlertDays)
                                {
                                    // Alert to be created 
                                    GoodForTable = true;

                                    AlertDays = Difference - Ac.ReplAlertDays;
                                }
                                else
                                {
                                    GoodForTable = false;
                                }
                            }

                            switch (Color)
                            {
                                case 11:
                                    {
                                        Color = 12;
                                        break;
                                    }
                                case 12:
                                    {
                                        Color = 11;
                                        break;
                                    }
                            }
                            if (GoodForTable == true)
                            {
                                DataRow RowSelected = DataTableG4SEntries.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Color"] = Color;
                                RowSelected["AtmNo"] = AtmNo;

                                RowSelected["CITId"] = CITId;
                                RowSelected["OriginFileName"] = OriginFileName;
                                RowSelected["OrderNo"] = OrderNo;

                                RowSelected["Cash_Loaded"] = Cash_Loaded;
                                RowSelected["CreatedDate"] = CreatedDate;

                                RowSelected["CreatedDate"] = CreatedDate.Date;

                                RowSelected["ExpectedDate"] = CreatedDate.Date.AddDays(Ac.ReplAlertDays);

                                RowSelected["AlertDays"] = AlertDays;

                                Na.ReadSessionsNotesAndValues(AtmNo, ReplCycleNo, 1);

                                RowSelected["CurrentCash"] = Na.Balances1.ReplToRepl;

                                // ADD ROW
                                DataTableG4SEntries.Rows.Add(RowSelected);
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
        // READ CIT_G4S_Repl_Entries to fill table by Date 
        //
        public void ReadCIT_G4S_Repl_EntriesToFillDataTableAND_Totals(string InOperator, string InSignedId,
                                                           string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            SaveMode = InMode;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 
            if (InOperator == "ETHNCY2N")
            {

                SqlString = "SELECT * "
                   + " FROM " + WTableId
                   //  + " WHERE CAST(ReplDateG4S AS Date) >= @ReplDateG4SFrom  AND CAST(ReplDateG4S AS Date) <= @ReplDateG4STo "
                   + InSelectionCriteria;
            }
            else
            {

                SqlString = "SELECT * "
                   + " FROM " + WTableId
                //   + " WHERE CITId = @CITId AND CAST(ReplDateG4S AS Date) >= @ReplDateG4SFrom  AND CAST(ReplDateG4S AS Date) <= @ReplDateG4STo "
                   + InSelectionCriteria;
            }


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.Fill(DataTableG4SEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            // Calculate TOTALS
            Loaded_Total = 0;
            Loaded_ATMS = 0;

            TotalNotProcessed = 0;
            Total11 = 0;
            TotalAA = 0;

            Total10 = 0;
            TotalShort = 0;
            TotalPresenter = 0;

            TotalShortAmt = 0;
            TotalPresenterAmt = 0;

            int I = 0;

            while (I <= (DataTableG4SEntries.Rows.Count - 1))
            {

                RecordFound = true;

                SeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];

                Mask = (string)DataTableG4SEntries.Rows[I]["Mask"];
                ShortFound = (decimal)DataTableG4SEntries.Rows[I]["ShortFound"];
                PresentedErrors = (decimal)DataTableG4SEntries.Rows[I]["PresentedErrors"];
                Cash_Loaded = (decimal)DataTableG4SEntries.Rows[I]["Cash_Loaded"];

                if (Cash_Loaded > 0)
                {
                    Loaded_Total = Loaded_Total + Cash_Loaded;
                    Loaded_ATMS = Loaded_ATMS + 1;
                }

                if (Mask == "") TotalNotProcessed = TotalNotProcessed + 1;
                if (Mask == "11") Total11 = Total11 + 1;
                if (Mask == "AA") TotalAA = TotalAA + 1;
                if (Mask == "10") Total10 = Total10 + 1;

                if (ShortFound > 0)
                {
                    TotalShort = TotalShort + 1;
                    TotalShortAmt = TotalShortAmt + ShortFound;
                }

                if (PresentedErrors > 0)
                {
                    TotalPresenter = TotalPresenter + 1;
                    TotalPresenterAmt = TotalPresenterAmt + PresentedErrors;
                }

                I++;

            }

            InsertWReportAtmRepl(InSignedId);

            // FIND WHAT ATMS NEED FOR GL RECONCILIATION - NBG CASE 

            ReadCIT_G4S_Repl_Entries_For_Banks_Get_Right_Entries();
        }

        //
        // READ CIT_G4S_Repl_Entries to fill table by Date 
        //

        public void ReadCIT_G4S_Repl_EntriesToFillDataTableAND_TotalsByDateRange(string InOperator, string InSignedId, string InCitId, DateTime InDateFrom, DateTime InDateTo,
                                                           string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            SaveMode = InMode;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 
            if (InCitId == "")
            {

                SqlString = "SELECT * "
                   + " FROM " + WTableId
                   + " WHERE Operator = @Operator AND CAST(ReplDateG4S AS Date) >= @ReplDateG4SFrom  AND CAST(ReplDateG4S AS Date) <= @ReplDateG4STo "
                   + InSelectionCriteria;
            }
            else
            {

                SqlString = "SELECT * "
                   + " FROM " + WTableId
                   + " WHERE CITId = @CITId AND CAST(ReplDateG4S AS Date) >= @ReplDateG4SFrom  AND CAST(ReplDateG4S AS Date) <= @ReplDateG4STo "
                   + InSelectionCriteria;
            }


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        if (InCitId == "")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4SFrom", InDateFrom);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4STo", InDateTo);
                        }
                        else
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@CITId", InCitId);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4SFrom", InDateFrom);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4STo", InDateTo);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableG4SEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            // Calculate TOTALS

            TotalNotProcessed = 0;
            Total11 = 0;
            TotalAA = 0;
            Total01 = 0;
            TotalShort = 0;
            TotalPresenter = 0;

            int I = 0;

            while (I <= (DataTableG4SEntries.Rows.Count - 1))
            {

                RecordFound = true;

                SeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];

                Mask = (string)DataTableG4SEntries.Rows[I]["Mask"];
                ShortFound = (decimal)DataTableG4SEntries.Rows[I]["ShortFound"];
                PresentedErrors = (decimal)DataTableG4SEntries.Rows[I]["PresentedErrors"];

                if (Mask == "") TotalNotProcessed = TotalNotProcessed + 1;
                if (Mask == "11") Total11 = Total11 + 1;
                if (Mask == "AA") TotalAA = TotalAA + 1;
                if (Mask == "01") Total01 = Total01 + 1;

                if (ShortFound > 0) TotalShort = TotalShort + 1;

                if (PresentedErrors > 0) TotalPresenter = TotalPresenter + 1;

                I++;

            }

            InsertWReportAtmRepl(InSignedId);

            // FIND WHAT ATMS NEED FOR GL RECONCILIATION - NBG CASE 

            ReadCIT_G4S_Repl_Entries_For_Banks_Get_Right_Entries();
        }


        decimal OpeningBalance1;
        decimal Dispensed1;
        decimal UnloadedMachine1;
        decimal Cash_Loaded1;
        decimal Deposits1;
        decimal PresentedErrors1;
        decimal AmtCheckedForMatching1;
        decimal OtherJournalErrors1;
        decimal OrderToBeLoaded1;
        string RemarksRRDM1;

        // Insert  Replenishment Report 
        private void InsertWReportAtmRepl(string InSignedId)
        {

            try
            {

                //Clear Table 
                DeleteReportAtmRepl(InSignedId);

                int I = 0;

                while (I <= (DataTableG4SEntries.Rows.Count - 1))
                {

                    RecordFound = true;

                    int SaveSeqNo = SeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];

                    ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, SaveMode);

                    //  
                    // Read The Matched In Bank 
                    //
                    int TempMode = 2; //  for the Bank 
                    ReadCIT_G4S_Repl_EntriesByATMandDate(AtmNo, ReplDateG4S.Date, TempMode);

                    if (RecordFound == true)
                    {
                        OpeningBalance1 = OpeningBalance;
                        Dispensed1 = Dispensed;
                        UnloadedMachine1 = UnloadedMachine;
                        Cash_Loaded1 = Cash_Loaded;
                        Deposits1 = Deposits;
                        PresentedErrors1 = PresentedErrors;
                        AmtCheckedForMatching1 = AmtCheckedForMatching;
                        OtherJournalErrors1 = OtherJournalErrors;
                        OrderToBeLoaded1 = OrderToBeLoaded;
                        RemarksRRDM1 = RemarksRRDM;
                    }
                    else
                    {
                        OpeningBalance1 = 0;
                        Dispensed1 = 0;
                        UnloadedMachine1 = 0;
                        Cash_Loaded1 = 0;
                        Deposits1 = 0;

                        PresentedErrors1 = 0;
                        AmtCheckedForMatching1 = 0;
                        OtherJournalErrors1 = 0;
                        OrderToBeLoaded1 = 0;
                        RemarksRRDM1 = "No Record Found";
                    }
                    if (SaveMode == 2)
                    {
                        // No Record in G4S
                        OpeningBalance = 0;

                        Dispensed = 0;
                        UnloadedMachine = 0;
                        UnloadedCounted = 0;
                        Cash_Loaded = 0;
                        Deposits = 0;
                        OverFound = 0;
                        ShortFound = 0;

                        RemarksG4S = "No Record in G4S";
                    }
                    else
                    {
                        // Read Again the G4S
                        ReadCIT_G4S_Repl_EntriesBySeqNo(SaveSeqNo, SaveMode);
                    }


                    // Insert record for printing 
                    //
                    InsertReportAtmRepl(InSignedId);

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

        }

        // Insert  Replenishment Report 
        private void InsertWReportAtmRepl_2(string InSignedId)
        {

            try
            {

                //Clear Table 
                DeleteReportAtmRepl(InSignedId);

                int I = 0;

                while (I <= (DataTableG4SEntries.Rows.Count - 1))
                {

                    RecordFound = true;

                    int SaveSeqNo = SeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];

                    ReadCIT_G4S_Repl_EntriesBySeqNo(SeqNo, SaveMode);

                    //  
                    // Read The Matched In Bank 
                    //
                    int TempMode = 2;
                    ReadCIT_G4S_Repl_EntriesByATMandDate(AtmNo, ReplDateG4S.Date, TempMode);

                    if (RecordFound == true)
                    {
                        OpeningBalance1 = OpeningBalance;
                        Dispensed1 = Dispensed;
                        UnloadedMachine1 = UnloadedMachine;
                        Cash_Loaded1 = Cash_Loaded;
                        Deposits1 = Deposits;
                        PresentedErrors1 = PresentedErrors;
                        AmtCheckedForMatching1 = AmtCheckedForMatching;
                        OtherJournalErrors1 = OtherJournalErrors;
                        OrderToBeLoaded1 = OrderToBeLoaded;
                        RemarksRRDM1 = RemarksRRDM;
                    }
                    else
                    {
                        OpeningBalance1 = 0;
                        Dispensed1 = 0;
                        UnloadedMachine1 = 0;
                        Cash_Loaded1 = 0;
                        Deposits1 = 0;

                        PresentedErrors1 = 0;
                        AmtCheckedForMatching1 = 0;
                        OtherJournalErrors1 = 0;
                        OrderToBeLoaded1 = 0;
                        RemarksRRDM1 = "No Record Found";
                    }
                    if (SaveMode == 2)
                    {
                        // No Record in G4S
                        OpeningBalance = 0;

                        Dispensed = 0;
                        UnloadedMachine = 0;
                        UnloadedCounted = 0;
                        Cash_Loaded = 0;
                        Deposits = 0;
                        OverFound = 0;
                        ShortFound = 0;

                        RemarksG4S = "No Record in G4S";

                        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
                        Ta.ReadSessionsStatusTraces(AtmNo, ReplCycleNo);

                        if (Ta.ProcessMode > 0)
                        {
                            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
                            Na.ReadSessionsNotesAndValues(AtmNo, ReplCycleNo, 2);

                            // Fill the fields up

                            //textBoxOpenBal1.Text = Na.Balances1.OpenBal.ToString("#,##0.00");
                            //textBoxDispensed1.Text = (Na.Balances1.OpenBal - Na.Balances1.ReplToRepl).ToString("#,##0.00");
                            //textBoxClosingCash1.Text = Na.Balances1.ReplToRepl.ToString("#,##0.00");
                            //textBoxMachineCounters1.Text = Na.Balances1.MachineBal.ToString("#,##0.00");
                            //textBoxDifference1.Text = (Na.Balances1.ReplToRepl - Na.Balances1.MachineBal).ToString("#,##0.00");
                            OpeningBalance = Na.Balances1.OpenBal;
                            UnloadedMachine = Na.Balances1.MachineBal;
                            UnloadedCounted = Na.Balances1.CountedBal;
                            // Will be taken from Deposits Analysis
                            // Deposits = Na.D
                            Cash_Loaded = Na.Cit_Loaded;

                        }

                    }
                    else
                    {
                        // Read Again the G4S
                        ReadCIT_G4S_Repl_EntriesBySeqNo(SaveSeqNo, SaveMode);
                    }


                    // Insert record for printing 
                    //
                    InsertReportAtmRepl(InSignedId);

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

        }

        //
        // READ CIT_G4S_Repl_Entries to fill table for Grid 2
        //

        public void ReadCIT_G4S_Repl_EntriesToFillDataTableForGrid2(string InOperator, string InSignedId, string InSelectionCriteria, int InMode, DateTime InReplDateG4S)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            SaveMode = InMode;

            DataTableG4SEntries = new DataTable();
            DataTableG4SEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM " + WTableId
               + InSelectionCriteria
               + " AND CAST(ReplDateG4S AS DATE) = @ReplDateG4S";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4S", InReplDateG4S.Date);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableG4SEntries);

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
        // READ CIT_G4S_Repl_Entries by Selection Criteria 
        //
        public void ReadCIT_G4S_Repl_EntriesBySelectionCriteria(string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
               + " FROM " + WTableId
               + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FieldName", InFieldName);

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
        // READ CIT_G4S_Repl_Entries by Selection Criteria and fill table with revevant Atms
        //
        DateTime Last_Cut_Off_Date;
        public void ReadCIT_G4S_Repl_Entries_For_Banks_Get_Right_Entries()
        {
            int SavedSeqNo = 0;
            string SavedAtmNo = "";
            bool Valid = false;

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            DataTableG4SEntriesSelectedForExcel = new DataTable();
            DataTableG4SEntriesSelectedForExcel.Clear();

            DataTableG4SEntriesSelectedForExcel.Columns.Add("SeqNo", typeof(int));
            DataTableG4SEntriesSelectedForExcel.Columns.Add("AtmNo", typeof(string));
            DataTableG4SEntriesSelectedForExcel.Columns.Add("Last_Cut_Off_Date", typeof(DateTime));

            int I = 0;
            int K = DataTableG4SEntries.Rows.Count;

            while (I <= (K - 1))
            {

                RecordFound = true;

                SeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];
                CITId = (string)DataTableG4SEntries.Rows[I]["CITId"];
                AtmNo = (string)DataTableG4SEntries.Rows[I]["AtmNo"];
                ProcessMode_Load = (int)DataTableG4SEntries.Rows[I]["ProcessMode_Load"];
                ReplDateG4S = (DateTime)DataTableG4SEntries.Rows[I]["ReplDateG4S"];
                ReplCycleNo = (int)DataTableG4SEntries.Rows[I]["ReplCycleNo"];

                if (ProcessMode_Load == -2)
                {
                    Ta.FindNextAndLastReplCycleId(AtmNo);

                    if (Ta.LastReconciled > 0)
                    {
                        if ((CITId != "1000" & ReplCycleNo <= Ta.LastReconciled)
                                || (CITId != "1000" & ReplCycleNo <= Ta.LastReconReady)
                                || (CITId == "1000" & ReplCycleNo != Ta.LastReconReady)
                                     )
                        {
                            // These will not be included in Excel
                            I = I + 1;
                            continue;
                        }
                    }

                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                    Rjc.Find_GL_Cut_Off_Before_GivenDate(Operator, ReplDateG4S.Date);
                    if (Rjc.RecordFound == true & Rjc.Counter == 0)
                    {
                        Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                        Valid = true;
                    }
                    else
                    {
                        Valid = false;
                    }

                    if (AtmNo == SavedAtmNo || Valid == false)
                    {
                        SavedSeqNo = SeqNo;
                        SavedAtmNo = AtmNo;
                        // Skip 
                    }
                    else
                    {

                        SavedSeqNo = SeqNo;
                        SavedAtmNo = AtmNo;
                        // New Atm
                        DataRow RowSelected = DataTableG4SEntriesSelectedForExcel.NewRow();

                        RowSelected["SeqNo"] = SeqNo;
                        RowSelected["AtmNo"] = AtmNo;
                        RowSelected["Last_Cut_Off_Date"] = Last_Cut_Off_Date;

                        // ADD ROW
                        DataTableG4SEntriesSelectedForExcel.Rows.Add(RowSelected);

                    }
                }

                I++;

            }

        }

        //
        // READ by ATM No and Repl Cycle No
        // READ AT POINT OF UPDAting 
        //
        public void ReadCIT_G4S_Repl_EntriesByAtmNoAndReplCycleNo(string InAtmNo, int InReplCycleNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
               + " FROM " + WTableId
               + " WHERE AtmNo = @AtmNo AND ReplCycleNo = @ReplCycleNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

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
        // READ CIT_G4S_Repl_Entries by Full Selection criteria 
        //
        public void ReadCIT_G4S_Repl_EntriesByATMandDate(string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
               + " FROM " + WTableId
               + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        //cmd.Parameters.AddWithValue("@ReplDateG4S", InDate);

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
        // READ CIT_G4S_Repl_Entries by ATM No, Date Of Replenishemnet
        //
        public void ReadCIT_G4S_Repl_EntriesByATMandDate(string InAtmNo, DateTime InDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
               + " FROM " + WTableId
               + " WHERE AtmNo = @AtmNo AND CAST(ReplDateG4S AS Date) = @ReplDateG4S ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplDateG4S", InDate);

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
        // READ CIT_G4S_Repl_Entries by SequenceNumber 
        //
        public void ReadCIT_G4S_Repl_EntriesBySeqNo(int InSeqNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
                 + " FROM " + WTableId
                 + " WHERE SeqNo = @SeqNo";

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
        // READ CIT_G4S_Repl_Entries by Totals
        //
        public void ReadCIT_G4S_Repl_EntriesTOTALS(string InCITId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalNotProcessed = 0;
            Total11 = 0;
            TotalAA = 0;
            Total01 = 0;
            TotalShort = 0;
            TotalPresenter = 0;

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string SqlString = "SELECT *"
                 + " FROM " + WTableId
                 + " WHERE CITId = @CITId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CITId", InCITId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            if (Mask == "") TotalNotProcessed = TotalNotProcessed + 1;
                            if (Mask == "11") Total11 = Total11 + 1;
                            if (Mask == "AA") TotalAA = TotalAA + 1;
                            if (Mask == "01") Total01 = Total01 + 1;

                            if (ShortFound > 0) TotalShort = TotalShort + 1;

                            if (PresentedErrors > 0) TotalPresenter = TotalPresenter + 1;

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
        // Insert 
        //
        public int InsertCIT_G4S_Repl_EntriesRecord(int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string cmdinsert = "INSERT INTO " + WTableId
                    + " ("

                    + " [CITId]"
                    + ",[OriginFileName] "
                    + ",[ExcelDate] "
                    + ",[AtmNo] "
                    + ",[AtmName] "
                    + ",[LoadingExcelCycleNo] "
                    + ",[ReplDateG4S]"
                     + ",[OrderNo]"
                    + ",[CreatedDate]"
                    + ",[IsDeposit] "
                    + ",[OpeningBalance]"
                    + ",[Dispensed]"
                    + ",[UnloadedMachine]"
                    + ",[UnloadedCounted]"
                    + ",[Cash_Loaded]"
                    + ",[Deposits]"
                    + ",[OverFound]"
                    + ",[ShortFound]"
                    + ",[RemarksG4S]"
                    + ",[PresentedErrors]"
                    + ",[ReplCycleNo]"
                    + ",[Cut_Off_date]"
                    + ",[Gl_Balance_At_CutOff]"

                    + ",[Load_FaceValue_1]"
                    + ",[Load_Cassette_1]"
                      + ",[Load_FaceValue_2]"
                    + ",[Load_Cassette_2]"
                      + ",[Load_FaceValue_3]"
                    + ",[Load_Cassette_3]"
                      + ",[Load_FaceValue_4]"
                    + ",[Load_Cassette_4]"

                      + ",[Un_Load_FaceValue_1]"
                    + ",[Un_Load_Cassette_1]"
                      + ",[Un_Load_FaceValue_2]"
                    + ",[Un_Load_Cassette_2]"
                      + ",[Un_Load_FaceValue_3]"
                    + ",[Un_Load_Cassette_3]"
                      + ",[Un_Load_FaceValue_4]"
                    + ",[Un_Load_Cassette_4]"

                      + ",[Deposits_Notes_Denom_1]"
                      + ",[Deposits_Notes_Denom_2]"
                      + ",[Deposits_Notes_Denom_3]"
                      + ",[Deposits_Notes_Denom_4]"

                    + ",[Operator]"
                    + ",[LoadedAtRMCycle]"
                    + ")"
                    + " VALUES "
                    + " ("
                    + " @CITId"
                    + ",@OriginFileName "
                     + ",@ExcelDate "
                    + ",@AtmNo "
                    + ",@AtmName "
                    + ",@LoadingExcelCycleNo "
                    + ",@ReplDateG4S "
                    + ",@OrderNo "
                    + ",@CreatedDate "

                    + ",@IsDeposit "
                    + ",@OpeningBalance "
                    + ",@Dispensed "
                    + ",@UnloadedMachine "
                    + ",@UnloadedCounted "

                    + ",@Cash_Loaded "
                    + ",@Deposits "

                    + ",@OverFound "
                    + ",@ShortFound "
                       + ",@RemarksG4S "
                        + ",@PresentedErrors "
                        + ",@ReplCycleNo "
                        + ",@Cut_Off_date "
                    + ",@Gl_Balance_At_CutOff "

                     + ",@Load_FaceValue_1 "
                     + ",@Load_Cassette_1 "
                     + ",@Load_FaceValue_2 "
                     + ",@Load_Cassette_2 "
                     + ",@Load_FaceValue_3 "
                     + ",@Load_Cassette_3 "
                     + ",@Load_FaceValue_4 "
                     + ",@Load_Cassette_4 "

                     + ",@Un_Load_FaceValue_1 "
                     + ",@Un_Load_Cassette_1 "
                     + ",@Un_Load_FaceValue_2 "
                     + ",@Un_Load_Cassette_2 "
                     + ",@Un_Load_FaceValue_3 "
                     + ",@Un_Load_Cassette_3 "
                     + ",@Un_Load_FaceValue_4 "
                     + ",@Un_Load_Cassette_4 "

                     + ",@Deposits_Notes_Denom_1 "
                     + ",@Deposits_Notes_Denom_2 "
                     + ",@Deposits_Notes_Denom_3 "
                     + ",@Deposits_Notes_Denom_4 "

                    + ",@Operator "
                     + ",@LoadedAtRMCycle "
                    + ")  "
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CITId", CITId);

                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@ExcelDate", ExcelDate);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", LoadingExcelCycleNo);
                        cmd.Parameters.AddWithValue("@ReplDateG4S", ReplDateG4S);
                        cmd.Parameters.AddWithValue("@OrderNo", OrderNo);

                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        cmd.Parameters.AddWithValue("@IsDeposit", IsDeposit);
                        cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                        cmd.Parameters.AddWithValue("@Dispensed", Dispensed);

                        cmd.Parameters.AddWithValue("@UnloadedMachine", UnloadedMachine);

                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);

                        cmd.Parameters.AddWithValue("@Cash_Loaded", Cash_Loaded);

                        cmd.Parameters.AddWithValue("@Deposits", Deposits);

                        cmd.Parameters.AddWithValue("@OverFound", OverFound);
                        cmd.Parameters.AddWithValue("@ShortFound", ShortFound);

                        cmd.Parameters.AddWithValue("@RemarksG4S", RemarksG4S);
                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@Cut_Off_date", Cut_Off_date);

                        cmd.Parameters.AddWithValue("@Gl_Balance_At_CutOff", Gl_Balance_At_CutOff);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_1", Load_FaceValue_1);
                        cmd.Parameters.AddWithValue("@Load_Cassette_1", Load_Cassette_1);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_2", Load_FaceValue_2);
                        cmd.Parameters.AddWithValue("@Load_Cassette_2", Load_Cassette_2);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_3", Load_FaceValue_3);
                        cmd.Parameters.AddWithValue("@Load_Cassette_3", Load_Cassette_3);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_4", Load_FaceValue_4);
                        cmd.Parameters.AddWithValue("@Load_Cassette_4", Load_Cassette_4);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_1", Un_Load_FaceValue_1);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_1", Un_Load_Cassette_1);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_2", Un_Load_FaceValue_2);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_2", Un_Load_Cassette_2);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_3", Un_Load_FaceValue_3);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_3", Un_Load_Cassette_3);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_4", Un_Load_FaceValue_4);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_4", Un_Load_Cassette_4);

                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_1", Deposits_Notes_Denom_1);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_2", Deposits_Notes_Denom_2);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_3", Deposits_Notes_Denom_3);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_4", Deposits_Notes_Denom_4);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

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
        // Insert the FEEDING RECORD 
        //
        public int InsertCIT_G4S_ATM_Feeding_Entries(int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            string cmdinsert = "INSERT INTO " + WTableId
                    + " ("
                    + " [CITId]" // Feeding
                    + ",[OriginFileName] " // Feeding
                     + ",[ExcelDate] " // Feeding

                    + ",[AtmNo] " // Feeding
                    + ",[AtmName] " // Feeding
                    + ",[LoadingExcelCycleNo] " // Feeding

                     + ",[OrderNo]" // Feeding
                    + ",[CreatedDate]" // Feeding
                    + ",[Cash_Loaded]" // Feeding     

                    + ",[ReplCycleNo]" // Feeding
                    + ",[Operator]" // Feeding
                    + ",[LoadedAtRMCycle]"  // Feeding 
                    + ")"
                    + " VALUES "
                    + " ("
                    + " @CITId"
                    + ",@OriginFileName "
                      + ",@ExcelDate "

                    + ",@AtmNo "
                    + ",@AtmName "
                    + ",@LoadingExcelCycleNo "

                    + ",@OrderNo "
                    + ",@CreatedDate "
                    + ",@Cash_Loaded "

                    + ",@ReplCycleNo "
                    + ",@Operator "
                     + ",@LoadedAtRMCycle "
                    + ")  "
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CITId", CITId);

                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@ExcelDate", ExcelDate);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", LoadingExcelCycleNo);

                        cmd.Parameters.AddWithValue("@OrderNo", OrderNo);
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Cash_Loaded", Cash_Loaded);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

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
        // UpdateCIT_G4S_Repl_EntriesRecord - PrcessMode_Load
        // 
        public void UpdateCIT_G4S_Repl_EntriesProcessMode_Load(int InSeqNo, int InProcessMode_Load, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                               + "ProcessMode_Load = @ProcessMode_Load "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ProcessMode_Load", InProcessMode_Load);

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
        // UpdateCIT_G4S_Repl_EntriesRecord - PrcessMode_UnLoad
        // 
        public void UpdateCIT_G4S_Repl_EntriesProcessMode_UnLoad(int InSeqNo, int InProcessMode_UnLoad, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                               + "ProcessMode_UnLoad = @ProcessMode_UnLoad "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ProcessMode_UnLoad", InProcessMode_UnLoad);

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
        // UpdateCIT_G4S_Repl_EntriesRecord - STAGES OF LOADING
        // 
        public void UpdateCIT_G4S_Repl_EntriesFrom_SM(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                               + "SM_Loaded = @SM_Loaded "
                              + " ,SM_Unloaded_Cassette = @SM_Unloaded_Cassette "
                              + ",SM_Unloaded_Deposits = @SM_Unloaded_Deposits "
                               + ",PresentedErrors = @PresentedErrors "

                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SM_Loaded", SM_Loaded);

                        cmd.Parameters.AddWithValue("@SM_Unloaded_Cassette", SM_Unloaded_Cassette);

                        cmd.Parameters.AddWithValue("@SM_Unloaded_Deposits", SM_Unloaded_Deposits);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

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
        // UpdateCIT_G4S_Repl_EntriesRecord - STAGES OF LOADING
        // 
        public void UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                               + "Repl_Load_Excel_Date = @Repl_Load_Excel_Date "
                               + ",RemarksG4S = @RemarksG4S "
                              + " ,Repl_Load_Status = @Repl_Load_Status "
                              + ",Repl_Load_Action = @Repl_Load_Action "
                               + ",Repl_UnLoad_Excel_Date = @Repl_UnLoad_Excel_Date "
                              + " ,Repl_UnLoad_Status = @Repl_UnLoad_Status "
                              + ",Repl_UnLoad_Action = @Repl_UnLoad_Action "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Repl_Load_Excel_Date", Repl_Load_Excel_Date);

                        cmd.Parameters.AddWithValue("@RemarksG4S", RemarksG4S);

                        cmd.Parameters.AddWithValue("@Repl_Load_Status", Repl_Load_Status);

                        cmd.Parameters.AddWithValue("@Repl_Load_Action", Repl_Load_Action);

                        cmd.Parameters.AddWithValue("@Repl_UnLoad_Excel_Date", Repl_UnLoad_Excel_Date);

                        cmd.Parameters.AddWithValue("@Repl_UnLoad_Status", Repl_UnLoad_Status);

                        cmd.Parameters.AddWithValue("@Repl_UnLoad_Action", Repl_UnLoad_Action);

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
        // UpdateCIT_G4S_Repl_EntriesRecord
        // 
        public void UpdateCIT_G4S_Repl_EntriesRecord(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                              + " LoadingExcelCycleNo = @LoadingExcelCycleNo "
                              + " ,ProcessMode_Load = @ProcessMode_Load "
                              + " ,ProcessMode_UnLoad = @ProcessMode_UnLoad "
                              + ",ReplCycleNo = @ReplCycleNo "
                              + " , RemarksG4S = @RemarksG4S "
                              // + ",Mask = @Mask "
                              + ",UnloadedCounted = @UnloadedCounted "
                              + ",PresentedErrors = @PresentedErrors "
                              + ",OverFound = @OverFound "
                              + ",ShortFound = @ShortFound "
                              + ",RemarksRRDM = @RemarksRRDM "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", LoadingExcelCycleNo);

                        cmd.Parameters.AddWithValue("@ProcessMode_Load", ProcessMode_Load);
                        cmd.Parameters.AddWithValue("@ProcessMode_UnLoad", ProcessMode_UnLoad);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@RemarksG4S", RemarksG4S); 

                        cmd.Parameters.AddWithValue("@Mask", Mask);

                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        cmd.Parameters.AddWithValue("@OverFound", OverFound);
                        cmd.Parameters.AddWithValue("@ShortFound", ShortFound);

                        cmd.Parameters.AddWithValue("@RemarksRRDM", RemarksRRDM);

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
        // UpdateCIT_G4S_Repl_EntriesRecord
        // 
        public void UpdateCIT_G4S_Repl_EntriesRecord_AtmNo_SesNo(string InAtmNo, int InReplNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                               + " LoadingExcelCycleNo = @LoadingExcelCycleNo "
                              + " ,ProcessMode_Load = @ProcessMode_Load "
                              + ",ReplCycleNo = @ReplCycleNo "
                              + ",Mask = @Mask "
                              + ",UnloadedCounted = @UnloadedCounted "
                              + ",PresentedErrors = @PresentedErrors "
                              + ",OverFound = @OverFound "
                              + ",ShortFound = @ShortFound "
                              + ",RemarksRRDM = @RemarksRRDM "
                              + ""
                              + " WHERE AtmNo = @AtmNo AND ReplCycleNo = @ReplCycleNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplNo);

                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", LoadingExcelCycleNo);

                        cmd.Parameters.AddWithValue("@ProcessMode_Load", ProcessMode_Load);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@Mask", Mask);

                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        cmd.Parameters.AddWithValue("@OverFound", OverFound);
                        cmd.Parameters.AddWithValue("@ShortFound", ShortFound);

                        cmd.Parameters.AddWithValue("@RemarksRRDM", RemarksRRDM);

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
        // UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead
        // At the time of Journal reading
        // 
        public void UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                              + " OrderNo = @OrderNo "
                              + ",OrderToBeLoaded = @OrderToBeLoaded "
                              + ",OpeningBalance = @OpeningBalance "
                              + ",Dispensed = @Dispensed "

                              + ",UnloadedMachine = @UnloadedMachine "
                              + ",UnloadedCounted = @UnloadedCounted "
                              + ",Deposits = @Deposits "
                              + ",Cash_Loaded = @Cash_Loaded "

                              + ",PresentedErrors = @PresentedErrors "
                              + ",OtherJournalErrors = @OtherJournalErrors "
                              + ",RemarksRRDM = @RemarksRRDM "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@OrderNo", OrderNo);

                        cmd.Parameters.AddWithValue("OrderToBeLoaded", OrderToBeLoaded);
                        cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                        cmd.Parameters.AddWithValue("@Dispensed", Dispensed);

                        cmd.Parameters.AddWithValue("@UnloadedMachine", UnloadedMachine);
                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);
                        cmd.Parameters.AddWithValue("@Deposits", Deposits);

                        cmd.Parameters.AddWithValue("@Cash_Loaded", Cash_Loaded);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        cmd.Parameters.AddWithValue("@OtherJournalErrors", OtherJournalErrors);

                        cmd.Parameters.AddWithValue("@RemarksRRDM", RemarksRRDM);

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
        // UpdateCIT_G4S_Repl_EntriesRecordDuringJournalRead
        // At the time of Journal reading
        // 
        public void UpdateCIT_G4S_Repl_EntriesRecordDuringExcelLoading(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "

                               + " [Load_FaceValue_1] = @Load_FaceValue_1 "
                               + "  ,[Load_Cassette_1] = @Load_Cassette_1 "
                               + "  ,[Load_FaceValue_2] = @Load_FaceValue_2 "
                                + " ,[Load_Cassette_2]  = @Load_Cassette_2 "
                               + "  ,[Load_FaceValue_3] = @Load_FaceValue_3 "
                                + " ,[Load_Cassette_3] = @Load_Cassette_3 "
                               + "  ,[Load_FaceValue_4]  = @Load_FaceValue_4 "
                                + " ,[Load_Cassette_4]  = @Load_Cassette_4 "
                               + "  ,[Cash_Loaded]  = @Cash_Loaded "
             + ",[Un_Load_FaceValue_1]  = @Un_Load_FaceValue_1 "
            + " ,[Un_Load_Cassette_1]  = @Un_Load_Cassette_1 "
            + " ,[Un_Load_FaceValue_2] = @Un_Load_FaceValue_2 "
            + " ,[Un_Load_Cassette_2]  = @Un_Load_Cassette_2 "
           + "  ,[Un_Load_FaceValue_3] = @Un_Load_FaceValue_3 "
           + "  ,[Un_Load_Cassette_3] = @Un_Load_Cassette_3 "
           + "  ,[Un_Load_FaceValue_4]  = @Un_Load_FaceValue_4 "
           + "  ,[Un_Load_Cassette_4]  = @Un_Load_Cassette_4 "

           + "  ,[UnloadedCounted] = @UnloadedCounted "

            + " ,[Dep_FaceValue_1] = @Dep_FaceValue_1 "
            + " ,[Deposits_Notes_Denom_1] = @Deposits_Notes_Denom_1 "
           + "  ,[Dep_FaceValue_2]  = @Dep_FaceValue_2 "
           + "  ,[Deposits_Notes_Denom_2]  = @Deposits_Notes_Denom_2 "
           + "  ,[Dep_FaceValue_3]  = @Dep_FaceValue_3 "
           + "  ,[Deposits_Notes_Denom_3]  = @Deposits_Notes_Denom_3 "
           + "  ,[Dep_FaceValue_4]  = @Dep_FaceValue_4 "
          + "   ,[Deposits_Notes_Denom_4]  = @Deposits_Notes_Denom_4 "

           + "  ,[Deposits] = @Deposits "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_1", Load_FaceValue_1);
                        cmd.Parameters.AddWithValue("@Load_Cassette_1", Load_Cassette_1);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_2", Load_FaceValue_2);
                        cmd.Parameters.AddWithValue("@Load_Cassette_2", Load_Cassette_2);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_3", Load_FaceValue_3);
                        cmd.Parameters.AddWithValue("@Load_Cassette_3", Load_Cassette_3);

                        cmd.Parameters.AddWithValue("@Load_FaceValue_4", Load_FaceValue_4);
                        cmd.Parameters.AddWithValue("@Load_Cassette_4", Load_Cassette_4);


                        cmd.Parameters.AddWithValue("@Cash_Loaded", Cash_Loaded);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_1", Un_Load_FaceValue_1);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_1", Un_Load_Cassette_1);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_2", Un_Load_FaceValue_2);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_2", Un_Load_Cassette_2);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_3", Un_Load_FaceValue_3);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_3", Un_Load_Cassette_3);

                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_4", Un_Load_FaceValue_4);
                        cmd.Parameters.AddWithValue("@Un_Load_Cassette_4", Un_Load_Cassette_4);

                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);

                        cmd.Parameters.AddWithValue("@Dep_FaceValue_1", Dep_FaceValue_1);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_1", Deposits_Notes_Denom_1);

                        cmd.Parameters.AddWithValue("@Dep_FaceValue_2", Dep_FaceValue_2);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_2", Deposits_Notes_Denom_2);

                        cmd.Parameters.AddWithValue("@Dep_FaceValue_3", Dep_FaceValue_3);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_3", Deposits_Notes_Denom_3);

                        cmd.Parameters.AddWithValue("@Dep_FaceValue_4", Dep_FaceValue_4);
                        cmd.Parameters.AddWithValue("@Deposits_Notes_Denom_4", Deposits_Notes_Denom_4);

                        cmd.Parameters.AddWithValue("@Deposits", Deposits);

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
        // DELETE SeqNo
        //
        public void DeleteCIT_G4S_Repl_EntriesRecord(int InSeqNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                WTableId = TableA;
            }
            if (InMode == 2)
            {
                WTableId = TableB;
            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTableId
                            + " WHERE SeqNo =  @SeqNo ", conn))
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


        public void InsertReportAtmRepl(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReportAtmRepl] "
                + " ([UserId] "
                    + " ,[CITId]"

                    + ",[OriginFileName] "
                    + ",[AtmNo] "
                    + ",[AtmName] "
                    + ",[ReplDateG4S]"
                    + ",[CreatedDate]"
                    + ",[IsDeposit] "
                    + ",[OpeningBalance]"
                    + ",[Dispensed]"
                    + ",[UnloadedMachine]"
                    + ",[UnloadedCounted]"
                    + ",[Cash_Loaded]"
                    + ",[Deposits]"
                    + ",[OverFound]"
                    + ",[ShortFound]"
                    + ",[RemarksG4S]"
                        + ",[PresentedErrors]"
                          //
                          + ",[OpeningBalance1]"
                    + ",[Dispensed1]"
                    + ",[UnloadedMachine1]"
                    + ",[Cash_Loaded1]"
                        + ",[Deposits1]"
                          //
                          + ",[PresentedErrors1]"
                    + ",[AmtCheckedForMatching1]"
                    + ",[OtherJournalErrors1]"
                    + ",[OrderToBeLoaded1]"
                        + ",[RemarksRRDM1]"
                + " ,[Operator]  ) "
                + " VALUES "
                + "(@UserId"
                + " ,@CITId"

                    + ",@OriginFileName "
                    + ",@AtmNo "
                    + ",@AtmName "
                    + ",@ReplDateG4S "
                    + ",@CreatedDate "

                    + ",@IsDeposit "
                    + ",@OpeningBalance "
                    + ",@Dispensed "
                    + ",@UnloadedMachine "
                    + ",@UnloadedCounted "

                    + ",@Cash_Loaded "
                    + ",@Deposits "

                    + ",@OverFound "
                    + ",@ShortFound "
                       + ",@RemarksG4S "
                        + ",@PresentedErrors "
                         //
                         + ",@OpeningBalance1 "

                    + ",@Dispensed1 "
                    + ",@UnloadedMachine1 "
                       + ",@Cash_Loaded1 "
                        + ",@Deposits1 "

                         //

                         + ",@PresentedErrors1 "

                    + ",@AmtCheckedForMatching1 "
                    + ",@OtherJournalErrors1 "
                       + ",@OrderToBeLoaded1 "
                        + ",@RemarksRRDM1 "
                //
                + " ,@Operator  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@CITId", CITId);

                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@ReplDateG4S", ReplDateG4S);

                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        cmd.Parameters.AddWithValue("@IsDeposit", IsDeposit);
                        cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                        cmd.Parameters.AddWithValue("@Dispensed", Dispensed);

                        cmd.Parameters.AddWithValue("@UnloadedMachine", UnloadedMachine);

                        cmd.Parameters.AddWithValue("@UnloadedCounted", UnloadedCounted);

                        cmd.Parameters.AddWithValue("@Cash_Loaded", Cash_Loaded);

                        cmd.Parameters.AddWithValue("@Deposits", Deposits);

                        cmd.Parameters.AddWithValue("@OverFound", OverFound);
                        cmd.Parameters.AddWithValue("@ShortFound", ShortFound);

                        cmd.Parameters.AddWithValue("@RemarksG4S", RemarksG4S);

                        cmd.Parameters.AddWithValue("@PresentedErrors", PresentedErrors);

                        // 
                        cmd.Parameters.AddWithValue("@OpeningBalance1", OpeningBalance1);

                        cmd.Parameters.AddWithValue("@Dispensed1", Dispensed1);
                        cmd.Parameters.AddWithValue("@UnloadedMachine1", UnloadedMachine1);

                        cmd.Parameters.AddWithValue("@Cash_Loaded1", Cash_Loaded1);

                        cmd.Parameters.AddWithValue("@Deposits1", Deposits1);


                        //
                        cmd.Parameters.AddWithValue("@PresentedErrors1", PresentedErrors1);

                        cmd.Parameters.AddWithValue("@AmtCheckedForMatching1", AmtCheckedForMatching1);
                        cmd.Parameters.AddWithValue("@OtherJournalErrors1", OtherJournalErrors1);

                        cmd.Parameters.AddWithValue("@OrderToBeLoaded1", OrderToBeLoaded1);

                        cmd.Parameters.AddWithValue("@RemarksRRDM1", RemarksRRDM1);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
        // DELETE 
        //
        public void DeleteAndUpdateToReverseEntriesbyLoadingExcelCycleNo(int InLoadingExcelCycleNo, string InCitId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            WTableId = TableA;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTableId
                            + " WHERE CITId = @CITId AND LoadingExcelCycleNo = @LoadingExcelCycleNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", InLoadingExcelCycleNo);
                        cmd.Parameters.AddWithValue("@CITId", InCitId);

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



            // UPDATE BANK's footer 

            WTableId = TableB;

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + WTableId
                              + " SET "
                              + " ProcessMode_Load = @ProcessMode_Load "
                              + ",LoadingExcelCycleNo = 0 "
                              + ",RemarksRRDM = '' "
                              + ",Mask = @Mask "
                              + " WHERE LoadingExcelCycleNo = @LoadingExcelCycleNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", InLoadingExcelCycleNo);
                        cmd.Parameters.AddWithValue("@CITId", InCitId);

                        cmd.Parameters.AddWithValue("@ProcessMode_Load", -2);

                        //cmd.Parameters.AddWithValue("@ProcessedAtReplCycleNo", ProcessedAtReplCycleNo);

                        cmd.Parameters.AddWithValue("@Mask", "");

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
        // DELETE 
        //
        public void DeleteRecordsByRMCycle(int InLoadedAtRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            WTableId = TableB;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + WTableId
                            + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtLoadedAtRMCycle", InLoadedAtRMCycle);

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
        // DELETE DeleteReportAtmRepl
        //
        public void DeleteReportAtmRepl(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReportAtmRepl] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

        public void CreateActionsAndTXNSforATM(string InOperator, string InSignedId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";


            try
            {
                // READ all CIT Entries Bank this Cycle into a table

                // Read each individual entry of the table and create action occurances for 
                // a) Unload Debits 
                // b) Unlaod Credits
                // c) Load Mmachine. 

                int WMode = 2; // Banks entries 

                string WSelectionCriteria = "WHERE LoadedAtRMCycle=" + InRMCycle;

                ReadCIT_G4S_Repl_EntriesToFillDataTable(Operator, InSignedId, WSelectionCriteria, WMode);


                // SAVE TABLE GL_1 ...Gt.DataTableAllFields; 

                // GET THE TOTALS OUT OF THE TABLE
                string SelectionCriteria = "";
                int WUniqueRecordId;
                bool WIsMatchingDone;
                bool WMatched;
                int WTransType;
                decimal WTransAmount;
                string WMatchMask;
                string WActionType;
                string WResponseCode;
                int WMetaExceptionId;
                bool WNotInJournal;

                decimal TotalDrAfter_1 = 0;
                decimal TotalCrAfter_1 = 0;
                //TotalDrExceptionsAfter = 0; // actions has been take to exceptions 
                // Therefore are included in GL Account
                //TotalCrExceptionsAfter = 0;

                int I = 0;

                while (I <= (DataTableG4SEntries.Rows.Count - 1))
                {

                    // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                    WUniqueRecordId = (int)DataTableG4SEntries.Rows[I]["UniqueRecordId"];
                    WIsMatchingDone = (bool)DataTableG4SEntries.Rows[I]["IsMatchingDone"];
                    WMatched = (bool)DataTableG4SEntries.Rows[I]["Matched"];
                    WTransType = (int)DataTableG4SEntries.Rows[I]["TransType"];
                    WTransAmount = (decimal)DataTableG4SEntries.Rows[I]["TransAmount"];

                    // For Each ATM SesNo create the needed txns. 


                    I = I + 1;
                }

            }
            catch (Exception ex)
            {


                CatchDetails(ex);

            }
        }



        int TotalInDiff = 0;

        string WJobCategory = "ATMs";
        int WReconcCycleNo;
        string Message;

        int WTransType;
        decimal WTransAmount;
        string WMatchingCateg;

        // decimal WUnloadedMachine;

        decimal WDiffGL;

        DataTable GL_1 = new DataTable();

        int WSeqNo = 0;

        bool ShowMessage = true;


        string WReconCategGroup;
        string WAtmNo = "";
        int WSesNo = 0;

        public void Create_Compined_Records_AndTXNS_For_Each_ATM_SM_Loaded(string InOperator, string InSignedId, int InRMCycle, int InExcelCycle)
        {
            // Update RRDM Replenishmet record
            //



            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(InOperator, WJobCategory);

            // Read all Outstanding Matched Entries from G4S file
            // Make a loop and update 

            // Make Selection Of Validated Entries 
            int TempMode = 2; // Bank entries 
            string SelectionCriteria = " WHERE LoadedAtRMCycle = " + InRMCycle;

            ReadCIT_G4S_Repl_EntriesToFillDataTable(InOperator, InSignedId, SelectionCriteria, TempMode);

            if (DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records to update");
                return;
            }

            int I = 0;

            while (I <= (DataTableG4SEntries.Rows.Count - 1))
            {
                // FOR EACH REPLENISHMENT 
                // CREATE TWO LINES one BASED ON IST AND SECOND BASED ON JOURNALS TXNS VS COREBANKING CLOSING 
                // FIRST ONE IS BASED ON IST TXNS during replenishement Cycle 
                // SECOND ONE IS BASED ON TRANSACTIONS BASED ON JOURNAL during replenishement Cycle 
                // FOR BOTH LINES WE CALCULATE THE REMAIN BASED ON OPENNING BALANCE AND RESPECTIVE TRANSACTIONS
                // THER GL BALANCE = REMAINS FROM CORE ... IS TAKEN FROM CORE TRANSACTIONS ( OPENING - WITHDRAWALS + DEPOSITS ). 
                // IF REMAINS LINE IS DIFFERENT THAN CORE THEN THERE IS EXCESS OR SHORTAGE 
                // 
                // TXNS ARE TAKEN BASED ON REPLENISHMENT DATES ( START AND END). 

                // GET ALL fields

                //    RecordFound = true;
                WSeqNo = (int)DataTableG4SEntries.Rows[I]["SeqNo"];
                WAtmNo = (string)DataTableG4SEntries.Rows[I]["AtmNo"];
                WSesNo = (int)DataTableG4SEntries.Rows[I]["ReplCycleNo"];
                decimal WUnloadedMachine = (decimal)DataTableG4SEntries.Rows[I]["UnloadedMachine"];
                decimal WCash_Loaded_Machine = (decimal)DataTableG4SEntries.Rows[I]["Cash_Loaded"];
                decimal WUnloadedMachineDep = (decimal)DataTableG4SEntries.Rows[I]["Deposits"];

                DateTime ReplDate = (DateTime)DataTableG4SEntries.Rows[I]["ReplDateG4S"];

                // Insert Replenishment record the way AUDI wants them. 
                Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);

                Sc.SesDtTimeStart = Ta.SesDtTimeStart;
                Sc.AtmNo = WAtmNo;
                Sc.SesNo = WSesNo;
                Sc.SesDtTimeStart = Ta.SesDtTimeStart;
                Sc.SesDtTimeEnd = Ta.SesDtTimeEnd;
                Sc.DateOpened = DateTime.Now;
                Sc.ProcessMode = 0; // Ready for replenishment
                Sc.RMCycle = WReconcCycleNo;

                Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

                Sc.OpenBal = Na.Balances1.OpenBal;
                // READ TXNS FROM IST TO FIND TOTALS
                Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_IST(WAtmNo,
                    Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);
                // Fill in table 
                GL_1 = Gt.DataTableAllFields;

                // FOR CORE 
                decimal TotalDrAfter_4 = 0;
                decimal TotalCrAfter_4 = 0;
                decimal TotalDrEMR203 = 0;
                decimal TotalCrEMR203 = 0;

                int K = 0;

                while (K <= (GL_1.Rows.Count - 1))
                {

                    // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                    WMatchingCateg = (string)GL_1.Rows[K]["MatchingCateg"];
                    WTransType = (int)GL_1.Rows[K]["TransType"];
                    WTransAmount = (decimal)GL_1.Rows[K]["TransAmt"];

                    //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                    //{
                    // it is in journal 
                    if (WTransType == 11)
                    {
                        TotalDrAfter_4 = TotalDrAfter_4 + WTransAmount;
                        if (WMatchingCateg == "EMR203")
                        {
                            TotalDrEMR203 = TotalDrEMR203 + WTransAmount;
                        }
                    }
                    if (WTransType == 23)
                    {
                        TotalCrAfter_4 = TotalCrAfter_4 + WTransAmount;
                        if (WMatchingCateg == "EMR203")
                        {
                            TotalCrEMR203 = TotalCrEMR203 + WTransAmount;
                        }
                    }
                    //}

                    K = K + 1;
                }

                decimal ISTCoreRemain = Na.Balances1.OpenBal
                              - TotalDrAfter_4
                              + TotalCrAfter_4
                               ;
                // ASSIGN IST VALUES
                Sc.WithDrawls = TotalDrAfter_4;
                Sc.Deposits = TotalCrAfter_4;
                Sc.Remaining = ISTCoreRemain;

                // DO COREBANKING
                // COREBANKING
                Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_COREBANKING(WAtmNo,
                    Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

                GL_1 = Gt.DataTableAllFields;

                // CORE 
                decimal TotalDrAfter_5 = 0;
                decimal TotalCrAfter_5 = 0;

                K = 0;

                while (K <= (GL_1.Rows.Count - 1))
                {

                    // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                    WTransType = (int)GL_1.Rows[K]["TransType"];
                    WTransAmount = (decimal)GL_1.Rows[K]["TransAmt"];


                    if (WTransType == 11)
                    {
                        TotalDrAfter_5 = TotalDrAfter_5 + WTransAmount;
                    }
                    if (WTransType == 23)
                    {
                        TotalCrAfter_5 = TotalCrAfter_5 + WTransAmount;
                    }


                    K = K + 1;
                }

                decimal CoreRemain2 = Na.Balances1.OpenBal
                              - TotalDrAfter_5
                              + TotalCrAfter_5
                               ;
                // CORRECTION FOR MISSING IN COREBANKING
                Sc.GL_BalanceFromCore = (CoreRemain2 - TotalDrEMR203 + TotalCrEMR203);

                // FIRST LINE
                Sc.Remark = "No Difference";
                if (Sc.Remaining > Sc.GL_BalanceFromCore)
                {
                    Sc.Excess = Sc.Remaining - Sc.GL_BalanceFromCore;
                    Sc.Remark = "There is excess";
                }
                else
                {
                    Sc.Excess = 0;
                }

                if (Sc.Remaining < Sc.GL_BalanceFromCore)
                {
                    Sc.Shortage = Sc.GL_BalanceFromCore - Sc.Remaining;
                    Sc.Remark = "There is Shortage";
                }
                else
                {
                    Sc.Shortage = 0;
                }


                Cc.ReadCapturedCardsNoWithinDatesRange(WAtmNo, Ta.SesDtTimeStart, Ta.SesDtTimeEnd);
                Sc.CapturedCards = Cc.CaptureCardsNo;
                //
                // DO FOR E-Journals 
                //
                Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_Mpa(WAtmNo,
                      Ta.SesDtTimeStart, Ta.SesDtTimeEnd, 2);

                GL_1 = Gt.DataTableAllFields;

                // FOR CORE 
                decimal TotalDrAfter_3 = 0;
                decimal TotalCrAfter_3 = 0;

                K = 0;

                while (K <= (GL_1.Rows.Count - 1))
                {

                    // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                    WMatchingCateg = (string)GL_1.Rows[K]["MatchingCateg"];
                    WTransType = (int)GL_1.Rows[K]["TransType"];
                    WTransAmount = (decimal)GL_1.Rows[K]["TransAmount"];

                    //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                    //{
                    // it is in journal 
                    if (WTransType == 11)
                    {
                        TotalDrAfter_3 = TotalDrAfter_3 + WTransAmount;

                    }
                    if (WTransType == 23)
                    {
                        TotalCrAfter_3 = TotalCrAfter_3 + WTransAmount;

                    }
                    //}

                    K = K + 1;
                }

                decimal JournalRemain = Na.Balances1.OpenBal
                              - TotalDrAfter_3
                              + TotalCrAfter_3
                               ;
                // SECOND LINE BASED ON JOURNAL 
                Sc.OpenBal1 = Sc.OpenBal;
                Sc.WithDrawls1 = TotalDrAfter_3;
                Sc.Deposits1 = TotalCrAfter_3;
                Sc.Remaining1 = JournalRemain;

                Sc.GL_BalanceFromCore1 = Sc.GL_BalanceFromCore; // Take it from the previous
                // SECOND LINE 
                Sc.Remark1 = "No Difference";
                if (Sc.Remaining1 > Sc.GL_BalanceFromCore1)
                {
                    Sc.Excess1 = Sc.Remaining1 - Sc.GL_BalanceFromCore1;
                    if (Sc.Excess1 == Sc.Excess)
                    {
                        Sc.Remark1 = "Excess Equal";
                    }
                    else
                    {
                        Sc.Remark1 = "Excess NOT Equal";
                    }

                }
                else
                {
                    Sc.Excess1 = 0;
                }

                if (Sc.Remaining1 < Sc.GL_BalanceFromCore1)
                {
                    Sc.Shortage1 = Sc.GL_BalanceFromCore1 - Sc.Remaining1;

                    if (Sc.Shortage1 == Sc.Shortage)
                    {
                        Sc.Remark1 = "Shortage Equal";
                    }
                    else
                    {
                        Sc.Remark1 = "Shortage NOT Equal";
                    }

                }
                else
                {
                    Sc.Shortage1 = 0;
                }

                Sc.CapturedCards1 = Sc.CapturedCards; // Take it from previous 

                Sc.InsertSessionsDataCombined();

                // If Txns to be created upon loading of Journals SM Mode

                string ParId = "945";
                string OccurId = "3"; // 
                Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
                if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                {
                    WDiffGL = 0;

                    if (WUnloadedMachine > 0)
                    {
                        CreateActions_Occurances_WithDrawels(InOperator, InSignedId, WAtmNo, WSesNo, WUnloadedMachine, WDiffGL);
                    }

                    //  WUnloadedMachineDep = G4.Deposits;
                    //  WDiffGL = WDepositsCounted - WUnloadedMachineDep;
                    WDiffGL = 0;
                    if (WUnloadedMachineDep > 0)
                    {
                        CreateActions_Occurances_Dep(InOperator, InSignedId, WAtmNo, WSesNo, WUnloadedMachine, WDiffGL);
                    }

                    //WCash_Loaded_Machine = G4.Cash_Loaded;

                    // WDiffGL = WCash_Loaded_CIT - WCash_Loaded_Machine;
                    WDiffGL = 0;

                    if (WCash_Loaded_Machine > 0)
                    {
                        CreateActions_Occurances_Load(InOperator, InSignedId, WAtmNo, WSesNo, WCash_Loaded_Machine, WDiffGL);
                    }

                    // UPDATE 
                    int WLoadingCycle = InExcelCycle; // This is the excel Cycle 

                    Aoc.UpdateOccurancesLoadingExcelCycleNo(WAtmNo, WSesNo, "", WLoadingCycle);

                }


                I++; // Read Next entry of the table 

            }

            // View the table

            string Selection = "";
            int WMode = 1;
            Sc.ReadSessionsDataCombined_Fill_the_Two_linesTable("", "", ""
                                                                  , NullPastDate, NullPastDate, "", WMode);

            GL_1 = Sc.TableSessionsDataCombined_2;
        }
        // FIND IST TOTALS 
        public decimal IstRemains;
        public decimal CoreRemains; 
        public decimal JournalRemains; 
        
        public void FindRemainsTotals(string WAtmNo, int WSesNo , DateTime InStartDateTime, DateTime InFinishDateTime)
        {
            // Find Openning Balaonce 
            Na.ReadSessionsNotesAndValues(WAtmNo, WSesNo, 2);

            Sc.OpenBal = Na.Balances1.OpenBal;
            // READ TXNS FROM IST TO FIND TOTALS
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_IST(WAtmNo,
                InStartDateTime, InFinishDateTime, 2);
            // Fill in table 
            GL_1 = Gt.DataTableAllFields;

            // FOR CORE 
            decimal TotalDrAfter_4 = 0;
            decimal TotalCrAfter_4 = 0;
            decimal TotalDrEMR203 = 0;
            decimal TotalCrEMR203 = 0;

            int K = 0;

            while (K <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WMatchingCateg = (string)GL_1.Rows[K]["MatchingCateg"];
                WTransType = (int)GL_1.Rows[K]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[K]["TransAmt"];

                //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                //{
                // it is in journal 
                if (WTransType == 11)
                {
                    TotalDrAfter_4 = TotalDrAfter_4 + WTransAmount;
                    if (WMatchingCateg == "EMR203")
                    {
                        TotalDrEMR203 = TotalDrEMR203 + WTransAmount;
                    }
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_4 = TotalCrAfter_4 + WTransAmount;
                    if (WMatchingCateg == "EMR203")
                    {
                        TotalCrEMR203 = TotalCrEMR203 + WTransAmount;
                    }
                }
                //}

                K = K + 1;
            }

            decimal ISTCoreRemain = Na.Balances1.OpenBal
                          - TotalDrAfter_4
                          + TotalCrAfter_4
                           ;
            // ASSIGN IST VALUES
            Sc.WithDrawls = TotalDrAfter_4;
            Sc.Deposits = TotalCrAfter_4;
            IstRemains = Sc.Remaining = ISTCoreRemain;

            // DO COREBANKING
            // COREBANKING
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_COREBANKING(WAtmNo,
                InStartDateTime, InFinishDateTime, 2);


            GL_1 = Gt.DataTableAllFields;

            // CORE 
            decimal TotalDrAfter_5 = 0;
            decimal TotalCrAfter_5 = 0;

            K = 0;

            while (K <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WTransType = (int)GL_1.Rows[K]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[K]["TransAmt"];


                if (WTransType == 11)
                {
                    TotalDrAfter_5 = TotalDrAfter_5 + WTransAmount;
                }
                if (WTransType == 23)
                {
                    TotalCrAfter_5 = TotalCrAfter_5 + WTransAmount;
                }


                K = K + 1;
            }

            decimal CoreRemain2 = Na.Balances1.OpenBal
                          - TotalDrAfter_5
                          + TotalCrAfter_5
                           ;
            // CORRECTION FOR MISSING IN COREBANKING
            CoreRemains = Sc.GL_BalanceFromCore = (CoreRemain2 - TotalDrEMR203 + TotalCrEMR203);
  
           // FIRST LINE
            Sc.Remark = "No Difference";
            if (Sc.Remaining > Sc.GL_BalanceFromCore)
            {
                Sc.Excess = Sc.Remaining - Sc.GL_BalanceFromCore;
                Sc.Remark = "There is excess";
            }
            else
            {
                Sc.Excess = 0;
            }

            if (Sc.Remaining < Sc.GL_BalanceFromCore)
            {
                Sc.Shortage = Sc.GL_BalanceFromCore - Sc.Remaining;
                Sc.Remark = "There is Shortage";
            }
            else
            {
                Sc.Shortage = 0;
            }


            //Cc.ReadCapturedCardsNoWithinDatesRange(WAtmNo, InStartDateTime, InFinishDateTime);

            //Sc.CapturedCards = Cc.CaptureCardsNo;
            //
            // DO FOR E-Journals 
            //
            Gt.ReadTrans_Table_FromPreviousReplenishmentTo_Current_Mpa(WAtmNo
                  , InStartDateTime, InFinishDateTime, 2);

            GL_1 = Gt.DataTableAllFields;

            // FOR CORE 
            decimal TotalDrAfter_3 = 0;
            decimal TotalCrAfter_3 = 0;

            K = 0;

            while (K <= (GL_1.Rows.Count - 1))
            {

                // int WSeqNo = (int)Gt.DataTableAllFields.Rows[I]["SeqNo"];
                WMatchingCateg = (string)GL_1.Rows[K]["MatchingCateg"];
                WTransType = (int)GL_1.Rows[K]["TransType"];
                WTransAmount = (decimal)GL_1.Rows[K]["TransAmount"];

                //if (WIsMatchingDone == true & WMatched == true & WNotInJournal == false)
                //{
                // it is in journal 
                if (WTransType == 11)
                {
                    TotalDrAfter_3 = TotalDrAfter_3 + WTransAmount;

                }
                if (WTransType == 23)
                {
                    TotalCrAfter_3 = TotalCrAfter_3 + WTransAmount;

                }
                //}

                K = K + 1;
            }

            decimal CalculatedJournalRemain = Na.Balances1.OpenBal
                          - TotalDrAfter_3
                          + TotalCrAfter_3
                           ;
            // SECOND LINE BASED ON JOURNAL 
            Sc.OpenBal1 = Sc.OpenBal;
            Sc.WithDrawls1 = TotalDrAfter_3;
            Sc.Deposits1 = TotalCrAfter_3;

            JournalRemains = Sc.Remaining1 = CalculatedJournalRemain;

        }

        //RRDMAtmsClass Ac = new RRDMAtmsClass();
        // Create Action Occurances WithDrawls 
        DataTable TEMPTableFromAction;
        string WUniqueRecordIdOrigin = "Replenishment";
        public void CreateActions_Occurances_WithDrawels(string InOperator, string InSignedId, string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 
            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;
            string WOperator = InOperator;
            string WSignedId = InSignedId;

            //int WFunction = 2;
            //Na.ReadSessionsNotesAndValues(WWAtmNo, WWSesNo, WFunction); // Read Values from NOTES

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }
            //
            if (WOperator == "AUDBEGCA")
                HybridRepl = false;

            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            string WMaker_ReasonOfAction;

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";

            if (HybridRepl == false)
            {
                // FIRST DOUBLE ENTRY 
                WActionId = "25"; // 25_DEBIT_ CIT Account/CR_AtmCash (UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment"

                WMaker_ReasonOfAction = "Un Load From ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt,
                                                      WWAtmNo, WWSesNo, WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }


            //
            // CLEAR PREVIOUS ACTIONS FOR THIS REPLENISHMENT
            //
            if (HybridRepl == false)
            {
                WActionId = "29";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "39";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "30";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            // Delete create Dispute Shortage

            if (HybridRepl == false)
            {
                WActionId = "87";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            if (HybridRepl == true)
            {
                WActionId = "77";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }

            //
            WActionId = "88";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            if (InDiffGL == 0)
            {
                // do nothing
            }

            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //         + "Will be moved to the Branch excess account "
                //    );
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "30"; //30_CREDIT Branch or CIT Excess / DR_AtmCash(UNLOAD)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad From ATM-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + InDiffGL.ToString("#,##0.00") + Environment.NewLine
                //        + "Will be moved to the Branch shortage account "
                //         );
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "29"; // 29_DEBIT_CIT Shortages/CR_AtmCash(UNLOAD)
                }
                if (HybridRepl == true)
                {
                    WActionId = "39"; // 29_DEBIT_Branch Shortages/CR_AtmCash(UNLOAD)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad From ATM-Shortage";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo,
                                                      WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;


            }

            // Handle Any Balance In Action Occurances 
            string WSelectionCriteria = "WHERE AtmNo ='" + WWAtmNo
                       + "' AND ReplCycle =" + WWSesNo
                       + " AND ( (Maker ='" + WSignedId + "' AND Stage<>'03') OR Stage = '03') ";

            Aoc.ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            if (Aoc.Current_DisputeShortage != 0)
            {
                //MessageBox.Show("Also note that Dispute Shortage will be handle here." + Environment.NewLine
                //         + "The Dispute Shortage is :" + Aoc.Current_DisputeShortage.ToString("#,##0.00") + Environment.NewLine
                //         + "Look at the resulted transactions");


                decimal CIT_Shortage = 0;
                decimal Shortage = 0;
                decimal Dispute_Shortage = -(Aoc.Current_DisputeShortage);
                decimal WExcess = Aoc.Excess;

                if (HybridRepl == false)
                {
                    CIT_Shortage = -(Aoc.CIT_Shortage);
                }
                if (HybridRepl == true)
                {
                    Shortage = -(Aoc.CIT_Shortage);
                }


                if (WExcess > 0)
                {
                    if (WExcess >= Dispute_Shortage)
                    {
                        // A
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = Dispute_Shortage;
                        WMaker_ReasonOfAction = "Settle Dispute Shortage";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                    }
                    else
                    {   // A
                        // Use all amount of Excess
                        WActionId = "88"; //88_CREDIT Dispute Shortage/DEBIT Branch Excess
                                          // 
                        WUniqueRecordId = WWSesNo; // SesNo 
                        WCcy = "EGP";
                        DoubleEntryAmt = WExcess; // Use all amount iin Excess
                        WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 1";
                        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                              WActionId, WUniqueRecordIdOrigin,
                                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                              , WMaker_ReasonOfAction, "Replenishment");

                        TEMPTableFromAction = Aoc.TxnsTableFromAction;

                        // The rest you take it from Shortage

                        decimal TempDiff1 = Dispute_Shortage - WExcess;
                        if (TempDiff1 > 0)
                        {
                            // Diff1 goes to Shortage
                            // B
                            if (HybridRepl == false)
                            {
                                WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            if (HybridRepl == true)
                            {
                                WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                            }

                            // 
                            WUniqueRecordId = WWSesNo; // SesNo 
                            WCcy = "EGP";
                            DoubleEntryAmt = TempDiff1;
                            WMaker_ReasonOfAction = "Settle Dispute Shortage Through Excess and Shortage 2";
                            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                                  WActionId, WUniqueRecordIdOrigin,
                                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                                  , WMaker_ReasonOfAction, "Replenishment");

                            TEMPTableFromAction = Aoc.TxnsTableFromAction;
                        }

                    }
                }

                if ((CIT_Shortage > 0 || (WExcess == 0 & CIT_Shortage == 0) & HybridRepl == false)
                    || (Shortage > 0 || (WExcess == 0 & Shortage == 0) & HybridRepl == true)
                    )
                {
                    // 
                    if (HybridRepl == false)
                    {
                        WActionId = "87"; //87_CREDIT Dispute Shortage/DEBIT CIT Branch Shortage
                    }
                    if (HybridRepl == true)
                    {
                        WActionId = "77"; //77_CREDIT Dispute Shortage/DEBIT Branch Shortage
                    }
                    // 
                    WUniqueRecordId = WWSesNo; // SesNo 
                    WCcy = "EGP";
                    DoubleEntryAmt = Dispute_Shortage;
                    WMaker_ReasonOfAction = "Settle Dispute Shortage through Shortage";
                    Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                          WActionId, WUniqueRecordIdOrigin,
                                                          WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                          , WMaker_ReasonOfAction, "Replenishment");

                    TEMPTableFromAction = Aoc.TxnsTableFromAction;
                }

            }

        }

        // CREATE ACTION OCCURANCES DEPOSITS
        public void CreateActions_Occurances_Dep(string InOperator, string InSignedId, string InAtmNo, int InSesNo, decimal InUnloadedCounted, decimal InDiffGL)
        {
            // Create 
            // Unload transaction for CIT and Bank
            // Create discrepancies 

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;
            string WOperator = InOperator;
            string WSignedId = InSignedId;

            RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
            string WActionId;
            // string WUniqueRecordIdOrigin ;
            int WUniqueRecordId;
            string WCcy;
            decimal DoubleEntryAmt;
            decimal CassetteAmt;
            decimal RetractedAmt;
            //



            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            DoubleEntryAmt = InUnloadedCounted;
            WUniqueRecordId = WWSesNo; // SesNo 
            WCcy = "EGP";
            string WMaker_ReasonOfAction;

            //DoubleEntryAmt = CassetteAmt + RetractedAmt;
            // FIRST DOUBLE ENTRY 
            if (HybridRepl == false)
            {
                WActionId = "26"; // 26_CREDIT CIT Account/DR_AtmCash (DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";

                //DoubleEntryAmt = Na.Balances1.CountedBal;
                WMaker_ReasonOfAction = "UnLoad Deposits";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            else
            {
                WActionId = "26";
                Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            }


            WActionId = "27";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);

            WActionId = "37";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);
            //
            WActionId = "28";
            Aoc.DeleteActionsOccurancesUniqueKeyAndActionID(WUniqueRecordIdOrigin, WUniqueRecordId, WActionId);


            if (InDiffGL > 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT excess account ");
                // Move to Excess 
                // SECOND DOUBLE ENTRY 
                WActionId = "28"; //28_CREDIT Branch Excess/DR_AtmCash(DEPOSITS)
                                  // WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InDiffGL;
                WMaker_ReasonOfAction = "UnLoad Deposits-Excess";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");

                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }
            if (InDiffGL < 0)
            {
                //MessageBox.Show("The amount of Difference:.." + DiffGL.ToString("#0.00") + Environment.NewLine
                //        + "Will be moved to the CIT shortage account ");
                // Move to Shortage
                // SECOND DOUBLE ENTRY 
                if (HybridRepl == false)
                {
                    WActionId = "27"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }
                if (HybridRepl == true)
                {
                    WActionId = "37"; // 27_DEBIT_Branch Shortages/CR_AtmCash(DEPOSITS)
                }

                //WUniqueRecordIdOrigin = "Replenishment";
                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = -InDiffGL; // Turn it to positive 
                WMaker_ReasonOfAction = "UnLoad Deposits-Shortages";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");
                TEMPTableFromAction = Aoc.TxnsTableFromAction;
            }

            //WTotalForCust = SM.TotalForCust;
            //WTotalCommision = SM.TotalCommision;
            //if (HybridRepl == false)
            //{
            //    if (WTotalForCust > 0)
            //    {
            //        // CREATE TRANSACTIONS FOR FOREX 
            //        DoubleEntryAmt = WTotalForCust;
            //        // FIRST DOUBLE ENTRY 
            //        WActionId = "33"; // 33_CREDIT_FOREX_INTERMEDIARY/DR_ATM CASH
            //                          // WUniqueRecordIdOrigin = "Replenishment";
            //        WUniqueRecordId = WSesNo; // SesNo 
            //        WCcy = "EGP";
            //        //DoubleEntryAmt = Na.Balances1.CountedBal;
            //        WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                              WActionId, WUniqueRecordIdOrigin,
            //                                              WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                              , WMaker_ReasonOfAction, "Replenishment");


            //        TEMPTableFromAction = Aoc.TxnsTableFromAction;
            //        // FOREX
            //        // FOREX 
            //        // FOREX
            //        if (WTotalCommision > 0)
            //        {
            //            // CREATE TRANSACTIONS FOR FOREX Commision 
            //            DoubleEntryAmt = WTotalCommision;
            //            // FIRST DOUBLE ENTRY 
            //            WActionId = "34"; // 34_CREDIT_FOREX_INTERMEDIARY/DR_Commision
            //                              // WUniqueRecordIdOrigin = "Replenishment";
            //            WUniqueRecordId = WSesNo; // SesNo 
            //            WCcy = "EGP";
            //            //DoubleEntryAmt = Na.Balances1.CountedBal;
            //            WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //            Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //                                                  WActionId, WUniqueRecordIdOrigin,
            //                                                  WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //                                                  , WMaker_ReasonOfAction, "Replenishment");


            //            TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //        }

            //        //// CREATE TRANSACTIONS FOR FOREX CIT
            //        //DoubleEntryAmt = WTotalForCust + WTotalCommision;
            //        //// FIRST DOUBLE ENTRY 
            //        //WActionId = "35"; // 35_CREDIT_CIT ACCOUNT GL/DR_Forex_Intermidiary(DEPOSITS)
            //        //                  // WUniqueRecordIdOrigin = "Replenishment";
            //        //WUniqueRecordId = WSesNo; // SesNo 
            //        //WCcy = "EGP";
            //        ////DoubleEntryAmt = Na.Balances1.CountedBal;
            //        //WMaker_ReasonOfAction = "UnLoad Deposits Forex";
            //        //Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
            //        //                                      WActionId, WUniqueRecordIdOrigin,
            //        //                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WAtmNo, WSesNo
            //        //                                      , WMaker_ReasonOfAction, "Replenishment");


            //        //TEMPTableFromAction = Aoc.TxnsTableFromAction;

            //    }
            //}

        }


        public void CreateActions_Occurances_Load(string InOperator, string InSignedId, string InAtmNo, int InSesNo, decimal InCashInAmount, decimal InDiffGL)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            bool HybridRepl = false;

            string WWAtmNo = InAtmNo;
            int WWSesNo = InSesNo;
            string WOperator = InOperator;
            string WSignedId = InSignedId;

            Ac.ReadAtm(WWAtmNo);
            if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            {
                HybridRepl = true;
                // In Such case do apply action for unload
                // Branch already has done this 
                // Also use actions for shortage in relation to branch and not to the CIT
            }
            else
            {
                HybridRepl = false;
            }

            // Make transaction if CIT
            if (HybridRepl == false)
            {
                // Create 
                // load transaction for CIT and Bank

                RRDMActions_Occurances Aoc = new RRDMActions_Occurances();
                string WActionId;
                // string WUniqueRecordIdOrigin ;
                int WUniqueRecordId;
                string WCcy;
                decimal DoubleEntryAmt;

                // FIRST DOUBLE ENTRY 
                WActionId = "24"; // 24_CREDIT CIT Account/DR_AtmCash (LOAD)

                WUniqueRecordId = WWSesNo; // SesNo 
                WCcy = "EGP";
                DoubleEntryAmt = InCashInAmount;
                string WMaker_ReasonOfAction = "Load ATM";
                Aoc.CreateActionsTxnsPerActionId(WOperator, WSignedId,
                                                      WActionId, WUniqueRecordIdOrigin,
                                                      WUniqueRecordId, WCcy, DoubleEntryAmt, WWAtmNo, WWSesNo
                                                      , WMaker_ReasonOfAction, "Replenishment");


                TEMPTableFromAction = Aoc.TxnsTableFromAction;

            }

        }

    }
}

using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMReconcCategATMsAtRMCycles : Logger
    {
        public RRDMReconcCategATMsAtRMCycles() : base() { }

        public int SeqNo;
        public string ReconcCategoryId;
        //public string MatchingCategoryId;
        //public int MinMaxTrace;

        public int AtmGroup;
        public string OwnerUserID;

        public int RMCycle;

        public string AtmNo;

        public DateTime CreatedDtTm;
        public string Currency;
        public decimal OpeningBalance;

        public decimal JournalAmt;

        public decimal MatchedAmtAtMatching;
        public decimal MatchedAmtAtDefault;
        public decimal MatchedAmtAtWorkFlow;
        public int NumberOfMatchedRecs;

        public decimal UnMatchedAmt;
        public int NumberOfUnMatchedRecs;

        public bool OpenRecord;
        public string Operator;
     
        public int Number_Presenter;
        public decimal Amount_Presenter;

        // Define the data table 

        // Define the data table 
        public DataTable TableRMATMsCycles = new DataTable();
        public int TotalSelected;

        // Totals
        public int TotalAtmsInGroupMatched;
        public int TotalAtmsInGroupUnMatched;
        public int TotalNumberOfMatchedRecs;
        public int TotalNumberOfUnMatchedRecs;

        public decimal TotalJournalAmt;

        public decimal TotalMatchedAmtAtMatching;
        public decimal TotalUnMatchedAmt;

        // END TOTALS

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        // READER FIELDS 
        private void ReadReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            ReconcCategoryId = (string)rdr["ReconcCategoryId"];

            //MatchingCategoryId = (string)rdr["MatchingCategoryId"];

            //MinMaxTrace = (int)rdr["MinMaxTrace"];

            AtmGroup = (int)rdr["AtmGroup"];
            OwnerUserID = (string)rdr["OwnerUserID"];

            RMCycle = (int)rdr["RMCycle"];
            AtmNo = (string)rdr["AtmNo"];

            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];

            Currency = (string)rdr["Currency"];

            OpeningBalance = (decimal)rdr["OpeningBalance"];
            JournalAmt = (decimal)rdr["JournalAmt"];
            MatchedAmtAtMatching = (decimal)rdr["MatchedAmtAtMatching"];
            MatchedAmtAtDefault = (decimal)rdr["MatchedAmtAtDefault"];
            MatchedAmtAtWorkFlow = (decimal)rdr["MatchedAmtAtWorkFlow"];

            NumberOfMatchedRecs = (int)rdr["NumberOfMatchedRecs"];

            UnMatchedAmt = (decimal)rdr["UnMatchedAmt"];
            NumberOfUnMatchedRecs = (int)rdr["NumberOfUnMatchedRecs"];

            OpenRecord = (bool)rdr["OpenRecord"];
            Operator = (string)rdr["Operator"];

            Number_Presenter = (int)rdr["Number_Presenter"];
            Amount_Presenter = (decimal)rdr["Amount_Presenter"];

    }
        //
        // Methods 
        // READ RM ATM Cycles 
        // READ SPECIFIC
        //
        // (RAtms.OpeningBalance + RAtms.MatchedAmtAtMatching + RAtms.MatchedAmtAtDefault)
        public decimal TotOpeningBalance;
        public decimal TotMatchedAmtAtMatching;
        public decimal TotMatchedAmtAtDefault;
        public decimal TotJournalAmt;

        public void ReadReconcCategoriesATMsRMCycleSpecificAtmforReconcCatTOTALS(string InOperator,
                     string InReconcCategoryId, int InRMCycle, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotOpeningBalance = 0;
            TotMatchedAmtAtMatching = 0;
            TotMatchedAmtAtDefault = 0;
            TotJournalAmt = 0;

            SqlString = "SELECT *"
                        + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                        + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle AND AtmNo = @AtmNo"
                        + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadReaderFields(rdr);

                            TotOpeningBalance = TotOpeningBalance + OpeningBalance;
                            TotMatchedAmtAtMatching = TotMatchedAmtAtMatching + MatchedAmtAtMatching;
                            TotMatchedAmtAtDefault = TotMatchedAmtAtDefault + MatchedAmtAtDefault;
                            TotJournalAmt = TotJournalAmt + JournalAmt;

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

        public void ReadReconcCategoriesATMsRMCycleSpecific(string InOperator,
                    string InReconcCategoryId, int InRMCycle, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                        + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                        + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle AND AtmNo = @AtmNo"
                        + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadReaderFields(rdr);

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
        // Read by SeqNo 
        public void ReadReconcCategoriesATMsRMCycleBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
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

                            ReadReaderFields(rdr);

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
        // READ RM ATM Cycles and FIND Totals
        // READ SPECIFIC
        //

        public decimal AllAtmsOpeningBalance; // This zero 
        public decimal AllAtmsJournalAmt; // 
        public decimal AllAtmsMatchedAmtAtMatching; // 
        public decimal AllAtmsMatchedAmtAtDefault;
        public decimal AllAtmsMatchedAmtAtWorkFlow; // 
        public void ReadReconcCategATMsAtRMCyclesCategoriesATMsRMCycleForTotals(string InOperator, string InReconcCategoryId, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            AllAtmsOpeningBalance = 0;
            AllAtmsJournalAmt = 0;
            AllAtmsMatchedAmtAtMatching = 0;
            AllAtmsMatchedAmtAtDefault = 0;
            AllAtmsMatchedAmtAtWorkFlow = 0;

            SqlString = "SELECT *"
                        + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                        + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle"
                        + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadReaderFields(rdr);

                            AllAtmsOpeningBalance = AllAtmsOpeningBalance + OpeningBalance;
                            AllAtmsJournalAmt = AllAtmsJournalAmt + JournalAmt;
                            AllAtmsMatchedAmtAtMatching = AllAtmsMatchedAmtAtMatching + MatchedAmtAtMatching;
                            AllAtmsMatchedAmtAtDefault = AllAtmsMatchedAmtAtDefault + MatchedAmtAtDefault;
                            AllAtmsMatchedAmtAtWorkFlow = AllAtmsMatchedAmtAtWorkFlow + MatchedAmtAtWorkFlow;

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
        // READ RM ATM RM Cycles
        // FILL UP A TABLE
        //
        string SelectionCriteria;
        bool ItHasOutstandings;
        public void ReadReconcCategoriesATMsRMCycleToFillTable(string InOperator, string InReconcCategoryId, int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // INMODE values
            // 1 : No Updating
            // 2 : With Updating of MatchedAmtAtWorkFlow

            TableRMATMsCycles = new DataTable();
            TableRMATMsCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableRMATMsCycles.Columns.Add("SeqNo", typeof(int));
            TableRMATMsCycles.Columns.Add("AtmNo", typeof(string));
            TableRMATMsCycles.Columns.Add("ReconcCategoryId", typeof(string));
            TableRMATMsCycles.Columns.Add("Matched", typeof(int));
            TableRMATMsCycles.Columns.Add("UnMatched", typeof(int));
            TableRMATMsCycles.Columns.Add("MatchedAmt", typeof(decimal));
            TableRMATMsCycles.Columns.Add("UnMatchedAmt", typeof(decimal));
            TableRMATMsCycles.Columns.Add("NotInJournal", typeof(decimal));
            TableRMATMsCycles.Columns.Add("OpeningBalance", typeof(decimal));
            TableRMATMsCycles.Columns.Add("JournalAmt", typeof(decimal));
            TableRMATMsCycles.Columns.Add("Difference", typeof(decimal));

            TableRMATMsCycles.Columns.Add("MatchedAmtAtDefault", typeof(decimal));
            TableRMATMsCycles.Columns.Add("MatchedAmtAtWorkFlow", typeof(decimal));

            TableRMATMsCycles.Columns.Add("RMCycle", typeof(int));

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                    + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId "
                    + " AND RMCycle = @RMCycle AND NumberOfUnMatchedRecs > 0"
                    + " ORDER BY AtmNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ItHasOutstandings = false;

                            TotalSelected = TotalSelected + 1;

                            ReadReaderFields(rdr);


                            if (RMCycle == InRMCycle)
                            {
                                if (NumberOfUnMatchedRecs > 0) ItHasOutstandings = true;
                            }
                            else
                            {
                                SelectionCriteria = " WHERE Operator ='" + InOperator + "' AND RMCateg ='" + InReconcCategoryId + "'"
                                                                + "  AND MatchingAtRMCycle = " + RMCycle
                                                                + "  AND TerminalId ='" + AtmNo + "'"
                                                                + "  AND IsMatchingDone = 1  "
                                                                + "  AND Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '7' ";

                                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                if (Mpa.RecordFound == true)
                                {
                                    ItHasOutstandings = true;
                                }
                            }

                            if (ItHasOutstandings == true)// There are exceptions for this ATM 
                            {
                                DataRow RowSelected = TableRMATMsCycles.NewRow();
                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["AtmNo"] = AtmNo;
                                RowSelected["ReconcCategoryId"] = ReconcCategoryId;
                                RowSelected["Matched"] = NumberOfMatchedRecs;
                                RowSelected["UnMatched"] = NumberOfUnMatchedRecs;
                                RowSelected["MatchedAmt"] = MatchedAmtAtMatching;
                                RowSelected["UnMatchedAmt"] = UnMatchedAmt;

                                // Find Out How Many Not In Journal 
                                SelectionCriteria = " WHERE Operator ='" + Operator + "' AND RMCateg ='" + ReconcCategoryId
                                          + "' AND TerminalId ='" + AtmNo + "'"
                                           + "  AND MatchingAtRMCycle =" + InRMCycle
                                          + "  AND IsMatchingDone = 1 "
                                          + "  AND ( MatchMask = '01'"
                                          + "  OR MatchMask = '011'"
                                          + "  OR MatchMask = '0111'"
                                          + "  OR MatchMask = '01111'"
                                          + "  OR MatchMask = '011111'"
                                          + "  OR MatchMask = '0111111' )"
                                          + "  AND Matched = 0 AND SettledRecord = 0 " + " AND ActionType != '7' ";

                                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                RowSelected["NotInJournal"] = Mpa.TotalSelected.ToString();

                                RowSelected["OpeningBalance"] = OpeningBalance;
                                RowSelected["JournalAmt"] = JournalAmt;
                                RowSelected["Difference"] = (JournalAmt - (MatchedAmtAtMatching + MatchedAmtAtDefault + MatchedAmtAtWorkFlow)).ToString("#,##0");

                                RowSelected["MatchedAmtAtDefault"] = MatchedAmtAtDefault;
                                RowSelected["MatchedAmtAtWorkFlow"] = MatchedAmtAtWorkFlow;


                                RowSelected["RMCycle"] = RMCycle;

                                // ADD ROW
                                TableRMATMsCycles.Rows.Add(RowSelected);
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
        // READ RM ATM RM Cycles
        // FIND ATM WITH DIFF
        //
        string ReturnValue;
        public string ReadReconcCategoriesATMsRMCycleToFindATMInDiff(string InOperator, string InReconcCategoryId, int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // INMODE values

            // 3 : Show ATM with difference
            ReturnValue = "NOATM";

            TableRMATMsCycles = new DataTable();
            TableRMATMsCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            //TableRMATMsCycles.Columns.Add("RMCycle", typeof(string));
            TableRMATMsCycles.Columns.Add("AtmNo", typeof(string));
            TableRMATMsCycles.Columns.Add("OpeningBalance", typeof(decimal));
            TableRMATMsCycles.Columns.Add("JournalAmt", typeof(decimal));
            TableRMATMsCycles.Columns.Add("MatchedAmtAtMatching", typeof(decimal));
            TableRMATMsCycles.Columns.Add("MatchedAmtAtWorkFlow", typeof(decimal));
            TableRMATMsCycles.Columns.Add("Difference", typeof(decimal));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                    + " WHERE Operator = @Operator AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle "
                    + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            ReadReaderFields(rdr);


                            if (JournalAmt != MatchedAmtAtMatching) // There are exceptions for this ATM 
                            {
                                DataRow RowSelected = TableRMATMsCycles.NewRow();

                                RowSelected["AtmNo"] = AtmNo;
                                RowSelected["OpeningBalance"] = OpeningBalance;
                                RowSelected["JournalAmt"] = JournalAmt;
                                RowSelected["MatchedAmtAtMatching"] = MatchedAmtAtMatching;
                                RowSelected["MatchedAmtAtWorkFlow"] = MatchedAmtAtWorkFlow;

                                RowSelected["Difference"] = (JournalAmt - MatchedAmtAtWorkFlow).ToString("#,##0");

                                // ADD ROW
                                TableRMATMsCycles.Rows.Add(RowSelected);
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    // Read Table and find ATM in Diff 

                    if (InMode == 3)
                    {
                        int I = 0;

                        while (I <= (TableRMATMsCycles.Rows.Count - 1))
                        {
                            AtmNo = (string)TableRMATMsCycles.Rows[I]["AtmNo"];
                            OpeningBalance = (decimal)TableRMATMsCycles.Rows[I]["OpeningBalance"];
                            JournalAmt = (decimal)TableRMATMsCycles.Rows[I]["JournalAmt"];
                            MatchedAmtAtMatching = (decimal)TableRMATMsCycles.Rows[I]["MatchedAmtAtMatching"];

                            ReadAllErrorsTableFromCategSessionForATMAddErrors
                               (ReconcCategoryId, RMCycle, AtmNo, (OpeningBalance + MatchedAmtAtMatching), 4);

                            if (((OpeningBalance + JournalAmt) - BanksClosedBalAdjWithErrors) != 0)
                            {
                                ReturnValue = AtmNo;
                            }
                            //UpdateReconcCategoriesATMsRMCycleForAtm(SeqNo, BanksClosedBalAdjWithErrors);

                            I++;
                        }

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
            return ReturnValue;
        }
        //
        // Methods 
        // TOTALS
        // 
        //
        public int NumberOfPresenter;
        public int NumberOfPresenter_Not_Settled; 
        public void ReadReconcCategoriesATMsRMCycleTotals(string InOperator, string InReconcCategoryId, int InRMCycle, int InAtmGroup)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMGasParameters Gp = new RRDMGasParameters(); 

            bool Is_Presenter_InReconciliation = false;
            // Presenter
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

            // Totals
            TotalAtmsInGroupMatched = 0;
            TotalAtmsInGroupUnMatched = 0;
            TotalNumberOfMatchedRecs = 0;
            TotalNumberOfUnMatchedRecs = 0;
            TotalJournalAmt = 0;
            TotalMatchedAmtAtMatching = 0;
            TotalUnMatchedAmt = 0;
            NumberOfPresenter = 0; 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                    + " WHERE ReconcCategoryId = @ReconcCategoryId "
                                        + " AND RMCycle = @RMCycle "
                                        + " AND AtmGroup = @AtmGroup ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmGroup", InAtmGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadReaderFields(rdr);

                            if (NumberOfUnMatchedRecs == 0)
                            {
                                TotalAtmsInGroupMatched = TotalAtmsInGroupMatched + 1;
                            }
                            if (NumberOfUnMatchedRecs > 0)
                            {
                                TotalAtmsInGroupUnMatched = TotalAtmsInGroupUnMatched + 1;
                            }

                            //if (AtmGroup > 0 & Is_Presenter_InReconciliation == true)
                            //{
                            //    // Our ATMS
                            //    Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(RunningJobNo, CategoryId, 2);
                            //    NumberOfUnMatchedRecs = NumberOfUnMatchedRecs + Mpa.Presenter_Not_Settled + Mpa.PresenterSettled;
                            //    RemainReconcExceptions = RemainReconcExceptions + Mpa.Presenter_Not_Settled;
                            //}


                            TotalNumberOfMatchedRecs = TotalNumberOfMatchedRecs + NumberOfMatchedRecs;
                            TotalNumberOfUnMatchedRecs = TotalNumberOfUnMatchedRecs + NumberOfUnMatchedRecs;

                            TotalJournalAmt = TotalJournalAmt + JournalAmt;

                            TotalMatchedAmtAtMatching = TotalMatchedAmtAtMatching + MatchedAmtAtMatching;
                            TotalUnMatchedAmt = TotalUnMatchedAmt + UnMatchedAmt;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    if (AtmGroup > 0 & Is_Presenter_InReconciliation == true)
                    {
                        // Our ATMS
                        Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(InRMCycle, InReconcCategoryId, 2);
                        NumberOfPresenter = NumberOfPresenter + Mpa.Presenter_Not_Settled + Mpa.PresenterSettled;

                        NumberOfPresenter_Not_Settled = NumberOfPresenter_Not_Settled + Mpa.Presenter_Not_Settled;
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
        //// UPDATE RECONC MATCHED ATMS CYCLE WITH ALL Fields  
        //// 
        public void UpdateReconcCategorATMsRMCycleForAtmALLFields(string InAtmNo, string InReconcCategoryId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategATMsAtRMCycles] SET "
                            //+ " MinMaxTrace = @MinMaxTrace, "
                            + " OwnerUserID = @OwnerUserID, "
                            + " OpeningBalance = @OpeningBalance, "
                            + " JournalAmt = @JournalAmt, "
                            + " MatchedAmtAtMatching = @MatchedAmtAtMatching, "
                            + " MatchedAmtAtDefault = @MatchedAmtAtDefault, "
                            + " MatchedAmtAtWorkFlow = @MatchedAmtAtWorkFlow, "
                            + " NumberOfMatchedRecs = @NumberOfMatchedRecs, "
                            + " UnMatchedAmt = @UnMatchedAmt, "
                            + " Number_Presenter = @Number_Presenter, "
                            + " NumberOfUnMatchedRecs = @NumberOfUnMatchedRecs, "
                            + " Amount_Presenter = @Amount_Presenter, "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE AtmNo = @AtmNo AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@MinMaxTrace", MinMaxTrace);
                        cmd.Parameters.AddWithValue("@OwnerUserID", OwnerUserID);
                        cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);
                        cmd.Parameters.AddWithValue("@JournalAmt", JournalAmt);

                        cmd.Parameters.AddWithValue("@MatchedAmtAtMatching", MatchedAmtAtMatching);

                        cmd.Parameters.AddWithValue("@MatchedAmtAtDefault", MatchedAmtAtDefault);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtWorkFlow", MatchedAmtAtWorkFlow);

                        cmd.Parameters.AddWithValue("@NumberOfMatchedRecs", NumberOfMatchedRecs);

                        cmd.Parameters.AddWithValue("@UnMatchedAmt", UnMatchedAmt);
                        cmd.Parameters.AddWithValue("@NumberOfUnMatchedRecs", NumberOfUnMatchedRecs);

                        cmd.Parameters.AddWithValue("@Number_Presenter", Number_Presenter);
                        cmd.Parameters.AddWithValue("@Amount_Presenter", Amount_Presenter);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

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
        public void InsertReconcCategATMSRecord(string InAtmNo, string InReconcCategoryId, int InRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";
            //int rows; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategATMsAtRMCycles] "
                + " ([ReconcCategoryId], "
                //+ " [MatchingCategoryId],"
                //+ " [MinMaxTrace],"
                + " [AtmGroup],"
                + " [OwnerUserID], "

                + " [RMCycle], "
                + " [AtmNo], "
                + " [CreatedDtTm], "
                + " [Currency],"
                + " [OpeningBalance],"

                + " [JournalAmt], "
                + " [MatchedAmtAtMatching],"
                + " [MatchedAmtAtDefault], "
                + " [MatchedAmtAtWorkFlow], "
                + " [NumberOfMatchedRecs], "

                + " [UnMatchedAmt], "
                + " [NumberOfUnMatchedRecs], "
                + " [Operator]  ) "
                + " VALUES "
                + "(@ReconcCategoryId,"
                //+ " @MatchingCategoryId,"
                //+ " @MinMaxTrace,"
                + " @AtmGroup,"
                + " @OwnerUserID, "

                + " @RMCycle, "
                + " @AtmNo, "
                + " @CreatedDtTm, "
                + " @Currency,"
                + " @OpeningBalance,"

                + " @JournalAmt, "
                + " @MatchedAmtAtMatching, "
                + " @MatchedAmtAtDefault, "
                + " @MatchedAmtAtWorkFlow,"
                + " @NumberOfMatchedRecs,"

                + " @UnMatchedAmt, "
                + " @NumberOfUnMatchedRecs, "
                + " @Operator  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        //cmd.Parameters.AddWithValue("@MatchingCategoryId", InMatchingCategoryId);
                        //cmd.Parameters.AddWithValue("@MinMaxTrace", MinMaxTrace);
                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);
                        cmd.Parameters.AddWithValue("@OwnerUserID", OwnerUserID);

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CreatedDtTm", CreatedDtTm);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@OpeningBalance", OpeningBalance);

                        cmd.Parameters.AddWithValue("@JournalAmt", JournalAmt);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtMatching", MatchedAmtAtMatching);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtDefault", MatchedAmtAtDefault);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtWorkFlow", MatchedAmtAtWorkFlow);
                        cmd.Parameters.AddWithValue("@NumberOfMatchedRecs", NumberOfMatchedRecs);

                        cmd.Parameters.AddWithValue("@UnMatchedAmt", UnMatchedAmt);
                        cmd.Parameters.AddWithValue("@NumberOfUnMatchedRecs", NumberOfUnMatchedRecs);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
        //// 
        //// UPDATE RECONC MATCHED ATMS CYCLE WITH MatchedAmtAtWorkFlow 
        //// 
        public void UpdateReconcCategoriesATMsRMCycleForAtmForAdjusted(string InAtmNo, string InReconcCategoryId, int InRMCycle, decimal InMatchedAmtAtDefault, decimal InMatchedAmtAtWorkFlow)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategATMsAtRMCycles] SET "
                            + " MatchedAmtAtDefault = @MatchedAmtAtDefault, MatchedAmtAtWorkFlow = @MatchedAmtAtWorkFlow "
                            + " WHERE AtmNo = @AtmNo AND ReconcCategoryId = @ReconcCategoryId AND RMCycle = @RMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtDefault", InMatchedAmtAtDefault);
                        cmd.Parameters.AddWithValue("@MatchedAmtAtWorkFlow", InMatchedAmtAtWorkFlow);

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


        // ERRORS 

        decimal ErrAmount;

        public int NumberOfErrors;
        public int NumberOfErrJournal;
        public int ErrJournalThisCycle;
        public int NumberOfErrDep;
        public int NumberOfErrHost;
        public int ErrHostToday;
        public int ErrOutstanding; // Action was not taken on them 
        public int ErrorsAdjastingBalances;

        public bool ErrorsFound;
        public int WBanksClosedBalNubOfErr;
        public int WBanksClosedBalErrOutstanding;
        //public string WBanksClosedBalCurrNm; 
        public decimal BanksClosedBalAdjWithErrors;
        public decimal ErrorsEffectOnUnMatched;

        public int WSesNo;
        public int WFunction;

        // READ Errors TO CALCULATE REFRESED BALANCES 
        //
        public void ReadAllErrorsTableFromCategSessionForATMAddErrors(string InReconcCategoryId, int InActionRMCycle,
                                                string InAtmNo, decimal InBanksClosedBal, int InFunction)

        {
            //WSesNo = InActionRMCycle;
            WFunction = InFunction;
            BanksClosedBalAdjWithErrors = InBanksClosedBal;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsFound = false; // Related to errors 

            NumberOfErrors = 0;
            NumberOfErrJournal = 0;
            ErrJournalThisCycle = 0;
            NumberOfErrDep = 0;
            NumberOfErrHost = 0;
            ErrHostToday = 0;
            ErrOutstanding = 0;

            ErrorsAdjastingBalances = 0;

            WBanksClosedBalNubOfErr = 0;
            WBanksClosedBalErrOutstanding = 0;

            int ErrId; int ErrType;
            string ErrDesc; int TraceNo; int SesNo;
            int UniqueRecordId; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction; bool DisputeAct;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE CategoryId = @CategoryId AND RMCycle<=@ActionRMCycle AND ErrId <> 55 "
                 + " AND AtmNo=@AtmNo AND (OpenErr=1 OR (OpenErr=0 AND ActionRMCycle = @ActionRMCycle))  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InReconcCategoryId);
                        cmd.Parameters.AddWithValue("@ActionRMCycle", InActionRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ErrorsFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrType = (int)rdr["ErrType"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            AtmNo = rdr["AtmNo"].ToString();
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;

                            if ((UnderAction == true || DisputeAct == true) & ErrId != 165) // Do not do the 011
                            {
                                BanksClosedBalAdjWithErrors = BanksClosedBalAdjWithErrors + ErrAmount;
                                ErrorsEffectOnUnMatched = ErrorsEffectOnUnMatched + ErrAmount;
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
      

    }
}



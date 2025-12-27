using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_GL_Balances_For_Categories_And_Atms : Logger
    {
        public RRDM_GL_Balances_For_Categories_And_Atms() : base() { }

        public int SeqNo;
        public string OriginFileName;
        public int OriginalRecordId;
        public DateTime Cut_Off_Date;
        public string MatchingCateg;
        public string AtmNo;
        public string Origin;
        public string TransTypeAtOrigin;
        public string GL_AccountNo;
        public string Ccy;
        public decimal GL_Balance;

        public DateTime DateCreated;
        public bool Processed;
        public int ProcessedAtRMCycle;
        public string Operator;

        // Define the data table 
        public DataTable Table_GL_Balances = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // SQL Reader Fields
        private void ReadRecordFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            OriginFileName = (string)rdr["OriginFileName"];
            OriginalRecordId = (int)rdr["OriginalRecordId"];
            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            AtmNo = (string)rdr["AtmNo"];
            Origin = (string)rdr["Origin"];
            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
            GL_AccountNo = (string)rdr["GL_AccountNo"];
            Ccy = (string)rdr["Ccy"];
            GL_Balance = (decimal)rdr["GL_Balance"];

            DateCreated = (DateTime)rdr["DateCreated"];
            Processed = (bool)rdr["Processed"];
            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];

            Operator = (string)rdr["Operator"];
        }

        //
        // READ Balances and create Table 
        //
        public void Read_GL_Balances_And_Fill_Table(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_GL_Balances = new DataTable();
            Table_GL_Balances.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms]"
               + InSelectionCriteria
               + " Order By SortSequence ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_GL_Balances);

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
        // READ Accounts SeqNo
        // 
        //
        public void Read_GL_Balances_And_BySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms]"
                      + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadRecordFields(rdr);
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
        // READ GL Balance By AtmNo and Cut_Off_Date
        // 
        //
        public void Read_GL_Balances_And_AtmNo_And_Cut_Off_Date(string InAtmNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms]"
                      + " WHERE AtmNo = @AtmNo AND Cut_Off_Date = @Cut_Off_Date ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadRecordFields(rdr);
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
        // Read Transactions > than TranNo   
        //
        public bool IsDataFound;
        public DateTime Last_Cut_Off_Date;
        public decimal TotalDispensed;
        public decimal FoundGlBalance;
        public decimal FindAdjusted_GL_Balance_AND_Update_Session_First_Method(string InOperator, string InAtmNo, int InReplNo, DateTime InReplDate)
        {
            TotalDispensed = 0;
            FoundGlBalance = 0;

            decimal GlAtReplenishment = 0;

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            Na.ReadSessionsNotesAndValues(InAtmNo, InReplNo, 2);

            if (Na.Is_GL_Adjusted == true)
            {
                GlAtReplenishment = Na.GL_Bal_Repl_Adjusted;
            }
            else
            {
                try
                {
                    // FIND Cut OFF Date = Previous than 

                    Last_Cut_Off_Date = NullPastDate;

                    IsDataFound = false;

                    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                    Rjc.Find_GL_Cut_Off_Before_GivenDate(InOperator, InReplDate.Date);
                    if (Rjc.RecordFound == true & Rjc.Counter == 0)
                    {
                        Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                        IsDataFound = true;
                    }
                    else
                    {
                        IsDataFound = false;
                    }

                    // FIND CUT OFF GL Balances from File (table)

                    if (IsDataFound == true)
                    {
                        // Move to next step   
                        Read_GL_Balances_And_AtmNo_And_Cut_Off_Date(InAtmNo, Last_Cut_Off_Date);
                        if (RecordFound == true)
                        {
                            FoundGlBalance = GL_Balance;
                            IsDataFound = true;
                        }
                        else
                        {
                            IsDataFound = false;
                        }
                    }


                    if (IsDataFound == true)
                    {
                        // Find last traces 

                        decimal CategoryDispensed;

                        RRDM_Cut_Off_LastSeqNumbers Cofl = new RRDM_Cut_Off_LastSeqNumbers();

                        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                        Cofl.ReadCIT_G4S_Repl_Entries_For_Table_ByAtmNo_Cut_Off_Date(InAtmNo, Last_Cut_Off_Date);

                        if (Cofl.DataTableCutOffEntries.Rows.Count == 0)
                        {
                            IsDataFound = false;
                        }
                        else
                        {
                            int SeqNo;
                            int I = 0;
                            int K = Cofl.DataTableCutOffEntries.Rows.Count;

                            while (I <= (Cofl.DataTableCutOffEntries.Rows.Count - 1))
                            {
                                RecordFound = true;
                                SeqNo = (int)Cofl.DataTableCutOffEntries.Rows[I]["SeqNo"];

                                Cofl.ReadCIT_G4S_Repl_EntriesSeqNo(SeqNo);

                                if (Cofl.IsSeqNoInMasterFound == true)
                                {

                                    CategoryDispensed = Mpa.ReadInPoolTransTotalGreaterThanFillTable(InAtmNo, Cofl.MatchingCateg
                                        , Cofl.SeqNoInMaster, Cofl.LastTraceDate, Cofl.ReplCycleNo,2);

                                    TotalDispensed = TotalDispensed + CategoryDispensed;
                                }
                                else
                                {
                                    IsDataFound = false;

                                    string SelectionCriteria = " WHERE  MatchingCateg ='" + Cofl.MatchingCateg + "'"
                                                  + " AND  TraceNoWithNoEndZero =" + Cofl.LastTrace
                                                  + " AND  TerminalId ='" + InAtmNo + "' AND IsMatchingDone = 0 "
                                                  + " ORDER By TraceNoWithNoEndZero Desc ";

                                    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,1);

                                    if (Mpa.RecordFound == true)
                                    {
                                        // OK 
                                        IsDataFound = true;
                                        Cofl.IsSeqNoInMasterFound = true;
                                        Cofl.SeqNoInMaster = Mpa.SeqNo;
                                        Cofl.ReplCycleNo = Mpa.ReplCycleNo;

                                        Cofl.UpdateCut_Off_EntriesRecordForMpa(SeqNo);

                                        CategoryDispensed = Mpa.ReadInPoolTransTotalGreaterThan(InAtmNo, Cofl.MatchingCateg, Cofl.SeqNoInMaster
                                            , Cofl.LastTraceDate, Cofl.ReplCycleNo,2);

                                        TotalDispensed = TotalDispensed + CategoryDispensed;
                                    }
                                    else
                                    {
                                        // Not Found 
                                        IsDataFound = false;
                                        break;
                                    }
                                }
                                I++; // Read Next entry of the table 
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CatchDetails(ex);
                }

                // Current Balance 
                GlAtReplenishment = FoundGlBalance - TotalDispensed;

                if (IsDataFound == true)
                {
                    Na.ReadSessionsNotesAndValues(InAtmNo, InReplNo, 2);

                    Na.GL_Balance = FoundGlBalance;

                    Na.Is_GL_Adjusted = true;

                    Na.GL_Bal_Repl_Adjusted = GlAtReplenishment;

                    Na.UpdateSessionsNotesAndValues(InAtmNo, InReplNo);
                }
            }

            return GlAtReplenishment;

        }
        //
        // SECOND METHOD FOR GL 
        //
        public decimal GlOpeningBalance;
        public decimal GlCalculated;
        public decimal GLDifference;
        public int RMCycle; 
        public decimal FindAdjusted_GL_Balance_AND_Update_Session_Second_Method(string InOperator, string InAtmNo, int InReplNo, DateTime InWorkingDate)
        {
            TotalDispensed = 0;
            FoundGlBalance = 0;

            GlOpeningBalance = 0;

            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            Na.ReadSessionsNotesAndValues(InAtmNo, InReplNo, 2);

            GlOpeningBalance = Na.Balances1.OpenBal;

            try
            {
                // FIND Cut OFF Date = Previous than 

                Last_Cut_Off_Date = NullPastDate;

                IsDataFound = false;

                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                Rjc.Find_GL_Cut_Off_Before_GivenDate(InOperator, InWorkingDate.Date);
                if (Rjc.RecordFound == true & Rjc.Counter == 0)
                {
                    RMCycle = Rjc.JobCycle;
                    Last_Cut_Off_Date = Rjc.Cut_Off_Date;
                    IsDataFound = true;
                }
                else
                {
                    IsDataFound = false;
                }

                // FIND CUT OFF GL Balances from File (table)

                if (IsDataFound == true)
                {
                    // Move to next step   
                    Read_GL_Balances_And_AtmNo_And_Cut_Off_Date(InAtmNo, Last_Cut_Off_Date);
                    if (RecordFound == true)
                    {
                        FoundGlBalance = GL_Balance;
                        IsDataFound = true;
                    }
                    else
                    {
                        IsDataFound = false;
                    }
                }


                if (IsDataFound == true)
                {
                    // Find last traces 

                    decimal CategoryDispensed;

                    RRDM_Cut_Off_LastSeqNumbers Cofl = new RRDM_Cut_Off_LastSeqNumbers();

                    RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                    Cofl.ReadCIT_G4S_Repl_Entries_For_Table_ByAtmNo_Cut_Off_Date(InAtmNo, Last_Cut_Off_Date);

                    if (Cofl.DataTableCutOffEntries.Rows.Count == 0)
                    {
                        IsDataFound = false;
                    }
                    else
                    {
                        int SeqNo;
                        int I = 0;
                        int K = Cofl.DataTableCutOffEntries.Rows.Count;

                        while (I <= (Cofl.DataTableCutOffEntries.Rows.Count - 1))
                        {
                            RecordFound = true;
                            SeqNo = (int)Cofl.DataTableCutOffEntries.Rows[I]["SeqNo"];

                            Cofl.ReadCIT_G4S_Repl_EntriesSeqNo(SeqNo);

                            if (Cofl.IsSeqNoInMasterFound == true)
                            {

                                CategoryDispensed = Mpa.ReadInPoolTransTotal_LESS_ThanFillTable(InAtmNo, Cofl.MatchingCateg, Cofl.SeqNoInMaster, Cofl.LastTraceDate, Cofl.ReplCycleNo);

                                TotalDispensed = TotalDispensed + CategoryDispensed;
                            }
                           
                            I++; // Read Next entry of the table 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            // Current Balance 
            //  FoundGlBalance = GlOpeningBalance - TotalDispensed;

            //if (IsDataFound == true)
            //{
            //    Na.ReadSessionsNotesAndValues(InAtmNo, InReplNo, 2);

            //    Na.GL_Balance = FoundGlBalance;

            //    Na.Is_GL_Adjusted = true;

            //    Na.GL_Bal_Repl_Adjusted = GlAtReplenishment;

            //    Na.UpdateSessionsNotesAndValues(InAtmNo, InReplNo);
            //}
            GlCalculated = GlOpeningBalance - TotalDispensed;

            GLDifference = FoundGlBalance - GlCalculated; 

            return GlCalculated;

        }
        //
        // Methods 
        // READ Balances by Selection 
        // 
        //
        public void Read_GL_Balances_And_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms] "
                        + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadRecordFields(rdr);
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
        // READ Balances by Selection 
        // 
        //
        public Decimal Todays_GL_Balance;
        public Decimal Yesterdays_GL_Balance;
        public DateTime LastGlDate;

        public void Read_GL_Balances_And_FindTodaysAndYesterdaysBalance(string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Todays_GL_Balance = 0;
            Yesterdays_GL_Balance = 0;

            TotalSelected = 0;

            SqlString = " SELECT * "
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms] "
                        + " WHERE MatchingCateg = @MatchingCateg "
                        + " Order by SeqNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadRecordFields(rdr);

                            if (TotalSelected == 1)
                            {
                                LastGlDate = Cut_Off_Date;
                                Todays_GL_Balance = GL_Balance;
                            }

                            if (TotalSelected == 2)
                            {
                                Yesterdays_GL_Balance = GL_Balance;
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
        }

        //
        // Insert Insert_GL_Balances
        //
        public int Insert_GL_Balances()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms] "
                        + " ("
                        + " [OriginFileName]"
                        + ",[OriginalRecordId] "
                        + ",[Cut_Off_Date] "
                        + ",[MatchingCateg] "

                        + ",[AtmNo] "
                        + ",[Origin]"
                         + ",[TransTypeAtOrigin]"
                        + ",[GL_AccountNo]"

                        + ",[Ccy] "
                        + ",[GL_Balance]"
                        + ",[DateCreated]"
                        + ",[Processed]"

                        + ",[ProcessedAtRMCycle]"
                        + ",[Operator]"
                        + ")"
                        + " VALUES "
                        + " ("
                        + " @OriginFileName"
                        + ",@OriginalRecordId "
                        + ",@Cut_Off_Date "
                        + ",@MatchingCateg "

                        + ",@AtmNo "
                        + ",@Origin "
                        + ",@TransTypeAtOrigin "
                        + ",@GL_AccountNo "

                        + ",@Ccy "
                        + ",@GL_Balance "
                        + ",@DateCreated "
                        + ",@Processed "

                        + ",@ProcessedAtRMCycle "
                        + ",@Operator "
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

                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Cut_Off_Date);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@GL_AccountNo", GL_AccountNo);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@GL_Balance", GL_Balance);
                        cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
                        cmd.Parameters.AddWithValue("@Processed", Processed);

                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", ProcessedAtRMCycle);
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
        // DELETE BANK
        //
        public void Delete_GL_Entry(string InAtmNo, DateTime InLast_Cut_Off_Date)
        {
            int RecordsDeleted;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_For_Categories_And_Atms] "
                            + " WHERE AtmNo = @AtmNo AND  Cut_Off_Date = @Cut_Off_Date ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InLast_Cut_Off_Date);

                        RecordsDeleted = cmd.ExecuteNonQuery();

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



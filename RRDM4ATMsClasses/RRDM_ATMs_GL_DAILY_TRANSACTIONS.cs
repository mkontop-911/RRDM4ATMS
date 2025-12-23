using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_ATMs_GL_DAILY_TRANSACTIONS : Logger
    {
        public RRDM_ATMs_GL_DAILY_TRANSACTIONS() : base() { }

        public int SeqNo;

        public string AtmNo;
        public int TransType; // 11 for  DR 21 for CR
        public string TransDescr;

        public string TransCurr;
        public decimal TransAmt;
        public DateTime TransDate;

        public bool IsFromCOREBANKING;
        public bool IsFromRRDM_System;
        public bool IsFromRRDM_Manual;

        public string Comment;
        public int ReplCycleNo;
        public int LoadedAtRMCycle;

        public DateTime Cut_Off_Date;
        public string Operator;


        // Define the data table 
        public DataTable Table_GL_TXNS = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // SQL Reader Fields
        private void ReadRecordFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            AtmNo = (string)rdr["AtmNo"];
            TransType = (int)rdr["TransType"];
            TransDescr = (string)rdr["TransDescr"];

            TransCurr = (string)rdr["TransCurr"];
            TransAmt = (decimal)rdr["TransAmt"];
            TransDate = (DateTime)rdr["TransDate"];

            IsFromCOREBANKING = (bool)rdr["IsFromCOREBANKING"];
            IsFromRRDM_System = (bool)rdr["IsFromRRDM_System"];
            IsFromRRDM_Manual = (bool)rdr["IsFromRRDM_Manual"];

            Comment = (string)rdr["Comment"];
            ReplCycleNo = (int)rdr["ReplCycleNo"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];

            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];

            Operator = (string)rdr["Operator"];
        }

        //
        // READ Balances and create Table 
        //
        public void Read_GL_TXNs_And_Fill_Table(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_GL_TXNS = new DataTable();
            Table_GL_TXNS.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS]"
               + InSelectionCriteria
               + " Order By TransDate ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_GL_TXNS);

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
        public void Read_GL_TXNS_BySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS]"
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
        public void Read_GL_TXNS_And_AtmNo_And_Cut_Off_Date(string InAtmNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS]"
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
        // Methods 
        // READ Balances by Selection 
        // 
        //
        public void Read_GL_TXNS_And_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "
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
        public decimal ATM_Replenishment;
        public decimal ATM_Deposits;
        public decimal DB_Corrections_on_ATM;
        public decimal CR_Corrections_on_ATM;
        public void Read_GL_TXNS_And_BySelectionCriteriaGetAll_TXNS_Totals (string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATM_Replenishment = 0;
            ATM_Deposits = 0;
            DB_Corrections_on_ATM = 0;
            CR_Corrections_on_ATM = 0; 

        TotalSelected = 0;

            SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "
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

                            if (TransDescr == "ATM Replenishment")
                            {
                                // Code 11
                                ATM_Replenishment = ATM_Replenishment + TransAmt; 
                            }
                            if (TransDescr == "ATM Deposits")
                            {
                                // Code 21
                                ATM_Deposits = ATM_Deposits + TransAmt;
                            }
                            if (TransDescr == "DB Corrections on ATM")
                            {

                                DB_Corrections_on_ATM = DB_Corrections_on_ATM + TransAmt;
                            }
                            if (TransDescr == "CR Corrections on ATM")
                            {
                                CR_Corrections_on_ATM = CR_Corrections_on_ATM + TransAmt;
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
        // READ Balances by Selection 
        // 
        //
        public decimal Todays_GL_Balance;
        public decimal Yesterdays_GL_Balance;
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
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "
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
                                //  Todays_GL_Balance = GL_Balance;
                            }

                            if (TotalSelected == 2)
                            {
                                //  Yesterdays_GL_Balance = GL_Balance;
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
        // Insert Insert_GL_TXNS 
        //
        //
        public int Insert_GL_Balances()
        {
            // INSERT IS DONE DURING LOADING OF bMaster
            // AND ALso Repl Cycle Number is updated too during file loading
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "

                + " ( "
                + "  [AtmNo] "
                + " ,[TransType]  "
                + " ,[TransDescr]  "
                + " ,[TransCurr]  "
                + "  ,[TransAmt] "
                + " ,[TransDate] "
                + "  ,[IsFromCOREBANKING] "
                + "  ,[IsFromRRDM_System] "
                + " ,[IsFromRRDM_Manual] "
                + " ,[Comment] "
                + " ,[ReplCycleNo] "
                + "  ,[LoadedAtRMCycle] "
                + " ,[Cut_Off_Date] "
                + "  ,[Operator] "
                + ")"
                       
                        + " VALUES "
                        + " ("
                         + "  @AtmNo "
                + " ,@TransType  "
                + " ,@TransDescr  "
                + " ,@TransCurr  "
                + "  ,@TransAmt "
                + " ,@TransDate "
                + "  ,@IsFromCOREBANKING "
                + "  ,@IsFromRRDM_System "
                + " ,@IsFromRRDM_Manual "
                + " ,@Comment "
                + " ,@ReplCycleNo "
                + "  ,@LoadedAtRMCycle "
                + " ,@Cut_Off_Date "
                + "  ,@Operator "
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

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);
                        cmd.Parameters.AddWithValue("@TransAmt", TransAmt);
                        cmd.Parameters.AddWithValue("@TransDate", TransDate);

                        cmd.Parameters.AddWithValue("@IsFromCOREBANKING", IsFromCOREBANKING);
                        cmd.Parameters.AddWithValue("@IsFromRRDM_System", IsFromRRDM_System);
                        cmd.Parameters.AddWithValue("@IsFromRRDM_Manual", IsFromRRDM_Manual);

                        cmd.Parameters.AddWithValue("@Comment", Comment);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Cut_Off_Date);
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
        // DELETE By ATMNo and LoadedCycle Number
        //
        public void Delete_GL_Entry(string InAtmNo,int LoadedAtRMCycle)
        {
            int RecordsDeleted;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "
                            + " WHERE AtmNo = @AtmNo AND  LoadedAtRMCycle = @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

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

        //
        // Delete_GL_Entries_For_This_CYCLE
        //
        public void Delete_GL_Entries_For_This_CYCLE(int LoadedAtRMCycle)
        {
            int RecordsDeleted;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS] "
                            + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle ", conn))
                    {
                       // cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);

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

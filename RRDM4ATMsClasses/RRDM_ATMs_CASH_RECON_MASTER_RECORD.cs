using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_ATMs_CASH_RECON_MASTER_RECORD : Logger
    {
        public RRDM_ATMs_CASH_RECON_MASTER_RECORD() : base() { }

        public int SeqNo;

        public string AtmNo;
        public string AtmName;
        public DateTime Previous_ReplDate;
        public DateTime ReplDate;

        public int ReplCycleNo;
        public int FUI;

        public string SM_DATA;
        public decimal SM_OpeningBalance;
        public decimal SM_Dispensed;

        public decimal SM_Remaining;
        public decimal SM_Cash_Loaded;
        public decimal SM_Cash_Loaded_Minus_SM_Remaining;
        public decimal SM_Deposits;
       
        public string SWITCH_DATA;
        public decimal SWITCH_OpeningBalance;
        public decimal SWITCH_Dispensed;

        public decimal SWITCH_Remaining;
        public decimal SWITCH_Cash_Loaded;
        public decimal SWITCH_Cash_Loaded_Minus_SWITCH_Remaining;
        public decimal SWITCH_Deposits;

        public string FOUND_BY_RRDM;

        public decimal RRDM_PresentedErrors;
        public decimal RRDM_SuspectFound; 

        public decimal RRDM_Deposits_Retracted;
        public string GL_ENTRIES_IN_BOOKS;
        public decimal ATM_Replenishment; //  Same terminology as in bMaster
        public decimal ATM_Deposits; // Same terminology as in bMaster
   
        public decimal DB_Corrections_on_ATM; //  Same terminology as in bMaster
        public decimal CR_Corrections_on_ATM; //  Same terminology as in bMaster
        public bool IsDifferent; // SHOWS THAT DO NOT RECONCILE 

        public decimal OverFound_Cassettes;
        public decimal ShortFound_Cassettes;

        public decimal OverFound_Deposits;
        public decimal ShortFound_Deposits;
      
        public decimal OverFound_Cassettes_SWITCH;
        public decimal ShortFound_Cassettes_SWITCH;

        public decimal OverFound_Deposits_SWITCH;
        public decimal ShortFound_Deposits_SWITCH;

        public decimal InsuranceAmt;
        public bool IsWithinInsurance; // If Loaded <= to insurance for the range of dates 

        public string Closing_REMARKS;
        public decimal SUM_TXNS_Journals_DSP;
        public decimal SUM_TXNS_Journal_DEP;
        public decimal SUM_TXNS_SWITCH_DSP;
        public decimal SUM_TXNS_SWITCH_DEP;

        public decimal SUM_TXNS_COREBANKING_DSP;
        public decimal SUM_TXNS_COREBANKING_DEP;
        public DateTime CreatedDate;
        public DateTime Cut_Off_date;
        public int LoadedAtRMCycle; 

        //Operator nvarchar(8) Unchecked

        public string Operator;


        // Define the data table 
        public DataTable Table_RECONC_RECORDS = new DataTable();

        public DataTable Table_RECONC_RECORDS_CYCLE = new DataTable();

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
            AtmName = (string)rdr["AtmName"];
            Previous_ReplDate = (DateTime)rdr["Previous_ReplDate"];
            ReplDate = (DateTime)rdr["ReplDate"];


            ReplCycleNo = (int)rdr["ReplCycleNo"];
            FUI = (int)rdr["FUI"];

            SM_DATA = (string)rdr["SM_DATA"];
            SM_OpeningBalance = (decimal)rdr["SM_OpeningBalance"];
            SM_Dispensed = (decimal)rdr["SM_Dispensed"];
            SM_Remaining = (decimal)rdr["SM_Remaining"];
            SM_Cash_Loaded = (decimal)rdr["SM_Cash_Loaded"];
            SM_Cash_Loaded_Minus_SM_Remaining = (decimal)rdr["SM_Cash_Loaded_Minus_SM_Remaining"];
            SM_Deposits = (decimal)rdr["SM_Deposits"];
           

            SWITCH_DATA = (string)rdr["SWITCH_DATA"];
            SWITCH_OpeningBalance = (decimal)rdr["SWITCH_OpeningBalance"];
            SWITCH_Dispensed = (decimal)rdr["SWITCH_Dispensed"];
            SWITCH_Remaining = (decimal)rdr["SWITCH_Remaining"];
            SWITCH_Cash_Loaded = (decimal)rdr["SWITCH_Cash_Loaded"];
            SWITCH_Cash_Loaded_Minus_SWITCH_Remaining = (decimal)rdr["SWITCH_Cash_Loaded_Minus_SWITCH_Remaining"];
            SWITCH_Deposits = (decimal)rdr["SWITCH_Deposits"];

            FOUND_BY_RRDM = (string)rdr["FOUND_BY_RRDM"];

            RRDM_PresentedErrors = (decimal)rdr["RRDM_PresentedErrors"];
            RRDM_SuspectFound = (decimal)rdr["RRDM_SuspectFound"];

            RRDM_Deposits_Retracted = (decimal)rdr["RRDM_Deposits_Retracted"];

            GL_ENTRIES_IN_BOOKS = (string)rdr["GL_ENTRIES_IN_BOOKS"];
            ATM_Replenishment = (decimal)rdr["ATM_Replenishment"];
            ATM_Deposits = (decimal)rdr["ATM_Deposits"];

            DB_Corrections_on_ATM = (decimal)rdr["DB_Corrections_on_ATM"];
            CR_Corrections_on_ATM = (decimal)rdr["CR_Corrections_on_ATM"];
            IsDifferent = (bool)rdr["IsDifferent"];

            OverFound_Cassettes = (decimal)rdr["OverFound_Cassettes"];
            ShortFound_Cassettes = (decimal)rdr["ShortFound_Cassettes"];

            OverFound_Deposits = (decimal)rdr["OverFound_Deposits"];
            ShortFound_Deposits = (decimal)rdr["ShortFound_Deposits"];

            OverFound_Cassettes_SWITCH = (decimal)rdr["OverFound_Cassettes_SWITCH"];
            ShortFound_Cassettes_SWITCH = (decimal)rdr["ShortFound_Cassettes_SWITCH"];

            OverFound_Deposits_SWITCH = (decimal)rdr["OverFound_Deposits_SWITCH"];
            ShortFound_Deposits_SWITCH = (decimal)rdr["ShortFound_Deposits_SWITCH"];

            InsuranceAmt = (decimal)rdr["InsuranceAmt"];
            IsWithinInsurance = (bool)rdr["IsWithinInsurance"];

            Closing_REMARKS = (string)rdr["Closing_REMARKS"];
            SUM_TXNS_Journals_DSP = (decimal)rdr["SUM_TXNS_Journals_DSP"];
            SUM_TXNS_Journal_DEP = (decimal)rdr["SUM_TXNS_Journal_DEP"];
            SUM_TXNS_SWITCH_DSP = (decimal)rdr["SUM_TXNS_SWITCH_DSP"];
            SUM_TXNS_SWITCH_DEP = (decimal)rdr["SUM_TXNS_SWITCH_DEP"];


            SUM_TXNS_COREBANKING_DSP = (decimal)rdr["SUM_TXNS_COREBANKING_DSP"];
            SUM_TXNS_COREBANKING_DEP = (decimal)rdr["SUM_TXNS_COREBANKING_DEP"];
            CreatedDate = (DateTime)rdr["CreatedDate"];
            Cut_Off_date = (DateTime)rdr["Cut_Off_date"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];

            Operator = (string)rdr["Operator"];
        }

        //
        // READ Balances and create Table 
        //
        public void Read_CASH_RECON_And_Fill_Table(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_RECONC_RECORDS = new DataTable();
            Table_RECONC_RECORDS.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD]"
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

                        sqlAdapt.Fill(Table_RECONC_RECORDS);

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
        // For CASH RECONCILIATION
        //
        public void Read_Cash_Recon_By_Cycle(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_RECONC_RECORDS_CYCLE = new DataTable();
            Table_RECONC_RECORDS_CYCLE.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            Table_RECONC_RECORDS_CYCLE.Columns.Add("Line Type", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("ATM No", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("Date", typeof(string));

            Table_RECONC_RECORDS_CYCLE.Columns.Add("Opening", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("Remaining", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("New Loaded", typeof(string));

            Table_RECONC_RECORDS_CYCLE.Columns.Add("Added Cash", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("GL Cash", typeof(string));

            Table_RECONC_RECORDS_CYCLE.Columns.Add("Deposits", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("GL Deposits", typeof(string));

            Table_RECONC_RECORDS_CYCLE.Columns.Add("Count Excess", typeof(string));
            Table_RECONC_RECORDS_CYCLE.Columns.Add("Count Shortage", typeof(string));

            Table_RECONC_RECORDS_CYCLE.Columns.Add("DIFF", typeof(string));



            string SqlString = " SELECT  * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD] "
                     + InSelectionCriteria
                     + " Order by AtmNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadRecordFields(rdr);

                            TotalSelected = TotalSelected + 1;
                            // -------- INSERT FIRST LINE --------
                            DataRow row1 = Table_RECONC_RECORDS_CYCLE.NewRow();

                            row1["Line Type"] = "JOURNAL";
                            row1["ATM No"] = AtmNo;
                            row1["Date"] = ReplDate.ToString();

                            row1["Opening"] = SM_OpeningBalance.ToString("#,##0.00");
                            row1["Remaining"] = SM_Remaining.ToString("#,##0.00");
                            row1["New Loaded"] = SM_Cash_Loaded.ToString("#,##0.00");

                            row1["Added Cash"] = SM_Cash_Loaded_Minus_SM_Remaining.ToString("#,##0.00");
                            row1["GL Cash"] = ATM_Replenishment.ToString("#,##0.00");

                            row1["Deposits"] = SM_Deposits.ToString("#,##0.00");
                            row1["GL Deposits"] = ATM_Deposits.ToString("#,##0.00");

                        
                            row1["Count Excess"] = OverFound_Cassettes.ToString("#,##0.00");
                            row1["Count Shortage"] = ShortFound_Cassettes.ToString("#,##0.00");

                            if (OverFound_Cassettes == 0 & ShortFound_Cassettes == 0)
                            {
                                row1["DIFF"] ="NO DIFF";
                            }
                            else
                            {
                                row1["DIFF"] = "YES DIFF";
                            }
                           

                            Table_RECONC_RECORDS_CYCLE.Rows.Add(row1);

                            // -------- INSERT SECOND LINE --------
                            DataRow row2 = Table_RECONC_RECORDS_CYCLE.NewRow();

                            row2["Line Type"] = "SWITCH";
                            row2["ATM No"] = "SAME";
                            row2["Date"] = "SAME";

                            row2["Opening"] = SWITCH_OpeningBalance.ToString("#,##0.00");
                            row2["Remaining"] = SWITCH_Remaining.ToString("#,##0.00");
                            row2["New Loaded"] = SWITCH_Cash_Loaded.ToString("#,##0.00");

                            row2["Added Cash"] = SWITCH_Cash_Loaded_Minus_SWITCH_Remaining.ToString("#,##0.00");
                            row2["GL Cash"] = ATM_Replenishment.ToString("#,##0.00");

                            row2["Deposits"] = SWITCH_Deposits.ToString("#,##0.00");
                            row2["GL Deposits"] = ATM_Deposits.ToString("#,##0.00");


                            row2["Count Excess"] = OverFound_Cassettes_SWITCH.ToString("#,##0.00");
                            row2["Count Shortage"] = ShortFound_Cassettes_SWITCH.ToString("#,##0.00");

                            if (OverFound_Cassettes_SWITCH == 0 & ShortFound_Cassettes_SWITCH == 0)
                            {
                                row2["DIFF"] = "NO DIFF";
                            }
                            else
                            {
                                row2["DIFF"] = "YES DIFF";
                            }


                            Table_RECONC_RECORDS_CYCLE.Rows.Add(row2);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //  InsertWReportAtmRepl(InSignedId);
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
        public void Read_CASH_RECON_BySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD]"
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
        public void Read_CASH_RECON_Cut_Off_Date(string InAtmNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD]"
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
        public void Read_CASH_RECON_And_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD] "
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
        // Insert Insert_CASH_RECON
        //
        public int Insert_CASH_RECON(string InAtmNo, int ReplCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD] "

                + " ( "
                + " [AtmNo] "
           + " ,[AtmName] "
           + " ,[Previous_ReplDate] "
           + " ,[ReplDate] "
           + " ,[ReplCycleNo] "

           + " ,[FUI] "

           + " ,[SM_DATA] "
           + " ,[SM_OpeningBalance] "
           + " ,[SM_Dispensed] "
           + " ,[SM_Remaining] "
           + " ,[SM_Cash_Loaded] "
           + " ,[SM_Cash_Loaded_Minus_SM_Remaining] "
           + " ,[SM_Deposits] "

           + " ,[SWITCH_DATA] "
           + " ,[SWITCH_OpeningBalance] "
           + " ,[SWITCH_Dispensed] "
           + " ,[SWITCH_Remaining] "
           + " ,[SWITCH_Cash_Loaded] "
           + " ,[SWITCH_Cash_Loaded_Minus_SWITCH_Remaining] "
           + " ,[SWITCH_Deposits] "

           + " ,[FOUND_BY_RRDM] "
           + " ,[RRDM_PresentedErrors] "
            + " ,[RRDM_SuspectFound] "

           + " ,[RRDM_Deposits_Retracted] "
           + " ,[GL_ENTRIES_IN_BOOKS] "
           + " ,[ATM_Replenishment] "
           + " ,[ATM_Deposits] "
           + " ,[DB_Corrections_on_ATM] "

           + " ,[CR_Corrections_on_ATM] "
           + " ,[IsDifferent] "
           + " ,[OverFound_Cassettes] "
           + " ,[ShortFound_Cassettes] "
             + " ,[OverFound_Deposits] "
           + " ,[ShortFound_Deposits] "
           + " ,[OverFound_Cassettes_SWITCH] "
           + " ,[ShortFound_Cassettes_SWITCH] "
             + " ,[OverFound_Deposits_SWITCH] "
           + " ,[ShortFound_Deposits_SWITCH] "
             + " ,[InsuranceAmt] "
               + " ,[IsWithinInsurance] "
           + " ,[Closing_REMARKS] "
            + " ,[SUM_TXNS_Journals_DSP] "
           + " ,[SUM_TXNS_Journal_DEP] "
           + " ,[SUM_TXNS_SWITCH_DSP] "
           + "  ,[SUM_TXNS_SWITCH_DEP] "
           + " ,[SUM_TXNS_COREBANKING_DSP] "

           + " ,[SUM_TXNS_COREBANKING_DEP] "
           + " ,[CreatedDate] "
           + " ,[Cut_Off_date] "
           + " ,[LoadedAtRMCycle] "
           + " ,[Operator] "
                + ")"
                       
                        + " VALUES "
                        + " ("
                  + " @AtmNo "
           + " ,@AtmName "
           + " ,@Previous_ReplDate "
           + " ,@ReplDate "
           + " ,@ReplCycleNo "

           + " ,@FUI "

           + " ,@SM_DATA "
           + " ,@SM_OpeningBalance "
           + " ,@SM_Dispensed "
           + " ,@SM_Remaining "
           + " ,@SM_Cash_Loaded "
           + " ,@SM_Cash_Loaded_Minus_SM_Remaining "
           + " ,@SM_Deposits "

           + " ,@SWITCH_DATA "
           + " ,@SWITCH_OpeningBalance "
           + " ,@SWITCH_Dispensed "
           + " ,@SWITCH_Remaining "
           + " ,@SWITCH_Cash_Loaded "
           + " ,@SWITCH_Cash_Loaded_Minus_SWITCH_Remaining "
           + " ,@SWITCH_Deposits "

           + " ,@FOUND_BY_RRDM "
           + " ,@RRDM_PresentedErrors "
             + " ,@RRDM_SuspectFound "

           + " ,@RRDM_Deposits_Retracted "
           + " ,@GL_ENTRIES_IN_BOOKS "
           + " ,@ATM_Replenishment "
           + " ,@ATM_Deposits "
           + " ,@DB_Corrections_on_ATM "

           + " ,@CR_Corrections_on_ATM "
           + " ,@IsDifferent "
           + " ,@OverFound_Cassettes "
           + " ,@ShortFound_Cassettes "
             + " ,@OverFound_Deposits "
           + " ,@ShortFound_Deposits "
              + " ,@OverFound_Cassettes_SWITCH "
           + " ,@ShortFound_Cassettes_SWITCH "
             + " ,@OverFound_Deposits_SWITCH "
           + " ,@ShortFound_Deposits_SWITCH "
             + " ,@InsuranceAmt "
               + " ,@IsWithinInsurance "
           + " ,@Closing_REMARKS "

           + " ,@SUM_TXNS_Journals_DSP "
           + " ,@SUM_TXNS_Journal_DEP "
           + " ,@SUM_TXNS_SWITCH_DSP "
           + "  ,@SUM_TXNS_SWITCH_DEP "
           + " ,@SUM_TXNS_COREBANKING_DSP "

           + " ,@SUM_TXNS_COREBANKING_DEP "
           + " ,@CreatedDate "
           + " ,@Cut_Off_date "
           + " ,@LoadedAtRMCycle "
           + " ,@Operator "
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
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@Previous_ReplDate", Previous_ReplDate);
                        cmd.Parameters.AddWithValue("@ReplDate", ReplDate);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@FUI", FUI);

                        cmd.Parameters.AddWithValue("@SM_DATA", "SM_DATA");
                        cmd.Parameters.AddWithValue("@SM_OpeningBalance", SM_OpeningBalance);
                        cmd.Parameters.AddWithValue("@SM_Dispensed", SM_Dispensed);
                        cmd.Parameters.AddWithValue("@SM_Remaining", SM_Remaining);
                        cmd.Parameters.AddWithValue("@SM_Cash_Loaded", SM_Cash_Loaded);
                        cmd.Parameters.AddWithValue("@SM_Cash_Loaded_Minus_SM_Remaining", SM_Cash_Loaded_Minus_SM_Remaining);
                        cmd.Parameters.AddWithValue("@SM_Deposits", SM_Deposits);

                        cmd.Parameters.AddWithValue("@SWITCH_DATA", "SWITCH_DATA");
                        cmd.Parameters.AddWithValue("@SWITCH_OpeningBalance", SWITCH_OpeningBalance);
                        cmd.Parameters.AddWithValue("@SWITCH_Dispensed", SWITCH_Dispensed);
                        cmd.Parameters.AddWithValue("@SWITCH_Remaining", SWITCH_Remaining);
                        cmd.Parameters.AddWithValue("@SWITCH_Cash_Loaded", SWITCH_Cash_Loaded);
                        cmd.Parameters.AddWithValue("@SWITCH_Cash_Loaded_Minus_SWITCH_Remaining", SWITCH_Cash_Loaded_Minus_SWITCH_Remaining);
                        cmd.Parameters.AddWithValue("@SWITCH_Deposits", SWITCH_Deposits);

                        cmd.Parameters.AddWithValue("@FOUND_BY_RRDM", "FOUND_BY_RRDM");
                        cmd.Parameters.AddWithValue("@RRDM_PresentedErrors", RRDM_PresentedErrors);
                        cmd.Parameters.AddWithValue("@RRDM_SuspectFound", RRDM_SuspectFound);

                        cmd.Parameters.AddWithValue("@RRDM_Deposits_Retracted", RRDM_Deposits_Retracted);
                        cmd.Parameters.AddWithValue("@GL_ENTRIES_IN_BOOKS", "GL_ENTRIES_IN_BOOKS");
                        cmd.Parameters.AddWithValue("@ATM_Replenishment", ATM_Replenishment);
                        cmd.Parameters.AddWithValue("@ATM_Deposits", ATM_Deposits);
                        cmd.Parameters.AddWithValue("@DB_Corrections_on_ATM", DB_Corrections_on_ATM);

                        cmd.Parameters.AddWithValue("@CR_Corrections_on_ATM", CR_Corrections_on_ATM);
                        cmd.Parameters.AddWithValue("@IsDifferent", IsDifferent);

                        cmd.Parameters.AddWithValue("@OverFound_Cassettes", OverFound_Cassettes);
                        cmd.Parameters.AddWithValue("@ShortFound_Cassettes", ShortFound_Cassettes);
                        cmd.Parameters.AddWithValue("@OverFound_Deposits", OverFound_Deposits);
                        cmd.Parameters.AddWithValue("@ShortFound_Deposits", ShortFound_Deposits);

                        cmd.Parameters.AddWithValue("@OverFound_Cassettes_SWITCH", OverFound_Cassettes_SWITCH);
                        cmd.Parameters.AddWithValue("@ShortFound_Cassettes_SWITCH", ShortFound_Cassettes_SWITCH);
                        cmd.Parameters.AddWithValue("@OverFound_Deposits_SWITCH", OverFound_Deposits_SWITCH);
                        cmd.Parameters.AddWithValue("@ShortFound_Deposits_SWITCH", ShortFound_Deposits_SWITCH);
                        cmd.Parameters.AddWithValue("@InsuranceAmt", InsuranceAmt);
                        cmd.Parameters.AddWithValue("@IsWithinInsurance", IsWithinInsurance);

                        cmd.Parameters.AddWithValue("@Closing_REMARKS", "Closing_REMARKS"); // AT INSERT STAGE

                        cmd.Parameters.AddWithValue("@SUM_TXNS_Journals_DSP", SUM_TXNS_Journals_DSP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_Journal_DEP", SUM_TXNS_Journal_DEP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_SWITCH_DSP", SUM_TXNS_SWITCH_DSP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_SWITCH_DEP", SUM_TXNS_SWITCH_DEP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_COREBANKING_DSP", SUM_TXNS_COREBANKING_DSP);
                      
                        cmd.Parameters.AddWithValue("@SUM_TXNS_COREBANKING_DEP", SUM_TXNS_COREBANKING_DEP);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Cut_Off_date", Cut_Off_date);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
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

        // UPDATE 

        public void Update_CASH_RECON(int InSeqNo)
        {
            int rows;

            string strUpdate = " UPDATE [RRDM_Reconciliation_ITMX].[dbo].ATMs_CASH_RECON_MASTER_RECORD"
                + " SET "
              +"   AtmNo = @AtmNo "
              + " , AtmName = @AtmName"
      + " , Previous_ReplDate = @Previous_ReplDate "
      + " , ReplDate = @ReplDate "
     + " , ReplCycleNo = @ReplCycleNo "
     + " , FUI = @FUI "

      + " , SM_DATA = @SM_DATA "
      + " , SM_OpeningBalance = @SM_OpeningBalance "
     + " , SM_Dispensed = @SM_Dispensed "
      + " , SM_Remaining = @SM_Remaining "
     + " , SM_Cash_Loaded = @SM_Cash_Loaded "
     + " , SM_Cash_Loaded_Minus_SM_Remaining = @SM_Cash_Loaded_Minus_SM_Remaining "
     + " , SM_Deposits = @SM_Deposits "

       + " , SWITCH_DATA = @SWITCH_DATA "
      + " , SWITCH_OpeningBalance = @SWITCH_OpeningBalance "
     + " , SWITCH_Dispensed = @SWITCH_Dispensed "
      + " , SWITCH_Remaining = @SWITCH_Remaining "
     + " , SWITCH_Cash_Loaded = @SWITCH_Cash_Loaded "
     + " , SWITCH_Cash_Loaded_Minus_SWITCH_Remaining = @SWITCH_Cash_Loaded_Minus_SWITCH_Remaining "
     + " , SWITCH_Deposits = @SWITCH_Deposits "

     + " , FOUND_BY_RRDM = @FOUND_BY_RRDM "
      + " , RRDM_PresentedErrors = @RRDM_PresentedErrors "
       + " , RRDM_SuspectFound = @RRDM_SuspectFound "

     + " , RRDM_Deposits_Retracted = @RRDM_Deposits_Retracted "
     + " , GL_ENTRIES_IN_BOOKS = @GL_ENTRIES_IN_BOOKS "
    + " , ATM_Replenishment = @ATM_Replenishment "
    + " , ATM_Deposits = @ATM_Deposits "
     + " , DB_Corrections_on_ATM = @DB_Corrections_on_ATM "
     + " , CR_Corrections_on_ATM = @CR_Corrections_on_ATM "
      + " , IsDifferent = @IsDifferent "
     + " , OverFound_Cassettes = @OverFound_Cassettes "
     + " , ShortFound_Cassettes = @ShortFound_Cassettes "
      + " , OverFound_Deposits = @OverFound_Deposits "
     + " , ShortFound_Deposits = @ShortFound_Deposits "
     + " , OverFound_Cassettes_SWITCH = @OverFound_Cassettes_SWITCH "
     + " , ShortFound_Cassettes_SWITCH = @ShortFound_Cassettes_SWITCH "
      + " , OverFound_Deposits_SWITCH = @OverFound_Deposits_SWITCH "
     + " , ShortFound_Deposits_SWITCH = @ShortFound_Deposits_SWITCH "
     + " , InsuranceAmt = @InsuranceAmt "
     + " , IsWithinInsurance = @IsWithinInsurance "
     
            + " , Closing_REMARKS = @Closing_REMARKS "
     + " , SUM_TXNS_Journals_DSP = @SUM_TXNS_Journals_DSP "
      + " , SUM_TXNS_Journal_DEP = @SUM_TXNS_Journal_DEP "
     + " , SUM_TXNS_SWITCH_DSP = @SUM_TXNS_SWITCH_DSP "
     + " , SUM_TXNS_SWITCH_DEP = @SUM_TXNS_SWITCH_DEP "
      + " , SUM_TXNS_COREBANKING_DSP = @SUM_TXNS_COREBANKING_DSP "
      + " , SUM_TXNS_COREBANKING_DEP = @SUM_TXNS_COREBANKING_DEP "
      + " , CreatedDate = @CreatedDate "
      + " , Cut_Off_date = @Cut_Off_date "
      + " , LoadedAtRMCycle = @LoadedAtRMCycle "
      + " , Operator = @Operator "

                + " WHERE SeqNo=@SeqNo ";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@Previous_ReplDate", Previous_ReplDate);
                        cmd.Parameters.AddWithValue("@ReplDate", ReplDate);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@FUI", FUI);

                        cmd.Parameters.AddWithValue("@SM_DATA", "SM_DATA");
                        cmd.Parameters.AddWithValue("@SM_OpeningBalance", SM_OpeningBalance);
                        cmd.Parameters.AddWithValue("@SM_Dispensed", SM_Dispensed);
                        cmd.Parameters.AddWithValue("@SM_Remaining", SM_Remaining);

                        cmd.Parameters.AddWithValue("@SM_Cash_Loaded", SM_Cash_Loaded);
                        cmd.Parameters.AddWithValue("@SM_Cash_Loaded_Minus_SM_Remaining", SM_Cash_Loaded_Minus_SM_Remaining);
                        cmd.Parameters.AddWithValue("@SM_Deposits", SM_Deposits);

                        cmd.Parameters.AddWithValue("@SWITCH_DATA", "SWITCH_DATA");
                        cmd.Parameters.AddWithValue("@SWITCH_OpeningBalance", SWITCH_OpeningBalance);
                        cmd.Parameters.AddWithValue("@SWITCH_Dispensed", SWITCH_Dispensed);
                        cmd.Parameters.AddWithValue("@SWITCH_Remaining", SWITCH_Remaining);
                        cmd.Parameters.AddWithValue("@SWITCH_Cash_Loaded", SWITCH_Cash_Loaded);
                        cmd.Parameters.AddWithValue("@SWITCH_Cash_Loaded_Minus_SWITCH_Remaining", SWITCH_Cash_Loaded_Minus_SWITCH_Remaining);
                        cmd.Parameters.AddWithValue("@SWITCH_Deposits", SWITCH_Deposits);


                        cmd.Parameters.AddWithValue("@FOUND_BY_RRDM", "FOUND_BY_RRDM");
                        cmd.Parameters.AddWithValue("@RRDM_PresentedErrors", RRDM_PresentedErrors);
                        cmd.Parameters.AddWithValue("@RRDM_SuspectFound", RRDM_SuspectFound);

                        cmd.Parameters.AddWithValue("@RRDM_Deposits_Retracted", RRDM_Deposits_Retracted);

                        cmd.Parameters.AddWithValue("@GL_ENTRIES_IN_BOOKS", "GL_ENTRIES_IN_BOOKS");
                        cmd.Parameters.AddWithValue("@ATM_Replenishment", ATM_Replenishment);
                        cmd.Parameters.AddWithValue("@ATM_Deposits", ATM_Deposits);
                        cmd.Parameters.AddWithValue("@DB_Corrections_on_ATM", DB_Corrections_on_ATM);
                        cmd.Parameters.AddWithValue("@CR_Corrections_on_ATM", CR_Corrections_on_ATM);

                        cmd.Parameters.AddWithValue("@IsDifferent", IsDifferent);

                        cmd.Parameters.AddWithValue("@OverFound_Cassettes", OverFound_Cassettes);
                        cmd.Parameters.AddWithValue("@ShortFound_Cassettes", ShortFound_Cassettes);
                        cmd.Parameters.AddWithValue("@OverFound_Deposits", OverFound_Deposits);
                        cmd.Parameters.AddWithValue("@ShortFound_Deposits", ShortFound_Deposits);

                        cmd.Parameters.AddWithValue("@OverFound_Cassettes_SWITCH", OverFound_Cassettes_SWITCH);
                        cmd.Parameters.AddWithValue("@ShortFound_Cassettes_SWITCH", ShortFound_Cassettes_SWITCH);
                        cmd.Parameters.AddWithValue("@OverFound_Deposits_SWITCH", OverFound_Deposits_SWITCH);
                        cmd.Parameters.AddWithValue("@ShortFound_Deposits_SWITCH", ShortFound_Deposits_SWITCH);

                        cmd.Parameters.AddWithValue("@InsuranceAmt", InsuranceAmt);
                        cmd.Parameters.AddWithValue("@IsWithinInsurance", IsWithinInsurance);

                        cmd.Parameters.AddWithValue("@Closing_REMARKS", "Closing_REMARKS");

                        cmd.Parameters.AddWithValue("@SUM_TXNS_Journals_DSP", SUM_TXNS_Journals_DSP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_Journal_DEP", SUM_TXNS_Journal_DEP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_SWITCH_DSP", SUM_TXNS_SWITCH_DSP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_SWITCH_DEP", SUM_TXNS_SWITCH_DEP);
                        cmd.Parameters.AddWithValue("@SUM_TXNS_COREBANKING_DSP", SUM_TXNS_COREBANKING_DSP);

                        cmd.Parameters.AddWithValue("@SUM_TXNS_COREBANKING_DEP", SUM_TXNS_COREBANKING_DEP);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Cut_Off_date", Cut_Off_date);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
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
        // DELETE By ATMNo and LoadedCycle Number
        //
        public void Delete_CASH_RECON_Entry(string InAtmNo,int LoadedAtRMCycle)
        {
            int RecordsDeleted;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD] "
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
        public void Delete_CASH_RECON_For_This_CYCLE(int LoadedAtRMCycle)
        {
            int RecordsDeleted;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[ATMs_CASH_RECON_MASTER_RECORD] "
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

using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_SM_SWITCH_TOTALS : Logger
    {
        public RRDM_SM_SWITCH_TOTALS() : base() { }
     
        //        SeqNo int Unchecked
        //OriginSeqNo int Unchecked
        //AtmNo nvarchar(30)    Unchecked
        //FUI int Unchecked
        //Datetime_Current datetime    Unchecked
        public int SeqNo;
        public int OriginSeqNo; // SeqNo in PANICOS_SM_TABLE
        public string AtmNo;
        public int FUI;

        public string FlagValid; // "Y" is Valid "N" is not valid 
        public string AdditionalCash; // "Y" = Additional Cash "N" not Additional cash 

        public int sessionstart_ruid;
        public int sessionend_ruid;

        public DateTime Datetime_Current;

        //Current_C1IN    decimal (18, 2)	Unchecked
        //Current_C1OUT   decimal (18, 2)	Unchecked
        //Current_C2IN    decimal (18, 2)	Unchecked
        //Current_C2OUT   decimal (18, 2)	Unchecked

        // Current_C3IN    decimal (18, 2)	Unchecked
        //  Current_C3OUT   decimal (18, 2)	Unchecked
        //   Current_C4IN    decimal (18, 2)	Unchecked
        //    Current_C4OUT   decimal (18, 2)	Unchecked

        public decimal Current_C1IN;
        public decimal Current_C1OUT;
        public decimal Current_C2IN;
        public decimal Current_C2OUT;

        public decimal Current_C3IN;
        public decimal Current_C3OUT;
        public decimal Current_C4IN;
        public decimal Current_C4OUT;

        //     Current_DEP_NO  int Unchecked
        //Current_DEP_Amt decimal (18, 2)	Unchecked
        // DateTime_Loaded datetime Unchecked
        public int Current_DEP_NO;
        public decimal Current_DEP_Amt;
        public int Current_PAY_NO;
        public decimal Current_PAY_Amt;
      //  ,[Current_PAY_NO]
      //,[Current_PAY_Amt]
        public DateTime DateTime_Loaded;


        //Loaded_C1IN decimal (18, 2)	Unchecked
        // Loaded_C2IN decimal (18, 2)	Unchecked
        //  Loaded_C3IN decimal (18, 2)	Unchecked
        //   Loaded_C4IN decimal (18, 2)	Unchecked

        public decimal Loaded_C1IN;
        public decimal Loaded_C2IN;
        public decimal Loaded_C3IN;
        public decimal Loaded_C4IN;


        //    DateTime_Transaction_END    datetime Unchecked
        //LoadedAtRMCycle int Unchecked
        public DateTime DateTime_Transaction_END;
        public int LoadedAtRMCycle;

        public decimal SWITCH_OpeningBalance;
        public decimal SWITCH_Dispensed;
        public decimal SWITCH_Remaining;
        public decimal SWITCH_Cash_Loaded;
        public decimal SWITCH_Cash_Loaded_Minus_SWITCH_Remaining;
        public decimal SWITCH_Deposits; 

        // Define the data table 
        public DataTable Table_SM_SWITCH_TOTALS = new DataTable();

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
            //        SeqNo int Unchecked
            //OriginSeqNo int Unchecked
            //AtmNo nvarchar(30)    Unchecked
            //FUI int Unchecked
            //Datetime_Current datetime    Unchecked
            SeqNo = (int)rdr["SeqNo"];
            OriginSeqNo = (int)rdr["OriginSeqNo"];
            AtmNo = (string)rdr["AtmNo"];
            FUI = (int)rdr["FUI"];

            FlagValid = (string)rdr["FlagValid"];
            AdditionalCash = (string)rdr["AdditionalCash"];

            sessionstart_ruid = (int)rdr["sessionstart_ruid"];
            sessionend_ruid = (int)rdr["sessionend_ruid"];

            Datetime_Current = (DateTime)rdr["Datetime_Current"];

            //Current_C1IN    decimal (18, 2)	Unchecked
            //Current_C1OUT   decimal (18, 2)	Unchecked
            //Current_C2IN    decimal (18, 2)	Unchecked
            //Current_C2OUT   decimal (18, 2)	Unchecked
            Current_C1IN = (decimal)rdr["Current_C1IN"];
            Current_C1OUT = (decimal)rdr["Current_C1OUT"];
            Current_C2IN = (decimal)rdr["Current_C2IN"];
            Current_C2OUT = (decimal)rdr["Current_C2OUT"];
            // Current_C3IN    decimal (18, 2)	Unchecked
            //  Current_C3OUT   decimal (18, 2)	Unchecked
            //   Current_C4IN    decimal (18, 2)	Unchecked
            //    Current_C4OUT   decimal (18, 2)	Unchecked

            Current_C3IN = (decimal)rdr["Current_C3IN"];
            Current_C3OUT = (decimal)rdr["Current_C3OUT"];
            Current_C4IN = (decimal)rdr["Current_C4IN"];
            Current_C4OUT = (decimal)rdr["Current_C4OUT"];

            //     Current_DEP_NO  int Unchecked
            //Current_DEP_Amt decimal (18, 2)	Unchecked
            // DateTime_Loaded datetime Unchecked
            Current_DEP_NO = (int)rdr["Current_DEP_NO"];
            Current_DEP_Amt = (decimal)rdr["Current_DEP_Amt"];
            Current_PAY_NO = (int)rdr["Current_PAY_NO"];
            Current_PAY_Amt = (decimal)rdr["Current_PAY_Amt"];
             
            DateTime_Loaded = (DateTime)rdr["DateTime_Loaded"];

            //Loaded_C1IN decimal (18, 2)	Unchecked
            // Loaded_C2IN decimal (18, 2)	Unchecked
            //  Loaded_C3IN decimal (18, 2)	Unchecked
            //   Loaded_C4IN decimal (18, 2)	Unchecked
            Loaded_C1IN = (decimal)rdr["Loaded_C1IN"];
            Loaded_C2IN = (decimal)rdr["Loaded_C2IN"];
            Loaded_C3IN = (decimal)rdr["Loaded_C3IN"];
            Loaded_C4IN = (decimal)rdr["Loaded_C4IN"];

            //    DateTime_Transaction_END    datetime Unchecked
            //LoadedAtRMCycle int Unchecked
            DateTime_Transaction_END = (DateTime)rdr["DateTime_Transaction_END"];
            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];
        }

        //
        // READ Balances and create Table 
        //
        public void Read_SM_SWITCH_TXNs_And_Fill_Table(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_SM_SWITCH_TOTALS = new DataTable();
            Table_SM_SWITCH_TOTALS.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_SWITCH_TOTALS] "
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

                        sqlAdapt.Fill(Table_SM_SWITCH_TOTALS);

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
        public void Read__SM_SWITCH_TXNS_BySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                       + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_SWITCH_TOTALS] "
                      + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                       // cmd.Parameters.AddWithValue("@Operator", InOperator);
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
        //
        // Methods 
        // READ Balances by Selection 
        // To Get RECORD
        //
        public void Read__SM_SWITCH_TXNS_And_BySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_SWITCH_TOTALS] "
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
        // To Get Totals
        //
        public void Read__SM_SWITCH_TXNS_And_BySelectionCriteria_FOR_TOTALS(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SWITCH_OpeningBalance = 0;
            SWITCH_Dispensed = 0;
            SWITCH_Remaining = 0;
            SWITCH_Cash_Loaded = 0;
            SWITCH_Cash_Loaded_Minus_SWITCH_Remaining = 0;
            SWITCH_Deposits = 0; 

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_SWITCH_TOTALS] "
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

                            SWITCH_OpeningBalance = Current_C1IN + Current_C2IN + Current_C3IN + Current_C4IN;
                            SWITCH_Dispensed = Current_C1OUT + +Current_C2OUT + Current_C3OUT + Current_C4OUT;
                            SWITCH_Remaining = SWITCH_OpeningBalance - SWITCH_Dispensed;
                            SWITCH_Cash_Loaded = Loaded_C1IN + Loaded_C2IN + Loaded_C3IN + Loaded_C4IN;

                            SWITCH_Cash_Loaded_Minus_SWITCH_Remaining = SWITCH_Cash_Loaded - SWITCH_Remaining;

                            SWITCH_Deposits  = Current_DEP_Amt + Current_PAY_Amt;

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

        public void Read__SM_SWITCH_TXNS_And_BySelectionCriteria_FOR_TOTALS_ADDED_CASH(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SWITCH_OpeningBalance = 0;
            SWITCH_Dispensed = 0;
            SWITCH_Remaining = 0;
            SWITCH_Cash_Loaded = 0;
            SWITCH_Cash_Loaded_Minus_SWITCH_Remaining = 0;
            SWITCH_Deposits = 0;

            TotalSelected = 0;

            SqlString = "SELECT *"
                      + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_SWITCH_TOTALS] "
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
                            // READ ONLY THE LOADED 
                           // ReadRecordFields(rdr);
                            Loaded_C1IN = (decimal)rdr["Loaded_C1IN"];
                            Loaded_C2IN = (decimal)rdr["Loaded_C2IN"];
                            Loaded_C3IN = (decimal)rdr["Loaded_C3IN"];
                            Loaded_C4IN = (decimal)rdr["Loaded_C4IN"];

                            //
                            SWITCH_Cash_Loaded = Loaded_C1IN + Loaded_C2IN + Loaded_C3IN + Loaded_C4IN;


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

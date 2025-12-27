using System;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMNVRulesForMatchingClass : Logger
    {
        public RRDMNVRulesForMatchingClass() : base() { }
        //  UPDATE[dbo].[NVRulesForMatching]

        public int SeqNo;
        public string ExternalBankID;
        public string ExternalAccNo;
        public string Ccy;
        public string TransactionCode;
   
        public bool DRTxn;
        public bool CRTxn;

        public bool IsRefIdentical;
        public bool IsRefPartial;
        public int NoOfCharactersPartial;

        public bool IsIdenticalAmount;
        public bool IsToleranceAmount;
        public bool IsTolerancePercentage;
        public bool IsToleranceFixed;

        public decimal TolerancePerc;
        public decimal ToleranceAmtFrom;
        public decimal ToleranceAmtTo;

        public bool IsValueDaysIdentical; 
        public bool IsValueDaysTolerance;
        public int ValueDaysBefore;
        public int ValueDaysAfter;
        public bool IsAgregation;
        public string MatchedType;

        public string Operator;

        string SqlString; 

        // Define the data table 
        public DataTable RulesTableForExternal = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Reader Fields 
        //
        private void RulesReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            ExternalBankID = (string)rdr["ExternalBankID"];
            ExternalAccNo = (string)rdr["ExternalAccNo"];
            Ccy = (string)rdr["Ccy"];

            TransactionCode = (string)rdr["TransactionCode"];

            DRTxn = (bool)rdr["DRTxn"];
            CRTxn = (bool)rdr["CRTxn"];

            IsRefIdentical = (bool)rdr["IsRefIdentical"];
            IsRefPartial = (bool)rdr["IsRefPartial"];
            NoOfCharactersPartial = (int)rdr["NoOfCharactersPartial"];

            IsIdenticalAmount = (bool)rdr["IsIdenticalAmount"];
            IsToleranceAmount = (bool)rdr["IsToleranceAmount"];
            IsTolerancePercentage = (bool)rdr["IsTolerancePercentage"];
            IsToleranceFixed = (bool)rdr["IsToleranceFixed"];

            TolerancePerc = (decimal)rdr["TolerancePerc"];
            ToleranceAmtFrom = (decimal)rdr["ToleranceAmtFrom"];
            ToleranceAmtTo = (decimal)rdr["ToleranceAmtTo"];

            IsRefIdentical = (bool)rdr["IsRefIdentical"];
            IsRefPartial = (bool)rdr["IsRefPartial"];
            NoOfCharactersPartial = (int)rdr["NoOfCharactersPartial"];

            IsValueDaysIdentical = (bool)rdr["IsValueDaysIdentical"];
            IsValueDaysTolerance = (bool)rdr["IsValueDaysTolerance"];
            ValueDaysBefore = (int)rdr["ValueDaysBefore"];
            ValueDaysAfter = (int)rdr["ValueDaysAfter"];

            IsAgregation = (bool)rdr["IsAgregation"];

            MatchedType = (string)rdr["MatchedType"];

            Operator = (string)rdr["Operator"];
        }

        // READ Rule 
        public bool WAmtTolerance;
        public bool WValDtTolerance;
        public int WValueDaysBefore;
        public int WValueDaysAfter;
        public bool WPartialReference;
        public int WPartialRefLength; 
        public void ReadRuleForTxnCode(string InSignedId, string InOperator,
                                         string InExternalBankID, string InExternalAccNo,
                                         string InTransactionCode, string InDRCR)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WAmtTolerance = false;
            WValDtTolerance = false;
            WValueDaysBefore = 0 ;
            WValueDaysAfter = 0 ;
            WPartialReference = false;
            WPartialRefLength = 0; 

            if (InDRCR == "DR")
            {
                SqlString = "SELECT * "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND DRTxn = 1 ";
            }
            if (InDRCR == "CR")
            {
                SqlString = "SELECT *"
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND CRTxn = 1 ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);     
                        cmd.Parameters.AddWithValue("@TransactionCode", InTransactionCode);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            if (IsToleranceAmount == true)
                            {
                                WAmtTolerance = true;
                            }
                            if (IsValueDaysTolerance == true)
                            {
                                WValDtTolerance = true;
                                if (ValueDaysBefore > 0) WValueDaysBefore = ValueDaysBefore;
                                if (ValueDaysAfter > 0) WValueDaysAfter = ValueDaysAfter;
                            }
                            if (IsRefPartial == true)
                            {
                                WPartialReference = true;
                                if (NoOfCharactersPartial > 0) WPartialRefLength = NoOfCharactersPartial;
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


        // READ RULE BY SELECTION CRITERIA 

        public void ReadRuleBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0; 

            SqlString = "SELECT * "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + InSelectionCriteria; 
                               
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            if (IsValueDaysTolerance == true)
                            {
                                WValDtTolerance = true;
                                // One record no need for If statement 
                                WValueDaysBefore = ValueDaysBefore;
                                WValueDaysAfter = ValueDaysAfter;
                            }
                            else
                            {
                                WValDtTolerance = false;
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

        // READ RULE BY SELECTION CRITERIA and INSERT NEW BANK ACCOUNT 

        public void ReadRuleBySelectionCriteriaAndInsert
            (string InSelectionCriteria, string InNewExternalBank, string InNewExternalAcc, string InCcy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT * "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            ExternalBankID = InNewExternalBank;
                            ExternalAccNo = InNewExternalAcc;
                            Ccy = InCcy;

                            InsertRule(ExternalBankID); 


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
        // READ DISTICT BANK AND ACCOUNT AND FILL ARRAY 
        //

        public ArrayList GetRulesListGroups(string InOperator)
        {
            ArrayList RulesList = new ArrayList();
         
            //  CitNosList.Add("Own");
            string ArrayMember; 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT DISTINCT [ExternalBankID],[ExternalAccNo] "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "; 
                              
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

                            // Read Rule Details 

                            ExternalBankID = (string)rdr["ExternalBankID"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];
                            ArrayMember = "Bank And Acc =," + ExternalBankID + "," + ExternalAccNo;
                            RulesList.Add(ArrayMember);
                           

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

            return RulesList;
        }


        // READ RULE BY AMT TOLERANCE
        public void ReadRuleForTxnCodeAndAmtRange(string InSignedId, string InOperator,
                                         string InExternalBankID, string InExternalAccNo,
                                         string InTransactionCode, string InDRCR, decimal InAbsAmt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InDRCR == "DR")
            {
                SqlString = "SELECT * "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND DRTxn = 1 "
                                 + " AND (@DifAmt > ToleranceAmtFrom AND @DifAmt <= ToleranceAmtTo)  ";
            }
            if (InDRCR == "CR")
            {
                SqlString = "SELECT *"
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND CRTxn = 1 "
                                 + " AND (@DifAmt > ToleranceAmtFrom AND @DifAmt <= ToleranceAmtTo)  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
                        cmd.Parameters.AddWithValue("@TransactionCode", InTransactionCode);
                        cmd.Parameters.AddWithValue("@DifAmt", InAbsAmt);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            if (IsValueDaysTolerance == true)
                            {
                                WValDtTolerance = true;
                                // One record no need for If statement 
                                WValueDaysBefore = ValueDaysBefore;
                                WValueDaysAfter = ValueDaysAfter;
                            }
                            else
                            {
                                WValDtTolerance = false;
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

        // READ RULE BY Val Date RANGE
        string SelectionCriteriaVal;
        public void ReadRuleForTxnCodeAndValueDateDiff(string InSignedId, string InOperator,
                                         string InExternalBankID, string InExternalAccNo,
                                         string InTransactionCode, string InDRCR, int AbsDiffInValDays, int InDiffInValDays)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InDiffInValDays > 0)
            {
                SelectionCriteriaVal = " AND @DiffValDays <= ValueDaysAfter ";
            }
            if (InDiffInValDays < 0)
            {
                SelectionCriteriaVal = " AND @DiffValDays <= ValueDaysBefore ";
            }

            if (InDRCR == "DR")
            {

                SqlString = "SELECT * "
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND DRTxn = 1 "
                                 + SelectionCriteriaVal
                                 ;
            }
            if (InDRCR == "CR")
            {
                SqlString = "SELECT *"
                                 + " FROM [ATMS].[dbo].[NVRulesForMatching] "
                                 + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                                 + " AND TransactionCode = @TransactionCode AND CRTxn = 1 "
                                 + SelectionCriteriaVal
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
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
                        cmd.Parameters.AddWithValue("@TransactionCode", InTransactionCode);
                        cmd.Parameters.AddWithValue("@DiffValDays", AbsDiffInValDays);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            if (IsValueDaysTolerance == true)
                            {
                                WValDtTolerance = true;
                                // One record no need for If statement 
                                WValueDaysBefore = ValueDaysBefore;
                                WValueDaysAfter = ValueDaysAfter;
                            }
                            else
                            {
                                WValDtTolerance = false;
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
        // READ ReadRulesTable By External Bank And Ccy
        //

        public void ReadRulesTableByExternalBankAndCcy(string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //InMode = 1 ... is ITMX
            //InMode = 2 ... is Clearing Bank = central Bank

            RulesTableForExternal = new DataTable();
            RulesTableForExternal.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RulesTableForExternal.Columns.Add("BankId", typeof(string));
            RulesTableForExternal.Columns.Add("ShortName", typeof(string));
            RulesTableForExternal.Columns.Add("Full Name", typeof(string));
            RulesTableForExternal.Columns.Add("Country", typeof(string));
            RulesTableForExternal.Columns.Add("DateInRRDM", typeof(string));

            if (InMode ==2)
            {
                RulesTableForExternal.Columns.Add("Ccy", typeof(string));
                RulesTableForExternal.Columns.Add("SettlementAcc", typeof(string));
                RulesTableForExternal.Columns.Add("Settl_Clearing", typeof(string));
            }

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[NVRulesForMatching] "
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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Rule Details 

                            RulesReaderFields(rdr);

                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = RulesTableForExternal.NewRow();

                            RowSelected["BankId"] = "";
                            RowSelected["ShortName"] = "";
                            RowSelected["Full Name"] = "";
                            RowSelected["Country"] = "";
                            RowSelected["DateInRRDM"] = "";
                                         
                            
                            // ADD ROW
                            RulesTableForExternal.Rows.Add(RowSelected);

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
// READ RULES BY BANK AND ACCNO
        public DataTable TableRulesByExternalAccount = new DataTable();
      
        public void ReadRulesBtExternalBankAndAccountNo(string InOperator, string InSignedId, 
                                              string InExternalBankID, string InExternalAccNo, string InSelectionCriteria )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableRulesByExternalAccount = new DataTable();
            TableRulesByExternalAccount.Clear();

            TotalSelected = 0;
           
                    
                SqlString =

                    " SELECT * FROM [ATMS].[dbo].[NVRulesForMatching] "
                    + InSelectionCriteria
                    + " AND ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo "
                    ;
            
            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    { 

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableRulesByExternalAccount);

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
        //
        // Insert NEW BANK & acc
        //
        public int InsertRule(string InExternalBankID)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVRulesForMatching]"
                    + " ([ExternalBankID] "
                    + " ,[ExternalAccNo] "
                    + ", [Ccy] "
                    + " ,[TransactionCode] "
                    + " ,[DRTxn] "
                    + "  ,[CRTxn] "
                    + " ,[IsRefIdentical] "
                    + "  ,[IsRefPartial] "
                    + "  ,[NoOfCharactersPartial] "
                    + " ,[IsIdenticalAmount] "
                    + "  ,[IsToleranceAmount] "
                    + " ,[IsTolerancePercentage] "
                    + " ,[IsToleranceFixed] "
                    + " ,[TolerancePerc] "
                    + " ,[ToleranceAmtFrom] "
                    + " ,[ToleranceAmtTo] "
                    + "  ,[IsValueDaysIdentical] "
                    + "  ,[IsValueDaysTolerance] "
                    + "  ,[ValueDaysBefore] "
                    + "   ,[ValueDaysAfter] "
                    + "   ,[IsAgregation] "
                    + " ,[MatchedType] "
                    + "    ,[Operator]) "
                + " VALUES "
               + " (  @ExternalBankID "
                    + " , @ExternalAccNo "
                    + ", @Ccy "
                    + " , @TransactionCode "
                    + " , @DRTxn "
                    + "  , @CRTxn "
                        + " , @IsRefIdentical "
                    + "  , @IsRefPartial "
                    + "  , @NoOfCharactersPartial "
                    + " , @IsIdenticalAmount "
                    + "  , @IsToleranceAmount "
                    + " , @IsTolerancePercentage "
                    + " , @IsToleranceFixed "
                    + " , @TolerancePerc "
                    + " , @ToleranceAmtFrom "
                    + " , @ToleranceAmtTo "
                    + " , @IsValueDaysIdentical "
                    + "  , @IsValueDaysTolerance "
                    + "  , @ValueDaysBefore "
                    + "   , @ValueDaysAfter "
                    + "   , @IsAgregation "
                    + " , @MatchedType "
                    + "    , @Operator) "
                  + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExternalBankID", ExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@TransactionCode", TransactionCode);

                        cmd.Parameters.AddWithValue("@DRTxn", DRTxn);
                        cmd.Parameters.AddWithValue("@CRTxn", CRTxn);

                        cmd.Parameters.AddWithValue("@IsRefIdentical", IsRefIdentical);
                        cmd.Parameters.AddWithValue("@IsRefPartial", IsRefPartial);
                        cmd.Parameters.AddWithValue("@NoOfCharactersPartial", NoOfCharactersPartial);

                        cmd.Parameters.AddWithValue("@IsIdenticalAmount", IsIdenticalAmount);
                        cmd.Parameters.AddWithValue("@IsToleranceAmount", IsToleranceAmount);
                        cmd.Parameters.AddWithValue("@IsTolerancePercentage", IsTolerancePercentage);
                        cmd.Parameters.AddWithValue("@IsToleranceFixed", IsToleranceFixed);

                        cmd.Parameters.AddWithValue("@TolerancePerc", TolerancePerc);
                        cmd.Parameters.AddWithValue("@ToleranceAmtFrom", ToleranceAmtFrom);
                        cmd.Parameters.AddWithValue("@ToleranceAmtTo", ToleranceAmtTo);
                        
                        cmd.Parameters.AddWithValue("@IsValueDaysIdentical", IsValueDaysIdentical);
                        cmd.Parameters.AddWithValue("@IsValueDaysTolerance", IsValueDaysTolerance);
                        cmd.Parameters.AddWithValue("@ValueDaysBefore", ValueDaysBefore);
                        cmd.Parameters.AddWithValue("@ValueDaysAfter", ValueDaysAfter);

                        cmd.Parameters.AddWithValue("@IsAgregation", IsAgregation);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        SeqNo = (int)cmd.ExecuteScalar();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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


        // UPDATE Rule
        // 
        public void UpdateRule(int InSeqNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVRulesForMatching] SET "

                        + " [ExternalBankID] = @ExternalBankID "
                        + " ,[ExternalAccNo] = @ExternalAccNo "
                        + " ,[Ccy] = @Ccy "
                        + " ,[TransactionCode] = @TransactionCode "
                        + " ,[DRTxn] = @DRTxn "
                        + " ,[CRTxn] = @CRTxn "
                         + " ,[IsRefIdentical] = @IsRefIdentical "
                        + " ,[IsRefPartial] = @IsRefPartial "
                        + " ,[NoOfCharactersPartial] = @NoOfCharactersPartial "
                        + " ,[IsIdenticalAmount] = @IsIdenticalAmount "
                        + " ,[IsToleranceAmount] = @IsToleranceAmount "
                        + " ,[IsTolerancePercentage] = @IsTolerancePercentage "
                        + " ,[IsToleranceFixed] = @IsToleranceFixed "
                        + " ,[TolerancePerc] = @TolerancePerc "
                        + " ,[ToleranceAmtFrom] = @ToleranceAmtFrom "
                        + " ,[ToleranceAmtTo] = @ToleranceAmtTo "
                         + " ,[IsValueDaysIdentical] = @IsValueDaysIdentical "
                        + " ,[IsValueDaysTolerance] = @IsValueDaysTolerance "
                        + " ,[ValueDaysBefore] = @ValueDaysBefore "
                        + " ,[ValueDaysAfter] = @ValueDaysAfter "
                        + " ,[IsAgregation] = @IsAgregation "
                        + " ,[MatchedType] = @MatchedType "
                        + " ,[Operator] = @Operator "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ExternalBankID", ExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@TransactionCode", TransactionCode);

                        cmd.Parameters.AddWithValue("@DRTxn", DRTxn);
                        cmd.Parameters.AddWithValue("@CRTxn", CRTxn);

                        cmd.Parameters.AddWithValue("@IsRefIdentical", IsRefIdentical);
                        cmd.Parameters.AddWithValue("@IsRefPartial", IsRefPartial);
                        cmd.Parameters.AddWithValue("@NoOfCharactersPartial", NoOfCharactersPartial);

                        cmd.Parameters.AddWithValue("@IsIdenticalAmount", IsIdenticalAmount);
                        cmd.Parameters.AddWithValue("@IsToleranceAmount", IsToleranceAmount);
                        cmd.Parameters.AddWithValue("@IsTolerancePercentage", IsTolerancePercentage);
                        cmd.Parameters.AddWithValue("@IsToleranceFixed", IsToleranceFixed);

                        cmd.Parameters.AddWithValue("@TolerancePerc", TolerancePerc);
                        cmd.Parameters.AddWithValue("@ToleranceAmtFrom", ToleranceAmtFrom);
                        cmd.Parameters.AddWithValue("@ToleranceAmtTo", ToleranceAmtTo);

                        cmd.Parameters.AddWithValue("@IsValueDaysIdentical", IsValueDaysIdentical);
                        cmd.Parameters.AddWithValue("@IsValueDaysTolerance", IsValueDaysTolerance);
                        cmd.Parameters.AddWithValue("@ValueDaysBefore", ValueDaysBefore);
                        cmd.Parameters.AddWithValue("@ValueDaysAfter", ValueDaysAfter);

                        cmd.Parameters.AddWithValue("@IsAgregation", IsAgregation);
                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
// DELETE Rule
        //
        public void DeleteRuleEntry(int InSeqNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVRulesForMatching] "
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

        //
        // DELETE Rule for Bank / Acc
        //
        public void DeleteRuleEntriesforBankAcc(string InExternalBankID, string InExternalAccNo)
        {
           
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVRulesForMatching] "
                            + " WHERE ExternalBankID = @ExternalBankID AND ExternalAccNo = @ExternalAccNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ExternalBankID", InExternalBankID);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", InExternalAccNo);
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
      

    }
}



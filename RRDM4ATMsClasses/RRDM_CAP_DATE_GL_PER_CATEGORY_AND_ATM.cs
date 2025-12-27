using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM : Logger
    {
        public RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM() : base() { }
        // 
        // 1) _CAP_DATE_GL_PER_CATEGORY_AND_ATM

        public int SeqNo;

        public DateTime CAP_DATE;
        public string MatchingCateg;
        public string AtmNo;
        public string OriginFile; 
        public int CreatedAtRMCycle;
        public int UpdatedAtRMCycle;
        public DateTime FirstDtTm; 

        public DateTime LastDtTm;

        public decimal TurnOverDebit; 
        public decimal TurnOverCredit; 
        public int TransDR; 
        public int TransCR;
       
        public decimal UnMatchedDebit;
        public decimal UnMatchedCredit;
        public int UnMatchedTransDR;
        public int UnMatchedTransCR;

        public int ReversalsTxns; 
        public decimal ReversalsAmt;

        public string GL_Number; 
        public decimal GL_Openning;

        public decimal GL_Closing;
        public DateTime DateCreated; 
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable Table_CAP_DATE;

        public DataTable Table_Settlement_DATE_Identity;

        public DataTable Table_Settlement_DATE_BIN;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        string SqlString; 


        // Fields
        private void Read_CAP_DATEFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CAP_DATE = (DateTime)rdr["CAP_DATE"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            AtmNo = (string)rdr["AtmNo"];

            OriginFile = (string)rdr["OriginFile"];

            CreatedAtRMCycle = (int)rdr["CreatedAtRMCycle"];

            UpdatedAtRMCycle = (int)rdr["UpdatedAtRMCycle"];

            FirstDtTm = (DateTime)rdr["FirstDtTm"];

            LastDtTm = (DateTime)rdr["LastDtTm"];

            TurnOverDebit = (decimal)rdr["TurnOverDebit"];
            TurnOverCredit = (decimal)rdr["TurnOverCredit"];
            TransDR = (int)rdr["TransDR"];
            TransCR = (int)rdr["TransCR"];

            UnMatchedDebit = (decimal)rdr["UnMatchedDebit"];
            UnMatchedCredit = (decimal)rdr["UnMatchedCredit"];
            UnMatchedTransDR = (int)rdr["UnMatchedTransDR"];
            UnMatchedTransCR = (int)rdr["UnMatchedTransCR"];

            ReversalsTxns = (int)rdr["ReversalsTxns"];
            ReversalsAmt = (decimal)rdr["ReversalsAmt"];

            GL_Number = (string)rdr["GL_Number"];
            GL_Openning = (decimal)rdr["GL_Openning"];
           
            GL_Closing = (decimal)rdr["GL_Closing"];
            DateCreated = (DateTime)rdr["DateCreated"];
            Operator = (string)rdr["Operator"];
        }
        //
        // READ by SeqNo
        //
        public void Read_CAP_DATEForSeqNo(int InSeqNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

        if (InMode == 1)
            {
                SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY]"
                                         + " WHERE SeqNo = @SeqNo "
                                          + " ";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs]"
                                         + " WHERE SeqNo = @SeqNo "
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
                       
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            Read_CAP_DATEFields(rdr);

                         
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
        // READ by Matching and CAP 
        //
        public void Read_CAP_DATEFor_Category_CapDate(string InMatchingCateg, DateTime InCAP_DATE, string InOriginFile)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

                SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY]"
                                         + " WHERE MatchingCateg = @MatchingCateg AND CAP_DATE = @CAP_DATE "
                                         +" AND OriginFile = @OriginFile "
                                          + " ";
           

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        cmd.Parameters.AddWithValue("@OriginFile", InOriginFile);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            Read_CAP_DATEFields(rdr);


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
        // READ by Matching and CAP 
        //
        public void Read_CAP_DATEFor_ATM_CapDate(string InAtmNo, DateTime InCAP_DATE)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT * "
                                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs]"
                                     + " WHERE AtmNo = @AtmNo AND CAP_DATE = @CAP_DATE "
                                      + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            Read_CAP_DATEFields(rdr);


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
        // READ by Selection Criteria 
        //
        public void Read_CAP_DATEForSelectionCriteria(string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY_AND_ATM] "
                        + InSelectionCriteria
                         + " ";
            }
            if (InMode == 2)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs] "
                        + InSelectionCriteria
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

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            Read_CAP_DATEFields(rdr);

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
        // READ and Fill Table 
        //

        public void ReadCIT_G4S_Repl_EntriesToFillDataTable(string InOperator, string InSignedId,
                                                           string InSelectionCriteria,DateTime InCap_Date ,int InRMCycle,int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_CAP_DATE = new DataTable();
            Table_CAP_DATE.Clear();

            if (InMode == 6)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] "
                        + InSelectionCriteria
                        + " ORDER BY MatchingCateg, CAP_DATE"
                        ;
            }
            if (InMode == 8)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] "
                        + InSelectionCriteria;
            }
            if (InMode == 7)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs] "
                        + InSelectionCriteria;
            }
            if (InMode == 9)
            {
                SqlString = "SELECT * "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs] "
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
                        if (InMode == 6 || InMode == 7)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCap_Date);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@UpdatedAtRMCycle", InRMCycle);
                        }
                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4SFrom", InDateFrom);
                       
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_CAP_DATE);

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
        // READ and Fill Table Distict GL 
        //
        public DataTable Table_GL_Distinct;
        //
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_Distict_GL(string InOperator, string InSignedId)                                                
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_GL_Distinct = new DataTable();
            Table_GL_Distinct.Clear();

                SqlString = " SELECT DISTINCT GL_Number, OriginFile , MatchingCateg As MatchingCategories "
                        + " FROM[RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] "
                        + " Where AtmNo = '' "
                        ;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //if (InMode == 6 || InMode == 7)
                        //{
                        //    sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCap_Date);
                        //    sqlAdapt.SelectCommand.Parameters.AddWithValue("@UpdatedAtRMCycle", InRMCycle);
                        //}
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplDateG4SFrom", InDateFrom);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_GL_Distinct);

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
        public DataTable Table_GL_Per_Account;

        public void ReadCIT_G4S_Repl_EntriesToFillDataTable_Per_GL(string InOperator, string InSignedId
                             , int InRMCycle, string InGL_Number)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Table_GL_Per_Account = new DataTable();
            Table_GL_Per_Account.Clear();

            SqlString = " SELECT SeqNo, CAP_DATE As SetlementDt,  CreatedAtRMCycle As Cycle "
                         +",TransDr As Matched_Dr, TurnOverDebit As Matched_Amt "
                    + " , UnMatchedTransDR as UnMatched, UnMatchedDebit As UnMatched_Amt, GL_Number, MatchingCateg As Categories "
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] "
                    + "  Where CreatedAtRMCycle = @CreatedAtRMCycle And GL_Number = @GL_Number "
                    ;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@CreatedAtRMCycle", InRMCycle);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@GL_Number", InGL_Number);
                    
                    sqlAdapt.Fill(Table_GL_Per_Account);

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


        // Insert CAP_DATE
        //
        public void Insert_CAP_DATE(int InMode, string InOriginFile)
        {

            //CAP_DATE = (DateTime)rdr["CAP_DATE"];
            //MatchingCateg = (string)rdr["MatchingCateg"];
            //AtmNo = (string)rdr["AtmNo"];
            //FirstDtTm = (DateTime)rdr["FirstDtTm"];
            string TableId = ""; 
            if (InMode == 1)
            {
                TableId = " [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] "; 
            }
            if (InMode == 2)
            {
                TableId = " [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs] ";
            }

            ErrorFound = false;
            ErrorOutput = "";
          //  Cgl.GL_Number = Mc.GlAccount;
            string cmdinsert = "INSERT INTO " + TableId
                    + "([CAP_DATE] "
                    + " , [MatchingCateg] "
                    + " , [AtmNo] "
                     + " , [OriginFile] "
                     + " , [CreatedAtRMCycle] "
                    + " , [FirstDtTm] "
                    + " , [GL_Number] "
                     + " , [DateCreated] "
                    + " , [Operator] )"
                    + " VALUES ( @CAP_DATE"
                    + ", @MatchingCateg "
                    + ", @AtmNo"
                     + ", @OriginFile"
                     + ", @CreatedAtRMCycle"
                    + ", @FirstDtTm"
                    + ", @GL_Number"
                      + ", @DateCreated"
                    + ", @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                         cmd.Parameters.AddWithValue("@CAP_DATE", CAP_DATE);
                         cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);          
                         cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                         cmd.Parameters.AddWithValue("@OriginFile", InOriginFile);

                         cmd.Parameters.AddWithValue("@CreatedAtRMCycle", CreatedAtRMCycle);

                         cmd.Parameters.AddWithValue("@FirstDtTm", FirstDtTm);
                         cmd.Parameters.AddWithValue("@GL_Number", GL_Number);
                         cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
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

        // UPDATE 
        // 
        
        public int Update_CAP_DATE(DateTime InCAP_DATE, string inMatchingCateg, string InAtmNo, int InCreatedAtRMCycle, int InMode)
        {          
            ErrorFound = false;
            ErrorOutput = "";

            string TableId ="";
            if (InMode == 1)
            {
                TableId = " [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY] ";
            }
            if (InMode == 2)
            {
                TableId = " [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_ATMs] ";
            }


            int Count = 0; 
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {


                    //ReversalsTxns = (int)rdr["ReversalsTxns"];
                    //ReversalsAmt = (decimal)rdr["ReversalsAmt"];
                    //UpdatedAtRMCycle
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + TableId 
                            + " SET "
                            + " LastDtTm = @LastDtTm "
                            + " ,TurnOverDebit = @TurnOverDebit "
                            + " ,UpdatedAtRMCycle = @UpdatedAtRMCycle "
                            + " ,TurnOverCredit = @TurnOverCredit "
                            + " ,TransDR = @TransDR "
                            + " ,TransCR = @TransCR "
                             + " ,ReversalsTxns = @ReversalsTxns "
                            + " ,ReversalsAmt = @ReversalsAmt "
                            + " ,UnMatchedDebit = @UnMatchedDebit "
                            + " ,UnMatchedCredit = @UnMatchedCredit "
                            + " ,UnMatchedTransDR = @UnMatchedTransDR "
                            + " ,UnMatchedTransCR = @UnMatchedTransCR "
                            + " WHERE CAP_DATE = @CAP_DATE AND MatchingCateg = @MatchingCateg "
                            + "AND AtmNo = @AtmNo AND CreatedAtRMCycle = @CreatedAtRMCycle ", conn))
                    {
                 
                        cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", inMatchingCateg);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CreatedAtRMCycle", InCreatedAtRMCycle);

                        cmd.Parameters.AddWithValue("@LastDtTm", LastDtTm);
                        cmd.Parameters.AddWithValue("@UpdatedAtRMCycle", UpdatedAtRMCycle);
                        cmd.Parameters.AddWithValue("@TurnOverDebit", TurnOverDebit);
                        cmd.Parameters.AddWithValue("@TurnOverCredit", TurnOverCredit);
                        cmd.Parameters.AddWithValue("@TransDR", TransDR);
                        cmd.Parameters.AddWithValue("@TransCR", TransCR);

                        cmd.Parameters.AddWithValue("@ReversalsTxns", ReversalsTxns);
                        cmd.Parameters.AddWithValue("@ReversalsAmt", ReversalsAmt);

                        cmd.Parameters.AddWithValue("@UnMatchedDebit", UnMatchedDebit);
                        cmd.Parameters.AddWithValue("@UnMatchedCredit", UnMatchedCredit);
                        cmd.Parameters.AddWithValue("@UnMatchedTransDR", UnMatchedTransDR);
                        cmd.Parameters.AddWithValue("@UnMatchedTransCR", UnMatchedTransCR);

                        Count = cmd.ExecuteNonQuery();
                       
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return Count; 
        }
        // SETTLEMENT SECTION
        // Insert Settlement
        //
        public DateTime Settlement_DATE;
        // public string MatchingCateg; 
        public string Card_BIN;
        public int TransType;
        public string Ccy;
        public int CB_TXNs;
        public decimal CB_TXNs_AMT;
        public decimal EQUIV; 

        public int CB_UnMatched_TXNS_1xx;
        public decimal CB_UnMatched_TXNS_1xx_Amt; 
            
        public int Other_Unmatched_x11;
        public decimal Other_Unmatched_x11_Amt; 
                   
        public int RMCycle;
        public string FileId;
        public string CategoryGroup;
        public string W_Identity; 
        public string Comment;
        // public DateTime DateCreated;
        //
        // Fields
        private void Read_Sellement_Fields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            Settlement_DATE = (DateTime)rdr["Settlement_DATE"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            Card_BIN = (string)rdr["Card_BIN"];

            TransType = (int)rdr["TransType"];

            Ccy = (string)rdr["Ccy"];

            CB_TXNs = (int)rdr["CB_TXNs"];

            CB_TXNs_AMT = (decimal)rdr["CB_TXNs_AMT"];

            CB_UnMatched_TXNS_1xx = (int)rdr["CB_UnMatched_TXNS_1xx"];

            CB_UnMatched_TXNS_1xx_Amt = (decimal)rdr["CB_UnMatched_TXNS_1xx_Amt"];

            Other_Unmatched_x11 = (int)rdr["Other_Unmatched_x11"];

            Other_Unmatched_x11_Amt = (decimal)rdr["Other_Unmatched_x11_Amt"];

            RMCycle = (int)rdr["RMCycle"];

            FileId = (string)rdr["FileId"];

            CategoryGroup = (string)rdr["CategoryGroup"];

            W_Identity = (string)rdr["W_Identity"];

            Comment = (string)rdr["Comment"];
            DateCreated = (DateTime)rdr["DateCreated"];
        
            Operator = (string)rdr["Operator"];
        }

        //
        // READ and Fill Table 
        //

        public void ReadSettlementToFillTable_By_Identity(string InOperator, string InSignedId,
                                                            int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            Table_Settlement_DATE_Identity = new DataTable();
            Table_Settlement_DATE_Identity.Clear();


            SqlString =
                    " SELECT [Settlement_DATE], [W_Identity], Ccy, SUM(CB_TXNs) As CB_TXNs, SUM(CB_TXNs_AMT) As CB_TXNs_AMT "
                    + ", SUM(EQUIV) As EQUIV "
          + "     ,SUM(CB_UnMatched_TXNS_1xx) as CB_UnMatched_TXNS_1xx,SUM(CB_UnMatched_TXNS_1xx_Amt) as CB_UnMatched_TXNS_1xx_Amt "
          + "	 , SUM(Other_Unmatched_x11) AS Other_Unmatched_x11, SUM(Other_Unmatched_x11_Amt) As Other_Unmatched_x11_Amt "

          + "  FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] "

          + "  WHERE [RMCycle] = @RMCycle "

          + "  Group by [Settlement_DATE], [W_Identity], Ccy "
          + "  Order by [Settlement_DATE], [W_Identity]  DESC " ; 


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_Settlement_DATE_Identity);

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
        // READ and Fill Table 
        //

        public void ReadSettlementToFillTable_By_BIN(string InOperator, string InSignedId,
                                                           DateTime InSettlement_DATE, string InW_Identity, int InRMCycle, int InMode, string InCcy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            Table_Settlement_DATE_BIN = new DataTable();
            Table_Settlement_DATE_BIN.Clear();


            SqlString = 
                    " SELECT * "
            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] "
            + " WHERE [Settlement_DATE] = @Settlement_DATE AND [W_Identity] = @W_Identity AND RMCycle = @RMCycle and Ccy = @Ccy "
            + " Order By MatchingCateg, Card_BIN "
                    ;


            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@W_Identity", InW_Identity);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Ccy", InCcy);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(Table_Settlement_DATE_BIN);

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

        // READ by 
        //
        public void Read_SettlementDate_BySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            SqlString = "SELECT * FROM "
                                     + TableId
                                     + " WHERE SeqNo = @SeqNo "
                                      + " ";


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
                            Read_Sellement_Fields(rdr);
                           
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

        // READ by RM cycle no 
        //
        public void Read_SettlementDate_ByCycleNo(int InRMCycle, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            SqlString = "SELECT * FROM "
                                     + TableId
                                     + " WHERE RMCycle = @RMCycle AND MatchingCateg = @MatchingCateg"
                                      + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            Read_Sellement_Fields(rdr);

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
        // READ by Settlement 
        //
        public void Read_SettlementDate_For_Category_BIN(DateTime InSettlement_DATE, string InMatchingCateg, 
                                                                           string InCard_BIN, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            SqlString = "SELECT * FROM "
                                     + TableId
                                     + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                                     + " AND Card_BIN = @Card_BIN AND RMCycle=@RMCycle "
                                      + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_BIN", InCard_BIN);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            Read_Sellement_Fields(rdr);
                           
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

        // READ by Settlement by Ccy 
        //
        public void Read_SettlementDate_For_Category_Ccy(DateTime InSettlement_DATE, string InMatchingCateg,
                                                                           string InCcy, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            SqlString = "SELECT * FROM "
                                     + TableId
                                     + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                                     + " AND Ccy = @Ccy AND RMCycle=@RMCycle "
                                      + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Ccy", InCcy);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            Read_Sellement_Fields(rdr);
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

        // READ by BIN and transaction Type
        //
        public void Read_SettlementDate_For_Category_BIN_TransType(DateTime InSettlement_DATE, string InMatchingCateg,
                                                                           string InCard_BIN, int InTransType, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //Settlement_DATE = (DateTime)rdr["Settlement_DATE"];
            //MatchingCateg = (string)rdr["MatchingCateg"];
            //Card_BIN = (string)rdr["Card_BIN"];

            //TransType = (int)rdr["TransType"];


            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";

            SqlString = "SELECT * FROM "
                                     + TableId
                                     + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                                     + " AND Card_BIN = @Card_BIN AND TransType = @TransType AND RMCycle=@RMCycle "
                                      + " ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_BIN", InCard_BIN);
                        cmd.Parameters.AddWithValue("@TransType", InTransType);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            Read_Sellement_Fields(rdr);
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

        // Insert Record
        public void Insert_Settlement_DATE_For_Non_ATMS(string InOperator, int InRMCycle)
        {
          
            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";
           
            ErrorFound = false;
            ErrorOutput = "";
     
        string cmdinsert = "INSERT INTO " + TableId
                    + "([Settlement_DATE] "
                    + " , [MatchingCateg] "
                  
                    + " , [Card_BIN] "
                      + ", [TransType]"
                       + ", [Ccy]"
                     + " , [CB_TXNs] "
                     + " , [CB_TXNs_AMT] "

                      + " , [EQUIV] "

                    + " , [CB_UnMatched_TXNS_1xx] "
                    + " , [CB_UnMatched_TXNS_1xx_Amt] "
                     + " , [Other_Unmatched_x11] "
                     + ", [Other_Unmatched_x11_Amt]  "
                     
                            + " ,[RMCycle] "
                            + " ,[FileId] "
                             + " ,[CategoryGroup] "
                             + " ,[W_Identity] "
                            + " ,[Comment] "
                            + " ,[DateCreated] "
                             + " ,[Operator] )"

                    + " VALUES ( @Settlement_DATE"
                   + " , @MatchingCateg "
              
                    + " , @Card_BIN "
                         + ", @TransType"
                            + ", @Ccy"
                     + " , @CB_TXNs "
                     + " , @CB_TXNs_AMT "

                        + " , @EQUIV "

                    + " , @CB_UnMatched_TXNS_1xx "
                    + " , @CB_UnMatched_TXNS_1xx_Amt "
                     + " , @Other_Unmatched_x11 "
                     + ", @Other_Unmatched_x11_Amt  "

                            + " ,@RMCycle "
                            + " ,@FileId "
                             + " ,@CategoryGroup "
                              + " ,@W_Identity "
                            + " ,@Comment "
                            + " ,@DateCreated "
                             + " ,@Operator ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                
                        cmd.Parameters.AddWithValue("@Settlement_DATE", Settlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_BIN", Card_BIN);
                   
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);

                        cmd.Parameters.AddWithValue("@CB_TXNs", CB_TXNs);
                        cmd.Parameters.AddWithValue("@CB_TXNs_AMT", CB_TXNs_AMT);

                        cmd.Parameters.AddWithValue("@EQUIV", EQUIV);

                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx", CB_UnMatched_TXNS_1xx);
                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx_Amt", CB_UnMatched_TXNS_1xx_Amt);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11", Other_Unmatched_x11);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11_Amt", Other_Unmatched_x11_Amt);

                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@CategoryGroup", CategoryGroup);
                        cmd.Parameters.AddWithValue("@W_Identity", W_Identity);

                        cmd.Parameters.AddWithValue("@Comment", Comment);
                        cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now );
                        
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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
        // UPDATE 
        // 

        public int Update_Settlement_DATE_Category_BIN(DateTime InSettlement_DATE, string InMatchingCateg, string InCard_BIN, int InRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";
            
            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";
            // Read Fields
           
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + TableId
                            + " SET "
                            + " CB_UnMatched_TXNS_1xx = @CB_UnMatched_TXNS_1xx "
                            + " ,CB_UnMatched_TXNS_1xx_Amt = @CB_UnMatched_TXNS_1xx_Amt "
                            + " ,Other_Unmatched_x11 = @Other_Unmatched_x11 "
                            + " ,Other_Unmatched_x11_Amt = @Other_Unmatched_x11_Amt "
                            + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                            + "AND Card_BIN = @Card_BIN AND RMCycle = @RMCycle", conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_BIN", InCard_BIN);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx", CB_UnMatched_TXNS_1xx);
                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx_Amt", CB_UnMatched_TXNS_1xx_Amt);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11", Other_Unmatched_x11);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11_Amt", Other_Unmatched_x11_Amt);

                        Count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return Count;
        }

        //
        // UPDATE by Ccy
        // 

        public int Update_Settlement_DATE_Category_Ccy(DateTime InSettlement_DATE, string InMatchingCateg, 
                                                                            string InCcy, int InRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";
            // Read Fields

            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + TableId
                            + " SET "
                            + " CB_UnMatched_TXNS_1xx = @CB_UnMatched_TXNS_1xx "
                            + " ,CB_UnMatched_TXNS_1xx_Amt = @CB_UnMatched_TXNS_1xx_Amt "
                            + " ,Other_Unmatched_x11 = @Other_Unmatched_x11 "
                            + " ,Other_Unmatched_x11_Amt = @Other_Unmatched_x11_Amt "
                            + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                            + "AND Ccy = @Ccy AND RMCycle = @RMCycle", conn))
                    {

                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Ccy", InCcy);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx", CB_UnMatched_TXNS_1xx);
                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx_Amt", CB_UnMatched_TXNS_1xx_Amt);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11", Other_Unmatched_x11);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11_Amt", Other_Unmatched_x11_Amt);

                        Count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return Count;
        }

        //
        // UPDATE by BIN and Transaction Type
        // 

        public int Update_Settlement_DATE_BIN_TransType(DateTime InSettlement_DATE, string InMatchingCateg,
                                                                           string InCard_BIN, int InTransType, int InRMCycle)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string TableId = " [RRDM_Reconciliation_ITMX].[dbo].[GL_Settlement_Records_PerCategory] ";
            // Read Fields

            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + TableId
                            + " SET "
                            + " CB_UnMatched_TXNS_1xx = @CB_UnMatched_TXNS_1xx "
                            + " ,CB_UnMatched_TXNS_1xx_Amt = @CB_UnMatched_TXNS_1xx_Amt "
                            + " ,Other_Unmatched_x11 = @Other_Unmatched_x11 "
                            + " ,Other_Unmatched_x11_Amt = @Other_Unmatched_x11_Amt "
                            + " WHERE Settlement_DATE = @Settlement_DATE AND MatchingCateg = @MatchingCateg "
                            + "AND Card_BIN = @Card_BIN AND TransType = @TransType AND RMCycle = @RMCycle", conn))
                    {

                        cmd.Parameters.AddWithValue("@Settlement_DATE", InSettlement_DATE);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_BIN", InCard_BIN);
                        cmd.Parameters.AddWithValue("@TransType", InTransType);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx", CB_UnMatched_TXNS_1xx);
                        cmd.Parameters.AddWithValue("@CB_UnMatched_TXNS_1xx_Amt", CB_UnMatched_TXNS_1xx_Amt);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11", Other_Unmatched_x11);
                        cmd.Parameters.AddWithValue("@Other_Unmatched_x11_Amt", Other_Unmatched_x11_Amt);

                        Count = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return Count;
        }

        //
        // DELETE A CAP_DATE
        //
        public void Delete_CAP_DATE(int InSeqNumber)
        {          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[CAP_DATE_GL_PER_CATEGORY_AND_ATM] "
                            + " WHERE SeqNumber =  @SeqNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNumber", InSeqNumber);

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
        
        //// Catch Details 
        //private static void CatchDetails(Exception ex)
        //{
        //    RRDMLog4Net Log = new RRDMLog4Net();

        //    StringBuilder WParameters = new StringBuilder();

        //    WParameters.Append("User : ");
        //    WParameters.Append("NotAssignYet");
        //    WParameters.Append(Environment.NewLine);

        //    WParameters.Append("ATMNo : ");
        //    WParameters.Append("NotDefinedYet");
        //    WParameters.Append(Environment.NewLine);

        //    string Logger = "RRDM4Atms";
        //    string Parameters = WParameters.ToString();

        //    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

        //    if (Environment.UserInteractive)
        //    {
        //        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
        //                                                 + " . Application will be aborted! Call controller to take care. ");
        //    }
        //}
    }
}



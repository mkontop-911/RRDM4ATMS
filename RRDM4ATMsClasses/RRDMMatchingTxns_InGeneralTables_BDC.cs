using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMMatchingTxns_InGeneralTables_BDC : Logger
    {
        public RRDMMatchingTxns_InGeneralTables_BDC() : base() { }

        public int SeqNo;

        public string OriginFileName;
        public int OriginalRecordId;
        public int LoadedAtRMCycle;
        public string MatchingCateg;
        public string Origin;
        public string TerminalType;

        public string TransTypeAtOrigin;

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
        public string RRNumber;
        public string FullTraceNo; //NEW
        public string AUTHNUM;
        public string ResponseCode;
        public string ActionType;  // Used For POS 
                                   // 0, Goes to the left if UniqueRecordId>0 = matched today
                                   // 4 for POS Settlement and UniqueRecordId>0 = matched today  
                                   // => It can go either to the right or left 
                                   // 3 for POS Settlement goes to the right 
                                   // 12: Matched = true;
                                   // 12:ToBeConfirmed = true; = to be authorised
                                   // 12:Goes to the left and right;
                                   // 17:Through the Manual Matched goes to the right for possible undo of manual;
                                   // 14:After Confirmed becomes 14 = manual matched => Master record is updated. With Manual Matched
                                   // After Authorisation Txns are marked as IsSettled = true                                   // ALERTS
                                   // 88: Are alerts of residual Internals 
                                   // If action is taken are marked as processed. 
                                   // For action create a meta exception to credit either the customer or profit and loss
        public bool Processed;
        public int ProcessedAtRMCycle;
        public string Mask;
        public bool IsSettled;

        public int UniqueRecordId; // 5*6 = 30
        public string Operator;
        public string TXNSRC;
        public string TXNDEST;
        public string Comment;
        public DateTime EXTERNAL_DATE;

        public DateTime Net_TransDate;
        public string Card_Encrypted;

        public string ACCEPTOR_ID;
        public string ACCEPTORNAME;
        public DateTime CAP_DATE;
        public DateTime SET_DATE;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable MatchingMasterDataTableATMs = new DataTable();

        public DataTable TableAgingAnalysis = new DataTable();

        public DataTable TablePOS_Settlement = new DataTable();

        public DataTable TablePOS_Settlement_Adj = new DataTable();

        public DataTable RMDataTableRight = new DataTable();

        public DataTable DataTableFileTotals = new DataTable();

        public DataTable DataTableAllFields = new DataTable();

        public DataTable DataTableAllFields_SeqNo = new DataTable();

        public DataTable DataTableSelectedFields = new DataTable();

        public DataTable MoveToPoolTable = new DataTable();


        public int TotalSelected;

        readonly string ATMSconnectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        readonly string recconConnString = AppConfig.GetConnectionString("ReconConnectionString");

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            OriginFileName = (string)rdr["OriginFileName"];

            OriginalRecordId = (int)rdr["OriginalRecordId"];

            LoadedAtRMCycle = (int)rdr["LoadedAtRMCycle"];

            MatchingCateg = (string)rdr["MatchingCateg"];
            Origin = (string)rdr["Origin"];

            TerminalType = (string)rdr["TerminalType"];

            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];

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
            RRNumber = (string)rdr["RRNumber"];
            FullTraceNo = (string)rdr["FullTraceNo"];
            AUTHNUM = (string)rdr["AUTHNUM"];

            ResponseCode = (string)rdr["ResponseCode"];
            ActionType = (string)rdr["ActionType"];

            Processed = (bool)rdr["Processed"];
            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
            Mask = (string)rdr["Mask"];

            IsSettled = (bool)rdr["IsSettled"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];
            Operator = (string)rdr["Operator"];

            TXNSRC = (string)rdr["TXNSRC"];
            TXNDEST = (string)rdr["TXNDEST"];
            Comment = (string)rdr["Comment"];

            EXTERNAL_DATE = (DateTime)rdr["EXTERNAL_DATE"];
            Net_TransDate = (DateTime)rdr["Net_TransDate"];
            Card_Encrypted = (string)rdr["Card_Encrypted"];

            ACCEPTOR_ID = (string)rdr["ACCEPTOR_ID"];
            ACCEPTORNAME = (string)rdr["ACCEPTORNAME"];
            CAP_DATE = (DateTime)rdr["CAP_DATE"];
            SET_DATE = (DateTime)rdr["SET_DATE"];

        }

        public int Count_11;
        public int Count_23;
        //
        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public void ReadTransSpecificFromSpecificTable_Primary(string InSelectionCriteria, string InTableName, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Count_11 = 0;
            Count_23 = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                 + " FROM " + InTableName
                 + InSelectionCriteria
                 + " ORDER BY SeqNo DESC ";
            }


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                Count_11 = Count_11 + 1;
                            }

                            if (TransType == 23)
                            {
                                Count_23 = Count_23 + 1;
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

        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public void ReadTransSpecificFromSpecificTable_By_SeqNo(string InTableName, int InSeqNo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                                  + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
                                  + " WHERE SeqNo=@SeqNo ";

            }
            if (In_DB_Mode == 2)
            {
                SqlString = " WITH MergedTbl AS "
        + " ( "
        + " SELECT *  "
        + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
         + " WHERE SeqNo=@SeqNo "
        + " UNION ALL  "
        + " SELECT * "
        + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
       + " WHERE SeqNo=@SeqNo "
        + " ) "
        + " SELECT * FROM MergedTbl "

         + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
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

                            ReadFieldsInTable(rdr);

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

        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public void ReadTransSpecificFromSpecificTable_By_SeqNo_2(string InTableName, int InSeqNo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * FROM "
                                  + InTableName
                                  + " WHERE SeqNo=@SeqNo ";

            }


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
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

                            ReadFieldsInTable(rdr);

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
        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public string AtmGLCash;
        public string AtmBranch;
        public string AtmName;
        public void ReadTransFrom_Bulk_Flexube(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT TOP(1) [COD_GL_ACCT] As AtmGLCash, RIGHT('000' + COD_CC_BRN, 3) As AtmBranch "
                  + " FROM[RRDM_Reconciliation_ITMX].[dbo].[BULK_Flexcube] "
                  + " WHERE TERM_ID = @TERM_ID AND ACQ_INST_ID = '601520' "
                  ;

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TERM_ID", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmGLCash = (string)rdr["AtmGLCash"];
                            AtmBranch = (string)rdr["AtmBranch"];

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

        public void ReadTransFrom_Bulk_COREBANKING_2(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            
            SqlString = "SELECT TOP(1) [COD_GL_ACCT] As AtmGLCash,"
                + " RIGHT('0000' + COD_CC_BRN, 4) As AtmBranch "
                  //  + " RIGHT(COD_GL_ACCT, 4) As AtmBranch "
                  + " FROM[RRDM_Reconciliation_ITMX].[dbo].[BULK_COREBANKING] "
                  + " WHERE TERM_ID = @TERM_ID "
                  +" AND ACQ_INST_ID = '601520' "
                  ;

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TERM_ID", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmGLCash = (string)rdr["AtmGLCash"];
                            AtmBranch = (string)rdr["AtmBranch"];

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
        public DataTable IST_STATS_IN_PRIMARY = new DataTable();
        public DataTable IST_TWIN_STATS_IN_PRIMARY = new DataTable();
        public DataTable MASTER_STATS_IN_PRIMARY = new DataTable();
        public DataTable TERMINAL_STATS_IN_PRIMARY = new DataTable();

        public DataTable Discrepancies_STATS_IN_PRIMARY = new DataTable();

        public void CREATE_EXCELS_FOR_PRIMARY_DATA_BASE(DateTime InCutOffDate, int IN_ReconcCycleNo)
        {
            // ************
            // IST 
            // ************
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            IST_STATS_IN_PRIMARY = new DataTable();
            IST_STATS_IN_PRIMARY.Clear();

            SqlString = " SELECT MatchingCateg, Net_TransDate, Count(*) as NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                    + " WHERE Net_TransDate<@FromDt "
                    + " GROUP by MatchingCateg, Net_TransDate "
                    + " Order By MatchingCateg, Net_TransDate "
                ;

            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        
                          sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InCutOffDate.AddDays(-10));
                       

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(IST_STATS_IN_PRIMARY);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();
          
            
               string  WDescription = "IST_STATS_IN_PRIMARY_Minus_10_days " + IN_ReconcCycleNo.ToString();
                string ExcelPath = "C:\\RRDM\\Working\\" + WDescription + ".xls";
          

            string WorkingDir = "C:\\RRDM\\Working\\";

            XL.ExportToExcel(IST_STATS_IN_PRIMARY, WorkingDir, ExcelPath);

            // ************
            // IST TWIN 
            // ************
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            IST_TWIN_STATS_IN_PRIMARY = new DataTable();
            IST_TWIN_STATS_IN_PRIMARY.Clear();

            SqlString = " SELECT MatchingCateg, Net_TransDate, Count(*) as NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
                    + " WHERE Net_TransDate<@FromDt "
                    + " GROUP by MatchingCateg, Net_TransDate "
                    + " Order By MatchingCateg, Net_TransDate "
                ;

            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InCutOffDate.AddDays(-10));


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(IST_TWIN_STATS_IN_PRIMARY);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            WDescription = "IST_TWIN_STATS_IN_PRIMARY_Minus_10_days " + IN_ReconcCycleNo.ToString();
            ExcelPath = "C:\\RRDM\\Working\\" + WDescription + ".xls";


            WorkingDir = "C:\\RRDM\\Working\\";

            XL.ExportToExcel(IST_TWIN_STATS_IN_PRIMARY, WorkingDir, ExcelPath);

            // ************
            // IST TERMINAL STATS
            // ************
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TERMINAL_STATS_IN_PRIMARY = new DataTable();
            TERMINAL_STATS_IN_PRIMARY.Clear();

            SqlString = " SELECT TerminalId, Count(*) as NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                    + " WHERE MatchingCateg = 'BDC201'" 
                     + " AND Net_TransDate<@FromDt "
                    + " GROUP by TerminalId "
                    + " Order By TerminalId "

                ;

            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InCutOffDate.AddDays(-10));


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(TERMINAL_STATS_IN_PRIMARY);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            WDescription = "TERMINAL_STATS_IN_PRIMARY_Minus_10_days " + IN_ReconcCycleNo.ToString(); ;
            ExcelPath = "C:\\RRDM\\Working\\" + WDescription + ".xls";


            WorkingDir = "C:\\RRDM\\Working\\";

            XL.ExportToExcel(TERMINAL_STATS_IN_PRIMARY, WorkingDir, ExcelPath);


            // ************
            // MASTER
            // ************
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MASTER_STATS_IN_PRIMARY = new DataTable();
            MASTER_STATS_IN_PRIMARY.Clear();

            SqlString = " SELECT MatchingCateg, Net_TransDate, Count(*) as NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                     + " WHERE Net_TransDate<@FromDt "
                    + " GROUP by MatchingCateg, Net_TransDate "
                    + " Order By MatchingCateg, Net_TransDate "
                ;

            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InCutOffDate.AddDays(-10));


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(MASTER_STATS_IN_PRIMARY);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            // Create the Excel from TPF table

            //RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();


            WDescription = "MASTER_STATS_IN_PRIMARY_Minus_10_days " + IN_ReconcCycleNo.ToString(); ;
            ExcelPath = "C:\\RRDM\\Working\\" + WDescription + ".xls";

            WorkingDir = "C:\\RRDM\\Working\\";

            XL.ExportToExcel(MASTER_STATS_IN_PRIMARY, WorkingDir, ExcelPath);

            // ************
            // Discrepancies
            // ************
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Discrepancies_STATS_IN_PRIMARY = new DataTable();
            Discrepancies_STATS_IN_PRIMARY.Clear();

            SqlString = " SELECT Net_TransDate, MatchingCateg, count(*) as Discrepancies "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                     + " Where Matched = 0 and Net_TransDate > @FirstDate "
                    + " GROUP by Net_TransDate, MatchingCateg "
                    + " Order By Net_TransDate, MatchingCateg "
                ;
 
            using (SqlConnection conn =
               new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FirstDate", InCutOffDate.AddDays(-10));


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(Discrepancies_STATS_IN_PRIMARY);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            // Create the Excel from TPF table

            //RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();


            WDescription = "Discrepancies_STATS_IN_PRIMARY_Last_10_Days " + IN_ReconcCycleNo.ToString();
            ExcelPath = "C:\\RRDM\\Working\\" + WDescription + ".xls";

            WorkingDir = "C:\\RRDM\\Working\\";

            XL.ExportToExcel(Discrepancies_STATS_IN_PRIMARY, WorkingDir, ExcelPath);


            MessageBox.Show("Production Of Excels Has Finished"); 

        }
        // Get both tables 
        public void ReadTransSpecificFromBothTables_By_SelectionCriteria(string InSelectionCriteria, string InTableName)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Count_11 = 0;
            Count_23 = 0;


            SqlString =
           " WITH MergedTbl AS "
           + " ( "
           + " SELECT *  "
           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " UNION ALL  "
           + " SELECT * "
           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " ) "
           + " SELECT * FROM MergedTbl "
           + " ORDER BY SeqNo DESC "
            + "  ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                Count_11 = Count_11 + 1;
                            }

                            if (TransType == 23)
                            {
                                Count_23 = Count_23 + 1;
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

        // READ The transactions based on dates for IST say 

        public void ReadTrans_Table_For_SpecificTable(string InTableId, string InSelectionCriteria,
                  DateTime InDateFrom, DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableSelectedFields = new DataTable();
            DataTableSelectedFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT [SeqNo] ,[TransType] ,[TransDescr] ,[CardNumber],Card_Encrypted,[AccNo]  "
               + ",[TransCurr],[TransAmt] ,[TransDate],[TraceNo]  ,[RRNumber] ,[TerminalId]"
               + ",[LoadedAtRMCycle], [ProcessedAtRMCycle],[MatchingCateg]  ,[AUTHNUM] ,[ResponseCode] ,[Processed]"
               + ",[Mask]  ,[TXNSRC]   ,[TXNDEST] ,[Comment]  ,[Net_TransDate]"
                + ",[ACCEPTOR_ID],[ACCEPTORNAME] ,[CAP_DATE]"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                  + InSelectionCriteria // Includes Where
                   + " AND ( Net_TransDate >= @DateFrom AND Net_TransDate <= @DateTo )"
               + " UNION ALL  "
               + " SELECT [SeqNo] ,[TransType] ,[TransDescr] ,[CardNumber],Card_Encrypted,[AccNo]  "
               + ",[TransCurr],[TransAmt] ,[TransDate],[TraceNo]  ,[RRNumber],[TerminalId]"
               + ",[LoadedAtRMCycle], [ProcessedAtRMCycle],[MatchingCateg]  ,[AUTHNUM] ,[ResponseCode] ,[Processed]"
               + ",[Mask]  ,[TXNSRC]   ,[TXNDEST] ,[Comment]  ,[Net_TransDate]"
                + ",[ACCEPTOR_ID],[ACCEPTORNAME] ,[CAP_DATE]"
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns] "
                 + InSelectionCriteria // Includes Where
                   + " AND (Net_TransDate >= @DateFrom AND Net_TransDate <= @DateTo) "
               + " ) "
               + " SELECT * FROM MergedTbl "
                ;

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableSelectedFields);

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
        // READ The transactions for Aging
        //
        public void ReadTrans_Table_For_Aging(string InTableId, string InMatchingCateg, DateTime InDateFrom, DateTime InDateTo)

        {
            //WDateTimeA = InDateTimeA;
            //WDateTimeB = InDateTimeB;
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            TableAgingAnalysis = new DataTable();
            TableAgingAnalysis.Clear();

            SqlString =
             " SELECT Net_TransDate As Date, count(*) As TotalTXNs, sum(TransAmt) As TotalAmt "
             + "  FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
             + "  WHERE MatchingCateg = @MatchingCateg AND Processed = 0 "
              + " AND ( Net_TransDate >= @DateFrom AND Net_TransDate <= @DateTo )"
             + "  group by Net_TransDate "
             + "  Having sum(TransAmt)> 0 "
             + "  Order by Net_TransDate DESC "
             ;


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableAgingAnalysis);

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
        // Read Totals from IST 
        //  
        //
        public decimal TotalWithdrawls;
        public decimal TotalDeposits;
        public void ReadTrans_Totals_FromSpecificTableBetween_Dates(string InTerminalId, DateTime InDateFrom
                                             , DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                  ;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                TotalWithdrawls = TotalWithdrawls + TransAmt;
                            }
                            if (TransType == 23)
                            {
                                TotalDeposits = TotalDeposits + TransAmt;
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
        //SELECT TOP 1 * FROM login order by date desc
        // Read_AND_Find_Repl_CAP_DATE
        public DateTime Read_AND_Find_Repl_CAP_DATE(string InTerminalId, DateTime InDateFrom
                                                             , DateTime InDateTo, string InOrderString, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime WCAP_DATE = NullPastDate;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT TOP 1 * "
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                 + InOrderString;
                //+ " ORDER BY TransDate DESC ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT TOP 1 * FROM MergedTbl "  // GET THE 
                  + InOrderString
                + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        cmd.CommandTimeout = 120;
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            //ReadFieldsInTable(rdr);
                            WCAP_DATE = (DateTime)rdr["CAP_DATE"];

                            RecordFound = true;

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
            return WCAP_DATE;
        }

        // READ The transactions up to replenishement for GL purposes

        public void ReadTrans_Totals_ForCurrent_CAP_DATE_Upto_Replenishement(string InTerminalId, DateTime InCAP_DATE, DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                  + " AND TransDate < @DateTo "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                  ;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                  + " AND TransDate < @DateTo "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
                  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                  + " AND TransDate < @DateTo "
                  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "

                + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        cmd.Parameters.AddWithValue("@DateTo", InDateTo);

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            if (TransType == 11)
                            {
                                TotalWithdrawls = TotalWithdrawls + TransAmt;
                            }
                            if (TransType == 23)
                            {
                                TotalDeposits = TotalDeposits + TransAmt;
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

        // READ The transactions up to replenishement for GL purposes

        public void ReadTrans_Table_FromReplenishmentToEND_CAP_DATE_(string InTerminalId,
            DateTime InCAP_DATE, DateTime InDateFrom, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                  + " AND CAP_DATE <> '1900-01-01' "
                      //  + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                      //  + " AND NotInJournal = 0 "
                      + " AND CAP_DATE <> '1900-01-01' "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
               + " ) "
               + " SELECT * FROM MergedTbl "
                 + " WHERE TransDescr <> 'FOREX_DEPOSIT'  " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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

        // READ The transactions up to replenishement for GL purposes

        public void ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE(string InTerminalId,
            DateTime InCAP_DATE, DateTime InDateFrom, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                   + " AND CAP_DATE<> '1900-01-01' ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                    + " AND CAP_DATE<> '1900-01-01' ) "
                      //  + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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

        // Find Totals Based on Journal 
        public void ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_2(string InTerminalId,
            DateTime InCAP_DATE, DateTime InDateFrom, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT TerminalId, TransType, Sum(TransAmount) as TotalAmt    "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                   + " AND CAP_DATE<> '1900-01-01' ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Group By TerminalId,TransType "
               + " UNION ALL  "
               + " SELECT TerminalId, TransType, Sum(TransAmount) as TotalAmt   "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                    + " AND CAP_DATE<> '1900-01-01' ) "
                      //  + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Group By TerminalId,TransType "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   "
                      + "  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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


        // Find Totals Based on IST
        public void ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_3(string InTerminalId,
            DateTime InCAP_DATE, DateTime InDateFrom, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT TerminalId, TransType, Sum(TransAmt) as TotalAmt    "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                   + " AND CAP_DATE<> '1900-01-01' ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals' "
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Group By TerminalId,TransType "
               + " UNION ALL  "
               + " SELECT TerminalId, TransType, Sum(TransAmt) as TotalAmt   "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                    + " AND CAP_DATE<> '1900-01-01' ) "
                      //  + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals' "
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Group By TerminalId,TransType "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   "
                      + "  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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

        public void ReadTrans_Table_FromPreviousReplenishmentTo_Current_Mpa(string InTerminalId,
            DateTime InDateFrom, DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId "
                  + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                       + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " Where TerminalId = @TerminalId "
                    + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                         + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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


        public void ReadTrans_Table_FromPreviousReplenishmentTo_Current_IST(string InTerminalId,
            DateTime InDateFrom, DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId "
                  + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId "
                    + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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


        public void ReadTrans_Table_FromPreviousReplenishmentTo_Current_COREBANKING(string InTerminalId,
            DateTime InDateFrom, DateTime InDateTo, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                  + " Where TerminalId = @TerminalId "
                  + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[COREBANKING] "
                  + " Where TerminalId = @TerminalId "
                    + " AND ( TransDate > @DateFrom  AND TransDate < @DateTo ) "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23)  AND Comment <> 'Reversals'"
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + "   " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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


        public void ReadTrans_Table_FromPreviousReplenishmentTo_Previous_END_CAP_DATE_IST(string InTerminalId,
          DateTime InCAP_DATE, DateTime InDateFrom, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

           

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId AND (CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                   + " AND CAP_DATE<> '1900-01-01' ) "
                     //  + " AND NotInJournal = 0 "
                     + " AND ResponseCode = '0' AND TXNSRC = '1'"
                      + " AND (TransType = 11 OR TransType = 23) AND Comment <> 'Reversals' "
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " UNION ALL  "
               + " SELECT * "
               + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns] "
                  + " Where TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                  + " AND TransDate > @DateFrom "
                    + " AND CAP_DATE<> '1900-01-01' ) "
                      //  + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + "AND (TransType = 11 OR TransType = 23) AND Comment <> 'Reversals' "
                       + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
               + " ) "
               + " SELECT * FROM MergedTbl "
                + " " // FOREX DEPOSIT  
                      + " ORDER By TransDate  ";

            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

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
        // READ And Order by date 
        //

        public void ReadTransSpecificFromSpecificTable_Order_By_Date(string InSelectionCriteria
            , string InTableName, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT *"
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
                  + InSelectionCriteria
                  + " ORDER BY TransDate ";
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
               " WITH MergedTbl AS "
               + " ( "
               + " SELECT *  "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
               + InSelectionCriteria // Includes Where
               + " UNION ALL  "
               + " SELECT * "
               + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
               + InSelectionCriteria // Includes Where
               + " ) "
               + " SELECT * FROM MergedTbl "
               + " ORDER BY TransDate "
                + "  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
        // WITH DATE AS PARAMETER 
        // We do not need this 
        public void ReadTransSpecificFromSpecificTable_And_Date_XXX(string InSelectionCriteria, string InTableName
                                            , DateTime InTransDate, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                  + " FROM " + InTableName
                  + InSelectionCriteria + " AND TransDate = @TransDate "
                  + "  ";

            //if (In_DB_Mode == 2)
            //{
            //    SqlString =
            //   " WITH MergedTbl AS "
            //   + " ( "
            //   + " SELECT *  "
            //   + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
            //   + InSelectionCriteria // Includes Where
            //   + " UNION ALL  "
            //   + " SELECT * "
            //   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
            //   + InSelectionCriteria // Includes Where
            //   + " ) "
            //   + " SELECT * FROM MergedTbl "
            //   + " ORDER BY TransDate "
            //    + "  ";
            //}

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
        public DataTable TableDetails_RAW = new DataTable();

        public bool FindRecordsFromMasterRecord(string InOperator, string InFileID, string InMatchingCateg,
           DateTime InTransDate,
           string InTerminalId, int InTraceNo, string InRRNumber, decimal InTransAmt, string InCard_Encrypted
                                                                                   , int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string WResponseCode = "";
            bool ReturnRecordFoundInUniversal = false;

            if (InFileID == "")
            {
                return ReturnRecordFoundInUniversal = false;
            }

            try
            {
                // IF IN MODE = 1 NORMAL read 
                // IF IN MODE = 2 read POS with RRN and no amount
                // IF IN MODE = 3 read Reversals 
                // IF IN MODE = 4 read POS with Encrypted Card

                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();


                Msf.ReadReconcSourceFilesByFileId(InFileID);

                string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";
                // string PhysicalNameRAW = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "_RAW]";

                if (In_DB_Mode == 1)
                {
                    if (InMode == 1) // This fills the Table by Trace, Terminal and Date (includes Succesfull and not) 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();
                        if (InTraceNo > 0)
                        {
                            SqlString =
                                                  " SELECT * "
                                                  + " FROM " + PhysicalName
                                                  + " WHERE Net_TransDate = @TransDate"
                                                    + " AND MatchingCateg = @MatchingCateg "
                                                  + " AND TerminalId = @TerminalId "
                                                  + " AND (TraceNo =@TraceNo ) "
                                                  + " AND TransAmt =@TransAmt "
                                                  + " ORDER BY ResponseCode"
                                                  + "  ";
                        }
                        else
                        {

                            // Mode = 2
                            SqlString =
                                                                    " SELECT * "
                                                                    + " FROM " + PhysicalName
                                                                    + " WHERE Net_TransDate = @TransDate"
                                                                      + " AND MatchingCateg = @MatchingCateg "
                                                                    //+ " AND TerminalId = @TerminalId "
                                                                    + " AND (RRNumber =@RRNumber ) "
                                                                    + " AND TransAmt =@TransAmt "
                                                                     + " ORDER BY ResponseCode"
                                                                    + " ";


                        }
                    }
                    if (InMode == 2) // This fills the Table for POS with Encrypted Card and RRN = Authorisation code
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                                                                     " SELECT * "
                                                                     + " FROM " + PhysicalName
                                                                     + " WHERE Card_Encrypted = @Card_Encrypted"
                                                                       + " AND MatchingCateg = @MatchingCateg "
                                                                     + " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                                                                      //  + " AND TransAmt =@TransAmt "
                                                                      + " ORDER BY ResponseCode"
                                                                     + " ";

                    }
                    if (InMode == 3) // This is for Reversals 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                                                                       " SELECT * "
                                                                       + " FROM " + PhysicalName
                                                                       + " WHERE Net_TransDate = @TransDate"
                                                                         + " AND MatchingCateg = @MatchingCateg "
                                                                       + " AND TerminalId = @TerminalId "
                                                                       + " AND (RRNumber =@RRNumber ) "
                                                                       + " AND TransAmt =@TransAmt "
                                                                        + " ORDER BY ResponseCode"
                                                                       + " ";

                    }
                }

                if (In_DB_Mode == 2)
                {
                    if (InMode == 1) // This fills the Table by Trace, Terminal and Date (includes Succesfull and not) 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();
                        if (InTraceNo > 0)
                        {

                            SqlString =
                          " WITH MergedTbl AS "
                            + " ( "
                            + " SELECT *  "
                            + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                            + " WHERE Net_TransDate = @TransDate "
                              + " AND MatchingCateg = @MatchingCateg "
                            + " AND TerminalId = @TerminalId "
                            + " AND (TraceNo =@TraceNo ) "
                            + " AND TransAmt =@TransAmt "
                            + " UNION ALL  "
                            + " SELECT * "
                            + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                            + " WHERE Net_TransDate = @TransDate"
                              + " AND MatchingCateg = @MatchingCateg "
                            + " AND TerminalId = @TerminalId "
                            + " AND (TraceNo =@TraceNo ) "
                            + " AND TransAmt =@TransAmt "
                            + " ) "
                            + " SELECT * FROM MergedTbl "
                             + " ORDER BY ResponseCode"
                            + "  ";
                        }
                        else
                        {

                            SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                           + " WHERE "
                           + " Net_TransDate = @TransDate "
                             + " AND MatchingCateg = @MatchingCateg "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND "
                           + " (RRNumber =@RRNumber ) "
                           + " AND TransAmt =@TransAmt "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                           + " WHERE Net_TransDate = @TransDate"
                             + " AND MatchingCateg = @MatchingCateg "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND (RRNumber =@RRNumber ) "
                           + " AND TransAmt =@TransAmt "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                        }
                    }
                    if (InMode == 2) // This fills the Table for POS with Encrypted Card and RRN = Authorisation code
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     //+ " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     //+ " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                    }
                    if (InMode == 4) // This fills the Table for POS with Encrypted Card and DATE
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted AND TransAmt= @TransAmt "
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     // + " AND ResponseCode <> '0' "
                                                                     + " AND Net_TransDate =@Net_TransDate  "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted  AND TransAmt= @TransAmt "
                             + " AND MatchingCateg = @MatchingCateg "
                                                                      // + " AND ResponseCode <> '0' "
                                                                      + " AND Net_TransDate =@Net_TransDate  "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                    }

                    // + "  ";
                }

                // NAME 
                //

                using (SqlConnection conn =
                 new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Net_TransDate", InTransDate.Date);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            if (InTraceNo > 0)
                            {
                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNo", InTraceNo);
                            }
                            else
                            {
                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@RRNumber", InRRNumber);
                            }

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransAmt", InTransAmt);
                            //  sqlAdapt.SelectCommand.Parameters.AddWithValue("@CardNumber", InCardNumber);
                            //Create a datatable that will be filled with the data retrieved from the command
                            sqlAdapt.SelectCommand.CommandTimeout = 350;

                            sqlAdapt.Fill(TableDetails_RAW);

                            // Close conn
                            conn.Close();


                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                // READ THE RESPONSE CODE 

                ResponseCode = "";
                int Count = 0;

                int I = 0;
                // 
                while (I <= (TableDetails_RAW.Rows.Count - 1))
                {
                    int WSeqNo = (int)TableDetails_RAW.Rows[I]["SeqNo"];
                    SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                           + "  ";

                    using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SqlString, conn))
                            {

                                // Read table 

                                cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                                SqlDataReader rdr = cmd.ExecuteReader();

                                while (rdr.Read())
                                {
                                    RecordFound = true;

                                    Count = Count + 1;

                                    if (Count > 1) ResponseCode = ResponseCode + "..AND.. ";

                                    string ResponseCodetemp = (string)rdr["ResponseCode"];

                                    ResponseCode = ResponseCode + ResponseCodetemp;

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
                    I = I + 1;
                }




                //if (InMode == 2) // GIVES THE DETAILS FOR Merchant (Succesful or not )
                //{
                //    RRDMJournalAudi_BDC Ps = new RRDMJournalAudi_BDC();
                //    Ps.ReadTransSpecific_POS(InOperator,
                //                              PhysicalName, InTransDate, InTerminalId, InRRNumber);
                //}

                if (TableDetails_RAW.Rows.Count > 0)
                {
                    ReturnRecordFoundInUniversal = true;
                }
                else
                {
                    ReturnRecordFoundInUniversal = false;
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

            return ReturnRecordFoundInUniversal;
        }

        public bool FindRecordsFromMasterRecordLoadedAtCycle(string InOperator, string InFileID,
          string InRRNumber, decimal InTransAmt, string InCard_Encrypted, int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string WResponseCode = "";
            bool ReturnRecordFoundInUniversal = false;

            if (InFileID == "")
            {
                return ReturnRecordFoundInUniversal = false;
            }

            try
            {

                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();


                Msf.ReadReconcSourceFilesByFileId(InFileID);

                string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";
                // string PhysicalNameRAW = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "_RAW]";

                TableDetails_RAW = new DataTable();
                TableDetails_RAW.Clear();

                SqlString =
                   " WITH MergedTbl AS "
                   + " ( "
                   + " SELECT *  "
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                   + " WHERE Card_Encrypted = @Card_Encrypted AND TransAmt= @TransAmt "
                                                              + " AND RRNumber = @RRNumber "
                                                             + " AND LoadedAtRMCycle =@LoadedAtRMCycle  "
                   + " UNION ALL  "
                   + " SELECT * "
                   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                   + " WHERE Card_Encrypted = @Card_Encrypted  AND TransAmt= @TransAmt "
                                                               + " AND RRNumber = @RRNumber "
                                                              + " AND LoadedAtRMCycle =@LoadedAtRMCycle  "
                   + " ) "
                   + " SELECT * FROM MergedTbl "
                    + " ORDER BY ResponseCode"
                   + "  ";

                // NAME 
                //

                using (SqlConnection conn =
                 new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {
                           
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                           
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);
                           
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@RRNumber", InRRNumber);
                         
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransAmt", InTransAmt);
                            //  sqlAdapt.SelectCommand.Parameters.AddWithValue("@CardNumber", InCardNumber);
                            //Create a datatable that will be filled with the data retrieved from the command
                            sqlAdapt.SelectCommand.CommandTimeout = 350;

                            sqlAdapt.Fill(TableDetails_RAW);

                            // Close conn
                            conn.Close();


                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                // READ THE RESPONSE CODE 

                ResponseCode = "";
                int Count = 0;

                int I = 0;
                // 
                while (I <= (TableDetails_RAW.Rows.Count - 1))
                {
                    int WSeqNo = (int)TableDetails_RAW.Rows[I]["SeqNo"];
                    SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                           + "  ";

                    using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SqlString, conn))
                            {

                                // Read table 

                                cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                                SqlDataReader rdr = cmd.ExecuteReader();

                                while (rdr.Read())
                                {
                                    RecordFound = true;

                                    Count = Count + 1;

                                    if (Count > 1) ResponseCode = ResponseCode + "..AND.. ";

                                    string ResponseCodetemp = (string)rdr["ResponseCode"];

                                    ResponseCode = ResponseCode + ResponseCodetemp;

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
                    I = I + 1;
                }




                //if (InMode == 2) // GIVES THE DETAILS FOR Merchant (Succesful or not )
                //{
                //    RRDMJournalAudi_BDC Ps = new RRDMJournalAudi_BDC();
                //    Ps.ReadTransSpecific_POS(InOperator,
                //                              PhysicalName, InTransDate, InTerminalId, InRRNumber);
                //}

                if (TableDetails_RAW.Rows.Count > 0)
                {
                    ReturnRecordFoundInUniversal = true;
                }
                else
                {
                    ReturnRecordFoundInUniversal = false;
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

            return ReturnRecordFoundInUniversal;
        }

        public bool FindRecordsFromMasterRecord_HST(string InOperator, string InFileID,string InMatchingCateg,
          DateTime InTransDate,
          string InTerminalId, int InTraceNo, string InRRNumber, decimal InTransAmt, string InCard_Encrypted
                                                                                  , int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string WResponseCode = "";
            bool ReturnRecordFoundInUniversal = false;

            if (InFileID == "")
            {
                return ReturnRecordFoundInUniversal = false;
            }

            try
            {
                // IF IN MODE = 1 NORMAL read 
                // IF IN MODE = 2 read POS with RRN and no amount
                // IF IN MODE = 3 read Reversals 
                // IF IN MODE = 4 read POS with Encrypted Card

                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();


                Msf.ReadReconcSourceFilesByFileId(InFileID);

                string PhysicalName = "[RRDM_Reconciliation_ITMX_HST].[dbo]." + "[" + Msf.InportTableName + "]";
                // string PhysicalNameRAW = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "_RAW]";

                if (In_DB_Mode == 1)
                {
                    if (InMode == 1) // This fills the Table by Trace, Terminal and Date (includes Succesfull and not) 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();
                        if (InTraceNo > 0)
                        {
                            SqlString =
                                                  " SELECT * "
                                                  + " FROM " + PhysicalName
                                                  + " WHERE Net_TransDate = @TransDate"
                                                  + " AND MatchingCateg = @MatchingCateg "
                                                  + " AND TerminalId = @TerminalId "
                                                  + " AND (TraceNo =@TraceNo ) "
                                                  + " AND TransAmt =@TransAmt "
                                                  + " ORDER BY ResponseCode"
                                                  + "  ";
                        }
                        else
                        {

                            // Mode = 2
                            SqlString =
                                                                    " SELECT * "
                                                                    + " FROM " + PhysicalName
                                                                    + " WHERE Net_TransDate = @TransDate"
                                                                      //+ " AND TerminalId = @TerminalId "
                                                                      + " AND MatchingCateg = @MatchingCateg "
                                                                    + " AND (RRNumber =@RRNumber ) "
                                                                    + " AND TransAmt =@TransAmt "
                                                                     + " ORDER BY ResponseCode"
                                                                    + " ";


                        }
                    }
                    if (InMode == 2) // This fills the Table for POS with Encrypted Card and RRN = Authorisation code
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                                                                     " SELECT * "
                                                                     + " FROM " + PhysicalName
                                                                     + " WHERE Card_Encrypted = @Card_Encrypted"
                                                                       + " AND MatchingCateg = @MatchingCateg "
                                                                     + " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                                                                      //  + " AND TransAmt =@TransAmt "
                                                                      + " ORDER BY ResponseCode"
                                                                     + " ";

                    }
                    if (InMode == 3) // This is for Reversals 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                                                                       " SELECT * "
                                                                       + " FROM " + PhysicalName
                                                                       + " WHERE Net_TransDate = @TransDate"
                                                                         + " AND MatchingCateg = @MatchingCateg "
                                                                       + " AND TerminalId = @TerminalId "
                                                                       + " AND (RRNumber =@RRNumber ) "
                                                                       + " AND TransAmt =@TransAmt "
                                                                        + " ORDER BY ResponseCode"
                                                                       + " ";

                    }
                }

                if (In_DB_Mode == 2)
                {
                    if (InMode == 1) // This fills the Table by Trace, Terminal and Date (includes Succesfull and not) 
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();
                        if (InTraceNo > 0)
                        {


                            SqlString =
                          " WITH MergedTbl AS "
                            + " ( "
                            + " SELECT *  "
                            + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo]." + Msf.InportTableName
                            + " WHERE Net_TransDate = @TransDate "
                              + " AND MatchingCateg = @MatchingCateg "
                            + " AND TerminalId = @TerminalId "
                            + " AND (TraceNo =@TraceNo ) "
                            + " AND TransAmt =@TransAmt "
                            + " UNION ALL  "
                            + " SELECT * "
                            + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + Msf.InportTableName
                            + " WHERE Net_TransDate = @TransDate"
                              + " AND MatchingCateg = @MatchingCateg "
                            + " AND TerminalId = @TerminalId "
                            + " AND (TraceNo =@TraceNo ) "
                            + " AND TransAmt =@TransAmt "
                            + " ) "
                            + " SELECT * FROM MergedTbl "
                             + " ORDER BY ResponseCode"
                            + "  ";
                        }
                        else
                        {

                            SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Net_TransDate = @TransDate "
                             + " AND MatchingCateg = @MatchingCateg "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND (RRNumber =@RRNumber ) "
                           + " AND TransAmt =@TransAmt "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Net_TransDate = @TransDate"
                             + " AND MatchingCateg = @MatchingCateg "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND (RRNumber =@RRNumber ) "
                           + " AND TransAmt =@TransAmt "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                        }
                    }
                    if (InMode == 2) // This fills the Table for POS with Encrypted Card and RRN = Authorisation code
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     //+ " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     //+ " AND TerminalId = @TerminalId "
                                                                     + " AND RRNumber =@RRNumber  "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                    }
                    if (InMode == 4) // This fills the Table for POS with Encrypted Card and DATE
                    {

                        TableDetails_RAW = new DataTable();
                        TableDetails_RAW.Clear();

                        SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                     + " AND ResponseCode <> '0' "
                                                                     + " AND Net_TransDate =@Net_TransDate  "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + Msf.InportTableName
                           + " WHERE Card_Encrypted = @Card_Encrypted"
                             + " AND MatchingCateg = @MatchingCateg "
                                                                      + " AND ResponseCode <> '0' "
                                                                      + " AND Net_TransDate =@Net_TransDate  "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                            + " ORDER BY ResponseCode"
                           + "  ";

                    }

                    // + "  ";
                }

                // NAME 
                //

                using (SqlConnection conn =
                 new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Net_TransDate", InTransDate.Date);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            if (InTraceNo > 0)
                            {
                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNo", InTraceNo);
                            }
                            else
                            {
                                sqlAdapt.SelectCommand.Parameters.AddWithValue("@RRNumber", InRRNumber);
                            }

                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransAmt", InTransAmt);
                            //  sqlAdapt.SelectCommand.Parameters.AddWithValue("@CardNumber", InCardNumber);
                            //Create a datatable that will be filled with the data retrieved from the command
                            sqlAdapt.SelectCommand.CommandTimeout = 350;

                            sqlAdapt.Fill(TableDetails_RAW);

                            // Close conn
                            conn.Close();


                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                // READ THE RESPONSE CODE 

                ResponseCode = "";
                int Count = 0;

                int I = 0;
                // 
                while (I <= (TableDetails_RAW.Rows.Count - 1))
                {
                    int WSeqNo = (int)TableDetails_RAW.Rows[I]["SeqNo"];
                    SqlString =
                           " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT *  "
                           + " FROM [RRDM_Reconciliation_ITMX_HST].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " UNION ALL  "
                           + " SELECT * "
                           + " FROM[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + Msf.InportTableName
                           + " WHERE SeqNo = @SeqNo "
                           + " ) "
                           + " SELECT * FROM MergedTbl "
                           + "  ";

                    using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SqlString, conn))
                            {

                                // Read table 

                                cmd.Parameters.AddWithValue("@SeqNo", WSeqNo);

                                SqlDataReader rdr = cmd.ExecuteReader();

                                while (rdr.Read())
                                {
                                    RecordFound = true;

                                    Count = Count + 1;

                                    if (Count > 1) ResponseCode = ResponseCode + "..AND.. ";

                                    string ResponseCodetemp = (string)rdr["ResponseCode"];

                                    ResponseCode = ResponseCode + ResponseCodetemp;

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
                    I = I + 1;
                }




                //if (InMode == 2) // GIVES THE DETAILS FOR Merchant (Succesful or not )
                //{
                //    RRDMJournalAudi_BDC Ps = new RRDMJournalAudi_BDC();
                //    Ps.ReadTransSpecific_POS(InOperator,
                //                              PhysicalName, InTransDate, InTerminalId, InRRNumber);
                //}

                if (TableDetails_RAW.Rows.Count > 0)
                {
                    ReturnRecordFoundInUniversal = true;
                }
                else
                {
                    ReturnRecordFoundInUniversal = false;
                }
            }
            catch (Exception ex)
            {

                CatchDetails(ex);
            }

            return ReturnRecordFoundInUniversal;
        }



        // MERGE TWO TABLES - FOR POS
        public int TotalEXT;
        public decimal TotalEXT_Amt;
        public int TotalINT;
        public decimal TotalINT_Amt;
        public decimal TotalAdj_Amt;

        public void ReadMergedTXNWithOthers(string InTableA, string InTableB, string InMatchingCateg, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In MODE 
            // 1 : Shows on the Left the unmatched ones with ActionType = 0
            // 3 : Shows on the Right the unmatched ones with ActionType = 3
            // 5 : Shows on the left the manual Matched for this Cycle
            // 7 : Move to the right for Making Undo on the manual matched

            TotalEXT = 0;
            TotalINT = 0;
            TotalEXT_Amt = 0;
            TotalINT_Amt = 0;
            TotalAdj_Amt = 0;
            int Mult = 0;

            TablePOS_Settlement = new DataTable();
            TablePOS_Settlement.Clear();

            TablePOS_Settlement.Columns.Add("SeqNo", typeof(int));
            TablePOS_Settlement.Columns.Add("Origin", typeof(string));
            TablePOS_Settlement.Columns.Add("MASK", typeof(string));
            TablePOS_Settlement.Columns.Add("Card Encypted", typeof(string));
            TablePOS_Settlement.Columns.Add("Amount", typeof(decimal));
            TablePOS_Settlement.Columns.Add("Auth Number", typeof(string));
            TablePOS_Settlement.Columns.Add("DateTime", typeof(string));
            TablePOS_Settlement.Columns.Add("RESP CODE", typeof(string));
            TablePOS_Settlement.Columns.Add("MerchantId", typeof(string));
            TablePOS_Settlement.Columns.Add("DR/CR", typeof(string));
            TablePOS_Settlement.Columns.Add("AccNo", typeof(string));
            TablePOS_Settlement.Columns.Add("Terminal", typeof(string));

            if (InMode == 1)
            {
                SqlString =
                     " WITH MergedTbl AS "
                           + " ( "
                           + " SELECT  "
                            + " [SeqNo] "
       + ",[OriginFileName]"
      + " ,[OriginalRecordId]"
      + " ,[LoadedAtRMCycle]"
      + " ,[MatchingCateg]"
      + " ,[Origin]"
      + " ,[TerminalType]"
      + " ,[TransTypeAtOrigin]"
      + " ,[TerminalId]"
      + ",[TransType]"
       + ",[TransDescr]"
       + ",[CardNumber]"
      + " ,[AccNo]"
       + ",[TransCurr]"
       + ",[TransAmt]"
      + " ,[AmtFileBToFileC]"
       + ",[TransDate]"
      + " ,[TraceNo]"
      + " ,[RRNumber]"
       + ",[FullTraceNo]"
      + " ,[AUTHNUM]"
       + ",[ResponseCode]"
      + " ,[ActionType]"
       + ",[Processed]"
       + ",[ProcessedAtRMCycle]"
       + ",[Mask]"
       + ",[IsSettled]"
      + " ,[UniqueRecordId]"
      + " ,[Operator]"
       + ",[TXNSRC]"
      + " ,[TXNDEST]"
       + ",[Comment]"
      + " ,[EXTERNAL_DATE]"
      + " ,[Net_TransDate]"
      + " ,[Card_Encrypted]"
       + ",[ACCEPTOR_ID]"
       + ",[ACCEPTORNAME]"
       + ",[CAP_DATE]"
                           + " FROM " + InTableA // [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                           + " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg = @MatchingCateg AND ActionType='00' AND IsSettled = 0 "

                           + " UNION ALL"
                           + " SELECT  "
                           + " [SeqNo] "
       + ",[OriginFileName]"
      + " ,[OriginalRecordId]"
      + " ,[LoadedAtRMCycle]"
      + " ,[MatchingCateg]"
      + " ,[Origin]"
      + " ,[TerminalType]"
      + " ,[TransTypeAtOrigin]"
      + " ,[TerminalId]"
      + ",[TransType]"
       + ",[TransDescr]"
       + ",[CardNumber]"
      + " ,[AccNo]"
       + ",[TransCurr]"
       + ",[TransAmt]"
      + " ,[AmtFileBToFileC]"
       + ",[TransDate]"
      + " ,[TraceNo]"
      + " ,[RRNumber]"
       + ",[FullTraceNo]"
      + " ,[AUTHNUM]"
       + ",[ResponseCode]"
      + " ,[ActionType]"
       + ",[Processed]"
       + ",[ProcessedAtRMCycle]"
       + ",[Mask]"
       + ",[IsSettled]"
      + " ,[UniqueRecordId]"
      + " ,[Operator]"
       + ",[TXNSRC]"
      + " ,[TXNDEST]"
       + ",[Comment]"
      + " ,[EXTERNAL_DATE]"
      + " ,[Net_TransDate]"
      + " ,[Card_Encrypted]"
       + ",[ACCEPTOR_ID]"
       + ",[ACCEPTORNAME]"
       + ",[CAP_DATE]"
                           + "FROM " + InTableB // [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
                           + " WHERE MatchingCateg = @MatchingCateg and Processed = 0  "
                           + " ) "
                           + " Select * "
                           + " From MergedTbl "
                           + " ORDER BY TransTypeAtOrigin ,TransDate ASC";
            }
            if (InMode == 3)
            {

                SqlString =
         " WITH MergedTbl AS "
               + " ( "
               + " SELECT  "
                + " [SeqNo] "
+ ",[OriginFileName]"
+ " ,[OriginalRecordId]"
+ " ,[LoadedAtRMCycle]"
+ " ,[MatchingCateg]"
+ " ,[Origin]"
+ " ,[TerminalType]"
+ " ,[TransTypeAtOrigin]"
+ " ,[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ " ,[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ " ,[AmtFileBToFileC]"
+ ",[TransDate]"
+ " ,[TraceNo]"
+ " ,[RRNumber]"
+ ",[FullTraceNo]"
+ " ,[AUTHNUM]"
+ ",[ResponseCode]"
+ " ,[ActionType]"
+ ",[Processed]"
+ ",[ProcessedAtRMCycle]"
+ ",[Mask]"
+ ",[IsSettled]"
+ " ,[UniqueRecordId]"
+ " ,[Operator]"
+ ",[TXNSRC]"
+ " ,[TXNDEST]"
+ ",[Comment]"
+ " ,[EXTERNAL_DATE]"
+ " ,[Net_TransDate]"
+ " ,[Card_Encrypted]"
+ ",[ACCEPTOR_ID]"
+ ",[ACCEPTORNAME]"
+ ",[CAP_DATE]"
               + " FROM " + InTableA
               + " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg = @MatchingCateg AND ActionType='03' "
               + " UNION ALL"
                + " SELECT  "
                + " [SeqNo] "
+ ",[OriginFileName]"
+ " ,[OriginalRecordId]"
+ " ,[LoadedAtRMCycle]"
+ " ,[MatchingCateg]"
+ " ,[Origin]"
+ " ,[TerminalType]"
+ " ,[TransTypeAtOrigin]"
+ " ,[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ " ,[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ " ,[AmtFileBToFileC]"
+ ",[TransDate]"
+ " ,[TraceNo]"
+ " ,[RRNumber]"
+ ",[FullTraceNo]"
+ " ,[AUTHNUM]"
+ ",[ResponseCode]"
+ " ,[ActionType]"
+ ",[Processed]"
+ ",[ProcessedAtRMCycle]"
+ ",[Mask]"
+ ",[IsSettled]"
+ " ,[UniqueRecordId]"
+ " ,[Operator]"
+ ",[TXNSRC]"
+ " ,[TXNDEST]"
+ ",[Comment]"
+ " ,[EXTERNAL_DATE]"
+ " ,[Net_TransDate]"
+ " ,[Card_Encrypted]"
+ ",[ACCEPTOR_ID]"
+ ",[ACCEPTORNAME]"
+ ",[CAP_DATE]"
               + "FROM " + InTableB
               + " WHERE MatchingCateg = @MatchingCateg AND ActionType='03' "


     + " ) "
               + " Select * "
               + " From MergedTbl "
               + " ORDER BY TransTypeAtOrigin ASC";
            }
            //Mg.ActionType = "12";
            //Mg.UniqueRecordId = WUnique;
            //Mg.Comment = "Manual_Matching";
            if (InMode == 5)
            {
                SqlString =
                           " SELECT * FROM " + InTableA
                           + " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg = @MatchingCateg AND ActionType='12' AND IsSettled = 0 "
                           + " UNION ALL"
                           + " SELECT * FROM " + InTableB
                           + " WHERE MatchingCateg = @MatchingCateg AND ActionType = '12' AND IsSettled = 0 "
                           + " ORDER BY UniqueRecordId, TransTypeAtOrigin ASC";
            }
            if (InMode == 7)
            {
                SqlString =
                           " SELECT * FROM " + InTableA
                           + " WHERE Mask<>'111' AND Mask<>''  AND MatchingCateg = @MatchingCateg AND ActionType='17' AND IsSettled = 0 "
                           + " UNION ALL"
                           + " SELECT * FROM " + InTableB
                           + " WHERE MatchingCateg = @MatchingCateg AND ActionType='17' AND IsSettled = 0 "
                           + " ORDER by TransTypeAtOrigin ASC";
            }


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowSelected = TablePOS_Settlement.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            if (Origin == "MasterCard_International")
                            {
                                RowSelected["Origin"] = "EXTERNAL";
                                TotalEXT = TotalEXT + 1;
                                if (TransType == 11)
                                {
                                    Mult = +1;
                                }
                                else
                                {
                                    // 21 deposit or 
                                    Mult = +1;
                                }
                                TotalEXT_Amt = TotalEXT_Amt + TransAmt * Mult;
                            }
                            else
                            {
                                TotalINT = TotalINT + 1;

                                if (TransTypeAtOrigin == "WAdjustment")
                                {
                                    if (TransType == 11)
                                    {
                                        Mult = +1;
                                    }
                                    else
                                    {
                                        // 21 deposit or 
                                        if (ResponseCode == "200000")
                                        {
                                            Mult = 1;
                                        }
                                        else
                                        {
                                            Mult = -1;
                                        }

                                    }
                                    RowSelected["Origin"] = "WAdjustment";
                                    TotalAdj_Amt = TotalAdj_Amt + TransAmt * Mult;
                                }
                                else
                                {
                                    RowSelected["Origin"] = "INTERNAL";
                                    // Not Including 
                                    TotalINT_Amt = TotalINT_Amt + TransAmt;
                                }



                            }
                            RowSelected["MASK"] = Mask;
                            if (Card_Encrypted != "")
                            {
                                RowSelected["Card Encypted"] = Card_Encrypted;
                            }
                            else
                            {
                                RowSelected["Card Encypted"] = CardNumber;
                            }

                            RowSelected["Amount"] = TransAmt;
                            RowSelected["Auth Number"] = RRNumber;
                            RowSelected["DateTime"] = TransDate.ToString();
                            RowSelected["RESP CODE"] = ResponseCode;

                            RowSelected["MerchantId"] = ACCEPTOR_ID + "_" + ACCEPTORNAME;

                            RowSelected["DR/CR"] = TransType.ToString();
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["Terminal"] = TerminalId;


                            // ADD ROW

                            TablePOS_Settlement.Rows.Add(RowSelected);

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
        // Prepare Table for Adjustments - POS
        //
        public void ReadMergedTXNWithOthersFOR_Adj(string InTable, string InMatchingCateg, string InCard_Encrypted
                                                                 , DateTime InNet_TransDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In MODE 
            // 21 : prepares table with all relatives to Adjustment 

            TablePOS_Settlement_Adj = new DataTable();
            TablePOS_Settlement_Adj.Clear();

            TablePOS_Settlement_Adj.Columns.Add("SeqNo", typeof(int));
            TablePOS_Settlement_Adj.Columns.Add("MerchantId", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("OriginFileName", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("CardNo", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("AccNo", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("DR/CR", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("Amount", typeof(decimal));
            TablePOS_Settlement_Adj.Columns.Add("AuthNumber", typeof(string));
            TablePOS_Settlement_Adj.Columns.Add("DateTime", typeof(DateTime));
            TablePOS_Settlement_Adj.Columns.Add("RESP CODE", typeof(string));


            if (InMode == 21)
            {
                SqlString =
                           " SELECT * FROM " + InTable // [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                                                       //  FROM[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]
                         + " WHERE MatchingCateg = @MatchingCateg AND Card_Encrypted = @Card_Encrypted AND Net_TransDate > @Net_TransDate "
                         + " ORDER BY Net_TransDate DESC";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);
                        cmd.Parameters.AddWithValue("@Net_TransDate", InNet_TransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            DataRow RowSelected = TablePOS_Settlement_Adj.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["MerchantId"] = FullTraceNo;

                            RowSelected["CardNo"] = CardNumber;
                            RowSelected["AccNo"] = AccNo;
                            if (TransType == 11) RowSelected["DR/CR"] = "DR";
                            if (TransType == 21) RowSelected["DR/CR"] = "CR";
                            RowSelected["Amount"] = TransAmt;
                            RowSelected["AuthNumber"] = RRNumber;
                            RowSelected["DateTime"] = TransDate;
                            RowSelected["RESP CODE"] = ResponseCode;

                            RowSelected["OriginFileName"] = OriginFileName;

                            // ADD ROW

                            TablePOS_Settlement_Adj.Rows.Add(RowSelected);

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
        // READ Physical table and fill memory table
        //
        public string PhysicalFiledID;
        public string PhysicalFileID_BULK;
        public void ReadTableAndFillTable(string InSignedId, string InTable, string InAtmNo, int InRMCycle, string InMatchingCateg,
                   int InMode, bool InCategoryOnly, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 : Processed this Cycle
            // InMode = 2 : Not processed yet 
            // InMode = 3 : Per ATM 
            // InMode = 5 : Discrepancies not taken action yet by maker  
            // InMode = 9 : BULK RECORDS

            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + Ms.InportTableName;
            PhysicalFileID_BULK = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + Ms.InportTableName;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            // DATA TABLE ROWS DEFINITION 

            if (In_DB_Mode == 1)
            {
                if (InCategoryOnly == true)
                {
                    if (InMode == 1 & InTable == "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                          + " FROM " + PhysicalFiledID
                          + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 1 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 AND MatchingCateg = @MatchingCateg AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 3 & InTable == "Atms_Journals_Txns")
                    {
                        // TERMINAL
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId AND IsMatchingDone = 1 "
                           + " AND MATCHED = 0 AND MatchingAtRMCycle = @RMCycle  ";
                    }
                    if (InMode == 3 & InTable != "Atms_Journals_Txns")
                    {
                        // TERMINAL
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId "
                           + " AND Processed = 1 AND ProcessedAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg ";
                    }
                }
                else
                {
                    // NEED ALL
                    if ((InMode == 1 || InMode == 5) & InTable == "Atms_Journals_Txns")
                    {
                        if (InMode == 5)
                        {
                            SqlString = "SELECT * "
                                                  + " FROM " + PhysicalFiledID
                                                  + " WHERE IsMatchingDone = 1   and ActionType = '00' And Matched = 0 "
                                                   + " and MatchingAtRMCycle =" + InRMCycle
                                                   + " Order By TerminalId ";

                        }
                        else
                        {
                            SqlString = "SELECT * "
                                                  + " FROM " + PhysicalFiledID
                                                  + " WHERE  IsMatchingDone = 0 ";
                            if (InAtmNo != "")
                            {
                                SqlString = "SELECT * "
                              + " FROM " + PhysicalFiledID
                              + " WHERE  IsMatchingDone = 0  AND TerminalId =@TerminalId ";
                            }
                        }

                    }

                    if (InMode == 1 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0  AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 2 & InTable == "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle  ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle  AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 2 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle  ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                          + " FROM " + PhysicalFiledID
                          + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle  AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 3 & InTable == "Atms_Journals_Txns")
                    {
                        // TERMINAL ID
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId AND IsMatchingDone = 1 "
                           + " AND MATCHED = 0 AND MatchingAtRMCycle = @RMCycle  ";
                    }

                    if (InMode == 3 & InTable != "Atms_Journals_Txns")
                    {
                        // TERMINAL ID
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId "
                           + " AND Processed = 1 AND ProcessedAtRMCycle = @RMCycle  ";
                    }
                    if (InMode == 9)
                    {
                        // READ BULK 
                        SqlString = "SELECT TOP 200 * "
                           + " FROM " + PhysicalFileID_BULK
                           + " "
                           + "  ";
                    }
                    if (InMode == 10)
                    {
                        // READ RRDM Table
                        SqlString = "SELECT TOP 200 * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE LoadedAtRMCycle = @RMCycle"
                           + "  ";
                    }

                }


            }
            //  FOR MERGING 
            if (In_DB_Mode == 2)
            {
                if (InCategoryOnly == true)
                {
                    if (InMode == 1 & InTable == "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                          + " FROM " + PhysicalFiledID
                          + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 1 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 AND MatchingCateg = @MatchingCateg AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 2 & InTable == "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 2 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                          + " FROM " + PhysicalFiledID
                          + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg  AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 3 & InTable == "Atms_Journals_Txns")
                    {
                        // TERMINAL
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId AND IsMatchingDone = 1 "
                           + " AND MATCHED = 0 AND MatchingAtRMCycle = @RMCycle  ";
                    }
                    if (InMode == 3 & InTable != "Atms_Journals_Txns")
                    {
                        // TERMINAL
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId "
                           + " AND Processed = 1 AND ProcessedAtRMCycle = @RMCycle AND MatchingCateg = @MatchingCateg ";
                    }
                }
                else
                {
                    // NEED ALL
                    if ((InMode == 1 || InMode == 5) & InTable == "Atms_Journals_Txns")
                    {
                        if (InMode == 5)
                        {
                            SqlString = "SELECT * "
                                                  + " FROM " + PhysicalFiledID
                                                  + " WHERE IsMatchingDone = 1   and ActionType = '00' And Matched = 0 "
                                                   + " and MatchingAtRMCycle =" + InRMCycle
                                                   + " Order By TerminalId ";

                        }
                        else
                        {
                            SqlString = "SELECT * "
                                                  + " FROM " + PhysicalFiledID
                                                  + " WHERE  IsMatchingDone = 0 ";
                            if (InAtmNo != "")
                            {
                                SqlString = "SELECT * "
                              + " FROM " + PhysicalFiledID
                              + " WHERE  IsMatchingDone = 0  AND TerminalId =@TerminalId ";
                            }
                        }

                    }

                    if (InMode == 1 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0 ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 0  AND TerminalId =@TerminalId ";
                        }

                    }

                    if (InMode == 2 & InTable == "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle  ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  IsMatchingDone = 1 AND MatchingAtRMCycle = @RMCycle  AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 2 & InTable != "Atms_Journals_Txns")
                    {
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle  ";
                        if (InAtmNo != "")
                        {
                            SqlString = "SELECT * "
                          + " FROM " + PhysicalFiledID
                          + " WHERE  Processed = 1 AND ProcessedAtRMCycle = @RMCycle  AND TerminalId =@TerminalId ";
                        }
                    }

                    if (InMode == 3 & InTable == "Atms_Journals_Txns")
                    {
                        // TERMINAL ID
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId AND IsMatchingDone = 1 "
                           + " AND MATCHED = 0 AND MatchingAtRMCycle = @RMCycle  ";
                    }
                    if (InMode == 3 & InTable != "Atms_Journals_Txns")
                    {
                        // TERMINAL ID
                        SqlString = "SELECT * "
                           + " FROM " + PhysicalFiledID
                           + " WHERE  TerminalId =@TerminalId "
                           + " AND Processed = 1 AND ProcessedAtRMCycle = @RMCycle  ";
                    }

                }
            }

            //if (In_DB_Mode == 2)
            //{
            //    SqlString =
            //   " WITH MergedTbl AS "
            //   + " ( "
            //   + " SELECT *  "
            //   + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
            //   + InSelectionCriteria // Includes Where
            //   + " UNION ALL  "
            //   + " SELECT * "
            //   + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
            //   + InSelectionCriteria // Includes Where
            //   + " ) "
            //   + " SELECT * FROM MergedTbl "
            //   + " ORDER BY TransDate "
            //    + "  ";
            //}

            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);


                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (InMode != 9 & InMode != 10)
            {
                // IF not BULK print 
                string DestinationFile;

                if (InTable != "Atms_Journals_Txns")
                {
                    DestinationFile =
                    "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

                }
                else
                {
                    DestinationFile =
                 "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

                }
                //
                //
                DeleteRecordsPerUser(DestinationFile, InSignedId);
                //
                InsertReport(DestinationFile, DataTableAllFields, InSignedId);
            }

        }

        //
        // GET TRANSACTIONS FOR ONE DATE
        //
        public void ReadTableAndFillTableForA_Date(string InSignedId, string InTable, string InMatchingCateg, DateTime InNet_TransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + Ms.InportTableName;
            PhysicalFileID_BULK = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + Ms.InportTableName;

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            SqlString = "SELECT * "
                         + " FROM " + PhysicalFiledID
                         + " WHERE  Net_TransDate = @Net_TransDate AND MatchingCateg = @MatchingCateg AND Processed = 0 ";



            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Net_TransDate", InNet_TransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);


                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }


            // IF not BULK print 
            string DestinationFile;


            DestinationFile =
         "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";
            //
            //
            DeleteRecordsPerUser(DestinationFile, InSignedId);
            //
            InsertReport(DestinationFile, DataTableAllFields, InSignedId);


        }
        //
        public decimal TotalDebit;
        public decimal TotalCredit;
        //
        public void ReadTrans_TXNS_FromSpecificTableBetween_Dates(string InSignedId, string InTable, string InTerminalId,
                                                         DateTime InDateFrom, DateTime InDateTo, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In Mode = 1 = both debits and credits 
            // In Mode = 2 Only withdrawls 
            // In Mode = 3 Only Deposits
            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Ms.InportTableName + "]";
            string PhysicalFiledID_Secondary = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Ms.InportTableName + "]";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            // DATA TABLE ROWS DEFINITION 
            if (In_DB_Mode == 1)
            {
                if (InMode == 1 & InTable == "Atms_Journals_Txns")
                {
                    SqlString = "SELECT * "
                  + " FROM " + PhysicalFiledID
                 + " Where TerminalId = @TerminalId "
                 + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                 + " AND NotInJournal = 0 "
                 + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'";

                }
                if (InMode == 1 & InTable != "Atms_Journals_Txns")
                {
                    // THIS EG FOR SWITCH 
                    SqlString = "SELECT * "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ORDER BY TransDate "
                      ;
                }
                if (InMode == 2 & InTable != "Atms_Journals_Txns")
                {
                    SqlString = "SELECT * "
                        + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ORDER BY TransDate "
                      ;
                }
                if (InMode == 3 & InTable != "Atms_Journals_Txns")
                {
                    SqlString = "SELECT * "
                        + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ORDER BY TransDate "
                      ;
                }
            }

            // FOR MERGED
            if (In_DB_Mode == 2)
            {
                if (InMode == 1 & InTable == "Atms_Journals_Txns")
                {

                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 2 & InTable == "Atms_Journals_Txns")
                {

                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 3 & InTable == "Atms_Journals_Txns")
                {

                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 23) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 23) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 8 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // See ALL Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                      + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                       + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 9 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // SEE all Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " Where TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                       + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                      + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 10 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // SEE all Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT *  "
                      + " FROM " + PhysicalFiledID
                      + " Where TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                       + " AND NotInJournal = 0 AND [Matched]= 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT * "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND TransDate > @DateFrom AND CAP_DATE = @DateTo  "
                      + " AND NotInJournal = 0 AND [Matched]= 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 11 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // See ALL Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                       + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate "
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND (CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom ) "
                      + " AND NotInJournal = 0  "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                        + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND (CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom ) "
                       + " AND NotInJournal = 0  "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 12 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // SEE all Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                       + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND ( CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom ) "
                       + " AND NotInJournal = 0  "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                        + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND (CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom ) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 1 & InTable != "Atms_Journals_Txns")
                {
                    // THIS EG FOR SWITCH 
                    //SqlString = "SELECT * "
                    //  + " FROM " + PhysicalFiledID
                    //  + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                    //  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                    //  + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                    //  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    //  + " ORDER BY TransDate "
                    //  ;
                    SqlString =
                   " WITH MergedTbl AS "
                     + " ( "
                     + " SELECT *  "
                     + " FROM " + PhysicalFiledID
                     + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                     + " UNION ALL  "
                     + " SELECT * "
                     + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                     + " ) "
                     + " SELECT * FROM MergedTbl "
                     + " ORDER BY TransDate ";
                }
                if (InMode == 2 & InTable != "Atms_Journals_Txns")
                {
                    //SqlString = "SELECT * "
                    //    + " FROM " + PhysicalFiledID
                    //  + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                    //  + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                    //  + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                    //  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    //  + " ORDER BY TransDate "
                    //  ;
                    SqlString =
                  " WITH MergedTbl AS "
                    + " ( "
                    + " SELECT *  "
                    + " FROM " + PhysicalFiledID
                    + " WHERE TerminalId = @TerminalId  "
                    + " AND (CAP_DATE <= @CAP_DATE  AND TransDate > @DateFrom  AND CAP_DATE<> '1900-01-01')  "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    + " UNION ALL  "
                    + " SELECT * "
                    + " FROM " + PhysicalFiledID_Secondary
                     + " WHERE TerminalId = @TerminalId  "
                     + " AND (CAP_DATE <= @CAP_DATE  AND TransDate > @DateFrom  AND CAP_DATE<> '1900-01-01')  "
                      + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    + " ) "
                    + " SELECT * FROM MergedTbl "
                    + " ORDER BY TransDate ";
                }
                if (InMode == 3 & InTable != "Atms_Journals_Txns")
                {
                    //SqlString = "SELECT * "
                    //    + " FROM " + PhysicalFiledID
                    //  + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @DateFrom AND @DateTo) "
                    //  + " AND ResponseCode = '0' AND TXNSRC = '1' "
                    //  + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                    //  + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    //  + " ORDER BY TransDate "
                    //  ;
                    SqlString =
                " WITH MergedTbl AS "
                  + " ( "
                  + " SELECT *  "
                  + " FROM " + PhysicalFiledID
                    + " WHERE TerminalId = @TerminalId  "
                    + " AND (CAP_DATE <= @CAP_DATE  AND TransDate > @DateFrom  AND CAP_DATE<> '1900-01-01')  "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                  + " UNION ALL  "
                  + " SELECT * "
                  + " FROM " + PhysicalFiledID_Secondary
                   + " WHERE TerminalId = @TerminalId  "
                   + " AND (CAP_DATE <= @CAP_DATE  AND TransDate > @DateFrom  AND CAP_DATE<> '1900-01-01')  "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23 ) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                  + " ) "
                  + " SELECT * FROM MergedTbl "
                  + " ORDER BY TransDate ";
                }
            }

            using (SqlConnection conn =
           new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InDateTo); // the same as CAP


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            string DestinationFile;

            if (InTable != "Atms_Journals_Txns")
            {
                DestinationFile =
                "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

                int TransType = 0;
                decimal TransAmt = 0;

                TotalDebit = 0;
                TotalCredit = 0;

                int I = 0;

                while (I <= (DataTableAllFields.Rows.Count - 1))
                {

                    TransType = (int)DataTableAllFields.Rows[I]["TransType"];

                    TransAmt = (decimal)DataTableAllFields.Rows[I]["TransAmt"];

                    if (TransType == 11)
                    {
                        TotalDebit = TotalDebit + TransAmt;
                    }
                    if (TransType == 23)
                    {
                        TotalCredit = TotalCredit + TransAmt;
                    }

                    I = I + 1;
                }


            }
            else
            {
                DestinationFile =
                 "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

                int TransType = 0;
                decimal TransAmount = 0;

                TotalDebit = 0;
                TotalCredit = 0;

                int I = 0;

                while (I <= (DataTableAllFields.Rows.Count - 1))
                {

                    TransType = (int)DataTableAllFields.Rows[I]["TransType"];

                    TransAmount = (decimal)DataTableAllFields.Rows[I]["TransAmount"];

                    if (TransType == 11)
                    {
                        TotalDebit = TotalDebit + TransAmount;
                    }
                    if (TransType == 23)
                    {
                        TotalCredit = TotalCredit + TransAmount;
                    }

                    I = I + 1;
                }

            }
            //
            //DeleteRecordsPerUser(DestinationFile, InSignedId);
            ////
            //InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }

        //
        public void ReadTrans_TXNS_FromSpecificTableBetween_Dates_Short(string InSignedId, string InTable, string InTerminalId,
                                                         DateTime InDateFrom, DateTime InDateTo, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string PhysicalFiledID_Secondary = ""; 

            // In Mode = 1 = both debits and credits 
            // In Mode = 2 Only withdrawls  
            // In Mode = 3 Only Deposits
            // In Mode = 11 Only withdrawls  up to Cap Date 
            // In Mode = 12 Only Deposits up to Cap Date  
            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            if (InTable == "tblMatchingTxnsMasterPoolATMs")
            {
                PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + InTable + "]";
                PhysicalFiledID_Secondary = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + InTable + "]";
            }
            else
            {
                Ms.ReadReconcSourceFilesByFileId(InTable);

                PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Ms.InportTableName + "]";
                PhysicalFiledID_Secondary = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Ms.InportTableName + "]";

            }

            
            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            // DATA TABLE ROWS DEFINITION 


            // FOR MERGED
            if (In_DB_Mode == 2)
            {
                if (InMode == 1 & InTable == "Atms_Journals_Txns")
                {
                    // Both 11 and 23
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'  "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 OR TransType = 23) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 2 & InTable == "Atms_Journals_Txns")
                {
                    //  Only 11 
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 11 ) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 3 & InTable == "Atms_Journals_Txns")
                {
                    // Only 23 
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 23) AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                      + " AND NotInJournal = 0 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND (TransType = 23 ) AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 1 & InTable != "Atms_Journals_Txns")
                {
                    // Both 11 and 23
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                         + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                          + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 2 & InTable != "Atms_Journals_Txns")
                {
                    // Only 11
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                       //+ " WHERE TerminalId = @TerminalId "
                       //+ " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                       //    + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                       //+ " AND Comment <> 'Reversals' AND (TransType = 11) "
                       //+ " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Where TerminalId = @TerminalId AND (TransDate <= @DateTo "
                          + " AND TransDate > @DateFrom "
                           + " AND CAP_DATE<> '1900-01-01' ) "
                             //  + " AND NotInJournal = 0 "
                             + " AND ResponseCode = '0' AND TXNSRC = '1'"
                              + " AND (TransType = 11 ) AND Comment <> 'Reversals' "
                               + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                       //+ " Where TerminalId = @TerminalId "
                       //+ " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                       //    + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                       //+ " AND Comment <> 'Reversals' AND (TransType = 11) "
                       //+ " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                       + " Where TerminalId = @TerminalId AND ( TransDate <= @DateTo "
                          + " AND TransDate > @DateFrom "
                            + " AND CAP_DATE<> '1900-01-01' ) "
                              //  + " AND NotInJournal = 0 "
                              + " AND ResponseCode = '0' AND TXNSRC = '1' "
                              + "AND (TransType = 11 ) AND Comment <> 'Reversals' "
                               + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";

                  
                }

                if (InMode == 3 & InTable != "Atms_Journals_Txns")
                {
                    // Only 23
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                          + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) "
                         + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "

                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 11 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // See ALL Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                      + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                       + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                       + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 11 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 12 & InTable == "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // SEE all Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                       + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " UNION ALL  "
                       + " SELECT  SeqNo, TerminalId, TraceNoWithNoEndZero as Trace, TransDescr, TransAmount, TransDate"
                      + "  ,CardNumber, AccNumber,MatchMask ,MatchingCateg,TransType ,TXNDEST, Fuid as JrnlId    "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                      + " AND NotInJournal = 0 AND [Matched]= 1 "
                      + " AND ResponseCode = '0' AND TXNSRC = '1' AND TransType = 23 AND Origin = 'Our Atms'"
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 11 & InTable != "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // See ALL Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                    + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                          + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                       + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                         + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 11) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 12 & InTable != "Atms_Journals_Txns")
                {
                    // Based on CAP_DATE
                    // SEE all Matched
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                         + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " WHERE TerminalId = @TerminalId AND CAP_DATE <= @CAP_DATE "
                      + " AND TransDate > @DateFrom "
                        + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      + " AND Comment <> 'Reversals' AND (TransType = 23) "
                      + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                      + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }
                if (InMode == 14 & InTable != "Atms_Journals_Txns")
                {
                    // FOREX TRANSACTIONS 
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr,TransCurr ,TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      //   + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      //+ " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      //+ " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransCurr ,TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      //    + " AND ResponseCode = '0'  AND TXNSRC = '1' "
                      //+ " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                      //+ " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                      + " ) "
                      + " SELECT * FROM MergedTbl "
                    //  + " WHERE TransDescr <> 'FOREX_DEPOSIT'  "
                      + " ORDER By TransDate  ";
                }

                if (InMode == 15 & InTable != "Atms_Journals_Txns")
                {

                    // Checking for FOREX -- at replenishement workflow
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr,TransCurr ,TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId, TraceNo as Trace, TransDescr, TransCurr ,TransAmt, TransDate"
                     + "  ,CardNumber, AccNo ,MatchingCateg,TransType ,TXNDEST    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      + " ) "
                      + " SELECT TransCurr As Ccy, Sum(TransAmt) as TotalAmt, count(*) As NumberOfTxns"
                      + " FROM MergedTbl "
                      + " Group By TransCurr  ";
                }

                if (InMode == 16 & InTable != "Atms_Journals_Txns")
                {

                    // Checking for FOREX -- at replenishement workflow in master pool 
                    SqlString =
                    " WITH MergedTbl AS "
                      + " ( "
                      + " SELECT  SeqNo, TerminalId, TransDate"
                     + "     "
                      + "  "
                      + " FROM " + PhysicalFiledID
                      + " WHERE TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      + " UNION ALL  "
                      + " SELECT  SeqNo, TerminalId , TransDate "
                     + "    "
                      + "  "
                      + " FROM " + PhysicalFiledID_Secondary
                      + " Where TerminalId = @TerminalId "
                      + " AND (TransDate BETWEEN @DateFrom AND @DateTo) AND TransType = 23 AND MatchingCateg =  'BDC209' "
                      + " ) "
                      + " SELECT * "
                      + " FROM MergedTbl "
                      + "  ";
                }

                ///

            }

            

                    using (SqlConnection conn =
           new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InDateTo); // the same as CAP


                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 250;   // seconds
                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (InMode == 14 || InMode == 15)
            {
                // THIS IS FOR FOREX 
                return; 
            }
            string DestinationFile;

            if (InTable != "Atms_Journals_Txns")
            {
                DestinationFile =
                "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

                int TransType = 0;
                decimal TransAmt = 0;

                TotalDebit = 0;
                TotalCredit = 0;

                int I = 0;

                while (I <= (DataTableAllFields.Rows.Count - 1))
                {

                    TransType = (int)DataTableAllFields.Rows[I]["TransType"];

                    TransAmt = (decimal)DataTableAllFields.Rows[I]["TransAmt"];

                    if (TransType == 11)
                    {
                        TotalDebit = TotalDebit + TransAmt;
                    }
                    if (TransType == 23)
                    {
                        TotalCredit = TotalCredit + TransAmt;
                    }

                    I = I + 1;
                }


            }
            else
            {
                DestinationFile =
                 "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

                int TransType = 0;
                decimal TransAmount = 0;

                TotalDebit = 0;
                TotalCredit = 0;

                int I = 0;

                while (I <= (DataTableAllFields.Rows.Count - 1))
                {

                    TransType = (int)DataTableAllFields.Rows[I]["TransType"];

                    TransAmount = (decimal)DataTableAllFields.Rows[I]["TransAmount"];

                    if (TransType == 11)
                    {
                        TotalDebit = TotalDebit + TransAmount;
                    }
                    if (TransType == 23)
                    {
                        TotalCredit = TotalCredit + TransAmount;
                    }

                    I = I + 1;
                }

            }
            //
            //if (InMode != 1)
            //{
            //    DeleteRecordsPerUser(DestinationFile, InSignedId);
            //    //
            //    InsertReport(DestinationFile, DataTableAllFields, InSignedId);
            //}

        }

        public void ReadTrans_TXNS_FromSpecificTableForCurrent_Cap_Date_UpTo_Date_Time(string InSignedId, string InTable, string InTerminalId,
                                                                   DateTime InCAP_DATE, DateTime InDateTo, int InMode, int In_DB_Mode)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // In Mode = 4 = both debits and credits 
            // In Mode = 5 Only withdrawls 
            // In Mode = 6 Only Deposits
            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Ms.InportTableName + "]";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            TotalWithdrawls = 0;
            TotalDeposits = 0;

            if (In_DB_Mode == 1)
            {
                if (InMode == 4)
                {
                    SqlString = "SELECT * "
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                     + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                     ;

                }
                if (InMode == 5)
                {
                    SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                     + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                     ;
                }
                if (InMode == 6)
                {
                    SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                     + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                     ;
                }
            }

            if (In_DB_Mode == 2)
            {
                if (InMode == 4)
                {
                    //SqlString = "SELECT * "
                    // + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                    // + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                    // + " AND TransDate < @DateTo "
                    // + " AND ResponseCode = '0' AND TXNSRC = '1' "
                    // + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                    // + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    // ;

                    SqlString =
         " WITH MergedTbl AS "
         + " ( "
         + " SELECT *  "
         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
         + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
         + " UNION ALL  "
         + " SELECT * "
         + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
          + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 OR TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
         + " ) "
         + " SELECT * FROM MergedTbl "
          // + " ORDER BY SeqNo DESC "
          + "  ";

                }
                if (InMode == 5)
                {
                    //SqlString = "SELECT * "
                    //  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                    // + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                    // + " AND TransDate < @DateTo "
                    // + " AND ResponseCode = '0' AND TXNSRC = '1' "
                    // + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                    // + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    // ;

                    SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
 + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
+ " UNION ALL  "
+ " SELECT * "
+ " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
 + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 11 ) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
+ " ) "
+ " SELECT * FROM MergedTbl "
// + " ORDER BY SeqNo DESC "
+ "  ";
                }
                if (InMode == 6)
                {
                    //SqlString = "SELECT * "
                    //  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                    // + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                    // + " AND TransDate < @DateTo "
                    // + " AND ResponseCode = '0' AND TXNSRC = '1' "
                    // + " AND Comment <> 'Reversals' AND (TransType = 23) "
                    // + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
                    // ;

                    SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
+ " UNION ALL  "
+ " SELECT * "
+ " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
  + " Where TerminalId = @TerminalId AND CAP_DATE = @CAP_DATE "
                     + " AND TransDate < @DateTo "
                     + " AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " AND Comment <> 'Reversals' AND (TransType = 23) "
                     + " AND TransDescr <>'810022-FAWRY' AND TransDescr <>'810012-FAWRY' "
+ " ) "
+ " SELECT * FROM MergedTbl "
// + " ORDER BY SeqNo DESC "
+ "  ";
                }
            }


            using (SqlConnection conn =
           new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            string DestinationFile;

            if (InTable != "Atms_Journals_Txns")
            {
                DestinationFile =
                "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

            }
            else
            {
                DestinationFile =
             "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            }
            //
            DeleteRecordsPerUser(DestinationFile, InSignedId);
            //
            InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }


        // Methods 
        // READ Reversals  and fill memory table
        //

        public void ReadTableAndFillTableWithReversals(string InTable, DateTime InFromDate, DateTime InToDate, string InSignedId
                                     , string InTerminalId, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 : All for range of dates

            // InMode = 3 : Per ATM 

            // Find Physical Name


            //  PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + InTable + "]";

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();

            if (In_DB_Mode == 1)
            {
                if (InMode == 1)
                {
                    SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                                          + " WHERE  "
                                          + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                }
                if (InMode == 3)
                {
                    SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                                          + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
                                          + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                }
                if (InMode == 4)
                {

                    SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                                          + " WHERE  "
                                          + "("
                                               //+" TransDescr_2 = '210000-BNA' OR TransDescr_2 = '214000-BNA' "
                                               // + " OR TransDescr_2 = '210001-BNA' OR TransDescr_2 = '213000-BNA'"
                                               //    + " OR TransDescr_2 = '213001-BNA' OR TransDescr_2 = '214000-BNA'"
                                               + " left([TransDescr_2], 2) = '21' "  // ALL DEPOSITS START WITH 21
                                          + " )"
                                            + " and FileId in ( 'Switch_IST_Txns_2' )"
                                          //  + " and ResponseCode_2 = 0 and ResponseCode_4 = 0 "
                                          + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                }


            }

            if (In_DB_Mode == 2)
            {
                if (InMode == 1)
                {
                    //SqlString = "SELECT * "
                    //                      + " FROM " + PhysicalFiledID
                    //                      + " WHERE  "
                    //                      + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";

                    SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
 + " WHERE  "
 + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
+ " UNION ALL  "
+ " SELECT * "
+ " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTable
+ " WHERE  "
 + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
+ " ) "
+ " SELECT * FROM MergedTbl "
// + " ORDER BY SeqNo DESC "
+ "  ";
                }
                if (InMode == 3)
                {
                    //SqlString = "SELECT * "
                    //                      + " FROM " + PhysicalFiledID
                    //                      + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
                    //                      + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";


                    SqlString =
    " WITH MergedTbl AS "
    + " ( "
    + " SELECT *  "
    + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
     + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
    + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
    + " UNION ALL  "
    + " SELECT * "
    + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTable
     + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
    + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
    + " ) "
    + " SELECT * FROM MergedTbl "
    // + " ORDER BY SeqNo DESC "
    + "  ";
                }
            }

            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDateFrom", InFromDate.Date);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDateTo", InToDate.Date);
                        sqlAdapt.SelectCommand.CommandTimeout = 350;
                        if (InMode == 3)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (InMode == 4 || InMode == 5)
            {
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

                DataTableSelectedFields = new DataTable();
                DataTableSelectedFields.Clear();
                // Read and find out if in Mpa

                DataTableSelectedFields.Columns.Add("SeqNo", typeof(int));
                DataTableSelectedFields.Columns.Add("Fuid", typeof(int));
                DataTableSelectedFields.Columns.Add("ATMNo", typeof(string));
                DataTableSelectedFields.Columns.Add("TRACE", typeof(string));
                DataTableSelectedFields.Columns.Add("Amount", typeof(string));
                DataTableSelectedFields.Columns.Add("DateTime", typeof(string));
                DataTableSelectedFields.Columns.Add("IsJournal_Loaded?", typeof(string));
                DataTableSelectedFields.Columns.Add("IN_Journal", typeof(string));
                DataTableSelectedFields.Columns.Add("Mpa_SeqNo", typeof(int));
                DataTableSelectedFields.Columns.Add("IsMatchingDone?", typeof(string));
                DataTableSelectedFields.Columns.Add("MASK", typeof(string));
                DataTableSelectedFields.Columns.Add("IST_RespCode", typeof(string));


                // Make Loop 

                int I = 0;
                // We have read all 
                while (I <= (DataTableAllFields.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int WSeqNo = (int)DataTableAllFields.Rows[I]["SeqNo"];
                    string WMatchingCateg = (string)DataTableAllFields.Rows[I]["MatchingCateg"];
                    string WTerminalId_2 = (string)DataTableAllFields.Rows[I]["TerminalId_2"];
                    int WTraceNo_2 = (int)DataTableAllFields.Rows[I]["TraceNo_2"];
                    decimal WTransAmt_2 = (decimal)DataTableAllFields.Rows[I]["TransAmt_2"];
                    DateTime WTransDate_2 = (DateTime)DataTableAllFields.Rows[I]["TransDate_2"];
                    string ResponseCode_2 = (string)DataTableAllFields.Rows[I]["ResponseCode_2"];

                    // Fill Table 
                    //if (WTerminalId_2 == "00000995")
                    // {
                    //     MessageBox.Show("THis is the one"); 
                    // }

                    DataRow RowSelected = DataTableSelectedFields.NewRow();

                    RowSelected["SeqNo"] = WSeqNo;

                    RowSelected["ATMNo"] = WTerminalId_2;
                    RowSelected["TRACE"] = WTraceNo_2;
                    RowSelected["Amount"] = WTransAmt_2;
                    RowSelected["DateTime"] = WTransDate_2;


                    string WSelectionCriteria = "where Left(FileName, 17) = '" + WTerminalId_2 + "_"
                            + WTransDate_2.ToString("yyyyMMdd") + "'";

                    Rfm.ReadLoadedFilesAND_Find_If_Loaded(WSelectionCriteria);

                    if (Rfm.RecordFound == true)
                    {

                        RowSelected["Fuid"] = Rfm.stpFuid;
                        RowSelected["IsJournal_Loaded?"] = "YES";
                    }
                    else
                    {
                        RowSelected["Fuid"] = 0;
                        RowSelected["IsJournal_Loaded?"] = "NO";
                    }

                    int MpaSeqNo = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversals
                   (WMatchingCateg, WTerminalId_2, WTraceNo_2, WTransAmt_2, WTransDate_2);

                    if (Mpa.RecordFound == true)
                    {
                        if (Mpa.NotInJournal == true)
                        {
                            RowSelected["IN_Journal"] = "Not found In Journal";
                        }
                        else
                        {
                            RowSelected["IN_Journal"] = "Found In Journal";
                        }

                        RowSelected["Mpa_SeqNo"] = MpaSeqNo;
                        RowSelected["MASK"] = Mpa.MatchMask;

                        if (Mpa.IsMatchingDone)
                        {
                            if (Mpa.NotInJournal == true)
                            {
                                // Not found in Journal
                                RowSelected["IsMatchingDone?"] = "YES_At Cycle:." + Mpa.MatchingAtRMCycle + "..and record Created";

                            }
                            else
                            {
                                // Found in Journal
                                RowSelected["IsMatchingDone?"] = "YES_At Cycle:." + Mpa.MatchingAtRMCycle + "..and record read from Journal";
                            }

                        }
                        else
                        {
                            RowSelected["IsMatchingDone?"] = "Not Done YET_Wait_For_NextJournal";

                            //  DateTime LastDateTime = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversalsLastMatchedDate
                            //(WMatchingCateg, WTerminalId_2, WTransDate_2);
                            //  if (Mpa.RecordFound == true)
                            //  {
                            //      if (WTransDate_2 > LastDateTime)
                            //      {
                            //          RowSelected["IsMatchingDone?"] = "No for this category YET. Last Date of Matching.." + LastDateTime.ToString();
                            //      }
                            //      else
                            //      {
                            //          RowSelected["IsMatchingDone?"] = "YES..for this category last date of matching" + LastDateTime.ToString(); ;
                            //      }
                            //  }
                        }


                    }
                    else
                    {
                        RowSelected["IN_Journal"] = "Not found In Journal";
                        RowSelected["Mpa_SeqNo"] = 0;
                        RowSelected["MASK"] = "";

                        RowSelected["IsMatchingDone?"] = "No for this category YET.";
                        // Find the last Txn Matching was done 
                        //DateTime LastDateTime = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversalsLastMatchedDate
                        //   (WMatchingCateg, WTerminalId_2, WTransDate_2);
                        //if (Mpa.RecordFound == true)
                        //{
                        //    if (WTransDate_2 > LastDateTime)
                        //    {
                        //        RowSelected["IsMatchingDone?"] = "No for this category YET. Last Date of Matching.." + LastDateTime.ToString();
                        //    }
                        //    else
                        //    {
                        //        RowSelected["IsMatchingDone?"] = "YES..for this category last date of matching" + LastDateTime.ToString(); ;
                        //    }
                        //}

                        //if (Rfm.RecordFound == true)
                        //{
                        //    RowSelected["IsMatchingDone?"] = "YES";
                        //}
                        //else
                        //{
                        //    RowSelected["IsMatchingDone?"] = "No Jnl not loaded";
                        //}

                    }
                    if (ResponseCode_2 == "0")
                    {
                        RowSelected["IST_RespCode"] = "Good Response";
                    }
                    else
                    {
                        RowSelected["IST_RespCode"] = "NoGood Response";
                    }

                    DataTableSelectedFields.Rows.Add(RowSelected);

                    I = I + 1;
                }



            }
            //string DestinationFile;
            ////   DestinationFile =
            ////"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            //DestinationFile =
            //       "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";
            ////
            //DeleteRecordsPerUser(DestinationFile, InSignedId);
            ////
            //InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }


        // Methods 
        // READ Reversals  and fill memory table
        //

        public void ReadTableAndFillTableWithDepositReversals(string InMatchingCateg, string InTable
                                     , DateTime InTransDate, string InTerminalId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 2 = IST = WMatchingTbl_02
            // InMode = 3 then Flexcube = WMatchingTbl_03

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            SqlString = "SELECT * "
                                             + " FROM " + InTable
                                              + " WHERE "
                                              + " Processed = 0 "
                                              + " AND "
                                              + " [MatchingCateg] = @MatchingCateg "
                                              + " AND [TerminalId] = @TerminalId "
                                              //+ " AND [ResponseCode] = '0'   "
                                              + " AND [TransDate] <= @TransDate "
                                              + " AND Comment  = 'Reversals' and Left(FullTraceNo,1) = '2' "
                                              + " AND TransType = 23 and TXNSRC = '1' "
                                                ;


            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);


                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            // Make Loop 

            int I = 0;
            // We have read all Actions Occurances 
            while (I <= (DataTableAllFields.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)DataTableAllFields.Rows[I]["SeqNo"];
                string WMatchingCateg = (string)DataTableAllFields.Rows[I]["MatchingCateg"];
                string WTerminalId = (string)DataTableAllFields.Rows[I]["TerminalId"];
                int WTraceNo = (int)DataTableAllFields.Rows[I]["TraceNo"];
                decimal WTransAmt = (decimal)DataTableAllFields.Rows[I]["TransAmt"];
                DateTime WTransDate = (DateTime)DataTableAllFields.Rows[I]["TransDate"];
                string ResponseCode = (string)DataTableAllFields.Rows[I]["ResponseCode"];

                int MpaSeqNo = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversals
                                   (WMatchingCateg, WTerminalId, WTraceNo, WTransAmt, WTransDate);

                if (Mpa.RecordFound == true)
                {
                    // Delete IST record from second working table 
                    if (InMode == 2)
                    {
                        DeleteRecordFromISTWorkingTable(WSeqNo, "WMatchingTbl_02");
                    }
                    if (InMode == 3)
                    {
                        DeleteRecordFromISTWorkingTable(WSeqNo, "WMatchingTbl_03");
                    }
                }

                I = I + 1;
            }




            //string DestinationFile;
            ////   DestinationFile =
            ////"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            //DestinationFile =
            //       "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";
            ////
            //DeleteRecordsPerUser(DestinationFile, InSignedId);
            ////
            //InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }
        public void DeleteRecordFromISTWorkingTable(int InSeqNo, string InWorkingTable)
        {
            //[WMatchingTbl_02]
            int DelCount = 0;
            string SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo]." + InWorkingTable
                + "  WHERE SeqNo = @SeqNo   "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.CommandTimeout = 350;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    // CatchDetails(ex);
                }
        }


        public void ReadTableAndFillTableWithDepositReversals_MULTI(string InMatchingCateg, string InTable
                                     , DateTime InTransDate, string InTerminalId, int InMode, string InTempFileName02, string InTempFileName03)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 2 = IST = WMatchingTbl_02
            // InMode = 3 then Flexcube = WMatchingTbl_03

            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            SqlString = "SELECT * "
                                             + " FROM " + InTable
                                              + " WHERE "
                                              + " Processed = 0 "
                                              + " AND "
                                              + " [MatchingCateg] = @MatchingCateg "
                                              + " AND [TerminalId] = @TerminalId "
                                              //+ " AND [ResponseCode] = '0'   "
                                              + " AND [TransDate] <= @TransDate "
                                              + " AND Comment  = 'Reversals' and Left(FullTraceNo,1) = '2' "
                                              + " AND TransType = 23 and TXNSRC = '1' "
                                                ;


            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);


                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            // Make Loop 

            int I = 0;
            // We have read all Actions Occurances 
            while (I <= (DataTableAllFields.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)DataTableAllFields.Rows[I]["SeqNo"];
                string WMatchingCateg = (string)DataTableAllFields.Rows[I]["MatchingCateg"];
                string WTerminalId = (string)DataTableAllFields.Rows[I]["TerminalId"];
                int WTraceNo = (int)DataTableAllFields.Rows[I]["TraceNo"];
                decimal WTransAmt = (decimal)DataTableAllFields.Rows[I]["TransAmt"];
                DateTime WTransDate = (DateTime)DataTableAllFields.Rows[I]["TransDate"];
                string ResponseCode = (string)DataTableAllFields.Rows[I]["ResponseCode"];

                int MpaSeqNo = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversals
                                   (WMatchingCateg, WTerminalId, WTraceNo, WTransAmt, WTransDate);

                if (Mpa.RecordFound == true)
                {
                    // Delete IST record from second working table 
                    if (InMode == 2)
                    {
                        DeleteRecordFromISTWorkingTable_MULTI(WSeqNo, InTempFileName02);
                    }
                    if (InMode == 3)
                    {
                        DeleteRecordFromISTWorkingTable_MULTI(WSeqNo, InTempFileName03);
                    }
                }

                I = I + 1;
            }




            //string DestinationFile;
            ////   DestinationFile =
            ////"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            //DestinationFile =
            //       "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";
            ////
            //DeleteRecordsPerUser(DestinationFile, InSignedId);
            ////
            //InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }
        public void DeleteRecordFromISTWorkingTable_MULTI(int InSeqNo, string InWorkingTable)
        {
            //[WMatchingTbl_02]
            int DelCount = 0;
            string SQLCmd = "DELETE FROM " + InWorkingTable
                + "  WHERE SeqNo = @SeqNo   "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.CommandTimeout = 350;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    // CatchDetails(ex);
                }
        }


        public void ReadTableAndFillTableWithReversalsSeqNo_Second_Table(string InTable, string InSignedId
                                      , int InSeqNo, int InMode, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 1 : All for range of dates

            // InMode = 3 : Per ATM 

            // Find Physical Name


            //  PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + InTable + "]";

            DataTableAllFields_SeqNo = new DataTable();
            DataTableAllFields_SeqNo.Clear();

            if (In_DB_Mode == 1)
            {
                //if (InMode == 1)
                //{
                //    SqlString = "SELECT * "
                //                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                //                          + " WHERE  "
                //                          + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                //}
                //if (InMode == 3)
                //{
                //    SqlString = "SELECT * "
                //                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                //                          + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
                //                          + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                //}
                //if (InMode == 4)
                //{
                //    SqlString = "SELECT * "
                //                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                //                          + " WHERE  TransDescr_2 = '210000-BNA' "
                //                            + " and FileId = 'Switch_IST_Txns_2' "
                //                           + " and ResponseCode_2 = 0 and ResponseCode_4 = 0 "
                //                          + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";
                //}

                if (InMode == 1)
                {
                    SqlString = "SELECT * "
                                         + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
                                          + " WHERE  "
                                          + " SeqNo = @SeqNo ";
                }
            }

            //            if (In_DB_Mode == 2)
            //            {
            //                if (InMode == 1)
            //                {
            //                    //SqlString = "SELECT * "
            //                    //                      + " FROM " + PhysicalFiledID
            //                    //                      + " WHERE  "
            //                    //                      + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";

            //                    SqlString =
            //" WITH MergedTbl AS "
            //+ " ( "
            //+ " SELECT *  "
            //+ " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
            // + " WHERE  "
            // + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
            //+ " UNION ALL  "
            //+ " SELECT * "
            //+ " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTable
            //+ " WHERE  "
            // + " (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
            //+ " ) "
            //+ " SELECT * FROM MergedTbl "
            //// + " ORDER BY SeqNo DESC "
            //+ "  ";
            //                }
            //                if (InMode == 3)
            //                {
            //                    //SqlString = "SELECT * "
            //                    //                      + " FROM " + PhysicalFiledID
            //                    //                      + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
            //                    //                      + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) ";


            //                    SqlString =
            //    " WITH MergedTbl AS "
            //    + " ( "
            //    + " SELECT *  "
            //    + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTable
            //     + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
            //    + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
            //    + " UNION ALL  "
            //    + " SELECT * "
            //    + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTable
            //     + " WHERE  (TerminalId_4 = @TerminalId and TerminalId_2 = @TerminalId) "
            //    + " AND (Cast(TransDate_2 As Date) >=@TransDateFrom AND Cast(TransDate_2 As Date) <=@TransDateTo) "
            //    + " ) "
            //    + " SELECT * FROM MergedTbl "
            //    // + " ORDER BY SeqNo DESC "
            //    + "  ";
            //                }
            //     }

            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDateFrom", InFromDate.Date);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDateTo", InToDate.Date);
                        //if (InMode == 3)
                        //{
                        //    sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        //}
                        if (InMode == 1)
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllFields_SeqNo);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            if (InMode == 4 || InMode == 1)
            {
                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

                DataTableSelectedFields = new DataTable();
                DataTableSelectedFields.Clear();
                // Read and find out if in Mpa

                DataTableSelectedFields.Columns.Add("SeqNo", typeof(int));
                DataTableSelectedFields.Columns.Add("Fuid", typeof(int));
                DataTableSelectedFields.Columns.Add("ATMNo", typeof(string));
                DataTableSelectedFields.Columns.Add("TRACE", typeof(string));
                DataTableSelectedFields.Columns.Add("Amount", typeof(string));
                DataTableSelectedFields.Columns.Add("DateTime", typeof(string));
                DataTableSelectedFields.Columns.Add("IsJournal_Loaded?", typeof(string));
                DataTableSelectedFields.Columns.Add("IN_Journal", typeof(string));
                DataTableSelectedFields.Columns.Add("Mpa_SeqNo", typeof(int));
                DataTableSelectedFields.Columns.Add("IsMatchingDone?", typeof(string));
                DataTableSelectedFields.Columns.Add("MASK", typeof(string));

                // Make Loop 
                try
                {
                    int I = 0;
                    // We have read all Actions Occurances 
                    while (I <= (DataTableAllFields_SeqNo.Rows.Count - 1))
                    {
                        //    RecordFound = true;
                        int WSeqNo = (int)DataTableAllFields_SeqNo.Rows[I]["SeqNo"];
                        string WMatchingCateg = (string)DataTableAllFields.Rows[I]["MatchingCateg"];
                        string WTerminalId_2 = (string)DataTableAllFields_SeqNo.Rows[I]["TerminalId_2"];
                        int WTraceNo_2 = (int)DataTableAllFields_SeqNo.Rows[I]["TraceNo_2"];
                        decimal WTransAmt_2 = (decimal)DataTableAllFields_SeqNo.Rows[I]["TransAmt_2"];
                        DateTime WTransDate_2 = (DateTime)DataTableAllFields_SeqNo.Rows[I]["TransDate_2"];

                        // Fill Table 

                        DataRow RowSelected = DataTableSelectedFields.NewRow();

                        RowSelected["SeqNo"] = WSeqNo;

                        RowSelected["ATMNo"] = WTerminalId_2;
                        RowSelected["TRACE"] = WTraceNo_2;
                        RowSelected["Amount"] = WTransAmt_2;
                        RowSelected["DateTime"] = WTransDate_2;

                        string WSelectionCriteria = "where Left(FileName, 17) = '" + WTerminalId_2 + "_"
                                + WTransDate_2.ToString("yyyyMMdd") + "'";

                        Rfm.ReadLoadedFilesAND_Find_If_Loaded(WSelectionCriteria);

                        if (Rfm.RecordFound == true)
                        {

                            RowSelected["Fuid"] = Rfm.stpFuid;
                            RowSelected["IsJournal_Loaded?"] = "YES";
                        }
                        else
                        {
                            RowSelected["Fuid"] = 0;
                            RowSelected["IsJournal_Loaded?"] = "NO";
                        }

                        int MpaSeqNo = Mpa.ReadInPoolTransSpecificToSeeIfDepositForReversals
                       (WMatchingCateg, WTerminalId_2, WTraceNo_2, WTransAmt_2, WTransDate_2);

                        if (Mpa.RecordFound == true)
                        {
                            RowSelected["IN_Journal"] = "YES In Journal";
                            RowSelected["Mpa_SeqNo"] = MpaSeqNo;
                            if (Mpa.IsMatchingDone)
                            {
                                RowSelected["IsMatchingDone?"] = "YES";
                            }
                            else
                            {
                                RowSelected["IsMatchingDone?"] = "No";
                            }

                            RowSelected["MASK"] = Mpa.MatchMask;
                        }
                        else
                        {
                            RowSelected["IN_Journal"] = "NOT In Journal";
                            RowSelected["Mpa_SeqNo"] = 0;
                            RowSelected["MASK"] = "";
                            RowSelected["IsMatchingDone?"] = "N/A";


                        }

                        DataTableSelectedFields.Rows.Add(RowSelected);

                        I = I + 1;
                    }
                }
                catch (Exception ex)
                {
                    CatchDetails(ex);
                }

            }
            //string DestinationFile;
            ////   DestinationFile =
            ////"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            //DestinationFile =
            //       "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";
            ////
            //DeleteRecordsPerUser(DestinationFile, InSignedId);
            ////
            //InsertReport(DestinationFile, DataTableAllFields, InSignedId);
        }


        // Methods 
        // READ Physical table and fill memory table
        //
        public string LogicalFiledID;

        public int CountAll_IST_112;
        public int CountAll_IST_Non_Presenter;
        public int CountPresenter;

        DataTable Tbl_IST_112 = new DataTable();

        public void ReadTableAndFillTableWithPresenter(string InTable,
                                   int InRMCycle, string InSignedId, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // READ ALL IST WITH 112 and check in Mpa

            CountAll_IST_112 = 0;
            CountAll_IST_Non_Presenter = 0;
            CountPresenter = 0;

            LogicalFiledID = InTable;

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            string PhysicalFiledID_1 = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Ms.InportTableName + "]";
            string PhysicalFiledID_2 = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Ms.InportTableName + "]";

            // FIRST TABLE
            Tbl_IST_112 = new DataTable();
            Tbl_IST_112.Clear();

            Tbl_IST_112.Columns.Add("SeqNo", typeof(int));
            //Tbl_IST_112.Columns.Add("Amount", typeof(string));
            //Tbl_IST_112.Columns.Add("DR/CR", typeof(string));

            // SECOND TABLE
            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            // DATA TABLE ROWS DEFINITION 
            DataTableAllFields.Columns.Add("SeqNo", typeof(int));
            DataTableAllFields.Columns.Add("Done", typeof(string));
            DataTableAllFields.Columns.Add("Amount", typeof(string));
            DataTableAllFields.Columns.Add("DR/CR", typeof(string));

            DataTableAllFields.Columns.Add("IsInMaster", typeof(bool));
            DataTableAllFields.Columns.Add("IsInJournal", typeof(bool));
            DataTableAllFields.Columns.Add("IsPresenter", typeof(bool));
            DataTableAllFields.Columns.Add("MASK", typeof(string));
            DataTableAllFields.Columns.Add("TerminalId", typeof(string));
            DataTableAllFields.Columns.Add("Descr", typeof(string));
            DataTableAllFields.Columns.Add("Date", typeof(string));
            DataTableAllFields.Columns.Add("Response", typeof(string));


            if (In_DB_Mode == 1)
            {
                SqlString = "SELECT * "
                     + " FROM " + PhysicalFiledID_1
                     + " WHERE  ResponseCode = '112' and "
                     + " ProcessedAtRMCycle = @RMCycle ";
            }
            if (In_DB_Mode == 2)
            {

                SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM " + PhysicalFiledID_1
   + " WHERE   "
   + " ProcessedAtRMCycle = @RMCycle AND TXNSRC = '1' AND ResponseCode = '112' AND Comment <> 'Reversals' AND Left(Comment, 12) <> 'Aging_Record' "
+ " UNION ALL  "
+ " SELECT * "
+ " FROM " + PhysicalFiledID_2
 + " WHERE   "
   + " ProcessedAtRMCycle = @RMCycle AND TXNSRC = '1' AND ResponseCode = '112' AND Comment <> 'Reversals' AND Left(Comment, 12) <> 'Aging_Record' "
+ " ) "
+ " SELECT * FROM MergedTbl "
// + " ORDER BY SeqNo DESC "
+ "  ";
            }
            using (SqlConnection conn =
             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            CountAll_IST_112 = CountAll_IST_112 + 1;

                            DataRow RowSelected = Tbl_IST_112.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            // ADD ROW
                            Tbl_IST_112.Rows.Add(RowSelected);

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

            // READ TABLE AND ........

            int InWhatFile;

            int I = 0;

            while (I <= (Tbl_IST_112.Rows.Count - 1))
            {

                int WSeqNo = (int)Tbl_IST_112.Rows[I]["SeqNo"];

                // Find where it is 
                // Look at the first
                ReadTransSpecificFromSpecificTable_By_SeqNo_2(PhysicalFiledID_1, WSeqNo, 1);
                if (RecordFound == true)
                {
                    // It is at the first
                    InWhatFile = 1;
                }
                else
                {
                    // It is at the second 
                    InWhatFile = 2;
                    ReadTransSpecificFromSpecificTable_By_SeqNo_2(PhysicalFiledID_2, WSeqNo, 1);

                }

                DataRow RowSelected = DataTableAllFields.NewRow();

                RowSelected["SeqNo"] = SeqNo;

                RowSelected["Amount"] = TransAmt.ToString("#,##0.00");
                // FOR FIRST ENTRY 
                switch (TransType)
                {
                    case 11:
                        {
                            RowSelected["DR/CR"] = "DR";

                            break;
                        }
                    case 23:
                        {
                            RowSelected["DR/CR"] = "CR";
                            break;
                        }
                    case 33:
                        {
                            //03_REMAINING 00000071 13-10-2019
                            RowSelected["DR/CR"] = "TR";
                            break;
                        }
                    default:
                        {
                            //MessageBox.Show("Not defined ");
                            RowSelected["DR/CR"] = "XX";

                            break;
                        }
                }

                string WSelectionCriteria = " WHERE TerminalId='" + TerminalId + "'"
                   + " AND TraceNoWithNoEndZero =" + TraceNo
                   + " AND TransAmount =" + TransAmt
                   + " AND Card_Encrypted ='" + Card_Encrypted + "' AND TXNSRC= '1'  AND Origin = 'Our Atms' "
                   ;
                Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                if (Mpa.RecordFound)
                {
                    //if (Mpa.UniqueRecordId ==175360)
                    //{
                    //    MessageBox.Show("THis is the one"); 
                    //}

                    if (Mpa.MatchMask == "")
                    {
                        RowSelected["MASK"] = "N/A";
                    }
                    else
                        RowSelected["MASK"] = Mpa.MatchMask;
                    //WUniqueRecordId = Mpa.UniqueRecordId;
                    if (Mpa.NotInJournal == true)
                    {

                        RowSelected["IsInMaster"] = false;
                        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
                        Msr.FillTablesProcessForJournal(Operator, InSignedId,
                                               TerminalId,
                                               TraceNo * 10, TransDate.Date, TransAmt);

                        if (Msr.TableJournalDetails.Rows.Count > 0)
                        {
                            // Record found 
                            RowSelected["IsInJournal"] = true;

                        }
                        else
                        {
                            RowSelected["IsInJournal"] = false;
                        }

                    }
                    else
                    {
                        RowSelected["IsInMaster"] = true;
                        RowSelected["IsInJournal"] = true;
                    }


                    if (Mpa.MetaExceptionId == 55)
                    {
                        RowSelected["IsPresenter"] = true;
                        CountPresenter = CountPresenter + 1;

                        if (ActionType != "112")
                        {
                            // Update IST with Actiontype 112
                            //ActionType = "112";
                            if (InWhatFile == 1)
                                UpdateRecordForISTSeqNumber(PhysicalFiledID_1, WSeqNo, ActionType);

                            if (InWhatFile == 2)
                                UpdateRecordForISTSeqNumber(PhysicalFiledID_2, WSeqNo, ActionType);

                        }
                    }
                    else
                    {
                        RowSelected["IsPresenter"] = false;

                        CountAll_IST_Non_Presenter = CountAll_IST_Non_Presenter + 1;
                    }
                }
                else
                {
                    RowSelected["MASK"] = "N/A";
                    RowSelected["IsInMaster"] = false;
                    // find if it is in Journal
                    RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
                    Msr.FillTablesProcessForJournal(Operator, InSignedId,
                                           TerminalId,
                                           TraceNo * 10, TransDate.Date, TransAmt);

                    if (Msr.TableJournalDetails.Rows.Count > 0)
                    {
                        // Record found 
                        RowSelected["IsInJournal"] = true;

                    }
                    else
                    {
                        RowSelected["IsInJournal"] = false;
                    }


                    RowSelected["IsPresenter"] = false;

                    CountAll_IST_Non_Presenter = CountAll_IST_Non_Presenter + 1;


                }
                //RowSelected["IsInJournal"] = SeqNo;
                RowSelected["TerminalId"] = TerminalId;
                RowSelected["Descr"] = TransDescr;
                RowSelected["Date"] = TransDate.ToString();
                RowSelected["Response"] = ResponseCode;

                // LEAVE IT HERE ... Action Type is for IST not Mpa and 112 shows that is presenter and 113 is invalid 
                if (ActionType == "112" || ActionType == "113")
                    RowSelected["Done"] = "YES";
                else RowSelected["Done"] = "NO";

                // ADD ROW
                DataTableAllFields.Rows.Add(RowSelected);

                // DO OTHER UPDATINGS

                I = I + 1;
            }


            string DestinationFile;
            //   DestinationFile =
            //"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            DestinationFile =
                   "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

        }

        public void ReadTableAndFillTableWithPresenter_For_ReplCycle(string InTable,
                                   string InSignedId,
                                   string InTerminalId,
                                   DateTime InFromTransDate, DateTime InToTransDate, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // READ ALL IST WITH 112 and check in Mpa

            CountAll_IST_112 = 0;
            CountAll_IST_Non_Presenter = 0;
            CountPresenter = 0;

            LogicalFiledID = InTable;

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            // Find Physical Name
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();

            Ms.ReadReconcSourceFilesByFileId(InTable);

            string PhysicalFiledID_1 = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Ms.InportTableName + "]";
            string PhysicalFiledID_2 = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + "[" + Ms.InportTableName + "]";

            // FIRST TABLE
            Tbl_IST_112 = new DataTable();
            Tbl_IST_112.Clear();

            Tbl_IST_112.Columns.Add("SeqNo", typeof(int));
            //Tbl_IST_112.Columns.Add("Amount", typeof(string));
            //Tbl_IST_112.Columns.Add("DR/CR", typeof(string));

            // SECOND TABLE
            DataTableAllFields = new DataTable();
            DataTableAllFields.Clear();


            // DATA TABLE ROWS DEFINITION 
            DataTableAllFields.Columns.Add("SeqNo", typeof(int));
            DataTableAllFields.Columns.Add("Done", typeof(string));
            DataTableAllFields.Columns.Add("Amount", typeof(string));
            DataTableAllFields.Columns.Add("DR/CR", typeof(string));

            DataTableAllFields.Columns.Add("IsInMaster", typeof(bool));
            DataTableAllFields.Columns.Add("IsInJournal", typeof(bool));
            DataTableAllFields.Columns.Add("IsPresenter", typeof(bool));
            DataTableAllFields.Columns.Add("MASK", typeof(string));
            DataTableAllFields.Columns.Add("TerminalId", typeof(string));
            DataTableAllFields.Columns.Add("Descr", typeof(string));
            DataTableAllFields.Columns.Add("Date", typeof(string));
            DataTableAllFields.Columns.Add("Response", typeof(string));


            if (In_DB_Mode == 2)
            {

                SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM " + PhysicalFiledID_1
    + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @FromTransDate AND @ToTransDate) "
   + " AND TXNSRC = '1' AND ResponseCode = '112' "
+ " UNION ALL  "
+ " SELECT * "
+ " FROM " + PhysicalFiledID_2
  + " WHERE TerminalId = @TerminalId and (TransDate BETWEEN @FromTransDate AND @ToTransDate) "
   + " AND TXNSRC = '1' AND ResponseCode = '112' "
+ " ) "
+ " SELECT * FROM MergedTbl "
// + " ORDER BY SeqNo DESC "
+ "  ";
            }

            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //TerminalId
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@FromTransDate", InFromTransDate);
                        cmd.Parameters.AddWithValue("@ToTransDate", InToTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            CountAll_IST_112 = CountAll_IST_112 + 1;

                            DataRow RowSelected = Tbl_IST_112.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            // ADD ROW
                            Tbl_IST_112.Rows.Add(RowSelected);

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

            // READ TABLE AND ........

            int InWhatFile;

            int I = 0;

            while (I <= (Tbl_IST_112.Rows.Count - 1))
            {

                int WSeqNo = (int)Tbl_IST_112.Rows[I]["SeqNo"];

                // Find where it is 
                // Look at the first
                ReadTransSpecificFromSpecificTable_By_SeqNo_2(PhysicalFiledID_1, WSeqNo, 1);
                if (RecordFound == true)
                {
                    // It is at the first
                    InWhatFile = 1;
                }
                else
                {
                    // It is at the second 
                    InWhatFile = 2;
                    ReadTransSpecificFromSpecificTable_By_SeqNo_2(PhysicalFiledID_2, WSeqNo, 1);

                }

                DataRow RowSelected = DataTableAllFields.NewRow();

                RowSelected["SeqNo"] = SeqNo;

                RowSelected["Amount"] = TransAmt.ToString("#,##0.00");
                // FOR FIRST ENTRY 
                switch (TransType)
                {
                    case 11:
                        {
                            RowSelected["DR/CR"] = "DR";

                            break;
                        }
                    case 23:
                        {
                            RowSelected["DR/CR"] = "CR";
                            break;
                        }
                    case 33:
                        {
                            //03_REMAINING 00000071 13-10-2019
                            RowSelected["DR/CR"] = "TR";
                            break;
                        }
                    default:
                        {
                            //MessageBox.Show("Not defined ");
                            RowSelected["DR/CR"] = "XX";

                            break;
                        }
                }

                string WSelectionCriteria = " WHERE TerminalId='" + TerminalId + "'"
                   + " AND TraceNoWithNoEndZero =" + TraceNo
                   + " AND TransAmount =" + TransAmt
                   + " AND Card_Encrypted ='" + Card_Encrypted + "' AND TXNSRC= '1'  AND Origin = 'Our Atms' "
                   ;
                Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, 2);

                if (Mpa.RecordFound)
                {
                    //if (Mpa.UniqueRecordId ==175360)
                    //{
                    //    MessageBox.Show("THis is the one"); 
                    //}

                    if (Mpa.MatchMask == "")
                    {
                        RowSelected["MASK"] = "N/A";
                    }
                    else
                        RowSelected["MASK"] = Mpa.MatchMask;
                    //WUniqueRecordId = Mpa.UniqueRecordId;
                    if (Mpa.NotInJournal == true)
                    {

                        RowSelected["IsInMaster"] = false;
                        RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
                        Msr.FillTablesProcessForJournal(Operator, InSignedId,
                                               TerminalId,
                                               TraceNo * 10, TransDate.Date, TransAmt);

                        if (Msr.TableJournalDetails.Rows.Count > 0)
                        {
                            // Record found 
                            RowSelected["IsInJournal"] = true;

                        }
                        else
                        {
                            RowSelected["IsInJournal"] = false;
                        }

                    }
                    else
                    {
                        RowSelected["IsInMaster"] = true;
                        RowSelected["IsInJournal"] = true;
                    }


                    if (Mpa.MetaExceptionId == 55)
                    {
                        RowSelected["IsPresenter"] = true;
                        CountPresenter = CountPresenter + 1;

                        if (ActionType != "112")
                        {
                            // Update IST with Actiontype 112
                            //ActionType = "112";
                            if (InWhatFile == 1)
                                UpdateRecordForISTSeqNumber(PhysicalFiledID_1, WSeqNo, ActionType);

                            if (InWhatFile == 2)
                                UpdateRecordForISTSeqNumber(PhysicalFiledID_2, WSeqNo, ActionType);

                        }
                    }
                    else
                    {
                        RowSelected["IsPresenter"] = false;

                        CountAll_IST_Non_Presenter = CountAll_IST_Non_Presenter + 1;
                    }
                }
                else
                {
                    RowSelected["MASK"] = "N/A";
                    RowSelected["IsInMaster"] = false;
                    // find if it is in Journal
                    RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
                    Msr.FillTablesProcessForJournal(Operator, InSignedId,
                                           TerminalId,
                                           TraceNo * 10, TransDate.Date, TransAmt);

                    if (Msr.TableJournalDetails.Rows.Count > 0)
                    {
                        // Record found 
                        RowSelected["IsInJournal"] = true;

                    }
                    else
                    {
                        RowSelected["IsInJournal"] = false;
                    }


                    RowSelected["IsPresenter"] = false;

                    CountAll_IST_Non_Presenter = CountAll_IST_Non_Presenter + 1;


                }
                //RowSelected["IsInJournal"] = SeqNo;
                RowSelected["TerminalId"] = TerminalId;
                RowSelected["Descr"] = TransDescr;
                RowSelected["Date"] = TransDate.ToString();
                RowSelected["Response"] = ResponseCode;

                // LEAVE IT HERE ... Action Type is for IST not Mpa and 112 shows that is presenter and 113 is invalid 
                if (ActionType == "112" || ActionType == "113")
                    RowSelected["Done"] = "YES";
                else RowSelected["Done"] = "NO";

                // ADD ROW
                DataTableAllFields.Rows.Add(RowSelected);

                // DO OTHER UPDATINGS

                I = I + 1;
            }


            string DestinationFile;
            //   DestinationFile =
            //"[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_Master_Pool_Report" + "]";

            DestinationFile =
                   "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "Working_General_Table" + "]";

        }

        // Insert 
        public void InsertReport(string InDestinationFile, DataTable InTable, string InSignedUserId)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = InDestinationFile;

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }

                UpdateRecordsPerUser(InDestinationFile, InSignedUserId);
            }

        }
        // Truncate file 
        private void TruncateFile(string InTable)
        {
            // Truncate  

            string SQLCmd = "TRUNCATE TABLE " + InTable;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
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
        // Delete records for a particular user 
        //
        private void DeleteRecordsPerUser(string InTable, string InSignedUserId)
        {
            // DELETE 

            string SQLCmd = "DELETE FROM " + InTable
                   + " WHERE SignedUserId = @SignedUserId ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SignedUserId", InSignedUserId);
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
        // Update records for a particular user 
        //
        private void UpdateRecordsPerUser(string InTable, string InSignedUserId)
        {
            // UPDATE
            //       UPDATE[dbo].[Working_General_Table]
            //       SET[SignedUserId] = ''
            //WHERE[SignedUserId] = '' 

            string SQLCmd = "UPDATE " + InTable
                   + " SET [SignedUserId] = @SignedUserId "
                   + "WHERE SignedUserId = '' ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SignedUserId", InSignedUserId);
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
        // Update records for a SeqNo
        //
        public void UpdateRecordForMaskBySeqNumber(string InTable, int InSeqNo, string InMask)
        {
            int count = 0;

            string SQLCmd = "UPDATE " + InTable
                   + " SET Mask = @Mask "
                   + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        count = cmd.ExecuteNonQuery();
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
        // Update records for a SeqNo
        //
        public void UpdateRecordForMaskBySeqNumber_POS(string InTable, int InSeqNo, string InMask, int InProcessedAtRMCycle
                         , int InUniqueRecordId, string InComment)
        {
            int count = 0;

            string SQLCmd = "UPDATE " + InTable
                   + " SET Mask = @Mask "
                   + " ,Processed = 1 "
                   + ", ProcessedAtRMCycle = @ProcessedAtRMCycle "
                    + ", UniqueRecordId = @UniqueRecordId "
                    + ", ActionType = '04' "
                    + ", Comment = @Comment  "
                   + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InProcessedAtRMCycle);
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
                        cmd.Parameters.AddWithValue("@Comment", InComment);
                        count = cmd.ExecuteNonQuery();
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

        // UPDATE MASK FROM DUBLICATES based on trace
        public void UpdateBasedOnDetails_Based_Trace(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InMask)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int count = 0;

            string SQLCmd = "UPDATE " + InTable
                    + " SET Mask = @Mask "
             + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @Net_TransDate"
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber "
                           + " AND ResponseCode = '0' " // 
                           ;
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@Net_TransDate", InTransDate.Date);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);

                        count = cmd.ExecuteNonQuery();
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
        // This is used from File loading 
        //
        public DataTable TableDublicates = new DataTable();
        public void FindDuplicateAddTableFromFileLoading(string InFileId, string InCase, int InPos, string InListMatchingFields, string IN_OnMatchingFields)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableDublicates = new DataTable();
            TableDublicates.Clear();

            TotalSelected = 0;

            SqlString =

           "     SELECT * "
+ " FROM " + InFileId + " y"
+ " INNER JOIN "
+ " (SELECT  TerminalId, TraceNo, AccNo, TransAmt, TransDate, ResponseCode, TransType, COUNT(*) AS CountOf "

+ " FROM  " + InFileId

+ " WHERE Comment <> 'Reversals' AND ResponseCode = '0' "

+ " GROUP BY TerminalId, TraceNo, AccNo, TransAmt, TransDate, ResponseCode, TransType  HAVING COUNT(*) > 1) dt "
+ " ON  y.TerminalId = dt.TerminalId AND y.TraceNo = dt.TraceNo AND y.AccNo = dt.AccNo  AND y.TransAmt = dt.TransAmt "
+ "  AND y.TransDate = dt.TransDate AND y.ResponseCode = dt.ResponseCode AND y.TransType = dt.TransType "
+ " Order By FullTraceNo ";

            //" SELECT * "
            ////+ " y.SeqNo, y.MatchingCateg, y.RMCycle, y.TerminalId, y.TraceNo, y.AccNo, y.TransAmt, y.TransDate "
            //+ " FROM " + InFileId + " y"
            //+ " INNER JOIN "
            //+ " ( "
            //+ " SELECT "
            ////+ InListMatchingFields + " , COUNT(*) AS CountOf"
            // + " TerminalId, TraceNo, AccNo,  TransAmt, TransDate, COUNT(*) AS CountOf"
            //+ " FROM  " + InFileId
            // + " GROUP BY TerminalId, TraceNo, AccNo, TransAmt, TransDate "
            ////+ " GROUP BY " + InListMatchingFields
            //+ " HAVING COUNT(*)>1 "
            //+ " ) dt"
            ////+ " ON " + IN_OnMatchingFields
            //+ " ON y.TerminalId=dt.TerminalId AND y.TraceNo= dt.TraceNo AND y.AccNo=dt.AccNo "
            //     + " AND y.TransAmt=dt.TransAmt AND y.TransDate =dt.TransDate "
            //+ " ";

            using (SqlConnection conn =
           new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableDublicates);

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

        public void DeleteDuplicatesInIST(int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;

            string CMD_Details = " WITH cte AS( "
                      + " SELECT "
                      + "  SeqNo, "
                      + "  TerminalId, "
                      + "  FullTraceNo, "
                       + "  AccNo, "
                      + "   TransAmt, "
                      + "   Minutes_Date, "
                      + "   ResponseCode, "
                        + "   TransType, "
                      + "   ROW_NUMBER() OVER( "
                      + "   PARTITION BY "
                     + "  TerminalId, "
                      + "  FullTraceNo, "
                       + "  AccNo, "
                      + "   TransAmt, "
                      + "   Minutes_Date, "
                      + "   ResponseCode, "
                        + "   TransType "
                      + "   ORDER BY "
                     + "  TerminalId, "
                      + "  FullTraceNo, "
                       + "  AccNo, "
                      + "   TransAmt, "
                      + "   Minutes_Date, "
                      + "   ResponseCode, "
                        + "   TransType "
                      + "    ) row_num "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                      //+ " where LoadedAtRMCycle>=@LoadedAtRMCycle "
                      + " ) "
                      + " DELETE FROM cte "
                      + " WHERE row_num > 1 ";
            //string SqlCommand = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //                + " WHERE MatchingCateg = @MatchingCateg "; 

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(CMD_Details, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle-3);

                        //rows number of record got updated
                        cmd.CommandTimeout = 300;
                        Count = cmd.ExecuteNonQuery();
                        //  Count = cmd.ExecuteNonQueryAsync(); 
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

            string text = "Dublicates were deleted from IST_" + Count.ToString();
            string caption = "IST_Loading";
            int timeout = 5000;
            AutoClosingMessageBox.Show(text, caption, timeout);
           // MessageBox.Show("Dublicates were deleted from IST_"+Count.ToString()); 
        }


        public void DeleteDuplicatesInCIT_FILE(int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;

            string CMD_Details = " WITH cte AS( "
                      + " SELECT "
                      + "  SeqNo, "
                      + "  TerminalId, "
                      + "  TransType, "
                      + "   TransAmt, "
                      + "   TransDate, "
                      + "   TraceNo, "
                      + "   ROW_NUMBER() OVER( "
                      + "   PARTITION BY "
                     + "  TerminalId, "
                      + "  TransType, "
                      + "   TransAmt, "
                      + "   TransDate, "
                      + "   TraceNo "
                      + "   ORDER BY "
                      + "  TerminalId, "
                      + "  TransType, "
                      + "   TransAmt, "
                      + "   TransDate, "
                      + "   TraceNo "
                      + "    ) row_num "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS] "
                     // + " where LoadedAtRMCycle>=@LoadedAtRMCycle "
                      + " ) "
                      + " DELETE FROM cte "
                      + " WHERE row_num > 1 ";
            //string SqlCommand = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //                + " WHERE MatchingCateg = @MatchingCateg "; 

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(CMD_Details, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle-3);

                        //rows number of record got updated
                        cmd.CommandTimeout = 300;
                        Count = cmd.ExecuteNonQuery();
                        //  Count = cmd.ExecuteNonQueryAsync(); 
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

        // UPDATE MASK FROM DUBLICATES based on RRN
        public void UpdateBasedOnDetails_Based_RRN(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        string InRRNumber, decimal InTransAmt, string InCardNumber, string InMask)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int count = 0;

            string SQLCmd = "UPDATE " + InTable
                    + " SET Mask = @Mask "
             + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @Net_TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber "
                           + " AND ResponseCode = '0' " // 
                           ;
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@Net_TransDate", InTransDate.Date);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        count = cmd.ExecuteNonQuery();
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
        // Update record for a SeqNo
        //
        public void UpdateRecordForISTSeqNumber(string InTable, int InSeqNo, string InActionType)
        {
            int count = 0;

            string SQLCmd = "UPDATE " + InTable
                   + " SET ActionType = @ActionType "
                   + "WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ActionType", InActionType);
                        count = cmd.ExecuteNonQuery();
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
        // Update records 
        //
        public void UpdateReversalsForMask_By_Trace(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InMask)
        {
            int count = 0;
            string SQLCmd = "UPDATE " + InTable
                   + " SET Mask = @Mask "
                   + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @TransDate"
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        count = cmd.ExecuteNonQuery();
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
        // Update reversal records 
        //
        public void UpdateReversalsForMask_By_RRN(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        string InRRNumber, decimal InTransAmt, string InCardNumber, string InMask)
        {
            int count = 0;
            string SQLCmd = "UPDATE " + InTable
                   + " SET Mask = @Mask "
                   + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        count = cmd.ExecuteNonQuery();
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
        // Update records 
        //
        public void UpdateReversalsForMask_By_RRN_For_POS(string InTable, string InMatchingCateg,
                                        string InRRNumber, decimal InTransAmt, string InCard_Encrypted, string InMask)
        {
            int count = 0;
            string SQLCmd = "UPDATE " + InTable
                   + " SET Mask = @Mask "
                   + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           // + " AND Net_TransDate = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + " AND Card_Encrypted = @Card_Encrypted ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        count = cmd.ExecuteNonQuery();
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
        // READ Specific by Selection criteria
        // 
        //
        public bool IsSecretAccNo;
        public void ReadTable_BySelectionCriteria(string InTableA, string InTableB, string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            IsSecretAccNo = false;
            TotalSelected = 0;

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bist = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            // MessageBox.Show("Correct Method to proceed-(ReadTable_BySelectionCriteria)");
            // return;

            if (In_DB_Mode == 1)
            {
                SqlString =
              " SELECT * FROM " + InTableA
            + InSelectionCriteria;
            }

            if (In_DB_Mode == 2)
            {
                SqlString =
" WITH MergedTbl AS "
+ " ( "
+ " SELECT *  "
+ " FROM " + InTableA
+ InSelectionCriteria
+ " UNION ALL  "
+ " SELECT * "
+ " FROM " + InTableB
+ InSelectionCriteria
+ " ) "
+ " SELECT * FROM MergedTbl "
+ " WHERE TransDate> @TransDate AND (ResponseCode = '0' OR ResponseCode = '200000')"
+ " ORDER BY TransDate DESC "
// + " ORDER BY SeqNo DESC "
+ "  ";
            }
            if (In_DB_Mode == 1)
            {
                using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            cmd.Parameters.AddWithValue("@TransDate", DateTime.Today.AddDays(-90));

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {

                                RecordFound = true;
                                ReadFieldsInTable(rdr);

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


            if (In_DB_Mode == 2)
            {
                TablePOS_Settlement = new DataTable();
                TablePOS_Settlement.Clear();

                TablePOS_Settlement.Columns.Add("SeqNo", typeof(int));
                TablePOS_Settlement.Columns.Add("Origin", typeof(string));

                TablePOS_Settlement.Columns.Add("Ccy", typeof(string));
                TablePOS_Settlement.Columns.Add("Amount", typeof(decimal));
                TablePOS_Settlement.Columns.Add("CH_Amount", typeof(decimal));
                TablePOS_Settlement.Columns.Add("Auth Number", typeof(string));
                TablePOS_Settlement.Columns.Add("DateTime", typeof(string));
                TablePOS_Settlement.Columns.Add("RESP CODE", typeof(string));
                TablePOS_Settlement.Columns.Add("Acceptor Id", typeof(string));
                TablePOS_Settlement.Columns.Add("Acceptor Name", typeof(string));
                TablePOS_Settlement.Columns.Add("MASK", typeof(string));
                TablePOS_Settlement.Columns.Add("DR/CR", typeof(string));
                TablePOS_Settlement.Columns.Add("AccNo", typeof(string));
                TablePOS_Settlement.Columns.Add("Terminal", typeof(string));
                TablePOS_Settlement.Columns.Add("Card Encypted", typeof(string));

                using (SqlConnection conn =
                        new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            cmd.Parameters.AddWithValue("@TransDate", DateTime.Today.AddDays(-90));

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {

                                RecordFound = true;

                                ReadFieldsInTable(rdr);

                                if (AccNo.Length > 6)
                                {
                                    if (AccNo.Substring(0, 7) == "0195425")
                                    {
                                        IsSecretAccNo = true;
                                    }
                                }

                                DataRow RowSelected = TablePOS_Settlement.NewRow();

                                RowSelected["SeqNo"] = SeqNo;

                                RowSelected["Origin"] = "POS";

                                RowSelected["Ccy"] = TransCurr;
                                RowSelected["Amount"] = TransAmt;

                                RowSelected["CH_Amount"] = Bist.Read_SOURCE_Table_And_CH_Amount(LoadedAtRMCycle, RRNumber, Card_Encrypted);

                                RowSelected["Auth Number"] = RRNumber;
                                RowSelected["DateTime"] = TransDate.ToString();
                                RowSelected["RESP CODE"] = ResponseCode;

                                RowSelected["Acceptor Id"] = ACCEPTOR_ID;
                                RowSelected["Acceptor Name"] = ACCEPTORNAME;

                                if (Mask == "")
                                {
                                    RowSelected["MASK"] = "Matched";
                                }
                                else
                                {
                                    RowSelected["MASK"] = Mask;
                                }

                                RowSelected["DR/CR"] = TransType.ToString();
                                RowSelected["AccNo"] = AccNo;
                                RowSelected["Terminal"] = TerminalId;
                                if (Card_Encrypted != "")
                                {
                                    RowSelected["Card Encypted"] = Card_Encrypted;
                                }
                                else
                                {
                                    RowSelected["Card Encypted"] = CardNumber;
                                }

                                // ADD ROW

                                TablePOS_Settlement.Rows.Add(RowSelected);

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



        //
        // Methods 
        // READ Specific by Selection criteria
        // 
        //
        public DataTable ATMsDataTable = new DataTable();
        string AtmNo;

        public void ReadTable_AndFindAll_Banks_Atms(string InOperator, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ATMsDataTable = new DataTable();
            ATMsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsDataTable.Columns.Add("AtmNo", typeof(string));

            SqlString =
               " SELECT distinct TerminalId As AtmNo "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
               + " WHERE TXNSRC = 1 AND Processed = 0 AND ResponseCode = '0' "
               //+"AND CAP_DATE = @Cut_Off_Date"
               + " Order by TerminalId "; // From Our ATMs

            // + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            AtmNo = (string)rdr["AtmNo"];

                            DataRow RowSelected = ATMsDataTable.NewRow();

                            RowSelected["AtmNo"] = AtmNo;

                            // ADD ROW
                            ATMsDataTable.Rows.Add(RowSelected);

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
        // Give number 
        public int ReadTable_AndFindNumberOfTxnsInIST(string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int TotalTxns = 0;

            SqlString =
               " SELECT Count(*) As TotalTxns"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
               + " WHERE TerminalId = @TerminalId AND TXNSRC = 1 AND Processed = 0 AND ResponseCode = '0' "
               + " "; // 

            // + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalTxns = (int)rdr["TotalTxns"];

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
            return TotalTxns;
        }

        //
        // Methods 
        // READ Specific by Selection criteria
        // 
        //
        public void ReadTable_BySelectionCriteria_FirstRecord(string InTable, string InSelectionCriteria, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT * FROM " + InTable
             + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReadFieldsInTable(rdr);
                            if (AccNo == "")
                            {
                                continue;
                            }
                            else
                            {
                                break; // When Found 
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
        // UPDATE Action Type 
        //
        public void UpdateActionTypeBySeqNo(string InTable, int InSeqNo, string InActionType)
        {
            int count = 0;
            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [ActionType] = @ActionType "
                    + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ActionType", InActionType);

                        count = cmd.ExecuteNonQuery();
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
        // UpdateProcessedBySeqNo
        //
        public void UpdateProcessedBySeqNo(string InTable, int InSeqNo, bool InProcessed, int InProcessedAtRMCycle)
        {
            int count = 0;
            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [Processed] = 1 "
                    + " , [ProcessedAtRMCycle] = @ProcessedAtRMCycle "
                    + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Processed", InProcessed);
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InProcessedAtRMCycle);

                        count = cmd.ExecuteNonQuery();
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
        // UpdateProcessedBySeqNo and coming from reversals 
        //
        public void UpdateProcessedBySeqNoFromReversals(string InTable, int InSeqNo, int InProcessedAtRMCycle,
                                                                              string InComment, SqlConnection cnx)
        {

            int count = 0;
            //
            string SQLCmd = "UPDATE " + InTable
                    + " SET [Processed] = 1 "
                    + "     ,[IsSettled] = 1 "
                    + "     ,[Comment] = @Comment "
                    + "     ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle "
                    + " WHERE SeqNo = @SeqNo ";

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            try
            {
                //conn.Open();
                using (SqlCommand cmd =
                    new SqlCommand(SQLCmd, cnx))
                {
                    cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                    cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InProcessedAtRMCycle);
                    cmd.Parameters.AddWithValue("@Comment", InComment);

                    count = cmd.ExecuteNonQuery();
                }
                // Close conn
                //conn.Close();
            }
            catch (Exception ex)
            {
                //conn.Close();

                CatchDetails(ex);
            }

        }

        //
        // UpdateProcessedBySeqNo and coming from reversals 
        //
        public void UpdateNonProcessedDepositReversals(string InTable, int InProcessedAtRMCycle)
        {

            int count = 0;
            //
            string SQLCmd = "UPDATE " + InTable
                    + " SET [Processed] = 0 "
                    + "     ,[IsSettled] = 0 "
                    // + "     ,[Comment] = @Comment "
                    + "     ,[ProcessedAtRMCycle] = 0 "
                    + " WHERE Comment  = 'Reversals' and Left(FullTraceNo,1) = '2' "
                    + "  AND ProcessedAtRMCycle = @ProcessedAtRMCycle AND TransType = 23 and TXNSRC = '1' ";


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InProcessedAtRMCycle);

                        count = cmd.ExecuteNonQuery();
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
        // UPDATE Action Type 
        //
        public void UpdateActionTypeBySeqNoForManualMatched(string InTable, int InSeqNo)
        {
            int count = 0;

            // 
            string SQLCmd = "UPDATE " + InTable
                    + " SET [ActionType] = @ActionType "
                          + " ,[UniqueRecordId] = @UniqueRecordId "
                          + " ,[Comment] = @Comment "
                    + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@Comment", Comment);

                        count = cmd.ExecuteNonQuery();
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
        // UNDO - UPDATE Action Type 
        //
        public void UpdateActionTypeByUniqueNoForManualMatched(string InTable, int InUniqueRecordId)
        {
            int count = 0;

            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [ActionType] = '0' "
                          + " ,[UniqueRecordId] = 0 "
                          + " ,[Comment] = '' "
                    + " WHERE UniqueRecordId = @UniqueRecordId ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);

                        count = cmd.ExecuteNonQuery();
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
        // UPDATE Action Type = 0 for ALL
        //

        public void UpdateActionTypeByALL(string InTable, string InMatchingCateg)
        {
            int count = 0;
            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [ActionType] = '00' "
                    + " WHERE ActionType = '03' AND MatchingCateg = @MatchingCateg AND (Processed = 0 OR (Mask<>'111' AND Mask<>''))";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg ", InMatchingCateg);

                        count = cmd.ExecuteNonQuery();
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
        // UPDATE Action Type by Selection Criteria
        //
        public void UpdateActionTypeBySelectionCriteria(string InTable, string InSelectionCriteria, string InActionType)
        {
            int count = 0;
            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [ActionType] = @ActionType "
                    + InSelectionCriteria;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //   cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@ActionType", InActionType);

                        count = cmd.ExecuteNonQuery();
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

        //public DateTime Net_TransDate;
        // Insert NEW Record in Table 
        //
        public int InsertNewRecordInTble(string InTable)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO " + InTable
                 + " ([OriginFileName], [OriginalRecordId], [LoadedAtRMCycle] "
                 + " ,[MatchingCateg],[Origin] "
                 + " ,[TerminalType],[TransTypeAtOrigin] "

                   + " ,[TerminalId],[TransType] " //2
                 + " ,[TransDescr],[CardNumber] "
                   + " ,[AccNo],[TransCurr] "

                 + " ,[TransAmt],[AmtFileBToFileC] "
                   + " ,[TransDate],[TraceNo] "
                 + " ,[RRNumber],[FullTraceNo] " // 

                   + " ,[ResponseCode],[ActionType] " // 4
                 + " ,[Processed],[ProcessedAtRMCycle] "
                   + " ,[Mask],[IsSettled] "

                 + " ,[UniqueRecordId],[Operator] "
                  + " ,[TXNSRC],[TXNDEST] "
                   + " ,[Comment],[EXTERNAL_DATE] " // 5

                 + "  ,[Net_TransDate] " // 31 all
                 + ",[Card_Encrypted] "
                 + ",[ACCEPTOR_ID] "
                 + ",[ACCEPTORNAME] "
                 + ")"
                + " VALUES "
                 + " (@OriginFileName, @OriginalRecordId , @LoadedAtRMCycle" //1
                 + " ,@MatchingCateg, @Origin "
                 + " ,@TerminalType,@TransTypeAtOrigin "

                   + " ,@TerminalId,@TransType " //2
                 + " ,@TransDescr,@CardNumber "
                   + " ,@AccNo,@TransCurr "

                 + " ,@TransAmt,@AmtFileBToFileC " // 3
                   + " ,@TransDate,@TraceNo "
                 + " ,@RRNumber,@FullTraceNo "

                   + " ,@ResponseCode,@ActionType " // 4
                 + " ,@Processed,@ProcessedAtRMCycle "
                   + " ,@Mask,@IsSettled "

                 + " ,@UniqueRecordId,@Operator " // 5
                  + " ,@TXNSRC,@TXNDEST "
                   + " ,@Comment,@EXTERNAL_DATE "

                 + "  ,@Net_TransDate"
                 + ",@Card_Encrypted"
                  + ",@ACCEPTOR_ID"
                   + ",@ACCEPTORNAME"

                  + ") "

                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", TerminalType);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);

                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);

                        cmd.Parameters.AddWithValue("@TransAmt", TransAmt);
                        cmd.Parameters.AddWithValue("@AmtFileBToFileC", AmtFileBToFileC);
                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@FullTraceNo", FullTraceNo);

                        cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@Processed", Processed);
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", ProcessedAtRMCycle);
                        cmd.Parameters.AddWithValue("@Mask", Mask);
                        cmd.Parameters.AddWithValue("@IsSettled", IsSettled);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                        cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);
                        cmd.Parameters.AddWithValue("@Comment", Comment);
                        cmd.Parameters.AddWithValue("@EXTERNAL_DATE", EXTERNAL_DATE);

                        cmd.Parameters.AddWithValue("@Net_TransDate", Net_TransDate);
                        cmd.Parameters.AddWithValue("@Card_Encrypted", Card_Encrypted);

                        cmd.Parameters.AddWithValue("@ACCEPTOR_ID", ACCEPTOR_ID);
                        cmd.Parameters.AddWithValue("@ACCEPTORNAME", ACCEPTORNAME);

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
        // UPDATE As Not Processed
        //
        public void CreateNewRecordsForPool_XXXXX(string InCategory, int InRMCycle, int InMode)
        {
            // Read all Move to pool for this cycle and create records.
            // Copy the previous but make them as not processed. 

            RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();


            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

            return; // This functinality is canncelled out. 


            MoveToPoolTable = new DataTable();
            MoveToPoolTable.Clear();


            string WSelectionCriteria = " WHERE "
                                                          + " RMCateg ='" + InCategory + "'"
                                                          + " AND MatchingAtRMCycle =" + InRMCycle
                                                          + " AND ActionType = '06' ";

            Mpa.ReadMatchingTxnsMasterPoolByCategoryAndCycleAndFillTable(WSelectionCriteria, 1);

            MoveToPoolTable = Mpa.DataTableActionsTaken;

            //    RowSelected["RecordId"] = UniqueRecordId;

            int I = 0;

            while (I <= (MoveToPoolTable.Rows.Count - 1))
            {
                // Do 

                int RecordId = (int)MoveToPoolTable.Rows[I]["RecordId"];

                string Selection_2 = " WHERE UniqueRecordId =" + RecordId;

                Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(Selection_2, 1);

                string FileA = Mpa.FileId01;
                string FileB = Mpa.FileId02;
                string FileC = Mpa.FileId03;
                int NewSeqNo;

                Msf.ReadReconcSourceFilesByFileId(Mpa.FileId01);

                if (Mpa.SeqNo01 == 0)
                {
                    // No record to be created for this 
                }
                else
                {
                    // Create new master record

                    Mpa.IsMatchingDone = false;
                    Mpa.Matched = false;
                    Mpa.MatchMask = "";
                    Mpa.MatchedType = "";
                    Mpa.SystemMatchingDtTm = DateTime.Now;

                    //Mpa.TraceNoWithNoEndZero = LastTraceNo;

                    Mpa.UserId = "";
                    Mpa.ActionByUser = false;
                    Mpa.Authoriser = "";

                    Mpa.NotInJournal = true;

                    Mpa.SettledRecord = false;

                    Mpa.UniqueRecordId = Gu.GetNextValue();

                    NewSeqNo = Mpa.InsertTransMasterPoolATMs(Mpa.Operator);

                }

                if (Mpa.SeqNo02 == 0)
                {
                    // No record to be created for this 
                }
                else
                {
                    // Create new master record
                    // FILE B
                    Msf.ReadReconcSourceFilesByFileId(Mpa.FileId02);
                    string PhysicalName_FileId02 = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";
                    string Selection5 = " WHERE SeqNo =" + Mpa.SeqNo02;
                    ReadTransSpecificFromSpecificTable_Primary(Selection5, PhysicalName_FileId02, 1);

                    Processed = false;

                    ProcessedAtRMCycle = 0;
                    Mask = "";
                    IsSettled = false;

                    NewSeqNo = InsertNewRecordInTble(PhysicalName_FileId02);
                }

                if (Mpa.SeqNo03 == 0)
                {
                    // No record to be created for this 
                }
                else
                {
                    // Create new master record
                    // FILE B
                    Msf.ReadReconcSourceFilesByFileId(Mpa.FileId03);
                    string PhysicalName_FileId03 = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";
                    string Selection5 = " WHERE SeqNo =" + Mpa.SeqNo03;
                    ReadTransSpecificFromSpecificTable_Primary(Selection5, PhysicalName_FileId03, 1);

                    Processed = false;

                    ProcessedAtRMCycle = 0;
                    Mask = "";
                    IsSettled = false;

                    NewSeqNo = InsertNewRecordInTble(PhysicalName_FileId03);


                }

                I++;
            }



        }

        //
        // UPDATE As Not Processed one table 
        //
        public void UpdateRecordAsNotProcessed(string InTable, int InSeqNo, bool InProcessed)
        {
            int count = 0;
            // Truncate
            string SQLCmd = "UPDATE " + InTable
                    + " SET [Processed] = @Processed "
                    + "WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Processed", InProcessed);

                        count = cmd.ExecuteNonQuery();
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





        int MaxTrace;
        // Find Max Trace for other files than Mpa 
        public int ReadAndFindMaxTraceNo(string InTable, string InMatchingCateg, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxTrace = 0;

            SqlString =
            " SELECT  ISNULL(MAX([TraceNo]), 0) As MaxTrace "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId  ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            MaxTrace = (int)rdr["MaxTrace"];

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
            return MaxTrace;
        }

        // Find Max dateTime for Mpa with TerminalId (ATM)

        public DateTime ReadAndFindMaxDateTimeForMpa(string InFile, string InMatchingCateg, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
            + " FROM " + InFile
            + " WHERE  "
                        + "     ([IsMatchingDone] = 0) "
                        + " AND ([MatchingCateg] = @MatchingCateg) "
                        + " AND (TerminalId = @TerminalId)"
                        + " AND (ResponseCode = '0')";


            using (SqlConnection conn =
                       new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return MaxDt;
        }

        // Find Max dateTime for Mpa Matched Txns At Cycle 

        public DateTime ReadAndFindMaxDateTimeForMpaAfterMathcing(string InTableId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
            + " FROM " + InTableId
            + " WHERE  "
                        + "     (IsMatchingDone = 1 AND Matched = 1) "
                        //+ " AND ([MatchingCateg] = @MatchingCateg) "
                        //+ " AND (TerminalId = @TerminalId)"
                        + " AND (ResponseCode = '0')";


            using (SqlConnection conn =
                       new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return MaxDt;
        }

        // Find Max dateTime for Mpa with TerminalId (ATM)

        public DateTime ReadAndFindMaxDateTimeForMpaNoCategory(string InFile, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDt = NullPastDate;

            SqlString =
            " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
            + " FROM " + InFile + " WITH (NOLOCK)"
            + " WHERE  "
                        + "     ([IsMatchingDone] = 0) "
                        //     + " AND ([MatchingCateg] = @MatchingCateg) "
                        + " AND (TerminalId = @TerminalId)"
                          + " AND (ResponseCode = '0') AND Origin = 'Our Atms' And MatchingCateg <> 'BDC299' ";


            using (SqlConnection conn =
                       new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.CommandTimeout = 700;

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return MaxDt;
        }

        // Find Max dateTime for Mpa with TerminalId (ATM)

        public DateTime ReadAndFindMaxDateTimeForMpa_NO_ATM(string InFile, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //  SELECT TOP 1 * FROM MyTable WHERE id = 'Some id' ORDER BY[Date] DESC

            // SqlString =
            //" SELECT  Count(*) As NumberOfRecords, MAX (TransDate) As MaxDt "
            //+ " FROM " + InFile
            //+ " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0 "
            //               + " ";
            SqlString =
             " SELECT TOP 1 * "
             + " FROM " + InFile
             + " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0 "
                       + " ORDER By TransDate DESC, RRNumber DESC ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            int NoRec = (int)rdr["NumberOfRecords"];

                            if (NoRec == 0)
                            {
                                // 
                                MaxDt = NullPastDate;
                            }
                            else
                            {
                                MaxDt = (DateTime)rdr["MaxDt"];
                                //    MaxDt = MaxDt.AddSeconds(59);
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
            return MaxDt;
        }

        // Find trace Number or rrn number that corresponce to MINMAX DtTime
        int MxTraceNo;




        // Find corresponding Data during Matching
        public void ReadAndFindDetails(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId "
                           + " AND TransDate  = @TransDate"
                            + " AND ResponseCode = '0' ";
            //     + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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



        // Find corresponding Exact DateTime based on details
        public DateTime ReadAndFindDateBasedOnDetails(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "

                           + " AND Net_TransDate = @TransDate"
                           + " AND TerminalId = @TerminalId "
                           + " AND CardNumber = @CardNumber "
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "

                           //   + " AND ResponseCode = '0' " //  to cover reversal pairs
                           //+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                           ;
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }
        public int WCount;
        // Find corresponding  based on details BASED on RRNumber 
        public DateTime ReadAndFindDateBasedOnDetails_RRNumber(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        string InRRNumber, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            WCount = 0;

            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND CAST(TransDate AS Date) = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt ";
            //  + " AND CardNumber = @CardNumber "
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        //    cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            WCount = WCount + 1;

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
            return TransDate;
        }
        // Find corresponding Exact DateTime based on details // Trace No 
        public DateTime ReadAndFindDateBasedOnDetails_2(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @TransDate"
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber ";
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }
        public DateTime ReadAndFindDateBasedOnDetails_2_ROM(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @TransDate"
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber ";
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }

        // Find if exixt and Response code = 0
        public void ReadAndFindDateBasedOnDetails_3(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        int InTraceNo, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           + " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate = @TransDate"
                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND CardNumber = @CardNumber "
                           + " AND ResponseCode = '0' " // 
                           ;
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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

        // Find if exixt and Response code = 0
        public void ReadAndFindDateBasedOnDetails_4(string InTerminalId,
                                        int InTraceNo, decimal InTransAmt,
                                        int InMinutes_Date
                                              )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
               " SELECT  *  "
            + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[Switch_IST_Txns]"
            + " WHERE "
                           + "  TerminalId = @TerminalId "

                           + " AND TraceNo = @TraceNo "
                           + " AND TransAmt = @TransAmt "
                           + " AND Minutes_Date = @Minutes_Date "
                           + "  " // 
                           ;
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        cmd.Parameters.AddWithValue("@Minutes_Date", InMinutes_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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


        // Find corresponding 
        public DateTime ReadAndFindDateBasedOnDetails_2_Based_On_RRN(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        string InRRNumber, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate  = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + " AND Comment = 'Reversals' ";
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        //   cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }

        // Find corresponding 
        public DateTime ReadAndFindDateBasedOnDetails_2_Based_On_RRN_P_type(string InTable, string InMatchingCateg, string InTerminalId,
                                        DateTime InTransDate,
                                        string InRRNumber, decimal InTransAmt, string InCardNumber, string InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND Net_TransDate  = @TransDate"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + "  ";
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        //   cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }

        //// Find corresponding 
        //public DateTime ReadAndFindDateBasedOnDetails_2_Based_On_RRN_AUTH_type(string InTable, string InMatchingCateg, string InTerminalId,
        //                                DateTime InTransDate,
        //                                string InAUTHNUM, decimal InTransAmt, string InCardNumber, string InAccNo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";


        //    SqlString =
        //       " SELECT  *  "
        //    + " FROM " + InTable
        //    + " WHERE MatchingCateg = @MatchingCateg "
        //                   // +" AND Processed = 0 "
        //                   //+ " AND TerminalId = @TerminalId "
        //                   + " AND Net_TransDate  = @TransDate"
        //                   + " AND AUTHNUM = @AUTHNUM "
        //                   + " AND TransAmt = @TransAmt "
        //                   + "  ";
        //    //   + " AND ResponseCode = '0' " //  to cover reversal pairs
        //    //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
        //    //+ " AND AccNo = @AccNo "


        //    using (SqlConnection conn =
        //                 new SqlConnection(ATMSconnectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
        //                cmd.Parameters.AddWithValue("@TransDate", InTransDate);

        //                cmd.Parameters.AddWithValue("@AUTHNUM", InAUTHNUM);
        //                cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
        //                //   cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
        //                //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    ReadFieldsInTable(rdr);

        //                }
        //                // Close Reader
        //                rdr.Close();
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //    return TransDate;
        //}


        // Find corresponding 
        public DateTime ReadAndFindIfReversalForMaster_POS_Based_On_RRN(string InTable, string InMatchingCateg,
                                        string InRRNumber, decimal InTransAmt, string InCard_Encrypted)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           // +" AND Processed = 0 "
                           //+ " AND TerminalId = @TerminalId "
                           + " AND Card_Encrypted  = @Card_Encrypted"
                           + " AND RRNumber = @RRNumber "
                           + " AND TransAmt = @TransAmt "
                           + "  ";
            //   + " AND ResponseCode = '0' " //  to cover reversal pairs
            //   + " AND CAST(left([FullTraceNo], 1) as int)  = 2  ";
            //+ " AND AccNo = @AccNo "


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        // cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@Card_Encrypted", InCard_Encrypted);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        //   cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        //cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            return TransDate;
        }

        // Find corresponding Exact DateTime based on details
        public DateTime ReadAndFindDateBasedOnDetails_NO_ATM(string InTable, string InMatchingCateg,
                                        string InCardNumber,
                                        decimal InTransAmt,
                                        DateTime InTransDate,
                                        string InRRNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT  *  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg "
                           + " AND Processed = 0  "
                           + " AND CardNumber = @CardNumber"
                           + " AND TransAmt = @TransAmt"
                           + " AND Net_TransDate = @TransDate"
                           + " AND RRNumber = @RRNumber "
            + " ORDER BY TransDate ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber); // In Card Number

                        cmd.Parameters.AddWithValue("@TransAmt", InTransAmt); // This is Net Date only

                        cmd.Parameters.AddWithValue("@TransDate", InTransDate.Date); // This is Net Date only

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
            // 
            if (RecordFound == false)
            {
                //
                // In case not found due to not equal RRNumber 
                //
                SqlString =
           " SELECT  TOP 1 *  "
           + " FROM " + InTable
           + " WHERE MatchingCateg = @MatchingCateg "
                          + " AND Processed = 0  "
                          + " AND TransDate <= @TransDate"
           // + " AND RRNumber = @RRNumber "
           + " ORDER BY TransDate DESC ";

                using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                            cmd.Parameters.AddWithValue("@TransDate", InTransDate); // This is full date

                            cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {

                                RecordFound = true;

                                ReadFieldsInTable(rdr);

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
            return TransDate;
        }

        DateTime MaxDt;
        // Find Max Trace for other files than Mpa 
        public DateTime ReadAndFindMaxDt(string InTable, string InMatchingCateg, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            SqlString =
        " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
        + " FROM " + InTable
             + " WHERE  "
             + "     ([Processed] = 0) "
             + " AND ([TransDate] <> '2050-12-31 00:00:00.000') "
             + " AND ([MatchingCateg] = @MatchingCateg) "
             + " AND (TerminalId = @TerminalId)"
             + " AND (ResponseCode = '0')";


            using (SqlConnection conn =
                       new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return MaxDt;
        }

        // Find Max Trace for other files than Mpa 
        public DateTime ReadAndFindMaxDt_No_Category(string InTable, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDt = NullPastDate;

            SqlString =
        " SELECT  ISNULL(MAX (TransDate), '1900-01-01') As MaxDt "
        + " FROM " + InTable + " WITH (NOLOCK) "
             + " WHERE  "
             + "     ([Processed] = 0) "
            + " AND ([TransDate] <> '2050-12-31 00:00:00.000') "
             + " AND (TerminalId = @TerminalId)"
             + " AND (ResponseCode = '0')";


            using (SqlConnection conn =
                       new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.CommandTimeout = 700;

                        // Read table 
                        MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return MaxDt;
        }

        // Find Max Trace for other files than Mpa 
        public DateTime ReadAndFindMaxDt_No_Category_ByATM(string InTable, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDt = NullPastDate;

            SqlString =
        " SELECT Top(1) TransDate, Comment  "
        + " FROM " + InTable + " WITH (NOLOCK) "
             + " WHERE  "
             + "     ([Processed] = 0) "
            // + " AND ([MatchingCateg] = @MatchingCateg) "
             + " AND (TerminalId = @TerminalId)"
             + " AND (ResponseCode = '0')"
             + " Order By TransDate Desc ";

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        cmd.CommandTimeout = 700;
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            MaxDt = (DateTime)rdr["TransDate"];

                            Comment = (string)rdr["Comment"];

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
            return MaxDt;
        }

        // Find Min DateTime by file

        public DateTime ReadAndFindMinDtForFile(string InTable, int InRMCycle, string InOriginFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InOriginFileName this is the record Id of the file
            // int RecordsInFile = 0;
            DateTime MinDt = NullPastDate;

            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + InTable + "]";

            SqlString =
       " SELECT  ISNULL(MIN([TransDate]),'1900-01-01') As MinDt "
       + " FROM " + PhysicalName
       + " WHERE LoadedAtRMCycle=@LoadedAtRMCycle AND OriginFileName = @OriginFileName"
       + " ";

            using (SqlConnection conn =
                 new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@OriginFileName", InOriginFileName);
                        cmd.CommandTimeout = 50;
                        // Read table 
                        MinDt = Convert.ToDateTime(cmd.ExecuteScalar());
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return MinDt;
        }

        // Fill Table of Txns for BULK 

        public void ReadAndFillTableFor_File(string InTable, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string PhysicalName = "";

            // InMode = 1 = Bulk 
            

         //   [RRDM_Reconciliation_ITMX].[dbo].[BULK_COREBANKING]


            if (InMode ==1)
            {
                PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + "BULK_" + InTable + "]";
            }

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InMode == 1)
            {
                SqlString = "SELECT * "
             + " FROM " + PhysicalName;
                //  + " where RMCateg = @MatchingCateg AND Mat
            }
            
           
            using (SqlConnection conn =
              new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                      
                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 190;   // seconds
                        sqlAdapt.Fill(DataTableAllFields);

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


        // Find Max Date Time
        public DateTime ReadAndFindMaxDtForFile(string InTable, int InRMCycle, string InOriginFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //int RecordsInFile = 0;

            DateTime MaxDt = NullPastDate;

            if (InTable == "CIT_Speed_Excel")
            {
                // No Max Date - Leave it null
            }
            else
            {
                string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + InTable + "]";


                SqlString =

               " SELECT  ISNULL(MAX([TransDate]),'1900-01-01') As MaxDt "
               + " FROM " + PhysicalName
               + " WHERE LoadedAtRMCycle=@LoadedAtRMCycle AND OriginFileName = @OriginFileName"
               + " AND ([TransDate] <> '2050-12-31 00:00:00.000') "
               + " ";

                using (SqlConnection conn =
                      new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                            cmd.Parameters.AddWithValue("@OriginFileName", InOriginFileName);
                            cmd.CommandTimeout = 150;
                            // Read table 
                            MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
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


            return MaxDt;
        }

        // Find Max Date Time
        public DateTime ReadAndFindMaxDtForFile_MOBILE(string InApplication, string InTable, int InRMCycle, string InOriginFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //int RecordsInFile = 0;
            string PhysicalName = "";

            string ConnectionString = "EMPTY"; 
            string ETISALATConnectionString = AppConfig.GetConnectionString("ETISALATConnectionString");
            string QAHERAConnectionString = AppConfig.GetConnectionString("QAHERAConnectionString");
            string ATMSconnectionString = AppConfig.GetConnectionString("ATMSConnectionString");

            DateTime MaxDt = NullPastDate;

            if (InApplication == "QAHERA")
            {
                ConnectionString = QAHERAConnectionString; 

                if (InTable == "QAHERA_TPF_TXNS")
                {
                    PhysicalName = "[QAHERA].[dbo]." + "[QAHERA_TPF_Txns_MASTER]";
                }
                else
                {
                    PhysicalName = "[QAHERA].[dbo]." + "[" + InTable + "]";
                }
            }

            if (InApplication == "ETISALAT")
            {
                ConnectionString = ETISALATConnectionString; 

                if (InTable == "ETISALAT_TPF_TXNS")
                {
                    PhysicalName = "[ETISALAT].[dbo]." + "[ETISALAT_TPF_Txns_MASTER]";
                }
                else
                {
                    PhysicalName = "[ETISALAT].[dbo]." + "[" + InTable + "]";
                }
            }

            if (InApplication == "EGATE")
            {
                ConnectionString = ATMSconnectionString;

                
                    PhysicalName = "[EGATE].[dbo]." + "[" + InTable + "]";
                
            }


            SqlString =

               " SELECT  ISNULL(MAX([TransDate]),'1900-01-01') As MaxDt "
               + " FROM " + PhysicalName
               + " WHERE LoadedAtRMCycle=@LoadedAtRMCycle "
               + " ";

                using (SqlConnection conn =
                      new SqlConnection(ConnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                            //cmd.Parameters.AddWithValue("@OriginFileName", InOriginFileName);
                            cmd.CommandTimeout = 150;
                            // Read table 
                            MaxDt = Convert.ToDateTime(cmd.ExecuteScalar());
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            return MaxDt;
        }
        // Find Maximum Date
        public DateTime ReadAndFindMaxDt_NO_ATM(string InTable, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDt = NullPastDate;

            SqlString =
        " SELECT TOP 1 * "
        + " FROM " + InTable
        + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND ResponseCode = '0' "
                  + " ORDER By TransDate DESC, RRNumber DESC ";


            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            MaxDt = TransDate;


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
            return MaxDt;
        }

        // Find Maximum Date based 
        public DateTime ReadAndFindMaxDt_NO_ATM_BASED_On_CAP_DATE(string InTable, string InMatchingCateg, DateTime InCAP_DATE)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MaxDt = NullPastDate;

            SqlString =
        " SELECT TOP 1 * "
        + " FROM " + InTable
        + " WHERE MatchingCateg = @MatchingCateg AND CAP_DATE=@CAP_DATE AND ResponseCode = '0' "
                  + " ORDER By TransDate DESC, RRNumber DESC ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            MaxDt = TransDate;


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
            return MaxDt;
        }

        public int NumberOfTXNSRemain;

        public decimal TotalAmountRemain;

        // Find Totals greater that some date 
        public void ReadAndFindTotals_NO_ATM_BASED_On_LAST_DATE(string InTable, string InMatchingCateg
                                         , DateTime InSET_DATE, DateTime InLastDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfTXNSRemain = 0;

            TotalAmountRemain = 0;

            SqlString =

                " SELECT count(*) as NumberOfTXNS, sum(TransAmt) As TotalAmount "
                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] "
                + " WHERE MatchingCateg = @MatchingCateg AND SET_DATE = @SET_DATE and TransDate > @TransDate ";
            //  + " WHERE MatchingCateg = 'BDC275' AND CAP_DATE = '2020-11-10' and TransDate > '2020-11-10 16:53:13.000' ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@SET_DATE", InSET_DATE);
                        cmd.Parameters.AddWithValue("@TransDate", InLastDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfTXNSRemain = (int)rdr["NumberOfTXNS"];
                            if (NumberOfTXNSRemain > 0)
                            {
                                TotalAmountRemain = (decimal)rdr["TotalAmount"];
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


        int LastTraceNo;
        // Find Max Trace for other files than Mpa 
        public int ReadAndFindMaxSeqNo(string InTable, string InMatchingCateg, string InTerminalId, int In_DB_Mode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            LastTraceNo = 0;

            SqlString =
            " SELECT   TOP 1 * "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId  "
            + " Order by TransDate DESC ";

            using (SqlConnection conn =
                         new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            LastTraceNo = TraceNo = (int)rdr["TraceNo"];

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
            return LastTraceNo;
        }



        int MaxPrimaryMatchingField;

        public void UpdateSourceTablesAsProcessed_NonAtms_Based_SET_DATE(string InMatchingCateg, string InFileId,
                                      DateTime InSET_DATE, int InRMCycle, string InMask)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle, Mask = @Mask "
                                   + " WHERE MatchingCateg = @MatchingCateg "
                                   + " AND SET_DATE <= @SET_DATE"
                                   + " AND Processed = 0 AND ResponseCode = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@SET_DATE", InSET_DATE);
                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Mask", InMask);

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

        public void UpdateSourceTablesAsProcessed_NonAtms(string InMatchingCateg, string InFileId,
                                      DateTime InTransDate, int InRMCycle, string InMask)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle, Mask = @Mask "
                                   + " WHERE MatchingCateg = @MatchingCateg "
                                   + " AND TransDate <= @TransDate"
                                   + " AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InTransDate);
                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Mask", InMask);

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
        // UPDATE SOURCE Tables as Processed FOR Master - third file 
        // 
        public void UpdateSourceTablesAsProcessed_OurAtms_But_Master_ThirdFile(string InWorkingTable, string InMatchingCateg, string InFileId,
                                      int InRMCycle, string InMatchingFields)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string WorkingTableName = "";
            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            string SqlCmd =
                       "   UPDATE "
                      + "    C1 "
                      + " SET "
                      + "  C1.AmtFileBToFileC = 9.99 "
                      + " FROM "

                      + WorkingTableName + "  As C1 "
                      + "    INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03] "
                     + "        AS C2 "
                     + "         ON "
                     + InMatchingFields

                     + " UPDATE t1 "
                     + " SET IsMatchingDone = 1, Matched = 1 ,MatchingAtRMCycle = @ProcessedAtRmCycle ,MatchMask = '111' "
                     + " FROM "

                     + WorkingTableName + " t2 "
                     + " INNER JOIN "

                     + InFileId + " t1 " // THIS Master file 
                     + "  ON "
                     + "  t1.SeqNo = t2.SeqNo "
                     + "  WHERE  (t1.IsMatchingDone = 0) AND t2.AmtFileBToFileC = 9.99 "
                     + " AND t1.MatchingCateg = @MatchingCateg "
                       ;


            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        //   cmd.Parameters.AddWithValue("@Mask", InMask);

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
                    return;
                }
        }


        //
        // UPDATE SOURCE Tables as Processed FOR OUR ATMs - IST - Third File 
        // 
        public void UpdateSourceTablesAsProcessed_OurAtms_But_POS_ThirdFile(string InWorkingTable, string InMatchingCateg, string InFileId,
                                      int InRMCycle, string InMatchingFields)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string WorkingTableName = "";
            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            string SqlCmd =
                       "   UPDATE "
                      + "    C1 "
                      + " SET "
                      + "  C1.AmtFileBToFileC = 9.99 "
                      + " FROM "

                      + WorkingTableName + "  As C1 "
                      + "    INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03] "
                     + "        AS C2 "
                     + "         ON "
                     + InMatchingFields
                     //+ "         c2.RRNumber = c1.RRNumber "
                     //+ "        AND  c2.TransAmt = c1.TransAmt "
                     //+ "       AND c2.TransDate = c1.TransDate "
                     //+ "       AND c2.TransType = c1.TransType "

                     + " UPDATE t1 "
                     + " SET [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle, Mask = '' "
                     + " FROM "

                     + WorkingTableName + " t2 "
                     + " INNER JOIN "

                     + InFileId + " t1 " // THIS IS IST 
                     + "  ON "
                     + "  t1.SeqNo = t2.SeqNo "
                     + "  WHERE  (t1.Processed = 0) AND t2.AmtFileBToFileC = 9.99 "
                     + " AND t1.[MatchingCateg]= @MatchingCateg "
                       ;


            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        //   cmd.Parameters.AddWithValue("@Mask", InMask);

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
                    return;
                }
        }

        //
        // UPDATE SOURCE Tables as Processed FOR NON ATMs - First File 
        // 
        public void UpdateSourceTablesAsProcessed_Records_POS_Type(string InWorkingTable, string InMatchingCateg, string InFileId,
                                      int InRMCycle, string InMatchingFields)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string WorkingTableName = "";
            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            string SqlCmd =
                       "   UPDATE "
                      + "    C1 "
                      + " SET "
                      + "  C1.AmtFileBToFileC = 9.99 "
                      + " FROM "

                      + WorkingTableName + "  As C1 "
                      + "    INNER JOIN[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01] "
                     + "        AS C2 "
                     + "         ON "
                     + InMatchingFields
                     //+ "         c2.RRNumber = c1.RRNumber "
                     //+ "        AND  c2.TransAmt = c1.TransAmt "
                     //+ "       AND c2.TransDate = c1.TransDate "
                     //+ "       AND c2.TransType = c1.TransType "
                     //**********************
                     // SECOND PART OF THE COMMAND 
                     //**********************
                     + " UPDATE t1 "
                     + " SET [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle, Mask = '' "
                     + " FROM "

                     + WorkingTableName + " t2 "
                     + " INNER JOIN "

                     + InFileId + " t1 " //
                     + "  ON "
                     + "  t1.SeqNo = t2.SeqNo "
                     + "  WHERE  (t1.Processed = 0) AND t2.AmtFileBToFileC = 9.99 "
                     + " AND t1.[MatchingCateg]= @MatchingCateg "

            ;


            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        //   cmd.Parameters.AddWithValue("@Mask", InMask);

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
                    return;
                }
        }

        //
        // UPDATE SOURCE Tables as Processed FOR NON ATMs 
        // 
        public void UpdateMaster_POS_Mpa_Mask(string InMatchingCateg, string InFileId,
                                      int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";
            //
            // InFileId  is Master POS
            //
            string SqlCmd =
                       "   UPDATE "
                      + "    C1 "
                      + " SET "
                      + "  C1.Mask = C2.MatchMask "
                      + " FROM "
                      + InFileId + "  As C1 "
                      + "  INNER JOIN "
                      + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                     + "        AS C2 "
                     + "         ON "
                     + " c1.SeqNo = c2.OriginalRecordId "

                     + " WHERE "
                     + " C2.MatchingCateg = @MatchingCateg AND C2.MatchingAtRMCycle = @ProcessedAtRmCycle"
                     + " AND C1.MatchingCateg = @MatchingCateg AND C1.ProcessedAtRMCycle = @ProcessedAtRmCycle"
                       ;

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);
                        //   cmd.Parameters.AddWithValue("@Mask", InMask);

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
                    return;
                }
        }
        //
        // UPDATE SOURCE Tables as Processed FOR NON ATMs 
        // 
        public void UpdateMaster_POS_WithSettled(string InMatchingCateg, string InFileId,
                                      int InRMCycle, string InMASK)
        {
            // It makes settled all with Mask = '111'
            ErrorFound = false;
            ErrorOutput = "";
            //
            // InFileId  is Master POS
            //
            string SqlCmd =
                       "   UPDATE " + InFileId
                      + " SET "
                      + "  IsSettled = 1 "
                     + " WHERE "
                     + " MatchingCateg = @MatchingCateg AND ProcessedAtRMCycle = @ProcessedAtRmCycle"
                     + " AND MASK = @MASK "
                       ;

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@MASK", InMASK);

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
                    return;
                }
        }

        //
        // UPDATE CAP_DATE TOTALS BY READING IST  FOR CATEGORY
        // 
        public void CreateRecords_CAP_DATE_For_Category(string InOperator, DateTime InCAP_DATE, int InRMCycle, int InMode)
        {

            return; // Maybe we need it at later stage 

            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime CAP_DATE = Net_TransDate;

            bool FirstRecord = true;
            string OldMatchingCateg = "";
            DateTime OldCAP_DATE = NullPastDate;

            string WOriginFile = "Switch_IST_Txns";

            decimal TurnOverDebit = 0;
            decimal TurnOverCredit = 0;
            int TransDR = 0;
            int TransCR = 0;

            int ReversalsTxns = 0;
            decimal ReversalsAmt = 0;

            decimal UnMatchedDebit = 0;
            decimal UnMatchedCredit = 0;
            int UnMatchedTransDR = 0;
            int UnMatchedTransCR = 0;

            if (InMode == 1)
            {
                // GL Affecting Categories
                SqlString = "SELECT  "
                     + " MatchingCateg, TerminalId, TransType, TransAmt, TransDate "
                     + " , ResponseCode, Processed, ProcessedAtRMCycle, Mask, Comment, CAP_DATE "
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                     + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle AND ResponseCode='0'"
                     //+ " WHERE Processed = 1 AND ResponseCode = '0' AND ProcessedAtRMCycle = @ProcessedAtRMCycle "
                     + " ORDER BY MatchingCateg, CAP_DATE, TransDate  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            MatchingCateg = (string)rdr["MatchingCateg"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransAmt = (decimal)rdr["TransAmt"];

                            TransDate = (DateTime)rdr["TransDate"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            Processed = (bool)rdr["Processed"];
                            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
                            Mask = (string)rdr["Mask"];

                            Comment = (string)rdr["Comment"];
                            CAP_DATE = (DateTime)rdr["CAP_DATE"];

                            // Check for 
                            if (FirstRecord == true)
                            {
                                // Do What you have to do
                                OldMatchingCateg = MatchingCateg;
                                OldCAP_DATE = CAP_DATE;
                                FirstRecord = false;

                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, OldCAP_DATE, WOriginFile);
                                if (Cgl.RecordFound == true)
                                {

                                }
                                else
                                {
                                    // Then Insert
                                    int Mode = 1;
                                    Cgl.CAP_DATE = OldCAP_DATE;
                                    Cgl.MatchingCateg = MatchingCateg;
                                    Cgl.AtmNo = "";
                                    Cgl.CreatedAtRMCycle = InRMCycle;
                                    Cgl.FirstDtTm = TransDate;
                                    Cgl.GL_Number = "Not available";
                                    Cgl.DateCreated = DateTime.Now;
                                    Cgl.Operator = InOperator;

                                    Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                    // Initialise

                                }
                            }


                            if (CAP_DATE != OldCAP_DATE & MatchingCateg == OldMatchingCateg)
                            {
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, OldCAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)
                                {

                                    Cgl.LastDtTm = TransDate;

                                    Cgl.UpdatedAtRMCycle = InRMCycle;

                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;


                                    // Then Update
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);

                                    // Then Insert new 
                                    int Mode = 1;
                                    Cgl.CAP_DATE = CAP_DATE; // New Date 
                                    Cgl.MatchingCateg = MatchingCateg;
                                    Cgl.AtmNo = "";
                                    Cgl.CreatedAtRMCycle = InRMCycle;
                                    Cgl.FirstDtTm = TransDate;
                                    Cgl.GL_Number = "Not available";
                                    Cgl.DateCreated = DateTime.Now;
                                    Cgl.Operator = InOperator;

                                    Cgl.Insert_CAP_DATE(Mode, WOriginFile);
                                }



                                // Initialise

                                TurnOverDebit = 0;
                                TurnOverCredit = 0;
                                TransDR = 0;
                                TransCR = 0;
                                ReversalsTxns = 0;
                                ReversalsAmt = 0;
                                UnMatchedDebit = 0;
                                UnMatchedCredit = 0;
                                UnMatchedTransDR = 0;
                                UnMatchedTransCR = 0;


                            }
                            if (MatchingCateg != OldMatchingCateg)
                            {
                                // Update 
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(OldMatchingCateg, OldCAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)

                                {

                                    Cgl.LastDtTm = TransDate;
                                    Cgl.UpdatedAtRMCycle = InRMCycle;
                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                    // Then Update
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);

                                    // Then Insert new if Does not exist
                                    // Look if Record exist
                                    Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);
                                    if (Cgl.RecordFound == true)
                                    {

                                    }
                                    else
                                    {
                                        // Then Insert
                                        int Mode = 1;
                                        Cgl.CAP_DATE = CAP_DATE;
                                        Cgl.MatchingCateg = MatchingCateg;
                                        Cgl.AtmNo = "";
                                        Cgl.CreatedAtRMCycle = InRMCycle;
                                        Cgl.FirstDtTm = TransDate;
                                        Cgl.GL_Number = "Not available";
                                        Cgl.DateCreated = DateTime.Now;
                                        Cgl.Operator = InOperator;

                                        Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                        // Initialise

                                    }

                                }

                                // Initialise

                                TurnOverDebit = 0;
                                TurnOverCredit = 0;
                                TransDR = 0;
                                TransCR = 0;
                                ReversalsTxns = 0;
                                ReversalsAmt = 0;
                                UnMatchedDebit = 0;
                                UnMatchedCredit = 0;
                                UnMatchedTransDR = 0;
                                UnMatchedTransCR = 0;

                            }

                            // WORK ON Counters
                            if (TransType == 11 & Comment != "Reversals")
                            {
                                TurnOverDebit = TurnOverDebit + TransAmt;
                                TransDR = TransDR + 1;
                            }

                            if (TransType == 23 & Comment != "Reversals")
                            {
                                TurnOverCredit = TurnOverCredit + TransAmt;
                                TransCR = TransCR + 1;
                            }


                            if (Mask.Contains("0") == true || Mask.Contains("R") == true)
                            {
                                // Then Issue
                                if (TransType == 11 & Comment != "Reversals")
                                {
                                    UnMatchedDebit = UnMatchedDebit + TransAmt;
                                    UnMatchedTransDR = UnMatchedTransDR + 1;
                                }

                                if (TransType == 23 & Comment != "Reversals")
                                {
                                    UnMatchedCredit = UnMatchedCredit + TransAmt;
                                    UnMatchedTransCR = UnMatchedTransCR + 1;
                                }
                            }

                            // REVERSALS
                            if (TransType == 11 & Comment == "Reversals")
                            {
                                ReversalsTxns = ReversalsTxns + 1;
                                ReversalsAmt = ReversalsAmt + TransAmt;
                            }


                            // Intialised Old
                            OldMatchingCateg = MatchingCateg;
                            OldCAP_DATE = CAP_DATE;

                        }


                        // Close Reader
                        rdr.Close();

                        // Close conn
                        conn.Close();

                        // LAST RECORD
                        if (RecordFound == true)
                        {
                            Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);
                            if (Cgl.RecordFound == true)
                            {

                                Cgl.LastDtTm = TransDate;
                                Cgl.UpdatedAtRMCycle = InRMCycle;
                                Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                Cgl.TransDR = Cgl.TransDR + TransDR;
                                Cgl.TransCR = Cgl.TransCR + TransCR;

                                Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                // Then Update
                                Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);
                            }
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
        // UPDATE TOTALS BY READING CATEGORY ORIGIN FILE 
        // ORIGIN FILE 
        public void CreateRecords_GL_ENTRIES_For_Category(string InOperator, string InMatchingCateg, DateTime InCAP_DATE, int InRMCycle, int InMode)
        {

            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();
            RRDMMatchingCategoriesVsSourcesFiles Mcsf = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMMatchingSourceFiles Msourcef = new RRDMMatchingSourceFiles();
            RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

            Mcsf.ReadReconcCategoriesVsSourcesAll(InMatchingCateg);
            Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
            string SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int WSeqNo = 0;
            DateTime CAP_DATE = Net_TransDate;
            string WOriginFile = Msourcef.InportTableName;

            DateTime WFirstDtTm = NullPastDate;

            bool FirstRecord = true;
            string OldMatchingCateg = "";
            DateTime OldCAP_DATE = NullPastDate;
            string OldOriginFileName = "";

            decimal TurnOverDebit = 0;
            decimal TurnOverCredit = 0;
            int TransDR = 0;
            int TransCR = 0;

            int ReversalsTxns = 0;
            decimal ReversalsAmt = 0;

            decimal UnMatchedDebit = 0;
            decimal UnMatchedCredit = 0;
            int UnMatchedTransDR = 0;
            int UnMatchedTransCR = 0;

            if (InMode == 3)
            {
                // GL Affecting Categories
                SqlString = "SELECT  "
                     + " OriginFileName, MatchingCateg, TerminalId, TransType, TransAmt, TransDate "
                     + " , ResponseCode, Processed, ProcessedAtRMCycle, Mask, Comment, CAP_DATE "
                     + " FROM " + SourceTable_A
                     + " WHERE MatchingCateg = @MatchingCateg AND LoadedAtRMCycle = @LoadedAtRMCycle AND ResponseCode='0'"
                     + " ORDER BY OriginFileName,CAP_DATE ,TransDate  ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            OriginFileName = (string)rdr["OriginFileName"];

                            MatchingCateg = (string)rdr["MatchingCateg"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransAmt = (decimal)rdr["TransAmt"];

                            TransDate = (DateTime)rdr["TransDate"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            Processed = (bool)rdr["Processed"];
                            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
                            Mask = (string)rdr["Mask"];

                            Comment = (string)rdr["Comment"];
                            CAP_DATE = (DateTime)rdr["CAP_DATE"];

                            // Check for 
                            if (FirstRecord == true)
                            {
                                // Do What you have to do
                                OldOriginFileName = OriginFileName;
                                FirstRecord = false;

                                WSeqNo = Convert.ToInt32(OldOriginFileName);

                                CAP_DATE = Rfm.ReadLoadedFilesBySeqNoToFindDate(WSeqNo);

                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)
                                {
                                    WFirstDtTm = Cgl.FirstDtTm;
                                }
                                else
                                {
                                    WFirstDtTm = TransDate;
                                }


                            }

                            if (OriginFileName != OldOriginFileName)
                            {
                                WSeqNo = Convert.ToInt32(OldOriginFileName);

                                CAP_DATE = Rfm.ReadLoadedFilesBySeqNoToFindDate(WSeqNo);

                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)
                                {
                                    WFirstDtTm = Cgl.FirstDtTm;
                                }
                                else
                                {
                                    RRDMMatchingCategories Mc = new RRDMMatchingCategories();
                                    Mc.ReadMatchingCategorybyActiveCategId(InOperator, MatchingCateg);
                                    // Then Insert
                                    int Mode = 1;
                                    Cgl.CAP_DATE = CAP_DATE;
                                    Cgl.MatchingCateg = MatchingCateg;
                                    Cgl.AtmNo = "";
                                    Cgl.CreatedAtRMCycle = InRMCycle;
                                    Cgl.FirstDtTm = WFirstDtTm;
                                    Cgl.GL_Number = Mc.GlAccount;
                                    Cgl.DateCreated = DateTime.Now;
                                    Cgl.Operator = InOperator;

                                    Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                    // Initialise

                                }
                                // Update
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)
                                {
                                    Cgl.LastDtTm = TransDate;
                                    Cgl.UpdatedAtRMCycle = InRMCycle;
                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                    // Then Update
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);

                                }

                                // Initialise

                                TurnOverDebit = 0;
                                TurnOverCredit = 0;
                                TransDR = 0;
                                TransCR = 0;
                                ReversalsTxns = 0;
                                ReversalsAmt = 0;
                                UnMatchedDebit = 0;
                                UnMatchedCredit = 0;
                                UnMatchedTransDR = 0;
                                UnMatchedTransCR = 0;

                            }

                            // WORK ON Counters
                            if (TransType == 11 & Comment != "Reversals")
                            {
                                TurnOverDebit = TurnOverDebit + TransAmt;
                                TransDR = TransDR + 1;
                            }

                            if (TransType == 21 & Comment != "Reversals")
                            {
                                TurnOverCredit = TurnOverCredit + TransAmt;
                                TransCR = TransCR + 1;
                            }
                            //
                            // Discrepancies 
                            //
                            if (Mask.Contains("0") == true || Mask.Contains("R") == true)
                            {
                                // Then Issue
                                if (TransType == 11 & Comment != "Reversals")
                                {
                                    UnMatchedDebit = UnMatchedDebit + TransAmt;
                                    UnMatchedTransDR = UnMatchedTransDR + 1;
                                }

                                if (TransType == 21 & Comment != "Reversals")
                                {
                                    UnMatchedCredit = UnMatchedCredit + TransAmt;
                                    UnMatchedTransCR = UnMatchedTransCR + 1;
                                }
                            }
                            // REVERSALS
                            if (TransType == 11 & Comment == "Reversals")
                            {
                                ReversalsTxns = ReversalsTxns + 1;
                                ReversalsAmt = ReversalsAmt + TransAmt;
                            }

                            // Intialised Old
                            OldOriginFileName = OriginFileName;
                            //OldCAP_DATE = CAP_DATE;

                        }
                        // **************
                        // LAST RECORD                    
                        // **************
                        if (OldOriginFileName != "")
                        {
                            WSeqNo = Convert.ToInt32(OldOriginFileName);

                            CAP_DATE = Rfm.ReadLoadedFilesBySeqNoToFindDate(WSeqNo);

                            Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);

                            if (Cgl.RecordFound == true)
                            {
                                Cgl.LastDtTm = TransDate;
                                Cgl.UpdatedAtRMCycle = InRMCycle;
                                Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                Cgl.TransDR = Cgl.TransDR + TransDR;
                                Cgl.TransCR = Cgl.TransCR + TransCR;

                                Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                // Then Update
                                Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);
                            }
                            else
                            {
                                RRDMMatchingCategories Mc = new RRDMMatchingCategories();
                                Mc.ReadMatchingCategorybyActiveCategId(InOperator, MatchingCateg);
                                // Then Insert
                                int Mode = 1;
                                Cgl.CAP_DATE = CAP_DATE;
                                Cgl.MatchingCateg = MatchingCateg;
                                Cgl.AtmNo = "";
                                Cgl.CreatedAtRMCycle = InRMCycle;
                                Cgl.FirstDtTm = WFirstDtTm;
                                Cgl.GL_Number = Mc.GlAccount;
                                Cgl.DateCreated = DateTime.Now;
                                Cgl.Operator = InOperator;

                                Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                // Update
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_Category_CapDate(MatchingCateg, CAP_DATE, WOriginFile);

                                if (Cgl.RecordFound == true)
                                {
                                    Cgl.LastDtTm = TransDate;
                                    Cgl.UpdatedAtRMCycle = InRMCycle;
                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                    // Then Update
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, Cgl.MatchingCateg, "", Cgl.CreatedAtRMCycle, 1);

                                }
                            }

                            // Close Reader
                            rdr.Close();

                            // Close conn
                            conn.Close();
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
        // UPDATE TOTALS BY READING CATEGORY ORIGIN FILE 
        // ORIGIN FILE 
        public void CreateRecords_GL_ENTRIES_For_Category_SecondVersion(string InOperator, string InFileId, string InCategories, int InRMCycle, int InMode)
        {
            // InMode 1 = Issuer 
            // InMode 2 = Acquirer


            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();

            string SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

            int NoOfTxns;
            decimal TotAmount;

            DataTableFileTotals = new DataTable();
            DataTableFileTotals.Clear();

            //decimal JournalAmt = 0;
            decimal TotalUnMatchedAmt = 0;
            int TotalUnMatchedTxns = 0;

            decimal TotalMatchedAmt = 0;
            int TotalMatchedTxns = 0;

            bool Matched;

            SqlString =
              " SELECT CAP_DATE,Count(SeqNo) As NoOfTxns, "
            + " sum(TransAmount) as TotAmount , Matched "
            + " FROM " + SourceTable_A
            + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
            + " and MatchingCateg in "
            + InCategories
            + " Group by CAP_DATE, Matched "
            + " order by CAP_DATE, Matched ";

            using (SqlConnection conn =
                        new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Categories", "('BDC210', 'BDC211')");

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableFileTotals);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    return;
                }

            // For two testing ATMs four records in table are created 
            // Matched and Unmatched for each 

            // READ TABLE ENTRIES AND UPDATE RECONCILIATION SESSION

            try
            {
                DateTime WCAP_DATE = NullPastDate;
                DateTime PreviousDate = NullPastDate;
                int I = 0;
                int K = DataTableFileTotals.Rows.Count - 1;
                bool VariablesEqual = false;

                if (I == K) VariablesEqual = true;

                while (I <= K)
                {

                    RecordFound = true;

                    WCAP_DATE = (DateTime)DataTableFileTotals.Rows[I]["CAP_DATE"];
                    if ((WCAP_DATE != PreviousDate & I != 0))
                    {
                        // Insert GL Transaction

                        Cgl.CAP_DATE = PreviousDate.Date;

                        Cgl.OriginFile = InFileId;

                        if (InMode == 1)
                        {
                            Cgl.MatchingCateg = InCategories;
                            Cgl.GL_Number = "123_Issuer";
                        }

                        if (InMode == 2)
                        {
                            Cgl.MatchingCateg = InCategories;
                            Cgl.GL_Number = "123_Acquirer";
                        }

                        Cgl.TurnOverDebit = TotalMatchedAmt;

                        Cgl.TransDR = TotalMatchedTxns;

                        Cgl.UnMatchedDebit = TotalUnMatchedAmt;

                        Cgl.UnMatchedTransDR = TotalUnMatchedTxns;

                        // Cgl.Insert_Settlement_DATE_For_Non_ATMS(InOperator, InRMCycle); 

                        // MAKE zero after insert of transaction 
                        TotalUnMatchedAmt = 0;
                        TotalUnMatchedTxns = 0;

                        TotalMatchedAmt = 0;
                        TotalMatchedTxns = 0;
                    }

                    NoOfTxns = (int)DataTableFileTotals.Rows[I]["NoOfTxns"];
                    TotAmount = (decimal)DataTableFileTotals.Rows[I]["TotAmount"];
                    Matched = (bool)DataTableFileTotals.Rows[I]["Matched"];

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

                    if (I == K) // Last Record
                    {
                        // Insert GL Transaction

                        Cgl.CAP_DATE = WCAP_DATE;

                        Cgl.OriginFile = InFileId;

                        if (InMode == 1)
                        {
                            Cgl.MatchingCateg = InCategories;
                            Cgl.GL_Number = "123_Issuer";
                        }

                        if (InMode == 2)
                        {
                            Cgl.MatchingCateg = InCategories;
                            Cgl.GL_Number = "123_Acquirer";
                        }

                        Cgl.TurnOverDebit = TotalMatchedAmt;

                        Cgl.TransDR = TotalMatchedTxns;

                        Cgl.UnMatchedDebit = TotalUnMatchedAmt;

                        Cgl.UnMatchedTransDR = TotalUnMatchedTxns;

                        // Cgl.Insert_Settlement_DATE_For_Non_ATMS(InOperator, InRMCycle);


                    }
                    PreviousDate = WCAP_DATE;

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }


        }

        //
        // UPDATE TOTALS BY READING CATEGORY ORIGIN FILE 
        // ORIGIN FILE 
        public void CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(string InOperator, string InFileId,
                        string InCategories, int InRMCycle, string InIdentity, int InMode)
        {
            // InMode 1 = Issuer 
            // InMode 2 = Acquirer
            // 3 = credit card 
            // 4 = "MEEZA TXNS_Bank_Is_Issuer"
            // 5 = TELDA 

            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();

            string SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

            int NoOfTxns;
            decimal TotAmount;

            DataTableFileTotals = new DataTable();
            DataTableFileTotals.Clear();

            //decimal JournalAmt = 0;
            decimal TotalUnMatchedAmt = 0;
            int TotalUnMatchedTxns = 0;

            decimal TotalMatchedAmt = 0;
            int TotalMatchedTxns = 0;

            bool Matched;
            if (InMode == 1)
            {
                SqlString =
            "  WITH MergedTbl_2 AS "
       + "  ( "
       + "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,6) as Card_BIN, "
     //  + " TXNDEST,"
       + " TransType , TransCurr As Ccy, "
       + "    SUM(TransAmount) as TotAmount"
       + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV " 
       + " , Count(*) As NoOfTxns "
       + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
       + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
       + "    AND MatchingCateg in " + InCategories
       + "    AND left(MatchMask,1) = '1' AND ResponseCode = '0' "  // AND ResponseCode = '0' 
       + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,6)"
       //+ " ,TXNDEST "
       + ", TransType, TransCurr "

       + "	UNION ALL "

        + "	SELECT SET_DATE,'Non_CB' AS CentralBank,MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,6) as Card_BIN, "
        //  + " TXNDEST,"
        + " TransType , TransCurr As Ccy, "
        + "    SUM(TransAmount) as TotAmount "
        + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
        + ", Count(*) As NoOfTxns "
        + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
        + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
         + "   AND MatchingCateg in " + InCategories
         + "   AND Matched = 0 AND ResponseCode = '0' "
        + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,6) "
        //+ " ,TXNDEST "
        + " , TransType, TransCurr "
        + "	) "
        + "	SELECT * FROM MergedTbl_2 "
        + "	ORDER by SET_DATE, CentralBank, MatchingCateg,Card_BIN ";

            }

            if (InMode == 2)
            {
                SqlString =
            "  WITH MergedTbl_2 AS "
       + "  ( "
       + "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg, left(MatchMask,1) as Mask,'BINxxx' AS Card_BIN,  "
          //  + " TXNDEST,"
          + " TransType , TransCurr As Ccy, "
       + "    SUM(TransAmount) as TotAmount "
       + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
       + ", Count(*) As NoOfTxns "
       + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
       + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
       + "    AND MatchingCateg in " + InCategories
       + "    AND left(MatchMask,1) = '1' AND ResponseCode = '0'  "
       + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1) "
       //+ " ,TXNDEST "
       + "   , TransType, TransCurr "

       + "	UNION ALL "

        + "	SELECT SET_DATE,'Non_CB' AS CentralBank,MatchingCateg As MatchingCateg, left(MatchMask,1) as Mask,'BINxxx' AS Card_BIN, "
           //  + " TXNDEST,"
           + " TransType , TransCurr As Ccy, "
        + "    SUM(TransAmount) as TotAmount "
        + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
        + " , Count(*) As NoOfTxns "
        + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
        + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
         + "   AND MatchingCateg in " + InCategories
         + "   AND Matched = 0 AND ResponseCode = '0'  "
        + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1)"
        //+ " ,TXNDEST "
        + " , TransType, TransCurr "
        + "	) "
        + "	SELECT * FROM MergedTbl_2 "
        + "	ORDER by SET_DATE, CentralBank, MatchingCateg ";

            }

            if (InMode == 3)
            {
                // Like Master
                SqlString =
            "  WITH MergedTbl_2 AS "
       + "  ( "
       + "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg,  left(MatchMask,1) as Mask,left(CardNumber,6) as Card_BIN, "
         + " TransType , TransCurr As Ccy, "
       + "    SUM(TransAmount) as TotAmount "
       + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
       + " , Count(*) As NoOfTxns "
       + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
       + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
       + "    AND MatchingCateg in " + InCategories
       + "    AND left(MatchMask,1) = '1' AND ResponseCode = '0' "
       + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,6), TransType, TransCurr "

       + "	UNION ALL "

        + "	SELECT SET_DATE,'Non_CB' AS CentralBank ,MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,6) as Card_BIN, "
          + " TransType , TransCurr As Ccy, "
        + "    SUM(TransAmount) as TotAmount "
        + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
        +  ", Count(*) As NoOfTxns "
        + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
        + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
         + "   AND MatchingCateg in " + InCategories
         + "   AND Matched = 0 AND ResponseCode = '0' "
        + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,6), TransType, TransCurr "
        + "	) "
        + "	SELECT * FROM MergedTbl_2 "
        + "	ORDER by SET_DATE, CentralBank, MatchingCateg,Card_BIN, TransType ";

            }

            if (InMode == 4)
            {
                SqlString =
            "  WITH MergedTbl_2 AS "
       + "  ( "
       + "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
       + " TransType , TransCurr As Ccy, "
       + "    SUM(TransAmount) as TotAmount "
       + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
       + ", Count(*) As NoOfTxns "
       + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
       + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
       + "    AND MatchingCateg in " + InCategories
       + "    AND left(MatchMask,1) = '1' AND ResponseCode = '0' "
       + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8), TransType, TransCurr "

       + "	UNION ALL "

        + "	SELECT SET_DATE,'Non_CB' AS CentralBank,MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
        + " TransType , TransCurr As Ccy, "
        + "    SUM(TransAmount) as TotAmount "
        + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
        + ", Count(*) As NoOfTxns "
        + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
        + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
         + "   AND MatchingCateg in " + InCategories
         + "   AND Matched = 0 AND ResponseCode = '0' "
        + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8) , TransType, TransCurr "
        + "	) "
        + "	SELECT * FROM MergedTbl_2 "
        + "	ORDER by SET_DATE, CentralBank, MatchingCateg,Card_BIN ";

            }

            if (InMode == 5)
            {
                SqlString =
      "  WITH MergedTbl_2 AS "
 + "  ( "
 + "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
 + " TransType , TransCurr As Ccy, "
 + "    SUM(TransAmount) as TotAmount "
 + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
 + ", Count(*) As NoOfTxns "
 + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
 + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "

  + "    AND (left(Cardnumber, 8) = '54844600' and MatchingCateg = 'BDC279')"
 + "    AND left(MatchMask,1) = '1' AND ResponseCode = '0' "
 + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8), TransType, TransCurr "

 + "	UNION ALL "

  + "	SELECT SET_DATE,'Non_CB' AS CentralBank,MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
  + " TransType , TransCurr As Ccy, "
  + "    SUM(TransAmount) as TotAmount "
  + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
  + ", Count(*) As NoOfTxns "
  + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
  + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "

  + "    AND (left(Cardnumber, 8) = '54844600' and MatchingCateg = 'BDC279')"
   + "   AND Matched = 0 AND ResponseCode = '0' "
  + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8) , TransType, TransCurr "
  + "	) "
  + "	SELECT * FROM MergedTbl_2 "
  + "	ORDER by SET_DATE, CentralBank, MatchingCateg,Card_BIN ";

                //         SqlString =
                //     "  WITH MergedTbl_2 AS "
                //+ "  ( "
                //+ "  SELECT SET_DATE,'CB' AS CentralBank, MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
                //+ " TransType , TransCurr As Ccy, "
                //+ "    SUM(TransAmount) as TotAmount "
                //+ " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
                //+ ", Count(*) As NoOfTxns "
                //+ "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                //+ "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
                //+ "    AND ( "
                //+ "   (MatchingCateg in " + InCategories +")"
                //+ "    OR (left(Cardnumber, 8) = '54844600' and MatchingCateg = 'BDC279')"
                //+ ") "
                //+ "    AND left(MatchMask,1) = '1' AND ResponseCode = '0' "
                //+ "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8), TransType, TransCurr "

                //+ "	UNION ALL "

                // + "	SELECT SET_DATE,'Non_CB' AS CentralBank,MatchingCateg, left(MatchMask,1) as Mask,left(CardNumber,8) as Card_BIN, "
                // + " TransType , TransCurr As Ccy, "
                // + "    SUM(TransAmount) as TotAmount "
                // + " ,SUM(CAST(SpareField as decimal(18, 2))) as EQUIV "
                // + ", Count(*) As NoOfTxns "
                // + "    FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                // + "    WHERE MatchingAtRMCycle = @MatchingAtRMCycle "
                // + "    AND ( "
                //+ "   (MatchingCateg in " + InCategories +")"
                //+ "    OR (left(Cardnumber, 8) = '54844600' and MatchingCateg = 'BDC279')"
                //+ ") "

                //  + "   AND Matched = 0 AND ResponseCode = '0' "
                // + "    Group by SET_DATE,MatchingCateg, left(MatchMask,1),left(CardNumber,8) , TransType, TransCurr "
                // + "	) "
                // + "	SELECT * FROM MergedTbl_2 "
                // + "	ORDER by SET_DATE, CentralBank, MatchingCateg,Card_BIN ";

            }

            using (SqlConnection conn =
                        new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Categories", "('BDC210', 'BDC211')");

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableFileTotals);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                    return;
                }

            // For two testing ATMs four records in table are created 
            // Matched and Unmatched for each 

            // READ TABLE ENTRIES AND UPDATE RECONCILIATION SESSION

            try
            {

                int I = 0;
                int K = DataTableFileTotals.Rows.Count - 1;
                // bool VariablesEqual = false;

                //if (I == K) VariablesEqual = true;

                while (I <= K)
                {

                    RecordFound = true;

                    string WhatType = (string)DataTableFileTotals.Rows[I]["CentralBank"];
                    if (WhatType == "CB") // CENTRAL BANK RECORD
                    {
                        // Make Insert
                        Cgl.Settlement_DATE = (DateTime)DataTableFileTotals.Rows[I]["SET_DATE"];

                        Cgl.MatchingCateg = (string)DataTableFileTotals.Rows[I]["MatchingCateg"];
                        Cgl.Card_BIN = (string)DataTableFileTotals.Rows[I]["Card_BIN"];
                        Cgl.TransType = (int)DataTableFileTotals.Rows[I]["TransType"];
                        Cgl.Ccy = (string)DataTableFileTotals.Rows[I]["Ccy"];
                        Cgl.CB_TXNs = (int)DataTableFileTotals.Rows[I]["NoOfTxns"];
                        Cgl.CB_TXNs_AMT = (decimal)DataTableFileTotals.Rows[I]["TotAmount"];
                        // This is the new field for Equivalent amount
                        Cgl.EQUIV = (decimal)DataTableFileTotals.Rows[I]["EQUIV"];
                        

                        Cgl.CB_UnMatched_TXNS_1xx = 0;
                        Cgl.CB_UnMatched_TXNS_1xx_Amt = 0;

                        //Cgl.Other_Unmatched_x11 = (int)DataTableFileTotals.Rows[I]["Other_Unmatched_x11"];
                        //Cgl.Other_Unmatched_x11_Amt = (decimal)DataTableFileTotals.Rows[I]["Other_Unmatched_x11_Amt"];
                        Cgl.Other_Unmatched_x11 = 0;
                        Cgl.Other_Unmatched_x11_Amt = 0;

                        Cgl.RMCycle = InRMCycle;
                        Cgl.FileId = InFileId;
                        Cgl.CategoryGroup = InCategories;
                        Cgl.W_Identity = InIdentity;
                        Cgl.Comment = "";
                        Cgl.Operator = InOperator;

                        Cgl.Insert_Settlement_DATE_For_Non_ATMS(InOperator, InRMCycle);

                    }
                    else
                    {
                        // NON_CBE (CENTRAL BANK RECORD)
                        // CBE Record With Unmatched
                        // Make Update 
                        // Find record and Update 
                        Cgl.Settlement_DATE = (DateTime)DataTableFileTotals.Rows[I]["SET_DATE"];
                        Cgl.MatchingCateg = (string)DataTableFileTotals.Rows[I]["MatchingCateg"];
                        Cgl.Card_BIN = (string)DataTableFileTotals.Rows[I]["Card_BIN"];
                        Cgl.TransType = (int)DataTableFileTotals.Rows[I]["TransType"];
                        string WMask = (string)DataTableFileTotals.Rows[I]["Mask"];
                        Cgl.Ccy = (string)DataTableFileTotals.Rows[I]["Ccy"];

                        // Find Record HERE

                        // if (Cgl.MatchingCateg == "BDC235" || Cgl.MatchingCateg == "BDC240")
                        if (Cgl.MatchingCateg == "BDC235")
                        {

                            // CURRENCY 
                            //  Look by Currency
                            Cgl.Read_SettlementDate_For_Category_Ccy(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                                                         Cgl.Ccy, InRMCycle);
                            //}
                            //if (Cgl.MatchingCateg == "BDC240")
                            //{
                            //    // Look by Currency 
                            //    Cgl.Read_SettlementDate_For_Category_BIN_TransType(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                            //                                                 Cgl.Card_BIN, Cgl.TransType, InRMCycle);
                            //}

                        }
                        else
                        {

                            // Based on TransType too. 
                            Cgl.Read_SettlementDate_For_Category_BIN_TransType(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                          Cgl.Card_BIN, Cgl.TransType, InRMCycle);
                            if (Cgl.RecordFound == false)
                            {
                                //MessageBox.Show("Not Record Found For .. Categ:.."+ Environment.NewLine
                                //    + "Card Bin.." + Cgl.Card_BIN + Environment.NewLine
                                //    + "Trans Type.." + Cgl.TransType + Environment.NewLine
                                //    );
                                // If not found we must insert Record
                                Cgl.Settlement_DATE = (DateTime)DataTableFileTotals.Rows[I]["SET_DATE"];

                                Cgl.MatchingCateg = (string)DataTableFileTotals.Rows[I]["MatchingCateg"];
                                Cgl.Card_BIN = (string)DataTableFileTotals.Rows[I]["Card_BIN"];
                                Cgl.TransType = (int)DataTableFileTotals.Rows[I]["TransType"];
                                Cgl.Ccy = (string)DataTableFileTotals.Rows[I]["Ccy"];
                                Cgl.CB_TXNs = 0;
                                //Cgl.CB_TXNs_AMT = (decimal)DataTableFileTotals.Rows[I]["TotAmount"];
                                Cgl.CB_TXNs_AMT = 0;

                                Cgl.EQUIV = (decimal)DataTableFileTotals.Rows[I]["EQUIV"];

                                Cgl.CB_UnMatched_TXNS_1xx = 0;
                                Cgl.CB_UnMatched_TXNS_1xx_Amt = 0;

                                //Cgl.Other_Unmatched_x11 = (int)DataTableFileTotals.Rows[I]["Other_Unmatched_x11"];
                                //Cgl.Other_Unmatched_x11_Amt = (decimal)DataTableFileTotals.Rows[I]["Other_Unmatched_x11_Amt"];
                                Cgl.Other_Unmatched_x11 = 0;
                                Cgl.Other_Unmatched_x11_Amt = 0;

                                Cgl.RMCycle = InRMCycle;
                                Cgl.FileId = InFileId;
                                Cgl.CategoryGroup = InCategories;
                                Cgl.W_Identity = InIdentity;
                                Cgl.Comment = "";
                                Cgl.Operator = InOperator;

                                Cgl.Insert_Settlement_DATE_For_Non_ATMS(InOperator, InRMCycle);

                                Cgl.Read_SettlementDate_For_Category_BIN_TransType(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                          Cgl.Card_BIN, Cgl.TransType, InRMCycle);
                                if (Cgl.RecordFound == true)
                                {
                                    // OK
                                }
                            }
                        }

                        if (WMask == "1")
                        {
                            // Update the first for CB
                            int WCB_UnMatched_TXNS_1xx = (int)DataTableFileTotals.Rows[I]["NoOfTxns"];
                            decimal WCB_UnMatched_TXNS_1xx_Amt = (decimal)DataTableFileTotals.Rows[I]["TotAmount"];
                            decimal EQUIV = (decimal)DataTableFileTotals.Rows[I]["EQUIV"];
                            Cgl.CB_UnMatched_TXNS_1xx = Cgl.CB_UnMatched_TXNS_1xx + WCB_UnMatched_TXNS_1xx;
                            Cgl.CB_UnMatched_TXNS_1xx_Amt = Cgl.CB_UnMatched_TXNS_1xx_Amt + WCB_UnMatched_TXNS_1xx_Amt;

                            // Update here 
                            if (Cgl.MatchingCateg == "BDC235")
                            {

                                // UPDATE by Currency 
                                Cgl.Update_Settlement_DATE_Category_Ccy(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                                                      Cgl.Ccy, InRMCycle);

                            }
                            else
                            {

                                Cgl.Update_Settlement_DATE_BIN_TransType(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                                                             Cgl.Card_BIN, Cgl.TransType, InRMCycle);

                            }
                        }
                        else
                        {
                            int WOther_Unmatched_x11 = (int)DataTableFileTotals.Rows[I]["NoOfTxns"];
                            decimal WOther_Unmatched_x11_Amt = (decimal)DataTableFileTotals.Rows[I]["TotAmount"];
                            decimal EQUIV = (decimal)DataTableFileTotals.Rows[I]["EQUIV"];
                            Cgl.Other_Unmatched_x11 = Cgl.Other_Unmatched_x11 + WOther_Unmatched_x11;
                            Cgl.Other_Unmatched_x11_Amt = Cgl.Other_Unmatched_x11_Amt + WOther_Unmatched_x11_Amt;

                            // Update here 
                            if (Cgl.MatchingCateg == "BDC235")
                            {
                                // UPDATE by Currency
                                Cgl.Update_Settlement_DATE_Category_Ccy(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                                                          Cgl.Ccy, InRMCycle);
                            }
                            else
                            {
                                Cgl.Update_Settlement_DATE_BIN_TransType(Cgl.Settlement_DATE, Cgl.MatchingCateg,
                                                                              Cgl.Card_BIN, Cgl.TransType, InRMCycle);
                            }
                        }
                    }

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

        }

        //
        // UPDATE CAP_DATE TOTALS BY READING IST  FOR ATMs
        // 
        public void CreateRecords_CAP_DATE_For_ATMs(string InOperator, DateTime InCAP_DATE, int InRMCycle, int InMode)
        {

            return; // Maybe we need it at later stage 

            RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime CAP_DATE = Net_TransDate;

            bool FirstRecord = true;
            string OldTerminalId = "";
            DateTime OldCAP_DATE = NullPastDate;
            string WOriginFile = "Switch_IST_Txns";

            decimal TurnOverDebit = 0;
            decimal TurnOverCredit = 0;
            int TransDR = 0;
            int TransCR = 0;

            int ReversalsTxns = 0;
            decimal ReversalsAmt = 0;

            decimal UnMatchedDebit = 0;
            decimal UnMatchedCredit = 0;
            int UnMatchedTransDR = 0;
            int UnMatchedTransCR = 0;

            //if (InMode == 1)
            //{
            //    // GL Affecting Categories
            //    SqlString = "SELECT  "
            //         + " MatchingCateg, TerminalId, TransType, TransAmt, TransDate "
            //         + " , ResponseCode, Processed, ProcessedAtRMCycle, Mask, Comment, CAP_DATE "
            //         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
            //         + " WHERE Processed = 1 AND ResponseCode = '0' AND ProcessedAtRMCycle = @ProcessedAtRMCycle "
            //         + " ORDER BY MatchingCateg, TransDate  ";
            //}
            if (InMode == 2)
            {
                // ONLY ATMS GL 
                // READ ALL LOADED IST RECORDS AND UPDATE GL TABLE BY CAP_DATE
                SqlString = "SELECT  "
                     + " MatchingCateg, TerminalId, TransType, TransAmt, TransDate "
                     + " , ResponseCode, Processed, ProcessedAtRMCycle, Mask, Comment, CAP_DATE "
                     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                     + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle  AND ResponseCode = '0' AND TXNSRC = '1' "
                     + " ORDER BY TerminalId, CAP_DATE, TransDate ";
            }

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);
                        // cmd.Parameters.AddWithValue("@CAP_DATE", InCAP_DATE);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            MatchingCateg = (string)rdr["MatchingCateg"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransAmt = (decimal)rdr["TransAmt"];

                            TransDate = (DateTime)rdr["TransDate"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            Processed = (bool)rdr["Processed"];
                            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
                            Mask = (string)rdr["Mask"];

                            Comment = (string)rdr["Comment"];
                            CAP_DATE = (DateTime)rdr["CAP_DATE"];

                            // Check for 
                            if (FirstRecord == true)
                            {
                                // Do What you have to do
                                OldTerminalId = TerminalId;
                                OldCAP_DATE = CAP_DATE;
                                FirstRecord = false;

                                // Look if Record exist

                                Cgl.Read_CAP_DATEFor_ATM_CapDate(TerminalId, OldCAP_DATE);
                                if (Cgl.RecordFound == true)
                                {

                                }
                                else
                                {
                                    // Then Insert
                                    int Mode = 2;
                                    Cgl.CAP_DATE = CAP_DATE;
                                    Cgl.MatchingCateg = "";
                                    Cgl.AtmNo = TerminalId;
                                    Cgl.CreatedAtRMCycle = InRMCycle;
                                    Cgl.FirstDtTm = TransDate;
                                    Cgl.GL_Number = "Not available";
                                    Cgl.DateCreated = DateTime.Now;
                                    Cgl.Operator = InOperator;

                                    Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                    // Initialise

                                }

                            }


                            if (CAP_DATE != OldCAP_DATE & TerminalId == OldTerminalId)
                            {
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_ATM_CapDate(TerminalId, OldCAP_DATE);

                                if (Cgl.RecordFound == true)
                                {
                                    // FOUND 
                                    // UPDATE
                                    Cgl.LastDtTm = TransDate;
                                    Cgl.UpdatedAtRMCycle = InRMCycle;

                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                    // Then Update
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, "", Cgl.AtmNo, Cgl.CreatedAtRMCycle, 2);

                                    // Then Insert for new ATM DATE
                                    int Mode = 2;
                                    Cgl.CAP_DATE = CAP_DATE; // New Date 
                                    Cgl.MatchingCateg = "";
                                    Cgl.AtmNo = TerminalId;
                                    Cgl.CreatedAtRMCycle = InRMCycle;
                                    Cgl.FirstDtTm = TransDate;
                                    Cgl.GL_Number = "Not available";
                                    Cgl.DateCreated = DateTime.Now;
                                    Cgl.Operator = InOperator;

                                    Cgl.Insert_CAP_DATE(Mode, WOriginFile);
                                }
                                else
                                {
                                    MessageBox.Show("WHY HERE?");

                                }

                                // Initialise

                                TurnOverDebit = 0;
                                TurnOverCredit = 0;
                                TransDR = 0;
                                TransCR = 0;
                                ReversalsTxns = 0;
                                ReversalsAmt = 0;
                                UnMatchedDebit = 0;
                                UnMatchedCredit = 0;
                                UnMatchedTransDR = 0;
                                UnMatchedTransCR = 0;


                            }

                            if (TerminalId != OldTerminalId)
                            {
                                // Update 
                                // Look if Record exist
                                Cgl.Read_CAP_DATEFor_ATM_CapDate(OldTerminalId, OldCAP_DATE);

                                if (Cgl.RecordFound == true)

                                {
                                    // UPDATE WITH ADD
                                    Cgl.LastDtTm = TransDate;
                                    Cgl.UpdatedAtRMCycle = InRMCycle;
                                    Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                    Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                    Cgl.TransDR = Cgl.TransDR + TransDR;
                                    Cgl.TransCR = Cgl.TransCR + TransCR;

                                    Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                    Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                    Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                    Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                    Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                    Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                    // Then Update OLD 
                                    Cgl.Update_CAP_DATE(Cgl.CAP_DATE, "", Cgl.AtmNo, Cgl.CreatedAtRMCycle, 2);

                                    // Then Insert NEW ATM NO if doesn't  exist

                                    Cgl.Read_CAP_DATEFor_ATM_CapDate(TerminalId, CAP_DATE);
                                    if (Cgl.RecordFound == true)
                                    {

                                    }
                                    else
                                    {
                                        // Then Insert
                                        int Mode = 2;
                                        Cgl.CAP_DATE = CAP_DATE;
                                        Cgl.MatchingCateg = "";
                                        Cgl.AtmNo = TerminalId;
                                        Cgl.CreatedAtRMCycle = InRMCycle;
                                        Cgl.FirstDtTm = TransDate;
                                        Cgl.GL_Number = "Not available";
                                        Cgl.DateCreated = DateTime.Now;
                                        Cgl.Operator = InOperator;

                                        Cgl.Insert_CAP_DATE(Mode, WOriginFile);

                                        // Initialise

                                    }


                                }
                                else
                                {
                                    MessageBox.Show("WHY HERE?...2");


                                }


                                // Initialise

                                TurnOverDebit = 0;
                                TurnOverCredit = 0;
                                TransDR = 0;
                                TransCR = 0;
                                ReversalsTxns = 0;
                                ReversalsAmt = 0;
                                UnMatchedDebit = 0;
                                UnMatchedCredit = 0;
                                UnMatchedTransDR = 0;
                                UnMatchedTransCR = 0;

                            }

                            // WORK ON Counters
                            if (TransType == 11 & Comment != "Reversals")
                            {
                                TurnOverDebit = TurnOverDebit + TransAmt;
                                TransDR = TransDR + 1;
                            }

                            if (TransType == 23 & Comment != "Reversals")
                            {
                                TurnOverCredit = TurnOverCredit + TransAmt;
                                TransCR = TransCR + 1;
                            }


                            if (Mask.Contains("0") == true || Mask.Contains("R") == true)
                            {
                                // Then Issue
                                if (TransType == 11 & Comment != "Reversals")
                                {
                                    UnMatchedDebit = UnMatchedDebit + TransAmt;
                                    UnMatchedTransDR = UnMatchedTransDR + 1;
                                }

                                if (TransType == 23 & Comment != "Reversals")
                                {
                                    UnMatchedCredit = UnMatchedCredit + TransAmt;
                                    UnMatchedTransCR = UnMatchedTransCR + 1;
                                }
                            }

                            // REVERSALS
                            if (TransType == 11 & Comment == "Reversals")
                            {
                                ReversalsTxns = ReversalsTxns + 1;
                                ReversalsAmt = ReversalsAmt + TransAmt;
                            }
                            // Intialised Old
                            OldTerminalId = TerminalId;
                            OldCAP_DATE = CAP_DATE;

                        }

                        // Close Reader
                        rdr.Close();

                        // Close conn
                        conn.Close();

                        // LAST RECORD
                        if (RecordFound == true)
                        {
                            Cgl.Read_CAP_DATEFor_ATM_CapDate(TerminalId, CAP_DATE);

                            if (Cgl.RecordFound == true)

                            {
                                Cgl.LastDtTm = TransDate;
                                Cgl.UpdatedAtRMCycle = InRMCycle;
                                Cgl.TurnOverDebit = Cgl.TurnOverDebit + TurnOverDebit;
                                Cgl.TurnOverCredit = Cgl.TurnOverCredit + TurnOverCredit;

                                Cgl.TransDR = Cgl.TransDR + TransDR;
                                Cgl.TransCR = Cgl.TransCR + TransCR;

                                Cgl.ReversalsTxns = Cgl.ReversalsTxns + ReversalsTxns;
                                Cgl.ReversalsAmt = Cgl.ReversalsAmt + ReversalsAmt;

                                Cgl.UnMatchedDebit = Cgl.UnMatchedDebit + UnMatchedDebit;
                                Cgl.UnMatchedCredit = Cgl.UnMatchedCredit + UnMatchedCredit;
                                Cgl.UnMatchedTransDR = Cgl.UnMatchedTransDR + UnMatchedTransDR;
                                Cgl.UnMatchedTransCR = Cgl.UnMatchedTransCR + UnMatchedTransCR;

                                // Then Update
                                Cgl.Update_CAP_DATE(Cgl.CAP_DATE, "", Cgl.AtmNo, Cgl.CreatedAtRMCycle, 2);
                            }
                            else
                            {
                                //
                                MessageBox.Show("WHY HERE?... 3");

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }
        // UPDATE Source Tables 
        // 
        //public void UpdateSourceTablesAsProcessed(string InMatchingCateg, string InFileId,
        //                             int InRRNumber, int InRMCycle, string InMask)
        public void UpdateSourceTablesAsProcessed_ATMs_V01(string InMatchingCateg, string InFileId,
                                      int InTraceNo, string InTerminalId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //int rows; 

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle "
                                   + " WHERE MatchingCateg = @MatchingCateg "
                                   + " AND TraceNo <= @TraceNo"
                                   + " AND TerminalId = @TerminalId AND Processed = 0 ", conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);

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
        //public void Update record by SeqNo
        //
        public void UpdateTablesRecordAsMatchedBySeqNo(string InFileId, int InSeqNo, string InMask, string InComment,
                                                                         int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [IsSettled] = 1, "
                                   + " [Mask] = @Mask, "
                                   + " [Comment] = @Comment, "
                                   + " ProcessedAtRmCycle = @ProcessedAtRmCycle "
                                   + " WHERE SeqNo = @SeqNo "
                                   + " ", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Mask", InMask);
                        cmd.Parameters.AddWithValue("@Comment", InComment);
                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);

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
        // UPDATE Source Tables 
        // 
        //public void UpdateSourceTablesAsProcessed(string InMatchingCateg, string InFileId,
        //                             int InRRNumber, int InRMCycle, string InMask)
        public void UpdateSourceTablesAsProcessed_ATMs_V02(string InMatchingCateg, string InFileId,
                                      DateTime InMinMaxDt, string InTerminalId, int InRMCycle)
        {
            // (WMatchingCateg, TableId, MinMaxDt_02, WAtmNo, WRMCycle);
            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle "
                                   + " WHERE "
                                   + " [Processed] = 0   "
                                   + " AND [MatchingCateg] = @MatchingCateg "
                                   + " AND [TerminalId] = @TerminalId "
                                   //+ " AND ([ResponseCode] = '0' OR [ResponseCode] = '112' ) "
                                   + " AND [ResponseCode] <> '8' " // Change 30/09/2022
                                   + " AND [TransDate] <= @TransDate ", conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);

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
        // UPDATE Full Table
        // 

        public void UpdateFullTableAsProcessed(string InMatchingCateg, string InFileId,
                                                                           int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " Processed = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle "
                                   + " WHERE MatchingCateg = @MatchingCateg "
                                    + " AND Processed = 0 ", conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@ProcessedAtRmCycle", InRMCycle);

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



        /* ==========================================================================================
            Methods and fields for Reconciliation File Monitor
           ========================================================================================== */

        #region Reconciliation File Monitor

        #region Create UNIV InMem DataTable
        public static DataTable CreateUNIV_MemTable()
        {
            DataTable InMemDataTbl = new DataTable();
            try
            {
                InMemDataTbl.Columns.Add("SeqNo", typeof(int));
                InMemDataTbl.Columns.Add("OriginFileName", typeof(string));
                InMemDataTbl.Columns.Add("OriginalRecordId", typeof(int));
                InMemDataTbl.Columns.Add("MatchingCateg", typeof(string));
                InMemDataTbl.Columns.Add("Origin", typeof(string));
                InMemDataTbl.Columns.Add("TerminalType", typeof(string));
                InMemDataTbl.Columns.Add("TransTypeAtOrigin", typeof(string));
                InMemDataTbl.Columns.Add("TerminalId", typeof(string));
                InMemDataTbl.Columns.Add("TransType", typeof(int));
                InMemDataTbl.Columns.Add("TransDescr", typeof(string));
                InMemDataTbl.Columns.Add("CardNumber", typeof(string));
                InMemDataTbl.Columns.Add("AccNo", typeof(string));
                InMemDataTbl.Columns.Add("TransCurr", typeof(string));
                InMemDataTbl.Columns.Add("TransAmt", typeof(decimal));
                InMemDataTbl.Columns.Add("AmtFileBToFileC", typeof(decimal));
                InMemDataTbl.Columns.Add("TransDate", typeof(DateTime));
                InMemDataTbl.Columns.Add("TraceNo", typeof(int));
                InMemDataTbl.Columns.Add("RRNumber", typeof(string));
                InMemDataTbl.Columns.Add("ResponseCode", typeof(string));
                InMemDataTbl.Columns.Add("ActionType", typeof(string));
                InMemDataTbl.Columns.Add("Processed", typeof(bool));
                InMemDataTbl.Columns.Add("ProcessedAtRMCycle", typeof(int));
                InMemDataTbl.Columns.Add("Mask", typeof(string));
                InMemDataTbl.Columns.Add("IsSettled", typeof(bool));
                InMemDataTbl.Columns.Add("UniqueRecordId", typeof(int));
                InMemDataTbl.Columns.Add("Operator", typeof(string));
            }
            catch (Exception ex)
            {
                // CatchDetails(ex);
                return (null);
            }
            return InMemDataTbl;
        }
        #endregion

        // Used only to initialize db for testing
        public void TruncateTable(string TableName)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdTruncate = "TRUNCATE TABLE [dbo].[" + TableName + "] ";
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdTruncate, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = string.Format("An error occured while TRUNCATING table {0}\nThe error message is:\n{1}", TableName, ex.Message);
                    CatchDetails(ex);
                }
        }

        // Used only to initialize db for testing

        public DataTable TempTableMpa = new DataTable();

        // public DataTable MatchingMasterDataTableATMs = new DataTable();


        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        // INSERT ALL RECORDS IN MASTER POOL - New Method
        public void ReadUniversalAndCreateMasterPoolRecords_NEW(string InOperator, string InTableName
                                                                                  , string InMatchingCateg, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            DateTime MaxDt01;
            DateTime MaxDt01_Minus;

            MaxDt01 = ReadAndFindMaxDt_NO_ATM(InTableName, InMatchingCateg);

            RRDMGasParameters Gp = new RRDMGasParameters();
            //
            // Check if you must get less transactions 
            // Through Parameter 820 
            //
            string ParId;
            //string OccurId;

            int MinusMin = 0;
            ParId = "820";
            //  OccurId = "1"; // 
            Gp.ReadParametersSpecificNm(InOperator, ParId, InMatchingCateg);

            MinusMin = (int)Gp.Amount;

            if (MinusMin > 0)
            {
                MaxDt01_Minus = MaxDt01.AddMinutes(-MinusMin); // LESS MinusMin value
            }
            else
            {
                MaxDt01_Minus = MaxDt01;
            }


            // 
            // Define Target Table
            // 
            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

            // Define the the Source table
            // Read In file and create the needed table 

            //int MaxSeqNo = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //
            // Get some needed info
            //
            Mc.ReadMatchingCategoryBySelectionCriteria(InMatchingCateg, 0, 12);

            Mcs.ReadReconcCategoriesVsSourcesAll(InMatchingCateg);

            int Counter;


            // LOAD Mpa

            string SQLCmd = "INSERT INTO "
                         + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
            + "( "
            + " [OriginFileName] "
            + ",[OriginalRecordId] "

            + ",[MatchingCateg] "
            + ",[RMCateg] "

            + ",[Origin] "
            + ",[TerminalType] "

            + ",[TransTypeAtOrigin] "
            + ",[TerminalId] "
            + ",[TransType] "
            + ",[TransDescr] "
            + ",[CardNumber] "

            + ",[AccNumber] "
            + ",[TransCurr] "
            + ",[TransAmount] "
            
          //  + ",[SpareField] " // AMOUNT_EQUIV
            + ",[TransDate] "

            + ",[TraceNoWithNoEndZero] " 
            + ",[AtmTraceNo] "
            + ",[MasterTraceNo] "

            + ",[RRNumber] "
            + ",[AUTHNUM] "
            + ",[ResponseCode] "

            + ",[UniqueRecordId] "
            + ",[Operator] "
            + ",[LoadedAtRMCycle]"
            // New values

            + ", [SeqNo01] "
            + ", [FileId01] "
            + ", [FileId02] "
            + ", [FileId03] "
            + ", [FileId04] "
            + ", [Card_Encrypted] "
            + ", [TXNSRC] "
            + ", [TXNDEST] "
            + ", [ACCEPTOR_ID] "
            + ", [ACCEPTORNAME] "
            + ", [CAP_DATE] "
              + ", [SET_DATE] "
                + ", [Comments] "

        + ") "
            //
            + " SELECT  "
            + " [OriginFileName] "
            + " ,[SeqNo] "
            + ",[MatchingCateg] "
            + ",[MatchingCateg] " // Leave as it is is for RM Category
            + ",[Origin] "
            + ",[TerminalType] "

            + ",[TransTypeAtOrigin] "
            + ",[TerminalId] "
            + ",[TransType] "
            + ",[TransDescr] "
            + ",[CardNumber] "

            + ",[AccNo] "
            + ",[TransCurr] "
            + ",[TransAmt] "
            //  + ",[AmtFileBToFileC] " // AMOUNT_EQUIV
            + ",[TransDate] "

            + ",[TraceNo] "
            + ",[TraceNo]*10 " // Times 10
            + ",[TraceNo]*10 " // Times 10

            + ",[RRNumber] "
             + ",[AUTHNUM] "
            + ",[ResponseCode] "

            + ", NEXT VALUE FOR [RRDM_Reconciliation_ITMX].dbo.ReconcSequence "
            + ",[Operator] "
            + ",@LoadedAtRMCycle" // Parmeter 
                                  // Additional
            + ",[SeqNo] "     // first SegNumber   
            + ",@FileId01"
            + ",@FileId02"
            + ",@FileId03"
            + ",@FileId04"
            + ",[Card_Encrypted] "
            + ", [TXNSRC] "
            + ", [TXNDEST] "
            + ", [ACCEPTOR_ID] "
            + ", [ACCEPTORNAME] "
            + ", [CAP_DATE] "
            + ", [SET_DATE] "
            + ", [Comment] "
           + " FROM " + InTableName
           + " WHERE Operator = @Operator AND MatchingCateg = @MatchingCateg AND Processed = 0 "
         //  + " AND TransDate <=@TransDate "
           ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycle);

                        cmd.Parameters.AddWithValue("@TransDate ", MaxDt01_Minus);

                        cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                        cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                        cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                        cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();

                    //System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                    //             + "..:.." + Counter.ToString());

                }
                catch (Exception ex)
                {
                    conn.Close();

                    MessageBox.Show("Cancel While Loading At Matching Category:" + InMatchingCateg);

                    CatchDetails(ex);
                }
            //UPDATE ORIGINAL RECORDS
            UpdatetSourceTxnsProcessedToTrueUpToMaxSeqNo(InTableName, InMatchingCateg, InRMCycle, MaxDt01_Minus);

            //MessageBox.Show("Loading for File " + InTableName + "And Matching Category " + InMatchingCateg ); 

        }

        //
        // UPDATE Processed = true in Source Table  
        // 
        public void UpdatetSourceTxnsProcessedToTrueUpToMaxSeqNo(string InTableName, string InMatchingCateg
                                       , int InRMCycle, DateTime InMaxDt01_Minus)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InTableName
                              + " SET Processed = 1 , ProcessedAtRMCycle = @ProcessedAtRMCycle "
                      //        + " WHERE Processed = 0  AND MatchingCateg = @MatchingCateg AND TransDate <=@TransDate ", conn))
                       + " WHERE Processed = 0  AND MatchingCateg = @MatchingCateg  ", conn))
                    {
                        //   cmd.Parameters.AddWithValue("@MaxSeq", InMaxSeqNo);
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InMaxDt01_Minus);
                        cmd.CommandTimeout = 350;  // seconds
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
        // UPDATE Processed = true in Source Table  
        // 
        public void UpdatetSourceTxnsProcessedToTrue_FOREX(string InTableName, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableName;

            int rows;

            using (SqlConnection conn =
                new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE " + PhysicalName
                              + " SET Processed = 1 , ProcessedAtRMCycle = @RMCycleNo "
                             + " WHERE Processed = 0 and LoadedAtRMCycle = @RMCycleNo ", conn))
                    {
                        //   cmd.Parameters.AddWithValue("@MaxSeq", InMaxSeqNo);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);
                        //cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

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

        //public void BulkInsertFromDataTable(DataTable DataTbl, string TWInitialTableName)
        //{
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
        //            {
        //                //SqlBulkCopyColumnMapping SeqNo = new SqlBulkCopyColumnMapping("SeqNo", "SeqNo");
        //                //bulkCopy.ColumnMappings.Add(SeqNo);
        //                //SqlBulkCopyColumnMapping Origin = new SqlBulkCopyColumnMapping("Origin", "Origin");
        //                //bulkCopy.ColumnMappings.Add(Origin);
        //                //SqlBulkCopyColumnMapping OriginFileName = new SqlBulkCopyColumnMapping("OriginFileName", "OriginFileName");
        //                //bulkCopy.ColumnMappings.Add(OriginFileName);
        //                //SqlBulkCopyColumnMapping OriginalRecordId = new SqlBulkCopyColumnMapping("OriginalRecordId", "OriginalRecordId");
        //                //bulkCopy.ColumnMappings.Add(OriginalRecordId);
        //                //SqlBulkCopyColumnMapping MatchingCateg = new SqlBulkCopyColumnMapping("MatchingCateg", "MatchingCateg");
        //                //bulkCopy.ColumnMappings.Add(MatchingCateg);
        //                //SqlBulkCopyColumnMapping TransTypeAtOrigin = new SqlBulkCopyColumnMapping("TransTypeAtOrigin", "TransTypeAtOrigin");
        //                //bulkCopy.ColumnMappings.Add(TransTypeAtOrigin);
        //                //SqlBulkCopyColumnMapping TerminalId = new SqlBulkCopyColumnMapping("TerminalId", "TerminalId");
        //                //bulkCopy.ColumnMappings.Add(TerminalId);
        //                //SqlBulkCopyColumnMapping TransType = new SqlBulkCopyColumnMapping("TransType", "TransType");
        //                //bulkCopy.ColumnMappings.Add(TransType);
        //                //SqlBulkCopyColumnMapping TransDescr = new SqlBulkCopyColumnMapping("TransDescr", "TransDescr");
        //                //bulkCopy.ColumnMappings.Add(TransDescr);
        //                //SqlBulkCopyColumnMapping CardNumber = new SqlBulkCopyColumnMapping("CardNumber", "CardNumber");
        //                //bulkCopy.ColumnMappings.Add(CardNumber);
        //                //SqlBulkCopyColumnMapping AccNo = new SqlBulkCopyColumnMapping("AccNo", "AccNo");
        //                //bulkCopy.ColumnMappings.Add(AccNo);
        //                //SqlBulkCopyColumnMapping TransCurr = new SqlBulkCopyColumnMapping("TransCurr", "TransCurr");
        //                //bulkCopy.ColumnMappings.Add(TransCurr);
        //                //SqlBulkCopyColumnMapping TransAmt = new SqlBulkCopyColumnMapping("TransAmt", "TransAmt");
        //                //bulkCopy.ColumnMappings.Add(TransAmt);
        //                //SqlBulkCopyColumnMapping AmtFileBToFileC = new SqlBulkCopyColumnMapping("AmtFileBToFileC", "AmtFileBToFileC");
        //                //bulkCopy.ColumnMappings.Add(AmtFileBToFileC);
        //                //SqlBulkCopyColumnMapping TransDate = new SqlBulkCopyColumnMapping("TransDate", "TransDate");
        //                //bulkCopy.ColumnMappings.Add(TransDate);
        //                //SqlBulkCopyColumnMapping TraceNo = new SqlBulkCopyColumnMapping("TraceNo", "TraceNo");
        //                //bulkCopy.ColumnMappings.Add(TraceNo);
        //                //SqlBulkCopyColumnMapping RRNumber = new SqlBulkCopyColumnMapping("RRNumber", "RRNumber");
        //                //bulkCopy.ColumnMappings.Add(RRNumber);
        //                //SqlBulkCopyColumnMapping ResponseCode = new SqlBulkCopyColumnMapping("", "ResponseCode");
        //                //bulkCopy.ColumnMappings.Add(ResponseCode);
        //                //SqlBulkCopyColumnMapping Twin = new SqlBulkCopyColumnMapping("", "Twin");
        //                //bulkCopy.ColumnMappings.Add(Twin);
        //                //SqlBulkCopyColumnMapping Processed = new SqlBulkCopyColumnMapping("Processed", "Processed");
        //                //bulkCopy.ColumnMappings.Add(Processed);
        //                //SqlBulkCopyColumnMapping ProcessedAtRMCycle = new SqlBulkCopyColumnMapping("ProcessedAtRMCycle", "ProcessedAtRMCycle");
        //                //bulkCopy.ColumnMappings.Add(ProcessedAtRMCycle);
        //                //SqlBulkCopyColumnMapping Operator = new SqlBulkCopyColumnMapping("Operator", "Operator");
        //                //bulkCopy.ColumnMappings.Add(Operator);

        //                try
        //                {
        //                    bulkCopy.DestinationTableName = TWInitialTableName;
        //                    bulkCopy.WriteToServer(DataTbl);
        //                    ErrorFound = false;
        //                }
        //                catch (Exception ex)
        //                {
        //                    conn.Close();
        //                    string msg = ex.Message;
        //                    Exception ex1 = ex.InnerException;
        //                    while (ex1 != null)
        //                    {
        //                        msg += "\r\n" + ex1.Message;
        //                        ex1 = ex1.InnerException;
        //                    }
        //                    ErrorFound = true;
        //                    ErrorOutput = msg;
        //                }
        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            // CatchDetails(ex);
        //            conn.Close();
        //            string msg = ex.Message;
        //            Exception ex1 = ex.InnerException;
        //            while (ex1 != null)
        //            {
        //                msg += "\r\n" + ex1.Message;
        //                ex1 = ex1.InnerException;
        //            }
        //            ErrorFound = true;
        //            ErrorOutput = msg;
        //        }
        //    }
        //}
        // 
        //public void DeleteRecordsByOriginFile(string FUID, string tblName)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("DELETE FROM [dbo].[" + tblName + "] WHERE OriginFileName =  @Org ", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@Org", FUID);
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
        // 
        public void DeleteRecordByOriginFileAndSeqNo(string InTable, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + InTable
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
        // Delete Adjustment 
        public void DeleteAdjustmentRecordByUniqueNo(string InTable, int InUniqueRecordId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + InTable
                        + " WHERE UniqueRecordId =  @UniqueRecordId AND TransTypeAtOrigin = 'WAdjustment' ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueRecordId", InUniqueRecordId);
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
        #endregion


        // Copy Records to Twin

        //public void CopyRecordsToTwin(string InTableA, string InTableB, string InCondition)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string SQLCmd = "INSERT INTO "
        //                     + InTableB + " T2"
        //                     + " SELECT * FROM "
        //                     + InTableA + " T1 "
        //                     + " WHERE T1.ID = @ID";

        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SQLCmd, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@ID", InCondition);
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


    }
}



using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingTxns_WorkingTables : Logger
    {
        public RRDMMatchingTxns_WorkingTables() : base() { }

        public int SeqNo;
        public string MatchingCateg;
        public int RMCycle;
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
        public string AUTHNUM;
       
        public DateTime FullDtTm;
        public string ResponseCode;


        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 


        //************************************************
        //

        public DataTable MatchingMasterDataTableATMs = new DataTable();

        public DataTable RMDataTableRight = new DataTable();

        public DataTable DataTableAtmsTotals = new DataTable();


        public int TotalSelected;
        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            MatchingCateg = (string)rdr["MatchingCateg"];
            RMCycle = (int)rdr["RMCycle"];
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
            AUTHNUM = (string)rdr["AUTHNUM"];

            FullDtTm = (DateTime)rdr["FullDtTm"];
            ResponseCode = (string)rdr["ResponseCode"];

    }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Number 
        //
        string WorkingTableName;
        public void ReadTransSpecificFromSpecificWorking(string InSelectionCriteria, string InWorkingFile)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (InWorkingFile == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingFile == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingFile == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";

            SqlString = "SELECT *"
              + " FROM " + WorkingTableName
              + InSelectionCriteria;
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
        // UPDATE Working Files 
        // 
        public void UpdateWorkingFilesAsProcessed(string InMatchingCateg, string InFileId,
                                      int InTraceNo, string InTerminalId, int InRMCycle)
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
        // Insert Working File General Structure 
        //
        // WITH TERMINAL ID 
        // 
        public void PopulateWorkingFile_ATMs_V01_XXXXX(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, int InMinMaxTrace)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";
         
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ", CAST(TransDate AS Date)"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) AND ([TerminalId] = @TerminalId) AND ([TraceNo] <= @TraceNo) AND([Processed] = 0)) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InMinMaxTrace);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

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
        // Insert Working File 01 General Structure 
        // FROM MASTER POOL
        // Working File 01 FROM Mpa
        // 
        public int PopulateWorkingFile_ATMs_Working01(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            // WorkingTableName = InWorkingTableName; 

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE(" 
+ "     ([IsMatchingDone] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ " AND (TerminalId = @TerminalId)"
+ " AND (ResponseCode = '0')"
+ " AND (TransDate <= @TransDate) "
+ ") "
+ ") "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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

        //
        // Insert Working File 01 General Structure 
        // FROM MASTER POOL
        // Working File 01 FROM Mpa
        // 
        public int PopulateWorkingFile_ATMs_Working01_ROM(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            // WorkingTableName = InWorkingTableName; 

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ",[UTRNNO]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ ",[UTRNNO]"
+ "FROM " + InOriginTable
+ " WHERE("
+ "     ([IsMatchingDone] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ " AND (TerminalId = @TerminalId)"
+ " AND (ResponseCode = '0')"
+ " AND (TransDate <= @TransDate) "
+ ") "
+ ") "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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

        public int PopulateWorkingFile_ATMs_Working01_WALLET(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string W_Application)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            // WorkingTableName = InWorkingTableName; 
            if (InWorkingTable == "01") WorkingTableName = "["+ W_Application + "].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[" + W_Application + "].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[" + W_Application + "].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[" + W_Application + "].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TransType]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",[TransDate]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[ResponseCode]"
+ ",[LoadedAtRMCycle]"
+ ",[IsMatchingDone]"
+ ",[SET_DATE]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ " , [MatchingCateg]"
+ ", @RMCycle "
+ ",[TransType]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",[TransDate]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[ResponseCode]"
+ ",[LoadedAtRMCycle]"
+ ",[IsMatchingDone]"
+ ",[SET_DATE]"
//SET_DATE
+ "FROM " + InOriginTable
+ " WHERE("
+ "     ([IsMatchingDone] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ " AND (ResponseCode = '0')"
+ " AND IsReversal = 0 "
+ ") "
+ ") "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                      
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        cmd.CommandTimeout = 500; 
                        
                        //rows number of record got updated

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
        // Insert Working Files 01, 02  General Structure 
        //
        // WITH TERMINAL ID 
        // 

        // Working File 01 FROM Mpa
        // 
        public int PopulateWorkingFile_ATMs_Working01_MULTI(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            WorkingTableName = InWorkingTable; 

            //if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            //if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            //if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable + " WITH (NOLOCK)"
+ " WHERE("
+ "     ([IsMatchingDone] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ " AND (TerminalId = @TerminalId)"
+ " AND (ResponseCode = '0')"
+ " AND (TransDate <= @TransDate) "
+ ") "
+ ") "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        // Insert Working Files 01, 02  General Structure 
        //
        // WITH TERMINAL ID 
        public int PopulateWorkingFile_ATMs_V02(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            //WorkingTableName = InWorkingTable;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate "
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE("
+ " ([Processed] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ "    AND ([TerminalId] = @TerminalId) "
+ " AND ((ResponseCode = '0')  OR (ResponseCode <>'0' AND Comment = 'Reversals'))"
+ " AND ([TransDate] <= @TransDate) "
//+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                    +")"
                    + ") "
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

                        Count  = cmd.ExecuteNonQuery();

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
        // Insert Working Files 01, 02  General Structure 
        //
        // WITH TERMINAL ID 
        public int PopulateWorkingFile_ATMs_V02_ROM(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            //WorkingTableName = InWorkingTable;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ",[UTRNNO]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate "
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ ",[UTRNNO]"
+ "FROM " + InOriginTable
+ " WHERE("
+ " ([Processed] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ "    AND ([TerminalId] = @TerminalId) "
+ " AND ((ResponseCode = '0')  OR (ResponseCode <>'0' AND Comment = 'Reversals'))"
+ " AND ([TransDate] <= @TransDate) "
                    //+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                    + ")"
                    + ") "
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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



        // WITH TERMINAL ID 
        public int PopulateWorkingFile_ATMs_V02_MULTI(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            WorkingTableName = InWorkingTable;

            //if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            //if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            //if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate "
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable + " WITH (NOLOCK)"
+ " WHERE("
+ " ([Processed] = 0) "
+ " AND ([MatchingCateg] = @MatchingCateg) "
+ "    AND ([TerminalId] = @TerminalId) "
+ " AND ((ResponseCode = '0')  OR (ResponseCode <>'0' AND Comment = 'Reversals'))"
+ " AND ([TransDate] <= @TransDate) "
                    //+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                    + ")"
                    + ") "
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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

        // WITH TERMINAL ID for Flexcube
        // 
        public int PopulateWorkingFile_ATMs_V03(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            // WorkingTableName = InWorkingTable;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate "
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ "    AND ([TerminalId] = @TerminalId) "
+ " AND ([TransDate] <= @TransDate) "
+ " AND([Processed] = 0) " 
+ " AND ResponseCode = '0' "
//+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

                        Count =  cmd.ExecuteNonQuery();

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
        public int PopulateWorkingFile_ATMs_V03_MULTI(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, string InAtmNo, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            WorkingTableName = InWorkingTable;

            //if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            //if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            //if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", CAST(TransDate AS Date)"
+ ", TransDate "
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable + " WITH (NOLOCK)"
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ "    AND ([TerminalId] = @TerminalId) "
+ " AND ([TransDate] <= @TransDate) "
+ " AND([Processed] = 0) "
+ " AND ResponseCode = '0' "
                    //+ " AND CAST(left([FullTraceNo], 1) as int)  = 2  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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

        // Count Active Records In File 
        // 
        public int CountActiveRecordsInThree(string InOriginTable, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            SqlString = " SELECT count(*) As Count "
               + " FROM " + InOriginTable
               + " WHERE MatchingCateg = @MatchingCateg AND Processed = 0 ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                       // cmd.Parameters.AddWithValue("@TerminalId", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Count = (int)rdr["Count"];
                            
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
            return Count; 
        }

        //
        // Insert Working Files 01, 02,03  General Structure 
        //
        // WITHOUT  TERMINAL ID 
        // 
        public void PopulateWorkingFile_V02_NotATMs(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle,  DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count; 

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", TransDate "
+ ", CAST(TransDate AS Date)"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ " AND ([TransDate] <= @TransDate) "
+ " AND([Processed] = 0) "
+ " AND ResponseCode = '0'  "
+ "  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                      
                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        // Insert Working Files 01, 02,03  General Structure 
        //
        // WITHOUT  TERMINAL ID 
        // 
        public void PopulateWorkingFile_V02_NotATMs_On_SET_DATE(string InWorkingTable, string InOriginTable
            , string InMatchingCateg, int InRMCycle, DateTime InSET_DATE)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
//+ ", TransDate "
+ ", CAST(TransDate AS Date)"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ " AND ([SET_DATE] <= @SET_DATE) "
+ " AND([Processed] = 0) "
+ " AND ResponseCode = '0' AND Comment <> 'Reversals' "
+ "  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@SET_DATE", InSET_DATE);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        public void PopulateWorkingFile_V02_NotATMs_On_SET_DATE_MOBILE(string InWorkingTable, string InOriginTable
           , string InMatchingCateg, int InRMCycle, DateTime InSET_DATE)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TransactionId]"
+ ",[Type]"
+ ",[Senderscheme]"
+ ",[Sendernumber]"
+ ",[Receivernumber]"
+ ",[TransDate]"
+ ",[Net_TransDate]"
+ ",[TransCurr]"
+ ",[TransAmt]"

+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",@RMCycle"
+ ",[TransactionId]"
+ ",[Type]"
+ ",[Senderscheme]"
+ ",[Sendernumber]"
+ ",[Receivernumber]"
+ ",[TransDate]"
+ ",[Net_TransDate]"
+ ",[TransCurr]"
+ ",[TransAmt]"

+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ " AND ([SET_DATE] <= @SET_DATE) "
+ " AND([Processed] = 0) "
+ " AND ResponseCode = '0'  "
+ "  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@SET_DATE", InSET_DATE);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                      //  cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }
        // WITHOUT  TERMINAL ID 
        // 
        public void PopulateWorkingFile_ATMs_V02_NO_ATM_But_POS(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";
            

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ",[Card_Encrypted]"
+ ") "
+ "("
+ " SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
+ ", CAST(TransDate AS Date)"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ ",[Card_Encrypted]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
//+ " AND ([TransDate] <= @TransDate) "
+ " AND([Processed] = 0) "
+ " AND (ResponseCode = '0' OR ResponseCode = '200000') "
+ "  "
                    + ")"
                    + "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

  //                      cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        //
        // Insert Working File 01 General Structure 
        // FROM MASTER POOL
        // Working File 01 FROM Mpa

        //   NO TERMINAL ID 
        // 
        public void PopulateWorkingFile_Working01_NOT_ATMS(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0; 

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
//+ ", TransDate "
+ ", CAST(TransDate AS Date)"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
+ " AND ([TransDate] <= @TransDate) "
+ " AND (ResponseCode = '0')"
+ "AND([IsMatchingDone] = 0) ) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        //   NO TERMINAL ID 
        // 
        public void PopulateWorkingFile_Working01_NOT_ATMS_No_Date(string InWorkingTable, string InOriginTable,
                                                        string InMatchingCateg, int InRMCycle, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
//+ ", TransDate "
+ ", CAST(TransDate AS Date)"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
//+ " AND ([TransDate] <= @TransDate) "
+ " AND (ResponseCode = '0')"
+ "AND([IsMatchingDone] = 0) ) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        // 
        public void PopulateWorkingFile_Working01_NOT_ATMS_No_Date_MOBILE(string InWorkingTable, string InOriginTable,
                                                        string InMatchingCateg, int InRMCycle, DateTime InMinMaxDt)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Count = 0;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
 
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TransactionId]"
+ ",[Type]"
+ ",[Senderscheme]"
+ ",[Sendernumber]"
+ ",[Receivernumber]"
+ ",[TransDate]"
+ ",[Net_TransDate]"
+ ",[TransCurr]"
+ ",[TransAmt]"
//+ ",[AUTHNUM]"
//+ ",[RRNumber]"
//+ ",[AUTHNUM]"
+ ",[ResponseCode]"
+ ") "
+ "(SELECT "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",@RMCycle"
+ ",[TransactionId]"
+ ",[Type]"
+ ",[Senderscheme]"
+ ",[Sendernumber]"
+ ",[Receivernumber]"
+ ",[TransDate]"
+ ",[Net_TransDate]"
+ ",[TransCurr]"
+ ",[TransAmt]"
//+ ",[AUTHNUM]"
//+ ",[RRNumber]"
//+ ",[AUTHNUM]"
+ ",[ResponseCode]" 
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
//+ " AND ([TransDate] <= @TransDate) "
+ " AND (ResponseCode = '0')"
+ "AND([IsMatchingDone] = 0) ) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                      //  cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                      //  cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }
        // 
        public int Count ;
        //
        public void PopulateWorkingFile_ATMs_Working01_NOT_ATMs_But_POS(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            Count = 0;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
+ "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[FullDtTm]"
+ ",[ResponseCode]"
+ ",[Card_Encrypted]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNumber]"
+ ",[TransCurr]"
+ ",[TransAmount]"
+ ",@AmtBtoC"
+ ", CAST(TransDate AS Date)"
+ ",[TraceNoWithNoEndZero]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ",[TransDate]"
+ ",[ResponseCode]"
+ ",[Card_Encrypted]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) "
//+ " AND ([TransDate] <= @TransDate) "
+ " AND (ResponseCode = '0' OR ResponseCode = '200000' )"
+ "AND([IsMatchingDone] = 0) ) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                     //   cmd.Parameters.AddWithValue("@TransDate", InMinMaxDt);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

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
        }

        //
        // Insert Working File 02, 03 for NOT ATMS 
        // 
        // NO NEED OF TERMINAL ID 
        // 
        public void PopulateWorkingFile_NOT_ATMs(string InWorkingTable, string InOriginTable, string InMatchingCateg, int InRMCycle, int InRRNumber)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int Rows;

            if (InWorkingTable == "01") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            if (InWorkingTable == "02") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            if (InWorkingTable == "03") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            if (InWorkingTable == "04") WorkingTableName = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_04]";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(
                            " SET IDENTITY_INSERT " + WorkingTableName + " ON "
                      + "INSERT INTO " + WorkingTableName
                      + "( "
+ "[SeqNo]"
+ ",[MatchingCateg]"
+ ",[RMCycle]"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",[AmtFileBToFileC]"
+ ",[TransDate]"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ ") "
+ "(SELECT "
+ " [SeqNo]"
+ ",[MatchingCateg]"
+ ", @RMCycle"
+ ",[TerminalId]"
+ ",[TransType]"
+ ",[TransDescr]"
+ ",[CardNumber]"
+ ",[AccNo]"
+ ",[TransCurr]"
+ ",[TransAmt]"
+ ",@AmtBtoC"
+ ", CAST(TransDate AS Date)"
+ ",[TraceNo]"
+ ",[RRNumber]"
+ ",[AUTHNUM]"
+ "FROM " + InOriginTable
+ " WHERE([MatchingCateg] = @MatchingCateg) AND ([RRNumber] <= @RRNumber) AND([Processed] = 0)) "
+ "SET IDENTITY_INSERT " + WorkingTableName + " OFF "
                            , conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);

                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AmtBtoC", 0);

                        //rows number of record got updated

                        Rows = cmd.ExecuteNonQuery();

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
        // DELETE Records in Working File  
        //
        public void TruncateInWorkingFile(string InTableId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int rows;
            // Truncate
            string SQLCmd = "TRUNCATE TABLE " + InTableId;

            using (SqlConnection conn = new SqlConnection(connectionString))
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
       
    }
}

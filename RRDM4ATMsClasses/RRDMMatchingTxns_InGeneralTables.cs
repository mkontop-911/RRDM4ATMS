using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMMatchingTxns_InGeneralTables : Logger
    {
        public RRDMMatchingTxns_InGeneralTables() : base() { }

        public int SeqNo;
        public string OriginFileName;
        public int OriginalRecordId;
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

        public string ResponseCode;

        public bool Processed;
        public int ProcessedAtRMCycle;
        public string Mask;

        public bool ItHasException;
        public int UniqueRecordId;
        public string Operator;

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

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
        string recconConnString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            OriginFileName = (string)rdr["OriginFileName"];

            OriginalRecordId = (int)rdr["OriginalRecordId"];

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

            ResponseCode = (string)rdr["ResponseCode"];
          

            Processed = (bool)rdr["Processed"];
            ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
            Mask = (string)rdr["Mask"];

            ItHasException = (bool)rdr["ItHasException"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];
            Operator = (string)rdr["Operator"];

        }

        public int Count_11;
        public int Count_22;
        //
        // READ SPECIFIC TRANSACTION FROM 
        //
        //  
        public void ReadTransSpecificFromSpecificWorking(string InSelectionCriteria, string InTableName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Count_11 = 0;
            Count_22 = 0;

            SqlString = "SELECT *"
                  + " FROM " + InTableName
                  + InSelectionCriteria
                  + " ORDER BY SeqNo DESC ";

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

                            if (TransType == 11)
                            {
                                Count_11 = Count_11 + 1;
                            }

                            if (TransType == 22)
                            {
                                Count_22 = Count_22 + 1;
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
        // Find totals for a Universal Table
        //
        public int TotalMatched;
        public decimal TotalAmountMatched;

        public int TotalUnMatched;
        public decimal TotalAmountUnMatched;

        public void ReadTransUniversalTableAndFindTotals(string InSelectionCriteria, string InTableName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalMatched = 0;
            TotalAmountMatched = 0;

            TotalUnMatched = 0;
            TotalAmountUnMatched = 0;

            SqlString = "SELECT *"
              + " FROM " + InTableName
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

                            if (Processed == true & ItHasException == false)
                            {
                                // Matched
                                TotalMatched = TotalMatched + 1;
                                TotalAmountMatched = TotalAmountMatched + TransAmt;

                            }
                            if (Processed == true & ItHasException == true)
                            {
                                // UnMatched
                                TotalUnMatched = TotalUnMatched + 1;
                                TotalAmountUnMatched = TotalAmountUnMatched + TransAmt;
                            }

                            //Processed = (bool)rdr["Processed"];
                            //ProcessedAtRMCycle = (int)rdr["ProcessedAtRMCycle"];
                            //Mask = (string)rdr["Mask"];

                            //ItHasException = (bool)rdr["ItHasException"];
                            //UniqueRecordId = (int)rdr["UniqueRecordId"];


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
                         new SqlConnection(connectionString))
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

            int TraceNo;

            SqlString =
            " SELECT  Count(*) As NumberOfRecords, MAX (TransDate) As MaxDt "
            + " FROM " + InFile
            + " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0 "
                           + "AND TerminalId = @TerminalId  ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            int NoRec = (int)rdr["NumberOfRecords"];

                            if (NoRec == 0)
                            {
                                // return; 
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
        public int ReadAndFindTraceNoFromMaxDateTimeForMpa(string InFile, string InMatchingCateg, string InTerminalId, DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MxTraceNo = 0;

            SqlString =
          " SELECT  TraceNoWithNoEndZero "
          + " FROM " + InFile
          + " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0 "
                         + "AND TerminalId = @TerminalId AND TransDate = @TransDate ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            MxTraceNo = (int)rdr["TraceNoWithNoEndZero"];

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
            return MxTraceNo;
        }

        // Find corresponding date for trace  
        public DateTime ReadAndFindDateTimeForTraceMpa(string InTable, string InMatchingCateg, string InTerminalId, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  TransDate  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0 AND TerminalId = @TerminalId "
                           + " AND TraceNoWithNoEndZero = @TraceNoWithNoEndZero ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNoWithNoEndZero", InTraceNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TransDate = (DateTime)rdr["TransDate"];

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

        DateTime MaxDt;
        // Find Max Trace for other files than Mpa 
        public DateTime ReadAndFindMaxDt(string InTable, string InMatchingCateg, string InTerminalId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT  Count(*) As NumberOfRecords, MAX([TransDate]) As MaxDt "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId  ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            int NoRec = (int)rdr["NumberOfRecords"];

                            if (NoRec == 0)
                            {
                                MaxDt = NullPastDate;
                                // return; 
                            }
                            else
                            {
                                MaxDt = (DateTime)rdr["MaxDt"];
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

        // Find Max Trace for other files than Mpa 
        public int ReadAndFindTraceForMaxDt(string InTable, string InMatchingCateg, string InTerminalId, DateTime InTransDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MxTraceNo = 0;

            SqlString =
               " SELECT  TraceNo "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId "
                           + "  AND TransDate = @TransDate ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            MxTraceNo = (int)rdr["TraceNo"];

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
            return MxTraceNo;
        }

        // Find corresponding date for trace  
        public DateTime ReadAndFindDateTimeForTrace(string InTable, string InMatchingCateg, string InTerminalId, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString =
               " SELECT  TransDate  "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 AND TerminalId = @TerminalId "
                           + " AND TraceNo = @TraceNo ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TransDate = (DateTime)rdr["TransDate"];

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

        // Make Processed the ones not 
        public void ReadRecordsWithoutATMAndSetThemAsProcesses(string InTable, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            SqlString =
            " SELECT  * "
            + " FROM " + InTable
            + " WHERE Processed = 0  "
            + " Order By TerminalId ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            SeqNo = (int)rdr["SeqNo"];
                            TerminalId = (string)rdr["TerminalId"];

                            Ac.ReadAtm(TerminalId);

                            if (Ac.RecordFound == true)
                            {
                                // There is ATM
                            }
                            else
                            {
                                // THERE IS NO ATM

                                UpdateSourceTablesAsProcessedBySeqNo(SeqNo, InTable, InRMCycle);
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
        public int ReadAndFindMaxSeqNo(string InTable, string InMatchingCateg, string InTerminalId)
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
                         new SqlConnection(connectionString))
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

        // Find Max Trace for Mpa with ... NO ... TerminalId (ATM)
        public int ReadAndFindMaxTraceNoForMpa_NO_ATM(string InTable, string InPrimaryMatchingFieldNm, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int MaxValue = 0; 

            string MaxInstruction
                     = " ISNULL(MAX([" + InPrimaryMatchingFieldNm + "]), 0) As PrimaryMatchingField ";
            SqlString =
        " SELECT " + MaxInstruction
        + " FROM " + InTable
        + " WHERE MatchingCateg = @MatchingCateg AND [IsMatchingDone] = 0  ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            MaxValue = (int)rdr["PrimaryMatchingField"];

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
            return MaxValue;
        }

        int MaxPrimaryMatchingField;
        // Find Max PrimaryMatchingField for input Tables  
        public int ReadAndFindMaxPrimaryMatchingField(string InTable, string InPrimaryMatchingFieldNm, string InMatchingCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string MaxInstruction
                     = " ISNULL(MAX([" + InPrimaryMatchingFieldNm + "]), 0) As PrimaryMatchingField ";

            SqlString =
            " SELECT " + MaxInstruction
            //" SELECT  ISNULL(MAX([TraceNo]), 0) As MaxTrace "
            + " FROM " + InTable
            + " WHERE MatchingCateg = @MatchingCateg AND [Processed] = 0 ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
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

                            MaxPrimaryMatchingField = (int)rdr["PrimaryMatchingField"];

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
            return MaxPrimaryMatchingField;
        }
        //
        // UPDATE SOURCE Tables as Processed FOR NON ATMs 
        // 
        public void UpdateSourceTablesAsProcessed_NonAtms(string InMatchingCateg, string InFileId,
                                      string InRRNumber, int InRMCycle, string InMask)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle, Mask = @Mask "
                                   + " WHERE MatchingCateg = @MatchingCateg "
                                   + " AND RRNumber <= @RRNumber"
                                   + " AND Processed = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRNumber);
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
        // UPDATE SOURCE Tables as Processed FOR NON ATMs 
        // 
        public void UpdateSourceTablesAsProcessedBySeqNo(int InSeqNo, string InFileId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " [Processed] = 1, ProcessedAtRmCycle = @ProcessedAtRmCycle "
                                   + " WHERE SeqNo = @SeqNo "
                                   + "  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
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
        // UPDATE Source Tables 
        // 
        //public void UpdateSourceTablesAsProcessed(string InMatchingCateg, string InFileId,
        //                             int InRRNumber, int InRMCycle, string InMask)
        public void UpdateSourceTablesAsProcessed_ATMs_V02(string InMatchingCateg, string InFileId,
                                      DateTime InMinMaxDt, string InTerminalId, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

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
                                   + " AND TransDate <= @TransDate"
                                   + " AND TerminalId = @TerminalId AND Processed = 0 ", conn))
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
        // UPDATE Footer
        // 
        public void UpdateSourceTablesFooter(int InSeqNo, string InFileId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InFileId
                                   + " SET "
                                   + " Mask = @Mask, ItHasException = @ItHasException, UniqueRecordId = @UniqueRecordId "
                                   + " WHERE SeqNo = @SeqNo "
                                   + "  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Mask", Mask);
                        cmd.Parameters.AddWithValue("@ItHasException", ItHasException);
                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);

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
                InMemDataTbl.Columns.Add("FullTraceNo", typeof(string));
                InMemDataTbl.Columns.Add("ResponseCode", typeof(string));
                InMemDataTbl.Columns.Add("Twin", typeof(string));
                InMemDataTbl.Columns.Add("Processed", typeof(bool));
                InMemDataTbl.Columns.Add("ProcessedAtRMCycle", typeof(int));
                InMemDataTbl.Columns.Add("Mask", typeof(string));
                InMemDataTbl.Columns.Add("ItHasException", typeof(bool));
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

        public void ReadUniversalAndCreateMasterPoolRecords(string InOperator, string InTableName, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            // 
            // Define Target Table
            // 
            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

            TempTableMpa = new DataTable();
            TempTableMpa.Clear();

            TempTableMpa.Columns.Add("OriginFileName", typeof(string));
            TempTableMpa.Columns.Add("OriginalRecordId", typeof(int));
            TempTableMpa.Columns.Add("UniqueRecordId", typeof(int));

            TempTableMpa.Columns.Add("MatchingCateg", typeof(string));
            TempTableMpa.Columns.Add("FileId01", typeof(string));
            TempTableMpa.Columns.Add("FileId02", typeof(string));
            TempTableMpa.Columns.Add("FileId03", typeof(string));
            TempTableMpa.Columns.Add("FileId04", typeof(string));
            TempTableMpa.Columns.Add("FileId05", typeof(string));

            TempTableMpa.Columns.Add("RMCateg", typeof(string));

            TempTableMpa.Columns.Add("Origin", typeof(string));
            TempTableMpa.Columns.Add("TerminalType", typeof(string));

            TempTableMpa.Columns.Add("TransTypeAtOrigin", typeof(string));

            TempTableMpa.Columns.Add("Product", typeof(string));
            TempTableMpa.Columns.Add("CostCentre", typeof(string));
            TempTableMpa.Columns.Add("TargetSystem", typeof(int));
            TempTableMpa.Columns.Add("MatchingAtRMCycle", typeof(int));
            TempTableMpa.Columns.Add("TerminalId", typeof(string));
            TempTableMpa.Columns.Add("TransType", typeof(int));
            TempTableMpa.Columns.Add("DepCount", typeof(int));

            TempTableMpa.Columns.Add("TransDescr", typeof(string));
            TempTableMpa.Columns.Add("CardNumber", typeof(string));
            TempTableMpa.Columns.Add("AccNumber", typeof(string));
            TempTableMpa.Columns.Add("TransCurr", typeof(string));

            TempTableMpa.Columns.Add("TransAmount", typeof(decimal));
            TempTableMpa.Columns.Add("TransDate", typeof(DateTime));
            TempTableMpa.Columns.Add("TraceNoWithNoEndZero", typeof(int));
            TempTableMpa.Columns.Add("AtmTraceNo", typeof(int));

            TempTableMpa.Columns.Add("MasterTraceNo", typeof(int));
            TempTableMpa.Columns.Add("MetaExceptionNo", typeof(int));

            TempTableMpa.Columns.Add("MetaExceptionId", typeof(int));
            TempTableMpa.Columns.Add("RRNumber", typeof(string));

            TempTableMpa.Columns.Add("ResponseCode", typeof(string));
            TempTableMpa.Columns.Add("Operator", typeof(string));

            TempTableMpa.Columns.Add("ReplCycleNo", typeof(int));


            // Define the the Source table
            // Read In file and create the needed table 

            int MaxSeqNo = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT   MAX(SeqNo) As MaxSeqNo "
            + " FROM " + InTableName;

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            MaxSeqNo = (int)rdr["MaxSeqNo"];

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

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();

            SqlString = "SELECT * "
                  + " FROM " + InTableName
                    + " WHERE Operator = @Operator AND Processed = 0 AND SeqNo <= @SeqNo";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@SeqNo", MaxSeqNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(MatchingMasterDataTableATMs);

                        // Close conn
                        conn.Close();

                        int K = 0;

                        while (K <= (MatchingMasterDataTableATMs.Rows.Count - 1))
                        {
                            // GET Table fields - Line by Line
                            //
                            RecordFound = true;

                            DataRow RowSelected = TempTableMpa.NewRow();

                            RowSelected["OriginFileName"] = (string)MatchingMasterDataTableATMs.Rows[K]["OriginFileName"];
                            RowSelected["OriginalRecordId"] = (int)MatchingMasterDataTableATMs.Rows[K]["SeqNo"];
                            RowSelected["UniqueRecordId"] = Gu.GetNextValue();

                            RowSelected["MatchingCateg"] = MatchingCateg = (string)MatchingMasterDataTableATMs.Rows[K]["MatchingCateg"];

                            Mcs.ReadReconcCategoriesVsSourcesAll(MatchingCateg);

                            RowSelected["FileId01"] = Mcs.SourceFileNameA;
                            RowSelected["FileId02"] = Mcs.SourceFileNameB;
                            RowSelected["FileId03"] = Mcs.SourceFileNameC;
                            RowSelected["FileId04"] = Mcs.SourceFileNameD;
                            RowSelected["FileId05"] = Mcs.SourceFileNameE;

                            Mc.ReadMatchingCategoryBySelectionCriteria(MatchingCateg, 0, 12);

                            RowSelected["RMCateg"] = MatchingCateg;

                            RowSelected["Origin"] = (string)MatchingMasterDataTableATMs.Rows[K]["Origin"];
                            RowSelected["TerminalType"] = (string)MatchingMasterDataTableATMs.Rows[K]["TerminalType"];
                            RowSelected["TransTypeAtOrigin"] = (string)MatchingMasterDataTableATMs.Rows[K]["TransTypeAtOrigin"];
                            RowSelected["Product"] = Mc.Product;
                            RowSelected["CostCentre"] = Mc.CostCentre;
                            RowSelected["TargetSystem"] = Mc.TargetSystemId;

                            RowSelected["MatchingAtRMCycle"] = 0;
                            RowSelected["TerminalId"] = (string)MatchingMasterDataTableATMs.Rows[K]["TerminalId"];
                            RowSelected["TransType"] = (int)MatchingMasterDataTableATMs.Rows[K]["TransType"];
                            RowSelected["DepCount"] = 0;

                            RowSelected["TransDescr"] = (string)MatchingMasterDataTableATMs.Rows[K]["TransDescr"];
                            RowSelected["CardNumber"] = (string)MatchingMasterDataTableATMs.Rows[K]["CardNumber"];
                            RowSelected["AccNumber"] = AccNo = (string)MatchingMasterDataTableATMs.Rows[K]["AccNo"]; ;
                            RowSelected["TransCurr"] = (string)MatchingMasterDataTableATMs.Rows[K]["TransCurr"];

                            RowSelected["TransAmount"] = TransAmt = (decimal)MatchingMasterDataTableATMs.Rows[K]["TransAmt"];
                            RowSelected["TransDate"] = (DateTime)MatchingMasterDataTableATMs.Rows[K]["TransDate"];
                            RowSelected["TraceNoWithNoEndZero"] = TraceNo = (int)MatchingMasterDataTableATMs.Rows[K]["TraceNo"];
                            RowSelected["AtmTraceNo"] = TraceNo = (int)MatchingMasterDataTableATMs.Rows[K]["TraceNo"]; ;

                            RowSelected["MasterTraceNo"] = 0;

                            RowSelected["MetaExceptionNo"] = 0;
                            RowSelected["MetaExceptionId"] = 0;

                            RowSelected["RRNumber"] = (string)MatchingMasterDataTableATMs.Rows[K]["RRNumber"];
                            RowSelected["ResponseCode"] = ResponseCode = (string)MatchingMasterDataTableATMs.Rows[K]["ResponseCode"]; ;
                            RowSelected["Operator"] = InOperator;
                            RowSelected["ReplCycleNo"] = 0; // WSesNo

                            TempTableMpa.Rows.Add(RowSelected);

                            K++; // Read Next entry of the table 

                        }


                    }

                    //TestTableMpa
                    using (SqlConnection conn2 =
                                   new SqlConnection(connectionString))
                        try
                        {
                            conn2.Open();

                            using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                            {
                                s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]";

                                foreach (var column in TempTableMpa.Columns)
                                    s.ColumnMappings.Add(column.ToString(), column.ToString());

                                s.WriteToServer(TempTableMpa);
                            }
                            conn2.Close();
                        }
                        catch (Exception ex)
                        {
                            conn2.Close();

                            CatchDetails(ex);
                        }

                    UpdatetSourceTxnsProcessedToTrueUpToMaxSeqNo(InTableName, MaxSeqNo, InRMCycle);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        // INSERT ALL RECORDS IN MASTER POOL - New Method
        public void ReadUniversalAndCreateMasterPoolRecords_NEW(string InOperator, string InTableName, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            // 
            // Define Target Table
            // 
            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMMatchingCategories Mc = new RRDMMatchingCategories();
            RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

            // Define the the Source table
            // Read In file and create the needed table 

            int MaxSeqNo = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString =
            " SELECT   MAX(SeqNo) As MaxSeqNo "
            + " FROM " + InTableName
            +" WHERE Operator = @Operator AND Processed = 0 ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            MaxSeqNo = (int)rdr["MaxSeqNo"];

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

            MatchingMasterDataTableATMs = new DataTable();
            MatchingMasterDataTableATMs.Clear();

            SqlString = "SELECT * "
                  + " FROM " + InTableName
                    + " WHERE Operator = @Operator AND Processed = 0 AND SeqNo <= @SeqNo";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                  //
                  // Get some needed info
                  //
                    Mc.ReadMatchingCategoryBySelectionCriteria(MatchingCateg, 0, 12);

                    Mcs.ReadReconcCategoriesVsSourcesAll(MatchingCateg);

                    int Counter;

                    // LOAD Mpa

      string SQLCmd = "INSERT INTO "
                   + " [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
      + "( "
      + " [OriginFileName] "
      + ",[OriginalRecordId] "
      + ",[MatchingCateg] "
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
      + ",[AmtFileBToFileC] "
      + ",[TransDate] "

      + ",[AtmTraceNo] "
      + ",[RRNumber] "
      + ",[ResponseCode] "
      + ",[Twin] "
      + ",[Processed] "

      + ",[ProcessedAtRMCycle] "
      + ",[Mask] "
      + ",[ItHasException] "
      + ",[UniqueRecordId] "
      + ",[Operator] "
      // New values
      + ", [FileId01] "
      + ", [FileId02] "
      + ", [FileId03] "
      + ", [FileId04] "
      + ") "
      + " SELECT  "
      + " [OriginFileName] "
      + ",[OriginalRecordId] "
      + ",[MatchingCateg] "
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
      + ",[AmtFileBToFileC] "
      + ",[TransDate] "

      + ",[TraceNo] "
      + ",[RRNumber] "
      + ",[ResponseCode] "
   
      + ",[Processed] "

      + ",[ProcessedAtRMCycle] "
      + ",[Mask] "
      + ",[ItHasException] "
      //   + ",[UniqueRecordId] "
      + ",[SeqNo] " // Use this as unique Record Id
      + ",[Operator] "
      // Additional
      + ",@FileId01" 
      + ",@FileId02"
      + ",@FileId03"
      + ",@FileId04"
      //+ ",@Product"
      //+ ",@CostCentre"
      //+ ",@TargetSystem"
     + " FROM " + InTableName
     + " WHERE Operator = @Operator AND Processed = 0 AND SeqNo <= @SeqNo"; ; 
                
                    using (SqlConnection conn2 = new SqlConnection(recconConnString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SQLCmd, conn))
                            {
                                cmd.Parameters.AddWithValue("@Operator", InOperator);
                                cmd.Parameters.AddWithValue("@SeqNo", MaxSeqNo);
                                cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                                cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                                cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                                cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);
                                //cmd.Parameters.AddWithValue("@Product", Mc.Product);
                                //cmd.Parameters.AddWithValue("@CostCentre", Mc.CostCentre);
                                //cmd.Parameters.AddWithValue("@TargetSystem", Mc.TargetSystemId);
                           
                                Counter = cmd.ExecuteNonQuery();
                            }
                            // Close conn
                            conn.Close();

                            System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                                         + "..:.." + Counter.ToString());

                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            CatchDetails(ex);
                        }
//UPDATE ORIGINAL RECORDS
                    UpdatetSourceTxnsProcessedToTrueUpToMaxSeqNo(InTableName, MaxSeqNo, InRMCycle);
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
        public void UpdatetSourceTxnsProcessedToTrueUpToMaxSeqNo(string InTableName, int InMaxSeqNo, int InRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE " + InTableName
                              + " SET Processed = 1 , ProcessedAtRMCycle = @ProcessedAtRMCycle "
                             + " WHERE Processed = 0  AND SeqNo <= @MaxSeq ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaxSeq", InMaxSeqNo);
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycle);

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

   

        public void BulkInsertFromDataTable(DataTable DataTbl, string TWInitialTableName)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(recconConnString))
            {
                try
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        //SqlBulkCopyColumnMapping SeqNo = new SqlBulkCopyColumnMapping("SeqNo", "SeqNo");
                        //bulkCopy.ColumnMappings.Add(SeqNo);
                        //SqlBulkCopyColumnMapping Origin = new SqlBulkCopyColumnMapping("Origin", "Origin");
                        //bulkCopy.ColumnMappings.Add(Origin);
                        //SqlBulkCopyColumnMapping OriginFileName = new SqlBulkCopyColumnMapping("OriginFileName", "OriginFileName");
                        //bulkCopy.ColumnMappings.Add(OriginFileName);
                        //SqlBulkCopyColumnMapping OriginalRecordId = new SqlBulkCopyColumnMapping("OriginalRecordId", "OriginalRecordId");
                        //bulkCopy.ColumnMappings.Add(OriginalRecordId);
                        //SqlBulkCopyColumnMapping MatchingCateg = new SqlBulkCopyColumnMapping("MatchingCateg", "MatchingCateg");
                        //bulkCopy.ColumnMappings.Add(MatchingCateg);
                        //SqlBulkCopyColumnMapping TransTypeAtOrigin = new SqlBulkCopyColumnMapping("TransTypeAtOrigin", "TransTypeAtOrigin");
                        //bulkCopy.ColumnMappings.Add(TransTypeAtOrigin);
                        //SqlBulkCopyColumnMapping TerminalId = new SqlBulkCopyColumnMapping("TerminalId", "TerminalId");
                        //bulkCopy.ColumnMappings.Add(TerminalId);
                        //SqlBulkCopyColumnMapping TransType = new SqlBulkCopyColumnMapping("TransType", "TransType");
                        //bulkCopy.ColumnMappings.Add(TransType);
                        //SqlBulkCopyColumnMapping TransDescr = new SqlBulkCopyColumnMapping("TransDescr", "TransDescr");
                        //bulkCopy.ColumnMappings.Add(TransDescr);
                        //SqlBulkCopyColumnMapping CardNumber = new SqlBulkCopyColumnMapping("CardNumber", "CardNumber");
                        //bulkCopy.ColumnMappings.Add(CardNumber);
                        //SqlBulkCopyColumnMapping AccNo = new SqlBulkCopyColumnMapping("AccNo", "AccNo");
                        //bulkCopy.ColumnMappings.Add(AccNo);
                        //SqlBulkCopyColumnMapping TransCurr = new SqlBulkCopyColumnMapping("TransCurr", "TransCurr");
                        //bulkCopy.ColumnMappings.Add(TransCurr);
                        //SqlBulkCopyColumnMapping TransAmt = new SqlBulkCopyColumnMapping("TransAmt", "TransAmt");
                        //bulkCopy.ColumnMappings.Add(TransAmt);
                        //SqlBulkCopyColumnMapping AmtFileBToFileC = new SqlBulkCopyColumnMapping("AmtFileBToFileC", "AmtFileBToFileC");
                        //bulkCopy.ColumnMappings.Add(AmtFileBToFileC);
                        //SqlBulkCopyColumnMapping TransDate = new SqlBulkCopyColumnMapping("TransDate", "TransDate");
                        //bulkCopy.ColumnMappings.Add(TransDate);
                        //SqlBulkCopyColumnMapping TraceNo = new SqlBulkCopyColumnMapping("TraceNo", "TraceNo");
                        //bulkCopy.ColumnMappings.Add(TraceNo);
                        //SqlBulkCopyColumnMapping RRNumber = new SqlBulkCopyColumnMapping("RRNumber", "RRNumber");
                        //bulkCopy.ColumnMappings.Add(RRNumber);
                        //SqlBulkCopyColumnMapping ResponseCode = new SqlBulkCopyColumnMapping("", "ResponseCode");
                        //bulkCopy.ColumnMappings.Add(ResponseCode);
                      
                        //SqlBulkCopyColumnMapping Processed = new SqlBulkCopyColumnMapping("Processed", "Processed");
                        //bulkCopy.ColumnMappings.Add(Processed);
                        //SqlBulkCopyColumnMapping ProcessedAtRMCycle = new SqlBulkCopyColumnMapping("ProcessedAtRMCycle", "ProcessedAtRMCycle");
                        //bulkCopy.ColumnMappings.Add(ProcessedAtRMCycle);
                        //SqlBulkCopyColumnMapping Operator = new SqlBulkCopyColumnMapping("Operator", "Operator");
                        //bulkCopy.ColumnMappings.Add(Operator);

                        try
                        {
                            bulkCopy.DestinationTableName = TWInitialTableName;
                            bulkCopy.WriteToServer(DataTbl);
                            ErrorFound = false;
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            string msg = ex.Message;
                            Exception ex1 = ex.InnerException;
                            while (ex1 != null)
                            {
                                msg += "\r\n" + ex1.Message;
                                ex1 = ex1.InnerException;
                            }
                            ErrorFound = true;
                            ErrorOutput = msg;
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    // CatchDetails(ex);
                    conn.Close();
                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    ErrorFound = true;
                    ErrorOutput = msg;
                }
            }
        }

        public void DeleteRecordsByOriginFile(string FUID, string tblName)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[" + tblName + "] WHERE OriginFileName =  @Org ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Org", FUID);
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

        public void CopyRecordsToTwin(string InTableA, string InTableB, string InCondition)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string SQLCmd = "INSERT INTO "
                             + InTableB + " T2"
                             + " SELECT * FROM "
                             + InTableA + " T1 "
                             + " WHERE T1.ID = @ID";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", InCondition);
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

        // Bulk Insert

        //public void InsertRecordsInTableFromTextFile_InBulk(string InTableA, string InTableB, string InCondition, string InFullPath)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";
        //    string SQLCmd;

        //    //            create temporary table #tmpImportCustFiles 

        //    InFullPath = "C:\\_KONTO\\txn_log.txt";

        //    InTableA = "[RRDM_Reconciliation_ITMX].[dbo].[BDC_tmpImport_IST_Records]";

        //    InTableB = "[RRDM_Reconciliation_ITMX].[dbo].[BDC_IST_Table] ";

        //    //string SQLCmdCreateTable = " Create table #tmpImport_IST_Records  "
        //    //             + " ( "
        //    //             + " LOCAL_DATE nvarchar(50), "
        //    //             + " LOCAL_TIME nvarchar(50), "
        //    //             + " TXN_TYPE nvarchar(50), "
        //    //             + " AMOUNT nvarchar(50), "
        //    //             + " TRACE nvarchar(50), "
        //    //             + " TERMID nvarchar(50), "
        //    //             + " ATM_ID nvarchar(50), "
        //    //             + " MASK_PAN nvarchar(50), "
        //    //             + " ACCTNUM nvarchar(50) "
        //    //             + " ) "
        //    //             ;


        //    //using (SqlConnection conn = new SqlConnection(recconConnString))
        //    //    try
        //    //    {
        //    //        conn.Open();
        //    //        using (SqlCommand cmd =
        //    //            new SqlCommand(SQLCmdCreateTable, conn))
        //    //        {
        //    //           // cmd.Parameters.AddWithValue("@FullPath", InFullPath);

        //    //            cmd.ExecuteNonQuery();

        //    //            //SQLCmd = " BULK INSERT "
        //    //            //     + " #tmpImport_IST_Records "
        //    //            //     + " FROM '" + InFullPath.Trim() +"'"
        //    //            //     + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253' , "
        //    //            //     + " ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',',"
        //    //            //     + " ROWTERMINATOR = '\n' )"
        //    //            //     ;
        //    //            //cmd.CommandText = SQLCmd;
        //    //            //cmd.ExecuteNonQuery();
        //    //        }
        //    //        // Close conn
        //    //        //    conn.Close();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        conn.Close();

        //    //        CatchDetails(ex);
        //    //    }

        //    // Truncate Table

        //    SQLCmd = "TRUNCATE TABLE " + InTableA;

        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SQLCmd, conn))
        //            {
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

        //    // Bulk insert the txt file to this temporary table

        //    SQLCmd = " BULK INSERT "
        //                  + InTableA
        //                  + " FROM '" + InFullPath.Trim() + "'"
        //                  + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253' , "
        //                  + " ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',',"
        //                  + " ROWTERMINATOR = '\n' )"
        //                  ;

        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SQLCmd, conn))
        //            {
        //                //cmd.Parameters.AddWithValue("@FullPath", InFullPath);

        //                cmd.ExecuteNonQuery();
        //            }
        //            // Close conn
        //           conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //    // VALIDATION

        //    RecordFound = false;

        //    string LOCAL_DATE;
        //    int Invalid_LOCAL_DATE = 0;
        //    string LOCAL_TIME;
        //    int Invalid_LOCAL_TIME = 0;
        //    decimal AMOUNT;
        //    int Invalid_AMOUNT = 0;
        //    string TRACE;
        //    int Invalid_TRACE = 0;

        //    DateTime tempLOCAL_DATE;
        //    int tempLOCAL_TIME;
        //    decimal tempAMOUNT;
        //    int tempTRACE;

        //    SqlString = "SELECT "
        //        + "[LOCAL_DATE] "
        //        + ",[LOCAL_TIME] "
        //        + ",CAST([AMOUNT] As decimal(18, 2)) As AMOUNT"
        //        //     + ",[AMOUNT] "
        //        + ",[TRACE] "
        //          + " FROM " + InTableA ;
        //      //    + " Where TXN_TYPE = 'DEPOSIT' OR TXN_TYPE = 'WITHDRAWAL' ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    LOCAL_DATE = (string)rdr["LOCAL_DATE"];
        //                    LOCAL_TIME = (string)rdr["LOCAL_TIME"];
        //                    AMOUNT = (decimal)rdr["AMOUNT"];
        //                    TRACE = (string)rdr["TRACE"];

        //                    // Date Validation
        //                    //if (DateTime.TryParse(LOCAL_DATE, out tempLOCAL_DATE))
        //                    //{
        //                    //    // Yay :)
        //                    //}
        //                    //else
        //                    //{
        //                    //    // Aww.. :(
        //                    //    Invalid_LOCAL_DATE = Invalid_LOCAL_DATE + 1;
        //                    //}

        //                    // Time validation
        //                    if (int.TryParse(LOCAL_TIME, out tempLOCAL_TIME))
        //                    {
        //                        // Yay :)
        //                    }
        //                    else
        //                    {
        //                        // Aww.. :(
        //                        Invalid_LOCAL_TIME = Invalid_LOCAL_TIME + 1;
        //                    }

        //                    // AMOUNT VALIDATION
        //                    //if (decimal.TryParse(AMOUNT, out tempAMOUNT))
        //                    //{
        //                    //    // Yay :)
        //                    //}
        //                    //else
        //                    //{
        //                    //    // Aww.. :(
        //                    //    Invalid_AMOUNT = Invalid_AMOUNT + 1;
        //                    //}
        //                    // TRACE VALIDATION
        //                    if (int.TryParse(TRACE, out tempTRACE))
        //                    {
        //                        // Yay :)
        //                    }
        //                    else
        //                    {
        //                        // Aww.. :(
        //                        Invalid_TRACE = Invalid_TRACE + 1;
        //                    }

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

        //    if (Invalid_LOCAL_DATE > 0
        //        || Invalid_LOCAL_TIME > 0
        //        || Invalid_AMOUNT > 0
        //        || Invalid_TRACE > 0
        //        )
        //    {
        //        // System.Windows.Forms.MessageBox.Show("There is Error In BANK's DATA." };
        //        System.Windows.Forms.MessageBox.Show("NOT CORRECT BANK's DATA" + Environment.NewLine
        //                          + "For " + InFullPath + Environment.NewLine
        //                          + "Data will not be loaded " + Environment.NewLine
        //                         , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //    return;
        //    }


        //    //
        //    // Merge records of this temporary table with the production table 
        //    //

        //    int Counter; 

        //    SQLCmd = "INSERT INTO "
        //        + InTableB
        //        + "( "
        //         + " [LOCAL_DATE_TIME] "
        //         + ",[TXN_TYPE] "
        //         + ",[AMOUNT] "
        //         + ",[TRACE] "
        //         + ",[TERMID] "
        //         + ",[ATM_ID] "
        //         + ",[MASK_PAN] "
        //         + ",[ACTNUM] "
        //         + ") "
        //         + " SELECT  "
        //         + "CAST([LOCAL_DATE] as datetime) "
        //            + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
        //            + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
        //            + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
        //         + ",[TXN_TYPE] "
        //         + ",CAST([AMOUNT] As decimal(18,2)) "
        //         + ",CAST([TRACE] as int) "
        //         + ",[TERMID] "
        //         + ",[ATM_ID] "
        //         + ",[MASK_PAN] "
        //         + ",[ACTNUM] "
        //         + "FROM [RRDM_Reconciliation_ITMX].[dbo].[BDC_tmpImport_IST_Records] ";
        //     //    + " Where TXN_TYPE = 'DEPOSIT' OR TXN_TYPE = 'WITHDRAWAL' ";


        //    using (SqlConnection conn = new SqlConnection(recconConnString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SQLCmd, conn))
        //            {
        //                //   cmd.Parameters.AddWithValue("@ID", InCondition);
        //                Counter = cmd.ExecuteNonQuery();
        //            }
        //            // Close conn
        //            conn.Close();
        //            System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
        //                         + "..:.." + Counter.ToString());

        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //}

    }
}

using System;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class RRDMPerformanceTraceClass : Logger
    {
        public RRDMPerformanceTraceClass() : base() { }

        // For critical proceses a record is inputed to see performance 

        public int RecordNo;
        public int Mode;
        // 1 : Performance critical processes 
        // 2 : Traces of critical processes 
        // 5 : Operational Updating Actions
        // 6 : Operational Not Updating Actions
        public string BankId;
        public string ProcessNm;
        public string AtmNo;
        public DateTime Cut_Off_Date; 
        public DateTime StartDT;
        public DateTime EndDT;
        public int Duration_Sec;
        public int Duration_Mili;
        public string Details; 
        public string UserId;
        public int RMCycleNo;
        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable TablePerformance = new DataTable();

        public DataTable TableTraces = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Read Fields
        private void Read_ReaderFields(SqlDataReader rdr)
        {
            // Read Details
            RecordNo = (int)rdr["RecordNo"];

            Mode = (int)rdr["Mode"];

            BankId = (string)rdr["BankId"];

            ProcessNm = (string)rdr["ProcessNm"];
            AtmNo = (string)rdr["AtmNo"];

            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];
            StartDT = (DateTime)rdr["StartDT"];
            EndDT = (DateTime)rdr["EndDT"];

            Duration_Sec = (int)rdr["Duration_Sec"];
            Duration_Mili = (int)rdr["Duration_Mili"];
            
            Details = (string)rdr["Details"];

            UserId = (string)rdr["UserId"];
            RMCycleNo = (int)rdr["RMCycleNo"];
            
            Operator = (string)rdr["Operator"];
        }
       //
// Read and create Data Table for Performance 
//
        public void ReadPerformanceTraceAndFillTableForPerformance(string InOperator, string InAtmNo, string InProcessNm )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TablePerformance = new DataTable();
            TablePerformance.Clear();

            TotalSelected = 0;

            string SqlString2 =
                    " SELECT "
                    + " CAST(StartDT AS Date) As Entry_Date, "
                     + " Sum(Duration_Sec) As TotalDuration,"
                    + " Max(Duration_Sec) AS Max_Duration, Sum(Counter) As Counter, (Sum(Counter)/Sum(Duration_Sec)) As PerSec "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + " WHERE Mode = 1 AND AtmNo = @AtmNo AND ProcessNm LIKE @ProcessNm"
                    + " GROUP BY CAST(StartDT AS Date) "
                    + " ORDER BY CAST(StartDT AS Date) DESC ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ProcessNm", InProcessNm);

                    //Create a datatable that will be filled with the data retrieved from the command
                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TablePerformance);

                   
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        
        }

        public void ReadPerformanceTraceAndFillTableForPerformance_2(string InOperator, string InProcessNm
                                                          , DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TablePerformance = new DataTable();
            TablePerformance.Clear();

            TotalSelected = 0;


            string SqlString2 =
                    " SELECT RMCycleNo, Details , "
                      + " CAST( EndDT AS Date) As Entry_Date, "
                     + " CAST(Duration_Sec as Decimal(12, 2))/ 60 As Duration_Min "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + " WHERE Mode in (5, 6) AND ProcessNm = @ProcessNm AND Duration_Sec > 0 "
                    + " AND CAST( EndDT AS Date) BETWEEN @DateFrom AND @DateTo "
                    + " ORDER BY RMCycleNo  ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ProcessNm", InProcessNm);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo);

                    //Create a datatable that will be filled with the data retrieved from the command
              
                    sqlAdapt.Fill(TablePerformance);

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

        }
        // CREATE TABLE FOR OPERATIONAL ACTIONS
        public void ReadPerformanceTraceAndFillTableForOperatingActions(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTraces = new DataTable();
            TableTraces.Clear();

            TotalSelected = 0;

            string SqlString2 =
                    " SELECT RecordNo"
                    + " ,EndDT As DateTime "
                    + " ,Cut_Off_Date "
                    + " ,ProcessNm "
                    + " ,Details "
                    + " ,CAST(Duration_Sec as Decimal(12,2))/60 As Duration_Min "
                    + " ,Mode "
                    + " ,UserId "
                    + " ,RMCycleNo "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + InSelectionCriteria
                    + " ORDER BY RecordNo DESC";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                  //  sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                   // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableTraces);


                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

            InsertWReport73_2(); 

        }
        // Range of Dates 
        public void ReadPerformanceTraceAndFillTableForOperatingActions_2(string InOperator,DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTraces = new DataTable();
            TableTraces.Clear();

            TotalSelected = 0;


            string SqlString2 =
                    " SELECT RecordNo"
                    + " ,EndDT As DateTime "
                    + " ,Cut_Off_Date "
                    + " ,ProcessNm "
                    + " ,Details "
                    + " ,CAST(Duration_Sec as Decimal(12,2))/60 As Duration_Min "
                    + " ,Mode "
                    + " ,UserId "
                    + " ,RMCycleNo "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + " WHERE "
                         + "  Mode in (5,6) And Cast(EndDT as Date) >=@FromDt AND  "
                             + "  Cast(EndDT as Date) <= @ToDt  "
                    + " ORDER BY RecordNo ASC";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InFromDt);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDt", InToDt);

                    //    DataSet MISds = new DataSet();
                    sqlAdapt.Fill(TableTraces);


                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

            InsertWReport73_2();

        }

        // Read and create Data Table for Performance 
        //
        public void ReadPerformanceTraceAndFillTableForTraces(string InOperator, DateTime InCutOffDate, string InCriticalProcess)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableTraces = new DataTable();
            TableTraces.Clear();

            TotalSelected = 0;

            string SqlString2 =
                    " SELECT * "
                    + " FROM [ATMS].[dbo].[PerformanceTrace] "
                    + " WHERE Mode = 2 AND Cut_Off_Date = @CutOffDate  AND ProcessNm = @ProcessNm"
                    + " ORDER BY RecordNo ";

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@CutOffDate", InCutOffDate);
                    sqlAdapt.SelectCommand.Parameters.AddWithValue("@ProcessNm", InCriticalProcess);

                    //Create a datatable that will be filled with the data retrieved from the command
               
                    sqlAdapt.Fill(TableTraces);

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }

            return; 

            InsertWReport73();
        }

        // Insert 
        public void InsertWReport73()
        {

            if (TableTraces.Rows.Count > 0)
            {
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport73();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport73]";

                            foreach (var column in TableTraces.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(TableTraces);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }
        }

        // Insert 73_2
        public void InsertWReport73_2()
        {
            return;

            if (TableTraces.Rows.Count > 0)
            {
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport73_2();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport73_2]";

                            foreach (var column in TableTraces.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(TableTraces);
                        }

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }
        }

        // READ records for a particular ATM 

        public void ReadPerformanceTrace(string InBankId, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[PerformanceTrace] "
          + " WHERE BankId = @BankId and AtmNo = @AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Read_ReaderFields(rdr);

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


        // READ record for a particular Record No

        public void ReadPerformanceTraceRecNo(int InRecordNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[PerformanceTrace] "
          + " WHERE RecordNo = @RecordNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details
                            Read_ReaderFields(rdr);

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
        // READ to find Max Record Number conditional on Bank Id and AtmNo  

        public void ReadMaxRecordNo(string InBankId, string InAtmNo, string InProcessNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT RecordNo = MAX(RecordNo)"
                   + " FROM [dbo].[PerformanceTrace]"
                   + " WHERE BankId = @BankId AND AtmNo = @AtmNo AND ProcessNm LIKE @ProcessNm"; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        InProcessNm = "%" + InProcessNm + "%"; 
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ProcessNm", InProcessNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            RecordNo = (int)rdr["RecordNo"];
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
        // Insert NEW Performance Trace GENERAL WITHOUT USER 
        //
        public void InsertPerformanceTrace(string InBankId,string InOperator, int InMode , 
                    string InProcessNm, string InAtmNo, DateTime InStartDT, DateTime InEndDT, string InDetails)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            //if (InMode == 1)
            //{
                // Calculate Duration in seconds 
                if (InEndDT > InStartDT)
            {
                TimeSpan DurationTemp = InEndDT - InStartDT;

                Duration_Sec = Convert.ToInt32(DurationTemp.TotalSeconds);
                Duration_Mili = Convert.ToInt32(DurationTemp.Milliseconds);
            }
               else
            {
                 Duration_Sec=0;
                 Duration_Mili=0;
            }
               
            //}
            //else
            //{
            //    Duration_Sec = 0 ;
            //    Duration_Mili = 0 ;
            //}
           

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            Rjc.ReadLastReconcJobCycle(InOperator); 

            string cmdinsert = "INSERT INTO [dbo].[PerformanceTrace]"   
             + " ([Mode], [BankId], "
             + " [ProcessNm],[AtmNo] ,"
             + " [Cut_Off_Date],"
             + " [StartDT],"
             + " [EndDT], "
             + " [Duration_Sec], "
             + " [Duration_Mili], "
             + " [Details] ,[Operator]) "
             +  "  VALUES "
             + " (@Mode, @BankId,  "
             + " @ProcessNm, @AtmNo , "
             + " @Cut_Off_Date, "
             + " @StartDT, "
             + " @EndDT, "
             + " @Duration_Sec, "
             + " @Duration_Mili, "
             + " @Details, @Operator) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@Mode", InMode);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
              
                        cmd.Parameters.AddWithValue("@ProcessNm", InProcessNm);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Rjc.Cut_Off_Date);

                        cmd.Parameters.AddWithValue("@StartDT", InStartDT);
                        cmd.Parameters.AddWithValue("@EndDT", InEndDT);

                        cmd.Parameters.AddWithValue("@Duration_Sec", Duration_Sec);
                        cmd.Parameters.AddWithValue("@Duration_Mili", Duration_Mili);

                        cmd.Parameters.AddWithValue("@Details", InDetails);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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
        // Insert NEW Performance Trace GENERAL WITH USER 
        //
        public void InsertPerformanceTrace_With_USER(string InBankId, string InOperator, int InMode,
                    string InProcessNm, string InAtmNo, DateTime InStartDT, DateTime InEndDT, string InDetails,string InSignedId, int InRMCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InEndDT >= InStartDT)
            {
                // OK
            }
            else
            {
                return; 
            }

                //if (InMode == 1)
                //{
           // Calculate Duration in seconds 
            TimeSpan DurationTemp = InEndDT - InStartDT;

            Duration_Sec = Convert.ToInt32(DurationTemp.TotalSeconds);
            Duration_Mili = Convert.ToInt32(DurationTemp.Milliseconds);
            //}
            //else
            //{
            //    Duration_Sec = 0 ;
            //    Duration_Mili = 0 ;
            //}


            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            Rjc.ReadLastReconcJobCycle(InOperator);

            if (Rjc.RecordFound == true)
            {
            }
            else
            {
                Rjc.Cut_Off_Date = DateTime.Now;
            }

            string cmdinsert = "INSERT INTO [dbo].[PerformanceTrace]"
             + " ([Mode], [BankId], "
             + " [ProcessNm],[AtmNo] ,"
             + " [Cut_Off_Date],"
             + " [StartDT],"
             + " [EndDT], "
             + " [Duration_Sec], "
             + " [Duration_Mili], "
             + " [Details], "
              + " [UserId],"
             + " [RMCycleNo]," 
             + "[Operator]" +
             ") "
             + "  VALUES "
             + " (@Mode, @BankId,  "
             + " @ProcessNm, @AtmNo , "
             + " @Cut_Off_Date, "
             + " @StartDT, "
             + " @EndDT, "
             + " @Duration_Sec, "
             + " @Duration_Mili, "
             + " @Details," +
               " @UserId," +
             " @RMCycleNo," +
             " @Operator" +
             ") ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@Mode", InMode);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        cmd.Parameters.AddWithValue("@ProcessNm", InProcessNm);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Rjc.Cut_Off_Date);

                        cmd.Parameters.AddWithValue("@StartDT", InStartDT);
                        cmd.Parameters.AddWithValue("@EndDT", InEndDT);

                        cmd.Parameters.AddWithValue("@Duration_Sec", Duration_Sec);
                        cmd.Parameters.AddWithValue("@Duration_Mili", Duration_Mili);

                        cmd.Parameters.AddWithValue("@Details", InDetails);
                        

                        cmd.Parameters.AddWithValue("@UserId", InSignedId);

                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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


        // UPDATE Performance Trace upon completion 
        // 
        public void UpdatePerformanceTrace(int InRecordNo, int InCounter)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 
            // Calculate Duration in seconds 
            TimeSpan DurationTemp = EndDT - StartDT;
     
            Duration_Sec = Convert.ToInt32(DurationTemp.TotalSeconds);

            if (Duration_Sec == 0 )
            {
                using (SqlConnection conn =
               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE FROM [dbo].[PerformanceTrace] "
                                + " WHERE RecordNo = @RecordNo ", conn))
                        {
                            cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);

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
            else
            {
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE [dbo].[PerformanceTrace] SET "
                                + " EndDT = @EndDT, Duration_Sec = @Duration, Counter = @Counter  "
                                + " WHERE RecordNo = @RecordNo ", conn))
                        {
                            cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);

                            cmd.Parameters.AddWithValue("@EndDT", EndDT);
                            cmd.Parameters.AddWithValue("@Duration_Sec", Duration_Sec);
                            cmd.Parameters.AddWithValue("@Counter", InCounter);

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
         
        }

        // UPDATE Performance Trace upon completion 
        // 
        public void DeleteLessThanDatePerformanceTrace(DateTime InCut_Off_Date)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ATMS].[dbo].[PerformanceTrace] "
                            + " WHERE Cut_Off_Date < @Cut_Off_Date ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        //rows number of record got updated
                        cmd.CommandTimeout = 300; 

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

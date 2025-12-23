using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
// using System.IO;


namespace RRDM4ATMs
{
    public class RRDMReconcFileMonitorLog : Logger
    {
        public RRDMReconcFileMonitorLog() : base() { }

        public int SeqNo;
        public int RMCycleNo;
        public string SystemOfOrigin;
        public string SourceFileID;
        public string StatusVerbose;
        public string FileName;
        public int FileSize;
        public DateTime DateTimeReceived;
        public DateTime DateExpected;   // added 13/01/2018
        public string DateOfFile;       // added 13/01/2018, 'string' enables to denote invalid value
        public string FileHASH;
        public int LineCount;

        public int stpFuid;
        public int stpReturnCode;
        public string stpErrorText;
        public string stpReferenceCode;

        public string ArchivedPath;
        public string ExceptionPath;
        public int Status; // 1 loaded OK 
                           // 0 Problem with Loading 
        public DateTime MAX_DATE;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int ValidTotalForCycle;
        public int InValidTotalForCycle;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Define the data table 
        public DataTable DataTableFileMonitorLog = new DataTable();

        public DataTable DataTableAtmsWithGaps_1 = new DataTable();
        public DataTable DataTableAtmsWithGaps_2 = new DataTable();
        public DataTable DataTableAtmsWithGaps_3 = new DataTable();

        public DataTable DataTableLast_20_Journals = new DataTable();

        // Uses AgentConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
        // Read Fields 
        private void ReadFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            RMCycleNo = (int)rdr["RMCycleNo"];
            SystemOfOrigin = (string)rdr["SystemOfOrigin"];
            SourceFileID = (string)rdr["SourceFileID"];
            StatusVerbose = (string)rdr["StatusVerbose"];
            FileName = (string)rdr["FileName"];
            FileSize = (int)rdr["FileSize"];
            DateTimeReceived = (DateTime)rdr["DateTimeReceived"];
            DateExpected = (DateTime)rdr["DateExpected"];
            DateOfFile = (string)rdr["DateOfFile"];
            FileHASH = (string)rdr["FileHASH"];
            LineCount = (int)rdr["LineCount"];

            stpFuid = (int)rdr["stpFuid"];
            stpReturnCode = (int)rdr["stpReturnCode"];
            stpErrorText = (string)rdr["stpErrorText"];
            stpReferenceCode = (string)rdr["stpReferenceCode"];

            ArchivedPath = (string)rdr["ArchivedPath"];
            ExceptionPath = (string)rdr["ExceptionPath"];
            Status = (int)rdr["Status"];
            MAX_DATE = (DateTime)rdr["MAX_DATE"];
        }
        // READ AND FILL UP TABLE through JobCycleNo
        public void ReadDataTableFileMonitorLogByCycleNo(string InOperator, string InSignedId, int InJobCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + " WHERE RMCycleNo = @RMCycleNo  "
               + " ORDER By DateTimeReceived DESC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycleNo", InJobCycleNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableFileMonitorLog);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            InsertWReport72(InSignedId);
        }


        // READ 
        public void ReadFileMonitorLogBy_ATM_DateOfFile(string InOperator, string InSignedId, DateTime InCutOff, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string WDateOfFile = InCutOff.ToString("yyyy-MM-dd");

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + " WHERE left([FileName], 8) = @AtmNo AND DateOfFile = @DateOfFile AND Status = 1  AND SystemOfOrigin = 'ATMs' "
               + " ORDER By RMCycleNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateOfFile", WDateOfFile);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

                            break;

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

        // READ 
        public void ReadFindFuidBased_ATM_DateOfFile(string InReplDate, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
          //  string WDateOfFile = InCutOff.ToString("yyyy-MM-dd");

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + " WHERE left([FileName], 8) = @AtmNo AND DateOfFile = @DateOfFile AND Status = 1   "
               + "  AND SystemOfOrigin = 'ATMs' ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@DateOfFile", InReplDate);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

                            break;

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

        // READ AND FILL UP TABLE through JobCycleNo

        public void ReadFileMonitorLogBy_ATM(string InOperator, string InSignedId, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //string WDateOfFile = InCutOff.ToString("yyyy-MM-dd");

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + " WHERE left([FileName], 8) = @AtmNo AND Status = 1 AND SystemOfOrigin = 'ATMs' "
               + " ORDER By RMCycleNo DESC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        //cmd.Parameters.AddWithValue("@DateOfFile", WDateOfFile);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

                            break;

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
        public DateTime DateTimePre;
        public DateTime DateTimeCur;
        // READ AND FILL UP TABLE through JobCycleNo
        public void ReadFileMonitorLogBy_ATM_MissingSequence(string InOperator, string InSignedId, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
    // FIND THE CANDIDATES        
            string SqlString =
               " ;WITH CTE AS ( "
+ " SELECT "
+ "        Left(FileName, 8) AS AtmNo  "
+ "		, DateOfFile  , DateExpected "
+ "		, ATMS.dbo.fnDateToJul(DateOfFile) AS CurFile  "
+ "		, LAG(ATMS.dbo.fnDateToJul(DateOfFile)) OVER (PARTITION BY Left(FileName, 8)  "
+ "		ORDER BY ATMS.dbo.fnDateToJul(DateOfFile)) AS PrevFile  "
+ "		FROM [ATMS].[dbo].[ReconcFileMonitorLog]  "
//+ "		WHERE SystemOfOrigin = 'ATMs'  AND DateOfFile  > @Cut_Off_Date_Minus_1 "
+ "		WHERE SystemOfOrigin = 'ATMs'  AND DateExpected  > @Cut_Off_Date_Minus_1 "

+ ")  "
+ " SELECT  "
+ "  ATMNO  "
+ ", DateOfFile  "
+ ", CurFile AS CurrentFileDateJul   "
+ ", PrevFile AS PreviousFileDateJul  "
+ ", CurFile - PrevFile As DiffFile  "
+ ", ATMS.dbo.fnJulToDate(CurFile) AS CurrentFileDate  "
+ ", ATMS.dbo.fnJulToDate(PrevFile) AS PreviousFileDate  "
+ ", DATEDIFF(day,ATMS.dbo.fnJulToDate(CurFile), ATMS.dbo.fnJulToDate(PrevFile)) AS DaysDiff  "
+ "FROM CTE  "
+ "WHERE (CurFile - PrevFile) > 1  AND DateExpected > @Cut_Off_Date_Minus_2 "
+ "ORDER BY ATMNO, DateExpected, CurFile  "
                ;
            DataTableAtmsWithGaps_1 = new DataTable();
            DataTableAtmsWithGaps_1.Clear();

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Cut_Off_Date_Minus_1", InCut_Off_Date.AddDays(-20));
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Cut_Off_Date_Minus_2", InCut_Off_Date.AddDays(-5));
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        sqlAdapt.SelectCommand.CommandTimeout = 800; 

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAtmsWithGaps_1);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

          //  return; 

            DataTableAtmsWithGaps_2 = new DataTable();
            DataTableAtmsWithGaps_2.Clear();

            DataTableAtmsWithGaps_2.Columns.Add("AtmNo", typeof(string));      
            DataTableAtmsWithGaps_2.Columns.Add("DateOfFile", typeof(string));
            DataTableAtmsWithGaps_2.Columns.Add("CurrentFileDateJul", typeof(int));
            DataTableAtmsWithGaps_2.Columns.Add("PreviousFileDateJul", typeof(int));
            DataTableAtmsWithGaps_2.Columns.Add("DiffFile", typeof(int));
            DataTableAtmsWithGaps_2.Columns.Add("CurrentFileDate", typeof(DateTime));
            DataTableAtmsWithGaps_2.Columns.Add("PreviousFileDate", typeof(DateTime));
            DataTableAtmsWithGaps_2.Columns.Add("DaysDiff", typeof(int));
            DataTableAtmsWithGaps_2.Columns.Add("MissingTotal", typeof(int));
            DataTableAtmsWithGaps_2.Columns.Add("Comment", typeof(string));
            
            int I = 0;

            while (I <= (DataTableAtmsWithGaps_1.Rows.Count - 1))
            {

                string WAtmNo = (string)DataTableAtmsWithGaps_1.Rows[I]["AtmNo"];
                string WDateOfFile = (string)DataTableAtmsWithGaps_1.Rows[I]["DateOfFile"];
                int WCurrentFileDateJul = (int)DataTableAtmsWithGaps_1.Rows[I]["CurrentFileDateJul"];
                int WPreviousFileDateJul = (int)DataTableAtmsWithGaps_1.Rows[I]["PreviousFileDateJul"];
                int WDiffFile = (int)DataTableAtmsWithGaps_1.Rows[I]["DiffFile"];
                DateTime WCurrentFileDate = (DateTime)DataTableAtmsWithGaps_1.Rows[I]["CurrentFileDate"];
                DateTime WPreviousFileDate = (DateTime)DataTableAtmsWithGaps_1.Rows[I]["PreviousFileDate"];
                int WDaysDiff = (int)DataTableAtmsWithGaps_1.Rows[I]["DaysDiff"];

                // FIND IF TRANSACTIONS IN IST IN BETWEEN
                if (WPreviousFileDate == NullPastDate)
                {
                    I = I + 1; 
                    continue;
                }
                //
                int MissingTotal = ReadFindTableInBetweenDatesIN_IST(WAtmNo, WPreviousFileDate, WCurrentFileDate);

                string TempComment = ""; 

                if (MissingTotal > 0)
                {
                    TempComment = "Records In Betwwen"; 
                }
                else
                {
                    TempComment = "NO Records In Betwwen";
                }

                // Create Table 

                DataRow RowSelected = DataTableAtmsWithGaps_2.NewRow();

                RowSelected["AtmNo"] = WAtmNo;
                RowSelected["DateOfFile"] = WDateOfFile;
                RowSelected["CurrentFileDateJul"] = WCurrentFileDateJul;
                RowSelected["PreviousFileDateJul"] = WPreviousFileDateJul;
                RowSelected["DiffFile"] = WDiffFile;
                RowSelected["CurrentFileDate"] = WCurrentFileDate;
                RowSelected["PreviousFileDate"] = WPreviousFileDate;
                RowSelected["DaysDiff"] = WDaysDiff;
                RowSelected["MissingTotal"] = MissingTotal;
                RowSelected["Comment"] = TempComment;

               

                // ADD ROW
                DataTableAtmsWithGaps_2.Rows.Add(RowSelected);

                I++; // Read Next entry of the table 

            }

        }

        // Find In Between dates for IST
        public int ReadFindTableInBetweenDatesIN_IST(string InTerminalId, DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //
            // Check IST for transactions in between
            //
            DataTableAtmsWithGaps_3 = new DataTable();
            DataTableAtmsWithGaps_3.Clear();

            string SqlString =
               " SELECT TerminalId, Net_TransDate, count(*)  as totalsByDate "
                + "    FROM[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + "    WHERE TerminalId = @TerminalId "
                + "    AND (NET_TransDate > @DateFrom AND NET_TransDate < @DateTo) "
                + "      AND TXNSRC = '1' and ( MatchingCateg BETWEEN 'BDC201' AND 'BDC209' ) "
                + "   group by TerminalId, Net_TransDate ";

            using (SqlConnection conn =
         new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateFrom", InDateFrom); // We ADD 1 to start from inbetwwen date
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DateTo", InDateTo); // myDate < '20090201 00:00:00' 
                       
                        sqlAdapt.SelectCommand.CommandTimeout = 300;

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAtmsWithGaps_3);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
            int GrandTotal = 0; 
            int I = 0; 
            while (I <= (DataTableAtmsWithGaps_3.Rows.Count - 1))
            {

                int WtotalsByDate = (int)DataTableAtmsWithGaps_3.Rows[I]["totalsByDate"];

                GrandTotal = GrandTotal + WtotalsByDate; 

                I++; // Read Next entry of the table 

            }

            return GrandTotal; 
        }
        // READ AND FILL UP TABLE through JobCycleNo with error 
        public void ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(string InOperator, string InSignedId, int InJobCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + " WHERE RMCycleNo = @RMCycleNo AND Status = 0 "
               + " ORDER By DateTimeReceived DESC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycleNo", InJobCycleNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableFileMonitorLog);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            InsertWReport72(InSignedId);
        }
        public int TotalSelected = 0;
        //
        // READ LOADED File by CYCLE NUMBER 
        //
        public bool IsFileNOTLoaded; 
        public void ReadLoadedFiles_Fill_Table(string InOperator, int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "";

            IsFileNOTLoaded = false; 

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableFileMonitorLog.Columns.Add("SeqNo", typeof(int));
            DataTableFileMonitorLog.Columns.Add("FileName", typeof(string));
            DataTableFileMonitorLog.Columns.Add("Status", typeof(string));

            DataTableFileMonitorLog.Columns.Add("MIN Date", typeof(string));
            DataTableFileMonitorLog.Columns.Add("MAX Date", typeof(string));

            DataTableFileMonitorLog.Columns.Add("DateTimeReceived", typeof(string));

            DataTableFileMonitorLog.Columns.Add("DateExpected", typeof(string));

            DataTableFileMonitorLog.Columns.Add("LineCount", typeof(int));
            DataTableFileMonitorLog.Columns.Add("stpErrorText", typeof(string));


            SqlString = " SELECT * "
        + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
        + " WHERE RMCycleNo = @RMCycleNo AND SourceFileID <> 'Atms_Journals_Txns' AND SystemOfOrigin <> 'MOBILE_WALLET' "
        + " ORDER By SeqNo ASC ";


            //SqlConnection conn2 = new SqlConnection(connectionString);
            //conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);


                            //FILL TABLE 
                            DataRow RowSelected = DataTableFileMonitorLog.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["FileName"] = SourceFileID;

                            if (Status == 1)
                            {
                                // OK
                                RowSelected["Status"] = "Loaded";
                            }
                            else
                            {
                                RowSelected["Status"] = "Not Loaded";

                                IsFileNOTLoaded = true;

                                RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
                                int Mode = 7; // Start Action 
                                string ProcessName = "Auto_Processed";
                                string Message = "Not Loaded File..."+ SourceFileID;
                                DateTime SavedStartDt = DateTime.Now;

                                Pt.InsertPerformanceTrace_With_USER(InOperator, InOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, "Controller", InRMCycleNo);

                            }

                            RowSelected["DateTimeReceived"] = DateTimeReceived.ToString();

                            // Pass the actual file id 
                            //string WFileName = SeqNo.ToString(); 
                            //DateTime MinDt = Mgt_BDC.ReadAndFindMinDtForFile(SourceFileID, InRMCycleNo, WFileName, conn2);

                            //if (MinDt.Date == NullPastDate)
                            //{
                            //    RowSelected["MIN Date"] = "NO MIN";
                            //}
                            //else
                            //{
                            //    RowSelected["MIN Date"] = MinDt.ToString();
                            //}

                          
                            RowSelected["MIN Date"] = "****";
                            

                            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();
                            Msf.ReadReconcSourceFilesByFileId(SourceFileID);


                            if (MAX_DATE.Date == NullPastDate)
                            {
                                RowSelected["MAX Date"] = "NO MAX";
                            }
                            else
                            {
                                RowSelected["MAX Date"] = MAX_DATE.ToString();
                            }


                            RowSelected["DateExpected"] = DateExpected.ToShortDateString();
                            RowSelected["LineCount"] = LineCount;
                            RowSelected["stpErrorText"] = stpErrorText;

                            // ADD ROW
                            DataTableFileMonitorLog.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // conn2.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    // conn2.Close();
                    CatchDetails(ex);
                }
        }

        //
        public void ReadLoadedFiles_Fill_Table_MOBILE(string InOperator, int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "";

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableFileMonitorLog.Columns.Add("SeqNo", typeof(int));
            DataTableFileMonitorLog.Columns.Add("FileName", typeof(string));
            DataTableFileMonitorLog.Columns.Add("Status", typeof(string));

            DataTableFileMonitorLog.Columns.Add("MIN Date", typeof(string));
            DataTableFileMonitorLog.Columns.Add("MAX Date", typeof(string));

            DataTableFileMonitorLog.Columns.Add("DateTimeReceived", typeof(string));

            DataTableFileMonitorLog.Columns.Add("DateExpected", typeof(string));

            DataTableFileMonitorLog.Columns.Add("LineCount", typeof(int));
            DataTableFileMonitorLog.Columns.Add("stpErrorText", typeof(string));


            SqlString = " SELECT * "
        + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
        + " WHERE RMCycleNo = @RMCycleNo  "
        + " ORDER By SeqNo ASC ";


            //SqlConnection conn2 = new SqlConnection(connectionString);
            //conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);


                            //FILL TABLE 
                            DataRow RowSelected = DataTableFileMonitorLog.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["FileName"] = SourceFileID;

                            if (Status == 1)
                            {
                                // OK
                                RowSelected["Status"] = "Loaded";
                            }
                            else
                            {
                                RowSelected["Status"] = "Not Loaded";
                            }

                            RowSelected["DateTimeReceived"] = DateTimeReceived.ToString();

                            // Pass the actual file id 
                            //string WFileName = SeqNo.ToString(); 
                            //DateTime MinDt = Mgt_BDC.ReadAndFindMinDtForFile(SourceFileID, InRMCycleNo, WFileName, conn2);

                            //if (MinDt.Date == NullPastDate)
                            //{
                            //    RowSelected["MIN Date"] = "NO MIN";
                            //}
                            //else
                            //{
                            //    RowSelected["MIN Date"] = MinDt.ToString();
                            //}

                            RowSelected["MIN Date"] = "****";

                            RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();
                            Msf.ReadReconcSourceFilesByFileId(SourceFileID);


                            if (MAX_DATE.Date == NullPastDate)
                            {
                                RowSelected["MAX Date"] = "NO MAX";
                            }
                            else
                            {
                                RowSelected["MAX Date"] = MAX_DATE.ToString();
                            }


                            RowSelected["DateExpected"] = DateExpected.ToShortDateString();
                            RowSelected["LineCount"] = LineCount;
                            RowSelected["stpErrorText"] = stpErrorText;

                            // ADD ROW
                            DataTableFileMonitorLog.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // conn2.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    // conn2.Close();
                    CatchDetails(ex);
                }
        }

        //
        // READ LOADED File by File Id 
        //
        public void ReadLoadedFiles_Fill_Table_By_FileId(string InOperator, string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "";

            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableFileMonitorLog.Columns.Add("SeqNo", typeof(int));
            DataTableFileMonitorLog.Columns.Add("FileName", typeof(string));
            DataTableFileMonitorLog.Columns.Add("Status", typeof(string));

            DataTableFileMonitorLog.Columns.Add("DateTimeReceived", typeof(string));
            DataTableFileMonitorLog.Columns.Add("DateExpected", typeof(string));
            DataTableFileMonitorLog.Columns.Add("Diff InDays", typeof(string));

            DataTableFileMonitorLog.Columns.Add("LineCount", typeof(int));
            DataTableFileMonitorLog.Columns.Add("stpErrorText", typeof(string));


            SqlString = " SELECT * "
        + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
        + " WHERE SourceFileID = @SourceFileID "
        + " ORDER By SeqNo DESC ";


            //SqlConnection conn2 = new SqlConnection(connectionString);
            //conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileID", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);


                            //FILL TABLE 
                            DataRow RowSelected = DataTableFileMonitorLog.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["FileName"] = FileName;

                            if (Status == 1)
                            {
                                // OK
                                RowSelected["Status"] = "Loaded";
                            }
                            else
                            {
                                RowSelected["Status"] = "Not Loaded";
                            }

                            RowSelected["DateTimeReceived"] = DateTimeReceived.ToString();

                            RowSelected["DateExpected"] = DateExpected.ToShortDateString();

                            double numberOfDays = (DateExpected - DateTimeReceived).TotalDays;

                            RowSelected["Diff InDays"] = numberOfDays.ToString();
                            RowSelected["LineCount"] = LineCount;
                            RowSelected["stpErrorText"] = stpErrorText;

                            // ADD ROW
                            DataTableFileMonitorLog.Rows.Add(RowSelected);


                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                    // conn2.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    // conn2.Close();
                    CatchDetails(ex);
                }
        }

        //
        // READ BY SEQNO 
        //
        public void ReadLoadedFilesBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            string SqlString = " SELECT * "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + " WHERE SeqNo = @SeqNo ";
            //RMCycleNo


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

                            ReadFields(rdr);

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
        // READ BY SEQNO 
        //
        public DateTime ReadLoadedFilesBySeqNoToFindDate(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime FileDate = NullPastDate;

            string SqlString = " SELECT Cast(Left ((Right(FileName, 12)), 8)  As Date) As FileDate "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + " WHERE SeqNo = @SeqNo ";
            //RMCycleNo


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

                            FileDate = (DateTime)rdr["FileDate"];

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
            return FileDate;
        }

        //
        // READ BY CycleNumber And Create Data String 
        //
        public string ALL_stpErrorText;
        //
        public void ReadLoadedFilesByCycleNumber_No_Journals(int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ALL_stpErrorText = "";

            string SqlString = " SELECT * "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + " WHERE RMCycleNo = @RMCycleNo AND SourceFileID <>'Atms_Journals_Txns' ";
            //RMCycleNo
            // WHERE SourceFileID <>'Atms_Journals_Txns'


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

                            ALL_stpErrorText = ALL_stpErrorText + stpErrorText + Environment.NewLine;
                            ALL_stpErrorText += "NEW FILE" + Environment.NewLine;

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
        public int Journal_Total;
        public int Files_Total;
        public void ReadLoadedFilesByCycleNumber_All(int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Journal_Total = 0;
            Files_Total = 0;

            ALL_stpErrorText = "";

            string SqlString = " SELECT * "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + " WHERE RMCycleNo = @RMCycleNo ";
            //RMCycleNo
            // WHERE SourceFileID <>'Atms_Journals_Txns'


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

                            if (SourceFileID == "Atms_Journals_Txns")
                            {
                                Journal_Total = Journal_Total + 1;
                            }
                            else
                            {
                                Files_Total = Files_Total + 1;
                            }


                            ALL_stpErrorText = ALL_stpErrorText + stpErrorText + Environment.NewLine;
                            ALL_stpErrorText += "NEW FILE" + Environment.NewLine;

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

        public void ReadLoadedFilesAND_Find_The_Latest(string InSourceFileID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT TOP(1) * "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + " WHERE SourceFileID = @SourceFileID AND SourceFileID <>'Atms_Journals_Txns' "
             + " ORDER BY RMCycleNo DESC ";
            //RMCycleNo
            // WHERE SourceFileID <>'Atms_Journals_Txns'


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SourceFileID", InSourceFileID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

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

        public void ReadLoadedFilesAND_Find_If_Loaded(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = " SELECT TOP(1) * "
             + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
             + InSelectionCriteria; 
         
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@SourceFileID", InSourceFileID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadFields(rdr);

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


        // Insert 
        public void InsertWReport72(string InSignedId)
        {

            if (DataTableFileMonitorLog.Rows.Count > 0)
            {
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport72();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport72]";

                            foreach (var column in DataTableFileMonitorLog.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(DataTableFileMonitorLog);
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

        // READ AND FILL UP TABLE through ATMNo
        public void ReadDataTableFileMonitorLogByAtmNo(string InOperator, string InSignedId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileMonitorLog = new DataTable();
            DataTableFileMonitorLog.Clear();

            //// DATA TABLE ROWS DEFINITION 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[ReconcFileMonitorLog]"
               + InSelectionCriteria
               + " ORDER By DateOfFile DESC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycleNo", InAtmNo);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableFileMonitorLog);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //InsertWReport72(InSignedId);
        }

        /// <summary>
        /// Truncate [dbo].[ReconcFileAgentLog] -- for POC purposes only
        /// </summary>
        public void TruncateTable()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdTruncate = "TRUNCATE TABLE [dbo].[ReconcFileMonitorLog] ";
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <summary>
        /// INSERT new record into ReconcFileMonitorLog
        /// </summary>
        public int Insert()
        {

            ErrorFound = false;
            ErrorOutput = "";
            //RMCycleNo
            string cmdInsert = " INSERT INTO [dbo].[ReconcFileMonitorLog] " +
                                    "( " +
                                    "[RMCycleNo],  " +
                                    "[SystemOfOrigin], [SourceFileID], [StatusVerbose],[FileName], [FileSize], [DateTimeReceived], " +
                                    "[DateExpected], [DateOfFile], " +
                                    "[FileHASH], [LineCount], [stpFuid], "
                                    + "[ArchivedPath], [ExceptionPath], [Status] " +
                                    ")" +
                                " VALUES ( " +
                                    "@RMCycleNo,  " +
                                    "@SystemOfOrigin, @SourceFileID, @StatusVerbose, @FileName, @FileSize, @DateTimeReceived, @DateExpected, @DateOfFile, @FileHASH, @LineCount, @stpFuid,"
                                    + " @ArchivedPath, @ExceptionPath, @Status ) " +
                                " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);
                        cmd.Parameters.AddWithValue("@SourceFileID", SourceFileID);
                        cmd.Parameters.AddWithValue("@StatusVerbose", StatusVerbose);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeReceived", DateTimeReceived);
                        cmd.Parameters.AddWithValue("@DateExpected", DateExpected);
                        cmd.Parameters.AddWithValue("@DateOfFile", DateOfFile);
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);
                        cmd.Parameters.AddWithValue("@stpFuid", stpFuid);
                        cmd.Parameters.AddWithValue("@ArchivedPath", ArchivedPath);
                        cmd.Parameters.AddWithValue("@ExceptionPath", ExceptionPath);
                        cmd.Parameters.AddWithValue("@Status", Status);


                        SeqNo = (int)cmd.ExecuteScalar();
                        // int rows = cmd.ExecuteNonQuery();

                        if (SeqNo != 0)
                        {
                            ErrorFound = false;
                            ErrorOutput = "";
                        }
                        else
                        {
                            ErrorFound = true;
                            ErrorOutput = "An error occured while INSERTING in [ReconcFileMonitorLog]... ";
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    SeqNo = 0;
                    ErrorFound = true;
                    ErrorOutput = "An error occured while INSERTING in [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
            return (SeqNo);
        }



        /// <summary>
        /// UPDATE record in RRDMReconcFileMonitorLog
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Update(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ReconcFileMonitorLog] SET "
                                    + " SystemOfOrigin = @SystemOfOrigin, SourceFileID = @SourceFileID , StatusVerbose = @StatusVerbose , FileName = @FileName, "
                                    + " FileSize = @FileSize, DateTimeReceived = @DateTimeReceived, "
                                    + " FileHASH = @FileHash, DateExpected = @DateExpected, DateOfFile = @DateOfFile, "
                                    + " LineCount = @LineCount, "
                                     + " stpFuid = @stpFuid, "
                                      + " stpReturnCode = @stpReturnCode, "
                                       + " stpErrorText = @stpErrorText, "
                                        + " stpReferenceCode = @stpReferenceCode, "
                                    + " ArchivedPath = @ArchivedPath, "
                                     + " ExceptionPath = @ExceptionPath, "
                                    + " Status = @Status "
                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@SystemOfOrigin", SystemOfOrigin);
                        cmd.Parameters.AddWithValue("@RMCycleNo", RMCycleNo);
                        cmd.Parameters.AddWithValue("@SourceFileID", SourceFileID);
                        cmd.Parameters.AddWithValue("@StatusVerbose", StatusVerbose);
                        cmd.Parameters.AddWithValue("@FileName", FileName);
                        cmd.Parameters.AddWithValue("@FileSize", FileSize);
                        cmd.Parameters.AddWithValue("@DateTimeReceived", DateTimeReceived);
                        cmd.Parameters.AddWithValue("@DateExpected", DateExpected);
                        cmd.Parameters.AddWithValue("@DateOfFile", DateOfFile);
                        cmd.Parameters.AddWithValue("@FileHASH", FileHASH);
                        cmd.Parameters.AddWithValue("@LineCount", LineCount);
                        cmd.Parameters.AddWithValue("@stpFuid", stpFuid);
                        cmd.Parameters.AddWithValue("@stpReturnCode", stpReturnCode);
                        cmd.Parameters.AddWithValue("@stpErrorText", stpErrorText);
                        cmd.Parameters.AddWithValue("@stpReferenceCode", stpReferenceCode);
                        cmd.Parameters.AddWithValue("@ArchivedPath", ArchivedPath);
                        cmd.Parameters.AddWithValue("@ExceptionPath", ExceptionPath);
                        cmd.Parameters.AddWithValue("@Status", Status);


                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while UPDATING [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }
        /// <summary>
        /// UPDATE MAX_DATE
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Update_MAX_DATE(string InSourceFileID, int InSeqNo, int InRMCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            DateTime MaxDt = new DateTime(1900, 01, 01);
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            // Check That File Exist in target data base 
            string TargetDB = "[RRDM_Reconciliation_ITMX]";
            Cv.ReadTableToSeeIfExist(TargetDB, InSourceFileID);

            if (Cv.RecordFound == true)
            {
                // File Exist
                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

                string WFileName = InSeqNo.ToString();
                MaxDt = Mgt.ReadAndFindMaxDtForFile(InSourceFileID, InRMCycleNo, WFileName);

            }
            else
            {
                MaxDt = NullPastDate;
               // return; 
            }

           

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ReconcFileMonitorLog] SET "
                                    + " MAX_DATE = @MAX_DATE "
                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@MAX_DATE", MaxDt);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while UPDATING [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <param name="InSeqNo"></param>
        

        /// <summary>
        /// UPDATE MAX_DATE
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Update_MAX_DATE_MOBILE(string InApplication,  string InSourceFileID, int InSeqNo, int InRMCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            DateTime MaxDt = new DateTime(1900, 01, 01);
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            // Check That File Exist in target data base 
            string TargetDB = InApplication;
            Cv.ReadTableToSeeIfExist(TargetDB, InSourceFileID);

            if (Cv.RecordFound == true)
            {
                // File Exist
                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

                string WFileName = InSeqNo.ToString();
                MaxDt = Mgt.ReadAndFindMaxDtForFile_MOBILE(InApplication,InSourceFileID, InRMCycleNo, WFileName);

            }
            else
            {
                MaxDt = NullPastDate;
                // return; 
            }



            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ReconcFileMonitorLog] SET "
                                    + " MAX_DATE = @MAX_DATE "
                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@MAX_DATE", MaxDt);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while UPDATING [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <summary>
        /// UPDATE record in RRDMReconcFileMonitorLog
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Update_stpErrorText(int InSeqNo, string In_stpErrorText)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ReconcFileMonitorLog] SET "

                                       + " stpErrorText = @stpErrorText "

                                    + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@stpErrorText", In_stpErrorText);


                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while UPDATING [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <summary>
        /// DELETE record in ReconcFileMonitorLog
        /// </summary>
        /// <param name="InSeqNo"></param>
        public void Delete_MOBILE(int InRMCycle)
        {
            //DELETE FROM[ATMS].[dbo].[ReconcFileMonitorLog]
            //     WHERE RMCycleNo = @RMCycleNo AND SourceFileID<> 'Atms_Journals_Txns'
            //set @rowcount =  @@ROWCOUNT

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM ATMS.[dbo].[ReconcFileMonitorLog] "
                                                        + " WHERE RMCycleNo = @RMCycleNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycle);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows != 1)
                        {
                            // ToDo
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured while DELETING from [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <summary>
        /// GetRecord given its SeqNo
        /// </summary>
        /// <param name="SeqNo"></param>
        public void GetRecordBySeqNo(int SeqNo)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from [ReconcFileMonitorLog]... ";

            // string connectionString = ConfigurationManager.ConnectionStrings["AgentConnectionString"].ConnectionString;

            string SqlString = "SELECT * FROM [dbo].[ReconcFileMonitorLog]"
                            + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ErrorFound = false;
                            ErrorOutput = "";

                            ReadFields(rdr);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING from [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// Get Totals by ReplCycle
        /// </summary>
        /// <param 
        public void GetTotals(int InRMCycleNo)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = " ";

            ValidTotalForCycle = 0;
            InValidTotalForCycle = 0;

            string SqlString = "SELECT count(*) As Valid FROM [dbo].[ReconcFileMonitorLog] "
                            + " WHERE RMCycleNo = @RMCycleNo AND SourceFileID = 'Atms_Journals_Txns' AND Status = 1 ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ValidTotalForCycle = (int)rdr["Valid"];
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
            // *****
            //
            SqlString = "SELECT count(*) As Valid FROM [dbo].[ReconcFileMonitorLog] "
                           + " WHERE RMCycleNo = @RMCycleNo AND SourceFileID = 'Atms_Journals_Txns' AND Status = 0 ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            InValidTotalForCycle = (int)rdr["Valid"];
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

        /// <summary>
        /// GetRecord given its SeqNo
        /// </summary>
        /// <param name="SeqNo"></param>
        public void GetRecordByFuid(int InstpFuid)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from [ReconcFileMonitorLog]... ";

            // string connectionString = ConfigurationManager.ConnectionStrings["AgentConnectionString"].ConnectionString;

            string SqlString = "SELECT * FROM ATMS.[dbo].[ReconcFileMonitorLog]"
                            + " WHERE stpFuid = @stpFuid ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@stpFuid", InstpFuid);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ErrorFound = false;
                            ErrorOutput = "";

                            ReadFields(rdr);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING from [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }


        /// <summary>
        /// Check if an entry with the specific FileHASH value exists in the table
        /// and that the status = 1 (meaning the file was processed successfully)
        /// If it exists, the column values are copied in the properties of the class
        /// </summary>
        /// <param name="FileHash"></param>
        public void GetRecordByFileHASH(string FileHash)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [dbo].[ReconcFileMonitorLog]"
                            + " WHERE FileHASH = @FileHASH AND Status = 1 ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FileHASH", FileHash);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            RecordFound = true;
                            while (rdr.Read())
                            {
                                ReadFields(rdr);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING by FileHASH from [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        /// <summary>
        /// Check if an entry with the specific FileId was read before
        /// </summary>
        /// 
        public void GetRecordByFileName(string InFileName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [dbo].[ReconcFileMonitorLog]"
                            + " WHERE FileName = @FileName  ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FileName", InFileName);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            RecordFound = true;
                            while (rdr.Read())
                            {
                                ReadFields(rdr);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING by FileName from [ReconcFileMonitorLog]... " + ex.Message;
                    CatchDetails(ex);
                }
        }

        // READ LAst succesful record loaded for file 
        public void ReadRecordByLoadedByCycle(string InSourceFileID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT TOP (1) *  FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
                            + " WHERE SourceFileID = @SourceFileID AND Status = 1 "
                            + " ORDER By SeqNo DESC "; // Reads the Last 

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileID", InSourceFileID);
                        //cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            //TotalSelected = TotalSelected + 1;

                            //// Read Fields 
                            ReadFields(rdr);

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
// Read last 20 journals for an ATM 
        public void ReadDataTableLast_20_Journals(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableLast_20_Journals = new DataTable();
            DataTableLast_20_Journals.Clear();

  //          SELECT[SeqNo],[RMCycleNo]
  //    ,[SourceFileID]  ,[StatusVerbose]  ,[FileName]
  //    ,[FileSize] ,[DateTimeReceived] 
  //    ,[stpReturnCode] ,[stpErrorText]
  //      FROM [ATMS].[dbo].[ReconcFileMonitorLog]
  //      WHERE Left(FileName, 8) = '00000017'  AND SourceFileID = 'Atms_Journals_Txns'
  //order by SeqNo DESC
            string SqlString = "SELECT TOP (20) [SeqNo],[RMCycleNo] "
                       + "  ,[SourceFileID]  ,[StatusVerbose]  ,[FileName] "
                       + "  ,[FileSize] ,[DateTimeReceived]  "
                       + "  ,[SourceFileID]  ,[StatusVerbose]  ,[FileName] "
                       + "  ,[stpReturnCode] ,[stpErrorText] "
                       + " FROM [ATMS].[dbo].[ReconcFileMonitorLog] "
                       + " WHERE Left(FileName, 8) = @InAtmNo  AND SourceFileID = 'Atms_Journals_Txns' "
                       + " Order by SeqNo DESC   ";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@InAtmNo", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 190;   // seconds
                        sqlAdapt.Fill(DataTableLast_20_Journals);

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


        }

    }
}

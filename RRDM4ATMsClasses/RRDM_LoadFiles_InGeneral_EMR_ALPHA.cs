using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

using System.IO;
//using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

using System.Globalization;
using System.Text.RegularExpressions;

namespace RRDM4ATMs
{
    public class RRDM_LoadFiles_InGeneral_EMR_ALPHA : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_ALPHA() : base() { }
        //
        // LOADING FILES FOR BANK DE CAIRE
        //
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Find Operator

        string PRX; // "+PRX+"
        string BKName; // "+BKName+"

        long commandExecutionTimeInMs = 0;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);
        readonly DateTime NullFutureDate = new DateTime(1950, 01, 01);

        string SqlString; // Do not delete 

        string WFileSeqNo = "";
        string WOperator;

        DateTime NewVersionDt;

        public DataTable TableATMsDailyStats = new DataTable();

        RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMGasParameters Gp = new RRDMGasParameters();

        readonly string ATMSconnectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
        readonly string recconConnString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;
        DateTime WCut_Off_Date;
        DateTime FileDATEresult;
        // 
        //public DataTable DuplicatesTable = new DataTable();
        public int stpLineCount;
        public int stpReturnCode;
        public string stpErrorText;
        public string stpReferenceCode;
        int LastSeqNo;
        bool TEST;
        string ReversedCut_Off_Date;

        public int WFlogSeqNo;

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        public void InsertRecordsInTableFromTextFile_InBulk(string InOperator, string InTableA
                                           , string InFullPath, string InCondition, string InFullPath2, int InFlogSeqNo)
        {
            // WE WANT as INPUT
            // a) FILE NAME => to see what Method to call eg Switch_IST_Txns 
            // a) Directory and FILE NAME to know whare to get from 

            // WE SHOULD PROVIDE AS OUTPUT
            // a) Status
            // b) Level of process 

            //      < add key = "DB_Location_Local" value = "true" />  
            //      < add key = "DB_Location_Remote" value = "false" />
            //      < add key = "DB_Location_Remote_IP" value = "\\172.17.85.25\c$\" />
            string SavedOriginalInFullPath = InFullPath;
            bool DB_Location_Local = false;
            bool DB_Location_Remote = false;
            string DB_Location_Calling_IP = "";
            bool showFullPath = false;
            string SX = "";

            try
            {
                string DB_Location = ConfigurationManager.AppSettings.Get("DB_Location_Local");
                if (DB_Location == "true")
                    DB_Location_Local = true;
                else
                {
                    DB_Location_Local = false;
                }

                DB_Location = ConfigurationManager.AppSettings.Get("DB_Location_Remote");
                // DB_Location = "true"; 
                if (DB_Location == "true")
                {
                    DB_Location_Remote = true;
                    DB_Location_Calling_IP = ConfigurationManager.AppSettings.Get("DB_Location_Calling_IP");

                    // '\\172.17.85.25\c$\'+ substring(@fullpath,4,len(@fullpath)-3)

                    // 'C:\RRDM\Archives\Atms_Journals_Txns\20200324_200\00000163_20200324_EJ_DBL.000'

                    SX = InFullPath.Substring(3, InFullPath.Length - 3);

                    InFullPath = @"\\" + DB_Location_Calling_IP + "\\c$\\" + SX;

                    //MessageBox.Show("InFullPath:..." + Environment.NewLine
                    //                                   + InFullPath
                    //                               );
                }
                else
                {
                    DB_Location_Remote = false;
                    // DO NOT CHANGE FULL PATH 
                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            bool Panicos = false; 

            if (Panicos == true)
            {
                // DELETE TESTING DATA
                // Read fields of existing table
                RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
                // 
                // CLEAN TABLES from ANY testing data 
                int TempWReconcCycleNo = 999999;
                Bio.CleanTables(InTableA, TempWReconcCycleNo);
            }
            
            PRX = "EMR";
            
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBankToGetName(99);
            //WOperator = Ba.Operator;
            string WBankShortName = Ba.ShortName; // Eg "BDC"

            BKName = WBankShortName; // "+BKName+"

            // FIND CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();


            // IF TEST IS FALSE Message are not shown
            TEST = false;

            WFlogSeqNo = InFlogSeqNo;

            WOperator = InOperator;

            WFileSeqNo = InFlogSeqNo.ToString();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            // Date of the file in string
            //var resultLastThreeDigits = InFullPath.Substring(InFullPath.Length - 3);
            //string result1 = InFullPath.Substring(InFullPath.Length - 12);
            //string result2 = result1.Substring(0, 8);

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {
                WCut_Off_Date = Rjc.Cut_Off_Date;

                ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

            }
            // DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12); // DO NOT ACTIVATE THIS
            //
            // FIND DATE OF THE FILE TO BE USED AS SETTLEMENT DATE FOR MASTER 
            //
            var resultLastThreeDigits = InFullPath.Substring(InFullPath.Length - 3);
            string result1 = InFullPath.Substring(InFullPath.Length - 12);
            string result2 = result1.Substring(0, 8);

            // GET THE FILEDATEResult

            if (DateTime.TryParseExact(result2, "yyyyMMdd", CultureInfo.InvariantCulture
                          , DateTimeStyles.None, out FileDATEresult))
            {

            }

            

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();

            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            stpLineCount = 0;
            stpReturnCode = -1;
            LastSeqNo = 0;
            //
            // GO TO LOAD
            //
            string text = "In Process Loading of .. " + InTableA;
            string caption = "LOADING OF FILES";
            int timeout = 2000;
            AutoClosingMessageBox.Show(text, caption, timeout);

            switch (InTableA)
            {
                case "base24":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_base24" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_base24(InTableA, InFullPath, WReconcCycleNo);

                        //
                        // Copy file to the Archive Directory
                        //

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);
                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath
                                                         );
                        }
                        //MessageBox.Show("Loaded "+ InTableA); 
                        break;
                    }
                case "bMaster":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_bMaster" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_bMaster(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);

                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath
                                                         );
                        }
                        //MessageBox.Show("Loaded " + InTableA);
                        break;
                        //
                    }
                case "CIT_Speed_Excel":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_GL_Balances_Atms_Daily" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_CIT_Speed_Excel(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);

                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath
                                                         );
                        }
                        //MessageBox.Show("Loaded " + InTableA);
                        break;
                        //
                    }
                case "GL_Balances_Atms_Daily":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_GL_Balances_Atms_Daily" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_GL_Balances_Atms_Daily(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);

                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath
                                                         );
                        }
                        //MessageBox.Show("Loaded " + InTableA);
                        break;
                        //
                    }
                
                
               
                default:
                    {
                        //stpErrorText = DateTime.Now + "_" + "01_Start_Loading_"+ InTableA + "\r\n";

                        //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GENERAL_AUTO_FILE(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {


                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);

                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath
                                                         );
                        }

                        //    stpErrorText = "NOT VALID FILE ID";
                        break;
                    }
            }

            // IT IS TEMPORARY 



            //}

        }

        //
        // base24 LOAD
        //
        // GTL = GET TRANSFORM LOAD
        public void InsertRecords_GTL_base24(string InOriginFileName, string InFullPath
                                                                   , int InReconcCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //

            string SQLCmd;

            string WTerminalType = "10";
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string InDelimiter = ",";

            // Truncate  BULK

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

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

                    stpErrorText = stpErrorText + "Cancel During  BULK Truncate";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            //******************************
            // UNDO FILES AFTER MOVING THEM FROM MATCHED TO
            //******************************

            // CALL STORE PROCEDURE TO UNDO THE REST
            // THESE ARE FILES LIKE Reconciliation Sessions etc. 

            

            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";
            int ret = -1;
            string WBULK_ID = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName; 

            string connectionStringITMX = ConfigurationManager.ConnectionStrings
                 ["ReconConnectionString"].ConnectionString;

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_BULK_INSERT_ALPHA_FILES]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@BULK_ID", WBULK_ID.Trim()));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01.Trim()));
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 300;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    //ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }

            

            RecordFound = false;

            // Find LAST SEQ NO
            // REASON: We needed it when we populate TWIN to read from IST all > that this SEQ NO
            int BULK_LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
               + " WITH(NOLOCK) "
               + " ";

            using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BULK_LastSeqNo = (int)rdr["MaxSeqNo"];

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
                    stpErrorText = stpErrorText + "Cancel At _Finding The BULK Maximum";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }


            // INSERT TO ALL
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
                + " Select * " + " ,@RMCycle " + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    conn.StatisticsEnabled = true;
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 300;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel During Bulk Insert To ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = false;

            // Find LAST SEQ NO
            // REASON: We needed it when we populate TWIN to read from IST all > that this SEQ NO
            LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo]."+InOriginFileName
               + " WITH(NOLOCK) "
               + " WHERE Processed = 0";

            using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            LastSeqNo = (int)rdr["MaxSeqNo"];

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
                    stpErrorText = stpErrorText + "Cancel At _Finding The Maximum";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }

           

            RecordFound = false;

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo]."+InOriginFileName // base24
                + "( "
                 + " [OriginFileName] "
                 + " ,[OriginalRecordId] "

                 + " ,[Origin] "
                 // + " ,[MatchingCateg] "
                 + " ,[TerminalType] "

                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                   + ",[TransCurr] "

                 + ",[TransAmt] "
                 + ",[TraceNo] "
                 + ",[RRNumber] "
                 + ",[FullTraceNo] "
                 + ",[AUTHNUM]"
                 + ",[Comment]" // Use a temporary field to host RRN in cases of Fawry 

                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "
                 + ",[ResponseCode] "

                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                   //+ ",[TXNSRC] "
                   //  + ",[TXNDEST] "
                  // + ", EXTERNAL_DATE "
                    + " , [Net_TransDate] "
                  
                      //+ ",[ACCEPTOR_ID] "
                      //  + ",[ACCEPTORNAME] "
                     + ",[CAP_DATE] "
                     + ",[SET_DATE] "
                 + ") "
                 + " SELECT "

                 + "@OriginFile "
                + ", SeqNo "
                 + " ,@Origin "

                   + ",'10' " // Terminal Type
                
                  // DATE TIME 
                  + ", CAST(TransactionDate AS DATETIME) + CAST(TransactionTime AS DATETIME) "
                 
                 + ",case " // transaction type
                 + " when Left(TransactionType, 7) = 'Withdra' THEN 11 "
                 + " when Left(TransactionType, 7) = 'Deposit' THEN 23 "
                 + " when TransactionType = 'Payment from current to card' THEN 11 "
                 + " else 99 "
                 + "end "

                 + ",TransactionType " // [TransDescr]

                 + " , CurrencyCode "
                 + " , TransactionAmount "

                 + ",TraceNumber " // TRace 

                 + ", ''" // RRN

                 + ",TraceNumber " // FULL TRACE 
                  + ",'' " // Auth Number 
                  + ", '' " // Comment 
                         
                 + ", ATM_ID " // Terminal Id 
                 + ", CardNumber   " // card number 
                                   
                 + " , CustomerID " // We place the customer id in the place of the account number 
               
                  + ",case " // RESPONSE CODE
                 + " when Left(Response_Status, 8) = 'Approved' THEN '0' "
                 + " else '99' "
                 + "end "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
          
                         //For  Net_TransDate
                         + " ,  CAST(TransactionDate AS DATE)"

                  
                     + ", CAST([Settlement] as date) "
                     + ", CAST([Settlement] as date) "


                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName +"_ALL"
                 + " WHERE  Left(Response_Status, 8) = 'Approved' "
                 + " AND SeqNo > " + BULK_LastSeqNo
             ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo); // seq for file
                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        //    cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        //cmd.Parameters.AddWithValue("@TransCurr", "EGP");

                        cmd.CommandTimeout = 350;  // seconds

                        stpLineCount = cmd.ExecuteNonQuery();

                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted for IST" + Environment.NewLine
                               + "..:.." + stpLineCount.ToString());
                    }


                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At _IST_Insert";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_03_IST_Records_loaded..Records:.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            
            //**********************************************************
            // UPDATE CATEGORY ID BASED ON BINS 
            //**********************************************************
            
            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo]."+InOriginFileName  // base24
                        + "  SET MatchingCateg = case "  // SET UP THE CATEGORY 
                        + " WHEN " // CREDIT CARD
                        +"( Left(CardNumber,6) = '526764' "
                        + " OR Left(CardNumber,8) = '53281675' "
                         + " OR Left(CardNumber,8) = '53239590' "
                          + " OR Left(CardNumber,8) = '53239524' "
                        + ") "
                          + " THEN '" + PRX + "302' " // CREDIT Card
                        + " WHEN " // Debit CARD
                        + " ( Left(CardNumber,6) = '537485' "
                        + " OR Left(CardNumber,8) = '53239519' "
                         + " OR Left(CardNumber,6) = '510215' "
                            + " OR Left(CardNumber,8) = '53239513' "
                          + " OR Left(CardNumber,8) = '51508802' "
                        + ") "
                        + " THEN '" + PRX + "304' " // Debit Card
                       
                  + " ELSE '" + PRX + "306' "
                 + " end "
                 + " WHERE LoadedAtRMCycle =" + InReconcCycleNo
                        ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of CATEGORIES finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**********************************************************
            // UPDATE EXTERNAL_DATE with the transaction Date but No Seconds 
            //**********************************************************

            SQLCmd = "  UPDATE [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName  // base24
                        + "  SET EXTERNAL_DATE = DATEADD(MINUTE, DATEDIFF(MINUTE, 0, TransDate), 0) "  //  
                 + " WHERE LoadedAtRMCycle =" + InReconcCycleNo
                        ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_Updating _EXTERNAL_DATE";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of EXTERNAL_DATE finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS

            string WFile = "[RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName;

            HandleReversals_ALPHA(WFile, InOriginFileName, InReconcCycleNo);

            if (ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "END OF bMaster WITH Error";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // Set return code to 1
                // Succesful completion 

                stpReturnCode = 0;
                return;
            }

            stpErrorText += DateTime.Now + "_" + "END OF base24 WITH SUCCESS";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Set return code to 1
            // Succesful completion 

            stpReturnCode = 0;
        }
        //
        // bMaster - Is the CoreBanking file 
        //
        // GTL = GET TRANSFORM LOAD
        public void InsertRecords_GTL_bMaster(string InOriginFileName, string InFullPath
                                                                   , int InReconcCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //

            string SQLCmd;

            string WTerminalType = "10";
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string InDelimiter = ",";

            // Truncate  BULK

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

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

                    stpErrorText = stpErrorText + "Cancel During  BULK Truncate";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            //******************************
            // UNDO FILES AFTER MOVING THEM FROM MATCHED TO
            //******************************

            // CALL STORE PROCEDURE TO UNDO THE REST
            // THESE ARE FILES LIKE Reconciliation Sessions etc. 

            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";
            int ret = -1;
            string WBULK_ID = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName; 

            string connectionStringITMX = ConfigurationManager.ConnectionStrings
                 ["ReconConnectionString"].ConnectionString;

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_BULK_INSERT_ALPHA_FILES]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@BULK_ID", WBULK_ID.Trim()));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01.Trim()));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 300;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }



            RecordFound = false;

            int BULK_LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
               + " WITH(NOLOCK) "
               + " ";

            using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BULK_LastSeqNo = (int)rdr["MaxSeqNo"];

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
                    stpErrorText = stpErrorText + "Cancel At _Finding The BULK Maximum";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }


            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
                + " Select * " + " ,@RMCycle " + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    conn.StatisticsEnabled = true;
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 300;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel During Bulk bMaster Insert To ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = false;

            // Find LAST SEQ NO
            // REASON: We needed it when we populate TWIN to read from IST all > that this SEQ NO
            LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
               + " WITH(NOLOCK) "
               + " WHERE Processed = 0";

            using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            LastSeqNo = (int)rdr["MaxSeqNo"];
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
                    stpErrorText = stpErrorText + "Cancel At _Finding The Maximum";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }


            //DateTime NullFutureDate = new DateTime(1950, 01, 01);
            RecordFound = false;

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName // bMaster
                + "( "
                 + " [OriginFileName] "
                 + " ,[OriginalRecordId] "

                 + " ,[Origin] "
                 // + " ,[MatchingCateg] "
                 + " ,[TerminalType] "

                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                   + ",[TransCurr] "

                 + ",[TransAmt] "
                 + ",[TraceNo] "
                 + ",[RRNumber] "
                 + ",[FullTraceNo] "
                 + ",[AUTHNUM]"
                 + ",[Comment]" // 

                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "
                 + ",[ResponseCode] "

                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                    //+ ",[TXNSRC] "
                    //  + ",[TXNDEST] "

                    + " , [Net_TransDate] "

                     //+ ",[ACCEPTOR_ID] "
                     //  + ",[ACCEPTORNAME] "
                     + ",EXTERNAL_DATE"
                     + ",[CAP_DATE] "
                     + ",[SET_DATE] "
                 + ") "
                 + " SELECT "

                 + "@OriginFile "
                 + " , SeqNo "
                 + " ,@Origin "

                   + ",'10' " // Terminal Type

                  // DATE TIME // INSERT FUTURE DATE 
                  //+ ", CAST(TransactionDate AS DATETIME) + CAST(TransactionTime AS DATETIME) "
                  + ",cast ('2050-12-31' as datetime) "

                 + ",case " // transaction type
                 + " when TransactionType = 'Foreign Card Withdrawal' THEN 11 "
                 + " when TransactionType = 'DB Card Withdrawal OffLine' THEN 11 "
                 + " when TransactionType = 'DB Card Withdrawal OnLine' THEN 11 "
                 + " when TransactionType = 'Cash Advance' THEN 11 "
                 + " when TransactionType = 'DB Card Deposit OffLine' THEN 23 "
                  + " when TransactionType = 'DB Card Deposit Online' THEN 23 "
                 + " else 99 "
                 + "end "

                 + ",TransactionType " // [TransDescr]

                 + " , CurrencyCode "
                 + " , TransactionAmount "

                // + ",TraceNumber " // 
                  + ",case " // TraceNumber
                 + " when TraceNumber = 'N/A' THEN 0 "
                 + " else TraceNumber "
                 + "end "

                 + ", ''" // RRN

                // + ",TraceNumber " // FULL TRACE 
                  + ",case " // 
                 + " when TraceNumber = 'N/A' THEN 0 "
                 + " else TraceNumber "
                 + "end "
                  + ",'' " // Auth Number 
                  + ", '' " // Comment 

                 + ", ATM_ID " // Terminal Id 
                 + ", CardNumber   " // card number 

                 + " , CustomerID " // We place the customer id in the place of the account number 

                  + ",case " // RESPONSE CODE
                 + " when Response_Status = 'Completed' THEN '0' "
                 + " else '99' "
                 + "end "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                          //For  Net_TransDate
                          // + " ,  CAST(TransactionDate AS DATE)"
                          + ",cast ('2050-12-31' as date) "
                         // FOR EXTERNAL DATE 
                         + ", CAST(TransactionDate AS DATETIME) + CAST(TransactionTime AS DATETIME) "
                     + ", CAST([Settlement] as date) "
                     + ", CAST([Settlement] as date) "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName+"_ALL"
                 + " WHERE Response_Status = 'Completed'  "
                 + " AND TransactionType <> 'ATM Replenishment' "
                   + " AND TransactionType <> 'ATM Deposits' "
                     + " AND TransactionType <> 'DB Corrections on ATM' "
                 + " AND TransactionType <> 'CR Corrections on ATM' "
                  + " AND SeqNo > " + BULK_LastSeqNo
             ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo); // seq for file
                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        //    cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        //cmd.Parameters.AddWithValue("@TransCurr", "EGP");

                        cmd.CommandTimeout = 350;  // seconds

                        stpLineCount = cmd.ExecuteNonQuery();

                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted for bMaster" + Environment.NewLine
                               + "..:.." + stpLineCount.ToString());
                    }


                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At _IST_Insert";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_03_bMaster_Records_loaded..Records:.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // INSERT THE GL TRANSACTIONS

            bool Panicos = true; 

            if (Panicos == true)
            {
                SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[ATMs_GL_DAILY_TRANSACTIONS]" // GL TXNS
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

               + " SELECT "

                 + " ATM_ID " // Terminal Id 
    
                 + ",case " // transaction type
                 + " when TransactionType = 'ATM Replenishment' THEN 11 "
                 + " when TransactionType = 'ATM Deposits' THEN 23 "
                 + " when TransactionType = 'DB Corrections on ATM' THEN 11 "
                 + " when TransactionType = 'CR Corrections on ATM' THEN 23 "
                 + " else 99 "
                 + "end "

               + ",TransactionType " // [TransDescr]

               + " , CurrencyCode "
               + " , TransactionAmount "

                // DATE TIME 
                + ", CAST(TransactionDate AS DATETIME) + CAST(TransactionTime AS DATETIME) "

                + ", 1 " // IsFromCorebanking
                 + ", 0 " // IsFromRRDM_System
                  + ", 0 " // IsFrom_Manual
                  + ", '' " // Comment
                + ", 0 " // Unknown ReplNo Cycle 


                   + ", @LoadedAtRMCycle"
                 + " , @Cut_Off_Date "
               + ", @Operator"


                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
               + " WHERE Response_Status = 'Completed'  "
               + " AND (TransactionType = 'ATM Replenishment' "
                 + " OR TransactionType = 'ATM Deposits' "
                   + " OR TransactionType = 'DB Corrections on ATM' "
               + " OR TransactionType = 'CR Corrections on ATM' ) "
                + " AND SeqNo > " + BULK_LastSeqNo
           ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();

                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo); // seq for file
                            cmd.Parameters.AddWithValue("@Origin", Origin);

                            cmd.Parameters.AddWithValue("@Cut_Off_Date", WCut_Off_Date);

                            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                            cmd.Parameters.AddWithValue("@Operator", WOperator);
                            //cmd.Parameters.AddWithValue("@TransCurr", "EGP");

                            cmd.CommandTimeout = 350;  // seconds

                            stpLineCount = cmd.ExecuteNonQuery();

                            var stats = conn.RetrieveStatistics();
                            commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                        }
                        // Close conn
                        conn.StatisticsEnabled = false;
                        conn.Close();
                        if (Environment.UserInteractive & TEST == true)
                        {
                            System.Windows.Forms.MessageBox.Show("Records Inserted for bMaster" + Environment.NewLine
                                   + "..:.." + stpLineCount.ToString());
                        }


                    }
                    catch (Exception ex)
                    {
                        conn.StatisticsEnabled = false;
                        conn.Close();
                        stpErrorText = stpErrorText + "Cancel At _IST_Insert";

                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;

                        CatchDetails(ex);

                        return;
                    }
                //**********************************************************
                stpErrorText += DateTime.Now + "_" + "Stage_03_bMaster_Records_loaded..Records:.." + stpLineCount.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            }

            // HERE WE GET THE DATE AND TIME FROM based24

            // UPDATE Details from IST Based on Accno and Terminal Id
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-2);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo]."+ InOriginFileName
          + " SET  "
           + " TransDate = t2.TransDate "
          + " ,Net_TransDate = t2.Net_TransDate "
           + " ,CardNumber = t2.CardNumber "
          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[bMaster] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[base24] t2" // base24

          + " ON "
          // + " t1.Processed = t2.Processed "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.TransAmt = t2.TransAmt "
          //+ " AND t1.AccNo = t2.AccNo "
          + " WHERE ( t1.Processed = 0  AND  t2.Processed = 0  ) "
           + "      OR (t2.Processed = 1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus ) "
          // + "      OR ( t2.Net_TransDate>@Net_Minus ) "
          ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Card Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "bMaster Updated from base24..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**********************************************************
            // UPDATE CATEGORY ID BASED ON BINS 
            //**********************************************************

            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName  // bMaster
                        + "  SET MatchingCateg = case "  // SET UP THE CATEGORY 
                        + " WHEN " // CREDIT CARD
                        + "( Left(CardNumber,6) = '526764' "
                        + " OR Left(CardNumber,8) = '53281675' "
                         + " OR Left(CardNumber,8) = '53239590' "
                          + " OR Left(CardNumber,8) = '53239524' "
                        + ") "
                          + " THEN '" + PRX + "302' " // CREDIT Card
                        + " WHEN " // Debit CARD
                        + " ( Left(CardNumber,6) = '537485' "
                        + " OR Left(CardNumber,8) = '53239519' "
                         + " OR Left(CardNumber,6) = '510215' "
                            + " OR Left(CardNumber,8) = '53239513' "
                          + " OR Left(CardNumber,8) = '51508802' "
                        + ") "
                        + " THEN '" + PRX + "304' " // Debit Card

                  + " ELSE '" + PRX + "306' " // Foreign OFF LINE 
                  + " end "
                 + " WHERE LoadedAtRMCycle =" + InReconcCycleNo
                        ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_bMaster Updating Categories";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of bMaster Categories finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE Transaction Trace for bMaster Cash Advance
            //
            TempNewCutOff = WCut_Off_Date.AddDays(-2);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
          + " SET  "
           + " TraceNo = t2.TraceNo "
             + " ,TransDate = t2.TransDate "
          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[bMaster] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[base24] t2" // base24

          + " ON "
          // + " t1.Processed = t2.Processed "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
         // + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.TransAmt = t2.TransAmt "
          + " AND t1.CardNumber = t2.CardNumber "
          + " AND t1.EXTERNAL_DATE = t2.EXTERNAL_DATE "
          + " WHERE ( t1.TransDescr = 'Cash Advance'  AND  t2.TransDescr = 'Withdrawal from card'  ) "
          // + "      OR (t2.Processed = 1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus ) "
          // + "      OR ( t2.Net_TransDate>@Net_Minus ) "
          ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During of Trace For CASH ADVANCE ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "bMaster Updated from base24.Trace For CASH ADVANCE.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS

            string WFile = "[RRDM_Reconciliation_ITMX].[dbo]."+ InOriginFileName;

            HandleReversals_ALPHA(WFile, InOriginFileName, InReconcCycleNo);

            if (ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "END OF bMaster WITH Error";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // Set return code to 1
                // Succesful completion 

                stpReturnCode = 0;
                return;
            }

            stpErrorText += DateTime.Now + "_" + "END OF IST WITH SUCCESS";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Set return code to 1
            // Succesful completion 

            stpReturnCode = 0;
        }

        // HANDLE REVERSALS IST 
        public void HandleReversals_ALPHA(string InTable_DB_Name, string InOriginFileName, int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string SQLCmd;
            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // FOR IST
            int Counter;
            string FileId = InOriginFileName;
            // INSERT In REVERSALS
            SQLCmd = "INSERT INTO "
                           + "[RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                           + "( "
                            + " [FileId] "
                             + " ,[RMCycleNo] "
                             + " ,[MatchingCateg] "
                            + " ,[TerminalId_4] "
                              + " ,[TerminalId_2] "
                             + " ,[CardNumber_4] "
                               + " ,[CardNumber_2] "

                                 + " ,[AccNo_4] "
                               + " ,[AccNo_2] "
                                 + " ,[TransDescr_4] "
                               + " ,[TransDescr_2] "
                                 + " ,[TransCurr_4] "
                               + " ,[TransCurr_2] "

                            + " ,[TransAmt_4] "
                             + " ,[TransAmt_2] "
                              + " ,[TraceNo_4] "
                             + " ,[TraceNo_2] "
                               + " ,[RRNumber_4] "
                              + " ,[RRNumber_2] "
                            + " ,[FullTraceno_4] "
                              + " ,[FullTraceno_2] "
                            + " ,[SeqNo_4] "
                            + " ,[SeqNo_2] "
                            + ",[ResponseCode_4] "
                             + ",[ResponseCode_2] "
                            + ",[TransDate_4] "
                            + ",[TransDate_2] "
                            
                            + ") "
          + " SELECT "
          + " @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "

          + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "

          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
          + ",A.TraceNo As TraceNo_4 ,B.TraceNo As TraceNo_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
        
          + " FROM [RRDM_Reconciliation_ITMX].[dbo]."+ FileId+" A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo]." + FileId + " B "
          + " ON A.TerminalId = B.TerminalId "
         // + " AND A.CardNumber = B.CardNumber "
          //+ " AND A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " AND A.TraceNo = B.TraceNo "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
          + " (A.Processed = 0 ) " 
            + " AND (B.Processed = 0 ) "
           + " AND(A.TransAmt < 0)  AND(B.TransAmt > 0) "
         + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            //+ " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) " // Leave it as it is 
            //+ " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
            //+ " AND (left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            //+ " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            //+ " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            ;
            //    Comment = 'Reversals'
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR .." + FileId + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_IST_1";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals IST..Rec:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            if (Counter>0)
            {
                // Proceed to process reversal 
            }
            else
            {
                return; 
            }

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            Counter = 0;

            string TableId = "[RRDM_Reconciliation_ITMX].[dbo]."+ FileId;
            int SeqNo_2;
            int SeqNo_4;

            SqlString = "SELECT * "
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                  + " WHERE RMCycleNo = @RMCycleNo and FileId = @FileId "
                  + "";
            // OPEN Connection to assist individual updatings
            SqlConnection conn2 = new SqlConnection(ATMSconnectionString);
            conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try

                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            string terminalid_2 = (string)rdr["terminalid_2"];
                            string RRNumber_2 = (string)rdr["RRNumber_2"];

                          
                            SeqNo_2 = (int)rdr["SeqNo_2"];
                            SeqNo_4 = (int)rdr["SeqNo_4"];

                            Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_2, InReconcCycleNo, "Reversals", conn2);
                            Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_4, InReconcCycleNo, "Reversals", conn2);

                            Counter = Counter + 2; // two instead of one 

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    var stats = conn.RetrieveStatistics();
                    commandExecutionTimeInMs = (long)stats["ExecutionTime"];



                    conn.StatisticsEnabled = false;
                    conn.Close();
                    conn2.Close();


                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    conn2.Close();


                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_IST";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update IST as reversed..Rec:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            
        }
        // // HANDLE REVERSALS TWIN
        public void HandleReversals_IST_TWIN(string InTable_DB_Name, string InOriginFileName, int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string SQLCmd;
            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // FOR IST
            int Counter;
            // FOR TESTING   
            string FileId = InOriginFileName + "_TWIN";

            // INSERT In REVERSALS GENERAL 
            SQLCmd = "INSERT INTO "
                           + "[RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                           + "( "
                            + " [FileId] "
                             + " ,[RMCycleNo] "
                             + " ,[MatchingCateg] "
                            + " ,[TerminalId_4] "
                              + " ,[TerminalId_2] "
                             + " ,[CardNumber_4] "
                               + " ,[CardNumber_2] "

                                 + " ,[AccNo_4] "
                               + " ,[AccNo_2] "
                                 + " ,[TransDescr_4] "
                               + " ,[TransDescr_2] "
                                 + " ,[TransCurr_4] "
                               + " ,[TransCurr_2] "

                            + " ,[TransAmt_4] "
                             + " ,[TransAmt_2] "
                              + " ,[TraceNo_4] "
                             + " ,[TraceNo_2] "
                               + " ,[RRNumber_4] "
                              + " ,[RRNumber_2] "
                            + " ,[FullTraceno_4] "
                              + " ,[FullTraceno_2] "
                            + " ,[SeqNo_4] "
                            + " ,[SeqNo_2] "
                            + ",[ResponseCode_4] "
                             + ",[ResponseCode_2] "
                            + ",[TransDate_4] "
                            + ",[TransDate_2] "
                            + ",[TXNSRC_4] "
                             + ",[TXNSRC_2] "
                            + ",[TXNDEST_4] "
                              + ",[TXNDEST_2] "
                            + ") "
          + " SELECT "
          + " @FileId "
           + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "

          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
          + ",A.TraceNo As TraceNo_4 ,B.TraceNo As TraceNo_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] B "
          + " ON "
          //    + " A.TerminalId = B.TerminalId "
          + "  A.CardNumber = B.CardNumber "
          //+ " AND A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.TransDate = B.TransDate "
       + " WHERE(A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4') "
            + " AND( B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1'))  ) "
            + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();


                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR .." + FileId + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_IST_TWIN_1..Records:.." + "\r\n";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals IST TWIN first level.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //
            // INSERT In REVERSALS Conditional for Visa
            //
            SQLCmd = "INSERT INTO "
                           + "[RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                           + "( "
                            + " [FileId] "
                            + " ,[RMCycleNo] "
                             + " ,[MatchingCateg] "
                            + " ,[TerminalId_4] "
                              + " ,[TerminalId_2] "
                             + " ,[CardNumber_4] "
                               + " ,[CardNumber_2]"

                                 + " ,[AccNo_4] "
                               + " ,[AccNo_2] "
                                 + " ,[TransDescr_4] "
                               + " ,[TransDescr_2] "
                                 + " ,[TransCurr_4] "
                               + " ,[TransCurr_2] "

                            + " ,[TransAmt_4] "
                             + " ,[TransAmt_2] "
                              + " ,[TraceNo_4] "
                             + " ,[TraceNo_2] "
                               + " ,[RRNumber_4] "
                              + " ,[RRNumber_2] "
                            + " ,[FullTraceno_4] "
                              + " ,[FullTraceno_2] "
                            + " ,[SeqNo_4] "
                            + " ,[SeqNo_2] "
                            + ",[ResponseCode_4] "
                             + ",[ResponseCode_2] "
                            + ",[TransDate_4] "
                            + ",[TransDate_2] "
                            + ",[TXNSRC_4] "
                             + ",[TXNSRC_2] "
                            + ",[TXNDEST_4] "
                              + ",[TXNDEST_2] "
                            + ") "
          + " SELECT "
          + " @FileId "
           + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
          + ",A.TraceNo As TraceNo_4 ,B.TraceNo As TraceNo_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          //+ " AND A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE ( A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' )  " // leave as it is 
          + " AND  ( B.Processed = 0 AND LEFT(B.FullTraceNo, 1) = '2' )  "
          + " AND((A.TXNSRC = 1 AND A.TXNDEST = 4) AND(B.TXNSRC = 1 AND B.TXNDEST = 4)) "
            + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR VISA From TWIN.." + FileId + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_IST_TWIN_2";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }
            stpErrorText += DateTime.Now + "_" + "Insert Reversals IST TWIN for Visa.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            Counter = 0;

            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]";
            int SeqNo_2;
            int SeqNo_4;

            SqlString = "SELECT * "
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                  + " WHERE RMCycleNo = @RMCycleNo and FileId = @FileId "
                  + "";

            // OPEN Connection to assist individual updatings
            SqlConnection conn2 = new SqlConnection(ATMSconnectionString);
            conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try

                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            string terminalid_2 = (string)rdr["terminalid_2"];
                            string RRNumber_2 = (string)rdr["RRNumber_2"];

                            if (terminalid_2 == "01901009" & RRNumber_2 == "124616171268")
                            {
                                RRNumber_2 = RRNumber_2; // testing
                            }

                            SeqNo_2 = (int)rdr["SeqNo_2"];
                            SeqNo_4 = (int)rdr["SeqNo_4"];


                            Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_2, InReconcCycleNo, "Reversals", conn2);
                            Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_4, InReconcCycleNo, "Reversals", conn2);

                            Counter = Counter + 2; // two instead of one 

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    var stats = conn.RetrieveStatistics();
                    commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    conn.StatisticsEnabled = false;
                    conn.Close();
                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    conn2.Close();

                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_IST_TWIN";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update IST_TWIN as reversed..Rec:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
        }
        //
        // HANDLE DAILY STATISTICS FOR ATMS
        //
        public void HandleDailyStatisticsForAtms(string InTable_DB_Name, string InOriginFileName, int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();

            string SQLCmd;

            TableATMsDailyStats = new DataTable();
            TableATMsDailyStats.Clear();

            SqlString =
             " SELECT [Net_TransDate] ,[TransType] ,[TerminalId] ,Sum([TransAmt]) AS Amount , count(*) as Number "
             + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
             + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle AND TXNSRC = '1' "
             + " AND TransType in (11, 23) and Processed = 0 AND ResponseCode = '0' "
             + " group by [Net_TransDate],[TransType],[TerminalId] "
             + "   order by TerminalId, TransType ";


            using (SqlConnection conn =
             new SqlConnection(recconConnString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        sqlAdapt.Fill(TableATMsDailyStats);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();

                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _ATMs Stats _1_:.." + "\r\n";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            try
            {

                int I = 0;

                while (I <= (TableATMsDailyStats.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    DateTime WNet_TransDate = (DateTime)TableATMsDailyStats.Rows[I]["Net_TransDate"];
                    int WTransType = (int)TableATMsDailyStats.Rows[I]["TransType"];
                    string WAtmNo = (string)TableATMsDailyStats.Rows[I]["TerminalId"];
                    decimal WAmount = (decimal)TableATMsDailyStats.Rows[I]["Amount"];
                    int WNumber = (int)TableATMsDailyStats.Rows[I]["Number"];

                    Ah.ReadTransHistory_Dispensed_Deposited(WAtmNo, WNet_TransDate, InReconcCycleNo);
                    if (Ah.RecordFound == true)
                    {
                        if (WTransType == 11)
                        {
                            Ah.DrTransactions = Ah.DrTransactions + WNumber;
                            Ah.DispensedAmt = Ah.DispensedAmt + WAmount;
                        }

                        // Over 23 = Deposits 

                        if (WTransType == 23)
                        {
                            Ah.CrTransactions = Ah.CrTransactions + WNumber;
                            Ah.DepAmount = Ah.DepAmount + WAmount;
                        }

                        Ah.UpdateDailyStatsPerAtm(WAtmNo, WNet_TransDate);
                    }
                    else
                    {

                        Ah.AtmNo = WAtmNo;
                        Ah.BankId = WOperator;
                        Ah.Dt = WNet_TransDate;
                        Ah.LoadedAtRMCycle = InReconcCycleNo;

                        Ah.DrTransactions = 0;
                        Ah.DispensedAmt = 0;
                        Ah.CrTransactions = 0;
                        Ah.DepAmount = 0;

                        if (WTransType == 11)
                        {
                            Ah.DrTransactions = Ah.DrTransactions + WNumber;
                            Ah.DispensedAmt = Ah.DispensedAmt + WAmount;
                        }

                        if (WTransType == 23)
                        {
                            Ah.CrTransactions = Ah.CrTransactions + WNumber;
                            Ah.DepAmount = Ah.DepAmount + WAmount;

                        }

                        Ah.Operator = WOperator;

                        Ah.InsertTransHistory_With_Default(WAtmNo, WNet_TransDate);
                    }


                    I++; // Read Next entry of the table 
                }

            }
            catch (Exception ex)
            {
                ErrorFound = true;
                stpErrorText = stpErrorText + "Cancel At _ATMs Stats _2_:.." + "\r\n";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;

                CatchDetails(ex);
                return;

            }

        }

        // HANDLE DAILY STATISTICS FOR ATMS _ UNDO STATS
        public void HandleDailyStatisticsForAtms_UNDO(int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            // DELETE STATS

            string SQLCmd = "DELETE FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
                     + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During DELETE STATs";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

        }

        // HandleSM_Bank_Records
        public void HandleSM_Bank_Records_UNDO(int InRMCycle, DateTime InCut_Off_Date)
        {

            ErrorFound = false;
            ErrorOutput = "";

            // DELETE BOTH BANK ENTRIES AND POSTED 
            // DELETE AUDI RECORDS TOO
            // 

            string SQLCmd = " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_Bank_Repl_Entries] "
                     + " WHERE LoadedAtRMCycle = @RMCycle "
                      + " DELETE FROM [ATMS].[dbo].[PostedTrans] "
                          + " WHERE RMCycle  = @RMCycle "
                      + " DELETE FROM [ATMS].[dbo].[Cit_ExcelProcessCycles] "
                          + " WHERE RMCycle  = @RMCycle "
                      + " DELETE FROM [ATMS].[dbo].[SessionsDataCombined] "
                          + " WHERE RMCycle  = @RMCycle "
                     ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During DELETE CIT Record";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

        }

        //*******************
        //// CREATE CIT_Speed_Excel
        /////****************
        /// 1. Convert excel to tap
        /// 2. Call stp from pambos to create records in File
        /// 3. Read the file and create the final one 
        private DataTable DataTableExcelEntries = new DataTable();

        public void InsertRecords_GTL_CIT_Speed_Excel(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            decimal W_SM_Loaded = 0;
            decimal W_SM_Unloaded_Cassette = 0;
            decimal W_SM_Unloaded_Deposits = 0;
            decimal W_PresentedErrors = 0;

            int Counter = 0;

            string SQLCmd;


            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table CIT_Speed_Excel FOR BULK

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;  // seconds
                        cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During CIT_Speed_Excel BULK Truncate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=1500 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\r\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During CIT_Speed_Excel Bulk Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert for CIT_Speed_Excel finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // Find Parametes 

            int K = 0;

            string ExcelDate = "";

            string s_Load_FaceValue_1 = "20";
            string s_Load_FaceValue_2 = "50";
            string s_Load_FaceValue_3 = "100";
            string s_Load_FaceValue_4 = "200";

            string s_Un_Load_FaceValue_1 = "20";
            string s_Un_Load_FaceValue_2 = "50";
            string s_Un_Load_FaceValue_3 = "100";
            string s_Un_Load_FaceValue_4 = "200";

            string s_Dep_FaceValue_1 = "20";
            string s_Dep_FaceValue_2 = "50";
            string s_Dep_FaceValue_3 = "100";
            string s_Dep_FaceValue_4 = "200";

            // Convert(date, '01-02-14', 3)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //
            // GET FROM EXCEL THE NEEDED INFORMATION
            //

            string SqlString = "SELECT TOP (10)  * "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;
            using (SqlConnection conn =
                          new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@Operator", InOperator);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            K = K + 1;
                            if (K == 1)
                            {
                                ExcelDate = (string)rdr["Bulk_Deposits_Notes_Denom_1"];
                            }
                            if (K == 10)
                            {

                                s_Load_FaceValue_1 = (string)rdr["Bulk_Load_Cassette_1"];
                                s_Load_FaceValue_2 = (string)rdr["Bulk_Load_Cassette_2"];
                                s_Load_FaceValue_3 = (string)rdr["Bulk_Load_Cassette_3"];
                                s_Load_FaceValue_4 = (string)rdr["Bulk_Load_Cassette_4"];

                                s_Un_Load_FaceValue_1 = (string)rdr["Bulk_Un_Load_Cassette_1"];
                                s_Un_Load_FaceValue_2 = (string)rdr["Bulk_Un_Load_Cassette_2"];
                                s_Un_Load_FaceValue_3 = (string)rdr["Bulk_Un_Load_Cassette_3"];
                                s_Un_Load_FaceValue_4 = (string)rdr["Bulk_Un_Load_Cassette_4"];

                                s_Dep_FaceValue_1 = (string)rdr["Bulk_Deposits_Notes_Denom_1"];
                                s_Dep_FaceValue_2 = (string)rdr["Bulk_Deposits_Notes_Denom_2"];
                                s_Dep_FaceValue_3 = (string)rdr["Bulk_Deposits_Notes_Denom_3"];
                                s_Dep_FaceValue_4 = (string)rdr["Bulk_Deposits_Notes_Denom_4"];


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

                    stpErrorText = stpErrorText + "Cancel During CIT_Speed_Excel FIND Parameters ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            if (s_Load_FaceValue_1 == "20" & s_Load_FaceValue_2 == "50" & s_Load_FaceValue_3 == "100" & s_Load_FaceValue_4 == "200")
            {
                // THIS IS CORRECT
            }
            else
            {
                MessageBox.Show("Something wrong with excel. Format has been changed!" + Environment.NewLine
                         + "Denominations format has been changed"
                    );
                stpReturnCode = -1;
                return;
            }


            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //       
            // CREATE CIT GL_Balances_Atms_Daily from BULK
            //
            //


            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[CIT_Speed_Excel]  "
                + "( "
                    + "  [CITId] "
                        + "  ,[OriginFile] "
                          + ",[ExcelDate] "
                         + " ,[AtmNo] "
                         + " ,[AtmName] "
                        + "  ,[ReplDateG4S] "
                        //  + " ,[OrderNo] "
                        + "  ,[CreatedDate] "
                        //  + " ,[OverFound] "
                        // + "  ,[ShortFound] "
                        //   + " ,[PresentedErrors] "
                        //   + " ,[RemarksG4S] "
                        //  + "  ,[RemarksRRDM] "
                        //   + " ,[ProcessMode] "
                        //    + " ,[ProcessedAtReplCycleNo] "
                        //   + "  ,[Mask]  "
                        //    + ",[Gl_Balance_At_CutOff] "
                        + "  ,[Ccy] "
                         + " ,[Load_FaceValue_1] "
                        + "  ,[Load_Cassette_1] "
                        + "  ,[Load_FaceValue_2] "
                         + " ,[Load_Cassette_2] "
                        + "  ,[Load_FaceValue_3] "
                         + " ,[Load_Cassette_3] "
                        + "  ,[Load_FaceValue_4] "
                         + " ,[Load_Cassette_4] "
                        + "  ,[Cash_Loaded] "
      + ",[Un_Load_FaceValue_1] "
     + " ,[Un_Load_Cassette_1]  "
     + " ,[Un_Load_FaceValue_2] "
     + " ,[Un_Load_Cassette_2] "
    + "  ,[Un_Load_FaceValue_3] "
    + "  ,[Un_Load_Cassette_3] "
    + "  ,[Un_Load_FaceValue_4] "
    + "  ,[Un_Load_Cassette_4] "
    + "  ,[UnloadedCounted] "
     + " ,[Dep_FaceValue_1] "
     + " ,[Deposits_Notes_Denom_1] "
    + "  ,[Dep_FaceValue_2] "
    + "  ,[Deposits_Notes_Denom_2] "
    + "  ,[Dep_FaceValue_3] "
    + "  ,[Deposits_Notes_Denom_3] "
    + "  ,[Dep_FaceValue_4] "
   + "   ,[Deposits_Notes_Denom_4] "
    + "  ,[DepositsCounted] "
    + "  ,[Journal] "
     // + " ,[Processed] "
     + " ,[LoadedAtRMCycle] "
    + "  ,[Operator] "

                 + ") "
                 + " SELECT "
                      + "  @CITId "
                        + "  ,@OriginFile "
                          //+ ", Convert(date, @ExcelDate , 3) "
                          + " ,CAST((SUBSTRING(@ExcelDate, 7, 4) + SUBSTRING(@ExcelDate, 4, 2) + SUBSTRING(@ExcelDate, 1, 2)) as Date) "
                             //  + " ,isnull(Bulk_AtmNo,'') "
                             + ",case "
                            + " WHEN LEN(isnull(Bulk_AtmNo,''))=3 THEN '01900'+ Bulk_AtmNo "
                            + " WHEN LEN(isnull(Bulk_AtmNo,''))=4 THEN '0190'+ Bulk_AtmNo "
                            + " else 'No_AtmNo' "
                            + "end "
                         + " ,isnull(Bulk_AtmName,'') "

                         // 29/08/2021
                         + " ,CAST((SUBSTRING(Bulk_Repl_Date, 7, 4) + SUBSTRING(Bulk_Repl_Date, 4, 2) + SUBSTRING(Bulk_Repl_Date, 1, 2)) as Date) "
                         + "  ,getdate() " // ,[CreatedDate]
                        + "  ,@Ccy "
                        + " ,@Load_FaceValue_1 "
                        + " , isnull(Bulk_Load_Cassette_1,0) "
                        + " ,@Load_FaceValue_2 "
                        + " , isnull(Bulk_Load_Cassette_2,0) "
                        + " ,@Load_FaceValue_3 "
                        + " , isnull(Bulk_Load_Cassette_3,0) "
                        + " ,@Load_FaceValue_4 "
                        + " , isnull(Bulk_Load_Cassette_4,0) "
                        + "  , isnull(cast(replace( replace(replace(Bulk_Cash_Loaded,'\"','') ,',',''),'.','')as float),0) "

                        + ",@Un_Load_FaceValue_1 "
                        + " ,isnull(Bulk_Un_Load_Cassette_1,0) "
                        + ",@Un_Load_FaceValue_2 "
                        + " ,isnull(Bulk_Un_Load_Cassette_2,0) "
                        + ",@Un_Load_FaceValue_3 "
                        + " ,isnull(Bulk_Un_Load_Cassette_3,0) "
                        + ",@Un_Load_FaceValue_4 "
                        + " ,isnull(Bulk_Un_Load_Cassette_4,0) "
                        + "  , isnull(cast(replace( replace(replace(Bulk_UnloadedCounted,'\"','') ,',',''),'.','')as float),0) "

                          + " ,@Dep_FaceValue_1 "
                          + " ,isnull(Bulk_Deposits_Notes_Denom_1,0) "
                          + " ,@Dep_FaceValue_2 "
                          + " ,isnull(Bulk_Deposits_Notes_Denom_2,0) "
                          + " ,@Dep_FaceValue_3 "
                          + " ,isnull(Bulk_Deposits_Notes_Denom_3,0) "
                          + " ,@Dep_FaceValue_4 "
                          + " ,isnull(Bulk_Deposits_Notes_Denom_4,0) "
                           + "  , isnull(cast(replace( replace(replace(Bulk_DepositsCounted,'\"','') ,',',''),'.','')as float),0) "

                            + "  ,isnull(Bulk_Journal,0) "

                          + " ,@LoadedAtRMCycle "
                         + "  ,@Operator "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE bulk_First_column = 'print' AND isnull(Bulk_AtmNo,'') <> ''  "
                + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@CITId", "2000"); // Speed
                        cmd.Parameters.AddWithValue("@OriginFile", WFullPath_01.Trim());
                        cmd.Parameters.AddWithValue("@ExcelDate", ExcelDate);
                        cmd.Parameters.AddWithValue("@Ccy", "818");

                        cmd.Parameters.AddWithValue("@Load_FaceValue_1", s_Load_FaceValue_1); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Load_FaceValue_2", s_Load_FaceValue_2); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Load_FaceValue_3", s_Load_FaceValue_3); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Load_FaceValue_4", s_Load_FaceValue_4); // 20, 50, 100, 200


                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_1", s_Un_Load_FaceValue_1); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_2", s_Un_Load_FaceValue_2); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_3", s_Un_Load_FaceValue_3); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Un_Load_FaceValue_4", s_Un_Load_FaceValue_4); // 20, 50, 100, 200


                        cmd.Parameters.AddWithValue("@Dep_FaceValue_1", s_Dep_FaceValue_1); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Dep_FaceValue_2", s_Dep_FaceValue_2); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Dep_FaceValue_3", s_Dep_FaceValue_3); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@Dep_FaceValue_4", s_Dep_FaceValue_4); // 20, 50, 100, 200
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        cmd.CommandTimeout = 350;  // seconds
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For CIT_Speed_Excel " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During CIT_Speed_Excel ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            DataTableExcelEntries = new DataTable();
            DataTableExcelEntries.Clear();


            // GET each line and check if already exist 

            RRDM_CIT_G4S_And_Bank_Repl_Entries G4 = new RRDM_CIT_G4S_And_Bank_Repl_Entries();


            // 
            //// DATA TABLE ROWS DEFINITION
            /// READ THE INDIVITUAL ENTRIES one by one and decide whether to insert or update the final table
            /// READ CIT_Speed_Excel just loaded 
            /// Insert or update G4 table [RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_G4S_Repl_Entries] 
            /// 
            string WSelectionCriteria = " WHERE OriginFile='" + WFullPath_01.Trim() + "'";

            SqlString = "SELECT * "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_Speed_Excel]"
               + WSelectionCriteria
               + " ORDER BY AtmNo "
               ;

            using (SqlConnection conn =
             new SqlConnection(recconConnString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.Fill(DataTableExcelEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            decimal WLoadedTotal = 0;
            decimal WUnloadedCassTotal = 0;
            decimal WUnloadedDepTotal = 0;

            int WValidInExcelRecords = 0;

            // ReplDate = 29/08/2023
            string DD = ExcelDate.Substring(0, 2);
            string MM = ExcelDate.Substring(3, 2);
            string YYYY = ExcelDate.Substring(6, 4);

            string ExcelDate_2 = YYYY + MM + DD;

            var newDate = DateTime.ParseExact(ExcelDate_2,
                                  "yyyyMMdd",
                                   CultureInfo.InvariantCulture);

            string WCitId = "2000";

            int WMode = 1;


            int L = 0;

            while (L <= (DataTableExcelEntries.Rows.Count - 1))
            {

                //RecordFound = true;

                int SeqNo = (int)DataTableExcelEntries.Rows[L]["SeqNo"];
                string CITId = (string)DataTableExcelEntries.Rows[L]["CITId"];
                string OriginFile = (string)DataTableExcelEntries.Rows[L]["OriginFile"];

                DateTime WExcelDate = (DateTime)DataTableExcelEntries.Rows[L]["ExcelDate"];

                string AtmNo = (string)DataTableExcelEntries.Rows[L]["AtmNo"];
                string AtmName = (string)DataTableExcelEntries.Rows[L]["AtmName"];

                DateTime ReplDateG4S = (DateTime)DataTableExcelEntries.Rows[L]["ReplDateG4S"];

                DateTime CreatedDate = (DateTime)DataTableExcelEntries.Rows[L]["CreatedDate"];

                string Ccy = (string)DataTableExcelEntries.Rows[L]["Ccy"];
                int Load_FaceValue_1 = (int)DataTableExcelEntries.Rows[L]["Load_FaceValue_1"];
                int Load_Cassette_1 = (int)DataTableExcelEntries.Rows[L]["Load_Cassette_1"];

                int Load_FaceValue_2 = (int)DataTableExcelEntries.Rows[L]["Load_FaceValue_2"];
                int Load_Cassette_2 = (int)DataTableExcelEntries.Rows[L]["Load_Cassette_2"];

                int Load_FaceValue_3 = (int)DataTableExcelEntries.Rows[L]["Load_FaceValue_3"];
                int Load_Cassette_3 = (int)DataTableExcelEntries.Rows[L]["Load_Cassette_3"];

                int Load_FaceValue_4 = (int)DataTableExcelEntries.Rows[L]["Load_FaceValue_4"];
                int Load_Cassette_4 = (int)DataTableExcelEntries.Rows[L]["Load_Cassette_4"];

                decimal Cash_Loaded = (decimal)DataTableExcelEntries.Rows[L]["Cash_Loaded"];

                int Un_Load_FaceValue_1 = (int)DataTableExcelEntries.Rows[L]["Un_Load_FaceValue_1"];
                int Un_Load_Cassette_1 = (int)DataTableExcelEntries.Rows[L]["Un_Load_Cassette_1"];

                int Un_Load_FaceValue_2 = (int)DataTableExcelEntries.Rows[L]["Un_Load_FaceValue_2"];
                int Un_Load_Cassette_2 = (int)DataTableExcelEntries.Rows[L]["Un_Load_Cassette_2"];

                int Un_Load_FaceValue_3 = (int)DataTableExcelEntries.Rows[L]["Un_Load_FaceValue_3"];
                int Un_Load_Cassette_3 = (int)DataTableExcelEntries.Rows[L]["Un_Load_Cassette_3"];

                int Un_Load_FaceValue_4 = (int)DataTableExcelEntries.Rows[L]["Un_Load_FaceValue_4"];
                int Un_Load_Cassette_4 = (int)DataTableExcelEntries.Rows[L]["Un_Load_Cassette_4"];

                decimal UnloadedCounted = (decimal)DataTableExcelEntries.Rows[L]["UnloadedCounted"];

                int Dep_FaceValue_1 = (int)DataTableExcelEntries.Rows[L]["Dep_FaceValue_1"];
                int Deposits_Notes_Denom_1 = (int)DataTableExcelEntries.Rows[L]["Deposits_Notes_Denom_1"];

                int Dep_FaceValue_2 = (int)DataTableExcelEntries.Rows[L]["Dep_FaceValue_2"];
                int Deposits_Notes_Denom_2 = (int)DataTableExcelEntries.Rows[L]["Deposits_Notes_Denom_2"];

                int Dep_FaceValue_3 = (int)DataTableExcelEntries.Rows[L]["Dep_FaceValue_3"];
                int Deposits_Notes_Denom_3 = (int)DataTableExcelEntries.Rows[L]["Deposits_Notes_Denom_3"];

                int Dep_FaceValue_4 = (int)DataTableExcelEntries.Rows[L]["Dep_FaceValue_4"];
                int Deposits_Notes_Denom_4 = (int)DataTableExcelEntries.Rows[L]["Deposits_Notes_Denom_4"];

                decimal DepositsCounted = (decimal)DataTableExcelEntries.Rows[L]["DepositsCounted"];

                bool Journal = (bool)DataTableExcelEntries.Rows[L]["Journal"];

                int LoadedAtRMCycle = (int)DataTableExcelEntries.Rows[L]["LoadedAtRMCycle"];

                string Operator = (string)DataTableExcelEntries.Rows[L]["Operator"];
                //
                // Sum up the totals 
                //
                WLoadedTotal = WLoadedTotal + Cash_Loaded;
                WUnloadedCassTotal = WUnloadedCassTotal + UnloadedCounted;
                WUnloadedDepTotal = WUnloadedDepTotal + DepositsCounted;

                WValidInExcelRecords = WValidInExcelRecords + 1;
                //
                // Get the record by ReplDate 
                // If there are two Replanishments at the same date then we have a problem WE SHOULD SOLVE
                //
                G4.ReadCIT_G4S_Repl_EntriesByATMandDate(AtmNo, ReplDateG4S, WMode);

                if (G4.RecordFound == true)
                {
                    // If found then Update or alert if mature 
                    if (Cash_Loaded > 0 & G4.Cash_Loaded > 0)
                    {
                        MessageBox.Show("Duplicate Entry For Cash_Loaded"+Environment.NewLine
                            + "Previous Loading.." + G4.Repl_Load_Excel_Date.ToShortDateString() + Environment.NewLine
                            + "ATM No.." + AtmNo + Environment.NewLine
                            + "ReplCycleDate" + ReplDateG4S.ToShortDateString()
                            );

                        L = L + 1;

                        continue; 

                    }

                    if (Cash_Loaded > 0)
                    {
                        // Fill in the loaded fields 

                        G4.Load_FaceValue_1 = Load_FaceValue_1;
                        G4.Load_Cassette_1 = Load_Cassette_1;

                        G4.Load_FaceValue_2 = Load_FaceValue_2;
                        G4.Load_Cassette_2 = Load_Cassette_2;

                        G4.Load_FaceValue_3 = Load_FaceValue_3;
                        G4.Load_Cassette_3 = Load_Cassette_3;

                        G4.Load_FaceValue_4 = Load_FaceValue_4;
                        G4.Load_Cassette_4 = Load_Cassette_4;

                        G4.Cash_Loaded = Cash_Loaded;

                        // Update statuses 
                        G4.Repl_Load_Excel_Date = newDate;
                        G4.Repl_Load_Status = 2;
                        G4.Repl_Load_Action = "No Action Yet";

                        G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);

                    }

                    if (UnloadedCounted > 0 & G4.UnloadedCounted > 0)
                    {
                        MessageBox.Show("Duplicate Entry For Cash_Loaded" + Environment.NewLine
                             + "Previous UnLoading.." + G4.Repl_UnLoad_Excel_Date.ToShortDateString() + Environment.NewLine
                            + "ATM No.." + AtmNo + Environment.NewLine
                            + "ReplCycleDate" + ReplDateG4S.ToShortDateString() + Environment.NewLine
                            + "Entry will be ignored"
                            );

                        L = L + 1;

                        continue;

                    }


                    if (UnloadedCounted > 0)
                    {
                        // Fill in the un loaded fields 


                        G4.Un_Load_FaceValue_1 = Un_Load_FaceValue_1;
                        G4.Un_Load_Cassette_1 = Un_Load_Cassette_1;

                        G4.Un_Load_FaceValue_2 = Un_Load_FaceValue_2;
                        G4.Un_Load_Cassette_2 = Un_Load_Cassette_2;

                        G4.Un_Load_FaceValue_3 = Un_Load_FaceValue_3;
                        G4.Un_Load_Cassette_3 = Un_Load_Cassette_3;

                        G4.Un_Load_FaceValue_4 = Un_Load_FaceValue_4;
                        G4.Un_Load_Cassette_4 = Un_Load_Cassette_4;

                        G4.UnloadedCounted = UnloadedCounted;

                        // deposits
                        G4.Dep_FaceValue_1 = Dep_FaceValue_1;
                        G4.Deposits_Notes_Denom_1 = Deposits_Notes_Denom_1;

                        G4.Dep_FaceValue_2 = Dep_FaceValue_2;
                        G4.Deposits_Notes_Denom_2 = Deposits_Notes_Denom_2;

                        G4.Dep_FaceValue_3 = Dep_FaceValue_3;
                        G4.Deposits_Notes_Denom_3 = Deposits_Notes_Denom_3;

                        G4.Dep_FaceValue_4 = Dep_FaceValue_4;
                        G4.Deposits_Notes_Denom_4 = Deposits_Notes_Denom_4;

                        G4.Deposits = DepositsCounted;

                        // Update statuses 
                        G4.Repl_UnLoad_Excel_Date = newDate;
                        G4.Repl_UnLoad_Status = 2;
                        G4.Repl_UnLoad_Action = "No Action Yet";

                        G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);

                    }

                    G4.UpdateCIT_G4S_Repl_EntriesRecordDuringExcelLoading(G4.SeqNo, WMode);

                }
                else
                {
                    // If not found then insert 
                    // Either for loading or unloading 
                    RecordFound = false;
                    ErrorFound = false;
                    ErrorOutput = "";
                    //Intbl_CIT_G4S_Repl_Entries
                    SQLCmd = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_G4S_Repl_Entries]  "
                       + "( "
                           + "  [CITId] "
                               + "  ,[OriginFileName] "
                                 + ",[ExcelDate] "
                                + " ,[AtmNo] "
                                + " ,[AtmName] "
                                + "  ,[ReplDateG4S] "
                               //  + " ,[OrderNo] "
                               + "  ,[CreatedDate] "
                               //+ "  ,LoadingExcelCycleNo "
                                //  + " ,[OverFound] "
                                // + "  ,[ShortFound] "
                                //   + " ,[PresentedErrors] "
                                //   + " ,[RemarksG4S] "
                                //  + "  ,[RemarksRRDM] "
                                //   + " ,[ProcessMode] "
                                //    + " ,[ProcessedAtReplCycleNo] "
                                //   + "  ,[Mask]  "
                                //    + ",[Gl_Balance_At_CutOff] "
                                // + "  ,[Ccy] "
                                + " ,[Load_FaceValue_1] "
                               + "  ,[Load_Cassette_1] "
                               + "  ,[Load_FaceValue_2] "
                                + " ,[Load_Cassette_2] "
                               + "  ,[Load_FaceValue_3] "
                                + " ,[Load_Cassette_3] "
                               + "  ,[Load_FaceValue_4] "
                                + " ,[Load_Cassette_4] "
                               + "  ,[Cash_Loaded] "
             + ",[Un_Load_FaceValue_1] "
            + " ,[Un_Load_Cassette_1]  "
            + " ,[Un_Load_FaceValue_2] "
            + " ,[Un_Load_Cassette_2] "
           + "  ,[Un_Load_FaceValue_3] "
           + "  ,[Un_Load_Cassette_3] "
           + "  ,[Un_Load_FaceValue_4] "
           + "  ,[Un_Load_Cassette_4] "

           + "  ,[UnloadedCounted] "

            + " ,[Dep_FaceValue_1] "
            + " ,[Deposits_Notes_Denom_1] "
           + "  ,[Dep_FaceValue_2] "
           + "  ,[Deposits_Notes_Denom_2] "
           + "  ,[Dep_FaceValue_3] "
           + "  ,[Deposits_Notes_Denom_3] "
           + "  ,[Dep_FaceValue_4] "
          + "   ,[Deposits_Notes_Denom_4] "

           + "  ,[Deposits] "
            //+ "  ,[Journal] "
            // + " ,[Processed] "
            + " ,[LoadedAtRMCycle] "
           + "  ,[Operator] "

                        + ") "
                        + " SELECT "
                             + "  CITId "
                               + "  ,OriginFile "
                                 + ", ExcelDate "
                                //  + " ,isnull(Bulk_AtmNo,'') "
                                + " ,[AtmNo] "
                                + " ,[AtmName] "
                                + "  ,[ReplDateG4S] "
                               + "  ,[CreatedDate] "
                               // + "  ,@LoadingExcelCycleNo "
                                // + "  ,@Ccy "
                                + " ,[Load_FaceValue_1] "
                               + "  ,[Load_Cassette_1] "
                               + "  ,[Load_FaceValue_2] "
                                + " ,[Load_Cassette_2] "
                               + "  ,[Load_FaceValue_3] "
                                + " ,[Load_Cassette_3] "
                               + "  ,[Load_FaceValue_4] "
                                + " ,[Load_Cassette_4] "
                               + "  ,[Cash_Loaded] "

             + ",[Un_Load_FaceValue_1] "
            + " ,[Un_Load_Cassette_1]  "
            + " ,[Un_Load_FaceValue_2] "
            + " ,[Un_Load_Cassette_2] "
           + "  ,[Un_Load_FaceValue_3] "
           + "  ,[Un_Load_Cassette_3] "
           + "  ,[Un_Load_FaceValue_4] "
           + "  ,[Un_Load_Cassette_4] "
           + "  ,[UnloadedCounted] "

            + " ,[Dep_FaceValue_1] "
            + " ,[Deposits_Notes_Denom_1] "
           + "  ,[Dep_FaceValue_2] "
           + "  ,[Deposits_Notes_Denom_2] "
           + "  ,[Dep_FaceValue_3] "
           + "  ,[Deposits_Notes_Denom_3] "
           + "  ,[Dep_FaceValue_4] "
           + "   ,[Deposits_Notes_Denom_4] "

           + "   ,[DepositsCounted] "

                                 + " ,LoadedAtRMCycle "
                                + "  ,Operator "

                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_Speed_Excel] "
                        + " WHERE SeqNo = " + SeqNo
                       + " ";

                    using (SqlConnection conn = new SqlConnection(recconConnString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SQLCmd, conn))
                            {
                                //cmd.Parameters.AddWithValue("@CITId", WCitId); // Speed
                                //cmd.Parameters.AddWithValue("@OriginFileName", WFullPath_01);
                                //cmd.Parameters.AddWithValue("@ExcelDate", ExcelDate);
                               // cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", WLoadingExcelCycleNo);

                                //// cmd.Parameters.AddWithValue("@Ccy", "818");

                                //cmd.Parameters.AddWithValue("@Load_FaceValue_1", s_Load_FaceValue_1); // 20, 50, 100, 200

                                cmd.Parameters.AddWithValue("@Operator", WOperator);

                                cmd.CommandTimeout = 350;  // seconds
                                stpLineCount = cmd.ExecuteNonQuery();
                            }
                            // Close conn
                            conn.Close();
                            if (Environment.UserInteractive & TEST == true)
                            {
                                System.Windows.Forms.MessageBox.Show("Records Inserted For CIT_Speed_Excel " + Environment.NewLine
                                         + "..:.." + stpLineCount.ToString());
                            }

                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            stpErrorText = stpErrorText + "Cancel During CIT_Speed_Excel 343";
                            stpReturnCode = -1;

                            stpReferenceCode = stpErrorText;
                            CatchDetails(ex);
                            return;
                        }
                    // 
                    // Statuses 
                    //
                    WMode = 1;
                    G4.ReadCIT_G4S_Repl_EntriesByATMandDate(AtmNo, ReplDateG4S, WMode);

                    if (Cash_Loaded > 0)

                    {
                        // Check against Banks Loaded
                        // Update statuses 
                        G4.Repl_Load_Excel_Date = newDate;
                        G4.Repl_Load_Status = 2;
                        G4.Repl_Load_Action = "No Action Yet";

                        G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);
                    }

                    WMode = 1;
                    G4.ReadCIT_G4S_Repl_EntriesByATMandDate(AtmNo, ReplDateG4S, WMode);

                    if (UnloadedCounted > 0)
                    {
                        // Update statuses 
                        G4.Repl_UnLoad_Excel_Date = newDate;
                        G4.Repl_UnLoad_Status = 2;
                        G4.Repl_UnLoad_Action = "No Action Yet";

                        G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);
                    }
                }

                L++;

            }

           

            // UPDATE REPLENISHEMENT CYCLE 
            // READ ALL  Records and based on the Repl Date find out from Ta the replenishemnet Cycle 
            // 
            // WHAT DO WE DO IF Journal Was loaded? 
            // At the stage of Loading we continue
            // At the time of MAKER we decide what to do
            // 
            int TempMode = 1;
            string SelectionCriteria = " WHERE (Repl_Load_Status = 2 OR Repl_UnLoad_Status = 2) ";
               // + "  (LoadingExcelCycleNo=" + WLoadingExcelCycleNo + " OR Cast(ReplDateG4s as Date) = '1900-01-01' ) ";
            string WSignedId = "Controller";
            G4.ReadCIT_G4S_Repl_EntriesToFillDataTable(WOperator, WSignedId, SelectionCriteria, TempMode);

            if (G4.DataTableG4SEntries.Rows.Count == 0)
            {
                MessageBox.Show("There are no records in excel to update");
               // return;
            }

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            //int WMode = 1; 

            int I = 0;

            while (I <= (G4.DataTableG4SEntries.Rows.Count - 1))
            {
                // GET ALL fields

                //    RecordFound = true;
                int WSeqNo = (int)G4.DataTableG4SEntries.Rows[I]["SeqNo"];
                string WAtmNo = (string)G4.DataTableG4SEntries.Rows[I]["AtmNo"];
                DateTime WReplDate = (DateTime)G4.DataTableG4SEntries.Rows[I]["ReplDateG4S"];

                // Initialise
                W_SM_Loaded = 0;
                W_SM_Unloaded_Cassette = 0;
                W_SM_Unloaded_Deposits = 0;
                W_PresentedErrors = 0;

                // FIND HERE THE REPL CYCLE and Update 

                // READ Ta and find replenishment cycle starting with the WReplDate
                //
                Ta.ReadSessionsStatusTracesToFindSesNoBasedDateEnd(WAtmNo , WReplDate);

                if (Ta.RecordFound == true)
                {
                    WMode = 1; // Cit excel 
                    G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode);
                    G4.ReplCycleNo = Ta.SesNo;
                    G4.RemarksG4S = "Journal Loaded";
                    G4.UpdateCIT_G4S_Repl_EntriesRecord(WSeqNo, WMode);
                }
                else
                {
                    // Decide 
                    // Journal not loaded 
                    G4.Repl_Load_Status = 2 ; // No action yet 
                    G4.RemarksG4S = "Journal Not Loaded Yet";
                    //G4.Repl_UnLoad_Status = 2 ; // No action yet 
                    //G4.Repl_UnLoad_Action = "Journal Not Loaded Yet";
                    WMode = 1; // Cit excel
                    G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(WSeqNo, WMode);

                    I = I + 1; 
                    continue; 
                }

                // Statuses 
                //

                WMode = 1; // Cit excel 
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode);

                if (G4.Cash_Loaded > 0)

                {
                    // Update statuses 

                    // Check against Banks 
                    Na.ReadSessionsNotesAndValues(WAtmNo, G4.ReplCycleNo, 2);
                    W_SM_Loaded = Na.Balances1.OpenBal;
                    
                    if (Na.Balances1.OpenBal == G4.Cash_Loaded)
                    {
                        G4.Repl_Load_Status = 4;
                    }
                    else
                    {
                        G4.Repl_Load_Status = 6;
                        G4.Repl_UnLoad_Action = "Loaded Not Equal To Journal";
                    }
                 
                    G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);
                }

                WMode = 1; // Cit excel 
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(WSeqNo, WMode);
                //if (WAtmNo == "01901009")
                //{
                //    WMode = 1; 
                //}

                if (G4.UnloadedCounted > 0 || G4.Deposits > 0)
                {
                    // Update statuses 
                    
                    Na.ReadSessionsNotesAndValues(WAtmNo, G4.ReplCycleNo, 2);
                    
                    W_SM_Unloaded_Cassette = Na.Balances1.MachineBal;
                    W_PresentedErrors = Na.Balances1.PresenterValue;

                    if (G4.UnloadedCounted > 0)
                    {
                        if ((Na.Balances1.MachineBal+ Na.Balances1.PresenterValue == G4.UnloadedCounted))
                        {
                            G4.Repl_UnLoad_Status = 4;
                        }
                        else
                        {
                            G4.Repl_UnLoad_Status = 6;
                            G4.Repl_UnLoad_Action = "To Be Moved To Workflow - Due to Unload";
                        }

                    }
                    decimal SM_Deposits = 0;

                    if (G4.Deposits > 0 & G4.Repl_Load_Status != 6)
                    {
                        // Find Deposits 
                        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();
                        
                        //RRDMGasParameters Gp = new RRDMGasParameters();
                        string ParId = "218";
                        string OccurId = "1";
                        Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                        string From_SM = Gp.OccuranceNm;

                        bool DepositsFound = false;
                        string WCcy = "EGP";

                        if (From_SM == "YES")
                        {
                            // Find the first fuid to avoid duplication created by two the same journals
                            //
                            SM.Read_SM_AND_Get_First_fuid(WAtmNo, G4.ReplCycleNo);

                            if (SM.RecordFound == true)
                            {
                                // Fuid found
                                DepositsFound = true;

                                // Get the totals from SM and not from Mpa            
                                // GET TABLE
                                SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, G4.ReplCycleNo, SM.Fuid);
                                //
                                //***********************
                                //
                                string SM_SelectionCriteria1 = " WHERE AtmNo ='" + WAtmNo + "' AND RRDM_ReplCycleNo =" + G4.ReplCycleNo
                                                              + " AND FlagValid = 'Y' AND AdditionalCash = 'N' "
                                                                 ;

                                SM.Read_SM_Record_Specific_By_Selection(SM_SelectionCriteria1, WAtmNo, G4.ReplCycleNo, 2);

                                if (SM.RecordFound == true)
                                {
                                    // Get other Totals 

                                    int WDEP_COUNTERFEIT = SM.DEP_COUNTERFEIT;
                                    int WDEP_SUSPECT = SM.DEP_SUSPECT;

                                }
                                int WRECYCLEDTotalNotes;
                                decimal WRECYCLEDTotalMoney;

                                //int WTotalNCR_DepositsDispensedNotes;
                                //decimal WTotalNCR_DepositsDispensedMoney;

                                // Read Table 
                                K = 0;
                                int M = 0;


                                while (M <= (SM.DataTable_SM_Deposits.Rows.Count - 1))
                                {
                                    // "  SELECT Currency As Ccy, SUM(Cassette) as TotalNotes, sum(Facevalue * CASSETTE) as TotalMoney "

                                    WCcy = (string)SM.DataTable_SM_Deposits.Rows[M]["Ccy"];

                                    if (WCcy.Trim() == "")
                                    {
                                        M = M + 1;
                                        continue;
                                    }
                                    int WTotalCassetteNotes = (int)SM.DataTable_SM_Deposits.Rows[M]["TotalCassetteNotes"];
                                    decimal WTotalCassetteMoney = (decimal)SM.DataTable_SM_Deposits.Rows[M]["TotalCassetteMoney"];

                                    int WRetractTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[M]["TotalRETRACTNotes"];
                                    decimal WRetractTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[M]["TotalRETRACTMoney"];

                                    WRECYCLEDTotalNotes = (int)SM.DataTable_SM_Deposits.Rows[M]["TotalRECYCLEDNotes"];
                                    WRECYCLEDTotalMoney = (decimal)SM.DataTable_SM_Deposits.Rows[M]["TotalRECYCLEDMoney"];

                                    //WTotalNCR_DepositsDispensedNotes = (int)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedNotes"];
                                    //WTotalNCR_DepositsDispensedMoney = (decimal)SM.DataTable_SM_Deposits.Rows[I]["TotalNCR_DepositsDispensedMoney"];

                                    K = K + 1;

                                    if (K == 1)
                                    {
                                        // TOTAL REMAIN IN CASSETTES 
                                        // THIS IS THE FORMULA !!!!!!!!!!!!!!!!!!!
                                        // (WTotalMoneyDecimal+WRECYCLEDTotalMoney) -  WTotalNCR_DepositsDispensedMoney
                                        SM_Deposits  = WTotalCassetteMoney + WRECYCLEDTotalMoney;
                                        W_SM_Unloaded_Deposits = SM_Deposits;
                                        

                                    }
                                    M = M + 1;
                                }

                            }

                        }
                        else
                        {
                           // MessageBox.Show("No Deposits Found");
                            // return;
                        }

                        if (SM_Deposits == G4.Deposits )
                        {
                            if (G4.Repl_UnLoad_Status == 6)
                            {
                                // do nothing 
                            }
                            else
                            {
                                G4.Repl_UnLoad_Status = 4;
                            }
                        }
                        else
                        {
                            if (G4.Repl_UnLoad_Status == 6)
                            {
                                G4.Repl_UnLoad_Action = "To Be Moved To Workflow-Due to Unload and Deposits";
                            }
                            else
                            {
                                G4.Repl_UnLoad_Status = 6;
                                G4.Repl_UnLoad_Action = "To Be Moved To Workflow-Due to Deposits";
                            }
                            
                        }

                    }

                   
                }

                G4.UpdateCIT_G4S_Repl_EntriesRecordWithTwoStages(G4.SeqNo, WMode);

                // UPDATE IN CIT SM FIELDS
                WMode = 1; // Cit excel 
                G4.ReadCIT_G4S_Repl_EntriesBySeqNo(G4.SeqNo, WMode);

                G4.SM_Loaded = W_SM_Loaded;
                G4.SM_Unloaded_Cassette = W_SM_Unloaded_Cassette;
                G4.SM_Unloaded_Deposits = W_SM_Unloaded_Deposits;
                G4.PresentedErrors = W_PresentedErrors;

                G4.UpdateCIT_G4S_Repl_EntriesFrom_SM(G4.SeqNo, WMode);


                I++; // Read Next entry of the table 

            }

            stpErrorText += DateTime.Now + "_" + "Insert in CIT_Speed_Excel with records.." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            stpErrorText += DateTime.Now + "_" + "CIT_Speed_Excel FINISHES with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }


        //*******************
        //// CREATE GL_Balances_Atms_Daily
        /////****************
        public void InsertRecords_GTL_GL_Balances_Atms_Daily(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;


            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table FAWRY FOR BULK

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;  // seconds
                        cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During GL BULK Truncate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\r\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During GL Bulk Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert for GL finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            RecordFound = false;

            //       
            // CREATE GL_Balances_Atms_Daily from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[GL_Balances_Atms_Daily]  "
                + "( "
                 + " [OriginFileName] "

                 + " ,[LoadedAtRMCycle] "
                  + " ,[Cut_Off_Date] "
                 + " ,[MatchingCateg] "
                 + " ,[AtmNo] "
                 + " ,[Ccy_Cash] "
                 + ",[GL_Acc_ATM_Cash] "
                 + ", [GL_Bal_ATM_Cash]  "
                 + " ,[Ccy_Inter] "
                 + ",[GL_Acc_ATM_Inter] "
                 + ", [GL_Bal_ATM_Inter]  "
                 + " ,[Ccy_Excess] "
                 + ",[GL_Acc_ATM_Excess] "
                 + ", [GL_Bal_ATM_Excess]  "
                 + " ,[Ccy_Short] "
                 + ",[GL_Acc_ATM_Short] "
                 + ", [GL_Bal_ATM_Short]  "
                 + " , DateCreated"
                 + ", [Operator]  "
                 //+ ", [DateCreated] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + ", @LoadedAtRMCycle"

                 + " , Cast(Cut_Off_Date as DATE) "
                 + " , '' " // Matching Category 
                 + ", AtmNo " // ATM

                 + ",Ccy_Cash " // Ccy
                 + ", GL_Acc_ATM_Cash " // Account
                 + ",CAST([GL_Bal_ATM_Cash] As decimal(18,2))"

                 + ",Ccy_Inter " // Ccy
                 + ", GL_Acc_ATM_Inter " // Account
                 + ",CAST([GL_Bal_ATM_Inter] As decimal(18,2))"

                 + ",Ccy_Excess " // Ccy
                 + ", GL_Acc_ATM_Excess " // Account
                 + ",CAST([GL_Bal_ATM_Excess] As decimal(18,2))"

                 + ",Ccy_Short " // Ccy
                 + ", GL_Acc_ATM_Short " // Account
                 + ",CAST([GL_Bal_ATM_Short] As decimal(18,2))"
                 + ", @DateCreated "
                 + ", @Operator "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // + " WHERE  "
                //+ "  "

                + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For GL_Balances_Atms_Daily" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During GL_Balances_Atms_Daily";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in GL_Balances_Atms_Daily with records.." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            stpErrorText += DateTime.Now + "_" + "GL_Balances_Atms_Daily FINISHES with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }


     
       
        //*******************
        //// CREATE GENERAL AUTO FILE
        /////****************
        public void InsertRecords_GENERAL_AUTO_FILE(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_NCR_FOREX_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[NCR_FOREX] 

            stpErrorText = DateTime.Now + "_" + "Start_Loading_" + InOriginFileName + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);
            string WTableStructureId = Mf.TableStructureId;
            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            //TRY TO LOAD AUTO 
            // CALL CALL KONTO TO LOAD AND CREATE
            // .........

            // CALL METHOD TO DO MAPPING AND CREATE NEXT INSTRUCTION 
            RRDMMappingFileFieldsFromBankToRRDM Mfr = new RRDMMappingFileFieldsFromBankToRRDM();
            RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

            // Leave the below here in case it was not created
            // VALIDATE THAT GIVEN TEXT IS THE SAME AS Prior 
            // YOU DEFINE ONLY THE TABLES IN SQL
            La.CreateBulk_And_STD_RRDM_Tables(WFullPath_01, InOriginFileName, Mf.Delimiter);

            if (La.ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "Not Valid Input Text File.." + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                //  CatchDetails(ex);
                return;
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "Valid Text File.." + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }
            //
            // Load BULK from Delimiter file
            //
            La.LoadBulk_First_Table(WFullPath_01, InOriginFileName, Mf.Delimiter);

            if (La.ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "Error With Loading Text File.." + "\r\n" + La.ErrorOutput + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                //  CatchDetails(ex);
                return;
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "Text File Loaded.Records.." + La.Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }
            //
            // Move Just loaded to BULK ALL
            //
            La.MoveBULK_From_First_To_ALL(InOriginFileName, Mf.Delimiter, InReconcCycleNo);

            if (La.ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "Error With Moving Records to Bulk All.." + "\r\n" + La.ErrorOutput + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                //  CatchDetails(ex);
                return;
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "Records had moved to Bulk ALL.Records." + La.Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }

            // RRDM STANDARD TABLE 
            // Load from BULK TO RRDM STD table COMMAND And Creation 
            string BulkTable = "BULK_" + InOriginFileName;
            Mfr.ReadTableFieldsBySourceFile_Reformat_AND_CREATE_COMMAND(BulkTable, InOriginFileName, InReconcCycleNo);

            if (Mfr.ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "Error While Creating the command for Inserting Records to RRDM.." + "\r\n"
                                                                                          + Mfr.ErrorOutput + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                //  CatchDetails(ex);
                return;
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "Records had moved to Bulk ALL.Records." + La.Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }

            //
            // Assign created command
            //
            SQLCmd = Mfr.FullCreatedSqlCommand_ITMX;

            // INSERT IN FILE 
            La.Insert_Records_RRDM_STD_Table(WFileSeqNo, Origin, WTerminalType, WOperator, InReconcCycleNo, SQLCmd);

            if (La.ErrorFound == true)
            {
                stpErrorText += DateTime.Now + "_" + "Error With INSERT In RRDM STD TABLE.." + "\r\n" + La.ErrorOutput + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                //  CatchDetails(ex);
                return;
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "Records had been insert in RRDM Table." + La.Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }
            //
            // Create Reversals 
            //
            if (WTableStructureId == "Atms And Cards")
            {
                La.Create_RRDM_STD_Table_Reversals(InOriginFileName, InReconcCycleNo);

                if (La.ErrorFound == true)
                {
                    stpErrorText += DateTime.Now + "_" + "Error During creation of reversals.." + "\r\n" + La.ErrorOutput + "\r\n";
                    Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    //  CatchDetails(ex);
                    return;
                }
                else
                {
                    stpErrorText += DateTime.Now + "_" + "Reversals if any created." + La.Counter.ToString() + "\r\n";
                    Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                }
            }

            // 
            // HANDLE END-EXCEPTIONS
            //
            if (InOriginFileName == "NCR_FOREX")
            {
                // EXTRA FIELDS
                UpdateFiles_With_EXTRA(WOperator, InReconcCycleNo);
                int Test = 2;
                if (Test == 1)
                {
                    La.COPY_NCR_FOREX_ToIST(InOriginFileName, InReconcCycleNo);

                    if (La.ErrorFound == true)
                    {
                        stpErrorText += DateTime.Now + "_" + "Error During copying to IST.." + "\r\n" + La.ErrorOutput + "\r\n";
                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;
                        //  CatchDetails(ex);
                        return;
                    }
                    else
                    {
                        stpErrorText += DateTime.Now + "_" + "Records were copied to IST.." + La.Counter.ToString() + "\r\n";
                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                    }


                    RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();
                    Mg.UpdatetSourceTxnsProcessedToTrue_FOREX(InOriginFileName, InReconcCycleNo);

                }

            }
            //
            // PROCESS CAME TILL HERE WITH SUCCESS
            //
            stpReturnCode = 0;
            stpErrorText += DateTime.Now + "_" + "END_Loading of File .SUCCESS." + La.Counter.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
        }
       

     

        //*************************************
        // AFTER FILE LOADING UPDATE / SYCHRONISE FILES
        //**************************************
        int Counter = 0;
        public void UpdateRecordsWithTraceAndOther(string WOperator, int InRMCycleNo
            , int WFlogSeqNo, int InMode, DateTime InCut_Off_Date)
        {
            // TABLES INVOLVED
            //
            // REASON OF UPDATING : TO HAVE TRACE TO ABLE TO SHOW JOURNAL 
            //
            // 123
            // Master
            // VISA
            WCut_Off_Date = InCut_Off_Date; 
            DateTime LOW_LimitDate = InCut_Off_Date.AddDays(-12);
            DateTime LOW_LimitDate_2 = InCut_Off_Date.AddDays(-2);

            // Mode = 1 from Files ... if this is the case you process all 
            // Mode = 2 From Matching  ... only process Mpa.  
            string SQLCmd;

            if (InMode == 1)
            {
                stpReturnCode = -1;
                stpErrorText += "\r\n";
                stpErrorText += DateTime.Now + "_" + "UPDATE OF TRACES STARTS-NEEDED TO GET JOURNAL" + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE in 123 
                // AGAINST IST
                // Initialise counter 
                Counter = 0;

                stpReturnCode = 0;
            }

            // HERE WE GET THE DATE AND TIME FROM based24

            // UPDATE Details from IST Based on Accno and Terminal Id
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-2);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
          + " SET  "
           + " TransDate = t2.TransDate "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[base24] t2 " // base24
          + " ON "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
          + " AND t1.AtmTraceNo = t2.TraceNo "
          + " AND t1.TransAmount = t2.TransAmt "
          + " WHERE  (t1.IsMatchingDone = 0 ) "
            + " AND  (t2.Processed = 0 )  "
         
          ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During updating of Mpa from based24 ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Mpa Updated from base24..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Master with Replenishment Cycle No FROM [ATMS].[dbo].[SessionsStatusTraces]
            //
            // Initialise counter 
            Counter = 0;

            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + "  ReplCycleNo = t2.SesNo  "
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2"
    + " ON "
    + " t1.TerminalId = t2.AtmNo "
    + " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' AND t2.ProcessMode IN ( 0 ,-1, -5 )) "
    + "  AND (t1.TransDate BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel At Master updated with Repl Number 1.1...";
                    CatchDetails(ex);
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Repl Number 1..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // Matched Transactions
            // WHY we need this? 
            // WE need it when already the transactions are matched and then the Replenishment is done
            Counter = 0;
            //KONTO: Time take 44 seconds 
            // Set up a limit date to look into 
            SQLCmd =
    " UPDATE [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + "  ReplCycleNo = t2.SesNo  "
       + " FROM [RRDM_Reconciliation_MATCHED_TXNS].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2"
    + " ON "
    + " t1.TerminalId = t2.AtmNo "
    + " WHERE  (t1.IsMatchingDone = 1 AND t1.Origin='Our Atms' AND t2.ProcessMode IN ( 0 ,-1, -5 )) "
    + "  AND (t1.TransDate BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel AT Master updated with Repl Number 2...";
                    CatchDetails(ex);
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Repl Number 2..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            

            SQLCmd =
    " UPDATE [ATMS].[dbo].[CapturedCards] "
    + " SET "
    + "  SesNo = t2.SesNo  "
       + " FROM [ATMS].[dbo].[CapturedCards] t1 "
          + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2"
    + " ON "
    + " t1.AtmNo = t2.AtmNo "
    + " WHERE  (t2.ProcessMode IN ( 0 ,-1, -5 )) "
    + "  AND (t1.CaptDtTm BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel At CapturedCards updating ...";
                    CatchDetails(ex);
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "CapturedCards updating ..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //
            // UPDATE Errors with Replenishment Cycle No FROM [ATMS].[dbo].[SessionsStatusTraces]
            //
            // Initialise counter 
            Counter = 0;

           
            //
            // UPDATE Master with CARDNumber for Cardless Based on IST
            // NO NEED for BDC209
            // Initialise counter 
            Counter = 0;

            Counter = 0;
           
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Update the ones with Responsecode <> .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************
           
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Update the ones with Responsecode <> 0\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************


            stpErrorText += "OTHER UPDATES FINISHES";
            stpReturnCode = 0;

        }

        //*************************************
        public void UpdateFiles_With_EXTRA(string InOperator, int InReconcCycleNo)
        {
            try
            {
                // *****************************
                // UPDATE FILE WITH EXTRAS 
                RRDMMatchingBankToRRDMFileFields_EXTRA Me = new RRDMMatchingBankToRRDMFileFields_EXTRA();

                Me.ReadExtraFieldsAndFillTable_Distinct(InOperator);

                if (Me.DataTableExtraFields.Rows.Count > 0)
                {
                    int I = 0;

                    while (I <= (Me.DataTableExtraFields.Rows.Count - 1))
                    {
                        //    RecordFound = true;
                        string TableId = (string)Me.DataTableExtraFields.Rows[I]["TableId"];

                        string SQLCmd;

                        Me.ReadTable_EXTRA_AND_CREATE_COMMAND(TableId);
                        //
                        // Assign created command
                        //
                        SQLCmd = Me.FullCreatedSqlCommand;
                        RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

                        La.UPDATE_FILES_With_EXTRA(WOperator, InReconcCycleNo, SQLCmd);
                        // ***************************

                        I = I + 1;
                    }

                }

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }
        }

        public void UpdateCIT_FirstStageRecords_AUDI(string InOperator)
        {
            // Update them that replenishment is done
            try
            {

                string SQLCmd =
        " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_G4S_Repl_Entries]"
        + " SET "
        + "  ProcessMode_Load = -2  "
        + "   ,ReplDateG4S = t2.SesDtTimeEnd  "

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Intbl_CIT_G4S_Repl_Entries] t1 "
              + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2 "
        + " ON "
        + " t1.AtmNo = t2.AtmNo "
        + " AND t1.ReplCycleNo = t2.SesNo "
        + " WHERE  (t1.ProcessMode_load = 1) AND (t2.ProcessMode IN ( 0 ,-1, -5 )) "
        + "  ";

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                            cmd.CommandTimeout = 350;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                            var stats = conn.RetrieveStatistics();
                            commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                        }
                        // Close conn
                        conn.StatisticsEnabled = false;

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.StatisticsEnabled = false;
                        conn.Close();

                        stpErrorText = stpErrorText + "Cancel AT Master updated with Repl Number 2...";
                        CatchDetails(ex);
                        // return;
                    }

                stpErrorText += DateTime.Now + "_" + "Master updated with Repl Number 2..." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

        }
        // 
        // DELETE RECORDS TO SET STARTING DATE
        //
        public void DeleteRecordsToSetStartingPoint(string InOperator, DateTime InDate, DateTime InCut_Off_Date, int InRMCycleNo)
        {
            // CLEAR TABLE FOR TESTING
            //
            //            DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]
            int DelCount;

            
           
                string SQLCmd2;

            SQLCmd2 = " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                  + " WHERE TransDate < @TransDate    "
                  + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[base24] "
                  + " WHERE TransDate < @TransDate "
                  + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[bMaster] "
                  + " WHERE TransDate < @TransDate "
                  ;


            //SQLCmd2 = " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //      + " WHERE TerminalId = '95101069' AND AtmTraceNo < 6910    "
            //      + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[base24] "
            //      + " WHERE TerminalId = '95101069' AND TraceNo < 6910 "
            //      + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[bMaster] "
            //      + " WHERE TerminalId = '95101069' AND TraceNo < 6910 "
            //      ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd2, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", InDate);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails_2(ex, "Origin:001");
                        // CatchDetails(ex);
                    }
            

           
        }
        //
        // Update After Matching the created records 01 and 011 with Replenishemnt Cycle 
        //
        public void UPDATE_Mpa_After_Matching_With_ReplCycle(string InOperator, int InRMCycle)
        {
            // Initialise counter 
            Counter = 0;

            string SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + "  ReplCycleNo = t2.SesNo  "
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2 "
    + " ON "
    + " t1.TerminalId = t2.AtmNo "
    + " WHERE  (t1.IsMatchingDone = 1 AND MATCHED = 0  "
    + " AND t1.Origin='Our Atms' AND t2.ProcessMode IN ( 0 ,-1, -5 )) "
    + "  AND (t1.TransDate BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel AT Master updated with Repl AFTER MATCHING ";
                    CatchDetails(ex);
                    // return;
                }


        }
        // 
        // Get Totals

        //

        public int POOL_Total_Non_Processed;
        public int POOL_Total_Processed;

        public int IST_Total_Non_Processed;
        public int IST_Total_Processed;

        public int FCB_Total_Non_Processed;
        public int FCB_Total_Processed;

        public int NET_123_Total_Non_Processed;
        public int NET_123_Total_Processed;

        public int MASTER_Total_Non_Processed;
        public int MASTER_Total_Processed;

        public int POS_Total_Non_Processed;
        public int POS_Total_Processed;

        public int VISA_Total_Non_Processed;
        public int VISA_Total_Processed;

        public DataTable SourceFilesTotals = new DataTable();

        // 
        //
        int Total_Non_Processed;
        int Total_Processed;

        public void GetTotals(string InOperator, int InRMCycle)
        {
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            SourceFilesTotals = new DataTable();
            SourceFilesTotals.Clear();

            SourceFilesTotals.Columns.Add("File", typeof(string));
            SourceFilesTotals.Columns.Add("Non_Processed", typeof(string));
            SourceFilesTotals.Columns.Add("Processed", typeof(string));

            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();
            string WSelectionCriteria = "Operator = '" + InOperator + "'  AND Enabled = 1 ";
            Ms.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;

            while (I <= (Ms.SourceFilesDataTable.Rows.Count - 1))
            {
                string SourceFile_ID = (string)Ms.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string OriginSystem = (string)Ms.SourceFilesDataTable.Rows[I]["OriginSystem"];
                string DbTblName = (string)Ms.SourceFilesDataTable.Rows[I]["DbTblName"];

                string PhysicalFiledID = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + DbTblName + "]";

                //ArrayList FieldNamesC = new ArrayList();
                bool WithHeaders = false;

                // FIND OUT IF FILE EXIST
                Bg.ReadTableToGetFieldNames_Array_List(PhysicalFiledID, WithHeaders, 2);

                if (Bg.RecordFound == true)
                {
                    if (OriginSystem == "ATMs")
                    {
                        MethodGetTotalsForFileMpa(InOperator, InRMCycle, PhysicalFiledID);
                    }
                    else
                    {
                        MethodGetTotalsForFile_Non_Mpa(InOperator, InRMCycle, PhysicalFiledID);
                    }

                    //FILL TABLE 
                    DataRow RowSelected = SourceFilesTotals.NewRow();

                    RowSelected["File"] = SourceFile_ID;
                    RowSelected["Non_Processed"] = Total_Non_Processed;
                    RowSelected["Processed"] = Total_Processed;

                    // ADD ROW
                    SourceFilesTotals.Rows.Add(RowSelected);

                }

                I = I + 1;
            }

        }

        public void MethodGetTotalsForFileMpa(string InOperator, int InRMCycle, string InPhysicalFiledID)
        {
            Total_Non_Processed = 0;
            Total_Processed = 0;
            using (SqlConnection conn =
                      new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    //
                    //
                    // POOL
                    SqlString = "select  count(*) "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
                            + " WHERE IsMatchingDone = 0 AND Origin = 'Our Atms'";

                    SqlCommand comd1 = new SqlCommand(SqlString, conn);
                    comd1.CommandTimeout = 300;
                    Total_Non_Processed = Convert.ToInt32(comd1.ExecuteScalar());

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

        }

        public void MethodGetTotalsForFile_Non_Mpa(string InOperator, int InRMCycle, string InPhysicalFiledID)
        {
            Total_Non_Processed = 0;
            Total_Processed = 0;

            using (SqlConnection conn1 =
                      new SqlConnection(recconConnString))
                try
                {

                    conn1.Open();
                    //
                    //

                    //******************
                    // eg IST
                    SqlString = "SELECT count(*) "
                      + " FROM  " + InPhysicalFiledID
                      + " WHERE Processed = 0 "
                      + " ";

                    SqlCommand comd3 = new SqlCommand(SqlString, conn1)
                    {
                        CommandTimeout = 300
                    };
                    Total_Non_Processed = Convert.ToInt32(comd3.ExecuteScalar());


                    // Close conn
                    conn1.Close();
                }
                catch (Exception ex)
                {
                    conn1.Close();

                    MessageBox.Show("TOTALS A : Menam copy this and report that message was created from file.." + InPhysicalFiledID);

                    CatchDetails(ex);
                }

        }

        public void MethodToUpdateErrors(string InOperator, int InRMCycle, string InPhysicalFiledID)
        {

            string SQLCmd =
          " UPDATE [ATMS].[dbo].[ErrorsTable] "
           + " SET "
           + "  SesNo = t2.SesNo  "
          + " FROM [ATMS].[dbo].[ErrorsTable] t1 "
          + " INNER JOIN [ATMS].[dbo].[SessionsStatusTraces] t2"
    + " ON "
    + " t1.AtmNo = t2.AtmNo "
    + " WHERE  (t2.ProcessMode IN ( 0 ,-1, -5 )) "
    + "  AND (t1.[DateTime] BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During UPDATE Errors with ReplNo...";
                    CatchDetails(ex);
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATE Errors with ReplNo..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

        }

        // Catch details 
        private static void CatchDetails_2(Exception ex, string InOrigin)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("Origin of Error : ");
            WParameters.Append(InOrigin);
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");
            }
            //  Environment.Exit(0);
        }
    }
}

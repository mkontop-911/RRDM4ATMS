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
    public class RRDM_LoadFiles_InGeneral_EMR_ROM : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_ROM() : base() { }
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

        string SqlString; // Do not delete 

        string WFileSeqNo = "";
        string WOperator;

        DateTime NewVersionDt;

        public DataTable TableATMsDailyStats = new DataTable();
        public DataTable TableBULKSwitch = new DataTable();

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

            // Read fields of existing table
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            // 
            // CLEAN TABLES from ANY testing data 
            int TempWReconcCycleNo = 999999;
            Bio.CleanTables(InTableA, TempWReconcCycleNo);

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBankToGetName(99);
            //WOperator = Ba.Operator;
            string WBankShortName = Ba.ShortName; // Eg "BDC"

            BKName = WBankShortName; // "+BKName+"

            //Gp.ReadParameterByOccuranceId("101", "2");
            //if (Gp.RecordFound == true)
            //{
            //    PRX = Gp.OccuranceNm;
            //}

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

            NewVersionDt = new DateTime(2050, 03, 24);
            string ParId = "822"; // When version of files changes 
            string OccurId = "01"; // For IST and Master 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    NewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("822 parameter date is wrong");

                    ErrorFound = true;
                    CatchDetails(ex);
                }


                // MessageBox.Show("Master"); 

                //DateTime NewVersion3 = Convert.ToDateTime("24/03/2021");
                // date of change 
            }
            else
            {
                // Not found 
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
                case "Switch_IST_Txns":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_IST" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        // Switch_IST_Txns_20210324

                        InsertRecords_GTL_IST(InTableA, InFullPath, WReconcCycleNo);

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
                case "COREBANKING":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_COREBANKING" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_COREBANKING_2(InTableA, InFullPath, WReconcCycleNo);

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
             
                case "NCR_FOREX":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_NCR_FOREX" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_NCR_FOREX(InTableA, InFullPath, WReconcCycleNo);

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
        // IST LOAD
        //
        // GTL = GET TRANSFORM LOAD
        public void InsertRecords_GTL_IST(string InOriginFileName, string InFullPath
                                                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            //   stpErrorText = "";
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

            //string InDelimiter = ",";

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

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                        + " FROM '" + WFullPath_01.Trim() + "'"
                        + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                        + " ,ROWs_PER_BATCH=15000 "
                        + ",FIELDTERMINATOR = '\t'"
                       + " ,ROWTERMINATOR = '\r\n' )"
                        ;

            //\r\n
            //SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //            + " FROM '" + WFullPath_01.Trim() + "'"
            //            + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
            //            + " ,ROWs_PER_BATCH=15000 "
            //            + ",FIELDTERMINATOR = '\t'"
            //           + " ,ROWTERMINATOR = '\n' )"
            // ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);

                        cmd.CommandTimeout = 150;  // seconds
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
                    stpErrorText = stpErrorText + "Cancel At _Bulk_Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                         + WFullPath_01
                        );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_02_Bulk_Insert_Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // return; 

            //
            // Correct Amount = AMOUNT
            // remove not Needed Characters 
            // ""2,250.00""
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, ',', '')"
                     //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_COREBANKING_BULK_Records_2]"
                     //+ " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '.', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '-', '')"
                      + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [AMOUNT] = TRIM([AMOUNT]) "
                      + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET  [CURRENCY] = 'RON' "
                     + "  WHERE [CURRENCY] = '946' "
                      + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET  [CURRENCY] = 'EUR' "
                     + "  WHERE [CURRENCY] = '978' "
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]"
                    //+ " SET TRAN_DATE = '12/31/2018' "

                    + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Correcting AMOUNT";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            int fCount = 0; 

            ErrorFound = false;
            ErrorOutput = "";

            // DELETE THE FULL DUBLICATES
            string SqlString = " WITH cte AS( "
                          + " SELECT "
                          + "   [DATE], "
                            + "   [TIME], "
                              + "   TID, "
                          + "  AMOUNT, "
                          + "  PAN, "
                          + "  UTRNNO, "
                          + "   REFNUM , "
                          + "   ROW_NUMBER() OVER( "
                          + "       PARTITION BY "
                          + "  TID, "
                          + "  AMOUNT, "
                          + " PAN, "
                          + " UTRNNO, "
                          + "   REFNUM  "
                          + "   ORDER BY "
                          + " TID, "
                          + " AMOUNT, "
                          + "   REFNUM, "
                          + "   TIME "
                          + "    ) row_num "
                          + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + " WHERE TranDesc = 'DEPOSIT' and AUTHNUM <> '' AND RESULT = 'OK'  "
                          + " ) "
                          + " DELETE FROM cte  where (row_num = 2) ";
            //+ " Select * From cte " +
            //              " where (row_num > 1 ) ";
            //  + " DELETE FROM cte  where (row_num = 2 and Result = 'OK') ";
            //string SqlCommand = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //                + " WHERE MatchingCateg = @MatchingCateg "; 

            using (SqlConnection conn =
                new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

                        //rows number of record got updated
                        cmd.CommandTimeout = 300;
                        fCount = cmd.ExecuteNonQuery();
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


            // FIND OTHER DUBLICATES
            SqlString =
               " SELECT REFNUM, UTRNNO, TID, DATE,Time , Amount "
               + " FROM[RRDM_Reconciliation_ITMX].[dbo].BULK_Switch_IST_Txns "
               + " WHERE REFNUM IN( "
               + " SELECT REFNUM "
              + " FROM[RRDM_Reconciliation_ITMX].[dbo].BULK_Switch_IST_Txns "
              + "GROUP BY REFNUM "
              + "HAVING COUNT(*) > 1 "
              + ") "
              + "order by REFNUM "; 
                        
            using (SqlConnection conn =
                new SqlConnection(recconConnString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@ATM_NO", InATM_NO);

                        sqlAdapt.Fill(TableBULKSwitch);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }



            // " SELECT REFNUM, UTRNNO, TID, DATE,Time , Amount "

            RRDM_Journal_TransactionSummary_V2 Tb = new RRDM_Journal_TransactionSummary_V2(); 
          
            int Count = 0;

            int I = 0;
            // 
            while (I <= (TableBULKSwitch.Rows.Count - 1))
            {
                // READ
                string WREFNUM = (string)TableBULKSwitch.Rows[I]["REFNUM"];
                string WUTRNNO = (string)TableBULKSwitch.Rows[I]["UTRNNO"];
                string WAtmNo = (string)TableBULKSwitch.Rows[I]["TID"];
                string WDATE = (string)TableBULKSwitch.Rows[I]["DATE"];
                string Time = (string)TableBULKSwitch.Rows[I]["Time"];
                string Amount = (string)TableBULKSwitch.Rows[I]["Amount"];

                Tb.ReadAndFind_UTRNNO(WAtmNo, WUTRNNO); 

                if (Tb.RecordFound == true)
                {
                    // OK no correction needed
                }
                else
                {
                    // THIS A FANTOM RECORD
                    // Change Record Characteristics 
                    Tb.Update_RESPONSE_ValuesInSWITCH(WUTRNNO); 
                }


                //  Update_TOTALS_RECYCLE_AND_NOT(WSeqNo);

                I = I + 1;
            }




            RecordFound = false;
            int LastSeqNo_BULK;
            // Find LAST SEQ NO_BULK 
            // REASON: We needed it when we populate Switch file to get the original seqnumber 
            LastSeqNo_BULK = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
               //+ " WITH(NOLOCK) "
               + "";

            using (SqlConnection conn =
                             new SqlConnection(recconConnString))
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

                            LastSeqNo_BULK = (int)rdr["MaxSeqNo"];

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
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
               + "WITH(NOLOCK) "
               + "WHERE Processed = 0";

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

            //
            // INSERT IN IST NEW RECORDS READ
            //
            // INSERT IN IST NEW RECORDS READ
            // 
            //
            //1,1 : Our ATMs => Switch => to to ABEHOST(if debit = 489322) or pphost(if prepaid)
            //    124,1 : ABE POS  => Switch => to ABEHOST(if debit) or pphost(if prepaid)
            //    123,1 : 123 Net => Switch => to ABEHOST(if debit) or pphost(if prepaid)


            RecordFound = false;

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
                + "( "
                 + " [OriginFileName] "
                   + " ,[OriginalRecordId] "

                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
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
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "

                     + ",[EXTERNAL_DATE] "
                    + " , [Net_TransDate] "
                    + ",[Card_Encrypted] "
                      + ",[ACCEPTOR_ID] "
                        + ",[ACCEPTORNAME] "
                     + ",[CAP_DATE] "
                     + ",[SET_DATE] "
                     + ", [UTRNNO]  "
                 + ") "
                 + " SELECT "

                 + "@OriginFile "
                  //
                  + " , SeqNo "
                 + " ,@Origin "

                 + ",'" + PRX + "203' "

                  + ", '10' "

                  + ", CAST(CONCAT([DATE], ' ', TIME) AS DATETIME) "

                 // 
                 + ",case "
                 + " when [TRANS_TYPE] = '500' THEN 11 "
                 + " when [TRANS_TYPE] = '726' THEN 11 "
                 + " when [TRANS_TYPE] = '541' THEN 11 "
                 + " when [TRANS_TYPE] = '540' THEN 23 "

                 + " else 11 "
                 + "end "


                 + ",[TRANS_TYPE_DESCRIPTION] " // [TransDescr]


                 + " , ISNULL([CURRENCY], '') " // Currency 
                 + " , CAST([AMOUNT] As decimal(18,2)) " //  Amount

                 + ", 0  " // TRace numeric 
                           // + ",CAST(Right([TRACE],6) as int) " // TRace 

                 + ",ltrim(rtrim(ISNULL([REFNUM], '')))" // RRN

                 + ", '' " // FULL TRACE  char not available 
                           //  + ",[TRACE] " // FULL TRACE 

                  + ",'' " // Auth Number not available 
                  + ", '' " // Comment 
                            // + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here 
                 + ", ISNULL(TID, '') " // Terminal Id 

                 + ", ltrim(rtrim(ISNULL([PAN], '')))   "
                 // + ", ISNULL([ACTNUM], '') "
                 + " , '' " // ACCOUNT NOT AVAILABLE


                   + ",CASE " // Response CODE
                 + " when RESPONSE_CODE = '-1' THEN '0' " //
                 + " else RESPONSE_CODE "
                 + "end "
                 // + ",ltrim(rtrim([RESPCODE]))  " // RESPONSE CODE 

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", '' " // Transacton source 
                   + ", '' " // Destination



                        + ", CAST([DATE] as datetime) " // External 
                        + ", CAST([DATE] as datetime) " // Net Trans Date


                     + ", '' " // For Encrypted Card

                     + ", '' " // MERCHANT_TYPE
                     + ", '' " // ACCEPTORNAME
                               // SET CAP DATE AS Settlememnt date because looks that it is not used
                               // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                      + " , CAST(([DATE]) as date) " // Settlemment date 
                     + " , CAST(([DATE]) as date) " // 

                     + ", UTRNNO  "

                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
                 + " WHERE RESPONSE_CODE = '-1' AND SeqNo>" + LastSeqNo_BULK

            + " ";

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



            // CREATE TWIN


            //

            //
            // CREATE THE IST TWIN
            //
            //

            //
            // INSERT NEW RECORDS IN IST TWIN
            //


            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
            + "( "
             + " [OriginFileName] "
             + " ,[OriginalRecordId] "
             + " ,[Origin] "
              + " ,[MatchingCateg] "
             + " ,[TerminalType] "
             + " ,[TransDate] "

             + " ,[TransType] "
             + ",[TransDescr] "
               + ",[TransCurr] "
             + ",[TransAmt] "

             + ",[TraceNo] "

             + ",[RRNumber] "
             + ",[FullTraceNo] "
             + ",[AUTHNUM]" // Auth Number 
             + ",[TerminalId] "
             + ",[CardNumber] "
             + ",[AccNo] "

             + ",[ResponseCode] "
             + ",[LoadedAtRMCycle] "
             + ",[Operator] "
               + ",[TXNSRC] "
                 + ",[TXNDEST] "
                    + ",[EXTERNAL_DATE] "
                    + " , [Net_TransDate] "
                          + ",[Card_Encrypted] "
                             + ",[CAP_DATE] "
                             + ",[SET_DATE] "
                               + ",[UTRNNO] "
             //UTRNNO
             + ") "
             + " SELECT "
             + " [OriginFileName] "
             + " ,[OriginalRecordId] "
             + " ,[Origin] "
                 + " , 'EMR204' "

  + " ,[TerminalType] "
   //
   // + " ,[TransDate] "
   + " ,case"
               + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN [EXTERNAL_DATE] " // All outgoing Visa       
               + " else [TransDate] "
       //   WHERE TXNSRC = '1' AND TXNDEST = '13' and left([TransDescr], 2) = '81' AND AMOUNT <> '0'
       + " end "
  + " ,[TransType] "
  + " ,[TransDescr] "
  + " ,[TransCurr] "
  + " ,[TransAmt] "
  + " ,[TraceNo] "

  + " ,[RRNumber] "
  + " ,[FullTraceNo] "
  + ",[AUTHNUM]" // Auth Number 
    + " ,[TerminalId] "
     + " ,[CardNumber] "
  + " ,[AccNo] "
  + " ,[ResponseCode] "
  + " ,[LoadedAtRMCycle] "
  + " ,[Operator] "
  + " ,[TXNSRC] "
  + " ,[TXNDEST] "
  + " ,case"
               + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN [TransDate] " // All outgoing Visa       
               + " else [EXTERNAL_DATE] "

       + " end "

       + " , [Net_TransDate] "
       + ", [Card_Encrypted] "
      + ", [CAP_DATE] "
      + ", [SET_DATE] "
       + ", [UTRNNO] "

             + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "  // FROM IST

             + " WHERE  SeqNo > @SeqNo "
             + " AND LEFT(CardNumber, 6) "
             + " in ('405367' "
             + ", '411988' "
             + ", '414049' "
             + ", '423177' "
             + ", '424453' "
             + ", '426235' " //6
               + ", '454943' "
                + ", '460738' "
                  + ", '404801' "
                   + ", '486924' "
                    + ", '486951' "
                     + ", '486949' "
             //    460738........3339
             //404801........4009
             // 486924........7921
             //486951........9805
             //486949........1050
             + ", '460953' "
             + ", '529912' "
             + ", '529913' "
             + ", '545593' "
              + ", '549532' "
               + ", '552696' "
                + ", '553911' " //7
             + ") "
                 // +" AND ( "
                 //   + "  (TXNSRC = '1' AND TXNDEST = 'EBC')  " // All outgoing EBC
                 ////   + " OR (TXNSRC = '1' AND TXNDEST = '5')  " // 
                 //     + " ) "
                 ;
            //            405367
            //411988
            //414049
            //423177
            //424453
            //426235

            //460953
            //529912
            //529913
            //545593
            //549532
            //552696
            //553911

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", LastSeqNo);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For IST_TWIN" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At _IST_TWIN";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_04_IST_TWIN_Created..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            // REVERSALS 
            string WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] ";

            HandleReversals_IST(WFile, InOriginFileName, InReconcCycleNo);

            //if (ErrorFound == true) return;

            stpErrorText += DateTime.Now + "_" + "END OF IST WITH SUCCESS";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Set return code to 1
            // Succesful completion 

            stpReturnCode = 0;

            return;

            // HANDLE THE PRESENTER ERROR 
            // Make the one a normal transaction and the other as process to avoid to become reversal 

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
             + " SET "
             + " [Processed] = 1 "
             + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
               + " ,[Comment] = 'Presenter' "
            + " WHERE   Processed = 0 AND LEFT(FullTraceNo,1) = '4' AND ResponseCode = '112' "
             + " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
             + " SET "
               + " [Processed] = 1 "
             + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
               + " ,[Comment] = 'Presenter' "
            + " WHERE   Processed = 0 AND LEFT(FullTraceNo,1) = '4' AND ResponseCode = '112' "
             + " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
             + " SET "
               + " ResponseCode = '0' "
            //+ " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
              + " ,[Comment] = 'Presenter' "
            + " WHERE   Processed = 0 AND LEFT(FullTraceNo,1) = '2' AND ResponseCode = '112' "
            + " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
             + " SET "
               + " ResponseCode = '0' "
            //+ " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
             + " ,[Comment] = 'Presenter' "
            + " WHERE   Processed = 0 AND LEFT(FullTraceNo,1) = '2' AND ResponseCode = '112' "
            ;
            //   RESPCODE = '112' and TXNSRC = '1'
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            //**************************************************************************************
            // 15/03/2021
            // Here Update Master Card which is 235 category with the Settlement amount and currency
            // GET THE VALUES FROM THE BULK FILE 
            // ************************************************************************************
            // LEAVE IT HERE
            //
            // GO TO BDC TO FIND CODE IF NEEDED 
            //

            // CLEAR TABLE 
            //
            //  Set Comment to blank 
            // 
            //int U_Count = 0;

            //SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
            //    + " SET "
            //    + " [Comment] = '' "
            //   + " WHERE  LoadedAtRMCycle = @LoadedAtRMCycle AND Comment <> '' "
            //    + " "
            //    ;

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

            //            cmd.CommandTimeout = 350;  // seconds

            //            U_Count = cmd.ExecuteNonQuery();
            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails(ex);
            //    }
            // CLEAR TABLE 
            //
            //  After the TWIN table with NOT WANTED FAWRY
            // 
            // GO TO BDC FOR DETAILS OF CODE 
            //


            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // FOR IST
            //string From = "IST";
            //string WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] ";

            //HandleReversals_IST(WFile, InOriginFileName, InReconcCycleNo);

            //if (ErrorFound == true) return;

            //**********************************************************
            //stpErrorText += DateTime.Now + "_" + "Stage_06_IST_Reversals_Updated\r\n";

            //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************

            ////
            //// FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            //// FOR TWINS
            // From = "IST_TWIN";
            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] ";
            HandleReversals_IST_TWIN(WFile, InOriginFileName, InReconcCycleNo);

            if (ErrorFound == true) return;


            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] ";
            HandleDailyStatisticsForAtms(WFile, InOriginFileName, InReconcCycleNo);

            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "ATMs Daily STATs _Updated.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************
            //****************************************************
            //****************************************************

            stpErrorText += DateTime.Now + "_" + "END OF IST WITH SUCCESS";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Set return code to 1
            // Succesful completion 

            stpReturnCode = 0;
        }

        // HANDLE REVERSALS IST 
        public void HandleReversals_IST(string InTable_DB_Name, string InOriginFileName, int InReconcCycleNo)
        {
            return;
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
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          //+ " AND A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
            //+ " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) " // Leave it as it is 
            //+ " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
            //+ " AND (left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            + "  A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
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

            bool Panicos = false;
            if (Panicos == true)
            {
                // INSERT In REVERSALS IST FOR POS 
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
              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] A "
              + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] B "
              + " ON "
              //   + " A.TerminalId = B.TerminalId "
              + " A.CardNumber = B.CardNumber "
              //+ " AND A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
              + " AND A.RRNumber = B.RRNumber "
              + " AND A.TransDate = B.TransDate "
              + " WHERE "
                 //+ "(left(A.[TransDescr], 1) = '0' AND left(B.[TransDescr], 1) = '0')"
                 //+ " AND (LEFT(A.FullTraceNo, 1) = '4' AND A.Processed = 0) "
                 //+ " AND((LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1'))  AND B.Processed = 0) "
                 //+ " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
                 //+ " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
                 + " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) "
                + " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
                + " AND (left(A.[TransDescr], 1) = '0' AND left(B.[TransDescr], 1) = '0')"  // NOT POS
                + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
                + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
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

                stpErrorText += DateTime.Now + "_" + "Insert Reversals..Rec:.." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            }

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            Counter = 0;

            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]";
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
                                RRNumber_2 = RRNumber_2;
                            }
                            //  where terminalid_2 = '01901009' and RRNumber_2 = '124616171268'

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

            //CORRECTION1950
            // HERE READ ALL ATM DEPOSITS WIth FULL TRACE NUMBER STARTING WITH 2 PROCESSED AT THIS CYCLE AND UNDO THEM 
            // LEAVE THE REVERSAL COMMENT 
            // WRITE A METHOD BELOW 

            bool CORRECTION1950 = true;
            if (CORRECTION1950 == true) ;
            Mg.UpdateNonProcessedDepositReversals(TableId, InReconcCycleNo);

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
        //// CREATE COREBANKING FROM CORE
        /////
        public void InsertRecords_GTL_COREBANKING_2(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].["+PRX+"_123_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] 

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate COREBANKING FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "_COREBANKING_BULK_Records_2";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table
            //            BULK INSERT BULK_COREBANKING
            //FROM 'C:\RRDM\FilePool\COREBANKING\COREBANKING_20250401.001'
            //WITH(
            //    FIELDTERMINATOR = '\t',
            //    ROWTERMINATOR = '\n',
            //    FIRSTROW = 2-- Skip header row
            //);

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2  " // MUST be examined (may be change db character set to UTF8)
                                                  //+ " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t' "
                         + " ,ROWTERMINATOR = '\n' ) "
                          ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "_COREBANKING_BULK_Records Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert finishes with.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            RecordFound = false;

            SQLCmd =
              
                     " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                    + " SET Refnum = replace(Refnum, ';', '')"

                   + "; ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Correcting AMOUNT";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }



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

            // return;

            RecordFound = false;

            // INSERT IN COREBANKING
            //

            SQLCmd =
                 // "SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING]"
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                  + " ,[TerminalId] "

                 + " ,[TerminalType] "

                 + " ,[TransDate] "
                 + ", [AccNo] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransCurr] "
                 + ",[TransAmt] "

                  + ",[TraceNo] "
                 + ",[RRNumber] "


                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "

                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                     + ",[ACCEPTORNAME] "

                    + " , [Net_TransDate] "
                   + " , [SET_DATE] "
                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                  + " , 'EMR204' "

                 + ", ISNULL([TID], '') " // Terminal 
                 + ", @TerminalType " // Depends from Origin 10 or 20 
                                      // LEFT([DAT_TXN_POSTING],10) = '10/01/2019 00:'
                                      //  + ",ISNULL(try_convert(datetime, [DAT_TXN_POSTING] + ' ' "
                 //+ ", CONVERT(DATETIME, [TrnTimestamp], 105)"
                 + ",CONVERT(DATETIME,TRIM(Replace(Replace([TrnTimestamp],CHAR(10),''),CHAR(13),'')), 105)"
                 //+ ",CONCAT(try_convert(date, substring(trim([TrnTimestamp]), 1, 10), 105), ' ', CAST(try_convert(time, substring(trim([TrnTimestamp]), 11, 09)) AS TIME)) "
                    + ", AcEntrySerialNo "

                    + ",case "
                + " WHEN [DRCR] = 'D' THEN '11'  " // Withdrawl
                + " WHEN [DRCR] = 'c' THEN '23'  " // Withdrawl

                  + " else '0' "
                  + "end "

                     + " ,TrnDesc "

                 + ", CCy " // Currency 

                 + ", CAST([LCYAmount] As decimal(18,2)) "

                + " ,  0 " // In place of trace 
                 + ",ISNULL(LTRIM(REFNUM) , '') " //  RRN 

                 + ", '0' "
                 //+ ", Case " // Looks that response contains not ascii characters 
                 // + " WHEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(RESPONSECODE, CHAR(10), CHAR(32)),CHAR(13), CHAR(32)),CHAR(160), CHAR(32)),CHAR(9),CHAR(32)))) = '00' then '0' "
                 // + " ELSE LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(RESPONSECODE, CHAR(10), CHAR(32)),CHAR(13), CHAR(32)),CHAR(160), CHAR(32)),CHAR(9),CHAR(32)))) "
                 //  + " End "

                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'0'" // Origin ATMs -initialise with zero

                  + " ,'0'" // Final Destination -initialise with zero



                 + " ,'' " // for the comment 

                 + " , ''  "

                + "  , CONVERT(DATE, [ValueDate], 103) "// NET VALUE 
                 + "  , CONVERT(DATE, [ValueDate], 103) "// SET DATE 

                 //DAT_TXN_VALUE
                 // + " , CAST(ISNULL(try_convert(datetime, [LOCALTRANSACTIONDATETIME], 103 ), '1900-01-01') AS Date)  " // CAP_DATE ... Value Date 

                 //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tmpTransactions] "
                //+ " WHERE  "
                //+ " CAST([LCYAmount] As decimal(18,2)) > 0 "
                + " ORDER BY TID ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                        var stats = conn.RetrieveStatistics();
                        commandExecutionTimeInMs = (long)stats["ExecutionTime"];

                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted COREBANKING" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT RECORDS IN COREBANKING table";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO COREBANKING with..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            stpErrorText += DateTime.Now + "_" + "COREBANKING Finishes with SUCCESS.." + "\r\n";

            SQLCmd =

                     " Update [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] " 
                    + " SET RRNumber = replace(RRNumber, ';', '') "
                   + "; ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Correcting AMOUNT";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // 
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
          + ", A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
               + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ", A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
          + ",A.TraceNo As TraceNo_4 ,B.TraceNo As TraceNo_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          // Files
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] B "
          + " ON "//A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " A.RRNumber = B.RRNumber "
          + " AND A.TerminalId = B.TerminalId "
          + " AND A.Net_TransDate = b.Net_TransDate "
             //+ " AND A.AccNo = b.AccNo "
             + " WHERE "
          + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND ("
          + " (A.TransAmt < 0 AND B.TransAmt > 0) "
            + ")"
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR COREBANKING." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_COREBANKING";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //string FileId = "COREBANKING";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[COREBANKING]";
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
                    conn.Close();
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    conn2.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_COREBANKING";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

            return;

            //// TESTING 
            //stpErrorText += DateTime.Now + "_" + "COREBANKING Finishes with SUCCESS.." + "\r\n";

            //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //// Return CODE
            //stpReturnCode = 0;

            //return;


            // UPDATE Details from IST  and Terminal Id
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
          + " SET  "
          + " MatchingCateg = t2.MatchingCateg "
          //
          + " ,TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST " // We get the Description from here
          + ",Card_Encrypted = t2.Card_Encrypted "

          + ", TransDate = t2.TransDate " // there is a difference in the seconds 

            // + ",MatchingCateg = t2.MatchingCateg "

            + ",TransDescr = t2.TransDescr "

         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "

           + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = 9.99 " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          // + " t1.Processed = t2.Processed "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
          + " AND t1.Net_TransDate = t2.Net_TransDate "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.TransAmt = t2.TransAmt "
          //+ " AND t1.AccNo = t2.AccNo "
          + " WHERE   "
            + "(t1.Processed = 0 ) AND ((t2.Processed = 0 )  "
            + "      OR (t2.Processed = 1  AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
          ;  // 


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

            stpErrorText += DateTime.Now + "_" + "Card No_1 and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //CORRECTION1950
            // HERE READ ALL ATM DEPOSITS WIth FULL TRACE NUMBER STARTING WITH 2 PROCESSED AT THIS CYCLE AND UNDO THEM 
            // MAKE THEM NOT PROCESSED
            // LEAVE THE REVERSAL COMMENT 
            // WRITE A METHOD BELOW 

            //bool CORRECTION1950 = true;
            //if (CORRECTION1950 == true) ;
            //Mg.UpdateNonProcessedDepositReversals(TableId, InReconcCycleNo);

            //
            stpErrorText += DateTime.Now + "_" + "COREBANKING Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

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
                        MessageBox.Show("Duplicate Entry For Cash_Loaded" + Environment.NewLine
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
                Ta.ReadSessionsStatusTracesToFindSesNoBasedDateEnd(WAtmNo, WReplDate);

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
                    G4.Repl_Load_Status = 2; // No action yet 
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
                        if ((Na.Balances1.MachineBal + Na.Balances1.PresenterValue == G4.UnloadedCounted))
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
                                        SM_Deposits = WTotalCassetteMoney + WRECYCLEDTotalMoney;
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

                        if (SM_Deposits == G4.Deposits)
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
        //*******************
        //// CREATE NCR_FOREX
        /////****************
        public DataTable ForexTrans;
        public void InsertRecords_GTL_NCR_FOREX(string InOriginFileName, string InFullPath
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

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

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


            // Truncate Table FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During EMR_NCR_FOREX_BULK_Records_TRUNCATE";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Truncate Table FOR BULK FOREX CHILD

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_CHILD";

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

                    stpErrorText = stpErrorText + "Cancel During EMR_NCR_FOREX_BULK_Records_CHILD_TRUNCATE";
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

                    stpErrorText = stpErrorText + "Cancel During EMR_NCR_FOREX_BULK_Records Bulk Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

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

            //       
            // CREATE NCR_FOREX_CHILD from BULK 
            //

            SQLCmd =
                " WITH cte "
+ " AS(  SELECT     ( '20'+ SUBSTRING(NewATMDate,7 ,2) "
+ " +'-'+SUBSTRING(NewATMDate,4 ,2) "
+ " +'-'+SUBSTRING(NewATMDate,1 ,2) "
+ "  + ' ' + SUBSTRING(NewATMDate,10 ,5)) AS NewATMDate "
+ " , [ATMId]  ,CAST([TransactionSequence] as Int) AS TransactionSequence   "
+ " ,CAST([TotalNetValue] AS decimal(18,2)) AS TotalNetValue "
 + " ,  [CurrencyCode] "
+ "	   ,CAST([NoteCount] as Int) as [NoteCount] "
+ "  ,CAST([NoteWeight] as Int) AS [NoteWeight] "
+ "	   ,CAST ((Cast(NoteCount As Int)*Cast(NoteWeight As int)) as decimal(18,2)) AS Fcy  " // Fcy 
+ "	   ,CAST (EquivalentTotal as decimal(18,3))  AS LcyTotal " // local equivalent 
+ "	   ,  NoteRate " // Note ccy rate
+ "	   ,cast (CommissionValue as decimal(18,3)) AS CommisionTotal " // Commision 
 + "    ,  [Canceled] ,[Confirmed], "
 + " ROW_NUMBER() OVER(        PARTITION BY    "
 + "  [ATMId],   [TransactionSequence], [TotalNetValue]  "
 + "    ORDER BY       [ATMId],   [TransactionSequence],        [TotalNetValue]     ) "
+ "	 row_num  FROM[RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX] "
+ "  ) "
+ "  SELECT *  FROM cte "
+ " ORDER BY ATMId, TransactionSequence, CurrencyCode ";
            //ISNULL([REFNUM], '')
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ForexTrans = new DataTable();
            ForexTrans.Clear();

            using (SqlConnection conn =
             new SqlConnection(recconConnString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SQLCmd, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ForexTrans);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During EMR_NCR_FOREX_BULK_Records CHILD CREATION";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            int I = 0;
            int WSeqNo = 0;
            int WSeqNoDep = 0;
            bool Withdrawl;

            int TotalCount_Weight = 0;
            decimal WDeposit = 0;
            int WCounter = 0;

            string OldCcy = "";
            string OldATMId = "";
            int OldTransactionSequence = 0;

            try
            {
                while (I <= (ForexTrans.Rows.Count - 1))
                {
                    // Do 

                    // public decimal LcyEquivalent;
                    //public decimal LcyRounded;
                    //public decimal Commision; 

                    La.NewATMDate = (string)ForexTrans.Rows[I]["NewATMDate"];
                    La.ATMId = (string)ForexTrans.Rows[I]["ATMId"];
                    La.TransactionSequence = (int)ForexTrans.Rows[I]["TransactionSequence"];
                    La.TotalNetValue = (decimal)ForexTrans.Rows[I]["TotalNetValue"];
                    La.CurrencyCode = (string)ForexTrans.Rows[I]["CurrencyCode"];
                    int WNoteCount = (int)ForexTrans.Rows[I]["NoteCount"];
                    int WNoteWeight = (int)ForexTrans.Rows[I]["NoteWeight"];
                    decimal Fcy = (decimal)ForexTrans.Rows[I]["Fcy"];
                    La.LcyEquivalentTotal = (decimal)ForexTrans.Rows[I]["LcyTotal"];
                    La.NoteRate = (string)ForexTrans.Rows[I]["NoteRate"];
                    La.CommisionTotal = (decimal)ForexTrans.Rows[I]["CommisionTotal"];

                    La.Canceled = (string)ForexTrans.Rows[I]["Canceled"];
                    La.Confirmed = (string)ForexTrans.Rows[I]["Confirmed"];
                    long row_num = (int)(long)ForexTrans.Rows[I]["row_num"];

                    TotalCount_Weight = WNoteCount * WNoteWeight;
                    WDeposit = Fcy;

                    if (OldATMId != La.ATMId ||
                                 OldTransactionSequence != La.TransactionSequence)
                    {
                        WCounter = 0;
                    }
                    WCounter = WCounter + 1;

                    if (WCounter == 1)
                    {
                        // insert Withdrawl 
                        Withdrawl = true;
                        WSeqNo = La.Insert_FOREX_CHILD(Withdrawl);

                        // insert Deposit 
                        Withdrawl = false;

                        La.DepositAmt = WDeposit;

                        WSeqNoDep = La.Insert_FOREX_CHILD(Withdrawl);

                        OldCcy = La.CurrencyCode;
                    }
                    if (WCounter > 1)
                    {
                        // HANDLE DEPOSITS
                        //
                        // Update record 
                        if (La.CurrencyCode == OldCcy)
                        {
                            // Update based based on SeqNo
                            La.ReadFOREX_Record(WSeqNoDep);

                            decimal NewDepositAmt = La.Pre_DepositAmt + WDeposit;

                            La.UpdateFOREX_Deposit(WSeqNoDep, NewDepositAmt);
                        }
                        else
                        {
                            // INSERT NEW DEPOSIT
                            Withdrawl = false;
                            La.DepositAmt = WDeposit;
                            WSeqNoDep = La.Insert_FOREX_CHILD(Withdrawl);

                            OldCcy = La.CurrencyCode;

                        }

                    }

                    OldATMId = La.ATMId;
                    OldTransactionSequence = La.TransactionSequence;


                    I++;
                }
            }
            catch (Exception ex)
            {

                stpErrorText = stpErrorText + "Cancel During EMR_NCR_FOREX_BULK_Records CHILD CREATION_2";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                CatchDetails(ex);
                return;
            }


            RecordFound = false;

            //// KEEP Forex Child ALL 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD_ALL]"
                + " Select "
               + "  [NewATMDate] "
               + " ,RIGHT('00000000' + ATMId, 8) "
               + " ,[TransactionSequence] "
               + " ,[TotalNetValue] "
               + " ,[CurrencyCode] "
               + " ,[DepositAmt] "
               + " ,[LcyEquivalentTotal] "
               + " ,[NoteRate] "
               + " ,[CommisionTotal] "
               + " ,[Canceled] "
               + " ,[Confirmed] "
               + " ,[Withdrawl] "

                + " ,@RMCycle " + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD] ";

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
                    stpErrorText = stpErrorText + "Cancel During Creation of COREBANKING";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Forex Child ALL s..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //       
            // CREATE NCR_FOREX from BULK FOR DEPOSITS
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[NCR_FOREX]  "
                 + "( "
                 + " [OriginFileName] "

                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransCurr] "

                 + ",[TransAmt] "
                 + ",[TraceNo] "
                 + ",[RRNumber] "
                 + ",[AUTHNUM] "
                 + ",[TerminalId] "
                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     // + ",[Comment] "
                     + " ,[EXTERNAL_DATE] "
                     + ",[Net_TransDate] "
                      + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                 + " ,'" + PRX + "209' " // Within the cardless transactions 
                 + ", @TerminalType"
                 // NewATMDate // FOREX 
                 //+ ", CAST( (+'20'+ SUBSTRING(NewATMDate,7 ,2)+'/'+SUBSTRING(NewATMDate,4 ,2)+'/'+SUBSTRING(NewATMDate,1 ,2)"
                 //+ " + ' ' + SUBSTRING(NewATMDate,10 ,8)) as datetime) "
                 + " , NewATMDate "
                 // Deposit
                 + ",case "
                 + " when ([Confirmed] = '1') THEN 23  " // Deposit
                 + " when ([Confirmed] = '0') THEN 11  " // Reversal 
                 + " else 0 "
                 + "end "
                  + ",case "
                 + " when ([Confirmed] = '1') THEN 'FOREX_DEPOSIT'  "
                 + " when ([Confirmed] = '0') THEN 'FOREX_DEPOSIT_Not_confirmed'   " // Reversal 
                 + " else 'FOREX_DEPOSIT-Invalid' "
                 + " end "
                 // + ", 'FOREX_DEPOSIT' "
                 + ",[CurrencyCode]" //
                 + ", DepositAmt "
                 + ",TransactionSequence " // Trace 
                 + ", 'CR'+ + Cast(TransactionSequence As Char)  " // RRN + Authorisation Number
                 + ",  '0' "
                 + ",  RIGHT('00000000' + ATMId, 8) " // Terminal Id

                 + " , '123456******1111' "
                  //
                  + ",case "
                 + " when ([Confirmed] = '1') THEN '0'  " // Response code
                 + " when ([Confirmed] = '0') THEN 112  " // Reversal 
                 + " else '0' "
                 + "end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'1' "   // Origin NCR
                     + ",'128' " // Destination FOREX
                                 //+ " ,"
                                 //  + ", CAST( (+'20'+ SUBSTRING(transactiondatetime,7 ,2)+'/'+SUBSTRING(transactiondatetime,4 ,2)+'/'+SUBSTRING(transactiondatetime,1 ,2)"
                                 //+ "  ) as datetime) "
                   + " , NewATMDate " // external date
                   + " , CAST(NewATMDate as Date) " // Net_Trans date 
                                                    // + ", CAST( (+'20'+ SUBSTRING(transactiondatetime,7 ,2)+'/'+SUBSTRING(transactiondatetime,4 ,2)+'/'+SUBSTRING(transactiondatetime,1 ,2)"
                                                    //+ " ) as date) "
                  + ", @FileDATEresult "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_CHILD"
                // GET Only the valid ones
                + " WHERE Withdrawl = 0  "
                + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For NCR_FOREX - CREDIT " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During NCR_FOREX insert of CREDIT records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in NCR_FOREX with CREDIT records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //return; 

            //       
            // CREATE NCR_FOREX from BULK FOR Withdrawls
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[NCR_FOREX]  "
                 + "( "
                 + " [OriginFileName] "

                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransCurr] "

                 + ",[TransAmt] "
                 + ",[TraceNo] "
                 + ",[RRNumber] "
                 + ",[AUTHNUM] "
                 + ",[TerminalId] "
                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     // + ",[Comment] "
                     + " ,[EXTERNAL_DATE] "
                     + ",[Net_TransDate] "
                      + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                 + " ,'" + PRX + "209' " // Within the cardless transactions 
                 + ", @TerminalType"
                 // NewATMDate // FOREX 
                 + " , NewATMDate "
                 // Deposit
                 + ",case "
                 + " when ([Confirmed] = '1') THEN 11  " // Withdrawl
                 + " when ([Confirmed] = '0') THEN 21  " // Reversal 
                 + " else 0 "
                 + "end "
                 + ",case "
                 + " when ([Confirmed] = '1') THEN 'FOREX_WITHDRAWL'  " // Withdrawl
                 + " when ([Confirmed] = '0') THEN 'FOREX_WITHDRAWL_Not_confirmed'   " // Reversal 
                 + " else 'FOREX_WITHDRAWL-Invalid' "
                 + " end "
                 + ", 'EGP' " // LOCAL CURRENCY
                 + ",TotalNetValue   " // LOCAL Currency
                 + ",TransactionSequence " // Trace 
                 + ", 'DR'+ + Cast(TransactionSequence As Char)  " // RRN + Authorisation Number
                 + ",  '0' "
                 + ",  RIGHT('00000000' + ATMId, 8) " // Terminal Id

                 + " , '123456******1234' "
                  //
                  + ",case "
                 + " when ([Confirmed] = '1') THEN '0'  " // Response code
                 + " when ([Confirmed] = '0') THEN 112  " // Reversal 
                 + " else '0' "
                 + "end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'1' "   // Origin ATMs
                     + ",'125' " // Destination Cardless
                                 //+ " ,"
                  + " , NewATMDate " // external date
                   + " , CAST(NewATMDate as Date) " // Net_Trans date 
                      + ", @FileDATEresult "

                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_CHILD"
                  // GET Only the valid ones
                  + " WHERE Withdrawl = 1  "
                + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For NCR_FOREX - WITHDRAWL " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During NCR_FOREX insert of WITHDRAWL records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in NCR_FOREX with WITHDRAWL records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // 
            // HANDLE END-EXCEPTIONS
            //
            try
            {
                // EXTRA FIELDS
                UpdateFiles_With_EXTRA(WOperator, InReconcCycleNo);


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
            catch (Exception ex)
            {

                stpErrorText = stpErrorText + "Cancel During FOREX INSERT TO IST";
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                CatchDetails(ex);
                return;
            }

            stpErrorText += DateTime.Now + "_" + "Insert in IST FROM NCR_FOREX .." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            stpErrorText += DateTime.Now + "_" + "NCR_FOREX FINISHES with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

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

                SQLCmd =
              " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]"
              + " SET TraceNo = t2.TraceNo "
                + "  , AccNo = t2.AccNo "
                  + "  , Card_Encrypted = t2.Card_Encrypted "
              // + "  , CAP_DATE = t2.CAP_DATE "

              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

              + " ON "
              + "  t1.TerminalId = t2.TerminalId "
              + " AND t1.RRNumber = t2.RRNumber "
              + " AND t1.TransAmt = t2.TransAmt"
              //+ " WHERE  (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) "
              + " WHERE  (t1.Processed = 0) AND ((t2.Processed=0 AND T2.MatchingCateg = '" + PRX + "210') "
              + "OR (t2.Processed=1 AND T2.MatchingCateg = '" + PRX + "210' AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate)  "
           + "      OR (t2.Processed = 1 AND T2.MatchingCateg = '" + PRX + "210' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
          ; //  


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During Update with Trace..For..123_NET.1.";
                        // Initialise counter 
                        Counter = 0;
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "Update with Trace..For..123_NET.1." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE in 123 
                // Against  TWIN
                // Initialise counter 
                Counter = 0;

                SQLCmd =
              " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]"
              + " SET TraceNo = t2.TraceNo "
               + "  , Card_Encrypted = t2.Card_Encrypted "
                 + "  , AccNo = t2.AccNo "
               // + "  , CAP_DATE = t2.CAP_DATE "

               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

              + " on  "
                + "  t1.Net_TransDate = t2.Net_TransDate "
                 + " AND t1.TerminalId = t2.TerminalId "
              + " AND t1.TransAmt = t2.TransAmt "
            + " AND t1.RRNumber = t2.RRNumber "
              + " WHERE   "
                + "  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "215' )  "
                + "     AND ((T2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "215' )  "
                + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "215' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
       ; // 
                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During Update with Trace..For..123_NET.2.";
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "Update with Trace..For..123_NET.2." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                //
                // UPDATE TRACE in Master Card
                //
                // Initialise counter 
                Counter = 0;

                SQLCmd =
             " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
              + " SET TraceNo = t2.TraceNo "
               + "  , Card_Encrypted = t2.Card_Encrypted "
                 + "  , AccNo = t2.AccNo "
                  + "  , CAP_DATE = t2.CAP_DATE "

              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

              + " on t1.RRNumber = t2.RRNumber "
              + " AND t1.TransAmt = t2.TransAmt"
                // + " WHERE  (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                + " WHERE  (t1.Processed = 0) "
                + " AND ((t2.Processed=0 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232')) "
                       + "OR (t2.Processed=1 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232') AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate) "
                           + " OR (t2.Processed = 1 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232') AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..MASTER.1.";

                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..MASTER.1." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE and other information in Master Card AGAINST TWIN
                // Initialise counter 
                Counter = 0;

                SQLCmd =
             " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
              + " SET TraceNo = t2.TraceNo "
                 + "  , AccNo = t2.AccNo "
                  //+ " , TransAmt = t2.TransAmt "
                  + "  , Card_Encrypted = t2.Card_Encrypted "
                   + "  , CAP_DATE = t2.CAP_DATE "

               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

              + " on t1.RRNumber = t2.RRNumber "
              + " AND t1.TerminalId = t2.TerminalId"
                //+ " WHERE  (t1.MatchingCateg = 'BDC235' AND t2.MatchingCateg = 'BDC235') AND (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                + " WHERE  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "235')  "
                + "AND ((t2.Processed=0 AND t2.MatchingCateg = '" + PRX + "235' )  "
                           + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "235' AND t2.TransAmt>0"
                           + " AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..MASTER.2.";

                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..MASTER.2." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE in VISA Card
                // Initialise counter 
                Counter = 0;

                SQLCmd =
             " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]"
              + " SET TerminalId = t2.TerminalId "
                + "  ,TraceNo = t2.TraceNo "
                     + "  , AccNo = t2.AccNo "
                  + "  , Card_Encrypted = t2.Card_Encrypted "
                   + "  , CAP_DATE = t2.CAP_DATE "

                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

              + " on t1.RRNumber = t2.RRNumber "
              + " AND t1.TransAmt = t2.TransAmt"
                //  + " WHERE  (t1.MatchingCateg = 'BDC225' AND t2.MatchingCateg = 'BDC225' ) AND (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                + " WHERE (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "225')  "

                + "AND ((t2.Processed=0 AND t2.MatchingCateg = '" + PRX + "225' )  "
                           + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "225' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..VISA..";
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..VISA.." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE in CREDIT Card
                // Initialise counter 
                Counter = 0;

                SQLCmd =
             " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CREDIT_Card]"
              + " SET TerminalId = t2.TerminalId "
                + "  , Card_Encrypted = t2.Card_Encrypted "
                + "  ,TraceNo = t2.TraceNo "
                     + "  , AccNo = t2.AccNo "
                      + "  , CAP_DATE = t2.CAP_DATE "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CREDIT_Card] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"
              + " on t1.RRNumber = t2.RRNumber "
              + " AND t1.TransAmt = t2.TransAmt"
                //+ " WHERE  (t1.MatchingCateg = 'BDC240' AND t2.MatchingCateg = 'BDC240' ) AND (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                + " WHERE  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "240'  ) "
                 + "AND ((t2.Processed=0 AND t2.MatchingCateg = '" + PRX + "240' )  "
                           + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "240' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..Credit..";
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..Credit.." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE CARD and trace in FAWRY
                // Initialise counter 
                Counter = 0;

                SQLCmd =
             " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[FAWRY]"
              + " SET CardNumber = t2.CardNumber "
                + "  , Card_Encrypted = t2.Card_Encrypted "
                + "  ,TraceNo = t2.TraceNo "
                 + "  , CAP_DATE = t2.CAP_DATE "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"
              + " on t1.RRNumber = t2.RRNumber "
              + " AND t1.TransAmt = t2.TransAmt"
              + " AND t1.TerminalId = t2.TerminalId"
                + " WHERE  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "251'  ) "
                 + "AND ((t2.Processed=0 AND t2.MatchingCateg = '" + PRX + "251' )  "
                           + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "251' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..FAWRY..";
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..FAWRY.." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                //
                // POS UPDATE Account Number from IST
                // ONLY FOR BDC231 and not for BDC233 which is prepaid
                //
                // Initialise counter 
                Counter = 0;

                SQLCmd =
              " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
              + " SET AccNo = t2.AccNo "
               + "  , CAP_DATE = t2.CAP_DATE "

              + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

              + " ON "
              //+ " t1.TransAmt = t2.TransAmt " // it could be different 
              + "  t1.RRNumber = t2.RRNumber "
              + " AND t1.CardNumber = t2.CardNumber "
              //   + " AND t1.Net_TransDate = t2.Net_TransDate "

              + " WHERE  (t1.Processed = 0 ) "
                   + "AND ((t2.Processed = 0 and T2.MatchingCateg = '" + PRX + "231' )  "
                     + " OR (t2.Processed = 1 and T2.MatchingCateg = '" + PRX + "231' AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate ) "
                           + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "231' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;


                //TXNDEST = '1') AND TXN = '0-POS Purchase' THEN 'BDC231' " // All Incoming POS
                //       + " WHEN (TXNSRC = '5' AND TXNDEST = '12') THEN 'BDC233' " // All Incoming POS - Prepaid

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                        stpErrorText = stpErrorText + "Cancel During UPDATE For POS Acno Updated from IST...";
                        CatchDetails(ex);
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE For POS Acno Updated from IST..." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                stpErrorText += "UPDATE Of TRACES FINISHES" + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                stpReturnCode = 0;
            }
            //
            // UPDATE Master with Matching Category ID Based on IST
            // UPDATE OTHER INFO TOO
            // Initialise counter 
            Counter = 0;

            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + " MatchingCateg = t2.MatchingCateg "
    + " ,TransDescr = t2.TransDescr "
    + " ,CardNumber = t2.CardNumber "
    // + " ,AccNumber = t2.AccNo "
    + " ,TransDate = t2.TransDate" // Correct transaction date with seconds 
    + " ,TXNSRC = t2.TXNSRC "
    + " ,TXNDEST = t2.TXNDEST "
    + " ,RRNumber = t2.RRNumber "
    + " ,Card_Encrypted = t2.Card_Encrypted "
    + "  , CAP_DATE = t2.CAP_DATE "
    + "  , SET_DATE = t2.SET_DATE " // SET DATE SAME AS CAP Because Origin is ATMS
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

    + " ON "
    + " t1.TerminalId = t2.TerminalId "
       //+ " AND t1.CardNumber = t2.CardNumber "
       + " AND t1.TraceNoWithNoEndZero = t2.TraceNo "
    + " AND t1.TransAmount = t2.TransAmt"
    + " AND t1.Minutes_Date = t2.Minutes_Date"

    //+ " AND t1.AccNumber = t2.AccNo "
    + " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' ) "
     + "AND ((t2.Processed = 0 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81')  "
                           + " OR (t2.Processed = 1 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81' "
                           + "AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;
            // For not processed yet records coming from ATMs

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                    stpErrorText = stpErrorText + "Cancel At Master updated with Category 1...";
                    CatchDetails(ex);
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Category 1..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Master with Matching Category ID Based on IST
            // Less MatchingCateg = 'BDC299'
            // Initialise counter 
            //        Counter = 0;

            //        SQLCmd =
            //" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //+ " SET "
            //+ " MatchingCateg = t2.MatchingCateg "
            //+ " ,AccNumber = t2.AccNo "
            //+ " ,CardNumber = t2.CardNumber "
            //+ " ,TransDate = t2.TransDate " // Correct transaction date with seconds 
            //+ " ,TXNSRC = t2.TXNSRC "
            //+ " ,TXNDEST = t2.TXNDEST "
            //+ " ,RRNumber = t2.RRNumber "
            //+ " ,Card_Encrypted = t2.Card_Encrypted "
            //+ "  , CAP_DATE = t2.CAP_DATE "
            // + "  , SET_DATE = t2.SET_DATE " // SET DATE SAME AS CAP Because Origin is ATMS
            //   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
            //      + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

            //+ " ON "
            //+ " t1.TerminalId = t2.TerminalId "
            //   //  + " AND t1.CardNumber = t2.CardNumber "
            //   + " AND t1.TraceNoWithNoEndZero = t2.TraceNo "
            //+ " AND t1.TransAmount = t2.TransAmt"
            //+ " AND t1.Minutes_Date = t2.Minutes_Date"

            ////+ " AND t1.AccNumber = t2.AccNo "
            //+ " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' ) "
            // + "AND ("
            //        + "(t2.Processed = 0 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81')  "
            //                       + "  OR (t2.Processed = 1  AND t2.TXNSRC= '1'  AND left(t2.TransDescr, 2) <> '81' AND t2.Net_TransDate>= @Net_TransDate )"
            //                       + "  OR (t2.Processed = 1 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81'"
            //                       + "          AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )"
            //                       + ") "
            //            ;
            //        // For not processed yet records coming from ATMs

            //        using (SqlConnection conn = new SqlConnection(recconConnString))
            //            try
            //            {
            //                conn.StatisticsEnabled = true;
            //                conn.Open();
            //                using (SqlCommand cmd =
            //                    new SqlCommand(SQLCmd, conn))
            //                {
            //                    cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
            //                    cmd.CommandTimeout = 350;  // seconds
            //                    Counter = cmd.ExecuteNonQuery();
            //                    var stats = conn.RetrieveStatistics();
            //                    commandExecutionTimeInMs = (long)stats["ExecutionTime"];

            //                }
            //                // Close conn
            //                conn.StatisticsEnabled = false;

            //                conn.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                conn.StatisticsEnabled = false;
            //                conn.Close();

            //                stpErrorText = stpErrorText + "Cancel At Master updated with Category 2...";
            //                CatchDetails(ex);
            //                //return;
            //            }

            //        stpErrorText += DateTime.Now + "_" + "Master updated with Category 2..." + Counter.ToString() + "\r\n";
            //        stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Master with Matching Category ID Based on IST
            // For cases where IST has been moved to AGING
            // IST DATA Base is the matched data base 
            // Initialise counter 
            //RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            //int WReconcCycleNoFirst = Rjc.ReadFirstReconcJobCycle(WOperator, "ATMs");


            // DateTime WFirstCut_Off_Date = Rjc.Cut_Off_Date.Date;
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            string Test = "NotBeDone";
            //
            if (Test == "ToBeDone")
            {
                string WSelectionCriteria = " WHERE MatchingCateg = 'EMR299' AND LoadedAtRMCycle=" + InRMCycleNo;
                int WDB_Mode = 1;

                Mpa.ReadTablePoolDataToGetTableByCriteria(WSelectionCriteria, WDB_Mode);

                RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
                // You get a table here 
                int WSeqNo = 0;
                int K = 0;

                // Occurances for this action 
                while (K <= (Mpa.MpaTable.Rows.Count - 1))
                {

                    //    RecordFound = true;
                    WSeqNo = (int)Mpa.MpaTable.Rows[K]["SeqNo"];
                    WDB_Mode = 1;
                    WSelectionCriteria = " WHERE SeqNo = " + WSeqNo;
                    Mpa.ReadInPoolTransSpecificBySelectionCriteria(WSelectionCriteria, WDB_Mode);

                    // Based on info read IST recond 
                    // If IST record found then update Mpa record with category id 


                    Mgt.ReadAndFindDateBasedOnDetails_4(Mpa.TerminalId, Mpa.TraceNoWithNoEndZero, Mpa.TransAmount, Mpa.Minutes_Date);

                    if (Mgt.RecordFound == true)
                    {
                        // This the case of late coming Journal
                        // There is 
                        if (Mgt.Comment.Length < 5) ;
                        {
                            //MessageBox.Show("Comment less than 5 "); 
                        }
                        if (Mgt.Comment.Length >= 5)
                        {
                            if (Mgt.Processed == true
                                & Mgt.Comment.Substring(0, 5) == "Aging")
                            {
                                Mpa.MatchingCateg = Mgt.MatchingCateg;
                                Mpa.AccNumber = Mgt.AccNo;
                                Mpa.CardNumber = Mgt.CardNumber;
                                Mpa.TransDate = Mgt.TransDate;
                                Mpa.TXNSRC = Mgt.TXNSRC;
                                Mpa.TXNDEST = Mgt.TXNDEST;
                                Mpa.RRNumber = Mgt.RRNumber;
                                Mpa.Card_Encrypted = Mgt.Card_Encrypted;
                                Mpa.CAP_DATE = Mgt.CAP_DATE;
                                Mpa.SET_DATE = Mgt.SET_DATE;

                                Mpa.UpdateMpaWithDetailsFromIST_Aging(WSeqNo, WDB_Mode = 1);

                            }
                        }


                    }

                    K = K + 1;
                }



            }

            Counter = 0;


            //
            // CLEAR ACCOUNT IN Mpa
            // ONCE UPDATED FROM IST WE GET THE LAST 14 digits 
            // Initialise counter 
            //Counter = 0;

            //SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //         + " SET AccNumber = RIGHT(ltrim(rtrim(AccNumber)),12) "
            //          + " WHERE  IsMatchingDone = 0   "
            //          ;

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            //    try
            //    {
            //        conn.StatisticsEnabled = true;
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {

            //            cmd.CommandTimeout = 350;  // seconds
            //            Counter = cmd.ExecuteNonQuery();
            //            var stats = conn.RetrieveStatistics();
            //            commandExecutionTimeInMs = (long)stats["ExecutionTime"];


            //        }
            //        // Close conn
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.StatisticsEnabled = false;
            //        conn.Close();
            //        stpErrorText = stpErrorText + "Cancel UPDATING of Acc for Master  ..";
            //        stpReturnCode = -1;

            //        stpReferenceCode = stpErrorText;
            //        CatchDetails(ex);

            //        //return;
            //    }

            //stpErrorText += DateTime.Now + "_" + "UPDATING of Acc for Master  .." + Counter.ToString() + "\r\n";
            //stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //************************
            //************************
            // UPDATE MAXIMUM DATE ON THE Repl Sessions with -1
            //
            stpErrorText += DateTime.Now + "_" + "START UPDATING Max Date for Repl Sessions with -1  .." + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            Mpa.ReadInPoolTransAndGetMaxDatesByATM(LOW_LimitDate);

            stpErrorText += DateTime.Now + "_" + "FINISH UPDATING Max Date for Repl Sessions with -1  .." + Counter.ToString() + "\r\n";
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

            //        //
            //        // UPDATE Actions Occurances in case the Repl Cycle has been changed
            //        //
            //        // Initialise counter 
            //        Counter = 0;

            //        SQLCmd =
            //" UPDATE [ATMS].[dbo].[Actions_Occurances] "
            //+ " SET "
            //+ "  ReplCycle = t2.ReplCycleNo  "
            //   + " FROM [ATMS].[dbo].[Actions_Occurances] t1 "
            //      + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t2"
            //+ " ON "
            //+ "     t1.AtmNo = t2.TerminalId "
            //+ " AND t1.UniqueKey = t2.UniqueRecordId "
            //+ " WHERE  (t1.UniqueKeyOrigin = 'Master_Pool' ) AND (t2.ActionType <> '00' ) "
            //+ "  AND (t1.CaptDtTm BETWEEN t2.SesDtTimeStart AND t2.SesDtTimeEnd)";

            //        using (SqlConnection conn = new SqlConnection(recconConnString))
            //            try
            //            {
            //                conn.StatisticsEnabled = true;
            //                conn.Open();
            //                using (SqlCommand cmd =
            //                    new SqlCommand(SQLCmd, conn))
            //                {
            //                    // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
            //                    cmd.CommandTimeout = 350;  // seconds
            //                    Counter = cmd.ExecuteNonQuery();
            //                    var stats = conn.RetrieveStatistics();
            //                    commandExecutionTimeInMs = (long)stats["ExecutionTime"];

            //                }
            //                // Close conn
            //                conn.StatisticsEnabled = false;

            //                conn.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                conn.StatisticsEnabled = false;
            //                conn.Close();

            //                stpErrorText = stpErrorText + "Cancel At CapturedCards updating ...";
            //                CatchDetails(ex);
            //                // return;
            //            }

            //        stpErrorText += DateTime.Now + "_" + "CapturedCards updating ..." + Counter.ToString() + "\r\n";
            //        stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Capture Cards with Replenishment Cycle No FROM [ATMS].[dbo].[SessionsStatusTraces]
            //
            // Initialise counter 
            Counter = 0;

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

            SQLCmd =
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

            //
            // UPDATE Master with CARDNumber for Cardless Based on IST
            // NO NEED for BDC209
            // Initialise counter 
            Counter = 0;



            // Update RMCateg
            // Initialise counter 
            Counter = 0;

            SQLCmd = "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET RMCateg =  MatchingCateg "
                      + " WHERE IsMatchingDone = 0 AND Origin <> 'Our Atms'  "
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
                    stpErrorText = stpErrorText + "Mpa Cancel At_Update RM Category ...";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    //return;
                }

            stpErrorText += DateTime.Now + "_" + "Update RM Category ..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Presenter errors with the correct category
            // FROM Mpa
            // THIS IS AFTER UPDATING OF Mpa
            // Initialise counter 
            Counter = 0;

            SQLCmd =
    " UPDATE [ATMS].[dbo].[ErrorsTable] "
    + " SET "
    + " CategoryId = t2.MatchingCateg "

     + " FROM [ATMS].[dbo].[ErrorsTable] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t2"
    + " ON "
    + " t1.UniqueRecordId = t2.UniqueRecordId "
    + " WHERE  (t2.IsMatchingDone = 0 AND t2.Origin='Our Atms'  ) "; // For not processed yet records

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

                    stpErrorText = stpErrorText + "Mpa Cancel at Update Presenter Errors updated with Category ...";
                    CatchDetails(ex);
                    //return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Presenter Errors updated with Category ..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Group of ATMS
            //
            // Initialise counter 
            Counter = 0;
            //KONTO Time Taken = 98 seconds 
            // We need this. ???? Or we can do this while loading or during changing of ATM group 
            // Check this 
            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + " RMCateg = 'RECATMS-' + CAST(t2.AtmsReconcGroup As Char) "

    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
    + " INNER JOIN  [ATMS].[dbo].[ATMsFields] t2 "

    + " ON "
    + " t1.TerminalId = t2.AtmNo "
    + " WHERE  (t1.IsMatchingDone = 0  AND t1.Origin='Our Atms' ) "; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
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

                    stpErrorText = stpErrorText + "Cancel At Master updated with ATM Group ...";
                    CatchDetails(ex);
                    //return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with ATM Group ..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            RRDMMatchingCategories Mc = new RRDMMatchingCategories();

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, "ALL");

            // LOOP FOR Matching Categories
            stpErrorText += DateTime.Now + "_" + "UPDATE Matched Files ." + "\r\n";


            int I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 
                string WMatchingCateg = (string)Mc.TableMatchingCateg.Rows[I]["Identity"];

                RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();

                Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

                //
                SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                          + " SET FileId01 =  @FileId01, FileId02 =  @FileId02, FileId03 =  @FileId03, FileId04 =  @FileId04  "
                          + " WHERE  IsMatchingDone = 0 AND MatchingCateg = @MatchingCateg    "
                          ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {

                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {

                            cmd.Parameters.AddWithValue("@MatchingCateg", WMatchingCateg);

                            cmd.Parameters.AddWithValue("@FileId01", Mcs.SourceFileNameA);
                            cmd.Parameters.AddWithValue("@FileId02", Mcs.SourceFileNameB);
                            cmd.Parameters.AddWithValue("@FileId03", Mcs.SourceFileNameC);
                            cmd.Parameters.AddWithValue("@FileId04", Mcs.SourceFileNameD);
                            cmd.CommandTimeout = 350;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        stpErrorText = stpErrorText + "Cancel At_UPDATE Master Pool Involved Files Based on Categories .";
                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;
                        CatchDetails(ex);

                        // return;
                    }

                I++; // Read Next entry of the table ... Next Category 
            }

            stpErrorText += DateTime.Now + "_" + "UPDATE Master Pool Involved Files Based on Categories ." + "\r\n";
            //
            // UPDATE GL ACCOUNTS 
            //
            RRDMAccountsClass Acc = new RRDMAccountsClass();
            if (PRX == "BDC")
            {

                Acc.ReadAllATMsAndUpdateAccNo(WOperator);

                stpErrorText += DateTime.Now + "_" + "GL Accounts For ATMs UPDATED." + "\r\n";

            }
            {
                if (PRX == "EMR")
                    Acc.ReadAllATMsAndUpdateAccNo_AUDI(WOperator, WCut_Off_Date);

                // MessageBox.Show("READ HERE THE CASH ACCOUNT NUMBER FOR THE ATM ");
            }

            Counter = 0;
            //KONTO Time taken = 75 seconds 
            //
            // Update IST as processed the ones with errors
            // LEAVE IT HERE AT THE END

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [Processed] = 1 "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'With Response code not 0 ' "
                 + " WHERE  "
               + "  Processed = 0 and ResponseCode <> '0' and ResponseCode <> '112' AND Net_TransDate < @Net_TransDate " // Non Processed 
             ; // 

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);
                        cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate_2);
                        cmd.CommandTimeout = 350;
                        conn.ResetStatistics();
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

                    CatchDetails(ex);

                    return;
                }

            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Update the ones with Responsecode <> .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************
            //
            // Update IST TWIN as processed the ones with errors
            //
            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
                + " SET "
                + " [Processed] = 1"
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'With Error' "
                 + " WHERE  "
               + "  Processed = 0 and ResponseCode <> '0' AND Net_TransDate < @Net_TransDate " // Non Processed 
             ; // 

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);
                        cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate_2);
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

                    CatchDetails(ex);

                    return;
                }

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

            //ADDED FOR AUDI
            // DELETE To Normalise Files 

            //            DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]
            //        WHERE TransDate< '2021-09-08 16:30:00.000'


            //DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]
            //      WHERE TransDate< '2021-09-08 16:30:00.000'


            //DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING]
            // Deletions are based on the Data on CoreBanking available
            // Also the same data are based on available Journals. 
            bool Deletebefore = true;

            if (Deletebefore == true)
            {
                string SQLCmd2 = " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                         + " WHERE (Net_TransDate < '2021-08-31' OR  Net_TransDate > '2021-09-01') and TerminalId = '01900501'  "
                         + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                         + " WHERE (Net_TransDate < '2021-09-02' OR  Net_TransDate > '2021-09-05') and TerminalId = '01901009' "
                         + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                         + " WHERE (Net_TransDate < '2021-09-02' OR  Net_TransDate > '2021-09-16') and TerminalId = '01903507' "
                          + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                         + " WHERE (Net_TransDate < '2021-08-29' OR  Net_TransDate > '2021-09-01') and TerminalId = '01905505' "
                         ;

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

                SQLCmd2 = " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " WHERE TransDate  < @TransDate    "
                      + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                      + " WHERE TransDate  < @TransDate "
                      + " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                      + " WHERE TransDate  < @TransDate "
                      ;

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


            // RETURN FOR AUDI
            return;

            // DELETE FROM IST THE UNWANTED
            // THESE ARE FAWRY Txns originated from ATMs that must be deleted 

            string SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " Where TransDescr = '0-POS Purchase' and TXNSRC = '1'   "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@TransDate", InDate);

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

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
               + " WHERE TransDate  < @TransDate   "
               + " "
               ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    CatchDetails_2(ex, "Origin:002");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
             + " WHERE TransDate  < @TransDate   "
             + "   " // 
             ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    CatchDetails_2(ex, "Origin:003");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
              + " WHERE TransDate  < @TransDate   "
              + "  "
              ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    CatchDetails_2(ex, "Origin:004");
                }
            //
            //
            //            DELETE FROM 123 txns before testing period

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]"
                 + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    stpErrorText = stpErrorText + "Cancel During Delete ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails_2(ex, "Origin:005");
                    // return;
                }

            //
            // DELETE logically Meeza Transactions
            //
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "937";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "NO")
            {
                //
                //
                //            DELETE FROM MEEZA_OWN_ATMS txns before testing period


                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS]"
                 + " WHERE TransDate  < @TransDate   ";

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
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

                        stpErrorText = stpErrorText + "Cancel During Delete ";
                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;
                        CatchDetails_2(ex, "Origin:006");
                        // return;
                    }

                //
                //
                //            DELETE FROM MEEZA_OTHER_ATMS txns before testing period


                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS]"
                     + " WHERE TransDate  < @TransDate   ";

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
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

                        stpErrorText = stpErrorText + "Cancel During Delete ";
                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;
                        CatchDetails_2(ex, "Origin:007");
                        //return;
                    }

                //
                //
                //            DELETE FROM MEEZA_POS txns before testing period
                //

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS]"
                     + " WHERE TransDate  < @TransDate   ";

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
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

                        stpErrorText = stpErrorText + "Cancel During Delete ";
                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;
                        CatchDetails_2(ex, "Origin:008");
                        //return;
                    }

            }

            //            DELETE FROM MASTER CARD txns before testing period

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
                 + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    stpErrorText = stpErrorText + "Cancel During DELETE ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails_2(ex, "Origin:009");
                    //return;
                }
            //            DELETE FROM MASTER CARD txns before testing period

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card_POS] "
                 + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    stpErrorText = stpErrorText + "Cancel During DELETE ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails_2(ex, "Origin:010");
                    // return;
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]"
             + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:011");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]"
                + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:012");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] "
               + " WHERE TransDate  < @TransDate   ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:013");
                }

            // DELETE FROM POOL 
            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE TransDate  < @TransDate  AND IsMatchingDone = 0  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
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

                    CatchDetails_2(ex, "Origin:014");
                }
            //
            // DELETE FROM POOL the BDC299 which is the result of toxic journals 
            //
            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE MatchingCateg  = 'EMR299' AND TransDate  < @Cut_Off_Date  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date.AddDays(-11));

                        cmd.CommandTimeout = 350;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:089");
                }
            //
            // DELETE logically Special Accounts (Secret accounts)
            //

            ParId = "933";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET "
                      + " IsMatchingDone = 1 "
                      + " , Matched = 1 "
                     + " , [MatchMask] = '111' "
                      + " , MatchingAtRMCycle = @ProcessedAtRMCycle"
                     + " , SettledRecord = 1 "
                      + " ,Comments = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNumber])),7) = '0195425' AND MatchingCateg = '" + PRX + "201' AND IsMatchingDone = 0 "
                    + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET "
                      + " IsMatchingDone = 1 "
                      + " , Matched = 1 "
                     + " , [MatchMask] = '11' "
                      + " , MatchingAtRMCycle = @ProcessedAtRMCycle"
                      + " ,Comments = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNumber])),7) = '0195425' AND MatchingCateg <> '" + PRX + "201' AND IsMatchingDone = 0 AND ResponseCode = '0'"
                    + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNo])),7) = '0195425' AND [Processed] = 0 "
                     + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNo])),7) = '0195425' AND [Processed] = 0 "
                    + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNo])),7) = '0195425' AND [Processed] = 0 "
                      + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD] "
                        + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNo])),7) = '0195425'  AND [Processed] = 0 "
                    + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Secret Accounts' "
                    + " where LEFT(ltrim(rtrim([AccNo])),7) = '0195425'  AND [Processed] = 0 ";


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:015");
                    }
                MessageBox.Show("Special Accounts Transactions Updated as processed:.." + Counter.ToString());

            }



            //
            // DELETE logically Cardless Transactions
            //

            ParId = "934";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET "
                      + " IsMatchingDone = 1 "
                      + " , Matched = 1 "
                      + " , MatchingAtRMCycle = @ProcessedAtRMCycle"
                      + " ,Comments = 'CardLess' "
                      + " where LEFT(ltrim(rtrim([CardNumber])),6) = '123456' AND IsMatchingDone = 0 "

                    ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:016");
                    }

                MessageBox.Show("Cardless Transactions for ATMs Updated as processed:.." + Counter.ToString());


                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'CardLess' "
                    + " where LEFT(ltrim(rtrim([CardNumber])),6) = '123456' AND [Processed] = 0 "

                    ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:017");
                    }

                MessageBox.Show("Cardless Transactions for IST Updated as processed:.." + Counter.ToString());

            }

            //
            // DELETE logically Fawry Transactions
            //

            ParId = "935";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {

                SQLCmd =
                     "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                       + " SET "
                       + " [Processed] = 1"
                       + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                       + " ,[Comment] = 'Fawry' "
                     + " where left([TransDescr], 2) = '81' AND [Processed] = 0 "

                     ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:018");
                    }

                MessageBox.Show("Fawry Transactions for IST Updated as processed:.." + Counter.ToString());


                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Fawry' "
                    + " where  TransDescr = 'Fawry' AND[Processed] = 0 "
                   ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:019");
                    }

                MessageBox.Show("Fawry Transactions for COREBANKING Updated as processed:.." + Counter.ToString());

            }

            //
            // DELETE logically Meeza Transactions
            // OR UPDATE ACCOUNT NUMBER FOR PRE PAID CARDS

            ParId = "937";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {

                // DELETE logically Meeza Transactions
                SQLCmd =
                     "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET "
                      + " IsMatchingDone = 1 "
                      + " , Matched = 1 "
                      + " , MatchingAtRMCycle = @ProcessedAtRMCycle"
                      + " ,Comments = 'Meeza' "
                      + " ,SettledRecord = 1 "
                      + " where LEFT(ltrim(rtrim([CardNumber])),6) = '507803' AND IsMatchingDone = 0 "
                     //
                     + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                       + " SET "
                       + " [Processed] = 1"
                       + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                       + " ,[Comment] = 'Meeza' "
                     + " where LEFT(ltrim(rtrim([CardNumber])),6) = '507803' AND [Processed] = 0 "

                     ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:020");
                    }

                MessageBox.Show("Meeza Transactions for IST Updated as processed:.." + Counter.ToString());

                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                      + " SET "
                      + " [Processed] = 1"
                      + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                      + " ,[Comment] = 'Meeza' "
                    + " where LEFT(ltrim(rtrim([CardNumber])),6) = '507803' AND[Processed] = 0 "

                   ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:021");
                    }

                MessageBox.Show("Meeza Transactions for COREBANKING Updated as processed:.." + Counter.ToString());


                MessageBox.Show("Meeza Transactions for COREBANKING Updated as processed:.." + Counter.ToString());

            }

            // UPDATE ACCOUNT NUMBER FOR PRE PAID CARDS with Card
            // to be used at Action stage stage. 
            SQLCmd =
" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
+ "   SET [AccNumber] = Card_Encrypted "
+ "  WHERE MatchingCateg = '" + PRX + "203' "

+ " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "211'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "232'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg ='" + PRX + "231'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg ='" + PRX + "233'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "271'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS]  "
+ "  SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "273'  "
              ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:022");
                }


            //if (txtline.Substring(0, 14) == "CardLess Forex")
            //{
            //    trandesc = "FOREX_" + trandesc;

            //    CardNo = "123456******1234";
            //}

            //
            // DELETE BOTH logically AND PHYSICALLY "FOREX" Transactions
            //

            ParId = "936";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " SET "
                      + " IsMatchingDone = 1 "
                      + " , Matched = 1 "
                      + " , MatchingAtRMCycle = @ProcessedAtRMCycle"
                      + " ,Comments = 'CardLess_FOREX' "
                      + " ,SettledRecord = 1 "
                      + " where LEFT(ltrim(rtrim([CardNumber])),6) = '123456' "
                      + "AND LEFT(ltrim(rtrim([TransDescr])),5)= 'FOREX' AND IsMatchingDone = 0 "

                    ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:023");
                    }

                // MessageBox.Show("Cardless Transactions for ATMs Updated as processed:.." + DelCount.ToString());
                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " WHERE LEFT(ltrim(rtrim([CardNumber])),6) = '123456' "
                      + "AND LEFT(ltrim(rtrim([TransDescr])),5)= 'FOREX' AND IsMatchingDone = 0 "
                    ;


                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:024");
                    }

            }

            // MessageBox.Show("Cardless Transactions for ATMs Updated as processed:.." + DelCount.ToString());

            //
            // HERE DELETE ALL OLD of Master Card version 
            // To be excluded after 30 days of the date of new version 
            // 
            NewVersionDt = new DateTime(2050, 03, 24);
            WOperator = "BCAIEGCX";
            ParId = "822"; // When version of files changes 
            OccurId = "01"; // For IST and Master 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound == true)
            {
                NewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);

                //DateTime NewVersion3 = Convert.ToDateTime("24/03/2021");
                // date of change 
            }
            else
            {
                // Not found 
            }

            if (Gp.RecordFound == true) // FOUND for Occurance 01 which is Master
            {
                DelCount = 0;
                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                   + " WHERE TransDate  < @TransDate AND MatchingCateg in ('EMR230','EMR231','EMR232','EMR233','EMR235','EMR236')  "
                   + " "
                   ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", NewVersionDt);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:Master01");
                    }

                DelCount = 0;

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
                 + " WHERE TransDate  < @TransDate AND MatchingCateg in ('EMR230','EMR231','EMR232','EMR233','EMR235','EMR236')  "
                 + "   " // 
                 ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", NewVersionDt);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:Master02");
                    }

                DelCount = 0;

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[COREBANKING] "
                  + " WHERE TransDate  < @TransDate  AND MatchingCateg in ('EMR230','EMR231','EMR232','EMR233','EMR235','EMR236') "
                  + "  "
                  ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", NewVersionDt);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:Master03");
                    }

                DelCount = 0;

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
                  + " WHERE TransDate  < @TransDate  AND MatchingCateg in ('EMR230','EMR231','EMR232','EMR233','EMR235','EMR236') "
                  + "  "
                  ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", NewVersionDt);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:Master04");
                    }

                DelCount = 0;

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                  + " WHERE TransDate  < @TransDate  AND MatchingCateg in ('EMR230','EMRC231','EMR232','EMR233','EMR235','EMR236') "
                  + "  "
                  ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", NewVersionDt);

                            cmd.CommandTimeout = 350;  // seconds

                            DelCount = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails_2(ex, "Origin:Master05");
                    }

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

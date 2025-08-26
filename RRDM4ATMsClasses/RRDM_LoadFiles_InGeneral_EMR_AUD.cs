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
    public class RRDM_LoadFiles_InGeneral_EMR_AUD : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_AUD() : base() { }
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
                case "Egypt_123_NET":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_123_NET" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_123_NET(InTableA, InFullPath, WReconcCycleNo);

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
                case "MEEZA_OWN_ATMS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MEEZA_OWN_ATMS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_MEEZA_OWN_ATMS(InTableA, InFullPath, WReconcCycleNo);

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
                case "MEEZA_OTHER_ATMS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MEEZA_OTHER_ATMS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_MEEZA_OTHER_ATMS(InTableA, InFullPath, WReconcCycleNo);

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
                case "MEEZA_POS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MEEZA_POS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_MEEZA_POS(InTableA, InFullPath, WReconcCycleNo);

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
                case "FAWRY":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_FAWRY" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_FAWRY(InTableA, InFullPath, WReconcCycleNo);

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
                case "MASTER_CARD":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MASTER_CARD" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_MASTER(InTableA, InFullPath, WReconcCycleNo);

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
                case "VISA_CARD":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_VISA_CARD" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                        InsertRecords_GTL_VISA(InTableA, InFullPath, WReconcCycleNo);

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

                case "E_FINANCE_D_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_E_FINANCE_D_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                        InsertRecords_GTL_E_FINANCE_D_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "E_FINANCE_GL":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_E_FINANCE_GL" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                        InsertRecords_GTL_E_FINANCE_GL(InTableA, InFullPath, WReconcCycleNo);

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
                case "Credit_Card":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_Credit_Card" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_Credit_Card(InTableA, InFullPath, WReconcCycleNo);

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
                case "MASTER_CARD_POS":
                    {
                        // return; 
                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MASTER_CARD_POS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        if (FileDATEresult >= NewVersionDt)
                        {
                            InsertRecords_GTL_MASTER_CARD_POS_2(InTableA, InFullPath, WReconcCycleNo);
                        }
                        else
                        {
                            InsertRecords_GTL_MASTER_CARD_POS(InTableA, InFullPath, WReconcCycleNo);
                        }



                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, InFullPath);

                        }
                        else
                        {
                            MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + InFullPath);
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

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                        + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = '" + InDelimiter + "'"
                         // + " ,ROWTERMINATOR = '\n' )"
                         + " ,ROWTERMINATOR = '\r\n' )"
                          ;

            //SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //            + " FROM '" + WFullPath_01.Trim() + "'"
            //            + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
            //            + " ,ROWs_PER_BATCH=15000 "
            //            + ",FIELDTERMINATOR = '\t'"
            //           + " ,ROWTERMINATOR = '\n' )"
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
            // Correct CREATE Destination 
            // 
            // 
            //       SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //                + " SET Destination = case "
            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '125')  THEN 'TAHWEEL' "
            // + "   WHEN(TXNSRC = '1' AND TXNDEST = '555556')  THEN 'DEBIT' "

            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '555558')  THEN 'DEBIT' "

            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '555559')  THEN 'DEBIT' "

            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '555560')  THEN 'DEBIT' "

            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '9000818444')  THEN 'VISA' "

            // + "    WHEN(TXNSRC = '1' AND TXNDEST = '9000818555')  THEN 'MASTER' "

            //+ "     WHEN(TXNSRC = '1' AND TXNDEST = '981819')  THEN 'EBC' "

            //+ "   WHEN(TXNSRC = '1' AND TXNDEST = '9000818222')  THEN 'CREDIT' "
            //+ "  else 'Not_Def' "
            //+ "    end ";

            //       using (SqlConnection conn = new SqlConnection(recconConnString))
            //           try
            //           {
            //               conn.Open();
            //               using (SqlCommand cmd =
            //                   new SqlCommand(SQLCmd, conn))
            //               {
            //                   Counter = cmd.ExecuteNonQuery();
            //               }
            //               // Close conn
            //               conn.Close();
            //           }
            //           catch (Exception ex)
            //           {
            //               conn.Close();

            //               stpErrorText = stpErrorText + "Cancel During Correct Destination";
            //               stpReturnCode = -1;

            //               stpReferenceCode = stpErrorText;
            //               CatchDetails(ex);
            //               return;
            //           }

            RecordFound = false;

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
                 //    + " ,[OriginalRecordId] "

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
                 + ") "
                 + " SELECT "

                 + "@OriginFile "
                 //
                 // TXNSRC 1 Destination MASTER Goes to CoreBanking Except Deposits
                 // TXNSRC 1 Destination TAHWEEL  Goes to IST
                 // TXNSRC 1 Destination VISA Goes to CoreBanking
                 // TXNSRC 1 Destination EBC Goes to CoreBanking
                 // TXNSRC 1 Destination DEBIT Goes to CoreBanking
                 //
                 + " ,@Origin "
                   + " ,case "  // SET UP THE CATEGORY 
                   + " WHEN (TXNSRC = '1' AND Destination = 'DEBIT')  THEN '" + PRX + "201' " // Only DEBIT CARD 
                    + " WHEN (TXNSRC = '1' AND Destination = 'TAHWEEL')  THEN '" + PRX + "203' " //  CARDLESS
                    + " WHEN (TXNSRC = '1' AND Destination = 'EBC')  THEN '" + PRX + "205' " //  123
                      + " WHEN (TXNSRC = '1' AND Destination = 'MASTER' AND left([TXN], 2) <> '21' )  THEN '" + PRX + "206' " //  MASTER
                       + " WHEN (TXNSRC = '1' AND Destination = 'MASTER' AND left([TXN], 2) = '21' )  THEN '" + PRX + "203' " //  MASTER DEPOSIT THAT DOESNT GO TO COREBANKING

                        + " WHEN (TXNSRC = '1' AND Destination = 'VISA')  THEN '" + PRX + "207' " // VISA

                  + " else 'Not_Def' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "
                  // DATE TIME 
                  + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"

                 // 
                 + ",case "
                 + " when left([TXN], 2) = '10' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '10' AND Left(TRACE, 1)='4' THEN 21 "
                 + " when left([TXN], 2) = '11' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '11' AND Left(TRACE, 1)='4' THEN 21 "
                 + " when left([TXN], 2) = '12' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '12' AND Left(TRACE, 1)='4' THEN 21 "
                 + " when left([TXN], 2) = '13' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '13' AND Left(TRACE, 1)='4' THEN 21 "
                 + " when left([TXN], 2) = '30' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '30' AND Left(TRACE, 1)='4' THEN 21 "
                 + " when left([TXN], 2) = '81' AND Left(TRACE, 1)='2' THEN 11 "
                 + " when left([TXN], 2) = '81' AND Left(TRACE, 1)='4' THEN 21 "

                    + " when left([TXN], 2) = '21' AND Left(TRACE, 1)='2' THEN 23 "
                 + " when left([TXN], 2) = '21' AND Left(TRACE, 1)='4' THEN 13 "

                     + " when left([TXN], 2) = '26' AND Left(TRACE, 1)='2' THEN 23 "
                 + " when left([TXN], 2) = '26' AND Left(TRACE, 1)='4' THEN 13 "

                  + " when left([TXN], 2) = '40' AND Left(TRACE, 1)='2' THEN 33 "
                 + " when left([TXN], 2) = '40' AND Left(TRACE, 1)='4' THEN 43 "

                    + " when left([TXN], 2) = '41' AND Left(TRACE, 1)='2' THEN 33 "
                 + " when left([TXN], 2) = '41' AND Left(TRACE, 1)='4' THEN 43 "

                 + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                 + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                 + " else 11 "
                 + "end "
                 + ",[TXN] " // [TransDescr]

                 //+ ", Case "   // Currency Code
                 //+ " WHEN TXNSRC = '5' THEN ISNULL([SETTLEMENT_CODE], '') "
                 //+ " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                 //+ " end "
                 //  + ", Case "   // Transaction amount
                 //+ " WHEN TXNSRC = '5' THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                 //+ " ELSE CAST([AMOUNT] As decimal(18,2)) "
                 //+ " end "
                 + " , ISNULL([ACQ_CURRENCY_CODE], '') "
                 + " , CAST([AMOUNT] As decimal(18,2)) "

                 //  + ", ISNULL([ACQ_CURRENCY_CODE], '') " it was .... 
                 // + ",CAST([AMOUNT] As decimal(18,2)) "  it was 

                 + ",CAST(Right([TRACE],6) as int) " // TRace 

                 + ",ltrim(rtrim(ISNULL([REFNUM], '')))" // RRN

                 + ",[TRACE] " // FULL TRACE 
                  + ",ISNULL(AUTHNUM, '')" // Auth Number 
                  + ", '' " // Comment 
                            // + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here 
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 12) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "

                 //  + ",CASE " // Response CODE
                 //+ " when Left(TRACE, 1)='2' AND ltrim(rtrim([RESPCODE])) ='112' THEN '0' " // Terminal Type
                 //+ " else ltrim(rtrim([RESPCODE])) "
                 //+ "end "
                 + ",ltrim(rtrim([RESPCODE]))  " // RESPONSE CODE 

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", Left(ltrim(rtrim([Destination])),10)" // GET THE FIRST 10 Characters 
                                                              // For External Date 
                        + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                         //For  Net_TransDate
                         + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"

                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME
                                                       // SET CAP DATE AS Settlememnt date because looks that it is not used
                                                       // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                     + ", CAST([SETTLEMENT_DATE] as date) "
                     + ", CAST([SETTLEMENT_DATE] as date) "


                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE  "
           + " ( "
           + " (left([TXN], 2) IN "
           + "( '10'," // Withdrawl 
           + "  '11'," // Withdrawl 
           + "  '12'," // Withdrawl 
           + "  '13'," // Withdrawl 
           + "  '14'," // Withdrawl
           + "  '15',"
           + "  '20',"
           //+ "  '21',"
           + "  '30',"
           + "  '22',"
           + "  '26',"
           + "  '40',"
           + "  '41'))"
           + " OR (LEFT([TXN], 3) IN "
           + " ('210','213', '214','215' ))"
           + " OR (left([TXN], 1) = '0')" // '0-POS Purchase'
           + " OR (left([TXN], 6) = '810022' AND AMOUNT <> '0')  " // Fawry .. leaving out the '810021'

           //**************************
           //+ " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '5' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry going to COREBANKING
           + " )"
           + " ORDER by TERMID "
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


            //
            // Make a credit card transaction out of tranfers with full card number 
            // Make ALSO Master - where is FAWRY is involved
            // Insert it into TWIN
            // 
            // GO TO BDC FOR DETAILS 
            // 

            // ALREADY COVERED IN IST CREATION
            //**********************************************************
            // UPDATE [TransType] when Reversals
            // 21 for withdrawls 13 for Deposits 
            //**********************************************************
            /*
            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                        + " SET [TransType] = 21 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0  "
                        + "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
                        + " SET [TransType] = 21 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0  "

                        + " UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                         + " SET [TransType] = 13 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0  AND TransDescr = '210000-BNA' "
                        + "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
                        + " SET [TransType] = 13 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0  AND TransDescr = '210000-BNA' "
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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of TransType finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            */


            //
            // CREATE THE IST TWIN
            //
            //

            //
            // INSERT NEW RECORDS IN IST TWIN
            //

            bool IsABE_TWIN = false;

            if (IsABE_TWIN == true)
            {
                SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
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
                 + ") "
                 + " SELECT "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                    // + " ,[MatchingCateg] "
                    + " ,case"
                   + " WHEN (TXNSRC = '1' AND TXNDEST = 'EBC') THEN '" + PRX + "215' " // Kept for EBC

                   + " else 'Not_Def' "

                    + " end "

      //      + " ,case"
      //        + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '"+PRX+"207' " // All outgoing Visa
      //        + " WHEN (TXNSRC = '1' AND TXNDEST = '5')  THEN '"+PRX+"206' " // All outgoing Master
      //        + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '"+PRX+"205' " // All outgoing 123 
      //        + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 2) = '81' AND TransAmt <> '0') THEN '"+PRX+"206' " // All outgoing CREDIT Card Going Through Master Card
      //        + " else 'Not_Def' "
      ////   WHERE TXNSRC = '1' AND TXNDEST = '13' and left([TransDescr], 2) = '81' AND AMOUNT <> '0'
      //         + " end "
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
           //   WHERE TXNSRC = '1' AND TXNDEST = '13' and left([TransDescr], 2) = '81' AND AMOUNT <> '0'
           + " end "

           + " , [Net_TransDate] "
           + ", [Card_Encrypted] "
          + ", [CAP_DATE] "
          + ", [SET_DATE] "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "  // FROM IST

                 + " WHERE  SeqNo > @SeqNo  AND ( "
                   + "  (TXNSRC = '1' AND TXNDEST = 'EBC')  " // All outgoing EBC
                //   + " OR (TXNSRC = '1' AND TXNDEST = '5')  " // 
                     + " ) ";

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

            }

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
            string WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] ";

            HandleReversals_IST(WFile, InOriginFileName, InReconcCycleNo);

            if (ErrorFound == true) return;

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
            + " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) " // Leave it as it is 
            + " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
            + " AND (left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
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

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
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

            //
            // Correct Amount
            // remove not Needed Characters 
            // "2,214.69"
            //SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '\"', '') "
            //         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, ',', '')"
            //         //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_COREBANKING_BULK_Records_2]"
            //         //+ " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '.', '')"
            //         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '-', '')"
            //          + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET [COD_ACCT_NO] = TRIM([COD_ACCT_NO]) "
            //        //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]"
            //        //+ " SET TRAN_DATE = '12/31/2018' "

            //        + " ";

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {
            //            cmd.CommandTimeout = 350;
            //            Counter = cmd.ExecuteNonQuery();
            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        stpErrorText = stpErrorText + "Cancel During Correcting AMOUNT";
            //        stpReturnCode = -1;

            //        stpReferenceCode = stpErrorText;
            //        CatchDetails(ex);
            //        return;
            //    }

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
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransCurr] "
                 + ",[TransAmt] "

                  + ",[TraceNo] "
                 + ",[RRNumber] "

                 + ",[AUTHNUM] "
                 + ",[CardNumber] "
                + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "

                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                     + ",[ACCEPTORNAME] "

                    + " , [Net_TransDate] "
                 //  + " , [CAP_DATE] "
                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                      + ",case "
                + " WHEN LEFT(Merchant_Name,7) = 'BK AUDI' THEN '" + PRX + "201'"   // MATCHING CATEGORY FOR DEBIT Origin ATMS 
                  + " else 'Not_Def' "
                  + "end "
                 // SET MATCHING CATEGORY to 299 
                 //  THEN SET IT TO CORRECT VALUE AFTER YOU GET THE VALUES OF 
                 // TXN SRC AND TXNDST FROM IST

                 + ", ISNULL([TERMINALID], '') " // Terminal 
                 + ", @TerminalType " // Depends from Origin 10 or 20 
                                      // LEFT([DAT_TXN_POSTING],10) = '10/01/2019 00:'
                                      //  + ",ISNULL(try_convert(datetime, [DAT_TXN_POSTING] + ' ' "
                 + ", CAST(ISNULL(try_convert(datetime, [LOCALTRANSACTIONDATETIME], 103 ), '1900-01-01') AS DateTime) "

                    + ",case "
                + " WHEN [TRANSACTIONCODE] = '11' AND [REVERSAL] = '0' THEN '11'  " // Withdrawl
                + " WHEN [TRANSACTIONCODE] = '11' AND [REVERSAL] = '1' THEN '21'  " // Reversal for Withdrawl 
                + " WHEN [TRANSACTIONCODE] = '21' AND [REVERSAL] = '0' THEN '23'  " // Deposit
                + " WHEN [TRANSACTIONCODE] = '21' AND [REVERSAL] = '1' THEN '13'  " // Reversal for Deposit 
                + " WHEN [TRANSACTIONCODE] = '33' AND [REVERSAL] = '0' THEN '33'  " // Transfers
                + " WHEN [TRANSACTIONCODE] = '33' AND [REVERSAL] = '1' THEN '43'  " // Reversal to Transfers
                  + " else '0' "
                  + "end "

                     //                  --  TRANSACTIONCODE = '11'  and Description = 'Debit' and REVERSAL = '0' THEN '11'
                     //--  TRANSACTIONCODE = '11'  and Description = 'Debit' and REVERSAL = '1' THEN '21'
                     //--  TRANSACTIONCODE = '21'  and Description = 'Credit'  and REVERSAL = '0' THEN '23'
                     //--  TRANSACTIONCODE = '21'  and Description = 'Credit'  and REVERSAL = '1' THEN '13'
                     //--  TRANSACTIONCODE = '33'  and Description = 'Credit'  and REVERSAL = '0' THEN '33'
                     //--  TRANSACTIONCODE = '33'  and Description = 'Credit'  and REVERSAL = '1' THEN '43'
                     //                     // The below is for description 
                     + ",case "
                + " WHEN [TRANSACTIONCODE] = '11' AND [REVERSAL] = '0' THEN 'Withdrawl'  " // Withdrawl
                + " WHEN [TRANSACTIONCODE] = '11' AND [REVERSAL] = '1' THEN 'Reversal to Withdrawl' " // Reversal for Withdrawl 
                + " WHEN [TRANSACTIONCODE] = '23' AND [REVERSAL] = '0' THEN 'Deposit'  " // Deposit
                + " WHEN [TRANSACTIONCODE] = '23' AND [REVERSAL] = '1' THEN 'Reversal to Deposit'  " // Reversal for Depost 
                  + " else 'Not Defined' "
                  + "end "

                 + ", CURRENCY_LOCAL "
                 + ", CAST([AMOUNT_LCY] As decimal(18,2)) "
                + " ,  TRACENUMBER " // In place of trace 
                 + ",ISNULL([REFERENCENUMBER] , '') " // in place RRN 

                 + ",  ISNULL(REPLACE(LTRIM(REPLACE(AUTHORIZATIONCODE, '0', ' ')), ' ', '0') , '') " // We put Authorisation code here     
                                                                                                     // REPLACE(LTRIM(REPLACE(AUTHORIZATIONCODE, '0', ' ')), ' ', '0')

                + " , ISNULL([CARDNUMBER] , '')  "                                                                             //+ " , ISNULL([COD_ACCT_NO], '') "
                 + ", ISNULL([ACCOUNTNUMBER] , '') "
                 + ", Case " // Looks that response contains not ascii characters 
                  + " WHEN LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(RESPONSECODE, CHAR(10), CHAR(32)),CHAR(13), CHAR(32)),CHAR(160), CHAR(32)),CHAR(9),CHAR(32)))) = '00' then '0' "
                  + " ELSE LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(RESPONSECODE, CHAR(10), CHAR(32)),CHAR(13), CHAR(32)),CHAR(160), CHAR(32)),CHAR(9),CHAR(32)))) "
                   + " End "

                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'0'" // Origin ATMs -initialise with zero

                  + " ,'0'" // Final Destination -initialise with zero



                 + " ,'' " // for the comment 

                 + " , ISNULL([Merchant_Name] , '')  "

                + " , CAST(ISNULL(try_convert(datetime, [LOCALTRANSACTIONDATETIME], 103 ), '1900-01-01') AS Date) "// NET VALUE 

                 //DAT_TXN_VALUE
                 // + " , CAST(ISNULL(try_convert(datetime, [LOCALTRANSACTIONDATETIME], 103 ), '1900-01-01') AS Date)  " // CAP_DATE ... Value Date 

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                + " WHERE  "
                + " CAST([AMOUNT_LCY] As decimal(18,2)) > 0 "
                + " ORDER BY TERMINALID, [LOCALTRANSACTIONDATETIME]";

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
          + " AND A.AccNo = b.AccNo "
             + " WHERE "
          + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND ("
          + " (A.TransType = 11 AND B.TransType = 21) "
           + " OR (A.TransType = 23 AND B.TransType = 13) "
            + " OR (A.TransType = 33 AND B.TransType = 43) "
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

            //CORRECTION1950
            // HERE READ ALL ATM DEPOSITS WIth FULL TRACE NUMBER STARTING WITH 2 PROCESSED AT THIS CYCLE AND UNDO THEM 
            // MAKE THEM NOT PROCESSED
            // LEAVE THE REVERSAL COMMENT 
            // WRITE A METHOD BELOW 

            bool CORRECTION1950 = true;
            if (CORRECTION1950 == true) ;
            Mg.UpdateNonProcessedDepositReversals(TableId, InReconcCycleNo);

            //
            stpErrorText += DateTime.Now + "_" + "COREBANKING Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

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


        ///
        //// CREATE 123_NET
        ///
        public void InsertRecords_GTL_123_NET(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_123_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] 

            string SQLCmd;


            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table 123 Net FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During 123 BULK Truncate";
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

                    stpErrorText = stpErrorText + "Cancel During 123 Bulk Insert";
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
            RecordFound = false;

            //       
            // CREATE EGYPT 123 from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]  "
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransAmt] "
                 + ",[RRNumber] "
                 + ",[FullTraceNo] "
                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ",[Comment] "
                     + ",[Net_TransDate] "
                     + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " ,case" // Destination // 3
                 + " WHEN ACQ = '" + BKName + "' THEN '" + PRX + "215' " // All Outgoing
                 + " WHEN ISSUER = '" + BKName + "' AND  (LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460')   THEN '" + PRX + "210' " // ALL Debit 544460******7103   
                 + " WHEN ISSUER = '" + BKName + "' AND LEFT([PAN],6) Not In ('526402','544460')  THEN '" + PRX + "211' "
                 // + " WHEN ISSUER = 'BDC' AND ( LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '507803' ) THEN 'BDC210' " // ALL Debit
                 //+ " WHEN ISSUER = 'BDC' AND LEFT([PAN],6) <> '526402' AND LEFT([PAN],6) <> '507803' THEN 'BDC211' "
                 + " else 'Not_DEFINED' "
                 + " end "

                 + ", @TerminalType"
                 + ", CAST([TRANS_DATE] As datetime) + CAST(TRANS_TIME As datetime) "
                 + ",case "
                 + " when (TYP = 'Transaction' AND [TRANSACTIO] = 'Withdrawl') THEN 11  "
                 + " when (TYP = 'Reversal' AND [TRANSACTIO] = 'Withdrawl') THEN 21 "
                 + " else 0 "
                 + "end "
                 + ",[TRANSACTIO] "
                 + ",CAST([AMT1] As decimal(18,2)) "
                 + ",[SEQ_NUM] "
                 + ",[SEQ_NUM] "
                   + " ,case"
                 + " WHEN LEFT([ATM_ID],3) = '" + BKName + "' THEN Right([ATM_ID],8) "
                 + " else LEFT([ATM_ID],8) "
                 + " end "
                 // + ",[PAN] "  // 123NET LEFT([ATM_ID],3)

                 + " ,LEFT([PAN],6) + '******' + Right([PAN],4) "
                 + ",[ACCOUNT_TY] " // 123_NET checking  etc
                                    //  + ",ltrim(rtrim([RESP_CODE]))  "
                 + " ,case"
                 + " WHEN ltrim(rtrim([RESP_CODE])) = '000' THEN '0' " // All Outgoing [TXNSRC]
                 + " else ltrim(rtrim([RESP_CODE])) "
                 + " end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                   + " ,case"
                 + " WHEN ACQ = '" + BKName + "' THEN '1' " // All Outgoing [TXNSRC]
                 + " else '8' " // 123 txns incoming 
                 + " end "
                     + " ,case"
                 + " WHEN (ACQ <> '" + BKName + "' AND ISSUER = '" + BKName + "') AND LEFT([PAN],6) = '526402' THEN '1' " // ALL Debit
                 + " WHEN (ACQ <> '" + BKName + "' AND ISSUER = '" + BKName + "') AND LEFT([PAN],6) <> '526402' THEN '12' "
                 + " WHEN (ACQ = '" + BKName + "' ) THEN '8' " // Leave as this to cover ISSUER = 'HDB'
                                                               // + " WHEN (ACQ = 'BDC' AND ISSUER <> 'BDC') THEN '8' " // TXNDEST
                 + " else '29' " // 
                 + " end "
                 + " ,[RESPONSE_C]"
                  //+ ", ltrim(rtrim([TXNDEST]))"
                  // For Net_TransDate
                  + ", CAST([TRANS_DATE] As date) "
                  + ", CAST([SETTLE_DAT] As date) "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE  "
                + " [TRANSACTIO] = 'Withdrawl' "

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Egypt 123 Net" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During 123 insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in 123 with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            //
            // UPDATE Details from IST Based on RRNumber and amnt 
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] "
          + " SET"

          + " TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "

          + ",Card_Encrypted = t2.Card_Encrypted "
           + ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "
         // + ",TransDate = t2.TransDate "
         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "
           + ",CAP_DATE = t2.CAP_DATE "
          //  + ",Net_TransDate = t2.Net_TransDate "
          // + ",TransCurr = t2.TransCurr "

          + ",AmtFileBToFileC = 999.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.CardNumber  = t2.CardNumber  "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

             + " WHERE  "
           + " ( t1.Processed = 0  AND t1.MatchingCateg in ( '" + PRX + "210', '" + PRX + "211' ) ) "
  + " AND ( "
  + " (t2.Processed = 0 AND t2.MatchingCateg in ( '" + PRX + "210', '" + PRX + "211' ) ) "

  + "   OR ( "
  + " (t2.Processed = 1 AND t2.MatchingCateg in ( '" + PRX + "210', '" + PRX + "211' ) "
  + "                AND t2.Comment = 'Reversals' AND t2.Net_TransDate> @Net_Minus ) "
  + "		 ) "
  + "		 OR ( "
  + "	 (t2.Processed = 1 AND t2.MatchingCateg in ( '" + PRX + "210', '" + PRX + "211' ) "
  + "                AND t2.ResponseCode <> '0' AND t2.Net_TransDate> @Net_Minus ) "
 + " 	 ) "
    + "			  ) "

          ; // 123  
            // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating_NOT Equal EMR215";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "1_Card No and Origin Updated from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Details from IST Based on RRNumber and amnt 
            // FOR Matching Categ BDC215

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] "
          + " SET"
          //  " CardNumber = t2.CardNumber "
          + " TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + ",TransDescr = t2.TransDescr "
          + ",Card_Encrypted = t2.Card_Encrypted "

         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "
         + ",CAP_DATE = t2.CAP_DATE "

          + ",AmtFileBToFileC = 9999.99 " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.TerminalId = t2.TerminalId "
          // + " AND t1.CardNumber  = t2.CardNumber  "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "215' ) "
                //+ " AND (t1.MatchingCateg = 'BDC215' ) "
                //+ "   AND t1.ResponseCode = '0'  "
                + "AND ((t2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "215' )  "
           + "      OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "215'  AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
                ; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating_=EMR215";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "2_Card No and Origin Updated from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Update 123 as processed the ones with errors
            //

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] "
                + " SET "
                + " [Processed] = 1 "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'With Response code not 0 ' "
                 + " WHERE  "
               + "  Processed = 0 and ResponseCode <> '0' " // Non Processed 
             ; // 

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

                    return;
                }

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // EGYPT 123 NET

            // INSERT In REVERSALS for 123 NET  
            string FileId = InOriginFileName;

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
          + "  @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND (A.TransType = 21 AND B.TransType = 11)  "
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
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for 123 NET" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_'123'_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //FileId = "COREBANKING";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Egypt_123";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "123 Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }

        //*******************
        //// CREATE MEEZA_OWN_ATMS
        /////****************
        public void InsertRecords_GTL_MEEZA_OWN_ATMS(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MEEZA_OWN_ATMS_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] 

            string SQLCmd;


            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table MEEZA_OWN_ATMS FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS BULK Truncate";
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS Bulk Insert";
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

            RecordFound = false;

            // Correct error dates 

            SQLCmd = " UPDATE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " SET "
                          + "  EMVTransactionDate = TransactionDate     "
                          + " WHERE TransactionType = 'Withdrawal' AND ResponseCode = 'Approved or completed Successfully' "
                          + "AND (EMVTransactionDate = '000000' OR EMVTransactionDate<'190820' ) " // THESE ARE ERRORS IN FILE
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS During Date Updating";
                    MessageBox.Show(stpErrorText + "..Please Check..." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Error Updating finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //       
            // CREATE MEEZA_OWN_ATMS from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS]  "
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

                 + ",[TerminalId] "
                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     // + ",[Comment] "
                     + ",[Net_TransDate] "
                     + ", SET_DATE "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " ,'" + PRX + "275' " // Meeza done on our ATMs
                                         //" , CAST(EMVTransactionDate as Date) "
                 + ", @TerminalType"
                // + ", CAST([TransactionDate] As datetime) " // Transaction Date without minutes 
                + " , CAST(EMVTransactionDate as datetime) "
                 + ",case "
                 + " when (MessageType = 'Original' AND TransactionType = 'Withdrawal') THEN 11  "
                 + " when (MessageType = 'Reversal'  AND TransactionType = 'Withdrawal') THEN 21  "
                 + " when (MessageType = 'Original' AND TransactionType = 'Deposit') THEN 23  "
                 + " when (MessageType = 'Reversal'  AND TransactionType = 'Deposit') THEN 13  "
                 + " else 0 "
                 + "end "
                 + ", TransactionType "
                 + ",'EGP'"
                 + ",CAST(TransactionAmount As decimal(18,2)) "
                 + ",[SystemTraceAuditNumber] " // TRace 
                 + ",[RRN] " // RRN 
                 + ",  right (terminalId,8) " // Terminal Id
                 + ", ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "  // CARD  
                                                                                 //+ " , [PAN] "
                                                                                 //
                 + " ,case"
                 + " WHEN ResponseCode = 'Approved or completed Successfully' THEN '0' " // All Outgoing [TXNSRC]
                 + " else '' "
                 + " end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'1' "   // Origin from ATMs 
                     + ",'18' " // Destination Meeza
                                //+ " ,"

                  + ", CAST(EMVTransactionDate as date) "
               //     + ", DATEADD(DAY, -1, Cast(SettlementDate as DATE)) "
               + ", CAST(SettlementDate as DATE) "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 // GET Only the valid ones
                 + " WHERE (TransactionType = 'Withdrawal' AND ResponseCode = 'Approved or completed Successfully' ) "
                 + " OR  (TransactionType = 'Deposit' AND ResponseCode = 'Approved or completed Successfully')  "
                + " ";
            // Deposit
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
                        // cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA EMR ATMS" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During MEEZA EMR ATMS insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in MEEZA OWN ATMS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //return; 


            //
            // UPDATE Date with IST Date
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] "
          + " SET"

          + " [TransDate] = t2.[TransDate] "
          + " ,[TransCurr] = t2.[TransCurr] "
          + ",CAP_DATE = t2.CAP_DATE " // 

          + ",AmtFileBToFileC = 9.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.TransType = t2.TransType "

          + " WHERE  "
          + "(t1.Processed = 0  AND t1.MatchingCateg = '" + PRX + "275') "
          + " AND (t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals') ) AND  t2.MatchingCateg = '" + PRX + "275'"
          ; // For not processed yet records

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS Updating For date and time";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Date and Time from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MEEZA_OWN_ATMS

            // INSERT In REVERSALS for MEEZA_OWN_ATMS
            string FileId = InOriginFileName;

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
          + "  @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND ((A.TransType = 21 AND B.TransType = 11) OR (A.TransType = 23 AND B.TransType = 13)) "
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
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for MEEZA_OWN_ATMS" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_OWN_ATMS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //FileId = "COREBANKING";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_OWN_ATMS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "MEEZA_OWN_ATMS Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }

        //*******************
        //// CREATE MEEZA_OTHER_ATMS
        /////****************
        public void InsertRecords_GTL_MEEZA_OTHER_ATMS(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MEEZA_OTHER_ATMS_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] 

            string SQLCmd;


            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table MEEZA_OTHER_ATMS FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OTHER_ATMS BULK Truncate";
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OTHER_ATMS Bulk Insert";
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

            // Correct error dates 

            SQLCmd = " UPDATE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " SET "
                          + "  EMVTransactionDate = TransactionDate     "
                          + " WHERE ResponseCode = 'Approved or completed Successfully' "
                          + "AND (EMVTransactionDate = '000000' OR EMVTransactionDate<'190820' ) " // THESE ARE ERRORS IN FILE
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS During Date Updating";
                    MessageBox.Show(stpErrorText + "..Please Check..." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Error Updating finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            RecordFound = false;

            //       
            // CREATEMEEZA_OTHER_ATMS from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS]  "
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
                     + ",[Net_TransDate] "
                     + ", [SET_DATE]"
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " ,'" + PRX + "270' " // Meeza OTHER ATMs - It can be BDC270(Debit) or BDC271(Prepaid) 

                 + ", @TerminalType"
                 // + ",CAST(TransactionDate as Date)" // This is not the IST date 
                 + ", CAST([EMVTransactionDate] As datetime) " // Transaction Date without minutes.. This is the IST date 
                 + ",case "
                 + " when (MessageType = 'Original' AND TransactionType = 'Withdrawal' ) THEN 11  "
                 + " when (MessageType = 'Reversal' AND TransactionType = 'Withdrawal' ) THEN 21  "
                 + " when (MessageType = 'Original' AND TransactionType = 'Deposit' ) THEN 23  "
                 + " when (MessageType = 'Reversal' AND TransactionType = 'Deposit' ) THEN 13  "
                 + " else 0 "
                 + "end "
                 + ", TransactionType "
                 + ",'EGP'"
                 + ",CAST(TransactionAmount As decimal(18,2)) "
                 + ", 0 " // TRace to be updated from I
                 + ",RRN  " // RRN + Authorisation Number
                 + ", AuthorizationNumber"
                 + ",  Left(TerminalId,8) " // Terminal Id

                 + " ,case"
                 + " WHEN Left(PAN, 6 )= '507808' THEN Left(PAN, 6 )+ '00' + '****' + right([PAN], 4) " // 
                 + " else ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "
                 + " end "

                 //+ ", ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "  // CARD  
                 //                                                                // + " , [PAN] "
                 //                                                                //
                 + " ,case"
                 + " WHEN ResponseCode = 'Approved or completed Successfully' THEN '0' " // All Outgoing [TXNSRC]
                 + " else '' "
                 + " end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'18' "   // Origin Meeza
                     + ",'1' " // Destination COREBANKING - it can be prepaid 
                               //+ " ,"
                               // + ",CAST(TransactionDate as Date)" This not the IST date 
                  + ", CAST([EMVTransactionDate] As DATE) " // This is the IST date 
                                                            //  + ", DATEADD(DAY, -1, Cast(SettlementDate as DATE)) "
                     + ", CAST(SettlementDate as DATE) "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 // GET Only the valid ones
                 + " WHERE (TransactionType = 'Withdrawal' OR TransactionType = 'Deposit' ) AND ResponseCode = 'Approved or completed Successfully'  "
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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_OTHER_ATMS" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OTHER_ATMS insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in MEEZA_OTHER_ATMS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Date with IST Date
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] "
          + " SET"
            + " [MatchingCateg] = t2.[MatchingCateg] "
           //+ ", [TransType] = t2.[TransType] "
           + " ,[TransDate] = t2.[TransDate] "
           + " ,[TransCurr] = t2.[TransCurr] "
           + " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
            + " ,[TXNDEST] = t2.[TXNDEST] "
            + ",CAP_DATE = t2.CAP_DATE "

          + " ,AmtFileBToFileC = 9.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          + "  t1.RRNumber = t2.RRNumber "
          + " AND t1.AUTHNUM = t2.AUTHNUM "
          + " AND t1.TransType = t2.TransType "

          + " WHERE  "
          + "(t1.Processed = 0  AND t1.MatchingCateg in ('" + PRX + "270', '" + PRX + "271')) " // in previous we have set all bdc270
          + " AND (t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus  ) ) "
          + "AND  t2.MatchingCateg in ('" + PRX + "270', '" + PRX + "271')"
          ; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OWN_ATMS Updating For date and time";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Date and Time from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MEEZA_OTHER_ATMS

            // INSERT In REVERSALS for MEEZA_OTHER_ATMS
            string FileId = InOriginFileName;

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
          + "  @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND ((A.TransType = 21 AND B.TransType = 11) OR (A.TransType = 23 AND B.TransType = 13)) "
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
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for MEEZA_OTHER_ATMS" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_OTHER_ATMS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //FileId = "COREBANKING";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_OTHER_ATMS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "MEEZA_OTHER_ATMS Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }

        //*******************
        //// CREATE MEEZA_POS
        /////****************
        public void InsertRecords_GTL_MEEZA_POS(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MEEZA_POS_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] 

            string SQLCmd;


            string WTerminalType = "20";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table MEEZA_POS FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_POS BULK Truncate";
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_POS Bulk Insert";
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

            RecordFound = false;

            //       
            // CREATE MEEZA_POS from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS]  "
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

                 + " ,'" + PRX + "273' " // MEEZA_POS - It can be BDC272(Debit) or BDC273(Prepaid) 

                 + ", @TerminalType"
                 // + ", CAST([AuthorizationDate] As datetime) " // Transaction Date without minutes 
                 + ", CAST([AcquirerDate] As datetime) " // Transaction Date without minutes 

                 + ",case "
                 + " when (MessageType = 'Original') THEN 11  "
                 + " when (MessageType = 'Reversal') THEN 21  "
                 + " else 0 "
                 + "end "
                 + ", 'MEEZA POS TXN' "
                 + ",TransactionCurrencyCode"
                 + ",CAST(TransactionAmount As decimal(18,2)) "
                 + ", 0 " // TRace to be updated from I
                 + ",RRN  " // RRN + Authorisation Number
                 + ", AuthorizationNumber"
                 + ",  TerminalId " // Terminal Id
                 + ", ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "  // CARD  
                                                                                 // + " , [PAN] "
                                                                                 //
                 + ", '0'" // Response Code
                           //+ " ,case"
                           //+ " WHEN ResponseCode = 'Approved or completed Successfully' THEN '0' " // All Outgoing [TXNSRC]
                           //+ " else '' "
                           //+ " end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'18' "   // Origin Meeza
                     + ",'12' " // Destination prepaid - it can be COREBANKING
                                //+ " ,"
                  + ", CAST(AuthorizationDate As datetime) " // For external date 
                  + ", CAST([AcquirerDate] As date) "

                   //  + ", DATEADD(DAY, -1, Cast(ClearingSettlementDate as DATE)) " // DATE OF ARRIVING
                   + ", CAST(ClearingSettlementDate as DATE) "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // GET Them ALL with Authorisation number
                //  FILE Contains all Authorised 
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
                        //cmd.Parameters.AddWithValue("@ReversedCut_Off_Date", ReversedCut_Off_Date);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_POS" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During MEEZA_POS insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in MEEZA_POS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Date with IST Date
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
          + " SET"
            + " [MatchingCateg] = t2.[MatchingCateg] "
           + " ,[TransDate] = t2.[TransDate] "
           + " ,[TransCurr] = t2.[TransCurr] "
           + " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
           + " ,[TXNDEST] = t2.[TXNDEST] "
           + " ,[EXTERNAL_DATE] = t2.[EXTERNAL_DATE] "
           + " ,CAP_DATE = t2.CAP_DATE " // We already inserted the clearing date
           + " ,[Net_TransDate] = t2.[Net_TransDate] "

          + " ,AmtFileBToFileC = 9.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          + "  t1.RRNumber = t2.RRNumber "
          + " AND t1.AUTHNUM = t2.AUTHNUM "
          + " AND t1.TransType = t2.TransType "

          + " WHERE  "
          + "(t1.Processed = 0  AND t1.MatchingCateg = '" + PRX + "273') " // As set during loading
          + " AND "
          + "(t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus ) )"
          + " AND  t2.MatchingCateg in ('" + PRX + "272', '" + PRX + "273')"
          ; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_POS Updating For date and time";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Date and Time from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MEEZA_POS

            // INSERT In REVERSALS for MEEZA_POS
            string FileId = InOriginFileName;

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
          + "  @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND (A.TransType = 21 AND B.TransType = 11)  "
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
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for MEEZA_POS" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_POS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;

            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_POS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "MEEZA_POS Finishes with SUCCESS.." + "\r\n";

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

        //*******************
        //// CREATE FAWRY
        /////****************
        public void InsertRecords_GTL_FAWRY(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_123_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] 

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

                    stpErrorText = stpErrorText + "Cancel During FAWRY BULK Truncate";
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

                    stpErrorText = stpErrorText + "Cancel During FAWRY Bulk Insert";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert for FAWRY finishes with.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            RecordFound = false;

            //       
            // CREATE FAWRY from BULK
            //

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[FAWRY]  "
                + "( "
                 + " [OriginFileName] "

                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ", [TransCurr]  "
                 + ",[TransAmt] "
                 + ",[TraceNo] "
                 + ",[RRNumber] "
                 + ",[FullTraceNo] "
                 + ",[TerminalId] "
                 + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ",[Comment] "
                     + ",[Net_TransDate] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " , '" + PRX + "251' " // Matching Category 

                 + ", @TerminalType"

                 + " , Cast((RIGHT(TransDate, 4) + '' + LEFT(TransDate, 2)+ '' + SUBSTRING(TransDate, 4, 2) ) as datetime) + CAST(TransTime As datetime) "

                 + ",case "
                 + " WHEN ltrim(rtrim([Reconciliation_Group])) = 'Successful'  THEN 11  " // FAWRY
                 + " WHEN ltrim(rtrim([Reconciliation_Group])) = 'ToReverse'  THEN 21  "
                 + " WHEN ltrim(rtrim([Reconciliation_Group])) = 'ReversedOL'  THEN 21  "
                 + " else 0 "
                 + "end "
                 + ",FawryTransStatus + '_' +  Reconciliation_Group + '_Ref' + FawryRefNo "

                 + ", 'EGP' "
                 + " ,CAST([TransAmt] As decimal(18,2)) + CAST([CustomerFees] As decimal(18,2))  "
                 + ", 0 " // There is no trace number -- set it to zero
                          // + ",Cast([RectNo] as int) " // TRACE NUMBER
                 + ",[DrRefNo]" // THIS THE AUTHORISATION NUMBER that Goes to RRNumber 
                 + ",[BillType_Code]+'_'+ [FawryRefNo] " // FULL TRACE NUMBER 

                 + ",[TermNo]  " // FAWRY BDC TERMINAL 

                 + ",[BillingAC] " // FAWRY ACCOUNT NO

                 + " ,case"
                 + " WHEN ltrim(rtrim([Reconciliation_Group])) = 'Successful' THEN '0' " // FAWRY RESPONSE CODE
                 + " else ltrim(rtrim([Reconciliation_Group])) "
                 + " end "
                  + ", @LoadedAtRMCycle"
                 + ", @Operator"

                  + " ,'1' " // [TXNSRC]
                  + " ,'12' " // [TXNDEST]

                 + " , '' " // FAWRY leave empty 

                 + " , Cast((RIGHT(TransDate, 4) + '' + LEFT(TransDate, 2)+ '' + SUBSTRING(TransDate, 4, 2) ) as datetime)  "

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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Egypt 123 Net" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During 123 insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in FAWRY with records.." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Details from IST Based on RRNumber and amnt 
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);
            //
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] "
          + " SET "
          + " CardNumber = t2.CardNumber "
          + " ,Card_Encrypted = t2.Card_Encrypted "
          + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "
           + ",CAP_DATE = t2.CAP_DATE "
          + " ,AmtFileBToFileC = 9.99 " // indication that this record is updated 
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"
          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.RRNumber = t2.RRNumber "  // THIS IS THE AUTHORISATION NUMBER
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.Net_TransDate = t2.Net_TransDate "
          + " WHERE  t1.Processed = 0 "
            + "AND ((t2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "251')  "
           + "      OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "251' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
          ; //  
            ; // For not processed yet records

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating_For_FAWRY ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FAWRY Card No  Updated from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // FAWRY

            string FileId = InOriginFileName;

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
          + "  @FileId "
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
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
           + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND (A.TransType = 21 AND B.TransType = 11) "
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
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for FAWRY" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _FAWRY_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert Reversals.." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            //FileId = "FAWRY";
            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[FAWRY]";

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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_FAWRY";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "FAWRY FINISHES with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }
        //
        //// CREATE MASTER CARD FILEs 
        //
        public void InsertRecords_GTL_MASTER(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            // stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MASTER_BULK_Records]
            // "[RRDM_Reconciliation_ITMX].[dbo].[Master_Card]"

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table MASTER FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During TRuncate ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // 
                                                                           //+ " ,ROWs_PER_BATCH=15000 "
                          + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',' "
                          //+ ",FIELDTERMINATOR = '\t'"
                          + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "BULK INSERT FINISHES with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // TEST LIMIT


            //
            // Correct Amount
            // remove not Needed Characters 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET SETTLEMENT_AMOUNT = replace(SETTLEMENT_AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET SETTLEMENT_AMOUNT = replace(SETTLEMENT_AMOUNT, '-', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET SETTLEMENT_AMOUNT = replace(SETTLEMENT_AMOUNT, ',', '')";
            //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_MASTER_BULK_Records_2]"
            //+ " SET AMOUNT = replace(AMOUNT, '.', '')";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Correct AMOUNT";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            //return;

            //RecordFound = false;

            // CREATE MASTER from BULK
            //

            // Truncate Table MASTER

            //
            // Change date format and insert in Master 
            //

            SQLCmd =
                 //"SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Master_Card]"
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
                 + ",[FullTraceNo] "
                 + ",[TerminalId] "
                 + ",[CardNumber] "

                 + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                    + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                     + " , [Net_TransDate] "
                     + " , [Card_Encrypted] "
                     + " , [SET_DATE] "
                 + ") "
                 //
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN '" + PRX + "235' " // All Outgoing
                 + " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN '" + PRX + "230' " // All DEBIT
                 + " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532' ) THEN '" + PRX + "232' " // All PREPAID
                 + " else '" + PRX + "999' "
                 + " end "
                 + ", @TerminalType"

                 + ", CAST([TRANDATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"
                 + ",case "
                 + " when Cast (SETTLEMENT_AMOUNT as decimal(18,2)) > 0 THEN 11  "
                 //  + " when (TYP = 'Reversal' AND [TRANSACTIO] = 'Withdrawl') THEN 21 "
                 + " else 0 "
                 + "end "
                 //  in place of Transction Description 
                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN 'EMR ATM Withdrawl' " // All Outgoing
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN 'EMR Card Used for Withdrawl-DEBIT' " // All DEBIT
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532') THEN 'EMR Card Used for Withdrawl-PREPAID' " // All PREPAID
                 + " else '" + PRX + "999' "
                 + " end "

                 + ", ISNULL(LEFT([SETTLEMENT_CODE],3), '') "
                  + " ,case"
                 + " WHEN ACQISS = 'A' THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // All Outgoing [TXNSRC]
                 + " WHEN ACQISS = 'I' THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // ALL Incoming
                 + " else 0 "
                 + " end "

                 //+ ", ISNULL(LEFT([ACQ_CURRENCY_CODE],3), '') "
                 // + " ,case"
                 //+ " WHEN ACQISS = 'A' THEN CAST([AMOUNT] As decimal(18,2)) " // All Outgoing [TXNSRC]
                 //+ " WHEN ACQISS = 'I' THEN CAST([AMOUNT] As decimal(18,2)) " // ALL Incoming
                 //+ " else 0 "
                 //+ " end "
                 + ",[TRACE] "
                 + ",[REFNUM] "
                 + ",[REFNUM] "
                 + " ,[TERMID] "
                 // + ",[PAN] "  // 
                 + " ,LEFT([PAN],6) + '******' + Right([PAN],4) "
                 + ",[MSGTYPE] " // FOR ACCOUNT NUMBER                 // 
                 + " ,[RESPCODE] "
                 + ", @LoadedAtRMCycle"
                    + ", @Operator"
                   + " ,case"
                 + " WHEN ACQISS = 'A' THEN '1' " // All Outgoing [TXNSRC]
                 + " WHEN ACQISS = 'I' THEN '5' " // ALL Incoming
                 + " else '99' "
                 + " end "

                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN '5' " // TXNDEST
                 + " WHEN ( ACQISS = 'I' AND ((LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876')) ) THEN '1'  " // All DEBIT
                 + " WHEN (ACQISS = 'I' AND (LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532')) THEN '12' " // All PREPAID
                 + " else '99' "
                 + " end "
                 + " ,'' " // Blank for the comment 
                           // Net date
                + ", CAST([TRANDATE] as date) "
                + ", PAN "
                + ", SETTLEMENT_DATE "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE  "
                + " CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) > 0 AND MSGTYPE = 'FREC' "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);
                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Master" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT IN MASTER CARD";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT MASTER FINISHES with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MASTER CARD
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
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " WHERE  "
          + " (A.Processed = 0 AND B.Processed = 0) "
          + " AND (A.SeqNo <> B.SeqNo) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR MASTER.." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MASTER_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Reversals created  with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE REVERSALS for  (ORIGINAL RECORD)
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;

            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Master_Card]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_MASTER CARD";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "MASTER CARD Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }

        //
        //// CREATE MASTER CARD FILEs 
        //
        public void InsertRecords_GTL_MASTER_XXX(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            // stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MASTER_BULK_Records]
            // "[RRDM_Reconciliation_ITMX].[dbo].[Master_Card]"

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Table MASTER FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During TRuncate ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // 
                                                                           //+ " ,ROWs_PER_BATCH=15000 "
                          + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',' "
                          //+ ",FIELDTERMINATOR = '\t'"
                          + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "BULK INSERT FINISHES with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // TEST LIMIT


            //
            // Correct Amount
            // remove not Needed Characters 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '-', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, ',', '')";
            //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_MASTER_BULK_Records_2]"
            //+ " SET AMOUNT = replace(AMOUNT, '.', '')";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Correct AMOUNT";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            return;

            //RecordFound = false;

            // CREATE MASTER from BULK
            //

            // Truncate Table MASTER

            //
            // Change date format and insert in Master 
            //

            SQLCmd =
                 //"SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Master_Card]"
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

                 + ",[RRNumber] "
                 + ",[FullTraceNo] "
                 + ",[TerminalId] "
                 + ",[CardNumber] "

                 + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                    + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                     + " , [Net_TransDate] "
                     + " , [Card_Encrypted] "
                     + " , [SET_DATE] "
                 + ") "
                 //
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN '" + PRX + "235' " // All Outgoing
                 + " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN '" + PRX + "230' " // All DEBIT
                 + " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532' ) THEN '" + PRX + "232' " // All PREPAID
                 + " else '" + PRX + "999' "
                 + " end "
                 + ", @TerminalType"

                 + ", CAST([TRANDATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"
                 + ",case "
                 + " when Cast (AMOUNT as Decimal) > 0 THEN 11  "
                 //  + " when (TYP = 'Reversal' AND [TRANSACTIO] = 'Withdrawl') THEN 21 "
                 + " else 0 "
                 + "end "
                 //  in place of Transction Description 
                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN 'EMR ATM Withdrawl' " // All Outgoing
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876') THEN 'EMR Card Used for Withdrawl-DEBIT' " // All DEBIT
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532') THEN 'EMR Card Used for Withdrawl-PREPAID' " // All PREPAID
                 + " else '" + PRX + "999' "
                 + " end "
                 + ", ISNULL(LEFT([ACQ_CURRENCY_CODE],3), '') "
                  + " ,case"
                 + " WHEN ACQISS = 'A' THEN CAST([AMOUNT] As decimal(18,2)) " // All Outgoing [TXNSRC]
                 + " WHEN ACQISS = 'I' THEN CAST([AMOUNT] As decimal(18,2)) " // ALL Incoming
                 + " else 0 "
                 + " end "
                 + ",[REFNUM] "
                 + ",[REFNUM] "
                 + " ,[TERMID] "
                 // + ",[PAN] "  // 
                 + " ,LEFT([PAN],6) + '******' + Right([PAN],4) "
                 + ",[MSGTYPE] " // FOR ACCOUNT NUMBER                 // 
                 + " ,[RESPCODE] "
                 + ", @LoadedAtRMCycle"
                    + ", @Operator"
                   + " ,case"
                 + " WHEN ACQISS = 'A' THEN '1' " // All Outgoing [TXNSRC]
                 + " WHEN ACQISS = 'I' THEN '5' " // ALL Incoming
                 + " else '99' "
                 + " end "

                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN '5' " // TXNDEST
                 + " WHEN (ACQISS = 'I' AND LEFT([PAN],6) = '526402') THEN '1' " // All DEBIT
                 + " WHEN (ACQISS = 'I' AND LEFT([PAN],6) = '517582') THEN '12' " // All PREPAID
                 + " else '99' "
                 + " end "
                 + " ,[RESPCODE]" // for the comment 
                                  // Net date
                + ", CAST([TRANDATE] as date) "
                + ", PAN "
                + ", SETTLEMENT_DATE "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE  "
                + " CAST([AMOUNT] As decimal(18,2)) > 0 AND MSGTYPE = 'FREC' "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);
                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Master" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT IN MASTER CARD";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT MASTER FINISHES with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //  return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MASTER CARD
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
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] B "
          + " ON A.TerminalId = B.TerminalId "
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " WHERE  "
          + " (A.Processed = 0 AND B.Processed = 0) "
          + " AND (A.SeqNo <> B.SeqNo) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR MASTER.." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MASTER_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Reversals created  with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE REVERSALS for  (ORIGINAL RECORD)
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;

            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Master_Card]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_MASTER CARD";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "MASTER CARD Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }

        //
        //// CREATE VISA CARD FILEs V02
        //
        public void InsertRecords_GTL_VISA(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records_OneField_V2]
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;


            // Truncate Table InTableB_OneField

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

                    stpErrorText = stpErrorText + "Cancel During TRUNCATE";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            // BULK INSERT ONE FIELD

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=1,TABLOCK,CODEPAGE ='1253'  " // 
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During BULK INSERT 1";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FIRST LEVEL BULK Finished with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Truncate Table VISA FOR BULK

            SQLCmd = "TRUNCATE TABLE [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2";

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

                    stpErrorText = stpErrorText + "Cancel During Truncate 2";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // CREATE BULK VISA FROM ONE RECORD BULK

            SQLCmd =
                        //"SET dateformat dmy "
                        " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                        //  + InTableB
                        + "( "
                         + " [BAT_Date_Time] "
                         + " ,[BAT_NUM] "
                        + " , [Date_Time] "

                         + " ,[CARD] "

                         + " ,[RRN] "
                         + " ,[TRACE] "
                         + ",[ISSUER] "
                         + ",[TRAN_TYPE] "

                         + ",[PROCESS_CODE] "
                         + ",[ENT_MOD] "
                         + ",[REAS_CODE] "
                          + ",[CN_CODE] "

                         + ",[RSP_CD] "
                         + ",[AMOUNT] "
                         + ",[CUR] "
                         + ",[SETTL_AMOUNT] "

                         + ",[SETTL_AMOUNT_Code] "
                         + ") "

                         + " SELECT "
                         + " BAT_Date_Time"
                          + " ,case " // Month name 
                             + " when TRAN_TYPE = '0422'  THEN substring(BAT_Date_Time,1, 3)  "
                             + " else substring(BAT_Date_Time,1, 2) "
                             + "end "

                               + ",case " // Month name 
                             + " when TRAN_TYPE = '0422'  THEN substring(BAT_Date_Time, 5, 17)  "
                             + " else substring(BAT_Date_Time, 4, 17) "
                             + "end "

                         //+ " ,substring(BAT_Date_Time, 4, 2) " // DD
                         //+ "  ,substring(BAT_Date_Time, 6, 3) " // Date NAME
                         //    + ",case " // Month name 

                         //    + "end "
                         //+ " , @YEAR"
                         //+ " ,substring(BAT_Date_Time, 10, 8) " // time 

                         + " ,[CARD] "

                         + " ,[RRN] "
                         + " ,[TRACE] "
                         + ",[ISSUER] "
                         + ",[TRAN_TYPE] "

                         + ",[PROCESS_CODE] "
                         + ",[ENT_MOD] "
                         + ",[REAS_CODE] "
                          + ",[CN_CODE] "

                         + ",[RSP_CD] "
                         + ",[AMOUNT] "
                         + ",[CUR] "
                         + ",[SETTL_AMOUNT] "

                         + ", right(SETTL_AMOUNT, 2) "

                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName;


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
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Visa Creation";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "SECOND LEVEL BULK Finished with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // DELETE NULL LINES

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " WHERE BAT_NUM IS NULL";

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

                    stpErrorText = stpErrorText + "Cancel During DELETE NULLS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            //
            // Correct Amount
            // remove not Needed Characters 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET AMOUNT = replace(AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET AMOUNT = replace(AMOUNT, '-', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET AMOUNT = replace(AMOUNT, ',', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records_2]"
                    //+ " SET TRAN_DATE = '12/31/2018' "
                    //+ " WHERE TRAN_DATE = '31DEC' "
                    // + " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]" 
                    //+ " SET TRAN_DATE = '01/01/2019' "
                    //+ " WHERE TRAN_DATE = '01JAN' "
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

            //
            // Create Date 
            // 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET  "
                    + "  [DD] = substring(Date_Time, 1, 2) " // DD
                    + " ,[Month_Name] = substring(Date_Time, 3, 3) " // Date NAME
                    + " ,[MM] = "
                        + "case " // Month name 
                        + " when substring(Date_Time, 3, 3) = 'JAN' THEN '01'  "
                        + " when substring(Date_Time, 3, 3) = 'FEB' THEN '02'  "
                        + " when substring(Date_Time, 3, 3) = 'MAR' THEN '03'  "

                        + " when substring(Date_Time, 3, 3) = 'APR' THEN '04'  "
                        + " when substring(Date_Time, 3, 3) = 'MAY' THEN '05'  "
                        + " when substring(Date_Time, 3, 3) = 'JUN' THEN '06'  "

                        + " when substring(Date_Time, 3, 3) = 'JUL' THEN '07'  "
                        + " when substring(Date_Time, 3, 3) = 'AUG' THEN '08'  "
                        + " when substring(Date_Time, 3, 3) = 'SEP' THEN '09'  "

                        + " when substring(Date_Time, 3, 3) = 'OCT'  THEN '10'  "
                        + " when substring(Date_Time, 3, 3) = 'NOV' THEN '11'  "
                        + " when substring(Date_Time, 3, 3) = 'DEC' THEN '12'  "
                        + " else '00' "
                        + "end "
                        + " ,[YYYY] = "
                          + "case " // YEAR BASED ON Month name 
                        + " when substring(Date_Time, 3, 3) = 'JAN' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'FEB' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'MAR' THEN @YEAR2 "

                        + " when substring(Date_Time, 3, 3) = 'APR' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'MAY' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'JUN' THEN @YEAR2 "

                        + " when substring(Date_Time, 3, 3) = 'JUL' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'AUG' THEN @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'SEP' THEN @YEAR2 "

                        + " when substring(Date_Time, 3, 3) = 'OCT' THEN  @YEAR2 "
                        + " when substring(Date_Time, 3, 3) = 'NOV' THEN  @YEAR1 "
                        + " when substring(Date_Time, 3, 3) = 'DEC' THEN  @YEAR1 "
                        + " else '00' "
                        + "end "
                    // + " , YYYY = @YEAR"
                    + " ,TRAN_TIME = substring(Date_Time, 7, 13) " // time
                    + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        if (DateTime.Now.Month == 01 || DateTime.Now.Month == 02)
                        {
                            cmd.Parameters.AddWithValue("@YEAR1", "2019");
                            cmd.Parameters.AddWithValue("@YEAR2", "2020");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@YEAR1", "2020");
                            cmd.Parameters.AddWithValue("@YEAR2", "2020");
                        }

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


            RecordFound = false;

            // INSERT VISA from BULK
            //

            SQLCmd =
                // "SET dateformat ymd "
                " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]"
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "

                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransAmt] "

                 + ",[RRNumber] "
                 + ",[TraceNo] "

                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                      + " , [Net_TransDate] "

                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " , '" + PRX + "225' " // All VISA Outgoing
                                          //       + " WHEN ACQISS = 'I' THEN 'BDC230' " // All INcoming
                                          //    + " else 'BDC999' "
                                          //    + " end "
                 + ", @TerminalType"

            //     + ", CAST ((DD+'/'+MM+'/'+YYYY) AS datetime) + CAST(TRAN_TIME AS datetime) "
                  + " , ISNULL(try_convert(datetime, (DD + '/' + MM + '/' + YYYY) + ' ' "
                            + "+ TRAN_TIME, 103), '1900-01-01')"
                 + ",case "
                 + " when [SETTL_AMOUNT_Code] = 'DR' THEN 21  " // Opposite for Visa 
                 + " when [SETTL_AMOUNT_Code] = 'CR' THEN 11  "
                 + " else 0 "
                 + "end "
                 + ",'TXN Issuer... '+ [ISSUER]  "//  in place of Transction Description 
                 + ",CAST([AMOUNT] As decimal(18,2)) / 100"

                 + ",[RRN] "
                 + ",[TRACE] "

                  + ", ISNULL(left([CARD], 6) + '******' + right([CARD], 4), '') "  // CARD  
                                                                                    // + ",[CARD] "  // 
                     + ",case "
                     + " when [RSP_CD] = '00' THEN '0'  "
                     + " else [RSP_CD] "
                     + "end "
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'1'" // Origin
                 + " ,'4'" // Destination

                 + " ,[RSP_CD]" // for the comment 
                 + " , CAST(ISNULL(try_convert(datetime, (DD + '/' + MM + '/' + YYYY) + ' ' "
                            + "+ TRAN_TIME, 103), '1900-01-01') as Date)"
                 //  + " ,ISNULL(try_convert(datetime, (DD+'/'+MM+'/'+YYYY)) , '1900-12-31') "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                 + " WHERE  "
                + " CAST([AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Visa" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT VISA RECORDS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO VISA with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //
            //            DELETE FROM VISA CARD txns before testing period
            int DelCount;

            //
            // UPDATE CARD Number and Origin
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[VISA_CARD] "
          + " SET "

          + " TerminalId = t2.TerminalId "
          + ",Net_TransDate = t2.Net_TransDate "
           + ",CAP_DATE = t2.CAP_DATE "

          + ",AmtFileBToFileC = 9.999 " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[VISA_CARD] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          //  + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber "
          //    + " AND t1.Net_TransDate = t2.Net_TransDate "
          + " WHERE  (t1.Processed = 0  AND t1.MatchingCateg = '" + PRX + "225') AND (t2.Processed = 0  AND t2.MatchingCateg = '" + PRX + "225') "; // For not processed yet records

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

                    stpErrorText = stpErrorText + "Cancel During Card Updating";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Terminal Id Updated from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // VISA CARD
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
            + ",A.TraceNo AS TraceNo_4 ,B.TraceNo AS TraceNo_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card] B "
          + " ON A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
           + " AND a.Net_TransDate = b.Net_TransDate "
          + " WHERE "
          + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND (A.TransType = 21 AND B.TransType = 11)"
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR VISA." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_VISA_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;

            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_VISA CARD";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }



            //
            stpErrorText += DateTime.Now + "_" + "VISA CARD Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;

        }

        //
        //// CREATE E_FINANCE_D_TXNS
        //
        public void InsertRecords_GTL_E_FINANCE_D_TXNS(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            // [RRDM_Reconciliation_ITMX].[dbo].[E_FINANCE_D_TXNS]

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;


            // Truncate Table InTableB_OneField

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

                    stpErrorText = stpErrorText + "Cancel During TRUNCATE";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            // BULK INSERT 

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // 
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During BULK INSERT E_FINANCE_D_TXNS";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FIRST LEVEL BULK Finished with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Correct Amount
            // remove not Needed Characters 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET TransactionAmount = replace(TransactionAmount, '\"', '') "

                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET TransactionAmount = replace(TransactionAmount, ',', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName 
                    //+ " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records_2]"
                    //+ " SET TRAN_DATE = '12/31/2018' "
                    //+ " WHERE TRAN_DATE = '31DEC' "
                    // + " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]" 
                    //+ " SET TRAN_DATE = '01/01/2019' "
                    //+ " WHERE TRAN_DATE = '01JAN' "
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

            // DELETE NULL LINES

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " WHERE ReportType IS NULL";

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

                    stpErrorText = stpErrorText + "Cancel During DELETE NULLS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // INSERT E_FINANCE_D_TXNS from BULK
            //

            SQLCmd =
                // "SET dateformat ymd "
                " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[E_FINANCE_D_TXNS]"
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " , [TerminalId] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ", [TransCurr] "
                 + ",[TransAmt] "

                 + ",[RRNumber] "
                 + ",[TraceNo] "

                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                      + " , [Net_TransDate] "
                         + ",[CAP_DATE] "
                     + ",[SET_DATE] "

                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " , '" + PRX + "280' " // For E-Finance 
                 + ", @TerminalType"
                 + " , Branchname " // In place of Terminal ID. 

                 + "  ,cast(substring(TransDateTime, 7, 4) + substring(TransDateTime, 4, 2) "
                 + " + substring(TransDateTime, 1, 2) + ' '"
                 + "+ substring(TransDateTime, 12, 8) as datetime)"
                 // TransType
                 + " , 11 "
                 // Tereminal Id 

                 + " , ServiceType + ' '+ details2 + ' ' + details3 "//  in place of Transction Description 
                 + " , 'EGP' "
                 + ", cast (TransactionAmount as decimal(18,2)) + cast (E_PayCommission as decimal(18,2))"
                 //+ ",CAST([AMOUNT] As decimal(18,2)) " // Amount 
                 + " , Reference " // in place of RRN 
                                   //+ ",[RRN] "
                 + ", '' "  // Blank in place of Trace 
                            // + ",[TRACE] "
                + " , 'No Card Trans' " // in place of card
                                        // + ", ISNULL(left([CARD], 6) + '******' + right([CARD], 4), '') "  // CARD  
                                        // + ",[CARD] "  // 
                     + ", '0' " // in place of response code
                                //+ ",case "
                                // + " when [RSP_CD] = '00' THEN '0'  "
                                // + " else [RSP_CD] "
                                // + "end "
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'99'" // Origin arbitrary definition
                 + " ,'99'" // Destination arbitrary definition

                 + " ,''" // for the comment 
                          // Net Date
                  + "  ,cast(substring(TransDateTime, 7, 4) + substring(TransDateTime, 4, 2) " // NET _ Date Time 
                 + " + substring(TransDateTime, 1, 2) as Date) "
                    // CAP_DATE
                    + "  ,cast(substring(TransDateTime, 7, 4) + substring(TransDateTime, 4, 2) " // NET _ Date Time 
                 + " + substring(TransDateTime, 1, 2) as Date) "
                    // SET_DATE
                    + "  ,cast(substring(TransDateTime, 7, 4) + substring(TransDateTime, 4, 2) " // NET _ Date Time 
                 + " + substring(TransDateTime, 1, 2) as Date) "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // + " WHERE  "
                // + " CAST([AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For E_FINANCE_D_TXNS" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT E_FINANCE_D_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO E_FINANCE TXNS with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            stpErrorText += DateTime.Now + "_" + "E-Finance TXNS Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;

        }

        //
        //// CREATE E_FINANCE_D_TXNS
        //
        public void InsertRecords_GTL_E_FINANCE_GL(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            // [RRDM_Reconciliation_ITMX].[dbo].[E_FINANCE_GL]

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;


            // Truncate Table InTableB_OneField

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

                    stpErrorText = stpErrorText + "Cancel During TRUNCATE";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            // BULK INSERT ONE FIELD

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // 
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During BULK INSERT E_FINANCE_GL";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FIRST BULK Finished with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Correct Amount
            // remove not Needed Characters 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET Amount = replace(Amount, '\"', '') "

                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET Amount = replace(Amount, ',', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName 
                    //+ " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records_2]"
                    //+ " SET TRAN_DATE = '12/31/2018' "
                    //+ " WHERE TRAN_DATE = '31DEC' "
                    // + " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_VISA_BULK_Records]" 
                    //+ " SET TRAN_DATE = '01/01/2019' "
                    //+ " WHERE TRAN_DATE = '01JAN' "
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


            // DELETE NULL LINES

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " WHERE UserName IS NULL";

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

                    stpErrorText = stpErrorText + "Cancel During DELETE NULLS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // return; 

            RecordFound = false;

            // INSERT E_FINANCE_GL from BULK
            //

            SQLCmd =
                // "SET dateformat ymd "
                " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[E_FINANCE_GL]"
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " , [TerminalId] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ", [TransCurr] "
                 + ",[TransAmt] "

                 + ",[RRNumber] "
                 + ",[TraceNo] "

                 + ",[CardNumber] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                      + " , [Net_TransDate] "
                         + ",[CAP_DATE] "
                     + ",[SET_DATE] "

                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " , '" + PRX + "280' " // For E-Finance 
                 + ", @TerminalType"
                 + " , [GLCode] + ' '+ GLNumber " // For terminal Id 

                 // DATE
                 + " ,cast(substring(Transactiondate, 7, 4) "
                 + "  + substring(Transactiondate, 4, 2) "
                 + "  + substring(Transactiondate, 1, 2) "
                  + "  + substring(Transactiondate, 12, 8) as date) "

                 // TransType
                 + " , 11 "
                 // Tereminal Id 

                 + "  , 'Branch'+ ' '+GLCode + ' User:' + USER_ID "//  in place of Transaction Description 
                 + " , 'EGP' "
                 + " , Cast(Amount as decimal(18,2)) "
                 //+ ",CAST([AMOUNT] As decimal(18,2)) " // Amount 
                 + " , Discreption " // in place of RRN 
                                     //+ ",[RRN] "
                 + ", '' "  // Blank in place of Trace 
                            // + ",[TRACE] "
                + " , 'No Card Trans' " // in place of card
                                        // + ", ISNULL(left([CARD], 6) + '******' + right([CARD], 4), '') "  // CARD  
                                        // + ",[CARD] "  // 
                     + ", '0' " // in place of response code
                                //+ ",case "
                                // + " when [RSP_CD] = '00' THEN '0'  "
                                // + " else [RSP_CD] "
                                // + "end "
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'99'" // Origin arbitrary definition
                 + " ,'99'" // Destination arbitrary definition

                 + " ,''" // for the comment 
                          // Net Date
                + " ,cast(substring(Transactiondate, 7, 4) "
                 + "  + substring(Transactiondate, 4, 2) "
                 + "  + substring(Transactiondate, 1, 2) "
                  + "  + substring(Transactiondate, 12, 8) as date) "
                     // CAP_DATE
                     + " ,cast(substring(Transactiondate, 7, 4) "
                 + "  + substring(Transactiondate, 4, 2) "
                 + "  + substring(Transactiondate, 1, 2) "
                  + "  + substring(Transactiondate, 12, 8) as date) "
                    // SET_DATE
                    + " ,cast(substring(Transactiondate, 7, 4) "
                 + "  + substring(Transactiondate, 4, 2) "
                 + "  + substring(Transactiondate, 1, 2) "
                  + "  + substring(Transactiondate, 12, 8) as date) "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // + " WHERE  "
                // + " CAST([AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For E_FINANCE_GL" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT E_FINANCE_GL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO E_FINANCE_GL with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //
            stpErrorText += DateTime.Now + "_" + "E_Finance GL Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;

        }
        //
        //// CREATE Credit_Card FILEs 
        //
        public void InsertRecords_GTL_Credit_Card(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE

            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_CREDIT_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]

            string SQLCmd;

            string WTerminalType = "10"; // ATM

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate BULK Table 

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

                    stpErrorText = stpErrorText + "Cancel During TRuncate ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'   " // MUST be examined (may be change db character set to UTF8)
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT. Credit_Card";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "BULK INSERT FINISHES with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            // INSERT Credit Card from BULK
            //

            SQLCmd =
                //"SET dateformat dmy "
                " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]"
                + "( "
                 + " [OriginFileName] "
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TerminalId] "
                 + " ,[TransDate] "
                 + " ,[TransType] "
                 + ",[TransDescr] "
                 + ",[TransCurr] "
                 + ",[TransAmt] "

                 + ",[TraceNo] "
                 + ",[RRNumber] "

                 + ",[CardNumber] "
                  + ",[AccNo] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "
                      + " , [Net_TransDate] "
                      + " , SET_DATE "
                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                 + " , '" + PRX + "240' " // All CREDIT Card Outgoing

                 + ", @TerminalType"
                 + ", [TERMID] " // 
                 + ", CAST([TRNDATE] As datetime) "
                 + ",case "
                 + " when [TFEE] = '07' THEN 11  " // Withdrawl 
                 + " when [TFEE] = '06' THEN 23  " // Deposit  
                 + " when [TFEE] = '26' THEN 13  " // Deposit Reversal
                 + " when [TFEE] = '27' THEN 27  " // Reversal without reference 
                 + " else  0 "
                 + "end "
                 + ",[DISC] " //  Transction Description 
                 + ",  ISNULL([CURR], '')"
                 + ",CAST([Amt/100] As decimal(18,2)) / 100"

                 + ", 0 " // No Trace available 
                 + ", ISNULL([REFNUMB], '') " // RRNumber

                 + ", ISNULL(left([CARDVO], 6) + '******' + right([CARDVO], 4), '') "  // CARD  
                                                                                       // + ", REPLACE(LTRIM(REPLACE([ACC], '0', ' ')), ' ', '0') " //AccNo
                 + " , RIGHT(ACC, 14) "
                  + ", '0' " // Response code
                             //+ ",case "
                             //+ " when [RSP_CD] = '00' THEN '0'  "
                             //+ " else [RSP_CD] "
                             //+ "end "
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'1'" // Origin
                 + " ,'13'" // Destination
                 + " , 'AuthNo' + ISNULL([ATHUNUM], '') " // for the comment 
                  + ", CAST([TRNDATE] As date) "
                  + ", CAST([STTDATE] As date) "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE  "
                + " ([TFEE] = '07' OR [TFEE] = '06' OR [TFEE] = '26' OR [TFEE] = '27') AND ATM_ONUS = 'ATM ONUS' "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@TransCurr", "EGP");

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For Credit" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT Credit_Card RECORDS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO Credit_Card with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE RRN NUMBER FROM THE CREDIT CARD 
            SQLCmd =
        " update H "
     + " set [RRNumber] = R.RRNumber, OriginalRecordId = R.SeqNo "
     + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] H "
     + " inner join "
     + " [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] R "
     + " ON "
    + "  H.AccNo  = R.AccNo "
     + "  AND H.TerminalId = R.TerminalId "
     + "  AND H.TransAmt = R.TransAmt "
     + "  AND H.Comment = R.Comment " // The comment contains the authorisation Number 
      + " WHERE H.Processed = 0  "
      + "               AND R.Processed= 0  AND H.RRNumber = '' AND R.RRNumber <> ''   "  // 
     ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals Credit Card";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            //
            stpErrorText += DateTime.Now + "_" + "Credit_Card Update RRNumber.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE OTHER INFO FROM SWITCH
            //
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] "
          + " SET  "

          + " TraceNo = t2.TraceNo "
          + ",Card_Encrypted = t2.Card_Encrypted "

          + ",TransDate = t2.TransDate "
         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "

          + ",Net_TransDate = t2.Net_TransDate "
           + ",CAP_DATE = t2.CAP_DATE "

        + ", OriginalRecordId = t2.SeqNo "
          + ",AmtFileBToFileC = 999.99 " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.CardNumber = t2.CardNumber "
          //   + " AND t1.AccNo = t2.AccNo " //Accounts not the same

          + " WHERE  t1.Processed = 0 AND (t2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "240') "; // For not processed yet records

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
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel During Credit_Card Updating";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Trace No and Origin Updated from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MASTER CARD
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
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card] B "
          + " ON A.TransAmt = B.TransAmt "
          + " AND A.AccNo = B.AccNo "
          + " AND A.TerminalId = B.TerminalId "
          + " AND A.RRNumber = B.RRNumber "
          + "  AND A.Comment = B.Comment " // The comment contains the authorisation Number 
                                           //   + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
            + " AND ( "
            + "(A.TransType = 11 AND B.TransType = 27) "
            + " OR (A.TransType = 13 AND B.TransType = 23) "
            + " ) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR Credit_Card." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Credit_Card";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            //string FileId = "Credit_Card";
            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_CREDIT CARD";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            //
            stpErrorText += DateTime.Now + "_" + "CREDIT_CARD Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

        }

        //
        //// CREATE MASTER_CARD_POS
        //
        public void InsertRecords_GTL_MASTER_CARD_POS(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE

            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_CREDIT_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]

            string SQLCmd;

            string WTerminalType = "20"; // 10 for ATM 20 for POS and Internet

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate BULK Table 

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

                    stpErrorText = stpErrorText + "Cancel During TRuncate ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1252'   " // MUST be examined (may be change db character set to UTF8)
                                                                            //+ " ,ROWs_PER_BATCH=15000 "
                          + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',' "
                         //+ ",FIELDTERMINATOR = '\t'"
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "BULK INSERT FINISHES with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // Correct Amount
            // remove not Needed Characters 
            // "2,214.69"
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, ',', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_POS_BULK_Records_3]"
                    //+ " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_POS_BULK_Records]"
                    //+ " SET AMOUNT = '' "
                    //+ " WHERE AMOUNT = '918E+11' "

                    + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
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



            RecordFound = false;

            // INSERT POS from BULK
            //

            SQLCmd =
                 // "SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
                + "( "
                 + " [OriginFileName] " // 1
                 + " ,[Origin] "  // 2.1
                  + " ,[TransTypeAtOrigin] "  // 2.2
                  + " ,[MatchingCateg] " // 3
                  + " ,[TerminalId] " // 4

                 + " ,[TerminalType] " // 5

                 + " ,[TransDate] " // 6
                 + " ,[TransType] " // 7
                 + ",[TransDescr] " // 8

                 + ",[TransCurr] " // 9
                 + ",[TransAmt] "  // 10

                 + ",[RRNumber] " // 11
                 + ",[FullTraceNo] " // 12

                 + ",[CardNumber] " // 13

                 + ",[ResponseCode] " // 14
                 + ",[LoadedAtRMCycle] " // 15
                   + ",[Operator] " // 16
                   + ",[TXNSRC] " // 17

                    + ",[TXNDEST] " // 18
                    + ",[Comment] " // 19
                     + ",[Card_Encrypted] " // 20
                       + ",[ACCEPTOR_ID] "
                        + ",[ACCEPTORNAME] "
                       + " , [SET_DATE] " // 21
                 + ") "

                 + " SELECT "
                 + "@OriginFile" // 1
                 + " ,@Origin"  // 2.1
                  + " ,'EXTERNAL' "  // 2.2
                   + " ,case" // Destination // 3 
                 + " WHEN ( LEFT([PAN_MASK],6) = '526402' OR LEFT([PAN_MASK],6) = '544460' OR LEFT([PAN_MASK],6) = '557876') THEN '" + PRX + "231' " // All DEBIT
                 + " else '" + PRX + "233' " // REST ALL Prepaid
                 + " end "
                 //+ " , 'BDC231' COREBANKING" // All Master POS
                 //    , 'BDC233' Prepaid    // They have two destinations 
                 //                 // a) COREBANKING and Prepaid ( Destination = 1 and 12)

                 + ", ISNULL(TERMID, '') " // 4
                 + ", @TerminalType " // 5

                   + ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                 // Date // 6
                 //+ ",ISNULL(try_convert(datetime, [LOCAL_DATE] + ' ' "
                 //+ "   + substring(right('00000000' + [LOCAL_TIME], 8), 1, 2) + ':' "
                 //+ "    + substring(right('00000000' + [LOCAL_TIME], 8), 3, 2) "
                 //+ "    + ':' + substring(right('00000000' + [LOCAL_TIME], 8), 5, 2), 103), '1900-01-01') "

                 + ",case " // 7
                 + " when ([P0025_1] = '' AND PCODE <>'200000') THEN 11  "
                 + " when [P0025_1] = 'R' THEN 21  "
                 + " when ([P0025_1] = '' AND PCODE = '200000') THEN 21  " // Credit Adjustment
                 + " else 0 "
                 + "end "
                   //    + ",[ACCEPTORNAME] " //  in place of Transction Description 
                   // Txn Descr
                   + ",case " // // 8 
                 + " when ([P0025_1] = '' AND PCODE <>'200000') THEN 'POS DEBIT TXN'  "  // Description
                 + " when [P0025_1] = 'R' THEN 'POS REVERSAL'  "
                  + " when ([P0025_1] = '' AND PCODE = '200000') THEN 'POS CREDIT ADJUSTMENT'  "
                 + " else 'Not Defined' "
                 + "end "

                  + ",ISNULL(LEFT([ACQ_CURRENCY_CODE],3), '') " // Ccy // 9
                 + ",TRY_CAST([AMOUNT] As decimal(18,2)) " // 10
                 + ",ISNULL(AUTHNUM, '') " // AUTHORISATION goes in place RRN // 11
                                           // FULL Trace // 12 
                 + ", ISNULL('ACCEPTOR_ID:' + ACCEPTOR_ID + '\r\n ACCEPTORNAME:'+ [ACCEPTORNAME] + '\r\n REFERENCE:'+ [ACQ_REFDATA] , '') "  // in Place Of Full Trace

                 + ",ISNULL([PAN_MASK], '') "  // 13
                 + ", ISNULL(PCODE, '') " // 14 Response code 

                 + ", @LoadedAtRMCycle" // 15
                  + ", @Operator" // 16

                 + " ,'5'" // Origin 526402******7213 // 17

                 + " ,case" // Destination
                 + " WHEN ( LEFT([PAN_MASK],6) = '526402' OR LEFT([PAN_MASK],6) = '544460' OR LEFT([PAN_MASK],6) = '557876') THEN '1' " // 18
                 + " else '12' "
                 + " end "

                 + " ,'Comment' " // for the comment // 19
                 + " , ISNULL(PAN, '') " // for the comment // 20

                 + ", ISNULL([ACCEPTOR_ID], '') " // ACCEPTOR_ID
                 + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME
                 + ", SETTLEMENT_DATE "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // + " WHERE  "
                //+ " CAST([AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@Ccy", "EGP");

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MASTER POS" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT POS RECORDS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO MASTER POS with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE [Net_TransDate]
            // Clear Data 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
          + " SET [Net_TransDate] = Cast(TransDate as  Date) "
          + " WHERE  (Processed = 0 ) "
            + " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
         + " WHERE (Processed = 0 ) AND TransAmt IS NULL "
            + " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
           + " WHERE (Processed = 0 ) AND ResponseCode = '0' AND RRNumber = '' ";
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Card Updating";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Net_TransDate Updated within..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //  return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MASTER CARD POS
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
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] B "
          + " ON A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Card_Encrypted = B.Card_Encrypted "
          //    + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
           + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND (A.TransType = 21 AND B.TransType = 11) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR MASTER_POS." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MASTER_POS_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            //string FileId = "MASTER_POS";
            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_MASTER_POS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            //
            stpErrorText += DateTime.Now + "_" + "MASTER_CARD_POS Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

        }

        //
        //// CREATE MASTER_CARD_POS
        //
        public void InsertRecords_GTL_MASTER_CARD_POS_2(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE

            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_CREDIT_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]

            string SQLCmd;

            string WTerminalType = "20"; // 10 for ATM 20 for POS and Internet

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate BULK Table 

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

                    stpErrorText = stpErrorText + "Cancel During TRuncate ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1252'   " // MUST be examined (may be change db character set to UTF8)
                                                                            //+ " ,ROWs_PER_BATCH=15000 "
                          + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',' "
                         //+ ",FIELDTERMINATOR = '\t'"
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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT";
                    MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            stpErrorText += DateTime.Now + "_" + "BULK INSERT FINISHES with..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // Correct Amount
            // remove not Needed Characters 
            // "2,214.69"
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMOUNT = replace(AMOUNT, ',', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_POS_BULK_Records_3]"
                    //+ " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_POS_BULK_Records]"
                    //+ " SET AMOUNT = '' "
                    //+ " WHERE AMOUNT = '918E+11' "

                    + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
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

            RecordFound = false;

            // INSERT POS from BULK
            //

            SQLCmd =
                 // "SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
                + "( "
                 + " [OriginFileName] " // 1
                 + " ,[Origin] "  // 2.1
                  + " ,[TransTypeAtOrigin] "  // 2.2
                  + " ,[MatchingCateg] " // 3
                  + " ,[TerminalId] " // 4

                 + " ,[TerminalType] " // 5

                 + " ,[TransDate] " // 6
                 + " ,[TransType] " // 7
                 + ",[TransDescr] " // 8

                 + ",[TransCurr] " // 9
                 + ",[TransAmt] "  // 10

                 + ",[RRNumber] " // 11
                 + ",[FullTraceNo] " // 12

                 + ",[CardNumber] " // 13

                 + ",[ResponseCode] " // 14
                 + ",[LoadedAtRMCycle] " // 15
                   + ",[Operator] " // 16
                   + ",[TXNSRC] " // 17

                    + ",[TXNDEST] " // 18
                    + ",[Comment] " // 19

                     + ",[Card_Encrypted] " // 20
                       + ",[ACCEPTOR_ID] "
                        + ",[ACCEPTORNAME] "

                       + " , [SET_DATE] " // 21
                 + ") "

                 + " SELECT "
                 + "@OriginFile" // 1
                 + " ,@Origin"  // 2.1
                  + " ,'EXTERNAL' "  // 2.2
                   + " ,case" // Destination // 3 
                 + " WHEN ( LEFT([PAN_MASK],6) = '526402' OR LEFT([PAN_MASK],6) = '544460' OR LEFT([PAN_MASK],6) = '557876') THEN '" + PRX + "231' " // All DEBIT
                 + " else '" + PRX + "233' " // REST ALL Prepaid
                                             //   + " WHEN ACQISS = 'A' THEN '" + PRX + "235' " // All Outgoing
                                             //+ " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN '" + PRX + "230' " // All DEBIT
                                             //+ " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532' ) THEN '" + PRX + "232' " // All PREPAID
                 + " end "
                 //+ " , 'BDC231' COREBANKING" // All Master POS
                 //    , 'BDC233' Prepaid    // They have two destinations 
                 //                 // a) COREBANKING and Prepaid ( Destination = 1 and 12)

                 + ", ISNULL(TERMID, '') " // 4
                 + ", @TerminalType " // 5

                   + ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                 // Date // 6
                 //+ ",ISNULL(try_convert(datetime, [LOCAL_DATE] + ' ' "
                 //+ "   + substring(right('00000000' + [LOCAL_TIME], 8), 1, 2) + ':' "
                 //+ "    + substring(right('00000000' + [LOCAL_TIME], 8), 3, 2) "
                 //+ "    + ':' + substring(right('00000000' + [LOCAL_TIME], 8), 5, 2), 103), '1900-01-01') "

                 + ",case " // 7
                 + " when ([P0025_1] = '' AND PCODE <>'200000') THEN 11  "
                 + " when [P0025_1] = 'R' THEN 21  "
                 + " when ([P0025_1] = '' AND PCODE = '200000') THEN 21  " // Credit Adjustment
                 + " else 0 "
                 + "end "
                   //    + ",[ACCEPTORNAME] " //  in place of Transction Description 
                   // Txn Descr
                   + ",case " // // 8 
                 + " when ([P0025_1] = '' AND PCODE <>'200000') THEN 'POS DEBIT TXN'  "  // Description
                 + " when [P0025_1] = 'R' THEN 'POS REVERSAL'  "
                  + " when ([P0025_1] = '' AND PCODE = '200000') THEN 'POS CREDIT ADJUSTMENT'  "
                 + " else 'Not Defined' "
                 + "end "

                  + ", ISNULL(LEFT([SETTLEMENT_CODE],3), '') "// Ccy // 9
                 + ",TRY_CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // 10
                 + ",ISNULL(AUTHNUM, '') " // AUTHORISATION goes in place RRN // 11
                                           // FULL Trace // 12 
                 + ", ISNULL('ACCEPTOR_ID:' + ACCEPTOR_ID + '\r\n ACCEPTORNAME:'+ [ACCEPTORNAME] + '\r\n REFERENCE:'+ [ACQ_REFDATA] , '') "  // in Place Of Full Trace

                 + ",ISNULL([PAN_MASK], '') "  // 13
                     + " ,case" // PCODE
                 + " WHEN   ISNULL(PCODE, '')  = '3000' Then '0'  " // 14
                 + " else ISNULL(PCODE, '') "
                 + " end "
                 //+ ", ISNULL(PCODE, '') " // 14 Response code 

                 + ", @LoadedAtRMCycle" // 15
                  + ", @Operator" // 16

                 + " ,'5'" // Origin 526402******7213 // 17

                 + " ,case" // Destination
                 + " WHEN ( LEFT([PAN_MASK],6) = '526402' OR LEFT([PAN_MASK],6) = '544460' OR LEFT([PAN_MASK],6) = '557876') THEN '1' " // 18
                 + " else '12' "
                 + " end "

                 + " ,'Comment' " // for the comment // 19
                 + " , ISNULL(PAN, '') " // for the comment // 20

                 + ", ISNULL([ACCEPTOR_ID], '') " // ACCEPTOR_ID
                 + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME
                 + ", SETTLEMENT_DATE "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                // + " WHERE  "
                //+ " CAST([AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@Ccy", "EGP");

                        cmd.CommandTimeout = 350;
                        stpLineCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                    if (Environment.UserInteractive & TEST == true)
                    {
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MASTER POS" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT POS RECORDS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO MASTER POS with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE [Net_TransDate]
            // Clear Data 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
          + " SET [Net_TransDate] = Cast(TransDate as  Date) "
          + " WHERE  (Processed = 0 ) "
            + " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
         + " WHERE (Processed = 0 ) AND TransAmt IS NULL "
            + " DELETE FROM[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
           + " WHERE (Processed = 0 ) AND ResponseCode = '0' AND RRNumber = '' ";
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Card Updating";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Net_TransDate Updated within..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //  return; 

            //
            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS
            // MASTER CARD POS
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
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] B "
          + " ON A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Card_Encrypted = B.Card_Encrypted "
          //    + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
           + " (A.Processed = 0 AND B.Processed = 0)  "
          + " AND (A.TransType = 21 AND B.TransType = 11) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR MASTER_POS." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MASTER_POS_1";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            //string FileId = "MASTER_POS";
            int SeqNo_2;
            int SeqNo_4;
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";

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
                    stpErrorText = stpErrorText + "Cancel At _Updating Reversals_MASTER_POS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            //
            stpErrorText += DateTime.Now + "_" + "MASTER_CARD_POS Finishes with SUCCESS.." + "\r\n";

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

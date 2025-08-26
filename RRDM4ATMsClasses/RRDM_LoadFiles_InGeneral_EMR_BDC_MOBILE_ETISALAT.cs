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
    public class RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_ETISALAT : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_ETISALAT() : base() { }
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

        DateTime Meeza_Global_Date = new DateTime(2022, 02, 01);

        string SqlString; // Do not delete 

        string WFileSeqNo = "";
        string WOperator;
        int WReconcCycleNo; 

        DateTime NewVersionDt;

        public DataTable TableATMsDailyStats = new DataTable();

        public DataTable Duplicates_Salaries_Withdrawls = new DataTable();

        RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMGasParameters Gp = new RRDMGasParameters();

        readonly string connectionString_ETI = ConfigurationManager.ConnectionStrings["ETISALATConnectionString"].ConnectionString;
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

        public void InsertRecordsInTableFromTextFile_InBulk_ETISALAT(string InOperator, string InTableA
                                           , string InFullPath, string InCondition, string InFullPath2, int InFlogSeqNo, int InReconcCycleNo)
        {
            // WE WANT as INPUT
            // a) FILE NAME => to see what Method to call eg Switch_IST_Txns 
            // a) Directory and FILE NAME to know whare to get from 

            // WE SHOULD PROVIDE AS OUTPUT
            // a) Status 
            // b) Level of process 
            // WE WANT as INPUT
            // a) FILE NAME => to see what Method to call eg Switch_IST_Txns 
            // a) Directory and FILE NAME to know whare to get from 


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


            // Read fields of existing table
            //RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //// 
            //// CLEAN TABLES from ANY testing data 
            //int TempWReconcCycleNo = 999999;
            //Bio.CleanTables(InTableA, TempWReconcCycleNo);

            if (InOperator == "BCAIEGCX")
            {
                PRX = "BDC"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }


            WReconcCycleNo = InReconcCycleNo;


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


            // Date of the file in string
            //var resultLastThreeDigits = InFullPath.Substring(InFullPath.Length - 3);
            //string result1 = InFullPath.Substring(InFullPath.Length - 12);
            //string result2 = result1.Substring(0, 8);

            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo); 

           
            WCut_Off_Date = Rjc.Cut_Off_Date;

            ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

            
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

            //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            //RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();

            RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

            stpLineCount = 0;
            stpReturnCode = -1;
            LastSeqNo = 0;
            //
            // GO TO LOAD
            //
            //
            // GO TO LOAD
            //
            string text = "In Process Loading of .. " + InTableA;
            string caption = "LOADING OF FILES";
            int timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            switch (InTableA)
            {
                case "ETISALAT_TPF_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_TPF_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_TPF_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_MEEZA_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_MEEZA_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_MEEZA_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "ETISALAT_MEEZA_TOTALS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_MEEZA_TOTALS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_MEEZA_TOTALS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_NODE_TOTALS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_NODE_TOTALS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_NODE_TOTALS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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



                case "ETISALAT_FAWRY_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_FAWRY_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_FAWRY_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "ETISALAT_DISPUTES_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_DISPUTES_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_DISPUTES_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_SURPLUS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_FAWRY_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_SURPLUS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                    // THIS HAS TO DO WITH IRAQ



                case "ETISALAT_1_SALARY_WITHDRAWLS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_1_SALARY_WITHDRAWLS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_1_SALARY_WITHDRAWLS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "ETISALAT_2_AGENTS_PAY_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_2_AGENTS_PAY_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_2_AGENTS_PAY_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_5_MONITORING_Report_Data_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_LOADING_ETISALAT_5_MONITORING_Report_Data_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_5_MONITORING_Report_Data_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_6_CLEARING_REPORT_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_6_CLEARING_REPORT_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_6_CLEARING_REPORT_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "ETISALAT_7_MoneyGrane_Connector_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_ETISALAT_7_MoneyGrane_Connector_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_ETISALAT_7_MoneyGrane_Connector_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                        return; 

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
            //if (InOriginFileName == "NCR_FOREX")
            //{
            //    // EXTRA FIELDS
            //    UpdateFiles_With_EXTRA(WOperator, InReconcCycleNo);
            //    int Test = 2;
            //    if (Test == 1)
            //    {
            //        La.COPY_NCR_FOREX_ToIST(InOriginFileName, InReconcCycleNo);

            //        if (La.ErrorFound == true)
            //        {
            //            stpErrorText += DateTime.Now + "_" + "Error During copying to IST.." + "\r\n" + La.ErrorOutput + "\r\n";
            //            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //            stpReturnCode = -1;

            //            stpReferenceCode = stpErrorText;
            //            //  CatchDetails(ex);
            //            return;
            //        }
            //        else
            //        {
            //            stpErrorText += DateTime.Now + "_" + "Records were copied to IST.." + La.Counter.ToString() + "\r\n";
            //            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //        }


            //        RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();
            //        Mg.UpdatetSourceTxnsProcessedToTrue_FOREX(InOriginFileName, InReconcCycleNo);

            //    }

            //}
            //
            // PROCESS CAME TILL HERE WITH SUCCESS
            //
            stpReturnCode = 0;
            stpErrorText += DateTime.Now + "_" + "END_Loading of File .SUCCESS." + La.Counter.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
        }

        //
        //// CREATE ETISALAT_TPF_TXNS
        //
        int ReturnCode;
        string ErrorText;
        string ErrorReference;
        int ret; 

        public void InsertRecords_ETISALAT_TPF_TXNS(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string TableId_Bulk_ALL = "[ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL]";

            // MAKE DELIMETER VALIDATION 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Del = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            bool Correct = Del.FindIfDelimiterErrorsInInputTable(TableId_Bulk_ALL, WFullPath_01, connectionString_ETI); 

            if (Correct == false)
            {

                string OldString = "faisal,giza";
                string NewString = "faisal giza"; // No comma
               
                Del.ReplaceValueInGivenTextFile(WFullPath_01, OldString, NewString);
            }

            // DO THE TEST AGAIN AFTER CHANGING THE input file 

            Correct = Del.FindIfDelimiterErrorsInInputTable(TableId_Bulk_ALL, WFullPath_01, connectionString_ETI);

            if (Correct == false)
            {
                MessageBox.Show("FILE IS WRONG" + WFullPath_01);
                return; 
            }

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_TPF_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT TPF ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT TPF " + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }

        public void InsertRecords_ETISALAT_MEEZA_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE
          
            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;


            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_MEEZA_TXNS]";
                                                        
            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters
                    
                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    
                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size =1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT MEEZA ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT MEEZA " + Environment.NewLine
                    +WErrorText); 

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

            ErrorFound = false;
          //  reErrorOutput = "";
            int CountDel;

            RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();

            string WSelectionCriteria = " WHERE IsReversal = 0 and TransType = 'ATMCashOutReversalOffUs' " ;

            string W_Application = "ETISALAT";

            string WTableName = "[ETISALAT].[dbo].[ETISALAT_MEEZA_TXNS]"; 

            string WTableName_MATCHED = ""; 
                                                                               ;
            int DB_Mode = 1;
            Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                           NullPastDate, NullPastDate, DB_Mode, W_Application);

                int I = 0;

                while (I <= (Mmob.DataTableAllFields.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNoA = (int)Mmob.DataTableAllFields.Rows[I]["SeqNo"];
                    string Meeza_TransactionReference = (string)Mmob.DataTableAllFields.Rows[I]["Meeza_TransactionReference"];
                    decimal TransAmount = (decimal)Mmob.DataTableAllFields.Rows[I]["TransAmount"];
                    string ReceivedTelephone = (string)Mmob.DataTableAllFields.Rows[I]["ReceivedTelephone"];

                string WSelectionCrriteria = "";
                bool IsSAIB = false; 

                if (Meeza_TransactionReference == "07SAIB-ATM4503" & ReceivedTelephone == "201156565458")
                {
                    Meeza_TransactionReference = "07SAIB-ATM4503"; 
                }
                if (Meeza_TransactionReference.Substring(0,6) == "07SAIB")
                {
                    // Read if there is other record and find SeqNo. 
                    WSelectionCrriteria = " WHERE  Left(Meeza_TransactionReference, 6) = '06SAIB' "
                                               + " AND TransAmount = " + TransAmount
                                               + " AND ReceivedTelephone ='" + ReceivedTelephone + "'"
                                               + " AND TransType = 'AtmCashOutOffUs' AND IsReversal = 0  "; // AtmCashOutOffUs
                    IsSAIB = true; 
                }
                else
                {
                    WSelectionCrriteria = " WHERE  Meeza_TransactionReference='" + Meeza_TransactionReference + "'"
                                               + " AND TransAmount = " + TransAmount
                                               + " AND ReceivedTelephone ='" + ReceivedTelephone + "'"
                                               + " AND TransType = 'AtmCashOutOffUs' AND IsReversal = 0  "; // AtmCashOutOffUs
                    IsSAIB = false; 
                }

                
                                            ;
                int WMode = 1;
                int WDB_Mode = 1;
                // For Both Update as reversals (IsReversal) 
                Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, WTableName, "", WSelectionCrriteria,
                                                   WMode, WDB_Mode);

                if (Mmob.RecordFound)
                {
                    int SeqNoB = Mmob.SeqNo;

                    Mmob.UpdateIsReversalBySeqNo(WTableName, SeqNoA);

                    if(IsSAIB == true)
                    {
                        Mmob.UpdateIsReversalBySeqNoWithRefToo(WTableName, SeqNoB, Meeza_TransactionReference); // MAKE THEM EQUAL
                    }
                    else
                    {
                        Mmob.UpdateIsReversalBySeqNo(WTableName, SeqNoB);
                    }

                    // UPDATE BOTH RECORDS As IsReversal = true 
                }
                else
                {
                    WSelectionCrriteria = " WHERE "
                                          + "  TransAmount = " + TransAmount
                                          + " AND ReceivedTelephone ='" + ReceivedTelephone + "'"
                                          + " AND TransType = 'AtmCashOutOffUs' "
                                           ;

                    //if (ReceivedTelephone == "201156565458")
                    //{
                    //    ReceivedTelephone = "201156565458"; 
                    //}
                    WMode = 1;
                    WDB_Mode = 1;
                    // For Both Update as reversals (IsReversal) 
                    Mmob.FindSourceRecords_MOBILE_2_RRDM_TWO_Files_Details(WOperator, WTableName, "", WSelectionCrriteria,
                                                       WMode, WDB_Mode);
                    if (Mmob.RecordFound)
                    {
                        int SeqNoB = Mmob.SeqNo;

                        Mmob.UpdateIsReversalBySeqNo(WTableName, SeqNoA);

                        Mmob.UpdateIsReversalBySeqNoWithRefToo(WTableName, SeqNoB, Meeza_TransactionReference); // MAKE THEM EQUAL

                      //  Mmob.UpdateIsReversalBySeqNo(WTableName, SeqNoB);
                        // UPDATE BOTH RECORDS As IsReversal = true 
                    }
                }

                I++; // Read Next entry of the table 
                }



            WSelectionCriteria = " WHERE IsReversal = 0 and TransType = 'AgentCashoutReversalOffUs'  ";

            W_Application = "ETISALAT";

            WTableName = "[ETISALAT].[dbo].[ETISALAT_MEEZA_TXNS]";

            WTableName_MATCHED = "";
            
            DB_Mode = 1;
            Mmob.ReadTrans_MASTERTable_Fill_SpecificTable_OneORTwoTables(WTableName, WTableName_MATCHED, WSelectionCriteria,
                                                           NullPastDate, NullPastDate, DB_Mode, W_Application);

            I = 0;

            while (I <= (Mmob.DataTableAllFields.Rows.Count - 1))
            {
                //    RecordFound = true;
                int SeqNoA = (int)Mmob.DataTableAllFields.Rows[I]["SeqNo"];
                string Meeza_TransactionReference = (string)Mmob.DataTableAllFields.Rows[I]["Meeza_TransactionReference"];
                decimal TransAmount = (decimal)Mmob.DataTableAllFields.Rows[I]["TransAmount"];
                string ReceivedTelephone = (string)Mmob.DataTableAllFields.Rows[I]["ReceivedTelephone"];
                string WMeeza_TransactionReference = (string)Mmob.DataTableAllFields.Rows[I]["Meeza_TransactionReference"];

                Mmob.UpdateIsReversalByMeezaReference(WTableName, WMeeza_TransactionReference); 

              

                I++; // Read Next entry of the table 
            }



        }


        public void InsertRecords_ETISALAT_FAWRY_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_FAWRY_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT FAWRY ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT FAWRY " + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

            ErrorFound = false;
            ErrorOutput = "";
            int CountDel;

            // Temporary fix till Pambos acts for FAwry 

        //    DELETE FROM [ETISALAT].[dbo].[ETISALAT_FAWRY_TXNS]
        //where RRNumber = '000000000000'

            using (SqlConnection conn =
                new SqlConnection(connectionString_ETI))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ETISALAT].[dbo].[ETISALAT_FAWRY_TXNS] "
                           + " WHERE RRNumber = '000000000000' AND LoadedAtRMCycle = @LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        CountDel = cmd.ExecuteNonQuery();


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


        public void InsertRecords_ETISALAT_DISPUTES_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_DISPUTES_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT DISPUTES";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT DISPUTES " + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

            //
            // UPDATE ETISALAT_CUSTOMER_DISPUTES_TXNS with CUSTOMER ID 
            // 
            

            SQLCmd =
          " UPDATE [ETISALAT].[dbo].[ETISALAT_CUSTOMER_DISPUTES_TXNS] "
          + " SET CustomerID = t2.CustomerID "
           + " FROM [ETISALAT].[dbo].[ETISALAT_CUSTOMER_DISPUTES_TXNS]  t1 "
          + " INNER JOIN [ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] t2"

          + " ON "
          + " t1.[Transaction Id] = t2.MeezaDigitalTrxId "
          + " WHERE   t1.[Transaction Type] <> 'AtmCashInAuthentication' AND t1.LoadedAtRMCycle = @LoadedAtRMCycle  "
           ; // 123  

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle ", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Updated CUSTOMER FROM TPF..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE ETISALAT_CUSTOMER_DISPUTES_TXNS with CUSTOMER ID based on the receiver number 
            // 
//            UPDATE[ETISALAT].[dbo].[ETISALAT_CUSTOMER_DISPUTES_TXNS]
//SET CustomerID = isnull((
//SELECT TOP 1 CustomerID
//FROM[ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] b
//where[Receiver number] = b.MSISDN
//ORDER BY SeqNo DESC
//),'Not Found Cust') 
//WHERE[Transaction Type] = 'AtmCashInAuthentication' AND LoadedAtRMCycle = 1318

            SQLCmd =
          " UPDATE [ETISALAT].[dbo].[ETISALAT_CUSTOMER_DISPUTES_TXNS] "
          + " SET CustomerID = isnull(( "
           + " SELECT TOP 1 CustomerID "
          + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] b "
          + " WHERE [Receiver number] = b.MSISDN"
          + " ORDER BY SeqNo DESC "
          + " ),'Not Found Cust')  "
          + " WHERE [Transaction Type] = 'AtmCashInAuthentication' AND LoadedAtRMCycle = @LoadedAtRMCycle "
           ; // 123  

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle ", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Updated CUSTOMER FROM TPF..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;


        }
        public void InsertRecords_ETISALAT_SURPLUS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_SURPLUS_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT SURPLUS ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT SURPLUS " + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

            //
            // UPDATE ETISALAT_CUSTOMER_SURPLUS_TXNS with CUSTOMER ID 
            // 


            SQLCmd =
          " UPDATE [ETISALAT].[dbo].[ETISALAT_CUSTOMER_SURPLUS_TXNS] "
          + " SET CustomerID = t2.CustomerID "
           + " FROM [ETISALAT].[dbo].[ETISALAT_CUSTOMER_SURPLUS_TXNS]  t1 "
          + " INNER JOIN [ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] t2"

          + " ON "
          + " t1.TransactionId = t2.MeezaDigitalTrxId "
          + " WHERE  t1.[TransactionType] <> 'AtmCashInAuthentication' AND t1.LoadedAtRMCycle = @LoadedAtRMCycle "
           ; // 123  

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle ", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Card No and Origin_1 Updated CUSTOMER FROM TPF..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE ETISALAT_CUSTOMER_SURPLUS_TXNS with CUSTOMER ID based on the receiver number 
//            // 
//            UPDATE[ETISALAT].[dbo].[ETISALAT_CUSTOMER_SURPLUS_TXNS]
//SET CustomerID = isnull((
//SELECT TOP 1 CustomerID
//FROM[ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] b
//where[Receivernumber] = b.MSISDN
//ORDER BY SeqNo DESC
//),'Not Found Cust') 
//WHERE[TransactionType] = 'AtmCashInAuthentication' AND LoadedAtRMCycle = 1318



            SQLCmd =
          " UPDATE [ETISALAT].[dbo].[ETISALAT_CUSTOMER_SURPLUS_TXNS] "
          + " SET CustomerID = isnull(( "
           + " SELECT TOP 1 CustomerID "
          + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL] b "
          + " WHERE [Receivernumber] = b.MSISDN"
          + " ORDER BY SeqNo DESC "
          + " ),'Not Found Cust')  "
          + " WHERE [TransactionType] = 'AtmCashInAuthentication' AND LoadedAtRMCycle = @LoadedAtRMCycle "
           ; // 123  

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle ", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Updated CUSTOMER FROM TPF..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;

        }

        public void InsertRecords_ETISALAT_NODE_TOTALS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_NODE_TOTALS_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT NODE TOTALS ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT NODE TOTALS " + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }

        public void InsertRecords_ETISALAT_MEEZA_TOTALS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
            
            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string WErrorText = "";

            // THE PHYSICAL DATA BASES ARE

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            //string SPName = "[ETISALAT].[dbo].[connectionString_ETI]";

            string SPName = "[ETISALAT].[dbo].[BULK_SP_ETISALAT_MEEZA_TOTALS_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString_ETI))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    SqlParameter WReconcCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WReconcCycleNo.Direction = ParameterDirection.Input;
                    WReconcCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WReconcCycleNo);

                    SqlParameter WPath = new SqlParameter("@FullPath", WFullPath_01);
                    WPath.Direction = ParameterDirection.Input;
                    WPath.SqlDbType = SqlDbType.NVarChar;
                    WPath.Size = 400;
                    cmd.Parameters.Add(WPath);
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);


                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 1024;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 40;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    WErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;

                        // File.Delete(WJournalTxtFile);
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }


            if (ret == 0)
            {
                stpErrorText += DateTime.Now + "_" + InOriginFileName + "..Finishes with SUCCESS.." + "\r\n";

                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                // UPDATE REVERSALS for ORIGINAL RECORDs
                // Return CODE
                stpReturnCode = 0;

            }
            else
            {
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading ETISALAT MEEZA TOTALS ";

                MessageBox.Show("Cancel At _Calling STP For Loading ETISALAT MEEZA TOTALS" + Environment.NewLine
                    + WErrorText);

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }


        public void InsertRecords_ETISALAT_1_SALARY_WITHDRAWLS_TXNS(string InOriginFileName, string InFullPath
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

            // Truncate Flexcube FOR BULK

            SQLCmd = "TRUNCATE TABLE [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ETISALAT].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS Insert";
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    }

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS finishes with.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            //**********************************************************
            // UPDATE Transaction Date
            //**********************************************************

            SQLCmd = "  UPDATE  [ETISALAT].[dbo].BULK_" + InOriginFileName
                        + " SET [MerchantCommission] = 0 "
                        + "  WHERE  MerchantCommission is NULL "
                        ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _Trans DATE ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            RecordFound = false;

            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [ETISALAT].[dbo].[BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS_ALL] " //  We insert in the second IST
                + " Select * " + " ,@RMCycle " + " FROM [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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
                    stpErrorText = stpErrorText + "Cancel During insert in BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS_ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // FIND DUPLICATES

            Duplicates_Salaries_Withdrawls = new DataTable();
            Duplicates_Salaries_Withdrawls.Clear();


            Duplicates_Salaries_Withdrawls.Columns.Add("CardNumber", typeof(string));
            Duplicates_Salaries_Withdrawls.Columns.Add("TransactionAmount", typeof(string));
            Duplicates_Salaries_Withdrawls.Columns.Add("AUTH_CODE", typeof(string));

            SqlString =

    "	 WITH MergedTbl AS "
   + "     ( "
 + "  select Cardnumber, TransactionAmount, AUTH_CODE , Count(*) As RecCount "
 + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS] "
// + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
+ " group by CardNumber, TransactionAmount, AUTH_CODE "
  + "     ) "
   + "     SELECT * FROM MergedTbl "
    + "	WHERE RecCount > 1 "

    ;

            using (SqlConnection conn =
                    new SqlConnection(connectionString_ETI))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);



                            // SELECT ROW
                            DataRow RowSelected = Duplicates_Salaries_Withdrawls.NewRow();

                            RowSelected["CardNumber"] = (string)rdr["Cardnumber"];
                            RowSelected["TransactionAmount"] = (string)rdr["TransactionAmount"];
                            RowSelected["AUTH_CODE"] = (string)rdr["AUTH_CODE"];

                            // ADD ROW
                            Duplicates_Salaries_Withdrawls.Rows.Add(RowSelected);

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

                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During duplicate Salarie";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;

                }

            // READ THE DUPLICATES AND INSERT IN TABLE

            int I = 0;
           
            while (I <= (Duplicates_Salaries_Withdrawls.Rows.Count - 1))
            {
                RecordFound = true;
               
                string CardNumber = (string)Duplicates_Salaries_Withdrawls.Rows[I]["CardNumber"];
                string TransactionAmount = (string)Duplicates_Salaries_Withdrawls.Rows[I]["TransactionAmount"];
                string AUTH_CODE = (string)Duplicates_Salaries_Withdrawls.Rows[I]["AUTH_CODE"];

                // INSERT IN DUPLICATES FILES
                InsertInDuplicates(CardNumber, TransactionAmount, AUTH_CODE, InReconcCycleNo); 

                I++; // Read Next entry of the table 
            }


                //
                // UPDATE FROM THE JUST LOADED
                // CREATE TABLE USED FOR MATCHING
                //
                string MatchingCateg = "ETI375";
            SQLCmd = " INSERT INTO [ETISALAT].[dbo].ETISALAT_1_SALARY_WITHDRAWLS_TXNS  "
                + "( "

                  + "[MatchingCateg] "
                 + " ,[LoadedAtRMCycle] "
                 + " , TransCurr "
                 + ", TransType "
                  + ",[ResponseCode] " //Response Code        
                   + ",[SET_DATE] "
                 + ",[RRNumber] " // PUT HERE MARCHANT Number 
                + ",[TransAmount] " // Put here the total
                  +" )"
                 + " SELECT "
                 + "@MatchingCateg"
                 + " ,@LoadedAtRMCycle"
                 + ", 'IQD' " // Currency Code
                 + ", 'SALARY WITHDRAWAL (POS)' " // Trans Type -  Description 
                 + " , '0' "
   + ", PROCESS_DATE "
   + ", MERCHANT_ID"
   + ",  ISNULL( SUM( (CAST(TransactionAmount as decimal(18, 3)) + CAST(MerchantCommission as decimal(18, 3)))  ), 0) "
 + " FROM[ETISALAT].[dbo].[BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS] "
 //+ "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle " // NO NEED FOR THIS
+ "  Group by   PROCESS_DATE, MERCHANT_ID "
+ "  ORDER by PROCESS_DATE, MERCHANT_ID "
                + " ";

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For ETISALAT_1_SALARY_WITHDRAWLS_TXNS " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_1_SALARY_WITHDRAWLS_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in ETISALAT_1_SALARY_WITHDRAWLS_TXNS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //**********************************************************
            // UPDATE Transaction Date
            //**********************************************************

            SQLCmd = "  UPDATE [ETISALAT].[dbo].ETISALAT_1_SALARY_WITHDRAWLS_TXNS "
                        + " SET [TransDate] = [SET_DATE] "
                        + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
                        ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _Trans DATE ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of TransDATE finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = true;
        }
      
       

        public void InsertRecords_ETISALAT_2_AGENTS_PAY_TXNS(string InOriginFileName, string InFullPath
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
         
            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Flexcube FOR BULK

            SQLCmd = "TRUNCATE TABLE [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "BULK_ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ETISALAT].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "BULK_ETISALAT_2_AGENTS_PAY_TXNS Insert";
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    }

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Bulk Insert finishes with.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            RecordFound = false;

            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [ETISALAT].[dbo].[BULK_ETISALAT_2_AGENTS_PAY_TXNS_ALL] " //  We insert in the second IST
                + " Select * " + " ,@RMCycle " + " FROM [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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
                    stpErrorText = stpErrorText + "Cancel During insert in BULK_ETISALAT_2_AGENTS_PAY_TXNS_ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // Work till here 
            // return;


            SqlString =

    "	 WITH MergedTbl AS "
   + "     ( "
 + "  select Cardnumber, TransactionAmount, AUTH_CODE , Count(*) As RecCount "
 + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS_ALL] "
 + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
+ " group by CardNumber, TransactionAmount, AUTH_CODE "
  + "     ) "
   + "     SELECT * FROM MergedTbl "
    + "	WHERE RecCount > 1 "

    ;

            using (SqlConnection conn =
                    new SqlConnection(connectionString_ETI))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);

                           string Cardnumber = (string)rdr["Cardnumber"];

                            string TransactionAmount = (string)rdr["TransactionAmount"];


                            string AUTH_CODE = (string)rdr["AUTH_CODE"];
                            //decimal SecondTransAmount;

                            //if (InMatchingCateg == "ETI375")
                            //{
                            //    string SelectionCriteria = " WHERE RRNumber='" + RRNumber + "'";
                            //    SecondTransAmount = Mmob.ReadTransactionAmountFromTable(SelectionCriteria, InFileIdB, W_Application);

                            //    if (SecondTransAmount != 0)
                            //    {
                            //        // Second Record exist and amount is different  
                            //        if (InPos == 2)
                            //        {
                            //            // THIS WAS FOUND WHEN We had checked the A to B file
                            //            // We prefer not to register it again
                            //            continue;
                            //        }

                            //        // HERE We continue only for cases A to B or InPos ==1 

                            //        //var valOne = Decimal.Round(1.1234560M, 6);    // Gives 1.123456
                            //        //var valTwo = Decimal.Round(1.1234569M, 6);    // Gives 1.123457
                            //        //var Dif = Math.Abs(TransAmount - SecondTransAmount);
                            //        if (Math.Abs(TransAmount - SecondTransAmount) >= Tolerance)
                            //        // if (Math.Abs(TransAmount - SecondTransAmount) >= 0.100M)
                            //        {
                            //            // Do nothing and continue to register it as discrepancy 

                            //        }
                            //        else
                            //        {
                            //            continue;
                            //        }
                            //    }


                            //}



                            //SeqNo = (int)rdr["SeqNo"];
                            //MatchingCateg = (string)rdr["MatchingCateg"];


                            //TransDate = (DateTime)rdr["TransDate"];

                            //TransCurr = (string)rdr["TransCurr"];
                            //// TransAmount = (decimal)rdr["TransAmount"];

                            //ResponseCode = (string)rdr["ResponseCode"];

                            //// Fill In Table
                            ////
                            //DataRow RowSelected = TableUnMatched.NewRow();

                            //NumberOfUnmatched = NumberOfUnmatched + 1;

                            //RowSelected["UserId"] = WSignedId;
                            //RowSelected["OriginSeqNo"] = SeqNo;
                            //RowSelected["TransDate"] = TransDate;
                            //RowSelected["WCase"] = InCase;
                            //RowSelected["M_Type"] = 4; // Not In 
                            //RowSelected["DublInPos"] = 0;
                            //RowSelected["InPos"] = InPos;
                            //RowSelected["NotInPos"] = NotInPos;

                            //RowSelected["RRNumber"] = RRNumber;

                            ////if (RRNumber ==
                            ////    "63fcbcf3-257e-4492-beb7-eb84557d3e22")
                            ////{
                            ////    RRNumber = RRNumber;
                            ////}

                            //RowSelected["TransAmount"] = TransAmount;

                            //RowSelected["MatchingCateg"] = MatchingCateg;
                            //RowSelected["RMCycle"] = WRMCycle;
                            //RowSelected["FileId"] = "";
                            //if (MatchingCateg == "ETI360")
                            //{
                            //    RowSelected["Matched_Characters"] = RRNumber;
                            //}
                            //else
                            //{
                            //    RowSelected["Matched_Characters"] = RRNumber + TransAmount.ToString();
                            //}

                            //RowSelected["ResponseCode"] = ResponseCode;

                            //// ADD ROW
                            //TableUnMatched.Rows.Add(RowSelected);


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

                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;

                }

            // UPDATE FROM THE JUST LOADED
            string MatchingCateg = "ETI375";
            SQLCmd = "INSERT INTO [ETISALAT].[dbo].ETISALAT_2_AGENTS_PAY_TXNS  "
                + "( "

                  + "[MatchingCateg] "
                 + " ,[LoadedAtRMCycle] "
                 + " , TransCurr "
                 + ", TransType "
                  + ",[ResponseCode] " //Response Code      
                   + ",[SET_DATE] "
                 + ",[RRNumber] " // PUT HERE MARCHANT Number 
                + ",[TransAmount] " // Put here the total

                 + ") "
                 + " SELECT "
                 + "@MatchingCateg"
                 + " ,@LoadedAtRMCycle"
                 + ", 'IQD' " // Currency Code
                 + ", 'AGENTS PAY' " // Trans Type -  Description 
                 + ", '0' "
   + ", PROC_DATE "
   + ", MERCHANT_ID"
   + ",  ISNULL( SUM( CAST(PAYMENT_AMOUNT as decimal(18, 3)))  , 0) "
 + " FROM[ETISALAT].[dbo].[BULK_ETISALAT_2_AGENTS_PAY_TXNS] "
//+ "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle " // NO NEED FOR THIS
+ "  Group by   PROC_DATE, MERCHANT_ID "
+ "  ORDER by PROC_DATE, MERCHANT_ID "
                + " ";

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For ETISALAT_2_AGENTS_PAY_TXNS " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in ETISALAT_2_AGENTS_PAY_TXNS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //**********************************************************
            // UPDATE Transaction Date
            //**********************************************************

            SQLCmd = "  UPDATE  [ETISALAT].[dbo].ETISALAT_2_AGENTS_PAY_TXNS "
                        + " SET [TransDate] = [SET_DATE] "
                        + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "   
                        ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransDate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of TransDATE finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = true; 
        }

        //ETISALAT_5_MONITORING_Report_Data_TXNS
        //
        //
        public void InsertRecords_ETISALAT_5_MONITORING_Report_Data_TXNS(string InOriginFileName, string InFullPath
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

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Flexcube FOR BULK

            SQLCmd = "TRUNCATE TABLE [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "BULK_ETISALAT_5_MONITORING_Report_Data_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ETISALAT].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "BULK_ETISALAT_2_AGENTS_PAY_TXNS Insert";
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    }

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            SQLCmd = "  UPDATE   [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] "
                     + " SET  [Sender_First_Name] = TRIM(Sender_First_Name)     "
                     + "  ,[Sender_Middle_Name] =TRIM(Sender_Middle_Name)    "
                      + "  ,[Sender_Last_Name] =TRIM(Sender_Last_Name)  "
                      ;
                                                           
                       
            // + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "


            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransDate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }


            //stpErrorText += DateTime.Now + "_" + "Bulk Insert finishes with.." + Counter.ToString() + "\r\n";
            //stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE THE CARD FROM CONNECTOR
            SQLCmd =
         " UPDATE [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] "
         + " SET Card_Number = t2.TRE_CAR_NUMB "
         //
         + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] t1 "
         + " INNER JOIN [ETISALAT].[dbo].[BULK_ETISALAT_7_MoneyGrane_Connector_TXNS] t2"

         + " ON "
        
         + "  t1.Sender_First_Name = t2.TRE_KYC_S_CUST_FNAME" //terminal not the same for 123 
         + " AND t1.Sender_Middle_Name = t2.TRE_KYC_S_CUST_SNAME "
         + " AND t1.Sender_Last_Name = t2.TRE_KYC_S_CUST_LNAME "
         + " AND t1.Face_Amount = t2.TRE_TRANSFER_AMOU "

         ;  // 


            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

            stpErrorText += DateTime.Now + "_" + "UPDATE CARD FROM Connector..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            RecordFound = false;

            // KEEP   
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO BULK_ETISALAT_5_MONITORING_Report_Data_TXNS_ALL " //  We insert in the second IST
                + " Select * " + " ,@RMCycle " + " FROM BULK_" + InOriginFileName
                + " WHERE  Reference_Number is not Null"
                ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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
                    stpErrorText = stpErrorText + "Cancel During insert in BULK_ETISALAT_2_AGENTS_PAY_TXNS_ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

          
            // UPDATE FROM THE THE ALL

            string MatchingCateg = "ETI376";
            SQLCmd = "INSERT INTO ETISALAT_5_MONITORING_Report_Data_TXNS  "
                + "( "  
                 + " OriginalRecordId "
                  + ", [MatchingCateg] "
                 + " ,[LoadedAtRMCycle] "
                 + " , TransCurr "
                  + ",[TransAmount] " // 
                  + ",TransDate"
                 + ", TransType "
                  + ",[ResponseCode] " //Response Code   
                   + ",[RRNumber] " // CARD Number
                   + ",[SET_DATE] "
                
                 + ") "
                 + " SELECT "
                 + " SeqNo "
                 + ", @MatchingCateg"
                 + " ,@LoadedAtRMCycle"
                 + ", Sender_Local_Currency_Code " // Currency Code
                  + ", Face_Amount " // TRANS AMOUNT
                  + " ,  CAST(Send_Date as Date) "
                 + ", 'Monitoring Report'  " // Trans Type -  Description 
                 + ", '0' "
                 + ", Card_Number "
              + ", CAST(Send_Date as Date) "       
 
 + " FROM BULK_ETISALAT_5_MONITORING_Report_Data_TXNS_ALL "
+ "  WHERE LoadedAtRMCycle =@LoadedAtRMCycle "
+ "   "
                + " ";

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For ETISALAT_5_MONITORING_Report_Data_TXNS " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_5_MONITORING_Report_Data_TXNS ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in ETISALAT_5_MONITORING_Report_Data_TXNS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            

            RecordFound = true;
        }

        //
        public void InsertRecords_ETISALAT_6_CLEARING_REPORT_TXNS(string InOriginFileName, string InFullPath
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

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Flexcube FOR BULK

            SQLCmd = "TRUNCATE TABLE [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "BULK_ETISALAT_6_CLEARING_REPORT_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ETISALAT].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "BULK_ETISALAT_6_CLEARING_REPORT_TXNS Insert";
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    }

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**********************************************************
            // UPDATE CORRECT AMOUNTe
            //**********************************************************

            SQLCmd = "  UPDATE   [ETISALAT].[dbo].[BULK_ETISALAT_6_CLEARING_REPORT_TXNS] "
                        + " SET  [_TRAN_AMOUNT_] = REPLACE(REPLACE(_TRAN_AMOUNT_, '\"', ''), ',', '')     "  ; 
                        // + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
                        

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransDate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }
          

            RecordFound = true;

            // KEEP   
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO BULK_ETISALAT_6_CLEARING_REPORT_TXNS_ALL " //  We insert in the second IST
                + " Select * " + " ,@RMCycle " + " FROM BULK_" + InOriginFileName
                + " WHERE ISSUER_BANK is not null "
                ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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
                    stpErrorText = stpErrorText + "Cancel During insert in BULK_ETISALAT_6_CLEARING_REPORT_TXNS_ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE FROM THE THE ALL

            string MatchingCateg = "ETI376";
            SQLCmd = "INSERT INTO ETISALAT_6_CLEARING_REPORT_TXNS  "
                + "( "
                 + " OriginalRecordId "
                  + ", [MatchingCateg] "
                 + " ,[LoadedAtRMCycle] "
                 + " , TransCurr "
                  + ",[TransAmount] " // 
                  + ",TransDate"
                 + ", TransType "
                  + ",[ResponseCode] " //Response Code   
                   + ",[RRNumber] " // CARD Number
                   + ",[SET_DATE] "

                 + ") "
                 + " SELECT "
                 + " SeqNo "
                 + ", @MatchingCateg"
                 + " ,@LoadedAtRMCycle"
                 + ", 'USD' " // Currency Code
                  + ", _TRAN_AMOUNT_ " // TRANS AMOUNT
                  + " ,   Cast (TRANSACTION_DATE as Date) "
                 + ", 'Money Gram Transfer'  " // Trans Type -  Description 
                 + ", '0' "
                 + ", CARD_NUMBER "
              + ",  Cast (PROC_DATE as Date) "

 + " FROM BULK_ETISALAT_6_CLEARING_REPORT_TXNS_ALL "
+ "  WHERE LoadedAtRMCycle =@LoadedAtRMCycle "
+ "   "
                + " ";

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For ETISALAT_6_CLEARING_REPORT_TXNS " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_6_CLEARING_REPORT_TXNS ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in ETISALAT_6_CLEARING_REPORT_TXNS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = true;
        }

        public void InsertRecords_ETISALAT_7_MoneyGrane_Connector_TXNS(string InOriginFileName, string InFullPath
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

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Truncate Flexcube FOR BULK

            SQLCmd = "TRUNCATE TABLE [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "BULK_ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ETISALAT].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
                         + " ,ROWTERMINATOR = '\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "BULK_ETISALAT_2_AGENTS_PAY_TXNS Insert";
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..Data_Format of.." + Environment.NewLine
                        + WFullPath_01
                       );
                    }

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = true;

            //    " UPDATE [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] "
            //+ " SET Card_Number = t2.TRE_CAR_NUMB "
            ////
            //+ " FROM [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] t1 "
            //+ " INNER JOIN [ETISALAT].[dbo].[BULK_ETISALAT_7_MoneyGrane_Connector_TXNS] t2"

            //+ " ON "

            //+ "  t1.Sender_First_Name = t2.TRE_KYC_S_CUST_FNAME" //terminal not the same for 123 
            //+ " AND t1.Sender_Middle_Name = t2.TRE_KYC_S_CUST_SNAME "
            //+ " AND t1.Sender_Last_Name = t2.TRE_KYC_S_CUST_LNAME "
            //+ " AND t1.Face_Amount = t2.TRE_TRANSFER_AMOU "

            //         UPDATE[ETISALAT].[dbo].[BULK_ETISALAT_7_MoneyGrane_Connector_TXNS]
            //SET
            //   [TRE_KYC_S_CUST_FNAME] = TRIM(TRE_KYC_S_CUST_FNAME)
            //   ,[TRE_KYC_S_CUST_SNAME] =TRIM(TRE_KYC_S_CUST_SNAME)
            //   ,[TRE_KYC_S_CUST_LNAME] =TRIM(TRE_KYC_S_CUST_LNAME)

            //SQLCmd = "  UPDATE   [ETISALAT].[dbo].[BULK_ETISALAT_5_MONITORING_Report_Data_TXNS] "
            //          + " SET  [Sender_First_Name] = TRIM(Sender_First_Name)     "
            //          + "  ,[Sender_Middle_Name] =TRIM(Sender_Middle_Name)                                                              "
            //           + "  ,[Sender_Last_Name] =TRIM(Sender_Last_Name)  "
            //           ;

            SQLCmd = "  UPDATE   [ETISALAT].[dbo].[BULK_ETISALAT_7_MoneyGrane_Connector_TXNS] "
                       + " SET  [TRE_KYC_S_CUST_FNAME] = TRIM(TRE_KYC_S_CUST_FNAME)     "
                       + "  ,[TRE_KYC_S_CUST_SNAME] =TRIM(TRE_KYC_S_CUST_SNAME)       "
                        + "  ,[TRE_KYC_S_CUST_LNAME] =TRIM(TRE_KYC_S_CUST_LNAME)        "
                       ;
            // + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "


            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransDate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            return;

            RecordFound = false;

            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [ETISALAT].[dbo].[BULK_ETISALAT_2_AGENTS_PAY_TXNS_ALL] " //  We insert in the second IST
                + " Select * " + " ,@RMCycle " + " FROM [ETISALAT].[dbo].BULK_" + InOriginFileName;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
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
                    stpErrorText = stpErrorText + "Cancel During insert in BULK_ETISALAT_2_AGENTS_PAY_TXNS_ALL";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // Work till here 
            // return;


            SqlString =

    "	 WITH MergedTbl AS "
   + "     ( "
 + "  select Cardnumber, TransactionAmount, AUTH_CODE , Count(*) As RecCount "
 + " FROM [ETISALAT].[dbo].[BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS_ALL] "
 + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
+ " group by CardNumber, TransactionAmount, AUTH_CODE "
  + "     ) "
   + "     SELECT * FROM MergedTbl "
    + "	WHERE RecCount > 1 "

    ;

            using (SqlConnection conn =
                    new SqlConnection(connectionString_ETI))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);

                            string Cardnumber = (string)rdr["Cardnumber"];

                            string TransactionAmount = (string)rdr["TransactionAmount"];


                            string AUTH_CODE = (string)rdr["AUTH_CODE"];
                            //decimal SecondTransAmount;




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

                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;

                }

            // UPDATE FROM THE JUST LOADED
            string MatchingCateg = "ETI375";
            SQLCmd = "INSERT INTO [ETISALAT].[dbo].ETISALAT_2_AGENTS_PAY_TXNS  "
                + "( "

                  + "[MatchingCateg] "
                 + " ,[LoadedAtRMCycle] "
                 + " , TransCurr "
                 + ", TransType "
                  + ",[ResponseCode] " //Response Code      
                   + ",[SET_DATE] "
                 + ",[RRNumber] " // PUT HERE MARCHANT Number 
                + ",[TransAmount] " // Put here the total

                 + ") "
                 + " SELECT "
                 + "@MatchingCateg"
                 + " ,@LoadedAtRMCycle"
                 + ", 'IQD' " // Currency Code
                 + ", 'AGENTS PAY' " // Trans Type -  Description 
                 + ", '0' "
   + ", PROC_DATE "
   + ", MERCHANT_ID"
   + ",  ISNULL( SUM( CAST(PAYMENT_AMOUNT as decimal(18, 3)))  , 0) "
 + " FROM[ETISALAT].[dbo].[BULK_ETISALAT_2_AGENTS_PAY_TXNS] "
//+ "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle " // NO NEED FOR THIS
+ "  Group by   PROC_DATE, MERCHANT_ID "
+ "  ORDER by PROC_DATE, MERCHANT_ID "
                + " ";

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For ETISALAT_2_AGENTS_PAY_TXNS " + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During Inserted For ETISALAT_2_AGENTS_PAY_TXNS";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in ETISALAT_2_AGENTS_PAY_TXNS with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //**********************************************************
            // UPDATE Transaction Date
            //**********************************************************

            SQLCmd = "  UPDATE  [ETISALAT].[dbo].ETISALAT_2_AGENTS_PAY_TXNS "
                        + " SET [TransDate] = [SET_DATE] "
                        + "  WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
                        ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

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
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransDate";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of TransDATE finishes .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = true;
        }
        //*************************************
        // AFTER FILE LOADING UPDATE / SYCHRONISE FILES
        //**************************************
        int Counter = 0;

        //
        // AUTO Complement files 
        //*************************************
        //public void UpdateFiles_With_EXTRA(string InOperator, int InReconcCycleNo)
        //{
        //    try
        //    {
        //        // *****************************
        //        // UPDATE FILE WITH EXTRAS 
        //        RRDMMatchingBankToRRDMFileFields_EXTRA Me = new RRDMMatchingBankToRRDMFileFields_EXTRA();

        //        Me.ReadExtraFieldsAndFillTable_Distinct(InOperator);

        //        if (Me.DataTableExtraFields.Rows.Count > 0)
        //        {
        //            int I = 0;

        //            while (I <= (Me.DataTableExtraFields.Rows.Count - 1))
        //            {
        //                //    RecordFound = true;
        //                string TableId = (string)Me.DataTableExtraFields.Rows[I]["TableId"];

        //                string SQLCmd;

        //                Me.ReadTable_EXTRA_AND_CREATE_COMMAND(TableId);
        //                //
        //                // Assign created command
        //                //
        //                SQLCmd = Me.FullCreatedSqlCommand;
        //                RRDM_LoadFiles_InGeneral_Auto La = new RRDM_LoadFiles_InGeneral_Auto();

        //                La.UPDATE_FILES_With_EXTRA(WOperator, InReconcCycleNo, SQLCmd);
        //                // ***************************

        //                I = I + 1;
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        CatchDetails(ex);
        //    }
        //}

        // DELETE RECORDS TO SET STARTING DATE
        //

        // Update After Matching the created records 01 and 011 with Replenishemnt Cycle 
        //
        private void InsertInDuplicates(string InCardNumber, string InTransactionAmount, string InAUTH_CODE, int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int CountDel;

            using (SqlConnection conn =
                new SqlConnection(connectionString_ETI))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ETISALAT].[dbo].[ETISALAT_1_SALARY_WITHDRAWLS_TXNS_DUPLICATES] "
                             + " WHERE CardNumber = @CardNumber AND TransactionAmount = @TransactionAmount" +
                             " AND AUTH_CODE = @AUTH_CODE  AND LoadedAtRMCycle=@LoadedAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        cmd.Parameters.AddWithValue("@TransactionAmount", InTransactionAmount);
                        cmd.Parameters.AddWithValue("@AUTH_CODE", InAUTH_CODE);

                        //rows number of record got updated

                        CountDel = cmd.ExecuteNonQuery();


                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

            ErrorFound = false;
            ErrorOutput = "";

           string SQLCmd = " INSERT INTO [ETISALAT].[dbo].[ETISALAT_1_SALARY_WITHDRAWLS_TXNS_DUPLICATES] " //  We insert in the second IST
               + " Select * " + " ,@LoadedAtRMCycle " + " FROM [ETISALAT].[dbo].BULK_ETISALAT_1_SALARY_WITHDRAWLS_TXNS"
               + " WHERE CardNumber = @CardNumber AND TransactionAmount = @TransactionAmount AND AUTH_CODE = @AUTH_CODE " 
               ;
            ;

            using (SqlConnection conn = new SqlConnection(connectionString_ETI))
                try
                {

                    conn.Open();
                    conn.StatisticsEnabled = true;
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@CardNumber", InCardNumber);
                        cmd.Parameters.AddWithValue("@TransactionAmount", InTransactionAmount);
                        cmd.Parameters.AddWithValue("@AUTH_CODE", InAUTH_CODE);

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
                    stpErrorText = stpErrorText + "Cancel During insert in INSERT OF DUPLICATES";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_TO SALARIES DUPLICATES.Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
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
            string WSelectionCriteria = "Operator = '" + InOperator + "' ";
            Ms.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;

            while (I <= (Ms.SourceFilesDataTable.Rows.Count - 1))
            {
                string SourceFile_ID = (string)Ms.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string OriginSystem = (string)Ms.SourceFilesDataTable.Rows[I]["OriginSystem"];
                string DbTblName = (string)Ms.SourceFilesDataTable.Rows[I]["DbTblName"];
                string TableStructureId = (string)Ms.SourceFilesDataTable.Rows[I]["TableStructureId"];

                if(TableStructureId == "ETISALAT")
                {
                    I = I + 1;
                    continue; 
                }

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

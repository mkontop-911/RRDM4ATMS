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
    public class RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_QAHERA : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_BDC_MOBILE_QAHERA() : base() { }
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

        public void InsertRecordsInTableFromTextFile_InBulk_QAHERA(string InOperator, string InTableA
                                           , string InFullPath, string InCondition, string InFullPath2, int InFlogSeqNo, int InReconcCycleNo)
        {
            // WE WANT as INPUT
            // a) FILE NAME => to see what Method to call eg Switch_IST_Txns 
            // a) Directory and FILE NAME to know whare to get from 

            // WE SHOULD PROVIDE AS OUTPUT
            // a) Status 
            // b) Level of process 

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
            string text = "In Process Loading of .. " + InTableA;
            string caption = "LOADING OF FILES";
            int timeout = 2000;
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }

            switch (InTableA)
            {
                case "QAHERA_TPF_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_TPF_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_TPF_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_MEEZA_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_MEEZA_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_MEEZA_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "QAHERA_AMAN_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_AMAN_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_AMAN_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_FAWRY_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_FAWRY_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_FAWRY_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_DISPUTE_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_DISPUTE_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_DISPUTE_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_SURPLUS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_SURPLUS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_SURPLUS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_NODE_TOTALS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_NOTES_TOTALS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_NODE_TOTALS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_MEEZA_TOTALS_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_MEEZA_TOTALS_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_MEEZA_TOTALS_TXNS(InTableA, InFullPath, WReconcCycleNo);

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
                case "QAHERA_BDCSW_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_QAHERA_BDCSW_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_QAHERA_BDCSW_TXNS(InTableA, InFullPath, WReconcCycleNo);

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

                case "MEEZA_UPG_MOBILE":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MEEZA_UPG_MOBILE" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        //InsertRecords_GTL_MEEZA_UPG_MOBILE(InTableA, InFullPath, WReconcCycleNo);

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

                case "MOBILE_TXNS_UPG":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_E_FINANCE_D_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                        InsertRecords_GTL_MOBILE_TXNS_UPG(InTableA, InFullPath, WReconcCycleNo);

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
        //// CREATE QAHERA_TPF_TXNS
        //
        int ReturnCode;
        string ErrorText;
        string ErrorReference;
        int ret; 

        public void InsertRecords_QAHERA_TPF_TXNS(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            ErrorText = "";
            ErrorReference = ""; 
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

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_TPF_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA TPF ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }
            
        }

        public void InsertRecords_QAHERA_MEEZA_TXNS(string InOriginFileName, string InFullPath
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

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_MEEZA_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA MEEZA ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }


        public void InsertRecords_QAHERA_AMAN_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_AMAN_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_AMAN_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }


        public void InsertRecords_QAHERA_FAWRY_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_FAWRY_TXNS]";
            //[dbo].[BULK_SP__QAHERA_FAWRY_TXNS]
            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_FAWRY_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }

        public void InsertRecords_QAHERA_DISPUTE_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_DISPUTE_TXNS]";
            //[dbo].[BULK_SP__QAHERA_FAWRY_TXNS]
            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_DISPUTE_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }


        public void InsertRecords_QAHERA_SURPLUS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            ErrorReference = ""; 

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_SURPLUS_TXNS]";
            //[dbo].[BULK_SP__QAHERA_FAWRY_TXNS]
            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_SURPLUS_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }

        public void InsertRecords_QAHERA_NODE_TOTALS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {
           // InsertRecords_QAHERA_NOTES_TOTALS_TXNS
            ErrorFound = false;
            ErrorOutput = "";

            ErrorText = "";
            ErrorReference = ""; 

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_NODE_TOTALS_TXNS]";
            //[dbo].[BULK_SP__QAHERA_FAWRY_TXNS]
            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_NODE_TOTALS_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }

        public void InsertRecords_QAHERA_MEEZA_TOTALS_TXNS(string InOriginFileName, string InFullPath
                                   , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;
            //    stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            string SQLCmd;

            string WTerminalType = "10";

            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_MEEZA_TOTALS_TXNS]";
            //[dbo].[BULK_SP__QAHERA_FAWRY_TXNS]
            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_MEEZA_TOTALS_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }
        public void InsertRecords_QAHERA_BDCSW_TXNS(string InOriginFileName, string InFullPath
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

            string QAHERAConnectionString = ConfigurationManager.ConnectionStrings
                  ["QAHERAConnectionString"].ConnectionString;

            string SPName = "[QAHERA].[dbo].[BULK_SP_QAHERA_SW34_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(QAHERAConnectionString))
            {
                try
                {
                    ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@FullPath", WFullPath_01));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 750;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

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
                stpErrorText = stpErrorText + "Cancel At _Calling STP For Loading QAHERA_BDCSW_TXNS ";

                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
            }

        }


        //
        //
        //// MOBILE_TXNS_UPG
        //
        public void InsertRecords_GTL_MOBILE_TXNS_UPG(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_UPG_MOBILE]

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

            // Find LAST SEQ NO in BULK_"+InOriginFileName+"_ALL "
          
            LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL  "
               + "WITH(NOLOCK) "
               + "WHERE LoadedAtRMCycle = @LoadedAtRMCycle";

            using (SqlConnection conn =
                             new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
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

            // KEEP BULK 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_"+InOriginFileName+"_ALL " //  We insert in the second BULK
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
                    stpErrorText = stpErrorText + "Cancel During Creation of Flexcube";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Master Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = false;

            // Find LAST SEQ NO in Switch_IST_Txns 
            // REASON: We needed it when we populate TWIN to read from IST all > that this SEQ NO
            
            if (LastSeqNo == 0)
            {
                SQLCmd = "SELECT ISNULL(MIN(SeqNo), 0) AS MINSeqNo "
              + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL "
              + "WITH(NOLOCK) "
              + "WHERE LoadedAtRMCycle = @LoadedAtRMCycle";

                using (SqlConnection conn =
                                 new SqlConnection(ATMSconnectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                LastSeqNo = (int)rdr["MINSeqNo"];

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
            }



            // INSERT TXNS FOR MOBILE
            //
            RecordFound = false;

            SQLCmd =
                   // "SET dateformat ymd "
                   " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
                   + "( "
                   + " [OriginFileName] "  // 01
         + ",[OriginalRecordId] " // 02
         + ",[LoadedAtRMCycle] "  // 03
         + " ,[MatchingCateg] "  // 04
         + " ,[Origin] " // 05
         + " ,[TransactionId] " // 06
         + " ,[Type] "  //07
                    + " ,[Senderscheme] " // 08
                    + " ,[Receiverscheme] " //09
                    + " ,[Sendernumber] "  //10
                    + " ,[Receivernumber] " //11
                    + " ,[TransDate] " // 12
                    + " ,[TransAmt] " // 13
                     + " ,[TransCurr] " // 01
                     + " ,[Interchangeamount] " // 02
                     + " ,[SenderInterchangecurrency] " // 03
                     + " ,[SenderInterchangeAction] " // 04
                     + " ,[ReceiverInterchangeAction] " // 05
                     + " ,[ResponseCode] " //06
                     + " ,[Responsedescription] " //07
                     + " ,[TransactionReference] " //08
                     + " ,[Sendername] " // 09
                     + " ,[Receivername] " //10
                     + " ,[TerminalId] " //11

                     + " ,[Comment1] " // 01
                     + " ,[Comment2] " // 02

                     + " ,[TXNSRC] " //03
                     + " ,[TXNDEST] " //04
                     + " ,[Net_TransDate] " // 05
                     + "  ,[CAP_DATE] " // 06
                     + " ,[SET_DATE] " // 07
                    + "  ,[Operator] " // 08
                    + ") "

                    + " SELECT "
             + " @OriginFile " // 01
         + " ,SeqNo " // SEQ NUMBER FROM THE ALL // 02 
         + " ,@LoadedAtRMCycle " // 03
         + " , CASE " // 04
         + " WHEN ISNULL(Type , 'Not Def') = 'Meeza Digital Receive (P)' THEN '" + PRX + "700' "  // MatchingCateg P2M// Meeza Digital Receive (P)
         + " WHEN ISNULL(Type , 'Not Def') = 'Meeza Digital Send' THEN '" + PRX + "701' "  // MatchingCateg M2M// Meeza Digital Send
         + " ELSE ISNULL(Type , 'Not Def')  "
         + " END "
         + " ,@Origin " //05
                        // + " ,ISNULL(NetworkReference, '')" //NetworkReference//TransactionId  // 06
        + " ,ISNULL(TransactionId, '')" //NetworkReference//TransactionId  // 06
         + " ,ISNULL(Type , 'Not Def') " // Same  // 07
                       + "  ,ISNULL(PayerSchemeName, '') " // PayerSchemeName // Senderscheme // 08
                       + " ,'BDC-UPG' " // Receiverscheme // BDC-UPG // 09
                       + " , ISNULL(Mobile, '')  " // Sendernumber // Mobile //10
                       + " ,'Is Empty' " // Receivernumber // IS Empty //11
                                         //+ " , CAST(Date as datetime) "  // TransactionTimestamp // Date
                                         // + " , @DATE "  // TransactionTimestamp // Date // 12
                                         //ISNULL(Type , 'Not Def')
          + " ,ISNULL(try_parse(TRIM('\"' FROM TranDate) as datetime), '1901-12-01') "
           + " ,ISNULL(CAST([Amount] As decimal(18,2)), 0)  " // Amount // 13
          + " ,ISNULL(Currency, '') " // Currency //01
                                                        + " , 0  " // Interchangeamount // 02
                                                           + " ,'Is Empty' " // SenderInterchangecurrency // 03 
                                                           + " ,'Is Empty' " // SenderInterchangeAction // 04
                                                           + " ,'Is Empty' " // ReceiverInterchangeAction // 05

              + " , CASE " // Response // 06
              + "  WHEN Response = '0 - Approved' THEN '0' "
              + "  ELSE ISNULL(Response, '') "
              + "  END "

              + " ,ISNULL(Response , '') " //07
              + " , 'Is Empty' " // TransactionReference //08
              + " ,'Is Empty' " // Sendername // 09
              + " ,ISNULL(MerchantName, '') " // MerchantName // 10
              + " ,ISNULL(Terminal_Id, '') " // TerminalId // Terminal_Id // 11

              + " , '' "    //[Comment1] " // 01
              + " , '' "   //[Comment2] " // 02

              + " ,'71' " // [TXNSRC] // 03
              + " ,'71' " // [TXNDEST] // 04
              + " , ISNULL(try_parse(TRIM('\"' FROM TranDate) as date), '1901-12-01') " // NET DATE // 05
              + "  ,@DATE " // CAP DATE // 06
              + " , @DATE "  // SET DATE // 07
             + "  , @Operator " // 08
             + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
                    + " WHERE SeqNo >= @LastSeqNo AND  ISNULL(try_parse(TRIM('\"' FROM TranDate) as date), '1901- 12 - 01')  <> '1901 - 12 - 01' "
                           + " ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LastSeqNo", LastSeqNo);
                        cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@DATE",FileDATEresult);


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

                    stpErrorText = stpErrorText + "Cancel During INSERT " + InOriginFileName;
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + InOriginFileName + "INSERT TXNS with..." + stpLineCount.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            
            //
            stpErrorText += DateTime.Now + "_" + InOriginFileName + " Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // UPDATE REVERSALS for ORIGINAL RECORDs
            // Return CODE
            stpReturnCode = 0;

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

                if(TableStructureId == "QAHERA")
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

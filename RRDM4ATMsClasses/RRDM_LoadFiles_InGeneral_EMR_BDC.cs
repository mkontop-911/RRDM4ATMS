using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
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
    public class RRDM_LoadFiles_InGeneral_EMR_BDC : Logger
    {
        public RRDM_LoadFiles_InGeneral_EMR_BDC() : base() { }
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
        readonly DateTime Meeza_Global_Date = new DateTime(2022, 02, 01);

        string SqlString; // Do not delete 

        string WFileSeqNo = "";
        string WOperator;

        DateTime NewVersionDt;
        DateTime MeezaNewVersionDt;
        DateTime TwoCcyNewVersionDt;

        bool MeezaIsPresent;
        bool MasterTwoCurrencies;

        public DataTable TableATMsDailyStats = new DataTable();

        RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        readonly string ATMSconnectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        readonly string recconConnString = AppConfig.GetConnectionString("ReconConnectionString");
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
                string DB_Location = AppConfig.Configuration["DB_Location_Local"];
                if (DB_Location == "true")
                    DB_Location_Local = true;
                else
                {
                    DB_Location_Local = false;
                }

                DB_Location = AppConfig.Configuration["DB_Location_Remote"];
                // DB_Location = "true"; 
                if (DB_Location == "true")
                {
                    DB_Location_Remote = true;
                    DB_Location_Calling_IP = AppConfig.Configuration["DB_Location_Calling_IP"];

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



            //  string WFullPath =  InFullPath; 

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
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("822 parameter date is wrong");
                    }

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

            MeezaNewVersionDt = new DateTime(2050, 03, 24);
            ParId = "822"; // When version of files changes 
            OccurId = "02"; // For IST and flexube and Meeza Global LCL  
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    MeezaNewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);
                    MeezaIsPresent = true;

                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("822 parameter date is wrong for Meeza");
                    }

                    MeezaIsPresent = false;
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
                MeezaIsPresent = false;
            }


            TwoCcyNewVersionDt = new DateTime(2050, 03, 24);
            ParId = "822"; // When version of files changes 
            OccurId = "03"; // For IST Two Currencies 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    TwoCcyNewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);

                    MasterTwoCurrencies = true;

                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("822 parameter date is wrong for two currency");
                    }

                    MasterTwoCurrencies = false;
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
                MasterTwoCurrencies = false;
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
            if (Environment.UserInteractive)
            {
                AutoClosingMessageBox.Show(text, caption, timeout);
            }


            switch (InTableA)
            {
                case "Switch_IST_Txns":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_IST" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        // Switch_IST_Txns_20210324
                        //MeezaNewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);
                        //MeezaIsPresent = true;

                        if (MeezaIsPresent == true & WCut_Off_Date >= MeezaNewVersionDt)
                        {
                            if (Environment.UserInteractive)
                            {
                                if (WCut_Off_Date == MeezaNewVersionDt)
                                {
                                    MessageBox.Show("THIS IS THE NEW VERSION WHICH IS ACTIVATED ON THE.... " + MeezaNewVersionDt.ToShortDateString() + Environment.NewLine
                                        + "DINA PLEASE UPDATE RRDM THAT YOU HAVE RECEIVED THIS MESSAGE "
                                        );
                                }
                            }

                            InsertRecords_GTL_IST_3(InTableA, InFullPath, WReconcCycleNo);
                        }
                        else
                        {
                            InsertRecords_GTL_IST_2(InTableA, InFullPath, WReconcCycleNo);
                        }


                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                       + SavedOriginalInFullPath);
                            }


                        }
                        //MessageBox.Show("Loaded "+ InTableA); 
                        break;
                    }
                case "Flexcube":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_Flexcube" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        if (MeezaIsPresent == true & WCut_Off_Date >= MeezaNewVersionDt)
                        {
                            InsertRecords_GTL_FLEXCUBE_3(InTableA, InFullPath, WReconcCycleNo);
                        }
                        else
                        {
                            InsertRecords_GTL_FLEXCUBE_2(InTableA, InFullPath, WReconcCycleNo);
                        }

                        //if (WCut_Off_Date >= Meeza_Global_Date & Environment.MachineName == "RRDM-PANICOS")
                        //{
                        //    InsertRecords_GTL_FLEXCUBE_3(InTableA, InFullPath, WReconcCycleNo);
                        //}
                        //else
                        //{
                        //    InsertRecords_GTL_FLEXCUBE_2(InTableA, InFullPath, WReconcCycleNo);
                        //}

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + SavedOriginalInFullPath
                                                         );
                            }

                        }
                        //MessageBox.Show("Loaded " + InTableA);
                        break;
                    }

                case "MEEZA_GLOBAL_LCL":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_MEEZA_GLOBAL" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_MEEZA_GLOBAL_LCL(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                              + SavedOriginalInFullPath
                                                           );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + SavedOriginalInFullPath
                                                         );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                              + SavedOriginalInFullPath
                                                           );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                               + SavedOriginalInFullPath
                                                            );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                              + SavedOriginalInFullPath
                                                           );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                            + SavedOriginalInFullPath
                                                         );
                            }

                        }
                        //MessageBox.Show("Loaded " + InTableA);
                        break;
                    }
                case "CIT_EXCEL_TO_BANK":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_CIT_EXCEL_TO_BANK" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                        InsertRecords_GTL_CIT_EXCEL_TO_BANK(InTableA, InFullPath, WReconcCycleNo);

                        //  InsertRecords_GTL_CIT_Speed_Excel(InTableA, InFullPath, WReconcCycleNo);

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

                case "E_FINANCE_D_TXNS":
                    {

                        stpErrorText = DateTime.Now + "_" + "01_Start_Loading_E_FINANCE_D_TXNS" + "\r\n";

                        Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                        InsertRecords_GTL_E_FINANCE_D_TXNS(InTableA, InFullPath, WReconcCycleNo);

                        if (stpReturnCode == 0)
                        {

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                               + SavedOriginalInFullPath
                                                            );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                             + SavedOriginalInFullPath
                                                          );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                             + SavedOriginalInFullPath
                                                          );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                             + SavedOriginalInFullPath
                                                          );
                            }

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

                            Ed.MoveFileToArchiveDirectory(WOperator, WReconcCycleNo, ReversedCut_Off_Date, InTableA, SavedOriginalInFullPath);
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                MessageBox.Show("Error with Loading file:.." + Environment.NewLine
                                             + SavedOriginalInFullPath
                                                          );
                            }

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

            stpErrorText += DateTime.Now + "_" + "Stage_02_Bulk_Insert_Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Correct Origin and Destination for MEEZA
            // 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [TXNSRC] = '18' " // SOURCE
                                               // + " WHERE ([TXNSRC] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808' ) OR ([TXNSRC] = '8' AND LEFT(TXN,1) = '0') )"
                    + " WHERE ( [TXNSRC] = '8' AND Left(MASK_PAN,3) = '507' )  OR ([TXNSRC] = '8' AND LEFT(TXN,1) = '0')  "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [TXNDEST] = '18' " // DESTINATION
                                                // + " WHERE [TXNDEST] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808')"
                     + " WHERE [TXNDEST] = '8' AND Left(MASK_PAN,3) = '507' "
                        ;

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
            //

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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                   + " ,case "
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) <> '81')  THEN '" + PRX + "201' " // Only DEBIT CARD for Flex 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "207' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5')  THEN '" + PRX + "206' " // All outgoing Master
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "205' " // All outgoing 123 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "208' " // All outgoing MEEZA

                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) <> '81' AND AMOUNT <> '0') "
                                                                                       + "THEN '" + PRX + "204' " // Credit Card
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '12') AND left([TXN], 2) <> '81'  THEN '" + PRX + "203'" // Prepaid 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0') "
                                                                                    + " THEN '" + PRX + "202' " // FAWRY -- All outgoing CREDIT Card Going Through Master Card

                  + " WHEN (TXNSRC = '1' "
                  + " AND TXNDEST <> '1' "
                  + " AND TXNDEST <> '4'"
                  + " AND TXNDEST <> '5' "
                  + " AND TXNDEST <> '8'"
                  + " AND TXNDEST <> '13'"
                  + " AND TXNDEST <> '12'"
                  + " AND TXNDEST <> '18'"
                       + ")  THEN '" + PRX + "202' " // All others  

                  + " WHEN (TXNSRC = '8' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                         //  + "  AND (LEFT([MASK_PAN],6) = '526402' )"
                                         + " THEN '" + PRX + "210' " // All incoming 123 NET - DEBIT
                   + " WHEN (TXNSRC = '8' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                         // + " AND LEFT([MASK_PAN],6) <> '526402') "
                                         + " THEN '" + PRX + "211' " // All incoming 123 NET - NON DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                         + " THEN '" + PRX + "270' " // All incoming MEEZA DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                         + " THEN '" + PRX + "271' " // All incoming MEEZA PREPAID
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '1' AND TXN = '0-POS Purchase') THEN '" + PRX + "272' " // All incoming 123 NET - MEEZA - POS DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '12' AND TXN = '0-POS Purchase') THEN '" + PRX + "273' " // All incoming 123 NET - MEEZA - POS NON DEBIT
                   + " WHEN (TXNSRC = '4' AND TXNDEST = '1') THEN '" + PRX + "220' " // All Incoming Visa
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "230' " // All Incoming Master to Flex
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
                   + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "231' " // All Incoming POS
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231

                   + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN '" + PRX + "250' " // FAWRY THAT GOES TO FLEXCUBE

                  + " else 'Not_Def' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "
                 + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                    //+ ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                 + ",case "
                 + " when left([TXN], 2) = '10' THEN 11 "
                 + " when left([TXN], 2) = '11' THEN 11 "
                 + " when left([TXN], 2) = '12' THEN 11 "
                 + " when left([TXN], 2) = '13' THEN 11 "
                 + " when left([TXN], 2) = '30' THEN 11 "
                 + " when left([TXN], 2) = '81' THEN 11 "
                 + " when left([TXN], 2) = '21' THEN 23 "
                 + " when left([TXN], 2) = '40' THEN 33 "
                 + " when left([TXN], 2) = '41' THEN 33 "
                 + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                 + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                 + " else 11 "
                 + "end "
                 + ",[TXN] " // [TransDescr]

                 + ", ISNULL([ACQ_CURRENCY_CODE], '') "
                 + ",CAST([AMOUNT] As decimal(18,2)) "
                 + ",CAST(Right([TRACE],6) as int) " // TRace 

                  + ",case "
                 + " WHEN (left([TXN], 1) = '0' AND TXNSRC = '18') THEN ltrim(rtrim(ISNULL([REFNUM], '')))  " // RRN For MEEZA POS                                                                                       
                 + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
                 + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
                 + " else ltrim(rtrim(ISNULL([REFNUM], ''))) "
                 + " end "

                 + ",[TRACE] "
                  + ",ISNULL(AUTHNUM, '')" // Auth Number 
                 + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here 
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
                     // + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
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
           + "  '30',"
           + "  '22',"
           + "  '26',"
           + "  '40',"
           + "  '41'))"
           + " OR (LEFT([TXN], 3) IN "
           + " ('210', '214'))"
           + " OR (left([TXN], 1) = '0')" // '0-POS Purchase'
           + " OR (left([TXN], 6) = '810022' AND AMOUNT <> '0')  " // Fawry .. leaving out the '810021'

           //**************************
           //+ " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '5' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry going to Flexcube
           + " )"
           + " ORDER by TERMID, [LOCAL_DATE],[LOCAL_TIME] "
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
            // Insert it into TWIN
            //
            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                  + ", 'BDC240' "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "

                 + ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                + ", 23 " // TRANSACTION TYPE FOR CREDIT
                 + ",'CREDIT TO Credit Card' " // [TransDescr]

                 + ", ISNULL([ACQ_CURRENCY_CODE], '') "
                 + ",CAST([AMOUNT] As decimal(18,2)) "
                 + ",CAST(Right([TRACE],6) as int) " // TRACE
                  + " , ltrim(rtrim(ISNULL([REFNUM], ''))) "

                 + ",[TRACE] "
                  + ",ISNULL(AUTHNUM, '')" // Auth Number 
                 + ", '' " //  
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", ltrim(rtrim(ISNULL([FULL_CARD], '')))   "  // We get the card of the Credit Card
                                                                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                      + ", CAST([SETTLEMENT_DATE] as date) "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE FULL_CARD <> '' and TXN = '410002-TRANSFER' AND RESPCODE = '0' " // SUCCESFUL CREDIT CARD
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
            stpErrorText += DateTime.Now + "_" + "Stage_CREDIT CARD RECORDS CREATED IN TWIN:.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE INTERNAL FOR POS
            //
            //SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
            //          + " SET [TransTypeAtOrigin] = 'INTERNAL' "
            //          + " WHERE MatchingCateg = '" + PRX + "231' AND Processed = 0 "
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
            //        stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
            //        stpReturnCode = -1;

            //        stpReferenceCode = stpErrorText;
            //        CatchDetails(ex);

            //        return;
            //    }

            //stpErrorText += DateTime.Now + "_" + "UPDATING of TransType finishes .." + Counter.ToString() + "\r\n";
            //stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**********************************************************
            // UPDATE [TransType] when Reversals
            //**********************************************************

            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                        + " SET [TransType] = 21 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0 AND [TransType] = 11 "
                        + "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                        + " SET [TransType] = 13 "
                        + " WHERE LEFT([FullTraceNo], 1) = '4'  AND Processed = 0 AND [TransType] = 23 "
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



            //
            // CREATE THE IST TWIN
            //
            //

            //
            // INSERT NEW RECORDS IN IST TWIN


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
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "225' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) <> '81') THEN '" + PRX + "235' " // All outgoing Master
                                                                                                                       //  + " WHEN (TXNSRC = '1' AND TXNDEST = '5') AND left([TransDescr], 2) <> '81' THEN '"+PRX+"235' " // All outgoing Master
                                                                                                                       // + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81' AND TransAmt<> '0') THEN '"+PRX+"251' "
                  + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "215' " // All outgoing 123 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) <> '81' THEN '" + PRX + "240' "  // Credit CARD
                    + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) = '81' THEN '" + PRX + "251' " // All outgoing CREDIT Card FOR FAWRY (Was Going Through Master Card ???)
                     + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81') THEN '" + PRX + "251' "
                   + " WHEN TXNSRC = '1' AND left([TransDescr], 6) = '810022' AND (TXNDEST = 1 OR TXNDEST = 12) THEN '" + PRX + "251'  " // FAWRY THAT GOES TO FLEXCUBE OR 12 prepaid
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "275' " // ALL OUTGOING MEEZA
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
                   + "  (TXNSRC = '1' AND TXNDEST = '4')  " // All outgoing Visa
                   + " OR (TXNSRC = '1' AND TXNDEST = '5')  " // All outgoing Master
                   + " OR (TXNSRC = '1' AND TXNDEST = '8')  " // All outgoing 123 
                   + " OR (TXNSRC = '1' AND TXNDEST = '13') " // All outgoing CREDIT Card Going Through Master Card
                                                              //  TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 2) = '81' AND AMOUNT <> '0'                  
                   + " OR (left([TransDescr], 6) = '810022' )  "// ALL FAWRY ... leaving out the '810021'
                   + " OR (TXNSRC = '1' AND TXNDEST = '18') " // ALL OUTGOING TO MEZZA

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

            //
            // INSERT NEW RECORDS IN IST TWIN
            // FOR MASTER CARD "+PRX+"235
            // ALL Outgoing Fawry 
            // All Outgoing Prepaid Fawry


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
                    + " ,'" + PRX + "235' " // Matching category for FAWRY coming from Master 

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

      + " ,[Comment] " // Here the comment holds the RRN 
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

                   + "  (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 6) = '810022')  " // All outgoing Master FAWRY

                   + " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 6) = '810022') " // All outgoing Prepaid(Fawry) coming through Through Master Card

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
                    stpErrorText = stpErrorText + "Cancel At_IST_TWIN_Second";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_04_IST_TWIN_Created_Second..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**************************************************************************************
            // 15/03/2021
            // Here Update Master Card which is 235 category with the Settlement amount and currency
            // GET THE VALUES FROM THE BULK FILE 
            // ************************************************************************************

            // CLEAR TABLE 
            //
            //  Set Comment to blank 
            // 
            int U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [Comment] = '' "
               + " WHERE  LoadedAtRMCycle = @LoadedAtRMCycle AND Comment <> '' "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            // CLEAR TABLE 
            //
            //  After the TWIN table with NOT WANTED FAWRY
            // 
            U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [MatchingCateg] = 'FawryType' "
                + " ,[Processed] = 1 "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'Fawry gone to IST Twin' "
               + " WHERE   Processed = 0 AND left([TransDescr], 6) = '810022' AND MatchingCateg <> '" + PRX + "250'  "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

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
              //*************************
              + " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
              + " SET "
                + " ResponseCode = '0' "
                + " , TransTypeAtOrigin = '112' "
             //+ " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
             //  + " ,[Comment] = 'Presenter' "
             + " WHERE   Processed = 0 AND LEFT(FullTraceNo,1) = '2' AND ResponseCode = '112' "
             //******************
             + " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
              + " SET "
                + " ResponseCode = '0' "
                + " , TransTypeAtOrigin = '112' "
             //+ " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
             //  + " ,[Comment] = 'Presenter' "
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

                        U_Count = cmd.ExecuteNonQuery();
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
            // CLEAR OLD ERRORS = Response code <> 0
            //
            // From both IST primary and TWIN 
            // 
            //U_Count = 0;

            //SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
            //    + " SET "
            //    + " [MatchingCateg] = 'FawryType' "
            //    + " ,[Processed] = 1 "
            //    + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
            //      + " ,[Comment] = 'Fawry gone to IST Twin' "
            //   + " WHERE   Processed = 0 AND left([TransDescr], 6) = '810022' AND MatchingCateg <> '" + PRX + "250'  "
            //    + " "
            //    ;

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InReconcCycleNo);

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

        // IST LOAD VERSION 2
        //
        // GTL = GET TRANSFORM LOAD
        public void InsertRecords_GTL_IST_2(string InOriginFileName, string InFullPath
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

            // Reassign File name
            InOriginFileName = InOriginFileName + "_2";

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

            stpErrorText += DateTime.Now + "_" + "Stage_02_Bulk_Insert_Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Correct Origin and Destination for MEEZA
            // 
            // 
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [TXNSRC] = '18' " // SOURCE
                                               // + " WHERE ([TXNSRC] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808' OR Left(MASK_PAN,6) = '507810')"
                     + " WHERE ([TXNSRC] = '8' AND (Left(MASK_PAN,3) = '507' )"
                     + " OR ([TXNSRC] = '8' AND LEFT(TXN,1) = '0') )"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [TXNDEST] = '18' " // DESTINATION
                                                //  + " WHERE [TXNDEST] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808' OR Left(MASK_PAN,6) = '507810')"
                     + " WHERE [TXNDEST] = '8' AND Left(MASK_PAN,3) = '507'"
                        ;

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

            RecordFound = false;

            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_Switch_IST_Txns_ALL_2 " //  We insert in the second IST
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
            //

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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                   + " ,case "
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) <> '81')  THEN '" + PRX + "201' " // Only DEBIT CARD for Flex 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "207' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5')  THEN '" + PRX + "206' " // All outgoing Master
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "205' " // All outgoing 123 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "208' " // All outgoing MEEZA

                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) <> '81' AND AMOUNT <> '0') "
                                                                                       + "THEN '" + PRX + "204' " // Credit Card
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '12') AND left([TXN], 2) <> '81'  THEN '" + PRX + "203'" // Prepaid 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0') "
                                                                                    + " THEN '" + PRX + "202' " // FAWRY -- All outgoing CREDIT Card Going Through Master Card

                  + " WHEN (TXNSRC = '1' "
                  + " AND TXNDEST <> '1' "
                  + " AND TXNDEST <> '4'"
                  + " AND TXNDEST <> '5' "
                  + " AND TXNDEST <> '8'"
                  + " AND TXNDEST <> '13'"
                  + " AND TXNDEST <> '12'"
                  + " AND TXNDEST <> '18'"
                       + ")  THEN '" + PRX + "202' " // All others  

                  + " WHEN (TXNSRC = '8' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                         //  + "  AND (LEFT([MASK_PAN],6) = '526402' )"
                                         + " THEN '" + PRX + "210' " // All incoming 123 NET - DEBIT
                   + " WHEN (TXNSRC = '8' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                         // + " AND LEFT([MASK_PAN],6) <> '526402') "
                                         + " THEN '" + PRX + "211' " // All incoming 123 NET - NON DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                         + " THEN '" + PRX + "270' " // All incoming MEEZA DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                         + " THEN '" + PRX + "271' " // All incoming MEEZA PREPAID
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '1' AND TXN = '0-POS Purchase') THEN '" + PRX + "272' " // All incoming 123 NET - MEEZA - POS DEBIT
                   + " WHEN (TXNSRC = '18' AND TXNDEST= '12' AND TXN = '0-POS Purchase') THEN '" + PRX + "273' " // All incoming 123 NET - MEEZA - POS NON DEBIT
                   + " WHEN (TXNSRC = '4' AND TXNDEST = '1') THEN '" + PRX + "220' " // All Incoming Visa
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "230' " // All Incoming Master to Flex
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
                   + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "231' " // All Incoming POS
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231

                   + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN '" + PRX + "250' " // FAWRY THAT GOES TO FLEXCUBE

                  + " else 'Not_Def' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "
                 + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                    //+ ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                 + ",case "
                 + " when left([TXN], 2) = '10' THEN 11 "
                 + " when left([TXN], 2) = '11' THEN 11 "
                 + " when left([TXN], 2) = '12' THEN 11 "
                 + " when left([TXN], 2) = '13' THEN 11 "
                 + " when left([TXN], 2) = '30' THEN 11 "
                 + " when left([TXN], 2) = '81' THEN 11 "
                 + " when left([TXN], 2) = '21' THEN 23 "
                 + " when left([TXN], 2) = '40' THEN 33 "
                 + " when left([TXN], 2) = '41' THEN 33 "
                 + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                 + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                 + " else 11 "
                 + "end "
                 + ",[TXN] " // [TransDescr]

                 + ", Case "   // Currency Code
                 + " WHEN (TXNSRC = '5' AND (left([TXN], 1) <> '0') AND (left([TXN], 2) <> '30') ) THEN ISNULL([SETTLEMENT_CODE], '') "
                 + " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                 + " end "
                   + ", Case "   // Transaction amount
                 + " WHEN (TXNSRC = '5' AND (left([TXN], 1) <> '0') AND (left([TXN], 2) <> '30') ) THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                 + " ELSE CAST([AMOUNT] As decimal(18,2)) "
                 + " end "


                 //+ ",[TraceNo] "
                 //+ ",[RRNumber] "
                 //+ ",[FullTraceNo] "
                 //+ ",[AUTHNUM]"

                 + ",CAST(Right([TRACE],6) as int) " // TRace 
                  + ",case " //  RRNumber
                 + " WHEN (left([TXN], 1) = '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   " // RRN For MEEZA POS                                                                                       
                 + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS // Leave it like this 
                 + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
                                                                                                                    //    + " WHEN (left([TXN], 1) <> '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)  " // RRN For MEEZA MAKE IT 8 digit 
                 + " else RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   "
                 + " end "

                 // + ",case " //  RRNumber
                 //+ " WHEN (left([TXN], 1) = '0' AND TXNSRC = '18') THEN ltrim(rtrim(ISNULL([REFNUM], '')))  " // RRN For MEEZA POS                                                                                       
                 //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([REFNUM], ''))) " // All Incoming POS
                 //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([REFNUM], ''))) " // All Incoming POS - Prepaid
                 ////+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
                 ////+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                 //+ " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY
                 //+ " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Prepaid
                 //+ " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
                 // + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
                 //+ " else ltrim(rtrim(ISNULL([REFNUM], ''))) "
                 //+ " end "

                 + ",[TRACE] " // FullTraceNo
                  + ",ISNULL(AUTHNUM, '')" // Auth Number 

                 + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here for Fawry
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
                     // + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
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
           + "  '30',"
           + "  '22',"
           + "  '26',"
           + "  '40',"
           + "  '41'))"
           + " OR (LEFT([TXN], 3) IN "
           + " ('210', '214'))"
           + " OR (left([TXN], 1) = '0')" // '0-POS Purchase'
           + " OR (left([TXN], 6) = '810022' AND AMOUNT <> '0')  " // Fawry .. leaving out the '810021'

           //**************************
           //+ " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '5' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry going to Flexcube
           + " )"
           + " ORDER by TERMID, [LOCAL_DATE],[LOCAL_TIME] "
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
            // INSERT - MASTER FOREIGN AMOUNT
            // 
            //
            //bool MasterTwoCurrencies = true; 

            //if (MasterTwoCurrencies == true & Environment.MachineName == "RRDM-PANICOS")
            //{
            //    RecordFound = false;

            //    SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies]"
            //        + "( "
            //         + " [OriginFileName] "
            //         //    + " ,[OriginalRecordId] "

            //         + " ,[Origin] "
            //          + " ,[MatchingCateg] "
            //         + " ,[TerminalType] "

            //         + " ,[TransDate] "
            //         + " ,[TransType] "
            //         + ",[TransDescr] "
            //           + ",[TransCurr] " // 818
            //         + ",[TransAmt] " // Amount Equivalent 

            //         + ",[TransCurr_TWO] "
            //         + ",[TransAmt_TWO] "

            //         + ",[TraceNo] "
            //         + ",[RRNumber] "
            //         + ",[FullTraceNo] "
            //         + ",[AUTHNUM]"
            //         + ",[Comment]" // Use a temporary field to host RRN in cases of Fawry 

            //         + ",[TerminalId] "
            //         + ",[CardNumber] "
            //         + ",[AccNo] "
            //         + ",[ResponseCode] "

            //         + ",[LoadedAtRMCycle] "
            //         + ",[Operator] "
            //           + ",[TXNSRC] "
            //             + ",[TXNDEST] "

            //             + ",[EXTERNAL_DATE] "
            //            + " , [Net_TransDate] "
            //            + ",[Card_Encrypted] "
            //              + ",[ACCEPTOR_ID] "
            //                + ",[ACCEPTORNAME] "
            //             + ",[CAP_DATE] "
            //             + ",[SET_DATE] "
            //         + ") "
            //         + " SELECT "
            //         //  + " TOP 150000 "
            //         + "@OriginFile"
            //         //    + " , SeqNo "
            //         + " ,@Origin "
            //           + " ,case "

            //           + " WHEN (TXNSRC = '1' AND (TXNDEST = '5' OR TXNDEST = '13') )  THEN '" + PRX + "235' " // All outgoing Master

            //           + " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "230' " // All Incoming Master to Flex
            //           + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
            //           + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "231' " // All Incoming POS
            //           + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231

            //          + " else 'Not_Def' "
            //         + " end "

            //           + ",case "
            //         + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
            //         + " else '10' "
            //         + "end "
            //         + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
            //            //+ ", CAST([LOCAL_DATE] as datetime) "
            //            + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
            //            + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
            //            + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
            //         + ",case "
            //         + " when left([TXN], 2) = '10' THEN 11 "
            //         + " when left([TXN], 2) = '11' THEN 11 "
            //         + " when left([TXN], 2) = '12' THEN 11 "
            //         + " when left([TXN], 2) = '13' THEN 11 "
            //         + " when left([TXN], 2) = '30' THEN 11 "
            //         + " when left([TXN], 2) = '81' THEN 11 "
            //         + " when left([TXN], 2) = '21' THEN 23 "
            //         + " when left([TXN], 2) = '40' THEN 33 "
            //         + " when left([TXN], 2) = '41' THEN 33 "
            //         + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
            //         + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
            //         + " else 11 "
            //         + "end "
            //         + ",[TXN] " // [TransDescr]

            //        // First Pair Ccy and Amount - SETTLEMENT 
            //         + " , '818' "
            //         + " ,CAST([AMOUNT_EQUIV] As decimal(18,2)) " // In Place of the spare amount field 
            //        // + " , CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
            //          // Second Pair Ccy and Amount
            //          + " , ISNULL([ACQ_CURRENCY_CODE], '') "
            //         + " , CAST([AMOUNT] As decimal(18,2)) "

            //         + ",CAST(Right([TRACE],6) as int) " // TRace 

            //         + " , ltrim(rtrim(ISNULL([REFNUM], '')))"

            //         // + ",case "

            //         //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
            //         //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid

            //         ////+ " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
            //         // + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
            //         //+ " else ltrim(rtrim(ISNULL([REFNUM], ''))) "
            //         //+ " end "

            //         + ",[TRACE] "
            //          + ",ISNULL(AUTHNUM, '')" // Auth Number 
            //         + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here 
            //         + ", ISNULL(TERMID, '') " // Terminal Id 
            //         + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
            //         // + ", ISNULL([ACTNUM], '') "
            //         + " , RIGHT(ACTNUM, 14) "
            //         // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
            //         + ",ltrim(rtrim([RESPCODE]))  "

            //         + ", @LoadedAtRMCycle"
            //         + ", @Operator"
            //          + ", ltrim(rtrim([TXNSRC]))"
            //           + ", ltrim(rtrim([TXNDEST]))"
            //              + ", CAST([TRANDATE] as datetime) "
            //             + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
            //             + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
            //             + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

            //             //For  Net_TransDate
            //             + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
            //             // + ", CAST([LOCAL_DATE] as datetime) "
            //             + ", ISNULL([PAN], '') " // For Encrypted Card

            //             + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
            //             + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

            //              + ", CAST([CAP_DATE] as date) " // CAP_DATE
            //                                              // + ", CAST([CAP_DATE] as date) " // CAP_DATE
            //              + ", CAST([SETTLEMENT_DATE] as date) "
            //            + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " WHERE  "
            //         + " TXNSRC = '1' AND (TXNDEST = '5' OR TXNDEST = '13') " // ALL MASTER OUTGOING + FAWRY
            //         + " OR "
            //         + " TXNSRC = '5' "  // ALL MASTER INCOMING
            //         + " AND RESPCODE = '0' "
            //         ;


            //    using (SqlConnection conn = new SqlConnection(recconConnString))
            //        try
            //        {
            //            conn.StatisticsEnabled = true;
            //            conn.Open();

            //            using (SqlCommand cmd =
            //                new SqlCommand(SQLCmd, conn))
            //            {
            //                cmd.Parameters.AddWithValue("@OriginFile", WFileSeqNo); // seq for file
            //                cmd.Parameters.AddWithValue("@Origin", Origin);

            //                //    cmd.Parameters.AddWithValue("@TerminalType", WTerminalType);

            //                cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
            //                cmd.Parameters.AddWithValue("@Operator", WOperator);
            //                //cmd.Parameters.AddWithValue("@TransCurr", "EGP");

            //                cmd.CommandTimeout = 350;  // seconds

            //                stpLineCount = cmd.ExecuteNonQuery();

            //                var stats = conn.RetrieveStatistics();
            //                commandExecutionTimeInMs = (long)stats["ExecutionTime"];

            //            }
            //            // Close conn
            //            conn.StatisticsEnabled = false;
            //            conn.Close();
            //            if (Environment.UserInteractive & TEST == true)
            //            {
            //                System.Windows.Forms.MessageBox.Show("Records Inserted for IST" + Environment.NewLine
            //                       + "..:.." + stpLineCount.ToString());
            //            }


            //        }
            //        catch (Exception ex)
            //        {
            //            conn.StatisticsEnabled = false;
            //            conn.Close();
            //            stpErrorText = stpErrorText + "Cancel At _IST_Insert_TWO CURRENCIES";

            //            stpReturnCode = -1;

            //            stpReferenceCode = stpErrorText;

            //            CatchDetails(ex);

            //            return;
            //        }
            //    //**********************************************************
            //    stpErrorText += DateTime.Now + "_" + "Stage_03_IST_Records_loaded..Records:.Master Two Currencies." + stpLineCount.ToString() + "\r\n";
            //    stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //    Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //}

            //
            // Make a credit card transaction out of tranfers with full card number 
            // Make ALSO Master - where is FAWRY is involved
            // Insert it into TWIN
            //
            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                    //  + ", 'BDC240' "
                    + ", Case "   // [TransDescr]
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 'BDC235' "
                 + " WHEN TXN = '410002-TRANSFER' THEN 'BDC240' "
                 + " ELSE '' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "

                 + ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                  + ", Case "   // TRANSACTION TYPE
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 11 "
                 + " WHEN TXN = '410002-TRANSFER' THEN 23 "
                 + " ELSE 0 "
                 + " end "
                 + ", Case "   // [TransDescr]
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 'Master for Fawry' "
                 + " WHEN TXN = '410002-TRANSFER' THEN 'CREDIT TO Credit Card' "
                 + " ELSE '' "
                 + " end "
                 + ", Case "   // Currency Code
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN ISNULL([SETTLEMENT_CODE], '') "
                 + " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                 + " end "
                   + ", Case "   // Transaction amount
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                 + " ELSE CAST([AMOUNT] As decimal(18,2)) "
                 + " end "
                 //+ ", ISNULL([ACQ_CURRENCY_CODE], '') "
                 //+ ",CAST([AMOUNT] As decimal(18,2)) "
                 + ",CAST(Right([TRACE],6) as int) " // TRACE
                  + " , ltrim(rtrim(ISNULL([REFNUM], ''))) "

                 + ",[TRACE] "
                 + ", Case "   // [TransDescr]
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN REFNUM "
                 + " WHEN TXN = '410002-TRANSFER' THEN ISNULL(AUTHNUM, '') "
                 + " ELSE '' "
                 + " end "

                 + ", '' " //  
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", Case "   // [TransDescr]
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN [MASK_PAN] "
                 + " WHEN TXN = '410002-TRANSFER' THEN ltrim(rtrim(ISNULL([FULL_CARD], ''))) " // We get the card of the Credit Card
                 + " ELSE '' "
                 + " end "

                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                      + ", CAST([SETTLEMENT_DATE] as date) "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE (FULL_CARD <> '' and TXN = '410002-TRANSFER' AND RESPCODE = '0') " // SUCCESFUL CREDIT CARD
                   + " OR (TXNSRC = '1' AND TXNDEST = '5'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND AMOUNT <> '0') " // CREATE EXTRA RECORD FOR MASTER CARD AND FAWRY
                   + " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 6) = '810022' AND RESPCODE = '0' AND AMOUNT <> '0' ) "; //  same

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
            stpErrorText += DateTime.Now + "_" + "Stage_CREDIT CARD RECORDS CREATED IN TWIN:.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //**********************************************************
            // UPDATE [TransType] when Reversals
            // 21 for withdrawls 13 for Deposits 
            //**********************************************************

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



            //
            // CREATE THE IST TWIN
            //
            //

            //
            // INSERT NEW RECORDS IN IST TWIN


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
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "225' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) <> '81') THEN '" + PRX + "235' " // All outgoing Master
                                                                                                                       //  + " WHEN (TXNSRC = '1' AND TXNDEST = '5') AND left([TransDescr], 2) <> '81' THEN '"+PRX+"235' " // All outgoing Master
                                                                                                                       // + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81' AND TransAmt<> '0') THEN '"+PRX+"251' "
                  + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "215' " // All outgoing 123 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) <> '81' THEN '" + PRX + "240' "  // Credit CARD
                    + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) = '81' THEN '" + PRX + "251' " // All outgoing CREDIT Card FOR FAWRY (Was Going Through Master Card ???)
                     + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81') THEN '" + PRX + "251' " // Fawry paid through credit card
                   + " WHEN TXNSRC = '1' AND left([TransDescr], 6) = '810022' AND (TXNDEST = 1 OR TXNDEST = 12) THEN '" + PRX + "251'  " // FAWRY THAT GOES TO FLEXCUBE OR 12 prepaid
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "275' " // ALL OUTGOING MEEZA
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
                   + "  (TXNSRC = '1' AND TXNDEST = '4')  " // All outgoing Visa
                   + " OR (TXNSRC = '1' AND TXNDEST = '5')  " // All outgoing Master
                   + " OR (TXNSRC = '1' AND TXNDEST = '8')  " // All outgoing 123 
                   + " OR (TXNSRC = '1' AND TXNDEST = '13') " // All outgoing CREDIT Card Going Through Master Card
                                                              //  TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 2) = '81' AND AMOUNT <> '0'                  
                   + " OR (left([TransDescr], 6) = '810022' )  "// ALL FAWRY ... leaving out the '810021'
                   + " OR (TXNSRC = '1' AND TXNDEST = '18') " // ALL OUTGOING TO MEZZA

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

            //
            // INSERT NEW RECORDS IN IST TWIN
            // FOR MASTER CARD "+PRX+"235
            // ALL Outgoing Fawry 
            // All Outgoing Prepaid Fawry


            //      SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
            //          + "( "
            //           + " [OriginFileName] "
            //           + " ,[Origin] "
            //            + " ,[MatchingCateg] "
            //           + " ,[TerminalType] "
            //           + " ,[TransDate] "

            //           + " ,[TransType] "
            //           + ",[TransDescr] "
            //             + ",[TransCurr] "
            //           + ",[TransAmt] "

            //           + ",[TraceNo] "

            //           + ",[RRNumber] "
            //           + ",[FullTraceNo] "
            //           + ",[AUTHNUM]" // Auth Number 
            //           + ",[TerminalId] "
            //           + ",[CardNumber] "
            //           + ",[AccNo] "

            //           + ",[ResponseCode] "
            //           + ",[LoadedAtRMCycle] "
            //           + ",[Operator] "
            //             + ",[TXNSRC] "
            //               + ",[TXNDEST] "
            //                  + ",[EXTERNAL_DATE] "
            //                  + " , [Net_TransDate] "
            //                        + ",[Card_Encrypted] "
            //                           + ",[CAP_DATE] "
            //                           + ",[SET_DATE] "
            //           + ") "
            //           + " SELECT "
            //           + " [OriginFileName] "
            //           + " ,[Origin] "
            //              // + " ,[MatchingCateg] "
            //              + " ,'" + PRX + "235' " // Matching category for FAWRY coming from Master 

            //+ " ,[TerminalType] "
            // //
            // // + " ,[TransDate] "
            // + " ,case"
            //             + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN [EXTERNAL_DATE] " // All outgoing Visa       
            //             + " else [TransDate] "
            //     //   WHERE TXNSRC = '1' AND TXNDEST = '13' and left([TransDescr], 2) = '81' AND AMOUNT <> '0'
            //     + " end "
            //+ " ,[TransType] "
            //+ " ,[TransDescr] "
            //+ " ,[TransCurr] "
            //+ " ,[TransAmt] "
            //+ " ,[TraceNo] "

            //+ " ,[Comment] " // Here the comment holds the RRN 
            //+ " ,[FullTraceNo] "
            //+ ",[AUTHNUM]" // Auth Number 
            //  + " ,[TerminalId] "
            //   + " ,[CardNumber] "
            //+ " ,[AccNo] "
            //+ " ,[ResponseCode] "
            //+ " ,[LoadedAtRMCycle] "
            //+ " ,[Operator] "
            //+ " ,[TXNSRC] "
            //+ " ,[TXNDEST] "
            //+ " ,case"
            //             + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN [TransDate] " // All outgoing Visa       
            //             + " else [EXTERNAL_DATE] "
            //     //   WHERE TXNSRC = '1' AND TXNDEST = '13' and left([TransDescr], 2) = '81' AND AMOUNT <> '0'
            //     + " end "

            //     + " , [Net_TransDate] "
            //     + ", [Card_Encrypted] "
            //    + ", [CAP_DATE] "
            //    + ", [SET_DATE] "

            //           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "  // FROM IST

            //           + " WHERE  SeqNo > @SeqNo  AND ( "

            //             //+ "  (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 6) = '810022')  " // All outgoing Master FAWRY

            //             + " (TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 6) = '810022') " // All outgoing Prepaid(Fawry) coming through Through Master Card

            //               + " ) ";

            //      using (SqlConnection conn = new SqlConnection(recconConnString))
            //          try
            //          {
            //              conn.StatisticsEnabled = true;

            //              conn.Open();
            //              using (SqlCommand cmd =
            //                  new SqlCommand(SQLCmd, conn))
            //              {
            //                  cmd.Parameters.AddWithValue("@SeqNo", LastSeqNo);
            //                  cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
            //                  cmd.CommandTimeout = 350;  // seconds
            //                  Counter = cmd.ExecuteNonQuery();
            //                  var stats = conn.RetrieveStatistics();
            //                  commandExecutionTimeInMs = (long)stats["ExecutionTime"];


            //              }
            //              // Close conn
            //              conn.StatisticsEnabled = false;
            //              conn.Close();
            //              if (Environment.UserInteractive & TEST == true)
            //              {
            //                  System.Windows.Forms.MessageBox.Show("Records Inserted For IST_TWIN" + Environment.NewLine
            //                           + "..:.." + Counter.ToString());
            //              }
            //          }
            //          catch (Exception ex)
            //          {
            //              conn.StatisticsEnabled = false;
            //              conn.Close();
            //              stpErrorText = stpErrorText + "Cancel At_IST_TWIN_Second";

            //              stpReturnCode = -1;

            //              stpReferenceCode = stpErrorText;

            //              CatchDetails(ex);

            //              return;
            //          }
            //      //**********************************************************
            //      stpErrorText += DateTime.Now + "_" + "Stage_04_IST_TWIN_Created_Second..Records:.." + Counter.ToString() + "\r\n";
            //      stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            //      Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //**************************************************************************************
            // 15/03/2021
            // Here Update Master Card which is 235 category with the Settlement amount and currency
            // GET THE VALUES FROM THE BULK FILE 
            // ************************************************************************************
            // LEAVE IT HERE
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
          + " SET"
            + " [TransCurr] = ISNULL(t2.[SETTLEMENT_CODE], '')  "
           + " ,[TransAmt] = CAST(t2.[SETTLEMENT_AMOUNT] As decimal(18,2)) "
            + ",AmtFileBToFileC = 99.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t1 "
          + " INNER JOIN "
          + " [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + " t2"

          + " ON "
          + "  t1.RRNumber = t2.REFNUM "
          + " AND t1.TerminalId = t2.TERMID "

          + " WHERE  "
          + " t1.Processed = 0  AND t1.MatchingCateg in ('" + PRX + "235') And t1.LoadedAtRMCycle = @LoadedAtRMCycle AND AmtFileBToFileC <> 99.99" // 
          + " AND (t2.TXNSRC = '1' AND t2.TXNDEST = '5' AND CAST(t2.[SETTLEMENT_AMOUNT] As decimal(18,2)) > 0  ) "
          ;
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During UPDATE TWIN For Master Settlement ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATE TWIN For Master Settlement..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // CLEAR TABLE 
            //
            //  Set Comment to blank 
            // 
            int U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [Comment] = '' "
               + " WHERE  LoadedAtRMCycle = @LoadedAtRMCycle AND Comment <> '' "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }


            // CLEAR TABLE 
            //
            //  After the TWIN table with NOT WANTED FAWRY
            // 
            U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [MatchingCateg] = 'FawryType' "
                + " ,[Processed] = 1 "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'Fawry gone to IST Twin' "
               + " WHERE   Processed = 0 AND left([TransDescr], 6) = '810022' AND MatchingCateg <> '" + PRX + "250'  "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            /*
             * 
             * THe Below doesnt apply to the BDC as the two records for presenter 
             * ARE both normal starting with 2 and having the one 0 response and the other 112 
             * 
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

                       U_Count = cmd.ExecuteNonQuery();
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
          */



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

        // IST LOAD VERSION 3
        // Accomodates MEEZA GLOBAL 
        // GTL = GET TRANSFORM LOAD
        public void InsertRecords_GTL_IST_3(string InOriginFileName, string InFullPath
                          , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            ErrorFound = false;
            ErrorOutput = "";

            //   stpErrorText = "";
            stpReferenceCode = "";

            int Counter = 0;

            // THE PHYSICAL DATA BASES ARE
            //

            string SQLCmd;

            string WTerminalType = "10"; // Leave it here 
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            Mf.ReadReconcSourceFilesByFileId(InOriginFileName);

            string Origin = Mf.SystemOfOrigin;
            string WOperator = Mf.Operator;

            string WFullPath_01 = InFullPath;

            // Reassign File name
            InOriginFileName = InOriginFileName + "_2";

            


            // Check if file is a Windows and Not Unix. 


            //   stpErrorText = "";
            stpReferenceCode = "";


            // CHECK THE FILE
            string TableId_Bulk = "[RRDM_Reconciliation_ITMX].[dbo].[BULK_Switch_IST_Txns_2]";

            // MAKE UNIX oR DELIMETER VALIDATION 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Del = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            bool Correct = Del.CheckIfWindowsOrUnix_ATMS(InFullPath);

            // Correct = true; // WE MAKE TRUE TILL MARIOS HELP 

            if (Correct == false)
            {
                MessageBox.Show("The file: " + InFullPath + Environment.NewLine
                    + " IS not a Windows File. It cannnot be accepted "
                    );
                return;
            }

            string InDelimiter = ",";

            Correct = Del.FindIfDelimiterErrorsInInputTable_ATMS(TableId_Bulk, InFullPath, InDelimiter, recconConnString);


            if (Correct == false)
            {

                //string OldString = "faisal,giza";
                //string NewString = "faisal giza"; // No comma

                //  Del.ReplaceValueInGivenTextFile(InFullPath, OldString, NewString);
            }

            // THE PHYSICAL DATA BASES ARE
            //



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

            stpErrorText += DateTime.Now + "_" + "Stage_02_Bulk_Insert_Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // Correct Origin and Destination for MEEZA POS


            //SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET [TXNSRC] = '18' " // SOURCE
            //                                   // + " WHERE ([TXNSRC] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808' OR Left(MASK_PAN,6) = '507810')"
            //         + " WHERE ([TXNSRC] = '8' AND (Left(MASK_PAN,3) = '507' )"
            //         + " OR ([TXNSRC] = '8' AND LEFT(TXN,1) = '0') )"
            //         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //         + " SET [TXNDEST] = '18' " // DESTINATION
            //                                    //  + " WHERE [TXNDEST] = '8' AND (Left(MASK_PAN,6) = '507803' OR Left(MASK_PAN,6) = '507808' OR Left(MASK_PAN,6) = '507810')"
            //         + " WHERE [TXNDEST] = '8' AND Left(MASK_PAN,3) = '507'"
            //            ;

            // using (SqlConnection conn = new SqlConnection(recconConnString))
            //         try
            //         {
            //             conn.Open();
            //             using (SqlCommand cmd =
            //                 new SqlCommand(SQLCmd, conn))
            //             {
            //                 Counter = cmd.ExecuteNonQuery();
            //             }
            //             // Close conn
            //             conn.Close();
            //         }
            //         catch (Exception ex)
            //         {
            //             conn.Close();

            //             stpErrorText = stpErrorText + "Cancel During Correct AMOUNT";
            //             stpReturnCode = -1;

            //             stpReferenceCode = stpErrorText;
            //             CatchDetails(ex);
            //             return;
            //         }

            RecordFound = false;

            // KEEP IST 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_Switch_IST_Txns_ALL_2 " //  We insert in the second IST
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
            //

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
                 //+ ",[AmtFileBToFileC] "
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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                   + " ,case " // Matching Category
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) <> '81')  THEN '" + PRX + "201' " // Only DEBIT CARD for Flex 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "207' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5')  THEN '" + PRX + "206' " // All outgoing Master
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "205' " // All outgoing MEEZA
                                                                                     //  + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "208' " // All outgoing MEEZA

                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) <> '81' AND AMOUNT <> '0') "
                                                                                       + "THEN '" + PRX + "204' " // Credit Card
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '12') AND left([TXN], 2) <> '81'  THEN '" + PRX + "203'" // Prepaid 
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0') "
                                                                                    + " THEN '" + PRX + "202' " // FAWRY -- All outgoing CREDIT Card Going Through Master Card

                  + " WHEN (TXNSRC = '1' "
                  + " AND TXNDEST <> '1' "
                  + " AND TXNDEST <> '4'"
                  + " AND TXNDEST <> '5' "
                  + " AND TXNDEST <> '8'"
                  + " AND TXNDEST <> '13'"
                  + " AND TXNDEST <> '12'"
                  + " AND TXNDEST <> '8'"
                       + ")  THEN '" + PRX + "202' " // All others  

                  + " WHEN (TXNSRC = '8' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                         //  + "  AND (LEFT([MASK_PAN],6) = '526402' )"
                                         + " THEN '" + PRX + "277' " // All incoming MEEZA - DEBIT
                   + " WHEN (TXNSRC = '8' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                         // + " AND LEFT([MASK_PAN],6) <> '526402') "
                                         + " THEN '" + PRX + "278' " // All incoming MEEZA - NON DEBIT
                                                                     //+ " WHEN (TXNSRC = '18' AND TXNDEST= '1' AND TXN <> '0-POS Purchase') "
                                                                     //                      + " THEN '" + PRX + "270' " // All incoming MEEZA DEBIT
                                                                     //+ " WHEN (TXNSRC = '18' AND TXNDEST= '12' AND TXN <> '0-POS Purchase') "
                                                                     //                      + " THEN '" + PRX + "271' " // All incoming MEEZA PREPAID
                   + " WHEN (TXNSRC = '8' AND TXNDEST= '1' AND TXN = '0-POS Purchase') THEN '" + PRX + "272' " // All incoming - MEEZA - POS DEBIT
                   + " WHEN (TXNSRC = '8' AND TXNDEST= '12' AND TXN = '0-POS Purchase') THEN '" + PRX + "273' " // All incoming  - MEEZA - POS NON DEBIT
                   + " WHEN (TXNSRC = '4' AND TXNDEST = '1') THEN '" + PRX + "220' " // All Incoming Visa
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' AND Left(TXN,4) <> '2000' THEN '" + PRX + "230' " // All Incoming Master to Flex
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' AND Left(TXN,4) <> '2000' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
                   + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' OR  Left(TXN,4) = '2000') THEN '" + PRX + "231' " // All Incoming POS
                                                                                                                                                                   // + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '2000-Refund' AND AUTHNUM <> '' ) THEN '" + PRX + "231' " // All Incoming POS REFUND 
                   + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' OR  Left(TXN,4) = '2000' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231

                   + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN '" + PRX + "250' " // FAWRY THAT GOES TO FLEXCUBE

                  + " else 'Not_Def' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "
                 + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                    //+ ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                 + ",case "
                 + " when left([TXN], 2) = '10' THEN 11 "
                 + " when left([TXN], 2) = '11' THEN 11 "
                 + " when left([TXN], 2) = '12' THEN 11 "
                 + " when left([TXN], 2) = '13' THEN 11 "
                 + " when left([TXN], 2) = '30' THEN 11 "
                 + " when left([TXN], 2) = '81' THEN 11 "
                 + " when left([TXN], 2) = '21' THEN 23 "
                 + " when left([TXN], 2) = '40' THEN 33 "
                 + " when left([TXN], 2) = '41' THEN 33 "
                 + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                 + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                 + " else 11 "
                 + "end "
                 + ",[TXN] " // [TransDescr]

                 + ", Case "   // Currency Code // HERE exception for master card 
                 + " WHEN (TXNSRC = '5' AND (left([TXN], 1) <> '0') AND (left([TXN], 2) <> '30') ) THEN ISNULL([SETTLEMENT_CODE], '') "
                 + " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                 + " end "
                   + ", Case "   // Transaction amount -  
                 + " WHEN (TXNSRC = '5' AND (left([TXN], 1) <> '0') AND (left([TXN], 2) <> '30') ) THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // MASTER NOT POS
                 + " ELSE CAST([AMOUNT] As decimal(18,2)) "
                 + " end "

                 // + " ,CAST([AMOUNT_EQUIV] As decimal(18,2)) " // In Place of the spare amount field 

                 //+ ",[TraceNo] "
                 //+ ",[RRNumber] "
                 //+ ",[FullTraceNo] "
                 //+ ",[AUTHNUM]"

                 + ",CAST(Right([TRACE],6) as int) " // TRace 
                                                     // + ",RIGHT('000000000000' + REFNUM, 12) " // RRN insert leading zeros 
                  + ",case " //  RRNumber
                 + " WHEN (left([TXN], 1) = '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   " // RRN For MEEZA POS                                                                                       
                 + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS // Leave it like this 
                 + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
                                                                                                                    //    + " WHEN (left([TXN], 1) <> '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)  " // RRN For MEEZA MAKE IT 8 digit 
                 + " else RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   "
                 + " end "

                 + ",[TRACE] " // FullTraceNo

                 + ", Case " // Auth Number 
                  + " WHEN AUTHNUM = '000000' then '' "
                  + " Else ISNULL(AUTHNUM, '') "
                  + " end"

                 + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here for Fawry
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
                     // + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
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
           //+ "  '20'," // Refund
           + "  '30',"
           + "  '22',"
           + "  '26',"
           + "  '40',"
           + "  '41'))"
           + " OR (LEFT([TXN], 3) IN "
           + " ('210', '214'))"
           + " OR (left([TXN], 1) = '0')" // '0-POS Purchase'
           + " OR (left([TXN], 6) = '810022' AND AMOUNT <> '0')  " // Fawry .. leaving out the '810021'
                                                                   //+ " AND (TXN <> '200000-Refund' AND TXNSRC = '8') " //  Exclude the REFUND

           //**************************
           //+ " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '5' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry 
           //+ " OR (TXNSRC = '1' AND TXNDEST = '1' AND left([TXN], 2) = '81' AND AMOUNT <> '0' ) " // Fawry going to Flexcube
           //+ " ) AND (TXN <> '200000-Refund' ) " //Exclude the REFUND
           + " )  "
           + " ORDER by TERMID, [LOCAL_DATE],[LOCAL_TIME] "
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
            // INSERT - MASTER FOREIGN AMOUNT
            // MASTER CARD CASE
            //

            if (MasterTwoCurrencies == true)
            {
                RecordFound = false;

                SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies]"
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

                     + ",[TransCurr_TWO] "
                     + ",[TransAmt_TWO] "

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
                     //  + " TOP 150000 "
                     + "@OriginFile"
                     //    + " , SeqNo "
                     + " ,@Origin "
                       + " ,case "

                       + " WHEN (TXNSRC = '1' AND (TXNDEST = '5' OR TXNDEST = '13') )  THEN '" + PRX + "235' " // All outgoing Master

                       + " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "230' " // All Incoming Master to Flex
                       + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
                       + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "231' " // All Incoming POS
                       + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231

                      + " else 'Not_Def' "
                     + " end "

                       + ",case "
                     + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                     + " else '10' "
                     + "end "
                     + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                        //+ ", CAST([LOCAL_DATE] as datetime) "
                        + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                        + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                        + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                     + ",case "
                     + " when left([TXN], 2) = '10' THEN 11 "
                     + " when left([TXN], 2) = '11' THEN 11 "
                     + " when left([TXN], 2) = '12' THEN 11 "
                     + " when left([TXN], 2) = '13' THEN 11 "
                     + " when left([TXN], 2) = '30' THEN 11 "
                     + " when left([TXN], 2) = '81' THEN 11 "
                     + " when left([TXN], 2) = '21' THEN 23 "
                     + " when left([TXN], 2) = '40' THEN 33 "
                     + " when left([TXN], 2) = '41' THEN 33 "
                     + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                     + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                     + " else 11 "
                     + "end "
                     + ",[TXN] " // [TransDescr]

                          // First Pair Ccy and Amount - SETTLEMENT // This is true for master card 
                          //  + " , ISNULL([SETTLEMENT_CODE], '') "
                          + ",Case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN ISNULL([SETTLEMENT_CODE], '') "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN ISNULL([ACQ_CURRENCY_CODE], '') "
                     + " WHEN TXNSRC = '5' THEN  ISNULL([SETTLEMENT_CODE], '') "
                     + " else '818' "
                     + " end "
                        // + " , CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                        + ",Case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN CAST([AMOUNT] As decimal(18,2)) "
                     + " WHEN TXNSRC = '5' THEN  CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                     + " else 0 "
                     + " end "
                      // Second Pair Ccy and Amount
                      //     + ", Case "   // Currency Code
                      //+ " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN ISNULL([SETTLEMENT_CODE], '') "
                      //+ " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                      //+ " end "
                      //  + ", Case "   // Transaction amount
                      //+ " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                      //+ " ELSE CAST([AMOUNT] As decimal(18,2)) "
                      //+ " end "

                      + " , '818' "
                      + ",case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN CAST([AMOUNT] As decimal(18,2)) "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN CAST([AMOUNT_EQUIV] As decimal(18,2)) "
                     + " WHEN TXNSRC = '5' THEN  CAST([AMOUNT_EQUIV] As decimal(18,2)) "
                     + " else 0 "
                     + " end "
                     //+ " , CAST([AMOUNT_EQUIV] As decimal(18,2)) "

                     + ",CAST(Right([TRACE],6) as int) " // TRace 

                     + ",case " //  RRNumber
                 + " WHEN (left([TXN], 1) = '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   " // RRN For MEEZA POS                                                                                       
                 + " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS // Leave it like this 
                 + " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS
                                                                                                                                                              //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // All Incoming POS - Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Credit Card
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ltrim(rtrim(ISNULL([AUTHNUM], ''))) " // RRN For FAWRY Master Card
                                                                                                                    //    + " WHEN (left([TXN], 1) <> '0' AND TXNSRC = '8') THEN RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)  " // RRN For MEEZA MAKE IT 8 digit 
                 + " else RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12)   "
                 + " end "

                     + ",[TRACE] "
                      + ",ISNULL(AUTHNUM, '')" // Auth Number 
                     + ", ISNULL([REFNUM], '') " // Put temporarily the RRN here 
                     + ", ISNULL(TERMID, '') " // Terminal Id 
                     + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                     // + ", ISNULL([ACTNUM], '') "
                     + " , RIGHT(ACTNUM, 14) "
                     // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                     + ",ltrim(rtrim([RESPCODE]))  "

                     + ", @LoadedAtRMCycle"
                     + ", @Operator"
                      + ", ltrim(rtrim([TXNSRC]))"
                       + ", ltrim(rtrim([TXNDEST]))"
                          + ", CAST([TRANDATE] as datetime) "
                         + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                         + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                         + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                         //For  Net_TransDate
                         + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
                         // + ", CAST([LOCAL_DATE] as datetime) "
                         + ", ISNULL([PAN], '') " // For Encrypted Card

                         + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                         + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                          + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                          // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                          + ", CAST([SETTLEMENT_DATE] as date) "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " WHERE  "
                     + " ((TXNSRC = '1' AND (TXNDEST = '5' OR TXNDEST = '13')) " // ALL MASTER OUTGOING + FAWRY
                     + " OR "
                     + " TXNSRC = '5' )"  // ALL MASTER INCOMING
                     + " AND Left(TXN,2) <> '20'  " //Exclude the REFUND
                     + " AND RESPCODE = '0' "

                     //    + "  (TXNSRC = '1' AND TXNDEST = '5'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') "
                     //+ "  OR (TXNSRC = '1' AND TXNDEST = '13'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') " //  extra recirds for master Fawry 
                     //+ " OR (TXNSRC = '1' AND TXNDEST = '4') "  // VISA 
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
                        stpErrorText = stpErrorText + "Cancel At _IST_Insert_TWO CURRENCIES";

                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;

                        CatchDetails(ex);

                        return;
                    }
                //**********************************************************
                stpErrorText += DateTime.Now + "_" + "Stage_03_IST_Records_loaded..Records:.Master Two Currencies." + stpLineCount.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                RecordFound = false;
                //
                // Do the same for the BDC236 - Additional Records
                //

                SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies]"
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

                     + ",[TransCurr_TWO] "
                     + ",[TransAmt_TWO] "

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
                     //  + " TOP 150000 "
                     + "@OriginFile"
                     //    + " , SeqNo "
                     + " ,@Origin "
                       + " ,case "

                       //       + "  (TXNSRC = '1' AND TXNDEST = '5'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') "
                       //+ "  OR (TXNSRC = '1' AND TXNDEST = '13'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') " //  extra recirds for master Fawry 
                       //+ " OR (TXNSRC = '1' AND TXNDEST = '4') "  // VISA 

                       + " WHEN (TXNSRC = '1' AND (TXNDEST = '5') )  THEN '" + PRX + "236' " // All BDC236

                         //+ " WHEN (TXNSRC = '5' AND TXNDEST = '1') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "230' " // All Incoming Master to Flex
                         //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND TXN <> '0-POS Purchase' AND left([TXN], 2) <> '30' THEN '" + PRX + "232' " // All Incoming Master to Prepaid
                         //+ " WHEN (TXNSRC = '5' AND TXNDEST ='1') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "231' " // All Incoming POS
                         //+ " WHEN (TXNSRC = '5' AND TXNDEST = '12') AND (TXN = '0-POS Purchase' OR left([TXN], 2) = '30' ) THEN '" + PRX + "233' " // All Incoming POS - Prepaid we have made the 233 t0 231
                         + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "225' " // VISA 
                      + " else 'Not_Def' "
                     + " end "

                       + ",case "
                     + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                     + " else '10' "
                     + "end "
                     + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS DateTime)"
                        //+ ", CAST([LOCAL_DATE] as datetime) "
                        + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                        + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                        + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                     + ",case "
                     + " when left([TXN], 2) = '10' THEN 11 "
                     + " when left([TXN], 2) = '11' THEN 11 "
                     + " when left([TXN], 2) = '12' THEN 11 "
                     + " when left([TXN], 2) = '13' THEN 11 "
                     + " when left([TXN], 2) = '30' THEN 11 "
                     + " when left([TXN], 2) = '81' THEN 11 "
                     + " when left([TXN], 2) = '21' THEN 23 "
                     + " when left([TXN], 2) = '40' THEN 33 "
                     + " when left([TXN], 2) = '41' THEN 33 "
                     + " when left([TXN], 2) = '20' THEN 21 " // REFUND '22'
                     + " when left([TXN], 2) = '22' THEN 21 " // Credit Adjustment 
                     + " else 11 "
                     + "end "
                     + ",[TXN] " // [TransDescr]

                          // First Pair Ccy and Amount - SETTLEMENT // This is true for master card 
                          //  + " , ISNULL([SETTLEMENT_CODE], '') "
                          + ",Case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN ISNULL([SETTLEMENT_CODE], '') "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '4') THEN ISNULL([SETTLEMENT_CODE], '') " // VISA
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN ISNULL([ACQ_CURRENCY_CODE], '') "
                     + " WHEN TXNSRC = '5' THEN  ISNULL([SETTLEMENT_CODE], '') "
                     + " else '818' "
                     + " end "
                        // + " , CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                        + ",Case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '4') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN CAST([AMOUNT] As decimal(18,2)) "
                     + " WHEN TXNSRC = '5' THEN  CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                     + " else 0 "
                     + " end "
                      // Second Pair Ccy and Amount
                      //     + ", Case "   // Currency Code
                      //+ " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN ISNULL([SETTLEMENT_CODE], '') "
                      //+ " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                      //+ " end "
                      //  + ", Case "   // Transaction amount
                      //+ " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                      //+ " ELSE CAST([AMOUNT] As decimal(18,2)) "
                      //+ " end "

                      + " , '818' "
                      + ",case "
                     + " WHEN (TXNSRC = '1'  AND TXNDEST = '5') THEN CAST([AMOUNT] As decimal(18,2)) "
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '4') THEN CAST([AMOUNT] As decimal(18,2)) " // VISA 
                      + " WHEN (TXNSRC = '1'  AND TXNDEST = '13') THEN CAST([AMOUNT_EQUIV] As decimal(18,2)) "
                     + " WHEN TXNSRC = '5' THEN  CAST([AMOUNT_EQUIV] As decimal(18,2)) "
                     + " else 0 "
                     + " end "
                     //+ " , CAST([AMOUNT_EQUIV] As decimal(18,2)) "

                     + ",CAST(Right([TRACE],6) as int) " // TRace 

                  + ", RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12) " // RRN FOR Master Fawry 

                     + ",[TRACE] " // Full
                      + ",ISNULL(AUTHNUM, '')" // Auth Number 
                     + ", 'Fawry For Master' " // 
                     + ", ISNULL(TERMID, '') " // Terminal Id 
                     + ", ltrim(rtrim(ISNULL([MASK_PAN], '')))   "
                     // + ", ISNULL([ACTNUM], '') "
                     + " , RIGHT(ACTNUM, 14) "
                     // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                     + ",ltrim(rtrim([RESPCODE]))  "

                     + ", @LoadedAtRMCycle"
                     + ", @Operator"
                      + ", ltrim(rtrim([TXNSRC]))"
                       + ", ltrim(rtrim([TXNDEST]))"
                          + ", CAST([TRANDATE] as datetime) "
                         + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                         + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                         + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                         //For  Net_TransDate
                         + " , CAST(ISNULL(try_convert(datetime, [LOCAL_DATE], 103 ), '1900-01-01') AS Date)"
                         // + ", CAST([LOCAL_DATE] as datetime) "
                         + ", ISNULL([PAN], '') " // For Encrypted Card

                         + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                         + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                          + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                          // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                          + ", CAST([SETTLEMENT_DATE] as date) "
                        + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " WHERE  "
                   + "  (TXNSRC = '1' AND TXNDEST = '5'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') "  // Extra records for master Fawry 
                   + "  OR (TXNSRC = '1' AND TXNDEST = '13'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') " //  extra records for master Fawry 
                   + " OR (TXNSRC = '1' AND TXNDEST = '4') "  // VISA 
                   ;
                //WHEN(TXNSRC = '1' AND TXNDEST = '4') THEN 'BDC225'

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
                            System.Windows.Forms.MessageBox.Show("Records Inserted for dual Currencies" + Environment.NewLine
                                   + "..:.." + stpLineCount.ToString());
                        }


                    }
                    catch (Exception ex)
                    {
                        conn.StatisticsEnabled = false;
                        conn.Close();
                        stpErrorText = stpErrorText + "Cancel At _IST_Insert_TWO CURRENCIES";

                        stpReturnCode = -1;

                        stpReferenceCode = stpErrorText;

                        CatchDetails(ex);

                        return;
                    }
                //**********************************************************
                stpErrorText += DateTime.Now + "_" + "Stage_03_IST_Records_loaded..Records:.Master Two Currencies. MASTER FAWRY" + stpLineCount.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }

            // BDC240 and BDC236 and BDC225
            // Make a credit card transaction out of tranfers with full card number 
            // Make ALSO Master - where is FAWRY is involved - BDC236
            // Insert it into TWIN
            //
            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN]"
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
                 // + ",[AmtFileBToFileC] "
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
                 //  + " TOP 150000 "
                 + "@OriginFile"
                 //    + " , SeqNo "
                 + " ,@Origin "
                    //  + ", 'BDC240' "
                    + ", Case "   // 
                                  //   + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 'BDC236' " // 
                 + " WHEN (TXNDEST = '5'  OR TXNDEST = '13') THEN 'BDC236' "
                  + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN 'BDC225' "
                 + " WHEN TXN = '410002-TRANSFER' THEN 'BDC240' "
                 + " ELSE '' "
                 + " end "

                   + ",case "
                 + " when left([TXN], 1) = '0' THEN '20' " // Terminal Type
                 + " else '10' "
                 + "end "

                 + ", CAST([LOCAL_DATE] as datetime) "
                    + "+ CAST(substring(right('000000' + [LOCAL_TIME], 6), 1, 2) + ':' "
                    + "+ substring(right('000000' + [LOCAL_TIME], 6), 3, 2)"
                    + " + ':' + substring(right('000000' + [LOCAL_TIME], 6), 5, 2) as datetime)"
                  + ", Case "   // TRANSACTION TYPE
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 11 "
                  + " WHEN (TXNDEST = '4') THEN 11 "
                 + " WHEN TXN = '410002-TRANSFER' THEN 23 "
                 + " ELSE 0 "
                 + " end "
                 + ", Case "   // [TransDescr]
                 + " WHEN (TXNDEST = '5' OR TXNDEST = '13' ) THEN 'Master for Fawry' "
                 //+ " WHEN (TXNDEST = '4') THEN 'Master for Fawry' "
                 //+ " WHEN (TXNDEST = '5' OR TXNDEST = '13') THEN 'Master for Fawry' "
                 + " WHEN TXN = '410002-TRANSFER' THEN 'CREDIT TO Credit Card' "
                 + " ELSE TXN "
                 + " end "
                 + ", Case "   // Currency Code
                 + " WHEN (TXNDEST in ( '4' ,'5', '13' )) THEN ISNULL([SETTLEMENT_CODE], '') "
                 + " ELSE ISNULL([ACQ_CURRENCY_CODE], '') "
                 + " end "
                   + ", Case "   // Transaction amount
                 + " WHEN (TXNDEST in ( '4' ,'5', '13' )) THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) "
                 + " ELSE CAST([AMOUNT] As decimal(18,2)) "
                 + " end "
                 //  + " , CAST([AMOUNT_EQUIV] As decimal(18,2)) " // In Place of the spare amount field 
                 //+ ", ISNULL([ACQ_CURRENCY_CODE], '') "
                 //+ ",CAST([AMOUNT] As decimal(18,2)) "

                 + ",CAST(Right([TRACE],6) as int) " // TRACE

                  // + " , ltrim(rtrim(ISNULL([REFNUM], ''))) " // RRNumber
                  + ", Case "
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '1') THEN ISNULL(REPLACE(LTRIM(REPLACE([REFNUM], '0', ' ')), ' ', '0'), '')  " // RRN For FAWRY
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '12') THEN ISNULL(REPLACE(LTRIM(REPLACE([REFNUM], '0', ' ')), ' ', '0'), '')  " // RRN For FAWRY Prepaid
                 + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '13') THEN ISNULL(REPLACE(LTRIM(REPLACE([REFNUM], '0', ' ')), ' ', '0'), '')  " // RRN For FAWRY Credit Card
                  + " WHEN (left([TXN], 6) = '810022' AND TXNDEST = '5') THEN ISNULL(REPLACE(LTRIM(REPLACE([REFNUM], '0', ' ')), ' ', '0'), '') " // RRN For FAWRY Master Card
                  + " ELse ltrim(rtrim(ISNULL([REFNUM], ''))) "
                  + " End "

                 + ",[TRACE] " // FULL TRACE
                 + ", Case "   // AUTH Number
                 + " WHEN (TXNDEST = '5' ) THEN REFNUM "
                 + " WHEN TXN = '410002-TRANSFER' THEN ISNULL(AUTHNUM, '') "
                 + " ELSE '' "
                 + " end "

                 + ", '' " //  Comment 
                 + ", ISNULL(TERMID, '') " // Terminal Id 
                 + ", Case "   // [CARD]
                 + " WHEN (TXNDEST in ( '4', '5','13') ) THEN [MASK_PAN] "
                 + " WHEN TXN = '410002-TRANSFER' THEN ltrim(rtrim(ISNULL([FULL_CARD], ''))) " // We get the card of the Credit Card
                 + " ELSE '' "
                 + " end "

                 // + ", ISNULL([ACTNUM], '') "
                 + " , RIGHT(ACTNUM, 14) "
                 // + " , REPLACE(LTRIM(REPLACE([ACTNUM], '0', ' ')), ' ', '0') "
                 + ",ltrim(rtrim([RESPCODE]))  "

                 + ", @LoadedAtRMCycle"
                 + ", @Operator"
                  + ", ltrim(rtrim([TXNSRC]))"
                   + ", ltrim(rtrim([TXNDEST]))"
                      + ", CAST([TRANDATE] as datetime) "
                     + "+ CAST(substring(right('000000' + [TRANTIME], 6), 1, 2) + ':' "
                     + "+ substring(right('000000' + [TRANTIME], 6), 3, 2)"
                     + " + ':' + substring(right('000000' + [TRANTIME], 6), 5, 2) as datetime)"

                     //For  Net_TransDate
                     + ", CAST([LOCAL_DATE] as datetime) "
                     + ", ISNULL([PAN], '') " // For Encrypted Card

                     + ", ISNULL([MERCHANT_TYPE], '') " // MERCHANT_TYPE
                     + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME

                      + ", CAST([CAP_DATE] as date) " // CAP_DATE
                                                      // + ", CAST([CAP_DATE] as date) " // CAP_DATE
                      + ", CAST([SETTLEMENT_DATE] as date) "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 + " WHERE (FULL_CARD <> '' and TXN = '410002-TRANSFER' AND RESPCODE = '0') " // SUCCESFUL CREDIT CARD
                  + " OR  TXNSRC = '1' AND TXNDEST = '4'  " // All outgoing Visa 
                   + " OR (TXNSRC = '1' AND TXNDEST = '5'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0') " // CREATE EXTRA RECORD FOR MASTER CARD AND FAWRY
                  + "  OR (TXNSRC = '1' AND TXNDEST = '13'  AND left([TXN], 6) = '810022'  AND RESPCODE = '0' AND SETTLEMENT_AMOUNT <> '0')  "                                                                                                                             //  + " OR (TXNSRC = '1' AND TXNDEST = '13' AND left([TXN], 6) = '810022' AND RESPCODE = '0' AND AMOUNT <> '0' ) "
                   ; //  same

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
            stpErrorText += DateTime.Now + "_" + "Stage_CREDIT CARD RECORDS CREATED IN TWIN:.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //**********************************************************
            // UPDATE [TransType] when Reversals
            // 21 for withdrawls 13 for Deposits 
            //**********************************************************

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
                 + " ,[Origin] "
                  + " ,[MatchingCateg] "
                 + " ,[TerminalType] "
                 + " ,[TransDate] "

                 + " ,[TransType] "
                 + ",[TransDescr] "
                   + ",[TransCurr] "
                 + ",[TransAmt] "
                 //  + ",[AmtFileBToFileC] "
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
                   // + " WHEN (TXNSRC = '1' AND TXNDEST = '4') THEN '" + PRX + "225' " // All outgoing Visa
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) <> '81') THEN '" + PRX + "235' " // All outgoing Master
                                                                                                                       //  + " WHEN (TXNSRC = '1' AND TXNDEST = '5') AND left([TransDescr], 2) <> '81' THEN '"+PRX+"235' " // All outgoing Master
                                                                                                                       // + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81' AND TransAmt<> '0') THEN '"+PRX+"251' "
                  + " WHEN (TXNSRC = '1' AND TXNDEST = '8') THEN '" + PRX + "279' " // All outgoing MEEZA our ATMS
                    + " WHEN (TXNSRC = '1' AND TXNDEST = '10') THEN '" + PRX + "279' " // All outgoing MEEZA our ATMS
                   + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) <> '81' THEN '" + PRX + "240' "  // Credit CARD
                    + " WHEN (TXNSRC = '1' AND TXNDEST = '13') AND left([TransDescr], 2) = '81' THEN '" + PRX + "251' " // All outgoing CREDIT Card FOR FAWRY (Was Going Through Master Card ???)
                     + " WHEN (TXNSRC = '1' AND TXNDEST = '5' AND left([TransDescr], 2) = '81') THEN '" + PRX + "251' " // Fawry paid through credit card
                   + " WHEN TXNSRC = '1' AND left([TransDescr], 6) = '810022' AND (TXNDEST = 1 OR TXNDEST = 12) THEN '" + PRX + "251'  " // FAWRY THAT GOES TO FLEXCUBE OR 12 prepaid
                                                                                                                                         // + " WHEN (TXNSRC = '1' AND TXNDEST = '18') THEN '" + PRX + "275' " // ALL OUTGOING MEEZA
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
      // FOR VISA USE SETTLEMENT AMOUNT AND Sett
      + " ,[TransCurr] "
      + " ,[TransAmt] "
      //  + ",[AmtFileBToFileC] "
      + " ,[TraceNo] "
      + ", Case "
                  + " WHEN (left([TransDescr], 6) = '810022') THEN ISNULL(REPLACE(LTRIM(REPLACE([RRNumber], '0', ' ')), ' ', '0'), '')  "
                  + " ELse RRNumber "
                  + " End "
      //+ " ,[RRNumber] "
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
                   //   + "  (" +
                   //   " TXNSRC = '1' AND TXNDEST = '4')  " // All outgoing Visa 
                   + " (TXNSRC = '1' AND TXNDEST = '5')  " // All outgoing Master
                   + " OR (TXNSRC = '1' AND TXNDEST = '8')  " // All outgoing MEEZA GLOBAL 
                                                              //  + " OR (TXNSRC = '1' AND TXNDEST = '10')  " // All outgoing MEEZA GLOBAL too 
                   + " OR (TXNSRC = '1' AND TXNDEST = '13') " // All outgoing CREDIT Card Going Through Master Card
                                                              //  TXNSRC = '1' AND TXNDEST = '13' AND left([TransDescr], 2) = '81' AND AMOUNT <> '0'                  
                   + " OR (left([TransDescr], 6) = '810022' )  "// ALL FAWRY ... leaving out the '810021'
                                                                //  + " OR (TXNSRC = '1' AND TXNDEST = '18') " // ALL OUTGOING TO MEZZA

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


            //**************************************************************************************
            // 15/03/2021
            // Here Update Master Card which is 235, 236 category with the Settlement amount and currency
            // GET THE VALUES FROM THE BULK FILE 
            // ************************************************************************************
            // LEAVE IT HERE
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
          + " SET"
            + " [TransCurr] = ISNULL(t2.[SETTLEMENT_CODE], '')  "
           + " ,[TransAmt] = CAST(t2.[SETTLEMENT_AMOUNT] As decimal(18,2)) "
            + ",AmtFileBToFileC = 99.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t1 "
          + " INNER JOIN "
          + " [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + " t2"

          + " ON "
          + "  t1.RRNumber = t2.REFNUM "
          + " AND t1.TerminalId = t2.TERMID "

          + " WHERE  "
          + " t1.Processed = 0  AND t1.MatchingCateg in ('" + PRX + "235', 'BDC236') And t1.LoadedAtRMCycle = @LoadedAtRMCycle AND AmtFileBToFileC <> 99.99" // 
          + " AND (t2.TXNSRC = '1' AND t2.TXNDEST = '5' AND CAST(t2.[SETTLEMENT_AMOUNT] As decimal(18,2)) > 0  ) "
          ;
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During UPDATE TWIN For Master Settlement ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATE TWIN For Master Settlement..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // CLEAR TABLE 
            //
            //  Set Comment to blank 
            // 
            int U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [Comment] = '' "
               + " WHERE  LoadedAtRMCycle = @LoadedAtRMCycle AND Comment <> '' "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }


            // CLEAR TABLE 
            //
            //  After the TWIN table with NOT WANTED FAWRY
            // 
            U_Count = 0;

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " SET "
                + " [MatchingCateg] = 'FawryType' "
                + " ,[Processed] = 1 "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"
                  + " ,[Comment] = 'Fawry gone to IST Twin' "
               + " WHERE   Processed = 0 AND left([TransDescr], 6) = '810022' AND MatchingCateg <> '" + PRX + "250'  "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
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
            // DELETE DUBLICATES IN IST 

            Mgt.DeleteDuplicatesInIST(InReconcCycleNo);

            //****************************************************
            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            string WFileName = "CIT_EXCEL_TO_BANK";
            string TargetDB = "[RRDM_Reconciliation_ITMX]";
            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

            if (Cv.RecordFound == true )
            {
                //
                // CREATE WORKING TABLE FOR IST Vs CIT Transactions
                //

                RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

                string WTableId = "[RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS]";
                // MessageBox.Show("We Include all previous cycles for testing peurposes. Correct when needed  ");
                Ce.Insert_SWITCH_TXNS_For_ATMS(WTableId, InReconcCycleNo, LastSeqNo);
                //****************************************************
                Mgt.DeleteDuplicatesInCIT_FILE(InReconcCycleNo);
            }

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
          + " AND A.Minutes_Date = B.Minutes_Date "
          //+ " AND A.TraceNo = B.TraceNo "
          // + " AND A.TransAmt = B.TransAmt "
          + " WHERE "
          + " ( "
            + " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) " // Leave it as it is    
            + " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
            // + " AND (A.ResponseCode = 0 AND B.ResponseCode = 0 )"  //  LEAVE IT AS COMMENT - do not activate it. 
            + " AND (left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND A.MatchingCateg <> 'BDC277'  "  // EXCLUDE BDC277
            + " AND B.MatchingCateg <> 'BDC277'  "  // EXCLUDE 
            + " ) "
            //+ " OR "
            // + " A.MatchingCateg = 'BDC278' AND TrnasType " // Meeza Special 
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
          + " AND A.Minutes_Date = B.Minutes_Date "
          + " AND A.TraceNo = B.TraceNo " // Include it for BDC277
                                          // + " AND A.TransAmt = B.TransAmt "
          + " WHERE "
          + " ( "
            + " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) " // Leave it as it is    
            + " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR (LEFT(B.FullTraceNo, 1) = '1')) ) "
            // + " AND (A.ResponseCode = 0 AND B.ResponseCode = 0 )"  //  LEAVE IT AS COMMENT - do not activate it. 
            + " AND (left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
               + " AND A.MatchingCateg = 'BDC277'  "  // EXCLUDE BDC277
            + " AND B.MatchingCateg = 'BDC277'  "  // EXCLUDE 
            + " ) "
            //+ " OR "
            // + " A.MatchingCateg = 'BDC278' AND TrnasType " // Meeza Special 
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
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE "
            //+ "(left(A.[TransDescr], 1) = '0' AND left(B.[TransDescr], 1) = '0')"
            //+ " AND (LEFT(A.FullTraceNo, 1) = '4' AND A.Processed = 0) "
            //+ " AND((LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1'))  AND B.Processed = 0) "
            //+ " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
            //+ " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
             + " (A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4' ) "
            + " AND (B.Processed = 0 AND (LEFT(B.FullTraceNo, 1) = '2' OR(LEFT(B.FullTraceNo, 1) = '1')) ) "
            + " AND (left(A.[TransDescr], 1) = '0' AND left(B.[TransDescr], 1) = '0') "  // POS
            + " AND (left(A.[ResponseCode], 1) = '0' AND left(B.[ResponseCode], 1) = '0') "  // POS
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
            // MAKE THEM NOT PROCESSED
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
            return; // no need for now 
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


        //
        //// CREATE FLEXCUBE FROM CORE
        /////
        public void InsertRecords_GTL_FLEXCUBE_2(string InOriginFileName, string InFullPath
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

            // Truncate Flexcube FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "_FLEXCUBE_BULK_Records_2";
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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "_FLEXCUBE_BULK_Records Insert";
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

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            RecordFound = false;

            //
            // Correct Amount
            // remove not Needed Characters 
            // "2,214.69"
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, ',', '')"
                     //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_FLEXCUBE_BULK_Records_2]"
                     //+ " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '.', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '-', '')"
                      + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [COD_ACCT_NO] = TRIM([COD_ACCT_NO]) "
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


            RecordFound = false;

            // INSERT IN FLEXCUBE
            //

            SQLCmd =
                 // "SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Flexcube]"
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

                 + ",[FullTraceNo] "

                + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "

                    + " , [Net_TransDate] "
                    + " , [CAP_DATE] "
                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                + " ,'" + PRX + "237'"  // UPDATE AS OTHER THEN BASED ON CARD WE SET UP BDC201
                                        //  + " ,case" // Destination
                                        //+ " WHEN LEFT([PAN_MASK],6) = '526402' THEN 'BDC201' "
                                        //+ " else 'BDC202' "
                                        //+ " end "
                                        //+ " , 'BDC231' Flexcube" // All Master POS
                                        //  

                 //    + ",case "
                 // + " when [COD_TXN_MNEMONIC] = '2201'  THEN '" + PRX + "201'"  // From our ATMs 
                 // + " when [COD_TXN_MNEMONIC] = '2301'  THEN '" + PRX + "210'"  //  from 123 and others
                 // + " when [COD_TXN_MNEMONIC] = '2250' THEN '" + PRX + "201'" // deposits
                 // + " when [COD_TXN_MNEMONIC] = '2204' THEN '" + PRX + "201'" // transfer
                 //// + " when [COD_TXN_MNEMONIC] = '2208'  THEN 'Fawry' " // Fawry
                 //   + " else '" + PRX + "237'"
                 //   + "end "

                 + ", ISNULL([TERM_ID], '') " // Terminal 
                 + ", @TerminalType " // Depends from Origin 10 or 20 
                                      // LEFT([DAT_TXN_POSTING],10) = '10/01/2019 00:'
                                      //  + ",ISNULL(try_convert(datetime, [DAT_TXN_POSTING] + ' ' "
                 + ",ISNULL(try_convert(datetime, LEFT([DAT_TXN_PROCESSING],10) + ' ' "
                 + "   + substring(right('000000' + [TIM_LOCAL], 6), 1, 2) + ':' "
                 + "    + substring(right('000000' + [TIM_LOCAL], 6), 3, 2) "
                 + "    + ':' + substring(right('000000' + [TIM_LOCAL], 6), 5, 2), 103), '1900-01-01') "

                  + ",case "
                // [COD_TXN_MNEMONIC] = '2201'--  Withdrawel , 2250 deposits, 2208 Fawry, 
                // 2301 withdrawl from 123 , '2204' transfer
                + " when [COD_TXN_MNEMONIC] = '2201'  THEN 11  " // From our ATMs 
                + " when [COD_TXN_MNEMONIC] = '2301'  THEN 11  " //  from 123 and others
                + " when [COD_TXN_MNEMONIC] = '2250' THEN 23 " // deposits
                + " when [COD_TXN_MNEMONIC] = '2204' THEN 33 " // transfer
                + " when [COD_TXN_MNEMONIC] = '2208'  THEN 11 " // Fawry
                  + " else 0 "
                  + "end "

                  // + ",[CA_NAME] " //  in place of Transction Description 
                  + ",case "
                + " when [COD_TXN_MNEMONIC] = '2201'  THEN 'From OWN ATMS - Withdrawl'  " // From our ATMs 
                + " when [COD_TXN_MNEMONIC] = '2301'  THEN 'From 123 and others - Debit'  " //  from 123 and others
                + " when [COD_TXN_MNEMONIC] = '2250' THEN 'From OWN ATMS - Deposits' " // deposits
                + " when [COD_TXN_MNEMONIC] = '2204' THEN 'From OWN ATMS - Transfers' " // transfer
                + " when [COD_TXN_MNEMONIC] = '2208'  THEN 'Fawry' " // Fawry
                  + " else 'Not Found' "
                  + "end "
                  + ", 'EGP' "
                 + ",CAST([AMT_TXN_LCY] As decimal(18,2)) "
                //  + ",ISNULL([REF_SYS_TR_AUD_NO], '') " // in place TRACE 
                + " ,ISNULL(TRY_CAST([REF_SYS_TR_AUD_NO] AS INT),0)" // In place of trace 
                 + ",ISNULL([RETRIEVAL_REF_NO] , '') " // in place RRN 
                 + ",case "
                 + " when [COD_MSG_TYP] = '200'  THEN '2' + ISNULL([REF_SYS_TR_AUD_NO] , '') "
                 + " when [COD_MSG_TYP] = '220'  THEN '2' + ISNULL([REF_SYS_TR_AUD_NO] , '') "
                 + " when [COD_MSG_TYP] = '420'  THEN '4' + ISNULL([REF_SYS_TR_AUD_NO] , '')  " // Reversals for withdrawls and deposits
                 + " when [COD_MSG_TYP] = '401'  THEN '4' + ISNULL([REF_SYS_TR_AUD_NO] , '') " // Reversals for transfers
                 + " else '0' "
                 + "end "                                                                            //   
                                                                                                     //  + ", ISNULL([PAN_MASK], '') "  // 
                + " , RIGHT(COD_ACCT_NO, 14) "                                                                             //+ " , ISNULL([COD_ACCT_NO], '') "
                                                                                                                           //  + " , REPLACE(LTRIM(REPLACE([COD_ACCT_NO], '0', ' ')), ' ', '0') "
                + ", '0' " // In Place of PCODE
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'0'" // Origin ATMs -initialise with zero

                  + " ,'0'" // Final Destination -initialise with zero

                 //+ " ,case" // Destination
                 //+ " WHEN LEFT([PAN_MASK],6) = '526402' THEN '1' "
                 //+ " else '12' "
                 //+ " end "

                 + " ,'Comment' " // for the comment 
                                  // NET Trans Dte
                                  // + " , ISNULL(try_convert(datetime, [DAT_TXN_POSTING], 103 ), '1900-01-01')  "
                + " , ISNULL(try_convert(datetime, LEFT([DAT_TXN_PROCESSING],10),103 ), '1900-01-01')  " // NET VALUE 

                 //DAT_TXN_VALUE
                 + " , ISNULL(try_convert(datetime, LEFT([DAT_TXN_VALUE],10),103 ), '1900-01-01')  " // CAP_DATE ... Value Date 

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                + " WHERE  "
                + " CAST([AMT_TXN_LCY] As decimal(18,2)) > 0 "
                + " ORDER BY TERM_ID, [DAT_TXN_PROCESSING], [TIM_LOCAL] ";

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted FLEXCUBE" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT RECORDS IN Flexcube table";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO Flexcube with..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE Details from IST Based on Accno and Terminal Id
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
          + " ,RRNumber = t2.RRNumber " // leave it here
          + ",Card_Encrypted = t2.Card_Encrypted "
            + ",AUTHNUM = t2.AUTHNUM "
          //+ ",TransDescr = t2.TransDescr " // DONT UPATE DESCRIPTION
          + ",MatchingCateg = t2.MatchingCateg "
         // + ",TransDate = t2.TransDate "
         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "
          // + ",TerminalId = t2.TerminalId "
          + ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
           + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          // + " t1.Processed = t2.Processed "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
                                              // + " AND t1.TransDate = t2.TransDate "
         + " AND t1.Net_TransDate = t2.Net_TransDate "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.TransAmt = t2.TransAmt "
          + " AND t1.AccNo = t2.AccNo "
          + " WHERE  (t1.Processed = 0 ) "
            + "AND ((t2.Processed = 0 AND T2.TXNDEST = '1')  "
            //  + "      OR (t2.Processed = 1 AND T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            + "      OR (T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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
            // UPDATE details from IST where Origin is 123 or Meeza
            // 
            // Reason is because Flexcube has wrong terminal whereas 123 and IST is correct
            // Terminal not included in matching 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
          + ",Card_Encrypted = t2.Card_Encrypted "
           + ",AUTHNUM = t2.AUTHNUM "
          //+ ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "
          //+ ",TransDate = t2.TransDate "
          //  + ",TerminalId = t2.TerminalId "
          //+ ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
             + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          //+  "t1.TerminalId = t2.TerminalId "
          + " t1.Net_TransDate = t2.Net_TransDate "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber  "
           //+ " AND t1.FullTraceNo = t2.FullTraceNo "
           //+ " AND t1.AccNo = t2.AccNo "
           + " AND t1.TransAmt = t2.TransAmt "

          + " WHERE   (t1.Processed = 0 and t1.AmtFileBToFileC = 0 ) "
          + "AND ((t2.Processed = 0 AND (T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1')  "
           //  + "      OR (t2.Processed = 1 AND (T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
           + "      OR ((T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
           ; // 123  

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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Card No and Origin_1 Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE details from IST where Acno not the same and Terminal not the same
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
            + ",MatchingCateg = t2.MatchingCateg "

          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
             + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "

          + ",TransDescr = t2.TransDescr "

          + ",TransCurr = t2.TransCurr "
          + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.TraceNo = t2.TraceNo "

          + " AND t1.RRNumber = t2.RRNumber  "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0 ) "
           + "AND ((t2.Processed = 0 AND T2.TXNDEST = 12) "
              //  + "      OR (t2.Processed = 1 AND T2.TXNDEST = 12 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
              + "      OR (T2.TXNDEST = 12 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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

                    stpErrorText = stpErrorText + "Cancel During where Acno not the same Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Card No and Not same Accno and Origin _ 1 Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE details from IST  Acno the same and Terminal not the same For Transfers
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
         + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "
          //+ ",TransType = t2.TransType "
          //+ ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "
          //+ ",TransDate = t2.TransDate "
          + ",TerminalId = t2.TerminalId "
          //+ ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
        + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.TraceNo = t2.TraceNo "

          + " AND t1.RRNumber = t2.RRNumber  "
          + " AND t1.AccNo = t2.AccNo "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0  )  "
            + "AND ((t2.Processed = 0 AND T2.TransType = 33) "
             //   + "      OR (t2.Processed = 1 AND T2.TransType = 33 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
             + "      OR ( T2.TransType = 33 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating for Transfers _1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Transfers_1 Card No and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE details from IST where Amount is in Local And Original in Fcy - MASTER
            //
            // UPDATE AMOUNT 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + " ,TransCurr = t2.TransCurr "
          + " ,TransAmt = t2.TransAmt " // here we update amount for fx txns 
             + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "


          + ",MatchingCateg = t2.MatchingCateg "
            + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

          + "  t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber  "

           + " AND t1.AccNo = t2.AccNo "
           + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  t1.Processed = 0  and t1.AmtFileBToFileC = 0  "
             + "AND ((t2.Processed = 0 AND T2.TXNSRC = '5' and T2.TXNDEST = '1') "
              // + "      OR (t2.Processed = 1 AND T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
              + "      OR (T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            ; // For not processed yet records
            // MASTER and POS

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

                    stpErrorText = stpErrorText + "Cancel During Foreign Amount Updating _ 1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FX updating _1 and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //
            // UPDATE details from IST where Amount is in Local And Original in Fcy - MASTER
            //
            // UPDATE AMOUNT 
            // FOR POS what not update in previous ... Different Matching fields

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + ",TransCurr = t2.TransCurr "
          + " ,TransAmt = t2.TransAmt " // here we update amount for fx txns 
          + ",Card_Encrypted = t2.Card_Encrypted "
             + ",AUTHNUM = t2.AUTHNUM "
          // , RRNumber, AUTHNUM
          + ",RRNumber = t2.RRNumber " // RRN 

          + ",MatchingCateg = t2.MatchingCateg "
          + ",TransDate = t2.TransDate "

          + ",Net_TransDate = t2.Net_TransDate "
          + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

          + "  t1.TraceNo = t2.TraceNo "
           //+ " AND t1.RRNumber = t2.RRNumber  " // FOR POS MATCH ACCOUNT AND TRACE 
           + " AND t1.AccNo = t2.AccNo "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0 )  "
             + "AND ((t2.Processed = 0 AND T2.TXNSRC = '5' and T2.TXNDEST = '1') "
               // + "      OR (t2.Processed = 1 AND  T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
               + "      OR ( T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            ; // For not processed yet records
              // MASTER 

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

                    stpErrorText = stpErrorText + "Cancel During Amount Updating for POS";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "POS and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // ***************************
            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
                    + " SET [TransTypeAtOrigin] = 'INTERNAL' "
                    + " WHERE Processed = 0 AND MatchingCateg = '" + PRX + "231'  "
                    ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo); NO CYCLE 
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Internal..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // 
            // Handle Transfers second transaction of transfers
            // Flexcube has two txns

            SQLCmd =
        " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " set [Processed] = 1 , IsSettled = 1 "
            + " ,[ProcessedAtRMCycle] = @RMCycle," + "Comment = 'TransferTo' "
        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"
        + " ON "
            + "  t1.RRNumber = t2.RRNumber "
            + " AND t1.TransAmt = t2.TransAmt "
            + " AND t1.TerminalId = t2.TerminalId "
         + " WHERE t1.Processed = 0  AND t1.AccNo <> t2.AccNo AND t1.MatchingCateg = '" + PRX + "201'"
         + " AND ( t2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "201')";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
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
                    stpErrorText = stpErrorText + "Cancel During Handling Transfers";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Handling Transfers.." + Counter.ToString() + "\r\n";
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
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] B "
          + " ON "//A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " A.RRNumber = B.RRNumber "
          + " AND A.TerminalId = B.TerminalId "
          + " AND A.Net_TransDate = b.Net_TransDate "
          + " AND A.AccNo = b.AccNo "
          + " AND A.AUTHNUM = b.AUTHNUM "
             + " WHERE "
            //+ "(left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            + " ( A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4') "
            + " AND (B.Processed = 0 AND LEFT(B.FullTraceNo, 1) = '2' ) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR FLEXCUBE." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Flexcube";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //string FileId = "Flexcube";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Flexcube]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Flexcube";

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
            stpErrorText += DateTime.Now + "_" + "Flexcube Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;

        }

        //
        //// CREATE FLEXCUBE FROM CORE
        /////
        public void InsertRecords_GTL_FLEXCUBE_3(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

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

            string InDelimiter = "tap"; // It Is tap for 


            // Check if file is a Windows and Not Unix. 


            //   stpErrorText = "";
            stpReferenceCode = "";

            // CHECK THE FILE
            string TableId_Bulk = "[RRDM_Reconciliation_ITMX].[dbo].[BULK_Flexcube]";

            // MAKE UNIX oR DELIMETER VALIDATION 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Del = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            bool Correct = Del.CheckIfWindowsOrUnix_ATMS(InFullPath);

            //Correct = true; // WE MAKE TRUE TILL MARIOS HELP 

            if (Correct == false)
            {
                //MessageBox.Show("The file: " + InFullPath + Environment.NewLine
                //    + " IS not a Windows File. It cannnot be accepted "
                //    );
                // return;
            }
            if (Environment.MachineName == "RRDM-PANICOS")
            {
                Correct = Del.FindIfDelimiterErrorsInInputTable_ATMS(TableId_Bulk, InFullPath, InDelimiter, recconConnString);

                if (Correct == false)
                {

                    //string OldString = "faisal,giza";
                    //string NewString = "faisal giza"; // No comma

                    //  Del.ReplaceValueInGivenTextFile(InFullPath, OldString, NewString);
                }
            }

            // Truncate Flexcube FOR BULK

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

                    stpErrorText = stpErrorText + "Cancel During " + PRX + "_FLEXCUBE_BULK_Records_2";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // Bulk insert the tap txt file to this temporary table

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

                    stpErrorText = stpErrorText + "Cancel " + PRX + "_FLEXCUBE_BULK_Records Insert";
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

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            RecordFound = false;

            //
            // Correct Amount
            // remove not Needed Characters 
            // "2,214.69"
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '\"', '') "
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, ',', '')"
                     //+ " Update [RRDM_Reconciliation_ITMX].[dbo].[BDC_FLEXCUBE_BULK_Records_2]"
                     //+ " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '.', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET AMT_TXN_LCY = replace(AMT_TXN_LCY, '-', '')"
                      + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET [COD_ACCT_NO] = TRIM([COD_ACCT_NO]) "
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


            RecordFound = false;

            // INSERT IN FLEXCUBE
            //

            SQLCmd =
                 // "SET dateformat dmy "
                 " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Flexcube]"
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

                 + ",[FullTraceNo] "

                + ",[AccNo] "
                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                   + ",[Operator] "
                   + ",[TXNSRC] "

                     + ",[TXNDEST] "
                    + ",[Comment] "

                    + " , [Net_TransDate] "
                    + " , [CAP_DATE] "
                 + ") "

                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"
                + " ,'" + PRX + "237'"  // UPDATE AS OTHER THEN BASED ON CARD WE SET UP BDC201
                                        //  + " ,case" // Destination
                                        //+ " WHEN LEFT([PAN_MASK],6) = '526402' THEN 'BDC201' "
                                        //+ " else 'BDC202' "
                                        //+ " end "
                                        //+ " , 'BDC231' Flexcube" // All Master POS
                                        //  

                 //    + ",case "
                 // + " when [COD_TXN_MNEMONIC] = '2201'  THEN '" + PRX + "201'"  // From our ATMs 
                 // + " when [COD_TXN_MNEMONIC] = '2301'  THEN '" + PRX + "210'"  //  from 123 and others
                 // + " when [COD_TXN_MNEMONIC] = '2250' THEN '" + PRX + "201'" // deposits
                 // + " when [COD_TXN_MNEMONIC] = '2204' THEN '" + PRX + "201'" // transfer
                 //// + " when [COD_TXN_MNEMONIC] = '2208'  THEN 'Fawry' " // Fawry
                 //   + " else '" + PRX + "237'"
                 //   + "end "

                 + ", ISNULL([TERM_ID], '') " // Terminal 
                 + ", @TerminalType " // Depends from Origin 10 or 20 
                                      // LEFT([DAT_TXN_POSTING],10) = '10/01/2019 00:'
                                      //  + ",ISNULL(try_convert(datetime, [DAT_TXN_POSTING] + ' ' "
                 + ",ISNULL(try_convert(datetime, LEFT([DAT_TXN_PROCESSING],10) + ' ' "
                 + "   + substring(right('000000' + [TIM_LOCAL], 6), 1, 2) + ':' "
                 + "    + substring(right('000000' + [TIM_LOCAL], 6), 3, 2) "
                 + "    + ':' + substring(right('000000' + [TIM_LOCAL], 6), 5, 2), 103), '1900-01-01') "

                  + ",case "
                // [COD_TXN_MNEMONIC] = '2201'--  Withdrawel , 2250 deposits, 2208 Fawry, 
                // 2301 withdrawl from 123 , '2204' transfer
                + " when [COD_TXN_MNEMONIC] = '2201'  THEN 11  " // From our ATMs 
                + " when [COD_TXN_MNEMONIC] = '2301'  THEN 11  " //  from 123 and others
                + " when [COD_TXN_MNEMONIC] = '2250' THEN 23 " // deposits
                + " when [COD_TXN_MNEMONIC] = '2204' THEN 33 " // transfer
                + " when [COD_TXN_MNEMONIC] = '2208'  THEN 11 " // Fawry
                  + " else 0 "
                  + "end "

                  // + ",[CA_NAME] " //  in place of Transction Description 
                  + ",case "
                + " when [COD_TXN_MNEMONIC] = '2201'  THEN 'From OWN ATMS - Withdrawl'  " // From our ATMs 
                + " when [COD_TXN_MNEMONIC] = '2301'  THEN 'From 123 and others - Debit'  " //  from 123 and others
                + " when [COD_TXN_MNEMONIC] = '2250' THEN 'From OWN ATMS - Deposits' " // deposits
                + " when [COD_TXN_MNEMONIC] = '2204' THEN 'From OWN ATMS - Transfers' " // transfer
                + " when [COD_TXN_MNEMONIC] = '2208'  THEN 'Fawry' " // Fawry
                  + " else 'Not Found' "
                  + "end "
                  + ", 'EGP' "
                 + ",CAST([AMT_TXN_LCY] As decimal(18,2)) "
                //  + ",ISNULL([REF_SYS_TR_AUD_NO], '') " // in place TRACE 
                + " ,ISNULL(TRY_CAST([REF_SYS_TR_AUD_NO] AS INT),0)" // In place of trace 
                                                                     //  + ",ISNULL([RETRIEVAL_REF_NO] , '') " // in place RRN 

                + ",RIGHT('000000000000'+ISNULL(rtrim(RETRIEVAL_REF_NO),''),12)" // in place RRN 12 digit 

                 + ",case "
                 + " when [COD_MSG_TYP] = '200'  THEN '2' + ISNULL([REF_SYS_TR_AUD_NO] , '') "
                 + " when [COD_MSG_TYP] = '220'  THEN '2' + ISNULL([REF_SYS_TR_AUD_NO] , '') "
                 + " when [COD_MSG_TYP] = '420'  THEN '4' + ISNULL([REF_SYS_TR_AUD_NO] , '')  " // Reversals for withdrawls and deposits
                 + " when [COD_MSG_TYP] = '401'  THEN '4' + ISNULL([REF_SYS_TR_AUD_NO] , '') " // Reversals for transfers
                 + " else '0' "
                 + "end "                                                                            //   
                                                                                                     //  + ", ISNULL([PAN_MASK], '') "  // 
                + " , RIGHT(COD_ACCT_NO, 14) "                                                                             //+ " , ISNULL([COD_ACCT_NO], '') "
                                                                                                                           //  + " , REPLACE(LTRIM(REPLACE([COD_ACCT_NO], '0', ' ')), ' ', '0') "
                + ", '0' " // In Place of PCODE
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'0'" // Origin ATMs -initialise with zero

                  + " ,'0'" // Final Destination -initialise with zero

                 //+ " ,case" // Destination
                 //+ " WHEN LEFT([PAN_MASK],6) = '526402' THEN '1' "
                 //+ " else '12' "
                 //+ " end "

                 + " ,'Comment' " // for the comment 
                                  // NET Trans Dte
                                  // + " , ISNULL(try_convert(datetime, [DAT_TXN_POSTING], 103 ), '1900-01-01')  "
                + " , ISNULL(try_convert(datetime, LEFT([DAT_TXN_PROCESSING],10),103 ), '1900-01-01')  " // NET VALUE 

                 //DAT_TXN_VALUE
                 + " , ISNULL(try_convert(datetime, LEFT([DAT_TXN_VALUE],10),103 ), '1900-01-01')  " // CAP_DATE ... Value Date 

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                + " WHERE  "
                + " CAST([AMT_TXN_LCY] As decimal(18,2)) > 0 "
                + " ORDER BY TERM_ID, [DAT_TXN_PROCESSING], [TIM_LOCAL] ";

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted FLEXCUBE" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During INSERT RECORDS IN Flexcube table";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "INSERT TO Flexcube with..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // UPDATE Details from IST Based on Accno and Terminal Id
            //
            DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
          + " ,RRNumber = t2.RRNumber " // leave it here
          + ",Card_Encrypted = t2.Card_Encrypted "
            + ",AUTHNUM = t2.AUTHNUM "
          //+ ",TransDescr = t2.TransDescr " // DONT UPATE DESCRIPTION
          + ",MatchingCateg = t2.MatchingCateg "
         // + ",TransDate = t2.TransDate "
         + " ,EXTERNAL_DATE = t2.EXTERNAL_DATE "
          // + ",TerminalId = t2.TerminalId "
          + ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
           + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          // + " t1.Processed = t2.Processed "
          + "  t1.TerminalId = t2.TerminalId" //terminal not the same for 123 
          + " AND t1.TransDate = t2.TransDate "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.TransAmt = t2.TransAmt "
          + " AND t1.AccNo = t2.AccNo "
          + " WHERE  (t1.Processed = 0 ) "
            + "AND ((t2.Processed = 0 AND T2.TXNDEST = '1')  "
            //  + "      OR (t2.Processed = 1 AND T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            + "      OR (T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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
            // UPDATE details from IST where Origin is 123 or Meeza
            // 
            // Reason is because Flexcube has wrong terminal whereas 123 and IST is correct
            // Terminal not included in matching 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
          + ",Card_Encrypted = t2.Card_Encrypted "
           + ",AUTHNUM = t2.AUTHNUM "
          //+ ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "
          //+ ",TransDate = t2.TransDate "
          //  + ",TerminalId = t2.TerminalId "
          //+ ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
             + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          //+  "t1.TerminalId = t2.TerminalId "
          + " t1.Net_TransDate = t2.Net_TransDate "
          + " AND t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber  "
           //+ " AND t1.FullTraceNo = t2.FullTraceNo "
           //+ " AND t1.AccNo = t2.AccNo "
           + " AND t1.TransAmt = t2.TransAmt "

          + " WHERE   (t1.Processed = 0 and t1.AmtFileBToFileC = 0 ) "
          + "AND ((t2.Processed = 0 AND (T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1')  "
           //  + "      OR (t2.Processed = 1 AND (T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
           + "      OR ((T2.TXNSRC = '8' OR T2.TXNSRC = '18') and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
           ; // 123  

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

                    stpErrorText = stpErrorText + "Cancel During when Origin 123 is Ist Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Card No and Origin_1 Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE details from IST where Acno not the same and Terminal not the same
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
             + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "

          + ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "

          + ",TransCurr = t2.TransCurr "
          + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.TraceNo = t2.TraceNo "

          + " AND t1.RRNumber = t2.RRNumber  "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0 ) "
           + "AND ((t2.Processed = 0 AND T2.TXNDEST = 12) "
              //  + "      OR (t2.Processed = 1 AND T2.TXNDEST = 12 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
              + "      OR (T2.TXNDEST = 12 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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

                    stpErrorText = stpErrorText + "Cancel During where Acno not the same Updating_1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Card No and Not same Accno and Origin _ 1 Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE details from IST  Acno the same and Terminal not the same For Transfers
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
         + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "
          //+ ",TransType = t2.TransType "
          //+ ",TransDescr = t2.TransDescr "
          + ",MatchingCateg = t2.MatchingCateg "
          //+ ",TransDate = t2.TransDate "
          + ",TerminalId = t2.TerminalId "
          //+ ",Net_TransDate = t2.Net_TransDate "
          + ",TransCurr = t2.TransCurr "
        + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON t1.TransAmt = t2.TransAmt "
          + " AND t1.TraceNo = t2.TraceNo "

          + " AND t1.RRNumber = t2.RRNumber  "
          + " AND t1.AccNo = t2.AccNo "
          + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0  )  "
            + "AND ((t2.Processed = 0 AND T2.TransType = 33) "
             //   + "      OR (t2.Processed = 1 AND T2.TransType = 33 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
             + "      OR ( T2.TransType = 33 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating for Transfers _1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Transfers_1 Card No and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE details from IST where Amount is in Local And Original in Fcy - MASTER
            //
            // UPDATE AMOUNT 

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + " ,TransCurr = t2.TransCurr "
          + " ,TransAmt = t2.TransAmt " // here we update amount for fx txns 
             + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "


          + ",MatchingCateg = t2.MatchingCateg "
            + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

          + "  t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber  "

           + " AND t1.AccNo = t2.AccNo "
           + " AND t1.Net_TransDate = t2.Net_TransDate "

          + " WHERE  t1.Processed = 0  and t1.AmtFileBToFileC = 0  "
             + "AND ((t2.Processed = 0 AND T2.TXNSRC = '5' and T2.TXNDEST = '1') "
              // + "      OR (t2.Processed = 1 AND T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
              + "      OR (T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            ; // For not processed yet records
            // MASTER and POS

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

                    stpErrorText = stpErrorText + "Cancel During Foreign Amount Updating _ 1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FX updating _1 and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // UPDATE Transaction amount where it is different in Flexcube from IST 
            // and Category is BDC277
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + " ,TransCurr = t2.TransCurr "
          + " ,TransAmt = t2.TransAmt " // here we update amount for fx txns 
             + ",AUTHNUM = t2.AUTHNUM "
          + ",Card_Encrypted = t2.Card_Encrypted "


          + ",MatchingCateg = t2.MatchingCateg "
            + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

            + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

          + "  t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber  "

          + " WHERE  t1.Processed = 0 AND t2.Processed = 0 and t1.AmtFileBToFileC = 0 "
                    + "AND t2.MatchingCateg = 'BDC277' AND t1.TransAmt <> t2.TransAmt "

            ; // For not processed yet records
            // MASTER and POS

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

                    stpErrorText = stpErrorText + "Cancel During Foreign Amount Updating _ 1";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "FX updating _1 and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            //
            // UPDATE details from IST where Amount is in Local And Original in Fcy - MASTER
            //
            // UPDATE AMOUNT 
            // FOR POS what not update in previous ... Different Matching fields

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " SET CardNumber = t2.CardNumber "
          + ",TXNSRC = t2.TXNSRC "
          + " ,TXNDEST = t2.TXNDEST "
           + ",TransCurr = t2.TransCurr "
          + " ,TransAmt = t2.TransAmt " // here we update amount for fx txns 
          + ",Card_Encrypted = t2.Card_Encrypted "
             + ",AUTHNUM = t2.AUTHNUM "
          // , RRNumber, AUTHNUM
          + ",RRNumber = t2.RRNumber " // RRN 

          + ",MatchingCateg = t2.MatchingCateg "
          + ",TransDate = t2.TransDate "

          + ",Net_TransDate = t2.Net_TransDate "
          + ",CAP_DATE = t2.CAP_DATE "
           + ",SET_DATE = t2.SET_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

          + "  t1.TraceNo = t2.TraceNo "
           //+ " AND t1.RRNumber = t2.RRNumber  " // FOR POS MATCH ACCOUNT AND TRACE 
           + " AND t1.AccNo = t2.AccNo "

          + " WHERE  (t1.Processed = 0 and t1.AmtFileBToFileC = 0 )  "
             + "AND ((t2.Processed = 0 AND T2.TXNSRC = '5' and T2.TXNDEST = '1') "
               // + "      OR (t2.Processed = 1 AND  T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
               + "      OR ( T2.TXNSRC = '5' and T2.TXNDEST = '1' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus )) "
            ; // For not processed yet records
              // MASTER 

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

                    stpErrorText = stpErrorText + "Cancel During Amount Updating for POS";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "POS and Origin Updated from IST..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // ***************************
            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
                    + " SET [TransTypeAtOrigin] = 'INTERNAL' "
                    + " WHERE Processed = 0 AND MatchingCateg = '" + PRX + "231'  "
                    ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        // cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo); NO CYCLE 
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_Updating _TransType";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);

                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Internal..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // 
            // Handle Transfers second transaction of transfers
            // Flexcube has two txns

            SQLCmd =
        " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
          + " set [Processed] = 1 , IsSettled = 1 "
            + " ,[ProcessedAtRMCycle] = @RMCycle," + "Comment = 'TransferTo' "
        + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"
        + " ON "
            + "  t1.RRNumber = t2.RRNumber "
            + " AND t1.TransAmt = t2.TransAmt "
            + " AND t1.TerminalId = t2.TerminalId "
         + " WHERE t1.Processed = 0  AND t1.AccNo <> t2.AccNo AND t1.MatchingCateg = '" + PRX + "201'"
         + " AND ( t2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "201')";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);
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
                    stpErrorText = stpErrorText + "Cancel During Handling Transfers";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Handling Transfers.." + Counter.ToString() + "\r\n";
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
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] B "
          + " ON "//A.TransAmt = B.TransAmt " Some times IST forgets to AMt in reversal
          + " A.RRNumber = B.RRNumber "
          + " AND A.TerminalId = B.TerminalId "
          + " AND A.Net_TransDate = b.Net_TransDate "
          + " AND A.AccNo = b.AccNo "
          + " AND A.AUTHNUM = b.AUTHNUM "
             + " WHERE "
            //+ "(left(A.[TransDescr], 1) <> '0' AND left(B.[TransDescr], 1) <> '0')"  // NOT POS
            + " ( A.Processed = 0 AND LEFT(A.FullTraceNo, 1) = '4') "
            + " AND (B.Processed = 0 AND LEFT(B.FullTraceNo, 1) = '2' ) "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted FOR FLEXCUBE." + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Flexcube";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Created Reversals..." + Counter.ToString() + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            Counter = 0;
            //string FileId = "Flexcube";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[Flexcube]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_Flexcube";

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
            stpErrorText += DateTime.Now + "_" + "Flexcube Finishes with SUCCESS.." + "\r\n";

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

                    stpErrorText = stpErrorText + "Cancel During Card Updating_NOT Equal BDC215";
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

                    stpErrorText = stpErrorText + "Cancel During Card Updating_=BDC215";
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
            //FileId = "Flexcube";
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
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..." + Environment.NewLine
                          + WFullPath_01
                         );
                    }

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA BDC ATMS" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During MEEZA BDC ATMS insert of records";
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

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OWN_ATMS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON t1.TraceNo = t2.TraceNo "
          + " AND t1.RRNumber = t2.RRNumber "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.TransType = t2.TransType "
          + " AND t1.TransAmt = t2.TransAmt "

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
            //FileId = "Flexcube";
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
                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show(stpErrorText + "..Please Check..." + Environment.NewLine
                          + WFullPath_01
                         );
                    }

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
                      + ",[Comment] "
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

                 + " ,case" // LEAVE IT HERE 
                + " WHEN Left(PAN, 6 )= '507808' THEN Left(PAN, 6 )+ '**' + '****' + right([PAN], 4) " // 
                 + " else ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "
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
                     + ",'1' " // Destination Flexcube - it can be prepaid 
                               //+ " ,"
                     + " ,case" // LEAVE IT HERE GOES TEMPORALY TO COMMENT
                      + " WHEN Left(PAN, 5 )= '50780' THEN Left(PAN, 8 ) + '****' + right([PAN], 4) " // 
                      + " else ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "
                     + " end "

                  + ", CAST([EMVTransactionDate] As DATE) " // This is the IST date  // NET TRANSACTION DATE
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

          + " ,AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
          + "  t1.RRNumber = t2.RRNumber "
          //+ " AND t1.AUTHNUM = t2.AUTHNUM "
          + " AND t1.TransType = t2.TransType "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.TransAmt = t2.TransAmt "
          + " AND t1.CardNumber = t2.CardNumber "

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA_OTHER_ATMS Updating For date and time";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Update Date and Time from IST..." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Update Card number with the kept one. 
            //

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_OTHER_ATMS] "
                + " SET "
                + " [CardNumber] = Comment "
                + " , Comment = '' "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"

                 + " WHERE  "
               + "  Processed = 0 and Comment <> '' " // Non Processed and comment 
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
            //FileId = "Flexcube";
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
        //// CREATE MEEZA_GLOBAL LCL
        /////****************
        public void InsertRecords_GTL_MEEZA_GLOBAL_LCL(string InOriginFileName, string InFullPath
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
            // [RRDM_Reconciliation_ITMX].[dbo].[BDC_MEEZA_GLOBAL_BULK_Records]
            // [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL] 

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



            RecordFound = false;

            //return;

            // Bulk insert the txt file to this temporary table
            string InDelimiter = ",";

            SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          // + ",FIELDTERMINATOR = '\t'"
                          + ",FIELDTERMINATOR = '" + InDelimiter + "'"
                         + " ,ROWTERMINATOR = '\r\n' )"
                          ;

            //// Bulk insert the tap txt file to this temporary table

            //SQLCmd = " BULK INSERT [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            //              + " FROM '" + WFullPath_01.Trim() + "'"
            //              + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
            //              + " ,ROWs_PER_BATCH=15000 "
            //              + ",FIELDTERMINATOR = '\t'"
            //             + " ,ROWTERMINATOR = '\n' )"
            //              ;

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

                    stpErrorText = stpErrorText + "Cancel During MEEZA__GLOBAL Bulk Insert";
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

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            RecordFound = false;

            ////return;




            ////       
            //// CREATE MEEZA_OWN_ATMS from BULK
            ////

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL]  "
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
                     + ", SET_DATE "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                     + ",case " // MatchingCateg
                 + " WHEN Left(TerminalId,3)='" + PRX + "' THEN '" + PRX + "279'  "
                 + " WHEN left([PAN], 8) = '54844600' AND Left(TerminalId,3)<>'" + PRX + "' THEN '" + PRX + "278'   " // This BIN is not in IST 
                 + " else '" + PRX + "277'  "
                 + "end "

                 + ", @TerminalType"
                 + " ,LocalDateTime "
                 // + ", CAST([TransactionDate] As datetime) " // Transaction Date without minutes 
                 //+ " , CAST('20' + SUBSTRING(LocalDateTime, 7, 2)+ SUBSTRING(LocalDateTime, 4, 2) + SUBSTRING(LocalDateTime, 1, 2) + ' ' + SUBSTRING(LocalDateTime, 10, 5) AS datetime) " // Trans Date
                 + ",case "
                 + " when (TransactionType = '10' AND MessageType = '0210') THEN 11  "
                 + " when (TransactionType = '10' AND MessageType = '0420') THEN 21  "
                 + " when (TransactionType = '20' AND MessageType = '0210') THEN 23  "
                 + " when (TransactionType = '20' AND MessageType = '0420') THEN 13  "
                 // + " when (MessageType = 'Original' AND TransactionType = 'Deposit') THEN 23  "
                 // + " when (MessageType = 'Reversal'  AND TransactionType = 'Deposit') THEN 13  "
                 + " else 0 "
                 + "end "

                 + ",case " // DESCRIPTION
                 + " when (TransactionType = '10' AND MessageType = '0210') THEN 'Debit Txn'  "
                 + " when (TransactionType = '10' AND MessageType = '0420') THEN 'Reversal of Debit '  "
                 + " when (TransactionType = '20' AND MessageType = '0210') THEN 'Deposit Txn '  "
                 + " when (TransactionType = '20' AND MessageType = '0420') THEN 'Reversal of Deposit'  "
                 // + " when (MessageType = 'Original' AND TransactionType = 'Deposit') THEN 23  "
                 // + " when (MessageType = 'Reversal'  AND TransactionType = 'Deposit') THEN 13  "
                 + " else 'NOT DEFINED'  "
                 + "end "
                 + ",TransactionCurrencyCode " // Ccy
                 + ",CAST(TransactionAmount As decimal(18,2)) "
                 + ",ISNULL(STAN, 0 ) " // TRace ..SystemTraceAuditNumber 
                 + ",RIGHT('000000000000' + RRN, 12) " // RRN insert leading zeros 

                   + ", Case " // Auth Number 
                  + " WHEN AUTHNUM = '000000' then '' "
                  + " Else ISNULL(AUTHNUM, '') "
                  + " end"

                  // + ",ISNULL(AuthNum, '' ) " // AuthNum
                  //+ ",ISNULL(RIGHT('000000' + AuthNum, 6), '' ) " // AuthNum
                  //  + ",ISNULL(AuthNum, '' ) " // AuthNum
                  + ",case " // Terminal Id
                 + " WHEN Left(TerminalId,3)='" + PRX + "' THEN right (TerminalId,8)  "
                 + " else TerminalId "
                 + "end "
                     // 5484 4600
                     + ",case " // Card ID
                 + " WHEN left([PAN], 6) = '526402' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                  + " WHEN left([PAN], 6) = '544460' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                   + " WHEN left([PAN], 6) = '529532' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                    + " WHEN left([PAN], 6) = '981804' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                     + " WHEN left([PAN], 6) = '517582' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                      + " WHEN left([PAN], 6) = '519001' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                       + " WHEN left([PAN], 6) = '531156' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                        + " WHEN left([PAN], 5) = '55787' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                         + " WHEN left([PAN], 8) = '50780309' THEN ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '')  "
                         // For 8 
                         + " WHEN left([PAN], 8) = '50780311' THEN ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "
                          + " WHEN left([PAN], 8) = '50780315' THEN ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "
                           + " WHEN left([PAN], 8) = '50780342' THEN ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "
                            + " WHEN left([PAN], 8) = '50780343' THEN ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "
                 + " else ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '')  "
                 + "end "
                   //  + ",  right (TerminalId,8) " // Terminal Id
                   //  + ", ISNULL(left([PAN], 8) + '****' + right([PAN], 4), '') "  // CARD  
                   //+ " , [PAN] "

                   + ",case " // Response code
                 + " WHEN ResponseCode = '00' THEN '0'  "
                 + " WHEN left(pan, 8) = '54844600' and ProcessingCode = '210000'and TransactionAmount <> '0'and IssuerBank = 'Banque du Caire' AND ResponseCode = '72' THEN '0' "
                 + " else ResponseCode "
                 + "end "
                  //   + " , '0' " // Response code

                  + ", @LoadedAtRMCycle"
                 + ", @Operator"

                 + ",case " // Origin 
                 + " WHEN Left(TerminalId,3)='" + PRX + "' THEN '1'  "
                 + " else '8' "
                 + "end "

                 + ",case " // Destination
                 + " WHEN Left(TerminalId,3) ='" + PRX + "' THEN '8'  "
                 + " else '1' "
                 + "end "
                 + ", cast(LocalDateTime as Date) "
                 + ", ProcessingDate "
                 //+ ", SettlementDate "
                 //     + ", CAST('20' + SUBSTRING(LocalDateTime, 7, 2)+ SUBSTRING(LocalDateTime, 4, 2) + SUBSTRING(LocalDateTime, 1, 2) + ' ' + SUBSTRING(LocalDateTime, 10, 5) AS date) " // NET DATE
                 //+ ", CAST(SettlementDate as DATE) " // SET_DATE
                 //+ ",  CAST('20' + SUBSTRING(SettlementDate, 7, 2)+ SUBSTRING(SettlementDate, 4, 2) + SUBSTRING(SettlementDate, 1, 2) AS date) " // SET_DATE
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                 // GET Only the valid ones
                 // + " WHERE (ResponseCode = '00' And Cast(TransactionAmount as decimal(18,2))> 0 ) "
                 + " WHERE (Cast(TransactionAmount as decimal(18,2))> 0 ) AND ResponseCode = '00' "
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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For MEEZA_GLOBAL" + Environment.NewLine
                                 + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    stpErrorText = stpErrorText + "Cancel During MEEZA BDC ATMS insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in MEEZA_GLOBAL_LCL with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);




            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "
          + " SET "
           + " [MatchingCateg] = t2.[MatchingCateg] " // BDC 277 or BDC278 
                                                      //+ ", [TransType] = t2.[TransType] "
                                                      // + " ,[TransDate] = t2.[TransDate] "

           //+ " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
            + " ,[TXNDEST] = t2.[TXNDEST] "
            + ",Card_Encrypted = t2.Card_Encrypted "
            + ",CAP_DATE = t2.CAP_DATE "
          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON  "
          + "  t1.RRNumber = t2.RRNumber "
          // + " AND t1.TerminalId = t2.TerminalId "
          //+ " AND t1.TransType = t2.TransType "
          + " AND t1.TransAmt = t2.TransAmt "
          + " AND t1.TransDate = t2.TransDate  "
          //+ " AND t1.AUTHNUM = t2.AUTHNUM "

          + " WHERE  "
          + " (t1.Processed = 0 AND t1.MatchingCateg <> 'BDC279' )   "
          + " AND (t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals') OR (t2.Processed=1 AND t2.Mask = '01') OR (t2.Processed=1 AND t2.Mask = '011')  ) "
          + " AND  (t2.MatchingCateg = '"
              + PRX + "277' OR t2.MatchingCateg = '" + PRX + "278' ) "

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




            // DATE 2024-11-27 Dina Said That all with 548446 is TELDA and do not exist in IST and Flex
            // 54844608

            //// They are not in IST .... So we insert them not to show unmatched 
            //// INSERT NEW RECORDS IN IST
            //// THESE ARE RECORDS COMING FROM BIN 
            //// 54844600****4316 //54844600 AND 54844608
            //// WE have set it in Category BDC278


            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]"
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
                 + ",[AUTHNUM]"
                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ",[Comment] "
                        + ",[EXTERNAL_DATE] "
                        + " , [Net_TransDate] "
                              + ",[Card_Encrypted] "
                                 + ",[CAP_DATE] "
                                 + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
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
                  + ",case " // + ",[FullTraceNo] "
                 + " when TransType = 11 THEN '2000000' " // FULL TRACE No Starts with 2 
                 + " when TransType = 21 THEN '4000000' " // FULL TRACE No Starts with 4
                 + " when TransType = 23 THEN '2000000' " // FULL TRACE No Starts with 2 
                 + " when TransType = 13 THEN '4000000' " // FULL TRACE No Starts with 4
                 + " else '0' "
                 + "end "

                 + ",[AUTHNUM]"
                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ", 'TXN CREATED BY FORCE FROM MEEZA for BINs 548446 TELDA' etc "  //,[Comment]
                        + ",[EXTERNAL_DATE] "
                        + " , [Net_TransDate] "
                              + ",[Card_Encrypted] "
                                 + ",[CAP_DATE] "
                                 + ",[SET_DATE] "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "  // FROM 

                 + " WHERE  left(CardNumber, 6) in ( '548446', '440701','510493'"
                                                    + ",'520031', '553592' )"
                                                    + " "
                 // TESLA AND OTHERS 
                 //+ " AND [Comment] = '54844600 Marked as gone to IST' "
                 + " AND MatchingCateg in ( 'BDC277', 'BDC278' , 'BDC279') AND Processed = 0 AND ResponseCode ='0' " // leave it here ... do not move it
                 + " AND [Comment] = '' "
                 + " AND LoadedAtRMCycle = @LoadedAtRMCycle  ";

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For IST from Meeza" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_IST_TWIN_Second";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_Create Records From Meeza:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // Insert for switch tWIN 

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
                + ",[AUTHNUM]"
                + ",[TerminalId] "
                + ",[CardNumber] "
                + ",[AccNo] "

                + ",[ResponseCode] "
                + ",[LoadedAtRMCycle] "
                + ",[Operator] "
                  + ",[TXNSRC] "
                    + ",[TXNDEST] "
                    + ",[Comment] "
                       + ",[EXTERNAL_DATE] "
                       + " , [Net_TransDate] "
                             + ",[Card_Encrypted] "
                                + ",[CAP_DATE] "
                                + ",[SET_DATE] "
                + ") "
                + " SELECT "
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
                 + ",case " // + ",[FullTraceNo] "
                + " when TransType = 11 THEN '2000000' " // FULL TRACE No Starts with 2 
                + " when TransType = 21 THEN '4000000' " // FULL TRACE No Starts with 4
                + " when TransType = 23 THEN '2000000' " // FULL TRACE No Starts with 2 
                + " when TransType = 13 THEN '4000000' " // FULL TRACE No Starts with 4
                + " else '0' "
                + "end "

                + ",[AUTHNUM]"
                + ",[TerminalId] "
                + ",[CardNumber] "
                + ",[AccNo] "

                + ",[ResponseCode] "
                + ",[LoadedAtRMCycle] "
                + ",[Operator] "
                  + ",[TXNSRC] "
                    + ",[TXNDEST] "
                    + ", 'TXN CREATED BY FORCE FROM MEEZA for Terminal Id starts 888 etc' "  //,[Comment]
                       + ",[EXTERNAL_DATE] "
                       + " , [Net_TransDate] "
                             + ",[Card_Encrypted] "
                                + ",[CAP_DATE] "
                                + ",[SET_DATE] "

                + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "  // FROM 

                + " WHERE left(Terminalid, 3) = '888' "
                // TESLA AND OTHERS 
                //+ " AND [Comment] = '54844600 Marked as gone to IST' "
                + " AND MatchingCateg in ('BDC279') AND Processed = 0 AND ResponseCode ='0' " // leave it here ... do not move it
                + " AND [Comment] = '' "
                + " AND LoadedAtRMCycle = @LoadedAtRMCycle  ";

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For IST TWIN from Meeza " + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_IST_TWIN_Second";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_Create Records From Meeza:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            // 548446

            //// They are not in FLEXCUBE .... So we insert them not to show unmatched 
            //// INSERT NEW RECORDS IN FLEXCUBE
            //// THESE ARE RECORDS COMING FROM BIN 
            //// 54844608****4316 
            //// WE have set it in Category BDC277


            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Flexcube]"
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
                 + ",[AUTHNUM]"
                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ",[Comment] "
                        + ",[EXTERNAL_DATE] "
                        + " , [Net_TransDate] "
                              + ",[Card_Encrypted] "
                                 + ",[CAP_DATE] "
                                 + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
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
                  + ",case " // + ",[FullTraceNo] "
                 + " when TransType = 11 THEN '2000000' " // FULL TRACE No Starts with 2 
                 + " when TransType = 21 THEN '4000000' " // FULL TRACE No Starts with 4
                 + " when TransType = 23 THEN '2000000' " // FULL TRACE No Starts with 2 
                 + " when TransType = 13 THEN '4000000' " // FULL TRACE No Starts with 4
                 + " else '0' "
                 + "end "

                 + ",[AUTHNUM]"
                 + ",[TerminalId] "
                 + ",[CardNumber] "
                 + ",[AccNo] "

                 + ",[ResponseCode] "
                 + ",[LoadedAtRMCycle] "
                 + ",[Operator] "
                   + ",[TXNSRC] "
                     + ",[TXNDEST] "
                     + ", 'TXN CREATED BY FORCE FROM MEEZA for BIN 548446-TELDA' "  //,[Comment]
                        + ",[EXTERNAL_DATE] "
                        + " , [Net_TransDate] "
                              + ",[Card_Encrypted] "
                                 + ",[CAP_DATE] "
                                 + ",[SET_DATE] "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "  // FROM 

                  + " WHERE  left(CardNumber, 6) in ( '548446', '440701','510493'"
                                                    + ",'520031', '553592' )" // TESLA AND OTHERS 
                                                                              //+ " AND [Comment] = '54844608 Marked as gone to Flex' "
                 + " AND MatchingCateg in ( 'BDC277') AND Processed = 0 AND ResponseCode ='0' " // leave it here ... do not move it
                 + " AND [Comment] = '' "
                 + " AND LoadedAtRMCycle = @LoadedAtRMCycle  ";

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted For IST from Meeza" + Environment.NewLine
                                 + "..:.." + Counter.ToString());
                    }
                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At_IST_TWIN_Second";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }
            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_Create Records From Meeza:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);




            int U_Count = 0;
            // Update Comment 
            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "
                + " SET "
                  + " [Comment] = '548446XX Marked as gone to IST' "
                     //
                     + " WHERE  left(CardNumber, 6) in ( '548446') "
                       + " AND MatchingCateg in ('BDC277', 'BDC278') " // leave it here ... do not move it
                     + " AND LoadedAtRMCycle = @LoadedAtRMCycle  "
                     + " AND [Comment] = '' "
                //+ " AND [Comment] <> 'Reversals' "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds

                        U_Count = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            // FIND AND UPDATE TRANSACTIONS WITH REVERSALS FOR THE INSERTED RECORDS
            // FOR IST
            string From = "IST";
            string WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] ";

            HandleReversals_IST(WFile, InOriginFileName, InReconcCycleNo);

            if (ErrorFound == true) return;

            //
            // UPDATE CATEGORY AND Date seconds with IST TWIN
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "
          + " SET"
           + " [MatchingCateg] = t2.[MatchingCateg] " // Here is BDC279
                                                      //+ ", [TransType] = t2.[TransType] "
                                                      //+ " ,[TransDate] = t2.[TransDate] "
                                                      // + " ,[TransCurr] = t2.[TransCurr] "
           + " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
            + " ,[TXNDEST] = t2.[TXNDEST] "
              + ",Card_Encrypted = t2.Card_Encrypted "
            + ",CAP_DATE = t2.CAP_DATE "

          + ",AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON "
          // "t1.TraceNo = t2.TraceNo "
          + "  t1.RRNumber = t2.RRNumber "
          //  + " AND t1.TerminalId = t2.TerminalId "
          //+ " AND t1.TransType = t2.TransType "
          + " AND t1.TransAmt = t2.TransAmt "
          //+ " AND t1.Net_TransDate = t2.Net_TransDate  "
          //+ " AND t1.AUTHNUM = t2.AUTHNUM  "

          + " WHERE  "
          + " (t1.Processed = 0 AND t1.MatchingCateg = 'BDC279' And t1.AmtFileBToFileC = 0 ) "
          + " AND (t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals') ) AND  t2.MatchingCateg = '" + PRX + "279' "
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
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] B "
          + " ON A.TerminalId = B.TerminalId "
          //+ " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Minutes_Date = B.Minutes_Date "
          + " AND A.AUTHNUM = B.AUTHNUM "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND ("
          + " (A.TransType = 11 AND B.TransType = 21) OR (A.TransType = 13 AND B.TransType = 23) "
          //+ " OR (A.TransType = 21 AND B.TransType = 11) OR (A.TransType = 23 AND B.TransType = 13) " NO NEED FOR THIS - Creates double 
          + ") "
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
                        System.Windows.Forms.MessageBox.Show("REVERSAL ENTRIES Inserted for MEEZA_GLOBAL_LCL" + Environment.NewLine
                                  + "..:.." + Counter.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_GLOBAL_LCL";
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
            //FileId = "Flexcube";
            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL]";
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
                    stpErrorText = stpErrorText + "Cancel At _Reversals_MEEZA_GLOBAL_LCL";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "MEEZA_GLOBAL_LCL Finishes with SUCCESS.." + "\r\n";

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

            string InDelimiter = "tap"; // It Is tap for POS


            // Check if file is a Windows and Not Unix. 


            //   stpErrorText = "";
            stpReferenceCode = "";

            // CHECK THE FILE
            string TableId_Bulk = "[RRDM_Reconciliation_ITMX].[dbo].[BULK_MEEZA_POS]";

            // MAKE UNIX oR DELIMETER VALIDATION 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Del = new RRDM_BULK_IST_AndOthers_Records_ALL_2();

            bool Correct = Del.CheckIfWindowsOrUnix_ATMS(InFullPath);

            //Correct = true; // WE MAKE TRUE TILL MARIOS HELP 

            if (Correct == false)
            {
                MessageBox.Show("The file: " + InFullPath + Environment.NewLine
                    + " IS not a Windows File. It cannnot be accepted "
                    );
                return;
            }
            if (Environment.MachineName == "RRDM-PANICOS")
            {
                Correct = Del.FindIfDelimiterErrorsInInputTable_ATMS(TableId_Bulk, InFullPath, InDelimiter, recconConnString);


                if (Correct == false)
                {

                    //string OldString = "faisal,giza";
                    //string NewString = "faisal giza"; // No comma

                    //  Del.ReplaceValueInGivenTextFile(InFullPath, OldString, NewString);
                }
            }


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

            // Bulk insert the tap txt file to this temporary table

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
                     + ",[Comment] "
                     + " ,[EXTERNAL_DATE] "
                     + ",[Net_TransDate] "
                     + ",[SET_DATE] "
                 + ") "
                 + " SELECT "
                 + "@OriginFile"
                 + " ,@Origin"

                  //+ " ,'" + PRX + "273' " // MEEZA_POS - It can be BDC272(Debit) or BDC273(Prepaid) 
                  + " ,CASE " // LEAVE IT 
                 + " WHEN Left(PAN, 8 )= '50780309' THEN '" + PRX + "272' "  // Goes to FLEXCUBE
                + " WHEN Left(PAN, 6 )= '544460' THEN '" + PRX + "272' "
                + " WHEN Left(PAN, 6 )= '557876' THEN '" + PRX + "272' "
                + " WHEN Left(PAN, 6 )= '526402' THEN '" + PRX + "272' "
                 + " else '" + PRX + "273' "
                 + " END "
                 + ", @TerminalType"
                //+ ", CAST([AuthorizationDate] As datetime) " // Transaction Date without minutes 
                // + ", CAST([AcquirerDate] As datetime) " // Transaction Date without minutes 
                + " , ISNULL(Cast(AuthorizationDate as DateTime), '1900-01-01') "
                 + ",case "
                 + " when (MessageType = 'Original' AND ProcessingCode <> '200000') THEN 11  "
                 + " when (MessageType = 'Reversal') THEN 21  "
                 + " When (MessageType = 'Original' AND ProcessingCode = '200000' ) THEN 23  " // Credit Adjustment 
                 + " else 0 "
                 + "end "

                  // + ",case " // 7
                  //+ " when ([P0025_1] = '' AND PCODE <>'200000') THEN 11  "
                  //+ " when [P0025_1] = 'R' THEN 21  "
                  //+ " when ([P0025_1] = '' AND PCODE = '200000') THEN 21  " // Credit Adjustment
                  //+ " else 0 "
                  //+ "end "

                  + ",case "
                 + " when (ProcessingCode = '200000') THEN 'POS CREDIT ADJUSTMENT'  "
                 + " else 'MEEZA POS TXN'  "
                 + "end "
                 //  + ", 'MEEZA POS TXN' "
                 + ",TransactionCurrencyCode"
                 + ",CAST(TransactionAmount As decimal(18,2)) "
                 + ", 0 " // 
                          //  + ",RRN  " // RRN + Authorisation Number
                   + ",RIGHT('000000000000'+ISNULL(rtrim(RRN),''),12) " // FULL REFERENCE NUMBER
                 + ", AuthorizationNumber"
                 + ",  Left(TerminalId,8) " // Terminal Id
                + " ,case" // LEAVE IT HERE 
                + " WHEN Left(PAN, 6 )= '507808' THEN Left(PAN, 6 )+ '**' + '****' + right([PAN], 4) " // 
                 + " else ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "
                 + " end "
                 + " ,case" // ProcessingCode for Resp Code
                 + " WHEN   ISNULL(ProcessingCode, '')  = '3000' Then '0'  " // 14
                 + " WHEN   ISNULL(ProcessingCode, '')  = '200000' Then '200000'  " // 14
                 + " else '0' "
                 + " end "
                  //  + ", '0'" // Response Code
                  //  + ", ProcessingCode " // Response Code or processing code

                  + ", @LoadedAtRMCycle"
                 + ", @Operator"
                 + ",'8' "   // Origin Meeza
                  + " ,CASE " // LEAVE IT 
                 + " WHEN Left(PAN, 8 )= '50780309' THEN '1' "  // Goes to FLEXCUBE
                + " WHEN Left(PAN, 6 )= '544460' THEN '1' "
                + " WHEN Left(PAN, 6 )= '557876' THEN '1' "
                + " WHEN Left(PAN, 6 )= '526402' THEN '1' "
                 + " else '12' "
                 + " END "
                          //   + ",'12' " // Destination prepaid - it can be flexcube

                          + " ,case" // LEAVE IT HERE GOES TEMPORALY TO COMMENT
                + " WHEN Left(PAN, 5 )= '50780' THEN Left(PAN, 8 ) + '****' + right([PAN], 4) " // 
                 + " else ISNULL(left([PAN], 6) + '******' + right([PAN], 4), '') "
                 + " end "
                  //+ " ,"
                  + ", CAST(AuthorizationDate As datetime) " // For external date 
                                                             //  + ", CAST([AcquirerDate] As date) "
                  + ", CAST([AuthorizationDate] As date) "

                   //  + ", DATEADD(DAY, -1, Cast(ClearingSettlementDate as DATE)) " // DATE OF ARRIVING
                   + ", CAST(ClearingSettlementDate as DATE) "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
            + " WHERE AuthorizationDate is not Null ";
            // GET Them ALL with Authorisation number
            //  FILE Contains all Authorised 
            //  + " WHERE ISNULL(left([PAN], 6),'') not in ('440700', '440701', '510493', '512397', '520031', '525550','553593') ";

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
            // UPDATE based on RRN and Auth Number 
            //
            // DateTime TempNewCutOff = WCut_Off_Date.AddDays(-12);

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
          + " SET"
            + " [MatchingCateg] = t2.[MatchingCateg] "
            + " ,[TransDate] = t2.[TransDate] " // Leave here - although it could be different we borrow it for consistency with IST
           + " ,[TransCurr] = t2.[TransCurr] "
           + " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
           + " ,[TXNDEST] = t2.[TXNDEST] "
             // + " ,[Net_TransDate] = t2.[Net_TransDate] " // Leave here - although it could be different we bollow it for consistency with IST
             // + " ,[EXTERNAL_DATE] = t2.[EXTERNAL_DATE] "
             + " ,CAP_DATE = t2.CAP_DATE " // We already inserted the clearing date
             + " ,Card_Encrypted = t2.Card_Encrypted " // Leave it here needed at Matching 


          + " ,AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "
               // + "  t1.RRNumber = t2.RRNumber "
               //+ " AND t1.AUTHNUM = t2.AUTHNUM "
               //+ " AND t1.TransType = t2.TransType "

               + "  t1.RRNumber = t2.RRNumber "
            //  + " AND t1.AUTHNUM = t2.AUTHNUM "
          + " AND t1.TransType = t2.TransType "
            // + " AND t1.TerminalId = t2.TerminalId " // leave it here 
            //+ " AND t1.TransAmt = t2.TransAmt " leave as comment
            //+ " AND t1.CardNumber = t2.CardNumber "
            + " AND t1.AUTHNUM = t2.AUTHNUM "

          //  + " AND t1.[Net_TransDate] = t2.[Net_TransDate]"

          + " WHERE  "
          //+ "(t1.Processed = 0  AND t1.MatchingCateg = '" + PRX + "273') " // As set during loading
          + "(t1.Processed = 0  ) " // As set during loading
          + " AND "
        //  + "(t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus ) )"
        + "(t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' ) )"
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
                        // cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

            //
            // UPDATE based on Auth No
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
          + " SET"
            + " [MatchingCateg] = t2.[MatchingCateg] "
           // + " ,[RRNumber] = t2.[RRNumber] " // Force it to be the same // Sometimes there is difference of RRN in Meeza
           // + " ,[TransDate] = t2.[TransDate] " // Leave here - although it could be different we borrow it for consistency with IST
           + " ,[TransCurr] = t2.[TransCurr] "
           + " ,[TraceNo] = t2.[TraceNo] "
           + " ,[AccNo] = t2.[AccNo] "
           + " ,[TXNDEST] = t2.[TXNDEST] "
           //  + " ,[Net_TransDate] = t2.[Net_TransDate] " // Leave here - although it could be different we bollow it for consistency with IST
           + " ,Card_Encrypted = t2.Card_Encrypted " // Leave it here needed at Matching 
           + " ,CAP_DATE = t2.CAP_DATE " // We already inserted the clearing date
                                         //+ " ,[Net_TransDate] = t2.[Net_TransDate] "

          + " ,AmtFileBToFileC = t2.SeqNo " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

          + " ON "

               + " t1.AUTHNUM = t2.AUTHNUM "

          + " AND t1.TransType = t2.TransType "
          // + " AND t1.TerminalId = t2.TerminalId " // leave it here 
          //+ " AND t1.TransAmt = t2.TransAmt " leave as comment
          //  + " AND t1.AUTHNUM = t2.AUTHNUM "
          //+ " AND t1.CardNumber = t2.CardNumber "
          //  + " AND t1.[Net_TransDate] = t2.[Net_TransDate]"

          + " WHERE  "
          //+ "(t1.Processed = 0  AND t1.MatchingCateg = '" + PRX + "273') " // As set during loading
          + "(t1.Processed = 0  AND t1.MatchingCateg in ('" + PRX + "272', '" + PRX + "273') ) " // As set during loading
          + " AND t1.AmtFileBToFileC = 0 "
          + " AND "
        //  + "(t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' AND t2.Net_TransDate>@Net_Minus ) )"
        + "(t2.Processed = 0 OR (t2.Processed=1 AND t2.Comment = 'Reversals' ) )"
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
                        //  cmd.Parameters.AddWithValue("@Net_Minus", TempNewCutOff); // Get The previous days 
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

            RecordFound = false;

            // Create records in IST
            // These are missing from IST
            // ONLY BDC273 .... so only IST
            // OR  BDC272 .... for IST AND Flex

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
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
                   + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
                   + " AND ( ISNULL(left([CardNumber], 6),'') in ('440700', '440701', '510493', '512397', '520031', '525550','553593','548446')" +
                   " OR ISNULL(left([Comment], 8),'') in ('50780309','50780836' ,'50780311') )" +
                   " "
                   //50780309 * ***4392
                   ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted for IST from Meeza POS" + Environment.NewLine
                               + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At _IST_Insert_From Meeza POS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }


            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_08_IST_Records_loaded..Records:.Meeza POS." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Flexcube]"
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
                   + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
                   + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle "
                   + " AND " +
                   " ISNULL(left([Comment], 8),'') in ('50780309') " +
                   " "
                   //50780309 * ***4392
                   ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

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
                        System.Windows.Forms.MessageBox.Show("Records Inserted for IST from Meeza POS" + Environment.NewLine
                               + "..:.." + stpLineCount.ToString());
                    }

                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();
                    stpErrorText = stpErrorText + "Cancel At _IST_Insert_From Meeza POS";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);

                    return;
                }


            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Stage_08_IST_Records_loaded..Records:.Meeza POS." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //
            // Update Card number with the kept one. 
            //

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
                + " SET "
                + " [CardNumber] = Comment " // Here the 8 digit card was kept
                + " , Comment = '' "
                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle"

                 + " WHERE  "
               + "  Processed = 0 and Comment <> '' " // Non Processed and comment 
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
        //// CREATE CIT_EXCEL_TO BANK RECORDS
        /////****************
        public void InsertRecords_GTL_CIT_EXCEL_TO_BANK(string InOriginFileName, string InFullPath
                                           , int InReconcCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            stpLineCount = 0;

            stpReturnCode = 0;

            stpReferenceCode = "";

            int Counter = 0;


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

                    stpErrorText = stpErrorText + "Cancel During CIT EXCEL BULK Truncate";
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

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //

            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                     + " SET Total_replenishment = replace(Total_replenishment, '\"', '')  "
                     + " ,Total_Returned = replace(Total_Returned, '\"', '')  "
                     + " ,Total_Deposit_EGP = replace(Total_Deposit_EGP, '\"', '')  "

                     + " ,Total_Deposit_USD = replace(Total_Deposit_USD, '\"', '')  "
                      + " ,EUR_Total_Deposit = replace(EUR_Total_Deposit, '\"', '')  "
                       + " ,GBP_Total_Deposit_ = replace(GBP_Total_Deposit_, '\"', '')  "
                        + " ,KWD_Total_Deposit = replace(KWD_Total_Deposit, '\"', '')  "
                         + " ,AED_Total_Deposit = replace(AED_Total_Deposit, '\"', '')  "
                         + " ,SAR_Total_Deposit = replace(SAR_Total_Deposit, '\"', '')  "


                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                      + "  SET  Total_replenishment = replace(Total_replenishment, ',', '')  "
                     + " ,Total_Returned = replace(Total_Returned, ',', '')  "
                     + " ,Total_Deposit_EGP = replace(Total_Deposit_EGP, ',', '')  "

                      + " ,Total_Deposit_USD = replace(Total_Deposit_USD,',', '')  "
                      + " ,EUR_Total_Deposit = replace(EUR_Total_Deposit, ',', '')  "
                       + " ,GBP_Total_Deposit_ = replace(GBP_Total_Deposit_, ',', '')  "
                        + " ,KWD_Total_Deposit = replace(KWD_Total_Deposit, ',', '')  "
                         + " ,AED_Total_Deposit = replace(AED_Total_Deposit, ',', '')  "
                         + " ,SAR_Total_Deposit = replace(SAR_Total_Deposit, ',', '')  "

                         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
                      + "  SET  CIT = replace(CIT, ' ', '')  "
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

            LastSeqNo = 0;

            SQLCmd = "SELECT ISNULL(MAX(SeqNo), 0) AS MaxSeqNo "
               + "FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
               + " WITH(NOLOCK) "
               + "";

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



            // KEEP ALL 
            ErrorFound = false;
            ErrorOutput = "";

            SQLCmd = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL" //  We insert in the second IST
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
                    stpErrorText = stpErrorText + "Cancel During Creation of CIT Excel ALL ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To CIT EXCEL Bulk Finishes..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //       
            // CREATE CIT FILE
            //
            // [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK]


            //,ISNULL(try_convert(datetime, (LEFT([FromDt],10)+' ' +TIME_FROM),103), '1900-01-01')

            SQLCmd = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
                   + "( "
                   + " [OriginTextFile] " // Para // 1
                   + " ,[ExcelDate] " // Para //2.1
                   + " ,[LoadedDtTm] " // Para //2.2
                    + " ,[BulkRecordId] " //3

                   + " ,[CIT_ID] " //4
                   + " ,[AtmNo] "  //5
                   + " ,[GROUP_ATMS] " //6

                   + ",[ReplCycleStartDate] " //7
                   + ",[ReplCycleEndDate] " //8

                   + ",[CIT_Total_Replenished] " //9
                   + ",[CIT_Total_Returned] " //10
                   + ",[CIT_Total_Deposit_Local_Ccy] " //11

                   + ",[LoadedAtRMCycle] " //12
                   + ") "
                   + " SELECT "
                   + "@OriginTextFile" //1
                   + " ,@ExcelDate" //2.1
                   + " ,@LoadedDtTm" //2.2

                   + " , SeqNo " // 3

                   //+ " , CIT " //4
                   + " , CASE " // Translate // 4
                             + " when CIT = 'Target'  THEN '5000'  "
                             + " when CIT = 'TIBA'  THEN '6000'  "
                             + " when CIT = 'SWAT'  THEN '7000'  "
                             + " else 'Not Def' "
                             + "end "
                   + " , RIGHT('00000000'+ISNULL(rtrim(ID),''),8)  " //5
                   + ", GROUP_ATMS" //6

                   + " , ISNULL(try_convert(datetime, (LEFT([FromDt], 10) + ' ' + TIME_FROM), 103), '1900-01-01') " // 7
                   + " , ISNULL(try_convert(datetime, (LEFT([ToDt], 10) + ' ' + TIME_TO), 103), '1900-01-01') " // 8

                   + ", cast(Total_Replenishment as Decimal(18, 2)) " //9 
                   + ", cast(Total_Returned as Decimal(18, 2)) " // 10
                   + ", cast(Total_Deposit_EGP as Decimal(18, 2)) " //11


                  + ", @LoadedAtRMCycle" //12

                   + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_ALL"
                   + " WHERE SeqNo > " + LastSeqNo;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginTextFile", WFullPath_01.Trim());
                        cmd.Parameters.AddWithValue("@ExcelDate", FileDATEresult);
                        cmd.Parameters.AddWithValue("@LoadedDtTm", DateTime.Now);

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

                    stpErrorText = stpErrorText + "Cancel During CIT EXCEL File Creation insert of records";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Insert in CIT_Excel with records.." + stpLineCount.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            // HERE YOU INSERT THE GROUP OF ATMS AND THE USER OWNER 

            //
            // UPDATE OTHER INFO FROM UsersAtmTable
            //
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
          + " SET  "
          + " GroupOfAtmsRRDM = t2.GroupOfAtms "
          + ",UserId = t2.UserId "
         
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] t1 "
          + " INNER JOIN [ATMS].[dbo].[UsersAtmTable] t2"

          + " ON t1.AtmNo = t2.AtmNo "

          + " WHERE  t1.LoadedAtRMCycle =  @LoadedAtRMCycle " //ONLY THESES LOADED AT THIS CYCLE
          + " OR (t1.GroupOfAtmsRRDM = 0 AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )" // Or in Previous 
           + " OR (t1.UserId = '' AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )"; // Or in Previous 

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
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
                    stpErrorText = stpErrorText + "Cancel During Updating of Group and user owner ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            //
            // UPDATE CIT_ID_RRDM FROM ATMS
            //
            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] "
          + " SET  "
          + " CIT_ID_RRDM = t2.CitId "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo].[CIT_EXCEL_TO_BANK] t1 "
          + " INNER JOIN [ATMS].[dbo].[ATMsFields] t2"

          + " ON t1.AtmNo = t2.AtmNo "

          + " WHERE  t1.LoadedAtRMCycle =  @LoadedAtRMCycle " // ONLY THESES LOADED AT THIS CYCLE 
          + " OR (t1.CIT_ID_RRDM = '' AND t1.LoadedAtRMCycle <> @LoadedAtRMCycle )"; // Or in Previous 

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);
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
                    stpErrorText = stpErrorText + "Cancel During Updating of Group and user owner ";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            // UPDATE TEMPORARILY THE VALID ONES 

            //SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
            //         + " SET [Valid_Entry] = 1 "
            //         + "  where Cast(ReplCycleStartDate as Date ) = '2025-03-06' "
            //         + " AND Cast(ReplCycleEndDate as Date ) = '2025-03-07' "
            //        + " ";
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName
                    + " SET [Valid_Entry] = 1 "
                    + "  WHERE GroupOfAtmsRRDM <> 0 AND UserId <> '' "
                      + " AND CIT_ID_RRDM <> '' AND CIT_ID = CIT_ID_RRDM "
                        //+"AND CIT_ID_RRDM <> '1000' "
                        + " AND Valid_Entry = 0 " // take only the ones with no valid yet 
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

                    stpErrorText = stpErrorText + "Cancel During Valid Upadting";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

            bool IsRecycle = false;

            int WBulkRecordId;

            int WSeqNo;
            string WAtmNo;
            DateTime WReplCycleStartDate;
            DateTime WReplCycleEndDate;
            decimal WCIT_Total_Replenished;
            decimal WCIT_Total_Returned;
            decimal WCIT_Total_Deposit_Local_Ccy;
            string WGROUP_ATMS;
            int WTransType;
            bool WJournal;

            int WLoadedAtRMCycle ;
            int WReplCycle ;

            int WGroupOfAtmsRRDM ;

            decimal SwitchDebits;
            decimal SwitchDeposits;

            decimal JournalDebits;
            decimal JournalDeposits;
            // Recycle 
            decimal Remains_Switch;
            decimal Remains_Journal;
            decimal Remains_CIT;
            // Not Recycle 
            decimal Remains_Switch_Returned;
            decimal Remains_Switch_Deposits;

            decimal Remains_Journal_Returned;
            decimal Remains_Journal_Deposits;

            bool ItHasForex = false;

            decimal TotalPresenter;

            int CounterMathedRecycle = 0;
            int CounterUnMatchedRecycle = 0;

            int CounterMatchedOther_DR = 0;
            int CounterUnMatchedOther_DR = 0;
            int CounterMatchedOther_CR = 0;
            int CounterUnMatchedOther_CR = 0;

            int I;

            string WSelectionCriteria;

            //DateTime LimitDate = new DateTime(2025, 03, 05);
            // Create first the IST transactions for ATMs
            // In File 
            string WTableId = "[RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS]";

            // CHECK and Update Matching with Replenishement date

            WTableId = " [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName;
            WSelectionCriteria = " WHERE Valid_Entry = 1 "; // No selection 
            Ce.ReadRecordsFrom_Excel_DataByDatesParameters(WTableId, WSelectionCriteria);

            bool IsAutoActive = false; 

            I = 0;

            try
            {
                while (I <= (Ce.DataTableAllFields.Rows.Count - 1))
                {

                    WSeqNo = (int)Ce.DataTableAllFields.Rows[I]["SeqNo"];
                    WAtmNo = (string)Ce.DataTableAllFields.Rows[I]["AtmNo"];
                    WReplCycleStartDate = (DateTime)Ce.DataTableAllFields.Rows[I]["ReplCycleStartDate"];
                    WReplCycleEndDate = (DateTime)Ce.DataTableAllFields.Rows[I]["ReplCycleEndDate"];
            
                    Ac.ReadAtm(WAtmNo);
                    if (Ac.RecordFound == true)
                    {
                        //if (Ac.CitId != "7000")
                        //{
                        //    Ac.TypeOfRepl = "10";
                        //    Ac.CitId = "7000";
                        //    Ac.UpdateATM(WAtmNo);
                        //}
                        //if (Ac.AtmsReplGroup == 0 || Ac.AtmReplUserId == null)
                        //{
                        //    Ac.AtmsReplGroup = 111;
                        //    Ac.AtmsReconcGroup = 111;
                        //    Ac.AtmReplUserId = "ahm.osman";
                        //    Ac.UpdateATM(WAtmNo);
                        //}
                    }
                    else
                    {
                        //MessageBox.Show("ATM.." + WAtmNo + "..Not Found in Database");
                    }

                    //Ce.GroupOfAtmsRRDM = Ac.AtmsReplGroup;
                    //Ce.UserId = Ac.AtmReplUserId; 
                    //// UPDATE 
                    //Ce.Update_User_AND_GROUP(WSeqNo);

                    Ta.ReadReplCycles_FOR_ATM_And_DATE(WAtmNo, WReplCycleEndDate.Date);

                    if (Ta.RecordFound == true)
                    {
                        Ce.SesDtTimeStart = Ta.SesDtTimeStart;
                        Ce.SesDtTimeEnd = Ta.SesDtTimeEnd;

                        Ce.ProcessMode = Ta.ProcessMode;
                        Ce.ReplCycle = Ta.SesNo;
                        Ce.Journal = true;
                        Ce.JournalForced = false;

                        Ce.UpdateAtmOfDatesFromJournal(WSeqNo);
                    }


                    I++; // Read Next entry of the table 
                }
            }
            catch (Exception ex)
            {
                // conn.Close();

                stpErrorText = stpErrorText + "Cancel During Reading the valid ";
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                CatchDetails(ex);
                return;
            }

            // UPDATE EXCEL WITH DIFFERENT INFORMATION INCLUDING FINANCIAL 

            WTableId = " [RRDM_Reconciliation_ITMX].[dbo]." + InOriginFileName;
            WSelectionCriteria = " WHERE Valid_Entry = 1 ";
            Ce.ReadRecordsFrom_Excel_DataBySelectionCriteria(WTableId, WSelectionCriteria);


            I = 0;

            try
            {
                while (I <= (Ce.DataTableAllFields.Rows.Count - 1))
                {

                    WSeqNo = (int)Ce.DataTableAllFields.Rows[I]["SeqNo"];
                    WBulkRecordId = (int)Ce.DataTableAllFields.Rows[I]["BulkRecordId"];
                    WAtmNo = (string)Ce.DataTableAllFields.Rows[I]["AtmNo"];
                    WReplCycleStartDate = (DateTime)Ce.DataTableAllFields.Rows[I]["ReplCycleStartDate"];
                    WReplCycleEndDate = (DateTime)Ce.DataTableAllFields.Rows[I]["ReplCycleEndDate"];
                    WCIT_Total_Replenished = (decimal)Ce.DataTableAllFields.Rows[I]["CIT_Total_Replenished"];
                    WCIT_Total_Returned = (decimal)Ce.DataTableAllFields.Rows[I]["CIT_Total_Returned"];
                    WCIT_Total_Deposit_Local_Ccy = (decimal)Ce.DataTableAllFields.Rows[I]["CIT_Total_Deposit_Local_Ccy"];

                    WJournal = (bool)Ce.DataTableAllFields.Rows[I]["Journal"];
                    WGROUP_ATMS = (string)Ce.DataTableAllFields.Rows[I]["GROUP_ATMS"];

                    WLoadedAtRMCycle = (int)Ce.DataTableAllFields.Rows[I]["LoadedAtRMCycle"];
                    WReplCycle = (int)Ce.DataTableAllFields.Rows[I]["ReplCycle"];

                    WGroupOfAtmsRRDM = (int)Ce.DataTableAllFields.Rows[I]["GroupOfAtmsRRDM"];

                    if (WReplCycleStartDate == NullPastDate || WReplCycleEndDate == NullPastDate)
                    {
                        string Filter = " WHERE SeqNo="+ WSeqNo;
                        Ce.Read_CIT_Excel_Table_BySelectionCriteria(Filter); 

                        Ce.STATUS = "01";

                        Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo); 
                        I++; // Read Next entry of the table 
                        continue; 
                    }
                    
                   // Check if it has forex. 
                    ItHasForex = Ce.Check_IF_FOREX_IN_BULK(WBulkRecordId);

                    if (WGROUP_ATMS == "R")
                    {
                        IsRecycle = true;
                    }
                    else
                    {
                        IsRecycle = false;
                    }

                    // Ce.Read

                    WTransType = 11;
                    SwitchDebits = Ce.GetTotalsFrom_SWITCH_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                            , WReplCycleEndDate, WTransType);
                    WTransType = 23;
                    SwitchDeposits = Ce.GetTotalsFrom_SWITCH_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                            , WReplCycleEndDate, WTransType);
                    TotalPresenter = Ce.GetTotalsFrom_Master_For_Presented(WAtmNo, WReplCycleStartDate
                                                                                            , WReplCycleEndDate);
                    //decimal JournalDebits;
                    //decimal JournalDeposits;
                    JournalDebits = 0;
                    JournalDeposits = 0;
                    Remains_Journal = 0;

                    if (WJournal == true)
                    {
                        WTransType = 11;
                        JournalDebits = Ce.GetTotalsFrom_JOURNAL_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                            , WReplCycleEndDate, WTransType);
                        WTransType = 23;
                        JournalDeposits = Ce.GetTotalsFrom_JOURNAL_By_Dates(WAtmNo, WReplCycleStartDate
                                                                                            , WReplCycleEndDate, WTransType);
                    }


                    //We have 5 groups(A, B & C) For withdrawal ATMS
                    //(D & R) for (deposit & recycle) ATMS
                    // For Recycling we apply : Remaining = (Open Bal+ Deposits)- Withdrawals
                    // so this must be equal to what CIT input for cassettes + Deposits

                    if (IsRecycle == true)
                    {
                        // If Recycle we add all deposits on Opening balance and we subtract all Withdrawls
                        // This gives us the remains for both the Cassettes + Deposits 
                        // Remains_IST_DR = (WCIT_Total_Replenished + (TotalCredit_IST + Mpa.TotalCredit)) - Corrected_DR_IST;
                        Remains_Switch = WCIT_Total_Replenished + SwitchDeposits - SwitchDebits;
                        if (WJournal == true)
                        {
                            Remains_Journal = WCIT_Total_Replenished + JournalDeposits - JournalDebits;
                        }

                        Remains_CIT = WCIT_Total_Returned + WCIT_Total_Deposit_Local_Ccy;
                        // REMAINS CANNOT BE DEFINED FROM IST FOR DEPOSITS
                        if (Remains_CIT == Remains_Switch)
                        {
                            CounterMathedRecycle = CounterMathedRecycle + 1;

                            Ce.SWITCH_Total_Returned = Remains_Switch;
                            Ce.SWITCH_Total_Deposit_Local_Ccy = 0;

                            // Journal
                            Ce.JNL_SM_Total_Returned = Remains_Journal;
                            Ce.JNL_SM_Deposit_Local_Ccy = 0;

                            Ce.OverFound_Amt_Cassettes = 0;
                            Ce.ShortFound_Amt_Cassettes = 0;
                            Ce.PresentedErrors = TotalPresenter;
                            Ce.CreatedDate = DateTime.Now;

                            Ce.IsAuto = false;

                            if (ItHasForex == true || TotalPresenter > 0 || WJournal == false)
                            {
                                Ce.STATUS = "01";
                            }
                            else
                            {
                                if (IsAutoActive == true)
                                {
                                    Ce.STATUS = "04";
                                    Ce.IsAuto = true;
                                    UpdateReplCycle(WAtmNo, WReplCycle, WLoadedAtRMCycle, "RECATMS-" + WGroupOfAtmsRRDM.ToString());
                                }
                                else
                                {
                                    Ce.IsAuto = true; // We keep it here to evaluate how many could have been auto
                                    Ce.STATUS = "01";
                                }
                                         
                            }

                            Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);

                        }
                        else
                        {
                            //CounterUnMatchedRecycle = CounterUnMatchedRecycle + 1;

                            Ce.SWITCH_Total_Returned = Remains_Switch;
                            Ce.SWITCH_Total_Deposit_Local_Ccy = 0;

                            Ce.JNL_SM_Total_Returned = Remains_Journal;
                            Ce.JNL_SM_Deposit_Local_Ccy = 0;

                            if (Remains_CIT > Remains_Switch)
                            {
                                Ce.OverFound_Amt_Cassettes = Remains_CIT - Remains_Switch;
                            }
                            else
                            {
                                Ce.ShortFound_Amt_Cassettes = Remains_Switch - Remains_CIT;
                            }

                            Ce.PresentedErrors = TotalPresenter;

                            Ce.CreatedDate = DateTime.Now;
                            Ce.IsAuto = false;

                            Ce.STATUS = "01";

                            Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);
                        }

                    }
                    else
                    {
                        // Not Recycle 
                        Remains_Journal_Returned = 0;
                        Remains_Journal_Deposits = 0;

                        Remains_Switch_Returned = WCIT_Total_Replenished - SwitchDebits;
                        Remains_Switch_Deposits = SwitchDeposits;
                        if (WJournal == true)
                        {
                            Remains_Journal_Returned = WCIT_Total_Replenished - JournalDebits;
                            Remains_Journal_Deposits = JournalDeposits;
                        }


                        // Cassettes

                        Ce.SWITCH_Total_Returned = Remains_Switch_Returned;
                        Ce.SWITCH_Total_Deposit_Local_Ccy = Remains_Switch_Deposits;

                        Ce.JNL_SM_Total_Returned = Remains_Journal_Returned;
                        Ce.JNL_SM_Deposit_Local_Ccy = Remains_Journal_Deposits;

                        Ce.OverFound_Amt_Cassettes = 0;
                        Ce.ShortFound_Amt_Cassettes = 0;

                        if (WCIT_Total_Returned > Remains_Switch_Returned)
                        {
                            Ce.OverFound_Amt_Cassettes = WCIT_Total_Returned - Remains_Switch_Returned;
                        }
                        if (Remains_Switch_Returned > WCIT_Total_Returned)
                        {
                            Ce.ShortFound_Amt_Cassettes = Remains_Switch_Returned - WCIT_Total_Returned;
                        }

                        // Deposits 

                        Ce.OverFound_Amt_Deposits = 0;
                        Ce.ShortFound_Amt_Deposits = 0;

                        if (WCIT_Total_Deposit_Local_Ccy > Remains_Switch_Deposits)
                        {
                            Ce.OverFound_Amt_Deposits = WCIT_Total_Deposit_Local_Ccy - Remains_Switch_Deposits;
                        }
                        if (Remains_Switch_Deposits > WCIT_Total_Deposit_Local_Ccy)
                        {
                            Ce.ShortFound_Amt_Deposits = Remains_Switch_Deposits - WCIT_Total_Deposit_Local_Ccy;
                        }

                        Ce.PresentedErrors = TotalPresenter;
                        Ce.IsAuto = false;

                        // Update Process mode

                        if ((WCIT_Total_Returned + WCIT_Total_Deposit_Local_Ccy)
                             == (Remains_Switch_Returned + Remains_Switch_Deposits)
                             & ItHasForex == false & TotalPresenter == 0 & WJournal == true
                             )
                        {
                            if (IsAutoActive == true)
                            {
                                Ce.STATUS = "04";
                                Ce.IsAuto = true;
                                UpdateReplCycle(WAtmNo, WReplCycle, WLoadedAtRMCycle, "RECATMS-" + WGroupOfAtmsRRDM.ToString());
                            }
                            else
                            {
                                Ce.IsAuto = true; // We keep it here to evaluate how many could have been auto
                                Ce.STATUS = "01";
                            }
                           
                        }
                        else
                        {
                            Ce.STATUS = "01";
                        }

                        Ce.CreatedDate = DateTime.Now;

                        Ce.UpdateAtmDuringMatchingOfCitExcel(WSeqNo);

                    }

                    I++; // Read Next entry of the table 
                }
            }
            catch (Exception ex)
            {
                // conn.Close();

                stpErrorText = stpErrorText + "Cancel During Reading the valid ";
                stpReturnCode = -1;

                stpReferenceCode = stpErrorText;
                CatchDetails(ex);
                return;
            }


            //return; 


            stpErrorText += DateTime.Now + "_" + "CIT EXCEL Finishes with SUCCESS.." + "\r\n";

            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // Return CODE
            stpReturnCode = 0;
        }
        private void UpdateReplCycle(string WAtmNo, int WSesNo, int WRMCycle, string WReconcCategoryId)
        {
            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
            Ta.ReadSessionsStatusTraces(WAtmNo, WSesNo);
            Ta.Repl1.DiffRepl = false;
            Ta.ReconcAtRMCycle = WRMCycle;
            Ta.Recon1.RecFinDtTm = DateTime.Now;
            Ta.Stats1.NoOfCheques = 1;
            Ta.UpdateSessionsStatusTraces(WAtmNo, WSesNo);
            //Ta.UpdateTracesFinishReplOrReconc(WAtmNo, WSesNo, WSignedId, Mode);
            Ta.UpdateTracesFinishRepl_From_Form51(WAtmNo, WSesNo, "Controller",
                "By_System", WReconcCategoryId);
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

                    stpErrorText = stpErrorText + "Cancel During BDC_NCR_FOREX_BULK_Records_TRUNCATE";
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

                    stpErrorText = stpErrorText + "Cancel During BDC_NCR_FOREX_BULK_Records_CHILD_TRUNCATE";
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

                    stpErrorText = stpErrorText + "Cancel During BDC_NCR_FOREX_BULK_Records Bulk Insert";
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

                    stpErrorText = stpErrorText + "Cancel During BDC_NCR_FOREX_BULK_Records CHILD CREATION";

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

                stpErrorText = stpErrorText + "Cancel During BDC_NCR_FOREX_BULK_Records CHILD CREATION_2";

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
                    stpErrorText = stpErrorText + "Cancel During Creation of Flexcube";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "Stage_03_Insert_To Forex Child ALL s..Records:.." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

            //       
            // CREATE(Insert) NCR_FOREX from BULK FOR DEPOSITS
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
            // CREATE(Insert) NCR_FOREX from BULK FOR Withdrawls
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
                 + ", '818' " // LOCAL CURRENCY
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
            // AND COPY NCR FOREX IN IST
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
                    // CORRECT 
                    //****************************************************
                    RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                    string WFileName = "CIT_EXCEL_TO_BANK";
                    string TargetDB = "[RRDM_Reconciliation_ITMX]";
                    Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                    if (Cv.RecordFound == true)
                    {
                        
                        //
                        // CREATE WORKING TABLE FOR IST Vs CIT Transactions
                        //
                        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

                        string WTableId = "[RRDM_Reconciliation_ITMX].[dbo].[CIT_SWITCH_TXNS]";
                        // MessageBox.Show("We Include all previous cycles for testing peurposes. Correct when needed  ");
                        Ce.Insert_SWITCH_TXNS_For_ATMS_FOREX(WTableId, InReconcCycleNo, LastSeqNo);
                        //****************************************************
                        Mgt.DeleteDuplicatesInCIT_FILE(InReconcCycleNo);
                        //Mgt.DeleteDuplicatesInCIFILE(InReconcCycleNo);
                    }

                    stpErrorText += DateTime.Now + "_" + "Records were copied to IST.." + La.Counter.ToString() + "\r\n";
                    Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                }

                // UPDATE THEM AFTER WERE INPUT IN IST 
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
                 + ",[AUTHNUM] "
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

                 + ", '818' "
                 + " ,CAST([TransAmt] As decimal(18,2)) + CAST([CustomerFees] As decimal(18,2))  "
                 + ", 0 " // There is no trace number -- set it to zero
                          // + ",Cast([RectNo] as int) " // TRACE NUMBER
                 + ",[DrRefNo]" // THIS THE AUTHORISATION NUMBER that Goes to RRNumber 
                 + ",[BillType_Code]+'_'+ [FawryRefNo] " // FULL TRACE NUMBER 
                  + ",[DrRefNo]" //  AUTHORISATION NUMBER 
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
                 + " WHERE TermNo <> 'BDCWallet' "
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
                 + " , [AmtFileBToFileC]"
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
                 + ",case "  // TXN Code
                 + " when Cast (SETTLEMENT_AMOUNT as decimal(18,2)) > 0 THEN 11  "
                 //  + " when (TYP = 'Reversal' AND [TRANSACTIO] = 'Withdrawl') THEN 21 "
                 + " else 0 "
                 + "end "
                 //  in place of Transction Description 
                 + " ,case"
                 + " WHEN ACQISS = 'A' THEN 'BDC ATM Withdrawl' " // All Outgoing
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN 'BDC Card Used for Withdrawl-DEBIT' " // All DEBIT
                 + " WHEN ACQISS = 'I' AND (LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532') THEN 'BDC Card Used for Withdrawl-PREPAID' " // All PREPAID
                 + " else '" + PRX + "999' "
                 + " end "

                 + ", ISNULL(LEFT([SETTLEMENT_CODE],3), '') "
                  + " ,case " // GET the setlement amount 
                 + " WHEN ACQISS = 'A' THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // All Outgoing [TXNSRC]
                 + " WHEN ACQISS = 'I' THEN CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // ALL Incoming
                 + " else 0 "
                 + " end "
                 + " , CAST([AMOUNT] As decimal(18,2)) "

                 //+ ", ISNULL(LEFT([ACQ_CURRENCY_CODE],3), '') "
                 // + " ,case"
                 //+ " WHEN ACQISS = 'A' THEN CAST([AMOUNT] As decimal(18,2)) " // All Outgoing [TXNSRC]
                 //+ " WHEN ACQISS = 'I' THEN CAST([AMOUNT] As decimal(18,2)) " // ALL Incoming
                 //+ " else 0 "
                 //+ " end "
                 + ",[TRACE] "
                 + ",RIGHT('000000000000'+ISNULL(rtrim(REFNUM),''),12) " // FULL REFERENCE NUMBER
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
            //**************************************************************************************
            // 19/11/2022
            // UPdate Category BDC236 -  master fawry 
            // Turn BDC235 to BDC236
            // *************************************************************************************
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
          + " SET"
            + " MatchingCateg = t2.MatchingCateg " //  BDC236 
            + ",AmtFileBToFileC = 99.99 " // indication that this record is updated 

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] t1 "
          + " INNER JOIN "
          + " [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

          + " ON "
          + "  t1.RRNumber = t2.RRNumber "
          + " AND t1.TerminalId = t2.TerminalId "
          + " AND t1.TransAmt = t2.TransAmt "
          + " WHERE  "
          + " t1.Processed = 0  AND t1.MatchingCateg in ('" + PRX + "235') " // 
          + " AND t2.Processed = 0 AND t2.MatchingCateg = 'BDC236'  "
          ;
            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo); // Get The previous days 
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

                    stpErrorText = stpErrorText + "Cancel During UPDATE TWIN For Master Settlement ";
                    CatchDetails(ex);
                    return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATE  Master Card for fawry..." + Counter.ToString() + "\r\n";
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
                          + " ,case " // BAT_NUM 
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
            // replace(SETTL_AMOUNT, 'CR"', '')
            SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET SETTL_AMOUNT = replace(SETTL_AMOUNT, '.', '')"
            //         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
            //         + " SET AMOUNT = replace(AMOUNT, '-', '')"
                     + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET SETTL_AMOUNT = replace(SETTL_AMOUNT, ',', '')"
                       + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET SETTL_AMOUNT = replace(SETTL_AMOUNT, 'CR', '')"
                        + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                     + " SET SETTL_AMOUNT = replace(SETTL_AMOUNT, 'DR', '')"
                    //         + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                    //         + " SET AMOUNT = replace(AMOUNT, '.', '')"
                    //          + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                    //         + " SET SETTL_AMOUNT = TRIM('.' FROM SETTL_AMOUNT)  "
                    //+ " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                    //    + " SET SETTL_AMOUNT = TRIM('CR' FROM SETTL_AMOUNT)  "
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
                           + " ,[YYYY] =  "
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

                        + " when substring(Date_Time, 3, 3) = 'OCT' THEN  @YEAR1 "
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

                        cmd.Parameters.AddWithValue("@YEAR2", FileDATEresult.Year); // File year 
                        cmd.Parameters.AddWithValue("@YEAR1", FileDATEresult.Year - 1);
                        //if (DateTime.Now.Month == 01 || DateTime.Now.Month == 02)
                        //{
                        //    cmd.Parameters.AddWithValue("@YEAR1", "2019");
                        //    cmd.Parameters.AddWithValue("@YEAR2", "2020");
                        //}
                        //else
                        //{
                        //    cmd.Parameters.AddWithValue("@YEAR1", "2020");
                        //    cmd.Parameters.AddWithValue("@YEAR2", "2020");
                        //}

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
                 + ",[TransCurr] "
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
                      + " , [SET_DATE] "

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

                    + ",case "  // Currency
                     + " WHEN  ISNULL(CUR,'') = 'USD' THEN '840'   "
                      + " WHEN  ISNULL(CUR,'') = 'EGP' THEN '818'   "
                       + " WHEN  ISNULL(CUR,'') = 'KWD' THEN '414'   "
                      + " WHEN  ISNULL(CUR,'') = 'GBP' THEN '826'   "
                       + " WHEN  ISNULL(CUR,'') = 'AED' THEN '784'   "
                      + " WHEN  ISNULL(CUR,'') = 'SAR' THEN '682'   "
                       + " WHEN  ISNULL(CUR,'') = 'EUR' THEN '978'   "
                     + " else ISNULL(CUR,'') "
                     + "end "
                   //+ ", ISNULL(CUR,'') "
                   + ",CAST([SETTL_AMOUNT] As decimal(18,2)) /100"  // Amount
                 + ",[RRN] "
                 + ",[TRACE] "

                  + ",case "
                     + " WHEN  LEN(CARD) = 16 THEN ISNULL(left([CARD], 6) + '******' + right(RTRIM(LTRIM(CARD)), 4), '')   "
                     + " WHEN  LEN(CARD) = 19 THEN ISNULL(left([CARD], 6) + '*********' + right(RTRIM(LTRIM(CARD)), 4), '')   "
                     + " else CARD  "
                     + "end "
                     // RTRIM(LTRIM(CARD))
                     //    + ", ISNULL(left([CARD], 6) + '******' + right([CARD], 4), '') "  // CARD  
                     // + ",[CARD] "  // 
                     + ",case "
                     + " when [RSP_CD] = '00' THEN '0'  "
                     + " else [RSP_CD] "
                     + "end "
                 + ", @LoadedAtRMCycle"
                  + ", @Operator"

                 + " ,'1'" // Origin
                 + " ,'4'" // Destination
                           //                 0017_Dispute on BDC
                           //2502_Uknown
                           //2004_Failure TXN to other Banks customer
                           //0013_Dispute rejected

                   + ",case " // REAS_CODE
                 + " when ISNULL(Reas_CODE,'') = '0013' THEN 'Reason:0013_Dispute rejected'  " // 
                  + " when ISNULL(Reas_CODE,'') = '0017' THEN 'Reason:0017_Dispute on BDC'  " // 
                      + " when ISNULL(Reas_CODE,'') = '2004' THEN 'Reason:2004_Failure TXN to other Bank Customers'  " // 
                       + " when ISNULL(Reas_CODE,'') = '2502' THEN 'Reason:2502_Unknown'  " // 
                 + " else '' "
                 + "end "

                 + " , CAST(ISNULL(try_convert(datetime, (DD + '/' + MM + '/' + YYYY) + ' ' "
                            + "+ TRAN_TIME, 103), '1900-01-01') as Date)"

                  + " , @FileDATEresult "

                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName + "_2"
                 + " WHERE  "
                + " CAST([SETTL_AMOUNT] As decimal(18,2)) > 0 "
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
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", WCut_Off_Date);
                        cmd.Parameters.AddWithValue("@FileDATEresult", FileDATEresult);
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
            // DELETE FROM VISA CARD txns before testing period
            //
            int DelCount;

            //
            // UPDATE CARD Number and Origin
            //

            SQLCmd =
          " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[VISA_CARD] "
          + " SET "
          + " TransCurr = t2.TransCurr "
          + " ,TerminalId = t2.TerminalId "
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
                 + " , Left(Branchname,20) " // In place of Terminal ID. 

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
                 + " , Left(Reference, 30) " // in place of RRN 
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
                 //+ " , 'BDC231' Flexcube" // All Master POS
                 //    , 'BDC233' Prepaid    // They have two destinations 
                 //                 // a) Flexcube and Prepaid ( Destination = 1 and 12)

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
                 + " when ([P0025_1] = '' AND PCODE = '200000') THEN 23  " // Credit Adjustment
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
                         // + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1252'   " // MUST be examined (may be change db character set to UTF8)
                         //                                                   //+ " ,ROWs_PER_BATCH=15000 "
                         // + " ,ROWs_PER_BATCH=15000, FIELDTERMINATOR = ',' "
                         ////+ ",FIELDTERMINATOR = '\t'"
                         //+ " ,ROWTERMINATOR = '\r\n' )"
                         // ;
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

                    stpErrorText = stpErrorText + "Cancel During Bulk INSERT";
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
                 + ",[AUTHNUM] " // 11.2
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
                 + " WHEN ( LEFT([PAN_MASK],6) = '529532' OR LEFT([PAN_MASK],6) = '517582' ) THEN '" + PRX + "233' " // All PREPAID
                 + " else '" + PRX + "000' " // 
                                             //For Debit card Pin:
                                             //526402 * *****
                                             //544460 * *****
                                             //557876 * *****
                                             //     For Prepaid Pin:
                                             //529532 * *****
                                             //517582 * *****



                             //   + " WHEN ACQISS = 'A' THEN '" + PRX + "235' " // All Outgoing
                             //+ " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '526402' OR LEFT([PAN],6) = '544460' OR LEFT([PAN],6) = '557876' ) THEN '" + PRX + "230' " // All DEBIT
                             //+ " WHEN ACQISS = 'I' AND ( LEFT([PAN],6) = '517582' OR LEFT([PAN],6) = '529532' ) THEN '" + PRX + "232' " // All PREPAID
                             + " end "
                 //+ " , 'BDC231' Flexcube" // All Master POS
                 //    , 'BDC233' Prepaid    // They have two destinations 
                 //                 // a) Flexcube and Prepaid ( Destination = 1 and 12)

                 + ", ISNULL(TERMID, '') " // 4
                 + ", @TerminalType " // 5

                   + ", convert(datetime, LOCAL_DATE, 104) " // Convert from dd/mm/yyyy
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

                  // + ", ISNULL(LEFT([SETTLEMENT_CODE],3), '') "// Ccy // 9
                  //+ ",TRY_CAST([SETTLEMENT_AMOUNT] As decimal(18,2)) " // 10
                  + ", ISNULL(LEFT(ACQ_CURRENCY_CODE,3), '') "// Ccy // 9
                                                              //
                  + ",TRY_CAST(AMOUNT As decimal(18,2)) " // 10

                 + ",ISNULL(AUTHNUM, '') " // RRN// 11
                 + ",ISNULL(AUTHNUM, '') " // AUTHORISATION  // 11.2
                                           // FULL Trace // 12 
                 + ", ISNULL('ACCEPTOR_ID:' + ACCEPTOR_ID + '\r\n ACCEPTORNAME:'+ [ACCEPTORNAME] + '\r\n REFERENCE:'+ [ACQ_REFDATA] , '') "  // in Place Of Full Trace

                 + ",ISNULL([PAN_MASK], '') "  // 13
                     + " ,case" // PCODE
                 + " WHEN   ISNULL(PCODE, '')  = '3000' Then '0'  " // 14
                  + " WHEN   ISNULL(PCODE, '')  = '180000' Then '0'  " // 14
                  + " WHEN   ISNULL(PCODE, '')  = '182000' Then '0'  " // 14
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

                     + " ,case" // PCODE
                 + " WHEN   ISNULL(PCODE, '')  = '3000' Then 'Resp = 3000'  " // 19
                  + " WHEN   ISNULL(PCODE, '')  = '180000' Then 'Resp = 180000'  " // 19
                  + " WHEN   ISNULL(PCODE, '')  = '182000' Then 'Resp = 182000'  " // 19
                 + " else ISNULL(PCODE, '') "
                 + " end "
                 // + " ,'Comment' " // for the comment // 19
                 + " , ISNULL(PAN, '') " // for the comment // 20

                 + ", ISNULL([ACCEPTOR_ID], '') " // ACCEPTOR_ID
                 + ", ISNULL([ACCEPTORNAME], '') " // ACCEPTORNAME
                 + ",  convert(date, SETTLEMENT_DATE, 104) "
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InOriginFileName
               + " WHERE  "
              + " ( LEFT([PAN_MASK],6) = '526402' OR LEFT([PAN_MASK],6) = '544460' OR LEFT([PAN_MASK],6) = '557876')  " // All DEBIT
                 + " OR ( LEFT([PAN_MASK],6) = '529532' OR LEFT([PAN_MASK],6) = '517582' ) "  // All PREPAID
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

            // IF TRANSACTIONS GREATED THAN CUTOFF DATE
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            //string FileId = "MASTER_POS";

            string TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";

            SqlString = "SELECT Count(*)  As TotalInvalid"
                  + " FROM " + TableId
                  + " WHERE Net_TransDate > @CutOffDate "
                  + "";

            // OPEN Connection to assist individual updatings
            //SqlConnection conn2 = new SqlConnection(ATMSconnectionString);
            //conn2.Open();

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try

                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@CutOffDate", WCut_Off_Date);
                        //cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Counter = (int)rdr["TotalInvalid"];

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
                    ErrorFound = true;
                    stpErrorText = stpErrorText + "Cancel checking MASTER_POS if dates > cut off";

                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;

                    CatchDetails(ex);
                    return;
                }

            if (Counter > 0)
            {
                MessageBox.Show("Master POS:There are transactions with transaction date > than Cutoff " + Environment.NewLine
                    + "Number is" + Counter.ToString() + Environment.NewLine
                    + "These will be deleted during Matching"
                    );

            }

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
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";

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
        // DELETE RECORDS TO SET STARTING DATE
        //
        public void DeleteRecordsToSetStartingPoint(string InOperator, DateTime InDate, DateTime InCut_Off_Date, int InRMCycleNo, int InMode)
        {
            // CLEAR TABLE FOR TESTING
            //
            //           
            // InMode =  1 means it is coming from date that is before the first cycle 
            // InMode = 2 means it is coming from HST date 
            bool ApplyRange = false;
            // DateTime FixedDate = new DateTime(2025, 01, 18);
            int DelCount;
            // if (InDate == FixedDate)
            //   {
            // Apply range 
            //     ApplyRange = true;
            //  }

            // DELETE FROM IST THE UNWANTED
            // THESE ARE 0-POS Purchase Txns originated from ATMs that must be deleted 
            // Delete also all starting with 

            string SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " Where (TransDescr = '0-POS Purchase' and TXNSRC = '1' ) OR MatchingCateg = 'Not_Def'  "
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

                        cmd.CommandTimeout = 750;  // seconds

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

            // DELETE FROM IST THE UNWANTED
            // CATEGORY 231 is not ACTIVE YET

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                + " Where MatchingCateg in ('BDC231' , 'BDC233') "
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

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails_2(ex, "Origin:231");
                    // CatchDetails(ex);
                }

            // DELETE FROM IST THE UNWANTED
            // CATEGORY 231 is not ACTIVE YET

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
                + " Where MatchingCateg = 'BDC231'  "
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

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails_2(ex, "Origin:forex231");
                    // CatchDetails(ex);
                }

            //
            // DELETE FROM POOL the BDC299 which is the result of toxic journals 
            // we subtract 11 days 
            //
            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE MatchingCateg  = 'BDC299' AND Net_TransDate  < @Cut_Off_Date  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date.AddDays(-11));

                        cmd.CommandTimeout = 750;  // seconds

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

            DelCount = 0;

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
              + " WHERE MatchingCateg  = 'BDC237' AND Net_TransDate  < @Cut_Off_Date  "
              + "  "
              ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date.AddDays(-11));

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:Meeza03");
                }


            // DELETE FROM BULK IST THE UNWANTED 
            // GREATER THAN LAST 15 days

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_Switch_IST_Txns_ALL_2] "
                + "  WHERE LoadedAtRMCycle <= @LoadedAtRMCycle   "
                + " "
                ;

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycleNo - 15); // Subtract 10 from current cycle number

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails_2(ex, "Origin:786");
                    // CatchDetails(ex);
                }

            // START DELETING OLD AND NOT PROCESSED
            // 
            // WORK AROUND AFTER LONG PRODUCTION INTERACTION. 

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
               + " WHERE Net_TransDate  < @TransDate AND Processed = 0   "
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

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:901");
                }


            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] "
             + " WHERE Net_TransDate  < @TransDate AND Processed = 0  "
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

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:903");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
              + " WHERE Net_TransDate  < @TransDate  AND Processed = 0  "
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

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:904");
                }

            // DELETE FROM POOL 
            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs]"
               + " WHERE Net_TransDate  < @TransDate  AND IsMatchingDone = 0  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:905");
                }




            //
            // DELETE logically Meeza Transactions
            //
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId;
            string OccurId;



            RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            string WFileName;
            string TargetDB;

            WFileName = "MEEZA_GLOBAL_LCL";
            TargetDB = "[RRDM_Reconciliation_ITMX]";
            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

            if (Cv.RecordFound == true)
            {
                // File Exist

                SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo]." + WFileName
                      + " WHERE Net_TransDate  < @TransDate   AND Processed = 0  ";

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@TransDate", InDate);

                            cmd.CommandTimeout = 750;  // seconds

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
                        CatchDetails_2(ex, "Origin:906");
                        //return;
                    }
            }
            // DELETE MEEZA_POS 
            SQLCmd = " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
             + " WHERE Net_TransDate  < @TransDate  AND Processed = 0  "
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

                    CatchDetails_2(ex, "Origin:907");
                }
            //
            //
            //           UPDATE MEEZA_POS txns before testing or History period
            //           This will be reported as unmatched 
            //

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_POS] "
                     + " SET "
                     + " Comment = 'OLD_Transaction' "
                      + " WHERE TransDate  < @TransDate AND Processed = 0  ";
            ;


            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        cmd.CommandTimeout = 750;  // seconds

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:Meeza POS 908");
                }


            //            DELETE FROM MASTER CARD txns before testing period

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
                 + " WHERE Net_TransDate  < @TransDate   AND Processed = 0  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        cmd.CommandTimeout = 750;  // seconds

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
                    CatchDetails_2(ex, "Origin:909Master");
                    //return;
                }

            // DELETE FROM Master Card POS anything before
            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
               + " WHERE Net_TransDate  < @TransDate  AND Processed = 0  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        cmd.CommandTimeout = 750;  // seconds

                        DelCount = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:910");
                }

            //            DELETE FROM MASTER CARD txns After CUT Off date 

            SQLCmd = " DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                 + " WHERE Net_TransDate  > @TransDate   AND Processed = 0  ";

            using (SqlConnection conn = new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransDate", InCut_Off_Date);

                        cmd.CommandTimeout = 750;  // seconds

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
                    CatchDetails_2(ex, "Origin:911MasterPos");
                    //return;
                }
            //
            //
            //           UPDATE master_POS txns before testing or History period
            //           This will be reported as unmatched 
            //

            SQLCmd = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] "
                     + " SET "
                     + " Comment = 'OLD_Transaction' "
                      + " WHERE TransDate  < @TransDate AND Processed = 0  ";

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

                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex, "Origin:MasterPOS 912");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Credit_Card]"
             + " WHERE Net_TransDate  < @TransDate  AND Processed = 0  ";

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

                    CatchDetails_2(ex, "Origin:913");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[VISA_Card]"
                + " WHERE Net_TransDate  < @TransDate   AND Processed = 0  ";

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

                    CatchDetails_2(ex, "Origin:914");
                }

            SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[FAWRY] "
               + " WHERE Net_TransDate  < @TransDate AND Processed = 0  ";

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

                    CatchDetails_2(ex, "Origin:915");
                }




            //DelCount = 0;

            //SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL] "
            //  + " WHERE TransDate  < @TransDate  AND MatchingCateg in ('BDC277','BDC278','BDC279') "
            //  + "  "
            //  ;

            //using (SqlConnection conn = new SqlConnection(recconConnString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SQLCmd, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@TransDate", MeezaNewVersionDt);

            //            cmd.CommandTimeout = 350;  // seconds

            //            DelCount = cmd.ExecuteNonQuery();
            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails_2(ex, "Origin:Meeza04");
            //    }


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
                     + "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
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
                //MessageBox.Show("Special Accounts Transactions Updated as processed:.." + Counter.ToString());

            }



            //
            // DELETE logically Cardless Transactions
            //
            // IN THE PARAMETER IS NO not YES

            ParId = "934";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES") // 
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

                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cardless Transactions for ATMs Updated as processed:.." + Counter.ToString());

                }


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
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cardless Transactions for IST Updated as processed:.." + Counter.ToString());
                }


            }



            //
            // DELETE logically Fawry Transactions
            //
            // THE PARAMETER HERE IS NO

            ParId = "935";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                // PARAMETER IS NO SO IT DOESNT go in here 
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
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Fawry Transactions for IST Updated as processed:.." + Counter.ToString());

                }


                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
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

                if (Environment.UserInteractive)
                {

                    MessageBox.Show("Fawry Transactions for Flexcube Updated as processed:.." + Counter.ToString());

                }


            }

            //
            // DELETE logically Meeza Transactions
            // OR UPDATE ACCOUNT NUMBER FOR PRE PAID CARDS
            // The parameter is NO

            ParId = "937";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                // WE DO NOT DEACTIVATE MEEZA 
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
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Meeza Transactions for IST Updated as processed:.." + Counter.ToString());
                }


                SQLCmd =
                    "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Flexcube] "
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
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Meeza Transactions for Flexcube Updated as processed:.." + Counter.ToString());
                }



                //

            }

            // UPDATE ACCOUNT NUMBER FOR PRE PAID CARDS with Card
            // to be used at Action stage . 
            SQLCmd =
" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
+ "   SET [AccNumber] = Card_Encrypted "
+ "  WHERE MatchingCateg = '" + PRX + "203' "

//+ " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]  "
//+ "   SET [AccNo] = Card_Encrypted  "
//+ "  WHERE MatchingCateg = '" + PRX + "211'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "232'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg ='" + PRX + "231'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg ='" + PRX + "233'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL]  "
+ "   SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "277'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL]  "
+ "  SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "278'  "

+ "  UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MEEZA_GLOBAL_LCL]  "
+ "  SET [AccNo] = Card_Encrypted  "
+ "  WHERE MatchingCateg = '" + PRX + "279'  "
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
            // PARAMERE IS NO
            ParId = "936";
            OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                // IT DOESNT GO HERE
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

                if (Environment.UserInteractive)
                {
                    SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " WHERE LEFT(ltrim(rtrim([CardNumber])),6) = '123456' "
                      + "AND LEFT(ltrim(rtrim([TransDescr])),5)= 'FOREX' AND IsMatchingDone = 0 "
                    ;
                }



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
        }
        //
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

                //      SQLCmd =
                //    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]"
                //    + " SET TraceNo = t2.TraceNo "
                //      + "  , AccNo = t2.AccNo "
                //        + "  , Card_Encrypted = t2.Card_Encrypted "
                //    // + "  , CAP_DATE = t2.CAP_DATE "

                //    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
                //    + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

                //    + " ON t1.TransAmt = t2.TransAmt "
                //+ " AND t1.RRNumber = t2.RRNumber "
                //+ " AND t1.TerminalId = t2.TerminalId "
                //+ " AND t1.CardNumber  = t2.CardNumber  "
                //+ " AND t1.Net_TransDate = t2.Net_TransDate "
                //    //+ " WHERE  (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) "
                //    + " WHERE  (t1.Processed = 0) AND ((t2.Processed=0 AND T2.MatchingCateg = '" + PRX + "210') "
                //    + "OR (t2.Processed=1 AND T2.MatchingCateg = '" + PRX + "210' AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate)  "
                // + "      OR (t2.Processed = 1 AND T2.MatchingCateg = '" + PRX + "210' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                //; //  


                //      using (SqlConnection conn = new SqlConnection(recconConnString))
                //          try
                //          {
                //              conn.StatisticsEnabled = true;
                //              conn.Open();
                //              using (SqlCommand cmd =
                //                  new SqlCommand(SQLCmd, conn))
                //              {
                //                  cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
                //                  cmd.CommandTimeout = 750;  // seconds
                //                  Counter = cmd.ExecuteNonQuery();
                //                  var stats = conn.RetrieveStatistics();
                //                  commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                //              }
                //              // Close conn
                //              conn.StatisticsEnabled = false;
                //              conn.Close();
                //          }
                //          catch (Exception ex)
                //          {
                //              conn.StatisticsEnabled = false;
                //              conn.Close();

                //              stpErrorText = stpErrorText + "Cancel During Update with Trace..For..123_NET.1.";
                //              // Initialise counter 
                //              Counter = 0;
                //              CatchDetails_2(ex, "Origin:1801");
                //              //return;
                //          }

                //      stpErrorText += DateTime.Now + "_" + "Update with Trace..For..123_NET.1." + Counter.ToString() + "\r\n";
                //      stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                //      Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE in 123 
                // Against  TWIN
                // Initialise counter 
                //    Counter = 0;

                //    SQLCmd =
                //  " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET]"
                //  + " SET TraceNo = t2.TraceNo "
                //   + "  , Card_Encrypted = t2.Card_Encrypted "
                //     + "  , AccNo = t2.AccNo "
                //   // + "  , CAP_DATE = t2.CAP_DATE "

                //   + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] t1 "
                //  + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

                //  + " on  "
                //    + "  t1.Net_TransDate = t2.Net_TransDate "
                //     + " AND t1.TerminalId = t2.TerminalId "
                //  + " AND t1.TransAmt = t2.TransAmt "
                //+ " AND t1.RRNumber = t2.RRNumber "
                //  + " WHERE   "
                //    + "  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "215' )  "
                //    + "     AND ((T2.Processed = 0 AND t2.MatchingCateg = '" + PRX + "215' )  "
                //    + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "215' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                //                                                                     ; // 
                //    using (SqlConnection conn = new SqlConnection(recconConnString))
                //        try
                //        {
                //            conn.StatisticsEnabled = true;
                //            conn.Open();
                //            using (SqlCommand cmd =
                //                new SqlCommand(SQLCmd, conn))
                //            {
                //                cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
                //                cmd.CommandTimeout = 750;  // seconds
                //                Counter = cmd.ExecuteNonQuery();
                //                var stats = conn.RetrieveStatistics();
                //                commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                //            }
                //            // Close conn
                //            conn.StatisticsEnabled = false;

                //            conn.Close();
                //        }
                //        catch (Exception ex)
                //        {
                //            conn.StatisticsEnabled = false;

                //            conn.Close();

                //            stpErrorText = stpErrorText + "Cancel During Update with Trace..For..123_NET.2.";
                //            CatchDetails_2(ex, "Origin:1802");
                //            //return;
                //        }

                //    stpErrorText += DateTime.Now + "_" + "Update with Trace..For..123_NET.2." + Counter.ToString() + "\r\n";
                //    stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                //    Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


                //
                // UPDATE TRACE in Master Card
                //
                // Initialise counter 
                Counter = 0;

                //   SQLCmd =
                //" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
                // + " SET TraceNo = t2.TraceNo "
                //  + "  , Card_Encrypted = t2.Card_Encrypted "
                //    + "  , AccNo = t2.AccNo "
                //     + "  , CAP_DATE = t2.CAP_DATE "
                // //  + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV

                // + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] t1 "
                // + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

                // + " on t1.RRNumber = t2.RRNumber "
                // + " AND t1.TransAmt = t2.TransAmt"
                //   // + " WHERE  (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                //   + " WHERE  (t1.Processed = 0) "
                //   + " AND ((t2.Processed=0 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232')) "
                //          + "OR (t2.Processed=1 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232') AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate) "
                //              + " OR (t2.Processed = 1 AND T2.MatchingCateg in ('" + PRX + "230','" + PRX + "232') AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                //   ;


                //   using (SqlConnection conn = new SqlConnection(recconConnString))
                //       try
                //       {
                //           conn.StatisticsEnabled = true;
                //           conn.Open();
                //           using (SqlCommand cmd =
                //               new SqlCommand(SQLCmd, conn))
                //           {
                //               cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
                //               cmd.CommandTimeout = 350;
                //               Counter = cmd.ExecuteNonQuery();
                //               var stats = conn.RetrieveStatistics();
                //               commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                //           }
                //           // Close conn
                //           conn.StatisticsEnabled = false;

                //           conn.Close();
                //       }
                //       catch (Exception ex)
                //       {
                //           conn.StatisticsEnabled = false;

                //           conn.Close();

                //           stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..MASTER.1.";

                //           CatchDetails_2(ex, "Origin:1803");
                //           //return;
                //       }

                //   stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..MASTER.1." + Counter.ToString() + "\r\n";
                //   stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                //   Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                // UPDATE TRACE and other information in Master Card AGAINST TWIN
                // Initialise counter 
                Counter = 0;

                //   SQLCmd =
                //" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] "
                // + " SET TraceNo = t2.TraceNo "
                //    + "  , AccNo = t2.AccNo "
                //     //+ " , TransAmt = t2.TransAmt "
                //     + "  , Card_Encrypted = t2.Card_Encrypted "
                //      + "  , CAP_DATE = t2.CAP_DATE "
                //  // + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV

                //  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Master_Card] t1 "
                // + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWIN] t2"

                // + " on t1.RRNumber = t2.RRNumber "
                // + " AND t1.TerminalId = t2.TerminalId"
                //   //+ " WHERE  (t1.MatchingCateg = 'BDC235' AND t2.MatchingCateg = 'BDC235') AND (t1.Processed = 0 AND t2.Processed=0) OR ( t2.ResponseCode = '112' AND t2.Processed = 1) ";
                //   + " WHERE  (t1.Processed = 0 AND t1.MatchingCateg = '" + PRX + "235')  "
                //   + "AND ((t2.Processed=0 AND t2.MatchingCateg = '" + PRX + "235' )  "
                //              + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "235' AND t2.TransAmt>0"
                //              + " AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                //   ;


                //   using (SqlConnection conn = new SqlConnection(recconConnString))
                //       try
                //       {
                //           conn.StatisticsEnabled = true;
                //           conn.Open();
                //           using (SqlCommand cmd =
                //               new SqlCommand(SQLCmd, conn))
                //           {
                //               cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
                //               cmd.CommandTimeout = 350;
                //               Counter = cmd.ExecuteNonQuery();
                //               var stats = conn.RetrieveStatistics();
                //               commandExecutionTimeInMs = (long)stats["ExecutionTime"];



                //           }
                //           // Close conn
                //           conn.StatisticsEnabled = false;
                //           conn.Close();
                //       }
                //       catch (Exception ex)
                //       {
                //           conn.StatisticsEnabled = false;
                //           conn.Close();

                //           stpErrorText = stpErrorText + "Cancel During UPDATE TRACE ..For..MASTER.2.";

                //           CatchDetails_2(ex, "Origin:1804");
                //           //return;
                //       }

                //   stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..MASTER.2." + Counter.ToString() + "\r\n";
                //   stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";

                //   Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

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
                        CatchDetails_2(ex, "Origin:1805");
                        //return;
                    }

                stpErrorText += DateTime.Now + "_" + "UPDATE TRACE ..For..VISA.." + Counter.ToString() + "\r\n";
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
               //   + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV
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
                        CatchDetails_2(ex, "Origin:1807");
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

                //  SQLCmd =
                //" UPDATE [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]"
                //+ " SET AccNo = t2.AccNo "
                // + "  , CAP_DATE = t2.CAP_DATE "
                ////  + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV
                //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS] t1 "
                //+ " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

                //+ " ON "
                ////+ " t1.TransAmt = t2.TransAmt " // it could be different 
                //+ "  t1.RRNumber = t2.RRNumber "
                //+ " AND t1.CardNumber = t2.CardNumber "
                ////   + " AND t1.Net_TransDate = t2.Net_TransDate "

                //+ " WHERE  (t1.Processed = 0 ) "
                //     + "AND ((t2.Processed = 0 and T2.MatchingCateg = '" + PRX + "231' )  "
                //       + " OR (t2.Processed = 1 and T2.MatchingCateg = '" + PRX + "231' AND t2.Comment = 'Secret Accounts' AND t2.Net_TransDate>= @Net_TransDate ) "
                //             + " OR (t2.Processed = 1 AND t2.MatchingCateg = '" + PRX + "231' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                //  ;


                //TXNDEST = '1') AND TXN = '0-POS Purchase' THEN 'BDC231' " // All Incoming POS
                //       + " WHEN (TXNSRC = '5' AND TXNDEST = '12') THEN 'BDC233' " // All Incoming POS - Prepaid

                //using (SqlConnection conn = new SqlConnection(recconConnString))
                //    try
                //    {
                //        conn.StatisticsEnabled = true;
                //        conn.Open();
                //        using (SqlCommand cmd =
                //            new SqlCommand(SQLCmd, conn))
                //        {
                //            cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
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

                //        stpErrorText = stpErrorText + "Cancel During UPDATE For POS Acno Updated from IST...";
                //        CatchDetails_2(ex, "Origin:1808");
                //        //return;
                //    }

                //stpErrorText += DateTime.Now + "_" + "UPDATE For POS Acno Updated from IST..." + Counter.ToString() + "\r\n";
                //stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                //stpErrorText += "UPDATE Of TRACES FINISHES" + "\r\n";
                //Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

                //stpReturnCode = 0;
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
    + " ,AccNumber = t2.AccNo "
    + " ,TransDate = t2.TransDate" // Correct transaction date with seconds 
    + " ,TXNSRC = t2.TXNSRC "
    + " ,TXNDEST = t2.TXNDEST "
    + " ,RRNumber = t2.RRNumber "
    + " ,Card_Encrypted = t2.Card_Encrypted "
    + "  , CAP_DATE = t2.CAP_DATE "
    + "  , SET_DATE = t2.SET_DATE " // SET DATE SAME AS CAP Because Origin is ATMS
                                    // + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

    + " ON "
    + " t1.TerminalId = t2.TerminalId "
     + " AND t1.CardNumber = t2.CardNumber "
       + " AND t1.TraceNoWithNoEndZero = t2.TraceNo "
    + " AND t1.TransAmount = t2.TransAmt"
    + " AND t1.Minutes_Date = t2.Minutes_Date"

    //+ " AND t1.AccNumber = t2.AccNo "
    + " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' AND t1.MatchingCateg <> 'BDC209' ) "
     + "AND ((t2.Processed = 0 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81')  "
                              //  + " OR (t2.Processed = 1 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81' "
                              + " OR ( t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81' "
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
                    CatchDetails_2(ex, "Origin:1809");
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Category 1..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


            //
            // UPDATE Master with Matching Category ID Based on IST
            // Less MatchingCateg = 'BDC299'
            // Initialise counter 
            Counter = 0;

            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + " MatchingCateg = t2.MatchingCateg "
    + " ,AccNumber = t2.AccNo "
    + " ,CardNumber = t2.CardNumber "
    + " ,TransDate = t2.TransDate " // Correct transaction date with seconds 
    + " ,TXNSRC = t2.TXNSRC "
    + " ,TXNDEST = t2.TXNDEST "
    + " ,RRNumber = t2.RRNumber "
    + " ,Card_Encrypted = t2.Card_Encrypted "
    + "  , CAP_DATE = t2.CAP_DATE "
     + "  , SET_DATE = t2.SET_DATE " // SET DATE SAME AS CAP Because Origin is ATMS
                                     //+ "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

    + " ON "
    + " t1.TerminalId = t2.TerminalId "
       //  + " AND t1.CardNumber = t2.CardNumber "
       + " AND t1.TraceNoWithNoEndZero = t2.TraceNo "
    + " AND t1.TransAmount = t2.TransAmt"
    + " AND t1.Minutes_Date = t2.Minutes_Date"

    //+ " AND t1.AccNumber = t2.AccNo "
    + " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' AND t1.MatchingCateg <> 'BDC209' ) "
     + "AND ("
            + "(t2.Processed = 0 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81')  "
                           + "  OR (t2.Processed = 1  AND t2.TXNSRC= '1'  AND left(t2.TransDescr, 2) <> '81' AND t2.Net_TransDate>= @Net_TransDate )"
                              //   + "  OR (t2.Processed = 1 AND t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81'"
                              + "  OR ( t2.TXNSRC = '1' AND left(t2.TransDescr, 2) <> '81'"
                           + "          AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )"
                           + ") "
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

                    stpErrorText = stpErrorText + "Cancel At Master updated with Category 2...";
                    CatchDetails_2(ex, "Origin:1810");
                    //return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Category 2..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);


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
                string WSelectionCriteria = " WHERE MatchingCateg = 'BDC299' AND LoadedAtRMCycle=" + InRMCycleNo;
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
            Counter = 0;

            SQLCmd = "  UPDATE[RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                     + " SET AccNumber = RIGHT(ltrim(rtrim(AccNumber)),14) "
                      + " WHERE  IsMatchingDone = 0   "
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
                    stpErrorText = stpErrorText + "Cancel UPDATING of Acc for Master  ..";
                    stpReturnCode = -1;

                    stpReferenceCode = stpErrorText;
                    CatchDetails_2(ex, "Origin:1811");

                    //return;
                }

            stpErrorText += DateTime.Now + "_" + "UPDATING of Acc for Master  .." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //************************
            //************************
            // UPDATE MAXIMUM DATE ON THE Repl Sessions with -1
            //
            stpErrorText += DateTime.Now + "_" + "START UPDATING Max Date for Repl Sessions with -1  .." + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            // cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
            // RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            Counter = 0;
            Counter = Mpa.ReadInPoolTransAndGetMaxDatesByATM(LOW_LimitDate);

            if (Mpa.ErrorFound == false)
            {
                stpErrorText += DateTime.Now + "_" + "FINISH UPDATING Max Date for Repl Sessions with -1  .." + Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }
            else
            {
                stpErrorText += DateTime.Now + "_" + "CANCEL or not found DURING UPDATING Max Date for Repl Sessions with -1  .." + Counter.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            }



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
    + " WHERE  (t1.IsMatchingDone = 0 AND t1.Origin='Our Atms' ) "
       + "  AND (t1.Net_TransDate > @Net_TransDate AND t2.SesDtTimeEnd > @Net_TransDate AND t2.ProcessMode IN ( 0 ,-1, -5 ) )"
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

                    stpErrorText = stpErrorText + "Cancel At Master updated with Repl Number 1.1...";
                    CatchDetails_2(ex, "Origin:1812");
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
    + " WHERE  (t1.IsMatchingDone = 1 AND t1.Origin='Our Atms' ) "
     + "  AND (t1.Net_TransDate > @Net_TransDate AND t2.SesDtTimeEnd > @Net_TransDate AND t2.ProcessMode IN ( 0 ,-1, -5 ) )"
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
                        cmd.Parameters.AddWithValue("@Net_TransDate", LOW_LimitDate);
                        cmd.CommandTimeout = 550;  // seconds
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
                    CatchDetails_2(ex, "Origin:1813");
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Repl Number 2..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);




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
                    CatchDetails_2(ex, "Origin:1814");
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
                    CatchDetails_2(ex, "Origin:1815");
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

            SQLCmd =
    " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
    + " SET "
    + " [CardNumber] = t2.[CardNumber] "
    + " ,MatchingCateg = t2.MatchingCateg "
    // + "  , RMCateg = t2.MatchingCateg " this is covered in the next step
    + " ,TransDate = t2.TransDate" // Correct transaction date with seconds 
    + " ,TXNSRC = t2.TXNSRC "
    + " ,TXNDEST = t2.TXNDEST "
    + " ,RRNumber = t2.RRNumber "
    + " ,Card_Encrypted = t2.Card_Encrypted "
    + "  , CAP_DATE = t2.CAP_DATE "
      + "  , SET_DATE = t2.SET_DATE " // SET DATE 
                                      // + "  , AmtFileBToFileC = t2.AmtFileBToFileC " // AMOUNT_EQUIV
       + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
          + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] t2"

    + " ON "
    //   + " CAST(t1.TransDate As Date) = t2.Net_TransDate" // too slow if include this 
    + "  t1.TerminalId = t2.TerminalId "
    + " AND t1.TransAmount = t2.TransAmt"
    + " AND t1.TraceNoWithNoEndZero = t2.TraceNo "
    + " AND t1.Minutes_Date = t2.Minutes_Date"
    //+ " AND t1.AccNumber = t2.AccNo "
    + " WHERE  (t1.IsMatchingDone = 0 AND LEFT(ltrim(rtrim(t1.CardNumber)),6) = '123456' AND t1.Origin='Our Atms'AND t1.MatchingCateg <> 'BDC209' ) "
     + "AND ((t2.Processed = 0 AND t2.TXNDEST= '125')  "
                            //  + " OR (t2.Processed = 1 AND t2.TXNDEST= '125' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                            + " OR (t2.Processed = 1 AND t2.TXNDEST= '125' AND t2.Comment = 'Reversals' AND t2.Net_TransDate>= @Net_TransDate )) "
                ;
            ; // For not processed yet records

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

                    stpErrorText = stpErrorText + "Cancel At Master updated with Category for Cardless...";
                    CatchDetails_2(ex, "Origin:1816");
                    // return;
                }

            stpErrorText += DateTime.Now + "_" + "Master updated with Category for Cardless..." + Counter.ToString() + "\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);

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
                    CatchDetails_2(ex, "Origin:1817");

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
                    CatchDetails_2(ex, "Origin:1818");
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
                    CatchDetails_2(ex, "Origin:1819");
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
                        CatchDetails_2(ex, "Origin:1820");

                        // return;
                    }

                I++; // Read Next entry of the table ... Next Category 
            }

            stpErrorText += DateTime.Now + "_" + "UPDATE Master Pool Involved Files Based on Categories ." + "\r\n";
            //
            // UPDATE GL ACCOUNTS 
            //
            RRDMAccountsClass Acc = new RRDMAccountsClass();
            Acc.ReadAllATMsAndUpdateAccNo(WOperator);

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
               + "  Processed = 0 and ResponseCode <> '0' and ResponseCode <> '112' AND Net_TransDate < @Net_TransDate And Comment ='' " // Non Processed 
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

                    CatchDetails_2(ex, "Origin:1821");

                    //  return;
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

                    CatchDetails_2(ex, "Origin:1822");

                    // return;
                }

            //**********************************************************
            stpErrorText += DateTime.Now + "_" + "Update the ones with Responsecode <> 0\r\n";
            stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
            Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
            //**********************************************************


            stpErrorText += DateTime.Now + "_" + "GL Accounts For ATMs UPDATED." + "\r\n";

            // AT the END UPDATE STATS

            string connectionStringITMX = AppConfig.GetConnectionString("ReconConnectionString");

            // AT the END UPDATE STATS
            int ReturnCode = -1;
            int ret;
            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

            using (SqlConnection conn =
               new SqlConnection(connectionStringITMX))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                        retCode.Direction = ParameterDirection.Output;
                        retCode.SqlDbType = SqlDbType.Int;
                        cmd.Parameters.Add(retCode);

                        cmd.ExecuteNonQuery();

                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            stpErrorText += "OTHER UPDATES FINISHES";
            stpReturnCode = 0;

        }
        //
        // AUTO Complement files 
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

        //
        // UPDATE MASTER FILE WITH SECOND Master file
        //*************************************
        public void UpdateMasterAfterMatchingWithSecondCurrency(string InOperator, int InReconcCycleNo)
        {
            //  return; // Now need for table with two currencies 
            try
            {

                Counter = 0;

                // DELETE Unwanted from TWO Currencies 
                // 

                string SQLCmd = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies] "
                    + "  WHERE MatchingCateg NOT IN('BDC230', 'BDC232' , 'BDC235', 'BDC236' , 'BDC225')   "
                    + " "
                    ;

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            // cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InRMCycleNo - 10); // Subtract 10 from current cycle number

                            cmd.CommandTimeout = 350;  // seconds

                            Counter = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails_2(ex, "Origin:797");
                        // CatchDetails(ex);
                    }

                SQLCmd =
        " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
        + " SET "
        + " [SpareField] = CAST(T2.TransAmt_TWO as nvarchar)  "

           + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
              + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies] t2"

        + " ON "
        //   + " CAST(t1.TransDate As Date) = t2.Net_TransDate" // too slow if include this 
        + "  t1.MatchingCateg = t2.MatchingCateg "
         + " AND  t1.TerminalId = t2.TerminalId "
        + " AND t1.TransAmount = t2.TransAmt"
        + " AND t1.RRNumber = t2.RRNumber"
         + " AND t1.Minutes_Date = t2.Minutes_Date"
        //+ " AND t1.AccNumber = t2.AccNo "
        + " WHERE  (t1.MatchingAtRMCycle = @MatchingAtRMCycle AND t1.MatchingCateg IN " +
        "                             ('BDC230', 'BDC232', 'BDC235' , 'BDC236' , 'BDC225') "
                               //"('BDC230', 'BDC231', 'BDC232', 'BDC233', 'BDC235', 'BDC236') "
                               + ") "

                    ;
                ; // For not processed yet records

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InReconcCycleNo);
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

                        stpErrorText = stpErrorText + "Cancel At Master updated with Category for Cardless...";
                        CatchDetails_2(ex, "Origin:1823");
                        // return;
                    }

                stpErrorText += DateTime.Now + "_" + "Master updated with Category for Cardless..." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);
                //
                // SET NEW matching For BDC225
                //
                SQLCmd =
      " UPDATE [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
      + " SET "
      + " [SpareField] = CAST(T2.TransAmt_TWO as nvarchar)  "

         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] t1 "
            + " INNER JOIN [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns_TWO_Currencies] t2"

      + " ON "
      //   + " CAST(t1.TransDate As Date) = t2.Net_TransDate" // too slow if include this 
      + "  t1.MatchingCateg = t2.MatchingCateg "
       + " AND  t1.TerminalId = t2.TerminalId "
      + " AND t1.TransAmount = t2.TransAmt"
      + " AND t1.RRNumber = t2.RRNumber"
       + " AND t1.CardNumber = t2.CardNumber "
      //+ " AND t1.AccNumber = t2.AccNo "
      + " WHERE  (t1.MatchingAtRMCycle = @MatchingAtRMCycle AND t1.MatchingCateg IN " +
      "                             ('BDC225') "
                             //"('BDC230', 'BDC231', 'BDC232', 'BDC233', 'BDC235', 'BDC236') "
                             + ") "

                  ;
                ; // For not processed yet records

                using (SqlConnection conn = new SqlConnection(recconConnString))
                    try
                    {
                        conn.StatisticsEnabled = true;
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InReconcCycleNo);
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

                        stpErrorText = stpErrorText + "Cancel At Master updated with Category for Cardless...";
                        CatchDetails_2(ex, "Origin:1823");
                        // return;
                    }

                stpErrorText += DateTime.Now + "_" + "Visa updated with Category BDC225..." + Counter.ToString() + "\r\n";
                stpErrorText += DateTime.Now + "_" + "Time Taken in Ms " + commandExecutionTimeInMs.ToString() + "\r\n";
                Flog.Update_stpErrorText(WFlogSeqNo, stpErrorText);



            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }
        }

        // Per ATM find the MIN max date for master , IST and Flexcube
        // Create the file [ATMS].[dbo].[AtmsMinMaxWorking]
        //
        public void Create_ATMS_AtmsMinMaxWorking_FLEX_Or_COREBANKING(string InOperator, int InRMCycle, int InMode)
        {
            int ReturnCode = -20;
            string ErrorReference = "";
            int ret = -1;


            string SPName = "";
            if (InMode == 1) // FLEXCUBE
            {
                SPName = "[ATMs].[dbo].[stp_Reconc_ATMs_MIN_MAX_DATES_FLEX]";
            }
            if (InMode == 2) // COREBANKING
            {
                SPName = "[ATMs].[dbo].[stp_Reconc_ATMs_MIN_MAX_DATES_COREBANKING]";
            }

            using (SqlConnection conn2 = new SqlConnection(ATMSconnectionString))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycle", InRMCycle));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

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
            string WSelectionCriteria = "Operator = '" + InOperator + "' ";
            Ms.ReadReconcSourceFilesToFillDataTable(WSelectionCriteria);

            int I = 0;

            while (I <= (Ms.SourceFilesDataTable.Rows.Count - 1))
            {
                string SourceFile_ID = (string)Ms.SourceFilesDataTable.Rows[I]["SourceFile_ID"];
                string OriginSystem = (string)Ms.SourceFilesDataTable.Rows[I]["OriginSystem"];
                string DbTblName = (string)Ms.SourceFilesDataTable.Rows[I]["DbTblName"];
                string TableStructureId = (string)Ms.SourceFilesDataTable.Rows[I]["TableStructureId"];

                if (TableStructureId == "MOBILE_WALLET")
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

                    if (Environment.UserInteractive)
                    {
                        MessageBox.Show("TOTALS A : Menam copy this and report that message was created from file.." + InPhysicalFiledID);

                    }


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




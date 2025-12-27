using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Linq;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE : Logger
    {
        public RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MOBILE() : base() { }
        /// <summary>
        /// FILE 01 - tblMatchingTxnsMasterPoolATMs
        /// </summary>
        /// 
        // Record Fields 

        public int SeqNo;
        public string MatchingCateg;
        public int RMCycle;
        public string TerminalId;
        public int TransType;
        public string TransDescr;
        public string CardNumber;
        public string AccNo;
        public string TransCurr;
        public decimal TransAmount;
        public decimal AmtFileBToFileC;
        public DateTime TransDate;
        public int TraceNo;
        public string RRNumber;
        public string Matched_Characters;
        public int OriginSeqNo;
        public DateTime FullDtTm;
        public string ResponseCode;

        bool EqualDatesForATMS;

        string WOrigin;

        //string WSelectionCriteria;

        string WJobCategory = "ATMs";

        //public DateTime TxnDate; 

        public string WDepCurNm;

        // Define the data tables 
        public DataTable TableThisCategoryGroups = new DataTable();

        public DataTable TableThisGroupAtms = new DataTable();

        public DataTable TableUnMatched = new DataTable();
        public DataTable TableUnMatchedSaved = new DataTable();
        public int TotalSelected;

        string Message;

        public string CategForMatch;

        public string MatchedCategories;

        bool IS_Matching_At_SET_DATE;

        // Define the data table 
        public DataTable TableUnMatchedCompressed = new DataTable();

        public int TotalSelected2;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        bool ShowMessage;

        // Define the data table 

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        readonly string connectionStringATMs = AppConfig.GetConnectionString("ATMSConnectionString");
        readonly string connectionStringRec = AppConfig.GetConnectionString("ReconConnectionString");

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMReconcCategATMsAtRMCycles Ratms = new RRDMReconcCategATMsAtRMCycles();
        //  RRDMMatchingOfTxnsFindOriginRAW Msf = new RRDMMatchingOfTxnsFindOriginRAW();
        RRDMMatchingCategoriesVsSourcesFiles Mcsf = new RRDMMatchingCategoriesVsSourcesFiles();
        RRDMMatchingSourceFiles Msourcef = new RRDMMatchingSourceFiles();
        RRDMMatchingCategStageVsMatchingFields Mf = new RRDMMatchingCategStageVsMatchingFields();

        RRDMMatchingTxns_WorkingTables Mtw = new RRDMMatchingTxns_WorkingTables();
        //   RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();

        RRDMMatchingReconcExceptionsInfoAnyTables Mre = new RRDMMatchingReconcExceptionsInfoAnyTables();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        string SelectionCriteria;

        DateTime WCut_Off_Date;

        public int NumberOfMatchedCategories;

        // Initialise for next
        DateTime BeforeCallDtTime; // Use For Performance 
        DateTime BeforeCallDtTime_G; // For Groups
        // string TraceOrRRN;

        int NumberOfTables;

        string SourceTable_A;
        string SourceTable_B;
        string SourceTable_C;

        string SourceTable_A_Matched;
        string SourceTable_B_Matched;
        string SourceTable_C_Matched;

        // Table Fields 
        public string TableId;
        public string WCase;
        public int M_Type;
        string TransactionId;

        string Senderscheme;
        string SenderTelephone;
        string Receivernumber;
        //  DateTime TransDate;
        DateTime Net_TransDate;
        public int DublInPos;
        public int NotInPos;
        public string UserId;

        public string W_MPComment;

        public int ReadyCategories;

        public int NumberOfUnmatchedForCategory;

        DateTime MaxDt01;
        DateTime MaxDt02;
        DateTime MaxDt03;

        DateTime MinMaxDt;
        DateTime MinMaxDt_01;
        DateTime MinMaxDt_02;
        DateTime MinMaxDt_03;

        bool POS_Type;
        int UnMatchedForWorkingDays;
        int UnMatchedForCalendarDays;

        string PRX;

        bool No_Group_Type;
        bool Has_Adjustments;
        bool TwoAndThreeIsRRN;
        bool StartFromThree;

        bool IsFrom_TPF;

        //bool TestingTwoFiles; 

        string WMatchingCateg;
        int WRMCycle;
        int WGroupId;
        string WReconcCategoryId;
        string WSignedId;
        string WOperator;
        string W_Application;
        string W_DB;
        bool WTestingWorkingFiles;

        //
        // Make Matching with Categories that are ready to be matched
        // 
        public void MatchReadyCategoriesUpdate(string InOperator, string InSignedId,
                                       int InRMCycle, string In_Application)
        {
            CategForMatch = "";
            MatchedCategories = "";
            WOperator = InOperator;
            W_DB = W_Application = In_Application;
            // THIS IS FOR BANK De Caire where journal and Ist and Flecube have same date

            if (InOperator == "BCAIEGCX")
            {
                PRX = "QAH"; // "+PRX+" eg "BDC"
            }
            else
            {
                PRX = "EMR";
            }

            EqualDatesForATMS = true;

            ShowMessage = false;
            string Origin = In_Application;
            Mc.ReadMatchingCategoriesAndFillTable(WOperator, In_Application);

            //WMatchingCateg = "";

            bool ReadyCat;

            // LOOP FOR Matching Categories

            int I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 

                int WCat_SeqNo = (int)Mc.TableMatchingCateg.Rows[I]["SeqNo"];

                Mc.ReadMatchingCategorybySeqNoActive(WOperator, WCat_SeqNo);

                if (Mc.RecordFound == false)
                {
                    I = I + 1;
                    continue;
                }

                WMatchingCateg = Mc.CategoryId;
                POS_Type = Mc.Pos_Type;
                UnMatchedForWorkingDays = Mc.UnMatchedForWorkingDays;
                UnMatchedForCalendarDays = Mc.UnMatchedForCalendarDays;
                WOrigin = Mc.Origin;

                ReadyCat = false;

                ////
                // ALLOW THIS
                //

                if (WMatchingCateg == PRX + "370"
                    //  || WMatchingCateg == PRX + "273"


                    //|| WMatchingCateg == PRX + "215" // Credit Card BDC ATMs

                    )
                {

                    //Message = "THIS THE ONE - " + WMatchingCateg + " MOBILE   ";
                    //System.Windows.Forms.MessageBox.Show(Message);
                    // CONTINUE TO MATCHING 
                }
                else
                {
                    //I++;
                    //continue;
                }

                //ALLOW ONLY THIS
                // RRDM-PANICOS
                //if (Environment.MachineName == "RRDM-PANICOS")
                //{
                //    if (WMatchingCateg == PRX + "270"   // 
                //                                        // || WMatchingCateg == PRX+"251" //  Fawry file 
                //                                        //  || WMatchingCateg == PRX+"215" //  123 Net BDC ATMs
                //                                        // WMatchingCateg == PRX+"225" // Visa BDC ATMs
                //                                        //   WMatchingCateg == PRX+"215" // Credit Card BDC ATMs
                //                      )
                //    {
                //        Message = "THIS THE ONE - " + WMatchingCateg + "   - -- ";
                //        System.Windows.Forms.MessageBox.Show(Message);
                //        // CONTINUE TO MATCHING 
                //    }
                //    else
                //    {
                //        I++;
                //        continue;
                //    }
                //}




                SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Mcsf.ReadReconcCategoriesVsSourcebyCategory(WMatchingCateg);

                // ShowMessage = true;
                //Mcsf.TotalOne = 0; 
                if (Mcsf.RecordFound == true & (Mcsf.TotalZero == 0 & Mcsf.TotalOne == 0))
                {
                    // Means that All are -1 (Journal is excluded) => READY
                    W_MPComment = "";
                    //******************************************************************
                    //*****************************************************************
                    // Insert Message And show it if needed
                    //

                    Message =
                           "Matching Category :" + WMatchingCateg + Environment.NewLine
                         + "Ready for Matching." + Environment.NewLine
                         + "";
                    // Initialise for next Performance 
                    //BeforeCallDtTime = DateTime.Now;
                    //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                    string text = "Start Matching of category .. " + WMatchingCateg;
                    string caption = "Matching of Categories";
                    int timeout = 2000;
                    if (Environment.UserInteractive)
                    {
                        AutoClosingMessageBox.Show(text, caption, timeout);
                    }

                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                    //******+++++++++++++++++++++++++++*******************************



                    ReadyCat = true;

                    NumberOfMatchedCategories = NumberOfMatchedCategories + 1;

                }
                else
                {
                    //******************************************************************
                    //*****************************************************************
                    // Insert Message And show it if needed
                    //
                    if (Mcsf.RecordFound == true)
                    {
                        Message =
                         "Matching Category :" + WMatchingCateg + Environment.NewLine
                                   + " Not Ready for Matching. " + Environment.NewLine
                                   + " File..." + Environment.NewLine
                                   + Mcsf.SourceFileName + Environment.NewLine
                                   + " ....not loaded yet!";
                    }
                    else
                    {
                        Message = "Matching Category :" + WMatchingCateg + Environment.NewLine
                                                          + " Has No Defined Files. "
                                                          + " ";

                    }

                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }
                    //
                    //****************************************************************
                    //****************************************************************

                    ReadyCat = false;
                }
                if (ReadyCat == true)
                {

                    CategForMatch = CategForMatch + WMatchingCateg + "..Ready" + "\r\n";
                }
                if (ReadyCat == false)
                {
                    CategForMatch = CategForMatch + WMatchingCateg + ".. NOT Ready" + "\r\n";

                    W_MPComment = DateTime.Now + "_" + ".. NOT Ready Category.. " + WMatchingCateg + Environment.NewLine;
                    W_MPComment += DateTime.Now + "_" + ".. Either Matching Files Not Defined For this Category" + Environment.NewLine;
                    W_MPComment += DateTime.Now + "_" + ".. OR Not all Files Read . " + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WMatchingCateg, InRMCycle, W_MPComment);

                }

                #region Do MATCHING FOR THE READ CATEGORIES

                if (ReadyCat == true)
                {
                    //*****************************************************
                    // DO MATCHING 
                    //*****************************************************


                    Message = "Matching Starts";
                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    //**********************
                    BeforeCallDtTime = DateTime.Now;

                    W_MPComment += DateTime.Now + "_" + "Matching Starts Category.. " + WMatchingCateg + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WMatchingCateg, InRMCycle, W_MPComment);

                    //**********************

                    bool TestingWorkingFiles = false;

                    Matching_ForThisMatchingCategory(WOperator, InSignedId, WMatchingCateg,
                                                                                                 InRMCycle, TestingWorkingFiles);


                    if (NumberOfUnmatchedForCategory > 0)
                    {
                        //******************************************************************
                        //*****************************************************************
                        // Insert Message And show it if needed
                        //
                        Message =
                              "ALL records for category " + WMatchingCateg + " have been processed" + Environment.NewLine
                                   + " Discrepancies were found " + Environment.NewLine
                                   + " Number of Discrepancies..." + NumberOfUnmatchedForCategory.ToString() + Environment.NewLine
                                   + " Please do Reconciliation ";

                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, BeforeCallDtTime, DateTime.Now, Message);

                        if (ShowMessage == true & Environment.UserInteractive)
                        {
                            System.Windows.Forms.MessageBox.Show(Message);
                        }
                        //****************************************************************
                        //******+++++++++++++++++++++++++++*******************************
                    }
                    if (NumberOfUnmatchedForCategory == 0)
                    {

                        //******************************************************************
                        //*****************************************************************
                        // Insert Message And show it if needed
                        //
                        Message =
                              "ALL records for category " + WMatchingCateg + " have been processed" + Environment.NewLine
                                   + " Discrepancies were NOT found ";

                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, BeforeCallDtTime, DateTime.Now, Message);

                        if (ShowMessage == true & Environment.UserInteractive)
                        {
                            System.Windows.Forms.MessageBox.Show(Message);
                        }
                        //****************************************************************
                        //******+++++++++++++++++++++++++++*******************************
                    }

                    // FOR THIS CATEGORY TURN ALL CATEGORIES/FILES COMBINATIONS TO 1

                    Mcsf.UpdateReconcCategoryVsSourceRecordProcessCodeToOne(WMatchingCateg);

                    MatchedCategories = MatchedCategories + WMatchingCateg + "..MATCHED" + "\r\n";

                }
                #endregion

                I++; // Read Next entry of the table ... Next Category 
            }
        }

        public int ENQ_NumberOfCatToBeMatched;
        public string ENQ_CategForMatch;
        public string ENQ_MatchedCateg;

        public void MatchReadyCategoriesEnquiry(string InOperator, string InSignedId,
                                     int InRMCycle, string In_Application)
        {

            CategForMatch = "";

            WOperator = InOperator;

            ShowMessage = false;

            W_Application = In_Application; 

            Mc.ReadMatchingCategoriesAndFillTable(WOperator, In_Application);

            string WMatchingCateg;
            bool ReadyCat;
            W_MPComment = "CATEGORIES MATCHING STATUS" + "\r\n" + "__________" + "\r\n";

            // LOOP FOR Matching Categories
            // MOBILE

            int I = 0;

            while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
            {
                // Do 
                WMatchingCateg = (string)Mc.TableMatchingCateg.Rows[I]["Identity"];
                WOrigin = (string)Mc.TableMatchingCateg.Rows[I]["Origin"];

                ReadyCat = false;

                ////

                SelectionCriteria = " CategoryId ='" + WMatchingCateg + "'";

                Mcsf.ReadReconcCategoriesVsSourcebyCategory(WMatchingCateg);

                if (Mcsf.RecordFound == true & (Mcsf.TotalZero == 0 & Mcsf.TotalOne == 0))
                {
                    // Means that All are -1 (Journal is excluded) => READY

                    //******************************************************************
                    //*****************************************************************
                    // Insert Message And show it if needed
                    //

                    ReadyCat = true;

                    NumberOfMatchedCategories = NumberOfMatchedCategories + 1;

                }
                else
                {
                    ReadyCat = false;
                }
                if (ReadyCat == true)
                {

                    W_MPComment = W_MPComment + WMatchingCateg + ".....Ready For Matching" + "\r\n";
                }
                if (ReadyCat == false)
                {
                    W_MPComment = W_MPComment + WMatchingCateg + ".....NON Ready For Matching" + "\r\n";
                }


                I++; // Read Next entry of the table ... Next Category 
            }
        }


        // For this Category Find Groups and Call to Create Working Files
        // For each atm in each group 
        public void Matching_ForThisMatchingCategory
                                      (string InOperator, string InSignedId,
                                       string InMatchingCategoryId,
                                       int InRMCycle, bool InTestingWorkingFiles)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;

            WMatchingCateg = InMatchingCategoryId;
            WRMCycle = InRMCycle;
            WTestingWorkingFiles = InTestingWorkingFiles;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            // SHOW MESSAGE 
            string ParId = "719";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES")
            {
                ShowMessage = true;
            }
            else
            {
                ShowMessage = false;
            }

            int TempCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, W_Application );
            if (Rjc.RecordFound==true)
            {
                WCut_Off_Date = Rjc.Cut_Off_Date;
            }
            else
            {
                MessageBox.Show("Not Found Cycle At 898797");
                return; 
            }
            

            #region Assign Files as defined in Matching Category

            Mc.ReadMatchingCategorybyActiveCategId(WOperator, WMatchingCateg);

            //if (WMatchingCateg == "ETI360")
            //{
            //    WMatchingCateg = "ETI360";
            //}
            Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
            NumberOfTables = Mcsf.TotalRecords;

            if (NumberOfTables > 2)
            {
                MessageBox.Show("Category.." + WMatchingCateg + Environment.NewLine
                             + "Has more than two files to matched" + Environment.NewLine
                             + "ASK RRDM to set up the parameters for this job.." + Environment.NewLine
                                  );
                return;
            }

            //if (WMatchingCateg == "QAH310") WOrigin = "QAHERA TPF VS MEEZA"; // CATEGORY 310

            if (Mcsf.SourceFileNameA == "QAHERA_TPF_TXNS"
                || Mcsf.SourceFileNameA == "ETISALAT_TPF_TXNS"
                || Mcsf.SourceFileNameA == "IPN_TPF_TXNS"
                )
            {
                IsFrom_TPF = true;
            }
            else
            {
                IsFrom_TPF = false;
            }

            if (NumberOfTables == 2 & IsFrom_TPF == true)
            {

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
                SourceTable_A = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "_MASTER" + "]";
                SourceTable_A_Matched = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "_MASTER" + "]";

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "]";

            }

            if (NumberOfTables == 2 & IsFrom_TPF == false)
            {
                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);

                SourceTable_A = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "]";

                // Move Records from A to Master Pool
                string TempMaster = "";
                if (W_Application == "ETISALAT")
                {
                    TempMaster = W_DB + ".[dbo].[ETISALAT_TPF_TXNS_MASTER]";
                }
                if (W_Application == "QAHERA")
                {
                    TempMaster = W_DB + ".[dbo].[QAHERA_TPF_TXNS_MASTER]";
                }
                if (W_Application == "IPN")
                {
                    TempMaster = W_DB + ".[dbo].[IPN_TPF_TXNS_MASTER]";
                }
                // MOVE TABLEA TO MASTER
                Mmob.ReadUniversalAndCreateMaster_Mobile_Records(WOperator, TempMaster, SourceTable_A, WMatchingCateg, WRMCycle, W_Application);

                // Change SourceTable_A to MASTER 
                SourceTable_A = TempMaster;
                SourceTable_A_Matched = TempMaster;

                Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
                SourceTable_B = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "]";
                SourceTable_B_Matched = W_DB + ".[dbo]." + "[" + Msourcef.InportTableName + "]";
            }

            #endregion

            #region Make Matching For All Groups

            #region Read And Find All Groups of this Category 

            NumberOfUnmatchedForCategory = 0;


            //******************************************************
            // CREATE FILES FOR THE SPECIFIC CATEGORY BASED ON MINMAX
            // NO GROUP => But Category
            // UPDATE SOURCE FILES AS PROCESSED          
            //******************************************************

            WReconcCategoryId = WMatchingCateg;
            RecordsToMatch = false;

            W_MPComment += DateTime.Now + "_" + "Create Working Files for category . ID:.." + WReconcCategoryId + Environment.NewLine;

            Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

            if (POS_Type == true)
            {
                CreateWorkingTables_POS_Or_OutGoing(WOperator, WSignedId,
                                            WMatchingCateg, WReconcCategoryId,
                                                           WRMCycle);

            }
            else
            {
                //CreateWorkingTables(WOperator, WSignedId,
                //                            WMatchingCateg, WReconcCategoryId,
                //                                           WRMCycle, WGroupId);

            }
            RecordsToMatch = true;
            if (RecordsToMatch == true)
            {
                W_MPComment += DateTime.Now + "_" + "Working Files were created.." + Environment.NewLine;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                //*******************************
                // MAkE MATCHING

                //******************************
                // Find out if this Category belongs to matching by SET_DATE
                //

                ParId = "821";

                Gp.ReadParametersSpecificNm(InOperator, ParId, WReconcCategoryId);

                if (Gp.RecordFound == true)
                {
                    // IN OUR CASE THIS THE ETI360 which is the FAWRY
                    IS_Matching_At_SET_DATE = true;
                    // 
                }
                else
                {
                    IS_Matching_At_SET_DATE = false;
                }


                W_MPComment += DateTime.Now + "_" + "Matching Starts." + Environment.NewLine;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);
                //************************
                //*************************
                MakeMatchingByGroupOrCategory(WOperator, WSignedId, WMatchingCateg, WReconcCategoryId, WRMCycle, WGroupId);

                //**********************
                // UPDATE Rcs
                //**********************
                if (NumberOfDublicates > 0)
                {
                    W_MPComment += DateTime.Now + "_" + "Number of Dublicates .. " + NumberOfDublicates.ToString() + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                }
                if (NumberOfUnmatched > 0)
                {
                    W_MPComment += DateTime.Now + "_" + "Number of Unmatched (double).. " + NumberOfUnmatched.ToString() + Environment.NewLine;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                   

                }

                //NumberOfRecords
                W_MPComment += DateTime.Now + "_" + "Number of Records Matched.. " + NumberOfRecords.ToString() + Environment.NewLine;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);


                //************************
                // FINALLY - UPDATING AFTER MATCHING 
                //************************
                UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);
            }
            else
            {
                W_MPComment += DateTime.Now + "_" + "No Records to match for this Category.. Process finishes for this category" + Environment.NewLine;
                W_MPComment += DateTime.Now + "_________________________" + Environment.NewLine;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                //************************
                // FINALLY - UPDATING AFTER MATCHING 
                //************************
                // UpdatingAfterMatching_Mode_2(WOperator, WMatchingCateg, WReconcCategoryId);

                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);

            }




            #endregion

            #endregion

            //#region UpdateCategory VS Source Files that Matching is Done  

            //Mcsf.UpdateReconcCategoryVsSourceRecord(WOperator, 66);     

            //#endregion

        }
        //
        // CREATE FILES FOR THE SPECIFIC GROUP, 
        //
        int NumberOfLoadedToWorkingTablesATMs;
        int WAtmsReconcGroup;
        string WAtmNo;
        bool RecordsToMatch;
        //string PrimaryMatchingFieldNm;

        //int MaxPrimaryMatchingField_01;
        //int MaxPrimaryMatchingField_02;
        //int MaxPrimaryMatchingField_03;
        //DateTime LastDateTime;
        private void CreateWorkingTables(string InOperator, string InSignedId,
                                                  string InMatchingCateg, string InReconcCateg,
                                                  int WRMCycle, int InAtmsReconcGroup)
        {
            #region Initialise Variables

            RRDMAtmsMinMax Mm = new RRDMAtmsMinMax();

            EqualDatesForATMS = true;

            NumberOfLoadedToWorkingTablesATMs = 0;
            int I = 0;

            #endregion

            #region For this Group Find all ATMs

            #endregion



            #region Find MINMax for Tables in the Category that comes not from Our ATMs and Insert Records in Working Files

            //********************
            // NOT ATMs PROCESSING
            //********************
            string RRNumber01;
            string RRNumber02;
            string RRNumber03;

            string Mask_Card01;
            string Mask_Card02;
            string Mask_Card03;

            decimal TransAmt01;
            decimal TransAmt02;
            decimal TransAmt03;
            DateTime WCAP_DATE;
            DateTime WSET_DATE;

            if (IsFrom_TPF == false)
            {

                //*****************************************************
                // CHECK IF THERE ARE RECORDS TO MATCHED FOR THE THREE FILES
                //*****************************************************
                // Find MaxDate 
                //

                MaxDt01 = NullPastDate;
                MaxDt02 = NullPastDate;
                if (NumberOfTables == 3)
                {
                    MaxDt03 = NullPastDate;
                }


                TransAmt01 = 0;
                TransAmt02 = 0;
                TransAmt03 = 0;

                TableId = SourceTable_A;

                DateTime MaxSetl_date = new DateTime(1900 - 01 - 01);

                //MaxDt01 = Mgt_BDC.ReadAndFindMaxDateTimeForMpa(TableId, InMatchingCateg, WAtmNo);
                //RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
                //MaxDt01 = Mgt_BDC.ReadAndFindMaxDateTimeForMpa_NO_ATM(TableId, InMatchingCateg);
                MaxDt01 = Mmob.ReadAndFindMaxDate_MOBILE(TableId, InMatchingCateg, 1);

                if (Mmob.RecordFound == true)
                {
                    Mmob.ReadAndFindRRNUMBER_MOBILE(TableId, InMatchingCateg, MaxDt01, 1);
                    //RRNumber01 = Mpa.RRNumber;
                    //Mask_Card01 = Mpa.CardNumber;
                    //TransAmt01 = Mpa.TransAmount;
                    MaxSetl_date = Mmob.SET_DATE;
                }

                // TRACE Step1
                Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                   + "Step1:" + MaxDt01.ToString();
                Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                //
                // Handling the 123 exception where after cut off 123 do not sends all transactions
                // AND OTHER CATEGORIES
                int MinusMin = 0;

                // Check if this Matching Category goes to the direction of SET to CAP_DATE reconciliation method
                bool SET_TO_SET = false;

                string ParId = "821";

                Gp.ReadParametersSpecificNm(InOperator, ParId, InMatchingCateg);

                if (Gp.RecordFound == true)
                {
                    SET_TO_SET = true;
                    // 
                }

                // SET it to avoid 
                MinusMin = 0; // Set it to avoid activation of the next process
                if (SET_TO_SET == true & MinusMin == 0)
                {
                    // OK 
                    // If MinusMin = 0 means that we go this direction where we compare based on CAP_DATE
                    // 
                    // WE DO MATCHING UP TO WCAP_DATE OF THE 123 file

                    WSET_DATE = MaxDt01.Date; // This was the old method

                    WSET_DATE = MaxSetl_date;

                    if (MaxDt01.Date == NullPastDate)
                    {
                        // No records for one of files 
                        return;
                    }

                    //WCut_Off_Date = WCut_Off_Date;

                    // THIS FOR THE CATEGORIES NOT THE ATMS
                    //

                    RecordsToMatch = true;

                    //
                    // INITIALISE WORKING TABLE
                    //
                    Method_TruncateWorkingTables_MOBILE(NumberOfTables);
                    ///
                    RecordsToMatch = true;

                    // Call stored Procedure to create files

                    Mtw.PopulateWorkingFile_Working01_NOT_ATMS_No_Date_MOBILE("01", SourceTable_A, WMatchingCateg, WRMCycle, WSET_DATE);

                    //Take all records with the set Settlement date 
                    Mtw.PopulateWorkingFile_V02_NotATMs_On_SET_DATE_MOBILE("02", SourceTable_B, WMatchingCateg, WRMCycle, WSET_DATE);

                    if (NumberOfTables == 3)
                    {
                        // Insert Records In Working File03

                        Mtw.PopulateWorkingFile_V02_NotATMs_On_SET_DATE_MOBILE("03", SourceTable_C, WMatchingCateg, WRMCycle, WSET_DATE);
                    }
                    ///

                    //
                    // Call to update source files 
                    //
                    // Update SourceTable_A with Processed = 1 and RMCycle
                    string MatchMask = "";
                    if (NumberOfTables == 3)
                    {
                        MatchMask = "111";
                    }
                    if (NumberOfTables == 2)
                    {
                        MatchMask = "11";
                    }

                    // UPDATE Master

                    TableId = SourceTable_A;
                    Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

                    // Update SourceTable_B with Processed = 1 and RMCycle
                    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                    TableId = SourceTable_B;

                    Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

                    if (NumberOfTables == 3)
                    {
                        // Update SourceTable_C with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                        TableId = SourceTable_C;

                        Mmob.UpdateSourceTablesAsProcessed_NonAtms_Based_SET_DATE(WMatchingCateg, TableId, WSET_DATE, WRMCycle, MatchMask);


                    }

                    // Update Seq numbers in Mpa for Table 
                    TableId = SourceTable_B;
                    //Mgt_BDC.UpdateMpaWithSeqNoForTables_NonAtms(WMatchingCateg, TableId,
                    //                        WRMCycle);

                    if (NumberOfTables == 3)
                    {
                        TableId = SourceTable_C;
                        //      Mgt_BDC.UpdateMpaWithSeqNoForTables_NonAtms(WMatchingCateg, TableId,
                        //                          WRMCycle);
                    }

                    Message = "Txns added in Working Files for Matching Category :"
                                                  + InMatchingCateg;

                }
                else
                {
                    RRNumber01 = "";
                    RRNumber02 = "";
                    RRNumber03 = "";
                    Mask_Card01 = "";
                    Mask_Card02 = "";
                    Mask_Card03 = "";
                    // NOT DONE FOR MOBILE
                    // PREPARE WORKING FILES BASED ON MIN MAX DATES
                    TableId = SourceTable_B;

                    MaxDt02 = Mgt.ReadAndFindMaxDt_NO_ATM(TableId, InMatchingCateg);

                    if (Mgt.RecordFound == true)
                    {

                        RRNumber02 = Mgt.RRNumber;
                        Mask_Card02 = Mgt.CardNumber;
                        TransAmt02 = Mgt.TransAmt;
                    }

                    // TRACE Step4
                    Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                       + "Step4:" + MaxDt02.ToString();
                    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);


                    if (NumberOfTables == 3)
                    {
                        TableId = SourceTable_C;

                        MaxDt03 = Mgt.ReadAndFindMaxDt_NO_ATM(TableId, InMatchingCateg);

                        if (Mgt.RecordFound == true)
                        {
                            RRNumber03 = Mgt.RRNumber;
                            Mask_Card03 = Mgt.CardNumber;
                            TransAmt03 = Mgt.TransAmt;
                        }
                    }
                    // CHECK IF NO RECORDS TO MATCH
                    if (NumberOfTables == 2)
                    {
                        if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate)
                        {
                            // No records for one of files 
                            return;
                        }

                    }
                    if (NumberOfTables == 3)
                    {
                        if (MaxDt01 == NullPastDate || MaxDt02 == NullPastDate || MaxDt03 == NullPastDate)
                        {
                            // No records for one of files 
                            return;
                        }
                    }


                    // Initialise 
                    MinMaxDt = NullPastDate;


                    if (NumberOfTables == 2)
                    {

                        DateTime[] array1 = { MaxDt01, MaxDt02 };

                        //
                        // Find minimum number.
                        // 

                        MinMaxDt = array1.Where(a => a != NullPastDate).Min();

                        // Find details of the last Txn

                        // TRACE Step5
                        Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                           + "Step5:" + MinMaxDt.ToString();
                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);


                        bool EqualMinMax = false;

                        if (MaxDt01 == MaxDt02) EqualMinMax = true;

                        if (MaxDt01 == MinMaxDt)
                        {
                            MinMaxDt_01 = MinMaxDt;

                            // Based on Details Locate exact DateTime on File B
                            TableId = SourceTable_B;
                            MinMaxDt_02 = Mgt.ReadAndFindDateBasedOnDetails_NO_ATM(TableId, WMatchingCateg,
                                                                                                Mask_Card01,
                                                                                                TransAmt01,
                                                                                                    MaxDt01,
                                                                                                        RRNumber01);
                            // TRACE Step6
                            Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                               + "Step6:" + MinMaxDt_02.ToString();
                            Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                            if (Mgt.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // No records for one of files 
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 1059...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}

                                return;
                            }

                        }
                        if (MaxDt02 == MinMaxDt & EqualMinMax == false)
                        {
                            MinMaxDt_02 = MinMaxDt;

                            MinMaxDt_01 = Mpa.ReadInPoolTransSpecificDuringMatching_2_NO_ATM(WMatchingCateg,
                                                          Mask_Card02,
                                                          TransAmt02,
                                                          RRNumber02,
                                                          MaxDt02,
                                                          1);
                            if (Mpa.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}

                                return;
                            }
                        }

                    }

                    if (NumberOfTables == 3)
                    {

                        DateTime[] array1 = { MaxDt01, MaxDt02, MaxDt03 };

                        //
                        // Find minimum date.
                        // 
                        //if (InMatchingCateg == PRX+"210")
                        //{
                        //    MessageBox.Show(" MAX DATE 123 " + MaxDt01.ToString() + Environment.NewLine
                        //        + " MAX DATE for IST..." + MaxDt02.ToString() + Environment.NewLine
                        //         + " MAX DATE for Flex" + MaxDt03.ToString() + Environment.NewLine
                        //        );
                        //}

                        MinMaxDt = array1.Where(a => a != NullPastDate).Min();

                        //if (InMatchingCateg == PRX+"210")
                        //{
                        //    MessageBox.Show(" MINIMAX DATE " + MinMaxDt.ToString() + Environment.NewLine

                        //        );
                        //}
                        //

                        // Find details of the last Txn

                        if (MaxDt01 == MinMaxDt)
                        {
                            MinMaxDt_01 = MinMaxDt;

                            // Based on Details Locate exact DateTime on File B
                            TableId = SourceTable_B;
                            MinMaxDt_02 = Mgt.ReadAndFindDateBasedOnDetails_NO_ATM(TableId, WMatchingCateg,
                                                                                                   Mask_Card01,
                                                                                                TransAmt01,
                                                                                                    MaxDt01,
                                                                                                        RRNumber01);



                            if (Mgt.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // No records for one of files 
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 1059...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                //if (InMatchingCateg == PRX+"210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine
                                //        );
                                //}

                                return;
                            }
                            TableId = SourceTable_C;
                            MinMaxDt_03 = Mgt.ReadAndFindDateBasedOnDetails_NO_ATM(TableId, WMatchingCateg,
                                                                                                   Mask_Card01,
                                                                                                TransAmt01,
                                                                                                    MaxDt01,
                                                                                                        RRNumber01);

                            if (Mgt.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // No records for one of files 
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 10592...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                //if (InMatchingCateg == PRX+"210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                //        );
                                //}
                                return;
                            }

                        }
                        if (MaxDt02 == MinMaxDt)
                        {
                            MinMaxDt_02 = MinMaxDt;
                            // Find Details of Last Reocord in FileB

                            MinMaxDt_01 = Mpa.ReadInPoolTransSpecificDuringMatching_2_NO_ATM(WMatchingCateg,
                                                         Mask_Card02,
                                                          TransAmt02,
                                                          RRNumber02,
                                                          MaxDt02,
                                                          1);

                            if (Mpa.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                //if (InMatchingCateg == PRX+"210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                //        );
                                //}

                                return;
                            }
                            TableId = SourceTable_C;
                            MinMaxDt_03 = Mgt.ReadAndFindDateBasedOnDetails_NO_ATM(TableId, WMatchingCateg,
                                                                                                    Mask_Card02,
                                                                                                TransAmt02,
                                                                                                    MaxDt02,
                                                                                                        RRNumber02);

                            if (Mgt.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // No records for one of files 
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 10593...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                if (InMatchingCateg == PRX + "210")
                                {
                                    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                        );
                                }

                                return;
                            }
                        }
                        if (MaxDt03 == MinMaxDt)
                        {
                            MinMaxDt_03 = MinMaxDt;

                            MinMaxDt_01 = Mpa.ReadInPoolTransSpecificDuringMatching_2_NO_ATM(WMatchingCateg,
                                                          Mask_Card03,
                                                          TransAmt03,
                                                          RRNumber03,
                                                          MaxDt03,
                                                          1);
                            if (Mpa.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 1061...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                //if (InMatchingCateg == PRX+"210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                //        );
                                //}

                                return;
                            }

                            // Based on Details Locate exact DateTime on File B
                            TableId = SourceTable_B;
                            MinMaxDt_02 = Mgt.ReadAndFindDateBasedOnDetails_NO_ATM(TableId, WMatchingCateg,
                                                                                                    Mask_Card03,
                                                                                                TransAmt03,
                                                                                                    MaxDt03,
                                                                                                        RRNumber03);

                            if (Mgt.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // No records for one of files 
                                //if (ShowMessage == true & Environment.UserInteractive)
                                //{
                                System.Windows.Forms.MessageBox.Show("ERROR 10594...FOR.." + WMatchingCateg + "..DATE.." + MinMaxDt.ToString());
                                //}
                                //if (InMatchingCateg == PRX+"210")
                                //{
                                //    MessageBox.Show(" RRNumber01 " + RRNumber01 + Environment.NewLine

                                //        );
                                //}

                                return;
                            }
                        }
                    }

                    #endregion

                    #region For the Category Insert Records in Working Files and set original records as processed
                    //
                    // THIS FOR THE CATEGORIES NOT THE ATMS
                    //
                    if (MinMaxDt != NullPastDate)
                    {
                        RecordsToMatch = true;

                        //
                        // INITIALISE WORKING TABLE
                        //
                        Method_TruncateWorkingTables_MOBILE(NumberOfTables);
                        ///
                        RecordsToMatch = true;

                        // TRACE Step7
                        Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                           + "Step7:" + MinMaxDt_01.ToString();
                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                        // TRACE Step8
                        Message = "MATCHING.." + WMatchingCateg + Environment.NewLine
                                           + "Step8:" + MinMaxDt_02.ToString();
                        Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);


                        // Call stored Procedure to create files
                        // Insert Records In File01Y from Mpa
                        if (WTestingWorkingFiles == false)
                            //     Mtw.PopulateWorkingFile_NOT_ATMs("01", SourceTable_A, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                            Mtw.PopulateWorkingFile_Working01_NOT_ATMS("01", SourceTable_A, WMatchingCateg, WRMCycle, MinMaxDt_01);
                        // Insert Records In Working File02 

                        if (WTestingWorkingFiles == false)
                            //Mtw.PopulateWorkingFile_NOT_ATMs("02", SourceTable_B, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                            Mtw.PopulateWorkingFile_V02_NotATMs("02", SourceTable_B, WMatchingCateg, WRMCycle, MinMaxDt_02);
                        if (NumberOfTables == 3)
                        {
                            // Insert Records In Working File03

                            if (WTestingWorkingFiles == false)
                                //   Mtw.PopulateWorkingFile_NOT_ATMs("03", SourceTable_C, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                                Mtw.PopulateWorkingFile_V02_NotATMs("03", SourceTable_C, WMatchingCateg, WRMCycle, MinMaxDt_03);
                        }
                        ///
                        //if (InMatchingCateg == PRX+"210")
                        //{
                        //    MessageBox.Show(" WORKING TABLES CREATED " + Environment.NewLine
                        //        + " MinMaxDate_01 " + MinMaxDt_01 + Environment.NewLine
                        //         + " MinMaxDate_02 " + MinMaxDt_02 + Environment.NewLine
                        //          + " MinMaxDate_03 " + MinMaxDt_03 + Environment.NewLine
                        //        );
                        //}

                        //
                        // Call to update source files 
                        //
                        // Update SourceTable_A with Processed = 1 and RMCycle
                        string MatchMask = "";
                        if (NumberOfTables == 3)
                        {
                            MatchMask = "111";
                        }
                        if (NumberOfTables == 2)
                        {
                            MatchMask = "11";
                        }

                        // UPDATE Mpa
                        Mpa.UpdateMpaRecordsAsProcessed_NO_ATM(WMatchingCateg, MinMaxDt_01, WRMCycle, MatchMask, 1);

                        // Update SourceTable_B with Processed = 1 and RMCycle
                        //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
                        TableId = SourceTable_B;

                        Mgt.UpdateSourceTablesAsProcessed_NonAtms(WMatchingCateg, TableId, MinMaxDt_02, WRMCycle, MatchMask);

                        if (NumberOfTables == 3)
                        {
                            // Update SourceTable_C with Processed = 1 and RMCycle
                            //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankingSystemTxns]";
                            TableId = SourceTable_C;

                            Mgt.UpdateSourceTablesAsProcessed_NonAtms(WMatchingCateg, TableId, MinMaxDt_03, WRMCycle, MatchMask);


                        }

                        // Update Seq numbers in Mpa for Table 
                        TableId = SourceTable_B;
                        //   Mgt_BDC.UpdateMpaWithSeqNoForTables_NonAtms(WMatchingCateg, TableId,
                        //                           WRMCycle);

                        if (NumberOfTables == 3)
                        {
                            TableId = SourceTable_C;
                            //      Mgt_BDC.UpdateMpaWithSeqNoForTables_NonAtms(WMatchingCateg, TableId,
                            //                          WRMCycle);
                        }

                        Message = "Txns added in Working Files for Matching Category :"
                                                      + InMatchingCateg;


                    }

                }



            }

            #endregion


        }


        public void CreateWorkingTables_POS_Or_OutGoing(string InOperator, string InSignedId,
                                                string InMatchingCateg, string InReconcCateg,
                                                int WRMCycle)
        {

            #region For the Category Insert Records in Working Files and set original records as processed


            RecordsToMatch = false;

            //
            // INITIALISE WORKING TABLES
            //
            Method_TruncateWorkingTables_MOBILE(NumberOfTables);
            ///

            // Call stored Procedure to create files
            // Insert Records In File01Y from Mpa
            if (WTestingWorkingFiles == false)
                //     Mtw.PopulateWorkingFile_NOT_ATMs("01", SourceTable_A, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                Mtw.PopulateWorkingFile_ATMs_Working01_NOT_ATMs_But_POS("01", SourceTable_A, WMatchingCateg, WRMCycle);

            if (Mtw.Count > 0 & StartFromThree == false)
            {
                RecordsToMatch = true;

            }

            if (WTestingWorkingFiles == false)
                //Mtw.PopulateWorkingFile_NOT_ATMs("02", SourceTable_B, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                Mtw.PopulateWorkingFile_ATMs_V02_NO_ATM_But_POS("02", SourceTable_B, WMatchingCateg, WRMCycle);

            if (NumberOfTables == 3)
            {
                // Insert Records In Working File03

                if (WTestingWorkingFiles == false)
                    //   Mtw.PopulateWorkingFile_NOT_ATMs("03", SourceTable_C, WMatchingCategoryId, WRMCycle, MinMaxPrimaryMatchingField);
                    Mtw.PopulateWorkingFile_ATMs_V02_NO_ATM_But_POS("03", SourceTable_C, WMatchingCateg, WRMCycle);

                if (Mtw.Count > 0 & StartFromThree == true)
                {
                    RecordsToMatch = true;

                }

            }

            if (RecordsToMatch == false) return;
            ///

            //
            // Call to update source files 
            // 
            // Update SourceTable_A with Processed = 1 and RMCycle
            // 
            // We mark all as matched . After this process if unmatched are found are marked accordingly
            // 

            Mcsf.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);
            NumberOfTables = Mcsf.TotalRecords;

            Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameA);
            string T_SourceTable_A = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameB);
            string T_SourceTable_B = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            Msourcef.ReadReconcSourceFilesByFileId(Mcsf.SourceFileNameC);
            string T_SourceTable_C = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msourcef.InportTableName + "]";

            if (WOrigin == "Our Atms")
            {
                // Master will be updated during matching 
                // Third file is the primary. Based on its records the Mpa is updated 
                if (POS_Type == true & StartFromThree == true)
                {
                    // The file must be marked as processed
                    Mgt.UpdateFullTableAsProcessed(WMatchingCateg, T_SourceTable_C,
                                                                           WRMCycle);
                }
            }
            else
            {
                // Origin of the initial Transaction is not from Our ATMs
                string MatchMask = "";
                if (NumberOfTables == 3)
                {
                    MatchMask = "111";
                }
                if (NumberOfTables == 2)
                {
                    MatchMask = "11";
                }

                // UPDATE Mpa - 
                Mpa.UpdateMpaRecordsAsProcessed_NO_ATM_But_POS(WMatchingCateg, WRMCycle, MatchMask, 1);

            }

            // Update SourceTable_B with Processed = 1 and RMCycle

            // THE OTHER FILES ARE MARKED AS PROCESSED AT THE POINT OF MATCHING

            Message = "Txns added in Working Files for Matching Category :"
                                          + InMatchingCateg;
            //}

            #endregion
        }
        string Type;
        // Fields of Working files 
        private void WorkingTableFields_MOBILE(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];
            MatchingCateg = (string)rdr["MatchingCateg"];
            //RMCycle = (int)rdr["RMCycle"];
            RRNumber = (string)rdr["RRNumber"];
            // Type = (string)rdr["Type"]; // type of Mobile 
            //  Senderscheme = (string)rdr["Senderscheme"];
            //SenderTelephone = (string)rdr["Sendernumber"];
            //   Receivernumber = (string)rdr["Receivernumber"];
            TransDate = (DateTime)rdr["TransDate"];
            //   Net_TransDate = (DateTime)rdr["Net_TransDate"];
            TransCurr = (string)rdr["TransCurr"];
            TransAmount = (decimal)rdr["TransAmount"];

            ResponseCode = (string)rdr["ResponseCode"];

        }
        // Read Fields From Universal Table 
        private void ReadFieldsInTableUNIVERSAL(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            MatchingCateg = (string)rdr["MatchingCateg"];

            TerminalId = (string)rdr["TerminalId"];

            TransType = (int)rdr["TransType"];
            TransDescr = (string)rdr["TransDescr"];
            CardNumber = (string)rdr["CardNumber"];

            AccNo = (string)rdr["AccNo"];
            TransCurr = (string)rdr["TransCurr"];
            TransAmount = (decimal)rdr["TransAmt"];
            AmtFileBToFileC = (decimal)rdr["AmtFileBToFileC"];

            FullDtTm = TransDate = (DateTime)rdr["TransDate"];
            TraceNo = (int)rdr["TraceNo"];
            RRNumber = (string)rdr["RRNumber"];

            ResponseCode = (string)rdr["ResponseCode"];

            RMCycle = (int)rdr["ProcessedAtRMCycle"];


            //EXTERNAL_DATE = (DateTime)rdr["EXTERNAL_DATE"];
            //Net_TransDate = (DateTime)rdr["Net_TransDate"];
            //Card_Encrypted = (string)rdr["Card_Encrypted"];
        }

        // TRUNCATE WORKING TABLES
        private void Method_TruncateWorkingTables_MOBILE(int InNumberOfTables)
        {
            // Initialise File 01
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_01]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);
            // Initialise File 02
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_02]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);

            if (InNumberOfTables == 3)
            {
                // Initialise File 03
                TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_MOBILE_03]";

                if (WTestingWorkingFiles == false)
                    Mtw.TruncateInWorkingFile(TableId);
            }

        }

        // Make Matching 
        private void MakeMatchingByGroupOrCategory(string InOperator, string InSignedId, string InMatchingCateg, string InReconcCateg, int InRMCycle, int InAtmsReconcGroup)
        {

            #region Make Matching for this Group which includes all of its ATMs

            //

            //  MAKE MATCHING AND Update a record per ATM + Rcs (Reconciliation Category)
            //

            Message = " Category : " + InMatchingCateg + Environment.NewLine
                            + " Group : " + InAtmsReconcGroup.ToString() + Environment.NewLine
                            + " MATCHING PROCESS WILL START FOR THIS GROUP  ";

            //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

            if (ShowMessage == true & Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(Message);
            }

            // ******************************************************************
            // MAKE MATCHING , Create Report, Save Discrepancies After Summarisation
            //*******************************************************************

            if (NumberOfTables == 3 & POS_Type == false)
            {
                MakeMatchingOf_3_Working_Tables(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
            }
            if (NumberOfTables == 3 & POS_Type == true & StartFromThree == false)
            {
                MakeMatchingOf_3_Working_Tables_POS(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
            }
            if (NumberOfTables == 3 & POS_Type == true & StartFromThree == true)
            {
                MakeMatchingOf_3_Working_Tables_POS_StartFromThree(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
            }
            //
            if (NumberOfTables == 2 & POS_Type == false)
            {
               
                bool PanicosW = true; 
                if (PanicosW == true)
                {
                    MakeMatchingOf_2_Working_Tables_NEW(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
                }
                else
                {
                   // MakeMatchingOf_2_Working_Tables(WOperator, WSignedId,
                           //                                  InMatchingCateg, InReconcCateg,
                               //                              WRMCycle);

                }
            }
            if (NumberOfTables == 2 & POS_Type == true)
            {
                MakeMatchingOf_2_Working_Tables_POS(WOperator, WSignedId,
                                                              InMatchingCateg, InReconcCateg,
                                                              WRMCycle);
            }
            // *****************************************************************************
            // Final Message  
            // *****************************************************************************
            Message = "Matching completed for GROUP:"
                                                      + WAtmsReconcGroup.ToString();

            //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

            if (ShowMessage == true & Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(Message);
            }



            #endregion


            #region Make Mathing for this Category - No coming from our ATMs



            #endregion
        }

        // Updating after Matching 
        private void UpdatingAfterMatching_Mode_2(string InOperator, string InMatchingCateg, string InReconcCateg)
        {
            // *********************************************************************
            // Update a Reconciliation record per ATM + Rcs (Reconciliation Category)
            // ******************************************************************

            SelectionCriteria = "WHERE "
                                + " MatchingCateg='" + InMatchingCateg + "'"
                                + " AND IsMatchingDone = 1"
                                + " AND MatchingAtRMCycle =" + WRMCycle;

            ReadUnMatchedTxnsMasterPoolATMsTotals_AND_Update_Mode_2(WOperator, WRMCycle, SelectionCriteria);


            // *****************************************************************************
            // Update [tblMatchingTxnsMasterPoolATMs] mask card number with full card number 
            // *****************************************************************************
            //  Check Parameters if wish from cardnumber masked to full 
            // 

            //string ParId = "266";
            //string OccurId = "1";
            //RRDMGasParameters Gp = new RRDMGasParameters();
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //if (Gp.OccuranceNm == "YES")
            //{
            //    // IF YES THEN UPDATE MASK CARD WITH FULL 

            //    if (NumberOfTables == 2)
            //    {
            //        // Second File = Destination File ... SourceTable_B
            //        UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_B);

            //    }

            //    if (NumberOfTables == 3)
            //    {
            //        // Second File = Destination File ... SourceTable_C
            //        UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(InMatchingCateg, WRMCycle, SourceTable_A, SourceTable_C);
            //    }

            //}
        }
        //
        int NumberOfDublicates;
        int NumberOfUnmatched;
        string MatchingFields;
        int WPosA;
        int WPosB; // If NOT FOUND IN THIS POSITION

        string TableX;
        string TableY;

        string ListMatchingFields;
        string OnMatchingFields;
        //
        // TABLE UNMATCHED FIELDS DEFINITION
        //
        private void UnmatchedTableFieldsDefinition()
        {
            TableUnMatched.Columns.Add("UserId", typeof(string));
            TableUnMatched.Columns.Add("OriginSeqNo", typeof(int));
            TableUnMatched.Columns.Add("TransDate", typeof(DateTime));

            TableUnMatched.Columns.Add("WCase", typeof(string));
            TableUnMatched.Columns.Add("M_Type", typeof(int)); // MATCHING TYPE
            TableUnMatched.Columns.Add("DublInPos", typeof(int));

            TableUnMatched.Columns.Add("InPos", typeof(int));
            TableUnMatched.Columns.Add("NotInPos", typeof(int));

            //TableUnMatched.Columns.Add("TransactionId", typeof(string));

            TableUnMatched.Columns.Add("RRNumber", typeof(string));

            TableUnMatched.Columns.Add("SenderTelephone", typeof(string));
            TableUnMatched.Columns.Add("TransAmount", typeof(decimal));
            TableUnMatched.Columns.Add("MatchingCateg", typeof(string));

            TableUnMatched.Columns.Add("RMCycle", typeof(int));
            TableUnMatched.Columns.Add("FileId", typeof(string));
            TableUnMatched.Columns.Add("Matched_Characters", typeof(string));

            TableUnMatched.Columns.Add("FullDtTm", typeof(DateTime));
            TableUnMatched.Columns.Add("ResponseCode", typeof(string));
        }

        //*************************************
        // MAKE MATCHING FOR THREE WORKING FILES FOR NON POS TYPE
        //*************************************
        public void MakeMatchingOf_3_Working_Tables
            (string InOperator, string InSignedId,
                  string InMatchingCateg, string InReconcCateg,
                                        int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;
            // Table Fields Definition
            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            int WPos;


            // Dublicates 
            // Dublicates in A : 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            // F
            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //// Dublicates in B : Switch
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // Dublicates in C : BANKING File 
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "Dublicate In File03";
            WPos = 3;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");

            ListMatchingFields = Mf.Dublicate_List_FieldsStageX + ", FullDtTm";
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX + " AND  y.FullDtTm = dt.FullDtTm";

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified  :"
                                                  + NumberOfDublicates;

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Do Matching based on the created working files

            // STAGE A
            // In A and Not In B
            // In A and Not In C
            // In B and Not In A 

            // STAGE B
            // In B and Not In C
            // In C and Not In B

            // In C and Not In A

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            //  FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // *******************************
            // In A and Not In C 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "In File01 And Not In File03";
            WPosA = 1;
            WPosB = 3; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            //  FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // ******************************
            // In B and Not In A
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File02 And Not In File01";
            WPosA = 2;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            //  FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // ******************************
            // In B and Not In C
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "In File02 And Not In File03";
            WPosA = 2;
            WPosB = 3; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            MatchingFields = Mf.MatchingFieldsStageX;

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // ******************************
            // In C and Not In B
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File03 And Not In File02";

            WPosA = 3;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //// ******************************
            //// In C and Not In A
            /// NOT NEEDED AS IF A=B and B=C then A=C
            //// ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File03 And Not In File01";

            WPosA = 3;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);


            //
            // SHOW MESSAGES FOR UNMATCHED
            //
            if (NumberOfUnmatched > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;

                Message = "UNMATCHED IDENTIFIED :"
                                                  + NumberOfUnmatched.ToString();

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }
            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records ";

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 

            Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);

            //
            //Insert Records For Report WReport97_ForMatching
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching_MOBILE]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 238" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "604");
                }


            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies


            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {

                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC]";

                        // DONOT CHANGE 
                        //else
                        //{
                        //    // EMR case 
                        //    s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies]";
                        //}


                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 239" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "605");
                }
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {

                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    if (NumberOfUnmatched > 2000)
                    {
                        MessageBox.Show("There are high number of discrepancies!.." + Environment.NewLine
                            + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                            + "Number of discrepancies : " + NumberOfUnmatched.ToString()
                            );
                    }
                    ReadExceptionsAndCompressAndUpdatefor_3_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                string Message1 = "UNMATCHED IDENTIFIED : "
                                                 + NumberOfUnmatched.ToString();
                string Message2 = "DUBLICATE IDENTIFIED : "
                                                 + NumberOfDublicates.ToString();

                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(WReconcCategoryId + Environment.NewLine
                                                         + Message1 + Environment.NewLine
                                                          + Message2 + Environment.NewLine
                                                          );
                }



            }

            #endregion
        }

        //*************************************
        // MAKE MATCHING FOR THREE WORKING FILES FOR POS TYPE
        //*************************************
        public void MakeMatchingOf_3_Working_Tables_POS
            (string InOperator, string InSignedId,
                  string InMatchingCateg, string InReconcCateg,
                                        int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;
            // Table Fields Definition
            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            //string WCase;

            // IF POS DUBLICATES ARE ONLY CHECKED IN PRIMARY FILE 

            // Dublicates 
            // Dublicates in A : Journal
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);



            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified  :"
                                                  + NumberOfDublicates;

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Do Matching based on the created working files

            // STAGE A
            // In A and Not In B
            // DO NOT CHECK : In B and Not In A 

            // STAGE B
            // DO NOT CHECK : In B and Not In C
            // DO NOT CHECK : In C and Not In B
            // In A and Not In C

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //*****************************
            // UPDATE FILE B RECORDS AS PROCESSED
            TableId = SourceTable_B; // This IS IST

            Mgt.UpdateSourceTablesAsProcessed_Records_POS_Type("02", WMatchingCateg, TableId, WRMCycle, MatchingFields);

            //******************************

            // ******************************
            // In A and Not In C
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "In File01 And Not In File03";
            WPosA = 1;
            WPosB = 3; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            MatchingFields = Mf.MatchingFieldsStageX;

            //  FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //*****************************
            // UPDATE FILE C RECORDS AS PROCESSED
            TableId = SourceTable_C; // This Flexcube

            Mgt.UpdateSourceTablesAsProcessed_Records_POS_Type("03", WMatchingCateg, TableId, WRMCycle, MatchingFields);

            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records till now. ";
            }
            //*******************************
            // FIND IF IN IST , IT it is in File A(123 say) but the record is either reverse or in Error
            //*******************************

            WPosA = 2; // In Position 2 
            WPosB = 1; // If NOT FOUND IN THIS POSITION
            FindExistInTableX_AND_TableY(SourceTable_B, "In File02 And Not In File01",
                WPosA, WPosB, MatchingFields, InMatchingCateg, WRMCycle);

            //*******************************
            // FIND IF IN FLEXCUBE AND REVERSE OR ERROR IN File A
            //*******************************
            WPosA = 3; // In Position 3 
            WPosB = 1; // If NOT FOUND IN THIS POSITION
            FindExistInTableX_AND_TableY(SourceTable_C, "In File03 And Not In File01",
                WPosA, WPosB, MatchingFields, InMatchingCateg, WRMCycle);

            //******************************
            // FIND NOT MATCHED FOR DAYS
            //******************************
            //******************************

            // Find Limit date
            // Work with Cycles
            // If say is one then find previous Cycle date
            // Anything Less or Equal to Previous then is an Exception
            DateTime LimitDate = NullPastDate;
            if (UnMatchedForWorkingDays > 0)
            {

                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                Rjc.ReadPastJobCyclesByInCounter(WOperator, WJobCategory, UnMatchedForWorkingDays);
                if (Rjc.LimitFound == true)
                {
                    LimitDate = Rjc.ReadPastJobCyclesByInCounter(WOperator, WJobCategory, UnMatchedForWorkingDays);

                    TableId = SourceTable_B;
                    WCase = "In File02 And Not In File01";

                    WPosA = 2;
                    WPosB = 1; // If NOT FOUND IN THIS POSITION

                    ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);

                    TableId = SourceTable_C;
                    WCase = "In File03 And Not In File01";

                    WPosA = 3;
                    WPosB = 1; // If NOT FOUND IN THIS POSITION

                    ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);

                }

            }            // 
                         // Do the same for Calindar dates
                         //
            if (UnMatchedForCalendarDays > 0)
            {
                LimitDate = DateTime.Today.AddDays(-UnMatchedForCalendarDays).Date;

                TableId = SourceTable_B;
                WCase = "In File02 And Not In File01";

                WPosA = 2;
                WPosB = 1; // If NOT FOUND IN THIS POSITION

                ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);

                TableId = SourceTable_C;
                WCase = "In File03 And Not In File01";

                WPosA = 3;
                WPosB = 1; // If NOT FOUND IN THIS POSITION

                ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);


            }



            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records ";

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 

            Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);

            //
            //Insert Records For Report WReport97_ForMatching
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 238" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "604");
                }


            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies

            //
            //Insert Records For MatchingDiscrepancies_BDC - Level 1
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 239" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "605");
                }
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {

                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    if (NumberOfUnmatched > 2000)
                    {
                        MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                            + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                            + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                            );
                    }
                    ReadExceptionsAndCompressAndUpdatefor_3_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                string Message1 = "UNMATCHED IDENTIFIED : "
                                                 + NumberOfUnmatched.ToString();
                string Message2 = "DUBLICATE IDENTIFIED : "
                                                 + NumberOfDublicates.ToString();

                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(WReconcCategoryId + Environment.NewLine
                                                         + Message1 + Environment.NewLine
                                                          + Message2 + Environment.NewLine
                                                          );
                }

                // 
                //*****************************
                // UPDATE FILE MASTER POS FOR THE DISCREPANCIES

                //******************************
                if (MatchingCateg == PRX + "231" || MatchingCateg == PRX + "233")
                {
                    string FilePOS = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";
                    Mgt.UpdateMaster_POS_Mpa_Mask(WMatchingCateg, FilePOS,
                                        WRMCycle);
                    // Make as Settled the ones with '111' the rest are made as not settled
                    string WMask = "111";
                    Mgt.UpdateMaster_POS_WithSettled(WMatchingCateg, FilePOS,
                                                                       WRMCycle, WMask);
                }

            }



            #endregion
        }
        //*************************************
        // MAKE MATCHING FOR THREE WORKING FILES FOR POS TYPE
        //*************************************
        public void MakeMatchingOf_2_Working_Tables_POS
            (string InOperator, string InSignedId,
                  string InMatchingCateg, string InReconcCateg,
                                        int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;
            // Table Fields Definition
            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            //string WCase;

            // IF POS DUBLICATES ARE ONLY CHECKED IN PRIMARY FILE 

            // Dublicates 
            // Dublicates in A : Journal
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);


            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified  :"
                                                  + NumberOfDublicates;

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Do Matching based on the created working files

            // STAGE A - No other stage 
            // In A and Not In B
            // DO NOT CHECK : In B and Not In A 

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;
            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);


            //*****************************
            // UPDATE FILE B RECORDS AS PROCESSED
            TableId = SourceTable_B; // This IS IST

            Mgt.UpdateSourceTablesAsProcessed_Records_POS_Type("02", WMatchingCateg, TableId, WRMCycle, MatchingFields);

            //******************************

            //// ******************************
            //// In A and Not In C
            //// ******************************

            //TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            //TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //WCase = "In File01 And Not In File03";
            //WPosA = 1;
            //WPosB = 3; // If NOT FOUND IN THIS POSITION

            //Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            //MatchingFields = Mf.MatchingFieldsStageX;

            //FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            ////*****************************
            //// UPDATE FILE C RECORDS AS PROCESSED
            //TableId = SourceTable_C; // This Flexcube

            //Mgt_BDC.UpdateSourceTablesAsProcessed_Records_POS_Type("03", WMatchingCateg, TableId, WRMCycle, MatchingFields);

            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records till now. ";
            }
            //*******************************
            //******************************
            // FIND NOT MATCHED FOR DAYS
            //******************************
            //******************************
            TableId = SourceTable_B;
            WCase = "In File02 And Not In File01";

            WPosA = 2;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            //**************************************
            //*******************************
            // FIND IF IN IST , IT it is in File A(123 say) but the record is either reverse or in Error
            //*******************************
            //********************

            WPosA = 2; // In Position 2 
            WPosB = 1; // If NOT FOUND IN THIS POSITION
            FindExistInTableX_AND_TableY(SourceTable_B, "In File02 And Not In File01",
                WPosA, WPosB, MatchingFields, InMatchingCateg, WRMCycle);



            //******************************
            // FIND NOT MATCHED FOR DAYS
            //******************************
            //******************************

            // Find Limit date
            // Work with Cycles
            // If say is one then find previous Cycle date
            // Anything Less or Equal to Previous then is an Exception


            DateTime LimitDate = NullPastDate;

            if (UnMatchedForWorkingDays > 0)
            {

                RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
                Rjc.ReadPastJobCyclesByInCounter(WOperator, WJobCategory, UnMatchedForWorkingDays);
                if (Rjc.LimitFound == true)
                {
                    LimitDate = Rjc.ReadPastJobCyclesByInCounter(WOperator, WJobCategory, UnMatchedForWorkingDays);

                    TableId = SourceTable_B;
                    WCase = "In File02 And Not In File01";

                    WPosA = 2;
                    WPosB = 1; // If NOT FOUND IN THIS POSITION

                    ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);

                }

            }            // 
                         // Do the same for Calindar dates
                         //
            if (UnMatchedForCalendarDays > 0)
            {
                LimitDate = DateTime.Today.AddDays(-UnMatchedForCalendarDays).Date;

                TableId = SourceTable_B;
                WCase = "In File02 And Not In File01";

                WPosA = 2;
                WPosB = 1; // If NOT FOUND IN THIS POSITION

                ExistInTableButNotMatchedForDays_Non_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, LimitDate);

            }



            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records ";

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 

            Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);

            //
            //Insert Records For Report WReport97_ForMatching
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching_MOBILE]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        s.BulkCopyTimeout = 350;
                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 238" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "604");
                }


            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies

            //
            //Insert Records For MatchingDiscrepancies_BDC - Level 1
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());
                        s.BulkCopyTimeout = 350;
                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 239" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "605");
                }
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {

                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    if (NumberOfUnmatched > 2000)
                    {
                        MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                            + "Reconciliation Matching Category:.. " + WReconcCategoryId + Environment.NewLine
                            + "Number of discrepancies : " + NumberOfUnmatched.ToString()
                            );
                    }
                    ReadExceptionsAndCompressAndUpdatefor_2_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                string Message1 = "UNMATCHED IDENTIFIED : "
                                                 + NumberOfUnmatched.ToString();
                string Message2 = "DUBLICATE IDENTIFIED : "
                                                 + NumberOfDublicates.ToString();

                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(WReconcCategoryId + Environment.NewLine
                                                         + Message1 + Environment.NewLine
                                                          + Message2 + Environment.NewLine
                                                          );
                }

                // 
                //*****************************
                // UPDATE FILE MASTER POS FOR THE DISCREPANCIES

                //******************************
                if (MatchingCateg == PRX + "231")
                {
                    string FilePOS = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";
                    Mgt.UpdateMaster_POS_Mpa_Mask(WMatchingCateg, FilePOS,
                                        WRMCycle);

                    string WMASK = "11";
                    // Make as Settled the ones with '111' the rest are made as not settled
                    Mgt.UpdateMaster_POS_WithSettled(WMatchingCateg, FilePOS,
                                                                       WRMCycle, WMASK);
                }

            }



            #endregion
        }
        //*************************************
        // MAKE MATCHING FOR THREE WORKING FILES FOR POS TYPE AND Start From Three
        //*************************************
        public void MakeMatchingOf_3_Working_Tables_POS_StartFromThree
            (string InOperator, string InSignedId,
                  string InMatchingCateg, string InReconcCateg,
                                        int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;
            // Table Fields Definition
            UnmatchedTableFieldsDefinition();

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            //string WCase;

            // IF POS DUBLICATES ARE ONLY CHECKED IN PRIMARY FILE 

            // Dublicates 
            // Dublicates in 3 : File coming from Master ATMs = Category = BDC206
            TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            WCase = "Dublicate In File03";
            WPos = 3;

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            ////// Dublicates in B : Switch
            //TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            //WCase = "Dublicate In File02";
            //WPos = 2;

            //Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage A");
            //ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            //OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            //FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            //// Dublicates in C : BANKING File 
            //TableId = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //WCase = "Dublicate In File03";
            //WPos = 3;

            //Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            //ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            //OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            //FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields);

            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified  :"
                                                  + NumberOfDublicates;

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Do Matching based on the created working files
            // CHECK 
            // STAGE B
            // In C and Not In B
            // In C and Not In A

            // *******************************
            // In C and Not In B
            // *******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            WCase = "In File03 And Not In File02";
            WPosA = 3;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            MatchingFields = Mf.MatchingFieldsStageX;

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //*****************************
            // UPDATE FILE B RECORDS THAT ARE Matched AS PROCESSED
            TableId = SourceTable_B; // This IS IST

            Mgt.UpdateSourceTablesAsProcessed_OurAtms_But_POS_ThirdFile("02", WMatchingCateg, TableId, WRMCycle, MatchingFields);
            //******************************

            // ******************************
            // In C and Not In A
            // ******************************

            TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            WCase = "In File03 And Not In File01";
            WPosA = 3;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX(InMatchingCateg, "Stage B");
            MatchingFields = Mf.MatchingFieldsStageX;

            // FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            //*****************************
            // UPDATE FILE A RECORDS AS PROCESSED
            TableId = SourceTable_A; // This Master Pool 

            Mgt.UpdateSourceTablesAsProcessed_OurAtms_But_Master_ThirdFile("01", WMatchingCateg, TableId, WRMCycle, MatchingFields);

            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records till now. ";
            }
            //*******************************
            //******************************
            // FIND NOT MATCHED FOR DAYS in Master Pool 
            //******************************
            //******************************
            //TableId = SourceTable_A;
            //WCase = "In File01 And Not In File03";

            //WPosA = 1;
            //WPosB = 3; // If NOT FOUND IN THIS POSITION
            //DateTime WLimitDate = new DateTime(2019, 07, 02);
            //ExistInTableButNotMatchedForDays_Our_ATMS(TableId, WMatchingCateg, WCase, WPosA, WPosB, WRMCycle, WLimitDate);

            // Repeat this to show in 2 and not in 1 same call
            //TableId = SourceTable_B;

            //WCase = "In File02 And Not In File01";

            //WPosA = 2;
            //WPosB = 1; // If NOT FOUND IN THIS POSITION

            //ExistInTableButNotMatchedForDays(TableId, WMatchingCategoryId, WCase, WPosA, WPosB, WRMCycle);


            //// ******************************
            //// In C and Not In B
            //// ******************************

            //TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_02]";
            //WCase = "In File03 And Not In File02";

            //WPosA = 3;
            //WPosB = 2; // If NOT FOUND IN THIS POSITION

            //FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);

            //// ******************************
            //// In C and Not In A
            //// NOT NEEDED AS IF A = B and B = C then A = C
            //// ******************************

            //TableX = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_03]";
            //TableY = "[RRDM_Reconciliation_ITMX].[dbo].[WMatchingTbl_01]";
            //WCase = "In File03 And Not In File01";

            //WPosA = 3;
            //WPosB = 1; // If NOT FOUND IN THIS POSITION

            //FindNotExistAddTableTableXToTableY(TableX, TableY, WCase, WPosA, WPosB, MatchingFields);


            ////
            //// SHOW MESSAGES FOR UNMATCHED
            ////
            //if (NumberOfUnmatched > 0)
            //{
            //    NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;

            //    Message = "UNMATCHED IDENTIFIED :"
            //                                      + NumberOfUnmatched.ToString();

            //    Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCategoryId, DateTime.Now, DateTime.Now, Message);
            //    if (ShowMessage == true & Environment.UserInteractive)
            //    {
            //        System.Windows.Forms.MessageBox.Show(Message);
            //    }

            //}
            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records ";

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

            #region Create Report

            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
            // RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

            //Clear Table 

            Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);

            //
            //Insert Records For Report WReport97_ForMatching
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching_MOBILE]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 238" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "604");
                }


            #endregion

            #region Saved Dicrepancies And Summarise (compress) Discrepancies

            //
            //Insert Records For MatchingDiscrepancies_BDC - Level 1
            //
            using (SqlConnection conn2 =
                           new SqlConnection(connectionStringATMs))
                try
                {
                    conn2.Open();

                    using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                    {
                        s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC]";

                        foreach (var column in TableUnMatched.Columns)
                            s.ColumnMappings.Add(column.ToString(), column.ToString());

                        s.WriteToServer(TableUnMatched);
                    }
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 239" + Environment.NewLine;
                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "605");
                }
            // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {
                TableUnMatchedSaved = TableUnMatched;
            }

            if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
            {

                // UPDATE Mpa with unmatched records
                if (NumberOfUnmatched > 0)
                {
                    if (NumberOfUnmatched > 2000)
                    {
                        MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                            + "Reconciliation Matching Category:.. " + WReconcCategoryId + Environment.NewLine
                            + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                            );
                    }
                    ReadExceptionsAndCompressAndUpdatefor_3_Tables();
                }

                if (NumberOfDublicates > 0)
                {
                    // Dublicate 
                    ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                }

                //
                // CREATE COPRESSED TABLE
                // 

                ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);

                string Message1 = "UNMATCHED IDENTIFIED : "
                                                 + NumberOfUnmatched.ToString();
                string Message2 = "DUBLICATE IDENTIFIED : "
                                                 + NumberOfDublicates.ToString();

                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(WReconcCategoryId + Environment.NewLine
                                                         + Message1 + Environment.NewLine
                                                          + Message2 + Environment.NewLine
                                                          );
                }

                // 
                //*****************************
                // UPDATE FILE MASTER POS FOR THE DESCREPANCIES

                //******************************
                if (MatchingCateg == PRX + "231")
                {
                    string FilePOS = "[RRDM_Reconciliation_ITMX].[dbo].[MASTER_CARD_POS]";
                    Mgt.UpdateMaster_POS_Mpa_Mask(WMatchingCateg, FilePOS,
                                        WRMCycle);
                    string WMASK = "111";
                    // Make as Settled the ones with '111' the rest are made as not settled
                    Mgt.UpdateMaster_POS_WithSettled(WMatchingCateg, FilePOS,
                                                                       WRMCycle, WMASK);
                }

            }



            #endregion
        }

        //*************************************
        // MAKE MATCHING FOR TWO WORKING FILES 
        //*************************************
        //public void MakeMatchingOf_2_Working_Tables
        //  (string InOperator, string InSignedId,
        //        string InMatchingCateg, string InReconcCateg,
        //                              int InRMCycle)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    NumberOfDublicates = 0;
        //    NumberOfUnmatched = 0;

        //    // MOBILE LIKE ETISALAT
        //    // MOBILE LIKE QAHERA                                       

        //    #region UnMatched Table Definition

        //    TableUnMatched = new DataTable();
        //    TableUnMatched.Clear();
        //    TotalSelected = 0;

        //    UnmatchedTableFieldsDefinition();

        //    if (IS_Matching_At_SET_DATE == true)
        //    {
        //        TotalSelected = TotalSelected;
        //        // ETI375
        //    }
        //    if (InMatchingCateg == "ETI310")
        //    {
        //        MatchingCateg = MatchingCateg;
        //    }

        //    #endregion

        //    #region Find dublicates 

        //    //string FileId;
        //    int WPos;

        //    string FileIdA;
        //    string FileIdB;
        //    //string WCase;

        //    // Dublicates 
        //    // Dublicates in A : 
        //    TableId = SourceTable_A;
        //    WCase = "Dublicate In File01";
        //    WPos = 1;

        //    Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
        //    ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
        //    OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

        //    FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

        //    // Dublicates in B : 
        //    TableId = SourceTable_B;
        //    WCase = "Dublicate In File02";
        //    WPos = 2;

        //    Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
        //    ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
        //    OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

        //    FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

        //    if (NumberOfDublicates > 0)
        //    {
        //        NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
        //        Message = "Dublicates identified :"
        //                                          + NumberOfDublicates.ToString();

        //        // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
        //        if (ShowMessage == true & Environment.UserInteractive)
        //        {
        //            System.Windows.Forms.MessageBox.Show(Message);
        //        }
        //    }
        //    if (NumberOfDublicates == 0)
        //    {
        //        Message = "No Dublicates identified ";

        //        //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
        //        if (ShowMessage == true & Environment.UserInteractive)
        //        {
        //            System.Windows.Forms.MessageBox.Show(Message);
        //        }
        //    }

        //    #endregion

        //    #region Do Matching based on the TABLES

        //    // *******************************
        //    // In A and Not In B 
        //    // *******************************

        //    TableX = SourceTable_A;
        //    TableY = SourceTable_B;
        //    WCase = "In File01 And Not In File02";
        //    WPosA = 1;
        //    WPosB = 2; // If NOT FOUND IN THIS POSITION

        //    Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
        //    MatchingFields = Mf.MatchingFieldsStageX;

        //    //MatchingFields = "c2.TransAmount = c1.TransAmount AND  c2.RRNumber = c1.RRNumber ";
        //    FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE, WRMCycle);

        //    // ******************************
        //    // In B and Not In A
        //    // ******************************

        //    TableX = SourceTable_B;
        //    TableY = SourceTable_A;
        //    WCase = "In File02 And Not In File01";
        //    WPosA = 2;
        //    WPosB = 1; // If NOT FOUND IN THIS POSITION

        //    Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
        //    MatchingFields = Mf.MatchingFieldsStageX;

        //    FindNotExistAddTableTableXToTableY_MOBILE(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE, WRMCycle);

        //    // HERE YOU HAVE IDENTIFY ALL DISCREPANCIES SET ALL RECORDS TO MATCHED
        //    // AFTER BASED ON MISSMATCHED 
        //    string MatchMask = "";

        //    MatchMask = "11";

        //    // UPDATE Master

        //    TableId = SourceTable_A;
        //    Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

        //    // Update SourceTable_B with Processed = 1 and RMCycle
        //    //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
        //    TableId = SourceTable_B;

        //    Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

        //    //*****************************************

        //    if (NumberOfUnmatched > 0 || NumberOfDublicates > 0)
        //    {
        //        NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;
        //        Message = "UnMatched identified : "
        //                                          + NumberOfUnmatched.ToString();

        //        //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
        //        if (ShowMessage == true & Environment.UserInteractive)
        //        {
        //            System.Windows.Forms.MessageBox.Show(Message);
        //        }


        //        #region Create Report

        //        ////ReadTable And Insert In Sql Table 
        //        RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
        //        //  RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

        //        //Clear Table 
        //        Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);
        //        int number = TableUnMatched.Columns.Count;
        //        //Insert Records For WReport97_ForMatching
        //        using (SqlConnection conn2 =
        //                       new SqlConnection(connectionStringATMs))
        //            try
        //            {
        //                conn2.Open();

        //                using (SqlBulkCopy s = new SqlBulkCopy(conn2))
        //                {
        //                    s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching_MOBILE]";

        //                    foreach (var column in TableUnMatched.Columns)
        //                        s.ColumnMappings.Add(column.ToString(), column.ToString());

        //                    s.WriteToServer(TableUnMatched);
        //                }
        //                conn2.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                conn2.Close();

        //                W_MPComment = W_MPComment + "Process has been cancelled stage 241" + Environment.NewLine;

        //                W_MPComment += DateTime.Now;

        //                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

        //                CatchDetails(ex, "606");
        //            }

        //        #endregion

        //        #region Saved Dicrepancies And Summarise (compress) Discrepancies
        //        //Insert Records For MatchingDiscrepancies_BDC
        //        //using (SqlConnection conn2 =
        //        //               new SqlConnection(connectionStringATMs))
        //        //    try
        //        //    {
        //        //        conn2.Open();

        //        //        using (SqlBulkCopy s = new SqlBulkCopy(conn2))
        //        //        {
        //        //            s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_MOBILE]";

        //        //            foreach (var column in TableUnMatched.Columns)
        //        //                s.ColumnMappings.Add(column.ToString(), column.ToString());

        //        //            s.WriteToServer(TableUnMatched);
        //        //        }
        //        //        conn2.Close();

        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        conn2.Close();
        //        //        W_MPComment = W_MPComment + "Process has been cancelled stage 242" + Environment.NewLine;

        //        //        W_MPComment += DateTime.Now;

        //        //        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

        //        //        CatchDetails(ex, "607");
        //        //    }
        //        //
        //        // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
        //        //
        //        if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
        //        {
        //            TableUnMatchedSaved = TableUnMatched;
        //        }
        //        //
        //        // COMPRESS FOUND CASES
        //        //
        //        if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
        //        {
        //            // UPDATE Mpa with unmatched records
        //            if (NumberOfUnmatched > 0)
        //            {
        //                if (NumberOfUnmatched > 2000)
        //                {
        //                    MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
        //                        + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
        //                        + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
        //                        );
        //                }
        //                ReadExceptionsAndCompressAndUpdatefor_2_Tables();
        //            }

        //            if (NumberOfDublicates > 0)
        //            {
        //                // Dublicate 
        //                ReadDublicatesAndCompressAndUpdateExceptionsforTables();
        //            }

        //            //
        //            // CREATE COPRESSED TABLE
        //            // 

        //            ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);


        //        }
        //        #endregion

        //    }
        //    if (NumberOfUnmatched == 0)
        //    {
        //        Message = "No UnMatched Records found. ";

        //        //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

        //        if (ShowMessage == true & Environment.UserInteractive)
        //        {
        //            System.Windows.Forms.MessageBox.Show(Message);
        //        }

        //    }

        //    #endregion

        //}

        //*************************************
        // MAKE MATCHING FOR TWO WORKING FILES 
        //*************************************
        int TempCount1 = 0;
        int TempCount2 = 0;
        int TempCount3 = 0;
        int NumberOfRecords; 
        public void MakeMatchingOf_2_Working_Tables_NEW
          (string InOperator, string InSignedId,
                string InMatchingCateg, string InReconcCateg,
                                      int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // Truncate Tables
            NumberOfTables = 2; 
            Method_TruncateWorkingTables(NumberOfTables,W_Application);

            // Copy tables to working

            TempCount1 = 0;
            TempCount2 = 0;
            TempCount3 = 0;
            // 
            // Insert Records In File01Y from Mpa
            // This is the master table 
            TempCount1 = Mtw.PopulateWorkingFile_ATMs_Working01_WALLET("01", SourceTable_A, WMatchingCateg, WRMCycle, W_Application);
            // Insert Records In File02Y from IST
            NumberOfRecords = TempCount1; 

            //InsertRecordsInWorkingFile_02(connectionString, WMatchingCategoryId, WAtmNo, MinMaxTrace, WRMCycle);
            TempCount2 = Mtw.PopulateWorkingFile_ATMs_Working01_WALLET("02", SourceTable_B, WMatchingCateg, WRMCycle, W_Application);

            string SourceTable_A_W = "["+ W_Application + "].[dbo].[WMatchingTbl_01]"; 
            string SourceTable_B_W = "["+ W_Application + "].[dbo].[WMatchingTbl_02]";


            NumberOfDublicates = 0;
            NumberOfUnmatched = 0;

            // MOBILE LIKE ETISALAT
            // MOBILE LIKE QAHERA                                       

            #region UnMatched Table Definition

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();
            TotalSelected = 0;

            UnmatchedTableFieldsDefinition();

            if (IS_Matching_At_SET_DATE == true)
            {
                TotalSelected = TotalSelected;
                // ETI375
            }
            if (InMatchingCateg == "ETI360")
            {
                MatchingCateg = MatchingCateg;
            }

            #endregion

            #region Find dublicates 

            //string FileId;
            int WPos;

            string FileIdA;
            string FileIdB;
            //string WCase;

            // Dublicates 
            // Dublicates in A : 
            TableId = SourceTable_A_W;
            WCase = "Dublicate In File01";
            WPos = 1;

            Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            // Dublicates in B : 
            TableId = SourceTable_B_W; 
            WCase = "Dublicate In File02";
            WPos = 2;

            Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
            ListMatchingFields = Mf.Dublicate_List_FieldsStageX;
            OnMatchingFields = Mf.Dublicate_ON_FieldsStageX;

            FindDuplicateAddTable(TableId, WCase, WPos, ListMatchingFields, OnMatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE);

            if (NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfDublicates;
                Message = "Dublicates identified :"
                                                  + NumberOfDublicates.ToString();

                // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }
            if (NumberOfDublicates == 0)
            {
                Message = "No Dublicates identified ";

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }
            }

            #endregion

            #region Do Matching based on the TABLES

            // *******************************
            // In A and Not In B 
            // *******************************

            TableX = SourceTable_A_W;
            TableY = SourceTable_B_W;
            WCase = "In File01 And Not In File02";
            WPosA = 1;
            WPosB = 2; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            //MatchingFields = "c2.TransAmount = c1.TransAmount AND  c2.RRNumber = c1.RRNumber ";
            FindNotExistAddTableTableXToTableY_MOBILE_NEW(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE, WRMCycle);

            // ******************************
            // In B and Not In A
            // ******************************

            TableX = SourceTable_B_W;
            TableY = SourceTable_A_W;
            WCase = "In File02 And Not In File01";
            WPosA = 2;
            WPosB = 1; // If NOT FOUND IN THIS POSITION

            Mf.CreateStringOfMatchingFieldsForStageX_ABS(InMatchingCateg, "Stage A");
            MatchingFields = Mf.MatchingFieldsStageX;

            FindNotExistAddTableTableXToTableY_MOBILE_NEW(TableX, TableY, WCase, WPosA, WPosB, MatchingFields, InMatchingCateg, IS_Matching_At_SET_DATE, WRMCycle);

            // HERE YOU HAVE IDENTIFY ALL DISCREPANCIES SET ALL RECORDS TO MATCHED
            // AFTER BASED ON MISSMATCHED 
            string MatchMask = "";

            MatchMask = "11";

            // UPDATE Master

            TableId = SourceTable_A;
            Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

            // Update SourceTable_B with Processed = 1 and RMCycle
            //FileId = "[RRDM_Reconciliation_ITMX].[dbo].[IntblBankSwitchTxns]"; 
            TableId = SourceTable_B;

            Mmob.UpdateSourceTablesAsProcessed_MOBILE_Based_ON_ALL(WMatchingCateg, TableId, WRMCycle, MatchMask);

            //*****************************************

            if (NumberOfUnmatched > 0 || NumberOfDublicates > 0)
            {
                NumberOfUnmatchedForCategory = NumberOfUnmatchedForCategory + NumberOfUnmatched;
                Message = "UnMatched identified : "
                                                  + NumberOfUnmatched.ToString();

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }


                #region Create Report

                ////ReadTable And Insert In Sql Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();
                //  RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();

                //Clear Table 
                Tr.DeleteWReport97_ForMatching_MOBILE(WSignedId, WMatchingCateg);
                int number = TableUnMatched.Columns.Count;
                //Insert Records For WReport97_ForMatching
                using (SqlConnection conn2 =
                               new SqlConnection(connectionStringATMs))
                    try
                    {
                        conn2.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport97_ForMatching_MOBILE]";

                            foreach (var column in TableUnMatched.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(TableUnMatched);
                        }
                        conn2.Close();
                    }
                    catch (Exception ex)
                    {
                        conn2.Close();

                        W_MPComment = W_MPComment + "Process has been cancelled stage 241" + Environment.NewLine;

                        W_MPComment += DateTime.Now;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                        CatchDetails(ex, "606");
                    }

                #endregion

                #region Saved Dicrepancies And Summarise (compress) Discrepancies
                //Insert Records For MatchingDiscrepancies_BDC
                //using (SqlConnection conn2 =
                //               new SqlConnection(connectionStringATMs))
                //    try
                //    {
                //        conn2.Open();

                //        using (SqlBulkCopy s = new SqlBulkCopy(conn2))
                //        {
                //            s.DestinationTableName = "[RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_MOBILE]";

                //            foreach (var column in TableUnMatched.Columns)
                //                s.ColumnMappings.Add(column.ToString(), column.ToString());

                //            s.WriteToServer(TableUnMatched);
                //        }
                //        conn2.Close();

                //    }
                //    catch (Exception ex)
                //    {
                //        conn2.Close();
                //        W_MPComment = W_MPComment + "Process has been cancelled stage 242" + Environment.NewLine;

                //        W_MPComment += DateTime.Now;

                //        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                //        CatchDetails(ex, "607");
                //    }
                //
                // SAVE TABLE BECAUSE IT WILL USE FOR OTHER PURPOSE
                //
                if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
                {
                    TableUnMatchedSaved = TableUnMatched;
                }
                //
                // COMPRESS FOUND CASES
                //
                if (NumberOfDublicates > 0 || NumberOfUnmatched > 0)
                {
                    // UPDATE Mpa with unmatched records
                    if (NumberOfUnmatched > 0)
                    {
                        if (NumberOfUnmatched > 2000)
                        {
                            MessageBox.Show("There are high number of discrepancies!" + Environment.NewLine
                                + "Reconciliation Matching Category: " + WReconcCategoryId + Environment.NewLine
                                + "Number of discrepancies :.. " + NumberOfUnmatched.ToString()
                                );
                        }
                        ReadExceptionsAndCompressAndUpdatefor_2_Tables();
                    }

                    if (NumberOfDublicates > 0)
                    {
                        // Dublicate 
                        ReadDublicatesAndCompressAndUpdateExceptionsforTables();
                    }

                    //
                    // CREATE COPRESSED TABLE
                    // 

                    ReadMpaAndFindDiscrepanciesfromMatching_Cards(MatchingCateg, RMCycle);


                }
                #endregion

            }
            if (NumberOfUnmatched == 0)
            {
                Message = "No UnMatched Records found. ";

                //Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                if (ShowMessage == true & Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show(Message);
                }

            }

            #endregion

        }

        // TRUNCATE WORKING TABLES
        private void Method_TruncateWorkingTables(int InNumberOfTables, string W_Application)
        {
            // Initialise File 01
            TableId = "["+ W_Application + "].[dbo].[WMatchingTbl_01]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);
            // Initialise File 02
            TableId = "[" + W_Application + "].[dbo].[WMatchingTbl_02]";

            if (WTestingWorkingFiles == false)
                Mtw.TruncateInWorkingFile(TableId);

            if (InNumberOfTables == 3)
            {
                // Initialise File 03
                TableId = "[" + W_Application + "].[dbo].[WMatchingTbl_03]";

                if (WTestingWorkingFiles == false)
                    Mtw.TruncateInWorkingFile(TableId);
            }

        }

        //
        // Methods 
        // Find Duplicate file X 
        // FILL UP A TABLE
        // GROUP OF ATMS / Category 
        public void FindDuplicateAddTable(string InFileId, string InCase, int InPos, string InListMatchingFields,
            string IN_OnMatchingFields, string InMatchingCateg, bool IS_Matching_At_SET_DATE)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            string connectionString = "";
            if (W_Application == "ETISALAT")
            {
                connectionString = AppConfig.GetConnectionString("ETISALATConnectionString");
            }
            if (W_Application == "QAHERA")
            {
                connectionString = AppConfig.GetConnectionString("QAHERAConnectionString");
            }


            if (IS_Matching_At_SET_DATE == true)
            {
                SqlString =
       " SELECT * "
       //+ " y.SeqNo, y.MatchingCateg, y.RMCycle, y.TerminalId, y.TraceNo, y.AccNo, y.TransAmt, y.TransDate "
       + " FROM " + InFileId + " y"
       + " INNER JOIN "
       + " ( "
       + " SELECT "
       + InListMatchingFields + " , COUNT(*) AS CountOf"
       //  + " TerminalId, TraceNo, AccNo,  TransAmt, TransDate, COUNT(*) AS CountOf"
       + " FROM  " + InFileId
       + " WHERE MatchingCateg ='" + InMatchingCateg + "'  "
                       + " AND IsMatchingDone = 0 and ResponseCode = 0 and SET_DATE <> '1900-01-01' "
       + " GROUP BY " + InListMatchingFields
       + " HAVING COUNT(*)>1 "
       + " ) dt"
       + " ON " + IN_OnMatchingFields
       //+ " ON y.TerminalId=dt.TerminalId AND y.TraceNo= dt.TraceNo AND y.AccNo=dt.AccNo "
       //     + " AND y.TransAmt=dt.TransAmt AND y.TransDate =dt.TransDate "
       + " ";
            }
            else
            {
                SqlString =
       " SELECT * "
       //+ " y.SeqNo, y.MatchingCateg, y.RMCycle, y.TerminalId, y.TraceNo, y.AccNo, y.TransAmt, y.TransDate "
       + " FROM " + InFileId + " y"
       + " INNER JOIN "
       + " ( "
       + " SELECT "
       + InListMatchingFields + " , COUNT(*) AS CountOf"
       //  + " TerminalId, TraceNo, AccNo,  TransAmt, TransDate, COUNT(*) AS CountOf"
       + " FROM  " + InFileId
       + " WHERE MatchingCateg ='" + InMatchingCateg + "'  "
                       + " AND IsMatchingDone = 0 and ResponseCode = 0 "
       + " GROUP BY " + InListMatchingFields
       + " HAVING COUNT(*)>1 "
       + " ) dt"
       + " ON " + IN_OnMatchingFields
       //+ " ON y.TerminalId=dt.TerminalId AND y.TraceNo= dt.TraceNo AND y.AccNo=dt.AccNo "
       //     + " AND y.TransAmt=dt.TransAmt AND y.TransDate =dt.TransDate "
       + " ";
            }


            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);
                            SeqNo = (int)rdr["SeqNo"];
                            MatchingCateg = (string)rdr["MatchingCateg"];

                            RRNumber = (string)rdr["RRNumber"];

                            TransDate = (DateTime)rdr["TransDate"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfDublicates = NumberOfDublicates + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["WCase"] = InCase;
                            RowSelected["M_Type"] = 1; // 1 for dublicates 
                            RowSelected["DublInPos"] = InPos;
                            RowSelected["InPos"] = 0;
                            RowSelected["NotInPos"] = 0;

                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["TransAmount"] = TransAmount;

                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = WRMCycle;
                            RowSelected["FileId"] = "";
                            if (MatchingCateg == "ETI360" || MatchingCateg == "QAH360" || MatchingCateg == "IPN360")
                            {
                                RowSelected["Matched_Characters"] = RRNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = RRNumber + TransAmount.ToString();
                            }

                            RowSelected["ResponseCode"] = ResponseCode;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);

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

                    W_MPComment = W_MPComment + "Process has been cancelled stage 263" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "608");

                }
        }



        // Find Exist In A But Not In B 
        public void FindNotExistAddTableTableXToTableY_MOBILE(string InFileIdA,
                                string InFileIdB, string InCase, int InPos, int NotInPos, string InMatchingFields, string InMatchingCateg, bool IS_Matching_At_SET_DATE, int InWRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal Tolerance = 0; 

            // Find TOLERANCE For Difference 
            if (InMatchingCateg == "ETI375")
            {
                Gp.ReadParametersSpecificId(WOperator, "511", "1", "", ""); // 
                if (Gp.RecordFound == true)
                {
                    Tolerance = (decimal)Gp.Amount;
                }
                else
                {
                   Tolerance = 0.1000M;
                }
            }

            string MatchingString = Mf.CreateStringOfMatchingFieldsForStage_String(WMatchingCateg, "Stage A");

            string TraceOrRRN;

            //  InMatchingFields = InMatchingFields + " AND c2.MatchingCateg = c1.MatchingCateg ='" + InMatchingCateg + "'"; 

            TotalSelected = 0;

            //and SET_DATE <> '1900-01-01 00:00:00'

            if (IS_Matching_At_SET_DATE == true)
            {
                SqlString =
        " SELECT c2.RRNumber,c1.*, c1.RRNumber as RRN "
        + "  FROM " + InFileIdA + " c1 "
         + " Left join "
              + InFileIdB + " c2 "
              + " ON " + InMatchingFields
              + " AND c1.MatchingCateg = c2.MatchingCateg "
              + " AND c1.SET_DATE = c2.SET_DATE "
              + " AND c1.LoadedAtRMCycle =  c2.LoadedAtRMCycle "
              + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "'"
             + "   AND c1.IsReversal = 0  and c1.ResponseCode = '0' and c1.SET_DATE <> '1900-01-01' "
              + " AND c1.IsMatchingDone = 0 "
              + " AND ISNULL(c2.IsMatchingDone,0) = 0  " // YOU CHECK THIS BECAUSE IF MISSING IS null
                + " AND ISNULL(c2.IsReversal,0) = 0 "
              //  + " AND c2.ResponseCode = '0'  " Not to include this 
              + " AND c2.RRNumber IS NULL "; // yOU WANT THESE THAT DO NOT EXIST 
            }
            else
            {
                SqlString =
        " SELECT c2.RRNumber,c1.*, c1.RRNumber as RRN "
        + "  FROM " + InFileIdA + " c1 "
         + " Left join "
              + InFileIdB + " c2 "
              + " ON " + InMatchingFields
              + " AND c1.MatchingCateg = c2.MatchingCateg "
              + " AND c1.LoadedAtRMCycle =  c2.LoadedAtRMCycle "
              + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "'"
             //+ " AND c1.LoadedAtRMCycle =" + InWRMCycle
             //+ " AND c2.LoadedAtRMCycle =" + InWRMCycle
             + "   AND c1.IsReversal = 0  and c1.ResponseCode = '0' "
              + " AND c1.IsMatchingDone = 0 "
              + " AND ISNULL(c2.IsMatchingDone,0) = 0  " // YOU CHECK THIS BECAUSE IF MISSING IS null
                + " AND ISNULL(c2.IsReversal,0) = 0 "
              // + " AND c2.ResponseCode = '0'  " NOT to include this
              + " AND c2.RRNumber IS NULL "; // yOU WANT THESE THAT DO NOT EXIST 
            }

            //SqlString =

            //" SELECT * , " + MatchingString + " As WMatchingString "
            // + " FROM " + InFileIdA + " c1"
            // + " WHERE NOT EXISTS(SELECT * "
            // + " FROM " + InFileIdB + " c2 "
            // + " WHERE " + InMatchingFields + ")";
            string connectionString = ""; 
            if (W_Application == "ETISALAT")
            {
                connectionString = AppConfig.GetConnectionString("ETISALATConnectionString");
            }
            if (W_Application == "QAHERA")
            {
                connectionString = AppConfig.GetConnectionString("QAHERAConnectionString");
            }


            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);

                            TransAmount = (decimal)rdr["TransAmount"];

                            RRNumber = (string)rdr["RRN"];
                            decimal SecondTransAmount;

                            if (InMatchingCateg == "ETI375")
                            {
                                string SelectionCriteria = " WHERE RRNumber='" + RRNumber + "'";
                                SecondTransAmount = Mmob.ReadTransactionAmountFromTable(SelectionCriteria, InFileIdB, W_Application);

                                if (SecondTransAmount !=0)
                                {
                                    // Second Record exist and amount is different  
                                    if (InPos == 2)
                                    {
                                        // THIS WAS FOUND WHEN We had checked the A to B file
                                        // We prefer not to register it again
                                        continue; 
                                    }

                                    // HERE We continue only for cases A to B or InPos ==1 

                                    //var valOne = Decimal.Round(1.1234560M, 6);    // Gives 1.123456
                                    //var valTwo = Decimal.Round(1.1234569M, 6);    // Gives 1.123457
                                    //var Dif = Math.Abs(TransAmount - SecondTransAmount);
                                    if (Math.Abs(TransAmount - SecondTransAmount) >= Tolerance)
                                    // if (Math.Abs(TransAmount - SecondTransAmount) >= 0.100M)
                                    {
                                        // Do nothing and continue to register it as discrepancy 

                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                         
                                
                            }

                           

                            SeqNo = (int)rdr["SeqNo"];
                            MatchingCateg = (string)rdr["MatchingCateg"];


                            TransDate = (DateTime)rdr["TransDate"];

                            TransCurr = (string)rdr["TransCurr"];
                           // TransAmount = (decimal)rdr["TransAmount"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfUnmatched = NumberOfUnmatched + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["WCase"] = InCase;
                            RowSelected["M_Type"] = 4; // Not In 
                            RowSelected["DublInPos"] = 0;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = NotInPos;

                            RowSelected["RRNumber"] = RRNumber;

                            //if (RRNumber ==
                            //    "63fcbcf3-257e-4492-beb7-eb84557d3e22")
                            //{
                            //    RRNumber = RRNumber;
                            //}

                            RowSelected["TransAmount"] = TransAmount;

                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = WRMCycle;
                            RowSelected["FileId"] = "";
                            if (MatchingCateg == "ETI360")
                            {
                                RowSelected["Matched_Characters"] = RRNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = RRNumber + TransAmount.ToString();
                            }

                            RowSelected["ResponseCode"] = ResponseCode;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);


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

                    W_MPComment = W_MPComment + "Process has been cancelled stage 244" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "610");

                }
        }

        public void FindNotExistAddTableTableXToTableY_MOBILE_NEW(string InFileIdA,
                                string InFileIdB, string InCase, int InPos, int NotInPos, string InMatchingFields, string InMatchingCateg, bool IS_Matching_At_SET_DATE, int InWRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            decimal Tolerance = 0;

            // Find TOLERANCE For Difference 
            if (InMatchingCateg == "ETI375")
            {
                Gp.ReadParametersSpecificId(WOperator, "511", "1", "", ""); // 
                if (Gp.RecordFound == true)
                {
                    Tolerance = (decimal)Gp.Amount;
                }
                else
                {
                    Tolerance = 0.1000M;
                }
            }

            string MatchingString = Mf.CreateStringOfMatchingFieldsForStage_String(WMatchingCateg, "Stage A");

            string TraceOrRRN;

            //  InMatchingFields = InMatchingFields + " AND c2.MatchingCateg = c1.MatchingCateg ='" + InMatchingCateg + "'"; 

            TotalSelected = 0;

            //and SET_DATE <> '1900-01-01 00:00:00'
            // Copy InFileA to WORKING TABLE 

            if (IS_Matching_At_SET_DATE == true)
            {
                // This MUST BE DEALT IN LOADING OF WORKING TABLES
        //        SqlString =
        //" SELECT c2.RRNumber,c1.*, c1.RRNumber as RRN "
        //+ "  FROM " + InFileIdA + " c1 "
        // + " Left join "
        //      + InFileIdB + " c2 "
        //      + " ON " + InMatchingFields
        //      + " AND c1.MatchingCateg = c2.MatchingCateg "
        //      + " AND c1.SET_DATE = c2.SET_DATE "
        //      + " AND c1.LoadedAtRMCycle =  c2.LoadedAtRMCycle "
        //      + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "'"
        //     + "   AND c1.IsReversal = 0  and c1.ResponseCode = '0' and c1.SET_DATE <> '1900-01-01' "
        //      + " AND c1.IsMatchingDone = 0 "
        //      + " AND ISNULL(c2.IsMatchingDone,0) = 0  " // YOU CHECK THIS BECAUSE IF MISSING IS null
        //        + " AND ISNULL(c2.IsReversal,0) = 0 "
        //      //  + " AND c2.ResponseCode = '0'  " Not to include this 
        //      + " AND c2.RRNumber IS NULL "; // yOU WANT THESE THAT DO NOT EXIST 
            }
            else
            {
        //        SqlString =
        //" SELECT c2.RRNumber,c1.*, c1.RRNumber as RRN "
        //+ "  FROM " + InFileIdA + " c1 "
        // + " Left join "
        //      + InFileIdB + " c2 "
        //      + " ON " + InMatchingFields
        //      + " AND c1.MatchingCateg = c2.MatchingCateg "
        //      + " AND c1.LoadedAtRMCycle =  c2.LoadedAtRMCycle "
        //      + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "'"
        //     //+ " AND c1.LoadedAtRMCycle =" + InWRMCycle
        //     //+ " AND c2.LoadedAtRMCycle =" + InWRMCycle
        //     + "   AND c1.IsReversal = 0  and c1.ResponseCode = '0' "
        //      + " AND c1.IsMatchingDone = 0 "
        //      + " AND ISNULL(c2.IsMatchingDone,0) = 0  " // YOU CHECK THIS BECAUSE IF MISSING IS null
        //        + " AND ISNULL(c2.IsReversal,0) = 0 "
        //      // + " AND c2.ResponseCode = '0'  " NOT to include this
        //      + " AND c2.RRNumber IS NULL "; // yOU WANT THESE THAT DO NOT EXIST 
            }

            // WORKING TABLES ARE CLEAR AND READY TO DO THE BELOW INSTRUCTIONS 

            SqlString =

            " SELECT * , " + MatchingString + " As WMatchingString "
             + " FROM " + InFileIdA + " c1"
             + " WHERE NOT EXISTS(SELECT * "
             + " FROM " + InFileIdB + " c2 "
             + " WHERE " + InMatchingFields + ")";

            string connectionString = "";
            if (W_Application == "ETISALAT")
            {
                connectionString = AppConfig.GetConnectionString("ETISALATConnectionString");
            }
            if (W_Application == "QAHERA")
            {
                connectionString = AppConfig.GetConnectionString("QAHERAConnectionString");
            }

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //WorkingTableFields_MOBILE(rdr);

                            TransAmount = (decimal)rdr["TransAmount"];

                            RRNumber = (string)rdr["RRNumber"];

                            decimal SecondTransAmount;

                            if (InMatchingCateg == "ETI375")
                            {
                                string SelectionCriteria = " WHERE RRNumber='" + RRNumber + "'";
                                SecondTransAmount = Mmob.ReadTransactionAmountFromTable(SelectionCriteria, InFileIdB, W_Application);

                                if (SecondTransAmount != 0)
                                {
                                    // Second Record exist and amount is different  
                                    if (InPos == 2)
                                    {
                                        // THIS WAS FOUND WHEN We had checked the A to B file
                                        // We prefer not to register it again
                                        continue;
                                    }

                                    // HERE We continue only for cases A to B or InPos ==1 

                                    //var valOne = Decimal.Round(1.1234560M, 6);    // Gives 1.123456
                                    //var valTwo = Decimal.Round(1.1234569M, 6);    // Gives 1.123457
                                    //var Dif = Math.Abs(TransAmount - SecondTransAmount);
                                    if (Math.Abs(TransAmount - SecondTransAmount) >= Tolerance)
                                    // if (Math.Abs(TransAmount - SecondTransAmount) >= 0.100M)
                                    {
                                        // Do nothing and continue to register it as discrepancy 

                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }


                            }


                            SeqNo = (int)rdr["SeqNo"];
                            MatchingCateg = (string)rdr["MatchingCateg"];


                            TransDate = (DateTime)rdr["TransDate"];

                            TransCurr = (string)rdr["TransCurr"];
                            // TransAmount = (decimal)rdr["TransAmount"];

                            ResponseCode = (string)rdr["ResponseCode"];

                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfUnmatched = NumberOfUnmatched + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["WCase"] = InCase;
                            RowSelected["M_Type"] = 4; // Not In 
                            RowSelected["DublInPos"] = 0;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = NotInPos;

                            RowSelected["RRNumber"] = RRNumber;

                            //if (RRNumber ==
                            //    "63fcbcf3-257e-4492-beb7-eb84557d3e22")
                            //{
                            //    RRNumber = RRNumber;
                            //}

                            RowSelected["TransAmount"] = TransAmount;

                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = WRMCycle;
                            RowSelected["FileId"] = "";
                            if (MatchingCateg == "ETI360")
                            {
                                RowSelected["Matched_Characters"] = RRNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = RRNumber + TransAmount.ToString();
                            }

                            RowSelected["ResponseCode"] = ResponseCode;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);


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

                    W_MPComment = W_MPComment + "Process has been cancelled stage 244" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "610");

                }
        }

        // Find Exist In A AND Exist In B 
        // To be Checked for Reversals and Errors
        //
        public void FindExistInTableX_AND_TableY(string InFileIdA,
                                string InCase, int InPos, int NotInPos
                                     , string InMatchingFields, string InMatchingCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();
            Msf.ReadReconcCategoriesVsSourcesAll(InMatchingCateg);
            string InFileIdB = "[RRDM_Reconciliation_ITMX].[dbo]." + Msf.SourceFileNameA; // This is 123 say

            string MatchingString = Mf.CreateStringOfMatchingFieldsForStage_String(WMatchingCateg, "Stage A");

            string TraceOrRRN;

            TotalSelected = 0;

            SqlString =
            //+" AND Processed = 0 AND ResponseCode = '0' "
            " SELECT * , " + MatchingString + " As WMatchingString "
             + " FROM " + InFileIdA + " c1"
             + " WHERE EXISTS (SELECT * "
             + " FROM " + InFileIdB + " c2 "
            // + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Egypt_123_NET] c2"
             + " WHERE c1.MatchingCateg ='" + InMatchingCateg + "' AND c2.MatchingCateg = c1.MatchingCateg AND c1.Processed = 0 AND c1.ResponseCode = '0' AND c2.Mask = '' AND "
             + InMatchingFields + " )";

            using (SqlConnection conn =
                      new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            //return; 

                            //WorkingTableFields(rdr);
                            SeqNo = (int)rdr["SeqNo"];
                            MatchingCateg = (string)rdr["MatchingCateg"];
                            RMCycle = InRMCycle;
                            TerminalId = (string)rdr["TerminalId"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = (string)rdr["TransDescr"];
                            CardNumber = (string)rdr["CardNumber"];
                            AccNo = (string)rdr["AccNo"];
                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmt"];
                            AmtFileBToFileC = (decimal)rdr["AmtFileBToFileC"];
                            TransDate = (DateTime)rdr["TransDate"];
                            TraceNo = (int)rdr["TraceNo"];
                            RRNumber = (string)rdr["RRNumber"];
                            FullDtTm = (DateTime)rdr["TransDate"];
                            ResponseCode = (string)rdr["ResponseCode"];
                            string WMatchingString = (string)rdr["WMatchingString"];
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfUnmatched = NumberOfUnmatched + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate;

                            RowSelected["WCase"] = InCase;
                            RowSelected["Type"] = 4; // Not In 
                            RowSelected["DublInPos"] = 0;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = NotInPos;
                            RowSelected["TerminalId"] = TerminalId;

                            RowSelected["TraceNo"] = TraceNo;
                            // Not From Our ATMs 
                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["TransAmt"] = TransAmount;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = RMCycle;
                            RowSelected["FileId"] = TableId;
                            if (IsFrom_TPF == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmount.ToString() + CardNumber;
                            }
                            else
                            {

                                RowSelected["Matched_Characters"] = WMatchingString;
                                //RowSelected["Matched_Characters"] = TransDate.Date.ToString() + RRNumber + TransAmt.ToString() + CardNumber;
                            }

                            RowSelected["FullDtTm"] = FullDtTm;
                            RowSelected["ResponseCode"] = ResponseCode;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);

                            //bool T_Processed = true;
                            //Mgt.UpdateProcessedBySeqNo(InFileIdA, SeqNo, T_Processed, InRMCycle);

                            // Update IST and Flexcube as Processed. 

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

                    W_MPComment = W_MPComment + "Process has been cancelled stage 244" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "610");

                }
        }
        //
        // Not Matched for days
        //
        public void ExistInTableButNotMatchedForDays_Non_Our_ATMS(string InTableId, string InMatchingCateg,
                                       string InCase, int InPos, int NotInPos, int InRMCycle, DateTime InLimitDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string TraceOrRRN;

            TotalSelected = 0;


            SqlString =
            //    
            " SELECT * "
             + " FROM " + InTableId
             + " WHERE MatchingCateg = @MatchingCateg And Cast(TransDate as Date) <= @TransDate "
             + " AND Processed = 0 AND ResponseCode = '0' "
             + " ";

            using (SqlConnection conn =
                      new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@TransDate", InLimitDate.Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTableUNIVERSAL(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = TableUnMatched.NewRow();

                            NumberOfUnmatched = NumberOfUnmatched + 1;

                            RowSelected["UserId"] = WSignedId;
                            RowSelected["OriginSeqNo"] = SeqNo;
                            RowSelected["TransDate"] = TransDate.Date;

                            RowSelected["WCase"] = InCase;
                            RowSelected["M_Type"] = 4; // Not In 
                            RowSelected["DublInPos"] = 0;
                            RowSelected["InPos"] = InPos;
                            RowSelected["NotInPos"] = NotInPos;
                            RowSelected["TerminalId"] = TerminalId;

                            RowSelected["TraceNo"] = TraceNo;
                            // Not From Our ATMs 
                            RowSelected["RRNumber"] = RRNumber;

                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNo"] = AccNo;
                            RowSelected["TransAmt"] = TransAmount;
                            RowSelected["MatchingCateg"] = MatchingCateg;
                            RowSelected["RMCycle"] = InRMCycle;
                            RowSelected["FileId"] = TableId;
                            if (IsFrom_TPF == true)
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmount.ToString() + CardNumber;
                            }
                            else
                            {
                                RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + RRNumber + TransAmount.ToString() + CardNumber;
                            }

                            RowSelected["FullDtTm"] = FullDtTm;
                            RowSelected["ResponseCode"] = ResponseCode;

                            // ADD ROW
                            TableUnMatched.Rows.Add(RowSelected);

                            //bool T_Processed = true;
                            //Mgt.UpdateProcessedBySeqNo(InTableId, SeqNo, T_Processed, InRMCycle);


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

                    W_MPComment = W_MPComment + "Process has been cancelled stage 244" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "610");

                }
        }

        ////
        //// Not Matched for days in Master pool 
        ////
        //public void ExistInTableButNotMatchedForDays_Our_ATMS(string InTableId, string InMatchingCateg,
        //                               string InCase, int InPos, int NotInPos, int InRMCycle, DateTime InLimitDate)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string TraceOrRRN;

        //    TotalSelected = 0;

        //    SqlString =
        //    //    
        //    " SELECT * "
        //     + " FROM " + InTableId
        //     + " WHERE MatchingCateg = @MatchingCateg And TransDate< @LimitDate "
        //     + " AND IsMatchingDone = 0 "
        //     + " ";

        //    using (SqlConnection conn =
        //              new SqlConnection(connectionStringATMs))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                cmd.Parameters.AddWithValue("@LimitDate", InLimitDate);


        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {
        //                    RecordFound = true;

        //                    ReadFieldsInTableUNIVERSAL(rdr);
        //                    //
        //                    // Fill In Table
        //                    //
        //                    DataRow RowSelected = TableUnMatched.NewRow();

        //                    NumberOfUnmatched = NumberOfUnmatched + 1;

        //                    RowSelected["UserId"] = WSignedId;
        //                    RowSelected["OriginSeqNo"] = SeqNo;
        //                    RowSelected["TransDate"] = TransDate.Date;

        //                    RowSelected["WCase"] = InCase;
        //                    RowSelected["Type"] = 4; // Not In 
        //                    RowSelected["DublInPos"] = 0;
        //                    RowSelected["InPos"] = InPos;
        //                    RowSelected["NotInPos"] = NotInPos;
        //                    RowSelected["TerminalId"] = TerminalId;

        //                    RowSelected["TraceNo"] = TraceNo;
        //                    // Not From Our ATMs 
        //                    RowSelected["RRNumber"] = RRNumber;

        //                    RowSelected["CardNumber"] = CardNumber;
        //                    RowSelected["AccNo"] = AccNo;
        //                    RowSelected["TransAmt"] = TransAmount;
        //                    RowSelected["MatchingCateg"] = MatchingCateg;
        //                    RowSelected["RMCycle"] = InRMCycle;
        //                    RowSelected["FileId"] = TableId;
        //                    if (IsFrom_TPF == true)
        //                    {
        //                        RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + TraceNo + TransAmount.ToString() + CardNumber;
        //                    }
        //                    else
        //                    {
        //                        RowSelected["Matched_Characters"] = TerminalId + TransDate.Date.ToString() + RRNumber + TransAmount.ToString() + CardNumber;
        //                    }

        //                    RowSelected["FullDtTm"] = FullDtTm;
        //                    RowSelected["ResponseCode"] = ResponseCode;

        //                    // ADD ROW
        //                    TableUnMatched.Rows.Add(RowSelected);

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

        //            W_MPComment = W_MPComment + "Process has been cancelled stage 244" + Environment.NewLine;

        //            W_MPComment += DateTime.Now;

        //            Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

        //            CatchDetails(ex, "610");

        //        }
        //}


        bool ZeroInOne;
        bool ZeroInTwo;
        bool ZeroInThree;
        int LastTraceNo = 0;
        string LastRRNumber = "";
        string LastMatched_Characters = "";

        string LastTerminalId = "";
        string LastAccNo;
        decimal LastTransAmt;
        int LastType;
        DateTime LastTransDate;

        bool NotSameAccount;
        bool NotSameAmt;

        //    int FirstSeqNo;

        bool FoundInOne;
        bool FoundInTwo;
        bool FoundInThree;
        int SeqNoInOne;
        int SeqNoInTwo;
        int SeqNoInThree;
        int InPos;

        // Read detail umpatched Cases and Summarise them and Update (2  Files) 
        public void ReadExceptionsAndCompressAndUpdatefor_2_Tables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool CheckAccNo = false;

            CountToVisit = 0;

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "717";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES") CheckAccNo = true;

            FoundInOne = false;
            FoundInTwo = false;

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;

            TotalSelected2 = 0;

            SqlString =
            " SELECT  * "
            + " FROM [ATMS].[dbo].[WReport97_ForMatching_MOBILE] "
            + " WHERE M_Type = 4 AND UserId = @UserId AND MatchingCateg ='" + WMatchingCateg + "'"
            + " ORDER By Matched_Characters, WCase, NotInPos ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 245" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "611");
                }


            ////Clear field
            LastMatched_Characters = "";

            int I = 0;
            int K = TableUnMatched.Rows.Count;

            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;

                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                M_Type = (int)TableUnMatched.Rows[I]["M_Type"]; // 
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                //SenderTelephone = (string)TableUnMatched.Rows[I]["SenderTelephone"];

                TransAmount = (decimal)TableUnMatched.Rows[I]["TransAmount"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];

                if (RRNumber == "63fcbcf3-257e-4492-beb7-eb84557d3e22")
                {
                    RRNumber = RRNumber;
                }

                if ((Matched_Characters.Trim() != LastMatched_Characters.Trim())
                    //|| AccNo != LastAccNo
                    //|| TransAmt != LastTransAmt
                    || I == K - 1)
                {
                    //  if (K == 1) FirstSeqNo = OriginSeqNo;

                    //if (CheckAccNo == true)
                    //{
                    //    if (AccNo != LastAccNo & Matched_Characters.Trim() == LastMatched_Characters.Trim())
                    //    {
                    //        NotSameAccount = true;
                    //    }
                    //}

                    //if (TransAmt != LastTransAmt & Matched_Characters.Trim() == LastMatched_Characters.Trim())
                    //{
                    //    NotSameAmt = true;
                    //}

                    if ((LastMatched_Characters.Trim() != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim())) //  
                    {

                        char[] WMask = { '0', '0' };
                        if (FoundInOne == true) WMask[0] = '1';
                        if (FoundInTwo == true) WMask[1] = '1';
                        //if (ZeroInThree == true) WMask[2] = '0';

                        string WWMask = new string(WMask);

                        //if (NotSameAccount == true || NotSameAmt == true)
                        //{

                        //    WWMask = "AA";
                        //    //if (NotSameAccount == true)
                        //    MessageBox.Show("TerminalId = " +TerminalId + Environment.NewLine
                        //        + "LastTerminalId = "+ LastTerminalId + Environment.NewLine
                        //        + "LastAccNo = " + LastAccNo + Environment.NewLine
                        //        + "LastTransAmt  = " + LastTransAmt + Environment.NewLine
                        //        + "");
                        //}

                        // Our ATMs and cards - Normal Cycle
                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                        FoundInOne = false;
                        FoundInTwo = false;
                        SeqNoInOne = 0;
                        SeqNoInTwo = 0;

                        NotSameAccount = false;
                        NotSameAmt = false;

                    }

                    if (I == K - 1) // LAST RECORD
                    {
                        if (InPos == 1)
                        {
                            FoundInOne = true;
                            SeqNoInOne = OriginSeqNo;
                        }
                        if (InPos == 2)
                        {
                            FoundInTwo = true;
                            SeqNoInTwo = OriginSeqNo;
                        }

                        LastMatched_Characters = Matched_Characters;
                        LastTransDate = TransDate;
                        string LastTransactionId = TransactionId;
                        LastTransAmt = TransAmount;
                        //  LastType = Type;

                        char[] WMask = { '0', '0' };
                        if (FoundInOne == true) WMask[0] = '1';
                        if (FoundInTwo == true) WMask[1] = '1';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            WWMask = "AA";
                        }
                        // LAST RECORD

                        // OUR ATMs AND JCC
                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                    }

                    //   if (I != K - 1) FirstSeqNo = OriginSeqNo;
                    LastMatched_Characters = Matched_Characters;
                    LastTransDate = TransDate;
                    string LastTransactionId2 = TransactionId;
                    LastTransAmt = TransAmount;
                    LastType = M_Type;
                }

                if (I != K - 1) // If not LAST RECORD
                {
                    if (InPos == 1)
                    {
                        FoundInOne = true;
                        SeqNoInOne = OriginSeqNo;
                    }
                    if (InPos == 2)
                    {
                        FoundInTwo = true;
                        SeqNoInTwo = OriginSeqNo;
                    }

                }

                I++; // Read Next entry of the table 

            }

            CountToVisit = CountToVisit;


        }


        // Read detail umpatched Cases and Summarise them and Update (3  Tables) 
        public void ReadExceptionsAndCompressAndUpdatefor_3_Tables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool CheckAccNo = false;

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "717";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.OccuranceNm == "YES") CheckAccNo = true;

            FoundInOne = false;
            FoundInTwo = false;
            FoundInThree = false;
            ZeroInOne = false;
            ZeroInTwo = false;
            ZeroInThree = false;

            SeqNoInOne = 0;
            SeqNoInTwo = 0;
            SeqNoInThree = 0;

            NotSameAccount = false;
            NotSameAmt = false;

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;

            TotalSelected2 = 0;

            SqlString =
            " SELECT  * "
            + " FROM [ATMS].[dbo].[WReport97_ForMatching] "
            + " WHERE Type = 4 AND UserId = @UserId "
            + " ORDER By Matched_Characters, WCase, NotInPos ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 246" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "612");
                }

            //////ReadTable And Insert In Sql Table 
            ////Clear field
            LastMatched_Characters = "";

            int I = 0;
            int K = TableUnMatched.Rows.Count;

            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;
                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                M_Type = (int)TableUnMatched.Rows[I]["M_Type"];
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                TerminalId = (string)TableUnMatched.Rows[I]["TerminalId"];
                TraceNo = (int)TableUnMatched.Rows[I]["TraceNo"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                CardNumber = (string)TableUnMatched.Rows[I]["CardNumber"];
                AccNo = (string)TableUnMatched.Rows[I]["AccNo"];
                TransAmount = (decimal)TableUnMatched.Rows[I]["TransAmt"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];
                //string LastAccNo;
                //decimal LastTransAmt;



                if (Matched_Characters.Trim() != LastMatched_Characters.Trim()
                    || AccNo != LastAccNo
                    || TransAmount != LastTransAmt
                    || I == K - 1
                    //|| TerminalId != LastTerminalId
                    )
                {
                    // if (K == 1) FirstSeqNo = SeqNo;

                    // This condition is defined in parameter 717
                    if (CheckAccNo == true)
                    {
                        if (AccNo != LastAccNo & Matched_Characters == LastMatched_Characters)
                        {
                            NotSameAccount = true;
                        }
                    }


                    if (TransAmount != LastTransAmt & Matched_Characters == LastMatched_Characters)
                    {
                        NotSameAmt = true;
                    }

                    if ((LastMatched_Characters != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim())) //  
                    {

                        //if (FoundInOne || FoundInTwo || FoundInThree
                        //                    || ZeroInOne || ZeroInTwo || ZeroInThree
                        //                    & LastTransAmt > 0)
                        //{ }

                        //if (FoundInOne == true & ZeroInOne == true)
                        //{
                        //    ZeroInOne = false;
                        //}
                        //if (FoundInTwo == true & ZeroInTwo == true)
                        //{
                        //    ZeroInTwo = false;
                        //}
                        //if (FoundInThree == true & ZeroInThree == true)
                        //{
                        //    ZeroInThree = false;
                        //}

                        char[] WMask = { '0', '0', '0' };

                        //if (ZeroInOne == false ) WMask[0] = '1';

                        //if (ZeroInTwo == false) WMask[1] = '1';

                        //if (ZeroInThree == false ) WMask[2] = '1';
                        // in 1 and not in 2 = 101
                        // in 1 and not in 3 = 110
                        // in 2 and not 1 and in 3 and not in 1 = 011
                        // ***************
                        if (POS_Type == true & FoundInThree == true)
                        {
                            // This is the case where we test for records that never came to us
                            // We check if Record is in Flexcube and not processed for long time and not in Origin file 
                            ZeroInTwo = true;
                        }

                        if (FoundInOne == true) WMask[0] = '1';

                        if (FoundInOne == false & ZeroInOne == false) WMask[0] = '1'; // Case found in 2 and not in 3
                        if (FoundInOne == false & ZeroInOne == true) WMask[0] = '0'; // Case found in 2 and not in 3

                        if (FoundInTwo == true) WMask[1] = '1';

                        if (FoundInTwo == false & ZeroInTwo == false) WMask[1] = '1'; // 

                        if (FoundInTwo == false & ZeroInTwo == true) WMask[1] = '0'; // 

                        if (FoundInThree == true) WMask[2] = '1';

                        if (FoundInThree == false & ZeroInThree == false) WMask[2] = '1'; // found in two and not 1
                        if (FoundInThree == false & ZeroInThree == true) WMask[2] = '0';                                                              // or found in one and not in two only 

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            if (WWMask == "111") WWMask = "AAA";
                            if (WWMask == "101") WWMask = "A0A";
                            if (WWMask == "011") WWMask = "0AA";
                        }

                        if (POS_Type == true & WWMask == "011")
                        {
                            // Check if exist in two 
                            if (SeqNoInTwo == 0)
                            {
                                WWMask = "001";
                            }
                            if (SeqNoInTwo > 0 & SeqNoInThree == 0 & WWMask == "011")
                            {
                                WWMask = "010";
                            }
                        }

                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                        //
                        // INITIALISE VARIABLES
                        //
                        FoundInOne = false;
                        FoundInTwo = false;
                        FoundInThree = false;

                        ZeroInOne = false;
                        ZeroInTwo = false;
                        ZeroInThree = false;

                        SeqNoInOne = 0;
                        SeqNoInTwo = 0;
                        SeqNoInThree = 0;

                        NotSameAccount = false;
                        NotSameAmt = false;

                    }
                    if (I == K - 1) // LAST RECORD
                    {
                        if (InPos == 1)
                        {
                            // FoundInOne = true;
                            SeqNoInOne = OriginSeqNo;
                        }
                        if (InPos == 2)
                        {
                            //   FoundInTwo = true;
                            SeqNoInTwo = OriginSeqNo;
                        }
                        if (InPos == 3)
                        {
                            //   FoundInThree = true;
                            SeqNoInThree = OriginSeqNo;
                        }

                        if (NotInPos == 1) ZeroInOne = true; // We set Found In one = false to cover cases of differrent amount
                        if (NotInPos == 2) ZeroInTwo = true;
                        if (NotInPos == 3) ZeroInThree = true;

                        if (InPos == 1) FoundInOne = true;
                        if (InPos == 2) FoundInTwo = true;
                        if (InPos == 3) FoundInThree = true;

                      
                        // FirstSeqNo = SeqNo;
                        LastMatched_Characters = Matched_Characters;
                        LastTransDate = TransDate;
                        LastTerminalId = TerminalId;
                        LastAccNo = AccNo;
                        LastTraceNo = TraceNo;
                        LastRRNumber = RRNumber;
                        LastTransAmt = TransAmount;
                        LastType = M_Type;

                        char[] WMask = { '0', '0', '0' };


                        if (FoundInOne == true) WMask[0] = '1';

                        if (FoundInOne == false & ZeroInOne == false) WMask[0] = '1'; // Case found in 2 and not in 3
                        if (FoundInOne == false & ZeroInOne == true) WMask[0] = '0'; // Case found in 2 and not in 3

                        if (FoundInTwo == true) WMask[1] = '1';

                        if (FoundInTwo == false & ZeroInTwo == false) WMask[1] = '1'; // 

                        if (FoundInTwo == false & ZeroInTwo == true) WMask[1] = '0'; // 

                        if (FoundInThree == true) WMask[2] = '1';

                        if (FoundInThree == false & ZeroInThree == false) WMask[2] = '1'; // found in two and not 1
                        if (FoundInThree == false & ZeroInThree == true) WMask[2] = '0';

                        string WWMask = new string(WMask);

                        if (NotSameAccount == true || NotSameAmt == true)
                        {
                            if (WWMask == "111") WWMask = "AAA";
                            if (WWMask == "101") WWMask = "A0A";
                            if (WWMask == "011") WWMask = "0AA";
                        }

                        if (POS_Type == true & WWMask == "011")
                        {
                            // Check if exist in two 
                            if (SeqNoInTwo == 0)
                            {
                                WWMask = "001";
                            }
                            if (SeqNoInTwo > 0 & SeqNoInThree == 0 & WWMask == "011")
                            {
                                WWMask = "010";
                            }
                        }

                        InsertAndUpdateTablesFromOurAtms_AND_JCC(WWMask);

                    }

                    // SAVE PREVIOUS 

                    LastMatched_Characters = Matched_Characters;
                    LastTransDate = TransDate;
                    LastTerminalId = TerminalId;
                    LastAccNo = AccNo;
                    LastTransAmt = TransAmount;
                    LastTraceNo = TraceNo;
                    LastRRNumber = RRNumber;
                    LastType = M_Type;
                }

                if (InPos == 1) FoundInOne = true;
                if (InPos == 2) FoundInTwo = true;
                if (InPos == 3) FoundInThree = true;

                if (NotInPos == 1) ZeroInOne = true; // We set Found In one = false to cover cases of differrent amount
                if (NotInPos == 2) ZeroInTwo = true;
                if (NotInPos == 3) ZeroInThree = true;

                if (I != K - 1) // If not LAST RECORD
                {
                    if (InPos == 1)
                    {
                        //FoundInOne = true;
                        SeqNoInOne = OriginSeqNo;
                    }
                    if (InPos == 2)
                    {
                        //FoundInTwo = true;
                        SeqNoInTwo = OriginSeqNo;
                    }
                    if (InPos == 3)
                    {
                        //FoundInThree = true;
                        SeqNoInThree = OriginSeqNo;
                    }

                }

                I++; // Read Next entry of the table 

            }

        }

        int CountToVisit = 0;
        int CountToVisitInsert = 0;

        // Insert and Update exceptions  
        private void InsertAndUpdateTablesFromOurAtms_AND_JCC(string WWMask)
        {
            // Insert Or Update Footer 
            try
            {

                int SeqOriginRecord = 0;
                string WSelectionCriteria = "";
                bool MasterPOS = false;
                bool IsReversal = false;
                bool Previous = false;
                bool Is_DiffAmt_ETI375 = false;
                decimal DifAmt_EIT375 = 0;
                decimal FirstAmt = 0;
                decimal SecondAmt=0;
                // 
                if (WWMask == "10")
                {
                    if (WWMask.Substring(0, 1) == "1")
                    {
                        // CHECK If Done Previously with 01 and same RRN and Amt

                        TableId = SourceTable_A;

                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInOne;
                        Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, TableId, 1, W_Application);

                        
                        // The record is read

                        if (MatchingCateg == "ETI375")
                        {
                            // Keep the TransAmount of the first record
                            FirstAmt = Mmob.TransAmount; 
                            int Db_Mode = 1; // look only the unmatched and reversals
                            SelectionCriteria = " WHERE MatchingCateg = '" + WMatchingCateg + "'"
                                    + " AND RRNumber ='" + Mmob.RRNumber + "'"
                                    + " AND LoadedAtRMCycle =" + WRMCycle                              
                                    ;
                            // Check if 0 record came eg yesterday 

                            Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, SourceTable_B, Db_Mode, W_Application);
                            if (Mmob.RecordFound)
                            {
                                Is_DiffAmt_ETI375 = true;
                                SecondAmt = Mmob.TransAmount;
                                DifAmt_EIT375 = FirstAmt - SecondAmt; 
                            }
                        }
                        else
                        {
                            int Db_Mode = 1; // look only the unmatched and reversals
                            SelectionCriteria = " WHERE MatchingCateg = '" + WMatchingCateg + "'"
                                    + " AND RRNumber ='" + Mmob.RRNumber + "'"
                                    + " AND TransAmount =" + Mmob.TransAmount
                                    // + " AND CustomerID ='" + Mmob.CustomerID+"'"
                                    + " AND LoadedAtRMCycle >=" + (WRMCycle - 3)
                                    + " AND SeqNo <>" + SeqNoInOne
                                    ;
                            // Check if 0 record came eg yesterday 
                           
                            Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, SourceTable_B, Db_Mode, W_Application);
                            if (Mmob.RecordFound == true)
                            {
                                // Check if reversal 
                                // Check if done before with Mask 01
                                if (Mmob.Matched == false & Mmob.LoadedAtRMCycle < WRMCycle)
                                {
                                    // Previous day we had the record as 01
                                    Previous = true;
                                    Mmob.MatchMask = "N1"; // we update the old record as N1 ... came as Next day 
                                    Mmob.Comments = "Matching Record Came next cycle";
                                    Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(SourceTable_B, Mmob.SeqNo);
                                }
                                if (Mmob.IsReversal == true & Mmob.LoadedAtRMCycle == WRMCycle)
                                {
                                    IsReversal = true;
                                }

                            }

                        }



                        Mmob.IsMatchingDone = true;
                        Mmob.Matched = false;
                        Mmob.MatchMask = WWMask;
                        if (IsReversal == true)
                        {
                            Mmob.MatchMask = "1R";
                            Mmob.Comments = "Record is reversed ";
                        }
                        else
                        {
                            if (Previous == true)
                            {
                                Mmob.Comments = "Matched record appeared previous cycle";
                            }
                        }

                        if (Is_DiffAmt_ETI375 == true)
                        {
                            Mmob.Comments = "The Agent.'"+Mmob.RRNumber+"' Total differs"+Environment.NewLine
                                + "--------------------------" + Environment.NewLine
                                + "Salary Total.:" + FirstAmt.ToString("#,##0.000")+Environment.NewLine
                                + "Agent Total..:" + SecondAmt.ToString("#,##0.000") + Environment.NewLine
                                 + "----------------------------------" + Environment.NewLine
                                 + "----------------------------------" + Environment.NewLine
                                + "Difference...:" + DifAmt_EIT375.ToString("#,##0.000");
                            ; 
                        }

                        Mmob.MatchingAtRMCycle = WRMCycle;

                        Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(TableId, SeqNoInOne);

                    }

                    //
                    // Additional Updating if needed
                    //
                    RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
                    Mcs.ReadReconcCategoriesVsSourcesAll(WMatchingCateg);

                    string TempFileA = Mcs.SourceFileNameA;
                    string TempFileB = Mcs.SourceFileNameB;

                    string TempFrom = "";

                    if (W_Application == "ETISALAT")
                    {
                        TempFrom = "[ETISALAT].[dbo]." + "ETISALAT_TPF_TXNS"; // By default we move them to master
                        TempFileA = "[ETISALAT].[dbo]." + TempFileA;
                        TempFileB = "[ETISALAT].[dbo]." + TempFileB;

                    }
                    if (W_Application == "QAHERA")
                    {
                        TempFrom = "[QAHERA].[dbo]." + "QAHERA_TPF_TXNS";
                        TempFileA = "[QAHERA].[dbo]." + TempFileA;
                        TempFileB = "[QAHERA].[dbo]." + TempFileB;
                    }
                    if (W_Application == "IPN")
                    {
                        TempFrom = "[IPN].[dbo]." + "IPN_TPF_TXNS";
                        TempFileA = "[IPN].[dbo]." + TempFileA;
                        TempFileB = "[IPN].[dbo]." + TempFileB;
                    }


                    if (TempFileA != TempFrom)
                    // & MatchingCateg != "ETI350" & MatchingCateg != "QAH350" & MatchingCateg != "IPN350")
                    {
                        // Update the original file
                        // Means master loaded from other file 
                        // eg QAH410 and QAH420
                        TableId = SourceTable_A;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInOne;
                        Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, TableId, 1, W_Application);
                        if (Mmob.RecordFound == true)
                        {
                            SelectionCriteria = " WHERE MatchingCateg = '" + WMatchingCateg + "'"
                                + " AND RRNumber ='" + Mmob.RRNumber + "'"
                                + " AND TransAmount =" + Mmob.TransAmount
                                ;
                            Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, TempFileA, 1, W_Application);
                            if (Mmob.RecordFound == true)
                            {
                                Mmob.IsMatchingDone = true;
                                Mmob.Matched = false;
                                Mmob.MatchMask = WWMask;
                                Mmob.MatchingAtRMCycle = WRMCycle;
                                if (IsReversal == true)
                                {
                                    Mmob.MatchMask = "1R";
                                    Mmob.Comments = "Record is reversed ";
                                }
                                else
                                {
                                    Mmob.Comments = "";
                                }

                                Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(TempFileA, Mmob.SeqNo);
                            }
                        }

                    }

                    SeqNoInOne = 0;
                    SeqNoInTwo = 0;

                }

                //
                string WCustomerID ;
                string WReference_TPF ;
                //     WCustomerID = Mmob.CustomerID
                if (WWMask == "01")
                {
                    // Initialize 
                    WCustomerID = "";
                    WReference_TPF = "";

                    CountToVisit = CountToVisit + 1;
                    if (WWMask.Substring(1, 1) == "1")
                    {
                        TableId = SourceTable_B;
                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                    }
                    // TableID in full name 
                    Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, TableId, 1, W_Application);

                    if (Mmob.RecordFound == false)
                    {
                        MessageBox.Show("1833 : Check for System Error AND TableId " + TableId + Environment.NewLine
                            + "AND Selection Criteria.." + SelectionCriteria + TableId + Environment.NewLine
                            + "WWMask.. " + WWMask + Environment.NewLine
                            + "Category: " + WReconcCategoryId + Environment.NewLine
                            + LastMatched_Characters + Environment.NewLine
                            + Matched_Characters + Environment.NewLine
                            );
                    }

                    if (Mmob.RRNumber == "00015671-1526-5869-0111-7c1e08206594" || Mmob.RRNumber == "0015b703-e161-44d2-96bc-bc0ef8a0e4f5")
                    {
                        Mmob.RRNumber = Mmob.RRNumber;
                    }
                    //string OriginSourceTable_A = "";
                    //Mmob.RecordFound = true;
                    if (Mmob.RecordFound == true)
                    {
                        // LOOK For Unmatched 
                        int Db_Mode = 1; // look only the unmatched and reversals
                        SelectionCriteria = " WHERE MatchingCateg = '" + WMatchingCateg + "'"
                                + " AND RRNumber ='" + Mmob.RRNumber + "'"
                                + " AND TransAmount =" + Mmob.TransAmount
                                //+ " AND CustomerID ='" + Mmob.CustomerID + "'"
                                + " AND LoadedAtRMCycle >=" + (WRMCycle - 3)
                                //+ " AND SeqNo <>" + SeqNoInTwo  // Not to look for the same record 
                                ;
                        //TableId = SourceTable_A;
                        // Check if done previous cycles 
                        if (WMatchingCateg == "ETI320")
                        {
                            WMatchingCateg = WMatchingCateg; 
                        }
                        Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, SourceTable_A, Db_Mode, W_Application);
                        if (Mmob.RecordFound == true)
                        {
                            // Check if reversal 
                            // Check if done before with Mask 01
                            if (Mmob.Matched == false & Mmob.LoadedAtRMCycle < WRMCycle)
                            {
                                // Previous day we had the record as 01
                                WWMask = "P1";
                                WCustomerID = Mmob.CustomerID; // Get the customer id too
                                WReference_TPF = Mmob.Reference_TPF;
                                Mmob.MatchMask = "1N"; // we update the old record as N1 ... came as Next day 
                                Mmob.Comments = "Matching Record Came next cycle";
                                Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(SourceTable_A, Mmob.SeqNo);
                            }
                            if (Mmob.IsReversal == true & Mmob.LoadedAtRMCycle == WRMCycle)
                            {
                                IsReversal = true;
                                WReference_TPF = Mmob.Reference_TPF;
                            }

                            //WCustomerID = Mmob.CustomerID;
                           
                        }
                        // Read 01 Record again


                        SelectionCriteria = " WHERE SeqNo = " + SeqNoInTwo;

                        // READ AGAIN 
                        Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, SourceTable_B, 1, W_Application);
                        // WE HAVE ALL INFORMATION NEEDED TO CREATE MASTER RECORD
                        if (Mmob.RecordFound == false)
                        {
                            MessageBox.Show("1833 : Check for System Error AND TableId " + TableId + Environment.NewLine
                                + "AND Selection Criteria.." + SelectionCriteria + TableId + Environment.NewLine
                                + "WWMask.. " + WWMask + Environment.NewLine
                                + "Category: " + WReconcCategoryId + Environment.NewLine
                                + LastMatched_Characters + Environment.NewLine
                                + Matched_Characters + Environment.NewLine
                                );
                        }

                        DateTime Found_SET_DATE = NullPastDate;

                        // Find Out the Replenishment Cycle No by using Last Trace Number
                        // Mmob.MatchingAtRMCycle = WRMCycle; 
                        if (Mmob.TransDate == null)
                        {
                            MessageBox.Show("Transaction date is NULL for file:" + SourceTable_B + Environment.NewLine
                                                     + " and RRN " + Mmob.RRNumber);
                            Mmob.TransDate = NullPastDate;
                        }

                        Mmob.IsMatchingDone = true;
                        Mmob.Matched = false;

                        if (IsReversal == true)
                        {
                            WWMask = "R1";
                        }
                        Mmob.MatchMask = WWMask; // it is 01 or P1
                        Mmob.MatchingAtRMCycle = WRMCycle;

                        Mmob.ActionByUser = false;
                        Mmob.UserId = "";
                        Mmob.Authoriser = "";
                        Mmob.AuthoriserDtTm = NullPastDate;
                        Mmob.ActionType = "00";

                        if (WWMask == "P1")
                        {
                            Mmob.Comments = "Record exist previous cycle as not matched as Mask 10 ";
                        }
                        else
                        {
                            // leave it as no comments 
                        }
                        
                        Mmob.SettledRecord = false;
                        //Mmob.Net_TransDate = (DateTime)rdr["Net_TransDate"];
                        //Mmob.Minutes_Date = (int)rdr["Minutes_Date"];
                       
                        Mmob.SET_DATE = WCut_Off_Date;
                        
                        
                        if (WCustomerID != "")
                        {
                            Mmob.CustomerID = WCustomerID; // 
                        }
                        else
                        {
                            Mmob.CustomerID = Mmob.CustomerID; 
                        }

                        if (WReference_TPF != "")
                        {
                            Mmob.Reference_TPF = WReference_TPF;
                        }
                        else
                        {
                            Mmob.Reference_TPF = "Not Available";
                        }

                        
                        int NewSeqNo = 0; 
                        NewSeqNo = Mmob.InsertNewRecordInTable_MOBILE_MASTER(SourceTable_A);
                        if ( NewSeqNo == 0)
                        {
                            // Means we had a cancel 
                            MessageBox.Show("We had a cancel at file:" + SourceTable_B + Environment.NewLine
                                                     + " and RRN " + Mmob.RRNumber);

                            MessageBox.Show("The Transaction date is :" + TransDate.ToString() + Environment.NewLine
                                                     + " ");
                        }

                        TableId = SourceTable_B;

                        Mmob.IsMatchingDone = true;
                        Mmob.Matched = false;
                        Mmob.MatchMask = WWMask;
                        Mmob.MatchingAtRMCycle = WRMCycle;


                        Mmob.Comments = "";


                        Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(TableId, SeqNoInTwo);

                        SeqNoInOne = 0;
                        SeqNoInTwo = 0;

                    }

                }
            }
            catch (Exception ex)
            {

                W_MPComment = W_MPComment + "Process has been cancelled at InsertAndUpdateTablesFromOurAtms_AND_JCC" + Environment.NewLine;

                W_MPComment += DateTime.Now;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                CatchDetails(ex, WWMask);
            }

        }


        int CounterDublInPosOne;
        int CounterDublInPosTwo;
        int CounterDublInPosThree;
        //*****************************************
        // UPDATE Dublicates For Two OR three 
        //***************************************** 
        public void ReadDublicatesAndCompressAndUpdateExceptionsforTables()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            ////if (WMatchingCateg == PRX+"233")
            ////{
            ////    MessageBox.Show("Do Degugging here"); 
            ////}

            TableUnMatched = new DataTable();
            TableUnMatched.Clear();

            TotalSelected2 = 0;

            SqlString =
            " SELECT  *"
            + " FROM [ATMS].[dbo].[WReport97_ForMatching_MOBILE] " // leave it as is 
            + " WHERE M_Type = 1 AND UserId = @UserId AND MatchingCateg ='" + WMatchingCateg + "'"             /* Dublicate  */
            + " ORDER By Matched_Characters, WCase, OriginSeqNo ";

            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableUnMatched);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 247" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "613");
                }

            //  LastTraceNo = 0;
            LastMatched_Characters = "";
            Matched_Characters = "";
            LastTerminalId = "";
            CounterDublInPosOne = 0;
            CounterDublInPosTwo = 0;
            CounterDublInPosThree = 0;
            SeqNoInOne = 0;
            SeqNoInTwo = 0;
            SeqNoInThree = 0;
            int I = 0;
            int K = TableUnMatched.Rows.Count;
            // READ AND UPDATE Mpa
            while (I <= (TableUnMatched.Rows.Count - 1))
            {
                RecordFound = true;

                UserId = (string)TableUnMatched.Rows[I]["UserId"];
                OriginSeqNo = (int)TableUnMatched.Rows[I]["OriginSeqNo"];
                TransDate = (DateTime)TableUnMatched.Rows[I]["TransDate"];
                TableId = (string)TableUnMatched.Rows[I]["FileId"];
                WCase = (string)TableUnMatched.Rows[I]["WCase"];
                M_Type = (int)TableUnMatched.Rows[I]["M_Type"]; // 
                DublInPos = (int)TableUnMatched.Rows[I]["DublInPos"];
                InPos = (int)TableUnMatched.Rows[I]["InPos"];
                NotInPos = (int)TableUnMatched.Rows[I]["NotInPos"];
                RRNumber = (string)TableUnMatched.Rows[I]["RRNumber"];
                //SenderTelephone = (string)TableUnMatched.Rows[I]["SenderTelephone"];

                TransAmount = (decimal)TableUnMatched.Rows[I]["TransAmount"];
                MatchingCateg = (string)TableUnMatched.Rows[I]["MatchingCateg"];
                RMCycle = (int)TableUnMatched.Rows[I]["RMCycle"];
                Matched_Characters = (string)TableUnMatched.Rows[I]["Matched_Characters"];
                FullDtTm = (DateTime)TableUnMatched.Rows[I]["FullDtTm"];
                ResponseCode = (string)TableUnMatched.Rows[I]["ResponseCode"];


                if ((LastMatched_Characters != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim()) || I == K - 1)
                {
                    //if (K == 1) FirstSeqNo = SeqNo;

                    if ((Matched_Characters != "" & Matched_Characters.Trim() != LastMatched_Characters.Trim())) //    
                    {
                        // Do action for previous 

                        // OUR 
                        UpdateDuplicatesInMasterTableForMOBILE();

                        CounterDublInPosOne = 0;
                        CounterDublInPosTwo = 0;
                        CounterDublInPosThree = 0;
                        SeqNoInOne = 0;
                        SeqNoInTwo = 0;
                        SeqNoInThree = 0;

                    }

                    if (I == K - 1) // last record in table 
                    {
                        // It is last of a group with same trace no 
                        // Or the only one 

                        if (DublInPos == 1)
                        {
                            CounterDublInPosOne = CounterDublInPosOne + 1;
                            SeqNoInOne = OriginSeqNo;
                        }

                        if (DublInPos == 2)
                        {
                            CounterDublInPosTwo = CounterDublInPosTwo + 1;
                            SeqNoInTwo = OriginSeqNo;
                        }

                        if (DublInPos == 3)
                        {
                            CounterDublInPosThree = CounterDublInPosThree + 1;
                            SeqNoInThree = OriginSeqNo;
                        }

                        LastMatched_Characters = Matched_Characters;
                        LastTransDate = TransDate;
                        //LastTerminalId = TerminalId;
                        //LastAccNo = AccNo;
                        LastTransAmt = TransAmount;
                        LastType = M_Type;

                        // OUR ATMs
                        UpdateDuplicatesInMasterTableForMOBILE();

                        I++;

                        continue;

                    }

                }

                LastMatched_Characters = Matched_Characters;
                LastTransDate = TransDate;
                // LastTerminalId = TerminalId;
                //LastAccNo = AccNo;
                LastTransAmt = TransAmount;
                LastType = M_Type;

                if (DublInPos == 1)
                {
                    CounterDublInPosOne = CounterDublInPosOne + 1;
                    SeqNoInOne = OriginSeqNo;
                }

                if (DublInPos == 2)
                {
                    CounterDublInPosTwo = CounterDublInPosTwo + 1;
                    SeqNoInTwo = OriginSeqNo;
                }

                if (DublInPos == 3)
                {
                    CounterDublInPosThree = CounterDublInPosThree + 1;
                    SeqNoInThree = OriginSeqNo;
                }


                I++; // Read Next entry of the table 

            }

        }
        //
        // Update MAster Table with dublicates         //
        private void UpdateDuplicatesInMasterTableForMOBILE()
        {
            // UPDATE Mmob (Mobile)
            //
            //string SelectionCriteria2 = "";
            string WSelectionCriteria = "";



            Mmob.RecordFound = false;

            if (SeqNoInOne > 0)
            {
                TableId = SourceTable_A;
                Mmob.ReadTransSpecificFromSpecificTable_By_SeqNo(TableId, SeqNoInOne, 1);
            }

            if (SeqNoInOne == 0 & SeqNoInTwo > 0)
            {
                TableId = SourceTable_B;
                Mmob.ReadTransSpecificFromSpecificTable_By_SeqNo(TableId, SeqNoInTwo, 1);

                TableId = SourceTable_A;
                // LEAVE IT HERE
                if (MatchingCateg == "ETI360" || MatchingCateg == "QAH360" || MatchingCateg == "IPN360")
                {
                    // 
                    WSelectionCriteria = " WHERE RRNumber ='" + Mmob.RRNumber + "' "
                    //+ " AND TransAmount =" + Mmob.TransAmount
                    + " AND  MatchingCateg ='" + Mmob.MatchingCateg + "' ";
                }
                else
                {
                    WSelectionCriteria = " WHERE RRNumber ='" + Mmob.RRNumber + "' "
                    + " AND TransAmount =" + Mmob.TransAmount
                    + " AND  MatchingCateg ='" + Mmob.MatchingCateg + "' ";
                }



                Mmob.ReadTransSpecificFromSpecificTable_By_SelectionCriteria(TableId, WSelectionCriteria, 1);

                if (Mmob.RecordFound == true)
                {
                    SeqNoInOne = Mmob.SeqNo;
                    //TableId = SourceTable_A;
                    //Mmob.ReadTransSpecificFromSpecificTable_By_SeqNo(TableId, SeqNoInOne, 1);
                    //if (Mmob.RecordFound == true)
                    //{

                    //}
                }
                else
                {
                    MessageBox.Show("Message: Master not found");
                    return;
                }

            }
            if (SeqNoInOne == 0 & SeqNoInTwo == 0)
                return;

            char[] WMask = Mmob.MatchMask.ToCharArray();

            //  if (CounterDublInPosOne == 1) WMask[0] = 'X'; // to cover all records in Mpa
            if (CounterDublInPosOne == 2) WMask[0] = '2';
            if (CounterDublInPosOne == 3) WMask[0] = '3';
            if (CounterDublInPosOne == 4) WMask[0] = '4';
            if (CounterDublInPosOne == 5) WMask[0] = '5';
            if (CounterDublInPosOne > 5) WMask[0] = 'X'; // to cover all records in Mpa

            if (CounterDublInPosTwo == 2) WMask[1] = '2';
            if (CounterDublInPosTwo == 3) WMask[1] = '3';
            if (CounterDublInPosTwo == 4) WMask[1] = '4';
            if (CounterDublInPosTwo == 5) WMask[1] = '5';
            if (CounterDublInPosTwo > 5) WMask[1] = 'X';

            if (CounterDublInPosThree == 2) WMask[2] = '2';
            if (CounterDublInPosThree == 3) WMask[2] = '3';
            if (CounterDublInPosThree == 4) WMask[2] = '4';
            if (CounterDublInPosThree == 5) WMask[2] = '5';
            if (CounterDublInPosThree > 5) WMask[2] = 'X';

            if (CounterDublInPosOne > 0 & SeqNoInTwo == 0)
            {
                WMask[1] = '0';
            }
            if (NumberOfTables == 3 & CounterDublInPosOne > 0 & SeqNoInThree == 0)
            {
                WMask[2] = '0';
            }

            string WWMask = new string(WMask);
            // Update Footer 
            if (MatchingCateg == "ETI360" || MatchingCateg == "QAH360" || MatchingCateg == "IPN360")
            {
                // 
                WSelectionCriteria = " WHERE RRNumber ='" + Mmob.RRNumber + "' "
                //+ " AND TransAmount =" + Mmob.TransAmount
                + " AND  MatchingCateg ='" + Mmob.MatchingCateg + "' ";
            }
            else
            {
                WSelectionCriteria = " WHERE RRNumber ='" + Mmob.RRNumber + "' "
                + " AND TransAmount =" + Mmob.TransAmount
                + " AND  MatchingCateg ='" + Mmob.MatchingCateg + "' ";
            }
            // UNMATCHED DUE TO DUBLICATES

            TableId = SourceTable_A;

            Mmob.IsMatchingDone = true;
            Mmob.Matched = false;
            Mmob.MatchMask = WWMask;
            Mmob.MatchingAtRMCycle = WRMCycle;
            Mmob.Comments = "";

            //Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(TableId, SeqNoInOne);
            Mmob.UpdateRecordAsUnmatchedByFilter_MOBILE(TableId, WSelectionCriteria);

            if (SeqNoInTwo > 0)
            {
                TableId = SourceTable_B;

                Mmob.IsMatchingDone = true;
                Mmob.Matched = false;
                Mmob.MatchMask = WWMask;
                Mmob.MatchingAtRMCycle = WRMCycle;
                Mmob.Comments = "";

                Mmob.UpdateRecordAsUnmatchedByFilter_MOBILE(TableId, WSelectionCriteria);
                // Mmob.UpdateRecordAsUnmatchedBySeqNumber_MOBILE(TableId, SeqNoInTwo);
            }


        }

        // Find Discrepancies from Our ATMs Mpa 
        //
        // public DataTable TableUnMatchedCompressed = new DataTable();
        public void ReadMpaAndFindDiscrepanciesfromMatching_Cards(string InMatchingCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableUnMatchedCompressed = new DataTable();
            TableUnMatchedCompressed.Clear();
            TotalSelected = 0;

            SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCateg + "'"
                                    + " AND MatchingAtRMCycle = " + RMCycle
                                    + " AND IsMatchingDone = 1 AND Matched = 0 "
                                    + " Order by TransDate ";
            SqlString =
            " SELECT  SeqNo,  "
            + " CAST(TransDate AS Date) As Date"
            + " , RRNumber ,   Matched,   MatchMask,   SenderTelephone,   TransAmount  "

            //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            + " FROM " + SourceTable_A
            + SelectionCriteria;



            using (SqlConnection conn =
             new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 150;  // seconds
                        sqlAdapt.Fill(TableUnMatchedCompressed);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 248" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "614");
                }

        }





        //
        // READ Working File by Selection Criteria
        //
        //
        //
        //int TSeqNo;
        //string TMatchingCateg;
        //int TRMCycle;
        //string TTerminalId;
        //int TTransType;
        //string TTransDescr;
        //string TCardNumber;
        //string TAccNo;
        //string TTransCurr;
        //decimal TTransAmt;
        //decimal TAmtFileBToFileC;
        //DateTime TTransDate;
        //int TTraceNo;
        //int TRRNumber;

        // Get Next Unique Id 
        static int GetNextValue(string InConnectionString)
        {
            int iResult = 0;

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[usp_GetNextUniqueId]";

            using (SqlConnection conn = new SqlConnection(InConnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        SqlParameter iNextValue = new SqlParameter("@iNextValue", SqlDbType.Int);
                        iNextValue.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(iNextValue);
                        cmd.ExecuteNonQuery();
                        string sResult = cmd.Parameters["@iNextValue"].Value.ToString();
                        int.TryParse(sResult, out iResult);

                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex, "615");
                }
            return iResult;
        }



        public int TotalUnMatchedWithNoAction;
        public int TotalUnMatchedInProcess;
        public int TotalUnMatchedSettled;
        public int TotalUnMatched;

        string RMCateg;

        // Define the data table 
        public DataTable DataTableAtmsTotals = new DataTable();

        // DO THE FINAL UPDATINGS ON ATMs Totals and Reconciliation Session totals 
        public void ReadUnMatchedTxnsMasterPoolATMsTotalsForAtmsAND_UPDATE_OurATMs(string InOperator, int InRMCycle, string InSelectionCriteria)
        {
            int NoOfTxns;
            decimal TotAmount;

            //decimal JournalAmt = 0;
            decimal TotalGroupUnMatchedAmt = 0;
            decimal TotalGroupUnMatchedAmtNotInJournal = 0;
            int TotalGroupUnMatchedTxns = 0;

            decimal TotalGroupMatchedAmt = 0;
            int TotalGroupMatchedTxns = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAtmsTotals = new DataTable();
            DataTableAtmsTotals.Clear();

            TotalUnMatched = 0;
            TotalUnMatchedWithNoAction = 0;
            TotalUnMatchedInProcess = 0;
            TotalUnMatchedSettled = 0;

            SqlString =
               " SELECT RMCateg, MatchingCateg, TerminalId, Count(SeqNo) As NoOfTxns, "
             + " sum(TransAmount) as TotAmount , Matched, NotInJournal "
              //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
              + " FROM " + SourceTable_A
             + InSelectionCriteria
             + " Group by RMCateg, MatchingCateg, TerminalId, Matched, NotInJournal "
             + " order by TerminalId, Matched, NotInJournal ";

            using (SqlConnection conn =
                        new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableAtmsTotals);

                        // Close conn
                        conn.Close();

                        // For two testing ATMs four records in table are created 
                        // Matched and Unmatched for each ATM 

                        int I = 0;

                        while (I <= (DataTableAtmsTotals.Rows.Count - 1))
                        {

                            RecordFound = true;

                            RMCateg = (string)DataTableAtmsTotals.Rows[I]["RMCateg"];
                            MatchingCateg = (string)DataTableAtmsTotals.Rows[I]["MatchingCateg"];
                            TerminalId = (string)DataTableAtmsTotals.Rows[I]["TerminalId"];
                            NoOfTxns = (int)DataTableAtmsTotals.Rows[I]["NoOfTxns"];
                            TotAmount = (decimal)DataTableAtmsTotals.Rows[I]["TotAmount"];
                            bool Matched = (bool)DataTableAtmsTotals.Rows[I]["Matched"];
                            bool NotInJournal = (bool)DataTableAtmsTotals.Rows[I]["NotInJournal"];

                            // ATMS Updating 
                            // Read specific record 
                            Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                                 (WOperator, RMCateg, InRMCycle, TerminalId);
                            if (Ratms.RecordFound)
                            {
                                // Update Record By Adding
                                if (Matched == false)
                                {

                                    Ratms.UnMatchedAmt = Ratms.UnMatchedAmt + TotAmount;
                                    Ratms.NumberOfUnMatchedRecs = Ratms.NumberOfUnMatchedRecs + NoOfTxns;

                                    TotalGroupUnMatchedAmt = TotalGroupUnMatchedAmt + TotAmount;
                                    TotalGroupUnMatchedTxns = TotalGroupUnMatchedTxns + NoOfTxns;
                                    if (NotInJournal == false)
                                    {
                                        // In Journal 
                                        Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                    }

                                    if (NotInJournal == true)
                                    {
                                        TotalGroupUnMatchedAmtNotInJournal = TotalGroupUnMatchedAmtNotInJournal + TotAmount;
                                    }

                                }
                                if (Matched == true)
                                {
                                    if (NotInJournal == false)
                                    {
                                        // In Journal 
                                        Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                    }

                                    Ratms.MatchedAmtAtMatching = Ratms.MatchedAmtAtMatching + TotAmount;
                                    Ratms.NumberOfMatchedRecs = Ratms.NumberOfMatchedRecs + NoOfTxns;

                                    TotalGroupMatchedAmt = TotalGroupMatchedAmt + TotAmount;
                                    TotalGroupMatchedTxns = TotalGroupMatchedTxns + NoOfTxns;
                                }

                                Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(TerminalId, RMCateg, InRMCycle);
                            }
                            else
                            {
                                // Insert New Record with the corresponding values 

                                Ratms.ReconcCategoryId = WReconcCategoryId;
                                //Ratms.MatchingCategoryId = MatchingCateg;

                                //Ratms.MinMaxTrace = MinMaxTrace;
                                Ratms.AtmGroup = WAtmsReconcGroup;

                                Ratms.RMCycle = InRMCycle;
                                Ratms.AtmNo = TerminalId;
                                Ratms.CreatedDtTm = DateTime.Now;
                                Ratms.Currency = WDepCurNm;

                                Ratms.OwnerUserID = WSignedId;

                                Ratms.OpeningBalance = 0;

                                Ratms.JournalAmt = 0;
                                Ratms.MatchedAmtAtMatching = 0;
                                Ratms.MatchedAmtAtDefault = 0;
                                Ratms.MatchedAmtAtWorkFlow = 0;
                                Ratms.NumberOfMatchedRecs = 0;

                                Ratms.UnMatchedAmt = 0;
                                Ratms.NumberOfUnMatchedRecs = 0;
                                Ratms.Operator = WOperator;

                                Ratms.InsertReconcCategATMSRecord(TerminalId, RMCateg, InRMCycle);

                                Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                                (WOperator, RMCateg, InRMCycle, TerminalId);
                                if (Ratms.RecordFound)
                                {
                                    // Update Record By Adding
                                    if (Matched == false)
                                    {

                                        Ratms.UnMatchedAmt = Ratms.UnMatchedAmt + TotAmount;
                                        Ratms.NumberOfUnMatchedRecs = Ratms.NumberOfUnMatchedRecs + NoOfTxns;

                                        TotalGroupUnMatchedAmt = TotalGroupUnMatchedAmt + TotAmount;
                                        TotalGroupUnMatchedTxns = TotalGroupUnMatchedTxns + NoOfTxns;
                                        if (NotInJournal == false)
                                        {
                                            // In Journal 
                                            Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                        }

                                        if (NotInJournal == true)
                                        {
                                            TotalGroupUnMatchedAmtNotInJournal = TotalGroupUnMatchedAmtNotInJournal + TotAmount;
                                        }

                                    }
                                    if (Matched == true)
                                    {
                                        if (NotInJournal == false)
                                        {
                                            // In Journal 
                                            Ratms.JournalAmt = Ratms.JournalAmt + TotAmount;
                                        }

                                        Ratms.MatchedAmtAtMatching = Ratms.MatchedAmtAtMatching + TotAmount;
                                        Ratms.NumberOfMatchedRecs = Ratms.NumberOfMatchedRecs + NoOfTxns;

                                        TotalGroupMatchedAmt = TotalGroupMatchedAmt + TotAmount;
                                        TotalGroupMatchedTxns = TotalGroupMatchedTxns + NoOfTxns;
                                    }

                                    Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(TerminalId, RMCateg, InRMCycle);
                                }

                            }

                            I++; // Read Next entry of the table 

                        }

                        // UPDATE PRESENTER
                        //
                        string WSelectionCriteria = " WHERE "
                            + "  MatchingCateg ='" + MatchingCateg + "' AND  RMCateg ='" + WReconcCategoryId + "'"
                            + " AND MetaExceptionId = 55 AND MatchingAtRMCycle =" + InRMCycle.ToString();
                        int W_DB_Mode = 1;
                        Mpa.ReadTablePoolDataToGetTableByCriteria(WSelectionCriteria, W_DB_Mode);

                        I = 0;

                        while (I <= (Mpa.MpaTable.Rows.Count - 1))
                        {

                            RecordFound = true;

                            string MatchingCateg = (string)Mpa.MpaTable.Rows[I]["MatchingCateg"];
                            string RMCateg = (string)Mpa.MpaTable.Rows[I]["RMCateg"];
                            TerminalId = (string)Mpa.MpaTable.Rows[I]["TerminalId"];
                            decimal Amount = (decimal)Mpa.MpaTable.Rows[I]["TransAmount"];

                            Ratms.ReadReconcCategoriesATMsRMCycleSpecific
                                                (WOperator, RMCateg, InRMCycle, TerminalId);
                            Ratms.Number_Presenter = Ratms.Number_Presenter + 1;
                            Ratms.Amount_Presenter = Ratms.Amount_Presenter + Amount;

                            Ratms.UpdateReconcCategorATMsRMCycleForAtmALLFields(TerminalId, RMCateg, InRMCycle);

                            I++; // Read Next entry of the table 

                        }

                        // At this point Update Rcs
                        // RECONCILIATION CATEGORIES SESSIONS 

                        Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, RMCateg, InRMCycle);

                        //if (RMCateg == "RECATMS-110")
                        //{
                        //    MessageBox.Show("Here we are");
                        //}

                        if (Rcs.RecordFound == false)
                        {
                            Message = "Reconciliation Category not found for : "
                                                                           + WReconcCategoryId;

                            // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);
                            if (ShowMessage == true & Environment.UserInteractive)
                            {
                                System.Windows.Forms.MessageBox.Show(Message);
                            }


                        }

                        Rcs.GlTodaysBalance = Rcs.GlTodaysBalance + TotalGroupMatchedAmt
                                                       + TotalGroupUnMatchedAmt - TotalGroupUnMatchedAmtNotInJournal;
                        Rcs.MatchedTransAmt = Rcs.MatchedTransAmt + TotalGroupMatchedAmt;
                        Rcs.NotMatchedTransAmt = Rcs.NotMatchedTransAmt + TotalGroupUnMatchedAmt;

                        Rcs.NumberOfMatchedRecs = Rcs.NumberOfMatchedRecs + TotalGroupMatchedTxns;
                        Rcs.NumberOfUnMatchedRecs = Rcs.NumberOfUnMatchedRecs + TotalGroupUnMatchedTxns;
                        if (Rcs.NumberOfUnMatchedRecs > 0) Rcs.Difference = true;
                        else Rcs.Difference = false;
                        Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions + TotalGroupUnMatchedTxns;

                        Rcs.UpdateReconcCategorySessionAtMatchingProcess(RMCateg, InRMCycle);

                        // Update Matching Category as passing Matched. 
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat04(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat05(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat06(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat07(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat08(WOperator, InRMCycle, RMCateg, MatchingCateg);
                        Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat09(WOperator, InRMCycle, RMCateg, MatchingCateg);

                        W_MPComment += DateTime.Now + "_" + "Matching Has Been Completed" + Environment.NewLine;
                        W_MPComment += DateTime.Now + "_________________________" + Environment.NewLine;
                        Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(RMCateg, InRMCycle, W_MPComment);

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 251" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "616");

                }

        }

        // DO THE FINAL UPDATINGS Reconciliation Session totals 
        public void ReadUnMatchedTxnsMasterPoolATMsTotals_AND_Update_Mode_2(string InOperator, int InRMCycle, string InSelectionCriteria)
        {
            int NoOfTxns;
            decimal TotAmount;

            DataTableAtmsTotals = new DataTable();
            DataTableAtmsTotals.Clear();

            //decimal JournalAmt = 0;
            decimal TotalUnMatchedAmt = 0;
            int TotalUnMatchedTxns = 0;

            decimal TotalMatchedAmt = 0;
            int TotalMatchedTxns = 0;

            bool Matched;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //  SqlString =
            //  " SELECT RMCateg, MatchingCateg, TerminalId, Count(SeqNo) As NoOfTxns, "
            //+ " sum(TransAmount) as TotAmount , Matched, NotInJournal "
            // //+ " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            // + " FROM " + SourceTable_A
            //+ InSelectionCriteria
            //+ " Group by RMCateg, MatchingCateg, TerminalId, Matched, NotInJournal "
            //+ " order by TerminalId, Matched, NotInJournal ";

            SqlString =
               " SELECT MatchingCateg, Count(SeqNo) As NoOfTxns, "
             + " sum(TransAmount) as TotAmount , Matched "
             + " FROM " + SourceTable_A
             + InSelectionCriteria
             + " Group by MatchingCateg, Matched "
             + " order by Matched ";

            using (SqlConnection conn =
                        new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.Fill(DataTableAtmsTotals);

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 252" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "617");


                }

            // For two testing ATMs four records in table are created 
            // Matched and Unmatched for each 

            // READ TABLE ENTRIES AND UPDATE RECONCILIATION SESSION

            try
            {
                int I = 0;

                while (I <= (DataTableAtmsTotals.Rows.Count - 1))
                {

                    RecordFound = true;

                    MatchingCateg = (string)DataTableAtmsTotals.Rows[I]["MatchingCateg"];
                    NoOfTxns = (int)DataTableAtmsTotals.Rows[I]["NoOfTxns"];
                    TotAmount = (decimal)DataTableAtmsTotals.Rows[I]["TotAmount"];
                    Matched = (bool)DataTableAtmsTotals.Rows[I]["Matched"];

                    if (Matched == false)
                    {
                        TotalUnMatchedAmt = TotalUnMatchedAmt + TotAmount;
                        TotalUnMatchedTxns = TotalUnMatchedTxns + NoOfTxns;
                    }
                    else
                    {
                        TotalMatchedAmt = TotalMatchedAmt + TotAmount;
                        TotalMatchedTxns = TotalMatchedTxns + NoOfTxns;
                    }

                    I++; // Read Next entry of the table 

                }

                // At this point Update Rcs
                // RECONCILIATION CATEGORIES SESSIONS 

                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WReconcCategoryId, InRMCycle);

                if (Rcs.RecordFound == false)
                {
                    Message = "Reconciliation Category not found for : "
                                                                   + WReconcCategoryId;

                    // Pt.InsertPerformanceTrace(WOperator, WOperator, 2, "Matching", WMatchingCateg, DateTime.Now, DateTime.Now, Message);

                    if (ShowMessage == true & Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Message);
                    }

                }

                Rcs.GlTodaysBalance = Rcs.GlTodaysBalance + TotalMatchedAmt
                                               + TotalUnMatchedAmt;
                Rcs.MatchedTransAmt = Rcs.MatchedTransAmt + TotalMatchedAmt;
                Rcs.NotMatchedTransAmt = Rcs.NotMatchedTransAmt + TotalUnMatchedAmt;

                Rcs.NumberOfMatchedRecs = Rcs.NumberOfMatchedRecs + TotalMatchedTxns;
                Rcs.NumberOfUnMatchedRecs = Rcs.NumberOfUnMatchedRecs + TotalUnMatchedTxns;
                if (Rcs.NumberOfUnMatchedRecs > 0) Rcs.Difference = true;
                else Rcs.Difference = false;
                Rcs.RemainReconcExceptions = Rcs.RemainReconcExceptions + TotalUnMatchedTxns;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess(WReconcCategoryId, InRMCycle);

                // Update Matching Category as Matched. ( WReconcCategoryId = MatchingCategory )
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);
                Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, WReconcCategoryId, WMatchingCateg);

                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat01(WOperator, InRMCycle, WReconcCategoryId);
                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat02(WOperator, InRMCycle, WReconcCategoryId);
                //Rcs.UpdateReconcCategorySessionAtOpeningNewCycleCat03(WOperator, InRMCycle, WReconcCategoryId);

                // UPDATE COMMENT
                W_MPComment += DateTime.Now + "_" + "Matching Has Been Competed" + Environment.NewLine;
                W_MPComment += DateTime.Now + "_________________________" + Environment.NewLine;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, InRMCycle, W_MPComment);
            }
            catch (Exception ex)
            {

                W_MPComment = W_MPComment + "Process has been cancelled stage 253" + Environment.NewLine;

                W_MPComment += DateTime.Now;

                Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                CatchDetails(ex, "618");

            }

        }


        //
        // UPDATE Mpa Masked Card with full card Number  
        // 
        public void UpdateMpaWithFullCardNumber_OurATMs_AND_JCC(string InMatchingCateg, int InRMCycle, string InTableA, string InTableB)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionStringATMs))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE t1 "
     + " SET t1.[CardNumber] = t2.CardNumber, t1.[AccNumber] = t2.AccNo "
     + " from " + InTableA + " t1 "
      + " inner join " + InTableB + "  t2 "
     + " on t1.TraceNoWithNoEndZero = t2.TraceNo "
     + " AND t1.TransAmount = t2.TransAmt "
     + " AND t1.TerminalId = t2.TerminalId "
     + " WHERE t1.MatchingCateg = @MatchingCateg AND t1.MatchingAtRMCycle = @MatchingAtRMCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

                        //rows number of record got updated
                        cmd.CommandTimeout = 150;  // seconds
                        rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    W_MPComment = W_MPComment + "Process has been cancelled stage 254" + Environment.NewLine;

                    W_MPComment += DateTime.Now;

                    Rcs.UpdateReconcCategorySessionAtMatchingProcess_MPComment(WReconcCategoryId, WRMCycle, W_MPComment);

                    CatchDetails(ex, "619");
                }
        }

        //
        //// UPDATE Mpa Masked Card with full card Number  
        //// 
        //public void UpdatePrimaryWithFullCardNumber_NOT_OurATMs(string InMatchingCateg, int InRMCycle, string InTableA, string InTableB)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string CommandString =
        //                " UPDATE t1 "
        //                + " SET t1.CardNumber = t2.CardNumber "
        //                + " from " + InTableA + " t1 "
        //                + " inner join " + InTableB + "  t2 "
        //                + " on t1.RRNumber = t2.RRNumber "
        //                + " AND t1.TransAmount = t2.TransAmt "
        //                + " WHERE t1.MatchingCateg = @MatchingCateg AND t1.MatchingAtRMCycle = @MatchingAtRMCycle ";

        //    int rows;

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(CommandString, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
        //                cmd.Parameters.AddWithValue("@MatchingAtRMCycle", InRMCycle);

        //                //rows number of record got updated

        //                rows = cmd.ExecuteNonQuery();

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //}

        // Catch details 
        private static void CatchDetails(Exception ex, string InOrigin)
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



